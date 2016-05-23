using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;
using System.Collections;
using BP.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 节点属性.
    /// </summary>
    public class NodeExt : Entity
    {
        #region 索引
        /// <summary>
        /// 获取节点的帮助信息url
        /// <para></para>
        /// <para>added by liuxc,2014-8-19</para> 
        /// </summary>
        /// <param name="sysNo">帮助网站中所属系统No</param>
        /// <param name="searchTitle">帮助主题标题</param>
        /// <returns></returns>
        private string this[string sysNo, string searchTitle]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(sysNo) || string.IsNullOrWhiteSpace(searchTitle))
                    return "javascript:alert('此处还没有帮助信息！')";

                return string.Format("http://online.ccflow.org/KM/Tree.aspx?no={0}&st={1}", sysNo, Uri.EscapeDataString(searchTitle));
            }
        }
        #endregion

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
        /// 超时处理方式
        /// </summary>
        public OutTimeDeal HisOutTimeDeal
        {
            get
            {
                return (OutTimeDeal)this.GetValIntByKey(NodeAttr.OutTimeDeal);
            }
            set
            {
                this.SetValByKey(NodeAttr.OutTimeDeal, (int)value);
            }
        }
        /// <summary>
        /// 访问规则
        /// </summary>
        public ReturnRole HisReturnRole
        {
            get
            {
                return (ReturnRole)this.GetValIntByKey(NodeAttr.ReturnRole);
            }
            set
            {
                this.SetValByKey(NodeAttr.ReturnRole, (int)value);
            }
        }
        /// <summary>
        /// 访问规则
        /// </summary>
        public DeliveryWay HisDeliveryWay
        {
            get
            {
                return (DeliveryWay)this.GetValIntByKey(NodeAttr.DeliveryWay);
            }
            set
            {
                this.SetValByKey(NodeAttr.DeliveryWay, (int)value);
            }
        }
        /// <summary>
        /// 步骤
        /// </summary>
        public int Step
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.Step);
            }
            set
            {
                this.SetValByKey(NodeAttr.Step, value);
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.NodeID);
            }
            set
            {
                this.SetValByKey(NodeAttr.NodeID, value);
            }
        }
        /// <summary>
        /// 超时处理内容
        /// </summary>
        public string DoOutTime
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.DoOutTime);
            }
            set
            {
                this.SetValByKey(NodeAttr.DoOutTime, value);
            }
        }
        /// <summary>
        /// 超时处理条件
        /// </summary>
        public string DoOutTimeCond
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.DoOutTimeCond);
            }
            set
            {
                this.SetValByKey(NodeAttr.DoOutTimeCond, value);
            }
        }
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
        public string DeliveryParas
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
                Flow fl = new Flow(this.FK_Flow);
                if (BP.Web.WebUser.No == "admin")
                    uac.IsUpdate = true;
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

                Map map = new Map();
                //map 的基 础信息.
                map.PhysicsTable = "WF_Node";
                map.EnDesc = "节点";
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;

                #region  基础属性
                map.AddTBIntPK(NodeAttr.NodeID, 0, "节点ID", true, true);
                map.SetHelperUrl(NodeAttr.NodeID, "http://ccbpm.mydoc.io/?v=5404&t=17901");
                map.AddTBInt(NodeAttr.Step, 0, "步骤(无计算意义)", true, false);
                map.SetHelperUrl(NodeAttr.Step, "http://ccbpm.mydoc.io/?v=5404&t=17902");
                //map.SetHelperAlert(NodeAttr.Step, "它用于节点的排序，正确的设置步骤可以让流程容易读写."); //使用alert的方式显示帮助信息.
                map.AddTBString(NodeAttr.FK_Flow, null, "流程编号", false, false, 3, 3, 10, false, "http://ccbpm.mydoc.io/?v=5404&t=17023");
                map.AddTBString(NodeAttr.Name, null, "名称", true, true, 0, 100, 10, false, "http://ccbpm.mydoc.io/?v=5404&t=17903");
                map.AddTBString(NodeAttr.Tip, null, "操作提示", true, false, 0, 100, 10, false, "http://ccbpm.mydoc.io/?v=5404&t=18084");


                map.AddDDLSysEnum(NodeAttr.WhoExeIt, 0, "谁执行它",true, true, NodeAttr.WhoExeIt, "@0=操作员执行@1=机器执行@2=混合执行");
                map.SetHelperUrl(NodeAttr.WhoExeIt, "http://ccbpm.mydoc.io/?v=5404&t=17913");

                map.AddDDLSysEnum(NodeAttr.TurnToDeal, 0, "发送后转向",
                 true, true, NodeAttr.TurnToDeal, "@0=提示ccflow默认信息@1=提示指定信息@2=转向指定的url@3=按照条件转向");
                map.SetHelperUrl(NodeAttr.TurnToDeal, "http://ccbpm.mydoc.io/?v=5404&t=17914");
                map.AddTBString(NodeAttr.TurnToDealDoc, null, "转向处理内容", true, false, 0, 1000, 10, true, "http://ccbpm.mydoc.io/?v=5404&t=17914");
                map.AddDDLSysEnum(NodeAttr.ReadReceipts, 0, "已读回执", true, true, NodeAttr.ReadReceipts,
                    "@0=不回执@1=自动回执@2=由上一节点表单字段决定@3=由SDK开发者参数决定");
                map.SetHelperUrl(NodeAttr.ReadReceipts, "http://ccbpm.mydoc.io/?v=5404&t=17915");

                map.AddDDLSysEnum(NodeAttr.CondModel, 0, "方向条件控制规则", true, true, NodeAttr.CondModel,
                 "@0=由连接线条件控制@1=让用户手工选择");
                map.SetHelperUrl(NodeAttr.CondModel, "http://ccbpm.mydoc.io/?v=5404&t=17917"); //增加帮助

                // 撤销规则.
                map.AddDDLSysEnum(NodeAttr.CancelRole,(int)CancelRole.OnlyNextStep, "撤销规则", true, true,
                    NodeAttr.CancelRole,"@0=上一步可以撤销@1=不能撤销@2=上一步与开始节点可以撤销@3=指定的节点可以撤销");
                map.SetHelperUrl(NodeAttr.CancelRole, "http://ccbpm.mydoc.io/?v=5404&t=17919");

                // 节点工作批处理. edit by peng, 2014-01-24.    by huangzhimin 采用功能专题方式，移至左侧列表
                //map.AddDDLSysEnum(NodeAttr.BatchRole, (int)BatchRole.None, "工作批处理", true, true, NodeAttr.BatchRole, "@0=不可以批处理@1=批量审核@2=分组批量审核");
                //map.AddTBInt(NodeAttr.BatchListCount, 12, "批处理数量", true, false);
                ////map.SetHelperUrl(NodeAttr.BatchRole, this[SYS_CCFLOW, "节点工作批处理"]); //增加帮助
                //map.SetHelperUrl(NodeAttr.BatchRole, "http://ccbpm.mydoc.io/?v=5404&t=17920");
                //map.SetHelperUrl(NodeAttr.BatchListCount, "http://ccbpm.mydoc.io/?v=5404&t=17920");
                //map.AddTBString(NodeAttr.BatchParas, null, "批处理参数", true, false, 0, 300, 10, true);
                //map.SetHelperUrl(NodeAttr.BatchParas, "http://ccbpm.mydoc.io/?v=5404&t=17920");


                map.AddBoolean(NodeAttr.IsTask, true, "允许分配工作否?", true, true, false, "http://ccbpm.mydoc.io/?v=5404&t=17904");
                map.AddBoolean(NodeAttr.IsRM, true, "是否启用投递路径自动记忆功能?", true, true, false, "http://ccbpm.mydoc.io/?v=5404&t=17905");

                map.AddTBDateTime("DTFrom", "生命周期从", true, true);
                map.AddTBDateTime("DTTo", "生命周期到", true, true);

                map.AddBoolean(NodeAttr.IsBUnit, false, "是否是节点模版（业务单元）?", true, true, true, "http://ccbpm.mydoc.io/?v=5404&t=17904");

                
                map.AddTBString(NodeAttr.FocusField, null, "焦点字段", true, false, 0, 50, 10, true, "http://ccbpm.mydoc.io/?v=5404&t=17932");
                map.AddDDLSysEnum(NodeAttr.SaveModel, 0, "保存方式", true, true);
                map.SetHelperUrl(NodeAttr.SaveModel, "http://ccbpm.mydoc.io/?v=5404&t=17934");

                map.AddBoolean(NodeAttr.IsGuestNode, false, "是否是外部用户执行的节点(非组织结构人员参与处理工作的节点)?", true, true, true);
                #endregion  基础属性
                 
                #region 分合流子线程属性
                map.AddDDLSysEnum(NodeAttr.RunModel, 0, "节点类型",
                    true, true, NodeAttr.RunModel, "@0=普通@1=合流@2=分流@3=分合流@4=子线程");

                map.SetHelperUrl(NodeAttr.RunModel, "http://ccbpm.mydoc.io/?v=5404&t=17940"); //增加帮助.
        
                //子线程类型.
                map.AddDDLSysEnum(NodeAttr.SubThreadType, 0, "子线程类型", true, true, NodeAttr.SubThreadType, "@0=同表单@1=异表单");
                map.SetHelperUrl(NodeAttr.SubThreadType, "http://ccbpm.mydoc.io/?v=5404&t=17944"); //增加帮助


                map.AddTBDecimal(NodeAttr.PassRate, 100, "完成通过率", true, false);
                map.SetHelperUrl(NodeAttr.PassRate, "http://ccbpm.mydoc.io/?v=5404&t=17945"); //增加帮助.

                // 启动子线程参数 2013-01-04
                map.AddDDLSysEnum(NodeAttr.SubFlowStartWay, (int)SubFlowStartWay.None, "子线程启动方式", true, true,
                    NodeAttr.SubFlowStartWay, "@0=不启动@1=指定的字段启动@2=按明细表启动");
                map.AddTBString(NodeAttr.SubFlowStartParas, null, "启动参数", true, false, 0, 100, 10, true);
                map.SetHelperUrl(NodeAttr.SubFlowStartWay, "http://ccbpm.mydoc.io/?v=5404&t=17946"); //增加帮助

                //待办处理模式.
                map.AddDDLSysEnum(NodeAttr.TodolistModel, (int)TodolistModel.QiangBan, "待办处理模式", true, true, NodeAttr.TodolistModel,
                    "@0=抢办模式@1=协作模式@2=队列模式@3=共享模式");
                map.SetHelperUrl(NodeAttr.TodolistModel, "http://ccbpm.mydoc.io/?v=5404&t=17947"); //增加帮助.
                

                //发送阻塞模式.
                //map.AddDDLSysEnum(NodeAttr.BlockModel, (int)BlockModel.None, "发送阻塞模式", true, true, NodeAttr.BlockModel,
                //    "@0=不阻塞@1=当前节点有未完成的子流程时@2=按约定格式阻塞未完成子流程@3=按照SQL阻塞@4=按照表达式阻塞");
                //map.SetHelperUrl(NodeAttr.BlockModel, "http://ccbpm.mydoc.io/?v=5404&t=17948"); //增加帮助.

                //map.AddTBString(NodeAttr.BlockExp, null, "阻塞表达式", true, false, 0, 700, 10,true);
                //map.SetHelperUrl(NodeAttr.BlockExp, "http://ccbpm.mydoc.io/?v=5404&t=17948");

                //map.AddTBString(NodeAttr.BlockAlert, null, "被阻塞时提示信息", true, false, 0, 700, 10, true);
                //map.SetHelperUrl(NodeAttr.BlockAlert, "http://ccbpm.mydoc.io/?v=5404&t=17948");


                map.AddBoolean(NodeAttr.IsAllowRepeatEmps, false, "是否允许子线程接受人员重复(仅当分流点向子线程发送时有效)?", true, true, true);

                #endregion 分合流子线程属性

                #region 自动跳转规则
                map.AddBoolean(NodeAttr.AutoJumpRole0, false, "处理人就是发起人", true, true, false);
                map.SetHelperUrl(NodeAttr.AutoJumpRole0, "http://ccbpm.mydoc.io/?v=5404&t=17949"); //增加帮助

                map.AddBoolean(NodeAttr.AutoJumpRole1, false, "处理人已经出现过", true, true, false);
                map.AddBoolean(NodeAttr.AutoJumpRole2, false, "处理人与上一步相同", true, true, false);
                map.AddDDLSysEnum(NodeAttr.WhenNoWorker, 0, "找不到处理人处理规则",
       true, true, NodeAttr.WhenNoWorker, "@0=提示错误@1=自动转到下一步");
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
           NodeAttr.ThreadKillRole, "@0=不能删除@1=手工删除@2=自动删除",true);
                //map.SetHelperUrl(NodeAttr.ThreadKillRole, ""); //增加帮助
               

                map.AddTBString(BtnAttr.SubFlowLab, "子流程", "子流程按钮标签", true, false, 0, 50, 10);
                map.SetHelperUrl(BtnAttr.SubFlowLab, "http://ccbpm.mydoc.io/?v=5404&t=16262");
                map.AddDDLSysEnum(BtnAttr.SubFlowCtrlRole, 0, "控制规则", true, true, BtnAttr.SubFlowCtrlRole, "@0=无@1=不可以删除子流程@2=可以删除子流程");

                map.AddTBString(BtnAttr.JumpWayLab, "跳转", "跳转按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(NodeAttr.JumpWay, 0, "跳转规则", true, true, NodeAttr.JumpWay);
                map.AddTBString(NodeAttr.JumpToNodes, null, "可跳转的节点", true, false, 0, 200, 10, true);
                map.SetHelperUrl(NodeAttr.JumpWay, "http://ccbpm.mydoc.io/?v=5404&t=16261"); //增加帮助.

                map.AddTBString(BtnAttr.ReturnLab, "退回", "退回按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(NodeAttr.ReturnRole, 0,"退回规则",true, true, NodeAttr.ReturnRole);
              //  map.AddTBString(NodeAttr.ReturnToNodes, null, "可退回节点", true, false, 0, 200, 10, true);
                map.SetHelperUrl(NodeAttr.ReturnRole, "http://ccbpm.mydoc.io/?v=5404&t=16255"); //增加帮助.

                map.AddBoolean(NodeAttr.IsBackTracking, false, "是否可以原路返回(启用退回功能才有效)", true, true, false);
                map.AddTBString(BtnAttr.ReturnField, "", "退回信息填写字段", true, false, 0, 50, 10);
                map.SetHelperUrl(NodeAttr.IsBackTracking, "http://ccbpm.mydoc.io/?v=5404&t=16255"); //增加帮助.

                map.AddTBString(BtnAttr.CCLab, "抄送", "抄送按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(NodeAttr.CCRole, 0, "抄送规则", true, true, NodeAttr.CCRole,
                    "@@0=不能抄送@1=手工抄送@2=自动抄送@3=手工与自动@4=按表单SysCCEmps字段计算@5=在发送前打开抄送窗口");
                map.SetHelperUrl(BtnAttr.CCLab, "http://ccbpm.mydoc.io/?v=5404&t=16259"); //增加帮助.

                // add 2014-04-05.
                map.AddDDLSysEnum(NodeAttr.CCWriteTo, 0, "抄送写入规则",
             true, true, NodeAttr.CCWriteTo, "@0=写入抄送列表@1=写入待办@2=写入待办与抄送列表", true);
                map.SetHelperUrl(NodeAttr.CCWriteTo, "http://ccbpm.mydoc.io/?v=5404&t=17976"); //增加帮助

                map.AddTBString(BtnAttr.ShiftLab, "移交", "移交按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ShiftEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.ShiftLab, "http://ccbpm.mydoc.io/?v=5404&t=16257"); //增加帮助.note:none

                map.AddTBString(BtnAttr.DelLab, "删除", "删除按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.DelEnable, 0, "删除规则", true, true, BtnAttr.DelEnable);
                map.SetHelperUrl(BtnAttr.DelLab, "http://ccbpm.mydoc.io/?v=5404&t=17992"); //增加帮助.

                map.AddTBString(BtnAttr.EndFlowLab, "结束流程", "结束流程按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.EndFlowEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.EndFlowLab, "http://ccbpm.mydoc.io/?v=5404&t=17989"); //增加帮助

                map.AddTBString(BtnAttr.PrintDocLab, "打印单据", "打印单据按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.PrintDocEnable, 0, "打印方式", true,
                    true, BtnAttr.PrintDocEnable, "@0=不打印@1=打印网页@2=打印RTF模板@3=打印Word模版");
                map.SetHelperUrl(BtnAttr.PrintDocEnable, "http://ccbpm.mydoc.io/?v=5404&t=17979"); //增加帮助

                // map.AddBoolean(BtnAttr.PrintDocEnable, false, "是否启用", true, true);
                //map.AddTBString(BtnAttr.AthLab, "附件", "附件按钮标签", true, false, 0, 50, 10);
                //map.AddDDLSysEnum(NodeAttr.FJOpen, 0, this.ToE("FJOpen", "附件权限"), true, true, 
                //    NodeAttr.FJOpen, "@0=关闭附件@1=操作员@2=工作ID@3=流程ID");

                map.AddTBString(BtnAttr.TrackLab, "轨迹", "轨迹按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.TrackEnable, true, "是否启用", true, true);
                //map.SetHelperUrl(BtnAttr.TrackLab, this[SYS_CCFLOW, "轨迹"]); //增加帮助
                map.SetHelperUrl(BtnAttr.TrackLab, "http://ccbpm.mydoc.io/?v=5404&t=24369");

                map.AddTBString(BtnAttr.HungLab, "挂起", "挂起按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.HungEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.HungLab, "http://ccbpm.mydoc.io/?v=5404&t=16267"); //增加帮助.

                map.AddTBString(BtnAttr.SelectAccepterLab, "接受人", "接受人按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.SelectAccepterEnable, 0, "工作方式",
          true, true, BtnAttr.SelectAccepterEnable);
                map.SetHelperUrl(BtnAttr.SelectAccepterLab, "http://ccbpm.mydoc.io/?v=5404&t=16256"); //增加帮助


                map.AddTBString(BtnAttr.SearchLab, "查询", "查询按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.SearchEnable, false, "是否启用", true, true);
                //map.SetHelperUrl(BtnAttr.SearchLab, this[SYS_CCFLOW, "查询"]); //增加帮助
                map.SetHelperUrl(BtnAttr.SearchLab, "http://ccbpm.mydoc.io/?v=5404&t=24373");

                map.AddTBString(BtnAttr.WorkCheckLab, "审核", "审核按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.WorkCheckEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.BatchLab, "批处理", "批处理按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.BatchEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.BatchLab, "http://ccbpm.mydoc.io/?v=5404&t=17920"); //增加帮助

                map.AddTBString(BtnAttr.AskforLab, "加签", "加签按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.AskforEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.AskforLab, "http://ccbpm.mydoc.io/?v=5404&t=16258");

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
                map.AddBoolean(BtnAttr.PRIEnable, false, "是否启用", true, true);

                // add by 周朋 2015-08-06. 节点时限.
                map.AddTBString(BtnAttr.CHLab, "节点时限", "节点时限", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.CHEnable, false, "是否启用", true, true);


                // add by 周朋 2015-12-24. 节点时限.
                map.AddTBString(BtnAttr.FocusLab, "关注", "关注", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.FocusEnable, true, "是否启用", true, true);

                //map.AddBoolean(BtnAttr.SelectAccepterEnable, false, "是否启用", true, true);
                #endregion  功能按钮状态

                #region 考核属性
               // // 考核属性
               // map.AddTBFloat(NodeAttr.TSpanDay, 0, "限期(天)", true, false); //"限期(天)".
               // map.AddTBFloat(NodeAttr.TSpanHour, 8, "小时", true, false); //"限期(天)".

               // map.AddTBFloat(NodeAttr.WarningDay, 0, "工作预警(天)", true, false);    // "警告期限(0不警告)"
               // map.AddTBFloat(NodeAttr.WarningHour, 0, "工作预警(小时)", true, false); // "警告期限(0不警告)"
               // map.SetHelperUrl(NodeAttr.WarningHour, "http://ccbpm.mydoc.io/?v=5404&t=17999");
               // map.AddTBFloat(NodeAttr.TCent, 1, "扣分(每延期1小时)", true, false); //"扣分(每延期1天扣)"
                
               // //map.AddTBFloat(NodeAttr.MaxDeductCent, 0, "最高扣分", true, false);   //"最高扣分"
               // //map.AddTBFloat(NodeAttr.SwinkCent, float.Parse("0.1"), "工作得分", true, false); //"工作得分".
               // //map.AddTBString(NodeAttr.FK_Flows, null, "flow", false, false, 0, 100, 10);

               // map.AddDDLSysEnum(NodeAttr.CHWay, 0, "考核方式", true, true, NodeAttr.CHWay,"@0=不考核@1=按时效@2=按工作量");

               //// map.AddTBFloat(NodeAttr.Workload, 0, "工作量(单位:小时)", true, false);

               // // 是否质量考核点？
               // map.AddBoolean(NodeAttr.IsEval, false, "是否质量考核点", true, true, true);


               // // 去掉了, 移动到 主题功能界面处理了.
               // map.AddDDLSysEnum(NodeAttr.OutTimeDeal, 0, "超时处理", true, true, NodeAttr.OutTimeDeal,
               // "@0=不处理@1=自动向下运动@2=自动跳转指定的节点@3=自动移交给指定的人员@4=向指定的人员发消息@5=删除流程@6=执行SQL");
               // map.AddTBString(NodeAttr.DoOutTime, null, "处理内容", true, false, 0, 300, 10, true);
               // map.AddTBString(NodeAttr.DoOutTimeCond, null, "执行超时条件", true, false, 0, 100, 10, true);
               // map.SetHelperUrl(NodeAttr.OutTimeDeal, "http://ccbpm.mydoc.io/?v=5404&t=18001");
                #endregion 考核属性

                #region 审核组件属性, 此处变更了BP.Sys.FrmWorkCheck 也要变更.
                // BP.Sys.FrmWorkCheck
                map.AddDDLSysEnum(FrmWorkCheckAttr.FWCSta, (int)FrmWorkCheckSta.Disable, "审核组件状态",
                    true, true, FrmWorkCheckAttr.FWCSta, "@0=禁用@1=启用@2=只读");
                map.SetHelperUrl(FrmWorkCheckAttr.FWCSta, "http://ccbpm.mydoc.io/?v=5404&t=17936");
                map.AddDDLSysEnum(FrmWorkCheckAttr.FWCShowModel, (int)FrmWorkShowModel.Free, "显示方式",
                    true, true, FrmWorkCheckAttr.FWCShowModel, "@0=表格方式@1=自由模式"); //此属性暂时没有用.
                map.SetHelperUrl(FrmWorkCheckAttr.FWCShowModel, "http://ccbpm.mydoc.io/?v=5404&t=17937");
                map.AddDDLSysEnum(FrmWorkCheckAttr.FWCType, (int)FWCType.Check, "工作方式", true, true,
                    FrmWorkCheckAttr.FWCType, "@0=审核组件@1=日志组件@2=周报组件@3=月报组件");
                map.SetHelperUrl(FrmWorkCheckAttr.FWCType, "http://ccbpm.mydoc.io/?v=5404&t=17938");
                // add by stone 2015-03-19. 如果为空，就去节点名称显示到步骤里.
                map.AddTBString(FrmWorkCheckAttr.FWCNodeName, null, "节点意见名称", true, false, 0, 100, 10);

                map.AddDDLSysEnum(FrmWorkCheckAttr.FWCAth, (int)FWCAth.None, "附件上传", true, true,
                   FrmWorkCheckAttr.FWCAth, "@0=不启用@1=多附件@2=单附件(暂不支持)@3=图片附件(暂不支持)");
                map.SetHelperAlert(FrmWorkCheckAttr.FWCAth,
                    "在审核期间，是否启用上传附件？启用什么样的附件？注意：附件的属性在节点表单里配置。"); //使用alert的方式显示帮助信息.

                map.AddBoolean(FrmWorkCheckAttr.FWCTrackEnable, true, "轨迹图是否显示？", true, true, false);
                map.AddBoolean(FrmWorkCheckAttr.FWCListEnable, true, "历史审核信息是否显示？(否,仅出现意见框)", true, true, true);
                map.AddBoolean(FrmWorkCheckAttr.FWCIsShowAllStep, false, "在轨迹表里是否显示所有的步骤？", true, true,true);
                map.AddBoolean(FrmWorkCheckAttr.SigantureEnabel, false, "使用图片签名(在信息填写底部显示文字Or图片签名)？", true, true, true);
                map.AddBoolean(FrmWorkCheckAttr.FWCIsFullInfo, true, "如果用户未审核是否按照默认意见填充？", true, true, true);


                map.AddTBString(FrmWorkCheckAttr.FWCOpLabel, "审核", "操作名词(审核/审阅/批示)", true, false, 0, 50, 10);
                map.AddTBString(FrmWorkCheckAttr.FWCDefInfo, "同意", "默认审核信息", true, false, 0, 50, 10);

                //map.AddTBFloat(FrmWorkCheckAttr.FWC_X, 5, "位置X", true, false);
                //map.AddTBFloat(FrmWorkCheckAttr.FWC_Y, 5, "位置Y", true, false);
                
                // 高度与宽度, 如果是自由表单就不要变化该属性.
                map.AddTBFloat(FrmWorkCheckAttr.FWC_H, 300, "高度", true, false);
                map.SetHelperAlert(FrmWorkCheckAttr.FWC_H, "如果是自由表单就不要变化该属性,为0，则标识为100%,应用的组件模式."); //增加帮助
                map.AddTBFloat(FrmWorkCheckAttr.FWC_W, 400, "宽度", true, false);
                map.SetHelperAlert(FrmWorkCheckAttr.FWC_W, "如果是自由表单就不要变化该属性,为0，则标识为100%,应用的组件模式."); //增加帮助
                
                map.AddTBStringDoc(FrmWorkCheckAttr.FWCFields, null, "审批格式化字段", true, false,true);
                #endregion 审核组件属性.

                #region 公文按钮 del by zhoupeng. 按照新昌的标准修改.
                //map.AddTBString(BtnAttr.OfficeOpenLab, "打开本地", "打开本地标签", true, false, 0, 50, 10);
                //map.SetHelperUrl(BtnAttr.OfficeOpenLab, "http://ccbpm.mydoc.io/?v=5404&t=17998");
                //map.AddBoolean(BtnAttr.OfficeOpenEnable, false, "是否启用", true, true);

                //map.AddTBString(BtnAttr.OfficeOpenTemplateLab, "打开模板", "打开模板标签", true, false, 0, 50, 10);
                //map.SetHelperUrl(BtnAttr.OfficeOpenTemplateLab, "http://ccbpm.mydoc.io/?v=5404&t=17998");
                //map.AddBoolean(BtnAttr.OfficeOpenTemplateEnable, false, "是否启用", true, true);

                //map.AddTBString(BtnAttr.OfficeSaveLab, "保存", "保存标签", true, false, 0, 50, 10);
                //map.SetHelperUrl(BtnAttr.OfficeSaveLab, "http://ccbpm.mydoc.io/?v=5404&t=17998");
                //map.AddBoolean(BtnAttr.OfficeSaveEnable, true, "是否启用", true, true);

                //map.AddTBString(BtnAttr.OfficeAcceptLab, "接受修订", "接受修订标签", true, false, 0, 50, 10);
                //map.SetHelperUrl(BtnAttr.OfficeAcceptLab, "http://ccbpm.mydoc.io/?v=5404&t=17998");
                //map.AddBoolean(BtnAttr.OfficeAcceptEnable, false, "是否启用", true, true);

                //map.AddTBString(BtnAttr.OfficeRefuseLab, "拒绝修订", "拒绝修订标签", true, false, 0, 50, 10);
                //map.SetHelperUrl(BtnAttr.OfficeRefuseLab, "http://ccbpm.mydoc.io/?v=5404&t=17998");
                //map.AddBoolean(BtnAttr.OfficeRefuseEnable, false, "是否启用", true, true);

                //map.AddTBString(BtnAttr.OfficeOverLab, "套红", "套红按钮标签", true, false, 0, 50, 10);
                //map.SetHelperUrl(BtnAttr.OfficeOverLab, "http://ccbpm.mydoc.io/?v=5404&t=17998");
                //map.AddBoolean(BtnAttr.OfficeOverEnable, false, "是否启用", true, true);
               

                //map.AddTBString(BtnAttr.OfficePrintLab, "打印", "打印按钮标签", true, false, 0, 50, 10);
                //map.SetHelperUrl(BtnAttr.OfficePrintLab, "http://ccbpm.mydoc.io/?v=5404&t=17998");
                //map.AddBoolean(BtnAttr.OfficePrintEnable, false, "是否启用", true, true);

                //map.AddTBString(BtnAttr.OfficeSealLab, "签章", "签章按钮标签", true, false, 0, 50, 10);
                //map.SetHelperUrl(BtnAttr.OfficeSealLab, "http://ccbpm.mydoc.io/?v=5404&t=17998");
                //map.AddBoolean(BtnAttr.OfficeSealEnable, false, "是否启用", true, true);

                //map.AddTBString(BtnAttr.OfficeDownLab, "下载", "下载按钮标签", true, false, 0, 50, 10);
                //map.SetHelperUrl(BtnAttr.OfficeDownLab, "http://ccbpm.mydoc.io/?v=5404&t=17998");
                //map.AddBoolean(BtnAttr.OfficeDownEnable, false, "是否启用", true, true);

                //map.AddTBString(BtnAttr.OfficeInsertFlowLab, "插入流程", "插入流程标签", true, false, 0, 50, 10);
                //map.SetHelperUrl(BtnAttr.OfficeInsertFlowLab, "http://ccbpm.mydoc.io/?v=5404&t=17998");
                //map.AddBoolean(BtnAttr.OfficeInsertFlowEnable, false, "是否启用", true, true);

                //map.AddBoolean(BtnAttr.OfficeNodeInfo, false, "是否记录节点信息", true, true);
                //map.AddBoolean(BtnAttr.OfficeReSavePDF, false, "是否该自动保存为PDF", true, true);


                //map.AddBoolean(BtnAttr.OfficeIsMarks, true, "是否进入留痕模式", true, true);
                //map.AddTBString(BtnAttr.OfficeTemplate, "", "指定文档模板", true, false, 0, 100, 10);
                //map.SetHelperUrl(BtnAttr.OfficeTemplate, "http://ccbpm.mydoc.io/?v=5404&t=17998");

                //map.AddBoolean(BtnAttr.OfficeMarksEnable, true, "是否查看用户留痕", true, true, true);

                //map.AddBoolean(BtnAttr.OfficeIsParent, true, "是否使用父流程的文档", true, true);

                //map.AddBoolean(BtnAttr.OfficeTHEnable, false, "是否自动套红", true, true);
                //map.AddTBString(BtnAttr.OfficeTHTemplate, "", "自动套红模板", true, false, 0, 200, 10);
                //map.SetHelperUrl(BtnAttr.OfficeTHTemplate, "http://ccbpm.mydoc.io/?v=5404&t=17998");

                //if (Glo.IsEnableZhiDu)
                //{
                //    map.AddTBString(BtnAttr.OfficeFengXianTemplate, "", "风险点模板", true, false, 0, 100, 10);
                //    map.AddTBString(BtnAttr.OfficeInsertFengXian, "插入风险点", "插入风险点标签", true, false, 0, 50, 10);
                //    map.AddBoolean(BtnAttr.OfficeInsertFengXianEnabel, false, "是否启用", true, true);
                //}

                //map.AddTBString(BtnAttr.OfficeDownLab, "下载", "下载按钮标签", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.OfficeIsDown, false, "是否启用", true, true);
                #endregion

                #region 移动设置.
                map.AddDDLSysEnum(NodeAttr.MPhone_WorkModel, 0, "手机工作模式", true, true, NodeAttr.MPhone_WorkModel, "@0=原生态@1=浏览器@2=禁用");
                map.AddDDLSysEnum(NodeAttr.MPhone_SrcModel, 0, "手机屏幕模式", true, true, NodeAttr.MPhone_SrcModel, "@0=强制横屏@1=强制竖屏@2=由重力感应决定");

                map.AddDDLSysEnum(NodeAttr.MPad_WorkModel, 0, "平板工作模式", true, true, NodeAttr.MPad_WorkModel, "@0=原生态@1=浏览器@2=禁用");
                map.AddDDLSysEnum(NodeAttr.MPad_SrcModel, 0, "平板屏幕模式", true, true, NodeAttr.MPad_SrcModel, "@0=强制横屏@1=强制竖屏@2=由重力感应决定");
                map.SetHelperUrl(NodeAttr.MPhone_WorkModel, "http://bbs.ccflow.org/showtopic-2866.aspx");
                #endregion 移动设置.

                //节点工具栏, 主从表映射.
                map.AddDtl(new NodeToolbars(), NodeToolbarAttr.FK_Node);

                #region 基础功能.
                RefMethod rm = null;

                rm = new RefMethod();
                rm.Title = "接收人规则";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Admin/CCFormDesigner/Img/Menu/Sender.png";
                rm.ClassMethodName = this.ToString() + ".DoAccepterRoleNew";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "抄送人规则";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Admin/CCFormDesigner/Img/Menu/CC.png";
                rm.ClassMethodName = this.ToString() + ".DoCCer";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "表单方案";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Admin/CCFormDesigner/Img/Form.png";

                rm.ClassMethodName = this.ToString() + ".DoSheet";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                 rm = new RefMethod();
                rm.Title = "节点事件"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoAction";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Event.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "节点消息"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoMessage";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Event.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

             

                rm = new RefMethod();
                rm.Title = "父子流程";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/Menu/SubFlows.png";
                rm.ClassMethodName = this.ToString() + ".DoSubFlow";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "手机表单字段顺序";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Mobile.png";
                rm.ClassMethodName = this.ToString() + ".DoSortingMapAttrs";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "流程完成条件"; // "流程完成条件";
                rm.ClassMethodName = this.ToString() + ".DoCond";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/Menu/Cond.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "发送后转向"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoTurnToDeal";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Msg.gif";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "发送阻塞规则"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoBlockModel";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Msg.gif";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

              


                rm = new RefMethod();
                rm.Title = "消息收听"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoListen";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Msg.gif";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                if (Glo.IsEnableZhiDu)
                {
                    rm = new RefMethod();
                    rm.Title = "对应制度章节"; // "个性化接受人窗口";
                    rm.ClassMethodName = this.ToString() + ".DoZhiDu";
                    rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                    map.AddRefMethod(rm);

                    rm = new RefMethod();
                    rm.Title = "风险点"; // "个性化接受人窗口";
                    rm.ClassMethodName = this.ToString() + ".DoFengXianDian";
                    rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                    map.AddRefMethod(rm);

                    rm = new RefMethod();
                    rm.Title = "岗位职责"; // "个性化接受人窗口";
                    rm.ClassMethodName = this.ToString() + ".DoGangWeiZhiZe";
                    rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                    map.AddRefMethod(rm);
                }

                #endregion 基础功能.

                #region 字段相关功能（不显示在菜单里）
                rm = new RefMethod();
                rm.Title = "可退回的节点(当退回规则设置可退回指定的节点时,该设置有效.)"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoCanReturnNodes";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
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
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;

                //设置相关字段.
                rm.RefAttrKey = NodeAttr.CancelRole;
                rm.RefAttrLinkLabel = "";
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "发送成功转向条件"; // "转向条件";
                rm.ClassMethodName = this.ToString() + ".DoTurn";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/Menu/Cond.png";

                //设置相关字段.
                rm.RefAttrKey = NodeAttr.TurnToDealDoc;
                rm.RefAttrLinkLabel = "";
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "绑定rtf打印格式模版(当打印方式为打印RTF格式模版时,该设置有效)"; //"单据&单据";
                rm.ClassMethodName = this.ToString() + ".DoBill";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/FileType/doc.gif";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;

                //设置相关字段.
                rm.RefAttrKey = NodeAttr.PrintDocEnable;
                rm.RefAttrLinkLabel = "";
                rm.Target = "_blank";
                map.AddRefMethod(rm);
                if (BP.Sys.SystemConfig.CustomerNo == "HCBD")
                {
                    /* 为海成邦达设置的个性化需求. */
                    rm = new RefMethod();
                    rm.Title = "DXReport设置";
                    rm.ClassMethodName = this.ToString() + ".DXReport";
                    rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/FileType/doc.gif";
                    map.AddRefMethod(rm);
                }

                rm = new RefMethod();
                rm.Title = "设置自动抄送规则(当节点为自动抄送时,该设置有效.)"; // "抄送规则";
                rm.ClassMethodName = this.ToString() + ".DoCCRole";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                //设置相关字段.
                rm.RefAttrKey = NodeAttr.CCRole;
                rm.RefAttrLinkLabel = "自动抄送设置";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);
                #endregion 字段相关功能（不显示在菜单里）

                #region 考核.
                rm = new RefMethod();
                rm.Title = "设置考核规则";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Admin/CCFormDesigner/Img/CH.png";
                rm.ClassMethodName = this.ToString() + ".DoCHRole";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "考核规则";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "超时处理规则";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Admin/CCFormDesigner/Img/CH.png";
                rm.ClassMethodName = this.ToString() + ".DoCHOvertimeRole";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "考核规则";
                map.AddRefMethod(rm);
                #endregion 考核.

                #region 实验中的功能
                rm = new RefMethod();
                rm.Title = "批量设置节点属性";
                rm.Icon = Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/Node.png";
                rm.ClassMethodName = this.ToString() + ".DoNodeAttrs()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "实验中的功能";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设置独立表单树权限";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoNodeFormTree";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "实验中的功能";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "工作批处理规则";
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoBatchStartFields()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "实验中的功能";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "节点运行模式(开发中)"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoRunModel";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "实验中的功能";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "特别字段特殊用户权限";
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoSpecFieldsSpecUsers()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "实验中的功能";
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
            return Glo.CCFlowAppPath + "WF/Admin/AttrNode/CHRole.aspx?FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// 超时处理规则
        /// </summary>
        /// <returns></returns>
        public string DoCHOvertimeRole()
        {
            return Glo.CCFlowAppPath + "WF/Admin/AttrNode/CHOvertimeRole.aspx?FK_Node=" + this.NodeID;
        }
        #endregion 考核规则.

        #region 基础设置.
        /// <summary>
        /// 批处理规则
        /// </summary>
        /// <returns></returns>
        public string DoBatchStartFields()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/AttrNode/BatchStartFields.aspx?s=d34&FK_Flow=" + this.FK_Flow + "&FK_Node="+this.NodeID;
        }
        /// <summary>
        /// 批量修改节点属性
        /// </summary>
        /// <returns></returns>
        public string DoNodeAttrs()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/AttrFlow/NodeAttrs.aspx?NodeID=0&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 表单方案
        /// </summary>
        /// <returns></returns>
        public string DoSheet()
        {
            return Glo.CCFlowAppPath + "WF/Admin/AttrNode/NodeFromWorkModel.aspx?FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// 父子流程
        /// </summary>
        /// <returns></returns>
        public string DoSubFlow()
        {
            return Glo.CCFlowAppPath + "WF/Admin/AttrNode/SubFlows.aspx?FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// 接受人规则
        /// </summary>
        /// <returns></returns>
        public string DoAccepterRoleNew()
        {
            return Glo.CCFlowAppPath + "WF/Admin/FindWorker/NodeAccepterRole.aspx?FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// 发送阻塞规则
        /// </summary>
        /// <returns></returns>
        public string DoBlockModel()
        {
            return Glo.CCFlowAppPath + "WF/Admin/AttrNode/BlockModel.aspx?FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// 发送后转向规则
        /// </summary>
        /// <returns></returns>
        public string DoTurnToDeal()
        {
            return Glo.CCFlowAppPath + "WF/Admin/AttrNode/TurnToDeal.aspx?FK_Node=" + this.NodeID;
        }
        
        /// <summary>
        /// 抄送人规则
        /// </summary>
        /// <returns></returns>
        public string DoCCer()
        {
            return Glo.CCFlowAppPath + "WF/Admin/FindWorker/NodeCCRole.aspx?FK_Node=" + this.NodeID;
        }
        #endregion 

        /// <summary>
        /// 特别用户特殊字段权限.
        /// </summary>
        /// <returns></returns>
        public string DoSpecFieldsSpecUsers()
        {
            return Glo.CCFlowAppPath + "WF/Admin/AttrNode/SepcFiledsSepcUsers.aspx?FK_Flow=" + this.FK_Flow + "&FK_MapData=ND" +
                   this.NodeID + "&FK_Node="+this.NodeID+"&t=" + DataType.CurrentDataTime;
        }

        /// <summary>
        /// 节点运行模式.
        /// </summary>
        /// <returns></returns>
        public string DoRunModel()
        {
            return Glo.CCFlowAppPath + "WF/Admin/AttrNode/NodeRunModel.aspx?FK_Flow=" + this.FK_Flow + "&FK_MapData=ND" +
                   this.NodeID + "&t=" + DataType.CurrentDataTime;
        }
        /// <summary>
        /// 排序字段顺序
        /// </summary>
        /// <returns></returns>
        public string DoSortingMapAttrs()
        {
            return Glo.CCFlowAppPath + "WF/Admin/AttrNode/SortingMapAttrs.aspx?FK_Flow=" + this.FK_Flow + "&FK_MapData=ND" +
                   this.NodeID + "&t=" + DataType.CurrentDataTime;
        }
        /// <summary>
        /// 集团部门树
        /// </summary>
        /// <returns></returns>
        public string DoDepts()
        {
            PubClass.WinOpen(Glo.CCFlowAppPath + "WF/Comm/Port/DeptTree.aspx?s=d34&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&RefNo=" + DataType.CurrentDataTime, 500, 550);
            return null;
        }
        /// <summary>
        /// 设置独立表单树权限
        /// </summary>
        /// <returns></returns>
        public string DoNodeFormTree()
        {
            return Glo.CCFlowAppPath + "WF/Admin/FlowFormTree.aspx?s=d34&FK_Flow=" + this.FK_Flow + "&FK_Node=" +
                   this.NodeID + "&RefNo=" + DataType.CurrentDataTime;
        }
        /// <summary>
        /// 制度
        /// </summary>
        /// <returns></returns>
        public string DoZhiDu()
        {
            PubClass.WinOpen(Glo.CCFlowAppPath + "ZhiDu/NodeZhiDuDtl.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow, "制度", "Bill", 700, 400, 200, 300);
            return null;
        }
        /// <summary>
        /// 风险点
        /// </summary>
        /// <returns></returns>
        public string DoFengXianDian()
        {
            PubClass.WinOpen(Glo.CCFlowAppPath + "ZhiDu/NodeFengXianDian.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow, "制度", "Bill", 700, 400, 200, 300);
            return null;
        }
        /// <summary>
        /// 接收人
        /// </summary>
        /// <returns></returns>
        public string DoSelectAccepter()
        {
            BP.WF.Node nd = new BP.WF.Node(this.NodeID);
            if (nd.HisDeliveryWay != DeliveryWay.ByCCFlowBPM)
                return Glo.CCFlowAppPath + "WF/Admin/FindWorker/List.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
            return Glo.CCFlowAppPath + "WF/Admin/FindWorker/NodeAccepterRole.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 找人规则
        /// </summary>
        /// <returns></returns>
        public string DoAccepterRole()
        {
            BP.WF.Node nd = new BP.WF.Node(this.NodeID);

            if (nd.HisDeliveryWay != DeliveryWay.ByCCFlowBPM)
                return Glo.CCFlowAppPath + "WF/Admin/FindWorker/List.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow; 
            //    return "节点访问规则您没有设置按照bpm模式，所以您能执行该操作。要想执行该操作请选择节点属性中节点规则访问然后选择按照bpm模式计算，点保存按钮。";

            return Glo.CCFlowAppPath + "WF/Admin/FindWorker/List.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow; 
         //   return null;
        }
        public string DoTurn()
        {
            return Glo.CCFlowAppPath + "WF/Admin/TurnTo.aspx?FK_Node=" + this.NodeID;
            //, "节点完成转向处理", "FrmTurn", 800, 500, 200, 300);
            //BP.WF.Node nd = new BP.WF.Node(this.NodeID);
            //return nd.DoTurn();
        }
        /// <summary>
        /// 抄送规则
        /// </summary>
        /// <returns></returns>
        public string DoCCRole()
        {
            return Glo.CCFlowAppPath + "WF/Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.CC&PK=" + this.NodeID; 
            //PubClass.WinOpen("./RefFunc/UIEn.aspx?EnName=BP.WF.CC&PK=" + this.NodeID, "抄送规则", "Bill", 800, 500, 200, 300);
            //return null;
        }
        /// <summary>
        /// 个性化接受人窗口
        /// </summary>
        /// <returns></returns>
        public string DoAccepter()
        {
            return Glo.CCFlowAppPath + "WF/Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.Selector&PK=" + this.NodeID;
        }
        /// <summary>
        /// 可触发的子流程
        /// </summary>
        /// <returns></returns>
        public string DoActiveFlows()
        {
            return Glo.CCFlowAppPath + "WF/Admin/ConditionSubFlow.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 退回节点
        /// </summary>
        /// <returns></returns>
        public string DoCanReturnNodes()
        {
            return Glo.CCFlowAppPath + "WF/Admin/CanReturnNodes.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 撤销发送的节点
        /// </summary>
        /// <returns></returns>
        public string DoCanCancelNodes()
        {
            return Glo.CCFlowAppPath + "WF/Admin/CanCancelNodes.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow; 
        }
        /// <summary>
        /// DXReport
        /// </summary>
        /// <returns></returns>
        public string DXReport()
        {
            return Glo.CCFlowAppPath + "WF/Admin/DXReport.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        public string DoPush2Current()
        {
            return Glo.CCFlowAppPath + "WF/Admin/Listen.aspx?CondType=0&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&FK_Attr=&DirType=&ToNodeID=";
        }
        public string DoPush2Spec()
        {
            return Glo.CCFlowAppPath + "WF/Admin/Listen.aspx?CondType=0&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&FK_Attr=&DirType=&ToNodeID=";
        }
        /// <summary>
        /// 执行消息收听
        /// </summary>
        /// <returns></returns>
        public string DoListen()
        {
            return Glo.CCFlowAppPath + "WF/Admin/Listen.aspx?CondType=0&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&FK_Attr=&DirType=&ToNodeID=";
        }
        public string DoFeatureSet()
        {
            return Glo.CCFlowAppPath + "WF/Admin/FeatureSetUI.aspx?CondType=0&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&FK_Attr=&DirType=&ToNodeID=";
        }
        public string DoShowSheets()
        {
            return Glo.CCFlowAppPath + "WF/Admin/ShowSheets.aspx?CondType=0&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&FK_Attr=&DirType=&ToNodeID=";
        }
        public string DoCond()
        {
            return Glo.CCFlowAppPath + "WF/Admin/Condition.aspx?CondType=" + (int)CondType.Flow + "&FK_Flow=" + this.FK_Flow + "&FK_MainNode=" + this.NodeID + "&FK_Node=" + this.NodeID + "&FK_Attr=&DirType=&ToNodeID=" + this.NodeID;
        }
        /// <summary>
        /// 设计傻瓜表单
        /// </summary>
        /// <returns></returns>
        public string DoFormCol4()
        {
            return Glo.CCFlowAppPath + "WF/MapDef/MapDef.aspx?PK=ND" + this.NodeID;
        }
        /// <summary>
        /// 设计自由表单
        /// </summary>
        /// <returns></returns>
        public string DoFormFree()
        {
            return Glo.CCFlowAppPath + "WF/MapDef/CCForm/Frm.aspx?FK_MapData=ND" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 绑定独立表单
        /// </summary>
        /// <returns></returns>
        public string DoFormTree()
        {
            return Glo.CCFlowAppPath + "WF/Admin/BindFrms.aspx?ShowType=FlowFrms&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&Lang=CH";
        }
        
        public string DoMapData()
        {
            int i = this.GetValIntByKey(NodeAttr.FormType);

            // 类型.
            NodeFormType type = (NodeFormType)i;
            switch (type)
            {
                case NodeFormType.FreeForm:
                    PubClass.WinOpen(Glo.CCFlowAppPath + "WF/MapDef/CCForm/Frm.aspx?FK_MapData=ND" + this.NodeID + "&FK_Flow=" + this.FK_Flow, "设计表单", "sheet", 1024, 768, 0, 0);
                    break;
                default:
                case NodeFormType.FixForm:
                    PubClass.WinOpen(Glo.CCFlowAppPath + "WF/MapDef/MapDef.aspx?PK=ND" + this.NodeID, "设计表单", "sheet", 800, 500, 210, 300);
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
            return Glo.CCFlowAppPath + "WF/Admin/AttrNode/PushMessage.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// 事件
        /// </summary>
        /// <returns></returns>
        public string DoAction()
        {
            return Glo.CCFlowAppPath + "WF/Admin/Action.aspx?NodeID=" + this.NodeID + "&FK_Flow=" + this.FK_Flow + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// 单据打印
        /// </summary>
        /// <returns></returns>
        public string DoBill()
        {
            return Glo.CCFlowAppPath + "WF/Admin/Bill.aspx?NodeID=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 设置
        /// </summary>
        /// <returns></returns>
        public string DoFAppSet()
        {
            return Glo.CCFlowAppPath + "WF/Admin/FAppSet.aspx?NodeID=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        
        protected override bool beforeUpdate()
        {
            //更新流程版本
            Flow.UpdateVer(this.FK_Flow);

            //把工具栏的配置放入 sys_mapdata里.
            ToolbarExcel te = new ToolbarExcel("ND" + this.NodeID);
            te.Copy(this);
            try
            {
                te.Update();
            }
            catch
            {
            }
           
            #region  检查考核逾期处理的设置的完整性.
            //string doOutTime = this.GetValStrByKey(NodeAttr.DoOutTime);
            //switch (this.HisOutTimeDeal)
            //{
            //    case OutTimeDeal.AutoJumpToSpecNode:
            //        string[] jumps = doOutTime.Split(',');
            //        if (jumps.Length  > 2)
            //        {
            //            string msg = "自动跳转到相应节点,配置的内容不正确,格式应该为: Node,EmpNo , 比如: 101,zhoupeng  现在设置的格式为:" + doOutTime;
            //            throw new Exception(msg);
            //        }
            //        break;
            //    case OutTimeDeal.AutoShiftToSpecUser:
            //    case OutTimeDeal.RunSQL:
            //    case OutTimeDeal.SendMsgToSpecUser:
            //        if (string.IsNullOrEmpty(doOutTime) == false)
            //            throw new Exception("@在考核逾期处理方式上，您选择的是:" + this.HisOutTimeDeal + " ,但是您没有为该规则设置内容。");
            //        break;
            //    default:
            //        break;
            //}
            #endregion 检查考核逾期处理的设置的完整性

            #region 处理节点数据.
            Node nd = new Node(this.NodeID);
            if (nd.IsStartNode == true)
            {
                /*处理按钮的问题*/
                //不能退回, 加签，移交，退回, 子线程.
                this.SetValByKey(BtnAttr.ReturnRole,(int)ReturnRole.CanNotReturn);
                this.SetValByKey(BtnAttr.HungEnable, false);
                this.SetValByKey(BtnAttr.ThreadEnable, false); //子线程.
            }

            if (nd.HisRunModel == RunModel.HL || nd.HisRunModel == RunModel.FHL)
            {
                /*如果是合流点*/
            }
            else
            {
                this.SetValByKey(BtnAttr.ThreadEnable, false); //子线程.
            }
            #endregion 处理节点数据.

            #region 处理消息参数字段.
            //this.SetPara(NodeAttr.MsgCtrl, this.GetValIntByKey(NodeAttr.MsgCtrl));
            //this.SetPara(NodeAttr.MsgIsSend, this.GetValIntByKey(NodeAttr.MsgIsSend));
            //this.SetPara(NodeAttr.MsgIsReturn, this.GetValIntByKey(NodeAttr.MsgIsReturn));
            //this.SetPara(NodeAttr.MsgIsShift, this.GetValIntByKey(NodeAttr.MsgIsShift));
            //this.SetPara(NodeAttr.MsgIsCC, this.GetValIntByKey(NodeAttr.MsgIsCC));

            //this.SetPara(NodeAttr.MailEnable, this.GetValIntByKey(NodeAttr.MailEnable));
            //this.SetPara(NodeAttr.MsgMailTitle, this.GetValStrByKey(NodeAttr.MsgMailTitle));
            //this.SetPara(NodeAttr.MsgMailDoc, this.GetValStrByKey(NodeAttr.MsgMailDoc));

            //this.SetPara(NodeAttr.MsgSMSEnable, this.GetValIntByKey(NodeAttr.MsgSMSEnable));
            //this.SetPara(NodeAttr.MsgSMSDoc, this.GetValStrByKey(NodeAttr.MsgSMSDoc));
            #endregion

            //创建审核组件附件
            FrmAttachment workCheckAth = new FrmAttachment();
            bool isHave = workCheckAth.RetrieveByAttr(FrmAttachmentAttr.MyPK, this.NodeID + "_FrmWorkCheck");
            //不包含审核组件
            if (isHave == false)
            {
                workCheckAth = new FrmAttachment();
                /*如果没有查询到它,就有可能是没有创建.*/
                workCheckAth.MyPK = this.NodeID + "_FrmWorkCheck";
                workCheckAth.FK_MapData = this.NodeID.ToString();
                workCheckAth.NoOfObj = this.NodeID + "_FrmWorkCheck";
                workCheckAth.Exts = "*.*";

                //存储路径.
                workCheckAth.SaveTo = "/DataUser/UploadFile/";
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
                workCheckAth.SetValByKey("AtPara", "@IsWoEnablePageset=1@IsWoEnablePrint=1@IsWoEnableViewModel=1@IsWoEnableReadonly=0@IsWoEnableSave=1@IsWoEnableWF=1@IsWoEnableProperty=1@IsWoEnableRevise=1@IsWoEnableIntoKeepMarkModel=1@FastKeyIsEnable=0@IsWoEnableViewKeepMark=1@FastKeyGenerRole=@IsWoEnableTemplete=1");
                workCheckAth.Insert();
            }   

            //清除所有的缓存.
            BP.DA.CashEntity.DCash.Clear();

            return base.beforeUpdate();
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
        #endregion

        public override Entity GetNewEntity
        {
            get { return new NodeExt(); }
        }
    }
}
