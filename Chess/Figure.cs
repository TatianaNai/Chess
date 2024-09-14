using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public abstract class Figure
    {
        public Point Position { get; set; }

        public Image Image { get; set; }

        public bool IsChoosen { get; set; }

        public bool IsWhite { get; set; }

        public Figure(bool isWhite, Point point)
        {
            IsWhite = isWhite;
            Position = point;
        }

        public abstract IEnumerable<Point> GetAvaliablePositions(IEnumerable<Figure> figures);

        public abstract Image GetImage();
    }
}
