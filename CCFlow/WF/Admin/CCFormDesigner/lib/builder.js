﻿function Builder() {

}

/**Image base path*/
Builder.IMAGE_BASE_PATH = './assets/images/';

/**Path to fill icon image*/
Builder.IMAGE_FILL_ICON_PATH = Builder.IMAGE_BASE_PATH + 'prop-icon-fill.png';

/**Path to stroke icon image*/
Builder.IMAGE_STROKE_ICON_PATH = Builder.IMAGE_BASE_PATH + 'prop-icon-stroke.png';

/**Path to line width icon image*/
Builder.IMAGE_LINEWIDTH_ICON_PATH = Builder.IMAGE_BASE_PATH + 'prop-icon-linewidth.png';

/**Line (dashed) style icon image*/
Builder.IMAGE_LINESTYLE_ICON_PATH = Builder.IMAGE_BASE_PATH + 'prop-icon-linestyle.png';

/**Path to start style icon image*/
Builder.IMAGE_STARTSTYLE_ICON_PATH = Builder.IMAGE_BASE_PATH + 'prop-icon-startstyle.png';

/**Path to end style icon image*/
Builder.IMAGE_ENDSTYLE_ICON_PATH = Builder.IMAGE_BASE_PATH + 'prop-icon-endstyle.png';

/**Path to width icon image*/
Builder.IMAGE_WIDTH_ICON_PATH = Builder.IMAGE_BASE_PATH + 'prop-icon-h-resize.png';

/**Path to height icon image*/
Builder.IMAGE_HEIGHT_ICON_PATH = Builder.IMAGE_BASE_PATH + 'prop-icon-v-resize.png';

/**Path to URL style icon image*/
Builder.IMAGE_URL_ICON_PATH = Builder.IMAGE_BASE_PATH + 'prop-icon-url.png';

/**Path to text style icon image*/
Builder.IMAGE_TEXT_ICON_PATH = Builder.IMAGE_BASE_PATH + 'prop-icon-text.png';


/**Creates a {Builder} out of JSON parsed object
*@param {JSONObject} o - the JSON parsed object
*@return {Builder} a newly constructed Builder
*@author Alex Gheorghiu <alex@scriptoid.com>
**/
//Builder.load = function(o){
//    var newBuilder = new Builder();
//    newBuilder.properties = BuilderProperty.loadArray(o.properties);
//    newBuilder.figureId = o.figureId;
//    return newBuilder;
//}

/**
*Creates the property panel for a shape {Figure} or {Connector}
*@param {DOMObject} DOMObject - the div of the properties panel
*@param {Figure} shape - the figure for which the properties will be displayed
**/
Builder.constructPropertiesPanel = function (DOMObject, shape) {
    for (var i = 0; i < shape.properties.length; i++) {
        // regExp to avoid properties of Text editor
        //if (/(primitives\.\d+|middleText)\.(str|size|font|align|underlined|style\.fillStyle)/g.test(shape.properties[i].property) === false) {
        //}
        shape.properties[i].injectInputArea(DOMObject, shape.id);
    }
};

/**
*Creates the property panel for a Text primitive of shape {Figure} and returns it
*@param {DOMObject} textEditor - the <div> of the properties panel (declared inside editor page)
*@param {DOMObject} textEditorTools - the <div> of the text editor's tools (declared insider editor page)
*@param {Figure} shape - the figure - parent of Text primitive
*@param {Number} textPrimitiveId - the id value of Text primitive child of figure for which the properties will be displayed
*
*@return {TextEditorPopup} - new instance of TextEditorPopup after init
*  @author Artyom Pokatilov <artyom.pokatilov@gmail.com>
**/
Builder.constructTextPropertiesPanel = function (textEditor, textEditorTools, shape, textPrimitiveId) {
    var textEditor = new TextEditorPopup(textEditor, textEditorTools, shape, textPrimitiveId);
    textEditor.init();

    return textEditor;
};

