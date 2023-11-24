using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;
using BP.DA;
using BP.Cloud.WeXinAPI;
using BP.Port;
using BP.Sys;
using Glo = BP.Cloud.WeXinAPI.Glo;

namespace CCFlow.Admin.WeChat
{
    public partial class Callme : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            string sVerifyMsgSig = BP.Cloud.WeXinAPI.Glo.getValue("msg_signature");
            string sVerifyTimeStamp = BP.Cloud.WeXinAPI.Glo.getValue("timestamp");
            string sVerifyNonce = BP.Cloud.WeXinAPI.Glo.getValue("nonce");
            string sVerifyEchoStr = BP.Cloud.WeXinAPI.Glo.getValue("echostr").Replace(" ", "+");

            if (Request.RequestType.ToUpper().Equals("POST") == false)
            {
                Tencent.WXBizMsgCrypt wxcpt = new Tencent.WXBizMsgCrypt(Glo.Token,
                Glo.EncodingAESKey, Glo.CorpID);

                //验证回调URL
                if (string.IsNullOrEmpty(sVerifyEchoStr) == false)
                {
                    string sEchoStr = string.Empty;
                    int ret = wxcpt.VerifyURL(sVerifyMsgSig, sVerifyTimeStamp, sVerifyNonce,
                        sVerifyEchoStr, ref sEchoStr);

                    if (ret != 0)
                        System.Console.WriteLine("ERR: VerifyURL fail, ret: " + ret);

                    //ret==0表示验证成功，sEchoStr参数表示明文，用户需要将sEchoStr作为get请求的返回参数，返回给企业微信。
                    //this.Response.Write(sEchoStr);
                    HttpContext.Current.Response.Write(sEchoStr);
                    HttpContext.Current.Response.End();
                }

                //授权设置.
                //glo.getPreAuthCode();
                return;
            }

            //推送suite_ticket、授权成功通知、组织变更通知、人员变更通知，用于获取suite_access_token。
            string msg = "";
            try
            {
                msg = MainBuessPage();
            }
            catch (Exception ex)
            {
                msg = "err@" + ex.Message;
            }

