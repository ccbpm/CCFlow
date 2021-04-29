$(function () {
    SetHegiht();
    //打开表单检查正则表达式
    if (typeof FormOnLoadCheckIsNull != 'undefined' && FormOnLoadCheckIsNull instanceof Function) {
        FormOnLoadCheckIsNull();
    }

});
   

function SetHegiht() {

    var screenHeight = document.documentElement.clientHeight;

    var messageHeight = $('#Message').height();
    var topBarHeight = 40;
    var childHeight = $('#childThread').height();
    var infoHeight = $('#flowInfo').height();

    var allHeight = messageHeight + topBarHeight + childHeight + childHeight + infoHeight;
    try {

        var BtnWord = $("#BtnWord").val();
        if (BtnWord == 2)
            allHeight = allHeight + 30;

        var frmHeight = $("#FrmHeight").val();
        if (frmHeight == NaN || frmHeight == "" || frmHeight == null)
            frmHeight = 0;

        if (screenHeight > parseFloat(frmHeight) + allHeight) {
            // $("#divCCForm").height(screenHeight - allHeight);

            $("#TDWorkPlace").height(screenHeight - allHeight - 10);

        }
        else {
            //$("#divCCForm").height(parseFloat(frmHeight) + allHeight);
            $("#TDWorkPlace").height(parseFloat(frmHeight) + allHeight - 10);
        }
    }
    catch (e) {
    }
}

$(window).resize(function () {
    //SetHegiht();
});
 
