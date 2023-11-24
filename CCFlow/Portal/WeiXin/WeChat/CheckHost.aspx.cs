using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Cloud.WeXinAPI;

namespace CCFlow.Admin.WeChat
{
    public partial class CheckHost : System.Web.UI.Page
    {
        /// <summary>
        /// 用来效验服务器与腾讯是否可以连接.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //构造函数.
            Tencent.WXBizMsgCrypt wxcpt = new Tencent.WXBizMsgCrypt(Glo.Token,
            Glo.EncodingAESKey, Glo.CorpID);

            //sxx
            string sVerifyMsgSig = HttpUtility.UrlDecode(this.Request["msg_signature"] ?? string.Empty);
            string sVerifyTimeStamp = HttpUtility.UrlDecode(this.Request["timestamp"] ?? string.Empty);
            string sVerifyNonce = HttpUtility.UrlDecode(this.Request["nonce"] ?? string.Empty);
            string sVerifyEchoStr = HttpUtility.UrlDecode(this.Request["echostr"] ?? string.Empty).Replace(" ", "+");
            //验证回调URL
            if (!string.IsNullOrEmpty(sVerifyEchoStr))
            {
                int ret = 0;
                string sEchoStr = string.Empty;
                ret = wxcpt.VerifyURL(sVerifyMsgSig, sVerifyTimeStamp, sVerifyNonce,
                    sVerifyEchoStr, ref sEchoStr);

                if (ret != 0)
                {
                    this.Response.Write("ERR: VerifyURL fail, ret: " + ret);
                    return;
                    //System.Console.WriteLine();
                }

                //ret==0表示验证成功，sEchoStr参数表示明文，用户需要将sEchoStr作为get请求的返回参数，返回给企业微信。
                //this.Response.Write(sEchoStr);
                this.Response.Write(sEchoStr);
                this.Response.End();
            }
        }
    }
}