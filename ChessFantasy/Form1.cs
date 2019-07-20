using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessFantasy
{
    public partial class Form1 : Form
    {
        static Board _MainBoard = new Board();//главная доска на которой будет проходить игра
        bool _activatedFigure = false;//игрок тронул фигуру?
        FiguresXY _WhiteFigures;//массив координат всех белых фигур, в том числе короля
        FiguresXY _BlackFigures;//массив координат всех черных фигур, в том числе короля
        Move _LastMove;//последний ход для эмпасана
        Move[] _AvailableMoves = new Move[0];
        XY _MovingFigureXY;

        public Form1()
        {
            InitializeComponent();
            VisualBoard.DrawVisualBoard(this, _MainBoard);

            //нужно в конструктор добавить инициализацию массивов фигур
            _BlackFigures = new FiguresXY(Color.Black);//добавить все черные фигуры
            _WhiteFigures = new FiguresXY(Color.White);//добавить все белые фигуры
        }

        public void Draw(Bitmap Bmp)
        {
            this.picture.Image = Bmp;
        }

        public Point CursorPosition()
        {
            this.Cursor = new Cursor(Cursor.Current.Handle);
            return Cursor.Position;
        }

        private void Picture_Click(object sender, EventArgs e)
        {
            Point point = this.PointToClient(new Point(Cursor.Position.X, Cursor.Position.Y));
            int CursorX = point.X;
            int CursorY = point.Y;
            Move[] Moves;

            if ((CursorX < 0) | (CursorX > 800) | (CursorY < 0) | (CursorY > 800))
            { return; }//пользователь жмет не на доску

            int row = (int)Math.Floor((double)CursorY / 100);
            int column = (int)Math.Floor((double)CursorX / 100);
            XY FigureXY = new XY(row, column);

            if (!_activatedFigure)//игрок еще не тронул фигуру
            {
                Cell Chosen = _MainBoard.BoardArr[row, column];
                
                if (Chosen == Cell.Empty) { return; }//просто нажал на пустую клетку

                if (_MainBoard.NextColor == Color.White)//если ходит белый игрок
                {
                    if ((Chosen == Cell.WhitePawn) | (Chosen == Cell.WhiteKnight) |//если мы выбрали свою фигуру
                         (Chosen == Cell.WhiteBishop) | (Chosen == Cell.WhiteRook) |
                         (Chosen == Cell.WhiteQueen) | (Chosen == Cell.WhiteKing))
                    {
                        XY kingXY = _WhiteFigures.King;
                        FiguresXY enemyFigures = _BlackFigures;
                        Move enemyLastMove = _LastMove;

                        //найдем все ходы тронутой фигуры:
                        Moves = Board.FindCellMoves(FigureXY, _MainBoard, _MainBoard.NextColor, kingXY, enemyFigures, enemyLastMove);
                        if (Moves.Length == 0) { return; }
                        _activatedFigure = true;//игрок тронул фигуру
                        _MovingFigureXY = FigureXY;//игрок собирается переместить эту фигуру
                        _AvailableMoves = Moves;
                        VisualBoard.DrawVisualBoardAvailable(this, _MainBoard, _AvailableMoves, FigureXY);//нарисуем доступные ходы
                    }
                }
                else//Если ходит черный игрок
                {
                    if ((Chosen == Cell.BlackPawn) | (Chosen == Cell.BlackKnight) |//если мы выбрали свою фигуру
                         (Chosen == Cell.BlackBishop) | (Chosen == Cell.BlackRook) |
                         (Chosen == Cell.BlackQueen) | (Chosen == Cell.BlackKing))
                    {
                        XY kingXY = _BlackFigures.King;
                        FiguresXY enemyFigures = _WhiteFigures;
                        Move enemyLastMove = _LastMove;

                        //найдем все ходы тронутой фигуры:
                        Moves = Board.FindCellMoves(FigureXY, _MainBoard, _MainBoard.NextColor, kingXY, enemyFigures, enemyLastMove);
                        if (Moves.Length == 0) { return; }
                        _activatedFigure = true;//игрок тронул фигуру
                        _MovingFigureXY = FigureXY;//игрок собирается переместить эту фигуру
                        _AvailableMoves = Moves;
                        VisualBoard.DrawVisualBoardAvailable(this, _MainBoard, _AvailableMoves, FigureXY);//нарисуем доступные ходы
                    }
                }

            }
            else//игрок уже активировал фигуру
            {
                int CountFound = _AvailableMoves.Length;// количество доступных ходов
                Move move = null;

                for (int i = 0; i < CountFound; i++)
                {
                    if ((FigureXY.r == _AvailableMoves[i].XY2.r) && (FigureXY.c == _AvailableMoves[i].XY2.c))
                    {
                        move = _AvailableMoves[i];
                        break;
                    }
                }

                if (move != null)//пользователь нажал на доступный ход
                {
                    if (move._moveType == MoveType.Taking)//если это был ход взятия 
                        //и в ходе хода кого-то съели нужно удалить фигуру из массива
                    {
                        if (_MainBoard.NextColor == Color.White)//из какого массива удалять съеденую фигуру
                        {
                            int count = _BlackFigures.Figures.Length;
                            XY[] FiguresCuted = new XY[count - 1];

                            int j = 0;
                            for (int i = 1; i < count; i++ )
                            {
                                if ((_BlackFigures.Figures[i].r == move.XY2.r) && (_BlackFigures.Figures[i].c == move.XY2.c))
                                { continue; }

                                FiguresCuted[j] = _BlackFigures.Figures[i];
                                j++;
                            }
                        }
                        else//если мы черными съели белую фигуру
                        {
                            int count = _WhiteFigures.Figures.Length;
                            XY[] FiguresCuted = new XY[count - 1];

                            int j = 0;
                            for (int i = 1; i < count; i++)
                            {
                                if ((_WhiteFigures.Figures[i].r == move.XY2.r) && (_WhiteFigures.Figures[i].c == move.XY2.c))
                                { continue; }

                                FiguresCuted[j] = _WhiteFigures.Figures[i];
                                j++;
                            }
                        }

                        //также нужно поменять координаты фигуры которая ходила
                        if (_MainBoard.NextColor == Color.White)//в каком массиве нужно изменить координаты
                        {
                            int count = _WhiteFigures.Figures.Length;

                            for (int i = 0; i < count; i++)
                            {
                                if ((_WhiteFigures.Figures[i].r == move.XY1.r) && (_WhiteFigures.Figures[i].c == move.XY1.c))
                                {
                                    _WhiteFigures.Figures[i].r = move.XY2.r;
                                    _WhiteFigures.Figures[i].c = move.XY2.c;
                                    break;
                                }
                            }
                        }
                        else//если мы ходим черной фигурой
                        {
                            int count = _BlackFigures.Figures.Length;

                            for (int i = 1; i < count; i++)
                            {
                                if ((_BlackFigures.Figures[i].r == move.XY1.r) && (_BlackFigures.Figures[i].c == move.XY1.c))
                                {
                                    _BlackFigures.Figures[i].r = move.XY2.r;
                                    _BlackFigures.Figures[i].c = move.XY2.c;
                                    break;
                                }
                            }
                        }
                    }
                    else if (move._moveType == MoveType.Moving)//если это был ход перемещения
                        //и нужно изменить координаты фигуры в массиве фигур
                    {
                        if (_MainBoard.NextColor == Color.White)//в каком массиве нужно изменить координаты
                        {
                            int count = _WhiteFigures.Figures.Length;

                            for (int i = 0; i < count; i++)
                            {
                                if ((_WhiteFigures.Figures[i].r == move.XY1.r) && (_WhiteFigures.Figures[i].c == move.XY1.c))
                                {
                                    _WhiteFigures.Figures[i].r = move.XY2.r;
                                    _WhiteFigures.Figures[i].c = move.XY2.c;
                                    break;
                                }
                            }
                        }
                        else//если мы ходим черной фигурой
                        {
                            int count = _BlackFigures.Figures.Length;

                            for (int i = 1; i < count; i++)
                            {
                                if ((_BlackFigures.Figures[i].r == move.XY1.r) && (_BlackFigures.Figures[i].c == move.XY1.c))
                                {
                                    _BlackFigures.Figures[i].r = move.XY2.r;
                                    _BlackFigures.Figures[i].c = move.XY2.c;
                                    break;
                                }
                            }
                        }
                    }


                    _MainBoard = Board.DoMove(_MainBoard, move, _MainBoard.NextColor);//делаем ход
                    _LastMove = move;//записываем как последний ход
                    VisualBoard.DrawVisualBoard(this, _MainBoard);//перерисовываем доску
                }
                else
                {
                    VisualBoard.DrawVisualBoard(this, _MainBoard);//перерисовываем доску
                }
                _activatedFigure = false;//сбрасываем тронутую фигуру
                _AvailableMoves = new Move[0];
            }

        }
    }
}
