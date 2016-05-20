using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.WF.Rpt
{
	/// <summary>
	/// 报表部门
	/// </summary>
	public class RptDeptAttr  
	{
		#region 基本属性
		/// <summary>
		/// 报表ID
		/// </summary>
		public const  string FK_Rpt="FK_Rpt";
		/// <summary>
		/// 部门
		/// </summary>
		public const  string FK_Dept="FK_Dept";		 
		#endregion	
	}
	/// <summary>
	/// RptDept 的摘要说明。
	/// </summary>
	public class RptDept :Entity
	{
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
		/// 报表ID
		/// </summary>
		public string FK_Rpt
		{
			get
			{
				return this.GetValStringByKey(RptDeptAttr.FK_Rpt);
			}
			set
			{
				SetValByKey(RptDeptAttr.FK_Rpt,value);
			}
		}
        public string FK_DeptT
        {
            get
            {
                return this.GetValRefTextByKey(RptDeptAttr.FK_Dept);
            }
        }
		/// <summary>
		///部门
		/// </summary>
		public string FK_Dept
		{
			get
			{
				return this.GetValStringByKey(RptDeptAttr.FK_Dept);
			}
			set
			{
				SetValByKey(RptDeptAttr.FK_Dept,value);
			}
		}		  
		#endregion

		#region 扩展属性
		 
		#endregion		

		#region 构造函数
		/// <summary>
		/// 报表岗位
		/// </summary> 
		public RptDept(){}
		/// <summary>
		/// 报表部门对应
		/// </summary>
		/// <param name="_empoid">报表ID</param>
		/// <param name="wsNo">部门编号</param> 	
		public RptDept(string _empoid,string wsNo)
		{
			this.FK_Rpt  = _empoid;
			this.FK_Dept = wsNo ;
			if (this.Retrieve()==0)
				this.Insert();
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
				
				Map map = new Map("Sys_RptDept");
				map.EnDesc="报表部门对应信息";	
				map.EnType=EnType.Dot2Dot;

                map.AddTBStringPK(RptDeptAttr.FK_Rpt, null, "报表", false, false, 1, 15, 1);
				map.AddDDLEntitiesPK(RptDeptAttr.FK_Dept,null,"部门",new Depts(),true);

				this._enMap=map;
				return this._enMap;
			}
		}
		#endregion
	}
	/// <summary>
	/// 报表部门 
	/// </summary>
	public class RptDepts : Entities
	{
		#region 构造
		/// <summary>
		/// 报表与部门集合
		/// </summary>
		public RptDepts(){}
		#endregion

		#region 方法
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new RptDept();
			}
		}	
		#endregion 

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<RptDept> ToJavaList()
        {
            return (System.Collections.Generic.IList<RptDept>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<RptDept> Tolist()
        {
            System.Collections.Generic.List<RptDept> list = new System.Collections.Generic.List<RptDept>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((RptDept)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
