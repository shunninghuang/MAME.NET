using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using mame;

namespace ui
{
    public partial class segaForm : Form
    {
        private mainForm _myParentForm;
        private int locationX, locationY;
        public segaForm(mainForm form)
        {
            this._myParentForm = form;
            InitializeComponent();
        }
        private void segaForm_Load(object sender, EventArgs e)
        {
            tbLayer.Text = "0";
        }
        private void segaForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        private void btnDraw_Click(object sender, EventArgs e)
        {
            int n;
            Bitmap bm1;
            n = int.Parse(tbLayer.Text);
            bm1 = sega.GetLayer(n);
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
        private void btnDrawtemp_Click(object sender, EventArgs e)
        {
            Bitmap bm1;
            bm1 = sega.gettempbitmap();
            pictureBox1.Image = bm1;
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            locationX = e.Location.X;
            locationY = e.Location.Y;
            tsslLocation.Text = locationX + "," + locationY;
            Application.DoEvents();
        }        
    }
}
