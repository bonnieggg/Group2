using Microsoft.VisualStudio.TestTools.UnitTesting;
using teamLu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace teamLu.Tests
{
    /// <summary>
    /// 陆金所数据抓取处理类的单元测试
    /// </summary>
    [TestClass()]
    public class WebContentTests
    {
     
        [TestMethod()]
        public void getWebContentTest()
        {
            //初始化测试参数URL
            string url = "https://list.lu.com/list/p2p?minMoney=&maxMoney=&minDays=&maxDays=&minRate=&maxRate=&mode=&tradingMode=&isCx=&currentPage=1&orderCondition=&isShared=&canRealized=&productCategoryEnum=";

            WebClient myWebClient = new WebClient();
            myWebClient.Credentials = CredentialCache.DefaultCredentials;

            Byte[] pageData = myWebClient.DownloadData(url);
            string pageHtml = Encoding.UTF8.GetString(pageData);
            
            using (StreamWriter sw = new StreamWriter("D:\\test.html"))
            {
                sw.Write(pageHtml);
            }
            
            Assert.AreNotEqual(pageHtml.Length, 0);
        }

        [TestMethod()]
        public void getWebDataTest()
        {
            //初始化测试参数pageHtml
            string pageHtml = WebContent.getWebContent("https://list.lu.com/list/p2p?minMoney=&maxMoney=&minDays=&maxDays=&minRate=&maxRate=&mode=&tradingMode=&isCx=&currentPage=1&orderCondition=&isShared=&canRealized=&productCategoryEnum="); ;

            StreamWriter swd = new StreamWriter("D:\\data.txt");
            int startIndex = 0;
            while (true)
            {
                startIndex = pageHtml.IndexOf("class=\"product-name\">", startIndex);
                if (startIndex == -1)
                    break;

                Assert.AreNotEqual(startIndex, -1);

                startIndex = pageHtml.IndexOf('>', startIndex);
                startIndex = pageHtml.IndexOf('>', startIndex + 1);
                startIndex++;
                int endIndex = pageHtml.IndexOf("</a>", startIndex);
                Assert.AreNotEqual(endIndex, -1);

                string productName = pageHtml.Substring(startIndex, endIndex - startIndex);

                startIndex = pageHtml.IndexOf("class=\"num-style\">", endIndex);
                Assert.AreNotEqual(startIndex, -1);

                startIndex = pageHtml.IndexOf('>', startIndex) + 1;
                endIndex = pageHtml.IndexOf("</p>", startIndex);
                Assert.AreNotEqual(endIndex, -1);

                string interestRate = pageHtml.Substring(startIndex, endIndex - startIndex);

                startIndex = pageHtml.IndexOf("<p>", endIndex) + 3;
                Assert.AreNotEqual(startIndex, -1);

                endIndex = pageHtml.IndexOf("</p>", startIndex);
                Assert.AreNotEqual(endIndex, -1);
                string investPeriod = pageHtml.Substring(startIndex, endIndex - startIndex).Trim();

                startIndex = pageHtml.IndexOf("class=\"collection-method\">", endIndex) + "class=\"collection-method\">".Length;
                Assert.AreNotEqual(startIndex, -1);
                endIndex = pageHtml.IndexOf("</p>", startIndex);
                Assert.AreNotEqual(endIndex, -1);
                string collectionMode = pageHtml.Substring(startIndex, endIndex - startIndex).Trim();

                startIndex = pageHtml.IndexOf("<em class=\"num-style\">", endIndex) + "<em class=\"num-style\">".Length;
                Assert.AreNotEqual(startIndex, -1);
                endIndex = pageHtml.IndexOf("</em>", startIndex);
                Assert.AreNotEqual(endIndex, -1);
                string productAmount = pageHtml.Substring(startIndex, endIndex - startIndex);

                startIndex = endIndex;


                swd.WriteLine(productName);
                swd.WriteLine(interestRate);
                swd.WriteLine(investPeriod);
                swd.WriteLine(collectionMode);
                swd.WriteLine(productAmount);
                swd.WriteLine();
            }
            swd.Close();
        }
    }
}