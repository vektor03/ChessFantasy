using System;
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
        static public void Processing()
        {

        }

    }

    public class Three //содержит дерево возможных ходов
    {
        #region свойства
        static private int _maxDepth = 4;//Максимальная глубина на которую нужно считать
        public int MaxDepth
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
            //set { _figures = value; }
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

        private int _valueMax = -1000;//максимальное значение состояния доски в конце рассчета на которое можно рассчитывать
        public int ValueMax
        {
            get { return _valueMax; }
            //set { _figures = value; }
        }

        private int _valueMin = 1000;//минимальное значение состояния доски в конце рассчета на которое можно рассчитывать
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
        public Move[] MmovesAvailable
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
            _boardMoved = board;
            //this._move = null;
            //this._valueMax = -1000;
            //this._valueMin = 1000;
            int _depth = 0;
            Move[] _movesAvailable = Board.FindNextColorMoves(board);

        }

        /// <summary>
        /// конструктор наследования/углубления
        /// реализует один из возможных ходов родительской доски
        /// </summary>
        public Three(Three a, int moveNumber)
        {
            this._parent = a;//породивший узел называется родителем
            a.AddChild(this);//вновь созданный узел называется потомком

            Move[] MovesAvailable = a._movesAvailable;
            if ((MovesAvailable != null) && (MovesAvailable.Length > moveNumber))
            {
                this._boardMoved = Board.DoMove(a._boardMoved, MovesAvailable[moveNumber])  ;
            }

            this._move = MovesAvailable[moveNumber];

            //this._valueMax = -1000;
            //this._valueMin = 1000;

            int _depth = a._depth + 1;

            if (_depth < _maxDepth)//если мы еще не достигли нужной глубины нужно найти все возможные ходы этой доски
            { this._movesAvailable = Board.FindNextColorMoves(a._boardMoved); }
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

    }

    
}
