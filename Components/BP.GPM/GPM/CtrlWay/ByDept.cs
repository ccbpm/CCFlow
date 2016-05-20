using System;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.GPM
{
	/// <summary>
	/// 到部门
	/// </summary>
	public class ByDeptAttr  
	{
		#region 基本属性
		/// <summary>
		/// 控制对象
		/// </summary>
		public const  string RefObj="RefObj";
		/// <summary>
		/// 部门
		/// </summary>
		public const  string FK_Dept="FK_Dept";		 
		#endregion	
	}
	/// <summary>
    /// 到部门
	/// </summary>
    public class ByDept : Entity
    {
        #region 访问权限控制
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        #endregion 访问权限控制

        #region 基本属性
        /// <summary>
        /// 控制对象
        /// </summary>
        public string RefObj
        {
            get
            {
                return this.GetValStringByKey(ByDeptAttr.RefObj);
            }
            set
            {
                SetValByKey(ByDeptAttr.RefObj, value);
            }
        }
        public string FK_DeptT
        {
            get
            {
                return this.GetValRefTextByKey(ByDeptAttr.FK_Dept);
            }
        }
        /// <summary>
        ///部门
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStringByKey(ByDeptAttr.FK_Dept);
            }
            set
            {
                SetValByKey(ByDeptAttr.FK_Dept, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 到部门
        /// </summary> 
        public ByDept() { }
        /// <summary>
        /// 到部门
        /// </summary>
        /// <param name="RefObj"></param>
        /// <param name="fk_Dept"></param>
        public ByDept(string RefObj, string fk_Dept)
        {
            this.RefObj = RefObj;
            this.FK_Dept = fk_Dept;
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

                Map map = new Map("GPM_ByDept");
                map.EnDesc = "到部门";
                map.EnType = EnType.Dot2Dot;

                map.AddTBStringPK(ByDeptAttr.RefObj, null, "控制对象", false, false, 1, 15, 1);
                map.AddDDLEntitiesPK(ByDeptAttr.FK_Dept, null, "部门", new Depts(), true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
    /// 到部门 
	/// </summary>
	public class ByDepts : Entities
	{
		#region 构造
		/// <summary>
        /// 到部门
		/// </summary>
		public ByDepts()
		{
		}
		/// <summary>
        /// 到部门s
		/// </summary>
		public ByDepts(string DeptNo)
		{
			QueryObject qo = new QueryObject(this);
			qo.AddWhere(ByDeptAttr.FK_Dept, DeptNo);
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
				return new ByDept();
			}
		}	
		#endregion 

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<ByDept> ToJavaList()
        {
            return (System.Collections.Generic.IList<ByDept>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<ByDept> Tolist()
        {
            System.Collections.Generic.List<ByDept> list = new System.Collections.Generic.List<ByDept>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((ByDept)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
