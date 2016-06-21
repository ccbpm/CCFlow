using System;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.WF.Port
{
	/// <summary>
	/// 人员岗位
	/// </summary>
	public class EmpStationAttr  
	{
		#region 基本属性
		/// <summary>
		/// 工作人员ID
		/// </summary>
		public const  string FK_Emp="FK_Emp";
		/// <summary>
		/// 工作岗位
		/// </summary>
		public const  string FK_Station="FK_Station";		 
		#endregion	
	}
	/// <summary>
    /// 人员岗位 的摘要说明。
	/// </summary>
    public class EmpStation : Entity
    {
        #region 基本属性
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
        /// <summary>
        /// 工作人员ID
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(EmpStationAttr.FK_Emp);
            }
            set
            {
                SetValByKey(EmpStationAttr.FK_Emp, value);
            }
        }
        public string FK_StationT
        {
            get
            {
                return this.GetValRefTextByKey(EmpStationAttr.FK_Station);
            }
        }
        /// <summary>
        ///工作岗位
        /// </summary>
        public string FK_Station
        {
            get
            {
                return this.GetValStringByKey(EmpStationAttr.FK_Station);
            }
            set
            {
                SetValByKey(EmpStationAttr.FK_Station, value);
            }
        }
        #endregion

        #region 扩展属性

        #endregion

        #region 构造函数
        /// <summary>
        /// 工作人员岗位
        /// </summary> 
        public EmpStation() { }
        /// <summary>
        /// 工作人员工作岗位对应
        /// </summary>
        /// <param name="_empoid">工作人员ID</param>
        /// <param name="wsNo">工作岗位编号</param> 	
        public EmpStation(string _empoid, string wsNo)
        {
            this.FK_Emp = _empoid;
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

                Map map = new Map("Port_EmpStation", "人员岗位");
                

                map.AddTBStringPK(EmpStationAttr.FK_Emp, null, "操作员", false, false, 1, 15, 1);
                map.AddDDLEntitiesPK(EmpStationAttr.FK_Station, null, "工作岗位", new Stations(), true);
                map.AddSearchAttr(EmpStationAttr.FK_Station);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
    /// 人员岗位 
	/// </summary>
	public class EmpStations : Entities
	{
		#region 构造
		/// <summary>
		/// 工作人员岗位
		/// </summary>
		public EmpStations()
		{
		}
		/// <summary>
		/// 工作人员与工作岗位集合
		/// </summary>
		public EmpStations(string stationNo)
		{
			QueryObject qo = new QueryObject(this);
			qo.AddWhere(EmpStationAttr.FK_Station, stationNo);
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
				return new EmpStation();
			}
		}	
		#endregion 

		#region 查询方法
		/// <summary>
		/// 工作岗位对应的节点
		/// </summary>
		/// <param name="stationNo">工作岗位编号</param>
		/// <returns>节点s</returns>
		public Emps GetHisEmps(string stationNo)
		{
			QueryObject qo = new QueryObject(this);
			qo.AddWhere(EmpStationAttr.FK_Station, stationNo );
			qo.addOrderBy(EmpStationAttr.FK_Station);
			qo.DoQuery();

			Emps ens = new Emps();
			foreach(EmpStation en in this)
				ens.AddEntity( new Emp(en.FK_Emp)) ;
			
			return ens;
		}
		/// <summary>
		/// 工作人员岗位s
		/// </summary>
		/// <param name="empId">empId</param>
		/// <returns>工作人员岗位s</returns> 
		public Stations GetHisStations(string empId)
		{
			Stations ens = new Stations();
			if ( Cash.IsExits("EmpStationsOf"+empId, Depositary.Application))
			{
				return (Stations)Cash.GetObjFormApplication("EmpStationsOf"+empId,null );				 
			}
			else
			{
				QueryObject qo = new QueryObject(this);
				qo.AddWhere(EmpStationAttr.FK_Emp,empId);
				qo.addOrderBy(EmpStationAttr.FK_Station);
				qo.DoQuery();				
				foreach(EmpStation en in this)
					ens.AddEntity( new Station(en.FK_Station) ) ;
				Cash.AddObj("EmpStationsOf"+empId,Depositary.Application,ens);
				return ens;
			}
		}
		#endregion
	}
}
