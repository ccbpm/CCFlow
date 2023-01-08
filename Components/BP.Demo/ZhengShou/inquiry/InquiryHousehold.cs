    using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.ZS
{
    /// <summary>
    /// 调查户 attr
    /// </summary>
    public class InquiryHouseholdAttr : EntityNoNameAttr
    {
        public const string SGSta = "SGSta";

    }
    /// <summary>
    ///  调查户
    /// </summary>
    public class InquiryHousehold : EntityNoName
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
        /// 调查户
        /// </summary>
        public InquiryHousehold()
        {
        }
        /// <summary>
        /// 调查户
        /// </summary>
        /// <param name="_No"></param>
        public InquiryHousehold(string _No) : base(_No) { }
        /// <summary>
        /// 调查户Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("ZS_inquiry_household", "调查户信息表");
                map.IsAutoGenerNo = true;
                map.CodeStruct = "4";

                map.AddTBStringPK("No", null, "编号", false, true, 5, 5, 20);
                map.AddTBString("Name", null, "项目名称", true, false, 0, 50, 50);
                map.AddTBString("building_id", null, "房幢id", true, false, 0, 50, 50);
                map.AddTBString("building_name", null, "房幢名称", true, false, 0, 50, 50);
                map.AddTBString("unit", null, "所属单元", true, false, 0, 50, 50);
                map.AddTBString("storey", null, "所属楼层", true, false, 0, 50, 50);
                map.AddTBString("room_no", null, "房间号", true, false, 0, 50, 50);
                map.AddTBString("type", null, "产权类型", true, false, 0, 50, 50);
                map.AddTBString("owner", null, "产权人", true, false, 0, 50, 50);
                map.AddTBString("property_number", null, "产权号", true, false, 0, 50, 50);
                map.AddTBString("property_area", null, "产权面积（平方米）", true, false, 0, 50, 50);
                map.AddTBString("residential_area", null, "住宅面积（平方米）", true, false, 0, 50, 50);
                map.AddTBString("non_residential_area", null, "非住宅面积（平方米）", true, false, 0, 50, 50);
                map.AddTBString("non_confirmed_area", null, "未登记认定面积（平方米）", true, false, 0, 50, 50);
                map.AddTBString("temp_violate_area", null, "临建违建面积（平方米）", true, false, 0, 50, 50);
                map.AddTBString("multi_storey", null, "是否一户多层: 0否, 1是", true, false, 0, 50, 50);
                map.AddTBString("storey_num", null, "一户多层-层数", true, false, 0, 50, 50);
                map.AddTBString("design_purpose", null, "设计用途", true, false, 0, 50, 50);
                map.AddTBString("practical_purpose", null, "实际用途", true, false, 0, 50, 50);
                map.AddTBString("property_address", null, "产权地址", true, false, 0, 50, 50);
                map.AddTBString("current_address", null, "现地址", true, false, 0, 50, 50);
                map.AddTBString("site_photo", null, "现场照片", true, false, 0, 50, 50);
                map.AddTBString("draft", null, "是否为草稿", true, false, 0, 50, 50);
                map.AddTBString("status", null, "状态", true, false, 0, 50, 50);
                map.AddTBString("apply_date", null, "申请日期", true, false, 0, 50, 50);
                map.AddTBString("review_date", null, "复核日期", true, false, 0, 50, 50);



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
    /// 调查户s
    /// </summary>
    public class InquiryHouseholds : EntitiesNoName
    {
        /// <summary>
        /// 调查户s
        /// </summary>
        public InquiryHouseholds() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new InquiryHousehold();
            }
        }
    }
}
