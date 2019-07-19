using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//TODO: Сделать все условия при поиске всех ходов через else if
namespace ChessFantasy
{
    public enum Cell
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
    public enum Color { White = 0, Black = 1}
    public enum MoveType
    {
        Moving = 0,//Ход-перемещение фигуры
        Taking = 1,//Ход-взятие фигуры
        WhiteLeftEmpassant = 2,//белая пешка берет на проходе черную пешку слева
        WhiteRightEmpassant = 3,//белая пешка берет на проходе черную пешку справа
        WhiteLeftRogue = 4,//Белый король делает левую рокировку
        WhiteRightRogue = 5,//Белый король делает правую рокировку
        BlackLeftEmpassant = 6,//белая пешка берет на проходе черную пешку слева
        BlackRightEmpassant = 7,//белая пешка берет на проходе черную пешку справа
        BlackLeftRogue = 8,//Белый король делает левую рокировку
        BlackRightRogue = 9//Белый король делает правую рокировку
    }

    public class Board //содержит доску с фигурами и все дествия с ней, а также всю информацию для игры
    {
        //Флаги о том что белый и черный игроки двигали королей и ладьи (для рокировки)
        public bool _WLRogueAvailable = true;//У белых есть право на длинную рокировку (левая ладья и король не делали ход)
        public bool _WRRogueAvailable = true;//У белых есть право на короткую рокировку (правая ладья и король не делали ход)
        public bool _BLRogueAvailable = true;//У черных есть право на длинную рокировку (левая ладья и король не делали ход)
        public bool _BRRogueAvailable = true;//У черных есть право на короткую рокировку (правая ладья и король не делали ход)

        private Cell[,] _board; //Сама доска, массив 8х8 в котором записано состояние каждой клетки
        public Cell[,] BoardArr//Свойства чтобы можно было снаружи смотреть массив
        {
            get { return _board; }
        }

        private Color _nextColor; //Цвет игрока, который ходит следующим
        public Color NextColor//Свойства чтобы можно было снаружи смотреть массив
        {
            get { return _nextColor; }
        }

        public Board()//создание начальной доски
            {
                _board = new Cell[8, 8] {{ (Cell)10, (Cell)8, (Cell)9, (Cell)11, (Cell)12, (Cell)9, (Cell)8, (Cell)10 },
                                         { (Cell)7,  (Cell)7, (Cell)7, (Cell)7,  (Cell)7,  (Cell)7, (Cell)7, (Cell)7 },
                                         { 0,        0,       0,       0,        0,        0,       0,       0 },
                                         { 0,        0,       0,       0,        0,        0,       0,       0 },
                                         { 0,        0,       0,       0,        0,        0,       0,       0 },
                                         { 0,        0,       0,       0,        0,        0,       0,       0 },
                                         { (Cell)1,  (Cell)1, (Cell)1, (Cell)1,  (Cell)1,  (Cell)1, (Cell)1, (Cell)1 },
                                         { (Cell)4,  (Cell)2, (Cell)3, (Cell)5,  (Cell)6,  (Cell)3, (Cell)2, (Cell)4 } };
            }

        public Board(Board a)//конструктор копирования
        {
            _board = a._board;
            _WLRogueAvailable = a._WLRogueAvailable;
            _WRRogueAvailable = a._WRRogueAvailable;
            _BLRogueAvailable = a._BLRogueAvailable;
            _BRRogueAvailable = a._BRRogueAvailable;
    }

        /// <summary>
        /// Возвращает доску с примененным ходом без проверки на правила
        /// TODO: ход с превращением пешки в ферзя
        /// </summary>
        public static Board DoMove(Board board, Move move, Color color)
        {
            //если координаты хода находятся на доске
            if ((move.XY1.r > 0) && (move.XY1.r < 7) && (move.XY1.c > 0) && (move.XY1.c < 7) &&
                (move.XY2.r > 0) && (move.XY2.r < 7) && (move.XY2.c > 0) && (move.XY2.c < 7))
            {
                Board boardOut = board;//доска с выполненным ходом
                Cell Figure = boardOut._board[move.XY1.r, move.XY1.c];//фигура которой мы ходим

                boardOut._board[move.XY1.r, move.XY1.c] = Cell.Empty;//клетка с которой начинался ход обнуляется
                boardOut._board[move.XY2.r, move.XY2.c] = Figure;//клетка, где заканчивается ход, занимается фигурой

                if (color == Color.White)//Если это какой-то экзотический ход типа взятия на проходе или рокировки, то нужно знать цвет фигуры
                {
                    if (Figure == Cell.WhiteKing)
                    {
                        board._WLRogueAvailable = false;
                        board._WRRogueAvailable = false;
                    }
                    else if ((Figure == Cell.WhiteRook) && (move.XY1.r == 7) && (move.XY1.c == 0))
                        { board._WLRogueAvailable = false; }
                    else if((Figure == Cell.WhiteRook) && (move.XY1.r == 7) && (move.XY1.c == 7))
                        { board._WRRogueAvailable = false; }

                    if (move._moveType == MoveType.WhiteLeftEmpassant)//взятие на проходе белой пешкой влево
                    {
                        boardOut._board[move.XY1.r, move.XY1.c - 1] = Cell.Empty;//клетка на которой стояла вражеская пешка обнуляется
                    }
                    else if (move._moveType == MoveType.WhiteRightEmpassant)//взятие на проходе белой пешкой вправо
                    {
                        boardOut._board[move.XY1.r, move.XY1.c + 1] = Cell.Empty;//клетка на которой стояла вражеская пешка обнуляется
                    }
                    else if (move._moveType == MoveType.WhiteLeftRogue)//рокировка белого короля влево
                    {
                        boardOut._board[7, 0] = Cell.Empty;//клетка на которой стояла ладья обнуляется
                        boardOut._board[7, 3] = Cell.WhiteRook;//ладья переставляется
                    }
                    else if (move._moveType == MoveType.WhiteRightRogue)//рокировка белого короля вправо
                    {
                        boardOut._board[7, 0] = Cell.Empty;//клетка на которой стояла ладья обнуляется
                        boardOut._board[7, 3] = Cell.WhiteRook;//ладья переставляется
                    }
                }
                else //Ходим черными фигурами рокировку и взятие на проходе
                {
                    if (Figure == Cell.BlackKing)
                    {
                        board._BLRogueAvailable = false;
                        board._BRRogueAvailable = false;
                    }
                    else if ((Figure == Cell.BlackRook) && (move.XY1.r == 0) && (move.XY1.c == 0))
                    { board._BLRogueAvailable = false; }
                    else if ((Figure == Cell.BlackRook) && (move.XY1.r == 0) && (move.XY1.c == 7))
                    { board._BRRogueAvailable = false; }

                    if (move._moveType == MoveType.BlackLeftEmpassant)//взятие на проходе белой пешкой влево
                    {
                        boardOut._board[move.XY1.r, move.XY1.c - 1] = Cell.Empty;//клетка на которой стояла вражеская пешка обнуляется
                    }
                    else if (move._moveType == MoveType.BlackRightEmpassant)//взятие на проходе белой пешкой вправо
                    {
                        boardOut._board[move.XY1.r, move.XY1.c + 1] = Cell.Empty;//клетка на которой стояла вражеская пешка обнуляется
                    }
                    else if (move._moveType == MoveType.BlackLeftRogue)//рокировка черного короля влево
                    {
                        boardOut._board[0, 0] = Cell.Empty;//клетка на которой стояла ладья обнуляется
                        boardOut._board[0, 3] = Cell.BlackRook;//ладья переставляется
                    }
                    else if (move._moveType == MoveType.BlackRightRogue)//рокировка черного короля вправо
                    {
                        boardOut._board[0, 0] = Cell.Empty;//клетка на которой стояла ладья обнуляется
                        boardOut._board[0, 3] = Cell.BlackRook;//ладья переставляется
                    }
                }

                if (boardOut._nextColor == Color.White)//нужно передать ход другому игроку
                {
                    boardOut._nextColor = Color.Black;
                }
                else
                {
                    boardOut._nextColor = Color.White;
                }
                
                return boardOut;
            }
            else { return null; }
        }


