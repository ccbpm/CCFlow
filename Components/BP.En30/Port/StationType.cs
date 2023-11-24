using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Difference;

namespace BP.Port
{
    /// <summary>
    /// 角色类型
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
    ///  角色类型
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
        /// 角色类型
        /// </summary>
        public StationType()
        {
        }
        /// <summary>
        /// 角色类型
        /// </summary>
        /// <param name="_No"></param>
        public StationType(string _No) : base(_No) { }
        #endregion

        /// <summary>
        /// 角色类型Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Port_StationType", "角色类型");
                map.CodeStruct = "2";

                map.AddTBStringPK(StationTypeAttr.No, null, "编号", true, true, 1, 40, 40);
                map.AddTBString(StationTypeAttr.Name, null, "名称", true, false, 1, 50, 20);
                map.AddTBInt(StationTypeAttr.Idx, 0, "顺序", true, false);

                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                {
                    map.AddTBString(StationAttr.OrgNo, null, "隶属组织", false, false, 0, 50, 250);
                    map.AddHidden(StationAttr.OrgNo, "=", "@WebUser.OrgNo"); //加隐藏条件.
                }

                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.GroupInc)
                {
                    map.AddTBString(StationAttr.OrgNo, null, "隶属组织", true, true, 0, 50, 250);

                    if (BP.Difference.SystemConfig.GroupStationModel == 0)
                        map.AddHidden(StationAttr.OrgNo, "=", "@WebUser.OrgNo");//每个组织都有自己的岗责体系的时候. 加隐藏条件.
                    if (BP.Difference.SystemConfig.GroupStationModel == 2)
                    {
                        map.AddTBString(StationAttr.FK_Dept, null, "隶属部门", false, false, 0, 50, 250);
                        map.AddHidden(StationAttr.FK_Dept, "=", "@WebUser.FK_Dept");

                    }
                }

                this._enMap = map;
                return this._enMap;
            }
        }
        protected override bool beforeUpdateInsertAction()
        {
            if (DataType.IsNullOrEmpty(this.Name) == true)
                throw new Exception("请输入名称");

            if (DataType.IsNullOrEmpty(this.OrgNo) == true && BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                this.OrgNo = BP.Web.WebUser.OrgNo;

            if (BP.Difference.SystemConfig.GroupStationModel == 2)
                this.SetValByKey(StationAttr.FK_Dept, BP.Web.WebUser.DeptNo);

            return base.beforeUpdateInsertAction();
        }

    }
    /// <summary>
    /// 角色类型
    /// </summary>
    public class StationTypes : EntitiesNoName
    {
        #region 构造方法..
        /// <summary>
        /// 角色类型s
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
        #endregion 构造方法..

        #region 查询..
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

            //按照orgNo查询.
            return this.Retrieve("OrgNo", BP.Web.WebUser.OrgNo, "Idx");
        }
        /// <summary>
        /// 查询全部
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAll(string idx) 
        {
            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                return base.RetrieveAll(idx);

            //集团模式下的角色体系: @0=每套组织都有自己的角色体系@1=所有的组织共享一套岗则体系.
            if (BP.Difference.SystemConfig.GroupStationModel == 1)
                return base.RetrieveAll(idx);

            //按照orgNo查询.
            return this.Retrieve("OrgNo", BP.Web.WebUser.OrgNo, idx);
        }
        #endregion 查询..

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
