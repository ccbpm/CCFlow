/**Object with default values for figures*/
var FigureDefaults = {
    /**Size of figure's segment*/
    segmentSize: 70,

    /**Size of figure's short segment*/
    segmentShortSize: 40,

    /**Size of radius*/
    radiusSize: 35,

    /**Size of offset for parallels
    * For example: for parallelogram it's projection of inclined line on X axis*/
    parallelsOffsetSize: 40,

    /**Corner radius
    * For example: for rounded rectangle*/
    corner: 10,

    /**Corner roundness
    * Value from 0 to 10, where 10 - it's circle radius.*/
    cornerRoundness: 8,

    /**Color of lines*/
    strokeStyle: "#000000",

    /**Color of fill*/
    fillStyle: "#ffffff",

    /**Text size*/
    textSize: 15,

    /**Text label*/
    textStr: "Text",

    /**Text font*/
    textFont: "Arial",

    /**Color of text*/
    textColor: "#000000"
};

/**Controls set declaration*/
figureSets["basic"] = {
    name: '基本控件',
    description: 'A basic set of Controls',
    figures: [
        { figureFunction: null, name: "Poiner", image: "Poiner.png", CNName: "指针" },
        { figureFunction: null, name: CCForm_Controls.Line, image: "Line.png", CNName: "直线" },
        { figureFunction: "Label", name: CCForm_Controls.Label, image: "Label.png", CNName: "文字" },
        { figureFunction: "Button", name: CCForm_Controls.Button, image: "Button.png", CNName: "按钮" },
        { figureFunction: "HyperLink", name: CCForm_Controls.HyperLink, image: "HyperLink.png", CNName: "超链接" },
        { figureFunction: "Image", name: CCForm_Controls.Image, image: "Img.png", CNName: "图片" }

    ]
};

/**method of create Label **/
function figure_Label(x, y) {
    var f = new Figure('Label');
    //ccform Property
    f.CCForm_Shape = CCForm_Controls.Label;
    f.style.fillStyle = FigureDefaults.fillStyle;

    f.properties.push(new BuilderProperty('基本属性', 'group', BuilderProperty.TYPE_GROUP_LABEL));
    f.properties.push(new BuilderProperty(BuilderProperty.SEPARATOR));
    f.properties.push(new BuilderProperty('文本', 'primitives.0.str', BuilderProperty.TYPE_SINGLE_TEXT));
    f.properties.push(new BuilderProperty('字体大小', 'primitives.0.size', BuilderProperty.TYPE_TEXT_FONT_SIZE));
    f.properties.push(new BuilderProperty('字体', 'primitives.0.font', BuilderProperty.TYPE_TEXT_FONT_FAMILY));
    f.properties.push(new BuilderProperty('对齐', 'primitives.0.align', BuilderProperty.TYPE_TEXT_FONT_ALIGNMENT));
    f.properties.push(new BuilderProperty('下划线', 'primitives.0.underlined', BuilderProperty.TYPE_TEXT_UNDERLINED));
    f.properties.push(new BuilderProperty('字体加粗', 'primitives.0.fontWeight', BuilderProperty.TYPE_TEXT_FONTWEIGHT));
    f.properties.push(new BuilderProperty('字体颜色', 'primitives.0.style.fillStyle', BuilderProperty.TYPE_COLOR));

    var t2 = new Text("Label", x, y + FigureDefaults.radiusSize / 2, FigureDefaults.textFont, FigureDefaults.textSize);
    t2.style.fillStyle = FigureDefaults.textColor;

    f.addPrimitive(t2);

    f.finalise();
    return f;
}

/**method of create Button **/
function figure_Button(x, y) {
    var f = new Figure("Button");
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
    f.properties.push(new BuilderProperty('按钮标签', 'primitives.1.str', BuilderProperty.TYPE_SINGLE_TEXT));
    f.properties.push(new BuilderProperty('按钮事件', 'ButtonEvent', BuilderProperty.CCFormEnum));
    f.properties.push(new BuilderProperty('事件内容', 'BtnEventDoc', BuilderProperty.TYPE_TEXT));

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

    var t2 = new Text("Btn...", x + FigureDefaults.segmentSize / 2, y + FigureDefaults.segmentShortSize / 2 + FigureDefaults.corner, FigureDefaults.textFont, FigureDefaults.textSize);
    t2.style.fillStyle = FigureDefaults.textColor;

    f.addPrimitive(t2);

    f.finalise();
    return f;
}

