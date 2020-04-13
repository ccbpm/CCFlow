using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_Admin_TestingContainer : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_TestingContainer()
        {
        }

        /// <summary>
        /// 左侧的树刷新
        /// </summary>
        /// <returns></returns>
        public string Left_Init()
        {
            return "";
        }
        /// <summary>
        /// 测试页面初始化
        /// </summary>
        /// <returns></returns>
        public string Default_Init()
        {
            string userNo = this.GetRequestVal("UserNo");
            if (WebUser.No.Equals(userNo) == false)
                BP.WF.Dev2Interface.Port_Login(userNo);

            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(this.FK_Flow, userNo);
            return workid.ToString();
        }


        /// <summary>
        /// 数据库信息
        /// </summary>
        /// <returns></returns>
        public string DBInfo_Init()
        {
            //数据容器，用于盛放数据，并返回json.
            DataSet ds = new DataSet();

            //流程引擎控制表.
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            ds.Tables.Add(gwf.ToDataTableField("WF_GenerWorkFlow"));

            //流程引擎人员列表.
            GenerWorkerLists gwls = new GenerWorkerLists(this.WorkID);
            gwls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.RDT);
            ds.Tables.Add(gwls.ToDataTableField("WF_GenerWorkerList"));


            //获得Track数据.
            string table = "ND" + int.Parse(this.FK_Flow) + "Track";
            string sql = "SELECT * FROM " + table + " WHERE WorkID=" + this.WorkID;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Track";
            //把列大写转化为小写.
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                Track tk = new Track();
                foreach (Attr attr in tk.EnMap.Attrs)
                {
                    if (dt.Columns.Contains(attr.Key.ToUpper()) == true)
                    {
                        dt.Columns[attr.Key.ToUpper()].ColumnName = attr.Key;

                    }
                }
            }
            ds.Tables.Add(dt);

            //获得NDRpt表
            string rpt = "ND" + int.Parse(this.FK_Flow) + "Rpt";
            GEEntity en = new GEEntity(rpt);
            en.PKVal = this.WorkID;
            en.RetrieveFromDBSources();
            ds.Tables.Add(en.ToDataTableField("NDRpt"));

            //转化为json ,返回出去.
            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 让adminer登录.
        /// </summary>
        /// <returns></returns>
        public string Default_LetAdminerLogin()
        {
            string adminer = this.GetRequestVal("Adminer");
            string SID = this.GetRequestVal("SID");
            BP.WF.Dev2Interface.Port_LoginBySID(SID);

            return "登录成功.";
            //Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(this.FK_Flow, userNo);
            //return workid.ToString();
        }
        /// <summary>
        /// 切换用户
        /// </summary>
        /// <returns></returns>
        public string SelectOneUser_ChangUser()
        {

            string adminer = this.GetRequestVal("Adminer");
            string SID = this.GetRequestVal("SID");

            try
            {
                BP.WF.Dev2Interface.Port_Login(this.FK_Emp);
                return "登录成功.";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        #region TestFlow2020_Init
        /// <summary>
        /// 发起.
        /// </summary>
        /// <returns></returns>
        public string TestFlow2020_StartIt()
        {

            string sid = this.GetRequestVal("SID");
            if (WebUser.IsAdmin == false)
                return "err@非管理员无法测试,关闭后重新登录。";

            //用户编号.
            string userNo = this.GetRequestVal("UserNo");

            //判断是否可以测试该流程？ 
            BP.Port.Emp myEmp = new BP.Port.Emp();
            int i = myEmp.Retrieve("SID", sid);
            if (i == 0 && 1 == 2)
                throw new Exception("err@非法的SID:" + sid);

            //组织url发起该流程.
            string url = "Default.html?RunModel=1&FK_Flow=" + this.FK_Flow + "&SID=" + sid + "&UserNo=" + userNo;
            return url;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string TestFlow2020_Init()
        {
            //清除缓存.
            BP.Sys.SystemConfig.DoClearCash();

            if (BP.Web.WebUser.IsAdmin == false)
                return "err@您不是管理员，无法执行该操作.";

            FlowExt fl = new FlowExt(this.FK_Flow);

            if (1 == 2 && BP.Web.WebUser.No.Equals("admin") && fl.Tester.Length <= 1)
            {
                string msg = "err@二级管理员[" + BP.Web.WebUser.Name + "]您好,您尚未为该流程配置测试人员.";
                msg += "您需要在流程属性里的底部[设置流程发起测试人]的属性里，设置可以发起的测试人员,多个人员用逗号分开.";
                return msg;
            }

            #region 检查.
            int nodeid = int.Parse(this.FK_Flow + "01");
            DataTable dt = null;
            string sql = "";
            BP.WF.Node nd = new BP.WF.Node(nodeid);
            if (nd.IsGuestNode)
            {
                /*如果是 guest 节点，就让其跳转到 guest登录界面，让其发起流程。*/
                //这个地址需要配置.
                return "url@/SDKFlowDemo/GuestApp/Login.htm?FK_Flow=" + this.FK_Flow;
            }
            #endregion 测试人员.

            #region 从配置里获取-测试人员.
            /* 检查是否设置了测试人员，如果设置了就按照测试人员身份进入
             * 设置测试人员的目的是太多了人员影响测试效率.
             * */
            if (fl.Tester.Length > 2)
            {
                // 构造人员表.
                DataTable dtEmps = new DataTable();
                dtEmps.Columns.Add("No");
                dtEmps.Columns.Add("Name");
                dtEmps.Columns.Add("FK_DeptText");

                string[] strs = fl.Tester.Split(',');
                foreach (string str in strs)
                {
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;

                    Emp emp = new Emp();
                    emp.SetValByKey("No", str);
                    int i = emp.RetrieveFromDBSources();
                    if (i == 0)
                        continue;

                    DataRow dr = dtEmps.NewRow();
                    dr["No"] = emp.No;
                    dr["Name"] = emp.Name;
                    dr["FK_DeptText"] = emp.FK_DeptText;
                    dtEmps.Rows.Add(dr);
                }
                return BP.Tools.Json.ToJson(dtEmps);
            }
            #endregion 测试人员.

            //fl.DoCheck();

            try
            {
                #region 从设置里获取-测试人员.

                switch (nd.HisDeliveryWay)
                {
                    case DeliveryWay.ByStation:
                    case DeliveryWay.ByStationOnly:
                        if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
                            sql = "SELECT Port_Emp.No  FROM Port_Emp LEFT JOIN Port_Dept   Port_Dept_FK_Dept ON  Port_Emp.FK_Dept=Port_Dept_FK_Dept.No  join Port_DeptEmpStation on (fk_emp=Port_Emp.No) join WF_NodeStation on (WF_NodeStation.fk_station=Port_DeptEmpStation.fk_station) WHERE (1=1) AND  FK_Node=" + nd.NodeID;
                        else
                            sql = "SELECT Port_Emp.No FROM Port_Emp WHERE OrgNo='" + BP.Web.WebUser.OrgNo + "' LEFT JOIN Port_Dept   Port_Dept_FK_Dept ON  Port_Emp.FK_Dept=Port_Dept_FK_Dept.No  join Port_DeptEmpStation on (fk_emp=Port_Emp.No) join WF_NodeStation on (WF_NodeStation.fk_station=Port_DeptEmpStation.fk_station) WHERE (1=1) AND  FK_Node=" + nd.NodeID;

                        // emps.RetrieveInSQL_Order("select fk_emp from Port_Empstation WHERE fk_station in (select fk_station from WF_NodeStation WHERE FK_Node=" + nodeid + " )", "FK_Dept");
                        break;
                    case DeliveryWay.ByGroup: //按照组织智能计算。
                        sql = "SELECT A.No,A.Name FROM Port_Emp A, WF_NodeGroup B, GPM_GroupEmp C ";
                        sql += " WHERE A.No=C.FK_Emp AND B.FK_Group=C.FK_Group AND B.FK_Node=" + nd.NodeID +" AND A.OrgNo='"+BP.Web.WebUser.OrgNo+"'";
                        break;
                    case DeliveryWay.ByGroupOnly: //仅按群组计算. @lizhen.

                        sql = "SELECT A.No,A.Name FROM Port_Emp A, WF_NodeGroup B, GPM_GroupEmp C ";
                        sql += " WHERE A.No=C.FK_Emp AND B.FK_Group=C.FK_Group AND B.FK_Node=" + nd.NodeID;
                        break;
                    case DeliveryWay.ByDept:
                        sql = "SELECT No,Name FROM Port_Emp A, WF_NodeDept B WHERE A.FK_Dept=B.FK_Dept AND B.FK_Node=" + nodeid;
                        break;
                    case DeliveryWay.ByBindEmp:
                        sql = "SELECT No,Name from Port_Emp WHERE No in (select FK_Emp from WF_NodeEmp where FK_Node='" + nodeid + "') ";
                        //emps.RetrieveInSQL("select fk_emp from wf_NodeEmp WHERE fk_node=" + int.Parse(this.FK_Flow + "01") + " ");
                        break;
                    case DeliveryWay.ByDeptAndStation:

                        sql = "SELECT pdes.FK_Emp AS No"
                              + " FROM   Port_DeptEmpStation pdes"
                              + "        INNER JOIN WF_NodeDept wnd"
                              + "             ON  wnd.FK_Dept = pdes.FK_Dept"
                              + "             AND wnd.FK_Node = " + nodeid
                              + "        INNER JOIN WF_NodeStation wns"
                              + "             ON  wns.FK_Station = pdes.FK_Station"
                              + "             AND wnd.FK_Node =" + nodeid
                              + " ORDER BY"
                              + "        pdes.FK_Emp";

                        break;
                    case DeliveryWay.BySelected: //所有的人员多可以启动, 2016年11月开始约定此规则.

                        if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
                            sql = "SELECT c.No, c.Name, B.Name as FK_DeptText FROM Port_DeptEmp A, Port_Dept B, Port_Emp C WHERE A.FK_Dept=B.No AND A.FK_Emp=C.No ";
                        else
                            sql = "SELECT c.No, c.Name, B.Name as FK_DeptText FROM Port_DeptEmp A, Port_Dept B, Port_Emp C WHERE A.FK_Dept=B.No AND B.OrgNo='" + BP.Web.WebUser.OrgNo + "' AND A.FK_Emp=C.No ";

                        dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                        return BP.Tools.Json.ToJson(dt);
                    case DeliveryWay.BySelectedOrgs: //按照设置的组织计算: 20202年3月开始约定此规则.

                        if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
                            throw new Exception("err@非集团版本，不能设置启用此模式.");

                        sql = "SELECT B.No, B.Name, d.Name as FK_DeptText FROM Port_DeptEmp A, Port_Emp B,";
                        sql += " WF_FlowOrg C,port_dept D WHERE A.FK_Emp = B.No AND A.OrgNo = C.OrgNo AND A.FK_Dept = D.No AND C.FlowNo = '" + this.FK_Flow + "'";

                        // sql = "SELECT c.No, c.Name, B.Name as FK_DeptText FROM Port_DeptEmp A, Port_Dept B, WF_FlowOrg C  ";
                        // sql += "WHERE A.FK_Dept=B.No AND B.OrgNo=C.OrgNo AND C.FlowNo='" + this.FK_Flow + "'";
                        //if (dt.Rows.Count > 300 && 1 == 2)
                        //{
                        //    if (SystemConfig.AppCenterDBType == BP.DA.DBType.MSSQL)
                        //        sql = "SELECT Top 200 c.No, c.Name, B.Name as FK_DeptText FROM Port_DeptEmp A, Port_Dept B, WF_FlowOrg C  WHERE A.FK_Dept=B.No AND B.OrgNo=C.OrgNo AND C.FlowNo='" + nd.FK_Flow + "'";

                        //    if (SystemConfig.AppCenterDBType == BP.DA.DBType.Oracle)
                        //        sql = "SELECT c.No, c.Name, B.Name as FK_DeptText FROM Port_DeptEmp A, Port_Dept B, WF_FlowOrg C  WHERE A.FK_Dept=B.No AND B.OrgNo=C.OrgNo AND C.FlowNo='" + nd.FK_Flow + "' AND ROWNUM <300 ";

                        //    if (SystemConfig.AppCenterDBType == BP.DA.DBType.MySQL)
                        //        sql = "SELECT c.No, c.Name, B.Name as FK_DeptText FROM Port_DeptEmp A, Port_Dept B, WF_FlowOrg C  WHERE A.FK_Dept=B.No AND B.OrgNo=C.OrgNo AND C.FlowNo='" + nd.FK_Flow + "'    limit 0,200   ";
                        //}

                        dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                        return BP.Tools.Json.ToJson(dt);
                        break;
                    case DeliveryWay.BySQL:
                        if (DataType.IsNullOrEmpty(nd.DeliveryParas))
                            return "err@您设置的按SQL访问开始节点，但是您没有设置sql.";
                        break;
                    default:
                        break;
                }

                dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0)
                    return "err@您按照:" + nd.HisDeliveryWay + "的方式设置的开始节点的访问规则，但是开始节点没有人员。";

                if (dt.Rows.Count > 2000)
                    return "err@可以发起开始节点的人员太多，会导致系统崩溃变慢，您需要在流程属性里设置可以发起的测试用户.";

                // 构造人员表.
                DataTable dtMyEmps = new DataTable();
                dtMyEmps.Columns.Add("No");
                dtMyEmps.Columns.Add("Name");
                dtMyEmps.Columns.Add("FK_DeptText");

                //处理发起人数据.
                string emps = "";
                foreach (DataRow dr in dt.Rows)
                {
                    string myemp = dr[0].ToString();
                    if (emps.Contains("," + myemp + ",") == true)
                        continue;

                    emps += "," + myemp + ",";

                    BP.Port.Emp emp = new Emp(myemp);

                    DataRow drNew = dtMyEmps.NewRow();

                    drNew["No"] = emp.No;
                    drNew["Name"] = emp.Name;
                    drNew["FK_DeptText"] = emp.FK_DeptText;

                    dtMyEmps.Rows.Add(drNew);
                }

                //返回数据源.
                return BP.Tools.Json.ToJson(dtMyEmps);
                #endregion 从设置里获取-测试人员.

            }
            catch (Exception ex)
            {
                return "err@您没有正确的设置开始节点的访问规则，这样导致没有可启动的人员 系统错误提示:" + ex.Message;
            }
        }
        #endregion

    }
}
