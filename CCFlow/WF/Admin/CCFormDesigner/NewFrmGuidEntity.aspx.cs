using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;

namespace CCFlow.WF.Admin.CCFormDesigner
{
    public partial class NewFrmGuidEntity : System.Web.UI.Page
    {
        #region 参数
        public int Step
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["Step"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        public int FrmType
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["FrmType"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 数据源
        /// </summary>
        public string DBSrc
        {
            get
            {
                string val = this.Request.QueryString["DBSrc"];
                if (val == null || val == "")
                    return "local";
                return val;
            }
        }
        /// <summary>
        /// 表单类别
        /// </summary>
        public string FK_FrmSort
        {
            get
            {
                return this.Request.QueryString["FK_FrmSort"];
            }
        }
        #endregion 参数
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Step == 1)
                BindList();
        }
        public void BindList()
        {
            System.Data.DataTable dt = BP.DA.DBAccess.RunSQLReturnTable("SELECT No,Name,ParentNo FROM Sys_FormTree WHERE DBSrc='local' AND IsDir='0'");
            this.DDL_FrmTree.DataSource = dt;
            this.DDL_FrmTree.DataTextField = "Name";
            this.DDL_FrmTree.DataValueField = "No";
            this.DDL_FrmTree.DataBind();
        }
        public void tb_TextChanged(object sender, EventArgs e)
        {
            string name = this.TB_Name.Text;
            string val = "";

            if (this.RB0.Checked)
                val = BP.DA.DataType.ParseStringToPinyin(name);
            else
                val = BP.DA.DataType.ParseStringToPinyinJianXie(name);

            this.TB_No.Text = val;
            this.TB_PTable.Text = "CCFrom_" + val;
        }

        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            MapData md = new MapData();
            md.Name = this.TB_Name.Text;
            md.No = this.TB_No.Text;
            md.PTable = this.TB_PTable.Text;
            md.FK_FrmSort = this.DDL_FrmTree.SelectedValue;
            md.FK_FormTree = this.DDL_FrmTree.SelectedValue;
            md.AppType = "0";//独立表单
            md.DBSrc = this.DBSrc;

            if (md.Name.Length == 0 || md.No.Length == 0 || md.PTable.Length == 0)
            {
                BP.Sys.PubClass.Alert("必填项不能为空.");
                return;
            }

            if (md.IsExits == true)
            {
                BP.Sys.PubClass.Alert("表单ID:" + md.No + "已经存在.");
                return;
            }
            md.HisFrmTypeInt = this.FrmType; //表单类型.

            md.Insert();

            if (md.HisFrmType == BP.Sys.FrmType.FreeFrm)
            {
                this.Response.Redirect("FormDesigner.aspx?FK_MapData=" + md.No);
            }
        }
    }
}