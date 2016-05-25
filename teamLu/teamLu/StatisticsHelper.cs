using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace teamLu
{
    /// <summary>
    /// 统计相关类
    /// </summary>
    public class StatisticsHelper
    {
        public static void doStatistics(ref List<InvertRecord> records, ref SortedDictionary<string, double> valueLst,
            ref Dictionary<string, double> profitLst, ref Dictionary<string, double> profitRateLst)
        {
            try
            {
                foreach (InvertRecord i in records)
                {
                    if (valueLst.ContainsKey(i.InvertDate))
                    {
                        valueLst[i.InvertDate] += i.InvertAmount;
                    }
                    else
                    {
                        valueLst.Add(i.InvertDate, i.InvertAmount);
                    }
                    if (profitLst.ContainsKey(i.InvertDate))
                    {
                        profitLst[i.InvertDate] += i.Profit;
                    }
                    else
                    {
                        profitLst.Add(i.InvertDate, i.Profit);
                    }

                    if (i.Back)
                    {
                        if (valueLst.ContainsKey(i.BackDate))
                        {
                            valueLst[i.BackDate] -= i.InvertAmount;
                        }
                        else
                        {
                            valueLst.Add(i.BackDate, -i.InvertAmount);
                        }
                    }
                }

                for (int i = 1; i < valueLst.Count; i++)
                {
                    string key = valueLst.ElementAt(i).Key;
                    double value = valueLst.ElementAt(i).Value;
                    value += valueLst.ElementAt(i - 1).Value;
                    valueLst[key] = value;
                }

                double now = DateTime.Now.ToOADate();

                string k = profitLst.ElementAt(0).Key;
                double v = profitLst.ElementAt(0).Value;

                double r = v / MainWindow.InitMoney / ((int)(now - Convert.ToDateTime(k).ToOADate())) * 365;
                profitRateLst.Add(k, r);

                for (int i = 1; i < profitLst.Count; i++)
                {
                    string key = profitLst.ElementAt(i).Key;
                    double value = profitLst.ElementAt(i).Value;
                    value += profitLst.ElementAt(i - 1).Value;
                    profitLst[key] = value;
                    double rate = value / MainWindow.InitMoney / ((int)(now - Convert.ToDateTime(key).ToOADate())) * 365;
                    profitRateLst.Add(key, rate);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("错误的数据格式！", "错误");
                LogHelper.WriteLogErr(typeof(StatisticsHelper), "修改了表格中的数据，但给了错误的格式！");
                return;
            }
        }
    }
}
