using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.CN
{
	/// <summary>
	/// 片区
	/// </summary>
	public class PQAttr: EntityNoNameAttr
	{
		#region 基本属性
		public const string FK_SF="FK_SF";
		#endregion
	}
	/// <summary>
    /// 片区
	/// </summary>
	public class PQ :EntityNoName
	{	
		#region 基本属性
		#endregion 

		#region 构造函数
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
		/// 片区
		/// </summary>		
		public PQ(){}
		public PQ(string no):base(no)
		{
		}

		
		/// <summary>
		/// Map
		/// </summary>
		public override Map EnMap
		{
			get
			{
				if (this._enMap!=null) 
					return this._enMap;
				Map map = new Map("CN_PQ","片区");

				#region 基本属性 
				map.EnDBUrl =new DBUrl(DBUrlType.AppCenterDSN) ; 
				map.AdjunctType = AdjunctType.AllType ;  
				map.DepositaryOfMap=Depositary.Application; 
				map.DepositaryOfEntity=Depositary.None; 
				map.IsCheckNoLength=false;
				map.EnType=EnType.App;
				map.CodeStruct="4";
				#endregion

				#region 字段 
				map.AddTBStringPK(PQAttr.No,null,"编号",true,false,0,50,50);
				map.AddTBString(PQAttr.Name,null,"名称",true,false,0,50,200);
				#endregion

				this._enMap=map;
				return this._enMap;
			}
		}
        public override Entities GetNewEntities
        {
            get { return new PQs(); }
        }
		#endregion
	}
	/// <summary>
	/// 片区
	/// </summary>
	public class PQs : EntitiesNoName
	{
		#region 
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new PQ();
			}
		}	
		#endregion 

		#region 构造方法
		/// <summary>
		/// 片区s
		/// </summary>
		public PQs(){}
		#endregion
	}
	
}
