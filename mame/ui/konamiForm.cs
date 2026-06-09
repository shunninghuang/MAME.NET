using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using mame;

namespace ui
{
    public partial class konamiForm : Form
    {
        private mainForm _myParentForm;
        private int locationX, locationY;
        public konamiForm(mainForm form)
        {
            this._myParentForm = form;
            InitializeComponent();
        }
        private void konamiForm_Load(object sender, EventArgs e)
        {
            tbSprite.Text= "0000-4000";
        }
        private void konamiForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        private void btnDraw_Click(object sender, EventArgs e)
        {
            Konami.bTile0 = cbT0.Checked;
            Konami.bTile1 = cbT1.Checked;
            Konami.bTile2 = cbT2.Checked;
            Konami.bSprite = cbSprite.Checked;
            Bitmap bm1 = Konami.GetAllGDI();
            pictureBox1.Image = bm1;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            pictureBox1.Image.Save("1.png", ImageFormat.Png);
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            locationX = e.Location.X;
            locationY = e.Location.Y;
            tsslLocation.Text = locationX + "," + locationY;
            Application.DoEvents();
        }
        private void btnDump_Click(object sender, EventArgs e)
        {
            BinaryWriter writer = new BinaryWriter(new FileStream("1.dmp", FileMode.Create));
            Konami.SaveStateBinary_K053251(writer);
            Konami.SaveStateBinary_K052109(writer);
            Konami.SaveStateBinary_K053245(writer);
            writer.Write(Memory.mainram, 0, 0x4000);
            writer.Write(Konami.mainram2, 0, 0x80);
            writer.Close();
        }        
    }
}
