using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public class GameStatus
    {
        private static Figure attackingFigure;

        public static bool IsMate(bool isWhiteTurn, IEnumerable<Figure> figures)
        {

            bool canEatAttackingFigure = CanEatAttackingFigure(isWhiteTurn, figures);
            bool canKingMove = IsKingAbleToMove(isWhiteTurn, figures);
            bool isPossibleToProtectKing = IsPossibleToProtectKing(isWhiteTurn, figures);

            if (!canEatAttackingFigure && !canKingMove && !isPossibleToProtectKing)
            { return true; }
            else { return false; }

        }

        public static bool IsStalemate(bool isWhiteTurn, IEnumerable<Figure> figures)
        {
            bool canFiguresMove = AllFiguresCanMove(isWhiteTurn, figures);

            if (!canFiguresMove)
            {
                return true;
            }
            else { return false; }
        }

        private static King FindKing(bool isWhiteTurn, IEnumerable<Figure> figures)
        {
            King king = (King)figures.First(f => (f is King && f.IsWhite == isWhiteTurn));
            return king;
        }

        // проверка на шах, находим атакующую фигуру
        public static bool IsCheck(bool isWhiteTurn, IEnumerable<Figure> figures)
        {
            King king = FindKing(isWhiteTurn, figures);
            bool isCheck = false;

            IEnumerable<Figure> noTurnFigures = figures.Where(f => f.IsWhite != isWhiteTurn).ToList();
            foreach (Figure figure in noTurnFigures)
            {
                IEnumerable<Point> availablePositionsNoTurnFigures = figure.GetAvaliablePositions(figures);
                foreach (var position in availablePositionsNoTurnFigures)
                {
                    if (position == king.Position)
                    {
                        isCheck = true;
                        attackingFigure = figure;
                        break;
                    }
                }
            }

            return isCheck;
        }

        // можно ли съесть атакующую фигуру
        private static bool CanEatAttackingFigure(bool isWhiteTurn, IEnumerable<Figure> figures)
        {
            King king = FindKing(isWhiteTurn, figures);
            bool canEatAttackingFigure = false;
            IEnumerable<Figure> figuresWithKingColour = figures.Where(f => f.IsWhite == isWhiteTurn).ToList();

            foreach (Figure figure in figuresWithKingColour)
            {
                IEnumerable<Point> CurrentTurnFiguresAvailablePositions = figure.GetAvaliablePositions(figures);
                foreach (var position in CurrentTurnFiguresAvailablePositions)
                {
                    if (attackingFigure.Position == position)
                    {
                        canEatAttackingFigure = true;
                        break;
                    }
                }
            }

            return canEatAttackingFigure;
        }

        // может ли ходить король
        private static bool IsKingAbleToMove(bool isWhiteTurn, IEnumerable<Figure> figures)
        {
            King king = FindKing(isWhiteTurn, figures);
            bool canKingMove = true;
            List<Point> kingAvailablePositions = king.GetAvaliablePositions(figures).ToList();
            kingAvailablePositions.Remove(new Point(king.Position.X + 2, king.Position.Y));
            kingAvailablePositions.Remove(new Point(king.Position.X - 2, king.Position.Y));

            if (kingAvailablePositions.Count == 0)
            {
                canKingMove = false;
            }
            return canKingMove;
        }

        //у всех ли фигур текущего хода есть возможность ходить
        private static bool AllFiguresCanMove(bool isWhiteTurn, IEnumerable<Figure> figures)
        {
            King king = FindKing(isWhiteTurn, figures);
            bool figuresCanMove = false;

            IEnumerable<Figure> figuresWithKingColour = figures.Where(f => f.IsWhite == isWhiteTurn).ToList();
            foreach (Figure figure in figuresWithKingColour)
            {
                List<Point> CurrentTurnFiguresAvailablePositions = figure.GetAvaliablePositions(figures).ToList();
                if (CurrentTurnFiguresAvailablePositions.Count != 0)
                {
                    figuresCanMove = true;
                    break;
                }
            }

            return figuresCanMove;
        }

        // можно ли защитить короля другой фигурой
        private static bool IsPossibleToProtectKing(bool isWhiteTurn, IEnumerable<Figure> figures)
        {
            bool isPossibleToProtectKing = false;
            King king = FindKing(isWhiteTurn, figures);

            List<Point> positionsAroundKing = king.AllTheKingMoves(figures);
            positionsAroundKing.Remove(new Point(king.Position.X + 2, king.Position.Y));
            positionsAroundKing.Remove(new Point(king.Position.X - 2, king.Position.Y));

            foreach (var position in positionsAroundKing)
            {
                if (attackingFigure.Position == position)
                { return isPossibleToProtectKing; }
            }

            IEnumerable<Figure> FiguresWhoseTurn = figures.Where(f => f.IsWhite == isWhiteTurn).ToList();
            List<Point> attackFigureAvailablePositions = new List<Point>();
            IEnumerable<Point> attackFigureAllAvailablePositions = attackingFigure.GetAvaliablePositions(figures);

            if (attackingFigure is Knight Knight)
            {
                return isPossibleToProtectKing;
            }
            else if (attackingFigure is Rook)
            {
                attackFigureAvailablePositions = AvailableMovesForRook(king, attackingFigure, attackFigureAllAvailablePositions);
            }
            else if (attackingFigure is Bishop)
            {
                attackFigureAvailablePositions = AvailableMovesForBishop(king, attackingFigure, attackFigureAllAvailablePositions);
            }
            else if (attackingFigure is Queen)
            {
                attackFigureAvailablePositions = AvailableMovesForQueen(king, attackingFigure, attackFigureAllAvailablePositions);
            }

            Func<Point, Point, bool> predicate = (Point1, Point2) => Point1 == Point2;

            foreach (Figure figure in FiguresWhoseTurn)
            {
                IEnumerable<Point> FiguresWhoseTurnAvailablePositions = figure.GetAvaliablePositions(figures);
                foreach (var position in FiguresWhoseTurnAvailablePositions)
                {
                    foreach (var pos in attackFigureAvailablePositions)
                    {
                        if (predicate(position, pos))
                        {
                            isPossibleToProtectKing = true;
                            break;
                        }
                    }
                }
            }

            return isPossibleToProtectKing;
        }

        private static void AddPositios(Func<Point, bool> predicate, Point point, List<Point> attackFigureAP)
        {
            if (predicate(point))
            {
                attackFigureAP.Add(point);
            }
        }

        private static List<Point> AvailableMovesForRook(King king, Figure attackingFigure, IEnumerable<Point> attackFigureAllAP)
        {
            List<Point> attackFigureAP = new List<Point>();

            foreach (var position in attackFigureAllAP)
            {
                AddPositios(position => position.X > king.Position.X && position.X < attackingFigure.Position.X, position, attackFigureAP);
                AddPositios(position => position.X > attackingFigure.Position.X && position.X < king.Position.X, position, attackFigureAP);
                AddPositios(position => position.Y > king.Position.Y && position.Y < attackingFigure.Position.Y, position, attackFigureAP);
                AddPositios(position => position.Y > attackingFigure.Position.Y && position.Y < king.Position.Y, position, attackFigureAP);
            }

            return attackFigureAP;
        }

        private static List<Point> AvailableMovesForBishop(King king, Figure attackingFigure, IEnumerable<Point> attackFigureAllAP)
        {
            List<Point> attackFigureAP = new List<Point>();

            foreach (var position in attackFigureAllAP)
            {
                AddPositios(position => position.X > attackingFigure.Position.X && position.X < king.Position.X
                && position.Y > attackingFigure.Position.Y && position.Y < king.Position.Y, position, attackFigureAP);
                AddPositios(position => position.X > attackingFigure.Position.X && position.X < king.Position.X
                && position.Y < attackingFigure.Position.Y && position.Y > king.Position.Y, position, attackFigureAP);
                AddPositios(position => position.X < attackingFigure.Position.X && position.X > king.Position.X
                && position.Y > attackingFigure.Position.Y && position.Y < king.Position.Y, position, attackFigureAP);
                AddPositios(position => position.X < attackingFigure.Position.X && position.X > king.Position.X
                && position.Y < attackingFigure.Position.Y && position.Y > king.Position.Y, position, attackFigureAP);
            }

            return attackFigureAP;
        }

        private static List<Point> AvailableMovesForQueen(King king, Figure attackingFigure, IEnumerable<Point> attackFigureAllAP)
        {
            List<Point> attackFigureAP = new List<Point>();

            foreach (var position in attackFigureAllAP)
            {
                AddPositios(position => position.X > king.Position.X && position.X < attackingFigure.Position.X
                    && position.Y == attackingFigure.Position.Y && position.Y == king.Position.Y, position, attackFigureAP);
                AddPositios(position => position.X > attackingFigure.Position.X && position.X < king.Position.X
                && position.Y == attackingFigure.Position.Y && position.Y == king.Position.Y, position, attackFigureAP);
                AddPositios(position => position.Y > king.Position.Y && position.Y < attackingFigure.Position.Y
                && position.X == attackingFigure.Position.X && position.X == king.Position.X, position, attackFigureAP);
                AddPositios(position => position.Y > attackingFigure.Position.Y && position.Y < king.Position.Y
                && position.X == attackingFigure.Position.X && position.X == king.Position.X, position, attackFigureAP);

                AddPositios(position => position.X > attackingFigure.Position.X && position.X < king.Position.X
                && position.Y > attackingFigure.Position.Y && position.Y < king.Position.Y, position, attackFigureAP);
                AddPositios(position => position.X > attackingFigure.Position.X && position.X < king.Position.X
                && position.Y < attackingFigure.Position.Y && position.Y > king.Position.Y, position, attackFigureAP);
                AddPositios(position => position.X < attackingFigure.Position.X && position.X > king.Position.X
                && position.Y > attackingFigure.Position.Y && position.Y < king.Position.Y, position, attackFigureAP);
                AddPositios(position => position.X < attackingFigure.Position.X && position.X > king.Position.X
                && position.Y < attackingFigure.Position.Y && position.Y > king.Position.Y, position, attackFigureAP);
            }

            return attackFigureAP;
        }

        public static IEnumerable<Point> ChangeAvailablePositionProtectingFigures(bool isWhiteTurn, IEnumerable<Figure> figures, Figure currentFigure)
        {
            King king = FindKing(isWhiteTurn, figures);

            var availablePositionsCurrentFigure = currentFigure.GetAvaliablePositions(figures);

            var attackFigureAvailablePositions = new List<Point>();
            var attackFigureAllAvailablePositions = attackingFigure.GetAvaliablePositions(figures);

            if (attackingFigure is Rook)
            {
                attackFigureAvailablePositions = AvailableMovesForRook(king, attackingFigure, attackFigureAllAvailablePositions);
            }
            else if (attackingFigure is Bishop)
            {
                attackFigureAvailablePositions = AvailableMovesForBishop(king, attackingFigure, attackFigureAllAvailablePositions);
            }
            else if (attackingFigure is Queen)
            {
                attackFigureAvailablePositions = AvailableMovesForQueen(king, attackingFigure, attackFigureAllAvailablePositions);
            }

            return availablePositionsCurrentFigure.Intersect(attackFigureAvailablePositions);

        }

        public static bool CheckPositionsAroundCurrentFigure(bool isWhiteTurn, IEnumerable<Figure> figures, Figure currentFigure, out List <Point> currentfigureAvailablePos)
        {

            currentfigureAvailablePos = currentFigure.GetAvaliablePositions(figures).ToList();
            var canMove = false;
            King king = FindKing(isWhiteTurn, figures);

            var positionsRook = figures.Where(fig => fig is Rook && fig.IsWhite != currentFigure.IsWhite)
                  .Select(fig => fig.Position).ToList();
            var positionsQueen = figures.Where(fig => fig is Queen && fig.IsWhite != currentFigure.IsWhite)
                  .Select(fig => fig.Position).ToList();
            var positionsBishop = figures.Where(fig => fig is Bishop && fig.IsWhite != currentFigure.IsWhite)
                  .Select(fig => fig.Position).ToList();

            var availablePositionsFigures = figures.Where(fig => fig != king).Select(fig => fig.Position);
            var availablePositionsTowardAttackingFigure = new List<Point>();

            if (currentFigure.Position.X == king.Position.X)
            {
                if (currentFigure.Position.Y < king.Position.Y)
                {
                    for (int i = currentFigure.Position.Y + 1; i < king.Position.Y; i++)
                    {
                        var position = new Point(currentFigure.Position.X, i);
                        if (availablePositionsFigures.Any(pos => pos == position))
                        {
                            return canMove = true;
                        }
                        availablePositionsTowardAttackingFigure.Add(position);
                    }
                    for (int i = currentFigure.Position.Y - 1; i >= 0; i--)
                    {
                        var position = new Point(currentFigure.Position.X, i);
                        if (positionsRook.All(pos => pos != position) && positionsQueen.All(pos => pos != position)
                                                            && availablePositionsFigures.Any(pos => pos == position))
                        {
                            return canMove = true;
                        }
                        else if (positionsRook.Any(pos => pos == position) || positionsQueen.Any(pos => pos == position))
                        {
                            availablePositionsTowardAttackingFigure.Add(position);
                            break;
                        }
                        availablePositionsTowardAttackingFigure.Add(position);
                    }
                }
                else
                {
                    for (int i = currentFigure.Position.Y - 1; i > king.Position.Y; i--)
                    {
                        var position = new Point(currentFigure.Position.X, i);
                        if (availablePositionsFigures.Any(pos => pos == position))
                        {
                            return canMove = true;
                        }
                        availablePositionsTowardAttackingFigure.Add(position);
                    }
                    for (int i = currentFigure.Position.Y + 1; i <= 7; i++)
                    {
                        var position = new Point(currentFigure.Position.X, i);
                        if (positionsRook.All(pos => pos != position) && positionsQueen.All(pos => pos != position)
                                                            && availablePositionsFigures.Any(pos => pos == position))
                        {
                            return canMove = true;
                        }
                        else if (positionsRook.Any(pos => pos == position) || positionsQueen.Any(pos => pos == position))
                        {
                            availablePositionsTowardAttackingFigure.Add(position);
                            break;
                        }
                        availablePositionsTowardAttackingFigure.Add(position);
                    }
                }
                currentfigureAvailablePos = currentfigureAvailablePos.Intersect(availablePositionsTowardAttackingFigure).ToList();
            }

            //позиции справа и слева по прямой
            if (currentFigure.Position.Y == king.Position.Y)
            {
                if (currentFigure.Position.X < king.Position.X)
                {
                    for (int i = currentFigure.Position.X + 1; i < king.Position.X; i++)
                    {
                        var position = new Point(i, currentFigure.Position.Y);
                        if (availablePositionsFigures.Any(pos => pos == position))
                        {
                            return canMove = true;
                        }
                        availablePositionsTowardAttackingFigure.Add(position);
                    }
                    for (int i = currentFigure.Position.X - 1; i >= 0; i--)
                    {
                        var position = new Point(i, currentFigure.Position.Y);
                        if (positionsRook.All(pos => pos != position) && positionsQueen.All(pos => pos != position)
                                                            && availablePositionsFigures.Any(pos => pos == position))
                        {
                            return canMove = true;
                        }
                        else if (positionsRook.Any(pos => pos == position) || positionsQueen.Any(pos => pos == position))
                        {
                            availablePositionsTowardAttackingFigure.Add(position);
                            break;
                        }
                        availablePositionsTowardAttackingFigure.Add(position);
                    }
                }
                else
                {
                    for (int i = currentFigure.Position.X - 1; i > king.Position.X; i--)
                    {
                        var position = new Point(i, currentFigure.Position.Y);
                        if (availablePositionsFigures.Any(pos => pos == position))
                        {
                            return canMove = true;
                        }
                        availablePositionsTowardAttackingFigure.Add(position);
                    }
                    for (int i = currentFigure.Position.X + 1; i <= 7; i++)
                    {
                        var position = new Point(i, currentFigure.Position.Y);
                        if (positionsRook.All(pos => pos != position) && positionsQueen.All(pos => pos != position)
                                                            && availablePositionsFigures.Any(pos => pos == position))
                        {
                            return canMove = true;
                        }
                        else if (positionsRook.Any(pos => pos == position) || positionsQueen.Any(pos => pos == position))
                        {
                            availablePositionsTowardAttackingFigure.Add(position);
                            break;
                        }
                        availablePositionsTowardAttackingFigure.Add(position);
                    }
                }
                currentfigureAvailablePos = currentfigureAvailablePos.Intersect(availablePositionsTowardAttackingFigure).ToList();
            }


            if ((currentFigure.Position.X < king.Position.X && currentFigure.Position.Y < king.Position.Y) ||
               (currentFigure.Position.X > king.Position.X && currentFigure.Position.Y < king.Position.Y) ||
               (currentFigure.Position.X < king.Position.X && currentFigure.Position.Y > king.Position.Y) ||
               (currentFigure.Position.X > king.Position.X && currentFigure.Position.Y > king.Position.Y))
            {
                int directionX = currentFigure.Position.X < king.Position.X ? 1 : -1;
                int directionY = currentFigure.Position.Y < king.Position.Y ? 1 : -1;
                int startX = currentFigure.Position.X + directionX;
                int startY = currentFigure.Position.Y + directionY;

                var position = new Point(startX, startY);

                while (startX >= 0 && startX <= 7 && startY >= 0 && startY <= 7 && position != king.Position)
                {
                    position = new Point(startX, startY);
                    if (availablePositionsFigures.Any(pos => pos == position))
                    {
                        return canMove = true;
                    }
                    availablePositionsTowardAttackingFigure.Add(position);

                    startX += directionX;
                    startY += directionY;
                }

                // Проверка диагональных позиций вниз от текущей позиции
                directionX = -directionX;
                directionY = -directionY;
                startX = currentFigure.Position.X + directionX;
                startY = currentFigure.Position.Y + directionY;

                while (startX >= 0 && startX <= 7 && startY >= 0 && startY <= 7)
                {
                    position = new Point(startX, startY);
                    if (positionsBishop.All(pos => pos != position) && positionsQueen.All(pos => pos != position)
                                                                  && availablePositionsFigures.Any(pos => pos == position))
                    {
                        return canMove = true;
                    }
                    else if (positionsBishop.Any(pos => pos == position) || positionsQueen.Any(pos => pos == position))
                    {
                        availablePositionsTowardAttackingFigure.Add(position);
                        break;
                    }
                    availablePositionsTowardAttackingFigure.Add(position);

                    startX += directionX;
                    startY += directionY;
                }

                if (availablePositionsTowardAttackingFigure.All(pos => !positionsBishop.Contains(pos)) &&
                    availablePositionsTowardAttackingFigure.All(pos => !positionsQueen.Contains(pos)))
                {
                    return canMove = true;
                }
                currentfigureAvailablePos = currentfigureAvailablePos.Intersect(availablePositionsTowardAttackingFigure).ToList();
            }
            return canMove;

        }
    }
}
