using System;
using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json.Linq;
using BP.Sys;
using BP.DA;
using Newtonsoft.Json;
using System.Linq;
using System.Xml;
using System.Net;
using BP.Port;
using BP.Difference;

namespace BP.Cloud.WeXinAPI
{
    public class Glo
    {
        /// <summary>
        /// 执行安装
        /// </summary>
        /// <param name="xd"></param>
        /// <returns></returns>
        public static string MainBuess(XmlDocument xd)
        {
            string infoType = xd.FirstChild["InfoType"].InnerText;

            //推送suite_ticket协议每十分钟微信推送一次，判断是推送ticket的情况，取得ticket保存到缓存中
            if (infoType.Equals("suite_ticket"))
            {
                string suitTicket = xd.FirstChild["SuiteTicket"].InnerText;
                Glo.Suite_ticket = suitTicket;
                BP.DA.Log.DebugWriteInfo("接收ticket信息,时间：" + DataType.CurrentDataTime + ";suitTicket:" + suitTicket);
                HttpContext.Current.Cache.Insert("suitTicket", suitTicket);
                HttpContext.Current.Response.Write("success");
                return "success";
            }

            //授权成功通知回调
            if (infoType.Equals("create_auth"))
            {
                //授权成功通知回调推送给服务商.
                string authCode = xd.FirstChild["AuthCode"].InnerText;
                HttpContext.Current.Cache.Insert("authcode", authCode);
                try
                {
                    //根据临时授权码，获得永久授权码并安装应用.
                    return BP.Cloud.WeXinAPI.Glo.InstallIt(authCode);
                }
                catch (Exception ex)
                {
                    return "err@安装失败,失败信息:" + ex.Message;
                }
            }
            //取消安装的，安装修改.
            if (infoType.Equals("cancel_auth"))
            {
                string authCorpId = xd.FirstChild["AuthCorpId"].InnerText;

                BP.Cloud.Org org = new BP.Cloud.Org();
                int i = org.Retrieve("CorpID", authCorpId);
                if (i == 1)
                    org.DoDelete();
                return "成功卸载，欢迎下次使用.";
            }

            //授权变更.
            if (infoType.Equals("change_auth"))
            {
                string authCode = xd.FirstChild["AuthCode"].InnerText;
                HttpContext.Current.Cache.Insert("authcode", authCode);
                return "change_auth执行成功.";
            }

            //通讯录变更
            if (infoType.Equals("change_contact"))
            {
                string changeType = xd.FirstChild["ChangeType"].InnerText;
                string SuiteId = xd.FirstChild["SuiteId"].InnerText;//第三方应用ID
                string corpID = xd.FirstChild["AuthCorpId"].InnerText;//授权企业的CorpID

                BP.Cloud.Org org = new BP.Cloud.Org();
                int i = org.Retrieve(BP.Cloud.OrgAttr.CorpID, corpID);
                if (i == 0)
                    return "err@不应该查询不到 AuthCorpId= " + corpID + "的数据.";

                switch (changeType)
                {
                    case "create_user": //新建成员
                        return BP.Cloud.WeXinAPI.Glo.changeConCreateUser(xd, org);
                    case "update_user": //变更成员信息
                        return BP.Cloud.WeXinAPI.Glo.changeConUpdateUser(xd, org);
                    case "delete_user": //删除成员
                        return BP.Cloud.WeXinAPI.Glo.changeConUpdateUser(xd, org);
                    case "create_party": //新增部门
                        return BP.Cloud.WeXinAPI.Glo.changeConCreateDept(xd, org);
                    case "update_party": //更新部门
                        return BP.Cloud.WeXinAPI.Glo.changeConUpdateDept(xd, org);
                    case "delete_party": //删除部门
                        return BP.Cloud.WeXinAPI.Glo.changeConDelDept(xd, org);
                    default:
                        return "err@没有判断的类型" + changeType;
                }
            }
            return "err@没有执行的判断." + infoType;
        }

        #region 通用的配置项 web.config 配置信息.
        private static BP.WF.HttpWebResponseUtility _httpWebResponseUtility = null;
        public static BP.WF.HttpWebResponseUtility HttpWebResponseUtility
        {
            get
            {
                if (_httpWebResponseUtility == null)
                    _httpWebResponseUtility = new BP.WF.HttpWebResponseUtility();
                return _httpWebResponseUtility;
            }
        }
        /// <summary>
        /// 企业ID
        /// </summary>
        public static string CorpID
        {
            get
            {
                return SystemConfig.AppSettings["CorpID"];
            }
        }
        /// <summary>
        /// 推广包ID
        /// </summary>
        public static string TemplateId
        {
            get
            {
                return SystemConfig.AppSettings["TemplateId"];
            }
        }
        /// <summary>
        /// 服务器域名
        /// </summary>
        public static string Domain
        {
            get
            {
                return SystemConfig.AppSettings["Domain"];
            }
        }
        /// <summary>
        /// 服务商ProviderSecret
        /// </summary>
        public static string ProviderSecret
        {
            get
            {
                return SystemConfig.AppSettings["ProviderSecret"];
            }
        }
        /// <summary>
        /// 服务商应用ID-SuiteID
        /// </summary>
        public static string SuiteID
        {
            get
            {
                return SystemConfig.AppSettings["SuiteID"];
            }
        }
        /// <summary>
        /// 服务商应用ID-密钥
        /// </summary>
        public static string SuiteID_Secret
        {
            get
            {
                return SystemConfig.AppSettings["SuiteID_Secret"];
            }
        }
        /// <summary>
        /// 我们应用的值
        /// </summary>
        public static string Token
        {
            get
            {
                return SystemConfig.AppSettings["Token"];
            }
        }
        /// <summary>
        /// 用于消息内容加密 EncodingAESKey
        /// </summary>
        public static string EncodingAESKey
        {
            get
            {
                return SystemConfig.AppSettings["EncodingAESKey"];
            }
        }
        #endregion web.config配置信息.

