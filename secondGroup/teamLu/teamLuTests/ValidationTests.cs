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
    public class ValidationTests
    {
        [TestMethod()]
        public void IsExistedUserTest()
        {
            string user = "admin";
            string pwd = "admin";

            bool expected = true;
            bool result = Validation.IsExistedUser(user, pwd);

            Assert.AreEqual(expected, result);

            user = "admin";
            pwd = "noadmin";

            expected = false;
            result = Validation.IsExistedUser(user, pwd);

            Assert.AreEqual(expected, result);
        }
    }
}