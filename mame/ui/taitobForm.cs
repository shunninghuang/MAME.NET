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
    public partial class taitobForm : Form
    {
        private mainForm _myParentForm;
        private int locationX, locationY;
        public taitobForm(mainForm form)
        {
            this._myParentForm = form;
            InitializeComponent();
        }
        private void taitobForm_Load(object sender, EventArgs e)
        {

        }
        private void taitobForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        private void btnDraw_Click(object sender, EventArgs e)
        {
            Taitob.bBg = cbBg.Checked;
            Taitob.bFg = cbFg.Checked;
            Taitob.bTx = cbTx.Checked;
            Taitob.bSprite = cbSprite.Checked;
            Bitmap bm1 = Taitob.GetAllGDI();
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
            tsslLocation.Text = locationX + "," + locationY;
            Application.DoEvents();
        }        
    }
}
