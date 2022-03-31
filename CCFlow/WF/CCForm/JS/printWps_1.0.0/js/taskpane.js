function onbuttonclick(idStr)
{
    if (typeof (wps.Enum) != "object") { // 如果没有内置枚举值
        wps.Enum = WPS_Enum
    }
    switch(idStr)
    {
        case "dockLeft":{
                let tsId = wps.PluginStorage.getItem("taskpane_id")
                if (tsId){
                    let tskpane = wps.GetTaskPane(tsId)
                    tskpane.DockPosition = wps.Enum.msoCTPDockPositionLeft
                }
                break
            }
        case "dockRight":{
            let tsId = wps.PluginStorage.getItem("taskpane_id")
                if (tsId){
                    let tskpane = wps.GetTaskPane(tsId)
                    tskpane.DockPosition = wps.Enum.msoCTPDockPositionRight
                }
                break
        }
        case "hideTaskPane":{
            let tsId = wps.PluginStorage.getItem("taskpane_id")
                if (tsId){
                    let tskpane = wps.GetTaskPane(tsId)
                    tskpane.Visible = false
                }
                break
        }
        case "addString":{
            let doc = wps.WpsApplication().ActiveDocument
            if (doc){
                doc.Range(0, 0).Text="Hello, wps加载项!"
                //好像是wps的bug, 这两句话触发wps重绘
                let rgSel = wps.WpsApplication().Selection.Range
                if (rgSel)
                    rgSel.Select()
            }
            break;
        }
        case "getDocName":{
            let doc = wps.WpsApplication().ActiveDocument
                let textValue = "";
                if (!doc){
                    textValue = textValue + "当前没有打开任何文档"
                    return
                }
                textValue = textValue + doc.Name
                document.getElementById("text_p").innerHTML = textValue
                break
        }
    }
}

window.onload = function () {
    var xmlReq = WpsInvoke.CreateXHR();
    var url = location.origin + "/.debugTemp/NotifyDemoUrl"
    xmlReq.open("GET", url);
    xmlReq.onload = function (res) {
        var node = document.getElementById("DemoSpan");
        node.innerText = res.target.responseText;
    };
    xmlReq.send();
}