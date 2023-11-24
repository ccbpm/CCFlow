import $ from 'jquery'
let dynamicHandler = '';
let parameters = {};
let formData;
import {
    GetQueryString,
} from './QueryString'

// 定义公共引用方法  需要element 改造
function ThrowMakeErrInfo(funcName, obj, url) {

    let msg = "1. " + funcName + " err@系统发生异常.";
    msg += "\t\n2.检查请求的URL连接是否错误：" + url;
    msg += "\t\n3.估计是数据库连接错误或者是系统环境问题. ";
    msg += "\t\n4.技术信息:status: " + obj.status + " readyState: " + obj.readyState;
    msg += "\t\n5 您可以执行一下http://127.0.0.1/WF/Default.aspx/jsp/php 测试一下，动态文件是否可以被执行。";
    alert(msg);
}

// 定义公共引用方法  需要element 改造
function ToJson(data) {

    try {
        data = JSON.parse(data);
        return data;
    } catch (e) {
        return eval(data);
    }

}



const HttpHandler = function (handlerName) {
    this.handlerName = handlerName;
    parameters = {};
    formData = undefined;
    this.validate = function (s) {
        if (s == null || typeof s === "undefined") {
            return false;
        }
        s = s.replace(/^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g, "");
        if (s == "" || s == "null" || s == "undefined") {
            return false;
        }
        return true;
    }
    dynamicHandler = process.env.VUE_APP_HANDLER;
}

HttpHandler.prototype = {

    constructor: HttpHandler,
    AddUrlData: function (url) {
        let queryString = url;
        if (url == null || url == undefined || url == "")
            queryString = document.location.search.substr(1);
        queryString = decodeURI(queryString);
        let self = this;
        $.each(queryString.split("&"), function (i, o) {
            var param = o.split("=");
            if (param.length == 2 && self.validate(param[1])) {

                (function (key, value) {

                    if (key == "DoType" || key == "DoMethod" || key == "HttpHandlerName")
                        return;

                    self.AddPara(key, value);

                })(param[0], param[1]);
            }
        });

    },

    AddFormData: function () {
        if ($("form").length == 0)
            throw Error('必须是Form表单才可以使用该方法');

        formData = $("form").serialize();
        //序列化时把空格转成+，+转义成％２Ｂ，在保存时需要把+转成空格
        formData = formData.replace(/\+/g, " ");
        //form表单序列化时调用了encodeURLComponent方法将数据编码了
        // formData = decodeURIComponent(formData, true);
        if (formData.length > 0) {
            let self = this;
            $.each(formData.split("&"), function (i, o) {
                let param = o.split("=");
                if (param.length == 2 && self.validate(param[1])) {
                    (function (key, value) {
                        self.AddPara(key, decodeURIComponent(value, true));
                    })(param[0], param[1]);
                }
            });
        }
    },

    AddPara: function (key, value) {
        parameters[key] = value;
    },

    AddJson: function (json) {

        for (let key in json) {
            parameters[key] = json[key];
        }
    },

    Clear: function () {
        parameters = {};
        formData = undefined;
    },

    getParams: function () {
        let params = [];
        $.each(parameters, function (key, value) {

            if (value.indexOf('<script') != -1)
                value = '';

            params.push(key + "=" + value);

        });




        return params.join("&");
    },

    DoMethodReturnString: function (methodName) {
        if (dynamicHandler == "")
            return;
        let self = this;
        let jsonString;
         // 如果没有携带token， 自动补上
         if (Object.prototype.hasOwnProperty.call(parameters, 'Token') == false) {
            parameters['Token']=GetQueryString('Token');
        }
        $.ajax({
            type: 'post',
            async: false,
            xhrFields: {
                withCredentials: true
            },
            crossDomain: true,
            credentials: true,
            url: dynamicHandler + "?DoType=HttpHandler&DoMethod=" + methodName + "&HttpHandlerName=" + self.handlerName + "&t=" + Math.random(),
            data: parameters,
            dataType: 'html',
            success: function (data) {
                jsonString = data;
            },
            error: function (XMLHttpRequest, textStatus) {
                var url = dynamicHandler + "?DoType=HttpHandler&DoMethod=" + methodName + "&HttpHandlerName=" + self.handlerName + "&t=" + Math.random();
                ThrowMakeErrInfo("HttpHandler-DoMethodReturnString-" + methodName, textStatus, url);
            }
        });

        return jsonString;

    },

    DoMethodReturnJSON: function (methodName) {

        let jsonString = this.DoMethodReturnString(methodName);

        if (jsonString.indexOf("err@") == 0) {
            alert(jsonString);

            //alert('请查看控制台(DoMethodReturnJSON):' + jsonString);
            console.log(jsonString);
            return jsonString;
        }

        try {

            jsonString = ToJson(jsonString);

            //jsonString = JSON.parse(jsonString);
        } catch (e) {
            jsonString = "err@json解析错误: " + jsonString;
            alert(jsonString);
            //  console.log(jsonString);
        }
        return jsonString;
    }
}



let jsonString;
const Entity = function (enName, pkval) {

    if (enName == null || enName == "" || enName == undefined) {
        alert('enName不能为空');
        throw Error('enName不能为空');
    }

    this.enName = enName;

    if (pkval != null && typeof pkval === "object") {
        jsonString = {};
        this.CopyJSON(pkval);
    } else {
        this.pkval = pkval || "";
        this.loadData();
    }

};

