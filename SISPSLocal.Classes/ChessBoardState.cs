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
        public ChessBoard Board { get; }
        public ChessCastles AllowedCastles { get; }
        public int MoveCounter { get; }
        public int HalfMoveClock { get; }
        public bool WhiteToMove { get; }         
        public ChessSquarePosition? EnPassantSquare { get; }

        public static ChessBoardState InitialState
        {
            get { return new ChessBoardState("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"); }
        }

        public ChessBoardState(ChessBoard board,
            ChessCastles allowedCastles,
            int moveCounter,
            int halfMoveClock,
            bool whiteToMove,
            ChessSquarePosition? enPassantSquare)
        {
            Board = board;
            AllowedCastles = allowedCastles;
            MoveCounter = moveCounter;
            HalfMoveClock = halfMoveClock;
            WhiteToMove = whiteToMove;
            EnPassantSquare = enPassantSquare;
        }

        public ChessSquareWithPiece GetPieceAt(ChessSquarePosition position)
        {
            return Board.Squares[position.Index];
        }

        public ReadOnlyCollection<ChessSquareWithPiece> GetPiecePositions(ChessPiece piece)
        {
            return Board.GetPiecePositions(piece);
        }

        public List<ChessMove> GetLegalMoves(bool disregardChecks = false)
        {
            var moves = new List<ChessMove>();
            moves.AddRange(GetLegalPawnMoves());
            moves.AddRange(GetLegalKnightMoves());
            moves.AddRange(GetLegalBishopMoves());
            moves.AddRange(GetLegalRookMoves());
            moves.AddRange(GetLegalQueenMoves());
            moves.AddRange(GetLegalKingMoves());
            moves.AddRange(GetLegalCastles());
            if (disregardChecks)
            {
                return moves;
            }
            else
            {
                return moves.Where(x => !x.StateAfterMove.CanKingBeCaptured()).ToList();
            }
        }

        public bool CanKingBeCaptured()
        {
            var king = WhiteToMove ? ChessPiece.BlackKing : ChessPiece.WhiteKing;
            var position = Board.GetPiecePositions(king).First().Position;
            return CanPieceOnSquareBeCaptured(position);
        }

        public bool CanPieceOnSquareBeCaptured(ChessSquarePosition position)
        {
            foreach (var move in GetLegalMoves(true))
            {
                if (move.DestinationSquare == position)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsCheck
        {
            get
            {
                var invertedState = new ChessBoardState(Board, AllowedCastles, MoveCounter, HalfMoveClock, !WhiteToMove, EnPassantSquare);
                return invertedState.CanKingBeCaptured();
            }
        }

        public bool IsStalemate
        {
            get
            {
                return !IsCheck && !GetLegalMoves().Any();
            }
        }

        public bool IsMate
        {
            get
            {
                return IsCheck && !GetLegalMoves().Any();
            }
        }

        private List<ChessSquarePosition> GetMoveSquares(ChessColor pieceColor,
            ChessSquarePosition initialPosition,
            int rankModifier,
            int fileModifier)
        {
            var result = new List<ChessSquarePosition>();
            var currentRank = initialPosition.Rank + rankModifier;
            var currentFile = initialPosition.File + fileModifier;
            while (currentRank >= 1 && currentRank <= 8 && currentFile >= 1 && currentFile <= 8)
            {
                var piece = GetPieceAt(new ChessSquarePosition(currentRank, currentFile));
                if (piece.Piece.Color == pieceColor)
                {
                    break;
                }
                else if (piece.Piece != ChessPiece.None)
                {
                    result.Add(new ChessSquarePosition(currentRank, currentFile));
                    break;
                }
                else
                {
                    result.Add(new ChessSquarePosition(currentRank, currentFile));
                }
                currentRank += rankModifier;
                currentFile += fileModifier;
            }
            return result;
        }

        private List<ChessMove> GetLegalCastles()
        {
            var testedCastles = WhiteToMove ? new[] { ChessCastles.WhiteKingside, ChessCastles.WhiteQueenside } :
                new[] { ChessCastles.BlackKingside, ChessCastles.BlackQueenside };
            var kingSource = WhiteToMove ? new ChessSquarePosition("e1") : new ChessSquarePosition("e8");                 
            var result = new List<ChessMove>();
            foreach (var castle in testedCastles)
            {
                if (AllowedCastles.HasFlag(castle))
                {
                    var rookSource = castle == ChessCastles.BlackKingside ? new ChessSquarePosition("h8") :
                                     castle == ChessCastles.BlackQueenside ? new ChessSquarePosition("a8") :
                                     castle == ChessCastles.WhiteKingside ? new ChessSquarePosition("h1") :
                                     new ChessSquarePosition("a1");
                    var rookTarget = castle == ChessCastles.BlackKingside ? new ChessSquarePosition("f8") :
                                     castle == ChessCastles.BlackQueenside ? new ChessSquarePosition("d8") :
                                     castle == ChessCastles.WhiteKingside ? new ChessSquarePosition("f1") :
                                     new ChessSquarePosition("d1");
                    var kingTarget = castle == ChessCastles.BlackKingside ? new ChessSquarePosition("g8") :
                                     castle == ChessCastles.BlackQueenside ? new ChessSquarePosition("c8") :
                                     castle == ChessCastles.WhiteKingside ? new ChessSquarePosition("g1") :
                                     new ChessSquarePosition("c1");
                    var flyOverSquare = castle == ChessCastles.BlackQueenside ? new ChessSquarePosition("b8") :
                                        castle == ChessCastles.WhiteQueenside ? new ChessSquarePosition("b1") :
                                        (ChessSquarePosition?)null;
                    if (GetPieceAt(rookSource).Piece == (WhiteToMove ? ChessPiece.WhiteRook : ChessPiece.BlackRook) &&
                        GetPieceAt(rookTarget).Piece == ChessPiece.None &&
                        GetPieceAt(kingTarget).Piece == ChessPiece.None &&
                        (flyOverSquare == null || GetPieceAt(flyOverSquare.Value).Piece == ChessPiece.None))
                    {
                        var invertedState = new ChessBoardState(Board, AllowedCastles, MoveCounter, HalfMoveClock, !WhiteToMove, EnPassantSquare);
                        if (!invertedState.CanPieceOnSquareBeCaptured(rookTarget))
                        {
                            result.Add(new ChessMove(this, castle));
                        }
                    }
                }
            }
            return result;
        }

        private List<ChessMove> GetLegalKingMoves()
        {
            var result = new List<ChessMove>();
            var positions = GetPiecePositions(WhiteToMove ? ChessPiece.WhiteKing : ChessPiece.BlackKing);
            foreach (var king in positions)
            {
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (!(i == 0 && j == 0))
                        {
                            var square = GetMoveSquares(WhiteToMove ? ChessColor.White : ChessColor.Black,
                                king.Position,
                                i, j).FirstOrDefault();
                            if (square != null)
                            {
                                result.Add(new ChessMove(this, king.Position, square, ChessPiece.None));
                            }                           
                        }
                    }
                }
            }
            return result;
        }

        private List<ChessMove> GetLegalQueenMoves()
        {
            var result = new List<ChessMove>();
            var positions = GetPiecePositions(WhiteToMove ? ChessPiece.WhiteQueen : ChessPiece.BlackQueen);
            foreach (var queen in positions)
            {
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (!(i == 0 && j == 0))
                        {
                            var squares = GetMoveSquares(WhiteToMove ? ChessColor.White : ChessColor.Black,
                                queen.Position,
                                i, j);
                            var self = this;
                            result.AddRange(squares.Select(sq => new ChessMove(self, queen.Position, sq, ChessPiece.None)));
                        }
                    }
                }
            }
            return result;
        }

        private List<ChessMove> GetLegalRookMoves()
        {
            var result = new List<ChessMove>();
            var positions = GetPiecePositions(WhiteToMove ? ChessPiece.WhiteRook : ChessPiece.BlackRook);
            foreach (var rook in positions)
            {
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if ((i == 0 || j == 0) && !(i == 0 && j == 0))
                        {
                            var squares = GetMoveSquares(WhiteToMove ? ChessColor.White : ChessColor.Black,
                                rook.Position,
                                i, j);
                            var self = this;
                            result.AddRange(squares.Select(sq => new ChessMove(self, rook.Position, sq, ChessPiece.None)));
                        }
                    }
                }
            }
            return result;
        }

        private List<ChessMove> GetLegalBishopMoves()
        {
            var result = new List<ChessMove>();
            var positions = GetPiecePositions(WhiteToMove ? ChessPiece.WhiteBishop : ChessPiece.BlackBishop);
            foreach (var bishop in positions)
            {
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i != 0 && j != 0)
                        {
                            var squares = GetMoveSquares(WhiteToMove ? ChessColor.White : ChessColor.Black,
                                bishop.Position,
                                i, j);
                            var self = this;
                            result.AddRange(squares.Select(sq => new ChessMove(self, bishop.Position, sq, ChessPiece.None)));
                        }
                    }
                }
            }
            return result;
        }

        private List<ChessMove> GetLegalKnightMoves()
        {
            var result = new List<ChessMove>();
            var positions = GetPiecePositions(WhiteToMove ? ChessPiece.WhiteKnight : ChessPiece.BlackKnight);
            foreach (var knight in positions)
            {
                for (int i = -1; i <= 1; i+= 2)
                {
                    for (int j = -1; j <= 1; j += 2)
                    {
                        var rankA = knight.Position.Rank + i;
                        var rankB = knight.Position.Rank + i * 2;
                        var fileA = knight.Position.File + j * 2;
                        var fileB = knight.Position.File + j;
                        if (rankA >= 1 && rankA <= 8 && fileA >= 1 && fileA <= 8)
                        {
                            var positionA = new ChessSquarePosition(rankA, fileA);
                            if (GetPieceAt(positionA).Piece.Color != (WhiteToMove ? ChessColor.White : ChessColor.Black))
                            {
                                result.Add(new ChessMove(this, knight.Position, positionA, ChessPiece.None));
                            }
                        }
                        if (rankB >= 1 && rankB <= 8 && fileB >= 1 && fileB <= 8)
                        {
                            var positionB = new ChessSquarePosition(rankB, fileB);
                            if (GetPieceAt(positionB).Piece.Color != (WhiteToMove ? ChessColor.White : ChessColor.Black))
                            {
                                result.Add(new ChessMove(this, knight.Position, positionB, ChessPiece.None));
                            }
                        }
                    }
                }
            }
            return result;
        }

        private List<ChessMove> GetLegalPawnMoves()
        {
            var result = new List<ChessMove>();
            var positions = GetPiecePositions(WhiteToMove ? ChessPiece.WhitePawn : ChessPiece.BlackPawn);
            var modifier = WhiteToMove ? +1 : -1;
            foreach (var pawn in positions)
            {
                var newRank = pawn.Position.Rank + modifier;
                if (newRank >= 2 && newRank <= 7)
                {
                    var newPosition = new ChessSquarePosition(newRank, pawn.Position.File);
                    if (GetPieceAt(newPosition).Piece == ChessPiece.None)
                    {
                        result.Add(new ChessMove(this, pawn.Position, newPosition, ChessPiece.None));
                    }
                }
                else if ((newRank == 1 && !WhiteToMove) || (newRank == 8 && WhiteToMove))
                {
                    var newPosition = new ChessSquarePosition(newRank, pawn.Position.File);
                    if (GetPieceAt(newPosition).Piece == ChessPiece.None)
                    {
                        result.Add(new ChessMove(this, pawn.Position, newPosition, WhiteToMove ? ChessPiece.WhiteQueen : ChessPiece.BlackQueen));
                        result.Add(new ChessMove(this, pawn.Position, newPosition, WhiteToMove ? ChessPiece.WhiteRook : ChessPiece.BlackRook));
                        result.Add(new ChessMove(this, pawn.Position, newPosition, WhiteToMove ? ChessPiece.WhiteBishop : ChessPiece.BlackBishop));
                        result.Add(new ChessMove(this, pawn.Position, newPosition, WhiteToMove ? ChessPiece.WhiteKnight : ChessPiece.BlackKnight));
                    }
                }
                result.AddRange(GetLegalPawnDoubleMove(pawn));
                result.AddRange(GetLegalPawnCaptures(pawn));
            }
            return result;
        }

        private List<ChessMove> GetLegalPawnDoubleMove(ChessSquareWithPiece square)
        {
            if (WhiteToMove && square.Position.Rank == 2)
            {
                var jumpPosition = new ChessSquarePosition(3, square.Position.File);
                var destPosition = new ChessSquarePosition(4, square.Position.File);
                if (GetPieceAt(jumpPosition).Piece == ChessPiece.None && GetPieceAt(destPosition).Piece == ChessPiece.None)
                {
                    return new List<ChessMove> { new ChessMove(this, square.Position, destPosition, ChessPiece.None) };
                }
            }
            else if (!WhiteToMove && square.Position.Rank == 7)
            {
                var jumpPosition = new ChessSquarePosition(6, square.Position.File);
                var destPosition = new ChessSquarePosition(5, square.Position.File);
                if (GetPieceAt(jumpPosition).Piece == ChessPiece.None && GetPieceAt(destPosition).Piece == ChessPiece.None)
                {
                    return new List<ChessMove> { new ChessMove(this, square.Position, destPosition, ChessPiece.None) };
                }
            }
            return new List<ChessMove>();
        }

        private List<ChessMove> GetLegalPawnCaptures(ChessSquareWithPiece pawn)
        {
            var modifier = WhiteToMove ? +1 : -1;
            var newRank = pawn.Position.Rank + modifier;
            var leftFile = pawn.Position.File - 1;
            var rightFile = pawn.Position.File + 1;
            var result = new List<ChessMove>();
            if (newRank >= 2 && newRank <= 7)
            {
                if (leftFile >= 1 && leftFile <= 8)
                {
                    var newLeftPosition = new ChessSquarePosition(newRank, leftFile);
                    if (GetPieceAt(newLeftPosition).Piece != ChessPiece.None || newLeftPosition == EnPassantSquare)
                    {
                        result.Add(new ChessMove(this, pawn.Position, newLeftPosition, ChessPiece.None));
                    }
                }
                if (rightFile >= 1 && rightFile <= 8)
                {
                    var newRightPosition = new ChessSquarePosition(newRank, rightFile);
                    if (GetPieceAt(newRightPosition).Piece != ChessPiece.None || newRightPosition == EnPassantSquare)
                    {
                        result.Add(new ChessMove(this, pawn.Position, newRightPosition, ChessPiece.None));
                    }
                }
            }
            else if ((newRank == 1 && !WhiteToMove) || (newRank == 8 && WhiteToMove))
            {
                if (leftFile >= 1 && leftFile <= 8)
                {
                    var newLeftPosition = new ChessSquarePosition(newRank, leftFile);
                    if ((GetPieceAt(newLeftPosition).Piece != ChessPiece.None) || newLeftPosition == EnPassantSquare)
                    {
                        result.Add(new ChessMove(this, pawn.Position, newLeftPosition, WhiteToMove ? ChessPiece.WhiteQueen : ChessPiece.BlackQueen));
                        result.Add(new ChessMove(this, pawn.Position, newLeftPosition, WhiteToMove ? ChessPiece.WhiteRook : ChessPiece.BlackRook));
                        result.Add(new ChessMove(this, pawn.Position, newLeftPosition, WhiteToMove ? ChessPiece.WhiteBishop : ChessPiece.BlackBishop));
                        result.Add(new ChessMove(this, pawn.Position, newLeftPosition, WhiteToMove ? ChessPiece.WhiteKnight : ChessPiece.BlackKnight));
                    }
                }
                if (rightFile >= 1 && rightFile <= 8)
                {
                    var newRightPosition = new ChessSquarePosition(newRank, rightFile);
                    if ((GetPieceAt(newRightPosition).Piece != ChessPiece.None) || newRightPosition == EnPassantSquare)
                    {
                        result.Add(new ChessMove(this, pawn.Position, newRightPosition, WhiteToMove ? ChessPiece.WhiteQueen : ChessPiece.BlackQueen));
                        result.Add(new ChessMove(this, pawn.Position, newRightPosition, WhiteToMove ? ChessPiece.WhiteRook : ChessPiece.BlackRook));
                        result.Add(new ChessMove(this, pawn.Position, newRightPosition, WhiteToMove ? ChessPiece.WhiteBishop : ChessPiece.BlackBishop));
                        result.Add(new ChessMove(this, pawn.Position, newRightPosition, WhiteToMove ? ChessPiece.WhiteKnight : ChessPiece.BlackKnight));
                    }
                }
            }
            return result;
        }

        #region Parsing the position
        public ChessBoardState(string fen)
        {
            if (fen == null) throw new ArgumentNullException(nameof(fen));
            var splitFen = fen.Split(' ');
            if (splitFen.Length != 6)
            {
                throw new ArgumentException($"Invalid FEN notation: expected 6 fields, found {splitFen.Length}");
            }
            Board = new ChessBoard(splitFen[0]);
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
        #endregion

        #region Stringifying the position
        public override string ToString()
        {
            return $"{PositionFen} {ToMoveFen} {CastlesFen} {EnPassantFen} {HalfMoveClock} {MoveCounter}";
        }
        
        private string PositionFen
        {
            get
            {
                return Board.ToString();
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
#endregion
    }
}