        /// <summary>
        /// 动态的凭证(经常用)(全局)
        /// </summary>
        private static string _Suite_ticket = null;
        public static string Suite_ticket
        {
            get
            {
                if (_Suite_ticket == null)
                    _Suite_ticket = HttpContext.Current.Cache["suitTicket"] as string;
                return _Suite_ticket;
            }
            set
            {
                _Suite_ticket = value;
                HttpContext.Current.Cache.Insert("suitTicket", value);
            }
        }
        /// <summary>
        /// 获取第三方应用凭证
        /// 检查表里是否有SuitAccessToken，如果存在并有效则返回
        /// </summary>
        /// <returns></returns>
        public static string getSuitAccessToken()
        {
            String accessToken = "";
            GloVar glovar = new GloVar();
            glovar.No = "suiteAccessToken";
            GloVar glovarEx = new GloVar();
            glovarEx.No = "suiteAccessTokenExpiresIn";
            if (glovar.RetrieveFromDBSources() > 0 && glovarEx.RetrieveFromDBSources() > 0)
            {
                //有效则返回
                if (DataType.IsNullOrEmpty(glovarEx.Val) == false
                        && DateTime.Compare(Convert.ToDateTime(DateTime.Now),
                        Convert.ToDateTime(glovarEx.Val)) < 0)
                {
                    //如果没有失效，就直接返回表里数据.
                    accessToken = glovar.Val;
                    BP.DA.Log.DebugWriteInfo("GloVar表中存在suiteAccessToken且有效，suiteAccessToken：" + accessToken);
                }
                else
                {
                    Dictionary<string, object> dd = getSuitAccessToken_S();
                    //失效,重新取，更新
                    string suitToken = (string)dd["suite_access_token"];
                    string expiresIn = (string)dd["expires_in"];
                    glovar.Val = suitToken;
                    glovar.GroupKey = "WeiXin";
                    glovar.Update();

                    DateTime ss = DateTime.Now.AddSeconds(double.Parse(expiresIn));
                    glovarEx.Val = ss.ToString("yyyy-MM-dd HH:mm:ss");
                    glovarEx.GroupKey = "WeiXin";
                    glovarEx.Update();

                    accessToken = suitToken;
                    BP.DA.Log.DebugWriteInfo("GloVar表中不存在suiteAccessToken，suiteAccessToken重新获取：" + accessToken);
                }
            }
            else
            {
                //不存在，获取插入
                Dictionary<string, object> dd = getSuitAccessToken_S();
                //失效,重新取，更新
                string suitToken = (string)dd["suite_access_token"];
                string expiresIn = (string)dd["expires_in"];
                glovar.Val = suitToken;
                glovar.GroupKey = "WeiXin";
                glovar.Insert();

                DateTime ss = DateTime.Now.AddSeconds(double.Parse(expiresIn));
                glovarEx.Val = ss.ToString("yyyy-MM-dd HH:mm:ss");
                glovarEx.GroupKey = "WeiXin";
                glovarEx.Insert();

                accessToken = suitToken;
                BP.DA.Log.DebugWriteInfo("GloVar表中不存在suiteAccessToken，suiteAccessToken重新获取：" + accessToken);
            }
            BP.DA.Log.DebugWriteInfo("Glo.cs中getSuitAccessToken()方法获取suiteAccessToken：" + accessToken);
            return accessToken;
        }
        /// <summary>
        /// 获取第三方应用凭证,服务器IP地址一定要先加入服务商的IP白明达中，否则验证失败。
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, object> getSuitAccessToken_S()
        {
            //获取第三方应用凭证，获得suite_access_token以获取预授权码
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("suite_id", HttpUtility.UrlEncode(BP.Cloud.WeXinAPI.Glo.SuiteID));//应用suiteId
            parameters.Add("suite_secret", HttpUtility.UrlEncode(BP.Cloud.WeXinAPI.Glo.SuiteID_Secret));//应用suiteSecret
            parameters.Add("suite_ticket", BP.Cloud.WeXinAPI.Glo.Suite_ticket);//suite_ticket
            BP.DA.Log.DebugWriteInfo("suite_ticket" + BP.Cloud.WeXinAPI.Glo.Suite_ticket);
            string suiteUrl = "https://qyapi.weixin.qq.com/cgi-bin/service/get_suite_token";
            try
            {


                //根据参数:获得资源.
                string res = Glo.HttpWebResponseUtility.HttpResponsePost_Json(suiteUrl,
                    JsonConvert.SerializeObject(parameters));
                BP.DA.Log.DebugWriteInfo("res" + res);
                Dictionary<string, object> dd = res.Trim(new char[] { '{', '}' }).Split(',').ToDictionary(s => s.Split(':')[0].Trim('"'), s => (object)s.Split(':')[1].Trim('"'));

                if (string.IsNullOrEmpty((string)dd["suite_access_token"]))
                {
                    getSuitAccessToken_S();
                    BP.DA.Log.DebugWriteError("err@获取SuitAccessToken错误，参数为：SuiteID:" + BP.Cloud.WeXinAPI.Glo.SuiteID + "SuiteID_Secret:" + BP.Cloud.WeXinAPI.Glo.SuiteID_Secret + "Suite_ticket:" + BP.Cloud.WeXinAPI.Glo.Suite_ticket);
                }


                return dd;
            }
            catch (Exception e)
            {
                BP.DA.Log.DebugWriteInfo(e.Message.ToString());
                throw e;
            }
        }
        /// <summary>
        /// 获取预授权码
        /// 该API用于获取预授权码。预授权码用于企业授权时的第三方服务商安全验证。
        /// </summary>
        /// <returns></returns>
        public static void getPreAuthCode()
        {
            //获取第三方应用凭证
            string suitAccessToken = getSuitAccessToken();
            if (string.IsNullOrEmpty(suitAccessToken))
                return;

            string yuUrl = "https://qyapi.weixin.qq.com/cgi-bin/service/get_pre_auth_code?suite_access_token=" + suitAccessToken;

            string res = Glo.HttpWebResponseUtility.HttpResponseGet(yuUrl);
            Dictionary<string, object> dd = res.Trim(new char[] { '{', '}' }).Split(',').ToDictionary(s => s.Split(':')[0].Trim('"'), s => (object)s.Split(':')[1].Trim('"'));
            string preAuthCode = (string)dd["pre_auth_code"];
            if (string.IsNullOrEmpty(preAuthCode))
            {
                return;
            }
            //设置授权配置,该接口可对某次授权进行配置。可支持测试模式（应用未发布时）。
            string resS = setSessionInfo(suitAccessToken, preAuthCode);
            Dictionary<string, object> ddresS = resS.Trim(new char[] { '{', '}' }).Split(',').ToDictionary(s => s.Split(':')[0].Trim('"'), s => (object)s.Split(':')[1].Trim('"'));
            string errcode = (string)ddresS["errcode"];
            if (!errcode.Equals("0"))
            {
                return;
            }
        }

        ///<summary>
        ///设置授权配置
        ///该接口可对某次授权进行配置。可支持测试模式（应用未发布时）。
        ///请求方式：POST（HTTPS）
        ///请求地址： https://qyapi.weixin.qq.com/cgi-bin/service/set_session_info?suite_access_token=SUITE_ACCESS_TOKEN
        /// </summary>
        public static string setSessionInfo(string suitAccessToken, string preAuthCode)
        {
            string url = "https://qyapi.weixin.qq.com/cgi-bin/service/set_session_info?suite_access_token=" + suitAccessToken;

            string parameters = "{\"pre_auth_code\":\"" + preAuthCode + "\",\"session_info\":{\"appid\":[],\"auth_type\":0}}";

            string res = Glo.HttpWebResponseUtility.HttpResponsePost_Json(url, parameters);
            return res;
        }

        /// <summary>
        /// 获取企业凭证 第三方服务商在取得企业的永久授权码后，通过此接口可以获取到企业的access_token。
        ///获取后可通过通讯录、应用、消息等企业接口来运营这些应用。
        /// </summary>
        /// <returns></returns>
        public static string getAccessToken(BP.Cloud.Org org, string permanentCode)
        {
            //获取第三方应用凭证
            string suitAccessToken = getSuitAccessToken();

            // string permanentCode = CreateOrg();//获取永久授权码
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("auth_corpid", org.CorpID);//授权方corpid
            parameters.Add("permanent_code", permanentCode);//永久授权码，通过get_permanent_code获取
            string accessTokenUrl = "https://qyapi.weixin.qq.com/cgi-bin/service/get_corp_token?suite_access_token=" + suitAccessToken;

            string res = Glo.HttpWebResponseUtility.HttpResponsePost_Json(accessTokenUrl, JsonConvert.SerializeObject(parameters));

            Dictionary<string, object> dd = res.Trim(new char[] { '{', '}' }).Split(',').ToDictionary(s => s.Split(':')[0].Trim('"'), s => (object)s.Split(':')[1].Trim('"'));
            string accessToken = (string)dd["access_token"];//授权方（企业）access_token,最长为512字节
            string expires_in = (string)dd["expires_in"];
            DateTime ss = DateTime.Now.AddSeconds(double.Parse(expires_in));
            //更新accessToken到org表中
            //BP.Cloud.Org org = new BP.Cloud.Org(corpid);

            org.AccessToken = accessToken;
            org.AccessTokenExpiresIn = ss.ToString("yyyy-MM-dd HH:mm:ss");

            //org.Update();
            return accessToken;
        }

