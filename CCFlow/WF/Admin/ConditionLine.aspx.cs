using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;

namespace CCFlow.WF.Admin
{
    public partial class ConditionLine : System.Web.UI.Page
    {
        #region Property

        public string CondType
        {
            get { return Request.QueryString["CondType"]; }
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

        public string ToNodeId
        {
            get { return Request.QueryString["ToNodeId"]; }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            //获取以当前FK_Node为起点所能到达的所有结点方向信息
            //added by liuxc,2014.11.29
            var sql = new StringBuilder();
            sql.AppendLine("SELECT wd.Node,");
            sql.AppendLine("       wn2.Name AS NodeName,");
            sql.AppendLine("       wd.ToNode,");
            sql.AppendLine("       wn.Name AS ToNodeName,");
            sql.AppendLine("       wd.DirType");
            sql.AppendLine("FROM   WF_Direction wd");
            sql.AppendLine("       INNER JOIN WF_Node wn");
            sql.AppendLine("            ON  wn.NodeID = wd.ToNode");
            sql.AppendLine("       INNER JOIN WF_Node wn2");
            sql.AppendLine("            ON  wn2.NodeID = wd.Node");
            sql.AppendLine("WHERE  wd.Node = " + FK_Node);

            rptLines.DataSource = DBAccess.RunSQLReturnTable(sql.ToString());
            rptLines.DataBind();
        }
    }
}