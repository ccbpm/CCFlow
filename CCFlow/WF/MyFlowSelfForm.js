
$(function () {

    //打开表单检查正则表达式
    if (typeof FormOnLoadCheckIsNull != 'undefined' && FormOnLoadCheckIsNull instanceof Function) {
        FormOnLoadCheckIsNull();
    }

});

//. 保存嵌入式表单. add 2015-01-22 for GaoLing.
function SaveSelfFrom() {

    // 不支持火狐浏览器。
    var frm = document.getElementById('SelfForm');
    if (frm == null) {
        alert('系统错误,没有找到SelfForm的ID.');
    }

    //审核组件
    if ($("#WorkCheck_Doc").length == 1) {
        //保存审核信息
        SaveWorkCheck();
    }

    //执行保存.
    var frmSrc = document.getElementById('SelfForm').src;
    
    //嵌入的表单和当前页面在同一个域
    if (frmSrc.indexOf(basePath + "/") != -1)
        return frm.contentWindow.Save();

    //出现跨域问题
    /**
     嵌入的页面需要增加的方法
     window.addEventListener('message', function (e) {
           console.log(e.data);
           var data=e.data;
           if(data.Save){
               Save();
           }
       }, false);
     */
    return frm.contentWindow.postMessage({ Save:"Save" }, "*");
   
}






//20160106 by 柳辉
//获取页面参数
//sArgName表示要获取哪个参数的值
function GetPageParas(sArgName) {

    var sHref = window.location.href;
    var args = sHref.split("?");
    var retval = "";
    if (args[0] == sHref) /*参数为空*/{
        return retval; /*无需做任何处理*/
    }
    var str = args[1];
    args = str.split("&");
    for (var i = 0; i < args.length; i++) {
        str = args[i];
        var arg = str.split("=");
        if (arg.length <= 1) continue;
        if (arg[0] == sArgName) retval = arg[1];
    }
    return retval;
}


function To(url) {
    //window.location.href = url;
    window.name = "dialogPage"; window.open(url, "dialogPage")
}

function WinOpen(url, winName) {
    var newWindow = window.open(url, winName, 'width=700,height=400,top=100,left=300,scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes;');
    newWindow.focus();
    return;
}


function Do(warning, url) {
    if (window.confirm(warning) == false)
        return;
    window.location.href = url;
}

window.onload = function () {
    //  ResizeWindow();
    setToobarUnVisible();
};

//然浏览器最大化.
function ResizeWindow() {
    if (window.screen) {  //判断浏览器是否支持window.screen判断浏览器是否支持screen     
        var myw = screen.availWidth;   //定义一个myw，接受到当前全屏的宽     
        var myh = screen.availHeight;  //定义一个myw，接受到当前全屏的高     
        window.moveTo(0, 0);           //把window放在左上角     
        window.resizeTo(myw, myh);     //把当前窗体的长宽跳转为myw和myh     
    }
}

function OpenCC() {
    var url = $("#CC_Url").val();
    var v = window.showModalDialog(url, 'cc', 'scrollbars=yes;resizable=yes;center=yes;minimize:yes;maximize:yes;dialogHeight: 650px; dialogWidth: 850px; dialogTop: 100px; dialogLeft: 150px;');
    if (v == '1')
        return true;
    return false;
}



//以下是软通写的
//初始化网页URL参数
function initPageParam() {
    pageData.FK_Flow = GetQueryString("FK_Flow");
    pageData.FK_Node = GetQueryString("FK_Node");
    pageData.FID = GetQueryString("FID") == null ? 0 : GetQueryString("FID");
    pageData.WorkID = GetQueryString("WorkID");
    pageData.Paras = GetQueryString("Paras");
    pageData.IsReadonly = GetQueryString("IsReadonly"); //如果是IsReadonly，就表示是查看页面，不是处理页面
    pageData.IsStartFlow = GetQueryString("IsStartFlow"); //是否是启动流程页面 即发起流程

    pageData.DoType1 = GetQueryString("DoType")//View
}

//将获取过来的URL参数转成URL中的参数形式  &
function pageParamToUrl() {
    var paramUrlStr = '';
    for (var param in pageData) {

        var val = pageData[param];
        if (val == null || val == undefined)
            continue;

        paramUrlStr += '&' + (param.indexOf('@') == 0 ? param.substring(1) : param) + '=' + pageData[param];
    }
    return paramUrlStr;
}

//设置附件为只读
function setAttachDisabled() {
    //附件设置
    var attachs = $('iframe[src*="Ath.htm"]');
    $.each(attachs, function (i, attach) {
        if (attach.src.indexOf('IsReadonly') == -1) {
            $(attach).attr('src', $(attach).attr('src') + "&IsReadonly=1");
        }
    })
}
//隐藏下方的功能按钮
function setToobarUnVisible() {
    //隐藏下方的功能按钮
    $('#bottomToolBar').css('display', 'none');
}


//保存
function Save() {
    SaveSelfFrom();
}

