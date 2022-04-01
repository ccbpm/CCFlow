using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web;
using BP.WF.Port.Admin2Group;

namespace BP.WF.Port.AdminGroup
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
        /// 父级组织编号
        /// </summary>
        public string ParentNo
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.ParentNo);
            }
            set
            {
                this.SetValByKey(OrgAttr.ParentNo, value);
            }
        }
        /// <summary>
        /// 父级组织名称
        /// </summary>
        public string ParentName
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.ParentName);
            }
            set
            {
                this.SetValByKey(OrgAttr.ParentName, value);
            }
        }
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
                //uac.IsDelete = false;
                //uac.IsInsert = false;
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
                map.AdjunctType = AdjunctType.None;
                // map.EnType = EnType.View; //独立组织是一个视图.

                map.AddTBStringPK(OrgAttr.No, null, "编号(与部门编号相同)", true, false, 1, 30, 40);
                map.AddTBString(OrgAttr.Name, null, "组织名称", true, false, 0, 60, 200, true);

                map.AddTBString(OrgAttr.ParentNo, null, "父级组织编号", false, false, 0, 60, 200, true);
                map.AddTBString(OrgAttr.ParentName, null, "父级组织名称", false, false, 0, 60, 200, true);

                map.AddTBString(OrgAttr.Adminer, null, "主要管理员(创始人)", true, true, 0, 60, 200, true);
                map.AddTBString(OrgAttr.AdminerName, null, "管理员名称", true, true, 0, 60, 200, true);

                map.AddTBInt("FlowNums", 0, "流程数", true, true);
                map.AddTBInt("FrmNums", 0, "表单数", true, true);
                map.AddTBInt("Users", 0, "用户数", true, true);
                map.AddTBInt("Depts", 0, "部门数", true, true);

                map.AddTBInt("GWFS", 0, "运行中流程", true, true);
                map.AddTBInt("GWFSOver", 0, "结束的流程", true, true);

                map.AddTBInt(OrgAttr.Idx, 0, "排序", true, false);

                RefMethod rm = new RefMethod();
                rm.Title = "检查正确性";
                rm.ClassMethodName = this.ToString() + ".DoCheck";
                //rm.HisAttrs.AddTBString("No", null, "子公司管理员编号", true, false, 0, 100, 100);
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "修改主管理员";
                rm.ClassMethodName = this.ToString() + ".ChangeAdminer";
                rm.HisAttrs.AddTBString("adminer", null, "新主管理员编号", true, false, 0, 100, 100);
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "取消独立组织";
                rm.ClassMethodName = this.ToString() + ".DeleteOrg";
                rm.Warning = "您确定要取消独立组织吗？系统将要删除该组织以及该组织的管理员，但是不删除部门数据.";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "在集团下新增组织";
                rm.ClassMethodName = this.ToString() + ".AddOrg";
                rm.HisAttrs.AddTBString("no", null, "组织编号", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("name", null, "组织名称", true, false, 0, 100, 100);

                rm.HisAttrs.AddTBString("adminer", null, "管理员编号", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("adminName", null, "管理员名称", true, false, 0, 100, 100);

                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "把指定的部门设置为组织";
                rm.ClassMethodName = this.ToString() + ".AddOrgByDept";
                rm.HisAttrs.AddTBString("adminer", null, "部门编号", true, false, 0, 100, 100);
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "发布菜单权限";
                rm.ClassMethodName = this.ToString() + ".AddClearUserRegedit";
                rm.RefMethodType = RefMethodType.Func;
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <returns></returns>
        public string AddClearUserRegedit()
        {
            DBAccess.RunSQL("DELETE FROM Sys_UserRegedit WHERE OrgNo='" + WebUser.OrgNo + "' AND CfgKey='Menus'");
            return "执行成功.";
        }
        /// <summary>
        /// 在集团下新增组织
        /// </summary>
        /// <param name="orgNo"></param>
        /// <param name="orgName"></param>
        /// <returns></returns>
        public string AddOrg(string orgNo, string orgName, string adminerNo, string adminerName)
        {

            Dept dept = new Dept();
            dept.No = orgNo;
            if (dept.RetrieveFromDBSources() != 0)
                return "err@部门编号已经存在.";
            //取出来根目录.
            dept.Retrieve(DeptAttr.ParentNo, "0");

            BP.GPM.Emp emp = new BP.GPM.Emp();
            emp.No = adminerNo;
            if (emp.RetrieveFromDBSources() != 0)
                return "err@管理员编号已经存在.";

            try
            {

                //增加部门信息.
                Dept mydept = new Dept();
                mydept.ParentNo = dept.No;
                mydept.No = orgNo;
                mydept.Name = orgName;
                mydept.OrgNo = orgNo;
                mydept.Insert();

                //增加组织.
                Org org = new Org();
                org.Copy(mydept);
                org.Adminer = emp.No;
                org.AdminerName = emp.Name;
                org.Insert();


                #region 创建子部门.
                mydept.No = DBAccess.GenerGUID();
                mydept.Name = "部门1";
                mydept.ParentNo = orgNo;
                mydept.Insert();

                mydept.No = DBAccess.GenerGUID();
                mydept.Name = "部门2";
                mydept.ParentNo = orgNo;
                mydept.Insert();
                #endregion 创建子部门.

                //增加人员。
                emp.Name = adminerName;
                emp.FK_Dept = orgNo;
                emp.OrgNo = orgNo;
                emp.Pass = "123";
                emp.Insert();

                //增加管理员.
                OrgAdminer oa = new OrgAdminer();
                oa.FK_Emp = emp.No;
                oa.OrgNo = org.No;
                oa.MyPK = oa.OrgNo + "_" + emp.Name;
                oa.Insert();

                //增加流程树.
                BP.WF.Template.FlowSort fs = new Template.FlowSort();
                fs.No = org.No;
                fs.ParentNo = "100";
                fs.Name = org.Name;
                fs.OrgNo = org.No;
                fs.DirectInsert();
                EntityTree en = fs.DoCreateSubNode("办公流程");
                en.SetValByKey("OrgNo", org.No);
                en.Update();

                en = fs.DoCreateSubNode("人力资源流程");
                en.SetValByKey("OrgNo", org.No);
                en.Update();

                //增加表单树.
                BP.Sys.FrmTree ft = new BP.Sys.FrmTree();
                ft.No = org.No;
                ft.ParentNo = "100";
                ft.Name = org.Name;
                ft.OrgNo = org.No;
                ft.DirectInsert();

                en = ft.DoCreateSubNode("办公表单");
                en.SetValByKey("OrgNo", org.No);
                en.Update();

                en = ft.DoCreateSubNode("人力表单");
                en.SetValByKey("OrgNo", org.No);
                en.Update();

                return "增加成功...";
            }
            catch (Exception ex)
            {
                //删除数据.
                dept.Delete("OrgNo", orgNo);
                emp.Delete("OrgNo", orgNo);

                return ex.Message;
            }
        }
        public string AddOrgByDept(string deptNo)
        {
            Dept en = new Dept();
            en.No = deptNo;
            if (en.RetrieveFromDBSources() == 0)
                return "err@";

            return "ok.";
        }

        protected override bool beforeUpdateInsertAction()
        {

            this.SetValByKey("FlowNums", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) AS a FROM WF_Flow WHERE OrgNo='" + WebUser.OrgNo + "'"));
            this.SetValByKey("FrmNums", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) AS a FROM Sys_MapData WHERE OrgNo='" + WebUser.OrgNo + "'"));

            this.SetValByKey("Users", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) AS a FROM Port_Emp WHERE OrgNo='" + WebUser.OrgNo + "'"));
            this.SetValByKey("Depts", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) AS a FROM Port_Dept WHERE OrgNo='" + WebUser.OrgNo + "'"));
            this.SetValByKey("GWFS", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) AS a FROM WF_GenerWorkFlow WHERE OrgNo='" + WebUser.OrgNo + "' AND WFState!=3"));
            this.SetValByKey("GWFSOver", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) AS a FROM WF_GenerWorkFlow WHERE OrgNo='" + WebUser.OrgNo + "' AND WFState=3"));

            //map.AddTBInt("FlowNums", 0, "流程数", true, true);
            //map.AddTBInt("FrmNums", 0, "表单数", true, true);
            //map.AddTBInt("Users", 0, "用户数", true, true);
            //map.AddTBInt("Depts", 0, "部门数", true, true);
            //map.AddTBInt("GWFS", 0, "运行中流程", true, true);
            //map.AddTBInt("GWFSOver", 0, "结束的流程", true, true);

            return base.beforeUpdateInsertAction();
        }

        public string DeleteOrg()
        {
            if (WebUser.No.Equals("admin") == false)
                return "err@只有admin帐号才可以执行。";

            //流程类别.
            BP.WF.Template.FlowSorts fss = new Template.FlowSorts();
            fss.Retrieve(OrgAdminerAttr.OrgNo, this.No);
            foreach (BP.WF.Template.FlowSort en in fss)
            {
                Flows fls = new Flows();
                fls.Retrieve(BP.WF.Template.FlowAttr.FK_FlowSort, en.No);

                if (fls.Count != 0)
                    return "err@在流程目录：" + en.Name + "有[" + fls.Count + "]个流程没有删除。";
            }

            //表单类别.
            BP.Sys.FrmTrees ftTrees = new BP.Sys.FrmTrees();
            ftTrees.Retrieve(BP.Sys.FrmTreeAttr.OrgNo, this.No);
            foreach (BP.WF.Template.FlowSort en in fss)
            {
                BP.Sys.MapDatas mds = new BP.Sys.MapDatas();
                mds.Retrieve(BP.Sys.MapDataAttr.FK_FormTree, en.No);

                if (mds.Count != 0)
                    return "err@在表单目录：" + en.Name + "有[" + mds.Count + "]个表单没有删除。";
            }

            OrgAdminers oas = new OrgAdminers();
            oas.Delete(OrgAdminerAttr.OrgNo, this.No);

            BP.WF.Template.FlowSorts fs = new Template.FlowSorts();
            fs.Delete(OrgAdminerAttr.OrgNo, this.No);

            fss.Delete(OrgAdminerAttr.OrgNo, this.No); //删除流程目录.
            ftTrees.Delete(BP.Sys.FrmTreeAttr.OrgNo, this.No); //删除表单目录。

            this.Delete();
            return "info@成功注销组织,请关闭窗口刷新页面.";
        }

        public string ChangeAdminer(string adminer)
        {
            if (WebUser.No.Equals("admin") == false)
                return "err@非admin管理员，您无法执行该操作.";

            BP.Port.Emp emp = new BP.Port.Emp();
            emp.UserID = adminer;
            if (emp.RetrieveFromDBSources() == 0)
                return "err@管理员编号错误.";

            string old = this.Adminer;

            this.Adminer = emp.UserID;
            this.AdminerName = emp.Name;
            this.Update();

            //检查超级管理员是否存在？
            OrgAdminer oa = new OrgAdminer();
            oa.FK_Emp = old;
            oa.OrgNo = this.No;
            oa.Delete(OrgAdminerAttr.FK_Emp, old, OrgAdminerAttr.OrgNo, this.No);

            //插入到管理员.
            oa.FK_Emp = emp.UserID;
            oa.Save();

            //检查超级管理员是否存在？
            return "修改成功,请关闭当前记录重新打开.";
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

            Dept deptParent = new Dept();
            deptParent.No = this.ParentNo;
            if (deptParent.RetrieveFromDBSources() == 0)
                return "err@部门组织结构树上父级缺少[" + this.No + "]的部门.";

            if (this.ParentName.Equals(deptParent.Name) == false)
            {
                this.ParentName = deptParent.Name;
                err += "info@父级部门名称与组织名称已经同步.";
            }
            this.Update(); //执行更新.

            //设置子集部门，的OrgNo.
            if (DBAccess.IsView("Port_Dept") == false)
                SetSubDeptOrgNo(this.No);

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
            BP.Sys.FrmTree ftRoot = new Sys.FrmTree();
            ftRoot.Retrieve(BP.WF.Template.FlowSortAttr.ParentNo, "0");

            //设置表单树权限.
            BP.Sys.FrmTree ft = new Sys.FrmTree();
            ft.No = this.No;
            if (ft.RetrieveFromDBSources() == 0)
            {
                ft.Name = this.Name;
                ft.Name = "表单树";
                ft.ParentNo = ftRoot.No;
                ft.OrgNo = this.No;
                ft.Idx = 999;
                ft.DirectInsert();

                //创建两个目录.
                BP.Sys.FrmTree mySubFT = ft.DoCreateSubNode() as BP.Sys.FrmTree;
                mySubFT.Name = "表单目录1";
                mySubFT.OrgNo = this.No;
                mySubFT.DirectUpdate();


                mySubFT = ft.DoCreateSubNode() as BP.Sys.FrmTree;
                mySubFT.Name = "表单目录2";
                mySubFT.OrgNo = this.No;
                mySubFT.DirectUpdate();

            }
            else
            {
                ft.Name = this.Name;
                ft.Name = "表单树"; //必须这个命名，否则找不到。
                ft.ParentNo = ftRoot.No;
                ft.OrgNo = this.No;
                ft.Idx = 999;
                ft.DirectUpdate();
            }
            #endregion 检查表单树.

            if (DataType.IsNullOrEmpty(err) == true)
                return "系统正确";

            //检查表单树.
            return "err@" + err;
        }
        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="no"></param>
        public void SetSubDeptOrgNo(string no)
        {
            //同步当前部门与当前部门的子集部门，设置相同的orgNo.
            Depts subDepts = new Depts();
            subDepts.Retrieve(DeptAttr.ParentNo, no);
            foreach (Dept subDept in subDepts)
            {
                //判断当前部门是否是组织？
                Org org = new Org();
                org.No = subDept.No;
                if (org.RetrieveFromDBSources() == 1)
                    continue; //说明当前部门是组织.

                subDept.OrgNo = this.No;
                subDept.Update();

                //递归调用.
                SetSubDeptOrgNo(subDept.No);
            }
        }
        /// <summary>
        /// 检查组织结构信息.
        /// </summary>
        /// <returns></returns>
        public string CheckOrgInfo()
        {
            /*
             * 检查内容如下：
             * 1. 与org里面的部门是否存在？
             */



            return "";
        }


        //public string SetSubOrg(string userNo)
        //{
        //    BP.WF.Port.Admin2Group.Dept dept = new WF.Port.Admin2Group.Dept(this.No);
        //    return dept.SetSubOrg(userNo);
        //}

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
