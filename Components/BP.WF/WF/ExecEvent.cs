using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using BP.Sys;
using BP.DA;
using BP.En;
using BP.Web;
using BP.WF.Template;
using BP.Port;

namespace BP.WF
{
    /// <summary>
    /// 执行事件
    /// </summary>
    public class ExecEvent
    {
        /// <summary>
        /// 执行表单
        /// </summary>
        /// <param name="md"></param>
        /// <param name="eventType"></param>
        /// <param name="en"></param>
        /// <param name="atParas"></param>
        /// <returns></returns>
        public static string DoFrm(MapData md, string eventType, Entity en,string atParas = null)
        {
            #region 首先执行通用的事件重载方法.
            if (EventListFrm.FrmLoadBefore.Equals(eventType) == true)
                BP.En.OverrideFile.FrmEvent_LoadBefore(md.No, en);

            //装载之后.
            if (EventListFrm.FrmLoadAfter.Equals(eventType) == true)
                BP.En.OverrideFile.FrmEvent_FrmLoadAfter(md.No, en);

            ///保存之前.
            if (EventListFrm.SaveBefore.Equals(eventType) == true)
                BP.En.OverrideFile.FrmEvent_SaveBefore(md.No, en);

            //保存之后.
            if (EventListFrm.SaveAfter.Equals(eventType) == true)
                BP.En.OverrideFile.FrmEvent_SaveAfter(md.No, en);
            #endregion 首先执行通用的事件重载方法.

            string str = md.FrmEvents.DoEventNode(eventType, en);

            string mystrs = null;
            if (md.HisFEB != null)
                mystrs = md.HisFEB.DoIt(eventType, en, atParas);

            if (str == null)
                return mystrs;

            if (mystrs == null)
                return str;

            return str + "@" + mystrs;
        }
        /// <summary>
        /// 执行节点事件
        /// </summary>
        /// <param name="doType"></param>
        /// <param name="nd"></param>
        /// <param name="wk"></param>
        /// <param name="objs"></param>
        /// <param name="atParas"></param>
        /// <returns></returns>
        public static string DoNode(string doType, Node nd, Work wk, SendReturnObjs objs = null, string atParas = null)
        {
            WorkNode wn = new WorkNode(wk, nd);
            return DoNode(doType, wn, objs, atParas);
        }
        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="eventFlag"></param>
        /// <param name="wn"></param>
        public static string DoNode(string doType, WorkNode wn, SendReturnObjs objs = null,
        string atPara = null)
        {
            if (wn.HisNode == null)
                return null;
            if (objs == null)
                objs = wn.HisMsgObjs;
             
            //如果执行了节点发送成功时间. 
            if (doType.Equals(EventListNode.SendSuccess) == true)
                WorkNodePlus.SendDraftSubFlow(wn); //执行自动发送子流程草稿.

            #region 2021.5.30 gaoxin. 更新授权岗位:为中科软
            //更新授权岗位:为中科软. 如果是当前节点的处理人员是按照岗位绑定的，就需要吧授权岗，写入到 Emps里面去.
            if (doType.Equals(EventListNode.SendSuccess) == true && SystemConfig.GetValByKeyBoolen("IsEnableAuthDeptStation",false)==true)
            {
                // 如果这些计算人员的方式有岗位的因素，就需要把当前人员授权岗增加上去.
                if (wn.HisNode.HisDeliveryWay== DeliveryWay.ByStation
                    || wn.HisNode.HisDeliveryWay == DeliveryWay.ByStationOnly
                    || wn.HisNode.HisDeliveryWay == DeliveryWay.ByStationAndEmpDept
                    || wn.HisNode.HisDeliveryWay== DeliveryWay.ByDeptAndStation)
                {
                    string sql = "SELECT A.FK_Dept, A.FK_Station FROM Port_DeptEmpStation A, WF_NodeStation B ";
                    sql += " WHERE A.FK_Emp='"+WebUser.No+"' AND A.FK_Station=B.FK_Station AND B.FK_Node="+wn.HisNode.NodeID;
                    DataTable dt = DBAccess.RunSQLReturnTable(sql);

                    foreach (DataRow dr in dt.Rows)
                    {
                       string strs = "@"+dr[0].ToString() + "_" + dr[1].ToString()+"@";
                        if (wn.HisGenerWorkFlow.Emps.Contains("@"+strs) == true)
                            continue;

                        wn.HisGenerWorkFlow.Emps += strs;
                    }
                }
            }
            #endregion 2021.5.30. 

            //写入消息之前，删除所有的消息.
            if (BP.WF.Glo.IsEnableSysMessage == true)
            {
                try
                {
                    switch (doType)
                    {
                        case EventListNode.SendWhen:
                            if (wn.HisNode.TodolistModel == TodolistModel.QiangBan)
                                DBAccess.RunSQL("DELETE FROM Sys_SMS WHERE WorkID=" + wn.HisWork.OID);
                            else
                                DBAccess.RunSQL("DELETE FROM Sys_SMS WHERE SendTo='"+WebUser.No+"' AND WorkID=" + wn.HisWork.OID);
                            break;
                        case EventListNode.ReturnAfter:
                        case EventListNode.ShitAfter:
                            DBAccess.RunSQL("DELETE FROM Sys_SMS WHERE WorkID=" + wn.HisWork.OID);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    SMS sms = new SMS();
                    sms.CheckPhysicsTable();
                }
            }

            int toNodeID = 0;
            if (wn.JumpToNode != null)
                toNodeID = wn.JumpToNode.NodeID;

            string msg = null; //定义执行的消息.
            if (wn.HisFlow.FEventEntity == null)
            {
            }

            if (wn.HisFlow.FEventEntity != null)
            {
                wn.HisFlow.FEventEntity.SendReturnObjs = objs;
                msg = wn.HisFlow.FEventEntity.DoIt(doType, wn.HisNode, wn.HisWork, atPara, toNodeID, wn.JumpToEmp);
            }

            #region 执行事件.
            FrmEvents fes = wn.HisNode.FrmEvents; //获得当前的事件.
            if (fes.Count != 0)
            {
                // 2019-08-27 取消节点事件 zl
                string msg1 = fes.DoEventNode(doType, wn.HisWork, atPara);
                if (msg != null && msg1 != null)
                    msg += msg1;

                if (msg == null)
                    msg = msg1;
            }
            #endregion 执行事件.

            #region 处理消息推送
            //有一些事件没有消息，直接 return ;
            switch (doType)
            {
                case EventListNode.WorkArrive:
                case EventListNode.SendSuccess:
                case EventListNode.ShitAfter:
                case EventListNode.ReturnAfter:
                case EventListNode.UndoneAfter:
                case EventListNode.AskerReAfter:
                case EventListFlow.FlowOverAfter: //流程结束后.
                    break;
                default:
                    return msg;
            }

            //执行消息的发送.
            PushMsgs pms = wn.HisNode.HisPushMsgs;
            if (pms.Count == 0)
                return msg; //如果没有设置消息.

            //撤销之后.
            if (doType.Equals(EventListNode.UndoneAfter) == true)
            {
                AtPara ap = new AtPara(atPara);
                if (toNodeID == 0)
                    toNodeID = ap.GetValIntByKey("ToNode");
                if (toNodeID == 0)
                    return msg;

                Node toNode = new Node(toNodeID);
                pms = toNode.HisPushMsgs;
            }

            string msgAlert = ""; //生成的提示信息.
            foreach (PushMsg item in pms)
            {
                if (item.FK_Event != doType)
                    continue;

                if (item.SMSPushWay == 0)
                    continue; /* 如果都没有消息设置，就放过.*/

                //执行发送消息.
                msgAlert += item.DoSendMessage(wn.HisNode, wn.HisWork, atPara, objs);
            }
            return msg + msgAlert;
            #endregion 处理消息推送.
        }
        public static string DoFlow(string doType, Work wk, Node nd, string atPara)
        {
            WorkNode wn = new WorkNode(wk, nd);
            return DoFlow(doType, wn, atPara);
        }
        /// <summary>
        /// 执行流程事件
        /// </summary>
        /// <param name="doType"></param>
        /// <param name="wn"></param>
        /// <param name="atPara"></param>
        /// <returns></returns>
        public static string DoFlow(string doType, WorkNode wn, string atPara)
        {
            if (wn.HisNode == null)
                return null;

            int toNodeID = 0;
            if (wn.JumpToNode != null)
                toNodeID = wn.JumpToNode.NodeID;

            //执行的参数.
            BP.WF.CCBill_FlowEvent.DoFlow(doType, wn, atPara);

            #region 写入消息之前,删除消息,不让其在提醒.
            if (BP.WF.Glo.IsEnableSysMessage == true)
            {
                switch (doType)
                {
                    case EventListFlow.FlowOverAfter:
                        DBAccess.RunSQL("DELETE FROM Sys_SMS WHERE (MsgType='"+ EventListNode.SendSuccess + "' OR MsgType='" + EventListNode.ReturnAfter + "'  ) AND WorkID=" + wn.HisWork.OID);
                        break;
                    case EventListFlow.AfterFlowDel: //删除所有的消息，包括抄送.
                        DBAccess.RunSQL("DELETE FROM Sys_SMS WHERE AtPara LIKE '%="+wn.HisWork.OID+"@' OR WorkID=" + wn.HisWork.OID);
                        break;
                    default:
                        break;
                }
            }
            #endregion 写入消息之前,删除消息,不让其在提醒.

            string msg = null;
            if (wn.HisFlow.FEventEntity == null)
            {
            }
            if (wn.HisFlow.FEventEntity != null)
            {
                wn.HisFlow.FEventEntity.SendReturnObjs = wn.HisMsgObjs;
                msg = wn.HisFlow.FEventEntity.DoIt(doType, wn.HisNode, wn.HisWork, null, toNodeID, wn.JumpToEmp);
            }

            #region 执行事件.
            FrmEvents fes = wn.HisFlow.FrmEvents; //获得当前的事件.
            if (fes.Count != 0)
            {
                string msg1 = fes.DoEventNode(doType, wn.HisWork, atPara);
                if (msg1 != null && msg != null)
                    msg += msg1;

                if (msg == null)
                    msg = msg1;
            }
            #endregion 执行事件.

            #region 处理消息推送
            //有一些事件没有消息，直接 return ;
            switch (doType)
            {
                case EventListFlow.BeforeFlowDel:
                case EventListFlow.FlowOnCreateWorkID:
                case EventListFlow.FlowOverBefore:
                    break;
                default:
                    return msg;
            }

            //执行消息的发送.
            PushMsgs pms = wn.HisFlow.PushMsgs;
            if (pms.Count == 0)
                return msg;

           

            string msgAlert = ""; //生成的提示信息.
            foreach (PushMsg item in pms)
            {
                if (item.FK_Event != doType)
                    continue;

                if (item.SMSPushWay == 0)
                    continue; /* 如果都没有消息设置，就放过.*/

                //执行发送消息.
                msgAlert += item.DoSendMessage(wn.HisNode, wn.HisWork, atPara, wn.HisMsgObjs);
            }
            return msg + msgAlert;
            #endregion 处理消息推送.
        }
    }
}
