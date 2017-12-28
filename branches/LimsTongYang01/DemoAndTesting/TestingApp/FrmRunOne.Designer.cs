namespace TestingApp
{
    partial class FrmRunOne
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
            this.label1 = new System.Windows.Forms.Label();
            this.Btn_Do = new System.Windows.Forms.Button();
            this.Btn_Exit = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "请选择执行的Case";
            // 
            // Btn_Do
            // 
            this.Btn_Do.Location = new System.Drawing.Point(363, 386);
            this.Btn_Do.Name = "Btn_Do";
            this.Btn_Do.Size = new System.Drawing.Size(75, 23);
            this.Btn_Do.TabIndex = 2;
            this.Btn_Do.Text = "Do";
            this.Btn_Do.UseVisualStyleBackColor = true;
            this.Btn_Do.Click += new System.EventHandler(this.Btn_Do_Click);
            // 
            // Btn_Exit
            // 
            this.Btn_Exit.Location = new System.Drawing.Point(454, 387);
            this.Btn_Exit.Name = "Btn_Exit";
            this.Btn_Exit.Size = new System.Drawing.Size(70, 22);
            this.Btn_Exit.TabIndex = 3;
            this.Btn_Exit.Text = "Exit";
            this.Btn_Exit.UseVisualStyleBackColor = true;
            this.Btn_Exit.Click += new System.EventHandler(this.Btn_Exit_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(14, 35);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(499, 328);
            this.listBox1.TabIndex = 4;
            // 
            // FrmRunOne
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(525, 412);
            this.ControlBox = false;
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.Btn_Exit);
            this.Controls.Add(this.Btn_Do);
            this.Controls.Add(this.label1);
            this.Name = "FrmRunOne";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmRunOne";
            this.Load += new System.EventHandler(this.FrmRunOne_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Btn_Do;
        private System.Windows.Forms.Button Btn_Exit;
        private System.Windows.Forms.ListBox listBox1;
    }
}