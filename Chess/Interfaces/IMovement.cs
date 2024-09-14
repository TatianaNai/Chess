using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Interfaces
{
    public interface IMovement
    {

        public IEnumerable<Point> GetMove(IEnumerable<Figure> figures, Figure figure, int xDirection, int yDirection)
        {
            var positions = new List<Point>();
            var x = figure.Position.X + xDirection;
            var y = figure.Position.Y + yDirection;

            while (x is >= 0 and <= 7 && y is >= 0 and <= 7)
            {
                var figureStep = figures.FirstOrDefault(f => f.Position == new Point(x, y));

                if (figureStep is not null)
                {
                    if (figureStep is King && figureStep.IsWhite != figure.IsWhite)
                    {
                        positions.Add(new Point(x, y)); 
                    }

                    else if (figureStep.IsWhite != figure.IsWhite || figureStep.IsWhite == figure.IsWhite)
                    {
                        positions.Add(new Point(x, y));
                        break;
                    }
                }

                positions.Add(new Point(x, y));

                x += xDirection;
                y += yDirection;
            }

            return positions;
        }
    }

}
