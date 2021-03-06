﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessFantasy
{
    #region перечисления
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
    #endregion

    public class Board //содержит доску с фигурами и все дествия с ней, а также всю информацию для игры
    {
        #region свойства
        //Флаги о том что белый и черный игроки двигали королей и ладьи (для рокировки)
        private bool _WLRogueAvailable = true;//У белых есть право на длинную рокировку (левая ладья и король не делали ход)
        public bool WLRogueAvailable  {  get { return _WLRogueAvailable; }    }

        private bool _WRRogueAvailable = true;//У белых есть право на короткую рокировку (правая ладья и король не делали ход)
        public bool WRRogueAvailable { get { return _WRRogueAvailable; } }

        private bool _BLRogueAvailable = true;//У черных есть право на длинную рокировку (левая ладья и король не делали ход)
        public bool BLRogueAvailable { get { return _BLRogueAvailable; } }

        private bool _BRRogueAvailable = true;//У черных есть право на короткую рокировку (правая ладья и король не делали ход)
        public bool BRRogueAvailable { get { return _BRRogueAvailable; } }

        private Cell[,] _board; //Сама доска, массив 8х8 в котором записано состояние каждой клетки
        public Cell[,] BoardArr//Свойства чтобы можно было снаружи смотреть массив
        {
            get { return _board; }
        }

        private Color _nextColor = Color.White; //Цвет игрока, который ходит следующим
        public Color NextColor//Свойства чтобы можно было снаружи смотреть массив
        {
            get { return _nextColor; }
        }

        private FiguresXY _WhiteFigures;//массив координат всех белых фигур, в том числе короля
        public FiguresXY WhiteFigures//Свойства чтобы можно было снаружи смотреть массив
        {
            get { return _WhiteFigures; }
        }
        private FiguresXY _BlackFigures;//массив координат всех черных фигур, в том числе короля
        public FiguresXY BlackFigures//Свойства чтобы можно было снаружи смотреть массив
        {
            get { return _BlackFigures; }
        }
        private Move _LastMove = null;//последний ход для эмпасана
        public Move LastMove//Свойства чтобы можно было снаружи смотреть массив
        {
            get { return _LastMove; }
            set { _LastMove = value; }
        }
        #endregion

        /// <summary>
        ///создание начальной доски
        /// </summary>
        public Board()
            {
                _board = new Cell[8, 8] {{ (Cell)10, (Cell)8, (Cell)9, (Cell)11, (Cell)12, (Cell)9, (Cell)8, (Cell)10 },
                                         { (Cell)7,  (Cell)7, (Cell)7, (Cell)7,  (Cell)7,  (Cell)7, (Cell)7, (Cell)7 },
                                         { 0,        0,       0,       0,        0,        0,       0,       0 },
                                         { 0,        0,       0,       0,        0,        0,       0,       0 },
                                         { 0,        0,       0,       0,        0,        0,       0,       0 },
                                         { 0,        0,       0,       0,        0,        0,       0,       0 },
                                         { (Cell)1,  (Cell)1, (Cell)1, (Cell)1,  (Cell)1,  (Cell)1, (Cell)1, (Cell)1 },
                                         { (Cell)4,  (Cell)2, (Cell)3, (Cell)5,  (Cell)6,  (Cell)3, (Cell)2, (Cell)4 } };

            //нужно в конструктор добавить инициализацию массивов фигур
            _BlackFigures = new FiguresXY(Color.Black);//добавить все черные фигуры
            _WhiteFigures = new FiguresXY(Color.White);//добавить все белые фигуры
        }

        /// <summary>
        ///конструктор копирования
        /// </summary>
        public Board(Board a)
        {
            _WLRogueAvailable = a._WLRogueAvailable;
            _WRRogueAvailable = a._WRRogueAvailable;
            _BLRogueAvailable = a._BLRogueAvailable;
            _BRRogueAvailable = a._BRRogueAvailable;
            _nextColor = a._nextColor;

            _LastMove =  new Move(a._LastMove);

            _board = new Cell[8, 8];
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    _board[r,c] = a._board[r, c];
                }
            }

            //Поэлементное копирование массива белых фигур
            this._WhiteFigures = new FiguresXY(a._WhiteFigures); ;

            //Поэлементное копирование массива черных фигур
            this._BlackFigures = new FiguresXY(a._BlackFigures); ;
        }

        /// <summary>
        /// Возвращает доску с примененным ходом без проверки на правила
        /// </summary>
        public static Board DoMove(Board board, Move move)
        {
            Color color = board._nextColor;
            //если координаты хода находятся на доске
            if ((move.XY1.r > -1) && (move.XY1.r < 8) && (move.XY1.c > -1) && (move.XY1.c < 8) &&
                (move.XY2.r > -1) && (move.XY2.r < 8) && (move.XY2.c > -1) && (move.XY2.c < 8))
            {
                Board boardOut = new Board(board);//доска с выполненным ходом
                Cell Figure = boardOut._board[move.XY1.r, move.XY1.c];//фигура которой мы ходим

                boardOut._board[move.XY1.r, move.XY1.c] = Cell.Empty;//клетка с которой начинался ход обнуляется
                boardOut._board[move.XY2.r, move.XY2.c] = Figure;//клетка, где заканчивается ход, занимается фигурой
                
                 
                    if (color == Color.White)//Если это какой-то экзотический ход типа взятия на проходе или рокировки, то нужно знать цвет фигуры
                {
                    boardOut.WhiteFigures.RewriteXY(move.XY1, move.XY2);//меняем координаты в массиве
                    if (move._moveType == MoveType.Taking)
                    {
                        boardOut.BlackFigures.DeleteXY(move.XY2);//удаляем съеденную фигуру
                    }

                    if (Figure == Cell.WhiteKing) //лишаем права на экзотические ходы
                    {
                        boardOut._WLRogueAvailable = false;
                        boardOut._WRRogueAvailable = false;
                        boardOut._WhiteFigures.King = move.XY2;
                    }
                    else if ((Figure == Cell.WhiteRook) && (move.XY1.r == 7) && (move.XY1.c == 0))//если игрок ходит ладьей
                        { boardOut._WLRogueAvailable = false; }
                    else if((Figure == Cell.WhiteRook) && (move.XY1.r == 7) && (move.XY1.c == 7))//если игрок ходит ладьей
                    { boardOut._WRRogueAvailable = false; }
                    else if ((move.XY2.r == 0) && (move.XY2.c == 0))//если черную ладью съели
                    { boardOut._BLRogueAvailable = false; }
                    else if ((move.XY2.r == 0) && (move.XY2.c == 7))//если черную ладью съели
                    { boardOut._BRRogueAvailable = false; }

                    if (move._moveType == MoveType.WhiteLeftEmpassant)//взятие на проходе белой пешкой влево
                    {
                        boardOut._board[move.XY1.r, move.XY1.c - 1] = Cell.Empty;//клетка на которой стояла вражеская пешка обнуляется
                        XY Temp = new XY(move.XY1.r, move.XY1.c - 1);
                        boardOut.BlackFigures.DeleteXY(Temp);//удаляем съеденную фигуру
                    }
                    else if (move._moveType == MoveType.WhiteRightEmpassant)//взятие на проходе белой пешкой вправо
                    {
                        boardOut._board[move.XY1.r, move.XY1.c + 1] = Cell.Empty;//клетка на которой стояла вражеская пешка обнуляется
                        XY Temp = new XY(move.XY1.r, move.XY1.c + 1);
                        boardOut.BlackFigures.DeleteXY(Temp);//удаляем съеденную фигуру
                    }
                    else if (move._moveType == MoveType.WhiteLeftRogue)//рокировка белого короля влево
                    {
                        boardOut._board[7, 0] = Cell.Empty;//клетка на которой стояла ладья обнуляется
                        boardOut._board[7, 3] = Cell.WhiteRook;//ладья переставляется
                        XY Old = new XY(7, 0);
                        XY New = new XY(7, 3);
                        boardOut.WhiteFigures.RewriteXY(Old, New);//удаляем съеденную фигуру
                    }
                    else if (move._moveType == MoveType.WhiteRightRogue)//рокировка белого короля вправо
                    {
                        boardOut._board[7, 7] = Cell.Empty;//клетка на которой стояла ладья обнуляется
                        boardOut._board[7, 5] = Cell.WhiteRook;//ладья переставляется
                        XY Old = new XY(7, 7);
                        XY New = new XY(7, 5);
                        boardOut.WhiteFigures.RewriteXY(Old, New);//удаляем съеденную фигуру
                    }
                    
                    //если пешко дошла до конца
                    if ((move.XY2.r == 0) && (boardOut._board[move.XY2.r, move.XY2.c] == Cell.WhitePawn))
                    {
                        boardOut._board[move.XY2.r, move.XY2.c] = Cell.WhiteQueen;//дать ферзя
                    }
                }
                else //Ходим черными фигурами рокировку и взятие на проходе
                {
                    boardOut.BlackFigures.RewriteXY(move.XY1, move.XY2);//меняем координаты в массиве
                    if (move._moveType == MoveType.Taking)
                    {
                        boardOut.WhiteFigures.DeleteXY(move.XY2);//удаляем съеденную фигуру
                    }
                    if (Figure == Cell.BlackKing)
                    {
                        boardOut._BLRogueAvailable = false;
                        boardOut._BRRogueAvailable = false;
                        boardOut._BlackFigures.King = move.XY2;
                    }
                    else if ((Figure == Cell.BlackRook) && (move.XY1.r == 0) && (move.XY1.c == 0))
                    { boardOut._BLRogueAvailable = false; }
                    else if ((Figure == Cell.BlackRook) && (move.XY1.r == 0) && (move.XY1.c == 7))
                    { boardOut._BRRogueAvailable = false; }
                    else if ((move.XY2.r == 7) && (move.XY2.c == 0))//если белую ладью съели
                    { boardOut._WLRogueAvailable = false; }
                    else if ((move.XY2.r == 7) && (move.XY2.c == 7))//если белую ладью съели
                    { boardOut._WRRogueAvailable = false; }

                    if (move._moveType == MoveType.BlackLeftEmpassant)//взятие на проходе черной пешкой влево
                    {
                        boardOut._board[move.XY1.r, move.XY1.c - 1] = Cell.Empty;//клетка на которой стояла вражеская пешка обнуляется
                        XY Temp = new XY(move.XY1.r, move.XY1.c - 1);
                        boardOut.WhiteFigures.DeleteXY(Temp);//удаляем съеденную фигуру
                    }
                    else if (move._moveType == MoveType.BlackRightEmpassant)//взятие на проходе черной пешкой вправо
                    {
                        boardOut._board[move.XY1.r, move.XY1.c + 1] = Cell.Empty;//клетка на которой стояла вражеская пешка обнуляется
                        XY Temp = new XY(move.XY1.r, move.XY1.c + 1);
                        boardOut.WhiteFigures.DeleteXY(Temp);//удаляем съеденную фигуру
                    }
                    else if (move._moveType == MoveType.BlackLeftRogue)//рокировка черного короля влево
                    {
                        boardOut._board[0, 0] = Cell.Empty;//клетка на которой стояла ладья обнуляется
                        boardOut._board[0, 3] = Cell.BlackRook;//ладья переставляется
                        XY Old = new XY(0, 0);
                        XY New = new XY(0, 3);
                        boardOut.BlackFigures.RewriteXY(Old, New);//удаляем съеденную фигуру
                    }
                    else if (move._moveType == MoveType.BlackRightRogue)//рокировка черного короля вправо
                    {
                        boardOut._board[0, 7] = Cell.Empty;//клетка на которой стояла ладья обнуляется
                        boardOut._board[0, 5] = Cell.BlackRook;//ладья переставляется
                        XY Old = new XY(0, 7);
                        XY New = new XY(0, 5);
                        boardOut.BlackFigures.RewriteXY(Old, New);//удаляем съеденную фигуру
                    }

                    //если пешко дошла до конца
                    if ((move.XY2.r == 7) && (boardOut._board[move.XY2.r, move.XY2.c] == Cell.BlackPawn))
                    {
                        boardOut._board[move.XY2.r, move.XY2.c] = Cell.BlackQueen;//дать ферзя
                    }
                }

                if (boardOut._nextColor == Color.White)//нужно передать право хода другому игроку
                {
                    boardOut._nextColor = Color.Black;
                }
                else
                {
                    boardOut._nextColor = Color.White;
                }

                boardOut.LastMove = move;//записываем как последний ход

                return boardOut;
            }
            else { return null; }
        }

        /// <summary>
        /// Инвертирует цвет игрока который должен ходить
        /// </summary>
        public Board InvertColor()
        {
            if (this._nextColor == Color.White)
            { this._nextColor = Color.Black; }
            else
            { this._nextColor = Color.White; }
            return this;
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
            Board TempBoard;//доска на которой проверяется шах королю
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
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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
                if ((rMove > -1) && (cMove > -1) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) ||
                    (board._board[rMove, cMove] == Cell.BlackKnight) ||
                    (board._board[rMove, cMove] == Cell.BlackBishop) ||
                    (board._board[rMove, cMove] == Cell.BlackRook) ||
                    (board._board[rMove, cMove] == Cell.BlackQueen) ||
                    (board._board[rMove, cMove] == Cell.BlackKing)))
                {
                    Move = new Move(pawnXY, rMove, cMove, MoveType.Taking);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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
                if ((cMove < 8) && (rMove > -1) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) ||
                    (board._board[rMove, cMove] == Cell.BlackKnight) ||
                    (board._board[rMove, cMove] == Cell.BlackBishop) ||
                    (board._board[rMove, cMove] == Cell.BlackRook) ||
                    (board._board[rMove, cMove] == Cell.BlackQueen) ||
                    (board._board[rMove, cMove] == Cell.BlackKing)))
                {
                    Move = new Move(pawnXY, rMove, cMove, MoveType.Taking);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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
                if ((pawnXY.c > 0) && //нужно чтоб игура могла ходить налево
                    (pawnXY.r == 3) && //нужно находиться на нужной горизонтали(5-й)
                    (board._board[pawnXY.r, pawnXY.c - 1] == Cell.BlackPawn) &&// чтобы слева была вражеская пешка
                    (enemyLastMove.XY1.r == 1) && (enemyLastMove.XY1.c == pawnXY.c - 1) && //прошлый вражеский ход начинался с 7 строки и нужного столбца
                    (enemyLastMove.XY2.r == 3) && (enemyLastMove.XY1.c == pawnXY.c - 1))//прошлый вражеский ход закончился на 5 строке и нужном столбце
                {
                    Move = new Move(pawnXY, pawnXY.r - 1, pawnXY.c - 1, MoveType.WhiteLeftEmpassant);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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
                if ((pawnXY.c < 7) && //нужно чтоб игура могла ходить направо
                    (pawnXY.r == 3) && //нужно находиться на нужной горизонтали(5-й)
                    (board._board[pawnXY.r, pawnXY.c + 1] == Cell.BlackPawn) &&// чтобы слева была вражеская пешка
                    (enemyLastMove.XY1.r == 1) && (enemyLastMove.XY1.c == pawnXY.c + 1) && //прошлый вражеский ход начинался с 7 строки и нужного столбца
                    (enemyLastMove.XY2.r == 3) && (enemyLastMove.XY1.c == pawnXY.c + 1))//прошлый вражеский ход закончился на 5 строке и нужном столбце
                {
                    Move = new Move(pawnXY, pawnXY.r - 1, pawnXY.c + 1, MoveType.WhiteRightEmpassant);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
                    if (!CheckKing)
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //ход черной пешкой на две клетки вперед
                if ((pawnXY.r == 1) && (board._board[pawnXY.r + 1, pawnXY.c] == Cell.Empty) &&
                    (board._board[pawnXY.r + 2, pawnXY.c] == Cell.Empty))
                {
                    Move = new Move(pawnXY, pawnXY.r + 2, pawnXY.c);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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
                if ((cMove > -1) && (rMove < 8) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) ||
                    (board._board[rMove, cMove] == Cell.WhiteKnight) ||
                    (board._board[rMove, cMove] == Cell.WhiteBishop) ||
                    (board._board[rMove, cMove] == Cell.WhiteRook) ||
                    (board._board[rMove, cMove] == Cell.WhiteQueen) ||
                    (board._board[rMove, cMove] == Cell.WhiteKing)))
                {
                    Move = new Move(pawnXY, rMove, cMove, MoveType.Taking);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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
                if ((cMove < 8) && (rMove < 8) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) ||
                    (board._board[rMove, cMove] == Cell.WhiteKnight) ||
                    (board._board[rMove, cMove] == Cell.WhiteBishop) ||
                    (board._board[rMove, cMove] == Cell.WhiteRook) ||
                    (board._board[rMove, cMove] == Cell.WhiteQueen) ||
                    (board._board[rMove, cMove] == Cell.WhiteKing)))
                {
                    Move = new Move(pawnXY, rMove, cMove, MoveType.Taking);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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
                if ((pawnXY.c > 0) && //нужно чтоб игура могла ходить налево
                    (pawnXY.r == 4) && //нужно находиться на нужной горизонтали(4-й)
                    (board._board[pawnXY.r, pawnXY.c - 1] == Cell.BlackPawn) &&// чтобы слева была вражеская пешка
                    (enemyLastMove.XY1.r == 6) && (enemyLastMove.XY1.c == pawnXY.c - 1) && //прошлый вражеский ход начинался с 2 строки и нужного столбца
                    (enemyLastMove.XY2.r == 4) && (enemyLastMove.XY1.c == pawnXY.c - 1))//прошлый вражеский ход закончился на 4 строке и нужном столбце
                {
                    Move = new Move(pawnXY, pawnXY.r + 1, pawnXY.c - 1, MoveType.BlackLeftEmpassant);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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
                if ((pawnXY.c < 7) && //нужно чтоб игура могла ходить направо
                    (pawnXY.r == 4) && //нужно находиться на нужной горизонтали(5-й)
                    (board._board[pawnXY.r, pawnXY.c + 1] == Cell.BlackPawn) &&// чтобы слева была вражеская пешка
                    (enemyLastMove.XY1.r == 6) && (enemyLastMove.XY1.c == pawnXY.c + 1) && //прошлый вражеский ход начинался с 7 строки и нужного столбца
                    (enemyLastMove.XY2.r == 4) && (enemyLastMove.XY1.c == pawnXY.c + 1))//прошлый вражеский ход закончился на 5 строке и нужном столбце
                {
                    Move = new Move(pawnXY, pawnXY.r + 1, pawnXY.c + 1, MoveType.BlackRightEmpassant);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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
            Board TempBoard;//доска на которой проверяется шах королю
            bool CheckKing = false;//Шах королю?
            int Count = 0; //количество найденных ходов коня

            int rMove = -1;//Координаты искомого хода, строка
            int cMove = -1;//Координаты искомого хода, солбец

            if (color == Color.White)//ищем ходы белого коня
            {
                //атака белым конем на 1 час
                rMove = knightXY.r - 2;
                cMove = knightXY.c + 1;
                if ((rMove > -1) && (cMove < 8) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) ||
                    (board._board[rMove, cMove] == Cell.BlackKnight) ||
                    (board._board[rMove, cMove] == Cell.BlackBishop) ||
                    (board._board[rMove, cMove] == Cell.BlackRook) ||
                    (board._board[rMove, cMove] == Cell.BlackQueen) ||
                    (board._board[rMove, cMove] == Cell.BlackKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
                    if (!CheckKing)
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //атака белым конем на 2 часа
                rMove = knightXY.r - 1;
                cMove = knightXY.c + 2;
                if ((rMove > -1) && (cMove < 8) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) ||
                    (board._board[rMove, cMove] == Cell.BlackKnight) ||
                    (board._board[rMove, cMove] == Cell.BlackBishop) ||
                    (board._board[rMove, cMove] == Cell.BlackRook) ||
                    (board._board[rMove, cMove] == Cell.BlackQueen) ||
                    (board._board[rMove, cMove] == Cell.BlackKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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

                //атака белым конем на 4 часа
                rMove = knightXY.r + 1;
                cMove = knightXY.c + 2;
                if ((rMove < 8) && (cMove < 8) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) ||
                    (board._board[rMove, cMove] == Cell.BlackKnight) ||
                    (board._board[rMove, cMove] == Cell.BlackBishop) ||
                    (board._board[rMove, cMove] == Cell.BlackRook) ||
                    (board._board[rMove, cMove] == Cell.BlackQueen) ||
                    (board._board[rMove, cMove] == Cell.BlackKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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
                if ((rMove < 8) && (cMove < 8) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) ||
                    (board._board[rMove, cMove] == Cell.BlackKnight) ||
                    (board._board[rMove, cMove] == Cell.BlackBishop) ||
                    (board._board[rMove, cMove] == Cell.BlackRook) ||
                    (board._board[rMove, cMove] == Cell.BlackQueen) ||
                    (board._board[rMove, cMove] == Cell.BlackKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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
                if ((rMove < 8) && (cMove > -1) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) ||
                    (board._board[rMove, cMove] == Cell.BlackKnight) ||
                    (board._board[rMove, cMove] == Cell.BlackBishop) ||
                    (board._board[rMove, cMove] == Cell.BlackRook) ||
                    (board._board[rMove, cMove] == Cell.BlackQueen) ||
                    (board._board[rMove, cMove] == Cell.BlackKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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
                if ((rMove < 8) && (cMove > -1) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) ||
                    (board._board[rMove, cMove] == Cell.BlackKnight) ||
                    (board._board[rMove, cMove] == Cell.BlackBishop) ||
                    (board._board[rMove, cMove] == Cell.BlackRook) ||
                    (board._board[rMove, cMove] == Cell.BlackQueen) ||
                    (board._board[rMove, cMove] == Cell.BlackKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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
                if ((rMove > -1) && (cMove > -1) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) ||
                    (board._board[rMove, cMove] == Cell.BlackKnight) ||
                    (board._board[rMove, cMove] == Cell.BlackBishop) ||
                    (board._board[rMove, cMove] == Cell.BlackRook) ||
                    (board._board[rMove, cMove] == Cell.BlackQueen) ||
                    (board._board[rMove, cMove] == Cell.BlackKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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
                if ((rMove > -1) && (cMove > -1) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) ||
                    (board._board[rMove, cMove] == Cell.BlackKnight) ||
                    (board._board[rMove, cMove] == Cell.BlackBishop) ||
                    (board._board[rMove, cMove] == Cell.BlackRook) ||
                    (board._board[rMove, cMove] == Cell.BlackQueen) ||
                    (board._board[rMove, cMove] == Cell.BlackKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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
                if ((rMove > -1) && (cMove < 8) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) ||
                    (board._board[rMove, cMove] == Cell.WhiteKnight) ||
                    (board._board[rMove, cMove] == Cell.WhiteBishop) ||
                    (board._board[rMove, cMove] == Cell.WhiteRook) ||
                    (board._board[rMove, cMove] == Cell.WhiteQueen) ||
                    (board._board[rMove, cMove] == Cell.WhiteKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
                    if (!CheckKing)
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //атака черным конем на 2 часа
                rMove = knightXY.r - 1;
                cMove = knightXY.c + 2;
                if ((rMove > -1) && (cMove < 8) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) ||
                    (board._board[rMove, cMove] == Cell.WhiteKnight) ||
                    (board._board[rMove, cMove] == Cell.WhiteBishop) ||
                    (board._board[rMove, cMove] == Cell.WhiteRook) ||
                    (board._board[rMove, cMove] == Cell.WhiteQueen) ||
                    (board._board[rMove, cMove] == Cell.WhiteKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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

                //атака черным конем на 4 часа
                rMove = knightXY.r + 1;
                cMove = knightXY.c + 2;
                if ((rMove < 8) && (cMove < 8) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) ||
                    (board._board[rMove, cMove] == Cell.WhiteKnight) ||
                    (board._board[rMove, cMove] == Cell.WhiteBishop) ||
                    (board._board[rMove, cMove] == Cell.WhiteRook) ||
                    (board._board[rMove, cMove] == Cell.WhiteQueen) ||
                    (board._board[rMove, cMove] == Cell.WhiteKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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
                if ((rMove < 8) && (cMove < 8) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) ||
                    (board._board[rMove, cMove] == Cell.WhiteKnight) ||
                    (board._board[rMove, cMove] == Cell.WhiteBishop) ||
                    (board._board[rMove, cMove] == Cell.WhiteRook) ||
                    (board._board[rMove, cMove] == Cell.WhiteQueen) ||
                    (board._board[rMove, cMove] == Cell.WhiteKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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
                if ((rMove < 8) && (cMove > -1) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) ||
                    (board._board[rMove, cMove] == Cell.WhiteKnight) ||
                    (board._board[rMove, cMove] == Cell.WhiteBishop) ||
                    (board._board[rMove, cMove] == Cell.WhiteRook) ||
                    (board._board[rMove, cMove] == Cell.WhiteQueen) ||
                    (board._board[rMove, cMove] == Cell.WhiteKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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
                if ((rMove < 8) && (cMove > -1) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) ||
                    (board._board[rMove, cMove] == Cell.WhiteKnight) ||
                    (board._board[rMove, cMove] == Cell.WhiteBishop) ||
                    (board._board[rMove, cMove] == Cell.WhiteRook) ||
                    (board._board[rMove, cMove] == Cell.WhiteQueen) ||
                    (board._board[rMove, cMove] == Cell.WhiteKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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
                if ((rMove > -1) && (cMove > -1) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) ||
                    (board._board[rMove, cMove] == Cell.WhiteKnight) ||
                    (board._board[rMove, cMove] == Cell.WhiteBishop) ||
                    (board._board[rMove, cMove] == Cell.WhiteRook) ||
                    (board._board[rMove, cMove] == Cell.WhiteQueen) ||
                    (board._board[rMove, cMove] == Cell.WhiteKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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
                if ((rMove > -1) && (cMove > -1) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) ||
                    (board._board[rMove, cMove] == Cell.WhiteKnight) ||
                    (board._board[rMove, cMove] == Cell.WhiteBishop) ||
                    (board._board[rMove, cMove] == Cell.WhiteRook) ||
                    (board._board[rMove, cMove] == Cell.WhiteQueen) ||
                    (board._board[rMove, cMove] == Cell.WhiteKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(knightXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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
            Board TempBoard;//доска на которой проверяется шах королю
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
                
                while ((rMove > -1) && (cMove < 8) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) ||
                    (board._board[rMove, cMove] == Cell.BlackKnight) ||
                    (board._board[rMove, cMove] == Cell.BlackBishop) ||
                    (board._board[rMove, cMove] == Cell.BlackRook) ||
                    (board._board[rMove, cMove] == Cell.BlackQueen) ||
                    (board._board[rMove, cMove] == Cell.BlackKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(bishopXY, rMove, cMove);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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
                    
                    //если мы наткнулись на противника то больше ходить нельзя
                    if (board._board[rMove, cMove] != Cell.Empty)
                    {
                        Move._moveType = MoveType.Taking;
                        break;
                    }

                    j++;

                    rMove = bishopXY.r - j;
                    cMove = bishopXY.c + j;
                }

                //атака белым слоном вниз-вправо
                j = 1;
                rMove = bishopXY.r + j;
                cMove = bishopXY.c + j;
                
                while ((rMove < 8) && (cMove < 8) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) ||
                    (board._board[rMove, cMove] == Cell.BlackKnight) ||
                    (board._board[rMove, cMove] == Cell.BlackBishop) ||
                    (board._board[rMove, cMove] == Cell.BlackRook) ||
                    (board._board[rMove, cMove] == Cell.BlackQueen) ||
                    (board._board[rMove, cMove] == Cell.BlackKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(bishopXY, rMove, cMove);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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

                    //если мы наткнулись на противника то больше ходить нельзя
                    if (board._board[rMove, cMove] != Cell.Empty)
                    {
                        Move._moveType = MoveType.Taking;
                        break;
                    }

                    j++;

                    rMove = bishopXY.r + j;
                    cMove = bishopXY.c + j;
                }

                //атака белым слоном вниз-влево
                j = 1;
                rMove = bishopXY.r + j;
                cMove = bishopXY.c - j;
                while ((rMove < 8) && (cMove > -1) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) ||
                    (board._board[rMove, cMove] == Cell.BlackKnight) ||
                    (board._board[rMove, cMove] == Cell.BlackBishop) ||
                    (board._board[rMove, cMove] == Cell.BlackRook) ||
                    (board._board[rMove, cMove] == Cell.BlackQueen) ||
                    (board._board[rMove, cMove] == Cell.BlackKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(bishopXY, rMove, cMove);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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

                    //если мы наткнулись на противника то больше ходить нельзя
                    if (board._board[rMove, cMove] != Cell.Empty)
                    {
                        Move._moveType = MoveType.Taking;
                        break;
                    }

                    j++;

                    rMove = bishopXY.r + j;
                    cMove = bishopXY.c - j;
                }

                //атака белым слоном вверх-влево
                j = 1;
                rMove = bishopXY.r - j;
                cMove = bishopXY.c - j;
                while ((rMove > -1) && (cMove > -1) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) ||
                    (board._board[rMove, cMove] == Cell.BlackKnight) ||
                    (board._board[rMove, cMove] == Cell.BlackBishop) ||
                    (board._board[rMove, cMove] == Cell.BlackRook) ||
                    (board._board[rMove, cMove] == Cell.BlackQueen) ||
                    (board._board[rMove, cMove] == Cell.BlackKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(bishopXY, rMove, cMove);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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

                    //если мы наткнулись на противника то больше ходить нельзя
                    if (board._board[rMove, cMove] != Cell.Empty)
                    {
                        Move._moveType = MoveType.Taking;
                        break;
                    }

                    j++;

                    rMove = bishopXY.r - j;
                    cMove = bishopXY.c - j;
                }

            }
            else//Если мы ищем атаки черного слона
            {
                //атака черным слоном на верх-право
                j = 1;
                rMove = bishopXY.r - j;
                cMove = bishopXY.c + j;
                while ((rMove > -1) && (cMove < 8) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) ||
                    (board._board[rMove, cMove] == Cell.WhiteKnight) ||
                    (board._board[rMove, cMove] == Cell.WhiteBishop) ||
                    (board._board[rMove, cMove] == Cell.WhiteRook) ||
                    (board._board[rMove, cMove] == Cell.WhiteQueen) ||
                    (board._board[rMove, cMove] == Cell.WhiteKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(bishopXY, rMove, cMove);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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

                    //если мы наткнулись на противника то больше ходить нельзя
                    if (board._board[rMove, cMove] != Cell.Empty)
                    {
                        Move._moveType = MoveType.Taking;
                        break;
                    }

                    j++;

                    rMove = bishopXY.r - j;
                    cMove = bishopXY.c + j;
                }

                //атака черным слоном вниз-вправо
                j = 1;
                rMove = bishopXY.r + j;
                cMove = bishopXY.c + j;
                while ((rMove < 8) && (cMove < 8) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) ||
                    (board._board[rMove, cMove] == Cell.WhiteKnight) ||
                    (board._board[rMove, cMove] == Cell.WhiteBishop) ||
                    (board._board[rMove, cMove] == Cell.WhiteRook) ||
                    (board._board[rMove, cMove] == Cell.WhiteQueen) ||
                    (board._board[rMove, cMove] == Cell.WhiteKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(bishopXY, rMove, cMove);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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

                    //если мы наткнулись на противника то больше ходить нельзя
                    if (board._board[rMove, cMove] != Cell.Empty)
                    {
                        Move._moveType = MoveType.Taking;
                        break;
                    }

                    j++;

                    rMove = bishopXY.r + j;
                    cMove = bishopXY.c + j;
                }

                //атака черным слоном вниз-влево
                j = 1;
                rMove = bishopXY.r + j;
                cMove = bishopXY.c - j;
                while ((rMove < 8) && (cMove > -1) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) ||
                    (board._board[rMove, cMove] == Cell.WhiteKnight) ||
                    (board._board[rMove, cMove] == Cell.WhiteBishop) ||
                    (board._board[rMove, cMove] == Cell.WhiteRook) ||
                    (board._board[rMove, cMove] == Cell.WhiteQueen) ||
                    (board._board[rMove, cMove] == Cell.WhiteKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(bishopXY, rMove, cMove);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck( TempBoard, color);
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

                    //если мы наткнулись на противника то больше ходить нельзя
                    if (board._board[rMove, cMove] != Cell.Empty)
                    {
                        Move._moveType = MoveType.Taking;
                        break;
                    }

                    j++;

                    rMove = bishopXY.r + j;
                    cMove = bishopXY.c - j;
                }

                //атака черным слоном вверх-влево
                j = 1;
                rMove = bishopXY.r - j;
                cMove = bishopXY.c - j;
                while ((rMove > -1) && (cMove > -1) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) ||
                    (board._board[rMove, cMove] == Cell.WhiteKnight) ||
                    (board._board[rMove, cMove] == Cell.WhiteBishop) ||
                    (board._board[rMove, cMove] == Cell.WhiteRook) ||
                    (board._board[rMove, cMove] == Cell.WhiteQueen) ||
                    (board._board[rMove, cMove] == Cell.WhiteKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(bishopXY, rMove, cMove);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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

                    //если мы наткнулись на противника то больше ходить нельзя
                    if (board._board[rMove, cMove] != Cell.Empty)
                    {
                        Move._moveType = MoveType.Taking;
                        break;
                    }

                    j++;

                    rMove = bishopXY.r - j;
                    cMove = bishopXY.c - j;
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
            Board TempBoard;//доска на которой проверяется шах королю
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

                while ((cMove < 8) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) ||
                    (board._board[rMove, cMove] == Cell.BlackKnight) ||
                    (board._board[rMove, cMove] == Cell.BlackBishop) ||
                    (board._board[rMove, cMove] == Cell.BlackRook) ||
                    (board._board[rMove, cMove] == Cell.BlackQueen) ||
                    (board._board[rMove, cMove] == Cell.BlackKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(rookXY, rMove, cMove);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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

                    //если мы наткнулись на противника то больше ходить нельзя
                    if (board._board[rMove, cMove] != Cell.Empty)
                    {
                        Move._moveType = MoveType.Taking;
                        break;
                    }

                    j++;

                    rMove = rookXY.r;
                    cMove = rookXY.c + j;

                }

                //атака белой ладьей вниз
                j = 1;
                rMove = rookXY.r + j;
                cMove = rookXY.c;

                while ((rookXY.r + j < 8) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) ||
                    (board._board[rMove, cMove] == Cell.BlackKnight) ||
                    (board._board[rMove, cMove] == Cell.BlackBishop) ||
                    (board._board[rMove, cMove] == Cell.BlackRook) ||
                    (board._board[rMove, cMove] == Cell.BlackQueen) ||
                    (board._board[rMove, cMove] == Cell.BlackKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(rookXY, rMove, cMove);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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

                    //если мы наткнулись на противника то больше ходить нельзя
                    if (board._board[rMove, cMove] != Cell.Empty)
                    {
                        Move._moveType = MoveType.Taking;
                        break;
                    }

                    j++;

                    rMove = rookXY.r + j;
                    cMove = rookXY.c;

                }

                //атака белой ладьей влево
                j = 1;
                rMove = rookXY.r;
                cMove = rookXY.c - j;

                while ((rookXY.c - j > -1) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) ||
                    (board._board[rMove, cMove] == Cell.BlackKnight) ||
                    (board._board[rMove, cMove] == Cell.BlackBishop) ||
                    (board._board[rMove, cMove] == Cell.BlackRook) ||
                    (board._board[rMove, cMove] == Cell.BlackQueen) ||
                    (board._board[rMove, cMove] == Cell.BlackKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(rookXY, rMove, cMove);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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

                    //если мы наткнулись на противника то больше ходить нельзя
                    if (board._board[rMove, cMove] != Cell.Empty)
                    {
                        Move._moveType = MoveType.Taking;
                        break;
                    }

                    j++;

                    rMove = rookXY.r;
                    cMove = rookXY.c - j;
                }

                //атака белой ладьей вверх
                j = 1;
                rMove = rookXY.r - j;
                cMove = rookXY.c;

                while ((rookXY.r - j > -1) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) ||
                    (board._board[rMove, cMove] == Cell.BlackKnight) ||
                    (board._board[rMove, cMove] == Cell.BlackBishop) ||
                    (board._board[rMove, cMove] == Cell.BlackRook) ||
                    (board._board[rMove, cMove] == Cell.BlackQueen) ||
                    (board._board[rMove, cMove] == Cell.BlackKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(rookXY, rMove, cMove);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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

                    //если мы наткнулись на противника то больше ходить нельзя
                    if (board._board[rMove, cMove] != Cell.Empty)
                    {
                        Move._moveType = MoveType.Taking;
                        break;
                    }

                    j++;

                    rMove = rookXY.r - j;
                    cMove = rookXY.c;
                }

            }
            else//Если мы ищем атаки черной ладьи
            {
                //атака черной ладьей вправо
                j = 1;
                rMove = rookXY.r;
                cMove = rookXY.c + j;

                while ((rookXY.c + j < 8) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) ||
                    (board._board[rMove, cMove] == Cell.WhiteKnight) ||
                    (board._board[rMove, cMove] == Cell.WhiteBishop) ||
                    (board._board[rMove, cMove] == Cell.WhiteRook) ||
                    (board._board[rMove, cMove] == Cell.WhiteQueen) ||
                    (board._board[rMove, cMove] == Cell.WhiteKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(rookXY, rMove, cMove);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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

                    //если мы наткнулись на противника то больше ходить нельзя
                    if (board._board[rMove, cMove] != Cell.Empty)
                    {
                        Move._moveType = MoveType.Taking;
                        break;
                    }

                    j++;

                    rMove = rookXY.r;
                    cMove = rookXY.c + j;
                }

                //атака черной ладьей вниз
                j = 1;
                rMove = rookXY.r + j;
                cMove = rookXY.c;

                while ((rookXY.r + j < 8) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) ||
                    (board._board[rMove, cMove] == Cell.WhiteKnight) ||
                    (board._board[rMove, cMove] == Cell.WhiteBishop) ||
                    (board._board[rMove, cMove] == Cell.WhiteRook) ||
                    (board._board[rMove, cMove] == Cell.WhiteQueen) ||
                    (board._board[rMove, cMove] == Cell.WhiteKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(rookXY, rMove, cMove);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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
                    
                    //если мы наткнулись на противника то больше ходить нельзя
                    if (board._board[rMove, cMove] != Cell.Empty)
                    {
                        Move._moveType = MoveType.Taking;
                        break;
                    }

                    j++;

                    rMove = rookXY.r + j;
                    cMove = rookXY.c;
                }

                //атака черной ладьей влево
                j = 1;
                rMove = rookXY.r;
                cMove = rookXY.c - j;

                while ((rookXY.c - j > -1) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) ||
                    (board._board[rMove, cMove] == Cell.WhiteKnight) ||
                    (board._board[rMove, cMove] == Cell.WhiteBishop) ||
                    (board._board[rMove, cMove] == Cell.WhiteRook) ||
                    (board._board[rMove, cMove] == Cell.WhiteQueen) ||
                    (board._board[rMove, cMove] == Cell.WhiteKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(rookXY, rMove, cMove);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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

                    //если мы наткнулись на противника то больше ходить нельзя
                    if (board._board[rMove, cMove] != Cell.Empty)
                    {
                        Move._moveType = MoveType.Taking;
                        break;
                    }

                    j++;

                    rMove = rookXY.r;
                    cMove = rookXY.c - j;
                }

                //атака черной ладьей вправо
                j = 1;
                rMove = rookXY.r - j;
                cMove = rookXY.c;

                while ((rookXY.r - j > -1) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) ||
                    (board._board[rMove, cMove] == Cell.WhiteKnight) ||
                    (board._board[rMove, cMove] == Cell.WhiteBishop) ||
                    (board._board[rMove, cMove] == Cell.WhiteRook) ||
                    (board._board[rMove, cMove] == Cell.WhiteQueen) ||
                    (board._board[rMove, cMove] == Cell.WhiteKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(rookXY, rMove, cMove);

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck( TempBoard, color);
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

                    //если мы наткнулись на противника то больше ходить нельзя
                    if (board._board[rMove, cMove] != Cell.Empty)
                    {
                        Move._moveType = MoveType.Taking;
                        break;
                    }

                    j++;

                    rMove = rookXY.r - j;
                    cMove = rookXY.c;
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
            if ((Temp2 != null) && (Temp2.Length != 0))
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
                        Moves[i] = Temp2[i - Count];
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
            if ((Temp2 != null) && (Temp2.Length != 0))
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
                        Moves[i] = Temp2[i - Count];
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
            Board TempBoard;//доска на которой проверяется шах королю
            int Count = 0; //количество найденных ходов пешки

            int rMove = -1;//Координаты искомого хода, строка
            int cMove = -1;//Координаты искомого хода, солбец

            if (color == Color.White)//ищем ходы белого короля
            {
                //ход белого короля на одну клетку вправо
                rMove = kingXY.r;
                cMove = kingXY.c + 1;
                if ((cMove < 8) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) ||
                    (board._board[rMove, cMove] == Cell.BlackKnight) ||
                    (board._board[rMove, cMove] == Cell.BlackBishop) ||
                    (board._board[rMove, cMove] == Cell.BlackRook) ||
                    (board._board[rMove, cMove] == Cell.BlackQueen) ||
                    (board._board[rMove, cMove] == Cell.BlackKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
                    if (!CheckKing)
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //ход белого короля на одну клетку вниз
                rMove = kingXY.r + 1;
                cMove = kingXY.c;


                if ((rMove < 8) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) ||
                    (board._board[rMove, cMove] == Cell.BlackKnight) ||
                    (board._board[rMove, cMove] == Cell.BlackBishop) ||
                    (board._board[rMove, cMove] == Cell.BlackRook) ||
                    (board._board[rMove, cMove] == Cell.BlackQueen) ||
                    (board._board[rMove, cMove] == Cell.BlackKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck( TempBoard, color);
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

                if ((cMove > -1) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) ||
                    (board._board[rMove, cMove] == Cell.BlackKnight) ||
                    (board._board[rMove, cMove] == Cell.BlackBishop) ||
                    (board._board[rMove, cMove] == Cell.BlackRook) ||
                    (board._board[rMove, cMove] == Cell.BlackQueen) ||
                    (board._board[rMove, cMove] == Cell.BlackKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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

                if ((rMove > -1) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) ||
                    (board._board[rMove, cMove] == Cell.BlackKnight) ||
                    (board._board[rMove, cMove] == Cell.BlackBishop) ||
                    (board._board[rMove, cMove] == Cell.BlackRook) ||
                    (board._board[rMove, cMove] == Cell.BlackQueen) ||
                    (board._board[rMove, cMove] == Cell.BlackKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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

                if ((rMove > -1) && (cMove < 8) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) ||
                    (board._board[rMove, cMove] == Cell.BlackKnight) ||
                    (board._board[rMove, cMove] == Cell.BlackBishop) ||
                    (board._board[rMove, cMove] == Cell.BlackRook) ||
                    (board._board[rMove, cMove] == Cell.BlackQueen) ||
                    (board._board[rMove, cMove] == Cell.BlackKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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

                if ((rMove < 8) && (cMove < 8) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) ||
                    (board._board[rMove, cMove] == Cell.BlackKnight) ||
                    (board._board[rMove, cMove] == Cell.BlackBishop) ||
                    (board._board[rMove, cMove] == Cell.BlackRook) ||
                    (board._board[rMove, cMove] == Cell.BlackQueen) ||
                    (board._board[rMove, cMove] == Cell.BlackKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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

                if ((rMove < 8) && (cMove > -1) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) ||
                    (board._board[rMove, cMove] == Cell.BlackKnight) ||
                    (board._board[rMove, cMove] == Cell.BlackBishop) ||
                    (board._board[rMove, cMove] == Cell.BlackRook) ||
                    (board._board[rMove, cMove] == Cell.BlackQueen) ||
                    (board._board[rMove, cMove] == Cell.BlackKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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

                if ((rMove > -1) && (cMove > -1) &&
                    ((board._board[rMove, cMove] == Cell.BlackPawn) ||
                    (board._board[rMove, cMove] == Cell.BlackKnight) ||
                    (board._board[rMove, cMove] == Cell.BlackBishop) ||
                    (board._board[rMove, cMove] == Cell.BlackRook) ||
                    (board._board[rMove, cMove] == Cell.BlackQueen) ||
                    (board._board[rMove, cMove] == Cell.BlackKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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
                    (!CheckCheck(board, color)) &&//король не попадает под шах нигде на протяжении рокировки
                    (!CheckCheck(board, color, new XY(7, 2))) &&
                    (!CheckCheck(board, color, new XY(7, 3))))
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
                    (!CheckCheck(board, color)) &&//король не попадает под шах нигде на протяжении рокировки
                    (!CheckCheck(board, color, new XY(7, 5))) &&
                    (!CheckCheck(board, color, new XY(7, 6))))
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
                if ((cMove < 8) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) ||
                    (board._board[rMove, cMove] == Cell.WhiteKnight) ||
                    (board._board[rMove, cMove] == Cell.WhiteBishop) ||
                    (board._board[rMove, cMove] == Cell.WhiteRook) ||
                    (board._board[rMove, cMove] == Cell.WhiteQueen) ||
                    (board._board[rMove, cMove] == Cell.WhiteKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
                    if (!CheckKing)
                    {
                        Moves = new Move[] { Move };
                        Count = 1;
                    }
                }

                //ход черного короля на одну клетку вниз
                rMove = kingXY.r + 1;
                cMove = kingXY.c;

                if ((rMove < 8) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) ||
                    (board._board[rMove, cMove] == Cell.WhiteKnight) ||
                    (board._board[rMove, cMove] == Cell.WhiteBishop) ||
                    (board._board[rMove, cMove] == Cell.WhiteRook) ||
                    (board._board[rMove, cMove] == Cell.WhiteQueen) ||
                    (board._board[rMove, cMove] == Cell.WhiteKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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

                if ((cMove > -1) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) ||
                    (board._board[rMove, cMove] == Cell.WhiteKnight) ||
                    (board._board[rMove, cMove] == Cell.WhiteBishop) ||
                    (board._board[rMove, cMove] == Cell.WhiteRook) ||
                    (board._board[rMove, cMove] == Cell.WhiteQueen) ||
                    (board._board[rMove, cMove] == Cell.WhiteKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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

                if ((rMove > -1) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) ||
                    (board._board[rMove, cMove] == Cell.WhiteKnight) ||
                    (board._board[rMove, cMove] == Cell.WhiteBishop) ||
                    (board._board[rMove, cMove] == Cell.WhiteRook) ||
                    (board._board[rMove, cMove] == Cell.WhiteQueen) ||
                    (board._board[rMove, cMove] == Cell.WhiteKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck( TempBoard, color);
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

                if ((rMove > -1) && (cMove < 8) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) ||
                    (board._board[rMove, cMove] == Cell.WhiteKnight) ||
                    (board._board[rMove, cMove] == Cell.WhiteBishop) ||
                    (board._board[rMove, cMove] == Cell.WhiteRook) ||
                    (board._board[rMove, cMove] == Cell.WhiteQueen) ||
                    (board._board[rMove, cMove] == Cell.WhiteKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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

                if ((rMove < 8) && (cMove < 8) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) ||
                    (board._board[rMove, cMove] == Cell.WhiteKnight) ||
                    (board._board[rMove, cMove] == Cell.WhiteBishop) ||
                    (board._board[rMove, cMove] == Cell.WhiteRook) ||
                    (board._board[rMove, cMove] == Cell.WhiteQueen) ||
                    (board._board[rMove, cMove] == Cell.WhiteKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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

                if ((rMove < 8) && (cMove > -1) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) ||
                    (board._board[rMove, cMove] == Cell.WhiteKnight) ||
                    (board._board[rMove, cMove] == Cell.WhiteBishop) ||
                    (board._board[rMove, cMove] == Cell.WhiteRook) ||
                    (board._board[rMove, cMove] == Cell.WhiteQueen) ||
                    (board._board[rMove, cMove] == Cell.WhiteKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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

                if ((rMove > -1) && (cMove > -1) &&
                    ((board._board[rMove, cMove] == Cell.WhitePawn) ||
                    (board._board[rMove, cMove] == Cell.WhiteKnight) ||
                    (board._board[rMove, cMove] == Cell.WhiteBishop) ||
                    (board._board[rMove, cMove] == Cell.WhiteRook) ||
                    (board._board[rMove, cMove] == Cell.WhiteQueen) ||
                    (board._board[rMove, cMove] == Cell.WhiteKing) ||
                    (board._board[rMove, cMove] == Cell.Empty)))
                {
                    Move = new Move(kingXY, rMove, cMove);
                    if (board._board[rMove, cMove] != Cell.Empty) { Move._moveType = MoveType.Taking; }

                    //проверка этого хода на шах королю (может этот ход открывает короля для атаки)
                    TempBoard = Board.DoMove(board, Move);//делаем ход
                    CheckKing = CheckCheck(TempBoard, color);
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
                    (!CheckCheck( board, color)) &&//король не попадает под шах нигде на протяжении рокировки
                    (!CheckCheck(board, color, new XY(0, 2))) &&
                    (!CheckCheck(board, color, new XY(0, 3))))
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
                    (!CheckCheck( board, color)) &&//король не попадает под шах нигде на протяжении рокировки
                    (!CheckCheck(board, color, new XY(0, 5))) &&
                    (!CheckCheck(board, color, new XY(0, 6))))
                {
                    Move = new Move(kingXY, 0, 6);
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

        /// <summary>
        /// Поиск всех ходов, которые может совершить игрок, который ходит следующим
        /// </summary>
        static public Move[] FindNextColorMoves(Board board)
        {
            Color color = board._nextColor;
            XY[] OurFiguresXY;//массив наших фигур
            FiguresXY enemyFigures;//структура для хранения всех фигур противника
            Move enemyLastMove;
            XY kingXY;

            if (color == Color.White)
            {
                OurFiguresXY = board._WhiteFigures.Figures;
                enemyFigures = board._BlackFigures;
                enemyLastMove = board._LastMove;
                kingXY = board._WhiteFigures.King;
            }
            else
            {
                OurFiguresXY = board._BlackFigures.Figures;
                enemyFigures = board._WhiteFigures;
                enemyLastMove = board._LastMove;
                kingXY = board._BlackFigures.King;
            }

            Move[] Out = new Move[0];//массив для хранени всех ходов
            Move[] Moves = new Move[0];//массив для хранени всех ходов данной фигуры

            int Length = OurFiguresXY.Length;
            for (int j = 0; j < Length; j++)
            {
                int rFig = OurFiguresXY[j].r;
                int cFig = OurFiguresXY[j].c;

                switch (board._board[rFig, cFig])//клетка с которой начинается ход
                {
                    case (Cell.BlackPawn):
                    case (Cell.WhitePawn):
                        Moves = FindPawnMoves(OurFiguresXY[j], board, color, enemyLastMove, kingXY, enemyFigures);
                        break;
                    case (Cell.BlackKnight):
                    case (Cell.WhiteKnight):
                        Moves = FindKnightMoves(OurFiguresXY[j], board, color, kingXY, enemyFigures);
                        break;
                    case (Cell.BlackBishop):
                    case (Cell.WhiteBishop):
                        Moves = FindBishopMoves(OurFiguresXY[j], board, color, kingXY, enemyFigures);
                        break;
                    case (Cell.BlackRook):
                    case (Cell.WhiteRook):
                        Moves = FindRookMoves(OurFiguresXY[j], board, color, kingXY, enemyFigures);
                        break;
                    case (Cell.BlackQueen):
                    case (Cell.WhiteQueen):
                        Moves = FindQueenMoves(OurFiguresXY[j], board, color, kingXY, enemyFigures);
                        break;
                    case (Cell.BlackKing):
                    case (Cell.WhiteKing):
                        Moves = FindKingMoves(OurFiguresXY[j], board, color, enemyFigures);
                        break;
                    default:
                        return null;
                }

                Out = Move.Add(Out, Moves);
            }

            return Out;
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
                if (((cellXY.r == pawnXY.r - 1) && (cellXY.c == pawnXY.c - 1)) ||
                    ((cellXY.r == pawnXY.r - 1) && (cellXY.c == pawnXY.c + 1)))
                {
                    return true;
                }
            }
            else//Если мы ищем атаки черной пешки
            {
                if (((cellXY.r == pawnXY.r + 1) && (cellXY.c == pawnXY.c - 1)) ||
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

            if (((cellXY.r == knightXY.r - 2) && (cellXY.c == knightXY.c + 1)) ||
                ((cellXY.r == knightXY.r - 1) && (cellXY.c == knightXY.c + 2)) ||
                ((cellXY.r == knightXY.r + 1) && (cellXY.c == knightXY.c + 2)) ||
                ((cellXY.r == knightXY.r + 2) && (cellXY.c == knightXY.c + 1)) ||
                ((cellXY.r == knightXY.r + 2) && (cellXY.c == knightXY.c - 1)) ||
                ((cellXY.r == knightXY.r + 1) && (cellXY.c == knightXY.c - 2)) ||
                ((cellXY.r == knightXY.r - 1) && (cellXY.c == knightXY.c - 2)) ||
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
            if ((cellXY.r - bishopXY.r == 0) || (cellXY.c - bishopXY.c == 0))//чтоб не было дедения на 0
            {
                return false;
            }
            //для этого нужно убедиться что следующий параметр равен единице
            float Ratio = ((float)Math.Abs(cellXY.r - bishopXY.r) / Math.Abs(cellXY.c - bishopXY.c));
            if (Ratio != 1) 
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

            return BishopAtacks || RookAtacks;
        }

        /// <summary>
        /// Атакует ли король заданную клетку?
        /// </summary>
        static bool CheckKingAtacks(XY kingXY, XY cellXY)
        {
            //Ответ на вопрос, находится ли заданная клетка под атакой конкретного короля

            if (((cellXY.r == kingXY.r) && (cellXY.c == kingXY.c + 1)) ||
                ((cellXY.r == kingXY.r + 1) && (cellXY.c == kingXY.c)) ||
                ((cellXY.r == kingXY.r) && (cellXY.c == kingXY.c - 1)) ||
                ((cellXY.r == kingXY.r - 1) && (cellXY.c == kingXY.c)) ||
                ((cellXY.r == kingXY.r - 1) && (cellXY.c == kingXY.c + 1)) ||
                ((cellXY.r == kingXY.r + 1) && (cellXY.c == kingXY.c + 1)) ||
                ((cellXY.r == kingXY.r + 1) && (cellXY.c == kingXY.c - 1)) ||
                ((cellXY.r == kingXY.r - 1) && (cellXY.c == kingXY.c - 1)))
            {
                return true;
            }
            else { return false; }
        }

        /// <summary>
        /// Проверка доски на шах королю
        /// </summary>
        public static bool CheckCheck(Board board, Color kingColor, XY kingXY = null)
        {
            if ( (kingXY == null) && (kingColor == Color.White) )
            { kingXY = board._WhiteFigures.King; }
            else if (kingXY == null)
            { kingXY = board._BlackFigures.King; }

            FiguresXY enemyFigures;
            
            if (kingColor == Color.White)
            {enemyFigures = board._BlackFigures;}
            else
            {enemyFigures = board._WhiteFigures;}

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
                        case (Cell.WhitePawn):
                            TempFlag = CheckPawnAtacks(enemyXY, (Color)0, kingXY);
                            if (TempFlag) { return TempFlag; }
                            break;
                        case (Cell.WhiteKnight):
                            TempFlag = CheckKnightAtacks(enemyXY, kingXY);
                            if (TempFlag) { return TempFlag; }
                            break;
                        case (Cell.WhiteBishop):
                            TempFlag = CheckBishopAtacks(enemyXY, board, kingXY);
                            if (TempFlag) { return TempFlag; }
                            break;
                        case (Cell.WhiteRook):
                            TempFlag = CheckRookAtacks(enemyXY, board, kingXY);
                            if (TempFlag) { return TempFlag; }
                            break;
                        case (Cell.WhiteQueen):
                            TempFlag = CheckQueenAtacks(enemyXY, board, kingXY);
                            if (TempFlag) { return TempFlag; }
                            break;
                        case (Cell.WhiteKing):
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
        /// Проверка доски на мат королю
        /// </summary>
        public static bool CheckMate(Board board)
        {
            XY kingXY;
            FiguresXY Figures;
            FiguresXY enemyFigures;
            Move enemyLastMove = board.LastMove;
            Color kingColor = board.NextColor;

            if (kingColor == Color.White)
            {
                kingXY = board._WhiteFigures.King;
                Figures = board._WhiteFigures;
                enemyFigures = board._BlackFigures;
            }
            else
            {
                kingXY = board._BlackFigures.King;
                Figures = board._BlackFigures;
                enemyFigures = board._WhiteFigures;
            }

            int FiguresCount = Figures.Figures.Length;//Количество наших фигур
            Move[] FoundMoves = null;//массив доступных ходов нашей фигуры

            for (int i = 0; i < FiguresCount; i++)
            {
                int rFig = Figures.Figures[i].r;
                int cFig = Figures.Figures[i].c;
                XY FigXY = Figures.Figures[i];// координаты фигуры, которую мы сейчас проверяем

                if (kingColor == Color.White)//ищем доступные ходы белых фигур
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
                else//ищем доступные ходы черных фигур
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
                if (FoundMoves.Length != 0)//сужествует хоть один ход, доступный фигуре игрока
                {
                    return false;
                }
            }

            return true;//обойдя все фигуры не нашлось фигуры у которой был бы возможный ход
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

    #region второстепенные классы
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
        private XY[] _move = null;//массив в котором лежат координаты начала и конца хода
        public XY XY1//Свойства чтобы можно было смотреть позиции начала и конца хода
        {
            get { return _move[0]; }
            set { _move[0] = value; }
        }
        public XY XY2
        {
            get { return _move[1]; }
            set { _move[1] = value; }
        }

        public MoveType _moveType = MoveType.Moving;//тип хода

        public Move(XY xy1, int x2, int y2, MoveType type = MoveType.Moving)//конструктор из координат хода
        {
            XY xy2 = new XY(x2, y2);
            this._move = new XY[2] { xy1, xy2 };
            this._moveType = type;
        }

        public Move(XY xy1, XY xy2, MoveType type = MoveType.Moving)//конструктор из координат XY
        {
            this._move = new XY[2] { xy1, xy2 };
            this._moveType = type;
        }

        public Move(Move a)//конструктор копирования
        {
            if ((a != null) && (a._move !=  null))
            {
                this._move = new XY[] { a._move[0], a._move[1] };
                this._moveType = a._moveType;
            }
        }

        static public Move[] Add(Move[] a, Move b)//добавление хода в массив ходов
        {
            int Length = a.Length;
            Move[] Out = new Move[Length + 1];

            for (int i = 0; i < Length; i++)
            {
                Out[i] = a[i];
            }
            Out[Length] = b;

            return Out;
        }

        static public Move[] Add(Move[] a, Move[] b)//добавление массива ходов в массив ходов
        {
            int LengthA = a.Length;
            int LengthB = b.Length;
            Move[] Out = new Move[LengthA + LengthB];

            for (int i = 0; i < LengthA; i++)
            {
                Out[i] = a[i];
            }
            for (int i = LengthA; i < LengthA + LengthB; i++)
            {
                Out[i] = b[i - LengthA];
            }
            
            return Out;
        }
    }

    public class FiguresXY //содержит координаты всех фигур игрока кроме короля
    {
        private XY[] _figures;//массив координат
        public XY[] Figures//Свойство чтобы можно было использовать массив в циклах
        {
            get { return _figures; }
            set { _figures = value; }
        }
        private XY _king;//координаты короля
        public XY King//Свойство чтобы можно было использовать массив в циклах
        {
            get { return _king; }
            set { _king = value; }
        }

        public FiguresXY(XY[] figures, XY king)//конструктор из массива
        {
            _figures = figures;
            _king = king;
        }

        public FiguresXY(FiguresXY a)//конструктор копирования
        {
            int Length = a._figures.Length;
            this._figures = new XY[Length];
            for (int i = 0; i < Length; i++)
            {
                this._figures[i] = new XY(a._figures[i]);
            }
            this._king = a._king;
        }

        public void DeleteXY(XY deleteXY)//удаление фигуры из массива
        {
            int count = this.Figures.Length;
            XY[] FiguresCuted = new XY[count - 1];

            int j = 0;
            for (int i = 0; i < count; i++)
            {
                if ((this.Figures[i].r == deleteXY.r) && (this.Figures[i].c == deleteXY.c))
                { continue; }

                FiguresCuted[j] = new XY(this.Figures[i]);
                j++;
            }
            this.Figures = FiguresCuted;
        }

        public void RewriteXY(XY oldXY, XY newXY)//изменение координат фигуры
        {
            int count = this.Figures.Length;

            for (int i = 0; i < count; i++)
            {
                if ((this.Figures[i].r == oldXY.r) && (this.Figures[i].c == oldXY.c))
                {
                    this.Figures[i].r = newXY.r;
                    this.Figures[i].c = newXY.c;
                    break;
                }
            }
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
    #endregion
}



