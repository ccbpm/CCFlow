using System;
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
        /// <param name="doType">执行事件类型</param>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string DoFrm(string doType, Entity en)
        {

            return "";
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

            //写入消息之前，删除所有的消息.
            if (SystemConfig.IsEnableAthEncrypt)
            BP.DA.DBAccess.RunSQL("DELETE FROM Sys_SMS WHERE WorkID="+wn.HisWork.OID );

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
            PushMsgs pms = wn.HisFlow.HisPushMsgs;
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
