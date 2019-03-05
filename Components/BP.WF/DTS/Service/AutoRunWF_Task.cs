using BP.DA;
using BP.En;
using BP.Port;
using BP.Sys;
using BP.Web.Controls;
using BP.WF.Data;
using BP.WF.Template;
using System;
using System.Collections;
using System.Data;
using System.Reflection;

namespace BP.WF.DTS
{
    /// <summary>
    /// Method 的摘要说明
    /// </summary>
    public class AutoRunWF_Task : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public AutoRunWF_Task()
        {
            this.Title = "自动启动流程，使用扫描WF_Task表的模式.";
            this.Help = "自动启动任务方式的流程, WF_Task";
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
            string info = "";
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
            {
                return "无任务";
            }

            #region 自动启动流程
            foreach (DataRow dr in dt.Rows)
            {
                string mypk = dr["MyPK"].ToString();
                string taskSta = dr["TaskSta"].ToString();
                string paras = dr["Paras"].ToString();
                string starter = dr["Starter"].ToString();
                string fk_flow = dr["FK_Flow"].ToString();

                //获得到达的节点，与接受人。
                string toEmps = dr["ToEmps"].ToString();
                if (DataType.IsNullOrEmpty(toEmps))
                {
                    toEmps = null;
                }

                string toNodeStr = dr["ToNode"].ToString();
                int toNodeID = 0;
                if (DataType.IsNullOrEmpty(toNodeStr) == false)
                {
                    toNodeID = int.Parse(toNodeStr);
                }

                string startDT = dr[TaskAttr.StartDT].ToString();
                if (string.IsNullOrEmpty(startDT) == false)
                {
                    /*如果设置了发起时间,就检查当前时间是否与现在的时间匹配.*/
                    if (DateTime.Now < DateTime.Parse(startDT))
                    {
                        continue;
                    }
                }

                Flow fl = new Flow(fk_flow);

                if (fl.HisFlowAppType == FlowAppType.PRJ)
                {
                    if (paras.Contains("PrjNo=") == false || paras.Contains("PrjName=") == false)
                    {
                        info += "err@工程类的流程，没有PrjNo，PrjName参数:"+fl.Name;
                        continue;
                    }
                }

                Int64 workID = 0;
                try
                {
                    string fTable = "ND" + int.Parse(fl.No + "01").ToString();
                    MapData md = new MapData(fTable);
                    //sql = "";
                    sql = "SELECT * FROM " + md.PTable + " WHERE MainPK='" + mypk + "' AND WFState=1";
                    try
                    {
                        if (DBAccess.RunSQLReturnTable(sql).Rows.Count != 0)
                        {
                            continue;
                        }
                    }
                    catch
                    {
                        info += "开始节点表单表:" + fTable + "没有设置的默认字段MainPK. " + sql; ;
                        continue;
                    }

                    if (BP.Web.WebUser.No != starter)
                    {
                        BP.Web.WebUser.Exit();
                        BP.Port.Emp empadmin = new BP.Port.Emp(starter);
                        BP.Web.WebUser.SignInOfGener(empadmin);
                    }

                    Work wk = fl.NewWork();
                    workID = wk.OID;
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

                    if (fl.HisFlowAppType == FlowAppType.PRJ)
                    {
                        string prjNo = wk.GetValStrByKey("PrjNo");
                        if (DataType.IsNullOrEmpty(prjNo) == true)
                        {
                            info += "err@没有找到工程编号：MainPK" + mypk;
                            continue;
                        }
                    }

                    WorkNode wn = new WorkNode(wk, fl.HisStartNode);

                    string msg = "";

                    if (toNodeID == 0)
                    {
                        msg = wn.NodeSend(null, toEmps).ToMsgOfText();
                    }
                    else
                    {
                        msg = wn.NodeSend(new Node(toNodeID), toEmps).ToMsgOfText();
                    }

                    msg = msg.Replace("'", "~");

                    DBAccess.RunSQL("UPDATE WF_Task SET TaskSta=1,Msg='" + msg + "' WHERE MyPK='" + mypk + "'");
                }
                catch (Exception ex)
                {
                    //删除流程数据
                    if (workID != 0)
                        BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(fk_flow, workID);

                    //如果发送错误。
                    info += ex.Message;
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

            return info;
        }
    }
}
