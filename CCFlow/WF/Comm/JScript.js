function numberFormat(obj, decimals) {
    /*
    * 参数说明：
    * number：要格式化的数字
    * decimals：保留几位小数
    * roundtag:舍入参数，默认 ‘ceil‘ 向上取,‘floor‘向下取,‘round‘ 四舍五入
    * */
    var number = obj.value;
    //obj不是对象的判断
    if (number == undefined) {
        number = obj;
    }
    number = (number + "").replace(/[^0-9+-Ee.]/g, "");
    number = number.replace(/,/g, "");
    roundtag = "round";  // 四舍五入
    var n = !isFinite(+number) ? 0 : +number;
    var prec = !isFinite(+decimals) ? 0 : Math.abs(decimals);
    var sep = ","; //千分位符号
    var dec = "."; //小数点符号
    var s = "";
    var toFixedFix = function (n, prec) {
        var k = Math.pow(10, prec);

        return "" + parseFloat(Math[roundtag](parseFloat((n * k).toFixed(prec * 2))).toFixed(prec * 2)) / k
    }
    s = (prec ? toFixedFix(n, prec) : "" + Math.round(n)).split(".");
    var re = /(-?\d+)(\d{3})/;
    while (re.test(s[0])) {
        s[0] = s[0].replace(re, "$1" + sep + "$2");
    }

    if ((s[1] || "").length < prec) {
        s[1] = s[1] || "";
        s[1] += new Array(prec - s[1].length + 1).join("0");
    }
    //obj不是对象的判断
    if (obj.value == undefined) {
        return s.join(dec);
    }
    obj.value = s.join(dec);//给原input框重新赋值
}
//获取WF之前路径
function GetLocalWFPreHref() {
    var url = GetHrefUrl();
    if (url.indexOf('/WF/') >= 0) {
        var index = url.indexOf('/WF/');
        url = url.substring(0, index);
    }
    return url;
}

function endWith(str, s) {
    if (s == null || s == "" || str.length == 0 || s.length > str.length)
        return false;
    if (str.substring(str.length - s.length) == s)
        return true;
    else
        return false;
    return true;
}

function startWith(str, s) {
    if (s == null || s == "" || str.length == 0 || s.length > str.length)
        return false;
    if (str.substr(0, s.length) == s)
        return true;
    else
        return false;
    return true;

}

function replaceAll(s1, s2, s3) {
    return s1.replace(new RegExp(s2, 'gm'), s3);
}


/**
 * 输入验证firfox, ff浏览器不支持execCommand()
 */
function isFF() {
    return navigator.userAgent.indexOf("Firefox") > 0;
}
function valitationBefore(o, validateType) {
    if (isFF()) {
        var value = o.value;
        var flag = false;
        switch (validateType) {
            case "int":
                flag = (!isNaN(value) && value % 1 === 0);
                break;
            case "float":
            case "money":
                if (validateType == "money")
                    value = value.replace(/,/g, "");
                if (value.indexOf("-") == 0 && value.length == 1)
                    break;
                else {
                    flag = !isNaN(value);
                    break;
                }
        }
        if (flag) {
            return;
        }
    }
}

var idx = 0;
var oldCount = 0;
function valitationAfter(o, validateType) {
    // debugger
    idx = getCursortPosition(o);
    oldCount = getStrCount(o.value.toString().substr(0, idx), ',');
    var value = o.value;
    if (validateType == "int") {
        value = value.replace(/[^\d,-]/g, "");
        o.value = value;
    }


    //if (isFF()) {
    var flag = false;
    switch (validateType) {
        case "int":
            flag = (!isNaN(value) && value % 1 === 0);
            break;
        case "float":
        case "money":
            if (validateType == "money")
                value = value.replace(/,/g, "");
            if (value.indexOf("-") == 0 && value.length == 1)
                break;
            else {
                flag = !isNaN(value);
                break;
            }
    }
    if (!flag) {
        o.value = '';
    }

}



function addplaceholder(obj, bit) {
    if (obj.value == "") {
        //obj.value = 0;
        //return;
        switch (bit) {
            case 0:
                obj.value = '0';
                break;
            default:
                obj.value = '0.00';
        }

    }

}
function removeplaceholder(obj, bit) {
    if (obj.value == 0) {

        return obj.value = '';
    }

}
/**
 * 输入验证firfox, ff浏览器不支持execCommand()
 */

function limitLength(obj, length) {
    obj.value = obj.value.replace(/[^\d,.-]/g, "");  //清除“数字”和“.”以外的字符 ;
    if (length != null && length != "" && length != "undefined") {
        if (obj.value.indexOf('.') >= 0 && obj.value.split('.')[1].length > length) {
            var val = obj.value.toString();
            obj.value = val.toFixed(length);
            //obj.focus();
        }
    }
}

