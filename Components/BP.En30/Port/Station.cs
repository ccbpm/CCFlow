using System;
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
        /// 隶属组织
        /// </summary>
        public const string OrgNo = "OrgNo";
    }
    /// <summary>
    /// 岗位
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
        /// 岗位
        /// </summary> 
        public Station()
        {
        }
        /// <summary>
        /// 岗位
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

                Map map = new Map("Port_Station", "岗位");
                map.CodeStruct = "3";
                map.IsAutoGenerNo = true;

                map.AddTBStringPK(StationAttr.No, null, "编号", true, true, 1, 50, 200);
                map.AddTBString(StationAttr.Name, null, "名称", true, false, 0, 100, 200);
                map.AddDDLEntities(StationAttr.FK_StationType, null, "类型", new StationTypes(), true);
                map.AddTBString(StationAttr.OrgNo, null, "隶属组织", true, false, 0, 50, 250);
                map.AddSearchAttr(StationAttr.FK_StationType);

                if (SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                {

                }
                else
                {
                    map.AddHidden(StationTypeAttr.OrgNo, "=", BP.Web.WebUser.OrgNo);
                }


                this._enMap = map;
                return this._enMap;
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
