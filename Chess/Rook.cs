using Chess.Interfaces;
using Chess.Properties;

namespace Chess;

public class Rook : Figure, IMarkable, IMovement
{
    public bool IsFirstTurn { get; set; } = true;

    public Rook(bool isWhite, Point point) : base(isWhite, point)
    {
    }

    public override IEnumerable<Point> GetAvaliablePositions(IEnumerable<Figure> figures)
    {
        var positions = new List<Point>();
        positions.AddRange(((IMovement)this).GetMove(figures, this, -1, 0)); // По горизонтали слева
        positions.AddRange(((IMovement)this).GetMove(figures, this, 1, 0));  // По горизонтали справа
        positions.AddRange(((IMovement)this).GetMove(figures, this, 0, -1)); // По вертикали сверху
        positions.AddRange(((IMovement)this).GetMove(figures, this, 0, 1));  // По вертикали снизу

        return positions;
    }

    public override Image GetImage()
    {
        if (IsWhite)
        {
            if (IsChoosen)
                return Resources.Castle_White_Green;
            
            return (Position.X + Position.Y) % 2 == 0 ? Resources.Castle_White_White : Resources.Castle_White_Black;
        }

        if (IsChoosen)
            return Resources.Castle_Black_Green;
        return (Position.X + Position.Y) % 2 == 0 ? Resources.Castle_Black_White : Resources.Castle_Black_Black;
    }
}