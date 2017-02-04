namespace CCFlowWord2007
{
    partial class FrmStartFlow
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
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.Btn_Start = new System.Windows.Forms.Button();
            this.Btn_Exit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(3, 1);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(244, 307);
            this.treeView1.TabIndex = 0;
            // 
            // Btn_Start
            // 
            this.Btn_Start.Location = new System.Drawing.Point(273, 42);
            this.Btn_Start.Name = "Btn_Start";
            this.Btn_Start.Size = new System.Drawing.Size(75, 23);
            this.Btn_Start.TabIndex = 1;
            this.Btn_Start.Text = "发起流程";
            this.Btn_Start.UseVisualStyleBackColor = true;
            this.Btn_Start.Click += new System.EventHandler(this.Btn_Start_Click);
            // 
            // Btn_Exit
            // 
            this.Btn_Exit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Btn_Exit.Location = new System.Drawing.Point(273, 275);
            this.Btn_Exit.Name = "Btn_Exit";
            this.Btn_Exit.Size = new System.Drawing.Size(75, 23);
            this.Btn_Exit.TabIndex = 2;
            this.Btn_Exit.Text = "退出";
            this.Btn_Exit.UseVisualStyleBackColor = true;
            this.Btn_Exit.Click += new System.EventHandler(this.Btn_Exit_Click);
            // 
            // FrmStartFlow
            // 
            this.AcceptButton = this.Btn_Start;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Btn_Exit;
            this.ClientSize = new System.Drawing.Size(360, 310);
            this.Controls.Add(this.Btn_Exit);
            this.Controls.Add(this.Btn_Start);
            this.Controls.Add(this.treeView1);
            this.Name = "FrmStartFlow";
            this.Text = "发起流程";
            this.Load += new System.EventHandler(this.FrmStartFlow_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button Btn_Start;
        private System.Windows.Forms.Button Btn_Exit;
    }
}