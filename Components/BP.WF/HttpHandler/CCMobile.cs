using System;
using System.Collections;
using System.Data;
using System.Text;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF.Port.Admin2Group;
using BP.Difference;
using BP.CCFast.CCMenu;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class CCMobile : DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CCMobile()
        {
            BP.Web.WebUser.SheBei = "Mobile";
        }

        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            switch (this.DoType)
            {

                case "DtlFieldUp": //字段上移
                    return "执行成功.";
                default:
                    break;
            }

            //找不不到标记就抛出异常.
            throw new Exception("@标记[" + this.DoType + "]，没有找到.");
        }
        #endregion 执行父类的重写方法.

        public string Login_Init()
        {
            BP.WF.HttpHandler.WF ace = new WF();
            return ace.Login_Init();
        }

        public string Login_Submit()
        {
            string userNo = this.GetRequestVal("TB_No");
            if (DataType.IsNullOrEmpty(userNo) == true)
                userNo = this.GetRequestVal("TB_UserNo");
            string pass = this.GetRequestVal("TB_PW");
            if (DataType.IsNullOrEmpty(pass) == true)
                pass = this.GetRequestVal("TB_Pass");
            if(SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
            {
                Emp saemp = null;
                if (DataType.IsNullOrEmpty(this.OrgNo) == false)
                {
                    string empNo = this.OrgNo + "_" + userNo;
                    saemp.No = empNo;
                    if (saemp.RetrieveFromDBSources() == 0)
                        return "err@账号" + userNo + "在组织" + this.OrgNo + "还未注册";
                }
                else
                {
                    //获取当前用户
                    Emps emps = new Emps();
                    emps.Retrieve(EmpAttr.UserID, userNo, EmpAttr.EmpSta, 1);
                    if (emps.Count == 0)
                        return "err@还未注册该账号或该账号已经禁用";
                    saemp = emps[0] as Emp;
                }
                if (saemp.CheckPass(pass) == false)
                    return "err@用户名或者密码错误.";
                //调用登录方法.
                BP.WF.Dev2Interface.Port_Login(saemp.UserID, saemp.OrgNo);
                return "url@Home.htm?Token=" + BP.WF.Dev2Interface.Port_GenerToken("PC") + "&UserNo=" + saemp.UserID;
            }
            BP.Port.Emp emp = new Emp();
            emp.UserID = userNo;
            if (emp.RetrieveFromDBSources() == 0)
            {
                if (DBAccess.IsExitsTableCol("Port_Emp", "NikeName") == true)
                {
                    /*如果包含昵称列,就检查昵称是否存在.*/
                    Paras ps = new Paras();
                    ps.SQL = "SELECT " + BP.Sys.Base.Glo.UserNo + " FROM Port_Emp WHERE NikeName=" + SystemConfig.AppCenterDBVarStr + "userNo";
                    ps.Add("userNo", userNo);
                    string no = DBAccess.RunSQLReturnStringIsNull(ps, null);
                    if (no == null)
                        return "err@用户名或者密码错误.";

                    emp.UserID = no;
                    int i = emp.RetrieveFromDBSources();
                    if (i == 0)
                        return "err@用户名或者密码错误.";
                }
                else if (DBAccess.IsExitsTableCol("Port_Emp", "Tel") == true)
                {
                    /*如果包含昵称列,就检查昵称是否存在.*/
                    Paras ps = new Paras();
                    ps.SQL = "SELECT " + BP.Sys.Base.Glo.UserNo + " FROM Port_Emp WHERE Tel=" + SystemConfig.AppCenterDBVarStr + "userNo";
                    ps.Add("userNo", userNo);
                    //string sql = "SELECT No FROM Port_Emp WHERE NikeName='" + userNo + "'";
                    string no = DBAccess.RunSQLReturnStringIsNull(ps, null);
                    if (no == null)
                        return "err@用户名或者密码错误.";

                    emp.UserID = no;
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
                if (DBAccess.IsExitsTableCol("Port_Emp", "EmpSta") == true)
                {
                    string sql = "SELECT EmpSta FROM Port_Emp WHERE No='" + emp.No + "'";
                    if (DBAccess.RunSQLReturnValInt(sql, 1) == 1)
                        return "err@该用户已经被禁用.";
                }
                return "url@/FastMobilePortal/Default.htm?Token=" + BP.WF.Dev2Interface.Port_GenerToken("PC") + "&UserNo=" + emp.UserID;
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
                return "url@/FastMobilePortal/Default.htm?Token=" + BP.WF.Dev2Interface.Port_GenerToken("PC") + "&UserNo=" + emp.UserID;
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
                    return "url@/FastMobilePortal/Default.htm?Token=" + BP.WF.Dev2Interface.Port_GenerToken("PC") + "&UserNo=" + emp.UserID + "&OrgNo=" + emp.OrgNo;
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

            string token = BP.WF.Dev2Interface.Port_GenerToken("PC");

            //判断是否是多个组织的情况.
            if (adminers.Count == 1)
                return "url@/FastMobilePortal/Default.htm?Token=" + token + "&UserNo=" + emp.UserID + "&OrgNo=" + emp.OrgNo;

            return "url@SelectOneOrg.htm?Token=" + token + "&UserNo=" + emp.UserID + "&OrgNo=" + emp.OrgNo;

            return "登录成功.";
        }


        public string Login_SubmitSingle()
        {
            string userNo = this.GetRequestVal("TB_No");
            string pass = this.GetRequestVal("TB_PW");

            BP.Port.Emp emp = new Emp();
            emp.No = userNo;
            if (emp.RetrieveFromDBSources() == 0)
            {
                if (DBAccess.IsExitsTableCol("Port_Emp", "NikeName") == true)
                {
                    /*如果包含昵称列,就检查昵称是否存在.*/
                    Paras ps = new Paras();
                    ps.SQL = "SELECT " + BP.Sys.Base.Glo.UserNo + " FROM Port_Emp WHERE NikeName=" + SystemConfig.AppCenterDBVarStr + "userNo";
                    ps.Add("userNo", userNo);
                    //string sql = "SELECT No FROM Port_Emp WHERE NikeName='" + userNo + "'";
                    string no = DBAccess.RunSQLReturnStringIsNull(ps, null);
                    if (no == null)
                        return "err@用户名或者密码错误.";

                    emp.UserID = no;
                    int i = emp.RetrieveFromDBSources();
                    if (i == 0)
                        return "err@用户名或者密码错误.";
                }
                else if (DBAccess.IsExitsTableCol("Port_Emp", "Tel") == true)
                {
                    /*如果包含昵称列,就检查昵称是否存在.*/
                    Paras ps = new Paras();
                    ps.SQL = "SELECT " + BP.Sys.Base.Glo.UserNo + " FROM Port_Emp WHERE Tel=" + SystemConfig.AppCenterDBVarStr + "userNo";
                    ps.Add("userNo", userNo);
                    //string sql = "SELECT No FROM Port_Emp WHERE NikeName='" + userNo + "'";
                    string no = DBAccess.RunSQLReturnStringIsNull(ps, null);
                    if (no == null)
                        return "err@用户名或者密码错误.";

                    emp.UserID = no;
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

            //调用登录方法.
            BP.WF.Dev2Interface.Port_Login(emp.UserID);


            return "登录成功.";
        }
        /// <summary>
        /// 会签列表
        /// </summary>
        /// <returns></returns>
        public string HuiQianList_Init()
        {
            WF wf = new WF();
            return wf.HuiQianList_Init();
        }

        public string GetUserInfo()
        {
            if (WebUser.No == null)
                return "{err:'nologin'}";

            StringBuilder append = new StringBuilder();
            append.Append("{");
            string userPath = SystemConfig.PathOfWebApp + "DataUser/UserIcon/";
            string userIcon = userPath + BP.Web.WebUser.No + "Biger.png";
            if (System.IO.File.Exists(userIcon))
            {
                append.Append("UserIcon:'" + BP.Web.WebUser.No + "Biger.png'");
            }
            else
            {
                append.Append("UserIcon:'DefaultBiger.png'");
            }
            append.Append(",UserName:'" + BP.Web.WebUser.Name + "'");
            append.Append(",UserDeptName:'" + BP.Web.WebUser.FK_DeptName + "'");
            append.Append("}");
            return append.ToString();
        }
        public string StartGuide_MulitSend()
        {
            WF_MyFlow en = new WF_MyFlow();
            return en.StartGuide_MulitSend();
        }
        public string Home_Init()
        {
            Hashtable ht = new Hashtable();
            ht.Add("UserNo", BP.Web.WebUser.No);
            ht.Add("UserName", BP.Web.WebUser.Name);

            //系统名称.
            ht.Add("SysName", SystemConfig.SysName);
            ht.Add("CustomerName", SystemConfig.CustomerName);

            ht.Add("Todolist_EmpWorks", BP.WF.Dev2Interface.Todolist_EmpWorks);
            ht.Add("Todolist_Runing", BP.WF.Dev2Interface.Todolist_Runing);
            ht.Add("Todolist_Complete", BP.WF.Dev2Interface.Todolist_Complete);
            ht.Add("Todolist_CCWorks", BP.WF.Dev2Interface.Todolist_CCWorks);

            ht.Add("Todolist_HuiQian", BP.WF.Dev2Interface.Todolist_HuiQian); //会签数量.
            ht.Add("Todolist_Drafts", BP.WF.Dev2Interface.Todolist_Draft); //会签数量.

            return BP.Tools.Json.ToJsonEntityModel(ht);
        }
        public string Default_Init()
        {
            DataSet ds = new DataSet();
            //获取最近发起的流程
            string sql = "";
            int top = GetRequestValInt("Top");
            if (top == 0) top = 8;

            switch (BP.Difference.SystemConfig.AppCenterDBType)
            {
                case DBType.MSSQL:
                    sql = " SELECT TOP " + top + "  FK_Flow,FlowName,F.Icon FROM WF_GenerWorkFlow G ,WF_Flow F WHERE  F.IsCanStart=1 AND F.No=G.FK_Flow AND Starter='" + WebUser.No + "' ORDER By SendDT DESC";
                    break;
                case DBType.MySQL:
                case DBType.PostgreSQL:
                case DBType.UX:
                    sql = " SELECT DISTINCT FK_Flow,FlowName,F.Icon FROM WF_GenerWorkFlow G ,WF_Flow F WHERE  F.IsCanStart=1 AND F.No=G.FK_Flow AND Starter='" + WebUser.No + "'  Order By SendDT  limit  " + top;
                    break;
                case DBType.Oracle:
                case DBType.DM:
                case DBType.KingBaseR3:
                case DBType.KingBaseR6:
                    sql = " SELECT * FROM (SELECT DISTINCT FK_Flow as \"FK_Flow\",FlowName as \"FlowName\",F.Icon FROM WF_GenerWorkFlow G ,WF_Flow F WHERE F.IsCanStart=1 AND F.No=G.FK_Flow AND Starter='" + WebUser.No + "' GROUP BY FK_Flow,FlowName,ICON Order By SendDT) WHERE  rownum <=" + top;
                    break;
                default:
                    throw new Exception("err@系统暂时还未开发使用" + BP.Difference.SystemConfig.AppCenterDBType + "数据库");
            }
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Flows";
            ds.Tables.Add(dt);
            //应用中心
            MySystems systems = new MySystems();
            systems.RetrieveAll();
            MySystems systemsCopy = new MySystems();
            //权限中心.
            BP.CCFast.CCMenu.PowerCenters pcs = new BP.CCFast.CCMenu.PowerCenters();
            pcs.RetrieveAll();
            string mydepts = "" + WebUser.FK_Dept + ","; //我的部门.
            string mystas = ""; //我的角色.
            DataTable mydeptsDT = DBAccess.RunSQLReturnTable("SELECT FK_Dept,FK_Station FROM Port_DeptEmpStation WHERE FK_Emp='" + WebUser.No + "'");
            foreach (DataRow dr in mydeptsDT.Rows)
            {
                mydepts += dr[0].ToString() + ",";
                mystas += dr[1].ToString() + ",";
            }

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

                    //是否包含角色？
                    if (pc.CtrlModel.Equals("Stations") == true && BP.DA.DataType.IsHaveIt(pc.IDs, mystas) == true)
                    {
                        systemsCopy.AddEntity(item);
                        break;
                    }

                    //SQL？
                    if (pc.CtrlModel.Equals("SQL") == true)
                    {
                        sql = BP.WF.Glo.DealExp(pc.IDs, null, "");
                        if (DBAccess.RunSQLReturnValFloat(sql) > 0)
                        {
                            systemsCopy.AddEntity(item);
                        }
                        break;
                    }
                }
            }
            ds.Tables.Add(systemsCopy.ToDataTableField("Systems"));
            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public string Home_Init_WorkCount()
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT  TSpan as No, '' as Name, COUNT(WorkID) as Num, FROM WF_GenerWorkFlow WHERE Emps LIKE '%" + SystemConfig.AppCenterDBVarStr + "Emps%' GROUP BY TSpan";
            ps.Add("Emps", WebUser.No);
            //string sql = "SELECT  TSpan as No, '' as Name, COUNT(WorkID) as Num, FROM WF_GenerWorkFlow WHERE Emps LIKE '%" + WebUser.No + "%' GROUP BY TSpan";
            DataSet ds = new DataSet();
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            ds.Tables.Add(dt);

            dt.Columns[0].ColumnName = "TSpan";
            dt.Columns[1].ColumnName = "Num";

            string sql = "SELECT IntKey as No, Lab as Name FROM " + BP.Sys.Base.Glo.SysEnum() + " WHERE EnumKey='TSpan'";
            DataTable dt1 = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                foreach (DataRow mydr in dt1.Rows)
                {

                }
            }

            return BP.Tools.Json.ToJson(dt);
        }
        public string MyFlow_Init()
        {
            BP.WF.HttpHandler.WF_MyFlow wfPage = new BP.WF.HttpHandler.WF_MyFlow();
            return wfPage.MyFlow_Init();
        }

        public string Runing_Init()
        {
            WF wfPage = new WF();
            return wfPage.Runing_Init();
        }

        /// <summary>
        /// 新版本.
        /// </summary>
        /// <returns></returns>
        public string Todolist_Init()
        {
            string fk_node = this.GetRequestVal("FK_Node");
            string showWhat = this.GetRequestVal("ShowWhat");
            string orderBy = this.GetRequestVal("OrderBy");
            DataTable dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable(WebUser.No, this.FK_Node, showWhat,null,null, orderBy);
            dt.Columns.Add("WFStateLabel");
            foreach (DataRow dr in dt.Rows)
            {
                int sta = int.Parse(dr["WFState"].ToString());
                dr["WFStateLabel"] = "待办";
                if (sta == 5)
                {
                    dr["WFStateLabel"] = "退回";
                }
                if (sta == 4)
                {
                    string atpara = dr["AtPara"].ToString();
                    if (atpara.Contains("@HungupSta=2") == true)
                        dr["WFStateLabel"] = "拒绝挂起";
                    if (atpara.Contains("@HungupSta=1") == true)
                        dr["WFStateLabel"] = "同意挂起";
                    if (atpara.Contains("@HungupSta=0") == true)
                        dr["WFStateLabel"] = "挂起等待审批";
                }

            }


            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 查询已完成.
        /// </summary>
        /// <returns></returns>
        public string Complete_Init()
        {
            DataTable dt = null;
            dt = BP.WF.Dev2Interface.DB_FlowComplete();
            return BP.Tools.Json.ToJson(dt);
        }
        public string DB_GenerReturnWorks()
        {
            /* 如果工作节点退回了*/
            BP.WF.ReturnWorks rws = new BP.WF.ReturnWorks();
            rws.Retrieve(BP.WF.ReturnWorkAttr.ReturnToNode, this.FK_Node, BP.WF.ReturnWorkAttr.WorkID, this.WorkID, BP.WF.ReturnWorkAttr.RDT);
            StringBuilder append = new StringBuilder();
            append.Append("[");

            return BP.Tools.Json.ToJson(rws.ToDataTableField());
        }

        public string Start_Init()
        {
            WF wfPage = new WF();
            return wfPage.Start_Init();
        }

        public string HandlerMapExt()
        {
            WF_CCForm en = new WF_CCForm();
            return en.HandlerMapExt();
        }

        /// <summary>
        /// 打开手机端
        /// </summary>
        /// <returns></returns>
        public string Do_OpenFlow()
        {
            string sid = this.GetRequestVal("Token");
            string[] strs = sid.Split('_');
            GenerWorkerList wl = new GenerWorkerList();
            int i = wl.Retrieve(GenerWorkerListAttr.FK_Emp, strs[0],
                GenerWorkerListAttr.WorkID, strs[1],
                GenerWorkerListAttr.IsPass, 0);

            if (i == 0)
            {
                return "err@提示:此工作已经被别人处理或者此流程已删除。";
            }

            BP.Port.Emp empOF = new BP.Port.Emp(wl.FK_Emp);
            Web.WebUser.SignInOfGener(empOF);
            return "MyFlow.htm?FK_Flow=" + wl.FK_Flow + "&WorkID=" + wl.WorkID + "&FK_Node=" + wl.FK_Node + "&FID=" + wl.FID;
        }
        /// <summary>
        /// 流程单表单查看.
        /// </summary>
        /// <returns>json</returns>
        public string FrmView_Init()
        {
            WF wf = new WF();
            return wf.FrmView_Init();
        }
        /// <summary>
        /// 撤销发送
        /// </summary>
        /// <returns></returns>
        public string FrmView_UnSend()
        {
            WF_WorkOpt_OneWork en = new WF_WorkOpt_OneWork();
            return en.OP_UnSend();
        }

        public string AttachmentUpload_Down()
        {
            WF_CCForm ccform = new WF_CCForm();
            return ccform.AttachmentUpload_Down();
        }

        public string AttachmentUpload_DownByStream()
        {
            WF_CCForm ccform = new WF_CCForm();
            return ccform.AttachmentUpload_DownByStream();
        }

        #region 关键字查询.

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <returns></returns>
        public string SearchKey_Query()
        {
            WF_RptSearch search = new WF_RptSearch();
            return search.KeySearch_Query();
        }
        #endregion 关键字查询.

        #region 查询.
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string Search_Init()
        {
            DataSet ds = new DataSet();
            string sql = "";

            string sqlOrgNoWhere = "";
            if (SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                sqlOrgNoWhere = " AND OrgNo='" + BP.Web.WebUser.OrgNo + "'";

            string tSpan = this.GetRequestVal("TSpan");
            if (tSpan == "")
                tSpan = null;

            #region 1、获取时间段枚举/总数.
            SysEnums ses = new SysEnums("TSpan");
            DataTable dtTSpan = ses.ToDataTableField();
            dtTSpan.TableName = "TSpan";
            ds.Tables.Add(dtTSpan);

            if (this.FK_Flow == null)
                sql = "SELECT  TSpan as No, COUNT(WorkID) as Num FROM WF_GenerWorkFlow WHERE (Emps LIKE '%" + WebUser.No + "%' OR Starter='" + WebUser.No + "') AND WFState > 1 " + sqlOrgNoWhere + "  GROUP BY TSpan";
            else
                sql = "SELECT  TSpan as No, COUNT(WorkID) as Num FROM WF_GenerWorkFlow WHERE FK_Flow='" + this.FK_Flow + "' AND (Emps LIKE '%" + WebUser.No + "%' OR Starter='" + WebUser.No + "')  AND WFState > 1  " + sqlOrgNoWhere + " GROUP BY TSpan";

            DataTable dtTSpanNum = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow drEnum in dtTSpan.Rows)
            {
                string no = drEnum["IntKey"].ToString();
                foreach (DataRow dr in dtTSpanNum.Rows)
                {
                    if (dr["No"].ToString() == no)
                    {
                        drEnum["Lab"] = drEnum["Lab"].ToString() + "(" + dr["Num"] + ")";
                        break;
                    }
                }
            }
            #endregion

            #region 2、处理流程类别列表.
            if (tSpan == "-1")
                sql = "SELECT  FK_Flow as No, FlowName as Name, COUNT(WorkID) as Num FROM WF_GenerWorkFlow WHERE (Emps LIKE '%" + WebUser.No + "%' OR TodoEmps LIKE '%" + BP.Web.WebUser.No + ",%' OR Starter='" + WebUser.No + "')  AND WFState > 1 AND FID = 0  " + sqlOrgNoWhere + " GROUP BY FK_Flow, FlowName";
            else
                sql = "SELECT  FK_Flow as No, FlowName as Name, COUNT(WorkID) as Num FROM WF_GenerWorkFlow WHERE TSpan=" + tSpan + " AND (Emps LIKE '%" + WebUser.No + "%' OR TodoEmps LIKE '%" + BP.Web.WebUser.No + ",%' OR Starter='" + WebUser.No + "')  AND WFState > 1 AND FID = 0  " + sqlOrgNoWhere + " GROUP BY FK_Flow, FlowName";

            DataTable dtFlows = DBAccess.RunSQLReturnTable(sql);

            dtFlows.Columns[0].ColumnName = "No";
            dtFlows.Columns[1].ColumnName = "Name";
            dtFlows.Columns[2].ColumnName = "Num";

            dtFlows.TableName = "Flows";
            ds.Tables.Add(dtFlows);
            #endregion

            #region 3、处理流程实例列表.
            GenerWorkFlows gwfs = new GenerWorkFlows();
            string sqlWhere = "";
            sqlWhere = "(1 = 1)AND (((Emps LIKE '%" + WebUser.No + "%')OR(TodoEmps LIKE '%" + WebUser.No + "%')OR(Starter = '" + WebUser.No + "')) AND (WFState > 1) " + sqlOrgNoWhere;
            if (tSpan != "-1")
            {
                sqlWhere += "AND (TSpan = '" + tSpan + "') ";
            }

            if (this.FK_Flow != null)
            {
                sqlWhere += "AND (FK_Flow = '" + this.FK_Flow + "')) ";
            }
            else
            {
                sqlWhere += ")";
            }
            sqlWhere += "ORDER BY RDT DESC";

            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.KingBaseR3 || SystemConfig.AppCenterDBType == DBType.KingBaseR6)
                sql = "SELECT NVL(WorkID, 0) WorkID,NVL(FID, 0) FID ,FK_Flow,FlowName,Title, NVL(WFSta, 0) WFSta,WFState,  Starter, StarterName,Sender,NVL(RDT, '2018-05-04 19:29') RDT,NVL(FK_Node, 0) FK_Node,NodeName, TodoEmps FROM (select * from WF_GenerWorkFlow where " + sqlWhere + ") where rownum <= 500";
            else if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                sql = "SELECT  TOP 500 ISNULL(WorkID, 0) WorkID,ISNULL(FID, 0) FID ,FK_Flow,FlowName,Title, ISNULL(WFSta, 0) WFSta,WFState,  Starter, StarterName,Sender,ISNULL(RDT, '2018-05-04 19:29') RDT,ISNULL(FK_Node, 0) FK_Node,NodeName, TodoEmps FROM WF_GenerWorkFlow where " + sqlWhere;
            else if (SystemConfig.AppCenterDBType == DBType.MySQL)
                sql = "SELECT IFNULL(WorkID, 0) WorkID,IFNULL(FID, 0) FID ,FK_Flow,FlowName,Title, IFNULL(WFSta, 0) WFSta,WFState,  Starter, StarterName,Sender,IFNULL(RDT, '2018-05-04 19:29') RDT,IFNULL(FK_Node, 0) FK_Node,NodeName, TodoEmps FROM WF_GenerWorkFlow where " + sqlWhere + " LIMIT 500";
            else if (SystemConfig.AppCenterDBType == DBType.PostgreSQL || SystemConfig.AppCenterDBType == DBType.UX)
                sql = "SELECT COALESCE(WorkID, 0) WorkID,COALESCE(FID, 0) FID ,FK_Flow,FlowName,Title, COALESCE(WFSta, 0) WFSta,WFState,  Starter, StarterName,Sender,COALESCE(RDT, '2018-05-04 19:29') RDT,COALESCE(FK_Node, 0) FK_Node,NodeName, TodoEmps FROM WF_GenerWorkFlow where " + sqlWhere + " LIMIT 500";
            DataTable mydt = DBAccess.RunSQLReturnTable(sql);

            mydt.Columns[0].ColumnName = "WorkID";
            mydt.Columns[1].ColumnName = "FID";
            mydt.Columns[2].ColumnName = "FK_Flow";
            mydt.Columns[3].ColumnName = "FlowName";
            mydt.Columns[4].ColumnName = "Title";
            mydt.Columns[5].ColumnName = "WFSta";
            mydt.Columns[6].ColumnName = "WFState";
            mydt.Columns[7].ColumnName = "Starter";
            mydt.Columns[8].ColumnName = "StarterName";
            mydt.Columns[9].ColumnName = "Sender";
            mydt.Columns[10].ColumnName = "RDT";
            mydt.Columns[11].ColumnName = "FK_Node";
            mydt.Columns[12].ColumnName = "NodeName";
            mydt.Columns[13].ColumnName = "TodoEmps";

            mydt.TableName = "WF_GenerWorkFlow";
            if (mydt != null)
            {
                mydt.Columns.Add("TDTime");
                foreach (DataRow dr in mydt.Rows)
                {
                    dr["TDTime"] = GetTraceNewTime(dr["FK_Flow"].ToString(), int.Parse(dr["WorkID"].ToString()), int.Parse(dr["FID"].ToString()));
                }
            }
            #endregion


            ds.Tables.Add(mydt);

            return BP.Tools.Json.ToJson(ds);
        }
        public static string GetTraceNewTime(string fk_flow, Int64 workid, Int64 fid)
        {
            #region 获取track数据.
            string sqlOfWhere2 = "";
            string sqlOfWhere1 = "";
            string dbStr = SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            if (fid == 0)
            {
                sqlOfWhere1 = " WHERE (FID=" + dbStr + "WorkID11 OR WorkID=" + dbStr + "WorkID12 )  ";
                ps.Add("WorkID11", workid);
                ps.Add("WorkID12", workid);
            }
            else
            {
                sqlOfWhere1 = " WHERE (FID=" + dbStr + "FID11 OR WorkID=" + dbStr + "FID12 ) ";
                ps.Add("FID11", fid);
                ps.Add("FID12", fid);
            }

            string sql = "";
            sql = "SELECT MAX(RDT) FROM ND" + int.Parse(fk_flow) + "Track " + sqlOfWhere1;
            sql = "SELECT RDT FROM  ND" + int.Parse(fk_flow) + "Track  WHERE RDT=(" + sql + ")";
            ps.SQL = sql;

            try
            {
                return DBAccess.RunSQLReturnString(ps);
            }
            catch
            {
                // 处理track表.
                Track.CreateOrRepairTrackTable(fk_flow);
                return DBAccess.RunSQLReturnString(ps);
            }
            #endregion 获取track数据.
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public string Search_Search()
        {
            string TSpan = this.GetRequestVal("TSpan");
            string FK_Flow = this.GetRequestVal("FK_Flow");

            GenerWorkFlows gwfs = new GenerWorkFlows();
            QueryObject qo = new QueryObject(gwfs);
            qo.AddWhere(GenerWorkFlowAttr.Emps, " LIKE ", "%" + BP.Web.WebUser.No + "%");
            if (!DataType.IsNullOrEmpty(TSpan))
            {
                qo.addAnd();
                qo.AddWhere(GenerWorkFlowAttr.TSpan, this.GetRequestVal("TSpan"));
            }
            if (!DataType.IsNullOrEmpty(FK_Flow))
            {
                qo.addAnd();
                qo.AddWhere(GenerWorkFlowAttr.FK_Flow, this.GetRequestVal("FK_Flow"));
            }
            qo.Top = 50;

            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL || SystemConfig.AppCenterDBType == DBType.UX
                || SystemConfig.AppCenterDBType == DBType.KingBaseR3 || SystemConfig.AppCenterDBType == DBType.KingBaseR6)
            {
                qo.DoQuery();
                DataTable dt = gwfs.ToDataTableField("Ens");
                return BP.Tools.Json.ToJson(dt);
            }
            else
            {
                DataTable dt = qo.DoQueryToTable();
                return BP.Tools.Json.ToJson(dt);
            }
        }

        #endregion

    }
}
