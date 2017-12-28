namespace CCFlowServices
{
    partial class ToolBox
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
            this.Btn_Close = new System.Windows.Forms.Button();
            this.Btn_DTSNDxxxRpt = new System.Windows.Forms.Button();
            this.Btn_Imp = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.Btn_ChOfNode = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Btn_Close
            // 
            this.Btn_Close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Btn_Close.Location = new System.Drawing.Point(475, 214);
            this.Btn_Close.Name = "Btn_Close";
            this.Btn_Close.Size = new System.Drawing.Size(75, 23);
            this.Btn_Close.TabIndex = 1;
            this.Btn_Close.Text = "Close";
            this.Btn_Close.UseVisualStyleBackColor = true;
            this.Btn_Close.Click += new System.EventHandler(this.Btn_Close_Click);
            // 
            // Btn_DTSNDxxxRpt
            // 
            this.Btn_DTSNDxxxRpt.Location = new System.Drawing.Point(41, 68);
            this.Btn_DTSNDxxxRpt.Name = "Btn_DTSNDxxxRpt";
            this.Btn_DTSNDxxxRpt.Size = new System.Drawing.Size(134, 23);
            this.Btn_DTSNDxxxRpt.TabIndex = 2;
            this.Btn_DTSNDxxxRpt.Text = "调度NDRpt数据";
            this.Btn_DTSNDxxxRpt.UseVisualStyleBackColor = true;
            this.Btn_DTSNDxxxRpt.Click += new System.EventHandler(this.Btn_DTSNDxxxRpt_Click);
            // 
            // Btn_Imp
            // 
            this.Btn_Imp.Location = new System.Drawing.Point(41, 29);
            this.Btn_Imp.Name = "Btn_Imp";
            this.Btn_Imp.Size = new System.Drawing.Size(119, 23);
            this.Btn_Imp.TabIndex = 8;
            this.Btn_Imp.Text = "执行导入流程数据";
            this.Btn_Imp.UseVisualStyleBackColor = true;
            this.Btn_Imp.Click += new System.EventHandler(this.Btn_Imp_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 240);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(550, 22);
            this.statusStrip1.TabIndex = 9;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // Btn_ChOfNode
            // 
            this.Btn_ChOfNode.Location = new System.Drawing.Point(41, 111);
            this.Btn_ChOfNode.Name = "Btn_ChOfNode";
            this.Btn_ChOfNode.Size = new System.Drawing.Size(134, 23);
            this.Btn_ChOfNode.TabIndex = 10;
            this.Btn_ChOfNode.Text = "调度WF_ChOfNode数据";
            this.Btn_ChOfNode.UseVisualStyleBackColor = true;
            this.Btn_ChOfNode.Click += new System.EventHandler(this.Btn_ChOfNode_Click);
            // 
            // ToolBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Btn_Close;
            this.ClientSize = new System.Drawing.Size(550, 262);
            this.Controls.Add(this.Btn_ChOfNode);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.Btn_Imp);
            this.Controls.Add(this.Btn_DTSNDxxxRpt);
            this.Controls.Add(this.Btn_Close);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ToolBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ToolBox";
            this.Load += new System.EventHandler(this.ToolBox_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Btn_Close;
        private System.Windows.Forms.Button Btn_DTSNDxxxRpt;
        private System.Windows.Forms.Button Btn_Imp;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Button Btn_ChOfNode;

    }
}