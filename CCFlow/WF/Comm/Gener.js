﻿/* ESC Key Down */
function Esc() {
    if (event.keyCode == 27)
        window.close();
    return true;
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
function GenerFullDropDownListCtrl(ddlCtrlID, data, noCol, nameCol, selectVal) {

    //判断data是否是一个数组，如果是一个数组，就取第1个对象.
    var json = data;
    if (data[0].length == 1)
        json = data[0];

    // 清空默认值, 写一个循环把数据给值.
    $("#" + ddlCtrlID).empty();

    //如果他的数量==0，就return.
    if (json.length == 0)
        return;

    if (json[0][noCol] == undefined) {
        alert('@在绑定[' + ddlCtrlID + ']错误，No列名' + noCol + '不存在,无法行程期望的下拉框value . ');
        return;
    }

    if (json[0][nameCol] == undefined) {
        alert('@在绑定[' + ddlCtrlID + ']错误，Name列名' + nameCol + '不存在,无法行程期望的下拉框value . ');
        return;
    }

    for (var i = 0; i < json.length; i++) {
        $("#" + ddlCtrlID).append("<option value='" + json[i][noCol] + "'>" + json[i][nameCol] + "</option>");
    }

    //    $("#DDL_" + attr).attr('selectedIndex', 0);
    //    $("#DDL_" + attr).prop('selectedIndex', 0);
    //    return;

    //    if (selectVal == undefined) {
    //        alert(selectVal);
    //        $("#DDL_" + attr).attr("selectedIndex", 0);
    //        //  $("#DDL_" + attr).val(selectVal);
    //        return;
    //    }

    //设置选中的值.
    if (selectVal != undefined) {
        $("#DDL_" + attr).val(selectVal);
    }
    else {
        $("#DDL_" + attr).prop('selectedIndex', 0);
    }

    //    alert('ss');
    //$("#DDL_" + attr).options[0].selected = true;
}

/*
 绑定枚举值.
*/
function GenerBindEnumKey(ctrlDDLId, enumKey, selectVal) {

    $.ajax({
        type: 'post',
        async: true,
        url: "/WF/Comm/Handler.ashx?DoType=EnumList&EnumKey=" + enumKey + "&m=" + Math.random(),
        dataType: 'html',
        success: function (data) {
            data = JSON.parse(data);
            //绑定枚举值.
            GenerFullDropDownListCtrl(ctrlDDLId, data, "IntKey", "Lab");
            return;
        }
    });
}


/*
  绑定枚举值外键表.
*/
function GenerBindEntities(ctrlDDLId, ensName, selectVal) {

    $.ajax({
        type: 'post',
        async: true,
        url: "/WF/Comm/Handler.ashx?DoType=EnsData&EnsName=" + ensName + "&m=" + Math.random(),
        dataType: 'html',
        success: function (data) {

//            if (data.indexof('err@') ==0 )
//            {
//               alert(data);
//               return ;
//            }

            data = JSON.parse(data);
            //绑定枚举值.
            GenerFullDropDownListCtrl(ctrlDDLId, data, "No", "Name",selectVal);
            return;
        },
        error: function (jqXHR, textStatus, errorThrown) {
            /*错误信息处理*/
            alert("GenerBindEntities,错误:参数:EnsName"+ensName+" , 异常信息 responseText:"+jqXHR.responseText+"; status:"+jqXHR.status+"; statusText:"+jqXHR.statusText+"; \t\n textStatus="+textStatus+";errorThrown="+errorThrown);
        }
    });
}


/*
  绑定 s 外键表.
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
            GenerFullDropDownListCtrl(ctrlDDLId, data, "No", "Name", selectVal);
            return;
        },
        error: function (jqXHR, textStatus, errorThrown) {
            /*错误信息处理*/
            alert("GenerBindSFTable,错误:参数:EnsName" + ensName + " , 异常信息 responseText:" + jqXHR.responseText + "; status:" + jqXHR.status + "; statusText:" + jqXHR.statusText + "; \t\n textStatus=" + textStatus + ";errorThrown=" + errorThrown);
        }
    });
}


/*
   为页面的所有字段属性赋值.
*/

function GenerFullAllCtrlsVal(data) {

    //判断data是否是一个数组，如果是一个数组，就取第1个对象.
    var json = data;
    if (data.length == 1)
        json = data[0];

    var unSetCtrl = "";
    for (var attr in json) {

        var val = json[attr]; //值

        // textbox
        var tb = document.getElementById('TB_' + attr);
        if (tb != null) {
            tb.value = val;
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

        unSetCtrl += "@" + attr + " = " + val;
    }

    // alert('没有找到的控件类型:' + unSetCtrl);
}