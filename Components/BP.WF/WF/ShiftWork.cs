using System;
using System.Collections.Generic;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.DA;
using BP.WF.Template;

namespace BP.WF
{
    public class ShiftWork
    {
        /// <summary>
        /// 工作移交
        /// </summary>
        /// <param name="workID">工作ID</param>
        /// <param name="toEmp">要移交的人</param>
        /// <param name="msg">移交信息</param>
        /// <returns>执行结果</returns>
        public static string Node_Shift_ToEmp(Int64 workID, string toEmp, string msg)
        {
            if (toEmp.Equals(WebUser.No) == true)
                throw new Exception("err@您不能移交给您自己。");

            GenerWorkFlow gwf = new GenerWorkFlow(workID);
            if (gwf.WFSta == WFSta.Complete)
                throw new Exception("err@流程已经完成，您不能执行移交了。");

            int i = 0;
            //人员.
            Emp emp = new Emp(toEmp);
            Node nd = new Node(gwf.FK_Node);
            Work work = nd.HisWork;
            work.OID = workID;
            if (nd.TodolistModel == TodolistModel.Order
                || nd.TodolistModel == TodolistModel.Teamup
                || nd.TodolistModel == TodolistModel.TeamupGroupLeader)
            {
                /*如果是队列模式，或者是协作模式, 就直接把自己的gwl 信息更新到被移交人身上. */

                //检查被移交人是否在当前的待办列表里否？
                GenerWorkerList gwl = new GenerWorkerList();
                i = gwl.Retrieve(GenerWorkerListAttr.FK_Emp, emp.UserID,
                    GenerWorkerListAttr.FK_Node, nd.NodeID,
                    GenerWorkerListAttr.WorkID, workID);
                if (i == 1)
                    return "err@移交失败，您所移交的人员(" + emp.UserID + " " + emp.Name + ")已经在代办列表里.";

                //把自己的待办更新到被移交人身上.
                string sql = "UPDATE WF_GenerWorkerlist SET IsRead=0, FK_Emp='" + emp.UserID + "', FK_EmpText='" + emp.Name + "' WHERE FK_Emp='" + WebUser.No + "' AND FK_Node=" + gwf.FK_Node + " AND WorkID=" + workID;
                int myNum = DBAccess.RunSQL(sql);

                #region 判断是否是,admin的移交.
                if (myNum == 0)
                {
                    //说明移交人是 admin，执行的.
                    GenerWorkerLists mygwls = new GenerWorkerLists();
                    mygwls.Retrieve(GenerWorkerListAttr.WorkID, workID,
                        GenerWorkerListAttr.FK_Node, gwf.FK_Node);
                    if (mygwls.Count == 0)
                        throw new Exception("err@系统错误，没有找到待办.");

                    //把他们都删除掉.
                    mygwls.Delete(GenerWorkerListAttr.WorkID, workID,
                        GenerWorkerListAttr.FK_Node, gwf.FK_Node);

                    //取出来第1个，把人员信息改变掉.
                    foreach (GenerWorkerList item in mygwls)
                    {
                        item.FK_Emp = WebUser.No;
                        item.FK_EmpText = WebUser.Name;

                        item.FK_Dept = WebUser.FK_Dept;
                        item.DeptName = WebUser.FK_DeptName;

                        item.IsRead = false;

                        item.Insert(); //执行插入.
                        break;
                    }
                }
                #endregion 判断是否是,admin的移交.

                //记录日志.
                Glo.AddToTrack(ActionType.Shift, nd.FK_Flow, workID, gwf.FID, nd.NodeID, nd.Name,
                    WebUser.No, WebUser.Name, nd.NodeID, nd.Name, toEmp, emp.Name, msg, null);

                //移交后事件
                string atPara1 = "@SendToEmpIDs=" + emp.UserID;
                string info = "@" + ExecEvent.DoNode(EventListNode.ShitAfter, nd, work, null, atPara1);

                //处理移交后发送的消息事件,发送消息.
                PushMsgs pms1 = new PushMsgs();
                pms1.Retrieve(PushMsgAttr.FK_Node, nd.NodeID, PushMsgAttr.FK_Event, EventListNode.ShitAfter);
                foreach (PushMsg pm in pms1)
                    pm.DoSendMessage(nd, nd.HisWork, null, null, null, emp.UserID);

                gwf.WFState = WFState.Shift;
                gwf.TodoEmpsNum = 1;
                gwf.TodoEmps = WebUser.No + "," + WebUser.Name + ";";
                gwf.Update();
                return "移交成功." + info;
            }

            //非协作模式.
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.FK_Node, gwf.FK_Node, GenerWorkerListAttr.WorkID, gwf.WorkID);
            gwls.Delete(GenerWorkerListAttr.FK_Node, gwf.FK_Node, GenerWorkerListAttr.WorkID, gwf.WorkID);

