using System;
using System.Collections.Generic;
using System.Text;

namespace BP.WF.Template
{
    /// <summary>
    /// 节点属性
    /// </summary>
    public class NodeAttr
    {
        #region 新属性
        /// <summary>
        /// 审核组件状态
        /// </summary>
        public const string FWCSta = "FWCSta";
        /// <summary>
        /// 审核组件高度
        /// </summary>
        public const string FWC_H = "FWC_H";
        /// <summary>
        /// 阻塞模式
        /// </summary>
        public const string BlockModel = "BlockModel";
        /// <summary>
        /// 阻塞的表达式
        /// </summary>
        public const string BlockExp = "BlockExp";
        /// <summary>
        /// 被阻塞时提示信息
        /// </summary>
        public const string BlockAlert = "BlockAlert";
        /// <summary>
        /// 待办处理模式
        /// </summary>
        public const string TodolistModel = "TodolistModel";
        /// <summary>
        /// 组长确认规则
        /// </summary>
        public const string TeamLeaderConfirmRole = "TeamLeaderConfirmRole";
        /// <summary>
        /// 组长确认内容
        /// </summary>
        public const string TeamLeaderConfirmDoc = "TeamLeaderConfirmDoc";
        /// <summary>
        /// 当没有找到处理人时处理方式
        /// </summary>
        public const string WhenNoWorker = "WhenNoWorker";
        /// <summary>
        /// 子线程类型
        /// </summary>
        public const string SubThreadType = "SubThreadType";
        /// <summary>
        /// 是否可以隐性退回
        /// </summary>
        public const string IsCanHidReturn = "IsCanHidReturn";
        /// <summary>
        /// 通过率
        /// </summary>
        public const string PassRate = "PassRate";
      
     //   public const string IsCCNode = "IsCCNode";
        public const string IsCCFlow = "IsCCFlow";
        public const string HisStas = "HisStas";
        public const string HisToNDs = "HisToNDs";
        public const string HisBillIDs = "HisBillIDs";
        public const string NodePosType = "NodePosType";
        public const string HisDeptStrs = "HisDeptStrs";
        public const string HisEmps = "HisEmps";
        public const string GroupStaNDs = "GroupStaNDs";
        public const string IsHandOver = "IsHandOver";
        public const string IsCanDelFlow = "IsCanDelFlow";
        /// <summary>
        /// 是否可以原路返回
        /// </summary>
        public const string IsBackTracking = "IsBackTracking";
        /// <summary>
        /// 退回原因(多个原因使用@符号分开.)
        /// </summary>
        public const string ReturnReasonsItems = "ReturnReasonsItems";
        /// <summary>
        /// 单节点退回规则
        /// </summary>
        public const string ReturnOneNodeRole = "ReturnOneNodeRole";
        /// <summary>
        /// 退回提示
        /// </summary>
        public const string ReturnAlert = "ReturnAlert";
        /// <summary>
        /// 是否启用投递路径自动记忆功能?
        /// </summary>
        public const string IsRM = "IsRM";
        public const string FormType = "FormType";
        public const string FormUrl = "FormUrl";
        /// <summary>
        /// 发送之前的信息提示
        /// </summary>
        public const string BeforeSendAlert = "BeforeSendAlert";
        /// <summary>
        /// 是否可以强制删除线程
        /// </summary>
        public const string ThreadKillRole = "ThreadKillRole";
        /// <summary>
        /// 接受人sql
        /// </summary>
        public const string DeliveryParas = "DeliveryParas";
        /// <summary>
        /// 退回规则
        /// </summary>
        public const string ReturnRole = "ReturnRole";
        /// <summary>
        /// 可触发子流程
        /// </summary>
        public const string SFActiveFlows = "SFActiveFlows";
        /// <summary>
        /// 转向处理
        /// </summary>
        public const string TurnToDeal = "TurnToDeal";
        /// <summary>
        /// 考核方式
        /// </summary>
        public const string CHWay = "CHWay";
        /// <summary>
        /// 工作量
        /// </summary>
        public const string WorkloadDel = "Workload";
        #endregion

