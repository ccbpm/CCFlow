using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.En
{
	/// <summary>
	/// 属性
	/// </summary>
	public class SimpleNoNameFixAttr : EntityNoNameAttr
	{}
	
	abstract public class SimpleNoNameFix : EntityNoName
	{		 
		#region 构造
		public override UAC HisUAC
		{
			get
			{
				UAC uac = new UAC();
				uac.OpenForSysAdmin();
				return uac;
			}
		}

		public SimpleNoNameFix()
		{
		}
		public SimpleNoNameFix(string _No) : base(_No){}
		public override Map EnMap
		{
			get
			{
				if (this._enMap!=null) return this._enMap;
				Map map = new Map(this.PhysicsTable);
				map.EnDesc=this.Desc;
				map.CodeStruct ="2" ;

				map.IsAutoGenerNo=true;

				map.DepositaryOfEntity=Depositary.Application;
				map.DepositaryOfMap=Depositary.Application;
				map.EnType=EnType.App;

				map.CodeStruct="3";
				map.IsAutoGenerNo=true;
				
 				map.AddTBStringPK(SimpleNoNameFixAttr.No,null,"编号",true,true,1,30,3);
                map.AddTBString(SimpleNoNameFixAttr.Name,null,"名称",true,false,1,60,500);
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

		#region  重写基类的方法。
		#endregion
	}
	/// <summary>
	/// SimpleNoNameFixs
	/// </summary>
	abstract public class SimpleNoNameFixs : EntitiesNoName
	{
		/// <summary>
		/// SimpleNoNameFixs
		/// </summary>
		public SimpleNoNameFixs()
		{
		}


	}
}
