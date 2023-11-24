//初始化按钮
var webUser = new WebUser();
var initData;
var pageFrom = "MyFlow";
var wf_node = null;
function InitToolBar(pageType) {
    pageFrom = pageType
    if (pageType == "CC") {
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyCC");
        handler.AddUrlData();
        handler.AddPara("IsMobile", 1);
        initData = handler.DoMethodReturnString("InitToolBar");
        //MyView
    } else if (pageType == "MyView") {
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyView");
        handler.AddUrlData();
        handler.AddPara("IsMobile", 1);
        initData = handler.DoMethodReturnString("InitToolBar");
        //MyFlow   
    } else {
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
        handler.AddUrlData();
        handler.AddPara("IsMobile", 1);
        initData = handler.DoMethodReturnString("InitToolBar");
    }

    if (initData.indexOf("err@") != -1) {
        alert(initData);
        console.log(initData);
        return;
    }
    initData = JSON.parse(initData);
    //当前节点的信息
    if (initData.WF_Node != undefined)
        wf_node = initData.WF_Node[0];
    else
        wf_node = new Entity("BP.WF.Node", pageData.FK_Node);

    var toolBars = initData.ToolBar;
    if (toolBars == undefined)
        toolBars = initData;

    var btnLab = new Entity("BP.WF.Template.BtnLab", pageData.FK_Node);
   
    var bottombar = $('#bottomToolBar');
    var popoverBar = $('#popoverBar');
    var barcount = 0;
    var currentPath = GetHrefUrl();
    var appPath = currentPath.substring(0, currentPath.indexOf('/CCMobile') + 1);
    var realBars = $.grep(toolBars, function (toolBar) {
        return toolBar.No != "Close" && toolBar.No != "PackUp_zip" || toolBar.No != "PackUp_html" || toolBar.No != "PackUp_pdf";
    });
    if (realBars.length <= 4) {
        $.each(realBars, function (i, toolBar) {
            //增加按钮操作
            bottombar.append("<a class='mui-tab-item' id='" + toolBar.No + "' name='" + toolBar.No + "' href='#' >" + toolBar.Name + "</ a>");
        });
    }
    if (realBars.length > 4) {
        $.each(realBars, function (i, toolBar) {
            barcount++;
            //增加按钮操作
            if (barcount == 4) {
                bottombar.append('<a class="mui-tab-item" href="#Popover">更多</a>');
                barcount++;
            }
            if (barcount < 4)
                bottombar.append("<a class='mui-tab-item' id='" + toolBar.No + "' name='" + toolBar.No + "' href='#' >" + toolBar.Name + "</ a>");

            if (barcount > 4)
                popoverBar.append("<li class='mui-table-view-cell'><a id='" + toolBar.No + "' name='" + toolBar.No + "' href='#' >" + toolBar.Name + "</ a></li>");
        });
    }
    $.each(toolBars, function (i, toolBar) {
        if (toolBar.No == "Close")
            return;
        if (toolBar.No == "PackUp_zip" || toolBar.No == "PackUp_html" || toolBar.No == "PackUp_pdf")
            return true;

        //barcount++;
        ////增加按钮操作
        //if (barcount == 4) {
        //    bottombar.append('<a class="mui-tab-item" href="#Popover">更多</a>');
        //    barcount++;
        //}
        //if (barcount < 4)
        //    bottombar.append("<a class='mui-tab-item' id='" + toolBar.No + "' name='" + toolBar.No +"' href='#' >" + toolBar.Name + "</ a>");
            
        //if (barcount > 4)
        //    popoverBar.append("<li class='mui-table-view-cell'><a id='" + toolBar.No + "' name='" + toolBar.No +"' href='#' >" + toolBar.Name + "</ a></li>");
       //处理按钮的点击操作   
        if (toolBar.Oper != undefined && toolBar.Oper != "") {
            //发送
            if (toolBar.No == "Send") {
                if (toolBar.Oper.indexOf("SendSelfFrom") != -1) {
                    $("#Send").on("tap", function () {
                        btnLab.SendJS
                        if (SendSelfFrom() == false)
                            return false;
                        SendIt();
                    });
                } else {
                    $("#Send").on("tap", function () {
                        btnLab.SendJS;
                        SaveDtlAll();
                        SendIt();
                    });
                }
                return;
            }

            //保存
            if (toolBar.No == "Save") {
                //嵌入式表单的保存
                if (toolBar.Oper.indexOf("SaveSelfFrom") != -1) {
                    $("#Save").on("tap", function () {
                        SaveSelfFrom();

                    });
                } else {
                    //保存
                    $("#Save").on("tap", function () {
                        //如果是嵌入式表单
                        if (SysCheckFrm() == false)
                            return false;
                        Save(0);

                    });
                }
                return;
            }

            //会签发送
            if (toolBar.No == "SendHuiQian") {
                $("#SendHuiQian").on("tap", function () {
                    if (SysCheckFrm() == false)
                        return false;
                    SaveDtlAll();
                    SendIt(true);
                });
                return;
            }

            //撤销
            if (toolBar.No == "UnSend") {
                $("#UnSend").on("tap", function () {
                    UnSend();
                });
                return;
            }

            //催办
            if (toolBar.No == "Press") {
                $("#Press").on("tap", function () {
                    Press();
                });
                return;
            }

            //帮助提示
           if (toolBar.No == "Help") {
                $("#Help").on("tap", function () {
                    $("#Popover").hide();
                    // 弹出提示框
                    HelpAlter();
                });
               return;
            }
           
            $("#" + toolBar.No).on("tap", function () {
                if (toolBar.No == "UnSendAllThread")
                    UnSendAllThread();
                if (toolBar.Oper != null && toolBar.Oper != undefined && toolBar.Oper != "") {
                    toolBar.Oper = toolBar.Oper.toString().replace(/~/g, "'");
                    cceval(toolBar.Oper);
                }
               
            });
            
        }
           
    });

    if ($('[name=Return]').length > 0)
        $('[name=Return]').bind('tap', function () { initModal("returnBack"); $('#returnWorkModal').modal().show(); });
    
    if ($('[name=Shift]').length > 0) 
        $('[name=Shift]').bind('tap', function () { initModal("shift"); $('#returnWorkModal').modal().show(); });
    

    if ($('[name=Askfor]').length > 0)
        $('[name=Askfor]').bind('tap', function () { initModal("askfor"); $('#returnWorkModal').modal().show(); });
    

    if ($('[name=HuiQian]').length > 0) {
        $('[name=HuiQian]').bind('tap', function () { initModal("HuiQian"); $('#returnWorkModal').modal().show(); });
    }

    //if ($('[name=SendHuiQian]').length > 0) {
    //    $('[name=SendHuiQian]').attr('tap', '');
    //    $('[name=SendHuiQian]').unbind('tap');
    //    $('[name=SendHuiQian]').bind('tap', function () { initModal("SendHuiQian"); $('#returnWorkModal').modal().show(); });
    //}

    if ($('[name=AddLeader]').length > 0)
        $('[name=AddLeader]').bind('tap', function () { initModal("AddLeader"); $('#returnWorkModal').modal().show(); });
    


    if ($('[name=PackUp_zip]').length > 0) {
        $('[name=PackUp_zip]').attr('style', 'visibility:hidden');
        $('[name=PackUp_zip]').bind('tap', function () { initModal("PackUp_zip"); $('#returnWorkModal').modal().show(); });

    }
   

    if ($('[name=PackUp_html]').length > 0) {
        $('[name=PackUp_html]').attr('style', 'visibility:hidden');
        $('[name=PackUp_html]').bind('tap', function () { initModal("PackUp_html"); $('#returnWorkModal').modal().show(); });

    }
    

    if ($('[name=PackUp_pdf]').length > 0) {
        $('[name=PackUp_pdf]').attr('style', 'visibility:hidden');
        $('[name=PackUp_pdf]').bind('tap', function () { initModal("PackUp_pdf"); $('#returnWorkModal').modal().show(); });
    }

    if ($('[name=Track]').length > 0) {
        $('[name=Track]').bind('tap', function () { initModal("Track"); $('#returnWorkModal').modal().show(); });
    }
    if ($('[name=EndFlow]').length > 0) {
        $('[name=EndFlow]').bind('tap', function () { DoStop(btnLab.EndFlowLab, pageData.FK_Flow, pageData.WorkID); $('#returnWorkModal').modal().show(); });
    }

    if ($('[name=SelectAccepter]').length > 0) {
        $('[name=SelectAccepter]').bind('tap', function () {initModal("accepter");$('#returnWorkModal').modal().show();
        });
    }
    if ($('[name=Delete]').length > 0) {
        $('[name=Delete]').bind('tap', DeleteFlow);
    }
    if ($('[name=CC]').length > 0) {
        $('[name=CC]').bind('tap', function () { initModal("CC"); $('#returnWorkModal').modal().show(); });
    }

    if ($('[name=Note]').length > 0) {
        $('[name=Note]').bind('tap', function () { initModal("Note"); $('#returnWorkModal').modal().show(); });
    }
    //流转自定义
    if ($('[name=TransferCustom]').length > 0) {
        $('[name=TransferCustom]').bind('tap', function () { initModal("TransferCustom"); $('#returnWorkModal').modal().show(); });
    }
    //分流节点，查看表单
    if ($('[name=OpenFrm]').length > 0) {
        $('[name=OpenFrm]').bind('tap', function () { initModal("OpenFrm"); $('#returnWorkModal').modal().show(); });
    }

}