        #region 基本属性
        /// <summary>
        /// OID
        /// </summary>
        public const string NodeID = "NodeID";
        /// <summary>
        /// 节点的流程
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 操作提示
        /// </summary>
        public const string Tip = "Tip";
        /// <summary>
        /// FK_FlowSort
        /// </summary>
        public const string FK_FlowSort = "FK_FlowSort";
        /// <summary>
        /// FK_FlowSortT
        /// </summary>
        public const string FK_FlowSortT = "FK_FlowSortT";
        /// <summary>
        /// 流程名
        /// </summary>
        public const string FlowName = "FlowName";
        /// <summary>
        /// 是否分配工作
        /// </summary>
        public const string IsTask = "IsTask";
        /// <summary>
        /// 是否是业务单元？
        /// </summary>
        public const string IsBUnit = "IsBUnit";
        /// <summary>
        /// 节点工作类型
        /// </summary>
        public const string NodeWorkType = "NodeWorkType";
        /// <summary>
        /// 节点的描述
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// x
        /// </summary>
        public const string X = "X";
        /// <summary>
        /// y
        /// </summary>
        public const string Y = "Y";
        /// <summary>
        /// 限期小时
        /// </summary>
        public const string TSpanHour = "TSpanHour";
        /// <summary>
        /// 限期天
        /// </summary>
        public const string TimeLimit = "TimeLimit";
        /// <summary>
        /// 时间计算方式
        /// </summary>
        public const string TWay = "TWay";
        /// <summary>
        /// 逾期提示规则
        /// </summary>
        public const string TAlertRole = "TAlertRole";
        /// <summary>
        /// 逾期提示方式
        /// </summary>
        public const string TAlertWay = "TAlertWay";
        /// <summary>
        /// 预警小时
        /// </summary>
        public const string WarningDay = "WarningDay";
        /// <summary>
        /// 预警提示规则
        /// </summary>
        public const string WAlertRole = "WAlertRole";
        /// <summary>
        /// 预警提示方式
        /// </summary>
        public const string WAlertWay = "WAlertWay";
        /// <summary>
        /// 扣分
        /// </summary>
        public const string TCent = "TCent";
        /// <summary>
        /// 流程步骤
        /// </summary>
        public const string Step = "Step";
        /// <summary>
        /// 工作内容
        /// </summary>
        public const string Doc = "Doc";
        /// <summary>
        ///  物理表名
        /// </summary>
        public const string PTable = "PTable";
        /// <summary>
        /// 签字类型
        /// </summary>
        public const string SignType = "SignType";
        /// <summary>
        /// 显示的表单
        /// </summary>
        public const string ShowSheets = "ShowSheets";
        /// <summary>
        /// 运行模式
        /// </summary>
        public const string RunModel = "RunModel";
        /// <summary>
        /// 谁执行它？
        /// </summary>
        public const string WhoExeIt = "WhoExeIt";
        /// <summary>
        /// IsSubFlow
        /// </summary>
        public const string HisSubFlows = "HisSubFlows";
        /// <summary>
        /// 超时处理内容
        /// </summary>
        public const string DoOutTime = "DoOutTime";
        /// <summary>
        /// 超时处理内容
        /// </summary>
        public const string OutTimeDeal = "OutTimeDeal";
        /// <summary>
        /// 执行超时的条件
        /// </summary>
        public const string DoOutTimeCond = "DoOutTimeCond";
        /// <summary>
        /// 是否允许子线程接受人员重复？
        /// </summary>
        public const string IsAllowRepeatEmps = "IsAllowRepeatEmps";
        /// <summary>
        /// 是否启动自动运行？
        /// </summary>
        public const string AutoRunEnable = "AutoRunEnable";
        /// <summary>
        /// 自动运行参数
        /// </summary>
        public const string AutoRunParas = "AutoRunParas";
        /// <summary>
        /// 属性
        /// </summary>
        public const string FrmAttr = "FrmAttr";
        /// <summary>
        /// 个性化发送信息
        /// </summary>
        public const string TurnToDealDoc = "TurnToDealDoc";
        /// <summary>
        /// 访问规则
        /// </summary>
        public const string DeliveryWay = "DeliveryWay";
        /// <summary>
        /// 本节点接收人不允许包含上一步发送人
        /// </summary>
        public const string IsExpSender = "IsExpSender";
        /// <summary>
        /// 焦点字段
        /// </summary>
        public const string FocusField = "FocusField";
        /// <summary>
        /// 节点表单ID
        /// </summary>
        public const string NodeFrmID = "NodeFrmID";
        /// <summary>
        /// 跳转规则
        /// </summary>
        public const string JumpWay = "JumpWay";
        /// <summary>
        /// 可跳转的节点
        /// </summary>
        public const string JumpToNodes = "JumpToNodes";
        /// <summary>
        /// 已读回执
        /// </summary>
        public const string ReadReceipts = "ReadReceipts";
        /// <summary>
        /// 操送规则
        /// </summary>
        public const string CCRole = "CCRole";
        /// <summary>
        /// 保存模式
        /// </summary>
        public const string SaveModel = "SaveModel";
        /// <summary>
        /// 方向条件控制规则
        /// </summary>
        public const string CondModel = "CondModel";
        /// <summary>
        /// 子流程启动方式
        /// </summary>
        public const string SubFlowStartWay = "SubFlowStartWay";
        /// <summary>
        /// 子流程启动参数
        /// </summary>
        public const string SubFlowStartParas = "SubFlowStartParas";
        /// <summary>
        /// 是否工作质量考核点
        /// </summary>
        public const string IsEval = "IsEval";
        /// <summary>
        /// 撤销规则
        /// </summary>
        public const string CancelRole = "CancelRole";
        /// <summary>
        /// 对方已读不能撤销
        /// </summary>
        public const string CancelDisWhenRead = "CancelDisWhenRead";
        /// <summary>
        /// 抄送数据写入规则
        /// </summary>
        public const string CCWriteTo = "CCWriteTo";
        /// <summary>
        /// 批处理
        /// </summary>
        public const string BatchRole = "BatchRole";
        /// <summary>
        /// 批处理参数
        /// </summary>
        public const string BatchParas = "BatchParas";
        /// <summary>
        /// 批处理总数
        /// </summary>
        public const string BatchListCount = "BatchListCount";
        /// <summary>
        /// 自动跳转规则-1
        /// </summary>
        public const string AutoJumpRole0 = "AutoJumpRole0";
        /// <summary>
        /// 自动跳转规则-2
        /// </summary>
        public const string AutoJumpRole1 = "AutoJumpRole1";
        /// <summary>
        /// 自动跳转规则-3
        /// </summary>
        public const string AutoJumpRole2 = "AutoJumpRole2";
        /// <summary>
        /// 是否是客户执行节点?
        /// </summary>
        public const string IsGuestNode = "IsGuestNode";
        /// <summary>
        /// 打印单据方式
        /// </summary>
        public const string PrintDocEnable = "PrintDocEnable";
        /// <summary>
        /// icon头像
        /// </summary>
        public const string ICON = "ICON";
        /// <summary>
        /// 自定义参数字段
        /// </summary>
        public const string SelfParas = "SelfParas";
        #endregion

