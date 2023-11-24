/***toolbar的图标颜色81C4FF，大小20 */
var wf_node = null;
var webUser = new WebUser();
var toolbarPos = getConfigByKey("ToolbarPos", '0');  //签名图片的默认后缀
var btnMenu = null;
$(function () {
    var barHtml = "";
    var data;
    //广西计算中心特意加的一个url参数，用来控制toolbar隐藏
    if (GetQueryString("hideAllBtn") === "1") return;

    var handler = "";
    //MyCC
    if ($("#JS_CC").length == 1) {
        handler = "BP.WF.HttpHandler.WF_MyCC";
    } else if ($("#JS_MyView").length == 1 || $("#JS_MyFrm").length == 1) {
        handler = "BP.WF.HttpHandler.WF_MyView";
    } else {
        handler = "BP.WF.HttpHandler.WF_MyFlow";
    }

    var handler = new HttpHandler(handler);
    handler.AddUrlData();
    if ($("#JS_MyFrm").length == 1)
        data = handler.DoMethodReturnString("MyFrm_InitToolBar");
    else
        data = handler.DoMethodReturnString("InitToolBar");

    if (data.indexOf("err@") != -1) {
        alert(data);
        console.log(data);
        return;
    }
    data = JSON.parse(data);

    //当前节点的信息
    if (data.WF_Node != undefined)
        wf_node = data.WF_Node[0];
    else
        wf_node = new Entity("BP.WF.Node", paramData.FK_Node);

   



    var toolBars = data.ToolBar;
    if (toolBars == undefined) {
        toolBars = data;
    }
        
    var _html = "";
    var sendBtnOper = "";
    $.each(toolBars, function (i, toolBar) {
        var Oper = "";

        if (toolBar.Oper != "") {
            if (toolBar.No == "Send" || toolBar.No == "SendBtn" || toolBar.No == "Save")
                Oper = "onclick=\"" + toolBar.Oper + "\"";
            else
                Oper = "data-info=\"" + toolBar.Oper + "\"";
                //Oper = "onclick=\"" + toolBar.Oper + "\"";

            if (toolBar.No == "Send")
                sendBtnOper = Oper;
        }

        if (toolBar.No == "Save") {
            //_html += "<button  class='layui-btn layui-btn-sm layui-btn-primary' lay-submit lay-filter='Save' name='" + toolBar.No + "Btn' enable=true " + Oper + "><img src='" + basePath + "/WF/Img/Btn/" + toolBar.No + ".png' width='20px' height='20px'>&nbsp;" + toolBar.Name + "</button>";

            _html += "<button type='button' class='layui-btn layui-btn-sm layui-btn-primary' lay-submit lay-filter='Save' name='" + toolBar.No + "Btn' enable=true " + Oper + "><img src='" + basePath + "/WF/Img/Btn/" + toolBar.No + ".png' width='20px' height='20px'>&nbsp;" + toolBar.Name + "</button>";
            return true;
        }


        if (toolBar.No == "NodeToolBar") { //自定义工具栏按钮

            var Icon = toolBar.Icon;
            //自定义的默认按钮
            var img = "<img src='" + basePath + "/WF/Img/Btn/CH.png' width='20px' height='20px'>&nbsp;"
            //有上传的icon,否则用默认的
            if (Icon != "" && Icon != undefined) {
                var index = Icon.indexOf("\DataUser");
                if (index != -1)
                    Icon = Icon.replace(Icon.substr(0, index), "../");
                img = "<img src='" + Icon + "' width='20px' height='20px'>&nbsp;";
            }

            _html += "<button type='button' class='layui-bar layui-btn layui-btn-sm layui-btn-primary' name='" + toolBar.No + "' enable=true " + Oper + ">" + img + toolBar.Name + "</button>";

        }
        else {
            if (toolBar.No == "Send")
                //_html += "<button   class='layui-btn layui-btn-sm layui-btn-primary' name='" + toolBar.No + "Btn' enable=true " + Oper + " lay-submit lay-filter='Send'><img src='" + basePath + "/WF/Img/Btn/" + toolBar.No + ".png' width='20px' height='20px'>&nbsp;" + toolBar.Name + "</button>";

                _html += "<button  type='button' class='layui-btn layui-btn-sm layui-btn-primary' name='" + toolBar.No + "Btn' enable=true " + Oper + " lay-submit lay-filter='Send'><img src='" + basePath + "/WF/Img/Btn/" + toolBar.No + ".png' width='20px' height='20px'>&nbsp;" + toolBar.Name + "</button>";
            else {

                //_html += "<button  class='layui-bar layui-btn-sm layui-btn layui-btn-primary' name='" + toolBar.No + "' enable=true " + Oper + " ><img src='" + basePath + "/WF/Img/Btn/" + toolBar.No + ".png' width='20px' height='20px'>&nbsp;" + toolBar.Name + "</button>";

                _html += "<button type='button' class='layui-bar layui-btn-sm layui-btn layui-btn-primary' name='" + toolBar.No + "' enable=true " + Oper + " ><img src='" + basePath + "/WF/Img/Btn/" + toolBar.No + ".png' width='20px' height='20px'>&nbsp;" + toolBar.Name + "</button>";

            }
        }
    });

    if (toolbarPos == "0") {
        $('#ToolBar').html(_html);
        $('#Toolbar').html(_html);
        $(".layui-header").show();
        if (toolBars.length == 0)
            $(".layui-header").hide();
    }

    if (toolbarPos == "1") {
        $('#bottomToolBar').html(_html);
        $('#bottomToolBar').html(_html);
        $(".layui-footer").show();
        $(".layui-header").hide();
        $(".layui-fluid").css("padding-top", "0px");
       
        $(".layui-fluid").css("top", "0px");
    }

    //引入数据批阅的js
    if ($('[name=FrmDBRemark]').length > 0) {
        loadScript(basePath + "/WF/CCForm/JS/FrmDBRemark.js?t=" + Math.random());
    }

    if ($('[name=FrmDBVer]').length > 0) {
        loadScript(basePath + "/WF/CCForm/JS/FrmDBVer.js?t=" + Math.random());
    }

    //按钮旁的下来框
    if (wf_node != null && wf_node.IsBackTrack == 0)
        InitToNodeDDL(data, wf_node);

    //如果启用的按钮操作太多时，自适应高度
   
    if (toolbarPos == "0") {
        var btnH = $("#ToolBar").height();
        if (btnH > 50) {
            $("#ToolBar").parent().height(btnH);
            $("#ContentDiv").parent().css("padding-top", btnH + "px");
        }
    }
    if (toolbarPos == "1") {
        var btnH = $('#bottomToolBar').height();
        if (btnH > 50) {
            $("#bottomToolBar").parent().height(btnH);
            $("#ContentDiv").parent().css("padding-bottom", btnH + "px");
        }
    }
    $('[name=SaveBtn]').attr("saveType", 0);

    $('.layui-bar').on('click', function () {
        var oper = $(this).data("info");
        if (oper != null && oper != undefined && oper != "") {
            oper = oper.toString().replace(/~/g, "'");
            oper = DealExp(oper, webUser, false);
            cceval(oper);
        }


    });

    if ($('[name=Return]').length > 0) {
        $('[name=Return]').bind('click', function () {
            //增加退回前的事件
            if (typeof beforeReturn != 'undefined' && beforeReturn instanceof Function)
                if (beforeReturn() == false)
                    return false;

            if (typeof Save != 'undefined' && Save instanceof Function)
                Save(1);
            initModal("returnBack");
        });
    }

    //流转自定义
    if ($('[name=TransferCustom]').length > 0) {
        $('[name=TransferCustom]').bind('click', function () {
            initModal("TransferCustom");
        });
    }


    //挂起
    if ($('[name=Hungup]').length > 0) {
        $('[name=Hungup]').bind('click', function () {
            initModal("Hungup");
        });
    }


    if ($('[name=Thread]').length > 0) {
        $('[name=Thread]').bind('click', function () {
            initModal("Thread");
        });
    }


    if ($('[name=Shift]').length > 0) {
        $('[name=Shift]').bind('click', function () { initModal("shift"); });
    }

    if ($('[name=Btn_WorkCheck]').length > 0) {
        $('[name=Btn_WorkCheck]').bind('click', function () { initModal("shift"); });
    }

    if ($('[name=Askfor]').length > 0) {
        $('[name=Askfor]').bind('click', function () { initModal("askfor"); });
    }

    if ($('[name=Track]').length > 0) {
        $('[name=Track]').bind('click', function () { initModal("Track"); });
    }



    if ($('[name=HuiQian]').length > 0) {
        $('[name=HuiQian]').bind('click', function () { initModal("HuiQian"); });
    }

    if ($('[name=AddLeader]').length > 0) {
        $('[name=AddLeader]').bind('click', function () { initModal("AddLeader"); });
    }


    if ($('[name=CC]').length > 0) {
        $('[name=CC]').bind('click', function () { initModal("CC"); });
    }

    if ($('[name=PackUp_zip]').length > 0) {
        $('[name=PackUp_zip]').bind('click', function () { initModal("PackUp_zip"); });
    }

    if ($('[name=PackUp_html]').length > 0) {
        $('[name=PackUp_html]').bind('click', function () {
            printHtml();
        });
    }

    if ($('[name=PackUp_pdf]').length > 0) {
        $('[name=PackUp_pdf]').bind('click', function () { initModal("PackUp_pdf"); });
    }
    if ($('[name=PrintDoc]').length > 0) {
        $('[name=PrintDoc]').bind('click', function () { initModal("PrintDoc"); });
    }

    if ($('[name=PR]').length > 0) {
        $('[name=PR]').bind('click', function () { initModal("PR"); });
    }

    if ($('[name=SelectAccepter]').length > 0) {
        $('[name=SelectAccepter]').bind('click', function () {
            initModal("accepter");
        });
    }

    if ($('[name=DBTemplate]').length > 0) {
        $('[name=DBTemplate]').bind('click', function () {
            initModal("DBTemplate");
        });
    }

    if ($('[name=Delete]').length > 0) {
        $('[name=Delete]').bind('click', function () {
            //增加删除前事件
            if (typeof beforeDelete != 'undefined' && beforeDelete instanceof Function)
                if (beforeDelete() == false)
                    return false;

            DeleteFlow();
        });
    }


    if ($('[name=ParentForm]').length > 0) {
        $('[name=ParentForm]').bind('click', function () {

            var myPWorkID = GetQueryString("PWorkID");
            var myPFlow = GetQueryString("PFlowNo");
            if (myPFlow == null) {
                //取得父流程FK_Flow
                var gwf = new Entity("BP.WF.GenerWorkFlow", GetQueryString("WorkID"));
                myPFlow = gwf.PFlowNo;
            }
            var url = "MyView.htm?WorkID=" + myPWorkID + "&FK_Flow=" + myPFlow+"&IsReadonly=1";
            window.open(url);
        });
    }

    if ($('[name=CH]').length > 0) {
        $('[name=CH]').bind('click', function () { initModal("CH"); });
    }

    if ($('[name=Note]').length > 0) {
        $('[name=Note').bind('click', function () { initModal("Note"); });
    }


    //公文正文组件
    if ($('[name=GovDocFile]').length > 0) {
        $('[name=GovDocFile').bind('click', function () { initModal("GovDocFile"); });
    }

    //公文
    if ($('[name=DocWord]').length > 0) {
        $('[name=DocWord').bind('click', function () { initModal("DocWord"); });
    }


    //回滚 Rollback
    if ($('[name=Rollback]').length > 0) {
        $('[name=Rollback]').bind('click', function () { initModal("Rollback"); });
    }

    //跳转 JumpWay
    if ($('[name=JumpWay]').length > 0) {
        $('[name=JumpWay]').bind('click', function () { initModal("JumpWay"); });
    }
    //生成二维码
    if ($('[name=QRCode]').length > 0) {
        $('[name=QRCode]').bind('click', function () { initModal("QRCode"); });
    }
    //数据库版本
   // if ($('[name=FrmDBVer]').length > 0) {
    //    $('[name=FrmDBVer]').bind('click', function () { initModal("FrmDBVer"); });
    //}

    //小纸条
    if ($('[name=Scrip]').length > 0) {
        $('[name=Scrip]').bind('click', function () { initModal("Scrip"); });
    }

    //评论
    if ($('[name=FlowBBS]').length > 0) {
        $('[name=FlowBBS]').bind('click', function () { initModal("FlowBBS"); });
    }

    //IM
    if ($('[name=IM]').length > 0) {
        $('[name=IM]').bind('click', function () { initModal("IM"); });
    }

    //分流节点，查看表单
    if ($('[name=OpenFrm]').length > 0) {
        $('[name=OpenFrm]').bind('click', function () { initModal("OpenFrm"); });
    }
    //切换组织
    if ($('[name=ChangeOrg]').length > 0) {
        $('[name=ChangeOrg]').bind('click', function () {
            //获取所有的部门
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_Setting");
            var depts = handler.DoMethodReturnString("ChangeDept_Init");
            if (depts.indexOf('err@') == 0) {
                layer.alert(depts);
                return;
            }

            depts = JSON.parse(depts);
            if (depts.length == 1) {
                layer.alert("您只有一个部门[" + depts[0].Name + "],不需要切换部门");
                return;
            }
            var _html = "";
            depts.forEach(function (dept) {
                if(webUser.FK_Dept === dept.No)
                    _html += '<a href="javascript:void(0)" onclick="ChangeDept(\'' + dept.No + '\')"><span style="color:green">' + dept.Name + '(当前部门)</span></a></br>';
                else
                    _html += '<a href="javascript:void(0)" onclick="ChangeDept(\'' + dept.No + '\')">' + dept.Name + '</a></br>';
            })
            layer.open({
                title: '切换部门'
                , content: _html
            });

        });
    }
    //延期发送
    if ($('[name=DelayedSend]').length > 0) {
        $('[name=DelayedSend]').bind('click', function () {
            DelayedSend();
        });
    }
    HelpAlter();
    if (typeof AfterToolbarLoad == "function")
        AfterToolbarLoad();
});

