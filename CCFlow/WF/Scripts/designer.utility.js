/// <reference path="easyUI/jquery-1.8.0.min.js" />
/// <reference path="easyUI/jquery.easyui.min.js" />

//added by liuxc,2015-02-12
//此文件可用于存放流程/表单设计器用到的公用JS方法等

var isie = isIE(),
    uh = new UrlHerf(location.href);

/*公用类*/
function Params() {
    /// <summary>JSON传输data参数生成对象</summary>
    /// <desc>可使用Params.push(key,value)来添加参数，value也可为Array数组</desc>
    this.keys = new Array();
    this.values = new Array();

    if (typeof Params._initialized == "undefined") {
        Params.prototype.push = function (key, value) {
            if (key == undefined || key == null) {
                $.messager.alert('错误', 'key不能为空', 'error');
            }

            this.keys.push(key);
            this.values.push(value);
        }

        Params.prototype.clear = function () {
            this.keys.length = 0;
            this.values.length = 0;
        }

        Params.prototype.toJsonDataString = function () {
            var s = '{';
            var isString, isArr;

            for (var i = 0, j = this.keys.length; i < j; i++) {
                isString = typeof this.values[i] === 'string';
                isArr = isArray(this.values[i]);
                s += this.keys[i] + ":";

                if (isString) {
                    s += "'" + this.values[i] + "',";
                }
                else if (isArr) {
                    s += "[";
                    isString = typeof this.values[i][0] === 'string';

                    $.each(this.values[i], function () {
                        s += (isString ? "'" : "") + this + (isString ? "'," : ",");
                    });

                    s = removeLastComma(s) + "],";
                }
                else if (this.values[i] == null) {
                    s += "null,";
                }
                else {
                    s += this.values[i] + ",";
                }
            }

            s = removeLastComma(s) + '}';
            return s;
        }

        Params._initialized = true;
    }
}

function UrlHerf(url) {
    /// <summary>url传参辅助类</summary>
    /// <param name="sLocationSearch" Type="String">传递的url的</param>
    /// <desc>可直接使用索引式属性来获取传递参数的值，如：
    /// <para> var uh = new UrlHerh(location.href);
    /// <para> var fk_flow = uh["fk_flow"];
    /// </desc>
    this.lurl = url;

    if (typeof UrlHerf._initialized == "undefined") {
        UrlHerf.prototype.check = function () {
            var urls = (this.lurl || "?").split('?');
            if (urls.length != 2) return;

            var uparams = urls[1].split('&'),
                uparam;

            for (var i = 0; i < uparams.length; i++) {
                uparam = uparams[i].split('=');

                if (uparam.length < 2) continue;

                this[uparam[0]] = uparam[1];
            }
        }
    }

    this.check();
}

/*公用方法*/
function ajaxService(type, method, dataString, fnSuccess, fnSuccessArgs, fnError, fnErrorArgs) {
    /// <summary>ajax异步调用/Admin/XAP/WebService.asmx</summary>
    /// <param name="type" Type="String">调用的是流程[flow]还是表单[form]的服务</param>
    /// <param name="method" Type="String">WebService公开方法</param>
    /// <param name="dataString" Type="String">调用时发送的数据，格式必须与$.ajax方法的data数据格式一致，如"{name:'xxx',age:12}"
    /// <para>可使用Params类生成该字符串,如：
    /// <para>  var ps = new Params();
    /// <para>  ps.push('name','xxx');
    /// <para>  ps.push('age',12);
    /// <para>  var dataString = ps.toJsonDataString();
    /// <para>输出：{'name':'xxx','age':12}
    /// </param>
    /// <param name="fnSuccess" Type="Function">调用成功后，要运行的方法，如：function(re){}，其中re为异步调用返回的结果</param>
    /// <param name="fnSuccessArgs" Type="Object">调用成功后运行方法的参数</param>
    /// <param name="fnError" Type="Function">调用失败后，要运行的方法，如：function(re){}，其中re为异步调用失败的responseText</param>
    /// <param name="fnErrorArgs" Type="Object">调用失败后运行方法的参数</param>

    var asmx = type.toLowerCase() == "flow" ? "FlowDesignerSvr.asmx/" : "FormDesignerSvr.asmx/";
    
    $.ajax({
        type: "Post",
        contentType: "application/json;utf-8",
        url: asmx + method,
        dataType: "json",
        data: dataString,
        success: function (re) {
            if (fnSuccess != undefined) {
                fnSuccess(re.d, fnSuccessArgs);
            }
        },
        error: function (re) {
            if (fnError != undefined) {
                fnError(re, fnErrorArgs);
            }
            else {
                $.messager.alert('错误', re.responseText, 'error');
            }
        }
    });
}

