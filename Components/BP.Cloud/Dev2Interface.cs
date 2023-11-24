using BP.DA;
using BP.Sys;
using BP.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Threading.Tasks;
using BP.En30.Utility.Web;
using BP.Difference;
using BP.WF;

namespace BP.Cloud
{
    /// <summary>
    /// 云的公共类
    /// </summary>
    public class Dev2Interface
    {
        /// <summary>
        /// 验证部门是否可以删除
        /// </summary>
        /// <param name="orgNo"></param>
        /// <param name="deptNo"></param>
        /// <returns></returns>
        /// 
        public static BaseResponse<string> CheckDeptIsUsed(string deptNo)
        {
            BaseResponse<string> res = new BaseResponse<string>();
            try
            {
                if (deptNo == WebUser.OrgNo)
                {
                    res.code = ResponseCode.fail;
                    res.msg = "SaaS组织不允许删除";

                    return res;
                }

                int count = DBAccess.RunSQLReturnValInt("select count(*) num from port_dept where parentno='" + deptNo + "'");
                if (count > 0)
                {
                    res.code = ResponseCode.fail;
                    res.msg = "请先删除子部门";

                    return res;
                }
                count = DBAccess.RunSQLReturnValInt("select count(*) num from port_emp where fk_dept='" + deptNo + "'");
                if (count > 0)
                {
                    res.code = ResponseCode.fail;
                    res.msg = "请先删除部门人员";

                    return res;
                }

                count = DBAccess.RunSQLReturnValInt("select count(*) num from Port_DeptEmpStation where fk_dept='" + deptNo + "'");
                if (count > 0)
                {
                    res.code = ResponseCode.fail;
                    res.msg = "请先删除部门岗位";

                    return res;
                }

                //以前没有增加对隶属部门的判断
                count = DBAccess.RunSQLReturnValInt("select count(*) num from Port_DeptEmp where fk_dept='" + deptNo + "'");
                if (count > 0)
                {
                    res.code = ResponseCode.fail;
                    res.msg = "请先删除子部门人员";

                    return res;
                }
            }
            catch (Exception ex)
            {
                res.code = ResponseCode.fail;
                res.msg = ex.Message;

                Log.DebugWriteError(res.msg);
            }

            return res;
        }
        /// <summary>
        /// 验证岗位是否已被占用
        /// </summary>
        /// <param name="orgNo"></param>
        /// <param name="stationNo"></param>
        /// <returns></returns>
        public static BaseResponse<string> CheckStationIsUsed(string orgNo, string stationNo)
        {
            BaseResponse<string> res = new BaseResponse<string>();
            try
            {
                string sql = "select count(*) num from Port_DeptEmpStation where orgno='" + orgNo + "' and FK_Station='" + stationNo + "'";
                int count = DBAccess.RunSQLReturnValInt(sql);
                if (count > 0)
                {
                    res.code = ResponseCode.fail;
                    res.msg = "岗位已被使用";
                }
            }
            catch (Exception ex)
            {
                res.code = ResponseCode.fail;
                res.msg = ex.Message;

                Log.DebugWriteError(res.msg);
            }

            return res;
        }
        /// <summary>
        /// 验证岗位类型是否已被占用
        /// </summary>
        /// <param name="orgNo"></param>
        /// <param name="stationNo"></param>
        /// <returns></returns>
        public static BaseResponse<string> CheckStationTypeIsUsed(string orgNo, string stationTypeNo)
        {
            BaseResponse<string> res = new BaseResponse<string>();
            try
            {
                string sql = "select count(*) num from Port_Station where orgno='" + orgNo + "' and FK_StationType='" + stationTypeNo + "'";
                int count = DBAccess.RunSQLReturnValInt(sql);
                if (count > 0)
                {
                    res.code = ResponseCode.fail;
                    res.msg = "岗位类型已被使用";
                }
            }
            catch (Exception ex)
            {
                res.code = ResponseCode.fail;
                res.msg = ex.Message;

                Log.DebugWriteError(res.msg);
            }

            return res;
        }
        public static BaseResponse<BP.Cloud.Emp> CheckTokenCode(string token)
        {
           
            BaseResponse<BP.Cloud.Emp> res = new BaseResponse<BP.Cloud.Emp>();

            try
            {
                BP.Cloud.Emp emp = new BP.Cloud.Emp();
                int count = emp.Retrieve(BP.Cloud.EmpAttr.SID, token);
                if (count != 1)
                {
                    res.code = ResponseCode.fail;
                    res.msg = "用户不存在";//或者组织数据异常token重复的问题
                    return res;
                }

                //执行登录，退出时才改变SID
                BP.Cloud.Dev2Interface.Port_Login(emp.UserID, emp.OrgNo, false);

                res.code = ResponseCode.success;
                res.data = emp;
            }
            catch (Exception ex)
            {
                res.code = ResponseCode.fail;
                res.msg = "用户不存在或者已被禁用";

                Log.DebugWriteError(res.msg);
            }

            return res;
        }
        /// <summary>
        /// 多组织登录接口.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="orgNo"></param>
        public static string Port_Login(string userID, string orgNo, bool isChangeSID)
        {
            BP.Cloud.Emp emp = new BP.Cloud.Emp();
            int i = emp.Retrieve("UserID", userID, "OrgNo", orgNo);
            if (i == 0)
                throw new Exception("err@用户名[" + userID + "],OrgNo[" + orgNo + "]不存在.");

            //调用登录.
            return Port_Login(emp, isChangeSID);
        }
        /// <summary>
        /// 登录
        /// </summary>
        public static string Port_Login(BP.Cloud.Emp emp, bool isChangeSID)
        {
            // cookie操作，为适应不同平台，统一使用HttpContextHelper
            Dictionary<string, string> cookieValues = new Dictionary<string, string>();

            cookieValues.Add("No", emp.UserID);
            cookieValues.Add("Name", HttpUtility.UrlEncode(emp.Name));

            cookieValues.Add("FK_Dept", emp.FK_Dept);
            cookieValues.Add("FK_DeptName", HttpUtility.UrlEncode(emp.FK_DeptText));

            cookieValues.Add("OrgNo", emp.OrgNo);
            cookieValues.Add("OrgName", emp.OrgName);

            var res = WebUser.CheckEmpSID(emp.No);
            string sid = null;

            if (res.code != ResponseCode.success)
            {
                isChangeSID = true;
                sid = res.data;
            }

            if (isChangeSID == true)
            {
                string sql = "UPDATE Port_Emp SET SID='" + sid + "' WHERE No='" + emp.No + "' AND OrgNo = '" + emp.OrgNo + "'";
                DBAccess.RunSQL(sql);

                /*sql = "UPDATE WF_Emp SET Token='" + sid + "' WHERE No='" + emp.No + "'";
                DBAccess.RunSQL(sql);*/
                cookieValues.Add("Token", sid);
                cookieValues.Add("SID", sid);
            }
            else
            {
                sid = DBAccess.RunSQLReturnString("SELECT SID FROM Port_Emp WHERE No='" + emp.No + "' AND OrgNo = '" + emp.OrgNo + "'");
                if (string.IsNullOrEmpty(sid))
                {
                    sid = BP.Tools.SecurityUnit.EncryptByAes(emp.No + DBAccess.GenerGUID()); ;
                    string sql = "UPDATE Port_Emp SET SID='" + sid + "' WHERE No='" + emp.No + "' AND OrgNo = '" + emp.OrgNo + "'";
                    DBAccess.RunSQL(sql);
                    /*sql = "UPDATE WF_Emp SET Token='" + sid + "' WHERE No='" + emp.No + "'";
                  DBAccess.RunSQL(sql);*/
                }


                cookieValues.Add("Token", sid);
                cookieValues.Add("SID", sid);
            }

            cookieValues.Add("Tel", emp.Tel);
            cookieValues.Add("Lang", "CH");

            HttpContextHelper.ResponseCookieAdd(cookieValues, null, "CCS");

            //给 session 赋值.
            if (HttpContextHelper.Current.Session != null)
            {
                HttpContextHelper.Current.Session["No"] = emp.UserID;
                HttpContextHelper.Current.Session["Name"] = emp.Name;
                HttpContextHelper.Current.Session["FK_Dept"] = emp.FK_Dept;
                HttpContextHelper.Current.Session["FK_DeptText"] = emp.FK_DeptText;
                HttpContextHelper.Current.Session["OrgNo"] = emp.OrgNo;
                HttpContextHelper.Current.Session["OrgName"] = emp.OrgName;
            }

            //if (isChangeSID == true)
            //{
            //    HttpContextHelper.Current.Session["SID"] = sid;
            //    HttpContextHelper.Current.Session["Token"] = sid;
            //}

            return sid;

            //HttpContextHelper.SessionSet("No", emp.UserID);
            //HttpContextHelper.SessionSet("Name", emp.Name);
            //HttpContextHelper.SessionSet("FK_Dept", emp.FK_Dept);
            //HttpContextHelper.SessionSet("FK_DeptText", emp.FK_DeptText);
            //HttpContextHelper.SessionSet("OrgNo", emp.OrgNo);
            //HttpContextHelper.SessionSet("OrgName", emp.OrgName);

        }
        public static DataTable DB_StarFlows(string userNo, string domain = null)
        {
            string sql = "SELECT A.ICON, A.No,A.Name,a.IsBatchStart,";
            sql += " a.FK_FlowSort,B.Name AS FK_FlowSortText,B.Domain,A.IsStartInMobile, A.Idx,";
            sql += " a.WorkModel,"; // 0=内部流程1=外部流程2=实体台账3=表单.
            sql += " a.PTable as FrmID "; // 表单ID,为实体台账的时候存储的表单ID.
            sql += " FROM WF_Flow A, WF_FlowSort B  ";
            sql += " WHERE   A.IsCanStart=1 AND A.FK_FlowSort=B.No  AND A.OrgNo='" + WebUser.OrgNo + "' ";
            sql += " ORDER BY A.Idx ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return dt;
        }
        /// <summary>
        /// 获取未完成的流程(也称为在途流程:我参与的但是此流程未完成)
        /// </summary>
        /// <returns>返回从数据视图WF_GenerWorkflow查询出来的数据.</returns>
        public static DataTable DB_GenerRuning(string userNo = null, bool isContainFuture = false, string domain = null)
        {
            if (userNo == null)
                userNo = WebUser.No;

            DataTable dt = DB_GenerRuning(userNo, null, false, null, isContainFuture);

            /*暂时屏蔽type的拼接，拼接后转json会报错 于庆海修改*/
            /*dt.Columns.Add("Type");
            foreach (DataRow row in dt.Rows)
            {
                row["Type"] = "RUNNING";
            }*/

            dt.DefaultView.Sort = "RDT DESC";
            return dt.DefaultView.ToTable();
        }