function DelayedSend(formType) {
    //设置延期发送，需要验证表单填写内容是否全面
    if (beforeSendCheck(formType) == false)
        return false;
    //需要先保存，当前表单的数据
    isSaveOnly = true;
    $('[name=SaveBtn]').attr("saveType", 1);
    $('[name=SaveBtn]').trigger("click");
    $('[name=SaveBtn]').attr("saveType", 0);
    if (isSaveOnly == false)
        return;
    var toNodeID = 0;
    var selectToNode;
    var isSelectEmps = false;
    if ($('#DDL_ToNode').length > 0) {
        selectToNode = $('#TB_ToNode').data();
        toNodeID = selectToNode.id;
        if (["1", "2", "3", "4", "5"].includes(selectToNode.IsSelectEmps))
            isSelectEmps = true;
    }

    if (isSelectEmps == true) {
        debugger;
        Send(false, formType,1);
        return;
    }
    var _html = `<form class="layui-form" action="">
                    <div class="layui-form-item">
                         <div class="layui-input-inline">
                          <input type="text" name="TB_Day" id="TB_Day"  class="layui-input" value="0">
                        </div>
                        <label class="layui-form-label">天</label>
                        <div class="layui-input-inline">
                            <input type="text" name="TB_Hour" id="TB_Hour"  class="layui-input"value="0">
                        </div>
                        <label class="layui-form-label">小时</label>
                       <div class="layui-input-inline">
                              <select name="DDL_Minute" id="DDL_Minute">
                                <option value="0">0</option>
                                <option value="15">15</option>
                                <option value="30">30</option>
                                <option value="45">45</option>
                              </select>
                        </div>
                        <label class="layui-form-label">分</label>
                    </div>
                </form>
                `;
    layui.use(['form', 'layer'], function () {
        var form = layui.form;
        var layer = layui.layer;
        layer.open({
            title: '设置延期发送'
            , content: _html,
            area: ['auto', '350px'],
            yes: function (index, layero) {
               
                var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
                handler.AddUrlData();
                var day = $("#TB_Day").val() || 0;
                var hour = $("#TB_Hour").val() || 0;
                var minute = $("#DDL_Minute").val() || 0;
                if (day == 0 && hour == 0 && minute == 0) {
                    layer.alert("请设置延期发送的时间");
                    return;
                }
                    
                handler.AddPara("TB_Day", day);
                handler.AddPara("TB_Hour", hour);
                handler.AddPara("DDL_Minute", minute);
                handler.AddPara("ToNodeID", toNodeID);
                var data = handler.DoMethodReturnString("DelayedSend");
                if (data.indexOf("err@") != -1) {
                    layer.alert(data);
                    return;
                }
                layer.alert("延期发送设置成功");
                layer.close(index); //如果设定了yes回调，需进行手工关闭
                closeWindow();
            }
        });
        form.render();
    });
}
function SelectEmps(selectEmpType,toNodeID) {
    if (selectEmpType == "1")
        initModal("sendAccepter", toNodeID, 0);

}
function ChangeDept(deptNo) {
    if (deptNo == WebUser.FK_Dept)
        return;
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Setting");
    handler.AddPara("DeptNo", deptNo);
    var data = handler.DoMethodReturnString("ChangeDept_Submit");

    if (data.indexOf('err@') == 0) {
        layer.alert(data);
        return;
    }
    SetHref(GetHrefUrl());
}
//添加保存动态
function SaveOnly() {

    $("button[name=Save]").html("<img src='./Img/Btn/Save.png' width='22px' height='22px'>&nbsp;正在保存...");
    try {
        Save($('[name=SaveBtn]').attr("saveType"));
    } catch (e) {
        alert(e);
        return;
    }
    $("button[name=Save]").html("<img src='./Img/Btn/Save.png' width='22px' height='22px'>&nbsp;....");
    setTimeout(function () { $("button[name=Save]").html("<img src='./Img/Btn/Save.png' width='22px' height='22px'>&nbsp;保存"); }, 300);
}
function setModalMax() {
    //设置bootstrap最大化窗口
    var w = ddocument.body.clientWidth - 40;
    $("#returnWorkModal .modal-dialog").css("width", w + "px");
}