        #region 父子流程
        /// <summary>
        /// (当前节点为启动子流程节点时)是否检查所有子流程结束后,该节点才能向下发送?
        /// </summary>
        public const string IsCheckSubFlowOver_del = "IsCheckSubFlowOver_del";
        #endregion

        #region 移动设置.
        /// <summary>
        /// 手机工作模式
        /// </summary>
        public const string MPhone_WorkModel = "MPhone_WorkModel";
        /// <summary>
        /// 手机屏幕模式
        /// </summary>
        public const string MPhone_SrcModel = "MPhone_SrcModel";
        /// <summary>
        /// pad工作模式
        /// </summary>
        public const string MPad_WorkModel = "MPad_WorkModel";
        /// <summary>
        /// pad屏幕模式
        /// </summary>
        public const string MPad_SrcModel = "MPad_SrcModel";
        #endregion 移动设置.


        #region 未来处理人.
        /// <summary>
        /// 是否计算未来处理人
        /// </summary>
        public const string IsFullSA = "IsFullSA";
        /// <summary>
        /// 是否计算未来处理人的处理时间.
        /// </summary>
        public const string IsFullSATime = "IsFullSATime";
        /// <summary>
        /// 是否接受未来处理人的消息提醒.
        /// </summary>
        public const string IsFullSAAlert = "IsFullSAAlert";

        public const string RefOneFrmTreeType = "RefOneFrmTreeType";
        #endregion 未来处理人.

    }
}
