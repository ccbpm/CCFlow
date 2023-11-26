
using BP.DA;
using BP.Sys;
using BP.WF.Template;
using BP.Difference;
using BP.Port.DINGTalk.Utility;
using BP.Port.DINGTalk;
using BP.Port.WeiXin.Msg;
using BP.Tools;
using BP.Web;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Text;

namespace BP.WF
{
    /// <summary>
    /// 事件重写.
    /// </summary>
    public class OverrideEvent
    {
        #region 消息机制个性化处理 - 用户可以根据自己的需求改造代码 - 注意不要提交仓库.
        public static void SendToEmail(SMS sms)
        {
            string emailStrs = sms.Email;
            emailStrs = emailStrs.Replace(",", ";");
            emailStrs = emailStrs.Replace("，", ";");
            //包含多个邮箱
            if (emailStrs.Contains(";") == true)
            {
                string[] emails = emailStrs.Split(';');
                foreach (string email in emails)
                {
                    if (DataType.IsNullOrEmpty(email) == true)
                        continue;
                    SMS.SendEmailNowAsync(email, sms.Title, sms.DocOfEmail);
                }
            }
            else
            {   //单个邮箱
                SMS.SendEmailNowAsync(sms.Email, sms.Title, sms.DocOfEmail);
            }
        }
        public static void SendToDingDing(SMS sms)
        {
            int fk_node = 0;
            Int64 workid = 0;
            //获取关键数据，FK_Node，WorkID
            string msgFlg = sms.MsgFlag.Replace("WKAlt", "");

            AtPara para = new AtPara(sms.AtPara);
            int nodeID = para.GetValIntByKey("NodeID");
            Int64 workID = para.GetValInt64ByKey("WorkID");

            //获取应用ID，企业应用必须存在
            string dId = BP.Difference.SystemConfig.Ding_AgentID ?? null;
            //接收钉消息发送后返回值的类
            Ding_Post_ReturnVal postVal = null;

            //获取权限，token
            string access_token = BP.Port.DINGTalk.DingDing.getAccessToken();

            //根据手机号获取成员的userid
            string url = "https://oapi.dingtalk.com/user/get_by_mobile?access_token=" + access_token + "&mobile=" + sms.Mobile;
            string strJson = new HttpWebResponseUtility().HttpResponseGet(url);
            //Log.DefaultLogWriteLineError("获取接收人的信息："+strJson);
            CreateUser_PostVal user = new CreateUser_PostVal();
            user = FormatToJson.ParseFromJson<CreateUser_PostVal>(strJson);

            if (dId != null)
            {
                //消息模版，touser必须是userid
                Ding_Msg_OA msgOA = new Ding_Msg_OA();
                msgOA.Access_Token = BP.Port.DINGTalk.DingDing.getAccessToken();
                msgOA.agentid = BP.Difference.SystemConfig.Ding_AgentID;
                msgOA.touser = user.userid;
                //打开消息时，跳转的路径
                msgOA.messageUrl = BP.Difference.SystemConfig.Ding_MessageUrl + "/CCMobile/DingTalk.aspx?ActionType=ToDo&FK_Flow=" + para.GetValStrByKey("FK_Flow") + "&WorkID=" + workid + "&FK_Node=" + fk_node + "&m=" + DateTime.Now.ToString("yyyyMMddHHmmss");
                //00是完全透明，ff是完全不透明，比较适中的透明度值是 1e
                msgOA.head_bgcolor = "FFBBBBBB";
                msgOA.head_text = "审批";
                msgOA.body_title = sms.Title;
                //消息主体，可自定义
                Hashtable hs = new Hashtable();
                hs.Add("审批内容", "您有一条待办工作需审核。");
                msgOA.body_form = hs;
                msgOA.body_author = WebUser.No;
                //执行消息发送
                postVal = DingTalk_Message.Msg_OAText_Send(msgOA);
            }
            return;
        }
        public static void SendToWeiXin(SMS sms)
        {
            AtPara ap = new AtPara(sms.AtPara);
            //微信企业号ID
            string agentId = BP.Difference.SystemConfig.WX_AgentID ?? null;
            if (agentId != null)
            {
                //申请权限，获取token
                string accessToken = BP.Port.WeiXin.WeiXinEntity.getAccessToken();//获取 AccessToken

                //组织消息模版
                NewsArticles newArticle = new NewsArticles();
                newArticle.title = "您有一条待办消息";
                string New_Url = "";
                if (sms.MobileInfo.StartsWith("http:") == true)
                {
                    byte[] bytes = UTF8Encoding.UTF8.GetBytes(sms.MobileInfo);
                    sms.MobileInfo = Convert.ToBase64String(bytes);

                    //点击消息时，打开的连接
                    New_Url = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + BP.Difference.SystemConfig.WX_CorpID
                     + "&redirect_uri=" + BP.Difference.SystemConfig.WX_MessageUrl + "/CCMobile/action.aspx&response_type=code&scope=snsapi_base&state=URL_" + sms.MobileInfo + ",WorkID_" + ap.GetValInt64ByKey("WorkID") + ",FK_Node_" + ap.GetValInt64ByKey("NodeID") + "#wechat_redirect";
                    GenerWorkFlow gwf = new GenerWorkFlow(ap.GetValInt64ByKey("WorkID"));
                    //消息显示格式
                    string str = "\t\n您好:";
                    str += "\t\n    工作{" + gwf.Title + "}有一条新消息 .";
                    str += "\t\n    发起人" + gwf.StarterName;
                    str += "\t\n    发起时间" + gwf.SendDT;
                    newArticle.description = str;
                }
                else
                {
                    newArticle.description = sms.MobileInfo;
                    New_Url = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + BP.Difference.SystemConfig.WX_CorpID
                    + "&redirect_uri=" + BP.Difference.SystemConfig.WX_MessageUrl + "/CCMobile/action.aspx&response_type=code&scope=snsapi_base&state=MyView,WorkID_" + ap.GetValInt64ByKey("WorkID") + ",FK_Node_" + ap.GetValInt64ByKey("NodeID") + "#wechat_redirect";
                }

                newArticle.url = New_Url;
                //消息模块中显示的图片
                //http://discuz.comli.com/weixin/weather/icon/cartoon.jpg
                newArticle.picurl = BP.Difference.SystemConfig.WX_MessageUrl + "/DataUser/ICON/ccicon.png";

                //新消息类模版
                MsgNews wxMsg = new MsgNews();
                wxMsg.Access_Token = accessToken;
                wxMsg.agentid = BP.Difference.SystemConfig.WX_AgentID;
                wxMsg.touser = sms.Mobile;
                wxMsg.articles.Add(newArticle);
                //执行发送
                BP.Port.WeiXin.Glo.PostMsgOfNews(wxMsg);
            }
            return;
        }
        public static void MessageIsEnableSelf(SMS sms)
        {
            //获得流程信息.
            //AtPara ap = new AtPara(sms.AtPara);
            //int nodeID = ap.GetValIntByKey("NodeID");
            //Int64 workID = ap.GetValIntByKey("WorkID");
            //string flowNo = ap.GetValStrByKey("FlowNo");

            //    json += " \"sender\": \"" + WebUser.No + "\","; //发送人.
            //    json += " \"sendTo\": \"" + this.SendToEmpNo + "\","; //要发送给谁.
            //    json += " \"tel\": \"" + this.Mobile + "\","; //手机号.
            //    json += " \"title\":\"" + this.Title + "\","; //标题.
            //    json += " \"msgFlag\":\"" + this.MsgFlag.Replace("WKAlt", "") + "\","; //标记
            //    json += " \"content\":\"" + this.MobileInfo + " \","; //短消息.
            //    json += " \"openUrl\":\"" + this.OpenURL + " \"}"; //打开的链接.
            return;
        }
        #endregion 消息机制个性化处理.

