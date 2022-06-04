/**
 * 章节表单从表的比对
 * @param {any} dtlNo 表单编号
 * @param {any} mainVerPK 主版本的MyPK
 * @param {any} comparePK 比对版本的MyPK
 */
var mainAllDtl = {};
var compareAllDtl = {};
function ChapterFrmDB_Dtl(dtlNo, mainVerPK, compareVerPK) {
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm_ChapterFrmDB");
    handler.AddPara("DtlNo", dtlNo);
    var data = handler.DoMethodReturnString("ChapterFrmDB_DtlInit");
    if (data.indexOf("err@") != -1) {
        layer.alert(data.replace("err@", ""));
        return;
    }
    //获取从表的信息
    data = JSON.parse(data);
    var mapDtl = data.Sys_MapDtl[0];
    var attrs = data.Sys_MapAttr;

    if (mainAllDtl[mapDtl.PTable] == undefined) {
        handler.Clear();
        handler.AddPara("MainVerPK", mainVerPK);
        handler.AddPara("CompareVerPK", compareVerPK);
        var data = handler.DoMethodReturnString("ChapterFrmDB_Dtl");
        if (data.indexOf("err@") != -1) {
            layer.alert(data.replace("err@", ""));
            return;
        }
        data = JSON.parse(data);
        mainAllDtl = data[0]["MainDtls"];
        mainAllDtl = JSON.parse(mainAllDtl);
        compareAllDtl = data[0]["CompareDtls"];
        compareAllDtl = JSON.parse(compareAllDtl);

    }
    var mainDtls = mainAllDtl[mapDtl.PTable];
    var compareDtls = compareAllDtl[mapDtl.PTable];
    var isMainAllDif = false;
    var isCompareAllDif = false;
    if (mainDtls.length == 0 && compareDtls.length != 0)
        isCompareAllDif = true;
    if (mainDtls.length != 0 && compareDtls.length == 0)
        isMainAllDif = true;
    //比对内容
    var isHaveXuHao = dtlTable("main", mainDtls, compareDtls, attrs);
    if (isHaveXuHao == true) {
        var checkMsg = checkGroup(mainDtls);
        if (checkMsg.IsPass)
            $("#mainHeJi").html('<p>合计：<span style="font-weight:bold" >' + new Decimal(checkMsg.TotalJinE).toFixed(6) + '</span>&nbsp;万元</p>');

    }
    
    
    dtlTable("compare", compareDtls, mainDtls, attrs);
    if (isHaveXuHao == true) {
        var checkMsg = checkGroup(compareDtls);
        if (checkMsg.IsPass)
            $("#compareHeJi").html('<p>合计：<span style="font-weight:bold" >' + new Decimal(checkMsg.TotalJinE).toFixed(6) + '</span>&nbsp;万元</p>');

    }
   
}
function dtlTable(type, dtlData, compareData, attrs) {
    var isHavXuhao = false;
    //解析col的展示
    var cols = [];
    attrs.forEach(attr => {
        if (attr.KeyOfEn == "XuHao")
            isHavXuhao = true;
        if (attr.UIVisible == 1) {
            cols.push({
                field: attr.KeyOfEn,
                title: attr.Name,
                fixed: false,
                minWidth: attr.Width,
                sort: false,
                templet: function (d) {
                    var idx = d.LAY_INDEX;
                    if (compareData.length >= idx) {
                        if (d[this.field] != compareData[idx - 1][this.field])
                            return "<span style='background-color:yellow'>" + d[this.field] + "</span>";
                        return d[this.field];
                    }
                    return "<span style='background-color:yellow'>" + d[this.field] + "</span>";

                }
            })
        }
    });
    layui.table.render({
        elem: '#' + type + 'Table',
        id: type + 'Table',
        data: dtlData,
        title: '数据表',
        limit: Number.MAX_VALUE,
        cols: [cols],
        page: false
    });
    return isHavXuhao;
}
var frmData = null;
function ChapterFrmDB_FrmLink(frmID, oid, mainVer, compareVer) {
    frmData = null;
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm_ChapterFrmDB");
    handler.AddPara("FrmID", frmID);
    handler.AddPara("OID", oid);
    handler.AddPara("MainVer", mainVer);
    handler.AddPara("CompareVer", compareVer);
    var data = handler.DoMethodReturnString("ChapterFrmDB_FrmGener");
    if (data.indexOf("err@") != -1) {
        layer.alert(data.replace("err@", ""));
        return;
    }
    frmData = JSON.parse(data);
    url = "./FrmGenerDB.htm?IsReadonly=1&FrmID=" + frmID + "&OID=" + oid + "&FK_MapData=" + frmID + "&FK_Node=" + GetQueryString("FK_Node") + "&IsReadonly=1";
    iframeFun(url + "&DBVer=" + mainVer + "&VerType=main", "main");
    iframeFun(url + "&DBVer=" + compareVer + "&VerType=compare", "compare");
}

