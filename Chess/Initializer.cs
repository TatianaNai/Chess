using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Chess;

internal static class Initializer
{
    private const int BoardSize = 8;
    internal static Button GetButton(int ButtonSize, int x, int y)
    {
        return new Button
        {
            Width = ButtonSize,
            Height = ButtonSize,
            Location = new Point(x * ButtonSize , y * ButtonSize ),
            BackgroundImage = (x + y) % 2 == 0 ? Chess.Properties.Resources.Empty_White : Chess.Properties.Resources.Empty_Black,
            Tag = new Point(x, y),

        };
    }

    public static List<Figure> GetFigures(bool isWhiteDown)
    {
        var figures = new List<Figure>();
        for (var y = 0; y < BoardSize; y++)
        {
            for (var x = 0; x < BoardSize; x++)
            {
                var figure = GetFigure(x, y, isWhiteDown);
                if (figure is not null)
                {
                    figures.Add(figure);
                }
            }
        }

        return figures;
    }
    public static Figure? GetFigure(int x, int y, bool isWhiteDown)
    {
        switch (y)
        {
            case 0:
            case 7:
                switch (x)
                {
                    case 0:
                    case 7:
                        return new Rook(y is 7, new Point(x, y));
                    case 1:
                    case 6:
                        return new Knight(y is 7, new Point(x, y));
                    case 2:
                        return new Bishop(y is 7, new Point(x, y));
                    case 5:
                        return new Bishop(y is 7, new Point(x, y));
                    case 3:
                        return new Queen(y is 7, new Point(x, y));
                    case 4:
                        return new King(y is 7, new Point(x, y));
                }

                break;
            case 1:
            case 6:
                return new Pawn(y is 6, new Point(x, y), isWhiteDown);
        }

        return null;
    }
}