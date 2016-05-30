﻿using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Port;
using BP.En;
using BP.Web;
using BP.Sys;
using BP.WF.Data;

namespace BP.WF.Template
{
    /// <summary>
    /// 流程
    /// </summary>
    public class FlowExt : EntityNoName
    {
        #region 属性.
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
                if (str == "")
                    return this.No;
                return str;
            }
            set
            {
                this.SetValByKey(FlowAttr.FlowMark, value);
            }
        }

        #region   前置导航
        /// <summary>
        /// 前置导航方式
        /// </summary>
        public StartGuideWay StartGuideWay
        {
            get
            {
                return (StartGuideWay)this.GetValIntByKey(FlowAttr.StartGuideWay);

            }
            set
            {
                this.SetValByKey(FlowAttr.StartGuideWay, (int)value);
            }

        }
        /// <summary>
        /// 前置导航参数1
        /// </summary>
        public string StartGuidePara1
        {

            get
            {
                return this.GetValStringByKey(FlowAttr.StartGuidePara1);
            }
            set
            {
                this.SetValByKey(FlowAttr.StartGuidePara1, value);
            }

        }
        /// <summary>
        /// 前置导航参数2
        /// </summary>
        public string StartGuidePara2
        {

            get
            {
                return this.GetValStringByKey(FlowAttr.StartGuidePara2);
            }
            set
            {
                this.SetValByKey(FlowAttr.StartGuidePara2, value);
            }

        }
        /// <summary>
        /// 前置导航参数3
        /// </summary>
        public string StartGuidePara3
        {

            get
            {
                return this.GetValStringByKey(FlowAttr.StartGuidePara3);
            }
            set
            {
                this.SetValByKey(FlowAttr.StartGuidePara3, value);
            }

        }

        /// <summary>
        /// 启动方式
        /// </summary>
        public FlowRunWay FlowRunWay
        {
            get
            {
                return (FlowRunWay)this.GetValIntByKey(FlowAttr.FlowRunWay);

            }
            set
            {
                this.SetValByKey(FlowAttr.FlowRunWay, (int)value);
            }

        }

        /// <summary>
        /// 运行内容
        /// </summary>
        public string RunObj
        {

            get
            {
                return this.GetValStringByKey(FlowAttr.RunObj);
            }
            set
            {
                this.SetValByKey(FlowAttr.RunObj, value);
            }

        }

        /// <summary>
        /// 是否启用开始节点数据重置按钮
        /// </summary>
        public bool IsResetData
        {

            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsResetData);
            }
            set
            {
                this.SetValByKey(FlowAttr.IsResetData, value);
            }
        }

        /// <summary>
        /// 是否自动装载上一笔数据
        /// </summary>
        public bool IsLoadPriData
        {
            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsLoadPriData);
            }
            set
            {
                this.SetValByKey(FlowAttr.IsLoadPriData, value);
            }
        }
        #endregion
        /// <summary>
        /// 设计者编号
        /// </summary>
        public string DesignerNo
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.DesignerNo);
            }
            set
            {
                this.SetValByKey(FlowAttr.DesignerNo, value);
            }
        }
        /// <summary>
        /// 设计者名称
        /// </summary>
        public string DesignerName
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.DesignerName);
            }
            set
            {
                this.SetValByKey(FlowAttr.DesignerName, value);
            }
        }
        /// <summary>
        /// 编号生成格式
        /// </summary>
        public string BillNoFormat
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.BillNoFormat);
            }
            set
            {
                this.SetValByKey(FlowAttr.BillNoFormat, value);
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
                if (BP.Web.WebUser.No == "admin" || this.DesignerNo == WebUser.No)
                {
                    uac.IsUpdate = true;
                }
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
            if (SystemConfig.IsDebug)
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

                Map map = new Map("WF_Flow", "流程");

                #region 基本属性。
                map.AddTBStringPK(FlowAttr.No, null, "编号", true, true, 1, 10, 3);
                map.SetHelperUrl(FlowAttr.No, "http://ccbpm.mydoc.io/?v=5404&t=17023"); //使用alert的方式显示帮助信息.

                map.AddDDLEntities(FlowAttr.FK_FlowSort, "01", "流程类别", new FlowSorts(), true);
                map.SetHelperUrl(FlowAttr.FK_FlowSort, "http://ccbpm.mydoc.io/?v=5404&t=17024");
                map.AddTBString(FlowAttr.Name, null, "名称", true, false, 0, 50, 10, true);

                // add 2013-02-14 唯一确定此流程的标记
                map.AddTBString(FlowAttr.FlowMark, null, "流程标记", true, false, 0, 150, 10);
                map.AddTBString(FlowAttr.FlowEventEntity, null, "流程事件实体", true, true, 0, 150, 10);
                map.SetHelperUrl(FlowAttr.FlowMark, "http://ccbpm.mydoc.io/?v=5404&t=16847");
                map.SetHelperUrl(FlowAttr.FlowEventEntity, "http://ccbpm.mydoc.io/?v=5404&t=17026");

                // add 2013-02-05.
                map.AddTBString(FlowAttr.TitleRole, null, "标题生成规则", true, false, 0, 150, 10, true);
                map.SetHelperUrl(FlowAttr.TitleRole, "http://ccbpm.mydoc.io/?v=5404&t=17040");

                //add  2013-08-30.
                map.AddTBString(FlowAttr.BillNoFormat, null, "单据编号格式", true, false, 0, 50, 10, false);
                map.SetHelperUrl(FlowAttr.BillNoFormat, "http://ccbpm.mydoc.io/?v=5404&t=17041");

                // add 2014-10-19.
                map.AddDDLSysEnum(FlowAttr.ChartType, (int)FlowChartType.Icon, "节点图形类型", true, true,
                    "ChartType", "@0=几何图形@1=肖像图片");

                map.AddBoolean(FlowAttr.IsCanStart, true, "可以独立启动否？(独立启动的流程可以显示在发起流程列表里)", true, true, true);
                map.SetHelperUrl(FlowAttr.IsCanStart, "http://ccbpm.mydoc.io/?v=5404&t=17027");
                map.AddBoolean(FlowAttr.IsMD5, false, "是否是数据加密流程(MD5数据加密防篡改)", true, true, true);
                map.SetHelperUrl(FlowAttr.IsMD5, "http://ccbpm.mydoc.io/?v=5404&t=17028");
                map.AddBoolean(FlowAttr.IsFullSA, false, "是否自动计算未来的处理人？", true, true, true);
                map.SetHelperUrl(FlowAttr.IsFullSA, "http://ccbpm.mydoc.io/?v=5404&t=17034");

                map.AddBoolean(FlowAttr.IsAutoSendSubFlowOver, false,
                    "(为子流程时)在流程结束时，是否检查所有子流程完成后，让父流程自动发送到下一步。", true, true, true);
                //map.SetHelperBaidu(FlowAttr.IsAutoSendSubFlowOver, "ccflow 是否检查所有子流程完成后父流程自动发送到下一步");
                map.AddBoolean(FlowAttr.IsGuestFlow, false, "是否外部用户参与流程(非组织结构人员参与的流程)", true, true, false);
                map.SetHelperUrl(FlowAttr.IsGuestFlow, "http://ccbpm.mydoc.io/?v=5404&t=17039");

                //批量发起 add 2013-12-27. 
                map.AddBoolean(FlowAttr.IsBatchStart, false, "是否可以批量发起流程？(如果是就要设置发起的需要填写的字段,多个用逗号分开)", true, true, true);
                map.AddTBString(FlowAttr.BatchStartFields, null, "发起字段s", true, false, 0, 500, 10, true);
                map.SetHelperUrl(FlowAttr.IsBatchStart, "http://ccbpm.mydoc.io/?v=5404&t=17047");
                map.AddDDLSysEnum(FlowAttr.FlowAppType, (int)FlowAppType.Normal, "流程应用类型",
                  true, true, "FlowAppType", "@0=业务流程@1=工程类(项目组流程)@2=公文流程(VSTO)");
                map.SetHelperUrl(FlowAttr.FlowAppType, "http://ccbpm.mydoc.io/?v=5404&t=17035");
                map.AddDDLSysEnum(FlowAttr.TimelineRole, (int)TimelineRole.ByNodeSet, "时效性规则",
                 true, true, FlowAttr.TimelineRole, "@0=按节点(由节点属性来定义)@1=按发起人(开始节点SysSDTOfFlow字段计算)");
                map.SetHelperUrl(FlowAttr.TimelineRole, "http://ccbpm.mydoc.io/?v=5404&t=17036");
                // 草稿
                map.AddDDLSysEnum(FlowAttr.Draft, (int)DraftRole.None, "草稿规则",
               true, true, FlowAttr.Draft, "@0=无(不设草稿)@1=保存到待办@2=保存到草稿箱");
                map.SetHelperUrl(FlowAttr.Draft, "http://ccbpm.mydoc.io/?v=5404&t=17037");

                // 数据存储.
                map.AddDDLSysEnum(FlowAttr.DataStoreModel, (int)DataStoreModel.ByCCFlow,
                    "流程数据存储模式", true, true, FlowAttr.DataStoreModel,
                   "@0=数据轨迹模式@1=数据合并模式");
                map.SetHelperUrl(FlowAttr.DataStoreModel, "http://ccbpm.mydoc.io/?v=5404&t=17038");

                //add 2013-05-22.
                map.AddTBString(FlowAttr.HistoryFields, null, "历史查看字段", true, false, 0, 500, 10, true);
                //map.SetHelperBaidu(FlowAttr.HistoryFields, "ccflow 历史查看字段");
                map.AddTBString(FlowAttr.FlowNoteExp, null, "备注的表达式", true, false, 0, 500, 10, true);
                map.SetHelperUrl(FlowAttr.FlowNoteExp, "http://ccbpm.mydoc.io/?v=5404&t=17043");
                map.AddTBString(FlowAttr.Note, null, "流程描述", true, false, 0, 100, 10, true);

                map.AddDDLSysEnum(FlowAttr.FlowAppType, (int)FlowAppType.Normal, "流程应用类型", true, true, "FlowAppType", "@0=业务流程@1=工程类(项目组流程)@2=公文流程(VSTO)");

                map.AddTBString(FlowAttr.HelpUrl, null, "帮助文档", true, false, 0, 300, 10, true);

                //移动到这里 by zhoupeng 2016.04.08.
                map.AddBoolean(FlowAttr.IsResetData, false, "是否启用开始节点数据重置按钮？", true, true, true);
                map.AddBoolean(FlowAttr.IsLoadPriData, false, "是否自动装载上一笔数据？", true, true, true);
                #endregion 基本属性。

                #region 启动方式
                //map.AddDDLSysEnum(FlowAttr.FlowRunWay, (int)FlowRunWay.HandWork, "启动方式",
                //    true, true, FlowAttr.FlowRunWay, "@0=手工启动@1=指定人员定时启动@2=定时访问数据集自动启动@3=触发式启动");

                //map.SetHelperUrl(FlowAttr.FlowRunWay, "http://ccbpm.mydoc.io/?v=5404&t=17088");
                //// map.AddTBString(FlowAttr.RunObj, null, "运行内容", true, false, 0, 100, 10, true);
                //map.AddTBStringDoc(FlowAttr.RunObj, null, "运行内容", true, false, true);
                #endregion 启动方式

                #region 流程启动限制
                //string role = "@0=不限制";
                //role += "@1=每人每天一次";
                //role += "@2=每人每周一次";
                //role += "@3=每人每月一次";
                //role += "@4=每人每季一次";
                //role += "@5=每人每年一次";
                //role += "@6=发起的列不能重复,(多个列可以用逗号分开)";
                //role += "@7=设置的SQL数据源为空,或者返回结果为零时可以启动.";
                //role += "@8=设置的SQL数据源为空,或者返回结果为零时不可以启动.";
                //map.AddDDLSysEnum(FlowAttr.StartLimitRole, (int)StartLimitRole.None, "启动限制规则", true, true, FlowAttr.StartLimitRole, role, true);
                //map.AddTBString(FlowAttr.StartLimitPara, null, "规则参数", true, false, 0, 500, 10, true);
                //map.AddTBStringDoc(FlowAttr.StartLimitAlert, null, "限制提示", true, false, true);
                //map.SetHelperUrl(FlowAttr.StartLimitRole, "http://ccbpm.mydoc.io/?v=5404&t=17872");

                //   map.AddTBString(FlowAttr.StartLimitAlert, null, "限制提示", true, false, 0, 500, 10, true);
                //    map.AddDDLSysEnum(FlowAttr.StartLimitWhen, (int)StartLimitWhen.StartFlow, "提示时间", true, true, FlowAttr.StartLimitWhen, "@0=启动流程时@1=发送前提示", false);
                #endregion 流程启动限制

                #region 发起前导航。
                //map.AddDDLSysEnum(FlowAttr.DataStoreModel, (int)DataStoreModel.ByCCFlow,
                //    "流程数据存储模式", true, true, FlowAttr.DataStoreModel,
                //   "@0=数据轨迹模式@1=数据合并模式");

                //发起前设置规则.
                //map.AddDDLSysEnum(FlowAttr.StartGuideWay, 0, "前置导航方式", true, true,
                //    FlowAttr.StartGuideWay,
                //    "@0=无@1=按系统的URL-(父子流程)单条模式@2=按系统的URL-(子父流程)多条模式@3=按系统的URL-(实体记录,未完成)单条模式@4=按系统的URL-(实体记录,未完成)多条模式@5=从开始节点Copy数据@10=按自定义的Url@11=按用户输入参数", true);
                //map.SetHelperUrl(FlowAttr.StartGuideWay, "http://ccbpm.mydoc.io/?v=5404&t=17883");

                //map.AddTBStringDoc(FlowAttr.StartGuidePara1, null, "参数1", true, false, true);
                //map.AddTBStringDoc(FlowAttr.StartGuidePara2, null, "参数2", true, false, true);
                //map.AddTBStringDoc(FlowAttr.StartGuidePara3, null, "参数3", true, false, true);

                //map.AddBoolean(FlowAttr.IsResetData, false, "是否启用开始节点数据重置按钮？", true, true, true);
                ////     map.AddBoolean(FlowAttr.IsImpHistory, false, "是否启用导入历史数据按钮？", true, true, true);
                //map.AddBoolean(FlowAttr.IsLoadPriData, false, "是否自动装载上一笔数据？", true, true, true);

                #endregion 发起前导航。

                #region 延续流程。
                ////延续流程.
                //map.AddDDLSysEnum(FlowAttr.CFlowWay, (int)CFlowWay.None, "延续流程", true, true,
                //    FlowAttr.CFlowWay, "@0=无:非延续类流程@1=按照参数@2=按照字段配置");
                //map.AddTBStringDoc(FlowAttr.CFlowPara, null, "延续流程参数", true, false, true);
                //map.SetHelperUrl(FlowAttr.CFlowWay, "http://ccbpm.mydoc.io/?v=5404&t=17891");

                //// add 2013-03-24.
                //map.AddTBString(FlowAttr.DesignerNo, null, "设计者编号", false, false, 0, 32, 10);
                //map.AddTBString(FlowAttr.DesignerName, null, "设计者名称", false, false, 0, 100, 10);
                #endregion 延续流程。

                #region 数据同步方案
                ////数据同步方式.
                //map.AddDDLSysEnum(FlowAttr.DTSWay, (int)FlowDTSWay.None, "同步方式", true, true,
                //    FlowAttr.DTSWay, "@0=不同步@1=同步");
                //map.SetHelperUrl(FlowAttr.DTSWay, "http://ccbpm.mydoc.io/?v=5404&t=17893");

                //map.AddDDLEntities(FlowAttr.DTSDBSrc, "", "数据库", new BP.Sys.SFDBSrcs(), true);

                //map.AddTBString(FlowAttr.DTSBTable, null, "业务表名", true, false, 0, 50, 50, false);

                //map.AddTBString(FlowAttr.DTSBTablePK, null, "业务表主键", true, false, 0, 50, 50, false);
                //map.SetHelperAlert(FlowAttr.DTSBTablePK, "如果同步方式设置了按照业务表主键字段计算,那么需要在流程的节点表单里设置一个同名同类型的字段，系统将会按照这个主键进行数据同步。");

                //map.AddTBString(FlowAttr.DTSFields, null, "要同步的字段s,中间用逗号分开.", false, false, 0, 200, 100, false);

                //map.AddDDLSysEnum(FlowAttr.DTSTime, (int)FlowDTSTime.AllNodeSend, "执行同步时间点", true, true,
                //   FlowAttr.DTSTime, "@0=所有的节点发送后@1=指定的节点发送后@2=当流程结束时");
                //map.SetHelperUrl(FlowAttr.DTSTime, "http://ccbpm.mydoc.io/?v=5404&t=17894");

                //map.AddTBString(FlowAttr.DTSSpecNodes, null, "指定的节点ID", true, false, 0, 50, 50, false);
                //map.SetHelperAlert(FlowAttr.DTSSpecNodes, "如果执行同步时间点选择了按指定的节点发送后,多个节点用逗号分开.比如: 101,102,103");


                //map.AddDDLSysEnum(FlowAttr.DTSField, (int)DTSField.SameNames, "要同步的字段计算方式", true, true,
                // FlowAttr.DTSField, "@0=字段名相同@1=按设置的字段匹配");
                //map.SetHelperUrl(FlowAttr.DTSField, "http://ccbpm.mydoc.io/?v=5404&t=17895");

                //map.AddTBString(FlowAttr.PTable, null, "流程数据存储表", true, false, 0, 30, 10);
                //map.SetHelperUrl(FlowAttr.PTable, "http://ccbpm.mydoc.io/?v=5404&t=17897");

                #endregion 数据同步方案

                #region 权限控制.
                //map.AddBoolean(FlowAttr.PStarter, true, "发起人可看(必选)", true, false,true);
                //map.AddBoolean(FlowAttr.PWorker, true, "参与人可看(必选)", true, false, true);
                //map.AddBoolean(FlowAttr.PCCer, true, "被抄送人可看(必选)", true, false, true);

                //map.AddBoolean(FlowAttr.PMyDept, true, "本部门人可看", true, true, true);
                //map.AddBoolean(FlowAttr.PPMyDept, true, "直属上级部门可看(比如:我是)", true, true, true);

                //map.AddBoolean(FlowAttr.PPDept, true, "上级部门可看", true, true, true);
                //map.AddBoolean(FlowAttr.PSameDept, true, "平级部门可看", true, true, true);

                //map.AddBoolean(FlowAttr.PSpecDept, true, "指定部门可看", true, true, false);
                //map.AddTBString(FlowAttr.PSpecDept + "Ext", null, "部门编号", true, false, 0, 200, 100, false);


                //map.AddBoolean(FlowAttr.PSpecSta, true, "指定的岗位可看", true, true, false);
                //map.AddTBString(FlowAttr.PSpecSta + "Ext", null, "岗位编号", true, false, 0, 200, 100, false);

                //map.AddBoolean(FlowAttr.PSpecGroup, true, "指定的权限组可看", true, true, false);
                //map.AddTBString(FlowAttr.PSpecGroup + "Ext", null, "权限组", true, false, 0, 200, 100, false);

                //map.AddBoolean(FlowAttr.PSpecEmp, true, "指定的人员可看", true, true, false);
                //map.AddTBString(FlowAttr.PSpecEmp + "Ext", null, "指定的人员编号", true, false, 0, 200, 100, false);
                #endregion 权限控制.

                //查询条件.
                map.AddSearchAttr(FlowAttr.FK_FlowSort);
                map.AddSearchAttr(FlowAttr.TimelineRole);

                #region 基本功能.
                //map.AddRefMethod(rm);
                RefMethod rm = new RefMethod();
                rm = new RefMethod();
                rm.Title = "调试运行"; // "设计检查报告";
                //rm.ToolTip = "检查流程设计的问题。";
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/EntityFunc/Flow/Run.png";
                rm.ClassMethodName = this.ToString() + ".DoRunIt";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "检查报告"; // "设计检查报告";
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/EntityFunc/Flow/CheckRpt.png";
                rm.ClassMethodName = this.ToString() + ".DoCheck";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设计报表"; // "报表运行";
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/Btn/Rpt.gif";
                rm.ClassMethodName = this.ToString() + ".DoOpenRpt()";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "自动发起";
                rm.Icon = Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/AutoStart.png";
                rm.ClassMethodName = this.ToString() + ".DoSetStartFlowDataSources()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "发起限制规则";
                rm.Icon = Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/Limit.png";
                rm.ClassMethodName = this.ToString() + ".DoLimit()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "发起前置导航";
                rm.Icon = Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/StartGuide.png";
                rm.ClassMethodName = this.ToString() + ".DoStartGuide()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "批量修改节点属性";
                //rm.Icon = Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                //rm.ClassMethodName = this.ToString() + ".DoFeatureSetUI()";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "流程事件&消息"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoAction";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Event.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "与业务表数据同步"; // "抄送规则";
                //rm.ClassMethodName = this.ToString() + ".DoBTable";
                //rm.Icon = Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/DTS.png";
                //rm.RefAttrLinkLabel = "业务表字段同步配置";
                //rm.RefMethodType = RefMethodType.LinkeWinOpen;
                //rm.Target = "_blank";
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "独立表单树";
                //rm.Icon = Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                //rm.ClassMethodName = this.ToString() + ".DoFlowFormTree()";
                //map.AddRefMethod(rm);





                #endregion 流程设置.

                #region 实验中的功能

                rm = new RefMethod();
                rm.Title = "流程轨迹表单";
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoBindFlowExt()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "实验中的功能";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "批量设置节点";
                rm.Icon = Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/Node.png";
                rm.ClassMethodName = this.ToString() + ".DoNodeAttrs()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "实验中的功能";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "轨迹查看权限";
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/Setting.png";
                rm.ClassMethodName = this.ToString() + ".DoTruckRight()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "实验中的功能";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "数据源管理(如果新增数据源后需要关闭重新打开)";
                rm.ClassMethodName = this.ToString() + ".DoDBSrc";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                //设置相关字段.
                rm.RefAttrKey = FlowAttr.DTSDBSrc;
                rm.RefAttrLinkLabel = "数据源管理";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                rm.GroupName = "实验中的功能";
                map.AddRefMethod(rm);
                #endregion 实验中的功能

                #region 流程模版管理.
                rm = new RefMethod();
                rm.Title = "模版导入";
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/redo.png";
                rm.ClassMethodName = this.ToString() + ".DoImp()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "流程模版";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "模版导出";
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/undo.png";
                rm.ClassMethodName = this.ToString() + ".DoExps()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "流程模版";
                map.AddRefMethod(rm);
                #endregion 流程模版管理.

                #region 开发接口.


                rm = new RefMethod();
                rm.Title = "与业务表数据同步";
                rm.Icon = Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/DTS.png";

                rm.ClassMethodName = this.ToString() + ".DoDTSBTable()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "开发接口";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "URL调用接口";
                rm.Icon = Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/URL.png";
                rm.ClassMethodName = this.ToString() + ".DoAPI()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "开发接口";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "SKD开发接口";
                rm.Icon = Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/API.png";
                rm.ClassMethodName = this.ToString() + ".DoAPICode()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "开发接口";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "代码事件开发接口";
                rm.Icon = Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/API.png";
                rm.ClassMethodName = this.ToString() + ".DoAPICodeFEE()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "开发接口";
                map.AddRefMethod(rm);
                #endregion 开发接口.

                #region 报表设计.
                rm = new RefMethod();
                rm.Title = "报表设计";
                rm.Icon = Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/DesignRpt.png";
                rm.ClassMethodName = this.ToString() + ".DoDRpt()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "报表设计";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "流程查询";
                rm.Icon = Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/Search.png";
                rm.ClassMethodName = this.ToString() + ".DoDRptSearch()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "报表设计";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "自定义查询";
                rm.Icon = Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/SQL.png";
                rm.ClassMethodName = this.ToString() + ".DoDRptSearchAdv()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "报表设计";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "分组分析";
                rm.Icon = Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/Group.png";
                rm.ClassMethodName = this.ToString() + ".DoDRptGroup()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "报表设计";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "交叉报表";
                rm.Icon = Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/D3.png";
                rm.ClassMethodName = this.ToString() + ".DoDRptD3()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "报表设计";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "对比分析";
                rm.Icon = Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/Contrast.png";
                rm.ClassMethodName = this.ToString() + ".DoDRptContrast()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "报表设计";
                map.AddRefMethod(rm);
                #endregion 报表设计.

                #region 流程运行维护.
                rm = new RefMethod();
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.Title = "重生成报表数据"; // "删除数据";
                rm.Warning = "您确定要执行吗? 注意:此方法耗费资源。";// "您确定要执行删除流程数据吗？";
                rm.ClassMethodName = this.ToString() + ".DoReloadRptData";
                rm.GroupName = "流程维护";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "重生成流程标题";
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoGenerTitle()";
                //设置相关字段.
                //rm.RefAttrKey = FlowAttr.TitleRole;
                rm.RefAttrLinkLabel = "重新生成流程标题";
                rm.RefMethodType = RefMethodType.Func;
                rm.Target = "_blank";
                rm.Warning = "您确定要根据新的规则重新产生标题吗？";
                rm.GroupName = "流程维护";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "重生成FlowEmps字段";
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoGenerFlowEmps()";
                rm.RefAttrLinkLabel = "补充修复emps字段，包括wf_generworkflow,NDxxxRpt字段.";
                rm.RefMethodType = RefMethodType.Func;
                rm.Target = "_blank";
                rm.Warning = "您确定要重新生成吗？";
                rm.GroupName = "流程维护";
                map.AddRefMethod(rm);

                //带有参数的方法.
                rm = new RefMethod();
                rm.GroupName = "流程维护";
                rm.Title = "重命名节点表单字段";
                //  rm.Warning = "您确定要处理吗？";
                rm.HisAttrs.AddTBString("FieldOld", null, "旧字段英文名", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("FieldNew", null, "新字段英文名", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("FieldNewName", null, "新字段中文名", true, false, 0, 100, 100);
                rm.HisAttrs.AddBoolen("thisFlowOnly", true, "仅仅当前流程");

                rm.ClassMethodName = this.ToString() + ".DoChangeFieldName";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "节点表单字段视图";
                rm.Icon = Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/Field.png";
                rm.ClassMethodName = this.ToString() + ".DoFlowFields()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "流程维护";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/Btn/Delete.gif";
                rm.Title = "删除全部流程数据"; // this.ToE("DelFlowData", "删除数据"); // "删除数据";
                rm.Warning = "您确定要执行删除流程数据吗? \t\n该流程的数据删除后，就不能恢复，请注意删除的内容。";// "您确定要执行删除流程数据吗？";
                rm.ClassMethodName = this.ToString() + ".DoDelData";
                rm.GroupName = "流程维护";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/Btn/Delete.gif";
                rm.Title = "按工作ID删除"; // this.ToE("DelFlowData", "删除数据"); // "删除数据";
                rm.GroupName = "流程维护";
                rm.ClassMethodName = this.ToString() + ".DoDelDataOne";
                rm.HisAttrs.AddTBInt("WorkID", 0, "输入工作ID", true, false);
                rm.HisAttrs.AddTBString("beizhu", null, "删除备注", true, false, 0, 100, 100);
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "回滚流程";
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/Btn/Back.png";
                rm.ClassMethodName = this.ToString() + ".DoRebackFlowData()";
                // rm.Warning = "您确定要回滚它吗？";
                rm.HisAttrs.AddTBInt("workid", 0, "请输入要会滚WorkID", true, false);
                rm.HisAttrs.AddTBInt("nodeid", 0, "回滚到的节点ID", true, false);
                rm.HisAttrs.AddTBString("note", null, "回滚原因", true, false, 0, 600, 200);
                rm.GroupName = "流程维护";
                map.AddRefMethod(rm);
                #endregion 流程运行维护.

                #region 流程监控.

                rm = new RefMethod();
                rm.Title = "监控面板";
                rm.Icon = Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/Monitor.png";
                rm.ClassMethodName = this.ToString() + ".DoDataManger_Welcome()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "流程监控";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "流程查询";
                rm.Icon = Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/Search.png";
                rm.ClassMethodName = this.ToString() + ".DoDataManger()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "流程监控";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "节点列表";
                rm.Icon = Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/flows.png";
                rm.ClassMethodName = this.ToString() + ".DoDataManger_Nodes()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "流程监控";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "综合查询";
                rm.Icon = Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/Search.png";
                rm.ClassMethodName = this.ToString() + ".DoDataManger_Search()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "流程监控";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "综合分析";
                rm.Icon = Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/Group.png";
                rm.ClassMethodName = this.ToString() + ".DoDataManger_Group()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "流程监控";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "实例增长分析";
                rm.Icon = Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/Grow.png";
                rm.ClassMethodName = this.ToString() + ".DoDataManger_InstanceGrowOneFlow()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "流程监控";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "逾期未完成实例";
                rm.Icon = Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/Warning.png";
                rm.ClassMethodName = this.ToString() + ".DoDataManger_InstanceWarning()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "流程监控";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "逾期已完成实例";
                rm.Icon = Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/overtime.png";
                rm.ClassMethodName = this.ToString() + ".DoDataManger_InstanceOverTimeOneFlow()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "流程监控";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "删除日志";
                rm.Icon = Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/log.png";
                rm.ClassMethodName = this.ToString() + ".DoDataManger_DeleteLog()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "流程监控";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "数据订阅-实验中";
                rm.Icon = Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/RptOrder.png";
                rm.ClassMethodName = this.ToString() + ".DoDataManger_RptOrder()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "流程监控";
                map.AddRefMethod(rm);
                #endregion 流程监控.


                //rm = new RefMethod();
                //rm.Title = "执行流程数据表与业务表数据手工同步"; 
                //rm.ClassMethodName = this.ToString() + ".DoBTableDTS";
                //rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                //rm.Warning = "您确定要执行吗？如果执行了可能会对业务表数据造成错误。";
                ////设置相关字段.
                //rm.RefAttrKey = FlowAttr.DTSSpecNodes;
                //rm.RefAttrLinkLabel = "业务表字段同步配置";
                //rm.RefMethodType = RefMethodType.Func;
                //rm.Target = "_blank";
                ////  map.AddRefMethod(rm);


                //rm = new RefMethod();
                //rm.Title = "设置自动发起"; // "报表运行";
                //rm.Icon = "/WF/Img/Btn/View.gif";
                //rm.ClassMethodName = this.ToString() + ".DoOpenRpt()";
                ////rm.Icon = "/WF/Img/Btn/Table.gif"; 
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = this.ToE("Event", "事件"); // "报表运行";
                //rm.Icon = "/WF/Img/Btn/View.gif";
                //rm.ClassMethodName = this.ToString() + ".DoOpenRpt()";
                ////rm.Icon = "/WF/Img/Btn/Table.gif";
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = this.ToE("FlowExtDataOut", "数据转出定义");  //"数据转出定义";
                ////  rm.Icon = "/WF/Img/Btn/Table.gif";
                //rm.ToolTip = "在流程完成时间，流程数据转储存到其它系统中应用。";
                //rm.ClassMethodName = this.ToString() + ".DoExp";
                //map.AddRefMethod(rm);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 流程监控.
        public string DoDataManger_Welcome()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/CCBPMDesigner/App/OneFlow/Welcome.aspx?FK_Flow=" + this.No;
        }
        public string DoDataManger_Nodes()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/CCBPMDesigner/App/OneFlow/Nodes.aspx?FK_Flow=" + this.No;
        }
        public string DoDataManger_Search()
        {
            return SystemConfig.CCFlowWebPath + "WF/Comm/Search.aspx?EnsName=BP.WF.Data.GenerWorkFlowViews&FK_Flow=" + this.No + "&WFSta=all";
        }
        public string DoDataManger_Group()
        {
            return SystemConfig.CCFlowWebPath + "WF/Comm/Group.aspx?EnsName=BP.WF.Data.GenerWorkFlowViews&FK_Flow=" + this.No + "&WFSta=all";
        }

        public string DoDataManger_InstanceGrowOneFlow()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/FlowDB/InstanceGrowOneFlow.aspx?anaTime=mouth&FK_Flow=" + this.No;
        }

        public string DoDataManger_InstanceWarning()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/FlowDB/InstanceWarning.aspx?anaTime=mouth&FK_Flow=" + this.No;
        }

        public string DoDataManger_InstanceOverTimeOneFlow()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/FlowDB/InstanceOverTimeOneFlow.aspx?anaTime=mouth&FK_Flow=" + this.No;
        }
        public string DoDataManger_DeleteLog()
        {
            return SystemConfig.CCFlowWebPath + "WF/Comm/Search.aspx?EnsName=BP.WF.WorkFlowDeleteLogs&FK_Flow=" + this.No + "&WFSta=all";
        }

        /// <summary>
        /// 数据订阅
        /// </summary>
        /// <returns></returns>
        public string DoDataManger_RptOrder()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/CCBPMDesigner/App/RptOrder.aspx?anaTime=mouth&FK_Flow=" + this.No;
        }
        #endregion 流程监控.

        #region 报表设计.
        public string DoDRpt()
        {
            return SystemConfig.CCFlowWebPath + "WF/Rpt/OneFlow.aspx?FK_Flow=" + this.No + "&FK_MapData=ND" + int.Parse(this.No) + "MyRpt";
        }
        public string DoDRptSearch()
        {
            return SystemConfig.CCFlowWebPath + "WF/Rpt/Search.aspx?FK_Flow=" + this.No + "&RptNo=ND" + int.Parse(this.No) + "MyRpt";
        }
        public string DoDRptSearchAdv()
        {
            return SystemConfig.CCFlowWebPath + "WF/Rpt/SearchAdv.aspx?FK_Flow=" + this.No + "&RptNo=ND" + int.Parse(this.No) + "MyRpt";
        }
        public string DoDRptGroup()
        {
            return SystemConfig.CCFlowWebPath + "WF/Rpt/Group.aspx?FK_Flow=" + this.No + "&RptNo=ND" + int.Parse(this.No) + "MyRpt";
        }
        public string DoDRptD3()
        {
            return SystemConfig.CCFlowWebPath + "WF/Rpt/D3.aspx?FK_Flow=" + this.No + "&RptNo=ND" + int.Parse(this.No) + "MyRpt";
        }
        public string DoDRptContrast()
        {
            return SystemConfig.CCFlowWebPath + "WF/Rpt/Contrast.aspx?FK_Flow=" + this.No + "&RptNo=ND" + int.Parse(this.No) + "MyRpt";
        }
        #endregion 报表设计.

        #region 开发接口.
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
            return Glo.CCFlowAppPath + "WF/Admin/AttrFlow/FlowFields.aspx?FK_Flow=" + this.No;
        }
        /// <summary>
        /// 与业务表数据同步
        /// </summary>
        /// <returns></returns>
        public string DoDTSBTable()
        {
            return Glo.CCFlowAppPath + "WF/Admin/AttrFlow/DTSBTable.aspx?FK_Flow=" + this.No;
        }
        public string DoAPI()
        {
            return Glo.CCFlowAppPath + "WF/Admin/AttrFlow/API.aspx?FK_Flow=" + this.No;
        }
        public string DoAPICode()
        {
            return Glo.CCFlowAppPath + "WF/Admin/AttrFlow/APICode.aspx?FK_Flow=" + this.No;
        }
        public string DoAPICodeFEE()
        {
            return Glo.CCFlowAppPath + "WF/Admin/AttrFlow/APICodeFEE.aspx?FK_Flow=" + this.No;
        }
        #endregion 开发接口

        #region  基本功能
        /// <summary>
        /// 事件
        /// </summary>
        /// <returns></returns>
        public string DoAction()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/Action.aspx?NodeID=0&FK_Flow=" + this.No + "&tk=" + new Random().NextDouble();
        }
        public string DoDBSrc()
        {
            return SystemConfig.CCFlowWebPath + "WF/Comm/Search.aspx?EnsName=BP.Sys.SFDBSrcs";
        }
        public string DoBTable()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/AttrFlow/DTSBTable.aspx?s=d34&ShowType=FlowFrms&FK_Node=" + int.Parse(this.No) + "01&FK_Flow=" + this.No + "&ExtType=StartFlow&RefNo=" + DataType.CurrentDataTime;
        }
        /// <summary>
        /// 批量修改节点属性.
        /// </summary>
        /// <returns></returns>
        public string DoFeatureSetUI()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/FeatureSetUI.aspx?s=d34&ShowType=FlowFrms&FK_Node=" + int.Parse(this.No) + "01&FK_Flow=" + this.No + "&ExtType=StartFlow&RefNo=" + DataType.CurrentDataTime;
        }
        /// <summary>
        /// 批量修改节点属性
        /// </summary>
        /// <returns></returns>
        public string DoNodeAttrs()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/AttrFlow/NodeAttrs.aspx?NodeID=0&FK_Flow=" + this.No + "&tk=" + new Random().NextDouble();
        }
        public string DoBindFlowExt()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/BindFrms.aspx?s=d34&ShowType=FlowFrms&FK_Node=0&FK_Flow=" + this.No + "&ExtType=StartFlow";
        }
        /// <summary>
        /// 轨迹查看权限
        /// </summary>
        /// <returns></returns>
        public string DoTruckRight()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/AttrFlow/TruckViewPower.aspx?FK_Flow=" + this.No;
        }
        /// <summary>
        /// 批量发起字段
        /// </summary>
        /// <returns></returns>
        public string DoBatchStartFields()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/AttrFlow/BatchStartFields.aspx?s=d34&FK_Flow=" + this.No + "&ExtType=StartFlow";
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
            GERpt rpt = new GERpt("ND" + int.Parse(this.No) + "Rpt");
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
                gwf.Title = rpt.Title;
                gwf.WFState = WFState.ReturnSta; /*设置为退回的状态*/
                gwf.FK_Dept = rpt.FK_Dept;

                Dept dept = new Dept(empStarter.FK_Dept);

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
                string actionType = (int)ActionType.Forward + "," + (int)ActionType.FlowOver + "," + (int)ActionType.ForwardFL + "," + (int)ActionType.ForwardHL;
                string sql = "SELECT  * FROM " + ndTrack + " WHERE   ActionType IN (" + actionType + ")  and WorkID=" + workid + " ORDER BY RDT DESC, NDFrom ";
                System.Data.DataTable dt = DBAccess.RunSQLReturnTable(sql);
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

                    gwl.FK_Emp = EmpFrom;
                    gwl.FK_EmpText = EmpFromT;
                    if (gwl.IsExits)
                        continue; /*有可能是反复退回的情况.*/

                    Emp emp = new Emp(gwl.FK_Emp);
                    gwl.FK_Dept = emp.FK_Dept;

                    gwl.RDT = dr["RDT"].ToString();
                    gwl.SDT = dr["RDT"].ToString();
                    gwl.DTOfWarning = gwf.SDTOfNode;
                    gwl.WarningHour = nd.WarningHour;
                    gwl.IsEnable = true;
                    gwl.WhoExeIt = nd.WhoExeIt;
                    gwl.Insert();
                }

                #region 加入退回信息, 让接受人能够看到退回原因.
                ReturnWork rw = new ReturnWork();
                rw.WorkID = workid;
                rw.ReturnNode = backToNodeID;
                rw.ReturnNodeName = endN.Name;
                rw.Returner = WebUser.No;
                rw.ReturnerName = WebUser.Name;

                rw.ReturnToNode = currWl.FK_Node;
                rw.ReturnToEmp = currWl.FK_Emp;
                rw.Note = note;
                rw.RDT = DataType.CurrentDataTime;
                rw.IsBackTracking = false;
                rw.MyPK = BP.DA.DBAccess.GenerGUID();
                #endregion   加入退回信息, 让接受人能够看到退回原因.

                //更新流程表的状态.
                rpt.FlowEnder = currWl.FK_Emp;
                rpt.WFState = WFState.ReturnSta; /*设置为退回的状态*/
                rpt.FlowEndNode = currWl.FK_Node;
                rpt.Update();

                // 向接受人发送一条消息.
                BP.WF.Dev2Interface.Port_SendMsg(currWl.FK_Emp, "工作恢复:" + gwf.Title, "工作被:" + WebUser.No + " 恢复." + note, "ReBack" + workid, BP.WF.SMSMsgType.ToDo, this.No, int.Parse(this.No + "01"), workid, 0);

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
            if (WebUser.No != "admin")
                return "非admin用户不能执行。";

            Flow fl = new Flow(this.No);

            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.Retrieve(GenerWorkFlowAttr.FK_Flow, this.No);

            foreach (GenerWorkFlow gwf in gwfs)
            {
                string emps = "";
                string sql = "SELECT EmpFrom FROM ND" + int.Parse(this.No) + "Track  WHERE WorkID=" + gwf.WorkID;

                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    if (emps.Contains("," + dr[0].ToString() + ","))
                        continue;
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
                if (wk.Rec != WebUser.No)
                {
                    BP.Web.WebUser.Exit();
                    try
                    {
                        Emp emp = new Emp(wk.Rec);
                        BP.Web.WebUser.SignInOfGener(emp);
                    }
                    catch
                    {
                        continue;
                    }
                }
                string sql = "";
                string title = WorkNode.GenerTitle(fl, wk);
                Paras ps = new Paras();
                ps.Add("Title", title);
                ps.Add("OID", wk.OID);
                ps.SQL = "UPDATE " + table + " SET Title=" + SystemConfig.AppCenterDBVarStr + "Title WHERE OID=" + SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

                ps.SQL = "UPDATE " + md.PTable + " SET Title=" + SystemConfig.AppCenterDBVarStr + "Title WHERE OID=" + SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

                ps.SQL = "UPDATE WF_GenerWorkFlow SET Title=" + SystemConfig.AppCenterDBVarStr + "Title WHERE WorkID=" + SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

                ps.SQL = "UPDATE WF_GenerFH SET Title=" + SystemConfig.AppCenterDBVarStr + "Title WHERE FID=" + SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQLs(sql);
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
            if (WebUser.No != "admin")
                return "非admin用户不能执行。";
            Flow fl = new Flow(this.No);
            Node nd = fl.HisStartNode;
            Works wks = nd.HisWorks;
            wks.RetrieveAllFromDBSource(WorkAttr.Rec);
            string table = nd.HisWork.EnMap.PhysicsTable;
            string tableRpt = "ND" + int.Parse(this.No) + "Rpt";
            Sys.MapData md = new Sys.MapData(tableRpt);
            foreach (Work wk in wks)
            {

                if (wk.Rec != WebUser.No)
                {
                    BP.Web.WebUser.Exit();
                    try
                    {
                        Emp emp = new Emp(wk.Rec);
                        BP.Web.WebUser.SignInOfGener(emp);
                    }
                    catch
                    {
                        continue;
                    }
                }
                string sql = "";
                string title = WorkNode.GenerTitle(fl, wk);
                Paras ps = new Paras();
                ps.Add("Title", title);
                ps.Add("OID", wk.OID);
                ps.SQL = "UPDATE " + table + " SET Title=" + SystemConfig.AppCenterDBVarStr + "Title WHERE OID=" + SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

                ps.SQL = "UPDATE " + md.PTable + " SET Title=" + SystemConfig.AppCenterDBVarStr + "Title WHERE OID=" + SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

                ps.SQL = "UPDATE WF_GenerWorkFlow SET Title=" + SystemConfig.AppCenterDBVarStr + "Title WHERE WorkID=" + SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

                ps.SQL = "UPDATE WF_GenerFH SET Title=" + SystemConfig.AppCenterDBVarStr + "Title WHERE FID=" + SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQLs(sql);
            }
            Emp emp1 = new Emp("admin");
            BP.Web.WebUser.SignInOfGener(emp1);

            return "全部生成成功,影响数据(" + wks.Count + ")条";
        }
        /// <summary>
        /// 流程监控
        /// </summary>
        /// <returns></returns>
        public string DoDataManger()
        {
            return Glo.CCFlowAppPath + "WF/Comm/Search.aspx?s=d34&EnsName=BP.WF.Data.GenerWorkFlowViews&FK_Flow=" + this.No + "&ExtType=StartFlow&RefNo=";
        }
        /// <summary>
        /// 绑定独立表单
        /// </summary>
        /// <returns></returns>
        public string DoFlowFormTree()
        {
            PubClass.WinOpen(Glo.CCFlowAppPath + "WF/Admin/FlowFormTree.aspx?s=d34&FK_Flow=" + this.No + "&ExtType=StartFlow&RefNo=" + DataType.CurrentDataTime, 700, 500);
            return null;
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
        /// 删除流程
        /// </summary>
        /// <param name="workid"></param>
        /// <param name="sd"></param>
        /// <returns></returns>
        public string DoDelDataOne(int workid, string note)
        {
            try
            {
                BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(this.No, workid, true);
                return "删除成功 workid=" + workid + "  理由:" + note;
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
            string flowID = int.Parse(this.No).ToString() + "01";
            return Glo.CCFlowAppPath + "WF/Admin/AttrFlow/AutoStart.aspx?s=d34&FK_Flow=" + this.No + "&ExtType=StartFlow&RefNo=";
            //return Glo.CCFlowAppPath + "WF/MapDef/MapExt.aspx?s=d34&FK_MapData=ND" + flowID + "&ExtType=StartFlow&RefNo=";
        }
        public string DoCCNode()
        {
            PubClass.WinOpen(Glo.CCFlowAppPath + "WF/Admin/CCNode.aspx?FK_Flow=" + this.No, 400, 500);
            return null;
        }
        /// <summary>
        /// 执行运行
        /// </summary>
        /// <returns></returns>
        public string DoRunIt()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/TestFlow.aspx?FK_Flow=" + this.No + "&Lang=CH";
        }
        /// <summary>
        /// 执行检查
        /// </summary>
        /// <returns></returns>
        public string DoCheck()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/AttrFlow/CheckFlow.aspx?FK_Flow=" + this.No + "&Lang=CH";
        }

        /// <summary>
        /// 启动限制规则
        /// </summary>
        /// <returns>返回URL</returns>
        public string DoLimit()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/AttrFlow/Limit.aspx?FK_Flow=" + this.No + "&Lang=CH";
        }
        /// <summary>
        /// 设置发起前置导航
        /// </summary>
        /// <returns></returns>
        public string DoStartGuide()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/AttrFlow/StartGuide.aspx?FK_Flow=" + this.No + "&Lang=CH";
        }
        /// <summary>
        /// 执行数据同步
        /// </summary>
        /// <returns></returns>
        public string DoDTS()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/AttrFlow/DTSBTable.aspx?FK_Flow=" + this.No + "&Lang=CH";
        }
        /// <summary>
        /// 导入
        /// </summary>
        /// <returns></returns>
        public string DoImp()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/AttrFlow/Imp.aspx?FK_Flow=" + this.No + "&Lang=CH";
        }
        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        public string DoExps()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/AttrFlow/Exp.aspx?FK_Flow=" + this.No + "&Lang=CH";
        }
        /// <summary>
        /// 执行重新装载数据
        /// </summary>
        /// <returns></returns>
        public string DoReloadRptData()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            return fl.DoReloadRptData();
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
            return Glo.CCFlowAppPath + "WF/Rpt/OneFlow.aspx?FK_Flow=" + this.No + "&DoType=Edit&FK_MapData=ND" +
                   int.Parse(this.No) + "Rpt";
        }
        /// <summary>
        /// 更新之后的事情，也要更新缓存。
        /// </summary>
        protected override void afterUpdate()
        {
            // Flow fl = new Flow();
            // fl.No = this.No;
            // fl.RetrieveFromDBSources();
            //fl.Update();

            if (Glo.OSModel == OSModel.OneMore)
            {
                DBAccess.RunSQL("UPDATE  GPM_Menu SET Name='" + this.Name + "' WHERE Flag='Flow" + this.No + "' AND FK_App='" + SystemConfig.SysNo + "'");
            }
        }
        protected override bool beforeUpdate()
        {
            //更新流程版本
            Flow.UpdateVer(this.No);

            #region 校验 flowmark 是不是唯一.
            if (this.FlowMark.Length > 0)
            {
                /*校验该标记是否重复.*/
                Flows fls = new Flows();
                fls.Retrieve(FlowAttr.FlowMark, this.FlowMark);
                foreach (Flow myfl in fls)
                {
                    if (myfl.No != this.No)
                        throw new Exception("@该流程标记{" + this.FlowMark + "}已经存在.");
                }
            }
            #endregion 校验 flowmark 是不是唯一.


            #region 同步事件实体.
            try
            {
                this.FlowEventEntity = BP.WF.Glo.GetFlowEventEntityStringByFlowMark(this.FlowMark, this.No);
            }
            catch
            {
                this.FlowEventEntity = "";
            }
            #endregion 同步事件实体.

            //更新缓存数据。
            Flow fl = new Flow(this.No);
            fl.Copy(this);

            #region 检查数据完整性 - 同步业务表数据。
            // 检查业务是否存在.
            if (fl.DTSWay != FlowDTSWay.None)
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

                    if (string.IsNullOrEmpty(nodes) == true)
                        throw new Exception("@业务数据同步数据配置错误，您设置了按照指定的节点配置，但是您没有设置节点,节点的设置格式如下：101,102,103");

                    string[] strs = nodes.Split(',');
                    foreach (string str in strs)
                    {
                        if (string.IsNullOrEmpty(str) == true)
                            continue;

                        if (BP.DA.DataType.IsNumStr(str) == false)
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
                if (!string.IsNullOrEmpty(fl.PTable))
                {
                    /*检查流程数据存储表填写的是否正确.*/
                    sql = "select count(*) as Num from  " + fl.PTable + " where 1=2";
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
            //同步流程数据表.
            string ndxxRpt = "ND" + int.Parse(this.No) + "Rpt";
            Flow fl = new Flow(this.No);
            if (fl.PTable != "ND" + int.Parse(this.No) + "Rpt")
            {
                BP.Sys.MapData md = new Sys.MapData(ndxxRpt);
                if (md.PTable != fl.PTable)
                    md.Update();
            }
            base.afterInsertUpdateAction();
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

