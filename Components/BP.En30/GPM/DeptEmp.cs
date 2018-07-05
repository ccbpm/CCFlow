using System;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.GPM
{
    /// <summary>
    /// 部门人员信息
    /// </summary>
    public class DeptEmpAttr
    {
        #region 基本属性
        /// <summary>
        /// 部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 人员
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        #endregion
    }
    /// <summary>
    /// 部门人员信息 的摘要说明。
    /// </summary>
    public class DeptEmp : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
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
        /// 人员
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(DeptEmpAttr.FK_Emp);
            }
            set
            {
                SetValByKey(DeptEmpAttr.FK_Emp, value);
                this.MyPK = this.FK_Dept + "_" + this.FK_Emp;
            }
        }
        /// <summary>
        /// 部门
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStringByKey(DeptEmpAttr.FK_Dept);
            }
            set
            {
                SetValByKey(DeptEmpAttr.FK_Dept, value);
                this.MyPK = this.FK_Dept + "_" + this.FK_Emp;
            }
        } 
        #endregion

        #region 扩展属性

        #endregion

        #region 构造函数
        /// <summary>
        /// 工作部门人员信息
        /// </summary> 
        public DeptEmp() { }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="deptNo">部门编号</param>
        /// <param name="empNo">人员编号</param>
        public DeptEmp(string deptNo, string empNo)
        {
            this.FK_Dept = deptNo;
            this.FK_Emp = empNo;
            this.MyPK = this.FK_Dept + "_" + this.FK_Emp;
            this.Retrieve();
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Port_DeptEmp");
                map.EnDesc = "部门人员信息";  
                
                map.AddMyPK();
                map.AddTBString(DeptEmpAttr.FK_Dept, null, "部门", false, false, 1, 50, 1);
                map.AddDDLEntities(DeptEmpAttr.FK_Emp, null, "操作员", new BP.Port.Emps(), false);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 更新前做的事情
        /// </summary>
        /// <returns></returns>
        protected override bool beforeUpdateInsertAction()
        {
            this.MyPK = this.FK_Dept + "_" + this.FK_Emp;
            return base.beforeUpdateInsertAction();
        }
    }
    /// <summary>
    /// 部门人员信息 
    /// </summary>
    public class DeptEmps : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 工作部门人员信息
        /// </summary>
        public DeptEmps()
        {
        }
        #endregion

        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new DeptEmp();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<DeptEmp> ToJavaList()
        {
            return (System.Collections.Generic.IList<DeptEmp>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<DeptEmp> Tolist()
        {
            System.Collections.Generic.List<DeptEmp> list = new System.Collections.Generic.List<DeptEmp>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((DeptEmp)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

    }
}