        /// <summary>
        /// 获取未完成的流程(也称为在途流程:我参与的但是此流程未完成)
        /// 该接口为在途菜单提供数据,在在途工作中，可以执行撤销发送。
        /// </summary>
        /// <param name="userNo">操作员</param>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="isMyStarter">是否仅仅查询我发起的在途流程</param>
        /// <returns>返回从数据视图WF_GenerWorkflow查询出来的数据.</returns>
        public static DataTable DB_GenerRuning(string userNo, string fk_flow,
            bool isMyStarter = false, string domain = null, bool isContainFuture = false)
        {
            string dbStr = SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();

            string domainSQL = "";
            if (domain == null)
                domainSQL = " AND Domain='" + domain + "' ";
            //获取用户当前所在的节点
            String currNode = "";
            switch (DBAccess.AppCenterDBType)
            {
                case DBType.Oracle:
                    currNode = "(SELECT FK_Node FROM (SELECT FK_Node FROM WF_GenerWorkerlist WHERE FK_Emp='" + WebUser.No + "' AND  OrgNo='" + WebUser.OrgNo + "'  Order by RDT DESC ) WHERE RowNum=1)";
                    break;
                case DBType.MySQL:
                case DBType.PostgreSQL:
                    currNode = "(SELECT  FK_Node FROM WF_GenerWorkerlist WHERE FK_Emp='" + WebUser.No + "' AND  OrgNo='" + WebUser.OrgNo + "' Order by RDT DESC LIMIT 1)";
                    break;
                case DBType.MSSQL:
                    currNode = "(SELECT TOP 1 FK_Node FROM WF_GenerWorkerlist WHERE FK_Emp='" + WebUser.No + "' AND  OrgNo='" + WebUser.OrgNo + "' Order by RDT DESC)";
                    break;
                default:
                    break;
            }


            //授权模式.
            string sql = "";
            string futureSQL = "";
            if (isContainFuture == true)
            {
                switch (DBAccess.AppCenterDBType)
                {
                    case DBType.MySQL:
                        futureSQL = " UNION SELECT A.WorkID,A.StarterName,A.Title,A.DeptName,D.Name AS NodeName,A.RDT,B.FK_Node,A.FK_Flow,A.FID,A.FlowName,C.EmpName AS TodoEmps," + currNode + " AS CurrNode ,1 AS RunType FROM WF_GenerWorkFlow A, WF_SelectAccper B,"
                                + "(SELECT GROUP_CONCAT(B.EmpName SEPARATOR ';') AS EmpName, B.WorkID,B.FK_Node FROM WF_GenerWorkFlow A, WF_SelectAccper B WHERE A.WorkID = B.WorkID  group By B.FK_Node) C,WF_Node D"
                                + " WHERE A.WorkID = B.WorkID AND B.WorkID = C.WorkID AND B.FK_Node = C.FK_Node AND A.FK_Node = D.NodeID AND B.FK_Emp = '" + WebUser.No + "'"
                                + " AND B.FK_Node Not in(Select DISTINCT FK_Node From WF_GenerWorkerlist G where G.WorkID = B.WorkID)AND A.WFState != 3";
                        break;
                    case DBType.MSSQL:
                        futureSQL = " UNION SELECT A.WorkID,A.StarterName,A.Title,A.DeptName,D.Name AS NodeName,A.RDT,B.FK_Node,A.FK_Flow,A.FID,A.FlowName,C.EmpName AS TodoEmps ," + currNode + " AS CurrNode ,1 AS RunType FROM WF_GenerWorkFlow A, WF_SelectAccper B,"
                                + "(SELECT EmpName=STUFF((Select ';'+FK_Emp+','+EmpName From WF_SelectAccper t Where t.FK_Node=B.FK_Node FOR xml path('')) , 1 , 1 , '') , B.WorkID,B.FK_Node FROM WF_GenerWorkFlow A, WF_SelectAccper B WHERE A.WorkID = B.WorkID  group By B.FK_Node,B.WorkID) C,WF_Node D"
                                + " WHERE A.WorkID = B.WorkID AND B.WorkID = C.WorkID AND B.FK_Node = C.FK_Node AND A.FK_Node = D.NodeID AND B.FK_Emp = '" + WebUser.No + "'"
                                + " AND B.FK_Node Not in(Select DISTINCT FK_Node From WF_GenerWorkerlist G where G.WorkID = B.WorkID)AND A.WFState != 3";
                        break;
                    default:
                        break;

                }
            }




            //非授权模式，

            if (DataType.IsNullOrEmpty(fk_flow))
            {
                if (isMyStarter == true)
                {
                    sql = "SELECT DISTINCT a.WorkID,a.StarterName,a.Title,a.DeptName,a.NodeName,a.RDT,a.FK_Node,a.FK_Flow,a.FID ,a.FlowName,a.TodoEmps," + currNode + " AS CurrNode,0 AS RunType FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.TodoEmps  not like '%" + WebUser.No + ",%' AND A.WorkID=B.WorkID AND B.FK_Emp=" + dbStr + "FK_Emp AND B.IsEnable=1 AND  (B.IsPass=1 or B.IsPass < -1) AND  A.Starter=" + dbStr + "Starter  AND  OrgNo='" + WebUser.OrgNo + "'";
                    if (isContainFuture == true)
                    {
                        sql += futureSQL;
                    }
                    ps.SQL = sql;
                    ps.Add("FK_Emp", userNo);
                    ps.Add("Starter", userNo);
                }
                else
                {
                    sql = "SELECT DISTINCT a.WorkID,a.StarterName,a.Title,a.DeptName,a.NodeName,a.RDT,a.FK_Node,a.FK_Flow,a.FID ,a.FlowName,a.TodoEmps ," + currNode + " AS CurrNode,0 AS RunType FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.TodoEmps  not like '%" + WebUser.No + ",%' AND A.WorkID=B.WorkID AND B.FK_Emp=" + dbStr + "FK_Emp AND B.IsEnable=1 AND  (B.IsPass=1 or B.IsPass < -1)  AND  OrgNo='" + WebUser.OrgNo + "'";
                    if (isContainFuture == true)
                    {
                        sql += futureSQL;
                    }
                    ps.SQL = sql;
                    ps.Add("FK_Emp", userNo);
                }
            }
            else
            {
                if (isMyStarter == true)
                {
                    sql = "SELECT DISTINCT a.WorkID,a.StarterName,a.Title,a.DeptName,a.NodeName,a.RDT,a.FK_Node,a.FK_Flow,a.FID ,a.FlowName,a.TodoEmps ," + currNode + " AS CurrNode,0 AS RunType FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.TodoEmps  not like '%" + WebUser.No + ",%' AND A.FK_Flow=" + dbStr + "FK_Flow  AND A.WorkID=B.WorkID AND B.FK_Emp=" + dbStr + "FK_Emp AND B.IsEnable=1 AND (B.IsPass=1 or B.IsPass < -1 ) AND  A.Starter=" + dbStr + "Starter  AND  OrgNo='" + WebUser.OrgNo + "'";
                    if (isContainFuture == true)
                    {
                        sql += futureSQL;
                    }
                    ps.SQL = sql;
                    ps.Add("FK_Flow", fk_flow);
                    ps.Add("FK_Emp", userNo);
                    ps.Add("Starter", userNo);
                }
                else
                {
                    sql = "SELECT DISTINCT a.WorkID,a.StarterName,a.Title,a.DeptName,a.NodeName,a.RDT,a.FK_Node,a.FK_Flow,a.FID ,a.FlowName,a.TodoEmps ," + currNode + " AS CurrNode,0 AS RunType  FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.TodoEmps  not like '%" + WebUser.No + ",%' AND A.FK_Flow=" + dbStr + "FK_Flow  AND A.WorkID=B.WorkID AND B.FK_Emp=" + dbStr + "FK_Emp AND B.IsEnable=1 AND (B.IsPass=1 or B.IsPass < -1 )  AND  OrgNo='" + WebUser.OrgNo + "'";
                    if (isContainFuture == true)
                    {
                        sql += futureSQL;
                    }
                    ps.SQL = sql;
                    ps.Add("FK_Flow", fk_flow);
                    ps.Add("FK_Emp", userNo);
                }
            }

            //获得sql.
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            if (SystemConfig.AppCenterDBType == DBType.Oracle
                || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["STARTERNAME"].ColumnName = "StarterName";
                dt.Columns["TITLE"].ColumnName = "Title";
                dt.Columns["NODENAME"].ColumnName = "NodeName";
                dt.Columns["RDT"].ColumnName = "RDT";
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FID"].ColumnName = "FID";
                dt.Columns["FK_NODE"].ColumnName = "FK_Node";
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";
                dt.Columns["DEPTNAME"].ColumnName = "DeptName";
                dt.Columns["TODOEMPS"].ColumnName = "TodoEmps";
                dt.Columns["CURRNODE"].ColumnName = "CurrNode";
                dt.Columns["RUNTYPE"].ColumnName = "RunType";
            }
            return dt;
        }

