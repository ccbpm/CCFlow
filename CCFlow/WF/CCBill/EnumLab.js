
// 方法的模式:. 就是什么类型的方法.
if (typeof MethodModel == "undefined") {

    var MethodModel = {}

    MethodModel.Link = "Link", //链接或者按钮.
        MethodModel.Func = "Func", //方法.
        MethodModel.Bill = "Bill", //单据.

        MethodModel.FlowBaseData = "FlowBaseData", //修改基础数据流程.
        MethodModel.FlowNewEntity = "FlowNewEntity", //新建实体流程.
        MethodModel.FlowEntityBatchStart = "FlowEntityBatchStart", //实体批量发起.
        //其他业务流程.
        MethodModel.FlowEtc = "FlowEtc",

        //单个实体,历史流程查询
        MethodModel.SingleDictGenerWorkFlows = "SingleDictGenerWorkFlows",
        //审核评论分析组件.
        MethodModel.FrmBBS = "FrmBBS",
        ///从文件里导入数据.
        MethodModel.ImpFromFile = "ImpFromFile",
        ///操作日志.
        MethodModel.DictLog = "DictLog",
        ///实体二维码.
        MethodModel.QRCode = "QRCode",
        //数据快照
        MethodModel.DataVer = "DataVer"
}

// 方法的执行窗口的类型:  就是什么类型的方法.
if (typeof RefMethodType == "undefined") {
    var RefMethodType = {}
    //功能
    RefMethodType.Func = 0,
    // 模态窗口打开
    RefMethodType.LinkModel = 1,
    // 新窗口打开
    RefMethodType.LinkeWinOpen = 2,
    // 右侧窗口打开
    RefMethodType.RightFrameOpen = 3,
    //Tab页签打开
    RefMethodType.TabOpen =4
}


