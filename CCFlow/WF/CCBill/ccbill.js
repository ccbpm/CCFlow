//�Զ��ļ���js Gener.js config.js queryString.js

/** %%%%%%%%%%%%%%%%%%%%%%%%%%%%%  ǰ̨�����ķ����� %%%%%%%%%%%%%%%%%%%%%%%%%%%%%
 * 
 * 
*/


/* ��ÿ�����д�ı�����. ����: No,Name,FrmType,TreeNo,TreeName �� json. FrmType=�ǵ��ݣ�����ʵ��.
 * 1. �÷��������������ɵ�ǰ�û����Է���ĵ����б�.
 * 2. �����ṩ��һ��ͨ�õİٴ��ķ���ҳ��.
 * */

function GenerFrmsOfCanStart() {

}

//��ÿ�����д�ı�����.  ���� No,Name,FrmType ��json. FrmType=�ǵ��ݣ�����ʵ��.
//ָ��Ŀ¼���µ�.
function GenerFrmsOfCanStart(specTreeNo) {

}


//��ÿ�����д�ı�����. ���� No,Name,FrmType��IsView, IsNew, IsSubmit, IsUpdate IsDelete ��json.
// FrmType = �ǵ��ݣ�����ʵ��.
function GenerMyFrms() {

}

//��ÿ�����д�ı�����.  ���أ� No,Name,Is
//ָ��Ŀ¼���µ�.
function GenerMyFrmsSpecTreeNo(specTreeNo) {

}



//��ñ���Url.
//��ID, ����.
function GenerFrmOpenUrl(frmID, pkval) {
    return "../WF/CCBill/MyBill.htm?FrmID=" + frmID + "&OID=" + pkval;
}

//������ʵ��. ˵��:ָ������ID, specID,�����������ʵ��.
// ��ID,
// ָ����OID,
// �����ֶ�.KeyValue �� json ��ʽ��.  ����Ϊ��.
// return frmJSON 
function NewBillSpecOID(frmID, specID, paras) {

}

//������ʵ��. ˵��:ָ������ID,OID,�����������ʵ��.
// ��ID, 
// �����ֶ�.KeyValue��json��ʽ��.
// return frmJSON 
function NewBill(frmID, paras) {

}


//ɾ����ʵ��. ˵��:ָ������ID,OIDɾ��ʵ��.
// ��ID, 

/**
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
 * �ڸ�Ŀ¼�´�����2��Ŀ¼. ��Ŀ¼������:, ������Ŀ¼�����ı��.
 * @param {any} dirName
 */
function Admin_CreateTreeDir( dirName)
{

}

/**
 * ɾ������
 * @param {any} treeNo
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








