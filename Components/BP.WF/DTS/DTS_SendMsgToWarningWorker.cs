using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
using BP.Sys;
namespace BP.WF.DTS
{
    /// <summary>
    /// 向预期的工作人员发送提醒消息 的摘要说明
    /// </summary>
    public class DTS_SendMsgToWarningWorker : Method
    {
        /// <summary>
        /// 向预期的工作人员发送提醒消息
        /// </summary>
        public DTS_SendMsgToWarningWorker()
        {
            this.Title = "向预期的工作人员发送提醒消息";
            this.Help = "该方法每天的8点自动执行";
        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
            //this.Warning = "您确定要执行吗？";
            //HisAttrs.AddTBString("P1", null, "原密码", true, false, 0, 10, 10);
            //HisAttrs.AddTBString("P2", null, "新密码", true, false, 0, 10, 10);
            //HisAttrs.AddTBString("P3", null, "确认", true, false, 0, 10, 10);
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

            /*查找一天预警1次的消息记录，并执行推送。*/
            string sql = "SELECT A.WorkID, A.Title, A.FlowName, A.TodoSta, B.FK_Emp, b.FK_EmpText, C.WAlertWay  FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B, WF_Node C  ";
            sql += " WHERE A.WorkID=B.WorkID AND A.FK_Node=C.NodeID AND a.TodoSta=1 AND ( C.WAlertRole=1 OR C.WAlertRole=2 ) ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                CHAlertWay way = (CHAlertWay)int.Parse(dr["WAlertWay"].ToString()); //提醒方式.
                Int64 workid = Int64.Parse(dr["WorkID"].ToString());
                string title = dr["Title"].ToString();
                string flowName = dr["FlowName"].ToString();
                string empNo = dr["FK_Emp"].ToString();
                string empName = dr["FK_EmpText"].ToString();

                BP.WF.Port.WFEmp emp = new Port.WFEmp(empNo);

                if (way == CHAlertWay.ByEmail)
                {
                    string titleMail = "";
                    string docMail = "";
                    //  BP.WF.Dev2Interface.Port_SendEmail(emp.Email, titleMail, "");
                }

                if (way == CHAlertWay.BySMS)
                {
                    string titleMail = "";
                    string docMail = "";
                    //BP.WF.Dev2Interface.Port_SendMsg(emp.Email, titleMail, "");
                }
            }
            return "执行成功...";
        }
    }
}
