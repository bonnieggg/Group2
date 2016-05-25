using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace teamLu
{
    /// <summary>
    /// EXCEL导出操作类
    /// </summary>
    public class ExcelHelper
    {
        public static void ExportToExcel(string fileName, ref List<InvertRecord> exportRecord)
        {
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbooks wbks = app.Workbooks;
            _Workbook wbk;

            wbk = wbks.Add(true);
            Sheets shs = wbk.Sheets;
            _Worksheet wsh = (_Worksheet)shs.Item[1];

            wsh.Cells[1, 1] = "项目名称";
            wsh.Cells[1, 2] = "投资金额";
            wsh.Cells[1, 3] = "投资日期";
            wsh.Cells[1, 4] = "操作";
            wsh.Cells[1, 5] = "赎回日期";

            for (int i = 0; i < exportRecord.Count; i++)
            {
                wsh.Cells[i + 2, 1] = exportRecord[i].ProductName;
                wsh.Cells[i + 2, 2] = exportRecord[i].InvertAmount;
                wsh.Cells[i + 2, 3] = exportRecord[i].InvertDate;
                if (!exportRecord[i].Back)
                    wsh.Cells[i + 2, 4] = "购买";
                else
                    wsh.Cells[i + 2, 4] = "赎回";
                wsh.Cells[i + 2, 5] = exportRecord[i].BackDate;
            }

            wbk.SaveAs(fileName, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                   Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value,
                   Missing.Value, Missing.Value, Missing.Value);
            wbk.Close(null, null, null);
            wbks.Close();
            app.Quit();

            MessageBox.Show("成功导出记录！", "成功");
        }

        public static bool ImportFromExcel(string fileName, ref List<InvertRecord> records)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application app =
                    new Microsoft.Office.Interop.Excel.Application();
                Workbooks wbks = app.Workbooks;
                Workbook wbk = wbks.Open(fileName);


                Sheets shs = wbk.Sheets;
                Worksheet ws = (Worksheet)shs.Item[1];

                int row = ws.UsedRange.Cells.Rows.Count;

                records.Clear();

                int now = (int)DateTime.Now.ToOADate();

                for (int i = 2; i <= row; i++)
                {
                    Range rng = ws.Cells[1][i];
                    string n = rng.Value2;
                    rng = ws.Cells[2][i];
                    double m = Convert.ToDouble(rng.Value2);
                    rng = ws.Cells[3][i];
                    double d = rng.Value2;

                    rng = ws.Cells[4][i];
                    string status = rng.Value2;
                    bool isBack;
                    if (status.Equals("购买"))
                        isBack = false;
                    else
                        isBack = true;

                    double dayPassed = (double)(now - (int)d);

                    double backDate = 0.0;

                    double p;
                    if (!isBack)
                    {
                        p = m * (1 + (0.084 * dayPassed / 365)) - m;
                        p = Math.Round(p, 2);
                    }
                    else
                    {
                        rng = ws.Cells[5][i];
                        backDate = rng.Value2;
                        int passed = (int)(backDate - d);
                        p = m * (1 + (0.084 * passed / 365)) - m;
                        p = Math.Round(p, 2);
                    }
                    string dd = DateTime.FromOADate(double.Parse(d.ToString())).ToString();
                    dd = dd.Substring(0, dd.IndexOf(" "));

                    string bd;
                    if (isBack)
                    {
                        bd = DateTime.FromOADate(double.Parse(backDate.ToString())).ToString();
                        bd = bd.Substring(0, bd.IndexOf(" "));
                    }
                    else
                        bd = "";

                    records.Add(new InvertRecord(n, m, dd, p, isBack, status, bd));
                }

                records.Sort();

                wbk.Close(null, null, null);
                wbks.Close();
                app.Quit();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
    }
}
