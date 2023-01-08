using System;
using System.Data;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
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
        /// 测试页面初始化
        /// </summary>
        /// <returns></returns>
        public string Default_Init()
        {
            string testerNo = this.GetRequestVal("TesterNo");
            if (WebUser.No.Equals(testerNo) == false)
            {
                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                    BP.WF.Dev2Interface.Port_Login(testerNo);
                else
                    BP.WF.Dev2Interface.Port_Login(testerNo, BP.Web.WebUser.OrgNo);
            }

            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(this.FK_Flow, testerNo);
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
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel ==FieldCaseModel.UpperCase)
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
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                Track tk = new Track();
                foreach (Attr attr in tk.EnMap.Attrs)
                {
                    if (dt.Columns.Contains(attr.Key.ToLower()) == true)
                    {
                        dt.Columns[attr.Key.ToLower()].ColumnName = attr.Key;

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
        /// SelectOneUser_Init
        /// </summary>
        /// <returns></returns>
        public string SelectOneUser_Init()
        {
            //Default_LetAdminerLogin();

            BP.WF.GenerWorkerLists ens = new GenerWorkerLists();
            QueryObject qo = new QueryObject(ens);
            qo.AddWhere("WorkID", this.WorkID);
            qo.addOr();
            qo.AddWhere("FID", this.WorkID);
            qo.addOrderBy(" FK_Node,RDT,CDT ");
            qo.DoQuery();

            return ens.ToJson();
        }

        /// <summary>
        /// 让adminer登录.
        /// </summary>
        /// <returns></returns>
        public string Default_LetAdminerLogin()
        {
            try
            {
                string token = this.GetRequestVal("Token");
                string userNo = BP.WF.Dev2Interface.Port_LoginByToken(token);
                Dev2Interface.Port_GenerToken(userNo);
                return userNo;
            }
            catch (Exception ex)
            {
                //@ 多人用同一个账号登录，就需要加上如下代码.
                if (DataType.IsNullOrEmpty(this.UserNo) == false)
                {
                    BP.WF.Dev2Interface.Port_Login(this.UserNo);
                    Dev2Interface.Port_GenerToken(this.UserNo);
                    return this.UserNo;
                }
                return ex.Message;
            }
        }
        /// <summary>
        /// 切换用户
        /// </summary>
        /// <returns></returns>
        public string SelectOneUser_ChangUser()
        {

            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
            {
                string adminer = this.GetRequestVal("Adminer");
                string SID = this.GetRequestVal("Token");
                try
                {
                    BP.WF.Dev2Interface.Port_Login(this.FK_Emp);
                    string token = Dev2Interface.Port_GenerToken(this.FK_Emp);
                    return token;
                }
                catch (Exception ex)
                {
                    return "err@" + ex.Message;
                }
            }

            try
            {
                BP.WF.Dev2Interface.Port_Login(this.FK_Emp, this.OrgNo);
                string token = Dev2Interface.Port_GenerToken(this.FK_Emp);
                return token;
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
            //此SID是管理员的SID.
            string testerNo = this.GetRequestVal("TesterNo");
            FlowExt fl = new FlowExt(this.FK_Flow);
            fl.Tester = testerNo;
            fl.Update();

            //选择的人员登录
            BP.WF.Dev2Interface.Port_Login(testerNo);
            string token = BP.WF.Dev2Interface.Port_GenerToken(testerNo);

            //组织url发起该流程.
            string url = "Default.html?RunModel=1&FK_Flow=" + this.FK_Flow + "&TesterNo=" + testerNo;
            url += "&OrgNo=" + WebUser.OrgNo;
            url += "&UserNo=" + this.GetRequestVal("UserNo");
            url += "&Token=" + token;
            return url;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string TestFlow2020_Init()
        {
            //清除缓存.
            BP.Difference.SystemConfig.DoClearCash();

            if (BP.Web.WebUser.IsAdmin == false)
                return "err@您不是管理员，无法执行该操作.";

            FlowExt fl = new FlowExt(this.FK_Flow);

            #region 检查.
            int nodeid = int.Parse(this.FK_Flow + "01");
            DataTable dt = null;
            string sql = "";
            BP.WF.Node nd = new BP.WF.Node(nodeid);
            /* if (nd.IsGuestNode)
             {
                 *//*如果是 guest 节点，就让其跳转到 guest登录界面，让其发起流程。*//*
                 //这个地址需要配置.
                 return "url@/SDKFlowDemo/GuestApp/Login.htm?FK_Flow=" + this.FK_Flow;
             }*/
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
                dtEmps.Columns.Add("DeptFullName");


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
                    dr["No"] = emp.UserID;
                    dr["Name"] = emp.Name;
                    dr["FK_DeptText"] = emp.FK_DeptText;

                    //dr["DeptFullName"] = ;

                    dtEmps.Rows.Add(dr);
                }

                if (dtEmps.Rows.Count >= 1)
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
                        {
                            sql = "SELECT Port_Emp.No  FROM Port_Emp LEFT JOIN Port_Dept ON  Port_Emp.FK_Dept=Port_Dept.No  join Port_DeptEmpStation ON (fk_emp=Port_Emp.No) join WF_NodeStation on (WF_NodeStation.fk_station=Port_DeptEmpStation.fk_station) WHERE (1=1) AND  FK_Node=" + nd.NodeID;
                        }
                        else
                        {
                            // 查询当前组织下所有的该角色的人员. 
                            sql = "SELECT a." + BP.Sys.Base.Glo.UserNo + " as No FROM Port_Emp A, Port_DeptEmpStation B, WF_NodeStation C ";
                            sql += " WHERE A.OrgNo='" + WebUser.OrgNo + "' AND C.FK_Node=" + nd.NodeID;
                            sql += " AND A.No=B.FK_Emp AND B.FK_Station=C.FK_Station ";
                        }
                        break;
                    case DeliveryWay.ByTeamOrgOnly: //按照组织智能计算。
                    case DeliveryWay.ByTeamDeptOnly: //按照组织智能计算。
                        sql = "SELECT A." + BP.Sys.Base.Glo.UserNo + ",A.Name FROM Port_Emp A, WF_NodeTeam B, Port_TeamEmp C ";
                        sql += " WHERE A." + BP.Sys.Base.Glo.UserNo + "=C.FK_Emp AND B.FK_Team=C.FK_Team AND B.FK_Node=" + nd.NodeID + " AND A.OrgNo='" + BP.Web.WebUser.OrgNo + "'";
                        break;
                    case DeliveryWay.ByTeamOnly: //仅按用户组计算. 

                        sql = "SELECT A." + BP.Sys.Base.Glo.UserNo + ",A.Name FROM Port_Emp A, WF_NodeTeam B, Port_TeamEmp C ";
                        sql += " WHERE A.No=C.FK_Emp AND B.FK_Team=C.FK_Team AND B.FK_Node=" + nd.NodeID;
                        break;
                    case DeliveryWay.ByDept:
                        sql = "SELECT " + BP.Sys.Base.Glo.UserNo + ",Name FROM Port_Emp A, WF_NodeDept B WHERE A.FK_Dept=B.FK_Dept AND B.FK_Node=" + nodeid;
                        break;
                    case DeliveryWay.ByBindEmp:
                        if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                            sql = "SELECT " + BP.Sys.Base.Glo.UserNo + ",Name FROM Port_Emp WHERE " + BP.Sys.Base.Glo.UserNo + " IN (SELECT FK_Emp from WF_NodeEmp where FK_Node='" + nodeid + "') AND OrgNo='" + BP.Web.WebUser.OrgNo + "'";
                        else
                            sql = "SELECT " + BP.Sys.Base.Glo.UserNo + ",Name FROM Port_Emp WHERE " + BP.Sys.Base.Glo.UserNo + " in (SELECT FK_Emp from WF_NodeEmp where FK_Node='" + nodeid + "') ";

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
                        {
                            sql = "SELECT A.No, A.Name, B.Name as FK_DeptText FROM  Port_Emp A, Port_Dept B WHERE A.FK_Dept=B.No";
                            //sql = "SELECT c.No, c.Name, B.Name as FK_DeptText FROM Port_DeptEmp A, Port_Dept B, Port_Emp C WHERE A.FK_Dept=B.No AND A.FK_Emp=C.No";
                        }
                        else
                        {
                            sql = "SELECT c." + BP.Sys.Base.Glo.UserNo + ", c.Name, B.Name as FK_DeptText FROM Port_DeptEmp A, Port_Dept B, Port_Emp C WHERE A.FK_Dept=B.No  AND A.FK_Emp=C." + BP.Sys.Base.Glo.UserNoWhitOutAS + " ";
                            sql += " AND A.OrgNo='" + BP.Web.WebUser.OrgNo + "' ";
                            sql += " AND B.OrgNo='" + BP.Web.WebUser.OrgNo + "' ";
                            sql += " AND C.OrgNo='" + BP.Web.WebUser.OrgNo + "' ";
                        }

                        break;
                    case DeliveryWay.BySelectedOrgs: //按照设置的组织计算: 20202年3月开始约定此规则.

                        if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
                            throw new Exception("err@非集团版本，不能设置启用此模式.");

                        sql = " SELECT A." + BP.Sys.Base.Glo.UserNo + ",A.Name,C.Name as FK_DeptText FROM Port_Emp A, WF_FlowOrg B, port_dept C ";
                        sql += " WHERE A.OrgNo = B.OrgNo AND B.FlowNo = '" + this.FK_Flow + "' AND A.FK_Dept = c.No ";

                        break;
                    case DeliveryWay.BySQL:
                        if (DataType.IsNullOrEmpty(nd.DeliveryParas))
                            return "err@您设置的按SQL访问开始节点，但是您没有设置sql.";
                        break;
                    default:
                        break;
                }

                dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0)
                    return "err@您按照:[" + nd.HisDeliveryWay + "]的方式设置的开始节点的访问规则，但是开始节点没有人员.";

                if (dt.Rows.Count > 500)
                    return "err@可以发起开始节点的人员太多，会导致系统崩溃变慢，请<a href='javascript:SetTester()' >设置测试发起人</a>。";

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

                    //查询数据。
                    BP.Port.Emp emp = new Emp();

                    if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                    {
                        emp.No = this.OrgNo + "_" + myemp;
                        emp.RetrieveFromDBSources();
                    }
                    else
                    {
                        emp.No = myemp;
                        emp.RetrieveFromDBSources();
                    }

                    //if (emp.RetrieveFromDBSources())

                    DataRow drNew = dtMyEmps.NewRow();

                    drNew["No"] = emp.UserID;
                    drNew["Name"] = emp.Name;
                    drNew["FK_DeptText"] = emp.FK_DeptText;

                    dtMyEmps.Rows.Add(drNew);
                }


                if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
                {
                    dtMyEmps.Columns["NO"].ColumnName = "No";
                    dtMyEmps.Columns["NAME"].ColumnName = "Name";
                    dtMyEmps.Columns["FK_DEPTTEXT"].ColumnName = "FK_DeptText";
                }

                if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
                {
                    dtMyEmps.Columns["no"].ColumnName = "No";
                    dtMyEmps.Columns["name"].ColumnName = "Name";
                    dtMyEmps.Columns["fk_depttext"].ColumnName = "FK_DeptText";
                }

                //返回数据源.
                return BP.Tools.Json.ToJson(dtMyEmps);
                #endregion 从设置里获取-测试人员.

            }
            catch (Exception ex)
            {
                return "err@您没有正确的设置开始节点的访问规则，这样导致没有可启动的人员，方法：TestFlow2020_Init。系统错误提示:" + ex.Message;
            }
        }
        #endregion

    }
}