//撤销
function UnSend() {
    mui.confirm('您确定要撤销发送吗？', function (e) {
        if (e.index == 1) {
            //加载标签页
            var handler = new HttpHandler("BP.WF.HttpHandler.CCMobile_WorkOpt_OneWork");
            handler.AddPara("FK_Node", pageData.FK_Node);
            handler.AddPara("FK_Flow", pageData.FK_Flow);
            handler.AddPara("WorkID", pageData.WorkID);
            handler.AddPara("FID", pageData.FID);
            var data = handler.DoMethodReturnString("TimeBase_UnSend");

            if (data.indexOf('err@') == 0) {
                mui.alert(data);
                return;
            }

            var url = "/CCMobile/MyFlow.htm?FK_Flow=" + pageData.FK_Flow + "&WorkID=" + pageData.WorkID + "&t=" + Math.random();
            SetHref(url);
        }
    })
}
//催办
function Press() {
    mui.prompt('请输入催办信息', '该工作因为xxx原因，需要您优先处理.', function (e) {
        if (e.index == 1) {
            if (e.value == "")
                return;
            var handler = new HttpHandler("BP.WF.HttpHandler.WF");
            handler.AddUrlData();
            handler.AddPara("Msg", e.value);
            var data = handler.DoMethodReturnString("Runing_Press");

            if (data.indexOf('err@') == 0) {
                console.log(data);
                mui.alert(data);
                return;
            }

            mui.alert(data);
            return;
        }
    });
   
}

