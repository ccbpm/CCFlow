﻿using System;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.GPM;
using BP.CCFast.CCMenu;
using System.Collections;
using BP.WF.Port.Admin2Group;
using BP.Tools;
using System.Security.Cryptography;
using BP.CCFast.Portal;
using BP.Difference;
using BP.WF.XML;
using ICSharpCode.SharpZipLib.Zip;

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
            BP.CCFast.Portal.WindowTemplates ens = new BP.CCFast.Portal.WindowTemplates();
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
            ht.Add("SysNo", BP.Difference.SystemConfig.SysNo);
            ht.Add("SysName", BP.Difference.SystemConfig.SysName);

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
                return "url@/WF/Admin/DBInstall.htm";

            string doType = GetRequestVal("LoginType");
            if (DataType.IsNullOrEmpty(doType) == false && doType.Equals("Out") == true)
            {
                //清空cookie
                WebUser.Exit();
            }

            //是否需要自动登录。 这里都把cookeis的数据获取来了.
            string userNo = this.GetRequestVal("UserNo");
            string sid = this.GetRequestVal("Token");

            if (String.IsNullOrEmpty(sid) == false && String.IsNullOrEmpty(userNo) == false)
            {
                //调用登录方法.
                BP.WF.Dev2Interface.Port_Login(this.UserNo, this.SID);
                return "url@Apps.htm?UserNo=" + this.UserNo + "&Token=" + SID;

            }

            Hashtable ht = new Hashtable();
            ht.Add("SysName", BP.Difference.SystemConfig.SysName);
            ht.Add("SysNo", BP.Difference.SystemConfig.SysNo);
            ht.Add("ServiceTel", BP.Difference.SystemConfig.ServiceTel);
            ht.Add("CustomerName", BP.Difference.SystemConfig.CustomerName);
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

                

                BP.Port.Emp emp = new BP.Port.Emp();
                emp.UserID = userNo;
                if (emp.RetrieveFromDBSources() == 0)
                {
                    if (DBAccess.IsExitsTableCol("Port_Emp", "NikeName") == true)
                    {
                        /*如果包含昵称列,就检查昵称是否存在.*/
                        Paras ps = new Paras();
                        ps.SQL = "SELECT No FROM Port_Emp WHERE NikeName=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "NikeName";
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
                        ps.SQL = "SELECT No FROM Port_Emp WHERE Tel=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Tel";
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

                if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
                {
                    //调用登录方法.
                    BP.WF.Dev2Interface.Port_Login(emp.UserID);
                    if (DBAccess.IsExitsTableCol("Port_Emp", "EmpSta") == true)
                    {
                        string sql = "SELECT EmpSta FROM Port_Emp WHERE No='" + emp.No + "'";
                        if (DBAccess.RunSQLReturnValInt(sql, 1) == 1)
                            return "err@该用户已经被禁用.";
                    }
                    return "url@Default.htm?Token=" + BP.WF.Dev2Interface.Port_GenerToken(WebUser.No, "PC", 4000, true) + "&UserNo=" + emp.UserID;
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
                        BP.WF.Dev2Interface.Port_Login(emp.UserID, emp.OrgNo);
                        return "url@Default.htm?Token=" + BP.WF.Dev2Interface.Port_GenerToken(emp.UserID, "PC", 6000, true) + "&UserNo=" + emp.UserID + "&OrgNo=" + emp.OrgNo;
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
                BP.WF.Dev2Interface.Port_Login(emp.UserID, emp.OrgNo);

                string token = BP.WF.Dev2Interface.Port_GenerToken(emp.UserID, "PC", 40000, true);

                //判断是否是多个组织的情况.
                if (adminers.Count == 1)
                    return "url@Default.htm?Token=" + token + "&UserNo=" + emp.UserID + "&OrgNo=" + emp.OrgNo;

                return "url@SelectOneOrg.htm?Token=" + token + "&UserNo=" + emp.UserID + "&OrgNo=" + emp.OrgNo;
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
            if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                sqlWhere = "   OrgNo='" + BP.Web.WebUser.OrgNo + "' AND No!='" + WebUser.OrgNo + "'";
            else
                sqlWhere = "   No!='100' ";


            //求内容.
            sql = "SELECT No,Name FROM Sys_FormTree WHERE  " + sqlWhere + " ORDER BY Idx ";
            DataTable dtSort = DBAccess.RunSQLReturnTable(sql);
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
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
            if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
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

            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
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
            if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                sqlWhere = "   OrgNo='" + BP.Web.WebUser.OrgNo + "' AND WFState>0 ";
            else
                sqlWhere = " WFState>0 ";


            string sql = "SELECT  FK_FlowSort, WFState, COUNT(*) AS Num FROM WF_GenerWorkFlow WHERE " + sqlWhere + " GROUP BY FK_FlowSort, WFState ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            //求内容. 
            if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                sqlWhere = "   OrgNo='" + BP.Web.WebUser.OrgNo + "' AND No!='" + WebUser.OrgNo + "'";
            else
                sqlWhere = "   ParentNo!='0' ";
            sql = "SELECT No,Name, 0 as WFSta2, 0 as WFSta3, 0 as WFSta5 FROM WF_FlowSort WHERE  " + sqlWhere + " ORDER BY Idx ";
            DataTable dtSort = DBAccess.RunSQLReturnTable(sql);
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
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
            if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                sqlWhere = " AND OrgNo='" + BP.Web.WebUser.OrgNo + "'";

            //求流程数量.
            sql = "SELECT FK_Flow,WFState, COUNT(*) AS Num FROM WF_GenerWorkFlow WHERE 1=1 " + sqlWhere + " GROUP BY FK_Flow, WFState ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            //求流程内容.
            sql = "SELECT No,Name,WorkModel, FK_FlowSort, 0 as WFSta2, 0 as WFSta3, 0 as WFSta5 FROM WF_Flow WHERE 1=1 " + sqlWhere + " ORDER BY Idx ";
            DataTable dtFlow = DBAccess.RunSQLReturnTable(sql);
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
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

        #region 消息.

        /// <summary>
        /// 消息初始化
        /// </summary>
        /// <returns></returns>
        public string Message_Init()
        {
            //获得消息分组.
            string sql = "SELECT MsgType, Count(*) as Num FROM Sys_SMS WHERE SendTo='" + WebUser.No + "'  GROUP BY MsgType";
            DataTable groups = DBAccess.RunSQLReturnTable(sql);
            groups.TableName = "Groups";
            groups.Columns.Add("TypeName");
            //foreach (DataRow dr in groups.Rows)
            //{
            //    dr["TypeName"] = dr["MsgType"];
            //}

            //获得消息.
            sql = "SELECT MyPK, EmailTitle,EmailDoc,EmailSta, RDT,Sender, AtPara,MsgType,IsRead FROM Sys_SMS WHERE SendTo='" + WebUser.No + "' ORDER BY IsRead ";
            DataTable infos = DBAccess.RunSQLReturnTable(sql);
            infos.TableName = "Messages";

            DataSet ds = new DataSet();
            ds.Tables.Add(groups);
            ds.Tables.Add(infos);

            //返回信息.
            return BP.Tools.Json.ToJson(ds);
        }
        #endregion 消息.
        #region 通知公告.

        /// <summary>
        /// 消息初始化
        /// </summary>
        /// <returns></returns>
        public string Info_Init()
        {

            //获得消息.
            string sql = "SELECT No, Name,Docs,InfoPRI, InfoSta,RecName, RelerName,RelDeptName,RDT FROM OA_Info WHERE InfoSta=0 ORDER BY RDT ";
            DataTable infos = DBAccess.RunSQLReturnTable(sql);
            infos.TableName = "Infos";

            DataSet ds = new DataSet();
            ds.Tables.Add(infos);
            

            //返回信息.
            return BP.Tools.Json.ToJson(ds);
        }
        #endregion 通知公告
        

        #region   加载菜单 .

        /// <summary>
        /// 获得菜单:权限.
        /// </summary>
        /// <returns></returns>
        public string Default_InitExt()
        {
            string pkval = BP.Web.WebUser.No + "_Menus";
            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                pkval += "_" + BP.Web.WebUser.OrgNo;

            //string docs = DBAccess.GetBigTextFromDB("Sys_UserRegedit", "MyPK", pkval, "BigDocs");
            //if (DataType.IsNullOrEmpty(docs) == false)
            //    return docs;

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
            BP.CCFast.CCMenu.PowerCenters pcs = new BP.CCFast.CCMenu.PowerCenters();
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
                //如果被禁用了.
                if (item.IsEnable == false) continue;

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
                    if (pc.CtrlModel.Equals("Depts") == true && BP.DA.DataType.IsHaveIt(pc.IDs, mydepts) == true)
                    {
                        systemsCopy.AddEntity(item);
                        break;
                    }

                    //是否包含岗位？
                    if (pc.CtrlModel.Equals("Stations") == true && BP.DA.DataType.IsHaveIt(pc.IDs, mystas) == true)
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
                    //如果被禁用了.
                    if (module.IsEnable == false) continue;


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

                    //如果被禁用了.
                    if (menu.IsEnable == false) continue;

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
            DataTable dtSystem = systemsCopy.ToDataTableField("System");
            dtSystem.Columns.Add("IsOpen");

            //给第1个系统，第1个模块设置打开状态.
            DataTable dtModule = modulesCopy.ToDataTableField("Module");
            dtModule.Columns.Add("IsOpen");
            if (dtSystem.Rows.Count > 0)
            {
                dtSystem.Rows[0]["IsOpen"] = "true";
                string systemNo = dtSystem.Rows[0]["No"].ToString();
                foreach (DataRow dr in dtModule.Rows)
                {
                    if (dr["SystemNo"].ToString().Equals(systemNo) == false)
                        continue;

                    dr["IsOpen"] = "true";
                    break;
                }
            }

            ds.Tables.Add(dtSystem);
            ds.Tables.Add(dtModule);

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

            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
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
            dtSys.Columns.Add("IsOpen");


            //模块.
            Modules modules = new Modules();
            modules.RetrieveAll();
            DataTable dtModule = modules.ToDataTableField("Module");
            dtModule.Columns.Add("IsOpen");

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
                if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                    sqlWhere = " AND OrgNo='" + BP.Web.WebUser.OrgNo + "'";

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

            #region 让第一个系统的第1个模块的默认打开的.
            //让第一个打开.
            myds.Tables["System"].Rows[0]["IsOpen"] = "true";
            string systemNo = myds.Tables["System"].Rows[0]["No"].ToString();
            foreach (DataRow dr in myds.Tables["Module"].Rows)
            {
                if (dr["SystemNo"].ToString().Equals(systemNo) == false)
                    continue;

                dr["IsOpen"] = "true";
                break;
            }
            #endregion 让第一个系统的第1个模块的第一个菜单打开.


            return BP.Tools.Json.ToJson(myds);
        }
        #endregion   加载菜单.


        /// <summary>
        /// 生成页面
        /// </summary>
        /// <returns></returns>
        public string LoginGenerQRCodeMobile_Init()
        {
            var url =  BP.Difference.SystemConfig.HostURL + "/FastMobilePortal/Login.htm";
            return url;
        }

        #region 按照流程类别批量导出流程模板
        /// <summary>
        /// 批量导出流程模板
        /// </summary>
        /// <returns></returns>
        public string Flow_BatchExpFlowTemplate()
        {
            string flowSort = this.GetRequestVal("FK_Sort");
            string flowSortName = this.GetRequestVal("FlowSortName");
            if (DataType.IsNullOrEmpty("flowSort") == true)
                return "err@流程的类别不能为空";
            //根据流程类别获取改类别下的所有流程
            Flows flows = new Flows(flowSort);
            //在临时文件中指定一个目录
            string path =  BP.Difference.SystemConfig.PathOfTemp + flowSortName + "/";
            if (System.IO.Directory.Exists(path) == true)
                System.IO.Directory.Delete(path, true);
            else
                System.IO.Directory.CreateDirectory(path);
            foreach (Flow flow in flows)
            {
                flow.DoExpFlowXmlTemplete(path);
            }
            //生成压缩包文件
            string zipFile =  BP.Difference.SystemConfig.PathOfTemp + flowSortName + ".zip";
            try
            {
                while (System.IO.File.Exists(zipFile) == true)
                {
                    System.IO.File.Delete(zipFile);
                }
                //执行压缩.
                FastZip fz = new FastZip();
                fz.CreateZip(zipFile, path, true, "");
                //删除临时文件夹
                System.IO.Directory.Delete(path, true);
            }
            catch (Exception ex)
            {
                return "err@执行压缩出现错误:" + ex.Message + ",路径tempPath:" + path + ",zipFile=" + zipFile;
            }
            return "url@DataUser/Temp/" + flowSortName + ".zip";
        }
        #endregion 按照流程类别批量导出流程模板
        #region 按照表单类别批量导出表单模板
        /// <summary>
        /// 批量导出表单模板
        /// </summary>
        /// <returns></returns>
        public string Form_BatchExpFrmTemplate()
        {
            string frmTree = this.GetRequestVal("FK_FrmTree");
            string frmTreeName = this.GetRequestVal("FrmTreeName");
            if (DataType.IsNullOrEmpty("frmTree") == true)
                return "err@表单的类别不能为空";
            //根据流程类别获取改类别下的所有流程
            MapDatas mds = new MapDatas();
            mds.Retrieve(MapDataAttr.FK_FormTree, frmTree, MapDataAttr.Idx);
            //在临时文件中指定一个目录
            string path =  BP.Difference.SystemConfig.PathOfTemp + frmTreeName + "/";
            if (System.IO.Directory.Exists(path) == true)
                System.IO.Directory.Delete(path, true);
            else
                System.IO.Directory.CreateDirectory(path);
            foreach (MapData md in mds)
            {
                DataSet ds = BP.Sys.CCFormAPI.GenerHisDataSet_AllEleInfo(md.No);

                string file = path + md.Name + ".xml";
                ds.WriteXml(file);
            }
            //生成压缩包文件
            string zipFile =  BP.Difference.SystemConfig.PathOfTemp + frmTreeName + ".zip";
            try
            {
                while (System.IO.File.Exists(zipFile) == true)
                {
                    System.IO.File.Delete(zipFile);
                }
                //执行压缩.
                FastZip fz = new FastZip();
                fz.CreateZip(zipFile, path, true, "");
                //删除临时文件夹
                System.IO.Directory.Delete(path, true);
            }
            catch (Exception ex)
            {
                return "err@执行压缩出现错误:" + ex.Message + ",路径tempPath:" + path + ",zipFile=" + zipFile;
            }
            return "url@DataUser/Temp/" + frmTreeName + ".zip";
        }
        #endregion 按照表单类别批量导出表单模板
    }
}
