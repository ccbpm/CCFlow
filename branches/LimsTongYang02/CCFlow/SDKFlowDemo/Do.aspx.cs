using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class SDKFlows_Do : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        switch (this.Request.QueryString["DoType"])
        {
            case "DelInfo":
                this.Response.Write("执行成功。");
                break;
            default:
                break;
        }

    }

    //public void TurnTo2Node(string fk_flow, int FK_NodeSheetfJump,Int64 fromOID, Int64 fromFID)
    //{
    //    //发起一下新流程直接跳转到103节点上去。
    //    // 注意: 103 节点的人员接受规则是，与上一步的操作员一致。
    //    BP.WF.Dev2Interface.Node_StartWork("001", null, 103);
    //}
}