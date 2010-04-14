using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;

namespace Sokoban
{

    [TestFixture]
    public class CommonFuncTests
    {
        // TEST 1
        // ===============================================================================================

        [Test]
        public void CF_1()  // test of: encoding and decoding
        {
            byte[] buffer = new byte[50];

            Int32 val1 = 100;
            Int64 val2 = 123456789001234;
            int offset = 0;

            // encode
            CommonFunc.BlockCopyEx(buffer, val1, ref offset);
            CommonFunc.BlockCopyEx(buffer, val2, ref offset);

            // decode
            offset = 0;
            Assert.That(CommonFunc.Block2Int32(buffer, ref offset) == val1);
            Assert.That(CommonFunc.Block2Int64(buffer, ref offset) == val2);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void CF_2()  // test of: 
        {
            byte[] buffer = new byte[3];

            Int32 val1 = 100;
            int offset = 0;

            // encode
            CommonFunc.BlockCopyEx(buffer, val1, ref offset);
        }

    }

}