        #region FindFiguresMoves
        /// <summary>
        /// Поиск всех ходов, которые может совершить пешка
        /// </summary>
        static Move[] FindPawnMoves(XY pawnXY, Board board, Color color, Move enemyLastMove, XY kingXY, FiguresXY enemyFigures)
        {
            //пешка может совершить 6 возможных ходов: перемещение на одну или 2 клетки вперед, 
            //2 атаки по диагонали и 2 взятия на проходе с 2-х сторон
            Move[] Moves = new Move[0];//массив для хранени всех ходов пешки
            Move[] Temp1;
            Move Move;
            bool CheckKing = false;//Шах королю?
            int Count = 0; //количество найденных ходов пешки
            int rMove = -1;//Координаты искомого хода, строка
            int cMove = -1;//Координаты искомого хода, солбец

            if (color == Color.White)//ищем ходы белой пешки
            {
                //ход белой пешкой на одну клетку вперед
                rMove = pawnXY.r - 1;
                cMove = pawnXY.c;
                if (board._board[rMove, cMove] == Cell.Empty)
                {
                    Move = new Move(pawnXY, rMove, cMove);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //ход белой пешкой на две клетки вперед
                if ((pawnXY.r == 6) && (board._board[pawnXY.r - 1, pawnXY.c] == Cell.Empty) &&
                    (board._board[pawnXY.r - 2, pawnXY.c] == Cell.Empty))
                {
                    Move = new Move(pawnXY, pawnXY.r - 2, pawnXY.c);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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
                }

                //атака белой пешкой налево
                rMove = pawnXY.r - 1;
                cMove = pawnXY.c - 1;
                if ((pawnXY.c > 0) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) |
                    (board._board[rMove, cMove] == Cell.BlackKnight) |
                    (board._board[rMove, cMove] == Cell.BlackBishop) |
                    (board._board[rMove, cMove] == Cell.BlackRook) |
                    (board._board[rMove, cMove] == Cell.BlackQueen) |
                    (board._board[rMove, cMove] == Cell.BlackKing)))
                {
                    Move = new Move(pawnXY, rMove, cMove, MoveType.Taking);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //атака белой пешкой направо
                rMove = pawnXY.r - 1;
                cMove = pawnXY.c + 1;
                if ((pawnXY.c < 7) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) |
                    (board._board[rMove, cMove] == Cell.BlackKnight) |
                    (board._board[rMove, cMove] == Cell.BlackBishop) |
                    (board._board[rMove, cMove] == Cell.BlackRook) |
                    (board._board[rMove, cMove] == Cell.BlackQueen) |
                    (board._board[rMove, cMove] == Cell.BlackKing)))
                {
                    Move = new Move(pawnXY, rMove, cMove, MoveType.Taking);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //взятие на проходе белой пешкой налево
                if ((pawnXY.r == 3) && //нужно находиться на нужной горизонтали(5-й)
                    (board._board[pawnXY.r, pawnXY.c - 1] == Cell.BlackPawn) &&// чтобы слева была вражеская пешка
                    (enemyLastMove.XY1.r == 1) && (enemyLastMove.XY1.c == pawnXY.c - 1) && //прошлый вражеский ход начинался с 7 строки и нужного столбца
                    (enemyLastMove.XY2.r == 3) && (enemyLastMove.XY1.c == pawnXY.c - 1))//прошлый вражеский ход закончился на 5 строке и нужном столбце
                {
                    Move = new Move(pawnXY, pawnXY.r - 1, pawnXY.c - 1, MoveType.WhiteLeftEmpassant);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //взятие на проходе белой пешкой направо
                if ((pawnXY.r == 3) && //нужно находиться на нужной горизонтали(5-й)
                    (board._board[pawnXY.r, pawnXY.c + 1] == Cell.BlackPawn) &&// чтобы слева была вражеская пешка
                    (enemyLastMove.XY1.r == 1) && (enemyLastMove.XY1.c == pawnXY.c + 1) && //прошлый вражеский ход начинался с 7 строки и нужного столбца
                    (enemyLastMove.XY2.r == 3) && (enemyLastMove.XY1.c == pawnXY.c + 1))//прошлый вражеский ход закончился на 5 строке и нужном столбце
                {
                    Move = new Move(pawnXY, pawnXY.r - 1, pawnXY.c + 1, MoveType.WhiteRightEmpassant);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

            }
            else//Если мы ищем атаки черной пешки
            {
                //ход черной пешкой на одну клетку вперед
                rMove = pawnXY.r + 1;
                cMove = pawnXY.c;
                if (board._board[rMove, cMove] == Cell.Empty)
                {
                    Move = new Move(pawnXY, rMove, cMove);
                    Moves = new Move[] { Move };
                    Count = 1;
                }

                //ход черной пешкой на две клетки вперед
                if ((pawnXY.r == 6) && (board._board[pawnXY.r + 1, pawnXY.c] == Cell.Empty) &&
                    (board._board[pawnXY.r + 2, pawnXY.c] == Cell.Empty))
                {
                    Move = new Move(pawnXY, pawnXY.r + 2, pawnXY.c);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //атака черной пешкой налево
                rMove = pawnXY.r + 1;
                cMove = pawnXY.c - 1;
                if ((pawnXY.c > 0) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) |
                    (board._board[rMove, cMove] == Cell.BlackKnight) |
                    (board._board[rMove, cMove] == Cell.BlackBishop) |
                    (board._board[rMove, cMove] == Cell.BlackRook) |
                    (board._board[rMove, cMove] == Cell.BlackQueen) |
                    (board._board[rMove, cMove] == Cell.BlackKing)))
                {
                    Move = new Move(pawnXY, rMove, cMove, MoveType.Taking);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //атака черной пешкой направо
                rMove = pawnXY.r + 1;
                cMove = pawnXY.c + 1;
                if ((pawnXY.c < 7) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) |
                    (board._board[rMove, cMove] == Cell.BlackKnight) |
                    (board._board[rMove, cMove] == Cell.BlackBishop) |
                    (board._board[rMove, cMove] == Cell.BlackRook) |
                    (board._board[rMove, cMove] == Cell.BlackQueen) |
                    (board._board[rMove, cMove] == Cell.BlackKing)))
                {
                    Move = new Move(pawnXY, rMove, cMove, MoveType.Taking);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //взятие на проходе черной пешкой налево
                if ((pawnXY.r == 4) && //нужно находиться на нужной горизонтали(4-й)
                    (board._board[pawnXY.r, pawnXY.c - 1] == Cell.BlackPawn) &&// чтобы слева была вражеская пешка
                    (enemyLastMove.XY1.r == 6) && (enemyLastMove.XY1.c == pawnXY.c - 1) && //прошлый вражеский ход начинался с 2 строки и нужного столбца
                    (enemyLastMove.XY2.r == 4) && (enemyLastMove.XY1.c == pawnXY.c - 1))//прошлый вражеский ход закончился на 4 строке и нужном столбце
                {
                    Move = new Move(pawnXY, pawnXY.r + 1, pawnXY.c - 1, MoveType.BlackLeftEmpassant);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //взятие на проходе черной пешкой направо
                if ((pawnXY.r == 4) && //нужно находиться на нужной горизонтали(5-й)
                    (board._board[pawnXY.r, pawnXY.c + 1] == Cell.BlackPawn) &&// чтобы слева была вражеская пешка
                    (enemyLastMove.XY1.r == 6) && (enemyLastMove.XY1.c == pawnXY.c + 1) && //прошлый вражеский ход начинался с 7 строки и нужного столбца
                    (enemyLastMove.XY2.r == 4) && (enemyLastMove.XY1.c == pawnXY.c + 1))//прошлый вражеский ход закончился на 5 строке и нужном столбце
                {
                    Move = new Move(pawnXY, pawnXY.r + 1, pawnXY.c + 1, MoveType.BlackRightEmpassant);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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
            }

            return Moves;
        }

        /// <summary>
        /// Поиск всех ходов, которые может совершить конь
        /// </summary>
        static Move[] FindKnightMoves(XY knightXY, Board board, Color color, XY kingXY, FiguresXY enemyFigures)
        {
            Move[] Moves = new Move[0];//массив для хранени всех ходов коня
            Move[] Temp;
            Move Move;
            bool CheckKing = false;//Шах королю?
            int Count = 0; //количество найденных ходов коня

            int rMove = -1;//Координаты искомого хода, строка
            int cMove = -1;//Координаты искомого хода, солбец

            if (color == Color.White)//ищем ходы белого коня
            {
                //атака белым конем на 1 час
                rMove = knightXY.r - 2;
                cMove = knightXY.c + 1;
                if ((rMove > 0) && (cMove < 7) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) |
                    (board._board[rMove, cMove] == Cell.BlackKnight) |
                    (board._board[rMove, cMove] == Cell.BlackBishop) |
                    (board._board[rMove, cMove] == Cell.BlackRook) |
                    (board._board[rMove, cMove] == Cell.BlackQueen) |
                    (board._board[rMove, cMove] == Cell.BlackKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //атака белым конем на 2 часа
                rMove = knightXY.r - 1;
                cMove = knightXY.c + 2;
                if ((rMove > 0) && (cMove < 7) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) |
                    (board._board[rMove, cMove] == Cell.BlackKnight) |
                    (board._board[rMove, cMove] == Cell.BlackBishop) |
                    (board._board[rMove, cMove] == Cell.BlackRook) |
                    (board._board[rMove, cMove] == Cell.BlackQueen) |
                    (board._board[rMove, cMove] == Cell.BlackKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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
                }

                //атака белым конем на 4 часа
                rMove = knightXY.r + 1;
                cMove = knightXY.c + 2;
                if ((rMove < 7) && (cMove < 7) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) |
                    (board._board[rMove, cMove] == Cell.BlackKnight) |
                    (board._board[rMove, cMove] == Cell.BlackBishop) |
                    (board._board[rMove, cMove] == Cell.BlackRook) |
                    (board._board[rMove, cMove] == Cell.BlackQueen) |
                    (board._board[rMove, cMove] == Cell.BlackKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //атака белым конем на 5 часов
                rMove = knightXY.r + 2;
                cMove = knightXY.c + 1;
                if ((rMove < 7) && (cMove < 7) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) |
                    (board._board[rMove, cMove] == Cell.BlackKnight) |
                    (board._board[rMove, cMove] == Cell.BlackBishop) |
                    (board._board[rMove, cMove] == Cell.BlackRook) |
                    (board._board[rMove, cMove] == Cell.BlackQueen) |
                    (board._board[rMove, cMove] == Cell.BlackKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //атака белым конем на 7 часов
                rMove = knightXY.r + 2;
                cMove = knightXY.c - 1;
                if ((rMove < 7) && (cMove > 0) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) |
                    (board._board[rMove, cMove] == Cell.BlackKnight) |
                    (board._board[rMove, cMove] == Cell.BlackBishop) |
                    (board._board[rMove, cMove] == Cell.BlackRook) |
                    (board._board[rMove, cMove] == Cell.BlackQueen) |
                    (board._board[rMove, cMove] == Cell.BlackKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //атака белым конем на 8 часов
                rMove = knightXY.r + 1;
                cMove = knightXY.c - 2;
                if ((rMove < 7) && (cMove > 0) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) |
                    (board._board[rMove, cMove] == Cell.BlackKnight) |
                    (board._board[rMove, cMove] == Cell.BlackBishop) |
                    (board._board[rMove, cMove] == Cell.BlackRook) |
                    (board._board[rMove, cMove] == Cell.BlackQueen) |
                    (board._board[rMove, cMove] == Cell.BlackKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //атака белым конем на 10 часов
                rMove = knightXY.r - 1;
                cMove = knightXY.c - 2;
                if ((rMove > 0) && (cMove > 0) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) |
                    (board._board[rMove, cMove] == Cell.BlackKnight) |
                    (board._board[rMove, cMove] == Cell.BlackBishop) |
                    (board._board[rMove, cMove] == Cell.BlackRook) |
                    (board._board[rMove, cMove] == Cell.BlackQueen) |
                    (board._board[rMove, cMove] == Cell.BlackKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //атака белым конем на 11 часов
                rMove = knightXY.r - 2;
                cMove = knightXY.c - 1;
                if ((rMove > 0) && (cMove > 0) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) |
                    (board._board[rMove, cMove] == Cell.BlackKnight) |
                    (board._board[rMove, cMove] == Cell.BlackBishop) |
                    (board._board[rMove, cMove] == Cell.BlackRook) |
                    (board._board[rMove, cMove] == Cell.BlackQueen) |
                    (board._board[rMove, cMove] == Cell.BlackKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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
            }
            else//Если мы ищем атаки черного коня
            {
                //атака черным конем на 1 час
                rMove = knightXY.r - 2;
                cMove = knightXY.c + 1;
                if ((rMove > 0) && (cMove < 7) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) |
                    (board._board[rMove, cMove] == Cell.WhiteKnight) |
                    (board._board[rMove, cMove] == Cell.WhiteBishop) |
                    (board._board[rMove, cMove] == Cell.WhiteRook) |
                    (board._board[rMove, cMove] == Cell.WhiteQueen) |
                    (board._board[rMove, cMove] == Cell.WhiteKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //атака черным конем на 2 часа
                rMove = knightXY.r - 1;
                cMove = knightXY.c + 2;
                if ((rMove > 0) && (cMove < 7) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) |
                    (board._board[rMove, cMove] == Cell.WhiteKnight) |
                    (board._board[rMove, cMove] == Cell.WhiteBishop) |
                    (board._board[rMove, cMove] == Cell.WhiteRook) |
                    (board._board[rMove, cMove] == Cell.WhiteQueen) |
                    (board._board[rMove, cMove] == Cell.WhiteKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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
                }

                //атака черным конем на 4 часа
                rMove = knightXY.r + 1;
                cMove = knightXY.c + 2;
                if ((rMove < 7) && (cMove < 7) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) |
                    (board._board[rMove, cMove] == Cell.WhiteKnight) |
                    (board._board[rMove, cMove] == Cell.WhiteBishop) |
                    (board._board[rMove, cMove] == Cell.WhiteRook) |
                    (board._board[rMove, cMove] == Cell.WhiteQueen) |
                    (board._board[rMove, cMove] == Cell.WhiteKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //атака черным конем на 5 часов
                rMove = knightXY.r + 2;
                cMove = knightXY.c + 1;
                if ((rMove < 7) && (cMove < 7) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) |
                    (board._board[rMove, cMove] == Cell.WhiteKnight) |
                    (board._board[rMove, cMove] == Cell.WhiteBishop) |
                    (board._board[rMove, cMove] == Cell.WhiteRook) |
                    (board._board[rMove, cMove] == Cell.WhiteQueen) |
                    (board._board[rMove, cMove] == Cell.WhiteKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //атака черным конем на 7 часов
                rMove = knightXY.r + 2;
                cMove = knightXY.c - 1;
                if ((rMove < 7) && (cMove > 0) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) |
                    (board._board[rMove, cMove] == Cell.WhiteKnight) |
                    (board._board[rMove, cMove] == Cell.WhiteBishop) |
                    (board._board[rMove, cMove] == Cell.WhiteRook) |
                    (board._board[rMove, cMove] == Cell.WhiteQueen) |
                    (board._board[rMove, cMove] == Cell.WhiteKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //атака черным конем на 8 часов
                rMove = knightXY.r + 1;
                cMove = knightXY.c - 2;
                if ((rMove < 7) && (cMove > 0) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) |
                    (board._board[rMove, cMove] == Cell.WhiteKnight) |
                    (board._board[rMove, cMove] == Cell.WhiteBishop) |
                    (board._board[rMove, cMove] == Cell.WhiteRook) |
                    (board._board[rMove, cMove] == Cell.WhiteQueen) |
                    (board._board[rMove, cMove] == Cell.WhiteKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //атака черным конем на 10 часов
                rMove = knightXY.r - 1;
                cMove = knightXY.c - 2;
                if ((rMove > 0) && (cMove > 0) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) |
                    (board._board[rMove, cMove] == Cell.WhiteKnight) |
                    (board._board[rMove, cMove] == Cell.WhiteBishop) |
                    (board._board[rMove, cMove] == Cell.WhiteRook) |
                    (board._board[rMove, cMove] == Cell.WhiteQueen) |
                    (board._board[rMove, cMove] == Cell.WhiteKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //атака черным конем на 11 часов
                rMove = knightXY.r - 2;
                cMove = knightXY.c - 1;
                if ((rMove > 0) && (cMove > 0) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) |
                    (board._board[rMove, cMove] == Cell.WhiteKnight) |
                    (board._board[rMove, cMove] == Cell.WhiteBishop) |
                    (board._board[rMove, cMove] == Cell.WhiteRook) |
                    (board._board[rMove, cMove] == Cell.WhiteQueen) |
                    (board._board[rMove, cMove] == Cell.WhiteKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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
            }

            return Moves;
        }

        /// <summary>
        /// Поиск всех ходов, которые может совершить слон
        /// </summary>
        static Move[] FindBishopMoves(XY bishopXY, Board board, Color color, XY kingXY, FiguresXY enemyFigures)
        {
            Move[] Moves = new Move[0];//массив для хранени всех ходов слона
            Move[] Temp1;
            Move Move;
            bool CheckKing = false;//Шах королю?
            int Count = 0; //количество найденных ходов слона
            int j;

            int rMove = -1;//Координаты искомого хода, строка
            int cMove = -1;//Координаты искомого хода, солбец

            if (color == Color.White)//ищем ходы белого слона
            {
                //атака белым слоном на верх-право
                j = 1;
                rMove = bishopXY.r - j;
                cMove = bishopXY.c + j;
                
                while ((rMove > 0) && (cMove < 7) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) |
                    (board._board[rMove, cMove] == Cell.BlackKnight) |
                    (board._board[rMove, cMove] == Cell.BlackBishop) |
                    (board._board[rMove, cMove] == Cell.BlackRook) |
                    (board._board[rMove, cMove] == Cell.BlackQueen) |
                    (board._board[rMove, cMove] == Cell.BlackKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(bishopXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //атака белым слоном вниз-вправо
                j = 1;
                rMove = bishopXY.r + j;
                cMove = bishopXY.c + j;
                
                while ((rMove > 0) && (cMove < 7) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) |
                    (board._board[rMove, cMove] == Cell.BlackKnight) |
                    (board._board[rMove, cMove] == Cell.BlackBishop) |
                    (board._board[rMove, cMove] == Cell.BlackRook) |
                    (board._board[rMove, cMove] == Cell.BlackQueen) |
                    (board._board[rMove, cMove] == Cell.BlackKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(bishopXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //атака белым слоном вниз-влево
                j = 1;
                rMove = bishopXY.r + j;
                cMove = bishopXY.c - j;
                while ((rMove > 0) && (cMove < 7) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) |
                    (board._board[rMove, cMove] == Cell.BlackKnight) |
                    (board._board[rMove, cMove] == Cell.BlackBishop) |
                    (board._board[rMove, cMove] == Cell.BlackRook) |
                    (board._board[rMove, cMove] == Cell.BlackQueen) |
                    (board._board[rMove, cMove] == Cell.BlackKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(bishopXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //атака белым слоном вверх-влево
                j = 1;
                rMove = bishopXY.r - j;
                cMove = bishopXY.c - j;
                while ((rMove > 0) && (cMove < 7) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) |
                    (board._board[rMove, cMove] == Cell.BlackKnight) |
                    (board._board[rMove, cMove] == Cell.BlackBishop) |
                    (board._board[rMove, cMove] == Cell.BlackRook) |
                    (board._board[rMove, cMove] == Cell.BlackQueen) |
                    (board._board[rMove, cMove] == Cell.BlackKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(bishopXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

            }
            else//Если мы ищем атаки черного слона
            {
                //атака черным слоном на верх-право
                j = 1;
                rMove = bishopXY.r - j;
                cMove = bishopXY.c + j;
                while ((rMove > 0) && (cMove < 7) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) |
                    (board._board[rMove, cMove] == Cell.WhiteKnight) |
                    (board._board[rMove, cMove] == Cell.WhiteBishop) |
                    (board._board[rMove, cMove] == Cell.WhiteRook) |
                    (board._board[rMove, cMove] == Cell.WhiteQueen) |
                    (board._board[rMove, cMove] == Cell.WhiteKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(bishopXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //атака черным слоном вниз-вправо
                j = 1;
                rMove = bishopXY.r + j;
                cMove = bishopXY.c + j;
                while ((rMove > 0) && (cMove < 7) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) |
                    (board._board[rMove, cMove] == Cell.WhiteKnight) |
                    (board._board[rMove, cMove] == Cell.WhiteBishop) |
                    (board._board[rMove, cMove] == Cell.WhiteRook) |
                    (board._board[rMove, cMove] == Cell.WhiteQueen) |
                    (board._board[rMove, cMove] == Cell.WhiteKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(bishopXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //атака черным слоном вниз-влево
                j = 1;
                rMove = bishopXY.r + j;
                cMove = bishopXY.c - j;
                while ((rMove > 0) && (cMove < 7) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) |
                    (board._board[rMove, cMove] == Cell.WhiteKnight) |
                    (board._board[rMove, cMove] == Cell.WhiteBishop) |
                    (board._board[rMove, cMove] == Cell.WhiteRook) |
                    (board._board[rMove, cMove] == Cell.WhiteQueen) |
                    (board._board[rMove, cMove] == Cell.WhiteKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(bishopXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //атака черным слоном вверх-влево
                j = 1;
                rMove = bishopXY.r - j;
                cMove = bishopXY.c - j;
                while ((rMove > 0) && (cMove < 7) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) |
                    (board._board[rMove, cMove] == Cell.WhiteKnight) |
                    (board._board[rMove, cMove] == Cell.WhiteBishop) |
                    (board._board[rMove, cMove] == Cell.WhiteRook) |
                    (board._board[rMove, cMove] == Cell.WhiteQueen) |
                    (board._board[rMove, cMove] == Cell.WhiteKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(bishopXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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
            }

            return Moves;
        }

        /// <summary>
        /// Поиск всех ходов, которые может совершить ладья
        /// </summary>
        static Move[] FindRookMoves(XY rookXY, Board board, Color color, XY kingXY, FiguresXY enemyFigures)
        {
            Move[] Moves = new Move[0];//массив для хранени всех ходов ладьи
            Move[] Temp1;
            Move Move;
            bool CheckKing = false;//Шах королю?
            int Count = 0; //количество найденных ходов ладьи
            int j;

            int rMove = -1;//Координаты искомого хода, строка
            int cMove = -1;//Координаты искомого хода, солбец

            if (color == Color.White)//ищем ходы белой ладьи
            {
                //атака белой ладьей вправо
                j = 1;
                rMove = rookXY.r;
                cMove = rookXY.c + j;

                while ((rookXY.c + j < 7) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) |
                    (board._board[rMove, cMove] == Cell.BlackKnight) |
                    (board._board[rMove, cMove] == Cell.BlackBishop) |
                    (board._board[rMove, cMove] == Cell.BlackRook) |
                    (board._board[rMove, cMove] == Cell.BlackQueen) |
                    (board._board[rMove, cMove] == Cell.BlackKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(rookXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //атака белой ладьей вниз
                j = 1;
                rMove = rookXY.r + j;
                cMove = rookXY.c;

                while ((rookXY.r + j < 7) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) |
                    (board._board[rMove, cMove] == Cell.BlackKnight) |
                    (board._board[rMove, cMove] == Cell.BlackBishop) |
                    (board._board[rMove, cMove] == Cell.BlackRook) |
                    (board._board[rMove, cMove] == Cell.BlackQueen) |
                    (board._board[rMove, cMove] == Cell.BlackKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(rookXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //атака белой ладьей влево
                j = 1;
                rMove = rookXY.r;
                cMove = rookXY.c - j;

                while ((rookXY.c - j > 0) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) |
                    (board._board[rMove, cMove] == Cell.BlackKnight) |
                    (board._board[rMove, cMove] == Cell.BlackBishop) |
                    (board._board[rMove, cMove] == Cell.BlackRook) |
                    (board._board[rMove, cMove] == Cell.BlackQueen) |
                    (board._board[rMove, cMove] == Cell.BlackKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(rookXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //атака белой ладьей вверх
                j = 1;
                rMove = rookXY.r - j;
                cMove = rookXY.c;

                while ((rookXY.r - j > 0) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) |
                    (board._board[rMove, cMove] == Cell.BlackKnight) |
                    (board._board[rMove, cMove] == Cell.BlackBishop) |
                    (board._board[rMove, cMove] == Cell.BlackRook) |
                    (board._board[rMove, cMove] == Cell.BlackQueen) |
                    (board._board[rMove, cMove] == Cell.BlackKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(rookXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

            }
            else//Если мы ищем атаки черной ладьи
            {
                //атака черной ладьей вправо
                j = 1;
                rMove = rookXY.r;
                cMove = rookXY.c + j;

                while ((rookXY.c + j < 7) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) |
                    (board._board[rMove, cMove] == Cell.WhiteKnight) |
                    (board._board[rMove, cMove] == Cell.WhiteBishop) |
                    (board._board[rMove, cMove] == Cell.WhiteRook) |
                    (board._board[rMove, cMove] == Cell.WhiteQueen) |
                    (board._board[rMove, cMove] == Cell.WhiteKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(rookXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //атака черной ладьей вниз
                j = 1;
                rMove = rookXY.r + j;
                cMove = rookXY.c;

                while ((rookXY.r + j < 7) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) |
                    (board._board[rMove, cMove] == Cell.WhiteKnight) |
                    (board._board[rMove, cMove] == Cell.WhiteBishop) |
                    (board._board[rMove, cMove] == Cell.WhiteRook) |
                    (board._board[rMove, cMove] == Cell.WhiteQueen) |
                    (board._board[rMove, cMove] == Cell.WhiteKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(rookXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //атака черной ладьей влево
                j = 1;
                rMove = rookXY.r;
                cMove = rookXY.c - j;

                while ((rookXY.c - j > 0) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) |
                    (board._board[rMove, cMove] == Cell.WhiteKnight) |
                    (board._board[rMove, cMove] == Cell.WhiteBishop) |
                    (board._board[rMove, cMove] == Cell.WhiteRook) |
                    (board._board[rMove, cMove] == Cell.WhiteQueen) |
                    (board._board[rMove, cMove] == Cell.WhiteKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(rookXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //атака черной ладьей вправо
                j = 1;
                rMove = rookXY.r - j;
                cMove = rookXY.c;

                while ((rookXY.r - j > 0) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) |
                    (board._board[rMove, cMove] == Cell.WhiteKnight) |
                    (board._board[rMove, cMove] == Cell.WhiteBishop) |
                    (board._board[rMove, cMove] == Cell.WhiteRook) |
                    (board._board[rMove, cMove] == Cell.WhiteQueen) |
                    (board._board[rMove, cMove] == Cell.WhiteKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(rookXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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
            }

            return Moves;
        }

        /// <summary>
        /// Поиск всех ходов, которые может совершить ферзь
        /// </summary>
        static Move[] FindQueenMoves(XY queenXY, Board board, Color color, XY kingXY, FiguresXY enemyFigures)
        {
            Move[] Moves = new Move[0];//массив для хранени всех ходов ферзя
            Move[] Temp1;
            Move[] Temp2;
            int Count = 0; //количество найденных ходов ферзя

            //атаки ферзя по диагонали
            Temp2 = FindBishopMoves(queenXY, board, color, kingXY, enemyFigures);
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
            Temp2 = FindRookMoves(queenXY, board, color, kingXY, enemyFigures);
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
        /// </summary>
        static Move[] FindKingMoves(XY kingXY, Board board, Color color, FiguresXY enemyFigures)
        {
            //король может совершить всего 8 ходов во все стороны на одну клетку
            Move[] Moves = new Move[0];//массив для хранени всех найденных ходов короля
            Move[] Temp;
            bool CheckKing = false;//Шах королю?
            Move Move;
            int Count = 0; //количество найденных ходов пешки

            int rMove = -1;//Координаты искомого хода, строка
            int cMove = -1;//Координаты искомого хода, солбец

            if (color == Color.White)//ищем ходы белого короля
            {
                //ход белого короля на одну клетку вправо
                rMove = kingXY.r;
                cMove = kingXY.c + 1;
                if ((cMove < 7) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) |
                    (board._board[rMove, cMove] == Cell.BlackKnight) |
                    (board._board[rMove, cMove] == Cell.BlackBishop) |
                    (board._board[rMove, cMove] == Cell.BlackRook) |
                    (board._board[rMove, cMove] == Cell.BlackQueen) |
                    (board._board[rMove, cMove] == Cell.BlackKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //ход белого короля на одну клетку вниз
                rMove = kingXY.r + 1;
                cMove = kingXY.c;

                if ((rMove < 7) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) |
                    (board._board[rMove, cMove] == Cell.BlackKnight) |
                    (board._board[rMove, cMove] == Cell.BlackBishop) |
                    (board._board[rMove, cMove] == Cell.BlackRook) |
                    (board._board[rMove, cMove] == Cell.BlackQueen) |
                    (board._board[rMove, cMove] == Cell.BlackKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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
                }

                //ход белого короля на одну клетку влево
                rMove = kingXY.r;
                cMove = kingXY.c - 1;

                if ((cMove > 0) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) |
                    (board._board[rMove, cMove] == Cell.BlackKnight) |
                    (board._board[rMove, cMove] == Cell.BlackBishop) |
                    (board._board[rMove, cMove] == Cell.BlackRook) |
                    (board._board[rMove, cMove] == Cell.BlackQueen) |
                    (board._board[rMove, cMove] == Cell.BlackKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //ход белого короля на одну клетку вверх
                rMove = kingXY.r - 1;
                cMove = kingXY.c;

                if ((rMove > 0) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) |
                    (board._board[rMove, cMove] == Cell.BlackKnight) |
                    (board._board[rMove, cMove] == Cell.BlackBishop) |
                    (board._board[rMove, cMove] == Cell.BlackRook) |
                    (board._board[rMove, cMove] == Cell.BlackQueen) |
                    (board._board[rMove, cMove] == Cell.BlackKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //ход белого короля вверх-вправо
                rMove = kingXY.r - 1;
                cMove = kingXY.c + 1;

                if ((rMove > 0) && (cMove < 7) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) |
                    (board._board[rMove, cMove] == Cell.BlackKnight) |
                    (board._board[rMove, cMove] == Cell.BlackBishop) |
                    (board._board[rMove, cMove] == Cell.BlackRook) |
                    (board._board[rMove, cMove] == Cell.BlackQueen) |
                    (board._board[rMove, cMove] == Cell.BlackKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //ход белого короля вниз-вправо
                rMove = kingXY.r + 1;
                cMove = kingXY.c + 1;

                if ((rMove < 7) && (cMove < 7) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) |
                    (board._board[rMove, cMove] == Cell.BlackKnight) |
                    (board._board[rMove, cMove] == Cell.BlackBishop) |
                    (board._board[rMove, cMove] == Cell.BlackRook) |
                    (board._board[rMove, cMove] == Cell.BlackQueen) |
                    (board._board[rMove, cMove] == Cell.BlackKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //ход белого короля вниз-влево
                rMove = kingXY.r + 1;
                cMove = kingXY.c - 1;

                if ((rMove < 7) && (cMove > 0) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) |
                    (board._board[rMove, cMove] == Cell.BlackKnight) |
                    (board._board[rMove, cMove] == Cell.BlackBishop) |
                    (board._board[rMove, cMove] == Cell.BlackRook) |
                    (board._board[rMove, cMove] == Cell.BlackQueen) |
                    (board._board[rMove, cMove] == Cell.BlackKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //ход белого короля вверх-влево
                rMove = kingXY.r - 1;
                cMove = kingXY.c - 1;

                if ((rMove > 0) && (cMove > 0) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) |
                    (board._board[rMove, cMove] == Cell.BlackKnight) |
                    (board._board[rMove, cMove] == Cell.BlackBishop) |
                    (board._board[rMove, cMove] == Cell.BlackRook) |
                    (board._board[rMove, cMove] == Cell.BlackQueen) |
                    (board._board[rMove, cMove] == Cell.BlackKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //ход белого короля: левая рокировка
                if ((board._WLRogueAvailable) && (board._board[7, 1] == Cell.Empty) &&//рокировка доступна
                    (board._board[7, 2] == Cell.Empty) && (board._board[7, 3] == Cell.Empty) &&//нужные поля свободны
                    (!CheckCheck(kingXY, board, color, enemyFigures)) &&//король не попадает под шах нигде на протяжении рокировки
                    (!CheckCheck(new XY(7, 2), board, color, enemyFigures)) &&
                    (!CheckCheck(new XY(7, 3), board, color, enemyFigures)))
                {
                    Move = new Move(kingXY, 7, 2);
                    Move._moveType = MoveType.WhiteLeftRogue;//Для Board.DoMove чтоб знать какие фигуры двигать

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

                //ход белого короля: правая рокировка
                if ((board._WRRogueAvailable) && (board._board[7, 5] == Cell.Empty) &&//рокировка доступна
                    (board._board[7, 6] == Cell.Empty) &&//нужные поля свободны
                    (!CheckCheck(kingXY, board, color, enemyFigures)) &&//король не попадает под шах нигде на протяжении рокировки
                    (!CheckCheck(new XY(7, 5), board, color, enemyFigures)) &&
                    (!CheckCheck(new XY(7, 6), board, color, enemyFigures)))
                {
                    Move = new Move(kingXY, 7, 6);
                    Move._moveType = MoveType.WhiteRightRogue;//Для Board.DoMove чтоб знать какие фигуры двигать

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
                rMove = kingXY.r;
                cMove = kingXY.c + 1;
                if ((cMove < 7) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) |
                    (board._board[rMove, cMove] == Cell.WhiteKnight) |
                    (board._board[rMove, cMove] == Cell.WhiteBishop) |
                    (board._board[rMove, cMove] == Cell.WhiteRook) |
                    (board._board[rMove, cMove] == Cell.WhiteQueen) |
                    (board._board[rMove, cMove] == Cell.WhiteKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //ход черного короля на одну клетку вниз
                rMove = kingXY.r + 1;
                cMove = kingXY.c;

                if ((rMove < 7) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) |
                    (board._board[rMove, cMove] == Cell.WhiteKnight) |
                    (board._board[rMove, cMove] == Cell.WhiteBishop) |
                    (board._board[rMove, cMove] == Cell.WhiteRook) |
                    (board._board[rMove, cMove] == Cell.WhiteQueen) |
                    (board._board[rMove, cMove] == Cell.WhiteKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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
                }

                //ход черного короля на одну клетку влево
                rMove = kingXY.r;
                cMove = kingXY.c - 1;

                if ((cMove > 0) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) |
                    (board._board[rMove, cMove] == Cell.WhiteKnight) |
                    (board._board[rMove, cMove] == Cell.WhiteBishop) |
                    (board._board[rMove, cMove] == Cell.WhiteRook) |
                    (board._board[rMove, cMove] == Cell.WhiteQueen) |
                    (board._board[rMove, cMove] == Cell.WhiteKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //ход черного короля на одну клетку вверх
                rMove = kingXY.r - 1;
                cMove = kingXY.c;

                if ((rMove > 0) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) |
                    (board._board[rMove, cMove] == Cell.WhiteKnight) |
                    (board._board[rMove, cMove] == Cell.WhiteBishop) |
                    (board._board[rMove, cMove] == Cell.WhiteRook) |
                    (board._board[rMove, cMove] == Cell.WhiteQueen) |
                    (board._board[rMove, cMove] == Cell.WhiteKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //ход черного короля вверх-вправо
                rMove = kingXY.r - 1;
                cMove = kingXY.c + 1;

                if ((rMove > 0) && (cMove < 7) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) |
                    (board._board[rMove, cMove] == Cell.WhiteKnight) |
                    (board._board[rMove, cMove] == Cell.WhiteBishop) |
                    (board._board[rMove, cMove] == Cell.WhiteRook) |
                    (board._board[rMove, cMove] == Cell.WhiteQueen) |
                    (board._board[rMove, cMove] == Cell.WhiteKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //ход черного короля вниз-вправо
                rMove = kingXY.r + 1;
                cMove = kingXY.c + 1;

                if ((rMove < 7) && (cMove < 7) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) |
                    (board._board[rMove, cMove] == Cell.WhiteKnight) |
                    (board._board[rMove, cMove] == Cell.WhiteBishop) |
                    (board._board[rMove, cMove] == Cell.WhiteRook) |
                    (board._board[rMove, cMove] == Cell.WhiteQueen) |
                    (board._board[rMove, cMove] == Cell.WhiteKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //ход черного короля вниз-влево
                rMove = kingXY.r + 1;
                cMove = kingXY.c - 1;

                if ((rMove < 7) && (cMove > 0) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) |
                    (board._board[rMove, cMove] == Cell.WhiteKnight) |
                    (board._board[rMove, cMove] == Cell.WhiteBishop) |
                    (board._board[rMove, cMove] == Cell.WhiteRook) |
                    (board._board[rMove, cMove] == Cell.WhiteQueen) |
                    (board._board[rMove, cMove] == Cell.WhiteKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //ход черного короля вверх-влево
                rMove = kingXY.r - 1;
                cMove = kingXY.c - 1;

                if ((rMove > 0) && (cMove > 0) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) |
                    (board._board[rMove, cMove] == Cell.WhiteKnight) |
                    (board._board[rMove, cMove] == Cell.WhiteBishop) |
                    (board._board[rMove, cMove] == Cell.WhiteRook) |
                    (board._board[rMove, cMove] == Cell.WhiteQueen) |
                    (board._board[rMove, cMove] == Cell.WhiteKing) |
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    CheckKing = CheckCheck(kingXY, board, color, enemyFigures);
                    if (!CheckKing)
                    {
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

                //ход черного короля: левая рокировка
                if ((board._BLRogueAvailable) && (board._board[0, 1] == Cell.Empty) &&//рокировка доступна
                    (board._board[0, 2] == Cell.Empty) && (board._board[0, 3] == Cell.Empty) &&//нужные поля свободны
                    (!CheckCheck(kingXY, board, color, enemyFigures)) &&//король не попадает под шах нигде на протяжении рокировки
                    (!CheckCheck(new XY(0, 2), board, color, enemyFigures)) &&
                    (!CheckCheck(new XY(0, 3), board, color, enemyFigures)))
                {
                    Move = new Move(kingXY, 0, 2);
                    Move._moveType = MoveType.BlackLeftRogue;//Для Board.DoMove чтоб знать какие фигуры двигать

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

                //ход черного короля: правая рокировка
                if ((board._BRRogueAvailable) && (board._board[0, 5] == Cell.Empty) &&//рокировка доступна
                    (board._board[0, 6] == Cell.Empty) &&//нужные поля свободны
                    (!CheckCheck(kingXY, board, color, enemyFigures)) &&//король не попадает под шах нигде на протяжении рокировки
                    (!CheckCheck(new XY(0, 5), board, color, enemyFigures)) &&
                    (!CheckCheck(new XY(0, 6), board, color, enemyFigures)))
                {
                    Move = new Move(kingXY, 7, 6);
                    Move._moveType = MoveType.BlackRightRogue;//Для Board.DoMove чтоб знать какие фигуры двигать

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
        /// Поиск всех ходов, которые может совершить выбранная фигура
        /// </summary>
        static public Move[] FindCellMoves(XY figureXY, Board board, Color color, XY kingXY, FiguresXY enemyFigures, Move enemyLastMove)
        {
            Move[] Moves = new Move[0];//массив для хранени всех ходов

            int rFig = figureXY.r;
            int cFig = figureXY.c;

            if (board._board[rFig, cFig] == Cell.Empty)//Если искомая клетка пуста, вернуть пустой массив
            { return Moves; }

            switch (board._board[rFig, cFig])//клетка с которой начинается ход
            {
                case (Cell.BlackPawn):
                case (Cell.WhitePawn):
                    Moves = FindPawnMoves(figureXY, board, color, enemyLastMove, kingXY, enemyFigures);
                    break;
                case (Cell.BlackKnight):
                case (Cell.WhiteKnight):
                    Moves = FindKnightMoves(figureXY, board, color, kingXY, enemyFigures);
                    break;
                case (Cell.BlackBishop):
                case (Cell.WhiteBishop):
                    Moves = FindBishopMoves(figureXY, board, color, kingXY, enemyFigures);
                    break;
                case (Cell.BlackRook):
                case (Cell.WhiteRook):
                    Moves = FindRookMoves(figureXY, board, color, kingXY, enemyFigures);
                    break;
                case (Cell.BlackQueen):
                case (Cell.WhiteQueen):
                    Moves = FindQueenMoves(figureXY, board, color, kingXY, enemyFigures);
                    break;
                case (Cell.BlackKing):
                case (Cell.WhiteKing):
                    Moves = FindKingMoves(figureXY, board, color, enemyFigures);
                    break;
                default:
                    return Moves;
            }

            return Moves;
        }

        #endregion

        #region Checks
        /// <summary>
        /// Атакует ли пешка заданную клетку?
        /// </summary>
        static bool CheckPawnAtacks(XY pawnXY, Color color, XY cellXY)
        {
            //Ответ на вопрос, находится ли заданная клетка под атакой конкретной пешки

            if (color == Color.White)//ищем атаки белой пешки
            {
                if (((cellXY.r == pawnXY.r - 1) && (cellXY.c == pawnXY.c - 1)) |
                    ((cellXY.r == pawnXY.r - 1) && (cellXY.c == pawnXY.c + 1)))
                {
                    return true;
                }
            }
            else//Если мы ищем атаки черной пешки
            {
                if (((cellXY.r == pawnXY.r + 1) && (cellXY.c == pawnXY.c - 1)) |
                    ((cellXY.r == pawnXY.r + 1) && (cellXY.c == pawnXY.c + 1)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Атакует ли конь заданную клетку?
        /// </summary>
        static bool CheckKnightAtacks(XY knightXY, XY cellXY)
        {
            //Ответ на вопрос, находится ли заданная клетка под атакой конкретного коня

            if (((cellXY.r == knightXY.r - 2) && (cellXY.c == knightXY.c + 1)) |
                ((cellXY.r == knightXY.r - 1) && (cellXY.c == knightXY.c + 2)) |
                ((cellXY.r == knightXY.r + 1) && (cellXY.c == knightXY.c + 2)) |
                ((cellXY.r == knightXY.r + 2) && (cellXY.c == knightXY.c + 1)) |
                ((cellXY.r == knightXY.r + 2) && (cellXY.c == knightXY.c - 1)) |
                ((cellXY.r == knightXY.r + 1) && (cellXY.c == knightXY.c - 2)) |
                ((cellXY.r == knightXY.r - 1) && (cellXY.c == knightXY.c - 2)) |
                ((cellXY.r == knightXY.r - 2) && (cellXY.c == knightXY.c - 1)))
            {
                return true;
            }
            else { return false; }   
        }

        /// <summary>
        /// Атакует ли слон заданную клетку?
        /// </summary>
        static bool CheckBishopAtacks(XY bishopXY, Board board, XY cellXY)
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

            if ((cellXY.r < bishopXY.r) && (cellXY.c > bishopXY.c))//клетка сверху-справа
            {
                for (int i = 1; i < CountMiddleCells + 1; i++)
                {
                    if (board._board[bishopXY.r - i, bishopXY.c + i] != Cell.Empty)//Если между ними не пустая клетка
                    { return false; }
                }
                return true;
            }
            else if ((cellXY.r > bishopXY.r) && (cellXY.c > bishopXY.c))//клетка снизу-справа
            {
                for (int i = 1; i < CountMiddleCells + 1; i++)
                {
                    if (board._board[bishopXY.r + i, bishopXY.c + i] != Cell.Empty)//Если между ними не пустая клетка
                    { return false; }
                }
                return true;
            }
            else if ((cellXY.r > bishopXY.r) && (cellXY.c < bishopXY.c))//клетка снизу-слева
            {
                for (int i = 1; i < CountMiddleCells + 1; i++)
                {
                    if (board._board[bishopXY.r + i, bishopXY.c - i] != Cell.Empty)//Если между ними не пустая клетка
                    { return false; }
                }
                return true;
            }
            else if ((cellXY.r < bishopXY.r) && (cellXY.c < bishopXY.c))//клетка сверху-слева
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
        static bool CheckRookAtacks(XY rookXY, Board board, XY cellXY)
        {
            //Ответ на вопрос, находится ли заданная клетка под атакой конкретной ладьи

            //для начала нужно убедиться что ладья и атакованная клетка лежат на одной горизонтали/вертикали
            if ( (cellXY.r != rookXY.r) && (cellXY.c != rookXY.c) )
            {
                return false;
            }

            //если ладья рядом с клеткой то она точно может ее атаковать
            int dr = Math.Abs(cellXY.r - rookXY.r);//абсолютная разница по строкам
            int dc = Math.Abs(cellXY.c - rookXY.c);//абсолютная разница по столбцам
            if (( dr + dc ) == 1)
            { return true; }

            int CountMiddleCells = dr + dc - 1;//количество пустых клеток между ладьей и целью

            if ((cellXY.r == rookXY.r) && (cellXY.c > rookXY.c))//клетка сверху-справа
            {
                for (int i = 1; i < CountMiddleCells + 1; i++)
                {
                    if (board._board[rookXY.r, rookXY.c + i] != Cell.Empty)//Если между ними не пустая клетка
                    { return false; }
                }
                return true;
            }
            else if ((cellXY.r > rookXY.r) && (cellXY.c == rookXY.c))//клетка снизу-справа
            {
                for (int i = 1; i < CountMiddleCells + 1; i++)
                {
                    if (board._board[rookXY.r + i, rookXY.c] != Cell.Empty)//Если между ними не пустая клетка
                    { return false; }
                }
                return true;
            }
            else if ((cellXY.r == rookXY.r) && (cellXY.c < rookXY.c))//клетка снизу-слева
            {
                for (int i = 1; i < CountMiddleCells + 1; i++)
                {
                    if (board._board[rookXY.r, rookXY.c - i] != Cell.Empty)//Если между ними не пустая клетка
                    { return false; }
                }
                return true;
            }
            else if ((cellXY.r < rookXY.r) && (cellXY.c == rookXY.c))//клетка сверху-слева
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
        static bool CheckQueenAtacks(XY queenXY, Board board, XY cellXY)
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
        static bool CheckKingAtacks(XY kingXY, XY cellXY)
        {
            //Ответ на вопрос, находится ли заданная клетка под атакой конкретного короля

            if (((cellXY.r == kingXY.r) && (cellXY.c == kingXY.c + 1)) |
                ((cellXY.r == kingXY.r + 1) && (cellXY.c == kingXY.c)) |
                ((cellXY.r == kingXY.r) && (cellXY.c == kingXY.c - 1)) |
                ((cellXY.r == kingXY.r - 1) && (cellXY.c == kingXY.c)) |
                ((cellXY.r == kingXY.r - 1) && (cellXY.c == kingXY.c + 1)) |
                ((cellXY.r == kingXY.r + 1) && (cellXY.c == kingXY.c + 1)) |
                ((cellXY.r == kingXY.r + 1) && (cellXY.c == kingXY.c - 1)) |
                ((cellXY.r == kingXY.r - 1) && (cellXY.c == kingXY.c - 1)))
            {
                return true;
            }
            else { return false; }
        }

        /// <summary>
        /// Проверка доски на шах королю
        /// </summary>
        static bool CheckCheck(XY kingXY, Board board, Color kingColor, FiguresXY enemyFigures)
        {
            int FiguresCount = enemyFigures.Figures.Length;
            bool TempFlag = false;//флаг для проверки что эта фигура атакует короля.
            //если флаг равен 1, значит королю шах

            for (int i = 0; i < FiguresCount; i++)
            {
                int rEnemy = enemyFigures.Figures[i].r;
                int cEnemy = enemyFigures.Figures[i].c;
                XY enemyXY = enemyFigures.Figures[i];

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
        /// Проверка доски на шах королю
        /// </summary>
        static bool CheckMate(XY kingXY, Board board, Color kingColor, FiguresXY Figures, FiguresXY enemyFigures, Move enemyLastMove)
        {
            int FiguresCount = Figures.Figures.Length;//Количество наших фигур
            Move[] FoundMoves = null;//массив доступных ходов нашей фигуры

            for (int i = 0; i < FiguresCount; i++)
            {
                int rFig = Figures.Figures[i].r;
                int cFig = Figures.Figures[i].c;
                XY FigXY = Figures.Figures[i];// координаты фигуры, которую мы сейчас проверяем

                if (kingColor == Color.White)//проверяем шах белого короля
                {
                    switch (board._board[rFig, cFig])//клетка с которой начинается ход
                    {
                        case (Cell.WhitePawn):
                            FoundMoves = FindPawnMoves(FigXY, board, kingColor, enemyLastMove, kingXY, enemyFigures);
                            break;
                        case (Cell.WhiteKnight):
                            FoundMoves = FindKnightMoves(FigXY, board, kingColor, kingXY, enemyFigures);
                            break;
                        case (Cell.WhiteBishop):
                            FoundMoves = FindBishopMoves(FigXY, board, kingColor, kingXY, enemyFigures);
                            break;
                        case (Cell.WhiteRook):
                            FoundMoves = FindRookMoves(FigXY, board, kingColor, kingXY, enemyFigures);
                            break;
                        case (Cell.WhiteQueen):
                            FoundMoves = FindQueenMoves(FigXY, board, kingColor, kingXY, enemyFigures);
                            break;
                        case (Cell.WhiteKing):
                            FoundMoves = FindKingMoves(FigXY, board, kingColor, enemyFigures);
                            break;
                        default:
                            return false;
                    }
                }
                else//проверяем шах черного короля
                {
                    switch (board._board[rFig, cFig])//клетка с которой начинается ход
                    {
                        case (Cell.BlackPawn):
                            FoundMoves = FindPawnMoves(FigXY, board, kingColor, enemyLastMove, kingXY, enemyFigures);
                            break;
                        case (Cell.BlackKnight):
                            FoundMoves = FindKnightMoves(FigXY, board, kingColor, kingXY, enemyFigures);
                            break;
                        case (Cell.BlackBishop):
                            FoundMoves = FindBishopMoves(FigXY, board, kingColor, kingXY, enemyFigures);
                            break;
                        case (Cell.BlackRook):
                            FoundMoves = FindRookMoves(FigXY, board, kingColor, kingXY, enemyFigures);
                            break;
                        case (Cell.BlackQueen):
                            FoundMoves = FindQueenMoves(FigXY, board, kingColor, kingXY, enemyFigures);
                            break;
                        case (Cell.BlackKing):
                            FoundMoves = FindKingMoves(FigXY, board, kingColor, enemyFigures);
                            break;
                        default:
                            return false;
                    }
                }

                if (FoundMoves != null)//нужно узнать, сужествует ли хоть один ход, доступный фигуре игрока
                {
                    return true;
                }
            }
            return false;//обойдя все фигуры не нашлось фигуры у которой был бы возможный ход
        }

        /// <summary>
        /// Проверка хода на соответствие правилам
        /// </summary>
        static bool CheckMove(XY kingXY, Board board, Color color, FiguresXY enemyFigures, Move move, Move enemyLastMove)
        {
            int r1 = move.XY1.r;//строка начала хода
            int c1 = move.XY1.c;//столбец начала хода
            int r2 = move.XY2.r;//строка конца хода
            int c2 = move.XY2.c;//столбец конца хода
            Move[] FoundMoves = null;//все ходы фигуры, которая ходит

            //для начала нужно убедиться что мы пытаемся ходить своей фигурой
            if (color == Color.White)//если мы пытаемся ходить белыми фигурами
            {

                switch (board._board[r1, c1])//клетка с которой начинается ход
                {
                    case (Cell.WhitePawn):
                        FoundMoves = FindPawnMoves(move.XY1, board, color, enemyLastMove, kingXY, enemyFigures);
                        break;
                    case (Cell.WhiteKnight):
                        FoundMoves = FindKnightMoves(move.XY1, board, color, kingXY, enemyFigures);
                        break;
                    case (Cell.WhiteBishop):
                        FoundMoves = FindBishopMoves(move.XY1, board, color, kingXY, enemyFigures);
                        break;
                    case (Cell.WhiteRook):
                        FoundMoves = FindRookMoves(move.XY1, board, color, kingXY, enemyFigures);
                        break;
                    case (Cell.WhiteQueen):
                        FoundMoves = FindQueenMoves(move.XY1, board, color, kingXY, enemyFigures);
                        break;
                    case (Cell.WhiteKing):
                        FoundMoves = FindKingMoves(move.XY1, board, color, enemyFigures);
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
                        FoundMoves = FindPawnMoves(move.XY1, board, color, enemyLastMove, kingXY, enemyFigures);
                        break;
                    case (Cell.BlackKnight):
                        FoundMoves = FindKnightMoves(move.XY1, board, color, kingXY, enemyFigures);
                        break;
                    case (Cell.BlackBishop):
                        FoundMoves = FindBishopMoves(move.XY1, board, color, kingXY, enemyFigures);
                        break;
                    case (Cell.BlackRook):
                        FoundMoves = FindRookMoves(move.XY1, board, color, kingXY, enemyFigures);
                        break;
                    case (Cell.BlackQueen):
                        FoundMoves = FindQueenMoves(move.XY1, board, color, kingXY, enemyFigures);
                        break;
                    case (Cell.BlackKing):
                        FoundMoves = FindKingMoves(move.XY1, board, color, enemyFigures);
                        break;
                    default:
                        return false;
                }
            }

            if (FoundMoves != null)//берутся все возможные ходы этой фигуры и сравниваются с предложенным ходом
            {
                int CountFound = FoundMoves.Length;
                for (int i = 0; i < CountFound; i++)
                {
                    if ((FoundMoves[i].XY2.r == r2) && (FoundMoves[i].XY2.c == c2))
                    {
                        return true;//заданный ход найден среди возможных
                    }
                }
            }

            return false;//заданного хода нет среди доступных
        }


        #endregion
    }

    public class XY //содержит строку и ряд клетки на доске
    {
        public int r;//row
        public int c;//column

        public XY(int r, int c)
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

    public class Move //содержит координаты начала и конца хода
    {
        private XY[] _move;//массив в котором лежат координаты начала и конца хода
        public XY XY1//Свойства чтобы можно было смотреть позиции начала и конца хода
        {
            get { return _move[0]; }
        }
        public XY XY2
        {
            get { return _move[1]; }
        }

        public MoveType _moveType = MoveType.Moving;//тип хода

        public Move(XY xy1, int x2, int y2, MoveType type = MoveType.Moving)//конструктор из координат хода
        {
            XY xy2 = new XY(x2, y2);
            _move = new XY[2] { xy1, xy2 };
            _moveType = type;
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

    public class FiguresXY //содержит координаты всех фигур игрока кроме короля
    {
        private XY[] _figures;//массив координат
        public XY[] Figures//Свойство чтобы можно было использовать массив в циклах
        {
            get { return _figures; }
        }
        private XY _king;//координаты короля
        public XY King//Свойство чтобы можно было использовать массив в циклах
        {
            get { return _king; }
        }

        public FiguresXY(XY[] figures, XY king)//конструктор из массива
        {
            _figures = figures;
            _king = king;
        }

        public FiguresXY(FiguresXY a)//конструктор копирования
        {
            _figures = a._figures;
            _king = a._king;
        }

        public FiguresXY(Color color)//конструктор для начальной позиции шахмат
        {
            int p = 0;//итератор для выходного массива фигур
            XY[] OutFigures = new XY[16];
            XY TempXY = null;

            if (color == Color.White)
            {
                for (int j = 7; j > 5; j--)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        TempXY = new XY(j, i);
                        OutFigures[p] = TempXY;
                        p++;
                    }
                }
                TempXY = new XY(7, 4);
                _king = TempXY;
            }
            else
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        TempXY = new XY(j, i);
                        OutFigures[p] = TempXY;
                        p++;
                    }
                }
                TempXY = new XY(0, 4);
                _king = TempXY;
            }
            _figures = OutFigures;
        }
    }
}
