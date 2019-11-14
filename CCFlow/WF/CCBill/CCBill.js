//自动的加载js Gener.js config.js queryString.js

/** %%%%%%%%%%%%%%%%%%%%%%%%%%%%%  前台操作的方法： %%%%%%%%%%%%%%%%%%%%%%%%%%%%%
 * 
 * 
*/


/* 获得可以操作的单据列表. 返回: No,Name,FrmType,TreeNo,TreeName 的 json. FrmType=是单据，还是实体.
 * 1. 该方法可以用于生成当前用户可以发起的单据列表.
 * 2. 我们提供了一个通用的百搭款的风格的页面. /WF/CCBill/Start.htm
 * */

function GenerFrmsOfCanOption() {

}

/**
 * 获得可以操作的单据列表
 * @param {执行的目录树下的单据} specTreeNo
 */
function GenerFrmsOfCanOption(specTreeNo) {

}

//获得可以填写的表单集合. 返回 No,Name,FrmType，IsView, IsNew, IsSubmit, IsUpdate IsDelete 的json.
// FrmType = 是单据，还是实体.
function GenerMyFrms() {

}

//获得可以填写的表单集合.  返回： No,Name,Is
//指定目录树下的.
function GenerMyFrmsSpecTreeNo(specTreeNo) {

}


/**
 * 获得表单的Url.
 * @param {表单ID} frmID
 * @param {主键} pkval
 */
function GenerFrmOpenUrl(frmID, pkval) {
    return "../WF/CCBill/MyBill.htm?FrmID=" + frmID + "&OID=" + pkval;
}

/**
 * 创建表单实例. 说明:指定表单的ID, specID,与参数创建表单实例.
 * @param {表单ID} frmID
 * @param {指定的int类型的OID，作为主键} specID
 * @param {指定的Title，可以为空} specTitle
 * @param {主表字段的参数，一个key val 的json格式的数据.} paras
 */
function NewBillAsSpecOID(frmID, specID, specTitle, paras) {

}

/**
 * 创建表单实例. 说明:指定表单的ID, specID,与参数创建表单实例.
 * @param {表单ID} frmID
 * @param {指定的int类型的OID，作为主键} specBillNo
 * @param {指定的Title，可以为空} specTitle
 * @param {主表字段的参数，一个key val 的json格式的数据.} paras
 */
function NewBillAsSpecBillNo(frmID, specBillNo, specTitle, paras) {

}


/**
 *  创建表单实例： 返回一个 frmJson。 
 * @param {表单ID} frmID
 * @param {标题/名称:可以为空} specTitle
 * @param {主表的参数 Key Val 可为空} paras
 */
function NewBill(frmID, specTitle, paras) {

}


/**
 * 删除表单实例. 说明:指定表单的ID,OID删除实例.
 * 
 * @param {any} frmID
 * @param {any} oid
 * 如果返回 err@xxxx 则表是失败.
 */
function DeleteBillEntity(frmID, oid) {

}


/** 后台管理所调用的方法. ******************************************************************************************************
 *  1. 用户在后台调用权限控制.
 *  2. 用于集成自己的权限管理架构.
 *  3. 如果返回 err@xxxx 则表是删除失败.
 * **/

/**
 * 在根目录下创建子2级目录. 子目录的名字:,  返回子目录创建的编号. 
 * @param {目录名字} dirName
 */
function Admin_CreateTreeDir(dirName) {

}

/**
 * 删除表单树
 * @param {目录编号} treeNo
 */
function Admin_DeleteTreeDir(treeNo) {

}

/**
 * 创建表单-单据
 * @param {创建在那个表单树的叶子下} treeNo
 * @param {表单ID} frmID
 * @param {表单名称} frmName
 * @param {存储表,如果为Null则与frmID相同} pTable
 * 如果返回 err@xxxx 则表是失败.
 */
function Admin_CreateFormCCBill(treeNo, frmID, frmName, pTable) {

}

/**
 * 创建表单-实体
 * @param {创建在那个表单树的叶子下} treeNo
 * @param {表单ID} frmID
 * @param {表单名称} frmName
 * @param {存储表,如果为Null则与frmID相同} pTable
 * 如果返回 err@xxxx 则表是失败.
 */
function Admin_CreateFormCCDict(treeNo, frmID, frmName, pTable) {

}

/**
 * 删除表单、单据
 * @param {表单ID，不管是Dict还是Bill} frmID
 */
function Admin_DropFrom(frmID) {

}

/**
 * 按照角色绑定权限
 * @param {表单ID} frmID
 * @param {岗位编号: 001,002,003} staNos
 * @param {是否可以查看: 0-1} isView
 * @param {是否可以新建: 0-1} isNew
 * @param {是否可以提交: 0-1} isSubmit
 * @param {是否可以更新: 0-1} isUpdate
 * @param {是否可以删除: 0-1} isDelete
 */
function Admin_Power_AddToStation(frmID, staNos, isView, isNew, isSubmit, isUpdate isDelete) {

}

/**
 * 按照人员绑定权限
 * @param {表单ID} frmID
 * @param {岗位编号: 001,002,003} staNos
 * @param {是否可以查看: 0-1} isView
 * @param {是否可以新建: 0-1} isNew
 * @param {是否可以提交: 0-1} isSubmit
 * @param {是否可以更新: 0-1} isUpdate
 * @param {是否可以删除: 0-1} isDelete
 */
function Admin_Power_AddToUser(frmID, userNos, isView, isNew, isSubmit, isUpdate isDelete) {

}








