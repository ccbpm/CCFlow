namespace CCFormExcel2010
{
	partial class RibbonCCFlow : Microsoft.Office.Tools.Ribbon.RibbonBase
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		public RibbonCCFlow()
			: base(Globals.Factory.GetRibbonFactory())
		{
			InitializeComponent();
		}

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

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tab1 = this.Factory.CreateRibbonTab();
			this.groupMain = this.Factory.CreateRibbonGroup();
			this.btnSaveFrm = this.Factory.CreateRibbonButton();
			this.group1 = this.Factory.CreateRibbonGroup();
			this.button1 = this.Factory.CreateRibbonButton();
			this.GetValByName = this.Factory.CreateRibbonEditBox();
			this.GetValByAddr = this.Factory.CreateRibbonEditBox();
			this.CurrentSelectionName = this.Factory.CreateRibbonButton();
			this.tab1.SuspendLayout();
			this.groupMain.SuspendLayout();
			this.group1.SuspendLayout();
			// 
			// tab1
			// 
			this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
			this.tab1.Groups.Add(this.groupMain);
			this.tab1.Groups.Add(this.group1);
			this.tab1.Label = "表单处理";
			this.tab1.Name = "tab1";
			// 
			// groupMain
			// 
			this.groupMain.Items.Add(this.btnSaveFrm);
			this.groupMain.Label = "常用操作";
			this.groupMain.Name = "groupMain";
			// 
			// btnSaveFrm
			// 
			this.btnSaveFrm.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
			this.btnSaveFrm.Description = "保存当前Excel表单到服务器";
			this.btnSaveFrm.Label = "保存　　　";
			this.btnSaveFrm.Name = "btnSaveFrm";
			this.btnSaveFrm.OfficeImageId = "FileSave";
			this.btnSaveFrm.ScreenTip = "保存";
			this.btnSaveFrm.ShowImage = true;
			this.btnSaveFrm.SuperTip = "保存当前Excel表单到服务器";
			this.btnSaveFrm.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnSaveFrm_Click);
			// 
			// group1
			// 
			this.group1.Items.Add(this.button1);
			this.group1.Items.Add(this.GetValByName);
			this.group1.Items.Add(this.GetValByAddr);
			this.group1.Items.Add(this.CurrentSelectionName);
			this.group1.Label = "调试用功能";
			this.group1.Name = "group1";
			// 
			// button1
			// 
			this.button1.Label = "区域信息";
			this.button1.Name = "button1";
			this.button1.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.button1_Click);
			// 
			// GetValByName
			// 
			this.GetValByName.Label = "通过命名获取值";
			this.GetValByName.Name = "GetValByName";
			this.GetValByName.Text = null;
			this.GetValByName.TextChanged += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.GetValByName_TextChanged);
			// 
			// GetValByAddr
			// 
			this.GetValByAddr.Label = "通过Addr获取值";
			this.GetValByAddr.Name = "GetValByAddr";
			this.GetValByAddr.Text = null;
			this.GetValByAddr.TextChanged += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.GetValByAddr_TextChanged);
			// 
			// CurrentSelectionName
			// 
			this.CurrentSelectionName.Label = "获取选区命名";
			this.CurrentSelectionName.Name = "CurrentSelectionName";
			this.CurrentSelectionName.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.CurrentSelectionName_Click);
			// 
			// RibbonCCFlow
			// 
			this.Name = "RibbonCCFlow";
			this.RibbonType = "Microsoft.Excel.Workbook";
			this.Tabs.Add(this.tab1);
			this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.RibbonCCFlow_Load);
			this.tab1.ResumeLayout(false);
			this.tab1.PerformLayout();
			this.groupMain.ResumeLayout(false);
			this.groupMain.PerformLayout();
			this.group1.ResumeLayout(false);
			this.group1.PerformLayout();

		}

		#endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupMain;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnSaveFrm;
		internal Microsoft.Office.Tools.Ribbon.RibbonButton button1;
		internal Microsoft.Office.Tools.Ribbon.RibbonGroup group1;
		internal Microsoft.Office.Tools.Ribbon.RibbonEditBox GetValByName;
		internal Microsoft.Office.Tools.Ribbon.RibbonEditBox GetValByAddr;
		internal Microsoft.Office.Tools.Ribbon.RibbonButton CurrentSelectionName;
	}

	partial class ThisRibbonCollection
	{
		internal RibbonCCFlow RibbonCCFlow
		{
			get { return this.GetRibbon<RibbonCCFlow>(); }
		}
	}
}
