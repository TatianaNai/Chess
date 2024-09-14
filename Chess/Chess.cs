using Chess.Properties;
using System.Drawing.Printing;
using System.Resources;

namespace Chess;

public partial class Chess : Form
{
    //Team Two
    private const int BoardSize = 8;
    private const int ButtonSize = 75;

    private readonly Button[,] chessButtons = new Button[BoardSize, BoardSize];

    private readonly FigureMover _figureMover;

    private readonly bool isWhiteTurn = true;

    public Chess()
    {
        _figureMover = new FigureMover(true);
        this.Text = "Шахматы";
        this.AutoSize = true;
        this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        this.StartPosition = FormStartPosition.CenterScreen;
        InitializeChessBoard();
    }

    private void SetFigures()
    {
        foreach (var figure in _figureMover.Figures)
        {
            chessButtons[figure.Position.X, figure.Position.Y].Tag = figure;
            chessButtons[figure.Position.X, figure.Position.Y].Image = figure.GetImage();
        }
    }

    private void SetImageToAvaliablePositions(bool isFillCells)
    {
        foreach (var position in _figureMover.AvaliablePositions)
        {
            var figuresPosition = _figureMover.Figures.Where(f => f.IsWhite == _figureMover.CurrentFigure.IsWhite)
                                                     .Select(f => f.Position).ToList();

            if (figuresPosition.Any(p => p == position)) continue;

            var figure = _figureMover.GetFigure(position);
            if (figure != null)
            {
                if (isFillCells)
                {
                    figure.IsChoosen = true;
                }
                else
                {
                    figure.IsChoosen = false;
                }

                chessButtons[position.X, position.Y].Image = figure.GetImage();
            }
            else if (isFillCells)
            {
                chessButtons[position.X, position.Y].Image = Properties.Resources.Empty_Green;
            }
            else
            {
                chessButtons[position.X, position.Y].Image = GetEmptyImage(new Point(position.X, position.Y));
            }
        }
    }

    private Image GetEmptyImage(Point point)
    {
        return (point.X + point.Y) % 2 == 0
            ? Properties.Resources.Empty_White
            : Properties.Resources.Empty_Black;
    }

    private void ClearCurrentCell(Point point)
    {
        chessButtons[point.X, point.Y].Image = GetEmptyImage(point);
        chessButtons[point.X, point.Y].Tag = point;
    }


    private void InitializeChessBoard()
    {
        for (var y = 0; y < BoardSize; y++)
        {
            for (var x = 0; x < BoardSize; x++)
            {
                var button = Initializer.GetButton(ButtonSize, x, y);
                chessButtons[x, y] = button;
                Controls.Add(chessButtons[x, y]);
                chessButtons[x, y].Click += ChessButton_Click;
            }
        }
        SetFigures();
    }

    private void ChessButton_Click(object sender, EventArgs e)
    {
        var button = (Button)sender;

        if (button.Tag is Figure figure &&
            figure.IsWhite == _figureMover.IsWhiteTurn)
        {
            if (_figureMover.CurrentFigure == null ||
                _figureMover.CurrentFigure.IsWhite != figure.IsWhite)
            {
                ShowFigureMoves(figure);
            }
            else if (_figureMover.CurrentFigure.IsWhite == figure.IsWhite)
            {
                var previousFigure = _figureMover.CurrentFigure;
                TakebacksToTryDifferentMove(previousFigure);
                ShowFigureMoves(figure);
            }

        }
        else if (_figureMover.CurrentFigure != null &&
                _figureMover.CurrentFigure.IsWhite == _figureMover.IsWhiteTurn)
        {
            var point = button.Tag is Figure ? ((Figure)button.Tag).Position : (Point)button.Tag;


            var isPointEmptyCell = _figureMover.CurrentFigure.GetAvaliablePositions(_figureMover.Figures).Contains(point);
            if (!isPointEmptyCell)
            {
                TakebacksToTryDifferentMove(_figureMover.CurrentFigure);
                _figureMover.CurrentFigure = null;
            }
            else
            {
                if (_figureMover.CurrentFigure is King && point.X == 6)
                {
                    King king = (King)_figureMover.CurrentFigure;
                    if (king.IsFirstTurn)
                    { 
                        var Rook = _figureMover.Figures.Where(f => f is Rook)
                                                       .Where(f => f.IsWhite == _figureMover.CurrentFigure.IsWhite)
                                                       .Where(f => f.Position.X == 7).First();

                        var moveRookPoint = new Point(point.X - 1, point.Y);
                        ClearCurrentCell(Rook.Position);
                        Rook.Position = moveRookPoint;
                    }

                }

                if (_figureMover.CurrentFigure is King && point.X == 2)
                {
                    King king = (King)_figureMover.CurrentFigure;
                    if(king.IsFirstTurn)
                    {
                        var Rook = _figureMover.Figures.Where(f => f is Rook)
                                                       .Where(f => f.IsWhite == _figureMover.CurrentFigure.IsWhite)
                                                       .Where(f => f.Position.X == 0).First();

                        var moveRookPoint = new Point(point.X + 1, point.Y);
                        ClearCurrentCell(Rook.Position);
                        Rook.Position = moveRookPoint;
                    }
                }

                ClearCurrentCell(_figureMover.CurrentFigure.Position);
                _figureMover.Move(point);
                SetFigures();
                SetImageToAvaliablePositions(false);

                ShowGameStatusMethod();

            }
        }
    }


    private void ShowGameStatusMethod()
    {
        bool isCheck = GameStatus.IsCheck(_figureMover.IsWhiteTurn, _figureMover.Figures);

        if (isCheck)
        {
            bool isMate = GameStatus.IsMate(_figureMover.IsWhiteTurn, _figureMover.Figures);
            if (isMate)
            {
                if (_figureMover.IsWhiteTurn)
                { MessageBox.Show("Black win"); }
                else { MessageBox.Show("White win"); }

                for (var y = 0; y < BoardSize; y++)
                {
                    for (var x = 0; x < BoardSize; x++)
                    {
                        chessButtons[x, y].Enabled = false;
                    }
                }
            }
            else
            {
                if (_figureMover.IsWhiteTurn)
                { MessageBox.Show("Check to the White King!"); }
                else { MessageBox.Show("Check to the Black King!"); }
            }
        }
        else
        {
            bool isStalemate = GameStatus.IsStalemate(_figureMover.IsWhiteTurn, _figureMover.Figures);
            if (isStalemate)
            { MessageBox.Show("Draw"); }
        }
    }

    private void TakebacksToTryDifferentMove(Figure previousFigure)
    {
        previousFigure.IsChoosen = false;
        chessButtons[previousFigure.Position.X, previousFigure.Position.Y].Image = previousFigure.GetImage();
        SetImageToAvaliablePositions(false);
    }

    private void ShowFigureMoves(Figure figure)
    {
        _figureMover.ChooseFigure(figure);
        chessButtons[figure.Position.X, figure.Position.Y].Image = figure.GetImage();
        SetImageToAvaliablePositions(true);
    }
}