            foreach (GenerWorkerList item in gwls)
            {
                item.FK_Emp = emp.UserID;
                item.FK_EmpText = emp.Name;
                item.IsEnable = true;
                item.Insert();
                break;
            }

            gwf.WFState = WFState.Shift;
            gwf.TodoEmpsNum = 1;
            gwf.TodoEmps = emp.UserID + "," + emp.Name + ";";
            gwf.Update();

            //记录日志.
            Glo.AddToTrack(ActionType.Shift, nd.FK_Flow, workID, gwf.FID, nd.NodeID, nd.Name,
                WebUser.No, WebUser.Name, nd.NodeID, nd.Name, toEmp, emp.Name, msg, null);

            string inf1o = "@工作移交成功。@您已经成功的把工作移交给：" + emp.UserID + " , " + emp.Name;
            //移交后事件
            string atPara = "@SendToEmpIDs=" + emp.UserID;
            WorkNode wn = new WorkNode(work, nd);
            inf1o += "@" + ExecEvent.DoNode(EventListNode.ShitAfter, wn, null, atPara);
            return inf1o;
        }
        /// <summary>
        /// 工作移交
        /// </summary>
        /// <param name="workID">工作ID</param>
        /// <param name="toEmps">要移交的多个人,比如:zhangsan,lisi</param>
        /// <param name="msg"></param>
        /// <returns>执行信息.err@说明执行错误.</returns>
        public static string Node_Shift_ToEmps(Int64 workID, string toEmps, string msg)
        {
            if (toEmps.Equals(WebUser.No) == true)
                throw new Exception("err@您不能移交给您自己。");

            GenerWorkFlow gwf = new GenerWorkFlow(workID);
            if (gwf.WFSta == WFSta.Complete)
                throw new Exception("err@流程已经完成，您不能执行移交了。");

            //定义变量,查询出来当前的人员列表.
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.FK_Node, gwf.FK_Node,
                        GenerWorkerListAttr.WorkID, workID);
            //定义变量.
            GenerWorkerList gwl = null;

            int i = 0;
            //人员.
            Node nd = new Node(gwf.FK_Node);
            Work work = nd.HisWork;
            work.OID = workID;

            string info = null;
            string[] strs = toEmps.Split(',');
            string empNames = "";
            foreach (string toEmp in strs)
            {
                Emp emp = new Emp(toEmp);
                if (nd.TodolistModel == TodolistModel.Order
                    || nd.TodolistModel == TodolistModel.Teamup
                    || nd.TodolistModel == TodolistModel.TeamupGroupLeader)
                {
                    /*如果是队列模式，或者是协作模式, 就直接把自己的gwl 信息更新到被移交人身上. */

                    //检查被移交人是否在当前的待办列表里否？
                    i = gwl.Retrieve(GenerWorkerListAttr.FK_Emp, emp.UserID,
                        GenerWorkerListAttr.FK_Node, nd.NodeID,
                        GenerWorkerListAttr.WorkID, workID);
                    if (i == 1)
                    {
                        info += "err@移交失败，您所移交的人员(" + emp.UserID + " " + emp.Name + ")已经在代办列表里.";
                        continue;
                    }

                    if (i == 0)
                        return "";

                    //写入移交数据.
                    gwl = (GenerWorkerList)gwls[0];
                    gwl.FK_Emp = emp.UserID;
                    gwl.FK_EmpText = emp.Name;
                    gwl.IsPassInt = 0;
                    gwl.Insert();
                     
                    //记录日志.
                    Glo.AddToTrack(ActionType.Shift, nd.FK_Flow, workID, gwf.FID, nd.NodeID, nd.Name,
                        WebUser.No, WebUser.Name, nd.NodeID, nd.Name, toEmp, emp.Name, msg, null);

                    //移交后事件
                    string atPara1 = "@SendToEmpIDs=" + emp.UserID;
                    info += "@" + ExecEvent.DoNode(EventListNode.ShitAfter, nd, work, null, atPara1);

                    //处理移交后发送的消息事件,发送消息.
                    PushMsgs pms1 = new PushMsgs();
                    pms1.Retrieve(PushMsgAttr.FK_Node, nd.NodeID, PushMsgAttr.FK_Event, EventListNode.ShitAfter);
                    foreach (PushMsg pm in pms1)
                        pm.DoSendMessage(nd, nd.HisWork, null, null, null, emp.UserID);
                
                    info += "info@成功移交给:" + emp.UserID + "," + emp.Name;
                    continue;
                }

                //非协作模式.
                //写入移交数据.
                gwl = (GenerWorkerList)gwls[0];
                gwl.FK_Emp = emp.UserID;
                gwl.FK_EmpText = emp.Name;
                gwl.IsPassInt = 0;
                gwl.Insert();
            }

