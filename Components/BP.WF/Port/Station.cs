using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.WF.Port
{	 
	/// <summary>
	/// 岗位属性
	/// </summary>
    public class StationAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 岗位类型
        /// </summary>
        public const string FK_StationType = "FK_StationType";
    }
	/// <summary>
	/// 岗位
	/// </summary>
    public class Station : EntityNoName
    {
        #region 实现基本的方方法
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 岗位
        /// </summary> 
        public Station()
        {
        }
        /// <summary>
        /// 岗位
        /// </summary>
        /// <param name="_No"></param>
        public Station(string _No) : base(_No) { }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Port_Station", "岗位");
                map.Java_SetEnType(EnType.Admin);
             
                map.Java_SetDepositaryOfEntity(Depositary.Application);
                map.Java_SetCodeStruct("2");; // 最大级别是7.

                map.AddTBStringPK(StationAttr.No, null, "编号", true, true, 2, 2, 2);
                map.AddTBString(StationAttr.Name, null, "名称", true, false, 2, 50, 250);
                map.AddDDLEntities(StationAttr.FK_StationType, null, "岗位类型", new StationTypes(), true);

                //查询条件.
                map.AddSearchAttr(StationAttr.FK_StationType);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	 /// <summary>
	 /// 岗位s
	 /// </summary>
	public class Stations : EntitiesNoName
	{
		/// <summary>
		/// 岗位
		/// </summary>
        public Stations() { }
		/// <summary>
		/// 得到它的 Entity
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new Station();
			}
		}
	}
}
