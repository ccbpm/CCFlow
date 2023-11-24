/**
*页面初始化
*/
$(function () {
     var node = new Entity("BP.WF.Node",GetQueryString("FK_Node"));
     document.title = node.Name;
     document.getElementById("title").innerHTML =node.Name;

    //加载JS文件 改变JS文件的加载方式 解决JS在资源中不显示的问题.
    var enName = "ND"+GetQueryString("FK_Node");
    if(enName == null || enName == "")
        enName = "ND"+parseInt(GetQueryString("FK_Flow"))+"01";
    try {
        ////加载JS文件
        var s = document.createElement('script');
        s.type = 'text/javascript';
        s.src = "../DataUser/JSLibData/" + enName + "_Self.js";
        var tmp = document.getElementsByTagName('script')[0];
        tmp.parentNode.insertBefore(s, tmp);
    }
    catch (err) {

    }

    try {
        var s = document.createElement('script');
        s.type = 'text/javascript';
        s.src = "../DataUser/JSLibData/" + enName + ".js";
        var tmp = document.getElementsByTagName('script')[0];
        tmp.parentNode.insertBefore(s, tmp);
    }
    catch (err) {

    }


    initPageParam(); //初始化参数

    InitToolBar(); //工具栏

    FlowFormTree_Init();//获取树形表单

   
    //加载mui的点击事件
    var ccforms = $(".ccform");
    //表单是否是第一次加载
    var IsFirstLoad = [];
    ccforms.each(function(i, ccform) {
        var id = this.getAttribute('id');
        IsFirstLoad[i] = true;
        ccform.addEventListener('tap', function() {
//             var collapse = $(".ccform").parents('.mui-collapse.mui-active');
//            if (collapse.length>0) {
//                 collapse[0].classList.remove("mui-active");
//              }
//             //设置页面的起始位置
//            mui($("#MainForm").parent()).scroll().refresh();
            //执行刷新事件
            if(IsFirstLoad[i] == true){
                GenerFrm(id);
                IsFirstLoad[i] = false;
           
          }
           

        });
        
    });

    
    //打开表单检查正则表达式
    if (typeof FormOnLoadCheckIsNull != 'undefined' && FormOnLoadCheckIsNull instanceof Function) {
        FormOnLoadCheckIsNull();
    }
});

//初始化网页URL参数
function initPageParam() {
    //新建独有
    pageData.UserNo = GetQueryString("UserNo");
    pageData.DoWhat = GetQueryString("DoWhat");
    pageData.IsMobile = GetQueryString("IsMobile");

    pageData.FK_Flow = GetQueryString("FK_Flow");
    pageData.FK_Node = GetQueryString("FK_Node");
    pageData.FID = GetQueryString("FID") == null ? 0 : GetQueryString("FID");
    pageData.WorkID = GetQueryString("WorkID");
    pageData.PWorkID = GetQueryString("PWorkID");
    pageData.IsRead = GetQueryString("IsRead");
    pageData.T = GetQueryString("T");
    pageData.Paras = GetQueryString("Paras");
    pageData.IsReadOnly = GetQueryString("IsReadOnly"); //如果是IsReadOnly，就表示是查看页面，不是处理页面
    pageData.IsStartFlow = GetQueryString("IsStartFlow"); //是否是启动流程页面 即发起流程
    pageData.DoType1 = GetQueryString("DoType");
}

//表单树初始化
function FlowFormTree_Init() {
    var href = GetHrefUrl();
    //表单参数没有传递过去, 这个需要把url 所有的参数都要传递过去.
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    var urlParam = href.substring(href.indexOf('?') + 1, href.length);
    urlParam = urlParam.replace('&DoType=', '&DoTypeDel=xx');
    urlParam = urlParam.replace('&DoType=', '&DoTypeDel=xx');
    handler.AddUrlData(urlParam);
    var data = handler.DoMethodReturnString("FlowFormTree_Init");

    if (data.indexOf('err@') == 0) {//发送时发生错误
        alert(data);
        return;
    }

    var pushData = cceval('(' + data + ')');

    //增加摘要字段的显示
    var node = new Entity("BP.WF.Node", pageData.FK_Node);
    var _html = "";
    if (node != null && node != undefined && node != "") {
        //获取节点表单的数据
        handler = new HttpHandler("BP.WF.HttpHandler.CCMobile_MyFlow");
        handler.AddJson(pageData);
        data = handler.DoMethodReturnString("GetNoteValue");
        if (data.indexOf('err@') == 0) {//发送时发生错误
            alert(data);
            return;
        }

        _html += "<div>";
        var nodeNote = cceval('(' + data + ')');
        for (var i = 0; i < nodeNote.length; i++) {
            _html += "<label style='padding-right:5px;font-weight:bold'>" + nodeNote[i].NoteVal + "</label>";
        }
        _html += "</div>";
        $("#note").html(_html);

    }
    _html = "";
    $.each(pushData, function (i) {
        var row = pushData[i];
           if (row.id == "00") {
               var childNodes = row.children;
               if (childNodes.length > 0) {
                   $.each(childNodes, function (i) {
                        var subrow = childNodes[i];
                        var subchildNodes = subrow.children;
                         if (subchildNodes.length > 0) {
                                $.each(subchildNodes, function (i) {
                                    var subchildNodesrow = subchildNodes[i];
                                    var fourchildNodes = subchildNodesrow.children;
                                    if (fourchildNodes.length > 0) {
                                        $.each(fourchildNodes, function (i,node) {
                                              //显示表单
                                               _html +="<li class='mui-table-view-cell mui-collapse'>";
                                               _html += "<a class='mui-navigate-right ccform' href='#' id='"+node.id+"'>"+subchildNodesrow.text+"-----"+node.text+"</a>";
                                               _html += "<div class='mui-collapse-content'>";
                                               _html += "<form  id='CCForm_"+node.id+"' method='post'  class='mui-input-group'>";
                                               _html += "</form>";
                                               //_html += "</div>";
                                               _html +="</div>";
                                               _html +="</li>";
                                        });
                                       
                                    } else {
                                         //显示表单
                                       _html +="<li class='mui-table-view-cell mui-collapse'>";
                                       _html += "<a class='mui-navigate-right ccform' href='#' id='"+subchildNodesrow.id+"'>"+subrow.text+"-----"+subchildNodesrow.text+"</a>";
                                       _html += "<div class='mui-collapse-content'>";
                                       _html += "<form id='CCForm_"+subchildNodesrow.id+"' method='post'  class='mui-input-group'>";
                                       _html += "</form>";
                                      // _html +="</div>";
                                       _html +="</div>";
                                       _html +="</li>";
                                        
                                    }
                                });
                        } else {
                            //显示表单
                            _html +="<li class='mui-table-view-cell mui-collapse'>";
                            _html += "<a class='mui-navigate-right ccform' href='#'  id='"+subrow.id+"' >"+subrow.text+"</a>";
                            _html += "<div class='mui-collapse-content'>";
                            _html += "<form  id='CCForm_"+subrow.id+"' method='post'  class='mui-input-group'>";
                            _html += "</form>";
                            _html +="</div>";
                            _html +="</li>";

                      }
                });
            }
        }
    });

    $("#flowtree").append(_html);

    //判断该节点是否启用了帮助提示 0 禁用 1 启用 2 强制提示 3 选择性提示
    HelpAlter(node);
}

//加载表单数据
function GenerFrm(FK_MapData) {

    var href = GetHrefUrl();
    var urlParam = href.substring(href.indexOf('?') + 1, href.length);
    urlParam = urlParam.replace('&DoType=', '&DoTypeDel=xx');
    var handler = new HttpHandler("BP.WF.HttpHandler.CCMobile_CCForm");
    handler.AddJson(pageData);
    handler.AddPara("FK_MapData", FK_MapData);
    handler.AddPara("IsMobile", 1);
    var data = handler.DoMethodReturnString("Frm_Init");
    if (data.indexOf('err@') == 0) {
        alert(data);
        return;
    }

    if (data.indexOf("FrmVSTO") == 0 || data.indexOf("FrmWord") == 0 || data.indexOf("FrmExcel") == 0) {
        mui.alter("VSTO、Work、Excel的表单未解析");
        return;
    }

    //自由表单或者傻瓜表单
    else if (data.indexOf("url@../FrmView") == 0) {
        handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
        var fk_flow = getQueryStringByNameFromUrl(data, "FK_Flow");
        var fk_node = getQueryStringByNameFromUrl(data, "FK_Node");
        var fid = getQueryStringByNameFromUrl(data, "FID");
        var workID = getQueryStringByNameFromUrl(data, "WorkID");

        //修改参数值
        pageData.WorkID = workID;
        pageData.FK_Flow = fk_flow;
        pageData.FK_Node = fk_node;
        pageData.FID = fid;

        handler.AddPara("FK_Flow", fk_flow);
        handler.AddPara("FK_Node", fk_node);
        handler.AddPara("FID", fid);
        handler.AddPara("WorkID", workID);
        handler.AddPara("FK_MapData", FK_MapData);
        data = handler.DoMethodReturnString("FrmGener_Init");
        if (data.indexOf('err@') == 0) {
            alert("装载表单出错, 请查看控制台console, 或者反馈给管理员.");
            return;
        }
        //URL的表单
    } else {
        data = data.replace("url@", "");
        var htmlobj = $.ajax({ url: data, async: false });
        if (htmlobj.status == 404)
            return;
        var str = htmlobj.responseText;
        return;
    }

    try {
        frmData = JSON.parse(data);
    }
    catch (err) {
        alert(" frmData数据转换JSON失败:" + data);
        console.log(data);
        return;
    }

    //获得sys_mapdata.
    var mapData = frmData["Sys_MapData"][0];
    //获取没有解析的外部数据源
    var uiBindKeys = frmData["UIBindKey"];
    if (uiBindKeys.length != 0) {
        //获取外部数据源 handler/JavaScript
        var operdata;
        for (var i = 0; i < uiBindKeys.length; i++) {
            var sfTable = new Entity("BP.Sys.SFTable", uiBindKeys[i].No);
            var srcType = sfTable.SrcType;
            if (srcType != null && srcType != "") {
                //Handler 获取外部数据源
                if (srcType == 5) {
                    var selectStatement = sfTable.SelectStatement;
                    if (plant == 'CCFlow')
                        selectStatement = basePath + "/DataUser/SFTableHandler.ashx" + selectStatement;
                    else
                        selectStatement = basePath + "/DataUser/SFTableHandler/" + selectStatement;
                    operdata = DBAccess.RunDBSrc(selectStatement, 1);
                }
                //JavaScript获取外部数据源
                if (srcType == 6) {
                    operdata = DBAccess.RunDBSrc(sfTable.FK_Val, 2);
                }
                frmData[uiBindKeys[i].No] = operdata;
            }
        }

    }
    
    //绑定表单
    BindFrm(frmData,FK_MapData);

   
    var isReadonly = GetQueryString("IsReadonly");

    //原有的。
    //为 DISABLED 的 TEXTAREA 加TITLE 
    var disabledTextAreas = $('#divCCForm textarea:disabled');
    $.each(disabledTextAreas, function (i, obj) {
        $(obj).attr('title', $(obj).val());
    })


    //根据NAME 设置ID的值
    var inputs = $('[name]');
    $.each(inputs, function (i, obj) {
        if ($(obj).attr("id") == undefined || $(obj).attr("id") == '') {
            $(obj).attr("id", $(obj).attr("name"));
        }
    })

    // 加载JS文件 改变JS文件的加载方式 解决JS在资源中不显示的问题.
    var enName = frmData.Sys_MapData[0].No;
    try {
        var s = document.createElement('script');
        s.type = 'text/javascript';
        s.src = "../../DataUser/JSLibData/" + FK_MapData + "_Self.js";
        var tmp = document.getElementsByTagName('script')[0];
        tmp.parentNode.insertBefore(s, tmp);
    }
    catch (err) {

    }

    try {
        var s = document.createElement('script');
        s.type = 'text/javascript';
        s.src = "../../DataUser/JSLibData/" + FK_MapData + ".js";
        var tmp = document.getElementsByTagName('script')[0];
        tmp.parentNode.insertBefore(s, tmp);
    }
    catch (err) {

    }

    CCFormLoaded(frmData,FK_MapData);
    //处理下拉框级联等扩展信息
    AfterBindEn_DealMapExt(frmData);


    //设置是否隐藏分组、获取字段所有分组的

    var fromId = "CCForm_" + FK_MapData;
    document.getElementById(fromId).onload = function () {
        var dividers = $("#" + fromId + " .mui-table-view-divider");
        var isHidden = false;
        $.each(dividers, function (i, obj) {
            //获取所有跟随的同胞元素，其中有不隐藏的div,就跳出循环
            var sibles = $(obj).nextAll();
            for (var k = 0; k < sibles.length; k++) {
                var sible = $(sibles[k]);
                if (sible.find(".mui-table-view-divider").length > 0)
                    break;
                if (sible.is(":hidden") == false) {
                    isHidden = false;
                    break;
                }
                isHidden = true;
            }
            if (isHidden == true)
                $(obj).hide();

        });
    }

}


