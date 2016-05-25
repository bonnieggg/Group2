using Microsoft.VisualStudio.TestTools.UnitTesting;
using teamLu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamLu.Tests
{
    [TestClass()]
    public class WebContentTests
    {
        [TestMethod()]
        public void getWebContentTest()
        {
            string url = "http://www.lu.com";
            int errorLength = 0;
            int actualLength = WebContent.getWebContent(url).Length;
            Assert.AreNotEqual(errorLength, actualLength);
        }
    }
}