//全局变量
var figureSetsURL = 'Controls';
var CCPMB_Form_V = "2";

$(function () {

    CCForm_FK_MapData = Application.common.getArgsFromHref("FK_MapData");
    CCForm_NodeID = CCForm_FK_MapData.replace("ND", "");
    CCPMB_Form_V = 1;

    /**default height for canvas*/
    CanvasProps.DEFAULT_HEIGHT = 1200;
    /**default width for canvas*/
    CanvasProps.DEFAULT_WIDTH = 1000;

    //验证登录用户
    checklogin();

    //初始化画板
    Init_Panel(CCForm_FK_MapData);

    //显示网格.
    ShowGrid();

    //右键菜单
    InitContexMenu();

    //鼠标双击
    InitDbClick();

    InitClick();

    //鼠标移动
    //InitonMouseMove();
    //初始节点元素
    buildPanel();
    //设置属性高度
    ReSetEditDivCss();

});

//初始化右键菜单
function InitContexMenu() {
    //画板右键
    $("#a").bind('contextmenu', function (ev) {
        //阻止右键菜单弹出
        ev.preventDefault();
        //将下面的 RETURN 注释去掉 就不会弹出右键菜单【删除--属性】
        //return;

        var coords = getCanvasXY(ev);
        var x = coords[0];
        var y = coords[1];
        lastClick = [x, y];
        // store id value (from Stack) of clicked text primitive
        var textPrimitiveId = -1;

        //find Connector at (x,y)
        var cId = CONNECTOR_MANAGER.connectorGetByXY(x, y);
        // check if we clicked a connector
        if (cId != -1) {
            textPrimitiveId = 0; // (0 by default)
        } else {
            cId = CONNECTOR_MANAGER.connectorGetByTextXY(x, y);

            // check if we clicked a text of connector
            if (cId != -1) {
                textPrimitiveId = 0; // (0 by default)
            } else {

                //find Figure at (x,y)
                var fId = STACK.figureGetByXY(x, y);
                // check if we clicked a figure
                if (fId != -1) {
                    $('#figureMenu').menu({
                        onShow: function () {
                            $("#HD_FigureID").val(fId);
                        }, onClick: FigureProperty_Funs
                    });
                    //弹出右键菜单
                    ev.preventDefault();
                    $('#figureMenu').menu('show', {
                        left: ev.pageX,
                        top: ev.pageY
                    });
                } else {
                    //find Container at (x,y)
                    var contId = STACK.containerGetByXY(x, y);
                    ev.preventDefault();
                    $('#mFormSheet').menu('show', {
                        left: ev.pageX,
                        top: ev.pageY
                    });
                }
            }
        }
    });
}

//画板单击事件绑定
function InitDbClick() {
    $('#a').bind('dblclick', function (ev) {
        window.getSelection().removeAllRanges();
        var coords = getCanvasXY(ev);
        var x = coords[0];
        var y = coords[1];
        var figureId = STACK.figureGetByXY(x, y);
        if (figureId != -1) {
            var figure = STACK.figureGetById(figureId);
            ondbclickCallBackFun(figure);
            return;
        }
        //看看有没有线
        var connectorIndex = CONNECTOR_MANAGER.connectorGetByXY(x, y);
        if (connectorIndex >= 0) {
            $('#right').css('display', 'display');
            $('#container').css('right', 'auto');
            return;
        }
        //打开表单属性.
        CCForm_Attr();

        return;
    })
}

function InitClick() {
    $('#a').bind('click', function () {
        $('#right').css('display', 'none');
        $('#container').css('right', '0px');
    })
}

var figureIdMouseMove = -1;
var dealWhithMouseMove = false;
//画板鼠标悬浮事件绑定
function InitonMouseMove() {
    $('#a').bind('mousemove', function (ev) {
        if (!dealWhithMouseMove) {
            dealWhithMouseMove = true;
            var coords = getCanvasXY(ev);
            var x = coords[0];
            var y = coords[1];
            var figureId = STACK.figureGetByXY(x, y);
            if (figureIdMouseMove != figureId) {
                figureIdMouseMove = figureId;

                if (figureId != -1) {
                    var figure = STACK.figureGetById(figureId);
                    mouseHoverCallBackFun(figure, x, y);
                }
            }
            dealWhithMouseMove = false;
        }
    })
}
function mouseHoverCallBackFun(figure, x, y) {
    var keyOfEnPro = $.grep(figure.properties, function (p) { return p.property == "KeyOfEn" });
    if (keyOfEnPro.length == 1) {
        $('#figureTip').css('top', y).css('left', x).text(keyOfEnPro[0].PropertyValue).show();
    }
    //5S后消失
    setTimeout("$('#figureTip').hide()", 5000);
}

//画板元素双击时的回掉方法
function ondbclickCallBackFun(figure) {
    showFigurePropertyWin(figure);
}

