
if (typeof RunModel == "undefined") {
    var RunModel = {}
    // 普通
    RunModel.Ordinary = 0,
        // 合流
        RunModel.HL = 1,
        // 分流
        RunModel.FL = 2,
        // 分合流
        RunModel.FHL = 3,
        // 子线程
        RunModel.SubThread = 4
}

function getRunModelName(keyValue) {
    switch (keyValue) {
        case 0:
            return "Ordinary";
        case 1:
            return "HL";
        case 2:
            return "FL";
        case 3:
            return "FHL";
        case 4:
            return "SubThread";
        default:
            return "Ordinary";
    }
}


//投递方式
if (typeof DeliveryWay == "undefined") {
    var DeliveryWay = {}
    // 按岗位(以部门为纬度)
    DeliveryWay.ByStation = 0,
        //在指定的部门里按照岗位计算.
        DeliveryWay.FindSpecDeptEmpsInStationlist = 19,

        // 按部门
        DeliveryWay.ByDept = 1,
        // 按SQL
        DeliveryWay.BySQL = 2,
        // 按本节点绑定的人员
        DeliveryWay.ByBindEmp = 3,
        // 由上一步发送人选择
        DeliveryWay.BySelected = 4,
        //所有人员都可以发起
        DeliveryWay.BySelected_1 = 41,
        // 按表单选择人员
        DeliveryWay.ByPreviousNodeFormEmpsField = 5,
        DeliveryWay.ByPreviousNodeFormDepts = 52,
        //按表单选择岗位
        DeliveryWay.ByPreviousNodeFormStationsAI = 53,
        DeliveryWay.ByPreviousNodeFormStationsOnly = 54,
        //按表单选择部门
        // 与上一节点的人员相同
        DeliveryWay.ByPreviousNodeEmp = 6,
        // 与开始节点的人员相同
        DeliveryWay.ByStarter = 7,
        // 与指定节点的人员相同
        DeliveryWay.BySpecNodeEmp = 8,
        // 按岗位与部门交集计算
        DeliveryWay.ByDeptAndStation = 9,
        // 按岗位计算(以部门集合为纬度)
        DeliveryWay.ByStationAndEmpDept = 10,
        // 按指定节点的人员或者指定字段作为人员的岗位计算
        DeliveryWay.BySpecNodeEmpStation = 11,
        // 按SQL确定子线程接受人与数据源
        DeliveryWay.BySQLAsSubThreadEmpsAndData = 12,
        // 按明细表确定子线程接受人
        DeliveryWay.ByDtlAsSubThreadEmps = 13,
        // 仅按岗位计算
        DeliveryWay.ByStationOnly = 14,
        // FEE计算
        DeliveryWay.ByFEE = 15,
        // 按绑定部门计算,该部门一人处理标识该工作结束(子线程)
        DeliveryWay.BySetDeptAsSubthread = 16,
        // 按SQL模版计算
        DeliveryWay.BySQLTemplate = 17,
        // 从人员到人员
        DeliveryWay.ByFromEmpToEmp = 18,
        //找本部门范围内的岗位集合里面的人员
        DeliveryWay.FindSpecDeptEmps = 19,
        //按项目组内的岗位计算
        DeliveryWay.ByStationForPrj = 20,
        //由上一节点发送人通过“人员选择器”选择接受人
        DeliveryWay.BySelectedForPrj = 21,
        DeliveryWay.BySelectedOrgs = 22,
        //找部门负责人
        DeliveryWay.ByDeptLeader = 23,
        //找直属领导.
        DeliveryWay.ByEmpLeader = 50,
        //找分管领导 - ShipLeader ccflow不负责维护.
        DeliveryWay.ByDeptShipLeader = 28,
        //按照用户组计算(全集团)
        DeliveryWay.ByTeamOrgOnly = 24,
        //仅按用户组计算.
        DeliveryWay.ByTeamOnly = 25,
        // 按照用户组计算(本部门范围内)
        DeliveryWay.ByTeamDeptOnly = 26,
        //按照集团模式的选择人接收器. 2020.06 for xinxizhongxin.
        DeliveryWay.BySelectedEmpsOrgModel = 43,
        //按照自定义url.
        DeliveryWay.BySelfUrl = 44,
        //按API/URL
        DeliveryWay.ByAPIUrl = 45,
        //发送人的上级部门的负责人
        DeliveryWay.BySenderParentDeptLeader = 46,
        //发送人上级部门指定的岗位
        DeliveryWay.BySenderParentDeptStations = 47,

        //外部用户可以发起
        DeliveryWay.ByGuest = 51,

        // 按照ccflow的BPM模式处理
        DeliveryWay.ByCCFlowBPM = 100
}


