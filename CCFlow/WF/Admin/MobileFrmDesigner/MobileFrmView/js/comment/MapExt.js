//ref WF/CCForm/MapExt.js
function CCForm_DealMapExt(mapExts) {
    var fun_Script = "";
    for (var i = 0; i < mapExts.length; i++) {
        var myPK = mapExts[i].MyPK;
        var ExtType = mapExts[i].ExtType;
        switch (ExtType) {
            case "AutoFullDLL": //装载填充
                var autoAttrOfOper = "DDL_" + mapExts[i].AttrOfOper;
                var autoDDLOper = $("#" + autoAttrOfOper);
                //判断控件是否存在
                if (autoDDLOper.length == 0)
                    continue;
                //填充下拉框
                var value = $("#" + autoAttrOfOper).val();
                AutoFullDLL(value, autoAttrOfOper, myPK);
                break;
            case "DDLFullCtrl": //自动填充其他的控件
                var fullAttrOfOper = "DDL_" + mapExts[i].AttrOfOper;
                var fullDDLOper = $("#" + fullAttrOfOper);
                //判断控件是否存在
                if (fullDDLOper.length == 0)
                    continue;

                fun_Script += " $(\"#" + fullAttrOfOper + "\").change(function () {";
                fun_Script += "     var selectValue = $(\"#" + fullAttrOfOper + "\").val();";
                fun_Script += "     DDLFullCtrl(selectValue, \"" + fullAttrOfOper + "\", \"" + myPK + "\");";
                fun_Script += "});\n\t";
                break;
            case "ActiveDDL": //自动初始化ddl的下拉框数据
                var attrOfOper = "DDL_" + mapExts[i].AttrOfOper;
                var attrsOfActive = "DDL_" + mapExts[i].AttrsOfActive;

                var ddlParent = $("#" + attrOfOper);
                var ddlChild = $("#" + attrsOfActive);
                //判断控件是否存在
                if (ddlParent.length == 0 || ddlChild.length == 0)
                    continue;

                fun_Script += " $(\"#" + attrOfOper + "\").change(function () {";
                fun_Script += "     var selectValue = $(\"#" + attrOfOper + "\").val();";
                fun_Script += "     DDLAnsc(selectValue, \"" + attrsOfActive + "\", \"" + myPK + "\");";
                fun_Script += "});\n\t";
                break;
        }
    }
    //执行函数
    if (fun_Script != "")
        cceval(fun_Script);
}

//按钮处理事件
function FrmBtnEventFactory(objID, EventType, EventContent) {
    switch (EventType) {
        case "6": //js脚本
            cceval(EventContent);    
            break;
    }
}