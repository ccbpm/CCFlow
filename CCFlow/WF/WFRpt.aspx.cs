using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace CCFlow.WF
{
    public partial class WF_WFRpt : BP.Web.WebPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string fk_flow = this.Request.QueryString["FK_Flow"];
            string fk_node = this.Request.QueryString["FK_Node"];
            string workid = this.Request.QueryString["WorkID"];
            string fid = this.Request.QueryString["FID"];

            //在途链接 默认显示轨迹图tab页面   by tianbaoyan 2016-11-28
            string redirectURL = "./WorkOpt/OneWork/OneWork.htm?CurrTab=Truck&FK_Flow=";

            if (this.Request.QueryString["DoType"] == "CC")
            {
                if (this.Request.QueryString["CCSta"] != "0")
                {
                    BP.WF.Template.CCList cc = new BP.WF.Template.CCList();
                    cc.MyPK = this.Request.QueryString["CCID"];
                    cc.RetrieveFromDBSources();
                    if (cc.HisSta == BP.WF.Template.CCSta.UnRead)
                    {
                        cc.HisSta = BP.WF.Template.CCSta.Read;
                        cc.Update();
                    }
                }
                this.Response.Redirect(redirectURL +BP.WF.Dev2Interface.TurnFlowMarkToFlowNo(fk_flow) + "&FK_Node=" + fk_node + "&WorkID=" + workid + "&FID=" + fid, true);
                return;
            }

            if (this.Request.QueryString["DoType"] != null)
                return;

            if (this.Request.QueryString["ViewWork"] != null)
                return;

            this.Response.Redirect(redirectURL + BP.WF.Dev2Interface.TurnFlowMarkToFlowNo(fk_flow) + "&FK_Node=" + fk_node + "&WorkID=" + workid + "&FID=" + fid, true);
            return;
        }
    }

}