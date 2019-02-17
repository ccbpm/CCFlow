
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
    // 按表单选择人员
        DeliveryWay.ByPreviousNodeFormEmpsField = 5,
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
    //按项目组内的岗位计算
        DeliveryWay.ByStationForPrj = 20,
    //由上一节点发送人通过“人员选择器”选择接受人
        DeliveryWay.BySelectedForPrj = 21,

    // 按照ccflow的BPM模式处理
        DeliveryWay.ByCCFlowBPM = 100

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
     StartGuideWay.ByParentFlowModel = 9
}

//表单模式
if (typeof FormType == "undefined") {
    var FormType = {}
    //傻瓜表单
    FormType.FoolForm = 0,
    /// <summary>
    /// 自由表单.
    /// </summary>
     FormType.FreeForm = 1,
    /// <summary>
    /// 嵌入式表单.
    /// </summary>
     FormType.SelfForm = 2,
    /// <summary>
    /// SDKForm
    /// </summary>
     FormType.SDKForm = 3,
    /// <summary>
    /// SL表单
    /// </summary>
     FormType.SLForm = 4,
    /// <summary>
    /// 表单树
    /// </summary>
     FormType.SheetTree = 5,
    /// <summary>
    /// 动态表单树
    /// </summary>
     FormType.SheetAutoTree = 6,
    /// <summary>
    /// 公文表单
    /// </summary>
     FormType.WebOffice = 7,
    /// <summary>
    /// Excel表单
    /// </summary>
     FormType.ExcelForm = 8,
    /// <summary>
    /// Word表单
    /// </summary>
     FormType.WordForm = 9,
    /// <summary>
    /// 傻瓜轨迹表单
    /// </summary>
    FormType.FoolTruck = 10,
    /// <summary>
    /// 表单库的表单
    /// </summary>
    FormType.RefOneFrmTree = 11,
    /// <summary>
    /// 禁用(对多表单流程有效)
    /// </summary>
    FormType.DisableIt = 100
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