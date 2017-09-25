using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISPSLocal.Classes
{
    public struct ChessMove
    {
        public ChessSquarePosition OriginSquare { get; }
        public ChessSquarePosition DestinationSquare { get; }
        public ChessPiece MovingPiece { get; }
        public int MoveNumber { get; }
        public ChessColor Color { get; }
        public bool IsCapture { get; }
        public bool IsEnPassant { get; }
        private bool? _isCheck;
        public bool IsCheck
        {
            get { return _isCheck ?? StateAfterMove.IsCheck; }
        }

        private bool? _isMate;
        public bool IsMate
        {
            get { return _isMate ?? StateAfterMove.IsMate; }
        }

        private bool? _needsRankDisambiguation;
        public bool NeedsRankDisambiguation
        {
            get
            {
                if (_needsRankDisambiguation != null) return _needsRankDisambiguation.Value;
                var movingPiece = MovingPiece;
                var originSquare = OriginSquare;
                var destinationSquare = DestinationSquare;
                var sameDestMoves = StateBeforeMove.GetLegalMoves()
                    .Where(x => x.MovingPiece.Type == movingPiece.Type && x.DestinationSquare == destinationSquare);
                _needsRankDisambiguation = sameDestMoves.Where(x => x.OriginSquare.File == originSquare.File).Count() > 1;
                return _needsRankDisambiguation.Value;
            }
        }

        private bool? _needsFileDisambiguation;
        public bool NeedsFileDisambiguation
        {
            get
            {
                if (_needsFileDisambiguation != null) return _needsFileDisambiguation.Value;
                var movingPiece = MovingPiece;
                var originSquare = OriginSquare;
                var destinationSquare = DestinationSquare;
                var sameDestMoves = StateBeforeMove.GetLegalMoves()
                    .Where(x => x.MovingPiece.Type == movingPiece.Type && x.DestinationSquare == destinationSquare);
                _needsFileDisambiguation = sameDestMoves.Count() > 1 && !sameDestMoves.All(x => x.OriginSquare.File == originSquare.File);
                return _needsFileDisambiguation.Value;
            }
        }

        public ChessPiece PromotionPiece { get; }
        public ChessCastles Castle { get; }
        public ChessBoardState StateBeforeMove { get; }
        public ChessBoardState StateAfterMove { get; }

        public ChessMove(ChessBoardState previousState,
            ChessCastles castle)
        {
            _isCheck = _isMate = _needsFileDisambiguation = _needsRankDisambiguation = null;
            StateBeforeMove = previousState;
            Color = castle == ChessCastles.BlackKingside || castle == ChessCastles.BlackQueenside ?
                ChessColor.Black : ChessColor.White;
            OriginSquare = Color == ChessColor.Black ?
                new ChessSquarePosition("e8") :
                new ChessSquarePosition("e1");
            DestinationSquare = castle == ChessCastles.BlackKingside ? new ChessSquarePosition("g8") :
                                castle == ChessCastles.BlackQueenside ? new ChessSquarePosition("c8") :
                                castle == ChessCastles.WhiteKingside ? new ChessSquarePosition("g1") :
                                new ChessSquarePosition("c1");
            MovingPiece = Color == ChessColor.Black ? ChessPiece.BlackKing : ChessPiece.WhiteKing;
            MoveNumber = previousState.MoveCounter;
            IsEnPassant = false;
            IsCapture = false;
            PromotionPiece = ChessPiece.None;
            Castle = castle;


            var newSquares = StateBeforeMove.Board.Squares.ToList();
            newSquares[OriginSquare.Index] = new ChessSquareWithPiece(OriginSquare, ChessPiece.None);
            newSquares[DestinationSquare.Index] = new ChessSquareWithPiece(DestinationSquare, MovingPiece);
            var rookSource = castle == ChessCastles.BlackKingside ? new ChessSquarePosition("h8") :
                             castle == ChessCastles.BlackQueenside ? new ChessSquarePosition("a8") :
                             castle == ChessCastles.WhiteKingside ? new ChessSquarePosition("h1") :
                             new ChessSquarePosition("a1");
            var rookTarget = castle == ChessCastles.BlackKingside ? new ChessSquarePosition("f8") :
                             castle == ChessCastles.BlackQueenside ? new ChessSquarePosition("d8") :
                             castle == ChessCastles.WhiteKingside ? new ChessSquarePosition("f1") :
                             new ChessSquarePosition("d1");
            newSquares[rookSource.Index] = new ChessSquareWithPiece(rookSource, ChessPiece.None);
            newSquares[rookTarget.Index] = new ChessSquareWithPiece(rookTarget, Color == ChessColor.Black ? ChessPiece.BlackRook : ChessPiece.WhiteRook);

            var castlesToDrop = Color == ChessColor.Black ?
                ChessCastles.BlackKingside | ChessCastles.BlackQueenside :
                ChessCastles.WhiteKingside | ChessCastles.WhiteQueenside;
            var newCastles = (previousState.AllowedCastles & ~castlesToDrop) & ChessCastles.All;

            StateAfterMove = new ChessBoardState(new ChessBoard(newSquares),
                newCastles,
                previousState.WhiteToMove ? previousState.MoveCounter : previousState.MoveCounter + 1,
                previousState.HalfMoveClock + 1,
                !previousState.WhiteToMove,
                null
            );
        }

        private static ChessCastles GetNewCastles(ChessBoardState previousState,
            ChessColor color,
            ChessPiece movingPiece,
            ChessSquarePosition originSquare,
            ChessSquarePosition destinationSquare)
        {
            var previousCastles = previousState.AllowedCastles;
            if (color == ChessColor.White)
            {
                if (movingPiece.Type == ChessPieceType.King)
                {
                    previousCastles = (previousCastles & ~ChessCastles.WhiteKingside) & ChessCastles.All;
                    previousCastles = (previousCastles & ~ChessCastles.WhiteQueenside) & ChessCastles.All;
                }
                else if (movingPiece.Type == ChessPieceType.Rook && originSquare == new ChessSquarePosition("a1"))
                {
                    previousCastles = (previousCastles & ~ChessCastles.WhiteQueenside) & ChessCastles.All;
                }
                else if (movingPiece.Type == ChessPieceType.Rook && originSquare == new ChessSquarePosition("h1"))
                {
                    previousCastles = (previousCastles & ~ChessCastles.WhiteKingside) & ChessCastles.All;
                }
                else if (destinationSquare == new ChessSquarePosition("a8"))
                {
                    previousCastles = (previousCastles & ~ChessCastles.BlackQueenside) & ChessCastles.All;
                }
                else if (destinationSquare == new ChessSquarePosition("h8"))
                {
                    previousCastles = (previousCastles & ~ChessCastles.BlackKingside) & ChessCastles.All;
                }
            }
            if (color == ChessColor.Black)
            {
                if (movingPiece.Type == ChessPieceType.King)
                {
                    previousCastles = (previousCastles & ~ChessCastles.BlackKingside) & ChessCastles.All;
                    previousCastles = (previousCastles & ~ChessCastles.BlackQueenside) & ChessCastles.All;
                }
                else if (movingPiece.Type == ChessPieceType.Rook && originSquare == new ChessSquarePosition("a8"))
                {
                    previousCastles = (previousCastles & ~ChessCastles.BlackQueenside) & ChessCastles.All;
                }
                else if (movingPiece.Type == ChessPieceType.Rook && originSquare == new ChessSquarePosition("h8"))
                {
                    previousCastles = (previousCastles & ~ChessCastles.BlackKingside) & ChessCastles.All;
                }
                else if (destinationSquare == new ChessSquarePosition("a1"))
                {
                    previousCastles = (previousCastles & ~ChessCastles.WhiteQueenside) & ChessCastles.All;
                }
                else if (destinationSquare == new ChessSquarePosition("h1"))
                {
                    previousCastles = (previousCastles & ~ChessCastles.WhiteKingside) & ChessCastles.All;
                }
            }
            return previousCastles;
        }

        public ChessMove(ChessBoardState previousState,
            ChessPiece movingPiece,
            int? originRank,
            int? originFile,
            ChessSquarePosition destinationSquare,
            ChessPiece promotionPiece)
        {
            var previousLegalMoves = previousState.GetLegalMoves();
            var matchingMoves = previousLegalMoves.Where(x => x.MovingPiece == movingPiece &&
            (originRank == null || x.OriginSquare.Rank == originRank.Value) &&
            (originFile == null || x.OriginSquare.File == originFile.Value) &&
            x.DestinationSquare == destinationSquare &&
            x.PromotionPiece == promotionPiece);
            if (matchingMoves.Count() == 0)
            {
                throw new ArgumentException($"The move could not be made");
            }
            if (matchingMoves.Count() > 1)
            {
                throw new ArgumentException($"Ambiguous move");
            }
            var move = matchingMoves.First();
            _isCheck = _isMate = _needsFileDisambiguation = _needsRankDisambiguation = null;
            StateBeforeMove = previousState;
            OriginSquare = move.OriginSquare;
            DestinationSquare = move.DestinationSquare;
            MovingPiece = move.MovingPiece;
            Color = move.Color;
            PromotionPiece = move.PromotionPiece;
            MoveNumber = move.MoveNumber;
            IsEnPassant = move.IsEnPassant;
            IsCapture = move.IsCapture;
            Castle = move.Castle;
            StateAfterMove = move.StateAfterMove;
        }

        public ChessMove(ChessBoardState previousState,
            ChessSquarePosition originSquare,
            ChessSquarePosition destinationSquare,
            ChessPiece promotionPiece) : this()
        {
            _isCheck = _isMate = _needsFileDisambiguation = _needsRankDisambiguation = null;
            StateBeforeMove = previousState;
            OriginSquare = originSquare;
            DestinationSquare = destinationSquare;
            var movingPiece = previousState.GetPieceAt(originSquare).Piece;
            MovingPiece = movingPiece;
            Color = MovingPiece.Color;
            PromotionPiece = promotionPiece;
            MoveNumber = previousState.MoveCounter;
            IsEnPassant = previousState.EnPassantSquare != null &&
                previousState.EnPassantSquare.Value == destinationSquare &&
                MovingPiece.Type == ChessPieceType.Pawn &&
                originSquare.File != destinationSquare.File;
            IsCapture = IsEnPassant || previousState.GetPieceAt(destinationSquare).Piece != ChessPiece.None;
            Castle = ChessCastles.None;
            var newSquares = StateBeforeMove.Board.Squares.ToList();
            newSquares[OriginSquare.Index] = new ChessSquareWithPiece(OriginSquare, ChessPiece.None);
            newSquares[DestinationSquare.Index] = new ChessSquareWithPiece(DestinationSquare, MovingPiece);
            if (IsEnPassant)
            {
                var epPosition = StateBeforeMove.WhiteToMove ? new ChessSquarePosition(destinationSquare.Rank - 1, destinationSquare.File) :
                    new ChessSquarePosition(destinationSquare.Rank + 1, destinationSquare.File);
                newSquares[epPosition.Index] = new ChessSquareWithPiece(epPosition, ChessPiece.None);
            }


            StateAfterMove = new ChessBoardState(new ChessBoard(newSquares),
                GetNewCastles(previousState, Color, MovingPiece, OriginSquare, DestinationSquare),
                previousState.WhiteToMove ? previousState.MoveCounter : previousState.MoveCounter + 1,
                (IsCapture || MovingPiece.Type == ChessPieceType.Pawn) ? 0 : previousState.HalfMoveClock + 1,
                !previousState.WhiteToMove,
                (MovingPiece.Type == ChessPieceType.Pawn && Math.Abs(OriginSquare.Rank - DestinationSquare.Rank) == 2) ?
                new ChessSquarePosition((OriginSquare.Rank + DestinationSquare.Rank) / 2, DestinationSquare.File) :
                (ChessSquarePosition?)null
            );
        }

        public string DisplayString
        {
            get
            {
                if (Castle != ChessCastles.None)
                {
                    return _castleString;
                }
                return $"{_pieceString}{_sourceFileString}{_sourceRankString}{_captureString}{_targetSquareString}{_promotionString}{_checkMateString}";
            }
        }

        public override string ToString()
        {
            return $"{MoveNumber}. {(Color == ChessColor.White ? "" : "...")}{DisplayString}";
        }

        private string _targetSquareString => DestinationSquare.ToString();
        private string _captureString => IsCapture ? "x" : "";
        private string _sourceRankString => NeedsRankDisambiguation ? OriginSquare.Rank.ToString() : "";
        private string _sourceFileString => NeedsFileDisambiguation ? ((char)('a' + (OriginSquare.File - 1))).ToString() : "";
        private string _pieceString
        {
            get
            {
                var fen = MovingPiece.GetFen().ToString().ToUpper();
                return fen == "P" ? "" : fen;
            }
        }            
        private string _promotionString
        {
            get
            {
                if (PromotionPiece == ChessPiece.None) return "";
                var fen = PromotionPiece.GetFen().ToString().ToUpper();
                return $"={fen}";
            }
        }
        private string _castleString => (Castle == ChessCastles.BlackKingside || Castle == ChessCastles.WhiteKingside) ?
            "O-O" : "O-O-O";

        private string _checkMateString => IsMate ? "#" : IsCheck ? "+" : "";
    }
}