function Change() {
    var btn = document.getElementById('Btn_Save');
    if (btn != null) {
        if (btn.value.valueOf('*') == -1)
            btn.value = btn.value + '*';
    }
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

//执行分支流程退回到分合流节点。
function DoSubFlowReturn(fid, workid, fk_node) {
    var url = 'ReturnWorkSubFlowToFHL.htm?FID=' + fid + '&WorkID=' + workid + '&FK_Node=' + fk_node;
    var v = WinShowModalDialog(url, 'df');
    window.location.href = window.history.url;
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

function DoDelSubFlow(fk_flow, workid) {
    if (window.confirm('您确定要终止进程吗？') == false)
        return;

    var para = 'DoType=DelSubFlow&FK_Flow=' + fk_flow + '&WorkID=' + workid;

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    
    handler.AddPara("FK_Flow", fk_flow);
    handler.AddPara("WorkID", workid);
    var data = handler.DoMethodReturnString("DelSubFlow"); 
    alert(data);
    window.location.href = window.location.href;

}


function Do(warning, url) {
    if (window.confirm(warning) == false)
        return;
    window.location.href = url;
}
  
//然浏览器最大化.
function ResizeWindow() {
    if (window.screen) {  //判断浏览器是否支持window.screen判断浏览器是否支持screen     
        var myw = screen.availWidth;   //定义一个myw，接受到当前全屏的宽     
        var myh = screen.availHeight;  //定义一个myw，接受到当前全屏的高     
        window.moveTo(0, 0);           //把window放在左上角     
        window.resizeTo(myw, myh);     //把当前窗体的长宽跳转为myw和myh     
    }
}   
  


//以下是软通写的
//初始化网页URL参数
function initPageParam() {
    //新建独有
    pageData.UserNo = GetQueryString("UserNo");
    pageData.DoWhat = GetQueryString("DoWhat");
    pageData.IsMobile = GetQueryString("IsMobile");

    pageData.FK_Flow = GetQueryString("FK_Flow");
    pageData.FK_Node = GetQueryString("FK_Node");
    //FK_Flow=004&FK_Node=402&FID=0&WorkID=232&IsRead=0&T=20160920223812&Paras=
    pageData.FID = GetQueryString("FID") == null ? 0 : GetQueryString("FID");
    pageData.WorkID = GetQueryString("WorkID");
    pageData.IsRead = GetQueryString("IsRead");
    pageData.T = GetQueryString("T");
    pageData.Paras = GetQueryString("Paras");
    pageData.IsReadonly = GetQueryString("IsReadonly"); //如果是IsReadonly，就表示是查看页面，不是处理页面
    pageData.IsStartFlow = GetQueryString("IsStartFlow"); //是否是启动流程页面 即发起流程

    pageData.DoType1 = GetQueryString("DoType")//View
    //$('#navIframe').attr('src', 'Admin/CCBPMDesigner/truck/centerTrakNav.html?FK_Flow=' + pageData.FK_Flow + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID);
}

//将获取过来的URL参数转成URL中的参数形式  &
function pageParamToUrl() {
    var paramUrlStr = '';
    for (var param in pageData) {
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

//设置表单元素不可用
function setFormEleDisabled() {
    //文本框等设置为不可用
    $('#divCCForm textarea').attr('disabled', 'disabled');
    $('#divCCForm select').attr('disabled', 'disabled');
    $('#divCCForm input[type!=button]').attr('disabled', 'disabled');
}

  

//刷新子流程
function refSubSubFlowIframe() {
    var iframe = $('iframe[src*="SubFlow.aspx"]');
    //iframe[0].contentWindow.location.reload();
    iframe[0].contentWindow.location.href = iframe[0].src;
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
 
//AtPara  @PopValSelectModel=0@PopValFormat=0@PopValWorkModel=0@PopValShowModel=0
function GepParaByName(name, atPara) {
    var params = atPara.split('@');
    var result = $.grep(params, function (value) {
        return value != '' && value.split('=').length == 2 && value.split('=')[0] == value;
    })
    return result;
}

//填充默认数据
function ConvertDefVal(workNodeData, defVal, keyOfEn) {
    //计算URL传过来的表单参数@TXB_Title=事件测试

    var pageParams = getQueryString();
    var pageParamObj = {};
    $.each(pageParams, function (i, pageParam) {
        if (pageParam.indexOf('@') == 0) {
            var pageParamArr = pageParam.split('=');
            pageParamObj[pageParamArr[0].substring(1, pageParamArr[0].length)] = pageParamArr[1];
        }
    });

    var result = defVal;

    //通过MAINTABLE返回的参数
    for (var ele in workNodeData.MainTable[0]) {
        if (keyOfEn == ele && workNodeData.MainTable[0] != '') {
            //console.info(ele + "==" + workNodeData.MainTable[0][ele]);
            result = workNodeData.MainTable[0][ele];
            break;
        }
    }

    if (result != undefined && typeof (result) == 'string') {
        //result = result.replace(/｛/g, "{").replace(/｝/g, "}").replace(/：/g, ":").replace(/，/g, ",").replace(/【/g, "[").replace(/】/g, "]").replace(/；/g, ";").replace(/~/g, "'").replace(/‘/g, "'").replace(/‘/g, "'");
    }
    //console.info(defVal+"=="+keyOfEn+"=="+result);
    var result = unescape(result);

    if (result == "null")
        result = "";

    return result;
}
   
           
//将v1版本表单元素转换为v2 杨玉慧  silverlight 自由表单转化为H5表单
function GenerWorkNode() {

    var href = window.location.href;
    var urlParam = href.substring(href.indexOf('?') + 1, href.length);
    urlParam = urlParam.replace('&DoType=', '&DoTypeDel=xx');


    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddUrlData();
    var data = handler.DoMethodReturnString("FrmFreeReadonly_Init"); 
//    $.ajax({
//        type: 'post',
//        async: true,
//        data: pageData,
//        url: MyFlow + "?DoType=FrmFreeReadonly_Init&m=" + Math.random() + "&" + urlParam,
//        dataType: 'html',
//        success: function (data) {

            if (data.indexOf('err@') == 0) {
                alert(data);
                return;
            }

            jsonStr = data;
            var gengerWorkNode = {};
            var flow_Data;

            try {

                flow_Data = JSON.parse(data);
                workNodeData = flow_Data;

            } catch (err) {
                alert("GenerWorkNode转换JSON失败:" + jsonStr);
                return;
            }

            //设置标题.
            document.title = flow_Data.WF_Node[0].Name;

            var Sys_GroupFields = workNodeData.Sys_GroupField;

            $('#CCForm').html('');

            var html = "<table style='width:100%;' >";

            var frmName = workNodeData.Sys_MapData[0].Name;

            html += "<tr>";
            html += "<td colspan=4 ><div style='float:left' ><img src='../DataUser/ICON/LogBiger.png'  style='height:50px;' /></div> <div  style='float:right;padding-right:10px;'  ><br><h4><b>" + frmName + "</b></h4></div></td>";
            html += "</tr>";

            //遍历循环生成 listview
            for (var i = 0; i < Sys_GroupFields.length; i++) {

                var gf = Sys_GroupFields[i];
                html += "<tr>";
                html += "  <th colspan=4>" + gf.Lab + "</th>";
                html += "</tr>";

                //从表..
                if (gf.CtrlType == 'Dtl') {

                    var dtls = workNodeData.Sys_MapDtl;

                    for (var k = 0; k < dtls.length; k++) {

                        var dtl = dtls[k];

                        if (dtl.No != gf.CtrlID)
                            continue;

                        html += "<tr>";
                        html += "  <td colspan='4' >";

                        html += figure_Template_Dtl(dtl);

                        html += "  </td>";
                        html += "</tr>";
                    }
                    continue;
                }


                //附件类的控件.
                if (gf.CtrlType == 'Ath') {

                    html += "<tr>";
                    html += "  <td colspan='4' >";

                    html += figure_Template_Attachment(workNodeData, gf);

                    html += "  </td>";
                    html += "</tr>";

                    continue;
                }

                //审核组件..
                if (gf.CtrlType == 'FWC') {

                    html += "<tr>";
                    html += "  <td colspan='4' >";

                    html += figure_Template_FigureFrmCheck(workNodeData.WF_Node[0]);

                    html += "  </td>";
                    html += "</tr>";

                    continue;
                }

                //字段类的控件.
                if (gf.CtrlType == '' || gf.CtrlType == null) {
                    html += InitMapAttr(workNodeData.Sys_MapAttr, workNodeData, gf.OID);
                    continue;
                }
            }

            html += "</table>";

            //加入隐藏控件.

            for (var attr in workNodeData.Sys_MapAttr) {
                if (attr.UIVisable == 0) {
                    var defval = ConvertDefVal(workNodeData, attr.DefVal, attr.KeyOfEn);
                    html += "<input type='hidden' id='TB_" + attr.KeyOfEn + "' name='TB_" + attr.KeyOfEn + "' value='" + defval + "' />";
                }
            }

            $('#CCForm').html("").append(html);

            //循环之前的提示信息.
            var info = "";
            for (var i in flow_Data.AlertMsg) {
                var alertMsg = flow_Data.AlertMsg[i];
                var alertMsgEle = figure_Template_MsgAlert(alertMsg, i);
                $('#Message').append(alertMsgEle);
                $('#Message').append($('<hr/>'));
            }

            if (flow_Data.AlertMsg.length != 0) {
                $('#MessageDiv').modal().show();
            }

            //循环组件 轨迹图 审核组件 子流程 子线程
            // $('#CCForm').append(figure_Template_FigureFlowChart(flow_Data["WF_FrmNodeComponent"][0]));
            // $('#CCForm').append(figure_Template_FigureFrmCheck(flow_Data["WF_FrmNodeComponent"][0]));
            // $('#CCForm').append(figure_Template_FigureSubFlowDtl(flow_Data["WF_FrmNodeComponent"][0]));
            // $('#CCForm').append(figure_Template_FigureThreadDtl(flow_Data["WF_FrmNodeComponent"][0]));


            //初始化Sys_MapData
            var h = flow_Data.Sys_MapData[0].FrmH;
            var w = flow_Data.Sys_MapData[0].FrmW;

            // $('#topContentDiv').height(h);
            $('#topContentDiv').width(w);
            $('.Bar').width(w + 15);
            $('#lastOptMsg').width(w + 15);
            var marginLeft = $('#topContentDiv').css('margin-left');
            marginLeft = marginLeft.replace('px', '');

            marginLeft = parseFloat(marginLeft.substr(0, marginLeft.length - 2)) + 50;
            $('#topContentDiv i').css('left', marginLeft.toString() + 'px');
            //原有的

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

            ////加载JS文件 改变JS文件的加载方式 解决JS在资源中不显示的问题
            var enName = workNodeData.Sys_MapData[0].No;
            try {
                ////加载JS文件
                //jsSrc = "<script language='JavaScript' src='/DataUser/JSLibData/" + enName + "_Self.js' ></script>";
                //$('body').append($('<div>' + jsSrc + '</div>'));

                var s = document.createElement('script');
                s.type = 'text/javascript';
                s.src = "../DataUser/JSLibData/" + enName + "_Self.js";
                var tmp = document.getElementsByTagName('script')[0];
                tmp.parentNode.insertBefore(s, tmp);
            }
            catch (err) {

            }

            var jsSrc = '';
            try {

                var s = document.createElement('script');
                s.type = 'text/javascript';
                s.src = "../DataUser/JSLibData/" + enName + "_Self.js";
                var tmp = document.getElementsByTagName('script')[0];
                tmp.parentNode.insertBefore(s, tmp);
            }
            catch (err) {

            }


            //设置默认值
            for (var j = 0; j < workNodeData.Sys_MapAttr.length; j++) {
                var mapAttr = workNodeData.Sys_MapAttr[j];

                //添加 label
                //如果是整行的需要添加  style='clear:both'
                var defValue = ConvertDefVal(workNodeData, mapAttr.DefVal, mapAttr.KeyOfEn);

                if ($('#TB_' + mapAttr.KeyOfEn).length == 1) {
                    $('#TB_' + mapAttr.KeyOfEn).val(defValue);
                }

                if ($('#DDL_' + mapAttr.KeyOfEn).length == 1) {
                    $('#DDL_' + mapAttr.KeyOfEn).val(defValue);
                }
            }

            showNoticeInfo();

            showTbNoticeInfo();


            //初始化复选下拉框 
            var selectPicker = $('.selectpicker');
            $.each(selectPicker, function (i, selectObj) {
                var defVal = $(selectObj).attr('data-val');
                var defValArr = defVal.split(',');
                $(selectObj).selectpicker('val', defValArr);
            });

            //给富文本 创建编辑器
            window.UEs = [];
            if (document.UE_MapAttr) {
                document.UE_MapAttr.forEach(function (item) {
                    var obj = {};
                    //根据字段只读属性 调整外观
                    if (item.MapAttr.UIIsEnable == "0") {
                        obj.editor = UM.getEditor(item.id, {
                            'toolbar': [],
                            'readonly': true,
                            'autoHeightEnabled': false,
                            'fontsize': [10, 12, 14, 16, 18, 20, 24, 36]
                        });
                    } else {
                        obj.editor = UM.getEditor(item.id, {
                            'autoHeightEnabled': false,
                            'fontsize': [10, 12, 14, 16, 18, 20, 24, 36]
                        });
                    }
                    obj.attr = item.MapAttr;
                    window.UEs.push(obj);

                    //调整样式,让必选的红色 * 随后垂直居中
                    obj.editor.$container.css({ "display": "inline-block", "margin-right": "10px", "vertical-align": "middle" });
                });
            }
//        }
//    })
}

//解析表单字段 MapAttr.
function InitMapAttr(Sys_MapAttr, workNodeData, groupID) {

    var html = "";
    var isDropTR = true;
    for (var i = 0; i < Sys_MapAttr.length; i++) {

        var attr = Sys_MapAttr[i];

        if (attr.GroupID != groupID || attr.UIVisible == 0)
            continue;

        var enable = attr.UIIsEnable == "1" ? "" : " ui-state-disabled";
        var defval = ConvertDefVal(workNodeData, attr.DefVal, attr.KeyOfEn);

        var lab = "";
        if (attr.UIContralType == 0)
            lab = "<label for='TB_" + attr.KeyOfEn + "' class='" + (attr.UIIsInput == 1 ? "mustInput" : "") + "'>" + attr.Name + "</label>";

        if (attr.UIContralType == 1)
            lab = "<label for='DDL_" + attr.KeyOfEn + "' class='" + (attr.UIIsInput == 1 ? "mustInput" : "") + "'>" + attr.Name + "</label>";

        if (attr.UIIsInput == 1 && attr.UIIsEnable == 1) {
            lab += " <span style='color:red' class='mustInput' data-keyofen='" + attr.KeyOfEn + "' >*</span>";
        }

        //        if (item.UIContralType == 2)
        //            lab = "<label for='CB_" + item.KeyOfEn + "' >" + item.Name + "</label>";

        //线性展示并且colspan=3
        if (attr.ColSpan == 3) {
            isDropTR = true;
            html += "<tr>";
            html += "<td  class='FoolFrmFieldCtrl'  >" + lab + "</td>";
            html += "<td ColSpan=3>";
            html += InitMapAttrOfCtrl(attr, enable, defval);
            html += "</td>";
            html += "</tr>";
            continue;
        }

        //线性展示并且colspan=4
        if (attr.ColSpan == 4) {
            isDropTR = true;
            html += "<tr>";
            html += "<td ColSpan=4 style='width:100%' >" + lab + "</br>";
            html += InitMapAttrOfCtrl(attr, enable, defval);
            html += "</td>";
            html += "</tr>";
            continue;
        }

        if (isDropTR == true) {
            html += "<tr>";
            html += "<td class='FoolFrmFieldCtrl' style='width:80px;' >" + lab + "</td>";
            html += "<td class='FContext'  >";
            html += InitMapAttrOfCtrl(attr, enable, defval);
            html += "</td>";
            isDropTR = !isDropTR;
            continue;
        }

        if (isDropTR == false) {
            html += "<td  class='FoolFrmFieldCtrl'>" + lab + "</td>";
            html += "<td class='FContext'  >";
            html += InitMapAttrOfCtrl(attr, enable, defval);
            html += "</td>";
            html += "<tr>";
            isDropTR = !isDropTR;
            continue;
        }
    }
    return html;
}

function InitMapAttrOfCtrl(mapAttr) {


    var str = '';
    var defValue = ConvertDefVal(workNodeData, mapAttr.DefVal, mapAttr.KeyOfEn);

    var isInOneRow = false; //是否占一整行
    var islabelIsInEle = false; //

    var eleHtml = '';

    //外键类型.
    if (mapAttr.LGType == 2) {
        //   return "<select data-val='" + ConvertDefVal(workNodeData, mapAttr.DefVal, mapAttr.KeyOfEn) + "' class='" + isMultiSeleClass + "' " + isMultiSele + " name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable==1 ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(workNodeData, mapAttr, defValue) + "</select>";
    }

    //添加文本框 ，日期控件等
    //AppString
    if (mapAttr.MyDataType == "1") {  //不是外键

        if (mapAttr.UIHeight <= 40) //普通的文本框.
            return "<input maxlength=" + mapAttr.MaxLen + "  name='TB_" + mapAttr.KeyOfEn + "' style='width:" + mapAttr.UIWidth + ";height:" + mapAttr.UIHeight + ";' type='text' " + (mapAttr.UIIsEnable == 1 ? '' : ' disabled="disabled"') + " />";

        if (mapAttr.AtPara && mapAttr.AtPara.indexOf("@IsRichText=1") >= 0) {

            var eleHtml = "";
            //如果是富文本就使用百度 UEditor
            if (document.UE_MapAttr === undefined) {
                document.UE_MapAttr = [];
            }
            var editorPara = {};
            editorPara.id = "container" + document.UE_MapAttr.length;
            editorPara.MapAttr = mapAttr;
            document.UE_MapAttr.push(editorPara);

            //设置编辑器的默认样式
            var styleText = "text-align:left;font-size:12px;";
            styleText += "width:100%;";
            styleText += "height:" + mapAttr.UIHeight + "px;";

            if (mapAttr.UIIsEnable == "0") {
                //字段处于只读状态.注意这里 name 属性也是可以用来绑定字段名字的
                eleHtml += "<script id='" + editorPara.id + "' name='TB_" + mapAttr.KeyOfEn + "' type='text/plain' style='" + styleText + "'>" + defValue + "</script>";
            } else {
                eleHtml += "<script id='" + editorPara.id + "' name='TB_" + mapAttr.KeyOfEn + "' type='text/plain' style='" + styleText + "'></script>";
            }

            eleHtml = "<div style='white-space:normal;'>" + eleHtml + "</div>";
            return eleHtml
        }


        //普通的大块文本.
        return "<textarea maxlength=" + mapAttr.MaxLen + " style='height:" + mapAttr.UIHeight + "px;width:100%;' name='TB_" + mapAttr.KeyOfEn + "' type='text'  " + (mapAttr.UIIsEnable == 1 ? '' : ' disabled="disabled"') + " />"
    }

    //日期类型.
    if (mapAttr.MyDataType == 6) {
        var enableAttr = '';
        if (mapAttr.UIIsEnable == 1)
            enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd'})" + '";';
        else
            enableAttr = "disabled='disabled'";

        return "<input " + enableAttr + " style='width:80px;' name='TB_" + mapAttr.KeyOfEn + "' />";
    }

    if (mapAttr.MyDataType == 7) {

        var enableAttr = '';
        if (mapAttr.UIIsEnable == 1)
            enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd HH:mm'})" + '";';
        else
            enableAttr = "disabled='disabled'";

        return "<input  type='text' " + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "' />";
    }


    if (mapAttr.MyDataType == 4) {  // AppBoolean = 7

        if (mapAttr.UIIsEnable == 1)
            enableAttr = "";
        else
            enableAttr = "disabled='disabled'";

        //CHECKBOX 默认值
        var checkedStr = '';
        if (checkedStr != "true" && checkedStr != '1') {
            checkedStr = ' checked="checked" ';
        }

        checkedStr = ConvertDefVal(workNodeData, '', mapAttr.KeyOfEn);

        return "<input " + enableAttr + " " + (defValue == 1 ? "checked='checked'" : "") + " type='checkbox' id='CB_" + mapAttr.KeyOfEn + "'  name='CB_" + mapAttr.KeyOfEn + "' " + checkedStr + " /><label for='CB_" + mapAttr.KeyOfEn + "' >" + mapAttr.Name + "</label>";
    }

    if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) { //AppInt Enum
        return "<select name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable == 1 ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(workNodeData, mapAttr, defValue) + "</select>";
    }

    // AppDouble  AppFloat
    if (mapAttr.MyDataType == 5 || mapAttr.MyDataType == 3) {

        var enableAttr = '';
        if (mapAttr.UIIsEnable != 1)
            enableAttr = "disabled='disabled'";

        return "<input style='text-align:right;' onkeyup=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>";
    }

    if ((mapAttr.MyDataType == 2)) {//AppInt
        var enableAttr = '';
        if (mapAttr.UIIsEnable != 1) {
            enableAttr = "disabled='disabled'";
        }
        return "<input style='text-align:right;' onkeyup=" + '"' + "if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>";
    }

    //AppMoney  AppRate
    if (mapAttr.MyDataType == 8) {
        var enableAttr = '';
        if (mapAttr.UIIsEnable == 1) {

        } else {
            enableAttr = "disabled='disabled'";
        }
        return "<input style='text-align:right;' onkeyup=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>";
    }

    alert(mapAttr.Name + "的类型没有判断.");
    return;
}

var workNodeData = {};


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
    if (workNodeData.Sys_FrmImgAthDB) {
        $.each(workNodeData.Sys_FrmImgAthDB, function (i, obj) {
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
        var url = "/WF/CCForm/ImgAth.htm?W=" + frmImageAth.W + "&H=" + frmImageAth.H + "&FK_MapData=ND" + pageData.FK_Node + "&MyPK=" + pageData.WorkID + "&ImgAth=" + frmImageAth.MyPK;

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


//审核组件
function figure_Template_FigureFrmCheck(wf_node) {
    //审核组键FWCSta Sta,FWC_X X,FWC_Y Y,FWC_H H, FWC_W W from WF_Node
    var sta = wf_node.FWCSta;

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
    if (sta == 2)//只读
    {
        src += "&DoType=View";
    }
    else {
        fwcOnload = "onload= 'WC" + wf_node.NodeID + "load();'";
        $('body').append(addLoadFunction("WC" + wf_node.NodeID, "blur", "SaveDtl"));
    }
    src += "&r=q" + paras;
    var eleHtml = "<iframe width='100%' height='" + wf_node.FWC_H + "px' id='FFWC" + wf_node.NodeID + "' src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>";


    return eleHtml;
}

//初始化 附件
function figure_Template_Attachment(workNodeData, gf) {

    var ath = workNodeData.Sys_FrmAttachment[0];

    var eleHtml = '';
    if (ath.UploadType == 0) { //单附件上传 L4204
        return '';
    }
    var src = "";
    if (pageData.IsReadonly)
        src = "./CCForm/Ath.htm?PKVal=" + pageData.WorkID + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + ath.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK + "&IsReadonly=1";
    else
        src = "./CCForm/Ath.htm?PKVal=" + pageData.WorkID + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + ath.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK;

    eleHtml += "<iframe style='width:100%;height:" + ath.H + "px;' ID='Attach_" + ath.MyPK + "'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';


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

var appPath = "../../";
var DtlsCount = " + dtlsCount + "; //应该加载的明细表数量

//初始化从表
function figure_Template_Dtl(frmDtl) {
    var eleHtml = $("<DIV id='Fd" + frmDtl.No + "' style='position:absolute; left:" + frmDtl.X + "px; top:" + frmDtl.Y + "px; width:" + frmDtl.W + "px; height:" + frmDtl.H + "px;text-align: left;' >");
    var paras = this.pageData;
    var strs = "";
    for (var str in paras) {
        if (str == "EnsName" || str == "RefPKVal" || str == "IsReadonly")
            continue
        else
            strs += "&" + str + "=" + paras[str];
    }
    var src = "";
    var href = window.location.href;
    var urlParam = href.substring(href.indexOf('?') + 1, href.length);
    urlParam = urlParam.replace('&DoType=', '&DoTypeDel=xx');
    if (frmDtl.EditModel == "0") {

        var dtlUrl = "Dtl2017";
        if (frmDtl.DtlVer == 1)
            dtlUrl = "Dtl2019";

        if (pageData.IsReadonly) {
            src = dtlUrl + ".htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&IsReadonly=1&" + urlParam + "&Version=" + Math.random();
        } else {
            src = dtlUrl + ".htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&IsReadonly=0&" + urlParam + "&Version=" + Math.random();
        }
    }
    else if (frmDtl.EditModel == "1") {
        if (pageData.IsReadonly)
            src = "DtlCard.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&IsReadonly=1" + strs;
        else
            src = "DtlCard.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&IsReadonly=0" + strs;

    }
    var eleIframe = '<iframe></iframe>';
    eleIframe = $("<iframe class='Fdtl' ID='F" + frmDtl.No + "' src='" + src +
                 "' frameborder=0  style='position:absolute;width:100%; height:" + frmDtl.H +
                 "px;text-align: left;'  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");
    if (pageData.IsReadonly) {

    } else {
        if (frmDtl.DtlSaveModel == 0) {
            eleHtml.append(addLoadFunction(frmDtl.No, "blur", "SaveDtl"));
            eleIframe.attr('onload', frmDtl.No + "load()");
        }
    }
    eleHtml.append(eleIframe);
    //added by liuxc,2017-1-10,此处前台JS中增加变量DtlsLoadedCount记录明细表的数量，用于加载完全部明细表的判断
    var js = "";
    if (pageData.IsReadonly == false) {
        js = "<script type='text/javascript' >";
        js += " function SaveDtl(dtl) { ";
        js += "   GenerPageKVs(); //调用产生kvs ";
        js += "\n   var iframe = document.getElementById('F' + dtl );";
        js += "   if(iframe && iframe.contentWindow){ ";
        js += "      iframe.contentWindow.SaveDtlData(); ";
        js += "   } ";
        js += " } ";
        js += " function SaveM2M(dtl) { ";
        js += "   document.getElementById('F' + dtl ).contentWindow.SaveM2M();";
        js += "} ";
        js += "</script>";
        eleHtml.append($(js));
    }
    return eleHtml;
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


//子线程
function figure_Template_FigureThreadDtl(wf_node) {

    //FrmThreadSta Sta,FrmThread_X X,FrmThread_Y Y,FrmThread_H H,FrmThread_W
    var sta = wf_node.FrmThreadSta;
    var x = wf_node.FrmThread_X;
    var y = wf_node.FrmThread_Y;
    var h = wf_node.FrmThread_H;
    var w = wf_node.FrmThread_W;
    if (sta == 0 || sta == '0')
        return $('');

    var src = "./WorkOpt/Thread.htm?s=2";
    var fwcOnload = "";
    var paras = '';

    paras += "&FID=" + pageData["FID"];
    paras += "&OID=" + pageData["WorkID"];
    paras += '&FK_Flow=' + pageData.FK_Flow;
    paras += '&FK_Node=' + pageData.FK_Node;
    paras += '&WorkID=' + pageData.WorkID;

    if (sta == 2) //只读
    {
        src += "&DoType=View";
    }
    else {
        fwcOnload = "onload= 'WC" + wf_node.NodeID + "load();'";
        $('body').append(addLoadFunction("WC" + wf_node.NodeID, "blur", "SaveDtl"));
    }
    src += "&r=q" + paras;
    var eleHtml = '<div id=DIVFT' + wf_node.NodeID + '>' + "<iframe id=FFT" + wf_node.NodeID + " style='width:100%;height:" + h + "px;'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    eleHtml = $(eleHtml);
    eleHtml.css('position', 'absolute').css('top', y).css('left', x).css('width', w).css('height', h);

    return eleHtml;
}

//子流程
function figure_Template_FigureSubFlowDtl(wf_node) {
    //SFSta Sta,SF_X X,SF_Y Y,SF_H H, SF_W W
    var sta = wf_node.SFSta;
    var x = wf_node.SF_X;
    var y = wf_node.SF_Y;
    var h = wf_node.SF_H;
    var w = wf_node.SF_W;
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
    var eleHtml = '<div id=DIVWC' + wf_node.NodeID + '>' + "<iframe id=FSF" + wf_node.NodeID + " style='width:" + w + "px';height:" + h + "px'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    eleHtml = $(eleHtml);
    eleHtml.css('position', 'absolute').css('top', y).css('left', x).css('width', w).css('height', h);

    return eleHtml;
}

//初始化框架
function figure_Template_IFrame(fram) {
    var eleHtml = '';
    var src = dealWithUrl(fram.src) + "IsReadonly=0";
    eleHtml = $('<div id="iframe' + fram.MyPK + '">' + '</div>');
    var iframe = $(+"<iframe  style='width:" + fram.W + "px; height:" + fram.H + "'     src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling='no'></iframe>");

    eleHtml.css('position', 'absolute').css('top', fram.Y).css('left', fram.X).css('width', fram.W).css('height', fram.H);
    return frameHtml;
}

function figure_Template_MsgAlert(msgAlert, i) {
    var eleHtml = $('<div></div>');
    var titleSpan = $('<span class="titleAlertSpan"> ' + (parseInt(i) + 1) + "&nbsp;&nbsp;&nbsp;" + msgAlert.Title + '</span>');
    var msgDiv = $('<div>' + msgAlert.Msg + '</div>');
    eleHtml.append(titleSpan).append(msgDiv)
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
                            params[i] = paramArr[0].substring(1) + "=" + workNodeData.MainTable[0][paramArr[1].substr('@WebUser.'.length)];
                        }
                        if (workNodeData.MainTable[0][paramArr[1].substr(1)] != undefined) {
                            params[i] = paramArr[0].substring(1) + "=" + workNodeData.MainTable[0][paramArr[1].substr(1)];
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
                        for (var ele in workNodeData.MainTable[0]) {
                            if (paramArr[0].substring(1) == ele) {
                                result = workNodeData.MainTable[0][ele];
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

//从MyFlowFree2017.htm 中拿过过的.

var pageData = {};
var globalVarList = {};

$(function () {

    var frm = document.forms["divCCForm"];

    if (plant == "CCFlow")
        frm.action = "MyFlow.ashx?method=login";
    else
        frm.action = MyFlow + "?method=login";

    initPageParam(); //初始化参数

    GenerWorkNode(); //表单数据.ajax


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