//状态 1=备注状态 ， 0=无状态.
var dbVerSta = 0;
var frmID = "";
var compareFrmID = "";
var isTree = false;
var curDocument = window.document;
/**
 * 初始化有批注的字段.
 * 根据是否有批注，就在控件上加批注标识.
 * */
function FrmDBVer_Init() {
    $(".remark", curDocument).remove();
    $(".tips", curDocument).remove();
    if (wf_node.FormType == 5)
        isTree = true;
    if (typeof repremarkSta != "undefined")
        repremarkSta = 0;
    if (wf_node.FormType == 5 && frmID != vm.frmGenerNo) {
        $(".frmDBVer", curDocument).remove();
        dbVerSta = 0;
    }
       
    //如果是
    if (dbVerSta == 1) {
        FrmDBVer_UnDo();
        return; 
    }

    var isMyView = false;
    if ($("#JS_CC").length != 0 || $("#JS_MyFrm").length != 0 || $("#JS_MyView").length != 0)
        isMyView = true;
    //根据节点属性获取当前节点性质
    if (wf_node.FormType == 5) {
        //获取当前打开的页面FrmID
        frmID = vm.frmGenerNo;
        compareFrmID = frmID;
    }

    //傻瓜表单、开发者表单、绑定表单库的表单
    if (wf_node.FormType == 0 || wf_node.FormType == 12 || wf_node.FormType == 11) {
        //直接比对当前的数据和历史数据
        frmID = wf_node.NodeFrmID;
        compareFrmID = frmID;
        if (frmID == null || frmID == undefined || frmID == "") {
            frmID = "ND" + wf_node.NodeID;
            compareFrmID = "ND" + parseInt(wf_node.FK_Flow) + "Rpt";
        }
    }

    var mapData = new Entity("BP.Sys.MapData", frmID);
    //如果是章节表单
    if (mapData.FrmType == 10) {
        layer.alert(mapData.Name + "是章节表单,查看数据版本请点击每个章节的【审阅&版本】功能");
        return;
    }
    //获取版本信息
    var frmDBVers = new Entities("BP.Sys.FrmDBVers");
    frmDBVers.Retrieve("RefPKVal", paramData.WorkID, "FrmID", compareFrmID, "RDT");
    frmDBVers = frmDBVers.TurnToArry();
    //如果当前是查看页面把当前版本去掉
    if (isMyView == true) {
        if (frmDBVers.length == 0 || frmDBVers.length == 1) {
            layer.alert("不存在数据版本变更,不需要比对");
            return;
        }
        frmDBVers.splice(frmDBVers.length - 1, 1);
    }

    var _html = "";
    $.each(frmDBVers, function (i, item) {
        _html += "<a href='javascript:void(0)' style='line-height:30px;padding-left:20px'onclick='FrmDBVerCompare(\"" + item.MyPK + "\")'>" + item.RDT + "</a><br/>";
    })
    //弹出选择版本号进行比对
    layer.open({
        title: "选择版本",
        type: 1,
        area: '300px;',
        shade: 0,
        content: _html,
    });
    //设置为批注状态.
    dbVerSta = 1;
}

function FrmDBVerCompare(mypk, frmID) {
    layer.close(layer.index);
    //根据MYPK获取版本的数据
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    handler.AddPara("MyPK", mypk);
    var data = handler.DoMethodReturnString("FrmDB_DoCompare");
    if (data.indexOf("err@")!=-1) {
        layer.alert(data);
        return;
    }
    data = JSON.parse(data);
    var mainData = data["mainData"][0];
    var mapData = data["Sys_MapData"][0];
    //主表信息的比对
    MainData_Compare(mainData, mapData);

    //从表的比对
    var dtls = data["Sys_MapDtl"];
    $.each(dtls, function (i, dtl) {
       
        var element = $("#Dtl_" + dtl.No, curDocument);
        if (element.length == 0)
            return true;
        var style = foolStyle.replace("bottom: -1px","top:-1px");
        if (mapData.FrmType == 8)
            style = developStyle.replace("bottom: -12px", "top:-1px");
        var w = $("#Dtl_" + dtl.No, curDocument).width() - 10;
        var dtlE = $("<div class='frmDBVer' data-info='"+dtl.No+"'data-name='"+dtl.Name+"' style='" + style + "margin-left:" + w + "px;'></div>")
        element.after(dtlE);
        dtlE.on("click", function () {
            var url = "./CCForm/FrmDBVerAndRemark.htm?Field=" + $(this).attr("data-info") + "&FieldType=1&FrmID=" + frmID + "&RFrmID=" + compareFrmID + "&RefPKVal=" + paramData.WorkID + "&IsEnable=0&PageType=0";
            layer.open({
                type: 2,
                id: 'frmDBRemark',
                title:$(this).attr("data-name"),
                content: url,
                offset: 'r',
                area: ['500px', '100%']
            });
        })

    })
}

