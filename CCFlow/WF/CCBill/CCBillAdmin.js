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
