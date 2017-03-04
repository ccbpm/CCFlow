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

    if (plant == 'JFlow') {
        url = url.replace('.aspx', '.jsp');
        OpenEasyUiDialog(url, 'CCForm_ShowDialog', title, 860, 560, 'icon-library', false);
    } else {
        OpenEasyUiDialog(url, 'CCForm_ShowDialog', title, 860, 560, 'icon-library', false);
    }
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


//表单属性
function CCForm_Attr() {
    var url = '/WF/Comm/RefFunc/UIEn.aspx?EnsName=BP.WF.Template.MapFrmFrees&PK=' + CCForm_FK_MapData;
    OpenWindow(url, 500,400);
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
    //transe old CCForm to new
    $.post(controllerURLConfig, {
        action: 'CcformElements',
        FK_MapData: CCForm_FK_MapData
    }, function (jsonData) {
        var jData = $.parseJSON(jsonData);
        console.log(jData)
        if (jData.success == true) {
            var flow_Data = $.parseJSON(jData.data);
            //循环MapAttr
            for (var mapAtrrIndex in flow_Data.MapAttr) {
                var mapAttr = flow_Data.MapAttr[mapAtrrIndex];
                var createdFigure = figure_MapAttr_Template(mapAttr);
                //move it into position
                createdFigure.transform(Matrix.translationMatrix(mapAttr.X - createdFigure.rotationCoords[0].x, mapAttr.Y - createdFigure.rotationCoords[0].y))
                createdFigure.style.lineWidth = defaultLineWidth;
                //add to STACK
                STACK.figureAdd(createdFigure);
            }

            //循环FrmLab
            for (var i in flow_Data.FrmLab) {
                var frmLab = flow_Data.FrmLab[i];
                var createdFigure = figure_Template_Label(frmLab);
                //move it into position
                createdFigure.transform(Matrix.translationMatrix(frmLab.X - createdFigure.rotationCoords[0].x, frmLab.Y - createdFigure.rotationCoords[0].y))
                createdFigure.style.lineWidth = defaultLineWidth;
                //add to STACK
                STACK.figureAdd(createdFigure);
            }

            //循环FrmRB
            //for (var i in flow_Data.FrmRb) {
            //    var frmRb = flow_Data.FrmRb[i];
            //    var createdFigure = figure_Template_Rb(frmRb);
            //    //move it into position
            //    createdFigure.transform(Matrix.translationMatrix(frmRb.X - createdFigure.rotationCoords[0].x, frmRb.Y - createdFigure.rotationCoords[0].y))
            //    createdFigure.style.lineWidth = defaultLineWidth;
            //    //add to STACK
            //    STACK.figureAdd(createdFigure);
            //}

            //循环FrmBtn
            for (var i in flow_Data.FrmBtn) {
                var frmBtn = flow_Data.FrmBtn[i];
                var createdFigure = figure_Template_Btn(frmBtn);
                //move it into position
                createdFigure.transform(Matrix.translationMatrix(frmBtn.X - createdFigure.rotationCoords[0].x, frmBtn.Y - createdFigure.rotationCoords[0].y))
                createdFigure.style.lineWidth = defaultLineWidth;
                //add to STACK
                STACK.figureAdd(createdFigure);
            }
            redraw = true;
            draw();
            //save(false);
        } else {
            Designer_ShowMsg('错误: ' + jData.msg);
        }
    });
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
        if (mapAttr.LGType == 1 ) {
            f.CCForm_Shape = "DropDownListEnum";
        }//外键下拉框
        else if (mapAttr.LGType == 2) {
            f.CCForm_Shape = "DropDownListTable";
        }
    } else if (mapAttr.UIContralType == 2) {//复选框
        f = new Figure("TextBox");
        f.CCForm_Shape = "TextBoxBoolean";
    } else if (mapAttr.UIContralType == 3) {//单选妞
        return;
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

        f.properties.push(new BuilderProperty(property.ProText, property.proName, property.ProType, propertyVale));
    }

    //Image
    var url = figureSetsURL + "/DataView/" + f.CCForm_Shape + ".png";

    var ifig = new ImageFrame(url, mapAttr.X, mapAttr.Y, true, 150, 30);
    ifig.debug = true;
    f.addPrimitive(ifig);

    var t2 = new Text(mapAttr.KeyOfEn, mapAttr.X, mapAttr.Y, FigureDefaults.textFont, FigureDefaults.textSize);
    t2.style.fillStyle = FigureDefaults.textColor;
    f.addPrimitive(t2);

    f.gradientBounds = [100, 100, 200, 200];

    f.finalise();
    return f;
}

