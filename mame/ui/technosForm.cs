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
    public partial class technosForm : Form
    {
        private mainForm _myParentForm;
        private int locationX, locationY;
        public technosForm(mainForm form)
        {
            this._myParentForm = form;
            InitializeComponent();
        }
        private void technosForm_Load(object sender, EventArgs e)
        {

        }
        private void technosForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        private void btnDraw_Click(object sender, EventArgs e)
        {
            Technos.bBg = cbBg.Checked;
            Technos.bSprite = cbSprite.Checked;
            Technos.bFg = cbFg.Checked;
            Bitmap bm1 = Technos.GetAllGDI();
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
