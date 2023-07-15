var flow = null;
var ccbpmRunModel = 0;
var flowNo = GetQueryString("FK_Flow");
flow = new Entity("BP.WF.Flow", flowNo);

//子流程集合
var subFlows = new Entities("BP.WF.Template.SFlow.SubFlows");
subFlows.Retrieve("FK_Flow", flowNo);
subFlows = subFlows.TurnToArry();
//子流程显示方式
var subFlowShowType = flow.SubFlowShowType;

var webUser = new WebUser();
var basepath = "";
var flowDevModel = flow.GetPara("FlowDevModel") || flow.FlowDevModel ; //设计模式.
var pageFrom = GetQueryString("From");
pageFrom = pageFrom == null || pageFrom == undefined ? "" : pageFrom;

$(function () {

    if (flowDevModel == null || flowDevModel == undefined)
        flowDevModel = 0;

    ccbpmRunModel = webUser.CCBPMRunModel;
    var runModel = GetQueryString("RunModel");
    if (runModel != null && runModel != undefined && runModel == "2")
        ccbpmRunModel = 2;

    if (pageFrom == "")
        basepath = "../";
    else
        basepath = "../Admin/";
    //}
    //设置状态. 根据不同的模式来设计.
    SetState();

    ShowFlowDevModelText();

    $("#pmfun,#nodeMenu").hover(function () {
        var mLeft = parseInt($("#jqContextMenu").css("left").replace('px', ''));
        var mTop = parseInt($("#jqContextMenu").css("top").replace('px', '')) - 50;
        var mmTop = parseInt($("#jqContextMenu >#mainul> #pmfun").get(0).offsetTop);
        $("#nodeMenu").css({ "left": parseInt(mLeft) + 148 + "px", "top": mTop + mmTop + "px" });
        $("#nodeMenu").show();
    }, function () {
        $("#nodeMenu").hide();
    });


    //节点类型--普通
    $('#Node_Ordinary').on('click', function () {
        var nodeID = document.getElementById("leipi_active_id");

        SetNodeRunModel(nodeID.value, 0);

    });
    //节点类型--分流
    $('#Node_FL').on('click', function () {
        var nodeID = document.getElementById("leipi_active_id");
        SetNodeRunModel(nodeID.value, 2);
    });
    //节点类型--合流
    $('#Node_HL').on('click', function () {
        var nodeID = document.getElementById("leipi_active_id");
        SetNodeRunModel(nodeID.value, 1);
    });
    //节点类型--分合流
    $('#Node_FHL').on('click', function () {
        var nodeID = document.getElementById("leipi_active_id");
        SetNodeRunModel(nodeID.value, 3);
    });
    //节点类型--同表单的子线程
    $('#Node_SubThread0').on('click', function () {
        var nodeID = document.getElementById("leipi_active_id");
        SetNodeRunModel(nodeID.value, 4, 0);
    });

    //节点类型--异表单子线程
    $('#Node_SubThread1').on('click', function () {
        var nodeID = document.getElementById("leipi_active_id");
        SetNodeRunModel(nodeID.value, 5, 1);
    });

    //begin 审核组件状态的设置
    $("#pmWorkCheck,#fwcMenu").hover(function () {
        var mLeft = $("#jqContextMenu").css("left").replace('px', '');
        var mTop = $("#jqContextMenu").css("top").replace('px', '');
        $("#fwcMenu").css({ "left": parseInt(mLeft) + 148 + "px", "top": parseInt(mTop) + 62 + "px" });
        $("#fwcMenu").show();
    }, function () {
        $("#fwcMenu").hide();
    });

    //审核状态---禁用
    $('#FWC_Disable').on('click', function () {
        var nodeID = document.getElementById("leipi_active_id");
        SetNodeFWCSta(nodeID.value, 0);
    });
    //审核状态---启用
    $('#FWC_Enable').on('click', function () {
        var nodeID = document.getElementById("leipi_active_id");
        SetNodeFWCSta(nodeID.value, 1);
    });

    //审核状态--只读
    $('#FWC_ReadOnly').on('click', function () {
        var nodeID = document.getElementById("leipi_active_id");
        SetNodeFWCSta(nodeID.value, 2);
    });

    //批量设置
    $('#FWC_Batch').on('click', function () {
        var nodeID = document.getElementById("leipi_active_id").value;
        var url = "../AttrFlow/BatchFWC.htm?FK_Flow=" + GetQueryString("FK_Flow") + "&NodeID=" + nodeID;
        //window.parent.addTab(nodeID, "审核组件状态", url);
        var dgId = "iframDg";
        var w = window.innerWidth - 240;
        var h = window.innerHeight - 120;
        OpenEasyUiDialog(url, dgId, '设置审核组件状态', w, h, 'icon-new', false);
    });

});



//设计表单
function NodeFrmD(nodeID) {

    var node = new Entity("BP.WF.Node", nodeID);
    if (node.FormType == 1 || node.FormType == 0 || node.FormType == 10) { //自由表单，傻瓜表单.
        //傻瓜表单.
        var url = basePath + "/WF/Admin/FoolFormDesigner/Designer.htm?FK_MapData=ND" + nodeID + "&IsFirst=1&FK_Flow=" + flowNo + "&FK_Node=" + nodeID;
        url += "&UserNo=" + GetQueryString("UserNo");
        url += "&Token=" + GetQueryString("Token");
        OpenLayuiDialog(url, "设计表单ND" + nodeID, window.innerWidth * 0.9);
        // SetHref(url);
        return;
    }

    if (node.FormType == 12) { //开发者表单
        //傻瓜表单.
        var url = basePath + "/WF/Admin/DevelopDesigner/Designer.htm?FK_MapData=ND" + nodeID + "&IsFirst=1&FK_Flow=" + flowNo + "&FK_Node=" + nodeID;
        url += "&UserNo=" + GetQueryString("UserNo");
        url += "&Token=" + GetQueryString("Token");

        OpenLayuiDialog(url, "设计表单ND" + nodeID, window.innerWidth * 0.9);
        //window.location.href = filterXSS(url);
        return;
    }

    if (node.FormType == 11) { //RefOneFrmTree, 绑定表单库的单表单.

        var node = new Entity("BP.WF.Node", nodeID);
        var frmID = node.NodeFrmID;
        var myPK = frmID + "_" + nodeID + "_" + GetQueryString("FK_Flow");
        var url = basePath + "/WF/Comm/En.htm?EnName=BP.WF.Template.FrmNodeExt&MyPK=" + myPK + "&Lang=CH";

        OpenLayuiDialog(url, "设计表单" + nodeID, window.innerWidth * 0.9);
        //window.location.href = filterXSS(url);
        //NodeFrmRefOneFrmTree(nodeID);
        return;
    }

    if (node.FormType == 5) { //RefOneFrmTree, 绑定表单库的单表单.

        var url = basePath + "/WF/Admin/Sln/BindFrms.htm?FK_Node=" + nodeID + "&FK_Flow=" + flowNo + "&Lang=CH";

        OpenLayuiDialog(url, "设计表单" + nodeID, window.innerWidth * 0.9);
        //window.location.href = filterXSS(url);
        //NodeFrmRefOneFrmTree(nodeID);
        return;
    }

    alert("自定义表单不能设计,请查看表单方案.");
    //alert("没有判断的表单类型:" + node.FormType);
}


