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
            var fen = new ChessBoardState("rnbqkbnr/pp1ppppp/8/2p5/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2");       
        }
    }
}
