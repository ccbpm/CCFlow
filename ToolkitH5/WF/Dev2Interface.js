/*
 * 说明: &&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&& 菜单API  &&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
 * 1. 该API 是被发起、待办、在途三个菜单列表页面引入，并获取的数据的API。
 * 2. 其中需要  config.js 获得 ccbpmHostDevelopAPI 定义的服务器IP. 
 * 3. 需要cookies 中的sid 校验码(token).
 * 4. 获取流程发起列表: DB_Start()
 * 5. 获得待办 DB_Todolist()
 * 6. 获得在途 DB_Runing()
 * 7. 打开表单 OpenForm() 发起、待办、在途三个列表都需要打开表单.
 */

//获得发起列表.
function DB_Start() {
    var myurl = ccbpmHostDevelopAPI + "?DoType=DB_Start&Token=" + GetToken() + "&Domain=" + domain;
    return RunUrlReturnJSON(myurl);
}

/**
 * 1.获得待办. 返回的Json数据源.
 * 2.列: Title,WorkID,FK_Flow,FK_Node .
 * 3.获得该数据源,调用
 * */
function DB_Todolist() {
    var myurl = ccbpmHostDevelopAPI + "?DoType=DB_Todolist&Token=" + GetToken() + "&Domain=" + domain + "&t=" + new Date().getTime();
    return RunUrlReturnJSON(myurl);
}

//获得在途.
function DB_Runing() {
    var myurl = ccbpmHostDevelopAPI + "?DoType=DB_Runing&Token=" + GetToken() + "&Domain=" + domain;
    return RunUrlReturnJSON(myurl);
}

//获得草稿.
function DB_Draft() {
    var myurl = ccbpmHostDevelopAPI + "?DoType=DB_Draft&Token=" + GetToken() + "&Domain=" + domain;
    return RunUrlReturnJSON(myurl);
}

//获得流程注册表信息,返回没有完成的数据.
function DB_GenerWorkFlow(flowNo) {
    var myurl = ccbpmHostDevelopAPI + "?DoType=DB_GenerWorkFlow&Token=" + GetToken() + "&FK_Flow=" + flowNo;
    return RunUrlReturnJSON(myurl);
}

/**
 * 打开表单， 如果是仅仅传入的是FlowNo就是启动流程.
 * @param {any} flowNo
 * @param {any} nodeID
 * @param {any} workid
 * @param {any} fid
 * @param {any} paras
 */
function OpenForm(flowNo, nodeID, workid, fid, paras) {
    var url = GenerFrmUrl(flowNo, nodeID, workid, fid, paras);
    // 打开工作处理器.
    OpenMyFlow(url);
    //   window.open(url);
}

/**
 * 获得表单的 URL.
 * 该表单的URL存储在开始节点表单方案里.
 * @param {流程编号} flowNo
 * @param {节点ID默认为0} nodeID
 * @param {实例的ID} workid
 * @param {默认为:0} fid
 * @param {参数:格式为 &KeHuBianHao=001&KeHuMingCheng=新疆天业} paras
 */
function GenerFrmUrl(flowNo, nodeID = 0, workid = 0, fid = 0, paras = "") {

    debugger;
    // ccbpmHostDevelopAPI 变量是定义在 /config.js 的服务地址. 访问必须两个参数DoWhat,SID.
    //首先获得表单的URL.
    var myUrl = ccbpmHostDevelopAPI + "?DoType=GenerFrmUrl&Token=" + GetToken() + "&WorkID=" + workid + "&FK_Flow=" + flowNo + "&FK_Node=" + nodeID + "&FID=" + fid;
    var frmUrl = RunUrlReturnString(myUrl);
    frmUrl += paras;

    //如果包含了通用的工作处理器.
    if (frmUrl.indexOf("WF/MyFlow.htm") >= 0) {
        frmUrl = host + frmUrl;
    }
    return frmUrl;
}

/*   ******************************************************************* 开发接口JS: *******************************************************************
 * 1. 该文件里提供了一些高级开发接口,
 * 2. 比如：创建WorkID,执行发送,催办. 批量删除.
 * 3. 每个接口都有明确的注释.
 */


/**
 * 创建空白的WorkID.
 * @param {校验码(登录时候产生的)} sid
 * @param {流程编号} flowNo
 */
function Node_CreateBlankWorkID(flowNo) {
    var url = ccbpmHostDevelopAPI + "?DoType=Node_CreateBlankWorkID&Token=" + GetToken() + "&FK_Flow=" + flowNo;
    return RunUrlReturnString(url);
}

function Node_SetDraft(flowNo, workID) {
    var url = ccbpmHostDevelopAPI + "?DoType=Node_SetDraft&Token=" + GetToken() + "&FK_Flow=" + flowNo + "&WorkID=" + workID;
    return RunUrlReturnString(url);
}

