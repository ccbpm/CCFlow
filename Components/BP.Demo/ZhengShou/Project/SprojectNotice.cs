using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.ZS
{
    /// <summary>
    /// 项目公告 attr
    /// </summary>
    public class SprojectNoticeAttr : EntityNoNameAttr
    {
        public const string SGSta = "SGSta";

    }
    /// <summary>
    ///  项目公告
    /// </summary>
    public class SprojectNotice : EntityNoName
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
        /// 项目公告
        /// </summary>
        public SprojectNotice()
        {
        }
        /// <summary>
        /// 项目公告
        /// </summary>
        /// <param name="_No"></param>
        public SprojectNotice(string _No) : base(_No) { }
        /// <summary>
        /// 项目公告Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("ZS_project_notice", "项目公告表");
                map.IsAutoGenerNo = true;
                map.CodeStruct = "5";

                map.AddTBStringPK("No", null, "编号", true, true, 5, 5, 20);
                map.AddTBString("Name", null, "项目名称", true, false, 0, 50, 50);
                map.AddTBString("project_id", null, "所属项目id", false, false, 0, 50, 50);
               
                map.AddDDLSysEnum("notice_type", 0, "公告类型", true, false, "notice_type", "@0=@1=征收范围公告@2=调查结果公告@3=补偿方案征求意见公告@4=补偿方案修改公告@5=补偿决定公告");
                map.AddTBString("notice_number", null, "公告文号", true, false, 0, 50, 50);
                map.AddTBString("notice_date", null, "公告日期", true, false, 0, 50, 50);
                map.AddTBString("notice_content", null, "公告内容", true, false, 0, 50, 50);
                map.AddTBString("file", null, "附件", true, false, 0, 50, 50);
                map.AddTBString("site_photo", null, "现场照片", true, false, 0, 50, 50);
                map.AddTBString("agency", null, "发布机构", true, false, 0, 50, 50);

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
    /// 项目公告s
    /// </summary>
    public class SprojectNotices : EntitiesNoName
    {
        /// <summary>
        /// 项目公告s
        /// </summary>
        public SprojectNotices() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SprojectNotice();
            }
        }
    }
}
