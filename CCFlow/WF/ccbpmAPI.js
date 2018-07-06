/*  ****************************  说明: ***************************
1. 该功能js是ccbpm常用的接口, 比如：发起，待办，在途，撤销.
2. 使用该js必须引用,如下.
    <script type="text/javascript" src="/WF/Scripts/bootstrap/js/jquery.min.js"></script>
    <script src="/WF/Scripts/config.js" type="text/javascript"></script>
    <script src="/WF/Comm/Gener.js" type="text/javascript"></script>
*/

/* 
 获得发起列表.
 1. 获得当前登录人员可以发起的流程.
 2. 返回的是一个json格式的数据集合，具有 No,Name,FK_FlowSort, 等列.
 3. 您调用这个方法后，可以生成您自己的发起页面,并连接到工作处理器上，比如: /WF/MyFlow.htm?FK_Flow=001
*/
function Start() {

    var handler = new HttpHandler("BP.WF.HttpHandler.WF");
    var data = handler.DoMethodReturnString("Start_Init");
    if (data.indexOf('err@') == 0) {
        alert(data);
        return;
    }
    data = JSON.parse(data);
    return data;
}

/* 
获得待办列表.
1. 获得当前登录人员未完成，但是参与的流程.
2. 返回的是一个json格式的数据集合，具有 WorkID,FK_Flow,FK_Node,Title 等列.
3. 您调用这个方法后，可以生成您自己的发起页面,并连接到工作处理器上. 比如: /WF/MyFlow.htm?FK_Flow=001&WorkID=9999&FK_Node=103
*/
function Todolist_Init() {

    var handler = new HttpHandler("BP.WF.HttpHandler.WF");
    var data = handler.DoMethodReturnString("Todolist_Init");
    if (data.indexOf('err@') == 0) {
        alert(data);
        return;
    }
    data = JSON.parse(data);
    return data;
}
 

/* 
获得在途列表.
1. 获得当前登录人员未完成，但是参与的流程.
2. 返回的是一个json格式的数据集合，具有 WorkID,FK_Flow,FK_Node,Title 等列.
3. 您调用这个方法后，可以生成您自己的发起页面,并连接到工作处理器上. 比如: /WF/WFRpt.htm?FK_Flow=001&WorkID=9999&FK_Node=103
*/
function Runing() {

    var handler = new HttpHandler("BP.WF.HttpHandler.WF");
    var data = handler.DoMethodReturnString("Runing_Init");
    if (data.indexOf('err@') == 0) {
        alert(data);
        return;
    }
    data = JSON.parse(data);
    return data;
}
 