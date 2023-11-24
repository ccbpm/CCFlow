using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF.Template;

namespace BP.WF.DTS
{
    /// <summary>
    /// Method 的摘要说明
    /// </summary>
    public class AddAuthStation : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public AddAuthStation()
        {
            this.Title = "增加授权角色";
            this.Help = "1. 解决一个流程执行完成后，那些授权角色参与了该流程.";
            this.Help += "\t\n 2. 在WF_GenerWorkFlow 的Emps的字段上增加  @部门编号+下划线+角色编号; ";
            this.Help += "\t\n 3. 解决中科软的人员离职后的工作交接后，按照授权角色查询已经办理过的流量问题.";

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
            GenerWorkFlows ens = new GenerWorkFlows();
            ens.Retrieve("", "");

            //查询出来，没有做过同步的 并且流程已经完成的 流程实例.
            string sql = "SELECT WorkID FROM WF_GenerWorkFlow WHERE WFState=3 AND  Emps  NOT LIKE '%@Over' ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            //遍历这些实例.
            foreach (DataRow dr in dt.Rows)
            {
                Int64 workid = Int64.Parse(dr[0].ToString());

                GenerWorkFlow gwf = new GenerWorkFlow(workid);

                //查询出来当前流程的Track.
                sql = "SELECT * FROM ND" + int.Parse(gwf.FlowNo) + "Track WHERE WorkID=" + workid + " ORDER BY RDT ";
                DataTable tarck = DBAccess.RunSQLReturnTable(sql);

                //查询出来节点.
                Nodes nds = new Nodes(gwf.FlowNo);

                //遍历节点.
                foreach (Node nd in nds)
                {
                    if (this.ItIsHaveStation(nd) == false)
                        continue;

                    //求节点与角色的集合.
                    NodeStations ndstations = new NodeStations(nd.NodeID);
                    if (ndstations.Count == 0)
                        continue;

                    //找到处理当前工作的人员集合.
                    sql = "SELECT EmpFrom FROM ND" + int.Parse(gwf.FlowNo) + "Track WHERE WorkID=" + workid + " AND NDFrom=" + nd.NodeID + " ORDER BY RDT ";
                    DataTable dtTarck = DBAccess.RunSQLReturnTable(sql);

                    foreach (DataRow drTrack in dtTarck.Rows)
                    {
                        string empNo = drTrack[0].ToString();

                        //获得人员的集合，与节点绑定的集合.
                        sql = "SELECT A.FK_Dept, A.FK_Station FROM Port_DeptEmpStation A, WF_NodeStation B WHERE  ";
                        sql += " WHERE A.FK_Station=B.FK_Station ";
                        sql += " AND A.FK_Emp= '" + empNo + "'";
                        sql += " AND B.FK_Node= " + nd.NodeID;
                        DataTable dtDeptStatio = DBAccess.RunSQLReturnTable(sql);

                        foreach (DataRow mydtDeptStatio in dtDeptStatio.Rows)
                        {
                            string deptNo = mydtDeptStatio["FK_Dept"].ToString();
                            string stationNo = mydtDeptStatio["FK_Station"].ToString();

                            string str = "@" + deptNo + "_" + stationNo + ";";
                            if (gwf.TodoEmps.Contains(str) == false)
                                gwf.TodoEmps += str;
                        }
                    }
                }

                //设置同步标记执行更新.
                gwf.TodoEmps += "@Over";
                gwf.Update();

            }
            return "调度完成..";
        }

        public bool ItIsHaveStation(Node nd)
        {
            if (nd.HisDeliveryWay == DeliveryWay.ByDeptAndStation)
                return true;
            if (nd.HisDeliveryWay == DeliveryWay.ByStation)
                return true;

            return false;
        }

    }
}