/**
*Creates the properties for main CanvasProps
*@param {DOMObject} DOMObject - the div of the properties panel
*@param {CanvasProps} canvasProps - the CanvasProps for which the properties will be displayed
**/
Builder.constructCanvasPropertiesPanel = function (DOMObject, canvasProps) {
    var div = document.createElement("div");
    var icon;

    // colorPicker plugin requires div to be already appended to the DOM
    DOMObject.appendChild(div);

    //title
    var titleDiv = document.createElement("div");
    titleDiv.className = 'GroupLabel';
    titleDiv.textContent = '画板设置';
    div.appendChild(titleDiv);

    //separator
    var separator = document.createElement("hr");
    div.appendChild(separator);

    //fill color
    var colorDiv = document.createElement("div");
    colorDiv.className = "line";

    var currentColor = canvasProps.getFillColor();
    var uniqueId = new Date().getTime();

    //    var labelDiv = document.createElement("div");
    //    labelDiv.className = "label";
    //    labelDiv.textContent = "背景颜色";

    //    icon = new Image();
    //    icon.className = 'prop-icon';
    //    icon.src = Builder.IMAGE_FILL_ICON_PATH;
    //    labelDiv.appendChild(icon);

    //    colorDiv.appendChild(labelDiv);

    //    var colorSelectorDiv = document.createElement("div");
    //    colorSelectorDiv.id = 'colorSelector' + uniqueId;
    //    colorSelectorDiv.className = 'color-selector';

    //    var colorInput = document.createElement("input");
    //    colorInput.type = "text";
    //    colorInput.id = 'colorpickerHolder' + uniqueId;
    //    colorInput.value = currentColor;
    //    colorSelectorDiv.appendChild(colorInput);

    //    colorDiv.appendChild(colorSelectorDiv);

    //    div.appendChild(colorDiv);

    //    var colorPicker = document.getElementById('colorpickerHolder' + uniqueId);

    //    //let plugin do the job
    //    $(colorPicker).colorPicker();

    //    //on change update the canvasProps
    //    colorPicker.onchange = function () {
    //        var newColor = colorPicker.value;
    //        //Did we change fill color?
    //        if (canvasProps.getFillColor() !== newColor) {
    //            var cmd = new CanvasChangeColorCommand(newColor);
    //            cmd.execute();
    //            History.addUndo(cmd);

    //            //trigger a repaint;
    //            draw();
    //        }
    //    };

    //width
    var divWidth = document.createElement("div");
    divWidth.className = 'label';
    divWidth.textContent = '宽度';

    icon = new Image();
    icon.className = 'prop-icon';
    icon.src = Builder.IMAGE_WIDTH_ICON_PATH;
    divWidth.appendChild(icon);

    var inputWidth = document.createElement("input");
    inputWidth.type = "text";
    inputWidth.className = "text"; //required for onkeydown
    inputWidth.value = canvasProps.getWidth();
    divWidth.appendChild(inputWidth);

    div.appendChild(divWidth);

    //height
    var divHeight = document.createElement("div");
    divHeight.className = 'label';
    divHeight.textContent = '高度';

    icon = new Image();
    icon.className = 'prop-icon';
    icon.src = Builder.IMAGE_HEIGHT_ICON_PATH;
    divHeight.appendChild(icon);

    var inputHeight = document.createElement("input");
    inputHeight.type = "text";
    inputHeight.className = "text"; //required for onkeydown
    inputHeight.value = canvasProps.getHeight();
    divHeight.appendChild(inputHeight);

    div.appendChild(divHeight);

    //button block: Update and Fit options
    var divButton = document.createElement("div");

    //update button
    var btnUpdate = document.createElement("input");
    btnUpdate.setAttribute("type", "button");
    btnUpdate.setAttribute("value", "应用");

    btnUpdate.onclick = function () {
        //update canvas props
        // Did we entered number value for width?
        var widthVal = Number(inputWidth.value);
        if (isNaN(widthVal)) {
            widthVal = canvasProps.getWidth();
            inputWidth.value = widthVal;
        }

        var heightVal = Number(inputHeight.value);
        if (isNaN(heightVal)) {
            heightVal = canvasProps.getHeight();
            inputHeight.value = heightVal;
        }

        //Did we change width or height?
        if (canvasProps.getWidth() !== widthVal || canvasProps.getHeight() !== heightVal) {
            var undo = new CanvasChangeSizeCommand(widthVal, heightVal);
            undo.execute();
            History.addUndo(undo);
        }
        //执行保存到数据库MapData--CCForm_FK_MapData

        draw();
    };

    divButton.appendChild(btnUpdate);

    //fit button
    //    var btnFit = document.createElement("input");
    //    btnFit.setAttribute("type", "button");
    //    btnFit.setAttribute("value", "Fit");

    //    btnFit.onclick = function () {
    //        /* Algorithm
    //        * 1) Find border points of Figure/Container/Connector bounds
    //        * 2) Get new width and height of canvas from 1)
    //        * 3) If canvas size changed -> Use CanvasChangeSizeCommand to change canvas size
    //        */

    //        var workAreaBounds = STACK.getWorkAreaBounds();
    //        var newCanvasWidth = workAreaBounds[2] - workAreaBounds[0];
    //        var newCanvasHeight = workAreaBounds[3] - workAreaBounds[1];

    //        // Did canvas size changed?
    //        if (newCanvasWidth !== canvasProps.getWidth() || newCanvasHeight !== canvasProps.getHeight()) {
    //            var cmdCanvasFit = new CanvasFitCommand(
    //                newCanvasWidth + DIAGRAMO.CANVAS_FIT_PADDING * 2,
    //                newCanvasHeight + DIAGRAMO.CANVAS_FIT_PADDING * 2,
    //                workAreaBounds[0] - DIAGRAMO.CANVAS_FIT_PADDING,  // new (0,0) point goes this X coordinate
    //                workAreaBounds[1] - DIAGRAMO.CANVAS_FIT_PADDING   // new (0,0) point goes this Y coordinate
    //            );
    //            cmdCanvasFit.execute();
    //            History.addUndo(cmdCanvasFit);
    //            //  redraw canvas
    //            draw();
    //        }
    //    };
    //    divButton.appendChild(btnFit);

    div.appendChild(divButton);
}



/** The structure that will declare any visible and changable property of a shape.
* 
*  Note:  A {BuilderProperty} DOES NOT STORE THE VALUE OF THE PROPERTY but only
* describe what properties of a {Style} are exposed and how the {Builder} will
* create interface (fragments) for user in properties panel. 
*
* @constructor
* @this {Builder}
* @param {String} name - the name of the property
* @param {String} property - the property (in dot notation) we are using in the form of 'primitives.0.style.strokeStyle'
* @param {Object} type - could be either, 'Color', 'Boolean', 'Text' or {Array}
* In case it's an {Array} it is of type [{Text,Value}] and is used to generate a DD menu
*/
function BuilderProperty(name, property, type, value) {
    this.name = name;
    this.property = property;
    this.type = type;
    this.PropertyValue = value ? value : null;
}

/**Color property type*/
BuilderProperty.TYPE_COLOR = 'Color';

/**Text property type*/
BuilderProperty.TYPE_TEXT = 'Text';

/**SingleText property type*/
BuilderProperty.TYPE_SINGLE_TEXT = 'SingleText';

/**Text size property type*/
BuilderProperty.TYPE_TEXT_FONT_SIZE = 'TextFontSize';

/**Font family property type*/
BuilderProperty.TYPE_TEXT_FONT_FAMILY = 'TextFontFamily';

/**Text aligment property type*/
BuilderProperty.TYPE_TEXT_FONT_ALIGNMENT = 'TextFontAlignment';

/**Text underlined property type*/
BuilderProperty.TYPE_TEXT_UNDERLINED = 'TextUnderlined';

/**Boolean property type*/
BuilderProperty.TYPE_BOOLEAN = 'Boolean';

/**Line width property type*/
BuilderProperty.TYPE_LINE_WIDTH = 'LineWidth';

/**Line width property style*/
BuilderProperty.TYPE_LINE_STYLE = 'LineStyle';

/**Image Fill type*/
BuilderProperty.TYPE_IMAGE_FILL = 'ImageFill';

/**File Upload type*/
BuilderProperty.TYPE_IMAGE_UPLOAD = "ImageUpload";

/**Connector's end property type*/
BuilderProperty.TYPE_CONNECTOR_END = 'ConnectorEnd';

