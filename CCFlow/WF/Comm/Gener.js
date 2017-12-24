



/* ESC Key Down */

function ToLoeo( dt ) {

return dt;
 
}

function Esc() {
    if (event.keyCode == 27)
        window.close();
    return true;
}


/* 把一个 @XB=1@Age=25 转化成一个js对象.  */
function AtParaToJson(json) {
    var jsObj = {};
    if (json) {
        var atParamArr = json.split('@');
        $.each(atParamArr, function (i,atParam) {
            if (atParam != '') {
                var  atParamKeyValue = atParam.split('=');
                if (atParamKeyValue.length == 2) {
                    jsObj[atParamKeyValue[0]] = atParamKeyValue[1];
                }
            }
        });
    }
    return jsObj;
}

//处理url，删除无效的参数.
function DearUrlParas(urlParam) {

    //如何获得全部的参数？ &FK_Node=120&FK_Flow=222 放入到url里面去？
    //var href = window.location.href;
    //var urlParam = href.substring(href.indexOf('?') + 1, href.length);

    if (urlParam==null || urlParam==undefined)
        urlParam = window.location.search.substring(1);

    var params = {};
    if (urlParam == "" && urlParam.length == 0) {
        urlParam = "1=1"
    } else {
        $.each(urlParam.split("&"), function (i, o) {
            if (o) {
                var param = o.split("=");
                if (param.length == 2) {
                    var key = param[0];
                    var value = param[1];

                    if (key == "DoType")
                        return true;

                    if (value == "null" || typeof value == "undefined")
                        return true;

                    if (value != null && typeof value != "undefined"
                            && value != "null"
                            && value != "undefined") {
                        value = value.trim();
                        if (value != "" && value.length > 0) {
                            if (typeof params[key] == "undefined") {
                                params[key] = value;
                            }
                        }
                    }
                }
            }
        });
    }
    urlParam = "";
    $.each(params, function (i, o) {
        urlParam += i + "=" + o + "&";
    });

    urlParam = urlParam.replace("&&", "&");
    urlParam = urlParam.replace("&&", "&");
    urlParam = urlParam.replace("&&", "&");
    return urlParam;
}

function GetRadioValue(groupName) {
    var obj;
    obj = document.getElementsByName(groupName);
    if (obj != null) {
        var i;
        for (i = 0; i < obj.length; i++) {
            if (obj[i].checked) {
                return obj[i].value;
            }
        }
    }
    return null;
}

//获得所有的checkbox 的id组成一个string用逗号分开, 以方便后台接受的值保存.
function GenerCheckIDs() {

    var checkBoxIDs = "";
    var arrObj = document.all;

    for (var i = 0; i < arrObj.length; i++) {

        if (arrObj[i].type != 'checkbox')
            continue;

        var cid = arrObj[i].name;
        if (cid == null || cid == "" || cid == '')
            continue;

        checkBoxIDs += arrObj[i].id + ',';
    }
    return checkBoxIDs;
}

//填充下拉框.
function GenerBindDDL(ddlCtrlID, data, noCol, nameCol, selectVal) {

    if (noCol == null)
        noCol = "No";

    if (nameCol == null)
        nameCol = "Name";

    //判断data是否是一个数组，如果是一个数组，就取第1个对象.
    var json = data;

    //如果他的数量==0，就return.
    if (json.length == 0)
        return;

    if (data[0].length == 1)
        json = data[0];

    // 清空默认值, 写一个循环把数据给值.
    $("#" + ddlCtrlID).empty();

    if (json[0][noCol] == undefined) {
        alert('@在绑定[' + ddlCtrlID + ']错误，No列名' + noCol + '不存在,无法行程期望的下拉框value . ');
        return;
    }

    if (json[0][nameCol] == undefined) {
        alert('@在绑定[' + ddlCtrlID + ']错误，Name列名' + nameCol + '不存在,无法行程期望的下拉框value. ');
        return;
    }

    for (var i = 0; i < json.length; i++) {
        $("#" + ddlCtrlID).append("<option value='" + json[i][noCol] + "'>" + json[i][nameCol] + "</option>");
    }

    //设置选中的值.
    if (selectVal != undefined ) {

        var v = $("#" + ddlCtrlID)[0].options.length;
        if (v == 0)
            return;

        $("#" + ddlCtrlID).val(selectVal);

        var v = $("#" + ddlCtrlID).val();
        if (v == null) {
            $("#" + ddlCtrlID)[0].options[0].selected = true;
        }
    }
}

