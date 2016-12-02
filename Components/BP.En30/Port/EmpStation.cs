using System;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.Port
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
    public class EmpStation : EntityMM
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

        #region 构造函数
        /// <summary>
        /// 工作人员岗位
        /// </summary> 
        public EmpStation() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Port_EmpStation");
                map.EnDesc = "人员岗位";
                map.EnType = EnType.Dot2Dot;

                map.AddDDLEntitiesPK(EmpStationAttr.FK_Emp, null, "操作员", new Emps(), true);
                map.AddDDLEntitiesPK(EmpStationAttr.FK_Station, null, "工作岗位", new Stations(), true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
    /// 人员岗位s 
	/// </summary>
	public class EmpStations : EntitiesMM
	{
		#region 构造
		/// <summary>
		/// 工作人员岗位
		/// </summary>
		public EmpStations()
		{
		}
        /// <summary>
        /// 工作人员岗位
        /// </summary>
        public EmpStations(string empNo)
        {
            if (BP.Sys.SystemConfig.OSDBSrc == Sys.OSDBSrc.Database)
            {
                this.Retrieve("FK_Emp", BP.Web.WebUser.No);
                return;
            }

            if (BP.Sys.SystemConfig.OSDBSrc == Sys.OSDBSrc.WebServices)
            {
                var ws = DataType.GetPortalInterfaceSoapClientInstance();
                DataTable dt = ws.GetEmpHisStations(Web.WebUser.No);
                foreach (DataRow dr in dt.Rows)
                {
                    EmpStation es = new EmpStation();
                    es.FK_Emp = BP.Web.WebUser.No;
                    es.FK_Station = dr[0].ToString();
                    es.Row["FK_StationText"] = dr[1].ToString();
                    this.AddEntity(es);
                }
                return;
            }
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

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<EmpStation> ToJavaList()
        {
            return (System.Collections.Generic.IList<EmpStation>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<EmpStation> Tolist()
        {
            System.Collections.Generic.List<EmpStation> list = new System.Collections.Generic.List<EmpStation>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((EmpStation)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