//初始化退回、移交、加签窗口
function initModal(modalType, toNode, url, isDelayedSend) {

    $("#returnWorkModal").on('hide.bs.modal', function () {
        setToobarEnable();
    });
    var isFrameCross = GetHrefUrl().indexOf(basePath) == -1 ? 1 : 0;
    var modalIframeSrc = '';
    var width = window.innerWidth / 2;
    var height = 50;
    var title = "";
    if (modalType != undefined) {
        var isShowColseBtn = 1;

        switch (modalType) {
            case "returnBack":
                title = "退回";
                width = window.innerWidth / 2;
                var node = new Entity("BP.WF.Template.NodeExt", paramData.FK_Node);

                var info = "";

                if (info == "" && node.ReturnField != "") {
                    if ($("#" + node.ReturnField).length == 1)
                        info = $("#" + node.ReturnField).val();
                    if (info == undefined && $("#TB_" + node.ReturnField).length == 1)
                        info = $("#TB_" + node.ReturnField).val();
                    if (info == undefined)
                        info = "";
                }
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/ReturnWork.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=" + info + "&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "Send":
                SetChildPageSize(80, 80);
                break;
            case "TransferCustom":
                title = "流转自定义";
                width = window.innerWidth * 3 / 5;
                height = 60;
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/TransferCustom.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "accpter":
                title = "工作移交";
                width = window.innerWidth * 4 / 5;
                height = 80;
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/Accepter.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                isShowColseBtn = 0;
                break;
            case "Thread":
            case "thread":
                title = "子线程";
                width = window.innerWidth * 4 / 5;
                height = 80;
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/ThreadDtl.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "shift":
                title = "工作移交";
                width = window.innerWidth * 4 / 5;
                height = 80;
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/Shift.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                isShowColseBtn = 0;
                break;
            case "GovDocFile":
                title = "公文正文";
                width = window.innerWidth * 2 / 5;
                height = 40;
                modalIframeSrc = ccbpmPath + "/WF/CCForm/GovDocFile.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "DocWord":
                title = "公文";
                width = window.innerWidth * 2 / 5;
                height = 40;
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/DocWord.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "askfor":
                title = "加签";
                width = window.innerWidth * 4 / 5;
                height = 80;
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/Askfor.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "Btn_WorkCheck":
                title = "审核";
                width = window.innerWidth * 4 / 5;
                height = 80;
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/WorkCheck.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "Track": //轨迹.
                title = "处理记录、轨迹";
                width = window.innerWidth * 4 / 5;
                height = 80;
                var workID = paramData.WorkID;
                if (paramData.WorkID == 0 && paramData.FID != 0)
                    workID = paramData.FID;
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/OneWork/OneWork.htm?CurrTab=Truck&FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + workID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "HuiQian":
                width = window.innerWidth * 4 / 5;
                height = 80;
                if (toNode != null)
                    title = "先会签，后发送。";
                else
                    title = "加签";
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/HuiQian.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&ToNode=" + toNode + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                isShowColseBtn = 0;
                break;
            case "AddLeader":
                title = "加主持人";
                width = window.innerWidth * 4 / 5;
                height = 80;
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/HuiQian.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&ToNode=" + toNode + "&HuiQianType=AddLeader&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "Hungup":
                var gwf = new Entity("BP.WF.GenerWorkFlow", paramData.WorkID);
                if (gwf.WFState == 4) {
                    alert("业务已经被挂起，不可以重复操作.");
                    return;
                }

                title = "挂起/延期";
                width = window.innerWidth * 4 / 5;
                height = 80;
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/Hungup.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&ToNode=" + toNode + "&HuiQianType=AddLeader&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "CC":
                title = "抄送";
                width = window.innerWidth * 4 / 5;
                height = 80;
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/CC.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&ToNode=" + toNode + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "PackUp_zip":
            case "PackUp_html":
            case "PackUp_pdf":
                title = "打包下载/打印";
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/Packup.htm?FileType=" + modalType.replace('PackUp_', '') + "&FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "PrintDoc":
                title = "打印单据";
                if (wf_node.FormType == 5) {
                    //绑定表单的时候增加一个tab页到页面
                    var formData = { No: "PrintDoc", "Name": "打印单据" };
                    vm.openPage(formData);
                    return;
                }
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/PrintDoc.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&s=" + Math.random();
                break;
            case "Press":
                //$('#modalHeader').text("催办");
                //modalIframeSrc = ccbpmPath + "/WF/WorkOpt/Press.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "Accepter":
            case "accepter":
                title = "选择下一个节点及下一个节点接受人";
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/Accepter.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&s=" + Math.random() + "&isFrameCross=" + isFrameCross + "&IsDelayedSend=" + isDelayedSend;
                isShowColseBtn = 0;
                break;

            //发送选择接收节点和接收人                
            case "sendAccepter":
                //获取到达节点
                var nodeOne = new Entity("BP.WF.Template.NodeSimple", toNode);
                title = "选择接受人(到达节点:" + nodeOne.Name + ")";
                width = window.innerWidth * 4 / 5;
                height = 80;
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/Accepter.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&PWorkID=" + GetQueryString("PWorkID") + "&ToNode=" + toNode + "&s=" + Math.random() + "&isFrameCross=" + isFrameCross + "&IsDelayedSend=" + isDelayedSend;
                isShowColseBtn = 0;
                break;
            case "SelectNodeUrl":
                title = "请选择到达的节点";
               // width = 700;// window.innerWidth * 1 / 2;
                width = window.innerWidth * 0.8; // / 2;

                height = 100;
                modalIframeSrc = url;
                break;

            case "BySelfUrl"://接收人规则自定义URL
                width = window.innerWidth * 4 / 5;
                height = 80;
                modalIframeSrc = url;
                if ($("#returnWorkModal .modal-footer").length == 0) {
                    var footBtn = $('<div class="modal-footer"><div>');
                    $("#returnWorkModal .modal-body").after(footBtn);
                    var footerOK = $('<button type="button" class="Btn" data-dismiss="modal"> 发送</button >');
                    footerOK.click(function () {

                        var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
                        handler.AddUrlData();
                        var data = handler.DoMethodReturnString("Send");
                        if (data.indexOf("err@") != -1) {
                            var reg = new RegExp('err@', "g")
                            var data = data.replace(reg, '');

                            $(".modal-body").html(data);
                            $('#MessageDiv').modal().show();
                            setToobarEnable();
                            return;
                        }
                        OptSuc(data);
                    });
                    var footerClose = $('<button type="button" class="Btn" data - dismiss="modal" > 取消</button >');
                    footerClose.click(function () {
                        var sels = new Entities("BP.WF.Template.SelectAccpers");
                        sels.Delete("WorkID", GetQueryString("WorkID"), "FK_Node", getQueryStringByNameFromUrl(url, "ToNode"));
                        $("#returnWorkModal").modal('hide');

                    });
                    footBtn.append(footerOK).append(footerClose);
                }
                $(".modal-content").css("height", "auto");

                break;
            case "sendAccepterOfOrg":
                title = "选择接受人";
                width = window.innerWidth * 4 / 5;
                height = 80;
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/AccepterOfOrg.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&PWorkID=" + GetQueryString("PWorkID") + "&ToNode=" + toNode + "&s=" + Math.random() + "&isFrameCross=" + isFrameCross + "&IsDelayedSend=" + isDelayedSend;
                break;
            case "AccepterOfDept":
                title = "选择接受人";
                width = window.innerWidth * 4 / 5;
                height = 80;
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/AccepterOfDept.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&PWorkID=" + GetQueryString("PWorkID") + "&ToNode=" + toNode + "&s=" + Math.random() + "&isFrameCross=" + isFrameCross + "&IsDelayedSend=" + isDelayedSend;
                break;
            case "AccepterOfOfficer":
                title = "选择联络员";
                width = window.innerWidth * 3 / 5;
                height = 80;
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/AccepterOfOfficer.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&PWorkID=" + GetQueryString("PWorkID") + "&ToNode=" + toNode + "&s=" + Math.random() + "&isFrameCross=" + isFrameCross + "&IsDelayedSend=" + isDelayedSend;
                break;
            case "DBTemplate":
                title = "历史发起记录&模版";
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/DBTemplate.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "CH":
                title = "节点时限";
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/CH.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "Note":
                title = "备注";
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/Note.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
            case "PR":
                title = "重要性设置";
                width = window.innerWidth * 1 / 2;
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/PRI.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&PRIEnable=" + paramData.PRIEnable + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "Rollback":
                title = "回滚";
                width = window.innerWidth / 2;
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/Rollback.htm?WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random();
                break;
            case "JumpWay":
                title = "流程节点跳转";
                width = window.innerWidth / 2;
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/JumpWay.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "QRCode":
                title = "二维码扫描";
                width = window.innerWidth / 2;
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/QRCode/GenerCode.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&FID=" + paramData.FID + "&PWorkID=" + paramData.PWorkID + "&Info=&s=" + Math.random();
                break;
            //case "FrmDBVer":
            //    title = "数据版本";
            //    width = window.innerWidth * 4 / 5;
            //    modalIframeSrc = ccbpmPath + "/WF/CCForm/FrmDBVer.htm?FK_Node=" + paramData.FK_Node + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&s=" + Math.random();
            //    window.open(modalIframeSrc);
            //    return;
            case "Scrip":
                OpenScrip();

                return;
            case "FlowBBS":
                var type = $("input[name=FlowBBS]").attr("data-info");
                title = "评论";
                width = window.innerWidth * 4 / 5;
                height = 80;
                modalIframeSrc = ccbpmPath + "/WF/CCBill/OptComponents/FrmBBS.htm?FrmID=ND" + parseInt(paramData.FK_Flow) + "Rpt" + "&WorkID=" + paramData.WorkID + "&s=" + Math.random();
                if (type == 2)
                    modalIframeSrc += "&IsReadonly=1";
                break;
            case "IM":
                title = "即时通讯";
                width = window.innerWidth / 3;
                height = 80;
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/IM.htm?NodeID=" + paramData.FK_Node + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&s=" + Math.random();
                OpenLayuiDialog(modalIframeSrc, title, width, height, "rb", false, false, false, null, null, null, 0);
                break;
            case "OpenFrm":
                title = "查看表单";
                var workID = paramData.WorkID;
                if (paramData.WorkID == 0 && paramData.FID != 0)
                    workID = paramData.FID;
                width = window.innerWidth * 4/5;
                height = 80;
                modalIframeSrc = ccbpmPath + "/WF/MyFrm.htm?NodeID=" + paramData.FK_Node + "&FK_Node=" + paramData.FK_Node + "&WorkID=" + workID + "&FK_Flow=" + paramData.FK_Flow + "&s=" + Math.random();
                OpenLayuiDialog(modalIframeSrc, title, width, height, "auto");
            default:
                break;
        }
    }
    height = 100;
    if (isShowColseBtn == 0)
        OpenLayuiDialog(modalIframeSrc, title, width, height, "r", false, false, false, null, null, null, 0);
    else
        OpenLayuiDialog(modalIframeSrc, title, width, height, "r");
    return false;
}


