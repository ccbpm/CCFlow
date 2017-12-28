namespace CCFlowServices
{
    partial class FrmEfficiency
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
            this.TB_RunTimes = new System.Windows.Forms.TextBox();
            this.TB_Text = new System.Windows.Forms.TextBox();
            this.Btn_StartStop = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "运行次数";
            // 
            // TB_RunTimes
            // 
            this.TB_RunTimes.Location = new System.Drawing.Point(71, 6);
            this.TB_RunTimes.Name = "TB_RunTimes";
            this.TB_RunTimes.Size = new System.Drawing.Size(100, 21);
            this.TB_RunTimes.TabIndex = 1;
            this.TB_RunTimes.Text = "1000";
            // 
            // TB_Text
            // 
            this.TB_Text.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.TB_Text.Location = new System.Drawing.Point(0, 51);
            this.TB_Text.Multiline = true;
            this.TB_Text.Name = "TB_Text";
            this.TB_Text.Size = new System.Drawing.Size(516, 149);
            this.TB_Text.TabIndex = 3;
            // 
            // Btn_StartStop
            // 
            this.Btn_StartStop.Location = new System.Drawing.Point(194, 6);
            this.Btn_StartStop.Name = "Btn_StartStop";
            this.Btn_StartStop.Size = new System.Drawing.Size(226, 23);
            this.Btn_StartStop.TabIndex = 4;
            this.Btn_StartStop.Text = "运行3节点-数据合并模式";
            this.Btn_StartStop.UseVisualStyleBackColor = true;
            this.Btn_StartStop.Click += new System.EventHandler(this.Btn_RunIt_Click);
            // 
            // FrmEfficiency
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 200);
            this.Controls.Add(this.Btn_StartStop);
            this.Controls.Add(this.TB_Text);
            this.Controls.Add(this.TB_RunTimes);
            this.Controls.Add(this.label1);
            this.Name = "FrmEfficiency";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "效率测试";
            this.Load += new System.EventHandler(this.FrmEfficiency_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TB_RunTimes;
        private System.Windows.Forms.TextBox TB_Text;
        private System.Windows.Forms.Button Btn_StartStop;
    }
}