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
            this.Btn_StartStop = new System.Windows.Forms.Button();
            this.Btn_Exit = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CB_GSM = new System.Windows.Forms.CheckBox();
            this.CB_Alter = new System.Windows.Forms.CheckBox();
            this.statusStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "驰骋工作流程服务";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.Click += new System.EventHandler(this.notifyIcon1_Click);
            // 
            // Btn_StartStop
            // 
            this.Btn_StartStop.Location = new System.Drawing.Point(150, 403);
            this.Btn_StartStop.Name = "Btn_StartStop";
            this.Btn_StartStop.Size = new System.Drawing.Size(75, 23);
            this.Btn_StartStop.TabIndex = 2;
            this.Btn_StartStop.Text = "启动";
            this.Btn_StartStop.UseVisualStyleBackColor = true;
            this.Btn_StartStop.Click += new System.EventHandler(this.Btn_Runing_Click);
            // 
            // Btn_Exit
            // 
            this.Btn_Exit.Location = new System.Drawing.Point(252, 403);
            this.Btn_Exit.Name = "Btn_Exit";
            this.Btn_Exit.Size = new System.Drawing.Size(75, 23);
            this.Btn_Exit.TabIndex = 3;
            this.Btn_Exit.Text = "退出";
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
            this.statusStrip1.Text = "状态提示...";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(131, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
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
            this.groupBox1.Text = "执行信息";
            // 
            // CB_GSM
            // 
            this.CB_GSM.AutoSize = true;
            this.CB_GSM.Checked = true;
            this.CB_GSM.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_GSM.Location = new System.Drawing.Point(47, 407);
            this.CB_GSM.Name = "CB_GSM";
            this.CB_GSM.Size = new System.Drawing.Size(84, 16);
            this.CB_GSM.TabIndex = 5;
            this.CB_GSM.Text = "启动短信？";
            this.CB_GSM.UseVisualStyleBackColor = true;
            // 
            // CB_Alter
            // 
            this.CB_Alter.AutoSize = true;
            this.CB_Alter.Location = new System.Drawing.Point(248, 442);
            this.CB_Alter.Name = "CB_Alter";
            this.CB_Alter.Size = new System.Drawing.Size(108, 16);
            this.CB_Alter.TabIndex = 6;
            this.CB_Alter.Text = "是否提示消息？";
            this.CB_Alter.UseVisualStyleBackColor = true;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(425, 496);
            this.Controls.Add(this.CB_Alter);
            this.Controls.Add(this.CB_GSM);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.Btn_Exit);
            this.Controls.Add(this.Btn_StartStop);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FrmMain";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "驰骋工作流引擎短信提醒服务";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Button Btn_StartStop;
        private System.Windows.Forms.Button Btn_Exit;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox CB_GSM;
        private System.Windows.Forms.CheckBox CB_Alter;
    }
}

