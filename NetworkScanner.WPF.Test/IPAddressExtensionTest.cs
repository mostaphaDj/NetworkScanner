using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkScanner.Models;

namespace NetworkScanner.WPF.Test
{
    [TestClass]
    public class IPAddressExtensionTest
    {
        [TestMethod]
        public void incrementTest()
        {
            IPAddress ipAddress = new IPAddress(new byte[] { 0, 0, 0, 0 });
            int i = 0;

            while (true)
            {
                if (ipAddress.Compare(new IPAddress(new byte[] { 0, 0, 255, 255 })) == 1)
                    break;

                ipAddress.Increment();
                i++;
            }
            Assert.AreEqual(i, 256*256);
            //Assert.AreNotEqual(i, 0xFF_FF_FF_FF);
        }
    }
}
