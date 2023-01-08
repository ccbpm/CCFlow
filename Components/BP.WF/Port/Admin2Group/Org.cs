using BP.DA;
using BP.En;
using BP.Web;
using BP.WF.Template;
using System.Data;

namespace BP.WF.Port.Admin2Group
{
    /// <summary>
    /// 独立组织属性
    /// </summary>
    public class OrgAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 管理员帐号
        /// </summary>
        public const string Adminer = "Adminer";
        /// <summary>
        /// 管理员名称
        /// </summary>
        public const string AdminerName = "AdminerName";
        /// <summary>
        /// 父级组织编号
        /// </summary>
        public const string ParentNo = "ParentNo";
        /// <summary>
        /// 父级组织名称
        /// </summary>
        public const string ParentName = "ParentName";
        /// <summary>
        /// 序号
        /// </summary>
        public const string Idx = "Idx";
    }
    /// <summary>
    /// 独立组织
    /// </summary>
    public class Org : EntityNoName
    {
        #region 属性
        /// <summary>
        /// 父节点编号
        /// </summary>
        public string Adminer
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.Adminer);
            }
            set
            {
                this.SetValByKey(OrgAttr.Adminer, value);
            }
        }
        /// <summary>
        /// 管理员名称
        /// </summary>
        public string AdminerName
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.AdminerName);
            }
            set
            {
                this.SetValByKey(OrgAttr.AdminerName, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 独立组织
        /// </summary>
        public Org() { }
        /// <summary>
        /// 独立组织
        /// </summary>
        /// <param name="no">编号</param>
        public Org(string no) : base(no) { }
        #endregion

        #region 重写方法
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                uac.IsDelete = false;
                uac.IsInsert = false;
                return uac;
            }
        }
        /// <summary>
        /// Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Port_Org", "独立组织");
                // map.EnType = EnType.View; //独立组织是一个视图.

                map.AddTBStringPK(OrgAttr.No, null, "编号(与部门编号相同)", true, false, 1, 30, 40);
                map.AddTBString(OrgAttr.Name, null, "组织名称", true, false, 0, 60, 200, true);

                map.AddTBString(OrgAttr.Adminer, null, "主要管理员(创始人)", true, true, 0, 60, 200, true);
                map.AddTBString(OrgAttr.AdminerName, null, "管理员名称", true, true, 0, 60, 200, true);

                map.AddTBInt("FlowNums", 0, "流程数", true, true);
                map.AddTBInt("FrmNums", 0, "表单数", true, true);
                map.AddTBInt("Users", 0, "用户数", true, true);
                map.AddTBInt("Depts", 0, "部门数", true, true);

                map.AddTBInt("GWFS", 0, "运行中流程", true, true);
                map.AddTBInt("GWFSOver", 0, "结束的流程", true, true);
                map.AddTBInt(OrgAttr.Idx, 0, "排序", false, false);

                RefMethod rm = new RefMethod();

                //rm = new RefMethod();
                //rm.Title = "组织结构";
                //rm.ClassMethodName = this.ToString() + ".DoOrganization";
                //rm.Icon = "icon-organization";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "人员台账";
                //rm.ClassMethodName = this.ToString() + ".DoEmps";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //map.AddRefMethod(rm);


                //rm = new RefMethod();
                //rm.Title = "角色类型";
                //rm.ClassMethodName = this.ToString() + ".DoStationTypes";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "角色";
                //rm.ClassMethodName = this.ToString() + ".DoStations";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "清空菜单权限缓存";
                //rm.ClassMethodName = this.ToString() + ".AddClearUserRegedit";
                //rm.RefMethodType = RefMethodType.Func;
                //map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "检查正确性";
                rm.ClassMethodName = this.ToString() + ".DoCheck";
                rm.Icon = "icon-check";
                //rm.HisAttrs.AddTBString("No", null, "子公司管理员编号", true, false, 0, 100, 100);
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "增加管理员";
                rm.Icon = "icon-user";
                rm.ClassMethodName = this.ToString() + ".AddAdminer";
                rm.HisAttrs.AddTBString("adminer", null, "管理员编号", true, false, 0, 100, 100);
                map.AddRefMethod(rm);
                //管理员.
                map.AddDtl(new OrgAdminers(), OrgAdminerAttr.OrgNo, null, DtlEditerModel.DtlSearch, "icon-people");

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        public string DoOrganization()
        {
            return "/GPM/Organization.htm";
        }
        public string DoEmps()
        {
            return "/WF/Comm/Search.htm?EnsName=BP.Port.Emps";
        }

        public string DoStationTypes()
        {
            return "/WF/Comm/Ens.htm?EnsName=BP.Port.StationTypes";
        }
        public string DoStations()
        {
            return "/WF/Comm/Search.htm?EnsName=BP.Port.Stations";
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <returns></returns>
        public string AddClearUserRegedit()
        {
            DBAccess.RunSQL("DELETE FROM Sys_UserRegedit WHERE OrgNo='" + this.No + "' AND CfgKey='Menus'");
            return "执行成功.";
        }

        protected override bool beforeUpdateInsertAction()
        {
            this.SetValByKey("FlowNums", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) AS a FROM WF_Flow WHERE OrgNo='" + this.No + "'"));
            this.SetValByKey("FrmNums", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) AS a FROM Sys_MapData WHERE OrgNo='" + this.No + "'"));

            this.SetValByKey("Users", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) AS a FROM Port_Emp WHERE OrgNo='" + this.No + "'"));
            this.SetValByKey("Depts", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) AS a FROM Port_Dept WHERE OrgNo='" + this.No + "'"));
            this.SetValByKey("GWFS", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) AS a FROM WF_GenerWorkFlow WHERE OrgNo='" + this.No + "' AND WFState!=3"));
            this.SetValByKey("GWFSOver", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) AS a FROM WF_GenerWorkFlow WHERE OrgNo='" + this.No + "' AND WFState=3"));
            return base.beforeUpdateInsertAction();
        }

        public string AddAdminer(string adminer)
        {

            BP.Port.Emp emp = new BP.Port.Emp();
            emp.No = adminer;
            if (emp.RetrieveFromDBSources() == 0)
                return "err@管理员编号错误.";

            //检查超级管理员是否存在？
            OrgAdminer oa = new OrgAdminer();
            oa.FK_Emp = adminer;
            oa.OrgNo = this.No;
            oa.MyPK = this.No + "_" + oa.FK_Emp;
            if (oa.RetrieveFromDBSources() == 1)
                return "err@管理员已经存在.";

            oa.Delete(OrgAdminerAttr.FK_Emp, adminer, OrgAdminerAttr.OrgNo, this.No);

            //插入到管理员.
            oa.FK_Emp = emp.UserID;
            oa.Save();

            //如果不在同一个组织.就给他一个兼职部门.
            BP.Port.DeptEmps depts = new BP.Port.DeptEmps();
            depts.Retrieve("OrgNo", this.No, "FK_Emp", adminer);
            if (depts.Count == 0)
            {
                BP.Port.DeptEmp de = new BP.Port.DeptEmp();
                de.FK_Dept = this.No;
                de.FK_Emp = adminer;
                de.MyPK = this.No + "_" + adminer;
                de.OrgNo = this.No;
                de.Save();
            }
            

            //检查超级管理员是否存在？
            return "管理员增加成功,请关闭当前记录重新打开,请给管理员[" + emp.No + "," + emp.Name + "]分配权限";
        }
        private void SetOrgNo(string deptNo)
        {
            DBAccess.RunSQL("UPDATE Port_Emp SET OrgNo='" + this.No + "' WHERE FK_Dept='" + deptNo + "'");
            DBAccess.RunSQL("UPDATE Port_DeptEmp SET OrgNo='" + this.No + "' WHERE FK_Dept='" + deptNo + "'");
            DBAccess.RunSQL("UPDATE Port_DeptEmpStation SET OrgNo='" + this.No + "' WHERE FK_Dept='" + deptNo + "'");

            Depts depts = new Depts();
            depts.Retrieve(DeptAttr.ParentNo, deptNo);
            string sql = "";
            foreach (Dept item in depts)
            {
                //如果部门下组织不能检查更新
                sql = "SELECT COUNT(*) From Port_Org Where No='" + item.No + "'";
                if (DBAccess.RunSQLReturnValInt(sql) == 1)
                    continue;

                DBAccess.RunSQL("UPDATE Port_Emp SET OrgNo='" + this.No + "' WHERE FK_Dept='" + item.No + "'");
                DBAccess.RunSQL("UPDATE Port_DeptEmp SET OrgNo='" + this.No + "' WHERE FK_Dept='" + item.No + "'");
                DBAccess.RunSQL("UPDATE Port_DeptEmpStation SET OrgNo='" + this.No + "' WHERE FK_Dept='" + item.No + "'");

                if (item.OrgNo.Equals(this.No) == false)
                {
                    item.OrgNo = this.No;
                    item.Update();
                }
                //递归调用.
                SetOrgNo(item.No);
            }
        }

        public string DoCheck()
        {
            string err = "";

            #region 组织结构信息检查.
            //检查orgNo的部门是否存在？
            Dept dept = new Dept();
            dept.No = this.No;
            if (dept.RetrieveFromDBSources() == 0)
                return "err@部门组织结构树上缺少[" + this.No + "]的部门.";

            if (this.Name.Equals(dept.Name) == false)
            {
                this.Name = dept.Name;
                err += "info@部门名称与组织名称已经同步.";
            }
            this.Update(); //执行更新.

            //设置子集部门，的OrgNo.
            if (DBAccess.IsView("Port_Dept") == false)
                this.SetOrgNo(this.No);
            #endregion 组织结构信息检查.

            #region 检查流程树.
            BP.WF.Template.FlowSort fs = new WF.Template.FlowSort();
            fs.No = this.No;
            if (fs.RetrieveFromDBSources() == 1)
            {
                fs.OrgNo = this.No;
                fs.Name = "流程树";
                fs.DirectUpdate();
            }
            else
            {
                //获得根目录节点.
                BP.WF.Template.FlowSort root = new Template.FlowSort();
                int i = root.Retrieve(BP.WF.Template.FlowSortAttr.ParentNo, "0");

                //设置流程树权限.
                fs.No = this.No;
                fs.Name = this.Name;
                fs.Name = "流程树";
                fs.ParentNo = root.No;
                fs.OrgNo = this.No;
                fs.Idx = 999;
                fs.DirectInsert();

                //创建下一级目录.
                BP.WF.Template.FlowSort en = fs.DoCreateSubNode() as BP.WF.Template.FlowSort;

                en.Name = "发文流程";
                en.OrgNo = this.No;
                en.Domain = "FaWen";
                en.DirectUpdate();

                en = fs.DoCreateSubNode() as BP.WF.Template.FlowSort;
                en.Name = "收文流程";
                en.OrgNo = this.No;
                en.Domain = "ShouWen";
                en.DirectUpdate();

                en = fs.DoCreateSubNode() as BP.WF.Template.FlowSort;
                en.Name = "业务流程";
                en.OrgNo = this.No;
                en.Domain = "Work";
                en.DirectUpdate();
                en = fs.DoCreateSubNode() as BP.WF.Template.FlowSort;
                en.Name = "会议流程";
                en.OrgNo = this.No;
                en.Domain = "Meet";
                en.DirectUpdate();
            }
            #endregion 检查流程树.

            #region 检查表单树.
            //表单根目录.
            SysFormTree ftRoot = new SysFormTree();
            int val = ftRoot.Retrieve(BP.WF.Template.FlowSortAttr.ParentNo, "0");
            if (val == 0)
            {
                val = ftRoot.Retrieve(BP.WF.Template.FlowSortAttr.No, "100");
                if (val == 0)
                {
                    ftRoot.No = "100";
                    ftRoot.Name = "表单库";
                    ftRoot.ParentNo = "0";
                    ftRoot.Insert();
                }
                else
                {
                    ftRoot.ParentNo = "0";
                    ftRoot.Name = "表单库";
                    ftRoot.Update();
                }
            }

            //设置表单树权限.
            SysFormTree ft = new SysFormTree();
            ft.No = this.No;
            if (ft.RetrieveFromDBSources() == 0)
            {
                ft.Name = this.Name;
                ft.Name = "表单树(" + this.Name + ")";
                ft.ParentNo = ftRoot.No;
                ft.OrgNo = this.No;
                ft.Idx = 999;
                ft.DirectInsert();

                //创建两个目录.
                SysFormTree mySubFT = ft.DoCreateSubNode() as SysFormTree;
                mySubFT.Name = "表单目录1";
                mySubFT.OrgNo = this.No;
                mySubFT.DirectUpdate();


                mySubFT = ft.DoCreateSubNode() as SysFormTree;
                mySubFT.Name = "表单目录2";
                mySubFT.OrgNo = this.No;
                mySubFT.DirectUpdate();
            }
            else
            {
                ft.Name = this.Name;
                ft.Name = "表单树(" + this.Name + ")"; //必须这个命名，否则找不到。
                ft.ParentNo = ftRoot.No;
                ft.OrgNo = this.No;
                ft.Idx = 999;
                ft.DirectUpdate();

                //检查数量.
                SysFormTrees frmSorts = new SysFormTrees();
                frmSorts.Retrieve("OrgNo", this.No);
                if (frmSorts.Count <= 1)
                {
                    //创建两个目录.
                    SysFormTree mySubFT = ft.DoCreateSubNode() as SysFormTree;
                    mySubFT.Name = "表单目录1";
                    mySubFT.OrgNo = this.No;
                    mySubFT.DirectUpdate();

                    mySubFT = ft.DoCreateSubNode() as SysFormTree;
                    mySubFT.Name = "表单目录2";
                    mySubFT.OrgNo = this.No;
                    mySubFT.DirectUpdate();
                }
            }
            #endregion 检查表单树.

            #region 删除无效的数据.
            string sqls = "";
            if (DBAccess.IsView("Port_DeptEmp") == false)
            {
                sqls += "@DELETE FROM Port_DeptEmp WHERE FK_Dept not in (select no from port_dept)";
                sqls += "@DELETE FROM Port_DeptEmp WHERE FK_Emp not in (select no from port_Emp)";
            }
            if (DBAccess.IsView("Port_DeptEmpStation") == false)
            {
                sqls += "@DELETE FROM Port_DeptEmpStation WHERE FK_Dept not in (select no from port_dept)";
                sqls += "@DELETE FROM Port_DeptEmpStation WHERE FK_Emp not in (select no from port_Emp)";
                sqls += "@DELETE FROM Port_DeptEmpStation WHERE FK_Station not in (select no from port_Station)";
            }
            //删除无效的管理员,
            if (DBAccess.IsView("Port_OrgAdminer") == false)
            {
                sqls += "@DELETE from Port_OrgAdminer where OrgNo not in (select No from port_dept)";
                sqls += "@DELETE from Port_OrgAdminer where FK_Emp not in (select No from port_emp)";
            }
            //删除无效的组织.
            if (DBAccess.IsView("Port_Org") == false)
                sqls += "@DELETE from Port_Org where No not in (select No from port_dept)";
            DBAccess.RunSQLs(sqls);
            #endregion 删除无效的数据.

            #region 检查人员信息.. 应该增加在整体检查的提示
            //string sql = "SELECT * FROM Port_Emp WHERE OrgNo NOT IN (SELECT No from Port_Dept )";
            //DataTable dt = DBAccess.RunSQLReturnTable(sql);
            //if (dt.Rows.Count != 0)
            //    err += " 人员表里有:" + dt.Rows.Count + "笔 组织编号有丢失. 请处理:" + sql;

            //sql = "SELECT * FROM Port_Emp WHERE FK_DEPT NOT IN (SELECT No from Port_Dept )";
            //dt = DBAccess.RunSQLReturnTable(sql);
            //if (dt.Rows.Count != 0)
            //    err += " 人员表里有:" + dt.Rows.Count + "笔数据部门编号丢失. 请处理:" + sql;
            #endregion 检查组织编号信息.

            if (DataType.IsNullOrEmpty(err) == true)
                return "系统正确";

            //检查表单树.
            return "err@" + err;
        }
    }
    /// <summary>
    ///独立组织集合
    /// </summary>
    public class Orgs : EntitiesNoName
    {
        /// <summary>
        /// 得到一个新实体
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Org();
            }
        }
        /// <summary>
        /// create ens
        /// </summary>
        public Orgs()
        {
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Org> ToJavaList()
        {
            return (System.Collections.Generic.IList<Org>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Org> Tolist()
        {
            System.Collections.Generic.List<Org> list = new System.Collections.Generic.List<Org>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Org)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
