using System;
using System.Collections;
using BP.DA;
using BP.Difference;
using BP.En;
using BP.Sys;
using BP.Web;

namespace BP.Port
{
    /// <summary>
    /// 用户组类型
    /// </summary>
    public class TeamTypeAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 排序字段
        /// </summary>
        public const string Idx = "Idx";
    }
    /// <summary>
    ///  用户组类型
    /// </summary>
    public class TeamType : EntityNoName
    {
        #region 属性
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
        /// 用户组类型
        /// </summary>
        public TeamType()
        {
        }
        /// <summary>
        /// 用户组类型
        /// </summary>
        /// <param name="_No"></param>
        public TeamType(string _No) : base(_No) { }
        #endregion

        /// <summary>
        /// 用户组类型Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Port_TeamType", "标签类型");
                map.CodeStruct = "2";

                map.AddTBStringPK(TeamTypeAttr.No, null, "编号", true, true, 1, 5, 5);
                map.AddTBString(TeamTypeAttr.Name, null, "名称", true, false, 1, 50, 20);
               
                map.AddTBInt(TeamTypeAttr.Idx, 0, "顺序", true, false);

                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                {
                    map.AddTBString(StationAttr.OrgNo, null, "隶属组织", true, true, 0, 50, 250);
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

        protected override bool beforeInsert()
        {
            if (SystemConfig.CCBPMRunModel != Sys.CCBPMRunModel.Single)
            {
                if (DataType.IsNullOrEmpty(this.GetValStringByKey("OrgNo")) == true)
                    this.SetValByKey("OrgNo", WebUser.OrgNo);
            }
            if (DataType.IsNullOrEmpty(this.GetValStringByKey("No")) == true)
                this.SetValByKey("No",DBAccess.GenerGUID());

            return base.beforeInsert();
        }

        protected override bool beforeUpdateInsertAction()
        {
            if (DataType.IsNullOrEmpty(this.Name) == true)
                throw new Exception("请输入名称");
            return base.beforeUpdateInsertAction();
        }
    }
    /// <summary>
    /// 用户组类型
    /// </summary>
    public class TeamTypes : EntitiesNoName
    {
        #region 构造.
        /// <summary>
        /// 用户组类型s
        /// </summary>
        public TeamTypes() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new TeamType();
            }
        }
        #endregion 构造.


        #region 查询..
        /// <summary>
        /// 查询全部
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAll()
        {
            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                return base.RetrieveAll("Idx");

            if (SystemConfig.CCBPMRunModel== CCBPMRunModel.SAAS)
                return this.Retrieve("OrgNo", BP.Web.WebUser.OrgNo, "Idx");

            //集团模式下的角色体系: @0=每套组织都有自己的角色体系@1=所有的组织共享一套岗则体系.
            if (BP.Difference.SystemConfig.GroupStationModel == 1)
                return base.RetrieveAll("Idx");

            //按照orgNo查询.
            return this.Retrieve("OrgNo", BP.Web.WebUser.OrgNo, "Idx");
        }
        public override int RetrieveAllFromDBSource()
        {
            return this.RetrieveAll();
        }
        #endregion 查询..

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<TeamType> ToJavaList()
        {
            return (System.Collections.Generic.IList<TeamType>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<TeamType> Tolist()
        {
            System.Collections.Generic.List<TeamType> list = new System.Collections.Generic.List<TeamType>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((TeamType)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