/** CCForm Propertys **/
BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly = "SingleTextReadOnly";
BuilderProperty.TYPE_GROUP_LABEL = 'GroupLabel';
BuilderProperty.CCFormEnum = "Combobox";
BuilderProperty.CCFormUpload = "UploadFile";
BuilderProperty.TYPE_TEXT_FONTWEIGHT = 'FontWeight';
BuilderProperty.TYPE_TEXT_FONTWEIGHTS = [
    { Text: 'normal', Value: 'normal' },
    { Text: 'bold', Value: 'bold' },
    { Text: 'bolder', Value: 'bolder' },
    { Text: 'lighter', Value: 'lighter' }
];
BuilderProperty.CCFormLink = 'linkButton';
/**URL attached to a figure*/
BuilderProperty.TYPE_URL = 'URL';



//BuilderProperty.IMAGE_FILL = [{Text: 'No Scaling', Value: CanvasImage.FIXED_NONE},{Text: 'Fit to Area', Value: CanvasImage.FIXED_BOTH},{Text: 'Fit to Width',Value: CanvasImage.FIXED_WIDTH},{Text: 'Fit to Height',Value: CanvasImage.FIXED_HEIGHT},{Text: ' Auto Fit',Value: CanvasImage.FIXED_AUTO}]

/**Line widths*/
BuilderProperty.LINE_WIDTHS = [
    { Text: '0.5px', Value: '0.5' }, { Text: '1px', Value: '1' }, { Text: '2px', Value: '2' }, { Text: '3px', Value: '3' },
    { Text: '4px', Value: '4' }, { Text: '5px', Value: '5' }, { Text: '6px', Value: '6' },
    { Text: '7px', Value: '7' }, { Text: '8px', Value: '8' }, { Text: '9px', Value: '9' },
    { Text: '10px', Value: '10'}];


/**Line styles*/
BuilderProperty.LINE_STYLES = [
    { Text: 'Continous', Value: 'continuous' },
    { Text: 'Dotted', Value: 'dotted' },
    { Text: 'Dashed', Value: 'dashed' }
];

/**Font sizes*/
BuilderProperty.FONT_SIZES = [];
for (var i = 0; i < 73; i++) {
    BuilderProperty.FONT_SIZES.push({ Text: i + 'px', Value: i });
}

/**Connector ends*/
BuilderProperty.CONNECTOR_ENDS = [{ Text: 'Normal', Value: 'Normal' }, { Text: 'Arrow', Value: 'Arrow' },
    { Text: 'Empty Triangle', Value: 'Empty' }, { Text: 'Filled Triangle', Value: 'Filled'}];

/**Display separator*/
BuilderProperty.SEPARATOR = 'SEPARATOR';

/**Css class for button checking control*/
BuilderProperty.BUTTON_CHECKER_CLASS = 'button-checker';

/**Name of attribute to define is property checked or not*/
BuilderProperty.BUTTON_CHECKED_ATTRIBUTE = 'button-checked';

/**Label for text underlined property*/
BuilderProperty.TEXT_UNDERLINED_LABEL = 'U';


/**Creates a {BuilderProperty} out of JSON parsed object
*@param {JSONObject} o - the JSON parsed object
*@return {BuilderProperty} a newly constructed Point
*@author Alex Gheorghiu <alex@scriptoid.com>
**/
BuilderProperty.load = function (o) {
    var prop = new BuilderProperty();
    prop.name = o.name;
    prop.property = o.property;
    prop.type = o.type;
    prop.PropertyValue = o.PropertyValue;
    return prop;
}


/**Creates an array of BuilderProperties from an array of {JSONObject}s
*@param {Array} v - the array of JSONObjects
*@return an {Array} of {BuilderProperty}-ies
*@author Alex Gheorghiu <alex@scriptoid.com>
**/
BuilderProperty.loadArray = function (v) {
    var newProps = [];
    for (var i = 0; i < v.length; i++) {
        newProps.push(BuilderProperty.load(v[i]));
    }
    return newProps;
}

