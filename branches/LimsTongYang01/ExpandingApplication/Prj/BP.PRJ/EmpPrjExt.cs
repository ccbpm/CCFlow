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
	public class EmpPrjExtAttr 
	{
		#region 基本属性
		/// <summary>
		/// 工作人员ID
		/// </summary>
		public const  string FK_Emp="FK_Emp";
		/// <summary>
		/// 项目组
		/// </summary>
		public const  string FK_Prj="FK_Prj";
        /// <summary>
        /// EmpPrjExt
        /// </summary>
        public const string EmpPrjExt = "EmpPrjExt";
        /// <summary>
        /// Name
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// 岗位集合
        /// </summary>
        public const string StationStrs = "StationStrs";
		#endregion	
	}
	/// <summary>
    /// 人员项目组 的摘要说明。
	/// </summary>
    public class EmpPrjExt : EntityMyPK
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
        /// <summary>
        /// 工作人员ID
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(EmpPrjExtAttr.FK_Emp);
            }
            set
            {
                SetValByKey(EmpPrjExtAttr.FK_Emp, value);
            }
        }
        public string StationStrs
        {
            get
            {
                return this.GetValStringByKey(EmpPrjExtAttr.StationStrs);
            }
            set
            {
                SetValByKey(EmpPrjExtAttr.StationStrs, value);
            }
        }
        
        public string FK_PrjT
        {
            get
            {
                return this.GetValRefTextByKey(EmpPrjExtAttr.FK_Prj);
            }
        }
        /// <summary>
        ///项目组
        /// </summary>
        public string FK_Prj
        {
            get
            {
                return this.GetValStringByKey(EmpPrjExtAttr.FK_Prj);
            }
            set
            {
                SetValByKey(EmpPrjExtAttr.FK_Prj, value);
            }
        }
        public string Name
        {
            get
            {
                return this.GetValStringByKey(EmpPrjExtAttr.Name);
            }
            set
            {
                SetValByKey(EmpPrjExtAttr.Name, value);
            }
        }
        #endregion

        #region 扩展属性

        #endregion

        #region 构造函数
        /// <summary>
        /// 工作人员项目组
        /// </summary> 
        public EmpPrjExt()
        {
        }
        /// <summary>
        /// 工作人员项目组对应
        /// </summary>
        /// <param name="_empoid">工作人员ID</param>
        /// <param name="wsNo">项目组编号</param> 	
        public EmpPrjExt(string _empoid, string wsNo)
        {
            this.FK_Emp = _empoid;
            this.FK_Prj = wsNo;
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

                Map map = new Map("Prj_EmpPrj");
                map.EnDesc = "人员项目组";
                map.EnType = EnType.Dot2Dot;

                map.AddMyPK();
                map.AddTBString(EmpPrjExtAttr.Name, null, "Name", false, false, 0, 3000, 20);

                map.AddDDLEntities(EmpPrjExtAttr.FK_Prj, null, "项目组", new Prjs(), true);
                map.AddDDLEntities(EmpPrjExtAttr.FK_Emp, null, "成员", new BP.WF.Port.WFEmps(), true);


                map.AddTBString(EmpPrjExtAttr.StationStrs, null, "岗位集合", true, true, 0, 4000, 20);

                map.AddSearchAttr(EmpPrjExtAttr.FK_Prj);

                map.AttrsOfOneVSM.Add(new EmpPrjStations(), new Stations(),
                    EmpPrjStationAttr.FK_EmpPrj, EmpPrjStationAttr.FK_Station,
                    DeptAttr.Name, DeptAttr.No, "岗位");

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion


        protected override bool beforeUpdate()
        {
            EmpPrjStations ens = new EmpPrjStations();
            ens.Retrieve(EmpPrjStationAttr.FK_EmpPrj, this.MyPK);

            string strs = "";
            foreach (EmpPrjStation en in ens)
            {
                strs += en.FK_StationT + ",";
            }

            this.StationStrs = strs;
            return base.beforeUpdate();
        }
    }
	/// <summary>
    /// 人员项目组
	/// </summary>
    public class EmpPrjExts : Entities
    {
        #region 构造
        /// <summary>
        /// 工作人员项目组
        /// </summary>
        public EmpPrjExts()
        {
        }
        /// <summary>
        /// 工作人员与项目组集合
        /// </summary>
        public EmpPrjExts(string GroupNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(EmpPrjExtAttr.FK_Prj, GroupNo);
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
                return new EmpPrjExt();
            }
        }
        #endregion
    }
}