        public static DataTable DB_GenerRuningNotMyStart(string userNo)
        {
            string dbStr = SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();

            //获取用户当前所在的节点
            String currNode = "";
            switch (DBAccess.AppCenterDBType)
            {
                case DBType.Oracle:
                    currNode = "(SELECT FK_Node FROM (SELECT FK_Node FROM WF_GenerWorkerlist WHERE FK_Emp='" + WebUser.No + "' AND  OrgNo='" + WebUser.OrgNo + "'  Order by RDT DESC ) WHERE RowNum=1)";
                    break;
                case DBType.MySQL:
                case DBType.PostgreSQL:
                    currNode = "(SELECT  FK_Node FROM WF_GenerWorkerlist WHERE FK_Emp='" + WebUser.No + "' AND  OrgNo='" + WebUser.OrgNo + "' Order by RDT DESC LIMIT 1)";
                    break;
                case DBType.MSSQL:
                    currNode = "(SELECT TOP 1 FK_Node FROM WF_GenerWorkerlist WHERE FK_Emp='" + WebUser.No + "' AND  OrgNo='" + WebUser.OrgNo + "' Order by RDT DESC)";
                    break;
                default:
                    break;
            }
            string sql = "SELECT DISTINCT a.WorkID,a.StarterName,a.Title,a.DeptName,a.NodeName,a.RDT,a.FK_Node,a.FK_Flow,a.FID ,a.FlowName,a.TodoEmps ," + currNode + " AS CurrNode,0 AS RunType FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.TodoEmps  not like '%" + WebUser.No + ",%' AND A.WorkID=B.WorkID AND B.FK_Emp=" + dbStr + "FK_Emp AND B.IsEnable=1 AND  (B.IsPass=1 or B.IsPass < -1)  AND  OrgNo='" + WebUser.OrgNo + "' AND A.Starter!=" + dbStr + "Starter";

            ps.SQL = sql;
            ps.Add("FK_Emp", userNo);
            ps.Add("Starter", userNo);

            //获得sql.
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            if (SystemConfig.AppCenterDBType == DBType.Oracle
                || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["STARTERNAME"].ColumnName = "StarterName";
                dt.Columns["TITLE"].ColumnName = "Title";
                dt.Columns["NODENAME"].ColumnName = "NodeName";
                dt.Columns["RDT"].ColumnName = "RDT";
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FID"].ColumnName = "FID";
                dt.Columns["FK_NODE"].ColumnName = "FK_Node";
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";
                dt.Columns["DEPTNAME"].ColumnName = "DeptName";
                dt.Columns["TODOEMPS"].ColumnName = "TodoEmps";
                dt.Columns["CURRNODE"].ColumnName = "CurrNode";
                dt.Columns["RUNTYPE"].ColumnName = "RunType";
            }
            return dt;
        }

