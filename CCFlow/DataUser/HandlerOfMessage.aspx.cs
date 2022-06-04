using BP.DA;
using BP.GPM.DTalk.DINGTalk;
using BP.GPM.WeiXin;
using BP.Sys;
using BP.Tools;
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
        public string GetVal(string key)
        {
            string val = this.Request.QueryString[key];
            return BP.Tools.DealString.DealStr(val);
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            byte[] data;
            string txt;
            int fk_node = 0;
            Int64 workid = 0;
            string msgFlg = "";
            Dictionary<string, object> dictionary = null;
            string doType = this.GetVal("DoType"); //消息类型标记,在节点事件上配置的标记.
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

                        //转成json格式
                        dictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(txt);

                        //获取参数信息
                        string msgContent = dictionary["content"].ToString();
                        msgFlg = dictionary["msgFlag"].ToString();
                        
                        //获取关键数据
                        if (DataType.IsNullOrEmpty(msgFlg) == false)
                        {
                            fk_node = int.Parse(msgFlg.Split('_')[0]);
                            workid = Int64.Parse(msgFlg.Split('_')[1]);
                        }
                        //微信企业号ID
                        string agentId = BP.Difference.SystemConfig.WX_AgentID ?? null;
                        if (agentId != null)
                        {
                            //申请权限，获取token
                            string accessToken = BP.GPM.WeiXin.WeiXinEntity.getAccessToken();//获取 AccessToken

                            //组织消息模版
                            NewsArticles newArticle = new NewsArticles();
                            newArticle.title = "您有一条待办消息";
                            string New_Url = "";
                            if (msgContent.StartsWith("http:") == true)
                            {
                                byte[] bytes = UTF8Encoding.UTF8.GetBytes(msgContent);
                                msgContent = Convert.ToBase64String(bytes);

                                //点击消息时，打开的连接
                                New_Url = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + BP.Difference.SystemConfig.WX_CorpID
                                 + "&redirect_uri=" + BP.Difference.SystemConfig.WX_MessageUrl + "/CCMobile/action.aspx&response_type=code&scope=snsapi_base&state=URL_" + msgContent + ",WorkID_" + workid + ",FK_Node_" + fk_node + "#wechat_redirect";
                                GenerWorkFlow gwf = new GenerWorkFlow(workid);
                               //消息显示格式
                                string str = "\t\n您好:";
                                str += "\t\n    工作{" + gwf.Title + "}有一条新消息 .";
                                str += "\t\n    发起人" + gwf.StarterName;
                                str += "\t\n    发起时间" + gwf.SendDT;
                                newArticle.description = str;
                            }
                            else
                            {
                                newArticle.description = msgContent;
                                New_Url = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + BP.Difference.SystemConfig.WX_CorpID
                                + "&redirect_uri=" + BP.Difference.SystemConfig.WX_MessageUrl + "/CCMobile/action.aspx&response_type=code&scope=snsapi_base&state=MyView,WorkID_" + workid + ",FK_Node_" + fk_node + "#wechat_redirect";
                            }

                            newArticle.url = New_Url;
                            //消息模块中显示的图片
                            //http://discuz.comli.com/weixin/weather/icon/cartoon.jpg
                            newArticle.picurl = BP.Difference.SystemConfig.WX_MessageUrl + "/DataUser/ICON/ccicon.png";

                            BP.Port.Emp emp = new BP.Port.Emp(this.GetVal("sendTo"));

                            //新消息类模版
                            MsgNews wxMsg = new MsgNews();
                            wxMsg.Access_Token = accessToken;
                            wxMsg.agentid = BP.Difference.SystemConfig.WX_AgentID;
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
                    //钉消息发送是否成功，可查看日志：DataUser/Log
                    data = new byte[this.Request.InputStream.Length];
                    this.Request.InputStream.Read(data, 0, data.Length); //获得传入来的数据.
                    txt = System.Text.Encoding.UTF8.GetString(data);  //编码.

                    //转成json.
                    dictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(txt);
                    //获取关键数据，FK_Node，WorkID
                    msgFlg = dictionary["msgFlag"].ToString();
                    if (DataType.IsNullOrEmpty(msgFlg) == false)
                    {
                        fk_node = int.Parse(msgFlg.Split('_')[0]);
                        workid = Int64.Parse(msgFlg.Split('_')[1]);
                    }
                    Node nd = new Node(fk_node);
                    //获取应用ID，企业应用必须存在
                    string dId = BP.Difference.SystemConfig.Ding_AgentID ?? null;
                    //接收钉消息发送后返回值的类
                    Ding_Post_ReturnVal postVal = null;
                    //获取处理人的手机号
                    BP.Port.Emp empUser = new BP.Port.Emp(this.GetVal("sendTo"));
                    //Log.DefaultLogWriteLineError("处理人：" + this.GetVal("sendTo"]);
                    //获取权限，token
                    string access_token= BP.GPM.DTalk.DingDing.getAccessToken();
                    //根据手机号获取成员的userid
                    string url = "https://oapi.dingtalk.com/user/get_by_mobile?access_token=" + access_token + "&mobile=" + empUser.Tel;
                    string strJson = new HttpWebResponseUtility().HttpResponseGet(url);
                    //Log.DefaultLogWriteLineError("获取接收人的信息："+strJson);
                    CreateUser_PostVal user = new CreateUser_PostVal();
                    user = FormatToJson.ParseFromJson<CreateUser_PostVal>(strJson);

                    if (dId != null)
                    {
                        //消息模版，touser必须是userid
                        Ding_Msg_OA msgOA = new Ding_Msg_OA();
                        msgOA.Access_Token = BP.GPM.DTalk.DingDing.getAccessToken();
                        msgOA.agentid = BP.Difference.SystemConfig.Ding_AgentID;
                        msgOA.touser = user.userid;
                        //打开消息时，跳转的路径
                        msgOA.messageUrl = BP.Difference.SystemConfig.Ding_MessageUrl + "/CCMobile/DingTalk.aspx?ActionType=ToDo&FK_Flow="+ nd.FK_Flow + "&WorkID="+workid+"&FK_Node="+ fk_node + "&m="+DateTime.Now.ToString("yyyyMMddHHmmss");
                        //00是完全透明，ff是完全不透明，比较适中的透明度值是 1e
                        msgOA.head_bgcolor = "FFBBBBBB";
                        msgOA.head_text = "审批";
                        msgOA.body_title = dictionary["title"].ToString();
                        //消息主体，可自定义
                        Hashtable hs = new Hashtable();
                        hs.Add("审批内容", "您有一条待办工作需审核。");
                        msgOA.body_form = hs;
                        msgOA.body_author = WebUser.No;
                        //执行消息发送
                        postVal = DingTalk_Message.Msg_OAText_Send(msgOA);
                    }
                    break;

            }
            return;
        }
    }
}