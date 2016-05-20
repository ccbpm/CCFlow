using System;
using System.Collections.Generic;
using System.Text;

namespace BP.WF
{
    /// <summary>
    /// 节点属性
    /// </summary>
    public class NodeAttr
    {
        #region 新属性
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
        /// <summary>
        /// 是否可以设置流程完成
        /// </summary>
        public const string IsCanOver = "IsCanOver";
        /// <summary>
        /// 是否是保密步骤
        /// </summary>
        public const string IsSecret = "IsSecret";
        public const string IsCCNode = "IsCCNode";
        public const string IsCCFlow = "IsCCFlow";
        public const string HisStas = "HisStas";
        public const string HisToNDs = "HisToNDs";
        public const string HisBillIDs = "HisBillIDs";
        public const string NodePosType = "NodePosType";
        public const string HisDeptStrs = "HisDeptStrs";
        public const string HisEmps = "HisEmps";
        public const string GroupStaNDs = "GroupStaNDs";
        public const string FJOpen = "FJOpen";
        public const string IsCanReturn = "IsCanReturn";
        public const string IsHandOver = "IsHandOver";
        public const string IsCanDelFlow = "IsCanDelFlow";
        /// <summary>
        /// 是否可以原路返回
        /// </summary>
        public const string IsBackTracking = "IsBackTracking";
        /// <summary>
        /// 是否起用投递路径自动记忆功能?
        /// </summary>
        public const string IsRM = "IsRM";

        public const string FormType = "FormType";
        public const string FormUrl = "FormUrl";
        /// <summary>
        /// 是否可以查看工作报告
        /// </summary>
        public const string IsCanRpt = "IsCanRpt";
        /// <summary>
        /// 发送之前的信息提示
        /// </summary>
        public const string BeforeSendAlert = "BeforeSendAlert";
        /// <summary>
        /// 是否可以强制删除线程
        /// </summary>
        public const string IsForceKill = "IsForceKill";
        /// <summary>
        /// 接受人sql
        /// </summary>
        public const string RecipientSQL = "RecipientSQL";
        /// <summary>
        /// 退回规则
        /// </summary>
        public const string ReturnRole = "ReturnRole";
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
        public const string Workload = "Workload";
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
        /// WarningDays(警告天数)
        /// </summary>
        public const string WarningDays_del = "WarningDays";
        /// <summary>
        /// DeductDays(扣分天数)
        /// </summary>
        public const string DeductDays = "DeductDays";
        /// <summary>
        /// 警告天
        /// </summary>
        public const string WarningDays = "WarningDays";
        /// <summary>
        /// 扣分
        /// </summary>
        public const string DeductCent = "DeductCent";
        /// <summary>
        /// 最高扣分
        /// </summary>
        public const string MaxDeductCent = "MaxDeductCent";
        /// <summary>
        /// 辛苦加分
        /// </summary>
        public const string SwinkCent = "SwinkCent";
        /// <summary>
        /// 最大的辛苦加分
        /// </summary>
        public const string MaxSwinkCent = "MaxSwinkCent";
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
        /// 分流规则
        /// </summary>
        public const string FLRole = "FLRole";
        /// <summary>
        /// 是否是干流
        /// </summary>
        public const string FNType = "FNType";
        /// <summary>
        /// IsSubFlow
        /// </summary>
        public const string HisSubFlows = "HisSubFlows";
        /// <summary>
        /// 超时处理内容
        /// </summary>
        public const string DoOutTime = "DoOutTime";
        public const string OutTimeDeal = "OutTimeDeal";
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
        /// 焦点字段
        /// </summary>
        public const string FocusField = "FocusField";
        /// <summary>
        /// 跳转规则
        /// </summary>
        public const string JumpWay = "JumpWay";
        /// <summary>
        /// 可跳转的节点
        /// </summary>
        public const string JumpSQL = "JumpSQL";
        /// <summary>
        /// 操送规则
        /// </summary>
        public const string CCRole = "CCRole";
        /// <summary>
        /// 保存模式
        /// </summary>
        public const string SaveModel = "SaveModel";
        #endregion
    }
}