function OpenScrip() {
    var en = new Entity("BP.WF.GenerWorkFlow", GetQueryString("WorkID"));
    var ScripNodeID = en.GetPara("ScripNodeID");
    var msg = en.GetPara("ScripMsg");
    if (ScripNodeID != GetQueryString("FK_Node"))
        msg = "";
    if (msg == null || msg == undefined)
        msg = "";

    layer.open({
        type: 1
        , title: "小纸条"
        , closeBtn: false
        , area: ['40%', '40%']
        , shade: 0
        , id: 'Layui_Scrip' //设定一个id，防止重复弹出
        , btn: ['保存', '取消']
        , btnAlign: 'c'
        , moveType: 1 //拖拽模式，0或者1
        , content: '<textarea style="margin:10px;width:95%"  placeholder="请输入内容" class="layui-textarea">' + msg + '</textarea>'
        , yes: function (index, layero) {
            layer.close(index);
            en.SetPara("ScripNodeID", GetQueryString("FK_Node"));
            en.SetPara("ScripMsg", layero.find("textarea").val());
            en.Update();
        }
    });
}
//禁用按钮功能
function setToobarDisiable() {
    $('#ToolBar input').css('background', 'gray');
    $('#ToolBar input').attr('disabled', 'disabled');
}

//启用按钮功能
function setToobarEnable() {
    $('#ToolBar input').css('background', '');
    $('#ToolBar input').removeAttr('disabled');
}

//初始化发送节点下拉框
function InitToNodeDDL(JSonData, wf_node) {

    if (JSonData.ToNodes == undefined)
        return;

    if (JSonData.ToNodes.length == 0)
        return;

    //如果没有发送按钮，就让其刷新,说明加载不同步.
    var btn = $('[name=SendBtn]');
    if (btn == null || btn == undefined) {
        Reload();
        return;
    }

    //如果是会签且不是主持人时，则发送给主持人，不需要选择下一个节点和接收人
    if (btn.length != 0) {
        var dataType = $(btn[0]).attr("data-type");
        if (dataType != null && dataType != undefined && dataType == "isAskFor")
            return;
    }
    var _html = '<button  type="button" class="layui-bar layui-btn layui-btn-sm layui-btn-primary " id="Btn_ToNode">';
    _html += '<span id="DDL_ToNode"></span>';
    _html += '<input type="hidden" id="TB_ToNode"/>';
   _html += '<i class="layui-icon layui-icon-down layui-font-12"></i>';
    _html += '</button>';
    $('[name=SendBtn]').after(_html);
    var data = [];
    var isSelected = false;
    $.each(JSonData.ToNodes, function (i, toNode) {
        var item = {
            id: toNode.No,
            title: toNode.Name,
            DeliveryParas: toNode.DeliveryParas,
            IsSelectEmps: toNode.IsSelectEmps,
            IsSelected: toNode.IsSelected
        }
        data.push(item)
        if (item.IsSelected == "1") {
            isSelected = true;
            $("#DDL_ToNode").html(item.title);
            $("#TB_ToNode").data(item);
        }

    })

    if (isSelected == false) {
        $("#DDL_ToNode").html(data[0].title);
        $("#TB_ToNode").data(data[0]);
    }

    btnMenu = layui.dropdown.render({
        elem: '#Btn_ToNode',
        data: data,
        click: function (obj) {
            $("#DDL_ToNode").html(obj.title);
            $("#TB_ToNode").data(obj);
        }
    });
    if (wf_node.CondModel == 3) {
        var _html = "";
        $.each(JSonData.ToNodes, function (i, toNode) {
            var obj = $("<Button type='button' class='layui-btn layui-btn-sm layui-btn-primary' id='" + toNode.No + "' name='ToNodeBtn' enable=true ><img src='" + basePath + "/WF/Img/Btn/Send.png' width='22px' height='22px'>&nbsp;" + toNode.Name + "</button>");
            $('[name=SendBtn]').before(obj);
            obj.data(toNode);
        });
        $("#Btn_ToNode").hide();
        $('[name=SendBtn]').hide();
        $('[name=ToNodeBtn]').on('click', function () {
            var toNode = this.id;
            $("#TB_ToNode").data($("#" + toNode).data());
            $('[name=SendBtn]').trigger("click");

        });
    }

}

function beforeSendCheck(formType) {
    if (wf_node != null && wf_node.ScripRole == 2) {
        var gwf = new Entity("BP.WF.GenerWorkFlow", GetQueryString("WorkID"));
        var ScripNodeID = gwf.GetPara("ScripNodeID");
        var msg = gwf.GetPara("ScripMsg");
        if (ScripNodeID != GetQueryString("FK_Node"))
            msg = "";
        if (msg == null || msg == undefined)
            msg = "";
        var val = promptGener("请输入要传达的信息,可以为空.", msg);
        if (val != null && val != '') {
            gwf.SetPara("ScripNodeID", GetQueryString("FK_Node"));
            gwf.SetPara("ScripMsg", val);
            gwf.Update();
        }
    }
    //如果启用了流程流转自定义，必须设置选择的游离态节点
    if ($('[name=TransferCustom]').length > 0) {
        var ens = new Entities("BP.WF.TransferCustoms");
        ens.Retrieve("WorkID", paramData.WorkID, "IsEnable", 1);
        if (ens.length == 0) {
            alert("该节点启用了流程流转自定义，但是没有设置流程流转的方向，请点击流转自定义按钮进行设置");
            return false;
        }
        msg = "";
        $.each(ens, function (i, en) {
            if (en.Worker == null || en.Worker == "")
                msg += "节点[" + en.NodeName + "],";
        })
        if (msg != "") {
            msg += "没有设置接收人。";
            alert(msg);
            return false;
        }
    }

    //发送前事件
    if (typeof beforeSend != 'undefined' && beforeSend instanceof Function)
        if (beforeSend() == false)
            return false;

    /**发送前处理的信息 Start**/
    //SDK表单
    if (formType == 3) {
        if (SDKSend() == false)
            return false;
    }

    //嵌入式表单
    if (formType == 2) {
        if (SendSelfFrom() == false)
            return false;
    }
    //表单方案：傻瓜表单、自由表单、开发者表单、累加表单、绑定表单库的表单（单表单)
    if (formType == 0 || formType == 1 || formType == 10 || formType == 11 || formType == 12) {
        if (NodeFormSend() == false)
            return false;
    }

    //绑定多表单
    if (formType == 5)
        if (FromTreeSend() == false)
            return false;

    if (IsRecordUserLog == true) {
        if (paramData.FK_Node == parseInt(paramData.FK_Flow) + "01")
            UserLogInsert("StartFlow", "发起流程");
        else
            UserLogInsert("TodoList", "处理待办");
    }
    return true;
}
/**
 * 流程发送的方法,这个是通用的方法
 * @param {isHuiQian} isHuiQian 是否是会签模式
 * @param {formType} formType 表单方案模式
 */
