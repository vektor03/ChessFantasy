using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessFantasy
{
    enum Cell
    {
        Empty = 0,
        WhitePawn = 1,
        WhiteKnight = 2,
        WhiteBishop = 3,
        WhiteRook = 4,
        WhiteQueen = 5,
        WhiteKing = 6,
        BlackPawn = 7,
        BlackKnight = 8,
        BlackBishop = 9,
        BlackRook = 10,
        BlackQueen = 11,
        BlackKing = 12
    }
    enum Color { White = 0, Black = 1}

    class Board //содержит доску с фигурами и все дествия с ней
    {
        private Cell[,] _board; // доска
        public Cell[,] BoardArr//Свойства чтобы можно было снаружи смотреть массив
        {
            get { return _board; }
        }

        public Board()//создание начальной доски
            {
                _board = new Cell[8, 8] {{ (Cell)10, (Cell)8, (Cell)9, (Cell)11, (Cell)12, (Cell)9, (Cell)8, (Cell)10 },
                                         { (Cell)7, (Cell)7, (Cell)7, (Cell)7, (Cell)7, (Cell)7, (Cell)7, (Cell)7 },
                                         { 0, 0, 0, 0, 0, 0, 0, 0 },
                                         { 0, 0, 0, 0, 0, 0, 0, 0 },
                                         { 0, 0, 0, 0, 0, 0, 0, 0 },
                                         { 0, 0, 0, 0, 0, 0, 0, 0 },
                                         { (Cell)1, (Cell)1, (Cell)1, (Cell)1, (Cell)1, (Cell)1, (Cell)1, (Cell)1 },
                                         { (Cell)4, (Cell)2, (Cell)3, (Cell)4, (Cell)5, (Cell)3, (Cell)2, (Cell)4 } };
            }

        public Board(Board a)//конструктор копирования
        {
            _board = a._board;
        }

        /// <summary>
        /// Возвращает доску с примененным ходом без проверки на правила
        /// </summary>
        Board DoMove(Board board, Move move)
        {
            //если координаты хода находятся на доске
            if ((move.XY1.r > 0) & (move.XY1.r < 7) & (move.XY1.c > 0) & (move.XY1.c < 7) &
                (move.XY2.r > 0) & (move.XY2.r < 7) & (move.XY2.c > 0) & (move.XY2.c < 7))
            {
                Board boardOut = board;//доска с выполненным ходом
                Cell Figure = boardOut._board[move.XY1.r, move.XY1.c];//фигура которой мы ходим
                boardOut._board[move.XY1.r, move.XY1.c] = Cell.Empty;//клетка с которой начинался ход обнуляется
                boardOut._board[move.XY2.r, move.XY2.c] = Figure;//клетка, где заканчивается ход, занимается фигурой

                return boardOut;
            }
            else { return null; }
        }


        #region FindFiguresMoves
        /// <summary>
        /// Поиск всех ходов, которые может совершить пешка
        /// </summary>
        Move[] FindPawnMoves(XY pawnXY, Board board, Color color, Move enemyLastMove)
        {
            //пешка может совершить 6 возможных ходов: перемещение на одну или 2 клетки вперед, 
            //2 атаки по диагонали и 2 взятия на проходе с 2-х сторон
            Move[] Moves = new Move[0];//массив для хранени всех ходов пешки
            Move[] Temp1;
            Move Move;
            int Count = 0; //количество найденных ходов пешки

            if (color == Color.White)//ищем ходы белой пешки
            {
                //ход белой пешкой на одну клетку вперед
                if (board._board[pawnXY.r - 1, pawnXY.c] == Cell.Empty)
                {
                    Move = new Move(pawnXY, pawnXY.r - 1, pawnXY.c);
                    Moves = new Move[] { Move };
                    Count = 1;
                }

                //ход белой пешкой на две клетки вперед
                if ((pawnXY.r == 6) & (board._board[pawnXY.r - 1, pawnXY.c] == Cell.Empty) &
                    (board._board[pawnXY.r - 2, pawnXY.c] == Cell.Empty))
                {
                    Move = new Move(pawnXY, pawnXY.r - 2, pawnXY.c);
                    if (Count == 1)
                    {
                        Temp1 = Moves;
                        Moves = new Move[] { Temp1[0], Move };
                        Count = 2;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //атака белой пешкой налево
                if ((pawnXY.c > 0) &
                    ((board._board[pawnXY.r - 1, pawnXY.c - 1] == Cell.BlackPawn) |
                    (board._board[pawnXY.r - 1, pawnXY.c - 1] == Cell.BlackKnight) |
                    (board._board[pawnXY.r - 1, pawnXY.c - 1] == Cell.BlackBishop) |
                    (board._board[pawnXY.r - 1, pawnXY.c - 1] == Cell.BlackRook) |
                    (board._board[pawnXY.r - 1, pawnXY.c - 1] == Cell.BlackQueen) |
                    (board._board[pawnXY.r - 1, pawnXY.c - 1] == Cell.BlackKing)))
                {
                    Move = new Move(pawnXY, pawnXY.r - 1, pawnXY.c - 1);
                    if (Count > 0)
                    {
                        Temp1 = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp1[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //атака белой пешкой направо
                if ((pawnXY.c < 7) &
                    ((board._board[pawnXY.r - 1, pawnXY.c + 1] == Cell.BlackPawn) |
                    (board._board[pawnXY.r - 1, pawnXY.c + 1] == Cell.BlackKnight) |
                    (board._board[pawnXY.r - 1, pawnXY.c + 1] == Cell.BlackBishop) |
                    (board._board[pawnXY.r - 1, pawnXY.c + 1] == Cell.BlackRook) |
                    (board._board[pawnXY.r - 1, pawnXY.c + 1] == Cell.BlackQueen) |
                    (board._board[pawnXY.r - 1, pawnXY.c + 1] == Cell.BlackKing)))
                {
                    Move = new Move(pawnXY, pawnXY.r - 1, pawnXY.c + 1);
                    if (Count > 0)
                    {
                        Temp1 = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp1[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //взятие на проходе белой пешкой налево
                if ((pawnXY.r == 3) & //нужно находиться на нужной горизонтали(5-й)
                    (board._board[pawnXY.r, pawnXY.c - 1] == Cell.BlackPawn) &// чтобы слева была вражеская пешка
                    (enemyLastMove.XY1.r == 1) & (enemyLastMove.XY1.c == pawnXY.c - 1) & //прошлый вражеский ход начинался с 7 строки и нужного столбца
                    (enemyLastMove.XY2.r == 3) & (enemyLastMove.XY1.c == pawnXY.c - 1))//прошлый вражеский ход закончился на 5 строке и нужном столбце
                {
                    Move = new Move(pawnXY, pawnXY.r - 1, pawnXY.c - 1);
                    if (Count > 0)
                    {
                        Temp1 = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp1[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //взятие на проходе белой пешкой направо
                if ((pawnXY.r == 3) & //нужно находиться на нужной горизонтали(5-й)
                    (board._board[pawnXY.r, pawnXY.c + 1] == Cell.BlackPawn) &// чтобы слева была вражеская пешка
                    (enemyLastMove.XY1.r == 1) & (enemyLastMove.XY1.c == pawnXY.c + 1) & //прошлый вражеский ход начинался с 7 строки и нужного столбца
                    (enemyLastMove.XY2.r == 3) & (enemyLastMove.XY1.c == pawnXY.c + 1))//прошлый вражеский ход закончился на 5 строке и нужном столбце
                {
                    Move = new Move(pawnXY, pawnXY.r - 1, pawnXY.c + 1);
                    if (Count > 0)
                    {
                        Temp1 = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp1[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

            }
            else//Если мы ищем атаки черной пешки
            {
                //ход черной пешкой на одну клетку вперед
                if (board._board[pawnXY.r + 1, pawnXY.c] == Cell.Empty)
                {
                    Move = new Move(pawnXY, pawnXY.r + 1, pawnXY.c);
                    Moves = new Move[] { Move };
                    Count = 1;
                }

                //ход черной пешкой на две клетки вперед
                if ((pawnXY.r == 6) & (board._board[pawnXY.r + 1, pawnXY.c] == Cell.Empty) &
                    (board._board[pawnXY.r + 2, pawnXY.c] == Cell.Empty))
                {
                    Move = new Move(pawnXY, pawnXY.r + 2, pawnXY.c);
                    if (Count == 1)
                    {
                        Temp1 = Moves;
                        Moves = new Move[] { Temp1[0], Move };
                        Count = 2;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //атака черной пешкой налево
                if ((pawnXY.c > 0) &
                    ((board._board[pawnXY.r + 1, pawnXY.c - 1] == Cell.BlackPawn) |
                    (board._board[pawnXY.r + 1, pawnXY.c - 1] == Cell.BlackKnight) |
                    (board._board[pawnXY.r + 1, pawnXY.c - 1] == Cell.BlackBishop) |
                    (board._board[pawnXY.r + 1, pawnXY.c - 1] == Cell.BlackRook) |
                    (board._board[pawnXY.r + 1, pawnXY.c - 1] == Cell.BlackQueen) |
                    (board._board[pawnXY.r + 1, pawnXY.c - 1] == Cell.BlackKing)))
                {
                    Move = new Move(pawnXY, pawnXY.r + 1, pawnXY.c - 1);
                    if (Count > 0)
                    {
                        Temp1 = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp1[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //атака черной пешкой направо
                if ((pawnXY.c < 7) &
                    ((board._board[pawnXY.r + 1, pawnXY.c + 1] == Cell.BlackPawn) |
                    (board._board[pawnXY.r + 1, pawnXY.c + 1] == Cell.BlackKnight) |
                    (board._board[pawnXY.r + 1, pawnXY.c + 1] == Cell.BlackBishop) |
                    (board._board[pawnXY.r + 1, pawnXY.c + 1] == Cell.BlackRook) |
                    (board._board[pawnXY.r + 1, pawnXY.c + 1] == Cell.BlackQueen) |
                    (board._board[pawnXY.r + 1, pawnXY.c + 1] == Cell.BlackKing)))
                {
                    Move = new Move(pawnXY, pawnXY.r + 1, pawnXY.c + 1);
                    if (Count > 0)
                    {
                        Temp1 = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp1[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //взятие на проходе черной пешкой налево
                if ((pawnXY.r == 4) & //нужно находиться на нужной горизонтали(4-й)
                    (board._board[pawnXY.r, pawnXY.c - 1] == Cell.BlackPawn) &// чтобы слева была вражеская пешка
                    (enemyLastMove.XY1.r == 6) & (enemyLastMove.XY1.c == pawnXY.c - 1) & //прошлый вражеский ход начинался с 2 строки и нужного столбца
                    (enemyLastMove.XY2.r == 4) & (enemyLastMove.XY1.c == pawnXY.c - 1))//прошлый вражеский ход закончился на 4 строке и нужном столбце
                {
                    Move = new Move(pawnXY, pawnXY.r + 1, pawnXY.c - 1);
                    if (Count > 0)
                    {
                        Temp1 = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp1[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //взятие на проходе черной пешкой направо
                if ((pawnXY.r == 4) & //нужно находиться на нужной горизонтали(5-й)
                    (board._board[pawnXY.r, pawnXY.c + 1] == Cell.BlackPawn) &// чтобы слева была вражеская пешка
                    (enemyLastMove.XY1.r == 6) & (enemyLastMove.XY1.c == pawnXY.c + 1) & //прошлый вражеский ход начинался с 7 строки и нужного столбца
                    (enemyLastMove.XY2.r == 4) & (enemyLastMove.XY1.c == pawnXY.c + 1))//прошлый вражеский ход закончился на 5 строке и нужном столбце
                {
                    Move = new Move(pawnXY, pawnXY.r + 1, pawnXY.c + 1);
                    if (Count > 0)
                    {
                        Temp1 = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp1[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }
            }

            return Moves;
        }

        /// <summary>
        /// Поиск всех ходов, которые может совершить конь
        /// </summary>
        Move[] FindKnightMoves(XY knightXY, Board board, Color color)
        {
            Move[] Moves = new Move[0];//массив для хранени всех ходов коня
            Move[] Temp;
            Move Move;
            int Count = 0; //количество найденных ходов коня

            if (color == Color.White)//ищем ходы белого коня
            {
                //атака белым конем на 1 час
                if ((knightXY.r - 2 > 0) & (knightXY.c + 1 < 7) &
                    ((board._board[knightXY.r - 2, knightXY.c + 1] == Cell.BlackPawn) |
                    (board._board[knightXY.r - 2, knightXY.c + 1] == Cell.BlackKnight) |
                    (board._board[knightXY.r - 2, knightXY.c + 1] == Cell.BlackBishop) |
                    (board._board[knightXY.r - 2, knightXY.c + 1] == Cell.BlackRook) |
                    (board._board[knightXY.r - 2, knightXY.c + 1] == Cell.BlackQueen) |
                    (board._board[knightXY.r - 2, knightXY.c + 1] == Cell.BlackKing) |
                    (board._board[knightXY.r - 2, knightXY.c + 1] == Cell.Empty)))
                {
                    Move = new Move(knightXY, knightXY.r - 2, knightXY.c + 1);
                    Moves = new Move[] { Move };
                    Count = 1;
                }

                //атака белым конем на 2 часа
                if ((knightXY.r - 1 > 0) & (knightXY.c + 2 < 7) &
                    ((board._board[knightXY.r - 1, knightXY.c + 2] == Cell.BlackPawn) |
                    (board._board[knightXY.r - 1, knightXY.c + 2] == Cell.BlackKnight) |
                    (board._board[knightXY.r - 1, knightXY.c + 2] == Cell.BlackBishop) |
                    (board._board[knightXY.r - 1, knightXY.c + 2] == Cell.BlackRook) |
                    (board._board[knightXY.r - 1, knightXY.c + 2] == Cell.BlackQueen) |
                    (board._board[knightXY.r - 1, knightXY.c + 2] == Cell.BlackKing) |
                    (board._board[knightXY.r - 1, knightXY.c + 2] == Cell.Empty)))
                {
                    Move = new Move(knightXY, knightXY.r - 1, knightXY.c + 2);
                    if (Count == 1)
                    {
                        Temp = Moves;
                        Moves = new Move[] { Temp[1], Move };
                        Count = 2;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    } 
                }

                //атака белым конем на 4 часа
                if ((knightXY.r + 1 < 7) & (knightXY.c + 2 < 7) &
                    ((board._board[knightXY.r + 1, knightXY.c + 2] == Cell.BlackPawn) |
                    (board._board[knightXY.r + 1, knightXY.c + 2] == Cell.BlackKnight) |
                    (board._board[knightXY.r + 1, knightXY.c + 2] == Cell.BlackBishop) |
                    (board._board[knightXY.r + 1, knightXY.c + 2] == Cell.BlackRook) |
                    (board._board[knightXY.r + 1, knightXY.c + 2] == Cell.BlackQueen) |
                    (board._board[knightXY.r + 1, knightXY.c + 2] == Cell.BlackKing) |
                    (board._board[knightXY.r + 1, knightXY.c + 2] == Cell.Empty)))
                {
                    Move = new Move(knightXY, knightXY.r + 1, knightXY.c + 2);
                    if (Count > 0)
                    {
                        Temp = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //атака белым конем на 5 часов
                if ((knightXY.r + 2 < 7) & (knightXY.c + 1 < 7) &
                    ((board._board[knightXY.r + 2, knightXY.c + 1] == Cell.BlackPawn) |
                    (board._board[knightXY.r + 2, knightXY.c + 1] == Cell.BlackKnight) |
                    (board._board[knightXY.r + 2, knightXY.c + 1] == Cell.BlackBishop) |
                    (board._board[knightXY.r + 2, knightXY.c + 1] == Cell.BlackRook) |
                    (board._board[knightXY.r + 2, knightXY.c + 1] == Cell.BlackQueen) |
                    (board._board[knightXY.r + 2, knightXY.c + 1] == Cell.BlackKing) |
                    (board._board[knightXY.r + 2, knightXY.c + 1] == Cell.Empty)))
                {
                    Move = new Move(knightXY, knightXY.r + 2, knightXY.c + 1);
                    if (Count > 0)
                    {
                        Temp = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //атака белым конем на 7 часов
                if ((knightXY.r + 2 < 7) & (knightXY.c - 1 > 0) &
                    ((board._board[knightXY.r + 2, knightXY.c - 1] == Cell.BlackPawn) |
                    (board._board[knightXY.r + 2, knightXY.c - 1] == Cell.BlackKnight) |
                    (board._board[knightXY.r + 2, knightXY.c - 1] == Cell.BlackBishop) |
                    (board._board[knightXY.r + 2, knightXY.c - 1] == Cell.BlackRook) |
                    (board._board[knightXY.r + 2, knightXY.c - 1] == Cell.BlackQueen) |
                    (board._board[knightXY.r + 2, knightXY.c - 1] == Cell.BlackKing) |
                    (board._board[knightXY.r + 2, knightXY.c - 1] == Cell.Empty)))
                {
                    Move = new Move(knightXY, knightXY.r + 2, knightXY.c - 1);
                    if (Count > 0)
                    {
                        Temp = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //атака белым конем на 8 часов
                if ((knightXY.r + 1 < 7) & (knightXY.c - 2 > 0) &
                    ((board._board[knightXY.r + 1, knightXY.c - 2] == Cell.BlackPawn) |
                    (board._board[knightXY.r + 1, knightXY.c - 2] == Cell.BlackKnight) |
                    (board._board[knightXY.r + 1, knightXY.c - 2] == Cell.BlackBishop) |
                    (board._board[knightXY.r + 1, knightXY.c - 2] == Cell.BlackRook) |
                    (board._board[knightXY.r + 1, knightXY.c - 2] == Cell.BlackQueen) |
                    (board._board[knightXY.r + 1, knightXY.c - 2] == Cell.BlackKing) |
                    (board._board[knightXY.r + 1, knightXY.c - 2] == Cell.Empty)))
                {
                    Move = new Move(knightXY, knightXY.r + 1, knightXY.c - 2);
                    if (Count > 0)
                    {
                        Temp = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //атака белым конем на 10 часов
                if ((knightXY.r - 1 > 0) & (knightXY.c - 2 > 0) &
                    ((board._board[knightXY.r - 1, knightXY.c - 2] == Cell.BlackPawn) |
                    (board._board[knightXY.r - 1, knightXY.c - 2] == Cell.BlackKnight) |
                    (board._board[knightXY.r - 1, knightXY.c - 2] == Cell.BlackBishop) |
                    (board._board[knightXY.r - 1, knightXY.c - 2] == Cell.BlackRook) |
                    (board._board[knightXY.r - 1, knightXY.c - 2] == Cell.BlackQueen) |
                    (board._board[knightXY.r - 1, knightXY.c - 2] == Cell.BlackKing) |
                    (board._board[knightXY.r - 1, knightXY.c - 2] == Cell.Empty)))
                {
                    Move = new Move(knightXY, knightXY.r - 1, knightXY.c - 2);
                    if (Count > 0)
                    {
                        Temp = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //атака белым конем на 11 часов
                if ((knightXY.r - 2 > 0) & (knightXY.c - 1 > 0) &
                    ((board._board[knightXY.r - 2, knightXY.c - 1] == Cell.BlackPawn) |
                    (board._board[knightXY.r - 2, knightXY.c - 1] == Cell.BlackKnight) |
                    (board._board[knightXY.r - 2, knightXY.c - 1] == Cell.BlackBishop) |
                    (board._board[knightXY.r - 2, knightXY.c - 1] == Cell.BlackRook) |
                    (board._board[knightXY.r - 2, knightXY.c - 1] == Cell.BlackQueen) |
                    (board._board[knightXY.r - 2, knightXY.c - 1] == Cell.BlackKing) |
                    (board._board[knightXY.r - 2, knightXY.c - 1] == Cell.Empty)))
                {
                    Move = new Move(knightXY, knightXY.r - 2, knightXY.c - 1);
                    if (Count > 0)
                    {
                        Temp = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }
            }
            else//Если мы ищем атаки черного коня
            {
                //атака черным конем на 1 час
                if ((knightXY.r - 2 > 0) & (knightXY.c + 1 < 7) &
                    ((board._board[knightXY.r - 2, knightXY.c + 1] == Cell.WhitePawn) |
                    (board._board[knightXY.r - 2, knightXY.c + 1] == Cell.WhiteKnight) |
                    (board._board[knightXY.r - 2, knightXY.c + 1] == Cell.WhiteBishop) |
                    (board._board[knightXY.r - 2, knightXY.c + 1] == Cell.WhiteRook) |
                    (board._board[knightXY.r - 2, knightXY.c + 1] == Cell.WhiteQueen) |
                    (board._board[knightXY.r - 2, knightXY.c + 1] == Cell.WhiteKing) |
                    (board._board[knightXY.r - 2, knightXY.c + 1] == Cell.Empty)))
                {
                    Move = new Move(knightXY, knightXY.r - 2, knightXY.c + 1);
                    Moves = new Move[] { Move };
                    Count = 1;
                }

                //атака черным конем на 2 часа
                if ((knightXY.r - 1 > 0) & (knightXY.c + 2 < 7) &
                    ((board._board[knightXY.r - 1, knightXY.c + 2] == Cell.WhitePawn) |
                    (board._board[knightXY.r - 1, knightXY.c + 2] == Cell.WhiteKnight) |
                    (board._board[knightXY.r - 1, knightXY.c + 2] == Cell.WhiteBishop) |
                    (board._board[knightXY.r - 1, knightXY.c + 2] == Cell.WhiteRook) |
                    (board._board[knightXY.r - 1, knightXY.c + 2] == Cell.WhiteQueen) |
                    (board._board[knightXY.r - 1, knightXY.c + 2] == Cell.WhiteKing) |
                    (board._board[knightXY.r - 1, knightXY.c + 2] == Cell.Empty)))
                {
                    Move = new Move(knightXY, knightXY.r - 1, knightXY.c + 2);
                    if (Count == 1)
                    {
                        Temp = Moves;
                        Moves = new Move[] { Temp[1], Move };
                        Count = 2;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //атака черным конем на 4 часа
                if ((knightXY.r + 1 < 7) & (knightXY.c + 2 < 7) &
                    ((board._board[knightXY.r + 1, knightXY.c + 2] == Cell.WhitePawn) |
                    (board._board[knightXY.r + 1, knightXY.c + 2] == Cell.WhiteKnight) |
                    (board._board[knightXY.r + 1, knightXY.c + 2] == Cell.WhiteBishop) |
                    (board._board[knightXY.r + 1, knightXY.c + 2] == Cell.WhiteRook) |
                    (board._board[knightXY.r + 1, knightXY.c + 2] == Cell.WhiteQueen) |
                    (board._board[knightXY.r + 1, knightXY.c + 2] == Cell.WhiteKing) |
                    (board._board[knightXY.r + 1, knightXY.c + 2] == Cell.Empty)))
                {
                    Move = new Move(knightXY, knightXY.r + 1, knightXY.c + 2);
                    if (Count > 0)
                    {
                        Temp = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //атака черным конем на 5 часов
                if ((knightXY.r + 2 < 7) & (knightXY.c + 1 < 7) &
                    ((board._board[knightXY.r + 2, knightXY.c + 1] == Cell.WhitePawn) |
                    (board._board[knightXY.r + 2, knightXY.c + 1] == Cell.WhiteKnight) |
                    (board._board[knightXY.r + 2, knightXY.c + 1] == Cell.WhiteBishop) |
                    (board._board[knightXY.r + 2, knightXY.c + 1] == Cell.WhiteRook) |
                    (board._board[knightXY.r + 2, knightXY.c + 1] == Cell.WhiteQueen) |
                    (board._board[knightXY.r + 2, knightXY.c + 1] == Cell.WhiteKing) |
                    (board._board[knightXY.r + 2, knightXY.c + 1] == Cell.Empty)))
                {
                    Move = new Move(knightXY, knightXY.r + 2, knightXY.c + 1);
                    if (Count > 0)
                    {
                        Temp = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //атака черным конем на 7 часов
                if ((knightXY.r + 2 < 7) & (knightXY.c - 1 > 0) &
                    ((board._board[knightXY.r + 2, knightXY.c - 1] == Cell.WhitePawn) |
                    (board._board[knightXY.r + 2, knightXY.c - 1] == Cell.WhiteKnight) |
                    (board._board[knightXY.r + 2, knightXY.c - 1] == Cell.WhiteBishop) |
                    (board._board[knightXY.r + 2, knightXY.c - 1] == Cell.WhiteRook) |
                    (board._board[knightXY.r + 2, knightXY.c - 1] == Cell.WhiteQueen) |
                    (board._board[knightXY.r + 2, knightXY.c - 1] == Cell.WhiteKing) |
                    (board._board[knightXY.r + 2, knightXY.c - 1] == Cell.Empty)))
                {
                    Move = new Move(knightXY, knightXY.r + 2, knightXY.c - 1);
                    if (Count > 0)
                    {
                        Temp = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //атака черным конем на 8 часов
                if ((knightXY.r + 1 < 7) & (knightXY.c - 2 > 0) &
                    ((board._board[knightXY.r + 1, knightXY.c - 2] == Cell.WhitePawn) |
                    (board._board[knightXY.r + 1, knightXY.c - 2] == Cell.WhiteKnight) |
                    (board._board[knightXY.r + 1, knightXY.c - 2] == Cell.WhiteBishop) |
                    (board._board[knightXY.r + 1, knightXY.c - 2] == Cell.WhiteRook) |
                    (board._board[knightXY.r + 1, knightXY.c - 2] == Cell.WhiteQueen) |
                    (board._board[knightXY.r + 1, knightXY.c - 2] == Cell.WhiteKing) |
                    (board._board[knightXY.r + 1, knightXY.c - 2] == Cell.Empty)))
                {
                    Move = new Move(knightXY, knightXY.r + 1, knightXY.c - 2);
                    if (Count > 0)
                    {
                        Temp = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //атака черным конем на 10 часов
                if ((knightXY.r - 1 > 0) & (knightXY.c - 2 > 0) &
                    ((board._board[knightXY.r - 1, knightXY.c - 2] == Cell.WhitePawn) |
                    (board._board[knightXY.r - 1, knightXY.c - 2] == Cell.WhiteKnight) |
                    (board._board[knightXY.r - 1, knightXY.c - 2] == Cell.WhiteBishop) |
                    (board._board[knightXY.r - 1, knightXY.c - 2] == Cell.WhiteRook) |
                    (board._board[knightXY.r - 1, knightXY.c - 2] == Cell.WhiteQueen) |
                    (board._board[knightXY.r - 1, knightXY.c - 2] == Cell.WhiteKing) |
                    (board._board[knightXY.r - 1, knightXY.c - 2] == Cell.Empty)))
                {
                    Move = new Move(knightXY, knightXY.r - 1, knightXY.c - 2);
                    if (Count > 0)
                    {
                        Temp = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //атака черным конем на 11 часов
                if ((knightXY.r - 2 > 0) & (knightXY.c - 1 > 0) &
                    ((board._board[knightXY.r - 2, knightXY.c - 1] == Cell.WhitePawn) |
                    (board._board[knightXY.r - 2, knightXY.c - 1] == Cell.WhiteKnight) |
                    (board._board[knightXY.r - 2, knightXY.c - 1] == Cell.WhiteBishop) |
                    (board._board[knightXY.r - 2, knightXY.c - 1] == Cell.WhiteRook) |
                    (board._board[knightXY.r - 2, knightXY.c - 1] == Cell.WhiteQueen) |
                    (board._board[knightXY.r - 2, knightXY.c - 1] == Cell.WhiteKing) |
                    (board._board[knightXY.r - 2, knightXY.c - 1] == Cell.Empty)))
                {
                    Move = new Move(knightXY, knightXY.r - 2, knightXY.c - 1);
                    if (Count > 0)
                    {
                        Temp = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }
            }

            return Moves;
        }

        /// <summary>
        /// Поиск всех ходов, которые может совершить слон
        /// </summary>
        Move[] FindBishopMoves(XY bishopXY, Board board, Color color)
        {
            Move[] Moves = new Move[0];//массив для хранени всех ходов слона
            Move[] Temp1;
            Move Move;
            int Count = 0; //количество найденных ходов слона
            int j;

            if (color == Color.White)//ищем ходы белого слона
            {
                //атака белым слоном на верх-право
                j = 1;
                while ((bishopXY.r - j > 0) & (bishopXY.c + j < 7) &
                    ((board._board[bishopXY.r - j, bishopXY.c + j] == Cell.BlackPawn) |
                    (board._board[bishopXY.r - j, bishopXY.c + j] == Cell.BlackKnight) |
                    (board._board[bishopXY.r - j, bishopXY.c + j] == Cell.BlackBishop) |
                    (board._board[bishopXY.r - j, bishopXY.c + j] == Cell.BlackRook) |
                    (board._board[bishopXY.r - j, bishopXY.c + j] == Cell.BlackQueen) |
                    (board._board[bishopXY.r - j, bishopXY.c + j] == Cell.BlackKing) |
                    (board._board[bishopXY.r - j, bishopXY.c + j] == Cell.Empty)))
                {
                    Move = new Move(bishopXY, bishopXY.r - j, bishopXY.c + j);
                    if (Count > 0)
                    {
                        Temp1 = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp1[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                    j++;
                }

                //атака белым слоном вниз-вправо
                j = 1;
                while ((bishopXY.r + j > 0) & (bishopXY.c + j < 7) &
                    ((board._board[bishopXY.r + j, bishopXY.c + j] == Cell.BlackPawn) |
                    (board._board[bishopXY.r + j, bishopXY.c + j] == Cell.BlackKnight) |
                    (board._board[bishopXY.r + j, bishopXY.c + j] == Cell.BlackBishop) |
                    (board._board[bishopXY.r + j, bishopXY.c + j] == Cell.BlackRook) |
                    (board._board[bishopXY.r + j, bishopXY.c + j] == Cell.BlackQueen) |
                    (board._board[bishopXY.r + j, bishopXY.c + j] == Cell.BlackKing) |
                    (board._board[bishopXY.r + j, bishopXY.c + j] == Cell.Empty)))
                {
                    Move = new Move(bishopXY, bishopXY.r + j, bishopXY.c + j);
                    if (Count > 0)
                    {
                        Temp1 = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp1[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                    j++;
                }

                //атака белым слоном вниз-влево
                j = 1;
                while ((bishopXY.r + j > 0) & (bishopXY.c - j < 7) &
                    ((board._board[bishopXY.r + j, bishopXY.c - j] == Cell.BlackPawn) |
                    (board._board[bishopXY.r + j, bishopXY.c - j] == Cell.BlackKnight) |
                    (board._board[bishopXY.r + j, bishopXY.c - j] == Cell.BlackBishop) |
                    (board._board[bishopXY.r + j, bishopXY.c - j] == Cell.BlackRook) |
                    (board._board[bishopXY.r + j, bishopXY.c - j] == Cell.BlackQueen) |
                    (board._board[bishopXY.r + j, bishopXY.c - j] == Cell.BlackKing) |
                    (board._board[bishopXY.r + j, bishopXY.c - j] == Cell.Empty)))
                {
                    Move = new Move(bishopXY, bishopXY.r + j, bishopXY.c - j);
                    if (Count > 0)
                    {
                        Temp1 = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp1[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                    j++;
                }

                //атака белым слоном вверх-влево
                j = 1;
                while ((bishopXY.r - j > 0) & (bishopXY.c - j < 7) &
                    ((board._board[bishopXY.r - j, bishopXY.c - j] == Cell.BlackPawn) |
                    (board._board[bishopXY.r - j, bishopXY.c - j] == Cell.BlackKnight) |
                    (board._board[bishopXY.r - j, bishopXY.c - j] == Cell.BlackBishop) |
                    (board._board[bishopXY.r - j, bishopXY.c - j] == Cell.BlackRook) |
                    (board._board[bishopXY.r - j, bishopXY.c - j] == Cell.BlackQueen) |
                    (board._board[bishopXY.r - j, bishopXY.c - j] == Cell.BlackKing) |
                    (board._board[bishopXY.r - j, bishopXY.c - j] == Cell.Empty)))
                {
                    Move = new Move(bishopXY, bishopXY.r - j, bishopXY.c - j);
                    if (Count > 0)
                    {
                        Temp1 = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp1[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                    j++;
                }

            }
            else//Если мы ищем атаки черного слона
            {
                //атака черным слоном на верх-право
                j = 1;
                while ((bishopXY.r - j > 0) & (bishopXY.c + j < 7) &
                    ((board._board[bishopXY.r - j, bishopXY.c + j] == Cell.WhitePawn) |
                    (board._board[bishopXY.r - j, bishopXY.c + j] == Cell.WhiteKnight) |
                    (board._board[bishopXY.r - j, bishopXY.c + j] == Cell.WhiteBishop) |
                    (board._board[bishopXY.r - j, bishopXY.c + j] == Cell.WhiteRook) |
                    (board._board[bishopXY.r - j, bishopXY.c + j] == Cell.WhiteQueen) |
                    (board._board[bishopXY.r - j, bishopXY.c + j] == Cell.WhiteKing) |
                    (board._board[bishopXY.r - j, bishopXY.c + j] == Cell.Empty)))
                {
                    Move = new Move(bishopXY, bishopXY.r - j, bishopXY.c + j);
                    if (Count > 0)
                    {
                        Temp1 = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp1[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                    j++;
                }

                //атака черным слоном вниз-вправо
                j = 1;
                while ((bishopXY.r + j > 0) & (bishopXY.c + j < 7) &
                    ((board._board[bishopXY.r + j, bishopXY.c + j] == Cell.WhitePawn) |
                    (board._board[bishopXY.r + j, bishopXY.c + j] == Cell.WhiteKnight) |
                    (board._board[bishopXY.r + j, bishopXY.c + j] == Cell.WhiteBishop) |
                    (board._board[bishopXY.r + j, bishopXY.c + j] == Cell.WhiteRook) |
                    (board._board[bishopXY.r + j, bishopXY.c + j] == Cell.WhiteQueen) |
                    (board._board[bishopXY.r + j, bishopXY.c + j] == Cell.WhiteKing) |
                    (board._board[bishopXY.r + j, bishopXY.c + j] == Cell.Empty)))
                {
                    Move = new Move(bishopXY, bishopXY.r + j, bishopXY.c + j);
                    if (Count > 0)
                    {
                        Temp1 = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp1[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                    j++;
                }

                //атака черным слоном вниз-влево
                j = 1;
                while ((bishopXY.r + j > 0) & (bishopXY.c - j < 7) &
                    ((board._board[bishopXY.r + j, bishopXY.c - j] == Cell.WhitePawn) |
                    (board._board[bishopXY.r + j, bishopXY.c - j] == Cell.WhiteKnight) |
                    (board._board[bishopXY.r + j, bishopXY.c - j] == Cell.WhiteBishop) |
                    (board._board[bishopXY.r + j, bishopXY.c - j] == Cell.WhiteRook) |
                    (board._board[bishopXY.r + j, bishopXY.c - j] == Cell.WhiteQueen) |
                    (board._board[bishopXY.r + j, bishopXY.c - j] == Cell.WhiteKing) |
                    (board._board[bishopXY.r + j, bishopXY.c - j] == Cell.Empty)))
                {
                    Move = new Move(bishopXY, bishopXY.r + j, bishopXY.c - j);
                    if (Count > 0)
                    {
                        Temp1 = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp1[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                    j++;
                }

                //атака черным слоном вверх-влево
                j = 1;
                while ((bishopXY.r - j > 0) & (bishopXY.c - j < 7) &
                    ((board._board[bishopXY.r - j, bishopXY.c - j] == Cell.WhitePawn) |
                    (board._board[bishopXY.r - j, bishopXY.c - j] == Cell.WhiteKnight) |
                    (board._board[bishopXY.r - j, bishopXY.c - j] == Cell.WhiteBishop) |
                    (board._board[bishopXY.r - j, bishopXY.c - j] == Cell.WhiteRook) |
                    (board._board[bishopXY.r - j, bishopXY.c - j] == Cell.WhiteQueen) |
                    (board._board[bishopXY.r - j, bishopXY.c - j] == Cell.WhiteKing) |
                    (board._board[bishopXY.r - j, bishopXY.c - j] == Cell.Empty)))
                {
                    Move = new Move(bishopXY, bishopXY.r - j, bishopXY.c - j);
                    if (Count > 0)
                    {
                        Temp1 = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp1[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                    j++;
                }
            }

            return Moves;
        }

        /// <summary>
        /// Поиск всех ходов, которые может совершить ладья
        /// </summary>
        Move[] FindRookMoves(XY rookXY, Board board, Color color)
        {
            Move[] Moves = new Move[0];//массив для хранени всех ходов ладьи
            Move[] Temp1;
            Move Move;
            int Count = 0; //количество найденных ходов ладьи
            int j;

            if (color == Color.White)//ищем ходы белой ладьи
            {
                //атака белой ладьей вправо
                j = 1;
                while ((rookXY.c + j < 7) &
                    ((board._board[rookXY.r, rookXY.c + j] == Cell.BlackPawn) |
                    (board._board[rookXY.r, rookXY.c + j] == Cell.BlackKnight) |
                    (board._board[rookXY.r, rookXY.c + j] == Cell.BlackBishop) |
                    (board._board[rookXY.r, rookXY.c + j] == Cell.BlackRook) |
                    (board._board[rookXY.r, rookXY.c + j] == Cell.BlackQueen) |
                    (board._board[rookXY.r, rookXY.c + j] == Cell.BlackKing) |
                    (board._board[rookXY.r, rookXY.c + j] == Cell.Empty)))
                {
                    Move = new Move(rookXY, rookXY.r, rookXY.c + j);
                    if (Count > 0)
                    {
                        Temp1 = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp1[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                    j++;
                }

                //атака белой ладьей вниз
                j = 1;
                while ((rookXY.r + j < 7) &
                    ((board._board[rookXY.r + j, rookXY.c] == Cell.BlackPawn) |
                    (board._board[rookXY.r + j, rookXY.c] == Cell.BlackKnight) |
                    (board._board[rookXY.r + j, rookXY.c] == Cell.BlackBishop) |
                    (board._board[rookXY.r + j, rookXY.c] == Cell.BlackRook) |
                    (board._board[rookXY.r + j, rookXY.c] == Cell.BlackQueen) |
                    (board._board[rookXY.r + j, rookXY.c] == Cell.BlackKing) |
                    (board._board[rookXY.r + j, rookXY.c] == Cell.Empty)))
                {
                    Move = new Move(rookXY, rookXY.r + j, rookXY.c);
                    if (Count > 0)
                    {
                        Temp1 = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp1[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                    j++;
                }

                //атака белой ладьей вправо
                j = 1;
                while ((rookXY.c - j > 0) &
                    ((board._board[rookXY.r, rookXY.c - j] == Cell.BlackPawn) |
                    (board._board[rookXY.r, rookXY.c - j] == Cell.BlackKnight) |
                    (board._board[rookXY.r, rookXY.c - j] == Cell.BlackBishop) |
                    (board._board[rookXY.r, rookXY.c - j] == Cell.BlackRook) |
                    (board._board[rookXY.r, rookXY.c - j] == Cell.BlackQueen) |
                    (board._board[rookXY.r, rookXY.c - j] == Cell.BlackKing) |
                    (board._board[rookXY.r, rookXY.c - j] == Cell.Empty)))
                {
                    Move = new Move(rookXY, rookXY.r, rookXY.c - j);
                    if (Count > 0)
                    {
                        Temp1 = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp1[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                    j++;
                }

                //атака белой ладьей вправо
                j = 1;
                while ((rookXY.r - j > 0) &
                    ((board._board[rookXY.r - j, rookXY.c] == Cell.BlackPawn) |
                    (board._board[rookXY.r - j, rookXY.c] == Cell.BlackKnight) |
                    (board._board[rookXY.r - j, rookXY.c] == Cell.BlackBishop) |
                    (board._board[rookXY.r - j, rookXY.c] == Cell.BlackRook) |
                    (board._board[rookXY.r - j, rookXY.c] == Cell.BlackQueen) |
                    (board._board[rookXY.r - j, rookXY.c] == Cell.BlackKing) |
                    (board._board[rookXY.r - j, rookXY.c] == Cell.Empty)))
                {
                    Move = new Move(rookXY, rookXY.r - j, rookXY.c);
                    if (Count > 0)
                    {
                        Temp1 = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp1[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                    j++;
                }

            }
            else//Если мы ищем атаки черной ладьи
            {
                //атака черной ладьей вправо
                j = 1;
                while ((rookXY.c + j < 7) &
                    ((board._board[rookXY.r, rookXY.c + j] == Cell.WhitePawn) |
                    (board._board[rookXY.r, rookXY.c + j] == Cell.WhiteKnight) |
                    (board._board[rookXY.r, rookXY.c + j] == Cell.WhiteBishop) |
                    (board._board[rookXY.r, rookXY.c + j] == Cell.WhiteRook) |
                    (board._board[rookXY.r, rookXY.c + j] == Cell.WhiteQueen) |
                    (board._board[rookXY.r, rookXY.c + j] == Cell.WhiteKing) |
                    (board._board[rookXY.r, rookXY.c + j] == Cell.Empty)))
                {
                    Move = new Move(rookXY, rookXY.r, rookXY.c + j);
                    if (Count > 0)
                    {
                        Temp1 = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp1[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                    j++;
                }

                //атака черной ладьей вниз
                j = 1;
                while ((rookXY.r + j < 7) &
                    ((board._board[rookXY.r + j, rookXY.c] == Cell.WhitePawn) |
                    (board._board[rookXY.r + j, rookXY.c] == Cell.WhiteKnight) |
                    (board._board[rookXY.r + j, rookXY.c] == Cell.WhiteBishop) |
                    (board._board[rookXY.r + j, rookXY.c] == Cell.WhiteRook) |
                    (board._board[rookXY.r + j, rookXY.c] == Cell.WhiteQueen) |
                    (board._board[rookXY.r + j, rookXY.c] == Cell.WhiteKing) |
                    (board._board[rookXY.r + j, rookXY.c] == Cell.Empty)))
                {
                    Move = new Move(rookXY, rookXY.r + j, rookXY.c);
                    if (Count > 0)
                    {
                        Temp1 = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp1[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                    j++;
                }

                //атака черной ладьей вправо
                j = 1;
                while ((rookXY.c - j > 0) &
                    ((board._board[rookXY.r, rookXY.c - j] == Cell.WhitePawn) |
                    (board._board[rookXY.r, rookXY.c - j] == Cell.WhiteKnight) |
                    (board._board[rookXY.r, rookXY.c - j] == Cell.WhiteBishop) |
                    (board._board[rookXY.r, rookXY.c - j] == Cell.WhiteRook) |
                    (board._board[rookXY.r, rookXY.c - j] == Cell.WhiteQueen) |
                    (board._board[rookXY.r, rookXY.c - j] == Cell.WhiteKing) |
                    (board._board[rookXY.r, rookXY.c - j] == Cell.Empty)))
                {
                    Move = new Move(rookXY, rookXY.r, rookXY.c - j);
                    if (Count > 0)
                    {
                        Temp1 = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp1[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                    j++;
                }

                //атака черной ладьей вправо
                j = 1;
                while ((rookXY.r - j > 0) &
                    ((board._board[rookXY.r - j, rookXY.c] == Cell.WhitePawn) |
                    (board._board[rookXY.r - j, rookXY.c] == Cell.WhiteKnight) |
                    (board._board[rookXY.r - j, rookXY.c] == Cell.WhiteBishop) |
                    (board._board[rookXY.r - j, rookXY.c] == Cell.WhiteRook) |
                    (board._board[rookXY.r - j, rookXY.c] == Cell.WhiteQueen) |
                    (board._board[rookXY.r - j, rookXY.c] == Cell.WhiteKing) |
                    (board._board[rookXY.r - j, rookXY.c] == Cell.Empty)))
                {
                    Move = new Move(rookXY, rookXY.r - j, rookXY.c);
                    if (Count > 0)
                    {
                        Temp1 = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp1[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                    j++;
                }
            }

            return Moves;
        }

        /// <summary>
        /// Поиск всех ходов, которые может совершить ферзь
        /// </summary>
        Move[] FindQueenMoves(XY queenXY, Board board, Color color)
        {
            Move[] Moves = new Move[0];//массив для хранени всех ходов ферзя
            Move[] Temp1;
            Move[] Temp2;
            int Count = 0; //количество найденных ходов ферзя

            //атаки ферзя по диагонали
            Temp2 = FindBishopMoves(queenXY, board, color);
            if (Temp2 != null)
            {
                int CountFound = Temp2.Length;
                if (Count > 0)
                {
                    Temp1 = Moves;
                    Moves = new Move[Count + CountFound];
                    for (int i = 0; i < Count; i++)
                    {
                        Moves[i] = Temp1[i];
                    }
                    for (int i = Count; i < Count + CountFound; i++)
                    {
                        Moves[i] = Temp1[i];
                    }
                    Count++;
                }
                else
                {
                    Moves = Temp2;
                    Count = CountFound;
                }
            }

            //атаки ферзя по горизонтали
            Temp2 = FindRookMoves(queenXY, board, color);
            if (Temp2 != null)
            {
                int CountFound = Temp2.Length;
                if (Count > 0)
                {
                    Temp1 = Moves;
                    Moves = new Move[Count + CountFound];
                    for (int i = 0; i < Count; i++)
                    {
                        Moves[i] = Temp1[i];
                    }
                    for (int i = Count; i < Count + CountFound; i++)
                    {
                        Moves[i] = Temp1[i];
                    }
                    Count++;
                }
                else
                {
                    Moves = Temp2;
                    Count = CountFound;
                }
            }

            return Moves;
        }

        /// <summary>
        /// Поиск всех ходов, которые может совершить король
        /// TODO: королю нельзя приближаться к другому королю
        /// TODO: королю нельзя подставляться под шах
        /// </summary>
        Move[] FindKingMoves(XY kingXY, Board board, Color color)
        {
            //король может совершить всего 8 ходов во все стороны на одну клетку
            Move[] Moves = new Move[0];//массив для хранени всех найденных ходов короля
            Move[] Temp;
            Move Move;
            int Count = 0; //количество найденных ходов пешки

            if (color == Color.White)//ищем ходы белого короля
            {
                //ход белого короля на одну клетку вправо
                if ((kingXY.c + 1 < 7) &
                    ((board._board[kingXY.r, kingXY.c + 1] == Cell.BlackPawn) |
                    (board._board[kingXY.r, kingXY.c + 1] == Cell.BlackKnight) |
                    (board._board[kingXY.r, kingXY.c + 1] == Cell.BlackBishop) |
                    (board._board[kingXY.r, kingXY.c + 1] == Cell.BlackRook) |
                    (board._board[kingXY.r, kingXY.c + 1] == Cell.BlackQueen) |
                    (board._board[kingXY.r, kingXY.c + 1] == Cell.BlackKing) |
                    (board._board[kingXY.r, kingXY.c + 1] == Cell.Empty)))
                {
                    Move = new Move(kingXY, kingXY.r, kingXY.c + 1);
                    Moves = new Move[] { Move };
                    Count = 1;
                }

                //ход белого короля на одну клетку вниз
                if ((kingXY.r + 1 < 7) &
                    ((board._board[kingXY.r + 1, kingXY.c] == Cell.BlackPawn) |
                    (board._board[kingXY.r + 1, kingXY.c] == Cell.BlackKnight) |
                    (board._board[kingXY.r + 1, kingXY.c] == Cell.BlackBishop) |
                    (board._board[kingXY.r + 1, kingXY.c] == Cell.BlackRook) |
                    (board._board[kingXY.r + 1, kingXY.c] == Cell.BlackQueen) |
                    (board._board[kingXY.r + 1, kingXY.c] == Cell.BlackKing) |
                    (board._board[kingXY.r + 1, kingXY.c] == Cell.Empty)))
                {
                    Move = new Move(kingXY, kingXY.r + 1, kingXY.c);
                    if (Count == 1)
                    {
                        Temp = Moves;
                        Moves = new Move[] { Temp[0], Move };
                        Count = 2;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //ход белого короля на одну клетку влево
                if ((kingXY.c - 1 > 0) &
                    ((board._board[kingXY.r, kingXY.c - 1] == Cell.BlackPawn) |
                    (board._board[kingXY.r, kingXY.c - 1] == Cell.BlackKnight) |
                    (board._board[kingXY.r, kingXY.c - 1] == Cell.BlackBishop) |
                    (board._board[kingXY.r, kingXY.c - 1] == Cell.BlackRook) |
                    (board._board[kingXY.r, kingXY.c - 1] == Cell.BlackQueen) |
                    (board._board[kingXY.r, kingXY.c - 1] == Cell.BlackKing) |
                    (board._board[kingXY.r, kingXY.c - 1] == Cell.Empty)))
                {
                    Move = new Move(kingXY, kingXY.r, kingXY.c - 1);
                    if (Count > 0)
                    {
                        Temp = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //ход белого короля на одну клетку вверх
                if ((kingXY.r - 1 > 0) &
                    ((board._board[kingXY.r - 1, kingXY.c] == Cell.BlackPawn) |
                    (board._board[kingXY.r - 1, kingXY.c] == Cell.BlackKnight) |
                    (board._board[kingXY.r - 1, kingXY.c] == Cell.BlackBishop) |
                    (board._board[kingXY.r - 1, kingXY.c] == Cell.BlackRook) |
                    (board._board[kingXY.r - 1, kingXY.c] == Cell.BlackQueen) |
                    (board._board[kingXY.r - 1, kingXY.c] == Cell.BlackKing) |
                    (board._board[kingXY.r - 1, kingXY.c] == Cell.Empty)))
                {
                    Move = new Move(kingXY, kingXY.r - 1, kingXY.c);
                    if (Count > 0)
                    {
                        Temp = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //ход белого короля вверх-вправо
                if ((kingXY.r - 1 > 0) & (kingXY.c + 1 < 7) &
                    ((board._board[kingXY.r - 1, kingXY.c + 1] == Cell.BlackPawn) |
                    (board._board[kingXY.r - 1, kingXY.c + 1] == Cell.BlackKnight) |
                    (board._board[kingXY.r - 1, kingXY.c + 1] == Cell.BlackBishop) |
                    (board._board[kingXY.r - 1, kingXY.c + 1] == Cell.BlackRook) |
                    (board._board[kingXY.r - 1, kingXY.c + 1] == Cell.BlackQueen) |
                    (board._board[kingXY.r - 1, kingXY.c + 1] == Cell.BlackKing) |
                    (board._board[kingXY.r - 1, kingXY.c + 1] == Cell.Empty)))
                {
                    Move = new Move(kingXY, kingXY.r - 1, kingXY.c + 1);
                    if (Count > 0)
                    {
                        Temp = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //ход белого короля вниз-вправо
                if ((kingXY.r + 1 < 7) & (kingXY.c + 1 < 7) &
                    ((board._board[kingXY.r + 1, kingXY.c + 1] == Cell.BlackPawn) |
                    (board._board[kingXY.r + 1, kingXY.c + 1] == Cell.BlackKnight) |
                    (board._board[kingXY.r + 1, kingXY.c + 1] == Cell.BlackBishop) |
                    (board._board[kingXY.r + 1, kingXY.c + 1] == Cell.BlackRook) |
                    (board._board[kingXY.r + 1, kingXY.c + 1] == Cell.BlackQueen) |
                    (board._board[kingXY.r + 1, kingXY.c + 1] == Cell.BlackKing) |
                    (board._board[kingXY.r + 1, kingXY.c + 1] == Cell.Empty)))
                {
                    Move = new Move(kingXY, kingXY.r + 1, kingXY.c + 1);
                    if (Count > 0)
                    {
                        Temp = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //ход белого короля вниз-влево
                if ((kingXY.r + 1 < 7) & (kingXY.c - 1 > 0) &
                    ((board._board[kingXY.r + 1, kingXY.c - 1] == Cell.BlackPawn) |
                    (board._board[kingXY.r + 1, kingXY.c - 1] == Cell.BlackKnight) |
                    (board._board[kingXY.r + 1, kingXY.c - 1] == Cell.BlackBishop) |
                    (board._board[kingXY.r + 1, kingXY.c - 1] == Cell.BlackRook) |
                    (board._board[kingXY.r + 1, kingXY.c - 1] == Cell.BlackQueen) |
                    (board._board[kingXY.r + 1, kingXY.c - 1] == Cell.BlackKing) |
                    (board._board[kingXY.r + 1, kingXY.c - 1] == Cell.Empty)))
                {
                    Move = new Move(kingXY, kingXY.r + 1, kingXY.c - 1);
                    if (Count > 0)
                    {
                        Temp = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //ход белого короля вверх-влево
                if ((kingXY.r - 1 > 0) & (kingXY.c - 1 > 0) &
                    ((board._board[kingXY.r - 1, kingXY.c - 1] == Cell.BlackPawn) |
                    (board._board[kingXY.r - 1, kingXY.c - 1] == Cell.BlackKnight) |
                    (board._board[kingXY.r - 1, kingXY.c - 1] == Cell.BlackBishop) |
                    (board._board[kingXY.r - 1, kingXY.c - 1] == Cell.BlackRook) |
                    (board._board[kingXY.r - 1, kingXY.c - 1] == Cell.BlackQueen) |
                    (board._board[kingXY.r - 1, kingXY.c - 1] == Cell.BlackKing) |
                    (board._board[kingXY.r - 1, kingXY.c - 1] == Cell.Empty)))
                {
                    Move = new Move(kingXY, kingXY.r - 1, kingXY.c - 1);
                    if (Count > 0)
                    {
                        Temp = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }
            }
            else//Если мы ищем атаки черного короля
            {
                //ход черного короля на одну клетку вправо
                if ((kingXY.c + 1 < 7) &
                    ((board._board[kingXY.r, kingXY.c + 1] == Cell.WhitePawn) |
                    (board._board[kingXY.r, kingXY.c + 1] == Cell.WhiteKnight) |
                    (board._board[kingXY.r, kingXY.c + 1] == Cell.WhiteBishop) |
                    (board._board[kingXY.r, kingXY.c + 1] == Cell.WhiteRook) |
                    (board._board[kingXY.r, kingXY.c + 1] == Cell.WhiteQueen) |
                    (board._board[kingXY.r, kingXY.c + 1] == Cell.WhiteKing) |
                    (board._board[kingXY.r, kingXY.c + 1] == Cell.Empty)))
                {
                    Move = new Move(kingXY, kingXY.r, kingXY.c + 1);
                    Moves = new Move[] { Move };
                    Count = 1;
                }

                //ход черного короля на одну клетку вниз
                if ((kingXY.r + 1 < 7) &
                    ((board._board[kingXY.r + 1, kingXY.c] == Cell.WhitePawn) |
                    (board._board[kingXY.r + 1, kingXY.c] == Cell.WhiteKnight) |
                    (board._board[kingXY.r + 1, kingXY.c] == Cell.WhiteBishop) |
                    (board._board[kingXY.r + 1, kingXY.c] == Cell.WhiteRook) |
                    (board._board[kingXY.r + 1, kingXY.c] == Cell.WhiteQueen) |
                    (board._board[kingXY.r + 1, kingXY.c] == Cell.WhiteKing) |
                    (board._board[kingXY.r + 1, kingXY.c] == Cell.Empty)))
                {
                    Move = new Move(kingXY, kingXY.r + 1, kingXY.c);
                    if (Count == 1)
                    {
                        Temp = Moves;
                        Moves = new Move[] { Temp[0], Move };
                        Count = 2;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //ход черного короля на одну клетку влево
                if ((kingXY.c - 1 > 0) &
                    ((board._board[kingXY.r, kingXY.c - 1] == Cell.WhitePawn) |
                    (board._board[kingXY.r, kingXY.c - 1] == Cell.WhiteKnight) |
                    (board._board[kingXY.r, kingXY.c - 1] == Cell.WhiteBishop) |
                    (board._board[kingXY.r, kingXY.c - 1] == Cell.WhiteRook) |
                    (board._board[kingXY.r, kingXY.c - 1] == Cell.WhiteQueen) |
                    (board._board[kingXY.r, kingXY.c - 1] == Cell.WhiteKing) |
                    (board._board[kingXY.r, kingXY.c - 1] == Cell.Empty)))
                {
                    Move = new Move(kingXY, kingXY.r, kingXY.c - 1);
                    if (Count > 0)
                    {
                        Temp = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //ход черного короля на одну клетку вверх
                if ((kingXY.r - 1 > 0) &
                    ((board._board[kingXY.r - 1, kingXY.c] == Cell.WhitePawn) |
                    (board._board[kingXY.r - 1, kingXY.c] == Cell.WhiteKnight) |
                    (board._board[kingXY.r - 1, kingXY.c] == Cell.WhiteBishop) |
                    (board._board[kingXY.r - 1, kingXY.c] == Cell.WhiteRook) |
                    (board._board[kingXY.r - 1, kingXY.c] == Cell.WhiteQueen) |
                    (board._board[kingXY.r - 1, kingXY.c] == Cell.WhiteKing) |
                    (board._board[kingXY.r - 1, kingXY.c] == Cell.Empty)))
                {
                    Move = new Move(kingXY, kingXY.r - 1, kingXY.c);
                    if (Count > 0)
                    {
                        Temp = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //ход черного короля вверх-вправо
                if ((kingXY.r - 1 > 0) & (kingXY.c + 1 < 7) &
                    ((board._board[kingXY.r - 1, kingXY.c + 1] == Cell.WhitePawn) |
                    (board._board[kingXY.r - 1, kingXY.c + 1] == Cell.WhiteKnight) |
                    (board._board[kingXY.r - 1, kingXY.c + 1] == Cell.WhiteBishop) |
                    (board._board[kingXY.r - 1, kingXY.c + 1] == Cell.WhiteRook) |
                    (board._board[kingXY.r - 1, kingXY.c + 1] == Cell.WhiteQueen) |
                    (board._board[kingXY.r - 1, kingXY.c + 1] == Cell.WhiteKing) |
                    (board._board[kingXY.r - 1, kingXY.c + 1] == Cell.Empty)))
                {
                    Move = new Move(kingXY, kingXY.r - 1, kingXY.c + 1);
                    if (Count > 0)
                    {
                        Temp = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //ход черного короля вниз-вправо
                if ((kingXY.r + 1 < 7) & (kingXY.c + 1 < 7) &
                    ((board._board[kingXY.r + 1, kingXY.c + 1] == Cell.WhitePawn) |
                    (board._board[kingXY.r + 1, kingXY.c + 1] == Cell.WhiteKnight) |
                    (board._board[kingXY.r + 1, kingXY.c + 1] == Cell.WhiteBishop) |
                    (board._board[kingXY.r + 1, kingXY.c + 1] == Cell.WhiteRook) |
                    (board._board[kingXY.r + 1, kingXY.c + 1] == Cell.WhiteQueen) |
                    (board._board[kingXY.r + 1, kingXY.c + 1] == Cell.WhiteKing) |
                    (board._board[kingXY.r + 1, kingXY.c + 1] == Cell.Empty)))
                {
                    Move = new Move(kingXY, kingXY.r + 1, kingXY.c + 1);
                    if (Count > 0)
                    {
                        Temp = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //ход черного короля вниз-влево
                if ((kingXY.r + 1 < 7) & (kingXY.c - 1 > 0) &
                    ((board._board[kingXY.r + 1, kingXY.c - 1] == Cell.WhitePawn) |
                    (board._board[kingXY.r + 1, kingXY.c - 1] == Cell.WhiteKnight) |
                    (board._board[kingXY.r + 1, kingXY.c - 1] == Cell.WhiteBishop) |
                    (board._board[kingXY.r + 1, kingXY.c - 1] == Cell.WhiteRook) |
                    (board._board[kingXY.r + 1, kingXY.c - 1] == Cell.WhiteQueen) |
                    (board._board[kingXY.r + 1, kingXY.c - 1] == Cell.WhiteKing) |
                    (board._board[kingXY.r + 1, kingXY.c - 1] == Cell.Empty)))
                {
                    Move = new Move(kingXY, kingXY.r + 1, kingXY.c - 1);
                    if (Count > 0)
                    {
                        Temp = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //ход черного короля вверх-влево
                if ((kingXY.r - 1 > 0) & (kingXY.c - 1 > 0) &
                    ((board._board[kingXY.r - 1, kingXY.c - 1] == Cell.WhitePawn) |
                    (board._board[kingXY.r - 1, kingXY.c - 1] == Cell.WhiteKnight) |
                    (board._board[kingXY.r - 1, kingXY.c - 1] == Cell.WhiteBishop) |
                    (board._board[kingXY.r - 1, kingXY.c - 1] == Cell.WhiteRook) |
                    (board._board[kingXY.r - 1, kingXY.c - 1] == Cell.WhiteQueen) |
                    (board._board[kingXY.r - 1, kingXY.c - 1] == Cell.WhiteKing) |
                    (board._board[kingXY.r - 1, kingXY.c - 1] == Cell.Empty)))
                {
                    Move = new Move(kingXY, kingXY.r - 1, kingXY.c - 1);
                    if (Count > 0)
                    {
                        Temp = Moves;
                        Moves = new Move[Count + 1];
                        for (int i = 0; i < Count; i++)
                        {
                            Moves[i] = Temp[i];
                        }
                        Moves[Count] = Move;
                        Count++;
                    }
                    else
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }
            }

            return Moves;
        }

        #endregion

        #region Checks
        /// <summary>
        /// Атакует ли пешка заданную клетку?
        /// </summary>
        bool CheckPawnAtacks(XY pawnXY, Color color, XY cellXY)
        {
            //Ответ на вопрос, находится ли заданная клетка под атакой конкретной пешки

            if (color == Color.White)//ищем атаки белой пешки
            {
                if (((cellXY.r == pawnXY.r - 1) & (cellXY.c == pawnXY.c - 1)) |
                    ((cellXY.r == pawnXY.r - 1) & (cellXY.c == pawnXY.c + 1)))
                {
                    return true;
                }
            }
            else//Если мы ищем атаки черной пешки
            {
                if (((cellXY.r == pawnXY.r + 1) & (cellXY.c == pawnXY.c - 1)) |
                    ((cellXY.r == pawnXY.r + 1) & (cellXY.c == pawnXY.c + 1)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Атакует ли конь заданную клетку?
        /// </summary>
        bool CheckKnightAtacks(XY knightXY, XY cellXY)
        {
            //Ответ на вопрос, находится ли заданная клетка под атакой конкретного коня

            if (((cellXY.r == knightXY.r - 2) & (cellXY.c == knightXY.c + 1)) |
                ((cellXY.r == knightXY.r - 1) & (cellXY.c == knightXY.c + 2)) |
                ((cellXY.r == knightXY.r + 1) & (cellXY.c == knightXY.c + 2)) |
                ((cellXY.r == knightXY.r + 2) & (cellXY.c == knightXY.c + 1)) |
                ((cellXY.r == knightXY.r + 2) & (cellXY.c == knightXY.c - 1)) |
                ((cellXY.r == knightXY.r + 1) & (cellXY.c == knightXY.c - 2)) |
                ((cellXY.r == knightXY.r - 1) & (cellXY.c == knightXY.c - 2)) |
                ((cellXY.r == knightXY.r - 2) & (cellXY.c == knightXY.c - 1)))
            {
                return true;
            }
            else { return false; }   
        }

        /// <summary>
        /// Атакует ли слон
        /// </summary>
        bool CheckBishopAtacks(XY bishopXY, XY cellXY)
        {
            //Ответ на вопрос, находится ли заданная клетка под атакой конкретного слона

            //для начала нужно убедиться что слон и атакованная клетка лежат на одной диагонали
            //для этого нужно убедиться что следующий параметр является целым числом
            float Ratio = Math.Abs(cellXY.r - bishopXY.r) / Math.Abs(cellXY.c - bishopXY.c);
            if (Ratio % 1 != 0)
            {
                return false;
            }

            if ((cellXY.r < bishopXY.r) & (cellXY.c > bishopXY.c))//клетка сверху-справа
            {

            }
            else if ((cellXY.r > bishopXY.r) & (cellXY.c > bishopXY.c))//клетка снизу-справа
            {

            }
            else if ((cellXY.r > bishopXY.r) & (cellXY.c < bishopXY.c))//клетка снизу-слева
            {

            }
            else if ((cellXY.r < bishopXY.r) & (cellXY.c < bishopXY.c))//клетка сверху-слева
            {

            }


                if (((cellXY.r == bishopXY.r - 2) & (cellXY.c == bishopXY.c + 1)) |
                ((cellXY.r == bishopXY.r - 1) & (cellXY.c == bishopXY.c + 2)) |
                ((cellXY.r == bishopXY.r + 1) & (cellXY.c == bishopXY.c + 2)) |
                ((cellXY.r == bishopXY.r + 2) & (cellXY.c == bishopXY.c + 1)) |
                ((cellXY.r == bishopXY.r + 2) & (cellXY.c == bishopXY.c - 1)) |
                ((cellXY.r == bishopXY.r + 1) & (cellXY.c == bishopXY.c - 2)) |
                ((cellXY.r == bishopXY.r - 1) & (cellXY.c == bishopXY.c - 2)) |
                ((cellXY.r == bishopXY.r - 2) & (cellXY.c == bishopXY.c - 1)))
            {
                return true;
            }
            else { return false; }
        }

        /// <summary>
        /// Атакует ли король заданную клетку?
        /// </summary>
        bool CheckKingAtacks(XY kingXY, XY cellXY)
        {
            //Ответ на вопрос, находится ли заданная клетка под атакой конкретного короля

            if (((cellXY.r == kingXY.r) & (cellXY.c == kingXY.c + 1)) |
                ((cellXY.r == kingXY.r + 1) & (cellXY.c == kingXY.c)) |
                ((cellXY.r == kingXY.r) & (cellXY.c == kingXY.c - 1)) |
                ((cellXY.r == kingXY.r - 1) & (cellXY.c == kingXY.c)) |
                ((cellXY.r == kingXY.r - 1) & (cellXY.c == kingXY.c + 1)) |
                ((cellXY.r == kingXY.r + 1) & (cellXY.c == kingXY.c + 1)) |
                ((cellXY.r == kingXY.r + 1) & (cellXY.c == kingXY.c - 1)) |
                ((cellXY.r == kingXY.r - 1) & (cellXY.c == kingXY.c - 1)))
            {
                return true;
            }
            else { return false; }
        }

        /// <summary>
        /// Проверка доски на шах королю
        /// </summary>
        bool CheckCheck(XY kingXY, Board board, Color kingColor, FiguresXY enemyFigures)
        {
            int FiguresCount = enemyFigures.Array.Length;
            Move[] Temp = null;

            for (int i = 0; i < FiguresCount; i++)
            {
                int rEnemy = enemyFigures.Array[i].r;
                int cEnemy = enemyFigures.Array[i].c;
                XY enemyXY = new XY(rEnemy, cEnemy);

                if (kingColor == Color.White)//проверяем шах белого короля
                {
                    switch (board._board[rEnemy, cEnemy])//клетка на которой стоит вражеская фигура
                    {
                        case (Cell.BlackPawn):
                            Temp = FindPawnAtacks(enemyXY, board, (Color)1);
                            break;
                        case (Cell.BlackKnight):
                            Temp = FindKnightMoves(enemyXY, board, (Color)1);
                            break;
                        case (Cell.BlackBishop):
                            Temp = FindBishopMoves(enemyXY, board, (Color)1);
                            break;
                        case (Cell.BlackRook):
                            Temp = FindRookMoves(enemyXY, board, (Color)1);
                            break;
                        case (Cell.BlackQueen):
                            Temp = FindQueenMoves(enemyXY, board, (Color)1);
                            break;
                        default:
                            break;
                    }
                }
                else//проверяем шах черного короля
                {
                    switch (board._board[rEnemy, cEnemy])//клетка на которой стоит вражеская фигура
                    {
                        case (Cell.WhitePawn):
                            Temp = FindPawnAtacks(enemyXY, board, (Color)0);
                            break;
                        case (Cell.WhiteKnight):
                            Temp = FindKnightMoves(enemyXY, board, (Color)0);
                            break;
                        case (Cell.WhiteBishop):
                            Temp = FindBishopMoves(enemyXY, board, (Color)0);
                            break;
                        case (Cell.WhiteRook):
                            Temp = FindRookMoves(enemyXY, board, (Color)0);
                            break;
                        case (Cell.WhiteQueen):
                            Temp = FindQueenMoves(enemyXY, board, (Color)0);
                            break;
                        default:
                            break;
                    }
                }

                if (Temp != null)//если найденный массив ходов фигуры не пустой
                {
                    int CountFound = Temp.Length;
                    //нужно проверить для каждого хода, совпадает ли завершение хода с координатами короля
                    for (int j = 0; j < CountFound; j++)
                    {
                        if ((kingXY.r == Temp[j].XY2.r) & (kingXY.c == Temp[j].XY2.c))
                        { return true; }
                    }
                }

            }

            return false;
        }

        /// <summary>
        /// Проверка хода на соответствие правилам
        /// </summary>
        bool CheckMove(XY kingXY, Board board, Color color, FiguresXY enemyFigures, Move move)
        {
            int r1 = move.XY1.r;//строка начала хода
            int c1 = move.XY1.c;//столбец начала хода
            int r2 = move.XY2.r;//строка конца хода
            int c2 = move.XY2.c;//столбец конца хода
            Move[] Temp = null;//все ходы фигуры, которая ходит

            //для начала нужно убедиться что мы пытаемся ходить своей фигурой
            if (color == Color.White)//если мы пытаемся ходить белыми фигурами
            {

                switch (board._board[r1, c1])//клетка с которой начинается ход
                {
                    case (Cell.WhitePawn):
                        Temp = FindPawnMoves(move.XY1, board, color);
                        break;
                    case (Cell.WhiteKnight):
                        Temp = FindKnightMoves(move.XY1, board, color);
                        break;
                    case (Cell.WhiteBishop):
                        Temp = FindBishopMoves(move.XY1, board, color);
                        break;
                    case (Cell.WhiteRook):
                        Temp = FindRookMoves(move.XY1, board, color);
                        break;
                    case (Cell.WhiteQueen):
                        Temp = FindQueenMoves(move.XY1, board, color);
                        break;
                    case (Cell.WhiteKing):
                        Temp = FindKingMoves(move.XY1, board, color);
                        break;
                    default:
                        return false;
                }
            }
            else//если мы пытаемся ходить черными фигурами
            {
                switch (board._board[r1, c1])//клетка с которой начинается ход
                {
                    case (Cell.BlackPawn):
                        Temp = FindPawnMoves(move.XY1, board, color);
                        break;
                    case (Cell.BlackKnight):
                        Temp = FindKnightMoves(move.XY1, board, color);
                        break;
                    case (Cell.BlackBishop):
                        Temp = FindBishopMoves(move.XY1, board, color);
                        break;
                    case (Cell.BlackRook):
                        Temp = FindRookMoves(move.XY1, board, color);
                        break;
                    case (Cell.BlackQueen):
                        Temp = FindQueenMoves(move.XY1, board, color);
                        break;
                    case (Cell.BlackKing):
                        Temp = FindKingMoves(move.XY1, board, color);
                        break;
                    default:
                        return false;
                }
            }

            //теперь нужно убедиться что у этой фигуры есть доступный ход, совпадающий с заявленным
            int CountFound = Temp.Length;
            bool flagMatch = false;//найден доступный ход, совпадающий с заявленным
            for (int i = 0; i < CountFound; i++)
            {
                if ((Temp[i].XY2.r == r2) & (Temp[i].XY2.c == c2))
                {
                    flagMatch = true;
                    break;
                }
            }
            if (flagMatch == false) { return false; }//заданного хода нет среди доступных

            //теперь нужно проверить что после выполнения хода король не окажется под шахом
            Board BoardMove = DoMove(board, move);
            bool CheckFlag = CheckCheck(kingXY, BoardMove, color, enemyFigures);

            if (!CheckFlag) { return true; }//если шаха нет, то ход соответствует правилам
            return false;
        }
        #endregion
    }

    class XY //содержит строку и ряд клетки на доске
    {
        public int r;//row
        public int c;//column

        public XY(int c, int r)
        {
            this.r = r;
            this.c = c;
        }

        public XY(XY a)
        {
            r = a.r;
            c = a.c;
        }
    }

    class Move //содержит координаты начала и конца хода
    {
        private XY[] _move;

        public XY XY1//Свойства чтобы можно было смотреть позиции начала и конца хода
        {
            get { return _move[0]; }
        }
        public XY XY2
        {
            get { return _move[1]; }
        }

        public Move(int x1, int y1, int x2, int y2)//конструктор из координат хода, может не нужен
        {
            XY xy1 = new XY(x1, y1);
            XY xy2 = new XY(x2, y2);
            _move = new XY[2] { xy1, xy2 };
        }

        public Move(XY xy1, int x2, int y2)//конструктор из координат хода
        {
            XY xy2 = new XY(x2, y2);
            _move = new XY[2] { xy1, xy2 };
        }

        public Move(XY xy1, XY xy2)//конструктор из координат XY
        {
            _move = new XY[2] { xy1, xy2 };
        }

        public Move(Move a)//конструктор копирования
        {
            _move = a._move;
        }
    }

    class FiguresXY //содержит координаты всех фигур игрока кроме короля
    {
        private XY[] _figures;//массив координат
        public XY[] Array//Свойство чтобы можно было использовать массив в циклах
        {
            get { return _figures; }
        }

        public FiguresXY(XY[] figures)//конструктор из массива
        {
            _figures = figures;
        }

        public FiguresXY(FiguresXY a)//конструктор копирования
        {
            _figures = a._figures;
        }
    }
}
