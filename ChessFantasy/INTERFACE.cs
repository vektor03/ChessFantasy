using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ChessFantasy
{
    enum VisualCell
    {
        WhiteCell = 0,
        BlackCell = 1,
        WhiteCellAvailable = 2,
        BlackCellAvailable = 3
    }

    static public class VisualBoard
    {
        static private Image WhiteCell = Image.FromFile("Resources\\WhiteCell.png");
        static private Image BlackCell = Image.FromFile("Resources\\BlackCell.png");
        static private Image BlackPawn = Image.FromFile("Resources\\BlackPawn.png");
        static private Image BlackKnight = Image.FromFile("Resources\\BlackKnight.png");
        static private Image BlackBishop = Image.FromFile("Resources\\BlackBishop.png");
        static private Image BlackRook = Image.FromFile("Resources\\BlackRook.png");
        static private Image BlackQueen = Image.FromFile("Resources\\BlackQueen.png");
        static private Image BlackKing = Image.FromFile("Resources\\BlackKing.png");
        static private Image WhitePawn = Image.FromFile("Resources\\WhitePawn.png");
        static private Image WhiteKnight = Image.FromFile("Resources\\WhiteKnight.png");
        static private Image WhiteBishop = Image.FromFile("Resources\\WhiteBishop.png");
        static private Image WhiteRook = Image.FromFile("Resources\\WhiteRook.png");
        static private Image WhiteQueen = Image.FromFile("Resources\\WhiteQueen.png");
        static private Image WhiteKing = Image.FromFile("Resources\\WhiteKing.png");
        static private Image BlackCellAvailable = Image.FromFile("Resources\\BlackCellAvailable.png");
        static private Image WhiteCellAvailable = Image.FromFile("Resources\\WhiteCellAvailable.png");

        static Bitmap Bmp = new Bitmap(800, 800);
        static Graphics Graph = Graphics.FromImage(Bmp);

        /// <summary>
        /// нарисовать всю доску с фигурами
        /// </summary>
        static public void DrawVisualBoard(Form1 form, Board board)
        {
            for (int r = 0; r < 8; r++)//нарисовать шахматную доску
            {
                for (int c = 0; c < 8; c++)
                {
                    if ((r + c) % 2 == 0)//черная клетка
                    {
                        DrawCell(VisualCell.BlackCell, r, c);
                    }
                    else
                    {
                        DrawCell(VisualCell.WhiteCell, r, c);
                    }
                }
            }

            for (int r = 0; r < 8; r++)//нарисовать шахматную доску
            {
                for (int c = 0; c < 8; c++)
                {
                    DrawFigure(board, r, c);
                }
            }

            form.Draw(Bmp);
        }

        /// <summary>
        /// нарисовать все доступные фигуре ходы
        /// </summary>
        static public void DrawVisualBoardAvailable(Form1 form, Board board, Move[] AvailableMoves, XY MovingFigure)
        {
            int CountFound = AvailableMoves.Length;// количество доступных ходов
            if (CountFound == 0) { return; }

            int r = 0;
            int c = 0;

            for (int i = 0; i < CountFound; i++)//по всем доступным ходам
            {
                r = AvailableMoves[i].XY2.r;
                c = AvailableMoves[i].XY2.c;

                if ((r + c) % 2 == 0)//черная клетка
                {
                    DrawCell(VisualCell.BlackCellAvailable, r, c);//нарисовать клетку
                }
                else
                {
                    DrawCell(VisualCell.WhiteCellAvailable, r, c);//нарисовать клетку
                }

                DrawFigure(board, r, c);//нарисовать фигуру которая была сверху
            }

            //теперь нужно выделить фигуру которая будет ходить
            r = MovingFigure.r;
            c = MovingFigure.c;
            if ((r + c) % 2 == 0)//черная клетка
            {
                DrawCell(VisualCell.BlackCellAvailable, r, c);//нарисовать клетку
            }
            else
            {
                DrawCell(VisualCell.WhiteCellAvailable, r, c);//нарисовать клетку
            }

            DrawFigure(board, r, c);//нарисовать фигуру которая была сверху

            form.Draw(Bmp);
        }

        /// <summary>
        /// Рисует фигуру на клетке
        /// r,c int в диапазоне [0-7]
        /// </summary>
        static void DrawFigure (Board board, int row, int column)
        {
            int x = column * 100;
            int y = row * 100;

            Image FigureImage = null;

            switch (board.BoardArr[row, column])//клетка которую нужно нарисовать
            {
                case (Cell.BlackPawn):
                    FigureImage = BlackPawn;
                    break;
                case (Cell.BlackKnight):
                    FigureImage = BlackKnight;
                    break;
                case (Cell.BlackBishop):
                    FigureImage = BlackBishop;
                    break;
                case (Cell.BlackRook):
                    FigureImage = BlackRook;
                    break;
                case (Cell.BlackQueen):
                    FigureImage = BlackQueen;
                    break;
                case (Cell.BlackKing):
                    FigureImage = BlackKing;
                    break;
                case (Cell.WhitePawn):
                    FigureImage = WhitePawn;
                    break;
                case (Cell.WhiteKnight):
                    FigureImage = WhiteKnight;
                    break;
                case (Cell.WhiteBishop):
                    FigureImage = WhiteBishop;
                    break;
                case (Cell.WhiteRook):
                    FigureImage = WhiteRook;
                    break;
                case (Cell.WhiteQueen):
                    FigureImage = WhiteQueen;
                    break;
                case (Cell.WhiteKing):
                    FigureImage = WhiteKing;
                    break;
                default:
                    return;
            }

            DrawElement(FigureImage, x, y);
        }

        /// <summary>
        /// Рисует картинку по координатам
        /// </summary>
        static void DrawElement(Image element, int x, int y)//координаты считаются от верхнего левого угла к нижнему правому
        {
            VisualBoard.Graph.DrawImage(element, x, y);// Draw image to screen
        }

        /// <summary>
        /// обновляет изображение клетки на доске
        /// r,c int в диапазоне [0-7]
        /// </summary>
        static void DrawCell(VisualCell visualCell, int row, int column)
        {
            Image CellImage = null;

            int x = column * 100;
            int y = row * 100;

            switch (visualCell)//клетка которую нужно нарисовать
            {
                case (VisualCell.BlackCell):
                    CellImage = BlackCell;
                    break;
                case (VisualCell.BlackCellAvailable):
                    CellImage = BlackCellAvailable;
                    break;
                case (VisualCell.WhiteCell):
                    CellImage = WhiteCell;
                    break;
                case (VisualCell.WhiteCellAvailable):
                    CellImage = WhiteCellAvailable;
                    break;
                default:
                    return;
            }

            DrawElement(CellImage, x, y);//нарисовать клетку
        }
    }
}
