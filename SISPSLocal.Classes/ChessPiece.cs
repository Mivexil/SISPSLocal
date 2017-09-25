using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISPSLocal.Classes
{
    public struct ChessPiece
    {
        public ChessPieceType Type { get; }
        public ChessColor Color { get; }

        private ChessPiece(ChessPieceType type, ChessColor color)
        {
            if ((type == ChessPieceType.None && color != ChessColor.None) ||
                (type != ChessPieceType.None && color == ChessColor.None))
            {
                throw new ArgumentException("Partial piece/color specifications are not allowed");
            }
            this.Type = type;
            this.Color = color;
        }

        private static Dictionary<ChessPiece, char> PieceToFenMapping;
        private static Dictionary<char, ChessPiece> FenToPieceMapping;

        static ChessPiece()
        {
            PieceToFenMapping = new Dictionary<ChessPiece, char>
            {
                { BlackPawn, 'p' },
                { BlackKnight, 'n' },
                { BlackBishop, 'b' },
                { BlackRook, 'r' },
                { BlackQueen, 'q' },
                { BlackKing, 'k' },
                { WhitePawn, 'P' },
                { WhiteKnight, 'N' },
                { WhiteBishop, 'B' },
                { WhiteRook, 'R' },
                { WhiteQueen, 'Q' },
                { WhiteKing, 'K' },
                { None, ' ' }
            };
            FenToPieceMapping = PieceToFenMapping.ToDictionary(x => x.Value, x => x.Key);
        }

        public static ChessPiece GetPieceFromFen(char fenLetter)
        {
            if (!FenToPieceMapping.ContainsKey(fenLetter))
            {
                throw new ArgumentException($"Invalid FEN specifier: {fenLetter}");
            }
            return FenToPieceMapping[fenLetter];
        }

        public char GetFen()
        {
            return PieceToFenMapping[this];
        }

        public override bool Equals(object obj)
        {
            return obj is ChessPiece && Equals((ChessPiece)obj);
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode() * 17 + Color.GetHashCode();
        }

        private bool Equals(ChessPiece cp)
        {
            return Type == cp.Type && Color == cp.Color;
        }

        public override string ToString()
        {
            return GetFen().ToString();
        }        

        public static ChessPiece WhitePawn = new ChessPiece(ChessPieceType.Pawn, ChessColor.White);
        public static ChessPiece BlackPawn = new ChessPiece(ChessPieceType.Pawn, ChessColor.Black);
        public static ChessPiece WhiteKnight = new ChessPiece(ChessPieceType.Knight, ChessColor.White);
        public static ChessPiece BlackKnight = new ChessPiece(ChessPieceType.Knight, ChessColor.Black);
        public static ChessPiece WhiteBishop = new ChessPiece(ChessPieceType.Bishop, ChessColor.White);
        public static ChessPiece BlackBishop = new ChessPiece(ChessPieceType.Bishop, ChessColor.Black);
        public static ChessPiece WhiteRook = new ChessPiece(ChessPieceType.Rook, ChessColor.White);
        public static ChessPiece BlackRook = new ChessPiece(ChessPieceType.Rook, ChessColor.Black);
        public static ChessPiece WhiteQueen = new ChessPiece(ChessPieceType.Queen, ChessColor.White);
        public static ChessPiece BlackQueen = new ChessPiece(ChessPieceType.Queen, ChessColor.Black);
        public static ChessPiece WhiteKing = new ChessPiece(ChessPieceType.King, ChessColor.White);
        public static ChessPiece BlackKing = new ChessPiece(ChessPieceType.King, ChessColor.Black);
        public static ChessPiece None = new ChessPiece(ChessPieceType.None, ChessColor.None);

        public static bool operator==(ChessPiece left, ChessPiece right)
        {
            return left.Equals(right);
        }

        public static bool operator!=(ChessPiece left, ChessPiece right)
        {
            return !left.Equals(right);
        }

        private ChessSquarePosition? GetPositionIfExists(int rank, int file)
        {
            if (rank <= 0 || file <= 0 || rank > 8 || file > 8)
            {
                return null;
            }
            return new ChessSquarePosition(rank, file);
        }
    }
}
