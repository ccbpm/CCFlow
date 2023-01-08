 //DtlEditType 0 查看 1 编辑 2 新增
function Dtl_InitPage(DtlEditType, dtlNo, OID) {
    var frmData = dtlExt[dtlNo][0];
    //主表数据，用于变量替换.
    var mainTable = frmData["MainTable"]; //主表数据.
    //从表信息.
    sys_MapDtl = frmData["Sys_MapDtl"][0]; //从表描述.
    sys_mapAttr = frmData["Sys_MapAttr"]; //从表字段.
    var sys_mapExtDtl = frmData["Sys_MapExt"]; //扩展信息.
    var dbDtls = frmData["DBDtl"]; //从表数据.
    var mapDtls = frmData["MapDtls"]; //从表的从表集合.
    var gfs = new Entities("BP.Sys.GroupFields");
    gfs.Retrieve("FrmID", dtlNo);

    var dbDtl = new Entity(dtlNo, OID);

    //加入隐藏字段
    var html = "";
    $.grep(frmData.Sys_MapAttr, function (item) {
        return item.UIVisible == 0;

    }).forEach(function (attr) {
        var defval = ConvertDefVal(frmData, attr.DefVal, attr.KeyOfEn);
        html += "<input type='hidden' id='TB_" + attr.KeyOfEn + "' name='TB_" + attr.KeyOfEn + "' value='" + defval + "' />";
    });

    $("#DtlContent").html("").append(html);
    var isReadonly;
    if (DtlEditType == 0)
        isReadonly = "0";

    html = InitMapAttr(frmData.Sys_MapAttr, gfs[0].OID, frmData, isReadonly);

    $(html).appendTo('#DtlContent');

    //为 DISABLED 的 TEXTAREA 加TITLE 
    var disabledTextAreas = $('#form_Dtl textarea:disabled');
    $.each(disabledTextAreas, function (i, obj) {
        $(obj).attr('title', $(obj).val());
    })

    //根据NAME 设置ID的值
    var inputs = $('[name]');
    $.each(inputs, function (i, obj) {
        if ($(obj).attr("id") == undefined || $(obj).attr("id") == '') {
            $(obj).attr("id", $(obj).attr("name"));
        }
    });

    mui(".mui-switch").switch();

   

    //设置默认值
    for (var j = 0; j < frmData.Sys_MapAttr.length; j++) {

        var mapAttr = frmData.Sys_MapAttr[j];

        //添加 label
        //如果是整行的需要添加  style='clear:both'.
        var defValue = ConvertDefVal(dbDtl, mapAttr.DefVal, mapAttr.KeyOfEn);

        if ($('#TB_' + mapAttr.KeyOfEn).length == 1) {
            $('#TB_' + mapAttr.KeyOfEn).val(defValue);
            $('#TB_' + mapAttr.KeyOfEn).html(defValue);//只读大文本放到div里
            if (mapAttr.MyDataType == FormDataType.AppDate || mapAttr.MyDataType == FormDataType.AppDateTime)
                $('#LAB_' + mapAttr.KeyOfEn).html(defValue);
        }

        if ($('#DDL_' + mapAttr.KeyOfEn).length == 1) {
            // 判断下拉框是否有对应option, 若没有则追加
            if ($("option[value='" + defValue + "']", '#DDL_' + mapAttr.KeyOfEn).length == 0) {
                var mainTable = frmData.MainTable[0];
                var selectText = mainTable[mapAttr.KeyOfEn + "Text"];
                //@浙商银行
                if (selectText == undefined)
                    selectText = mainTable[mapAttr.FKText];
                if (selectText == undefined)
                    selectText = "";
                $('#DDL_' + mapAttr.KeyOfEn).append("<option value='" + defValue + "'>" + selectText + "</option>");
            }
            //
            $('#DDL_' + mapAttr.KeyOfEn).val(defValue);
        }

        if ($('#CB_' + mapAttr.KeyOfEn).length == 1) {
            if (defValue == "1") {
                $('#CB_' + mapAttr.KeyOfEn).attr("checked", true);
                //判断是否存在mui-active这个类
                if ($("#SW_" + mapAttr.KeyOfEn).hasClass("mui-active") == false)
                    $("#SW_" + mapAttr.KeyOfEn).addClass("mui-active");
            } else {
                $('#CB_' + mapAttr.KeyOfEn).attr("checked", false);
                //判断是否存在mui-active这个类
                if ($("#SW_" + mapAttr.KeyOfEn).hasClass("mui-active") == true)
                    $("#SW_" + mapAttr.KeyOfEn).removeClass("mui-active");
            }
        }

        if (mapAttr.UIIsEnable == "0") {

            $('#TB_' + mapAttr.KeyOfEn).attr('disabled', true);
            $('#DDL_' + mapAttr.KeyOfEn).attr('disabled', true);
            $('#CB_' + mapAttr.KeyOfEn).attr('disabled', true);
        }
    }

    if (isReadonly != 0)
        AfterBindEn_DealMapExt(frmData);


    //显示tb 提示信息.
    // ShowTbNoticeInfo();

    //初始化复选下拉框
    var selectPicker = $('.selectpicker');
    $.each(selectPicker, function (i, selectObj) {
        var defVal = $(selectObj).attr('data-val');
        var defValArr = defVal.split(',');
        $(selectObj).selectpicker('val', defValArr);
    });


    //根据OID获取
    $("#btn").html("");
    //获取dtl的信息 MapAttr,
    if (DtlEditType == 0) {
        //无操作
    }
    if (DtlEditType == 1) {

        //增加保存按钮
        $('<button type="button" class="mui-btn mui-btn-primary mui-btn-block" style="width: 95%;margin: 15px 10px; height: 46px; padding: 0px;" onclick="SaveDtl('+OID+',\''+dtlNo+'\',0)">保存</button>').appendTo('#btn');
    }
    if (DtlEditType == 2) {
        //增加新增按钮
        $('<button type="button" class="mui-btn mui-btn-primary mui-btn-block" style="width: 95%;margin: 15px 10px; height: 46px; padding: 0px;" onclick="SaveDtl(' + OID + ',\'' + dtlNo + '\',1,\'' + sys_MapDtl.MobileShowField+'\')">确认新增</button>').appendTo('#btn');
    }
    viewApi.go("#frmDtl");
}




