using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using BP.WF;
using BP.Port;
using BP.Web;
using BP.DA;

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
                sql += " WHERE ";
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

            if (nd.GenerWorkerListDelRole == 2)
                throw new Exception("err@尚未解析.");

        }

    }
}