function Node_IsCanDealWork(workID) {
    var url = ccbpmHostDevelopAPI + "?DoType=Node_Node_IsCanDealWork&Token=" + GetToken() + "&WorkID=" + workID;
    return RunUrlReturnString(url);
}
/**
 * 保存表单数据到流程实例中
 * @param {any} workid
 * @param {any} paras @Key1=val1@Key2=val2
 */
function Node_SaveParas(workid, paras) {

    //@mhj  这里要对参数格式执行校验,不符合不让保存.
    var url = ccbpmHostDevelopAPI + "?DoType=Node_SaveParas&Token=" + GetToken() + "&Paras=" + paras + "&WorkID=" + workid;
    return RunUrlReturnString(url);
}
/**
 * 执行发送
 * @param {工作实例ID} workid
 * @param {要达到的节点,为0不指定节点,由节点配置自动计算} toNodeID
 * @param {要发送的人员，为null,不指定人员，由流程配置自动计算} toEmps
 * @param {参数，格式为:@Key1=val1@Key2=val2 } paras
 */
function Node_SendWork(workid, toNodeID, toEmps, paras = "") {

    if (paras == null || paras == undefined)
        paras = "";
    paras = paras.replace('@', '&');

    var url = ccbpmHostDevelopAPI + "?DoType=Node_SendWork&Token=" + GetToken();
    url += "&WorkID=" + workid + "&ToNodeID=" + toNodeID;
    url += "&ToEmps=" + toEmps + "&1=2" + paras;
    return RunUrlReturnString(url);
}

/**
 * 获得可以退回的节点
 * @param {校验码} sid
 * @param {流程编号} flowNo
 * @param {工作实例ID} workid
 * @param {FID} fid
 */
function DB_GenerWillReturnNodes(flowNo, workid, fid = 0) {

    var url = ccbpmHostDevelopAPI + "?DoType=DB_GenerWillReturnNodes&Token=" + GetToken() + "&FK_Flow=" + flowNo;
    url += "&WorkID=" + workid + "&FID=" + fid;
    return RunUrlReturnString(url);
}

/**
 * 批处理：获得批处理的节点.
 */
function Batch_Init() {
    var url = ccbpmHostDevelopAPI + "?DoType=Batch_Init&Token=" + GetToken() + "&Domain=" + domain;
    return RunUrlReturnJSON(url);
}

function WorkCheckModel_Init(nodeID) {
    var url = ccbpmHostDevelopAPI + "?DoType=WorkCheckModel_Init&Token=" + GetToken() + "&NodeID=" + nodeID;
    return RunUrlReturnJSON(url);
}

function Node(nodeID) {
    var url = ccbpmHostDevelopAPI + "?DoType=En_Node&Token=" + GetToken() + "&NodeID=" + nodeID;
    return RunUrlReturnJSON(url);
}
function Flow(flowNo) {
    var url = ccbpmHostDevelopAPI + "?DoType=En_Flow&Token=" + GetToken() + "&No=" + flowNo;
    return RunUrlReturnJSON(url);
}
function Batch_InitDDL(nodeID) {
    var url = ccbpmHostDevelopAPI + "?DoType=Batch_InitDDL&Token=" + GetToken() + "&NodeID=" + nodeID;
    return RunUrlReturnJSON(url);
}
function WorkCheckModel_Send(nodeID, CheckNote, ToNode, ToEmps) {
    var url = ccbpmHostDevelopAPI + "?DoType=WorkCheckModel_Send&Token=" + GetToken() + "&NodeID=" + nodeID + "&ToNode=" + ToNode + "&ToEmps=" + ToEmps + "&CheckNote=" + CheckNote;
    return RunUrlReturnString(url);
}

function Batch_Delete(WorkIDs) {
    var url = ccbpmHostDevelopAPI + "?DoType=Batch_Delete&Token=" + GetToken() + "&WorkIDs=" + WorkIDs;
    return RunUrlReturnString(url);
}


/**
 * 退回
 * @param {校验码} sid
 * @param {工作实例ID} workid
 * @param {退回到节点ID} returnToNodeID
 * @param {退回给人员} returnToEmp
 * @param {退回意见} msg
 * @param {是否原路返回?} isBackToThisNode
 */
function Node_ReturnWork(workid, returnToNodeID, returnToEmp, msg, isBackToThisNode = false) {
    var url = ccbpmHostDevelopAPI + "?DoType=Node_ReturnWork&Token=" + GetToken();
    url += "&WorkID=" + workid;
    url += "&ReturnToNodeID=" + returnToNodeID;
    url += "&ReturnToEmp=" + returnToEmp;
    url += "&Msg=" + msg;

    if (isBackToThisNode == true)
        url += "&IsBackToThisNode=1";
    else
        url += "&IsBackToThisNode=0";
    return RunUrlReturnString(url);
}

