﻿namespace ui
{
    partial class namcos1Form
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
            this.btnDraw = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tbLayer = new System.Windows.Forms.TextBox();
            this.btnDraw2 = new System.Windows.Forms.Button();
            this.btnDrawS = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsslLocation = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDraw
            // 
            this.btnDraw.Location = new System.Drawing.Point(12, 177);
            this.btnDraw.Name = "btnDraw";
            this.btnDraw.Size = new System.Drawing.Size(70, 21);
            this.btnDraw.TabIndex = 0;
            this.btnDraw.Text = "draw";
            this.btnDraw.UseVisualStyleBackColor = true;
            this.btnDraw.Click += new System.EventHandler(this.btnDraw_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(88, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(512, 512);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            // 
            // tbLayer
            // 
            this.tbLayer.Location = new System.Drawing.Point(12, 95);
            this.tbLayer.Name = "tbLayer";
            this.tbLayer.Size = new System.Drawing.Size(70, 21);
            this.tbLayer.TabIndex = 2;
            // 
            // btnDraw2
            // 
            this.btnDraw2.Location = new System.Drawing.Point(12, 231);
            this.btnDraw2.Name = "btnDraw2";
            this.btnDraw2.Size = new System.Drawing.Size(70, 21);
            this.btnDraw2.TabIndex = 0;
            this.btnDraw2.Text = "draw2";
            this.btnDraw2.UseVisualStyleBackColor = true;
            this.btnDraw2.Click += new System.EventHandler(this.btnDraw2_Click);
            // 
            // btnDrawS
            // 
            this.btnDrawS.Location = new System.Drawing.Point(12, 204);
            this.btnDrawS.Name = "btnDrawS";
            this.btnDrawS.Size = new System.Drawing.Size(70, 21);
            this.btnDrawS.TabIndex = 0;
            this.btnDrawS.Text = "draws";
            this.btnDrawS.UseVisualStyleBackColor = true;
            this.btnDrawS.Click += new System.EventHandler(this.btnDrawS_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslLocation});
            this.statusStrip1.Location = new System.Drawing.Point(0, 540);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(614, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsslLocation
            // 
            this.tsslLocation.Name = "tsslLocation";
            this.tsslLocation.Size = new System.Drawing.Size(17, 17);
            this.tsslLocation.Text = "...";
            // 
            // namcos1Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(614, 562);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tbLayer);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnDrawS);
            this.Controls.Add(this.btnDraw2);
            this.Controls.Add(this.btnDraw);
            this.Name = "namcos1Form";
            this.Text = "namcos1Form";
            this.Load += new System.EventHandler(this.namcos1Form_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.namcos1Form_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDraw;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox tbLayer;
        private System.Windows.Forms.Button btnDraw2;
        private System.Windows.Forms.Button btnDrawS;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsslLocation;
    }
}