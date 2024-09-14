using Chess.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess
{
    public class FigureMover
    {
        public bool IsWhiteTurn { get; private set; } = true;

        public  IEnumerable<Point> AvaliablePositions { get;  set; }

        public Figure? CurrentFigure { get;  set; }

        public List<Figure> Figures { get; private set; }

        public FigureMover(bool isWhiteDown) => Figures = Initializer.GetFigures(isWhiteDown);

        public void ChooseFigure(Figure figure)
        {
            var currentfigureAvailablePos = figure.GetAvaliablePositions(Figures).ToList();

            if (figure.IsWhite == IsWhiteTurn)
            {
                if (figure is not King && GameStatus.IsCheck(IsWhiteTurn, Figures))
                {
                    AvaliablePositions = GameStatus.ChangeAvailablePositionProtectingFigures(IsWhiteTurn, Figures, figure);
                }
                else 
                {
                    if (GameStatus.CheckPositionsAroundCurrentFigure(IsWhiteTurn, Figures, figure, out currentfigureAvailablePos))
                    { AvaliablePositions = figure.GetAvaliablePositions(Figures); }
                    else 
                    {
                        AvaliablePositions = currentfigureAvailablePos;
                    }

                }
                
                figure.IsChoosen = true;
                CurrentFigure = figure;
            }
            var king = Figures.Where(f => f is King).Where(f => f.IsWhite == figure.IsWhite).First();
        }

        public Figure? GetFigure(Point point) => Figures.FirstOrDefault(x => x.Position == point);

        public void Move(Point point)
        {
            CurrentFigure.IsChoosen = false;
            if (AvaliablePositions.Any(position => position == point))
            {

                if (CurrentFigure is IMarkable markableFigure)
                {
                    markableFigure.IsFirstTurn = false;
                }

                CurrentFigure.Position = point;
                IsWhiteTurn = !IsWhiteTurn;

                var attactedFigure = Figures.FirstOrDefault(figure => figure.Position == point && figure.IsWhite != CurrentFigure.IsWhite);

                if (attactedFigure != null)
                {
                    Figures.Remove(attactedFigure);
                }
            }
        }
    }
}