//打开元素的属性窗口
function showFigurePropertyWin(figure) {
    
    var v = figure.CCForm_Shape;
    var shap = figure.CCForm_Shape;

    if (shap == 'TextBoxStr') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrString&PKVal=' + CCForm_FK_MapData + '_' + figure.CCForm_MyPK;
        CCForm_ShowDialog(url, '字段String属性');
        return;
    }

    if (shap == 'TextBoxDate') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrDT&PKVal=' + CCForm_FK_MapData + '_' + figure.CCForm_MyPK;
        CCForm_ShowDialog(url, '字段Date属性');
        return;
    }

    if (shap == 'TextBoxDateTime') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrDT&PKVal=' + CCForm_FK_MapData + '_' + figure.CCForm_MyPK;
        CCForm_ShowDialog(url, '字段DateTime属性');
        return;
    }

    if (shap == 'TextBoxMoney') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&PKVal=' + CCForm_FK_MapData + '_' + figure.CCForm_MyPK;
        CCForm_ShowDialog(url, '字段Money属性');
        return;
    }

    if (shap == 'TextBoxDouble') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&PKVal=' + CCForm_FK_MapData + '_' + figure.CCForm_MyPK;
        CCForm_ShowDialog(url, '字段Double属性');
        return;
    }

    if (shap == 'TextBoxInt') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&PKVal=' + CCForm_FK_MapData + '_' + figure.CCForm_MyPK;
        CCForm_ShowDialog(url, '字段Int属性');
        return;
    }

    if (shap == 'TextBoxFloat') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&PKVal=' + CCForm_FK_MapData + '_' + figure.CCForm_MyPK;
        CCForm_ShowDialog(url, '字段Float属性');
        return;
    }

    if (shap == 'DropDownListEnum') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrEnum&PKVal=' + CCForm_FK_MapData + '_' + figure.CCForm_MyPK;
        CCForm_ShowDialog(url, '字段Enum属性');
        return;
    }

    if (shap == 'TextBoxBoolean') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrBoolen&PKVal=' + CCForm_FK_MapData + '_' + figure.CCForm_MyPK;
        CCForm_ShowDialog(url, '字段 Boolen 属性');
        return;
    }

    if (shap == 'DropDownListTable') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrSFTable&PKVal=' + CCForm_FK_MapData + '_' + figure.CCForm_MyPK;
        CCForm_ShowDialog(url, '字段外键属性');
        return;
    }


    if (shap == 'Dtl') {
        var url = '../../Admin/FoolFormDesigner/MapDefDtlFreeFrm.htm?FK_MapData=' + CCForm_FK_MapData + '&FK_MapDtl=' + figure.CCForm_MyPK;
        var W = document.body.clientWidth - 40;
        var H = $("#editor")[0].clientHeight - 40;
        CCForm_ShowDialog(url, '从表/明细表' + figure.CCForm_MyPK + '属性', W, H);
        return;
    }

    if (shap == 'Image') {
        var url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtImg&PKVal=' + figure.CCForm_MyPK;
        CCForm_ShowDialog(url, '图片' + figure.CCForm_MyPK + '属性');
        return;
    }

    if (shap == 'Button') {
        var url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.FrmBtn&PKVal=' + figure.CCForm_MyPK;
        CCForm_ShowDialog(url, '按钮' + figure.CCForm_MyPK + '属性');
        return;
    }


    // 附件类的属性 .... 
    if (shap == 'AthSingle') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.FrmAttachmentExt&PKVal=' + CCForm_FK_MapData + '_' + figure.CCForm_MyPK;
        CCForm_ShowDialog(url, '单附件属性');
        return;
    }

    if (shap == 'AthMulti') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.FrmAttachmentExt&PKVal=' + CCForm_FK_MapData + '_' + figure.CCForm_MyPK;
        CCForm_ShowDialog(url, '多附件属性');
        return;
    }

    if (shap == 'AthImg') {

        //alert(CCForm_FK_MapData);

        var url = '../../Comm/RefFunc/EnOnly.htm?EnName=BP.Sys.FrmUI.FrmImgAth&PKVal=' + CCForm_FK_MapData + '_' + figure.CCForm_MyPK;
        CCForm_ShowDialog(url, '图片附件');
        return;
    }

    //流程类的组件.
    if (shap == 'FlowChart') {
        var url = '../../Comm/RefFunc/EnOnly.htm?EnName=BP.WF.Template.FrmTrack&PKVal=' + CCForm_FK_MapData.replace('ND', '') + '&tab=轨迹组件';
        CCForm_ShowDialog(url, '轨迹组件');
        return;
    }

    if (shap == 'FrmCheck') {
        var url = '../../Comm/RefFunc/EnOnly.htm?EnName=BP.WF.Template.NodeWorkCheck&PKVal=' + CCForm_FK_MapData.replace('ND', '') + '&tab=子线程组件';
        CCForm_ShowDialog(url, '审核组件');
        return;
    }

    if (shap == 'SubFlowDtl') {
        var url = '../../Comm/RefFunc/EnOnly.htm?EnName=BP.WF.Template.SFlow.FrmSubFlow&PKVal=' + CCForm_FK_MapData.replace('ND', '') + '&tab=子线程组件';
        CCForm_ShowDialog(url, '父子流程组件');
        return;
    }

    if (shap == 'ThreadDtl') {
        var url = '../../Comm/RefFunc/EnOnly.htm?EnName=BP.WF.Template.FrmThread&PKVal=' + CCForm_FK_MapData.replace('ND', '') + '&tab=子线程组件';
        CCForm_ShowDialog(url, '子线程组件');
        return;
    }

    if (shap == 'FrmTransferCustom') {
        var url = '../../Comm/RefFunc/EnOnly.htm?EnName=BP.WF.Template.FrmTransferCustom&PKVal=' + CCForm_FK_MapData.replace('ND', '') + '&tab=子线程组件';
        CCForm_ShowDialog(url, '流转自定义');
        return;
    }

    if (shap == 'HyperLink') {
        var url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.FrmLink&PKVal=' + figure.CCForm_MyPK;
        CCForm_ShowDialog(url, '超链接属性');
        return;
    }


    //枚举类型.
    if (shap == 'RadioButton') {

        var mypk = figure.CCForm_MyPK;

        mypk = mypk.replace('RB_', "");
        mypk = mypk.substr(0, mypk.lastIndexOf('_'));
        mypk = mypk.replace('_0', "");
        mypk = mypk.replace('_1', "");
        mypk = mypk.replace('_2', "");
        mypk = mypk.replace('_3', "");


        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrEnum&PKVal=' + CCForm_FK_MapData + "_" + mypk;
        CCForm_ShowDialog(url, '单选按钮属性');
        return;
    }

    if (shap == 'iFrame') {


        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapFrameExt&PKVal='+figure.CCForm_MyPK;
        CCForm_ShowDialog(url, '框架');
        return;
    }

    if (shap == 'Label') {
        alert('请在右边查看该元素的属性,如果被遮挡您需要全屏打开该设计器进行设计.');
        $('#right').css('display', 'block');
        $('#container').css('right', 'auto');
        return;
    }

    alert('没有判断的双击类型:' + shap);
}


//初始化画板元素
function buildPanel() {
    //var first = true;
    for (var setName in figureSets) {
        var set = figureSets[setName];
        var groupSetDiv = document.createElement('div');
        groupSetDiv.className = "figurePanel";
        groupSetDiv.addEventListener("click", function (setName) {
            return function (evt) {
                evt.preventDefault();
                setFigurePanel(setName);
            };
        }(setName), false);

        var groupImg = document.createElement('img');
        groupImg.setAttribute('src', "Img/Min.png");
        groupImg.setAttribute('align', "middle");
        groupImg.setAttribute('id', setName + "img");
        groupSetDiv.appendChild(groupImg);

        var groupText = document.createElement('div');
        groupText.innerHTML = set.name;
        groupSetDiv.appendChild(groupText);

        document.getElementById('figures').appendChild(groupSetDiv);

        var eSetDiv = document.createElement('div');
        eSetDiv.setAttribute('id', setName);
        document.getElementById('figures').appendChild(eSetDiv);

        var figures_UL = document.createElement("ul");
        figures_UL.className = "navlist";
        eSetDiv.appendChild(figures_UL);
        //add figures to the div
        for (var figure in set['figures']) {
            figure = set['figures'][figure];

            var figureFunctionName = 'figure_' + figure.figureFunction;
            var figureThumbURL = figureSetsURL + '/' + setName + '/' + figure.image;
            var figureName = figure.name;
            var cnName = figure.CNName;
            var eFigure = document.createElement('li');
            var eFigure_div = document.createElement('div');
            eFigure_div.setAttribute('id', figure.name);
            eFigure_div.style.backgroundImage = "url(" + figureThumbURL + ")";

            var eFigure_span = document.createElement('span');
            eFigure_span.className = "navlistspan";
            //eFigure_span.innerHTML = figure.image;
            eFigure_span.textContent = cnName;
            eFigure_div.appendChild(eFigure_span);
            eFigure.appendChild(eFigure_div);

            eFigure.addEventListener('mousedown', function (figureFunction, figureThumbURL, figureName) {
                return function (evt) {
                    evt.preventDefault();
                    createFigure(window[figureFunction], figureThumbURL, figureName);
                };
            }(figureFunctionName, figureThumbURL, figureName), false);

            //in case use drops the figure
            eFigure.addEventListener('mouseup', function () {
                createFigureFunction = null;
                selectedFigureThumb = null;
                state = STATE_NONE;
            }, false);

            eFigure.style.cursor = 'pointer';
            //eFigure.style.marginRight = '5px';
            eFigure.style.marginTop = '2px';

            figures_UL.appendChild(eFigure);
        }
    }
}

function FigureProperty_Funs(item) {
    var figureId = $("#HD_FigureID").val();
    var figure = STACK.figureGetById(figureId);

    //根据事件名称进行执行
    switch (item.name) {
        case "figure_delete": //删除元素
            STACK.figureRemoveById(figureId)
            draw();
            break;
        case "figure_property"://打开属性窗口
            showFigurePropertyWin(figure);
            break;
    }
}

//统一弹出消息窗口
function Designer_ShowMsg(msg, callBack) {
    if (window.parent && window.parent.BPMN_Msg) {
        window.parent.BPMN_Msg(msg, callBack);
    } else {
        alert(msg);
        if (callBack) callBack();
    }
}

//工具栏展开缩放
function setFigurePanel(id) {
    var div = document.getElementById(id);
    if (div != null) {
        var divImg = document.getElementById(id + "img");
        var display = div.style.display;
        if (display == "none") {
            div.style.display = 'block';
            if (divImg) divImg.src = "Img/Min.png";
        } else {
            div.style.display = 'none';
            if (divImg) divImg.src = "Img/Max.png";
        }
    }
}

//网格显示
function GridLineVisible() {
    gridVisible = !gridVisible;
    document.getElementById("gridCheckbox").checked = gridVisible;
    backgroundImage = null; // reset cached background image of canvas
    //trigger a repaint;
    draw();

    var curVisible = document.getElementById("div_gridvisible").innerHTML;
    if (curVisible == "隐藏网格") {
        document.getElementById("div_gridvisible").innerHTML = "显示网格";
    } else if (curVisible == "显示网格") {
        document.getElementById("div_gridvisible").innerHTML = "隐藏网格";
    }
}
//隐藏字段显示窗体
function Show_HidenField_Panel() {
    var url = "DialogCtr/FrmHiddenField.htm?FK_MapData=" + CCForm_FK_MapData + "&s=" + Math.random();
    OpenEasyUiDialog(url, 'FrmHiddenField', '隐藏字段', 600, 394, 'icon-new', true, function (scope) {
        var win = document.getElementById(dgId).contentWindow;
        var frmVal = win.GetFrmInfo();

        if (frmVal.Name == null || frmVal.Name.length == 0) {
            $.messager.alert('错误', '字段名称不能为空。', 'error');
        }
    }, this);
}

