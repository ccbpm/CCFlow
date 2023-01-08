/**
 * 初始化按钮的操作
 */
function ToolBar_Init(entityType) {
    var handler = new HttpHandler("BP.CCBill.WF_CCBill_API");
    handler.AddPara("FrmID", GetQueryString("FrmID"));
    handler.AddPara("IsReadonly", GetQueryString("IsReadonly"));
    handler.AddPara("IsMobile", 0);
    var data = handler.DoMethodReturnString("CCFrom_ToolBar_Init");
    if (data.indexOf("err@") != -1) {
        layer.alert(data);
        return;
    }
    data = JSON.parse(data);
    if (data.length == 0) {
        $("#ToolBar").parent().hide();
        $(".layui-fluid").css("padding-top", "15px");
        return;
    }
    var getTpl = document.getElementById("view").innerHTML
        , view = document.getElementById('ToolBar');
    layui.laytpl(getTpl).render(data, function (html) {
        view.innerHTML = html;
    });

    //解析事件.
    $("#ToolBar .layui-btn").on('click', function () {
        var methodNo = GetQueryString("MethodNo");
        methodNo = methodNo == null || methodNo == undefined ? "" : methodNo;
        var pworkid = GetQueryString("PWorkID");
        pworkid = pworkid == null || pworkid == undefined ? 0 : pworkid;
        var fromPage = GetQueryString("From");
        fromPage = fromPage == null || fromPage == undefined ? "" : fromPage;
        switch (this.name) {
            case "New":
                if (methodNo != "" && pworkid != 0) {
                    SetHref("./Opt/GotoLink.htm?FrmID=" + GetQueryString("FrmID") + "&MethodNo=" + methodNo + "&WorkID=" + pworkid + "&DoType=Bill");
                    return;
                }
                var handler = new HttpHandler("BP.CCBill.WF_CCBill");
                handler.AddUrlData();
                var data = "";
                if (entityType == 1)
                    data = handler.DoMethodReturnString("MyBill_CreateBlankBillID");
                else
                    data = handler.DoMethodReturnString("MyDict_CreateBlankDictID");
                if (data != null && data != undefined && data.indexOf('err@') > 0) {
                    layer.alert(data);
                    return;
                }
                if (entityType == 1)
                    SetHref('MyBill.htm?FrmID=' + GetQueryString("FrmID") + "&WorkID=" + data);
                else
                    SetHref('MyDict.htm?FrmID=' + GetQueryString("FrmID") + "&WorkID=" + data);

                break;
            case "Save":
                //保存从表数据
                $("[name=Dtl]").each(function (i, obj) {
                    var contentWidow = obj.contentWindow;
                    if (contentWidow != null && contentWidow.SaveAll != undefined && typeof (contentWidow.SaveAll) == "function") {
                        contentWidow.SaveAll();
                    }
                });
                if (checkBlanks() == false) {
                    layer.alert("必填项不能为空");
                    return;
                }
                //保存信息
                layui.form.on('submit(Save)', function (data) {
                    this.innerHTML = "<i class='iconfont icon-baocun'></i>正在保存";
                    var formData = getFormData(data.field);
                    var handler = new HttpHandler("BP.CCBill.WF_CCBill");
                    for (var key in formData) {
                        handler.AddPara(key, encodeURIComponent(formData[key]));
                    }
                    handler.AddUrlData();
                    //保存前事件
                    if (typeof beforeSave != 'undefined' && beforeSave() instanceof Function)
                        if (beforeSave() == false)
                            return false;

                    var data = "";
                    if (entityType == 1)
                        data = handler.DoMethodReturnString("MyBill_SaveIt");
                    else
                        data = handler.DoMethodReturnString("MyDict_SaveIt");
                    this.innerHTML = "<i class='iconfont icon-baocun'></i>保存";
                    if (data.indexOf("err@") != -1) {
                        layer.alert(data);
                        return false;
                    }
                    layer.alert("保存成功");
                    if (typeof isSaveAfterCloseOfBill != "undefined" && isSaveAfterCloseOfBill == true) {
                        var index = parent.parent.layer.getFrameIndex(window.parent.name);
                        parent.parent.layer.close(index);
                    }

                    return false;
                })
                break;
            case "Submit":
                //保存从表数据
                $("[name=Dtl]").each(function (i, obj) {
                    var contentWidow = obj.contentWindow;
                    if (contentWidow != null && contentWidow.SaveAll != undefined && typeof (contentWidow.SaveAll) == "function") {
                        contentWidow.SaveAll();
                    }
                });
                if (checkBlanks() == false) {
                    layer.alert("必填项不能为空");
                    return;
                }
                //保存信息
                layui.form.on('submit(Submit)', function (data) {
                    this.innerHTML = "<i class='iconfont icon-baocun'></i>正在提交";
                    var formData = getFormData(data.field);
                    var handler = new HttpHandler("BP.CCBill.WF_CCBill");
                    for (var key in formData) {
                        handler.AddPara(key, encodeURIComponent(formData[key]));
                    }
                    handler.AddUrlData();
					
					//发送提交前
					if (typeof beforeSend != 'undefined' && beforeSend instanceof Function)
						if (beforeSend() == false)
							return false;
						
                    var data = "";
                    if (entityType == 1)
                        data = handler.DoMethodReturnString("MyBill_Submit");
                    else
                        data = handler.DoMethodReturnString("MyDict_Submit");
                    this.innerHTML = "<i class='iconfont icon-baocun'></i>提交";
                    if (data.indexOf("err@") != -1) {
                        layer.alert(data);
                        return false;
                    }
                    layer.alert("提交成功");
                    if (typeof isSaveAfterCloseOfBill != "undefined" && isSaveAfterCloseOfBill == true) {
                        var index = parent.parent.layer.getFrameIndex(window.parent.name);
                        parent.parent.layer.close(index);
                    }

                    return false;
                })

                break;
            case "Delete":
                layer.confirm('您确定要删除吗?', function (index) {
                    layer.close(index);
                    var handler = new HttpHandler("BP.CCBill.WF_CCBill");
                    handler.AddUrlData();
					
					 //增加删除前事件
					if (typeof beforeDelete != 'undefined' && beforeDelete instanceof Function)
						if (beforeDelete() == false)
							return false;
						
                    var data = "";
                    if (entityType == 1)
                        data = handler.DoMethodReturnString("MyBill_Delete");
                    else
                        data = handler.DoMethodReturnString("MyDict_Delete");
                    if (data.indexOf("err@") != -1) {
                        layer.alert(data);
                        return;
                    }
                    if (window.parent && (window.parent.location.href.indexOf("SearchDict") != -1
                        || window.parent.location.href.indexOf("SearchBill")!=-1)) {
                        window.parent.location.reload();
                        //关闭该弹出层
                        window.parent.layer.close(layer.index);
                    } else {
                        if (entityType == 1)
                            SetHref('SearchBill.htm?FrmID=' + GetQueryString("FrmID"));
                        else
                            SetHref('SearchDict.htm?FrmID=' + GetQueryString("FrmID"));
                    }
                });
                break;
            case "DataVer":
                var url = "./OptComponents/DataVer.htm?FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID");
                SetHref(url);
                break;
            case "Search":
                if (entityType == 1) {
                     var url = "SearchBill.htm?FrmID=" + GetQueryString("FrmID");
                    if (methodNo != "")
                        url += "&MethodNo=" + methodNo;
                    if (pworkid != 0)
                        url += "&PWorkID=" + pworkid;
                    SetHref(url);
                }
                   
                else
                    SetHref("SearchDict.htm?FrmID=" + GetQueryString("FrmID"));
                
                break;
            case "Group":
                var url = "Group.htm?FrmID=" + GetQueryString("FrmID");
                if (methodNo != "")
                    url += "&MethodNo=" + methodNo;
                if (pworkid != 0)
                    url += "&PWorkID=" + pworkid;
                SetHref(url);
                break;
            case "Print":
                var type = $(this).data("type");
                var url = "";
                if (type == "HTML" || type == "PDF") {
                    url = "../WorkOpt/Packup.htm?FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID") + "&SourceType=Bill&FileType=" + type;
                    OpenLayuiDialog(url, "打印ZIP", window.innerWidth / 2, Window.innerHeight / 2, "auto");
                    break;
                }
               
                if (type == "RTF") {
                    url = "../WorkOpt/PrintDoc.htm?FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID") + "&SourceType=Bill";
                    OpenLayuiDialog(url, "打印RTF", window.innerWidth / 2, 50, "auto");
                    break;
                }
                if (type == "CCWord") {
                    url = "../WorkOpt/PrintDoc.htm?FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID") + "&SourceType=Bill";
                    OpenLayuiDialog(url, "打印CCWord", window.innerWidth / 2, 50, "auto");
                    break;
                }
                if (type == "ZIP") {
                    url = "../WorkOpt/Packup.htm?FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID") + "&SourceType=Bill&FileType=zip";
                    OpenLayuiDialog(url, "打印ZIP", window.innerWidth / 2, 50, "auto");
                    break;
                }
                break;
            case "dictFlow":
                break;
            case "Setting":
                var url = "../Comm/RefFunc/En.htm?EnName=BP.CCBill.FrmDict&PKVal=" + GetQueryString("FrmID");
                OpenLayuiDialog(url, "设置", window.innerWidth * 4 / 5, 80, "auto");
                break;
        }
    })
}

