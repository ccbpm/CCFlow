using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BP.Sys;
using System.Collections;
using BP.En;

namespace BP.EAI.Plugins.DINGTalk
{
    public class DingFlowMessage
    {
        public Ding_Post_ReturnVal Ding_SendWorkMessage(DingMsgType msgType, long WorkID, string sender)
        {
            //主业务表
            GenerWorkFlow workFlow = new GenerWorkFlow(WorkID);
            //结束不发送消息
            if (workFlow.WFState == WFState.Complete)
                return null;
            //判断节点类型，分合流等
            Node node = new Node(workFlow.FK_Node);

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

            switch (msgType)
            {
                case DingMsgType.text:
                    Ding_Msg_Text msgText = new Ding_Msg_Text();
                    msgText.Access_Token = DingDing.getAccessToken();
                    msgText.agentid = SystemConfig.Ding_AgentID;
                    msgText.touser = toUsers;
                    msgText.content = workFlow.Title + "\n发送人：" + sender + "\n时间：" + BP.DA.DataType.CurrentDataTimeCNOfShort;
                    return DingTalk_Message.Msg_AgentText_Send(msgText);
                case DingMsgType.link:
                    Ding_Msg_Link msgLink = new Ding_Msg_Link();
                    msgLink.Access_Token = DingDing.getAccessToken();
                    msgLink.touser = toUsers;
                    msgLink.agentid = SystemConfig.Ding_AgentID;
                    msgLink.messageUrl = SystemConfig.Ding_MessageUrl + "/CCMobile/login.aspx";
                    msgLink.picUrl = "@lALOACZwe2Rk";
                    msgLink.title = workFlow.Title;
                    msgLink.text = "发送人：" + sender + "\n时间：" + BP.DA.DataType.CurrentDataTimeCNOfShort;
                    return DingTalk_Message.Msg_AgentLink_Send(msgLink);
                case DingMsgType.OA:
                    string[] users = toUsers.Split('|');
                    string faildSend = "";
                    Ding_Post_ReturnVal postVal = null;
                    foreach (string user in users)
                    {
                        Ding_Msg_OA msgOA = new Ding_Msg_OA();
                        msgOA.Access_Token = DingDing.getAccessToken();
                        msgOA.agentid = SystemConfig.Ding_AgentID;
                        msgOA.touser = user;
                        msgOA.messageUrl = SystemConfig.Ding_MessageUrl + "/CCMobile/DingAction.aspx?ActionFrom=message&UserID=" + user
                            + "&ActionType=ToDo&FK_Flow=" + workFlow.FK_Flow + "&FK_Node=" + workFlow.FK_Node
                            + "&WorkID=" + workFlow.WorkID + "&FID=" + workFlow.FID;
                        //00是完全透明，ff是完全不透明，比较适中的透明度值是 1e
                        msgOA.head_bgcolor = "FFBBBBBB";
                        msgOA.head_text = "审批";
                        msgOA.body_title = workFlow.Title;
                        Hashtable hs = new Hashtable();
                        hs.Add("流程名", workFlow.FlowName);
                        hs.Add("当前节点", workFlow.NodeName);
                        hs.Add("申请人", workFlow.StarterName);
                        hs.Add("申请时间", workFlow.RDT);
                        msgOA.body_form = hs;
                        msgOA.body_author = sender;
                        postVal = DingTalk_Message.Msg_OAText_Send(msgOA);
                        if (postVal.errcode != "0")
                        {
                            if (faildSend.Length > 0)
                                faildSend += ",";
                            faildSend += user;
                        }
                    }
                    //有失败消息
                    if (faildSend.Length > 0)
                    {
                        postVal.errcode = "500";
                        postVal.errmsg = faildSend + "消息发送失败";
                    }
                    return postVal;
            }
            return null;
        }
    }
}
