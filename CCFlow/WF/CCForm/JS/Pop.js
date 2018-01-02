//部门人员的高级多选.
function DeptEmpModelAdv(mapExt) {

    var tb = $("#" + mapExt.AttrOfOper);

    // 如果该文本框只读，就return,因为只读模式下不让其选择了.
    // var tb.attr('disabled');

    //设置文本框只读.
    tb.attr('readonly', 'true');
    tb.attr('disabled', 'true');

    // 把文本框的内容，按照逗号分开, 并且块状显示. 右上角出现删除叉叉.

    // 文本框尾部出现选择的图标.
    icon = "glyphicon glyphicon-tree-deciduous";
    var eleHtml = ' <div class="input-group form_tree">' + tb.parent().html()
    eleHtml += '<span class="input-group-addon" onclick="' + "ReturnValCCFormPopValGoogle('TB_" + mapExt.AttrOfOper + "','" + mapExt.MyPK + "','" + mapExt.FK_MapData + "', " + mapExt.W + "," + mapExt.H + ",'" + GepParaByName("Title", mapExt.AtPara) + "');" + '"><span class="' + icon + '"></span></span></div>';
    tb.parent().html(eleHtml);

   // tb.parent().html(eleHtml);
   // alert(eleHtml);


    //在文本框双击，绑定弹出. DeptEmpModelAdv.htm的窗口.
    tb.attr("onclick", "alert('ssssssssssssss');");

//    tb.attr("onclick", "ShowHelpDiv('TB_" + mapExt.AttrOfOper + "','','" + mapExt.MyPK + "','" + mapExt.FK_MapData + "','returnvalccformpopval');");


    //窗口返回值的时候，重新计算文本块.

}