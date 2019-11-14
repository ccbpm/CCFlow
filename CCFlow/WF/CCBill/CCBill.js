//自动的加载js Gener.js config.js queryString.js

/** %%%%%%%%%%%%%%%%%%%%%%%%%%%%%  前台操作的方法： %%%%%%%%%%%%%%%%%%%%%%%%%%%%% */

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

/**
 * 获得操作的权限.
 * @param {any} frmID
 * 返回 No,Name,FrmType，IsView, IsNew, IsSubmit, IsUpdate IsDelete 的json.
 */
function GenerFrmPower(frmID) {

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
function DeleteEntity(frmID, oid) {

}