var IsRecordUserLog = getConfigByKey("IsRecordUserLog", false);
var isSaveOnly = true;
function Send(isHuiQian, formType, isDelayedSend) {
    isDelayedSend = isDelayedSend || 0;
    if (beforeSendCheck(formType) == false)
        return false;

    /**发送前处理的信息 End**/
    var isShowToNode = true;
    if (wf_node != null && wf_node.CondModel == 1 && wf_node.IsBackTrack == 0) {
        //协作模式
        if (wf_node.TodolistModel == 1) {
            var gwf = new Entity("BP.WF.GenerWorkFlow", GetQueryString("WorkID"));
            var huiqianSta = gwf.GetPara("HuiQianTaskSta");
            var todoEmps = gwf.TodoEmps.split(";");
            if (todoEmps.length > 1)
                isShowToNode = false;
        }
        if (isShowToNode == true) {
            isSaveOnly = true;
            $('[name=SaveBtn]').attr("saveType", 1);
            $('[name=SaveBtn]').trigger("click");
            $('[name=SaveBtn]').attr("saveType",0);
            if (isSaveOnly == false)
                return;
            var url = ccbpmPath + "/WF/WorkOpt/ToNodes.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&PWorkID=" + GetQueryString("PWorkID") + "&IsSend=0" + "&s=" + Math.random();

            initModal("SelectNodeUrl", null, url, isDelayedSend);
            return false;
        }

    }

    window.hasClickSend = true; //标志用来刷新待办.

    var toNodeID = 0;
    var isReturnNode = 0;


    //含有发送节点 且接收
    if ($('#DDL_ToNode').length > 0) {
        var gwf = new Entity("BP.WF.GenerWorkFlow", GetQueryString("WorkID"));

        var isLastHuiQian = true;
        //待办人数
        var todoEmps = gwf.TodoEmps;
        if (todoEmps != null && todoEmps != undefined) {
            var huiqianSta = gwf.GetPara("HuiQianTaskSta") == 1 ? true : false;
            if (wf_node.TodolistModel == 1 && huiqianSta == true && todoEmps.split(";").length > 1)
                isLastHuiQian = false;
        }

        var selectToNode = $('#TB_ToNode').data();
        toNodeID = selectToNode.id;
        if (selectToNode.IsSelected == 2)
            isReturnNode = 1;
        if (selectToNode.IsSelectEmps == "1" && isLastHuiQian == true) { //跳到选择接收人窗口
            isSaveOnly = true;
            $('[name=SaveBtn]').attr("saveType", 1);
            $('[name=SaveBtn]').trigger("click");
            $('[name=SaveBtn]').attr("saveType", 0);
            if (isSaveOnly == false)
                return;
            if (isHuiQian == true) {
                initModal("HuiQian", toNodeID);
            } else {
                initModal("sendAccepter", toNodeID, null, isDelayedSend);
            }
            return false;
        }
        if (selectToNode.IsSelectEmps == "2") {
            isSaveOnly = true;
            $('[name=SaveBtn]').attr("saveType", 1);
            $('[name=SaveBtn]').trigger("click");
            $('[name=SaveBtn]').attr("saveType", 0);
            if (isSaveOnly == false)
                return;
            if (isHuiQian == true) {
                initModal("HuiQian", toNodeID);
            } else {
                var url = selectToNode.DeliveryParas;
                if (url != null && url != undefined && url != "") {
                    url = url + "?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&ToNode=" + toNodeID + "&s=" + Math.random();
                    initModal("BySelfUrl", toNodeID, url, isDelayedSend);
                    return false;
                }
            }
        }
        if (selectToNode.IsSelectEmps == "3") {
            isSaveOnly = true;
            $('[name=SaveBtn]').attr("saveType", 1);
            $('[name=SaveBtn]').trigger("click");
            $('[name=SaveBtn]').attr("saveType", 0);
            if (isSaveOnly == false)
                return;
            //Save(1); //执行保存.
            if (isHuiQian == true) {
                initModal("HuiQian", toNodeID);
            } else {
                initModal("sendAccepterOfOrg", toNodeID, null, isDelayedSend);
            }
            return false;
        }

        if (selectToNode.IsSelectEmps == "4") {
            isSaveOnly = true;
            $('[name=SaveBtn]').attr("saveType", 1);
            $('[name=SaveBtn]').trigger("click");
            $('[name=SaveBtn]').attr("saveType", 0);
            if (isSaveOnly == false)
                return;

            if (isHuiQian == true) {
                initModal("HuiQian", toNodeID);
            } else {
                initModal("AccepterOfDept", toNodeID, null, isDelayedSend);
            }
            return false;
        }
        if (selectToNode.IsSelectEmps == "5") {
            isSaveOnly = true;
            $('[name=SaveBtn]').attr("saveType", 1);
            $('[name=SaveBtn]').trigger("click");
            $('[name=SaveBtn]').attr("saveType", 0);
            if (isSaveOnly == false)
                return;

            if (isHuiQian == true) {
                initModal("HuiQian", toNodeID);
            } else {
                initModal("AccepterOfOfficer", toNodeID, null, isDelayedSend);
            }
            return false;
        }
        if (isHuiQian == true) {
            isSaveOnly = true;
            $('[name=SaveBtn]').attr("saveType", 1);
            $('[name=SaveBtn]').trigger("click");
            $('[name=SaveBtn]').attr("saveType", 0);
            if (isSaveOnly == false)
                return;
            initModal("HuiQian", toNodeID);
            return false;
        }
    }

    //执行发送.
    execSend(toNodeID, formType, isReturnNode);
}

//const asyncLoad = () => new Promise((resolve, _) => {
//    const index = layer.open( {
//        content: '正在发送...',
//        shade: [0.2, '#000'],
//        title: '',
//        btn: [],
//        closeBtn: 0,
//        icon:16,
//    })
//    setTimeout(() => {
//        resolve(index);
//    },16)

//})

function execSend(toNodeID, formType, isReturnNode) {
    //正在发送弹出层
    //var index = layer.msg('正在发送，请稍后..', {
    //    icon: 16
    //    , shade: 0.01,
    //});
    //var index = layer.load();//此处用layui加载动画
    //先设置按钮等不可用.
    setToobarDisiable();
    
    layui.form.on('submit(Send)', async function (data) {
        //提交信息的校验
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
        handler.AddUrlData();
        if (formType != 3 && formType != 2) {
            var formData = getFormData(data.field);
            for (var key in formData) {
                handler.AddPara(key, encodeURIComponent(formData[key]));
            }
        }

        handler.AddPara("ToNode", toNodeID);
        handler.AddPara("IsReturnNode", isReturnNode);
        const idx = await asyncLoad("正在发送，请稍后..");
        try {
            var data = handler.DoMethodReturnString("Send"); //执行保存方法.
            layer.close(idx);//关闭正在发送
            if (data.indexOf('err@') == 0) { //发送时发生错误
                var reg = new RegExp('err@', "g")
                var data = data.replace(reg, '');
                layer.alert(data);
                setToobarEnable();
                return false;
            }

            if (data.indexOf('TurnUrl@') == 0) {  //发送成功时转到指定的URL 
                var url = data;
                url = url.replace('TurnUrl@', '');
                SetHref(url);
                return false;
            }
            if (data.indexOf('SelectNodeUrl@') == 0) {
                var url = data;
                url = url.replace('SelectNodeUrl@', '');
                initModal("SelectNodeUrl", null, url);
                return false;
            }

            if (data.indexOf('BySelfUrl@') == 0) {  //发送成功时转到自定义的URL 
                var url = data;
                url = url.replace('BySelfUrl@', '');
                initModal("BySelfUrl", null, url);
                return false;
            }


            if (data.indexOf('url@') == 0) {  //发送成功时转到指定的URL 

                if (data.indexOf("AccepterOfOrg") != -1) {
                    var params = data.split("&");

                    for (var i = 0; i < params.length; i++) {
                        if (params[i].indexOf("ToNode") == -1)
                            continue;

                        toNodeID = params[i].split("=")[1];
                        break;
                    }
                    initModal("sendAccepterOfOrg", toNodeID);
                    return false;
                }

                if (data.indexOf("AccepterOfDept") != -1) {
                    var params = data.split("&");

                    for (var i = 0; i < params.length; i++) {
                        if (params[i].indexOf("ToNode") == -1)
                            continue;

                        toNodeID = params[i].split("=")[1];
                        break;
                    }
                    initModal("AccepterOfDept", toNodeID);
                    return false;
                }
                if (data.indexOf("AccepterOfOfficer") != -1) {
                    var params = data.split("&");

                    for (var i = 0; i < params.length; i++) {
                        if (params[i].indexOf("ToNode") == -1)
                            continue;

                        toNodeID = params[i].split("=")[1];
                        break;
                    }
                    initModal("AccepterOfOfficer", toNodeID);
                    return false;
                }

                if (data.indexOf('Accepter') != 0 && data.indexOf('AccepterGener') == -1) {

                    //求出来 url里面的FK_Node=xxxx 
                    var params = data.split("&");

                    for (var i = 0; i < params.length; i++) {
                        if (params[i].indexOf("ToNode") == -1)
                            continue;

                        toNodeID = params[i].split("=")[1];
                        break;
                    }
                    initModal("sendAccepter", toNodeID);
                    return false;
                }


                var url = data;
                url = url.replace('url@', '');

                SetHref(url);
                return false;
            }
            OptSuc(data);

            return false;
        } catch {
            layer.close(idx);
        }
    });


}

