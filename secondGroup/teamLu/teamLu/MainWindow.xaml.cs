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

namespace teamLu
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static List<Product> products;

        public MainWindow()
        {
            InitializeComponent();

            string url = "https://list.lu.com/list/p2p?minMoney=&maxMoney=&minDays=&maxDays=&minRate=&maxRate=&mode=&tradingMode=&isCx=&currentPage=1&orderCondition=&isShared=&canRealized=&productCategoryEnum=";
            string htmlString = WebContent.getWebContent(url);
            products = WebContent.getWebData(htmlString);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            displayProduct();
        }

        private void SelectCondition(object sender, RoutedEventArgs e)
        {
            string mindate;
            string maxdate;
            if (minDate.Text.Length != 0)
                mindate = (Convert.ToInt32(minDate.Text) * 30).ToString();
            else
                mindate = "0";
            if (maxDate.Text.Length != 0)
                maxdate = (Convert.ToInt32(maxDate.Text) * 30).ToString();
            else
                maxdate = "";

            string minmoney;
            string maxmoney;
            if (minMoney.Text.Length != 0)
                minmoney = (Convert.ToInt32(minMoney.Text) * 10000).ToString();
            else
                minmoney = "0";
            if (maxMoney.Text.Length != 0)
                maxmoney = (Convert.ToInt32(maxMoney.Text) * 10000).ToString();
            else
                maxmoney = "";

            string mode;
            switch(modeCombo.SelectedIndex)
            {
                case 0:
                    mode = "";
                    break;
                case 1:
                    mode = "MONTHLY_AVERAGE_CAPITAL_PLUS_INTEREST";
                    break;
                case 2:
                    mode = "ONE_TIME_CAPITAL_PLUS_INTEREST";
                    break;
                case 3:
                    mode = "MONTHLY_PAY_INTEREST";
                    break;
                default:
                    mode = "";
                    break;
            }

            string order;
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

            string url = "https://list.lu.com/list/p2p?minMoney=" + minmoney + 
                "&maxMoney=" + maxmoney + "&minDays=" + mindate +
                "&maxDays=" + maxdate + "&minRate=" + minRate.Text + 
                "&maxRate=" + maxRate.Text + "&mode=" + mode + 
                "&tradingMode=&isCx=&currentPage=1&orderCondition=" + order + "&isShared=&canRealized=&productCategoryEnum=";

            LogHelper.WriteLogInfo(typeof(MainWindow), url);

            string htmlString = WebContent.getWebContent(url);
            products = WebContent.getWebData(htmlString);
            lstView.Items.Clear();
            displayProduct();
        }

        private void displayProduct()
        {
            foreach (Product product in products)
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
                h3.Text = "收益方式";
                h3.Margin = new Thickness(0, 10, 50, 0);
                TextBlock h4 = new TextBlock();
                h4.Text = "投资金额";
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
                td3.Text = product.CollectionMode;
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

                Button btn = new Button();
                btn.Content = "投资";
                btn.VerticalAlignment = VerticalAlignment.Top;
                btn.Padding = new Thickness(3);
                child.Children.Add(btn);
                Grid.SetRow(btn, 1);
                Grid.SetColumn(btn, 4);

                grid.Children.Add(child);
                Grid.SetRow(child, 1);

                item.Content = grid;
                lstView.Items.Add(item);
            }
        }
    }
}
