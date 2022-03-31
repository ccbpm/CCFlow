
//这个函数在整个wps加载项中是第一个执行的
function OnAddinLoad(ribbonUI){
    if (typeof (wps.ribbonUI) != "object"){
		wps.ribbonUI = ribbonUI
    }
    
    if (typeof (wps.Enum) != "object") { // 如果没有内置枚举值
        wps.Enum = WPS_Enum
    }

    wps.PluginStorage.setItem("EnableFlag", false) //往PluginStorage中设置一个标记，用于控制两个按钮的置灰
    wps.PluginStorage.setItem("ApiEventFlag", false) //往PluginStorage中设置一个标记，用于控制ApiEvent的按钮label
    return true
}

var WebNotifycount = 0;
function OnAction(control) {
    const eleId = control.Id
    switch (eleId) {
        case "btnShowMsg":
            {
                const doc = wps.WpsApplication().ActiveDocument
                if (!doc) {
                    alert("当前没有打开任何文档")
                    return
                }
                alert(doc.Name)
            }
            break;
        case "btnIsEnbable":
            {
                let bFlag = wps.PluginStorage.getItem("EnableFlag")
                wps.PluginStorage.setItem("EnableFlag", !bFlag)
                
                //通知wps刷新以下几个按饰的状态
                wps.ribbonUI.InvalidateControl("btnIsEnbable")
                wps.ribbonUI.InvalidateControl("btnShowDialog") 
                wps.ribbonUI.InvalidateControl("btnShowTaskPane") 
                //wps.ribbonUI.Invalidate(); 这行代码打开则是刷新所有的按钮状态
                break
            }
        case "btnShowDialog":
            wps.ShowDialog(GetUrlPath() + "/ui/dialog.html", "这是一个对话框网页", 400 * window.devicePixelRatio, 400 * window.devicePixelRatio, false)
            break
        case "btnShowTaskPane":
            {
                let tsId = wps.PluginStorage.getItem("taskpane_id")
                if (!tsId) {
                    let tskpane = wps.CreateTaskPane(GetUrlPath() + "/ui/taskpane.html")
                    let id = tskpane.ID
                    wps.PluginStorage.setItem("taskpane_id", id)
                    tskpane.Visible = true
                } else {
                    let tskpane = wps.GetTaskPane(tsId)
                    tskpane.Visible = !tskpane.Visible
                }
            }
            break
        case "btnApiEvent":
            {
                let bFlag = wps.PluginStorage.getItem("ApiEventFlag")
                let bRegister = bFlag ? false : true
                wps.PluginStorage.setItem("ApiEventFlag", bRegister)
                if (bRegister){
                    wps.ApiEvent.AddApiEventListener('DocumentNew', OnNewDocumentApiEvent)
                }
                else{
                    wps.ApiEvent.RemoveApiEventListener('DocumentNew', OnNewDocumentApiEvent)
                }

                wps.ribbonUI.InvalidateControl("btnApiEvent") 
            }
            break
        case "btnWebNotify":
            {
                let currentTime = new Date()
                let timeStr = currentTime.getHours() + ':' + currentTime.getMinutes() + ":" + currentTime.getSeconds()
                wps.OAAssist.WebNotify("这行内容由wps加载项主动送达给业务系统，可以任意自定义, 比如时间值:" + timeStr + "，次数：" + (++WebNotifycount), true)
            }
            break
        default:
            break
    }
    return true
}

function GetImage(control) {
    const eleId = control.Id
    switch (eleId) {
        case "btnShowMsg":
            return "images/1.svg"
        case "btnShowDialog":
            return "images/2.svg"
        case "btnShowTaskPane":
            return "images/3.svg"
        default:
            ;
    }
    return "images/newFromTemp.svg"
}

function OnGetEnabled(control) {
    const eleId = control.Id
    switch (eleId) {
        case "btnShowMsg":
            return true
            break
        case "btnShowDialog":
            {
                let bFlag = wps.PluginStorage.getItem("EnableFlag")
                return bFlag
                break
            }
        case "btnShowTaskPane":
            {
                let bFlag = wps.PluginStorage.getItem("EnableFlag")
                return bFlag
                break
            }
        default:
            break
    }
    return true
}

function OnGetVisible(control){
    return true
}

function OnGetLabel(control){
    const eleId = control.Id
    switch (eleId) {
        case "btnIsEnbable":
        {
            let bFlag = wps.PluginStorage.getItem("EnableFlag")
            return bFlag ?  "按钮Disable" : "按钮Enable"
            break
        }
        case "btnApiEvent":
        {
            let bFlag = wps.PluginStorage.getItem("ApiEventFlag")
            return bFlag ? "清除新建文件事件" : "注册新建文件事件"
            break
        }    
    }
    return ""
}

function OnNewDocumentApiEvent(doc){
    alert("新建文件事件响应，取文件名: " + doc.Name)
}
