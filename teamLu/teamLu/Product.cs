using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamLu
{
    /// <summary>
    /// 投资项目的信息：
    /// 项目名字、预期年利率、投资期限、项目价值、投资金额
    /// </summary>
    public class Product
    {
        private string productName;
        private double interestRate;
        private string invertPeriod;
        private double collectionCurrency;
        private double productAmount;
        private string detailUrl;

        public Product(string productName, double interestRate, string invertPeriod, double collectionCurrency, double productAmount, string detailUrl)
        {
            this.productName = productName;
            this.interestRate = interestRate;
            this.invertPeriod = invertPeriod;
            this.collectionCurrency = collectionCurrency;
            this.productAmount = productAmount;
            this.DetailUrl = detailUrl;
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

        public double CollectionCurrency
        {
            get
            {
                return collectionCurrency;
            }

            set
            {
                collectionCurrency = value;
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

        public string DetailUrl
        {
            get
            {
                return detailUrl;
            }

            set
            {
                detailUrl = value;
            }
        }
    }
}
