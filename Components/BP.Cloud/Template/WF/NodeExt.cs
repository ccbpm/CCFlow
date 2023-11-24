using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;
using System.Collections;
using BP.Port;
using System.IO;

namespace BP.Cloud.Template
{
    /// <summary>
    /// 节点属性.
    /// </summary>
    public class NodeExt : Entity
    {
        #region 常量
        /// <summary>
        /// CCFlow流程引擎
        /// </summary>
        private const string SYS_CCFLOW = "001";
        /// <summary>
        /// CCForm表单引擎
        /// </summary>
        private const string SYS_CCFORM = "002";
        #endregion

        #region 属性.
              
        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.Name);
            }
            set
            {
                this.SetValByKey(NodeAttr.Name, value);
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(NodeAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.FlowName);
            }
            set
            {
                this.SetValByKey(NodeAttr.FlowName, value);
            }
        }
        /// <summary>
        /// 接受人sql
        /// </summary>
        public string DeliveryParas11
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.DeliveryParas);
            }
            set
            {
                this.SetValByKey(NodeAttr.DeliveryParas, value);
            }
        }
        /// <summary>
        /// 是否可以退回
        /// </summary>
        public bool ReturnEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.ReturnRole);
            }
        }
        public bool IsYouLiTai
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsYouLiTai);
            }
        }
        /// <summary>
        /// 是否是返回节点?
        /// </summary>
        public bool IsSendBackNode
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsSendBackNode);
            }
            set
            {
                this.SetValByKey(NodeAttr.IsSendBackNode, value);
            }
        }
        /// <summary>
        /// 主键
        /// </summary>
        public override string PK
        {
            get
            {
                return "NodeID";
            }
        }

        #endregion 属性.

        #region 初试化全局的 Node
        /// <summary>
        /// 访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();

               
                if (BP.Web.WebUser.IsAdmin == false)
                    throw new Exception("err@管理员登录用户信息丢失,当前会话[" + BP.Web.WebUser.No + "," + BP.Web.WebUser.Name + "]");

                uac.IsUpdate = true;
                uac.IsDelete = false;
                uac.IsInsert = false;

                //uac.OpenForAdmin();
                return uac;
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 节点
        /// </summary>
        public NodeExt() { }
        /// <summary>
        /// 节点
        /// </summary>
        /// <param name="nodeid">节点ID</param>
        public NodeExt(int nodeid)
        {
            this.NodeID = nodeid;
            this.Retrieve();
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

                Map map = new Map("WF_Node", "节点");
                //map 的基 础信息.
                map.DepositaryOfEntity= Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;

                map.IndexField = NodeAttr.FK_Flow;

                #region  基础属性
                map.AddTBIntPK(NodeAttr.NodeID, 0, "节点ID", true, true);
                map.SetHelperUrl(NodeAttr.NodeID, "http://ccbpm.mydoc.io/?v=5404&t=17901");

                //map.SetHelperAlert(NodeAttr.Step, "它用于节点的排序，正确的设置步骤可以让流程容易读写."); //使用alert的方式显示帮助信息.
                map.AddTBString(NodeAttr.FK_Flow, null, "流程编号", false, false, 0, 5, 10, false, "http://ccbpm.mydoc.io/?v=5404&t=17023");
                map.AddTBString(NodeAttr.FlowName, null, "流程名", false, true, 0, 200, 10);

                map.AddTBString(NodeAttr.Name, null, "名称", true, false, 0, 100, 10, false, "http://ccbpm.mydoc.io/?v=5404&t=17903");
                map.SetHelperAlert(NodeAttr.Name, "修改节点名称时如果节点表单名称为空着节点表单名称和节点名称相同，否则节点名称和节点表单名称可以不相同");


                map.AddDDLSysEnum(NodeAttr.WhoExeIt, 0, "谁执行它", true, true, NodeAttr.WhoExeIt,
                    "@0=操作员执行@1=机器执行@2=混合执行");
                map.SetHelperUrl(NodeAttr.WhoExeIt, "http://ccbpm.mydoc.io/?v=5404&t=17913");

                map.AddDDLSysEnum(NodeAttr.ReadReceipts, 0, "已读回执", true, true, NodeAttr.ReadReceipts,
                "@0=不回执@1=自动回执@2=由上一节点表单字段决定@3=由SDK开发者参数决定");
                map.SetHelperUrl(NodeAttr.ReadReceipts, "http://ccbpm.mydoc.io/?v=5404&t=17915");


                //map.AddTBString(NodeAttr.DeliveryParas, null, "访问规则设置", true, false, 0, 300, 10);
                //map.AddDDLSysEnum(NodeAttr.CondModel, 0, "方向条件控制规则", true, true, NodeAttr.CondModel,
                //  "@0=由连接线条件控制@1=按照用户选择计算@2=发送按钮旁下拉框选择");
                //map.SetHelperUrl(NodeAttr.CondModel, "http://ccbpm.mydoc.io/?v=5404&t=17917"); //增加帮助

                // 撤销规则.
                map.AddDDLSysEnum(NodeAttr.CancelRole, (int)CancelRole.OnlyNextStep, "撤销规则", true, true,
                    NodeAttr.CancelRole, "@0=上一步可以撤销@1=不能撤销@2=上一步与开始节点可以撤销@3=指定的节点可以撤销");
                map.SetHelperUrl(NodeAttr.CancelRole, "http://ccbpm.mydoc.io/?v=5404&t=17919");

                map.AddBoolean(NodeAttr.CancelDisWhenRead, false, "对方已经打开就不能撤销", true, true);

                map.AddBoolean(NodeAttr.IsTask, true, "允许分配工作否?", true, true, false, "http://ccbpm.mydoc.io/?v=5404&t=17904");
                map.AddBoolean(NodeAttr.IsExpSender, true, "本节点接收人不允许包含上一步发送人", true, true, false);
                map.AddBoolean(NodeAttr.IsRM, true, "是否启用投递路径自动记忆功能?", true, true, false, "http://ccbpm.mydoc.io/?v=5404&t=17905");
                map.AddBoolean(NodeAttr.IsOpenOver, false, "已阅即完成?", true, true, true);
                map.SetHelperAlert(NodeAttr.IsOpenOver, "如果接受人打开了该工作，就标记该工作已经完成，而不必在点击发送按钮来标记完成。"); //增加帮助


                map.AddBoolean(NodeAttr.IsToParentNextNode, false, "子流程运行到该节点时，让父流程自动运行到下一步", true, true);
                map.AddBoolean(NodeAttr.IsYouLiTai, false, "该节点是否是游离态", true, true);
                map.SetHelperAlert(NodeAttr.IsYouLiTai, "当节点为游离状态的时候，只有连接的节点是固定节点才可以往下运行，否则流程结束");

                map.AddTBDateTime("DTFrom", "生命周期从", true, true);
                map.AddTBDateTime("DTTo", "生命周期到", true, true);

                map.AddBoolean(NodeAttr.IsBUnit, false, "是否是节点模版（业务单元）?", true, true, true, "http://ccbpm.mydoc.io/?v=5404&t=17904");

                map.AddTBString(NodeAttr.FocusField, null, "焦点字段", true, false, 0, 50, 10, true, "http://ccbpm.mydoc.io/?v=5404&t=17932");

                map.AddBoolean(NodeAttr.IsGuestNode, false, "是否是外部用户执行的节点(非组织结构人员参与处理工作的节点)?", true, true, true);

                //节点业务类型.
                map.AddTBInt("NodeAppType", 0, "节点业务类型", false, false);
                map.AddTBInt("FWCSta", 0, "节点状态", false, false);
                map.AddTBInt("FWCAth", 0, "审核附件是否启用", false, false);
                map.AddTBString(NodeAttr.SelfParas, null, "自定义属性", true, false, 0, 500, 10, true);


                map.AddTBInt(NodeAttr.Step, 0, "步骤(无计算意义)", true, false);
                map.SetHelperUrl(NodeAttr.Step, "http://ccbpm.mydoc.io/?v=5404&t=17902");
                map.AddTBString(NodeAttr.Tip, null, "操作提示", true, false, 0, 100, 10, false, "http://ccbpm.mydoc.io/?v=5404&t=18084");
                #endregion  基础属性

                #region 分合流子线程属性
                map.AddDDLSysEnum(NodeAttr.RunModel, 0, "节点类型",
                    true, false, NodeAttr.RunModel, "@0=普通@1=合流@2=分流@3=分合流@4=子线程");

                map.SetHelperUrl(NodeAttr.RunModel, "http://ccbpm.mydoc.io/?v=5404&t=17940"); //增加帮助.

                //子线程类型.
                map.AddDDLSysEnum(NodeAttr.SubThreadType, 0, "子线程类型", true, false, NodeAttr.SubThreadType, "@0=同表单@1=异表单");
                map.SetHelperUrl(NodeAttr.SubThreadType, "http://ccbpm.mydoc.io/?v=5404&t=17944"); //增加帮助

                map.AddTBDecimal(NodeAttr.PassRate, 100, "完成通过率", true, false);
                map.SetHelperUrl(NodeAttr.PassRate, "http://ccbpm.mydoc.io/?v=5404&t=17945"); //增加帮助.

                // 启动子线程参数 2013-01-04
                map.AddDDLSysEnum(NodeAttr.SubFlowStartWay, (int)SubFlowStartWay.None, "子线程启动方式", true, true,
                    NodeAttr.SubFlowStartWay, "@0=不启动@1=指定的字段启动@2=按明细表启动");
                map.AddTBString(NodeAttr.SubFlowStartParas, null, "启动参数", true, false, 0, 100, 10, true);
                map.SetHelperUrl(NodeAttr.SubFlowStartWay, "http://ccbpm.mydoc.io/?v=5404&t=17946"); //增加帮助


                //增加对退回到合流节点的 子线城的处理控制.
                map.AddBoolean(BtnAttr.ThreadIsCanDel, false, "是否可以删除子线程(当前节点已经发送出去的线程，并且当前节点是分流，或者分合流有效，在子线程退回后的操作)？", true, true, true);
                map.AddBoolean(BtnAttr.ThreadIsCanShift, false, "是否可以移交子线程(当前节点已经发送出去的线程，并且当前节点是分流，或者分合流有效，在子线程退回后的操作)？", true, true, true);

                ////待办处理模式.
                //map.AddDDLSysEnum(NodeAttr.TodolistModel, (int)TodolistModel.QiangBan, "多人待办处理模式", true, true, NodeAttr.TodolistModel,
                //    "@0=抢办模式@1=协作模式@2=队列模式@3=共享模式@4=协作组长模式");
                //map.SetHelperUrl(NodeAttr.TodolistModel, "http://ccbpm.mydoc.io/?v=5404&t=17947"); //增加帮助.
                //发送阻塞模式.
                //map.AddDDLSysEnum(NodeAttr.BlockModel, (int)BlockModel.None, "发送阻塞模式", true, true, NodeAttr.BlockModel,
                //    "@0=不阻塞@1=当前节点有未完成的子流程时@2=按约定格式阻塞未完成子流程@3=按照SQL阻塞@4=按照表达式阻塞");
                //map.SetHelperUrl(NodeAttr.BlockModel, "http://ccbpm.mydoc.io/?v=5404&t=17948"); //增加帮助.
                //map.AddTBString(NodeAttr.BlockExp, null, "阻塞表达式", true, false, 0, 700, 10,true);
                //map.SetHelperUrl(NodeAttr.BlockExp, "http://ccbpm.mydoc.io/?v=5404&t=17948");
                //map.AddTBString(NodeAttr.BlockAlert, null, "被阻塞时提示信息", true, false, 0, 700, 10, true);
                //map.SetHelperUrl(NodeAttr.BlockAlert, "http://ccbpm.mydoc.io/?v=5404&t=17948");

                map.AddBoolean(NodeAttr.IsAllowRepeatEmps, false, "是否允许子线程接受人员重复(仅当分流点向子线程发送时有效)?", true, true, true);
                map.AddBoolean(NodeAttr.AutoRunEnable, false, "是否启用自动运行？(仅当分流点向子线程发送时有效)", true, true, true);
                map.AddTBString(NodeAttr.AutoRunParas, null, "自动运行SQL", true, false, 0, 100, 10, true);

                //为广西计算中心加.
                map.AddBoolean(NodeAttr.IsSendBackNode, false, "是否是发送返回节点(发送当前节点,自动发送给该节点的发送人,发送节点.)?", true, true, true);
                #endregion 分合流子线程属性

                #region 自动跳转规则
                map.AddBoolean(NodeAttr.AutoJumpRole0, false, "处理人就是发起人", true, true, true);
                map.SetHelperUrl(NodeAttr.AutoJumpRole0, "http://ccbpm.mydoc.io/?v=5404&t=17949"); //增加帮助

                map.AddBoolean(NodeAttr.AutoJumpRole1, false, "处理人已经出现过", true, true, true);
                map.AddBoolean(NodeAttr.AutoJumpRole2, false, "处理人与上一步相同", true, true, true);
                map.AddBoolean(NodeAttr.WhenNoWorker, false, "(是)找不到人就跳转,(否)提示错误.", true, true, true);
                //         map.AddDDLSysEnum(NodeAttr.WhenNoWorker, 0, "找不到处理人处理规则",
                //true, true, NodeAttr.WhenNoWorker, "@0=提示错误@1=自动转到下一步");
                #endregion

                #region  功能按钮状态
                map.AddTBString(BtnAttr.SendLab, "发送", "发送按钮标签", true, false, 0, 50, 10);
                map.SetHelperUrl(BtnAttr.SendLab, "http://ccbpm.mydoc.io/?v=5404&t=16219");
                map.AddTBString(BtnAttr.SendJS, "", "按钮JS函数", true, false, 0, 999, 10);
                //map.SetHelperBaidu(BtnAttr.SendJS, "ccflow 发送前数据完整性判断"); //增加帮助.
                map.SetHelperUrl(BtnAttr.SendJS, "http://ccbpm.mydoc.io/?v=5404&t=17967");

                map.AddTBString(BtnAttr.SaveLab, "保存", "保存按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.SaveEnable, true, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.SaveLab, "http://ccbpm.mydoc.io/?v=5404&t=24366"); //增加帮助

                map.AddTBString(BtnAttr.ThreadLab, "子线程", "子线程按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ThreadEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.ThreadLab, "http://ccbpm.mydoc.io/?v=5404&t=16263"); //增加帮助

                map.AddDDLSysEnum(NodeAttr.ThreadKillRole, (int)ThreadKillRole.None, "子线程删除方式", true, true,
           NodeAttr.ThreadKillRole, "@0=不能删除@1=手工删除@2=自动删除", true);

                //map.SetHelperUrl(NodeAttr.ThreadKillRole, ""); //增加帮助

                //功能和子流程组件重复，屏蔽 hzm
                //  map.AddTBString(BtnAttr.SubFlowLab, "子流程", "子流程按钮标签", true, false, 0, 50, 10);
                //  map.SetHelperUrl(BtnAttr.SubFlowLab, "http://ccbpm.mydoc.io/?v=5404&t=16262");
                //   map.AddBoolean(BtnAttr.SubFlowEnable, false, "是否启用", true, true);
                //map.AddDDLSysEnum(BtnAttr.SubFlowCtrlRole, 0, "控制规则", true, true, BtnAttr.SubFlowCtrlRole, "@0=无@1=不可以删除子流程@2=可以删除子流程");

                map.AddTBString(BtnAttr.JumpWayLab, "跳转", "跳转按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(NodeAttr.JumpWay, 0, "跳转规则", true, true, NodeAttr.JumpWay);
                map.AddTBString(NodeAttr.JumpToNodes, null, "可跳转的节点", true, false, 0, 200, 10, true);
                map.SetHelperUrl(NodeAttr.JumpWay, "http://ccbpm.mydoc.io/?v=5404&t=16261"); //增加帮助.

                map.AddTBString(BtnAttr.ReturnLab, "退回", "退回按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(NodeAttr.ReturnRole, 0, "退回规则", true, true, NodeAttr.ReturnRole);
                map.SetHelperUrl(NodeAttr.ReturnRole, "http://ccbpm.mydoc.io/?v=5404&t=16255"); //增加帮助.
                map.AddTBString(NodeAttr.ReturnAlert, null, "被退回后信息提示", true, false, 0, 999, 10, true);

                map.AddBoolean(NodeAttr.IsBackTracking, false, "是否可以原路返回(启用退回功能才有效)", true, true, false);
                map.SetHelperUrl(NodeAttr.IsBackTracking, "http://ccbpm.mydoc.io/?v=5404&t=16255"); //增加帮助.
                
                //add for guangxi.
                map.AddBoolean(NodeAttr.IsKillEtcThread, true, "是否全部子线程退回(子线程退回到分流节点有效)", true, true, false);
                map.SetHelperAlert(NodeAttr.IsKillEtcThread, "子线程退回到分流节点是，是否允许全部退回。"); 

                //map.AddTBString(NodeAttr.RetunFieldsLable, "退回扩展字段", "退回扩展字段", true, false, 0, 50, 20);
                map.AddTBString(BtnAttr.ReturnField, "", "退回信息填写字段", true, false, 0, 50, 10);

                map.AddTBString(NodeAttr.ReturnReasonsItems, null, "退回原因", true, false, 0, 999, 10, true);

                map.AddBoolean(NodeAttr.ReturnCHEnable, false, "是否启用退回考核规则", true, true);

                map.AddDDLSysEnum(NodeAttr.ReturnOneNodeRole, 0, "单节点退回规则", true, true, NodeAttr.ReturnOneNodeRole,
                   "@@0=不启用@1=按照[退回信息填写字段]作为退回意见直接退回@2=按照[审核组件]填写的信息作为退回意见直接退回", true);


                map.AddTBString(BtnAttr.CCLab, "抄送", "抄送按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(NodeAttr.CCRole, 0, "抄送规则", true, true, NodeAttr.CCRole,
                    "@@0=不能抄送@1=手工抄送@2=自动抄送@3=手工与自动@4=按表单SysCCEmps字段计算@5=在发送前打开抄送窗口");
                map.SetHelperUrl(BtnAttr.CCLab, "http://ccbpm.mydoc.io/?v=5404&t=16259"); //增加帮助.

                // add 2014-04-05.
                map.AddDDLSysEnum(NodeAttr.CCWriteTo, 0, "抄送写入规则",
             true, true, NodeAttr.CCWriteTo, "@0=写入抄送列表@1=写入待办@2=写入待办与抄送列表", true);
                map.SetHelperUrl(NodeAttr.CCWriteTo, "http://ccbpm.mydoc.io/?v=5404&t=17976"); //增加帮助.
                map.AddTBString(NodeAttr.DoOutTime, null, "超时处理内容", true, false, 0, 300, 10, true);
                map.AddTBString(NodeAttr.DoOutTimeCond, null, "执行超时的条件", false, false, 0, 200, 100);

                map.AddTBString(BtnAttr.ShiftLab, "移交", "移交按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ShiftEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.ShiftLab, "http://ccbpm.mydoc.io/?v=5404&t=16257"); //增加帮助.note:none

                map.AddTBString(BtnAttr.DelLab, "删除", "删除按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.DelEnable, 0, "删除规则", true, true, BtnAttr.DelEnable);
                map.SetHelperUrl(BtnAttr.DelLab, "http://ccbpm.mydoc.io/?v=5404&t=17992"); //增加帮助.

                map.AddTBString(BtnAttr.EndFlowLab, "结束流程", "结束流程按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.EndFlowEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.EndFlowLab, "http://ccbpm.mydoc.io/?v=5404&t=17989"); //增加帮助

                map.AddTBString(BtnAttr.ShowParentFormLab, "查看父流程", "查看父流程按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ShowParentFormEnable, false, "是否启用", true, true);

                // add 2019.1.9 for 东孚.
                map.AddTBString(BtnAttr.OfficeBtnLab, "打开公文", "公文按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.OfficeBtnEnable, 0, "文件状态", true, true, BtnAttr.OfficeBtnEnable,
                "@0=不可用@1=可编辑@2=不可编辑", false);

                //map.AddTBString(BtnAttr.OfficeBtnLab, "公文主文件", "公文按钮标签", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.OfficeBtnEnable, false, "是否启用", true, true);

                // add 2017.9.1 for 天业集团.
                map.AddTBString(BtnAttr.PrintHtmlLab, "打印Html", "打印Html标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PrintHtmlEnable, false, "(打印Html)是否启用", true, true);
                // add 2020.5.25 for 交投集团.
                map.AddBoolean(BtnAttr.PrintHtmlMyView, false, "(打印Html)显示在查看器工具栏?", true, true);
                map.AddBoolean(BtnAttr.PrintHtmlMyCC, false, "(打印Html)显示在抄送工具栏?", true, true, true);


                // add 2017.9.1 for 天业集团.
                map.AddTBString(BtnAttr.PrintPDFLab, "打印pdf", "打印pdf标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PrintPDFEnable, false, "(打印pdf)是否启用", true, true);

                // add 2020.5.25 for 交投集团.
                map.AddBoolean(BtnAttr.PrintPDFMyView, false, "(打印pdf)显示在查看器工具栏?", true, true);
                map.AddBoolean(BtnAttr.PrintPDFMyCC, false, "(打印pdf)显示在抄送工具栏?", true, true, false);


                map.AddDDLSysEnum(BtnAttr.PrintPDFModle, 0, "PDF打印规则", true, true, BtnAttr.PrintPDFModle, "@0=全部打印@1=单个表单打印(针对树形表单)", true);
                map.AddTBString(BtnAttr.ShuiYinModle, null, "打印水印规则", true, false, 20, 100, 100, true);

                map.AddTBString(BtnAttr.PrintZipLab, "打包下载", "打包下载zip按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PrintZipEnable, false, "(打包下载zip)是否启用", true, true);

                // add 2020.5.25 for 交投集团.
                map.AddBoolean(BtnAttr.PrintZipMyView, false, "(打包下载zip)显示在查看器工具栏?", true, true);
                map.AddBoolean(BtnAttr.PrintZipMyCC, false, "(打包下载zip)显示在抄送工具栏?", true, true, false);



                map.AddTBString(BtnAttr.PrintDocLab, "打印单据", "打印单据按钮标签", true, false, 0, 50, 10);
                //map.AddDDLSysEnum(BtnAttr.PrintDocEnable, 0, "打印方式", true,
                //    true, BtnAttr.PrintDocEnable, "@0=不打印@1=打印网页@2=打印RTF模板@3=打印Word模版");
                map.AddBoolean(BtnAttr.PrintDocEnable, false, "是否启用", true, true);
                //map.SetHelperUrl(BtnAttr.PrintDocEnable, "http://ccbpm.mydoc.io/?v=5404&t=17979"); //增加帮助

                //map.AddBoolean(BtnAttr.PrintDocEnable, false, "是否启用", true, true);
                //map.AddTBString(BtnAttr.AthLab, "附件", "附件按钮标签", true, false, 0, 50, 10);
                //map.AddDDLSysEnum(NodeAttr.FJOpen, 0, this.ToE("FJOpen", "附件权限"), true, true, 
                //    NodeAttr.FJOpen, "@0=关闭附件@1=操作员@2=工作ID@3=流程ID");

                map.AddTBString(BtnAttr.TrackLab, "轨迹", "轨迹按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.TrackEnable, true, "是否启用", true, true);

                //map.SetHelperUrl(BtnAttr.TrackLab, this[SYS_CCFLOW, "轨迹"]); //增加帮助
                map.SetHelperUrl(BtnAttr.TrackLab, "http://ccbpm.mydoc.io/?v=5404&t=24369");

                map.AddTBString(BtnAttr.HungLab, "挂起", "挂起按钮标签", false, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.HungEnable, false, "是否启用", false, false);
                map.SetHelperUrl(BtnAttr.HungLab, "http://ccbpm.mydoc.io/?v=5404&t=16267"); //增加帮助.

                //      map.AddTBString(BtnAttr.SelectAccepterLab, "接受人", "接受人按钮标签", true, false, 0, 50, 10);
                //      map.AddDDLSysEnum(BtnAttr.SelectAccepterEnable, 0, "工作方式",
                //true, true, BtnAttr.SelectAccepterEnable);
                //      map.SetHelperUrl(BtnAttr.SelectAccepterLab, "http://ccbpm.mydoc.io/?v=5404&t=16256"); //增加帮助


                map.AddTBString(BtnAttr.SearchLab, "查询", "查询按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.SearchEnable, false, "是否启用", true, true);
                //map.SetHelperUrl(BtnAttr.SearchLab, this[SYS_CCFLOW, "查询"]); //增加帮助
                map.SetHelperUrl(BtnAttr.SearchLab, "http://ccbpm.mydoc.io/?v=5404&t=24373");

                //map.AddTBString(BtnAttr.WorkCheckLab, "审核", "审核按钮标签", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.WorkCheckEnable, false, "是否启用", true, true);

                //map.AddTBString(BtnAttr.BatchLab, "批处理", "批处理按钮标签", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.BatchEnable, false, "是否启用", true, true);
                //map.SetHelperUrl(BtnAttr.BatchLab, "http://ccbpm.mydoc.io/?v=5404&t=17920"); //增加帮助

                //功能暂时取消
                //map.AddTBString(BtnAttr.AskforLab, "加签", "加签按钮标签", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.AskforEnable, false, "是否启用", true, true);
                //map.SetHelperUrl(BtnAttr.AskforLab, "http://ccbpm.mydoc.io/?v=5404&t=16258");


                map.AddTBString(BtnAttr.HuiQianLab, "会签", "会签标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.HuiQianRole, 0, "会签模式", true, true, BtnAttr.HuiQianRole, "@0=不启用@1=协作(同事)模式@4=组长(领导)模式");

                //map.AddDDLSysEnum(BtnAttr.IsCanAddHuiQianer, 0, "协作模式被加签的人处理规则", true, true, BtnAttr.IsCanAddHuiQianer,
                //   "0=不允许增加其他协作人@1=允许增加协作人", false);

                map.AddDDLSysEnum(BtnAttr.HuiQianLeaderRole, 0, "组长会签规则", true, true, BtnAttr.HuiQianLeaderRole, "0=只有一个组长@1=最后一个组长发送@2=任意组长可以发送", true);

                map.AddTBString(BtnAttr.AddLeaderLab, "加主持人", "加主持人", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.AddLeaderEnable, false, "是否启用", true, true);

                // add by 周朋 2014-11-21. 让用户可以自己定义流转.
                map.AddTBString(BtnAttr.TCLab, "流转自定义", "流转自定义", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.TCEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.TCEnable, "http://ccbpm.mydoc.io/?v=5404&t=17978");

                //map.AddTBString(BtnAttr.AskforLabRe, "执行", "加签按钮标签", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.AskforEnable, false, "是否启用", true, true);
                // map.SetHelperUrl(BtnAttr.AskforLab, this[SYS_CCFLOW, "加签"]); //增加帮助

                // 删除了这个模式,让表单方案进行控制了,保留这两个字段以兼容.
                map.AddTBString(BtnAttr.WebOfficeLab, "公文", "文档按钮标签", false, false, 0, 50, 10);
                map.AddTBInt(BtnAttr.WebOfficeEnable, 0, "文档启用方式", false, false);

                //cut bye zhoupeng.
                //map.AddTBString(BtnAttr.WebOfficeLab, "公文", "文档按钮标签", true, false, 0, 50, 10);
                //map.AddDDLSysEnum(BtnAttr.WebOfficeEnable, 0, "文档启用方式", true, true, BtnAttr.WebOfficeEnable,
                //  "@0=不启用@1=按钮方式@2=标签页置后方式@3=标签页置前方式");//edited by liuxc,2016-01-18,from xc
                //map.SetHelperUrl(BtnAttr.WebOfficeLab, "http://ccbpm.mydoc.io/?v=5404&t=17993");

                // add by 周朋 2015-08-06. 重要性.
                map.AddTBString(BtnAttr.PRILab, "重要性", "重要性", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.PRIEnable, 0, "重要性规则", true, true, BtnAttr.PRIEnable, @"0=不启用@1=只读@2=编辑");
                //map.AddBoolean(BtnAttr.PRIEnable, false, "是否启用", true, true);

                // add by 周朋 2015-08-06. 节点时限.
                map.AddTBString(BtnAttr.CHLab, "节点时限", "节点时限", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.CHRole, 0, "时限规则", true, true, BtnAttr.CHRole, @"0=禁用@1=启用@2=只读@3=启用并可以调整流程应完成时间");

                // add 2017.5.4  邀请其他人参与当前的工作.
                map.AddTBString(BtnAttr.AllotLab, "分配", "分配按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.AllotEnable, false, "是否启用", true, true);

                // add by 周朋 2015-12-24. 节点时限.
                map.AddTBString(BtnAttr.FocusLab, "关注", "关注", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.FocusEnable, false, "是否启用", true, true);

                // add 2017.5.4 确认就是告诉发送人，我接受这件工作了.
                map.AddTBString(BtnAttr.ConfirmLab, "确认", "确认按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ConfirmEnable, false, "是否启用", true, true);

                // add 2019.3.10 增加List.
                map.AddTBString(BtnAttr.ListLab, "列表", "列表按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ListEnable, true, "是否启用", true, true);

                // 批量审核
                map.AddTBString(BtnAttr.BatchLab, "批量审核", "批量审核标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.BatchEnable, false, "是否启用", true, true);

                //备注 流程不流转，设置备注信息提醒已处理人员当前流程运行情况
                map.AddTBString(BtnAttr.NoteLab, "备注", "备注标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.NoteEnable, 0, "启用规则", true, true, BtnAttr.NoteEnable, @"0=禁用@1=启用@2=只读");

                //for 周大福.
                map.AddTBString(BtnAttr.HelpLab, "帮助", "帮助标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.HelpRole, 0, "帮助显示规则", true, true, BtnAttr.HelpRole, @"0=禁用@1=启用@2=强制提示@3=选择性提示");

                //for ctrl.cn
                map.AddTBString(BtnAttr.NextLab, "下一条", "下一条", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.NextRole, 0, "获得规则", true, true, BtnAttr.NextRole, @"0=禁用@1=相同节点@2=相同流程@3=相同的人@4=不限流程");

                #endregion  功能按钮状态

                //节点工具栏,主从表映射.
                map.AddDtl(new NodeToolbars(), NodeToolbarAttr.FK_Node);

                #region 基础功能.
                RefMethod rm = null;

                rm = new RefMethod();
                rm.Title = "接收人规则";
                rm.Icon = "../../WF/Admin/AttrNode/Img/Sender.png";
                rm.ClassMethodName = this.ToString() + ".DoAccepterRoleNew";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "节点事件"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoAction";
                rm.Icon = "../../WF/Img/Event.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "节点消息"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoMessage";
                rm.Icon = "../../WF/Img/Message24.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);



                rm = new RefMethod();
                rm.Title = "发送后转向"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoTurnToDeal";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Turnto.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "发送阻塞规则";
                rm.ClassMethodName = this.ToString() + ".DoBlockModel";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/BlockModel.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "多人处理规则";
                rm.ClassMethodName = this.ToString() + ".DoTodolistModel";
                rm.Icon = "../../WF/Img/Multiplayer.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "流程完成条件";
                rm.ClassMethodName = this.ToString() + ".DoCondFlow";
                rm.Icon = "../../WF/Admin/AttrNode/Img/Cond.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "节点完成条件";
                rm.ClassMethodName = this.ToString() + ".DoCondNode";
                rm.Icon = "../../WF/Admin/AttrNode/Img/Cond.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "待办删除规则";
                rm.ClassMethodName = this.ToString() + ".DoGenerWorkerListDelRole";
                //rm.Icon = "../../WF/Admin/AttrNode/Img/Cond.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);
                #endregion 基础功能.

                #region 字段相关功能（不显示在菜单里）
                rm = new RefMethod();
                rm.Title = "上传公文模板";
                rm.ClassMethodName = this.ToString() + ".DocTemp";
                rm.Icon = "../../WF/Img/FileType/doc.gif";
                //设置相关字段.
                rm.RefAttrKey = BtnAttr.OfficeBtnEnable;
                rm.RefAttrLinkLabel = "公文模板维护";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "可退回的节点(当退回规则设置可退回指定的节点时,该设置有效.)"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoCanReturnNodes";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkModel;
                //设置相关字段.
                rm.RefAttrKey = NodeAttr.ReturnRole;
                rm.RefAttrLinkLabel = "设置可退回的节点";
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "可撤销的节点"; // "可撤销发送的节点";
                rm.ClassMethodName = this.ToString() + ".DoCanCancelNodes";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.RefAttrKey = NodeAttr.CancelRole; //在该节点下显示连接.
                rm.RefAttrLinkLabel = "";
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "绑定打印格式模版(当打印方式为打印RTF格式模版时,该设置有效)";
                //rm.ClassMethodName = this.ToString() + ".DoBill";
                //rm.Icon = "../../WF/Img/FileType/doc.gif";
                //rm.RefMethodType = RefMethodType.LinkeWinOpen;

                //rm = new RefMethod();
                //rm.Title = "打印设置"; // "可撤销发送的节点";
                ////设置相关字段.
                //rm.RefAttrKey = NodeAttr.PrintDocEnable;
                //rm.Target = "_blank";
                //rm.RefMethodType = RefMethodType.LinkeWinOpen;
                //map.AddRefMethod(rm);
                //if (SystemConfig.CustomerNo == "HCBD")
                //{
                //    /* 为海成邦达设置的个性化需求. */
                //    rm = new RefMethod();
                //    rm.Title = "DXReport设置";
                //    rm.ClassMethodName = this.ToString() + ".DXReport";
                //    rm.Icon = "../../WF/Img/FileType/doc.gif";
                //    map.AddRefMethod(rm);
                //}

                rm = new RefMethod();
                rm.Title = "设置自动抄送规则(当节点为自动抄送时,该设置有效.)"; // "抄送规则";
                rm.ClassMethodName = this.ToString() + ".DoCCRole";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                //设置相关字段.
                rm.RefAttrKey = NodeAttr.CCRole;
                rm.RefAttrLinkLabel = "自动抄送设置";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);
                #endregion 字段相关功能（不显示在菜单里）

                #region 表单设置.
                rm = new RefMethod();
                rm.Title = "表单方案";
                rm.Icon = "../../WF/Admin/CCFormDesigner/Img/Form.png";
                rm.ClassMethodName = this.ToString() + ".DoSheet";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "表单设置";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "手机表单字段顺序";
                rm.Icon = "../../WF/Admin/CCFormDesigner/Img/telephone.png";
                //rm.Icon = ../../Img/Mobile.png";
                rm.ClassMethodName = this.ToString() + ".DoSortingMapAttrs";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "表单设置";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "节点组件";
                rm.Icon = "../../WF/Img/Components.png";
                //rm.Icon = ../../Img/Mobile.png";
                rm.ClassMethodName = this.ToString() + ".DoFrmNodeComponent";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "表单设置";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "特别控件特别用户权限";
                rm.Icon = "../../WF/Admin/CCFormDesigner/Img/SpecUserSpecFields.png";
                rm.ClassMethodName = this.ToString() + ".DoSpecFieldsSpecUsers()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "表单设置";
                map.AddRefMethod(rm);
                #endregion 表单设置.

                #region 考核属性.

                map.AddTBInt(NodeAttr.TAlertRole, 0, "逾期提醒规则", false, false); //"限期(天)"
                map.AddTBInt(NodeAttr.TAlertWay, 0, "逾期提醒方式", false, false); //"限期(天)
                map.AddTBInt(NodeAttr.WAlertRole, 0, "预警提醒规则", false, false); //"限期(天)"
                map.AddTBInt(NodeAttr.WAlertWay, 0, "预警提醒方式", false, false); //"限期(天)"

                map.AddTBFloat(NodeAttr.TCent, 2, "扣分(每延期1小时)", false, false);
                map.AddTBInt(NodeAttr.CHWay, 0, "考核方式", false, false); //"限期(天)"

                //考核相关.
                map.AddTBInt(NodeAttr.IsEval, 0, "是否工作质量考核", false, false);
                map.AddTBInt(NodeAttr.OutTimeDeal, 0, "超时处理方式", false, false);

                #endregion 考核属性.

                #region 父子流程.
                rm = new RefMethod();
                rm.Title = "子流程基本设置";
                rm.Icon = "../../WF/Admin/AttrNode/Img/SubFlows.png";
                rm.ClassMethodName = this.ToString() + ".DoSubFlow";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "父子流程";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "手动启动子流程"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoSubFlowHand";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "父子流程";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "自动触发子流程"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoSubFlowAuto";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "父子流程";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "延续子流程"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoSubFlowYanXu";
                //  rm.Icon = "../../WF/Img/Event.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "父子流程";
                map.AddRefMethod(rm);
                #endregion 父子流程.

                #region 考核.

                rm = new RefMethod();
                rm.Title = "设置考核规则";
                rm.Icon = "../../WF/Admin/CCFormDesigner/Img/CH.png";
                rm.ClassMethodName = this.ToString() + ".DoCHRole";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "考核规则";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "超时处理规则";
                rm.Icon = "../../WF/Admin/CCFormDesigner/Img/OvertimeRole.png";
                rm.ClassMethodName = this.ToString() + ".DoCHOvertimeRole";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "考核规则";
                map.AddRefMethod(rm);
                #endregion 考核.

                #region 实验中的功能
                rm = new RefMethod();
                rm.Title = "自定义属性(通用)";
                rm.ClassMethodName = this.ToString() + ".DoSelfParas()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "实验中的功能";
                rm.Visable = false;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "自定义属性(自定义)";
                rm.ClassMethodName = this.ToString() + ".DoNodeAttrExt()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "实验中的功能";
                rm.Visable = false;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设置节点类型";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Node.png";
                rm.ClassMethodName = this.ToString() + ".DoNodeAppType()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "实验中的功能";
                //rm.Visable = false;
                map.AddRefMethod(rm);

                if (Glo.CCBPMRunModel != CCBPMRunModel.SAAS)
                {
                    rm = new RefMethod();
                    rm.Title = "设置为模版";

                    string info = "如果把这个节点设置为模版,以后在新建节点的时候,就会按照当前的属性初始化节点数据.";
                    info += "\t\n产生的数据文件存储在\\DataUser\\Xml\\下.";
                    rm.Warning = info;

                    //  rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Node.png";
                    rm.ClassMethodName = this.ToString() + ".DoSetTemplate()";
                    rm.RefMethodType = RefMethodType.Func;
                    rm.GroupName = "实验中的功能";
                    //rm.Visable = false;
                    map.AddRefMethod(rm);
                }


                rm = new RefMethod();
                rm.Title = "批量设置节点属性";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Node.png";
                rm.ClassMethodName = this.ToString() + ".DoNodeAttrs()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "实验中的功能";
                rm.Visable = false;
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "设置独立表单树权限";
                //rm.Icon = ../../Img/Btn/DTS.gif";
                //rm.ClassMethodName = this.ToString() + ".DoNodeFormTree";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.GroupName = "实验中的功能";
                //map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "工作批处理规则";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoBatchStartFields()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "实验中的功能";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "抄送人规则";
                rm.GroupName = "实验中的功能";
                rm.Icon = "../../WF/Admin/AttrNode/Img/CC.png";
                rm.ClassMethodName = this.ToString() + ".DoCCer";  //要执行的方法名.
                rm.RefMethodType = RefMethodType.RightFrameOpen; // 功能类型
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "设置提示信息";
                rm.GroupName = "实验中的功能";
                //     rm.Icon = "../../WF/Admin/AttrNode/Img/CC.png";
                rm.ClassMethodName = this.ToString() + ".DoHelpRole";  //要执行的方法名.
                rm.RefAttrKey = BtnAttr.HelpRole; //帮助信息.
                rm.RefMethodType = RefMethodType.LinkeWinOpen; // 功能类型
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "退回扩展列";
                rm.ClassMethodName = this.ToString() + ".DtlOfReturn";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkModel;
                rm.RefAttrKey = NodeAttr.ReturnCHEnable;
                map.AddRefMethod(rm);


                #endregion 实验中的功能

                this._enMap = map;
                return this._enMap;
            }
        }

        #region 考核规则.
        /// <summary>
        /// 考核规则
        /// </summary>
        /// <returns></returns>
        public string DoCHRole()
        {
            return "../../Admin/AttrNode/CHRole.htm?FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// 超时处理规则
        /// </summary>
        /// <returns></returns>
        public string DoCHOvertimeRole()
        {
            return "../../Admin/AttrNode/CHOvertimeRole.htm?FK_Node=" + this.NodeID;
        }
        #endregion 考核规则.

        #region 基础设置.
        /// <summary>
        /// 待办删除规则
        /// </summary>
        /// <returns></returns>
        public string DoGenerWorkerListDelRole()
        {
            return "../../Admin/AttrNode/GenerWorkerListDelRole.htm?s=d34&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// 多人处理规则.
        /// </summary>
        /// <returns></returns>
        public string DoTodolistModel()
        {
            return "../../Admin/AttrNode/TodolistModel.htm?s=d34&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// 批处理规则
        /// </summary>
        /// <returns></returns>
        public string DoBatchStartFields()
        {
            return "../../Admin/AttrNode/BatchStartFields.htm?s=d34&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// 批量修改节点属性
        /// </summary>
        /// <returns></returns>
        public string DoNodeAttrs()
        {
            return "../../Admin/AttrFlow/NodeAttrs.htm?NodeID=0&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 表单方案
        /// </summary>
        /// <returns></returns>
        public string DoSheet()
        {
            return "../../Admin/AttrNode/FrmSln/Default.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        public string DoSheetOld()
        {
            return "../../Admin/AttrNode/NodeFromWorkModel.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }


        /// <summary>
        /// 接受人规则
        /// </summary>
        /// <returns></returns>
        public string DoAccepterRoleNew()
        {
            return "../../Admin/AttrNode/AccepterRole/Default.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }

        /// <summary>
        /// 发送阻塞规则
        /// </summary>
        /// <returns></returns>
        public string DoBlockModel()
        {
            return "../../Admin/AttrNode/BlockModel/Default.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 发送后转向规则
        /// </summary>
        /// <returns></returns>
        public string DoTurnToDeal()
        {
            return "../../Admin/AttrNode/TurnTo/0.TurntoDefault.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 抄送人规则
        /// </summary>
        /// <returns></returns>
        public string DoCCer()
        {
            return "../../Admin/AttrNode/CCRole.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 加载提示信息
        /// </summary>
        /// <returns></returns>
        public string DoHelpRole()
        {
            return "../../Admin/FoolFormDesigner/HelpRole.htm?NodeID=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        #endregion

        #region 表单相关.
        /// <summary>
        /// 节点组件
        /// </summary>
        /// <returns></returns>
        public string DoFrmNodeComponent()
        {
            return "../../Comm/EnOnly.htm?EnName=BP.WF.Template.FrmNodeComponent&PKVal=" + this.NodeID + "&t=" + DataType.CurrentDataTime;
        }
        /// <summary>
        /// 特别用户特殊字段权限.
        /// </summary>
        /// <returns></returns>
        public string DoSpecFieldsSpecUsers()
        {
            return "../../Admin/AttrNode/SepcFiledsSepcUsers.htm?FK_Flow=" + this.FK_Flow + "&FK_MapData=ND" +
                   this.NodeID + "&FK_Node=" + this.NodeID + "&t=" + DataType.CurrentDataTime;
        }
        /// <summary>
        /// 排序字段顺序
        /// </summary>
        /// <returns></returns>
        public string DoSortingMapAttrs()
        {
            return "../../Admin/MobileFrmDesigner/Default.htm?FK_Flow=" + this.FK_Flow + "&FK_MapData=ND" +
                   this.NodeID + "&t=" + DataType.CurrentDataTime;
        }
        #endregion 表单相关.

        #region 实验中的功能.
        /// <summary>
        /// 设置模版
        /// </summary>
        /// <returns></returns>
        public string DoSetTemplate()
        {
            DataTable dt = this.ToDataTableField();
            dt.TableName = "Node";
            dt.WriteXml(SystemConfig.PathOfDataUser + "\\XML\\DefaultNewNodeAttr.xml");
            return "执行成功.";
        }
        /// <summary>
        /// 自定义参数（通用）
        /// </summary>
        /// <returns></returns>
        public string DoSelfParas()
        {
            return "../../Admin/AttrNode/SelfParas.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// 自定义参数（自定义）
        /// </summary>
        /// <returns></returns>
        public string DoNodeAttrExt()
        {
            return "../../../DataUser/OverrideFiles/NodeAttrExt.htm?FK_Flow=" + this.NodeID;
        }

        /// <summary>
        /// 设置节点类型
        /// </summary>
        /// <returns></returns>
        public string DoNodeAppType()
        {
            return "../../Admin/AttrNode/NodeAppType.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow + "&tk=" + new Random().NextDouble();
        }
        #endregion

        #region 子流程。
        /// <summary>
        /// 父子流程
        /// </summary>
        /// <returns></returns>
        public string DoSubFlow()
        {
            return "../../Comm/RefFunc/EnOnly.htm?EnName=BP.WF.Template.FrmSubFlow&PK=" + this.NodeID;
        }
        /// <summary>
        /// 自动触发
        /// </summary>
        /// <returns></returns>
        public string DoSubFlowAuto()
        {
            return "../../Admin/AttrNode/SubFlow/SubFlowAuto.htm?FK_Node=" + this.NodeID + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// 手动启动子流程
        /// </summary>
        /// <returns></returns>
        public string DoSubFlowHand()
        {
            return "../../Admin/AttrNode/SubFlow/SubFlowHand.htm?FK_Node=" + this.NodeID + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// 延续子流程
        /// </summary>
        /// <returns></returns>
        public string DoSubFlowYanXu()
        {
            return "../../Admin/AttrNode/SubFlow/SubFlowYanXu.htm?FK_Node=" + this.NodeID + "&tk=" + new Random().NextDouble();
        }
        #endregion 子流程。


        public string DoTurn()
        {
            return "../../Admin/AttrNode/TurnTo.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
            //, "节点完成转向处理", "FrmTurn", 800, 500, 200, 300);
            //BP.WF.Node nd = new BP.WF.Node(this.NodeID);
            //return nd.DoTurn();
        }
        /// <summary>
        /// 公文模板
        /// </summary>
        /// <returns></returns>
        public string DocTemp()
        {
            return "../../Admin/AttrNode/DocTemp.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 抄送规则
        /// </summary>
        /// <returns></returns>
        public string DoCCRole()
        {
            return "../../Comm/En.htm?EnName=BP.WF.Template.CC&PKVal=" + this.NodeID + "&FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 个性化接受人窗口
        /// </summary>
        /// <returns></returns>
        public string DoAccepter()
        {
            return "../../Comm/En.htm?EnName=BP.WF.Template.Selector&PK=" + this.NodeID;
        }
        /// <summary>
        /// 可触发的子流程
        /// </summary>
        /// <returns></returns>
        public string DoActiveFlows()
        {
            return "../../Admin/ConditionSubFlow.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 退回节点
        /// </summary>
        /// <returns></returns>
        public string DoCanReturnNodes()
        {
            return "../../Admin/AttrNode/CanReturnNodes.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 撤销发送的节点
        /// </summary>
        /// <returns></returns>
        public string DoCanCancelNodes()
        {
            return "../../Admin/AttrNode/CanCancelNodes.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        
        /// <summary>
        /// 流程完成条件
        /// </summary>
        /// <returns></returns>
        public string DoCondFlow()
        {
            return "../../Admin/Cond2020/List.htm?CondType=" + (int)CondType.Flow + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&ToNodeID=" + this.NodeID;
        }
        /// <summary>
        /// 节点完成条件
        /// </summary>
        /// <returns></returns>
        public string DoCondNode()
        {
            return "../../Admin/Cond2020/List.htm?CondType=" + (int)CondType.Node + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&ToNodeID=" + this.NodeID;
        }
        /// <summary>
        /// 设计傻瓜表单
        /// </summary>
        /// <returns></returns>
        public string DoFormCol4()
        {
            return "../../Admin/FoolFormDesigner/Designer.htm?PK=ND" + this.NodeID;
        }
        /// <summary>
        /// 设计自由表单
        /// </summary>
        /// <returns></returns>
        public string DoFormFree()
        {
            return "../../Admin/FoolFormDesigner/CCForm/Frm.htm?FK_MapData=ND" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 绑定独立表单
        /// </summary>
        /// <returns></returns>
        public string DoFormTree()
        {
            return "../../Admin/Sln/BindFrms.htm?ShowType=FlowFrms&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&Lang=CH";
        }

        public string DoMapData()
        {
            int i = this.GetValIntByKey(NodeAttr.FormType);

            // 类型.
            NodeFormType type = (NodeFormType)i;
            switch (type)
            {
                case NodeFormType.FreeForm:
                    return "../../Admin/FoolFormDesigner/CCForm/Frm.htm?FK_MapData=ND" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
                    break;
                default:
                case NodeFormType.FoolForm:
                    return "../../Admin/FoolFormDesigner/Designer.htm?PK=ND" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
                    break;
            }
            return null;
        }

        /// <summary>
        /// 消息
        /// </summary>
        /// <returns></returns>
        public string DoMessage()
        {
            return "../../Admin/AttrNode/PushMessage.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// 事件
        /// </summary>
        /// <returns></returns>
        public string DoAction()
        {
            return "../../Admin/AttrNode/Action.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// 单据打印
        /// </summary>
        /// <returns></returns>
        public string DoBill()
        {
            return "../../Admin/AttrNode/Bill.htm?FK_Node=" + this.NodeID + "&NodeID=" + this.NodeID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// 保存提示信息
        /// </summary>
        /// <returns></returns>
        public string SaveHelpAlert(string text)
        {
            string file = SystemConfig.PathOfDataUser + "\\CCForm\\HelpAlert\\" + this.NodeID + ".htm";
            string folder = System.IO.Path.GetDirectoryName(file);
            //若文件夹不存在，则创建
            if (System.IO.Directory.Exists(folder) == false)
                System.IO.Directory.CreateDirectory(folder);

            DataType.WriteFile(file, text);
            return "保存成功！";
        }
        /// <summary>
        /// 读取提示信息
        /// </summary>
        /// <returns></returns>
        public string ReadHelpAlert()
        {
            string doc = "";
            string file = SystemConfig.PathOfDataUser + "\\CCForm\\HelpAlert\\" + this.NodeID + ".htm";
            string folder = System.IO.Path.GetDirectoryName(file);
            if (System.IO.Directory.Exists(folder) != false)
            {
                if (File.Exists(file))
                {
                    doc = DataType.ReadTextFile(file);

                }
            }
            return doc;
        }
        public string DtlOfReturn()
        {
            string url = "../../Admin/FoolFormDesigner/MapDefDtlFreeFrm.htm?FK_MapDtl=BP.WF.ReturnWorks" + "&For=BP.WF.ReturnWorks&FK_Flow=" + this.FK_Flow;
            return url;
        }
        protected override bool beforeUpdate()
        {
            //更新流程版本
            Flow.UpdateVer(this.FK_Flow);

            #region 处理节点数据.
            Node nd = new Node(this.NodeID);
            if (nd.IsStartNode == true)
            {
                //开始节点不能设置游离状态
                if (this.IsYouLiTai == true)
                    throw new Exception("当前节点是开始节点不能设置游离状态");
                /*处理按钮的问题*/
                //不能退回, 加签，移交，退回, 子线程.
                //this.SetValByKey(BtnAttr.ReturnRole,(int)ReturnRole.CanNotReturn); //开始节点可以退回。
                this.SetValByKey(BtnAttr.HungEnable, false);
                this.SetValByKey(BtnAttr.ThreadEnable, false); //子线程.
            }

            //是否是发送返回节点？
            nd.IsSendBackNode = this.IsSendBackNode;

            if (nd.IsSendBackNode == true)
            {
                //强制设置按照连接线控制.
                nd.CondModel = DirCondModel.ByLineCond;
            }
            nd.DirectUpdate(); //直接更新.

            if (this.IsSendBackNode == true)
            {
                if (nd.HisToNDNum != 0)
                    this.IsSendBackNode = false;

                //    throw new Exception("err@您设置当前节点为【发送自动返回节点】，但是该节点配置到到达节点，是不正确的。");
            }


            #region 如果有跳转，
            //if (this.AutoJumpRole0 == true || this.AutoJumpRole0
            //    || this.AutoJumpRole1 || this.AutoJumpRole2 == true || this.WhenNoWorker == true)
            //{
            //    /* 凡是到达当前节点的节点，都不能设置为让用户来选择. */

            //}
            #endregion 如果有跳转


            if (nd.HisRunModel == RunModel.HL || nd.HisRunModel == RunModel.FHL)
            {
                /*如果是合流点*/
            }
            else
            {
                this.SetValByKey(BtnAttr.ThreadEnable, false); //子线程.
            }

            //如果启动了会签,并且是抢办模式,强制设置为队列模式.或者组长模式.
            if (this.HuiQianRole != WF.HuiQianRole.None)
            {
                if (this.HuiQianRole == WF.HuiQianRole.Teamup)
                    DBAccess.RunSQL("UPDATE WF_Node SET TodolistModel=" + (int)TodolistModel.Teamup + "  WHERE NodeID=" + this.NodeID);

                if (this.HuiQianRole == WF.HuiQianRole.TeamupGroupLeader)
                    DBAccess.RunSQL("UPDATE WF_Node SET TodolistModel=" + (int)TodolistModel.TeamupGroupLeader + ", TeamLeaderConfirmRole=" + (int)TeamLeaderConfirmRole.HuiQianLeader + " WHERE NodeID=" + this.NodeID);
            }

            // @杜. 翻译&测试.
            if (nd.CondModel == DirCondModel.ByLineCond)
            {
                /* 如果当前节点方向条件控制规则是按照连接线决定的, 
                 * 那就判断到达的节点的接受人规则，是否是按照上一步来选择，如果是就抛出异常.*/

                //获得到达的节点.
                Nodes nds = nd.HisToNodes;
                foreach (Node mynd in nds)
                {
                    if (mynd.HisDeliveryWay == DeliveryWay.BySelected)
                    {
                        string errInfo = "设置矛盾:";
                        errInfo += "@当前节点您设置的访问规则是按照方向条件控制的";
                        errInfo += "但是到达的节点[" + mynd.Name + "]的接收人规则是按照上一步选择的,设置矛盾.";
                        // throw new Exception(errInfo);
                    }
                }
            }

            //如果启用了在发送前打开, 当前节点的方向条件控制模式，是否是在下拉框边选择.?
            if (1 == 2 && nd.CondModel != DirCondModel.SendButtonSileSelect)
            {
                /*如果是启用了按钮，就检查当前节点到达的节点是否有【按照选择接受人】的方式确定接收人的范围. */
                Nodes nds = nd.HisToNodes;
                foreach (Node mynd in nds)
                {
                    if (mynd.HisDeliveryWay == DeliveryWay.BySelected)
                    {
                        //强制设置安装人员选择器来选择.
                        this.SetValByKey(NodeAttr.CondModel, (int)DirCondModel.SendButtonSileSelect);
                        break;
                    }
                }
            }
            #endregion 处理节点数据.

            #region 创建审核组件附件
            if (this.FWCAth == FWCAth.MinAth)
            {
                FrmAttachment workCheckAth = new FrmAttachment();
                workCheckAth.MyPK = "ND" + this.NodeID + "_FrmWorkCheck";
                //不包含审核组件
                if (workCheckAth.RetrieveFromDBSources() == 0)
                {
                    workCheckAth = new FrmAttachment();
                    /*如果没有查询到它,就有可能是没有创建.*/
                    workCheckAth.MyPK = "ND" + this.NodeID + "_FrmWorkCheck";
                    workCheckAth.FK_MapData = "ND" + this.NodeID.ToString();
                    workCheckAth.NoOfObj = "FrmWorkCheck";
                    workCheckAth.Exts = "*.*";

                    //存储路径.
                   // workCheckAth.SaveTo = "/DataUser/UploadFile/";
                    workCheckAth.IsNote = false; //不显示note字段.
                    workCheckAth.IsVisable = false; // 让其在form 上不可见.

                    //位置.
                    workCheckAth.X = (float)94.09;
                    workCheckAth.Y = (float)333.18;
                    workCheckAth.W = (float)626.36;
                    workCheckAth.H = (float)150;

                    //多附件.
                    workCheckAth.UploadType = AttachmentUploadType.Multi;
                    workCheckAth.Name = "审核组件";
                    workCheckAth.SetValByKey("AtPara", "@IsWoEnablePageset=1@IsWoEnablePrint=1@IsWoEnableViewModel=1@IsWoEnableReadonly=0@IsWoEnableSave=1@IsWoEnableWF=1@IsWoEnableProperty=1@IsWoEnableRevise=1@IsWoEnableIntoKeepMarkModel=1@FastKeyIsEnable=0@IsWoEnableViewKeepMark=1@FastKeyGenerRole=");
                    workCheckAth.Insert();
                }
            }
            #endregion 创建审核组件附件

            #region 审核组件.
            GroupField gf = new GroupField();
            if (this.HisFrmWorkCheckSta == FrmWorkCheckSta.Disable)
            {
                gf.Delete(GroupFieldAttr.FrmID, "ND" + this.NodeID, GroupFieldAttr.CtrlType, GroupCtrlType.FWC);
            }
            else
            {
                if (gf.IsExit(GroupFieldAttr.CtrlType, GroupCtrlType.FWC, GroupFieldAttr.FrmID, "ND" + this.NodeID) == false)
                {
                    gf = new GroupField();
                    gf.EnName = "ND" + this.NodeID;
                    gf.CtrlType = GroupCtrlType.FWC;
                    gf.Lab = "审核信息";
                    gf.Idx = 0;
                    gf.Insert(); //插入.
                }
            }
            #endregion 审核组件.

            BtnLab btnLab = new BtnLab(this.NodeID);
            btnLab.RetrieveFromDBSources();

            //清除所有的缓存.
            CashEntity.DCash.Clear();

            return base.beforeUpdate();
        }
        protected override void afterInsertUpdateAction()
        {
            Node fl = new Node();
            fl.NodeID = this.NodeID;
            fl.RetrieveFromDBSources();
            if (this.IsYouLiTai == true)
                fl.SetPara("IsYouLiTai", 1);
            else
                fl.SetPara("IsYouLiTai", 0);
            fl.Update();

            BtnLab btnLab = new BtnLab();
            btnLab.NodeID = this.NodeID;
            btnLab.RetrieveFromDBSources();
            Cash2019.UpdateRow(btnLab.ToString(), this.NodeID.ToString(), btnLab.Row);


            CC cc = new CC();
            cc.NodeID = this.NodeID;
            cc.RetrieveFromDBSources();
            Cash2019.UpdateRow(cc.ToString(), this.NodeID.ToString(), cc.Row);

            FrmNodeComponent frmNodeComponent = new FrmNodeComponent();
            frmNodeComponent.NodeID = this.NodeID;
            frmNodeComponent.RetrieveFromDBSources();
            Cash2019.UpdateRow(frmNodeComponent.ToString(), this.NodeID.ToString(), frmNodeComponent.Row);

            FrmThread frmThread = new FrmThread();
            frmThread.NodeID = this.NodeID;
            frmThread.RetrieveFromDBSources();
            Cash2019.UpdateRow(frmThread.ToString(), this.NodeID.ToString(), frmThread.Row);

            FrmTrack frmTrack = new FrmTrack();
            frmTrack.NodeID = this.NodeID;
            frmTrack.RetrieveFromDBSources();
            Cash2019.UpdateRow(frmTrack.ToString(), this.NodeID.ToString(), frmTrack.Row);

            FrmTransferCustom frmTransferCustom = new FrmTransferCustom();
            frmTransferCustom.NodeID = this.NodeID;
            frmTransferCustom.RetrieveFromDBSources();
            Cash2019.UpdateRow(frmTransferCustom.ToString(), this.NodeID.ToString(), frmTransferCustom.Row);

            NodeWorkCheck frmWorkCheck = new NodeWorkCheck();
            frmWorkCheck.NodeID = this.NodeID;
            frmWorkCheck.RetrieveFromDBSources();
            Cash2019.UpdateRow(frmWorkCheck.ToString(), this.NodeID.ToString(), frmWorkCheck.Row);

            NodeSheet nodeSheet = new NodeSheet();
            nodeSheet.NodeID = this.NodeID;
            nodeSheet.RetrieveFromDBSources();
            Cash2019.UpdateRow(nodeSheet.ToString(), this.NodeID.ToString(), nodeSheet.Row);

            NodeSimple nodeSimple = new NodeSimple();
            nodeSimple.NodeID = this.NodeID;
            nodeSimple.RetrieveFromDBSources();
            Cash2019.UpdateRow(nodeSimple.ToString(), this.NodeID.ToString(), nodeSimple.Row);

            FrmSubFlow frmSubFlow = new FrmSubFlow();
            frmSubFlow.NodeID = this.NodeID;
            frmSubFlow.RetrieveFromDBSources();
            Cash2019.UpdateRow(frmSubFlow.ToString(), this.NodeID.ToString(), frmSubFlow.Row);

            GetTask getTask = new GetTask();
            getTask.NodeID = this.NodeID;
            getTask.RetrieveFromDBSources();
            Cash2019.UpdateRow(getTask.ToString(), this.NodeID.ToString(), getTask.Row);

            //如果是组长会签模式，通用选择器只能单项选择
            if (this.HuiQianRole == HuiQianRole.TeamupGroupLeader && this.HuiQianLeaderRole == HuiQianLeaderRole.OnlyOne)
            {
                Selector selector = new Selector();
                selector.NodeID = this.NodeID;
                selector.RetrieveFromDBSources();
                selector.IsSimpleSelector = true;
                selector.Update();

            }

            base.afterInsertUpdateAction();
        }
        #endregion
    }
    /// <summary>
    /// 节点集合
    /// </summary>
    public class NodeExts : Entities
    {
        #region 构造方法
        /// <summary>
        /// 节点集合
        /// </summary>
        public NodeExts()
        {
        }

        public NodeExts(string fk_flow)
        {
            this.Retrieve(NodeAttr.FK_Flow, fk_flow, NodeAttr.Step);
            return;
        }
        #endregion

        public override Entity GetNewEntity
        {
            get { return new NodeExt(); }
        }
    }
}
