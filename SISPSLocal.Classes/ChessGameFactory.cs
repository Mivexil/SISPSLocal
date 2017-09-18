using ilf.pgn;
using ilf.pgn.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISPSLocal.Classes
{
    public class ChessGameFactory
    {
        private PgnReader reader = new PgnReader();
        
        public Game GetRawGameFromString(string game)
        {
            return reader.ReadFromString(game).Games.First();
        }
    }
}