function EidtFrm() {

    var flowNo = GetQueryString("FK_Flow");
    var flow = new Entity("BP.WF.Flow", flowNo);

    var token = GetQueryString("Token");
    var userNo = GetQueryString("UserNo");

    // 极简表单. 
    if (flowDevModel == FlowDevModel.JiJian) {

        var nodeID = parseInt(GetQueryString("FK_Flow")) + "01";
        var node = new Entity("BP.WF.Node", nodeID);
        var url = "";
        if (node.FormType == 1) {
            var url = basePath + "/WF/Admin/FoolFormDesigner/Designer.htm?FK_MapData=ND" + nodeID + "&IsFirst=1&FK_Flow=" + flowNo + "&FK_Node=" + nodeID;
            url += "&Token=" + token;
            url += "&UserNo=" + userNo;
            SetHref(url);
            return;
        }
        //开发者表单.
        if (node.FormType == 12) {
            var url = basePath + "/WF/Admin/DevelopDesigner/Designer.htm?FK_MapData=ND" + nodeID + "&IsFirst=1&FK_Flow=" + flowNo + "&FK_Node=" + nodeID;
            url += "&Token=" + token;
            url += "&UserNo=" + userNo;
            SetHref(url);
            return;
        }
    }

    //绑定单个表单.
    if (flowDevModel == FlowDevModel.RefOneFrmTree) {
        var frmID = flow.FrmUrl;
        var nodeID = parseInt(flowNo + "01");
        var url = basePath + "/WF/Admin/FoolFormDesigner/Designer.htm?FrmID=" + frmID + "&FK_Flow=" + flowNo + "&FK_MapData=" + frmID + "&FK_Node=" + nodeID;
        url += "&UserNo=" + userNo;
        url += "&Token=" + token;
        SetHref(url);
        return;
    }

    //自定义表单.
    if (flowDevModel == FlowDevModel.SDKFrm || flowDevModel == FlowDevModel.SelfFrm) {

        var flow = new Entity("BP.WF.Flow", flowNo);
        var url = flow.FrmUrl;
        url = promptGener('请输入url', url);
        if (url == null || url == undefined)
            return;

        flow.FrmUrl = url;
        flow.Update();

        url += "&UserNo=" + userNo;
        url += "&Token=" + token;
        SetHref(url);
        return;
    }
}
/**
 * 设置审核组件的状态
 * @param {any} nodeID 节点ID
 * @param {any} fwcSta 审核组件的状态值
 */
function SetNodeFWCSta(nodeID, fwcSta) {

    // alert(nodeID.indexOf('01'));
    // alert(nodeID.length );

    if (nodeID.indexOf('01') == nodeID.length - 2) {
        //获得nodeID.
        var node = new Entity("BP.WF.Node", nodeID);

        node.FWCSta = 2; //只读
        node.Update();
        alert('开始节点审核组件状态必须为 只读 ,并且不能修改.');
        return;
    }

    //获得nodeID.
    var node = new Entity("BP.WF.Node", nodeID);
    node.FWCSta = fwcSta;
    node.Update();
    //如果是极简模式，修改FrmNode表单权限中审核组件的状态
    if (flowDevModel == FlowDevModel.JiJian) {
        var mypk = node.NodeFrmID + "_" + node.NodeID + "_" + node.FK_Flow;
        var frmNode = new Entity("BP.WF.Template.FrmNode");
        frmNode.SetPKVal(mypk);
        if (frmNode.RetrieveFromDBSources() == 1) {
            frmNode.IsEnableFWC = fwcSta;
            frmNode.Update();
        }
    }

    if (fwcSta == 0)
        layer.alert("修改成功:已禁用.");
    if (fwcSta == 1)
        layer.alert("修改成功:已启用.");
    if (fwcSta == 2)
        layer.alert("修改成功:已只读.");

}
function ShowFlowDevModelText() {

    if (flowDevModel == FlowDevModel.Prefessional) {
        $("#flowDevModelText").html("专业模式");
    }
    if (flowDevModel == FlowDevModel.JiJian) {
        $("#flowDevModelText").html("极简模式");
    }
    if (flowDevModel == FlowDevModel.FoolTruck) {
        $("#flowDevModelText").html("累加模式");
    }
    if (flowDevModel == FlowDevModel.RefOneFrmTree) {
        $("#flowDevModelText").html("绑定单库表单模式");
    }
    if (flowDevModel == FlowDevModel.FrmTree) {
        $("#flowDevModelText").html("绑定多表单模式");
    }
    if (flowDevModel == FlowDevModel.SDKFrm) {
        $("#flowDevModelText").html("SDK表单模式");
    }
    if (flowDevModel == FlowDevModel.SelfFrm) {
        $("#flowDevModelText").html("嵌入式表单模式");
    }
    if (flowDevModel == FlowDevModel.InternetOfThings) {
        $("#flowDevModelText").html("物联网流程");
    }
}
//设置状态。
function SetState() {

    $("#Btn_Frm").hide();

    if (flowDevModel == FlowDevModel.Prefessional ||
        flowDevModel == FlowDevModel.FoolTruck ||
        flowDevModel == FlowDevModel.InternetOfThings) {
        $("#pmAttribute").after("<li id='pmFrmSln'><i class='iconfont icon-biaodandingzhimoban'></i>&nbsp;<span class='_label'>表单方案</span></li>");
        $("#pmFrmSln").after("<li id='pmFrmD'><i class='iconfont icon-linshibiaoge'></i>&nbsp;<span class='_label'>设计表单</span></li>");
    }
    if (flowDevModel == FlowDevModel.FrmTree ||
        flowDevModel == FlowDevModel.SDKFrm ||
        flowDevModel == FlowDevModel.SelfFrm)
        $("#pmAttribute").after("<li id='pmFrmSln'><i class='iconfont icon-biaodandingzhimoban'></i>&nbsp;<span class='_label'>表单方案</span></li>");

    if (flowDevModel == FlowDevModel.JiJian ||
        flowDevModel == FlowDevModel.RefOneFrmTree) {
        $("#pmAttribute").after("<li id='pmFrmPower'><i class='iconfont icon-biaoge'></i>&nbsp;<span class='_label'>表单权限</span></li>");

    }


    //极简模式下.
    if (flowDevModel == FlowDevModel.JiJian) {
        $("#Btn_Frm").show(); //如果是旧版本的就隐藏该按钮.
        //1.隐藏掉节点右键菜单的，表单方案.

        //$("#pmAttribute").hide();   隐藏无效，因为此方法优先于 元素加载 ，获取不到元素
        //2.增加审核组件状态的编辑..
        $("#pmNodeAccepterRole").after("<li id='pmWorkCheck'> &nbsp;&nbsp;<span class='_label'>审核组件</span></li>");
    }

    //累加模式下.
    if (flowDevModel == FlowDevModel.FoolTrack) {
        $("#Btn_Frm").hide();
    }
    //绑定单个表单 .
    if (flowDevModel == FlowDevModel.RefOneFrmTree) {
        $("#Btn_Frm").show();
        //2.增加审核组件状态的编辑..
        $("#pmNodeAccepterRole").after("<li id='pmWorkCheck'> &nbsp;&nbsp;<span class='_label'>审核组件</span></li>");
    }

    //SDK和嵌入式 模式.
    if (flowDevModel == FlowDevModel.SDKFrm || flowDevModel == FlowDevModel.SelfFrm) {
        $("#Btn_Frm").show();

        //2.增加审核组件状态的编辑..
        $("#pmNodeAccepterRole").after("<li id='pmWorkCheck'> &nbsp;&nbsp;<span class='_label'>审核组件</span></li>");
    }

    //隐藏指定的菜单.
    $("#pmFrmSln").hide(); //表单方案
    $("#pmFrmD").hide(); //设计表单.
}