/**
 * 获取附件的数据比对
 * @param {any} athMyPK
 * @param {any} mainVerPK
 * @param {any} compareVerPK
 */
//全局变量比对版本的附件信息
var mianAllAths = {};
var compareAllAths = {};
function ChapterFrmDB_Ath(athMyPK, mainVerPK, compareVerPK) {
    var noOfObj = athMyPK.replace(frmID + "_", "");
    if (mainAllDtl[noOfObj] == undefined) {
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm_ChapterFrmDB");
        handler.AddPara("MainVerPK", mainVerPK);
        handler.AddPara("CompareVerPK", compareVerPK);
        var data = handler.DoMethodReturnString("ChapterFrmDB_Ath");
        if (data.indexOf("err@") != -1) {
            layer.alert(data.replace("err@", ""));
            return;
        }

        data = JSON.parse(data);
        mianAllAths = data[0]["MainAths"];
        mianAllAths = JSON.parse(mianAllAths);
        compareAllAths = data[0]["CompareAths"];
        compareAllAths = JSON.parse(compareAllAths);

    }
    var mainAths = mianAllAths[noOfObj];
    var compareAths = compareAllAths[noOfObj];
    athTable("main", mainAths, compareAths);
    athTable("compare", compareAths, mainAths);

}
/**
 * 解析附件的数据比对
 * @param {any} type 主版本（main)/比对版本(compare)
 * @param {any} athData 附件数据
 * @param {any} compareData 比对版本的数据
 */
function athTable(type, athData, compareData) {
    var cols = [
        { field: "FileName", title: "文件名", fixed: false, minWidth: 200, templet: function (row) { return SetFieldBackgroudColor(row, this.field, compareData); } },
        { field: "FileExts", title: "类型", fixed: false, minWidth: 100, templet: function (row) { return SetFieldBackgroudColor(row, this.field, compareData); } },
        { field: "RDT", title: "上传时间", fixed: false, minWidth: 120, templet: function (row) { return SetFieldBackgroudColor(row, this.field, compareData); } },
        { field: "RecName", title: "上传人", fixed: false, minWidth: 120, templet: function (row) { return SetFieldBackgroudColor(row, this.field, compareData); } }
    ];
    layui.table.render({
        elem: '#' + type + 'Table',
        id: type + 'Table',
        data: athData,
        title: '数据表',
        limit: Number.MAX_VALUE,
        cols: [cols],
        page: false
    });
}
/**
 * 返回字段背景色
 * @param {any} row
 * @param {any} field
 * @param {any} compareData
 */
function SetFieldBackgroudColor(row, field, compareData) {
    var idx = row.LAY_INDEX;
    if (compareData.length >= idx) {
        if (row[field] != compareData[idx - 1][field])
            return "<span style='background-color:yellow'>" + row[field] + "</span>";
        return row[field];
    }
    return "<span style='background-color:yellow'>" + row[field] + "</span>";
}