            //重新查询.
            gwls.Retrieve(GenerWorkerListAttr.FK_Node, gwf.FK_Node,
                      GenerWorkerListAttr.WorkID, workID);

            //工作处理人员.
            string todoEmps = "";
            foreach (GenerWorkerList mygwl in gwls)
            {
                todoEmps += mygwl.FK_Emp + "," + mygwl.FK_EmpText + ";";
            }

            //更新主表信息.
            gwf.WFState = WFState.Shift;
            gwf.TodoEmpsNum = gwls.Count;
            gwf.TodoEmps = todoEmps; //处理人员.
            gwf.Update();

            //删除自己的待办.
            DBAccess.RunSQL("DELETE FROM WF_GenerWorkerList WHERE WorkID="+gwf.WorkID+" AND FK_Node="+gwf.FK_Node+" AND FK_Emp='"+WebUser.No+"'");

            //记录日志.
            Glo.AddToTrack(ActionType.Shift, nd.FK_Flow, workID, gwf.FID, nd.NodeID, nd.Name,
                WebUser.No, WebUser.Name, nd.NodeID, nd.Name, toEmps, "移交给多个人", msg, null);

            //移交后事件.
            string atPara = "@SendToEmpIDs=" + toEmps;
            WorkNode wn = new WorkNode(work, nd);
            info += "@" + ExecEvent.DoNode(EventListNode.ShitAfter, wn, null, atPara);
            return info;
        }
        /// <summary>
        /// 撤消移交
        /// </summary>
        /// <returns></returns>
        public static string DoUnShift(Int64 workid)
        {

            GenerWorkFlow gwf = new GenerWorkFlow(workid);
            GenerWorkerLists wls = new GenerWorkerLists();
            wls.Retrieve(GenerWorkerListAttr.WorkID, workid, GenerWorkerListAttr.FK_Node, gwf.FK_Node);
            if (wls.Count == 0)
                return "移交失败没有当前的工作。";

            Node nd = new Node(gwf.FK_Node);
            Work wk1 = nd.HisWork;
            wk1.OID = workid;
            wk1.Retrieve();

            // 记录日志.
            WorkNode wn = new WorkNode(wk1, nd);
            wn.AddToTrack(ActionType.UnShift, WebUser.No, WebUser.Name, nd.NodeID, nd.Name, "撤消移交");

            //删除撤销信息.
            DBAccess.RunSQL("DELETE FROM WF_ShiftWork WHERE WorkID=" + workid + " AND FK_Node=" + gwf.FK_Node);

            //更新流程主表字段信息
            gwf.WFState = WFState.Runing;
            gwf.Update();

            if (wls.Count == 1)
            {
                GenerWorkerList wl = (GenerWorkerList)wls[0];
                wl.FK_Emp = WebUser.No;
                wl.FK_EmpText = WebUser.Name;
                wl.IsEnable = true;
                wl.IsPass = false;
                wl.Update();
                return "@撤消移交成功。";
            }

            GenerWorkerList mywl = null;
            foreach (GenerWorkerList wl in wls)
            {
                if (wl.FK_Emp == WebUser.No)
                {
                    wl.FK_Emp = WebUser.No;
                    wl.FK_EmpText = WebUser.Name;
                    wl.IsEnable = true;
                    wl.IsPass = false;
                    wl.Update();
                    mywl = wl;
                }
                else
                {
                    wl.Delete();
                }
            }
            if (mywl != null)
                return "@撤消移交成功";

            GenerWorkerList wk = (GenerWorkerList)wls[0];
            GenerWorkerList wkNew = new GenerWorkerList();
            wkNew.Copy(wk);
            wkNew.FK_Emp = WebUser.No;
            wkNew.FK_EmpText = WebUser.Name;
            wkNew.IsEnable = true;
            wkNew.IsPass = false;
            wkNew.Insert();
            return "@撤消移交成功";
        }
    }
}