        /// <summary>
        /// 组装安装授权页连接地址
        /// </summary>
        public static string getInstallUr()
        {
            //获取第三方应用凭证
            string suitAccessToken = getSuitAccessToken();
            if (string.IsNullOrEmpty(suitAccessToken))
            {
                return "";
            }
            string yuUrl = "https://qyapi.weixin.qq.com/cgi-bin/service/get_pre_auth_code?suite_access_token=" + suitAccessToken;

            string res = Glo.HttpWebResponseUtility.HttpResponseGet(yuUrl);
            Dictionary<string, object> dd = res.Trim(new char[] { '{', '}' }).Split(',').ToDictionary(s => s.Split(':')[0].Trim('"'), s => (object)s.Split(':')[1].Trim('"'));
            string preAuthCode = (string)dd["pre_auth_code"];
            if (string.IsNullOrEmpty(preAuthCode))
            {
                return "";
            }
            //设置授权配置,该接口可对某次授权进行配置。可支持测试模式（应用未发布时）。
            string resS = setSessionInfo(suitAccessToken, preAuthCode);
            Dictionary<string, object> ddresS = resS.Trim(new char[] { '{', '}' }).Split(',').ToDictionary(s => s.Split(':')[0].Trim('"'), s => (object)s.Split(':')[1].Trim('"'));
            string errcode = (string)ddresS["errcode"];
            if (!errcode.Equals("0"))
            {
                return "";
            }
            string redirect_uri = HttpUtility.UrlEncode(Glo.Domain + "Admin/WeChat/CallReg.aspx");

            //跳转链接中，第三方服务商需提供suite_id、预授权码、授权完成回调URI和state参数。其中redirect_uri是授权完成后的回调网址，redirect_uri需要经过一次urlencode作为参数；state可填a - zA - Z0 - 9的参数值（不超过128个字节），用于第三方自行校验session，防止跨域攻击。
            string urlInstall = "https://open.work.weixin.qq.com/3rdapp/install?suite_id=" + Glo.SuiteID +
                "&pre_auth_code=" + preAuthCode + "&redirect_uri=" + redirect_uri + "&state=1";
            return urlInstall;
        }
        /// <summary>
        /// 获取相应子节点的值
        /// </summary>
        /// <param name="childnodelist"></param>
        public static string JSON_SeleteNode(JToken json, string ReName)
        {
            try
            {
                string result = "";
                if (json == null)
                {
                    return result;
                }
                //这里6.0版块可以用正则匹配
                var node = json.SelectToken("$.." + ReName);
                if (node != null)
                {
                    //判断节点类型
                    if (node.Type == JTokenType.String || node.Type == JTokenType.Integer || node.Type == JTokenType.Float)
                    {
                        //返回string值
                        result = node.Value<object>().ToString();
                    }
                    if (node.Type == JTokenType.Object)
                    {
                        result = node.Value<object>().ToString();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static JToken ReadJSON(string jsonStr)
        {
            if (string.IsNullOrEmpty(jsonStr))
            {
                return null;
            }
            JObject jobj = JObject.Parse(jsonStr);
            JToken result = jobj as JToken;
            return result;
        }


        /// <summary>
        /// 应用提供商的provider_access_token
        /// </summary>
        /// <returns></returns>
        public static string getProviderAccessToken()
        {
            string provider_access_token = "";

            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("corpid", BP.Cloud.WeXinAPI.Glo.CorpID);
            parameters.Add("provider_secret", BP.Cloud.WeXinAPI.Glo.ProviderSecret);
            string url = "https://qyapi.weixin.qq.com/cgi-bin/service/get_provider_token";

            //获得返回的数据.
            string res = Glo.HttpWebResponseUtility.HttpResponsePost_Json(url, JsonConvert.SerializeObject(parameters));
            Dictionary<string, object> ddresS = res.Trim(new char[] { '{', '}' }).Split(',').ToDictionary(s => s.Split(':')[0].Trim('"'), s => (object)s.Split(':')[1].Trim('"'));
            /*string errcode = (string)ddresS["errcode"];
            if (!errcode.Equals("0"))
            {
                return "";
            }*/
            provider_access_token = (string)ddresS["provider_access_token"];//授权方（企业）access_token,最长为512字节
            string expires_in = (string)ddresS["expires_in"];
            return provider_access_token;
        }

        public static void testFetchDept()
        {
            //:https://qyapi.weixin.qq.com/cgi-bin/department/simplelist?access_token=NqJ1-81rVPhRQngMWsbnUTK_ACHtSRxrikjP2spVxRXlNNUg3b8tvRU1lOYX3IjOywyykTQvpR1FzLGyeyiZ6y7fegB5oSGLXIyx5-rqeKm88GIA0C-e8GysC7L82ZYxwPDUz-yXs2iaofjtG7Lssgb8nkwB_w1JlIHpqzu6odVWA24JeBYWyQHIeGYWs3VwjrOwLPZw2JplrMqFBU_y0Q
            string code = "g-0_iJsKzujQAaaMV3gu5D88sXZGebyNymb77LsiguM";
            BP.Cloud.Org org = new BP.Cloud.Org();
            org.No = "c206";
            org.RetrieveFromDBSources();
            createDept(org, code);
        }
        /// 
        /// <summary>
        /// 获取永久授权码
        /// 引导用户进入授权页
        ///用户确认授权后，会进入回调URI(即redirect_uri)，并在URI参数中带上临时授权码、过期时间以及state参数。第三方服务商据此获得临时授权码
        ///redirect_uri?auth_code=xxx&expires_in=600&state=xx
        /// </summary>
        /// <returns></returns>
        public static string InstallIt(string authCode)
        {
            //获取第三方应用凭证
            string suitAccessToken = getSuitAccessToken();

            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("auth_code", authCode);//临时授权码
            string codeUrl = "https://qyapi.weixin.qq.com/cgi-bin/service/get_permanent_code?suite_access_token=" + suitAccessToken;

            //获得返回的数据.
            string res = Glo.HttpWebResponseUtility.HttpResponsePost_Json(codeUrl, JsonConvert.SerializeObject(parameters));
            BP.DA.Log.DebugWriteInfo("install callback:" + res);
            //获取企业新信息，插入数据库
            //解析返回的json串
            JObject jobj = JObject.Parse(res);
            JToken resJtoken = jobj as JToken;

            //把数据写入log.
            string errcode = Glo.JSON_SeleteNode(resJtoken, "errcode");

            if (!string.IsNullOrEmpty(errcode) && !errcode.Equals("0"))
            {
                return "@err获取永久授权码错误，错误码为：" + errcode;
            }
            string permanentCode = Glo.JSON_SeleteNode(resJtoken, "permanent_code");

            //授权方（企业）access_token
            string accessToken = Glo.JSON_SeleteNode(resJtoken, "access_token");

            #region 企业信息
            string authCorpInfo = Glo.JSON_SeleteNode(resJtoken, "auth_corp_info");
            JToken authCorpInfoJtoken = Glo.ReadJSON(authCorpInfo);
            JToken authUserInfo = resJtoken["auth_user_info"];

            string corpId = Glo.JSON_SeleteNode(authCorpInfoJtoken, "corpid"); //授权方企业微信id

            string CorpSquareLogoUrl = Glo.JSON_SeleteNode(authCorpInfoJtoken, "corp_square_logo_url");//授权方企业方形头像
            SaveImgByUrl(CorpSquareLogoUrl, SystemConfig.PathOfDataUser + "\\ICONOrg\\", corpId + "_CorpSquareLogo.png");//上传头像

            string CorpRoundLogoUrl = Glo.JSON_SeleteNode(authCorpInfoJtoken, "corp_round_logo_url");//授权方企业圆形头像
            SaveImgByUrl(CorpRoundLogoUrl, SystemConfig.PathOfDataUser + "\\ICONOrg\\", corpId + "_CorpRoundLogo.png");//上传头像
            #endregion

            BP.Cloud.Org org = new BP.Cloud.Org();
            BP.Web.WebUser.OrgNo = "ccs";
            //执行登录.
            BP.WF.Dev2Interface.Port_Login("admin", "", "ccs");

            //写入到 Port_Org
            Insert_Port_Org(org, authCorpInfoJtoken, corpId, permanentCode, accessToken, authUserInfo);

            #region 授权信息。如果是通讯录应用，且没开启实体应用，是没有该项的。通讯录应用拥有企业通讯录的全部信息读写权限
            string authInfo = Glo.JSON_SeleteNode(resJtoken, "auth_info");
            JToken authInfoJtoken = Glo.ReadJSON(authInfo);
            //授权的应用信息，注意是一个数组，但仅旧的多应用套件授权时会返回多个agent，对新的单应用授权，永远只返回一个agent

            string agentid = Glo.JSON_SeleteNode(authInfoJtoken, "agentid");//授权方应用id
            org.AgentId = agentid;
            org.AgentName = Glo.JSON_SeleteNode(authInfoJtoken, "name");//授权方应用名字

            //保存img 的 log 数据.
            string SquareLogoUrl = Glo.JSON_SeleteNode(authInfoJtoken, "square_logo_url");//授权方应用方形头像
            SaveImgByUrl(SquareLogoUrl, SystemConfig.PathOfDataUser + "\\ICONOrg\\", corpId + "_SquareLogo.png");//上传头像
            string RoundLogoUrl = Glo.JSON_SeleteNode(authInfoJtoken, "round_logo_url");//授权方应用圆形头像
            SaveImgByUrl(RoundLogoUrl, SystemConfig.PathOfDataUser + "\\ICONOrg\\", corpId + "_RoundLogo.png");//上传头像
            #endregion

            //获取应用的管理员列表。
            InsertPort_OrgAdminer(corpId, agentid, suitAccessToken, org);

            //获取企业凭证 第三方服务商在取得企业的永久授权码后，通过此接口可以获取到企业的access_token。更新org表
            getAccessToken(org, permanentCode);

            //更新企业信息.
            org.Update();

            //获取部门列表，插入数据库.
            createDept(org, permanentCode);

            //检查是否有部门跟目录，没有就增加一个.
            Glo.Install_CheckHaveDeptRoot(org, authInfoJtoken);

            //执行登出..
            BP.WF.Dev2Interface.Port_SigOut();

            //获取人员信息，插入数据库.
            return permanentCode;
        }

        /// 获得部门数据.
        /// </summary>
        /// <param name="org">组织实体</param>
        /// <param name="permanentCode">永久授权码</param>
        public static void createDept(BP.Cloud.Org org, string permanentCode)
        {


            //如果AccessToken接近失效，要重新获取，更新.
            if (DataType.IsNullOrEmpty(org.AccessTokenExpiresIn) == false
                && DateTime.Compare(Convert.ToDateTime(DateTime.Now),
                Convert.ToDateTime(org.AccessTokenExpiresIn)) > 0)
            {
                //如果失效了，就直接更新一下.
                org.AccessToken = getAccessToken(org, permanentCode);//获取企业凭证,更新失效时间
            }

            //获得部门数据.
            string yuUrl = "https://qyapi.weixin.qq.com/cgi-bin/department/simplelist?access_token=" + org.AccessToken;
            BP.DA.Log.DebugWriteInfo("dept req url:" + yuUrl);
            string res = Glo.HttpWebResponseUtility.HttpResponseGet(yuUrl);


            BP.DA.Log.DebugWriteInfo("dept data list:" + res);

            try
            {

                //解析或得到的资源.
                Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
                Newtonsoft.Json.Linq.JArray jArray = Newtonsoft.Json.Linq.JArray.Parse(jObject.SelectToken("department_id").ToString());

                BP.DA.Log.DebugWriteInfo("jArray:" + jArray.ToString());

                //插入部门信息？
                foreach (Newtonsoft.Json.Linq.JToken item in jArray.Children())
                {
                    string id = item["id"].ToString();//部门ID
                    string name = id;//  item["name"].ToString();
                    string parentid = org.No + "_" + item["parentid"].ToString();

                    string order = item["order"].ToString();
                    BP.DA.Log.DebugWriteInfo("jToken:" + item.ToString());
                    //写入Port_Dept
                    #region 开始同步部门信息.
                    BP.Cloud.Dept deptRoot = new BP.Cloud.Dept();
                    deptRoot.No = org.No + "_" + id;
                    deptRoot.Name = name;
                    deptRoot.ParentNo = parentid;
                    deptRoot.OrgNo = org.No;

                    deptRoot.RefID = item["id"].ToString();
                    deptRoot.RefParentID = item["parentid"].ToString();

                    //如果是根目录.
                    if (deptRoot.RefParentID.Equals("0") == true)
                    {
                        deptRoot.ParentNo = "100";
                        deptRoot.Name = org.Name;
                        deptRoot.NameOfPath = org.Name;
                        deptRoot.No = org.No;
                    }

                    //如果是二级部门,其他级别的部门就不处理了.
                    if (deptRoot.RefParentID.Equals("1") == true)
                        deptRoot.ParentNo = org.No;

                    deptRoot.DirectInsert();

                    //获得部门下人员信息，写入人员表.
                    CreateEmp(org, id, deptRoot);
                    #endregion 开始同步部门信息.

                    //取得标签信息
                    createTag(org);
                    //递归数据.
                    // CreateDeptDiGui(item, org);
                }
            }
            catch (Exception ex)
            {
                BP.DA.Log.DebugWriteInfo("create dept error:" + ex.Message+" ee:"+ex.StackTrace+" cxx:"+ex.Source);
            }


        }

        /// <summary>
        /// 获得部门下人员信息，写入人员表
        /// </summary>
        /// <param name="res"></param>
        public static string CreateEmp(BP.Cloud.Org org, string id, BP.Cloud.Dept dept)
        {
            //构造url。
            string url = "https://qyapi.weixin.qq.com/cgi-bin/user/list?access_token=" + org.AccessToken + "&department_id=" + id + "&fetch_child=0";
            string res = DataType.ReadURLContext(url); //httpWebResponseUtility.HttpResponseGet(yuUrl);

            //解析资源.
            Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
            Newtonsoft.Json.Linq.JArray jArray = Newtonsoft.Json.Linq.JArray.Parse(jObject.SelectToken("userlist").ToString());

            foreach (Newtonsoft.Json.Linq.JToken item in jArray.Children())
            {
                string main_department = item["main_department"].ToString();//主部门

                /* string sql = "SELECT No FROM Port_Dept WHERE RefID='" + main_department + "' AND OrgNo='" + org.No + "'";
                 string mainDeptStr = DBAccess.RunSQLReturnStringIsNull(sql, null);
    */
                //插入Port_Emp中
                BP.Cloud.EmpWX emp = new BP.Cloud.EmpWX();
                emp.No = org.No + "_" + item["userid"].ToString(); //人员ID
                                                                   //open_userid全局唯一。对于同一个服务商，不同应用获取到企业内同一个成员的open_userid是相同的，最多64个字节。仅第三方应用可获取
                emp.OpenID = item["open_userid"].ToString();
                if (emp.RetrieveFromDBSources() == 0)
                {
                    emp.Name = item["name"].ToString();
                    emp.UserID = item["userid"].ToString();
                    emp.FK_Dept = org.No + "_" + main_department;
                    //如果主部门是部门的根目录.
                    if (main_department.Equals("1") == true)
                    {
                        emp.FK_Dept = org.No;
                    }
                    emp.OrgNo = org.No;
                    emp.Insert();

                    //处理多个部门.
                    string deptStrs = item["department"].ToString();
                    deptStrs = deptStrs.Replace("[", "");
                    deptStrs = deptStrs.Replace("]", "");
                    string[] strs = deptStrs.Split(','); //处理一下分割.
                    foreach (string str in strs)
                    {
                        string deptNo = str.Replace("\r\n  ", "");
                        if (DataType.IsNullOrEmpty(deptNo) == true)
                            continue;

                        //找到真实的部门编号.
                        /*sql = "SELECT No FROM Port_Dept WHERE RefID='" + str + "' AND OrgNo='" + org.No + "'";
                        string deptNo = DBAccess.RunSQLReturnStringIsNull(sql, null);
                        if (deptNo == null)
                            continue; //说明部门不存在，不应该出现的问题.
    */
                        BP.Cloud.DeptEmp de = new BP.Cloud.DeptEmp();
                        //如果是根目录.
                        if (deptNo.Trim().Equals("1") == true)
                        {
                            de.FK_Dept = org.No;
                        }
                        else
                        {
                            de.FK_Dept = org.No + "_" + deptNo.Trim();
                        }

                        de.FK_Emp = emp.UserID; //使用userID.
                        de.MyPK = org.No + "_" + deptNo.Trim() + "_" + de.FK_Emp;
                        de.OrgNo = org.No; //组织信息.
                        de.EmpNo = emp.No;

                        de.DirectInsert();
                    }

                }
            }


            return "";
        }

        /// <summary>
        /// 取得标签列表
        /// </summary>
        public static void createTag(BP.Cloud.Org org)
        {
            //构造url。
            string url = "https://qyapi.weixin.qq.com/cgi-bin/tag/list?access_token=" + org.AccessToken;
            string res = DataType.ReadURLContext(url); //httpWebResponseUtility.HttpResponseGet(yuUrl);

            //解析资源.
            Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
            //标签列表
            Newtonsoft.Json.Linq.JArray jArray = Newtonsoft.Json.Linq.JArray.Parse(jObject.SelectToken("taglist").ToString());

            foreach (Newtonsoft.Json.Linq.JToken item in jArray.Children())
            {
                string tagid = item["tagid"].ToString();//标签id
                string tagname = item["tagname"].ToString();//标签名
                                                            //根据标签id取得下属人员
                getTagEmp(org.AccessToken, tagid);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void getTagEmp(string accessToken, string tagid)
        {
            //构造url。
            string url = "https://qyapi.weixin.qq.com/cgi-bin/tag/get?access_token=" + accessToken + "&tagid=" + tagid;
            string res = DataType.ReadURLContext(url); //httpWebResponseUtility.HttpResponseGet(yuUrl);

            //解析资源.
            Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
            //标签下人员列表
            Newtonsoft.Json.Linq.JArray jArray = Newtonsoft.Json.Linq.JArray.Parse(jObject.SelectToken("userlist").ToString());

            foreach (Newtonsoft.Json.Linq.JToken item in jArray.Children())
            {
                string userid = item["userid"].ToString();//成员帐号
                string name = item["name"].ToString();//成员名称，此字段从2019年12月30日起，对新创建第三方应用不再返回，2020年6月30日起，对所有历史第三方应用不再返回，后续第三方仅通讯录应用可获取，第三方页面需要通过通讯录展示组件来展示名字
                string partylist = item["partylist"].ToString();//标签中包含的部门id列表
                TeamEmp teamEmp = new TeamEmp();
                teamEmp.FK_Emp = userid;
                teamEmp.FK_Team = tagid;
                teamEmp.Insert();
            }
        }
        /// <summary>
        /// 判断是否选择了部门
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        public static string Install_CheckHaveDeptRoot(Org org, JToken authInfoJtoken)
        {
            BP.Cloud.Depts depts = new BP.Cloud.Depts();
            depts.Retrieve("OrgNo", org.No);
            if (depts.Count == 0)
            {
                //插入部门做root.
                BP.Cloud.Dept dept = new BP.Cloud.Dept();
                dept.No = org.No;
                dept.Name = org.Name;
                dept.ParentNo = "100";
                dept.OrgNo = org.No;
                dept.Insert();
            }

            //d定义变量..
            BP.Cloud.Emp emp = new BP.Cloud.Emp();

            //增加管理员.
            BP.Cloud.OrgAdminers admins = new BP.Cloud.OrgAdminers();
            admins.Retrieve("OrgNo", org.No);
            foreach (BP.Cloud.OrgAdminer admin in admins)
            {
                if (emp.IsExit("OrgNo", org.No, "UserID", admin.FK_Emp) == true)
                    continue;

                emp.FK_Dept = org.No;
                emp.Name = admin.FK_Emp;
                emp.No = admin.FK_Emp;

                /* if (DataType.IsMobile(emp.No) == true)
                     emp.No = emp.No;
                 else*/
                emp.No = org.No + "_" + emp.No;

                emp.UserID = admin.FK_Emp;
                emp.OrgNo = org.No;

                //获得OpenID.
                string openID = GetOpenIDByUserID(org.AccessToken, emp.UserID);
                if (openID != null)
                    emp.OpenID = openID;

                emp.Insert();

                //给他分配部门.
                DeptEmp de = new DeptEmp();
                de.OrgNo = emp.OrgNo;
                de.FK_Dept = emp.FK_Dept;
                de.FK_Emp = emp.UserID;
                de.MyPK = emp.FK_Dept + "_" + de.FK_Emp;
                de.EmpNo = emp.No;

                de.Save();

            }

            //把管理员,获得范围内人员.
            string privilege = Glo.JSON_SeleteNode(authInfoJtoken, "privilege");//应用对应的权限
                                                                                //应用可见范围（成员）
            Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(privilege);
            Newtonsoft.Json.Linq.JArray jArray = Newtonsoft.Json.Linq.JArray.Parse(jObject.SelectToken("allow_user").ToString());

            foreach (Newtonsoft.Json.Linq.JToken token in jArray.Children())
            {
                string empNo = token.ToString();
                if (emp.IsExit("OrgNo", org.No, "UserID", empNo) == true)
                    continue;

                emp.No = empNo;
                emp.UserID = empNo;

                emp.FK_Dept = org.No;
                emp.Name = empNo;

                if (DataType.IsMobile(emp.No) == true)
                {
                    emp.No = empNo;
                    emp.Tel = empNo;
                }
                else
                {
                    emp.No = org.No + "_" + empNo;
                }

                emp.OrgNo = org.No;

                //获得OpenID.
                string openID = GetOpenIDByUserID(org.AccessToken, emp.UserID);
                if (openID != null)
                    emp.OpenID = openID;
                emp.Insert();


                //给他分配部门.
                DeptEmp de = new DeptEmp();
                de.OrgNo = emp.OrgNo;
                de.FK_Dept = emp.FK_Dept;
                de.FK_Emp = emp.UserID;
                de.MyPK = emp.FK_Dept + "_" + de.FK_Emp;
                de.EmpNo = emp.No;
                de.Save();

            }
            return "检查成功.";
        }
        /// <summary>
        /// 根据userid获取企业微信openid
        /// 如果有权限问题，就获取不到。
        /// </summary>
        /// <param name="tooken"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static string GetOpenIDByUserID(string tooken, string userID)
        {
            //根据uiserID,获得openID.
            IDictionary<string, string> parametersAdm = new Dictionary<string, string>();
            parametersAdm.Add("userid", HttpUtility.UrlEncode(userID));

            string url = "https://qyapi.weixin.qq.com/cgi-bin/user/convert_to_openid?access_token=" + tooken;

            string docs = HttpWebResponseUtility.HttpResponsePost_Json(url, JsonConvert.SerializeObject(parametersAdm));

            Dictionary<string, object> dd = docs.Trim(new char[] { '{', '}' }).Split(',').ToDictionary(s => s.Split(':')[0].Trim('"'), s => (object)s.Split(':')[1].Trim('"'));
            string code = (string)dd["errcode"];
            if (code.Equals("0") == true)
            {
                string str = (string)dd["openid"];
                return str;
            }

            return null;
        }
        /// <summary>
        /// 通讯录变更中用户新增事件
        /// </summary>
        /// <param name="xd"></param>
        /// <param name="org"></param>
        /// <returns></returns>
        public static string changeConCreateUser(XmlDocument xd, Org org)
        {
            string userID = xd.FirstChild["UserID"].InnerText;//成员UserID、变更信息的成员UserID
            string Name = xd.FirstChild["Name"].InnerText;//成员名称。2020年6月30日起，对所有历史第三方应用不再返回真实name，使用userid代替name，后续第三方仅通讯录应用可获取，第三方页面需要通过通讯录展示组件来展示名字
            string Department = xd.FirstChild["Department"].InnerText;//更新后成员所在部门列表，仅返回该应用有查看权限的部门id
            string MainDepartment = xd.FirstChild["MainDepartment"].InnerText;//主部门

            string strUserID = userID;
            if (BP.DA.DataType.IsMobile(userID) == false)
                strUserID = org.No + "_" + userID;

            //创建用户.
            BP.Cloud.Emp emp = new BP.Cloud.Emp();
            emp.No = strUserID;
            if (emp.RetrieveFromDBSources() == 0)
            {
                emp.Name = userID;
                emp.UserID = userID;

                //处理多个部门.
                string deptStrs = Department;
                deptStrs = deptStrs.Replace("[", "");
                deptStrs = deptStrs.Replace("]", "");
                string[] strs = deptStrs.Split(','); //处理一下分割.
                foreach (string str in strs)
                {
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;

                    BP.Cloud.DeptEmp de = new BP.Cloud.DeptEmp();
                    de.FK_Dept = org.No + "_" + str;
                    de.FK_Emp = emp.No;
                    de.MyPK = de.FK_Dept + "_" + de.FK_Emp;
                    de.OrgNo = org.No; //组织信息.
                    de.Insert();
                }
                emp.FK_Dept = org.No + "_" + MainDepartment;
                emp.OrgNo = org.No;
                emp.Insert();
            }
            else
            {
                //处理多个部门.
                string deptStrs = Department;
                deptStrs = deptStrs.Replace("[", "");
                deptStrs = deptStrs.Replace("]", "");
                string[] strs = deptStrs.Split(','); //处理一下分割.
                foreach (string str in strs)
                {
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;

                    BP.Cloud.DeptEmp de = new BP.Cloud.DeptEmp();
                    de.FK_Dept = org.No + "_" + str;
                    de.FK_Emp = emp.No;
                    de.MyPK = de.FK_Dept + "_" + de.FK_Emp;
                    de.OrgNo = org.No; //组织信息.
                    de.Insert();
                }
                emp.FK_Dept = org.No + "_" + MainDepartment;
                emp.OrgNo = org.No;
                emp.Update();
            }

            return "用户[" + emp.Name + "]新增成功.";
        }
        /// <summary>
        /// 通讯录变更中用户变更事件
        /// </summary>
        /// <param name="xd"></param>
        /// <param name="org"></param>
        public static string changeConUpdateUser(XmlDocument xd, Org org)
        {
            string userID = xd.FirstChild["UserID"].InnerText;//成员UserID、变更信息的成员UserID
            string newUserID = "";//新的UserID，变更时推送（userid由系统生成时可更改一次）
            if (xd.FirstChild["NewUserID"] == null)
                newUserID = userID;
            else
                newUserID = xd.FirstChild["NewUserID"].InnerText;
            //创建en.
            BP.Cloud.Emp emp = new BP.Cloud.Emp();

            //说明：主键ID, 没有变化.
            if (userID.Equals(newUserID) == true)
            {
                //是一个手机号.
                if (BP.DA.DataType.IsMobile(userID) == true)
                {
                    emp.No = userID;
                }
                else
                    emp.No = org.No + "_" + userID; //组合主键.

                int num = emp.RetrieveFromDBSources(); //从数据库把他查询出来.
                if (num == 0)
                {
                    emp.Name = userID;
                    emp.Insert(); //不应该出现的情况.
                }
            }

            //主键ID，变化了.
            if (userID.Equals(newUserID) == false)
            {
                string strNewUserID = newUserID;
                if (BP.DA.DataType.IsMobile(newUserID) == false)
                    strNewUserID = org.No + "_" + newUserID;

                string strUserID = userID;
                if (BP.DA.DataType.IsMobile(userID) == false)
                    strUserID = org.No + "_" + userID;

                //修改主键.
                string sql = "UPDATE Port_Emp SET No='" + strNewUserID + "' WHERE No='" + strUserID + "' AND OrgNo='" + org.No + "' ";
                DBAccess.RunSQL(sql);

                emp.No = strNewUserID;
                emp.RetrieveFromDBSources();
            }

            //string Name = xd.FirstChild["Name"].InnerText;//成员名称。2020年6月30日起，对所有历史第三方应用不再返回真实name，使用userid代替name，后续第三方仅通讯录应用可获取，第三方页面需要通过通讯录展示组件来展示名字
            string Department = xd.FirstChild["Department"].InnerText;//更新后成员所在部门列表，仅返回该应用有查看权限的部门id
            string MainDepartment = xd.FirstChild["MainDepartment"].InnerText;//主部门

            //处理多个部门.
            string deptStrs = Department;
            deptStrs = deptStrs.Replace("[", "");
            deptStrs = deptStrs.Replace("]", "");
            string[] strs = deptStrs.Split(','); //处理一下分割.
            foreach (string str in strs)
            {
                if (DataType.IsNullOrEmpty(str) == true)
                    continue;

                BP.Cloud.DeptEmp de = new BP.Cloud.DeptEmp();
                de.FK_Dept = org.No + "_" + str;
                de.FK_Emp = emp.No;
                de.MyPK = de.FK_Dept + "_" + de.FK_Emp;
                de.OrgNo = org.No; //组织信息.
                de.Update();
            }
            emp.FK_Dept = org.No + "_" + MainDepartment;
            emp.OrgNo = org.No;
            emp.Update();

            return "更新[" + emp.No + "]成功.";
        }

        /// <summary>
        /// 通讯录变更中用户删除事件
        /// </summary>
        /// <param name="xd"></param>
        /// <param name="org"></param>
        public static string changeConDeleteUser(XmlDocument xd, Org org)
        {
            string userID = xd.FirstChild["UserID"].InnerText;//成员UserID、变更信息的成员UserID
            if (BP.DA.DataType.IsMobile(userID) == false)
                userID = org.No + "_" + userID;

            BP.Cloud.Emp emp = new BP.Cloud.Emp();
            emp.No = userID; //人员ID
            if (emp.RetrieveFromDBSources() != 0)
                emp.Delete();

            //删除部门对应关系.
            BP.Cloud.DeptEmps des = new BP.Cloud.DeptEmps();
            des.Delete("FK_Emp", emp.UserID, "OrgNo", org.No);

            //删除部门岗位对应关系.
            BP.Cloud.DeptEmpStations dess = new BP.Cloud.DeptEmpStations();
            dess.Delete("FK_Emp", emp.UserID, "OrgNo", org.No);

            return "删除[" + emp.No + "]成功.";

        }

        /// <summary>
        /// 通讯录变更中部门新增事件
        /// </summary>
        /// <param name="xd"></param>
        /// <param name="org"></param>
        public static string changeConCreateDept(XmlDocument xd, Org org)
        {
            string id = xd.FirstChild["Id"].InnerText;//部门Id
            string Name = xd.FirstChild["Name"].InnerText;//部门名称，此字段从2019年12月30日起，对新创建第三方应用不再返回，2020年6月30日起，对所有历史第三方应用不再返回真实Name字段，使用Id字段代替Name字段，后续第三方仅通讯录应用可获取，第三方页面需要通过通讯录展示组件来展示名字
            string parentId = xd.FirstChild["ParentId"].InnerText;//父部门id
            if (parentId.Equals("0") == true)
            {
                parentId = "100";
                id = org.No;
            }
            else
            {
                parentId = org.No + "_" + parentId;
                id = org.No + "_" + id;
            }

            string idx = xd.FirstChild["Order"].InnerText;//部门排序
            BP.Cloud.Dept dept = new BP.Cloud.Dept();
            dept.No = id;
            if (dept.RetrieveFromDBSources() == 0)
            {
                dept.Name = xd.FirstChild["Id"].InnerText;  //1,2,3的ID. 
                dept.ParentNo = parentId;
                dept.OrgNo = org.No;
                dept.Idx = int.Parse(idx);

                //关联的父部门.
                dept.RefParentID = xd.FirstChild["ParentId"].InnerText;//父部门id
                dept.Insert();
            }
            else
            {
                dept.ParentNo = parentId;
                dept.OrgNo = org.No;
                dept.Idx = int.Parse(idx);

                //关联的父部门.
                dept.RefParentID = xd.FirstChild["ParentId"].InnerText;//父部门id
                dept.Update();
            }

            return "创建部门[" + dept.No + "]成功.";

        }
        /// <summary>
        /// 通讯录变更中部门修改事件
        /// </summary>
        /// <param name="xd"></param>
        /// <param name="org"></param>
        public static string changeConUpdateDept(XmlDocument xd, Org org)
        {
            string id = xd.FirstChild["Id"].InnerText;//部门Id
            string parentId = xd.FirstChild["ParentId"].InnerText;//父部门id

            if (parentId.Equals("0") == true)
            {
                parentId = "100";
                id = org.No;
            }
            else
            {
                parentId = org.No + "_" + parentId;
                id = org.No + "_" + id;
            }

            string idx = xd.FirstChild["Order"].InnerText;//部门排序
            BP.Cloud.Dept dept = new BP.Cloud.Dept();
            dept.No = id;
            if (dept.RetrieveFromDBSources() != 0)
            {
                dept.ParentNo = parentId;
                dept.OrgNo = org.No;
                dept.Idx = int.Parse(idx);
                //关联的父部门.
                dept.RefParentID = xd.FirstChild["ParentId"].InnerText;//父部门id
                dept.Update();
            }

            return "修改部门[" + dept.No + "]成功.";

        }

        /// <summary>
        /// 通讯录变更中部门删除事件
        /// </summary>
        /// <param name="xd"></param>
        /// <param name="org"></param>
        public static string changeConDelDept(XmlDocument xd, Org org)
        {
            string id = xd.FirstChild["Id"].InnerText;//部门Id
            string parentId = xd.FirstChild["ParentId"].InnerText;//父部门id
            if (parentId.Equals("0") == true)
            {
                parentId = "100";
                id = org.No;
            }
            else
            {
                parentId = org.No + "_" + parentId;
                id = org.No + "_" + id;
            }

            BP.Cloud.Dept dept = new BP.Cloud.Dept();
            dept.No = id;
            dept.Delete();

            return "删除部门[" + dept.No + "]成功.";

        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="url">url路径</param>
        /// <param name="saveToDir">保存到目录</param>
        /// <param name="fileName">保存的文件名</param>
        public static void SaveImgByUrl(string url, string saveToDir, string fileName)
        {
            if (string.IsNullOrEmpty(url))
                return;

            WebRequest imgRequest = WebRequest.Create(url);
            HttpWebResponse res;
            try
            {
                res = (HttpWebResponse)imgRequest.GetResponse();
            }
            catch (WebException ex)
            {
                res = (HttpWebResponse)ex.Response;
            }

            if (res.StatusCode.ToString() == "OK")
            {
                System.Drawing.Image downImage = System.Drawing.Image.FromStream(imgRequest.GetResponse().GetResponseStream());
                if (!System.IO.Directory.Exists(saveToDir))
                {
                    System.IO.Directory.CreateDirectory(saveToDir);
                }
                downImage.Save(saveToDir + fileName);
                downImage.Dispose();
            }
        }

        /// <summary>
        /// 编辑org信息写入
        /// </summary>
        /// <param name="org"></param>
        /// <param name="authCorpInfoJtoken"></param>
        /// <param name="corpId"></param>
        /// <param name="permanentCode"></param>
        /// <param name="accessToken"></param>
        public static void Insert_Port_Org(Org org, JToken authCorpInfoJtoken, string corpId, string permanentCode, string accessToken, JToken authUserInfo)
        {
            //判断是否注册过，如果注册过，就把它删除掉.
            int i = org.Retrieve(BP.Cloud.OrgAttr.CorpID, corpId);
            if (i == 1)
                org.DoDelete();

            //微信状态是使用中.
            org.WXUseSta = 1;

            //来源：微信
            org.RegFrom = 1;
            //赋值名称.
            org.Name = Glo.JSON_SeleteNode(authCorpInfoJtoken, "corp_name"); //授权方企业名称，即企业简称
            org.NameFull = Glo.JSON_SeleteNode(authCorpInfoJtoken, "corp_full_name"); //授权方企业的主体名称(仅认证或验证过的企业有)，即企业全称。
            if (DataType.IsNullOrEmpty(org.NameFull) == true)
                org.NameFull = org.Name;


            //首先按照组织的简称计算.
            org.CorpID = corpId;//授权方企业微信id
                                //   org.Name = JSON_SeleteNode(authCorpInfoJtoken, "corp_name"); //授权方企业名称，即企业简称
                                // org.NameFull = JSON_SeleteNode(authCorpInfoJtoken, "corp_full_name"); //授权方企业的主体名称(仅认证或验证过的企业有)，即企业全称。
            org.Addr = Glo.JSON_SeleteNode(authCorpInfoJtoken, "location");//企业所在地信息, 为空时表示未知
            org.PermanentCode = permanentCode; //永久授权码.
            org.AccessToken = accessToken;
            org.CorpUserMax = Glo.JSON_SeleteNode(authCorpInfoJtoken, "corp_user_max");//授权方企业用户规模
            org.CorpAgentMax = Glo.JSON_SeleteNode(authCorpInfoJtoken, "corp_agent_max");//授权方企业应用数上限
            org.CorpFullName = Glo.JSON_SeleteNode(authCorpInfoJtoken, "corp_full_name");//授权方企业的主体名称
            org.SubjectType = Glo.JSON_SeleteNode(authCorpInfoJtoken, "subject_type");//企业类型，1. 企业; 2. 政府以及事业单位; 3. 其他组织, 4.团队号
            org.VerifiedEndTime = Glo.JSON_SeleteNode(authCorpInfoJtoken, "verified_end_time");//认证到期时间
            org.CorpScale = Glo.JSON_SeleteNode(authCorpInfoJtoken, "corp_scale");//企业规模。当企业未设置该属性时，值为空
            org.CorpIndustry = Glo.JSON_SeleteNode(authCorpInfoJtoken, "corp_industry");//企业所属行业。当企业未设置该属性时，值为空
            org.CorpSubIndustry = Glo.JSON_SeleteNode(authCorpInfoJtoken, "corp_sub_industry");//企业所属子行业。当企业未设置该属性时，值为空
            org.Location = Glo.JSON_SeleteNode(authCorpInfoJtoken, "location"); //企业所在地信息, 为空时表示未知
            org.WXUseSta = 1; //使用状态.
            org.DTReg = DataType.CurrentDataTime;
            org.RegFrom = 1; //注册来源微信.

            BP.DA.Log.DebugWriteInfo("安装应用时间：" + DataType.CurrentDataTime + ";企业简称:" + org.Name + ";企业全称:" + org.NameFull + ";企业所在地信息:"
                + org.Location + ";企业类型:" + org.SubjectType);

            org.No = BP.Cloud.Org.GenerNewOrgNo();
            org.Adminer = (string)authUserInfo["userid"];
            BP.DA.Log.DebugWriteInfo("========" + org.No);
            org.Insert();

            //初始化数据.
            org.Init_OrgDatas();
        }

        /// <summary>
        /// 获得应用的管理员并插入Port_OrgAdminer中
        /// </summary>
        /// <param name="corpId"></param>
        /// <param name="agentid"></param>
        /// <param name="suitAccessToken"></param>
        /// <param name="org"></param>
        public static void InsertPort_OrgAdminer(string corpId, string agentid, string suitAccessToken, Org org)
        {
            IDictionary<string, string> parametersAdm = new Dictionary<string, string>();
            parametersAdm.Add("auth_corpid", HttpUtility.UrlEncode(corpId));
            parametersAdm.Add("agentid", HttpUtility.UrlEncode(agentid));//授权方安装的应用agentid
            string codeUrlAdm = "https://qyapi.weixin.qq.com/cgi-bin/service/get_admin_list?suite_access_token=" + suitAccessToken;

            string resAdm = Glo.HttpWebResponseUtility.HttpResponsePost_Json(codeUrlAdm, JsonConvert.SerializeObject(parametersAdm));
            //解析获取管理员
            JToken resAdmJtoken = Glo.ReadJSON(resAdm);

            if (Glo.JSON_SeleteNode(resAdmJtoken, "errcode").Equals("0"))
            {
                //插入Port_OrgAdminer中
                BP.Cloud.OrgAdminer orgAdminer = new BP.Cloud.OrgAdminer();

                Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(resAdm);
                Newtonsoft.Json.Linq.JArray jArray = Newtonsoft.Json.Linq.JArray.Parse(jObject.SelectToken("admin").ToString());

                foreach (Newtonsoft.Json.Linq.JToken item in jArray.Children())
                {
                    orgAdminer = new BP.Cloud.OrgAdminer();
                    orgAdminer.FK_Emp = item["userid"].ToString();//管理员
                    orgAdminer.OrgNo = org.No;
                    orgAdminer.Insert();
                }
                // org.Adminer = adminer;//获得管理员.
            }
        }

        public static void SendMsg(string sender, string toUserIDs, string title, string docs, string url, string orgNo)
        {
            BP.DA.Log.DefaultLogWriteLineInfo("+++++++++++进入SendMsg方法++++++++++++++++");
            //httppost请求
            BP.WF.HttpWebResponseUtility httpWebResponseUtility = new BP.WF.HttpWebResponseUtility();

            //根据orgNo取得AccessToken
            BP.Cloud.Org org = new BP.Cloud.Org();
            org.No = orgNo;
            if (org.RetrieveFromDBSources() == 0)
                return;

            //如果AccessToken接近失效，要重新获取，更新
            string accessToken = "";
            if (DataType.IsNullOrEmpty(org.AccessTokenExpiresIn) == false
                && DateTime.Compare(Convert.ToDateTime(DateTime.Now),
                Convert.ToDateTime(org.AccessTokenExpiresIn)) > 0)
            {
                // BP.DA.Log.DefaultLogWriteLineInfo("++++++++++进入获取getAccessToken++++++++++++++++");
                //如果失效了，就直接更新一下.
                BP.Cloud.HttpHandler.App_Org handler = new HttpHandler.App_Org();
                accessToken = handler.getAccessToken(org);//获取企业凭证,更新失效时间
                //BP.DA.Log.DefaultLogWriteLineInfo("++++++++++获取getAccessToken："+ accessToken + "++++++++++++++++");
            }
            else
            {
                accessToken = org.AccessToken;
            }

            //组织发送信息的参数
            if (DataType.IsNullOrEmpty(toUserIDs) == true)
                return;

            toUserIDs = toUserIDs.Replace("，", "|");
            toUserIDs = toUserIDs.Replace(",", "|");

            string[] strs = toUserIDs.Split('|');

            string mystr = "";
            foreach (string item in strs)
            {
                if (DataType.IsNullOrEmpty(item) == true)
                    continue;
                /*
                                if (BP.DA.DataType.IsMobile(item) == false)
                                    mystr += "" + org.No + "_" + item + "|";
                                else*/
                mystr += item + "|";
            }
            mystr = mystr.Substring(0, mystr.Length - 1);

            /*touser:指定接收消息的成员，成员ID列表（多个接收者用‘|’分隔，最多支持1000个）。特殊情况：指定为”@all”，则向该企业应用的全部成员发送
            *toparty:指定接收消息的部门，部门ID列表，多个接收者用‘|’分隔，最多支持100个。 当touser为”@all”时忽略本参数
            *totag:指定接收消息的标签，标签ID列表，多个接收者用‘|’分隔，最多支持100个。 当touser为”@all”时忽略本参数
            *msgtype:消息类型，此时固定为：text
            *agentid:企业应用的id，整型。企业内部开发，可在应用的设置页面查看；第三方服务商，可通过接口 获取企业授权信息 获取该参数值
            *text:消息内容，最长不超过2048个字节，超过将截断（支持id转译）
            *safe:表示是否是保密消息，0表示否，1表示是，默认0
            *enable_id_trans:表示是否开启id转译，0表示否，1表示是，默认0
            *enable_duplicate_check:表示是否开启重复消息检查，0表示否，1表示是，默认0
            *duplicate_check_interval:表示是否重复消息检查的时间间隔，默认1800s，最大不超过4小时*/
            string sendUrl = "https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token=" + accessToken;
            //文本模式
            /*string msgJson = "{\"touser\":\"" + mystr + "\",\"toparty\":\"\",\"totag\":\"\",\"msgtype\":\"text\",";
            msgJson += "\"agentid\": " + org.AgentId + ",";
            msgJson += "\"text\": { \"content\": \"" + docs + "\" },";
            msgJson += " \"safe\":0, \"enable_id_trans\":0, \"enable_duplicate_check\":0,";
            msgJson += " \"duplicate_check_interval\":1800";
            msgJson += "}";*/
            //文本卡片模式
            string msgJson = "{\"touser\":\"" + mystr + "\",\"toparty\":\"\",\"totag\":\"\",\"msgtype\":\"textcard\",";
            msgJson += "\"agentid\": " + org.AgentId + ",";
            msgJson += "\"textcard\": { \"title\":\"待办消息\",\"description\": \"" + title + "\",\"url\" : \"" + url + "\" },";
            msgJson += " \"enable_id_trans\":0, \"enable_duplicate_check\":0,";
            msgJson += " \"duplicate_check_interval\":1800";
            msgJson += "}";

            //BP.DA.Log.DefaultLogWriteLineInfo("++++++++++msgJson：" + msgJson + "++++++++++++++++");
            //获得返回的数据.
            string res = httpWebResponseUtility.HttpResponsePost_Json(sendUrl, msgJson);

            BP.DA.Log.DefaultLogWriteLineInfo("++++++++++res：" + res + "++++++++++++++++");
            //获取企业新信息，插入数据库
            //解析返回的json串
            Dictionary<string, object> dd = res.Trim(new char[] { '{', '}' }).Split(',').ToDictionary(s => s.Split(':')[0].Trim('"'), s => (object)s.Split(':')[1].Trim('"'));
            string errcode = (string)dd["errcode"];

            if (errcode.Equals("0"))
            {
                /*
                 * 如果部分接收人无权限或不存在，发送仍然执行，但会返回无效的部分（即invaliduser或invalidparty或invalidtag），
                 * 常见的原因是接收人不在应用的可见范围内。
                 * 如果全部接收人无权限或不存在，则本次调用返回失败，errcode为81013。
                 * 返回包中的userid，不区分大小写，统一转为小写
                 */
                string invaliduser = (string)dd["invaliduser"];//发送的接收人中无效的用户名
                                                               // string invalidparty = (string)dd["invalidparty"];//发送的接收人中无效的部门
                                                               //string invalidtag = (string)dd["invalidtag"];//发送的接收人中无效的标签
                return;
            }
            //如果全部接收人无权限或不存在，则本次调用返回失败，errcode为81013。
            if (errcode.Equals("81013"))
            {
                return;
            }

            return;
        }

        /// <summary>
        /// 检查是否是手机号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMobile(string str)
        {
            return BP.DA.DataType.IsMobile(str);

        }

        public static string getValue(string msgstr)
        {
            return HttpUtility.UrlDecode(HttpContext.Current.Request[msgstr] ?? string.Empty);
        }
    }
}
