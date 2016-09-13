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
    CanvasProps.DEFAULT_WIDTH = 900;

    //验证登录用户
    checklogin();

    //初始化画板
    init(CCForm_FK_MapData);
    //显示网格
    showGrid();
    //右键菜单
    InitContexMenu();
    //初始节点元素
    buildPanel();
    //设置属性高度
    ReSetEditDivCss();
});

//初始化右键菜单
function InitContexMenu() {
    //画板右键
    $("#a").bind('contextmenu', function (ev) {
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
                    var figure = STACK.figureGetById(fId);
                    var tId = STACK.textGetByFigureXY(fId, x, y);
                    // if we clicked text primitive inside of figure
                    if (tId !== -1) {
                        textPrimitiveId = tId;
                        $('#textMenu').menu({ onShow: function () {
                            $("#HD_BPMN_NodeID").val("");
                            $("#HD_BPMN_FigureID").val(fId);
                        }, onClick: TextProperty_Funs
                        });
                        //弹出右键菜单
                        ev.preventDefault();
                        $('#textMenu').menu('show', {
                            left: ev.pageX,
                            top: ev.pageY
                        });
                    }
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
        } (setName), false);

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
            var eFigure = document.createElement('li');
            var eFigure_div = document.createElement('div');
            eFigure_div.setAttribute('id', figure.name);
            eFigure_div.style.backgroundImage = "url(" + figureThumbURL + ")";

            var eFigure_span = document.createElement('span');
            eFigure_span.className = "navlistspan";
            //eFigure_span.innerHTML = figure.image;

            eFigure_div.appendChild(eFigure_span);
            eFigure.appendChild(eFigure_div);

            eFigure.addEventListener('mousedown', function (figureFunction, figureThumbURL, figureName) {
                return function (evt) {
                    evt.preventDefault();
                    createFigure(window[figureFunction], figureThumbURL, figureName);
                };
            } (figureFunctionName, figureThumbURL, figureName), false);

            //in case use drops the figure
            eFigure.addEventListener('mouseup', function () {
                createFigureFunction = null;
                selectedFigureThumb = null;
                state = STATE_NONE;
            }, false);

            eFigure.style.cursor = 'pointer';
            eFigure.style.marginRight = '5px';
            eFigure.style.marginTop = '2px';

            figures_UL.appendChild(eFigure);
        }
    }
}

function TextProperty_Funs(item) {
    var figureId = $("#HD_BPMN_FigureID").val();

    //根据事件名称进行执行
    switch (item.name) {
        case "text_edit": //编辑文本
            var figure = STACK.figureGetById(figureId);
            // check if we clicked a text primitive inside of shape
            // deselect current figure
            selectedFigureId = -1;
            // deselect current container
            selectedContainerId = -1;
            // deselect current connector
            selectedConnectorId = -1;
            // set current state
            state = STATE_TEXT_EDITING;
            // set up text editor
            setUpTextEditorPopup(figure, 0);
            redraw = true;
            draw();
            break;
        case "text_delete": //删除文本
            var cmdDelFig = new FigureDeleteCommand(figureId);
            cmdDelFig.execute();
            draw();
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
    OpenEasyUiDialog(url, 'FrmHiddenField', '新建文本', 600, 394, 'icon-new', true, function (scope) {
        var win = document.getElementById(dgId).contentWindow;
        var frmVal = win.GetFrmInfo();


        if (frmVal.Name == null || frmVal.Name.length == 0) {
            $.messager.alert('错误', '字段名称不能为空。', 'error');
        }
    }, this);
}
//打开窗体
function CCForm_ShowDialog(url, title) {
    OpenEasyUiDialog(url, 'CCForm_ShowDialog', title, 860, 560, 'icon-library', false);
}
//预览表单
function CCForm_BrowserView() {
    var url = "../../CCForm/Frm.aspx?FK_MapData=" + CCForm_FK_MapData + "&FrmType=FreeFrm&IsTest=1&WorkID=0&FK_Node=999999&s=2&T=" + GetDateString();
    OpenWindow(url);
}
//预览表单
function CCForm_FoolFrm() {
    var url = '/WF/Admin/FoolFormDesigner/Designer.aspx?FK_MapData=' + CCForm_FK_MapData + '&FK_Flow=001&MyPK=ND101&IsEditMapData=True';
   // var url = "../../CCForm/Frm.aspx?FK_MapData=" + CCForm_FK_MapData + "&FrmType=FreeFrm&IsTest=1&WorkID=0&FK_Node=999999&s=2&T=" + GetDateString();
    OpenWindow(url);
}

//打开页面方法
function OpenWindow(url) {
    var winWidth = 850;
    var winHeight = 680;
    if (screen && screen.availWidth) {
        winWidth = screen.availWidth;
        winHeight = screen.availHeight - 36;
    }
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