using System;
using System.Data;
using BP.DA;
using BP.Port;
using BP.En;
 
namespace BP.PRJ 
{
	/// <summary>
	/// 人员项目组 
	/// </summary>
	public class EmpPrjStationAttr
	{
		#region 基本属性
		/// <summary>
		/// 项目人员
		/// </summary>
        public const string FK_EmpPrj = "FK_EmpPrj";
		/// <summary>
		/// 项目组
		/// </summary>
		public const  string FK_Station="FK_Station";
        /// <summary>
        /// FK_Emp
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// FK_Prj
        /// </summary>
        public const string FK_Prj = "FK_Prj";
		#endregion
	}
	/// <summary>
    /// 人员项目组 的摘要说明。
	/// </summary>
    public class EmpPrjStation : Entity
    {
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }

        #region 基本属性
        public string FK_Prj
        {
            get
            {
                return this.GetValStringByKey(EmpPrjStationAttr.FK_Prj);
            }
            set
            {
                SetValByKey(EmpPrjStationAttr.FK_Prj, value);
            }
        }
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(EmpPrjStationAttr.FK_Emp);
            }
            set
            {
                SetValByKey(EmpPrjStationAttr.FK_Emp, value);
            }
        }
        /// <summary>
        /// 项目人员
        /// </summary>
        public string FK_EmpPrj
        {
            get
            {
                return this.GetValStringByKey(EmpPrjStationAttr.FK_EmpPrj);
            }
            set
            {
                SetValByKey(EmpPrjStationAttr.FK_EmpPrj, value);
            }
        }
        public string FK_StationT
        {
            get
            {
                return this.GetValRefTextByKey(EmpPrjStationAttr.FK_Station);
            }
        }
        /// <summary>
        ///项目组
        /// </summary>
        public string FK_Station
        {
            get
            {
                return this.GetValStringByKey(EmpPrjStationAttr.FK_Station);
            }
            set
            {
                SetValByKey(EmpPrjStationAttr.FK_Station, value);
            }
        }
        #endregion

        #region 扩展属性
        #endregion

        #region 构造函数
        /// <summary>
        /// 工作人员项目组
        /// </summary> 
        public EmpPrjStation() { }
        /// <summary>
        /// 工作人员项目组对应
        /// </summary>
        /// <param name="_empoid">项目人员</param>
        /// <param name="wsNo">项目组编号</param> 	
        public EmpPrjStation(string _empoid, string wsNo)
        {
            this.FK_EmpPrj = _empoid;
            this.FK_Station = wsNo;
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

                Map map = new Map("Prj_EmpPrjStation");
                map.EnDesc = "人员项目组";
                map.EnType = EnType.Dot2Dot;
                map.AddTBStringPK(EmpPrjStationAttr.FK_EmpPrj, null, "FK_EmpPrj", true, true, 0, 20, 20);
                map.AddDDLEntitiesPK(EmpPrjStationAttr.FK_Station, null, "岗位", new Stations(), true);
                map.AddTBString(EmpPrjStationAttr.FK_Emp, null, "FK_Emp", true, true, 0, 20, 20);
                map.AddTBString(EmpPrjStationAttr.FK_Prj, null, "FK_Prj", true, true, 0, 20, 20);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
        protected override bool beforeInsert()
        {
            string[] strs = this.FK_EmpPrj.Split('-');
            this.FK_Prj = strs[1];
            this.FK_Emp = strs[0];
            return base.beforeInsert();
        }
    }
	/// <summary>
    /// 人员项目组
	/// </summary>
    public class EmpPrjStations : Entities
    {
        #region 构造
        /// <summary>
        /// 工作人员项目组
        /// </summary>
        public EmpPrjStations()
        {
        }
        /// <summary>
        /// 工作人员与项目组集合
        /// </summary>
        public EmpPrjStations(string GroupNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(EmpPrjStationAttr.FK_Station, GroupNo);
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
                return new EmpPrjStation();
            }
        }
        #endregion
    }
}
