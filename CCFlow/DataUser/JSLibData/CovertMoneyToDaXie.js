function Rmb2DaXie(val) {
    //�˴���Ԥ������ؼ�����ͨ���ı���ʱ��¼������ֵķ����Զ�ȥ��
    var rmb = clearNoNum(val);
    var dx = AmountLtoU(rmb);
    if (dx == '��Ч') {
        alert('��Ч�����ָ�ʽ��');
        return false;
    }

    return dx;
}


function AmountLtoU(num) {
    ///<summery>Сд���ת����д���</summery>
    ///<param name=num type=number>���</param>
    if (isNaN(num)) return "��Ч";
    var strPrefix = "";
    if (num < 0) strPrefix = "(��)";
    num = Math.abs(num);
    if (num > 999000000000000) return "����(������999����)";    //������999����
    var strOutput = "";
    var strUnit = '��ʰ��Ǫ��ʰ��Ǫ��ʰ��Ǫ��ʰԲ�Ƿ�';
    var strCapDgt = '��Ҽ��������½��ƾ�';
    num += "00";
    var intPos = num.indexOf('.');
    if (intPos >= 0) {
        num = num.substring(0, intPos) + num.substr(intPos + 1, 2);
    }
    strUnit = strUnit.substr(strUnit.length - num.length);
    for (var i = 0; i < num.length; i++) {
        strOutput += strCapDgt.substr(num.substr(i, 1), 1) + strUnit.substr(i, 1);
    }
    return strPrefix + strOutput.replace(/������$/, '��').replace(/��[Ǫ��ʰ]/g, '��').replace(/��{2,}/g, '��').replace(/��([��|��])/g, '$1').replace(/��+Բ/, 'Բ').replace(/����{0,3}��/, '��').replace(/^Բ/, "��Բ");
};

function getArgsFromHref(sArgName) {
    var sHref = window.location.href;
    var args = sHref.split("?");
    var retval = "";

    if (args[0] == sHref) /*����Ϊ��*/
    {
        return retval; /*�������κδ���*/
    }

    var str = args[1];
    args = str.split("&");

    for (var i = 0; i < args.length; i++) {
        str = args[i];
        var arg = str.split("=");
        if (arg.length <= 1) continue;
        if (arg[0] == sArgName) retval = arg[1];
    }

    return retval;
}

function clearNoNum(val) {
    val = val.replace(/[^\d.]/g, "");  //��������֡��͡�.��������ַ�
    val = val.replace(/^\./g, "");  //��֤��һ���ַ������ֶ�����.
    val = val.replace(/\.{2,}/g, "."); //ֻ������һ��. ��������.
    val = val.replace(".", "$#$").replace(/\./g, "").replace("$#$", ".");
    return val;
}