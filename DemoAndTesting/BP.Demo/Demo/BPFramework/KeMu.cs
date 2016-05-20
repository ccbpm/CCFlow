using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo.BPFramework
{
	/// <summary>
	/// 科目 属性
	/// </summary>
	public class KeMuAttr: EntityNoNameAttr
	{
	}
	/// <summary>
    /// 科目
	/// </summary>
	public class KeMu :EntityNoName
	{	
		#region 基本属性
		#endregion 

		#region 构造函数
        /// <summary>
        /// 实体的权限控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsDelete = true;
                uac.IsUpdate = true;
                uac.IsInsert = true;
                return uac;
            }
        }
		/// <summary>
		/// 科目
		/// </summary>		
		public KeMu(){}
        /// <summary>
        /// 科目
        /// </summary>
        /// <param name="no">编号</param>
		public KeMu(string no):base(no)
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
				Map map = new Map();

				#region 基本属性 
				map.EnDBUrl =new DBUrl(DBUrlType.AppCenterDSN) ; 
				map.PhysicsTable="Demo_KeMu";   //表
				map.DepositaryOfEntity=Depositary.None;  //实体村放位置.
                map.IsAllowRepeatName = true;
				map.IsCheckNoLength=false;
				map.EnDesc="科目";
				map.EnType=EnType.App;
				map.CodeStruct="3"; //让其编号为3位, 从001 到 999 .
				#endregion

				#region 字段 
                map.AddTBStringPK(KeMuAttr.No, null, "编号", true, true, 3, 3, 3);
				map.AddTBString(KeMuAttr.Name,null,"名称",true,false,0,50,200);
				#endregion

				this._enMap=map;
				return this._enMap;
			}
		}
        public override Entities GetNewEntities
        {
            get { return new KeMus(); }
        }
		#endregion
		 
	}
	/// <summary>
	/// 科目
	/// </summary>
	public class KeMus : EntitiesNoName
	{
		#region 重写
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new KeMu();
			}
		}	
		#endregion 

		#region 构造方法
		/// <summary>
		/// 科目s
		/// </summary>
		public KeMus(){}
		#endregion
	}
	
}
