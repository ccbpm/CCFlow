using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.Port
{
    /// <summary>
    /// 岗位类型
    /// </summary>
    public class StationTypeAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 排序字段
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 组织机构编号
        /// </summary>
        public const string OrgNo = "OrgNo";

    }
    /// <summary>
    ///  岗位类型
    /// </summary>
    public class StationType : EntityNoName
    {
        #region 属性
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

        public string FK_StationTypeText
        {
            get
            {
                return this.GetValRefTextByKey(StationAttr.FK_StationType);
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
        public StationType()
        {
        }
        /// <summary>
        /// 岗位类型
        /// </summary>
        /// <param name="_No"></param>
        public StationType(string _No) : base(_No) { }
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
                map.CodeStruct = "2";


                map.AddTBStringPK(StationTypeAttr.No, null, "编号", true, true, 1, 5, 5);
                map.AddTBString(StationTypeAttr.Name, null, "名称", true, false, 1, 50, 20);
                map.AddTBInt(StationTypeAttr.Idx, 0, "顺序", true, false);
                map.AddTBString(StationTypeAttr.OrgNo, null, "组织机构编号", true, false, 0, 50, 20);

                
                if (SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                    map.AddHidden(StationTypeAttr.OrgNo, "=", BP.Web.WebUser.OrgNo);

                this._enMap = map;
                return this._enMap;
            }
        }
    }
    /// <summary>
    /// 岗位类型
    /// </summary>
    public class StationTypes : EntitiesNoName
    {
        /// <summary>
        /// 岗位类型s
        /// </summary>
        public StationTypes() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new StationType();
            }
        }
        /// <summary>
        /// 查询全部
        /// </summary>
        /// <param name="orderBy">排序</param>
        /// <returns></returns>
        public override int RetrieveAll(string orderBy)
        {
            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                return base.RetrieveAll(orderBy);

            //集团模式下的岗位体系: @0=每套组织都有自己的岗位体系@1=所有的组织共享一套岗则体系.
            if (BP.Sys.SystemConfig.GroupStationModel == 1)
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
            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                return base.RetrieveAll();

            //集团模式下的岗位体系: @0=每套组织都有自己的岗位体系@1=所有的组织共享一套岗则体系.
            if (BP.Sys.SystemConfig.GroupStationModel == 1)
                return base.RetrieveAll();

            //按照orgNo查询.
            return this.Retrieve("OrgNo", BP.Web.WebUser.OrgNo);
        }

       
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<StationType> ToJavaList()
        {
            return (System.Collections.Generic.IList<StationType>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<StationType> Tolist()
        {
            System.Collections.Generic.List<StationType> list = new System.Collections.Generic.List<StationType>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((StationType)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
