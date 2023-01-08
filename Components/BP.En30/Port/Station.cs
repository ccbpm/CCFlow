using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Difference;


namespace BP.Port
{
    /// <summary>
    /// 角色属性
    /// </summary>
    public class StationAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 角色类型
        /// </summary>
        public const string FK_StationType = "FK_StationType";
        /// 隶属组织
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// 隶属部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 序号
        /// </summary>
        public const string Idx = "Idx";

    }
    /// <summary>
    /// 角色
    /// </summary>
    public class Station : EntityNoName
    {
        #region 属性
        public string FK_StationType
        {
            get
            {
                return this.GetValStrByKey(StationAttr.FK_StationType);
            }
            set
            {
                this.SetValByKey(StationAttr.FK_StationType, value);
            }
        }
        /// <summary>
        /// 组织编码
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStrByKey(StationAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(StationAttr.OrgNo, value);
            }
        }
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(StationAttr.Idx);
            }
            set
            {
                this.SetValByKey(StationAttr.Idx, value);
            }
        }
        #endregion

        #region 实现基本的方方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 角色
        /// </summary> 
        public Station()
        {
        }
        /// <summary>
        /// 角色
        /// </summary>
        /// <param name="_No"></param>
        public Station(string _No) : base(_No) { }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Port_Station", "角色");
               // map.setCodeStruct("3");
              //  map.setIsAutoGenerNo(true);

                map.AddTBStringPK(StationAttr.No, null, "编号", true, true, 1, 50, 200);
                map.AddTBString(StationAttr.Name, null, "名称", true, false, 0, 100, 200);
                map.AddDDLEntities(StationAttr.FK_StationType, null, "类型", new StationTypes(), true);
                map.AddTBString(StationAttr.OrgNo, null, "隶属组织", false, false, 0, 50, 250);

                #region 根据组织结构类型不同.
                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                {
                    map.AddHidden(StationAttr.OrgNo, "=", BP.Web.WebUser.OrgNo); //加隐藏条件.
                }

                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.GroupInc)
                {
                    if (BP.Difference.SystemConfig.GroupStationModel == 0)
                        map.AddHidden(StationAttr.OrgNo, "=", BP.Web.WebUser.OrgNo);//每个组织都有自己的岗责体系的时候. 加隐藏条件.
                    //每个部门都有自己的角色体系.
                    if (BP.Difference.SystemConfig.GroupStationModel == 2)
                    {
                        map.AddTBString(StationAttr.FK_Dept, null, "部门编号", true, false, 0, 100, 200);
                        map.AddHidden(StationAttr.FK_Dept, "=", BP.Web.WebUser.FK_Dept);//每个组织都有自己的岗责体系的时候. 加隐藏条件.
                    }
                }

                map.AddTBInt(StationAttr.Idx, 0, "顺序号", true, false);
                #endregion 根据组织结构类型不同.

                map.AddSearchAttr(StationAttr.FK_StationType);

                //角色下的用户.
                map.AddDtl(new DeptEmpStations(), DeptEmpStationAttr.FK_Station, null, DtlEditerModel.DtlSearch, null);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            if (DataType.IsNullOrEmpty(this.No))
                this.No = DBAccess.GenerGUID();

            return base.beforeInsert();
        }

        protected override bool beforeUpdateInsertAction()
        {
            if (DataType.IsNullOrEmpty(this.Name) == true)
                throw new Exception("请输入名称");
            if (DataType.IsNullOrEmpty(this.FK_StationType) == true)
                throw new Exception("请选择类型"); //@hongyan.

            if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                this.OrgNo = BP.Web.WebUser.OrgNo;

            //每个部门都有自己的角色体系.
            if (BP.Difference.SystemConfig.GroupStationModel == 2)
                this.SetValByKey(StationAttr.FK_Dept, BP.Web.WebUser.FK_Dept);


            BP.Sys.Base.Glo.WriteUserLog("新建/修改角色:" + this.ToJson(), "组织数据操作");

            return base.beforeUpdateInsertAction();
        }
        protected override bool beforeDelete()
        {
            BP.Sys.Base.Glo.WriteUserLog("删除角色:" + this.ToJson(), "组织数据操作");
            return base.beforeDelete();
        }

    }
    /// <summary>
    /// 角色s
    /// </summary>
    public class Stations : EntitiesNoName
    {
        /// <summary>
        /// 角色
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
        /// <summary>
        /// 查询全部
        /// </summary>
        /// <param name="orderBy">排序</param>
        /// <returns></returns>
        public override int RetrieveAll(string orderBy)
        {
            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                return base.RetrieveAll(orderBy);

            //集团模式下的角色体系: @0=每套组织都有自己的角色体系@1=所有的组织共享一套岗则体系.
            if (BP.Difference.SystemConfig.GroupStationModel == 1)
                return base.RetrieveAll();

            //按照orgNo查询.
            return this.Retrieve("OrgNo", BP.Web.WebUser.OrgNo, orderBy);
        }
        /// <summary>
        /// 查询全部
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAll()
        {
            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                return base.RetrieveAll("Idx");

            //集团模式下的角色体系: @0=每套组织都有自己的角色体系@1=所有的组织共享一套岗则体系.
            if (BP.Difference.SystemConfig.GroupStationModel == 1)
                return base.RetrieveAll("Idx");
            if (BP.Difference.SystemConfig.GroupStationModel == 0)
                return base.Retrieve(StationAttr.OrgNo, BP.Web.WebUser.OrgNo);

            if (BP.Difference.SystemConfig.GroupStationModel == 2)
                return base.Retrieve(StationAttr.FK_Dept, BP.Web.WebUser.FK_Dept);

            //按照orgNo查询.
            return this.Retrieve("OrgNo", BP.Web.WebUser.OrgNo, "Idx");
        }

        public override int RetrieveAllFromDBSource()
        {
            return this.RetrieveAll();
        }

        public override int RetrieveAllFromDBSource(string orderBY)
        {
            return this.RetrieveAll(orderBY);
        }

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