//调用后，就关闭刷新按钮.
function returnWorkWindowClose(data) {

    if (data == "" || data == "取消") {
        layer.close(layer.index);
        setToobarEnable();
        return;
    }
    layer.close(layer.index);
    if (data == undefined) {
        //应该关闭页面
        if (window.top.vm && typeof window.top.vm.closeCurrentTabs == "function")
            window.top.vm.closeCurrentTabs(window.top.vm.selectedTabsIndex);
        else {
            // 取得父页面URL，用于判断是否是来自测试流程
            var pareUrl = window.top.document.referrer;
            if (pareUrl.indexOf("test") != -1 || pareUrl.indexOf("Test") != -1) {
                // 测试流程时，发送成功刷新测试容器页面右侧
                window.parent.parent.refreshRight();
            }
            window.close();
        }
        return;
    }
    //通过下发送按钮旁的下拉框选择下一个节点
    if (data != null && data != undefined && data.indexOf('SaveOK@') == 0) {
        //说明保存人员成功,开始调用发送按钮.
        var toNode = 0;
        //含有发送节点 且接收
        if ($('#TB_ToNode').length > 0) {
            var selectToNode = $('#TB_ToNode').data();
            toNode = selectToNode.No;
        }

        execSend(toNode);
        return false;
    } else {//可以重新打开接收人窗口
        winSelectAccepter = null;
    }

    if (data != null && data != undefined && (data.indexOf('err@') == 0 || data == "取消")) {//发送时发生错误
        layer.alert(data);
        return false;
    }

    OptSuc(data);
}


//发送 退回 移交等执行成功后转到  指定页面
var interval;
function OptSuc(msg) {
    if (window.parent != null && window.parent.WindowCloseReloadPage != null && typeof window.parent.WindowCloseReloadPage === "function") {
        window.parent.WindowCloseReloadPage(msg);
    } else {
        if (typeof WindowCloseReloadPage != 'undefined' && WindowCloseReloadPage instanceof Function)
            WindowCloseReloadPage(msg);
    }
    layer.close(layer.index);
    if (msg == null || msg == undefined)
        msg = "";
    msg = msg.replace("@查看<img src='./Img/Btn/PrintWorkRpt.gif' >", '')
    msg = msg.replace(/@/g, '<br/>').replace(/null/g, '');
    if (msg.indexOf("err@") != -1) {
        layer.alert(msg);
        setToobarEnable();
        return false;
    }
    var WF_WorkOpt_LeftSecond = getConfigByKey("WF_WorkOpt_LeftSecond", 30);
    layer.open({
        type: 1
        , id: 'layerDemo'
        , content: msg
        , btn: ["确定(" + WF_WorkOpt_LeftSecond-- + "秒)"]
        , btnAlign: 'r' //按钮居中
        , area: ['40%', '40%']
        , shade: 0

        , yes: function () {
            layer.closeAll();
        }
        , success: function (layero, index) {
            var timer = null;
            var fn = function () {
                layero.find(".layui-layer-btn0").text("确定(" + WF_WorkOpt_LeftSecond + "秒)");
                if (WF_WorkOpt_LeftSecond == 0) {
                    layer.close(index);
                    clearInterval(timer);
                    closeWindow();
                }
                WF_WorkOpt_LeftSecond--;
            };
            timer = setInterval(fn, 1000);
            fn();
        }
        , end: function () {
            clearInterval(interval);
            closeWindow();
        }
    });
    $("#layerDemo").css("padding", "0px 20px");
}
/**
 * 关闭弹出消息页面同时关闭父页面
 */
function closeWindow() {
    if (window.top.vm && typeof window.top.vm.closeCurrentTabs == "function") {
        if (window.top.vm.selectedTabsIndex == undefined)
            window.close();
        else
            window.top.vm.closeCurrentTabs(window.top.vm.selectedTabsIndex);
    }
    else {
        // 取得父页面URL，用于判断是否是来自测试流程
        var pareUrl = window.top.document.referrer;
        if (pareUrl.indexOf("test") != -1 || pareUrl.indexOf("Test") != -1) {
            // 测试流程时，发送成功刷新测试容器页面右侧
            window.parent.parent.refreshRight();
        }
        window.close();
    }
}

/**
 * SDK表单的发送前的验证
 */
function SDKSend() {
    if (Save() == false) {
        alert("信息保存失败");
        return false;
    }
    //审核信息的保存
    if ($("#WorkCheck_Doc").length == 1 ||$("#WorkCheck_Doc0").length == 1) {
        //保存审核信息
        SaveWorkCheck();
        if (isCanSend == false)
            return false;
    }
    return true;
}

/**
 * 节点表单发送前的验证
 */
function NodeFormSend() {

    if (typeof isChartFrm != "undefined" && isChartFrm == true && $("#ChapterIFrame").length > 0) {
        //获取IFrame的页面
        var frame = $("#ChapterIFrame")[0];
        if (frame && frame.contentWindow.Save != undefined && typeof (frame.contentWindow.Save) == "function") {
            var flag = frame.contentWindow.Save(false);
            if (flag == false) {
                return false;
            }
        }
        return true;
    }
    //检查，保存从表
    if ($("iframe[name=Dtl]").length > 0) {
        var formCheckResult = true;
        $("iframe[name=Dtl]").each(function (i, obj) {
            var contentWidow = obj.contentWindow;
            if (contentWidow != null) {
                if (contentWidow.SaveAll != undefined && typeof (contentWidow.SaveAll) == "function") {
                    var isTrue = contentWidow.SaveAll();
                    if (isTrue == false)
                        formCheckResult = false;
                }
                
                //最小从表明细不为0
                if (contentWidow.CheckDtlNum != undefined && typeof (contentWidow.CheckDtlNum) == "function") {
                    if (contentWidow.CheckDtlNum() == false) {
                        layer.alert("[" + contentWidow.sys_MapDtl.Name + "]明细不能少于最小数量" + contentWidow.sys_MapDtl.NumOfDtl + "");
                        formCheckResult = false;
                    }
                }
               
            }


        });
        if (formCheckResult == false)
            return false;

    }

    //审核组件
    if ($("#WorkCheck_Doc").length == 1 || $("#WorkCheck_Doc0").length == 1) {
        //保存审核信息
        SaveWorkCheck();
        if (isCanSend == false)
            return false;
    }

    if (checkBlanks() == false) {
        layer.alert("必填项不能为空");
        return false;
    }
    if (typeof checkReg == "undefined")
        return true;
    if (checkReg() == false)
        return false;
    //附件检查
    var msg = checkAths();
    if (msg != "") {
        alert(msg);
        return false;
    }


    return true;
}

/**
 * 绑定多表单发送前的验证
 */
function FromTreeSend() {
    //保存前事件
    if (typeof beforeSend != 'undefined' && beforeSend instanceof Function)
        if (beforeSend() == false)
            return false;
    if (Save(1) == false)
        return false;
    return true;
}
/**
 * 嵌入式表单
 */
