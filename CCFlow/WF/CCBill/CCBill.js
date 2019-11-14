//�Զ��ļ���js Gener.js config.js queryString.js

/** %%%%%%%%%%%%%%%%%%%%%%%%%%%%%  ǰ̨�����ķ����� %%%%%%%%%%%%%%%%%%%%%%%%%%%%%
 * 
 * 
*/


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

//��ÿ�����д�ı�����. ���� No,Name,FrmType��IsView, IsNew, IsSubmit, IsUpdate IsDelete ��json.
// FrmType = �ǵ��ݣ�����ʵ��.
function GenerMyFrms() {

}

//��ÿ�����д�ı�����.  ���أ� No,Name,Is
//ָ��Ŀ¼���µ�.
function GenerMyFrmsSpecTreeNo(specTreeNo) {

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
function DeleteBillEntity(frmID, oid) {

}


/** ��̨���������õķ���. ******************************************************************************************************
 *  1. �û��ں�̨����Ȩ�޿���.
 *  2. ���ڼ����Լ���Ȩ�޹���ܹ�.
 *  3. ������� err@xxxx �����ɾ��ʧ��.
 * **/

/**
 * �ڸ�Ŀ¼�´�����2��Ŀ¼. ��Ŀ¼������:,  ������Ŀ¼�����ı��. 
 * @param {Ŀ¼����} dirName
 */
function Admin_CreateTreeDir(dirName) {

}

/**
 * ɾ������
 * @param {Ŀ¼���} treeNo
 */
function Admin_DeleteTreeDir(treeNo) {

}

/**
 * ������-����
 * @param {�������Ǹ�������Ҷ����} treeNo
 * @param {��ID} frmID
 * @param {������} frmName
 * @param {�洢��,���ΪNull����frmID��ͬ} pTable
 * ������� err@xxxx �����ʧ��.
 */
function Admin_CreateFormCCBill(treeNo, frmID, frmName, pTable) {

}

/**
 * ������-ʵ��
 * @param {�������Ǹ�������Ҷ����} treeNo
 * @param {��ID} frmID
 * @param {������} frmName
 * @param {�洢��,���ΪNull����frmID��ͬ} pTable
 * ������� err@xxxx �����ʧ��.
 */
function Admin_CreateFormCCDict(treeNo, frmID, frmName, pTable) {

}

/**
 * ɾ����������
 * @param {��ID��������Dict����Bill} frmID
 */
function Admin_DropFrom(frmID) {

}

/**
 * ���ս�ɫ��Ȩ��
 * @param {��ID} frmID
 * @param {��λ���: 001,002,003} staNos
 * @param {�Ƿ���Բ鿴: 0-1} isView
 * @param {�Ƿ�����½�: 0-1} isNew
 * @param {�Ƿ�����ύ: 0-1} isSubmit
 * @param {�Ƿ���Ը���: 0-1} isUpdate
 * @param {�Ƿ����ɾ��: 0-1} isDelete
 */
function Admin_Power_AddToStation(frmID, staNos, isView, isNew, isSubmit, isUpdate isDelete) {

}

/**
 * ������Ա��Ȩ��
 * @param {��ID} frmID
 * @param {��λ���: 001,002,003} staNos
 * @param {�Ƿ���Բ鿴: 0-1} isView
 * @param {�Ƿ�����½�: 0-1} isNew
 * @param {�Ƿ�����ύ: 0-1} isSubmit
 * @param {�Ƿ���Ը���: 0-1} isUpdate
 * @param {�Ƿ����ɾ��: 0-1} isDelete
 */
function Admin_Power_AddToUser(frmID, userNos, isView, isNew, isSubmit, isUpdate isDelete) {

}