//比对主表数据
function MainData_Compare(mainData, mapData) {
    debugger
    var style = foolStyle;
    if (mapData.FrmType == 8) {
        style = developStyle;
        //if (isTree == true)
            style = developStyle.replace("bottom: -12px","bottom:10px")
    }
       

    var val = "";
    
    if (isTree == true) {
        var iframe = vm.$refs['iframe-' + vm.activeItem];
        if (iframe != null && iframe != undefined) {
            iframe = iframe[0];
            curDocument = iframe.contentWindow.document;
        }
    }
    var isHaveChage = false;
    for (var keyOfEn in mainData) {
        var w = $("#TD_" + keyOfEn, curDocument).width() - 10;

        if ($("#TB_" + keyOfEn, curDocument).length != 0) {
            //如果是隐藏的不显示
            if ($("#TB_" + keyOfEn, curDocument).is(":hidden") == true)
                continue;
            val = $("#TB_" + keyOfEn, curDocument).val();
            if (val == mainData[keyOfEn])
                continue;
            if (mapData.FrmType == 8)
                w = $("#TB_" + keyOfEn, curDocument)[0].offsetWidth - 10;
            $("#TB_" + keyOfEn, curDocument).after("<div class='frmDBVer' style='" + style + "margin-left:" + w + "px;'data-info='" + keyOfEn + "' ></div>");
            isHaveChage = true;
            continue;
        }

        if ($("#DDL_" + keyOfEn, curDocument).length != 0) {
            val = $("#DDL_" + keyOfEn, curDocument).val();
            if (val == mainData[keyOfEn])
                continue;
            if (mapData.FrmType == 8)
                w = $("#DDL_" + keyOfEn, curDocument)[0].offsetWidth - 10;
            $("#DDL_" + keyOfEn, curDocument).after("<div  class='frmDBVer' style='" + style + "margin-left:" + w + "px;'data-info='" + keyOfEn + "'></div>");
            isHaveChage = true;
            continue;
        }
        if ($("#CB_" + keyOfEn, curDocument).length != 0) {
            val = $("#CB_" + keyOfEn).val();
            if (val == mainData[keyOfEn])
                continue;
            if (mapData.FrmType == 8)
                w = $("#CB_" + keyOfEn, curDocument)[0].offsetWidth - 10;
            $("#CB_" + keyOfEn, curDocument).after("<div  class='frmDBVer'style='" + style + "margin-left:" + w + "px;'data-info='" + keyOfEn + "'></div>");
            isHaveChage = true;
            continue;
        }
        if ($("input[name=RB_" + keyOfEn + "]", curDocument).length != 0) {
            val = $("input[name=RB_" + keyOfEn + "]:checked", curDocument).val();
            if (val == mainData[keyOfEn])
                continue;
            if ($("#TD_" + keyOfEn, curDocument).length == 0)
                w = $("#SR_" + keyOfEn, curDocument).width() - 10;

            $("#SR_" + keyOfEn, curDocument).append("<div  class='frmDBVer' style='" + style + "margin-left:" + w + "px;'data-info='" + keyOfEn + "'></div>");
            isHaveChage = true;
            continue;
        }
       
        if ($("input[name=CB_" + keyOfEn + "]", curDocument).length != 0) {
            var arry = [];
            $("input[name=CB_" + keyOfEn + "]:checked", curDocument).each(function (i, item) {
                arry.push($(this).val());
            });
            val = arry.join(",");
            if (val == mainData[keyOfEn])
                continue;
            if ($("#TD_" + keyOfEn, curDocument).length == 0)
                w = $("#SC_" + keyOfEn, curDocument).width() - 10;

            $("#SC_" + keyOfEn, curDocument).append("<div  class='frmDBVer' style='" + style + "margin-left:" + w + "px;'data-info='" + keyOfEn + "' ></div>");
            isHaveChage = true;
            continue;
        }
        
       
    }
    if (isHaveChage == false)
        layer.msg("主表信息没有发生变更");
    $('.frmDBVer', curDocument).on('click', function () {
        var field = $(this).attr("data-info");
        var name = $(this).attr("data-name");
        FrmDBVer_Show(field);
    });
}

/**
 * 撤销批注状态
 * */
function FrmDBVer_UnDo() {

    //做的特殊标记都删除掉.
    $(".frmDBVer", curDocument).remove();
    //批注状态.
    dbVerSta = 0;
}


/**
 * 显示批阅信息（弹窗显示）
 * @param {字段} field
 */
function FrmDBVer_Show(field) {
    var url = "./CCForm/FrmDBVerAndRemark.htm?Field=" + field + "&FieldType=0&FrmID=" + frmID + "&RFrmID=" + compareFrmID + "&RefPKVal=" + paramData.WorkID + "&IsEnable=0&PageType=0";
    layer.open({
        type: 2,
        id: 'frmDBRemark',
        title: '数据批阅',
        content: url,
        offset: 'r',
        area: ['500px', '100%']
    });

}
var foolStyle = "border-style: solid;border-width: 0px 0px 10px 10px;border-color: transparent transparent red transparent;width: 0px;height: 0px; position: absolute;bottom: -1px;";
var developStyle = "border-style: solid;border-width: 0px 0px 10px 10px;border-color: transparent transparent red transparent;width: 0px;height: 0px; position: relative;bottom: -12px;";