//选择人接受范围限定规则.
if (typeof SelectorModel == "undefined") {
    var SelectorModel = {}
    SelectorModel.Station = 0,
        SelectorModel.Dept = 1,
        SelectorModel.Emp = 2,
        SelectorModel.SQL = 3,
        SelectorModel.SQLTemplate = 4,
        SelectorModel.GenerUserSelecter = 5,
        SelectorModel.DeptAndStation = 6,
        SelectorModel.Url = 7,
        SelectorModel.AccepterOfDeptStationEmp = 8,
        SelectorModel.AccepterOfDeptStationOfCurrentOper = 9,
        SelectorModel.TeamOrgOnly = 10,
        SelectorModel.TeamOnly = 11,
        SelectorModel.TeamDeptOnly = 12,
        SelectorModel.ByStationAI = 13,
        SelectorModel.ByWebAPI = 14,
        SelectorModel.ByMyDeptEmps = 15




}
//发送阻塞规则.
if (typeof BlockModel == "undefined") {
    var BlockModel = {}
    /// <summary>
    /// 不阻塞
    /// </summary>
    BlockModel.None = 0,
        /// <summary>
        /// 当前节点的有未完成的子线程
        /// </summary>
        BlockModel.CurrNodeAll = 1,
        /// <summary>
        /// 按照约定的格式阻塞.
        /// </summary>
        BlockModel.SpecSubFlow = 2,
        /// <summary>
        /// 按照配置的sql阻塞,返回大于等于1表示阻塞,否则不阻塞.
        /// </summary>
        BlockModel.BySQL = 3,
        /// <summary>
        /// 按照表达式阻塞，表达式类似方向条件的表达式.
        /// </summary>
        BlockModel.ByExp = 4,
        /// <summary>
        /// 为父流程时，指定的子流程未运行到指定节点，则阻塞
        /// </summary>
        BlockModel.SpecSubFlowNode = 5,
        /// <summary>
        /// 为平级子流程时，指定的子流程未运行到指定节点，则阻塞
        /// </summary>
        BlockModel.SameLevelSubFlow = 6,
        /// <summary>
        /// 其他选项设置
        /// </summary>
        BlockModel.ByOtherBlock = 7
}
//流程设计模式.
if (typeof FlowDevModel == "undefined") {
    var FlowDevModel = {}
    /// <summary>
    /// 专业模式
    /// </summary>
    FlowDevModel.Prefessional = 0,
        /// <summary>
        /// 极简模式
        /// </summary>
        FlowDevModel.JiJian = 1,
        /// <summary>
        /// 累加模式
        /// </summary>
        FlowDevModel.FoolTruck = 2,
        /// <summary>
        /// 绑定单表单
        /// </summary>
        FlowDevModel.RefOneFrmTree = 3,
        /// <summary>
        /// 绑定多表单
        /// </summary>
        FlowDevModel.FrmTree = 4,
        /// <summary>
        /// SDK表单
        /// </summary>
        FlowDevModel.SDKFrm = 5,
        /// <summary>
        /// 嵌入式表单
        /// </summary>
        FlowDevModel.SelfFrm = 6,
        /// <summary>
        /// 物联网流程
        /// </summary>
        FlowDevModel.InternetOfThings = 7,
        /// <summary>
        /// 决策树流程
        /// </summary>
        FlowDevModel.Tree = 8
}
//多人处理规则.
if (typeof TodolistModel == "undefined") {
    var TodolistModel = {}
    /// <summary>
    /// 抢办(谁抢到谁来办理,办理完后其他人就不能办理.)
    /// </summary>
    TodolistModel.QiangBan = 0,
        /// <summary>
        /// 协作(没有处理顺序，接受的人都要去处理,由最后一个人发送到下一个节点)
        /// </summary>
        TodolistModel.Teamup = 1,
        /// <summary>
        ///  队列(按照顺序处理，有最后一个人发送到下一个节点)
        /// </summary>
        TodolistModel.Order = 2,
        /// <summary>
        /// 共享模式(需要申请，申请后才能执行)
        /// </summary>
        TodolistModel.Sharing = 3,
        /// <summary>
        /// 协作组长模式
        /// </summary>
        TodolistModel.TeamupGroupLeader = 4

}
//考核规则.
if (typeof CHWay == "undefined") {
    var CHWay = {}
    /// <summary>
    /// 不考核
    /// </summary>
    CHWay.None = 0,
        /// <summary>
        /// 按照时效考核
        /// </summary>
        CHWay.ByTime = 1,
        /// <summary>
        /// 按工作量考核
        /// </summary>
        CHWay.ByWorkNum = 2,
        /// <summary>
        /// 是否是考核质量点
        /// </summary>
        CHWay.IsQuality = 3

}
//超时处理规则.
if (typeof OvertimeRole == "undefined") {
    var OvertimeRole = {}
    /// <summary>
    /// 不设置
    /// </summary>
    OvertimeRole.None = 0,
        /// <summary>
        /// 自动向下运动
        /// </summary>
        OvertimeRole.AutoDown = 1,
        /// <summary>
        /// 跳转到指定节点
        /// </summary>
        OvertimeRole.JumpToNode = 2,
        /// <summary>
        /// 移交给指定的人员
        /// </summary>
        OvertimeRole.TurnToEmp = 3,
        /// <summary>
        /// 给指定的人员发送消息
        /// </summary>
        OvertimeRole.SendMessageToEmp = 4,
        /// <summary>
        /// 删除流程
        /// </summary>
        OvertimeRole.DeleteFlow = 5,
        /// <summary>
        /// 执行SQL
        /// </summary>
        OvertimeRole.RunSql = 6
}

