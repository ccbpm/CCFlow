    using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.ZS
{
    /// <summary>
    /// 调查房栋 attr
    /// </summary>
    public class InquiryBuildingAttr : EntityNoNameAttr
    {
        public const string SGSta = "SGSta";

    }
    /// <summary>
    ///  调查房栋
    /// </summary>
    public class InquiryBuilding : EntityNoName
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
        /// 调查房栋
        /// </summary>
        public InquiryBuilding()
        {
        }
        /// <summary>
        /// 调查房栋
        /// </summary>
        /// <param name="_No"></param>
        public InquiryBuilding(string _No) : base(_No) { }
        /// <summary>
        /// 调查房栋Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("ZS_inquiry_building", "房幢信息表");
                map.IsAutoGenerNo = true;
                map.CodeStruct = "4";

                map.AddTBStringPK("No", null, "编号", true, true, 5, 5, 20);
                map.AddTBString("Name", null, "房幢名称", true, false, 0, 50, 50);
                map.AddTBString("project_id", null, "所属项目id", true, false, 0, 50, 50);
                
                map.AddTBString("type", null, "房幢类型", true, false, 0, 50, 50);
                map.AddTBString("address", null, "房幢地址", true, false, 0, 50, 50);
                map.AddTBString("structure", null, "建筑结构", true, false, 0, 50, 50);
                map.AddTBString("built_year", null, "建成年代", true, false, 0, 50, 50);
                map.AddTBString("unit_num", null, "单元数", true, false, 0, 50, 50);
                map.AddTBString("aboveground_num", null, "地上层数", true, false, 0, 50, 50);
                map.AddTBString("underground_num", null, "地下层数", true, false, 0, 50, 50);
                map.AddTBString("storey_hight", null, "层高", true, false, 0, 50, 50);
                map.AddTBString("area", null, "DEFAULT", true, false, 0, 50, 50);
                map.AddTBString("site_photo", null, "现场照片", true, false, 0, 50, 50);
                map.AddTBString("draft", null, "0", true, false, 0, 50, 50);
                map.AddTBString("status", null, "状态", true, false, 0, 50, 50);
                map.AddTBString("apply_date", null, "申请日期", true, false, 0, 50, 50);
                map.AddTBString("review_date", null, "复核日期", true, false, 0, 50, 50);

                map.AddTBDate("sys_created_time", null, "创建时间", true, false);
                map.AddTBDate("sys_modified_time", null, "修改时间", true, false);
                map.AddTBString("sys_created_user", null, "创建人ID", false, false, 0, 50, 50);
                map.AddTBString("sys_modified_user", null, "最后修改人ID", false, false, 0, 50, 50);


                map.AddDtl(new InquiryHouseholds(), "building_id");


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 调查房栋s
    /// </summary>
    public class InquiryBuildings : EntitiesNoName
    {
        /// <summary>
        /// 调查房栋s
        /// </summary>
        public InquiryBuildings() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new InquiryBuilding();
            }
        }
    }
}
