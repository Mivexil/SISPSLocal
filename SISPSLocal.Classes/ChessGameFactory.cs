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

        public ChessGame GetChessGameFromString(string game)
        {
            var pgnGame = GetRawGameFromString(game);
            var moveList = new List<ChessMove>();
            var state = ChessBoardState.InitialState;
            foreach (var move in pgnGame.MoveText)
            {
                if (move.Type == MoveTextEntryType.MovePair)
                {
                    var whiteMove = ConvertRawMoveToMove(state, ((MovePairEntry)move).White);
                    state = whiteMove.StateAfterMove;
                    moveList.Add(whiteMove);
                    var blackMove = ConvertRawMoveToMove(state, ((MovePairEntry)move).Black);
                    state = blackMove.StateAfterMove;
                    moveList.Add(blackMove);
                }
                else if (move.Type == MoveTextEntryType.SingleMove)
                {
                    var newMove = ConvertRawMoveToMove(state, ((HalfMoveEntry)move).Move);
                    state = newMove.StateAfterMove;
                    moveList.Add(newMove);
                }
            }
            return new ChessGame(moveList);
        }

        private ChessMove ConvertRawMoveToMove(ChessBoardState state, ilf.pgn.Data.Move rawMove)
        {
            if (rawMove.Type == MoveType.CastleKingSide)
            {
                var castleType = state.WhiteToMove ? ChessCastles.WhiteKingside : ChessCastles.BlackKingside;
                return new ChessMove(state, castleType);
            }
            else if (rawMove.Type == MoveType.CastleQueenSide)
            {
                var castleType = state.WhiteToMove ? ChessCastles.WhiteQueenside : ChessCastles.BlackQueenside;
                return new ChessMove(state, castleType);
            }
            else
            {
                var originFile = rawMove.OriginFile == null ? (int?)null : (int)rawMove.OriginFile;
                var originRank = rawMove.OriginRank;
                if (rawMove.OriginSquare != null)
                {
                    originFile = (int?)rawMove.OriginSquare.File;
                    originRank = rawMove.OriginSquare.Rank;
                }
                var destFile = (int)rawMove.TargetSquare.File;
                var destRank = rawMove.TargetSquare.Rank;
                var movingPiece = rawMove.Piece == null ? ChessPiece.None :
                    rawMove.Piece == PieceType.Bishop ? (state.WhiteToMove ? ChessPiece.WhiteBishop : ChessPiece.BlackBishop) :
                    rawMove.Piece == PieceType.Knight ? (state.WhiteToMove ? ChessPiece.WhiteKnight : ChessPiece.BlackKnight) :
                    rawMove.Piece == PieceType.Rook ? (state.WhiteToMove ? ChessPiece.WhiteRook : ChessPiece.BlackRook) :
                    rawMove.Piece == PieceType.Queen ? (state.WhiteToMove ? ChessPiece.WhiteQueen : ChessPiece.BlackQueen) :
                    rawMove.Piece == PieceType.Pawn ? (state.WhiteToMove ? ChessPiece.WhitePawn : ChessPiece.BlackPawn) :
                    rawMove.Piece == PieceType.King ? (state.WhiteToMove ? ChessPiece.WhiteKing : ChessPiece.BlackKing) :
                    ChessPiece.None;
                var promotedPiece = rawMove.PromotedPiece == null ? ChessPiece.None :
                    rawMove.PromotedPiece == PieceType.Bishop ? (state.WhiteToMove ? ChessPiece.WhiteBishop : ChessPiece.BlackBishop) :
                    rawMove.PromotedPiece == PieceType.Knight ? (state.WhiteToMove ? ChessPiece.WhiteKnight : ChessPiece.BlackKnight) :
                    rawMove.PromotedPiece == PieceType.Rook ? (state.WhiteToMove ? ChessPiece.WhiteRook : ChessPiece.BlackRook) :
                    rawMove.PromotedPiece == PieceType.Queen ? (state.WhiteToMove ? ChessPiece.WhiteQueen : ChessPiece.BlackQueen) :
                    ChessPiece.None;
                return new ChessMove(state, movingPiece, originRank, originFile, new ChessSquarePosition(destRank, destFile), promotedPiece);
            }
        }
    }
}