function Show_ModleField_Panel() {
    var url = "../FoolFormDesigner/Template/Fields.htm?FK_MapData=" + CCForm_FK_MapData + "&s=" + Math.random();
    OpenEasyUiDialog(url, 'FrmHiddenField', '模板字段', 600, 394, 'icon-new', true, function (scope) {
        var win = document.getElementById(dgId).contentWindow;
        var frmVal = win.GetFrmInfo();

        if (frmVal.Name == null || frmVal.Name.length == 0) {
            $.messager.alert('错误', '字段名称不能为空。', 'error');
        }
    }, this);


}

//打开窗体
function CCForm_ShowDialog(url, title, w, h) {

    if (w == undefined)
        w = 760;

    if (h == undefined)
        h = 460;

    if (plant == 'JFlow') {
        url = url.replace('.aspx', '.jsp');
        OpenEasyUiDialog(url, 'CCForm_ShowDialog', title, w, h, 'icon-library', false);
    } else {
        OpenEasyUiDialog(url, 'CCForm_ShowDialog', title, w, h, 'icon-library', false);
    }
}

//预览表单
function CCForm_BrowserView() {
    var url = "../../CCForm/Frm.htm?FK_MapData=" + CCForm_FK_MapData + "&FrmType=FreeFrm&IsTest=1&WorkID=0&FK_Node=999999&s=2&T=" + GetDateString();
    OpenWindow(url);
}

//预览表单
function CCForm_FoolFrm() {

    var url = '../../Admin/FoolFormDesigner/Designer.htm?FK_MapData=' + CCForm_FK_MapData + '&FK_Flow=001&MyPK=ND101&IsEditMapData=True&IsFirst=1';
    // var url = "../../CCForm/Frm.htm?FK_MapData=" + CCForm_FK_MapData + "&FrmType=FreeFrm&IsTest=1&WorkID=0&FK_Node=999999&s=2&T=" + GetDateString();
    OpenWindow(url);
}

//表单属性
function CCForm_Attr() {
    var url = '../../Comm/En.htm?EnName=BP.WF.Template.Frm.MapFrmFree&PKVal=' + CCForm_FK_MapData;
    var fk_node = Application.common.getArgsFromHref("FK_Node");
    //设置的为节点表单
    if (fk_node) {
        url = '../../Comm/En.htm?EnName=BP.WF.Template.Frm.MapFrmNode&PKVal=' + CCForm_FK_MapData;
    }

    CCForm_ShowDialog(url, '表单属性', $(parent).width() * 0.65, $(parent).height() * 0.8);
}

//打开页面方法
function OpenWindow(url, h, w) {

    var winWidth = 850;
    var winHeight = 680;

    if (w > 0)
        winWidth = w;

    if (h > 0)
        winHeight = h;

    if (screen && screen.availWidth) {
        winWidth = screen.availWidth;
        winHeight = screen.availHeight - 36;
    }

    //  OpenEasyUiDialog(url, 'CCForm_ShowDialog', title, winHeight, winWidth, 'icon-library', false);

    window.open(url, "_blank", "height=" + winHeight + ",width=" + winWidth + ",top=0,left=0,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no");
}


function GetDateString() {

    var strTimeKey = "";
    var date = new Date();
    strTimeKey += date.getFullYear(); //年
    strTimeKey += date.getMonth() + 1; //月 月比实际月份要少1
    strTimeKey += date.getDate(); //日
    strTimeKey += date.getHours(); //HH
    strTimeKey += date.getMinutes(); //MM
    strTimeKey += date.getSeconds(); //SS

    return strTimeKey;
}


//设置属性样式
function ReSetEditDivCss() {
    var h = $("#container").height(); //画板高度
    $("#edit").height(h - 75);

    //工具栏样式
    $('.navlist li div').click(function () {
        $('.navlist li div').removeClass("selected");
        $(this).addClass("selected");
        //默认
        var canvas = getCanvas();
        state = STATE_NONE;
        canvas.style.cursor = 'default';
        STATE_FORM_CREATE_LINE = false;

        var element = $(this)[0];
        if (element && element.id) {
            if (element.id == "Line") {
                STATE_FORM_CREATE_LINE = true;
                state = STATE_CONNECTOR_CREATE;
                canvas.style.cursor = 'crosshair';
            }
        }
    }).hover(function () {
        $(this).addClass("hover");
    }, function () {
        $(this).removeClass("hover");
    });
    //toolbar 鼠标样式
    $('.actions a label').hover(function () {
        $(this).addClass("hover");
    }, function () {
        $(this).removeClass("hover");
    });
}