//设置节点类型。
function SetNodeRunModel(nodeID, runModel, subThreadType) {
    //获得nodeID.
    var node = new Entity("BP.WF.Template.NodeExt", nodeID);
    node.RunModel = runModel;

    ////判断是否是同表单的,还是异表单的.
    //if (runModel == 4) {
    //    node.SubThreadType = subThreadType;
    //}

    if (node.RunModel == 1 || node.RunModel == 3) {//|| node.RunModel == 2
        alert("修改成功，已经帮你起用了子线程按钮...");
        node.Update();
        return;
    }

    node.Update();
    alert("修改成功！");
}

//改变节点风格.
function ChangeNodeIcon(nodeID, runModel) {
    alert('未实现.');
}

function callbackSuperDialog(selectValue) {
    var aResult = selectValue.split('@leipi@');
    $('#' + window._viewField).val(aResult[0]);
    $('#' + window._hidField).val(aResult[1]);
    //document.getElementById(window._hidField).value = aResult[1];
}
/**
* 弹出窗选择用户部门角色
* showModalDialog 方式选择用户
* URL 选择器地址
* viewField 用来显示数据的ID
* hidField 隐藏域数据ID
* isOnly 是否只能选一条数据
* dialogWidth * dialogHeight 弹出的窗口大小
*/
function superDialog(URL, viewField, hidField, isOnly, dialogWidth, dialogHeight) {
    dialogWidth || (dialogWidth = 620)
        , dialogHeight || (dialogHeight = 520)
        , loc_x = 500
        , loc_y = 40
        , window._viewField = viewField
        , window._hidField = hidField;
    // loc_x = document.body.scrollLeft+event.clientX-event.offsetX;
    //loc_y = document.body.scrollTop+event.clientY-event.offsetY;
    if (window.ActiveXObject) { //IE
        var selectValue = window.showModalDialog(URL, self, "edge:raised;scroll:1;status:0;help:0;resizable:1;dialogWidth:" + dialogWidth + "px;dialogHeight:" + dialogHeight + "px;dialogTop:" + loc_y + "px;dialogLeft:" + loc_x + "px");
        if (selectValue) {
            callbackSuperDialog(selectValue);
        }
    } else {  //非IE
        var selectValue = window.open(URL, 'newwindow', 'height=' + dialogHeight + ',width=' + dialogWidth + ',top=' + loc_y + ',left=' + loc_x + ',toolbar=no,menubar=no,scrollbars=no, resizable=no,location=no, status=no');
    }
}