/*绑定枚举值.*/
function GenerBindEnumKey(ctrlDDLId, enumKey, selectVal) {

    $.ajax({
        type: 'post',
        async: true,
        url: "/WF/Comm/Handler.ashx?DoType=EnumList&EnumKey=" + enumKey + "&m=" + Math.random(),
        dataType: 'html',
        success: function (data) {

            data = JSON.parse(data);
            //绑定枚举值.
            GenerBindDDL(ctrlDDLId, data, "IntKey", "Lab", selectVal);
            return;
        }
    });
}


/* 绑定枚举值外键表.*/
function GenerBindEntities(ctrlDDLId, ensName, selectVal, filter) {

    $.ajax({
        type: 'post',
        async: true,
        url: "/WF/Comm/Handler.ashx?DoType=EnsData&EnsName=" + ensName + "&Filter=" + filter + "&m=" + Math.random(),
        dataType: 'html',
        success: function (data) {

//            if (data.indexof('err@') ==0 )
//            {
//               alert(data);
//               return ;
//            }

            data = JSON.parse(data);
            //绑定枚举值.
            GenerBindDDL(ctrlDDLId, data, "No", "Name",selectVal);
            return;
        },
        error: function (jqXHR, textStatus, errorThrown) {
            /*错误信息处理*/
            alert("GenerBindEntities,错误:参数:EnsName"+ensName+" , 异常信息 responseText:"+jqXHR.responseText+"; status:"+jqXHR.status+"; statusText:"+jqXHR.statusText+"; \t\n textStatus="+textStatus+";errorThrown="+errorThrown);
        }
    });
}


/*
  绑定外键表.
*/
function GenerBindSFTable(ctrlDDLId, sfTable, selectVal) {

    $.ajax({
        type: 'post',
        async: true,
        url: "/WF/Comm/Handler.ashx?DoType=SFTable&SFTable=" + sfTable + "&m=" + Math.random(),
        dataType: 'html',
        success: function (data) {
            data = JSON.parse(data);
            //绑定枚举值.
            GenerBindDDL(ctrlDDLId, data, "No", "Name", selectVal);
            return;
        },
        error: function (jqXHR, textStatus, errorThrown) {
            /*错误信息处理*/
            alert("GenerBindSFTable,错误:参数:EnsName" + ensName + " , 异常信息 responseText:" + jqXHR.responseText + "; status:" + jqXHR.status + "; statusText:" + jqXHR.statusText + "; \t\n textStatus=" + textStatus + ";errorThrown=" + errorThrown);
        }
    });
}

/* 绑定SQL.
1. 调用这个方法，需要在 SQLList.xml 配置一个SQL , sqlKey 就是该sql的标记.
2, paras 就是向这个sql传递的参数, 比如： @FK_Mapdata=BAC@KeyOfEn=MyFild  .
*/
function GenerBindSQL(ctrlDDLId, sqlKey, paras, colNo, colName, selectVal) {

    if (colNo == null)
        colNo = "NO";
    if (colName == null)
        colName = "NAME";

    $.ajax({
        type: 'post',
        async: true,
        url: "/WF/Comm/Handler.ashx?DoType=SQLList&SQLKey=" + sqlKey + "&Paras=" + paras + "&m=" + Math.random(),
        dataType: 'html',
        success: function (data) {

            if (data.indexOf('err@') == 0) {
                alert(data);
            }

            data = JSON.parse(data);

            //绑定枚举值.
            GenerBindDDL(ctrlDDLId, data, colNo, colName, selectVal);

            return;
        }
    });
}