function setData(self) {
    if (typeof jsonString !== "undefined") {
        $.each(jsonString, function (n, o) {
            // 需要判断属性名与当前对象属性名是否相同
            if (typeof self[n] !== "function") {
                self[n] = o;
            }
        });
    }
}

function getParams(self) {
    let params = {};
    $.each(jsonString, function (n) {
        if (typeof self[n] !== "function") {
            params[n] = self[n];
        }
    });
    return params;
}

function getParams1(self) {

    let params = ["t=" + new Date().getTime()];
    $.each(jsonString, function (n, o) {

        if (typeof self[n] !== "function" && (self[n] != o)) {

            if (self[n] != undefined && self[n].toString().indexOf('<script') != -1)
                params.push(n + "=aa");
            else
                params.push(n + "=" + self[n]);

        }
    });
    return params.join("&");
}



dynamicHandler = process.env.VUE_APP_HANDLER;

Entity.prototype = {

    constructor: Entity,

    loadData: function () {
        let self = this;
        if (dynamicHandler == "")
            return;
        $.ajax({
            type: 'post',
            async: false,
            xhrFields: {
                withCredentials: true
            },
            crossDomain: true,
            url: dynamicHandler + "?DoType=Entity_Init&EnName=" + self.enName + "&PKVal=" + self.pkval + "&t=" + new Date().getTime(),
            dataType: 'html',
            success: function (data) {

                if (data.indexOf("err@") != -1) {
                    data = data.replace('@@', '@');
                    alert(data);
                    throw new Error(data);
                }

                if (data == "")
                    return;

                try {
                    jsonString = JSON.parse(data);
                    setData(self);
                } catch (e) {
                    alert("解析错误: " + data);
                }
            },
            error: function (XMLHttpRequest) {
                alert("Entity_Init 系统发生异常, status: " + XMLHttpRequest.status + " readyState: " + XMLHttpRequest.readyState + " enName=" + self.enName + " pkval=" + self.pkval);
            }
        });
    },

    SetValByKey: function (key, value) {
        this[key] = value;
    },

    GetValByKey: function (key) {
        return this[key];
    },

    Insert: function () {
        if (dynamicHandler == "")
            return;

        let self = this;
        let params = getParams(self);

        if (params.length == 0)
            params = getParams1(self);

        let result = "";

        $.ajax({
            type: 'post',
            async: false,
            xhrFields: {
                withCredentials: true
            },
            crossDomain: true,
            url: dynamicHandler + "?DoType=Entity_Insert&EnName=" + self.enName + "&t=" + new Date().getTime(),
            dataType: 'html',
            data: params,
            success: function (data) {

                result = data;
                if (data.indexOf("err@") != -1) {
                    alert(data);
                    return 0; //插入失败.
                }

                data = JSON.parse(data);
                result = data;

                let self = this;
                $.each(data, function (n, o) {
                    if (typeof self[n] !== "function") {
                        jsonString[n] = o;
                        self[n] = o;
                    }
                });

            },
            error: function (XMLHttpRequest) {
                alert("系统发生异常, status: " + XMLHttpRequest.status + " readyState: " + XMLHttpRequest.readyState);
            }
        });
        return result;
    },
    DirectInsert: function () {
        if (dynamicHandler == "")
            return;

        let self = this;
        let params = getParams(self);

        if (params.length == 0)
            params = getParams1(self);

        let result = "";

        $.ajax({
            type: 'post',
            async: false,
            xhrFields: {
                withCredentials: true
            },
            crossDomain: true,
            url: dynamicHandler + "?DoType=Entity_DirectInsert&EnName=" + self.enName + "&t=" + new Date().getTime(),
            dataType: 'html',
            data: params,
            success: function (data) {

                result = data;
                if (data.indexOf("err@") != -1) {
                    alert(data);
                    return 0; //插入失败.
                }

                data = JSON.parse(data);
                result = data;

                var self = this;
                $.each(data, function (n, o) {
                    if (typeof self[n] !== "function") {
                        jsonString[n] = o;
                        self[n] = o;
                    }
                });

            },
            error: function (XMLHttpRequest) {
                alert("系统发生异常, status: " + XMLHttpRequest.status + " readyState: " + XMLHttpRequest.readyState);
            }
        });
        return result;
    },

    Update: function () {
        if (dynamicHandler == "")
            return;

        let self = this;
        let params = getParams(self);
        let result;

        $.ajax({
            type: 'post',
            async: false,
            xhrFields: {
                withCredentials: true
            },
            crossDomain: true,
            url: dynamicHandler + "?DoType=Entity_Update&EnName=" + self.enName + "&t=" + new Date().getTime(),
            dataType: 'html',
            data: params,
            success: function (data) {
                result = data;
                if (data.indexOf("err@") != -1) {
                    var err = data.replace('err@', '');
                    this.$message('更新异常:' + err + " \t\nEnName" + self.enName);
                    return 0;
                }

                $.each(params, function (n, o) {
                    jsonString[n] = o;
                });
            },
            error: function (XMLHttpRequest) {
                alert("Entity Update系统发生异常, status: " + XMLHttpRequest.status + " readyState: " + XMLHttpRequest.readyState);
            }
        });
        return result;
    },

    Save: function () {
        if (dynamicHandler == "")
            return;

        let self = this;
        let params = getParams(self);
        let result;

        $.ajax({
            type: 'post',
            async: false,
            xhrFields: {
                withCredentials: true
            },
            crossDomain: true,
            url: dynamicHandler + "?DoType=Entity_Save&EnName=" + self.enName + "&t=" + new Date().getTime(),
            dataType: 'html',
            data: params,
            success: function (data) {
                result = data;
                if (data.indexOf("err@") != -1) {
                    alert(data);
                    return;
                }
                $.each(params, function (n, o) {
                    jsonString[n] = o;
                });
            },
            error: function (XMLHttpRequest) {
                alert("Save 系统发生异常, status: " + XMLHttpRequest.status + " readyState: " + XMLHttpRequest.readyState);
            }
        });
        return result;
    },

    Delete: function (key1, val1, key2, val2) {
        if (dynamicHandler == "")
            return;
        let self = this;
        //var params = getParams(self);
        let params = getParams1(this);


        let result;

        $.ajax({
            type: 'post',
            async: false,
            xhrFields: {
                withCredentials: true
            },
            crossDomain: true,
            url: dynamicHandler + "?DoType=Entity_Delete&EnName=" + self.enName + "&PKVal=" + this.GetPKVal() + "&Key1=" + key1 + "&Val1=" + val1 + "&Key2=" + key2 + "&Val2=" + val2 + "&t=" + new Date().getTime(),
            dataType: 'html',
            data: params,
            success: function (data) {
                result = data;
                if (data.indexOf("err@") != -1) {
                    alert(data);
                    return;
                }
                $.each(jsonString, function (n) {
                    jsonString[n] = undefined;
                });
                setData(self);
            },
            error: function (XMLHttpRequest) {
                alert("Delete 系统发生异常, status: " + XMLHttpRequest.status + " readyState: " + XMLHttpRequest.readyState);
            }
        });
        return result;
    },

    Retrieve: function () {
        if (dynamicHandler == "")
            return;

        let self = this;
        let params = getParams1(this);
        let result;
        $.ajax({
            type: 'post',
            async: false,
            xhrFields: {
                withCredentials: true
            },
            crossDomain: true,
            url: dynamicHandler + "?DoType=Entity_Retrieve&EnName=" + self.enName + "&" + params,
            dataType: 'html',
            success: function (data) {
                result = data;
                if (data.indexOf("err@") == 0) {
                    alert('查询失败:' + self.enName + "请联系管理员:\t\n" + data.replace('err@', ''));
                    return;
                }

                try {
                    jsonString = JSON.parse(data);
                    setData(self);
                    result = jsonString.Retrieve;

                } catch (e) {
                    result = "err@解析错误: " + data;
                    alert(result);
                }
            },
            error: function (textStatus) {
                const url = dynamicHandler + "?DoType=Entity_Retrieve&EnName=" + self.enName + "&" + params;
                ThrowMakeErrInfo("Retrieve-" + self.enName, textStatus, url);
            }
        });
        return result;
    },
    SetPKVal: function (pkVal) {

        self.pkval = pkVal;
        this["MyPK"] = self.pkval;
        this["OID"] = self.pkval;
        this["WorkID"] = self.pkval;
        this["NodeID"] = self.pkval;
        this["No"] = self.pkval;

        if (jsonString != null) {
            jsonString["MyPK"] = self.pkval;
            jsonString["OID"] = self.pkval;
            jsonString["WorkID"] = self.pkval;
            jsonString["NodeID"] = self.pkval;
            jsonString["No"] = self.pkval;
        }

    },
    GetPKVal: function () {

        let val = null;


        if (jsonString != null) {
            val = jsonString["MyPK"];
            if (val == undefined || val == "")
                val = jsonString["OID"];
            if (val == undefined || val == "")
                val = jsonString["WorkID"];
            if (val == undefined || val == "")
                val = jsonString["NodeID"];
            if (val == undefined || val == "")
                val = jsonString["No"];
            if (val == undefined || val == "")
                val = this.pkval;

            if (val != null && val != undefined && val != "")
                return val;

        }

        if (self != null) {
            val = self["MyPK"];
            if (val == undefined || val == "")
                val = self["OID"];
            if (val == undefined || val == "")
                val = self["WorkID"];
            if (val == undefined || val == "")
                val = self["NodeID"];
            if (val == undefined || val == "")
                val = self["No"];
            if (val == undefined || val == "")
                val = this.pkval;

            if (val != null && val != undefined && val != "")
                return val;
        }

        if (val == undefined || val == "")
            val = this["MyPK"];
        if (val == undefined || val == "")
            val = this["OID"];
        if (val == undefined || val == "")
            val = this["WorkID"];
        if (val == undefined || val == "")
            val = this["NodeID"];
        if (val == undefined || val == "")
            val = this["No"];
        if (val == undefined || val == "")
            val = this.pkval;

        return val;
    },
    RetrieveFromDBSources: function () {
        if (dynamicHandler == "")
            return;
        let self = this;

        const pkavl = this.GetPKVal();

        if (pkavl == null || pkavl == "") {
            alert('[' + this.enName + ']没有给主键赋值无法执行查询.');
            return;
        }

        let result;
        $.ajax({
            type: 'post',
            async: false,
            xhrFields: {
                withCredentials: true
            },
            crossDomain: true,
            url: dynamicHandler + "?DoType=Entity_RetrieveFromDBSources&EnName=" + self.enName + "&PKVal=" + pkavl,
            dataType: 'html',
            success: function (data) {
                result = data;
                if (data.indexOf("err@") == 0) {
                    alert(data);
                    return;
                }
                if (data == "")
                    return 0;
                try {
                    jsonString = JSON.parse(data);
                    setData(self);
                    result = jsonString.RetrieveFromDBSources;

                } catch (e) {
                    result = "err@解析错误: " + data;
                    alert(result);
                }
            },
            error: function (textStatus) {
                const url = dynamicHandler + "?DoType=Entity_RetrieveFromDBSources&EnName=" + self.enName + "&PKVal=" + pkavl;
                ThrowMakeErrInfo("Entity_RetrieveFromDBSources-" + self.enName + " pkval=" + pkavl, textStatus, url);

                //alert(JSON.stringify(XMLHttpRequest));
                //result = "RetrieveFromDBSources err@系统发生异常, status: " + XMLHttpRequest.status + " readyState: " + XMLHttpRequest.readyState;
                //alert(result);
            }
        });
        return result;
    },

    IsExits: function () {
        if (dynamicHandler == "")
            return;
        let self = this;
        let result;

        $.ajax({
            type: 'post',
            async: false,
            xhrFields: {
                withCredentials: true
            },
            crossDomain: true,
            url: dynamicHandler + "?DoType=Entity_IsExits&EnName=" + self.enName + "&" + getParams1(self),
            dataType: 'html',
            success: function (data) {

                if (data.indexOf("err@") != -1) {
                    alert(data);
                    return;
                }

                if (data == "1")
                    result = true;
                else
                    result = false;
            },
            error: function (textStatus) {
                ThrowMakeErrInfo("Entity_IsExits-" + self.enName, textStatus);
            }
        });
        return result;
    },   //一个参数直接传递,  多个参数，参数之间使用 ~隔开， 比如: zhangsna~123~1~山东济南.
    DoMethodReturnString: function (methodName, myparams) {
        if (dynamicHandler == "")
            return;
        let params = "";
        if (myparams == null || myparams == undefined)
            myparams = "";

        $.each(arguments, function (i, o) {
            if (i != 0)
                params += o + "~";
        });
        if (params.lastIndexOf("~") == params.length - 1)
            params = params.substr(0, params.length - 1);
        arguments["paras"] = params;


        const pkval = this.GetPKVal();
        if (pkval == null || pkval == "") {
            alert('[' + this.enName + ']没有给主键赋值无法执行查询.');
            return;
        }

        let self = this;
        let string;
        $.ajax({
            type: 'post',
            async: false,
            xhrFields: {
                withCredentials: true
            },
            crossDomain: true,
            url: dynamicHandler + "?DoType=Entity_DoMethodReturnString&EnName=" + self.enName + "&PKVal=" + pkval + "&MethodName=" + methodName + "&t=" + new Date().getTime(),
            dataType: 'html',
            data: arguments,
            success: function (data) {
                console.log(`🚀 :: data`, data);
                string = data;
            },
            error: function (textStatus) {
                console.log(`🚀 :: textStatus`, textStatus);
                var url = dynamicHandler + "?DoType=Entity_DoMethodReturnString&EnName=" + self.enName + "&PKVal=" + pkval + "&MethodName=" + methodName + "&t=" + new Date().getTime();
                ThrowMakeErrInfo("Entity_DoMethodReturnString-" + self.enName + " pkval=" + pkval + " MethodName=" + methodName, textStatus, url);

                //    string = "Entity.DoMethodReturnString err@系统发生异常, status: " + XMLHttpRequest.status + " readyState: " + XMLHttpRequest.readyState;
                //  alert(string);
            }
        });

        return string;

    },

    DoMethodReturnJSON: function (methodName, params) {

        let jsonString = this.DoMethodReturnString(methodName, params);

        if (jsonString.indexOf("err@") != -1) {
            alert(jsonString);
            return jsonString;
        }

        try {

            jsonString = ToJson(jsonString);

            //jsonString = JSON.parse(jsonString);
        } catch (e) {
            jsonString = "err@json解析错误: " + jsonString;
            alert(jsonString);
        }
        return jsonString;
    },

    toString: function () {
        return JSON.stringify(this);
    },

    GetPara: function (key) {
        let atPara = this.AtPara;
        if (typeof atPara != "string" || typeof key == "undefined" || key == "") {
            return undefined;
        }
        let reg = new RegExp("(^|@)" + key + "=([^@]*)(@|$)");
        let results = atPara.match(reg);
        if (results != null) {
            return unescape(results[2]);
        }
        return undefined;
    },

    SetPara: function (key, value) {
        let atPara = this.AtPara;
        if (typeof atPara != "string" || typeof key == "undefined" || key == "") {
            return;
        }

        let m = "@" + key + "=";
        let index = atPara.indexOf(m);
        if (index == -1) {
            this.AtPara += "@" + key + "=" + value;
            return;
        }

        const p = atPara.substring(0, index + m.length);
        const s = atPara.substring(index + m.length, atPara.length);
        const i = s.indexOf("@");
        if (i == -1) {
            this.AtPara = p + value;
        } else {
            this.AtPara = p + value + s.substring(i, s.length);
        }

    },

    CopyURL: function () {
        let self = this;
        $.each(self, function (n, o) {
            if (typeof o !== "function") {
                var value = GetQueryString(n);
                if (value != null && typeof value !== "undefined" && $.trim(value) != "") {
                    self[n] = value;
                    jsonString[n] = value;
                }
            }
        });
    },

    CopyForm: function () {

        $("input,select").each(function (i, e) {
            if (typeof $(e).attr("name") === "undefined" || $(e).attr("name") == "") {
                $(e).attr("name", $(e).attr("id"));
            }
        });

        // 新版本20180107 2130
        let self = this;
        // 普通属性
        $("[name^=TB_],[name^=CB_],[name^=RB_],[name^=DDL_]").each(function () {
            var target = $(this);
            var name = target.attr("name");
            var key = name.replace(/^TB_|CB_|RB_|DDL_/, "");
            if (typeof self[key] === "function") {
                return true;
            }
            if (name.match(/^TB_/)) {
                self[key] = target.val();
            } else if (name.match(/^DDL_/)) {
                self[key] = target.val();
            } else if (name.match(/^CB_/)) {
                if (target.length == 1) {	// 仅一个复选框
                    if (target.is(":checked")) {
                        // 已选
                        self[key] = "1";
                    } else {
                        // 未选
                        self[key] = "0";
                    }
                } else if (target.length > 1) {	// 多个复选框(待扩展)
                    // ?
                }
            } else if (name.match(/^RB_/)) {

                if (target.is(":checked")) {
                    // 已选
                    self[key] = "1";
                } else {
                    // 未选
                    self[key] = "0";
                }
            }
        });
        //获取树形结构的表单值
        let combotrees = $(".easyui-combotree");
        $.each(combotrees, function (i, combotree) {
            var name = $(combotree).attr('id');
            var tree = $('#' + name).combotree('tree');
            //获取当前选中的节点
            var data = tree.tree('getSelected');
            if (data != null) {
                self[name.replace("DDL_", "")] = data.id;
                self[name.replace("DDL_", "") + "T"] = data.text;
            }
        });
        // 参数属性
        $("[name^=TBPara_],[name^=CBPara_],[name^=RBPara_],[name^=DDLPara_]").each(function () {
            var target = $(this);
            var name = target.attr("name");
            var value;
            if (name.match(/^TBPara_/)) {
                value = target.val();
                value = value.replace('@', ''); //替换掉@符号.
            } else if (name.match(/^DDLPara_/)) {
                value = target.val();
                value = value.replace('@', ''); //替换掉@符号.
            } else if (name.match(/^CBPara_/)) {
                if (target.length == 1) {	// 仅一个复选框
                    if (target.is(":checked")) {
                        // 已选
                        value = "1";
                    } else {
                        // 未选
                        value = "0";
                    }
                } else if (target.length > 1) {	// 多个复选框(待扩展)
                    // ?
                }
            } else if (name.match(/^RBPara_/)) {
                if (target.is(":checked")) {
                    // 已选
                    value = "1";
                } else {
                    // 未选
                    value = "0";
                }
            }
            var key = name.replace(/^TBPara_|CBPara_|RBPara_|DDLPara_/, "");
            self.SetPara(key, value);
        });
    },

    CopyJSON: function (json) {
        let count = 0;
        if (json) {
            var self = this;
            $.each(json, function (n, o) {
                if (typeof self[n] !== "function") {

                    if (n == 'enName' || n == 'MyPK')
                        return;

                    self[n] = o;
                    jsonString[n] = o;
                    count++;
                }
            });
        }
        return count;
    },

    ToJsonWithParas: function () {
        let json = {};
        $.each(this, function (n, o) {
            if (typeof o !== "undefined") {
                json[n] = o;
            }
        });
        if (typeof this.AtPara == "string") {
            $.each(this.AtPara.split("@"), function (i, o) {
                if (o == "") {
                    return true;
                }
                const kv = o.split("=");
                if (kv.length == 2) {
                    json[kv[0]] = kv[1];
                }
            });
        }
        return json;
    }

};



