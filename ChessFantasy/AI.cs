using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessFantasy
{
    public static class AI
    {
        static Random _rnd = new Random(0);//чтоб одинаковые числа получать и тестить вначале

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
            int value = _rnd.Next(-100,100);
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

        private int _valueMax = -1000;//максимальное значение состояния доски на которое рассчитывает наш оппонент, выше мы его не поднимем
        public int ValueMax
        {
            get { return _valueMax; }
            //set { _figures = value; }
        }

        private int _valueMin = 1000;//минимальное значение состояния доски на которое мы можем рассчитывать. Ниже противник его не опустит
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
                if (_depth % 2 == 0)//нам мат
                {
                    //нужно предыдущий ход противника пометить как неизбежно ведущий к мату и удалить всех его потомков
                    //потому что если у противника есть возможность поставить нам мат, он ей воспользуется
                    a._valueMin = -1000000;//типа минус бесконечность, потом поменяю если что
                    a._children = null;
                }
                else //мы поставили мат
                {
                    if (_depth == 1)//есть доступный мат в один ход, ничего считать больше не нужно
                    {
                        a._move = this._move;
                        Three._moveFound = true;
                        a._children = null;
                    }
                    else//у нас есть доступный мат после вражеского хода. нужно просто пометить ход как потенциально выигрышный
                    {
                        a._valueMax = 1000000;//типа бесконечность, потом поменяю если что
                        //противник знает что этот ход приведет к нашей победе
                        a._children = null;
                    }

                }
                return;
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
                //узел который сейчас создается имеет глубину на единицу меньше максимальной глубины
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
                            if (this._valueMin > value) { this._valueMin = value; }//найти минимальный ход который противник может совершить после моего хода
                        }
                        else //в цикле считаются наши ходы
                        {
                            if (this._valueMax < value) { this._valueMax = value; }//найти максимальный ход который я могу совершить после хода противника
                        }
                    }

                    //TODO проверять на альфабета отсечение внутри цикла
                }
            }

            if ((this._depth) % 2 == 1)//теперь нужно передать значение выше
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
