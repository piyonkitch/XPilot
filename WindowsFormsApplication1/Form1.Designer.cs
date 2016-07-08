namespace WindowsFormsApplication1
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.Count = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.セーブToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pic = new System.Windows.Forms.PictureBox();
            this.buttonshoot = new System.Windows.Forms.Button();
            this.buttoninc = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pic)).BeginInit();
            this.SuspendLayout();
            // 
            // Count
            // 
            this.Count.AutoSize = true;
            this.Count.Location = new System.Drawing.Point(70, 117);
            this.Count.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Count.Name = "Count";
            this.Count.Size = new System.Drawing.Size(0, 18);
            this.Count.TabIndex = 9;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(10, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(383, 24);
            this.menuStrip1.TabIndex = 12;
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(32, 19);
            // 
            // セーブToolStripMenuItem
            // 
            this.セーブToolStripMenuItem.Name = "セーブToolStripMenuItem";
            this.セーブToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // pic
            // 
            this.pic.Location = new System.Drawing.Point(0, 0);
            this.pic.Name = "pic";
            this.pic.Size = new System.Drawing.Size(371, 275);
            this.pic.TabIndex = 13;
            this.pic.TabStop = false;
            this.pic.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // buttonshoot
            // 
            this.buttonshoot.Location = new System.Drawing.Point(52, 324);
            this.buttonshoot.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.buttonshoot.Name = "buttonshoot";
            this.buttonshoot.Size = new System.Drawing.Size(125, 34);
            this.buttonshoot.TabIndex = 8;
            this.buttonshoot.Text = "Shoot";
            this.buttonshoot.UseVisualStyleBackColor = true;
            this.buttonshoot.Click += new System.EventHandler(this.buttonshoot_Click);
            // 
            // buttoninc
            // 
            this.buttoninc.Location = new System.Drawing.Point(52, 282);
            this.buttoninc.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.buttoninc.Name = "buttoninc";
            this.buttoninc.Size = new System.Drawing.Size(125, 34);
            this.buttoninc.TabIndex = 6;
            this.buttoninc.Text = "Inc";
            this.buttoninc.UseVisualStyleBackColor = true;
            this.buttoninc.Click += new System.EventHandler(this.buttoninc_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(383, 392);
            this.Controls.Add(this.pic);
            this.Controls.Add(this.Count);
            this.Controls.Add(this.buttonshoot);
            this.Controls.Add(this.buttoninc);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pic)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label Count;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem セーブToolStripMenuItem;
        private System.Windows.Forms.PictureBox pic;
        private System.Windows.Forms.Button buttonshoot;
        private System.Windows.Forms.Button buttoninc;
    }
}

