using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BP.Tools;
using BP.En;

namespace BP.EAI.Plugins.WXin
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
            return null;

            /*

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
             * */
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
                    if (!string.IsNullOrEmpty(item.url)) append_Json.Append(",\"url\":\"" + item.url + "\"");
                    if (!string.IsNullOrEmpty(item.picurl)) append_Json.Append(",\"picurl\":\"" + item.picurl + "\"");
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

        /// <summary>
        /// 发送待办消息
        /// </summary>
        /// <param name="WorkID">业务编号</param>
        /// <param name="sender">发送人</param>
        /// <returns></returns>
        public static MessageErrorModel PostEmpWorksMsgOfNews(long WorkID, string sender)
        {
            return null;
            /*
            //企业应用必须存在
            string agentId = BP.Sys.SystemConfig.WX_AgentID ?? null;
            if (agentId != null)
            {
                string accessToken = new BP.EAI.Plugins.WXin.WeiXin().getAccessToken();//获取 AccessToken

                //当前业务
                GenerWorkFlow gwf = new GenerWorkFlow();
                gwf.WorkID = WorkID;
                gwf.RetrieveFromDBSources();
                //接收人
                Monitors empWorks = new Monitors();
                QueryObject obj = new QueryObject(empWorks);
                obj.AddWhere(MonitorAttr.WorkID, WorkID);
                obj.addOr();
                obj.AddWhere(MonitorAttr.FID, WorkID);
                obj.DoQuery();
                string toUsers = "";
                foreach (Monitor empWork in empWorks)
                {
                    if (toUsers.Length > 0)
                        toUsers += "|";
                    toUsers += empWork.FK_Emp;
                }
                if (toUsers.Length == 0)
                    return null;

                News_Articles newArticle = new News_Articles();
                newArticle.title = gwf.Title;

                string msgConten = "流程名称：" + gwf.FlowName + "\n";
                msgConten += "发 起 人：" + gwf.StarterName + "\n";
                msgConten += "发起人部门：" + gwf.DeptName + "\n";
                msgConten += "当前节点：" + gwf.NodeName + "\n";
                msgConten += "发 送 人：" + sender + "\n";
                newArticle.description = msgConten;

                string New_Url = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + BP.Sys.SystemConfig.WX_CorpID
                    + "&redirect_uri=" + BP.Sys.SystemConfig.WX_MessageUrl + "/CCMobile/action.aspx&response_type=code&scope=snsapi_base&state=empwork_" + WorkID + "#wechat_redirect";
                newArticle.url = New_Url;

                //http://discuz.comli.com/weixin/weather/icon/cartoon.jpg
                //newArticle.picurl = BP.Sys.SystemConfig.WX_MessageUrl + "/DataUser/ICON/" + BP.Sys.SystemConfig.SysNo + "/LogBig.png";

                WX_Msg_News wxMsg = new WX_Msg_News();
                wxMsg.Access_Token = accessToken;
                wxMsg.agentid = BP.Sys.SystemConfig.WX_AgentID;
                wxMsg.touser = toUsers;
                wxMsg.articles.Add(newArticle);
                //执行发送
                return WeiXinMessage.PostMsgOfNews(wxMsg);
            }
            return null;
             * */
        }
    }
}
