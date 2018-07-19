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
            DoTask();

            //自动启动流程.
            DoAutuFlows();

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
                                BP.WF.Dev2Interface.Node_Shift(fk_flow, fk_node, workid, 0, empShift.No,
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
            myEmail.From = new  MailAddress("ccbpmtester@tom.com", "ccbpm123", System.Text.Encoding.UTF8);

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
        /// <summary>
        /// 自动启动流程
        /// </summary>
        /// <param name="fls"></param>
        private void DoAutuFlows()
        {
            BP.WF.Flows fls = new Flows();
            fls.RetrieveAll();

            #region 自动启动流程
            foreach (BP.WF.Flow fl in fls)
            {
                if (fl.HisFlowRunWay == BP.WF.FlowRunWay.HandWork)
                    continue;

                if (DateTime.Now.ToString("HH:mm") == fl.Tag)
                    continue;

                if (fl.RunObj == null || fl.RunObj == "")
                {
                    string msg = "您设置自动运行流程错误，没有设置流程内容，流程编号：" + fl.No + ",流程名称:" + fl.Name;
                    BP.DA.Log.DebugWriteError(msg);
                    continue;
                }

                #region 判断当前时间是否可以运行它。
                string nowStr = DateTime.Now.ToString("yyyy-MM-dd,HH:mm");
                string[] strs = fl.RunObj.Split('@'); //破开时间串。
                bool IsCanRun = false;
                foreach (string str in strs)
                {
                    if (string.IsNullOrEmpty(str))
                        continue;
                    if (nowStr.Contains(str))
                        IsCanRun = true;
                }

                if (IsCanRun == false)
                    continue;

                // 设置时间.
                fl.Tag = DateTime.Now.ToString("HH:mm");
                #endregion 判断当前时间是否可以运行它。

                // 以此用户进入.
                switch (fl.HisFlowRunWay)
                {
                    case BP.WF.FlowRunWay.SpecEmp: //指定人员按时运行。
                        string RunObj = fl.RunObj;
                        string fk_emp = RunObj.Substring(0, RunObj.IndexOf('@'));

                        BP.Port.Emp emp = new BP.Port.Emp();
                        emp.No = fk_emp;
                        if (emp.RetrieveFromDBSources() == 0)
                        {
                            BP.DA.Log.DebugWriteError("启动自动启动流程错误：发起人(" + fk_emp + ")不存在。");
                            continue;
                        }

                        try
                        {
                            //让 userNo 登录.
                            BP.WF.Dev2Interface.Port_Login(emp.No);

                            //创建空白工作, 发起开始节点.
                            Int64 workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fl.No);

                            //执行发送.
                            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fl.No, workID);

                            //string info_send= BP.WF.Dev2Interface.Node_StartWork(fl.No,);
                            BP.DA.Log.DefaultLogWriteLineInfo("流程:" + fl.No + fl.Name + "的定时任务\t\n -------------- \t\n" + objs.ToMsgOfText());

                        }
                        catch (Exception ex)
                        {
                            BP.DA.Log.DebugWriteError("流程:" + fl.No + fl.Name + "自动发起错误:\t\n -------------- \t\n" + ex.Message);
                        }
                        continue;
                    case BP.WF.FlowRunWay.DataModel: //按数据集合驱动的模式执行。
                        this.DTS_Flow(fl);
                        continue;
                    default:
                        break;
                }
            }
            if (BP.Web.WebUser.No != "admin")
            {
                BP.Port.Emp empadmin = new BP.Port.Emp("admin");
                BP.Web.WebUser.SignInOfGener(empadmin);
            }
            #endregion 发送消息
        }
        public void DTS_Flow(BP.WF.Flow fl)
        {
            #region 读取数据.
            BP.Sys.MapExt me = new MapExt();
            me.MyPK = "ND" + int.Parse(fl.No) + "01" + "_" + MapExtXmlList.StartFlow;
            int i = me.RetrieveFromDBSources();
            if (i == 0)
            {
                BP.DA.Log.DefaultLogWriteLineError("没有为流程(" + fl.Name + ")的开始节点设置发起数据,请参考说明书解决.");
                return;
            }
            if (string.IsNullOrEmpty(me.Tag))
            {
                BP.DA.Log.DefaultLogWriteLineError("没有为流程(" + fl.Name + ")的开始节点设置发起数据,请参考说明书解决.");
                return;
            }

            // 获取从表数据.
            DataSet ds = new DataSet();
            string[] dtlSQLs = me.Tag1.Split('*');
            foreach (string sql in dtlSQLs)
            {
                if (string.IsNullOrEmpty(sql))
                    continue;

                string[] tempStrs = sql.Split('=');
                string dtlName = tempStrs[0];
                DataTable dtlTable = BP.DA.DBAccess.RunSQLReturnTable(sql.Replace(dtlName + "=", ""));
                dtlTable.TableName = dtlName;
                ds.Tables.Add(dtlTable);
            }
            #endregion 读取数据.

            #region 检查数据源是否正确.
            string errMsg = "";
            // 获取主表数据.
            DataTable dtMain = BP.DA.DBAccess.RunSQLReturnTable(me.Tag);
            if (dtMain.Rows.Count == 0)
            {
                BP.DA.Log.DefaultLogWriteLineError("流程(" + fl.Name + ")此时无任务.");
                return;
            }


            if (dtMain.Columns.Contains("Starter") == false)
                errMsg += "@配值的主表中没有Starter列.";

            if (dtMain.Columns.Contains("MainPK") == false)
                errMsg += "@配值的主表中没有MainPK列.";

            if (errMsg.Length > 2)
            {
                BP.DA.Log.DefaultLogWriteLineError("流程(" + fl.Name + ")的开始节点设置发起数据,不完整." + errMsg);
                return;
            }
            #endregion 检查数据源是否正确.

            #region 处理流程发起.
            string nodeTable = "ND" + int.Parse(fl.No) + "01";
            int idx = 0;
            foreach (DataRow dr in dtMain.Rows)
            {
                idx++;

                string mainPK = dr["MainPK"].ToString();
                string sql = "SELECT OID FROM " + nodeTable + " WHERE MainPK='" + mainPK + "'";
                if (DBAccess.RunSQLReturnTable(sql).Rows.Count != 0)
                {
                    continue; /*说明已经调度过了*/
                }

                string starter = dr["Starter"].ToString();
                if (WebUser.No != starter)
                {
                    BP.Web.WebUser.Exit();
                    BP.Port.Emp emp = new BP.Port.Emp();
                    emp.No = starter;
                    if (emp.RetrieveFromDBSources() == 0)
                    {
                        BP.DA.Log.DefaultLogWriteLineInfo("@数据驱动方式发起流程(" + fl.Name + ")设置的发起人员:" + emp.No + "不存在。");
                        continue;
                    }
                    WebUser.SignInOfGener(emp);
                }

                #region  给值.
                //System.Collections.Hashtable ht = new Hashtable();

                Work wk = fl.NewWork();

                string err = "";
                #region 检查用户拼写的sql是否正确？
                foreach (DataColumn dc in dtMain.Columns)
                {
                    string f = dc.ColumnName.ToLower();
                    switch (f)
                    {
                        case "starter":
                        case "mainpk":
                        case "refmainpk":
                        case "tonode":
                            break;
                        default:
                            bool isHave = false;
                            foreach (Attr attr in wk.EnMap.Attrs)
                            {
                                if (attr.Key.ToLower() == f)
                                {
                                    isHave = true;
                                    break;
                                }
                            }
                            if (isHave == false)
                            {
                                err += " " + f + " ";
                            }
                            break;
                    }
                }
                if (string.IsNullOrEmpty(err) == false)
                    throw new Exception("您设置的字段:" + err + "不存在开始节点的表单中，设置的sql:" + me.Tag);

                #endregion 检查用户拼写的sql是否正确？

                foreach (DataColumn dc in dtMain.Columns)
                    wk.SetValByKey(dc.ColumnName, dr[dc.ColumnName].ToString());

                if (ds.Tables.Count != 0)
                {
                    // MapData md = new MapData(nodeTable);
                    MapDtls dtls = new MapDtls(nodeTable);
                    foreach (MapDtl dtl in dtls)
                    {
                        foreach (DataTable dt in ds.Tables)
                        {
                            if (dt.TableName != dtl.No)
                                continue;

                            //删除原来的数据。
                            GEDtl dtlEn = dtl.HisGEDtl;
                            dtlEn.Delete(GEDtlAttr.RefPK, wk.OID.ToString());

                            // 执行数据插入。
                            foreach (DataRow drDtl in dt.Rows)
                            {
                                if (drDtl["RefMainPK"].ToString() != mainPK)
                                    continue;

                                dtlEn = dtl.HisGEDtl;
                                foreach (DataColumn dc in dt.Columns)
                                    dtlEn.SetValByKey(dc.ColumnName, drDtl[dc.ColumnName].ToString());

                                dtlEn.RefPK = wk.OID.ToString();
                                dtlEn.OID = 0;
                                dtlEn.Insert();
                            }
                        }
                    }
                }
                #endregion  给值.


                int toNodeID = 0;
                try
                {
                    toNodeID = int.Parse(dr["ToNode"].ToString());
                }
                catch
                {
                    /*有可能在4.5以前的版本中没有tonode这个约定.*/
                }

                // 处理发送信息.
                //  Node nd =new Node();
                string msg = "";
                try
                {
                    if (toNodeID == 0)
                    {
                        WorkNode wn = new WorkNode(wk, fl.HisStartNode);
                        msg = wn.NodeSend().ToMsgOfText();
                    }

                    if (toNodeID == fl.StartNodeID)
                    {
                        /* 发起后让它停留在开始节点上，就是为开始节点创建一个待办。*/
                        Int64 workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fl.No, null, null, WebUser.No, null);
                        if (workID != wk.OID)
                            throw new Exception("@异常信息:不应该不一致的workid.");
                        else
                            wk.Update();
                        msg = "已经为(" + WebUser.No + ") 创建了开始工作节点. ";
                    }

                    BP.DA.Log.DefaultLogWriteLineInfo(msg);
                }
                catch (Exception ex)
                {
                    BP.DA.Log.DefaultLogWriteLineWarning("@" + fl.Name + ",第" + idx + "条,发起人员:" + WebUser.No + "-" + WebUser.Name + "发起时出现错误.\r\n" + ex.Message);
                }
            }
            #endregion 处理流程发起.
        }
        /// <summary>
        /// 触发式发起流程
        /// </summary>
        public void DoTask()
        {
            string sql = "SELECT * FROM WF_Task WHERE TaskSta=0 ORDER BY Starter";
            DataTable dt = null;
            try
            {
                dt = DBAccess.RunSQLReturnTable(sql);
            }
            catch
            {
                Task ta = new Task();
                ta.CheckPhysicsTable();
                dt = DBAccess.RunSQLReturnTable(sql);
            }

            if (dt.Rows.Count == 0)
                return;

            #region 自动启动流程
            foreach (DataRow dr in dt.Rows)
            {
                string mypk = dr["MyPK"].ToString();
                string taskSta = dr["TaskSta"].ToString();
                string paras = dr["Paras"].ToString();
                string starter = dr["Starter"].ToString();
                string fk_flow = dr["FK_Flow"].ToString();

                string startDT = dr[TaskAttr.StartDT].ToString();
                if (string.IsNullOrEmpty(startDT) == false)
                {
                    /*如果设置了发起时间,就检查当前时间是否与现在的时间匹配.*/
                    if (DateTime.Now.ToString("yyyy-MM-dd HH:mm").Contains(startDT) == false)
                        continue;
                }

                Flow fl = new Flow(fk_flow);

                BP.DA.Log.DebugWriteError("开始执行(" + starter + ")发起(" + fl.Name + ")流程.");
                try
                {
                    string fTable = "ND" + int.Parse(fl.No + "01").ToString();
                    MapData md = new MapData(fTable);
                    sql = "";
                    //   sql = "SELECT * FROM " + md.PTable + " WHERE MainPK='" + mypk + "' AND WFState=1";
                    try
                    {
                        if (DBAccess.RunSQLReturnTable(sql).Rows.Count != 0)
                            continue;
                    }
                    catch(Exception ex)
                    {
                        BP.DA.Log.DebugWriteError(ex.Message + "err@开始节点表单表:" + fTable + "没有设置的默认字段MainPK. " + sql);
                        continue;
                    }

                    if (BP.Web.WebUser.No != starter)
                    {
                        BP.Web.WebUser.Exit();
                        BP.Port.Emp empadmin = new BP.Port.Emp(starter);
                        BP.Web.WebUser.SignInOfGener(empadmin);
                    }

                    Work wk = fl.NewWork();
                    string[] strs = paras.Split('@');
                    foreach (string str in strs)
                    {
                        if (string.IsNullOrEmpty(str))
                            continue;

                        if (str.Contains("=") == false)
                            continue;

                        string[] kv = str.Split('=');
                        wk.SetValByKey(kv[0], kv[1]);
                    }

                    wk.SetValByKey("MainPK", mypk);
                    wk.Update();

                    WorkNode wn = new WorkNode(wk, fl.HisStartNode);
                    string msg = wn.NodeSend().ToMsgOfText();
                    msg = msg.Replace("'", "~");
                    DBAccess.RunSQL("UPDATE WF_Task SET TaskSta=1,Msg='" + msg + "' WHERE MyPK='" + mypk + "'");
                }
                catch (Exception ex)
                {
                    //如果发送错误。
                    string msg = ex.Message;
                    try
                    {
                        DBAccess.RunSQL("UPDATE WF_Task SET TaskSta=2,Msg='" + msg + "' WHERE MyPK='" + mypk + "'");
                    }
                    catch
                    {
                        Task TK = new Task();
                        TK.CheckPhysicsTable();
                    }
                }
            }
            #endregion 自动启动流程
        }
    }
}