var flowNo = null;
$(function () {

    flowNo = GetQueryString("FK_Flow");
    if (flowNo == undefined || flowNo == null)
        flowNo = "002";

    var alertModal = $('#alertModal'), attributeModal = $("#attributeModal");
    var alertModal1 = $('#alertModal1'), attributeModal = $("#attributeModal");
    var alertModal2 = $('#alertModal2'), attributeModal = $("#attributeModal");
    var alertModal3 = $('#alertModal3'), attributeModal = $("#attributeModal");
    var alertModal4 = $('#alertModal4'), attributeModal = $("#attributeModal");
    //消息提示
    mAlert = function (messages, s) {
        if (!messages) messages = "";
        if (!s) s = 2000;
        alertModal.find(".modal-body").html(messages);
        alertModal.modal('toggle');
        setTimeout(function () { alertModal.modal("hide") }, s);
    }
    //消息弹出（节点）
    cAlert = function (messages, s) {
        if (!messages) messages = "";
        if (!s) s = 200000;
        alertModal1.find(".modal-body").html(messages);
        alertModal1.modal('toggle');
        setTimeout(function () { alertModal1.modal("hide") }, s);
    }
    //消息弹出（标签）
    labAlert = function (messages, s) {
        if (!messages) messages = "";
        if (!s) s = 200000;
        alertModal3.find(".modal-body").html(messages);
        alertModal3.modal('toggle');
        setTimeout(function () { alertModal3.modal("hide") }, s);
    }
    //连接线演示
    ShowGif = function (s) {

        if (!s) s = 200000;

        alertModal2.modal('toggle');
        setTimeout(function () { alertModal2.modal("hide") }, s);
    }
    //新建流程演示
    ShowNewFlowGif = function (s) {

        var msg = "操作提示:";
        msg += "\t\n1.新建流程:请点击工具栏【流程】链接，然后在目录上点右键。";
        msg += "\t\n2.新建节点:在画布的空白区域点击邮件，选择新建节点菜单。 ";
        msg += "\t\n3.在节点上点击右键打开节点属性，用于控制节点行为。 ";
        msg += "\t\n4.流程属性，用于控制整体流程行为。 ";
        alert(msg);
        return;


        // window.open('NewFlow.png');
        // alert('请在流程树的节点上点击右键.');
        return;

        if (!s) s = 200000;

        alertModal4.modal('toggle');
        setTimeout(function () { alertModal4.modal("hide") }, s);
    }
    //消息弹出（线）
    fAlert = function (messages, s, c) {

        if (!messages) messages = "请选择您要执行的操作";
        if (!s) s = 200000;

        //获取连接线连接的ID
        var fromNodeID = c.sourceId.replace('window', '');
        var toNodeID = c.targetId.replace('window', '');
        var dirLabId = "TB_Direction_LAB_" + fromNodeID + "_" + toNodeID;
        //获取流程编号
        var flowNo = GetQueryString("FK_Flow");

        var hander = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCBPMDesigner2018");
        hander.AddPara("FK_Node", fromNodeID);
        hander.AddPara("FK_Flow", flowNo);
        hander.AddPara("ToNode", toNodeID);
        var data = hander.DoMethodReturnString("Direction_Init");
        if (data.indexOf('err@') == 0) {
            alert(data); //删除失败的情况.
            return;
        }

        var labText = '请输入线上文字描述:<br/><input style="width:90%" id="' + dirLabId + '" type="text" value="' + data + '">';

        //$("#LineModal").find(".modal-body").html(messages);
        $("#LineModal").find(".modal-body").html(labText);
        $("#LineModal").modal('toggle');
        setTimeout(function () { alertModal1.modal("hide") }, s);
    }
    //属性设置
    attributeModal.on("hidden", function () {
        $(this).removeData("modal"); //移除数据，防止缓存
    });

    ajaxModal = function (url, fn) {

        url += url.indexOf('?') ? '&' : '?';
        url += '_t=' + new Date().getTime();
        attributeModal.find(".modal-body").html('<img src="Public/images/loading.gif" />');
        attributeModal.modal({
            remote: url
        });

        //加载完成执行
        if (fn) {
            attributeModal.on('shown', fn);
        }
    }

    /*步骤数据*/
    var processData = GenerDrowFlowData();
    //标签数据
    var labNoteData = GetLabNoteData();

    /*创建流程设计器*/
    var _canvas = $("#flowdesign_canvas").Flowdesign({
        "processData": processData,
        "labNoteData": labNoteData
        , mtAfterDrop: function (params) {
            //alert("连接：" + params.sourceId + " -> " + params.targetId);
        }
        /*画面右键*/
        , canvasMenus: {
            "cmNewNode": function (t) {
                $(".mymaskContainer").offset({ left: ($(document).innerWidth() - 120) / 2, top: ($(document).innerHeight() - 50) / 2 });
                $(".mymask").show();

                //获取坐标
                var mLeft = $("#jqContextMenu").css("left").replace('px', '');
                var mTop = $("#jqContextMenu").css("top").replace('px', '');


                //创建一个节点
                var hander = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCBPMDesigner2018");
                hander.AddPara("X", mLeft);
                hander.AddPara("Y", mTop);
                hander.AddPara("FK_Flow", flowNo);

                var data = hander.DoMethodReturnString("CreateNode");
                if (data.indexOf('err@') == 0) {
                    alert(data);
                    return;
                }

                //添加节点样式与坐标
                data = JSON.parse(data);
                var strs = "";
                strs += "{'id':'" + data.NodeID + "',";
                strs += "'flow_id':'" + flowNo + "',";
                strs += "'process_name':'" + data.Name + "',";
                strs += "'process_to':0,";
                strs += "'icon':'icon-ok',";
                strs += "'style':'width:auto;color:#0e76a8;left:" + mLeft + "px;top:" + mTop + "px;'";
                strs += "}";
                strs = cceval("(" + strs + ")");

                if (_canvas.addProcess(strs) == false) //添加
                {
                    alert("添加失败");
                    return;
                }
                $(".mymask").hide();
            },
            "cmNewCond": function (t) {
                $(".mymaskContainer").offset({ left: ($(document).innerWidth() - 120) / 2, top: ($(document).innerHeight() - 50) / 2 });
                $(".mymask").show();

                //获取坐标
                var mLeft = $("#jqContextMenu").css("left").replace('px', '');
                var mTop = $("#jqContextMenu").css("top").replace('px', '');


                //创建一个节点
                var hander = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCBPMDesigner2018");
                hander.AddPara("X", mLeft);
                hander.AddPara("Y", mTop);
                hander.AddPara("FK_Flow", flowNo);

                var data = hander.DoMethodReturnString("CreateCond");
                if (data.indexOf('err@') == 0) {
                    alert(data);
                    return;
                }

                //添加节点样式与坐标
                data = JSON.parse(data);
                var strs = "";
                strs += "{'id':'" + data.NodeID + "',";
                strs += "'flow_id':'" + flowNo + "',";
                strs += "'process_name':'" + data.Name + "',";
                strs += "'process_to':0,";
                strs += "'icon':'icon-ok',";
                strs += "'style':'width:auto;color:#0e76a8;left:" + mLeft + "px;top:" + mTop + "px;'";
                strs += "}";
                strs = cceval("(" + strs + ")");

                if (_canvas.addProcess(strs) == false) //添加
                {
                    alert("添加失败");
                    return;
                }
                $(".mymask").hide();
            },
            "cmSave": function (t) {

                var processInfo = _canvas.getProcessInfo(); //连接信息

                /*重要提示 start*/
                alert("这里使用ajax提交，请参考官网示例，可使用Fiddler软件抓包获取返回格式cc");

            },
            //刷新
            //添加标签
            "cmNewLabel": function (t) {
                var mLeft = $("#jqContextMenu").css("left").replace('px', '');
                var mTop = $("#jqContextMenu").css("top").replace('px', '');

                //创建一个标签
                var hander = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCBPMDesigner2018");
                hander.AddPara("X", mLeft);
                hander.AddPara("Y", mTop);
                hander.AddPara("FK_Flow", flowNo);
                hander.AddPara("LabName", "请输入标签");

                var data = hander.DoMethodReturnString("CreatLabNote");
                if (data.indexOf('err@') == 0) {
                    alert(data);
                    return;
                }

                //添加标签样式与坐标
                data = JSON.parse(data);
                var strs = "";
                strs += "{'id':'" + data.MyPK + "',";
                strs += "'flow_id':'" + data.FK_Flow + "',";
                strs += "'process_name':'请输入标签',";
                strs += "'style':'width:auto;height:30px;line-height:30px;color:#0e76a8;left:" + mLeft + "px;top:" + mTop + "px;'";
                strs += "}";
                strs = cceval("(" + strs + ")");

                if (_canvas.addLabProcess(strs) == false) //添加
                {
                    alert("添加失败");
                    return;
                }
            },
            "cmPaste": function (t) {
                var pasteId = _canvas.paste(); //右键当前的ID
                if (pasteId <= 0) {
                    alert("你未复制任何步骤");
                    return;
                }
                alert("粘贴:" + pasteId);
            },
            "cmHelp": function (t) {

                Help();
            }
        }
        /*步骤右键*/
        , processMenus: {

            "pmBegin": function (t) {
                var activeId = _canvas.getActiveId(); //右键当前的ID
                alert("设为第一步:" + activeId);
            },
            "pmAddson": function (t)//添加子步骤
            {
                var activeId = _canvas.getActiveId(); //右键当前的ID
            },
            "pmCopy": function (t) {
                //var activeId = _canvas.getActiveId();//右键当前的ID
                _canvas.copy(); //右键当前的ID
                alert("复制成功");
            },
            "pmDelete": function (t) {

                var activeId = _canvas.getActiveId(); //右键当前的ID.
                var str = activeId.substring(activeId.length - 2);
                if (str == "01") {
                    /*如果是开始节点. */
                    alert('开始节点不允许删除.');
                    return;
                }

                if (confirm("你确定删除节点吗？如果当前节点有待办或者有数据，系统将拒绝删除。") == false)
                    return;

                //节点.
                var hander = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCBPMDesigner2018");
                hander.AddPara("FK_Node", activeId);
                var data = hander.DoMethodReturnString("DeleteNode");
                if (data.indexOf('err@') == 0) {
                    alert(data); //删除失败的情况.
                    return;
                }
                _canvas.delProcess(activeId);
            },
            "pmName": function (t) {
                //节点属性.
                var activeId = _canvas.getActiveId(); //右键当前的ID
                NodeSetName(activeId);
            },
            "pmAttribute": function (t) {
                //节点属性.
                var activeId = _canvas.getActiveId(); //右键当前的ID
                NodeAttr(activeId);
            },
            "pmCondDir": function (t) {
                var activeId = _canvas.getActiveId(); //右键当前的ID
                CondDir(activeId);
            },
            "pmFrmSln": function (t) {
                //表单方案.
                var activeId = _canvas.getActiveId(); //右键当前的ID
                NodeFrmSln(activeId); //表单方案.
            },
            "pmFrmD": function (t) {
                var activeId = _canvas.getActiveId(); //右键当前的ID
                NodeFrmD(activeId);
            },
            "pmFrmPower": function (t) {
                var activeId = _canvas.getActiveId(); //表单权限ID
                FrmPower(activeId);
            },
            "pmFrmFool": function (t) {
                var activeId = _canvas.getActiveId(); //右键当前的ID
                NodeFrmFool(activeId);
            },
            "pmFrmFree": function (t) {
                var activeId = _canvas.getActiveId(); //右键当前的ID
                NodeFrmFree(activeId); 
            },
            "pmNodeAccepterRole": function (t) {

                var activeId = _canvas.getActiveId(); //右键当前的ID

                NodeAccepterRole(activeId);
            }
        },
        canvasLabMenu: {
            "clmDelete": function (t) {
                var activeId = _canvas.getActiveId(); //右键当前的ID.
                if (confirm("你确定删除该标签吗？") == false)
                    return;

                //删除标签.
                var lb = new Entity("BP.WF.Template.LabNote", activeId);
                lb.Delete();
                _canvas.delLabNote(activeId);

            },
            "clmNewName": function (t) {
                //修改标签名称.
                var activeId = _canvas.getActiveId(); //右键当前的ID
                console.log('activeId', activeId);
                var windowtext = $("#lab" + activeId).text();

                windowtext = windowtext.replace(/(^\s*)|(\s*$)/g, ""); //去掉左右空格.

                $("#alertModal3 div:eq(2) button").attr("class", "btn btn-primary savetext" + activeId);
                $("#alertModal3 div:eq(2) button").attr("onclick", "saveLabName(\"" + activeId + "\")");
                var xiuNodename = '<input style="width:90%" id="TB_LAB_' + activeId + '" type="text" value="' + windowtext + '">'
                $("#lab" + activeId + " span").html();

                labAlert(xiuNodename);
            }
        }
        , fnRepeat: function () {
            //alert("步骤连接重复1");//可使用 jquery ui 或其它方式提示
            mAlert("步骤连接重复了，请重新连接，或者关闭当前流程，重新打开。");

        }
        , fnClick: function () {

              // //和 pmAttribute 一样
            // var activeId = _canvas.getActiveId(); //右键当前的ID\
            // NodeAttr(activeId);
        }
        , fnDbClick: function () {
            //点击修改名称方法
            var activeId = _canvas.getActiveId(); //右键当前的ID
            //说明是子线程
            if (activeId.indexOf("S_") != -1) {
                var mypk = activeId.replace("S_","");
                //合并模式
                if (subFlowShowType == 1) {
                    var subs = subFlows.filter(item => item.FK_Node == mypk);
                    if (subs.length == 1) {
                        var url = basePath+"/WF/Admin/CCBPMDesigner/Designer.htm?FK_Flow=" + subs[0].SubFlowNo + "&UserNo=" + webUser.No + "&Token=" + webUser.Token + "&OrgNo=" + webUser.OrgNo;
                        OpenLayuiDialog(url, subs[0].SubFlowName, window.innerWidth * 0.8, 80, "auto");
                        return;
                    }
                    if (subs) {
                        var _html = "<ul>";
                        subs.forEach(function (item) {
                            var url = basePath +"/WF/Admin/CCBPMDesigner/Designer.htm?FK_Flow=" + item.SubFlowNo + "&UserNo=" + webUser.No + "&Token=" + webUser.Token + "&OrgNo=" + webUser.OrgNo;
                            _html += '<li style="line-heigth:34px;font-size:16px;font-weight:600" onclick="OpenLayuiDialog(\'' + url + '\',\'' + item.SubFlowName + '\',' + window.innerWidth * 0.8+',80,\'auto\')">' + item.SubFlowName + '</li>';
                        })
                        _html += "</ul>";
                        layer.open({
                            title: "子流程",
                            content: _html //这里content是一个DOM，注意：最好该元素要存放在body最外层，否则可能被其它的相对元素所影响
                        });
                    }
                    return;
                }
                if (subFlowShowType == 0) {
                    var sub = subFlows.filter(item => item.MyPK == mypk)[0];
                    var url = basePath +"/WF/Admin/CCBPMDesigner/Designer.htm?FK_Flow=" + sub.SubFlowNo + "&UserNo=" + webUser.No + "&Token=" + webUser.Token + "&OrgNo=" + webUser.OrgNo;
                    OpenLayuiDialog(url, sub.SubFlowName, window.innerWidth * 0.8,80,"auto");
                    return;
                }
                return;
            }
            var windowtext = $("#window" + activeId).text();
            var baocunbut = $("#alertModal1 div:eq(2) button:eq(0)").attr("class", "btn btn-primary savetext" + activeId);
            $("#alertModal1 div:eq(2) button:eq(0)").attr("onclick", "SaveNodeName(\"" + activeId + "\")");
            var baocunbut = $("#alertModal1 div:eq(2) button:eq(1)").attr("class", "btn btn-primary savetext" + activeId);
            $("#alertModal1 div:eq(2) button:eq(1)").attr("onclick", "SaveAndUpdateNodeName(\"" + activeId + "\")");
            windowtext = windowtext.replace(/(^\s*)|(\s*$)/g, "");

            var xiuNodename = '<input style="width:90%" id="TB_' + activeId + '" type="text" value="' + windowtext + '">'
            var spanaa = $("#window" + activeId + " span").html();

            cAlert(xiuNodename);
        }
    });
    /*新建*/
    $("#Btn_NewFlow").bind('click', function () {

        alert("请在流程树右键菜单新建流程！");


    });

    /*保存*/
    $("#Btn_Save").bind('click', function () {

        $("#Btn_Save").attr("disabled", true);
        $("#Btn_Save").html("<i class='iconfont icon-baocun'></i> 正在保存...");

        SaveFlow(_canvas);

        $("#Btn_Save").html("<i class='iconfont icon-baocun'></i> 保存成功");

        setTimeout(function () {
            $("#Btn_Save").html("<i class='iconfont icon-baocun'></i> 保存");
        }, 1000);

    });
    /*保存*/
    $("#Btn_SaveToColud").bind('click', function () {

        alert('开发中');
        return;
        $("#Btn_Save").attr("disabled", true);

    });
    /*清除连接线，用不到.*/
    $("#leipi_clear").bind('click', function () {
        return;
        if (_canvas.clear()) {
            //alert("清空连接成功");
            mAlert("清空连接成功，你可以重新连接");
        } else {
            //alert("清空连接失败");
            mAlert("清空连接失败");
        }
    });

});


