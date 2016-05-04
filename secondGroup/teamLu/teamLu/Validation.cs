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
        public static bool IsExistedUser(string user, string pwd)
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