//类型为Money时输入设置
function clearNoNum(obj) {
    //修复第一个字符是小数点 的情况.  
    if (obj.value != '' && obj.value.substr(0, 1) == '.') {
        obj.value = 0 + obj.value;
    }
    if (obj.value == "")
        obj.value = 0.00;

    //if (!/\./.test(obj.value)) //为整数字符串在末尾添加.00  
    //   obj.value += '.00';

    obj.value = obj.value.replace(/^0*(0\.|[1-9])/, '$1');//解决 粘贴不生效  
    obj.value = obj.value.replace(/[^\d.]/g, "");  //清除“数字”和“.”以外的字符  
    obj.value = obj.value.replace(/\.{2,}/g, "."); //只保留第一个. 清除多余的       
    obj.value = obj.value.replace(".", "$#$").replace(/\./g, "").replace("$#$", ".");
    obj.value = obj.value.replace(/^(\-)*(\d+)\.(\d\d).*$/, '$1$2.$3');//只能输入两个小数       
    if (obj.value.indexOf(".") < 0 && obj.value != "") {//以上已经过滤，此处控制的是如果没有小数点，首位不能为类似于 01、02的金额  
        if (obj.value.substr(0, 1) == '0' && obj.value.length == 2) {
            obj.value = obj.value.substr(1, obj.value.length);
        }
    }
}

//设置光标位置
function setCaretPosition(ctrl, pos) {
    if (ctrl.setSelectionRange) {
        ctrl.focus();
        ctrl.setSelectionRange(pos, pos);
    }
    else if (ctrl.createTextRange) {
        var range = ctrl.createTextRange();
        range.collapse(false);
        range.moveEnd('character', pos);
        range.moveStart('character', pos);
        range.select();
    }
}

// 获取光标位置
function getCursortPosition(ctrl) {
    var CaretPos = 0;   // IE Support
    if (document.selection) {
        ctrl.focus();
        var Sel = document.selection.createRange();
        Sel.moveStart('character', -ctrl.value.length);
        CaretPos = Sel.text.length;
    }
    // Firefox support
    else if (ctrl.selectionStart || ctrl.selectionStart == '0')
        CaretPos = ctrl.selectionStart;
    return (CaretPos);
}

function FormatMoney(obj, precision, separator, isDealPrec) {
    //获取之前的，
    var oldV = obj.value;

    if (precision == undefined || precision == null || precision == "")
        precision = 2;
    //if (precision != 2)
    //   return;
    oldV = oldV.replace(/[^\d.-]/g, "");
    var val = formatNumber(oldV, precision, separator, isDealPrec);
    if (val != NaN)
        obj.value = val;
    var didx = getStrCount(val.toString().substr(0, idx), ',');
    if (didx > oldCount && didx > 0)
        idx = idx + 1;
    if (isDealPrec==0)
        setCaretPosition(obj, idx);


}

