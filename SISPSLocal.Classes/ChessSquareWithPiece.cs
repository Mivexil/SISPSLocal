using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISPSLocal.Classes
{
    public struct ChessSquareWithPiece
    {
        public ChessSquarePosition Position { get; }
        public ChessPiece Piece { get; }

        public ChessSquareWithPiece(ChessSquarePosition position, ChessPiece piece)
        {
            Position = position;
            Piece = piece;
        }

        public override bool Equals(object obj)
        {
            return obj is ChessSquareWithPiece
                && Position.Equals(((ChessSquareWithPiece)obj).Position)
                && Piece.Equals(((ChessSquareWithPiece)obj).Piece);
        }

        public static bool operator ==(ChessSquareWithPiece left, ChessSquareWithPiece right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ChessSquareWithPiece left, ChessSquareWithPiece right)
        {
            return !left.Equals(right);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode() * 17 + Piece.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Position}: {Piece}";
        }
    }
}
