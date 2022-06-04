using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.WF.Template;
using BP.Difference;


namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_Admin : DirectoryPageBase
    {
        #region 属性.
        public string RefNo
        {
            get
            {
                return this.GetRequestVal("RefNo");
            }
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin()
        {
        }

        #region 测试页面.
        /// <summary>
        /// 获得运行的集成平台.
        /// </summary>
        /// <returns></returns>
        public string TestFlow_GetRunOnPlant()
        {
            return BP.Difference.SystemConfig.RunOnPlant;
        }
        /// <summary>
        /// 加密工具.
        /// </summary>
        /// <returns></returns>
        public string Encrypto_Exe()
        {
            DecryptAndEncryptionHelper.decode decode = new DecryptAndEncryptionHelper.decode();
            string mstr = this.GetRequestVal("mstr").Trim();
            string encryptoStr = decode.encrypto_exe(mstr);
            return encryptoStr;
        }

        /// <summary>
        /// 解密方法.
        /// </summary>
        /// <returns></returns>
        public string Decode_Exe()
        {
            DecryptAndEncryptionHelper.decode decode = new DecryptAndEncryptionHelper.decode();
            string sstr = this.GetRequestVal("sstr").Trim();
            string encryptoStr = decode.decode_exe(sstr);
            return encryptoStr;
        }
        /// <summary>
        /// 初始化界面.
        /// </summary>
        /// <returns></returns>
        public string TestFlow_Init()
        {
            //清除缓存.
            SystemConfig.DoClearCash();

            if (1 == 2 && BP.Web.WebUser.IsAdmin == false)
                return "err@您不是管理员，无法执行该操作.";

            // 让admin 登录.
            //   BP.WF.Dev2Interface.Port_Login("admin");

            if (this.RefNo != null)
            {
                Emp emp = new Emp(this.RefNo);
                BP.Web.WebUser.SignInOfGener(emp);
                HttpContextHelper.SessionSet("FK_Flow", this.FK_Flow);
                return "url@../MyFlow.htm?FK_Flow=" + this.FK_Flow;
            }

            FlowExt fl = new FlowExt(this.FK_Flow);

            if (1 == 2 && BP.Web.WebUser.No != "admin" && fl.Tester.Length <= 1)
            {
                string msg = "err@二级管理员[" + BP.Web.WebUser.Name + "]您好,您尚未为该流程配置测试人员.";
                msg += "您需要在流程属性里的底部[设置流程发起测试人]的属性里，设置可以发起的测试人员,多个人员用逗号分开.";
                return msg;
            }

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
                    dr["No"] = emp.UserID;
                    dr["Name"] = emp.Name;
                    dr["FK_DeptText"] = emp.FK_DeptText;
                    dtEmps.Rows.Add(dr);
                }
                return BP.Tools.Json.ToJson(dtEmps);
            }

            //fl.DoCheck();

            int nodeid = int.Parse(this.FK_Flow + "01");
            DataTable dt = null;
            string sql = "";
            BP.WF.Node nd = new BP.WF.Node(nodeid);

            if (nd.IsGuestNode)
            {
                /*如果是guest节点，就让其跳转到 guest登录界面，让其发起流程。*/
                //这个地址需要配置.
                return "url@/SDKFlowDemo/GuestApp/Login.htm?FK_Flow=" + this.FK_Flow;
            }

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
                        sql = "SELECT No,Name FROM Port_Emp where FK_Dept in (select FK_Dept from WF_NodeDept where FK_Node='" + nodeid + "') ";
                        //emps.RetrieveInSQL("");
                        break;

                    case DeliveryWay.ByBindEmp:
                        //sql = "SELECT No,Name from Port_Emp where No in (select FK_Emp from WF_NodeEmp where FK_Node='" + nodeid + "') ";
                        if (CCBPMRunModel.SAAS ==  BP.Difference.SystemConfig.CCBPMRunModel)
                            sql = "SELECT UserID,Name FROM Port_Emp A, WF_NodeEmp B WHERE A.UserID=B.FK_Emp AND B.FK_Node=" + nodeid;
                        else
                            sql = "SELECT No,Name FROM Port_Emp A, WF_NodeEmp B WHERE A.No=B.FK_Emp AND  B.FK_Node=" + nodeid;

                        //emps.RetrieveInSQL("select fk_emp from wf_NodeEmp WHERE fk_node=" + int.Parse(this.FK_Flow + "01") + " ");
                        break;
                    case DeliveryWay.ByDeptAndStation:
                        //added by liuxc,2015.6.30.

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
                        switch (BP.Difference.SystemConfig.AppCenterDBType)
                        {
                            case DBType.MSSQL:
                                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                                    sql = "SELECT top 300 No as FK_Emp FROM Port_Emp ";
                                else
                                    sql = "SELECT top 300 No as FK_Emp FROM Port_Emp WHERE OrgNo='" + WebUser.OrgNo + "' ";
                                break;
                            case DBType.Oracle:
                                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                                    sql = "SELECT  No as FK_Emp FROM Port_Emp WHERE ROWNUM < 300 ";
                                else
                                    sql = "SELECT  No as FK_Emp FROM Port_Emp WHERE ROWNUM < 300 AND OrgNo='" + WebUser.OrgNo + "'";
                                break;
                            case DBType.MySQL:
                                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                                    sql = "SELECT  No as FK_Emp FROM Port_Emp limit 0,300 ";
                                else
                                    sql = "SELECT  No as FK_Emp FROM Port_Emp limit 0,300 WHERE   OrgNo='" + WebUser.OrgNo + "' ";
                                break;
                            default:
                                return "err@没有判断的数据库类型.";
                        }
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
                    return "err@您按照:" + nd.HisDeliveryWay + "的方式设置的开始节点的访问规则，但是开始节点没有人员。" + sql;

                if (dt.Rows.Count > 2000)
                    return "err@可以发起开始节点的人员太多，会导致系统崩溃变慢，您需要在流程属性里设置可以发起的测试用户.";

                // 构造人员表.
                DataTable dtMyEmps = new DataTable();
                dtMyEmps.Columns.Add("No");
                dtMyEmps.Columns.Add("Name");
                dtMyEmps.Columns.Add("FK_DeptText");

                //处理发起人数据.
                string emps = "";
                BP.Port.Emp emp = new Emp();
                foreach (DataRow dr in dt.Rows)
                {
                    string myemp = dr[0].ToString();
                    if (emps.Contains("," + myemp + ",") == true)
                        continue;

                    emps += "," + myemp + ",";

                    if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                    {
                        emp = new Emp();
                        emp.No = myemp;
                        emp.Retrieve();
                    }
                    else
                    {
                        emp = new Emp(myemp);
                    }



                    DataRow drNew = dtMyEmps.NewRow();

                    drNew["No"] = emp.UserID;
                    drNew["Name"] = emp.Name;
                    drNew["FK_DeptText"] = emp.FK_DeptText;

                    dtMyEmps.Rows.Add(drNew);
                }

                //返回数据源.
                return BP.Tools.Json.ToJson(dtMyEmps);
            }
            catch (Exception ex)
            {
                return "err@<h3>您没有正确的设置开始节点的访问规则，这样导致没有可启动的人员，请在开始节点上右键设置接受人 </h3> 详细的错误信息：" + ex.Message;
            }
        }


        /// <summary>
        /// 转到指定的url.
        /// </summary>
        /// <returns></returns>
        public string TestFlow_ReturnToUser()
        {
            string userNo = this.GetRequestVal("UserNo");
            BP.WF.Dev2Interface.Port_Login(userNo);
            string sid = BP.WF.Dev2Interface.Port_GenerToken(userNo);
            string url = "../../WF/Port.htm?UserNo=" + userNo + "&Token=" + sid + "&DoWhat=" + this.GetRequestVal("DoWhat") + "&FK_Flow=" + this.FK_Flow + "&IsMobile=" + this.GetRequestVal("IsMobile");
            return "url@" + url;
        }
        #endregion 测试页面.

        #region 安装.
        /// <summary>
        /// 初始化安装包
        /// </summary>
        /// <returns></returns>
        public string DBInstall_Init()
        {
            try
            {
                if (DBAccess.TestIsConnection() == false)
                    return "err@数据库连接配置错误,请参考手册查看数据库配置连接.";

                //   DBAccess.IsCaseSensitive

                //判断是否可以安装,不能安装就抛出异常.
                BP.WF.Glo.IsCanInstall();

                //判断是不是有.
                if (DBAccess.IsExitsObject("WF_Flow") == true)
                    return "err@info数据库已经安装上了，您不必在执行安装. 点击:<a href='../Portal/Login.htm' >这里直接登录流程设计器</a>";

                Hashtable ht = new Hashtable();
                ht.Add("OSModel", 1); //组织结构类型.
                ht.Add("DBType", BP.Difference.SystemConfig.AppCenterDBType.ToString()); //数据库类型.
                ht.Add("Ver", BP.WF.Glo.Ver); //版本号.

                return BP.Tools.Json.ToJson(ht);
            }
            catch (Exception ex)
            {

                return "err@" + ex.Message;
            }
        }
        public string DBInstall_Submit()
        {
            string lang = "CH";

            //是否要安装demo.
            int demoTye = this.GetRequestValInt("DemoType");

            //运行ccflow的安装.
            BP.WF.Glo.DoInstallDataBase(lang, demoTye);

            //执行ccflow的升级。
            BP.WF.Glo.UpdataCCFlowVer();

            //加注释.
            BP.Pub.PubClass.AddComment();

            if (DBAccess.IsExitsTableCol("Port_Emp", "EmpSta") == true)
            {
                DBAccess.DropTableColumn("Port_Emp", "EmpSta");
                //string sql = "UPDATE Port_Emp SET EmpSta=1 ";
                //if (DBAccess.RunSQLReturnValInt(sql, 1) == 0)
                //    return "err@该用户已经被禁用.";
            }

            return "info@系统成功安装 点击:<a href='../../Portal/Default.htm' >这里直接登录流程设计器</a>";
            // this.Response.Redirect("DBInstall.aspx?DoType=OK", true);
        }
        #endregion

        public string ReLoginSubmit()
        {
            string userNo = this.GetValFromFrmByKey("TB_No");
            string password = this.GetValFromFrmByKey("TB_PW");

            BP.Port.Emp emp = new BP.Port.Emp();
            emp.UserID = userNo;
            if (emp.RetrieveFromDBSources() == 0)
                return "err@用户名或密码错误.";

            if (emp.CheckPass(password) == false)
                return "err@用户名或密码错误.";

            BP.Web.WebUser.SignInOfGener(emp);

            return "登录成功.";
        }
        /// <summary>
        /// 加载模版.
        /// </summary>
        /// <returns></returns>
        public string SettingTemplate_Init()
        {
            //类型.
            string templateType = this.GetRequestVal("TemplateType");
            string condType = this.GetRequestVal("CondType");

            BP.WF.Template.SQLTemplates sqls = new SQLTemplates();
            //sqls.Retrieve(BP.WF.Template.SQLTemplateAttr.SQLType, sqlType);

            DataTable dt = null;
            string sql = "";

            #region 节点方向条件模版.
            if (templateType == "CondBySQL")
            {
                /*方向条件, 节点方向条件.*/
                sql = "SELECT MyPK,Note,OperatorValue FROM WF_Cond WHERE CondType=" + condType + " AND DataFrom=" + (int)ConnDataFrom.SQL;
            }

            if (templateType == "CondByUrl")
            {
                /*方向条件, 节点方向url条件.*/
                sql = "SELECT MyPK,Note,OperatorValue FROM WF_Cond WHERE CondType=" + condType + " AND DataFrom=" + (int)ConnDataFrom.Url;
            }

            if (templateType == "CondByPara")
            {
                /*方向条件, 节点方向url条件.*/
                sql = "SELECT MyPK,Note,OperatorValue FROM WF_Cond WHERE CondType=" + condType + " AND DataFrom=" + (int)ConnDataFrom.Paras;
            }
            #endregion 节点方向条件模版.

            #region 表单扩展设置.

            string add = "+";

            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.Oracle || BP.Difference.SystemConfig.AppCenterDBType == DBType.PostgreSQL || BP.Difference.SystemConfig.AppCenterDBType == DBType.UX)
                add = "||";

            if (templateType == "DDLFullCtrl")
                sql = "SELECT MyPK, '下拉框:'" + add + " a.AttrOfOper as Name,Doc FROM Sys_MapExt a  WHERE ExtType='DDLFullCtrl'";

            if (templateType == "ActiveDDL")
                sql = "SELECT MyPK, '下拉框:'" + add + " a.AttrOfOper as Name,Doc FROM Sys_MapExt a  WHERE ExtType='ActiveDDL'";

            //显示过滤.
            if (templateType == "AutoFullDLL")
                sql = "SELECT MyPK, '下拉框:'" + add + " a.AttrOfOper as Name,Doc FROM Sys_MapExt a  WHERE ExtType='AutoFullDLL'";

            //文本框自动填充..
            if (templateType == "TBFullCtrl")
                sql = "SELECT MyPK, '文本框:'" + add + " a.AttrOfOper as Name,Doc FROM Sys_MapExt a  WHERE ExtType='TBFullCtrl'";

            //自动计算.
            if (templateType == "AutoFull")
                sql = "SELECT MyPK, 'ID:'" + add + " a.AttrOfOper as Name,Doc FROM Sys_MapExt a  WHERE ExtType='AutoFull'";
            #endregion 表单扩展设置.

            #region 节点属性的模版.
            //自动计算.
            if (templateType == "NodeAccepterRole")
                sql = "SELECT NodeID, FlowName +' - '+Name, a.DeliveryParas as Docs FROM WF_Node a WHERE  a.DeliveryWay=" + (int)DeliveryWay.BySQL;
            #endregion 节点属性的模版.

            if (sql == "")
                return "err@没有涉及到的标记[" + templateType + "].";

            dt = DBAccess.RunSQLReturnTable(sql);
            string strs = "";
            foreach (DataRow dr in dt.Rows)
            {
                BP.WF.Template.SQLTemplate en = new SQLTemplate();
                en.No = dr[0].ToString();
                en.Name = dr[1].ToString();
                en.Docs = dr[2].ToString();

                if (strs.Contains(en.Docs.Trim() + ";") == true)
                    continue;
                strs += en.Docs.Trim() + ";";
                sqls.AddEntity(en);
            }

            return sqls.ToJson();
        }
    }
}
