using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web;
using BP.Port;

namespace BP.Cloud
{
    /// <summary>
    /// 部门属性
    /// </summary>
    public class DeptAttr : EntityTreeAttr
    {
        /// <summary>
        /// 单位全名
        /// </summary>
        public const string NameOfPath = "NameOfPath";
        /// <summary>
        /// 管理员
        /// </summary>
        public const string Adminer = "Adminer";
        /// <summary>
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// 部门ID
        /// </summary>
        public const string RefID = "RefID";
        /// <summary>
        /// 关联的父节点ID
        /// </summary>
        public const string RefParentID = "RefParentID";
        /// <summary>
        /// 部门主管
        /// </summary>
        public const string Leader = "Leader";
    }
    /// <summary>
    /// 部门
    /// </summary>
    public class Dept : EntityTree
    {
        #region 属性
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
        /// <summary>
        /// 领导
        /// </summary>
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
        /// <summary>
        /// 部门ID
        /// </summary>
        public string RefID
        {
            get
            {
                return this.GetValStrByKey(DeptAttr.RefID);
            }
            set
            {
                this.SetValByKey(DeptAttr.RefID, value);
            }
        }
        public string RefParentID
        {
            get
            {
                return this.GetValStrByKey(DeptAttr.RefParentID);
            }
            set
            {
                this.SetValByKey(DeptAttr.RefParentID, value);
            }
        }
        /// <summary>
        /// adminer
        /// </summary>
        public string Adminer
        {
            get
            {
                return this.GetValStrByKey(DeptAttr.Adminer);
            }
            set
            {
                this.SetValByKey(DeptAttr.Adminer, value);
            }
        }
        /// <summary>
        /// 全名
        /// </summary>
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
        /// <summary>
        /// 父节点的ID
        /// </summary>
        public new string ParentNo
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

                map.AddTBStringPK(DeptAttr.No, null, "编号", true, true, 1, 50, 20);
                map.AddTBString(DeptAttr.Name, null, "部门名称", true, false, 0, 100, 30, false);

                map.AddTBString(DeptAttr.NameOfPath, null, "部门全称", true, true, 0, 100, 30, true);


                map.AddTBString(DeptAttr.ParentNo, null, "父节点编号", true, true, 0, 100, 30);
                map.AddTBString(DeptAttr.OrgNo, null, "组织编码", true, true, 0, 100, 30);

                map.AddTBString(DeptAttr.Leader, null, "部门负责人", true, false, 0, 100, 30);

                map.AddTBString(DeptAttr.Adminer, null, "管理员帐号", false, false, 0, 100, 30);
                map.AddTBInt(DeptAttr.Idx, 0, "Idx", false, false); //顺序号.

                //微信，丁丁的第三方的ID.
                map.AddTBString(DeptAttr.RefID, null, "RefID", false, true, 0, 100, 30);
                map.AddTBString(DeptAttr.RefParentID, null, "RefParentID", false, true, 0, 100, 30);

                ////节点绑定人员. 使用树杆与叶子的模式绑定.
                //map.AttrsOfOneVSM.AddBranchesAndLeaf(new DeptEmps(), new BP.Port.Emps(),
                //   DeptEmpAttr.FK_Dept,
                //   DeptEmpAttr.FK_Emp, "对应人员", EmpAttr.FK_Dept, EmpAttr.Name,
                //   EmpAttr.No, "@WebUser.FK_Dept");

                RefMethod rm = new RefMethod();
                rm.Title = "重置部门全称";
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

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeDelete()
        {
            DeptEmps ens = new DeptEmps();
            ens.Retrieve(DeptEmpAttr.FK_Dept, this.No);
            if (ens.Count > 0)
                throw new Exception("err@删除部门错误，该部门下有人员。");

            DeptEmpStations ensD = new DeptEmpStations();
            ensD.Retrieve(DeptEmpAttr.FK_Dept, this.No);
            if (ensD.Count > 0)
                throw new Exception("err@删除部门错误，该部门角色下有人员。");

            return base.beforeDelete();
        }

        protected override bool beforeInsert()
        {
            if (BP.DA.DataType.IsNullOrEmpty(this.No) == true)
                this.No = BP.DA.DBAccess.GenerGUID(5, "Port_Dept", "No");

            if (BP.DA.DataType.IsNullOrEmpty(this.OrgNo) == true)
                this.OrgNo = BP.Web.WebUser.OrgNo;

            this.Adminer = WebUser.No;
            return base.beforeInsert();
        }
        /// <summary>
        /// 创建下级节点.
        /// </summary>
        /// <returns></returns>
        public string DoMyCreateSubNode()
        {
            Entity en = this.DoCreateSubNode();
            return en.ToJson();
        }

        /// <summary>
        /// 创建同级节点.
        /// </summary>
        /// <returns></returns>
        public string DoMyCreateSameLevelNode()
        {
            Entity en = this.DoCreateSameLevelNode();
            return en.ToJson();
        }

        public string DoSameLevelDept(string no, string name)
        {
            Dept en = new Dept();
            en.No = no;
            if (en.RetrieveFromDBSources() == 1)
                return "err@编号已经存在";

            en.Name = name;
            en.ParentNo = this.ParentNo;
            en.Insert();

            return "增加成功..";
        }
        public string DoSubDept(string no, string name)
        {
            Dept en = new Dept();
            en.No = no;
            if (en.RetrieveFromDBSources() == 1)
                return "err@编号已经存在";

            en.Name = name;
            en.ParentNo = this.No;
            en.Insert();

            return "增加成功..";
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

            //根目录不再处理.
            if (this.ItIsRoot == true || this.No.Equals("100") == true)
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
                if (dept.ItIsRoot || dept.No == "100")
                    break;

                name = dept.Name + "\\" + name;
                dept = new Dept(dept.ParentNo);
            }
            //根目录
            name = dept.Name + "\\" + name;
            this.NameOfPath = name;
            this.DirectUpdate();

            this.GenerChildNameOfPath(this.No);
        }
        /// <summary>
        /// 处理子部门全名称
        /// </summary>
        /// <param name="FK_Dept"></param>
        public void GenerChildNameOfPath(string deptNo)
        {
            Depts depts = new Depts(deptNo);

            // BP.Port.Depts depts = new BP.Port.Depts(deptNo);
            if (depts != null && depts.Count > 0)
            {
                foreach (Dept dept in depts)
                {
                    dept.GenerNameOfPath();
                    GenerChildNameOfPath(dept.No);

                    ////更新人员路径信息.
                    //BP.Port.Emps emps = new Emps();
                    //emps.Retrieve(EmpAttr.FK_Dept, this.No);
                    //foreach (BP.Port.Emp emp in emps)
                    //    emp.Update();
                }
            }
        }
    }
    /// <summary>
    ///部门集合
    /// </summary>
    public class Depts : EntitiesTree
    {
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
        /// <param name="parentNo">父部门No</param>
        public Depts(string parentNo)
        {
            this.Retrieve(DeptAttr.ParentNo, parentNo);
        }
        /// <summary>
        /// 部门集合
        /// </summary>
        public Depts()
        {
        }
        public override int RetrieveAll()
        {
            return this.Retrieve(EmpAttr.OrgNo, BP.Web.WebUser.OrgNo);
        }


        #region 为了适应自动翻译成java的需要,把实体转换成IList, c#代码调用会出错误。
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
        #endregion 为了适应自动翻译成java的需要,把实体转换成IList, c#代码调用会出错误。
    }
}