//结束流程.
function DoStop(msg, flowNo, workid) {

    mui.confirm('您确定要执行 [' + msg + '] ?', function (e) {
        if (e.index == 1) {
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
            handler.AddPara("FK_Flow", flowNo);
            handler.AddPara("WorkID", workid);
            var data = handler.DoMethodReturnString("MyFlow_StopFlow");
            if (data.indexOf('err@') == 0)
                return;
            SetHref( "Todolist.htm?1=2");

        }
    })

}
//删除流程.
function DeleteFlow() {
    mui.confirm('您确定要删除吗？', function (e) {
        if (e.index == 1) {
            pageData = {
                WorkID: GetQueryString('WorkID'),
                FK_Flow: GetQueryString("FK_Flow")
            };

            var handler = new HttpHandler("BP.WF.HttpHandler.CCMobile_MyFlow");
            handler.AddPara("FK_Flow", pageData.FK_Flow);
            handler.AddPara("WorkID", pageData.WorkID);
            var data = handler.DoMethodReturnString("MyFlowGener_Delete");

            SetHref( "Todolist.htm?1=2");

        }
    })
}
//初始化退回、移交、加签窗口
function initModal(modalType, toNode) {

    //初始化退回窗口的SRC
    var returnWorkModalHtml = '<div class="modal fade" id="returnWorkModal" data-backdrop="static">' +
        '<div class="modal-dialog">'
        + '<div class="modal-content" style="border-radius:0px;width:700px;text-align:left;">'
        + '<div class="modal-header">'
        + '<button type="button" style="color:white;float: right;background: transparent;border: none;" data-dismiss="modal" aria-hidden="true">&times;</button>'
        + '<h4 class="modal-title" id="modalHeader">工作退回</h4>'
        + '</div>'
        + '<div class="modal-body">'
        + '<iframe style="width:100%;border:0px;height:400px;" id="iframeReturnWorkForm" name="iframeReturnWorkForm"></iframe>'
        + '</div>'
        + '</div><!-- /.modal-content -->'
        + '</div><!-- /.modal-dialog -->'
        + '</div>';

    $('body').append($(returnWorkModalHtml));
    Save(1);
    var modalIframeSrc = '';
    if (modalType != undefined) {
        switch (modalType) {
            case "returnBack":
                $('#modalHeader').text("工作退回");
                modalIframeSrc = "./WorkOpt/ReturnWork.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&s=" + Math.random()
                break;
            case "Track":
                $('#modalHeader').text("运行轨迹");
                var workID = pageData.WorkID;
                if (pageData.WorkID == 0 && pageData.FID != 0)
                    workID = pageData.FID;
                modalIframeSrc = "./WorkOpt/OneWork/TimeBase.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + workID + "&FK_Flow=" + pageData.FK_Flow + "&From=" + pageFrom + "&s=" + Math.random()
                if (pageFrom == "MyView")
                    modalIframeSrc += "&MyViewFrom=" + GetQueryString("MyViewFrom");
                if (pageFrom == "MyFlow")
                    modalIframeSrc += "&MyFlowFrom=" + GetQueryString("MyFlowFrom") + "&IsShowUnSend=0";
                if (pageFrom == "CC")
                    modalIframeSrc += "&MyCCFrom=" + GetQueryString("MyCCFrom") + "&IsShowUnSend=0";
                break;
            case "accpter":
                $('#modalHeader').text("工作移交");
                modalIframeSrc = "./WorkOpt/Accepter.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&Info=&s=" + Math.random()
                break;
            case "shift":
                $('#modalHeader').text("工作移交");
                modalIframeSrc = "./WorkOpt/Forward.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&Info=&s=" + Math.random()
                break;
            case "askfor":
                $('#modalHeader').text("加签");
                modalIframeSrc = "./WorkOpt/Askfor.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&Info=&s=" + Math.random()
                break;
            case "HuiQian":
                $('#modalHeader').text("会签");
                modalIframeSrc = "./WorkOpt/HuiQian.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&Info=&s=" + Math.random()
                break;
            case "AddLeader":
                $('#modalHeader').text("加组长");
                modalIframeSrc = "./WorkOpt/HuiQian.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "HuiQianType=AddLeader&s=" + Math.random()
                break;
            case "PackUp_zip":
            case "PackUp_html":
            case "PackUp_pdf":
                $('#modalHeader').text("打包下载/打印");
                var url = "./WorkOpt/Packup.htm?FileType=" + modalType.replace('PackUp_', '') + "&FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&Info=&s=" + Math.random();
                // alert(url);
                modalIframeSrc = "./WorkOpt/Packup.htm?FileType=" + modalType.replace('PackUp_', '') + "&FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&Info=&s=" + Math.random()
                break;
            case "accepter":
                $('#modalHeader').text("选择下一个节点及下一个节点接受人");
                modalIframeSrc = "./WorkOpt/Accepter.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&s=" + Math.random()
                break;
            //发送选择接收节点和接收人     
            case "sendAccepter":
                $('#modalHeader').text("发送到节点：" + toNode.Name);
                var toNodeNo = toNode.No;
                if (toNodeNo == undefined)
                    toNodeNo = toNode.NodeID;
                modalIframeSrc = "./WorkOpt/Accepter.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&ToNode=" + toNodeNo + "&s=" + Math.random()
                break;
            case "CC":
                $('#modalHeader').text("抄送");
                modalIframeSrc = "./WorkOpt/CC.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&ToNode=" + toNode + "&Info=&s=" + Math.random()
                break;
            case "Note":
                $('#modalHeader').text("备注");
                modalIframeSrc = "./WorkOpt/Note.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&Info=&s=" + Math.random();
                break;
            case "SubFlow":
                $('#modalHeader').text("子流程");
                modalIframeSrc = "./WorkOpt/SubFlow.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&From=" + pageFrom + "&Info=&s=" + Math.random();
                if (pageFrom == "MyView" || pageFrom == "CC")
                    modalIframeSrc += "&IsReadonly=1";
                if (pageFrom == "MyFlow")
                    modalIframeSrc += "&IsReadonly=0";
                break;
            case "TransferCustom":
                $('#modalHeader').text("流转自定义");
                modalIframeSrc = "./WorkOpt/TransferCustom.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&s=" + Math.random() ;
                break;
            case "OpenFrm":
                $('#modalHeader').text("查看表单");
                var workID = pageData.WorkID;
                if (pageData.WorkID == 0 && pageData.FID != 0)
                    workID = pageData.FID;
                modalIframeSrc = "/WF/MyFrm.htm?NodeID=" + pageData.FK_Node + "&FK_Node=" + pageData.FK_Node + "&WorkID=" + workID + "&FK_Flow=" + pageData.FK_Flow + "&s=" + Math.random();
                break;
            default:
                break;
        }
    }
    SetHref( modalIframeSrc);

}

