using System;
using System.Data;
using BP.DA;
using BP.Port;
using BP.En;

namespace BP.PRJ
{
	/// <summary>
	/// 人员项目组
	/// </summary>
	public class EmpPrjAttr
	{
		#region 基本属性
		/// <summary>
		/// 工作人员ID
		/// </summary>
		public const  string FK_Emp="FK_Emp";
		/// <summary>
		/// 项目组
		/// </summary>
		public const  string FK_Prj="FK_Prj";
        /// <summary>
        /// MyPK
        /// </summary>
        public const string MyPK = "MyPK";
        /// <summary>
        /// Name
        /// </summary>
        public const string Name = "Name";
		#endregion	
	}
	/// <summary>
    /// 人员项目组 的摘要说明。
	/// </summary>
    public class EmpPrj : Entity
    {
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
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
                return this.GetValStringByKey(EmpPrjAttr.FK_Emp);
            }
            set
            {
                SetValByKey(EmpPrjAttr.FK_Emp, value);
            }
        }
        public string FK_EmpT
        {
            get
            {
                return this.GetValRefTextByKey(EmpPrjAttr.FK_Emp);
            }
        }
        public string FK_PrjT
        {
            get
            {
                return this.GetValRefTextByKey(EmpPrjAttr.FK_Prj);
            }
        }
        /// <summary>
        ///项目组
        /// </summary>
        public string FK_Prj
        {
            get
            {
                return this.GetValStringByKey(EmpPrjAttr.FK_Prj);
            }
            set
            {
                SetValByKey(EmpPrjAttr.FK_Prj, value);
            }
        }
        public string MyPK
        {
            get
            {
                return this.GetValStringByKey(EmpPrjAttr.MyPK);
            }
            set
            {
                SetValByKey(EmpPrjAttr.MyPK, value);
            }
        }
        public string Name
        {
            get
            {
                return this.GetValStringByKey(EmpPrjAttr.Name);
            }
            set
            {
                SetValByKey(EmpPrjAttr.Name, value);
            }
        }
        #endregion

        #region 扩展属性

        #endregion

        #region 构造函数
        /// <summary>
        /// 工作人员项目组
        /// </summary> 
        public EmpPrj()
        {
        }
        /// <summary>
        /// 工作人员项目组对应
        /// </summary>
        /// <param name="_empoid">工作人员ID</param>
        /// <param name="wsNo">项目组编号</param> 	
        public EmpPrj(string _empoid, string wsNo)
        {
            this.FK_Emp = _empoid;
            this.FK_Prj = wsNo;
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

                Map map = new Map("Prj_EmpPrj");
                map.EnDesc = "人员项目组";
                map.EnType = EnType.Dot2Dot;

                map.AddTBString(EmpPrjAttr.MyPK, null, "MyPK", true, true, 0, 20, 20);
                map.AddTBString(EmpPrjAttr.Name, null, "Name", true, true, 0, 3000, 20);

                map.AddDDLEntitiesPK(EmpPrjAttr.FK_Emp, null, "操作员", new  Emps(), true);
                map.AddDDLEntitiesPK(EmpPrjAttr.FK_Prj, null, "项目组", new Prjs(), true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            Emp emp = new  Emp(this.FK_Emp);
            Prj p = new Prj(this.FK_Prj);

            this.MyPK = this.FK_Emp + "-" + this.FK_Prj;
            this.Name = p.Name + " - " + emp.Name;

            return base.beforeInsert();
        }
    }
	/// <summary>
    /// 人员项目组
	/// </summary>
    public class EmpPrjs : Entities
    {
        #region 构造
        /// <summary>
        /// 工作人员项目组
        /// </summary>
        public EmpPrjs()
        {
        }
        /// <summary>
        /// 工作人员与项目组集合
        /// </summary>
        public EmpPrjs(string GroupNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(EmpPrjAttr.FK_Prj, GroupNo);
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
                return new EmpPrj();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<EmpPrj> ToJavaList()
        {
            return (System.Collections.Generic.IList<EmpPrj>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<EmpPrj> Tolist()
        {
            System.Collections.Generic.List<EmpPrj> list = new System.Collections.Generic.List<EmpPrj>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((EmpPrj)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
