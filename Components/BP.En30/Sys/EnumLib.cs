using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BP.Sys
{
    /// <summary>
    /// 实体类型
    /// </summary>
    public enum EntityType
    {
        /// <summary>
        /// 独立表单
        /// </summary>
        SingleFrm = 0,
        /// <summary>
        /// 单据
        /// </summary>
        FrmBill=1,
        /// <summary>
        /// 实体
        /// </summary>
        FrmDict=2,
        /// <summary>
        /// 实体树
        /// </summary>
        EntityTree = 3,
        /// <summary>
        /// 数据源类型
        /// </summary>
        DBList = 100
    }
    /// <summary>
    /// 数据源类型
    /// </summary>
    public class DBSrcType
    {
        /// <summary>
        /// 本机数据库
        /// </summary>
        public const string local = "local";
        public const string MSSQL = "MSSQL";
        public const string Oracle = "Oracle";
        public const string MySQL = "MySQL";
        public const string Informix = "Informix";
        public const string PostgreSQL = "PostgreSQL";
        public const string KingBaseR3 = "KingBaseR3";
        public const string KingBaseR6 = "KingBaseR6";
        public const string UX = "UX";
        public const string HGDB = "HGDB";
        public const string WebServices = "WebServices";
        public const string Dubbo = "Dubbo";
        public const string CCFromRef = "CCFromRef";
        public const string LocalWS = "LocalWS";
        public const string LocalHandler = "LocalHandler";
        public const string WebApi = "WebApi";

    }
    /// <summary>
    /// 消息控制方式
    /// </summary>
    public enum MsgCtrl
    {
        /// <summary>
        /// bufasong 
        /// </summary>
        None,
        /// <summary>
        /// 按照设置计算
        /// </summary>
        BySet,
        /// <summary>
        /// 按照表单的是否发送字段计算，字段:IsSendMsg
        /// </summary>
        ByFrmIsSendMsg,
        /// <summary>
        /// 按照SDK参数计算.
        /// </summary>
        BySDK
    }
    /// <summary>
    /// 事件执行内容
    /// </summary>
    public enum EventDoType
    {
        /// <summary>
        /// 禁用
        /// </summary>
        Disable = 0,
        /// <summary>
        /// 执行存储过程
        /// </summary>
        SP = 1,
        /// <summary>
        /// 运行SQL
        /// </summary>
        SQL = 2,
        /// <summary>
        /// 自定义URL
        /// </summary>
        URLOfSelf = 3,
        /// <summary>
        /// 自定义WS
        /// </summary>
        WSOfSelf = 4,
        /// <summary>
        /// 执行ddl文件的类与方法
        /// </summary>
        SpecClass = 5,
        /// <summary>
        /// 基类
        /// </summary>
        EventBase = 6,
        /// <summary>
        /// 执行的业务单元
        /// </summary>
        BuessUnit = 7,
        /// <summary>
        /// 自定义WebApi
        /// </summary>
        WebApi=8,
        /// <summary>
        /// SFProduces
        /// </summary>
        SFProcedure = 9
    }
    /// <summary>
    /// 事件类型
    /// </summary>
    public enum EventSource
    {
        /// <summary>
        /// 表单
        /// </summary>
        Frm,
        /// <summary>
        /// 流程
        /// </summary>
        Flow,
        /// <summary>
        /// 节点
        /// </summary>
        Node
    }
    /// <summary>
    /// 从表显示模式
    /// </summary>
    public enum ListShowModel
    {
        /// <summary>
        /// 表格模式
        /// </summary>
        Table,
        /// <summary>
        /// 傻瓜表单模式
        /// </summary>
        Card
    }
    /// <summary>
    /// 从表显示方式
    /// </summary>
    public enum EditModel
    {
        /// <summary>
        /// 表格模式
        /// </summary>
        TableModel,
        /// <summary>
        /// 傻瓜表单模式
        /// </summary>
        FoolModel,
        /// <summary>
        /// 自由表单模式
        /// </summary>
        FreeModel
    }
    /// <summary>
    /// 明细表存盘方式
    /// </summary>
    public enum DtlAddRecModel
    {
        /// <summary>
        /// 自动初始化空白行
        /// </summary>
        ByBlank,
        /// <summary>
        /// 用按钮增加行
        /// </summary>
        ByButton
    }
    public enum DtlSaveModel
    {
        /// <summary>
        /// 失去焦点自动存盘
        /// </summary>
        AutoSave,
        /// <summary>
        /// 由保存按钮触发存盘
        /// </summary>
        HandSave
    }
    /// <summary>
    /// 棫行处理
    /// </summary>
    public enum WhenOverSize
    {
        /// <summary>
        /// 不处理
        /// </summary>
        None,
        /// <summary>
        /// 增加一行
        /// </summary>
        AddRow,
        /// <summary>
        /// 翻页
        /// </summary>
        TurnPage
    }
    public enum DtlOpenType
    {
        /// <summary>
        /// 对人员开放
        /// </summary>
        ForEmp=0,
        /// <summary>
        /// 对工作开放
        /// </summary>
        ForWorkID=1,
        /// <summary>
        /// 对流程开放
        /// </summary>
        ForFID=2,
        /// <summary>
        /// 父工作ID
        /// </summary>
        ForPWorkID=3,
        /// <summary>
        /// 按照workid+人员账号字段.
        /// </summary>
        ForWorkIDAndSpecEmpNo=4,
        ForP2WorkID,
        ForP3WorkID,
        /// <summary>
        /// 根流程的WorkID
        /// </summary>
        RootFlowWorkID
    }
    /// <summary>
    /// 明细表工作方式
    /// </summary>
    public enum DtlModel
    {
        /// <summary>
        /// 普通的
        /// </summary>
        Ordinary,
        /// <summary>
        /// 固定列
        /// </summary>
        FixRow
    }
}