//装载表单数据
function CCFormLoaded(frmData,FK_MapData){
    
    //设置默认值
    for (var j = 0; j < frmData.Sys_MapAttr.length; j++) {

        var mapAttr = frmData.Sys_MapAttr[j];

        //添加 label
        //如果是整行的需要添加  style='clear:both'.
        var defValue = ConvertDefVal(frmData.MainTable[0], mapAttr.DefVal, mapAttr.KeyOfEn);

        if (mapAttr.LGType == "2" && mapAttr.MyDataType == "1" && mapAttr.UIContralType == "1") {
            var uiBindKey = mapAttr.UIBindKey;
            if (uiBindKey != null && uiBindKey != undefined && uiBindKey != "") {
                var sfTable = new Entity("BP.Sys.SFTable");
                sfTable.SetPKVal(uiBindKey);
                var count = sfTable.RetrieveFromDBSources();
                if (count != 0 && sfTable.CodeStruct == "1") {
                    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Comm");
                    handler.AddPara("EnsName", uiBindKey);  //增加参数.
                    //获得map基本信息.
                    var pushData = handler.DoMethodReturnString("Tree_Init");
                    if (pushData.indexOf("err@") != -1) {
                        alert(pushData);
                        continue;
                    }
                    pushData = ToJson(pushData);
                    $('#CCForm_'+FK_MapData+' #DDL_' + mapAttr.KeyOfEn).combotree('loadData', pushData);
                    if (mapAttr.UIIsEnable == 0)
                        $('#CCForm_'+FK_MapData+' #DDL_' + mapAttr.KeyOfEn).combotree({ disabled: true });

                    $('#CCForm_'+FK_MapData+' #DDL_' + mapAttr.KeyOfEn).combotree('setValue', defValue);

                    continue;
                }
            }
        }

        if ($('#CCForm_'+FK_MapData+' #TB_' + mapAttr.KeyOfEn).length == 1) {
            if (mapAttr.MyDataType == 8)
                if (!/\./.test(defValue))
                    defValue += '.00';
            if ($("#CCForm_" + FK_MapData + " #SW_" + mapAttr.KeyOfEn).length == 1) {
                if (defValue == "1") {
                    //判断是否存在mui-active这个类
                    if ($("#CCForm_" + FK_MapData + " #SW_" + mapAttr.KeyOfEn).hasClass("mui-active") == false)
                        $("#CCForm_" + FK_MapData + " #SW_" + mapAttr.KeyOfEn).addClass("mui-active");
                } else {
                    //判断是否存在mui-active这个类
                    if ($("#CCForm_" + FK_MapData + " #SW_" + mapAttr.KeyOfEn).hasClass("mui-active") == true)
                        $("#CCForm_" + FK_MapData + " #SW_" + mapAttr.KeyOfEn).removeClass("mui-active");
                }
            }
           
            $('#CCForm_' + FK_MapData + ' #TB_' + mapAttr.KeyOfEn).val(defValue);
            $('#CCForm_' + FK_MapData + ' #TB_' + mapAttr.KeyOfEn).html(defValue);//只读大文本放到div里
            if (mapAttr.MyDataType == FormDataType.AppDate || mapAttr.MyDataType == FormDataType.AppDateTime)
                $('#CCForm_' + FK_MapData + ' #LAB_' + mapAttr.KeyOfEn).html(defValue);
        }

        if ($('#CCForm_'+FK_MapData+' #DDL_' + mapAttr.KeyOfEn).length == 1) {
            // 判断下拉框是否有对应option, 若没有则追加
            if (defValue!="" && $("option[value='" + defValue + "']", '#DDL_' + mapAttr.KeyOfEn).length == 0) {
                var mainTable = frmData.MainTable[0];
                var selectText = mainTable[mapAttr.KeyOfEn + "Text"];
                if (selectText == null || selectText == undefined || selectText == "")
                    selectText = mainTable[mapAttr.KeyOfEn + "T"];

                if (selectText != null && selectText != undefined && selectText != "")
                $('#CCForm_'+FK_MapData+' #DDL_' + mapAttr.KeyOfEn).append("<option value='" + defValue + "'>" + selectText + "</option>");
            }
            if(defValue!="")
                $('#CCForm_'+FK_MapData+' #DDL_' + mapAttr.KeyOfEn).val(defValue);
        }

        if ($('#CCForm_'+FK_MapData+' #CB_' + mapAttr.KeyOfEn).length == 1) {
            if (defValue == "1")
                $('#CCForm_'+FK_MapData+' #CB_' + mapAttr.KeyOfEn).attr("checked", true);
            else
                $('#CCForm_'+FK_MapData+' #CB_' + mapAttr.KeyOfEn).attr("checked", false);
        }

        //只读或者属性为不可编辑时设置
        if (mapAttr.UIIsEnable == "0" || pageData.IsReadonly == "1") {

            $('#CCForm_'+FK_MapData+' #TB_' + mapAttr.KeyOfEn).attr('disabled', true);
            $('#CCForm_'+FK_MapData+' #DDL_' + mapAttr.KeyOfEn).attr('disabled', true);
            $('#CCForm_'+FK_MapData+' #CB_' + mapAttr.KeyOfEn).attr('disabled', true);
        }
    }
}

var IsLoadFeedBack = false;
//绑定表单.
function BindFrm(frmData,FK_MapData) {

    //分组信息.
    var Sys_GroupFields = frmData.Sys_GroupField;
    var Fk_MapData = frmData.Sys_MapData[0].No;
    
    //遍历循环生成 li
    var mapAttrsHtml = "";
    for (var i = 0; i < Sys_GroupFields.length; i++) {

        var gf = Sys_GroupFields[i];
        if (gf.CtrlType != 'Ath')
            mapAttrsHtml += "<div class=\"mui-table-view-divider\"><h5 style='color:black;'>" + gf.Lab + "</h5></div>";

        //附件类的控件.
        if (gf.CtrlType == 'Ath') {
        	
            mapAttrsHtml += InitAth(frmData, gf);
            continue;
        }

        //明细表的控件.
        if (gf.CtrlType == 'Dtl') {
            mapAttrsHtml += InitDtl(frmData, gf);
            continue;
        }

        //字段类的控件.
        if (gf.CtrlType == '' || gf.CtrlType == null) {
            mapAttrsHtml += InitMapAttr(frmData.Sys_MapAttr, gf.OID,frmData);
            continue;
        }
    }
    //展显
    $("#CCForm_"+FK_MapData).html("").append(mapAttrsHtml);

    //节点属性
    var wf_node = new Entity("BP.WF.Node", pageData.FK_Node);
    //获取表单方案
    var frmNode = new Entity("BP.WF.Template.FrmNode", FK_MapData + "_" + pageData.FK_Node + "_" + pageData.FK_Flow);
    if (wf_node.FWCSta != 0 && frmNode != null && frmNode.IsEnableFWC == 1) {
        $("#CCForm_" + FK_MapData).append(figure_Template_FigureFrmCheck(wf_node));
        getWorkCheck();
    }



    //表单联动设置(傻瓜表单或者累加表单)
    if (frmData.Sys_MapData[0].FrmType == 0 || frmData.Sys_MapData[0].FrmType == 10)
        Set_Frm_Enable(frmData, "CCForm_" + FK_MapData);


    //为 DISABLED 的 TEXTAREA 加TITLE 
    var disabledTextAreas = $('#divCCForm textarea:disabled');
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

    if(frmData.Sys_FrmAttachment.length>0){
    	try {
            if (IsLoadFeedBack == false) {
                var s = document.createElement('script');
                s.type = 'text/javascript';
                s.src = "./js/mui/js/feedback-page.js";
                var tmp = document.getElementsByTagName('script')[0];
                tmp.parentNode.insertBefore(s, tmp);
                var s = document.createElement('script');
                IsLoadFeedBack = true
            }
        }
        catch (err) {
        }
    }
    


    //初始化复选下拉框
    var selectPicker = $('.selectpicker');
    $.each(selectPicker, function (i, selectObj) {
        var defVal = $(selectObj).attr('data-val');
        var defValArr = defVal.split(',');
        $(selectObj).selectpicker('val', defValArr);
    });


    var pickdates = $("#CCForm_"+FK_MapData+" .ccformdate");
    pickdates.each(function(i, pickdate) {
        var id = this.getAttribute('id');
         $("#"+id).html("<p>请选择时间<p>");
		pickdate.addEventListener('tap', function() {
			var _self = this;
			var optionsJson = this.getAttribute('data-options') || '{}';
			var options = JSON.parse(optionsJson);
			var id = this.getAttribute('id');
			/*
				* 首次显示时实例化组件
				* 示例为了简洁，将 options 放在了按钮的 dom 上
				* 也可以直接通过代码声明 optinos 用于实例化 DtPicker
				*/
			_self.picker = new mui.DtPicker(options);
			_self.picker.show(function(rs) {
				/*
					* rs.value 拼合后的 value
					* rs.text 拼合后的 text
					* rs.y 年，可以通过 rs.y.vaue 和 rs.y.text 获取值和文本
					* rs.m 月，用法同年
					* rs.d 日，用法同年
					* rs.h 时，用法同年
					* rs.i 分（minutes 的第二个字母），用法同年
					*/
								
				/* 
					* 返回 false 可以阻止选择框的关闭
					* return false;
					*/
				/*
					* 释放组件资源，释放后将将不能再操作组件
					* 通常情况下，不需要示放组件，new DtPicker(options) 后，可以一直使用。
					* 当前示例，因为内容较多，如不进行资原释放，在某些设备上会较慢。
					* 所以每次用完便立即调用 dispose 进行释放，下次用时再创建新实例。
					*/
                $("#"+id).html(rs.text);
    		    $("#TB_"+id.substr(4)).val(rs.text);
				_self.picker.dispose();
				_self.picker = null;
			});		
		}, false);
	});

}


