using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Cloud
{
    /// <summary>
    ///  岗位类型
    /// </summary>
    public class StationTypeExt : EntityNoName
    {
        #region 属性
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
        /// 岗位类型
        /// </summary>
        public StationTypeExt()
        {
        }
        /// <summary>
        /// 岗位类型
        /// </summary>
        /// <param name="_No"></param>
        public StationTypeExt(string _No) : base(_No) { }
        #endregion

        /// <summary>
        /// 岗位类型Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Port_StationType", "岗位类型");
                map.ItIsAutoGenerNo = false;

                map.AddTBStringPK(StationTypeAttr.No, null, "编号", false, true, 1, 40, 2);
                map.AddTBString(StationTypeAttr.Name, null, "名称", true, false, 1, 50, 400);
                map.AddTBInt(StationTypeAttr.Idx, 0, "顺序", true, false);


                map.AddTBString(StationTypeAttr.OrgNo, null, "组织机构编号", false, false, 0, 50, 20);

                //增加隐藏查询条件.
                  map.AddHidden(StationTypeAttr.OrgNo, "=", BP.Web.WebUser.OrgNo);

                this._enMap = map;
                return this._enMap;
            }
        }

        protected override bool beforeInsert()
        {
            if (DataType.IsNullOrEmpty(this.No) == true)
                this.No = DBAccess.GenerGUID();

            this.OrgNo = BP.Web.WebUser.OrgNo;
            return base.beforeInsert();
        }

        protected override bool beforeDelete()
        {
            Stations ens = new Stations();
            ens.Retrieve(StationAttr.FK_StationType, this.No);
            if (ens.Count > 0)
                throw new Exception("err@删除岗位类型错误，该类型下有岗位。");

            return base.beforeDelete();
        }

    }
    /// <summary>
    /// 岗位类型
    /// </summary>
    public class StationTypeExts : EntitiesNoName
    {
        /// <summary>
        /// 岗位类型s
        /// </summary>
        public StationTypeExts() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new StationTypeExt();
            }
        }
        public override int RetrieveAllFromDBSource()
        {
            return this.Retrieve(StationAttr.OrgNo, BP.Web.WebUser.OrgNo, "Idx");
        }
        public override int RetrieveAllFromDBSource(string orderByAttr)
        {
            return this.Retrieve(StationAttr.OrgNo, BP.Web.WebUser.OrgNo, orderByAttr);
        }
        public override int RetrieveAll()
        {
            return this.Retrieve(StationAttr.OrgNo, BP.Web.WebUser.OrgNo,"Idx");
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<StationTypeExt> ToJavaList()
        {
            return (System.Collections.Generic.IList<StationTypeExt>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<StationTypeExt> Tolist()
        {
            System.Collections.Generic.List<StationTypeExt> list = new System.Collections.Generic.List<StationTypeExt>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((StationTypeExt)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
