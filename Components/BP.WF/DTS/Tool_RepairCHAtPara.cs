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
