using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using mame;

namespace ui
{
    public partial class namcos1Form : Form
    {
        private mainForm _myParentForm;
        private int locationX, locationY;
        public namcos1Form(mainForm form)
        {
            this._myParentForm = form;
            InitializeComponent();
        }
        private void namcos1Form_Load(object sender, EventArgs e)
        {
            tbLayer.Text = "1";
        }
        private void namcos1Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        private void btnDraw_Click(object sender, EventArgs e)
        {
            int n;
            Bitmap bm1;
            n = int.Parse(tbLayer.Text);
            bm1 = Namcos1.GetLayer(n);
            switch (Machine.sDirection)
            {
                case "":
                    break;
                case "90":
                    bm1.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case "180":
                    bm1.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case "270":
                    bm1.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }
            pictureBox1.Image = bm1;
        }
        private void btnDrawS_Click(object sender, EventArgs e)
        {
            Bitmap bm1;
            bm1 = Namcos1.GetSprite();
            pictureBox1.Image = bm1;
        }
        private void btnDraw2_Click(object sender, EventArgs e)
        {
            Bitmap bm1;
            bm1 = Namcos1.GetPri();
            switch (Machine.sDirection)
            {
                case "":
                    break;
                case "90":
                    bm1.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case "180":
                    bm1.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case "270":
                    bm1.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }
            pictureBox1.Image = bm1;
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            locationX = e.Location.X;
            locationY = e.Location.Y;
            if (locationX >= 0 && locationX <= 0x1ff && locationY >= 0 && locationY <= 0x1ff)
            {
                tsslLocation.Text = locationX + "," + locationY + "," + Tilemap.priority_bitmap[locationX, 0x1ff - locationY].ToString();
            }
            Application.DoEvents();
        }
    }
}