/*为页面的所有字段属性赋值. */
function GenerFullAllCtrlsVal(data) {

    //判断data是否是一个数组，如果是一个数组，就取第1个对象.
    var json = data;
    if (data.length == 1)
        json = data[0];

    var unSetCtrl = "";
    for (var attr in json) {

        var val = json[attr]; //值

        var div = document.getElementById(attr);
        if (div != null) {
            div.innerHTML = val;
            continue;
        }


        // textbox
        var tb = document.getElementById('TB_' + attr);
        if (tb != null) {
            if (tb.tagName.toLowerCase() != "input") {
                tb.innerHTML = val;
            }
            else {
                tb.value = val;
            }

            continue;
        }

        //checkbox.
        var cb = document.getElementById('CB_' + attr);
        if (cb != null) {
            if (val == "1")
                cb.checked = true;
            else
                cb.checked = false;
            continue;
        }

        //下拉框.
        var ddl = document.getElementById('DDL_' + attr);
        if (ddl != null) {

            if (ddl.options.length == 0)
                continue;

            $("#DDL_" + attr).val(val); // 操作权限.
            continue;
        }

        // RadioButton. 单选按钮.
        var rb = document.getElementById('RB_' + attr+"_"+val);
        if (rb != null) {
            rb.checked = true;
            continue;
        }

        // 处理参数字段.....................

        // textbox
        tb = document.getElementById('TBPara_' + attr);
        if (tb != null) {
            tb.value = val;
            continue;
        }

        //checkbox.
        cb = document.getElementById('CBPara_' + attr);
        if (cb != null) {
            if (val == "1")
                cb.checked = true;
            else
                cb.checked = false;
            continue;
        }

        //下拉框.
        ddl = document.getElementById('DDLPara_' + attr);
        if (ddl != null) {

            if (ddl.options.length == 0)
                continue;

            $("#DDL_" + attr).val(val); // 操作权限.
            continue;
        }

        // RadioButton. 单选按钮.
        rb = document.getElementById('RBPara_' + attr + "_" + val);
        if (rb != null) {
            rb.checked = true;
            continue;
        }

        unSetCtrl += "@" + attr + " = " + val;
    }
}


/*为页面的所有 div 属性赋值. */
function GenerFullAllDivVal(data) {

    //判断data是否是一个数组，如果是一个数组，就取第1个对象.
    var json = data;
    if (data.length == 1)
        json = data[0];

    var unSetCtrl = "";
    for (var attr in json) {

        var val = json[attr]; //值

        var div = document.getElementById(attr);

        if (div != null) {
            div.innerHTML = val;
            continue;
        }
    }

    // alert('没有找到的控件类型:' + unSetCtrl);
}

function DoCheckboxValue(frmData, cbId) {
    if (frmData.indexOf(cbId + "=") == -1) {
        frmData += "&" + cbId + "=0";
    }
    else {
        frmData.replace(cbId + '=on', cbId + '=1');
    }

    return frmData;
}


/*隐藏与显示.*/
function ShowHidden(ctrlID) {

    var ctrl = document.getElementById(ctrlID);
    if (ctrl.style.display == "block") {
        ctrl.style.display = 'none';
    } else {
        ctrl.style.display = 'block';
    }
}

