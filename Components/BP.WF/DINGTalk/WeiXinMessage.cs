using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BP.Tools;

namespace BP.WF.WXin
{
    /// <summary>
    /// 微信消息处理
    /// </summary>
    public class WeiXinMessage
    {
        /// <summary>
        /// 发送文本消息
        /// </summary>
        /// <param name="msgText">消息实体类</param>
        /// <returns>发送消息结果</returns>
        public static MessageErrorModel PostMsgOfText(WX_Msg_Text msgText)
        {
            string url = "https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token=" + msgText.Access_Token;
            try
            {
                StringBuilder append_Json = new StringBuilder();
                append_Json.Append("{");
                append_Json.Append("\"msgtype\":\"text\"");
                //按人员
                if (!string.IsNullOrEmpty(msgText.touser)) append_Json.Append(",\"touser\":\"" + msgText.touser + "\"");
                //按部门
                if (!string.IsNullOrEmpty(msgText.toparty)) append_Json.Append(",\"toparty\":\"" + msgText.toparty + "\"");
                //标签
                if (!string.IsNullOrEmpty(msgText.totag)) append_Json.Append(",\"totag\":\"" + msgText.totag + "\"");
                append_Json.Append(",\"agentid\":\"" + msgText.agentid + "\"");
                append_Json.Append(",\"text\":{");
                append_Json.Append("\"content\":\"" + msgText.content + "\"");
                append_Json.Append("}");
                append_Json.Append(",\"safe\":\"" + msgText.safe + "\"");
                append_Json.Append("}");
                string str = new HttpWebResponseUtility().HttpResponsePost_Json(url, append_Json.ToString());
                MessageErrorModel postVal = FormatToJson.ParseFromJson<MessageErrorModel>(str);
                return postVal;
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        /// <summary>
        /// 发送新闻消息
        /// </summary>
        /// <param name="msgNews">消息实体类</param>
        /// <returns>发送消息结果</returns>
        public static MessageErrorModel PostMsgOfNews(WX_Msg_News msgNews)
        {
            string url = "https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token=" + msgNews.Access_Token;
            try
            {
                StringBuilder append_Json = new StringBuilder();
                append_Json.Append("{");
                append_Json.Append("\"msgtype\":\"news\"");
                //按人员
                if (!string.IsNullOrEmpty(msgNews.touser)) append_Json.Append(",\"touser\":\"" + msgNews.touser + "\"");
                //按部门
                if (!string.IsNullOrEmpty(msgNews.toparty)) append_Json.Append(",\"toparty\":\"" + msgNews.toparty + "\"");
                //标签
                if (!string.IsNullOrEmpty(msgNews.totag)) append_Json.Append(",\"totag\":\"" + msgNews.totag + "\"");

                append_Json.Append(",\"agentid\":\"" + msgNews.agentid + "\"");
                append_Json.Append(",\"news\":{");

                append_Json.Append("\"articles\":[");
                foreach (News_Articles item in msgNews.articles)
                {
                    append_Json.Append("{");
                    append_Json.Append("\"title\":\"" + item.title + "\"");
                    append_Json.Append(",\"description\":\"" + item.description + "\"");
                    string New_Url = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + BP.Sys.SystemConfig.WX_CorpID
                    + "&redirect_uri=http://demo.ccflow.org:8006/CCMobile/action.aspx&response_type=code&scope=snsapi_base&state=Start#wechat_redirect";
                    append_Json.Append(",\"url\":\"" + New_Url + "\"");
                    append_Json.Append(",\"picurl\":\"http://discuz.comli.com/weixin/weather/icon/cartoon.jpg\"");
                    append_Json.Append("},");
                }
                append_Json.Remove(append_Json.Length - 1, 1);

                append_Json.Append("]}");
                append_Json.Append("}");
                string str = new HttpWebResponseUtility().HttpResponsePost_Json(url, append_Json.ToString());
                MessageErrorModel postVal = FormatToJson.ParseFromJson<MessageErrorModel>(str);
                return postVal;
            }
            catch (Exception ex)
            {
            }
            return null;
        }
    }
}
