using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using BP.WF;
using BP.Web;
using BP.En;
using BP.DA;
using BP.En;

namespace BP.WF
{
    /// <summary>
    /// 发起流程参数列表,为了防止参数错误.
    /// </summary>
    public class StartFlowParaNameList
    {
        #region 功能的参数标记.
        /// <summary>
        /// 是否删除草稿
        /// </summary>
        public const string IsDeleteDraft = "IsDeleteDraft";
        #endregion 功能的参数标记.

        #region 从一个指定表里向开始节点copy数据.
        /// <summary>
        /// 指定的表名称.
        /// </summary>
        public const string FromTableName = "FromTableName";
        /// <summary>
        /// 主键
        /// </summary>
        public const string FromTablePK = "FromTablePK";
        /// <summary>
        /// 主键值
        /// </summary>
        public const string FromTablePKVal = "FromTablePKVal";
        #endregion 

        #region 从一个指定的节点里copy数据到开始节点表里.
        public const string CopyFormWorkID = "CopyFormWorkID";
        public const string CopyFormNode = "CopyFormNode";
        #endregion 

        #region 父子流程相关.
        public const string PFlowNo = "PFlowNo";
        public const string PNodeID = "PNodeID";
        public const string PWorkID = "PWorkID";
        public const string PFID = "PFID";
        /// <summary>
        /// 发起人
        /// </summary>
        public const string PEmp = "PEmp";
        #endregion.

        #region 流程跳转相关.
        public const string JumpToNode = "JumpToNode";
        public const string JumpToEmp = "JumpToEmp";
        #endregion 流程跳转相关.
    }
    /// <summary>
    /// 执行内容列表
    /// </summary>
    public class DoWhatList
    {
        public const string DoNode = "DoNode";
        public const string Start = "Start";
        public const string Start5 = "Start5";
        public const string StartSimple = "StartSimple";
        public const string Amaze = "Amaze";
        public const string StartLigerUI = "StartLigerUI";
        public const string MyWork = "MyWork";
        public const string Login = "Login";
        public const string FlowSearch = "FlowSearch";
        public const string FlowSearchSmall = "FlowSearchSmall";
        public const string FlowSearchSmallSingle = "FlowSearchSmallSingle";
        public const string Emps = "Emps";
        public const string EmpWorks = "EmpWorks";
        public const string MyFlow = "MyFlow";
        public const string MyCC = "MyCC";
        public const string CCList = "CCList";
        public const string Complete = "Complete";
        public const string StartEarlyer = "StartEarlyer";
        public const string Watchdog = "Watchdog";
        public const string DistributedOfMy = "DistributedOfMy";
        public const string SearchDataGrid = "SearchDataGrid";
        public const string SearchDelays = "SearchDelays";
        public const string HuiQianList = "HuiQianList";
        public const string TeamupList = "TeamupList";
        public const string AuthorList = "AuthorList";
        public const string AuthorTodolist2019 = "AuthorTodolist2019";
        public const string TaskPoolSharing = "TaskPoolSharing";
        public const string TaskPoolApply = "TaskPoolApply";
        public const string FutureTodolist = "FutureTodolist";
        public const string MySetting = "MySetting";
        public const string MyFocus = "MyFocus";
        public const string HungupList = "HungupList";
        public const string FlowFX = "FlowFX";
        public const string DealWork = "DealWork";
        public const string Bill = "Bill";
        public const string Home = "Home";
		public const string MyStartFlows = "MyStartFlows";
        public const string MyJoinFlows = "MyJoinFlows";
        public const string GenerWorkFlowViews = "GenerWorkFlowViews";
        /// <summary>
        /// 处理消息连接
        /// </summary>
        public const string DealMsg = "DealMsg";
        public const string Tools = "Tools";
        public const string ToolsSmall = "ToolsSmall";
        public const string Runing = "Runing";
        public const string Draft = "Draft";
        /// <summary>
        /// 打开iu成
        /// </summary>
        public const string Flows = "Flows";
        public const string Frms = "Frms";
        /// <summary>
        /// 工作处理器
        /// </summary>
        public const string OneWork = "OneWork";
    }
     
}