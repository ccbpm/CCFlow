using System;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.GPM
{
	/// <summary>
	/// 部门岗位人员对应
	/// </summary>
	public class DeptEmpStationAttr
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
        /// <summary>
        /// 人员
        /// </summary>
        public const string FK_Emp = "FK_Emp";
		#endregion	
	}
	/// <summary>
    /// 部门岗位人员对应 的摘要说明。
	/// </summary>
    public class DeptEmpStation : EntityMyPK
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
        /// 人员
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(DeptEmpStationAttr.FK_Emp);
            }
            set
            {
                SetValByKey(DeptEmpStationAttr.FK_Emp, value);
                this.MyPK = this.FK_Dept + "_" + this.FK_Emp+"_"+this.FK_Station;
            }
        }
        /// <summary>
        /// 部门
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStringByKey(DeptEmpStationAttr.FK_Dept);
            }
            set
            {
                SetValByKey(DeptEmpStationAttr.FK_Dept, value);
                this.MyPK = this.FK_Dept + "_" + this.FK_Emp + "_" + this.FK_Station;
            }
        }
        public string FK_StationT
        {
            get
            {
                //return this.GetValRefTextByKey(DeptEmpStationAttr.FK_Station);

                return this.GetValStringByKey(DeptEmpStationAttr.FK_Station);
            }
        }
        /// <summary>
        ///岗位
        /// </summary>
        public string FK_Station
        {
            get
            {
                return this.GetValStringByKey(DeptEmpStationAttr.FK_Station);
            }
            set
            {
                SetValByKey(DeptEmpStationAttr.FK_Station, value);
                this.MyPK = this.FK_Dept + "_" + this.FK_Emp + "_" + this.FK_Station;
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 工作部门岗位人员对应
        /// </summary> 
        public DeptEmpStation() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Port_DeptEmpStation");
                map.EnDesc = "部门岗位人员对应";

              //  map.AddMyPK();

                map.AddTBStringPK("MyPK", null, "主键MyPK", false, true, 1, 150, 10);


                map.AddTBString(DeptEmpStationAttr.FK_Dept, null, "部门", false, false, 1, 50, 1);
                map.AddTBString(DeptEmpStationAttr.FK_Station, null, "岗位", false, false, 1, 50, 1);
                map.AddTBString(DeptEmpStationAttr.FK_Emp, null, "操作员", false, false, 1, 50, 1);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 更新删除前做的事情
        /// </summary>
        /// <returns></returns>
        protected override bool beforeUpdateInsertAction()
        {
            this.MyPK = this.FK_Dept + "_" + this.FK_Emp + "_" + this.FK_Station;
            return base.beforeUpdateInsertAction();
        }
    }
	/// <summary>
    /// 部门岗位人员对应 
	/// </summary>
	public class DeptEmpStations : EntitiesMyPK
	{
		#region 构造
		/// <summary>
		/// 工作部门岗位人员对应
		/// </summary>
		public DeptEmpStations()
		{
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
				return new DeptEmpStation();
			}
		}	
		#endregion 
		
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<DeptEmpStation> ToJavaList()
        {
            return (System.Collections.Generic.IList<DeptEmpStation>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<DeptEmpStation> Tolist()
        {
            System.Collections.Generic.List<DeptEmpStation> list = new System.Collections.Generic.List<DeptEmpStation>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((DeptEmpStation)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
