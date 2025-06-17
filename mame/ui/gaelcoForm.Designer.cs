namespace ui
{
    partial class gaelcoForm
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
            this.cbMap0 = new System.Windows.Forms.CheckBox();
            this.cbMap1 = new System.Windows.Forms.CheckBox();
            this.cbSprite = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(287, 22);
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
            this.statusStrip1.Location = new System.Drawing.Point(0, 586);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(829, 22);
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
            this.btnDraw.Location = new System.Drawing.Point(12, 222);
            this.btnDraw.Name = "btnDraw";
            this.btnDraw.Size = new System.Drawing.Size(70, 21);
            this.btnDraw.TabIndex = 2;
            this.btnDraw.Text = "draw";
            this.btnDraw.UseVisualStyleBackColor = true;
            this.btnDraw.Click += new System.EventHandler(this.btnDraw_Click);
            // 
            // cbMap0
            // 
            this.cbMap0.AutoSize = true;
            this.cbMap0.Location = new System.Drawing.Point(12, 64);
            this.cbMap0.Name = "cbMap0";
            this.cbMap0.Size = new System.Drawing.Size(48, 16);
            this.cbMap0.TabIndex = 3;
            this.cbMap0.Text = "map0";
            this.cbMap0.UseVisualStyleBackColor = true;
            // 
            // cbMap1
            // 
            this.cbMap1.AutoSize = true;
            this.cbMap1.Location = new System.Drawing.Point(12, 86);
            this.cbMap1.Name = "cbMap1";
            this.cbMap1.Size = new System.Drawing.Size(48, 16);
            this.cbMap1.TabIndex = 3;
            this.cbMap1.Text = "map1";
            this.cbMap1.UseVisualStyleBackColor = true;
            // 
            // cbSprite
            // 
            this.cbSprite.AutoSize = true;
            this.cbSprite.Location = new System.Drawing.Point(12, 108);
            this.cbSprite.Name = "cbSprite";
            this.cbSprite.Size = new System.Drawing.Size(60, 16);
            this.cbSprite.TabIndex = 3;
            this.cbSprite.Text = "sprite";
            this.cbSprite.UseVisualStyleBackColor = true;
            // 
            // gaelcoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(829, 608);
            this.Controls.Add(this.cbSprite);
            this.Controls.Add(this.cbMap1);
            this.Controls.Add(this.cbMap0);
            this.Controls.Add(this.btnDraw);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "gaelcoForm";
            this.Text = "gaelcoForm";
            this.Load += new System.EventHandler(this.gaelcoForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.gaelcoForm_FormClosing);
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
        private System.Windows.Forms.CheckBox cbMap0;
        private System.Windows.Forms.CheckBox cbMap1;
        private System.Windows.Forms.CheckBox cbSprite;
    }
}