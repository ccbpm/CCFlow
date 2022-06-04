using System;
using System.Data;
using BP.DA;
using BP.En;
namespace BP.WF.DTS
{
    /// <summary>
    /// 升级ccflow6 要执行的调度
    /// </summary>
    public class Tool_RepairCHAtPara : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public Tool_RepairCHAtPara()
        {
            this.Title = "修改加签状态，不正确.";
            this.Help = "如果仍然出现，请反馈给开发人员，属于系统错误..";
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
                if (BP.Web.WebUser.No.Equals("admin")==true)
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
            string sql = "SELECT AtPara,WorkID FROM WF_GENERWORKFLOW WHERE WFState !=3 AND atpara like '%@HuiQianTaskSta=1%' ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            string msg = "";
            foreach (DataRow dr in dt.Rows)
            {
                string at = dr["AtPara"].ToString();
                Int64  workid = Int64.Parse( dr["WorkID"].ToString());

                GenerWorkFlow gwf = new GenerWorkFlow(workid);
                GenerWorkerLists gwls = new GenerWorkerLists(workid);
                gwls.Retrieve(GenerWorkerListAttr.WorkID, workid, GenerWorkerListAttr.FK_Node, gwf.FK_Node);
                if (gwls.Count ==1)
                {
                    if (gwf.HuiQianTaskSta == HuiQianTaskSta.HuiQianing)
                    {
                        gwf.HuiQianTaskSta = HuiQianTaskSta.None;
                        gwf.Update();
                    }
                }
            }
            return "执行成功.";
        }

        public string ssdd()
        {
            string sql = "SELECT WORKID FROM WF_GenerWorkFlow A WHERE WFSta <>1 AND WFState =2  AND WorkID Not IN (Select WorkID From WF_GENERWORKERLIST) ORDER BY RDT desc";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            string msg = "";
            foreach (DataRow dr in dt.Rows)
            {
                Int64 workid = Int64.Parse(dr["WorkID"].ToString());
                GenerWorkFlow gwf = new GenerWorkFlow(workid);
                string todoEmps = gwf.TodoEmps;
                // 如果不存在待办，则增加待办
                sql = "SELECT *  From WF_GENERWORKERLIST WHERE WORKID=" + workid + " AND instr('" + todoEmps + "',FK_Emp)>0  AND IsPass=0";
                if (DBAccess.RunSQLReturnCOUNT(sql) > 0)
                    continue;
                if (DataType.IsNullOrEmpty(todoEmps) == true)
                    continue;
                string[] strs = todoEmps.Split(';');
                foreach (string str in strs)
                {
                    if (DataType.IsNullOrEmpty(str))
                        continue;

                    GenerWorkerList gwl = new GenerWorkerList();
                    gwl.WorkID = workid;
                    string[] empStrs = str.Split(',');

                    gwl.FK_Emp = empStrs[0];
                    gwl.FK_Node = gwf.FK_Node;
                    if (empStrs.Length == 2)
                        gwl.FK_EmpText = empStrs[1];
                    gwl.FK_Flow = gwf.FK_Flow;
                    gwl.RDT = gwf.SDTOfNode;
                    gwl.CDT = gwf.SDTOfNode;
                    gwl.IsEnable = true;
                    gwl.IsRead = false;
                    gwl.IsPass = false;
                    gwl.WhoExeIt = 0;
                    gwl.Save();
                }

            }
            return "执行成功.";

        }

        public string ss()
        {
            string sql = "SELECT AtPara,WorkID FROM WF_GENERWORKFLOW WHERE WFState !=3 AND atpara like '%@HuiQianTaskSta=1%' ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            string msg = "";
            foreach (DataRow dr in dt.Rows)
            {
                string at = dr["AtPara"].ToString();
                Int64 workid = Int64.Parse(dr["WorkID"].ToString());

                GenerWorkFlow gwf = new GenerWorkFlow(workid);
                GenerWorkerLists gwls = new GenerWorkerLists(workid);
                gwls.Retrieve(GenerWorkerListAttr.WorkID, workid, GenerWorkerListAttr.FK_Node, gwf.FK_Node);
                if (gwls.Count == 1)
                {
                    if (gwf.HuiQianTaskSta == HuiQianTaskSta.HuiQianing)
                    {
                        gwf.HuiQianTaskSta = HuiQianTaskSta.None;
                        gwf.Update();
                    }
                }



                AtPara ap = new AtPara(at);

                string para = "";
                foreach (string item in ap.HisHT.Keys)
                {
                    if (item.IndexOf("CH") == 0)
                        continue;

                    para += "@" + item + "=" + ap.GetValStrByKey(item);
                }

                if (para == "")
                    continue;

                DBAccess.RunSQL("UPDATE WF_GENERWORKFLOW SET AtPara='" + para + "' where workID=" + workid + " ");
            }

            return "执行成功.";

        }
    }
}
