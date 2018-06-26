using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web;
using BP.Sys;

namespace BP.Port
{
    /// <summary>
    /// 部门属性
    /// </summary>
    public class DeptAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 父节点的编号
        /// </summary>
        public const string ParentNo = "ParentNo";
    }
    /// <summary>
    /// 部门
    /// </summary>
    public class Dept : EntityNoName
    {
        #region 属性
        /// <summary>
        /// 父节点的ID
        /// </summary>
        public string ParentNo
        {
            get
            {
                return this.GetValStrByKey(DeptAttr.ParentNo);
            }
            set
            {
                this.SetValByKey(DeptAttr.ParentNo, value);
            }
        }
        public int Grade
        {
            get
            {
                return 1;
            }
        }
        private Depts _HisSubDepts = null;
        /// <summary>
        /// 它的子节点
        /// </summary>
        public Depts HisSubDepts
        {
            get
            {
                if (_HisSubDepts == null)
                    _HisSubDepts = new Depts(this.No);
                return _HisSubDepts;
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 部门
        /// </summary>
        public Dept() { }
        /// <summary>
        /// 部门
        /// </summary>
        /// <param name="no">编号</param>
        public Dept(string no) : base(no) { }
        #endregion

        #region 重写方法
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
        /// Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map();
                map.EnDBUrl = new DBUrl(DBUrlType.AppCenterDSN); //连接到的那个数据库上. (默认的是: AppCenterDSN )
                map.PhysicsTable = "Port_Dept";
                map.Java_SetEnType(EnType.Admin);
                map.IsEnableVer = true;

                map.EnDesc = "部门"; //  实体的描述.
                map.Java_SetDepositaryOfEntity( Depositary.Application); //实体map的存放位置.
                map.Java_SetDepositaryOfMap( Depositary.Application);    // Map 的存放位置.

                map.AddTBStringPK(DeptAttr.No, null, "编号", true, false, 1, 50, 20);
                map.AddTBString(DeptAttr.Name, null, "名称", true, false, 0, 100, 30);
                map.AddTBString(DeptAttr.ParentNo, null, "父节点编号", true, true, 0, 100, 30);



                RefMethod rm = new RefMethod();
                rm.Title = "历史变更";
                rm.ClassMethodName = this.ToString() + ".History";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                #region 增加点对多属性
                //他的部门权限
               // map.AttrsOfOneVSM.Add(new DeptStations(), new Stations(), DeptStationAttr.FK_Dept, DeptStationAttr.FK_Station, StationAttr.Name, StationAttr.No, "岗位权限");
                #endregion 

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        public string History()
        {
            return "EnVerDtl.htm?EnName="+this.ToString()+"&PK="+this.No;
        }

        #region 重写查询. 2015.09.31 为适应ws的查询.
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public override int Retrieve()
        {
            if (BP.Sys.SystemConfig.OSDBSrc == OSDBSrc.WebServices)
            {
                var v = DataType.GetPortalInterfaceSoapClientInstance();
                DataTable dt = v.GetDept(this.No);
                if (dt.Rows.Count == 0)
                    throw new Exception("@编号为(" + this.No + ")的部门不存在。");
                this.Row.LoadDataTable(dt, dt.Rows[0]);
                return 1;
            }
            else
            {
                return base.Retrieve();
            }
        }
        /// <summary>
        /// 查询.
        /// </summary>
        /// <returns></returns>
        public override int RetrieveFromDBSources()
        {
            if (BP.Sys.SystemConfig.OSDBSrc == OSDBSrc.WebServices)
            {
                var v = DataType.GetPortalInterfaceSoapClientInstance();
                DataTable dt = v.GetDept(this.No);
                if (dt.Rows.Count == 0)
                    return 0;
                this.Row.LoadDataTable(dt, dt.Rows[0]);
                return 1;
            }
            else
            {
                return base.RetrieveFromDBSources();
            }
        }
        #endregion

    }
    /// <summary>
    ///部门s
    /// </summary>
    public class Depts : BP.En.EntitiesNoName
    {
        #region 初始化实体.
        /// <summary>
        /// 得到一个新实体
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Dept();
            }
        }
        /// <summary>
        /// 部门集合
        /// </summary>
        public Depts()
        {
        }
        /// <summary>
        /// 部门集合
        /// </summary>
        /// <param name="parentNo">父部门No</param>
        public Depts(string parentNo)
        {
            if (BP.Sys.SystemConfig.OSDBSrc == OSDBSrc.WebServices)
            {
                this.Clear(); //清除缓存数据.
                //获得数据.
                var v = DataType.GetPortalInterfaceSoapClientInstance();
                DataTable dt = v.GetDeptsByParentNo(parentNo);
                //设置查询.
                QueryObject.InitEntitiesByDataTable(this, dt, null);
            }
            else
            {
                this.Retrieve(DeptAttr.ParentNo, parentNo);
            }
        }
        #endregion 初始化实体.

        #region 重写查询,add by stone 2015.09.30 为了适应能够从webservice数据源查询数据.
        /// <summary>
        /// 重写查询全部适应从WS取数据需要
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAll()
        {
            if (BP.Sys.SystemConfig.OSDBSrc == OSDBSrc.WebServices)
            {
                this.Clear(); //清除缓存数据.
                //获得数据.
                var v = DataType.GetPortalInterfaceSoapClientInstance();
                DataTable dt = v.GetDepts();
                if (dt.Rows.Count == 0)
                    return 0;

                //设置查询.
                QueryObject.InitEntitiesByDataTable(this, dt, null);
                return dt.Rows.Count;
            }
            else
            {
                return base.RetrieveAll();
            }
        }
        /// <summary>
        /// 重写重数据源查询全部适应从WS取数据需要
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAllFromDBSource()
        {
            if (BP.Sys.SystemConfig.OSDBSrc == OSDBSrc.WebServices)
            {
                this.Clear(); //清除缓存数据.
                //获得数据.
                var v = DataType.GetPortalInterfaceSoapClientInstance();
                DataTable dt = v.GetDepts();
                if (dt.Rows.Count == 0)
                    return 0;

                //设置查询.
                QueryObject.InitEntitiesByDataTable(this, dt, null);
                return dt.Rows.Count;
            }
            else
            {
                return base.RetrieveAllFromDBSource();
            }
        }
        #endregion 重写查询.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Dept> ToJavaList()
        {
            return (System.Collections.Generic.IList<Dept>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Dept> Tolist()
        {
            System.Collections.Generic.List<Dept> list = new System.Collections.Generic.List<Dept>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Dept)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
