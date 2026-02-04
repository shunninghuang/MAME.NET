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
    public partial class megasys1Form : Form
    {
        private mainForm _myParentForm;
        private int locationX, locationY;
        public megasys1Form(mainForm form)
        {
            this._myParentForm = form;
            InitializeComponent();
        }                
        private void megasys1Form_Load(object sender, EventArgs e)
        {

        }
        private void megasys1Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        private void btnDraw_Click(object sender, EventArgs e)
        {
            Megasys1.bSprite = cbSprite.Checked;
            Bitmap bm1 = Megasys1.GetAllGDI();
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