function GetDtl(dbDtls, OID) {
    for (var i = 0; i < dbDtls.length; i++) {
        if (dbDtls[i].OID == OID)
            return dbDtls[i];
    }
}
function SaveDtl(OID, dtlNo,type,field) {
    if (type == 1) {
        var dtl = new Entity(dtlNo);
        dtl.RefPK = pageData.WorkID;
        dtl.FID = pageData.FID;
        dtl = dtl.Insert();
        dtl = JSON.parse(dtl);
        OID = dtl.OID;
    }
    //必填项和正则表达式检查
    var formCheckResult = true;

    if (checkBlanks() == false) {
        formCheckResult = false;
    }

    if (checkReg() == false) {
        formCheckResult = false;
    }

    if (formCheckResult == false) {
        mui.toast("保存失败，填写必填项！");
        return false;
    }

    var params = getFormData(true, true, "form_Dtl");
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    handler.AddPara("IsForDtl", 1);
    handler.AddPara("EnsName", dtlNo);
    handler.AddPara("RefPKVal", pageData.WorkID);
    handler.AddPara("OID", OID);
    handler.AddUrlData();
    $.each(params.split("&"), function (i, o) {
        var param = o.split("=");
        if (param.length == 2 && validate(param[1])) {
            handler.AddPara(param[0], decodeURIComponent(param[1], true));
        } else {
            handler.AddPara(param[0], "");
        }
    });
    
    var data = handler.DoMethodReturnString("FrmGener_Save");
    if (data.indexOf("err@") != -1) {
        alert(data);
        return;
    }
       
    mui.toast("保存成功");

    if (type == 1) {
        //返回主页
        viewApi.back();
         //获取添加按钮的上一个兄弟节点
        var ulObj = $("#Dtl_" + dtlNo).prev();
        //增加一条数据
        var _Html = "";
        var fieldValue = "";
        if ($("#TB_"+field).length != 0) {
            fieldValue = $("#TB_" + field).val();
        } else {
            if ($("#DDL_"+field).length != 0) {
                fieldValue = $("#DDL_"+field).val();
            } else {
                if ($("#CB_"+field).length != 0) {
                    fieldValue = $("#DDL_"+field).val();
                }
            }
        }
        _Html += '<li class="mui-table-view-cell mui-media">';
        _Html += '<a href="javascript:;">';
        _Html += "<button type='button' class='mui-btn mui-btn-danger mui-btn-outlined' style='right:90px' onclick='DeleteDtl(\"" + dtlNo + "\"," + OID + ",this)'>";
        _Html += '删除';
        _Html += '<span class="mui-icon mui-icon-trash"></span>';
        _Html += '</button>';
        _Html += "<button type='button' class='mui-btn mui-btn-success mui-btn-outlined' onclick='Dtl_InitPage(1,\"" + dtlNo + "\"," + OID + ")'>";
        _Html += '编辑';
        _Html += '<span class="mui-icon mui-icon-compose"></span>';
        _Html += '</button>';
        _Html += '<div class="mui-media-body">';
        _Html += fieldValue;
        _Html += ' </div>';
        _Html += '</a>';
        _Html += '</li>';
        ulObj.append(_Html);
       
      
       
    }
   
}