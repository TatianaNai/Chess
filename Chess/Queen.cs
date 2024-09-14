using Chess.Interfaces;
using Chess.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal class Queen : Figure, IMovement
    {
        public Queen(bool isWhite, Point point) : base(isWhite, point)
        {
        }

        public override IEnumerable<Point> GetAvaliablePositions(IEnumerable<Figure> figures)
        {
            var positions = new List<Point>();

            positions.AddRange(((IMovement)this).GetMove(figures, this, -1, 1));
            positions.AddRange(((IMovement)this).GetMove(figures, this, 1, 1));
            positions.AddRange(((IMovement)this).GetMove(figures, this, 1, -1));
            positions.AddRange(((IMovement)this).GetMove(figures, this, -1, -1));

            positions.AddRange(((IMovement)this).GetMove(figures, this, -1, 0));
            positions.AddRange(((IMovement)this).GetMove(figures, this, 1, 0));
            positions.AddRange(((IMovement)this).GetMove(figures, this, 0, -1));
            positions.AddRange(((IMovement)this).GetMove(figures, this, 0, 1));

            return positions;
        }

        public override Image GetImage()
        {
            if (IsWhite)
            {
                if (IsChoosen)
                    return Resources.Queen_White_Green;

                return (Position.X + Position.Y) % 2 == 0 ? Resources.Queen_White_White : Resources.Queen_White_Black_2;
            }

            if (IsChoosen)
                return Resources.Queen_Black_Green_2;
            return (Position.X + Position.Y) % 2 == 0 ? Resources.Queen_Black_White_2 : Resources.Queen_Black_Black;
        }
    }
}