//批处理规则
if (typeof BatchRole == "undefined") {
    var BatchRole = {}
    /// <summary>
    /// 不处理
    /// </summary>
    BatchRole.None = 0,
        /// <summary>
        /// 审核组件模式
        /// </summary>
        BatchRole.WorkCheckModel = 1,
        /// <summary>
        /// 审核字段分组模式
        /// </summary>
        BatchRole.Group = 2
}

//发送后转向
if (typeof TurnToDeal == "undefined") {
    var TurnToDeal = {}
    /// <summary>
    /// 提示CCFlow默认信息
    /// </summary>
    TurnToDeal.CCFlowMsg = 0,
        /// <summary>
        /// 提示指定信息
        /// </summary>
        TurnToDeal.SpecMsg = 1,
        /// <summary>
        /// 转向指定的URL
        /// </summary>
        TurnToDeal.SpecUrl = 2,
        /// <summary>
        /// 发送后关闭
        /// </summary>
        TurnToDeal.TurntoClose = 3,
        /// <summary>
        /// 按条件转向
        /// </summary>
        TurnToDeal.TurnToByCond = 4
}

//导入
if (typeof Imp == "undefined") {
    var Imp = {}
    /// <summary>
    /// 本地导入
    /// </summary>
    Imp.localhostImp = 0,
        /// <summary>
        /// 节点表单导入
        /// </summary>
        Imp.NodeFrmImp = 1,
        /// <summary>
        /// 其他流程导入
        /// </summary>
        Imp.FlowFrmImp = 2,
        /// <summary>
        /// 表单库导入
        /// </summary>
        Imp.FrmLibraryImp = 4,
        /// <summary>
        /// 外部数据源导入
        /// </summary>
        Imp.ExternalDataSourseImp = 5,
        /// <summary>
        /// 导出表单模板
        /// </summary>
        Imp.ExportFrm = 6,
        /// <summary>
        /// WebAPI导入
        /// </summary>
        Imp.WebAPIImp = 7
}
//方向条件控制
if (typeof DirCondModel == "undefined") {
    var DirCondModel = {}
    /// <summary>
    /// 按照方向条件计算的
    /// </summary>
    DirCondModel.ByLineCond = 0,
        /// <summary>
        /// 主观选择：下拉框模式
        /// </summary>
        DirCondModel.ByDDLSelected = 2,
        /// <summary>
        /// 由连接线控制
        /// </summary>
        DirCondModel.ByPopSelect = 1,
        /// <summary>
        /// 主观选择： 按钮模式
        /// </summary>
        DirCondModel.ByButtonSelected = 3
}