///保存方法
function SaveFlow(_canvas) {

    //获取所有节点信息.
    try {

        var nodes = new Entities("BP.WF.Template.NodeSimples");
        nodes.Retrieve("FK_Flow", flowNo);

        // 保存x,y位置.
        var processInfo = _canvas.getProcessInfo(); //连接信息.
        processInfo = JSON.parse(processInfo);

        //定义要生成的字符串.
        var nodePos = "";
        var subFlows = "";
        //定义名称. 格式为: @101=填写请假申请单@101=
        var nodeName = "";
        for (var nodeID in processInfo) {

            var nodeIDStr = JSON.stringify(nodeID);

            var nodeJSON = processInfo[nodeID];
            var isHave = false;
            for (var idx = 0; idx < nodes.length; idx++) {

                var node = nodes[idx];
                var myID = "\"" + node.NodeID + "\"";
                if (myID != nodeIDStr)
                    continue;

                //alert(node);

                var nodeName = $("#span_" + node.NodeID).text();

                nodePos += "@" + node.NodeID + "," + nodeJSON.left + "," + nodeJSON.top + "," + nodeName;
                isHave = true;
                continue;
            }
            if (isHave == false)
                subFlows += "@" + nodeIDStr.replace("S_", "") + "," + nodeJSON.left + "," + nodeJSON.top;
        }

        //方向的字符串.
        var myDirs = "";

        //保存方向.
        for (var nodeID in processInfo) {

            //获得toNode.
            var nodeJSON = processInfo[nodeID];
            var strs = JSON.stringify(nodeJSON);

            //获得toNode.
            var toNodes = nodeJSON.process_to;
            if (toNodes == "")
                continue;
           

            if (nodeID == undefined
                || nodeID == null
                || nodeID == "undefined"
                || nodeID == 'undefined') {
                continue;
            }

            for (var i = 0; i < toNodes.length; i++) {

                var toNodeID = toNodes[i];
                if (toNodeID.indexOf("S_") == 0)
                    continue;
                if (toNodeID == undefined
                    || toNodeID == null
                    || toNodeID == "undefined"
                    || toNodeID == 'undefined') {

                    var fromNode = new Entity("BP.WF.Template.NodeSimple", nodeID);
                    alert('保存出错: \t\n\t\n1.节点[' + fromNode.Name + ']到达节点没有把线连接正确.\t\n2.请您关闭当前流程，重新打开然后连接，执行保存。 \t\n3.其余的方向条件保存成功.');
                    $("#Btn_Save").attr("disabled", false);
                    $("#Btn_Save").html("保存");
                    continue;
                }
                var MyPK = flowNo + "_" + nodeID + "_" + toNodeID;
                myDirs += "@" + MyPK + "," + flowNo + "," + nodeID + "," + toNodeID;
            }
        }


        var labs = "";
        //标签.
        var labNoteInfo = _canvas.getLabNoteInfo(); //标签信息.
        labNoteInfo = JSON.parse(labNoteInfo);
        for (var lab in labNoteInfo) {

            var labJSON = labNoteInfo[lab];

            labs += "@" + lab + "," + labJSON.left + "," + labJSON.top;
        }

        var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCBPMDesigner");
        handler.AddPara("Nodes", nodePos);
        handler.AddPara("Dirs", myDirs);
        handler.AddPara("Labs", labs);
        handler.AddPara("FK_Flow", flowNo);
        handler.AddPara("SubFlows", subFlows);
        var data = handler.DoMethodReturnString("Designer_Save");

        if (data.indexOf('err@') == 0) {
            alert(data);
        }
    }
    catch (e) {
        alert(e);
    }

    //alert('保存成功!');

    $("#Btn_Save").attr("disabled", false);
    $("#Btn_Save").html("<image src='../../Img/Btn/Save.png' width='14px' height='14px'>&nbsp;保存");
    return;
}