//检查数据是否正确
function checkGroup(dtlData) {
    var reMsgObj = {};
    var grList = new Array();
    for (var i = 0; i < dtlData.length; i++) {
        dtlData[i].DirID = i;

        var gr = isDir(dtlData[i].XuHao);
        if (gr.IsDir) {
            dtlData[i].IsDir = 1;

            gr.RowIndex = i;
            gr.JinE = dtlData[i].JinE;
            grList.push(gr);

            continue;
        }
        var grSub = isSubDir(dtlData[i].XuHao);
        if (grSub.IsSubDir) {
            dtlData[i].IsDir = 0;

            var danJia = new Decimal(dtlData[i].DANJiaHuoZhiChuBiaoZ.toString());
            var shuLiang = new Decimal(dtlData[i].ShuLiang.toString());
            var checkJinE = danJia.mul(shuLiang).div(new Decimal(10000)).toFixed(6);
            if (new Decimal(dtlData[i].JinE).equals(checkJinE) == false) {
                reMsgObj.IsPass = false;
                reMsgObj.Msg = "序号:" + dtlData[i].XuHao + ",金额计算错误,请检查";
                reMsgObj.Data = null;

                return reMsgObj;
            } else {
                continue;
            }
        }

        reMsgObj.IsPass = false;
        reMsgObj.Msg = "序号:" + dtlData[i].XuHao + ",格式错误,请检查";
        reMsgObj.Data = null;

        return reMsgObj;

    }


    //检查分组的顺序是否正确 
    var res = false;
    var _parWinTotalJinE = new Decimal(0);//.add(new Decimal(subJine))
    for (var i = 0; i < grList.length; i++) {

        //检查子项是否正确
        var reMsg = getSubGroup(dtlData, grList[i]);
        if (reMsg.IsPass == false) {
            reMsgObj.IsPass = false;
            reMsgObj.Msg = "分组" + grList[i].OriVal + "子项异常,请检查";
            reMsgObj.Data = null;

            return reMsgObj;
        } else {
            if (reMsg.Data.equals(new Decimal(grList[i].JinE)) == false) {
                reMsgObj.IsPass = false;
                reMsgObj.Msg = "分组" + grList[i].OriVal + "金额计算错误,请检查";
                reMsgObj.Data = null;

                return reMsgObj;
            }
        }

        _parWinTotalJinE = _parWinTotalJinE.add(reMsg.Data);
        if (i + 1 == grList.length)
            break;

        if (grList[i].NumVal - grList[i + 1].NumVal == -1)
            res = true;
        else {
            res = false;

            reMsgObj.IsPass = false;
            reMsgObj.Msg = "错误的分组排序:" + grList[i + 1].OriVal + "和" + grList[i].OriVal;
            reMsgObj.Data = null;

            return reMsgObj;
        }
    }
    reMsgObj.IsPass = true;
    reMsgObj.Msg = "检查没有异常";
    reMsgObj.Data = dtlData;
    reMsgObj.TotalJinE = _parWinTotalJinE.toFixed();

    return reMsgObj;
}
function getSubGroup(dtlData, gr) {
    var reMsgObj = {};
    var subGroupData = new Array();
    for (var i = gr.RowIndex + 1; i < dtlData.length; i++) {
        if (dtlData[i].IsDir == 1) {
            break;
        }

        subGroupData.push(dtlData[i]);
    }

    //分组号必须从（一）开始
    var firstSubData = subGroupData.filter((item) => item.XuHao == '（一）');
    if (firstSubData.length == 0) {
        reMsgObj.IsPass = false;
        reMsgObj.Msg = "明细表分组格式错误,分组号：" + gr.OriVal;
        reMsgObj.Data = null;

        return reMsgObj;
    }
    var totalJinE = new Decimal(0);
    var res = false;
    for (var i = 0; i < subGroupData.length; i++) {
        //totalJinE += parseFloat(subGroupData[i].JinE);
        totalJinE = totalJinE.add(new Decimal(subGroupData[i].JinE));
        if (i + 1 == subGroupData.length) {
            res = true;
            break;
        }

        if (isSubDir(subGroupData[i].XuHao).NumVal - isSubDir(subGroupData[i + 1].XuHao).NumVal == -1)
            res = true;
        else {
            res = false;

            reMsgObj.IsPass = false;
            reMsgObj.Msg = "错误的分组子项排序:" + subGroupData[i + 1].XuHao + "和" + subGroupData[i].XuHao;
            reMsgObj.Data = null;

            return reMsgObj;
        }
    }
    if (res) {
        reMsgObj.IsPass = true;
        reMsgObj.Msg = "分组子项检查无异常";
        reMsgObj.Data = totalJinE;

        return reMsgObj;
    }
}

//是否为分组，以汉字、为格式
function isDir(val) {
    var obj = {};
    obj.OriVal = val;

    if (val == undefined) {
        obj.IsDir = false;
        obj.NumVal = 0;
        obj.CnVal = val;

        return obj;
    }

    if (val.length < 2) {
        obj.IsDir = false;
        obj.NumVal = 0;
        obj.CnVal = val;

        return obj;
    }

    var rightChar = val.substr(val.length - 1, 1);
    if (rightChar != '、') {

        obj.IsDir = false;
        obj.NumVal = 0;
        obj.CnVal = val;

        return obj;
    }
    val = val.substr(0, val.length - 1);
    var _numVal = ChineseToNumber(val);

    if (_numVal == 0) {
        obj.IsDir = false;
        obj.NumVal = _numVal;
        obj.CnVal = val;
    } else {
        obj.IsDir = true;
        obj.NumVal = _numVal;
        obj.CnVal = val;
    }

    return obj;
}
//是否为子项目
function isSubDir(val) {
    var obj = {};
    obj.OriVal = val;

    if (val.length < 3) {
        obj.IsSubDir = false;
        obj.NumVal = 0;
        obj.CnVal = val;

        return obj;
    }

    var leftChar = val.substr(0, 1);
    var rightChar = val.substr(val.length - 1, 1);

    if (leftChar != '（' || rightChar != '）') {
        obj.IsSubDir = false;
        obj.NumVal = 0;
        obj.CnVal = val;

        return obj;
    }
    val = val.substr(1, val.length - 1);
    val = val.substr(0, val.length - 1);
    var _numVal = ChineseToNumber(val);

    if (_numVal == 0) {
        obj.IsSubDir = false;
        obj.NumVal = 0;
        obj.CnVal = val;
    } else {
        obj.IsSubDir = true;
        obj.NumVal = _numVal;
        obj.CnVal = val;
    }

    return obj;
}
