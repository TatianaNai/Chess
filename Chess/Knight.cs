using Chess.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal class Knight : Figure
    {
        private const int BoardSize = 8;

        public Knight(bool isWhite, Point point) : base(isWhite, point)
        {
        }

        public override IEnumerable<Point> GetAvaliablePositions(IEnumerable<Figure> figures)
        {
            var positions = new List<Point>();
            for (var i = 0; i < BoardSize; i++)
            {
                for (var j = 0; j < BoardSize; j++)
                {
                    if ((Math.Abs(j - Position.X) * Math.Abs(i - Position.Y)) == 2)
                    {
                        var figure = figures.SingleOrDefault(f => f.Position == new Point(j, i));
                        if (figure == null || figure.IsWhite != IsWhite || figure.IsWhite == IsWhite)
                        {
                            positions.Add(new Point(j, i));
                        }
                    }
                }
            }
            return positions;
        }

        public override Image GetImage()
        {
            if (IsWhite)
            {
                if (IsChoosen)
                    return Resources.Knight_White_Green;

                return (Position.X + Position.Y) % 2 == 0 ? Resources.Knight_White_White : Resources.Knight_White_Black;
            }

            if (IsChoosen)
                return Resources.Knight_Black_Green;
            return (Position.X + Position.Y) % 2 == 0 ? Resources.Knight_Black_White : Resources.Knight_Black_Black;
        }
    }
}