//升级表单元素 初始化Label
function figure_Template_Label(frmLab) {
    var f = new Figure('Label');
    //ccform Property
    f.CCForm_Shape = "Label";
    f.style.fillStyle = FigureDefaults.fillStyle;
   
    f.CCForm_MyPK = frmLab.MyPK;
    f.name = "Label";
    var x = frmLab.X;
    var y = frmLab.Y;
    f.properties.push(new BuilderProperty('基本属性', 'group', BuilderProperty.TYPE_GROUP_LABEL));
    f.properties.push(new BuilderProperty(BuilderProperty.SEPARATOR));
    f.properties.push(new BuilderProperty('文本', 'primitives.0.str', BuilderProperty.TYPE_SINGLE_TEXT,frmLab.Text));
    f.properties.push(new BuilderProperty('字体大小', 'primitives.0.size', BuilderProperty.TYPE_TEXT_FONT_SIZE,frmLab.FontSize));
    f.properties.push(new BuilderProperty('字体', 'primitives.0.font', BuilderProperty.TYPE_TEXT_FONT_FAMILY,frmLab.FontName));
    //f.properties.push(new BuilderProperty('对齐', 'primitives.0.align', BuilderProperty.TYPE_TEXT_FONT_ALIGNMENT));
    f.properties.push(new BuilderProperty('下划线', 'primitives.0.underlined', BuilderProperty.TYPE_TEXT_UNDERLINED));
    f.properties.push(new BuilderProperty('字体加粗', 'primitives.0.fontWeight', BuilderProperty.TYPE_TEXT_FONTWEIGHT,frmLab.IsBold));
    f.properties.push(new BuilderProperty('字体颜色', 'primitives.0.style.fillStyle', BuilderProperty.TYPE_COLOR, frmLab.FontColor));

    var t2 = new Text(frmLab.Text, frmLab.X, frmLab.Y + FigureDefaults.radiusSize / 2, FigureDefaults.textFont, FigureDefaults.textSize);
    t2.style.fillStyle = FigureDefaults.textColor;

    f.addPrimitive(t2);

    f.finalise();
    return f;
}

//初始化按钮
function figure_Template_Btn(frmLab) {
    var f = new Figure("Button");
    f.CCForm_MyPK = frmLab.MyPK;
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
    f.properties.push(new BuilderProperty('按钮标签', 'primitives.1.str', BuilderProperty.TYPE_SINGLE_TEXT, frmLab.Text));
    f.properties.push(new BuilderProperty('按钮事件', 'ButtonEvent', BuilderProperty.CCFormEnum, frmLab.EventType));
    f.properties.push(new BuilderProperty('事件内容', 'BtnEventDoc', BuilderProperty.TYPE_TEXT,frmLab.EventContext));

    var x = frmLab.X;
    var y = frmLab.Y;
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

    var t2 = new Text(frmLab.Text, x + FigureDefaults.segmentSize / 2, y + FigureDefaults.segmentShortSize / 2 + FigureDefaults.corner, FigureDefaults.textFont, FigureDefaults.textSize);
    t2.style.fillStyle = FigureDefaults.textColor;

    f.addPrimitive(t2);

    f.finalise();
    return f;
}
//初始化单选按钮
function figure_Template_Rb(frmLab) {
    var f = new Figure("RadioButton");
    f.CCForm_Shape = "RadioButton";
    f.name = "Label";
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

        f.properties.push(new BuilderProperty(property.ProText, property.proName, property.ProType, propertyVale));
    }

    //Image
    var url = figureSetsURL + "/DataView/" + f.CCForm_Shape + ".png";

    var ifig = new ImageFrame(url, mapAttr.X, mapAttr.Y, true, 150, 30);
    ifig.debug = true;
    f.addPrimitive(ifig);

    var t2 = new Text(mapAttr.KeyOfEn, mapAttr.X, mapAttr.Y, FigureDefaults.textFont, FigureDefaults.textSize);
    t2.style.fillStyle = FigureDefaults.textColor;
    f.addPrimitive(t2);

    f.gradientBounds = [100, 100, 200, 200];

    f.finalise();
    return f;
}