//修改节点名称
function SaveNodeName(activeId) {
    ReLoginByToken();

    var text = document.getElementById("TB_" + activeId).value; //新修改的值.
    $("#span_" + activeId).text(text);

    //执行数据库保存.
    var node = new Entity("BP.WF.Template.NodeExt", activeId);
    node.DoMethodReturnString("Do_SaveNodeName", text);
    $("#span_" + activeId).text(text); //更新节点名称与显示
    return;
}

//修改并更新节点表单名称
function SaveAndUpdateNodeName(activeId) {
    ReLoginByToken();

    var text = document.getElementById("TB_" + activeId).value; //新修改的值.
    //$("#span_" + activeId).text(text);
    //return;
    //alert(text);

    var node = new Entity("BP.WF.Template.NodeExt", activeId);
    node.DoMethodReturnString("Do_SaveAndUpdateNodeName", text);

    //更新节点名称与显示
    $("#span_" + activeId).text(text);
}
//修改标签名称
function saveLabName(activeId) {
    ReLoginByToken();

    var lb = new Entity("BP.WF.Template.LabNote", activeId);
    var text = document.getElementById("TB_LAB_" + activeId).value; //新修改的值.
    lb.Name = text;
    lb.Update();

    //更新名称与显示
    $("#lab_span_" + activeId).text(text);
}
//获得流程数据并转化为指定格式的json.
function GenerDrowFlowData() {

    flowNo = GetQueryString("FK_Flow");
    if (flowNo == null || flowNo == undefined)
        flowNo = "002";


    //节点. 取出来显示
    var nodes = new Entities("BP.WF.Nodes");
    nodes.Retrieve("FK_Flow", flowNo);

    //方向. 取出来显示
    var dirs = new Entities("BP.WF.Template.Directions");
    dirs.Retrieve("FK_Flow", flowNo);

    var strs = "{'total':" + nodes.length + ", 'list':[";
    var lineDesList = new Array();
    //遍历节点个数，输入节点.
    for (var i = 0; i < nodes.length; i++) {

        var node = nodes[i];

        //获得到达的节点.
        var toNodes = "";

        for (var idx = 0; idx < dirs.length; idx++) {
            var dir = dirs[idx];

            if (dir.Node == node.NodeID) {
                toNodes += "," + dir.ToNode;
                var lineDes = {};
                lineDes.id = node.NodeID + "_" + dir.ToNode;
                lineDes.des = dir.Des;
                lineDesList.push(lineDes);
            }
        }

        //子流程展示
        if (subFlows.length > 0) {
            //平铺展示
            if (subFlowShowType == 0) {
                var idx = 0;
                subFlows.forEach(item => {
                    if (item.FK_Node == node.NodeID) {
                        toNodes += ",S_" + item.MyPK;
                        var lineDes = {};
                        lineDes.id = node.NodeID + "_S_" + item.MyPK;
                        lineDes.des = "子流程:" + item.SubFlowName;
                        lineDesList.push(lineDes);
                        strs += "{'id':'S_" + item.MyPK + "',";
                        strs += "'flow_id':'" + flowNo + "',";
                        strs += "'process_name':'" + item.SubFlowName.replace(/(^\s*)|(\s*$)/g, "") + "',";
                        strs += "'process_to':'',";
                        strs += "'icon':'icon-leaf',";
                        var subFlowX = item.X == 0 || item.X == undefined ? node.X + 400 + idx * 145 : item.X;
                        var subFlowY = item.Y == 0 || item.Y == undefined ? node.Y + idx * 50 : item.Y;
                        strs += "'style':'width:auto;minWidth:121px;color:#0e76a8;left:" + subFlowX + "px" + ";top:" + subFlowY + "px;'";
                        strs += "},";
                        idx++;
                    }
                })
            }
            //合并展示
            if (subFlowShowType == 1) {
                var isHave = subFlows.filter(function (item) { return item.FK_Node == node.NodeID }).length > 0 ? true : false;
                if (isHave == true) {
                    toNodes += ",S_" + node.NodeID;
                    var lineDes = {};
                    lineDes.id = node.NodeID + "_S_" + node.NodeID;
                    lineDes.des = "子流程";
                    lineDesList.push(lineDes);
                    strs += "{'id':'S_" + node.NodeID + "',";
                    strs += "'flow_id':'" + flowNo + "',";
                    strs += "'process_name':'启动的子流程',";
                    strs += "'process_to':'',";
                    strs += "'icon':'icon-leaf',";
                    var subFlowX = node.SubFlowX == 0 ? node.X + 400 : node.SubFlowX;
                    var subFlowY = node.SubFlowY == 0 ? node.Y : node.SubFlowY;
                    strs += "'style':'width:auto;minWidth:121px;color:#0e76a8;left:" + subFlowX + "px" + ";top:" + subFlowY + "px;'";
                    strs += "},";
                }

            }
        }

        if (node.Name == null || node.Name == "")
            node.Name = "节点x";

        strs += "{'id':'" + node.NodeID + "',";
        strs += "'flow_id':'" + flowNo + "',";
        strs += "'process_name':'" + node.Name.replace(/(^\s*)|(\s*$)/g, "") + "',";
        strs += "'process_to':'" + toNodes + "',";

        //判断是否是开始节点?
        var nodeID = "" + node.NodeID;
        var str = nodeID.substring(nodeID.length - 2);

        if (str == "01") {
            // strs += "'icon':'icon-ok',";
        } else if (toNodes == "") {
            strs += "'icon':'icon-ok',";
        } else {
            /* 如果是其他的情况,就要考虑分合流 */
        }

        strs += "'style':'width:auto;minWidth:121px;color:#0e76a8;left:" + node.X + "px" + ";top:" + node.Y + "px;'";

        if (i == nodes.length - 1)
            strs += "}";
        else
            strs += "},";
    }

    strs += "] }";

    var canvasObj = cceval("(" + strs + ")");
    canvasObj.process_des = lineDesList;
    return canvasObj;
}