function getMaxInArray(arr, propName) {
    /// <summary>获取指定对象数组中指定属性的最大值</summary>
    /// <param name="arr" Type="Array">对象数组</param>
    /// <param name="propName" Type="String">属性名称</param>
    var max = 0;

    $.each(arr, function () {
        for (prop in this) {
            if (prop == propName && !isNaN(this[prop])) {
                max = Math.max(max, this[prop]);
            }
        }
    });

    return max;
}

function checkUrl(url) {
    /// <summary>判断远程路径是否可以连接成功</summary>
    /// <param name="url" Type="String">远程路径url</param>
    var isSuccess;

    $.ajax({
        type: 'GET',
        cache: false,   //不下载远程url
        async: false,   //同步
        url: url,
        data: '',
        success: function () {
            isSuccess = true;
        },
        error: function () {
            isSuccess = false;
        }
    });

    return isSuccess;
}

function removeLastComma(str) {
    /// <summary>去除指定字符串最后的逗号</summary>
    /// <param name="str" Type="String">字符串</param>
    if (str.charAt(str.length - 1) == ',') {
        return str.substr(0, str.length - 1);
    }

    return str;
}

function isArray(object) {
    /// <summary>判断是否是数组</summary>
    /// <param name="object" Type="Object">要判断的对象</param>
    return object && typeof object === 'object' &&
            Array == object.constructor;
}

function getNavigatorInfo() {
    ///<summary>获取浏览器及版本信息</summary>
    var Sys = {};
    var ua = navigator.userAgent.toLowerCase();
    var s;
    (s = ua.match(/msie ([\d.]+)/)) ? Sys.ie = s[1] :
        (s = ua.match(/firefox\/([\d.]+)/)) ? Sys.firefox = s[1] :
        (s = ua.match(/chrome\/([\d.]+)/)) ? Sys.chrome = s[1] :
        (s = ua.match(/opera.([\d.]+)/)) ? Sys.opera = s[1] :
        (s = ua.match(/version\/([\d.]+).*safari/)) ? Sys.safari = s[1] : 0;
    return Sys;
}

function isIE() {
    return navigator.userAgent.toLowerCase().match(/msie ([\d.]+)/);
}

/*新增系统类的公用方法*/
String.prototype.format = function (args) {
    var result = this;
    if (arguments.length > 0) {
        if (arguments.length == 1 && typeof (args) == "object") {
            for (var key in args) {
                if (args[key] != undefined) {
                    var reg = new RegExp("({" + key + "})", "g");
                    result = result.replace(reg, args[key]);
                }
            }
        }
        else {
            for (var i = 0; i < arguments.length; i++) {
                if (arguments[i] != undefined) {
                    //var reg = new RegExp("({[" + i + "]})", "g");
                    ////这个在索引大于9时会有问题，谢谢何以笙箫的指出
                    var reg = new RegExp("({)" + i + "(})", "g");
                    result = result.replace(reg, arguments[i]);
                }
            }
        }
    }
    return result;
}

Array.prototype.index = function (obj) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] != undefined && this[i] == obj) {
            return i;
        }
    }

    return -1;
}

Array.prototype.remove = function (obj) {
    var isExist = false;
    for (var i = 0, n = 0; i < this.length; i++) {
        if (this[i] != obj) {
            this[n++] = this[i]
        }
        else if (isExist) {
            continue;
        }
        else {
            isExist = true;
        }
    }

    if (isExist) {
        this.length -= 1
    }
}

Array.prototype.findByField = function (field, value) {
    /// <summary>检索数组中的对象，找出对象中指定属性指定值的对象</summary>
    /// <param name="field" Type="String">属性名</param>
    /// <param name="value" Type="Object">属性值</param>
    for (var i = 0; i < this.length; i++) {
        if (this[i][field] != undefined && this[i][field] == value) {
            return this[i];
        }
    }

    return null;
}

Array.prototype.findBy2Field = function (field1, value1, field2, value2) {
    /// <summary>检索数组中的对象，找出对象中指定属性指定值的对象</summary>
    /// <param name="field1" Type="String">属性名</param>
    /// <param name="value1" Type="Object">属性值</param>
    /// <param name="field2" Type="String">属性名</param>
    /// <param name="value2" Type="Object">属性值</param>
    for (var i = 0; i < this.length; i++) {
        if (this[i][field1] != undefined && this[i][field1] == value1 && this[i][field2] != undefined && this[i][field2] == value2) {
            return this[i];
        }
    }

    return null;
}