using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace teamLu
{
    /// <summary>
    /// 陆金所数据抓取相关操作处理类
    /// </summary>
    public class WebContent
    {
        /// <summary>
        /// 抓取对应url中的所有HTML文本
        /// </summary>
        /// <param name="url">需要抓取数据的网站url</param>
        /// <returns>网站的所有html文本</returns>
        public static string getWebContent(string url)
        {
            WebClient myWebClient = new WebClient();
            myWebClient.Credentials = CredentialCache.DefaultCredentials;

            Byte[] pageData = myWebClient.DownloadData(url);
            string pageHtml = Encoding.UTF8.GetString(pageData);
            Console.Write(pageHtml);
            return pageHtml;
        }

        /// <summary>
        /// 处理getWebContent()方法返回的html代码数据，
        /// 找到网页中所需要收集的数据
        /// </summary>
        /// <param name="pageHtml">所需处理的html文本</param>
        /// <returns>处理完成的所有项目组成的列表</returns>
        public static List<Product> getWebData(string pageHtml)
        {
            List<Product> products = new List<Product>();
            int startIndex = 0;
            while (true)
            {
                startIndex = pageHtml.IndexOf("class=\"product-name\">", startIndex);
                if (startIndex == -1)
                    break;
                startIndex = pageHtml.IndexOf('>', startIndex);
                startIndex = pageHtml.IndexOf('>', startIndex + 1);
                startIndex++;
                int endIndex = pageHtml.IndexOf("</a>", startIndex);
                string productName = pageHtml.Substring(startIndex, endIndex - startIndex);

                startIndex = pageHtml.IndexOf("class=\"num-style\">", endIndex);
                startIndex = pageHtml.IndexOf('>', startIndex) + 1;
                endIndex = pageHtml.IndexOf("</p>", startIndex);
                string interestRate = pageHtml.Substring(startIndex, endIndex - startIndex);

                startIndex = pageHtml.IndexOf("<p>", endIndex) + 3;
                endIndex = pageHtml.IndexOf("</p>", startIndex);
                string investPeriod = pageHtml.Substring(startIndex, endIndex - startIndex).Trim();

                startIndex = pageHtml.IndexOf("class=\"collection-method\">", endIndex) + "class=\"collection-method\">".Length;
                endIndex = pageHtml.IndexOf("</p>", startIndex);
                string collectionMode = pageHtml.Substring(startIndex, endIndex - startIndex).Trim();

                startIndex = pageHtml.IndexOf("<em class=\"num-style\">", endIndex) + "<em class=\"num-style\">".Length;
                endIndex = pageHtml.IndexOf("</em>", startIndex);
                string productAmount = pageHtml.Substring(startIndex, endIndex - startIndex);

                startIndex = endIndex;

                //Product product = new Product(productName, Convert.ToDouble(interestRate), investPeriod, collectionMode, Convert.ToDouble(productAmount));
                Product product = new Product(productName, 1.0, investPeriod, collectionMode, 1.0);
                products.Add(product);
            }
            return products;
        }
    }
}
