/*
1. ��JS�ļ���Ƕ�뵽��MyFlowGener.htm �Ĺ�����������. 
2. �����߿�����д���ļ�����ͨ�õ�Ӧ��,����ͨ�õĺ���.

*/
//Ϊ���������������ӳ�ʼ������js�ļ�
$(function () {
  
    

    
});
//�ⷢ����.
function WaiFa() {

    var workid = GetQueryString('WorkID');
    var flowNo = GetQueryString('FK_Flow');
    var nodeid = GetQueryString('NodeID');


    var url = "../App/WaiFa.htm?WorkID=" + workid + "&FK_Flow=" + flowNo + "&NodeID=" + nodeid;
    WinOpen(url);
    //window.open(url);
}

//ת�ڷ�����.
function NeiFa()
{
    var workid = GetQueryString('WorkID');
    var flowNo = GetQueryString('FK_Flow');
    var mypk = GetQueryString('MyPK');
    var pkVal = GetQueryString('PKVal');

    var url = "../App/NeiFa.htm?WorkID=" + workid + "&FK_Flow=" + flowNo + "&MyPK=" + pkVal;
    WinOpen(url);
}

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


//�����Ķ�ҳ�����ӹر�ǰ�¼�
function beforeCCClose() {
    return true;
}
//���������Ķ�ҳ�����ӹر�ǰ�¼�
function beforeCCClose() {
    $("#TB_Msg").val("����");
   
    if ($("#TB_Msg").val() == null || $("#TB_Msg").val() == "" || $("#TB_Msg").val().trim().length == 0) {
        alert("����д��������!");
        return;
    }

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt_OneWork");
    handler.AddUrlData();
    handler.AddFormData();
    var data = handler.DoMethodReturnString("FlowBBS_Save");
    if (data.indexOf('err@') == 0) {
        alert(data);
        return;
    }

    return true;
}
//���������Ķ�ҳ�����ӹر�ǰ�¼�����
//�����������Ĵ򿪹��ĵķ�ʽ
function openhtml() {
    var strUrl = "office.html";
    var dataSendToChild = $("#param").val();
    var ntkoed = ntkoBrowser.ExtensionInstalled();
    if (ntkoed) {
        ntkoBrowser.openWindow(strUrl, "", "", "", "", "", dataSendToChild);
    } else {
        //û�а�װ��������ؼ��Ļ���ʾ���ذ�װ
        var iTop = ntkoBrowser.NtkoiTop(); //��ô��ڵĴ�ֱλ��;
        var iLeft = ntkoBrowser.NtkoiLeft(); //��ô��ڵ�ˮƽλ��;
        window.open("downloadExe.html", "", "height=200px,width=500px,top=" + iTop + "px,left=" + iLeft +
            "px,titlebar=no,toolbar=no,menubar=no,scrollbars=auto,resizeable=no,location=no,status=no");
    }
}

//�ڸ�ҳ�涨��Ŀ���������Ӧ�ó���ر��¼���Ӧ�������ҷ����������Զ��壬������ntkoCloseEvent
function ntkoCloseEvent() {
    console.log("����������Ӧ�ó��򴰿��ѹر�!");
}

//�ڸ�ҳ�涨������ڽ�����ҳ��ش�ֵ�ķ����������������Զ��壬
//�����ķ�������Ҫ����ҳ����ͨ��window.external.SetReturnValueToParentPage����ע��
function OnData(callData) {
    var data = decodeURI(callData);
    $("#callback").val(data);
}

//�����������Ĵ򿪹��ĵķ�ʽ
