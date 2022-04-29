namespace CS689HW01
{
    partial class Form1
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
            this.B01 = new System.Windows.Forms.Button();
            this.B02 = new System.Windows.Forms.Button();
            this.B03 = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.progressBar3 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.B04 = new System.Windows.Forms.Button();
            this.B05 = new System.Windows.Forms.Button();
            this.B06 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(196, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1000, 800);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // B01
            // 
            this.B01.Location = new System.Drawing.Point(12, 12);
            this.B01.Name = "B01";
            this.B01.Size = new System.Drawing.Size(178, 60);
            this.B01.TabIndex = 1;
            this.B01.Text = "Set 01";
            this.B01.UseVisualStyleBackColor = true;
            this.B01.Click += new System.EventHandler(this.B01_Click);
            // 
            // B02
            // 
            this.B02.Location = new System.Drawing.Point(12, 166);
            this.B02.Name = "B02";
            this.B02.Size = new System.Drawing.Size(178, 60);
            this.B02.TabIndex = 2;
            this.B02.Text = "Set 02";
            this.B02.UseVisualStyleBackColor = true;
            this.B02.Click += new System.EventHandler(this.B02_Click);
            // 
            // B03
            // 
            this.B03.Location = new System.Drawing.Point(12, 320);
            this.B03.Name = "B03";
            this.B03.Size = new System.Drawing.Size(178, 60);
            this.B03.TabIndex = 3;
            this.B03.Text = "Set 03";
            this.B03.UseVisualStyleBackColor = true;
            this.B03.Click += new System.EventHandler(this.B03_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 110);
            this.progressBar1.Maximum = 1000000;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(178, 10);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 4;
            // 
            // progressBar2
            // 
            this.progressBar2.Location = new System.Drawing.Point(12, 264);
            this.progressBar2.Maximum = 1000000;
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(178, 10);
            this.progressBar2.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar2.TabIndex = 5;
            // 
            // progressBar3
            // 
            this.progressBar3.Location = new System.Drawing.Point(12, 418);
            this.progressBar3.Maximum = 1000000;
            this.progressBar3.Name = "progressBar3";
            this.progressBar3.Size = new System.Drawing.Size(178, 10);
            this.progressBar3.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar3.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 123);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 17);
            this.label1.TabIndex = 7;
            this.label1.Text = "{  }";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 277);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "{  }";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 431);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 17);
            this.label3.TabIndex = 9;
            this.label3.Text = "{  }";
            // 
            // B04
            // 
            this.B04.Location = new System.Drawing.Point(12, 78);
            this.B04.Name = "B04";
            this.B04.Size = new System.Drawing.Size(178, 26);
            this.B04.TabIndex = 10;
            this.B04.Text = "Retry Set 01";
            this.B04.UseVisualStyleBackColor = true;
            this.B04.Click += new System.EventHandler(this.B04_Click);
            // 
            // B05
            // 
            this.B05.Location = new System.Drawing.Point(12, 232);
            this.B05.Name = "B05";
            this.B05.Size = new System.Drawing.Size(178, 26);
            this.B05.TabIndex = 11;
            this.B05.Text = "Retry Set 02";
            this.B05.UseVisualStyleBackColor = true;
            this.B05.Click += new System.EventHandler(this.B05_Click);
            // 
            // B06
            // 
            this.B06.Location = new System.Drawing.Point(12, 386);
            this.B06.Name = "B06";
            this.B06.Size = new System.Drawing.Size(178, 26);
            this.B06.TabIndex = 12;
            this.B06.Text = "Retry Set 03";
            this.B06.UseVisualStyleBackColor = true;
            this.B06.Click += new System.EventHandler(this.B06_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1209, 825);
            this.Controls.Add(this.B06);
            this.Controls.Add(this.B05);
            this.Controls.Add(this.B04);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar3);
            this.Controls.Add(this.progressBar2);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.B03);
            this.Controls.Add(this.B02);
            this.Controls.Add(this.B01);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button B01;
        private System.Windows.Forms.Button B02;
        private System.Windows.Forms.Button B03;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ProgressBar progressBar2;
        private System.Windows.Forms.ProgressBar progressBar3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button B04;
        private System.Windows.Forms.Button B05;
        private System.Windows.Forms.Button B06;
    }
}

