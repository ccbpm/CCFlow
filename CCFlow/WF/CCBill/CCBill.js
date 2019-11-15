//自动的加载js Gener.js config.js queryString.js

/** %%%%%%%%%%%%%%%%%%%%%%%%%%%%%  前台操作的方法： %%%%%%%%%%%%%%%%%%%%%%%%%%%%% */

/* 获得可以操作的单据列表. 返回: No,Name,FrmType,TreeNo,TreeName 的 json. FrmType=是单据，还是实体.
 * 1. 该方法可以用于生成当前用户可以发起的单据列表.
 * 2. 我们提供了一个通用的百搭款的风格的页面. /WF/CCBill/Start.htm
 * */
function CCFrom_GenerFrmListOfCanOption() {
    var handler = new HttpHandler("BP.Frm.WF_CCBill_API");
    var data = handler.DoMethodReturnJSON("CCFrom_GenerFrmListOfCanOption");
    return data;
}

/**
 * 获得可以操作的单据列表
 * @param {执行的目录树下的单据} specTreeNo
 */
function CCFrom_GenerFrmListBySpecTreeNo(specTreeNo) {
    var handler = new HttpHandler("BP.Frm.WF_CCBill_API");
    handler.AddPara("TreeNo", specTreeNo);
    var data = handler.DoMethodReturnJSON("CCFrom_GenerFrmListBySpecTreeNo");
    return data;
}

/**
 * 获得一个表单的操作权限.
 * @param {any} frmID
 * 返回 IsView, IsNew, IsSubmit, IsUpdate IsDelete 的json.
 */
function CCFrom_FrmPower(frmID) {
    var handler = new HttpHandler("BP.Frm.WF_CCBill_API");
    handler.AddPara("FrmID", frmID);
    var data = handler.DoMethodReturnJSON("CCFrom_FrmPower");
    return data;
}


/**
 * 获得表单的Url.
 * @param {表单ID} frmID
 * @param {主键} pkval
 */
function CCFrom_FrmOptionUrl(frmID, pkval) {
    return "../WF/CCBill/MyBill.htm?FrmID=" + frmID + "&OID=" + pkval;
}

/**
 * 获得表单查看的Url.
 * @param {表单ID} frmID
 * @param {主键} oid
 */
function CCFrom_FrmViewUrl(frmID, oid) {
    return "../WF/CCForm/Frm.htm?FrmID=" + frmID + "&OID=" + oid;
}

/**
 * 获得表单查看的Url.
 * @param {表单ID} frmID
 * @param {单据编号} billNo
 */
function CCFrom_FrmViewUrlByBillNo(frmID, billNo) {
    //var frm = new Entity(frmID); ??这里需要解析 BillNo传入的值.
    return "../WF/CCForm/Frm.htm?FrmID=" + frmID + "&BillNo=" + pkval;
}

 

/**
 * 创建表单实例. 说明:指定表单的ID, specID,与参数创建表单实例.
 * @param {表单ID} frmID
 * @param {指定的int类型的OID，作为主键} specOID
 * @param {指定的Title，可以为空} specTitle
 * @param {主表字段的参数，一个key val 的json格式的数据.} paras
 */
function CCFrom_NewFrmEntityAsSpecOID(frmID, specOID, specTitle, paras) {
    var handler = new HttpHandler("BP.Frm.WF_CCBill_API");
    handler.AddPara("FrmID", frmID);
    handler.AddPara("OID", specOID);
    handler.AddPara("Title", specTitle);
    handler.AddJson(paras); //把参数加入.

    var data = handler.DoMethodReturnJSON("CCFrom_NewFrmEntityAsSpecOID");
    if (data.indexOf('err@') == -1) {
        throw Exception(data); // ??是不是这个语法？
    }
    return data;
}

/**
 * 创建表单实例. 说明:指定表单的ID, specID,与参数创建表单实例.
 * @param {表单ID} frmID
 * @param {指定的int类型的OID，作为主键} specBillNo
 * @param {指定的Title，可以为空} specTitle
 * @param {主表字段的参数，一个key val 的json格式的数据.} paras
 */
function CCFrom_NewFrmEntityAsSpecBillNo(frmID, specBillNo, specTitle, paras) {
    var handler = new HttpHandler("BP.Frm.WF_CCBill_API");
    handler.AddPara("FrmID", frmID);
    handler.AddPara("BillNo", specBillNo);
    handler.AddPara("Title", specTitle);
    handler.AddJson(paras); //把参数加入.

    var data = handler.DoMethodReturnJSON("CCFrom_NewFrmEntityAsSpecBillNo");
    if (data.indexOf('err@') == -1) {
        throw Exception(data); // ??是不是这个语法？
    }
    return data;
}


/**
 *  创建表单实例： 返回一个 frmJson。 
 * @param {表单ID} frmID
 * @param {标题/名称:可以为空} specTitle
 * @param {主表的参数 Key Val 可为空} paras
 */
function CCFrom_NewFrmEntity(frmID, specTitle, paras) {

    var handler = new HttpHandler("BP.Frm.WF_CCBill_API");
    handler.AddPara("FrmID", frmID);
    handler.AddPara("OID", oid);
    var data = handler.DoMethodReturnJSON("CCFrom_NewFrmEntity");
    if (data.indexOf('err@') == -1) {
        throw Exception(data); // ??是不是这个语法？要检查是否可以创建的权限.
    }
    return data;
}


/**
 * 删除表单实例. 说明:指定表单的ID,OID删除实例.
 * 
 * @param {any} frmID
 * @param {any} oid
 * 如果返回 err@xxxx 则表是失败.
 */
function CCFrom_DeleteFrmEntity(frmID, oid) {

    var handler = new HttpHandler("BP.Frm.WF_CCBill_API");
    handler.AddPara("FrmID", frmID);
    handler.AddPara("OID", oid);
    var data = handler.DoMethodReturnJSON("CCFrom_DeleteFrmEntity");
    if (data.indexOf('err@') == -1) {
        throw Exception(data); // ??是不是这个语法？要检查删除权限.
    }
    return data;
}







