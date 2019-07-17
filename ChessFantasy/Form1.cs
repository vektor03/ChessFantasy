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
        public Form1()
        {
            InitializeComponent();
            Draw();
            MoveCursor();
        }

        private void Draw()
        {
            Bitmap Bmp = new Bitmap(picture.Width, picture.Height);
            Graphics Graph = Graphics.FromImage(Bmp);
            // Create image.
            Image newImage = Image.FromFile("Resources\\BlackCell.png");

            
            Point ulCorner = new Point(0, 100);// Create Point for upper-left corner of image
            Graph.DrawImage(newImage, ulCorner);// Draw image to screen

            ulCorner = new Point(100, 0);// Create Point for upper-left corner of image
            Graph.DrawImage(newImage, ulCorner);// Draw image to screen
            newImage = Image.FromFile("Resources\\WhiteCell.png");
            ulCorner = new Point(700, 700);// Create Point for upper-left corner of image
            Graph.DrawImage(newImage, ulCorner);// Draw image to screen
            newImage = Image.FromFile("Resources\\BlackKing.png");
            ulCorner = new Point(0, 100);// Create Point for upper-left corner of image
            Graph.DrawImage(newImage, ulCorner);// Draw image to screen
            newImage = Image.FromFile("Resources\\WhiteKing.png");
            ulCorner = new Point(100, 0);// Create Point for upper-left corner of image
            Graph.DrawImage(newImage, ulCorner);// Draw image to screen

            //System.Windows.Forms.Cursor.Position

            picture.Image = Bmp;
        }

        private void MoveCursor()
        {
            // Set the Current cursor, move the cursor's Position,

            //this.Cursor = new Cursor(Cursor.Current.Handle);
            //Cursor.Position = new Point(Cursor.Position.X - 500, Cursor.Position.Y - 500);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Picture_Click(object sender, EventArgs e)
        {

        }
    }
}