/**
 * 设置流程标题
 * @param  workID 工作ID.
 * @param  title 流程标题
 */
function Flow_SetTitle(workID, title) {
    var url = ccbpmHostDevelopAPI + "?DoType=Flow_SetTitle&Token=" + GetToken();
    url += "&WorkID=" + workID;
    url += "&Title=" + title;
    return RunUrlReturnString(url);
}

/**
 * 催办
 * @param {要执行的实例,多个实例用逗号分开比如：1001,1002,1003} workidStrs
 */
function Flow_DoPress(workidStrs, msg) {
    var url = ccbpmHostDevelopAPI + "?DoType=Flow_DoPress&Token=" + GetToken();
    url += "&WorkIDs=" + workidStrs;
    url += "&Msg=" + msg;
    return RunUrlReturnString(url);
}

/**
 * 撤销发送,如果产生失败就会返回 err@+失败信息.
 * @param {要执行的实例,多个实例用逗号分开比如：1001,1002,1003} workidStrs
 */
function Flow_DoUnSend(workidStrs) {

    var url = ccbpmHostDevelopAPI + "?DoType=Flow_DoUnSend&Token=" + GetToken();
    url += "&WorkIDs=" + workidStrs;
    return RunUrlReturnString(url);
}

/**
 * 删除流程
 * @param {要删除的实例,多个实例用逗号分开比如：1001,1002,1003} workidStrs
 * @param {是否删除子流程} isDeleteSubFlows
 */
function Flow_BatchDeleteByReal(workidStrs, isDeleteSubFlows = true) {

    var url = ccbpmHostDevelopAPI + "?DoType=Flow_BatchDeleteByReal&Token=" + GetToken();
    url += "&WorkIDs=" + workidStrs;

    if (isDeleteSubFlows == false)
        url += "&IsDeleteSubFlows=0";
    else
        url += "&IsDeleteSubFlows=1";
    return RunUrlReturnString(url);
}
/**
 * 恢复删除
 * @param {any} workidStrs
 */
function Flow_BatchDeleteByFlagAndUnDone(workidStrs) {

    var url = ccbpmHostDevelopAPI + "?DoType=Flow_BatchDeleteByFlagAndUnDone&Token=" + GetToken();
    url += "&WorkIDs=" + workidStrs;
    return RunUrlReturnString(url);
}

/**
 * 设置流程结束
 * @param {要执行的实例,多个实例用逗号分开比如：1001,1002,1003} workidStrs
 */
function Flow_DoFlowOver(workidStrs) {

    var url = ccbpmHostDevelopAPI + "?DoType=Flow_DoFlowOver&Token=" + GetToken();
    url += "&WorkIDs=" + workidStrs;
    return RunUrlReturnString(url);
}

/**
 * 批量设置抄送查看完毕
 * @param {any} workidStrs
 */

function CC_BatchCheckOver(workidStrs) {

    var url = ccbpmHostDevelopAPI + "?DoType=CC_BatchCheckOver&Token=" + GetToken();
    url += "&WorkIDs=" + workidStrs;

    return RunUrlReturnString(url);
}

/**
 * 删除草稿
 * @param {要执行的实例,多个实例用逗号分开比如：1001,1002,1003} workidStrs
 */
function Flow_DeleteDraft(workidStrs) {

    var url = ccbpmHostDevelopAPI + "?DoType=Flow_DeleteDraft&Token=" + GetToken();
    url += "&WorkIDs=" + workidStrs;
    return RunUrlReturnString(url);
}

/**
 * 移交
 * @param {工作ID} workID
 * @param {要移交到的人} toEmpNo
 * @param {移交消息} msg
 */
function Node_Shift(workID, toEmpNo, msg) {

    var url = ccbpmHostDevelopAPI + "?DoType=Node_Shift&Token=" + GetToken();
    url += "&WorkID=" + workID;
    url += "&ToEmpNo=" + toEmpNo;
    url += "&Msg=" + msg;
    return RunUrlReturnString(url);
}

/**
 * 增加人员
 * @param {工作ID} workID
 * @param {增加的人员ID} empID
 */
function Node_AddTodolist(workID, empID) {

    var url = ccbpmHostDevelopAPI + "?DoType=Node_AddTodolist&Token=" + GetToken();
    url += "&WorkID=" + workID;
    url += "&EmpNo=" + empID;
    return RunUrlReturnString(url);
}

/**
 * 流程实例的注册信息
 * @param {实例ID} workID
 * @returns 流程实例的注册信息
 */
function Flow_GenerWorkFlow(workID) {

    var url = ccbpmHostDevelopAPI + "?DoType=Flow_GenerWorkFlow&Token=" + GetToken();
    url += "&WorkID=" + workID;
    return RunUrlReturnString(url);
}