        /// <summary>
        /// 获取某一个人已完成的流程
        /// </summary>
        /// <param name="userNo">用户编码</param>
        /// <param name="isMyStart">是否是用户发起的</param>
        /// <returns></returns>
        public static DataTable DB_FlowCompleteNotMyStart(string userNo)
        {
            Paras ps = new Paras();
            string dbstr = SystemConfig.AppCenterDBVarStr;
            ps.SQL = "SELECT 'RUNNING' AS Type, T.* FROM WF_GenerWorkFlow T WHERE T.Starter!=" + dbstr + "Starter AND (T.Emps LIKE '%@" + userNo + "@%' OR  T.Emps LIKE '%@" + userNo + ",%') AND T.FID=0 AND T.WFState=" + (int)WFState.Complete + " ORDER BY  RDT DESC";
            ps.Add("Starter", userNo);
            DataTable dt = DBAccess.RunSQLReturnTable(ps);

            //需要翻译.
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dt.Columns["TYPE"].ColumnName = "Type";
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["FK_FLOWSORT"].ColumnName = "FK_FlowSort";
                dt.Columns["SYSTYPE"].ColumnName = "SysType";
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";
                dt.Columns["TITLE"].ColumnName = "Title";

                dt.Columns["WFSTA"].ColumnName = "WFSta";
                dt.Columns["WFSTATE"].ColumnName = "WFState";
                dt.Columns["STARTER"].ColumnName = "Starter";
                dt.Columns["STARTERNAME"].ColumnName = "StarterName";
                dt.Columns["SENDER"].ColumnName = "Sender";
                dt.Columns["FK_NODE"].ColumnName = "FK_Node";
                dt.Columns["NODENAME"].ColumnName = "NodeName";

                dt.Columns["FK_DEPT"].ColumnName = "FK_Dept";
                dt.Columns["DEPTNAME"].ColumnName = "DeptName";
                dt.Columns["SDTOFNODE"].ColumnName = "SDTOfNode";
                dt.Columns["SDTOFFLOW"].ColumnName = "SDTOfFlow";
                dt.Columns["PFLOWNO"].ColumnName = "PflowNo";
                dt.Columns["PWORKID"].ColumnName = "PWorkID";

                dt.Columns["PNODEID"].ColumnName = "PNodeID";
                dt.Columns["PEMP"].ColumnName = "PEmp";
                dt.Columns["GUESTNO"].ColumnName = "GuestNo";
                dt.Columns["GUESTNAME"].ColumnName = "GuestName";
                dt.Columns["BILLNO"].ColumnName = "BillNo";
                dt.Columns["FLOWNOTE"].ColumnName = "FlowNote";

                dt.Columns["TODOEMPS"].ColumnName = "TodoEmps";
                dt.Columns["TODOEMPSNUM"].ColumnName = "TodoEmpsNum";
                dt.Columns["TASKSTA"].ColumnName = "TaskSta";
                dt.Columns["ATPARA"].ColumnName = "AtPara";
                dt.Columns["EMPS"].ColumnName = "Emps";
                dt.Columns["DOMAIN"].ColumnName = "Domain";
                dt.Columns["SENDDT"].ColumnName = "SendDT";
                dt.Columns["WEEKNUM"].ColumnName = "WeekNum";
            }
            return dt;
        }
        /// <summary>
        /// 待办工作数量
        /// </summary>
        public static int Todolist_EmpWorks
        {
            get
            {
                string sql = "SELECT WorkID FROM WF_EmpWorks WHERE FK_Emp='" + BP.Web.WebUser.No + "' AND OrgNo='" + BP.Web.WebUser.OrgNo + "'";
                //获得授权信息.
                Auths aths = new Auths();
                aths.Retrieve(AuthAttr.AutherToEmpNo, WebUser.No, AuthAttr.OrgNo, WebUser.OrgNo);

                foreach (Auth ath in aths)
                {
                    if (ath.AuthType == AuthorWay.None || ath.Auther == WebUser.No)
                        continue;

                    string todata = ath.TakeBackDT.Replace("-", "");
                    if (DataType.IsNullOrEmpty(ath.TakeBackDT) == false)
                    {
                        int mydt = int.Parse(todata);
                        int nodt = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                        if (mydt < nodt)
                            continue;
                    }
                    sql += " UNION ";

                    if (ath.AuthType == AuthorWay.SpecFlows)
                        sql += "SELECT WorkID FROM WF_EmpWorks WHERE  FK_Emp='" + ath.Auther + "' AND FK_Flow='" + ath.FlowNo + "' AND OrgNo='" + BP.Web.WebUser.OrgNo + "'";
                    else
                        sql += "SELECT WorkID FROM WF_EmpWorks WHERE  FK_Emp='" + ath.Auther + "' AND OrgNo='" + BP.Web.WebUser.OrgNo + "'";

                }


                string mysql = "SELECT COUNT(*) From (" + sql + ") A";
                return DBAccess.RunSQLReturnValInt(mysql);
            }
        }

