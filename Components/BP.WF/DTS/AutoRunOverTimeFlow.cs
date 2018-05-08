using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
using BP.Sys;
using BP.WF.Data;
using BP.WF.Template;

namespace BP.WF.DTS
{
    /// <summary>
    /// Method 的摘要说明
    /// </summary>
    public class AutoRunOverTimeFlow : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public AutoRunOverTimeFlow()
        {
            this.Title = "处理逾期的任务";
            this.Help = "扫描并处理逾期的任务，按照节点配置的预期规则";
        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
        }
        /// <summary>
        /// 当前的操纵员是否可以执行这个方法
        /// </summary>
        public override bool IsCanDo
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {
            #region 找到要逾期的数据.
            DataTable generTab = null;
            string sql = "SELECT a.FK_Flow,a.WorkID,a.Title,a.FK_Node,a.SDTOfNode,a.Starter,a.TodoEmps ";
            sql += "FROM WF_GenerWorkFlow a, WF_Node b";
            sql += " WHERE a.SDTOfNode<='" + DataType.CurrentDataTime + "' ";
            sql += " AND WFState=2 and b.OutTimeDeal!=0";
            sql += " AND a.FK_Node=b.NodeID";
            generTab = DBAccess.RunSQLReturnTable(sql);
            #endregion 找到要逾期的数据.

            // 遍历循环,逾期表进行处理.
            string msg = "";
            string info = "";
            foreach (DataRow row in generTab.Rows)
            {
                string fk_flow = row["FK_Flow"] + "";
                int fk_node = int.Parse(row["FK_Node"] + "");
                long workid = long.Parse(row["WorkID"] + "");
                string title = row["Title"] + "";
                string compleateTime = row["SDTOfNode"] + "";
                string starter = row["Starter"] + "";


                GenerWorkerLists gwls = new GenerWorkerLists();
                gwls.Retrieve(GenerWorkerListAttr.WorkID, workid, GenerWorkerListAttr.FK_Node, fk_node);


                bool isLogin = false;
                foreach (GenerWorkerList item in gwls)
                {
                    if (item.IsEnable == false)
                        continue;

                    BP.Port.Emp emp = new Emp(item.FK_Emp);
                    BP.Web.WebUser.SignInOfGener(emp);
                    isLogin = true;
                }

                if (isLogin == false)
                {
                    BP.Port.Emp emp = new Emp("admin");
                    BP.Web.WebUser.SignInOfGener(emp);
                }


                try
                {
                    Node node = new Node(fk_node);
                    if (node.IsStartNode)
                        continue;

                    //获得该节点的处理内容.
                    string doOutTime = node.GetValStrByKey(NodeAttr.DoOutTime);
                    switch (node.HisOutTimeDeal)
                    {
                        case OutTimeDeal.None: //逾期不处理.
                            continue;
                        case OutTimeDeal.AutoJumpToSpecNode: //跳转到指定的节点.
                            try
                            {
                                //if (doOutTime.Contains(",") == false)
                                //    throw new Exception("@系统设置错误，不符合设置规范,格式为: NodeID,EmpNo  现在设置的为:"+doOutTime);

                                int jumpNode = int.Parse(doOutTime);
                                Node jumpToNode = new Node(jumpNode);

                                //设置默认同意.
                                BP.WF.Dev2Interface.WriteTrackWorkCheck(jumpToNode.FK_Flow, node.NodeID, workid, 0,
                                    "同意（预期自动审批）", null);

                                //执行发送.
                                info = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid, null, null, jumpToNode.NodeID, null).ToMsgOfText();

                                // info = BP.WF.Dev2Interface.Flow_Schedule(workid, jumpToNode.NodeID, emp.No);
                                msg = "流程 '" + node.FlowName + "',标题: '" + title + "'的应该完成时间为'" + compleateTime + "',当前节点'" + node.Name +
                                      "'超时处理规则为'自动跳转'," + info;


                                SetText(msg);
                                BP.DA.Log.DefaultLogWriteLine(LogType.Info, msg);

                            }
                            catch (Exception ex)
                            {
                                msg = "流程 '" + node.FlowName + "',WorkID=" + workid + ",标题: '" + title + "'的应该完成时间为'" + compleateTime + "',当前节点'" + node.Name +
                                      "'超时处理规则为'自动跳转',跳转异常:" + ex.Message;
                                SetText(msg);
                                BP.DA.Log.DefaultLogWriteLine(LogType.Error, msg);
                            }
                            break;
                        case OutTimeDeal.AutoShiftToSpecUser: //走动移交给.
                            // 判断当前的处理人是否是.
                            Emp empShift = new Emp(doOutTime);
                            try
                            {
                                BP.WF.Dev2Interface.Node_Shift(fk_flow, fk_node, workid, 0, empShift.No,
                                    "流程节点已经逾期,系统自动移交");

                                msg = "流程 '" + node.FlowName + "',标题: '" + title + "'的应该完成时间为'" + compleateTime + "',当前节点'" + node.Name +
                                      "'超时处理规则为'移交到指定的人',已经自动移交给'" + empShift.Name + ".";
                                SetText(msg);
                                BP.DA.Log.DefaultLogWriteLine(LogType.Info, msg);
                            }
                            catch (Exception ex)
                            {
                                msg = "流程 '" + node.FlowName + "' ,标题:'" + title + "'的应该完成时间为'" + compleateTime + "',当前节点'" + node.Name +
                                      "'超时处理规则为'移交到指定的人',移交异常：" + ex.Message;
                                SetText(msg);
                                BP.DA.Log.DefaultLogWriteLine(LogType.Error, msg);
                            }
                            break;
                        case OutTimeDeal.AutoTurntoNextStep:
                            try
                            {
                                GenerWorkerList workerList = new GenerWorkerList();
                                workerList.RetrieveByAttrAnd(GenerWorkerListAttr.WorkID, workid,
                                    GenerWorkFlowAttr.FK_Node, fk_node);

                                BP.Web.WebUser.SignInOfGener(workerList.HisEmp);

                                WorkNode firstwn = new WorkNode(workid, fk_node);
                                string sendIfo = firstwn.NodeSend().ToMsgOfText();
                                msg = "流程  '" + node.FlowName + "',标题: '" + title + "'的应该完成时间为'" + compleateTime + "',当前节点'" + node.Name +
                                      "'超时处理规则为'自动发送到下一节点',发送消息为:" + sendIfo;
                                SetText(msg);
                                BP.DA.Log.DefaultLogWriteLine(LogType.Info, msg);
                            }
                            catch (Exception ex)
                            {
                                msg = "流程  '" + node.FlowName + "',标题: '" + title + "'的应该完成时间为'" + compleateTime + "',当前节点'" + node.Name +
                                      "'超时处理规则为'自动发送到下一节点',发送异常:" + ex.Message;
                                SetText(msg);
                                BP.DA.Log.DefaultLogWriteLine(LogType.Error, msg);
                            }
                            break;
                        case OutTimeDeal.DeleteFlow:
                            info = BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(fk_flow, workid, true);
                            msg = "流程  '" + node.FlowName + "',标题: '" + title + "'的应该完成时间为'" + compleateTime + "',当前节点'" + node.Name +
                                  "'超时处理规则为'删除流程'," + info;
                            SetText(msg);
                            BP.DA.Log.DefaultLogWriteLine(LogType.Info, msg);
                            break;
                        case OutTimeDeal.RunSQL:
                            try
                            {
                                BP.WF.Work wk = node.HisWork;
                                wk.OID = workid;
                                wk.Retrieve();

                                doOutTime = BP.WF.Glo.DealExp(doOutTime, wk, null);

                                //替换字符串
                                doOutTime.Replace("@OID", workid + "");
                                doOutTime.Replace("@FK_Flow", fk_flow);
                                doOutTime.Replace("@FK_Node", fk_node.ToString());
                                doOutTime.Replace("@Starter", starter);
                                if (doOutTime.Contains("@"))
                                {
                                    msg = "流程 '" + node.FlowName + "',标题:  '" + title + "'的应该完成时间为'" + compleateTime + "',当前节点'" + node.Name +
                                          "'超时处理规则为'执行SQL'.有未替换的SQL变量.";
                                    SetText(msg);
                                    BP.DA.Log.DefaultLogWriteLine(LogType.Info, msg);
                                    break;
                                }

                                //执行sql.
                                DBAccess.RunSQL(doOutTime);
                            }
                            catch (Exception ex)
                            {
                                msg = "流程  '" + node.FlowName + "',标题: '" + title + "'的应该完成时间为'" + compleateTime + "',当前节点'" + node.Name +
                                      "'超时处理规则为'执行SQL'.运行SQL出现异常:" + ex.Message;
                                SetText(msg);
                                BP.DA.Log.DefaultLogWriteLine(LogType.Error, msg);
                            }
                            break;
                        case OutTimeDeal.SendMsgToSpecUser:
                            try
                            {
                                Emp myemp = new Emp(doOutTime);

                                bool boo = BP.WF.Dev2Interface.WriteToSMS(myemp.No, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "系统发送逾期消息",
                                    "您的流程:'" + title + "'的完成时间应该为'" + compleateTime + "',流程已经逾期,请及时处理!", "系统消息");
                                if (boo)
                                    msg = "'" + title + "'逾期消息已经发送给:'" + myemp.Name + "'";
                                else
                                    msg = "'" + title + "'逾期消息发送未成功,发送人为:'" + myemp.Name + "'";
                                SetText(msg);
                                BP.DA.Log.DefaultLogWriteLine(LogType.Info, msg);
                            }
                            catch (Exception ex)
                            {
                                msg = "流程  '" + node.FlowName + "',标题: '" + title + "'的应该完成时间为'" + compleateTime + "',当前节点'" + node.Name +
                                      "'超时处理规则为'执行SQL'.运行SQL出现异常:" + ex.Message;
                                SetText(msg);
                                BP.DA.Log.DefaultLogWriteLine(LogType.Error, msg);
                            }
                            break;
                        default:
                            msg = "流程 '" + node.FlowName + "',标题: '" + title + "'的应该完成时间为'" + compleateTime + "',当前节点'" + node.Name +
                                  "'没有找到相应的超时处理规则.";
                            SetText(msg);
                            BP.DA.Log.DefaultLogWriteLine(LogType.Error, msg);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    SetText("流程逾期出现异常:" + ex.Message);
                    BP.DA.Log.DefaultLogWriteLine(LogType.Error, ex.ToString());

                }
            }
            return generInfo;
        }

        public string generInfo="";
        public void SetText(string msg)
        {
            generInfo += "\t\n" + msg;
        }
    }
}
