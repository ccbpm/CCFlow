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
        /// 开始执行数据同步,在流程运动的过程中，
        /// 数据需要同步到不同的表里去.
        /// </summary>
        /// <param name="fl">流程</param>
        /// <param name="gwf">实体</param>
        /// <param name="rpt">实体</param>
        public static void DTSData(Flow fl, GenerWorkFlow gwf,GERpt rpt, Node currNode, bool isStopFlow)
        {
            //判断同步类型.
            if (fl.DTSWay == DataDTSWay.None)
                return;

            bool isActiveSave = false;
            // 判断是否符合流程数据同步条件.
            switch (fl.DTSTime)
            {
                case FlowDTSTime.AllNodeSend:
                    isActiveSave = true;
                    break;
                case FlowDTSTime.SpecNodeSend:
                    if (fl.DTSSpecNodes.Contains(currNode.NodeID.ToString()) == true)
                        isActiveSave = true;
                    break;
                case FlowDTSTime.WhenFlowOver:
                    if (isStopFlow)
                        isActiveSave = true;
                    break;
                default:
                    break;
            }
            if (isActiveSave == false)
                return;

            #region qinfaliang, 编写同步的业务逻辑,执行错误就抛出异常.

            string[] dtsArray = fl.DTSFields.Split('@');

            string[] lcArr = dtsArray[0].Split(',');//取出对应的主表字段
            string[] ywArr = dtsArray[1].Split(',');//取出对应的业务表字段

            string sql = "SELECT " + dtsArray[0] + " FROM " + fl.PTable.ToUpper() + " WHERE OID=" + rpt.OID;
            DataTable lcDt = DBAccess.RunSQLReturnTable(sql);
            if (lcDt.Rows.Count == 0) 
                throw new Exception("没有找到业务表数据.");

            BP.Sys.SFDBSrc src = new BP.Sys.SFDBSrc(fl.DTSDBSrc);
            sql = "SELECT " + dtsArray[1] + " FROM " + fl.DTSBTable.ToUpper();

            DataTable ywDt = src.RunSQLReturnTable(sql);

            string values = "";
            string upVal = "";

            for (int i = 0; i < lcArr.Length; i++)
            {
                switch (src.DBSrcType)
                {
                    case Sys.DBSrcType.Localhost:
                        switch (SystemConfig.AppCenterDBType)
                        {
                            case DBType.MSSQL:
                                break;
                            case DBType.Oracle:
                                if (ywDt.Columns[ywArr[i]].DataType == typeof(DateTime))
                                {
                                    if (!DataType.IsNullOrEmpty(lcDt.Rows[0][lcArr[i].ToString()].ToString()))
                                    {
                                        values += "to_date('" + lcDt.Rows[0][lcArr[i].ToString()] + "','YYYY-MM-DD'),";
                                    }
                                    else
                                    {
                                        values += "'',";
                                    }
                                    continue;
                                }
                                values += "'" + lcDt.Rows[0][lcArr[i].ToString()] + "',";
                                continue;
                                break;
                            case DBType.MySQL:
                                break;
                            case DBType.Informix:
                                break;
                            default:
                                throw new Exception("没有涉及到的连接测试类型...");
                        }
                        break;
                    case Sys.DBSrcType.SQLServer:
                        break;
                    case Sys.DBSrcType.MySQL:
                        break;
                    case Sys.DBSrcType.Oracle:
                        if (ywDt.Columns[ywArr[i]].DataType == typeof(DateTime))
                        {
                            if (!DataType.IsNullOrEmpty(lcDt.Rows[0][lcArr[i].ToString()].ToString()))
                            {
                                values += "to_date('" + lcDt.Rows[0][lcArr[i].ToString()] + "','YYYY-MM-DD'),";
                            }
                            else
                            {
                                values += "'',";
                            }
                            continue;
                        }
                        values += "'" + lcDt.Rows[0][lcArr[i].ToString()] + "',";
                        continue;
                    default:
                        throw new Exception("暂时不支您所使用的数据库类型!");
                }
                values += "'" + lcDt.Rows[0][lcArr[i].ToString()] + "',";
                //获取除主键之外的其他值
                if (i > 0)
                    upVal = upVal + ywArr[i] + "='" + lcDt.Rows[0][lcArr[i].ToString()] + "',";
            }

            values = values.Substring(0, values.Length - 1);
            upVal = upVal.Substring(0, upVal.Length - 1);

            //查询对应的业务表中是否存在这条记录
            sql = "SELECT * FROM " + fl.DTSBTable.ToUpper() + " WHERE " + fl.DTSBTablePK + "='" + lcDt.Rows[0][fl.DTSBTablePK] + "'";
            DataTable dt = src.RunSQLReturnTable(sql);
            //如果存在，执行更新，如果不存在，执行插入
            if (dt.Rows.Count > 0)
                sql = "UPDATE " + fl.DTSBTable.ToUpper() + " SET " + upVal + " WHERE " + fl.DTSBTablePK + "='" + lcDt.Rows[0][fl.DTSBTablePK] + "'";
            else
                sql = "INSERT INTO " + fl.DTSBTable.ToUpper() + "(" + dtsArray[1] + ") VALUES(" + values + ")";

            try
            {
                src.RunSQL(sql);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            #endregion qinfaliang, 编写同步的业务逻辑,执行错误就抛出异常.
            return ;
        }
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
