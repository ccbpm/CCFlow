//�Զ��ļ���js Gener.js config.js queryString.js

/** %%%%%%%%%%%%%%%%%%%%%%%%%%%%%  ǰ̨�����ķ����� %%%%%%%%%%%%%%%%%%%%%%%%%%%%% */

/* ��ÿ��Բ����ĵ����б�. ����: No,Name,FrmType,TreeNo,TreeName �� json. FrmType=�ǵ��ݣ�����ʵ��.
 * 1. �÷��������������ɵ�ǰ�û����Է���ĵ����б�.
 * 2. �����ṩ��һ��ͨ�õİٴ��ķ���ҳ��. /WF/CCBill/Start.htm
 * */
function GenerFrmsOfCanOption() {

}

/**
 * ��ÿ��Բ����ĵ����б�
 * @param {ִ�е�Ŀ¼���µĵ���} specTreeNo
 */
function GenerFrmsOfCanOption(specTreeNo) {

}

/**
 * ��ò�����Ȩ��.
 * @param {any} frmID
 * ���� No,Name,FrmType��IsView, IsNew, IsSubmit, IsUpdate IsDelete ��json.
 */
function GenerFrmPower(frmID) {

}


/**
 * ��ñ���Url.
 * @param {��ID} frmID
 * @param {����} pkval
 */
function GenerFrmOpenUrl(frmID, pkval) {
    return "../WF/CCBill/MyBill.htm?FrmID=" + frmID + "&OID=" + pkval;
}

/**
 * ������ʵ��. ˵��:ָ������ID, specID,�����������ʵ��.
 * @param {��ID} frmID
 * @param {ָ����int���͵�OID����Ϊ����} specID
 * @param {ָ����Title������Ϊ��} specTitle
 * @param {�����ֶεĲ�����һ��key val ��json��ʽ������.} paras
 */
function NewBillAsSpecOID(frmID, specID, specTitle, paras) {

}

/**
 * ������ʵ��. ˵��:ָ������ID, specID,�����������ʵ��.
 * @param {��ID} frmID
 * @param {ָ����int���͵�OID����Ϊ����} specBillNo
 * @param {ָ����Title������Ϊ��} specTitle
 * @param {�����ֶεĲ�����һ��key val ��json��ʽ������.} paras
 */
function NewBillAsSpecBillNo(frmID, specBillNo, specTitle, paras) {

}


/**
 *  ������ʵ���� ����һ�� frmJson�� 
 * @param {��ID} frmID
 * @param {����/����:����Ϊ��} specTitle
 * @param {����Ĳ��� Key Val ��Ϊ��} paras
 */
function NewBill(frmID, specTitle, paras) {

}


/**
 * ɾ����ʵ��. ˵��:ָ������ID,OIDɾ��ʵ��.
 * 
 * @param {any} frmID
 * @param {any} oid
 * ������� err@xxxx �����ʧ��.
 */
function DeleteEntity(frmID, oid) {

}







