/** ��̨���������õķ���. ******************************************************************************************************
 *  1. �û��ں�̨����Ȩ�޿���.
 *  2. ���ڼ����Լ���Ȩ�޹���ܹ�.
 *  3. ������� err@xxxx �����ɾ��ʧ��.
 * **/

/**
 * �ڸ�Ŀ¼�´�����2��Ŀ¼. ��Ŀ¼������:,  ������Ŀ¼�����ı��. 
 * @param {Ŀ¼����} dirName
 */
function Admin_TreeDir_Create(dirName) {
    var en = new Entity("BP.Sys.FrmTree", "100");
    return en.DoMethodReturnString("CreateSubNode");
}

/**
 * ɾ������
 * @param {Ŀ¼���} treeNo
 */
function Admin_TreeDir_Delete(treeNo) {
    var en = new Entity("BP.Sys.FrmTree", treeNo);
    en.Delete();
}

/**
 * ���ƶ�Ŀ¼
 * @param {Ŀ¼���} treeNo
 */
function Admin_TreeDir_Up(treeNo) {
    var en = new Entity("BP.Sys.FrmTree", treeNo);
    en.DoMethodReturnString("DoUp");
}
/**
 * ���ƶ�Ŀ¼
 * @param {Ŀ¼���} treeNo
 */
function Admin_TreeDir_Down(treeNo) {
    var en = new Entity("BP.Sys.FrmTree", treeNo);
    en.DoMethodReturnString("DoDown");
}

/**
 * ������-����
 * @param {�������Ǹ�������Ҷ����} treeNo
 * @param {��ID} frmID
 * @param {������} frmName
 * @param {�洢��,���ΪNull����frmID��ͬ} pTable
 * ������� err@xxxx �����ʧ��.
 */
function Admin_Form_CreateBill(treeNo, frmID, frmName, pTable) {

}

/**
 * ������-ʵ��
 * @param {�������Ǹ�������Ҷ����} treeNo
 * @param {��ID} frmID
 * @param {������} frmName
 * @param {�洢��,���ΪNull����frmID��ͬ} pTable
 * ������� err@xxxx �����ʧ��.
 */
function Admin_Form_CreateDict(treeNo, frmID, frmName, pTable) {

}

/**
 * ɾ����������
 * @param {��ID��������Dict����Bill} frmID
 */
function Admin_From_Drop(frmID) {
    var en = new Entity("BP.Sys.MapData", frmID);
    en.Delete();
}
/**
 * ���ƶ�,��ͬһ��Ŀ¼��
 * @param {��ID} frmID
 */
function Admin_From_Up(frmID) {
    var en = new Entity("BP.Sys.MapData", frmID);
    en.DoMethodReturnString("DoUp");
}

/**
 * ���ƶ�,��ͬһ��Ŀ¼��
 * @param {��ID} frmID
 */
function Admin_From_Up(frmID) {
    var en = new Entity("BP.Sys.MapData", frmID);
    en.DoMethodReturnString("DoOrderDown");
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
