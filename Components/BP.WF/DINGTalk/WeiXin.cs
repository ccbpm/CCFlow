using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;
using BP.Tools;

namespace BP.WF.WXin
{
    public class WeiXin
    {
        private string appid = BP.Sys.SystemConfig.WX_CorpID;// "wx8eac6a18c5efec30";
        private string appsecret = BP.Sys.SystemConfig.WX_AppSecret;// "KfFkE9AZ3Zp09zTuKvmqWLgtLj-_cHMPTvV992apOWgSKJHcbjpbu1jYVXh7gI7K";
        public string getAccessToken()
        {
            string accessToken = string.Empty;
            string url = "https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid=" + appid + "&corpsecret=" + appsecret + "";

            try
            {
                AccessToken AT = new AccessToken();
                HttpWebResponse response = new HttpWebResponseUtility().CreateGetHttpResponse(url, 10000, null, null);
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string str = reader.ReadToEnd();
                AT = FormatToJson.ParseFromJson<AccessToken>(str);
                reader.Dispose();
                reader.Close();
                if (response != null) response.Close();
                if (AT != null)
                {
                    accessToken = AT.access_token;
                }
            }
            catch
            {
            }
            return accessToken;
        }

        #region 发送微信信息
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

            HttpWebResponse response = new HttpWebResponseUtility().WXCreateGetHttpResponse(url, parameters, 10000, null, Encoding.UTF8, null);
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string str = reader.ReadToEnd();
            WriteLog(url + "----------------" + parameters + "---------------" + str);
            return str;
        }
        #endregion
        #region 记录相关交互日志
        /// 写日志(用于跟踪)
        /// </summary>
        private void WriteLog(string strMemo)
        {
            if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(@"logs\")))
            {
                Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(@"logs\"));
            }
            string filename = System.Web.HttpContext.Current.Server.MapPath(@"logs/" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
            StreamWriter sr = null;
            try
            {
                if (!File.Exists(filename))
                {
                    sr = File.CreateText(filename);
                }
                else
                {
                    sr = File.AppendText(filename);
                }
                sr.WriteLine(strMemo);
            }
            catch
            {
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }
        }
        #endregion
    }
}
