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
using BP.GPM.Menu2020;
using System.Collections;
using BP.WF.Port.Admin2;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_Portal : DirectoryPageBase
    {

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string Home_Init()
        {
            BP.GPM.Home.WindowTemplates ens = new GPM.Home.WindowTemplates();
            ens.RetrieveAll();

            DataTable dt = ens.ToDataTableField();
            dt.TableName = "WindowTemplates";
            return BP.Tools.Json.ToJson(dt);

            //return "移动成功..";
        }
        public string Home_DoMove()
        {
            string[] mypks = this.MyPK.Split(',');
            for (int i = 0; i < mypks.Length; i++)
            {
                string str = mypks[i];
                if (str == null || str == "")
                    continue;

                string sql = "UPDATE GPM_WindowTemplate SET Idx=" + i + " WHERE No='" + str + "' ";
                DBAccess.RunSQL(sql);
            }
            return "移动成功..";

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Portal()
        {
        }
        /// <summary>
        /// 系统信息
        /// </summary>
        /// <returns></returns>
        public string Login_InitInfo()
        {
            Hashtable ht = new Hashtable();
            ht.Add("SysNo", SystemConfig.SysNo);
            ht.Add("SysName", SystemConfig.SysName);

            return BP.Tools.Json.ToJson(ht);
        }
        /// <summary>
        /// 初始化登录界面.
        /// </summary>
        /// <returns></returns>
        public string Login_Init()
        {
            //判断是否已经安装数据库，是否需要更新
            if (CheckIsDBInstall() == true)
                return "url@../Admin/DBInstall.htm";

            string doType = GetRequestVal("LoginType");
            if (DataType.IsNullOrEmpty(doType) == false && doType.Equals("Out") == true)
            {
                //清空cookie
                WebUser.Exit();
            }

            //是否需要自动登录。 这里都把cookeis的数据获取来了.
            string userNo = this.GetRequestVal("UserNo");
            string sid = this.GetRequestVal("SID");

            if (String.IsNullOrEmpty(sid) == false && String.IsNullOrEmpty(userNo) == false)
            {
                //调用登录方法.
                BP.WF.Dev2Interface.Port_Login(this.UserNo, this.SID);
                return "url@Default.htm?UserNo=" + this.UserNo + "&SID=" + SID;

            }

            Hashtable ht = new Hashtable();
            ht.Add("SysName", SystemConfig.SysName);
            ht.Add("SysNo", SystemConfig.SysNo);
            ht.Add("ServiceTel", SystemConfig.ServiceTel);
            ht.Add("CustomerName", SystemConfig.CustomerName);
            if (WebUser.NoOfRel == null)
            {
                ht.Add("UserNo", "");
                ht.Add("UserName", "");
            }
            else
            {
                ht.Add("UserNo", WebUser.No);

                string name = WebUser.Name;

                if (DataType.IsNullOrEmpty(name) == true)
                    ht.Add("UserName", WebUser.No);
                else
                    ht.Add("UserName", name);
            }

            return BP.Tools.Json.ToJsonEntityModel(ht);
        }
        /// <summary>
        /// 登录.
        /// </summary>
        /// <returns></returns>
        public string Login_Submit()
        {
            try
            {
                string userNo = this.GetRequestVal("TB_No");
                if (userNo == null)
                    userNo = this.GetRequestVal("TB_UserNo");

                string pass = this.GetRequestVal("TB_PW");
                if (pass == null)
                    pass = this.GetRequestVal("TB_Pass");

                if (DataType.IsNullOrEmpty(userNo) == false && userNo.Equals("admin"))
                {

                    try
                    {
                        // 执行升级
                        BP.WF.Glo.UpdataCCFlowVer();
                    }
                    catch (Exception ex)
                    {
                        BP.WF.Glo.UpdataCCFlowVer();
                        string msg = "err@升级失败(ccbpm有自动修复功能,您可以刷新一下系统会自动创建字段,刷新多次扔解决不了问题,请反馈给我们)";
                        msg += "@系统信息:" + ex.Message;
                        return msg;
                    }
                }
                BP.Port.Emp emp = new Emp();
                emp.UserID = userNo;
                if (emp.RetrieveFromDBSources() == 0)
                {
                    if (DBAccess.IsExitsTableCol("Port_Emp", "NikeName") == true)
                    {
                        /*如果包含昵称列,就检查昵称是否存在.*/
                        Paras ps = new Paras();
                        ps.SQL = "SELECT No FROM Port_Emp WHERE NikeName=" + SystemConfig.AppCenterDBVarStr + "NikeName";
                        ps.Add("NikeName", userNo);
                        string no = DBAccess.RunSQLReturnStringIsNull(ps, null);
                        if (no == null)
                            return "err@用户名或者密码错误.";

                        emp.No = no;
                        int i = emp.RetrieveFromDBSources();
                        if (i == 0)
                            return "err@用户名或者密码错误.";
                    }
                    else if (DBAccess.IsExitsTableCol("Port_Emp", "Tel") == true)
                    {
                        /*如果包含Name列,就检查Name是否存在.*/
                        Paras ps = new Paras();
                        ps.SQL = "SELECT No FROM Port_Emp WHERE Tel=" + SystemConfig.AppCenterDBVarStr + "Tel";
                        ps.Add("Tel", userNo);
                        string no = DBAccess.RunSQLReturnStringIsNull(ps, null);
                        if (no == null)
                            return "err@用户名或者密码错误.";

                        emp.No = no;
                        int i = emp.RetrieveFromDBSources();
                        if (i == 0)
                            return "err@用户名或者密码错误.";


                    }
                    else
                    {
                        return "err@用户名或者密码错误.";
                    }
                }

                if (emp.CheckPass(pass) == false)
                    return "err@用户名或者密码错误.";

                if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
                {
                    //调用登录方法.
                    BP.WF.Dev2Interface.Port_Login(emp.UserID);

                    if (DBAccess.IsView("Port_Emp") == false)
                    {
                        string sid = DBAccess.GenerGUID();
                        DBAccess.RunSQL("UPDATE Port_Emp SET SID='" + sid + "' WHERE No='" + emp.UserID + "'");
                        WebUser.SID = sid;
                        emp.SID = sid;
                    }

                    return "url@Default.htm?SID=" + emp.SID + "&UserNo=" + emp.UserID;
                }

                //获得当前管理员管理的组织数量.
                OrgAdminers adminers = null;

                //查询他管理多少组织.
                adminers = new OrgAdminers();
                adminers.Retrieve(OrgAdminerAttr.FK_Emp, emp.UserID);
                if (adminers.Count == 0)
                {
                    BP.WF.Port.Admin2.Orgs orgs = new Orgs();
                    int i = orgs.Retrieve("Adminer", this.GetRequestVal("TB_No"));
                    if (i == 0)
                    {
                        //调用登录方法.
                        BP.WF.Dev2Interface.Port_Login(emp.UserID, null, emp.OrgNo);
                        return "url@Default.htm?SID=" + emp.SID + "&UserNo=" + emp.UserID + "&OrgNo=" + emp.OrgNo;
                    }


                    foreach (BP.WF.Port.Admin2.Org org in orgs)
                    {
                        OrgAdminer oa = new OrgAdminer();
                        oa.FK_Emp = WebUser.No;
                        oa.OrgNo = org.No;
                        oa.Save();
                    }
                    adminers.Retrieve(OrgAdminerAttr.FK_Emp, emp.UserID);
                }


                //设置他的组织，信息.
                WebUser.No = emp.UserID; //登录帐号.
                WebUser.FK_Dept = emp.FK_Dept;
                WebUser.FK_DeptName = emp.FK_DeptText;


                //执行登录.
                BP.WF.Dev2Interface.Port_Login(emp.UserID, null, emp.OrgNo);

                //设置SID.
                WebUser.SID = DBAccess.GenerGUID(); //设置SID.
                emp.SID = WebUser.SID; //设置SID.
                BP.WF.Dev2Interface.Port_SetSID(emp.UserID, WebUser.SID);

                //执行更新到用户表信息.
                // WebUser.UpdateSIDAndOrgNoSQL();

                //判断是否是多个组织的情况.
                if (adminers.Count == 1)
                    return "url@Default.htm?SID=" + emp.SID + "&UserNo=" + emp.UserID + "&OrgNo=" + emp.OrgNo;

                return "url@SelectOneOrg.htm?SID=" + emp.SID + "&UserNo=" + emp.UserID + "&OrgNo=" + emp.OrgNo;


            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        private bool CheckIsDBInstall()
        {
            //检查数据库连接.
            try
            {
                DBAccess.TestIsConnection();
            }
            catch (Exception ex)
            {
                throw new Exception("err@异常信息:" + ex.Message);
            }

            //检查是否缺少Port_Emp 表，如果没有就是没有安装.
            if (DBAccess.IsExitsObject("Port_Emp") == false && DBAccess.IsExitsObject("WF_Flow") == false)
                return true;

            //如果没有流程表，就执行安装.
            if (DBAccess.IsExitsObject("WF_Flow") == false)
                return true;
            return false;
        }

        #region Frm.htm 表单.
        /// <summary>
        /// 表单树.
        /// </summary>
        /// <returns></returns>
        public string Frms_InitSort()
        {
            //获得数量.
            string sqlWhere = "";
            string sql = "";
            if (SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                sqlWhere = " AND OrgNo='" + BP.Web.WebUser.OrgNo + "'";

            //求内容.
            sql = "SELECT No,Name FROM Sys_FormTree WHERE 1=1 " + sqlWhere + " ORDER BY Idx ";
            DataTable dtSort = DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                dtSort.Columns[0].ColumnName = "No";
                dtSort.Columns[1].ColumnName = "Name";
                //dtSort.Columns[2].ColumnName = "WFSta2";
                //dtSort.Columns[3].ColumnName = "WFSta3";
                //dtSort.Columns[4].ColumnName = "WFSta5";
            }
            return BP.Tools.Json.ToJson(dtSort);
        }
        /// <summary>
        /// 表单
        /// </summary>
        /// <returns></returns>
        public string Frms_Init()
        {
            //获得流程实例的数量.
            string sqlWhere = "";
            string sql = "";
            if (SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                sqlWhere = " AND OrgNo='" + BP.Web.WebUser.OrgNo + "'";


            //求流程内容.
            sql = "SELECT No,Name,FrmType,FK_FormTree,PTable,DBSrc,Icon,EntityType FROM Sys_MapData WHERE 1=1 " + sqlWhere + " ORDER BY Idx ";
            DataTable dtFlow = DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                dtFlow.Columns[0].ColumnName = "No";
                dtFlow.Columns[1].ColumnName = "Name";
                dtFlow.Columns[2].ColumnName = "FrmType";
                dtFlow.Columns[3].ColumnName = "FK_FormTree";
                dtFlow.Columns[4].ColumnName = "PTable";
                dtFlow.Columns[5].ColumnName = "DBSrc";
                dtFlow.Columns[6].ColumnName = "Icon";
                dtFlow.Columns[7].ColumnName = "EntityType";

                //dtFlow.Columns[2].ColumnName = "WorkModel";
                //dtFlow.Columns[3].ColumnName = "AtPara";
                //dtFlow.Columns[4].ColumnName = "FK_FlowSort";
                //dtFlow.Columns[5].ColumnName = "WFSta2";
                //dtFlow.Columns[6].ColumnName = "WFSta3";
                //dtFlow.Columns[7].ColumnName = "WFSta5";
            }
            return BP.Tools.Json.ToJson(dtFlow);
        }
        /// <summary>
        /// 流程移动.
        /// </summary>
        /// <returns></returns>
        public string Frms_Move()
        {
            string sortNo = this.GetRequestVal("SortNo");
            string[] flowNos = this.GetRequestVal("EnNos").Split(',');
            for (int i = 0; i < flowNos.Length; i++)
            {
                var flowNo = flowNos[i];

                string sql = "UPDATE Sys_MapData SET FK_FormTree ='" + sortNo + "',Idx=" + i + " WHERE No='" + flowNo + "'";
                DBAccess.RunSQL(sql);
            }
            return "表单顺序移动成功..";
        }
        public string Frms_MoveSort()
        {
            string[] ens = this.GetRequestVal("SortNos").Split(',');

            BP.Sys.FrmTree ft = new BP.Sys.FrmTree();

            string table = ft.EnMap.PhysicsTable;

            for (int i = 0; i < ens.Length; i++)
            {
                var en = ens[i];

                string sql = "UPDATE " + table + " SET Idx=" + i + " WHERE No='" + en + "'";
                DBAccess.RunSQL(sql);
            }
            return "目录移动成功..";
        }

        #endregion Frm.htm 表单.

        #region Flows.htm 流程.
        /// <summary>
        /// 初始化类别.
        /// </summary>
        /// <returns></returns>
        public string Flows_InitSort()
        {
            //获得数量.
            string sqlWhere = "";
            string sql = "";
            if (SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                sqlWhere = " AND OrgNo='" + BP.Web.WebUser.OrgNo + "'";

            //求数量.
            sql = "SELECT  FK_FlowSort, WFState, COUNT(*) AS Num FROM WF_GenerWorkFlow WHERE 1=1 " + sqlWhere + " GROUP BY FK_FlowSort, WFState ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            //求内容.
            sql = "SELECT No,Name, 0 as WFSta2, 0 as WFSta3, 0 as WFSta5 FROM WF_FlowSort WHERE 1=1 " + sqlWhere + " ORDER BY Idx ";
            DataTable dtSort = DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                dtSort.Columns[0].ColumnName = "No";
                dtSort.Columns[1].ColumnName = "Name";
                dtSort.Columns[2].ColumnName = "WFSta2";
                dtSort.Columns[3].ColumnName = "WFSta3";
                dtSort.Columns[4].ColumnName = "WFSta5";
            }

            // 给状态赋值.
            foreach (DataRow dr in dtSort.Rows)
            {
                string flowNo = dr[0] as string;
                foreach (DataRow mydr in dt.Rows)
                {
                    string fk_flow = mydr[0].ToString();
                    if (fk_flow.Equals(flowNo) == false)
                        continue;

                    int wfstate = int.Parse(mydr[1].ToString());
                    int Num = int.Parse(mydr[2].ToString());
                    if (wfstate == 2)
                        dr["WFSta2"] = Num;
                    if (wfstate == 3)
                        dr["WFSta3"] = Num;
                    if (wfstate == 5)
                        dr["WFSta5"] = Num;
                    break;
                }
            }
            return BP.Tools.Json.ToJson(dtSort);
        }
        public string Flows_Init()
        {
            //获得流程实例的数量.
            string sqlWhere = "";
            string sql = "";
            if (SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                sqlWhere = " AND OrgNo='" + BP.Web.WebUser.OrgNo + "'";

            //求流程数量.
            sql = "SELECT FK_Flow,WFState, COUNT(*) AS Num FROM WF_GenerWorkFlow WHERE 1=1 " + sqlWhere + " GROUP BY FK_Flow, WFState ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            //求流程内容.
            sql = "SELECT No,Name,WorkModel, FK_FlowSort, 0 as WFSta2, 0 as WFSta3, 0 as WFSta5 FROM WF_Flow WHERE 1=1 " + sqlWhere + " ORDER BY Idx ";
            DataTable dtFlow = DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                dtFlow.Columns[0].ColumnName = "No";
                dtFlow.Columns[1].ColumnName = "Name";
                dtFlow.Columns[2].ColumnName = "WorkModel";
                dtFlow.Columns[3].ColumnName = "AtPara";
                dtFlow.Columns[4].ColumnName = "FK_FlowSort";
                dtFlow.Columns[5].ColumnName = "WFSta2";
                dtFlow.Columns[6].ColumnName = "WFSta3";
                dtFlow.Columns[7].ColumnName = "WFSta5";
            }

            // 给状态赋值.
            foreach (DataRow dr in dtFlow.Rows)
            {
                string flowNo = dr[0] as string;
                foreach (DataRow mydr in dt.Rows)
                {
                    string fk_flow = mydr[0].ToString();
                    if (fk_flow.Equals(flowNo) == false)
                        continue;

                    int wfstate = int.Parse(mydr[1].ToString());
                    int Num = int.Parse(mydr[2].ToString());
                    if (wfstate == 2)
                        dr["WFSta2"] = Num;
                    if (wfstate == 3)
                        dr["WFSta3"] = Num;
                    if (wfstate == 5)
                        dr["WFSta5"] = Num;
                    break;
                }
            }
            return BP.Tools.Json.ToJson(dtFlow);
        }
        /// <summary>
        /// 流程移动.
        /// </summary>
        /// <returns></returns>
        public string Flows_Move()
        {
            string sortNo = this.GetRequestVal("SortNo");
            string[] flowNos = this.GetRequestVal("EnNos").Split(',');
            for (int i = 0; i < flowNos.Length; i++)
            {
                var flowNo = flowNos[i];

                string sql = "UPDATE WF_Flow SET FK_FlowSort ='" + sortNo + "',Idx=" + i + " WHERE No='" + flowNo + "'";
                DBAccess.RunSQL(sql);
            }
            return "流程顺序移动成功..";
        }

        public string Flows_MoveSort()
        {
            string[] ens = this.GetRequestVal("SortNos").Split(',');
            for (int i = 0; i < ens.Length; i++)
            {
                var en = ens[i];

                string sql = "UPDATE WF_FlowSort SET Idx=" + i + " WHERE No='" + en + "'";
                DBAccess.RunSQL(sql);
            }
            return "目录移动成功..";
        }
        #endregion 流程.

        #region   加载菜单 .
        /// <summary>
        /// 返回构造的JSON.
        /// </summary>
        /// <returns></returns>
        public string Default_Init()
        {
            DataSet myds = new DataSet();

            #region 构造数据容器.
            //系统表.
            MySystems systems = new MySystems();
            systems.RetrieveAll();
            DataTable dtSys = systems.ToDataTableField("System");

            //模块.
            Modules modules = new Modules();
            modules.RetrieveAll();
            DataTable dtModule = modules.ToDataTableField("Module");

            //菜单.
            Menus menus = new Menus();
            menus.RetrieveAll();
            DataTable dtMenu = menus.ToDataTableField("Menu");
            dtMenu.Columns["UrlExt"].ColumnName = "Url";
            #endregion 构造数据容器.

            #region 如果是admin.
            if (BP.Web.WebUser.IsAdmin == true && this.IsMobile == false)
            {
                #region 增加默认的系统.
                DataRow dr = dtSys.NewRow();
                dr["No"] = "Flows";
                //  dr["Name"] = "流程设计";
                dr["Icon"] = "";
                dtSys.Rows.Add(dr);

                dr = dtSys.NewRow();
                dr["No"] = "Frms";
                //    dr["Name"] = "表单设计";
                dr["Icon"] = "";
                dtSys.Rows.Add(dr);

                dr = dtSys.NewRow();
                dr["No"] = "System";
                //  dr["Name"] = "系统管理";
                dr["Icon"] = "";
                dtSys.Rows.Add(dr);
                #endregion 增加默认的系统.

                string sqlWhere = "";
                if (SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                    sqlWhere = " AND OrgNo='" + BP.Web.WebUser.OrgNo + "'";

                #region 流程树.
                string sql = "SELECT No,Name,ParentNo FROM WF_FlowSort WHERE 1=1 " + sqlWhere + " ORDER BY Idx,No ";
                DataTable dtFlowSorts = DBAccess.RunSQLReturnTable(sql);
                dtFlowSorts.TableName = "FlowTree";

                DataTable dtFlows = null;
                try
                {
                    sql = "SELECT No,Name,FK_FlowSort AS TreeNo, WorkModel, '' as Icon, '' as Url FROM WF_Flow WHERE 1=1  " + sqlWhere + " AND  FK_FlowSort!='' ORDER BY FK_FlowSort,Idx  ";
                    dtFlows = DBAccess.RunSQLReturnTable(sql);
                }
                catch (Exception ex)
                {
                    Flow fl = new Flow();
                    fl.CheckPhysicsTable();

                    sql = "SELECT No,Name,FK_FlowSort AS TreeNo, WorkModel, '' as Icon, '' as Url FROM WF_Flow WHERE 1=1  " + sqlWhere + " AND  FK_FlowSort!='' ORDER BY FK_FlowSort,Idx  ";
                    dtFlows = DBAccess.RunSQLReturnTable(sql);
                }

                dtFlows.TableName = "Flows";
                #endregion 流程树.

                #region 表单树.
                sql = "SELECT No,Name, ParentNo, '' as Icon FROM Sys_FormTree WHERE 1=1 " + sqlWhere + "  ORDER BY Idx,No ";
                DataTable dtFrmSorts = DBAccess.RunSQLReturnTable(sql);
                dtFrmSorts.TableName = "FrmTree";

                sql = "SELECT No,Name,FK_FormTree AS TreeNo,  '' as Icon, FrmType FROM Sys_MapData WHERE 1=1 " + sqlWhere + " AND (FK_FormTree!='' AND FK_FormTree IS NOT NULL)   ORDER BY FK_FormTree,Idx  ";
                DataTable dtFrms = DBAccess.RunSQLReturnTable(sql);
                dtFrms.TableName = "Frms";
                #endregion 表单树.

                //加入菜单信息.
                myds.Tables.Add(dtFlowSorts);
                myds.Tables.Add(dtFlows);
                myds.Tables.Add(dtFrmSorts);
                myds.Tables.Add(dtFrms);

                // BP.WF.XML.AdminMenus ens = new XML.AdminMenus();
                DataSet dsAdminMenus = new DataSet();

                //模版
                string file = SystemConfig.PathOfWebApp + "DataUser\\XML\\AdminMenu2021.xml";

                //获得文件.
                dsAdminMenus.ReadXml(file);

                //增加模块.
                DataTable dtGroup = dsAdminMenus.Tables["Group"];
                foreach (DataRow dtRow in dtGroup.Rows)
                {
                    DataRow drModel = dtModule.NewRow();
                    drModel["No"] = dtRow["No"];
                    drModel["Name"] = dtRow["Name"];
                    drModel["SystemNo"] = "System";
                    dtModule.Rows.Add(drModel);
                }

                //增加菜单.
                DataTable dtItem = dsAdminMenus.Tables["Item"];
                foreach (DataRow dtRow in dtItem.Rows)
                {
                    DataRow drMenu = dtMenu.NewRow();
                    drMenu["No"] = dtRow["No"];
                    drMenu["Name"] = dtRow["Name"];
                    drMenu["ModuleNo"] = dtRow["GroupNo"];
                    drMenu["Url"] = dtRow["Url"];
                    drMenu["Icon"] = dtRow["Icon"];
                    drMenu["SystemNo"] = "System";
                    dtMenu.Rows.Add(drMenu);
                }
            }
            #endregion 如果是admin.

            myds.Tables.Add(dtSys);
            myds.Tables.Add(dtModule);
            myds.Tables.Add(dtMenu);


            //   myds.WriteXml("c:\\11.xml");

            return BP.Tools.Json.ToJson(myds);
        }
        #endregion   加载菜单.



    }
}
