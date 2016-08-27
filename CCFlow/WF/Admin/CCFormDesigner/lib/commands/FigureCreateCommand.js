/** 
* An 'interface' for undoable actions, implemented by classes that specify 
* how to handle action
* 
* 
* @this {FigureCreateCommand} 
* @constructor
* @param {Function} factoryFunction - the function that will create the {Figure}. It will be local copy (of original pointer)
* @param {Number} x - the x coordinates
* @param {Number} y - the x coordinates
* @author Alex <alex@scriptoid.com>
*  @author Artyom Pokatilov <artyom.pokatilov@gmail.com>
*/
function FigureCreateCommand(factoryFunction, x, y) {
    this.oType = 'FigureCreateCommand';

    /**Any sequence of many mergeable actions can be packed by the history*/
    this.mergeable = false;
    this.factoryFunction = factoryFunction;
    this.x = x;
    this.y = y;
    this.firstExecute = true;
    this.figureId = null;
}


FigureCreateCommand.prototype = {
    /**This method got called every time the Command must execute*/
    execute: function () {
        if (this.firstExecute) {
            var canAddFigure = true;
            //create figure
            var createdFigure = this.factoryFunction(this.x, this.y);
            //CCForm private Property
            createdFigure.CCForm_Shape = createFigureName;
            //move it into position
            createdFigure.transform(Matrix.translationMatrix(this.x - createdFigure.rotationCoords[0].x, this.y - createdFigure.rotationCoords[0].y))
            createdFigure.style.lineWidth = defaultLineWidth;
            //ccflow business logic
            switch (createFigureName) {

                case CCForm_Controls.Label:
                case CCForm_Controls.Button:
                case CCForm_Controls.HyperLink:
                case CCForm_Controls.Image:
                    createdFigure.CCForm_MyPK = Util.NewGUID();
                    break;
                case CCForm_Controls.TextBox:
                case CCForm_Controls.TextBoxInt:
                case CCForm_Controls.TextBoxFloat:
                case CCForm_Controls.TextBoxMoney:
                case CCForm_Controls.Date:
                case CCForm_Controls.DateTime:
                case CCForm_Controls.CheckBox:
                    canAddFigure = false;
                    this.DataFieldCreate(createdFigure, this.x, this.y);
                    break;
                case CCForm_Controls.ListBox:
                case CCForm_Controls.HiddendField:
                    alert('目前还没有对{' + createFigureName + '}提供该控件的支持.');
                    return;
                case CCForm_Controls.RadioButton:
                case CCForm_Controls.DropDownListEnum:
                    canAddFigure = false;
                    this.RadioButtonCreate(createdFigure, this.x, this.y);
                    break;
                case CCForm_Controls.RadioButton:
                    canAddFigure = false;
                    this.RadioButtonCreate(createdFigure, this.x, this.y);
                    break;
            }

            if (canAddFigure == true) {
                //store id for later use
                //TODO: maybe we should try to recreate it with same ID (in case further undo will recreate objects linked to this)
                this.figureId = createdFigure.id;

                //add to STACK
                STACK.figureAdd(createdFigure);

                //make this the selected figure
                selectedFigureId = createdFigure.id;

                //set up it's editor
                setUpEditPanel(createdFigure);

                //move to figure selected state
                state = STATE_FIGURE_SELECTED;
            }
            this.firstExecute = false;
        }
        else { //redo
            throw "Not implemented";
        }
    },
    /**创建数据字段**/
    DataFieldCreate: function (createdFigure, x, y) {

        var dgId = "iframeTextBox";

        //        switch (createFigureName) {
        //            case CCForm_Controls.ListBox:
        //                alert('尚未完成.');
        //                return;
        //            default:
        //                break;
        //        }
        // alert(createFigureName);

        var url = "DialogCtr/FrmTextBox.htm?DataType=" + createFigureName + "&s=" + Math.random();
        var funIsExist = this.IsExist;

        OpenEasyUiDialog(url, dgId, '新建文本', 600, 394, 'icon-new', true, function (HidenFieldFun) {
            var win = document.getElementById(dgId).contentWindow;
            var newFormFieldInfo = win.getNewTBInfo();

            if (newFormFieldInfo.ZH_CN_FieldName == null || newFormFieldInfo.ZH_CN_FieldName.length == 0) {
                $.messager.alert('错误', '字段名称不能为空。', 'error');
                return false;
            }
            if (newFormFieldInfo.En_FieldName == null || newFormFieldInfo.En_FieldName.length == 0) {
                $.messager.alert('错误', '英文字段不能为空。', 'error');
                return false;
            }
            //判断主键是否存在
            var isExit = funIsExist(newFormFieldInfo.En_FieldName);
            if (isExit == true) {
                $.messager.alert("错误", "已存在ID为(" + newFormFieldInfo.En_FieldName + ")的元素，不允许添加同名元素！", "error");
                return false;
            }

            //控件数据类型
            if (newFormFieldInfo.FieldType == "1") {
                createdFigure.CCForm_Shape = "TextBox_Str";
            } else if (newFormFieldInfo.FieldType == "2") {
                createdFigure.CCForm_Shape = "TextBox_Int";
            } else if (newFormFieldInfo.FieldType == "3") {
                createdFigure.CCForm_Shape = "TextBox_Float";
            } else if (newFormFieldInfo.FieldType == "4") {
                createdFigure.CCForm_Shape = "TextBox_Boolean";
            } else if (newFormFieldInfo.FieldType == "5") {
                createdFigure.CCForm_Shape = "TextBox_Double";
            } else if (newFormFieldInfo.FieldType == "6") {
                createdFigure.CCForm_Shape = "TextBox_Date";
            } else if (newFormFieldInfo.FieldType == "7") {
                createdFigure.CCForm_Shape = "TextBox_DateTime";
            } else if (newFormFieldInfo.FieldType == "8") {
                createdFigure.CCForm_Shape = "TextBox_Money";
            }

            //如果为隐藏字段
            if (newFormFieldInfo.IsHidenField == true) {
                HidenFieldFun(newFormFieldInfo);
            } else {

                //根据信息创建不同类型的数字控件
                var transField = new TransFormDataField(createdFigure, newFormFieldInfo, x, y);

                // 定义参数，让其保存到数据库里。
                var param = {
                    action: "DoType",
                    DoType: "NewField",
                    v1: CCForm_FK_MapData,
                    v2: newFormFieldInfo.En_FieldName,
                    v3: newFormFieldInfo.ZH_CN_FieldName,
                    v4: newFormFieldInfo.FieldType,
                    v5: x,
                    v6: y
                };
                ajaxService(param, function (json) {
                    if (json == "true") {
                        //开始画这个-元素.
                        transField.paint();
                    } else {
                        Designer_ShowMsg(json);
                    }
                }, this);
            }
        }, this.HidenFieldCreate);

        return false;
    },
    /**创建单选按钮**/
    RadioButtonCreate: function (createdFigure, x, y) {
        var dgId = "iframeRadioButton";
        var url = "DialogCtr/FrmEnumeration.htm?DataType=&s=" + Math.random();
        var funIsExist = this.IsExist;

        var lab = '新建单选按钮';

        OpenEasyUiDialog(url, dgId, lab, 650, 394, 'icon-new', true, function () {
            var win = document.getElementById(dgId).contentWindow;
            var newFormFieldInfo = win.getNewTBInfo();

            if (newFormFieldInfo.ZH_CN_FieldName == null || newFormFieldInfo.ZH_CN_FieldName.length == 0) {
                $.messager.alert('错误', '字段名称不能为空。', 'error');
                return false;
            }
            if (newFormFieldInfo.En_FieldName == null || newFormFieldInfo.En_FieldName.length == 0) {
                $.messager.alert('错误', '英文字段不能为空。', 'error');
                return false;
            }
            //判断主键是否存在
            var isExit = funIsExist(newFormFieldInfo.En_FieldName);
            if (isExit == true) {
                $.messager.alert("错误", "已存在ID为" + newFormFieldInfo.En_FieldName + "的元素，不允许添加同名元素！", "error");
                return false;
            }

            //根据信息创建不同类型的数字控件
            var transField = new TransFormDataField(createdFigure, newFormFieldInfo, x, y);
            transField.paint();
        }, null);

        return false;
    },
    /**创建隐藏字段**/
    HidenFieldCreate: function (newFormFieldInfo) {
        var param = {
            action: "DoType",
            DoType: "NewHidF",
            v1: CCForm_FK_MapData,
            v2: newFormFieldInfo.En_FieldName,
            v3: newFormFieldInfo.ZH_CN_FieldName,
            v4: newFormFieldInfo.FieldType
        };
        ajaxService(param, function (json) {
            if (json == "true") {
            } else {
                Designer_ShowMsg(json);
            }
        }, this);
    },
    IsExist: function (MyPK) {
        var flag = false;
        for (f in STACK.figures) {
            if (STACK.figures[f].CCForm_MyPK == MyPK) {
                flag = true;
                break;
            }
        }
        return flag;
    },
    /**This method should be called every time the Command should be undone*/
    undo: function () {

        // if current figure is in text editing state
        if (state == STATE_TEXT_EDITING) {
            // remove current text editor
            currentTextEditor.destroy();
            currentTextEditor = null;
        }

        //remove it from container (if belongs to one)
        var containerId = CONTAINER_MANAGER.getContainerForFigure(this.figureId);
        if (containerId !== -1) {
            CONTAINER_MANAGER.removeFigure(containerId, this.figureId);
        }

        //remove figure
        STACK.figureRemoveById(this.figureId);

        //change state
        state = STATE_NONE;

        // set properties panel to canvas because current figure doesn't exist anymore
        setUpEditPanel(canvasProps);
    }
}