function DealSpecStr(str) {

    str = str.toString().replace(new RegExp('(["\"])', 'g'), "\\\"");
    str = str.replace("\"", "\\\"").replace("\r\n", "<br />").replace("\n", "<br />").replace("\r", "<br />");
    str = str.replace("\"", "\'");

    if (str.indexOf('开发要点说明') != -1) {
        return "";
    }
    return str;
}

function GetLabNoteData() {
    //标签. 取出来显示
    var labs = new Entities("BP.WF.Template.LabNotes");
    labs.Retrieve("FK_Flow", flowNo);

    var strs = "{'total':" + labs.length + ", 'list':[";
    //遍历标签个数
    for (var i = 0; i < labs.length; i++) {

        var lab = labs[i];

        //console.log(lab.Name);

        lab.Name = DealSpecStr(lab.Name);

        //alert(lab.Name);

        strs += "{'id':'" + lab.MyPK + "',";
        strs += "'flow_id':'" + lab.FK_Flow + "',";
        strs += "\"process_name\":\"" + lab.Name + "\",";
        strs += "'style':'width:auto;height:30px;line-height:30px;color:#0e76a8;left:" + lab.X + "px" + ";top:" + lab.Y + "px;'";

        if (i == labs.length - 1)
            strs += "}";
        else
            strs += "},";
    }

    strs += "] }";

    try {
        return cceval("(" + strs + ")");
    } catch (e) {
        return "";
    }
}

//刷新页面
function page_reload() {
    location.reload();
}

function ShowGif() {
    alert("用鼠标按住节点黑色区域,然后拖拉会出现连接线，然后指向要连接到的节点，请看演示");

    $("#Msg").css('display', 'block');
    setTimeout("HideGif()", 7000);
}
function HideGif() {
    $("#Msg").css('display', 'none');
}
function ShowNewFlowGif() {
    alert("请在流程树右键菜单新建流程！请看演示");

    $("#Msg").css('display', 'block');
    setTimeout("HideGif()", 7000);
}
function HideNewFlowGif() {
    $("#Msg").css('display', 'none');
}

//全局变量
function WinOpen(url) {
    window.open(url);
}

//流程属性.
function FlowProperty() {
    var url = "";
    var title = "流程属性" + flowNo;
    var url = "../../Comm/En.htm?EnName=BP.WF.Template.FlowExt&PKVal=" + flowNo + "&Lang=CH";
    OpenLayuiDialog(url, "流程属性", window.innerWidth * 0.9);
}

function OpenTabUrl(url, title, winName) {

    var html = "";
    if (top == self) {
        url = "../" + url;
        window.WinOpenFull(url, title);
        return;

    }
    if (pageFrom == "") {
        window.parent.addTab(winName, title, url);
        return;
    }
    window.top.vm.openTab(title, url);
}

//报表设计.
function FlowRpt() {

    if (window.confirm('该功能，我们将要取消,仅供内部开发人员使用.') == false)
        return;

    var flowId = Number(flowNo);
    flowId = String(flowId);
    // url = "../RptDfine/Default.htm?FK_Flow=" + flowNo + "&FK_MapData=ND" + flowId + "MyRpt";
    var url = basepath + "RptDfine/Default.htm?FK_Flow=" + flowNo + "&FK_MapData=ND" + flowId + "MyRpt";
    OpenLayuiDialog(url, "报表设计", window.innerWidth * 0.7);
}

