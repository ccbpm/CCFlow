using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web;
using BP.Port;
using BP.Sys;
using BP.WF.Template;

namespace BP.WF.Port.Admin2Group
{
    /// <summary>
    /// 部门属性
    /// </summary>
    public class DeptAttr : BP.Port.DeptAttr
    {
    }
    /// <summary>
    /// 部门
    /// </summary>
    public class Dept : EntityTree
    {
        #region 属性
        /// <summary>
        /// 父节点编号
        /// </summary>
        public string ParentNo
        {
            get
            {
                return this.GetValStrByKey(DeptAttr.ParentNo);
            }
            set
            {
                this.SetValByKey(DeptAttr.ParentNo, value);
            }
        }
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStrByKey(DeptAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(DeptAttr.OrgNo, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 部门
        /// </summary>
        public Dept() { }
        /// <summary>
        /// 部门
        /// </summary>
        /// <param name="no">编号</param>
        public Dept(string no) : base(no) { }
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

                Map map = new Map("Port_Dept", "部门");
                map.AddTBStringPK(DeptAttr.No, null, "编号", true, false, 1, 30, 40);
                map.AddTBString(DeptAttr.Name, null, "名称", true, false, 0, 60, 200);
                map.AddTBString(DeptAttr.ParentNo, null, "父节点编号", true, true, 0, 30, 40);
                map.AddTBString(DeptAttr.OrgNo, null, "隶属组织", true, true, 0, 50, 40);
                map.AddTBInt(DeptAttr.Idx, 0, "顺序号", true, false);

                if (BP.Web.WebUser.No.Equals("admin") == true)
                {
                    RefMethod rm = new RefMethod();
                    rm.Title = "设置为独立组织";
                    rm.Warning = "如果当前部门已经是独立组织，系统就会提示错误。";
                    rm.ClassMethodName = this.ToString() + ".SetDept2Org";
                    rm.HisAttrs.AddTBString("adminer", null, "组织管理员编号", true, false, 0, 100, 100);
                    map.AddRefMethod(rm);
                }

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeDelete()
        {
            //检查是否可以删除.
            BP.Port.Dept dept = new BP.Port.Dept(this.No);
            dept.CheckIsCanDelete();

            return base.beforeDelete();
        }
        /// <summary>
        /// 设置组织
        /// </summary>
        /// <param name="userNo">管理员编号</param>
        /// <returns></returns>
        public string SetDept2Org(string adminer)
        {
            if (WebUser.No.Equals("admin") == false)
                return "err@非admin管理员，您无法执行该操作.";

            //检查是否有该用户.
            BP.Port.Emp emp = new BP.Port.Emp();
            emp.UserID = adminer;
            if (emp.RetrieveFromDBSources() == 0)
                return "err@用户编号错误:" + adminer;

            //如果指定的人员.
            if (emp.DeptNo.Equals(this.No) == false) {
                Depts depts = new Depts();
                depts.Retrieve(DeptAttr.ParentNo, this.No);
                bool isHave = false;
                foreach (Dept dept in depts)
                {
                    if (emp.DeptNo.Equals(dept.No))
                    {
                        isHave = true;
                        break;
                    }
                }
                if (isHave == false)
                    return "err@管理员不在本部门及本部门下级部门下，您不能设置他为管理员.";
            }

            //检查该部门是否是独立组织.
            BP.WF.Port.Admin2Group.Org org = new BP.WF.Port.Admin2Group.Org();
            org.No = this.No;
            if (org.RetrieveFromDBSources() == 1)
                return "err@当前已经是独立组织.";

            org.Name = this.Name; //把部门名字改为组织名字.

            //设置父级信息.
            BP.Port.Dept parentDept = new BP.Port.Dept();
            if (this.ParentNo.Equals("0") == true)
                this.ParentNo = this.No;

            parentDept.No = this.ParentNo;
            parentDept.Retrieve();

            //设置管理员信息.
            org.Adminer = emp.UserID;
            org.AdminerName = emp.Name;
            org.Insert();

            //增加到管理员.
            OrgAdminer oa = new OrgAdminer();
            oa.EmpNo = emp.UserID;
            oa.OrgNo = this.No;
            oa.Insert();

            //设置部门编号.
            this.OrgNo = this.No;
            this.DirectUpdate();

            //如果不是视图.
            if (DBAccess.IsView("Port_StationType") == false)
            {
                StationTypes sts = new StationTypes();
                sts.Retrieve("OrgNo", this.No);
                if (sts.Count == 0)
                {

                    #region 高层角色.
                    StationType st = new StationType();
                    st.No = DBAccess.GenerGUID();
                    st.Name = "高层岗";
                    st.OrgNo = this.No;
                    st.DirectInsert();

                    Station sta = new Station();
                    sta.No = DBAccess.GenerGUID();
                    sta.Name = "总经理";
                    sta.OrgNo = this.No;
                    sta.FK_StationType = st.No;
                    sta.DirectInsert();
                    #endregion 高层角色.

                    #region 中层岗.
                    st = new StationType();
                    st.No = DBAccess.GenerGUID();
                    st.Name = "中层岗";
                    st.OrgNo = this.No;
                    st.DirectInsert();

                    sta = new Station();
                    sta.No = DBAccess.GenerGUID();
                    sta.Name = "财务部经理";
                    sta.OrgNo = this.No;
                    sta.FK_StationType = st.No;
                    sta.DirectInsert();

                    sta = new Station();
                    sta.No = DBAccess.GenerGUID();
                    sta.Name = "研发部经理";
                    sta.OrgNo = this.No;
                    sta.FK_StationType = st.No;
                    sta.DirectInsert();

                    sta = new Station();
                    sta.No = DBAccess.GenerGUID();
                    sta.Name = "市场部经理";
                    sta.OrgNo = this.No;
                    sta.FK_StationType = st.No;
                    sta.DirectInsert();
                    #endregion 中层岗.

                    #region 基层岗.
                    st = new StationType();
                    st.No = DBAccess.GenerGUID();
                    st.Name = "基层岗";
                    st.OrgNo = this.No;
                    st.DirectInsert();

                    sta = new Station();
                    sta.No = DBAccess.GenerGUID();
                    sta.Name = "会计岗";
                    sta.OrgNo = this.No;
                    sta.FK_StationType = st.No;
                    sta.DirectInsert();

                    sta = new Station();
                    sta.No = DBAccess.GenerGUID();
                    sta.Name = "销售岗";
                    sta.OrgNo = this.No;
                    sta.FK_StationType = st.No;
                    sta.DirectInsert();

                    sta = new Station();
                    sta.No = DBAccess.GenerGUID();
                    sta.Name = "程序员岗";
                    sta.OrgNo = this.No;
                    sta.FK_StationType = st.No;
                    sta.DirectInsert();
                    #endregion 基层岗.
                }
            }
            // 返回他的检查信息，这个方法里，已经包含了自动创建独立组织的，表单树，流程树。
            // 自动他创建，角色类型，角色信息.
            string info = org.DoCheck();

            if (info.IndexOf("err@") == 0)
                return info;

            return "设置成功.";
            //初始化表单树，流程树.
            //InitFlowSortTree();
            //return "设置成功,[" + ad.No + "," + ad.Name + "]重新登录就可以看到.";
        }
    }
    /// <summary>
    ///部门集合
    /// </summary>
    public class Depts : EntitiesTree
    {
        /// <summary>
        /// 查询全部。
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAll()
        {
            if (BP.Web.WebUser.No.Equals("admin") == true)
                return base.RetrieveAll();

            QueryObject qo = new QueryObject(this);
            qo.AddWhere(DeptAttr.No, " = ", BP.Web.WebUser.DeptNo);
            qo.addOr();
            qo.AddWhere(DeptAttr.ParentNo, " = ", BP.Web.WebUser.DeptNo);
            return qo.DoQuery();
        }
        /// <summary>
        /// 得到一个新实体
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Dept();
            }
        }
        /// <summary>
        /// create ens
        /// </summary>
        public Depts()
        {
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Dept> ToJavaList()
        {
            return (System.Collections.Generic.IList<Dept>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Dept> Tolist()
        {
            System.Collections.Generic.List<Dept> list = new System.Collections.Generic.List<Dept>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Dept)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
