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
using BP.GPM;
using BP.WF.Template;
using BP.GPM.Menu2020;
using System.Collections;
using BP.WF.Port.Admin2Group;
using BP.Tools;
using System.Security.Cryptography;
using BP.GPM.Home;
using BP.Difference;
using BP.WF.XML;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_Portal : DirectoryPageBase
    {
        public string PageID
        {
            get
            {
                string pageID = this.GetRequestVal("PageID");
                if (DataType.IsNullOrEmpty(pageID) == true)
                    pageID = "Home";

                return pageID;
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string Home_Init()
        {
            BP.GPM.Home.WindowTemplates ens = new BP.GPM.Home.WindowTemplates();
            ens.Retrieve(WindowTemplateAttr.PageID, this.PageID, "Idx");
            if (ens.Count == 0 && this.PageID.Equals("Home") == true)
            {
                ens.InitHomePageData(); //初始化数据.
                ens.Retrieve(WindowTemplateAttr.PageID, this.PageID, "Idx");
            }

            //初始化数据.
            ens.InitDocs();

            DataTable dt = ens.ToDataTableField();
            dt.TableName = "WindowTemplates";

            return BP.Tools.Json.ToJson(dt);
        }
        public string Home_DoMove()
        {
            string[] mypks = this.MyPK.Split(',');
            for (int i = 0; i < mypks.Length; i++)
            {
                string str = mypks[i];
                if (str == null || str == "")
                    continue;

                string sql = "UPDATE GPM_WindowTemplate SET Idx=" + i + " WHERE No='" + str + "' AND PageID='" + this.PageID + "' ";
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
                return "url@Apps.htm?UserNo=" + this.UserNo + "&SID=" + SID;

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
        public string Login_VerifyState()
        {
            if (!String.IsNullOrEmpty(HttpContextHelper.RequestCookieGet(this.ToString() + "_Login_Error", "CCS")))
            {
                return "err@" + Login_VerifyCode();
            }

            return "无需验证";
        }

        public string Login_VerifyCode()
        {
            return Verify.DrawImage(5, this.ToString(), "_Login_Error", "_VerifyCode");
        }
        /// <summary>
        /// 登录.
        /// </summary>
        /// <returns></returns>
        public string Login_Submit()
        {
            try
            {

                string verifyCode = this.GetRequestVal("VerifyCode");

                string checkVerifyCode = HttpUtility.UrlDecode(HttpContextHelper.RequestCookieGet(this.ToString() + "_VerifyCode", "CCS"));
                string strMd5 = string.IsNullOrEmpty(verifyCode) ? "" : Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(verifyCode)));

                string login_Error = HttpContextHelper.RequestCookieGet(this.ToString() + "_Login_Error", "CCS");

                if (string.IsNullOrEmpty(login_Error) == true && string.IsNullOrEmpty(verifyCode) == false)
                    return "err@错误的验证状态.";

                if (string.IsNullOrEmpty(login_Error) == false && checkVerifyCode != strMd5)
                    return "err@验证码错误.";

                var ccsCks = HttpContext.Current.Request.Cookies["CCS"];
                if (ccsCks != null)
                {
                    ccsCks.Expires = DateTime.Today.AddDays(-1);
                    HttpContextHelper.Response.Cookies.Add(ccsCks);
                    HttpContextHelper.Request.Cookies.Remove("CCS");
                }

                string userNo = this.GetRequestVal("TB_No");
                if (userNo == null)
                    userNo = this.GetRequestVal("TB_UserNo");

                userNo = userNo.Trim();

                string pass = this.GetRequestVal("TB_PW");
                if (pass == null)
                    pass = this.GetRequestVal("TB_Pass");

                pass = pass.Trim();
                //pass = HttpUtility.UrlDecode(pass,Encoding.UTF8);

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

                BP.Port.Emp emp = new BP.Port.Emp();
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
                        {
                            return "err@用户名或者密码错误.";
                            //HttpContextHelper.AddCookie("CCS", this.ToString() + "_Login_Error", this.ToString() + "_Login_Error");
                        }

                        emp.No = no;
                        int i = emp.RetrieveFromDBSources();
                        if (i == 0)
                        {
                            //HttpContextHelper.AddCookie("CCS", this.ToString() + "_Login_Error", this.ToString() + "_Login_Error");
                            return "err@用户名或者密码错误.";
                        }
                    }
                    
                    if (DBAccess.IsExitsTableCol("Port_Emp", "Tel") == true)
                    {
                        /*如果包含Name列,就检查Name是否存在.*/
                        Paras ps = new Paras();
                        ps.SQL = "SELECT No FROM Port_Emp WHERE Tel=" + SystemConfig.AppCenterDBVarStr + "Tel";
                        ps.Add("Tel", userNo);
                        string no = DBAccess.RunSQLReturnStringIsNull(ps, null);
                        if (no == null)
                        {
                            //HttpContextHelper.AddCookie("CCS", this.ToString() + "_Login_Error", this.ToString() + "_Login_Error");
                            return "err@用户名或者密码错误.";
                        }

                        emp.No = no;
                        int i = emp.RetrieveFromDBSources();
                        if (i == 0)
                        {
                            //HttpContextHelper.AddCookie("CCS", this.ToString() + "_Login_Error", this.ToString() + "_Login_Error");
                            return "err@用户名或者密码错误.";
                        }
                    }
                    else
                    {
                        //HttpContextHelper.AddCookie("CCS", this.ToString() + "_Login_Error", this.ToString() + "_Login_Error");
                        return "err@用户名或者密码错误.";
                    }
                }

                if (emp.CheckPass(pass) == false)
                {
                    //HttpContextHelper.AddCookie("CCS", this.ToString() + "_Login_Error", this.ToString() + "_Login_Error");
                    return "err@用户名或者密码错误.";
                }

                if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
                {
                    //调用登录方法.
                    BP.WF.Dev2Interface.Port_Login(emp.UserID);
                    if (DBAccess.IsExitsTableCol("Port_Emp", "EmpSta") == true)
                    {
                        string sql = "SELECT EmpSta FROM Port_Emp WHERE No='"+emp.No+"'";
                        if (DBAccess.RunSQLReturnValInt(sql,1) == 1)
                            return "err@该用户已经被禁用.";
                    }

                    #region 生成sid.
                    //生成SID,并写入数据.
                    string sid = DBAccess.GenerGUID();
                    var i= DBAccess.RunSQL("UPDATE WF_Emp SET Token='" + sid + "' WHERE No='" + emp.UserID + "'");
                    if (i == 0)
                    {
                        BP.WF.Port.WFEmp em = new BP.WF.Port.WFEmp();
                        em.No = emp.No;
                        em.FK_Dept = BP.Web.WebUser.FK_Dept;
                        em.Name = Web.WebUser.Name;
                        em.Token = sid;
                        em.Insert();
                    }
                    if (DBAccess.IsView("Port_Emp") == false)
                    {
                        if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                            DBAccess.RunSQL("UPDATE Port_Emp SET SID='" + sid + "' WHERE UserID='" + emp.UserID + "' AND OrgNo='" + emp.OrgNo + "'");
                        else
                            DBAccess.RunSQL("UPDATE Port_Emp SET SID='" + sid + "' WHERE No='" + emp.No + "'");
                    }

                    WebUser.SID = sid;
                    emp.SID = sid;
                    return "url@Default.htm?SID=" + emp.SID + "&UserNo=" + emp.UserID;
                    #endregion 生成sid.

                }

                //获得当前管理员管理的组织数量.
                OrgAdminers adminers = null;

                //查询他管理多少组织.
                adminers = new OrgAdminers();
                adminers.Retrieve(OrgAdminerAttr.FK_Emp, emp.UserID);
                if (adminers.Count == 0)
                {
                    BP.WF.Port.Admin2Group.Orgs orgs = new Orgs();
                    int i = orgs.Retrieve("Adminer", this.GetRequestVal("TB_No"));
                    if (i == 0)
                    {
                        //调用登录方法.
                        BP.WF.Dev2Interface.Port_Login(emp.UserID, null, emp.OrgNo);
                        return "url@Default.htm?SID=" + emp.SID + "&UserNo=" + emp.UserID + "&OrgNo=" + emp.OrgNo;
                    }

                    foreach (BP.WF.Port.Admin2Group.Org org in orgs)
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
                if (Glo.CCBPMRunModel == CCBPMRunModel.GroupInc)
                {
                    BP.WF.Port.WFEmp em = new BP.WF.Port.WFEmp();
                    em.No = emp.No;

                    if (em.RetrieveFromDBSources() == 0)
                    {
                        em.FK_Dept = BP.Web.WebUser.FK_Dept;
                        em.Name = Web.WebUser.Name;
                        em.Token = emp.SID;
                        em.Insert();
                    }
                    else
                    {
                        DBAccess.RunSQL("UPDATE WF_Emp SET Token='" + emp.SID + "' WHERE No='" + emp.No + "'");

                    }
                }
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
                sqlWhere = "   OrgNo='" + BP.Web.WebUser.OrgNo + "' AND No!='" + WebUser.OrgNo + "'";
            else
                sqlWhere = "   No!='100' ";


            //求内容.
            sql = "SELECT No,Name FROM Sys_FormTree WHERE  " + sqlWhere + " ORDER BY Idx ";
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
            DataTable dtFlow = null;
            try
            {
                dtFlow = DBAccess.RunSQLReturnTable(sql);
            }
            catch (Exception ex)
            {
                MapData md = new MapData();
                md.CheckPhysicsTable();
                dtFlow = DBAccess.RunSQLReturnTable(sql);
            }

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
            //求数量.
            string sqlWhere = "";
            if (SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                sqlWhere = "   OrgNo='" + BP.Web.WebUser.OrgNo + "' AND WFState>0 ";
            else
                sqlWhere = " WFState>0 ";


            string sql = "SELECT  FK_FlowSort, WFState, COUNT(*) AS Num FROM WF_GenerWorkFlow WHERE " + sqlWhere + " GROUP BY FK_FlowSort, WFState ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            //求内容. 
            if (SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                sqlWhere = "   OrgNo='" + BP.Web.WebUser.OrgNo + "' AND No!='" + WebUser.OrgNo + "'";
            else
                sqlWhere = "   ParentNo!='0' ";
            sql = "SELECT No,Name, 0 as WFSta2, 0 as WFSta3, 0 as WFSta5 FROM WF_FlowSort WHERE  " + sqlWhere + " ORDER BY Idx ";
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
                //dtFlow.Columns[3].ColumnName = "AtPara";
                dtFlow.Columns[3].ColumnName = "FK_FlowSort";
                dtFlow.Columns[4].ColumnName = "WFSta2";
                dtFlow.Columns[5].ColumnName = "WFSta3";
                dtFlow.Columns[6].ColumnName = "WFSta5";
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
        /// 获得菜单:权限.
        /// </summary>
        /// <returns></returns>
        public string Default_InitExt()
        {
            string pkval = BP.Web.WebUser.No + "_Menus";
            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                pkval += "_" + BP.Web.WebUser.OrgNo;

            string docs = DBAccess.GetBigTextFromDB("Sys_UserRegedit", "MyPK", pkval, "BigDocs");
            if (DataType.IsNullOrEmpty(docs) ==false)
                return docs; 

            //系统表.
            MySystems systems = new MySystems();
            systems.RetrieveAll();
            MySystems systemsCopy = new MySystems();

            //模块.
            Modules modules = new Modules();
            modules.RetrieveAll();
            Modules modulesCopy = new Modules();

            //菜单.
            Menus menus = new Menus();
            menus.RetrieveAll();
            Menus menusCopy = new Menus();

            //权限中心.
            BP.GPM.PowerCenters pcs = new BP.GPM.PowerCenters();
            pcs.RetrieveAll();

            string mydepts = "" + WebUser.FK_Dept + ","; //我的部门.
            string mystas = ""; //我的岗位.

            DataTable mydeptsDT = DBAccess.RunSQLReturnTable("SELECT FK_Dept,FK_Station FROM Port_DeptEmpStation WHERE FK_Emp='" + WebUser.No + "'");
            foreach (DataRow dr in mydeptsDT.Rows)
            {
                mydepts += dr[0].ToString() + ",";
                mystas += dr[1].ToString() + ",";
            }

            #region 1.0 首先解决系统权限问题.
            //首先解决系统的权限.
            string ids = "";
            foreach (MySystem item in systems)
            {
                //找到关于系统的控制权限集合.
                PowerCenters mypcs = pcs.GetEntitiesByKey(PowerCenterAttr.CtrlPKVal, item.No) as PowerCenters;
                //如果没有权限控制的描述，就默认有权限.
                if (mypcs == null)
                {
                    systemsCopy.AddEntity(item);
                    continue;
                }

                //控制遍历权限.
                foreach (PowerCenter pc in mypcs)
                {
                    if (pc.CtrlModel.Equals("Anyone") == true)
                    {
                        systemsCopy.AddEntity(item);
                        break;
                    }
                    if (pc.CtrlModel.Equals("Adminer") == true && BP.Web.WebUser.No.Equals("admin") == true)
                    {
                        systemsCopy.AddEntity(item);
                        break;
                    }

                    if (pc.CtrlModel.Equals("AdminerAndAdmin2") == true && BP.Web.WebUser.IsAdmin == true)
                    {
                        systemsCopy.AddEntity(item);
                        break;
                    }
                    ids = "," + pc.IDs + ",";
                    if (pc.CtrlModel.Equals("Emps") == true && ids.Contains("," + BP.Web.WebUser.No + ",") == true)
                    {
                        systemsCopy.AddEntity(item);
                        break;
                    }

                    //是否包含部门？
                    if (pc.CtrlModel.Equals("Depts") == true && this.IsHaveIt(pc.IDs, mydepts) == true)
                    {
                        systemsCopy.AddEntity(item);
                        break;
                    }

                    //是否包含岗位？
                    if (pc.CtrlModel.Equals("Stations") == true && this.IsHaveIt(pc.IDs, mystas) == true)
                    {
                        systemsCopy.AddEntity(item);
                        break;
                    }

                    //SQL？
                    if (pc.CtrlModel.Equals("SQL") == true)
                    {
                        string sql = BP.WF.Glo.DealExp(pc.IDs, null, "");
                        if (DBAccess.RunSQLReturnValFloat(sql) > 0)
                        {
                            systemsCopy.AddEntity(item);
                        }
                        break;
                    }
                }
            }
            #endregion 首先解决系统权限问题.

            #region 2.0 根据求出的系统集合处理权限， 求出模块权限..
            foreach (MySystem item in systemsCopy)
            {
                foreach (Module module in modules)
                {
                    if (module.SystemNo.Equals(item.No) == false) continue;

                    //找到关于系统的控制权限集合.
                    PowerCenters mypcs = pcs.GetEntitiesByKey(PowerCenterAttr.CtrlPKVal, module.No) as PowerCenters;
                    //如果没有权限控制的描述，就默认有权限.
                    if (mypcs == null)
                    {
                        modulesCopy.AddEntity(module);
                        continue;
                    }

                    //控制遍历权限.
                    foreach (PowerCenter pc in mypcs)
                    {
                        if (pc.CtrlModel.Equals("Anyone") == true)
                        {
                            modulesCopy.AddEntity(module);
                            break;
                        }
                        if (pc.CtrlModel.Equals("Adminer") == true && BP.Web.WebUser.No.Equals("admin") == true)
                        {
                            modulesCopy.AddEntity(module);
                            break;
                        }

                        if (pc.CtrlModel.Equals("AdminerAndAdmin2") == true && BP.Web.WebUser.IsAdmin == true)
                        {
                            modulesCopy.AddEntity(module);
                            break;
                        }

                        ids = "," + pc.IDs + ",";
                        if (pc.CtrlModel.Equals("Emps") == true && ids.Contains("," + BP.Web.WebUser.No + ",") == true)
                        {
                            modulesCopy.AddEntity(module);
                            break;
                        }

                        //是否包含部门？
                        if (pc.CtrlModel.Equals("Depts") == true && this.IsHaveIt(pc.IDs, mydepts) == true)
                        {
                            modulesCopy.AddEntity(module);
                            break;
                        }

                        //是否包含岗位？
                        if (pc.CtrlModel.Equals("Stations") == true && this.IsHaveIt(pc.IDs, mystas) == true)
                        {
                            modulesCopy.AddEntity(module);
                            break;
                        }
                    }
                }
            }
            #endregion 2.0 根据求出的系统集合处理权限,求出模块权限.

            #region 3.0 根据求出的模块集合处理权限， 求出菜单权限..
            foreach (Module item in modulesCopy)
            {
                foreach (Menu menu in menus)
                {
                    if (menu.ModuleNo.Equals(item.No) == false) continue;

                    //找到关于系统的控制权限集合.
                    PowerCenters mypcs = pcs.GetEntitiesByKey(PowerCenterAttr.CtrlPKVal, menu.No) as PowerCenters;
                    //如果没有权限控制的描述，就默认有权限.
                    if (mypcs == null)
                    {
                        menusCopy.AddEntity(menu);
                        continue;
                    }

                    //控制遍历权限.
                    foreach (PowerCenter pc in mypcs)
                    {
                        if (pc.CtrlModel.Equals("Anyone") == true)
                        {
                            menusCopy.AddEntity(menu);
                            break;
                        }
                        if (pc.CtrlModel.Equals("Adminer") == true && BP.Web.WebUser.No.Equals("admin") == true)
                        {
                            menusCopy.AddEntity(menu);
                            break;
                        }

                        if (pc.CtrlModel.Equals("AdminerAndAdmin2") == true && BP.Web.WebUser.IsAdmin == true)
                        {
                            menusCopy.AddEntity(menu);
                            break;
                        }

                        ids = "," + pc.IDs + ",";
                        if (pc.CtrlModel.Equals("Emps") == true && ids.Contains("," + BP.Web.WebUser.No + ",") == true)
                        {
                            menusCopy.AddEntity(menu);
                            break;
                        }

                        //是否包含部门？
                        if (pc.CtrlModel.Equals("Depts") == true && this.IsHaveIt(pc.IDs, mydepts) == true)
                        {
                            menusCopy.AddEntity(menu);
                            break;
                        }

                        //是否包含岗位？
                        if (pc.CtrlModel.Equals("Stations") == true && this.IsHaveIt(pc.IDs, mystas) == true)
                        {
                            menusCopy.AddEntity(menu);
                            break;
                        }
                    }
                }
            }
            #endregion 2.0 根据求出的系统集合处理权限,求出模块权限.

            #region 组装数据.
            DataSet ds = new DataSet();

            ds.Tables.Add(systemsCopy.ToDataTableField("System"));
            ds.Tables.Add(modulesCopy.ToDataTableField("Module"));

            DataTable dtMenu = menusCopy.ToDataTableField("Menu");
            dtMenu.Columns["UrlExt"].ColumnName = "Url";
            ds.Tables.Add(dtMenu);
            #endregion 组装数据.

            string json = BP.Tools.Json.ToJson(ds);
            DBAccess.SaveBigTextToDB(json, "Sys_UserRegedit", "MyPK", pkval, "BigDocs");

            try
            {
                DBAccess.RunSQL("UPDATE Sys_UserRegedit SET FK_Emp='" + BP.Web.WebUser.No + "', CfgKey='Menus',OrgNo='" + BP.Web.WebUser.OrgNo + "' WHERE MyPK='" + pkval + "'");
            }
            catch (Exception ex)
            {
                UserRegedit ur = new UserRegedit();
                ur.CheckPhysicsTable();
            }

            return json;

        }
        /// <summary>
        /// 比较两个字符串是否有交集
        /// </summary>
        /// <param name="ids1"></param>
        /// <param name="ids2"></param>
        /// <returns></returns>
        public bool IsHaveIt(string ids1, string ids2)
        {
            if (DataType.IsNullOrEmpty(ids1) == true)
                return false;
            if (DataType.IsNullOrEmpty(ids2) == true)
                return false;

            string[] str1s = ids1.Split(',');
            string[] str2s = ids2.Split(',');

            foreach (string str1 in str1s)
            {
                if (str1 == "" || str1 == null)
                    continue;

                foreach (string str2 in str2s)
                {
                    if (str2 == "" || str2 == null)
                        continue;

                    if (str2.Equals(str1) == true)
                        return true;
                }
            }
            return false;
        }
        public string Default_LogOut()
        {
            BP.Web.WebUser.Exit();

            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                return "http://passport.ccbpm.cn/";

            return "Login.htm?DoType=Logout";
        }
        /// <summary>
        /// 返回构造的JSON.
        /// </summary>
        /// <returns></returns>
        public string Default_Init()
        {
            //如果是admin. 
            if (BP.Web.WebUser.IsAdmin == true && this.IsMobile == false)
            {
            }
            else
            {
                return Default_InitExt();
            }

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

            #region 把数据加入里面去.
            myds.Tables.Add(dtSys);
            myds.Tables.Add(dtModule);
            myds.Tables.Add(dtMenu);
            #endregion 把数据加入里面去.

            #region 如果是admin.
            if (BP.Web.WebUser.IsAdmin == true && this.IsMobile == false)
            {
                #region 增加默认的系统.
                DataRow dr = dtSys.NewRow();
                dr["No"] = "Flows";
                dr["Name"] = "流程设计";
                dr["Icon"] = "";
                dtSys.Rows.Add(dr);

                dr = dtSys.NewRow();
                dr["No"] = "Frms";
                dr["Name"] = "表单设计";
                dr["Icon"] = "";
                dtSys.Rows.Add(dr);

                dr = dtSys.NewRow();
                dr["No"] = "System";
                dr["Name"] = "系统管理";
                dr["Icon"] = "";
                dtSys.Rows.Add(dr);
                #endregion 增加默认的系统.

                string sqlWhere = "";
                if (SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                    sqlWhere = " AND OrgNo='" + BP.Web.WebUser.OrgNo + "'";

                #region 流程树.
                string sql = "SELECT No,Name,ParentNo FROM WF_FlowSort WHERE 1=1 " + sqlWhere + " ORDER BY Idx,No ";
                DataTable dtFlowSorts = DBAccess.RunSQLReturnTable(sql);
                dtFlowSorts.Columns[0].ColumnName = "No";
                dtFlowSorts.Columns[1].ColumnName = "Name";
                dtFlowSorts.Columns[2].ColumnName = "ParentNo";
                dtFlowSorts.TableName = "FlowTree";

                //没有数据就预制数据.
                if (dtFlowSorts.Rows.Count == 0)
                {
                    FlowSort fs = new FlowSort();
                    fs.ParentNo = "100";
                    fs.Name = "流程根目录";
                    fs.No = WebUser.OrgNo;
                    fs.OrgNo = WebUser.OrgNo;
                    fs.Insert();

                    fs.No = DBAccess.GenerGUID();// "101";
                    fs.Name = "业务流程目录1";
                    fs.ParentNo = WebUser.OrgNo;
                    fs.Idx = 0;
                    fs.OrgNo = WebUser.OrgNo;
                    fs.Insert();

                    fs.No = DBAccess.GenerGUID(); // "102";
                    fs.Name = "业务流程目录2";
                    fs.ParentNo = WebUser.OrgNo;
                    fs.Idx = 2;
                    fs.OrgNo = WebUser.OrgNo;
                    fs.Insert();
                }

                DataTable dtFlows = null;
                try
                {
                    if (SystemConfig.AppCenterDBType == DBType.Oracle)
                        sql = "SELECT No,Name,FK_FlowSort AS TreeNo, WorkModel, '' as Icon, '' as Url FROM WF_Flow WHERE 1=1  " + sqlWhere + " AND  FK_FlowSort IS NOT NULL ORDER BY FK_FlowSort,Idx  ";
                    else
                        sql = "SELECT No,Name,FK_FlowSort AS TreeNo, WorkModel, '' as Icon, '' as Url FROM WF_Flow WHERE 1=1  " + sqlWhere + " AND  FK_FlowSort !='' ORDER BY FK_FlowSort,Idx ";
                    dtFlows = DBAccess.RunSQLReturnTable(sql);
                }
                catch (Exception ex)
                {
                    Flow fl = new Flow();
                    fl.CheckPhysicsTable();

                    if (SystemConfig.AppCenterDBType == DBType.Oracle)
                        sql = "SELECT No,Name,FK_FlowSort AS TreeNo, WorkModel, '' as Icon, '' as Url FROM WF_Flow WHERE 1=1  " + sqlWhere + " AND  FK_FlowSort IS NOT NULL ORDER BY FK_FlowSort,Idx  ";
                    else
                        sql = "SELECT No,Name,FK_FlowSort AS TreeNo, WorkModel, '' as Icon, '' as Url FROM WF_Flow WHERE 1=1  " + sqlWhere + " AND  FK_FlowSort !='' ORDER BY FK_FlowSort,Idx  ";
                    dtFlows = DBAccess.RunSQLReturnTable(sql);
                }
                dtFlows.Columns[0].ColumnName = "No";
                dtFlows.Columns[1].ColumnName = "Name";
                dtFlows.Columns[2].ColumnName = "TreeNo";
                dtFlows.Columns[3].ColumnName = "WorkModel";
                dtFlows.Columns[4].ColumnName = "Icon";
                dtFlows.Columns[5].ColumnName = "Url";
                dtFlows.TableName = "Flows";
                #endregion 流程树.

                #region 表单树.
                sql = "SELECT No,Name, ParentNo, '' as Icon FROM Sys_FormTree WHERE 1=1 " + sqlWhere + "  ORDER BY Idx,No ";
                DataTable dtFrmSorts = DBAccess.RunSQLReturnTable(sql);
                dtFrmSorts.Columns[0].ColumnName = "No";
                dtFrmSorts.Columns[1].ColumnName = "Name";
                dtFrmSorts.Columns[2].ColumnName = "ParentNo";
                dtFrmSorts.Columns[3].ColumnName = "Icon";
                dtFrmSorts.TableName = "FrmTree";

                //没有数据就预制数据.
                if (dtFrmSorts.Rows.Count == 0)
                {
                    if (SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                    {
                        FrmTree fs = new FrmTree();
                        fs.ParentNo = "0";
                        fs.Name = "根目录";
                        fs.No = "100";
                        fs.Insert();

                        fs.No = "101";
                        fs.Name = "业务表单目录1";
                        fs.ParentNo = "100";
                        fs.Idx = 0;
                        fs.Insert();

                        fs.No = "102";
                        fs.Name = "业务表单目录2";
                        fs.ParentNo = "100";
                        fs.Idx = 2;
                        fs.Insert();
                    }
                    else
                    {
                        FrmTree fs = new FrmTree();
                        fs.ParentNo = "100";
                        fs.Name = "根目录";
                        fs.No = WebUser.OrgNo;
                        fs.OrgNo = WebUser.OrgNo;
                        fs.Insert();

                        fs.No = DBAccess.GenerGUID(); // "101";
                        fs.Name = "业务表单目录1";
                        fs.ParentNo = WebUser.OrgNo;
                        fs.Idx = 0;
                        fs.OrgNo = WebUser.OrgNo;
                        fs.Insert();

                        fs.No = DBAccess.GenerGUID(); // "102";
                        fs.Name = "业务表单目录2";
                        fs.ParentNo = WebUser.OrgNo;
                        fs.Idx = 2;
                        fs.OrgNo = WebUser.OrgNo;
                        fs.Insert();
                    }
                }
                if (SystemConfig.AppCenterDBType == DBType.Oracle)
                    sql = "SELECT No,Name,FK_FormTree AS TreeNo,  '' as Icon, FrmType FROM Sys_MapData WHERE 1=1 " + sqlWhere + " AND (FK_FormTree IS NOT NULL)   ORDER BY FK_FormTree,Idx  ";
                else
                    sql = "SELECT No,Name,FK_FormTree AS TreeNo,  '' as Icon, FrmType FROM Sys_MapData WHERE 1=1 " + sqlWhere + " AND (FK_FormTree!='' AND FK_FormTree IS NOT NULL)   ORDER BY FK_FormTree,Idx  ";
                DataTable dtFrms = DBAccess.RunSQLReturnTable(sql);
                dtFrms.Columns[0].ColumnName = "No";
                dtFrms.Columns[1].ColumnName = "Name";
                dtFrms.Columns[2].ColumnName = "TreeNo";
                dtFrms.Columns[3].ColumnName = "Icon";
                dtFrms.Columns[4].ColumnName = "FrmType";
                dtFrms.TableName = "Frms";
                #endregion 表单树.

                //加入菜单信息.
                myds.Tables.Add(dtFlowSorts);
                myds.Tables.Add(dtFlows);
                myds.Tables.Add(dtFrmSorts);
                myds.Tables.Add(dtFrms);


                DataSet dsAdminMenus = new DataSet();
                AdminMenus mymenus = new AdminMenus();
                dsAdminMenus.ReadXml(mymenus.File);


                //增加模块.
                DataTable dtGroup = dsAdminMenus.Tables["Group"];
                foreach (DataRow dtRow in dtGroup.Rows)
                {
                    DataRow drModel = dtModule.NewRow();
                    drModel["No"] = dtRow["No"];
                    drModel["Name"] = dtRow["Name"];
                    drModel["SystemNo"] = "System";
                    drModel["Icon"] = dtRow["Icon"];
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

            //   myds.WriteXml("c:/11.xml");

            return BP.Tools.Json.ToJson(myds);
        }
        #endregion   加载菜单.


        /// <summary>
        /// 生成页面
        /// </summary>
        /// <returns></returns>
        public string LoginGenerQRCodeMobile_Init()
        {
            var url = SystemConfig.HostURL + "/FastMobilePortal/Login.htm";
            return url;
        }

    }
}
