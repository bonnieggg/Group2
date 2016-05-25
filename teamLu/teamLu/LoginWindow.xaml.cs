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
using System.IO;

namespace teamLu
{
    /// <summary>
    /// loginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            
        }
        
        private void LoginCheck(object sender, RoutedEventArgs e)
        {
            string user = userTxt.Text;
            string pwd = pwdTxt.Password;
            if(user.Length == 0 || pwd.Length == 0)
            {
                MessageBox.Show("请输入用户名和密码！", "提示");
                LogHelper.WriteLogInfo(typeof(LoginWindow),"没有输入用户名和密码");
                return;
            }
            double account = 0;
            double initMoney = 0;
            string initDate = "";
            bool passCheck = Validation.IsExistedUser(user, pwd, ref account, ref initDate, ref initMoney);
            if (passCheck)
            {
                MainWindow win = new MainWindow(user, account, initDate, initMoney);
                win.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("用户名或密码错误，请重试！", "提示");
            }
        }
        

        private void SignIn(object sender, RoutedEventArgs e)
        {
            Register register = new Register();

            register.ShowDialog();
        }
    }
}