//提示
function HelpAlter() {
    //判断该节点是否启用了帮助提示 0 禁用 1 启用 2 强制提示 3 选择性提示
    var btnLab = new Entity("BP.WF.Template.BtnLab", pageData.FK_Node);
    if (btnLab.HelpRole != 0) {
        var count = 0;
        if (btnLab.HelpRole == 3) {
            var mypk = webUser.No + "_ND" + pageData.FK_Node + "_HelpAlert";
            var userRegedit = new Entity("BP.Sys.UserRegedit");
            userRegedit.SetPKVal(mypk);
            count = userRegedit.RetrieveFromDBSources();
        }

        if (btnLab.HelpRole == 2 || (count == 0 && btnLab.HelpRole == 3)) {
            var filename = basePath + "/DataUser/CCForm/HelpAlert/" + pageData.FK_Node + ".htm";
            var htmlobj = $.ajax({ url: filename, async: false });
            if (htmlobj.status == 404)
                return;
            var str = htmlobj.responseText;
            if (str != null && str != "" && str != undefined) {
                mui.alert(str, '', "我知道了", function () {
                    //保存用户的帮助指引信息操作
                    var mypk = webUser.No + "_ND" + pageData.FK_Node + "_HelpAlert"
                    var userRegedit = new Entity("BP.Sys.UserRegedit");
                    userRegedit.SetPKVal(mypk);
                    var count = userRegedit.RetrieveFromDBSources();
                    if (count == 0) {
                        //保存数据
                        userRegedit.FK_Emp = webUser.No;
                        userRegedit.FK_MapData = "ND" + pageData.FK_Node;
                        userRegedit.Insert();
                    }
                });
                $(".mui-popup-title").hide();
            }
        }
    }
}

