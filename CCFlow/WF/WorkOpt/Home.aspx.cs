using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;

namespace CCFlow.WF.WorkOpt
{
public partial class WF_WorkOpt_Home : System.Web.UI.Page
{
    public Int64 WorkID
    {
        get
        {
            return Int64.Parse(this.Request.QueryString["WorkID"]);
        }
    }
    public int FK_Node
    {
        get
        {
            return int.Parse(this.Request.QueryString["FK_Node"]);
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        this.Pub2.AddFieldSet("流程概况");

        Node nd12 = new Node(this.FK_Node);

        GenerWorkerLists wls = new GenerWorkerLists(this.WorkID, nd12.FK_Flow);
        Flow fl = new Flow(nd12.FK_Flow);

        this.Pub2.Add("<Table border=0 wdith=100% align=left>");
        this.Pub2.AddTR();
        this.Pub2.Add("<TD colspan=1 class=ToolBar  >流程名称:" + fl.Name + "</TD>");
        this.Pub2.AddTREnd();

        Nodes nds = fl.HisNodes;
        this.Pub2.AddTR();
        this.Pub2.Add("<TD colspan=1 class=BigDoc align=left >");
        foreach (BP.WF.Node nd in nds)
        {
            bool isHave = false;
            foreach (GenerWorkerList wl in wls)
            {
                if (wl.FK_Node == nd.NodeID)
                {
                    this.Pub2.Add("<font color=green>Step:" + nd.Step + nd.Name);
                    //this.Pub2.Add("<BR>执行人:" + wl.FK_EmpText);
                    this.Pub2.Add("<BR>: <img src='/DataUser/Siganture/" + wl.FK_Emp + ".jpg' border=0 onerror=\"this.src='/DataUser/Siganture/UnName.jpg'\"/>");
                    this.Pub2.Add("<br>" + wl.RDT + "</font><hr>");
                    //this.Pub2.Add("<a href='WFRpt.aspx?WorkID=" + wl.WorkID + "&FID=0&FK_Flow=" + this.FK_Flow + "' target=_blank >详细..</a><hr>");
                    isHave = true;
                    break;
                }
            }
            if (isHave)
                continue;
        }
        this.Pub2.AddTDEnd();
        this.Pub2.AddTREnd();
        this.Pub2.AddTableEnd();
        this.Pub2.AddFieldSetEnd();
    }
}
}