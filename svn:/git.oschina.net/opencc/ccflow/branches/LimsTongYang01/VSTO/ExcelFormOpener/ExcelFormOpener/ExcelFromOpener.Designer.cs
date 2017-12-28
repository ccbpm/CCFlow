namespace ExcelFormOpener
{
	partial class ExcelFromOpener
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
			this.txtInfo = new System.Windows.Forms.TextBox();
			this.btnRetry = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtInfo
			// 
			this.txtInfo.Location = new System.Drawing.Point(12, 12);
			this.txtInfo.Multiline = true;
			this.txtInfo.Name = "txtInfo";
			this.txtInfo.ReadOnly = true;
			this.txtInfo.Size = new System.Drawing.Size(798, 310);
			this.txtInfo.TabIndex = 1;
			// 
			// btnRetry
			// 
			this.btnRetry.Enabled = false;
			this.btnRetry.Location = new System.Drawing.Point(365, 328);
			this.btnRetry.Name = "btnRetry";
			this.btnRetry.Size = new System.Drawing.Size(95, 30);
			this.btnRetry.TabIndex = 2;
			this.btnRetry.Text = "重试";
			this.btnRetry.UseVisualStyleBackColor = true;
			this.btnRetry.Click += new System.EventHandler(this.btnRetry_Click);
			// 
			// ExcelFromOpener
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(822, 370);
			this.Controls.Add(this.btnRetry);
			this.Controls.Add(this.txtInfo);
			this.Name = "ExcelFromOpener";
			this.Text = "正在打开Excel表单...";
			this.Load += new System.EventHandler(this.ExcelFromOpener_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtInfo;
		private System.Windows.Forms.Button btnRetry;
	}
}

