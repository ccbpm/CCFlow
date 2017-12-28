using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BP.WF
{
   
    /// <summary>
    /// 加签模式
    /// </summary>
    public enum AskforHelpSta
    {
        /// <summary>
        /// 加签后直接发送
        /// </summary>
        AfterDealSend=5,
        /// <summary>
        /// 加签后由我直接发送
        /// </summary>
        AfterDealSendByWorker=6
    }
    /// <summary>
    /// 删除流程规则
    /// @0=不能删除
    /// @1=逻辑删除
    /// @2=记录日志方式删除: 数据删除后，记录到WF_DeleteWorkFlow中。
    /// @3=彻底删除：
    /// @4=让用户决定删除方式
    /// </summary>
    public enum DelWorkFlowRole
    {
        /// <summary>
        /// 不能删除
        /// </summary>
        None,
        /// <summary>
        /// 按照标记删除(需要交互,填写删除原因)
        /// </summary>
        DeleteByFlag,
        /// <summary>
        /// 删除到日志库(需要交互,填写删除原因)
        /// </summary>
        DeleteAndWriteToLog,
        /// <summary>
        /// 彻底的删除(不需要交互，直接干净彻底的删除)
        /// </summary>
        DeleteReal,
        /// <summary>
        /// 让用户决定删除方式(需要交互)
        /// </summary>
        ByUser
    }
    /// <summary>
    /// 导入流程的模式
    /// </summary>
    public enum ImpFlowTempleteModel
    {
        /// <summary>
        /// 按新的流程
        /// </summary>
        AsNewFlow,
        /// <summary>
        /// 按原来的流程编号
        /// </summary>
        AsTempleteFlowNo,
        /// <summary>
        /// 按原来的流程编号如果存在该编号就覆盖它
        /// </summary>
        AsTempleteFlowNoOvrewaiteWhenExit,
        /// <summary>
        /// 按指定的流程编号导入
        /// </summary>
        AsSpecFlowNo
    }
    public enum ActionType
    {
        /// <summary>
        /// 发起
        /// </summary>
        Start,
        /// <summary>
        /// 前进(发送)
        /// </summary>
        Forward,
        /// <summary>
        /// 退回
        /// </summary>
        Return,
        /// <summary>
        /// 移交
        /// </summary>
        Shift,
        /// <summary>
        /// 撤消移交
        /// </summary>
        UnShift,
        /// <summary>
        /// 撤消发送
        /// </summary>
        UnSend,
        /// <summary>
        /// 分流前进
        /// </summary>
        ForwardFL,
        /// <summary>
        /// 合流前进
        /// </summary>
        ForwardHL,
        /// <summary>
        /// 流程正常结束
        /// </summary>
        FlowOver,
        /// <summary>
        /// 调用起子流程
        /// </summary>
        CallChildenFlow,
        /// <summary>
        /// 启动子流程
        /// </summary>
        StartChildenFlow,
        /// <summary>
        /// 子线程前进
        /// </summary>
        SubFlowForward,
        /// <summary>
        /// 取回
        /// </summary>
        Tackback,
        /// <summary>
        /// 恢复已完成的流程
        /// </summary>
        RebackOverFlow,
        /// <summary>
        /// 强制终止流程 For lijian:2012-10-24.
        /// </summary>
        FlowOverByCoercion,
        /// <summary>
        /// 挂起
        /// </summary>
        HungUp,
        /// <summary>
        /// 取消挂起
        /// </summary>
        UnHungUp,
        /// <summary>
        /// 强制移交
        /// </summary>
        ShiftByCoercion,
        /// <summary>
        /// 催办
        /// </summary>
        Press,
        /// <summary>
        /// 逻辑删除流程(撤销流程)
        /// </summary>
        DeleteFlowByFlag,
        /// <summary>
        /// 恢复删除流程(撤销流程)
        /// </summary>
        UnDeleteFlowByFlag,
        /// <summary>
        /// 抄送
        /// </summary>
        CC,
        /// <summary>
        /// 工作审核
        /// </summary>
        WorkCheck,
        /// <summary>
        /// 删除子线程
        /// </summary>
        DeleteSubThread,
        /// <summary>
        /// 请求加签
        /// </summary>
        AskforHelp,
        /// <summary>
        /// 加签向下发送
        /// </summary>
        ForwardAskfor,
        /// <summary>
        /// 自动条转的方式向下发送
        /// </summary>
        Skip,
        /// <summary>
        /// 信息
        /// </summary>
        Info
    }
    /// <summary>
    /// 挂起方式
    /// </summary>
    public enum HungUpWay
    {
        /// <summary>
        /// 永久挂起
        /// </summary>
        Forever,
        /// <summary>
        /// 在指定的日期解除
        /// </summary>
        SpecDataRel
    }
    /// <summary>
    /// 当没有找到处理人时
    /// </summary>
    public enum WhenNoWorker
    {
         /// <summary>
        /// 提示错误
        /// </summary>
        AlertErr,
        /// <summary>
        /// 跳转到下一步
        /// </summary>
        Skip
    }
    /// <summary>
    /// 自动跳转规则
    /// </summary>
    public enum AutoJumpRole
    {
        /// <summary>
        /// 处理人就是提交人
        /// </summary>
        DealerIsDealer,
        /// <summary>
        /// 处理人已经出现过
        /// </summary>
        DealerIsInWorkerList,
        /// <summary>
        /// 处理人与上一步相同
        /// </summary>
        DealerAsNextStepWorker
    }
    /// <summary>
    /// 节点工作批处理
    /// </summary>
    public enum BatchRole
    {
        /// <summary>
        /// 不可以
        /// </summary>
        No,
        /// <summary>
        /// 可以
        /// </summary>
        Yes
    }
    /// <summary>
    /// 流程应用类型
    /// </summary>
    public enum FlowAppType
    {
        /// <summary>
        /// 普通的
        /// </summary>
        Normal,
        /// <summary>
        /// 工程类
        /// </summary>
        PRJ,
        /// <summary>
        /// 公文流程
        /// </summary>
        DocFlow
    }
    /// <summary>
    /// 子线程启动方式
    /// </summary>
    public enum SubFlowStartWay
    {
        /// <summary>
        /// 不启动
        /// </summary>
        None,
        /// <summary>
        /// 按表单字段
        /// </summary>
        BySheetField,
        /// <summary>
        /// 按从表数据
        /// </summary>
        BySheetDtlTable
    }
    /// <summary>
    /// 撤销规则
    /// </summary>
    public enum CancelRole
    {
        /// <summary>
        /// 仅上一步
        /// </summary>
        OnlyNextStep,
        /// <summary>
        /// 不能撤销
        /// </summary>
        None,
        /// <summary>
        /// 上一步与开始节点.
        /// </summary>
        NextStepAndStartNode,
        /// <summary>
        /// 可以撤销指定的节点
        /// </summary>
        SpecNodes
    }
    /// <summary>
    /// 抄送方式
    /// </summary>
    public enum CCWay
    {
        /// <summary>
        /// 按照信息发送
        /// </summary>
        ByMsg,
        /// <summary>
        /// 按照e-mail
        /// </summary>
        ByEmail,
        /// <summary>
        /// 按照电话
        /// </summary>
        ByPhone,
        /// <summary>
        /// 按照数据库功能
        /// </summary>
        ByDBFunc
    }
    /// <summary>
    /// 抄送类型
    /// </summary>
    public enum CCType
    {
        /// <summary>
        /// 不抄送
        /// </summary>
        None,
        /// <summary>
        /// 按人员
        /// </summary>
        AsEmps,
        /// <summary>
        /// 按岗位
        /// </summary>
        AsStation,
        /// <summary>
        /// 按节点
        /// </summary>
        AsNode,
        /// <summary>
        /// 按部门
        /// </summary>
        AsDept,
        /// <summary>
        /// 按照部门与岗位
        /// </summary>
        AsDeptAndStation
    }
    /// <summary>
    /// 流程类型
    /// </summary>
    public enum FlowType_del
    {
        /// <summary>
        /// 平面流程
        /// </summary>
        Panel,
        /// <summary>
        /// 分合流
        /// </summary>
        FHL
    }
   
    /// <summary>
    /// 流程启动类型
    /// </summary>
    public enum FlowRunWay
    {
        /// <summary>
        /// 手工启动
        /// </summary>
        HandWork,
        /// <summary>
        /// 指定人员按时启动
        /// </summary>
        SpecEmp,
        /// <summary>
        /// 数据集按时启动
        /// </summary>
        DataModel,
        /// <summary>
        /// 触发式启动
        /// </summary>
        InsertModel
    }
    /// <summary>
    /// 保存模式
    /// </summary>
    public enum SaveModel
    {
        /// <summary>
        /// 仅节点表.
        /// </summary>
        NDOnly,
        /// <summary>
        /// 节点表与Rpt表.
        /// </summary>
        NDAndRpt
    }
    /// <summary>
    /// 节点完成转向处理
    /// </summary>
    public enum TurnToDeal
    {
        /// <summary>
        /// 按系统默认的提示
        /// </summary>
        CCFlowMsg,
        /// <summary>
        /// 指定消息
        /// </summary>
        SpecMsg,
        /// <summary>
        /// 指定Url
        /// </summary>
        SpecUrl,
        /// <summary>
        /// 按条件转向
        /// </summary>
        TurnToByCond
    }
    /// <summary>
    /// 投递方式
    /// </summary>
    public enum DeliveryWay
    {
        /// <summary>
        /// 按岗位(以部门为纬度)
        /// </summary>
        ByStation = 0,
        /// <summary>
        /// 按部门
        /// </summary>
        ByDept = 1,
        /// <summary>
        /// 按SQL
        /// </summary>
        BySQL = 2,
        /// <summary>
        /// 按本节点绑定的人员
        /// </summary>
        ByBindEmp = 3,
        /// <summary>
        /// 由上一步发送人选择
        /// </summary>
        BySelected = 4,
        /// <summary>
        /// 按表单选择人员
        /// </summary>
        ByPreviousNodeFormEmpsField = 5,
        /// <summary>
        /// 按上一步操作人员
        /// </summary>
        ByPreviousOper = 6,
        /// <summary>
        /// 按上一步操作人员并自动跳转
        /// </summary>
        ByPreviousOperSkip = 7,
        /// <summary>
        /// 按指定节点的人员计算
        /// </summary>
        BySpecNodeEmp = 8,
        /// <summary>
        /// 按岗位与部门交集计算
        /// </summary>
        ByDeptAndStation = 9,
        /// <summary>
        /// 按岗位计算(以部门集合为纬度)
        /// </summary>
        ByStationAndEmpDept = 10,
        /// <summary>
        /// 按指定节点的人员岗位计算
        /// </summary>
        BySpecNodeEmpStation = 11,
        /// <summary>
        /// 按SQL确定子线程接受人与数据源.
        /// </summary>
        BySQLAsSubThreadEmpsAndData = 12,
        /// <summary>
        /// 按明细表确定子线程接受人.
        /// </summary>
        ByDtlAsSubThreadEmps = 13,
        /// <summary>
        /// 仅按岗位计算
        /// </summary>
        ByStationOnly = 14,
        /// <summary>
        /// 按照ccflow的BPM模式处理
        /// </summary>
        ByCCFlowBPM=100
    }
    /// <summary>
    /// 节点工作退回规则
    /// </summary>
    public enum JumpWay
    {
        /// <summary>
        /// 不能跳转
        /// </summary>
        CanNotJump,
        /// <summary>
        /// 向后跳转
        /// </summary>
        Next,
        /// <summary>
        /// 向前跳转
        /// </summary>
        Previous,
        /// <summary>
        /// 任何节点
        /// </summary>
        AnyNode,
        /// <summary>
        /// 任意点
        /// </summary>
        JumpSpecifiedNodes
    }
    /// <summary>
    /// 节点工作退回规则
    /// </summary>
    public enum ReturnRole
    {
        /// <summary>
        /// 不能退回
        /// </summary>
        CanNotReturn,
        /// <summary>
        /// 只能退回上一个节点
        /// </summary>
        ReturnPreviousNode,
        /// <summary>
        /// 可退回以前任意节点(默认)
        /// </summary>
        ReturnAnyNodes,
        /// <summary>
        /// 可退回指定的节点
        /// </summary>
        ReturnSpecifiedNodes,
        /// <summary>
        /// 由流程图设计的退回路线来决定
        /// </summary>
        ByReturnLine
    }
    /// <summary>
    /// 附件开放类型
    /// </summary>
    public enum FJOpen
    {
        /// <summary>
        /// 不开放
        /// </summary>
        None,
        /// <summary>
        /// 对操作员开放
        /// </summary>
        ForEmp,
        /// <summary>
        /// 对工作ID开放
        /// </summary>
        ForWorkID,
        /// <summary>
        /// 对流程ID开放
        /// </summary>
        ForFID
    }
    /// <summary>
    /// 分流规则
    /// </summary>
    public enum FLRole
    {
        /// <summary>
        /// 按照接受人
        /// </summary>
        ByEmp,
        /// <summary>
        /// 按照部门
        /// </summary>
        ByDept,
        /// <summary>
        /// 按照岗位
        /// </summary>
        ByStation
    }
    /// <summary>
    /// 运行模式
    /// </summary>
    public enum RunModel
    {
        /// <summary>
        /// 普通
        /// </summary>
        Ordinary = 0,
        /// <summary>
        /// 合流
        /// </summary>
        HL = 1,
        /// <summary>
        /// 分流
        /// </summary>
        FL = 2,
        /// <summary>
        /// 分合流
        /// </summary>
        FHL = 3,
        /// <summary>
        /// 子线程
        /// </summary>
        SubThread = 4
    }
    /// <summary>
    /// 流程状态
    /// ccflow根据是否启用草稿分两种工作模式,它的设置是在web.config 是 IsEnableDraft 节点来配置的.
    /// 1, 不启用草稿  IsEnableDraft = 0.
    ///    这种模式下，就没有草稿状态, 一个用户进入工作界面后就生成一个Blank, 用户保存时，也是存储blank状态。
    /// 2, 启用草稿.
    /// </summary>
    public enum WFState
    {
        /// <summary>
        /// 空白
        /// </summary>
        Blank = 0,
        /// <summary>
        /// 草稿
        /// </summary>
        Draft = 1,
        /// <summary>
        /// 运行中
        /// </summary>
        Runing = 2,
        /// <summary>
        /// 已完成
        /// </summary>
        Complete = 3,
        /// <summary>
        /// 挂起
        /// </summary>
        HungUp = 4,
        /// <summary>
        /// 退回
        /// </summary>
        ReturnSta = 5,
        /// <summary>
        /// 转发(移交)
        /// </summary>
        Shift = 6,
        /// <summary>
        /// 删除(逻辑删除状态)
        /// </summary>
        Delete = 7,
        /// <summary>
        /// 加签
        /// </summary>
        Askfor=8,
        /// <summary>
        /// 冻结
        /// </summary>
        Fix=9
    }
    /// <summary>
    /// 节点工作类型
    /// </summary>
    public enum NodeWorkType
    {
        Work = 0,
        /// <summary>
        /// 开始节点
        /// </summary>
        StartWork = 1,
        /// <summary>
        /// 开始节点分流
        /// </summary>
        StartWorkFL = 2,
        /// <summary>
        /// 合流节点
        /// </summary>
        WorkHL = 3,
        /// <summary>
        /// 分流节点
        /// </summary>
        WorkFL = 4,
        /// <summary>
        /// 分合流
        /// </summary>
        WorkFHL = 5,
        /// <summary>
        /// 子流程
        /// </summary>
        SubThreadWork = 6
    }
    ///// <summary>
    ///// 流程节点类型
    ///// </summary>
    //public enum FNType
    //{
    //    /// <summary>
    //    /// 平面节点
    //    /// </summary>
    //    Plane = 0,
    //    /// <summary>
    //    /// 分合流
    //    /// </summary>
    //    River = 1,
    //    /// <summary>
    //    /// 支流
    //    /// </summary>
    //    Branch = 2
    //}
    /// <summary>
    /// 谁执行它
    /// </summary>
    public enum CCRole
    {
        /// <summary>
        /// 不能抄送
        /// </summary>
        UnCC,
        /// <summary>
        /// 手工抄送
        /// </summary>
        HandCC,
        /// <summary>
        /// 自动抄送
        /// </summary>
        AutoCC,
        /// <summary>
        /// 手工与自动并存
        /// </summary>
        HandAndAuto,
        /// <summary>
        /// 按字段
        /// </summary>
        BySysCCEmps
    }
    /// <summary>
    /// 谁执行它
    /// </summary>
    public enum WhoDoIt
    {
        /// <summary>
        /// 操作员
        /// </summary>
        Operator,
        /// <summary>
        /// 机器
        /// </summary>
        MachtionOnly,
        /// <summary>
        /// 混合
        /// </summary>
        Mux
    }
    /// <summary>
    /// 位置类型
    /// </summary>
    public enum NodePosType
    {
        Start,
        Mid,
        End
    }
    /// <summary>
    /// 节点数据采集类型
    /// </summary>
    public enum FormType
    {
        /// <summary>
        /// 傻瓜表单.
        /// </summary>
        FixForm = 0,
        /// <summary>
        /// 自由表单.
        /// </summary>
        FreeForm = 1,
        /// <summary>
        /// 自定义表单.
        /// </summary>
        SelfForm = 2,
        /// <summary>
        /// SDKForm
        /// </summary>
        SDKForm = 3,
        /// <summary>
        /// SL表单
        /// </summary>
        SLForm=4,
        /// <summary>
        /// 禁用(对多表单流程有效)
        /// </summary>
        DisableIt = 9
    }
    /// <summary>
    /// 工作类型
    /// </summary>
    public enum WorkType
    {
        /// <summary>
        /// 普通的
        /// </summary>
        Ordinary,
        /// <summary>
        /// 自动的
        /// </summary>
        Auto
    }
    /// <summary>
    /// 子线程类型
    /// </summary>
    public enum SubThreadType
    {
        /// <summary>
        /// 同表单
        /// </summary>
        SameSheet,
        /// <summary>
        /// 异表单
        /// </summary>
        UnSameSheet
    }
    /// <summary>
    /// 已读回执类型
    /// </summary>
    public enum ReadReceipts
    {
        /// <summary>
        /// 不回执
        /// </summary>
        None,
        /// <summary>
        /// 自动回执
        /// </summary>
        Auto,
        /// <summary>
        /// 由系统字段决定
        /// </summary>
        BySysField,
        /// <summary>
        /// 按开发者参数
        /// </summary>
        BySDKPara
    }
    /// <summary>
    /// 打印方式
    /// @0=不打印@1=打印网页@2=打印RTF模板
    /// </summary>
    public enum PrintDocEnable
    {
        /// <summary>
        /// 不打印
        /// </summary>
        None,
        /// <summary>
        /// 打印网页
        /// </summary>
        PrintHtml,
        /// <summary>
        /// 打印RTF模板
        /// </summary>
        PrintRTF
    }
}