//将v1版本表单元素转换为v2 杨玉慧  silverlight 自由表单转化为H5表单
function Conver_CCForm_V1ToV2() {

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCFormDesigner");
    handler.AddPara("FK_MapData", CCForm_FK_MapData);
    handler.AddPara("FK_Node", CCForm_FK_MapData.substr(2, CCForm_FK_MapData.length));

    var flow_Data = handler.DoMethodReturnJSON("CCForm_AllElements_ResponseJson");

    // var flow_Data = $.parseJSON(jsonData);

    //循环MapAttr
    for (var idx in flow_Data.Sys_MapAttr) {
        var mapAttr = flow_Data.Sys_MapAttr[idx];
        var createdFigure = figure_MapAttr_Template(mapAttr);
        if (createdFigure != undefined) {
            //move it into position
            //createdFigure.transform(Matrix.translationMatrix(mapAttr.X - createdFigure.rotationCoords[0].x, mapAttr.Y - createdFigure.rotationCoords[0].y))
            createdFigure.style.lineWidth = defaultLineWidth;
            //add to STACK
            STACK.figureAdd(createdFigure);
        }
    }



    ////循环FrmLab
    //for (var i in flow_Data.Sys_FrmLab) {
    //    var frmLab = flow_Data.Sys_FrmLab[i];
    //    var createdFigure = figure_Template_Label(frmLab);
    //    createdFigure.style.lineWidth = defaultLineWidth;
    //    STACK.figureAdd(createdFigure);
    //}

    //循环FrmRB
    for (var i in flow_Data.Sys_FrmRB) {
        var frmRb = flow_Data.Sys_FrmRB[i];
        if (frmRb.AtPara != null && frmRb.AtPara != undefined && frmRb.AtPara.indexOf("@MyDataType=4") != -1)
            continue;
       
        var createdFigure = figure_Template_Rb(frmRb);
        //move it into position
        //createdFigure.transform(Matrix.translationMatrix(frmRb.X - createdFigure.rotationCoords[0].x, frmRb.Y - createdFigure.rotationCoords[0].y))
        createdFigure.style.lineWidth = defaultLineWidth;
        //add to STACK
        STACK.figureAdd(createdFigure);
        //        }
        //        createdFigure = figure_Template_RbLab(frmRb);
        //        //move it into position
        //        //createdFigure.transform(Matrix.translationMatrix(frmRb.X - createdFigure.rotationCoords[0].x, frmRb.Y - createdFigure.rotationCoords[0].y))
        //        createdFigure.style.lineWidth = defaultLineWidth;
        //        //add to STACK
        //        STACK.figureAdd(createdFigure);
    }

    //循环FrmBtn
    for (var i in flow_Data.Sys_FrmBtn) {

        var frmBtn = flow_Data.Sys_FrmBtn[i];

        var createdFigure = figure_Template_Btn(frmBtn);
        //move it into position
        //createdFigure.transform(Matrix.translationMatrix(frmBtn.X - createdFigure.rotationCoords[0].x, frmBtn.Y - createdFigure.rotationCoords[0].y))
        createdFigure.style.lineWidth = defaultLineWidth;
        //add to STACK
        STACK.figureAdd(createdFigure);
    }

    //循环Image
    for (var i in flow_Data.Sys_FrmImg) {
        var frmImg = flow_Data.Sys_FrmImg[i];
        var createdFigure = figure_Template_Image(frmImg);
        //move it into position
        //createdFigure.transform(Matrix.translationMatrix(frmImg.X - createdFigure.rotationCoords[0].x, frmImg.Y - createdFigure.rotationCoords[0].y))
        createdFigure.style.lineWidth = defaultLineWidth;
        //add to STACK
        STACK.figureAdd(createdFigure);
    }

    //循环 Link
    for (var i in flow_Data.Sys_FrmLink) {
        var frmLink = flow_Data.Sys_FrmLink[i];
        var createdFigure = figure_Template_HyperLink(frmLink);
        //move it into position
        //createdFigure.transform(Matrix.translationMatrix(frmLink.X - createdFigure.rotationCoords[0].x, frmLink.Y - createdFigure.rotationCoords[0].y))
        createdFigure.style.lineWidth = defaultLineWidth;
        //add to STACK
        STACK.figureAdd(createdFigure);
    }

    //循环 图片附件
    for (var i in flow_Data.Sys_FrmImgAth) {
        var frmImgAth = flow_Data.Sys_FrmImgAth[i];
        var createdFigure = figure_Template_ImageAth(frmImgAth);
        //move it into position
        //createdFigure.transform(Matrix.translationMatrix(frmImgAth.X - createdFigure.rotationCoords[0].x, frmImgAth.Y - createdFigure.rotationCoords[0].y))
        createdFigure.style.lineWidth = defaultLineWidth;
        //add to STACK
        STACK.figureAdd(createdFigure);
    }
    //循环 附件
    for (var i in flow_Data.Sys_FrmAttachment) {
        var frmAttachment = flow_Data.Sys_FrmAttachment[i];
        if (frmAttachment.IsVisable == 0)
            continue;
        var createdFigure = figure_Template_Attachment(frmAttachment);
        //move it into position
        //createdFigure.transform(Matrix.translationMatrix(frmAttachment.X - createdFigure.rotationCoords[0].x, frmAttachment.Y - createdFigure.rotationCoords[0].y))
        //createdFigure.style.lineWidth = defaultLineWidth;
        //add to STACK
        STACK.figureAdd(createdFigure);
    }

    //循环 从表
    for (var i in flow_Data.Sys_MapDtl) {
        var frmMapDtl = flow_Data.Sys_MapDtl[i];
        var createdFigure = figure_Template_Dtl(frmMapDtl);
        //move it into position
        //createdFigure.transform(Matrix.translationMatrix(frmMapDtl.X - createdFigure.rotationCoords[0].x, frmMapDtl.Y - createdFigure.rotationCoords[0].y))
        createdFigure.style.lineWidth = defaultLineWidth;
        //add to STACK
        STACK.figureAdd(createdFigure);
    }

    ////循环线
    //for (var i in flow_Data.Sys_FrmLine) {
    //    var frmLine = flow_Data.Sys_FrmLine[i];
    //    var createdConnector = connector_Template_Line(frmLine);
    //}


    //循环组件 轨迹图 审核组件 子流程 子线程
    for (var i in flow_Data.FigureCom) {
        var figureCom = flow_Data.FigureCom[i];

        var createdFigure = figure_Template_FigureCom(figureCom);
        if (createdFigure != undefined) {
            STACK.figureAdd(createdFigure);
        }
    }

    //循环MapFrame
    for (var i in flow_Data.Sys_MapFrame) {
        var mapFrame = flow_Data.Sys_MapFrame[i];
        var createdFigure = figure_Template_MapFrame(mapFrame); 
        createdFigure.style.lineWidth = defaultLineWidth;
        STACK.figureAdd(createdFigure);
    }
    redraw = true;
    draw();
    //save(false);
}

//升级表单元素 初始化文本框、日期、时间
function figure_MapAttr_Template(mapAttr) {
    var f = undefined;
    if (mapAttr.UIContralType == 0) {
        f = new Figure("TextBox");
        //控件数据类型
        if (mapAttr.MyDataType == "1") {
            f.CCForm_Shape = "TextBoxStr";
        } else if (mapAttr.MyDataType == "2") {
            f.CCForm_Shape = "TextBoxInt";
        } else if (mapAttr.MyDataType == "3") {
            f.CCForm_Shape = "TextBoxFloat";
        } else if (mapAttr.MyDataType == "4") {
            f.CCForm_Shape = "TextBoxBoolean";
        } else if (mapAttr.MyDataType == "5") {
            f.CCForm_Shape = "TextBoxDouble";
        } else if (mapAttr.MyDataType == "6") {
            f.CCForm_Shape = "TextBoxDate";
        } else if (mapAttr.MyDataType == "7") {
            f.CCForm_Shape = "TextBoxDateTime";
        } else if (mapAttr.MyDataType == "8") {
            f.CCForm_Shape = "TextBoxMoney";
        }
    } else if (mapAttr.UIContralType == 1) {
        f = new Figure("DropDownList");
        //枚举下拉框
        if (mapAttr.LGType == 1) {
            f.CCForm_Shape = "DropDownListEnum";
        } //外键下拉框
        else if (mapAttr.LGType == 2) {
            f.CCForm_Shape = "DropDownListTable";
        }
        //外部数据源
        else if (mapAttr.LGType == 0) {
            f.CCForm_Shape = "DropDownListTable";
        }
    } else if (mapAttr.UIContralType == 2) {//复选框
        f = new Figure("TextBox");
        f.CCForm_Shape = "TextBoxBoolean";
    } else if (mapAttr.UIContralType == 3) {//单选妞
        return;
    } else if (mapAttr.UIContralType == 8) {//签字版
        f = new Figure("TextBox");
        f.CCForm_Shape = "HandSiganture";
    }
    else {
        alert(mapAttr)

    }

    f.name = f.CCForm_Shape;

    f.CCForm_MyPK = mapAttr.KeyOfEn;
    f.style.fillStyle = FigureDefaults.fillStyle;
    f.style.strokeStyle = FigureDefaults.strokeStyle;
    f.properties.push(new BuilderProperty('控件属性-' + f.CCForm_Shape, 'group', BuilderProperty.TYPE_GROUP_LABEL));
    f.properties.push(new BuilderProperty(BuilderProperty.SEPARATOR));

    for (var i = 0; i < CCForm_Control_Propertys[f.CCForm_Shape].length; i++) {
        var property = CCForm_Control_Propertys[f.CCForm_Shape][i];
        var propertyVale = mapAttr[property.proName];
        if (property.proName == 'FieldText') {
            propertyVale = mapAttr["Name"];
        }
        if (propertyVale == undefined) {
            propertyVale = property.DefVal;
        }

        //if (property.proName == "AutoFullDLL" || property.proName == "ActiveDDL" || property.proName == "DDLFullCtrl") {
        if (propertyVale != null && propertyVale != "" && !isNumeric(propertyVale)) {
            propertyVale = propertyVale.replace(new RegExp(/@FrmID@/g), mapAttr.FK_MapData);
            propertyVale = propertyVale.replace(new RegExp(/@KeyOfEn@/g), mapAttr.KeyOfEn);
        }
        //}
        f.properties.push(new BuilderProperty(property.ProText, property.proName, property.ProType, propertyVale));
    }

    //Image
    var url = figureSetsURL + "/DataView/" + f.CCForm_Shape + ".png";

    var ifig = new ImageFrame(url, mapAttr.X + mapAttr.UIWidth / 2, mapAttr.Y + mapAttr.UIHeight / 2, true, mapAttr.UIWidth, mapAttr.UIHeight);
    ifig.debug = true;
    f.addPrimitive(ifig);

    if (f.CCForm_Shape == "TextBoxBoolean") {
        //var t2 = new Text(mapAttr.Name, mapAttr.X + mapAttr.UIWidth / 2 + FigureDefaults.radiusSize / 2, mapAttr.Y + FigureDefaults.radiusSize / 2 + mapAttr.UIHeight / 2, FigureDefaults.textFont, FigureDefaults.textSize);
        // 导入表单checkbox与label错位
        var t2 = new Text(mapAttr.Name, mapAttr.X + mapAttr.UIWidth / 2 + FigureDefaults.radiusSize / 2, mapAttr.Y + mapAttr.UIHeight / 2, FigureDefaults.textFont, FigureDefaults.textSize);
        t2.style.fillStyle = FigureDefaults.textColor;
        f.addPrimitive(t2);
    }

    f.gradientBounds = [mapAttr.X, mapAttr.Y,
        mapAttr.X + mapAttr.UIWidth,
        mapAttr.Y + mapAttr.UIHeight];


    f.finalise();
    return f;
}

