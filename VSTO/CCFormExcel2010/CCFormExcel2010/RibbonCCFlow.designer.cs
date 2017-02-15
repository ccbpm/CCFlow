namespace CCFlowExcel
{
    partial class RibbonCCFlow : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public RibbonCCFlow()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

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

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tab1 = this.Factory.CreateRibbonTab();
            this.groupMain = this.Factory.CreateRibbonGroup();
            this.btnSaveFrm = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.groupMain.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(this.groupMain);
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

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupMain;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnSaveFrm;
    }

    partial class ThisRibbonCollection
    {
        internal RibbonCCFlow RibbonCCFlow
        {
            get { return this.GetRibbon<RibbonCCFlow>(); }
        }
    }
}
