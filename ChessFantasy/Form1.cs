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
        }

        private void Draw()
        {
            Bitmap Bmp = new Bitmap(picture.Width, picture.Height);
            Graphics Graph = Graphics.FromImage(Bmp);
            // Create image.
            Image newImage = Image.FromFile("D:\\VICTOR\\ChessFantasy\\ChessFantasy\\bin\\Debug\\Resources\\BlackCell.png");

            // Create Point for upper-left corner of image.
            Point ulCorner = new Point(100, 100);

            // Draw image to screen.
            Graph.DrawImage(newImage, ulCorner);

            picture.Image = Bmp;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