BuilderProperty.prototype = {

    constructor: BuilderProperty,

    toString: function () {
        return 'Propery type: ' + this.type + ' name: ' + this.name + ' property: ' + this.property;
    },

    equals: function (anotherBuilderProperty) {
        return this.type == anotherBuilderProperty.type
            && this.name == anotherBuilderProperty.name
            && this.property == anotherBuilderProperty.property;
    },

    /**
    *Generates a HTML fragment to allow to edit its property.
    *For example if current property is a color then this method will
    *inject a color picker in the specified DOMObject
    *
    *@param {HTMLElement} DOMObject - the div of the properties panel
    *@param {Number} figureId - the id of the figure we are using
    */
    injectInputArea: function (DOMObject, figureId) {
        if (this.name == BuilderProperty.SEPARATOR) {
            DOMObject.appendChild(document.createElement("hr"));
            return;
        }
        else if (this.type == BuilderProperty.TYPE_COLOR) {
            this.generateColorCode(DOMObject, figureId);
        }
        else if (this.type == BuilderProperty.TYPE_TEXT) {
            this.generateTextCode(DOMObject, figureId);
        }
        else if (this.type == BuilderProperty.TYPE_SINGLE_TEXT) {
            this.generateSingleTextCode(DOMObject, figureId);
        }
        else if (this.type == BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly) {
            this.generateSingleTextReadOnlyCode(DOMObject, figureId);
        }
        else if (this.type == BuilderProperty.TYPE_TEXT_FONT_SIZE) {
            this.generateArrayCode(DOMObject, figureId, BuilderProperty.FONT_SIZES);
        }
        else if (this.type == BuilderProperty.TYPE_TEXT_FONT_FAMILY) {
            this.generateArrayCode(DOMObject, figureId, Text.FONTS);
        }
        else if (this.type == BuilderProperty.TYPE_TEXT_FONT_ALIGNMENT) {
            this.generateArrayCode(DOMObject, figureId, Text.ALIGNMENTS);
        }
        else if (this.type == BuilderProperty.TYPE_TEXT_UNDERLINED) {
            this.generateButtonCheckerCode(DOMObject, figureId, BuilderProperty.TEXT_UNDERLINED_LABEL);
        }
        else if (this.type == BuilderProperty.TYPE_TEXT_FONTWEIGHT) {
            this.generateArrayCode(DOMObject, figureId, BuilderProperty.TYPE_TEXT_FONTWEIGHTS);
        }
        else if (this.type == BuilderProperty.TYPE_CONNECTOR_END) {
            this.generateArrayCode(DOMObject, figureId, BuilderProperty.CONNECTOR_ENDS);
        }
        else if (this.type == BuilderProperty.TYPE_LINE_WIDTH) {
            this.generateArrayCode(DOMObject, figureId, BuilderProperty.LINE_WIDTHS);
        }
        else if (this.type == BuilderProperty.TYPE_LINE_STYLE) {
            this.generateArrayCode(DOMObject, figureId, BuilderProperty.LINE_STYLES);
        }
        else if (this.type == BuilderProperty.TYPE_URL) {
            this.generateURLCode(DOMObject, figureId);
        }
        else if (this.type == BuilderProperty.TYPE_GROUP_LABEL) {
            this.generateGroupLabelCode(DOMObject, figureId);
            return;
        }
        else if (this.type == BuilderProperty.CCFormEnum) {
            var items = CCForm_Control_Enum[this.property];
            this.generateArrayCode(DOMObject, figureId, items);
        }
        else if (this.type == BuilderProperty.CCFormUpload) {
            this.generateUploadCode(DOMObject, figureId);
        }
        else if (this.type == BuilderProperty.CCFormLink) {
            this.generateCCFormLinkCode(DOMObject, figureId);
        }

        //ccbpm 控件下面增加虚线
        this.generateControlUnderLine(DOMObject);
    },

    /**
    *Creates a boolean editor; usually a chechbox
    *
    *@param {HTMLElement} DOMObject - the div of the properties panel
    *@param {Number} figureId - the id of the figure we are using
    **/
    generateBooleanCode: function (DOMObject, figureId) {
        var d = new Date();
        var uniqueId = d.getTime();
        var value = this.getValue(figureId);
        var div = document.createElement("div");

        var labelDiv = document.createElement("div");
        labelDiv.className = "label";
        labelDiv.textContent = this.name;

        div.appendChild(labelDiv);

        var check = document.createElement("input");
        check.type = "checkbox"
        check.className = "text"; //required for onkeydown
        check.checked = value;
        div.children[0].appendChild(check);
        check.onclick = function (figureId, property) {
            return function () {
                updateShape(figureId, property, this.checked)
            }
        } (figureId, this.property);

        DOMObject.appendChild(div);
    },
    generateControlUnderLine: function (DOMObject) {
        var uLine = document.createElement("hr");
        uLine.className = "editLine";
        DOMObject.appendChild(uLine);
    },

    /**Generate the code to edit the text.
    *The text got updated when you leave the input area
    *
    *@param {HTMLElement} DOMObject - the div of the properties panel
    *@param {Number} shapeId - the id of the {Figure} or {Connector} we are using
    **/
    generateTextCode: function (DOMObject, shapeId) {
        var uniqueId = new Date().getTime();
        var value = this.getValue(shapeId);

        var div = document.createElement("div");
        div.className = "textLine";

        var labelDiv = document.createElement("div");
        labelDiv.className = "label";
        labelDiv.textContent = this.name;

        div.appendChild(labelDiv);

        var text = document.createElement("textarea");
        text.className = "text"; //required for onkeydown
        text.value = value;
        text.spellcheck = false;
        text.className = "textarea";
        div.appendChild(document.createElement("br"));
        div.appendChild(text);

        // used to change Text property
        text.onchange = function (shapeId, property, scorpe) {
            return function () {
                // update shape but without adding {Command} to the {History}
                scorpe.PropertyValue = this.value;
                updateShape(shapeId, property, this.value);
            };
        } (shapeId, this.property, this);

        text.onmouseout = text.onchange;
        text.onkeyup = text.onchange;
        DOMObject.appendChild(div);
    },


    /**Generate the code to edit the text.
    *The text got updated when you leave the input area
    *
    *@param {HTMLElement} DOMObject - the div of the properties panel
    *@param {Number} figureId - the id of the figure we are using
    **/
    generateSingleTextCode: function (DOMObject, figureId) {
        var uniqueId = new Date().getTime();
        var value = this.getValue(figureId);

        var div = document.createElement("div");
        div.className = "line";

        var labelDiv = document.createElement("div");
        labelDiv.className = "label";
        labelDiv.textContent = this.name;
        //        var icon = new Image();
        //        icon.className = 'prop-icon';
        //        icon.src = Builder.IMAGE_TEXT_ICON_PATH;
        //        labelDiv.appendChild(icon);

        div.appendChild(labelDiv);

        var text = document.createElement("input");
        text.type = "text";
        text.className = "text"; //required for onkeydown
        text.value = value;
        div.appendChild(text);

        text.onchange = function (figureId, property, scop) {
            return function () {
                scop.PropertyValue = this.value;
                updateShape(figureId, property, this.value);
            }
        } (figureId, this.property, this);

        text.onmouseout = text.onchange;
        text.onkeyup = text.onchange;
        DOMObject.appendChild(div);
    },
    generateSingleTextReadOnlyCode: function (DOMObject, figureId) {
        var uniqueId = new Date().getTime();
        var value = this.getValue(figureId);

        var div = document.createElement("div");
        div.className = "line";

        var labelDiv = document.createElement("div");
        labelDiv.className = "label";
        labelDiv.textContent = this.name;

        div.appendChild(labelDiv);

        var text = document.createElement("input");
        text.type = "text";
        text.className = "textReadOnly"; //required for onkeydown
        text.value = value;
        text.setAttribute("readOnly", true);
        div.appendChild(text);

        text.onchange = function (figureId, property, scop) {
            return function () {
                scop.PropertyValue = this.value;
                updateShape(figureId, property, this.value);
            }
        } (figureId, this.property, this);

        text.onmouseout = text.onchange;
        text.onkeyup = text.onchange;
        DOMObject.appendChild(div);
    },

    /**Generate the code to edit the URL.
    *The URL got updated when you leave the input area
    *
    *@param {HTMLElement} DOMObject - the div of the properties panel
    *@param {Number} figureId - the id of the figure we are using
    **/
    generateURLCode: function (DOMObject, figureId) {
        var uniqueId = new Date().getTime();
        var value = this.getValue(figureId);

        var div = document.createElement("div");
        div.className = "line";

        var labelDiv = document.createElement("div");
        labelDiv.className = "label";
        labelDiv.textContent = this.name;

        var icon = new Image();
        icon.className = 'prop-icon';
        icon.src = Builder.IMAGE_URL_ICON_PATH;
        labelDiv.appendChild(icon);

        div.appendChild(labelDiv);

        var text = document.createElement("input");
        text.type = "text";
        text.className = "text"; //required for onkeydown
        text.value = value;
        div.appendChild(text);

        text.onchange = function (figureId, property) {
            return function () {
                Log.info("Builder.generateURLCode() value: " + this.value);
                updateShape(figureId, property, this.value);
            }
        } (figureId, this.property);


        text.onmouseout = text.onchange;
        text.onkeyup = text.onchange;
        DOMObject.appendChild(div);
    },
    generateGroupLabelCode: function (DOMObject, figureId) {
        var uniqueId = new Date().getTime();
        var value = this.getValue(figureId);

        var div = document.createElement("div");
        div.className = "line";

        var labelDiv = document.createElement("div");
        labelDiv.className = "GroupLabel";
        labelDiv.textContent = this.name;
        div.appendChild(labelDiv);

        DOMObject.appendChild(div);
    },
    /**Used to generate a upload file
    *
    *@param {HTMLElement} DOMObject - the div of the properties panel
    *@param {Number} figureId - the id of the figure we are using
    */
    generateUploadCode: function (DOMObject, figureId) {
        var uniqueId = new Date().getTime();
        var value = this.getValue(figureId);

        var div = document.createElement("div");
        div.className = "line";

        var labelDiv = document.createElement("div");
        labelDiv.className = "label";
        labelDiv.textContent = this.name;

        div.appendChild(labelDiv);

        var upload = document.createElement("input");
        upload.type = "button";
        upload.className = BuilderProperty.BUTTON_CHECKER_CLASS;
        upload.value = "选择文件";
        div.appendChild(upload);

        upload.onclick = function (figureId, property) {
            return function () {
                showInsertImageDialog();
                // property value stores in custom attribute
                var currentValue = this.getAttribute(BuilderProperty.BUTTON_CHECKED_ATTRIBUTE);
                // new value is inverse of the current one
                var newValue = currentValue == "true" ? false : true;
                // update control first
                this.setAttribute(BuilderProperty.BUTTON_CHECKED_ATTRIBUTE, newValue);
                Log.info("Builder.generateButtonCheckerCode() value: " + newValue);
                updateShape(figureId, property, newValue);
            };
        } (figureId, this.property);

        DOMObject.appendChild(div);
    },
    /**Used to generate a linkbutton for open window
    *
    *@param {HTMLElement} DOMObject - the div of the properties panel
    *@param {Number} figureId - the id of the figure we are using
    */
    generateCCFormLinkCode: function (DOMObject, figureId) {
        var uniqueId = new Date().getTime();
        var value = this.getValue(figureId);

        var div = document.createElement("div");
        div.className = "line";

        var href = document.createElement("a");

        href.href = "javascript:CCForm_ShowDialog('" + this.PropertyValue + "','" + this.name + "');";
        div.appendChild(href);

        var labelDiv = document.createElement("div");
        labelDiv.className = "label";
        labelDiv.textContent = this.name;
        href.appendChild(labelDiv);

        var icon = new Image();
        icon.className = 'prop-icon';
        icon.src = "Img/iconLink.png";
        labelDiv.appendChild(icon);

        DOMObject.appendChild(div);
    },
    /**Used to generate a drop down menu
    *
    *@param {HTMLElement} DOMObject - the div of the properties panel
    *@param {Number} figureId - the id of the figure we are using
    *@param {Array} v - a vector or hashes ex: [{Text:'Normal', Value:'Normal'},{Text:'Arrow', Value:'Arrow'}]
    */
    generateArrayCode: function (DOMObject, figureId, v) {
        //        Log.info("Font size length: " + v.length);
        var uniqueId = new Date().getTime();

        var value = this.getValue(figureId);

        var div = document.createElement("div");
        div.className = "line";

        var labelDiv = document.createElement("div");
        labelDiv.className = "label";
        labelDiv.textContent = this.name;

        // get last name of property to define it's icon
        var propNames = this.property.split('.');
        var propLastName = propNames.pop();

        var icon;
        switch (propLastName) {
            case "lineWidth":
                icon = new Image();
                icon.className = 'prop-icon';
                icon.src = Builder.IMAGE_LINEWIDTH_ICON_PATH;
                labelDiv.appendChild(icon);
                break;
            case "startStyle":
                icon = new Image();
                icon.className = 'prop-icon';
                icon.src = Builder.IMAGE_STARTSTYLE_ICON_PATH;
                labelDiv.appendChild(icon);
                break;
            case "endStyle":
                icon = new Image();
                icon.className = 'prop-icon';
                icon.src = Builder.IMAGE_ENDSTYLE_ICON_PATH;
                labelDiv.appendChild(icon);
                break;
            case "lineStyle":
                icon = new Image();
                icon.className = 'prop-icon';
                icon.src = Builder.IMAGE_LINESTYLE_ICON_PATH;
                labelDiv.appendChild(icon);
                break;
        }

        div.appendChild(labelDiv);

        var select = document.createElement("select");
        select.style.cssText = "float: right;";
        select.id = this.property; // for DOM manipulation
        div.appendChild(select);

        for (var i = 0; i < v.length; i++) {
            var option = document.createElement("option");
            option.value = v[i].Value;
            //            Log.info("\t Text : " + v[i].Text + " Value : " + v[i].Value);
            option.text = v[i].Text; //see: http://www.w3schools.com/jsref/coll_select_options.asp
            select.options.add(option); //push does not exist in the options array
            if (option.value == value) {
                option.selected = true;
            }
        }

        select.onchange = function (figureId, property, scorpe) {
            return function () {
                var selecedValue = this.options[this.selectedIndex].value;
                scorpe.PropertyValue = selecedValue;
                updateShape(figureId, property, selecedValue);
            };
        } (figureId, this.property, this);

        DOMObject.appendChild(div);
    },


    /**
    *Used to generate a color picker
    *
    *@param{HTMLElement} DOMObject - the div of the properties panel
    *@param{Number} figureId - the id of the figure we are using
    */
    generateColorCode: function (DOMObject, figureId) {
        var value = this.getValue(figureId);

        var uniqueId = new Date().getTime();
        var div = document.createElement("div");
        div.className = "line";

        var labelDiv = document.createElement("div");
        labelDiv.className = "label";
        labelDiv.textContent = this.name;

        // get last name of property to define it's icon
        var propNames = this.property.split('.');
        var propLastName = propNames.pop();

        var icon;
        switch (propLastName) {
            case "fillStyle":
                icon = new Image();
                icon.className = 'prop-icon';
                icon.src = Builder.IMAGE_FILL_ICON_PATH;
                labelDiv.appendChild(icon);
                break;
            case "strokeStyle":
                icon = new Image();
                icon.className = 'prop-icon';
                icon.src = Builder.IMAGE_STROKE_ICON_PATH;
                labelDiv.appendChild(icon);
                break;
        }

        div.appendChild(labelDiv);

        var colorSelectorDiv = document.createElement("div");
        colorSelectorDiv.id = 'colorSelector' + uniqueId;
        colorSelectorDiv.className = 'color-selector';

        var colorInput = document.createElement("input");
        colorInput.type = "text";
        colorInput.id = 'colorpickerHolder' + uniqueId;
        colorInput.value = value;
        colorSelectorDiv.appendChild(colorInput);

        div.appendChild(colorSelectorDiv);

        DOMObject.appendChild(div);

        var colorPicker = document.getElementById('colorpickerHolder' + uniqueId);

        //let plugin do the job
        $(colorPicker).colorPicker();

        //on change update the figure
        //var propExposedToAnonymous = this.property;
        colorPicker.onchange = function (figureId, property, scorpe) {
            return function () {
                var selecedValue = colorPicker.value.toUpperCase();
                scorpe.PropertyValue = selecedValue;
                updateShape(figureId, property, selecedValue);
            };
        } (figureId, this.property, this);
    },

    /**Generate the code to edit the boolean property with button.
    *Result control has 2 modes: checked/unchecked.
    *The property got updated on click
    *
    *@param {HTMLElement} DOMObject - the div of the properties panel
    *@param {Number} figureId - the id of the figure we are using
    **/
    generateButtonCheckerCode: function (DOMObject, figureId, buttonLabel) {
        var value = this.getValue(figureId);

        var div = document.createElement("div");
        div.className = "line";

        var labelDiv = document.createElement("div");
        labelDiv.className = "label";
        labelDiv.textContent = this.name;
        div.appendChild(labelDiv);

        var buttonChecker = document.createElement("input");
        buttonChecker.type = "button";
        buttonChecker.id = this.property; // for DOM manipulation
        buttonChecker.className = BuilderProperty.BUTTON_CHECKER_CLASS;
        buttonChecker.value = buttonLabel; // for now we have button checking only for underlined text property
        // property value stores in custom attribute
        buttonChecker.setAttribute(BuilderProperty.BUTTON_CHECKED_ATTRIBUTE, value);
        div.appendChild(buttonChecker);

        buttonChecker.onclick = function (figureId, property) {
            return function () {
                // property value stores in custom attribute
                var currentValue = this.getAttribute(BuilderProperty.BUTTON_CHECKED_ATTRIBUTE);
                // new value is inverse of the current one
                var newValue = currentValue == "true" ? false : true;
                // update control first
                this.setAttribute(BuilderProperty.BUTTON_CHECKED_ATTRIBUTE, newValue);
                Log.info("Builder.generateButtonCheckerCode() value: " + newValue);
                updateShape(figureId, property, newValue);
            };
        } (figureId, this.property);

        DOMObject.appendChild(div);
    },


    /**We use this to return a value of the property for a figure,
    *Similar to Javas Class.forname...sort of anyway
    *We need this because passing direct references to simple data types (including strings) 
    *only passes the value, not a reference to that value (call by value not by reference)
    *
    *@param{Number} figureId - the id of the shape {Figure} or {Connector} we are using, could also be the canvas (figureId = 'a')
    */
    getValue: function (figureId) {
        //Is it a Figure? 
        var obj = STACK.figureGetById(figureId);

        //Is it a Connector ?
        if (obj == null) { //ok so it's not a Figure...so it should be a Connector
            obj = CONNECTOR_MANAGER.connectorGetById(figureId);
        }

        //Is it the Canvas?
        if (obj == null) {
            if (figureId == "canvas") {
                obj = canvas;
            }
        }

        //Is it a Container?
        if (obj == null) {
            obj = STACK.containerGetById(figureId);
        }
        Log.debug("Unsplit property: " + this.property);

        var propertyAccessors = this.property.split(".");
        //        Log.info("BuilderProperty::getValue() : propertyAccessors : " + propertyAccessors );
        for (var i = 0; i < propertyAccessors.length - 1; i++) {
            //            Log.info("\tBuilderProperty::getValue() : i = " + i  + ' name= ' + propertyAccessors[i]);
            obj = obj[propertyAccessors[i]];
        }

        //Log.info("Object type: " + obj.oType);


        var propName = propertyAccessors[propertyAccessors.length - 1];
        //Log.info("Property name: " + propName);

        var propGet = "get" + Util.capitaliseFirstLetter(propName);

        //null is allowed, undefined is not
        if (propGet in obj) { //@see https://developer.mozilla.org/en/JavaScript/Reference/Operators/Special_Operators/in_Operator
            var returnValue = obj[propGet]();
            if (typeof (returnValue) == "undefined") return this.PropertyValue;
            return obj[propGet]();
        }
        else {
            //Access the object property's
            var returnValue = obj[propertyAccessors[propertyAccessors.length - 1]];
            if (typeof (returnValue) == "undefined") return this.PropertyValue;
            return obj[propertyAccessors[propertyAccessors.length - 1]];
        }
    }
};



