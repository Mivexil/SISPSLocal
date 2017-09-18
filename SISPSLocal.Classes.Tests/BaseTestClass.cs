using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISPSLocal.Classes.Tests
{
    [TestClass]
    public class BaseTestClass
    {
        protected ChessGameFactory Factory;

        [TestInitialize]
        public void TestInitialize()
        {
            Factory = new ChessGameFactory();
        }
    }
}
