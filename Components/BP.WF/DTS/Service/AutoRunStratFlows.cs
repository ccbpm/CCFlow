using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
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
                    case BP.WF.FlowRunWay.InsertModel: //按数据集合驱动的模式执行。
                        this.InsertModel(fl);
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
        public void InsertModel(BP.WF.Flow fl)
        {
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
    }
}
