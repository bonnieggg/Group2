using System;
using System.Collections.Generic;
using System.IO;
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

namespace teamLu
{
    /// <summary>
    /// SettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();
        }

        private void SettingCicked(object sender, RoutedEventArgs e)
        {
            if(initDateTxt.Text == "")
            {
                MessageBox.Show("请选择初始日期！", "提示");
                return;
            }
            

            double initMoney;
            if(initMoneyTxt.Text == "")
            {
                initMoney = 100000;
            }
            else
            {
                initMoney = Convert.ToDouble(initMoneyTxt.Text);
            }

            string initDate = initDateTxt.Text;
            if (initDate.IndexOf(" ") != -1)
                initDate = initDate.Substring(0, initDate.IndexOf(" "));

            MainWindow.InitDate = initDate;
            MainWindow.InitMoney = initMoney;

            StreamReader sr = new StreamReader("user.txt");
            List<string> lines = new List<string>();
            string tmp;
            while ((tmp = sr.ReadLine()) != null)
            {
                string ret;
                string[] ss = tmp.Split('#');
                if (ss[0] == MainWindow.User)
                {
                    ret = ss[0] + "#" + ss[1] + "#" + initMoney + "#" + MainWindow.Account + "#" + initDate;
                }
                else
                {
                    ret = tmp;
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
            this.Close();
        }
    }
}
