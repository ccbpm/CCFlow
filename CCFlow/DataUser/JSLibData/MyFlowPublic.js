/*
1. ��JS�ļ���Ƕ�뵽��MyFlowGener.htm �Ĺ�����������. 
2. �����߿�����д���ļ�����ͨ�õ�Ӧ��,����ͨ�õĺ���.
*/

function DZ() {

    alert('sss');
    var url = 'pop.htm';
    window.open(url);
}

/*

1. beforeSave��beforeSend�� beforeReturn�� beforeDelete 
2 .MyFlowGener��MyFlowTree�Ĺ̶���������ֹɾ��
3.��Ҫд����ǰ������ǰ���˻�ǰ��ɾ��ǰ�¼�
4.����ֵΪ true��false

*/

//����ǰ�¼�
function beforeSave() {
    return true;
}

//����ǰ�¼�
function beforeSend() {
    return true;
}

//�˻�ǰ�¼�
function beforeReturn() {
    return true;
}

//ɾ��ǰ�¼�
function beforeDelete() {
    return true;
}
