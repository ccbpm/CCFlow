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
    public class WF_Admin_TestingStep : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_TestingStep()
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
            if (WebUser.No.Equals(userNo)==false)
                BP.WF.Dev2Interface.Port_Login(userNo);

            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(this.FK_Flow, userNo);
            return workid.ToString();
        }

        #region TestFlow2020_Init
        /// <summary>
        /// 发起.
        /// </summary>
        /// <returns></returns>
        public string TestFlow2020_StartIt()
        {
            if (WebUser.IsAdmin == false)
                return "err@非管理员无法测试";

            //用户编号.
            string userNo = this.GetRequestVal("UserNo");

            //判断是否可以测试该流程？ 

            //组织url发起该流程.
            string url = "Default.html?RunModel=1&FK_Flow=" + this.FK_Flow + "&Adminer=" + WebUser.No + "&AdminerSID=" + WebUser.SID + "&UserNo=" + userNo;
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

            if (1 == 2 && BP.Web.WebUser.IsAdmin == false)
                return "err@您不是管理员，无法执行该操作.";

            // 让admin 登录.
            //   BP.WF.Dev2Interface.Port_Login("admin");

            if (this.RefNo != null)
            {
                Emp emp = new Emp(this.RefNo);
                BP.Web.WebUser.SignInOfGener(emp);
                HttpContextHelper.SessionSet("FK_Flow", this.FK_Flow); //设置当前的流程.
                return "url@../MyFlow.htm?FK_Flow=" + this.FK_Flow;
            }

            FlowExt fl = new FlowExt(this.FK_Flow);

            if (1 == 2 && BP.Web.WebUser.No != "admin" && fl.Tester.Length <= 1)
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

            #region 从设置里获取-测试人员.
            try
            {

                switch (nd.HisDeliveryWay)
                {
                    case DeliveryWay.ByStation:
                    case DeliveryWay.ByStationOnly:

                        sql = "SELECT Port_Emp.No  FROM Port_Emp LEFT JOIN Port_Dept   Port_Dept_FK_Dept ON  Port_Emp.FK_Dept=Port_Dept_FK_Dept.No  join Port_DeptEmpStation on (fk_emp=Port_Emp.No)   join WF_NodeStation on (WF_NodeStation.fk_station=Port_DeptEmpStation.fk_station) WHERE (1=1) AND  FK_Node=" + nd.NodeID;
                        // emps.RetrieveInSQL_Order("select fk_emp from Port_Empstation WHERE fk_station in (select fk_station from WF_NodeStation WHERE FK_Node=" + nodeid + " )", "FK_Dept");
                        break;
                    case DeliveryWay.ByDept:
                        sql = "select No,Name from Port_Emp where FK_Dept in (select FK_Dept from WF_NodeDept where FK_Node='" + nodeid + "') ";
                        //emps.RetrieveInSQL("");
                        break;
                    case DeliveryWay.ByBindEmp:
                        sql = "select No,Name from Port_Emp where No in (select FK_Emp from WF_NodeEmp where FK_Node='" + nodeid + "') ";
                        //emps.RetrieveInSQL("select fk_emp from wf_NodeEmp WHERE fk_node=" + int.Parse(this.FK_Flow + "01") + " ");
                        break;
                    case DeliveryWay.ByDeptAndStation:
                        //added by liuxc,2015.6.30.
                        //区别集成与BPM模式
                        if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneOne)
                        {
                            sql = "SELECT No FROM Port_Emp WHERE No IN ";
                            sql += "(SELECT No as FK_Emp FROM Port_Emp WHERE FK_Dept IN ";
                            sql += "( SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node=" + nodeid + ")";
                            sql += ")";
                            sql += "AND No IN ";
                            sql += "(";
                            sql += "SELECT FK_Emp FROM " + BP.WF.Glo.EmpStation + " WHERE FK_Station IN ";
                            sql += "( SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + nodeid + ")";
                            sql += ") ORDER BY No ";
                        }
                        else
                        {
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
                        }
                        break;
                    case DeliveryWay.BySelected: //所有的人员多可以启动, 2016年11月开始约定此规则.
                        sql = "SELECT No as FK_Emp FROM Port_Emp ";
                        dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                        if (dt.Rows.Count > 300)
                        {
                            if (SystemConfig.AppCenterDBType == BP.DA.DBType.MSSQL)
                                sql = "SELECT top 300 No as FK_Emp FROM Port_Emp ";

                            if (SystemConfig.AppCenterDBType == BP.DA.DBType.Oracle)
                                sql = "SELECT  No as FK_Emp FROM Port_Emp WHERE ROWNUM <300 ";

                            if (SystemConfig.AppCenterDBType == BP.DA.DBType.MySQL)
                                sql = "SELECT  No as FK_Emp FROM Port_Emp   limit 0,300 ";
                        }
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

                //检查物理表,避免错误.
                Nodes nds = new Nodes(this.FK_Flow);
                foreach (Node mynd in nds)
                {
                    mynd.HisWork.CheckPhysicsTable();
                }

                //返回数据源.
                return BP.Tools.Json.ToJson(dtMyEmps);
                #endregion 从设置里获取-测试人员.

            }
            catch (Exception ex)
            {
                return "err@<h2>您没有正确的设置开始节点的访问规则，这样导致没有可启动的人员，<a href='http://bbs.ccflow.org/showtopic-4103.aspx' target=_blank ><font color=red>点击这查看解决办法</font>.</a>。</h2> 系统错误提示:" + ex.Message + "<br><h3>也有可能你你切换了OSModel导致的，什么是OSModel,请查看在线帮助文档 <a href='http://ccbpm.mydoc.io' target=_blank>http://ccbpm.mydoc.io</a>  .</h3>";
            }
        }
        #endregion

    }
}