//记录改变字段样式 不可编辑，不可见
var AllObjSet = {};
var IsFirstLoad = true;
//表单联动
function Set_Frm_Enable(frmData, fromId) {
    IsFirstLoad = true;
    var mapAttrs = frmData.Sys_MapAttr;
    //解析设置表单字段联动显示与隐藏.
    for (var i = 0; i < mapAttrs.length; i++) {
        var mapAttr = mapAttrs[i];
        if (mapAttr.UIVisible == 0)
            continue;

        if (mapAttr.LGType != 1)
            continue;

        if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) {  // AppInt Enum
            if (mapAttr.AtPara.indexOf('@IsEnableJS=1') >= 0) {
                if (mapAttr.UIContralType == 1 || mapAttr.UIContralType == 3) {
                    /*启用了显示与隐藏.*/
                    var ddl;
                    if (fromId == null || fromId == undefined)
                        ddl = $("#DDL_" + mapAttr.KeyOfEn);
                    else
                        ddl = $("#" + fromId+" #DDL_" + mapAttr.KeyOfEn);
                    //初始化页面的值
                    var nowKey = ddl.val();//ddl.val();
                    if (nowKey == null || nowKey == undefined || nowKey == "")
                        continue;

                    setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, nowKey, fromId);

                }
            }
        }

    }
}

/**------------------------子线程退回分流节点的工作处理器按钮操作-------------------------------**/

/**
 * 驳回子线程
 */
function ReSend() {
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddUrlData();
    var data = handler.DoMethodReturnString("ReSend");
    if (data.indexOf("err@") != -1) {
        mui.alert(data);
        return;
    }
    OptSuc(data);
}
/**
 * 取消子线程
 */
function KillThread() {
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddUrlData();
    var data = handler.DoMethodReturnString("KillThread");
    if (data.indexOf("err@") != -1) {
        mui.alert(data);
        return;
    }
    window.close();
}
/**
 * 撤销整体发送
 */
function UnSendAllThread() {
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddUrlData();
    var data = handler.DoMethodReturnString("UnSendAllTread");
    if (data.indexOf("err@") != -1) {
        mui.alert(data);
        return;
    }
    if (data.indexOf("url@") != -1) {
        var url = data.replace("url@", "");
        SetHref(url);
    }
}
/**
 * 增加子线程
 */
function AddThread(toNodeID) {
    if (toNodeID == null || toNodeID == undefined || toNodeID == "")
        toNodeID = 0;
    if (wf_node.RunModel == 2 || wf_node.RunModel == 3) {

        var msg = "说明：";
        msg += "\t\n 1. 新增加的人员，从分流节点的下一个节点开始执行.";
        msg += "\t\n 2. 输入人员账号，点击确定后，系统就会自动为该人员分配一个任务.";

        var empNo = promptGener(msg + ' 请输入要增加的人员账号，多个人员用逗号分开(比如:zhangsan,lisi):');
        if (empNo == null || empNo == '')
            return;

        var workid = GetQueryString("WorkID");
        var en = new Entity("BP.WF.GenerWorkFlow", GetQueryString("FID"));
        var data = en.DoMethodReturnString("DoSubFlowAddEmps", empNo, toNodeID);
        mui.alert(data);
        Reload();
        return;
    }
    mui.alert('您没有权限增加人员.');

}
/**------------------------子线程退回分流节点的工作处理器按钮操作-------------------------------**/

