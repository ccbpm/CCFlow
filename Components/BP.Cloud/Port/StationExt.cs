using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.Cloud
{

    /// <summary>
    /// 岗位
    /// </summary>
    public class StationExt : EntityNoName
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
        /// <summary>
        /// 岗位类型
        /// </summary>
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
        #endregion

        #region 构造方法
        /// <summary>
        /// 岗位
        /// </summary> 
        public StationExt()
        {
        }
        /// <summary>
        /// 岗位
        /// </summary>
        /// <param name="no">岗位编号</param>
        public StationExt(string no)
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

                Map map = new Map("Port_Station", "岗位");

                map.AddTBStringPK(StationAttr.No, null, "编号", false, false, 1, 39, 4);
                map.AddTBString(StationAttr.Name, null, "名称", true, false, 0, 100, 400);
                map.AddDDLEntities(StationAttr.FK_StationType, null, "类型", new StationTypeExts(), true);
                map.AddTBString(StationAttr.OrgNo, null, "组织编号", false, false, 0, 100, 100);

                //查询条件.
                map.AddSearchAttr(StationAttr.FK_StationType);

                //隐藏的查询条件.
                map.AddHidden(StationTypeAttr.OrgNo, "=", BP.Web.WebUser.OrgNo);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            //当前人员所在的OrgNo.
            if (DataType.IsNullOrEmpty(this.No) == true)
                this.No = DBAccess.GenerGUID();

            this.OrgNo = BP.Web.WebUser.OrgNo;
            BP.Sys.Base.Glo.WriteUserLog("新增岗位:" + this.ToJson(), "岗位数据操作");
            return base.beforeInsert();
        }

        protected override bool beforeDelete()
        {

            DeptEmpStations ensD = new DeptEmpStations();
            ensD.Retrieve(DeptEmpStationAttr.FK_Station, this.No);
            if (ensD.Count > 0)
                throw new Exception("err@删除岗位错误，该岗位下有人员。");
            BP.Sys.Base.Glo.WriteUserLog("删除岗位:" + this.ToJson(), "岗位数据操作");
            return base.beforeDelete();
        }
    }
    /// <summary>
    /// 岗位s
    /// </summary>
    public class StationExts : EntitiesNoName
    {
        /// <summary>
        /// 岗位
        /// </summary>
        public StationExts() { }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new StationExt();
            }
        }
        public override int RetrieveAll()
        {
            return this.Retrieve(EmpAttr.OrgNo, BP.Web.WebUser.OrgNo);
        }


        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<StationExt> ToJavaList()
        {
            return (System.Collections.Generic.IList<StationExt>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<StationExt> Tolist()
        {
            System.Collections.Generic.List<StationExt> list = new System.Collections.Generic.List<StationExt>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((StationExt)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
