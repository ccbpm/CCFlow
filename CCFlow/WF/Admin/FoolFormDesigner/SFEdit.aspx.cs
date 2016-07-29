using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.DA;

namespace CCFlow.WF.Admin.FoolFormDesigner
{
    public partial class SFEdit : BP.Web.WebPage
    {
        #region 属性.
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        public string FK_SFTable
        {
            get
            {
                return this.Request.QueryString["FK_SFTable"];
            }
        }

        public string FromApp
        {
            get
            {
                return this.Request.QueryString["FromApp"];
            }
        }
        
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack == false)
            {
                this.RB_0.Checked = true;

                //数据库服务.
                SFDBSrcs srcs = new SFDBSrcs();
                srcs.RetrieveDBSrc();
                BP.Web.Controls.Glo.DDL_BindEns(this.DDL_Table_DBSrc, srcs, null);
                BP.Web.Controls.Glo.DDL_BindEns(this.DDL_SQL_DBSrc, srcs, null);

                //WS服务.
                srcs = new SFDBSrcs();
                srcs.RetrieveWCSrc();
                BP.Web.Controls.Glo.DDL_BindEns(this.DDL_WS_DBSrc, srcs, null);


                //绑定编码结构.
                BP.Web.Controls.Glo.DDL_BindEnum(this.DDL_Table_CodeStruct, SFTableAttr.CodeStruct, 0);
                BP.Web.Controls.Glo.DDL_BindEnum(this.DDL_SQL_CodeStruct, SFTableAttr.CodeStruct, 0);
                BP.Web.Controls.Glo.DDL_BindEnum(this.DDL_WS_CodeStruct, SFTableAttr.CodeStruct, 0);
            }

        }

        protected void TB_Table_Name_TextChanged(object sender, EventArgs e)
        {
            string name = this.TB_Table_Name.Text;
            if (string.IsNullOrEmpty(name))
                return ;
            this.TB_Table_No.Text ="SF_"+ BP.DA.DataType.ParseStringToPinyin(name);
        }

        protected void Btn_Create_Click(object sender, EventArgs e)
        {
            string no, name="";
            SFTable sf = new SFTable();

            #region 创建数据表.
            if (this.RB_0.Checked)
            {
                no = this.TB_Table_No.Text;
                name = this.TB_Table_Name.Text;

                if (no.Length == 0 || name.Length == 0)
                {
                    this.Alert("数据表的编号或者名称不能为空.");
                    return;
                }

                sf.No = no;
                if (sf.IsExits == true)
                {
                    this.Alert("数据表的编号为["+sf.No+"]，已经存在。.");
                    return;
                }

                sf.Name = name;
                sf.FK_SFDBSrc = this.DDL_Table_DBSrc.SelectedValue;
                sf.FK_Val = no;
                sf.CodeStruct = (CodeStruct)this.DDL_Table_CodeStruct.SelectedIndex;
                sf.Insert();
                 
                this.Response.Redirect("EditTable.aspx?FK_MapData=" + this.FK_MapData + "&FK_SFTable=" + this.FK_SFTable + "&FromApp=" + this.FromApp, true);
            }
            #endregion 创建数据表.

        }

        protected void Btn_Close_Click(object sender, EventArgs e)
        {
            this.WinClose();
        }
    }
}