    using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.ZS
{
    /// <summary>
    /// 机构选定 attr
    /// </summary>
    public class ProjectAgencyBindAttr : EntityNoNameAttr
    {
        public const string SGSta = "SGSta";

    }
    /// <summary>
    ///  机构选定
    /// </summary>
    public class ProjectAgencyBind : EntityNoName
    {
        #region 属性.
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
        #endregion 属性.

        #region 构造方法
        /// <summary>
        /// 机构选定
        /// </summary>
        public ProjectAgencyBind()
        {
        }
        /// <summary>
        /// 机构选定
        /// </summary>
        /// <param name="_No"></param>
        public ProjectAgencyBind(string _No) : base(_No) { }
        /// <summary>
        /// 机构选定Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("ZS_project_agency_bind", "单位选定");
                map.IsAutoGenerNo = true;
                map.CodeStruct = "4";
                map.AddTBStringPK("No", null, "编号", true, true, 5, 5, 20);
                map.AddTBString("Name", null, "名称", true, false, 0, 50, 50);
                map.AddTBString("project_id", null, "项目id", true, false, 0, 50, 50);
                map.AddTBString("agency_id", null, "机构id", true, false, 0, 50, 50);
                map.AddTBString("bind_date", null, "选定日期", true, false, 0, 50, 50);
                map.AddTBString("work_describe", null, "工作描述", true, false, 0, 50, 50);
                map.AddTBString("file", null, "附件", true, false, 0, 50, 50);
                map.AddTBString("sys_created_time", null, "创建时间", true, false, 0, 50, 50);
                map.AddTBString("sys_created_user", null, "创建人ID", true, false, 0, 50, 50);
                map.AddTBString("sys_modified_time", null, "UPDATE", true, false, 0, 50, 50);
                map.AddTBString("sys_modified_user", null, "最后修改人ID", true, false, 0, 50, 50);
                              

                map.AddTBDate("sys_created_time", null, "创建时间", false, false);
                map.AddTBDate("sys_modified_time", null, "修改时间", false, false);

                map.AddTBString("sys_created_user", null, "创建人ID", false, false, 0, 50, 50);
                map.AddTBString("sys_modified_user", null, "最后修改人ID", false, false, 0, 50, 50);

                map.AddMyFile("FU");

              //  map.AddMyFile("照片");



                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 机构选定s
    /// </summary>
    public class ProjectAgencyBinds : EntitiesNoName
    {
        /// <summary>
        /// 机构选定s
        /// </summary>
        public ProjectAgencyBinds() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ProjectAgencyBind();
            }
        }
    }
}
