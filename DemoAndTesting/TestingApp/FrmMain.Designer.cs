namespace SMSServices
{
    partial class FrmMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.Btn_OK = new System.Windows.Forms.Button();
            this.Btn_Exit = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.Btn_All = new System.Windows.Forms.Button();
            this.Btn_Editing = new System.Windows.Forms.Button();
            this.Btn_RunOne = new System.Windows.Forms.Button();
            this.Btn_XiaoLV = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "驰骋工作流程服务";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.Click += new System.EventHandler(this.notifyIcon1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox1.Location = new System.Drawing.Point(38, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(326, 370);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "产品质量是产品的生命线";
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(3, 20);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(320, 347);
            this.textBox1.TabIndex = 0;
            // 
            // Btn_OK
            // 
            this.Btn_OK.Location = new System.Drawing.Point(41, 403);
            this.Btn_OK.Name = "Btn_OK";
            this.Btn_OK.Size = new System.Drawing.Size(75, 23);
            this.Btn_OK.TabIndex = 2;
            this.Btn_OK.Text = "OK";
            this.Btn_OK.UseVisualStyleBackColor = true;
            this.Btn_OK.Click += new System.EventHandler(this.Btn_OK_Click);
            // 
            // Btn_Exit
            // 
            this.Btn_Exit.Location = new System.Drawing.Point(292, 403);
            this.Btn_Exit.Name = "Btn_Exit";
            this.Btn_Exit.Size = new System.Drawing.Size(75, 23);
            this.Btn_Exit.TabIndex = 3;
            this.Btn_Exit.Text = "Exit";
            this.Btn_Exit.UseVisualStyleBackColor = true;
            this.Btn_Exit.Click += new System.EventHandler(this.Btn_Exit_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 474);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(425, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(131, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // Btn_All
            // 
            this.Btn_All.Location = new System.Drawing.Point(207, 403);
            this.Btn_All.Name = "Btn_All";
            this.Btn_All.Size = new System.Drawing.Size(75, 23);
            this.Btn_All.TabIndex = 5;
            this.Btn_All.Text = "UnOK";
            this.Btn_All.UseVisualStyleBackColor = true;
            this.Btn_All.Click += new System.EventHandler(this.Btn_All_Click);
            // 
            // Btn_Editing
            // 
            this.Btn_Editing.Location = new System.Drawing.Point(126, 403);
            this.Btn_Editing.Name = "Btn_Editing";
            this.Btn_Editing.Size = new System.Drawing.Size(75, 23);
            this.Btn_Editing.TabIndex = 6;
            this.Btn_Editing.Text = "Editing";
            this.Btn_Editing.UseVisualStyleBackColor = true;
            this.Btn_Editing.Click += new System.EventHandler(this.Btn_Editing_Click);
            // 
            // Btn_RunOne
            // 
            this.Btn_RunOne.Location = new System.Drawing.Point(41, 448);
            this.Btn_RunOne.Name = "Btn_RunOne";
            this.Btn_RunOne.Size = new System.Drawing.Size(75, 23);
            this.Btn_RunOne.TabIndex = 7;
            this.Btn_RunOne.Text = "Run One";
            this.Btn_RunOne.UseVisualStyleBackColor = true;
            this.Btn_RunOne.Click += new System.EventHandler(this.Btn_RunOne_Click);
            // 
            // Btn_XiaoLV
            // 
            this.Btn_XiaoLV.Location = new System.Drawing.Point(126, 448);
            this.Btn_XiaoLV.Name = "Btn_XiaoLV";
            this.Btn_XiaoLV.Size = new System.Drawing.Size(75, 23);
            this.Btn_XiaoLV.TabIndex = 8;
            this.Btn_XiaoLV.Text = "效率测试";
            this.Btn_XiaoLV.UseVisualStyleBackColor = true;
            this.Btn_XiaoLV.Click += new System.EventHandler(this.Btn_XiaoLV_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(425, 496);
            this.Controls.Add(this.Btn_XiaoLV);
            this.Controls.Add(this.Btn_RunOne);
            this.Controls.Add(this.Btn_Editing);
            this.Controls.Add(this.Btn_All);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.Btn_Exit);
            this.Controls.Add(this.Btn_OK);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FrmMain";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CCFlow单元测试";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button Btn_OK;
        private System.Windows.Forms.Button Btn_Exit;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button Btn_All;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button Btn_Editing;
        private System.Windows.Forms.Button Btn_RunOne;
        private System.Windows.Forms.Button Btn_XiaoLV;
    }
}

