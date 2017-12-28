using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.En
{
	/// <summary>
	/// 属性
	/// </summary>
	public class SimpleNoNameAttr : EntityNoNameAttr
	{}
	
	abstract public class SimpleNoName: EntityNoName
	{
		#region 构造
		public SimpleNoName()
		{
			//	this.No  = this.GenerNewNo ;
		}
		 
		protected SimpleNoName(string _No) : base(_No){}
		public override Map EnMap
		{
			get
			{
				if (this._enMap!=null) return this._enMap;
				Map map = new Map(this.PhysicsTable);
				map.EnDesc=this.Desc;
				map.CodeStruct ="3" ;
				map.IsAutoGenerNo=true;
				map.DepositaryOfEntity=Depositary.Application;
				map.DepositaryOfMap=Depositary.Application;

                map.AddTBStringPK(SimpleNoNameAttr.No, null, "编号", true, true, 1, 20, 10);
                map.AddTBString(SimpleNoNameAttr.Name, null, "名称", true, false, 0, 400, 100);
				 
				this._enMap=map;
				return this._enMap;
			}
		}		 
		#endregion 		

		#region 需要子类重写的方法
		/// <summary>
		/// 指定表
		/// </summary>
		public abstract string PhysicsTable{get;}
		/// <summary>
		/// 描述
		/// </summary>
		public abstract string Desc{get;}
		#endregion 
	}
	/// <summary>
	/// SimpleNoNames
	/// </summary>
	abstract public class SimpleNoNames : EntitiesNoName
	{
		/// <summary>
		/// SimpleNoNames
		/// </summary>
		public SimpleNoNames()
		{}
	}
}
