using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SISPSLocal.Classes.Tests
{
    [TestClass]
    public class BasicParsingTests : BaseTestClass
    {
        [TestMethod]
        public void SimpleScholarMate()
        {
            var game = Factory.GetChessGameFromString(SampleGames.ScholarsMate);
        }
    }
}