//退回工作
function returnWorkWindowClose(data) {

    if (data == "" || data == "取消") {
        $('#returnWorkModal').modal('hide');
        return;
    }

    $('#returnWorkModal').modal('hide');
    //通过下发送按钮旁的下拉框选择下一个节点
    if (data.indexOf('SaveOK@') == 0) {
        //说明保存人员成功,开始调用发送按钮.
        var toNode = 0;
        //含有发送节点 且接收
        if ($('#DDL_ToNode').length > 0) {
            var selectToNode = $('#DDL_ToNode  option:selected').data();
            toNode = selectToNode.No;
        }

        execSend(toNode);
        //$('[name=Send]:visible').click();
        return;
    } else {//可以重新打开接收人窗口
        winSelectAccepter = null;
    }

    if (data.indexOf('err@') == 0 || data == "取消") {//发送时发生错误
        $('#Message').html(data);
        $('#MessageDiv').modal().show();
    }
    else {
        OptSuc(data);
    }
}


//回填扩展字段的值
function SetAth(data) {
    var atParamObj = $('#iframeAthForm').data();
    var tbId = atParamObj.tbId;
    var divId = atParamObj.divId;
    var athTb = $('#' + tbId);
    var athDiv = $('#' + divId);

    $('#athModal').modal('hide');
    //不存在或来自于viewWorkNodeFrm
    if (atParamObj != undefined && atParamObj.IsViewWorkNode != 1 && divId != undefined && tbId != undefined) {
        if (atParamObj.AthShowModel == "1") {
            athTb.val(data.join('*'));
            athDiv.html(data.join(';&nbsp;'));
        } else {
            athTb.val('@AthCount=' + data.length);
            athDiv.html("附件<span class='badge' >" + data.length + "</span>个");
        }
    } else {
        $('#athModal').removeClass('in');
    }
    $('#athModal').hide();
    var ifs = $("iframe[id^=track]").contents();
    if (ifs.length > 0) {
        for (var i = 0; i < ifs.length; i++) {
            $(ifs[i]).find(".modal-backdrop").hide();
        }
    }
}

//查看页面的附件展示  查看页面调用
function ShowViewNodeAth(athLab, atParamObj, src) {
    var athForm = $('iframeAthForm');
    var athModal = $('athModal');
    var athFormTitle = $('#athModal .modal-title');
    athFormTitle.text("上传附件：" + athLab);
    athModal.modal().show();
}


