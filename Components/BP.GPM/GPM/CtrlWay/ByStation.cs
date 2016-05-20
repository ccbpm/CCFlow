using System;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.GPM
{
	/// <summary>
	/// 到岗位
	/// </summary>
	public class ByStationAttr  
	{
		#region 基本属性
		/// <summary>
		/// 控制对象
		/// </summary>
		public const  string RefObj="RefObj";
		/// <summary>
		/// 工作岗位
		/// </summary>
		public const  string FK_Station="FK_Station";		 
		#endregion	
	}
	/// <summary>
    /// 到岗位
	/// </summary>
    public class ByStation : Entity
    {
        #region 访问权限控制
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        #endregion 访问权限控制

        #region 基本属性
        /// <summary>
        /// 控制对象
        /// </summary>
        public string RefObj
        {
            get
            {
                return this.GetValStringByKey(ByStationAttr.RefObj);
            }
            set
            {
                SetValByKey(ByStationAttr.RefObj, value);
            }
        }
        public string FK_StationT
        {
            get
            {
                return this.GetValRefTextByKey(ByStationAttr.FK_Station);
            }
        }
        /// <summary>
        ///工作岗位
        /// </summary>
        public string FK_Station
        {
            get
            {
                return this.GetValStringByKey(ByStationAttr.FK_Station);
            }
            set
            {
                SetValByKey(ByStationAttr.FK_Station, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 到岗位
        /// </summary> 
        public ByStation() { }
        /// <summary>
        /// 到岗位
        /// </summary>
        /// <param name="RefObj"></param>
        /// <param name="FK_Station"></param>
        public ByStation(string RefObj, string FK_Station)
        {
            this.RefObj = RefObj;
            this.FK_Station = FK_Station;
            if (this.Retrieve() == 0)
                this.Insert();
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("GPM_ByStation");
                map.EnDesc = "到岗位";
                map.EnType = EnType.Dot2Dot;

                map.AddTBStringPK(ByStationAttr.RefObj, null, "控制对象", false, false, 1, 15, 1);
                map.AddDDLEntitiesPK(ByStationAttr.FK_Station, null, "工作岗位", new Stations(), true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
    /// 到岗位 
	/// </summary>
	public class ByStations : Entities
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
	{
		#region 构造
		/// <summary>
        /// 到岗位
		/// </summary>
		public ByStations()
		{
		}
		/// <summary>
        /// 到岗位s
		/// </summary>
		public ByStations(string stationNo)
		{
			QueryObject qo = new QueryObject(this);
			qo.AddWhere(ByStationAttr.FK_Station, stationNo);
			qo.DoQuery();
		}		 
		#endregion

		#region 方法
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new ByStation();
			}
		}	
		#endregion 

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        public System.Collections.Generic.IList<ByStation> ToJavaList()
        {
            return (System.Collections.Generic.IList<ByStation>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<ByStation> Tolist()
        {
            System.Collections.Generic.List<ByStation> list = new System.Collections.Generic.List<ByStation>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((ByStation)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
