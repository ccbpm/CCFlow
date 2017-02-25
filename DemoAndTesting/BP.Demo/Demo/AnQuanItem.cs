using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo
{
	/// <summary>
	/// 安全项目
	/// </summary>
	public class AnQuanItemAttr: EntityNoNameAttr
	{
		#region 基本属性
		public const  string FK_SF="FK_SF";
        public const string Addr = "Addr";

		#endregion
	}
	/// <summary>
    /// 安全项目
	/// </summary>
    public class AnQuanItem : EntityNoName
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
        /// 安全项目
        /// </summary>		
        public AnQuanItem() { }
        /// <summary>
        /// 安全项目
        /// </summary>
        /// <param name="no"></param>
        public AnQuanItem(string no)
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

                Map map = new Map("Demo_AnQuanItem","安全项目");

                #region 基本属性
                map.EnDBUrl = new DBUrl(DBUrlType.AppCenterDSN);
                map.AdjunctType = AdjunctType.AllType;
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.IsCheckNoLength = false;
                map.Java_SetEnType(EnType.App);
                map.Java_SetCodeStruct("2");
                #endregion

                #region 字段
                map.AddTBStringPK(AnQuanItemAttr.No, null, "编号", true, false, 2, 2, 50);
                map.AddTBString(AnQuanItemAttr.Name, null, "名称", true, false, 0, 100, 200);
                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        public override Entities GetNewEntities
        {
            get { return new AnQuanItems(); }
        }
        #endregion
    }
	/// <summary>
	/// 安全项目
	/// </summary>
	public class AnQuanItems : EntitiesNoName
	{
		#region 
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new AnQuanItem();
			}
		}	
		#endregion 

		#region 构造方法
		/// <summary>
		/// 安全项目s
		/// </summary>
		public AnQuanItems(){}
		#endregion
	}
	
}