function SendSelfFrom() {
    var val = SaveSelfFrom();
    if (val == false) {
        return false;
    }

    if (val != true) {
        //就说明是传来的参数，这些参数需要存储到WF_GenerWorkFlow里面去，用于方向条件的判断。
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
        handler.AddPara("WorkID", GetQueryString("WorkID"));
        handler.AddPara("Paras", val);
        handler.DoMethodReturnString("SaveParas");
    }
    return true;
}

/**
 * 暂时不起作用
 */
function SysCheckFrm() {
}

/**
 * 保存后的事件
 * @param {any} formType 表单方案类型
 */
function SaveEnd(formType) {
    //SDK表单，保存表单中的信息
    if (formType == 3) {

    }
}

//关注 按钮.
function FocusBtn(btn, workid) {
    btn = $('[name=Focus]');
    if (btn.length == 1) {
        if (btn[0].innerText.trim() == "关注") {
            btn[0].innerHTML = "<img src='Img/Btn/Focus.png' width='22px' height='22px'>&nbsp;取消关注";
        }
        else {
            btn[0].innerHTML = "<img src='Img/Btn/Focus.png' width='22px' height='22px'>&nbsp;关注";
        }

        var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
        handler.AddPara("WorkID", workid);
        handler.DoMethodReturnString("Focus"); //执行保存方法.
    }
   
}

//确认 按钮.
function ConfirmBtn(btn, workid) {

    if (btn.innerText.trim() == '确认') {
        btn.innerHTML = "<img src='Img/Btn/Focus.png' width='22px' height='22px'>&nbsp;取消确认";
    }
    else {
        btn.innerHTML = "<img src='Img/Btn/Focus.png' width='22px' height='22px'>&nbsp;确认";
    }

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddPara("WorkID", workid);
    handler.DoMethodReturnString("Confirm");

}
//结束流程.
function DoStop(msg, flowNo, workid) {
    layer.confirm('您确定要执行 [' + msg + '] ?', function (index) {
        layer.close(index);
        //流程结束前
        if (typeof beforeStopFow != 'undefined' && beforeStopFow instanceof Function)
            if (beforeStopFow() == false)
                return false;
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
        handler.AddPara("FK_Flow", flowNo);
        handler.AddPara("WorkID", workid);
        var data = handler.DoMethodReturnString("MyFlow_StopFlow");
        layer.alert(data);
        if (data.indexOf('err@') == 0)
            return;
        //流程结束后
        if (typeof afterStopFow != 'undefined' && afterStopFow instanceof Function)
            if (afterStopFow() == false)
                return false;
        closeWindow();
    })

}
//执行分支流程退回到分合流节点。
function DoSubFlowReturn(fid, workid, fk_node) {
    var url = 'ReturnWorkSubFlowToFHL.htm?FID=' + fid + '&WorkID=' + workid + '&FK_Node=' + fk_node;
    var v = WinShowModalDialog(url, 'df');
    SetHref(window.history.url);
}

//结束子流程
function DoDelSubFlow(fk_flow, workid) {
    if (window.confirm('您确定要终止进程吗？') == false)
        return;
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddPara("FK_Flow", fk_flow);
    handler.AddPara("WorkID", workid);

    var data = handler.DoMethodReturnString("DelSubFlow"); //删除子流程..
    alert(data);
    Reload();

}


//删除流程
function DeleteFlow() {

   //让用户决定的方式删除
    if (wf_node.DelEnable == 4) {
        //按照用户选择的方式删除
        var url = "./WorkOpt/DeleteFlowInstance.htm?WorkID=" + paramData.WorkID + "&FK_Node=" + paramData.FK_Node + "&FK_Flow=" + paramData.FK_Flow;
        var  width = window.innerWidth * 3 / 5;
        var  height = 80;
        OpenLayuiDialog(url, "选择删除的方式", width, height, "auto");
        
        return;
    }

    
    //彻底删除
    if (wf_node.DelEnable == 3) {
        layer.confirm('删除后流程实例不可恢复,你确定要彻底删除流程吗?', function (index) {
            layer.close(index);
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
            handler.AddPara("FK_Flow", GetQueryString("FK_Flow"));
            handler.AddPara("FK_Node", GetQueryString("FK_Node"));
            handler.AddPara("WorkID", GetQueryString("WorkID"));
            handler.AddPara("DelEnable", wf_node.DelEnable);
            var str = handler.DoMethodReturnString("DeleteFlow");
            layer.alert(str);
            closeWindow();
        });
        return;
    }

    //逻辑删除/写入日志方式的删除
    layer.prompt({
        formType: 2,
        value: '删除原因',
        title: '请输入删除流程的原因',
        offset: '100px'
    }, function (value, index, elem) {
        layer.close(index);
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
        handler.AddPara("FK_Flow", GetQueryString("FK_Flow"));
        handler.AddPara("FK_Node", GetQueryString("FK_Node"));
        handler.AddPara("WorkID", GetQueryString("WorkID"));
        handler.AddPara("Msg", value);
        handler.AddPara("DelEnable", wf_node.DelEnable);
        var str = handler.DoMethodReturnString("DeleteFlow");
        layer.alert(str);
        closeWindow();
       
    });
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
        layer.alert(data);
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
        layer.alert(data);
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
        layer.alert(data);
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
        toNodeID=0;
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
        layer.alert(data);
        Reload();
        return;
    }
    layer.alert('您没有权限增加人员.');
   
}
/**------------------------子线程退回分流节点的工作处理器按钮操作-------------------------------**/

//催办
function Press() {
    var msg = window.prompt('请输入催办信息', '该工作因为xxx原因，需要您优先处理.');
    if (msg == null)
        return;
    var handler = new HttpHandler("BP.WF.HttpHandler.WF");
    handler.AddUrlData();
    handler.AddPara("Msg", msg);
    var data = handler.DoMethodReturnString("Runing_Press");

    if (data.indexOf('err@') == 0) {
        console.log(data);
        layer.alert(data);
        return;
    }

    layer.alert(data);
}

/***
 * 
 * 撤销
 */
function UnSend(type) {
    type = type || 0;
    if (window.confirm('您确定要撤销本次发送吗？') == false)
        return;
    
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyView");
    handler.AddUrlData();
    handler.AddPara("IsUnDelayedSend", type);
    var data = handler.DoMethodReturnString("MyView_UnSend");
    if (data.indexOf('err@') == 0) {
        data = data.replace('err@', '');
        data = data.replace('err@', '');
        alert(data);
        return;
    }

    var url = 'MyFlow.htm?FK_Flow=' + GetQueryString("FK_Flow") + '&WorkID=' + GetQueryString("WorkID") + '&FID=' + GetQueryString("FID");
    SetHref(url);
    return;
}
/**
 * 发起子流程
 * @param {any} subFlowNo 子流程编号
 */
function SendSubFlow(subFlowNo, subFlowMyPK) {
    var W = document.body.clientWidth - 340;
   
    var url = "./WorkOpt/SubFlowGuid.htm?SubFlowMyPK=" + subFlowMyPK + "&WorkID=" + GetQueryString("WorkID") + "&FK_Flow=" + GetQueryString("FK_Flow") + "&FK_Node=" + GetQueryString("FK_Node");
    OpenLayuiDialog(url,"选择", W,80,
        "auto", false, true, true, function () {
            debugger
            var iframe = $(window.frames["dlg"]).find("iframe");
            if (iframe) {
                var result = iframe[0].contentWindow.Btn_OK();
                if (result == true) {
                    var subFlowGuid = new Entity("BP.WF.Template.SFlow.SubFlowHandGuide", subFlowMyPK);
                    if (subFlowGuid.SubFlowHidTodolist == 1) {
                        if (window.parent != null && window.parent != undefined)
                            window.parent.close();
                        else
                            window.close();

                    }
                    //显示子流程信息
                    var html = SubFlow_Init(wf_node);
                    $("#SubFlow").html("").html(html);
                }

            }

        }, null, function () {

        });

}
/**
 * 发送子线程
 */
function StartThread() {
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddPara("WorkID", GetQueryString("WorkID"));
    handler.AddPara("FK_Flow", GetQueryString("FK_Flow"));
    handler.AddPara("FK_Node", GetQueryString("FK_Node"));
    var data = handler.DoMethodReturnString("MyFlow_StartThread");
    if (data.indexOf("err@") != -1) {
        alert(data);
        console.log(data);
        return;
    }

    if (data == null || data == undefined)
        data = "";
    data = data.replace("@查看<img src='./Img/Btn/PrintWorkRpt.gif' >", '');
    data = data.replace(/@/g, '<br/>').replace(/null/g, '');

    layer.open({
        type: 1
        , id: 'HelpAlter'
        , content: msg
        , btn: ["确定"]
        , btnAlign: 'r' //按钮居中
        , area: ['40%', '40%']
        , shade: 0

        , yes: function () {
            layer.closeAll();
        }
        , success: function (layero, index) {
            if ($("#SubFlow").length == 1) {
                $("#SubFlow").html(SubFlow_Init(wf_node));
            }
        }
    });
    $("#HelpAlter").css("padding", "0px 20px");

}


