using System;
using System.Collections.Generic;
using System.Data;
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


            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable("SELECT * FROM WF_GenerWorkFlow WHERE WorkID=" + WorkID);
            if (dt.Rows.Count == 0)
                return null;

            int wfState = int.Parse(dt.Rows[0]["WFState"].ToString());
            string title = dt.Rows[0]["Title"].ToString();
            string flowNo = dt.Rows[0]["FK_Flow"].ToString();
            string nodeID = dt.Rows[0]["FK_Node"].ToString();
            string fid = dt.Rows[0]["FID"].ToString();
            string flowName = dt.Rows[0]["FlowName"].ToString();
            string nodeName = dt.Rows[0]["NodeName"].ToString();
            string starterName = dt.Rows[0]["StarterName"].ToString();
            string rdt = dt.Rows[0]["RDT"].ToString();


            //结束不发送消息
            if (wfState==3 )
                return null;



            //判断节点类型，分合流等.
            dt = BP.DA.DBAccess.RunSQLReturnTable("SELECT * FROM WF_EmpWorks WHERE WorkID=" + WorkID+" OR FID="+WorkID);
            if (dt.Rows.Count == 0)
                return null;

            string toUsers = "";
            foreach (DataRow dr in dt.Rows)
            {
                if (toUsers.Length > 0)
                    toUsers += "|";
                toUsers += dr["FK_Emp"].ToString();
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
                    msgText.content = title + "\n发送人：" + sender + "\n时间：" + BP.DA.DataType.CurrentDataTimeCNOfShort;
                    return DingTalk_Message.Msg_AgentText_Send(msgText);
                case DingMsgType.link:
                    Ding_Msg_Link msgLink = new Ding_Msg_Link();
                    msgLink.Access_Token = DingDing.getAccessToken();
                    msgLink.touser = toUsers;
                    msgLink.agentid = SystemConfig.Ding_AgentID;
                    msgLink.messageUrl = SystemConfig.Ding_MessageUrl + "/CCMobile/login.aspx";
                    msgLink.picUrl = "@lALOACZwe2Rk";
                    msgLink.title = title;
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
                            + "&ActionType=ToDo&FK_Flow=" + flowNo + "&FK_Node=" + nodeID
                            + "&WorkID=" + WorkID+ "&FID=" + fid;
                        //00是完全透明，ff是完全不透明，比较适中的透明度值是 1e
                        msgOA.head_bgcolor = "FFBBBBBB";
                        msgOA.head_text = "审批";
                        msgOA.body_title = title;
                        Hashtable hs = new Hashtable();
                        hs.Add("流程名",flowName);
                        hs.Add("当前节点", nodeName);
                        hs.Add("申请人", starterName);
                        hs.Add("申请时间",rdt);
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
