using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISPSLocal.Classes
{
    public struct ChessBoard
    {
        public ReadOnlyCollection<ChessSquareWithPiece> Squares { get; }
        private Dictionary<ChessPiece, ReadOnlyCollection<ChessSquareWithPiece>> piecePositions;

        public ChessBoard(string fen)
        {
            Squares = ParsePosition(fen).AsReadOnly();
            piecePositions = BuildPositionDictionary(Squares);
        } 

        public ChessBoard(IEnumerable<ChessSquareWithPiece> squares)
        {
            Squares = squares.ToList().AsReadOnly();
            piecePositions = BuildPositionDictionary(Squares);
        }

        public ReadOnlyCollection<ChessSquareWithPiece> GetPiecePositions(ChessPiece piece)
        {
            return piecePositions.ContainsKey(piece) ? piecePositions[piece] : new List<ChessSquareWithPiece>().AsReadOnly();
        }

        private static Dictionary<ChessPiece, ReadOnlyCollection<ChessSquareWithPiece>> BuildPositionDictionary(ReadOnlyCollection<ChessSquareWithPiece> squares)
        {
            var result = new Dictionary<ChessPiece, List<ChessSquareWithPiece>>();
            foreach (var square in squares)
            {
                if (!result.ContainsKey(square.Piece))
                {
                    result[square.Piece] = new List<ChessSquareWithPiece>();
                }
                result[square.Piece].Add(square);
            }
            return result.ToDictionary(x => x.Key, x => x.Value.AsReadOnly());
        }

        private static List<ChessSquareWithPiece> ParsePosition(string fen)
        {
            var splitFen = UnrollFen(fen);
            if (splitFen.Count != 8)
            {
                throw new ArgumentException($"Invalid FEN notation: expected 8 ranks, found {splitFen.Count}");
            }
            var result = new List<ChessSquareWithPiece>();
            for (int i = 0; i < 8; i++)
            {
                if (splitFen[i].Length != 8)
                {
                    throw new ArgumentException($"Invalid FEN notation: expected 8 files in rank {8 - i}, found {splitFen[i].Length}");
                }
                for (int j = 0; j < 8; j++)
                {
                    result.Add(new ChessSquareWithPiece(new ChessSquarePosition(i * 8 + j), ChessPiece.GetPieceFromFen(splitFen[i][j])));
                }
            }
            return result;
        }

        private static List<string> UnrollFen(string fen)
        {
            var cleanedFen = fen.Split(' ')[0];
            var fenLines = cleanedFen.Split('/');
            var substitutedFen = new List<string>();
            foreach (var line in fenLines)
            {
                string l = line;
                for (int i = 0; i <= 8; i++)
                {
                    l = l.Replace(i.ToString(), new string(' ', i));
                }
                substitutedFen.Add(l);
            }
            return substitutedFen;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Squares.Count; i++)
            {
                sb = sb.Append(Squares[i].Piece);
                if (i % 8 == 7 && i != Squares.Count - 1)
                {
                    sb = sb.Append("/");
                }
            }
            sb = sb.Replace("        ", "8")
                .Replace("       ", "7")
                .Replace("      ", "6")
                .Replace("     ", "5")
                .Replace("    ", "4")
                .Replace("   ", "3")
                .Replace("  ", "2")
                .Replace(" ", "1");
            return sb.ToString();
        }
    }
}
