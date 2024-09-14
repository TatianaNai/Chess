using Chess.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public class King : Figure, IMarkable
    {
        private const int BoardSize = 8;

        public bool IsFirstTurn { get; set; } = true;

        public King(bool isWhite, Point point) : base(isWhite, point)
        {
        }

        public override IEnumerable<Point> GetAvaliablePositions(IEnumerable<Figure> figures)
        {

            var allTheKingMoves = AllTheKingMoves(figures);

            var whiteFiguresPositions = figures.Where(f => f.IsWhite);
            var blackFiguresPositions = figures.Where(f => f.IsWhite == false);

            var positionWhiteKing = figures.Where(f => f is King).Where(k => k.IsWhite).Select(f => f.Position).First();
            var positionBlackKing = figures.Where(f => f is King).Where(k => k.IsWhite == false).Select(f => f.Position).First();

            List<Point> getAvaliablePositions = new List<Point>();

            if (IsWhite)
            {
                var attackBlackFiguresPositions = AttackOfPiecesOtherThanPawns(blackFiguresPositions, figures);
                var theKingBlackAttack = AttacKing(positionBlackKing);
                var attackPawnBlack = figures.Where(f => f is Pawn).Where(f => f.IsWhite == false)
                                               .Select(f => (Pawn)f)
                                               .Select(position => position.GetAttackForKing(figures))
                                               .SelectMany(p => p).ToList();

                foreach (var move in allTheKingMoves)
                {

                    if (whiteFiguresPositions.All(f => f.Position != move) && attackBlackFiguresPositions.All(attack => attack != move)
                                                                           && theKingBlackAttack.All(attack => attack != move)
                                                                           && attackPawnBlack.All(attack => attack != move))

                    {
                        getAvaliablePositions.Add(move);
                    }
                }
            }

            if (!IsWhite)
            {
                var theKingWhiteAttack = AttacKing(positionWhiteKing);
                var attackWhiteFiguresPositions = AttackOfPiecesOtherThanPawns(whiteFiguresPositions, figures);
                var attackPawnWhite = figures.Where(f => f is Pawn).Where(p => p.IsWhite)
                                          .Select(f => (Pawn)f)
                                          .Select(position => position.GetAttackForKing(figures))
                                          .SelectMany(p => p).ToList();

                foreach (var move in allTheKingMoves)
                {
                    if (blackFiguresPositions.All(f => f.Position != move) && attackWhiteFiguresPositions.All(attack => attack != move)
                                                                           && theKingWhiteAttack.All(attack => attack != move)
                                                                           && attackPawnWhite.All(attack => attack != move))
                    {
                        getAvaliablePositions.Add(move);
                    }
                }
            }
            return getAvaliablePositions;
        }

        private List<Point> AttackOfPiecesOtherThanPawns(IEnumerable<Figure> figures, IEnumerable<Figure> allfigures)
        {

            var attack = figures.Where(f => !(f is Pawn))
                                            .Where(f => !(f is King))
                                            .Select(position => position.GetAvaliablePositions(allfigures))
                                            .SelectMany(p => p).ToList();
            return attack;
        }

        public override Image GetImage()
        {
            if (IsWhite)
            {
                if (IsChoosen)
                {
                    return Properties.Resources.King_White_Green;
                }
                else
                {
                    return (Position.X + Position.Y) % 2 == 0 ? Properties.Resources.King_White_White : Properties.Resources.King_White_Black;
                }

            }
            else
            {
                if (IsChoosen)
                {
                    return Properties.Resources.King_Black_Green;
                }
                else
                {
                    return (Position.X + Position.Y) % 2 == 0 ? Properties.Resources.King_Black_White : Properties.Resources.King_Black_Black;
                }
            }
        }

        public List<Point> AllTheKingMoves(IEnumerable<Figure> figures)
        {
            var allTheKingMoves = new List<Point>();

            var point = new Point(Position.X, Position.Y);
            if (Position.Y + 1 < BoardSize && Position.Y + 1 >= 0)
            {
                allTheKingMoves.Add(new Point(Position.X, Position.Y + 1));
            }

            if (Position.Y - 1 < BoardSize && Position.Y - 1 >= 0)
            {
                allTheKingMoves.Add(new Point(Position.X, Position.Y - 1));
            }

            if (Position.X + 1 < BoardSize && Position.X + 1 >= 0)
            {
                allTheKingMoves.Add(new Point(Position.X + 1, Position.Y));
            }

            if (Position.X - 1 < BoardSize && Position.X - 1 >= 0)
            {
                allTheKingMoves.Add(new Point(Position.X - 1, Position.Y));
            }

            if (Position.X + 1 < BoardSize && Position.Y - 1 < BoardSize && Position.X + 1 >= 0 && Position.Y - 1 >= 0)
            {
                allTheKingMoves.Add(new Point(Position.X + 1, Position.Y - 1));
            }

            if (Position.X + 1 < BoardSize && Position.Y + 1 < BoardSize && Position.X + 1 >= 0 && Position.Y + 1 >= 0)
            {
                allTheKingMoves.Add(new Point(Position.X + 1, Position.Y + 1));
            }

            if (Position.X - 1 < BoardSize && Position.Y + 1 < BoardSize && Position.X - 1 >= 0 && Position.Y + 1 >= 0)
            {
                allTheKingMoves.Add(new Point(Position.X - 1, Position.Y + 1));
            }

            if (Position.X - 1 < BoardSize && Position.Y - 1 < BoardSize && Position.X - 1 >= 0 && Position.Y - 1 >= 0)
            {
                allTheKingMoves.Add(new Point(Position.X - 1, Position.Y - 1));
            }

            var doCastling = DoCastling(figures);
            var result = allTheKingMoves.Concat(doCastling).ToList();

            return result;

        }

        private List<Point> AttacKing(Point point)
        {
            var AttacKing = new List<Point>();

            var newPoint = new Point(point.X, point.Y);
            if (point.Y + 1 < BoardSize && point.Y + 1 >= 0)
            {
                AttacKing.Add(new Point(point.X, point.Y + 1));
            }

            if (point.Y - 1 < BoardSize && point.Y - 1 >= 0)
            {
                AttacKing.Add(new Point(point.X, point.Y - 1));
            }

            if (point.X + 1 < BoardSize && point.X + 1 >= 0)
            {
                AttacKing.Add(new Point(point.X + 1, point.Y));
            }

            if (point.X - 1 < BoardSize && point.X - 1 >= 0)
            {
                AttacKing.Add(new Point(point.X - 1, point.Y));
            }

            if (point.X + 1 < BoardSize && point.Y - 1 < BoardSize && point.X + 1 >= 0 && point.Y - 1 >= 0)
            {
                AttacKing.Add(new Point(point.X + 1, point.Y - 1));
            }

            if (point.X + 1 < BoardSize && point.Y + 1 < BoardSize && point.X + 1 >= 0 && point.Y + 1 >= 0)
            {
                AttacKing.Add(new Point(point.X + 1, point.Y + 1));
            }

            if (point.X - 1 < BoardSize && point.Y + 1 < BoardSize && point.X - 1 >= 0 && point.Y + 1 >= 0)
            {
                AttacKing.Add(new Point(point.X - 1, point.Y + 1));
            }

            if (point.X - 1 < BoardSize && point.Y - 1 < BoardSize && point.X - 1 >= 0 && point.Y - 1 >= 0)
            {
                AttacKing.Add(new Point(point.X - 1, point.Y - 1));
            }

            return AttacKing;
        }

        private List<Point> DoCastling(IEnumerable<Figure> figures)
        {
            var doCastling = new List<Point>();
            var blackRooks = figures.Where(f => f is Rook).Where(f => f.IsWhite == false).Select(f => (Rook)f);
            var whiteRooks = figures.Where(f => f is Rook).Where(f => f.IsWhite).Select(f => (Rook)f).ToList();
            var whiteFiguresPositions = figures.Where(f => f.IsWhite);
            var blackFiguresPositions = figures.Where(f => f.IsWhite == false);
            var allFiguresPositions = whiteFiguresPositions.Concat(blackFiguresPositions);

            var rightPosition = new List<Point>();
            rightPosition.Add(new Point(Position.X + 1, Position.Y));
            rightPosition.Add(new Point(Position.X + 2, Position.Y));
            var LeftPosition = new List<Point>();
            LeftPosition.Add(new Point(Position.X - 1, Position.Y));
            LeftPosition.Add(new Point(Position.X - 2, Position.Y));
            LeftPosition.Add(new Point(Position.X - 3, Position.Y));

            if (IsWhite && whiteRooks != null && this.IsFirstTurn && whiteRooks.Where(f => f.IsFirstTurn == true) != null)
            {
                if (whiteRooks.Count() == 2 && whiteRooks.All(f => f.IsFirstTurn == true))
                {
                    if (ChekPositionForCastling(allFiguresPositions, LeftPosition))
                    {
                        doCastling.Add(new Point(Position.X - 2, Position.Y));
                    }
                    if (ChekPositionForCastling(allFiguresPositions, rightPosition))
                    {
                        doCastling.Add(new Point(Position.X + 2, Position.Y));
                    }
                }
                else if (whiteRooks.Count() == 2 && whiteRooks.Any(f => f.IsFirstTurn == true) ||
                         whiteRooks.Count() == 1 && whiteRooks.Any(f => f.IsFirstTurn == true))
                {
                    var positionRook = whiteRooks.Where(f => f.IsFirstTurn == true).First();

                    if (positionRook.Position.X == 0 && ChekPositionForCastling(allFiguresPositions, LeftPosition))
                    {
                        doCastling.Add(new Point(Position.X - 2, Position.Y));
                    }
                    if (positionRook.Position.X == 7 && ChekPositionForCastling(allFiguresPositions, rightPosition))
                    {
                        doCastling.Add(new Point(Position.X + 2, Position.Y));
                    }
                }
            }
            if (!IsWhite && blackRooks != null && this.IsFirstTurn && blackRooks.Where(f => f.IsFirstTurn == true) != null)
            {
                if (blackRooks.Count() == 2 && blackRooks.All(f => f.IsFirstTurn == true))
                {
                    if (ChekPositionForCastling(allFiguresPositions, LeftPosition))
                    {
                        doCastling.Add(new Point(Position.X - 2, Position.Y));
                    }
                    if (ChekPositionForCastling(allFiguresPositions, rightPosition))
                    {
                        doCastling.Add(new Point(Position.X + 2, Position.Y));
                    }
                }
                else if (blackRooks.Count() == 2 && blackRooks.Any(f => f.IsFirstTurn == true) ||
                         blackRooks.Count() == 1 && blackRooks.Any(f => f.IsFirstTurn == true))
                {
                    var positionRook = blackRooks.Where(f => f.IsFirstTurn == true).First();

                    if (positionRook.Position.X == 0)
                    {
                        doCastling.Add(new Point(Position.X - 2, Position.Y));
                    }
                    if (positionRook.Position.X == 7)
                    {
                        doCastling.Add(new Point(Position.X + 2, Position.Y));
                    }
                }
            }
            return doCastling;
        }

        private bool ChekPositionForCastling(IEnumerable<Figure> figures, List<Point> chekPoints)
        {
            bool result = true;

            foreach (var chekPoint in chekPoints)
            {
                if (figures.Select(f => f.Position).Any(position => position == chekPoint))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

    }

}