//初始化下拉列表框的OPERATION
function InitDDLOperation(workNodeData, mapAttr, defVal) {
    var operations = '';
    //外键类型
    if (mapAttr.LGType == 2) {
        if (workNodeData[mapAttr.KeyOfEn] != undefined) {
            $.each(workNodeData[mapAttr.KeyOfEn], function (i, obj) {
                operations += "<option " + (obj.No == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";
            });
        }
        else if (workNodeData[mapAttr.UIBindKey] != undefined) {
            $.each(workNodeData[mapAttr.UIBindKey], function (i, obj) {
                operations += "<option " + (obj.No == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";
            });
        }
    } else {
        var enums = workNodeData.Sys_Enum;
        enums = $.grep(enums, function (value) {
            return value.EnumKey == mapAttr.UIBindKey;
        });


        $.each(enums, function (i, obj) {
            operations += "<option " + (obj.IntKey == defVal ? " selected='selected' " : "") + " value='" + obj.IntKey + "'>" + obj.Lab + "</option>";
        });

    }
    return operations;
}


$(function () {

    $('#btnMsgModalOK').bind('click', function () {
        if (window.opener) {

            if (window.opener.name && window.opener.name == "main") {
                window.opener.location.href = window.opener.location.href;
                if (window.opener.top && window.opener.top.leftFrame) {
                    window.opener.top.leftFrame.location.href = window.opener.top.leftFrame.location.href;
                }
            } else if (window.opener.name && window.opener.name == "运行流程") {
                //测试运行流程，不进行刷新
            } else {
                //window.opener.location.href = window.opener.location.href;
            }
        }
        window.close();
    });

   
    $('#btnMsgModalOK1').bind('click', function () {
        window.close();
        opener.window.focus();
    });

})




var flowData;

//将v1版本表单元素转换为v2 杨玉慧  silverlight 自由表单转化为H5表单
function GenerWorkNode() {

    var href = window.location.href;
    var urlParam = href.substring(href.indexOf('?') + 1, href.length);
    urlParam = urlParam.replace('&DoType=', '&DoTypeDel=xx');

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddUrlData(urlParam);
    var data = handler.DoMethodReturnString("MyFlowSelfForm_Init");

    if (data.indexOf('err@') == 0) {
        alert(data);
        return;
    }

    jsonStr = data;
   

    try {

        flowData = JSON.parse(data);
        workNodeData = flowData;

    } catch (err) {
        alert("GenerWorkNode转换JSON失败:" + jsonStr);
        return;
    }

    //设置标题.
    document.title = flowData.WF_Node[0].Name;


    $('#CCForm').html('');

    var mapData = workNodeData.Sys_MapData[0];
    var wf_node = workNodeData.WF_Node[0];
    var frmName = mapData.Name;

    var url = wf_node.FormUrl;
    if (url == "")
        url = "../DataUser/DefaultSelfFormUrl.htm";
    else
      if(url.indexOf("http")==-1)
         url = basePath + url;

    if (url.indexOf('?') == -1) {
        url = url + "?1=2";
    }
    url += "&WorkID=" + GetPageParas("WorkID") + "&FK_Flow=" + GetPageParas("FK_Flow") + "&FK_Node=" + GetPageParas("FK_Node");

    var html = "<iframe ID='SelfForm' src='" + url + "' frameborder=0  style='width:100%;' leftMargin='0' topMargin='0'/>";

    var compoents = workNodeData.WF_FrmNodeComponent;
    //增加审核分组
    for (var i = 0; i < compoents.length; i++) {
        var component = compoents[i];
        if (component.FWCSta != 0) {
            if (wf_node.FormType == 10 && gf.FrmID != 'ND' + wf_node.NodeID)
                continue;
            html += Ele_FrmCheck(wf_node);
            continue;
        }
    }

    $('#CCForm').html("").append(html);

    //循环之前的提示信息.
    var info = "";
    for (var i in flowData.AlertMsg) {
        var alertMsg = flowData.AlertMsg[i];
        var alertMsgEle = figure_Template_MsgAlert(alertMsg, i);
        $('#Message').append(alertMsgEle);
        $('#Message').append($('<hr/>'));
    }

    if (flowData.AlertMsg.length != 0) {
        $('#MessageDiv').modal().show();
    }



    //初始化Sys_MapData
    var h = flowData.Sys_MapData[0].FrmH;
    var w = flowData.Sys_MapData[0].FrmW;

    $('#topContentDiv').width(w);
    $('.Bar').width(w + 15);
    $('#lastOptMsg').width(w + 15);

    var marginLeft = $('#topContentDiv').css('margin-left');
    marginLeft = marginLeft.replace('px', '');

    marginLeft = parseFloat(marginLeft.substr(0, marginLeft.length - 2)) + 50;
    $('#topContentDiv i').css('left', marginLeft.toString() + 'px');
    //原有的

   // InitToNodeDDL(flowData);

    //Common.MaxLengthError();




    //增加审核组件附件上传的功能
    if ($("#uploaddiv").length > 0) {
        var explorer = window.navigator.userAgent;
        if (((explorer.indexOf('MSIE') >= 0) && (explorer.indexOf('Opera') < 0) || (explorer.indexOf('Trident') >= 0)))
            AddUploadify("uploaddiv", $("#uploaddiv").attr("data-info"));
        else
            AddUploafFileHtm("uploaddiv", $("#uploaddiv").attr("data-info"));
    }

}


function setIframeHeight(iframe) {
    if (iframe) {
        var iframeWin = iframe.contentWindow || iframe.contentDocument.parentWindow;
        if (iframeWin.document.body) {
            iframe.height = iframeWin.document.documentElement.scrollHeight || iframeWin.document.body.scrollHeight;
        }
    }
};

window.onload = function () {
    setIframeHeight(document.getElementById('SelfForm'));
};

function Ele_FrmCheck(wf_node) {
    if (wf_node.FWCSta != 0) {
        if (wf_node.FWCVer == 0 || wf_node.FWCVer == "" || wf_node.FWCVer == undefined)
            pageData.FWCVer = 0;
        else
            pageData.FWCVer = 1;
        
    }

    return "<div id='WorkCheck'></div>";

}


function figure_Template_MsgAlert(msgAlert, i) {
    var eleHtml = $('<div></div>');
    var titleSpan = $('<span class="titleAlertSpan"> ' + (parseInt(i) + 1) + "&nbsp;&nbsp;&nbsp;" + msgAlert.Title + '</span>');
    var msgDiv = $('<div>' + msgAlert.Msg + '</div>');
    eleHtml.append(titleSpan).append(msgDiv)
    return eleHtml;
}

var workNodeData = {};



var colVisibleJsonStr = ''
var jsonStr = '';

var pageData = {};
var globalVarList = {};

$(function () {

    var frm = document.forms["divCCForm"];

    initPageParam(); //初始化参数

    //InitToolBar(); //工具栏.ajax
    

    GenerWorkNode(); //表单数据.ajax

    if ($("#Message").html() == "") {
        $(".Message").hide();
    }

    if (parent != null && parent.document.getElementById('MainFrames') != undefined) {
        //计算高度，展示滚动条
        var height = $(parent.document.getElementById('MainFrames')).height() - 110;
        //$('#topContentDiv').height(height);

        $(window).resize(function () {
            $("#CCForm").height($(window).height() - 115 + "px").css("overflow-y", "auto").css("scrollbar-face-color", "#fff"); ;
        });
    }
    else {//新加
        //计算高度，展示滚动条
        var height = $("#CCForm").height($(window).height() - 115 + "px").css("overflow-y", "auto").css("scrollbar-face-color", "#fff");
        // $('#topContentDiv').height(height);

        $(window).resize(function () {
            $("#CCForm").height($(window).height() - 115 + "px").css("overflow-y", "auto").css("scrollbar-face-color", "#fff"); ;
        });
    }

    $('#btnCloseMsg').bind('click', function () {
        $('.Message').hide();
    });
})