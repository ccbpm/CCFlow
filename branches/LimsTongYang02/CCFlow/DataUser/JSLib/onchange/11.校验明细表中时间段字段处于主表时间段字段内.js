
function CheckDateRegion(s) {
    if(isChanging){
		return true;
	}
	
	if(s.id.indexOf('TB_'+Dtl_DateFrom_Ctrl_ID) == -1 && s.id.indexOf('TB_'+Dtl_DateTo_Ctrl_ID) == -1){
		return true;
	}
	//获取主表中开始日期，结束日期控件
    if(!frmDFromCtrl)
        frmDFromCtrl = GetCtrl(window.parent.document, Form_DateFrom_Ctrl_ID);

    if(!frmDToCtrl)
        frmDToCtrl = GetCtrl(window.parent.document, Form_DateTo_Ctrl_ID);

    if (!frmDFromCtrl) {
        alert('主表中未查询到TB_' + Form_DateFrom_Ctrl_ID + '列,请联系管理员修改流程节点对应的JS文件DataUser/JSLibData/' + getArgsFromHref('EnsName') + '.js中的Form_DateFrom_Ctrl_ID变量为明细表中“开始日期”字段名称！');
        return false;
    }

    if (!frmDToCtrl) {
        alert('主表中未查询到TB_' + Form_DateTo_Ctrl_ID + '文本框,请联系管理员修改流程节点对应的JS文件DataUser/JSLibData/' + getArgsFromHref('EnsName') + '.js中的Form_DateTo_Ctrl_ID变量为主表单“结束日期”字段的名称！');
        return false;
    }
	
	var dateFrom = frmDFromCtrl.value;
	var dateTo = frmDToCtrl.value;
	//获取明细表中开始日期，结束日期控件
	var rowNum = 0;
	//如果是开始日期
	if(s.id.indexOf('TB_'+Dtl_DateFrom_Ctrl_ID) != -1){
		dtlDFromCtrl = s;
		rowNum = s.id.substr(('TB_'+Dtl_DateFrom_Ctrl_ID+'_').length);
		dtlDToCtrl = GetCtrl(document, Dtl_DateTo_Ctrl_ID+'_'+rowNum);
		
		if(!dtlDToCtrl){
			alert('未查询到TB_' + Dtl_DateTo_Ctrl_ID + '列,请联系管理员修改流程节点对应的JS文件DataUser/JSLibData/' + getArgsFromHref('EnsName') + '.js中的Dtl_DateTo_Ctrl_ID变量为明细表中“结束日期”字段名称！');
			return false;
		}
	}
	
	if(s.id.indexOf('TB_'+Dtl_DateTo_Ctrl_ID) != -1){
		dtlDToCtrl = s;
		rowNum = s.id.substr(('TB_'+Dtl_DateTo_Ctrl_ID+'_').length);
		dtlDFromCtrl = GetCtrl(document, Dtl_DateFrom_Ctrl_ID+'_'+rowNum);
		
		if(!dtlDFromCtrl){
			alert('未查询到TB_' + Dtl_DateFrom_Ctrl_ID + '列,请联系管理员修改流程节点对应的JS文件DataUser/JSLibData/' + getArgsFromHref('EnsName') + '.js中的Dtl_DateFrom_Ctrl_ID变量为明细表中“开始日期”字段名称！');
			return false;
		}
	}
	
	//校验日期范围准确性
	if(dateFrom && dateFrom.length > 0){
		var msg = '开始日期';
		if(dtlDFromCtrl.value){
			if(dtlDFromCtrl.value < dateFrom){
				msg += '不能小于“'+dateFrom+'”，';
			}
			if(dateTo && dateTo.length > 0 && dtlDFromCtrl.value > dateTo){
				msg += '不能大于“'+dateTo+'”，';
			}
		}
		
		if(msg.length > 4){
			alert(msg.substr(0,msg.length-1));
			isChanging = true;
			dtlDFromCtrl.value = dateFrom;
			isChanging = false;
			return true;
		}
	}
	
	if(dateTo && dateTo.length > 0){
		var msg = '结束日期';
		if(dtlDToCtrl.value){
			if(dtlDToCtrl.value > dateTo){
				msg += '不能大于“'+dateTo+'”，';
			}
			if(dateFrom && dateFrom.length > 0 && dtlDToCtrl.value < dateFrom){
				msg += '不能小于“'+dateFrom+'”，';
			}
		}
		
		if(msg.length > 4){
			alert(msg.substr(0,msg.length-1));
			isChanging = true;
			dtlDToCtrl.value = dateTo;
			isChanging = false;
			return true;
		}
	}
	
	if(dtlDFromCtrl.value && dtlDFromCtrl.value.length > 0 && dtlDToCtrl.value && dtlDToCtrl.value.length > 0 && dtlDToCtrl.value < dtlDFromCtrl.value){
		alert('结束日期不能小于开始日期！');
		isChanging = true;
		var tdate = dtlDFromCtrl.value;
		dtlDFromCtrl.value = dtlDToCtrl.value;
		dtlDToCtrl.value = tdate;
		isChanging = false;
		return true;
	}

    return true;
}

var Dtl_DateFrom_Ctrl_ID = "KSZYRQ";  //明细表开始日期字段的ID
var Dtl_DateTo_Ctrl_ID = "JSZYRQ";  //明细表结束日期字段的ID
var Form_DateFrom_Ctrl_ID = "KSRQ";   //主表单开始日期字段的ID
var Form_DateTo_Ctrl_ID = "JSRQ";   //主表单结束日期字段的ID
var frmDFromCtrl;
var frmDToCtrl;
var dtlDFromCtrl;
var dtlDToCtrl;
var isChanging = false;

function GetCtrl(doc, ctrlFieldId) {
    var ctrls = doc.getElementsByTagName("input");
    var idx;
    var ctrlid = 'TB_' + ctrlFieldId;
    var ctrl;

    for (var i = 0; i < ctrls.length; i++) {
        idx = ctrls[i].id.indexOf(ctrlid);
        if (idx != -1 && (idx + ctrlid.length) == ctrls[i].id.length) {
            ctrl = ctrls[i];
            break;
        }
    }

    return ctrl;
}

function getArgsFromHref(sArgName) {
    var sHref = window.location.href;
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