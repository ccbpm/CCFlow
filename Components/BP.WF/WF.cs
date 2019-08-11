using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
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
        public const string FlowFX = "FlowFX";
        public const string DealWork = "DealWork";
        /// <summary>
        /// 处理消息连接
        /// </summary>
        public const string DealMsg = "DealMsg";
        public const string Tools = "Tools";
        public const string ToolsSmall = "ToolsSmall";
        public const string Runing = "Runing";
        /// <summary>
        /// 工作处理器
        /// </summary>
        public const string OneWork = "OneWork";
    }
    public enum FlowShowType
    {
        /// <summary>
        /// 当前工作
        /// </summary>
        MyWorks,
        /// <summary>
        /// 新建
        /// </summary>
        WorkNew,
        /// <summary>
        /// 工作步骤
        /// </summary>
        WorkStep,
        /// <summary>
        /// 工作图片
        /// </summary>
        WorkImages
    }
    public enum WorkProgress
    {
        /// <summary>
        /// 正常运行
        /// </summary>
        Runing,
        /// <summary>
        /// 预警
        /// </summary>
        Alert,
        /// <summary>
        /// 逾期
        /// </summary>
        Timeout
    }

}