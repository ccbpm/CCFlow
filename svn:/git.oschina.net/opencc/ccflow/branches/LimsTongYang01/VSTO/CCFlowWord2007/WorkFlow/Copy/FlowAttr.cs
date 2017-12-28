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
        /// <summary>
        /// 运行的SQL
        /// </summary>
        public const string RunSQL = "RunSQL";
        /// <summary>
        /// 开始运行的列表
        /// </summary>
        public const string StartListUrl = "StartListUrl";
        /// <summary>
        /// 标题生成规则
        /// </summary>
        public const string TitleRole = "TitleRole";
        /// <summary>
        /// 流程类型
        /// </summary>
        public const string FlowType = "FlowType";
        /// <summary>
        /// 平均用天
        /// </summary>
        public const string AvgDay = "AvgDay";
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
        /// 明细表数量
        /// </summary>
        public const string NumOfDtl = "NumOfDtl";
        /// <summary>
        /// 是否可以启动？
        /// </summary>
        public const string IsCanStart = "IsCanStart";
        /// <summary>
        /// 类型
        /// </summary>
        public const string FlowAppType = "FlowAppType";
        /// <summary>
        /// 时效性规则
        /// </summary>
        public const string TimelineRole = "TimelineRole";
        /// <summary>
        /// 顺序号
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 参数
        /// </summary>
        public const string Paras = "Paras";
        /// <summary>
        /// 业务主表
        /// </summary>
        public const string PTable = "PTable";
        /// <summary>
        /// 流程数据存储模式
        /// </summary>
        public const string DataStoreModel = "DataStoreModel";
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string FlowCode = "FlowCode";
        /// <summary>
        /// 流程设计者编号
        /// </summary>
        public const string DesignerNo = "DesignerNo";
        /// <summary>
        /// 流程设计者名称
        /// </summary>
        public const string DesignerName = "DesignerName";
        /// <summary>
        /// 历史发起查看字段
        /// </summary>
        public const string HistoryFields = "HistoryFields";
        /// <summary>
        /// 是否是客户参与流程
        /// </summary>
        public const string IsGuestFlow = "IsGuestFlow";
        /// <summary>
        /// 单据编号格式
        /// </summary>
        public const string BillNoFormat = "BillNoFormat";
        /// <summary>
        /// 流程备注的表达式
        /// </summary>
        public const string FlowNoteExp = "FlowNoteExp";
        /// <summary>
        /// 数据权限控制方式
        /// </summary>
        public const string DRCtrlType = "DRCtrlType";
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
        /// 按流程
        /// </summary>
        ByFlow
    }
    /// <summary>
    /// 流程数据存储模式
    /// </summary>
    public enum DataStoreModel
    {
        /// <summary>
        /// 存储在CCFlow数据表里
        /// </summary>
        ByCCFlow,
        /// <summary>
        /// 指定的业务主表
        /// </summary>
        SpecTable
    }
    /// <summary>
    /// 流程部门权限控制类型(与报表查询相关)
    /// </summary>
    public enum FlowDeptDataRightCtrlType
    {
        /// <summary>
        /// 只能查看本部门
        /// </summary>
        MyDeptOnly,
        /// <summary>
        /// 查看本部门与下级部门
        /// </summary>
        MyDeptAndBeloneToMyDeptOnly,
        /// <summary>
        /// 按指定该流程的部门人员权限控制
        /// </summary>
        BySpecFlowDept,
        /// <summary>
        /// 不控制，任何人都可以查看任何部门的数据.
        /// </summary>
        AnyoneAndAnydept
    }
}
