
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

//function AfterWindowLoad() {
//}

////toolbar ���غ�ִ�е��¼�.
//function AfterToolbarLoad() {
//}
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

    if (1 == 2) {
        //���������.
        var xb = $("#DDL_XB").val();
    }

 //   var xb = this.r $("#DDL_XB").val();

    return true;
}
 
function LoaclOperation() {

}
/*

1. beforeSave��beforeSend�� beforeReturn�� beforeDelete 
2 .MyFlowGener��MyFlowTree�Ĺ̶���������ֹɾ��
3.��Ҫд����ǰ������ǰ���˻�ǰ��ɾ��ǰ�¼�
4.����ֵΪ true��false,  �������false �Ͳ�ִ�и��¼�.

*/

//����ǰ�¼�
function beforeSave() {
    return true;
}

//����ǰ�¼�
function beforeSend() {
    var nodeID = GetQueryString("NodeID"); //��õ�ǰ�ڵ�.
    if (nodeID == 101) {
        var name = $("#TB_Name").val(); //����ʹ��jquery.
    }
    if (nodeID === 702)
    {
        return CheckBlank();
    }



    if (nodeID === 901) {

        //var km = $("#DDL_KM").val();
        //var hj = $("#TB_BaoXiaoJinE").val();
        //if (km == 0 && hj > 10000) {
        //    alert('�������ܳ���1w.');
        //    return false;
        //}
        return true;
       // return CheckBlank();
    }

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

/**
 * ���ø���Ŀ���Ե���ǩ�ִ���
 */
function Siganture() {

}
/**
 * ���ø���Ŀ���Ը��´���
 */
function Stamp() {

}


