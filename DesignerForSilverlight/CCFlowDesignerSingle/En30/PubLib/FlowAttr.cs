using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BP.WF
{
    /// <summary>
    /// 流程属性
    /// </summary>
    public class FlowAttr
    {
        #region 基本属性
        /// <summary>
        /// 编号
        /// </summary>
        public const string No = "No";
        /// <summary>
        /// 名称
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// CCType
        /// </summary>
        public const string CCType = "CCType";
        /// <summary>
        /// 抄送方式
        /// </summary>
        public const string CCWay = "CCWay";
        /// <summary>
        /// 流程类别
        /// </summary>
        public const string FK_FlowSort = "FK_FlowSort";
        /// <summary>
        /// 建立的日期。
        /// </summary>
        public const string CreateDate = "CreateDate";
        /// <summary>
        /// BillTable
        /// </summary>
        public const string BillTable = "BillTable";
        /// <summary>
        /// 开始工作节点类型
        /// </summary>
        public const string StartNodeType = "StartNodeType";
        /// <summary>
        /// StartNodeID
        /// </summary>
        public const string StartNodeID = "StartNodeID";
        /// <summary>
        /// 能不能流程Num考核。
        /// </summary>
        public const string IsCanNumCheck = "IsCanNumCheck";
        /// <summary>
        /// 是否显示附件
        /// </summary>
        public const string IsFJ = "IsFJ";
        #endregion 基本属性

        /// <summary>
        /// 是否起用
        /// </summary>
        public const string IsOK = "IsOK";
        /// <summary>
        /// 是否是MD5
        /// </summary>
        public const string IsMD5 = "IsMD5";
        public const string CCStas = "CCStas";
        public const string Note = "Note";
        public const string RunSQL = "RunSQL";
        public const string StartListUrl = "StartListUrl";
        /// <summary>
        /// 流程类型
        /// </summary>
        public const string FlowType = "FlowType";
        /// <summary>
        /// 平均用天
        /// </summary>
        public const string AvgDay = "AvgDay";
        /// <summary>
        /// 独立表单类型
        /// </summary>
        public const string FlowSheetType = "FlowSheetType";
        /// <summary>
        /// 文档类型
        /// </summary>
        public const string DocType = "DocType";
        /// <summary>
        /// 行文类型
        /// </summary>
        public const string XWType = "XWType";
        /// <summary>
        /// 流程运行类型
        /// </summary>
        public const string FlowRunWay = "FlowRunWay";
        /// <summary>
        /// 运行的设置
        /// </summary>
        public const string RunObj = "RunObj";
        /// <summary>
        /// 是否有Bill
        /// </summary>
        public const string NumOfBill = "NumOfBill";
        /// <summary>
        /// 
        /// </summary>
        public const string NumOfDtl = "NumOfDtl";
        /// <summary>
        /// 是否可以启动？
        /// </summary>
        public const string IsCanStart = "IsCanStart";
        /// <summary>
        /// 轨迹字段
        /// </summary>
        public const string AppType = "AppType";
        /// <summary>
        /// 时效性规则
        /// </summary>
        public const string TimelineRole = "TimelineRole";
        /// <summary>
        /// 顺序号
        /// </summary>
        public const string Idx = "Idx";
    }
    /// <summary>
    /// 流程考核类型
    /// </summary>
    public enum TimelineRole
    {
        /// <summary>
        /// 按节点
        /// </summary>
        ByNodeSet,
        /// <summary>
        /// 
        /// </summary>
        ByFlow
    }
}
