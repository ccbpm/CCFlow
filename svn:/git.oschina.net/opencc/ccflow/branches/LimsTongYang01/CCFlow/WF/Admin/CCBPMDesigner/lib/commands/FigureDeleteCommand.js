/* 
* This is triggered when you delete a figure
* @this {FigureDeleteCommand} 
* @constructor
* @author Alex Gheorghiu <alex@scriptoid.com>
*/
function FigureDeleteCommand(figureId) {
    this.oType = 'FigureDeleteCommand';
    /**Any sequence of many mergeable actions can be packed by the history*/
    this.mergeable = false;
    this.figureId = figureId;
    this.deletedFigure = null;
    this.deletedGlues = null;
    this.deletedCPs = null;
    this.firstExecute = true;
}


FigureDeleteCommand.prototype = {

    /**This method got called every time the Command must execute*/
    execute: function () {
        if (this.firstExecute) {
            this.deletedFigure = STACK.figureGetById(this.figureId);
            //CCBPM delete NodeID
            var canDelete = true;
            if (this.deletedFigure.CCBPM_Shape != null) {
                //节点
                if (this.deletedFigure.CCBPM_Shape == CCBPM_Shape_Node) {
                    canDelete = confirm("删除节点就会删除节点表单运行数据，您确认要删除吗？");
                    if (canDelete == true) {
                        $.ajax({
                            type: 'POST',
                            url: Handler,
                            data: { action: 'DeleteNode', FK_Flow: CCBPM_Data_FK_Flow, FK_Node: this.deletedFigure.CCBPM_OID },
                            success: function (json) {

                                if (json.indexOf('err@') == 0) {
                                    canDelete = false;
                                    Designer_ShowMsg("err:" + json);
                                    //alert(json);
                                    return;
                                }

                            },
                            async: false
                        });
                    }
                }
                //事件
                if (this.deletedFigure.CCBPM_Shape == CCBPM_Shape_Event) {
                    if (this.deletedFigure.name == "StartEvent") {
                        canDelete = false;
                        Designer_ShowMsg("开始节点事件不允许删除。");
                    }
                    if (this.deletedFigure.name == "EndNone") {
                        canDelete = false;
                        Designer_ShowMsg("结束节点事件不允许删除。");
                    }
                }
                //网关
                if (this.deletedFigure.CCBPM_Shape == CCBPM_Shape_Gateway) {

                }
            }

            //是否允许删除
            if (canDelete == true) {
                //store deleted ConnectionPoints of target figure (safe copy)
                this.deletedCPs = CONNECTOR_MANAGER.connectionPointGetAllByParent(this.figureId);

                //store deleted Glues of all ConnectionPoints of target figure (safe copy)
                this.deletedGlues = [];

                var cpLength = this.deletedCPs.length;
                for (var k = 0; k < cpLength; k++) {
                    var glues = CONNECTOR_MANAGER.glueGetByFirstConnectionPointId(this.deletedCPs[k].id);
                    if (glues.length) {
                        this.deletedGlues.push(glues[0]);
                    }
                }

                //delete connector
                for (var m = 0; m < cpLength; m++) {
                    var cPoint = this.deletedCPs[m].point;
                    var cId = CONNECTOR_MANAGER.connectorGetByXY(cPoint.x, cPoint.y);
                    if (cId != -1) CONNECTOR_MANAGER.connectorRemoveById(cId, true);
                }

                //delete it
                STACK.figureRemoveById(this.figureId);

                //interface settings            
                selectedFigureId = -1;
                setUpEditPanel(canvasProps);
                state = STATE_NONE;

                this.firstExecute = false;
                //save canves
                save(false);
            }
        }
        else { //a redo
            throw "Not implemented";
        }
    },
    /**This method should be called every time the Command should be undone*/
    undo: function () {
        if (this.deletedFigure) {
            //add deleted ConnectionPoints back
            var length = this.deletedCPs.length;
            for (var i = 0; i < length; i++) {
                CONNECTOR_MANAGER.connectionPointAdd(this.deletedCPs[i]);
            }

            //add deleted Glues back
            length = this.deletedGlues.length;
            for (var j = 0; j < length; j++) {
                CONNECTOR_MANAGER.glueAdd(this.deletedGlues[j]);
            }

            //add deleted figure back
            //STACK.figureAdd(this.deletedFigure.clone());  //safe copy
            STACK.figureAdd(this.deletedFigure);
        }
        else {
            throw "No soted deleted figure";
        }
    }
}

