/*
1. ��JS�ļ���Ƕ�뵽��MyFlowGener.htm �Ĺ�����������. 
2. �����߿�����д���ļ�����ͨ�õ�Ӧ��,����ͨ�õĺ���.
*/

//ҳ����������.

function LoaclOperation() {

    var nodeID = GetQueryString("FK_Node");

    if (nodeID == 302 || nodeID == 402 || nodeID == 502 || nodeID == 621) {

    } else {
        return;
    }

    var len = document.getElementById("DDL_Nodes").options.length;
    if (len <= 2)
        return;

    for (var i = 0; i < len; i++) {

            $("#DDL_Nodes option").eq(i).hide();
    }

    $("#DDL_Nodes option").eq(len - 1).show();

    $("#DDL_Nodes").attr('value', $('#DDL_Nodes option:last').val());

    //var len = document.getElementById("DDL_Nodes").options.length;

    return;
}


