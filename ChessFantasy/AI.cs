﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessFantasy
{
    public static class AI
    {
        /// <summary>
        /// основной метод рассчета ИИ
        /// </summary>
        static public Move Processing(Board board)
        {
            //1 создать корень дерева
            Three Root = new Three(board);

            //2 создавать потомков до тех пор, пока не найдется лучший ход
            int Length = Root.MovesAvailable.Length;

            if (Length == 0) { return null; }//если ходов не найдено, значит мат и нечего тут искать

            Root.Children = new Three[Length];
            for (int i = 0; i < Length; i++)//потомки 1-го поколения, ходы ИИ
            {
                Root.Children[i] = new Three(Root, i);
                if ((Root.Children == null) && (Three.MoveFound == true))//видимо мат в один ход найден
                { return Root.Move; }
            }

            if ((Root.Children == null) && (Three.MoveFound == true))//найден ход
            { return Root.Move; }

            for (int i = 0; i < Length; i++)//потомки 1-го поколения, ходы ИИ
            {
                if (Root.ValueMax == Root.Children[i].ValueMin)//видимо мат в один ход найден
                { return Root.Children[i].Move; }
            }

            return null;//хз пока
        }

        /// <summary>
        /// оценка состояния доски
        /// </summary>
        static public int Evaluate(Board board)
        {
            int value = 0;

            //Сначала пересчитаем фигуры
            for (int i = 0; i < board.BlackFigures.Figures.Length; i++)
            {
                int rFig = board.BlackFigures.Figures[i].r;
                int cFig = board.BlackFigures.Figures[i].c;
                switch (board.BoardArr[rFig, cFig])//клетка с которой начинается ход
                {
                    case (Cell.BlackPawn):
                        value += 10;
                        break;
                    case (Cell.BlackKnight):
                        value += 30;
                        break;
                    case (Cell.BlackBishop):
                        value += 35;
                        break;
                    case (Cell.BlackRook):
                        value += 50;
                        break;
                    case (Cell.BlackQueen):
                        value += 95;
                        break;
                    case (Cell.BlackKing):
                        break;
                    default:
                        break;
                }
            }
            for (int i = 0; i < board.WhiteFigures.Figures.Length; i++)
            {
                int rFig = board.WhiteFigures.Figures[i].r;
                int cFig = board.WhiteFigures.Figures[i].c;
                switch (board.BoardArr[rFig, cFig])//клетка с которой начинается ход
                {
                    case (Cell.WhitePawn):
                        value -= 10;
                        break;
                    case (Cell.WhiteKnight):
                        value -= 30;
                        break;
                    case (Cell.WhiteBishop):
                        value -= 35;
                        break;
                    case (Cell.WhiteRook):
                        value -= 50;
                        break;
                    case (Cell.WhiteQueen):
                        value -= 95;
                        break;
                    case (Cell.WhiteKing):
                        break;
                    default:
                        break;
                }
            }

            //Потом пересчитаем возможные ходы, типа развитие фигур
            Move[] Temp;
            if (board.NextColor == Color.White)
            {
                Temp = Board.FindNextColorMoves(board);
                value -= Temp.Length / 2;

                Temp = Board.FindNextColorMoves(board.InvertColor());
                value += Temp.Length / 2;
            }
            else
            {
                Temp = Board.FindNextColorMoves(board);
                value += Temp.Length / 4;

                Temp = Board.FindNextColorMoves(board.InvertColor());
                value -= Temp.Length / 4;
            }
            board.InvertColor();

            //Добавим право на рокировку
            if (board.BLRogueAvailable) { value += 3; }
            if (board.BRRogueAvailable) { value += 3; }
            if (board.WLRogueAvailable) { value -= 3; }
            if (board.WRRogueAvailable) { value -= 3; }

            return value;
        }

    }

    public class Three //содержит дерево возможных ходов
    {
        #region свойства
        static private int _countThree = 1;//Количетсво созданных элементов
        static private bool _moveFound = false;//флаг о том что нужный ход найден и ничего боьше считать не нужно
        static public bool MoveFound
        {
            get { return _moveFound; }
            //set { _figures = value; }
        }

        static private int _maxDepth = 4;//Максимальная глубина на которую нужно считать
        static public int MaxDepth
        {
            get { return _maxDepth; }
            //set { _figures = value; }
        }

        private Three _parent = null;//родительский элемент
        public Three Parent
        {
            get { return _parent; }
            //set { _figures = value; }
        }

        private Three[] _children = new Three[0];//дочерние элементы
        public Three[] Children
        {
            get { return _children; }
            set { _children = value; }
        }

        private Board _boardMoved;//доска с произведенным ходом
        public Board BoardMoved
        {
            get { return _boardMoved; }
            //set { _figures = value; }
        }

        private Move _move;//ход, который был сделан
        public Move Move
        {
            get { return _move; }
            //set { _figures = value; }
        }

        private int _valueMax = -1000000;//максимальное значение состояния доски на которое рассчитывает наш оппонент, выше мы его не поднимем
        public int ValueMax
        {
            get { return _valueMax; }
            //set { _figures = value; }
        }

        private int _valueMin = 1000000;//минимальное значение состояния доски на которое мы можем рассчитывать. Ниже противник его не опустит
        public int ValueMin
        {
            get { return _valueMin; }
            //set { _figures = value; }
        }

        private int _depth;//глубина текущего узла
        public int Depth
        {
            get { return _depth; }
            //set { _figures = value; }
        }

        private Move[] _movesAvailable = null;//все доступные ходы
        public Move[] MovesAvailable
        {
            get { return _movesAvailable; }
            //set { _figures = value; }
        }
        #endregion

        /// <summary>
        /// базовый конструктор, создающий корень дерева
        /// </summary>
        public Three(Board board)
        {
            Three._countThree = 1;
            //this._parent = null;
            //this._children = new Three[0];
            this._boardMoved = board;
            //this._move = null;
            //this._valueMax = -1000;
            //this._valueMin = 1000;
            //int _depth = 0;
            this._movesAvailable = Board.FindNextColorMoves(board);

        }

        /// <summary>
        /// конструктор наследования/углубления
        /// реализует один из возможных ходов родительской доски
        /// </summary>
        public Three(Three a, int moveNumber)
        {
            _countThree++;
            this._parent = a;//породивший узел называется родителем

            Move[] MovesAvailable = a._movesAvailable;
            if ((MovesAvailable != null) && (MovesAvailable.Length > moveNumber))
            {
                this._boardMoved = Board.DoMove(a._boardMoved, MovesAvailable[moveNumber])  ;
            }

            this._move = MovesAvailable[moveNumber];

            //this._valueMax = -1000;
            //this._valueMin = 1000;

            this._depth = a._depth + 1;

            this._movesAvailable = Board.FindNextColorMoves(this._boardMoved);//найти все возможные ходы из этой доски

            int Length = this.MovesAvailable.Length;//количество ходов из этого узла

            if (Length == 0)//если ходов не найдено, значит мат, но кому?
            {
                if (_depth % 2 == 0)//нам мат или пат, энивей, проморгали проигрыш/ничью
                {
                    //нужно предыдущий ход противника пометить как неизбежно ведущий к мату и удалить всех его потомков
                    //потому что если у противника есть возможность поставить нам мат, он ей воспользуется
                    a._valueMin = -1000000;//типа минус бесконечность, потом поменяю если что
                    a._children = null;
                }
                else //мы поставили мат
                {
                    bool CheckCheck = Board.CheckCheck(this._boardMoved, this._boardMoved.NextColor);//проверим ход на шах

                    if (CheckCheck)//если это правда мат
                    {
                        if (_depth == 1)//есть доступный мат в один ход, ничего считать больше не нужно
                        {
                            a._move = this._move;
                            Three._moveFound = true;
                            a._children = null;
                            return;//ничего считать больше не нужно
                        }
                        else//у нас есть доступный мат после вражеского хода. нужно просто пометить ход как потенциально выигрышный
                        {
                            a._valueMax = 1000000;//типа бесконечность, потом поменяю если что
                                                  //противник знает что этот ход приведет к нашей победе
                            a._children = null;
                        }
                    }
                    else//если это просто пат
                    {
                        this._valueMin = -10000;//стоимость пата, но мы его не хотим, мы хотим раздавить противника. Кожанные мешки...биип..
                    }
                }
            }
            else //доступные ходы есть теперь нужно узнать, достаточно ли мы глубоко чтобы начать оценивать состояние досок
            {
                if (this._depth < _maxDepth - 1)//если мы еще не достигли нужной глубины нужно найти все возможные ходы этой доски
                {
                    this.Children = new Three[Length];
                    for (int i = 0; i < Length; i++)
                    {
                        this.Children[i] = new Three(this, i);//он может обнулить массив children  в родительском объекте
                        if (this.Children == null) { break; }
                    }

                    this.Children = null;
                }
                else//мы достигли нужной глубины
                //узел в котором мы находимся имеет глубину на единицу меньше максимальной глубины
                {
                    //мы лишь оценим состояние всех возможных ходов и скажем на что можно рассчитывать родительскому узлу
                    Board TempBoard;
                    int value;//для того чтоб хранить мгновенное состояние доски в цикле:
                    for (int i = 0; i < Length; i++)//нужно оценить состояние всех ходов
                    {
                        TempBoard = Board.DoMove(this._boardMoved, this._movesAvailable[i]);
                        value = AI.Evaluate(this._boardMoved);//оценка состояния доски

                        if ((this._depth + 1) % 2 == 0)//в цикле считаются ходы противника
                        {
                            if (this._valueMin > value)//найти минимальный ход который противник может совершить после моего хода
                            {
                                this._valueMin = value;
                                if (this._parent._valueMax > value)
                                { break; }//альфабета отсечение
                            }
                        }
                        else //в цикле считаются наши ходы
                        {
                            if (this._valueMax < value)//найти максимальный ход который я могу совершить после хода противника
                            {
                                this._valueMax = value;
                                if (this._parent._valueMin < value)
                                { break; }//альфабета отсечение
                            }
                        }
                    }

                    if ((this._depth + 1) % 2 == 0)//альфабета отсечение больших веток
                    {
                        if (a._parent._valueMin < this._valueMin)
                        { this._parent._children = null; }//альфабета отсечение
                    }
                    else
                    {
                        if (a._parent._valueMax > this._valueMax)
                        { this._parent._children = null; }//альфабета отсечение
                    }

                }
            }

            if ((this._depth + 1) % 2 == 0)//теперь нужно передать значение выше
            {
                if (a._valueMax < this._valueMin)//найти минимальный ход который противник может совершить после моего хода
                {
                    a._valueMax = this._valueMin;
                    if (this._depth > 1) 
                    {
                        a._parent.UpdateValue(this._valueMin);
                    }
                }
            }
            else
            {
                if (a._valueMin > this._valueMax)//найти максимальный ход который я могу совершить после хода противника
                {
                    a._valueMin = this._valueMax;
                    if (this._depth > 1)
                    {
                        a._parent.UpdateValue(this._valueMax);
                    }
                }
            }

        }

        /// <summary>
        /// добавляет в поле _children нового потомка
        /// </summary>
        public void AddChild(Three a)//Добавление наследника узлу дерева
        {
            int Length = this._children.Length;
            Three[] Children = new Three[Length + 1];

            for (int i = 0; i < Length; i++)
            {
                Children[i] = this._children[i];
            }
            Children[Length] = a;
            this._children = Children;
        }

        /// <summary>
        /// Обновляет значение value в зависимости от глубины
        /// </summary>
        public void UpdateValue(int value)
        {
            if ((this._depth) % 2 == 1)
            {
                this._valueMin = value;
            }
            else
            {
                this._valueMax = value;
            }

            if (this._children != null)//если есть потомки у которых нужно смотреть
            {
                if ((this._depth) % 2 == 1)
                {
                    for (int i = 0; i < this._children.Length; i++)
                    {
                        if ( (this._children[i] != null) && (this._valueMin > this._children[i]._valueMax) )
                        { this._valueMin = this._children[i]._valueMax; }
                    }
                }
                else
                {
                    for (int i = 0; i < this._children.Length; i++)
                    {
                        if ((this._children[i] != null) && (this._valueMax < this._children[i]._valueMin))
                        { this._valueMax = this._children[i]._valueMin; }
                    }
                }
            }
        }

    }

    
}
