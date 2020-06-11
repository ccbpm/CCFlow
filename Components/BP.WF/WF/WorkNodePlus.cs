using System;
using BP.En;
using BP.DA;
using System.Collections;
using System.Data;
using BP.Port;
using BP.Web;
using BP.Sys;
using BP.WF.Template;
using BP.WF.Data;
namespace BP.WF
{
    /// <summary>
    /// WorkNode的附加类: 2020年06月09号
    /// 1， 因为worknode的类方法太多，为了能够更好的减轻代码逻辑.
    /// 2.  一部分方法要移动到这里来. 
    /// </summary>
    public class WorkNodePlus
    {
        /// <summary>
        /// 处理协作模式下的删除规则
        /// </summary>
        /// <param name="nd">节点</param>
        /// <param name="gwf"></param>
        public static void GenerWorkerListDelRole(Node nd,GenerWorkFlow gwf)
        {
            if (nd.GenerWorkerListDelRole == 0)
                return;

            //按照部门删除,同部门下的人员.
            if (nd.GenerWorkerListDelRole==1)
            {
                //定义本部门的人员.
                string sqlUnion = "";
                sqlUnion += " SELECT No FROM Port_Emp WHERE FK_Dept='" + WebUser.FK_Dept + "' ";
                sqlUnion += " UNION ";
                sqlUnion += " SELECT FK_Dept FROM Port_DeptEmp WHERE FK_Dept='" +WebUser.FK_Dept + "''";

                //获得要删除的人员.
                string sql = " SELECT FK_Emp FROM WF_GenerWorkerlist WHERE ";
                sql += " WorkID="+gwf.WorkID+ " AND FK_Node="+gwf.FK_Node+" AND IsPass=0 ";
                sql += " AND FK_Emp IN ("+ sqlUnion + ")";

                //获得要删除的数据.
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    string empNo = dr[0].ToString();
                    sql = "UPDATE WF_GenerWorkerlist SET IsPass=1 WHERE WorkID="+gwf.WorkID+" AND FK_Node="+gwf.FK_Node+" AND FK_Emp='"+ empNo + "'";
                    DBAccess.RunSQL(sql);
                }
            }

            //按照岗位删除.
            if (nd.GenerWorkerListDelRole == 2)
            {
                NodeStations nss = new NodeStations();
                nss.Retrieve(NodeStationAttr.FK_Node, gwf.FK_Node);
                if (nss.Count == 0)
                    throw new Exception("err@流程配置错误，您设置了待办按照岗位删除的规则,但是在当前节点上，您没有设置岗位。");
                //定义岗位人员
                string station = "SELECT FK_Station FROM Port_DeptEmpStation WHERE FK_Emp='"+WebUser.No+"'";
                station = DBAccess.RunSQLReturnVal(station).ToString();
                string stationEmp = "SELECT FK_Emp FROM Port_DeptEmpStation where FK_Station = " + station + "";
                //获得要删除的人员.
                string sql = " SELECT FK_Emp FROM WF_GenerWorkerlist WHERE ";
                sql += " WorkID=" + gwf.WorkID + " AND FK_Node=" + gwf.FK_Node + " AND IsPass=0 ";
                sql += " AND FK_Emp IN (" + stationEmp + ")";
                //获得要删除的数据.

                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                for(int i=0;i< dt.Rows.Count;i++)
                {
                    string empNo = dt.Rows[i][0].ToString();
                    if (empNo == WebUser.No)
                        continue;
                    sql = "UPDATE WF_GenerWorkerlist SET IsPass=1 WHERE WorkID=" + gwf.WorkID + " AND FK_Node=" + gwf.FK_Node + " AND FK_Emp='" + empNo + "'";
                    DBAccess.RunSQL(sql);
                }
            }

        }

    }
}
