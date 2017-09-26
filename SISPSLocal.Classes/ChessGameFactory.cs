// <copyright file="ChessGameFactory.cs" company="Rolls-Royce plc">
//   Copyright (c) 2017 Rolls-Royce plc
// </copyright>

using System.Collections.Generic;
using System.Linq;
using ilf.pgn;
using ilf.pgn.Data;
using ilf.pgn.Data.MoveText;

namespace SISPSLocal.Classes
{
    public class ChessGameFactory
    {
        private readonly PgnReader reader = new PgnReader();

        public Game GetRawGameFromString(string game)
        {
            return reader.ReadFromString(game).Games.First();
        }

        public ChessGame GetChessGameFromString(string game)
        {
            var pgnGame = GetRawGameFromString(game);
            var state = ChessBoardState.InitialState;
            var moves = ParseMovetext(pgnGame.MoveText, state);
            return new ChessGame(moves);
        }

        private List<ChessMove> ParseMovetext(MoveTextEntryList entries,
            ChessBoardState initialState)
        {
            var state = initialState;
            var moveList = new List<ChessMove>();
            foreach (var move in entries)
            {
                if (move.Type == MoveTextEntryType.MovePair)
                {
                    var whiteMove = ConvertRawMoveToMove(state, ((MovePairEntry) move).White);
                    state = whiteMove.StateAfterMove;
                    moveList.Add(whiteMove);
                    var blackMove = ConvertRawMoveToMove(state, ((MovePairEntry) move).Black);
                    state = blackMove.StateAfterMove;
                    moveList.Add(blackMove);
                }
                else if (move.Type == MoveTextEntryType.SingleMove)
                {
                    var newMove = ConvertRawMoveToMove(state, ((HalfMoveEntry) move).Move);
                    state = newMove.StateAfterMove;
                    moveList.Add(newMove);
                }
                else if (move.Type == MoveTextEntryType.Comment)
                {
                    var prevMove = moveList[moveList.Count - 1];
                    if (prevMove.Castle != ChessCastles.None)
                    {
                        var newMove = new ChessMove(prevMove.StateBeforeMove,
                            prevMove.Castle,
                            prevMove.NAG,
                            ((CommentEntry) move).Comment,
                            prevMove.Variations);
                        moveList[moveList.Count - 1] = newMove;
                    }
                    else
                    {
                        var newMove = new ChessMove(prevMove.StateBeforeMove,
                            prevMove.OriginSquare,
                            prevMove.DestinationSquare,
                            prevMove.PromotionPiece,
                            prevMove.NAG,
                            ((CommentEntry) move).Comment,
                            prevMove.Variations);
                        moveList[moveList.Count - 1] = newMove;
                    }
                }
                else if (move.Type == MoveTextEntryType.NumericAnnotationGlyph)
                {
                    var prevMove = moveList[moveList.Count - 1];
                    if (prevMove.Castle != ChessCastles.None)
                    {
                        var newMove = new ChessMove(prevMove.StateBeforeMove,
                            prevMove.Castle,
                            ((NAGEntry) move).Code,
                            prevMove.Annotation,
                            prevMove.Variations);
                        moveList[moveList.Count - 1] = newMove;
                    }
                    else
                    {
                        var newMove = new ChessMove(prevMove.StateBeforeMove,
                            prevMove.OriginSquare,
                            prevMove.DestinationSquare,
                            prevMove.PromotionPiece,
                            ((NAGEntry) move).Code,
                            prevMove.Annotation,
                            prevMove.Variations);
                        moveList[moveList.Count - 1] = newMove;
                    }
                }
                else if (move.Type == MoveTextEntryType.RecursiveAnnotationVariation)
                {
                    var prevMove = moveList[moveList.Count - 1];
                    var parsedMovetext =
                        new ChessVariation(ParseMovetext(((RAVEntry) move).MoveText, prevMove.StateBeforeMove));
                    var newList = prevMove.Variations.ToList();
                    newList.Add(parsedMovetext);
                    if (prevMove.Castle != ChessCastles.None)
                    {
                        var newMove = new ChessMove(prevMove.StateBeforeMove,
                            prevMove.Castle,
                            prevMove.NAG,
                            prevMove.Annotation,
                            newList);
                        moveList[moveList.Count - 1] = newMove;
                    }
                    else
                    {
                        var newMove = new ChessMove(prevMove.StateBeforeMove,
                            prevMove.OriginSquare,
                            prevMove.DestinationSquare,
                            prevMove.PromotionPiece,
                            prevMove.NAG,
                            prevMove.Annotation,
                            newList);
                        moveList[moveList.Count - 1] = newMove;
                    }
                }
            }
            return moveList;
        }

