using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo.BPFramework
{
	/// <summary>
	/// 固定资产类别 属性
	/// </summary>
	public class ZCLBAttr: EntityNoNameAttr
	{
        /// <summary>
        /// 班主任
        /// </summary>
        public const string BZR = "BZR";
	}
	/// <summary>
    /// 固定资产类别
	/// </summary>
	public class ZCLB :BP.En.EntityNoName
	{	
		#region 基本属性
        /// <summary>
        /// 班主任
        /// </summary>
        public string BZR
        {
            get
            {
                return this.GetValStrByKey(ZCLBAttr.BZR);
            }
            set
            {
                this.SetValByKey(ZCLBAttr.BZR, value);
            }
        }
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
		/// 固定资产类别
		/// </summary>		
		public ZCLB(){}
		public ZCLB(string no):base(no)
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
             //   map.EnDBUrl = new DBUrl();
				map.PhysicsTable="Demo_ZCLB";   //表
				map.DepositaryOfEntity=Depositary.None;  //实体村放位置.
                map.IsAllowRepeatName = true;
				map.EnDesc="固定资产类别";
				map.EnType=EnType.App;
				map.CodeStruct="3"; //让其编号为3位, 从001 到 999 .
				#endregion

				#region 字段 
                map.AddTBStringPK(ZCLBAttr.No, null, "编号", true, true, 3, 3, 3);
				map.AddTBString(ZCLBAttr.Name,null,"名称",true,false,0,50,200);
                map.AddTBString(ZCLBAttr.BZR, null, "班主任", true, false, 0, 50, 200);
				#endregion

				this._enMap=map;
				return this._enMap;
			}
		}
        public override Entities GetNewEntities
        {
            get { return new ZCLBs(); }
        }
		#endregion
	}
	/// <summary>
	/// 固定资产类别s
	/// </summary>
	public class ZCLBs : BP.En.EntitiesNoName
	{
		#region 重写
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new ZCLB();
			}
		}
		#endregion 

		#region 构造方法
		/// <summary>
		/// 固定资产类别s
		/// </summary>
		public ZCLBs(){}
		#endregion
	}
	
}
