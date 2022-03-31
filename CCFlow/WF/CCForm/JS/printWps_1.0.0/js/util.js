//在后续的wps版本中，wps的所有枚举值都会通过wps.Enum对象来自动支持，现阶段先人工定义
var WPS_Enum = {
    msoCTPDockPositionLeft: 0,
    msoCTPDockPositionRight: 2
}

/**
 * WPS加载项自定义的枚举值
 */
var constStrEnum = {
    AllowOADocReOpen: "AllowOADocReOpen",
    AutoSaveToServerTime: "AutoSaveToServerTime",
    bkInsertFile: "bkInsertFile",
    buttonGroups: "buttonGroups",
    CanSaveAs: "CanSaveAs",
    copyUrl: "copyUrl",
    DefaultUploadFieldName: "DefaultUploadFieldName",
    disableBtns: "disableBtns",
    insertFileUrl: "insertFileUrl",
    IsInCurrOADocOpen: "IsInCurrOADocOpen",
    IsInCurrOADocSaveAs: "IsInCurrOADocSaveAs",
    isOA: "isOA",
    notifyUrl: "notifyUrl",
    OADocCanSaveAs: "OADocCanSaveAs",
    OADocLandMode: "OADocLandMode",
    OADocUserSave: "OADocUserSave",
    openType: "openType",
    picPath: "picPath",
    picHeight: "picHeight",
    picWidth: "picWidth",
    redFileElement: "redFileElement",
    revisionCtrl: "revisionCtrl",
    ShowOATabDocActive: "ShowOATabDocActive",
    SourcePath: "SourcePath",
    /**
     * 保存文档到业务系统服务端时，另存一份其他格式到服务端，其他格式支持：.pdf .ofd .uot .uof
     */
    suffix: "suffix",
    templateDataUrl: "templateDataUrl",
    TempTimerID: "TempTimerID",
    /**
     * 文档上传到业务系统的保存地址：服务端接收文件流的地址
     */
    uploadPath: "uploadPath",
    /**
     * 文档上传到服务端后的名称
     */
    uploadFieldName: "uploadFieldName",
    /**
     * 文档上传时的名称，默认取当前活动文档的名称
     */
    uploadFileName: "uploadFileName",
    uploadAppendPath: "uploadAppendPath",
    /**
     * 标志位： 1 在保存到业务系统时再保存一份suffix格式的文档， 需要和suffix参数配合使用
     */
    uploadWithAppendPath: "uploadWithAppendPath",
    userName: "userName",
    WPSInitUserName: "WPSInitUserName",
    taskpaneid: "taskpaneid",
    /**
     * 是否弹出上传前确认和成功后的确认信息：true|弹出，false|不弹出
     */
    Save2OAShowConfirm: "Save2OAShowConfirm",
    /**
     * 修订状态标志位
     */
    RevisionEnableFlag: "RevisionEnableFlag"
}

function GetUrlPath() {
    let e = document.location.toString()
    return -1 != (e = decodeURI(e)).indexOf("/") && (e = e.substring(0, e.lastIndexOf("/"))), e
}
/**
 * 通过wps提供的接口执行一段脚本
 * @param {*} param 需要执行的脚本
 */
function shellExecuteByOAAssist(param) {
    if (wps != null) {
        wps.OAAssist.ShellExecute(param)
    }
}

