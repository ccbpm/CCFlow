using System;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.GPM
{
	/// <summary>
	/// 到人员
	/// </summary>
	public class ByEmpAttr  
	{
		#region 基本属性
		/// <summary>
		/// 控制对象ID
		/// </summary>
		public const  string RefObj="RefObj";
		/// <summary>
		/// 人员
		/// </summary>
		public const  string FK_Emp="FK_Emp";		 
		#endregion	
	}
	/// <summary>
    /// 到人员
	/// </summary>
    public class ByEmp : Entity
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
        /// 控制对象ID
        /// </summary>
        public string RefObj
        {
            get
            {
                return this.GetValStringByKey(ByEmpAttr.RefObj);
            }
            set
            {
                SetValByKey(ByEmpAttr.RefObj, value);
            }
        }
        public string FK_EmpT
        {
            get
            {
                return this.GetValRefTextByKey(ByEmpAttr.FK_Emp);
            }
        }
        /// <summary>
        ///人员
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(ByEmpAttr.FK_Emp);
            }
            set
            {
                SetValByKey(ByEmpAttr.FK_Emp, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 到人员
        /// </summary> 
        public ByEmp() { }
        /// <summary>
        /// 到人员
        /// </summary>
        /// <param name="RefObj"></param>
        /// <param name="fk_Emp"></param>
        public ByEmp(string RefObj, string fk_Emp)
        {
            this.RefObj = RefObj;
            this.FK_Emp = fk_Emp;
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

                Map map = new Map("GPM_ByEmp");
                map.EnDesc = "到人员";
                map.EnType = EnType.Dot2Dot;

                map.AddTBStringPK(ByEmpAttr.RefObj, null, "控制对象", false, false, 1, 15, 1);
                map.AddDDLEntitiesPK(ByEmpAttr.FK_Emp, null, "人员", new Emps(), true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
    /// 到人员 
	/// </summary>
	public class ByEmps : Entities
	{
		#region 构造
		/// <summary>
        /// 到人员
		/// </summary>
		public ByEmps()
		{
		}
		/// <summary>
        /// 到人员s
		/// </summary>
		public ByEmps(string EmpNo)
		{
			QueryObject qo = new QueryObject(this);
			qo.AddWhere(ByEmpAttr.FK_Emp, EmpNo);
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
				return new ByEmp();
			}
		}	
		#endregion 

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<ByEmp> ToJavaList()
        {
            return (System.Collections.Generic.IList<ByEmp>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<ByEmp> Tolist()
        {
            System.Collections.Generic.List<ByEmp> list = new System.Collections.Generic.List<ByEmp>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((ByEmp)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