function keyDown(e) {
    e.preventDefault();
    var currKey = 0, e = e || event || window.event;
    currKey = e.keyCode || e.which || e.charCode;
    if (currKey == 83 && (e.ctrlKey || e.metaKey)) {

    }
    return true;
}

function SearchBill() {
    var url = "SearchBill.htm?FrmID=" + GetQueryString("FrmID");
    SetHref(url);
}


function DraftBox() {
    var url = "Draft.htm?FrmID=" + GetQueryString("FrmID");
    SetHref(url);
}

function RefBill(frmID) {
    //关联单据

    var W = document.body.clientWidth - 40;
    var H = document.body.clientHeight - 40;
    var url = "Opt/RefBill.htm?PFrmID=" + frmID + "&WorkID=" + GetQueryString("WorkID") + "&FrmID=" + GetQueryString("FrmID");
    OpenBootStrapModal(url, "eudlgframe", "关联单据", W, H, "icon-property", null, null, null, function () {
        Reload();
    }, null, "black");
}

//查看关联单据的信息
function ShowRefBillInfo(frmID) {
    var workID = frmData.MainTable[0].PWorkID;
    var url = "MyBill.htm?WorkID=" + workID + "&FrmID=" + frmID + "&FK_MapData=" + frmID;
    var W = document.body.clientWidth - 40;
    var H = document.body.clientHeight - 40;

    OpenBootStrapModal(url, "eudlgframe", "关联单据信息", W, H, "icon-property", null, null, null, null, null, "black");
}


