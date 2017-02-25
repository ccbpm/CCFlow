using System;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.Demo.YS
{
	/// <summary>
	/// 项目vs科目
	/// </summary>
	public class PrjKMAttr  
	{
		#region 基本属性
		/// <summary>
		/// 工作人员ID
		/// </summary>
		public const  string FK_Prj="FK_Prj";
		/// <summary>
		/// 工作岗位
		/// </summary>
		public const  string FK_KM="FK_KM";		 
		#endregion	
	}
	/// <summary>
    /// 项目vs科目 的摘要说明。
	/// </summary>
    public class PrjKM : Entity
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
        public string FK_Prj
        {
            get
            {
                return this.GetValStringByKey(PrjKMAttr.FK_Prj);
            }
            set
            {
                SetValByKey(PrjKMAttr.FK_Prj, value);
            }
        }
        public string FK_KMT
        {
            get
            {
                return this.GetValRefTextByKey(PrjKMAttr.FK_KM);
            }
        }
        /// <summary>
        ///工作岗位
        /// </summary>
        public string FK_KM
        {
            get
            {
                return this.GetValStringByKey(PrjKMAttr.FK_KM);
            }
            set
            {
                SetValByKey(PrjKMAttr.FK_KM, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 工作项目vs科目
        /// </summary> 
        public PrjKM() { }
        /// <summary>
        /// 工作人员工作岗位对应
        /// </summary>
        /// <param name="FK_Prj">工作人员ID</param>
        /// <param name="FK_KM">工作岗位编号</param> 	
        public PrjKM(string FK_Prj, string FK_KM)
        {
            this.FK_Prj = FK_Prj;
            this.FK_KM = FK_KM;
            this.Retrieve();
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

                Map map = new Map("Demo_YS_PrjKM", "项目vs科目");
                map.Java_SetEnType(EnType.Dot2Dot);

              //  map.AddDDLEntitiesPK(PrjKMAttr.FK_Prj, null, "操作员", new Emps(), true);
                map.AddTBStringPK(PrjKMAttr.FK_Prj, null, "项目", true, false, 0, 100, 100);
                map.AddDDLEntitiesPK(PrjKMAttr.FK_KM, null, "科目", new KMs(), true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
    /// 项目vs科目 
	/// </summary>
	public class PrjKMs : Entities
	{
		#region 构造
		/// <summary>
		/// 工作项目vs科目
		/// </summary>
		public PrjKMs()
		{
		}
		/// <summary>
		/// 工作人员与工作岗位集合
		/// </summary>
		public PrjKMs(string stationNo)
		{
			QueryObject qo = new QueryObject(this);
			qo.AddWhere(PrjKMAttr.FK_KM, stationNo);
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
				return new PrjKM();
			}
		}	
		#endregion 
	}
}