        /// <summary>
        /// 流程事件-总体拦截器.
        /// </summary>
        /// <param name="eventMark">流程标记</param>
        /// <param name="wn">worknode</param>
        /// <param name="paras">参数</param>
        /// <param name="checkNote">审核意见.</param>
        /// <returns>执行信息.</returns>
        public static string DoIt(string eventMark, WorkNode wn, string paras, string checkNote, int returnToNodeID, string returnToEmps, string returnMsg)
        {
            //发送成功.
            if (eventMark.Equals(EventListNode.SendSuccess) == true)
                return SendSuccess(wn);

            //退回后事件.
            if (eventMark.Equals(EventListNode.ReturnAfter) == true)
                return ReturnAfter(wn, returnToNodeID, returnToEmps, returnMsg);

            // 流程结束事件.
            if (eventMark.Equals(EventListFlow.FlowOverAfter) == true)
                return FlowOverAfter(wn);

            return null;
        }
        /// <summary>
        /// 执行发送.
        /// </summary>
        /// <param name="wn">工作节点</param>
        /// <returns></returns>
        public static string SendSuccess(WorkNode wn)
        {
            if (SystemConfig.CustomerNo.Equals("TianYu") == true)
            {
                if (wn.HisNode.ItIsStartNode == false)
                    return null; //如果不是开始节点发送,就不处理.

                //模板目录.
                string sortNo = wn.HisFlow.FlowSortNo;

                //找到系统编号.
                FlowSort fs = new FlowSort(sortNo);

                //子系统:当前目录的上一级目录必定是子系统,系统约定的.
                SubSystem system = new SubSystem(fs.ParentNo);

                //检查是否配置了?
                if (DataType.IsNullOrEmpty(system.TokenPiv) == true)
                    return null;

                //执行事件.
                DoPost("SendSuccess", wn, system);
            }
            return null;
        }
        /// <summary>
        /// 执行退回操作.
        /// </summary>
        /// <param name="wn"></param>
        /// <param name="toNodeID"></param>
        /// <param name="toEmp"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string ReturnAfter(WorkNode wn, int toNodeID, string toEmp, string info)
        {
            if (SystemConfig.CustomerNo.Equals("TianYu") == true)
            {
                if (toNodeID.ToString().EndsWith("01") == false)
                    return null; //如果不是退回的开始节点.


                //模板目录.
                string sortNo = wn.HisFlow.FlowSortNo;

                //找到系统编号.
                FlowSort fs = new FlowSort(sortNo);

                //子系统:当前目录的上一级目录必定是子系统,系统约定的.
                SubSystem system = new SubSystem(fs.ParentNo);

                //检查是否配置了?
                if (DataType.IsNullOrEmpty(system.TokenPiv) == true)
                    return null;

                //执行事件.
                DoPost("ReturnAfter", wn, system);
            }
            return null;
        }
        /// <summary>
        /// 流程结束事件
        /// </summary>
        /// <param name="wn"></param>
        /// <returns></returns>
        public static string FlowOverAfter(WorkNode wn)
        {
            if (SystemConfig.CustomerNo.Equals("TianYu") == true)
            {
                //模板目录.
                string sortNo = wn.HisFlow.FlowSortNo;

                //找到系统编号.
                FlowSort fs = new FlowSort(sortNo);

                //子系统:当前目录的上一级目录必定是子系统,系统约定的.
                SubSystem system = new SubSystem(fs.ParentNo);

                //检查是否配置了?
                if (DataType.IsNullOrEmpty(system.TokenPiv) == true)
                    return null;

                //执行事件.
                DoPost("FlowOverAfter", wn, system);
            }
            return null;
        }
        /// <summary>
        /// 执行天宇的回调.
        /// </summary>
        /// <param name="eventMark">事件目录.</param>
        /// <param name="wn"></param>
        /// <param name="system"></param>
        /// <returns></returns>
        public static string DoPost(string eventMark, WorkNode wn, SubSystem system)
        {
            string myEventMark = "0";
            if (eventMark.Equals("ReturnAfter"))
                myEventMark = "3";
            if (eventMark.Equals("SendSuccess"))
                myEventMark = "1";
            if (eventMark.Equals("FlowOverAfter"))
                myEventMark = "2";

            string apiParas = system.ApiParas; //配置的json字符串.
            apiParas = apiParas.Replace("~", "\"");

            apiParas = apiParas.Replace("@WorkID", wn.WorkID.ToString()); //工作ID.
            apiParas = apiParas.Replace("@FlowNo", wn.HisFlow.No); //流程编号.
            apiParas = apiParas.Replace("@NodeID", wn.HisNode.NodeID.ToString()); //节点ID.
            apiParas = apiParas.Replace("@TimeSpan", DBAccess.GenerOID("TS").ToString()); //时间戳.
            apiParas = apiParas.Replace("@EventMark", myEventMark); //稳超定义的，事件标记.
            apiParas = apiParas.Replace("@EventID", eventMark); //EventID 定义的事件类型.
            apiParas = apiParas.Replace("@SPYJ", "xxx无xx"); //审批意见.

            //如果表单的数据有,就执行一次替换.
            apiParas = Glo.DealExp(apiParas, wn.rptGe);

            //需要补充算法. @WenChao.
            string sign = "密钥:" + system.TokenPiv + ",公约" + system.TokenPublie;
            apiParas = apiParas.Replace("@sign", sign); //签名，用于安全验证.

            if (apiParas.Contains("@") == true)
                return "err@配置的参数没有替换下来:" + apiParas;

            //替换url参数.
            string url = system.CallBack; //回调的全路径.
            url = Glo.DealExp(url, wn.rptGe);

            //执行post.
            string data = BP.Tools.PubGlo.HttpPostConnect(url, apiParas, system.RequestMethod, system.ItIsJson);
            return data;
        }
    }
}