//升级表单元素 初始化Label
function figure_Template_Label(frmLab) {

    var f = new Figure('Label');
    f.CCForm_Shape = "Label";

    f.CCForm_MyPK = frmLab.MyPK;


    f.name = "Label";
    var x = frmLab.X;
    var y = frmLab.Y;
    var fontColor = frmLab.FontColor;
    if (fontColor.indexOf('#') == 0 && fontColor.length == 9) {
        fontColor = '#' + fontColor.substr(3);
    }

    if (frmLab.IsBold == 1) {
        frmLab.FontWeight = "bold";
    } else {
        frmLab.FontWeight = "normal";
    }

    f.properties.push(new BuilderProperty('基本属性', 'group', BuilderProperty.TYPE_GROUP_LABEL));
    f.properties.push(new BuilderProperty(BuilderProperty.SEPARATOR));
    f.properties.push(new BuilderProperty('文本', 'primitives.0.str', BuilderProperty.TYPE_SINGLE_TEXT, frmLab.Lab));
    f.properties.push(new BuilderProperty('字体大小', 'primitives.0.size', BuilderProperty.TYPE_TEXT_FONT_SIZE, frmLab.FontSize));
    f.properties.push(new BuilderProperty('字体', 'primitives.0.font', BuilderProperty.TYPE_TEXT_FONT_FAMILY, frmLab.FontName));
    f.properties.push(new BuilderProperty('对齐', 'primitives.0.align', BuilderProperty.TYPE_TEXT_FONT_ALIGNMENT));
    f.properties.push(new BuilderProperty('下划线', 'primitives.0.underlined', BuilderProperty.TYPE_TEXT_UNDERLINED));
    f.properties.push(new BuilderProperty('字体加粗', 'primitives.0.fontWeight', BuilderProperty.TYPE_TEXT_FONTWEIGHT, frmLab.FontWeight));
    f.properties.push(new BuilderProperty('字体颜色', 'primitives.0.style.fillStyle', BuilderProperty.TYPE_COLOR, fontColor));
    //var t2 = new Text(labelText, frmLab.X +  FigureDefaults.radiusSize / 2, frmLab.Y + FigureDefaults.radiusSize / 2, FigureDefaults.textFont, FigureDefaults.textSize);
    var labelText = frmLab.Lab.replace(new RegExp(/@/g), "\n");
    labelText = labelText.replace(new RegExp(/&nbsp;/g), " ");
    var x = (frmLab.FontSize * getXByteLen(labelText)) / 4 + frmLab.X;

    //修改表单设计偏移的问题
    y = (frmLab.FontSize * getYByteLen(labelText)) / 2 + frmLab.Y;
    t2 = new Text(labelText, x, y, frmLab.FontName, frmLab.FontSize);

    t2.style.fillStyle = fontColor//frmLab.FontColor;
    t2.size = frmLab.FontSize;
    t2.font = frmLab.FontName;
    t2.fontWeight = frmLab.FontWeight;
    t2.align = "left";
    f.addPrimitive(t2);

    f.gradientBounds = [frmLab.X, frmLab.Y,
        frmLab.X + (frmLab.FontSize * getXByteLen(labelText) / 2) + 50,
        frmLab.Y + ((frmLab.FontSize + 5) * getYByteLen(labelText)) + 50];
    f.finalise();
    return f;
}

//初始化按钮
function figure_Template_Btn(frmBtn) {
    var f = new Figure("Button");
    f.CCForm_MyPK = frmBtn.MyPK;
    //ccform Property
    f.CCForm_Shape = CCForm_Controls.Button;

    f.style.strokeStyle = "#c0c0c0";
    f.style.lineWidth = 2;

    f.properties.push(new BuilderProperty('基本属性-Button', 'group', BuilderProperty.TYPE_GROUP_LABEL));
    f.properties.push(new BuilderProperty(BuilderProperty.SEPARATOR));
    f.properties.push(new BuilderProperty('字体大小', 'primitives.1.size', BuilderProperty.TYPE_TEXT_FONT_SIZE));
    f.properties.push(new BuilderProperty('字体', 'primitives.1.font', BuilderProperty.TYPE_TEXT_FONT_FAMILY));
    f.properties.push(new BuilderProperty('字体加粗', 'primitives.1.fontWeight', BuilderProperty.TYPE_TEXT_FONTWEIGHT));
    f.properties.push(new BuilderProperty('字体颜色', 'primitives.1.style.fillStyle', BuilderProperty.TYPE_COLOR));

    f.properties.push(new BuilderProperty('控件属性', 'group', BuilderProperty.TYPE_GROUP_LABEL));
    f.properties.push(new BuilderProperty(BuilderProperty.SEPARATOR));
    f.properties.push(new BuilderProperty('按钮标签', 'primitives.1.str', BuilderProperty.TYPE_SINGLE_TEXT, frmBtn.Lab));
    f.properties.push(new BuilderProperty('按钮事件', 'ButtonEvent', BuilderProperty.CCFormEnum, frmBtn.EventType));
    f.properties.push(new BuilderProperty('事件内容', 'BtnEventDoc', BuilderProperty.TYPE_TEXT, frmBtn.EventContext));

    var x = frmBtn.X;
    var y = frmBtn.Y;

    var p = new Path();
    var hShrinker = 12;
    var vShrinker = 15;

    var l1 = new Line(new Point(x + hShrinker, y + vShrinker),
        new Point(x + FigureDefaults.segmentSize - hShrinker, y + vShrinker));

    var c1 = new QuadCurve(new Point(x + FigureDefaults.segmentSize - hShrinker, y + vShrinker),
        new Point(x + FigureDefaults.segmentSize - hShrinker + FigureDefaults.corner * (FigureDefaults.cornerRoundness / 10), y + FigureDefaults.corner / FigureDefaults.cornerRoundness + vShrinker),
        new Point(x + FigureDefaults.segmentSize - hShrinker + FigureDefaults.corner, y + FigureDefaults.corner + vShrinker))

    var l2 = new Line(new Point(x + FigureDefaults.segmentSize - hShrinker + FigureDefaults.corner, y + FigureDefaults.corner + vShrinker),
        new Point(x + FigureDefaults.segmentSize - hShrinker + FigureDefaults.corner, y + FigureDefaults.corner + FigureDefaults.segmentShortSize - vShrinker));

    var c2 = new QuadCurve(new Point(x + FigureDefaults.segmentSize - hShrinker + FigureDefaults.corner, y + FigureDefaults.corner + FigureDefaults.segmentShortSize - vShrinker),
        new Point(x + FigureDefaults.segmentSize - hShrinker + FigureDefaults.corner * (FigureDefaults.cornerRoundness / 10), y + FigureDefaults.corner + FigureDefaults.segmentShortSize - vShrinker + FigureDefaults.corner * (FigureDefaults.cornerRoundness / 10)),
        new Point(x + FigureDefaults.segmentSize - hShrinker, y + FigureDefaults.corner + FigureDefaults.segmentShortSize - vShrinker + FigureDefaults.corner))

    var l3 = new Line(new Point(x + FigureDefaults.segmentSize - hShrinker, y + FigureDefaults.corner + FigureDefaults.segmentShortSize - vShrinker + FigureDefaults.corner),
        new Point(x + hShrinker, y + FigureDefaults.corner + FigureDefaults.segmentShortSize - vShrinker + FigureDefaults.corner));

    var c3 = new QuadCurve(
        new Point(x + hShrinker, y + FigureDefaults.corner + FigureDefaults.segmentShortSize - vShrinker + FigureDefaults.corner),
        new Point(x + hShrinker - FigureDefaults.corner * (FigureDefaults.cornerRoundness / 10), y + FigureDefaults.corner + FigureDefaults.segmentShortSize - vShrinker + FigureDefaults.corner * (FigureDefaults.cornerRoundness / 10)),
        new Point(x + hShrinker - FigureDefaults.corner, y + FigureDefaults.corner + FigureDefaults.segmentShortSize - vShrinker))

    var l4 = new Line(new Point(x + hShrinker - FigureDefaults.corner, y + FigureDefaults.corner + FigureDefaults.segmentShortSize - vShrinker),
        new Point(x + hShrinker - FigureDefaults.corner, y + FigureDefaults.corner + vShrinker));

    var c4 = new QuadCurve(
        new Point(x + hShrinker - FigureDefaults.corner, y + FigureDefaults.corner + vShrinker),
        new Point(x + hShrinker - FigureDefaults.corner * (FigureDefaults.cornerRoundness / 10), y + vShrinker),
        new Point(x + hShrinker, y + vShrinker))

    p.addPrimitive(l1);
    p.addPrimitive(c1);
    p.addPrimitive(l2);
    p.addPrimitive(c2);
    p.addPrimitive(l3);
    p.addPrimitive(c3);
    p.addPrimitive(l4);
    p.addPrimitive(c4);
    f.addPrimitive(p);


    var t2 = new Text(frmBtn.Lab, x + FigureDefaults.segmentSize / 2, y + FigureDefaults.segmentShortSize / 2 + FigureDefaults.corner, FigureDefaults.textFont, FigureDefaults.textSize);
    //修改偏移的问题
    // x = t2.getNormalWidth() / 2 + frmBtn.X;
    //y = t2.getNormalHeight() / 2 + frmBtn.Y;
    //t2 = new Text(frmBtn.Lab, x, y, FigureDefaults.textFont, FigureDefaults.textSize);

    t2.style.fillStyle = FigureDefaults.textColor;

    f.addPrimitive(t2);



    f.finalise();
    return f;
}

