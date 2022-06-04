//状态 1=备注状态 ， 0=无状态.
var dbVerSta = 0;
var frmDBVers = null; //填过意见的.

/**
 * 初始化有批注的字段.
 * 根据是否有批注，就在控件上加批注标识.
 * */
function FrmDBVer_Init(frmID, pkval) {
    //如果是
    if (dbVerSta == 1) {
        FrmDBVer_UnDo();
        return;
    }

    //弹窗选择版本号.
    var vers = new Entities("BP.Sys.FrmDBVers");
    vers.Retrieve("FrmID", frmID, "RefPKVal", pkval, "RDT");
    if (vers.length == 0) {
        alert('没有版本可以比对...');
        return;
    }

    var html = "<table>";
    html += "<tr>";
    html += "<th>版本号</th>";
    html += "<th>日期</th>";
    html += "<th>提交者</th>";
    html += "<th>变化数</th>";
    html += "<th>操作</th>";
    html += "</tr>";

    for (var i = 0; i < vers.length; i++) {
        var ver = vers[i];
        html += "<tr>";
        html += " <td>" + (i + 1) + "</td>";
        html += " <td>" + ver.RDT + "</td>";
        html += " <td>" + ver.RecName + "</td>";
        html += " <td>" + ver.ChangeNum + "</td>";
        html += " <td><a href=\"javascript:DBVerCheck('"+ver.MyPK+"')\" >比对</a></td>";
        html += "</tr>";
    }
    html += "</table>";


    //获得有批注的字段.
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Template_JS");
    handler.AddPara("FrmID", frmID);
    handler.AddPara("PKVal", pkval);
    frmDBVers = handler.DoMethodReturnJSON("FrmDBVer_GenerChangeFields");

    //根据这些字段，为字段增加标签。
    for (var i = 0; i < frmDBVers.length; i++) {
        var en = frmDBVers[i];
    }
    //设置为批注状态.
    dbVerSta = 1;
}

/**
 * 执行比对
 * @param {any} myPK
 */
function DBVerCheck(myPK)
{

}
/**
 * 撤销批注状态
 * */
function FrmDBVer_UnDo() {

    //做的特殊标记都删除掉.

    //批注状态.
    dbVerSta = 0;
}


/**
 * 显示批阅信息（弹窗显示）
 * @param {字段} field
 */
function FrmDBVer_Show(field) {
    //遍历集合  FrmDBVers, 过滤相关的字段， 显示历史的信息.

}


/**
 * 保存
 * @param {表单ID} frmID
 * @param {字段ID} field
 * @param {主键} pkval
 * @param {批阅信息} remark
 */
function FrmDBVer_Save(frmID, field, pkval, remark) {
    var en = new Entity("BP.Sys.FrmData");
    en.FrmID = frmID; //表单ID.
    en.Field = field; //字段。
    en.Remark = remark; //批阅意见.
    en.PKVal = pkval; //主键字段.
    en.Insert(); //插入数据.
}

/**
 * 删除
 * @param {主键} mypk
 */
function FrmDBVer_Delete(mypk) {
    var en = new Entity("BP.Sys.FrmData", mypk);
    en.Delete();
}


