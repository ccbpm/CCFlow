using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.ZS
{
    /// <summary>
    /// 项目信息 attr
    /// </summary>
    public class ProjectAttr : EntityNoNameAttr
    {
        public const string SGSta = "SGSta";

    }
    /// <summary>
    ///  项目信息
    /// </summary>
    public class Project : EntityNoName
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
        /// 项目信息
        /// </summary>
        public Project()
        {
        }
        /// <summary>
        /// 项目信息
        /// </summary>
        /// <param name="_No"></param>
        public Project(string _No) : base(_No) { }
        /// <summary>
        /// 项目信息Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;


                Map map = new Map("ZS_project_info", "项目信息表");
                map.IsAutoGenerNo = true;
                map.CodeStruct = "4";

                map.AddTBStringPK("No", null, "编号", true, true, 5, 5, 20);
                map.AddTBString("Name", null, "项目名称", true, false, 0, 50, 50);

                map.AddTBString("district", null, "所属辖区", true, false, 0, 50, 50);

                map.AddTBString("project_type", null, "项目类型", true, false, 0, 50, 50);
                map.AddTBString("project_status", null, "项目状态", true, false, 0, 50, 50);
                map.AddTBString("cover_area", null, "占地面积(亩)", true, false, 0, 50, 50);
                map.AddTBString("contact_person", null, "市项目联系人", true, false, 0, 50, 50);
                map.AddTBString("district_person", null, "区项目联系人", true, false, 0, 50, 50);
                map.AddTBDate("start_date", null, "启动日期", true, false);

                map.AddTBStringDoc("bad_behavior", null, "描述", true, false, true);

                map.AddTBDate("sys_created_time", null, "创建时间", true, false);
                map.AddTBDate("sys_modified_time", null, "修改时间", true, false);
                map.AddTBString("sys_created_user", null, "创建人ID", false, false, 0, 50, 50);
                map.AddTBString("sys_modified_user", null, "最后修改人ID", false, false, 0, 50, 50);


                #region  基本信息
                RefMethod rm = new RefMethod();
                rm.Title = "评估人员";
                rm.GroupName = "基本信息";
                rm.ClassMethodName = this.ToString() + ".DoList()";
                map.AddRefMethod(rm);

                map.AddDtl(new SprojectNotices(), "project_id", "基本信息"); //公告
                map.AddDtl(new ProjectDocuments(), "project_id", "基本信息"); //文档.
                map.AddDtl(new InquiryBuildings(), "project_id", "基本信息"); //房栋信息.

                rm = new RefMethod();
                rm.Title = "增加房栋";
                rm.GroupName = "基本信息";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.ClassMethodName = this.ToString() + ".DoAddInquiryBuildings()";
                map.AddRefMethod(rm);
                #endregion  基本信息

                #region  产权评估
                rm = new RefMethod();
                rm.Title = "评估人列表";
                rm.GroupName = "产权评估";
                rm.ClassMethodName = this.ToString() + ".DoList1()";
                map.AddRefMethod(rm);
                map.AddDtl(new PrjShangGangRenYuans(), "project_id", "产权评估"); //房栋信息.
                #endregion  产权评估

                #region  调查管理
                rm = new RefMethod();
                rm.Title = "综合调查";
                rm.GroupName = "调查管理";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.ClassMethodName = this.ToString() + ".DoZongHeDiaoCha()";
                map.AddRefMethod(rm);
                #endregion  调查管理


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 基本信息.
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string DoList()
        {
            return "xxx";
        }
        /// <summary>
        /// 增加房栋
        /// </summary>
        /// <returns></returns>
        public string DoAddInquiryBuildings()
        {
          //  return "/WF/Comm/EnOnly.htm?EnName=BP.ZS.InquiryBuilding&project_id=" + this.No;
            return "/WF/Comm/EnOnly.htm?EnName=BP.ZS.InquiryBuilding&project_id=" + this.No;

        }
        #endregion 基本信息.

        /// <summary>
        /// 综合调查.
        /// </summary>
        /// <returns></returns>
        public string DoZongHeDiaoCha()
        {
            return "/App/ZhenShou/ZongHeDiaoCha.htm?No="+this.No+"&project_id=" + this.No;

        }

    }
    /// <summary>
    /// 项目信息s
    /// </summary>
    public class Projects : EntitiesNoName
    {
        /// <summary>
        /// 项目信息s
        /// </summary>
        public Projects() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Project();
            }
        }
    }
}