//流程计划时间
if (typeof SDTOfFlow == "undefined") {
    var SDTOfFlow = {}
    /// <summary>
    /// 不使用
    /// </summary>
    SDTOfFlow.None = 0,
        /// <summary>
        /// 按照节点表单的日期计算
        /// </summary>
        SDTOfFlow.NodeFrmDT = 1,
        /// <summary>
        /// 按照sql计算
        /// </summary>
        SDTOfFlow.SQLDT = 2,
        /// <summary>
        /// 按照所有节点的时间之和计算
        /// </summary>
        SDTOfFlow.NodeSumDT = 3,
        /// <summary>
        /// 按照规定的天数计算
        /// </summary>
        SDTOfFlow.DaysDT = 4
    ///// <summary>
    ///// 按照时间规则计算
    ///// </summary>
    //SDTOfFlow.TimeDT = 5,
    ///// <summary>
    ///// 为子流程时的规则
    ///// </summary>
    //SDTOfFlow.ChildFlowDT = 6,
    ///// <summary>
    ///// 按照发起字段不能重复规则
    ///// </summary>
    //SDTOfFlow.AttrNonredundant = 7

}
//发起限制规则
if (typeof StartLimitRole == "undefined") {
    var StartLimitRole = {}
    /// <summary>
    /// 不限制
    /// </summary>
    StartLimitRole.None = 0,
        /// <summary>
        /// 一人一天一次
        /// </summary>
        StartLimitRole.Day = 1,
        /// <summary>
        /// 一人一周一次
        /// </summary>
        StartLimitRole.Week = 2,
        /// <summary>
        /// 一人一月一次
        /// </summary>
        StartLimitRole.Month = 3,
        /// <summary>
        /// 一人一季度一次
        /// </summary>
        StartLimitRole.JD = 4,
        /// <summary>
        /// 一人一年一次
        /// </summary>
        StartLimitRole.Year = 5,
        /// <summary>
        /// 发起的列不能重复,(多个列可以用逗号分开)
        /// </summary>
        StartLimitRole.ColNotExit = 6,
        /// <summary>
        /// 设置的SQL数据源为空,或者返回结果为零时可以启动.
        /// </summary>
        StartLimitRole.ResultIsZero = 7,
        /// <summary>
        /// 设置的SQL数据源为空,或者返回结果为零时不可以启动.
        /// </summary>
        StartLimitRole.ResultIsNotZero = 8,
        /// <summary>
        /// 为子流程时仅仅只能被调用1次.
        /// </summary>
        StartLimitRole.OnlyOneSubFlow = 9
}
//自动发起
if (typeof AutoStart == "undefined") {
    var AutoStart = {}
    /// <summary>
    /// 手工启动（默认）
    /// </summary>
    AutoStart.None = 0,
        /// <summary>
        /// 按照指定的人员
        /// </summary>
        AutoStart.ByDesignee = 1,
        /// <summary>
        /// 数据集按时启动
        /// </summary>
        AutoStart.ByTineData = 2,
        /// <summary>
        /// 触发试启动
        /// </summary>
        AutoStart.ByTrigger = 3,
        AutoStart.ByDesigneeAdv = 4,
        AutoStart.ByDesigneeAdminSendTo02Node=5

}
//前置导航
if (typeof StartGuideWay == "undefined") {
    var StartGuideWay = {}
    //傻瓜表单
    StartGuideWay.None = 0,
        /// <summary>
        /// SQL单条模式.
        /// </summary>
        StartGuideWay.BySQLOne = 1,
        /// <summary>
        /// 按系统的URL-(子父流程)多条模式.
        /// </summary>
        StartGuideWay.SubFlowGuide = 2,
        /// <summary>
        /// 按系统的URL-(实体记录)单条模式
        /// </summary>
        StartGuideWay.BySystemUrlOneEntity = 3,
        /// <summary>
        /// 按系统的URL-(实体记录)多条模式
        /// </summary>
        StartGuideWay.SubFlowGuideEntity = 4,
        /// <summary>
        /// 历史数据
        /// </summary>
        StartGuideWay.ByHistoryUrl = 5,
        /// <summary>
        /// SQL多条模式
        /// </summary>
        StartGuideWay.BySQLMulti = 6,
        /// <summary>
        /// 按自定义的Url
        /// </summary>
        StartGuideWay.BySelfUrl = 7,
        /// <summary>
        /// 按照用户选择的表单
        /// </summary>
        StartGuideWay.ByFrms = 8,
        /// <summary>
        /// 父子流程模式
        /// </summary>
        StartGuideWay.ByParentFlowModel = 9,
        /// <summary>
        /// 子流程实例列表模式-多条
        /// </summary>
        StartGuideWay.ByChildFlowModel = 10
}


