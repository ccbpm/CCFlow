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
            if (!IsPostBack)
            {
                
                BP.WF.Template.BtnLab btn = new BP.WF.Template.BtnLab(this.FK_Node);
                Node nd = new Node(this.FK_Node);
                BP.WF.Template.CC cc = new BP.WF.Template.CC();
                cc.NodeID = this.FK_Node;
                cc.Retrieve();
                BP.Web.Controls.Glo.DDL_BindEnum(this.DDL_CCRole, "CCRole", (int)btn.CCRole);
                BP.Web.Controls.Glo.DDL_BindEnum(this.DDL_CCWriteTo, "CCWriteTo", (int)nd.CCWriteTo);
                this.DDL_CCRole.SelectedValue = btn.CCRole.ToString();
                this.DDL_CCWriteTo.SelectedValue = nd.CCWriteTo.ToString();
                this.TB_title_MB.Text = cc.CCTitle;
                this.TB_content_MB.Text = cc.CCDoc;
                if (cc.CCIsStations == true)
                {
                    this.CC_GW.Checked = true;
                }
                else 
                {
                    this.CC_GW.Checked = false;
                }
                if (cc.CCIsDepts == true)
                {
                    this.CC_DBM.Checked = true;
                }
                else 
                {
                    this.CC_DBM.Checked = false;
                }
                if (cc.CCIsEmps == true)
                {
                    this.CC_RY.Checked = true;
                }
                else 
                {
                    this.CC_RY.Checked = false;
                }
                if (cc.CCIsSQLs == true)
                {
                    this.CC_SQL.Checked = true;
                    this.CC_SQL1.Text = cc.CCSQL;
                }
                else
                {
                    this.CC_SQL.Checked = false;
                    this.CC_SQL1.Text = cc.CCSQL;
                }
            }

        }

        protected void Btn_SaveAndClose_Click(object sender, EventArgs e)
        {
            this.Save();
            BP.Sys.PubClass.WinClose();

        }

        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            this.Save();
        }

        protected void Btn_Close_Click(object sender, EventArgs e)
        {
            BP.Sys.PubClass.WinClose();
        }

        public void Save()
        {
            Node nd = new Node(this.FK_Node);
            //保存抄送规则
            nd.HisCCRole = (CCRole)(int.Parse(this.DDL_CCRole.SelectedValue));
            //保存写入规则           
            nd.CCWriteTo = (CCWriteTo)(int.Parse(this.DDL_CCWriteTo.SelectedValue));
            nd.DirectUpdate();
            //cha
            BP.WF.Template.CC cc = new BP.WF.Template.CC();
            cc.NodeID = this.FK_Node;
            cc.Retrieve();
            cc.CCTitle = this.TB_title_MB.Text;
            cc.CCDoc = this.TB_content_MB.Text;
            cc.DirectUpdate();
            if (CC_GW.Checked == true)
            {
                cc.CCIsStations = true;
                cc.DirectUpdate();
            }
            else 
            {
                cc.CCIsStations = false;
                cc.DirectUpdate();
            }
            if (CC_DBM.Checked == true)
            {
                cc.CCIsDepts = true;
                cc.DirectUpdate();
            }
            else {
                cc.CCIsDepts = false;
                cc.DirectUpdate();
            }
            if (CC_RY.Checked == true)
            {
                cc.CCIsEmps = true;
                cc.DirectUpdate();
            }
            else {
                cc.CCIsEmps = false;
                cc.DirectUpdate();
            }
            if (CC_SQL.Checked == true)
            {
                cc.CCIsSQLs = true;
                cc.CCSQL = this.CC_SQL1.Text;
                cc.DirectUpdate();
            }
            else 
            {
                cc.CCIsSQLs = false;
                cc.CCSQL = this.CC_SQL1.Text;
                cc.DirectUpdate();
            }
            
        }
    }
}