        /// <summary>
        /// 抄送数量
        /// </summary>
        public static int Todolist_CCWorks
        {
            get
            {
                Paras ps = new Paras();
                ps.SQL = "SELECT count(A.MyPK) as Num FROM WF_CCList A ,WF_GenerWorkFlow B WHERE A.CCTo=" + SystemConfig.AppCenterDBVarStr + "FK_Emp AND  B.WorkID=A.WorkID AND A.OrgNo=" + SystemConfig.AppCenterDBVarStr + "OrgNo";
                ps.Add("FK_Emp", BP.Web.WebUser.No);
                ps.Add("OrgNo", BP.Web.WebUser.OrgNo);
                return DBAccess.RunSQLReturnValInt(ps, 0);
            }
        }

        /// <summary>
        /// 退回给当前用户的数量
        /// </summary>
        public static int Todolist_ReturnNum
        {

            get
            {
                string sql = "SELECT WorkID FROM WF_EmpWorks WHERE WFState=5 AND FK_Emp='" + BP.Web.WebUser.No + "' AND OrgNo='" + BP.Web.WebUser.OrgNo + "'";
                //获得授权信息.
                Auths aths = new Auths();
                aths.Retrieve(AuthAttr.AutherToEmpNo, WebUser.No, AuthAttr.OrgNo, WebUser.OrgNo);

                foreach (Auth ath in aths)
                {
                    if (ath.AuthType == AuthorWay.None || ath.Auther == WebUser.No)
                        continue;

                    string todata = ath.TakeBackDT.Replace("-", "");
                    if (DataType.IsNullOrEmpty(ath.TakeBackDT) == false)
                    {
                        int mydt = int.Parse(todata);
                        int nodt = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                        if (mydt < nodt)
                            continue;
                    }


                    sql += " UNION ";

                    if (ath.AuthType == AuthorWay.SpecFlows)
                        sql += "SELECT WorkID FROM WF_EmpWorks WHERE  WFState=5 AND  FK_Emp='" + ath.Auther + "' AND FK_Flow='" + ath.FlowNo + "' AND OrgNo='" + BP.Web.WebUser.OrgNo + "'";
                    else
                        sql += "SELECT WorkID FROM WF_EmpWorks WHERE  WFState=5 AND  FK_Emp='" + ath.Auther + "' AND OrgNo='" + BP.Web.WebUser.OrgNo + "'";

                }


                string mysql = "SELECT COUNT(*) From (" + sql + ") A";
                return DBAccess.RunSQLReturnValInt(mysql);

            }
        }
        /// <summary>
        /// 待办逾期的数量
        /// </summary>
        public static int Todolist_OverWorkNum
        {
            get
            {
                string whereSQL = " AND convert(varchar(100),SDT,120)<CONVERT(varchar(100), GETDATE(), 120) AND WFState=2 AND FK_Node NOT like '%01' AND ListType=0";

                string sql = "SELECT WorkID FROM WF_EmpWorks WHERE FK_Emp='" + BP.Web.WebUser.No + "' AND OrgNo='" + BP.Web.WebUser.OrgNo + "' " + whereSQL;
                //获得授权信息.
                Auths aths = new Auths();
                aths.Retrieve(AuthAttr.AutherToEmpNo, WebUser.No, AuthAttr.OrgNo, WebUser.OrgNo);

                foreach (Auth ath in aths)
                {
                    if (ath.AuthType == AuthorWay.None || ath.Auther == WebUser.No)
                        continue;

                    string todata = ath.TakeBackDT.Replace("-", "");
                    if (DataType.IsNullOrEmpty(ath.TakeBackDT) == false)
                    {
                        int mydt = int.Parse(todata);
                        int nodt = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                        if (mydt < nodt)
                            continue;

                    }

                    sql += " UNION ";

                    if (ath.AuthType == AuthorWay.SpecFlows)
                        sql += "SELECT WorkID FROM WF_EmpWorks WHERE  FK_Emp='" + ath.Auther + "' AND FK_Flow='" + ath.FlowNo + "' AND OrgNo='" + BP.Web.WebUser.OrgNo + "' " + whereSQL;
                    else
                        sql += "SELECT WorkID FROM WF_EmpWorks WHERE  FK_Emp='" + ath.Auther + "' AND OrgNo='" + BP.Web.WebUser.OrgNo + "' " + whereSQL;
                }


                string mysql = "SELECT COUNT(*) From (" + sql + ") A";
                return DBAccess.RunSQLReturnValInt(mysql);
            }
        }
    }
}