/**数据字段处理
**图片：Image
**生成Label
**/
function TransFormDataField(newfigure, newFormFieldInfo, x, y) {
    this.figure = newfigure;
    this.dataArrary = newFormFieldInfo;
    this.x = x;
    this.y = y;
}

TransFormDataField.prototype = {
    /**输出控件**/
    paint: function () {
        var createdFigure = this.figure;
        createdFigure.CCForm_MyPK = this.dataArrary.En_FieldName;
        //添加到Figures
        //add to STACK
        STACK.figureAdd(createdFigure);
        //add property
        createdFigure = this.Transform();
        //change text
        var figureText = STACK.figuresTextPrimitiveGetByFigureId(createdFigure.id);
        if (figureText != null) {
            figureText.setTextStr(this.dataArrary.En_FieldName);
        }
        //创建标签
        this.LabelCreateForFigure();
        draw();
    },
    /**根据控件类型，生成不同控件描述 and propertys**/
    Transform: function () {
        var createdFigure = this.figure;
        var propertys = CCForm_Control_Propertys.TextBox_Str;
        var shap_src = "/Data/TextBoxBig.png";
        switch (createdFigure.name) {
            case CCForm_Controls.TextBox:
                propertys = CCForm_Control_Propertys[createdFigure.CCForm_Shape];
                break;
            case CCForm_Controls.Date:
            case CCForm_Controls.DateTime:
                shap_src = "/Data/DatetimeBig.png";
                break;
            case CCForm_Controls.CheckBox:
                shap_src = "/Data/TextBoxBig.png";
                break;
            case CCForm_Controls.RadioButton:
                shap_src = "/Data/TextBoxBig.png";
                break;
        }
        //shap image
        var imageFrame = STACK.figuresImagePrimitiveGetByFigureId(createdFigure.id);
        if (imageFrame != null) imageFrame.setUrl(figureSetsURL + shap_src);
        //push property
        createdFigure.properties.push(new BuilderProperty('控件属性', 'group', BuilderProperty.TYPE_GROUP_LABEL));
        createdFigure.properties.push(new BuilderProperty(BuilderProperty.SEPARATOR));
        for (var i = 0; i < propertys.length; i++) {
            var defVal = propertys[i].DefVal ? propertys[i].DefVal : "";
            if (defVal == "FieldText") {
                //字段中文名
                defVal = this.dataArrary.ZH_CN_FieldName;
            } else if (defVal == "KeyOfEn") {
                //字段英文名
                defVal = this.dataArrary.En_FieldName;
            }
            //替换系统值
            defVal = this.DealExp(defVal);
            createdFigure.properties.push(new BuilderProperty(propertys[i].ProText, propertys[i].proName, propertys[i].ProType, defVal));
        }
        return createdFigure;
    },
    /**创建控件对应的标签**/
    LabelCreateForFigure: function () {
        var x = this.x;
        var y = this.y;
        //计算位移
        var moveX = (this.dataArrary.ZH_CN_FieldName.length * 12) + 60;
        moveX = moveX < 90 ? 90 : moveX;
        x = x - moveX;
        y = y - 15;
        //create
        var labelFigure = figure_Label(x, y);
        labelFigure.CCForm_MyPK = Util.NewGUID();
        //add to STACK
        STACK.figureAdd(labelFigure);
        //make this the selected figure
        selectedFigureId = labelFigure.id;
        //change text
        figureText = STACK.figuresTextPrimitiveGetByFigureId(selectedFigureId);
        if (figureText != null) {
            figureText.setTextStr(this.dataArrary.ZH_CN_FieldName);
        }
    },
    /**替换系统表达式值**/
    DealExp: function (expString) {
        try {
            expString = expString.replace(/@NodeID@/g, CCForm_FK_MapData);
            expString = expString.replace(/@KeyOfEn@/g, this.dataArrary.En_FieldName);
        } catch (e) {
        }
        return expString;
    }
}