//初始化单选按钮
function figure_Template_Rb(frmRb) {
    //需要初始化3个东西  1个TextBox 1个 Label
    var f = new Figure("RadioButton");
    f.CCForm_Shape = "RadioButton";
    f.name = 'TextBox';
    f.CCForm_MyPK = frmRb.EnumKey + "_" + frmRb.IntKey;

    f.style.fillStyle = FigureDefaults.fillStyle;
    f.style.strokeStyle = FigureDefaults.strokeStyle;
    f.properties.push(new BuilderProperty('控件属性-' + f.CCForm_Shape, 'group', BuilderProperty.TYPE_GROUP_LABEL));
    f.properties.push(new BuilderProperty(BuilderProperty.SEPARATOR));
    for (var i = 0; i < CCForm_Control_Propertys[f.CCForm_Shape].length; i++) {
        var property = CCForm_Control_Propertys[f.CCForm_Shape][i];
        var propertyVale = frmRb[property.proName];
        if (property.proName == 'FieldText') {
            propertyVale = frmRb["Lab"];
        }
        if (property.proName == 'No') {
            propertyVale = frmRb["CtrlID"];
        }
        if (property.proName == 'UIBindKey') {
            propertyVale = frmRb["EnumKey"];
        }

        if (propertyVale == undefined) {
            propertyVale = frmRb["Lab"];
        }

        if (property.proName == "AutoFullDLL" || property.proName == "ActiveDDL" || property.proName == "DDLFullCtrl") {
            propertyVale = propertyVale.replace(new RegExp(/@FrmID@/g), frmRb.FK_MapData);
            propertyVale = propertyVale.replace(new RegExp(/@KeyOfEn@/g), frmRb.KeyOfEn);
        }
        f.properties.push(new BuilderProperty(property.ProText, property.proName, property.ProType, propertyVale));
    }


    //获取它的mapattr
    var mapAttr = new Entity("BP.Sys.MapAttr", frmRb.FK_MapData + "_" + frmRb.KeyOfEn);
    var t2;
    if (mapAttr.GetPara("RBShowModel") == "3")
        t2 = new Text("*" + frmRb.Lab, frmRb.X + FigureDefaults.radiusSize-1 , frmRb.Y + FigureDefaults.textSize / 2, FigureDefaults.textFont, FigureDefaults.textSize);
    else
        t2 = new Text("*" + frmRb.Lab, frmRb.X + FigureDefaults.radiusSize/2, frmRb.Y + FigureDefaults.textSize / 2, FigureDefaults.textFont, FigureDefaults.textSize);
    t2.style.fillStyle = FigureDefaults.textColor;
    f.addPrimitive(t2);

    f.gradientBounds = [100, 100, 200, 200];

    f.finalise();
    return f;
}

//初始化单选按钮
function figure_Template_RbLab(frmRb) {
    var f = new Figure('Label');
    //ccform Property
    f.CCForm_Shape = "Label";
    f.style.fillStyle = FigureDefaults.fillStyle;


    f.name = "Label";
    var x = frmRb.X;
    var y = frmRb.Y;
    f.properties.push(new BuilderProperty('基本属性', 'group', BuilderProperty.TYPE_GROUP_LABEL));
    f.properties.push(new BuilderProperty(BuilderProperty.SEPARATOR));
    f.properties.push(new BuilderProperty('文本', 'primitives.0.str', BuilderProperty.TYPE_SINGLE_TEXT, frmRb.Text));
    f.properties.push(new BuilderProperty('字体大小', 'primitives.0.size', BuilderProperty.TYPE_TEXT_FONT_SIZE, frmRb.FontSize));
    f.properties.push(new BuilderProperty('字体', 'primitives.0.font', BuilderProperty.TYPE_TEXT_FONT_FAMILY, frmRb.FontName));
    //f.properties.push(new BuilderProperty('对齐', 'primitives.0.align', BuilderProperty.TYPE_TEXT_FONT_ALIGNMENT));
    f.properties.push(new BuilderProperty('下划线', 'primitives.0.underlined', BuilderProperty.TYPE_TEXT_UNDERLINED));
    f.properties.push(new BuilderProperty('字体加粗', 'primitives.0.fontWeight', BuilderProperty.TYPE_TEXT_FONTWEIGHT, frmRb.IsBold));
    f.properties.push(new BuilderProperty('字体颜色', 'primitives.0.style.fillStyle', BuilderProperty.TYPE_COLOR, frmRb.FontColor));

    var t2 = new Text("*" + frmRb.Lab, frmRb.X + FigureDefaults.radiusSize / 2, frmRb.Y + FigureDefaults.radiusSize / 2, FigureDefaults.textFont, FigureDefaults.textSize);
    t2.style.fillStyle = FigureDefaults.textColor;

    f.addPrimitive(t2);

    f.finalise();
    return f;
}

//初始化超链接
function figure_Template_HyperLink(frmLin) {
    var f = new Figure('HyperLink');
    //ccform Property
    f.CCForm_MyPK = frmLin.MyPK;
    f.CCForm_Shape = CCForm_Controls.HyperLink;
    f.style.fillStyle = FigureDefaults.fillStyle;

    f.properties.push(new BuilderProperty('基本属性-HyperLink', 'group', BuilderProperty.TYPE_GROUP_LABEL));
    f.properties.push(new BuilderProperty(BuilderProperty.SEPARATOR));
    f.properties.push(new BuilderProperty('文本', 'primitives.0.str', BuilderProperty.TYPE_SINGLE_TEXT, frmLin.Lab));
    f.properties.push(new BuilderProperty('字体大小', 'primitives.0.size', BuilderProperty.TYPE_TEXT_FONT_SIZE, frmLin.FontSize));
    f.properties.push(new BuilderProperty('字体', 'primitives.0.font', BuilderProperty.TYPE_TEXT_FONT_FAMILY, frmLin.FontName));
    //f.properties.push(new BuilderProperty('对齐', 'primitives.0.align', BuilderProperty.TYPE_TEXT_FONT_ALIGNMENT));
    f.properties.push(new BuilderProperty('下划线', 'primitives.0.underlined', BuilderProperty.TYPE_TEXT_UNDERLINED));
    f.properties.push(new BuilderProperty('字体加粗', 'primitives.0.fontWeight', BuilderProperty.TYPE_TEXT_FONTWEIGHT, frmLin.IsBold));
    f.properties.push(new BuilderProperty('字体颜色', 'primitives.0.style.fillStyle', BuilderProperty.TYPE_COLOR, frmLin.FontColor));

    f.properties.push(new BuilderProperty('控件属性', 'group', BuilderProperty.TYPE_GROUP_LABEL));
    f.properties.push(new BuilderProperty(BuilderProperty.SEPARATOR));
    f.properties.push(new BuilderProperty('连接地址', 'URL', BuilderProperty.TYPE_SINGLE_TEXT, frmLin.URL));
    f.properties.push(new BuilderProperty('打开窗口', 'WinOpenModel', BuilderProperty.CCFormEnum, frmLin.Target));

    var x = frmLin.X;
    var y = frmLin.Y;

    var t2 = new Text(frmLin.Lab, x + FigureDefaults.radiusSize / 2, y + FigureDefaults.radiusSize / 2, FigureDefaults.textFont, FigureDefaults.textSize);

    //修改偏移的问题
    x = t2.getNormalWidth() / 2 + x;
    y = t2.getNormalHeight() / 2 + y;

    t2 = new Text(frmLin.Lab, x, y, FigureDefaults.textFont, FigureDefaults.textSize);
    t2.style.fillStyle = "#0000ff";
    t2.underlined = true;

    f.addPrimitive(t2);

    f.finalise();
    return f;
}

