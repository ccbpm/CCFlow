using System;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.GPM
{
	/// <summary>
	/// 部门岗位对应
	/// </summary>
	public class DeptStationAttr  
	{
		#region 基本属性
		/// <summary>
		/// 部门
		/// </summary>
		public const  string FK_Dept="FK_Dept";
		/// <summary>
		/// 岗位
		/// </summary>
		public const  string FK_Station="FK_Station";		 
		#endregion	
	}
	/// <summary>
    /// 部门岗位对应 的摘要说明。
	/// </summary>
    public class DeptStation : Entity
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
        /// 部门
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStringByKey(DeptStationAttr.FK_Dept);
            }
            set
            {
                SetValByKey(DeptStationAttr.FK_Dept, value);
            }
        }
        public string FK_StationT
        {
            get
            {
                return this.GetValRefTextByKey(DeptStationAttr.FK_Station);
            }
        }
        /// <summary>
        ///岗位
        /// </summary>
        public string FK_Station
        {
            get
            {
                return this.GetValStringByKey(DeptStationAttr.FK_Station);
            }
            set
            {
                SetValByKey(DeptStationAttr.FK_Station, value);
            }
        }
        #endregion

        #region 扩展属性

        #endregion

        #region 构造函数
        /// <summary>
        /// 工作部门岗位对应
        /// </summary> 
        public DeptStation() { }
        /// <summary>
        /// 工作人员岗位对应
        /// </summary>
        /// <param name="deptid">部门</param>
        /// <param name="stationid">岗位编号</param> 	
        public DeptStation(string deptid, string stationid)
        {
            this.FK_Dept = deptid;
            this.FK_Station = stationid;
            if (this.Retrieve(DeptStationAttr.FK_Dept, this.FK_Dept, DeptStationAttr.FK_Station, this.FK_Station) == 0)
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

                Map map = new Map("Port_DeptStation");
                map.EnDesc = "部门岗位对应";
                map.EnType = EnType.Dot2Dot; //实体类型，admin 系统管理员表，PowerAble 权限管理表,也是用户表,你要想把它加入权限管理里面请在这里设置。。

                map.AddTBStringPK(DeptStationAttr.FK_Dept, null, "部门", false, false, 1, 15, 1);
                map.AddDDLEntitiesPK(DeptStationAttr.FK_Station, null, "岗位", new Stations(), true);
                map.AddSearchAttr(DeptStationAttr.FK_Station);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
    /// 部门岗位对应 
	/// </summary>
	public class DeptStations : Entities
	{
		#region 构造
		/// <summary>
		/// 工作部门岗位对应
		/// </summary>
		public DeptStations()
		{
		}
		/// <summary>
		/// 工作人员与岗位集合
		/// </summary>
		public DeptStations(string stationNo)
		{
			QueryObject qo = new QueryObject(this);
			qo.AddWhere(DeptStationAttr.FK_Station, stationNo);
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
				return new DeptStation();
			}
		}	
		#endregion 

		#region 查询方法
		/// <summary>
		/// 岗位对应的节点
		/// </summary>
		/// <param name="stationNo">岗位编号</param>
		/// <returns>节点s</returns>
		public Emps GetHisEmps(string stationNo)
		{
			QueryObject qo = new QueryObject(this);
			qo.AddWhere(DeptStationAttr.FK_Station, stationNo );
			qo.addOrderBy(DeptStationAttr.FK_Station);
			qo.DoQuery();

			Emps ens = new Emps();
			foreach(DeptStation en in this)
				ens.AddEntity( new Emp(en.FK_Dept)) ;
			
			return ens;
		}
		/// <summary>
		/// 工作部门岗位对应s
		/// </summary>
		/// <param name="empId">empId</param>
		/// <returns>工作部门岗位对应s</returns> 
		public Stations GetHisStations(string empId)
		{
			Stations ens = new Stations();
			if ( Cash.IsExits("DeptStationsOf"+empId, Depositary.Application))
			{
				return (Stations)Cash.GetObjFormApplication("DeptStationsOf"+empId,null );				 
			}
			else
			{
				QueryObject qo = new QueryObject(this);
				qo.AddWhere(DeptStationAttr.FK_Dept,empId);
				qo.addOrderBy(DeptStationAttr.FK_Station);
				qo.DoQuery();				
				foreach(DeptStation en in this)
					ens.AddEntity( new Station(en.FK_Station) ) ;
				Cash.AddObj("DeptStationsOf"+empId,Depositary.Application,ens);
				return ens;
			}
		}
		#endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<DeptStation> ToJavaList()
        {
            return (System.Collections.Generic.IList<DeptStation>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<DeptStation> Tolist()
        {
            System.Collections.Generic.List<DeptStation> list = new System.Collections.Generic.List<DeptStation>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((DeptStation)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
