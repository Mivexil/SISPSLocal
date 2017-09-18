using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISPSLocal.Classes
{
    public struct ChessBoardState
    {
        public ReadOnlyCollection<ChessSquareWithPiece> BoardState { get; }

        public ChessCastles AllowedCastles { get; }

        public int MoveCounter { get; }
        public int HalfMoveClock { get; }
        public bool WhiteToMove { get; } 
        
        public ChessSquarePosition? EnPassantSquare { get; }

        public static ChessBoardState InitialState
        {
            get { return new ChessBoardState("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"); }
        }

        public ChessBoardState(string fen)
        {
            if (fen == null) throw new ArgumentNullException(nameof(fen));
            var splitFen = fen.Split(' ');
            if (splitFen.Length != 6)
            {
                throw new ArgumentException($"Invalid FEN notation: expected 6 fields, found {splitFen.Length}");
            }
            BoardState = ParsePosition(splitFen[0]).AsReadOnly();
            WhiteToMove = ParseIsWhiteToMove(splitFen[1]);
            AllowedCastles = ParseCastles(splitFen[2]);
            EnPassantSquare = ParseEnPassant(splitFen[3]);
            int halfMoveClock;
            if (!int.TryParse(splitFen[4], out halfMoveClock) || halfMoveClock > 50 || halfMoveClock < 0)
            {
                throw new ArgumentException($"Invalid FEN notation: {splitFen[4]} is not a valid halfmove clock value");
            }
            HalfMoveClock = halfMoveClock;
            int moveCounter;
            if (!int.TryParse(splitFen[5], out moveCounter) || moveCounter < 0)
            {
                throw new ArgumentException($"Invalid FEN notation: {splitFen[5]} is not a valid move counter value");
            }
            MoveCounter = moveCounter;
        }

        

        private static ChessSquarePosition? ParseEnPassant(string fenPart)
        {
            if (fenPart == "-") return null;
            return new ChessSquarePosition(fenPart);
        }

        private static ChessCastles ParseCastles(string fenPart)
        {
            if (fenPart == "-") return ChessCastles.None;
            if (fenPart.Any(ch => ch != 'K' && ch != 'Q' && ch != 'k' && ch != 'q') ||
                fenPart.Count(ch => ch == 'K') > 1 ||
                fenPart.Count(ch => ch == 'Q') > 1 ||
                fenPart.Count(ch => ch == 'k') > 1 ||
                fenPart.Count(ch => ch == 'q') > 1)
            {
                throw new ArgumentException($"Invalid FEN notation: {fenPart} is not a valid castling specifier");
            }
            var result = ChessCastles.None;
            if (fenPart.Contains('K')) result |= ChessCastles.WhiteKingside;
            if (fenPart.Contains('Q')) result |= ChessCastles.WhiteQueenside;
            if (fenPart.Contains('k')) result |= ChessCastles.BlackKingside;
            if (fenPart.Contains('q')) result |= ChessCastles.BlackQueenside;
            return result;
        }

        private static bool ParseIsWhiteToMove(string fenPart)
        {
            if (fenPart != "w" && fenPart != "b")
            {
                throw new ArgumentException($"Invalid FEN notation: {fenPart} is not a valid player on move specifier");
            }
            return fenPart == "w";
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
            return $"{PositionFen} {ToMoveFen} {CastlesFen} {EnPassantFen} {HalfMoveClock} {MoveCounter}";
        }

        private string PositionFen
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < BoardState.Count; i++)
                {
                    sb = sb.Append(BoardState[i].Piece);
                    if (i % 8 == 7 && i != BoardState.Count - 1)
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

        private string CastlesFen
        {
            get
            {
                if (AllowedCastles == ChessCastles.None) return "-";
                StringBuilder sb = new StringBuilder();
                if (AllowedCastles.HasFlag(ChessCastles.WhiteKingside)) sb = sb.Append("K");
                if (AllowedCastles.HasFlag(ChessCastles.WhiteQueenside)) sb = sb.Append("Q");
                if (AllowedCastles.HasFlag(ChessCastles.BlackKingside)) sb = sb.Append("k");
                if (AllowedCastles.HasFlag(ChessCastles.BlackQueenside)) sb = sb.Append("q");
                return sb.ToString();
            }
        }

        private string ToMoveFen
        {
            get
            {
                return WhiteToMove ? "w" : "b";
            }
        }

        private string EnPassantFen
        {
            get
            {
                return EnPassantSquare == null ? "-" : EnPassantSquare.Value.ToString();
            }
        }
    }
}
