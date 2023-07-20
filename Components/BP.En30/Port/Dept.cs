using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web;
using BP.Sys;

namespace BP.Port
{
    /// <summary>
    /// 部门属性
    /// </summary>
    public class DeptAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 父节点的编号
        /// </summary>
        public const string ParentNo = "ParentNo";
        /// <summary>
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// 部门编号
        /// </summary>
        public const string Leader = "Leader";
        /// <summary>
        /// 顺序号
        /// </summary>
        public const string Idx = "Idx";

        public const string NameOfPath = "NameOfPath";

    }
    /// <summary>
    /// 部门
    /// </summary>
    public class Dept : EntityTree
    {
        #region 属性
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
        /// <summary>
        /// 父节点的ID
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
        public string NameOfPath
        {
            get
            {
                return this.GetValStrByKey(DeptAttr.NameOfPath);
            }
            set
            {
                this.SetValByKey(DeptAttr.NameOfPath, value);
            }
        }
        public string Leader
        {
            get
            {
                return this.GetValStrByKey(DeptAttr.Leader);
            }
            set
            {
                this.SetValByKey(DeptAttr.Leader, value);
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
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
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
                map.IsEnableVer = true;

                map.AddTBStringPK(DeptAttr.No, null, "编号", true, false, 1, 50, 20);
                map.AddTBString(DeptAttr.Name, null, "名称", true, false, 0, 100, 30);
                map.AddTBString(DeptAttr.NameOfPath, null, "部门路径", true, false, 0, 100, 30);

                map.AddTBString(DeptAttr.ParentNo, null, "父节点编号", true, true, 0, 100, 30);
                map.AddTBString(DeptAttr.OrgNo, null, "OrgNo", true, true, 0, 50, 30);
                map.AddDDLEntities(DeptAttr.Leader, null, "部门领导", new BP.Port.Emps(), true);
                map.AddTBInt(DeptAttr.Idx, 0, "序号", false, true);


                if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                    map.AddHidden("OrgNo", "=", "@WebUser.OrgNo");

                RefMethod rm = new RefMethod();
                //rm.Title = "历史变更";
                //rm.ClassMethodName = this.ToString() + ".History";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //map.AddRefMethod(rm);

                #region 增加点对多属性
                rm.Title = "重置该部门以下的部门路径";
                rm.ClassMethodName = this.ToString() + ".DoResetPathName";
                rm.RefMethodType = RefMethodType.Func;

                string msg = "当该部门名称变化后,该部门与该部门的子部门名称路径(Port_Dept.NameOfPath)将发生变化.";
                msg += "\t\n 该部门与该部门的子部门的人员路径也要发生变化Port_Emp列DeptDesc.StaDesc.";
                msg += "\t\n 您确定要执行吗?";
                rm.Warning = msg;

                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "增加同级部门";
                //rm.ClassMethodName = this.ToString() + ".DoSameLevelDept";
                //rm.HisAttrs.AddTBString("No", null, "同级部门编号", true, false, 0, 100, 100);
                //rm.HisAttrs.AddTBString("Name", null, "部门名称", true, false, 0, 100, 100);
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "增加下级部门";
                //rm.ClassMethodName = this.ToString() + ".DoSubDept";
                //rm.HisAttrs.AddTBString("No", null, "同级部门编号", true, false, 0, 100, 100);
                //rm.HisAttrs.AddTBString("Name", null, "部门名称", true, false, 0, 100, 100);
                //map.AddRefMethod(rm);


                //节点绑定人员. 使用树杆与叶子的模式绑定.
                string rootNo = "0";
                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single && (DataType.IsNullOrEmpty(WebUser.No) == true || WebUser.IsAdmin == false))
                    rootNo = "@WebUser.FK_Dept";
                else
                    rootNo = "@WebUser.OrgNo";
                map.AttrsOfOneVSM.AddBranchesAndLeaf(new DeptEmps(), new BP.Port.Emps(),
                   DeptEmpAttr.FK_Dept,
                   DeptEmpAttr.FK_Emp, "对应人员", BP.Port.EmpAttr.FK_Dept, BP.Port.EmpAttr.Name, BP.Port.EmpAttr.No, rootNo);


                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeUpdateInsertAction()
        {
            BP.Sys.Base.Glo.WriteUserLog("新建/修改部门:" + this.ToJson(), "组织数据操作");
            return base.beforeUpdateInsertAction();
        }

        protected override bool beforeDelete()
        {
            this.CheckIsCanDelete();

            BP.Sys.Base.Glo.WriteUserLog("删除部门:" + this.ToJson(), "组织数据操作");
            return base.beforeDelete();
        }

        public bool CheckIsCanDelete()
        {
            string err = "";
            string sql = "select count(*) FROM Port_Emp WHERE FK_Dept='" + this.No + "'";
            int num = DBAccess.RunSQLReturnValInt(sql);
            if (num != 0)
                err += "err@该部门下有" + num + "个人员数据，您不能删除.";

            sql = "select count(*) FROM Port_DeptEmp WHERE FK_Dept='" + this.No + "'";
            num = DBAccess.RunSQLReturnValInt(sql);
            if (num != 0)
                err += "err@该部门在人员部门信息表里有" + num + "笔数据,您不能删除.";

            sql = "select count(*) FROM Port_DeptEmpStation WHERE FK_Dept='" + this.No + "'";
            num = DBAccess.RunSQLReturnValInt(sql);
            if (num != 0)
                err += "err@该部门在人员部门角色表里有" + num + "笔数据,您不能删除.";

            //检查是否有子级部门.
            sql = "select count(*) FROM Port_Dept WHERE ParentNo='" + this.No + "'";
            if (num != 0)
                err += "err@该部门有" + num + "个子部门,您不能删除.";

            //是不是组织？.
            sql = "select count(*) FROM Port_Org WHERE OrgNo='" + this.No + "'";
            if (num != 0)
                err += "err@该部门是一个组织,您不能删除.";

            if (DataType.IsNullOrEmpty(err) == false)
                throw new Exception(err);
            return true;
        }

        /// <summary>
        /// 重置部门
        /// </summary>
        /// <returns></returns>
        public string DoResetPathName()
        {
            this.GenerNameOfPath();
            return "重置成功.";
        }

        /// <summary>
        /// 生成部门全名称.
        /// </summary>
        public void GenerNameOfPath()
        {
            string name = this.Name;

            //根目录不再处理
            if (this.IsRoot == true)
            {
                this.NameOfPath = name;
                this.DirectUpdate();
                this.GenerChildNameOfPath(this.No);
                return;
            }

            Dept dept = new Dept();
            dept.No = this.ParentNo;
            if (dept.RetrieveFromDBSources() == 0)
                return;

            while (true)
            {
                if (dept.IsRoot)
                    break;

                name = dept.Name + "\\" + name;
                dept = new Dept(dept.ParentNo);
            }
            //根目录
            name = dept.Name + "\\" + name;
            this.NameOfPath = name;
            this.DirectUpdate();

            this.GenerChildNameOfPath(this.No);

            //更新人员路径信息.
            BP.Port.Emps emps = new BP.Port.Emps();
            emps.Retrieve(BP.Port.EmpAttr.FK_Dept, this.No);
            foreach (BP.Port.Emp emp in emps)
                emp.Update();
        }

        /// <summary>
        /// 处理子部门全名称
        /// </summary>
        /// <param name="FK_Dept"></param>
        public void GenerChildNameOfPath(string deptNo)
        {
            Depts depts = new Depts(deptNo);
            if (depts != null && depts.Count > 0)
            {
                foreach (Dept dept in depts)
                {
                    dept.GenerNameOfPath();
                    GenerChildNameOfPath(dept.No);

                    //更新人员路径信息.
                    BP.Port.Emps emps = new BP.Port.Emps();
                    emps.Retrieve(BP.Port.EmpAttr.FK_Dept, this.No);
                    foreach (BP.Port.Emp emp in emps)
                        emp.Update();
                }
            }
        }
        /// <summary>
        /// 执行排序
        /// </summary>
        /// <param name="deptIDs"></param>
        /// <returns></returns>
        public string DoOrder(string deptIDs)
        {
            string[] ids = deptIDs.Split(',');

            for (int i = 0; i < ids.Length; i++)
            {
                var id = ids[i];
                if (DataType.IsNullOrEmpty(id) == true)
                    continue;
                DBAccess.RunSQL("UPDATE Port_Dept SET Idx=" + i + " WHERE No='" + id + "'");
            }
            return "排序成功.";
        }

        public string History()
        {
            return "EnVerDtl.htm?EnName=" + this.ToString() + "&PK=" + this.No;
        }

        #region 重写查询. 2015.09.31 为适应ws的查询.
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public override int Retrieve()
        {
            return base.Retrieve();
        }
        /// <summary>
        /// 查询.
        /// </summary>
        /// <returns></returns>
        public override int RetrieveFromDBSources()
        {
            return base.RetrieveFromDBSources();
        }
        #endregion

    }
    /// <summary>
    ///部门s
    /// </summary>
    public class Depts : EntitiesTree
    {
        #region 初始化实体.
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
        /// 部门集合
        /// </summary>
        public Depts()
        {
        }
        /// <summary>
        /// 部门集合
        /// </summary>
        /// <param name="parentNo">父部门No</param>
        public Depts(string parentNo)
        {
            this.Retrieve(DeptAttr.ParentNo, parentNo);
        }

        #endregion 初始化实体.

        #region 重写查询,add by zhoupeng 2015.09.30 为了适应能够从webservice数据源查询数据.
        public override int RetrieveAll()
        {

            if (BP.Web.WebUser.No.Equals("admin") == true)
            {
                QueryObject qo = new QueryObject(this);
                qo.addOrderBy(DeptAttr.Idx);
                return qo.DoQuery();
            }

            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
            {
                QueryObject qo = new QueryObject(this);
                qo.AddWhere(DeptAttr.No, " = ", BP.Web.WebUser.FK_Dept);
                qo.addOr();
                qo.AddWhere(DeptAttr.ParentNo, " = ", BP.Web.WebUser.FK_Dept);
                qo.addOrderBy(DeptAttr.Idx);
                return qo.DoQuery();
            }

            return this.Retrieve("OrgNo", BP.Web.WebUser.OrgNo, DeptAttr.Idx);
        }
        /// <summary>
        /// 重写重数据源查询全部适应从WS取数据需要
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAllFromDBSource()
        {

            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                return base.RetrieveAllFromDBSource();

            //按照orgNo查询.
            return this.Retrieve("OrgNo", WebUser.OrgNo);
        }
        #endregion 重写查询.

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
