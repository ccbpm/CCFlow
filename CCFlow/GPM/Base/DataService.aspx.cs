using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.Xml;
using BP.Sys;
using BP.En;
using BP.DA;
using BP.Web;
using BP.GPM;
using BP.GPM.Utility;
using BP.EAI.Plugins;
using BP.EAI.Plugins.DINGTalk;

namespace GMP2.GPM
{
    public partial class DataService : WebPage
    {
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(Request[param], System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// 当前登录人，二级管理员拥有系统权限
        /// </summary>
        private string HaveRightApps
        {
            get
            {
                if (WebUser.No == "admin")
                    return "admin";
                string strApps = ",UnitFullName,AppSort,";
                //用户拥有的系统权限
                EmpApps empApps = new EmpApps();
                empApps.RetrieveByAttr(EmpAppAttr.FK_Emp, WebUser.No);
                foreach (EmpApp empApp in empApps)
                {
                    strApps += "," + empApp.FK_App + ",";
                }
                return strApps;
            }
        }
        //页面加载
        protected void Page_Load(object sender, EventArgs e)
        {
            //返回值
            string s_responsetext = string.Empty;

            if (BP.Web.WebUser.No == null)
            {
                s_responsetext = "nologin";
                //组装ajax字符串格式,返回调用客户端
                Response.Charset = "UTF-8";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.ContentType = "text/html";
                Response.Expires = 0;
                Response.Write(s_responsetext);
                Response.End();
            }
            string method = string.Empty;
            if (!string.IsNullOrEmpty(Request["method"]))
                method = Request["method"].ToString();

            switch (method)
            {
                case "getemps"://获取所有人员信息
                    s_responsetext = GetEmps();
                    break;
                case "getempsbynoorname"://根据用户名或编号模糊查找用户
                    s_responsetext = GetEmpsByNoOrName();
                    break;
                case "searchbyempnoorname"://根据用户账号、工号、姓名或手机号
                    s_responsetext = SearchByEmpNoOrName();
                    break;
                case "getempsbynameordept"://根据用户姓名或部门模糊查询用户列表
                    s_responsetext = getEmpsByNameOrDept();
                    break;
                case "getempgroups"://查找所有权限组
                    s_responsetext = GetEmpGroups();
                    break;
                case "getempgroupsbyname"://权限组模糊查找
                    s_responsetext = GetEmpGroupsByName();
                    break;
                case "getapps"://获取所有系统
                    s_responsetext = GetApps();
                    break;
                case "getmenusofmenuforemp"://获取所有目录菜单
                    s_responsetext = GetMenusOfMenuForEmp();
                    break;
                case "getmenusbyid"://根据编号获取菜单
                    s_responsetext = GetMenusById();
                    break;
                case "getleftmenu"://左侧菜单
                    s_responsetext = GetLeftMenu();
                    break;
                case "getsystemmenu":// 获取系统菜单
                    s_responsetext = GetSystemMenus();
                    break;
                case "menunodemanage"://菜单管理
                    s_responsetext = MenuNodeManage();
                    break;
                case "getmenubyempno"://根据用户编号查找菜单
                    s_responsetext = GetMenuByEmpNo();
                    break;
                case "getdeptemptree"://获取部门人员信息
                    s_responsetext = GetDeptEmpTree();
                    break;
                case "getdeptempchildnodes"://根据节点编号获取子部门人员
                    s_responsetext = GetDeptEmpChildNodes();
                    break;
                case "getempofmenusbyempno"://用户菜单权限
                    s_responsetext = GetEmpOfMenusByEmpNo();
                    break;
                case "saveuserofmenus"://保存用户与菜单的对应关系
                    s_responsetext = SaveUserOfMenus();
                    break;
                case "getempgroupofmenusbyno"://获取权限组菜单
                    s_responsetext = GetEmpGroupOfMenusByNo();
                    break;
                case "saveusergroupofmenus"://保存权限组菜单
                    s_responsetext = SaveUserGroupOfMenus();
                    break;
                case "clearofcopyuserpower"://清空式复制用户权限
                    s_responsetext = ClearOfCopyUserPower();
                    break;
                case "coverofcopyuserpower"://覆盖式复制用户权限
                    s_responsetext = CoverOfCopyUserPower();
                    break;
                case "clearofcopyusergrouppower"://清空式复制权限组权限
                    s_responsetext = ClearOfCopyUserGroupPower();
                    break;
                case "coverofcopyusergrouppower"://覆盖式覆盖权限组
                    s_responsetext = CoverOfCopyUserGroupPower();
                    break;
                case "getAppChildMenus"://打开新窗口 菜单获取
                    s_responsetext = getMenu();
                    break;
                case "GetAllDept"://获取所有部门
                    s_responsetext = GetAllDept();
                    break;
                case "gettemplatedata"://按菜单分配权限，获取模版数据
                    s_responsetext = getTemplateData();
                    break;
                case "savemenuforemp"://保存按菜单分配权限
                    s_responsetext = SaveMenuForEmp();
                    break;
                case "getsystemloginlogs"://获取系统登录日志
                    s_responsetext = GetSystemLoginLogs();
                    break;
                case "getstations"://获取所有岗位
                    s_responsetext = GetStations();
                    break;
                case "savestationofmenus":// 保存岗位菜单
                    s_responsetext = SaveStationOfMenus();
                    break;
                case "getstationofmenusbyno"://获取岗位菜单
                    s_responsetext = GetStationOfMenusByNo();
                    break;
                case "clearofcopystation"://清空式 复制岗位
                    s_responsetext = ClearOfCopyStation();
                    break;
                case "coverofcopystation"://覆盖式 复制岗位
                    s_responsetext = CoverOfCopyStation();
                    break;
                case "getstationbyname":// 岗位 模糊查找
                    s_responsetext = GetStationByName();
                    break;
                case "getapptreeforadmin"://获取分配给二级管理员系统
                    s_responsetext = GetAppTreeForAdmin();
                    break;
                case "loaddatagridempapp"://获取系统的管理员
                    s_responsetext = LoadDataGridEmpApp();
                    break;
                case "saveempapp"://保存系统管理员
                    s_responsetext = SaveEmpApp();
                    break;
                case "deleteempapp"://删除系统管理员
                    s_responsetext = DeleteEmpApp();
                    break;
                case "generstationtree"://获取岗位类型与岗位形成的树形
                    s_responsetext = GPM_StationTree();
                    break;
                case "getdeptempstationtemplatedata"://根据岗位获取人员
                    s_responsetext = GPM_GenerEmpsByStationNo();
                    break;
                case "savestationfordeptemps"://保存岗位到部门人员
                    s_responsetext = GPM_SaveStationFormDeptEmps();
                    break;
                #region 二级管理员 部门管理
                case "getManagerDept":
                    s_responsetext = GetManagerDept();
                    break;
                case "saveDeptManager"://保存部门管理员
                    s_responsetext = SaveDeptManager();
                    break;
                case "loaddatagridDeptManager"://获取部门的管理员
                    s_responsetext = LoadDatagridDeptManager();
                    break;
                case "deleteempdept"://删除部门管理员
                    s_responsetext = deleteempdept();
                    break;
                case "getOrganizationDept"://获取getOrganizationDept树  权限问题
                    s_responsetext = GetOrganizationDept();
                    break;
                case "loaddatagriddeptemp"://加载部门人员
                    s_responsetext = LoadDatagridDeptEmp();
                    break;
                case "savedeptemp"://保存部门新增人员信息
                    s_responsetext = SaveDeptEmp();
                    break;
                case "deletedeptemp"://删除部门人员
                    s_responsetext = DeleteDeptEmp();
                    break;
                case "disabledeptemp"://禁用人员
                    s_responsetext = DisableDeptEmp();
                    break;
                case "generdisableemps"://获取已禁用用户列表
                    s_responsetext = GenerDisableEmps();
                    break;
                case "replaceempbelongdept"://调整人员所属主部门
                    s_responsetext = ReplaceEmpbelongDept();
                    break;
                case "appendDataMet"://新增部门
                    s_responsetext = appendDataMet();
                    break;
                case "deleteNodeMet"://删除部门
                    s_responsetext = deleteNodeMet();
                    break;
                case "floatNodeMet"://上/下移部门
                    s_responsetext = floatNodeMet();
                    break;
                case "EmpFloatNodeMet"://上/下移部门
                    s_responsetext = EmpFloatNodeMet();
                    break;
                case "checkDeptInfoMet"://获取部门基本信息
                    s_responsetext = checkDeptInfoMet();
                    break;
                case "saveDeptInfoMet"://保存部门信息
                    s_responsetext = saveDeptInfoMet();
                    break;
                case "doSearchMet":
                    s_responsetext = doSearchMet();
                    break;
                case "getEmpInfoMet"://人员信息
                    s_responsetext = getEmpInfoMet();
                    break;
                case "editDeptEmpMet"://保存人员信息
                    s_responsetext = editDeptEmpMet();
                    break;
                case "checkEmpNoMet":
                    s_responsetext = checkEmpNoMet();
                    break;
                case "getDutyStationDllInfoMet"://新增人员时加载岗位，职务信息
                    s_responsetext = getDutyStationDllInfoMet();
                    break;
                case "getOtherEmpsMet"://读取关联人员信息
                    s_responsetext = getOtherEmpsMet();
                    break;
                case "glEmpMet"://保存关联人员信息
                    s_responsetext = glEmpMet();
                    break;
                case "modifyPwdMet"://密码重置
                    s_responsetext = modifyPwdMet();
                    break;
                case "checkDeptDutyAndStationMet"://检查该部门职务和岗位是否健全
                    s_responsetext = checkDeptDutyAndStationMet();
                    break;
                #endregion
            }
            if (string.IsNullOrEmpty(s_responsetext))
                s_responsetext = "";
            //组装ajax字符串格式,返回调用客户端
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "text/html";
            Response.Expires = 0;
            Response.Write(s_responsetext);
            Response.End();
        }

        /// <summary>
        /// 岗位类型与岗位组成的树形数据
        /// </summary>
        /// <returns></returns>
        private string GPM_StationTree()
        {
            //岗位类型
            StationTypes stationTypes = new StationTypes();
            stationTypes.RetrieveAllFromDBSource();
            //所有岗位
            Stations stations = new Stations();
            stations.RetrieveAllFromDBSource();
            //借用菜单树形结构
            BP.GPM.Menus menus = new BP.GPM.Menus();
            BP.GPM.Menu rootMenu = new BP.GPM.Menu();
            rootMenu.No = "R0";
            rootMenu.Name = "岗位";
            rootMenu.ParentNo = "0";
            rootMenu.MenuType = 0;
            //添加根节点
            menus.AddEntity(rootMenu);
            //添加岗位类型与岗位
            foreach (StationType stationType in stationTypes)
            {
                BP.GPM.Menu typeMenu = new BP.GPM.Menu();
                typeMenu.No = "T" + stationType.No;
                typeMenu.Name = stationType.Name;
                typeMenu.ParentNo = "R0";
                typeMenu.MenuType = 3;
                menus.AddEntity(typeMenu);
                foreach (Station station in stations)
                {
                    if (!stationType.No.Equals(station.FK_StationType))
                        continue;
                    BP.GPM.Menu menu = new BP.GPM.Menu();
                    menu.No = station.No;
                    menu.Name = station.Name;
                    menu.ParentNo = "T" + station.FK_StationType;
                    menu.MenuType = 5;
                    menus.AddEntity(menu);
                }
            }
            //生成数据
            TansEntitiesToGenerTree(menus, "0", "");
            return appendMenus.ToString();
        }

        /// <summary>
        /// 根据岗位编号获取人员
        /// </summary>
        /// <returns></returns>
        private string GPM_GenerEmpsByStationNo()
        {
            string fk_station = this.getUTF8ToString("FK_Station");
            string fk_dept = this.getUTF8ToString("FK_Dept");
            string onlyChecked = this.getUTF8ToString("ViewModel");
            string sql_Dept = "SELECT distinct No,Name,ParentNo,NameOfPath,Idx "
                + " FROM Port_Dept a,Port_DeptEmp b "
                + " where a.No = b.FK_Dept";

            string sql_Emps = "SELECT distinct a.No,a.Name,b.FK_Dept,"
                        + "(SELECT case when COUNT(FK_Dept) > 0 then 1 else 0 end "
                        + " FROM Port_DeptEmpStation WHERE FK_Emp=a.No and FK_Dept=b.FK_Dept AND FK_Station='" + fk_station + "') isCheck"
                        + " FROM Port_Emp a,Port_DeptEmp b"
                        + " WHERE a.No=b.FK_Emp";
            //只显示已选中
            if (onlyChecked == "1")
            {
                sql_Dept = "SELECT distinct No,Name,ParentNo,NameOfPath,Idx "
                    + " FROM Port_Dept a,Port_DeptEmpStation b "
                    + " where a.No = b.FK_Dept AND b.FK_Station='" + fk_station + "'";

                sql_Emps = "SELECT distinct a.No,a.Name,b.FK_Dept,1 isCheck"
                    + " FROM Port_Emp a,Port_DeptEmpStation b"
                    + " WHERE a.No=b.FK_Emp AND b.FK_Station='" + fk_station + "'";
            }

            //添加部门条件
            string deptIn = "";
            if (!string.IsNullOrEmpty(fk_dept) && fk_dept != "1")
            {
                deptIn = "'" + fk_dept + "'";
                GetChildDeptIds(fk_dept, ref deptIn);
            }
            else
            {
                //根目录
                Dept rootDept = new Dept();
                rootDept.RetrieveByAttr(DeptAttr.ParentNo, "0");
                //二级部门
                Depts depts = new Depts(rootDept.No);
                deptIn = "'" + rootDept.No + "'";
                foreach (Dept dept in depts)
                {
                    deptIn += ",'" + dept.No + "'";
                }
            }
            sql_Dept += " AND b.FK_Dept in (" + deptIn + ")";
            sql_Emps += " AND b.FK_Dept in (" + deptIn + ")";
            sql_Dept += " order by ParentNo,Idx";

            DataTable dt = DBAccess.RunSQLReturnTable(sql_Dept);
            string strdept = CommonDbOperator.GetListJsonFromTable(dt);
            DataTable dt_Emp = DBAccess.RunSQLReturnTable(sql_Emps);
            string stremp = CommonDbOperator.GetListJsonFromTable(dt_Emp);
            return "{bmList:" + strdept + ",empList:" + stremp + "}";
        }

        private void GetChildDeptIds(string FK_Dept,ref string deptIds)
        {
            Depts depts = new Depts(FK_Dept);
            foreach (Dept dept in depts)
            {
                if (deptIds.Contains(dept.No))
                    continue;

                deptIds += ",'" + dept.No + "'";
                GetChildDeptIds(dept.No,ref deptIds);
            }
        }

        /// <summary>
        /// 保存岗位到部门人员
        /// </summary>
        /// <returns></returns>
        private string GPM_SaveStationFormDeptEmps()
        {
            try
            {
                string FK_Station = this.getUTF8ToString("FK_Station");
                string FK_Dept = this.getUTF8ToString("FK_Dept");
                string deptAndEmps = this.getUTF8ToString("Vals");
                string isClearSave = this.getUTF8ToString("IsClearSave");

                //截图数据
                string[] arrary_DeptEmp = deptAndEmps.Split(',');
                //清空保存
                if (isClearSave == "1")
                {
                    //select * from Port_DeptEmpStation
                    //select * from Port_DeptStation
                    DeptStation deptStation = new DeptStation();
                    deptStation.Delete(DeptStationAttr.FK_Station, FK_Station, DeptStationAttr.FK_Dept, FK_Dept);
                    DeptEmpStation deptEmpStation = new DeptEmpStation();
                    deptEmpStation.Delete(DeptEmpStationAttr.FK_Station, FK_Station, DeptEmpStationAttr.FK_Dept, FK_Dept);
                    return "true";
                }

                DeptEmpStation des = new DeptEmpStation();
                //先删除，再添加
                des.Delete(DeptEmpStationAttr.FK_Station, FK_Station, DeptEmpStationAttr.FK_Dept, FK_Dept);

                foreach (string item in arrary_DeptEmp)
                {
                    if (string.IsNullOrEmpty(item) || item.Contains("@") == false)
                        continue;

                    string[] arrary = item.Split('@');
                    if (arrary.Length == 2 && !string.IsNullOrEmpty(arrary[0]))
                    {
                        DeptStation deptStation = new DeptStation(arrary[0], FK_Station);
                        DeptEmpStation deptEmpStation = new DeptEmpStation();

                        deptEmpStation.MyPK = arrary[0] + "_" + arrary[1] + "_" + FK_Station;
                        if (!deptEmpStation.IsExit("MyPK", deptEmpStation.MyPK))
                        {
                            deptEmpStation.FK_Dept = arrary[0];
                            deptEmpStation.FK_Emp = arrary[1];
                            deptEmpStation.FK_Station = FK_Station;
                            deptEmpStation.Insert();
                        }
                    }
                }
                return "true";
            }
            catch (Exception ex)
            {
            }
            return "false";
        }

        #region 二级管理员权限  -----对数据进行操作时，考虑了数据表的级联性。但是不包含流程表 qin
        /// <summary>
        /// 获取分配给二级管理员系统
        /// </summary>
        /// <returns></returns>
        private string GetAppTreeForAdmin()
        {
            string rootNo = getUTF8ToString("rootNo");

            //根据父节点编号获取子节点
            BP.GPM.Menus menus = new BP.GPM.Menus();
            menus.RetrieveByAttr("ParentNo", rootNo);

            StringBuilder appSend = new StringBuilder();
            appSend.Append("[");
            foreach (EntityTree item in menus)
            {
                if (appSend.Length > 1) appSend.Append(",{"); else appSend.Append("{");

                appSend.Append("\"id\":\"" + item.No + "\"");
                appSend.Append(",\"text\":\"" + item.Name + "\"");

                BP.GPM.Menu menu = item as BP.GPM.Menu;

                //节点图标
                string ico = "icon-" + menu.MenuType;

                appSend.Append(",iconCls:\"");
                appSend.Append(ico);
                appSend.Append("\"");
                appSend.Append(",\"children\":");
                //增加二级既系统类别
                appSend.Append(GetMenusByParentNo(item.No, "", "", true));

                appSend.Append("}");
            }
            appSend.Append("]");

            return appSend.ToString();
        }
        /// <summary>
        /// 获取该系统的管理员列表
        /// </summary>
        /// <returns></returns>
        private string LoadDataGridEmpApp()
        {
            string menuNo = getUTF8ToString("menuNo");
            string orderBy = getUTF8ToString("orderBy");
            BP.GPM.Menu menu = new BP.GPM.Menu();
            menu.RetrieveByAttr(MenuAttr.No, menuNo);

            //默认按系统排序
            if (string.IsNullOrEmpty(orderBy)) orderBy = " order by c.FK_App,b.Idx";
            if (orderBy == "emp") orderBy = " order by a.Name,c.FK_App";
            if (orderBy == "dept") orderBy = " order by b.Idx,c.FK_App";

            string sql = "select c.MyPK,b.Name DeptName,a.Name,c.Name AppName from Port_Emp a,Port_Dept b,GPM_EmpApp c "
                + "where a.FK_Dept=b.No and a.No=c.FK_Emp and FK_App='" + menu.FK_App + "'";
            //如果为根节点则显示所有系统
            if (menu.FK_App == "UnitFullName")
            {
                sql = "select c.MyPK,b.Name DeptName,a.Name,c.Name AppName,c.FK_App,b.Idx from Port_Emp a,Port_Dept b,GPM_EmpApp c "
                + "where a.FK_Dept=b.No and a.No=c.FK_Emp";
            }
            else if (menu.FK_App == "AppSort")//显示此类型的系统
            {
                sql = "select c.MyPK,b.Name DeptName,a.Name,c.Name AppName,c.FK_App,b.Idx "
                    + "from Port_Emp a,Port_Dept b,GPM_EmpApp c,GPM_Menu d "
                    + "where a.FK_Dept=b.No and a.No=c.FK_Emp and c.FK_App=d.FK_App "
                    + "and d.ParentNo='" + menuNo + "'";
            }
            //增加排序
            sql += orderBy;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return CommonDbOperator.GetJsonFromTable(dt);
        }
        /// <summary>
        /// 获取该部门的管理员列表  qin15、7、4
        /// </summary>
        /// <returns></returns>
        private string LoadDatagridDeptManager()
        {
            string deptNo = getUTF8ToString("deptNo");
            string orderBy = getUTF8ToString("orderBy");

            string pageNumber = getUTF8ToString("pageNumber");
            int iPageNumber = string.IsNullOrEmpty(pageNumber) ? 1 : Convert.ToInt32(pageNumber);
            //每页多少行
            string pageSize = getUTF8ToString("pageSize");
            int iPageSize = string.IsNullOrEmpty(pageSize) ? 9999 : Convert.ToInt32(pageSize);


            //默认排序
            if (string.IsNullOrEmpty(orderBy)) orderBy = "  name,deptName";
            if (orderBy == "emp") orderBy = " name ";
            if (orderBy == "dept") orderBy = " deptName ";

            string sql = " (select c.MyPK,a.no,a.name,a.empno,b.name  deptName,email,a.leader,a.PersonalID from port_emp a,port_dept b, GPM_DeptManager c "
                      + "where c.fk_dept='" + deptNo + "' and a.fk_dept=b.no and a.no=c.fk_emp ) dbSourse ";

            return DBPaging(sql, iPageNumber, iPageSize, "no", orderBy);
        }
        /// <summary>
        /// 获取该部门的所有人员
        /// </summary>
        /// <returns></returns>
        private string LoadDatagridDeptEmp()// qin  gai 分页
        {
            string deptNo = getUTF8ToString("deptNo");
            if (string.IsNullOrEmpty(deptNo))
            {
                return "{ total: 0, rows: [] }";
            }
            string orderBy = getUTF8ToString("orderBy");


            string searchText = getUTF8ToString("searchText");
            string addQue = "";
            if (!string.IsNullOrEmpty(searchText))
            {
                addQue = "  AND (pe.No like '%" + searchText + "%' or pe.Name like '%" + searchText + "%') ";
            }

            string pageNumber = getUTF8ToString("pageNumber");
            int iPageNumber = string.IsNullOrEmpty(pageNumber) ? 1 : Convert.ToInt32(pageNumber);
            //每页多少行
            string pageSize = getUTF8ToString("pageSize");
            int iPageSize = string.IsNullOrEmpty(pageSize) ? 9999 : Convert.ToInt32(pageSize);

            string sql = "(select pe.*,pd.name FK_DutyText from port_emp pe left join port_duty pd on pd.no = pe.fk_duty where pe.no in (select fk_emp from Port_DeptEmp where fk_dept='" + deptNo + "') "
                + addQue + " ) dbSo ";


            return DBPaging(sql, iPageNumber, iPageSize, "No", orderBy);

        }
        /// <summary>
        /// 保存系统管理员
        /// </summary>
        /// <returns></returns>
        private string SaveEmpApp()
        {
            try
            {
                string emps = getUTF8ToString("emps");
                string menuNo = getUTF8ToString("menuNo");
                string[] empArrary = emps.Split(',');

                BP.GPM.Menu menu = new BP.GPM.Menu();
                menu.RetrieveByAttr(MenuAttr.No, menuNo);

                //添加系统管理员
                foreach (string FK_Emp in empArrary)
                {
                    EmpApp me = new EmpApp();
                    me.Name = menu.Name;
                    me.Url = menu.Url;
                    me.FK_Emp = FK_Emp;
                    me.FK_App = menu.FK_App;
                    me.MyPK = menu.FK_App + "_" + me.FK_Emp;
                    me.DirectSave();
                }
                return "true";
            }
            catch (Exception)
            {
                return "false";
            }
        }
        /// <summary>
        /// 保存部门管理员---qin 15/7/4
        /// </summary>
        /// <returns></returns>
        private string SaveDeptManager()
        {
            try
            {
                string emps = getUTF8ToString("emps");
                string deptNo = getUTF8ToString("deptNo");
                string[] empArrary = emps.Split(',');

                //添加部门管理员
                foreach (string FK_Emp in empArrary)
                {
                    if (string.IsNullOrEmpty(FK_Emp))
                        continue;

                    DeptManager dm = new DeptManager();
                    dm.CheckPhysicsTable();
                    dm.MyPK = FK_Emp + "_" + deptNo;
                    dm.FK_Emp = FK_Emp;
                    dm.FK_Dept = deptNo;

                    dm.DirectSave();
                }
                return "true";
            }
            catch (Exception)
            {
                return "false";
            }
        }
        /// <summary>
        /// 保存部门新增人员信息
        /// </summary>
        /// <returns></returns>
        private string SaveDeptEmp()
        {
            string infoStr = getUTF8ToString("infoStr");
            string deptNo = getUTF8ToString("deptNo");
            string[] infoStrArrary = infoStr.Split(',');
            string empStationStr = getUTF8ToString("empStationStr");

            try
            {

                //添加部门人员
                Emp emp = new Emp();
                emp.Name = infoStrArrary[0];
                emp.No = infoStrArrary[1];
                emp.EmpNo = infoStrArrary[2];
                emp.FK_Duty = infoStrArrary[3];
                emp.Tel = infoStrArrary[4];
                emp.Email = infoStrArrary[5];
                emp.Leader = infoStrArrary[6];
                emp.FK_Dept = deptNo;
                emp.SignType = string.IsNullOrEmpty(infoStrArrary[8]) ? 0 : int.Parse(infoStrArrary[8]);
                emp.PersonalID = infoStrArrary[9];
                //如果启用钉钉通讯录同步，新增人员
                if (BP.GPM.Glo.IsEnable_DingDing == true)
                {
                    DingDing ding = new DingDing();
                    CreateUser_PostVal postVal = ding.GPM_Ding_CreateEmp(emp);
                    //新增成功，或在钉钉已经存在此用户
                    if (postVal.errcode == "0" || postVal.errcode == "60102")
                        emp.No = postVal.userid;
                    else
                        return "err:" + postVal.errcode + "-钉钉-" + postVal.errmsg;
                }
                //如果用户账号为空，则分配一个。
                if (string.IsNullOrWhiteSpace(emp.No) || string.IsNullOrEmpty(emp.No))
                {
                    //检查随机是否与现有数据重复
                    while (true)
                    {
                        Emp tempEmp = new Emp();
                        tempEmp.No = BP.DA.DBAccess.GenerOID("GPM_Emp_Random_No").ToString();
                        if (tempEmp.RetrieveFromDBSources() == 0)
                        {
                            emp.No = tempEmp.No;
                            break;
                        }
                    }
                }
                emp.Insert();

                DeptEmp deptEmp = new DeptEmp();
                deptEmp.FK_Emp = emp.No;
                deptEmp.FK_Dept = emp.FK_Dept;
                deptEmp.FK_Duty = emp.FK_Duty;
                deptEmp.DutyLevel = string.IsNullOrEmpty(infoStrArrary[7]) == true ? 0 : int.Parse(infoStrArrary[7]);
                deptEmp.Leader = emp.Leader;
                deptEmp.DirectSave();

                string[] empStationStrArray = empStationStr.Split(',');
                foreach (string item in empStationStrArray)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;

                    DeptEmpStation deptEmpStation = new DeptEmpStation();
                    deptEmpStation.FK_Dept = emp.FK_Dept;
                    deptEmpStation.FK_Station = item;
                    deptEmpStation.FK_Emp = emp.No;
                    deptEmpStation.DirectSave();
                }
                return "true";
            }
            catch (Exception)
            {
                return "false";
            }
        }
        /// <summary>
        /// 删除系统管理员
        /// </summary>
        /// <returns></returns>
        private string DeleteEmpApp()
        {
            try
            {
                string MyPKList = getUTF8ToString("MyPKList");
                string[] myPKArrary = MyPKList.Split(',');
                foreach (string item in myPKArrary)
                {
                    EmpApp empApp = new EmpApp();
                    empApp.Delete(EntityMyPKAttr.MyPK, item);
                }
                return "true";
            }
            catch (Exception)
            {
                return "false";
            }
        }
        /// <summary>
        /// 删除部门管理员
        /// </summary>
        /// <returns></returns>
        private string deleteempdept()
        {
            try
            {
                string MyPKList = getUTF8ToString("MyPKList");
                string[] myPKArrary = MyPKList.Split(',');
                foreach (string item in myPKArrary)
                {
                    DeptManager ed = new DeptManager();
                    ed.Delete(EntityMyPKAttr.MyPK, item);
                }
                return "true";
            }
            catch (Exception)
            {
                return "false";
            }
        }
        /// <summary>
        /// 删除部门选中人员 qinqin
        /// </summary>
        /// <returns></returns>
        private string DeleteDeptEmp()
        {
            try
            {
                string deptNo = getUTF8ToString("deptNo");
                if (string.IsNullOrEmpty(deptNo))
                    return "false";

                string emps = getUTF8ToString("emps");
                if (string.IsNullOrEmpty(emps))
                    return "false";

                string[] noArrary = emps.Split(',');
                foreach (string item in noArrary)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;

                    Emp emp = new Emp(item);
                    //如果启用钉钉通讯录同步，编辑人员所属部门
                    if (BP.GPM.Glo.IsEnable_DingDing == true)
                    {
                        //编辑之前人员所属部门集合
                        DeptEmps deptEmps = new DeptEmps();
                        deptEmps.RetrieveByAttr(DeptEmpAttr.FK_Emp, item);
                        //删除当前后剩余部门集合
                        List<string> list_DeptIds = new List<string>();
                        foreach (DeptEmp deptEmp in deptEmps)
                        {
                            //排除当前部门
                            if (deptEmp.FK_Dept == deptNo)
                                continue;
                            list_DeptIds.Add(deptEmp.FK_Dept);
                        }
                        //修改人员的归属部门
                        if (list_DeptIds.Count > 0)
                        {
                            DingDing ding = new DingDing();
                            Ding_Post_ReturnVal postVal = ding.GPM_Ding_EditEmp(emp, list_DeptIds);
                            if (postVal == null || postVal.errcode != "0")
                                return "false";
                        }
                    }

                    DeptEmp de = new DeptEmp();
                    de.Delete(DeptEmpAttr.FK_Emp, item, DeptEmpAttr.FK_Dept, deptNo);

                    DeptEmpStation des = new DeptEmpStation();
                    des.Delete(DeptEmpStationAttr.FK_Emp, item, DeptEmpStationAttr.FK_Dept, deptNo);

                    de = new DeptEmp();
                    bool isExit = de.RetrieveByAttr(DeptEmpAttr.FK_Emp, item);

                    //如果port_DeptEmp存在FK_Emp为ed[0]的数据，则判断是否删除的port_emp中的FK_Dept
                    if (isExit)
                    {
                        //如果是删除的主部门，则修改主部门
                        if (emp.FK_Dept == deptNo)
                        {
                            emp.FK_Dept = de.FK_Dept;
                            emp.Update();
                        }
                    }
                    else
                    {
                        //如果port_DeptEmp不存在FK_Emp为ed[0]的数据，则从port_emp表里直接删除
                        //如果启用钉钉通讯录同步,删除人员
                        if (BP.GPM.Glo.IsEnable_DingDing == true)
                        {
                            DingDing ding = new DingDing();
                            Ding_Post_ReturnVal postVal = ding.GPM_Ding_DeleteEmp(item);
                            //在钉钉找不到该用户
                            if (!(postVal.errcode == "0" || postVal.errcode == "60121"))
                            {
                                DeptEmp deptEmp = new DeptEmp();
                                deptEmp.FK_Emp = emp.No;
                                deptEmp.FK_Duty = emp.FK_Duty;
                                deptEmp.FK_Dept = deptNo;
                                deptEmp.Insert();
                                return "false";
                            }
                        }
                        //从CCGPM中删除
                        emp.Delete();
                    }
                }
                return "true";
            }
            catch (Exception)
            {
                return "false";
            }
        }

