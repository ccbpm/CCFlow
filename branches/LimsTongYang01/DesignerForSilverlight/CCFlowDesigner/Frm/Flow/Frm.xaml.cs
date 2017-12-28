using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Silverlight;
using WF.WS;


namespace BP.Frm
{
    public partial class Frm : ChildWindow
    {
        public MainPage HisMainPage = null;
        public AppType HisAppType = AppType.Application;
        public bool IsNew = false;
        public string SortNo { get; set; }
        public Frm()
        {
            InitializeComponent();
            this.MouseRightButtonDown += (sender, e) =>
            {
                e.Handled = true;
            };
        }
        public void BindNew()
        {
            this.IsNew = false;
            string sqls = "SELECT * FROM Sys_FormTree ORDER BY Name";
            sqls += "@SELECT * FROM Sys_MapData WHERE No='ssss'";
            sqls += "@SELECT No,Name FROM Sys_SFDBSrc WHERE DBSrcType!=100";
            sqls += "@SELECT IntKey as No, Lab as Name FROM Sys_Enum WHERE EnumKey='FrmType' ";

           // this.TB_No.IsEnabled = false;
            WSDesignerSoapClient daBindFrm = Glo.GetDesignerServiceInstance();
            daBindFrm.RunSQLReturnTableSAsync(sqls, Glo.UserNo, Glo.SID);
            daBindFrm.RunSQLReturnTableSCompleted += new EventHandler<RunSQLReturnTableSCompletedEventArgs>(daBindFrm_RunSQLReturnTableSCompleted);
        }
        void daBindFrm_RunSQLReturnTableSCompleted(object sender, RunSQLReturnTableSCompletedEventArgs e)
        {
            this.Btn_Save.IsEnabled = false;

            DataSet ds = new DataSet();
            ds.FromXml(e.Result);

            //表单目录.
            DataTable dtSort = ds.Tables[0];
            if (dtSort.Rows.Count == 0)
            {
                DataRow dr = dtSort.NewRow();
                dr[0] = "01";
                dr[1] = "默认类别";
                dtSort.Rows.Add(dr);
            }

            //数据源
            DataTable dtDBSrc = ds.Tables[2];
            if (dtDBSrc.Rows.Count == 0)
            {
                DataRow dr = dtDBSrc.NewRow();
                dr[0] = "local";
                dr[1] = "应用系统主数据库(默认)";
                dtDBSrc.Rows.Add(dr);
            }

            //表单目录.
            Glo.Ctrl_DDL_BindDataTable(this.DDL_FrmSort, dtSort, SortNo); //类别.

            //数据源.
            Glo.Ctrl_DDL_BindDataTable(this.DDL_SFDBSrc, dtDBSrc, "local");

            //表单类型.
            Glo.Ctrl_DDL_BindDataTable(this.DDL_FrmType, ds.Tables[3], "1");

            this.HisAppType = AppType.Application;
            this.Btn_Save.IsEnabled = true;
            return;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            string error = "";
            if (string.IsNullOrEmpty(this.TB_No.Text.Trim()))
            {
                MessageBox.Show("编号不能为空.");
                return;
            }

            if (string.IsNullOrEmpty(this.TB_Name.Text.Trim()))
            {
                MessageBox.Show("名称不能为空.");
                return;
            }
            
            //判断编号不能包含汉字
            if (BP.SL.Glo.ContainsChinese(this.TB_No.Text))
            {
                MessageBox.Show("编号 不能使用汉字！");
                return;
            }

            //判断表名称不能包含汉字
            if (BP.SL.Glo.ContainsChinese(this.TB_PTable.Text))
            {
                MessageBox.Show("表名称 不能使用汉字！");
                return;
            }

            string strs = "";
            strs += "@EnName=BP.Sys.MapData@PKVal=" + this.TB_No.Text;
            strs += "@No=" + this.TB_No.Text;
            strs += "@Name=" + this.TB_Name.Text;
            strs += "@PTable=" + this.TB_PTable.Text;
            strs += "@Url=" + this.TB_URL.Text;

            ListBoxItem lb = this.DDL_FrmSort.SelectedItem as ListBoxItem;
            if (lb != null)
                strs += "@FK_FrmSort=" + lb.Tag.ToString();
            if (lb != null)
                strs += "@FK_FormTree=" + lb.Tag.ToString();

            lb = this.DDL_FrmType.SelectedItem as ListBoxItem;
            if (lb != null)
                strs += "@FrmType=" + lb.Tag.ToString();

            lb = this.DDL_SFDBSrc.SelectedItem as ListBoxItem;
            if (lb != null)
                strs += "@SFDBSrc=" + lb.Tag.ToString();

            strs += "@AppType=" + (int)this.HisAppType;
            strs += "@Designer=" + this.TB_Designer.Text;
            strs += "@DesignerContact=" + this.TB_DesignerContact.Text;
            strs += "@DesignerUnit=" + this.TB_DesignerUnit.Text;

            WSDesignerSoapClient daSaveEn = Glo.GetDesignerServiceInstance();
            daSaveEn.SaveEnAsync(strs);
            daSaveEn.SaveEnCompleted += new EventHandler<SaveEnCompletedEventArgs>(daSaveEn_SaveEnCompleted);
        }
        void daSaveEn_SaveEnCompleted(object sender, SaveEnCompletedEventArgs e)
        {
            this.Btn_Save.IsEnabled = true;
            if (e.Result.Contains("Err"))
            {
                MessageBox.Show(e.Result, "保存错误", MessageBoxButton.OK);
            }
            else
            {
                this.HisMainPage.BindFormTree();
                this.DialogResult = true;
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.HisMainPage.BindFormTree();
            this.DialogResult = false;
        }
        private void DDL_FrmType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SetState();
        }
        public void SetState()
        {
            try
            {
                this.TB_PTable.IsEnabled = true;
                this.DDL_SFDBSrc.IsEnabled = true;
                this.TB_URL.IsEnabled = true;
                if (this.DDL_FrmType.SelectedIndex == 3)
                {
                    /*自定义表单*/
                    this.TB_PTable.IsEnabled = false;
                    this.DDL_SFDBSrc.IsEnabled = false;
                    this.TB_URL.IsEnabled = true;
                }
                else
                {
                    this.TB_PTable.IsEnabled = true;
                    this.DDL_SFDBSrc.IsEnabled = true;
                    this.TB_URL.IsEnabled = false;
                }
            }
            catch
            {
            }
        }
        private void TB_Name_LostFocus(object sender, RoutedEventArgs e)
        {
            string s = this.TB_Name.Text;
            var daPinYin = Glo.GetDesignerServiceInstance();
            daPinYin.ParseStringToPinyinAsync(s);
            daPinYin.ParseStringToPinyinCompleted += new EventHandler<ParseStringToPinyinCompletedEventArgs>(daPinYin_ParseStringToPinyinCompleted);
        }

        void daPinYin_ParseStringToPinyinCompleted(object sender, ParseStringToPinyinCompletedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.TB_No.Text.Trim()) == true)
            {
                this.TB_No.Text = e.Result;
            }

            if (string.IsNullOrEmpty(this.TB_PTable.Text.Trim()) == true)
            {
                this.TB_PTable.Text = e.Result;
            }
        }
    }
}

