using System;
using System.Collections.Generic;
using System.Collections;
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
using LitJson;
using BP.WF.XML;
using BP.WF.Port.Admin2;
namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 初始化函数 
    /// </summary>
    public class WF_Admin_CCBPMDesigner : DirectoryPageBase
    {
        /// <summary>
        /// 选择器
        /// </summary>
        /// <returns></returns>
        public string SelectEmps_Init()
        {
            string fk_flowsort = this.GetRequestVal("FK_FlowSort").Substring(1);

            if (DataType.IsNullOrEmpty(fk_flowsort) == true || fk_flowsort.Equals("undefined") == true)
                fk_flowsort = "99";
            DataSet ds = new DataSet();

            string sql = "";
            sql = "SELECT 'F' + No as No,Name, 'F' + ParentNo as ParentNo FROM WF_FlowSort WHERE No='" + fk_flowsort + "' OR ParentNo='" + fk_flowsort + "' ORDER BY Idx";

            DataTable dtFlowSorts = BP.DA.DBAccess.RunSQLReturnTable(sql);
            //if (dtFlowSort.Rows.Count == 0)
            //{
            //    fk_dept = BP.Web.WebUser.FK_Dept;
            //    sql = "SELECT No,Name,ParentNo FROM Port_Dept WHERE No='" + fk_dept + "' OR ParentNo='" + fk_dept + "' ORDER BY Idx ";
            //    dtDept = BP.DA.DBAccess.RunSQLReturnTable(sql);
            //}

            dtFlowSorts.TableName = "FlowSorts";
            ds.Tables.Add(dtFlowSorts);

            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dtFlowSorts.Columns[0].ColumnName = "No";
                dtFlowSorts.Columns[1].ColumnName = "Name";
                dtFlowSorts.Columns[2].ColumnName = "ParentNo";
            }

            //sql = "SELECT No,Name, FK_Dept FROM Port_Emp WHERE FK_Dept='" + fk_dept + "' ";
            sql = "SELECT  No,(NO + '.' + NAME) as Name, 'F' + FK_FlowSort as ParentNo, Idx FROM WF_Flow where FK_FlowSort='" + fk_flowsort + "' ";
            sql += " ORDER BY Idx ";

            DataTable dtFlows = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dtFlows.TableName = "Flows";
            ds.Tables.Add(dtFlows);
            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dtFlows.Columns[0].ColumnName = "No";
                dtFlows.Columns[1].ColumnName = "Name";
                dtFlows.Columns[2].ColumnName = "FK_FlowSort";
            }

            //转化为 json 
            return BP.Tools.Json.DataSetToJson(ds, false);
        }

        /// <summary>
        /// 按照管理员登录.
        /// </summary>
        /// <param name="userNo">管理员编号</param>
        /// <returns>登录信息</returns>
        public string AdminerChang_LoginAs()
        {
            string orgNo = this.GetRequestVal("OrgNo");
            WebUser.OrgNo = this.OrgNo;
            return "info@登录成功, 如果系统不能自动刷新，请手工刷新。";
        }

        public string Flows_Init()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("FlowNo");
            dt.Columns.Add("FlowName");

            dt.Columns.Add("NumOfRuning"); //运行中的.
            dt.Columns.Add("NumOfOK"); //已经完成的.
            dt.Columns.Add("NumOfEtc"); //其他.

            Flows fls = new Flows();
            fls.RetrieveAll();

            foreach (Flow fl in fls)
            {
                DataRow dr = dt.NewRow();
                dr["FlowNo"] = fl.No;
                dr["FlowName"] = fl.Name;
                dr["NumOfRuning"] = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM  WF_GenerWorkFlow WHERE FK_Flow='" + fl.No + "' AND WFState in (2,5)", 0);
                dr["NumOfOK"] = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM  WF_GenerWorkFlow WHERE FK_Flow='" + fl.No + "' AND WFState = 3 ", 0);
                dr["NumOfEtc"] = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM  WF_GenerWorkFlow WHERE FK_Flow='" + fl.No + "' AND WFState in (4,5,6,7,8) ", 0);

                dt.Rows.Add(dr);
            }

            return BP.Tools.Json.ToJson(dt);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_CCBPMDesigner()
        {
        }

        /// <summary>
        /// 保存节点名字. @sly
        /// </summary>
        /// <returns>返回保存方法</returns>
        public string Designer_SaveNodeName()
        {
            string sql = "UPDATE WF_Node SET Name='" + this.Name + "' WHERE NodeID=" + this.FK_Node;
            DBAccess.RunSQL(sql);

            //表单ID.
            string frmID = "ND" + this.FK_Node;
            sql = "UPDATE Sys_MapData SET Name='" + this.Name + "' WHERE No='" + frmID + "'  AND ( Name='' || Name IS Null) ";
            DBAccess.RunSQL(sql);

            //Node nd = new Node();
            //nd.NodeID = this.FK_Node;
            //nd.RetrieveFromDBSources();

            //MapData md = new MapData();
            //md.No = frmID;
            //md.RetrieveFromDBSources();

            //BP.WF.Template.NodeExt nodeExt = new BP.WF.Template.NodeExt(this.FK_Node);
            //nodeExt.Name = this.Name;
            //nodeExt.Update();

            //BP.WF.Node node = new BP.WF.Node(this.FK_Node);
            //node.Name = this.Name;
            //node.Update();

            //MapData mapData = new MapData("ND"+this.FK_Node);
            //if ( DataType.IsNullOrEmpty(mapData.Name)==true)
            //{
            //    mapData.Name = this.Name;
            //    mapData.Update();
            //}

            //修改分组名称.
            var groups = new BP.Sys.GroupFields();
            groups.Retrieve("FrmID", "ND" + this.FK_Node);
            if (groups.Count == 1)
            {
                var group = groups[0] as BP.Sys.GroupField;
                group.Lab = this.Name;
                group.Update();
            }

            //清除指定的名字.
            // BP.DA.Cash2019.ClearCashSpecEnName("BP.WF.Template.NodeExt");
            // BP.DA.Cash2019.ClearCashSpecEnName("BP.WF.Node");
            // BP.DA.Cash2019.ClearCashSpecEnName("BP.Sys.GroupField");

            //清楚缓存.
            BP.DA.Cash.ClearCash();

            return "更新成功.";
        }

        /// <summary>
        /// 执行流程设计图的保存.
        /// </summary>
        /// <returns></returns>
        public string Designer_Save()
        {
            //@sly. 
            if (BP.Web.WebUser.IsAdmin == false)
                return "err@当前您不是管理员,请重新登录.造成这种原因是您在测试容器没有正常退回造成的.";

            string sql = "";
            try
            {

                StringBuilder sBuilder = new StringBuilder();

                //保存方向.
                sBuilder = new StringBuilder();
                string[] dirs = this.GetRequestVal("Dirs").Split('@');
                foreach (string item in dirs)
                {
                    if (item == "" || item == null)
                        continue;
                    string[] strs = item.Split(',');
                    sBuilder.Append("DELETE FROM WF_Direction WHERE MyPK='" + strs[0] + "';");
                    sBuilder.Append("INSERT INTO WF_Direction (MyPK,FK_Flow,Node,ToNode,IsCanBack) VALUES ('" + strs[0] + "','" + strs[1] + "','" + strs[2] + "','" + strs[3] + "'," + "0);");
                }
                DBAccess.RunSQLs(sBuilder.ToString());

                //保存label位置.
                sBuilder = new StringBuilder();
                string[] labs = this.GetRequestVal("Labs").Split('@');
                foreach (string item in labs)
                {
                    if (item == "" || item == null)
                        continue;
                    string[] strs = item.Split(',');

                    sBuilder.Append("UPDATE WF_LabNote SET X=" + strs[1] + ",Y=" + strs[2] + " WHERE MyPK='" + strs[0] + "';");
                }

                string sqls = sBuilder.ToString();
                DBAccess.RunSQLs(sqls);

                //更新节点 HisToNDs，不然就需要检查一遍.
                BP.WF.Nodes nds = new Nodes();
                nds.Retrieve(BP.WF.Template.NodeAttr.FK_Flow, this.FK_Flow);

                //获得方向集合处理toNodes
                Directions mydirs = new Directions(this.FK_Flow);

                string mystrs = "";
                foreach (Node item in nds)
                {
                    string strs = "";
                    foreach (Direction dir in mydirs)
                    {
                        if (dir.Node != item.NodeID)
                            continue;

                        strs += "@" + dir.ToNode;
                    }

                    DBAccess.RunSQL("UPDATE WF_Node SET HisToNDs='" + strs + "' WHERE NodeID=" + item.NodeID);
                }

                //获得字符串格式. $101;@102@103
                //   string[] mystr = mystrs.Split('$');

                //保存节点位置. @101,2,30@102,3,1
                string[] nodes = this.GetRequestVal("Nodes").Split('@');
                foreach (string item in nodes)
                {
                    if (item == "" || item == null)
                        continue;

                    string[] strs = item.Split(',');
                    string nodeID = strs[0]; //获得nodeID.

                    //@sly 
                    sBuilder.Append("UPDATE WF_Node SET X=" + strs[1] + ",Y=" + strs[2] + ",Name='" + strs[3] + "' WHERE NodeID=" + strs[0] + ";");
                }

                DBAccess.RunSQLs(sBuilder.ToString());

                //清楚缓存.
                BP.DA.Cash.ClearCash();
                // Node nd = new Node(102);
                // throw new Exception(nd.Name);

                return "保存成功.";

            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// 下载流程模版
        /// </summary>
        /// <returns></returns>
        public string ExpFlowTemplete()
        {
            Flow flow = new Flow(this.FK_Flow);
            string fileXml = flow.GenerFlowXmlTemplete();
            string docs = DataType.ReadTextFile(fileXml);
            return docs;
        }
        /// <summary>
        /// 返回临时文件.
        /// </summary>
        /// <returns></returns>
        public string DownFormTemplete()
        {
            DataSet ds = BP.Sys.CCFormAPI.GenerHisDataSet_AllEleInfo(this.FK_MapData);

            string file = BP.Sys.SystemConfig.PathOfTemp + this.FK_MapData + ".xml";
            ds.WriteXml(file);
            string docs = DataType.ReadTextFile(file);
            return docs;

            //return file;
            //return docs;
        }

        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            return "err@没有判断的标记:" + this.DoType;
        }
        #endregion 执行父类的重写方法.

        /// <summary>
        /// 使管理员登录使管理员登录    /// </summary>
        /// <returns></returns>
        public string LetLogin()
        {
            LetAdminLogin(this.GetRequestVal("UserNo"), true);
            return "登录成功.";
        }
        /// <summary>
        /// 获得枚举列表的JSON.
        /// </summary>
        /// <returns></returns>
        public string Logout()
        {
            BP.WF.Dev2Interface.Port_SigOut();
            return "您已经安全退出,欢迎使用ccbpm.";
        }

        /// <summary>
        /// 根据部门、岗位获取人员列表
        /// </summary>
        /// <returns></returns>
        public string GetEmpsByStationTable()
        {
            string deptid = this.GetRequestVal("DeptNo");
            string stid = this.GetRequestVal("StationNo");

            if (string.IsNullOrWhiteSpace(deptid) || string.IsNullOrWhiteSpace(stid))
                return "[]";

            DataTable dt = new DataTable();
            dt.Columns.Add("NO", typeof(string));
            dt.Columns.Add("PARENTNO", typeof(string));
            dt.Columns.Add("NAME", typeof(string));
            dt.Columns.Add("TTYPE", typeof(string));


            BP.GPM.Emp emp = null;
            BP.GPM.Emps emps = new BP.GPM.Emps();
            emps.RetrieveAll();

            BP.GPM.DeptEmpStations dess = new BP.GPM.DeptEmpStations();
            dess.Retrieve(BP.GPM.DeptEmpStationAttr.FK_Dept, deptid, BP.GPM.DeptEmpStationAttr.FK_Station, stid);

            foreach (BP.GPM.DeptEmpStation des in dess)
            {
                emp = emps.GetEntityByKey(des.FK_Emp) as BP.GPM.Emp;

                dt.Rows.Add(emp.No, deptid + "|" + stid, emp.Name, "EMP");
            }

            return BP.Tools.Json.ToJson(dt);
        }

        public string GetStructureTreeRootTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("NO", typeof(string));
            dt.Columns.Add("PARENTNO", typeof(string));
            dt.Columns.Add("NAME", typeof(string));
            dt.Columns.Add("TTYPE", typeof(string));

            string parentrootid = this.GetRequestVal("parentrootid");  // context.Request.QueryString["parentrootid"];
            string newRootId = "";

            if (WebUser.No != "admin")
            {
                newRootId = WebUser.OrgNo;
            }


            BP.GPM.Dept dept = new BP.GPM.Dept();

            if (!string.IsNullOrWhiteSpace(newRootId))
            {
                if (dept.Retrieve(BP.GPM.DeptAttr.No, newRootId) == 0)
                {
                    dept.No = "-1";
                    dept.Name = "无部门";
                    dept.ParentNo = "";
                }
            }
            else
            {
                if (dept.Retrieve(BP.GPM.DeptAttr.ParentNo, parentrootid) == 0)
                {
                    dept.No = "-1";
                    dept.Name = "无部门";
                    dept.ParentNo = "";
                }
            }

            dt.Rows.Add(dept.No, dept.ParentNo, dept.Name, "DEPT");


            return BP.Tools.Json.ToJson(dt);
        }

        /// <summary>
        /// 获取指定部门下一级子部门及岗位列表
        /// </summary>
        /// <returns></returns>
        public string GetSubDeptsTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("NO", typeof(string));
            dt.Columns.Add("PARENTNO", typeof(string));
            dt.Columns.Add("NAME", typeof(string));
            dt.Columns.Add("TTYPE", typeof(string));

            string rootid = this.GetRequestVal("rootid");// context.Request.QueryString["rootid"];


            BP.GPM.Depts depts = new BP.GPM.Depts();
            depts.Retrieve(BP.GPM.DeptAttr.ParentNo, rootid);
            BP.Port.Stations sts = new BP.Port.Stations();
            sts.RetrieveAll();
            BP.GPM.DeptStations dss = new BP.GPM.DeptStations();
            dss.Retrieve(BP.GPM.DeptStationAttr.FK_Dept, rootid);
            BP.GPM.DeptEmps des = new BP.GPM.DeptEmps();
            des.Retrieve(BP.GPM.DeptEmpAttr.FK_Dept, rootid);
            BP.GPM.DeptEmpStations dess = new BP.GPM.DeptEmpStations();
            dess.Retrieve(BP.GPM.DeptEmpStationAttr.FK_Dept, rootid);
            Station stt = null;
            BP.GPM.Emp emp = null;
            List<string> inemps = new List<string>();

            foreach (BP.GPM.Dept dept in depts)
            {
                //增加部门
                dt.Rows.Add(dept.No, dept.ParentNo, dept.Name, "DEPT");
            }

            //增加部门岗位
            foreach (BP.GPM.DeptStation ds in dss)
            {
                stt = sts.GetEntityByKey(ds.FK_Station) as Station;

                if (stt == null) continue;

                dt.Rows.Add(ds.FK_Station, rootid, stt.Name, "STATION");
            }

            //增加没有岗位的人员
            foreach (BP.GPM.DeptEmp de in des)
            {
                if (dess.GetEntityByKey(BP.GPM.DeptEmpStationAttr.FK_Emp, de.FK_Emp) == null)
                {
                    if (inemps.Contains(de.FK_Emp))
                        continue;
                    inemps.Add(de.FK_Emp);
                }
            }

            foreach (string inemp in inemps)
            {
                emp = new BP.GPM.Emp(inemp);
                dt.Rows.Add(emp.No, rootid, emp.Name, "EMP");
            }

            return BP.Tools.Json.ToJson(dt);
        }

        #region 主页.
        /// <summary>
        /// 初始化登录界面.
        /// </summary>
        /// <returns></returns>
        public string Default_Init()
        {
            try
            {
                //如果登录信息丢失了,就让其重新登录一次.
                if (DataType.IsNullOrEmpty(BP.Web.WebUser.NoOfRel) == true)
                {
                    string userNo = this.GetRequestVal("UserNo");
                    string sid = this.GetRequestVal("SID");
                    BP.WF.Dev2Interface.Port_LoginBySID(sid);
                }

                if (BP.Web.WebUser.IsAdmin == false)
                    return "url@Login.htm?DoType=Logout&Err=NoAdminUsers";

                //如果没有流程表，就执行安装.
                if (BP.DA.DBAccess.IsExitsObject("WF_Flow") == false)
                    return "url@../DBInstall.htm";

                Hashtable ht = new Hashtable();

                ht.Add("OSModel", "1");

                //把系统信息加入里面去.
                ht.Add("SysNo", SystemConfig.SysNo);
                ht.Add("SysName", SystemConfig.SysName);

                ht.Add("CustomerNo", SystemConfig.CustomerNo);
                ht.Add("CustomerName", SystemConfig.CustomerName);

                //集成的平台.
                ht.Add("RunOnPlant", SystemConfig.RunOnPlant);

                try
                {
                    // 执行升级
                    string str = BP.WF.Glo.UpdataCCFlowVer();
                    if (str == null)
                        str = "";
                    ht.Add("Msg", str);
                }
                catch (Exception ex)
                {
                    return "err@" + ex.Message;
                }

                //生成Json.
                return BP.Tools.Json.ToJsonEntityModel(ht);
            }
            catch (Exception ex)
            {
                return "err@初始化界面期间出现如下错误:" + ex.Message;
            }
        }
        #endregion

        #region 登录窗口.
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
            //检查数据库连接.
            try
            {
                DBAccess.TestIsConnection();
            }
            catch (Exception ex)
            {
                return "err@异常信息:" + ex.Message;
            }

            //检查是否缺少Port_Emp 表，如果没有就是没有安装.
            if (DBAccess.IsExitsObject("Port_Emp") == false && DBAccess.IsExitsObject("WF_Flow") == false)
                return "url@../DBInstall.htm";

            ////让admin登录
            //if (DataType.IsNullOrEmpty(BP.Web.WebUser.No) || BP.Web.WebUser.IsAdmin == false)
            //    return "url@Login.htm?DoType=Logout";

            //如果没有流程表，就执行安装.
            if (BP.DA.DBAccess.IsExitsObject("WF_Flow") == false)
                return "url@../DBInstall.htm";

            //是否需要自动登录
            string userNo = this.GetRequestVal("UserNo");
            string sid = this.GetRequestVal("SID");

            if (!String.IsNullOrEmpty(sid) && !String.IsNullOrEmpty(userNo))
            {
                /*  */
                try
                {
                    string str = BP.WF.Glo.UpdataCCFlowVer();
                    BP.WF.Dev2Interface.Port_LoginBySID(sid);
                    if (this.FK_Flow == null)
                        return "url@Default.htm?UserNo=" + userNo + "&Key=" + DateTime.Now.ToBinary();
                    else
                        return "url@Designer.htm?UserNo=" + userNo + "&FK_Flow=" + this.FK_Flow + "&Key=" + DateTime.Now.ToBinary();
                }
                catch (Exception ex)
                {
                    return "err@登录失败" + ex.Message;
                }
            }

            try
            {
                // 执行升级
                string str = BP.WF.Glo.UpdataCCFlowVer();
                if (str == null)
                    str = "准备完毕,欢迎登录,当前小版本号为:" + BP.WF.Glo.Ver;
                return str;
            }
            catch (Exception ex)
            {
                string msg = "err@升级失败(ccbpm有自动修复功能,您可以刷新一下系统会自动创建字段,刷新多次扔解决不了问题,请反馈给我们)";
                msg += "@系统信息:" + ex.Message;
                return msg;
            }
        }
        //流程设计器登陆前台，转向规则，判断是否为天业BPM
        public string Login_Redirect()
        {
            if (SystemConfig.CustomerNo == "TianYe")
                return "url@../../../BPM/pages/login.html";

            return "url@../../AppClassic/Login.htm?DoType=Logout";
        }
        /// <summary>
        /// 提交
        /// 1.返回url@就需要转向。 
        /// 2.返回err@提示错误  
        /// 3.返回其他的就是Json选择组织. 返回的json是OrgNo
        /// </summary>
        /// <returns></returns>
        public string Login_Submit()
        {
            BP.Port.Emp emp = new BP.Port.Emp();
            emp.No = this.GetRequestVal("TB_No").Trim();
            if (emp.RetrieveFromDBSources() == 0)
                return "err@用户名或密码错误.";

            string pass = this.GetRequestVal("TB_PW").Trim();
            if (emp.CheckPass(pass) == false)
                return "err@用户名或密码错误.";

            //如果是单机版本，仅仅admin登录.
            if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
            {
                if (emp.No.Equals("admin") == false)
                    return "err@非admin不能登录.";

                //让其登录.
                BP.WF.Dev2Interface.Port_Login(emp.No);

                //只有一个组织的情况.
                if (DBAccess.IsView("Port_Emp") == false)
                {
                    string sid = BP.DA.DBAccess.GenerGUID();
                    string sql = "UPDATE Port_Emp SET SID='" + sid + "' WHERE No='" + emp.No + "'";
                    BP.DA.DBAccess.RunSQL(sql);
                    emp.SID = sid;
                }

                //设置SID.
                WebUser.SID = emp.SID; //设置SID.

                return "url@Default.htm?SID=" + emp.SID + "&UserNo=" + emp.No;
            }

            //获得当前管理员管理的组织数量.
            OrgAdminers adminers = null;
            if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
            {
                //WebUser.OrgNo = orgs[0].GetValStrByKey("No");
            }
            else
            {
                //查询他管理多少组织.
                adminers = new OrgAdminers();
                adminers.Retrieve(OrgAdminerAttr.FK_Emp, emp.No);
                if (adminers.Count == 0)
                {
                    BP.WF.Port.Admin2.Orgs orgs = new Orgs();
                    int i = orgs.Retrieve("Adminer", WebUser.No);
                    if (i == 0)
                        return "err@非管理员或二级管理员用户，不能登录后台.";

                    foreach (BP.WF.Port.Admin2.Org org in orgs)
                    {
                        OrgAdminer oa = new OrgAdminer();
                        oa.FK_Emp = WebUser.No;
                        oa.OrgNo = org.No;
                        oa.Save();
                    }
                    adminers.Retrieve(OrgAdminerAttr.FK_Emp, emp.No);
                }

                //     if (adminers.Count==0)
                //       WebUser.FK_Dept = WebUser.OrgNo; //FK_Dept.
            }

            //设置他的组织，信息.
            WebUser.No = emp.No; //登录帐号.
            WebUser.FK_Dept = emp.FK_Dept;
            WebUser.FK_DeptName = emp.FK_DeptText;

            WebUser.SID = DBAccess.GenerGUID(); //设置SID.

            //执行登录.
            BP.WF.Dev2Interface.Port_Login(emp.No);

            //执行更新到用户表信息.
            // WebUser.UpdateSIDAndOrgNoSQL();

            //判断是否是多个组织的情况.
            if (Glo.CCBPMRunModel == CCBPMRunModel.Single || adminers.Count == 1)
                return "url@Default.htm?SID=" + emp.SID + "&UserNo=" + emp.No;

            return "url@SelectOneOrg.htm?SID=" + emp.SID + "&UserNo=" + emp.No;

            // return orgs.ToJson(); //返回这个Json,让其选择一个组织登录.
        }
        /// <summary>
        ///初始化当前登录人的下的所有组织
        /// </summary>
        /// <returns></returns>
        public string SelectOneOrg_Init()
        {
            Orgs orgs = new Orgs();
            //            orgs.Retrieve("Adminer", WebUser.No);
            orgs.RetrieveInSQL("SELECT OrgNo FROM Port_OrgAdminer WHERE FK_Emp='" + WebUser.No + "'");
            return orgs.ToJson();
        }
        /// <summary>
        ///选择一个组织
        /// </summary>
        /// <returns></returns>
        public string SelectOneOrg_Selected()
        {
            WebUser.OrgNo = this.OrgNo;

            //找到管理员所在的部门.
            string sql = "SELECT a.No FROM Port_Dept A,Port_DeptEmp B WHERE A.No=B.FK_Dept AND B.FK_Emp='" + WebUser.No + "'  AND A.OrgNo='" + this.OrgNo + "'";
            string deptNo = DBAccess.RunSQLReturnStringIsNull(sql, this.OrgNo);

            WebUser.FK_Dept = deptNo;

            //执行更新到用户表信息.
            WebUser.UpdateSIDAndOrgNoSQL();

            return "url@Default.htm?SID=" + WebUser.SID + "&UserNo=" + WebUser.No + "&OrgNo=" + WebUser.OrgNo;
            // return "登录成功.";
        }
        #endregion 登录窗口.



        #region 流程相关 Flow
        /// <summary>
        /// 获取流程所有元素
        /// </summary>
        /// <returns>json data</returns>
        public string Flow_AllElements_ResponseJson()
        {
            BP.WF.Flow flow = new BP.WF.Flow();
            flow.No = this.FK_Flow;
            flow.RetrieveFromDBSources();

            DataSet ds = new DataSet();
            DataTable dtNodes = DBAccess.RunSQLReturnTable("SELECT NODEID,NAME,X,Y,RUNMODEL FROM WF_NODE WHERE FK_FLOW='" + this.FK_Flow + "'");
            dtNodes.TableName = "Nodes";
            ds.Tables.Add(dtNodes);

            DataTable dtDirection = DBAccess.RunSQLReturnTable("SELECT NODE,TONODE FROM WF_DIRECTION WHERE FK_FLOW='" + this.FK_Flow + "'");
            dtDirection.TableName = "Direction";
            ds.Tables.Add(dtDirection);

            DataTable dtLabNote = DBAccess.RunSQLReturnTable("SELECT MYPK,NAME,X,Y FROM WF_LABNOTE WHERE FK_FLOW='" + this.FK_Flow + "'");
            dtLabNote.TableName = "LabNote";
            ds.Tables.Add(dtLabNote);


            // return BP.Tools.Json.DataSetToJson(ds, false);
            return BP.Tools.Json.ToJson(ds);
        }
        #endregion end Flow

        #region 节点相关 Nodes
        /// <summary>
        /// gen
        /// </summary>
        /// <param name="figureName"></param>
        /// <returns></returns>
        public BP.WF.RunModel Node_GetRunModelByFigureName(string figureName)
        {
            BP.WF.RunModel runModel = BP.WF.RunModel.Ordinary;
            switch (figureName)
            {
                case "NodeOrdinary":
                    runModel = BP.WF.RunModel.Ordinary;
                    break;
                case "NodeFL":
                    runModel = BP.WF.RunModel.FL;
                    break;
                case "NodeHL":
                    runModel = BP.WF.RunModel.HL;
                    break;
                case "NodeFHL":
                    runModel = BP.WF.RunModel.FHL;
                    break;
                case "NodeSubThread":
                    runModel = BP.WF.RunModel.SubThread;
                    break;
                default:
                    runModel = BP.WF.RunModel.Ordinary;
                    break;
            }
            return runModel;
        }
        /// <summary>
        /// 根据节点编号删除流程节点
        /// </summary>
        /// <returns>执行结果</returns>
        public string DeleteNode()
        {
            try
            {
                BP.WF.Node node = new BP.WF.Node();
                node.NodeID = this.FK_Node;
                if (node.RetrieveFromDBSources() == 0)
                    return "err@删除失败,没有删除到数据，估计该节点已经别删除了.";

                if (node.IsStartNode == true)
                    return "err@开始节点不允许被删除。";

                node.Delete();
                return "删除成功.";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 修改节点名称
        /// </summary>
        /// <returns></returns>
        public string Node_EditNodeName()
        {
            string FK_Node = this.GetValFromFrmByKey("NodeID");
            //string NodeName = System.Web.HttpContext.Current.Server.UrlDecode(this.GetValFromFrmByKey("NodeName"));
            string NodeName = HttpContextHelper.UrlDecode(this.GetValFromFrmByKey("NodeName"));

            BP.WF.Node node = new BP.WF.Node();
            node.NodeID = int.Parse(FK_Node);
            int iResult = node.RetrieveFromDBSources();
            if (iResult > 0)
            {
                node.Name = NodeName;
                node.Update();
                return "@修改成功.";
            }

            return "err@修改节点失败，请确认该节点是否存在？";
        }

        /// <summary>
        /// 修改节点运行模式
        /// </summary>
        /// <returns></returns>
        public string Node_ChangeRunModel()
        {
            string runModel = GetValFromFrmByKey("RunModel");
            BP.WF.Node node = new BP.WF.Node(this.FK_Node);
            //节点运行模式
            switch (runModel)
            {
                case "NodeOrdinary":
                    node.HisRunModel = BP.WF.RunModel.Ordinary;
                    break;
                case "NodeFL":
                    node.HisRunModel = BP.WF.RunModel.FL;
                    break;
                case "NodeHL":
                    node.HisRunModel = BP.WF.RunModel.HL;
                    break;
                case "NodeFHL":
                    node.HisRunModel = BP.WF.RunModel.FHL;
                    break;
                case "NodeSubThread":
                    node.HisRunModel = BP.WF.RunModel.SubThread;
                    break;
            }
            node.Update();

            return "设置成功.";
        }
        #endregion end Node

        #region CCBPMDesigner
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public string GetWebUserInfo()
        {
            if (WebUser.No == null)
                return "err@当前用户没有登录，请登录后再试。";

            Hashtable ht = new Hashtable();

            BP.Port.Emp emp = new BP.Port.Emp(WebUser.No);

            ht.Add("No", emp.No);
            ht.Add("Name", emp.Name);
            ht.Add("FK_Dept", emp.FK_Dept);
            ht.Add("SID", emp.SID);

            return BP.Tools.Json.ToJsonEntityModel(ht);
        }

        StringBuilder sbJson = new StringBuilder();
        /// <summary>
        /// 获取流程树数据
        /// </summary>
        /// <returns>返回结果Json,流程树</returns>
        public string GetFlowTreeTable()
        {
            if (Glo.CCBPMRunModel == CCBPMRunModel.GroupInc
                || Glo.CCBPMRunModel == CCBPMRunModel.SAAS)
                return GetFlowTreeTable_GroupInc();

            string sql = @"SELECT * FROM (SELECT 'F'+No as NO,'F'+ParentNo PARENTNO, NAME, IDX, 1 ISPARENT,'FLOWTYPE' TTYPE, -1 DTYPE FROM WF_FlowSort  " +
                           "union " +
                           "SELECT NO, 'F'+FK_FlowSort as PARENTNO,(NO + '.' + NAME) as NAME,IDX,0 ISPARENT,'FLOW' TTYPE, 0 as DTYPE FROM WF_Flow ) A  ORDER BY DTYPE, IDX,NO ";

            if (BP.Sys.SystemConfig.AppCenterDBType == DBType.Oracle
                || BP.Sys.SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                sql = @"SELECT * FROM (SELECT 'F'||No as NO,'F'||ParentNo as PARENTNO,NAME, IDX, 1 ISPARENT,'FLOWTYPE' TTYPE,-1 DTYPE FROM WF_FlowSort " +
                        "  union " +
                        "SELECT NO, 'F'||FK_FlowSort as PARENTNO,NO||'.'||NAME as NAME,IDX,0 ISPARENT,'FLOW' TTYPE,0 as DTYPE FROM WF_Flow ) A  ORDER BY DTYPE, IDX,NO";
            }

            if (BP.Sys.SystemConfig.AppCenterDBType == DBType.MySQL)
            {
                sql = @"SELECT * FROM (SELECT CONCAT('F', No) NO, CONCAT('F', ParentNo) PARENTNO, NAME, IDX, 1 ISPARENT,'FLOWTYPE' TTYPE,-1 DTYPE FROM WF_FlowSort " +
                     "  " +
                     "union " +
                     "SELECT NO, CONCAT('F', FK_FlowSort) PARENTNO, CONCAT(NO, '.', NAME) NAME,IDX,0 ISPARENT,'FLOW' TTYPE, 0 as DTYPE FROM WF_Flow " +
                     " ) A  ORDER BY DTYPE, IDX,NO";
            }

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (BP.Sys.SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dt.Columns["no"].ColumnName = "NO";
                dt.Columns["name"].ColumnName = "NAME";
                dt.Columns["parentno"].ColumnName = "PARENTNO";
                dt.Columns["idx"].ColumnName = "IDX";
                dt.Columns["isparent"].ColumnName = "ISPARENT";
                dt.Columns["ttype"].ColumnName = "TTYPE";
                dt.Columns["dtype"].ColumnName = "DTYPE";
            }

            //判断是否为空，如果为空，则创建一个流程根结点，added by liuxc,2016-01-24
            if (dt.Rows.Count == 0)
            {
                FlowSort fs = new FlowSort();
                fs.No = "99";
                fs.ParentNo = "0";
                fs.Name = "流程树";
                fs.Insert();

                dt.Rows.Add("F99", "F0", "流程树", 0, 1, "FLOWTYPE", -1);
            }
            else
            {
                DataRow[] drs = dt.Select("NAME='流程树'");
                if (drs.Length > 0 && !Equals(drs[0]["PARENTNO"], "F0"))
                    drs[0]["PARENTNO"] = "F0";
            }


            /* if (WebUser.No != "admin")
             {*/
            DataRow rootRow = dt.Select("PARENTNO='F0'")[0];
            DataRow newRootRow = dt.Rows[0];

            newRootRow["PARENTNO"] = "F0";
            DataTable newDt = dt.Clone();
            newDt.Rows.Add(newRootRow.ItemArray);
            GenerChildRows(dt, newDt, newRootRow);
            dt = newDt;

            string str = BP.Tools.Json.ToJson(dt);
            return str;
        }
        public string GetFlowTreeTable_GroupInc()
        {
            string sql = "SELECT * FROM ( ";

            sql += "  SELECT 'F'+No as NO,'F'+ParentNo PARENTNO, NAME, IDX, 1 ISPARENT,'FLOWTYPE' TTYPE, -1 DTYPE FROM WF_FlowSort WHERE OrgNo ='" + WebUser.OrgNo + "' OR No=1 ";
            sql += "  UNION ";
            sql += "  SELECT NO, 'F'+FK_FlowSort as PARENTNO,(NO + '.' + NAME) as NAME,IDX,0 ISPARENT,'FLOW' TTYPE, 0 as DTYPE FROM WF_Flow WHERE OrgNo ='" + WebUser.OrgNo + "' ";
            sql += " ) A ";
            sql += "  ORDER BY DTYPE, IDX ";

            if (BP.Sys.SystemConfig.AppCenterDBType == DBType.Oracle
                || BP.Sys.SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                sql = @"SELECT * FROM (SELECT 'F'||No as NO,'F'||ParentNo as PARENTNO,NAME, IDX, 1 ISPARENT,'FLOWTYPE' TTYPE,-1 DTYPE FROM WF_FlowSort " +
                        " WHERE OrgNo ='" + WebUser.OrgNo + "' or No = 1 union " +
                        "SELECT NO, 'F'||FK_FlowSort as PARENTNO,NO||'.'||NAME as NAME,IDX,0 ISPARENT,'FLOW' TTYPE,0 as DTYPE FROM WF_Flow WHERE OrgNo ='" + WebUser.OrgNo + "') A  ORDER BY DTYPE, IDX";
            }

            if (BP.Sys.SystemConfig.AppCenterDBType == DBType.MySQL)
            {
                sql = @"SELECT * FROM (SELECT CONCAT('F', No) NO, CONCAT('F', ParentNo) PARENTNO, NAME, IDX, 1 ISPARENT,'FLOWTYPE' TTYPE,-1 DTYPE FROM WF_FlowSort " +
                     " WHERE OrgNo ='" + WebUser.OrgNo + "' or No = 1 " +
                     "union " +
                     "SELECT NO, CONCAT('F', FK_FlowSort) PARENTNO, CONCAT(NO, '.', NAME) NAME,IDX,0 ISPARENT,'FLOW' TTYPE, 0 as DTYPE FROM WF_Flow " +
                     " WHERE OrgNo ='" + WebUser.OrgNo + "') A  ORDER BY DTYPE, IDX";
            }

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (BP.Sys.SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dt.Columns["no"].ColumnName = "NO";
                dt.Columns["name"].ColumnName = "NAME";
                dt.Columns["parentno"].ColumnName = "PARENTNO";
                dt.Columns["idx"].ColumnName = "IDX";
                dt.Columns["isparent"].ColumnName = "ISPARENT";
                dt.Columns["ttype"].ColumnName = "TTYPE";
                dt.Columns["dtype"].ColumnName = "DTYPE";
            }

            //判断是否为空，如果为空，则创建一个流程根结点，added by liuxc,2016-01-24
            if (dt.Rows.Count == 0)
            {
                FlowSort fs = new FlowSort();
                fs.No = "99";
                fs.ParentNo = "0";
                fs.Name = "流程树";
                fs.Insert();
                dt.Rows.Add("F99", "F0", "流程树", 0, 1, "FLOWTYPE", -1);
            }
            else
            {
                DataRow[] drs = dt.Select("NAME='流程树'");
                if (drs.Length > 0 && !Equals(drs[0]["PARENTNO"], "F0"))
                    drs[0]["PARENTNO"] = "F0";
            }

            //如果为0。
            if (dt.Rows.Count == 0)
            {
                BP.WF.Port.Admin2.Org org = new Port.Admin2.Org(WebUser.OrgNo);
                org.DoCheck();
                return "err@系统出现错误，请刷新一次，如果仍然出现错误，请反馈给管理员.";
            }


            DataRow rootRow = dt.Select("PARENTNO='F0'")[0];
            DataRow newRootRow = dt.Select("NO='F" + WebUser.OrgNo + "'")[0];

            newRootRow["PARENTNO"] = "F0";
            DataTable newDt = dt.Clone();
            newDt.Rows.Add(newRootRow.ItemArray);
            GenerChildRows(dt, newDt, newRootRow);
            dt = newDt;

            string str = BP.Tools.Json.ToJson(dt);
            return str;
        }

        public void GenerChildRows(DataTable dt, DataTable newDt, DataRow parentRow)
        {
            DataRow[] rows = dt.Select("ParentNo='" + parentRow["NO"] + "'");
            foreach (DataRow r in rows)
            {
                newDt.Rows.Add(r.ItemArray);
                GenerChildRows(dt, newDt, r);
            }
        }

        public string GetBindingFormsTable()
        {
            string fk_flow = GetValFromFrmByKey("fk_flow");
            if (string.IsNullOrWhiteSpace(fk_flow))
                return "[]";

            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT wfn.FK_Frm NO,");
            sql.AppendLine("       smd.NAME,");
            sql.AppendLine("       NULL PARENTNO,");
            sql.AppendLine("       'FORM' TTYPE,");
            sql.AppendLine("       -1 DTYPE,");
            sql.AppendLine("       0 ISPARENT");
            sql.AppendLine("FROM   WF_FrmNode wfn");
            sql.AppendLine("       INNER JOIN Sys_MapData smd");
            sql.AppendLine("            ON  smd.No = wfn.FK_Frm");
            sql.AppendLine("WHERE  wfn.FK_Flow = '{0}'");
            sql.AppendLine("       AND wfn.FK_Node = (");
            sql.AppendLine("               SELECT wn.NodeID");
            sql.AppendLine("               FROM   WF_Node wn");
            sql.AppendLine("               WHERE  wn.FK_Flow = '{0}' AND wn.NodePosType = 0");
            sql.AppendLine("           )");

            DataTable dt = DBAccess.RunSQLReturnTable(string.Format(sql.ToString(), fk_flow));
            return BP.Tools.Json.ToJson(dt);
        }

        public string GetFormTreeTable()
        {
            #region 检查数据是否符合规范.
            string rootNo = DBAccess.RunSQLReturnStringIsNull("SELECT No FROM Sys_FormTree WHERE ParentNo='' OR ParentNo IS NULL", null);
            if (DataType.IsNullOrEmpty(rootNo) == false)
            {
                //删除垃圾数据.
                DBAccess.RunSQL(string.Format("DELETE FROM Sys_FormTree WHERE No='{0}'", rootNo));
            }
            //检查根目录是否符合规范.
            FrmTree ft = new FrmTree();
            ft.No = "1";
            if (ft.RetrieveFromDBSources() == 0)
            {
                ft.Name = "表单库";
                ft.ParentNo = "0";
                ft.Insert();
            }
            if (ft.ParentNo.Equals("0") == false)
            {
                ft.ParentNo = "0";
                ft.Update();
            }
            #endregion 检查数据是否符合规范.

            //如果是集团版
            if (Glo.CCBPMRunModel == CCBPMRunModel.GroupInc)
                return GetFormTreeTable_GroupInc();

            //组织数据源.
            string sqls = "";
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                sqls = "SELECT No \"No\", ParentNo \"ParentNo\",Name \"Name\", Idx \"Idx\", 1 \"IsParent\", 'FORMTYPE' \"TType\" FROM Sys_FormTree ORDER BY Idx ASC ; ";
                sqls += "SELECT No \"No\", FK_FormTree as \"ParentNo\", Name \"Name\",Idx \"Idx\", 0 \"IsParent\", 'FORM' \"TType\" FROM Sys_MapData  WHERE  AppType=0 AND FK_FormTree IN (SELECT No FROM Sys_FormTree) ORDER BY Idx ASC";
            }
            else
            {
                sqls = "SELECT No,ParentNo,Name, Idx, 1 IsParent, 'FORMTYPE' TType FROM Sys_FormTree    ORDER BY Idx ASC ; ";
                sqls += "SELECT No, FK_FormTree as ParentNo,Name,Idx,0 IsParent, 'FORM' TType FROM Sys_MapData  WHERE   AppType=0 AND FK_FormTree IN (SELECT No FROM Sys_FormTree) ORDER BY Idx ASC";
            }

            DataSet ds = DBAccess.RunSQLReturnDataSet(sqls);


            //获得表单数据.
            DataTable dtSort = ds.Tables[0]; //类别表.
            DataTable dtForm = ds.Tables[1].Clone(); //表单表,这个是最终返回的数据.

            if (SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dtForm.Columns["no"].ColumnName = "No";
                dtForm.Columns["name"].ColumnName = "Name";
                dtForm.Columns["parentno"].ColumnName = "ParentNo";
                dtForm.Columns["idx"].ColumnName = "Idx";
                dtForm.Columns["isparent"].ColumnName = "IsParent";
                dtForm.Columns["ttype"].ColumnName = "TType";
            }

            //增加顶级目录.
            DataRow[] rowsOfSort = dtSort.Select("ParentNo='0'");
            DataRow drFormRoot = dtForm.NewRow();
            drFormRoot[0] = rowsOfSort[0]["No"];
            drFormRoot[1] = "0";
            drFormRoot[2] = rowsOfSort[0]["Name"];
            drFormRoot[3] = rowsOfSort[0]["Idx"];
            drFormRoot[4] = rowsOfSort[0]["IsParent"];
            drFormRoot[5] = rowsOfSort[0]["TType"];
            dtForm.Rows.Add(drFormRoot); //增加顶级类别..

            //把类别数据组装到form数据里.
            foreach (DataRow dr in dtSort.Rows)
            {
                DataRow drForm = dtForm.NewRow();
                drForm[0] = dr["No"];
                drForm[1] = dr["ParentNo"];
                drForm[2] = dr["Name"];
                drForm[3] = dr["Idx"];
                drForm[4] = dr["IsParent"];
                drForm[5] = dr["TType"];
                dtForm.Rows.Add(drForm); //类别.
            }

            foreach (DataRow row in ds.Tables[1].Rows)
            {
                dtForm.Rows.Add(row.ItemArray);
            }

            String str = BP.Tools.Json.ToJson(dtForm);
            return str;
        }
        public string GetFormTreeTable_GroupInc()
        {
            //组织数据源.
            string sqls = "";

            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                sqls = "SELECT No \"No\", ParentNo \"ParentNo\",Name \"Name\", Idx \"Idx\", 1 \"IsParent\", 'FORMTYPE' \"TType\" FROM Sys_FormTree WHERE OrgNo ='" + WebUser.OrgNo + "'  or No = 1  ORDER BY Idx ASC ; ";
                sqls += "SELECT No \"No\", FK_FormTree as \"ParentNo\", Name \"Name\",Idx \"Idx\", 0 \"IsParent\", 'FORM' \"TType\" FROM Sys_MapData  WHERE OrgNo ='" + WebUser.OrgNo + "' AND AppType=0 AND FK_FormTree IN (SELECT No FROM Sys_FormTree) ORDER BY Idx ASC";
            }
            else
            {
                sqls = "SELECT No,ParentNo,Name, Idx, 1 IsParent, 'FORMTYPE' TType FROM Sys_FormTree where OrgNo ='" + WebUser.OrgNo + "'  or No = 1  ORDER BY Idx ASC ; ";
                sqls += "SELECT No, FK_FormTree as ParentNo,Name,Idx,0 IsParent, 'FORM' TType FROM Sys_MapData  WHERE OrgNo ='" + WebUser.OrgNo + "' AND AppType=0 AND FK_FormTree IN (SELECT No FROM Sys_FormTree) ORDER BY Idx ASC";
            }

            DataSet ds = DBAccess.RunSQLReturnDataSet(sqls);


            //获得表单数据.
            DataTable dtSort = ds.Tables[0]; //类别表.
            DataTable dtForm = ds.Tables[1].Clone(); //表单表,这个是最终返回的数据.

            if (SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dtForm.Columns["no"].ColumnName = "No";
                dtForm.Columns["name"].ColumnName = "Name";
                dtForm.Columns["parentno"].ColumnName = "ParentNo";
                dtForm.Columns["idx"].ColumnName = "Idx";
                dtForm.Columns["isparent"].ColumnName = "IsParent";
                dtForm.Columns["ttype"].ColumnName = "TType";
            }

            //增加顶级目录.
            DataRow[] rowsOfSort = dtSort.Select("ParentNo='0'");
            DataRow drFormRoot = dtForm.NewRow();
            drFormRoot[0] = rowsOfSort[0]["No"];
            drFormRoot[1] = "0";
            drFormRoot[2] = rowsOfSort[0]["Name"];
            drFormRoot[3] = rowsOfSort[0]["Idx"];
            drFormRoot[4] = rowsOfSort[0]["IsParent"];
            drFormRoot[5] = rowsOfSort[0]["TType"];
            dtForm.Rows.Add(drFormRoot); //增加顶级类别..

            //把类别数据组装到form数据里.
            foreach (DataRow dr in dtSort.Rows)
            {
                DataRow drForm = dtForm.NewRow();
                drForm[0] = dr["No"];
                drForm[1] = dr["ParentNo"];
                drForm[2] = dr["Name"];
                drForm[3] = dr["Idx"];
                drForm[4] = dr["IsParent"];
                drForm[5] = dr["TType"];
                dtForm.Rows.Add(drForm); //类别.
            }

            foreach (DataRow row in ds.Tables[1].Rows)
            {
                dtForm.Rows.Add(row.ItemArray);
            }

            /* if (WebUser.No.Equals("admin") == false)
             {*/
            DataRow[] rootRows = dtForm.Select("No='" + WebUser.OrgNo + "'");
            DataRow newRootRow = rootRows[0];

            newRootRow["ParentNo"] = "0";
            DataTable newDt = dtForm.Clone();
            newDt.Rows.Add(newRootRow.ItemArray);

            GenerChildRows(dtForm, newDt, newRootRow);
            dtForm = newDt;
            //}
            String str = BP.Tools.Json.ToJson(dtForm);
            return str;
        }

        public string GetStructureTreeTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("NO", typeof(string));
            dt.Columns.Add("PARENTNO", typeof(string));
            dt.Columns.Add("NAME", typeof(string));
            dt.Columns.Add("TTYPE", typeof(string));

            BP.GPM.Depts depts = new BP.GPM.Depts();
            depts.RetrieveAll();
            BP.Port.Stations sts = new BP.Port.Stations();
            sts.RetrieveAll();
            BP.GPM.Emps emps = new BP.GPM.Emps();
            emps.RetrieveAll(BP.WF.Port.EmpAttr.Name);
            BP.GPM.DeptStations dss = new BP.GPM.DeptStations();
            dss.RetrieveAll();
            BP.GPM.DeptEmpStations dess = new BP.GPM.DeptEmpStations();
            dess.RetrieveAll();
            BP.Port.Station stt = null;
            BP.GPM.Emp empt = null;

            foreach (BP.GPM.Dept dept in depts)
            {
                //增加部门
                dt.Rows.Add(dept.No, dept.ParentNo, dept.Name, "DEPT");

                //增加部门岗位
                dss.Retrieve(BP.GPM.DeptStationAttr.FK_Dept, dept.No);
                foreach (BP.GPM.DeptStation ds in dss)
                {
                    stt = sts.GetEntityByKey(ds.FK_Station) as Station;

                    if (stt == null) continue;

                    dt.Rows.Add(dept.No + "|" + ds.FK_Station, dept.No, stt.Name, "STATION");

                    //增加部门岗位人员
                    dess.Retrieve(BP.GPM.DeptEmpStationAttr.FK_Dept, dept.No, BP.GPM.DeptEmpStationAttr.FK_Station,
                                  ds.FK_Station);

                    foreach (BP.GPM.DeptEmpStation des in dess)
                    {
                        empt = emps.GetEntityByKey(des.FK_Emp) as BP.GPM.Emp;

                        if (empt == null) continue;

                        dt.Rows.Add(dept.No + "|" + ds.FK_Station + "|" + des.FK_Emp, dept.No + "|" + ds.FK_Station,
                                    empt.Name, "EMP");
                    }
                }
            }

            return BP.Tools.Json.ToJson(dt);
        }

        /// <summary>
        /// 获取设计器 - 系统维护菜单数据
        /// 系统维护管理员菜单 需要翻译
        /// </summary>
        /// <returns></returns>
        public string GetTreeJson_AdminMenu()
        {
            string treeJson = string.Empty;

            if (Glo.CCBPMRunModel == CCBPMRunModel.GroupInc)
            {
                //查询全部.
                Admin2MenuGroups groups = new Admin2MenuGroups();
                groups.RetrieveAll();

                Admin2Menus menus = new Admin2Menus();
                menus.RetrieveAll();

                // 定义容器.
                Admin2Menus newMenus = new Admin2Menus();

                foreach (Admin2MenuGroup menu in groups)
                {

                    //是否可以使用？
                    if (menu.IsCanUse(WebUser.No) == false)
                        continue;
                    Admin2Menu newMenu = new Admin2Menu();
                    newMenu.No = menu.No;
                    newMenu.Name = menu.Name;
                    newMenu.GroupNo = "0";
                    newMenu.For = menu.For;
                    newMenu.Url = "";
                    newMenus.Add(newMenu);
                }

                foreach (Admin2Menu menu in menus)
                {
                    newMenus.Add(menu);
                }
                //添加默认，无权限
                if (newMenus.Count == 0)
                {
                    Admin2Menu menu = new Admin2Menu();
                    menu.No = "1";
                    menu.GroupNo = "0";
                    menu.Name = "无权限";
                    menu.Url = "";
                    newMenus.Add(menu);
                }
                DataTable dt = newMenus.ToDataTable();
                treeJson = BP.Tools.Json.ToJson(newMenus.ToDataTable());
            }
            else
            {
                //查询全部.
                AdminMenuGroups groups = new AdminMenuGroups();
                groups.RetrieveAll();

                AdminMenus menus = new AdminMenus();
                menus.RetrieveAll();

                // 定义容器.
                AdminMenus newMenus = new AdminMenus();

                foreach (AdminMenuGroup menu in groups)
                {
                    //是否可以使用？
                    if (menu.IsCanUse(WebUser.No) == false)
                        continue;

                    AdminMenu newMenu = new AdminMenu();
                    newMenu.No = menu.No;
                    newMenu.Name = menu.Name;
                    newMenu.GroupNo = "0";
                    newMenu.For = menu.For;
                    newMenu.Url = "";
                    newMenus.Add(newMenu);
                }

                foreach (AdminMenu menu in menus)
                {
                    //是否可以使用？
                    if (menu.IsCanUse(WebUser.No) == false)
                        continue;

                    newMenus.Add(menu);
                }
                //添加默认，无权限
                if (newMenus.Count == 0)
                {
                    AdminMenu menu = new AdminMenu();
                    menu.No = "1";
                    menu.GroupNo = "0";
                    menu.Name = "无权限";
                    menu.Url = "";
                    newMenus.Add(menu);
                }
                DataTable dt = newMenus.ToDataTable();
                treeJson = BP.Tools.Json.ToJson(newMenus.ToDataTable());
            }
            return treeJson;
        }

        /// <summary>
        /// 根据DataTable生成Json树结构
        /// </summary>
        public string GetTreeJsonByTable(DataTable tabel, object pId, string rela = "ParentNo", string idCol = "No", string txtCol = "Name", string IsParent = "IsParent", string sChecked = "", string[] attrFields = null)
        {
            string treeJson = string.Empty;

            if (tabel.Rows.Count > 0)
            {
                sbJson.Append("[");
                string filer = string.Empty;
                if (pId.ToString() == "")
                {
                    filer = string.Format("{0} is null or {0}='-1' or {0}='0' or {0}='F0'", rela);
                }
                else
                {
                    filer = string.Format("{0}='{1}'", rela, pId);
                }

                DataRow[] rows = tabel.Select(filer, idCol);
                if (rows.Length > 0)
                {
                    for (int i = 0; i < rows.Length; i++)
                    {
                        DataRow row = rows[i];

                        string jNo = row[idCol] as string;
                        string jText = row[txtCol] as string;
                        if (jText.Length > 25)
                            jText = jText.Substring(0, 25) + "<img src='../Scripts/easyUI/themes/icons/add2.png' onclick='moreText(" + jNo + ")'/>";

                        string jIsParent = row[IsParent].ToString();
                        string jState = "1".Equals(jIsParent) ? "open" : "closed";
                        jState = "open".Equals(jState) && i == 0 ? "open" : "closed";

                        DataRow[] rowChild = tabel.Select(string.Format("{0}='{1}'", rela, jNo));
                        string tmp = "{\"id\":\"" + jNo + "\",\"text\":\"" + jText;

                        //增加自定义attributes列，added by liuxc,2015-10-6
                        var attrs = string.Empty;
                        if (attrFields != null && attrFields.Length > 0)
                        {
                            foreach (var field in attrFields)
                            {
                                if (!tabel.Columns.Contains(field)) continue;
                                if (DataType.IsNullOrEmpty(row[field].ToString()))
                                {
                                    attrs += ",\"" + field + "\":\"\"";
                                    continue;
                                }
                                attrs += ",\"" + field + "\":" +
                                         (tabel.Columns[field].DataType == typeof(string)
                                              ? string.Format("\"{0}\"", row[field])
                                              : row[field]);
                            }
                        }

                        if ("0".Equals(pId.ToString()) || row[rela].ToString() == "F0")
                        {
                            tmp += "\",\"attributes\":{\"IsParent\":\"" + jIsParent + "\",\"IsRoot\":\"1\"" + attrs + "}";
                        }
                        else
                        {
                            tmp += "\",\"attributes\":{\"IsParent\":\"" + jIsParent + "\"" + attrs + "}";
                        }

                        if (rowChild.Length > 0)
                        {
                            tmp += ",\"checked\":" + sChecked.Contains("," + jNo + ",").ToString().ToLower()
                                + ",\"state\":\"" + jState + "\"";
                        }
                        else
                        {
                            tmp += ",\"checked\":" + sChecked.Contains("," + jNo + ",").ToString().ToLower();
                        }

                        sbJson.Append(tmp);
                        if (rowChild.Length > 0)
                        {
                            sbJson.Append(",\"children\":");
                            GetTreeJsonByTable(tabel, jNo, rela, idCol, txtCol, IsParent, sChecked, attrFields);
                        }

                        sbJson.Append("},");
                    }
                    sbJson = sbJson.Remove(sbJson.Length - 1, 1);
                }
                sbJson.Append("]");
                treeJson = sbJson.ToString();
            }
            return treeJson;
        }
        /// <summary>
        /// 创建一个新流程模版2019版本.
        /// </summary>
        /// <returns></returns>
        public string Defualt_NewFlow()
        {
            try
            {
                int runModel = this.GetRequestValInt("RunModel");
                string FlowName = this.GetRequestVal("FlowName");
                string FlowSort = this.GetRequestVal("FlowSort").Trim();
                FlowSort = FlowSort.Trim();

                int DataStoreModel = this.GetRequestValInt("DataStoreModel");
                string PTable = this.GetRequestVal("PTable");
                string FlowMark = this.GetRequestVal("FlowMark");
                int flowFrmType = this.GetRequestValInt("FlowFrmType");
                string FrmUrl = this.GetRequestVal("FrmUrl");
                string FlowVersion = this.GetRequestVal("FlowVersion");

                string flowNo = BP.WF.Template.TemplateGlo.NewFlow(FlowSort, FlowName,
                        Template.DataStoreModel.SpecTable, PTable, FlowMark, FlowVersion);

                Flow fl = new Flow(flowNo);

                #region 对极简版特殊处理. @liuqiang
                //如果是简洁版.
                if (runModel == 1)
                {
                    fl.FlowFrmType = (BP.WF.FlowFrmType)flowFrmType;
                    fl.Update(); //更新表单类型.

                    //预制权限数据.
                    int nodeID = int.Parse(fl.No + "01");
                    FrmNode fn = new FrmNode();
                    fn.FK_Frm = "ND" + nodeID;
                    fn.IsEnableFWC = FrmWorkCheckSta.Disable;
                    fn.FK_Node = nodeID;
                    fn.FK_Flow = flowNo;
                    fn.FrmSln = FrmSln.Default;
                    fn.Insert();

                    nodeID = int.Parse(fl.No + "02");
                    fn = new FrmNode();
                    fn.FK_Frm = "ND" + nodeID;
                    fn.IsEnableFWC = FrmWorkCheckSta.Disable;
                    fn.FK_Node = nodeID;
                    fn.FK_Flow = flowNo;
                    fn.FrmSln = FrmSln.Default;
                    fn.Insert();

                }
                #endregion 对极简版特殊处理. @liuqiang


                //清空WF_Emp 的StartFlows ,让其重新计算.
                DBAccess.RunSQL("UPDATE  WF_Emp Set StartFlows =''");
                return flowNo;
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 上移流程类别
        /// </summary>
        /// <returns></returns>
        public String MoveUpFlowSort()
        {
            String fk_flowSort = this.GetRequestVal("FK_FlowSort").Replace("F", "");
            FlowSort fsSub = new FlowSort(fk_flowSort); //传入的编号多出F符号，需要替换掉
            fsSub.DoUp();
            return "F" + fsSub.No;
        }
        /// <summary>
        /// 下移流程类别
        /// </summary>
        /// <returns></returns>
        public String MoveDownFlowSort()
        {
            String fk_flowSort = this.GetRequestVal("FK_FlowSort").Replace("F", "");
            FlowSort fsSub = new FlowSort(fk_flowSort); //传入的编号多出F符号，需要替换掉
            fsSub.DoDown();
            return "F" + fsSub.No;
        }
        /// <summary>
        /// 上移流程
        /// </summary>
        /// <returns></returns>
        public string MoveUpFlow()
        {
            Flow flow = new Flow(this.FK_Flow);
            flow.DoUp();
            return flow.No;
        }
        /// <summary>
        /// 下移流程
        /// </summary>
        /// <returns></returns>
        public string MoveDownFlow()
        {
            Flow flow = new Flow(this.FK_Flow);
            flow.DoDown();
            return flow.No;
        }
        /// <summary>
        /// 删除流程类别.
        /// </summary>
        /// <returns></returns>
        public string DelFlowSort()
        {
            string fk_flowSort = this.GetRequestVal("FK_FlowSort").Replace("F", "");

            FlowSort fs = new FlowSort();
            fs.No = fk_flowSort;

            //检查是否有流程？
            Paras ps = new Paras();
            ps.SQL = "SELECT COUNT(*) FROM WF_Flow WHERE FK_FlowSort=" + SystemConfig.AppCenterDBVarStr + "fk_flowSort";
            ps.Add("fk_flowSort", fk_flowSort);
            //string sql = "SELECT COUNT(*) FROM WF_Flow WHERE FK_FlowSort='" + fk_flowSort + "'";
            if (DBAccess.RunSQLReturnValInt(ps) != 0)
                return "err@该目录下有流程，您不能删除。";

            //检查是否有子目录？
            ps = new Paras();
            ps.SQL = "SELECT COUNT(*) FROM WF_FlowSort WHERE ParentNo=" + SystemConfig.AppCenterDBVarStr + "ParentNo";
            ps.Add("ParentNo", fk_flowSort);
            //sql = "SELECT COUNT(*) FROM WF_FlowSort WHERE ParentNo='" + fk_flowSort + "'";
            if (DBAccess.RunSQLReturnValInt(ps) != 0)
                return "err@该目录下有子目录，您不能删除。";

            fs.Delete();

            return "删除成功.";
        }
        /// <summary>
        /// 新建同级流程类别 对照需要翻译
        /// </summary>
        /// <returns></returns>
        public string NewSameLevelFlowSort()
        {
            FlowSort fs = null;
            fs = new FlowSort(this.No.Replace("F", "")); //传入的编号多出F符号，需要替换掉.

            string orgNo = fs.OrgNo; //记录原来的组织结构编号. 对照需要翻译

            string sameNodeNo = fs.DoCreateSameLevelNode().No;
            fs = new FlowSort(sameNodeNo);
            fs.Name = this.Name;
            fs.OrgNo = orgNo; // 组织结构编号. 对照需要翻译
            fs.Update();
            return "F" + fs.No;
        }
        /// <summary>
        /// 新建下级类别. 
        /// </summary>
        /// <returns></returns>
        public string NewSubFlowSort()
        {
            FlowSort fsSub = new FlowSort(this.No.Replace("F", ""));//传入的编号多出F符号，需要替换掉.
            string orgNo = fsSub.OrgNo; //记录原来的组织结构编号. 对照需要翻译

            string subNodeNo = fsSub.DoCreateSubNode().No;
            FlowSort subFlowSort = new FlowSort(subNodeNo);
            subFlowSort.Name = this.Name;
            subFlowSort.OrgNo = orgNo; // 组织结构编号. 对照需要翻译.
            subFlowSort.Update();
            return "F" + subFlowSort.No;
        }
        /// <summary>
        /// 表单树 - 删除表单类别
        /// </summary>
        /// <returns></returns>
        public string CCForm_DelFormSort()
        {
            SysFormTree formTree = new SysFormTree(this.No);

            //检查是否有子类别？
            Paras ps = new Paras();
            ps.SQL = "SELECT COUNT(*) FROM Sys_FormTree WHERE ParentNo=" + SystemConfig.AppCenterDBVarStr + "ParentNo";
            ps.Add("ParentNo", this.No);
            //string sql = "SELECT COUNT(*) FROM Sys_FormTree WHERE ParentNo='" + this.No + "'";
            if (DBAccess.RunSQLReturnValInt(ps) != 0)
                return "err@该目录下有子类别，您不能删除。";

            //检查是否有表单？
            ps = new Paras();
            ps.SQL = "SELECT COUNT(*) FROM Sys_MapData WHERE FK_FormTree=" + SystemConfig.AppCenterDBVarStr + "FK_FormTree";
            ps.Add("FK_FormTree", this.No);
            //sql = "SELECT COUNT(*) FROM Sys_MapData WHERE FK_FormTree='" + this.No + "'";
            if (DBAccess.RunSQLReturnValInt(ps) != 0)
                return "err@该目录下有表单，您不能删除。";

            formTree.Delete();
            return "删除成功";
        }

        /// <summary>
        /// 表单树 - 删除表单
        /// </summary>
        /// <returns></returns>
        public string CCForm_DeleteCCFormMapData()
        {
            try
            {
                MapData mapData = new MapData(this.FK_MapData);
                mapData.Delete();
                return "删除成功.";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }


        /// <summary>
        /// 让admin 登陆
        /// </summary>
        /// <param name="lang">当前的语言</param>
        /// <returns>成功则为空，有异常时返回异常信息</returns>
        public string LetAdminLogin(string empNo, bool islogin)
        {
            try
            {
                if (islogin)
                {
                    BP.Port.Emp emp = new BP.Port.Emp(empNo);
                    WebUser.SignInOfGener(emp);
                }
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            return "@登录成功.";
        }
        #endregion

    }
}
