using BP.DA;
using BP.GPM.DTalk.DINGTalk;
using BP.GPM.WeiXin;
using BP.Sys;
using BP.Web;
using BP.WF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace CCFlow.DataUser
{
    public partial class HandlerOfMessage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            byte[] data;
            string txt;
            Dictionary<string, object> dictionary = null;
            string doType = this.Request.QueryString["DoType"]; //消息类型标记,在节点事件上配置的标记.
            switch (doType)
            {
                case "SendToCCMSG":
                    data = new byte[this.Request.InputStream.Length];
                    this.Request.InputStream.Read(data, 0, data.Length); //获得传入来的数据.
                    txt = System.Text.Encoding.UTF8.GetString(data);  //编码.

                    //转成json.

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
                        data = new byte[this.Request.InputStream.Length];
                        this.Request.InputStream.Read(data, 0, data.Length); //获得传入来的数据.
                        txt = System.Text.Encoding.UTF8.GetString(data);  //编码.

                        dictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(txt);

                        //获取参数信息
                        string msgContent = dictionary["content"].ToString();
                        string msgFlg = dictionary["msgFlag"].ToString();
                        int fk_node = 0;
                        Int64 workid = 0;
                        string fk_flow = "";
                        if (DataType.IsNullOrEmpty(msgFlg) == false)
                        {
                            string[] msgFlgs = msgFlg.Split('_');
                            fk_node = int.Parse(msgFlgs[0]);
                            workid = Int64.Parse(msgFlgs[1]);
                            if(msgFlgs.Length>=3)
                                fk_flow = msgFlg.Split('_')[2];
                        }
                        string agentId = SystemConfig.WX_AgentID ?? null;
                        if (agentId != null)
                        {
                            string accessToken = BP.GPM.WeiXin.WeiXinEntity.getAccessToken();//获取 AccessToken

                            NewsArticles newArticle = new NewsArticles();
                            newArticle.title = "您有一条待办消息";
                            string New_Url = "";
                            if (msgContent.StartsWith("http:") == true)
                            {
                                byte[] bytes = UTF8Encoding.UTF8.GetBytes(msgContent);
                                msgContent = Convert.ToBase64String(bytes);
                                

                                New_Url = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + SystemConfig.WX_CorpID
                                 + "&redirect_uri=" + SystemConfig.WX_MessageUrl + "/CCMobile/action.aspx&response_type=code&scope=snsapi_base&state=URL_" + msgContent + ",WorkID_" + workid + ",FK_Node_" + fk_node+",FK_Flow_"+ fk_flow + "#wechat_redirect";
                                GenerWorkFlow gwf = new GenerWorkFlow(workid);
                                string str = "\t\n您好:";
                                str += "\t\n    工作{" + gwf.Title + "}有一条新消息 .";
                                str += "\t\n    发起人" + gwf.StarterName;
                                str += "\t\n    发起时间" + gwf.SendDT;
                                newArticle.description = str;
                            }
                            else
                            {
                                newArticle.description = msgContent;
                                New_Url = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + SystemConfig.WX_CorpID
                                + "&redirect_uri=" + SystemConfig.WX_MessageUrl + "/CCMobile/action.aspx&response_type=code&scope=snsapi_base&state=MyView,WorkID_" + workid + ",FK_Node_" + fk_node + ",FK_Flow_" + fk_flow + "#wechat_redirect";
                            }

                            newArticle.url = New_Url;

                            //http://discuz.comli.com/weixin/weather/icon/cartoon.jpg
                            newArticle.picurl = SystemConfig.WX_MessageUrl + "/DataUser/ICON/ccicon.png";

                            BP.GPM.Emp emp = new BP.GPM.Emp(this.Request.QueryString["sendTo"]);

                            MsgNews wxMsg = new MsgNews();
                            wxMsg.Access_Token = accessToken;
                            wxMsg.agentid = SystemConfig.WX_AgentID;
                            wxMsg.touser = emp.Tel;
                            wxMsg.articles.Add(newArticle);
                            //执行发送
                            BP.GPM.WeiXin.Glo.PostMsgOfNews(wxMsg);
                        }
                    }
                    catch (Exception ex)
                    { }
                    break;
                case "SendToDingDing":
                    //企业应用必须存在
                    string dId = SystemConfig.Ding_AgentID ?? null;
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