//统计字符串中特定字符串的个数
function getStrCount(scrstr, armstr) { //scrstr 源字符串 armstr 特殊字符
    var count = 0;
    while (scrstr.indexOf(armstr) >= 1) {
        scrstr = scrstr.replace(armstr, "")
        count++;
    }
    return count;
}
/** 
* 将数值格式化成金额形式 
* 
* @param num 数值(Number或者String) 
* @param precision 精度，默认不变
* @param separator 分隔符，默认为逗号
* @return 金额格式的字符串,如'1,234,567'，默认返回NaN
* @type String 
*/
function formatNumber(num, precision, separator, isDealPrec) {

    //if (precision != 2)
    //   return num;
    var parts;
    // 判断是否为数字
    if (!isNaN(parseFloat(num)) && isFinite(num)) {
        // 把类似 .5, 5. 之类的数据转化成0.5, 5, 为数据精度处理做准, 至于为什么
        // 不在判断中直接写 if (!isNaN(num = parseFloat(num)) && isFinite(num))
        // 是因为parseFloat有一个奇怪的精度问题, 比如 parseFloat(12312312.1234567119)
        // 的值变成了 12312312.123456713
        //num = Number(num);
        num = num.toString();

        // 处理小数点位数
        if (isDealPrec == 1 && typeof precision != "undefined" && precision) {
            var suf = 0;
            if (num.indexOf(".") != -1)
                suf = num.substring(num.indexOf(".") + 1, num.length).length;
            if (suf == 0 && num.indexOf(".") == -1)
                num = num + ".";
            for (var i = suf; i < precision; i++)
                num += "0";

        }
        //    num = (typeof precision !== 'undefined' ? (Math.round(num * Math.pow(10, precision)) / Math.pow(10, precision)).toFixed(precision) : num).toString();

        // 分离数字的小数部分和整数部分
        parts = num.split('.');
        // 整数部分加[separator]分隔, 借用一个著名的正则表达式
        parts[0] = parts[0].toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1' + (separator || ','));
        return parts.join('.');
    }
    return 0;
}
/* 文本框根据输入内容自适应高度
* @param                {HTMLElement}        输入框元素
* @param                {Number}                设置光标与输入框保持的距离(默认0)
* @param                {Number}                设置最大高度(可选)
*/
var autoTextarea = function (elem, extra, maxHeight) {
    extra = extra || 0;
    var isFirefox = !!document.getBoxObjectFor || 'mozInnerScreenX' in window,
        isOpera = !!window.opera && !!window.opera.toString().indexOf('Opera'),
        addEvent = function (type, callback) {
            elem.addEventListener ?
                elem.addEventListener(type, callback, false) :
                elem.attachEvent('on' + type, callback);
        },
        getStyle = elem.currentStyle ? function (name) {
            var val = elem.currentStyle[name];

            if (name === 'height' && val.search(/px/i) !== 1) {
                var rect = elem.getBoundingClientRect();
                return rect.bottom - rect.top -
                    parseFloat(getStyle('paddingTop')) -
                    parseFloat(getStyle('paddingBottom')) + 'px';
            };

            return val;
        } : function (name) {
            return getComputedStyle(elem, null)[name];
        },
        minHeight = parseFloat(getStyle('height'));

    elem.style.resize = 'none';

    var change = function () {
        var scrollTop, height,
            padding = 0,
            style = elem.style;

        if (elem._length === elem.value.length) return;
        elem._length = elem.value.length;

        if (!isFirefox && !isOpera) {
            padding = parseInt(getStyle('paddingTop')) + parseInt(getStyle('paddingBottom'));
        };
        scrollTop = document.body.scrollTop || document.documentElement.scrollTop;

        elem.style.height = minHeight + 'px';
        if (elem.scrollHeight > minHeight) {
            if (maxHeight && elem.scrollHeight > maxHeight) {
                height = maxHeight - padding + 10;
                style.overflowY = 'auto';
            } else {
                height = elem.scrollHeight - padding + 10;
                style.overflowY = 'hidden';
            };
            style.height = height + extra + 'px';
            scrollTop += parseInt(style.height) - elem.currHeight;
            document.body.scrollTop = scrollTop;
            document.documentElement.scrollTop = scrollTop;
            elem.currHeight = parseInt(style.height);
        };
    };

    addEvent('propertychange', change);
    addEvent('input', change);
    addEvent('focus', change);
    change();
};

//身份证信息
function GetIDCardInfo(event) {
    var formData = new FormData();
    formData.append("file", event.target.files[0]);
    var url = dynamicHandler + "?DoType=HttpHandler&DoMethod=GetIDCardInfo&HttpHandlerName=BP.WF.HttpHandler.CCMobile_CCForm";
    $.ajax({
        type: "post",
        url: url,
        data: formData,
        processData: false,
        contentType: false,

        success: function (data) {
            data = JSON.parse(data);
            if (data.error_code != undefined) {
                console.log(data);
                alert('上传身份证解析错误:' + data.error_code + data.error_msg);
            } else {
                //显示给指定的字段赋值
                $("#TB_IDCardName").val(data.words_result.姓名.words);
                $("#TB_IDCardNo").val(data.words_result.公民身份号码.words);
                $("#TB_IDCardAddress").val(data.words_result.住址.words);
                $("#TB_IDCardName").html(data.words_result.姓名.words);
                $("#TB_IDCardNo").html(data.words_result.公民身份号码.words);
                $("#TB_IDCardAddress").html(data.words_result.住址.words);
                var mianData;
                if ("undefined" != typeof flowData && flowData != null && flowData != undefined)
                    mianData = flowData;
                if ((mianData == null || mianData == undefined) && ("undefined" != typeof frmData && frmData != null && frmData != undefined))
                    mianData = frmData;

                if (mianData != null && mianData != undefined) {
                    var mapExts = mianData.Sys_MapExt;
                    var mapAttrs = mianData.Sys_MapAttr;
                    for (var i = 0; i < mapExts.length; i++) {
                        var mapExt = mapExts[i];
                        var mapAttr = null;
                        for (var j = 0; j < mapAttrs.length; j++) {
                            if (mapAttrs[j].FK_MapData == mapExt.FK_MapData && mapAttrs[j].KeyOfEn == mapExt.AttrOfOper) {
                                mapAttr = mapAttrs[j];
                                break;
                            }
                        }
                        if (mapAttr == null)
                            continue;
                        if (mapAttr.UIContralType != 13)
                            continue;

                        var DDLFull = GetPara(mapAttr.AtPara, "IsFullData");
                        if (DDLFull != undefined && DDLFull != "" && DDLFull == "1" && (mapExt.MyPK.indexOf("DDLFullCtrl") != -1)) {
                            FullIt($("#TB_" + mapExt.AttrOfOper).val(), mapExt.MyPK, "TB_" + mapExt.AttrOfOper);
                        }
                    }
                }


            }

        },
        error: function (e) {

        }
    });

}

