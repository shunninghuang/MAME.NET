namespace ui
{
    partial class segaForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsslLocation = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnDraw = new System.Windows.Forms.Button();
            this.tbLayer = new System.Windows.Forms.TextBox();
            this.btnDrawtemp = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(257, 35);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(512, 512);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslLocation});
            this.statusStrip1.Location = new System.Drawing.Point(0, 603);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(834, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsslLocation
            // 
            this.tsslLocation.Name = "tsslLocation";
            this.tsslLocation.Size = new System.Drawing.Size(17, 17);
            this.tsslLocation.Text = "...";
            // 
            // btnDraw
            // 
            this.btnDraw.Location = new System.Drawing.Point(48, 157);
            this.btnDraw.Name = "btnDraw";
            this.btnDraw.Size = new System.Drawing.Size(70, 21);
            this.btnDraw.TabIndex = 2;
            this.btnDraw.Text = "draw";
            this.btnDraw.UseVisualStyleBackColor = true;
            this.btnDraw.Click += new System.EventHandler(this.btnDraw_Click);
            // 
            // tbLayer
            // 
            this.tbLayer.Location = new System.Drawing.Point(48, 130);
            this.tbLayer.Name = "tbLayer";
            this.tbLayer.Size = new System.Drawing.Size(70, 21);
            this.tbLayer.TabIndex = 3;
            // 
            // btnDrawtemp
            // 
            this.btnDrawtemp.Location = new System.Drawing.Point(48, 184);
            this.btnDrawtemp.Name = "btnDrawtemp";
            this.btnDrawtemp.Size = new System.Drawing.Size(70, 21);
            this.btnDrawtemp.TabIndex = 4;
            this.btnDrawtemp.Text = "draw temp";
            this.btnDrawtemp.UseVisualStyleBackColor = true;
            this.btnDrawtemp.Click += new System.EventHandler(this.btnDrawtemp_Click);
            // 
            // segaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 625);
            this.Controls.Add(this.btnDrawtemp);
            this.Controls.Add(this.tbLayer);
            this.Controls.Add(this.btnDraw);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "segaForm";
            this.Text = "segaForm";
            this.Load += new System.EventHandler(this.segaForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.segaForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsslLocation;
        private System.Windows.Forms.Button btnDraw;
        private System.Windows.Forms.TextBox tbLayer;
        private System.Windows.Forms.Button btnDrawtemp;
    }
}