//保存
function Save() {

    //必填项和正则表达式检查
    var formCheckResult = true;

    if (checkBlanks() == false) {
        formCheckResult = false;
    }

    if (checkReg() == false) {
        formCheckResult = false;
    }

    if (formCheckResult == false) {
        //alert("请检查表单必填项和正则表达式");
        return false;
    }

    setToobarDisiable();

    //分别保存表单数据
    var ccforms = $(".ccform");
    var isSave = true;
    ccforms.each(function(i, ccform) {
        var id = this.getAttribute('id');
        if(SaveForm("CCForm_"+id,id) == false){
            isSave = false;
            mui.toast("保存失败！");
         }
        
    });

    if(isSave)
        mui.toast("保存成功！");

}
function SaveForm(formId,fk_mapData){
    var url = "";

    var sys_MapData = frmData["Sys_MapData"][0];

     var params = getFormData(true, true,formId);

    if (sys_MapData.No.indexOf('ND') == 0) {
        var handler = new HttpHandler("BP.WF.HttpHandler.CCMobile_MyFlow");
       $.each(params.split("&"), function (i, o) {
            var param = o.split("=");
            if (param.length == 2 && validate(param[1])) {
                handler.AddPara(param[0], decodeURIComponent(param[1], true));
            } else {
                handler.AddPara(param[0], "");
            }
        });
        handler.AddPara("IsMobile", 1);
        var data = handler.DoMethodReturnString("Save");
    }else{
        var handler = new HttpHandler("BP.WF.HttpHandler.CCMobile_MyFlow");
        handler.AddPara("FK_MapData",fk_mapData);
       $.each(params.split("&"), function (i, o) {
            var param = o.split("=");
            if (param.length == 2 && validate(param[1])) {
                handler.AddPara(param[0], decodeURIComponent(param[1], true));
            } else {
                handler.AddPara(param[0], "");
            }
        });
        handler.AddPara("IsMobile", 1);
        handler.AddPara("OID", GetQueryString("WorkID"));
        var data = handler.DoMethodReturnString("FrmGener_Save");
    }
           
     if (data.indexOf('err@') == 0) {
            mui.alert(data);
            return false;
      }else{
        return true;
     }
}


//. 保存嵌入式表单
function SaveSelfFrom() {

    // 不支持火狐浏览器。
    var frm = document.getElementById('SelfForm');
    if (frm == null) {
        alert('系统错误,没有找到SelfForm的ID.');
    }
    //执行保存.
    return frm.contentWindow.Save();
}

function SendSelfFrom() {
    if (SaveSelfFrom() == false) {
        alert('表单保存失败，不能发送。');
        return false;
    }
    return true;
}

//发送
function SendIt() {
    //必填项和正则表达式检查.
    if (checkBlanks() == false) {
        mui.alert("检查必填项出现错误，边框变红颜色的是否填写完整？");
        return;
    }

    if (checkReg() == false) {
        mui.alert("发送错误:请检查字段边框变红颜色的是否填写完整？");
        return;
    }

    //保存数据
    Save();

    var toNode = 0;
    //含有发送节点且接收
    if ($('#DDL_ToNode').length > 0) {
        var selectToNode = $('#DDL_ToNode  option:selected').data();
        if (selectToNode.IsSelectEmps == "1") { //跳到选择接收人窗口
        	
            initModal("sendAccepter", selectToNode);
            return false;
        } else {
            toNode = selectToNode.No;
        }
    }

    execSend(toNode);
}

function execSend(toNode) {

    //先设置按钮等不可用
    setToobarDisiable();

    var handler = new HttpHandler("BP.WF.HttpHandler.CCMobile_MyFlow");
    handler.AddPara("IsMobile", 1);
    handler.AddPara("ToNode", toNode);
    handler.AddUrlData();
    var data = handler.DoMethodReturnString("Send"); //执行保存方法.


    if (data.indexOf('err@') == 0) { //发送时发生错误
        mui.alert(data);
        setToobarEnable();
        return;
    }

    if (data.indexOf('url@')>=0) { //发送成功时转到指定的URL 
        if(data.indexOf('Accepter')!=0 && data.indexOf('AccepterGener')==-1){
            //求出url里面的的FK_Node
            var params = data.split("&");
            var toNodeId='';
            for(var i=0;i<params.length;i++){
            	if(params[i].indexOf("ToNode")==-1)
            		continue;
            	toNodeId = params[i].split("=")[1];
            	break;
            }
            		
            var toNode = new Entity("BP.WF.Node",toNodeId);
            initModal("sendAccepter",toNode);
            $("#returnWorkModal").modal().show();
            return;
        }
        var url = data;
        url = url.replace('url@', '');
        SetHref(url);
        return;
    }

    data = data.replace('@', '<br/>@');
    data = data.replace(/@/g, '<br/>');
    OptSuc(data);
           
    return;
}


//停止流程.
function DoStop(msg, flowNo, workid) {

    if (confirm('您确定要执行 [' + msg + '] ?') == false)
        return;

    var para = 'DoType=MyFlow_StopFlow&FK_Flow=' + flowNo + '&WorkID=' + workid;

    AjaxService(para, function (msg, scope) {
        alert(msg);
        if (msg.indexOf('err@') == 0) {
            return;
        }
        SetHref('Todolist.aspx');
    });
}

//空方法，不能删除.
function SysCheckFrm() {

}
function KindEditerSync() {
    return true;
}

function Change() {
    var btn = document.getElementById('Btn_Save');
    if (btn != null) {
        if (btn.value.valueOf('*') == -1)
            btn.value = btn.value + '*';
    }
}
 
// 获取附件文件名称,如果附件没有上传就返回null.
function ReqAthFileName(athID) {
    var v = document.getElementById(athID);
    if (v == null) {
        return null;
    }
    var fileName = v.alt;
    return fileName;
}

//执行分支流程退回到分合流节点。
function DoSubFlowReturn(fid, workid, fk_node) {
    var url = 'ReturnWorkSubFlowToFHL.htm?FID=' + fid + '&WorkID=' + workid + '&FK_Node=' + fk_node;
    var v = WinShowModalDialog(url, 'df');
    SetHref(window.history.url);
}
function To(url) {
    //window.location.href = filterXSS(url);
    window.name = "dialogPage"; window.open(url, "dialogPage")
}

function DoDelSubFlow(fk_flow, workid) {
    if (window.confirm('您确定要终止进程吗？') == false)
        return;

    var para = 'DoType=DelSubFlow&FK_Flow=' + fk_flow + '&WorkID=' + workid;
    AjaxService(para, function (msg, scope) {
        alert(msg);
        Reload();
    });
}

//公共方法
function AjaxService(param, callback, scope, levPath) {
    $.ajax({
        type: "GET", //使用GET或POST方法访问后台
        dataType: "text", //返回json格式的数据
        contentType: "application/json; charset=utf-8",
        url: MyFlow, //要访问的后台地址
        data: param, //要发送的数据
        async: true,
        cache: false,
        complete: function () { }, //AJAX请求完成时隐藏loading提示
        error: function (XMLHttpRequest, errorThrown) {
            callback(XMLHttpRequest);
        },
        success: function (msg) {//msg为返回的数据，在这里做数据绑定
            var data = msg;
            callback(data, scope);
        }
    });
}

function Do(warning, url) {
    if (window.confirm(warning) == false)
        return;
    SetHref(url);
}

//关注 按钮.
function FocusBtn(btn, workid) {

    if (btn.value == '关注') {
        btn.value = '取消关注';
    }
    else {
        btn.value = '关注';
    }

    var para = "DoType=Focus&WorkID=" + workid;
    AjaxService(para, function (msg, scope) {
        // alert(msg);
    });
}

//确认 按钮.
function ConfirmBtn(btn, workid) {

    if (btn.value == '确认') {
        btn.value = '取消确认';
    }
    else {
        btn.value = '确认';
    }

    var para = "DoType=Confirm&WorkID=" + workid;
    AjaxService(para, function (msg, scope) {
        //  alert(msg);
    });
}



//将获取过来的URL参数转成URL中的参数形式  &
function pageParamToUrl() {
    var paramUrlStr = '';
    for (var param in pageData) {
        paramUrlStr += '&' + (param.indexOf('@') == 0 ? param.substring(1) : param) + '=' + pageData[param];
    }
    return paramUrlStr;
}

function addBarContent(barcount,bottombar,popoverBar,barHtml){
	if(barcount==4){
		bottombar.append('<a class="mui-tab-item" href="#Popover">更多</a>');
	}else if(barcount>4){
		popoverBar.append(barHtml);
	}else{
		bottombar.append(barHtml);
	}
}

//删除流程.
function DeleteFlow() {
    mui.confirm('您确定要删除吗？', function(e) {
        if (e.index == 1) {
            pageData = {
                WorkID: GetQueryString('WorkID'),
                FK_Flow: GetQueryString("FK_Flow")
            };

            var hand = "MyFlow.ashx";
            if (plant == "JFlow")
                plant = "MyFlow.do";

            var handler = new HttpHandler("BP.WF.HttpHandler.CCMobile_MyFlow");
            handler.AddPara("FK_Flow", pageData.FK_Flow);
            handler.AddPara("WorkID", pageData.WorkID);
            var data = handler.DoMethodReturnString("MyFlowGener_Delete");

            SetHref("Todolist.htm?1=2");
                        
        } 
    })  
}


//隐藏下方的功能按钮
function setToobarDisiable() {
    //隐藏下方的功能按钮
    $('.Bar input').css('background', 'gray');
    $('.Bar input').attr('disabled', 'disabled');
}

