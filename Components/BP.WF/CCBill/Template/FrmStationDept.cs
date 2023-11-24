using BP.En;

namespace BP.CCBill.Template
{
    /// <summary>
    /// 单据查询角色属性	  
    /// </summary>
    public class FrmStationDeptAttr
    {
        /// <summary>
        /// 表单
        /// </summary>
        public const string FK_Frm = "FK_Frm";
        /// <summary>
        /// 工作角色
        /// </summary>
        public const string FK_Station = "FK_Station";
        /// <summary>
        /// 部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
    }
    /// <summary>
    /// 单据查询角色
    /// 单据查询角色有两部分组成.	 
    /// </summary>
    public class FrmStationDept : EntityMM
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
                uac.OpenAll();
                return uac;
            }
        }
        /// <summary>
        ///单据
        /// </summary>
        public string FK_Frm
        {
            get
            {
                return this.GetValStringByKey(FrmStationDeptAttr.FK_Frm);
            }
            set
            {
                this.SetValByKey(FrmStationDeptAttr.FK_Frm, value);
            }
        }
      
        /// <summary>
        /// 工作角色
        /// </summary>
        public string FK_Station
        {
            get
            {
                return this.GetValStringByKey(FrmStationDeptAttr.FK_Station);
            }
            set
            {
                this.SetValByKey(FrmStationDeptAttr.FK_Station, value);
            }
        }
        public string DeptNo
        {
            get
            {
                return this.GetValStringByKey(FrmStationDeptAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(FrmStationDeptAttr.FK_Dept, value);
            }
        }

        #endregion

        #region 构造方法
        /// <summary>
        /// 单据工作角色
        /// </summary>
        public FrmStationDept() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Frm_StationDept", "单据角色部门");

                map.AddTBStringPK(FrmStationDeptAttr.FK_Frm, null, "单据编号", false, false, 1, 190, 20);

                map.AddDDLEntitiesPK(FrmStationDeptAttr.FK_Station, null, "工作角色", new BP.Port.Stations(), true);

                map.AddDDLEntitiesPK(FrmStationDeptAttr.FK_Dept, null, "部门", new BP.Port.Depts(), true);

                this._enMap = map;
                return this._enMap;
            }
        }

     
        #endregion

    }
    /// <summary>
    /// 单据查询角色
    /// </summary>
    public class FrmStationDepts : EntitiesMM
    {

        /// <summary>
        /// 单据查询角色
        /// </summary>
        public FrmStationDepts() { }
        /// <summary>
        /// 单据查询角色
        /// </summary>
        /// <param name="frmID">单据ID</param>
        public FrmStationDepts(string frmID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FrmStationDeptAttr.FK_Frm, frmID);
            qo.DoQuery();
        }
 
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmStationDept();
            }
        }
 
        #region 为了适应自动翻译成java的需要,把实体转换成 List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmStationDept> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmStationDept>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmStationDept> Tolist()
        {
            System.Collections.Generic.List<FrmStationDept> list = new System.Collections.Generic.List<FrmStationDept>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmStationDept)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成 List.
    }
}
