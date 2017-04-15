using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.WF.Template;
using System.Data.SqlClient;

namespace CCFlow.WF.Admin.FindWorker
{
    public partial class NodeCCRole : System.Web.UI.Page
    {
        #region 属性.
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }

        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack == false)
            {
                BP.WF.Template.BtnLab btn = new BP.WF.Template.BtnLab(this.FK_Node);
                Node nd = new Node(this.FK_Node);
                BP.WF.Template.CC cc = new BP.WF.Template.CC();
                cc.NodeID = this.FK_Node;
                cc.RetrieveFromDBSources();

                BP.Web.Controls.Glo.DDL_BindEnum(this.DDL_CCRole, "CCRole", (int)btn.CCRole);
                BP.Web.Controls.Glo.DDL_BindEnum(this.DDL_CCWriteTo, "CCWriteTo", (int)nd.CCWriteTo);
                this.DDL_CCRole.SelectedValue = btn.CCRole.ToString();
                this.DDL_CCWriteTo.SelectedValue = nd.CCWriteTo.ToString();
                this.TB_Title.Text = cc.CCTitle;
                this.TB_CCDoc.Text = cc.CCDoc;
                this.CB_Station.Checked = cc.CCIsStations;

                //this.CB_IsZhiShu.Checked = cc.CCIsZhiShuShangJi; //是否是直属上级部门岗位？

                //抄送按照岗位计算方式.
                BP.Web.Controls.Glo.DDL_BindEnum(this.DDL_CCStaWay, CCAttr.CCStaWay, (int)cc.CCStaWay);

                this.CB_Dept.Checked = cc.CCIsDepts;
                this.CB_Emp.Checked = cc.CCIsEmps;

                this.CB_SQL.Checked = cc.CCIsSQLs;
              
                this.TB_SQL.Text = cc.CCSQL;
            }
        }

        protected void Btn_SaveAndClose_Click(object sender, EventArgs e)
        {
            this.Btn_Save_Click(null,null);
            BP.Sys.PubClass.WinClose();
        }

        protected void Btn_Close_Click(object sender, EventArgs e)
        {
            BP.Sys.PubClass.WinClose();
        }
        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            Node nd = new Node(this.FK_Node);
            //保存抄送规则
            nd.HisCCRole = (CCRole)(int.Parse(this.DDL_CCRole.SelectedValue));
            //保存写入规则           
            nd.CCWriteTo = (CCWriteTo)(int.Parse(this.DDL_CCWriteTo.SelectedValue));
            nd.DirectUpdate();

            BP.WF.Template.CC cc = new BP.WF.Template.CC();
            cc.NodeID = this.FK_Node;
            cc.Retrieve();

            cc.CCTitle = this.TB_Title.Text;
            cc.CCDoc = this.TB_CCDoc.Text;

            cc.CCIsStations = this.CB_Station.Checked; //到岗位.
            //抄送到岗位的计算方式?
            cc.CCStaWay = (CCStaWay)this.DDL_CCStaWay.SelectedIndex;

            cc.CCIsDepts = this.CB_Dept.Checked; //抄送到部门.
            cc.CCIsEmps = this.CB_Emp.Checked; // 抄送到人员.

            //抄送到sql
            cc.CCIsSQLs = this.CB_SQL.Checked;
            cc.CCSQL = this.TB_SQL.Text;

            if (this.CB_SQL.Checked && cc.CCSQL.Length == 0)
            {
                BP.Sys.PubClass.Alert("@您设置了按照SQL抄送，请您设置一个SQL语句，返回No,Name两个列,SQL支持表达式。");
                return;
            }

            cc.DirectUpdate();
        }
    }
}