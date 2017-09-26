// <copyright file="ChessVariation.cs" company="Rolls-Royce plc">
//   Copyright (c) 2017 Rolls-Royce plc
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SISPSLocal.Classes
{
    public struct ChessVariation
    {
        public List<ChessMove> Moves { get; }

        public ChessVariation(IEnumerable<ChessMove> moves)
        {
            Moves = moves.ToList();
        }

        public override string ToString()
        {
            if (!Moves.Any())
            {
                return "";
            }
            var sb = new StringBuilder();
            sb.Append("(");
            if (Moves[0].Color == ChessColor.Black)
            {
                sb.Append(Moves[0].MoveNumber + ". ...");
            }
            else
            {
                sb.Append(Moves[0].MoveNumber + ". ");
            }
            for (var i = 0; i < Moves.Count; i++)
            {
                if (Moves[i].Color == ChessColor.White && i != 0)
                {
                    sb.Append(i / 2 + 1);
                    sb.Append(". ");
                }
                sb.Append(Moves[i].DisplayString);
                sb.Append(" ");
            }
            var str = sb.ToString();
            return str.Trim() + ")";
        }
    }
}