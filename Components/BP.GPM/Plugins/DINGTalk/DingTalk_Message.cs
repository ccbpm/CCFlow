using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;
using BP.Tools;
using BP.GPM;

namespace BP.EAI.Plugins.DINGTalk
{
    /// <summary>
    /// 钉钉消息类型
    /// </summary>
    public enum DingMsgType
    {
        /// <summary>
        /// 文本消息
        /// </summary>
        text,
        /// <summary>
        /// 声音，vido
        /// </summary>
        voice,
        /// <summary>
        /// 图片消息
        /// </summary>
        image,
        /// <summary>
        /// 文件消息
        /// </summary>
        file,
        /// <summary>
        /// 超链接消息
        /// </summary>
        link,
        /// <summary>
        /// OA消息
        /// </summary>
        OA
    }

    /// <summary>
    /// 钉钉消息处理类
    /// by dgq 2016.5.9
    /// </summary>
    public class DingTalk_Message
    {
        /// <summary>
        /// 普通消息
        /// </summary>
        /// <param name="dingMsg"></param>
        /// <returns></returns>
        public static Ding_Post_ReturnVal Msg_AgentText_Send(Ding_Msg_Text dingMsg)
        {
            string url = "https://oapi.dingtalk.com/message/send?access_token=" + dingMsg.Access_Token;
            try
            {
                StringBuilder append_Json = new StringBuilder();
                append_Json.Append("{");
                append_Json.Append("\"touser\":\"" + dingMsg.touser + "\"");
                append_Json.Append(",\"msgtype\":\"text\"");
                append_Json.Append(",\"agentid\":\"" + dingMsg.agentid + "\"");
                append_Json.Append(",\"text\":{\"content\":\"" + dingMsg.content + "\"}");
                append_Json.Append("}");
                string str = new HttpWebResponseUtility().HttpResponsePost_Json(url, append_Json.ToString());
                Ding_Post_ReturnVal postVal = FormatToJson.ParseFromJson<Ding_Post_ReturnVal>(str);
                return postVal;
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        /// <summary>
        /// 带有超链接消息
        /// </summary>
        /// <param name="dingMsg"></param>
        /// <returns></returns>
        public static Ding_Post_ReturnVal Msg_AgentLink_Send(Ding_Msg_Link dingMsg)
        {
            string url = "https://oapi.dingtalk.com/message/send?access_token=" + dingMsg.Access_Token;
            try
            {
                StringBuilder append_Json = new StringBuilder();
                append_Json.Append("{");
                append_Json.Append("\"touser\":\"" + dingMsg.touser + "\"");
                append_Json.Append(",\"msgtype\":\"link\"");
                append_Json.Append(",\"agentid\":\"" + dingMsg.agentid + "\"");
                append_Json.Append(",\"link\":{");
                append_Json.Append("\"messageUrl\":\"" + dingMsg.messageUrl + "\"");
                append_Json.Append(",\"picUrl\":\"" + dingMsg.picUrl + "\"");
                append_Json.Append(",\"title\":\"" + dingMsg.title + "\"");
                append_Json.Append(",\"text\":\"" + dingMsg.text + "\"");
                append_Json.Append("}");
                append_Json.Append("}");
                string str = new HttpWebResponseUtility().HttpResponsePost_Json(url, append_Json.ToString());
                Ding_Post_ReturnVal postVal = FormatToJson.ParseFromJson<Ding_Post_ReturnVal>(str);
                return postVal;
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        /// <summary>
        /// 发送OA型消息
        /// </summary>
        /// <param name="dingMsg"></param>
        /// <returns></returns>
        public static Ding_Post_ReturnVal Msg_OAText_Send(Ding_Msg_OA dingMsg)
        {
            string url = "https://oapi.dingtalk.com/message/send?access_token=" + dingMsg.Access_Token;
            try
            {
                StringBuilder append_Json = new StringBuilder();
                append_Json.Append("{");
                append_Json.Append("\"touser\":\"" + dingMsg.touser + "\"");
                append_Json.Append(",\"msgtype\":\"oa\"");
                append_Json.Append(",\"agentid\":\"" + dingMsg.agentid + "\"");
                append_Json.Append(",\"oa\":{");
                append_Json.Append("\"message_url\":\"" + dingMsg.messageUrl + "\"");

                append_Json.Append(",\"head\":{");
                append_Json.Append("\"bgcolor\":\"" + dingMsg.head_bgcolor + "\"");
                append_Json.Append(",\"text\":\"" + dingMsg.head_text + "\"");
                append_Json.Append("}");

                append_Json.Append(",\"body\":{");
                append_Json.Append("\"title\":\"" + dingMsg.body_title + "\"");
                if (dingMsg.body_form.Count > 0)
                {
                    append_Json.Append(",\"form\":[");
                    foreach (string itemKey in dingMsg.body_form.Keys)
                    {
                        append_Json.Append("{");
                        append_Json.Append("\"key\":\"" + itemKey + "\"");
                        append_Json.Append(",\"value\":\"" + dingMsg.body_form[itemKey] + "\"");
                        append_Json.Append("},");
                    }
                    append_Json.Remove(append_Json.Length - 1, 1);
                    append_Json.Append("]");
                }
                
                append_Json.Append(",\"author\":\"" + dingMsg.body_author + "\"");
                append_Json.Append("}");
                append_Json.Append("}");
                append_Json.Append("}");
                string str = new HttpWebResponseUtility().HttpResponsePost_Json(url, append_Json.ToString());
                Ding_Post_ReturnVal postVal = FormatToJson.ParseFromJson<Ding_Post_ReturnVal>(str);
                return postVal;
            }
            catch (Exception ex)
            {
            }
            return null;
        }
    }
}
