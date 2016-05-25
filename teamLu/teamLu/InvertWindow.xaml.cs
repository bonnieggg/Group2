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
using System.Windows.Shapes;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.IO;

namespace teamLu
{
    /// <summary>
    /// InvertWindow.xaml 的交互逻辑
    /// </summary>
    public partial class InvertWindow : System.Windows.Window
    {
        public InvertWindow(string name)
        {
            InitializeComponent();
            projectName.Text = name;
            accountTip.Text = "当前账户余额为：" + MainWindow.Account + "元";
        }

        private void Invert(object sender, RoutedEventArgs e)
        {
            try
            {
                if(moneyTxt.Text == "" || dateTxt.Text == "")
                {
                    MessageBox.Show("请填写完整信息！", "提示");
                    return;
                }
                string name = projectName.Text;
                string money = moneyTxt.Text;
                string date = dateTxt.Text;

                if(Convert.ToDouble(money) > MainWindow.Account)
                {
                    MessageBox.Show("账户余额不足！", "提示");
                    return;
                }

                if (date.IndexOf(" ") != -1)
                    date = date.Substring(0, date.IndexOf(" "));

                string user = MainWindow.User;

                Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                Workbooks wbks = app.Workbooks;
                _Workbook wbk;
                bool first = false;
                try
                {
                    wbk = wbks.Open(user + ".xlsx");
                }
                catch (Exception ex)
                {
                    wbk = wbks.Add(true);
                    first = true;
                }
                Sheets shs = wbk.Sheets;
                _Worksheet wsh = (_Worksheet)shs.Item[1];

                if (first)
                {
                    wsh.Cells[1, 1] = "项目名称";
                    wsh.Cells[1, 2] = "金额";
                    wsh.Cells[1, 3] = "日期";
                    wsh.Cells[1, 4] = "操作";
                    wsh.Cells[1, 5] = "赎回日期";
                }

                int row = wsh.UsedRange.Rows.Count;
                row++;
                wsh.Cells[row, 1] = name;
                wsh.Cells[row, 2] = money;
                wsh.Cells[row, 3] = date;
                wsh.Cells[row, 4] = "购买";

                if (first)
                {
                    wbk.SaveAs(user + ".xlsx", Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                        Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value,
                        Missing.Value, Missing.Value, Missing.Value);
                }
                else
                {
                    wbk.Save();
                }

                wbk.Close(null, null, null);
                wbks.Close();
                app.Quit();
                
                MainWindow.Account -= Convert.ToDouble(money);

                StreamReader sr = new StreamReader("user.txt");
                List<string> lines = new List<string>();
                string tmp;
                while((tmp = sr.ReadLine()) != null)
                {
                    string ret;
                    string[] ss = tmp.Split('#');
                    if(ss[0] == MainWindow.User)
                    {
                        ret = ss[0] + "#" + ss[1] + "#" + ss[2] + "#" + MainWindow.Account + "#" + ss[4];
                    }
                    else
                    {
                        ret = tmp;
                    }
                    lines.Add(ret);
                }
                sr.Close();

                StreamWriter sw = new StreamWriter("user.txt");
                foreach(string s in lines)
                {
                    sw.WriteLine(s);
                }
                sw.Close();

                MessageBox.Show("投资记录已添加！", "成功");
                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show("excel文件被占用，请先关闭该文件", "错误");
            }
        }
    }
}
