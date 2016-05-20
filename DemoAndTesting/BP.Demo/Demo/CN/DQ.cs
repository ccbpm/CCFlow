using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;
namespace BP.CN
{
	/// <summary>
	/// 洲地市
	/// </summary>
	public class ZDSAttr: EntityNoNameAttr
	{
		#region 基本属性
		public const  string FK_PQ="FK_PQ";
        public const string FK_ZDS = "FK_ZDS";
        public const string FK_SF = "FK_SF";
        public const string NameS = "NameS";


		#endregion
	}
	/// <summary>
    /// 洲地市
	/// </summary>
	public class ZDS :EntityNoName
	{	
		#region 基本属性
        public string NameS
        {
            get
            {
                return this.GetValStrByKey(ZDSAttr.NameS);
            }
        }
		 
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
		/// 洲地市
		/// </summary>		
		public ZDS(){}
		public ZDS(string no):base(no)
		{
		}
		/// <summary>
		/// Map
		/// </summary>
		public override Map EnMap
		{
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map();

                #region 基本属性
                map.EnDBUrl = new DBUrl(DBUrlType.AppCenterDSN);
                map.PhysicsTable = "CN_ZDS";
                map.AdjunctType = AdjunctType.AllType;
                map.DepositaryOfMap = Depositary.Application;
                map.DepositaryOfEntity = Depositary.None;
                map.IsCheckNoLength = false;
                map.EnDesc = "洲地市";
                map.EnType = EnType.App;
                map.CodeStruct = "4";
                #endregion

                #region 字段
                map.AddTBStringPK(ZDSAttr.No, null, "编号", true, false, 0, 50, 50);
                map.AddTBString(ZDSAttr.Name, null, "名称", true, false, 0, 50, 200);
                map.AddTBString(ZDSAttr.NameS, null, "名称s", true, false, 0, 50, 200);

                map.AddTBString(ZDSAttr.FK_SF, null, "FK_SF", true, false, 0, 50, 200);

                map.AddDDLEntities(ZDSAttr.FK_PQ, null, "片区", new PQs(), true);
              //  map.AddDDLEntities(ZDSAttr.FK_ZDS, null, "省份", new SFs(), true);
                #endregion

                this._enMap = map;
                return this._enMap;
            }
		}
		#endregion
		 
	}
	/// <summary>
	/// 洲地市
	/// </summary>
	public class ZDSs : EntitiesNoName
	{
		#region 
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new ZDS();
			}
		}	
		#endregion 

		#region 构造方法
		/// <summary>
		/// 洲地市s
		/// </summary>
		public ZDSs(){}

        public ZDSs(string sf)
        {
            this.Retrieve(ZDSAttr.FK_SF, sf);
        }

		#endregion
	}
	
}