        /// <summary>
        /// 禁用人员
        /// </summary>
        /// <returns></returns>
        private string DisableDeptEmp()
        {
            try
            {
                string emps = getUTF8ToString("emps");
                string[] noArrary = emps.Split(',');
                foreach (string item in noArrary)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;
                    if (item.ToLower() == "admin")
                        continue;
                    //如果port_DeptEmp不存在FK_Emp为ed[0]的数据，则从port_emp表里直接删除
                    //如果启用钉钉通讯录同步,删除人员
                    if (BP.GPM.Glo.IsEnable_DingDing == true)
                    {
                        DingDing ding = new DingDing();
                        Ding_Post_ReturnVal postVal = ding.GPM_Ding_DeleteEmp(item);
                        //在钉钉找不到该用户
                        if (!(postVal.errcode == "0" || postVal.errcode == "60121"))
                            return postVal.errcode + "-" + postVal.errmsg;
                    }
                    DeptEmp de = new DeptEmp();
                    de.Delete(DeptEmpAttr.FK_Emp, item);

                    DeptEmpStation des = new DeptEmpStation();
                    des.Delete(DeptEmpStationAttr.FK_Emp, item);
                    //禁用用户
                    Emp emp = new Emp(item);
                    emp.Pass = "FE4A-D402-451C-B9ED-C1A0";
                    emp.FK_Dept = "";
                    emp.Update();
                }
                return "true";
            }
            catch (Exception)
            {
                return "false";
            }
        }

        /// <summary>
        /// 获取禁用人员列表
        /// </summary>
        /// <returns></returns>
        private string GenerDisableEmps()
        {
            string pageNumber = getUTF8ToString("pageNumber");
            int iPageNumber = string.IsNullOrEmpty(pageNumber) ? 1 : Convert.ToInt32(pageNumber);
            //每页多少行
            string pageSize = getUTF8ToString("pageSize");
            int iPageSize = string.IsNullOrEmpty(pageSize) ? 9999 : Convert.ToInt32(pageSize);

            Emps emps = new Emps();
            QueryObject obj = new QueryObject(emps);
            obj.AddWhere("FK_Dept is null or FK_Dept =''");
            obj.addOrderBy(EmpAttr.Name);
            int total = obj.GetCount();
            obj.DoQuery(EmpAttr.No, iPageSize, iPageNumber);
            return BP.Tools.Entitis2Json.ConvertEntitis2GridJsonOnlyData(emps, total);
        }

        /// <summary>
        /// 调整人员所属主部门
        /// </summary>
        /// <returns></returns>
        private string ReplaceEmpbelongDept()
        {
            string fk_Dept = getUTF8ToString("FK_Dept");
            string fk_Emp = getUTF8ToString("FK_Emp");
            Emp emp = new Emp(fk_Emp);
            emp.FK_Dept = fk_Dept;
            if (emp.Update() > 0)
                return "true";
            return "false";
        }
        private string GetManagerDept()
        {
            string rootNo = getUTF8ToString("rootNo");

            Depts dt = new Depts();
            int row = dt.RetrieveAll();


            return BP.Tools.Entitis2Json.ConvertEntitis2GenerTree(dt, rootNo);
        }
        #region 下列方法为 GetOrganizationDept方法提供服务  -qin 15/7/6
        public string TansEntitiesToGenerTree(Depts ens, DeptManagers dms, string rootNo)
        {
            appendMenus = new StringBuilder();
            appendMenuSb = new StringBuilder();
            EntityTree root = ens.GetEntityByKey(EntityTreeAttr.ParentNo, rootNo) as EntityTree;
            if (root == null)
                throw new Exception("@没有找到rootNo=" + rootNo + "的entity.");

            appendMenus.Append("[{");
            appendMenus.Append("'id':'" + root.No + "'");
            appendMenus.Append(",'text':'" + root.Name + "'");
            appendMenus.Append(IsPermissionsNodes(ens, dms, root.No));

            // 增加它的子级.
            appendMenus.Append(",'children':");
            AddChildren(root, ens, dms);
            appendMenus.Append(appendMenuSb);
            appendMenus.Append("}]");

            return ReplaceIllgalChart(appendMenus.ToString());
        }
        public void AddChildren(EntityTree parentEn, Depts ens, DeptManagers dms)
        {
            appendMenus.Append(appendMenuSb);
            appendMenuSb.Clear();

            appendMenuSb.Append("[");
            foreach (EntityTree item in ens)
            {
                if (item.ParentNo != parentEn.No)
                    continue;

                appendMenuSb.Append("{'id':'" + item.No + "','text':'" + item.Name + "','state':'closed'");
                appendMenuSb.Append(IsPermissionsNodes(ens, dms, item.No));
                EntityTree treeNode = item as EntityTree;
                // 增加它的子级.
                appendMenuSb.Append(",'children':");
                AddChildren(item, ens, dms);
                appendMenuSb.Append("},");
            }
            if (appendMenuSb.Length > 1)
                appendMenuSb = appendMenuSb.Remove(appendMenuSb.Length - 1, 1);
            appendMenuSb.Append("]");
            appendMenus.Append(appendMenuSb);
            appendMenuSb.Clear();
        }
        public string ReplaceIllgalChart(string s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0, j = s.Length; i < j; i++)
            {

                char c = s[i];
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '/':
                        sb.Append("\\/");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }
        public string IsPermissionsNodes(Depts depts, DeptManagers dms, string no)
        {
            //string sql = "SELECT * FROM GPM_DeptManager WHERE FK_Emp='" + WebUser.No + "' AND FK_Dept='" + no + "'";
            ////表明当前登陆人没有权限(admin 有全部权限)   改变图标  红色标识的图标
            //if (DBAccess.RunSQLReturnCOUNT(sql) == 0 && WebUser.No != "admin")
            //    return ",'iconCls':'icon-tree_folder_no','attributes':{'authority':'no'}";

            //修改，将有权限的部门的父部门图标，也置为可操作图标，但是attributes中authority置为no，edited by liuxc
            string iconClsYes = "icon-accept";
            string iconClsNo = "icon-tree_folder_no";
            string authYes = "yes";
            string authNo = "no";
            bool haveAuth = false;
            bool haveYesCls = false;

            if (WebUser.No == "admin")
            {
                haveAuth = true;
                haveYesCls = true;
            }
            else
            {
                DeptManager dm = dms.GetEntityByKey(DeptManagerAttr.FK_Emp, WebUser.No,
                                                    DeptManagerAttr.FK_Dept, no) as DeptManager;

                if (dm == null)
                {
                    haveAuth = false;

                    //判断是否有下级存在可操作的，如果存在，则图标置为可操作，但权限依然是不可操作
                    haveYesCls = HaveChildAuthority(no, depts, dms);
                }
                else
                {
                    haveAuth = true;
                    haveYesCls = true;
                }
            }

            return string.Format(",'iconCls':'{0}','attributes':{{'authority':'{1}'}}",
                                 haveYesCls ? iconClsYes : iconClsNo, haveAuth ? authYes : authNo);
        }
        /// <summary>
        /// 根据设置的部门管理员，筛选可以操作的岗位
        /// </summary>
        /// <param name="no">树节点ID</param>
        /// <returns></returns>
        public string IsPermissionsNodesByStation(string no)
        {
            //修改，将有权限的部门的父部门图标，也置为可操作图标，但是attributes中authority置为no，edited by liuxc
            string iconClsYes = "icon-accept";
            string iconClsNo = "icon-tree_folder_no";
            string authYes = "yes";
            string authNo = "no";
            bool haveAuth = false;
            bool haveYesCls = false;

            if (WebUser.No == "admin")
            {
                haveAuth = true;
                haveYesCls = true;
            }
            else
            {
                string stations = "";
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable("SELECT * FROM Port_DeptStation");
                DeptManagers dm = new DeptManagers();
                dm.Retrieve(DeptManagerAttr.FK_Emp, WebUser.No);
                foreach (DeptManager dept in dm)
                {
                    DataRow[] drArr = dt.Select("FK_Dept='" + dept.FK_Dept + "'");
                    for (int i = 0; i < drArr.Length; i++)
                    {
                        if (stations.Contains(drArr[i]["FK_Station"].ToString()))
                            continue;
                        stations = stations + drArr[i]["FK_Station"].ToString() + ",";
                    }
                }
                if (stations.Contains(no))
                {
                    haveAuth = true;
                    haveYesCls = true;
                }
                else
                {
                    haveAuth = false;
                    haveYesCls = false;
                }
            }

            return string.Format(",'iconCls':'{0}','attributes':{{'authority':'{1}'}}",
                                 haveYesCls ? iconClsYes : iconClsNo, haveAuth ? authYes : authNo);
        }
        /// <summary>
        /// 判断当前用户是否含有指定部门的人员操作权限
        /// </summary>
        /// <param name="deptNo">部门编号</param>
        /// <param name="depts">部门集合</param>
        /// <param name="dms">权限集合</param>
        /// <returns></returns>
        private bool HaveChildAuthority(string deptNo, Depts depts, DeptManagers dms)
        {
            List<string> subDeptNos = new List<string>();

            foreach (Dept dept in depts)
            {
                if (dept.ParentNo == deptNo)
                    subDeptNos.Add(dept.No);
            }

            foreach (string subDeptNo in subDeptNos)
            {
                if (dms.GetEntityByKey(DeptManagerAttr.FK_Dept, subDeptNo, DeptManagerAttr.FK_Emp, WebUser.No) != null)
                    return true;

                if (HaveChildAuthority(subDeptNo, depts, dms))
                    return true;
            }

            return false;
        }

        #endregion
        private string GetOrganizationDept()
        {
            string rootNo = getUTF8ToString("rootNo");

            Depts dts = new Depts();
            QueryObject obj = new QueryObject(dts);
            obj.addOrderBy(DeptAttr.Idx);
            obj.DoQuery();

            DeptManagers dms = new DeptManagers();
            dms.RetrieveAll();

            return TansEntitiesToGenerTree(dts, dms, rootNo);
        }
        /// <summary>
        /// 新增部门--------------qin   ok   
        /// 考虑赋予当前人权限
        /// </summary>
        /// <returns></returns>
        private string appendDataMet()
        {
            string deptNo = getUTF8ToString("deptNo");
            string deptSort = getUTF8ToString("deptSort");

            if (string.IsNullOrEmpty(deptNo))
            {
                return "false";
            }

            try
            {
                //string sql = "select MAX(CAST(No as int)) maxNo from port_dept";
                //int maxNo = int.Parse(DBAccess.RunSQLReturnTable(sql).Rows[0][0].ToString());
                Dept dept = new Dept();
                dept.RetrieveByAttr(DeptAttr.No, deptNo);
                Dept newDept = null;
                if (deptSort == "peer")//同级部门
                {
                    if (dept.ParentNo == "0")//有且只有一个根节点
                        return "false";

                    newDept = dept.DoCreateSameLevelNode() as Dept;
                }
                if (deptSort == "son")
                    newDept = dept.DoCreateSubNode() as Dept;

                //添加部门
                dept = new Dept();
                dept.No = newDept.No;
                dept.Name = "新增部门" + newDept.No;
                dept.ParentNo = newDept.ParentNo;
                //如果启用钉钉通讯录同步
                if (BP.GPM.Glo.IsEnable_DingDing == true)
                {
                    //钉钉同一级部门不允许同名，需特殊处理。
                    string temp = string.Format("{0:MMddHHmmssffff}", DateTime.Now);
                    dept.Name = "新增部门" + temp;

                    DingDing ding = new DingDing();
                    CreateDepartMent_PostVal postVal = ding.GPM_Ding_CreateDept(dept);
                    if (postVal != null && postVal.errcode == "0")
                        dept.No = postVal.id;
                    else
                        return "false";
                }
                dept.DirectSave();

                //给新增部门赋予当前人权限
                DeptManager ed = new DeptManager();
                ed.MyPK = WebUser.No + "_" + dept.No;
                ed.FK_Dept = dept.No;
                ed.FK_Emp = WebUser.No;
                ed.DirectSave();

                return dept.No;
            }
            catch (Exception)
            {
                return "false";
            }
        }
        /// <summary>
        /// 删除选中部门   秦     OK
        /// 要求：仅当只有一种情况可以执行操作-没有任何人隶属这个部门。 
        ///       用户手动删除完该部门下的emp后才可以执行一下操作    -周要求
        /// 必要的再次判断，是否还有子节点，是否还存在有emp
        /// 考虑表之间的关联
        /// </summary>
        /// <returns></returns>
        private string deleteNodeMet()
        {
            string deptNo = getUTF8ToString("deptNo");

            if (string.IsNullOrEmpty(deptNo))
            {
                return "false";
            }

            try
            {
                Depts ds = new Depts();
                int count = ds.RetrieveByAttr(DeptAttr.ParentNo, deptNo);
                if (count != 0)//表明含有子级部门   禁止删除操作  
                    return "false";

                Emps es = new Emps();
                count = es.RetrieveByAttr(EmpAttr.FK_Dept, deptNo);
                if (count != 0)//表明有emps         禁止删除操作
                    return "err:此部门下还有人员不允许删除。";

                //如果启用钉钉通讯录同步
                if (BP.GPM.Glo.IsEnable_DingDing == true)
                {
                    DingDing ding = new DingDing();
                    Ding_Post_ReturnVal postVal = ding.GPM_Ding_DeleteDept(deptNo);
                    //60003钉钉部门不存在
                    if (postVal.errcode != "0" && postVal.errcode != "60003")
                        return "err:" + postVal.errcode + "-钉钉-" + postVal.errmsg;
                }
                Dept dept = new Dept(deptNo);
                dept.Delete();//只是一个空的部门，删除后要执行一下代码

                #region 级联删除相关记录

                DeptDutys dds = new DeptDutys();//部门职务
                dds.Delete(DeptDutyAttr.FK_Dept, deptNo);

                DeptStations dss = new DeptStations();//部门岗位对应
                dss.Delete(DeptStationAttr.FK_Dept, deptNo);

                DeptEmps des = new DeptEmps();//部门人员信息
                des.Delete(DeptEmpAttr.FK_Dept, deptNo);

                DeptEmpStations deSta = new DeptEmpStations();//部门岗位人员对应
                deSta.Delete(DeptEmpStationAttr.FK_Dept, deptNo);

                DeptManagers dms = new DeptManagers();//二级菜单的权限表
                dms.Delete(DeptManagerAttr.FK_Dept, deptNo);

                DeptSearchScorps dsss = new DeptSearchScorps();//部门查询权限--这是什么表?
                dsss.Delete(DeptSearchScorpAttr.FK_Dept, deptNo);

                #endregion

                return dept.ParentNo;
            }
            catch (Exception)
            {
                return "false";
            }
        }
        /// <summary>
        /// 上/下移部门
        /// </summary>
        /// <returns></returns>
        private string floatNodeMet()
        {
            string selectedNodeId = getUTF8ToString("selectedNodeId");
            string floatWay = getUTF8ToString("floatWay");
            try
            {
                Dept dept = new Dept(selectedNodeId);

                if (floatWay == "up")//上移  
                    dept.DoUp();

                if (floatWay == "down")//下移
                    dept.DoDown();

                //如果启用钉钉通讯录同步，编辑部门顺序
                if (BP.GPM.Glo.IsEnable_DingDing == true)
                {
                    DingDing ding = new DingDing();
                    Ding_Post_ReturnVal postVal = ding.GPM_Ding_EditDept(dept);
                    if (postVal.errcode != "0")
                        return "false";
                }
                return "true";
            }
            catch (Exception)
            {
                return "false";
            }
        }
        /// <summary>
        /// 上/下移人员
        /// </summary>
        /// <returns></returns>
        private string EmpFloatNodeMet()
        {
            string selectedEmpNo = getUTF8ToString("selectedEmpNo");
            string floatWay = getUTF8ToString("floatWay");
            try
            {
                Emp emp = new Emp(selectedEmpNo);

                if (floatWay == "up")//上移  
                    emp.DoUp();

                if (floatWay == "down")//下移
                    emp.DoDown();

                //如果启用钉钉通讯录同步，编辑部门顺序
                if (BP.GPM.Glo.IsEnable_DingDing == true)
                {
                    DingDing ding = new DingDing();
                    Ding_Post_ReturnVal postVal = ding.GPM_Ding_EditEmp(emp);
                    if (postVal.errcode != "0")
                        return "false";
                }
                return "true";
            }
            catch (Exception)
            {
                return "false";
            }
        }
        /// <summary>
        /// 部门信息
        /// </summary>
        /// <returns></returns>
        private string checkDeptInfoMet()
        {
            string selectedNodeId = getUTF8ToString("selectedNodeId");
            //如果启用钉钉通讯录同步，则根目录不允许修改
            if (BP.GPM.Glo.IsEnable_DingDing == true)
            {
                if (selectedNodeId == "1")
                    return "err:钉钉不允许修改根目录。";
            }
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("{");//开始拼接json

                //拼接部门基本信息
                Dept d = new Dept();
                d.RetrieveByAttr(DeptAttr.No, selectedNodeId);
                sb.Append("deptNo:[\"" + d.No + "\"],deptName:[\"" + d.Name + "\"],deptLeader:[\"" + d.Leader + "\"]");

                //拼接部门岗位
                DeptStations dss = new DeptStations();
                dss.RetrieveByAttr(DeptStationAttr.FK_Dept, selectedNodeId);

                Stations ss;//岗位集合
                StationTypes sts = new StationTypes();//岗位类型
                sts.RetrieveAll();

                sb.Append(",deptStation:[");
                sb.Append("{\"id\":\"CheId\",\"iconCls\":\"icon-save\",\"text\":\"岗位\",\"attributes\":{\"isSonNode\":\"no\"},\"children\":[");//组装根节点
                int count = 0;
                foreach (StationType item in sts)
                {
                    sb.Append("{\"id\":\"" + item.No
                        + "\",iconCls:\"icon-user\"" + ",\"text\":\"" + item.Name + "\",\"attributes\":{\"isSonNode\":\"no\"},\"children\":[");
                    ss = new Stations();
                    ss.RetrieveByAttr(StationAttr.FK_StationType, item.No);
                    int i = 0;
                    foreach (Station sta in ss)//在此处返回已选项目
                    {
                        bool addAgain = true;
                        foreach (DeptStation ds in dss)
                        {
                            if (ds.FK_Station == sta.No)
                            {
                                sb.Append("{\"id\":\"" + sta.No +
                                    "\",iconCls:\"icon-user\"" + ",\"text\":\"" + sta.Name + "\",\"checked\":true,\"attributes\":{\"isSonNode\":\"yes\"}");
                                addAgain = false;
                                break;
                            }
                        }
                        if (addAgain)
                            sb.Append("{\"id\":\"" + sta.No +
                                "\",iconCls:\"icon-user\"" + ",\"text\":\"" + sta.Name + "\",\"checked\":false,\"attributes\":{\"isSonNode\":\"yes\"}");
                        if (i == ss.Count - 1)
                        {
                            sb.Append("}");
                            break;
                        }
                        sb.Append("},"); i += 1;
                    }
                    if (count == sts.Count - 1)
                    {
                        sb.Append("]}");
                        break;
                    }
                    sb.Append("]},"); count += 1;
                }
                sb.Append("]}]");

                //拼接部门职务
                DeptDutys dds = new DeptDutys();
                dds.RetrieveByAttr(DeptDutyAttr.FK_Dept, selectedNodeId);//检索与selectedNodeId相关的信息

                Dutys dutys = new Dutys();
                dutys.RetrieveAll();

                sb.Append(",deptDuty:[");
                sb.Append("{\"id\":\"CheId\",\"iconCls\":\"icon-save\",\"text\":\"职务\",\"attributes\":{\"isSonNode\":\"no\"},\"children\":[");//自定义根节点
                int t = 0;
                foreach (Duty item in dutys)
                {
                    bool addAgain = true;
                    foreach (DeptDuty dd in dds)
                    {
                        if (dd.FK_Duty == item.No)//选择已有项
                        {
                            sb.Append("{\"id\":\"" + item.No +
                                "\",iconCls:\"icon-user\"" + ",\"text\":\"" + item.Name + "\",\"checked\":true,\"attributes\":{\"isSonNode\":\"yes\"}");
                            addAgain = false;
                            break;
                        }
                    }
                    if (addAgain)
                        sb.Append("{\"id\":\"" + item.No +
                            "\",iconCls:\"icon-user\"" + ",\"text\":\"" + item.Name + "\",\"checked\":false,\"attributes\":{\"isSonNode\":\"yes\"}");
                    if (t == dutys.Count - 1)
                    {
                        sb.Append("}");
                        break;
                    }
                    sb.Append("},"); t += 1;
                }

                sb.Append("]}]");
                //部门树
                string treeJson = GetDeptTreeJson(selectedNodeId);
                if (treeJson != "[]")
                {
                    sb.Append(",depttree:");
                    sb.Append(treeJson);
                }
                sb.Append("}");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return "err:" + ex.Message;
            }
        }
        /// <summary>
        /// 保存部门信息    秦  15.8.14   OK
        /// 考虑部门编号为空
        /// 考虑表的级联性
        /// </summary>
        /// <returns></returns>
        private string saveDeptInfoMet()
        {
            string deptNo = getUTF8ToString("deptNo");//部门编号
            string deptParentNo = getUTF8ToString("deptParentNo");//部门父节点编号
            string deptName = getUTF8ToString("deptName");//名称
            string deptLeader = getUTF8ToString("deptLeader");//领导
            string deptStationStr = getUTF8ToString("stationStr");//岗位
            string deptDutyStr = getUTF8ToString("dutyStr");//职务

            if (string.IsNullOrEmpty(deptNo))
                return "err:部门编号不存在。";

            try
            {
                Dept dept = new Dept();
                dept.RetrieveByAttr(DeptAttr.No, deptNo);

                #region 获取原先配置的岗位信息,计算出排除的岗位，然后看排除的岗位下是否有本部的人,有的话不允许排除此岗位
                string beforeStationStr = "";
                DeptStations deptStations = new DeptStations();
                deptStations.RetrieveByAttr(DeptStationAttr.FK_Dept, deptNo);
                foreach (DeptStation ds in deptStations)
                {
                    if (beforeStationStr.Contains(ds.FK_Station + ","))
                        continue;
                    beforeStationStr += ds.FK_Station + ",";
                }
                //传入岗位集合
                string[] strDeptStations_Arrary = deptStationStr.Split(',');
                //修改前包含岗位
                string[] beforedeptStations_Arrary = beforeStationStr.Split(',');
                //不能排除的岗位
                List<string> unDelDeptStation_List = new List<string>();
                if (beforedeptStations_Arrary.Length > 0)
                {
                    foreach (string beforeDeptSta in beforedeptStations_Arrary)
                    {
                        //排除为空
                        if (string.IsNullOrEmpty(beforeDeptSta) || string.IsNullOrWhiteSpace(beforeDeptSta))
                            continue;

                        bool isExit = false;
                        foreach (string item in strDeptStations_Arrary)
                        {
                            if (beforeDeptSta == item)
                            {
                                isExit = true;
                                break;
                            }
                        }
                        //说明已移除，判断该部门和岗位下是否有人
                        if (isExit == false)
                        {
                            DeptEmpStations deptEmpStations = new DeptEmpStations();
                            deptEmpStations.Retrieve(DeptEmpStationAttr.FK_Dept, deptNo, DeptEmpStationAttr.FK_Station, beforeDeptSta);
                            //部门岗位下包括人，不允许排除
                            if (deptEmpStations != null && deptEmpStations.Count > 0)
                                unDelDeptStation_List.Add(beforeDeptSta);
                        }
                    }
                }
                #endregion

                #region 获取原先配置的职务信息
                string beforeDutyStr = "";
                DeptDutys deptDutys = new DeptDutys();
                deptDutys.RetrieveByAttr(DeptDutyAttr.FK_Dept, deptNo);
                foreach (DeptDuty dd in deptDutys)
                {
                    if (beforeDutyStr.Contains(dd.FK_Duty + ","))
                        continue;
                    beforeDutyStr += dd.FK_Duty + ",";
                }

                //传入值，部门所选职务
                string[] strDeptDutys_Arrary = deptDutyStr.Split(',');
                //原有职务
                string[] beforeDeptDutys_Arrary = beforeDutyStr.Split(',');
                //不允许删除的职务
                List<string> unDelDeptDutys_List = new List<string>();

                if (beforeDutyStr.Length > 0)
                {
                    foreach (string beforeDeptDuty in beforeDeptDutys_Arrary)
                    {
                        //排除为空
                        if (string.IsNullOrEmpty(beforeDeptDuty) || string.IsNullOrWhiteSpace(beforeDeptDuty))
                            continue;

                        bool isExit = false;
                        foreach (string item in strDeptDutys_Arrary)
                        {
                            if (beforeDeptDuty == item)
                            {
                                isExit = true;//如果已经存在
                                break;
                            }
                        }
                        //表名新修改的职务不包含以前所选，可能会造成人员的误删
                        if (isExit == false)
                        {
                            DeptEmps deptEmps = new DeptEmps();
                            deptEmps.Retrieve(DeptEmpAttr.FK_Dept, deptNo, DeptEmpAttr.FK_Duty, beforeDeptDuty);
                            //部门职务下包括人，不允许排除
                            if (deptEmps != null && deptEmps.Count > 0)
                                unDelDeptDutys_List.Add(beforeDeptDuty);
                        }
                    }
                }
                #endregion

                #region 更新部门信息
                dept.Name = deptName;
                dept.Leader = deptLeader;
                //变更部门父节点
                if (deptParentNo != "0")
                {
                    dept.ParentNo = deptParentNo;
                }
                //如果启用钉钉通讯录同步，编辑部门
                if (BP.GPM.Glo.IsEnable_DingDing == true)
                {
                    DingDing ding = new DingDing();
                    Ding_Post_ReturnVal postVal = ding.GPM_Ding_EditDept(dept);
                    if (postVal.errcode != "0")
                        return "false";
                }
                if (dept.Update() > 0)
                    dept.GenerNameOfPath();
                #endregion

                #region    更新岗位对应
                //删除所有部门编号为deptNo记录
                DBAccess.RunSQL("DELETE FROM Port_DeptStation WHERE FK_Dept='" + deptNo + "'");
                foreach (string item in strDeptStations_Arrary)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;

                    DeptStation ds = new DeptStation();
                    ds.FK_Station = item;
                    ds.FK_Dept = deptNo;
                    ds.DirectSave();
                }
                //把不允许排除的追加
                foreach (string item in unDelDeptStation_List)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;

                    DeptStation ds = new DeptStation();
                    ds.FK_Station = item;
                    ds.FK_Dept = deptNo;
                    ds.DirectSave();
                }
                #endregion

                #region    更新职务对应
                //删除FK_Dept为deptNo的所有的记录
                DBAccess.RunSQL("DELETE FROM Port_DeptDuty WHERE FK_Dept='" + deptNo + "'");
                foreach (string item in strDeptDutys_Arrary)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;
                    DeptDuty dd = new DeptDuty();

                    dd.FK_Dept = deptNo;
                    dd.FK_Duty = item;

                    dd.DirectSave();
                }

                //把不允许排除的追加
                foreach (string item in unDelDeptDutys_List)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;
                    DeptDuty dd = new DeptDuty();
                    dd.FK_Dept = deptNo;
                    dd.FK_Duty = item;
                    dd.DirectSave();
                }
                #endregion

                return "true";
            }
            catch (Exception ex)
            {
                return "err:" + ex.Message;
            }
        }
        /// <summary>
        ///  模糊查询
        /// </summary>
        /// <returns></returns>
        private string doSearchMet()
        {
            string selectedNodeId = getUTF8ToString("selectedNodeId");
            string searchVal = getUTF8ToString("searchVal");//查询关键字

            try
            {
                string sql = "SELECT  DISTINCT FK_STATIONTYPE FROM PORT_STATION WHERE NAME LIKE '%" + searchVal + "%'";
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                string[] stationTypeArray = new string[dt.Rows.Count];

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    stationTypeArray[i] = dt.Rows[i]["FK_STATIONTYPE"].ToString();
                }

                DeptStations dss = new DeptStations();
                dss.RetrieveByAttr(DeptStationAttr.FK_Dept, selectedNodeId);

                Stations ss;//岗位集合
                StationTypes sts = new StationTypes();//岗位类型
                sts.RetrieveAll();

                StringBuilder AppendJson = new StringBuilder();

                //组装根节点
                AppendJson.Append("[{\"id\":\"CheId\",\"iconCls\":\"icon-save\",\"text\":\"岗位\",\"attributes\":{\"isSonNode\":\"no\"},\"children\":[");

                int count = 0;
                foreach (StationType item in sts)
                {
                    bool canContinue = false;
                    foreach (string stype in stationTypeArray)
                    {
                        if (item.No == stype)
                        {
                            canContinue = true;
                            break;
                        }
                    }

                    if (!canContinue) continue;

                    AppendJson.Append("{\"id\":\"" + item.No
                        + "\",iconCls:\"icon-user\"" + ",\"text\":\"" + item.Name + "\",\"attributes\":{\"isSonNode\":\"no\"},\"children\":[");
                    ss = new Stations();
                    ss.RetrieveByAttr(StationAttr.FK_StationType, item.No);
                    int i = 0;
                    foreach (Station sta in ss)//在此处返回已选项目
                    {
                        sql = "SELECT  NAME FROM PORT_STATION WHERE NAME LIKE '%" + searchVal + "%'";
                        dt = DBAccess.RunSQLReturnTable(sql);
                        string[] names = new string[dt.Rows.Count];
                        for (int t = 0; t < dt.Rows.Count; t++)
                        {
                            names[t] = dt.Rows[t]["NAME"].ToString();
                        }
                        bool canAddName = false;
                        foreach (string name in names)
                        {
                            if (sta.Name == name)
                            {
                                canAddName = true;
                                break;
                            }
                        }
                        if (!canAddName) continue;

                        bool addAgain = true;
                        foreach (DeptStation ds in dss)
                        {
                            if (ds.FK_Station == sta.No)
                            {
                                AppendJson.Append("{\"id\":\"" + sta.No +
                                    "\",iconCls:\"icon-user\"" + ",\"text\":\"" + sta.Name + "\",\"checked\":true,\"attributes\":{\"isSonNode\":\"yes\"}");
                                addAgain = false;
                                break;
                            }
                        }
                        if (addAgain)
                            AppendJson.Append("{\"id\":\"" + sta.No +
                                "\",iconCls:\"icon-user\"" + ",\"text\":\"" + sta.Name + "\",\"checked\":false,\"attributes\":{\"isSonNode\":\"yes\"}");
                        if (i == ss.Count - 1)
                        {
                            AppendJson.Append("}");
                            break;
                        }
                        AppendJson.Append("},"); i += 1;
                    }
                    if (count == sts.Count - 1)
                    {
                        AppendJson.Append("]}");
                        break;
                    }
                    AppendJson.Append("]},"); count += 1;
                }
                AppendJson.Append("]}]");

                return AppendJson.ToString();
            }
            catch (Exception)
            {
                return "false";
            }
        }
        /// <summary>
        /// 人员信息 qinqinqin
        /// </summary>
        /// <returns></returns>
        private string getEmpInfoMet()
        {
            string deptNo = getUTF8ToString("selectedNodeId");//部门编号
            if (string.IsNullOrEmpty(deptNo))
                return "";

            string empNo = getUTF8ToString("empNo");//人员编号
            if (string.IsNullOrEmpty(empNo))
                return "";

            Emp emp = new Emp(empNo);

            StringBuilder sb = new StringBuilder();
            sb.Append("{setName:[\"" + emp.Name + "\"]");
            sb.Append(",setNo:[\"" + emp.No + "\"]");
            sb.Append(",setZgbh:[\"" + emp.EmpNo + "\"]");
            sb.Append(",FK_Dept:[\"" + emp.FK_Dept + "\"]");

            DeptDutys dds = new DeptDutys();
            dds.RetrieveByAttr(DeptDutyAttr.FK_Dept, deptNo);

            string setZw = "";
            int i = 0;
            foreach (DeptDuty item in dds)
            {
                Duty d = new Duty();
                d.RetrieveByAttr(DutyAttr.No, item.FK_Duty);

                DeptEmp dEmp = new DeptEmp();
                dEmp.RetrieveByAttrAnd(DeptEmpAttr.FK_Emp, empNo, DeptEmpAttr.FK_Dept, deptNo);

                if (dEmp.FK_Duty == d.No)
                {
                    setZw += "{\"id\":\"" + d.No + "\",\"text\":\"" + d.Name + "\",\"selected\":\"selected\"}";
                }
                else
                {
                    setZw += "{\"id\":\"" + d.No + "\",\"text\":\"" + d.Name + "\"}";
                }
                if (i == dds.Count - 1)
                {
                    continue;
                }
                setZw += ",";
                i += 1;
            }

            sb.Append(",setZw:[" + setZw + "]");//下拉框
            sb.Append(",setTel:[\"" + emp.Tel + "\"]");
            sb.Append(",setEamil:[\"" + emp.Email + "\"]");
            sb.Append(",setLeader:[\"" + emp.Leader + "\"]");
            sb.Append(",SignType:[\"" + emp.SignType + "\"]");
            sb.Append(",PersonalID:[\"" + emp.PersonalID + "\"]");

            DeptEmp de = new DeptEmp();
            de.Retrieve(DeptEmpAttr.FK_Emp, empNo, DeptEmpAttr.FK_Dept, deptNo);
            sb.Append(",setZwlb:[\"" + de.DutyLevel.ToString() + "\"]");

            #region 拥有岗位
            sb.Append(",yygw:[");

            //拥有岗位
            DeptStations dss = new DeptStations();
            dss.RetrieveByAttr(DeptStationAttr.FK_Dept, deptNo);

            Stations ss;//岗位集合
            StationTypes sts = new StationTypes();//岗位类型
            sts.RetrieveAll();


            sb.Append("{\"id\":\"CheId\",\"iconCls\":\"icon-save\",\"text\":\"可选岗位\",\"attributes\":{\"isSonNode\":\"no\"},\"children\":[");//组装根节点
            int count = 0;
            foreach (StationType item in sts)
            {
                sb.Append("{\"id\":\"" + item.No
                    + "\",iconCls:\"icon-user\"" + ",\"text\":\"" + item.Name + "\",\"attributes\":{\"isSonNode\":\"no\"},\"children\":[");
                ss = new Stations();
                ss.RetrieveByAttr(StationAttr.FK_StationType, item.No);//检索所有对应类型的岗位
                i = 0;
                foreach (Station sta in ss)
                {
                    foreach (DeptStation ds in dss)
                    {
                        if (ds.FK_Station == sta.No)//只去与本部门有关的
                        {
                            DeptEmpStations ess = new DeptEmpStations();
                            ess.RetrieveByAttr(DeptEmpStationAttr.FK_Dept, deptNo);

                            string isCheck = "false";//isCheck定义为bool类型，返回前台时变成False,True小郁闷
                            foreach (DeptEmpStation singItem in ess)
                            {
                                if (singItem.FK_Station == sta.No && singItem.FK_Emp == empNo)
                                {
                                    isCheck = "true";
                                    break;
                                }
                            }

                            sb.Append("{\"id\":\"" + sta.No +
                                "\",iconCls:\"icon-user\"" + ",\"text\":\"" + sta.Name + "\",\"checked\":" + isCheck + ",\"attributes\":{\"isSonNode\":\"yes\"}");

                            if (i == dss.Count - 1)
                            {
                                sb.Append("}");
                                break;
                            }
                            sb.Append("},"); i += 1;
                            break;
                        }
                    }
                }
                sb.Replace(",", "", sb.ToString().Length - 1, 1);
                if (count == sts.Count - 1)
                {
                    sb.Append("]}");
                    break;
                }
                sb.Append("]},"); count += 1;
            }
            sb.Append("]}]");
            #endregion

            #region 归属部门
            DeptEmps deptEmps = new DeptEmps();
            deptEmps.RetrieveByAttr(DeptEmpAttr.FK_Emp, empNo);
            if (deptEmps.Count == 0)
            {
                //特殊情况处理，一般不会出现
                DeptEmp deptEmp = new DeptEmp();
                deptEmp.FK_Dept = deptNo;
                deptEmp.FK_Emp = empNo;
                deptEmp.Insert();
                deptEmps.AddEntity(deptEmp);
            }
            //部门集合
            Depts depts = new Depts();
            QueryObject obj = new QueryObject(depts);
            string strDepts = "";
            foreach (DeptEmp deptEmp in deptEmps)
            {
                strDepts += "'" + deptEmp.FK_Dept + "',";
            }
            //根据部门与用户关联表获取部门编号，查找所有部门
            strDepts = strDepts.Remove(strDepts.Length - 1);
            obj.AddWhere(DeptAttr.No, " in ", "(" + strDepts + ")");
            obj.addOrderBy(DeptAttr.NameOfPath);
            obj.DoQuery();
            string deptEmpsJson = BP.Tools.Entitis2Json.ConvertEntitis2GridJsonOnlyData(depts);
            sb.Append(",BlongDepts:");
            sb.Append(deptEmpsJson);
            #endregion

            sb.Append("}");

            return sb.ToString();
        }
        /// <summary>
        /// 保存修改后的信息-------------修
        /// </summary>
        /// <returns></returns>
        private string editDeptEmpMet()
        {
            string selectEmpNo = getUTF8ToString("selectEmpNo");
            string info = getUTF8ToString("info");
            string selectedNodeId = getUTF8ToString("selectedNodeId");
            string stationTreeNodesStr = getUTF8ToString("stationTreeNodesStr");

            int count = System.Text.RegularExpressions.Regex.Matches(info.ToString(), ",").Count;
            if (count != 8)//表明前台绕过js，或者破坏了脚本验证,或是人为的输入了','
                return "@err:传入字段数量不正确，应该为8，实际" + count;


            try
            {
                DBAccess.RunSQL("DELETE FROM Port_DeptEmpStation WHERE FK_Dept='" + selectedNodeId + "' AND FK_Emp='" + selectEmpNo + "'");

                DeptEmpStation des;
                string[] stationArray = stationTreeNodesStr.Split(',');
                foreach (string item in stationArray)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;

                    des = new DeptEmpStation();
                    des.FK_Dept = selectedNodeId;
                    des.FK_Station = item;
                    des.FK_Emp = selectEmpNo;

                    des.Insert();
                }


                string[] infoArray = info.Split(',');//所有信息都在数组内，开始更新操作

                Emp emp = new Emp();
                emp.RetrieveByAttr(EmpAttr.No, selectEmpNo);

                emp.Name = infoArray[0];
                emp.EmpNo = infoArray[1];
                emp.FK_Duty = infoArray[2];
                emp.Tel = infoArray[3];
                emp.Email = infoArray[4];
                emp.Leader = infoArray[5];
                emp.SignType = string.IsNullOrEmpty(infoArray[7]) ? 0 : int.Parse(infoArray[7]);
                //如果启用钉钉通讯录同步，编辑人员
                if (BP.GPM.Glo.IsEnable_DingDing == true)
                {
                    DingDing ding = new DingDing();
                    Ding_Post_ReturnVal postVal = ding.GPM_Ding_EditEmp(emp);
                    if (postVal.errcode != "0")
                        return "err:" + postVal.errcode + "-钉钉-" + postVal.errmsg;
                }
                //更新CCGPM中人员

                emp.PersonalID = infoArray[8];
                emp.Update();

                DBAccess.RunSQL("DELETE FROM Port_DeptEmp WHERE FK_Dept='" + selectedNodeId + "' AND FK_Emp='" + selectEmpNo + "'");
                DeptEmp deptEmp = new DeptEmp();

                deptEmp.FK_Emp = selectEmpNo;
                deptEmp.FK_Dept = selectedNodeId;
                deptEmp.Leader = infoArray[5];
                deptEmp.FK_Duty = infoArray[2];
                deptEmp.DutyLevel = int.Parse(infoArray[6]);

                deptEmp.Insert();

                return "true";
            }
            catch (Exception)
            {
                return "false";
            }
        }

        /// <summary>
        /// 检查是否重名
        /// </summary>
        /// <returns></returns>
        private string checkEmpNoMet()
        {
            string empNo = getUTF8ToString("empNo");
            try
            {
                if (string.IsNullOrEmpty(empNo))
                    return "false";
                Emps es = new Emps();

                int count = es.RetrieveByAttr(EmpAttr.No, empNo);

                if (count != 0)
                    return "false";

                return "true";
            }
            catch (Exception)
            {
                return "false";
            }
        }
        /// <summary>
        /// 添加人员职务下拉框 改
        /// </summary>
        /// <returns></returns>
        private string getDutyStationDllInfoMet()
        {
            string selectedNodeId = getUTF8ToString("selectedNodeId");//部门编号

            StringBuilder sb = new StringBuilder();
            DeptDutys dds = new DeptDutys();
            dds.RetrieveByAttr(DeptDutyAttr.FK_Dept, selectedNodeId);

            Dutys ds = new Dutys();
            ds.RetrieveAll();

            try
            {
                sb.Append("{zwDll:[");
                int i = 0;
                foreach (Duty item in ds)
                {
                    i = 0;
                    foreach (DeptDuty dd in dds)
                    {
                        if (dd.FK_Duty == item.No)
                        {
                            if (i == 0)
                            {
                                sb.Append("{\"id\":\"" + item.No +
                               "\",\"text\":\"" + item.Name + "\",\"selected\":\"selected\"}");
                            }
                            else
                            {
                                sb.Append("{\"id\":\"" + item.No +
                                      "\",\"text\":\"" + item.Name + "\"}");
                            }
                            if (i == dds.Count - 1)
                                continue;
                            sb.Append(",");
                            continue;
                        }
                        i += 1;
                    }
                }
                sb.Append("],gwDll:[");
                sb.Append("{\"id\":\"CheId\",\"iconCls\":\"icon-save\",\"text\":\"可选岗位\",\"attributes\":{\"isSonNode\":\"no\"},\"children\":[");//组装根节点

                DeptStations dss = new DeptStations();

                StationTypes sts = new StationTypes();//岗位类型
                sts.RetrieveAll();

                dss.RetrieveByAttr(DeptStationAttr.FK_Dept, selectedNodeId);
                int count = 0;
                foreach (StationType staType in sts)
                {
                    sb.Append("{\"id\":\"" + staType.No
                   + "\",iconCls:\"icon-user\"" + ",\"text\":\"" + staType.Name + "\",\"attributes\":{\"isSonNode\":\"no\"},\"children\":[");

                    foreach (DeptStation dsItem in dss)
                    {
                        Station s = new Station();
                        if (s.RetrieveByAttrAnd(StationAttr.FK_StationType, staType.No, StationAttr.No, dsItem.FK_Station))
                        {
                            sb.Append("{\"id\":\"" + s.No +
                                "\",\"text\":\"" + s.Name + "\",\"attributes\":{\"isSonNode\":\"yes\"}},");
                        }
                    }

                    sb.Replace(",", "", sb.ToString().Length - 1, 1);

                    if (count == sts.Count - 1)
                    {
                        sb.Append("]}");
                        break;
                    }
                    sb.Append("]},"); count += 1;
                }
                sb.Append("]}]}");

                return sb.ToString();
            }
            catch (Exception)
            {
                return "false";
            }
        }
        /// <summary>
        /// 读取关联人员信息
        /// </summary>
        /// <returns></returns>
        private string getOtherEmpsMet()
        {
            string deptNo = getUTF8ToString("deptNo");
            if (string.IsNullOrEmpty(deptNo))
                return "";

            string pageNumber = getUTF8ToString("pageNumber");
            int iPageNumber = string.IsNullOrEmpty(pageNumber) ? 1 : Convert.ToInt32(pageNumber);
            //每页多少行
            string pageSize = getUTF8ToString("pageSize");
            int iPageSize = string.IsNullOrEmpty(pageSize) ? 9999 : Convert.ToInt32(pageSize);

            string sql = " (select a.No,a.Name,b.name deptName from port_emp a,port_dept b where a.fk_dept =b.no and a. no not in("
                  + "select fk_emp from  Port_DeptEmp where fk_dept='" + deptNo + "')) dbSor ";

            return DBPaging(sql, iPageNumber, iPageSize, "No", "No");
        }

         /// <summary>
        /// 模糊查询关联人员信息
        /// </summary>
        /// <returns></returns>
        private string getEmpsByNameOrDept() 
        {

            string objSearch = getUTF8ToString("objSearch");

            string deptNo = getUTF8ToString("deptNo");
            
            string pageNumber = getUTF8ToString("pageNumber");
            int iPageNumber = string.IsNullOrEmpty(pageNumber) ? 1 : Convert.ToInt32(pageNumber);
            //每页多少行
            string pageSize = getUTF8ToString("pageSize");
            int iPageSize = string.IsNullOrEmpty(pageSize) ? 9999 : Convert.ToInt32(pageSize);

            string sql = "";

            if (string.IsNullOrEmpty(objSearch))
            {
                sql = " (select a.No,a.Name,b.name deptName from port_emp a,port_dept b where a.fk_dept =b.no and a. no not in("
                  + "select fk_emp from  Port_DeptEmp where fk_dept='" + deptNo + "')) dbSor ";
            }
            else {
                sql = " (select a.No,a.Name,b.name deptName from port_emp a,port_dept b where a.fk_dept =b.no and a. no not in("
                  + "select fk_emp from  Port_DeptEmp where fk_dept='" + deptNo + "')  and (a.Name like '%" + objSearch + "%'  or b.name like '%" + objSearch + "%' or a.no like '%" + objSearch + "%'  )) dbSor ";
            }

            return DBPaging(sql, iPageNumber, iPageSize, "No", "");
        
        }

        /// <summary>
        /// 保存关联人员数据
        /// 注意主表部门编号也要跟着改变
        /// </summary>
        /// <returns></returns>
        private string glEmpMet()
        {
            string deptNo = getUTF8ToString("deptNo");
            if (string.IsNullOrEmpty(deptNo))
                return "false";

            string empNoStr = getUTF8ToString("empNoStr");
            if (string.IsNullOrEmpty(empNoStr))
                return "false";

            string[] empNoArray = empNoStr.Split(',');
            try
            {
                foreach (string item in empNoArray)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;

                    Emp emp = new Emp();
                    emp.No = item;
                    emp.RetrieveFromDBSources();
                    //如果部门为空则为禁用用户，在此启用
                    if (emp.FK_Dept == null || emp.FK_Dept == "")
                    {
                        emp.FK_Dept = deptNo;
                        emp.Pass = "123";
                        emp.DirectUpdate();

                        DeptEmp deptEmp = new DeptEmp();
                        bool bExit = deptEmp.RetrieveByAttrAnd(DeptEmpAttr.FK_Dept, deptNo, DeptEmpAttr.FK_Emp, item);
                        if (!bExit)
                        {
                            deptEmp = new DeptEmp();
                            deptEmp.MyPK = deptNo + "-" + item;
                            deptEmp.FK_Dept = deptNo;
                            deptEmp.FK_Emp = item;
                            deptEmp.FK_Duty = emp.FK_Duty;
                            deptEmp.Leader = emp.Leader;
                            deptEmp.Insert();
                        }
                    }
                    //如果启用钉钉通讯录同步，编辑人员所属部门
                    if (BP.GPM.Glo.IsEnable_DingDing == true)
                    {
                        //编辑之前人员所属部门集合
                        DeptEmps deptEmps = new DeptEmps();
                        deptEmps.RetrieveByAttr(DeptEmpAttr.FK_Emp, item);
                        //部门集合
                        List<string> list_DeptIds = new List<string>();
                        foreach (DeptEmp deptEmp in deptEmps)
                        {
                            if (deptEmp.FK_Dept == deptNo)
                                continue;

                            list_DeptIds.Add(deptEmp.FK_Dept);
                        }
                        //追加当前部门编号
                        list_DeptIds.Add(deptNo);

                        DingDing ding = new DingDing();
                        Ding_Post_ReturnVal postVal = ding.GPM_Ding_EditEmp(emp, list_DeptIds);
                        if (postVal == null || postVal.errcode != "0")
                            return "Ding:" + postVal.errcode + "-" + postVal.errmsg;
                    }
                    //更新CCGPM中人员信息
                    //emp.FK_Dept = deptNo;//不修改主部门，只关联部门
                    //emp.Update();

                    DeptEmp de = new DeptEmp();
                    bool isExit = de.RetrieveByAttrAnd(DeptEmpAttr.FK_Dept, deptNo, DeptEmpAttr.FK_Emp, item);
                    if (!isExit)
                    {
                        de = new DeptEmp();
                        de.MyPK = deptNo + "-" + item;
                        de.FK_Dept = deptNo;
                        de.FK_Emp = item;
                        de.FK_Duty = emp.FK_Duty;
                        de.Leader = emp.Leader;
                        de.Insert();
                    }
                }
                return "true";
            }
            catch (Exception)
            {
                return "false";
            }
        }
        /// <summary>
        /// 密码重置
        /// </summary>
        /// <returns></returns>
        private string modifyPwdMet()
        {
            string empNo = getUTF8ToString("empNo");
            try
            {
                string appNo = BP.Sys.SystemConfig.AppSettings["CCPortal.AppNo"];
                Emp e = new Emp(empNo);

                //加密.
                if (SystemConfig.IsEnablePasswordEncryption == true)
                    e.Pass = BP.Tools.Cryptography.EncryptString("123");
                else
                    e.Pass = "123";

                //OA系统需要加密
                if ((!string.IsNullOrEmpty(appNo) && appNo == "CCOA") || SystemConfig.IsEnablePasswordEncryption)
                {
                    e.Pass = BP.Tools.Cryptography.EncryptString(e.Pass);
                }
                e.Update();

                return "true";
            }
            catch (Exception)
            {
                return "false";
            }
        }
        /// <summary>
        /// 检查该部门职务和岗位是否健全
        /// </summary>
        /// <returns></returns>
        private string checkDeptDutyAndStationMet()
        {
            string deptNo = getUTF8ToString("deptNo");

            if (string.IsNullOrEmpty(deptNo))
            {
                return "false";
            }

            DeptDutys dds = new DeptDutys();
            int count = dds.RetrieveByAttr(DeptDutyAttr.FK_Dept, deptNo);

            if (count == 0)
                return "false";

            DeptStations dss = new DeptStations();
            count = dss.RetrieveByAttr(DeptStationAttr.FK_Dept, deptNo);

            if (count == 0)
                return "false";

            return "true";
        }
        /// <summary>
        /// 以下算法只包含 oracle mysql sqlserver 三种类型的数据库 qin
        /// </summary>
        /// <param name="dataSource">表名</param>
        /// <param name="pageNumber">当前页</param>
        /// <param name="pageSize">当前页数据条数</param>
        /// <param name="key">计算总行数需要</param>
        /// <param name="orderKey">排序字段</param>
        /// <returns></returns>
        private string DBPaging(string dataSource, int pageNumber, int pageSize, string key, string orderKey)
        {
            string sql = "";
            string orderByStr = "";

            if (!string.IsNullOrEmpty(orderKey))
                orderByStr = " ORDER BY " + orderKey;

            switch (DBAccess.AppCenterDBType)
            {
                case DBType.Oracle:
                    int beginIndex = (pageNumber - 1) * pageSize + 1;
                    int endIndex = pageNumber * pageSize;

                    sql = "SELECT * FROM ( SELECT A.*, ROWNUM RN " +
                        "FROM (SELECT * FROM  " + dataSource + orderByStr + ") A WHERE ROWNUM <= " + endIndex + " ) WHERE RN >=" + beginIndex;
                    break;
                case DBType.MSSQL:
                    sql = "SELECT TOP " + pageSize + " * FROM " + dataSource + " WHERE " + key + " NOT IN  ("
                    + "SELECT TOP (" + pageSize + "*(" + pageNumber + "-1)) " + key + " FROM " + dataSource + " )" + orderByStr;
                    break;
                case DBType.MySQL:
                    pageNumber -= 1;
                    sql = "select * from  " + dataSource + orderByStr + " limit " + pageNumber + "," + pageSize;
                    break;
                default:
                    throw new Exception("暂不支持您的数据库类型.");
            }

            DataTable DTable = DBAccess.RunSQLReturnTable(sql);

            int totalCount = DBAccess.RunSQLReturnCOUNT("select " + key + " from " + dataSource);

            return DataTableConvertJson.DataTable2Json(DTable, totalCount);
        }
        #endregion

        /// <summary>
        /// 获取部门树
        /// </summary>
        /// <returns></returns>
        private string GetDeptTreeJson(string hidenDeptId)
        {
            Depts depts = new Depts();
            //不需要隐藏部门，返回全部
            if (string.IsNullOrEmpty(hidenDeptId))
            {
                depts.RetrieveAll();
                return BP.Tools.Entitis2Json.ConvertEntitis2GenerTree(depts, "0");
            }

            Dept dept = new Dept();
            dept.RetrieveByAttr(DeptAttr.No, hidenDeptId);
            //不能排除根目录
            if (dept.ParentNo == "0")
                return "[]";

            QueryObject obj = new QueryObject(depts);
            obj.AddWhereNotIn(DeptAttr.No, "'" + hidenDeptId + "'");
            obj.DoQuery();
            if (depts.Count == 0)
                return "[]";
            return BP.Tools.Entitis2Json.ConvertEntitis2GenerTree(depts, "0");
        }

        //--------------------------------------------------------------------------------------------------------------------
        #region  按岗位分配菜单
        /// <summary>
        /// 获取所有岗位
        /// </summary>
        /// <returns></returns>
        public string GetStations()
        {
            Stations stations = new Stations();
            stations.RetrieveAll("No");
            return TranslateEntitiesToGridJsonOnlyData(stations);
        }
        /// <summary>
        /// 保存 岗位 菜单
        /// </summary>
        /// <returns></returns>
        public string SaveStationOfMenus()
        {
            try
            {
                string stationNo = getUTF8ToString("stationNo");
                string menuIds = getUTF8ToString("menuIds");
                string menuIdsUn = getUTF8ToString("menuIdsUn");
                string menuIdsUnExt = getUTF8ToString("menuIdsUnExt");

                //系统和系统类别菜单编号
                string rootAndappSortMenuIds = GetRootAndAppSortMenuIds();

                //将未展开项包含的子项补充到已选择和未选择项中
                if (!string.IsNullOrEmpty(menuIdsUnExt))
                {
                    string[] menuParentNos = menuIdsUnExt.Split(',');
                    foreach (string item in menuParentNos)
                    {
                        //如果非admin,需要移除根节点和系统类别菜单编号
                        if (rootAndappSortMenuIds.Contains(item + ","))
                            continue;

                        SetUnCheckedStationOfMenus(stationNo, item, ref menuIds, ref menuIdsUn);
                    }
                }

                //超级管理员admin
                if (WebUser.No == "admin")
                {
                    StationMenus stationMenus = new StationMenus();
                    stationMenus.Delete(StationMenuAttr.FK_Station, stationNo);
                }
                else
                {
                    //删除该岗位下的菜单
                    DeleteStationMenuOfSecondAdmin(stationNo);
                }
                //保存选中菜单
                if (!string.IsNullOrEmpty(menuIds))
                {
                    string[] menus = menuIds.Split(',');
                    foreach (string item in menus)
                    {
                        //如果非admin,需要移除根节点和系统类别菜单编号
                        if (rootAndappSortMenuIds.Contains(item + ","))
                            continue;

                        StationMenu stationMenu = new StationMenu();
                        stationMenu.FK_Station = stationNo;
                        stationMenu.FK_Menu = item;
                        stationMenu.IsChecked = "1";
                        stationMenu.Insert();

                        SaveStationOfMenusChild(stationNo, item, menuIds);
                    }
                }
                //保存未完全选中项
                if (!string.IsNullOrEmpty(menuIdsUn))
                {
                    string[] menus = menuIdsUn.Split(',');
                    foreach (string item in menus)
                    {
                        StationMenu stationMenu = new StationMenu();
                        stationMenu.FK_Station = stationNo;
                        stationMenu.FK_Menu = item;
                        stationMenu.IsChecked = "0";
                        stationMenu.Insert();
                    }
                }
                //处理未完全选择项，不包含子项的未完全选择项删除
                Del_UnCheckedNoChildNodes("StationMenu", stationNo);
            }
            catch (Exception ex)
            {
                return "error" + ex.Message;
            }
            return "success";
        }
        /// <summary>
        /// 保存岗位菜单 子节点
        /// </summary>
        /// <returns></returns>
        public void SaveStationOfMenusChild(string stationNo, string parentNo, string menuIds)
        {
            //根据父节点编号获取子节点
            BP.GPM.Menus menus = new BP.GPM.Menus();
            menus.RetrieveByAttr("ParentNo", parentNo);

            foreach (BP.GPM.Menu item in menus)
            {
                if (menuIds.Contains(item.No))
                    continue;

                StationMenu stationMenu = new StationMenu();
                stationMenu.FK_Station = stationNo;
                stationMenu.FK_Menu = item.No;
                stationMenu.IsChecked = "1";
                stationMenu.Insert();

                SaveStationOfMenusChild(stationNo, item.No, menuIds);
            }

        }
        /// <summary>
        /// 获取岗位菜单
        /// </summary>
        public string GetStationOfMenusByNo()
        {
            string checkedMenuIds = "";
            string stationNo = getUTF8ToString("stationNo");
            string parentNo = getUTF8ToString("parentNo");
            string isLoadChild = getUTF8ToString("isLoadChild");
            //根据岗位编号获取菜单
            StationMenus stationMenus = new StationMenus();


            QueryObject objWhere = new QueryObject(stationMenus);
            objWhere.AddWhere(StationMenuAttr.FK_Station, stationNo);
            objWhere.addAnd();
            objWhere.AddWhere(StationMenuAttr.IsChecked, true);
            objWhere.DoQuery();

            //获取节点
            BP.GPM.Menus menus = new BP.GPM.Menus();
            menus.RetrieveByAttr("ParentNo", parentNo);

            //整理选中项
            foreach (StationMenu item in stationMenus)
            {
                checkedMenuIds += "," + item.FK_Menu + ",";
            }
            //整理未完全选中
            string unCheckedMenuIds = "";
            StationMenus unCStationMenus = new StationMenus();

            QueryObject unObjWhere = new QueryObject(unCStationMenus);
            unObjWhere.AddWhere(StationMenuAttr.FK_Station, stationNo);
            unObjWhere.addAnd();
            unObjWhere.AddWhere(StationMenuAttr.IsChecked, false);
            unObjWhere.DoQuery();
            foreach (StationMenu unItem in unCStationMenus)
            {
                unCheckedMenuIds += "," + unItem.FK_Menu + ",";
            }

            //如果是第一次加载
            if (isLoadChild == "false")
            {
                StringBuilder appSend = new StringBuilder();
                appSend.Append("[");
                foreach (EntityTree item in menus)
                {
                    if (appSend.Length > 1) appSend.Append(",{"); else appSend.Append("{");

                    appSend.Append("\"id\":\"" + item.No + "\"");
                    appSend.Append(",\"text\":\"" + item.Name + "\"");

                    BP.GPM.Menu menu = item as BP.GPM.Menu;

                    //节点图标
                    string ico = "icon-" + menu.MenuType;
                    //判断未完全选中
                    if (unCheckedMenuIds.Contains("," + item.No + ","))
                        ico = "collaboration";

                    appSend.Append(",iconCls:\"");
                    appSend.Append(ico);
                    appSend.Append("\"");

                    //判断选中
                    if (checkedMenuIds.Contains("," + item.No + ","))
                        appSend.Append(",\"checked\":true");

                    // 增加它的子级.
                    appSend.Append(",\"children\":");
                    appSend.Append(GetMenusByParentNo(item.No, checkedMenuIds, unCheckedMenuIds, true));
                    appSend.Append("}");
                }
                appSend.Append("]");

                return appSend.ToString();
            }
            //返回获取的子节点
            return GetTreeList(menus, checkedMenuIds, unCheckedMenuIds);
        }

        /// <summary>
        /// 清空式 复制岗位  保存
        /// </summary>
        /// <returns></returns>
        public string ClearOfCopyStation()
        {
            try
            {
                string copyStationNo = getUTF8ToString("copyStationNo");
                string pastStationNos = getUTF8ToString("pastStationNos");
                string[] pastArry = pastStationNos.Split(',');

                //获取复制岗位权限
                StationMenus copyStationMenus = new StationMenus();
                if (WebUser.No == "admin")
                {
                    copyStationMenus.RetrieveByAttr(StationMenuAttr.FK_Station, copyStationNo);
                }
                else
                {
                    QueryObject obj = new QueryObject(copyStationMenus);
                    obj.AddWhere(StationMenuAttr.FK_Station, copyStationNo);
                    obj.addAnd();
                    obj.AddWhere("FK_Menu in (select No from GPM_Menu a,GPM_EmpApp b where a.FK_App=b.FK_App and b.FK_Emp='" + WebUser.No + "')");
                    obj.DoQuery();
                }
                //循环目标对象
                foreach (string pastStation in pastArry)
                {
                    //清空之前的权限
                    if (WebUser.No == "admin")
                    {
                        StationMenu stationMenu = new StationMenu();
                        stationMenu.Delete(StationMenuAttr.FK_Station, pastStation);
                    }
                    else
                    {
                        DeleteStationMenuOfSecondAdmin(pastStation);
                    }

                    //授权
                    foreach (StationMenu copyMenu in copyStationMenus)
                    {
                        StationMenu stationMenu = new StationMenu();
                        stationMenu.FK_Station = pastStation;
                        stationMenu.FK_Menu = copyMenu.FK_Menu;
                        stationMenu.IsChecked = copyMenu.IsChecked;

                        stationMenu.Insert();
                    }
                }
            }
            catch (Exception ex)
            {
                return "error:" + ex.Message;
            }
            return "success";
        }

        /// <summary>
        /// 覆盖式 复制岗位 保存
        /// </summary>
        /// <returns></returns>
        public string CoverOfCopyStation()
        {
            try
            {
                string copyStationNo = getUTF8ToString("copyStationNo");
                string pastStationNos = getUTF8ToString("pastStationNos");
                string[] pastArry = pastStationNos.Split(',');

                //获取复制岗位权限
                StationMenus copyStationMenus = new StationMenus();
                if (WebUser.No == "admin")
                {
                    copyStationMenus.RetrieveByAttr(StationMenuAttr.FK_Station, copyStationNo);
                }
                else
                {
                    QueryObject obj = new QueryObject(copyStationMenus);
                    obj.AddWhere(StationMenuAttr.FK_Station, copyStationNo);
                    obj.addAnd();
                    obj.AddWhere("FK_Menu in (select No from GPM_Menu a,GPM_EmpApp b where a.FK_App=b.FK_App and b.FK_Emp='" + WebUser.No + "')");
                    obj.DoQuery();
                }

                //循环目标对象
                foreach (string pastStaion in pastArry)
                {
                    //授权
                    foreach (StationMenu copyMenu in copyStationMenus)
                    {
                        StationMenu stationMenu = new StationMenu();
                        bool isHave = stationMenu.RetrieveByAttrAnd(StationMenuAttr.FK_Station, pastStaion, StationMenuAttr.FK_Menu, copyMenu.FK_Menu);
                        //判断之前的权限是否存在
                        if (!isHave)
                        {
                            stationMenu = new StationMenu();
                            stationMenu.FK_Station = pastStaion;
                            stationMenu.FK_Menu = copyMenu.FK_Menu;
                            stationMenu.IsChecked = copyMenu.IsChecked;
                            stationMenu.Insert();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return "error:" + ex.Message;
            }
            return "success";
        }
        #endregion

        /// <summary>
        /// 岗位 模糊 查找
        /// </summary>
        /// <returns></returns>
        public string GetStationByName()
        {
            string stationName = getUTF8ToString("stationName");
            Stations stations = new Stations();
            QueryObject qo = new QueryObject(stations);
            qo.AddWhere(StationAttr.Name, " LIKE ", "'%" + stationName + "%'");
            qo.addOr();
            qo.AddWhere(StationAttr.No, " LIKE ", "'%" + stationName + "%'");
            qo.DoQuery();

            return TranslateEntitiesToGridJsonOnlyData(stations);
        }

        /// <summary>
        /// 获取模板数据
        /// </summary>
        /// <returns></returns>
        public string getTemplateData()
        {
            string sql = "";
            string menuNo = getUTF8ToString("menuNo");
            string model = getUTF8ToString("model");
            //按岗位分配
            if (model == "station")
            {
                sql = "SELECT a.No,a.Name"
                            + ",(case b.IsChecked "
                            + "when 1 then 1 "
                            + "when 0 then 1 "
                            + "else 5 end) as isCheck "
                            + "FROM Port_Station a "
                            + "left join GPM_StationMenu b "
                            + "on a.No=b.FK_Station "
                            + "and b.FK_Menu=" + menuNo
                            + " order by a.No";
                //获取所有岗位
                StationMenu station = new StationMenu();
                DataTable dt_StationMenu = station.RunSQLReturnTable(sql);
                string rVal = CommonDbOperator.GetListJsonFromTable(dt_StationMenu);
                rVal = "{station:" + rVal + "}";
                return rVal;
            }
            //按权限组分配
            if (model == "group")
            {
                sql = "SELECT a.No,a.Name"
                            + ",(case b.IsChecked "
                            + "when 1 then 1 "
                            + "when 0 then 1 "
                            + "else 5 end) as isCheck "
                            + "FROM GPM_Group a "
                            + "left join GPM_GroupMenu b "
                            + "on a.No=b.FK_Group "
                            + "and b.FK_Menu=" + menuNo
                            + " order by Idx";
                //获取所有权限组
                Group group = new Group();
                DataTable dt_GroupMenu = group.RunSQLReturnTable(sql);

                string rVal = CommonDbOperator.GetListJsonFromTable(dt_GroupMenu);
                rVal = "{group:" + rVal + "}";
                return rVal;
            }
            //按用户分配菜单
            sql = "SELECT distinct a.No,a.Name,a.FK_Dept,"
                        + "(case b.IsChecked "
                        + " when 1 then 1"
                        + " when 0 then 1"
                        + " else 5"
                        + " end) isCheck"
                        + " FROM Port_Emp a left join V_GPM_EmpMenu_GPM b"
                        + " on a.No=b.FK_Emp"
                        + " and b.FK_Menu =" + menuNo
                        + " order by a.Name";
            string strdept = GetDeptManagerInfo();
            Emp emp = new Emp();
            DataTable dt_Emp = emp.RunSQLReturnTable(sql);
            string stremp = CommonDbOperator.GetListJsonFromTable(dt_Emp);
            return "{bmList:" + strdept + ",empList:" + stremp + "}";
        }

        /// <summary>
        /// 保存按菜单分配权限
        /// </summary>
        /// <returns></returns>
        private string SaveMenuForEmp()
        {
            try
            {
                string menuNo = getUTF8ToString("menuNo");
                string saveNos = getUTF8ToString("ckNos");
                string curModel = getUTF8ToString("model");
                string saveChildNode = getUTF8ToString("saveChildNode");
                string[] str_Arrary = saveNos.Split(',');
                //按用户分配权限
                if (curModel == "emp")
                {
                    //删除菜单下的所有用户
                    UserMenus userMenus = new UserMenus();
                    userMenus.Delete("FK_Menu", menuNo);
                    //对用户进行授权
                    foreach (string item in str_Arrary)
                    {
                        UserMenu userMenu = new UserMenu();
                        userMenu.FK_Emp = item;
                        userMenu.FK_Menu = menuNo;
                        userMenu.IsChecked = "1";
                        userMenu.Insert();

                        //保存子菜单
                        if (saveChildNode == "true")
                            SaveUserOfMenusChild(item, menuNo, menuNo);
                    }
                    return "true";
                }
                //按岗位分配权限
                if (curModel == "station")
                {
                    //删除菜单下的所有岗位
                    StationMenus staMenus = new StationMenus();
                    staMenus.Delete("FK_Menu", menuNo);

                    //对用户进行授权
                    foreach (string item in str_Arrary)
                    {
                        StationMenu stationMenu = new StationMenu();
                        stationMenu.FK_Station = item;
                        stationMenu.FK_Menu = menuNo;
                        stationMenu.IsChecked = "1";
                        stationMenu.Insert();
                        //保存子菜单
                        if (saveChildNode == "true")
                            SaveStationOfMenusChild(item, menuNo, menuNo);
                    }
                    return "true";
                }
                //删除菜单下的权限组
                GroupMenus groupMenus = new GroupMenus();
                groupMenus.Delete(GroupMenuAttr.FK_Menu, menuNo);
                //对权限组进行授权
                foreach (string item in str_Arrary)
                {
                    GroupMenu groupMenu = new GroupMenu();
                    groupMenu.FK_Group = item;
                    groupMenu.FK_Menu = menuNo;
                    groupMenu.IsChecked = "1";
                    groupMenu.Insert();
                    //对子节点进行授权
                    if (saveChildNode == "true")
                        SaveGroupOfMenusChild(item, menuNo, menuNo);
                }
                return "true";
            }
            catch (Exception ex)
            {
                BP.DA.Log.DebugWriteError(ex.Message);
                return "false";
            }
        }

        /// <summary>
        /// 获取包含人员的部门,没有人员的就不加载
        /// </summary>
        /// <returns></returns>
        public string GetDeptManagerInfo()
        {
            string sql = "SELECT distinct No,Name,ParentNo,NameOfPath,Idx "
                + "FROM Port_Dept a,Port_DeptEmp b "
                + "where b.FK_Dept = a.No order by ParentNo,Idx";
            Dept dept = new Dept();
            DataTable dt = dept.RunSQLReturnTable(sql);
            return CommonDbOperator.GetListJsonFromTable(dt);
        }

        /// <summary>
        /// 获取 所有部门
        /// </summary>
        /// <returns></returns>
        public string GetAllDept()
        {
            Depts depts = new Depts();
            depts.RetrieveAll();
            return TranslateEntitiesToGridJsonOnlyData(depts);
        }

        /// <summary>
        /// 获取所有人员信息
        /// </summary>
        /// <returns></returns>
        private string GetEmps()
        {
            Emps emps = new Emps();
            emps.RetrieveAll("No");
            return TranslateEntitiesToGridJsonOnlyData(emps);
        }

        /// <summary>
        /// 根据用户编号或名称模糊查询
        /// </summary>
        /// <returns></returns>
        private string GetEmpsByNoOrName()
        {
            string objSearch = getUTF8ToString("objSearch");
            Emps emps = new Emps();
            BP.En.QueryObject qo = new QueryObject(emps);

            qo.AddWhere(EmpAttr.No, " LIKE ", "'%" + objSearch + "%'");
            qo.addOr();
            qo.AddWhere(EmpAttr.Name, " LIKE ", "'%" + objSearch + "%'");
            qo.addOr();
            qo.AddWhere(EmpAttr.EmpNo, " LIKE ", "'%" + objSearch + "%'");

            qo.DoQuery();
            return TranslateEntitiesToGridJsonOnlyData(emps);
        }

        /// <summary>
        /// 根据用户账号、工号、姓名或手机号
        /// </summary>
        /// <returns></returns>
        private string SearchByEmpNoOrName()
        {
            string objSearch = getUTF8ToString("objSearch");
            string pageNumber = getUTF8ToString("pageNumber");
            int iPageNumber = string.IsNullOrEmpty(pageNumber) ? 1 : Convert.ToInt32(pageNumber);
            //每页多少行
            string pageSize = getUTF8ToString("pageSize");
            int iPageSize = string.IsNullOrEmpty(pageSize) ? 9999 : Convert.ToInt32(pageSize);

            string sql = string.Format("("
                                       + "     SELECT pe.*,"
                                       + "            pd.Name FK_DutyText"
                                       + "     FROM   Port_Emp pe"
                                       + "            LEFT JOIN Port_Duty pd"
                                       + "                 ON  pd.No = pe.FK_Duty"
                                       + "     WHERE  pe.FK_Dept IS NOT NULL"
                                       + "            AND ("
                                       + "                    pe.No LIKE '%{0}%'"
                                       + "                    OR pe.Name LIKE '%{0}%'"
                                       + "                    OR pe.EmpNo LIKE '%{0}%'"
                                       + "                    OR pe.Tel LIKE '%{0}%'"
                                       + "                )"
                                       + " ) dbSo", objSearch);

            return DBPaging(sql, iPageNumber, iPageSize, "No", "Name");
        }

        /// <summary>
        /// 查找所有权限组
        /// </summary>
        /// <returns></returns>
        private string GetEmpGroups()
        {
            Groups groups = new Groups();
            groups.RetrieveAll(GroupAttr.Idx);
            return TranslateEntitiesToGridJsonOnlyData(groups);
        }

        /// <summary>
        /// 权限组模糊查找
        /// </summary>
        /// <returns></returns>
        private string GetEmpGroupsByName()
        {
            string objSearch = getUTF8ToString("objSearch");
            Groups groups = new Groups();
            QueryObject qo = new QueryObject(groups);
            qo.AddWhere(GroupAttr.Name, " LIKE ", "'%" + objSearch + "%'");
            qo.addOr();
            qo.AddWhere(GroupAttr.No, " LIKE ", "'%" + objSearch + "%'");
            qo.DoQuery();

            return TranslateEntitiesToGridJsonOnlyData(groups);
        }

        /// <summary>
        /// 保存用户与菜单的关系
        /// </summary>
        /// <returns></returns>
        private string SaveUserOfMenus()
        {
            try
            {
                string empNo = getUTF8ToString("empNo");
                string menuIds = getUTF8ToString("menuIds");
                string menuIdsUn = getUTF8ToString("menuIdsUn");
                string menuIdsUnExt = getUTF8ToString("menuIdsUnExt");

                //系统和系统类别菜单编号
                string rootAndappSortMenuIds = GetRootAndAppSortMenuIds();

                //将未展开项包含的子项补充到已选择和未选择项中
                if (!string.IsNullOrEmpty(menuIdsUnExt))
                {
                    string[] menuParentNos = menuIdsUnExt.Split(',');
                    foreach (string item in menuParentNos)
                    {
                        //如果非admin,需要移除根节点和系统类别菜单编号
                        if (rootAndappSortMenuIds.Contains(item + ","))
                            continue;
                        SetUnCheckedUserOfMenus(empNo, item, ref menuIds, ref menuIdsUn);
                    }
                }

                //超级管理员admin
                if (WebUser.No == "admin")
                {
                    //删除用户下的菜单
                    UserMenus userMenus = new UserMenus();
                    userMenus.Delete("FK_Emp", empNo);
                }
                else
                {
                    DeleteUserMenuOfSecondAdmin(empNo);
                }

                //保存选中菜单
                if (!string.IsNullOrEmpty(menuIds))
                {
                    string[] menus = menuIds.Split(',');
                    foreach (string item in menus)
                    {
                        //如果非admin,需要移除根节点和系统类别菜单编号
                        if (rootAndappSortMenuIds.Contains(item + ","))
                            continue;

                        UserMenu userMenu = new UserMenu();
                        userMenu.FK_Emp = empNo;
                        userMenu.FK_Menu = item;
                        userMenu.IsChecked = "1";
                        userMenu.Insert();
                        //追加菜单的子菜单为选中
                        SaveUserOfMenusChild(empNo, item, menuIds);
                    }
                }
                //保存未完全选中项
                if (!string.IsNullOrEmpty(menuIdsUn))
                {
                    string[] menus = menuIdsUn.Split(',');
                    foreach (string item in menus)
                    {
                        UserMenu userMenu = new UserMenu();
                        userMenu.FK_Emp = empNo;
                        userMenu.FK_Menu = item;
                        userMenu.IsChecked = "0";
                        userMenu.Insert();
                    }
                }
                //处理未完全选择项，不包含子项的未完全选择项删除
                Del_UnCheckedNoChildNodes("UserMenu", empNo);
            }
            catch (Exception ex)
            {
                return "error" + ex.Message;
            }
            return "success";
        }

        /// <summary>
        /// 保存用户菜单子节点
        /// </summary>
        private void SaveUserOfMenusChild(string fk_EmpNo, string parentNo, string menuIds)
        {
            //根据父节点编号获取子节点
            BP.GPM.Menus menus = new BP.GPM.Menus();
            menus.RetrieveByAttr("ParentNo", parentNo);

            foreach (BP.GPM.Menu cMenu in menus)
            {
                if (menuIds.Contains(cMenu.No))
                    continue;

                //删除菜单下的所有用户
                UserMenus userMenus = new UserMenus();
                userMenus.Delete("FK_Menu", cMenu.No, "FK_Emp", fk_EmpNo);

                UserMenu userMenu = new UserMenu();
                userMenu.FK_Emp = fk_EmpNo;
                userMenu.FK_Menu = cMenu.No;
                userMenu.IsChecked = "1";
                userMenu.Insert();

                SaveUserOfMenusChild(fk_EmpNo, cMenu.No, menuIds);
            }
        }

        /// <summary>
        /// 设置未展开项
        /// </summary>
        /// <param name="fk_EmpNo">用户编号</param>
        /// <param name="parentNo">父节点编号</param>
        /// <param name="menuIds">选择项，进行拼接</param>
        /// <param name="menuIdsUn">未完全选中项，进行拼接</param>
        private void SetUnCheckedUserOfMenus(string fk_EmpNo, string parentNo, ref string menuIds, ref string menuIdsUn)
        {
            //根据父节点编号获取子节点
            BP.GPM.Menu menu = new BP.GPM.Menu();
            string sql = "SELECT a.FK_Emp,a.FK_Menu,a.IsChecked FROM GPM_UserMenu a,GPM_Menu b "
                            + " WHERE a.FK_Menu = b.No "
                            + " AND b.ParentNo='" + parentNo + "'"
                            + " AND a.FK_Emp='" + fk_EmpNo + "'";
            //获取数据集
            DataTable dt = menu.RunSQLReturnTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    //未完全选中
                    if (row["IsChecked"].ToString() == "0")
                    {
                        if (!menuIdsUn.Contains(row["FK_Menu"].ToString()))
                            menuIdsUn += "," + row["FK_Menu"];
                    }
                    //选中
                    if (row["IsChecked"].ToString() == "1")
                    {
                        if (string.IsNullOrEmpty(menuIds))
                        {
                            menuIds = row["FK_Menu"].ToString();
                        }
                        else
                        {
                            if (!menuIds.Contains(row["FK_Menu"].ToString()))
                                menuIds += "," + row["FK_Menu"];
                        }
                    }
                    //迭代进行处理
                    SetUnCheckedUserOfMenus(fk_EmpNo, row["FK_Menu"].ToString(), ref menuIds, ref menuIdsUn);
                }
            }
        }

        /// <summary>
        /// 设置未展开项
        /// </summary>
        /// <param name="fk_StationNo">岗位编号</param>
        /// <param name="parentNo">父节点编号</param>
        /// <param name="menuIds">选择项，进行拼接</param>
        /// <param name="menuIdsUn">未完全选中项，进行拼接</param>
        private void SetUnCheckedStationOfMenus(string fk_StationNo, string parentNo, ref string menuIds, ref string menuIdsUn)
        {
            //根据父节点编号获取子节点
            BP.GPM.Menu menu = new BP.GPM.Menu();
            string sql = "SELECT a.FK_Station,a.FK_Menu,a.IsChecked FROM GPM_StationMenu a,GPM_Menu b "
                            + " WHERE a.FK_Menu = b.No "
                            + " AND b.ParentNo='" + parentNo + "'"
                            + " AND a.FK_Station='" + fk_StationNo + "'";
            //获取数据集
            DataTable dt = menu.RunSQLReturnTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    //未完全选中
                    if (row["IsChecked"].ToString() == "0")
                    {
                        if (!menuIdsUn.Contains(row["FK_Menu"].ToString()))
                            menuIdsUn += "," + row["FK_Menu"];
                    }
                    //选中
                    if (row["IsChecked"].ToString() == "1")
                    {
                        if (string.IsNullOrEmpty(menuIds))
                        {
                            menuIds = row["FK_Menu"].ToString();
                        }
                        else
                        {
                            if (!menuIds.Contains(row["FK_Menu"].ToString()))
                                menuIds += "," + row["FK_Menu"];
                        }
                    }
                    //迭代进行处理
                    SetUnCheckedStationOfMenus(fk_StationNo, row["FK_Menu"].ToString(), ref menuIds, ref menuIdsUn);
                }
            }
        }

        /// <summary>
        /// 设置未展开项
        /// </summary>
        /// <param name="FK_Group">权限组编号</param>
        /// <param name="parentNo">父节点编号</param>
        /// <param name="menuIds">选择项，进行拼接</param>
        /// <param name="menuIdsUn">未完全选中项，进行拼接</param>
        private void SetUnCheckedGroupOfMenus(string FK_Group, string parentNo, ref string menuIds, ref string menuIdsUn)
        {
            //根据父节点编号获取子节点
            BP.GPM.Menu menu = new BP.GPM.Menu();
            string sql = "SELECT a.FK_Group,a.FK_Menu,a.IsChecked FROM GPM_GroupMenu a,GPM_Menu b "
                            + " WHERE a.FK_Menu = b.No "
                            + " AND b.ParentNo='" + parentNo + "'"
                            + " AND a.FK_Group='" + FK_Group + "'";
            //获取数据集
            DataTable dt = menu.RunSQLReturnTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    //未完全选中
                    if (row["IsChecked"].ToString() == "0")
                    {
                        if (!menuIdsUn.Contains(row["FK_Menu"].ToString()))
                            menuIdsUn += "," + row["FK_Menu"];
                    }
                    //选中
                    if (row["IsChecked"].ToString() == "1")
                    {
                        if (string.IsNullOrEmpty(menuIds))
                        {
                            menuIds = row["FK_Menu"].ToString();
                        }
                        else
                        {
                            if (!menuIds.Contains(row["FK_Menu"].ToString()))
                                menuIds += "," + row["FK_Menu"];
                        }
                    }
                    //迭代进行处理
                    SetUnCheckedGroupOfMenus(FK_Group, row["FK_Menu"].ToString(), ref menuIds, ref menuIdsUn);
                }
            }
        }

        /// <summary>
        /// 将未完全选择项，不包含子节点的节点删除
        /// </summary>
        /// <param name="saveType"></param>
        /// <param name="FK_Val"></param>
        private void Del_UnCheckedNoChildNodes(string saveType, string FK_Val)
        {
            string sql = "";
            //保存用户菜单
            if (saveType == "UserMenu")
            {
                UserMenus userMenus = new UserMenus();
                userMenus.Retrieve("FK_Emp", FK_Val, "IsChecked", "0");
                if (userMenus != null && userMenus.Count > 0)
                {
                    //循环删除子级项，方法有待优化；先删除3级关联
                    for (int i = 0, k = 3; i < k; i++)
                    {
                        foreach (UserMenu userMenu in userMenus)
                        {
                            sql = "SELECT a.FK_Emp,a.FK_Menu,a.IsChecked FROM GPM_UserMenu a,GPM_Menu b "
                                + " WHERE a.FK_Menu = b.No"
                                + " AND b.ParentNo='" + userMenu.FK_Menu + "'"
                                + " AND a.FK_Emp='" + FK_Val + "'";
                            DataTable dt_UserMenu = DBAccess.RunSQLReturnTable(sql);
                            //判断是否含有子项
                            if (dt_UserMenu == null || dt_UserMenu.Rows.Count == 0)
                            {
                                //执行删除
                                DBAccess.RunSQL("DELETE FROM GPM_UserMenu WHERE FK_Emp='" + FK_Val + "' and FK_Menu='" + userMenu.FK_Menu + "'");
                            }
                        }
                    }
                }
            }
            else if (saveType == "StationMenu")//岗位菜单
            {
                StationMenus stationMenus = new StationMenus();
                stationMenus.Retrieve("FK_Station", FK_Val, "IsChecked", "0");
                if (stationMenus != null && stationMenus.Count > 0)
                {
                    //循环删除子级项，方法有待优化；先删除3级关联
                    for (int i = 0, k = 3; i < k; i++)
                    {
                        foreach (StationMenu stationMenu in stationMenus)
                        {
                            sql = "SELECT a.FK_Station,a.FK_Menu,a.IsChecked FROM GPM_StationMenu a,GPM_Menu b "
                                + " WHERE a.FK_Menu = b.No"
                                + " AND b.ParentNo='" + stationMenu.FK_Menu + "'"
                                + " AND a.FK_Station='" + FK_Val + "'";
                            DataTable dt_StationMenu = DBAccess.RunSQLReturnTable(sql);
                            //判断是否含有子项
                            if (dt_StationMenu == null || dt_StationMenu.Rows.Count == 0)
                            {
                                //执行删除
                                DBAccess.RunSQL("DELETE FROM GPM_StationMenu WHERE FK_Station='" + FK_Val + "' and FK_Menu='" + stationMenu.FK_Menu + "'");
                            }
                        }
                    }
                }
            }
            else if (saveType == "GroupMenu")//权限组菜单
            {
                GroupMenus groupMenus = new GroupMenus();
                groupMenus.Retrieve("FK_Group", FK_Val, "IsChecked", "0");
                if (groupMenus != null && groupMenus.Count > 0)
                {
                    //循环删除子级项，方法有待优化；先删除3级关联
                    for (int i = 0, k = 3; i < k; i++)
                    {
                        foreach (GroupMenu groupMenu in groupMenus)
                        {
                            sql = "SELECT a.FK_Group,a.FK_Menu,a.IsChecked FROM GPM_GroupMenu a,GPM_Menu b "
                                + " WHERE a.FK_Menu = b.No"
                                + " AND b.ParentNo='" + groupMenu.FK_Menu + "'"
                                + " AND a.FK_Group='" + FK_Val + "'";
                            DataTable dt_StationMenu = DBAccess.RunSQLReturnTable(sql);
                            //判断是否含有子项
                            if (dt_StationMenu == null || dt_StationMenu.Rows.Count == 0)
                            {
                                //执行删除
                                DBAccess.RunSQL("DELETE FROM GPM_GroupMenu WHERE FK_Group='" + FK_Val + "' and FK_Menu='" + groupMenu.FK_Menu + "'");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 保存权限组菜单子节点
        /// </summary>
        private void SaveGroupOfMenusChild(string fk_GroupNo, string parentNo, string menuIds)
        {
            //根据父节点编号获取子节点
            BP.GPM.Menus menus = new BP.GPM.Menus();
            menus.RetrieveByAttr("ParentNo", parentNo);

            foreach (BP.GPM.Menu item in menus)
            {
                if (menuIds.Contains(item.No))
                    continue;

                GroupMenu groupMenu = new GroupMenu();
                groupMenu.FK_Group = fk_GroupNo;
                groupMenu.FK_Menu = item.No;
                groupMenu.IsChecked = "1";
                groupMenu.Insert();

                SaveGroupOfMenusChild(fk_GroupNo, item.No, menuIds);
            }
        }

        /// <summary>
        /// 获取权限组菜单
        /// </summary>
        /// <returns></returns>
        private string GetEmpGroupOfMenusByNo()
        {
            string checkedMenuIds = "";
            string groupNO = getUTF8ToString("groupNo");
            string parentNo = getUTF8ToString("parentNo");
            string isLoadChild = getUTF8ToString("isLoadChild");
            //根据权限组编号获取菜单
            GroupMenus groupMenus = new GroupMenus();

            QueryObject objWhere = new QueryObject(groupMenus);
            objWhere.AddWhere(GroupMenuAttr.FK_Group, groupNO);
            objWhere.addAnd();
            objWhere.AddWhere(GroupMenuAttr.IsChecked, true);

            objWhere.DoQuery();
            //获取节点
            BP.GPM.Menus menus = new BP.GPM.Menus();
            menus.RetrieveByAttr("ParentNo", parentNo);

            //整理选中项
            foreach (GroupMenu item in groupMenus)
            {
                checkedMenuIds += "," + item.FK_Menu + ",";
            }
            //整理未完全选中
            string unCheckedMenuIds = "";
            GroupMenus unCGroupMenus = new GroupMenus();
            QueryObject unObjWhere = new QueryObject(unCGroupMenus);
            unObjWhere.AddWhere(GroupMenuAttr.FK_Group, groupNO);
            unObjWhere.addAnd();
            unObjWhere.AddWhere(GroupMenuAttr.IsChecked, false);
            unObjWhere.DoQuery();
            foreach (GroupMenu unItem in unCGroupMenus)
            {
                unCheckedMenuIds += "," + unItem.FK_Menu + ",";
            }

            //如果是第一次加载
            if (isLoadChild == "false")
            {
                StringBuilder appSend = new StringBuilder();
                appSend.Append("[");
                foreach (EntityTree item in menus)
                {
                    if (appSend.Length > 1) appSend.Append(",{"); else appSend.Append("{");

                    appSend.Append("\"id\":\"" + item.No + "\"");
                    appSend.Append(",\"text\":\"" + item.Name + "\"");

                    BP.GPM.Menu menu = item as BP.GPM.Menu;

                    //节点图标
                    string ico = "icon-" + menu.MenuType;
                    //判断未完全选中
                    if (unCheckedMenuIds.Contains("," + item.No + ","))
                        ico = "collaboration";

                    appSend.Append(",iconCls:\"");
                    appSend.Append(ico);
                    appSend.Append("\"");

                    //判断选中
                    if (checkedMenuIds.Contains("," + item.No + ","))
                        appSend.Append(",\"checked\":true");

                    // 增加它的子级.
                    appSend.Append(",\"children\":");
                    appSend.Append(GetMenusByParentNo(item.No, checkedMenuIds, unCheckedMenuIds, true));
                    appSend.Append("}");
                }
                appSend.Append("]");

                return appSend.ToString();
            }
            //返回获取的子节点
            return GetTreeList(menus, checkedMenuIds, unCheckedMenuIds);
        }

        /// <summary>
        /// 保存权限组菜单
        /// </summary>
        /// <returns></returns>
        private string SaveUserGroupOfMenus()
        {
            try
            {
                string groupNo = getUTF8ToString("groupNo");
                string menuIds = getUTF8ToString("menuIds");
                string menuIdsUn = getUTF8ToString("menuIdsUn");
                string menuIdsUnExt = getUTF8ToString("menuIdsUnExt");

                //系统和系统类别菜单编号
                string rootAndappSortMenuIds = GetRootAndAppSortMenuIds();

                //将未展开项包含的子项补充到已选择和未选择项中
                if (!string.IsNullOrEmpty(menuIdsUnExt))
                {
                    string[] menuParentNos = menuIdsUnExt.Split(',');
                    foreach (string item in menuParentNos)
                    {
                        //如果非admin,需要移除根节点和系统类别菜单编号
                        if (rootAndappSortMenuIds.Contains(item + ","))
                            continue;

                        SetUnCheckedGroupOfMenus(groupNo, item, ref menuIds, ref menuIdsUn);
                    }
                }
                //超级管理员admin
                if (WebUser.No == "admin")
                {
                    //删除权限组下的菜单
                    GroupMenus groupMenus = new GroupMenus();
                    groupMenus.Delete(GroupMenuAttr.FK_Group, groupNo);
                }
                else
                {
                    //删除权限组下的菜单
                    DeleteGroupMenuOfSecondAdmin(groupNo);
                }
                //保存选中菜单
                if (!string.IsNullOrEmpty(menuIds))
                {
                    string[] menus = menuIds.Split(',');
                    foreach (string item in menus)
                    {
                        //如果非admin,需要移除根节点和系统类别菜单编号
                        if (rootAndappSortMenuIds.Contains(item + ","))
                            continue;
                        GroupMenu groupMenu = new GroupMenu();
                        groupMenu.FK_Group = groupNo;
                        groupMenu.FK_Menu = item;
                        groupMenu.IsChecked = "1";
                        groupMenu.Insert();
                        //追加菜单的子菜单为选中
                        SaveGroupOfMenusChild(groupNo, item, menuIds);
                    }
                }
                //保存未完全选中项
                if (!string.IsNullOrEmpty(menuIdsUn))
                {
                    string[] menus = menuIdsUn.Split(',');
                    foreach (string item in menus)
                    {
                        GroupMenu groupMenu = new GroupMenu();
                        groupMenu.FK_Group = groupNo;
                        groupMenu.FK_Menu = item;
                        groupMenu.IsChecked = "0";
                        groupMenu.Insert();
                    }
                }
                //处理未完全选择项，不包含子项的未完全选择项删除
                Del_UnCheckedNoChildNodes("GroupMenu", groupNo);
            }
            catch (Exception ex)
            {
                return "error" + ex.Message;
            }
            return "success";
        }

        /// <summary>
        /// 清空式复制用户权限
        /// </summary>
        /// <returns></returns>
        private string ClearOfCopyUserPower()
        {
            try
            {
                string copyUser = getUTF8ToString("copyUser");
                string pastUsers = getUTF8ToString("pastUsers");
                string[] pastArry = pastUsers.Split(',');

                UserMenu userMenu = new UserMenu();
                //默认admin拥有所有权限
                string sql = "SELECT FK_Menu,IsChecked FROM V_GPM_EmpMenu_GPM WHERE FK_Emp='" + copyUser + "'";
                if (WebUser.No != "admin")
                {
                    //获取二级管理员所属范围内的菜单
                    sql = string.Format("SELECT FK_Menu,IsChecked FROM V_GPM_EmpMenu_GPM a,GPM_EmpApp b WHERE a.FK_App=b.FK_App and b.FK_Emp='{0}' and a.FK_Emp='{1}'", WebUser.No, copyUser);
                }

                //获取源用户权限
                DataTable copyUserMenu_dt = userMenu.RunSQLReturnTable(sql);

                //循环目标对象
                foreach (string pastUser in pastArry)
                {
                    //清空之前的权限
                    if (WebUser.No == "admin")
                    {
                        userMenu = new UserMenu();
                        userMenu.FK_Emp = pastUser;
                        userMenu.Delete();
                    }
                    else
                    {
                        DeleteUserMenuOfSecondAdmin(pastUser);
                    }

                    //授权
                    foreach (DataRow copyRow in copyUserMenu_dt.Rows)
                    {
                        userMenu = new UserMenu();
                        userMenu.FK_Emp = pastUser;
                        userMenu.FK_Menu = copyRow["FK_Menu"].ToString();
                        userMenu.IsChecked = copyRow["IsChecked"].ToString();
                        userMenu.Insert();
                    }
                }
            }
            catch (Exception ex)
            {
                return "error:" + ex.Message;
            }
            return "success";
        }

        /// <summary>
        /// 覆盖式复制用户权限
        /// </summary>
        /// <returns></returns>
        private string CoverOfCopyUserPower()
        {
            try
            {
                string copyUser = getUTF8ToString("copyUser");
                string pastUsers = getUTF8ToString("pastUsers");
                string[] pastArry = pastUsers.Split(',');

                UserMenu userMenu = new UserMenu();
                //默认admin拥有所有权限
                string sql = "SELECT FK_Menu,IsChecked FROM V_GPM_EmpMenu_GPM WHERE FK_Emp='" + copyUser + "'";
                if (WebUser.No != "admin")
                {
                    //获取二级管理员所属范围内的菜单
                    sql = string.Format("SELECT FK_Menu,IsChecked FROM V_GPM_EmpMenu_GPM a,GPM_EmpApp b WHERE a.FK_App=b.FK_App and b.FK_Emp='{0}' and a.FK_Emp='{1}'", WebUser.No, copyUser);
                }
                //获取源用户权限
                DataTable copyUserMenu_dt = userMenu.RunSQLReturnTable(sql);

                //循环目标对象
                foreach (string pastUser in pastArry)
                {
                    //目标对象已存在菜单权限
                    userMenu = new UserMenu();
                    DataTable menu_dt = userMenu.RunSQLReturnTable("SELECT FK_Menu,IsChecked FROM V_GPM_EmpMenu_GPM WHERE FK_Emp='" + pastUser + "'");
                    //授权
                    foreach (DataRow copyRow in copyUserMenu_dt.Rows)
                    {
                        bool isHave = false;
                        foreach (DataRow psRow in menu_dt.Rows)
                        {
                            if (copyRow["FK_Menu"].ToString() == psRow["FK_Menu"].ToString())
                            {
                                isHave = true;
                                break;
                            }
                        }
                        //如果不存在就新增
                        if (isHave == false)
                        {
                            userMenu = new UserMenu();
                            userMenu.FK_Emp = pastUser;
                            userMenu.FK_Menu = copyRow["FK_Menu"].ToString();
                            userMenu.IsChecked = copyRow["IsChecked"].ToString();
                            userMenu.Insert();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return "error:" + ex.Message;
            }
            return "success";
        }

        /// <summary>
        /// 清空式复制权限组权限
        /// </summary>
        /// <returns></returns>
        private string ClearOfCopyUserGroupPower()
        {
            try
            {
                string copyGroupNo = getUTF8ToString("copyGroupNo");
                string pastGroupNos = getUTF8ToString("pastGroupNos");
                string[] pastArry = pastGroupNos.Split(',');

                //获取复制权限组权限
                GroupMenus copyGroupMenus = new GroupMenus();
                if (WebUser.No == "admin")
                {
                    copyGroupMenus.RetrieveByAttr(GroupMenuAttr.FK_Group, copyGroupNo);
                }
                else
                {
                    QueryObject obj = new QueryObject(copyGroupMenus);
                    obj.AddWhere(GroupMenuAttr.FK_Group, copyGroupNo);
                    obj.addAnd();
                    obj.AddWhere("FK_Menu in (select No from GPM_Menu a,GPM_EmpApp b where a.FK_App=b.FK_App and b.FK_Emp='" + WebUser.No + "')");
                    obj.DoQuery();
                }
                //循环目标对象
                foreach (string pastGroup in pastArry)
                {
                    //清空之前的权限
                    //清空之前的权限
                    if (WebUser.No == "admin")
                    {
                        GroupMenu groupMenu = new GroupMenu();
                        groupMenu.Delete(GroupMenuAttr.FK_Group, pastGroup);
                    }
                    else
                    {
                        DeleteGroupMenuOfSecondAdmin(pastGroup);
                    }
                    //授权
                    foreach (GroupMenu copyMenu in copyGroupMenus)
                    {
                        GroupMenu groupMenu = new GroupMenu();
                        groupMenu.FK_Group = pastGroup;
                        groupMenu.FK_Menu = copyMenu.FK_Menu;
                        groupMenu.IsChecked = copyMenu.IsChecked;
                        groupMenu.Insert();
                    }
                }
            }
            catch (Exception ex)
            {
                return "error:" + ex.Message;
            }
            return "success";
        }

        /// <summary>
        /// 覆盖式复制权限组权限
        /// </summary>
        /// <returns></returns>
        private string CoverOfCopyUserGroupPower()
        {
            try
            {
                string copyGroupNo = getUTF8ToString("copyGroupNo");
                string pastGroupNos = getUTF8ToString("pastGroupNos");
                string[] pastArry = pastGroupNos.Split(',');

                //获取复制权限组权限
                GroupMenus copyGroupMenus = new GroupMenus();
                if (WebUser.No == "admin")
                {
                    copyGroupMenus.RetrieveByAttr(GroupMenuAttr.FK_Group, copyGroupNo);
                }
                else
                {
                    QueryObject obj = new QueryObject(copyGroupMenus);
                    obj.AddWhere(GroupMenuAttr.FK_Group, copyGroupNo);
                    obj.addAnd();
                    obj.AddWhere("FK_Menu in (select No from GPM_Menu a,GPM_EmpApp b where a.FK_App=b.FK_App and b.FK_Emp='" + WebUser.No + "')");
                    obj.DoQuery();
                }

                //循环目标对象
                foreach (string pastGroup in pastArry)
                {
                    //授权
                    foreach (GroupMenu copyMenu in copyGroupMenus)
                    {
                        GroupMenu groupMenu = new GroupMenu();
                        bool isHave = groupMenu.RetrieveByAttrAnd(GroupMenuAttr.FK_Group, pastGroup, GroupMenuAttr.FK_Menu, copyMenu.FK_Menu);
                        //判断之前的权限是否存在
                        if (!isHave)
                        {
                            groupMenu = new GroupMenu();
                            groupMenu.FK_Group = pastGroup;
                            groupMenu.FK_Menu = copyMenu.FK_Menu;
                            groupMenu.IsChecked = copyMenu.IsChecked;
                            groupMenu.Insert();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return "error:" + ex.Message;
            }
            return "success";
        }

        /// <summary>
        /// 获取系统
        /// </summary>
        /// <returns></returns>
        private string GetApps()
        {
            Apps apps = new Apps();
            apps.RetrieveAll();
            //admin返回所有系统权限
            if (WebUser.No == "admin")
            {
                return TranslateEntitiesToGridJsonOnlyData(apps);
            }
            //如果非admin判断其权限
            EmpApps empApps = new EmpApps();
            empApps.RetrieveByAttr(EmpAppAttr.FK_Emp, WebUser.No);
            //创建新集合
            Apps adminApps = new Apps();
            foreach (BP.GPM.App app in apps)
            {
                foreach (EmpApp empApp in empApps)
                {
                    if (app.No == empApp.FK_App)
                        adminApps.AddEntity(app);
                }
            }
            return TranslateEntitiesToGridJsonOnlyData(adminApps);
        }

        /// <summary>
        /// 获取根节点与系统类别菜单编号
        /// </summary>
        /// <returns></returns>
        private string GetRootAndAppSortMenuIds()
        {
            string strVal = "";

            //如果是admin,直接返回空
            if (WebUser.No == "admin") return strVal;

            string sql = "SELECT NO FROM GPM_Menu WHERE FK_App IN ('UnitFullName','AppSort')";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            foreach (DataRow row in dt.Rows)
            {
                strVal += row[0].ToString() + ",";
            }
            return strVal;
        }

        /// <summary>
        /// 二级管理员清空用户菜单对应关系表：GPM_UserMenu
        /// </summary>
        /// <param name="FK_Emp"></param>
        private void DeleteUserMenuOfSecondAdmin(string FK_Emp)
        {
            //普通管理员需要根据单个系统进行删除
            EmpApps empApps = new EmpApps();
            empApps.RetrieveByAttr(EmpAppAttr.FK_Emp, WebUser.No);
            foreach (EmpApp empApp in empApps)
            {
                DBAccess.RunSQL(string.Format("DELETE FROM GPM_UserMenu WHERE FK_Emp='{0}' AND FK_Menu IN (SELECT No FROM GPM_Menu WHERE FK_App='{1}')", FK_Emp, empApp.FK_App));
            }
            //删除根节点与系统类别
            //DBAccess.RunSQL(string.Format("DELETE FROM GPM_UserMenu WHERE FK_Emp='{0}' AND FK_Menu IN (select No from GPM_Menu where FK_App in ('UnitFullName','AppSort'))", FK_Emp));
        }

        /// <summary>
        /// 二级管理员清空岗位菜单对应关系表：GPM_StationMenu
        /// </summary>
        /// <param name="FK_Station"></param>
        private void DeleteStationMenuOfSecondAdmin(string FK_Station)
        {
            //普通管理员需要根据单个系统进行删除
            EmpApps empApps = new EmpApps();
            empApps.RetrieveByAttr(EmpAppAttr.FK_Emp, WebUser.No);
            foreach (EmpApp empApp in empApps)
            {
                DBAccess.RunSQL(string.Format("DELETE FROM GPM_StationMenu WHERE FK_Station='{0}' AND FK_Menu IN (SELECT No FROM GPM_Menu WHERE FK_App='{1}')", FK_Station, empApp.FK_App));
            }
        }

        /// <summary>
        /// 二级管理员清空权限组菜单对应关系表：GPM_GroupMenu
        /// </summary>
        /// <param name="FK_Group"></param>
        private void DeleteGroupMenuOfSecondAdmin(string FK_Group)
        {
            //普通管理员需要根据单个系统进行删除
            EmpApps empApps = new EmpApps();
            empApps.RetrieveByAttr(EmpAppAttr.FK_Emp, WebUser.No);
            foreach (EmpApp empApp in empApps)
            {
                DBAccess.RunSQL(string.Format("DELETE FROM GPM_GroupMenu WHERE FK_Group='{0}' AND FK_Menu IN (SELECT No FROM GPM_Menu WHERE FK_App='{1}')", FK_Group, empApp.FK_App));
            }
        }

        /// <summary>
        /// 获取系统日志
        /// </summary>
        /// <returns></returns>
        private string GetSystemLoginLogs()
        {
            string startdate = DateTime.Parse(getUTF8ToString("startdate")).ToString("yyyy-MM-dd HH:mm:ss");
            string enddate = DateTime.Parse(getUTF8ToString("enddate")).ToString("yyyy-MM-dd HH:mm:ss");

            string sql = "select OID,FK_EMP,Port_Emp.Name as EMP_Name,Port_Dept.Name as Dept_Name,GPM_App.Name as Sys_Name, LoginDateTime,IP "
                + "from Port_Emp inner join Port_Dept on Port_Emp.FK_Dept=Port_Dept.No "
                + "inner join GPM_SystemLoginLog on GPM_SystemLoginLog.FK_Emp=Port_Emp.No "
                + "and  CONVERT(DATETIME,GPM_SystemLoginLog.LoginDateTime)  between CONVERT(DATETIME,'" + startdate + "')  and  CONVERT(DATETIME,'" + enddate + "')  "
                + "inner join GPM_App on GPM_App.No=GPM_SystemLoginLog.FK_App order by CONVERT(DATETIME,LoginDateTime) desc";

            return "";

            //SystemLoginLog loginLog = new SystemLoginLog();
            //DataTable dt = loginLog.RunSQLReturnTable(sql);
            //return CommonDbOperator.GetJsonFromTable(dt);
        }

        /// <summary>
        /// 将实体类转为json格式
        /// </summary>
        /// <param name="ens"></param>
        /// <returns></returns>
        public string TranslateEntitiesToGridJsonOnlyData(BP.En.Entities ens)
        {
            Attrs attrs = ens.GetNewEntity.EnMap.Attrs;
            StringBuilder append = new StringBuilder();
            append.Append("[");

            foreach (Entity en in ens)
            {
                append.Append("{");
                foreach (Attr attr in attrs)
                {
                    //if (attr.IsRefAttr || attr.UIVisible == false)
                    //    continue;
                    string strValue = en.GetValStrByKey(attr.Key);
                    if (!string.IsNullOrEmpty(strValue) && strValue.LastIndexOf("\\") > -1)
                    {
                        strValue = strValue.Substring(0, strValue.LastIndexOf("\\"));
                    }
                    append.Append(attr.Key + ":\"" + strValue + "\",");
                }
                append = append.Remove(append.Length - 1, 1);
                append.Append("},");
            }
            if (append.Length > 1)
                append = append.Remove(append.Length - 1, 1);
            append.Append("]");
            return append.ToString();
        }

        /// <summary>
        /// 将实体类转为json格式 包含列名和数据
        /// </summary>
        /// <param name="ens"></param>
        /// <returns></returns>
        public string TranslateEntitiesToGridJsonColAndData(BP.En.Entities ens)
        {
            Attrs attrs = ens.GetNewEntity.EnMap.Attrs;
            StringBuilder append = new StringBuilder();
            append.Append("{");
            //整理列名
            append.Append("columns:[");
            foreach (Attr attr in attrs)
            {
                if (attr.IsRefAttr || attr.UIVisible == false)
                    continue;
                append.Append("{");
                append.Append(string.Format("field:'{0}',title:'{1}',width:{2}", attr.Key, attr.Desc, attr.UIWidth));
                append.Append("},");
            }
            if (append.Length > 10)
                append = append.Remove(append.Length - 1, 1);
            append.Append("]");

            //整理数据
            append.Append(",data:[");
            foreach (Entity en in ens)
            {
                append.Append("{");
                foreach (Attr attr in attrs)
                {
                    if (attr.IsRefAttr || attr.UIVisible == false)
                        continue;
                    append.Append(attr.Key + ":\"" + en.GetValStrByKey(attr.Key) + "\",");
                }
                append = append.Remove(append.Length - 1, 1);
                append.Append("},");
            }
            if (append.Length > 11)
                append = append.Remove(append.Length - 1, 1);
            append.Append("]");
            append.Append("}");
            return append.ToString();
        }

        /// <summary>
        /// 获取所有目录菜单
        /// </summary>
        /// <returns></returns>
        private string GetMenusOfMenuForEmp()
        {
            string parentNo = getUTF8ToString("parentNo");
            string isLoadChild = getUTF8ToString("isLoadChild");

            //根据父节点编号获取子节点
            BP.GPM.Menus menus = new BP.GPM.Menus();
            menus.RetrieveByAttr("ParentNo", parentNo);

            //如果是第一次加载
            if (isLoadChild == "false")
            {
                StringBuilder appSend = new StringBuilder();
                appSend.Append("[");
                foreach (EntityTree item in menus)
                {
                    if (appSend.Length > 1) appSend.Append(",{"); else appSend.Append("{");

                    appSend.Append("\"id\":\"" + item.No + "\"");
                    appSend.Append(",\"text\":\"" + item.Name + "\"");

                    BP.GPM.Menu menu = item as BP.GPM.Menu;
                    //节点图标
                    string ico = "icon-" + menu.MenuType;

                    appSend.Append(",iconCls:\"");
                    appSend.Append(ico);
                    appSend.Append("\"");
                    appSend.Append(",\"children\":");
                    appSend.Append(GetMenusByParentNo(item.No, "", "", true));
                    appSend.Append("}");
                }
                appSend.Append("]");

                return appSend.ToString();
            }
            //获取树节点数据
            return GetTreeList(menus, "", "");
        }

        //加载左侧菜单
        private string GetLeftMenu()
        {
            StringBuilder menuApp = new StringBuilder();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(SystemConfig.PathOfXML + "BPMMenu.xml");
            //得到顶层节点列表
            XmlNodeList topM = xmlDoc.DocumentElement.ChildNodes;
            menuApp.Append("[");
            foreach (XmlNode node in topM)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    if (menuApp.Length > 1) menuApp.Append(",");
                    menuApp.Append("{");
                    menuApp.Append("No:'" + node.Attributes["No"].InnerText + "'");
                    menuApp.Append(",Name:'" + node.Attributes["Name"].InnerText + "'");
                    menuApp.Append(",Img:'" + node.Attributes["Img"].InnerText + "'");
                    menuApp.Append(",Url:'" + node.Attributes["Url"].InnerText + "'");
                    menuApp.Append(",Children:[");
                    if (node.ChildNodes.Count > 0)
                    {
                        string childrenMenu = "";
                        foreach (XmlNode cNode in node.ChildNodes)
                        {
                            if (cNode.NodeType == XmlNodeType.Element)
                            {
                                if (childrenMenu.Length > 0) childrenMenu += ",";
                                childrenMenu += "{";
                                childrenMenu += "No:'" + cNode.Attributes["No"].InnerText + "'";
                                childrenMenu += ",Name:'" + cNode.Attributes["Name"].InnerText + "'";
                                childrenMenu += ",Img:'" + cNode.Attributes["Img"].InnerText + "'";
                                childrenMenu += ",Url:'" + cNode.Attributes["Url"].InnerText + "'";
                                childrenMenu += ",Children:[]";
                                childrenMenu += "}";
                            }
                        }
                        menuApp.Append(childrenMenu);
                    }
                    menuApp.Append("]}");
                }
            }
            menuApp.Append("]");
            return menuApp.ToString();
        }

        /// <summary>
        /// 得到菜单
        /// </summary>
        public string getMenu()
        {
            StringBuilder sbMenu = new StringBuilder();
            string appName = getUTF8ToString("appname");
            string no = getUTF8ToString("no");

            sbMenu.Append("[");
            string sql = "select * from GPM_Menu where  fk_app='" + appName + "' and  No='" + no + "'";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    sbMenu.Append("{");
                    foreach (DataColumn dc in dt.Rows[0].Table.Columns)
                    {
                        if (dc.Ordinal > 0) sbMenu.Append(",");
                        sbMenu.AppendFormat(dc.ColumnName + ":\"{0}\" ", dt.Rows[0][dc.ColumnName].ToString());
                    }
                    sbMenu.AppendFormat(",iconCls:\"{0}\" ", "icon-" + dt.Rows[0]["MenuType"].ToString());
                    sbMenu.Append(",children:[");
                    sbMenu.Append(GetChildMenu(appName, no));
                    sbMenu.Append("]}");
                }
            }
            sbMenu.Append("]");

            return sbMenu.ToString();
        }

        /// <summary>
        /// 迭代获取菜单
        /// </summary>
        /// <returns></returns>
        public string GetChildMenu(string appName, string nodeno)
        {
            StringBuilder sbContent = new StringBuilder("");
            string sql = "select * from GPM_Menu  where  fk_app='" + appName + "' and  ParentNo='" + nodeno + "' ORDER BY Idx";

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (dt != null)
            {
                int i = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    i++;
                    sbContent.Append("{");
                    foreach (DataColumn dc in dt.Rows[0].Table.Columns)
                    {
                        if (dc.Ordinal > 0) sbContent.Append(",");
                        sbContent.AppendFormat(dc.ColumnName + ":\"{0}\" ", dr[dc.ColumnName].ToString());
                    }
                    sbContent.AppendFormat(",iconCls:\"{0}\", ", "icon-" + dr["MenuType"].ToString());
                    sbContent.Append("children:[");
                    sbContent.Append(GetChildMenu(appName, dr["No"].ToString()));
                    if (i == dt.Rows.Count)
                    {
                        sbContent.Append("]}");
                    }
                    else
                    {
                        sbContent.Append("]},");
                    }

                }
                return sbContent.ToString();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 根据系统名称获取菜单
        /// </summary>
        /// <returns></returns>
        private string GetSystemMenus()
        {
            string strMenus = "[]";
            string appName = getUTF8ToString("appName");

            BP.GPM.App app = new BP.GPM.App(appName);
            BP.GPM.Menu sysMenu = new BP.GPM.Menu();
            DataTable dt = sysMenu.RunSQLReturnTable("select * from GPM_Menu WHERE FK_App='" + appName + "' ORDER BY Idx");
            strMenus = CommonDbOperator.GetGridTreeDataString(dt, "ParentNo", "No", app.RefMenuNo, true);

            if (strMenus.Length > 2)
                strMenus = strMenus.Remove(strMenus.Length - 2, 2);
            else
            {
                sysMenu = new BP.GPM.Menu();
                sysMenu.No = BP.DA.DBAccess.GenerOID("BP.GPM.Menu").ToString();
                sysMenu.Name = "新建目录";
                sysMenu.ParentNo = app.RefMenuNo;
                sysMenu.FK_App = appName;
                sysMenu.MenuType = 3;
                sysMenu.IsDir = true;
                sysMenu.Insert();
                //重新查询
                dt = sysMenu.RunSQLReturnTable("select * from GPM_Menu WHERE FK_App='" + appName + "' ORDER BY Idx");
                strMenus = CommonDbOperator.GetGridTreeDataString(dt, "ParentNo", "No", app.RefMenuNo, true);

                if (strMenus.Length > 2)
                    strMenus = strMenus.Remove(strMenus.Length - 2, 2);
            }
            return strMenus;
        }

        /// <summary>
        /// 根据编号获取子菜单
        /// </summary>
        /// <returns></returns>
        private string GetMenusById()
        {
            string strMenus = "[]";
            string id = getUTF8ToString("Id");
            BP.GPM.Menu sysMenu = new BP.GPM.Menu();
            DataTable dt = sysMenu.RunSQLReturnTable("select * from GPM_Menu WHERE ParentNo='" + id + "' ORDER BY Idx");
            strMenus = CommonDbOperator.GetGridTreeDataString(dt, "ParentNo", "No", id, true);

            if (strMenus.Length > 2)
                strMenus = strMenus.Remove(strMenus.Length - 2, 2);
            return strMenus;
        }

        /// <summary>
        /// 获取部门和用户
        /// </summary>
        /// <returns></returns>
        private string GetDeptEmpTree()
        {
            StringBuilder deptEmp = new StringBuilder();
            //部门信息
            Dept dept = new Dept();
            dept.Retrieve("ParentNo", "0");
            deptEmp.Append("[");
            if (dept != null)
            {
                deptEmp.Append("{");
                deptEmp.Append("\"id\":\"" + dept.No + "\",\"text\":\"" + dept.Name +
                    "\",\"iconCls\":\"icon-tree_folder\",\"attributes\":{\"isEmp\":\"no\"},\"state\":\"open\"");
                DataTable dt_dept = dept.RunSQLReturnTable("select * from Port_Dept where ParentNo='" + dept.No + "' order by Idx");
                DataTable dt_emp = dept.RunSQLReturnTable("select distinct b.No,b.Name from Port_DeptEmp a,Port_Emp b where a.FK_Dept = b.FK_Dept and a.FK_Dept = '" + dept.No + "'");
                deptEmp.Append(",\"children\":");
                deptEmp.Append("[");
                //绑定部门
                if (dt_dept != null && dt_dept.Rows.Count > 0)
                {
                    foreach (DataRow row in dt_dept.Rows)
                    {
                        deptEmp.Append("{");
                        deptEmp.Append("\"id\":\"" + row["No"].ToString() + "\",\"text\":\"" + row["Name"].ToString() +
                            "\",\"iconCls\":\"icon-tree_folder\",\"state\":\"closed\",\"attributes\":{\"isEmp\":\"no\"}");
                        deptEmp.Append(",\"children\":");
                        deptEmp.Append("[{");
                        deptEmp.Append("\"id\":\"" + row["No"].ToString() + "01" + "\",\"text\":\"加载中...\",\"attributes\":{\"isEmp\":\"no\"}");
                        deptEmp.Append("}]");
                        deptEmp.Append("},");
                    }
                    deptEmp = deptEmp.Remove(deptEmp.Length - 1, 1);
                }
                //绑定人员
                if (dt_emp != null && dt_emp.Rows.Count > 0)
                {
                    foreach (DataRow empRow in dt_emp.Rows)
                    {
                        deptEmp.Append(",{");
                        deptEmp.Append("\"id\":\"" + empRow["No"].ToString() + "\",\"text\":\"" + empRow["Name"].ToString() +
                            "\",\"iconCls\":\"icon-person\",\"attributes\":{\"isEmp\":\"yes\"}");
                        deptEmp.Append("}");
                    }
                }
                deptEmp.Append("]");
                deptEmp.Append("}");
            }
            deptEmp.Append("]");

            return deptEmp.ToString();
        }

        /// <summary>
        /// 获取部门用户子节点
        /// </summary>
        /// <returns></returns>
        private string GetDeptEmpChildNodes()
        {
            string nodeNo = getUTF8ToString("nodeNo");

            StringBuilder deptEmp = new StringBuilder();
            Dept dept = new Dept();
            DataTable dt_dept = dept.RunSQLReturnTable("select * from Port_Dept where ParentNo='" + nodeNo + "' order by Idx");
            DataTable dt_emp = dept.RunSQLReturnTable("select distinct b.No,b.Name from Port_DeptEmp a,Port_Emp b where a.FK_Dept = b.FK_Dept and a.FK_Dept = '" + nodeNo + "'");

            deptEmp.Append("[");
            //绑定部门
            if (dt_dept != null && dt_dept.Rows.Count > 0)
            {
                foreach (DataRow row in dt_dept.Rows)
                {
                    deptEmp.Append("{");
                    deptEmp.Append(string.Format("\"id\":\"{0}\",\"text\":\"{1}\",\"iconCls\":\"icon-tree_folder\",\"state\":\"closed\"", row["No"].ToString(), row["Name"].ToString()) + ",\"attributes\":{\"isEmp\":\"no\"}");
                    deptEmp.Append(",\"children\":");
                    deptEmp.Append("[{");
                    deptEmp.Append(string.Format("\"id\":\"{0}\",\"text\":\"{1}\"", row["No"].ToString() + "01", "加载中...") + ",\"attributes\":{\"isEmp\":\"no\"}");
                    deptEmp.Append("}]");
                    deptEmp.Append("},");
                }
                deptEmp = deptEmp.Remove(deptEmp.Length - 1, 1);
            }
            //绑定人员
            if (dt_emp != null && dt_emp.Rows.Count > 0)
            {
                foreach (DataRow empRow in dt_emp.Rows)
                {
                    if (deptEmp.Length == 1)
                        deptEmp.Append("{");
                    else
                        deptEmp.Append(",{");
                    deptEmp.Append(string.Format("\"id\":\"{0}\",\"text\":\"{1}\",\"iconCls\":\"icon-person\"", empRow["No"].ToString(),
                        empRow["Name"].ToString()) + ",\"attributes\":{\"isEmp\":\"yes\"}");
                    deptEmp.Append("}");
                }
            }
            deptEmp.Append("]");

            return deptEmp.ToString();
        }

        /// <summary>
        /// 菜单管理
        /// </summary>
        /// <returns></returns>
        private string MenuNodeManage()
        {
            string nodeNo = getUTF8ToString("nodeNo");
            string dowhat = getUTF8ToString("dowhat");
            string returnVal = "";
            BP.GPM.Menu sysMenu = new BP.GPM.Menu(nodeNo);
            switch (dowhat.ToLower())
            {
                case "sample"://新增同级节点                    
                    nodeNo = sysMenu.DoCreateSameLevelNode().No;
                    //新节点赋值
                    BP.GPM.Menu newMenu = new BP.GPM.Menu(nodeNo); ;
                    newMenu.FK_App = sysMenu.FK_App;
                    newMenu.Update();
                    returnVal = newMenu.No;
                    break;
                case "children"://新增子节点
                    nodeNo = sysMenu.DoCreateSubNode().No;
                    //新节点赋值
                    BP.GPM.Menu newcMenu = new BP.GPM.Menu(nodeNo);
                    newcMenu.FK_App = sysMenu.FK_App;
                    newcMenu.Update();
                    returnVal = newcMenu.No;
                    break;
                case "doup"://上移
                    sysMenu.DoUp();
                    break;
                case "dodown"://下移
                    sysMenu.DoDown();
                    break;
                case "delete"://删除
                    sysMenu.Delete();
                    break;
            }
            //返回
            return returnVal;
        }

        /// <summary>
        /// 根据用户编号获取菜单
        /// </summary>
        /// <returns></returns>
        private string GetMenuByEmpNo()
        {
            string fk_emp = getUTF8ToString("fk_emp");
            string fk_app = getUTF8ToString("fk_app");
            // 获取菜单，并把它展现出来.
            DataTable dt = BP.GPM.Dev2Interface.DB_Menus(fk_emp, fk_app);
            return CommonDbOperator.GetJsonFromTable(dt);
        }

        /// <summary>
        /// 获取所有菜单，根据用户权限设置所选项
        /// </summary>
        /// <returns></returns>
        private string GetEmpOfMenusByEmpNo()
        {
            string checkedMenuIds = "";
            string empNO = getUTF8ToString("empNo");
            string parentNo = getUTF8ToString("parentNo");
            string isLoadChild = getUTF8ToString("isLoadChild");
            //根据用户编号获取菜单
            //UserMenus userMenus = new UserMenus();

            //QueryObject objWhere = new QueryObject(userMenus);
            //objWhere.AddWhere(UserMenuAttr.FK_Emp, empNO);
            //objWhere.addAnd();
            //objWhere.AddWhere(UserMenuAttr.IsChecked, true);
            //objWhere.DoQuery();
            UserMenu userMenu = new UserMenu();
            DataTable userMenu_dt = userMenu.RunSQLReturnTable("SELECT FK_Menu FROM V_GPM_EmpMenu_GPM WHERE FK_Emp='" + empNO + "' AND IsChecked=1");

            //根据父节点编号获取子节点
            BP.GPM.Menus menus = new BP.GPM.Menus();
            menus.RetrieveByAttr("ParentNo", parentNo);

            //整理选中项
            foreach (DataRow row in userMenu_dt.Rows)
            {
                checkedMenuIds += "," + row["FK_Menu"].ToString() + ",";
            }
            //整理未完全选中
            string unCheckedMenuIds = "";
            DataTable unCheck_dt = userMenu.RunSQLReturnTable("SELECT FK_Menu FROM V_GPM_EmpMenu_GPM WHERE FK_Emp='" + empNO + "' AND IsChecked=0");
            foreach (DataRow unItem in unCheck_dt.Rows)
            {
                unCheckedMenuIds += "," + unItem["FK_Menu"] + ",";
            }
            //如果是第一次加载
            if (isLoadChild == "false")
            {
                StringBuilder appSend = new StringBuilder();
                appSend.Append("[");
                foreach (EntityTree item in menus)
                {
                    if (appSend.Length > 1) appSend.Append(",{"); else appSend.Append("{");

                    appSend.Append("\"id\":\"" + item.No + "\"");
                    appSend.Append(",\"text\":\"" + item.Name + "\"");

                    BP.GPM.Menu menu = item as BP.GPM.Menu;

                    //节点图标
                    string ico = "icon-" + menu.MenuType;
                    //判断未完全选中
                    if (unCheckedMenuIds.Contains("," + item.No + ","))
                        ico = "collaboration";

                    appSend.Append(",iconCls:\"");
                    appSend.Append(ico);
                    appSend.Append("\"");

                    //判断选中
                    if (checkedMenuIds.Contains("," + item.No + ","))
                        appSend.Append(",\"checked\":true");

                    appSend.Append(",\"children\":");
                    appSend.Append(GetMenusByParentNo(item.No, checkedMenuIds, unCheckedMenuIds, true));
                    appSend.Append("}");
                }
                appSend.Append("]");

                return appSend.ToString();
            }
            //获取树节点数据
            return GetTreeList(menus, checkedMenuIds, unCheckedMenuIds);
        }

        /// <summary>
        /// 根据父节点编号获取子菜单
        /// </summary>
        /// <returns></returns>
        private string GetMenusByParentNo(string parentNo, string checkedMenuIds, string unCheckedMenuIds, bool addChild)
        {
            StringBuilder menuAppend = new StringBuilder();
            //获取菜单
            BP.GPM.Menus menus = new BP.GPM.Menus();
            menus.RetrieveByAttr("ParentNo", parentNo);

            //是否添加下一级
            if (addChild == true)
            {
                menuAppend.Append("[");
                foreach (EntityTree item in menus)
                {
                    BP.GPM.Menu menu = item as BP.GPM.Menu;
                    if (HaveRightApps != "admin" && !HaveRightApps.Contains("," + menu.FK_App + ","))
                        continue;

                    if (menuAppend.Length > 1) menuAppend.Append(",{"); else menuAppend.Append("{");

                    menuAppend.Append("\"id\":\"" + item.No + "\"");
                    menuAppend.Append(",\"text\":\"" + item.Name + "\"");

                    //节点图标
                    string ico = "icon-" + menu.MenuType;
                    //判断未完全选中
                    if (unCheckedMenuIds.Contains("," + item.No + ","))
                        ico = "collaboration";

                    menuAppend.Append(",iconCls:\"");
                    menuAppend.Append(ico);
                    menuAppend.Append("\"");

                    //判断选中
                    if (checkedMenuIds.Contains("," + item.No + ","))
                        menuAppend.Append(",\"checked\":true");

                    menuAppend.Append(",\"children\":");
                    menuAppend.Append(GetMenusByParentNo(item.No, checkedMenuIds, unCheckedMenuIds, false));
                    menuAppend.Append("}");
                }
                menuAppend.Append("]");

                return menuAppend.ToString();
            }
            return GetTreeList(menus, checkedMenuIds, unCheckedMenuIds);
        }

        /// <summary>
        /// 获取树节点列表
        /// </summary>
        /// <param name="ens"></param>
        /// <param name="checkIds"></param>
        /// <returns></returns>
        public string GetTreeList(Entities ens, string checkIds, string unCheckIds)
        {
            StringBuilder appSend = new StringBuilder();
            appSend.Append("[");
            foreach (EntityTree item in ens)
            {
                BP.GPM.Menu menu = item as BP.GPM.Menu;
                if (HaveRightApps != "admin" && !HaveRightApps.Contains("," + menu.FK_App + ","))
                    continue;

                if (appSend.Length > 1) appSend.Append(",{"); else appSend.Append("{");

                appSend.Append("\"id\":\"" + item.No + "\"");
                appSend.Append(",\"text\":\"" + item.Name + "\"");

                //节点图标
                string ico = "icon-" + menu.MenuType;
                //判断未完全选中
                if (unCheckIds.Contains("," + item.No + ","))
                    ico = "collaboration";

                appSend.Append(",iconCls:\"");
                appSend.Append(ico);
                appSend.Append("\"");

                if (checkIds.Contains("," + item.No + ","))
                    appSend.Append(",\"checked\":true");

                //判断是否还有子节点
                BP.GPM.Menus menus = new BP.GPM.Menus();
                menus.RetrieveByAttr("ParentNo", item.No);

                if (menus != null && menus.Count > 0)
                {
                    appSend.Append(",state:\"closed\"");
                    appSend.Append(",\"children\":");
                    appSend.Append("[{");
                    appSend.Append(string.Format("\"id\":\"{0}\",\"text\":\"{1}\"", item.No + "01", "加载中..."));
                    appSend.Append("}]");
                }
                appSend.Append("}");
            }
            appSend.Append("]");

            return appSend.ToString();
        }

        /// <summary>
        /// 将实体转为树形
        /// </summary>
        /// <param name="ens"></param>
        /// <param name="rootNo"></param>
        /// <param name="checkIds"></param>
        StringBuilder appendMenus = new StringBuilder();
        StringBuilder appendMenuSb = new StringBuilder();
        public void TansEntitiesToGenerTree(Entities ens, string rootNo, string checkIds)
        {
            EntityTree root = ens.GetEntityByKey(EntityTreeAttr.ParentNo, rootNo) as EntityTree;
            if (root == null)
                throw new Exception("@没有找到rootNo=" + rootNo + "的entity.");
            appendMenus.Append("[{");
            appendMenus.Append("\"id\":\"" + rootNo + "\"");
            appendMenus.Append(",\"text\":\"" + root.Name + "\"");
            appendMenus.Append(IsPermissionsNodesByStation(rootNo));

            BP.GPM.Menu menu = root as BP.GPM.Menu;
            //添加图标
            //appendMenus.Append(",iconCls:\"");
            //appendMenus.Append("icon-" + menu.MenuType);
            //appendMenus.Append("\"");

            // 增加它的子级.
            appendMenus.Append(",\"children\":");
            AddChildren(menu, ens, checkIds);
            appendMenus.Append(appendMenuSb);
            appendMenus.Append("}]");
        }

        public void AddChildren(EntityTree parentEn, Entities ens, string checkIds)
        {
            appendMenus.Append(appendMenuSb);
            appendMenuSb.Clear();

            appendMenuSb.Append("[");
            foreach (EntityTree item in ens)
            {
                if (item.ParentNo != parentEn.No)
                    continue;

                if (checkIds.Contains("," + item.No + ","))
                    appendMenuSb.Append("{\"id\":\"" + item.No + "\",\"text\":\"" + item.Name + "\",\"checked\":true");
                else
                    appendMenuSb.Append("{\"id\":\"" + item.No + "\",\"text\":\"" + item.Name + "\",\"checked\":false");

                DeptManagers dms = new DeptManagers();
                dms.RetrieveAll();
                appendMenuSb.Append(IsPermissionsNodesByStation(item.No));

                BP.GPM.Menu menu = item as BP.GPM.Menu;
                if (menu != null)
                {
                    //添加图标
                    //appendMenuSb.Append(",iconCls:\"");
                    //appendMenuSb.Append("icon-" + menu.MenuType);
                    //appendMenuSb.Append("\"");
                }
                // 增加它的子级.
                appendMenuSb.Append(",\"children\":");
                AddChildren(item, ens, checkIds);
                treeResult.Append(treesb.ToString());
                treesb.Clear();
                appendMenuSb.Append("},");
            }
            if (appendMenuSb.Length > 1)
                appendMenuSb = appendMenuSb.Remove(appendMenuSb.Length - 1, 1);
            appendMenuSb.Append("]");
            appendMenus.Append(appendMenuSb);
            appendMenuSb.Clear();
        }



        /// <summary>
        /// 根据DataTable生成Json树结构
        /// </summary>
        /// <param name="tabel">数据源</param>
        /// <param name="idCol">ID列</param>
        /// <param name="txtCol">Text列</param>
        /// <param name="rela">关系字段</param>
        /// <param name="pId">父ID</param>
        ///<returns>easyui tree json格式</returns>
        StringBuilder treeResult = new StringBuilder();
        StringBuilder treesb = new StringBuilder();
        public string GetTreeJsonByTable(DataTable tabel, string idCol, string txtCol, string rela, object pId)
        {
            string treeJson = string.Empty;
            string treeState = "close";
            treeResult.Append(treesb.ToString());

            treesb.Clear();
            if (treeResult.Length == 0)
            {
                treeState = "open";
            }
            if (tabel.Rows.Count > 0)
            {
                treesb.Append("[");
                string filer = string.Empty;
                if (pId.ToString() == "")
                {
                    filer = string.Format("{0} is null", rela);
                }
                else
                {
                    filer = string.Format("{0}='{1}'", rela, pId);
                }
                DataRow[] rows = tabel.Select(filer);
                if (rows.Length > 0)
                {
                    foreach (DataRow row in rows)
                    {
                        treesb.Append("{\"id\":\"" + row[idCol]
                                + "\",\"text\":\"" + row[txtCol]
                                + "\",\"state\":\"" + treeState + "\"");

                        //更换节点图标
                        if (tabel.Columns.Contains("MenuType"))
                        {
                            //目录级别不展开
                            if (row["MenuType"].ToString() == "3")
                            {
                                treesb.Append(",state:\"closed\"");
                            }
                            treesb.Append(",iconCls:\"");
                            treesb.Append("icon-" + row["MenuType"].ToString());
                            treesb.Append("\"");
                        }

                        if (tabel.Select(string.Format("{0}='{1}'", rela, row[idCol])).Length > 0)
                        {
                            treesb.Append(",\"children\":");
                            GetTreeJsonByTable(tabel, idCol, txtCol, rela, row[idCol]);
                            treeResult.Append(treesb.ToString());
                            treesb.Clear();
                        }
                        treeResult.Append(treesb.ToString());
                        treesb.Clear();
                        treesb.Append("},");
                    }
                    treesb = treesb.Remove(treesb.Length - 1, 1);
                }
                treesb.Append("]");
                treeResult.Append(treesb.ToString());
                treeJson = treeResult.ToString();
                treesb.Clear();
            }
            return treeJson;
        }
    }
}