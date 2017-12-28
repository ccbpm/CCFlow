/** 
* An 'interface' for undoable actions, implemented by classes that specify 
* how to handle action
* 
* 
* @this {FigureCreateCommand} 
* @constructor
* @param {Function} factoryFunction - the function that will create the {Figure}. It will be local copy (of original 

pointer)
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
            //create figure
            var createdFigure = this.factoryFunction(this.x, this.y);
            var figureText_Str = null;
            //move it into position
            createdFigure.transform(Matrix.translationMatrix(this.x - createdFigure.rotationCoords[0].x, this.y - createdFigure.rotationCoords[0].y))
            createdFigure.style.lineWidth = defaultLineWidth;

            //CCBPM create NodeID
            var canAddFigure = true;
            var delagetSave = false;
            if (createdFigure.CCBPM_Shape != null) {
                if (createdFigure.CCBPM_Shape == CCBPM_Shape_Node) {
                    $.ajax({
                        type: 'POST',
                        url: Handler,
                        data: { action: 'CreateNode', FK_Flow: CCBPM_Data_FK_Flow, FigureName: createdFigure.name, x: this.x, y: this.y },
                        success: function (jsonData) {

                            //  alert(jsonData);

                            var jData = $.parseJSON(jsonData);
                            createdFigure.CCBPM_OID = jData.NodeID;
                            figureText_Str = jData.Name;
                            delagetSave = true;
                        },
                        async: false
                    });
                }

                //事件
                if (createdFigure.CCBPM_Shape == CCBPM_Shape_Event) {
                    delagetSave = true;
                    if (createdFigure.name == "StartEvent") {
                        for (f in STACK.figures) {
                            if (STACK.figures[f].name == "StartEvent") {
                                delagetSave = false;
                                canAddFigure = false;
                                Designer_ShowMsg("开始节点事件已存在，本流程只允许增加一个开始节点事件。");
                                break;
                            }
                        }
                    }
                    if (createdFigure.name == "EndNone") {
                        for (f in STACK.figures) {
                            if (STACK.figures[f].name == "EndNone") {
                                delagetSave = false;
                                canAddFigure = false;
                                Designer_ShowMsg("结束节点事件已存在，本流程只允许增加一个结束节点事件。");
                                break;
                            }
                        }
                    }
                }

                //网关
                if (createdFigure.CCBPM_Shape == CCBPM_Shape_Gateway) {
                }
            }

            if (canAddFigure == true) {
                //store id for later use
                //TODO: maybe we should try to recreate it with same ID (in case further undo will recreate objects linked to this)
                this.figureId = createdFigure.id;

                //add to STACK
                STACK.figureAdd(createdFigure);

                //See if we need to add it to a container if we dropped it inside one
                var containerId = STACK.containerGetByXY(this.x, this.y);
                if (containerId !== -1) { //
                    var container = STACK.containerGetById(containerId);
                    if (Util.areBoundsInBounds(createdFigure.getBounds(), container.getBounds())) {
                        CONTAINER_MANAGER.addFigure(containerId, this.figureId);
                    }
                }


                //make this the selected figure
                selectedFigureId = createdFigure.id;

                //set up it's editor
                setUpEditPanel(createdFigure);

                //move to figure selected state
                state = STATE_FIGURE_SELECTED;

                this.firstExecute = false;

                //change text
                var figureText = STACK.figuresTextPrimitiveGetByFigureId(selectedFigureId);
                if (figureText_Str != null && figureText != null) {
                    figureText.setTextStr(figureText_Str);
                    draw();
                }

                //save figure
                if (delagetSave == true) {
                    save(false);
                }
            }
        }
        else { //redo
            throw "Not implemented";
        }
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