
using BP.GPM.DTalk.DINGTalk;
using BP.GPM.WeiXin;
using BP.Sys;
using BP.Web;
using System;
using System.Collections;
using System.Collections.Generic;


namespace CCFlow.DataUser
{
    public partial class HandlerOfMessage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string doType = this.Request.QueryString["DoType"]; //消息类型标记,在节点事件上配置的标记.
            switch (doType)
            {
                case "SendToCCMSG":
                    byte[] data = new byte[this.Request.InputStream.Length];
                    this.Request.InputStream.Read(data, 0, data.Length); //获得传入来的数据.
                    string txt = System.Text.Encoding.UTF8.GetString(data);  //编码.

                    //转成json.
                    Dictionary<string, object> dictionary = null;
                    dictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(txt);

                    //获得里面的参数.
                    string send = dictionary["sender"].ToString(); //发送人.
                    string sendTo = dictionary["sendTo"].ToString(); //发送给 与人员表Port_Emp的No一致.
                    string tel = "";
                    if (dictionary["tel"] != null) //配置的电话。
                        tel = dictionary["tel"].ToString();

                    string title = dictionary["title"].ToString(); //标题
                    string content = dictionary["content"].ToString(); //信息内容.
                    string openUrl = dictionary["openUrl"].ToString(); //要打开的url.
                    break;
                case "SendToWeiXin":
                    try
                    {
                        string agentId = BP.Sys.SystemConfig.WX_AgentID ?? null;
                        if (agentId != null)
                        {
                            string accessToken = BP.GPM.WeiXin.WeiXinEntity.getAccessToken();//获取 AccessToken

                            News_Articles newArticle = new News_Articles();
                            newArticle.description = this.Request.QueryString["msgConten"];

                            newArticle.title = "您有一条待办消息";
                            string New_Url = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + BP.Sys.SystemConfig.WX_CorpID
                                + "&redirect_uri=" + BP.Sys.SystemConfig.WX_MessageUrl + "/CCMobile/action.aspx&response_type=code&scope=snsapi_base&state=TodoList#wechat_redirect";
                            newArticle.url = New_Url;

                            //http://discuz.comli.com/weixin/weather/icon/cartoon.jpg
                            newArticle.picurl = BP.Sys.SystemConfig.WX_MessageUrl + "/DataUser/ICON/ccicon.png";

                            BP.GPM.Emp emp = new BP.GPM.Emp(this.Request.QueryString["sendTo"]);

                            WX_Msg_News wxMsg = new WX_Msg_News();
                            wxMsg.Access_Token = accessToken;
                            wxMsg.agentid = BP.Sys.SystemConfig.WX_AgentID;
                            wxMsg.touser = emp.Tel;
                            wxMsg.articles.Add(newArticle);
                            //执行发送
                            WeiXinMessage.PostMsgOfNews(wxMsg);
                        }
                    }
                    catch (Exception ex)
                    { }
                    break;
                case "SendToDingDing":
                    //企业应用必须存在
                    string dId = BP.Sys.SystemConfig.Ding_AgentID ?? null;
                    Ding_Post_ReturnVal postVal = null;
                    if (dId != null)
                    {
                        Ding_Msg_OA msgOA = new Ding_Msg_OA();
                        msgOA.Access_Token = BP.GPM.DTalk.DingDing.getAccessToken();
                        msgOA.agentid = SystemConfig.Ding_AgentID;
                        msgOA.touser = this.Request.QueryString["sendTo"];
                        msgOA.messageUrl = SystemConfig.Ding_MessageUrl + "/CCMobile/DingTalk.aspx";
                        //00是完全透明，ff是完全不透明，比较适中的透明度值是 1e
                        msgOA.head_bgcolor = "FFBBBBBB";
                        msgOA.head_text = "审批";
                        msgOA.body_title = this.Request.QueryString["title"];
                        Hashtable hs = new Hashtable();
                        hs.Add("审批内容", this.Request.QueryString["msgConten"]);
                        msgOA.body_form = hs;
                        msgOA.body_author = WebUser.No;
                        postVal = DingTalk_Message.Msg_OAText_Send(msgOA);
                    }
                    break;

            }
            return;
        }
    }
}