using System;
using System.Data;
using System.Collections;
using BP.DA;
using System.Reflection;
using BP.Port;
using BP.Web;
using BP.En;
using BP.Sys;
using BP.WF.Data;
using BP.WF.Template;

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

            #region 自动启动流程
            foreach (BP.WF.Flow fl in fls)
            {
                if (fl.HisFlowRunWay == BP.WF.FlowRunWay.HandWork)
                    continue;

                if (DateTime.Now.ToString("HH:mm") == fl.Tag && fl.HisFlowRunWay == FlowRunWay.SpecEmp)
                    continue;

                /*if (fl.RunObj == null || fl.RunObj == "")
                {
                    string msg = "您设置自动运行流程错误，没有设置流程内容，流程编号：" + fl.No + ",流程名称:" + fl.Name;
                    Log.DebugWriteError(msg);
                    continue;
                }*/

                #region 判断当前时间是否可以运行它。
                if (fl.HisFlowRunWay == FlowRunWay.SpecEmp)
                {
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
                    if (IsCanRun == false )
                        continue;
                }

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
                        emp.UserID = fk_emp;
                        if (emp.RetrieveFromDBSources() == 0)
                        {
                            Log.DebugWriteError("启动自动启动流程错误：发起人(" + fk_emp + ")不存在。");
                            continue;
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
                            Log.DefaultLogWriteLineInfo("流程:" + fl.No + fl.Name + "的定时任务\t\n -------------- \t\n" + objs.ToMsgOfText());

                        }
                        catch (Exception ex)
                        {
                            Log.DebugWriteError("流程:" + fl.No + fl.Name + "自动发起错误:\t\n -------------- \t\n" + ex.Message);
                        }
                        continue;
                    case BP.WF.FlowRunWay.SelectSQLModel: //按数据集合驱动的模式执行。
                        this.SelectSQLModel(fl);
                        continue;
                    case BP.WF.FlowRunWay.WF_TaskTableInsertModel: //按数据集合驱动的模式执行。
                        this.WF_TaskTableInsertModel(fl);
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

            return "调度完成..";
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
            me.MyPK = MapExtXmlList.StartFlow + "_ND" + int.Parse(fl.No) + "01";
            int i = me.RetrieveFromDBSources();
            if (i == 0)
                return;

            if (string.IsNullOrEmpty(me.Tag))
            {
                Log.DefaultLogWriteLineError("没有为流程(" + fl.Name + ")的开始节点设置发起数据,请参考说明书解决.");
                return;
            }


            #endregion 读取数据.

            #region 检查数据源是否正确.
            string errMsg = "";
            // 获取主表数据.
            DataTable dtMain = DBAccess.RunSQLReturnTable(me.Tag);
            if (dtMain.Rows.Count == 0)
            {
                Log.DefaultLogWriteLineError("流程(" + fl.Name + ")此时无任务.");
                return;
            }

            if (dtMain.Columns.Contains("Starter") == false)
                errMsg += "@配值的主表中没有Starter列.";

            if (dtMain.Columns.Contains("MainPK") == false)
                errMsg += "@配值的主表中没有MainPK列.";

            if (errMsg.Length > 2)
            {
                Log.DefaultLogWriteLineError("流程(" + fl.Name + ")的开始节点设置发起数据,不完整." + errMsg);

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
                        Log.DefaultLogWriteLineInfo("@数据驱动方式发起流程(" + fl.Name + ")设置的发起人员:" + starter + "不存在。");
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
                    if (string.IsNullOrEmpty(sqlDtl))
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
                    Log.DefaultLogWriteLineInfo(msg);
                }
                catch (Exception ex)
                {
                    Log.DefaultLogWriteLineWarning("@" + fl.Name + ",第" + idx + "条,发起人员:" + WebUser.No + "-" + WebUser.Name + "发起时出现错误.\r\n" + ex.Message);
                }
            }
            #endregion 处理流程发起.
        }
    }
}
