using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.Port
{
    /// <summary>
    /// 岗位属性
    /// </summary>
    public class StationAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 岗位类型
        /// </summary>
        public const string FK_StationType = "FK_StationType";
        /// <summary>
        /// 隶属组织
        /// </summary>
        public const string OrgNo = "OrgNo";

    }
    /// <summary>
    /// 岗位
    /// </summary>
    public class Station : EntityNoName
    {
        #region 实现基本的方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        public new string Name
        {
            get
            {
                return this.GetValStrByKey("Name");
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 岗位
        /// </summary> 
        public Station()
        {
        }
        /// <summary>
        /// 岗位
        /// </summary>
        /// <param name="no">岗位编号</param>
        public Station(string no)
        {
            this.No = no.Trim();
            if (this.No.Length == 0)
                throw new Exception("@要查询的岗位编号为空。");

            this.Retrieve();
        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Port_Station","岗位");
                map.Java_SetEnType(EnType.Admin);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetDepositaryOfEntity( Depositary.Application);

                map.AddTBStringPK(EmpAttr.No, null, "编号", true, false, 4, 4, 4);
                map.AddTBString(EmpAttr.Name, null, "名称", true, false, 0, 100, 100);
                map.AddTBString(StationAttr.OrgNo, null, "隶属组织编号", true, false, 0, 100, 100);

                
                //如果是一人一部门多岗位,就启动这个映射.
                if (BP.Sys.SystemConfig.OSModel == OSModel.OneOne)
                {
                    //岗位人员.
                    map.AttrsOfOneVSM.Add(new EmpStations(), new Emps(), EmpStationAttr.FK_Station, EmpStationAttr.FK_Emp,
                      DeptAttr.Name, DeptAttr.No, "人员");
                }

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion


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
                DataTable dt= v.GetStation(this.No);
                if (dt.Rows.Count == 0)
                    throw new Exception("@编号为(" + this.No + ")的岗位不存在。");
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
                DataTable dt = v.GetStation(this.No);
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
    /// 岗位s
    /// </summary>
    public class Stations : EntitiesNoName
    {
        /// <summary>
        /// 岗位
        /// </summary>
        public Stations() { }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Station();
            }
        }

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
                DataTable dt = v.GetStations();
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
                DataTable dt = v.GetStations();
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
        public System.Collections.Generic.IList<Station> ToJavaList()
        {
            return (System.Collections.Generic.IList<Station>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Station> Tolist()
        {
            System.Collections.Generic.List<Station> list = new System.Collections.Generic.List<Station>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Station)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
