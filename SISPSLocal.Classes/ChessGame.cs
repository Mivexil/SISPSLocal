using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ilf.pgn;
using System.Collections.ObjectModel;

namespace SISPSLocal.Classes
{
    public struct ChessGame
    {
        public ReadOnlyCollection<ChessMove> Moves;

        public ChessGame(IEnumerable<ChessMove> moves)
        {
            Moves = moves.ToList().AsReadOnly();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Moves.Count; i++)
            {
                if (Moves[i].Color == ChessColor.White)
                {
                    sb.Append((i / 2) + 1);
                    sb.Append(". ");                    
                }
                sb.Append(Moves[i].DisplayString);
                sb.Append(" ");
            }
            return sb.ToString();
        }
    }
}