function setToobarEnable() {
    //隐藏下方的功能按钮
    $('.Bar input').css('background', '#2884fa');
    $('.Bar input').removeAttr('disabled');
}
//设置表单元素不可用
function setFormEleDisabled() {
    //文本框等设置为不可用
    $('#divCCForm textarea').attr('disabled', 'disabled');
    $('#divCCForm select').attr('disabled', 'disabled');
    $('#divCCForm input[type!=button]').attr('disabled', 'disabled');
}


 
//获得表单数据.
function getData(data, url, dataParam) {
    var jsonStr = '{"IsSuccess":true,"Msg":null,"ErrMsg":null,"List":null,"Data":2}';
    var data = JSON.parse(jsonStr);
    if (data.IsSuccess != true) {
        alert('返回参数失败，ErrMsg:' + data.ErrMsg + ";Msg:" + data.Msg + ";url:" + url);
    }
    return data;
}

var pageData = {};
var globalVarList = {};

//点击文件名称执行的下载.
function Down2017(mypk) {

    //组织url.
    var url = Handler + "?DoType=AttachmentUpload_Down&MyPK=" + mypk + "&m=" + Math.random();

    $.ajax({
        type: 'post',
        async: true,
        url: url,
        dataType: 'html',
        success: function (data) {

            if (data.indexOf('err@') == 0) {
                alert(data); //如果是异常，就提提示.
                return;
            }

            if (data.indexOf('url@') == 0) {

                data = data.replace('url@', ''); //如果返回url，就直接转向.

                var i = data.indexOf('\DataUser');
                var str = '/' + data.substring(i);
                str = str.replace('\\\\', '\\');
                window.open(str, "_blank");
                return;
            }
            alert(data);
            return;
        }
    });
}


//刷新子流程
function refSubSubFlowIframe() {
    var iframe = $('iframe[src*="SubFlow.aspx"]');
    iframe[0].contentWindow.location.href = filterXSS(iframe[0].src);
} 

window.onresize = function () {
    if (pageData.Col == 8) {
        if (jsonStr != undefined && jsonStr != '') {
            var frmData = JSON.parse(jsonStr);
            //设置CCFORM的表格宽度  
            if (document.body.clientWidth > 992) {//处于中屏时设置宽度最小值
                $('#CCForm').css('min-width', frmData.Sys_MapData[0].TableWidth);
            }
            else {
                $('#CCForm').css('min-width', 0);
            }
        }
    }
}

//逻辑类型、数据类型、控件类型
var FieldTypeS = { Normal: 0, Enum: 1, FK: 2, WinOpen: 3 },
    FormDataType = { AppString: 1, AppInt: 2, AppFloat: 3, AppBoolean: 4, AppDouble: 5, AppDate: 6, AppDateTime: 7, AppMoney: 8, AppRate: 9 },
    UIContralType = { TB: 0, DDL: 1, CheckBok: 2, RadioBtn: 3, MapPin: 4, MicHot: 5 };

//解析表单字段 MapAttr
function InitMapAttr(Sys_MapAttr, groupID,frmData) {

    var _html = "";
    $.grep(Sys_MapAttr, function (item) {
        return item.IsEnableInAPP != 0 && item.UIVisible != 0 && item.GroupID == groupID;

    }).forEach(function (attr) {
        
        //图片签名
        if (attr.IsSigan == "1") {
            _html += "<div class='mui-input-row'>";
            _html += FormUtils.CreateSignPicture(attr,frmData);
            _html += "</div>";
            return;
        }

        //加载其他数据控件
        switch (attr.LGType) {
            case FieldTypeS.Normal: //输出普通类型字段
                if (attr.UIContralType == UIContralType.DDL) {
                    //判断外部数据或WS类型. 
                    _html += "<div class='mui-input-row'>";                   
                    _html += FormUtils.CreateDDLPK(attr,frmData);
                    break;
                }
                switch (attr.MyDataType) {
                    case FormDataType.AppString:
                        _html += FormUtils.CreateTBString(attr,frmData);
                        break;
                    case FormDataType.AppInt:
                        _html += "<div class='mui-input-row'>";
                        _html += FormUtils.CreateTBInt(attr,frmData);
                        break;
                    case FormDataType.AppFloat:
                    case FormDataType.AppDouble:
                    case FormDataType.AppMoney:
                         _html += "<div class='mui-input-row'>";
                        _html += FormUtils.CreateTBFloat(attr,frmData);
                        break;
                    case FormDataType.AppDate:
                        //日期\boolen型的不允许获取焦点，所以只能禁用
                        _html += "<div class='mui-input-row'>";
                        _html += FormUtils.CreateTBDate(attr,frmData);
                        break;
                    case FormDataType.AppDateTime:
                        //日期\boolen型的不允许获取焦点，所以只能禁用
                        _html += "<div class='mui-input-row'>";
                        _html += FormUtils.CreateTBDateTime(attr,frmData);
                        break;
                    case FormDataType.AppBoolean:
                        //日期\boolen型的不允许获取焦点，所以只能禁用
                        _html += "<div class='mui-input-row'>";
                        _html += FormUtils.CreateCBBoolean(attr,frmData);
                        break;
                }
                break;
            case FieldTypeS.Enum: //枚举值下拉框
                _html += "<div class='mui-input-row'>";
                _html += FormUtils.CreateDDLEnum(attr,frmData);
                break;
            case FieldTypeS.FK: //外键表下拉框  
                _html += "<div class='mui-input-row'>";
                _html += FormUtils.CreateDDLPK(attr,frmData);
                break;
            case FieldTypeS.WinOpen: //打开系统页面
                _html += "<div class='mui-input-row'>";
                switch (item.UIContralType) {
                    case UIContralType.MapPin: //地图定位
                        _html += FormUtils.CreateMapPin(attr,frmData);
                        break;
                    case UIContralType.MicHot: //语音控件
                        _html += FormUtils.CreateMicHot(attr,frmData);
                        break;
                }
                break;
        }
        _html += "</div>";
    });

    return _html;
}

