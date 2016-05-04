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
    /// Register.xaml 的交互逻辑
    /// </summary>
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
        }

        private void TryRegister(object sender, RoutedEventArgs e)
        {
            string user = userTxt.Text;
            string pwd = pwdTxt.Password;
            string confirm = confirmTxt.Password;

            if (user.Length == 0 || pwd.Length == 0 || confirm.Length == 0)
            {
                MessageBox.Show("请填写完整必要信息！", "提示");
                return;
            }
            
            if(!pwd.Equals(confirm))
            {
                MessageBox.Show("两次填写的密码不一样，请重试", "提示");
                return;
            }

            StreamReader sr;
            try
            {
                sr = new StreamReader("user.txt");
            }
            catch (Exception ex)
            {
                sr = new StreamReader(new FileStream("user.txt", FileMode.Create));
                //LogHelper.WriteLogErr(typeof(Register), "用户信息文件不存在，创建它。");
            }

            string s;
            bool alreadyHas = false;
            while((s = sr.ReadLine()) != null)
            {
                string[] u = s.Split('#');
                if(u[0].Equals(user))
                {
                    alreadyHas = true;
                    break;
                }
            }
            sr.Close();
            if(alreadyHas)
            {
                MessageBox.Show("该用户名已存在，请重试", "提示");
                return;
            }
            else
            {
                StreamWriter sw = new StreamWriter(new FileStream("user.txt", FileMode.Append));
                string newUser = "\n" + user + "#" + pwd;
                sw.WriteLine(newUser);
                sw.Close();
                MessageBox.Show("注册成功！请登录", "成功");
                this.Close();
            }
        }
    }
}
