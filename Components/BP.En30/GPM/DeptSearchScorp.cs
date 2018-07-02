using System;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.GPM
{
    /// <summary>
    /// 部门查询权限
    /// </summary>
    public class DeptSearchScorpAttr
    {
        #region 基本属性
        /// <summary>
        /// 工作人员ID
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        #endregion
    }
    /// <summary>
    /// 部门查询权限 的摘要说明。
    /// </summary>
    public class DeptSearchScorp : Entity
    {

        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.No == "admin")
                {
                    uac.IsView = true;
                    uac.IsDelete = true;
                    uac.IsInsert = true;
                    uac.IsUpdate = true;
                    uac.IsAdjunct = true;
                }
                return uac;
            }
        }

        #region 基本属性
        /// <summary>
        /// 工作人员ID
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(DeptSearchScorpAttr.FK_Emp);
            }
            set
            {
                SetValByKey(DeptSearchScorpAttr.FK_Emp, value);
            }
        }
        public string FK_DeptT
        {
            get
            {
                return this.GetValRefTextByKey(DeptSearchScorpAttr.FK_Dept);
            }
        }
        /// <summary>
        ///部门
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStringByKey(DeptSearchScorpAttr.FK_Dept);
            }
            set
            {
                SetValByKey(DeptSearchScorpAttr.FK_Dept, value);
            }
        }
        #endregion

        #region 扩展属性

        #endregion

        #region 构造函数
        /// <summary>
        /// 工作人员岗位
        /// </summary> 
        public DeptSearchScorp() { }
        /// <summary>
        /// 工作人员部门对应
        /// </summary>
        /// <param name="_empoid">工作人员ID</param>
        /// <param name="wsNo">部门编号</param> 	
        public DeptSearchScorp(string _empoid, string wsNo)
        {
            this.FK_Emp = _empoid;
            this.FK_Dept = wsNo;
            if (this.Retrieve() == 0)
                this.Insert();
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

                Map map = new Map("Port_DeptSearchScorp");
                map.EnDesc = "部门查询权限";
                map.Java_SetEnType(EnType.Dot2Dot);

                map.AddTBStringPK(DeptSearchScorpAttr.FK_Emp, null, "操作员", true, true, 1, 50, 11);
                map.AddDDLEntitiesPK(DeptSearchScorpAttr.FK_Dept, null, "部门", new Depts(), true);
                // map.AddDDLEntitiesPK(DeptSearchScorpAttr.FK_Emp, null, "操作员", new Emps(), true);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 重载基类方法
        /// <summary>
        /// 插入前所做的工作
        /// </summary>
        /// <returns>true/false</returns>
        protected override bool beforeInsert()
        {
            return base.beforeInsert();
        }
        /// <summary>
        /// 更新前所做的工作
        /// </summary>
        /// <returns>true/false</returns>
        protected override bool beforeUpdate()
        {
            return base.beforeUpdate();
        }
        /// <summary>
        /// 删除前所做的工作
        /// </summary>
        /// <returns>true/false</returns>
        protected override bool beforeDelete()
        {
            return base.beforeDelete();
        }
        #endregion
    }
    /// <summary>
    /// 部门查询权限 
    /// </summary>
    public class DeptSearchScorps : Entities
    {
        #region 构造
        /// <summary>
        /// 部门查询权限
        /// </summary>
        public DeptSearchScorps() { }
        /// <summary>
        /// 部门查询权限
        /// </summary>
        /// <param name="FK_Emp">FK_Emp</param>
        public DeptSearchScorps(string FK_Emp)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(DeptSearchScorpAttr.FK_Emp, FK_Emp);
            qo.DoQuery();
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
                return new DeptSearchScorp();
            }
        }
        #endregion

        #region 查询方法

        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<DeptSearchScorp> ToJavaList()
        {
            return (System.Collections.Generic.IList<DeptSearchScorp>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<DeptSearchScorp> Tolist()
        {
            System.Collections.Generic.List<DeptSearchScorp> list = new System.Collections.Generic.List<DeptSearchScorp>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((DeptSearchScorp)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }

}