        private ChessMove ConvertRawMoveToMove(ChessBoardState state, Move rawMove)
        {
            var nag = rawMove.Annotation == null ? (int?) null : (int) rawMove.Annotation.Value;
            if (rawMove.Type == MoveType.CastleKingSide)
            {
                var castleType = state.WhiteToMove ? ChessCastles.WhiteKingside : ChessCastles.BlackKingside;
                return new ChessMove(state, castleType);
            }
            if (rawMove.Type == MoveType.CastleQueenSide)
            {
                var castleType = state.WhiteToMove ? ChessCastles.WhiteQueenside : ChessCastles.BlackQueenside;
                return new ChessMove(state, castleType);
            }
            var originFile = rawMove.OriginFile == null ? (int?) null : (int) rawMove.OriginFile;
            var originRank = rawMove.OriginRank;
            if (rawMove.OriginSquare != null)
            {
                originFile = (int?) rawMove.OriginSquare.File;
                originRank = rawMove.OriginSquare.Rank;
            }
            var destFile = (int) rawMove.TargetSquare.File;
            var destRank = rawMove.TargetSquare.Rank;
            var movingPiece = rawMove.Piece == null
                ? ChessPiece.None
                : rawMove.Piece == PieceType.Bishop
                    ? (state.WhiteToMove ? ChessPiece.WhiteBishop : ChessPiece.BlackBishop)
                    : rawMove.Piece == PieceType.Knight
                        ? (state.WhiteToMove ? ChessPiece.WhiteKnight : ChessPiece.BlackKnight)
                        : rawMove.Piece == PieceType.Rook
                            ? (state.WhiteToMove ? ChessPiece.WhiteRook : ChessPiece.BlackRook)
                            : rawMove.Piece == PieceType.Queen
                                ? (state.WhiteToMove ? ChessPiece.WhiteQueen : ChessPiece.BlackQueen)
                                : rawMove.Piece == PieceType.Pawn
                                    ? (state.WhiteToMove ? ChessPiece.WhitePawn : ChessPiece.BlackPawn)
                                    : rawMove.Piece == PieceType.King
                                        ? (state.WhiteToMove ? ChessPiece.WhiteKing : ChessPiece.BlackKing)
                                        : ChessPiece.None;
            var promotedPiece = rawMove.PromotedPiece == null
                ? ChessPiece.None
                : rawMove.PromotedPiece == PieceType.Bishop
                    ? (state.WhiteToMove ? ChessPiece.WhiteBishop : ChessPiece.BlackBishop)
                    : rawMove.PromotedPiece == PieceType.Knight
                        ? (state.WhiteToMove ? ChessPiece.WhiteKnight : ChessPiece.BlackKnight)
                        : rawMove.PromotedPiece == PieceType.Rook
                            ? (state.WhiteToMove ? ChessPiece.WhiteRook : ChessPiece.BlackRook)
                            : rawMove.PromotedPiece == PieceType.Queen
                                ? (state.WhiteToMove ? ChessPiece.WhiteQueen : ChessPiece.BlackQueen)
                                : ChessPiece.None;
            return new ChessMove(state,
                movingPiece,
                originRank,
                originFile,
                new ChessSquarePosition(destRank, destFile),
                promotedPiece);
        }
    }
}