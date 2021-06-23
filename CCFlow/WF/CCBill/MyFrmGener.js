/**
 * 初始化按钮的操作
 */
function ToolBar_Init() {
    var handler = new HttpHandler("BP.CCBill.WF_CCBill_API");
    handler.AddPara("FrmID", GetQueryString("FrmID"));
    handler.AddPara("IsReadonly", GetQueryString("IsReadonly"));
    var data = handler.DoMethodReturnString("CCFrom_ToolBar_Init");
    if (data.indexOf("err@") != -1) {
        layer.alert(data);
        return;
    }
    data = JSON.parse(data);
    var getTpl = document.getElementById("view").innerHTML
        , view = document.getElementById('ToolBar');
    layui.laytpl(getTpl).render(data, function (html) {
        view.innerHTML = html;
    });

    //解析事件.
    $("#ToolBar .layui-btn").on('click', function () {
        switch (this.name) {
            case "Add":
                var handler = new HttpHandler("BP.CCBill.WF_CCBill");
                handler.AddUrlData();
                var data = handler.DoMethodReturnString("MyDict_CreateBlankDictID");
                if (data != null && data != undefined && data.indexOf('err@') > 0) {
                    layer.alert(data);
                    return;
                }
                window.location.href = 'MyDict.htm?FrmID=' + GetQueryString("FrmID") + "&WorkID=" + data;
                break;
            case "Save":
                //保存信息
                layui.form.on('submit(Save)', function (data) {
                    var formData = getDictFormData(data.field);
                    var handler = new HttpHandler("BP.CCBill.WF_CCBill");
                    for (var key in formData) {
                        handler.AddPara(key, encodeURIComponent(formData[key]));
                    }
                    handler.AddUrlData();
                    var data = handler.DoMethodReturnString("MyDict_SaveIt");
                    if (data.indexOf("err@") != -1) {
                        layer.alert(data);
                    }
                    return false;
                })

                break;
            case "Submit":
                break;
            case "Delete":
                layer.confirm('您确定要删除吗?', function (index) {
                    layer.close(index);
                    var handler = new HttpHandler("BP.CCBill.WF_CCBill");
                    handler.AddUrlData();
                    var data = handler.DoMethodReturnString("MyDict_Delete");
                    if (data.indexOf("err@") != -1) {
                        layer.alert(data);
                        return;
                    }
                    if (window.parent && window.parent.location.href.indexOf("SearchDict") != -1) {
                        window.parent.reload();
                        //关闭该弹出层
                        window.parent.layer.close(layer.index);
                    } else {
                        window.location.href = 'SearchDict.htm?FrmID=' + GetQueryString("FrmID");
                    }
                });
                break;
            case "DataVer":
                var url = "./OptComponents/DataVer.htm?FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID");
                window.location.href = url;
                break;
            case "Search":
                var url = "SearchDict.htm?FrmID=" + GetQueryString("FrmID");
                window.location.href = url;
                break;
            case "Group":
                var url = "Group.htm?FrmID=" + GetQueryString("FrmID");
                window.location.href = url;
                break;
            case "Print":
                var type = $(this).data("type");
                var url = "";
                if (type == "HTML") {
                    url = "../WorkOpt/Packup.htm?FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID") + "&SourceType=Bill&FileType=htm";
                    OpenLayuiDialog(url, "打印ZIP", window.innerWidth / 2, Window.innerHeight / 2, "auto");
                    break;
                }
                if (type == "PDF") {
                    PrintPDF();
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
/**
 * 获取实体单据数据的集合
 * @param {any} formData
 */
function getDictFormData(formData) {
    //处理特殊的表单字段
    //1.富文本编辑器
    if ($(".rich").length > 0) {
        $(".rich").each(function (i, item) {
            var edit = layui.tinymce.get('#' + item.id)
            var val = edit.getContent();
            dataJson[item.id] = val;
        })
    }
    //2.复选框多选
    var mcheckboxs = $(".mcheckbox");
    if (mcheckboxs.length > 0) {
        const sorted = groupBy(mcheckboxs, function (item) {
            return item.name;
        });
        for (var key in sorted) {
            var ids = [];
            $("input[name='" + key + "']:checked").each(function (index, item) {
                ids.push($(this).val())
            });
            dataJson[key] = ids.join(",");
        }
    }
    //3.树形结构
    return formData;
}
function keyDown(e) {
    e.preventDefault();
    var currKey = 0, e = e || event || window.event;
    currKey = e.keyCode || e.which || e.charCode;
    if (currKey == 83 && (e.ctrlKey || e.metaKey)) {

        //  $('#Btn_Save').click();
        //	return false;xxxx
    }
    return true;
}

function SearchBill() {
    var url = "SearchBill.htm?FrmID=" + GetQueryString("FrmID");
    window.location.href = url;
}


function DraftBox() {
    var url = "Draft.htm?FrmID=" + GetQueryString("FrmID");
    window.location.href = url;
}

function RefBill(frmID) {
    //关联单据

    var W = document.body.clientWidth - 40;
    var H = document.body.clientHeight - 40;
    var url = "Opt/RefBill.htm?PFrmID=" + frmID + "&WorkID=" + GetQueryString("WorkID") + "&FrmID=" + GetQueryString("FrmID");
    OpenBootStrapModal(url, "eudlgframe", "关联单据", W, H, "icon-property", null, null, null, function () {
        window.location.href = window.location.href;
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


//检查附件数量.
function checkAths() {
    // 不支持火狐浏览器。
    if ("undefined" != typeof AthParams && AthParams.AthInfo != undefined) {
        var aths = document.getElementsByName("Ath");
        for (var i = 0; i < aths.length; i++) {
            var athment = aths[i].id.replace("Div_", "");
            if (AthParams.AthInfo[athment] != undefined && AthParams.AthInfo[athment].length > 0) {
                var athInfo = AthParams.AthInfo[athment][0];
                var minNum = athInfo[0];
                var maxNum = athInfo[1];
                var athNum = $("#Div_" + athment + " table tbody .athInfo").length;
                if (athNum.length == 0)
                    athNum = $("#Div_" + athment + " .athInfo").length;

                if (athNum < minNum)
                    return athment + "上传附件数量不能小于" + minNum;;
                if (athNum > maxNum)
                    return athment + "您最多上传[" + maxNum + "]个附件";
            }
        }
    }
    return "";

}


//必填项检查   名称最后是*号的必填  如果是选择框，值为'' 或者 显示值为 【*请选择】都算为未填 返回FALSE 检查必填项失败
function checkBlanks() {

    var checkBlankResult = true;
    //获得所有的class=mustInput的元素.
    var lbs = $('.mustInput');

    $.each(lbs, function (i, obj) {

        if ($(obj).parent().css('display') != 'none' && (($(obj).parent().next().css('display')) != 'none' || ($(obj).siblings("textarea").css('display')) != 'none')) {
        } else {
            return;
        }

        var keyofen = $(obj).data().keyofen;
        var ele = $('[name$=_' + keyofen + ']');
        if (ele.length == 0)
            return;

        $.each(ele, function (i, obj) {
            var eleM = $(obj);
            switch (eleM[0].tagName.toUpperCase()) {
                case "INPUT":
                    if (eleM.attr('type') == "text") {
                        if (eleM.val() == "") {
                            checkBlankResult = false;
                            eleM.addClass('errorInput');
                        } else {
                            eleM.removeClass('errorInput');
                        }
                    }
                    break;
                case "SELECT":
                    if (eleM.val() == "" || eleM.children('option:checked').text() == "*请选择") {
                        checkBlankResult = false;
                        eleM.addClass('errorInput');
                    } else {
                        eleM.removeClass('errorInput');
                    }
                    break;
                case "TEXTAREA":
                    if (eleM.val() == "") {
                        checkBlankResult = false;
                        eleM.addClass('errorInput');
                    } else {
                        eleM.removeClass('errorInput');
                    }
                    break;
            }
        });

    });




    return checkBlankResult;
}

//正则表达式检查
function checkReg() {
    var checkRegResult = true;
    var regInputs = $('.CheckRegInput');
    $.each(regInputs, function (i, obj) {
        var name = obj.name;
        var mapExtData = $(obj).data();
        if (mapExtData.Doc != undefined) {
            var regDoc = mapExtData.Doc.replace(/【/g, '[').replace(/】/g, ']').replace(/（/g, '(').replace(/）/g, ')').replace(/｛/g, '{').replace(/｝/g, '}').replace(/，/g, ',');
            var tag1 = mapExtData.Tag1;
            if ($(obj).val() != undefined) {

                var result = CheckRegInput(name, regDoc, tag1);
                if (!result) {
                    $(obj).addClass('errorInput');
                    checkRegResult = false;
                } else {
                    $(obj).removeClass('errorInput');
                }
            }
        }
    });

    return checkRegResult;
}
