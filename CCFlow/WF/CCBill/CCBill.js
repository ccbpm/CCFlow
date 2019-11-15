//�Զ��ļ���js Gener.js config.js queryString.js

/** %%%%%%%%%%%%%%%%%%%%%%%%%%%%%  ǰ̨�����ķ����� %%%%%%%%%%%%%%%%%%%%%%%%%%%%% */

/* ��ÿ��Բ����ĵ����б�. ����: No,Name,FrmType,TreeNo,TreeName �� json. FrmType=�ǵ��ݣ�����ʵ��.
 * 1. �÷��������������ɵ�ǰ�û����Է���ĵ����б�.
 * 2. �����ṩ��һ��ͨ�õİٴ��ķ���ҳ��. /WF/CCBill/Start.htm
 * */
function CCFrom_GenerFrmListOfCanOption() {
    var handler = new HttpHandler("BP.Frm.WF_CCBill_API");
    var data = handler.DoMethodReturnJSON("CCFrom_GenerFrmListOfCanOption");
    return data;
}

/**
 * ��ÿ��Բ����ĵ����б�
 * @param {ִ�е�Ŀ¼���µĵ���} specTreeNo
 */
function CCFrom_GenerFrmListBySpecTreeNo(specTreeNo) {
    var handler = new HttpHandler("BP.Frm.WF_CCBill_API");
    handler.AddPara("TreeNo", specTreeNo);
    var data = handler.DoMethodReturnJSON("CCFrom_GenerFrmListBySpecTreeNo");
    return data;
}

/**
 * ���һ�����Ĳ���Ȩ��.
 * @param {any} frmID
 * ���� IsView, IsNew, IsSubmit, IsUpdate IsDelete ��json.
 */
function CCFrom_FrmPower(frmID) {
    var handler = new HttpHandler("BP.Frm.WF_CCBill_API");
    handler.AddPara("FrmID", frmID);
    var data = handler.DoMethodReturnJSON("CCFrom_FrmPower");
    return data;
}


/**
 * ��ñ���Url.
 * @param {��ID} frmID
 * @param {����} pkval
 */
function CCFrom_FrmOptionUrl(frmID, pkval) {
    return "../WF/CCBill/MyBill.htm?FrmID=" + frmID + "&OID=" + pkval;
}

/**
 * ��ñ��鿴��Url.
 * @param {��ID} frmID
 * @param {����} oid
 */
function CCFrom_FrmViewUrl(frmID, oid) {
    return "../WF/CCForm/Frm.htm?FrmID=" + frmID + "&OID=" + oid;
}

/**
 * ��ñ��鿴��Url.
 * @param {��ID} frmID
 * @param {���ݱ��} billNo
 */
function CCFrom_FrmViewUrlByBillNo(frmID, billNo) {
    //var frm = new Entity(frmID); ??������Ҫ���� BillNo�����ֵ.
    return "../WF/CCForm/Frm.htm?FrmID=" + frmID + "&BillNo=" + pkval;
}

 

/**
 * ������ʵ��. ˵��:ָ������ID, specID,�����������ʵ��.
 * @param {��ID} frmID
 * @param {ָ����int���͵�OID����Ϊ����} specOID
 * @param {ָ����Title������Ϊ��} specTitle
 * @param {�����ֶεĲ�����һ��key val ��json��ʽ������.} paras
 */
function CCFrom_NewFrmEntityAsSpecOID(frmID, specOID, specTitle, paras) {
    var handler = new HttpHandler("BP.Frm.WF_CCBill_API");
    handler.AddPara("FrmID", frmID);
    handler.AddPara("OID", specOID);
    handler.AddPara("Title", specTitle);
    handler.AddJson(paras); //�Ѳ�������.

    var data = handler.DoMethodReturnJSON("CCFrom_NewFrmEntityAsSpecOID");
    if (data.indexOf('err@') == -1) {
        throw Exception(data); // ??�ǲ�������﷨��
    }
    return data;
}

/**
 * ������ʵ��. ˵��:ָ������ID, specID,�����������ʵ��.
 * @param {��ID} frmID
 * @param {ָ����int���͵�OID����Ϊ����} specBillNo
 * @param {ָ����Title������Ϊ��} specTitle
 * @param {�����ֶεĲ�����һ��key val ��json��ʽ������.} paras
 */
function CCFrom_NewFrmEntityAsSpecBillNo(frmID, specBillNo, specTitle, paras) {
    var handler = new HttpHandler("BP.Frm.WF_CCBill_API");
    handler.AddPara("FrmID", frmID);
    handler.AddPara("BillNo", specBillNo);
    handler.AddPara("Title", specTitle);
    handler.AddJson(paras); //�Ѳ�������.

    var data = handler.DoMethodReturnJSON("CCFrom_NewFrmEntityAsSpecBillNo");
    if (data.indexOf('err@') == -1) {
        throw Exception(data); // ??�ǲ�������﷨��
    }
    return data;
}


/**
 *  ������ʵ���� ����һ�� frmJson�� 
 * @param {��ID} frmID
 * @param {����/����:����Ϊ��} specTitle
 * @param {����Ĳ��� Key Val ��Ϊ��} paras
 */
function CCFrom_NewFrmEntity(frmID, specTitle, paras) {

    var handler = new HttpHandler("BP.Frm.WF_CCBill_API");
    handler.AddPara("FrmID", frmID);
    handler.AddPara("OID", oid);
    var data = handler.DoMethodReturnJSON("CCFrom_NewFrmEntity");
    if (data.indexOf('err@') == -1) {
        throw Exception(data); // ??�ǲ�������﷨��Ҫ����Ƿ���Դ�����Ȩ��.
    }
    return data;
}


/**
 * ɾ����ʵ��. ˵��:ָ������ID,OIDɾ��ʵ��.
 * 
 * @param {any} frmID
 * @param {any} oid
 * ������� err@xxxx �����ʧ��.
 */
function CCFrom_DeleteFrmEntity(frmID, oid) {

    var handler = new HttpHandler("BP.Frm.WF_CCBill_API");
    handler.AddPara("FrmID", frmID);
    handler.AddPara("OID", oid);
    var data = handler.DoMethodReturnJSON("CCFrom_DeleteFrmEntity");
    if (data.indexOf('err@') == -1) {
        throw Exception(data); // ??�ǲ�������﷨��Ҫ���ɾ��Ȩ��.
    }
    return data;
}







