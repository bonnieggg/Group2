using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Threading;
using Microsoft.Office.Interop.Excel;
using System.Windows.Controls.DataVisualization.Charting;
using System.Reflection;
using System.Threading;
using System.Globalization;
using System.Collections;

namespace teamLu
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private static string user;
        private static double account;
        private static string initDate;
        private static double initMoney;

        private const int MAIN_PAGE = 0;
        private const int PANIC_PAGE = 1;

        private static List<Product> products;
        private static int currentPage;
        private static string mindate;
        private static string maxdate;
        private static string minmoney;
        private static string maxmoney;
        private static string minrate;
        private static string maxrate;
        private static string mode;
        private static string order;
        private static int totalPage;
        private static string url;
        
        private static bool firstStart = true;
        private static bool firstStart2 = true;

        private static List<Product> panicProducts;
        private static string timerUrl;
        private static int panicTotalPage;   

        private List<InvertRecord> records = new List<InvertRecord>();
        private SortedDictionary<string, double> valueLst = new SortedDictionary<string, double>(new SecondGroupClass.MyComparer());
        private Dictionary<string, double> profitLst = new Dictionary<string, double>();
        private Dictionary<string, double> profitRateLst = new Dictionary<string, double>();

        private static WorkingTip tip;

        #region
        public MainWindow(string u, double a, string i, double m)
        {
            InitializeComponent();
            User = u;
            Account = a;
            InitDate = i;
            InitMoney = m;

            userName.Text = User;
            currentPage = 1;
            mindate = "";
            maxdate = "";
            minmoney = "";
            maxmoney = "";
            minrate = "";
            maxrate = "";
            mode = "";
            order = "";
            preBtn.IsEnabled = false;
            nextBtn.IsEnabled = false;
            lineChart.DataContext = null;
            profitChart.DataContext = null;
            profitRateChart.DataContext = null;
            totalPage = 1;
            
        }
        #endregion

        #region
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread t = new Thread(new ThreadStart(GetProductsBackground));
                t.Start();
                tip = new WorkingTip();
                tip.Show();
                t.Join();
                tip.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("网络异常，请稍后重试！", "错误");
                LogHelper.WriteLogErr(typeof(MainWindow), "网络异常");
                return;
            }
            accountTxt.Text += Account + "元";

            recordsLst.ItemsSource = null;
            bool isSuccess = ExcelHelper.ImportFromExcel(user + ".xlsx", ref records);
            recordsLst.ItemsSource = records;

            if (isSuccess)
            {
                staBtn.IsEnabled = true;
                exportBtn.IsEnabled = true;
            }
            UpdateButtonState();
            displayProduct(MAIN_PAGE);
        }
        #endregion


        #region
        /// <summary>
        /// 更新上一页-下一页按钮的状态
        /// </summary>
        private void UpdateButtonState()
        {
            if(CurrentPage == 1)
            {
                preBtn.IsEnabled = false;
            }
            else
            {
                preBtn.IsEnabled = true;
            }
            if (CurrentPage == TotalPage)
            {
                nextBtn.IsEnabled = false;
            }
            else
                nextBtn.IsEnabled = true;
        }
        #endregion

        #region
        /// <summary>
        /// 根据条件筛选
        /// </summary>
        private void SelectCondition(object sender, RoutedEventArgs e)
        {
            currentPage = 1;
            try
            {
                if (minDate.Text.Length != 0)
                    mindate = (Convert.ToInt32(minDate.Text) * 30).ToString();
                else
                    mindate = "0";
                if (maxDate.Text.Length != 0)
                    maxdate = (Convert.ToInt32(maxDate.Text) * 30).ToString();
                else
                    maxdate = "";
            
                if (minMoney.Text.Length != 0)
                    minmoney = (Convert.ToDouble(minMoney.Text) * 10000).ToString();
                else
                    minmoney = "0";
                if (maxMoney.Text.Length != 0)
                    maxmoney = (Convert.ToDouble(maxMoney.Text) * 10000).ToString();
                else
                    maxmoney = "";

                if (minRate.Text.Length != 0)
                    minrate = (Convert.ToDouble(minRate.Text)).ToString();
                else
                    minrate = "0";
                if (maxRate.Text.Length != 0)
                    maxrate = (Convert.ToDouble(maxRate.Text)).ToString();
                else
                    maxrate = "";
            }
            catch(Exception ex)
            {
                if(firstStart)
                {
                    firstStart = false;
                    return;
                }
                LogHelper.WriteLogErr(typeof(MainWindow), "输入的信息格式有误！");
                MessageBox.Show("输入的信息格式有误，请重试！");
                return;
            }
            
            switch (modeCombo.SelectedIndex)
            {
                case 0:
                    mode = "";
                    break;
                case 1:
                    mode = "NOW";
                    break;
                case 2:
                    mode = "BID";
                    break;
                default:
                    mode = "";
                    break;
            }

            switch(sortCombo.SelectedIndex)
            {
                case 0:
                    order = "INVEST_RATE_DESC";
                    break;
                case 1:
                    order = "INVEST_RATE_ASC";
                    break;
                default:
                    order = "INVEST_RATE_ASC";
                    break;
            }
            
            try
            {
                Thread t = new Thread(new ThreadStart(GetProductsBackground));
                t.Start();
                tip = new WorkingTip();
                tip.Show();
                t.Join();
                tip.Close();

                lstView.Items.Clear();
                displayProduct(MAIN_PAGE);
                scroller.ScrollToTop();
                UpdateButtonState();
            }
            catch(Exception ex)
            {
                if(firstStart2)
                {
                    firstStart2 = false;
                    return;
                }
                MessageBox.Show("网络异常，请稍后重试！", "错误");
                LogHelper.WriteLogErr(typeof(MainWindow), "网络异常");
                return;
            }
        }
        #endregion

        #region
        /// <summary>
        /// 显示出筛选出来的结果
        /// </summary>
        /// <param name="type">表示当前要显示的结果是主页面的筛选结果还是抢购的筛选结果</param>
        private void displayProduct(int type)
        {
            List<Product> ps;

            if (type == MAIN_PAGE)
                ps = products;
            else
                ps = panicProducts;
            
            if(ps == null || ps.Count == 0)
            {
                if(type == PANIC_PAGE)
                {
                    ListViewItem noneItem = new ListViewItem();
                    TextBlock tip = new TextBlock();
                    tip.Text = "当前没有适合您抢购条件的商品，正在持续搜索中... ...";
                    noneItem.Content = tip;
                    panicLst.Items.Add(noneItem);
                }
                else
                {
                    ListViewItem noneItem = new ListViewItem();
                    TextBlock tip = new TextBlock();
                    tip.Text = "没有符合条件的产品";
                    noneItem.Content = tip;
                    lstView.Items.Add(noneItem);
                    pageTip.Text = "0/0";

                }
                return;
            }
            if(type == PANIC_PAGE)
            {
                CancelPanic(null, null);
            }

            foreach (Product product in ps)
            {
                ListViewItem item = new ListViewItem();
                Grid grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());

                TextBlock tb = new TextBlock();
                tb.Text = product.ProductName;
                grid.Children.Add(tb);


                Grid child = new Grid();
                child.RowDefinitions.Add(new RowDefinition());
                child.RowDefinitions.Add(new RowDefinition());
                child.ColumnDefinitions.Add(new ColumnDefinition());
                child.ColumnDefinitions.Add(new ColumnDefinition());
                child.ColumnDefinitions.Add(new ColumnDefinition());
                child.ColumnDefinitions.Add(new ColumnDefinition());
                child.ColumnDefinitions.Add(new ColumnDefinition());

                TextBlock h1 = new TextBlock();
                h1.Text = "预期年化收益率";
                h1.Margin = new Thickness(0, 10, 50, 0);
                TextBlock h2 = new TextBlock();
                h2.Text = "投资期限";
                h2.Margin = new Thickness(0, 10, 50, 0);
                TextBlock h3 = new TextBlock();
                h3.Text = "项目价值";
                h3.Margin = new Thickness(0, 10, 50, 0);
                TextBlock h4 = new TextBlock();
                h4.Text = "转让金额";
                h4.Margin = new Thickness(0, 10, 50, 0);
                h1.FontSize = 12;
                h1.Foreground = new SolidColorBrush(Colors.Gray);
                h2.FontSize = 12;
                h2.Foreground = new SolidColorBrush(Colors.Gray);
                h3.FontSize = 12;
                h3.Foreground = new SolidColorBrush(Colors.Gray);
                h4.FontSize = 12;
                h4.Foreground = new SolidColorBrush(Colors.Gray);

                child.Children.Add(h1);
                child.Children.Add(h2);
                Grid.SetColumn(h2, 1);
                child.Children.Add(h3);
                Grid.SetColumn(h3, 2);
                child.Children.Add(h4);
                Grid.SetColumn(h4, 3);

                TextBlock td1 = new TextBlock();
                td1.Text = product.InterestRate + "%";
                td1.Margin = new Thickness(0, 10, 50, 20);
                td1.Foreground = new SolidColorBrush(Colors.Red);

                TextBlock td2 = new TextBlock();
                td2.Text = product.InvertPeriod;
                td2.Margin = new Thickness(0, 10, 50, 20);
                td2.Foreground = new SolidColorBrush(Colors.Red);

                TextBlock td3 = new TextBlock();
                td3.Text = product.CollectionCurrency + "元";
                td3.Margin = new Thickness(0, 10, 50, 20);
                td3.FontSize = 14;

                TextBlock td4 = new TextBlock();
                td4.Text = product.ProductAmount + "元起";
                td4.Margin = new Thickness(0, 10, 50, 20);
                td4.Foreground = new SolidColorBrush(Colors.Red);

                child.Children.Add(td1);
                child.Children.Add(td2);
                child.Children.Add(td3);
                child.Children.Add(td4);
                Grid.SetRow(td1, 1);
                Grid.SetRow(td2, 1);
                Grid.SetRow(td3, 1);
                Grid.SetRow(td4, 1);
                Grid.SetColumn(td2, 1);
                Grid.SetColumn(td3, 2);
                Grid.SetColumn(td4, 3);

                if (type == MAIN_PAGE)
                {
                    System.Windows.Controls.Button btn = new System.Windows.Controls.Button();
                    btn.Content = "投资";
                    btn.VerticalAlignment = VerticalAlignment.Top;
                    btn.Padding = new Thickness(3);
                    btn.Click += InvertClicked;

                    child.Children.Add(btn);
                    Grid.SetRow(btn, 1);
                    Grid.SetColumn(btn, 4);
                }

                grid.Children.Add(child);
                Grid.SetRow(child, 1);
                
                item.Content = grid;

                if (type == PANIC_PAGE)
                    item.MouseDoubleClick += WebBrowserPopUp;
                else if (type == MAIN_PAGE)
                    item.MouseDoubleClick += ShowDetail;

                if (type == MAIN_PAGE)
                    lstView.Items.Add(item);
                else
                    panicLst.Items.Add(item);
            }
            if(type == MAIN_PAGE)
                pageTip.Text = currentPage + "/" + totalPage;
        }
        #endregion

        #region
        /// <summary>
        /// 点击投资按钮之后的响应
        /// </summary>
        private void InvertClicked(object sender, RoutedEventArgs e)
        {
            DependencyObject p = ((System.Windows.Controls.Button)sender).Parent;
            p = ((Grid)p).Parent;
            DependencyObject child = VisualTreeHelper.GetChild(p, 0);
            string projectName = ((TextBlock)child).Text;
            InvertWindow invertWin = new InvertWindow(projectName);
            invertWin.ShowDialog();
            accountTxt.Text = "当前账户余额：" + account + "元";
            recordsLst.ItemsSource = null;
            bool isSuccess = ExcelHelper.ImportFromExcel(user + ".xlsx", ref records);
            recordsLst.ItemsSource = records;

            if (isSuccess)
            {
                staBtn.IsEnabled = true;
                exportBtn.IsEnabled = true;
            }
        }
        #endregion

        #region
        /// <summary>
        /// 弹出浏览器窗口
        /// </summary>
        private void WebBrowserPopUp(object sender, RoutedEventArgs e)
        {
            int i = panicLst.SelectedIndex;
            if (i < 0)
                return;
            string url = "https://list.lu.com" + panicProducts[i].DetailUrl;
            System.Diagnostics.Process.Start(url);
        }
        #endregion

        #region
        /// <summary>
        /// 下一页响应事件
        /// </summary>
        private void NextPage(object sender, RoutedEventArgs e)
        {
            currentPage++;
            try
            {
                Thread t = new Thread(new ThreadStart(GetProductsBackground));
                t.Start();
                tip = new WorkingTip();
                tip.Show();
                t.Join();
                tip.Close();

                lstView.Items.Clear();
                displayProduct(MAIN_PAGE);
                scroller.ScrollToTop();
                UpdateButtonState();
            }
            catch(Exception ex)
            {
                MessageBox.Show("网络连接异常，请稍后重试！", "错误");
                LogHelper.WriteLogErr(typeof(MainWindow), "网络异常");
                return;
            }
        }

        /// <summary>
        /// 上一页响应事件
        /// </summary>
        private void PrePage(object sender, RoutedEventArgs e)
        {
            currentPage--;
            try
            {
                Thread t = new Thread(new ThreadStart(GetProductsBackground));
                t.Start();
                tip = new WorkingTip();
                tip.Show();
                t.Join();
                tip.Close();

                lstView.Items.Clear();
                displayProduct(MAIN_PAGE);
                scroller.ScrollToTop();
                UpdateButtonState();
            }
            catch (Exception ex)
            {
                MessageBox.Show("网络连接异常，请稍后重试！", "错误");
                LogHelper.WriteLogErr(typeof(MainWindow), "网络异常");
                return;
            }
        }
        #endregion

        #region
        /// <summary>
        /// 点击下拉框之后响应事件
        /// </summary>
        private void SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                SelectCondition(null, null);
            }
            catch (Exception exn)
            {
            }
        }
        #endregion

        /// <summary>
        /// 点击列表项目的响应事件
        /// </summary>
        private void ShowDetail(object sender, RoutedEventArgs e)
        {
            int index = lstView.SelectedIndex;
            if (index < 0)
                return;
            string url = "https://list.lu.com" + products[index].DetailUrl;
            webDetail.Source = new Uri(url);
            productGrid.Visibility = Visibility.Hidden;
            detailGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 后退按钮响应事件
        /// </summary>
        private void BackClicked(object sender, RoutedEventArgs e)
        {
            productGrid.Visibility = Visibility.Visible;
            detailGrid.Visibility = Visibility.Hidden;
        }

        private DispatcherTimer timer;

        #region
        /// <summary>
        /// 开始抢购按钮响应事件
        /// </summary>
        private void PanicBuying(object sender, RoutedEventArgs e)
        {
            string mind, maxd, minr, maxr, minm, maxm, invertm;
            try
            {
                if (minD.Text.Length != 0)
                    mind = (Convert.ToInt32(minD.Text) * 30).ToString();
                else
                    mind = "0";
                if (maxD.Text.Length != 0)
                    maxd = (Convert.ToInt32(maxD.Text) * 30).ToString();
                else
                    maxd = "";

                if (minM.Text.Length != 0)
                    minm = (Convert.ToDouble(minM.Text) * 10000).ToString();
                else
                    minm = "0";
                if (maxM.Text.Length != 0)
                    maxm = (Convert.ToDouble(maxM.Text) * 10000).ToString();
                else
                    maxm = "";

                if (minR.Text.Length != 0)
                    minr = minR.Text;
                else
                    minr = "0";
                if (maxR.Text.Length != 0)
                    maxr = maxR.Text;
                else
                    maxr = "";
            }
            catch(Exception ex)
            {
                LogHelper.WriteLogErr(typeof(MainWindow), "输入的信息格式有误");
                MessageBox.Show("输入的信息格式有误，请重试！");
                return;
            }

            switch (panicMode.SelectedIndex)
            {
                case 0:
                    invertm = "";
                    break;
                case 1:
                    invertm = "NOW";
                    break;
                case 2:
                    invertm = "BID";
                    break;
                default:
                    invertm = "";
                    break;
            }

            timerUrl = "https://list.lu.com/list/transfer-p2p?minMoney=" + minm +
                "&maxMoney=" + maxm + "&minDays=" + mind +
                "&maxDays=" + maxd + "&minRate=" + minr +
                "&maxRate=" + maxr + "&mode=&tradingMode=" + invertm +
                "&isCx=&currentPage=" + 1 + "&orderCondition=" + "" + "&isShared=&canRealized=&productCategoryEnum=";

            panicBtn.IsEnabled = false;
            cancelBtn.IsEnabled = true;
            minD.IsEnabled = false;
            maxD.IsEnabled = false;
            minM.IsEnabled = false;
            maxM.IsEnabled = false;
            minR.IsEnabled = false;
            maxR.IsEnabled = false;
            panicMode.IsEnabled = false;

            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }

        /// <summary>
        /// 定时器事件
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                string pageHtml = WebContent.getWebContent(timerUrl);
                panicProducts = WebContent.getWebData(pageHtml, ref panicTotalPage);
                panicLst.Items.Clear();
                displayProduct(PANIC_PAGE);
            }
            catch(Exception ex)
            {
                MessageBox.Show("网络异常，请稍后重试！", "错误");
                LogHelper.WriteLogErr(typeof(MainWindow), "网络异常");
                CancelPanic(null, null);
                return;
            }
        }

        /// <summary>
        /// 取消定时器事件
        /// </summary>
        private void CancelPanic(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            panicBtn.IsEnabled = true;
            cancelBtn.IsEnabled = false;
            minD.IsEnabled = true;
            maxD.IsEnabled = true;
            minM.IsEnabled = true;
            maxM.IsEnabled = true;
            minR.IsEnabled = true;
            maxR.IsEnabled = true;
            panicMode.IsEnabled = true;
            panicLst.Items.Clear();
        }
        #endregion

        #region
        /// <summary>
        /// 从指定EXCEL表格导入数据
        /// </summary>
        private void ImportData(object sender, RoutedEventArgs e)
        {
            backBtn.IsEnabled = false;
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            string fileName;
            dialog.Filter = "表格文件|*.xlsx";
            if (dialog.ShowDialog() == true)
            {
                fileName = dialog.FileName;
            }
            else
            {
                return;
            }
            
            recordsLst.ItemsSource = null;

            ExcelHelper.ImportFromExcel(fileName, ref records);

            recordsLst.ItemsSource = records;
            staBtn.IsEnabled = true;
            exportBtn.IsEnabled = true;
        }
        #endregion

        #region
        /// <summary>
        /// 生成各种统计曲线
        /// </summary>
        private void StatisticsClick(object sender, RoutedEventArgs e)
        {
            valueLst.Clear();
            profitLst.Clear();
            profitRateLst.Clear();
            
            StatisticsHelper.doStatistics(ref records, ref valueLst, ref profitLst, ref profitRateLst);

            lineChart.Series.Clear();
            profitChart.Series.Clear();
            profitRateChart.Series.Clear();

            LineSeries ls = new LineSeries();
            
            ls.ItemsSource = valueLst;
            ls.DependentValueBinding = new System.Windows.Data.Binding("Value");
            ls.IndependentValueBinding = new System.Windows.Data.Binding("Key");
            ls.Title = "投资曲线";
            lineChart.Series.Add(ls);

            LineSeries pls = new LineSeries();
            pls.ItemsSource = profitLst;
            pls.DependentValueBinding = new System.Windows.Data.Binding("Value");
            pls.IndependentValueBinding = new System.Windows.Data.Binding("Key");
            pls.Title = "收益曲线";
            profitChart.Series.Add(pls);

            LineSeries rls = new LineSeries();
            rls.ItemsSource = profitRateLst;
            rls.DependentValueBinding = new System.Windows.Data.Binding("Value");
            rls.IndependentValueBinding = new System.Windows.Data.Binding("Key");
            rls.Title = "收益率曲线";
            profitRateChart.Series.Add(rls);

            lineChart.Visibility = Visibility.Visible;
            profitChart.Visibility = Visibility.Visible;
            profitRateChart.Visibility = Visibility.Visible;
        }
        #endregion

        #region
        private void ExportData(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();

            string fileName;
            dialog.Filter = "表格文件|*.xlsx";
            if (dialog.ShowDialog() == true)
            {
                fileName = dialog.FileName;
            }
            else
            {
                return;
            }

            List<InvertRecord> exportRecord = (List<InvertRecord>)recordsLst.ItemsSource;

            
            ExcelHelper.ExportToExcel(fileName,ref exportRecord);
        }
        #endregion

        #region
        private void MakeBack(object sender, RoutedEventArgs e)
        {
            int index = recordsLst.SelectedIndex;
            if (index < 0)
            {
                MessageBox.Show("请选择要赎回的项目！", "提示");
                return;
            }

            if (((List<InvertRecord>)recordsLst.ItemsSource)[index].Back)
            {
                MessageBox.Show("该项目已经赎回！", "提示");
                return;
            }

            MessageBoxResult m = MessageBox.Show("确认赎回？", "提示", MessageBoxButton.YesNo);
            if (m == MessageBoxResult.No)
                return;

            ((List<InvertRecord>)recordsLst.ItemsSource)[index].Back = true;
            List<InvertRecord> renewLst = (List<InvertRecord>)recordsLst.ItemsSource;
            renewLst[index].Back = true;


            try
            {
                string user = MainWindow.User;

                Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                Workbooks wbks = app.Workbooks;
                _Workbook wbk;

                wbk = wbks.Open(user + ".xlsx");
                Sheets shs = wbk.Sheets;
                _Worksheet wsh = (_Worksheet)shs.Item[1];

                int row = 2;

                foreach (InvertRecord i in renewLst)
                {
                    wsh.Cells[row, 1] = i.ProductName;
                    wsh.Cells[row, 2] = i.InvertAmount;
                    wsh.Cells[row, 3] = i.InvertDate;
                    if (i.Back)
                    {
                        Range rng = wsh.Cells[row, 4];
                        string tmp = rng.Value2;
                        wsh.Cells[row, 4] = "赎回";
                        if (!tmp.Equals("赎回"))
                        {
                            string date = DateTime.Now.ToString();
                            wsh.Cells[row, 5] = date.Substring(0, date.IndexOf(" "));
                        }
                    }
                    else
                        wsh.Cells[row, 4] = "购买";
                    row++;
                }

                wbk.Save();
                wbk.Close(null, null, null);
                wbks.Close();
                app.Quit();

                InvertRecord backRecord = ((List<InvertRecord>)recordsLst.ItemsSource)[index];
                account += backRecord.InvertAmount + backRecord.Profit;

                accountTxt.Text = "当前账户余额：" + account + "元";
                StreamReader sr = new StreamReader("user.txt");
                List<string> lines = new List<string>();
                string t;
                while ((t = sr.ReadLine()) != null)
                {
                    string ret;
                    string[] ss = t.Split('#');
                    if (ss[0] == MainWindow.User)
                    {
                        ret = ss[0] + "#" + ss[1] + "#" + ss[2] + "#" + MainWindow.Account + "#" + ss[4];
                    }
                    else
                    {
                        ret = t;
                    }
                    lines.Add(ret);
                }
                sr.Close();

                StreamWriter sw = new StreamWriter("user.txt");
                foreach (string s in lines)
                {
                    sw.WriteLine(s);
                }
                sw.Close();

                MessageBox.Show("赎回成功", "成功");
                recordsLst.ItemsSource = null;
                lineChart.Visibility = Visibility.Hidden;
                profitRateChart.Visibility = Visibility.Hidden;
                profitChart.Visibility = Visibility.Hidden;
                ExcelHelper.ImportFromExcel(user + ".xlsx", ref records);
                recordsLst.ItemsSource = records;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Excel文件被占用，请先关闭后再重试！", "失败");
                LogHelper.WriteLogErr(typeof(MainWindow), "Excel文件被占用");
                return;
            }
        }
        #endregion

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            backBtn.IsEnabled = true;
        }

        private void Setting(object sender, RoutedEventArgs e)
        {
            SettingWindow win = new SettingWindow();
            win.ShowDialog();
        }

        private static void GetProductsBackground()
        {
            try
            {
                string url = WebContent.GetURL();
                string htmlString = WebContent.getWebContent(url);
                products = WebContent.getWebData(htmlString, ref totalPage);

            }
            catch (Exception e)
            {
                MessageBox.Show("网络异常，请稍后重试！", "错误");
                LogHelper.WriteLogErr(typeof(MainWindow), "网络异常");
            }
        }

        public static int CurrentPage
        {
            get
            {
                return currentPage;
            }
        }

        public static string Mindate
        {
            get
            {
                return mindate;
            }
        }

        public static string Maxdate
        {
            get
            {
                return maxdate;
            }
        }

        public static string Minmoney
        {
            get
            {
                return minmoney;
            }
        }

        public static string Maxmoney
        {
            get
            {
                return maxmoney;
            }
        }

        public static string Minrate
        {
            get
            {
                return minrate;
            }
        }

        public static string Maxrate
        {
            get
            {
                return maxrate;
            }
        }

        public static string Mode
        {
            get
            {
                return mode;
            }
        }

        public static string Order
        {
            get
            {
                return order;
            }
        }

        public static int TotalPage
        {
            get
            {
                return totalPage;
            }
        }

        public static string User
        {
            get
            {
                return user;
            }

            set
            {
                user = value;
            }
        }

        public static double Account
        {
            get
            {
                return account;
            }

            set
            {
                account = value;
            }
        }

        public static string InitDate
        {
            get
            {
                return initDate;
            }

            set
            {
                initDate = value;
            }
        }

        public static double InitMoney
        {
            get
            {
                return initMoney;
            }

            set
            {
                initMoney = value;
            }
        }
    }
}