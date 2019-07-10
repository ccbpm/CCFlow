using System;
using System.Data;
using BP.DA ; 
using System.Collections;
using System.Net.Mail;
using BP.En;
using BP.WF;
using BP.Port ; 
using BP.En;
using BP.Sys;
using BP.WF.Data;
using BP.WF.Template;
using BP.DTS;
using BP.Web;

namespace BP.WF.DTS
{
    /// <summary>
    /// ccbpm服务
    /// </summary>
    public class ccbpmServices : Method
    {
        /// <summary>
        /// ccbpm服务
        /// </summary>
        public ccbpmServices()
        {
            this.Title = "ccbpm服务(1,自动发送邮件. 2,自动发起流程. 3,自动执行节点任务. )";
        }
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
        /// 开始执行方法.
        /// </summary>
        /// <returns></returns>
        public override object Do()
        {
            //扫描触发式自动发起流程表......
            //自动发起流程.
            AutoRunWF_Task wf_task = new AutoRunWF_Task();
            wf_task.Do();

            //自动发起流程.
            AutoRunStratFlows fls = new AutoRunStratFlows();
            fls.Do();

            //扫描消息表,想外发送消息....
            DoSendMsg();

            //扫描逾期流程数据，处理逾期流程.
            DTS_DealDeferredWork en = new DTS_DealDeferredWork();
            en.Do();

            //同步待办时间戳
            DTS_GenerWorkFlowTimeSpan en2 = new DTS_GenerWorkFlowTimeSpan();
            en2.Do();

            //更新WF_GenerWorkerFlow.TodoSta状态.
            DTS_GenerWorkFlowTodoSta en3 = new DTS_GenerWorkFlowTodoSta();
            en3.Do();

            return "执行完成...";
        }
        /// <summary>
        /// 逾期流程
        /// </summary>
        private void DoOverDueFlow()
        {
            //特殊处理天津的需求.
            if (SystemConfig.CustomerNo == "")
                DoTianJinSpecFunc();
            # region  流程逾期
            //判断是否有流程逾期的消息设置
            DataTable dt = null;
            string sql = "SELECT a.FK_Flow,a.WorkID,a.Title,a.FK_Node,a.SDTOfNode,a.Starter,a.TodoEmps ";
            sql += "FROM WF_GenerWorkFlow a, WF_Node b";
            sql += " WHERE a.SDTOfFlow<='" + DataType.CurrentDataTime + "' ";
            sql += " AND WFState=2 and b.OutTimeDeal!=0";
            sql += " AND a.FK_Node=b.NodeID";
            dt = DBAccess.RunSQLReturnTable(sql);
            // 遍历循环,逾期表进行处理.
            foreach(DataRow dr in dt.Rows){
                string fk_flow = dr["FK_Flow"] + "";
                int fk_node = int.Parse(dr["FK_Node"] + "");
                long workid = long.Parse(dr["WorkID"] + "");
                string title = dr["Title"] + "";
                //判断流程是否设置逾期消息
                PushMsg pushMsg = new PushMsg();
                int count = pushMsg.Retrieve(PushMsgAttr.FK_Flow, fk_flow, PushMsgAttr.FK_Node, 0, PushMsgAttr.FK_Event, EventListOfNode.FlowOverDue);
                if (count != 0)
                {
                    Node node = new Node(fk_node);
                    pushMsg.DoSendMessage(node, node.HisWork, null, null, null, null);
                }
                continue;
            }
            #endregion 流程逾期

            # region  流程预警 
            sql = "SELECT a.FK_Flow,a.WorkID,a.Title,a.FK_Node,a.SDTOfNode,a.Starter,a.TodoEmps ";
            sql += "FROM WF_GenerWorkFlow a, WF_Node b";
            sql += " WHERE a.SDTOfFlowWarning<='" + DataType.CurrentDataTime + "' ";
            sql += " AND WFState=2 and b.OutTimeDeal!=0";
            sql += " AND a.FK_Node=b.NodeID";
            dt = DBAccess.RunSQLReturnTable(sql);
            // 遍历循环,预警表进行处理.
            foreach (DataRow dr in dt.Rows)
            {
                string fk_flow = dr["FK_Flow"] + "";
                int fk_node = int.Parse(dr["FK_Node"] + "");
                long workid = long.Parse(dr["WorkID"] + "");
                string title = dr["Title"] + "";
                //判断流程是否设置逾期消息
                PushMsg pushMsg = new PushMsg();
                int count = pushMsg.Retrieve(PushMsgAttr.FK_Flow, fk_flow, PushMsgAttr.FK_Node, 0, PushMsgAttr.FK_Event, EventListOfNode.FlowWarning);
                if (count != 0)
                {
                    Node node = new Node(fk_node);
                    pushMsg.DoSendMessage(node, node.HisWork, null, null, null, null);
                }
                continue;
            }
            # endregion  流程预警

            #region 找到要节点逾期的数据.
            DataTable generTab = null;
            sql = "SELECT a.FK_Flow,a.WorkID,a.Title,a.FK_Node,a.SDTOfNode,a.Starter,a.TodoEmps ";
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
                    #region 启动逾期消息设置
                    PushMsg pushMsg = new PushMsg();
                    int count = pushMsg.Retrieve(PushMsgAttr.FK_Flow, node.FK_Flow, PushMsgAttr.FK_Node, node.NodeID, PushMsgAttr.FK_Event, EventListOfNode.NodeOverDue);
                    if (count != 0)
                    {
                        pushMsg.DoSendMessage(node, node.HisWork, null, null, null, null);
                    }
                    #endregion 启动逾期消息设置

                    //获得该节点的处理内容.
                    string doOutTime = node.GetValStrByKey(NodeAttr.DoOutTime);
                    switch (node.HisOutTimeDeal)
                    {
                        case OutTimeDeal.None: //逾期不处理.
                            continue;
                        case OutTimeDeal.AutoJumpToSpecNode: //跳转到指定的节点.
                            try
                            {
                               
                                int jumpNode = int.Parse(doOutTime);
                                Node jumpToNode = new Node(jumpNode);

                                //设置默认同意.
                                BP.WF.Dev2Interface.WriteTrackWorkCheck(jumpToNode.FK_Flow, node.NodeID, workid, 0,
                                    "同意（预期自动审批）", null);

                                //执行发送.
                                info = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid, null, null, jumpToNode.NodeID, null).ToMsgOfText();

                                msg = "流程 '" + node.FlowName + "',标题: '" + title + "'的应该完成时间为'" + compleateTime + "',当前节点'" + node.Name +
                                      "'超时处理规则为'自动跳转'," + info;

                                BP.DA.Log.DefaultLogWriteLine(LogType.Info, msg);

                            }
                            catch (Exception ex)
                            {
                                msg = "流程 '" + node.FlowName + "',WorkID=" + workid + ",标题: '" + title + "'的应该完成时间为'" + compleateTime + "',当前节点'" + node.Name +
                                      "'超时处理规则为'自动跳转',跳转异常:" + ex.Message;
                                BP.DA.Log.DefaultLogWriteLine(LogType.Error, msg);
                            }
                            break;
                        case OutTimeDeal.AutoShiftToSpecUser: //走动移交给.
                            // 判断当前的处理人是否是.
                            Emp empShift = new Emp(doOutTime);
                            try
                            {
                                BP.WF.Dev2Interface.Node_Shift( workid,   empShift.No,
                                    "流程节点已经逾期,系统自动移交");

                                msg = "流程 '" + node.FlowName + "',标题: '" + title + "'的应该完成时间为'" + compleateTime + "',当前节点'" + node.Name +
                                      "'超时处理规则为'移交到指定的人',已经自动移交给'" + empShift.Name + ".";
                                BP.DA.Log.DefaultLogWriteLine(LogType.Info, msg);
                            }
                            catch (Exception ex)
                            {
                                msg = "流程 '" + node.FlowName + "' ,标题:'" + title + "'的应该完成时间为'" + compleateTime + "',当前节点'" + node.Name +
                                      "'超时处理规则为'移交到指定的人',移交异常：" + ex.Message;
                                BP.DA.Log.DefaultLogWriteLine(LogType.Error, msg);
                            }
                            break;
                        case OutTimeDeal.AutoTurntoNextStep:
                            try
                            {
                                GenerWorkerList workerList = new GenerWorkerList();
                                workerList.RetrieveByAttrAnd(GenerWorkerListAttr.WorkID, workid,
                                    GenerWorkFlowAttr.FK_Node, fk_node);

                                WebUser.SignInOfGener(workerList.HisEmp);
                                WorkNode firstwn = new WorkNode(workid, fk_node);
                                string sendIfo = firstwn.NodeSend().ToMsgOfText();
                                msg = "流程  '" + node.FlowName + "',标题: '" + title + "'的应该完成时间为'" + compleateTime + "',当前节点'" + node.Name +
                                      "'超时处理规则为'自动发送到下一节点',发送消息为:" + sendIfo;
                                BP.DA.Log.DefaultLogWriteLine(LogType.Info, msg);
                            }
                            catch (Exception ex)
                            {
                                msg = "流程  '" + node.FlowName + "',标题: '" + title + "'的应该完成时间为'" + compleateTime + "',当前节点'" + node.Name +
                                      "'超时处理规则为'自动发送到下一节点',发送异常:" + ex.Message;
                                BP.DA.Log.DefaultLogWriteLine(LogType.Error, msg);
                            }
                            break;
                        case OutTimeDeal.DeleteFlow:
                            info = BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(fk_flow, workid, true);
                            msg = "流程  '" + node.FlowName + "',标题: '" + title + "'的应该完成时间为'" + compleateTime + "',当前节点'" + node.Name +
                                  "'超时处理规则为'删除流程'," + info;
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
                                BP.DA.Log.DefaultLogWriteLine(LogType.Info, msg);
                            }
                            catch (Exception ex)
                            {
                                msg = "流程  '" + node.FlowName + "',标题: '" + title + "'的应该完成时间为'" + compleateTime + "',当前节点'" + node.Name +
                                      "'超时处理规则为'执行SQL'.运行SQL出现异常:" + ex.Message;
                                BP.DA.Log.DefaultLogWriteLine(LogType.Error, msg);
                            }
                            break;
                        default:
                            msg = "流程 '" + node.FlowName + "',标题: '" + title + "'的应该完成时间为'" + compleateTime + "',当前节点'" + node.Name +
                                  "'没有找到相应的超时处理规则.";
                            BP.DA.Log.DefaultLogWriteLine(LogType.Error, msg);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    BP.DA.Log.DefaultLogWriteLine(LogType.Error, ex.ToString());
                }
            }
            BP.DA.Log.DefaultLogWriteLine(LogType.Info, "结束扫描逾期流程数据.");
        }
        /// <summary>
        /// 特殊处理天津的流程
        /// 当指定的节点，到了10号，15号自动向下发送.
        /// </summary>
        private void DoTianJinSpecFunc()
        {
            if (DateTime.Now.Day == 10 || DateTime.Now.Day == 15)
            {
                /* 一个是10号自动审批，一个是15号自动审批. */
            }
            else
            {
                return;
            }

            #region 找到要逾期的数据.
            DataTable generTab = null;
            string sql = "SELECT a.FK_Flow,a.WorkID,a.Title,a.FK_Node,a.SDTOfNode,a.Starter,a.TodoEmps ";
            sql += "FROM WF_GenerWorkFlow a, WF_Node b";
            sql += " WHERE  ";
            sql += "   a.FK_Node=b.NodeID  ";

            if (DateTime.Now.Day == 10)
                sql += "   AND  b.NodeID=13304 ";

            if (DateTime.Now.Day == 15)
                sql += "AND b.NodeID=13302 ";

            generTab = DBAccess.RunSQLReturnTable(sql);
            #endregion 找到要逾期的数据.

            // 遍历循环,逾期表进行处理.
            string msg = "";
            foreach (DataRow row in generTab.Rows)
            {
                string fk_flow = row["FK_Flow"] + "";
                string fk_node = row["FK_Node"] + "";
                long workid = long.Parse(row["WorkID"] + "");
                string title = row["Title"] + "";
                string compleateTime = row["SDTOfNode"] + "";
                string starter = row["Starter"] + "";
                try
                {
                    Node node = new Node(int.Parse(fk_node));

                    try
                    {
                        GenerWorkerList workerList = new GenerWorkerList();
                        workerList.RetrieveByAttrAnd(GenerWorkerListAttr.WorkID, workid,
                            GenerWorkFlowAttr.FK_Node, fk_node);

                        WebUser.SignInOfGener(workerList.HisEmp);

                        WorkNode firstwn = new WorkNode(workid, int.Parse(fk_node));
                        string sendIfo = firstwn.NodeSend().ToMsgOfText();
                        msg = "流程  '" + node.FlowName + "',标题: '" + title + "'的应该完成时间为'" + compleateTime + "',当前节点'" + node.Name +
                              "'超时处理规则为'自动发送到下一节点',发送消息为:" + sendIfo;

                        //输出消息.
                        BP.DA.Log.DefaultLogWriteLine(LogType.Info, msg);
                    }
                    catch (Exception ex)
                    {
                        msg = "流程  '" + node.FlowName + "',标题: '" + title + "'的应该完成时间为'" + compleateTime + "',当前节点'" + node.Name +
                              "'超时处理规则为'自动发送到下一节点',发送异常:" + ex.Message;
                        BP.DA.Log.DefaultLogWriteLine(LogType.Error, msg);
                    }
                }
                catch (Exception ex)
                {
                    BP.DA.Log.DefaultLogWriteLine(LogType.Error, ex.ToString());
                }
            }
            BP.DA.Log.DefaultLogWriteLine(LogType.Info, "结束扫描逾期流程数据.");
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        private void DoSendMsg()
        {
            int idx = 0;
            #region 发送消息
            SMSs sms = new SMSs();
            BP.En.QueryObject qo = new BP.En.QueryObject(sms);
            sms.Retrieve(SMSAttr.EmailSta, (int)MsgSta.UnRun);
            foreach (SMS sm in sms)
            {
                if (sm.Email.Length == 0)
                {
                    sm.HisEmailSta = MsgSta.RunOK;
                    sm.Update();
                    continue;
                }
                try
                {
                    this.SendMail(sm);
                }
                catch (Exception ex)
                {
                    BP.DA.Log.DefaultLogWriteLineError(ex.Message);
                }
            }
            #endregion 发送消息
        }
        /// <summary>
        /// 发送邮件。
        /// </summary>
        /// <param name="sms"></param>
        public void SendMail(SMS sms)
        {

            #region 发送邮件.
            if (string.IsNullOrEmpty(sms.Email))
            {
                BP.WF.Port.WFEmp emp = new BP.WF.Port.WFEmp(sms.SendToEmpNo);
                sms.Email = emp.Email;
            }

            System.Net.Mail.MailMessage myEmail = new System.Net.Mail.MailMessage();
            myEmail.From = new MailAddress("ccbpmtester@tom.com", "ccbpm123", System.Text.Encoding.UTF8);

            myEmail.To.Add(sms.Email);
            myEmail.Subject = sms.Title;
            myEmail.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码

            myEmail.Body = sms.DocOfEmail;
            myEmail.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码
            myEmail.IsBodyHtml = true;//是否是HTML邮件

            myEmail.Priority = MailPriority.High;//邮件优先级

            SmtpClient client = new SmtpClient();

            //邮件地址.
            string emailAddr = SystemConfig.GetValByKey("SendEmailAddress", null);
            if (emailAddr == null)
                emailAddr = "ccbpmtester@tom.com";

            string emailPassword = SystemConfig.GetValByKey("SendEmailPass", null);
            if (emailPassword == null)
                emailPassword = "ccbpm123";

            //是否启用ssl? 
            bool isEnableSSL = false;
            string emailEnableSSL = SystemConfig.GetValByKey("SendEmailEnableSsl", null);
            if (emailEnableSSL == null || emailEnableSSL == "0")
                isEnableSSL = false;
            else
                isEnableSSL = true;

            client.Credentials = new System.Net.NetworkCredential(emailAddr, emailPassword);

            //上述写你的邮箱和密码
            client.Port = SystemConfig.GetValByKeyInt("SendEmailPort", 25); //使用的端口
            client.Host = SystemConfig.GetValByKey("SendEmailHost", "smtp.tom.com");

            //是否启用加密,有的邮件服务器发送配置不成功就是因为此参数的错误。
            client.EnableSsl = SystemConfig.GetValByKeyBoolen("SendEmailEnableSsl", isEnableSSL);

            object userState = myEmail;
            try
            {
                client.SendAsync(myEmail, userState);
                sms.HisEmailSta = MsgSta.RunOK;
                sms.Update();
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                throw ex;
            }
            #endregion 发送邮件.
        }
    }
}
