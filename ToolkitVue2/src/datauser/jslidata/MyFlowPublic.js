/*
1. ��JS�ļ���Ƕ�뵽��MyFlowGener.htm �Ĺ�����������. 
2. �����߿�����д���ļ�����ͨ�õ�Ӧ��,����ͨ�õĺ���.
*/

function GenerNextStepEmp() {
    var qingjiaren = $("TB_QingJiaRen").val();

    var url = "xxx.aspx?QingJiaRen=" + qingjiaren;

    $("TB_DiYiJiShenPiRen").val("zhangsna");

    return "";
}

//ҳ����������.
$(document).ready(function () {
    //������ɸѡ���������򣨴˷����ڱ�����ǰִ�еģ�ȡ������Ӧ�ı�Ԫ�أ��޷����ɸѡ��
    LoaclOperation();

});

/*
 * �˺���Ϊ����ǰ��JSЧ����demo����.
 * 1. �����������ʹ��jquery�﷨.
 * 2. ���õ�����JS��ť�� if (CheckBlank()==false) return true; ���ɱ�����.
 * 3. �˷���Ҳ����д�뵽 xxxx_Self.js����.
 * 4. return trueִ�з��Ͷ�����return false ��ִֹ��.
 **/
function CheckBlank() {
    if ($("#TB_Email").val() == null) {
        alert('�ʼ�����Ϊ��.');
        return false;
    }
    if ($("#TB_Tel").val() == null) {
        alert('�绰����Ϊ��.');
        return false;
    }
    return true;
}

function DZ() {

    alert('sss');
    var url = 'pop.htm';
    window.open(url);
}
function LoaclOperation() {

    if (GetQueryString("NodeID") != "202")
        return;
    var val = $("#DDL_SFBJBL option:selected").val();

    if (val == 0) {

        //���������̵�ѡ������Ϊ���ɼ�
        $("#DDL_ToNode option").eq(0).hide();
        $("#DDL_ToNode option").eq(1).hide();
        $("#DDL_ToNode option").eq(2).show();

        //��Bar�ϵķ��ͽڵ�����Ϊ���ڵ�
        $("#DDL_ToNode").val("203");

    }
    else {

        //��ʾ�������������̵�ѡ��
        $("#DDL_ToNode option").eq(0).show();
        $("#DDL_ToNode option").eq(1).show();
        $("#DDL_ToNode option").eq(2).hide();

        //��Bar�ϵķ��ͽڵ�����Ϊ���ڵ�
        $("#DDL_ToNode").val("302");

    }
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


/**
 * ������֮�󣬵���������Ϣ.
 * @param {html��ʽ����Ϣ} msg
 */
function WindowCloseReloadPage(msg) {
    return;

    if ($('#returnWorkModal:hidden').length == 0 && $('#returnWorkModal').length > 0) {
        $('#returnWorkModal').modal('hide');
    }

    //����msg��ģ̬����
    //��ʼ���˻ش��ڵ�SRC.
    var html = '<div class="modal fade" id="msgModal" data-backdrop="static">'
        + '<div class="modal-dialog">'
        + '<div class="modal-content" style="border-radius: 0px;">'
        + '<div class="modal-header" style="background:#f2f2f2;">'
        + '<button type="button" class="close" id="btnMsgModalOK1" aria-hidden="true" style="color: #0000007a;display: none;">&times;</button>'
        + '<h4 class="modal-title" style="color:#000;">��ʾ��Ϣ</h4>'
        + '</div>'
        + '<div class="modal-body" style="text-align: left; word-wrap: break-word;">'
        + '<div style="width:100%; border: 0px; height: 200px;overflow-y:auto" id="msgModalContent" name="iframePopModalForm"></div>'
        + '<div style="text-align: right;">'
        + ' <button type="button" id="btnMsgModalOK" class="btn" data-dismiss="modal">ȷ��(30��)</button >'
        + '</div>'
        + '</div>'
        + '</div><!-- /.modal-content -->'
        + '</div><!-- /.modal-dialog -->'
        + '</div>';

    $('body').append($(html));
    if (msg == null || msg == undefined)
        msg = "";
    msg = msg.replace("@�鿴<img src='/WF/Img/Btn/PrintWorkRpt.gif' >", '')

    $("#msgModalContent").html(msg.replace(/@/g, '<br/>').replace(/null/g, ''));
    var trackA = $('#msgModalContent a:contains("�����켣")');
    var trackImg = $('#msgModalContent img[src*="PrintWorkRpt.gif"]');
    trackA.remove();
    trackImg.remove();

    $('#btnMsgModalOK').bind('click', function () {
        var id = window.parent.nthTabs.getActiveId();
        var idlist = id.split("TLJ");
        //console.log("==="+idlist);
        if (idlist.length > 0) {
            $('#' + idlist[1], parent.document).attr('src', $('#' + idlist[1], parent.document).attr('src'));
        }
        window.parent.nthTabs.delTab(id);
    });
    $('#btnMsgModalOK1').bind('click', function () {
        //��ʾ��Ϣ�д���ҳ�治��ת
        var msg = $("#msgModalContent").html();
        if (msg.indexOf("err@") == -1) {
            window.close();
        }
        else {
            setToobarEnable();
            $("#msgModal").modal("hidden");
        }

        if (window.parent != null && window.parent != undefined)
            window.parent.close();
        opener.window.focus();
    });

    $("#msgModal").modal().show();
    interval = setInterval("clock()", 1000);
}