function OpenDialogAndCloseRefresh(url, dlgTitle, dlgWidth, dlgHeight, dlgIcon, fnClosed) {
    ///<summary>使用EasyUiDialog打开一个页面，页面中嵌入iframe【id="eudlgframe"】</summary>
    ///<param name="url" type="String">页面链接</param>
    ///<param name="dlgTitle" type="String">Dialog标题</param>
    ///<param name="dlgWidth" type="int">Dialog宽度</param>
    ///<param name="dlgHeight" type="int">Dialog高度</param>
    ///<param name="dlgIcon" type="String">Dialog图标，必须是一个样式class</param>
    ///<param name="fnClosed" type="Function">窗体关闭调用的方法（注意：此方法中可以调用dialog中页面的内容；如此方法启用，则关闭窗体时的自动刷新功能会失效）</param>

    var dlg = $('#eudlg');
    var iframeId = "eudlgframe";

    if (dlg.length == 0) {
        var divDom = document.createElement('div');
        divDom.setAttribute('id', 'eudlg');
        document.body.appendChild(divDom);
        dlg = $('#eudlg');
        dlg.append("<iframe frameborder='0' src='' scrolling='auto' id='" + iframeId + "' style='width:100%;height:100%'></iframe>");
    }

    dlg.dialog({
        title: dlgTitle,
        left: document.body.clientWidth > dlgWidth ? (document.body.clientWidth - dlgWidth) / 2 : 0,
        top: document.body.clientHeight > dlgHeight ? (document.body.clientHeight - dlgHeight) / 2 : 0,
        width: dlgWidth,
        height: dlgHeight,
        iconCls: dlgIcon,
        resizable: true,
        modal: true,
        onClose: function () {
            if (fnClosed) {
                fnClosed();
                return;
            }

            Reload();
        },
        cache: false
    });
    
    dlg.dialog('open');
    $('#' + iframeId).attr('src', url);
}

function Reload() {
    ///<summary>重新加载当前页面</summary>
    var newurl = "";
    var urls = window.location.href.split('?');
    var params;

    if (urls.length == 1) {
        window.location.href = window.location.href + "?t=" + Math.random();
    }

    newurl = urls[0] + '?1=1';
    params = urls[1].split('&');

    for (var i = 0; i < params.length; i++) {
        if (params[i].indexOf("1=1") != -1 || params[i].toLowerCase().indexOf("t=") != -1) {
            continue;
        }

        newurl += "&" + params[i];
    }

    window.location.href = newurl + "&t=" + Math.random();
}

function ConvertDataTableFieldCase(dt, isLower) {
    ///<summary>转换datatable的json对象中的属性名称的大小写形式</summary>
    ///<param name="dt" type="Array">datatable json化后的[]数组</param>
    ///<param name="isLower" type="Boolean">是否转换成小写模式，默认转换成大写</param>
    if (!dt || !IsArray(dt)) {
        return dt;
    }

    if (dt.length == 0 || IsObject(dt[0]) == false) {
        return dt;
    }

    var newArr = [];
    var obj;

    for (var i = 0; i < dt.length; i++) {
        obj = {};

        for (var field in dt[i]) {
            obj[isLower ? field.toLowerCase() : field.toUpperCase()] = dt[i][field];
        }

        newArr.push(obj);
    }

    return newArr;
}

//通用的aj访问与处理工具.
function AjaxServiceGener(param, myUrl, callback, scope) {

    $.ajax({
        type: "GET", //使用GET或POST方法访问后台
        dataType: "html", //返回json格式的数据
        contentType: "application/json; charset=utf-8",
        url: Handler + myUrl, //要访问的后台地址
        data: param, //要发送的数据
        async: true,
        cache: false,
        complete: function () { }, //AJAX请求完成时隐藏loading提示
        error: function (XMLHttpRequest, errorThrown) {
            callback(XMLHttpRequest);
        },
        success: function (data) { //msg为返回的数据，在这里做数据绑定
            callback(data, scope);
        }
    });
}

function IsArray(obj) {
    ///<summary>判断是否是数组</summary>
    ///<param name="obj" type="All Type">要判断的对象</param>
    return Object.prototype.toString.call(obj) == "[object Array]";
}

function IsObject(obj) {
    ///<summary>判断是否是Object对象</summary>
    ///<param name="obj" type="All Type">要判断的对象</param>
    return typeof obj != "undefined" && obj.constructor == Object;
}

function To(url) {
    //window.location.href = url;
    window.name = "dialogPage"; window.open(url, "dialogPage")
}

function WinOpen(url, winName) {
    var newWindow = window.open(url, winName, 'width=700,height=400,top=100,left=300,scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes;');
    newWindow.focus();
    return;
}