/**
* This instance is responsible for creating and updating Text Editor Popup.
* Text Editor Popup is made out of:
*  - editor - a <div> (inside #container <div>) that contains the text and reflects the 
*  - tools - a <div> (inside #container <div>) that will contain all the buttons and options to format text
*  
* @constructor
* @this {TextEditorPopup}
* @param {HTMLElement} editor - the DOM object (a <div> inside editor page) to create Text Editor Popup
* @param {HTMLElement} tools - the DOM object (a <div> inside editor page) to create Text Editor Tools 
* @param  shape - the {Figure} or {Connector} - parent of Text primitive
* @param {Number} textPrimitiveId - the id value of Text primitive child of shape for which the properties will be displayed
* @author Artyom Pokatilov <artyom.pokatilov@gmail.com>
*/
function TextEditorPopup(editor, tools, shape, textPrimitiveId) {
    this.editor = editor;
    this.tools = tools;
    this.shape = shape;
    this.textPrimitiveId = textPrimitiveId;

    /*We need to construct the full path to the properties of Text*/
    // beginning of property string of BuilderProperty for primitive
    var propertyPrefix;

    if (this.shapeIsAConnector()) {
        // in case of connector with primitive = middleText
        propertyPrefix = "middleText.";
    } else {
        // in case of figure with primitive.id = textPrimitiveId
        propertyPrefix = "primitives." + this.textPrimitiveId + ".";
    }

    // value of BuiderProperty::property
    this.stringPropertyName = propertyPrefix + TextEditorPopup.STRING_PROPERTY_ENDING;
    this.sizePropertyName = propertyPrefix + TextEditorPopup.SIZE_PROPERTY_ENDING;
    this.fontPropertyName = propertyPrefix + TextEditorPopup.FONT_PROPERTY_ENDING;
    this.alignPropertyName = propertyPrefix + TextEditorPopup.ALIGN_PROPERTY_ENDING;
    this.colorPropertyName = propertyPrefix + TextEditorPopup.COLOR_PROPERTY_ENDING;
    this.underlinedPropertyName = propertyPrefix + TextEditorPopup.UNDERLINED_PROPERTY_ENDING;
}