/**method of create HyperLink **/
function figure_HyperLink(x, y) {

    var f = new Figure('HyperLink');
    //ccform Property
    f.CCForm_Shape = CCForm_Controls.HyperLink;
    f.style.fillStyle = FigureDefaults.fillStyle;

    f.properties.push(new BuilderProperty('基本属性-HyperLink', 'group', BuilderProperty.TYPE_GROUP_LABEL));
    f.properties.push(new BuilderProperty(BuilderProperty.SEPARATOR));
    f.properties.push(new BuilderProperty('文本', 'primitives.0.str', BuilderProperty.TYPE_SINGLE_TEXT));
    f.properties.push(new BuilderProperty('字体大小', 'primitives.0.size', BuilderProperty.TYPE_TEXT_FONT_SIZE));
    f.properties.push(new BuilderProperty('字体', 'primitives.0.font', BuilderProperty.TYPE_TEXT_FONT_FAMILY));
    //f.properties.push(new BuilderProperty('对齐', 'primitives.0.align', BuilderProperty.TYPE_TEXT_FONT_ALIGNMENT));
    f.properties.push(new BuilderProperty('下划线', 'primitives.0.underlined', BuilderProperty.TYPE_TEXT_UNDERLINED));
    f.properties.push(new BuilderProperty('字体加粗', 'primitives.0.fontWeight', BuilderProperty.TYPE_TEXT_FONTWEIGHT));
    f.properties.push(new BuilderProperty('字体颜色', 'primitives.0.style.fillStyle', BuilderProperty.TYPE_COLOR));

    f.properties.push(new BuilderProperty('控件属性', 'group', BuilderProperty.TYPE_GROUP_LABEL));
    f.properties.push(new BuilderProperty(BuilderProperty.SEPARATOR));
    f.properties.push(new BuilderProperty('连接地址', 'URL', BuilderProperty.TYPE_SINGLE_TEXT));
    f.properties.push(new BuilderProperty('打开窗口', 'WinOpenModel', BuilderProperty.CCFormEnum));

    var t2 = new Text("我的超连接.", x, y + FigureDefaults.radiusSize / 2, FigureDefaults.textFont, FigureDefaults.textSize);
    t2.style.fillStyle = "#0000ff";
    t2.underlined = true;

    f.addPrimitive(t2);

    f.finalise();
    return f;
}

/**method of create image **/
function figure_Image(x,  y) {
    var f = new Figure("Image");
    //ccform Property
    f.CCForm_Shape = CCForm_Controls.Image;
    f.style.fillStyle = FigureDefaults.fillStyle;
    f.style.strokeStyle = FigureDefaults.strokeStyle;

    //Image
    var url = figureSetsURL + "/basic/TempleteFile.png";

    var ifig = new ImageFrame(url, x, y, true, 120, 140);
    ifig.debug = true;

    f.addPrimitive(ifig);

    //Text
    f.properties.push(new BuilderProperty('控件属性-Image', 'group', BuilderProperty.TYPE_GROUP_LABEL));
    f.properties.push(new BuilderProperty(BuilderProperty.SEPARATOR));
    f.properties.push(new BuilderProperty('应用类型', 'ImgAppType', BuilderProperty.CCFormEnum));
    f.properties.push(new BuilderProperty('上传图片', 'ImgURL', BuilderProperty.CCFormUpload));
    f.properties.push(new BuilderProperty('指定路径', 'ImgPath', BuilderProperty.TYPE_SINGLE_TEXT));
    f.properties.push(new BuilderProperty('图片连接到', 'LinkURL', BuilderProperty.TYPE_SINGLE_TEXT));
    f.properties.push(new BuilderProperty('打开窗口', 'WinOpenModel', BuilderProperty.CCFormEnum));

    var t2 = new Text("", x, y + 36, FigureDefaults.textFont, FigureDefaults.textSize);
    t2.style.fillStyle = FigureDefaults.textColor;
    f.addPrimitive(t2);

    f.finalise();
    return f;
}

/**Controls set declaration*/
figureSets["Data"] = {
    name: '字段控件',
    description: 'A Data set of Controls',
    figures: [
        { figureFunction: "TextBox", name: CCForm_Controls.TextBox, image: "TextBox.png",CNName:"文本类型" },
        { figureFunction: "TextBox", name: CCForm_Controls.TextBoxInt, image: "TextBoxInt.png",CNName:"数值类型" },
        { figureFunction: "TextBox", name: CCForm_Controls.TextBoxMoney, image: "TextBoxMoney.png", CNName: "金额类型" },
        { figureFunction: "TextBox", name: CCForm_Controls.TextBoxFloat, image: "TextBoxFloat.png", CNName: "浮点类型" },
        { figureFunction: "TextBox", name: CCForm_Controls.Date, image: "TextBoxDate.png", CNName: "日期类型" },
        { figureFunction: "TextBox", name: CCForm_Controls.DateTime, image: "TextBoxDateTime.png", CNName: "日期时间类型" },
        { figureFunction: "TextBox", name: CCForm_Controls.CheckBox, image: "Checkbox.png", CNName: "选择框" },
        { figureFunction: "TextBox", name: CCForm_Controls.RadioButton, image: "Radiobutton.png", CNName: "枚举单选按钮" },
        { figureFunction: "TextBox", name: CCForm_Controls.DropDownListEnum, image: "DropDownListEnum.png", CNName: "枚举下拉框" },
        { figureFunction: "TextBox", name: CCForm_Controls.DropDownListTable, image: "DropDownListTable.png", CNName: "外键下拉框" }
     //   { figureFunction: "TextBox", name: CCForm_Controls.ListBox, image: "ListBox.png", CNName: "外部数据源" },
        //{ figureFunction: "TextBox", name: CCForm_Controls.HiddendField, image: "HiddendField.png", CNName: "隐藏字段" }
    ]
};