function WinOpenFull(url, winName) {
    var newWindow = window.open(url, winName, 'width=' + window.screen.availWidth + ',height=' + window.screen.availHeight + ',scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes;');
    newWindow.focus();
    return;
}

// document绑定esc键的keyup事件, 关闭弹出窗
function closeWhileEscUp() {
	$(document).bind("keyup", function (e) {
		e = e || window.event;
		var key = e.keyCode || e.which || e.charCode;
		if (key == 27) {
			// 可能需要调整if判断的顺序
			if (parent && typeof parent.doCloseDialog === 'function') {
				parent.doCloseDialog.call();
			} else if (typeof doCloseDialog === 'function') {
				doCloseDialog.call();
			} else if (parent && parent.parent && typeof parent.parent.doCloseDialog === "function") {
				parent.parent.doCloseDialog.call();
			} else {
				window.close();
			}
		}
	});
}

function DBAccess() {

    var url = Handler + "?SQL=select * from sss";
    $.ajax({
        type: "GET", //使用GET或POST方法访问后台
        dataType: "json", //返回json格式的数据
        contentType: "application/json; charset=utf-8",
        url: url, //要访问的后台地址
        async: true,
        cache: false,
        complete: function () { }, //AJAX请求完成时隐藏loading提示
        error: function (XMLHttpRequest, errorThrown) {
            callback(XMLHttpRequest);
        },
        success: function (data) { //msg为返回的数据，在这里做数据绑定
            callback(data, scope);
        }
    });

}


/* 关于实体的类
GEEntity_Init
var pkval="Demo_DtlExpImpDtl1";  
var EnName="BP.WF.Template.MapDtlExt";
GEntity en=new GEEntity(EnName,pkval);
var strs=  en.ImpSQLNames;
// var strss=en.GetValByKey('ImpSQLNames');
en.ImpSQLNames=aaa;
en.Updata();
*/

var Entity = (function () {

	var jsonString;

	var Entity = function (enName, pkval) {
		this.enName = enName;
		this.pkval = pkval;
		loadData(enName, pkval);
	};

	function setData() {
		if (typeof jsonString !== "undefined") {
			var self = this;
			$.each(jsonString, function (n, o) {
				// 需要判断属性名与当前对象属性名是否相同
				self[n] = o;
			});
		}
	}

	var pathName = document.location.pathname;
	var projectName = pathName.substring(0, pathName.substr(1).indexOf('/') + 1);
	if (projectName.startsWith("/WF")) {
		projectName = "";
	}
	var dynamicHandler = "/WF/Comm/Handler.ashx";

	function loadData(enName, pkval) {
		$.ajax({
			type: 'post',
			async: false,
			url: projectName + dynamicHandler + "?DoType=Entity_Init&EnName=" + enName + "&PKVal=" + pkval + "&t=" + new Date().getTime(),
			dataType: 'html',
			success: function (data) {
				if (data.indexOf("err@") != -1) {
					alert(data);
					return;
				}
				try {
					jsonString = JSON.parse(data);
					setData();
				} catch (e) {
					alert("解析错误: " + data);
				}
			},
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				alert("系统发生异常, status: " + XMLHttpRequest.status + " readyState: " + XMLHttpRequest.readyState);
			}
		});
	}

	Entity.prototype = {

		constructor : Entity,

		Insert : function () {
			var self = this;
			var modifyArrays = {};
			var count = 0;
			$.each(jsonString, function (n, defaultValue) {
				if (self[n] != defaultValue) {
					modifyArrays[n] = self[n];
					count++;
				}
			});
			if (count > 0) {
				$.ajax({
					type : 'post',
					async : false,
					url : projectName + dynamicHandler + "?DoType=Entity_Insert&EnName=" + self.enName + "&t=" + new Date().getTime(),
					dataType : 'html',
					data : modifyArrays,
					success : function (data) {
						if (data.indexOf("err@") != -1) {
							alert(data);
							return;
						}
						try {
							jsonString = JSON.parse(data);
							setData();
						} catch (e) {
							alert("解析错误: " + data);
						}
					},
					error : function (XMLHttpRequest, textStatus, errorThrown) {
						alert("系统发生异常, status: " + XMLHttpRequest.status + " readyState: " + XMLHttpRequest.readyState);
					}
				});
			}
		},

		Update : function () {
			var self = this;
			var modifyArrays = {};
			var count = 0;
			$.each(jsonString, function (n, defaultValue) {
				if (self[n] != defaultValue) {
					modifyArrays[n] = self[n];
					count++;
				}
			});
			if (count > 0) {
				$.ajax({
					type: 'post',
					async: false,
					url: projectName + dynamicHandler + "?DoType=Entity_Update&EnName=" + self.enName + "&PKVal=" + self.pkval + "&t=" + new Date().getTime(),
					dataType: 'html',
					data : modifyArrays,
					success: function (data) {
						$.each(modifyArrays, function (n, o) {
							jsonString[n] = o;
						});
					},
					error: function (XMLHttpRequest, textStatus, errorThrown) {
						alert("系统发生异常, status: " + XMLHttpRequest.status + " readyState: " + XMLHttpRequest.readyState);
					}
				});
			}
		},

		Delete : function () {
			var self = this;
			$.ajax({
				type: 'post',
				async: false,
				url: projectName + dynamicHandler + "?DoType=Entity_Delete&EnName=" + self.enName + "&PKVal=" + self.pkval + "&t=" + new Date().getTime(),
				dataType: 'html',
				success: function (data) {
					
				},
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					alert("系统发生异常, status: " + XMLHttpRequest.status + " readyState: " + XMLHttpRequest.readyState);
				}
			});
		}

	};

	return Entity;

})();

