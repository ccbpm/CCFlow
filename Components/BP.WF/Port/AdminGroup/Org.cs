using System;
using BP.DA;
using BP.En;
using BP.Web;
using BP.WF.Port.Admin2Group;
using BP.WF.Template;

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
        /// <summary>
        /// 状态
        /// </summary>
        public const string OrgSta = "OrgSta";
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
                uac.IsDelete = true;
               /* if (DataType.IsNullOrEmpty(this.No) == true)
                {
                    uac.IsUpdate = true;
                    uac.IsInsert = true;
                    return uac;
                }*/
                uac.IsInsert = false;
                uac.IsUpdate = true;
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

                map.AddTBStringPK(OrgAttr.No, null, "编号(与部门编号相同)", true, false, 1, 30, 40);
                map.AddTBString(OrgAttr.Name, null, "组织名称", true, false, 0, 60, 200, true);
                map.AddTBString(OrgAttr.ParentNo, null, "父级组织编号", false, false, 0, 60, 200, true);
                map.AddTBString(OrgAttr.ParentName, null, "父级组织名称", false, false, 0, 60, 200, true);

                map.AddTBString(OrgAttr.Adminer, null, "主要管理员(创始人)", true, true, 0, 60, 200, true);
                map.AddTBString(OrgAttr.AdminerName, null, "管理员名称", true, true, 0, 60, 200, true);
                map.AddTBString("SSOUrl", null, "SSOUrl", true, false, 0, 200, 200, true);
                map.AddTBInt(OrgAttr.OrgSta, 0, "组织状态", true, false);
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

                //管理员.
                map.AddDtl(new OrgAdminers(), OrgAdminerAttr.OrgNo, null, DtlEditerModel.DtlSearch, "icon-people");

                //rm = new RefMethod();
                //rm.Title = "在集团下新增组织";
                //rm.ClassMethodName = this.ToString() + ".AddOrg";
                //rm.HisAttrs.AddTBString("no", null, "组织编号", true, false, 0, 100, 100);
                //rm.HisAttrs.AddTBString("name", null, "组织名称", true, false, 0, 100, 100);
                //rm.HisAttrs.AddTBString("adminer", null, "管理员编号", true, false, 0, 100, 100);
                //rm.HisAttrs.AddTBString("adminName", null, "管理员名称", true, false, 0, 100, 100);
                //map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 调用admin2Group的检查.
        /// 1. 是否出现错误.
        /// 1. 数据是否完整.
        /// </summary>
        /// <returns></returns>
        public string DoCheck()
        {
            BP.WF.Port.Admin2Group.Org org = new BP.WF.Port.Admin2Group.Org(this.No);
            return org.DoCheck();
        }
        /// <summary>
        /// 去掉独立组织
        /// </summary>
        /// <returns></returns>
        public string DeleteOrg()
        {
            if (WebUser.No.Equals("admin") == false)
                return "err@只有admin帐号才可以执行。";

            if (this.No.Equals("100") == true)
                return "err@admin组织不能取消.";

            //流程类别.
            BP.WF.Template.FlowSorts fss = new BP.WF.Template.FlowSorts();
            fss.Retrieve(OrgAdminerAttr.OrgNo, this.No);
            foreach (BP.WF.Template.FlowSort en in fss)
            {
                Flows fls = new Flows();
                fls.Retrieve(BP.WF.Template.FlowAttr.FK_FlowSort, en.No);

                if (fls.Count != 0)
                    return "err@在流程目录：" + en.Name + "有[" + fls.Count + "]个流程没有删除。";
            }

            //表单类别.
            SysFormTrees ftTrees = new SysFormTrees();
            ftTrees.Retrieve(SysFormTreeAttr.OrgNo, this.No);
            foreach (BP.WF.Template.FlowSort en in fss)
            {
                BP.Sys.MapDatas mds = new BP.Sys.MapDatas();
                mds.Retrieve(BP.Sys.MapDataAttr.FK_FormTree, en.No);
                if (mds.Count != 0)
                    return "err@在表单目录：" + en.Name + "有[" + mds.Count + "]个表单没有删除。";
            }

            OrgAdminers oas = new OrgAdminers();
            oas.Delete(OrgAdminerAttr.OrgNo, this.No);

            BP.WF.Template.FlowSorts fs = new BP.WF.Template.FlowSorts();
            fs.Delete(OrgAdminerAttr.OrgNo, this.No);

            fss.Delete(OrgAdminerAttr.OrgNo, this.No); //删除流程目录.
            ftTrees.Delete(SysFormTreeAttr.OrgNo, this.No); //删除表单目录。

            //更新到admin的组织下.
            string sqls = "UPDATE Port_Emp SET OrgNo='" + BP.Web.WebUser.OrgNo + "' AND OrgNo='" + this.No + "'";
            sqls += "@UPDATE Port_Dept SET OrgNo='" + BP.Web.WebUser.OrgNo + "' AND OrgNo='" + this.No + "'";
            sqls += "@UPDATE Port_DeptEmp SET OrgNo='" + BP.Web.WebUser.OrgNo + "' AND OrgNo='" + this.No + "'";
            sqls += "@UPDATE Port_DeptEmpStation SET OrgNo='" + BP.Web.WebUser.OrgNo + "' AND OrgNo='" + this.No + "'";
            DBAccess.RunSQLs(sqls);

            this.Delete();
            return "info@成功注销组织,请关闭窗口刷新页面.";
        }
        /// <summary>
        /// 更改管理员（admin才能操作）
        /// </summary>
        /// <param name="adminer"></param>
        /// <returns></returns>
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
            oa.EmpNo = old;
            oa.OrgNo = this.No;
            oa.Delete(OrgAdminerAttr.FK_Emp, old, OrgAdminerAttr.OrgNo, this.No);

            //插入到管理员.
            oa.EmpNo = emp.UserID;
            oa.Save();

            //检查超级管理员是否存在？
            return "修改成功,请关闭当前记录重新打开.";
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
