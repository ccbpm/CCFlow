/** 后台管理所调用的方法. ******************************************************************************************************
 *  1. 用户在后台调用权限控制.
 *  2. 用于集成自己的权限管理架构.
 *  3. 如果返回 err@xxxx 则表是删除失败.
 * **/

/**
 * 获得创建的所有的 单据 
 */
function Admin_GenerAllBills() {
    var handler = new HttpHandler("BP.CCBill.WF_CCBill_API");
    var data = handler.DoMethodReturnJSON("CCBillAdmin_Admin_GenerAllBills");
    return data;
}

/**
 * 在根目录下创建子2级目录. 子目录的名字:,  返回子目录创建的编号. 
 * @param {目录名字} dirName
 */
function Admin_TreeDir_Create(dirName) {
    var en = new Entity("BP.WF.Template.SysFormTree", "100");
    return en.DoMethodReturnString("DoCreateSubNodeIt", dirName);
}

/**
 * 删除表单树
 * @param {目录编号} treeNo
 */
function Admin_TreeDir_Delete(treeNo) {
    var en = new Entity("BP.WF.Template.SysFormTree", treeNo);
    en.Delete();
}

/**
 * 上移动目录
 * @param {目录编号} treeNo
 */
function Admin_TreeDir_Up(treeNo) {
    var en = new Entity("BP.WF.Template.SysFormTree", treeNo);
    en.DoMethodReturnString("DoUp");
}
/**
 * 下移动目录
 * @param {目录编号} treeNo
 */
function Admin_TreeDir_Down(treeNo) {
    var en = new Entity("BP.WF.Template.SysFormTree", treeNo);
    en.DoMethodReturnString("DoDown");
}

/**
 * 创建表单-单据模版
 * @param {创建在那个表单树的叶子下,可以为null，默认创建根目录下} treeNo
 * @param {表单ID} frmID
 * @param {表单名称} frmName
 * @param {单据类型,0=傻瓜表单,1=自由表单,3=URL表单,4=WordFrm,5=ExcelFrm,6=VSTOForExcel,7=Entity,8=Develop} frmTpye
 * @param {存储表,如果为Null则与frmID相同} pTable
 * 如果返回 err@xxxx 则表是失败.
 */
function Admin_Form_CreateBill(treeNo, frmID, frmName, frmType, pTable) {
    return Admin_Form_Create(treeNo, frmID, frmName, frmType, pTable, 1);
}


/**
 * 创建表单-实体
 * @param {创建在那个表单树的叶子下,可以为null，默认创建根目录下} treeNo
 * @param {表单ID} frmID
 * @param {表单名称} frmName
 * @param {单据类型,0=傻瓜表单,1=自由表单,3=URL表单,4=WordFrm,5=ExcelFrm,6=VSTOForExcel,7=Entity,8=Develop} frmTpye
 * @param {存储表,如果为Null则与frmID相同} pTable
 * 如果返回 err@xxxx 则表是失败.
 */
function Admin_Form_CreateDict(treeNo, frmID, frmName, frmType, pTable) {
    return Admin_Form_Create(treeNo, frmID, frmName, frmType, pTable, 2);
}

function Admin_Form_Create(treeNo, frmID, frmName, frmType, pTable, entityType) {
    if (treeNo == null || treeNo == undefined)
        treeNo = "100";

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCFormDesigner");
    handler.AddPara("EntityType", entityType); //实体.

    handler.AddPara("TB_No", frmID);  //表单ID
    handler.AddPara("TB_Name", frmName); //表单名称.

    handler.AddPara("FK_FrmSort", treeNo); //树结构.
    handler.AddPara("DDL_FrmType", frmType); //表单类型.
    handler.AddPara("TB_PTable", pTable); //ptable.
    handler.AddPara("DDL_PTableModel", 0);  //模式,忘记了这个参数.
    handler.AddPara("DDL_DBSrc", "local");  //数据源.

    var data = handler.DoMethodReturnString("NewFrmGuide_Create");
    if (data.indexOf("err@") == 0) {
        alert(data);
        return data;
    }
    alert("创建成功.");
    return data;
}


/**
 * 获得设计表单的URL.
 * @param {表单ID} frmID
 */
function Admin_Form_GenerDesignerUrl(frmID) {
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCFormDesigner");
    handler.AddPara("FK_MapData", frmID);
    var data = handler.DoMethodReturnString("GoToFrmDesigner_Init");
    if (data.indexOf("err@") == 0) {
        alert(data);
        return data;
    }

    data = data.replace("url@..", "");
    data = basePath + "/WF/Admin" + data;

    return data;
}

/**
 * 表单属性
 * @param {表单ID} frmID
 */
function Admin_FromTemplateAttr(frmID) {

    var en = new Entity("BP.CCBill.FrmBill", frmID);

    //流程单据.
    if (en.EntityType == 0)
        url = '../Comm/En.htm?EnName=BP.WF.Template.Frm.MapFrmFree&PKVal=' + frmID;

    if (en.EntityType == 1)
        url = '../Comm/En.htm?EnName=BP.CCBill.FrmBill&PKVal=' + frmID;

    if (en.EntityType == 2 || en.EntityType == 3)
        url = '../Comm/En.htm?EnName=BP.CCBill.FrmDict&PKVal=' + frmID;
    return url;
}

/**
 * 修改表单模版类型
 * @param {any} frmID 表单ID 
 * @param {any} frmType 表单类型 
 * 0=傻瓜表单,1=自由表单,3=URL表单,4=WordFrm,5=ExcelFrm,6=VSTOForExcel,7=Entity,8=Develop
 */
function Admin_From_ChangeFrmType(frmID, frmType)
{
    var en = new Entity("BP.Sys.MapData", frmID);
    en.FrmType = frmType; //表单类型.
    en.Update();
}

/**
 * 删除表单、单据
 * @param {表单ID，不管是Dict还是Bill} frmID
 */
function Admin_From_Drop(frmID) {
    var en = new Entity("BP.Sys.MapData", frmID);
    en.Delete();
}
/**
 * 表单移动,在同一个目录下
 * @param {表单ID} frmID
 */
function Admin_From_Up(frmID) {
    var en = new Entity("BP.Sys.MapData", frmID);
    en.DoMethodReturnString("DoUp");
}

/**
 * 表单移动,在同一个目录下
 * @param {表单ID} frmID
 */
function Admin_From_Down(frmID) {
    var en = new Entity("BP.Sys.MapData", frmID);
    en.DoMethodReturnString("DoOrderDown");
}