//初始化 IMAGE
function figure_Template_Image(frmImage) {
    var f = new Figure("Image");
    //ccform Property
    f.CCForm_Shape = CCForm_Controls.Image;
    f.CCForm_MyPK = frmImage.MyPK;
    f.style.fillStyle = FigureDefaults.fillStyle;
    f.style.strokeStyle = FigureDefaults.strokeStyle;

    //Image
    var url = figureSetsURL + "/basic/TempleteFile.png";
    var x = frmImage.X + frmImage.W / 2;
    var y = frmImage.Y + frmImage.H / 2;
    var ifig = new ImageFrame(url, x, y, true, frmImage.W, frmImage.H);
    ifig.debug = true;
    f.addPrimitive(ifig);

    //Text
    f.properties.push(new BuilderProperty('控件属性-Image', 'group', BuilderProperty.TYPE_GROUP_LABEL));
    f.properties.push(new BuilderProperty(BuilderProperty.SEPARATOR));
    f.properties.push(new BuilderProperty('应用类型', 'ImgAppType', BuilderProperty.CCFormEnum, frmImage.ImageAppType));
    f.properties.push(new BuilderProperty('上传图片', 'ImgURL', BuilderProperty.CCFormUpload, frmImage.ImageURL));
    f.properties.push(new BuilderProperty('指定路径', 'ImgPath', BuilderProperty.TYPE_SINGLE_TEXT, frmImage.ImgPath));
    f.properties.push(new BuilderProperty('图片连接到', 'LinkURL', BuilderProperty.TYPE_SINGLE_TEXT, frmImage.LinkURL));
    f.properties.push(new BuilderProperty('打开窗口', 'WinOpenModel', BuilderProperty.CCFormEnum, frmImage.LinkTarget));

    //var x = frmImage.X;
    //var y = frmImage.Y;
    //var t2 = new Text("", x, y + 36, FigureDefaults.textFont, FigureDefaults.textSize);
    //t2.style.fillStyle = FigureDefaults.textColor;
    //f.addPrimitive(t2);

    f.finalise();
    return f;
}

//初始化 IMAGE
function figure_Template_ImageAth(frmImageAth) {
    var f = new Figure("AthImg");
    //ccform Property
    f.CCForm_Shape = CCForm_Controls.AthImg;
    f.CCForm_MyPK = frmImageAth.CtrlID;
    f.style.fillStyle = FigureDefaults.fillStyle;
    f.style.strokeStyle = FigureDefaults.strokeStyle;

    //Image
    var url = figureSetsURL + "/DataView/AthImg.png";
    var x = frmImageAth.X + frmImageAth.W / 2;
    var y = frmImageAth.Y + frmImageAth.H / 2;
    var ifig = new ImageFrame(url, x, y, true, frmImageAth.W, frmImageAth.H);
    ifig.debug = true;
    f.addPrimitive(ifig);

    //Text
    //f.properties.push(new BuilderProperty('控件属性-Image', 'group', BuilderProperty.TYPE_GROUP_LABEL));
    //f.properties.push(new BuilderProperty(BuilderProperty.SEPARATOR));
    //f.properties.push(new BuilderProperty('应用类型', 'ImgAppType', BuilderProperty.CCFormEnum, frmImage.ImageAppType));
    //f.properties.push(new BuilderProperty('上传图片', 'ImgURL', BuilderProperty.CCFormUpload, frmImage.ImageURL));
    //f.properties.push(new BuilderProperty('指定路径', 'ImgPath', BuilderProperty.TYPE_SINGLE_TEXT, frmImage.ImgPath));
    //f.properties.push(new BuilderProperty('图片连接到', 'LinkURL', BuilderProperty.TYPE_SINGLE_TEXT, frmImage.LinkURL));
    //f.properties.push(new BuilderProperty('打开窗口', 'WinOpenModel', BuilderProperty.CCFormEnum, frmImage.LinkTarget));
    for (var i = 0; i < CCForm_Control_Propertys[f.CCForm_Shape].length; i++) {
        var property = CCForm_Control_Propertys[f.CCForm_Shape][i];
        var propertyVale = frmImageAth[property.proName];
        if (property.proName == 'FieldText') {
            propertyVale = frmImageAth["Name"];
        }
        if (property.proName == 'No') {
            propertyVale = frmImageAth["CtrlID"];
        }
        if (property.proName == 'Name') {
            propertyVale = frmImageAth["CtrlID"];
        }
        if (propertyVale == undefined) {
            propertyVale = property.DefVal;
        }

        f.properties.push(new BuilderProperty(property.ProText, property.proName, property.ProType, propertyVale));
    }


    f.finalise();
    return f;
}

//初始化 附件
function figure_Template_Attachment(frmAttachment) {
    var f = undefined;
    if (frmAttachment.UploadType == 0) {
        f = new Figure("AthSingle");
    } else if (frmAttachment.UploadType == 1) {
        f = new Figure("AthMulti");
    }
    //ccform Property
    if (frmAttachment.UploadType == 0) {
        f.CCForm_Shape = CCForm_Controls.AthSingle;
    } else if (frmAttachment.UploadType == 1) {
        f.CCForm_Shape = CCForm_Controls.AthMulti;
    }
    f.name = "Square";
    f.CCForm_MyPK = frmAttachment.NoOfObj;
    f.style.fillStyle = FigureDefaults.fillStyle;
    f.style.strokeStyle = FigureDefaults.strokeStyle;

    f.properties.push(new BuilderProperty('控件属性-' + f.CCForm_Shape, 'group', BuilderProperty.TYPE_GROUP_LABEL));
    f.properties.push(new BuilderProperty(BuilderProperty.SEPARATOR));
    for (var i = 0; i < CCForm_Control_Propertys[f.CCForm_Shape].length; i++) {
        var property = CCForm_Control_Propertys[f.CCForm_Shape][i];
        var propertyVale = frmAttachment[property.proName];
        if (property.proName == 'FieldText') {
            propertyVale = frmAttachment["Name"];
        }
        if (property.proName == 'No') {
            propertyVale = frmAttachment["NoOfObj"];
        }
        if (propertyVale == undefined) {
            propertyVale = property.DefVal;
        }

        if (property.proName == "Set") {
            propertyVale = propertyVale.replace("@FrmID@", frmAttachment.FK_MapData);
            propertyVale = propertyVale.replace("@KeyOfEn@", frmAttachment.NoOfObj);
        }

        f.properties.push(new BuilderProperty(property.ProText, property.proName, property.ProType, propertyVale));
    }

    //Image
    var url = undefined;
    var x = frmAttachment.X + frmAttachment.W / 2;
    var y = frmAttachment.Y + frmAttachment.H / 2;

    if (frmAttachment.UploadType == 0) {
        url = figureSetsURL + "/DataView/AthSingle.png";
    } else if (frmAttachment.UploadType == 1) {
        url = figureSetsURL + "/DataView/AthMulti.png";
    }
    var ifig = new ImageFrame(url, x, y, true, frmAttachment.W, frmAttachment.H);
    ifig.debug = true;
    f.addPrimitive(ifig);
    //Text

    //var t2 = new Text(frmAttachment.Name, x + FigureDefaults.radiusSize / 2 + frmAttachment.W / 2, y + FigureDefaults.radiusSize / 2 + frmAttachment.H / 2, FigureDefaults.textFont, FigureDefaults.textSize);
    //var t2 = new Text(frmAttachment.Name, x, y, FigureDefaults.textFont, FigureDefaults.textSize);
    //t2.style.fillStyle = FigureDefaults.textColor;
    //f.addPrimitive(t2);


    f.gradientBounds = [frmAttachment.X + FigureDefaults.radiusSize / 2, frmAttachment.Y + FigureDefaults.radiusSize / 2, frmAttachment.W + frmAttachment.X, frmAttachment.H + frmAttachment.Y];
    f.finalise();
    return f;
}

