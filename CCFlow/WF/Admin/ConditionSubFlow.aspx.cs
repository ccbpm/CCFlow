using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;
using BP.WF.Template;

namespace CCFlow.WF.Admin
{
    public partial class ConditionSubFlow : BP.Web.WebPage
    {
        #region Property

        public string CondType
        {
            get { return "3"; }
        }

        public string FK_Flow
        {
            get { return Request.QueryString["FK_Flow"]; }
        }

        public string FK_MainNode
        {
            get { return Request.QueryString["FK_MainNode"]; }
        }

        public string FK_Node
        {
            get { return Request.QueryString["FK_Node"]; }
        }

        public string FK_Attr
        {
            get { return Request.QueryString["FK_Attr"]; }
        }

        public string DirType
        {
            get { return Request.QueryString["DirType"]; }
        }

        public string ToFlow
        {
            get { return Request.QueryString["ToFlow"]; }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            //获取以当前FK_Node为起点所能到达的所有结点方向信息

            //added by liuxc,2014.11.29
            var sql = new StringBuilder();

            FrmSubFlow sf = new FrmSubFlow( "ND"+this.FK_Node);

            if (sf.SFActiveFlows.Length <= 2)
            {
              //  this.Alert("您没有设置要启动的子流程，所以您不能设置触发子流程的条件。");
                this.WinCloseWithMsg("在节点【"+sf.NodeID+" "+sf.Name+"】上您没有设置要启动的子流程，所以您不能设置触发子流程的条件。");
                return;
            }


            BP.WF.Flows fls = new BP.WF.Flows();
            string[] strs = sf.SFActiveFlows.Split(',');
            foreach (string str in strs)
            {
                if (string.IsNullOrEmpty(str))
                    continue;

                BP.WF.Flow fl = new BP.WF.Flow(str);
                fls.AddEntity(fl);
            }

            //sql.AppendLine("SELECT wd.Node,");
            //sql.AppendLine("       wn2.Name AS NodeName,");
            //sql.AppendLine("       wd.ToNode,");
            //sql.AppendLine("       wn.Name AS ToNodeName,");
            //sql.AppendLine("       wd.DirType");
            //sql.AppendLine("FROM   WF_Direction wd");
            //sql.AppendLine("       INNER JOIN WF_Node wn");
            //sql.AppendLine("            ON  wn.NodeID = wd.ToNode");
            //sql.AppendLine("       INNER JOIN WF_Node wn2");
            //sql.AppendLine("            ON  wn2.NodeID = wd.Node");
            //sql.AppendLine("WHERE  wd.Node = " + FK_Node);

            rptLines.DataSource = fls.ToDataTableField(); // DBAccess.RunSQLReturnTable(sql.ToString());
            rptLines.DataBind();
        }
    }
}