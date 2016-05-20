using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF.Template;
using BP.WF;
using BP.Port;
using BP.Sys;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;

namespace CCFlow.WF.Admin
{
    public partial class WF_Admin_CondPRI : BP.Web.WebPage
    {
        #region 属性
        /// <summary>
        /// 主键
        /// </summary>
        public new string MyPK
        {
            get
            {
                return this.Request.QueryString["MyPK"];
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public int FK_MainNode
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_MainNode"]);
            }
        }
        public int ToNodeID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["ToNodeID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {
            switch (this.DoType)
            {
                case "Up":
                    Cond up = new Cond(this.MyPK);
                    up.DoUp(this.FK_MainNode);
                    up.RetrieveFromDBSources();
                    DBAccess.RunSQL("UPDATE WF_Cond SET PRI=" + up.PRI + " WHERE ToNodeID=" + up.ToNodeID);
                    break;
                case "Down":
                    Cond down = new Cond(this.MyPK);
                    down.DoDown(this.FK_MainNode);
                    down.RetrieveFromDBSources();
                    DBAccess.RunSQL("UPDATE WF_Cond SET PRI=" + down.PRI + " WHERE ToNodeID=" + down.ToNodeID);
                    break;
                default:
                    break;
            }

            BP.WF.Node nd = new BP.WF.Node(this.FK_MainNode);

            this.Pub1.AddTable("class='Table' cellSpacing='1' cellPadding='1'  border='1' style='width:100%'");
            this.Pub1.AddTR();
            this.Pub1.AddTD("class='GroupTitle' colspan='7'", nd.Name);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("class='GroupTitle'", "从节点ID");
            this.Pub1.AddTD("class='GroupTitle'", "从节点名称");
            this.Pub1.AddTD("class='GroupTitle'", "到节点ID");
            this.Pub1.AddTD("class='GroupTitle'", "到节点名称");
            this.Pub1.AddTD("class='GroupTitle'", "优先级");
            this.Pub1.AddTD("class='GroupTitle' colspan=2", "操作");
            this.Pub1.AddTREnd();

            Conds cds = new Conds();
            //BP.En.QueryObject qo = new QueryObject(cds);
            //qo.AddWhere(CondAttr.FK_Node, this.FK_MainNode);
            //qo.addAnd();
            //qo.AddWhere(CondAttr.FK_Node, this.FK_MainNode);

            cds.Retrieve(CondAttr.FK_Node, this.FK_MainNode, CondAttr.CondType, 2, CondAttr.PRI);
            string strs = "";

            foreach (Cond cd in cds)
            {
                if (strs.Contains("," + cd.ToNodeID.ToString()))
                    continue;

                strs += "," + cd.ToNodeID.ToString();

                BP.WF.Node mynd = new BP.WF.Node(cd.ToNodeID);

                this.Pub1.AddTR();
                this.Pub1.AddTD(nd.NodeID);
                this.Pub1.AddTD(nd.Name);
                this.Pub1.AddTD(mynd.NodeID);
                this.Pub1.AddTD(mynd.Name);
                this.Pub1.AddTD(cd.PRI);
                this.Pub1.AddTD("<a href='CondPRI.aspx?CondType=2&DoType=Up&FK_Flow=" + this.FK_Flow + "&FK_MainNode=" + this.FK_MainNode + "&ToNodeID=" + this.ToNodeID + "&MyPK=" + cd.MyPK + "' class='easyui-linkbutton' data-options=\"iconCls:'icon-up'\">上移</a>");
                this.Pub1.AddTD("<a href='CondPRI.aspx?CondType=2&DoType=Down&FK_Flow=" + this.FK_Flow + "&FK_MainNode=" + this.FK_MainNode + "&ToNodeID=" + this.ToNodeID + "&MyPK=" + cd.MyPK + "' class='easyui-linkbutton' data-options=\"iconCls:'icon-down'\">下移</a>");
                this.Pub1.AddTREnd();
            }

            this.Pub1.AddTableEnd();
            this.Pub1.AddBR();

            string help = "";
            help += "<ul>";
            help += "<li>在转向中，如果出现一个以上的路线都成立时时，系统就会按照第一个路线来计算，那一个排列最前面就按照那一个计算。</li>";
            help += "<li>例如：在demo中002.请假流程，如果一个人员既有基层岗，也有中层岗那么到达基层与中层的路线都会成立，如果设置了方向条件的优先级，系统就会按照优先满足的条件的路线计算。</li>";
            help += "</ul>";

            this.Pub1.AddEasyUiPanelInfo("帮助", "<span style='font-weight:bold'>什么是方向条件的优先级？</span><br />" + Environment.NewLine
                + help, "icon-help");
        }
    }
}