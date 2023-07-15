using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Port;
using BP.En;
using BP.Web;
using BP.Sys;
using BP.WF.Data;
using BP.WF.Template.Frm;
using System.Threading;
using System.Collections.Generic;
using System.Web;
using BP.Difference;

namespace BP.WF.Template
{
    /// <summary>
    /// 流程
    /// </summary>
    public class FlowExt : EntityNoName
    {
        #region 属性.
        /// <summary>
        /// 流程类别
        /// </summary>
        public string FK_FlowSort
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.FK_FlowSort);
            }
            set
            {
                this.SetValByKey(FlowAttr.FK_FlowSort, value);
            }
        }
        /// <summary>
        /// 系统类别（第2级流程树节点编号）
        /// </summary>
        public string SysType
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.SysType);
            }
            set
            {
                this.SetValByKey(FlowAttr.SysType, value);
            }
        }

        /// <summary>
        /// 是否可以独立启动
        /// </summary>
        public bool IsCanStart
        {
            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsCanStart);
            }
            set
            {
                this.SetValByKey(FlowAttr.IsCanStart, value);
            }
        }
        /// <summary>
        /// 流程事件实体
        /// </summary>
        public string FlowEventEntity
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.FlowEventEntity);
            }
            set
            {
                this.SetValByKey(FlowAttr.FlowEventEntity, value);
            }
        }
        /// <summary>
        /// 流程标记
        /// </summary>
        public string FlowMark
        {
            get
            {
                string str = this.GetValStringByKey(FlowAttr.FlowMark);
                if (DataType.IsNullOrEmpty(str) == true)
                    return this.No;
                return str;
            }
            set
            {
                this.SetValByKey(FlowAttr.FlowMark, value);
            }
        }
        public bool IsStartInMobile
        {
            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsStartInMobile);
            }
            set
            {
                this.SetValByKey(FlowAttr.IsStartInMobile, value);
            }
        }

        /// <summary>
        /// 测试人员
        /// </summary>
        public string Tester
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.Tester);
            }
            set
            {
                this.SetValByKey(FlowAttr.Tester, value);
            }
        }
        #endregion 属性.

        #region 构造方法
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();

                if (WebUser.IsAdmin == false)
                    throw new Exception("err@管理员登录用户信息丢失,当前会话[" + WebUser.No + "," + WebUser.Name + "]");
                uac.IsUpdate = true;
                //uac.OpenForAdmin();  //zsy修改 2020.5.15
                //if (BP.Web.WebUser.No.Equals("admin")==true || this.DesignerNo == WebUser.No)
                uac.IsDelete = false;
                uac.IsInsert = false;
                return uac;
            }
        }
        /// <summary>
        /// 流程
        /// </summary>
        public FlowExt()
        {
        }
        /// <summary>
        /// 流程
        /// </summary>
        /// <param name="_No">编号</param>
        public FlowExt(string _No)
        {
            this.No = _No;
            if (BP.Difference.SystemConfig.IsDebug)
            {
                int i = this.RetrieveFromDBSources();
                if (i == 0)
                    throw new Exception("流程编号不存在");
            }
            else
            {
                this.Retrieve();
            }
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_Flow", "流程模版");

                #region 基本属性。
                map.AddGroupAttr("基本属性");
                map.AddTBStringPK(FlowAttr.No, null, "编号", true, true, 1, 4, 3);
                map.SetHelperUrl(FlowAttr.No, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3661868&doc_id=31094"); //使用alert的方式显示帮助信息.
                // map.AddDDLEntities(FlowAttr.FK_FlowSort, null, "类别", new FlowSorts(), true);
                //处理流程类别.
                string sql = "";
                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                {
                    //map.AddDDLEntities(FlowAttr.FK_FlowSort, null, "类别", new FlowSorts(), true);
                    sql = "SELECT No,Name FROM WF_FlowSort WHERE Name!='流程树' ORDER BY NO, IDX";
                    map.AddDDLSQL(FlowAttr.FK_FlowSort, null, "类别", sql, true);
                }
                else
                {
                    sql = "SELECT No,Name FROM WF_FlowSort WHERE OrgNo='@WebUser.OrgNo' ORDER BY NO, IDX";
                    map.AddDDLSQL(FlowAttr.FK_FlowSort, null, "类别", sql, true);
                }

                //map.AddDDLEntities(FlowAttr.FK_FlowSort, "01", "流程类别", new FlowSorts(), false);
                //map.SetHelperUrl(FlowAttr.FK_FlowSort, "http://ccbpm.mydoc.io/?v=5404&t=17024");

                map.AddTBString(FlowAttr.Name, null, "名称", true, false, 0, 50, 10, true);

                // add 2013-02-14 唯一确定此流程的标记
                map.AddTBString(FlowAttr.FlowMark, null, "流程标记", true, false, 0, 150, 10);
                map.AddTBString(FlowAttr.FlowEventEntity, null, "流程事件实体", true, true, 0, 150, 10);
                map.SetHelperUrl(FlowAttr.FlowMark, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3661870&doc_id=31094");
                map.SetHelperUrl(FlowAttr.FlowEventEntity, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3661871&doc_id=31094");

                // add 2013-02-05.
                map.AddTBString(FlowAttr.TitleRole, null, "标题生成规则", true, false, 0, 150, 10, true);
                map.SetHelperUrl(FlowAttr.TitleRole, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3661872&doc_id=31094");

                map.AddTBString(FlowAttr.TitleRoleNodes, null, "生成标题的节点", true, false, 0, 300, 10);
                string msg = "设置帮助";
                //msg += "\r\n 1. 如果为空表示只在开始节点生成标题.";
                //msg += "\r\n 2. * 表示在任意节点可生成标题.";
                //msg += "\r\n 3. 要在指定的节点重新生成标题用逗号分开,比如: 102,105,109";
                map.SetHelperUrl(FlowAttr.TitleRoleNodes, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3661873&doc_id=31094");


                map.AddBoolean(FlowAttr.IsCanStart, true, "可以独立启动否？(独立启动的流程可以显示在发起流程列表里)", true, true, true);
                map.SetHelperUrl(FlowAttr.IsCanStart, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3661874&doc_id=31094");

                map.AddBoolean(FlowAttr.IsFullSA, false, "是否自动计算未来的处理人？", true, true, true);
                map.SetHelperUrl(FlowAttr.IsFullSA, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3661875&doc_id=31094");

                //map.AddDDLSysEnum(FlowAttr.IsAutoSendSubFlowOver, 0, "为子流程时结束规则", true, true,
                // FlowAttr.IsAutoSendSubFlowOver, "@0=不处理@1=让父流程自动运行下一步@2=结束父流程");

                //map.AddBoolean(FlowAttr.GuestFlowRole, false, "是否外部用户参与流程(非组织结构人员参与的流程)", true, true, false);
                map.AddDDLSysEnum(FlowAttr.GuestFlowRole, (int)GuestFlowRole.None, "外部用户参与流程规则",
                 true, true, "GuestFlowRole", "@0=不参与@1=开始节点参与@2=中间节点参与");
                map.SetHelperUrl(FlowAttr.GuestFlowRole, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3661876&doc_id=31094");

                // map.AddDDLSysEnum(FlowAttr.FlowAppType, 0, "流程应用类型", true, true, "FlowAppType", "@0=业务流程@1=工程类(项目组流程)@2=公文流程(VSTO)");
                //map.SetHelperUrl(FlowAttr.FlowAppType, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3661877&doc_id=31094");

                //map.AddDDLSysEnum(FlowAttr.SDTOfFlow, (int)TimelineRole.ByNodeSet, "时效性规则",
                // true, true, FlowAttr.SDTOfFlow, "@0=按节点(由节点属性来定义)@1=按发起人(开始节点SysSDTOfFlow字段计算)");
                //map.SetHelperUrl(FlowAttr.TimelineRole, "http://ccbpm.mydoc.io/?v=5404&t=17036");

                // 草稿
                map.AddDDLSysEnum(FlowAttr.Draft, (int)DraftRole.None, "草稿规则",
               true, true, FlowAttr.Draft, "@0=无(不设草稿)@1=保存到待办@2=保存到草稿箱");
                map.SetHelperUrl(FlowAttr.Draft, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3661878&doc_id=31094");

                //为 莲荷科技增加一个系统类型, 用于存储当前所在流程树的第2级流程树编号.
                map.AddTBString(FlowAttr.SysType, null, "系统类型", false, false, 0, 50, 10, false);
                map.AddTBString(FlowAttr.Tester, null, "发起测试人", true, false, 0, 100, 10, true);
                map.SetHelperUrl(FlowAttr.Tester, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3661882&doc_id=31094");

                // add 2014-10-19.
                map.AddDDLSysEnum(FlowAttr.ChartType, (int)FlowChartType.Icon, "节点图形类型", true, true,
                    "ChartType", "@0=几何图形@1=肖像图片");
                map.SetHelperUrl(FlowAttr.ChartType, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3661883&doc_id=31094");
                map.AddTBString(FlowAttr.Idx, null, "显示顺序号(发起列表)", true, false, 0, 300, 10);
                // map.AddTBString(FlowAttr.HostRun, null, "运行主机(IP+端口)", true, false, 0, 40, 10, true);
                map.AddTBDateTime(FlowAttr.CreateDate, null, "创建日期", true, true);
                map.AddTBString(FlowAttr.Creater, null, "创建人", true, true, 0, 100, 10, true);
                #endregion 基本属性。

                #region 表单数据.
                map.AddGroupAttr("数据&表单");
                //批量发起 add 2013-12-27. 
                map.AddBoolean(FlowAttr.IsBatchStart, false, "是否可以批量发起流程？(如果是就要设置发起的需要填写的字段,多个用逗号分开)", false, true, true);
                map.AddTBString(FlowAttr.BatchStartFields, null, "发起字段s", true, false, 0, 100, 10, true);
                map.SetHelperUrl(FlowAttr.IsBatchStart, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3952886&doc_id=31094");

                //移动到这里 by zhoupeng 2016.04.08.
                map.AddBoolean(FlowAttr.IsResetData, false, "是否启用开始节点数据重置按钮？已经取消)", false, true, true);
                map.AddBoolean(FlowAttr.IsLoadPriData, false, "是否自动装载上一笔数据？", true, true, true);
                map.AddBoolean(FlowAttr.IsDBTemplate, true, "是否启用数据模版？", true, true, true);
                map.AddBoolean(FlowAttr.IsStartInMobile, true, "是否可以在手机里启用？(如果发起表单特别复杂就不要在手机里启用了)", true, true, true);
                map.SetHelperAlert(FlowAttr.IsStartInMobile, "用于控制手机端流程发起列表.");

                map.AddBoolean(FlowAttr.IsMD5, false, "是否是数据防止篡改(MD5数据加密防篡改)", true, true, true);
                map.SetHelperUrl(FlowAttr.IsMD5, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3952971&doc_id=31094");

                map.AddBoolean(FlowAttr.IsJM, false, "是否是数据加密流程(把所有字段加密存储)", true, true, true);
                map.SetHelperUrl(FlowAttr.IsJM, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3952997&doc_id=31094");

                map.AddBoolean(FlowAttr.IsEnableDBVer, false, "是否是启用数据版本控制", true, true, true);


                map.AddTBString(FlowAttr.PTable, null, "流程数据存储表", true, false, 0, 100, 10);
                map.SetHelperUrl(FlowAttr.PTable, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=4000827&doc_id=31094");


                //add  2013-08-30.
                map.AddTBString(FlowAttr.BillNoFormat, null, "单据编号格式", true, false, 0, 50, 10, false);
                map.SetHelperUrl(FlowAttr.BillNoFormat, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3953012&doc_id=31094");

                // add 2019-09-25 by zhoupeng
                map.AddTBString(FlowAttr.BuessFields, null, "关键业务字段", true, false, 0, 100, 10, false);
                msg = "用于显示在待办上的业务字段信息.";
                msg += "\t\n 1. 用户在看到待办的时候，就可以看到流程的实例的关键信息。";
                msg += "\t\n 2. 用于待办的列表信息显示.";
                msg += "\t\n 3. 配置格式为. Tel,Addr,Email  这些字段区分大小写并且是节点表单字段.";
                msg += "\t\n 4. 数据存储在WF_GenerWorkFlow.AtPara里面.";
                msg += "\t\n 5. 存储格式为: @BuessFields = 电话^Tel^18992323232;地址^Addr^山东杭州;";
                map.SetHelperAlert(FlowAttr.BuessFields, msg);

                map.AddTBString(FlowAttr.AdvEmps, null, "高级查询人员", true, false, 0, 100, 10, true);
                //msg = "用于指定人员查询流程实例数据集合的设置.";
                //msg += "\t\n 存储格式：人员编号以逗号分割如zhangsan,lisi,";
                map.SetHelperUrl(FlowAttr.AdvEmps, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=6513111&doc_id=31094");
                #endregion 表单数据.

                #region 轨迹信息
                map.AddGroupAttr("轨迹");
                //map.AddBoolean(FlowAttr.IsFrmEnable, false, "是否显示表单", true, true, false);
                map.AddBoolean(FlowAttr.IsTruckEnable, true, "是否显示轨迹图", true, true, false);
                map.AddBoolean(FlowAttr.IsTimeBaseEnable, true, "是否显示时间轴", true, true, false);
                map.AddBoolean(FlowAttr.IsTableEnable, true, "是否显示时间表", true, true, false);
                //map.AddBoolean(FlowAttr.IsOPEnable, false, "是否显示操作", true, true, false);
                map.AddDDLSysEnum(FlowAttr.SubFlowShowType, 0, "子流程轨迹图显示模式", true, true, FlowAttr.SubFlowShowType, "@0=平铺模式显示@1=合并模式显示");
                map.AddDDLSysEnum(FlowAttr.TrackOrderBy, 0, "排序方式", true, true, FlowAttr.TrackOrderBy, "@0=按照时间先后顺序@1=倒序(新发生的在前面)");
                #endregion 轨迹信息

                #region 基本功能.
                map.AddGroupMethod("基本功能");
                RefMethod rm = new RefMethod();
                rm = new RefMethod();
                rm.Title = "自动发起";
                //rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/AutoStart.png";
                rm.ClassMethodName = this.ToString() + ".DoSetStartFlowDataSources()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-plane";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "发起限制规则";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Limit.png";
                rm.ClassMethodName = this.ToString() + ".DoLimit()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-ban";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "发起前置导航";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/StartGuide.png";
                rm.ClassMethodName = this.ToString() + ".DoStartGuideV2019()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-list";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "流程事件"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoAction";
                rm.Icon = "../../WF/Img/Event.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-energy";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "流程消息"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoMessage";
                rm.Icon = "../../WF/Img/Message24.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-bubbles";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "轨迹查看权限";
                rm.Icon = "../../WF/Img/Setting.png";
                rm.ClassMethodName = this.ToString() + ".DoTruckRight()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-briefcase";
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "修改ICON"; // "调用事件接口";
                //rm.ClassMethodName = this.ToString() + ".DoNodesICON";
                ////  rm.Icon = "../../WF/Img/Event.png";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.Icon = "icon-heart";
                //map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "版本管理";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Node.png";
                rm.ClassMethodName = this.ToString() + ".DoVer()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-doc";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "权限控制";
                // rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Node.png";
                rm.ClassMethodName = this.ToString() + ".DoPowerModel()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                // rm.GroupName = "实验中的功能";
                rm.Icon = "icon-settings";
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "批量发起";
                //rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/AutoStart.png";
                //rm.ClassMethodName = this.ToString() + ".DoBatchStart()";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.Icon = "icon-calculator";
                //map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "流程模版";
                rm.Icon = "../../WF/Img/undo.png";
                rm.ClassMethodName = this.ToString() + ".DoExps()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-paper-plane";
                map.AddRefMethod(rm);
                #endregion 流程设置.

                #region 时限规则
                map.AddGroupMethod("时限规则");
                rm = new RefMethod();
                rm.Title = "计划时间计算规则";
                rm.ClassMethodName = this.ToString() + ".DoSDTOfFlow";
                //rm.Icon = "../../WF/Img/Event.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-clock";
                map.AddRefMethod(rm);

                /*rm = new RefMethod();
                rm.Icon = "../../WF/Admin/CCFormDesigner/Img/CH.png";
                rm.ClassMethodName = this.ToString() + ".DoDeadLineRole()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                // rm.GroupName = "实验中的功能";
                rm.Icon = "icon-clock";
                map.AddRefMethod(rm);*/

                rm = new RefMethod();
                rm.Title = "预警、超期消息事件";
                rm.Icon = "../../WF/Admin/CCFormDesigner/Img/OvertimeRole.png";
                rm.ClassMethodName = this.ToString() + ".DoOverDeadLineRole()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-energy";
                map.AddRefMethod(rm);
                #endregion 时限规则

                #region 开发接口.
                map.AddGroupMethod("开发接口");
                rm = new RefMethod();
                rm.Title = "与业务表数据同步";
                rm.ClassMethodName = this.ToString() + ".DoDTSBTable()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-paper-plane";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "URL调用接口";
                //rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/URL.png";
                rm.ClassMethodName = this.ToString() + ".DoAPI()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-paper-plane";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "SDK开发接口";
                //rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/API.png";
                rm.ClassMethodName = this.ToString() + ".DoAPICode()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-paper-plane";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "代码事件开发接口";
                rm.ClassMethodName = this.ToString() + ".DoAPICodeFEE()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-paper-plane";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "流程属性自定义";
                rm.ClassMethodName = this.ToString() + ".DoFlowAttrExt()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-paper-plane";
                map.AddRefMethod(rm);
                #endregion 开发接口.

                #region 流程运行维护.
                map.AddGroupMethod("流程维护");
                rm = new RefMethod();
                rm.Title = "重生成流程标题";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoGenerTitle()";
                //设置相关字段.
                //rm.RefAttrKey = FlowAttr.TitleRole;
                rm.RefAttrLinkLabel = "重新生成流程标题";
                rm.RefMethodType = RefMethodType.Func;
                rm.Target = "_blank";
                rm.Warning = "您确定要根据新的规则重新产生标题吗？";
                rm.Icon = "icon-briefcase";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "重生成FlowEmps字段";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoGenerFlowEmps()";
                rm.RefAttrLinkLabel = "补充修复emps字段，包括wf_generworkflow,NDxxxRpt字段.";
                rm.RefMethodType = RefMethodType.Func;
                rm.Target = "_blank";
                rm.Warning = "您确定要重新生成吗？";
                rm.Icon = "icon-briefcase";
                map.AddRefMethod(rm);

                //带有参数的方法.
                rm = new RefMethod();
                rm.Title = "重命名节点表单字段";
                //  rm.Warning = "您确定要处理吗？";
                rm.HisAttrs.AddTBString("FieldOld", null, "旧字段英文名", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("FieldNew", null, "新字段英文名", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("FieldNewName", null, "新字段中文名", true, false, 0, 100, 100);
                rm.HisAttrs.AddBoolen("thisFlowOnly", true, "仅仅当前流程");
                rm.Icon = "icon-briefcase";
                rm.ClassMethodName = this.ToString() + ".DoChangeFieldName";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "节点表单字段视图";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Field.png";
                rm.ClassMethodName = this.ToString() + ".DoFlowFields()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-briefcase";
                map.AddRefMethod(rm);

                //带有参数的方法.
                rm = new RefMethod();
                rm.Title = "删除指定日期范围内的流程";
                rm.Warning = "您确定要删除吗？";
                rm.Icon = "../../WF/Img/Btn/Delete.gif";
                rm.HisAttrs.AddTBDateTime("DTFrom", null, "时间从", true, false);
                rm.HisAttrs.AddTBDateTime("DTTo", null, "时间到", true, false);
                rm.HisAttrs.AddBoolen("thisFlowOnly", true, "仅仅当前流程");
                rm.Icon = "icon-briefcase";
                rm.ClassMethodName = this.ToString() + ".DoDelFlows";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Icon = "../../WF/Img/Btn/Delete.gif";
                rm.Title = "按工作ID删除"; // this.ToE("DelFlowData", "删除数据"); // "删除数据";
                rm.ClassMethodName = this.ToString() + ".DoDelDataOne";
                rm.HisAttrs.AddTBInt("WorkID", 0, "输入工作ID", true, false);
                rm.HisAttrs.AddTBString("beizhu", null, "删除备注", true, false, 0, 100, 100);
                rm.Icon = "icon-briefcase";
                map.AddRefMethod(rm);

                //带有参数的方法.
                rm = new RefMethod();
                rm.Title = "强制设置接收人";
                rm.HisAttrs.AddTBInt("WorkID", 0, "输入工作ID", true, false);
                rm.HisAttrs.AddTBInt("NodeID", 0, "节点ID", true, false);
                rm.HisAttrs.AddTBString("Worker", null, "接受人编号", true, false, 0, 100, 100);
                rm.Icon = "icon-briefcase";
                rm.ClassMethodName = this.ToString() + ".DoSetTodoEmps";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                //   rm.Icon = "../../WF/Img/Btn/Delete.gif";
                rm.Title = "按工作ID强制结束"; // this.ToE("DelFlowData", "删除数据"); // "删除数据";
                rm.ClassMethodName = this.ToString() + ".DoStopFlow";
                rm.HisAttrs.AddTBInt("WorkID", 0, "输入工作ID", true, false);
                rm.Icon = "icon-briefcase";
                rm.HisAttrs.AddTBString("beizhu", null, "备注", true, false, 0, 100, 100);
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "回滚流程";
                rm.Icon = "../../WF/Img/Btn/Back.png";
                rm.ClassMethodName = this.ToString() + ".DoRebackFlowData()";
                // rm.Warning = "您确定要回滚它吗？";
                rm.HisAttrs.AddTBInt("workid", 0, "请输入要会滚WorkID", true, false);
                rm.HisAttrs.AddTBInt("nodeid", 0, "回滚到的节点ID", true, false);
                rm.HisAttrs.AddTBString("note", null, "回滚原因", true, false, 0, 600, 200);
                rm.Icon = "icon-briefcase";
                map.AddRefMethod(rm);
                #endregion 流程运行维护.

                #region 流程监控.
                map.AddGroupMethod("流程监控");
                rm = new RefMethod();
                rm.Title = "设计报表"; // "报表运行";
                rm.Icon = "../../WF/Img/Btn/Rpt.gif";
                rm.ClassMethodName = this.ToString() + ".DoOpenRpt()";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Icon = "icon-briefcase";
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "监控面板";
                //rm.Icon = ../../Admin/CCBPMDesigner/Img/Monitor.png";
                //rm.ClassMethodName = this.ToString() + ".DoDataManger_Welcome()";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.GroupName = "流程监控";
                //map.AddRefMethod(rm);

                //  rm = new RefMethod();
                //rm.Title = "图形分析";
                //rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Group.png";
                //rm.ClassMethodName = this.ToString() + ".DoDataManger_DataCharts()";
                //rm.RefMethodType = RefMethodType.LinkeWinOpen;
                //rm.GroupName = "流程监控";
                //rm.Icon = "icon-briefcase";
                //map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "综合查询";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Search.png";
                rm.ClassMethodName = this.ToString() + ".DoDataManger_Search()";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Icon = "icon-briefcase";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "综合分析";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Group.png";
                rm.ClassMethodName = this.ToString() + ".DoDataManger_Group()";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Icon = "icon-briefcase";
                map.AddRefMethod(rm);


                //rm = new RefMethod();
                //rm.Title = "实例增长分析";
                //rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Grow.png";
                //rm.ClassMethodName = this.ToString() + ".DoDataManger_InstanceGrowOneFlow()";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.GroupName = "流程监控";
                //rm.Visable = false;
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "逾期未完成实例";
                //rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Warning.png";
                //rm.ClassMethodName = this.ToString() + ".DoDataManger_InstanceWarning()";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.GroupName = "流程监控";
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "逾期已完成实例";
                //rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/overtime.png";
                //rm.ClassMethodName = this.ToString() + ".DoDataManger_InstanceOverTimeOneFlow()";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.Visable = false;
                //rm.GroupName = "流程监控";
                //map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "删除日志";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/log.png";
                rm.ClassMethodName = this.ToString() + ".DoDataManger_DeleteLog()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-briefcase";
                map.AddRefMethod(rm);
                #endregion 流程监控.

                #region 实验中的功能
                map.AddGroupMethod("实验中的功能");
                rm = new RefMethod();
                rm.Title = "数据源管理(如果新增数据源后需要关闭重新打开)";
                rm.ClassMethodName = this.ToString() + ".DoDBSrc";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                //设置相关字段.
                rm.RefAttrKey = FlowAttr.DTSDBSrc;
                rm.RefAttrLinkLabel = "数据源管理";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                rm.Icon = "icon-briefcase";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "业务表字段同步配置";
                rm.ClassMethodName = this.ToString() + ".DoBTable";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                //设置相关字段.
                rm.RefAttrKey = FlowAttr.DTSField;
                rm.RefAttrLinkLabel = "业务表字段同步配置";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                rm.Icon = "icon-briefcase";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "一键设置审核组件工作模式";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Node.png";
                rm.RefMethodType = RefMethodType.Func;
                rm.Warning = "您确定要设置审核组件模式吗？ \t\n 1, 第2个节点以后的节点表单都指向第2个节点表单.  \t\n  2, 结束节点都设置为只读模式. ";
                rm.ClassMethodName = this.ToString() + ".DoSetFWCModel()";
                rm.Icon = "icon-briefcase";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "一键设置施工-监理模式";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Node.png";
                rm.RefMethodType = RefMethodType.Func;
                rm.ClassMethodName = this.ToString() + ".DoSDGaoSu()";
                rm.Icon = "icon-briefcase";
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "删除NDxxRpt表,多余字段.";
                //rm.ClassMethodName = this.ToString() + ".DoDeleteFields()";
                //rm.RefMethodType = RefMethodType.Func;
                //rm.Warning = "您确定要执行吗？ \t\n 1, 表NDxxxRpt是自动创建的.  \t\n  2, 在设置流程过程中有些多余的字段会生成到NDxxRpt表里. \t\n 3,这里是删除数据字段为null 并且是多余的字段.";
                //rm.GroupName = "实验中的功能";
                //rm.Icon = "icon-briefcase";
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "删除NDxRpt表,数据为null的多余字段.";
                //rm.ClassMethodName = this.ToString() + ".DoDeleteFieldsIsNull()";
                //rm.RefMethodType = RefMethodType.Func;
                //rm.Warning = "您确定要执行吗？ \t\n 1, 表NDxxxRpt是自动创建的.  \t\n  2, 在设置流程过程中有些多余的字段会生成到NDxxxRpt表里. \t\n 3,这里是删除数据字段为null 并且是多余的字段.";
                //rm.GroupName = "实验中的功能";
                //rm.Icon = "icon-briefcase";
                //map.AddRefMethod(rm);
                #endregion 实验中的功能

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 设置施工监理审核模式.
        /// </summary>
        /// <returns></returns>
        public string DoSDGaoSu()
        {
            Nodes nds = new Nodes();
            nds.Retrieve("FK_Flow", this.No);


            foreach (Node nd in nds)
            {
                if (nd.IsStartNode == true)
                    continue; //开始节点不处理.

                //如果第2个节点.
                if (nd.NodeID.ToString().EndsWith("02") == true)
                {
                    nd.HisDeliveryWay = DeliveryWay.BySQL;
                    //   nd.DeliveryParas = "";
                }
                else
                {
                    //接受人模式与指定的节点相同。
                    nd.HisDeliveryWay = DeliveryWay.BySpecNodeEmp;
                    nd.DeliveryParas = int.Parse(this.No + "02").ToString();
                }

                //设置协作模式.
                nd.TodolistModel = TodolistModel.Teamup;
                nd.IsExpSender = false; //是否排除当前人员.
                nd.Update();
            }

            return "执行成功";
        }

        #region 流程监控.

        public string DoDataManger_DataCharts()
        {
            return "../../Admin/AttrFlow/DataCharts.htm?FK_Flow=" + this.No;
        }


        public string DoDataManger_Search()
        {
            return "../../Comm/Search.htm?EnsName=BP.WF.Data.GenerWorkFlowViews&FK_Flow=" + this.No + "&WFSta=all";
        }
        public string DoDataManger_Group()
        {
            return "../../Comm/Group.htm?EnsName=BP.WF.Data.GenerWorkFlowViews&FK_Flow=" + this.No + "&WFSta=all";
        }


        public string DoDataManger_DeleteLog()
        {
            return "../../Comm/Search.htm?EnsName=BP.WF.WorkFlowDeleteLogs&FK_Flow=" + this.No + "&WFSta=all";
        }


        #endregion 流程监控.

        #region 开发接口.
        /// <summary>
        /// 执行删除指定日期范围内的流程
        /// </summary>
        /// <param name="dtFrom">日期从</param>
        /// <param name="dtTo">日期到</param>
        /// <param name="isOk">仅仅删除当前流程？1=删除当前流程, 0=删除全部流程.</param>
        /// <returns></returns>
        public string DoDelFlows(string dtFrom, string dtTo, Boolean isDelCurrFlow)
        {
            //if (BP.Web.WebUser.No != "admin")
            if (BP.Web.WebUser.IsAdmin == false)
                return "非管理员用户，不能删除。";


            string sql = "";
            if (isDelCurrFlow == true)
                sql = "SELECT WorkID, FK_Flow FROM WF_GenerWorkFlow  WHERE RDT >= '" + dtFrom + "' AND RDT <= '" + dtTo + "'  AND FK_Flow='" + this.No + "' ";
            else
                sql = "SELECT WorkID, FK_Flow FROM WF_GenerWorkFlow  WHERE RDT >= '" + dtFrom + "' AND RDT <= '" + dtTo + "' ";

            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            string msg = "如下流程ID被删除:";
            foreach (DataRow dr in dt.Rows)
            {
                Int64 workid = Int64.Parse(dr["WorkID"].ToString());
                string fk_flow = dr["FK_Flow"].ToString();
                DoDelFlowByWorkID(workid, fk_flow);
                msg += " " + workid;
            }
            return msg;
        }
        public void DoDelFlowByWorkID(Int64 workid, string fk_flow)
        {
            Flow flow = new Flow(fk_flow);
            #region 删除独立表单数据
            FrmNodes fns = new FrmNodes();
            fns.Retrieve(FrmNodeAttr.FK_Flow, fk_flow);
            string strs = "";
            foreach (FrmNode frmNode in fns)
            {
                if (strs.Contains("@" + frmNode.FK_Frm) == true)
                    continue;
                strs += "@" + frmNode.FK_Frm + "@";
                try
                {
                    MapData md = new MapData(frmNode.FK_Frm);
                    DBAccess.RunSQL("DELETE FROM " + md.PTable + " WHERE OID=" + workid);
                }
                catch
                {

                }
            }
            #endregion 删除独立表单数据

            //流程运行信息
            DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist Where (WorkID=" + workid + " OR FID=" + workid + ")");
            //删除退回信息
            //DBAccess.RunSQL("DELETE FROM WF_ReturnWork Where WorkID=" + workid);
            //考核信息
            DBAccess.RunSQL("DELETE FROM WF_CH Where (WorkID=" + workid + " OR FID=" + workid + ")");
            DBAccess.RunSQL("DELETE FROM WF_CHEval Where WorkID=" + workid);

            //接收人信息删除
            DBAccess.RunSQL("DELETE FROM WF_SelectAccper Where WorkID=" + workid);
            //流程信息流转自定义
            DBAccess.RunSQL("DELETE FROM WF_TransferCustom Where WorkID=" + workid);

            //流程remberme的信息
            DBAccess.RunSQL("DELETE FROM WF_RememberMe Where FK_Node IN(SELECT NodeID From WF_Node WHERE FK_Flow='" + fk_flow + "')");
            //删除流程的抄送信息
            DBAccess.RunSQL("DELETE FROM WF_CCList WHERE WorkID=" + workid);

            //删除流程实例的信息
            DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow Where (WorkID=" + workid + " OR FID=" + workid + ")");

            //删除track信息
            if (DBAccess.IsExitsObject("ND" + int.Parse(fk_flow) + "Track") == true)
                DBAccess.RunSQL("DELETE FROM ND" + int.Parse(fk_flow) + "Track Where (WorkID=" + workid + " OR FID=" + workid + ")");

            //删除表单信息
            if (DataType.IsNullOrEmpty(flow.PTable) == false && DBAccess.IsExitsObject(flow.PTable) == true)
                DBAccess.RunSQL("DELETE FROM " + flow.PTable + " Where OID=" + workid);
            //删除节点信息
            Nodes nds = new Nodes(fk_flow);
            foreach (Node nd in nds)
            {
                try
                {
                    Work wk = nd.HisWork;
                    DBAccess.RunSQL("DELETE FROM " + wk.EnMap.PhysicsTable + " Where OID=" + workid);
                }
                catch
                {

                }
                MapDtls dtls = new MapDtls("ND" + nd.NodeID);
                foreach (MapDtl dtl in dtls)
                {
                    try
                    {
                        DBAccess.RunSQL("DELETE FROM " + dtl.PTable + " WHERE RefPK=" + workid);
                    }
                    catch
                    {

                    }

                }
            }

        }
        /// <summary>
        /// 批量重命名字段.
        /// </summary>
        /// <param name="FieldOld"></param>
        /// <param name="FieldNew"></param>
        /// <param name="FieldNewName"></param>
        /// <returns></returns>
        public string DoChangeFieldName(string fieldOld, string fieldNew, string FieldNewName, string thisFlowOnly)
        {

            if (thisFlowOnly == "1")
                return DoChangeFieldNameOne(this, fieldOld, fieldNew, FieldNewName);

            FlowExts fls = new FlowExts();
            fls.RetrieveAll();

            string resu = "";
            foreach (FlowExt item in fls)
            {
                resu += "   ====   " + DoChangeFieldNameOne(item, fieldOld, fieldNew, FieldNewName);

            }
            return resu;
        }
        public string DoChangeFieldNameOne(FlowExt flow, string fieldOld, string fieldNew, string FieldNewName)
        {
            string result = "开始执行对字段:" + fieldOld + " ，进行重命名。";
            result += "<br> ===============================================================   ";
            Nodes nds = new Nodes(flow.No);
            foreach (Node nd in nds)
            {
                result += " @ 执行节点:" + nd.Name + " 结果如下. <br>";
                result += "<br> ------------------------------------------------------------------------ ";
                MapDataExt md = new MapDataExt("ND" + nd.NodeID);
                result += "\t\n @" + md.DoChangeFieldName(fieldOld, fieldNew, FieldNewName);
            }

            result += "@ 执行Rpt结果如下. <br>";
            MapDataExt rptMD = new MapDataExt("ND" + int.Parse(flow.No) + "Rpt");
            result += "\t\n@ " + rptMD.DoChangeFieldName(fieldOld, fieldNew, FieldNewName);

            result += "@ 执行MyRpt结果如下. <br>";
            rptMD = new MapDataExt("ND" + int.Parse(flow.No) + "MyRpt");

            if (rptMD.Retrieve() > 0)
                result += "\t\n@ " + rptMD.DoChangeFieldName(fieldOld, fieldNew, FieldNewName);

            return result;
        }
        /// <summary>
        /// 字段视图
        /// </summary>
        /// <returns></returns>
        public string DoFlowFields()
        {
            return "../../Admin/AttrFlow/FlowFields.htm?FK_Flow=" + this.No;
        }
        /// <summary>
        /// 与业务表数据同步
        /// </summary>
        /// <returns></returns>
        public string DoDTSBTable()
        {
            return "../../Admin/AttrFlow/DTSBTable.htm?FK_Flow=" + this.No;
        }
        public string DoAPI()
        {
            return "../../Admin/AttrFlow/API.htm?FK_Flow=" + this.No;
        }
        public string DoAPICode()
        {
            return "../../Admin/AttrFlow/APICode.htm?FK_Flow=" + this.No;
        }
        public string DoAPICodeFEE()
        {
            return "../../Admin/AttrFlow/APICodeFEE.htm?FK_Flow=" + this.No;
        }
        /// <summary>
        /// 流程属性自定义
        /// </summary>
        /// <returns></returns>
        public string DoFlowAttrExt()
        {
            return "../../../DataUser/OverrideFiles/FlowAttrExts.html?FK_Flow=" + this.No;
        }
        public string DoVer()
        {
            return "../../Admin/AttrFlow/Ver.htm?FK_Flow=" + this.No;
        }
        public string DoPowerModel()
        {
            return "../../Admin/AttrFlow/PowerModel.htm?FK_Flow=" + this.No;
        }

        /// <summary>
        /// 时限规则
        /// </summary>
        /// <returns></returns>
        public string DoDeadLineRole()
        {
            return "../../Admin/AttrFlow/DeadLineRole.htm?FK_Flow=" + this.No;
        }

        /// <summary>
        /// 预警、超期规则
        /// </summary>
        /// <returns></returns>
        public string DoOverDeadLineRole()
        {
            return "../../Admin/AttrFlow/PushMessage.htm?FK_Flow=" + this.No;
        }


        #endregion 开发接口

        #region  基本功能
        /// <summary>
        /// 事件
        /// </summary>
        /// <returns></returns>
        public string DoAction()
        {
            return "../../Admin/AttrFlow/Action.htm?FK_Flow=" + this.No + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// 流程事件
        /// </summary>
        /// <returns></returns>
        public string DoMessage()
        {
            return "../../Admin/AttrFlow/PushMessage.htm?FK_Node=0&FK_Flow=" + this.No + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// 计划玩成
        /// </summary>
        /// <returns></returns>
        public string DoSDTOfFlow()
        {
            return "../../Admin/AttrFlow/SDTOfFlow/Default.htm?FK_Flow=" + this.No + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// 节点标签
        /// </summary>
        /// <returns></returns>
        public string DoNodesICON()
        {
            return "../../Admin/AttrFlow/NodesIcon.htm?FK_Flow=" + this.No + "&tk=" + new Random().NextDouble();
        }
        public string DoDBSrc()
        {
            return "../../Comm/Sys/SFDBSrcNewGuide.htm";
        }
        public string DoBTable()
        {
            return "../../Admin/AttrFlow/DTSBTable.htm?s=d34&ShowType=FlowFrms&FK_Node=" + int.Parse(this.No) + "01&FK_Flow=" + this.No + "&ExtType=StartFlow&RefNo=" + DataType.CurrentDateTime;
        }
        /// <summary>
        /// 批量发起
        /// </summary>
        /// <returns></returns>
        public string DoBatchStart()
        {
            return "../../Admin/AttrFlow/BatchStart.htm?s=d34&ShowType=FlowFrms&FK_Node=" + int.Parse(this.No) + "01&FK_Flow=" + this.No + "&ExtType=StartFlow&RefNo=" + DataType.CurrentDateTime;
        }
        /// <summary>
        /// 批量修改节点属性
        /// </summary>
        /// <returns></returns>
        public string DoNodeAttrs()
        {
            return "../../Admin/AttrFlow/NodeAttrs.htm?NodeID=0&FK_Flow=" + this.No + "&tk=" + new Random().NextDouble();
        }
        public string DoBindFlowExt()
        {
            return "../../Admin/Sln/BindFrms.htm?s=d34&ShowType=FlowFrms&FK_Node=0&FK_Flow=" + this.No + "&ExtType=StartFlow";
        }
        /// <summary>
        /// 轨迹查看权限
        /// </summary>
        /// <returns></returns>
        public string DoTruckRight()
        {
            return "../../Admin/AttrFlow/TruckViewPower.htm?FK_Flow=" + this.No;
        }
        /// <summary>
        /// 批量发起字段
        /// </summary>
        /// <returns></returns>
        public string DoBatchStartFields()
        {
            return "../../Admin/AttrNode/BatchStartFields.htm?s=d34&FK_Flow=" + this.No + "&ExtType=StartFlow";
        }
        /// <summary>
        /// 执行流程数据表与业务表数据手工同步
        /// </summary>
        /// <returns></returns>
        public string DoBTableDTS()
        {
            Flow fl = new Flow(this.No);
            return fl.DoBTableDTS();

        }
        /// <summary>
        /// 恢复已完成的流程数据到指定的节点，如果节点为0就恢复到最后一个完成的节点上去.
        /// </summary>
        /// <param name="workid">要恢复的workid</param>
        /// <param name="backToNodeID">恢复到的节点编号，如果是0，标示回复到流程最后一个节点上去.</param>
        /// <param name="note"></param>
        /// <returns></returns>
        public string DoRebackFlowData(Int64 workid, int backToNodeID, string note)
        {
            if (note.Length <= 2)
                return "请填写恢复已完成的流程原因.";

            Flow fl = new Flow(this.No);
            BP.WF.GERpt rpt = new BP.WF.GERpt("ND" + int.Parse(this.No) + "Rpt");
            rpt.OID = workid;
            int i = rpt.RetrieveFromDBSources();
            if (i == 0)
                throw new Exception("@错误，流程数据丢失。");
            if (backToNodeID == 0)
                backToNodeID = rpt.FlowEndNode;

            Emp empStarter = new Emp(rpt.FlowStarter);

            // 最后一个节点.
            Node endN = new Node(backToNodeID);
            GenerWorkFlow gwf = null;
            bool isHaveGener = false;
            try
            {
                #region 创建流程引擎主表数据.
                gwf = new GenerWorkFlow();
                gwf.WorkID = workid;
                if (gwf.RetrieveFromDBSources() == 1)
                {
                    isHaveGener = true;
                    //判断状态
                    if (gwf.WFState != WFState.Complete)
                        throw new Exception("@当前工作ID为:" + workid + "的流程没有结束,不能采用此方法恢复。");
                }

                gwf.FK_Flow = this.No;
                gwf.FlowName = this.Name;
                gwf.WorkID = workid;
                gwf.PWorkID = rpt.PWorkID;
                gwf.PFlowNo = rpt.PFlowNo;
                gwf.PNodeID = rpt.PNodeID;
                gwf.PEmp = rpt.PEmp;


                gwf.FK_Node = backToNodeID;
                gwf.NodeName = endN.Name;

                gwf.Starter = rpt.FlowStarter;
                gwf.StarterName = empStarter.Name;
                gwf.FK_FlowSort = fl.FK_FlowSort;
                gwf.SysType = fl.SysType;

                gwf.Title = rpt.Title;
                gwf.WFState = WFState.ReturnSta; /*设置为退回的状态*/
                gwf.TaskSta = TaskSta.None;/**取消共享模式*/
                gwf.FK_Dept = rpt.FK_Dept;

                Dept dept = new Dept(rpt.FK_Dept);

                gwf.DeptName = dept.Name;
                gwf.PRI = 1;

                DateTime dttime = DateTime.Now;
                dttime = dttime.AddDays(3);

                gwf.SDTOfNode = dttime.ToString("yyyy-MM-dd HH:mm:ss");
                gwf.SDTOfFlow = dttime.ToString("yyyy-MM-dd HH:mm:ss");
                if (isHaveGener)
                    gwf.Update();
                else
                    gwf.Insert(); /*插入流程引擎数据.*/

                #endregion 创建流程引擎主表数据
                string ndTrack = "ND" + int.Parse(this.No) + "Track";
                string actionType = (int)ActionType.Forward + "," + (int)ActionType.FlowOver + "," + (int)ActionType.ForwardFL + "," + (int)ActionType.ForwardHL + "," + (int)ActionType.Skip;
                string sql = "SELECT  * FROM " + ndTrack + " WHERE   ActionType IN (" + actionType + ")  and WorkID=" + workid + " ORDER BY RDT DESC, NDFrom ";
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0)
                    throw new Exception("@工作ID为:" + workid + "的数据不存在.");

                string starter = "";
                bool isMeetSpecNode = false;
                GenerWorkerList currWl = new GenerWorkerList();
                foreach (DataRow dr in dt.Rows)
                {
                    int ndFrom = int.Parse(dr["NDFrom"].ToString());
                    Node nd = new Node(ndFrom);

                    string ndFromT = dr["NDFromT"].ToString();
                    string EmpFrom = dr[TrackAttr.EmpFrom].ToString();
                    string EmpFromT = dr[TrackAttr.EmpFromT].ToString();

                    // 增加上 工作人员的信息.
                    GenerWorkerList gwl = new GenerWorkerList();
                    gwl.WorkID = workid;
                    gwl.FK_Flow = this.No;

                    gwl.FK_Node = ndFrom;
                    gwl.FK_NodeText = ndFromT;

                    if (gwl.FK_Node == backToNodeID)
                    {
                        gwl.IsPass = false;
                        currWl = gwl;
                    }
                    else
                        gwl.IsPass = true;

                    gwl.FK_Emp = EmpFrom;
                    gwl.FK_EmpText = EmpFromT;
                    if (gwl.IsExits)
                        continue; /*有可能是反复退回的情况.*/

                    Emp emp = new Emp(gwl.FK_Emp);
                    gwl.FK_Dept = emp.FK_Dept;

                    gwl.SDT = dr["RDT"].ToString();
                    gwl.DTOfWarning = gwf.SDTOfNode;

                    gwl.IsEnable = true;
                    gwl.WhoExeIt = nd.WhoExeIt;
                    gwl.Insert();
                }

                #region 加入退回信息, 让接受人能够看到退回原因.
                ReturnWork rw = new ReturnWork();
                rw.Delete(ReturnWorkAttr.WorkID, workid); //先删除历史的信息.

                rw.WorkID = workid;
                rw.ReturnNode = backToNodeID;
                rw.ReturnNodeName = endN.Name;
                rw.Returner = WebUser.No;
                rw.ReturnerName = WebUser.Name;

                rw.ReturnToNode = currWl.FK_Node;
                rw.ReturnToEmp = currWl.FK_Emp;
                rw.BeiZhu = note;
                rw.RDT = DataType.CurrentDateTime;
                rw.IsBackTracking = false;
                rw.setMyPK(DBAccess.GenerGUID());
                rw.Insert();
                #endregion   加入退回信息, 让接受人能够看到退回原因.

                //更新流程表的状态.
                rpt.FlowEnder = currWl.FK_Emp;
                rpt.WFState = WFState.ReturnSta; /*设置为退回的状态*/
                rpt.FlowEndNode = currWl.FK_Node;
                rpt.Update();

                // 向接受人发送一条消息.
                BP.WF.Dev2Interface.Port_SendMsg(currWl.FK_Emp, "工作恢复:" + gwf.Title, "工作被:" + WebUser.No + " 恢复." + note, "ReBack" + workid, BP.WF.SMSMsgType.SendSuccess, this.No, int.Parse(this.No + "01"), workid, 0);

                //写入该日志.
                WorkNode wn = new WorkNode(workid, currWl.FK_Node);
                wn.AddToTrack(ActionType.RebackOverFlow, currWl.FK_Emp, currWl.FK_EmpText, currWl.FK_Node, currWl.FK_NodeText, note);

                return "@已经还原成功,现在的流程已经复原到(" + currWl.FK_NodeText + "). @当前工作处理人为(" + currWl.FK_Emp + " , " + currWl.FK_EmpText + ")  @请通知他处理工作.";
            }
            catch (Exception ex)
            {
                //此表的记录删除已取消
                //gwf.Delete();
                GenerWorkerList wl = new GenerWorkerList();
                wl.Delete(GenerWorkerListAttr.WorkID, workid);

                string sqls = "";
                sqls += "@UPDATE " + fl.PTable + " SET WFState=" + (int)WFState.Complete + " WHERE OID=" + workid;
                DBAccess.RunSQLs(sqls);
                return "<font color=red>会滚期间出现错误</font><hr>" + ex.Message;
            }
        }

        /// <summary>
        /// 重新产生标题，根据新的规则.
        /// </summary>
        public string DoGenerFlowEmps()
        {
            if (WebUser.IsAdmin == false)
                return "非admin用户不能执行。";

            Flow fl = new Flow(this.No);

            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.Retrieve(GenerWorkFlowAttr.FK_Flow, this.No);
            foreach (GenerWorkFlow gwf in gwfs)
            {
                string emps = "@";
                string sql = "SELECT EmpFrom,EmpFromT FROM ND" + int.Parse(this.No) + "Track  WHERE WorkID=" + gwf.WorkID;

                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    if (emps.Contains("@" + dr[0].ToString() + "@") || emps.Contains("@" + dr[0].ToString() + "," + dr[1].ToString() + "@"))
                        continue;
                    emps += dr[0].ToString() + "," + dr[1].ToString() + "@";
                }

                sql = "UPDATE " + fl.PTable + " SET FlowEmps='" + emps + "' WHERE OID=" + gwf.WorkID;
                DBAccess.RunSQL(sql);

                sql = "UPDATE WF_GenerWorkFlow SET Emps='" + emps + "' WHERE WorkID=" + gwf.WorkID;
                DBAccess.RunSQL(sql);
            }

            Node nd = fl.HisStartNode;
            Works wks = nd.HisWorks;
            wks.RetrieveAllFromDBSource(WorkAttr.Rec);
            string table = nd.HisWork.EnMap.PhysicsTable;
            string tableRpt = "ND" + int.Parse(this.No) + "Rpt";
            Sys.MapData md = new Sys.MapData(tableRpt);
            foreach (Work wk in wks)
            {
                GERpt rpt = new GERpt(tableRpt, wk.OID);
                if (rpt.FlowStarter != WebUser.No)
                {
                    BP.Web.WebUser.Exit();
                    try
                    {
                        Emp emp = new Emp(rpt.FlowStarter);
                        BP.Web.WebUser.SignInOfGener(emp);
                    }
                    catch
                    {
                        continue;
                    }
                }
                string sql = "";
                string title = BP.WF.WorkFlowBuessRole.GenerTitle(fl, wk);
                Paras ps = new Paras();
                ps.Add("Title", title);
                ps.Add("OID", wk.OID);
                ps.SQL = "UPDATE " + table + " SET Title=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Title WHERE OID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

                ps.SQL = "UPDATE " + md.PTable + " SET Title=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Title WHERE OID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

                ps.SQL = "UPDATE WF_GenerWorkFlow SET Title=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Title WHERE WorkID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);
            }
            Emp emp1 = new Emp("admin");
            BP.Web.WebUser.SignInOfGener(emp1);
            return "全部生成成功,影响数据(" + wks.Count + ")条";
        }

        /// <summary>
        /// 重新产生标题，根据新的规则.
        /// </summary>
        public string DoGenerTitle()
        {
            if (WebUser.IsAdmin == false)
                return "非管理员用户不能执行。";
            string adminNo = WebUser.No;
            Flow fl = new Flow(this.No);
            Node nd = fl.HisStartNode;
            Works wks = nd.HisWorks;
            wks.RetrieveAllFromDBSource(WorkAttr.Rec);
            string table = nd.HisWork.EnMap.PhysicsTable;
            string tableRpt = "ND" + int.Parse(this.No) + "Rpt";
            MapData md = new MapData(tableRpt);
            foreach (Work wk in wks)
            {

                if (wk.Rec != WebUser.No)
                {
                    WebUser.Exit();
                    try
                    {
                        Emp emp = new Emp(wk.Rec);
                        WebUser.SignInOfGener(emp);
                    }
                    catch
                    {
                        continue;
                    }
                }
                string title = WorkFlowBuessRole.GenerTitle(fl, wk);
                Paras ps = new Paras();
                ps.Add("Title", title);
                ps.Add("OID", wk.OID);
                if (DBAccess.IsExitsTableCol(md.PTable, "Title") == true)
                {
                    ps.SQL = "UPDATE " + md.PTable + " SET Title=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Title WHERE OID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "OID";
                    DBAccess.RunSQL(ps);
                }

                if (table.Equals(md.PTable) == false && DBAccess.IsExitsTableCol(table, "Title") == true)
                {
                    ps.SQL = "UPDATE " + table + " SET Title=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Title WHERE OID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "OID";
                    DBAccess.RunSQL(ps);
                }

                ps.SQL = "UPDATE WF_GenerWorkFlow SET Title=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Title WHERE WorkID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

            }
            Emp emp1 = new Emp(adminNo);
            WebUser.SignInOfGener(emp1);

            return "全部生成成功,影响数据(" + wks.Count + ")条";
        }


        /// <summary>
        /// 定义报表
        /// </summary>
        /// <returns></returns>
        public string DoAutoStartIt()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            return fl.DoAutoStartIt();
        }
        /// <summary>
        /// 强制设置接受人
        /// </summary>
        /// <param name="workid">工作人员ID</param>
        /// <param name="nodeID">节点ID</param>
        /// <param name="worker">工作人员</param>
        /// <returns>执行结果.</returns>
        public string DoSetTodoEmps(int workid, int nodeID, string worker)
        {
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workid;
            if (gwf.RetrieveFromDBSources() == 0)
                return "workid=" + workid + "不正确.";

            BP.Port.Emp emp = new Emp();
            emp.UserID = worker;
            if (emp.RetrieveFromDBSources() == 0)
                return "人员编号不正确" + worker + ".";

            BP.WF.Node nd = new Node();
            nd.NodeID = nodeID;
            if (nd.RetrieveFromDBSources() == 0)
                return "err@节点编号[" + nodeID + "]不正确.";

            gwf.FK_Node = nodeID;
            gwf.NodeName = nd.Name;
            gwf.TodoEmps = emp.UserID + "," + emp.Name + ";";
            gwf.TodoEmpsNum = 1;
            gwf.HuiQianTaskSta = HuiQianTaskSta.None;
            gwf.Update();

            DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsPass=1 WHERE WorkID=" + workid);

            GenerWorkerList gwl = new GenerWorkerList();
            gwl.FK_Node = nodeID;
            gwl.WorkID = workid;
            gwl.FK_Emp = emp.UserID;
            if (gwl.RetrieveFromDBSources() == 0)
            {
                DateTime dt = DateTime.Now;
                gwl.FK_EmpText = emp.Name;

                if (nd.HisCHWay == CHWay.None)
                    gwl.SDT = "无";
                else
                    gwl.SDT = dt.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss");

                gwl.RDT = dt.ToString("yyyy-MM-dd HH:mm:ss");
                gwl.IsRead = false;
                gwl.Insert();
            }
            else
            {
                gwl.IsRead = false;
                gwl.IsPassInt = 0;
                gwl.Update();
            }
            return "执行成功.";
        }
        /// <summary>
        /// 删除流程
        /// </summary>
        /// <param name="workid"></param>
        /// <param name="sd"></param>
        /// <returns></returns>
        public string DoDelDataOne(int workid, string note)
        {
            try
            {
                BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(workid, true);
                return "删除成功 workid=" + workid + "  理由:" + note;
            }
            catch (Exception ex)
            {
                return "删除失败:" + ex.Message;
            }
        }
        public string DoStopFlow(Int64 workid, string note)
        {
            try
            {
                BP.WF.Dev2Interface.Flow_DoFlowOver(workid, note);
                return "流程被强制结束 workid=" + workid + "  理由:" + note;
            }
            catch (Exception ex)
            {
                return "删除失败:" + ex.Message;
            }
        }
        /// <summary>
        /// 设置发起数据源
        /// </summary>
        /// <returns></returns>
        public string DoSetStartFlowDataSources()
        {
            if (DataType.IsNullOrEmpty(this.No) == true)
                throw new Exception("传入的流程编号为空，请检查流程");
            string flowID = int.Parse(this.No).ToString() + "01";
            //return "../../Admin/AttrFlow/AutoStart.htm?s=d34&FK_Flow=" + this.No + "&ExtType=StartFlow&RefNo=";
            return "../../Admin/AttrFlow/AutoStart/Default.htm?FK_Flow=" + this.No;
        }
        /// <summary>
        /// 执行运行
        /// </summary>
        /// <returns></returns>
        public string DoRunIt()
        {
            return "../../Admin/TestFlow.htm?FK_Flow=" + this.No + "&Lang=CH";
        }
        /// <summary>
        /// 执行检查
        /// </summary>
        /// <returns></returns>
        public string DoCheck()
        {
            return "../../Admin/AttrFlow/CheckFlow.htm?FK_Flow=" + this.No + "&Lang=CH";
        }
        public string TestingContainer()
        {
            return "../../Admin/TestingContainer/TestFlow2020.htm?FK_Flow=" + this.No + "&Lang=CH";
        }
        public string DoCheck2018Url()
        {
            return "../../Admin/Testing/FlowCheckError.htm?FK_Flow=" + this.No + "&Lang=CH";
        }
        /// <summary>
        /// 启动限制规则
        /// </summary>
        /// <returns>返回URL</returns>
        public string DoLimit()
        {
            return "../../Admin/AttrFlow/Limit/Default.htm?FK_Flow=" + this.No + "&Lang=CH";
        }
        /// <summary>
        /// 设置发起前置导航
        /// </summary>
        /// <returns></returns>
        public string DoStartGuide()
        {
            return "../../Admin/AttrFlow/StartGuide.htm?FK_Flow=" + this.No + "&Lang=CH";
        }
        /// <summary>
        /// 设置发起前置导航
        /// </summary>
        /// <returns></returns>
        public string DoStartGuideV2019()
        {
            return "../../Admin/AttrFlow/StartGuide/Default.htm?FK_Flow=" + this.No + "&Lang=CH";
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <returns></returns>
        public string DoImp()
        {
            return "../../Admin/AttrFlow/Imp.htm?FK_Flow=" + this.No + "&Lang=CH&FK_FlowSort=" + this.FK_FlowSort;
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        public string DoExps()
        {
            return "../../Admin/AttrFlow/Exp.htm?FK_Flow=" + this.No + "&Lang=CH";
        }

        /// <summary>
        /// 删除数据.
        /// </summary>
        /// <returns></returns>
        public string DoDelData()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            return fl.DoDelData();
        }
        /// <summary>
        /// 运行报表
        /// </summary>
        /// <returns></returns>
        public string DoOpenRpt()
        {
            if (DataType.IsNullOrEmpty(this.No) == true)
                throw new Exception("传入的流程编号为空，请检查流程");
            return "../../Admin/RptDfine/Default.htm?FK_Flow=" + this.No + "&DoType=Edit&FK_MapData=ND" + int.Parse(this.No) + "Rpt";
        }
        /// <summary>
        /// 更新之后的事情，也要更新缓存。
        /// </summary>
        protected override void afterUpdate()
        {
            //写入日志.
            BP.Sys.Base.Glo.WriteUserLog("更新流程属性：" + this.Name + " - " + this.No);

            // Flow fl = new Flow();
            // fl.No = this.No;
            // fl.RetrieveFromDBSources();
            //fl.Update();
        }
        protected override bool beforeUpdate()
        {
            //检查设计流程权限,集团模式下，不是自己创建的流程，不能设计流程.
            BP.WF.Template.TemplateGlo.CheckPower(this.No);

            //更新流程版本
            Flow.UpdateVer(this.No);

            #region 同步事件实体.
            try
            {
                string flowMark = this.FlowMark;
                if (DataType.IsNullOrEmpty(flowMark) == true)
                    flowMark = this.No;

                this.FlowEventEntity = BP.WF.Glo.GetFlowEventEntityStringByFlowMark(flowMark);
            }
            catch
            {
                this.FlowEventEntity = "";
            }
            #endregion 同步事件实体.

            //更新缓存数据。
            Flow fl = new Flow(this.No);
            fl.RetrieveFromDBSources();

            #region StartFlows的清缓存
            if (fl.IsStartInMobile != this.IsStartInMobile || fl.IsCanStart != this.IsCanStart || fl.Name.Equals(this.Name) == false)
            {
                //清空WF_Emp 的StartFlows
                DBAccess.RunSQL("UPDATE  WF_Emp Set StartFlows =''");
            }
            #endregion StartFlows的清缓存

            fl.Copy(this);

            //2019-09-25 -by zhoupeng 为周大福增加 关键业务字段.
            fl.BuessFields = this.GetValStrByKey(FlowAttr.BuessFields);
            fl.DirectUpdate();

            #region 检查数据完整性 - 同步业务表数据。
            // 检查业务是否存在.
            if (fl.DTSWay != DataDTSWay.None)
            {
                /*检查业务表填写的是否正确.*/
                string sql = "select count(*) as Num from  " + fl.DTSBTable + " where 1=2";
                try
                {
                    DBAccess.RunSQLReturnValInt(sql, 0);
                }
                catch (Exception)
                {
                    throw new Exception("@业务表配置无效，您配置业务数据表[" + fl.DTSBTable + "]在数据中不存在，请检查拼写错误如果是跨数据库请加上用户名比如: for sqlserver: HR.dbo.Emps, For oracle: HR.Emps");
                }

                sql = "select " + fl.DTSBTablePK + " from " + fl.DTSBTable + " where 1=2";
                try
                {
                    DBAccess.RunSQLReturnValInt(sql, 0);
                }
                catch (Exception)
                {
                    throw new Exception("@业务表配置无效，您配置业务数据表[" + fl.DTSBTablePK + "]的主键不存在。");
                }


                //检查节点配置是否符合要求.
                if (fl.DTSTime == FlowDTSTime.SpecNodeSend)
                {
                    // 检查节点ID，是否符合格式.
                    string nodes = fl.DTSSpecNodes;
                    nodes = nodes.Replace("，", ",");
                    this.SetValByKey(FlowAttr.DTSSpecNodes, nodes);

                    if (DataType.IsNullOrEmpty(nodes) == true)
                        throw new Exception("@业务数据同步数据配置错误，您设置了按照指定的节点配置，但是您没有设置节点,节点的设置格式如下：101,102,103");

                    string[] strs = nodes.Split(',');
                    foreach (string str in strs)
                    {
                        if (DataType.IsNullOrEmpty(str) == true)
                            continue;

                        if (DataType.IsNumStr(str) == false)
                            throw new Exception("@业务数据同步数据配置错误，您设置了按照指定的节点配置，但是节点格式错误[" + nodes + "]。正确的格式如下：101,102,103");

                        Node nd = new Node();
                        nd.NodeID = int.Parse(str);
                        if (nd.IsExits == false)
                            throw new Exception("@业务数据同步数据配置错误，您设置的节点格式错误，节点[" + str + "]不是有效的节点。");

                        nd.RetrieveFromDBSources();
                        if (nd.FK_Flow != this.No)
                            throw new Exception("@业务数据同步数据配置错误，您设置的节点[" + str + "]不再本流程内。");
                    }
                }

                //检查流程数据存储表是否正确
                if (DataType.IsNullOrEmpty(fl.PTable) == false)
                {
                    /*检查流程数据存储表填写的是否正确.*/
                    sql = "select count(*) as Num from  " + fl.PTable + " WHERE 1=2";
                    try
                    {
                        DBAccess.RunSQLReturnValInt(sql, 0);
                    }
                    catch (Exception)
                    {
                        throw new Exception("@流程数据存储表配置无效，您配置流程数据存储表[" + fl.PTable + "]在数据中不存在，请检查拼写错误如果是跨数据库请加上用户名比如: for sqlserver: HR.dbo.Emps, For oracle: HR.Emps");
                    }
                }
            }
            #endregion 检查数据完整性. - 同步业务表数据。

            return base.beforeUpdate();
        }
        protected override void afterInsertUpdateAction()
        {
            #region 更新PTale 后的业务处理 
            // 同步流程数据表.
            string ndxxRpt = "ND" + int.Parse(this.No) + "Rpt";
            Flow fl = new Flow(this.No);
            //判断流程是不是有对应的实体绑定的流程,不同步修改低代码导出系统导入时报错 @hongyan
            if (DBAccess.IsExitsObject("Frm_Method") == true)
            {
                string dbstr = SystemConfig.AppCenterDBVarStr;
                string sql = "UPDATE Frm_Method SET Name=" + dbstr + "Name WHERE FlowNo=" + dbstr + "FlowNo AND MethodID=" + dbstr + "MethodID";
                Paras paras = new Paras();
                paras.SQL = sql;
                paras.Add("Name", this.Name);
                paras.Add("FlowNo", this.No);
                paras.Add("MethodID", this.No);
                DBAccess.RunSQL(paras);
            }
            
            MapData md = new MapData(ndxxRpt);
            if (md.PTable.Equals(fl.PTable) == false)
            {
                md.PTable = fl.PTable;
                md.Update();

                Nodes nds = new Nodes();
                nds.Retrieve(NodeAttr.FK_Flow, this.No);
                foreach (Node nd in nds)
                {
                   string sql = "";
                    if (nd.IsSubThread == true)
                        sql = "UPDATE Sys_MapData SET PTable=No WHERE No='ND" + nd.NodeID + "'";
                    else
                        sql = "UPDATE Sys_MapData SET PTable='" + fl.PTable + "' WHERE No='ND" + nd.NodeID + "'";

                    DBAccess.RunSQL(sql);
                }
                fl.CheckRpt(); // 检查业务表.
            }
            #endregion 更新PTale 后的业务处理 


            #region 为systype设置，当前所在节点的第2级别目录。
            FlowSort fs = new FlowSort(fl.FK_FlowSort);
            if (DataType.IsNullOrEmpty(fs.ParentNo) == true)
                fs.ParentNo = "0";

            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
            {
                if (fs.ParentNo.Equals(WebUser.OrgNo) == true || fs.ParentNo.Equals("100") == true)
                    this.SysType = fl.FK_FlowSort;
                else
                {
                    FlowSort fsP = new FlowSort(fs.ParentNo);
                    if (fs.ParentNo.Equals(WebUser.OrgNo) == true)
                    {
                        this.SysType = fsP.No;
                    }
                    else
                    {
                        FlowSort fsPP = new FlowSort(fsP.ParentNo);
                        this.SysType = fsPP.No;
                    }
                }
            }
            else
            {
                if (fs.ParentNo.Equals("99") || fs.ParentNo.Equals("0"))
                {
                    this.SysType = fl.FK_FlowSort;
                }
                else
                {
                    FlowSort fsP = new FlowSort(fs.ParentNo);
                    fsP.No = fs.ParentNo;

                    if (fsP.ParentNo == "99" || fsP.ParentNo == "0")
                    {
                        this.SysType = fsP.No;
                    }
                    else
                    {
                        FlowSort fsPP = new FlowSort();
                        fsPP.No = fsP.ParentNo;
                        if (fsPP.RetrieveFromDBSources() == 1)
                            this.SysType = fsPP.No;
                    }
                }
            }
            #endregion 为systype设置，当前所在节点的第2级别目录。

            fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            fl.Update();

            base.afterInsertUpdateAction();
        }
        #endregion

        #region 实验中的功能.
        /// <summary>
        /// 删除多余的字段.
        /// </summary>
        /// <returns></returns>
        public string DoDeleteFields()
        {
            return "尚未完成.";
        }
        /// <summary>
        /// 删除多余的字段.
        /// </summary>
        /// <returns></returns>
        public string DoDeleteFieldsIsNull()
        {
            return "尚未完成.";
        }
        /// <summary>
        /// 一件设置审核模式.
        /// </summary>
        /// <returns></returns>
        public string DoSetFWCModel()
        {
            GroupField gf = new GroupField();
            Nodes nds = new Nodes(this.No);
            foreach (Node nd in nds)
            {
                if (nd.IsStartNode)
                {
                    nd.NodeFrmID = "ND" + nd.NodeID;
                    nd.FrmWorkCheckSta = FrmWorkCheckSta.Disable;
                    // nd.HisFormType = NodeFormType.FoolForm;
                    nd.Update();
                    continue;
                }

                BP.WF.Template.FrmNodeComponent fnd = new FrmNodeComponent(nd.NodeID);

                //判断是否是最后一个节点.
                if (nd.IsEndNode == true || nd.HisToNodes.Count == 0)
                {
                    nd.FrmWorkCheckSta = FrmWorkCheckSta.Readonly; //最后一个是只读.
                    nd.NodeFrmID = "ND" + int.Parse(this.No) + "02";
                    nd.HisFormType = NodeFormType.RefNodeFrm; //引用到其他节点表单上.
                    nd.Update();

                    fnd.SetValByKey(NodeAttr.NodeFrmID, nd.NodeFrmID);
                    fnd.SetValByKey(NodeAttr.FWCSta, (int)nd.FrmWorkCheckSta);

                    fnd.Update();
                    if (gf.IsExit(GroupFieldAttr.FrmID, "ND" + nd.NodeID, GroupFieldAttr.CtrlType, GroupCtrlType.FWC) == false)
                    {
                        gf = new GroupField();
                        gf.FrmID = "ND" + nd.NodeID;
                        gf.CtrlType = GroupCtrlType.FWC;
                        gf.Lab = "审核组件";
                        gf.Idx = 0;
                        gf.Insert(); //插入.
                    }
                    continue;
                }

                nd.FrmWorkCheckSta = FrmWorkCheckSta.Enable;
                nd.NodeFrmID = "ND" + int.Parse(this.No) + "02";

                if (nd.NodeID.ToString().EndsWith("02") == true)
                    nd.HisFormType = NodeFormType.FoolForm;
                else
                    nd.HisFormType = NodeFormType.RefNodeFrm;
                nd.Update();

                if (gf.IsExit(GroupFieldAttr.FrmID, "ND" + nd.NodeID, GroupFieldAttr.CtrlType, GroupCtrlType.FWC) == false)
                {
                    gf = new GroupField();
                    gf.FrmID = "ND" + nd.NodeID;
                    gf.CtrlType = GroupCtrlType.FWC;
                    gf.Lab = "审核组件";
                    gf.Idx = 0;
                    gf.Insert(); //插入.
                }
                //fnd.HisFormType = NodeFormType.FoolForm;
                fnd.Update(); //不执行更新，会导致部分字段错误.
            }

            return "设置成功...";
        }
        #endregion
    }
    /// <summary>
    /// 流程集合
    /// </summary>
    public class FlowExts : EntitiesNoName
    {
        #region 查询
        /// <summary>
        /// 查询出来全部的在生存期间内的流程
        /// </summary>
        /// <param name="FlowSort">流程类别</param>
        /// <param name="IsCountInLifeCycle">是不是计算在生存期间内 true 查询出来全部的 </param>
        public void Retrieve(string FlowSort)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(BP.WF.Template.FlowAttr.FK_FlowSort, FlowSort);
            qo.addOrderBy(BP.WF.Template.FlowAttr.No);
            qo.DoQuery();
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 工作流程
        /// </summary>
        public FlowExts() { }
        /// <summary>
        /// 工作流程
        /// </summary>
        /// <param name="fk_sort"></param>
        public FlowExts(string fk_sort)
        {
            this.Retrieve(BP.WF.Template.FlowAttr.FK_FlowSort, fk_sort);
        }
        #endregion

        #region 得到实体
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FlowExt();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FlowExt> ToJavaList()
        {
            return (System.Collections.Generic.IList<FlowExt>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FlowExt> Tolist()
        {
            System.Collections.Generic.List<FlowExt> list = new System.Collections.Generic.List<FlowExt>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FlowExt)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