//流程表单模式
if (typeof FlowFrmType == "undefined") {
    var FlowFrmType = {}
    //完整版-2019年更早版本
    FlowFrmType.Ver2019Earlier = 0,
        /// <summary>
        /// 开发者表单.
        /// </summary>
        FlowFrmType.DeveloperFrm = 1,
        /// <summary>
        /// 傻瓜表单.
        /// </summary>
        FlowFrmType.FoolFrm = 2,
        /// <summary>
        /// 自定义(嵌入)表单
        /// </summary>
        FlowFrmType.SelfFrm = 3,
        /// <summary>
        /// SDK表单
        /// </summary>
        FlowFrmType.SDKFrm = 4
}


//表单应用类型.
if (typeof EntityType == "undefined") {
    var EntityType = {}
    //独立表单
    EntityType.SingleFrm = 0,
        /// <summary>
        /// 单据.
        /// </summary>
        EntityType.FrmBill = 1,
        /// <summary>
        /// 实体.
        /// </summary>
        EntityType.FrmDict = 2,
        /// <summary>
        ///   实体树
        /// </summary>
        EntityType.EntityTree = 3
}



//表单类型
if (typeof FrmType == "undefined") {
    var FrmType = {}
    //傻瓜表单
    FrmType.FoolForm = 0,
        /// <summary>
        /// 自由表单.
        /// </summary>
        FrmType.FreeForm = 1,
        /// <summary>
        /// Url.
        /// </summary>
        FrmType.Url = 3,
        /// <summary>
        ///     Word类型表单
        /// </summary>
        FrmType.WordFrm = 4,
        /// <summary>
        /// VSTOExccel模式
        /// </summary>
        FrmType.VSTOForExcel = 6,
        /// <summary>
        /// 实体类
        /// </summary>
        FrmType.SheetAutoTree = 7,
        /// <summary>
        /// 开发者表单
        /// </summary>
        FrmType.Develop = 8
}

