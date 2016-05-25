using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace teamLu
{
    public class Validation
    {
        public static bool IsExistedUser(string user, string pwd, ref double account, ref string initDate, ref double initMoney)
        {
            bool isExisted = false;
            StreamReader sr;
            try
            {
                sr = new StreamReader("user.txt");
            }
            catch (Exception e)
            {
                LogHelper.WriteLogErr(typeof(Validation), "没有找到用户信息文件！");
               
                return false;
            }
            string s;
            while ((s = sr.ReadLine()) != null)
            {
                string[] info = s.Split('#');
                if (info[0].Equals(user) && info[1].Equals(pwd))
                {
                    isExisted = true;
                    account = Convert.ToDouble(info[3]);
                    initDate = info[4];
                    initMoney = Convert.ToDouble(info[2]);
                    break;
                }
                else
                    continue;
            }
            sr.Close();
            if (isExisted)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
