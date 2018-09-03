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
                return "无任务";

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
