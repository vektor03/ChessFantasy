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
        bool _AIProcessing = false;//ИИ обсчитывает свой ход?

        Move[] _AvailableMoves = new Move[0];
        XY _MovingFigureXY;

        public Form1()
        {
            InitializeComponent();
            VisualBoard.DrawVisualBoard(this, _MainBoard);
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
            if (_AIProcessing == true){ return; }//ИИ обсчитывает свой ход

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
                        XY kingXY = _MainBoard.WhiteFigures.King;
                        FiguresXY enemyFigures = _MainBoard.BlackFigures;
                        Move enemyLastMove = _MainBoard.LastMove;

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
                    #region Черный игрок
                    /*if ((Chosen == Cell.BlackPawn) | (Chosen == Cell.BlackKnight) |//если мы выбрали свою фигуру
                         (Chosen == Cell.BlackBishop) | (Chosen == Cell.BlackRook) |
                         (Chosen == Cell.BlackQueen) | (Chosen == Cell.BlackKing))
                    {
                        XY kingXY = _MainBoard.BlackFigures.King;
                        FiguresXY enemyFigures = _MainBoard.WhiteFigures;
                        Move enemyLastMove = _MainBoard.LastMove;

                        //найдем все ходы тронутой фигуры:
                        Moves = Board.FindCellMoves(FigureXY, _MainBoard, _MainBoard.NextColor, kingXY, enemyFigures, enemyLastMove);
                        if (Moves.Length == 0) { return; }
                        _activatedFigure = true;//игрок тронул фигуру
                        _MovingFigureXY = FigureXY;//игрок собирается переместить эту фигуру
                        _AvailableMoves = Moves;
                        VisualBoard.DrawVisualBoardAvailable(this, _MainBoard, _AvailableMoves, FigureXY);//нарисуем доступные ходы
                    }*/
                    #endregion
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

                _activatedFigure = false;//сбрасываем тронутую фигуру
                _AvailableMoves = new Move[0];

                if (move == null)//пользователь нажал на недоступный ход
                {
                    VisualBoard.DrawVisualBoard(this, _MainBoard);//перерисовываем доску
                    return;
                }

                //пользователь нажал на доступный ход
                _MainBoard = Board.DoMove(_MainBoard, move);//делаем ход
                bool CheckMate = false;
                bool CheckCheck = Board.CheckCheck(_MainBoard, _MainBoard.NextColor);//проверим ход на шах
                if (CheckCheck)
                {
                    CheckMate = Board.CheckMate(_MainBoard);//проверим ход на мат
                    if (CheckMate)
                    {
                        VisualBoard.CheckMate(_MainBoard.NextColor);
                        VisualBoard.DrawVisualBoard(this, _MainBoard);//перерисовываем доску
                    }
                }

                if (!CheckMate)//если мы не поставили мат, то нужно искать ход противника
                {
                    #region Черный ИИ
                    _AIProcessing = true;
                    Move BestMove = AI.Processing(_MainBoard);

                    _MainBoard = Board.DoMove(_MainBoard, BestMove);//делаем ход

                    CheckCheck = Board.CheckCheck(_MainBoard, _MainBoard.NextColor);//проверим ход на шах
                    if (CheckCheck)
                    {
                        CheckMate = Board.CheckMate(_MainBoard);//проверим ход на мат
                        if (CheckMate)
                        {
                            VisualBoard.CheckMate(_MainBoard.NextColor);
                        }
                    }
                    VisualBoard.DrawVisualBoard(this, _MainBoard);//перерисовываем доску
                    _AIProcessing = false;
                    #endregion
                }
            }

        }
    }
}
