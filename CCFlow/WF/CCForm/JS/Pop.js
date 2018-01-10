//树干叶子模式.
function PopBranchesAndLeaf(mapExt) {

    var tb = $("#" + mapExt.AttrOfOper);

    // 如果该文本框只读，就return,因为只读模式下不让其选择了.
    // var tb.attr('disabled');

    //设置文本框只读.
    tb.attr('readonly', 'true');
    tb.attr('disabled', 'true');
    tb.attr("onclick", "alert('ssssssssssssss');");
    return;


  //  alert(tb);

    // 把文本框的内容，按照逗号分开, 并且块状显示. 右上角出现删除叉叉.

    // 文本框尾部出现选择的图标.
    icon = "glyphicon glyphicon-tree-deciduous";
    var eleHtml = ' <div class="input-group form_tree">' + tb.parent().html()
    eleHtml += '<span class="input-group-addon" onclick="PopBranchesAndLeaf_Deal(this,' + "'" + mapExt.MyPK + "','" + mapExt.FK_MapData + "', " + mapExt.W + "," + mapExt.H + ",'" + GepParaByName("Title", mapExt.AtPara) + "');" + '"><span class="' + icon + '"></span></span></div>';
    tb.parent().html(eleHtml);


   // tb.parent().html(eleHtml);
   // alert(eleHtml);

    //在文本框双击，绑定弹出. DeptEmpModelAdv.htm的窗口.
    tb.attr("onclick", "alert('ssssssssssssss');");

//    tb.attr("onclick", "ShowHelpDiv('TB_" + mapExt.AttrOfOper + "','','" + mapExt.MyPK + "','" + mapExt.FK_MapData + "','returnvalccformpopval');");
    //窗口返回值的时候，重新计算文本块.
}

function PopBranchesAndLeaf_Deal()
{
 
}

//树干模式.
function PopBranches(mapExt) {

    var tb = $("#TB_" + mapExt.AttrOfOper);

    //设置文本框只读.
    tb.attr('readonly', 'true');
    tb.attr('disabled', 'true');

    // 把文本框的内容，按照逗号分开, 并且块状显示. 右上角出现删除叉叉.

    // 文本框尾部出现选择的图标.
    icon = "glyphicon glyphicon-tree-deciduous";
    var eleHtml = ' <div class="input-group form_tree">' + tb.parent().html()
    eleHtml += '<span class="input-group-addon" onclick="' + "ReturnValCCFormPopValGoogle('TB_" + mapExt.AttrOfOper + "','" + mapExt.MyPK + "','" + mapExt.FK_MapData + "', " + mapExt.W + "," + mapExt.H + ",'" + GepParaByName("Title", mapExt.AtPara) + "');" + '"><span class="' + icon + '"></span></span></div>';
    tb.parent().html(eleHtml);

    //在文本框双击，绑定弹出. DeptEmpModelAdv.htm的窗口.
  //  tb.attr("onclick", "alert('ssssssssssssss');");

    //    tb.attr("onclick", "ShowHelpDiv('TB_" + mapExt.AttrOfOper + "','','" + mapExt.MyPK + "','" + mapExt.FK_MapData + "','returnvalccformpopval');");
    //窗口返回值的时候，重新计算文本块.

}

/******************************************  表格查询 **********************************/

function PopTableSearch(mapExt) {

    var tb = $("#TB_" + mapExt.AttrOfOper);
    if (tb.length == 0)
        return; //有可能字段被删除了.

    //设置文本框只读.
    tb.attr('readonly', 'true');
    // tb.attr('disabled', 'true');

    //在文本框双击，绑定弹出. PopGroupList.htm的窗口. 
    tb.bind("click", function () { PopTableSearch_Done(mapExt) });
}

function PopTableSearch_Done(mapExt) {

    //获得主键.
    var pkval = GetPKVal();

    //弹出这个url, 主要有高度宽度, 可以在  ReturnValCCFormPopValGoogle 上做修改.
    var url = 'Pop/TableSearch.htm?FK_MapExt=' + mapExt.MyPK + "&FK_MapData=" + mapExt.FK_MapData + "&PKVal=" + pkval + "&OID=" + pkval + "&KeyOfEn=" + mapExt.AttrOfOper;

    if (window.parent && window.parent.OpenBootStrapModal) {
        window.parent.OpenBootStrapModal(url, "eudlgframe", "导入数据", mapExt.H, mapExt.W, "icon-edit", false, function () { }, null, function () {
            location = location;
        });
        return;
    }
}


/******************************************  分组列表 **********************************/

function PopGroupList(mapExt) {

    var tb = $("#TB_" + mapExt.AttrOfOper);
    if (tb.length == 0)
        return; //有可能字段被删除了.

    //设置文本框只读.
    tb.attr('readonly', 'true');
   // tb.attr('disabled', 'true');

    //在文本框双击，绑定弹出. PopGroupList.htm的窗口. 
    tb.bind("click", function (){ PopGroupList_Done(mapExt)} );
}

function PopGroupList_Done(mapExt) {

    //获得主键.
    var pkval = GetPKVal();

    //弹出这个url, 主要有高度宽度, 可以在  ReturnValCCFormPopValGoogle 上做修改.
    var url = 'Pop/GroupList.htm?FK_MapExt=' + mapExt.MyPK + "&FK_MapData=" + mapExt.FK_MapData + "&PKVal="+pkval+"&OID=" + pkval + "&KeyOfEn=" + mapExt.AttrOfOper;

    if (window.parent && window.parent.OpenBootStrapModal) {
        window.parent.OpenBootStrapModal(url, "eudlgframe", "导入数据", mapExt.H, mapExt.W, "icon-edit", false, function () { }, null, function () {
            
            //location = location;

        });
        return;
    }
}


function GetPKVal() {

    var val = GetQueryString("OID");
    if (val==undefined || val=="")
        val = GetQueryString("No");
    if (val == undefined || val == "")
        val = GetQueryString("WorkID");

    if (val == undefined || val == "")
        val = GetQueryString("MyPK");

    return val;


}