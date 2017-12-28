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
            this.Btn_StartStop = new System.Windows.Forms.Button();
            this.Btn_OpenData = new System.Windows.Forms.Button();
            this.Btn_Exit = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.Btn_Min = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.Btn_ToolBox = new System.Windows.Forms.Button();
            this.CB_IsWriteToCCIM = new System.Windows.Forms.CheckBox();
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
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Location = new System.Drawing.Point(38, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(326, 370);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "运行信息";
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(3, 17);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(320, 350);
            this.textBox1.TabIndex = 0;
            // 
            // Btn_StartStop
            // 
            this.Btn_StartStop.Location = new System.Drawing.Point(36, 403);
            this.Btn_StartStop.Name = "Btn_StartStop";
            this.Btn_StartStop.Size = new System.Drawing.Size(75, 23);
            this.Btn_StartStop.TabIndex = 1;
            this.Btn_StartStop.Text = "运行/暂停";
            this.Btn_StartStop.UseVisualStyleBackColor = true;
            this.Btn_StartStop.Click += new System.EventHandler(this.Btn_StartStop_Click);
            // 
            // Btn_OpenData
            // 
            this.Btn_OpenData.Location = new System.Drawing.Point(121, 403);
            this.Btn_OpenData.Name = "Btn_OpenData";
            this.Btn_OpenData.Size = new System.Drawing.Size(75, 23);
            this.Btn_OpenData.TabIndex = 2;
            this.Btn_OpenData.Text = "数据统计";
            this.Btn_OpenData.UseVisualStyleBackColor = true;
            // 
            // Btn_Exit
            // 
            this.Btn_Exit.Location = new System.Drawing.Point(292, 403);
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
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(131, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // Btn_Min
            // 
            this.Btn_Min.Location = new System.Drawing.Point(207, 403);
            this.Btn_Min.Name = "Btn_Min";
            this.Btn_Min.Size = new System.Drawing.Size(75, 23);
            this.Btn_Min.TabIndex = 5;
            this.Btn_Min.Text = "最小化";
            this.Btn_Min.UseVisualStyleBackColor = true;
            this.Btn_Min.Click += new System.EventHandler(this.FrmMain_SizeChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(326, 446);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(72, 16);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "声音提示";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // Btn_ToolBox
            // 
            this.Btn_ToolBox.Location = new System.Drawing.Point(36, 432);
            this.Btn_ToolBox.Name = "Btn_ToolBox";
            this.Btn_ToolBox.Size = new System.Drawing.Size(160, 23);
            this.Btn_ToolBox.TabIndex = 7;
            this.Btn_ToolBox.Text = "工具箱";
            this.Btn_ToolBox.UseVisualStyleBackColor = true;
            this.Btn_ToolBox.Click += new System.EventHandler(this.Btn_ToolBox_Click);
            // 
            // CB_IsWriteToCCIM
            // 
            this.CB_IsWriteToCCIM.AutoSize = true;
            this.CB_IsWriteToCCIM.Location = new System.Drawing.Point(207, 446);
            this.CB_IsWriteToCCIM.Name = "CB_IsWriteToCCIM";
            this.CB_IsWriteToCCIM.Size = new System.Drawing.Size(108, 16);
            this.CB_IsWriteToCCIM.TabIndex = 8;
            this.CB_IsWriteToCCIM.Text = "向CCIM写入消息";
            this.CB_IsWriteToCCIM.UseVisualStyleBackColor = true;
            this.CB_IsWriteToCCIM.CheckedChanged += new System.EventHandler(this.CB_IsWriteToCCIM_CheckedChanged);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(425, 496);
            this.Controls.Add(this.CB_IsWriteToCCIM);
            this.Controls.Add(this.Btn_ToolBox);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.Btn_Min);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.Btn_Exit);
            this.Controls.Add(this.Btn_OpenData);
            this.Controls.Add(this.Btn_StartStop);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FrmMain";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "驰骋工作流程引擎服务";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.SizeChanged += new System.EventHandler(this.FrmMain_SizeChanged);
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
        private System.Windows.Forms.Button Btn_StartStop;
        private System.Windows.Forms.Button Btn_OpenData;
        private System.Windows.Forms.Button Btn_Exit;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button Btn_Min;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button Btn_ToolBox;
        private System.Windows.Forms.CheckBox CB_IsWriteToCCIM;
    }
}

