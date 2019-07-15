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
        /// Атакует ли слон заданную клетку?
        /// </summary>
        bool CheckBishopAtacks(XY bishopXY, Board board, XY cellXY)
        {
            //Ответ на вопрос, находится ли заданная клетка под атакой конкретного слона

            //для начала нужно убедиться что слон и атакованная клетка лежат на одной диагонали
            //для этого нужно убедиться что следующий параметр является целым числом
            float Ratio = Math.Abs(cellXY.r - bishopXY.r) / Math.Abs(cellXY.c - bishopXY.c);
            if (Ratio % 1 != 0)
            {
                return false;
            }

            //если слон рядом с клеткой то он чточно может ее атаковать
            if (Math.Abs(cellXY.r - bishopXY.r) == 1)
            { return true; }

            int CountMiddleCells = Math.Abs(cellXY.r - bishopXY.r) - 1;//количество пустых клеток между слоном и целью

            if ((cellXY.r < bishopXY.r) & (cellXY.c > bishopXY.c))//клетка сверху-справа
            {
                for (int i = 1; i < CountMiddleCells + 1; i++)
                {
                    if (board._board[bishopXY.r - i, bishopXY.c + i] != Cell.Empty)//Если между ними не пустая клетка
                    { return false; }
                }
                return true;
            }
            else if ((cellXY.r > bishopXY.r) & (cellXY.c > bishopXY.c))//клетка снизу-справа
            {
                for (int i = 1; i < CountMiddleCells + 1; i++)
                {
                    if (board._board[bishopXY.r + i, bishopXY.c + i] != Cell.Empty)//Если между ними не пустая клетка
                    { return false; }
                }
                return true;
            }
            else if ((cellXY.r > bishopXY.r) & (cellXY.c < bishopXY.c))//клетка снизу-слева
            {
                for (int i = 1; i < CountMiddleCells + 1; i++)
                {
                    if (board._board[bishopXY.r + i, bishopXY.c - i] != Cell.Empty)//Если между ними не пустая клетка
                    { return false; }
                }
                return true;
            }
            else if ((cellXY.r < bishopXY.r) & (cellXY.c < bishopXY.c))//клетка сверху-слева
            {
                for (int i = 1; i < CountMiddleCells + 1; i++)
                {
                    if (board._board[bishopXY.r - i, bishopXY.c - i] != Cell.Empty)//Если между ними не пустая клетка
                    { return false; }
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Атакует ли ладья заданную клетку?
        /// </summary>
        bool CheckRookAtacks(XY rookXY, Board board, XY cellXY)
        {
            //Ответ на вопрос, находится ли заданная клетка под атакой конкретной ладьи

            //для начала нужно убедиться что ладья и атакованная клетка лежат на одной горизонтали/вертикали
            if ( (cellXY.r != rookXY.r) & (cellXY.c != rookXY.c) )
            {
                return false;
            }

            //если ладья рядом с клеткой то она точно может ее атаковать
            int dr = Math.Abs(cellXY.r - rookXY.r);//абсолютная разница по строкам
            int dc = Math.Abs(cellXY.c - rookXY.c);//абсолютная разница по столбцам
            if (( dr + dc ) == 1)
            { return true; }

            int CountMiddleCells = dr + dc - 1;//количество пустых клеток между ладьей и целью

            if ((cellXY.r == rookXY.r) & (cellXY.c > rookXY.c))//клетка сверху-справа
            {
                for (int i = 1; i < CountMiddleCells + 1; i++)
                {
                    if (board._board[rookXY.r, rookXY.c + i] != Cell.Empty)//Если между ними не пустая клетка
                    { return false; }
                }
                return true;
            }
            else if ((cellXY.r > rookXY.r) & (cellXY.c == rookXY.c))//клетка снизу-справа
            {
                for (int i = 1; i < CountMiddleCells + 1; i++)
                {
                    if (board._board[rookXY.r + i, rookXY.c] != Cell.Empty)//Если между ними не пустая клетка
                    { return false; }
                }
                return true;
            }
            else if ((cellXY.r == rookXY.r) & (cellXY.c < rookXY.c))//клетка снизу-слева
            {
                for (int i = 1; i < CountMiddleCells + 1; i++)
                {
                    if (board._board[rookXY.r, rookXY.c - i] != Cell.Empty)//Если между ними не пустая клетка
                    { return false; }
                }
                return true;
            }
            else if ((cellXY.r < rookXY.r) & (cellXY.c == rookXY.c))//клетка сверху-слева
            {
                for (int i = 1; i < CountMiddleCells + 1; i++)
                {
                    if (board._board[rookXY.r - i, rookXY.c] != Cell.Empty)//Если между ними не пустая клетка
                    { return false; }
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Атакует ли ферзь заданную клетку?
        /// </summary>
        bool CheckQueenAtacks(XY queenXY, Board board, XY cellXY)
        {
            //Ферзь ходит как ладья и как слон и можно просто вызвать 
            //процедуры проверки атаки слона и ладьи, если хоть одна из них атакует, то ферзь тоже

            bool BishopAtacks = CheckBishopAtacks(queenXY, board, cellXY);
            bool RookAtacks = CheckRookAtacks(queenXY, board, cellXY);

            return BishopAtacks | RookAtacks;
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
            bool TempFlag = false;//флаг для проверки что эта фигура атакует короля.
            //если флаг равен 1, значит королю шах

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
                            TempFlag = CheckPawnAtacks(enemyXY, (Color)1, kingXY);
                            if (TempFlag) { return TempFlag; }
                            break;
                        case (Cell.BlackKnight):
                            TempFlag = CheckKnightAtacks(enemyXY, kingXY);
                            if (TempFlag) { return TempFlag; }
                            break;
                        case (Cell.BlackBishop):
                            TempFlag = CheckBishopAtacks(enemyXY, board, kingXY);
                            if (TempFlag) { return TempFlag; }
                            break;
                        case (Cell.BlackRook):
                            TempFlag = CheckRookAtacks(enemyXY, board, kingXY);
                            if (TempFlag) { return TempFlag; }
                            break;
                        case (Cell.BlackQueen):
                            TempFlag = CheckQueenAtacks(enemyXY, board, kingXY);
                            if (TempFlag) { return TempFlag; }
                            break;
                        case (Cell.BlackKing):
                            TempFlag = CheckKingAtacks(enemyXY, kingXY);
                            if (TempFlag) { return TempFlag; }
                            break;
                        default:
                            break;
                    }
                }
                else//проверяем шах черного короля
                {
                    switch (board._board[rEnemy, cEnemy])//клетка на которой стоит вражеская фигура
                    {
                        case (Cell.BlackPawn):
                            TempFlag = CheckPawnAtacks(enemyXY, (Color)0, kingXY);
                            if (TempFlag) { return TempFlag; }
                            break;
                        case (Cell.BlackKnight):
                            TempFlag = CheckKnightAtacks(enemyXY, kingXY);
                            if (TempFlag) { return TempFlag; }
                            break;
                        case (Cell.BlackBishop):
                            TempFlag = CheckBishopAtacks(enemyXY, board, kingXY);
                            if (TempFlag) { return TempFlag; }
                            break;
                        case (Cell.BlackRook):
                            TempFlag = CheckRookAtacks(enemyXY, board, kingXY);
                            if (TempFlag) { return TempFlag; }
                            break;
                        case (Cell.BlackQueen):
                            TempFlag = CheckQueenAtacks(enemyXY, board, kingXY);
                            if (TempFlag) { return TempFlag; }
                            break;
                        case (Cell.BlackKing):
                            TempFlag = CheckKingAtacks(enemyXY, kingXY);
                            if (TempFlag) { return TempFlag; }
                            break;
                        default:
                            break;
                    }
                }

            }

            return false;
        }

        /// <summary>
        /// Проверка хода на соответствие правилам
        /// </summary>
        bool CheckMove(XY kingXY, Board board, Color color, FiguresXY enemyFigures, Move move, Move enemyLastMove)
        {
            int r1 = move.XY1.r;//строка начала хода
            int c1 = move.XY1.c;//столбец начала хода
            int r2 = move.XY2.r;//строка конца хода
            int c2 = move.XY2.c;//столбец конца хода
            Move[] PawnFoundMoves = null;//все ходы фигуры, которая ходит

            bool TempFlag = false;//флаг для проверки что эта фигура может атаковать заданную клетку.
            //если флаг равен 1, значит королю шах

            //для начала нужно убедиться что мы пытаемся ходить своей фигурой
            if (color == Color.White)//если мы пытаемся ходить белыми фигурами
            {

                switch (board._board[r1, c1])//клетка с которой начинается ход
                {
                    case (Cell.WhitePawn):
                        TempFlag = CheckPawnAtacks(move.XY1, color, move.XY2);
                        if (TempFlag) { return TempFlag; }
                        PawnFoundMoves = FindPawnMoves(move.XY1, board, color, enemyLastMove);
                        break;
                    case (Cell.WhiteKnight):
                        TempFlag = CheckKnightAtacks(move.XY1, move.XY2);
                        if (TempFlag) { return TempFlag; }
                        break;
                    case (Cell.WhiteBishop):
                        TempFlag = CheckBishopAtacks(move.XY1, board, move.XY2);
                        if (TempFlag) { return TempFlag; }
                        break;
                    case (Cell.WhiteRook):
                        TempFlag = CheckRookAtacks(move.XY1, board, move.XY2);
                        if (TempFlag) { return TempFlag; }
                        break;
                    case (Cell.WhiteQueen):
                        TempFlag = CheckQueenAtacks(move.XY1, board, move.XY2);
                        if (TempFlag) { return TempFlag; }
                        break;
                    case (Cell.WhiteKing):
                        TempFlag = CheckKingAtacks(move.XY1, move.XY2);
                        if (TempFlag) { return TempFlag; }
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
                        TempFlag = CheckPawnAtacks(move.XY1, color, move.XY2);
                        if (TempFlag) { return TempFlag; }
                        PawnFoundMoves = FindPawnMoves(move.XY1, board, color, enemyLastMove);
                        break;
                    case (Cell.BlackKnight):
                        TempFlag = CheckKnightAtacks(move.XY1, move.XY2);
                        if (TempFlag) { return TempFlag; }
                        break;
                    case (Cell.BlackBishop):
                        TempFlag = CheckBishopAtacks(move.XY1, board, move.XY2);
                        if (TempFlag) { return TempFlag; }
                        break;
                    case (Cell.BlackRook):
                        TempFlag = CheckRookAtacks(move.XY1, board, move.XY2);
                        if (TempFlag) { return TempFlag; }
                        break;
                    case (Cell.BlackQueen):
                        TempFlag = CheckQueenAtacks(move.XY1, board, move.XY2);
                        if (TempFlag) { return TempFlag; }
                        break;
                    case (Cell.BlackKing):
                        TempFlag = CheckKingAtacks(move.XY1, move.XY2);
                        if (TempFlag) { return TempFlag; }
                        break;
                    default:
                        return false;
                }
            }

            if (PawnFoundMoves != null)//если мы пытаемся ходить пешкой, то нужно еще проверить 
                //взятие на проходе, для этого просто берутся все возможные ходы этой пешки и сравниваются с предложенным ходом
            {
                //теперь нужно убедиться что у пешки есть доступный ход, совпадающий с заявленным
                int CountFound = PawnFoundMoves.Length;
                bool flagMatch = false;//найден доступный ход, совпадающий с заявленным
                for (int i = 0; i < CountFound; i++)
                {
                    if ((PawnFoundMoves[i].XY2.r == r2) & (PawnFoundMoves[i].XY2.c == c2))
                    {
                        flagMatch = true;
                        break;
                    }
                }
                if (flagMatch == false) { return false; }//заданного хода нет среди доступных
            }
            

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