//检查流程.
function FlowCheck() {

    var flowId = Number(flowNo);
    flowId = String(flowId);
    url = "../AttrFlow/CheckFlow.htm?FK_Flow=" + flowNo + "&FK_MapData=ND" + flowId + "MyRpt";
    OpenLayuiDialog(url, "检查流程", window.innerWidth * 0.40);
    return;
}

function BatchSetting() {
    var flowNo = GetQueryString("FK_Flow");
    var url = "../BatchSetting/Default.htm?FrmID=" + flowNo + "&FK_Flow=" + flowNo + "&FK_MapData=" + flowNo + "&FlowNo=" + flowNo;
    OpenLayuiDialog(url, "批量设置" + flowNo, window.innerWidth * 0.80);
}

function Guide() {
    var flowNo = GetQueryString("FK_Flow");
    var url = "../AttrFlow/DevGuide.htm?FrmID=" + flowNo + "&FK_Flow=" + flowNo + "&FK_MapData=" + flowNo + "&FlowNo=" + flowNo;
    OpenLayuiDialog(url, "设计向导" + flowNo, window.innerWidth * 0.7);
}

//获取指定名称的cookie
function getCookie(name) {
    let strcookie = document.cookie;//获取cookie字符串
    let arrcookie = strcookie.split("; ");//分割
    //遍历匹配
    for (var i = 0; i < arrcookie.length; i++) {
        if (arrcookie[i].indexOf(name) == 0)
            return arrcookie[i]
    }
    return "";
}

function Help() {

    var msg = "<ul>";
    msg += "<li>开发者:济南驰骋信息技术有限公司.</li>";
    msg += "<li>官方网站: <a href='http://www.ccflow.org?Ref=ccbpmApp' target=_blank>http://ccflow.org</a></li>";
    msg += "<li>商务联系:0531-82374939, 微信:18660153393 QQ:793719823</li>";
    msg += "<li>地址:济南.高新区.碧桂园凤凰中心F19.</li>";
    msg += "</ul>";
    mAlert(msg, 20000);
    return;

    var url = "http://ccflow.org/ke.htm";
    OpenLayuiDialog(url, "帮助" + flowNo, window.innerWidth * 0.7);
}

/***********************  节点信息. ******************************************/

//节点属性
function NodeAttr(nodeID) {
    var url = basePath + "/WF/Comm/En.htm?EnName=BP.WF.Template.NodeExt&NodeID=" + nodeID + "&Lang=CH";
    OpenLayuiDialog(url, "节点属性" + nodeID, window.innerWidth * 0.88);
}

//节点属性
function NodeSetName(activeId) {
    //点击修改名称方法
    var windowtext = $("#window" + activeId).text();
    var baocunbut = $("#alertModal1 div:eq(2) button:eq(0)").attr("class", "btn btn-primary savetext" + activeId);
    $("#alertModal1 div:eq(2) button:eq(0)").attr("onclick", "SaveNodeName(\"" + activeId + "\")");
    var baocunbut = $("#alertModal1 div:eq(2) button:eq(1)").attr("class", "btn btn-primary savetext" + activeId);
    $("#alertModal1 div:eq(2) button:eq(1)").attr("onclick", "SaveAndUpdateNodeName(\"" + activeId + "\")");
    windowtext = windowtext.replace(/(^\s*)|(\s*$)/g, "");

    var xiuNodename = '<input style="width:90%" id="TB_' + activeId + '" type="text" value="' + windowtext + '">'
    var spanaa = $("#window" + activeId + " span").html();

    cAlert(xiuNodename);
}

//表单方案
function NodeFrmSln(nodeID) {
    //表单方案.
    var url = basePath + "/WF/Admin/AttrNode/FrmSln/Default.htm?FK_Node=" + nodeID;
    OpenLayuiDialog(url, "表单方案" + nodeID, window.innerWidth * 0.8);
    return;

}

//方向条件.
function CondDir(fromNodeID) {

    var flowNo = GetQueryString("FK_Flow");

    var targetId = fromNodeID;

    var url = "../Cond2020/ConditionLine.htm?FK_Flow=" + flowNo + "&FK_MainNode=" + fromNodeID + "&FK_Node=" + fromNodeID + "&ToNodeID=" + targetId + "&CondType=2&Lang=CH&t=" + new Date().getTime();
    $("#LineModal").hide();
    $(".modal-backdrop").hide();


    var title = "方向条件" + fromNodeID + "->" + targetId;
    OpenLayuiDialog(url, title, window.innerWidth * 0.8);
}

//表单权限.
function FrmPower(nodeID) {
    var frmID = "ND" + parseInt(flowNo + "01");
    if (flowDevModel == FlowDevModel.RefOneFrmTree)
        frmID = flow.FrmUrl;
    var en = new Entity("BP.WF.Template.FrmNodeExt");

    var mypk = frmID + "_" + nodeID + "_" + flowNo;
    en.SetPKVal(mypk);
    if (en.IsExits() == false) {
        en.FK_Frm = frmID;
        en.FK_Flow = flowNo;
        en.FK_Node = nodeID;
        en.FrmSln = 0;
        en.IsEnableFWC = 0;
        en.MyPK = mypk;
        en.Insert();
    }

    //傻瓜表单.
    //  var url = baseurl + "Comm/En.htm?EnName=BP.WF.Template.FrmNodeJiJian&MyPK=" + mypk + "&Lang=CH";
    var url = basePath + "/WF/Comm/En.htm?EnName=BP.WF.Template.FrmNodeExt&MyPK=" + mypk + "&Lang=CH";

    if (flowDevModel == FlowDevModel.RefOneFrmTree)
        url = basePath + "/WF/Comm/En.htm?EnName=BP.WF.Template.FrmNodeExt&MyPK=" + mypk + "&Lang=CH";

    OpenLayuiDialog(url, "表单权限" + nodeID, window.innerWidth * 0.80);

}


//接受人规则.
function NodeAccepterRole(nodeID) {

    var url = basePath + "/WF/Admin/AttrNode/AccepterRole/Default.htm?FK_MapData=ND" + nodeID + "&FK_Flow=" + flowNo + "&FK_Node=" + nodeID;
    OpenLayuiDialog(url, "接受人规则" + nodeID, window.innerWidth * 0.8);
    return;
}

function MyReload() {
    if (confirm('您确定要刷新吗？刷新将不能保存.') == false)
        return;
    Reload();
}

//打开.
function OpenEasyUiDialogExt(url, title, w, h, isReload) {

    OpenEasyUiDialog(url, "eudlgframe", title, w, h, "icon-property", true, null, null, null, function () {
        if (isReload == true) {
            MyReload();
        }
    });
}