// ����ͼ��
function ToolbarIcon() {

    //����һ��Json.
    var strs = "Send,Save,Shift,Return";
     
    //ѭ�����json.
    for (var i = 0; i < strs.length; i++) {

        var str = strs[i];

        var btn = $("#Btn_" + str);
        if (btn == null)
            continue;
        btn.css('background', '#FFF url(../DataUser/OverrideFiles/ToolbarImg/Send.png) no-repeat left 50%');
    } 
}
