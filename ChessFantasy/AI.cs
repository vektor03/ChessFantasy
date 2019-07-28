using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessFantasy
{
    public class Three //содержит дерево возможных ходов
    {
        private Three _parent;//родительский элемент
        public Three Parent
        {
            get { return _parent; }
            //set { _figures = value; }
        }

        private Three[] _children;//дочерние элементы
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

        private int _value;//оценка состояния доски
        public int Value
        {
            get { return _value; }
            //set { _figures = value; }
        }

        /*
        public FiguresXY(XY[] figures, XY king)//конструктор из массива
        {
            _figures = figures;
            _king = king;
        }*/

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

    class AI
    {


    }
}
