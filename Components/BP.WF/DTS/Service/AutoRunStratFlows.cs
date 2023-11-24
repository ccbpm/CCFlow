using System;
using System.Data;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Sys;

namespace BP.WF.DTS
{
    /// <summary>
    /// Method 的摘要说明
    /// </summary>
    public class AutoRunStratFlows : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public AutoRunStratFlows()
        {
            this.Title = "自动启动流程";
            this.Help = "在流程属性上配置的信息,自动发起流程,按照时间规则....";
            this.GroupName = "流程自动执行定时任务";

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
            BP.WF.Flows fls = new Flows();
            fls.RetrieveAll();

            string msg = "开始执行自动发起流程 :" + DataType.CurrentDateTimess;

            DateTime dt = DateTime.Now;
            int week = (int)dt.DayOfWeek;
            week++;

            #region 自动启动流程
            foreach (BP.WF.Flow fl in fls)
            {
                if (fl.HisFlowRunWay == BP.WF.FlowRunWay.HandWork)
                    continue;

                msg += "<br>扫描：" + fl.Name;

                #region 高级设置模式，是否达到启动的时间点？
                if (fl.HisFlowRunWay == FlowRunWay.SpecEmpAdv || fl.HisFlowRunWay == FlowRunWay.SpecEmp)
                {
                    string currTime = DataType.CurrentDateTime; //2022-09-01 09:10
                    string[] strs = null;

                    if (fl.HisFlowRunWay == FlowRunWay.SpecEmp)
                        strs = fl.RunObj.Split('@');

                    if (fl.HisFlowRunWay == FlowRunWay.SpecEmpAdv)
                        strs = fl.StartGuidePara2.Split('@'); //@11:15@12:15

                    bool isHave = false; //是否可以执行.
                    foreach (string s in strs)
                    {
                        if (DataType.IsNullOrEmpty(s) == true)
                            continue;
                        // 去除首尾空格
                        string str = s.Trim().Clone() as string;

                        //如果有周.
                        if (str.Contains("Week.") == true)
                        {
                            if (str.Contains("Week.1") == true && dt.DayOfWeek != DayOfWeek.Monday)
                                continue;
                            if (str.Contains("Week.2") == true && dt.DayOfWeek != DayOfWeek.Tuesday)
                                continue;
                            if (str.Contains("Week.3") == true && dt.DayOfWeek != DayOfWeek.Wednesday)
                                continue;
                            if (str.Contains("Week.4") == true && dt.DayOfWeek != DayOfWeek.Thursday)
                                continue;
                            if (str.Contains("Week.5") == true && dt.DayOfWeek != DayOfWeek.Friday)
                                continue;
                            if (str.Contains("Week.6") == true && dt.DayOfWeek != DayOfWeek.Saturday)
                                continue;
                            if (str.Contains("Week.7") == true && dt.DayOfWeek != DayOfWeek.Sunday)
                                continue;

                            str = str.Replace("Week.1", "");
                            str = str.Replace("Week.2", "");
                            str = str.Replace("Week.3", "");
                            str = str.Replace("Week.4", "");
                            str = str.Replace("Week.5", "");
                            str = str.Replace("Week.6", "");
                            str = str.Replace("Week.7", "");
                            str = str.Trim();
                        }

                        //是否每月的最后一天？
                        if (str.Contains("LastDayOfMonth") == true)
                        {
                            //获得当前月份有多少天.
                            int days = DateTime.DaysInMonth(dt.Year, dt.Month);
                            if (dt.Day != days)
                                continue;
                            str = str.Replace("LastDayOfMonth", "").Trim();
                        }

                        //如果自动发起流程过多，会延迟判断时间点，要补偿自动发起
                        if (!str.Contains(":"))
                            continue;

                        // 逻辑修正， 不包含: 不执行，不用再判断:
                        //是不是到时间了？
                        //if (str.Contains(":"))
                        //{
                            int i = str.Length;
                            string currTime01 = currTime.Substring(currTime.Length - i);
                            DateTime dt1 = Convert.ToDateTime(str);
                            DateTime dt2 = Convert.ToDateTime(currTime01);
                            if (DateTime.Compare(dt1, dt2) > 0) continue;
                        //}

                        //记录执行过的时间点，如果有该时间点，就不要在执行了。
                        // 这里的时间点是有问题的，之前是根据当前时刻计算，其实是不对的。
                        // 应该根据设定的时间保存时刻。
                        string pkval = "";
                        //是按照一天的时间点计算的.
                        if (s.Contains("LastDayOfMonth")) // 月度任务
                            pkval = "AutoFlow_" + fl.No + "_" + dt1.ToString("yyyyMM") + str;
                        else if (s.Contains("Week.")) //按周计算.
                            pkval = "AutoFlow_" + fl.No + "_" + dt1.ToString("yyyyMMdd") + dt.DayOfWeek + str;
                        else if (str.Length <= 6)
                            pkval = "AutoFlow_" + fl.No + "_" + dt1.ToString("yyyyMMdd") + str;
                        else
                            BP.DA.Log.DebugWriteError("不合法的发起规则: " + s);


                        BP.Sys.GloVar gv = new GloVar
                        {
                            No = pkval
                        };
                        if (gv.RetrieveFromDBSources() == 0)
                        {
                            gv.Name = fl.Name + "自动发起.";
                            gv.GroupKey = "AutoStartFlow";
                            gv.Insert();
                            BP.DA.Log.DebugWriteInfo("任务发起：" + gv.Name + ", No: " + gv.No);
                            isHave = true; //可以执行.
                            break;
                        }
                    }
                    if (isHave == false)
                        continue;
                }
                #endregion 高级设置模式，是否达到启动的时间点？



                // 以此用户进入.
                switch (fl.HisFlowRunWay)
                {
                    case BP.WF.FlowRunWay.SpecEmp: //指定人员按时运行。
                        msg += "<br>触发了:指定人员按时运行.";
                        this.SpecEmp(fl);
                        continue;
                    case BP.WF.FlowRunWay.SelectSQLModel: //按数据集合驱动的模式执行。
                        this.SelectSQLModel(fl);
                        continue;
                    case BP.WF.FlowRunWay.WF_TaskTableInsertModel: //按数据集合驱动的模式执行。
                        this.WF_TaskTableInsertModel(fl);
                        continue;
                    case BP.WF.FlowRunWay.SpecEmpAdv: //指定人员按时运行 高级模式.。
                        msg += "<br>触发了:指定人员按时运行 高级模式.";
                        msg += this.SpecEmpAdv(fl);
                        continue;
                    case BP.WF.FlowRunWay.LetAdminSendSpecEmp: //让admin发送给指定的人员.。
                        msg += "<br>触发了:指定人员按时运行 高级模式.";
                        msg += this.LetAdminSendSpecEmp(fl);
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

            return msg;
        }
        public void SpecEmp(Flow fl)
        {
            string RunObj = fl.RunObj;
            string fk_emp = RunObj.Substring(0, RunObj.IndexOf('@'));

            BP.Port.Emp emp = new BP.Port.Emp();
            emp.UserID = fk_emp;
            if (emp.RetrieveFromDBSources() == 0)
            {
                BP.DA.Log.DebugWriteError("启动自动启动流程错误：发起人(" + fk_emp + ")不存在。");
                return;
            }

            try
            {
                //让 userNo 登录.
                BP.WF.Dev2Interface.Port_Login(emp.UserID);

                //创建空白工作, 发起开始节点.
                Int64 workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fl.No);

                //执行发送.
                SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fl.No, workID);

                //string info_send= BP.WF.Dev2Interface.Node_StartWork(fl.No,);
                BP.DA.Log.DebugWriteInfo("流程:" + fl.No + fl.Name + "的定时任务\t\n -------------- \t\n" + objs.ToMsgOfText());
            }
            catch (Exception ex)
            {
                BP.DA.Log.DebugWriteError("流程:" + fl.No + fl.Name + "自动发起错误:\t\n -------------- \t\n" + ex.Message);
            }
        }
        public string LetAdminSendSpecEmp(Flow fl)
        {
            string empsExp = fl.StartGuidePara1; //获得人员信息。
            if (DataType.IsNullOrEmpty(empsExp) == true)
                return "配置的表达式错误:StartGuidePara1，人员信息不能为空。";

            #region 获得人员集合.
            string[] emps = null;
            if (empsExp.ToUpper().Contains("SELECT") == true)
            {
                string strs = "";
                empsExp = BP.WF.Glo.DealExp(empsExp, null, null);
                DataTable dt = DBAccess.RunSQLReturnTable(empsExp);
                foreach (DataRow dr in dt.Rows)
                {
                    strs += dr[0].ToString() + ";";
                }
                emps = strs.Split(';');
            }
            else
            {
                empsExp = empsExp.Replace("@", ";");
                empsExp = empsExp.Replace(",", ";");
                empsExp = empsExp.Replace("、", ";");
                empsExp = empsExp.Replace("，", ";");
                emps = empsExp.Split(';');
            }
            #endregion 获得人员集合.

            //让admin登录发送.
            BP.WF.Dev2Interface.Port_Login("admin");

            string msg = "";
            try
            {

                //创建空白工作, 发起开始节点.
                Int64 workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fl.No);

                BP.WF.Dev2Interface.Node_SendWork(fl.No, workID, 0, empsExp);

                string info = "流程:【" + fl.No + fl.Name + "】的定时任务 \t\n -------------- \t\n 已经启动，待办：" + empsExp + " , " + workID;
                BP.DA.Log.DebugWriteInfo(info);
            }
            catch (Exception ex)
            {
                BP.DA.Log.DebugWriteError("流程:" + fl.No + fl.Name + "自动发起错误:\t\n -------------- \t\n" + ex.Message);
            }
            return msg;
        }
        /// <summary>
        /// 指定人员按时启动高级模式
        /// </summary>
        /// <param name="fl">流程</param>
        /// <returns></returns>
        public string SpecEmpAdv(Flow fl)
        {
            string empsExp = fl.StartGuidePara1; //获得人员信息。
            if (DataType.IsNullOrEmpty(empsExp) == true)
                return "配置的表达式错误:StartGuidePara1，人员信息不能为空。";

            #region 获得人员集合.
            string[] emps = null;
            if (empsExp.ToUpper().Contains("SELECT") == true)
            {
                string strs = "";
                empsExp = BP.WF.Glo.DealExp(empsExp, null, null);
                DataTable dt = DBAccess.RunSQLReturnTable(empsExp);
                foreach (DataRow dr in dt.Rows)
                {
                    strs += dr[0].ToString() + ";";
                }
                emps = strs.Split(';');
                BP.DA.Log.DebugWriteInfo("strs:" + strs);
            }
            else
            {
                empsExp = empsExp.Replace("@", ";");
                empsExp = empsExp.Replace(",", ";");
                empsExp = empsExp.Replace("、", ";");
                empsExp = empsExp.Replace("，", ";");
                emps = empsExp.Split(';');
            }
            #endregion 获得人员集合.

            string msg = "";
            int idx = 0;
            foreach (string emp in emps)
            {
                if (DataType.IsNullOrEmpty(emp) == true)
                    continue;
                try
                {
                    //让 emp 登录.
                    BP.WF.Dev2Interface.Port_Login(emp);

                    //创建空白工作, 发起开始节点.
                    Int64 workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fl.No);

                    //执行发送.
                    SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fl.No, workID);

                    string info = "流程:" + fl.No + fl.Name + "的定时任务\t\n -------------- \t\n 已经启动，待办：" + emp + " , " + workID;
                    BP.DA.Log.DebugWriteInfo(info);
                    idx++;
                    msg += "<br/>第" + idx + "条:" + info;
                }
                catch (Exception ex)
                {
                    BP.DA.Log.DebugWriteError("流程:" + fl.No + fl.Name + "自动发起错误:\t\n -------------- \t\n" + ex.Message);
                }
            }
            return msg;
        }
        /// <summary>
        /// 触发模式
        /// </summary>
        /// <param name="fl"></param>
        public void WF_TaskTableInsertModel(BP.WF.Flow fl)
        {
        }

        public void SelectSQLModel(BP.WF.Flow fl)
        {
            #region 读取数据.
            BP.Sys.MapExt me = new MapExt();
            me.setMyPK(MapExtXmlList.StartFlow + "_ND" + int.Parse(fl.No) + "01");
            int i = me.RetrieveFromDBSources();
            if (i == 0)
                return;

            if (BP.DA.DataType.IsNullOrEmpty(me.Tag))
            {
                BP.DA.Log.DebugWriteError("没有为流程(" + fl.Name + ")的开始节点设置发起数据,请参考说明书解决.");
                return;
            }


            #endregion 读取数据.

            #region 检查数据源是否正确.
            string errMsg = "";
            // 获取主表数据.
            DataTable dtMain = DBAccess.RunSQLReturnTable(me.Tag);
            if (dtMain.Rows.Count == 0)
            {
                BP.DA.Log.DebugWriteError("流程(" + fl.Name + ")此时无任务.");
                return;
            }

            if (dtMain.Columns.Contains("Starter") == false)
                errMsg += "@配值的主表中没有Starter列.";

            if (dtMain.Columns.Contains("MainPK") == false)
                errMsg += "@配值的主表中没有MainPK列.";

            if (errMsg.Length > 2)
            {
                BP.DA.Log.DebugWriteError("流程(" + fl.Name + ")的开始节点设置发起数据,不完整." + errMsg);

                return;
            }
            #endregion 检查数据源是否正确.

            #region 处理流程发起.
            string frmID = "ND" + int.Parse(fl.No) + "01";

            string err = "";
            int idx = 0;
            foreach (DataRow dr in dtMain.Rows)
            {
                idx++;

                string mainPK = dr["MainPK"].ToString();
                string sql = "SELECT OID FROM " + fl.PTable + " WHERE MainPK='" + mainPK + "'";
                if (DBAccess.RunSQLReturnTable(sql).Rows.Count != 0)
                {
                    continue; /*说明已经调度过了*/
                }

                string starter = dr["Starter"].ToString();
                if (WebUser.No != starter)
                {
                    BP.Web.WebUser.Exit();
                    BP.Port.Emp emp = new BP.Port.Emp();
                    emp.UserID = starter;
                    if (emp.RetrieveFromDBSources() == 0)
                    {
                        BP.DA.Log.DebugWriteInfo("@数据驱动方式发起流程(" + fl.Name + ")设置的发起人员:" + starter + "不存在。");
                        continue;
                    }
                    WebUser.SignInOfGener(emp);
                }

                #region  给值.
                //System.Collections.Hashtable ht = new Hashtable();
                //创建workid.
                Int64 workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fl.No, null, null, starter, null);

                //创建工作.
                GEEntity wk = new GEEntity(frmID, workID);
                // Work wk = fl.NewWork();

                //给主表赋值.
                foreach (DataColumn dc in dtMain.Columns)
                    wk.SetValByKey(dc.ColumnName, dr[dc.ColumnName].ToString());

                // 获取从表数据.
                DataSet ds = new DataSet();
                string[] dtlSQLs = me.Tag1.Split('*');
                foreach (string sqlDtl in dtlSQLs)
                {
                    if (DataType.IsNullOrEmpty(sqlDtl))
                        continue;

                    //替换变量.
                    string mySqlDtl = sqlDtl.Replace("@MainPK", mainPK);

                    string[] tempStrs = mySqlDtl.Split('=');
                    string dtlName = tempStrs[0];
                    DataTable dtlTable = DBAccess.RunSQLReturnTable(mySqlDtl.Replace(dtlName + "=", ""));
                    dtlTable.TableName = dtlName;
                    ds.Tables.Add(dtlTable);
                }
                #endregion  给值.

                #region 取出约定的到达人员.
                int toNodeID = 0;
                try
                {
                    toNodeID = int.Parse(dr["ToNode"].ToString());
                }
                catch
                {
                    /*有可能在4.5以前的版本中没有tonode这个约定.*/
                }
                string toEmps = null;
                try
                {
                    toEmps = dr["ToEmps"].ToString();
                }
                catch
                {
                    /*有可能在4.5以前的版本中没有tonode这个约定.*/
                }
                #endregion 取出约定的到达人员.

                // 处理发送信息.
                //  Node nd =new Node();
                string msg = "";
                try
                {

                    //执行发送到下一个节点.
                    SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fl.No, workID, wk.Row, ds, toNodeID, toEmps);

                    msg = "@自动发起:" + mainPK + "第" + idx + "条:  - " + objs.MsgOfText;
                    BP.DA.Log.DebugWriteInfo(msg);
                }
                catch (Exception ex)
                {
                    BP.DA.Log.DebugWriteError("@" + fl.Name + ",第" + idx + "条,发起人员:" + WebUser.No + "-" + WebUser.Name + "发起时出现错误.\r\n" + ex.Message);
                }
            }
            #endregion 处理流程发起.
        }
    }
}