const Entities = function (ensName) {
    this.ensName = ensName;
    this.Paras = this.getParameters(arguments);
    if (arguments.length >= 3) {
        this.loadData();
    }
}
Entities.prototype = {
    constructor: Entities,
    getParameters: function (args, divisor) {
        let params = "";
        let length;
        let orderBy;
        if (divisor == null || divisor == undefined)
            divisor = 2;

        if (divisor == 2) {
            if (args.length % 2 == 0) {
                orderBy = args[args.length - 1];
                length = args.length - 1;
            } else {
                length = args.length;
            }
            for (let i = 1; i < length; i += 2) {
                params += "@" + args[i] + "=" + args[i + 1];
            }
            if (typeof orderBy !== "undefined") {
                params += "@OrderBy=" + orderBy;
            }
            return params;
        }

        if (divisor == 3) {
            if ((args.length - 1) % divisor != 0) {
                orderBy = args[args.length - 1];
                length = args.length - 1;
            } else {
                length = args.length;
            }
            // eslint-disable-next-line no-redeclare
            for (let i = 1; i < length; i += 3) { //args[i+1]是操作符
                params += "@" + args[i] + "|" + args[i + 1] + "|" + args[i + 2];
            }
            if (typeof orderBy !== "undefined") {
                params += "@OrderBy||" + orderBy;
            }
            return params;
        }

    },
    loadData: function () {
        let jsonString;
        if (dynamicHandler == "")
            return;
        let self = this;

        if (self.ensName == null || self.ensName == "" || self.ensName == "") {
            alert("在初始化实体期间EnsName没有赋值");
            return;
        }

        $.ajax({
            type: 'post',
            async: false,
            xhrFields: {
                withCredentials: true
            },
            crossDomain: true,
            url: dynamicHandler + "?DoType=Entities_Init&EnsName=" + self.ensName + "&Paras=" + self.Paras + "&t=" + new Date().getTime(),
            dataType: 'html',
            success: function (data) {

                if (data.indexOf("err@") != -1) {
                    data = data.replace('err@', '');
                    data += "\t\n参数信息:";
                    data += "\t\nDoType=Entities_Init";
                    // eslint-disable-next-line no-useless-escape
                    data += "\t\EnsName=" + self.ensName;
                    // eslint-disable-next-line no-useless-escape
                    data += "\t\Paras=" + self.Paras;
                    alert(data);
                    return;
                }

                try {
                    jsonString = JSON.parse(data);
                    if ($.isArray(jsonString)) {
                        self.length = jsonString.length;
                        $.extend(self, jsonString);
                    } else {
                        alert("解析失败, 返回值不是集合");
                    }
                } catch (e) {
                    alert("json解析错误: " + data);
                }
            },
            error: function (XMLHttpRequest, textStatus) {
                ThrowMakeErrInfo("Entities_Init-" + self.ensName, textStatus);

            }
        });
    },
    deleteIt: function () {
        if (dynamicHandler == "")
            return;
        let self = this;
        if (self.ensName == null || self.ensName == "" || self.ensName == "") {
            alert("在初始化实体期间EnsName没有赋值");
            return;
        }

        $.ajax({
            type: 'post',
            async: false,
            xhrFields: {
                withCredentials: true
            },
            crossDomain: true,
            url: dynamicHandler + "?DoType=Entities_Delete&EnsName=" + self.ensName + "&Paras=" + self.Paras + "&t=" + new Date().getTime(),
            dataType: 'html',
            success: function (data) {
                if (data.indexOf("err@") != -1) {
                    alert(data);
                    return;
                }


            },
            error: function (XMLHttpRequest, textStatus) {

                ThrowMakeErrInfo("Entities_Delte-" + self.ensName, textStatus);

            }
        });
    },

    Retrieve: function () {
        let args = [""];
        $.each(arguments, function (i, o) {
            args.push(o);
        });
        this.Paras = this.getParameters(args);
        this.loadData();
    },
    RetrieveCond: function () {
        let jsonString;
        if (dynamicHandler == "")
            return;
        let args = [""];
        $.each(arguments, function (i, o) {
            args.push(o);
        });
        this.Paras = this.getParameters(args, 3);
        let self = this;

        if (self.ensName == null || self.ensName == "" || self.ensName == "") {
            alert("在初始化实体期间EnsName没有赋值");
            return;
        }

        $.ajax({
            type: 'post',
            async: false,
            xhrFields: {
                withCredentials: true
            },
            crossDomain: true,
            url: dynamicHandler + "?DoType=Entities_RetrieveCond&EnsName=" + self.ensName + "&Paras=" + self.Paras + "&t=" + new Date().getTime(),
            dataType: 'html',
            success: function (data) {

                if (data.indexOf("err@") != -1) {
                    alert(data);
                    return;
                }

                try {
                    jsonString = JSON.parse(data);
                    if ($.isArray(jsonString)) {
                        self.length = jsonString.length;
                        $.extend(self, jsonString);
                    } else {
                        alert("解析失败, 返回值不是集合");
                    }
                } catch (e) {
                    alert("json解析错误: " + data);
                }
            },
            error: function (XMLHttpRequest, textStatus) {

                ThrowMakeErrInfo("Entities_RetrieveCond-" + self.ensName, textStatus);
            }
        });

    },
    Delete: function () {
        let args = [""];
        $.each(arguments, function (i, o) {
            args.push(o);
        });
        this.Paras = this.getParameters(args);

        this.deleteIt();
    },
    DoMethodReturnString: function (methodName) {
        if (dynamicHandler == "")
            return;
        let params = "";
        $.each(arguments, function (i, o) {
            if (i != 0)
                params += o + "~";
        });

        params = params.substr(0, params.length - 1);

        let self = this;
        let string;
        $.ajax({
            type: 'post',
            async: false,
            xhrFields: {
                withCredentials: true
            },
            crossDomain: true,
            url: dynamicHandler + "?DoType=Entities_DoMethodReturnString&EnsName=" + self.ensName + "&MethodName=" + methodName + "&paras=" + params + "&t=" + new Date().getTime(),
            dataType: 'html',
            success: function (data) {
                string = data;
            },
            error: function (XMLHttpRequest, textStatus) {
                ThrowMakeErrInfo("Entities_DoMethodReturnString-" + methodName, textStatus);
            }
        });

        return string;

    },
    GetEns: function () {
        let result = [];
        for (let key in this) {
            if (typeof this[key] === 'object') {
                result.push(this[key]);
            }
        }
        this.data = result;
        return this;
    },

    DoMethodReturnJSON: function (methodName, params) {
        let jsonString = this.DoMethodReturnString(methodName, params);
        if (jsonString.indexOf("err@") != -1) {
            alert(jsonString);
            return jsonString;
        }
        try {
            jsonString = ToJson(jsonString);
        } catch (e) {
            jsonString = "err@json解析错误: " + jsonString;
            alert(jsonString);
        }
        return jsonString;
    },
    RetrieveAll: function () {
        let jsonString;
        if (dynamicHandler == "")
            return;
        let pathRe = "";
        let self = this;
        $.ajax({
            type: 'post',
            async: false,
            xhrFields: {
                withCredentials: true
            },
            crossDomain: true,
            url: pathRe + dynamicHandler + "?DoType=Entities_RetrieveAll&EnsName=" + self.ensName + "&t=" + new Date().getTime(),
            dataType: 'html',
            success: function (data) {
                if (data.indexOf("err@") != -1) {
                    alert(data);
                    return;
                }
                try {

                    jsonString = ToJson(data);

                    if ($.isArray(jsonString)) {
                        self.length = jsonString.length;
                        $.extend(self, jsonString);
                    } else {
                        alert("解析失败, 返回值不是集合");
                    }
                } catch (e) {
                    alert("json解析错误: " + data);
                }
            },
            error: function (XMLHttpRequest, textStatus) {

                ThrowMakeErrInfo("Entities_RetrieveAll-", textStatus);

            }
        });
    }

};
const WebUser = function () {
    if (dynamicHandler == "")
        return;
    let json = {};
    dynamicHandler = process.env.VUE_APP_HANDLER;

    $.ajax({
        type: 'post',
        async: false,
        xhrFields: {
            withCredentials: true
        },
        crossDomain: true,
        url: dynamicHandler + "?DoType=WebUser_Init&t=" + new Date().getTime(),
        dataType: 'html',
        success: function (data) {

            if (data.indexOf("err@") != -1) {
                if (data.indexOf('登录信息丢失') != -1) {
                    alert("登录信息丢失，请重新登录。");
                } else {
                    alert(data);
                }
                return;
            }
            try {
                json = JSON.parse(data);
            } catch (e) {
                alert("json解析错误: " + data);
            }
        },
        error: function (XMLHttpRequest, textStatus) {
            const url = dynamicHandler + "?DoType=WebUser_Init&t=" + new Date().getTime();
            ThrowMakeErrInfo("WebUser-WebUser_Init", textStatus, url);
        }
    });
    let self = this;
    $.each(json, function (n, o) {
        self[n] = o;
    });

};
function DBAccess() {
}

