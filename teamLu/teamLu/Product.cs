using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamLu
{
    /// <summary>
    /// 投资项目的信息：
    /// 项目名字、预期年利率、投资期限、收益模式、投资金额
    /// </summary>
    public class Product
    {
        private string productName;
        private double interestRate;
        private string invertPeriod;
        private string collectionMode;
        private double productAmount;

        public Product(string productName, double interestRate, string invertPeriod, string collectionMode, double productAmount)
        {
            this.productName = productName;
            this.interestRate = interestRate;
            this.invertPeriod = invertPeriod;
            this.collectionMode = collectionMode;
            this.productAmount = productAmount;
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

        public double InterestRate
        {
            get
            {
                return interestRate;
            }

            set
            {
                interestRate = value;
            }
        }

        public string InvertPeriod
        {
            get
            {
                return invertPeriod;
            }

            set
            {
                invertPeriod = value;
            }
        }

        public string CollectionMode
        {
            get
            {
                return collectionMode;
            }

            set
            {
                collectionMode = value;
            }
        }

        public double ProductAmount
        {
            get
            {
                return productAmount;
            }

            set
            {
                productAmount = value;
            }
        }

        
    }
}
