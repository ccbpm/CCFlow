    using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.ZS
{
    /// <summary>
    /// 项目文书 attr
    /// </summary>
    public class ProjectDocumentAttr : EntityNoNameAttr
    {
        public const string SGSta = "SGSta";

    }
    /// <summary>
    ///  项目文书
    /// </summary>
    public class ProjectDocument : EntityNoName
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
        /// 项目文书
        /// </summary>
        public ProjectDocument()
        {
        }
        /// <summary>
        /// 项目文书
        /// </summary>
        /// <param name="_No"></param>
        public ProjectDocument(string _No) : base(_No) { }
        /// <summary>
        /// 项目文书Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("ZS_project_document", "项目文书表");
                map.IsAutoGenerNo = true;
                map.CodeStruct = "5";

                map.AddTBStringPK("No", null, "编号", true, true, 5, 5, 20);
                map.AddTBString("Name", null, "名称", true, false, 0, 50, 50);


                map.AddTBString("project_id", null, "所属项目id", false, false, 0, 50, 50);
                
                map.AddTBString("handle_unit", null, "经办单位", true, false, 0, 50, 50);
                map.AddTBDate("release_date", null, "发布日期", true, false);
                map.AddTBString("file", null, "附件", true, false, 0, 50, 50);
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
    /// 项目文书s
    /// </summary>
    public class ProjectDocuments : EntitiesNoName
    {
        /// <summary>
        /// 项目文书s
        /// </summary>
        public ProjectDocuments() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ProjectDocument();
            }
        }
    }
}
