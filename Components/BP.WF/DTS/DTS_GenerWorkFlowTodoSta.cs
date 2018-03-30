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
    public class DTS_GenerWorkFlowTodoSta : Method
    {
        /// <summary>
        /// 向预期的工作人员发送提醒消息
        /// </summary>
        public DTS_GenerWorkFlowTodoSta()
        {
            this.Title = "更新WF_GenerWorkerFlow.TodoSta状态.";
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
            //系统期望的是，每一个人仅发一条信息.  “您有xx个预警工作，yy个预期工作，请及时处理。”

            DataTable dtEmps = new DataTable();
            dtEmps.Columns.Add("EmpNo", typeof(string));
            dtEmps.Columns.Add("WarningNum", typeof(int));
            dtEmps.Columns.Add("OverTimeNum", typeof(int));

            string timeDT = DateTime.Now.ToString("yyyy-MM-dd");
            string sql = "";

            //查询出预警的工作.
            //sql = " SELECT DISTINCT FK_Emp,COUNT(FK_Emp) as Num , 0 as DBType FROM WF_GenerWorkerlist A WHERE a.DTOfWarning =< '" + timeDT + "' AND a.SDT <= '" + timeDT + "' AND A.IsPass=0  ";
            //sql += "  UNION ";
            //sql += "SELECT DISTINCT FK_Emp,COUNT(FK_Emp) as Num , 1 as DBType FROM WF_GenerWorkerlist A WHERE  a.SDT >'" + timeDT + "' AND A.IsPass=0 ";

            sql = "SELECT * FROM WF_GenerWorkerlist A WHERE a.DTOfWarning >'" + timeDT + "' AND a.SDT <'" + timeDT + "' AND A.IsPass=0 ORDER BY FK_Node,FK_Emp ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            #region 向预警人员发消息.
            // 向预警的人员发消息.
            Node nd = new Node();
            BP.WF.Port.WFEmp emp = new Port.WFEmp();
            foreach (DataRow dr in dt.Rows)
            {
                Int64 workid = Int64.Parse(dr["WorkID"].ToString());
                int fk_node = int.Parse(dr["FK_Node"].ToString());
                string fk_emp = dr["FK_Emp"].ToString();

                if (nd.NodeID != fk_node)
                {
                    nd.NodeID = fk_node;
                    nd.Retrieve();
                }

                if (nd.HisCHWay != CHWay.ByTime)
                    continue; //非按照时效考核.

                if (nd.WAlertRole == CHAlertRole.None)
                    continue;

                //如果仅仅提醒一次.
               // if (nd.WAlertRole == CHAlertRole.OneDayOneTime && isPM == true)
                if (nd.WAlertRole == CHAlertRole.OneDayOneTime && 1==1)
                {

                }
                else
                {
                    continue;
                }

                if (emp.No != fk_emp)
                {
                    emp.No = fk_emp;
                    emp.Retrieve();
                }
            }
            #endregion 向预警人员发消息.

            if (dt.Rows.Count >= 1)
            {
                //更新预警状态.
                sql = "UPDATE WF_GenerWorkFlow  SET TodoSta=1 ";
                sql += " WHERE WorkID IN (SELECT WorkID FROM WF_GenerWorkerlist A WHERE a.DTOfWarning >'" + timeDT + "' AND a.SDT <'" + timeDT + "' AND A.IsPass=0 ) ";
                sql += " AND WF_GenerWorkFlow.WFState!=3 ";
                sql += " AND WF_GenerWorkFlow.TodoSta=0 ";
                int i = BP.DA.DBAccess.RunSQL(sql);
            }

            //更新逾期期状态.
            sql = "UPDATE WF_GenerWorkFlow  SET TodoSta=2 ";
            sql += " WHERE WorkID IN (SELECT WorkID FROM WF_GenerWorkerlist A WHERE a.DTOfWarning >'" + timeDT + "' AND a.SDT <'" + timeDT + "' AND A.IsPass=0 ) ";
            sql += " AND WF_GenerWorkFlow.WFState!=3 ";
            sql += " AND WF_GenerWorkFlow.TodoSta=1 ";
            BP.DA.DBAccess.RunSQL(sql);

            return "时间戳修改成功";
        }
    }
}
