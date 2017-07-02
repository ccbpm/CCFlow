namespace IndustrialAutomationWorkflow
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.Btn_Do = new System.Windows.Forms.Button();
            this.TB_Url = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TB_Para = new System.Windows.Forms.TextBox();
            this.TB_Info = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.TB_FlowNo = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.TB_StartUserNo = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Btn_Do
            // 
            this.Btn_Do.Location = new System.Drawing.Point(130, 130);
            this.Btn_Do.Name = "Btn_Do";
            this.Btn_Do.Size = new System.Drawing.Size(75, 23);
            this.Btn_Do.TabIndex = 0;
            this.Btn_Do.Text = "执行";
            this.Btn_Do.UseVisualStyleBackColor = true;
            this.Btn_Do.Click += new System.EventHandler(this.Btn_Do_Click);
            // 
            // TB_Url
            // 
            this.TB_Url.Location = new System.Drawing.Point(70, 35);
            this.TB_Url.Name = "TB_Url";
            this.TB_Url.Size = new System.Drawing.Size(334, 21);
            this.TB_Url.TabIndex = 1;
            this.TB_Url.Text = "http://localhost:24933/DataUser/IndustrialAutomationWorkflowAPI.asmx";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "WS地址";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 108);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "输入参数";
            // 
            // TB_Para
            // 
            this.TB_Para.Location = new System.Drawing.Point(71, 99);
            this.TB_Para.Name = "TB_Para";
            this.TB_Para.Size = new System.Drawing.Size(428, 21);
            this.TB_Para.TabIndex = 4;
            this.TB_Para.Text = "@N02=1@N03=1@N04=1@N06=1@ShiForCiPin=1";
            // 
            // TB_Info
            // 
            this.TB_Info.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.TB_Info.Location = new System.Drawing.Point(0, 168);
            this.TB_Info.Multiline = true;
            this.TB_Info.Name = "TB_Info";
            this.TB_Info.Size = new System.Drawing.Size(538, 206);
            this.TB_Info.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(480, 150);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "执行结果";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "要启动的流程编号";
            // 
            // TB_FlowNo
            // 
            this.TB_FlowNo.Location = new System.Drawing.Point(130, 66);
            this.TB_FlowNo.Name = "TB_FlowNo";
            this.TB_FlowNo.Size = new System.Drawing.Size(100, 21);
            this.TB_FlowNo.TabIndex = 8;
            this.TB_FlowNo.Text = "147";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(284, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "发起人员";
            // 
            // TB_StartUserNo
            // 
            this.TB_StartUserNo.Location = new System.Drawing.Point(343, 66);
            this.TB_StartUserNo.Name = "TB_StartUserNo";
            this.TB_StartUserNo.Size = new System.Drawing.Size(100, 21);
            this.TB_StartUserNo.TabIndex = 10;
            this.TB_StartUserNo.Text = "liyan";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 374);
            this.Controls.Add(this.TB_StartUserNo);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.TB_FlowNo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TB_Info);
            this.Controls.Add(this.TB_Para);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TB_Url);
            this.Controls.Add(this.Btn_Do);
            this.Name = "Form1";
            this.Text = "工业自动化流程测试";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Btn_Do;
        private System.Windows.Forms.TextBox TB_Url;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TB_Para;
        private System.Windows.Forms.TextBox TB_Info;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TB_FlowNo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox TB_StartUserNo;
    }
}