function StartFlow() {
    alert('尚未完成.');
}


function PrintPDF() {
    var W = document.body.clientWidth - 40;
    var H = document.body.clientHeight - 40;
    $("#Btn_PrintPdf").val("PDF打印中...");
    $("#Btn_PrintPdf").attr("disabled", true);
    var _html = document.getElementById("divCurrentForm").innerHTML;
    _html = _html.replace("height: " + $("#topContentDiv").height() + "px", "");
    _html = _html.replace("height: " + $("#contentDiv").height() + "px", "");
    _html = _html.replace("height: " + $("#divCCForm").height() + "px", "");

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
    handler.AddPara("html", _html);
    handler.AddPara("FrmID", GetQueryString("FrmID"));
    handler.AddPara("WorkID", GetQueryString("WorkID"));
    handler.AddPara("SourceType", "Bill");
    var data = handler.DoMethodReturnString("Packup_Init");
    if (data.indexOf("err@") != -1) {
        alert(data);
    } else {
        $("#Btn_PrintPdf").val("PDF打印成功");
        $("#Btn_PrintPdf").attr("disabled", false);
        $("#Btn_PrintPdf").val("打印pdf");
        var urls = JSON.parse(data);
        for (var i = 0; i < urls.length; i++) {
            if (urls[i].No == "pdf") {
                window.open(urls[i].Name);
                break;
            }
        }
    }
}