           /* if (msg== "success")
            {
                return;
            }
            HttpContext.Current.Response.Charset = "UTF-8";
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
            HttpContext.Current.Response.ContentType = "text/html";
            HttpContext.Current.Response.Expires = 0;
            HttpContext.Current.Response.Write("success");
            HttpContext.Current.Response.End();*/
        }
        /// <summary>
        /// 推送suite_ticket、授权成功通知、组织变更通知、人员变更通知等通知事件推送
        /// </summary>
        public string MainBuessPage()
        {
            string sMsg = "";
            string sVerifyMsgSig = BP.Cloud.WeXinAPI.Glo.getValue("msg_signature");
            string sVerifyTimeStamp = BP.Cloud.WeXinAPI.Glo.getValue("timestamp");
            string sVerifyNonce = BP.Cloud.WeXinAPI.Glo.getValue("nonce");
            string postStrs = PostInput();

            XmlDocument xd = new XmlDocument();
            Tencent.WXBizMsgCrypt wxcpt = new Tencent.WXBizMsgCrypt(Glo.Token, Glo.EncodingAESKey, Glo.SuiteID);
            int code = wxcpt.DecryptMsg(sVerifyMsgSig, sVerifyTimeStamp, sVerifyNonce, postStrs, ref sMsg);
            if (code != 0)
                return "err@错误代码:" + code;
            xd.LoadXml(sMsg);

            //推送suite_ticket协议每十分钟微信推送一次，判断是推送ticket的情况，取得ticket保存到缓存中
            if (xd.FirstChild["InfoType"].InnerText.Equals("suite_ticket"))
            {
                string suitTicket = xd.FirstChild["SuiteTicket"].InnerText;
                Glo.Suite_ticket = suitTicket;
                HttpContext.Current.Cache.Insert("suitTicket", suitTicket);
                HttpContext.Current.Response.Write("success");
                BP.DA.Log.DebugWriteInfo("suitTicket" + suitTicket);

                return "success";
            }
            //授权成功通知回调
            if (xd.FirstChild["InfoType"].InnerText.Equals("create_auth"))
            {
                //授权成功通知回调推送给服务商
                string authCode = xd.FirstChild["AuthCode"].InnerText;

                HttpContext.Current.Cache.Insert("authcode", authCode);
                //根据临时授权码，获得永久授权码并安装应用
                BP.Cloud.WeXinAPI.Glo.InstallIt(authCode);
                HttpContext.Current.Response.Write("success");
                return "success";
            }
            //取消安装的，安装修改.
            if (xd.FirstChild["InfoType"].InnerText.Equals("cancel_auth"))
            {
                string authCorpId = xd.FirstChild["AuthCorpId"].InnerText;

                BP.Cloud.Org org = new BP.Cloud.Org();
                int i = org.Retrieve("CorpID", authCorpId);
                if (i == 1)
                    org.DoDelete();
                HttpContext.Current.Response.Write("success");
                return "success";
            }

            //授权变更.
            if (xd.FirstChild["InfoType"].InnerText.Equals("change_auth"))
            {
                string authCode = xd.FirstChild["AuthCode"].InnerText;
                HttpContext.Current.Cache.Insert("authcode", authCode);
                HttpContext.Current.Response.Write("success");
                return "success";
            }
            //通讯录变更
            if (xd.FirstChild["InfoType"].InnerText.Equals("change_contact"))
            {
                string ChangeType = xd.FirstChild["ChangeType"].InnerText;
                string SuiteId = xd.FirstChild["SuiteId"].InnerText;//第三方应用ID
                string corpID = xd.FirstChild["AuthCorpId"].InnerText;//授权企业的CorpID

                BP.Cloud.Org org = new BP.Cloud.Org();
                int i = org.Retrieve(BP.Cloud.OrgAttr.CorpID, corpID);
                if (i == 0)
                {
                    BP.DA.Log.DebugWriteError("err@不应该查询不到 AuthCorpId= " + corpID + "的数据.");
                    HttpContext.Current.Response.Write("err");
                    HttpContext.Current.Response.Write("success");
                    return "success";
                }

                //新建成员
                if (ChangeType.Equals("create_user"))
                {
                    BP.Cloud.WeXinAPI.Glo.changeConCreateUser(xd, org);
                    HttpContext.Current.Response.Write("success");
                    return "success";
                }

                //变更成员信息
                if (ChangeType.Equals("update_user"))
                {
                    BP.Cloud.WeXinAPI.Glo.changeConUpdateUser(xd, org);
                    HttpContext.Current.Response.Write("success");
                    return "success";
                }
                //删除成员
                if (ChangeType.Equals("delete_user"))
                {
                    BP.Cloud.WeXinAPI.Glo.changeConDeleteUser(xd, org);
                    HttpContext.Current.Response.Write("success");
                    return "success";
                }
                //新增部门
                if (ChangeType.Equals("create_party") == true)
                {
                    BP.Cloud.WeXinAPI.Glo.changeConCreateDept(xd, org);
                    HttpContext.Current.Response.Write("success");
                    return "success";
                }

                //更新部门
                if (ChangeType.Equals("update_party"))
                {
                    BP.Cloud.WeXinAPI.Glo.changeConUpdateDept(xd, org);
                    HttpContext.Current.Response.Write("success");
                    return "success";
                }
                //删除部门
                if (ChangeType.Equals("delete_party"))
                {
                    BP.Cloud.WeXinAPI.Glo.changeConDelDept(xd, org);
                    HttpContext.Current.Response.Write("success");
                    return "success";
                }
                HttpContext.Current.Response.Write("success");
            }
            //执行安装.
            // return BP.Cloud.WeXinAPI.Glo.MainBuess(xd);
            return "success";
        }
        /// <summary>
        /// 获取POST返回来的数据
        /// </summary>
        /// <returns></returns>
        private string PostInput()
        {
            try
            {
                System.IO.Stream s = Request.InputStream;
                int count = 0;
                byte[] buffer = new byte[1024];
                StringBuilder builder = new StringBuilder();
                while ((count = s.Read(buffer, 0, 1024)) > 0)
                {
                    builder.Append(Encoding.UTF8.GetString(buffer, 0, count));
                }
                s.Flush();
                s.Close();
                s.Dispose();
                return builder.ToString();
            }
            catch (Exception ex)
            { throw ex; }
        }
    }
}