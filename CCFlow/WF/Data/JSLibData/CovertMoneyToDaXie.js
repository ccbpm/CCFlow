function Rmb2DaXie(s) {
    s = clearNoNum(s.toString());  //此处可预防如果控件是普通的文本框时，录入非数字的符号自动去除
    var dx = AmountLtoU(s);
    if (dx == '无效') {
        alert('无效的数字格式！');
        return false;
    }

    return dx;
}

var DaXie_Ctrl_ID = 'DaXie';
var dxctrl;

function AmountLtoU(num) {
    ///<summery>小写金额转化大写金额</summery>
    ///<param name=num type=number>金额</param>
    if (isNaN(num)) return "无效";
    var strPrefix = "";
    if (num < 0) strPrefix = "(负)";
    num = Math.abs(num);
    if (num > 999000000000000) return "超额(不大于999万亿)";    //不超过999万亿
    var strOutput = "";
    var strUnit = '佰拾万仟佰拾亿仟佰拾万仟佰拾圆角分';
    var strCapDgt = '零壹贰叁肆伍陆柒捌玖';
    num += "00";
    var intPos = num.indexOf('.');
    if (intPos >= 0) {
        num = num.substring(0, intPos) + num.substr(intPos + 1, 2);
    }
    strUnit = strUnit.substr(strUnit.length - num.length);
    for (var i = 0; i < num.length; i++) {
        strOutput += strCapDgt.substr(num.substr(i, 1), 1) + strUnit.substr(i, 1);
    }
    return strPrefix + strOutput.replace(/零角零分$/, '整').replace(/零[仟佰拾]/g, '零').replace(/零{2,}/g, '零').replace(/零([亿|万])/g, '$1').replace(/零+圆/, '圆').replace(/亿零{0,3}万/, '亿').replace(/^圆/, "零圆");
};

function getArgsFromHref(sArgName) {
    var sHref = GetHrefUrl();
    var args = sHref.split("?");
    var retval = "";

    if (args[0] == sHref) /*参数为空*/
    {
        return retval; /*无需做任何处理*/
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

function clearNoNum(obj) {
    obj = obj.toString();
    obj = obj.replace(/[^\d.]/g, "");  //清除“数字”和“.”以外的字符
    obj = obj.replace(/^\./g, "");  //验证第一个字符是数字而不是.
    obj = obj.replace(/\.{2,}/g, "."); //只保留第一个. 清除多余的.
    obj = obj.replace(".", "$#$").replace(/\./g, "").replace("$#$", ".");
    return obj;
}