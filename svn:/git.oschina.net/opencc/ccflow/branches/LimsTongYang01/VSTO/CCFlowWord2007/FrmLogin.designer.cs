namespace BP.Comm
{
    partial class FrmLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmLogin));
            this.Btn_C = new System.Windows.Forms.Button();
            this.Btn_OK = new System.Windows.Forms.Button();
            this.TB_User = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.TB_Pass = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.PictureBox1 = new System.Windows.Forms.PictureBox();
            this.CB_SaveInfo = new System.Windows.Forms.CheckBox();
            this.CB_SavePass = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // Btn_C
            // 
            resources.ApplyResources(this.Btn_C, "Btn_C");
            this.Btn_C.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Btn_C.Name = "Btn_C";
            this.Btn_C.Click += new System.EventHandler(this.Btn_C_Click);
            // 
            // Btn_OK
            // 
            resources.ApplyResources(this.Btn_OK, "Btn_OK");
            this.Btn_OK.Name = "Btn_OK";
            this.Btn_OK.Click += new System.EventHandler(this.Btn_OK_Click);
            // 
            // TB_User
            // 
            resources.ApplyResources(this.TB_User, "TB_User");
            this.TB_User.Name = "TB_User";
            // 
            // Label2
            // 
            resources.ApplyResources(this.Label2, "Label2");
            this.Label2.Name = "Label2";
            // 
            // TB_Pass
            // 
            resources.ApplyResources(this.TB_Pass, "TB_Pass");
            this.TB_Pass.Name = "TB_Pass";
            // 
            // Label1
            // 
            resources.ApplyResources(this.Label1, "Label1");
            this.Label1.Name = "Label1";
            // 
            // PictureBox1
            // 
            resources.ApplyResources(this.PictureBox1, "PictureBox1");
            this.PictureBox1.Name = "PictureBox1";
            this.PictureBox1.TabStop = false;
            // 
            // CB_SaveInfo
            // 
            resources.ApplyResources(this.CB_SaveInfo, "CB_SaveInfo");
            this.CB_SaveInfo.Name = "CB_SaveInfo";
            this.CB_SaveInfo.UseVisualStyleBackColor = true;
            // 
            // CB_SavePass
            // 
            resources.ApplyResources(this.CB_SavePass, "CB_SavePass");
            this.CB_SavePass.Name = "CB_SavePass";
            this.CB_SavePass.UseVisualStyleBackColor = true;
            this.CB_SavePass.CheckedChanged += new System.EventHandler(this.CB_SavePass_CheckedChanged);
            // 
            // FrmLogin
            // 
            this.AcceptButton = this.Btn_OK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Btn_C;
            this.Controls.Add(this.CB_SavePass);
            this.Controls.Add(this.CB_SaveInfo);
            this.Controls.Add(this.PictureBox1);
            this.Controls.Add(this.Btn_C);
            this.Controls.Add(this.Btn_OK);
            this.Controls.Add(this.TB_User);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.TB_Pass);
            this.Controls.Add(this.Label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmLogin";
            this.Load += new System.EventHandler(this.FrmLogin_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Btn_C;
        private System.Windows.Forms.Button Btn_OK;
        private System.Windows.Forms.TextBox TB_User;
        private System.Windows.Forms.Label Label2;
        private System.Windows.Forms.TextBox TB_Pass;
        private System.Windows.Forms.Label Label1;
        private System.Windows.Forms.PictureBox PictureBox1;
        private System.Windows.Forms.CheckBox CB_SaveInfo;
        private System.Windows.Forms.CheckBox CB_SavePass;
    }
}