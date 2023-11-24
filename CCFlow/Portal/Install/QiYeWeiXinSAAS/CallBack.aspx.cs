using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Concurrent;
using System.IO;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.WF;
using BP.Web;
using BP.Port;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Xml;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;
using BP.Cloud;
using System.Text.RegularExpressions;
using Dept = BP.Cloud.Dept;
using Glo = BP.Cloud.WeXinAPI.Glo;
namespace CCFlow
{
    public partial class CallBack : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string auth_code = BP.Cloud.WeXinAPI.Glo.getValue("auth_code");
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("auth_code", auth_code);//临时授权码.

            //使用应用提供商的provider_access_token
            String provider_access_token = BP.Cloud.WeXinAPI.Glo.getProviderAccessToken();
            string codeUrl = "https://qyapi.weixin.qq.com/cgi-bin/service/get_login_info?access_token=" + provider_access_token;

            //获得返回的数据.
            string res = Glo.HttpWebResponseUtility.HttpResponsePost_Json(codeUrl, JsonConvert.SerializeObject(parameters));

            //解析资源.
            JObject jobj = JObject.Parse(res);
            JToken resJtoken = jobj as JToken;

            BP.Cloud.Org org = new BP.Cloud.Org();

            //取得授权方企业信息
            string authCorpInfo = Glo.JSON_SeleteNode(resJtoken, "corp_info");
            JToken authCorpInfoJtoken = Glo.ReadJSON(authCorpInfo);
            string corpId = Glo.JSON_SeleteNode(authCorpInfoJtoken, "corpid"); //授权方企业微信id

            //取得用户信息
            string userInfo = Glo.JSON_SeleteNode(resJtoken, "user_info");
            JToken userInfoJtoken = Glo.ReadJSON(userInfo);
            string userID = Glo.JSON_SeleteNode(userInfoJtoken, "userid");

            int i = org.Retrieve(BP.Cloud.OrgAttr.CorpID, corpId, BP.Cloud.OrgAttr.WXUseSta, "1");

            //没有注册
            if (i == 0)
            {
                string regiurl = BP.Cloud.WeXinAPI.Glo.getInstallUr();
                this.Response.Redirect(regiurl, true);
                return;
            }

            BP.Web.WebUser.OrgNo = org.No;
            //执行登录.
            string userNo = org.No + "_" + userID;
            BP.WF.Dev2Interface.Port_Login(userID,null, org.No);

            //真实的ID.
            BP.Web.WebUser.No = userID;

            BP.Web.WebUser.OrgName = org.Name;
            string url = "/App/Portal/Home.htm?UserID=" + BP.Web.WebUser.No + "&OrgNo=" + org.No;
            this.Response.Redirect(url, true);
            return;
        }

    }
}