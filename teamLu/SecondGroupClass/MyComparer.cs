using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondGroupClass
{
    public class MyComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            DateTimeFormatInfo format = new System.Globalization.DateTimeFormatInfo();
            format.ShortDatePattern = "yyyy/mm/dd";
            return (int)(Convert.ToDateTime(x, format).ToOADate() - Convert.ToDateTime(y, format).ToOADate());
        }
    }
}