dynamicHandler = process.env.VUE_APP_HANDLER;

DBAccess.RunSQL = function (sql) {
    if (dynamicHandler == "")
        return;
    let count = 0;
    sql = sql.replace(/'/g, '~');
    $.ajax({
        type: 'post',
        async: false,
        xhrFields: {
            withCredentials: true
        },
        crossDomain: true,
        url: dynamicHandler + "?DoType=DBAccess_RunSQL&t=" + new Date().getTime(),
        dataType: 'html',
        data: { "SQL": sql },
        success: function (data) {
            count = parseInt(data);
            if (isNaN(count)) {
                count = -1;
            }
        },
        error: function (XMLHttpRequest, textStatus) {
            ThrowMakeErrInfo("DBAccess_RunSQL-", textStatus);
        }
    });

    return count;

};
//执行数据源返回json.
DBAccess.RunDBSrc = function (dbSrc, dbType) {

    if (dbSrc == "" || dbSrc == null || dbSrc == undefined) {
        alert("数据源为空..");
        return;
    }

    if (dbType == undefined) {
        dbType = 0; //默认为sql.

        if (dbSrc.length <= 20) {
            dbType = 2; //可能是一个方法名称.
        }

        if (dbSrc.indexOf('/') != -1) {
            dbType = 1; //是一个url.
        }
    }
    //执行的SQL
    if (dbType == 0) {
        return DBAccess.RunSQLReturnTable(dbSrc);
    }

    //执行URL
    if (dbType == 1 || dbType == "1") {
        return DBAccess.RunUrlReturnJSON(dbSrc);
    }

    //执行方法名称返回json.
    if (dbType == 2 || dbType == "2") {

        var str = DBAccess.RunFunctionReturnStr(dbSrc);
        if (str == null || str == undefined || str == "")
            return null;

        return JSON.parse(str);
    }
};

//执行方法名返回str.
DBAccess.RunFunctionReturnStr = function (funcName) {

    try {
        funcName = funcName.replace(/~/g, "'");
        if (funcName.indexOf('(') == -1)
            return eval(funcName + "()");
        else
            return eval(funcName);

    } catch (e) {
        if (e.message)
            alert("执行方法[" + funcName + "]错误:" + e.message);
    }
};

//执行方法名返回str.
DBAccess.RunSQLReturnVal = function (sql) {
    let dt = DBAccess.RunSQLReturnTable(sql);
    if (dt.length == 0)
        return null;
    let firItem = dt[0];
    let firAttr = "";
    for (let k in firItem) {
        firAttr = k;
        break;
    }
    return firItem[firAttr];
};

DBAccess.RunSQLReturnTable = function (sql) {
    if (dynamicHandler == "")
        return;
    sql = sql.replace(/~/g, "'");
    sql = sql.replace(/[+]/g, "/#");
    sql = sql.replace(/-/g, '/$');
    let jsonString;

    $.ajax({
        type: 'post',
        async: false,
        xhrFields: {
            withCredentials: true
        },
        crossDomain: true,
        url: dynamicHandler + "?DoType=DBAccess_RunSQLReturnTable" + "&t=" + new Date().getTime(),
        dataType: 'html',
        data: { "SQL": sql },
        success: function (data) {
            if (data.indexOf("err@") != -1) {
                alert(data);
                return;
            }
            try {
                jsonString = JSON.parse(data);
            } catch (e) {
                alert("json解析错误: " + data);
            }
        },
        error: function (XMLHttpRequest, textStatus) {
            ThrowMakeErrInfo("DBAccess_RunSQLReturnTable-", textStatus);
        }
    });
    return jsonString;
};

DBAccess.RunUrlReturnString = function (url) {
    if (dynamicHandler == "")
        return;
    if (url == null || typeof url === "undefined") {
        alert("err@url无效");
        return;
    }
    if (url.match(/^http:\/\//)) {
        url = dynamicHandler + "?DoType=RunUrlCrossReturnString&t=" + new Date().getTime() + "&url=" + url
    }

    let string;

    $.ajax({
        type: 'post',
        async: false,
        url: url,
        dataType: 'html',
        xhrFields: {
            withCredentials: true
        },
        crossDomain: true,
        success: function (data) {
            if (data.indexOf("err@") != -1) {
                alert(data);
                return;
            }
            string = data;
        },
        error: function (XMLHttpRequest, textStatus) {
            alert(url);
            ThrowMakeErrInfo("HttpHandler-RunUrlCrossReturnString-", textStatus);
        }
    });

    return string;
};

DBAccess.RunUrlReturnJSON = function (url) {

    let jsonString = DBAccess.RunUrlReturnString(url);
    if (typeof jsonString === "undefined") {
        alert("执行错误:\t\n URL:" + url);
        return;
    }
    if (jsonString.indexOf("err@") != -1) {
        alert(jsonString + "\t\n URL:" + url);
        return jsonString;
    }
    try {
        jsonString = JSON.parse(jsonString);
    } catch (e) {
        jsonString = "err@json,RunUrlReturnJSON解析错误:" + jsonString;
        alert(jsonString);
    }
    return jsonString;
};
/* 把一个 @XB=1@Age=25 转化成一个js对象.  */
function AtParaToJson(json) {
    const jsObj = {};
    if (json) {
        const atParamArr = json.split('@');
        $.each(atParamArr, function (i, atParam) {
            if (atParam != '') {
                const atParamKeyValue = atParam.split('=');
                if (atParamKeyValue.length == 2) {
                    jsObj[atParamKeyValue[0]] = atParamKeyValue[1];
                }
            }
        });
    }
    return jsObj;
}

/**
 * 根据AtPara例如AtPara=@Helpurl=XXX@Count=XXX,获取HelpUrl的值
 * @param atPara
 * @param key
 * @returns {undefined|string}
 * @constructor
 */
const GetPara = function (atPara, key) {
    if (typeof atPara != "string" || typeof key == "undefined" || key == "") {
        return undefined;
    }
    const reg = new RegExp("(^|@)" + key + "=([^@]*)(@|$)");
    let results = atPara.match(reg);
    if (results != null) {
        return unescape(results[2]);
    }
    return undefined;

}

/**
 * 把URL转换成JSON格式
 * @param urlStr
 * @returns {{}|{PageName: string}}
 */
function decodeResponseParams(urlStr) {
    try {
        const obj = {};
        const url = urlStr.trim().replace('url@', '');
        const args = url.split('?');
        //获取到页面名称
        let pageName = args[0].substring(args[0].lastIndexOf('/') + 1) || '';
        pageName = pageName.replace('.htm', '').replace('.html', '').replace('.vue', '');
        if (args.length < 2 || !args[1].trim()) {
            return { PageName: pageName };
        }
        obj['PageName'] = pageName;
        args[1].split('&').forEach((arg) => {
            const [key, val] = arg.split('=');
            obj[key] = val;
        });
        return obj;
    } catch (e) {
        return {};
    }
}

/**
 * 执行url
 * @param url
 * @returns {*}
 * @constructor
 */
function RunUrlReturnString(url) {
    if (url == null || typeof url === "undefined") {
        alert("err@url无效");
        return;
    }
    url='api'+url;
    let str;
    $.ajax({
        type: 'post',
        async: false,
        url: url,
        dataType: 'html',
        success: function (data) {
            if (typeof data === 'string' && data.includes('err@url')) {
                str = data.replace('err@',''); //这个错误是合法的.
                return;
            }
            if (typeof data === 'string' && data.includes("err@")){
                alert(data);
                return;
            }
            str = data;
        },
        error: function () {
            if (confirm('系统异常:' + url + " 您想打开url查看吗？") == true) {
                window.open(url);
                str ="";
                return;
            }
        }
    });
    return str;
}
export {
    HttpHandler,
    Entity,
    Entities,
    WebUser,
    DBAccess,
    AtParaToJson,
    GetPara,
    decodeResponseParams,
    RunUrlReturnString,
};