/**
 * 帮助提醒
 */
function HelpAlter() {
    if (wf_node == null)
        return;
    var node = wf_node;
    //判断该节点是否启用了帮助提示 0 禁用 1 启用 2 强制提示 3 选择性提示
    var btnLab = new Entity("BP.WF.Template.BtnLab", node.NodeID);
    if (btnLab.HelpRole != 0) {
        var count = 0;
        if (btnLab.HelpRole == 3) {
            var mypk = webUser.No + "_ND" + node.NodeID + "_HelpAlert";
            var userRegedit = new Entity("BP.Sys.UserRegedit");
            userRegedit.SetPKVal(mypk);
            count = userRegedit.RetrieveFromDBSources();
        }

        if (btnLab.HelpRole == 2 || (count == 0 && btnLab.HelpRole == 3)) {
            var filename = basePath + "/DataUser/CCForm/HelpAlert/" + node.NodeID + ".htm";
            var htmlobj = $.ajax({ url: filename, async: false });
            if (htmlobj.status == 404)
                return;
            var str = htmlobj.responseText;
            if (str != null && str != "" && str != undefined) {
                layer.open({
                    type: 1
                    , id: 'HelpAlter'
                    , content: str
                    , btnAlign: 'r' //按钮居中
                    , area: ['40%', '40%']
                    , shade: 0
                    , success: function (layer, index) {
                        //保存用户的帮助指引信息操作
                        var mypk = webUser.No + "_ND" + node.NodeID + "_HelpAlert"
                        var userRegedit = new Entity("BP.Sys.UserRegedit");
                        userRegedit.SetPKVal(mypk);
                        var count = userRegedit.RetrieveFromDBSources();
                        if (count == 0) {
                            //保存数据
                            userRegedit.FK_Emp = webUser.No;
                            userRegedit.FK_MapData = "ND" + node.NodeID;
                            userRegedit.Insert();
                        }
                    }
                });
                $("#HelpAlter").css("padding", "0px 20px");
            }
        }
    }
}

//打印Html
function printHtml() {
   if (typeof isFool != "undefined" && isFool == true) {
        initModal("PackUp_html");
        return;
    }
    //判断是否打印单表单
    var targetNode = null;
    var document = window.document;
    if ($("#CCForm").length != 0) {
        var bodyNode = window.document.body.cloneNode(true);
        targetNode = bodyNode.querySelector('#ContentDiv');
        targetNode.querySelectorAll('#CCForm')[0].style.width = $('#ContentDiv').css("width");
        if (!targetNode) {
            alert("没有找到文档节点")
            return
        }
    } else {
        var targetIframes = Array.from(document.querySelectorAll('.tab-iframe'))
        var iframe = targetIframes.find(function (iframe) {
            return iframe.parentNode.style.display !== 'none';
        })
        if (iframe) {
            document = iframe.contentDocument;
            const bodyNode = iframe.contentDocument.body.cloneNode(true);
            targetNode = bodyNode.querySelector('.doc');
            if (!targetNode) {
                alert("没有找到文档节点")
                return
            }
        }
    }

    if (targetNode) {
        targetNode.querySelectorAll('td').forEach(tdElem => {
            tdElem.style.minWidth = '60px';
        })
        targetNode.querySelectorAll('input').forEach(input => {
            //如果是复选框的处理
            if (input.parentNode && (window.getComputedStyle(input).display !== 'none' || input.style.display !== 'none') && (input.type == "checkbox" || input.type == 'radio')) {
                input.style.display = 'inline';
                input.style.marginRight = '5px';
                //他的第一个兄弟节点隐藏
                input.nextElementSibling.style.display = 'none';
                //复选框
                if (input.type == "checkbox") {
                    var val = 0;
                    if (input.nextElementSibling.className.indexOf('layui-form-checked') != -1)
                        val = 1;
                    if (val == 1)
                        input.setAttribute("checked", true);
                    const p = document.createElement("span");
                    p.style.marginRight = '5px';
                    p.innerHTML = input.getAttribute('title');
                    input.parentNode.insertBefore(p, input.nextSibling);
                }
                //单选按钮 
                if (input.type == "radio") {
                    var val = 0;
                    if (input.nextElementSibling.className.indexOf(' layui-form-radioed') != -1)
                        val = 1;
                    if (val == 1)
                        input.setAttribute("checked", true);
                }
            } else {
                if (input.parentNode && window.getComputedStyle(input).display !== 'none' && input.style.display !== 'none' && input.type != 'hidden') {
                    //如果是傻瓜表单
                    if (input.className.indexOf("layui-input") != -1) {
                        input.setAttribute("value", input.value);
                    } else {
                        const p = document.createElement("div");
                        p.innerHTML = input.value

                        p.style = input.style
                        p.style.whiteSpace = 'pre-line'
                        if (input.className.indexOf('layui-unselect') != -1) {
                            input.nextElementSibling.remove();
                            p.style.textAlign = 'left';
                            p.style.paddingLeft = '10px';
                        }
                        if (input.className.indexOf('ccdate') != -1 && input.nextElementSibling!=null)
                            input.nextElementSibling.remove();

                        input.parentNode.appendChild(p);
                        input.parentNode.style.maxHeight = '120px'
                        input.parentNode.style.overflow = 'hidden'
                        input.parentNode.style.fontSzie = '16px'
                        input.parentNode.style.lineHeight = '16px'
                        $(input).hide();
                    }

                    
                }
            }
        })
        targetNode.querySelectorAll('textarea').forEach(textarea => {
            if (textarea.parentNode && window.getComputedStyle(textarea).display !== 'none' && textarea.style.display !== 'none' && textarea.type != 'hidden') {
                const p = document.createElement("div");
                p.innerHTML = textarea.value
                p.style = textarea.style
                p.style.whiteSpace = 'pre-line'
                textarea.parentNode.appendChild(p);
                textarea.parentNode.style.maxHeight = '120px'
                textarea.parentNode.style.overflow = 'hidden'
                textarea.parentNode.style.fontSzie = '16px'
                textarea.parentNode.style.lineHeight = '16px'
                textarea.remove()
            }
        })
        targetNode.querySelectorAll('select').forEach(select => {
            if (select.parentNode && window.getComputedStyle(select).display !== 'none' && select.style.display !== 'none' && select.type != 'hidden') {
                select.style.display = 'none';
            }

        });
        //获取从表信息
        var dtls = Array.from(targetNode.querySelectorAll('div')).filter(div => {
            return div.getAttribute('name') == 'Dtl';
        });
        dtls.forEach(dtl => {
            dtl.children[0].remove();
        })
        const html = generateHTML(targetNode, document.styleSheets, `
                    @media print { 
                        .paper { box-shadow: none !important; page-break-inside: avoid;}
                        .paper_A4 { width: 21cm !important; height: 27cm !important }
                                            .paper + .paper { margin-top: 0px !important }
                        /**body{
                                -webkit-print-color-adjust:exact;
                                -moz-print-color-adjust:exact;
                                -ms-print-color-adjust:exact;
                                print-color-adjust:exact;
                            }*/


                    }
                    .page > div { padding: 1.5cm 0 0 0 !important }
                    @page {
                      size: auto; 
                      margin: 1cm;
                    }
                `)
        const tempIframe = document.createElement('iframe')
        const handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
        handler.AddUrlData();
        handler.AddPara("html", html);
        handler.AddPara("WorkID", GetQueryString('WorkID'));
        handler.AddPara("FK_Node", GetQueryString('FK_Node'));
        const filePath = handler.DoMethodReturnString("CreateHtmlFile");
        tempIframe.src = basePath + '/' + filePath;
        tempIframe.style.height = '0px';
        tempIframe.onload = function () {
            tempIframe.contentWindow.print();
        }
        document.body.appendChild(tempIframe);
    } else {
        alert('当前页面不可打印')
    }
}

function getAllCssStyles(sheets) {
    let styleTag = '';
    for (const sheet of sheets) {
        const rules = sheet.rules || sheet.cssRules;
        for (const rule of rules) {
            styleTag += rule.cssText + ' '
        }
    }
    return styleTag;
}

function generateHTML(dom, styleSheets, extendCssRules = "") {
    var css = getAllCssStyles(styleSheets);
    return `
            <!DOCTYPE html>
            <html lang="en">
                <head>
                    <meta charset="utf-8" />
                    <meta name="viewport" content="width=device-width, initial-scale=1" />
                </head>
                <style>
                    ${css}
                    ${extendCssRules}
                    .DtlTh{
                        background-color:rgb(242, 242, 242) !important;
                        -webkit-print-color-adjust: exact;
                    }
                </style>
                <body>
                    ${dom.innerHTML}
                </body>
            </html>
            `
}




