using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
namespace BP.WF.DTS
{
    /// <summary>
    /// 升级ccflow6 要执行的调度
    /// </summary>
    public class Tool_Repair : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public Tool_Repair()
        {
            this.Title = "修复因显示不出来到达节点下拉框而导致的，发送不下去的bug引起的垃圾数据";
            this.Help = "此bug已经修复掉了,如果仍然出现类似的问题，有可能是其他问题引起的.";
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
                if (BP.Web.WebUser.No == "admin")
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {
            string sql = "SELECT * FROM WF_GENERWORKFLOW WHERE WFState=2 ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            string msg = "";
            foreach (DataRow dr in dt.Rows)
            {
                Int64 workid = Int64.Parse(dr["WorkID"].ToString());
                string todoEmps = dr["TODOEMPS"].ToString();
                string nodeID = dr["FK_NODE"].ToString();

                GenerWorkerLists gwls = new GenerWorkerLists();
                gwls.Retrieve(GenerWorkerListAttr.WorkID, workid, GenerWorkerListAttr.IsPass, 0);
                foreach (GenerWorkerList gwl in gwls)
                {
                    if (todoEmps.Contains(gwl.FK_Emp + ",") == false)
                    {
                        if (nodeID.ToString().EndsWith("01") == true)
                            continue;

                        GenerWorkFlow gwf = new GenerWorkFlow(workid);
                        msg += "<br>@流程:" + gwf.FlowName + "节点:" + gwf.FK_Node + "," + gwf.NodeName + " workid: " + workid + "title:" + gwf.Title + " todoEmps:" + gwf.TodoEmps;
                        msg += "不包含:" + gwl.FK_Emp + "," + gwl.FK_EmpText;

                        gwf.TodoEmps += gwl.FK_Emp + "," + gwl.FK_EmpText + ";";
                        gwf.Update();
                    }
                }
            }
            return msg;
        }
    }
}
