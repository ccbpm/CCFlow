using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo.BPFramework
{
    /// <summary>
    /// 员工考核得分 属性
    /// </summary>
    public class EmpCentAttr : EntityNoNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 员工
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 得分
        /// </summary>
        public const string Cent = "Cent";
        /// <summary>
        /// 年月
        /// </summary>
        public const string FK_NY = "FK_NY";
        /// <summary>
        /// 部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        #endregion
    }
    /// <summary>
    /// 员工考核得分
    /// </summary>
    public class EmpCent : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 员工
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(EmpCentAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(EmpCentAttr.FK_Emp, value);
            }
        }
        /// <summary>
        /// 得分
        /// </summary>
        public float Cent
        {
            get
            {
                return this.GetValFloatByKey(EmpCentAttr.Cent);
            }
            set
            {
                this.SetValByKey(EmpCentAttr.Cent, value);
            }
        }
        /// <summary>
        /// 年月
        /// </summary>
        public string FK_NY
        {
            get
            {
                return this.GetValStringByKey(EmpCentAttr.FK_NY);
            }
            set
            {
                this.SetValByKey(EmpCentAttr.FK_NY, value);
            }
        }
        /// <summary>
        /// 部门
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStringByKey(EmpCentAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(EmpCentAttr.FK_Dept, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 员工考核得分
        /// </summary>
        public EmpCent()
        {
        }
        /// <summary>
        /// 员工考核得分
        /// </summary>
        /// <param name="mypk"></param>
        public EmpCent(string mypk):base(mypk)
        {
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
                Map map = new Map("Demo_EmpCent", "员工考核得分");
                
                // 普通字段
                map.AddMyPK();
                map.AddTBString(EmpCentAttr.FK_Emp, null, "员工", true, false, 0, 200, 10);
                map.AddTBString(EmpCentAttr.FK_Dept, null, "隶属部门(冗余列)", true, false, 0, 200, 10);
                map.AddTBString(EmpCentAttr.FK_NY, null, "月份", true, false, 0, 200, 10);
                map.AddTBFloat(EmpCentAttr.Cent, 0, "得分", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 重写基类的方法.
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            return base.beforeInsert();
        }
    }
    /// <summary>
    /// 员工考核得分s
    /// </summary>
    public class EmpCents : EntitiesMyPK
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new EmpCent();
            }
        }
        /// <summary>
        /// 员工考核得分s
        /// </summary>
        public EmpCents() { }
        #endregion
    }
}
