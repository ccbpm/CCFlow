using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;
using BP.Tools;
using BP.DA;
using BP.WF;
using System.Collections;
using BP.Sys;

namespace BP.GPM.WeiXin
{
    /// <summary>
    /// 微信实体类
    /// </summary>
    public class WeiXinEntity
    {
        #region 基本配置.
        /// <summary>
        /// 微信应用的分配的单位ID.
        /// 格式:wx8eac6a18c5efec30
        /// </summary>
        public static string appid
        {
            get
            {
                return SystemConfig.WX_CorpID;// "wx8eac6a18c5efec30";
            }
        }
        /// <summary>
        /// 微信应用的分配给单位的一个加密字符串, 标识这个值对应的是这个单位的应用.
        /// 格式:KfFkE9AZ3Zp09zTuKvmqWLgtLj
        /// 也就是密钥.
        /// </summary>
        public static string appsecret
        {
            get
            {
                return SystemConfig.WX_AppSecret;// "KfFkE9AZ3Zp09zTuKvmqWLgtLj-_cHMPTvV992apOWgSKJHcbjpbu1jYVXh7gI7K";
            }
        }
        /// <summary>
        /// 获得token,每间隔x分钟，就会失效.
        /// </summary>
        /// <returns>token</returns>
        public static string getAccessToken()
        {
            string accessToken = string.Empty;
            string url = "https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid=" + appid + "&corpsecret=" + appsecret;

            AccessToken AT = new AccessToken();
            string str = BP.DA.DataType.ReadURLContext(url, 5000, Encoding.UTF8);
            AT = FormatToJson.ParseFromJson<AccessToken>(str);
            accessToken = AT.access_token;

            return accessToken;
        }
        #endregion 基本配置.

        #region 应用方法.
        /// <summary>
        /// 调用企业号获取地理位置
        /// </summary>
        /// <returns></returns>
        public static string GetWXConfigSetting(string pageUrl)
        {
            //必须是当前页面，如果在CCMobile/Home.htm调用，则传入Home.htm
            string htmlPage = pageUrl;
            Hashtable ht = new Hashtable();

            //生成签名的时间戳
            string timestamp = DateTime.Now.ToString("yyyyMMDDHHddss");
            //生成签名的随机串
            string nonceStr = DBAccess.GenerGUID();
            //企业号jsapi_ticket
            string jsapi_ticket = "";
            string url1 = htmlPage;
            //获取 AccessToken
            string accessToken = getAccessToken();

            string url = "https://qyapi.weixin.qq.com/cgi-bin/ticket/get?access_token="+ accessToken + "&type=wx_card";
            string str = DataType.ReadURLContext(url, 9999, null);

            //权限签名算法
            Ticket ticket = new Ticket();
            ticket = FormatToJson.ParseFromJson<Ticket>(str);

            if (ticket.errcode == "0")
                jsapi_ticket = ticket.ticket;
            else
                return "err:@获取jsapi_ticket失败+accessToken=" + accessToken;

            ht.Add("timestamp", timestamp);
            ht.Add("nonceStr", nonceStr);
            //企业微信的corpID
            ht.Add("AppID", SystemConfig.WX_CorpID);

            //生成签名算法
            string str1 = "jsapi_ticket=" + jsapi_ticket + "&noncestr=" + nonceStr + "&timestamp=" + timestamp + "&url=" + url1 + "";
            string Signature = Sha1Signature(str1);
            ht.Add("signature", Signature);

            return BP.Tools.Json.ToJson(ht);
        }
        #endregion 应用方法.

        #region 发送微信信息.
        public MessageErrorModel PostWeiXinMsg(StringBuilder sb)
        {
            string wxStr = string.Empty;
            string url = "https://qyapi.weixin.qq.com/cgi-bin/message/send?";

            MessageErrorModel m = new MessageErrorModel();
            wxStr = PostForWeiXin(sb, url);
            m = FormatToJson.ParseFromJson<MessageErrorModel>(wxStr);
            return m;
        }
        /// <summary>
        /// POST方式请求 微信返回信息
        /// </summary>
        /// <param name="parameters">参数</param>
        /// <param name="URL">请求地址</param>
        /// <returns>返回字符</returns>
        public string PostForWeiXin(StringBuilder parameters, string URL)
        {
            string access_token = getAccessToken();
            string url = URL + "access_token=" + access_token;

            //todo:zqp.该方法没有完善.
            string str = DataType.ReadURLContext(url, 9999, null, parameters);

            //HttpWebResponse response = new HttpWebResponseUtility().WXCreateGetHttpResponse(url, parameters,
            //    10000, null, Encoding.UTF8, null);
            //StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            //string str = reader.ReadToEnd();

            Log.DebugWriteInfo(url + "----------------" + parameters + "---------------" + str);
            return str;
        }
        #endregion

        
        /// <summary>
        /// 算法加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Sha1Signature(string str)
        {
            string s = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "SHA1").ToString();
            return s.ToLower();
        }
    }
}