var FormUtils = {
    CreateSignPicture: function (attr,frmData) {
        //图片签名+oitw "kyrw   \[i6514
        var val = ConvertDefVal(frmData.MainTable[0], attr.DefVal, attr.KeyOfEn);
        var html_Sign = "<label for=\"Sign_" + attr.KeyOfEn + "\"><p>" + attr.Name + "</p></label>";
        html_Sign += "<div align=\"left\">";
        //如果是图片签名，并且可以编辑
        if (attr.UIIsEnable == 1) {
            
            //是否签过
            var sealData = new Entities("BP.Tools.WFSealDatas");
            sealData.Retrieve("OID", GetQueryString("WorkID"), "FK_Node", GetQueryString("FK_Node"), "SealData", GetQueryString("UserNo"));

            if (sealData.length > 0) {
                val = sealData[0].SealData;
                html_Sign += "<input  type='hidden' maxlength=" + attr.MaxLen + "  id='TB_" + attr.KeyOfEn + "' name='TB_" + attr.KeyOfEn + "' value='" + val + "' />";
                html_Sign += "<img src='../../DataUser/Siganture/" + val + ".jpg' onerror=\"this.src='../../DataUser/Siganture/UnName.jpg'\"  style='border:0px;width:100px;height:30px;' id='Img" + attr.KeyOfEn + "' />";
                isSigantureChecked = true;
            }
            else {
                html_Sign += "<input maxlength=" + attr.MaxLen + "  id='TB_" + attr.KeyOfEn + "' name='TB_" + attr.KeyOfEn + "' value='" + val + "' type=hidden />";
                html_Sign += "<img src='../../DataUser/Siganture/siganture.jpg' onerror=\"this.src='../../DataUser/Siganture/UnName.jpg'\" ondblclick='figure_Template_Siganture(\"" + attr.KeyOfEn + "\",\"" + val + "\")' style='border:0px;width:100px;height:30px;' id='Img" + attr.KeyOfEn + "' />";
            }
             html_Sign += "</div>";
            return html_Sign;
        }
        //如果不可编辑，并且是图片名称
        if (attr.IsSigan == "1") {
            html_Sign += "<input maxlength=" + attr.MaxLen + "  id='TB_" + attr.KeyOfEn + "'  name='TB_" + attr.KeyOfEn + "' value='" + val + "' type=hidden />";
            html_Sign += "<img src='../../DataUser/Siganture/" + val + ".jpg' onerror=\"this.src='../../DataUser/Siganture/siganture.jpg'\" style='border:0px;width:100px;height:30px;' id='Img" + attr.KeyOfEn + "' />";
            html_Sign += "</div>";
            return html_Sign;
        }
       
    },
    CreateTBString: function (attr,frmData) {
        var html_string = "";
        var strPlaceholder = "";
        //启用二维码
        if (attr.IsEnableQrCode && attr.IsEnableQrCode == "1") {
            html_string += "<div class='mui-input-row'>";
            strPlaceholder = "通过扫一扫添加";
            Form_Ext_Function += "$('#Btn_" + attr.KeyOfEn + "').on('tap', function () { QrCodeToInput('TB_" + attr.KeyOfEn + "'); });"
            html_string += "<label for=\"TB_" + attr.KeyOfEn + "\">" + attr.Name + "</label>";
            html_string += "<div class=\"QrCodeBar ui-grid-a\">";
            html_string += "  <div class=\"ui-block-a\">";
            html_string += "      <input " + (attr.UIIsEnable == "0" ? "disabled" : "") + " type='text' name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" placeholder=\"" + strPlaceholder + "\" />";
            html_string += "  </div>";
            html_string += "  <div class=\"ui-block-b\">";
            html_string += "      <div style='margin-top:12px;'>";
            html_string += "         <img id='Btn_" + attr.KeyOfEn + "' src='image/Field/scanQbar.png' width='29' height='24'/>";
            html_string += "      </div>";
            html_string += "  </div>";
            html_string += "</div>";
            return html_string;
        }

        //大文本备注信息解析
        if (attr.UIContralType == 60) {
            html_string += "<div class='' style='padding:11px 15px;line-height:1.1;'>";
            var filename = basePath + "/DataUser/CCForm/BigNoteHtmlText/" + attr.FK_MapData + ".htm";
            var htmlobj = $.ajax({ url: filename, async: false });
            var str = htmlobj.responseText;
            if (htmlobj.status == 404)
                str == "";
            html_string += str;
            html_string += "</div>";
            return html_string;
        }

        //多行文本
        if (attr.UIHeight > 30) {
            html_string += "<div class='' style='padding:11px 15px;line-height:1.1;'>";
            html_string += "<label for=\"TB_" + attr.KeyOfEn + "\"><p>" + attr.Name + "</p></label>";
           html_string += "<textarea wrap='virtual' onpropertychange= 'this.style.posHeight=this.scrollHeight' readonly='readonly' cols='40' style='width:100%;border:solid 1px gray;' rows=\"5\" placeholder=\"" + strPlaceholder + "\" name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\"></textarea>";
           
            return html_string;
        }

        //单行文本
        if (attr.UIIsInput == 1 && attr.UIIsEnable == 1) {
            html_string += "<div class='mui-input-row'>";
            html_string += "<label for=\"TB_" + attr.KeyOfEn + "\"  class='mustInput' ><p>" + attr.Name + "</p></label>";
        } else {
            html_string += "<div class='mui-input-row'>";
            html_string += "<label for=\"TB_" + attr.KeyOfEn + "\" ><p>" + attr.Name + "</p></label>";
        }
         html_string += "<input style='background-color:#fff ' readonly='readonly' type='text' name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" placeholder=\"" + strPlaceholder + "\" />";
       
        return html_string;
    },
    CreateTBInt: function (attr,frmData) {
        var inputHtml = "<label style='background-color:#fff' for=\"TB_" + attr.KeyOfEn + "\"><p>" + attr.Name + "</p></label>";

        inputHtml += "<input style='background-color:#fff' type=\"number\" pattern=\"[0 - 9] * \"";
        inputHtml += " name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" placeholder='0' />";

        return inputHtml;
    },
    CreateTBFloat: function (attr,frmData) {
        return "<label style='background-color:#fff' for=\"TB_" + attr.KeyOfEn + "\"><p>" + attr.Name + "</p></label><input style='backgroud-color:#fff' type=\"number\" name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" placeholder=\"0.00\" />";
    },
    CreateTBDate: function (attr,frmData) {

        var inputHtml = "<label for=\"TB_" + attr.KeyOfEn + "\"><p>" + attr.Name + "</p></label>";
        if (attr.UIIsEnable == "0"){
        	inputHtml += "<input readonly='readonly' type='text' name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" />";	
        }
        else{
        	inputHtml += "<a class='mui-navigate-right'>";
        	inputHtml += "  <span name=\"LAB_" + attr.KeyOfEn + "\" id=\"LAB_" + attr.KeyOfEn + "\" data-options='{\"type\":\"date\"}' class='mui-pull-right ccformdate' style='min-width:140px;margin-top:7px'><p>请选择日期</p></span>";
        	inputHtml += "</a>";
        	inputHtml += "<input  type='hidden' name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" />";	
        }
        return inputHtml;
    },
    CreateTBDateTime: function (attr,frmData) {
        //Form_Ext_Function += "$('#TB_" + attr.KeyOfEn + "').datetimepicker({lang:'ch'});";
        var inputHtml = "<label for=\"TB_" + attr.KeyOfEn + "\"><p>" + attr.Name + "</p></label>";

        if (attr.UIIsEnable == "0"){
        	inputHtml += "<input name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" readonly='readonly' type='text' />";	
        }            
        else{
        	inputHtml += "<a class='mui-navigate-right'>";
        	inputHtml += " <label name=\"LAB_" + attr.KeyOfEn + "\" id=\"LAB_" + attr.KeyOfEn + "\" data-options='{\"type\":\"datetime\"}' class='mui-pull-right ccformdate' style='min-width:180px;margin-top:7px'>请选择时间</label>";
        	inputHtml += "</a>";
        	inputHtml += "<input  type='hidden' name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" />";	
        }
        return inputHtml;
    },
    CreateCBBoolean: function (attr,frmData) {
        var checkBoxVal = "";
        var keyOfEn = attr.KeyOfEn;
        var CB_Html = "";
        CB_Html += "  <label><p>" + attr.Name + "</p></label>";
        CB_Html += "  <input type='hidden' name='CB_" + keyOfEn + "' value='0'/>";
        CB_Html += "  <div class='mui-switch mui-switch-blue mui-switch-mini mui-active'>";
        CB_Html += "      <div class='mui-switch-handle'></div>";
        CB_Html += "  </div>";
        //CB_Html += "  <input readonly='" + (attr.UIIsEnable == "0" ? "readonly" : "") + "' type=\"checkbox\" name=\"CB_" + keyOfEn + "\" id=\"CB_" + keyOfEn + "\" " + checkBoxVal + " />";
        return CB_Html;
    },
    CreateDDLEnum: function (attr,frmData) {
        //下拉框和单选都使用下拉框实现
        var ctrl_ID = "RB_" + attr.KeyOfEn;
        if (attr.UIContralType == UIContralType.DDL) {
            ctrl_ID = "DDL_" + attr.KeyOfEn;
        }

        var html_Select = "<label for=\"" + ctrl_ID + "\"><p>" + attr.Name + "</p></label>";
        html_Select += "<select name=\"" + ctrl_ID + "\" id=\"" + ctrl_ID + "\"  " + (attr.UIIsEnable == "0" ? "disabled" : "") + " >";

        html_Select += InitDDLOperation(frmData,attr, "");
        html_Select += "</select>";
        return html_Select;
    },
    CreateDDLPK: function (attr,frmData) {
        var html_Select = "<label for=\"DDL_" + attr.KeyOfEn + "\"><p>" + attr.Name + "</p></label>";
        html_Select += "<select name=\"DDL_" + attr.KeyOfEn + "\" id=\"DDL_" + attr.KeyOfEn + "\" readonly='" + (attr.UIIsEnable == "0" ? "readonly" : "") + "'>";

        html_Select += InitDDLOperation(frmData,attr, "");
        html_Select += "</select>&nbsp;&nbsp;";
        return html_Select;
    },
    CreateMapPin: function (attr,frmData) {
        var html_MapPin = "<label for=\"TB_" + attr.KeyOfEn + "\">" + attr.Name + "</label>";
        //展示内容
        html_MapPin += "<div align=\"left\">";
        if (this.Enable == false) {
            html_MapPin += "<img name=\"MapPin_" + attr.KeyOfEn + "\" id=\"MapPin_" + attr.KeyOfEn + "\" src='image/Field/ic_pindisabled.png' border=0  width=\"" + attr.UIWidth + "\" height=\"" + attr.UIHeight + "\" align='middle'/>";
        } else {
            html_MapPin += "<img onclick=\"GetMapLocationAddress('" + attr.KeyOfEn + "')\" name=\"MapPin_" + attr.KeyOfEn + "\" id=\"MapPin_" + attr.KeyOfEn + "\" src='image/Field/ic_pin.png' border=0 width=\"" + attr.UIWidth + "\" height=\"" + attr.UIHeight + "\" align='middle'/>";
        }
        html_MapPin += "<span onclick=\"OpenMapView('" + attr.KeyOfEn + "')\" style=\"margin-left:5px;\" name=\"LBL_" + attr.KeyOfEn + "\" id=\"LBL_" + attr.KeyOfEn + "\"></span>";
        html_MapPin += "</div>";
        //数据控件
        html_MapPin += "<input type='hidden' name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" />";
        //地图定位
        return html_MapPin;
    },
    CreateMicHot: function (attr,frmData) {
        var html_MicHot = "<label for=\"TB_" + attr.KeyOfEn + "\">" + attr.Name + "</label>";
        var bDelete = this.Enable;
        //展示内容
        html_MicHot += "<div>";
        if (this.Enable == false) {
            html_MicHot += "<img align=\"left\" name=\"MicHot_" + attr.KeyOfEn + "\" id=\"MicHot_" + attr.KeyOfEn + "\" src='image/Field/microphonedisabled.png' border=0  width=\"" + attr.UIWidth + "\" height=\"" + attr.UIHeight + "\"/>";
        } else {
            html_MicHot += "<img align=\"left\" onclick=\"StartOpenRecord('" + attr.KeyOfEn + "')\" name=\"MicHot_" + attr.KeyOfEn + "\" ";
            html_MicHot += "id=\"MicHot_" + attr.KeyOfEn + "\" src='image/Field/microphonehot.png' border=0 width=\"" + attr.UIWidth + "\" height=\"" + attr.UIHeight + "\"/>";
        }
        html_MicHot += "<img src='image/Field/wx_startplay.gif' align='middle' style='display:none;' />";
        html_MicHot += "<div align=\"left\" style=\"margin-left:15px;float:left;\" name=\"Recorde_" + attr.KeyOfEn + "\" id=\"Recorde_" + attr.KeyOfEn + "\"></div>";
        html_MicHot += "</div><br /><br />";
        html_MicHot += "<div id=\"PanelRecords_" + attr.KeyOfEn + "\">";

        //获取历史语音
        var args = new RequestArgs();
        var keyOfEn = attr.KeyOfEn;

        html_MicHot += "</div>";
        //语音
        return html_MicHot;
    }
};

//双击签名
function figure_Template_Siganture(SigantureID, val, type) {
    //先判断，是否存在签名图片
    var handler = new HttpHandler("BP.WF.HttpHandler.WF");
    handler.AddPara('No', val);
    data = handler.DoMethodReturnString("HasSealPic");

    //如果不存在，就显示当前人的姓名
    if (data.length > 0 && type == 0) {
        $("#TB_" + SigantureID).val(data);
        var obj = document.getElementById("Img" + SigantureID);
        var impParent = obj.parentNode; //获取img的父对象
        impParent.removeChild(obj);
    }
    else {
        val = new WebUser().No;
         $("#TB_" + SigantureID).val(val);
        var src = '/DataUser/Siganture/' + val + '.jpg';    //新图片地址
        document.getElementById("Img" + SigantureID).src = src;
    }
    isSigantureChecked = true;
    document.getElementById("Img" + SigantureID).ondblclick = null;
    var sealData = new Entities("BP.Tools.WFSealDatas");
    sealData.Retrieve("OID", GetQueryString("WorkID"), "FK_Node", GetQueryString("FK_Node"), "SealData", GetQueryString("UserNo"));
    if (sealData.length > 0) {
        return;
    }
    else {
        sealData = new Entity("BP.Tools.WFSealData");
        sealData.MyPK = GetQueryString("WorkID") + "_" + GetQueryString("FK_Node") + "_" + val;
        sealData.OID = GetQueryString("WorkID");
        sealData.FK_Node = GetQueryString("FK_Node");
        sealData.SealData = val;
        sealData.Save();
    }

}

//AtPara  @PopValSelectModel=0@PopValFormat=0@PopValWorkModel=0@PopValShowModel=0
function GepParaByName(name, atPara) {
    var params = atPara.split('@');
    var result = $.grep(params, function (value) {
        return value != '' && value.split('=').length == 2 && value.split('=')[0] == value;
    })
    return result;
}