//节点表单方案
if (typeof FormSlnType == "undefined") {
    var FormSlnType = {}
    //傻瓜表单
    FormSlnType.FoolForm = 0,
        /// <summary>
        /// 自由表单.
        /// </summary>
        FormSlnType.FreeForm = 1,
        /// <summary>
        /// 嵌入式表单.
        /// </summary>
        FormSlnType.SelfForm = 2,
        /// <summary>
        /// SDKForm
        /// </summary>
        FormSlnType.SDKForm = 3,
        /// <summary>
        /// SL表单
        /// </summary>
        FormSlnType.SLForm = 4,
        /// <summary>
        /// 表单树
        /// </summary>
        FormSlnType.SheetTree = 5,
        /// <summary>
        /// 动态表单树
        /// </summary>
        FormSlnType.SheetAutoTree = 6,
        /// <summary>
        /// 公文表单
        /// </summary>
        FormSlnType.WebOffice = 7,
        /// <summary>
        /// Excel表单
        /// </summary>
        FormSlnType.ExcelForm = 8,
        /// <summary>
        /// Word表单
        /// </summary>
        FormSlnType.WordForm = 9,
        /// <summary>
        /// 傻瓜轨迹表单
        /// </summary>
        FormSlnType.FoolTruck = 10,
        /// <summary>
        /// 表单库的表单
        /// </summary>
        FormSlnType.RefOneFrmTree = 11,
        /// <summary>
        /// 开发者表单
        /// </summary>
        FormSlnType.Developer = 12,
        /// <summary>
        /// 只能SDK表单
        /// </summary>
        FormSlnType.SDKFormSmart = 13,
        /// <summary>
        /// 禁用(对多表单流程有效)
        /// </summary>
        FormSlnType.DisableIt = 100
}
/// 公文工作模式
if (typeof WebOfficeWorkModel == "undefined") {
    var WebOfficeWorkModel = {}

    /// <summary>
    /// 不启用
    /// </summary>
    WebOfficeWorkModel.None = 0,
        /// <summary>
        /// 按钮方式启用
        /// </summary>
        WebOfficeWorkModel.Button = 1,
        /// <summary>
        /// 表单在前
        /// </summary>
        WebOfficeWorkModel.FrmFirst = 2,
        /// <summary>
        /// 文件在前
        /// </summary>
        WebOfficeWorkModel.WordFirst = 3
}

///条件数据源
if (typeof ConnDataFrom == "undefined") {
    var ConnDataFrom = {}
    /// <summary>
    /// 表单数据
    /// </summary>
    ConnDataFrom.NodeForm = 0,
        /// <summary>
        /// 岗位数据
        /// </summary>
        ConnDataFrom.Stas = 1,
        /// <summary>
        /// Depts
        /// </summary>
        ConnDataFrom.Depts = 2,
        /// <summary>
        /// 按sql计算.
        /// </summary>
        ConnDataFrom.SQL = 3,
        /// <summary>
        /// 按参数
        /// </summary>
        ConnDataFrom.Paras = 4,
        /// <summary>
        /// 按Url.
        /// </summary>
        ConnDataFrom.Url = 5,
        /// <summary>
        /// 按sql模版计算.
        /// </summary>
        ConnDataFrom.SQLTemplate = 6,
        /// <summary>
        /// 独立表单
        /// </summary>
        ConnDataFrom.StandAloneFrm = 7
}
/// <summary>
/// 条件类型
/// </summary>
if (typeof CondType == "undefined") {
    var CondType = {}
    /// <summary>
    /// 节点完成条件
    /// </summary>
    CondType.Node = 0,
        /// <summary>
        /// 流程条件
        /// </summary>
        CondType.Flow = 1,
        /// <summary>
        /// 方向条件
        /// </summary>
        CondType.Dir = 2,
        /// <summary>
        /// 启动子流程
        /// </summary>
        CondType.SubFlow = 3
}

