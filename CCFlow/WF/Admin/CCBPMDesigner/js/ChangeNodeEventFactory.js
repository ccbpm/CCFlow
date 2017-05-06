/***************
修改流程节点
****************/
function ChangeNodeManager(figureId) {
    this.figureId = figureId;
    this.figure = STACK.figureGetById(figureId);
}
//属性方法
ChangeNodeManager.prototype = {
    /**CCBPM- 普通节点**/
    NodeOrdinary: function () {
        this.figure.name = "NodeOrdinary";
        //imageframe change
        var imageFrame = STACK.figuresImagePrimitiveGetByFigureId(this.figureId);
        if (imageFrame != null) imageFrame.setUrl(figureSetsURL + "/Nodes/nodeOrdinary_big.png");
        //service
        this.NodeRunModel(this.figure.CCBPM_OID, this.figure.name);
        draw();
        save(false);
    },
    /**CCBPM- 分流节点**/
    NodeFL: function () {
        this.figure.name = "NodeFL";
        //imageframe change
        var imageFrame = STACK.figuresImagePrimitiveGetByFigureId(this.figureId);
        if (imageFrame != null) imageFrame.setUrl(figureSetsURL + "/Nodes/nodeFL_big.png");
        //service
        this.NodeRunModel(this.figure.CCBPM_OID, this.figure.name);
        draw();
        save(false);
    },
    /**CCBPM- 合流节点**/
    NodeHL: function () {
        this.figure.name = "NodeHL";
        //imageframe change
        var imageFrame = STACK.figuresImagePrimitiveGetByFigureId(this.figureId);
        if (imageFrame != null) imageFrame.setUrl(figureSetsURL + "/Nodes/nodeHL_big.png");
        //service
        this.NodeRunModel(this.figure.CCBPM_OID, this.figure.name);
        draw();
        save(false);
    },
    /**CCBPM- 分合流节点**/
    NodeFHL: function () {
        this.figure.name = "NodeFHL";
        //imageframe change
        var imageFrame = STACK.figuresImagePrimitiveGetByFigureId(this.figureId);
        if (imageFrame != null) imageFrame.setUrl(figureSetsURL + "/Nodes/nodeFHL_big.png");
        //service
        this.NodeRunModel(this.figure.CCBPM_OID, this.figure.name);
        draw();
        save(false);
    },
    /**CCBPM- 子线程节点**/
    NodeSubThread: function () {
        this.figure.name = "NodeSubThread";
        //imageframe change
        var imageFrame = STACK.figuresImagePrimitiveGetByFigureId(this.figureId);
        if (imageFrame != null) imageFrame.setUrl(figureSetsURL + "/Nodes/nodeSubThread_big.png");
        //service
        this.NodeRunModel(this.figure.CCBPM_OID, this.figure.name);
        draw();
        save(false);
    },
    /**CCBPM- 修改节点运行类型**/
    NodeRunModel: function (nodeID, runModel) {
        $.ajax({
            type: 'POST',
            url: Handler,
            data: { action: 'Node_ChangeRunModel', FK_Node: nodeID, RunModel: runModel },
            success: function (jsonData) {

            },
            async: false
        });
    }
}
