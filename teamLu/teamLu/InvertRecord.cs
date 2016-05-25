using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamLu
{
    public class InvertRecord : IComparable
    {
        private string productName;
        private double invertAmount;
        private string invertDate;
        private double profit;
        private bool back;
        private string status;
        private string backDate;

        public InvertRecord(string productName, double invertAmount, string invertDate, double profit, bool back, string status, string backDate)
        {
            this.productName = productName;
            this.invertAmount = invertAmount;
            this.invertDate = invertDate;
            this.profit = profit;
            this.back = back;
            this.Status = status;
            this.backDate = backDate;
        }

        public string ProductName
        {
            get
            {
                return productName;
            }

            set
            {
                productName = value;
            }
        }

        public double InvertAmount
        {
            get
            {
                return invertAmount;
            }

            set
            {
                invertAmount = value;
            }
        }

        public string InvertDate
        {
            get
            {
                return invertDate;
            }

            set
            {
                invertDate = value;
            }
        }

        public double Profit
        {
            get
            {
                return profit;
            }

            set
            {
                profit = value;
            }
        }

        public bool Back
        {
            get
            {
                return back;
            }

            set
            {
                back = value;
            }
        }

        public string Status
        {
            get
            {
                return status;
            }

            set
            {
                status = value;
            }
        }

        public string BackDate
        {
            get
            {
                return backDate;
            }

            set
            {
                backDate = value;
            }
        }

        public int CompareTo(object obj)
        {
            InvertRecord o = (InvertRecord)obj;
            DateTimeFormatInfo format = new System.Globalization.DateTimeFormatInfo();
            format.ShortDatePattern = "yyyy/mm/dd";
            return (int)(Convert.ToDateTime(this.InvertDate, format).ToOADate() - Convert.ToDateTime(o.InvertDate, format).ToOADate());
        }
    }
}