//
if (typeof RefMethodType == "undefined") {
    var RefMethodType = {}
    /// <summary>
    /// 功能
    /// </summary>
    RefMethodType.Func = 0,
        /// <summary>
        /// 模态窗口打开
        /// </summary>
        RefMethodType.LinkModel = 1,
        /// <summary>
        /// 新窗口打开
        /// </summary>
        RefMethodType.LinkeWinOpen = 2,
        /// <summary>
        /// 右侧窗口打开
        /// </summary>
        RefMethodType.RightFrameOpen = 3
}
/// <summary>
/// 文件展现方式
/// </summary>
if (typeof FileShowWay == "undefined") {
    var FileShowWay = {}
    /// <summary>
    /// 表格
    /// </summary>
    FileShowWay.Table = 0,
        /// <summary>
        /// 图片
        /// </summary>
        FileShowWay.Pict = 1,
        /// <summary>
        /// 自由模式
        /// </summary>
        FileShowWay.Free = 2
}

/// <summary>
/// 附件删除规则
/// </summary>
if (typeof AthDeleteWay == "undefined") {
    var AthDeleteWay = {}
    /// <summary>
    /// 不删除 0
    /// </summary>
    AthDeleteWay.None = 0,
        /// <summary>
        /// 删除所有 1
        /// </summary>
        AthDeleteWay.DelAll = 1,
        /// <summary>
        /// 只删除自己上传 2
        /// </summary>
        AthDeleteWay.DelSelf = 2
}


/// <summary>
/// 子流程启动模式
/// </summary>
if (typeof SubFlowStartModel == "undefined") {
    var SubFlowStartModel = {}
    /// <summary>
    /// 单独启动 0
    /// </summary>
    SubFlowStartModel.Single = 0,
        /// <summary>
        /// 简单列表模式批量发起 1
        /// </summary>
        SubFlowStartModel.Simple = 1,
        /// <summary>
        /// 分组模式 2
        /// </summary>
        SubFlowStartModel.Group = 2,
        //树模式.
        SubFlowStartModel.Tree = 3
}

/// <summary>
/// 子流程显示模式
/// </summary>
if (typeof SubFlowShowModel == "undefined") {
    var SubFlowShowModel = {}
    /// <summary>
    /// 表格模式 0
    /// </summary>
    SubFlowShowModel.Table = 0,
        /// <summary>
        /// 列表模式 1
        /// </summary>
        SubFlowShowModel.List = 1
}

/// <summary>
/// 表单启用规则
/// </summary>
if (typeof BindFrmsNodeEnableRole == "undefined") {
    var BindFrmsNodeEnableRole = {}
    /// <summary>
    /// 表格模式 0
    /// </summary>
    BindFrmsNodeEnableRole.None = 0,
        /// <summary>
        /// 列表模式 1
        /// </summary>
        BindFrmsNodeEnableRole.ByData = 1,
        BindFrmsNodeEnableRole.ByPara = 1,
        BindFrmsNodeEnableRole.ByExp = 1,
        BindFrmsNodeEnableRole.BySQL = 1,
        BindFrmsNodeEnableRole.ByData = 1,
        BindFrmsNodeEnableRole.ByDat1a = 1,
        BindFrmsNodeEnableRole.ByDa3ta = 1
}

if (typeof TemplateFileModel == "undefined") {
    var TemplateFileModel = {}
    /// <summary>
    /// 旧版本的rtf模版格式
    /// </summary>
    TemplateFileModel.RTF = 0,
    /// <summary>
    /// VSTo的Word模板方式
   /// </summary>
    TemplateFileModel.VSTOForWord = 1,
    /// <summary>
    /// VSTO的Excel模板方式
    /// </summary>
    TemplateFileModel.VSTOForExcel = 2,
   /// <summary>
    /// WPS的模板方式
    /// </summary>
    TemplateFileModel.WPS = 3
}



