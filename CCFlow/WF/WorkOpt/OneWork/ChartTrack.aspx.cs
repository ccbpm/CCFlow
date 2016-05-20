using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
namespace CCFlow.WF.OneWork
{
public partial class WF_WorkOpt_OneWork_ChartTrack : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            var workId = Request.QueryString["WorkID"];
            var fk_flow = Request.QueryString["FK_Flow"];
            var fid = Request.QueryString["FID"];
            string url = string.Empty;

            url = "ChartTrack.htm?FID=" + fid + "&FK_Flow=" + fk_flow + "&WorkID=" + workId;
            //content.Attributes.Add("src", url);
        }
        catch (Exception ee) {
            Response.Write("轨迹图加载错误："+ee.Message);
        }
    }
}
}