using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.Admin.FindWorker
{
    public partial class Site : BP.Web.MasterPage 
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string pageID = this.PageID;

            string fk_flow = this.Request.QueryString["FK_Flow"];
            string FK_node = this.Request.QueryString["FK_Node"];

            this.Pub1.Add("<a href='List.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + FK_node + "' >返回列表</a>" + pageID);

            this.Pub1.MenuSelfBegin();

            if (pageID == "WorkFlow")
                this.Pub1.MenuSelfItem("WorkFlow.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + FK_node, "常用模式", "_self", true);
            else
                this.Pub1.MenuSelfItem("WorkFlow.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + FK_node, "常用模式", "_self", false);

            if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneMore)
            {
                if (pageID == "Leader")
                    this.Pub1.MenuSelfItem("Leader.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + FK_node, "找直线领导", "_self", true);
                else
                    this.Pub1.MenuSelfItem("Leader.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + FK_node, "找直线领导", "_self", false);

                if (pageID == "SpecEmps")
                    this.Pub1.MenuSelfItem("SpecEmps.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + FK_node, "找同事", "_self", true);
                else
                    this.Pub1.MenuSelfItem("SpecEmps.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + FK_node, "找同事", "_self", false);

                if (pageID == "ByDept")
                    this.Pub1.MenuSelfItem("ByDept.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + FK_node, "按部门", "_self", true);
                else
                    this.Pub1.MenuSelfItem("ByDept.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + FK_node, "按部门", "_self", false);

                if (pageID == "Etc")
                    this.Pub1.MenuSelfItem("Etc.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + FK_node, "特定人员", "_self", true);
                else
                    this.Pub1.MenuSelfItem("Etc.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + FK_node, "特定人员", "_self", false);
            }
            this.Pub1.MenuSelfEnd();
        }
    }
}