/**A set of predefined properties fragments*/
TextEditorPopup.STRING_PROPERTY_ENDING = 'str';
TextEditorPopup.SIZE_PROPERTY_ENDING = 'size';
TextEditorPopup.FONT_PROPERTY_ENDING = 'font';
TextEditorPopup.ALIGN_PROPERTY_ENDING = 'align';
TextEditorPopup.COLOR_PROPERTY_ENDING = 'style.fillStyle';
TextEditorPopup.UNDERLINED_PROPERTY_ENDING = 'underlined';

TextEditorPopup.prototype = {

    constructor: TextEditorPopup,

    /**
    *Returns true if target shape of TextEditorPopup is a Connector
    *@return {Boolean} - true shape property is a connector
    *@author Artyom Pokatilov <artyom.pokatilov@gmail.com>
    **/
    shapeIsAConnector: function () {
        return this.shape.oType === "Connector";
    },



    /**
    *Creates DOM structure and bind events
    *@author Artyom Pokatilov <artyom.pokatilov@gmail.com>
    **/
    init: function () {
        var textarea;

        // <div> for text tools contains font size, font family, alignment and color
        for (var i = 0; i < this.shape.properties.length; i++) {
            var curProperty = this.shape.properties[i].property; //get property in long format ex: primitives.1.style.fillStyle
            if (curProperty != null) {
                var curValue = this.shape.properties[i].getValue(this.shape.id);
                switch (curProperty) {
                    case this.stringPropertyName:
                        this.shape.properties[i].injectInputArea(this.editor, this.shape.id);
                        textarea = this.editor.getElementsByTagName('textarea')[0];

                        // remove all <br> tags from text-editor as they were added by injectInputArea method 
                        removeNodeList(this.editor.getElementsByTagName('br')); //defined in util.js

                        // set Text editor properties on initialization
                        this.setProperty(curProperty, curValue);

                        break;

                    case this.sizePropertyName:
                    case this.fontPropertyName:
                    case this.alignPropertyName:
                    case this.colorPropertyName:
                    case this.underlinedPropertyName:
                        this.shape.properties[i].injectInputArea(this.tools, this.shape.id);

                        // set Text editor properties on initialization
                        this.setProperty(curProperty, curValue);

                        break;
                }
            }
        }

        this.editor.className = 'active';
        this.tools.className = 'active';

        this.placeAndAutoSize();

        // select all text inside textarea (like in Visio)
        setSelectionRange(textarea, 0, textarea.value.length);
    },


    /**
    * Changing property inside Text Editor
    * provides WYSIWYG functionality
    * @param {String} property - property name that is being edited (in dotted notation)
    * @param {Object} value - the value to set the property to
    * @author Artyom Pokatilov <artyom.pokatilov@gmail.com>
    **/
    setProperty: function (property, value) {
        var textarea = this.editor.getElementsByTagName('textarea')[0];
        switch (property) {

            case this.sizePropertyName:
                // set new property value to editor's textarea
                textarea.style.fontSize = value + 'px';

                // set new property value to editor's tool
                document.getElementById(property).value = value;
                break;

            case this.fontPropertyName:
                // set new property value to editor's textarea
                textarea.style.fontFamily = value;

                // set new property value to editor's tool
                document.getElementById(property).value = value.toLowerCase();
                break;

            case this.alignPropertyName:
                // set new property value to editor's textarea
                textarea.style.textAlign = value;

                // IE doesn't apply text-align property correctly to all lines of the textarea on a fly
                // that is why we just copy it's text and paste it back to refresh text rendering
                if (Browser.msie) {
                    textarea.value = textarea.value;
                }

                // set new property value to editor's tool
                document.getElementById(property).value = value;
                break;

            case this.underlinedPropertyName:
                // set new property value to editor's textarea
                textarea.style.textDecoration = value == true ? 'underline' : '';

                // set new property value to editor's tool
                document.getElementById(property).setAttribute(BuilderProperty.BUTTON_CHECKED_ATTRIBUTE, value);
                break;

            case this.colorPropertyName:
                // set new property value to editor's textarea
                textarea.style['color'] = value;

                // set new property value to editor's tool (colorPicker)
                var colorPicker = this.tools.getElementsByClassName('color_picker')[0];
                colorPicker.style['background-color'] = value; //change the color to the proper one
                colorPicker.previousSibling.value = value; //set the value to the "hidden" text field
                break;
        }

        this.placeAndAutoSize();
    },


    /**
    *Places and sets size to the property panel
    *@author Artyom Pokatilov <artyom.pokatilov@gmail.com>
    **/
    placeAndAutoSize: function () {
        var textarea = this.editor.getElementsByTagName('textarea')[0];

        // set edit dialog position to top left (first) bound point of Text primitive
        var textBounds;

        if (this.shapeIsAConnector()) {
            // in case of connector primitive is a middleText property
            textBounds = this.shape.middleText.getBounds();
        } else {
            // in case of connector primitive is a primitives[this.textPrimitiveId] property
            textBounds = this.shape.primitives[this.textPrimitiveId].getBounds();
        }

        // change coordinates of editing Text primitive to include padding and border of Text Editor
        var leftCoord = textBounds[0] - defaultEditorBorderWidth - defaultEditorPadding;
        var topCoord = textBounds[1] - defaultEditorBorderWidth - defaultEditorPadding;

        var textareaWidth = textBounds[2] - textBounds[0];
        var textareaHeight = textBounds[3] - textBounds[1];

        // Firefox includes border & padding as part of width and height,
        // so width and height should additionally include border and padding twice
        // (similar to "feather" option in Fireworks)
        if (Browser.mozilla) {
            textareaHeight += (defaultEditorPadding) * 2;
            topCoord -= (defaultEditorPadding);
            textareaWidth += (defaultEditorPadding) * 2;
            leftCoord -= (defaultEditorPadding);
        }

        // some of IE magic:
        // enough to add half of font-size to textarea's width to prevent auto-breaking to next line
        // which is wrong in our case
        // (similar to "feather" option in Fireworks)
        if (Browser.msie) {
            var fontSize = parseInt(textarea.style['font-size'], 10);
            textareaWidth += fontSize / 2;
            leftCoord -= fontSize / 4;
        }

        this.editor.style.left = leftCoord + "px";
        this.editor.style.top = topCoord + "px";


        // visibility: 'hidden' allows us to get proper size but 
        // without getting strange visual artefacts (tiggered by settings positions & other)
        this.tools.style.visibility = 'hidden';

        // We set it to the left upper corner to get it's objective size
        this.tools.style.left = '0px';
        this.tools.style.top = '0px';

        // Get toolbox height and width. Notice that clientHeight differs from offsetHeight.
        //@see https://developer.mozilla.org/en/docs/DOM/element.offsetHeight
        //@see http://stackoverflow.com/questions/4106538/difference-between-offsetheight-and-clientheight
        var toolboxHeight = this.tools.offsetHeight;
        var toolboxWidth = this.tools.offsetWidth;

        // define toolbox left position
        var toolboxLeft = leftCoord;

        // get width of work area (#container <div> from editor)
        var workAreaWidth = getWorkAreaContainer().offsetWidth;

        // If it's not enough place for toolbox at the page right side
        if (toolboxLeft + toolboxWidth >= workAreaWidth - scrollBarWidth) {
            // then shift toolbox to left before it can be placed
            toolboxLeft = workAreaWidth - toolboxWidth - scrollBarWidth;
        }

        // define toolbox top position
        var toolboxTop = topCoord - toolboxHeight;
        // If it's not enough place for toolbox at the page top
        if (toolboxTop <= 0) {
            // then place toolbox below textarea
            toolboxTop = topCoord + toolboxHeight + defaultEditorBorderWidth + defaultEditorPadding;
        }

        this.tools.style.left = toolboxLeft + "px";
        this.tools.style.top = toolboxTop + "px";

        // return normal visibility to toolbox
        this.tools.style.visibility = 'visible';

        textarea.style.width = textareaWidth + "px";
        textarea.style.height = textareaHeight + "px";
    },

    /**
    *Removes DOM structure of editor and it's tools
    *@author Artyom Pokatilov <artyom.pokatilov@gmail.com>
    **/
    destroy: function () {
        this.editor.className = '';
        this.editor.style.cssText = '';
        this.editor.innerHTML = '';

        this.tools.className = '';
        this.tools.style.cssText = '';
        this.tools.innerHTML = '';
    },


    /**
    *Returns true if mouse clicked inside TextEditorPopup
    *@param {Event} e - mouseDown event object
    *@return {boolean} - true if clicked inside
    *@author Artyom Pokatilov <artyom.pokatilov@gmail.com>
    **/
    mouseClickedInside: function (e) {
        var target = e.target;

        // check if user fired mouse down on the part of editor, it's tools or active color picker
        // actually active color picker in that moment can be only for Text edit
        var inside = target.id === this.editor.id
           || target.parentNode.id === this.editor.id
           || target.parentNode.parentNode.id === this.editor.id

           || target.id === this.tools.id
           || target.parentNode.id === this.tools.id
           || target.parentNode.parentNode.id === this.tools.id

           || target.className === 'color_picker'

           || target.id === 'color_selector'
           || target.parentNode.id === 'color_selector'
           || target.parentNode.parentNode.id === 'color_selector';

        return inside;
    },

    /**
    * Checks if TextEditorPopup refers to target shape and id of Text primitive
    * @param  shape - target figure or connector to check
    * @param {Number} textPrimitiveId - the id value of a target Text primitive
    *
    *@return {Boolean} - true if refers to target objects
    *@author Artyom Pokatilov <artyom.pokatilov@gmail.com>
    **/
    refersTo: function (shape, textPrimitiveId) {
        var result = this.shape.equals(shape);

        // in case of connector textPrimitiveId will be underfined
        if (textPrimitiveId != null) {
            result &= this.textPrimitiveId === textPrimitiveId;
        }
        return result;
    },

    /**
    * Manually triggers onblur event of textarea inside TextEditor.
    * @author Artyom Pokatilov <artyom.pokatilov@gmail.com>
    **/
    blurTextArea: function () {
        var textarea = this.editor.getElementsByTagName('textarea')[0];
        textarea.onblur();
    }

};