//文本框创建控件
function figure_TextBox(x, y) {

    var f = new Figure("TextBox");

    //ccform Property
    f.CCForm_Shape = CCForm_Controls.TextBox;

    f.style.fillStyle = FigureDefaults.fillStyle;
    f.style.strokeStyle = FigureDefaults.strokeStyle;

    if (createFigureName != "RadioButton") {
        //Image
        var url = figureSetsURL + "/Data/TextBoxBig.png";

        var ifig = new ImageFrame(url, x, y, true, 150, 30);
        ifig.debug = true;
        f.addPrimitive(ifig);
    }
    var t2 = new Text("", x, y, FigureDefaults.textFont, FigureDefaults.textSize);
    t2.style.fillStyle = FigureDefaults.textColor;
    f.addPrimitive(t2);

    f.finalise();
    return f;
}

/**Controls set declaration*/
figureSets["Components"] = {
    name: '组件类',
    description: '组件类控件',
    figures: [
        { figureFunction: "Square", name: CCForm_Controls.Dtl, image: "Dtl.png", CNName: "明细表/从表" },
        { figureFunction: "Square", name: "AthMulti", image: "AthMulti.png", CNName: "多附件" },
        { figureFunction: "Square", name: "AthSingle", image: "AthSingle.png", CNName: "单附件" },
        { figureFunction: "Square", name: "AthImg", image: "AthImg.png", CNName: "图片附件" },
        { figureFunction: "Square", name: "HandSiganture", image: "HandSiganture.png", CNName: "签字板" },
        { figureFunction: "Square", name: "iFrame", image: "iFrame.png", CNName: "框架" },
        { figureFunction: "Square", name: "Fieldset", image: "Fieldset.png", CNName: "分组" }
    ]
};

/**Controls set declaration*/
figureSets["mobile"] = {
    name: '移动组件',
    description: '与手机平板移动设备应用相关的组件',
    figures: [
        { figureFunction: "Square", name: "Map", image: "Map.png", CNName: "地图控件" },
        { figureFunction: "Square", name: "Camera", image: "Camera.png", CNName: "拍照上传" },
        { figureFunction: "Square", name: "SoundRecord", image: "SoundRecord.png", CNName: "录音" },
        { figureFunction: "Square", name: "VideoRecord", image: "VideoRecord.png", CNName: "录像" },
        { figureFunction: "Square", name: "QRCode", image: "QRCode.png", CNName: "二维码" }
    ]
};


/**Controls set declaration*/
figureSets["ccbpm"] = {
    name: '流程组件',
    description: '与ccbpm相关的流程组件',
    figures: [
        { figureFunction: "Square", name: "CheckGroup", image: "CheckGroup.png", CNName: "审核分组" },
        { figureFunction: "Square", name: "FlowChart", image: "FlowChart.png", CNName: "轨迹图" },
        { figureFunction: "Square", name: "FrmCheck", image: "FrmCheck.png", CNName: "审核组件" },
        { figureFunction: "Square", name: "SubFlowDtl", image: "SubFlowDtl.png", CNName: "子流程" },
        { figureFunction: "Square", name: "ThreadDtl", image: "ThreadDtl.png", CNName: "子线程" },
        { figureFunction: "Square", name: "FrmTransferCustom", image: "FrmTransferCustom.png", CNName: "流转自定义" }
    ]
};

function figure_Square(x, y) {

    var f = new Figure("Square");


    //ccform Property
    f.CCForm_Shape = CCForm_Controls.TextBox;

    f.style.fillStyle = FigureDefaults.fillStyle;
    f.style.strokeStyle = FigureDefaults.strokeStyle;

    //Image
    var url = figureSetsURL + "/Data/TextBoxBig.png";

    var ifig = new ImageFrame(url, x, y, true, 150, 30);
    ifig.debug = true;
    f.addPrimitive(ifig);

    var t2 = new Text("", x, y, FigureDefaults.textFont, FigureDefaults.textSize);
    t2.style.fillStyle = FigureDefaults.textColor;
    f.addPrimitive(t2);

    f.finalise();
    return f;
    
}