//初始化下拉列表框的OPERATION
function InitDDLOperation(frmData, mapAttr, defVal) {

    var operations = '';
    var data = frmData[mapAttr.KeyOfEn];
    if (data == undefined)
        data = frmData[mapAttr.UIBindKey];
    if (data == undefined) {
        //枚举类型的.
        if (mapAttr.LGType == 1) {
            var enums = frmData.Sys_Enum;
            enums = $.grep(enums, function (value) {
                return value.EnumKey == mapAttr.UIBindKey;
            });


            $.each(enums, function (i, obj) {
                operations += "<option " + (obj.IntKey == mapAttr.DefVal ? " selected='selected' " : "") + " value='" + obj.IntKey + "'>" + obj.Lab + "</option>";
            });
        }
        return operations;
    }
    $.each(data, function (i, obj) {
        operations += "<option " + (obj.No == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";
    });
    return operations;
}

//填充默认数据
function ConvertDefVal(MainTable, defVal, keyOfEn) {
    var result = defVal;
    //通过MAINTABLE返回的参数
    for (var ele in MainTable) {
        if (keyOfEn == ele) {
            result = MainTable[ele];
            break;
        }
    }
    return result = unescape(result);
}

//获取表单数据
function getFormData(isCotainTextArea, isCotainUrlParam,formId) {

    //KindEditor 1:调用serialize之前把 KindEditor 数据放进去.
    if (window.editor) {
        $("textarea[name='" + editor.srcElement.attr("name") + "']").val(editor.html());
    }

    var formss = $('#'+formId).serialize();
    var formArr = formss.split('&');
    var formArrResult = [];
    //获取CHECKBOX的值
    $.each(formArr, function (i, ele) {
        if (ele.split('=')[0].indexOf('CB_') == 0) {
            if ($('#' + ele.split('=')[0] + ':checked').length == 1) {
                ele = ele.split('=')[0] + '=1';
            } else {
                ele = ele.split('=')[0] + '=0';
            }
        }
        formArrResult.push(ele);
    });
    
  //复选框checkbox未选中时序列化时不包含的添加
    var checkBoxs = $('input[type=checkbox]');
    $.each(checkBoxs, function (i, checkBox) {
    	//@浙商银行
    	var name = $(checkBox).attr("name");
    	if($("input[name='"+name+"']:checked").length==0){
    		formArrResult.push(name+'=0');
    	}
    });

    //获取表单中禁用的表单元素的值
    var disabledEles = $('#divCCForm :disabled');
    $.each(disabledEles, function (i, disabledEle) {
        var name = $(disabledEle).attr('name');
        switch (disabledEle.tagName.toUpperCase()) {
            case "INPUT":
                switch (disabledEle.type.toUpperCase()) {
                    case "CHECKBOX": //复选框
                        formArrResult.push(name + '=' + $(disabledEle).is(':checked') ? 1 : 0);
                        break;
                    case "TEXT": //文本框
                        formArrResult.push(name + '=' + $(disabledEle).val());
                        break;
                    case "RADIO": //单选钮
                        var eleResult = name + '=' + $('[name="' + name + ':checked"]').val();
                        if (!$.inArray(formArrResult, eleResult)) {
                            formArrResult.push();
                        }
                        break;
                }
                break;
            //下拉框      
            case "SELECT":
                formArrResult.push(name + '=' + $(disabledEle).children('option:checked').val());
                break;   
            case "TEXTAREA":
                formArrResult.push(name + '=' + $(disabledEle).val());
                break;
        }
    });

    if (!isCotainTextArea) {
        formArrResult = $.grep(formArrResult, function (value) {
            return value.split('=').length == 2 ? value.split('=')[1].length <= 50 : true;
        });
    }

    formss = formArrResult.join('&');
    var dataArr = [];
    //加上URL中的参数
    if (pageData != undefined && isCotainUrlParam) {
        var pageDataArr = [];
        for (var data in pageData) {
            pageDataArr.push(data + '=' + pageData[data]);
        }
        dataArr.push(pageDataArr.join('&'));
    }
    if (formss != '')
        dataArr.push(formss);
    var formData = dataArr.join('&');


    //为了复选框  合并一下值  复选框的值以  ，号分割
    //用& 符号截取数据
    var formDataArr = formData.split('&');
    var formDataResultObj = {};
    $.each(formDataArr, function (i, formDataObj) {
        //计算出等号的INDEX
        var indexOfEqual = formDataObj.indexOf('=');
        var objectKey = formDataObj.substr(0, indexOfEqual);
        var objectValue = formDataObj.substr(indexOfEqual + 1);
        if (formDataResultObj[objectKey] == undefined) {
            formDataResultObj[objectKey] = objectValue;
        } else {
            formDataResultObj[objectKey] = formDataResultObj[objectKey] + ',' + objectValue;
        }
    });

    var formdataResultStr = '';
    for (var ele in formDataResultObj) {
        formdataResultStr = formdataResultStr + ele + '=' + formDataResultObj[ele] + '&';
    }
    return formdataResultStr;
}
 


//发送 退回 移交等执行成功后转到  指定页面
function OptSuc(msg) {
	msg = msg.replace("@查看<img src='/WF/Img/Btn/PrintWorkRpt.gif' >", '')
    mui.alert(msg.replace(/@/g, '<br/>'));

}
//移交
//初始化发送节点下拉框
function InitToNodeDDL(flowData) {

    if (flowData.ToNodes == undefined)
        return;

    if (flowData.ToNodes.length == 0)
        return;

    //如果没有发送按钮，就让其刷新,说明加载不同步.
    var btn = $('[name=Send]');
    if (btn == null || btn == undefined) {
        Reload();
        return;
    }

    //var flowData = JSON.parse(jsonStr);

    if (flowData.ToNodes != undefined && flowData.ToNodes.length > 0) {
        // $('[value=发送]').
        var toNodeDDL = $('<select style="width:auto;" id="DDL_ToNode"></select>');
        $.each(flowData.ToNodes, function (i, toNode) {

            var opt = "";
            if (toNode.IsSelected == "1") {
                var opt = $("<option value='" + toNode.No + "' selected='true' >" + toNode.Name + "</option>");
                opt.data(toNode);
            } else {
                var opt = $("<option value='" + toNode.No + "'>" + toNode.Name + "</option>");
                opt.data(toNode);
            }

            toNodeDDL.append(opt);
        });


        $('[name=Send]').after(toNodeDDL);
    }
}

//根据下拉框选定的值，弹出提示信息  绑定那个元素显示，哪个元素不显示  
function ShowNoticeInputInfo() {
    var flowData = JSON.parse(jsonStr);
    var rbs = flowData.Sys_FrmRB;
    data = rbs;
    $("input[type=radio],select").bind('change', function (obj) {
        var needShowDDLids = [];
        var methodVal = obj.target.value;

        for (var j = 0; j < data.length; j++) {
            var value = data[j].IntKey;
            var noticeInfo = data[j].Tip;
            var drdlColName = data[j].KeyOfEn;

            if (obj.target.tagName == "SELECT") {
                drdlColName = 'DDL_' + drdlColName;
            } else {
                drdlColName = 'RB_' + drdlColName;
            }
            //if (methodVal == value &&  obj.target.name.indexOf(drdlColName) == (obj.target.name.length - drdlColName.length)) {
            if (methodVal == value && (obj.target.name == drdlColName)) {
                //高级JS设置;  设置表单字段的  可用 可见 不可用 
                var fieldConfig = data[j].FieldsCfg;
                var fieldConfigArr = fieldConfig.split('@');
                for (var k = 0; k < fieldConfigArr.length; k++) {
                    var fieldCon = fieldConfigArr[k];
                    if (fieldCon != '' && fieldCon.split('=').length == 2) {
                        var fieldConArr = fieldCon.split('=');
                        var ele = $('[name$=' + fieldConArr[0] + ']');
                        if (ele.length == 0) {
                            continue;
                        }
                        var labDiv = undefined;
                        var eleDiv = undefined;
                        if (ele.css('display').toUpperCase() == "NONE") {
                            continue;
                        }

                        if (ele.parent().attr('class').indexOf('input-group') >= 0) {
                            labDiv = ele.parent().parent().prev();
                            eleDiv = ele.parent().parent();
                        } else {
                            labDiv = ele.parent().prev();
                            eleDiv = ele.parent();
                        }
                        switch (fieldConArr[1]) {
                            case "1": //可用
                                if (labDiv.css('display').toUpperCase() == "NONE" && ele[0].id.indexOf('DDL_') == 0) {
                                    needShowDDLids.push(ele[0].id);
                                }

                                labDiv.css('display', 'block');
                                eleDiv.css('display', 'block');
                                ele.removeAttr('disabled');


                                break;
                            case "2": //可见
                                if (labDiv.css('display').toUpperCase() == "NONE" && ele[0].id.indexOf('DDL_') == 0) {
                                    needShowDDLids.push(ele[0].id);
                                }

                                labDiv.css('display', 'block');
                                eleDiv.css('display', 'block');
                                break;
                            case "3": //不可见
                                labDiv.css('display', 'none');
                                eleDiv.css('display', 'none');
                                break;
                        }
                    }
                }
                //根据下拉列表的值选择弹出提示信息
                if (noticeInfo == undefined || noticeInfo.trim() == '') {
                    break;
                }
                noticeInfo = noticeInfo.replace(/\\n/g, '<br/>')
                var selectText = '';
                if (obj.target.tagName.toUpperCase() == 'INPUT' && obj.target.type.toUpperCase() == 'RADIO') {//radio button
                    selectText = obj.target.nextSibling.textContent;
                } else {//select
                    selectText = $(obj.target).find("option:selected").text();
                }
                $($('#div_NoticeInputInfo .popover-title span')[0]).text(selectText);
                $('#div_NoticeInputInfo .popover-content').html(noticeInfo);

                var top = obj.target.offsetHeight;
                var left = obj.target.offsetLeft;
                var current = obj.target.offsetParent;
                while (current !== null) {
                    left += current.offsetLeft;
                    top += current.offsetTop;
                    current = current.offsetParent;
                }


                if (obj.target.tagName.toUpperCase() == 'INPUT' && obj.target.type.toUpperCase() == 'RADIO') {//radio button
                    left = left - 40;
                    top = top + 10;
                }
                if (top - $('#div_NoticeInputInfo').height() - 30 < 0) {
                    //让提示框在下方展示
                    $('#div_NoticeInputInfo').removeClass('top');
                    $('#div_NoticeInputInfo').addClass('bottom');
                    top = top;
                } else {
                    $('#div_NoticeInputInfo').removeClass('bottom');
                    $('#div_NoticeInputInfo').addClass('top');
                    top = top - $('#div_NoticeInputInfo').height() - 30;
                }
                $('#div_NoticeInputInfo').css('top', top);
                $('#div_NoticeInputInfo').css('left', left);
                $('#div_NoticeInputInfo').css('display', 'block');
                //$("#btnNoticeInfo").popover('show');
                //$('#btnNoticeInfo').trigger('click');
                break;
            }
        }

        $.each(needShowDDLids, function (i, ddlId) {
            $('#' + ddlId).change();
        });
    });


    $('#span_CloseNoticeInfo').bind('click', function () {
        $('#div_NoticeInputInfo').css('display', 'none');
    })

    $("input[type=radio]:checked,select").change();
    $('#span_CloseNoticeInfo').click();
}

//给出文本框输入提示信息
function ShowTbNoticeInfo() {
    var flowData = JSON.parse(jsonStr);
    var mapAttr = frmData.Sys_MapAttr;
    mapAttr = $.grep(mapAttr, function (attr) {
        var atParams = attr.AtPara;
        return atParams != undefined && AtParaToJson(atParams).Tip != undefined && AtParaToJson(atParams).Tip != '' && $('#TB_' + attr.KeyOfEn).length > 0 && $('#TB_' + attr.KeyOfEn).css('display') != 'none';
    })

    $.each(mapAttr, function (i, attr) {
        $('#TB_' + attr.KeyOfEn).bind('focus', function (obj) {

            var mapAttr = frmData.Sys_MapAttr;

            mapAttr = $.grep(mapAttr, function (attr) {
                return 'TB_' + attr.KeyOfEn == obj.target.id;
            })
            var atParams = AtParaToJson(mapAttr[0].AtPara);
            var noticeInfo = atParams.Tip;

            if (noticeInfo == undefined || noticeInfo == '')
                return;

            //noticeInfo = noticeInfo.replace(/\\n/g, '<br/>')

            $($('#div_NoticeInputInfo .popover-title span')[0]).text(mapAttr[0].Name);
            $('#div_NoticeInputInfo .popover-content').html(noticeInfo);

            var top = obj.target.offsetHeight;
            var left = obj.target.offsetLeft;
            var current = obj.target.offsetParent;
            while (current !== null) {
                left += current.offsetLeft;
                top += current.offsetTop;
                current = current.offsetParent;
            }

            if (top - $('#div_NoticeInputInfo').height() - 30 < 0) {
                //让提示框在下方展示
                $('#div_NoticeInputInfo').removeClass('top');
                $('#div_NoticeInputInfo').addClass('bottom');
                top = top;
            } else {
                $('#div_NoticeInputInfo').removeClass('bottom');
                $('#div_NoticeInputInfo').addClass('top');
                top = top - $('#div_NoticeInputInfo').height() - 30;
            }
            $('#div_NoticeInputInfo').css('top', top);
            $('#div_NoticeInputInfo').css('left', left);
            $('#div_NoticeInputInfo').css('display', 'block');
        });
    })
}


//必填项检查   名称最后是*号的必填  如果是选择框，值为'' 或者 显示值为 【*请选择】都算为未填 返回FALSE 检查必填项失败
function checkBlanks() {
    var checkBlankResult = true;
    //获取所有的列名 找到带* 的LABEL mustInput

    var lbs = $('.mustInput');
    $.each(lbs, function (i, obj) {
        if ($(obj).parent().css('display') != 'none') {

            var ele = $(obj).siblings("div").children();
            if (ele.length == 1) {
                switch (ele[0].tagName.toUpperCase()) {
                    case "INPUT":
                        if (ele.attr('type') == "text") {
                            if (ele.val() == "") {
                                checkBlankResult = false;
                                ele.addClass('errorInput');
                            } else {
                                ele.removeClass('errorInput');
                            }
                        }
                        break;
                    case "SELECT":
                        if (ele.val() == "" || ele.children('option:checked').text() == "*请选择") {
                            checkBlankResult = false;
                            ele.addClass('errorInput');
                        } else {
                            ele.removeClass('errorInput');
                        }
                        break;
                    case "TEXTAREA":
                        if (ele.val() == "") {
                            checkBlankResult = false;
                            ele.addClass('errorInput');
                        } else {
                            ele.removeClass('errorInput');
                        }
                        break;
                }
            }
        }
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
            if ($(obj).val() != undefined && $(obj).val() != '') {

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

function SaveDtlAll() {
    return true;
}


//初始化附件内容.
function InitAth(frmData, gf) {

    var ath = flowData.Sys_FrmAttachment[0];
    if (ath == null)
        return "没有找到附件定义，请与管理员联系。";

    var athDBs = new Entities("BP.Sys.FrmAttachmentDBs");
    athDBs.Retrieve("RefPKVal", this.GetQueryString("WorkID"));
    //if (athDBs == undefined || athDBs.length == 0)
    //   return "无";
    var url = "";
    var workID = GetQueryString("WorkID");
    var FK_Flow = GetQueryString("FK_Flow");
    var FK_Node = GetQueryString("FK_Node");
    url += "&WorkID=" + workID;
    url += "&FK_Node=" + FK_Node;
    url += "&FK_Flow=" + FK_Flow;
    url += "&IsReadonly=" + pageData.IsRead


    var html = "";

    src = "./CCForm/Ath.htm?PKVal=" + pageData.WorkID + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + ath.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK + url + "&M=" + Math.random();
    html += "<iframe style='width:100%;height:" + ath.H + "px;' ID='Attach_" + ath.MyPK + "'    src='" + src + "' frameborder=0  marginheight='0' marginwidth='0'  scrolling=no></iframe>" + '</div>';
    //    }

    return html;
}
var flowData = null;



var frmData = {};

//将#FF000000 转换成 #FF0000
function TranColorToHtmlColor(color) {
    if (color != undefined && color.indexOf('#') == 0 && color.length == 9) {
        color = color.substring(0, 7);
    }
    return color;
}

//FontStyle, FontWeight, IsBold, IsItalic
//fontStyle font-size:19;font-family:"Portable User Interface";font-weight:bolder;color:#FF0051; 为H5设计的，不用解析后面3个
function analysisFontStyle(ele, fontStyle, isBold, isItalic) {
    if (fontStyle != undefined && fontStyle.indexOf(':') > 0) {
        var fontStyleArr = fontStyle.split(';');
        $.each(fontStyleArr, function (i, fontStyleObj) {
            ele.css(fontStyleObj.split(':')[0], TranColorToHtmlColor(fontStyleObj.split(':')[1]));
        });
    }
    else {
        if (isBold == 1) {
            ele.css('font-weight', 'bold');
        }
        if (isItalic == 1) {
            ele.css('font-style', 'italic')
        }
    }
}

//明细表
function InitDtl(frmData,gf){
	var dtlHtml = "";
	if(frmData.Sys_MapDtl){
		$.each(frmData.Sys_MapDtl, function (i, dtl) {
            if (gf.CtrlID == dtl.No) {
                if (dtl.MobileShowModel == undefined || dtl.MobileShowModel == 0) {
                    var func = "Dtl_ShowPage(\"" + dtl.No + "\",\"" + dtl.Name + "\")";
                    dtlHtml += "<div class='mui-table-view-cell'>";
                    dtlHtml += "	<a class='mui-navigate-right' data-title-type='native' href='javascript:" + func + "'><h5>" + dtl.Name
                        + "<span id='" + dtl.No + "_Count'></span></h5>";
                    dtlHtml += "		<p>点击查看详细</p>";
                    dtlHtml += "	</a>";
                    dtlHtml += "</div>";
                    return;
                }
                //列表模式展示
                if (dtl.MobileShowModel == 1) {
                    dtlHtml = GetDtlList(dtl.No);
                }
			}
	    });	
	}
	return dtlHtml;
}

//打开明细表
function Dtl_ShowPage(dtlNo,dtlName){
	$("#frmDtlTitle").html(dtlName);
	$("#HD_CurDtl_No").val(dtlNo)
	Load_DtlInit();
	viewApi.go("#frmDtl");
}

var dtlExt = {};
//获取从表列表
function GetDtlList(dtlNo) {
    //获得mapdtl实体的基本信息.
    var hand = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    hand.AddPara("EnsName", dtlNo);
    hand.AddPara("RefPKVal", pageData.WorkID);
    hand.AddPara("FK_Node", pageData.FK_Node);
    hand.AddPara("IsReadonly", pageData.IsReadonly);
    mainData = hand.DoMethodReturnJSON("Dtl_Init");
    //主表数据，用于变量替换.
    var mainTable = mainData["MainTable"]; //主表数据.
    //从表信息.
    sys_MapDtl = mainData["Sys_MapDtl"][0]; //从表描述.
    sys_mapAttr = JSON.stringify(mainData["Sys_MapAttr"]); //从表字段.
    var sys_mapExtDtl = mainData["Sys_MapExt"]; //扩展信息.
    var dbDtl = mainData["DBDtl"]; //从表数据.

    if (!$.isArray(dtlExt[dtlNo])) {
        dtlExt[dtlNo] = [];
    }
    dtlExt[dtlNo].push(mainData);
    var _Html = '<div class="mui-card" style="margin-bottom: 35px;">';
    _Html += '<ul class="mui-table-view">';
    for (var i = 0; i < dbDtl.length; i++) {
        _Html += '<li class="mui-table-view-cell mui-media">';
        _Html += '<a href="javascript:;">';
      
        _Html += "<button type='button' class='mui-btn mui-btn-success mui-btn-outlined' onclick='Dtl_InitPage(0,\"" + dtlNo + "\"," + dbDtl[i].OID + ")'>";
        _Html += '查看';
        _Html += '<span class="mui-iconmui-icon-search"></span>';
        _Html += '</button>';
       
        _Html += '<div class="mui-media-body">';
        _Html += dbDtl[i][sys_MapDtl.MobileShowField];
        _Html += ' </div>';
        _Html += '</a>';
        _Html += '</li>';
    }
    _Html += '</ul>';
    _Html += '</div>';


    return _Html;
}

function ImgAth(url, athMyPK) {
    var v = window.showModalDialog(url, 'ddf', 'dialogHeight: 650px; dialogWidth: 950px;center: yes; help: no');
    if (v == null)
        return;
    document.getElementById('Img' + athMyPK).setAttribute('src', v);
}

//初始化 IMAGE附件
function figure_Template_ImageAth(frmImageAth) {
    var isEdit = frmImageAth.IsEdit;
    var eleHtml = $("<div></div>");
    var img = $("<img/>");

    var imgSrc = "/WF/Data/Img/LogH.PNG";
    //获取数据
    if (frmData.Sys_FrmImgAthDB) {
        $.each(frmData.Sys_FrmImgAthDB, function (i, obj) {
            if (obj.FK_FrmImgAth == frmImageAth.MyPK) {
                imgSrc = obj.FileFullName;
            }
        });
    }
    //设计属性
    img.attr('id', 'Img' + frmImageAth.MyPK).attr('name', 'Img' + frmImageAth.MyPK);
    img.attr("src", imgSrc).attr('onerror', "this.src='/WF/Data/Img/LogH.PNG'");
    img.css('width', frmImageAth.W).css('height', frmImageAth.H).css('padding', "0px").css('margin', "0px").css('border-width', "0px");
    //不可编辑
    if (isEdit == "1") {
        var fieldSet = $("<fieldset></fieldset>");
        var length = $("<legend></legend>");
        var a = $("<a></a>");
        var url = "/WF/CCForm/ImgAth.aspx?W=" + frmImageAth.W + "&H=" + frmImageAth.H + "&FK_MapData=ND" + pageData.FK_Node + "&MyPK=" + pageData.WorkID + "&ImgAth=" + frmImageAth.MyPK;

        a.attr('href', "javascript:ImgAth('" + url + "','" + frmImageAth.MyPK + "');").html("编辑");
        length.css('font-style', 'inherit').css('font-weight', 'bold').css('font-size', '12px');

        fieldSet.append(length);
        length.append(a);
        fieldSet.append(img);
        eleHtml.append(fieldSet);
    } else {
        eleHtml.append(img);
    }
    eleHtml.css('position', 'absolute').css('top', frmImageAth.Y).css('left', frmImageAth.X);
    return eleHtml;
}

function addLoadFunction(id, eventName, method) {
    var js = "";
    js = "<script type='text/javascript' >";
    js += "function F" + id + "load() { ";
    js += "if (document.all) {";
    js += "document.getElementById('F" + id + "').attachEvent('on" + eventName + "',function(event){" + method + "('" + id + "');});";
    js += "} ";

    js += "else { ";
    js += "document.getElementById('F" + id + "').contentWindow.addEventListener('" + eventName + "',function(event){" + method + "('" + id + "');}, false); ";
    js += "} }";

    js += "</script>";
    return $(js);
}

//初始化轨迹图
function figure_Template_FigureFlowChart(wf_node) {

    //轨迹图
    var sta = wf_node.FrmTrackSta;
    var x = wf_node.FrmTrack_X;
    var y = wf_node.FrmTrack_Y;
    var h = wf_node.FrmTrack_H;
    var w = wf_node.FrmTrack_W;

    if (sta == 0) {
        return $('');
    }

    if (sta == undefined) {
        return;
    }

    var src = "./WorkOpt/OneWork/OneWork.htm?CurrTab=Track";
    src += '&FK_Flow=' + pageData.FK_Flow;
    src += '&FK_Node=' + pageData.FK_Node;
    src += '&WorkID=' + pageData.WorkID;
    src += '&FID=' + pageData.FID;
    var eleHtml = '<div id="divtrack' + wf_node.NodeID + '">' + "<iframe id='track" + wf_node.NodeID + "' style='width:" + w + "px;height=" + h + "px;'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    eleHtml = $(eleHtml);
    eleHtml.css('position', 'absolute').css('top', y).css('left', x).css('width', w).css('height', h);

    return eleHtml;
}

//审核组件
function figure_Template_FigureFrmCheck(wf_node) {
    //审核组键FWCSta Sta,FWC_X X,FWC_Y Y,FWC_H H, FWC_W W from WF_Node
    var sta = wf_node.FWCSta;
    var x = wf_node.FWC_X;
    var y = wf_node.FWC_Y;

    if (sta == 0)
        return $('');

    var src = "./WorkOpt/WorkCheck.htm?s=2";
    var fwcOnload = "";
    var paras = '';

    paras += "&FID=" + pageData["FID"];
    paras += "&OID=" + pageData["WorkID"];
    paras += '&FK_Flow=' + pageData.FK_Flow;
    paras += '&FK_Node=' + pageData.FK_Node;
    paras += '&WorkID=' + pageData.WorkID;
    paras += '&IsReadonly=' + pageData.IsReadonly;
    paras += '&IsMobile=1';

    if (sta == 2)//只读
    {
        src += "&DoType=View";
    }
    else {
        fwcOnload = "onload= 'WC" + wf_node.NodeID + "load();'";
        $('body').append(addLoadFunction("WC" + wf_node.NodeID, "blur", "SaveDtl"));
    }
    src += "&r=q" + paras;

    //暂时修改高度为500px.

    var eleHtml = '<div  id="FFWC' + wf_node.NodeID + '" >' + "<div style='padding: 2px; width: 100%;'><table id='tbTracks'  style='border:1px solid #d6dde6;font-size:14px;padding: 0px; width: 100%;'></table></div>" + '</div>';
    eleHtml = $(eleHtml);


    return eleHtml;
}

//子流程
function figure_Template_FigureSubFlowDtl(wf_node) {
    var sta = wf_node.SFSta;
    var h = wf_node.SF_H;
    if (sta == 0)
        return $('');

    var src = "./WorkOpt/SubFlow.htm?s=2";
    var fwcOnload = "";
    var paras = '';

    paras += "&FID=" + pageData["FID"];
    paras += "&OID=" + pageData["WorkID"];
    paras += '&FK_Flow=' + pageData.FK_Flow;
    paras += '&FK_Node=' + pageData.FK_Node;
    paras += '&WorkID=' + pageData.WorkID;
    if (sta == 2)//只读
    {
        src += "&DoType=View";
    }
    else {
        fwcOnload = "onload= 'WC" + wf_node.NodeID + "load();'";
        $('body').append(addLoadFunction("WC" + wf_node.NodeID, "blur", "SaveDtl"));
    }
    src += "&r=q" + paras;
    var eleHtml = '<div id=DIVWC' + wf_node.NodeID + '>' + "<iframe id=FSF" + wf_node.NodeID + " style='width:100%;height:" + h + "px'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    eleHtml = $(eleHtml);

    return eleHtml;
}

//初始化框架
function figure_Template_IFrame(fram) {
    var eleHtml = '';
    var src = dealWithUrl(fram.src) + "IsReadOnly=0";
    eleHtml = $('<div id="iframe' + fram.MyPK + '">' + '</div>');
    var iframe = $(+"<iframe  style='width:" + fram.W + "px; height:" + fram.H + "'     src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling='no'></iframe>");
    eleHtml.append(iframe);
    return eleHtml;
}

//处理URL，MainTable URL 参数 替换问题
function dealWithUrl(src) {
    var src = fram.URL.replace(new RegExp(/(：)/g), ':');
    var params = '&FID=' + pageData.FID;
    params += '&WorkID=' + pageData.WorkID;
    if (src.indexOf("?") > 0) {
        var params = getQueryStringFromUrl(src);
        if (params != null && params.length > 0) {
            $.each(params, function (i, param) {
                if (param.indexOf('@') == 0) {//是需要替换的参数
                    paramArr = param.split('=');
                    if (paramArr.length == 2 && paramArr[1].indexOf('@') == 0) {
                        if (paramArr[1].indexOf('@WebUser.') == 0) {
                            params[i] = paramArr[0].substring(1) + "=" + frmData.MainTable[0][paramArr[1].substr('@WebUser.'.length)];
                        }
                        if (frmData.MainTable[0][paramArr[1].substr(1)] != undefined) {
                            params[i] = paramArr[0].substring(1) + "=" + frmData.MainTable[0][paramArr[1].substr(1)];
                        }

                        //使用URL中的参数
                        var pageParams = getQueryString();
                        var pageParamObj = {};
                        $.each(pageParams, function (i, pageParam) {
                            if (pageParam.indexOf('@') == 0) {
                                var pageParamArr = pageParam.split('=');
                                pageParamObj[pageParamArr[0].substring(1, pageParamArr[0].length)] = pageParamArr[1];
                            }
                        });
                        var result = "";
                        //通过MAINTABLE返回的参数
                        for (var ele in frmData.MainTable[0]) {
                            if (paramArr[0].substring(1) == ele) {
                                result = frmData.MainTable[0][ele];
                                break;
                            }
                        }
                        //通过URL参数传过来的参数
                        for (var pageParam in pageParamObj) {
                            if (pageParam == paramArr[0].substring(1)) {
                                result = pageParamObj[pageParam];
                                break;
                            }
                        }

                        if (result != '') {
                            params[i] = paramArr[0].substring(1) + "=" + unescape(result);
                        }
                    }
                }
            });
            src = src.substr(0, src.indexOf('?')) + "?" + params.join('&');
        }
    }
    else {
        src += "?q=1";
    }
    return src;
}

function figure_Template_MsgAlert(msgAlert, i) {
    var eleHtml = $('<div></div>');
    var titleSpan = $('<span class="titleAlertSpan"> ' + (parseInt(i) + 1) + "&nbsp;&nbsp;&nbsp;" + msgAlert.Title + '</span>');
    var msgDiv = $('<div>' + msgAlert.Msg + '</div>');
    eleHtml.append(titleSpan).append(msgDiv);
    return eleHtml;
}

//处理URL，MainTable URL 参数 替换问题
function dealWithUrl(src) {
    var src = fram.URL.replace(new RegExp(/(：)/g), ':');
    var params = '&FID=' + pageData.FID;
    params += '&WorkID=' + pageData.WorkID;
    if (src.indexOf("?") > 0) {
        var params = getQueryStringFromUrl(src);
        if (params != null && params.length > 0) {
            $.each(params, function (i, param) {
                if (param.indexOf('@') == 0) {//是需要替换的参数
                    paramArr = param.split('=');
                    if (paramArr.length == 2 && paramArr[1].indexOf('@') == 0) {
                        if (paramArr[1].indexOf('@WebUser.') == 0) {
                            params[i] = paramArr[0].substring(1) + "=" + frmData.MainTable[0][paramArr[1].substr('@WebUser.'.length)];
                        }
                        if (frmData.MainTable[0][paramArr[1].substr(1)] != undefined) {
                            params[i] = paramArr[0].substring(1) + "=" + frmData.MainTable[0][paramArr[1].substr(1)];
                        }

                        //使用URL中的参数
                        var pageParams = getQueryString();
                        var pageParamObj = {};
                        $.each(pageParams, function (i, pageParam) {
                            if (pageParam.indexOf('@') == 0) {
                                var pageParamArr = pageParam.split('=');
                                pageParamObj[pageParamArr[0].substring(1, pageParamArr[0].length)] = pageParamArr[1];
                            }
                        });
                        var result = "";
                        //通过MAINTABLE返回的参数
                        for (var ele in frmData.MainTable[0]) {
                            if (paramArr[0].substring(1) == ele) {
                                result = frmData.MainTable[0][ele];
                                break;
                            }
                        }
                        //通过URL参数传过来的参数
                        for (var pageParam in pageParamObj) {
                            if (pageParam == paramArr[0].substring(1)) {
                                result = pageParamObj[pageParam];
                                break;
                            }
                        }

                        if (result != '') {
                            params[i] = paramArr[0].substring(1) + "=" + unescape(result);
                        }
                    }
                }
            });
            src = src.substr(0, src.indexOf('?')) + "?" + params.join('&');
        }
    }
    else {
        src += "?q=1";
    }
    return src;
}
var colVisibleJsonStr = ''
var jsonStr = '';
var frmData = {};



function BackToHome() {
    SetHref('../CCMobilePortal/Home.htm?UserNo=' + GetQueryString('UserNo') + "&Token=" + GetQueryString("Token"));
}

function BackToTodolist() {
    SetHref('Todolist.htm?UserNo=' + GetQueryString('UserNo') + "&Token=" + GetQueryString("Token"));
}

function BackToStart() {
    SetHref('Start.htm?UserNo=' + GetQueryString('UserNo') + "&Token=" + GetQueryString("Token"));
}

//@浙商银行
function SetFrmReadonly() {
    $('#CCForm').find('input,textarea,select').attr('disabled', false);
    $('#CCForm').find('input,textarea,select').attr('readonly', true);
    $('#CCForm').find('input,textarea,select').attr('disabled', true); 
    $('#Btn_Save').attr('disabled', true);
}