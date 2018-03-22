using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;
namespace BP.CN
{
	/// <summary>
	/// 城市属性
	/// </summary>
    public class CityAttr : EntityNoNameAttr
    {
        #region 基本属性
        public const string FK_PQ = "FK_PQ";
        public const string FK_SF = "FK_SF";
        public const string Grade = "Grade";
        public const string Names = "Names";
        public const string PinYin = "PinYin";
        #endregion
    }
	/// <summary>
    /// 城市
	/// </summary>
    public class City : EntityNoName
    {
        #region 基本属性
        public string Names
        {
            get
            {
                return this.GetValStrByKey(CityAttr.Names);
            }
        }
        public string FK_PQ
        {
            get
            {
                return this.GetValStrByKey(CityAttr.FK_PQ);
            }
            set
            {
                this.SetValByKey(CityAttr.FK_PQ, value);
            }
        }
        public string FK_SF
        {
            get
            {
                return this.GetValStrByKey(CityAttr.FK_SF);
            }
            set
            {
                this.SetValByKey(CityAttr.FK_SF, value);
            }
        }
        public string PinYin
        {
            get
            {
                return this.GetValStrByKey(CityAttr.PinYin);
            }
            set
            {
                this.SetValByKey(CityAttr.PinYin, value);
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
        /// 城市
        /// </summary>		
        public City() { }
        public City(string no)
            : base(no)
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
                Map map = new Map("CN_City","城市");

                #region 基本属性
                map.EnDBUrl = new DBUrl(DBUrlType.AppCenterDSN);
                map.AdjunctType = AdjunctType.AllType;
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.IsCheckNoLength = false;
                map.Java_SetEnType(EnType.App);
                map.Java_SetCodeStruct("4");
                #endregion

                #region 字段
                map.AddTBStringPK(CityAttr.No, null, "编号", true, false, 0, 50, 50);
                map.AddTBString(CityAttr.Name, null, "名称", true, false, 0, 50, 200);
                map.AddTBString(CityAttr.Names, null, "小名", true, false, 0, 50, 200);
                map.AddTBInt(CityAttr.Grade, 0, "Grade", false, false);

                map.AddDDLEntities(CityAttr.FK_SF, null, "省份", new SFs(), true);
                map.AddDDLEntities(CityAttr.FK_PQ, null, "片区", new PQs(), true);
                map.AddTBString(CityAttr.PinYin, null, "搜索拼音", true, false, 0, 200, 200);

                map.AddTBString(CityAttr.PinYin, null, "拼音", true, false, 0, 200, 200);
                
                map.AddSearchAttr(CityAttr.FK_SF);
                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

    }
	/// <summary>
	/// 城市s
	/// </summary>
	public class Citys : EntitiesNoName
	{
		#region 
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new City();
			}
		}	
		#endregion 

		#region 构造方法
		/// <summary>
		/// 城市s
		/// </summary>
		public Citys(){}

        /// <summary>
        /// 城市s
        /// </summary>
        /// <param name="sf">省份</param>
        public Citys(string sf)
        {
            this.Retrieve(CityAttr.FK_SF, sf);
        }
		#endregion
	}
	
}
