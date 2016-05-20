using System;
using System.Data;
using BP.DA;
using BP.En;


namespace BP.Port
{
	/// <summary>
	/// 人员部门对应 -属性
	/// </summary>
	public class EmpDeptAttr  
	{
		#region 基本属性
		/// <summary>
		/// 工作人员ID
		/// </summary>
		public const  string FK_Emp="FK_Emp";
		/// <summary>
		/// 部门
		/// </summary>
		public const  string FK_Dept="FK_Dept";		 
		#endregion	
	}
	/// <summary>
    /// 人员部门对应
	/// </summary>
	public class EmpDept :EntityMM
    {
        /// <summary>
        /// 重写实现权限.
        /// </summary>
        public override UAC HisUAC
		{
			get
			{
				UAC uac = new UAC();
				if (BP.Web.WebUser.No== "admin"   )
				{
					uac.IsView=true;
					uac.IsDelete=true;
					uac.IsInsert=true;
					uac.IsUpdate=true;
					uac.IsAdjunct=true;
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
				return this.GetValStringByKey(EmpDeptAttr.FK_Emp);
			}
			set
			{
				SetValByKey(EmpDeptAttr.FK_Emp,value);
			}
		}
        public string FK_DeptT
        {
            get
            {
                return this.GetValRefTextByKey(EmpDeptAttr.FK_Dept);
            }
        }
		/// <summary>
		///部门
		/// </summary>
		public string FK_Dept
		{
			get
			{
				return this.GetValStringByKey(EmpDeptAttr.FK_Dept);
			}
			set
			{
				SetValByKey(EmpDeptAttr.FK_Dept,value);
			}
		}		  
		#endregion
	 

		#region 构造函数
		/// <summary>
		/// 工作人员岗位
		/// </summary> 
		public EmpDept()
        {
        }
		/// <summary>
		/// 重写基类方法
		/// </summary>
		public override Map EnMap
		{
			get
			{
				if (this._enMap!=null) 
					return this._enMap;
				
				Map map = new Map("Port_EmpDept");
				map.EnDesc="工作人员部门对应信息";	
				map.EnType=EnType.Dot2Dot; //实体类型，admin 系统管理员表，PowerAble 权限管理表,也是用户表,你要想把它加入权限管理里面请在这里设置。。

             //   map.AddTBStringPK(EmpDeptAttr.FK_Emp, null, "Emp", false, false, 1, 15,1);
                //map.AddTBStringPK(EmpDeptAttr.FK_Dept, null, "Dept", false, false, 1, 15,1);
                //map.AddDDLEntitiesPK(EmpDeptAttr.FK_Emp,null,"操作员",new Emps(),true);

                map.AddTBStringPK(EmpDeptAttr.FK_Emp, null, "操作员", false, false, 1, 20, 1);
				map.AddDDLEntitiesPK(EmpDeptAttr.FK_Dept,null,"部门",new Depts(),true);


                //map.AddDDLEntitiesPK(EmpDeptAttr.FK_Emp,0, DataType.AppInt,"操作员",new 县局(),"OID","Name",true);
				//map.AddSearchAttr(EmpDeptAttr.FK_Emp);
				//map.AddSearchAttr(EmpDeptAttr.FK_Dept);

				this._enMap=map;
				return this._enMap;
			}
		}
		#endregion
	}
	/// <summary>
    /// 人员部门对应s -集合 
	/// </summary>
	public class EmpDepts : EntitiesMM
	{
		#region 构造
		/// <summary>
		/// 工作人员与部门集合
		/// </summary>
		public EmpDepts(){}
		#endregion

		#region 重写方法
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new EmpDept();
			}
		}	
		#endregion 

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<EmpDept> ToJavaList()
        {
            return (System.Collections.Generic.IList<EmpDept>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<EmpDept> Tolist()
        {
            System.Collections.Generic.List<EmpDept> list = new System.Collections.Generic.List<EmpDept>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((EmpDept)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
		 
				
	}
	
}
