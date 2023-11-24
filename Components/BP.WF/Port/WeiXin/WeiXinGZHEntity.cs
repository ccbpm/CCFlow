using BP.DA;
using BP.Sys;
using BP.Tools;
using BP.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static BP.Port.WeiXin.Msg.WeiXinGZHModel;

namespace BP.Port.WeiXin
{
    public class WeiXinGZHEntity
    {
        #region 基本配置.
        /// <summary>
        /// 微信公众号应用的分配的单位ID.
        /// 格式:wx8eac6a18c5efec30
        /// </summary>
        public static string Appid
        {
            get
            {
                return BP.Difference.SystemConfig.WXGZH_Appid;// "wx8eac6a18c5efec30";
            }
        }
        /// <summary>
        /// 微信公众号开发则密码.
        /// </summary>
        public static string AppSecret
        {
            get
            {
                return BP.Difference.SystemConfig.WXGZH_AppSecret;// "KfFkE9AZ3Zp09zTuKvmqWLgtLj-_cHMPTvV992apOWgSKJHcbjpbu1jYVXh7gI7K";
            }
        }
        /// <summary>
        /// 微信公众号消息模版.
        /// </summary>
        public static string WeiXinGZHMessageID
        {
            get
            {
                return BP.Difference.SystemConfig.WeiXinGZHMessageID;
            }
        }
        /// <summary>
        /// 微信公众号消息回调地址.
        /// </summary>
        public static string GZH_redirect_uri
        {
            get
            {
                return BP.Difference.SystemConfig.GZH_redirect_uri;
            }
        }
        #endregion 基本配置.

        #region 获取用户access_token
        /// <summary>
        /// 获得token,每间隔x分钟，就会失效.
        /// </summary>
        /// <returns>token</returns>
        public static AccessToken getAccessToken(string code)
        {
            string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + Appid + "&secret=" + AppSecret
                + "&code=" + code + "&grant_type=authorization_code";

            AccessToken at = new AccessToken();
            string str = BP.DA.DataType.ReadURLContext(url, 5000, Encoding.UTF8);
            at = FormatToJson.ParseFromJson<AccessToken>(str);

            return at;
        }
        /// <summary>
        /// 微信网页开发获取token
        /// </summary>
        /// <returns></returns>
        public static AccessToken getAccessToken()
        {
            string url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + Appid + "&secret=" + AppSecret;

            AccessToken at = new AccessToken();
            string str = BP.DA.DataType.ReadURLContext(url, 5000, Encoding.UTF8);
            at = FormatToJson.ParseFromJson<AccessToken>(str);
            return at;
        }
        #endregion 获取用户access_token

        #region 获取用户信息
        public static GZHUser getUserInfo(string access_token,string openid)
        {
            string url = "https://api.weixin.qq.com/sns/userinfo?access_token=" + access_token + "&openid=" + openid
                + "&lang=zh_CN";

            GZHUser user = new GZHUser();
            string str = BP.DA.DataType.ReadURLContext(url, 5000, Encoding.UTF8);
            user = FormatToJson.ParseFromJson<GZHUser>(str);
            return user;
        }
        #endregion 获取用户信息

        public static string GetTicket(string access_token,string type) {
            string url = "https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token=" + access_token;

            string postData = "{\"expire_seconds\":604800,\"action_name\":\"QR_SCENE\",\"action_info\":{\"scene\":{\"scene_id\":123}}}";

            if (type=="124")
                postData = "{\"expire_seconds\":604800,\"action_name\":\"QR_SCENE\",\"action_info\":{\"scene\":{\"scene_id\":124}}}";

            string str = BP.Tools.PubGlo.HttpPostConnect(url, postData, "POST", true);
           

            Ticket ticket= FormatToJson.ParseFromJson<Ticket>(str);
            return ticket.ticket;
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="touser"></param>
        /// <returns></returns>
        public static string SendMsg(string touser) {
            try
            {
                string redircet_uri = GZH_redirect_uri + "?OpenID=" + touser;
                string url = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token=" + getAccessToken().access_token;
                string postData = "{\"touser\":\"" + touser + "\",\"template_id\":\"" + WeiXinGZHMessageID + "\"," +
                    "\"url\":\"" + redircet_uri + "\",\"data\":{\"thing5\":{\"value\":\"工单系统认证通知\"}," +
                    "\"time3\":{\"value\":\"" + DateTime.Now.ToString("yyyy-MM-dd hh:mm") + "\"}}}";

                string str = BP.Tools.PubGlo.HttpPostConnect(url, postData, "POST", true);
                return str;
            }
            catch (Exception ex) {
                return ex.Message;
            }
        }
    }
}