//初始化从表
function figure_Template_Dtl(frmDtl) {
    var f = new Figure("Dtl");
    //ccform Property
    f.CCForm_Shape = CCForm_Controls.Dtl;
    f.name = "TextBox";

    f.CCForm_MyPK = frmDtl.No;
    f.style.fillStyle = FigureDefaults.fillStyle;
    f.style.strokeStyle = FigureDefaults.strokeStyle;



    f.properties.push(new BuilderProperty('控件属性-Dtl', 'group', BuilderProperty.TYPE_GROUP_LABEL));
    f.properties.push(new BuilderProperty(BuilderProperty.SEPARATOR));
    for (var i = 0; i < CCForm_Control_Propertys[f.CCForm_Shape].length; i++) {
        var property = CCForm_Control_Propertys[f.CCForm_Shape][i];
        var propertyVale = frmDtl[property.proName];

        if (propertyVale == undefined) {
            propertyVale = property.DefVal;
        }

        if (property.proName == "Set") {
            propertyVale = propertyVale.replace("@FrmID@", frmDtl.FK_MapData);
            propertyVale = propertyVale.replace("@KeyOfEn@", frmDtl.No);
        }

        f.properties.push(new BuilderProperty(property.ProText, property.proName, property.ProType, propertyVale));
    }

    //Image
    var url = figureSetsURL + "/DataView/Dtl.png";
    var x = frmDtl.X + frmDtl.W / 2;
    var y = frmDtl.Y + frmDtl.H / 2;
    var ifig = new ImageFrame(url, x, y, true, frmDtl.W, frmDtl.H);
    ifig.debug = true;
    f.addPrimitive(ifig);
    //Text
    //var t2 = new Text(frmDtl.Name, x + frmDtl.W / 2 + FigureDefaults.radiusSize / 2, y + frmDtl.H / 2 + FigureDefaults.radiusSize / 2, FigureDefaults.textFont, FigureDefaults.textSize);
    //t2.style.fillStyle = FigureDefaults.textColor;
    //f.addPrimitive(t2);

    f.finalise();
    return f;
}

//初始化框架
function figure_Template_MapFrame(mapFrame) {
    var f = new Figure("MapFrame");
    //ccform Property
    f.CCForm_Shape = "iFrame";
    f.name = "TextBox";

    f.CCForm_MyPK = mapFrame.MyPK;
    f.style.fillStyle = FigureDefaults.fillStyle;
    f.style.strokeStyle = FigureDefaults.strokeStyle;



    f.properties.push(new BuilderProperty('控件属性-FrmEle', 'group', BuilderProperty.TYPE_GROUP_LABEL));
    f.properties.push(new BuilderProperty(BuilderProperty.SEPARATOR));
    for (var i = 0; i < CCForm_Control_Propertys[f.CCForm_Shape].length; i++) {
        var property = CCForm_Control_Propertys[f.CCForm_Shape][i];
        var propertyVale = mapFrame[property.proName];

        if (propertyVale == undefined) {
            propertyVale = property.DefVal;
        }

        if (property.proName == "Set") {
            propertyVale = propertyVale.replace("@FrmID@", mapFrame.FK_MapData);
            propertyVale = propertyVale.replace("@KeyOfEn@", mapFrame.MyPK);
        }

        f.properties.push(new BuilderProperty(property.ProText, property.proName, property.ProType, propertyVale));
    }

    //Image
    var url = figureSetsURL + "/DataView/Dtl.png";
    var x = parseFloat(mapFrame.X) + parseFloat(mapFrame.W / 2);
    var y = parseFloat(mapFrame.Y) + parseFloat(mapFrame.H / 2);
    var ifig = new ImageFrame(url, x, y, true, mapFrame.W, mapFrame.H);
    ifig.debug = true;
    f.addPrimitive(ifig);

    f.finalise();
    return f;
}


//初始化轨迹图 审核组件 子流程 子线程
function figure_Template_FigureCom(figureCom) {

    if (figureCom.Sta == null || figureCom.Sta == 0) {//未启用该组件
        return;
    }
    var f = new Figure(figureCom.No);
    var figureName = figureCom.No;
    //ccform Property
    f.CCForm_Shape = figureName;
    f.name = figureName;

    f.CCForm_MyPK = figureName;
    f.style.fillStyle = FigureDefaults.fillStyle;
    f.style.strokeStyle = FigureDefaults.strokeStyle;



    f.properties.push(new BuilderProperty('控件属性-' + figureCom.Name, 'group', BuilderProperty.TYPE_GROUP_LABEL));
    f.properties.push(new BuilderProperty(BuilderProperty.SEPARATOR));
    for (var i = 0; i < CCForm_Control_Propertys[f.CCForm_Shape].length; i++) {
        var property = CCForm_Control_Propertys[f.CCForm_Shape][i];
        var propertyVale = figureCom[property.proName];

        if (propertyVale == undefined) {
            propertyVale = property.DefVal;
        }

        if (property.proName == "Set") {
            propertyVale = propertyVale.replace("@FrmID@", figureCom.FK_MapData);
            propertyVale = propertyVale.replace("@KeyOfEn@", figureCom.No);
        }

        f.properties.push(new BuilderProperty(property.ProText, property.proName, property.ProType, propertyVale));
    }

    //Image
    var url = figureSetsURL + "/DataView/" + f.CCForm_Shape + ".png";
    var x = figureCom.X + figureCom.W / 2;
    var y = figureCom.Y + figureCom.H / 2;
    var ifig = new ImageFrame(url, x, y, true, figureCom.W, figureCom.H);
    ifig.debug = true;
    f.addPrimitive(ifig);
    //Text
    //var t2 = new Text(figureCom.Name, x + figureCom.W / 2 + FigureDefaults.radiusSize / 2, y + figureCom.H / 2 + FigureDefaults.radiusSize / 2, FigureDefaults.textFont, FigureDefaults.textSize);
    //t2.style.fillStyle = FigureDefaults.textColor;
    //f.addPrimitive(t2);

    f.finalise();
    return f;
}

//初始化线
function connector_Template_Line(frmLine) {
    var startPoint = new Point(frmLine.X1, frmLine.Y1);
    var endPoint = new Point(frmLine.X2, frmLine.Y2);

    //var startPoint = new Point(80, 20);
    //var endPoint = new Point(80, 500);
    var connector = new Connector(startPoint, endPoint, Connector.TYPE_STRAIGHT);
    connector.CCForm_Shape = "Connector";
    connector.CCForm_MyPK = frmLine.MyPK;

    var cId = CONNECTOR_MANAGER.connectorCreate(startPoint, endPoint, Connector.TYPE_STRAIGHT);
    var cps = CONNECTOR_MANAGER.connectionPointGetAllByParent(cId);

    var connectorCreate = CONNECTOR_MANAGER.connectorGetById(cId);
    connectorCreate.properties[2].PropertyValue = frmLine.BorderWidth;
    connectorCreate.properties[3].PropertyValue = frmLine.BorderColor;
    //draw();
    //connector.properties.push(new BuilderProperty('粗细', "LineWidth", 'style.lineWidth',frmLine.BorderWidth));
    //connector.properties.push(new BuilderProperty('颜色', "Color", 'style.strokeStyle', frmLine.Color));

}

function getXByteLen(valObj) {
    var valArr = valObj.split('\n');
    var resultLen = 0;
    var len = 0;
    for (var j = 0; j < valArr.length; j++) {
        var val = valArr[j];
        for (var i = 0; i < val.length; i++) {
            var length = val.charCodeAt(i);
            if (length >= 0 && length <= 128) {
                len += 1;
            }
            else {
                len += 2;
            }
        }
        if (len > resultLen) {
            resultLen = len;
        }
        len = 0;
    }
    return resultLen;
}

function getYByteLen(valObj) {
    var valArr = valObj.split('\n');
    return valArr.length;
}