/*
function GEEntity(enName, pkval) {
	this.DoType = "GEEntity_Init";
	this.EnName = enName;
	this.pkval = pkval;
	this.jsonString = undefined;
	this.loadData();
}

GEEntity.prototype = {

    constructor: GEEntity,

    loadData: function () {
        var self = this;
        //
        var pathName = document.location.pathname;
        var projectName = pathName.substring(0, pathName.substr(1).indexOf('/') + 1);
        if (projectName.startsWith("/WF")) {
            projectName = "";
        }
		var dynamicHandler;
		if (plant == "CCFlow") {
			// CCFlow
			dynamicHandler = "/WF/Comm/Handler.ashx";
		} else {
			// JFlow
			dynamicHandler = "/WF/Comm/ProcessRequest.do";
		}
        $.ajax({
            type: 'post',
            async: false,
            url: projectName + dynamicHandler + "?DoType=GEEntity_Init&EnName=" + self.EnName + "&PKVal=" + self.pkval + "&t=" + new Date().getTime(),
            dataType: 'html',
            success: function (data) {

                if (data.indexOf("err@") != -1) {
                    alert(data);
                    return;
                }

                try {
                    self.jsonString = JSON.parse(data);
                } catch (e) {
                    alert("解析错误: " + data);
                }


            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert("系统发生异常, status: " + XMLHttpRequest.status + " readyState: " + XMLHttpRequest.readyState);
            }
        });
    },

    GetValByKey: function (key) {
        if (typeof this.jsonString != "undefined" && typeof key != "undefined") {
            return this.jsonString[key];
        }
    }

};


function GEEntitiesOrderBy(ensName, key1, val1, orderBy) {

}


function GEEntitiesOrderBy(ensName, key1, val1, key2, val2, orderBy) {

}



function GEEntitiesOr(ensName, key1, val1, key2, val2, key3, val3, key4, val4) {
  
}


function GEEntities(ensName, key1, val1, key2, val2, key3, val3, key4, val4) {

    var para = "@" + key1 + "=" + val1 + "@" + key2 + "=" + val2 + "@" + key3 + "=" + val3 + "@" + key4 + "=" + val4;


    this.DoType = "GEEntities_Init";
    this.EnName = enName;
    this.pkval = pkval;
    this.jsonString = undefined;
    this.loadData();
}
*/