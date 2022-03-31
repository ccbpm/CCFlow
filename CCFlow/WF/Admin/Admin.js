/*
 * admin.js文件说明
 *  1. 为后台管理的页面增加的文件, 用于可以自动化的改变后台设置的一些特效.
 *  2. 解决帮助的统一风格问题.
 *  3. 目前增加了两个模式的操作。 
 *     3.1 fieldset 的 legend 的ID 包含 help 就是帮助内容页面. 出现的效果是加载后，是隐藏的，用户点击文字就要显示里面的内容
 *     3.2 对于textArea 如果在 class='SQL' 系统就认为是表达式sql文本输入框.
 *     3.3 对于标记 class="HelpImg" 的图片绑定事件，在click 让其可以全屏打开.
 *     3.4 class=Help 的div都是帮助用的div. 比如下面，想办法，让其点击隐藏与显示. 
 *     http://localhost:2207/WF/Admin/AttrNode/Selector/3.SQL.htm?FK_Node=1702
 *       <div id="DivHelp1" class="help">
                是对人员的分组,分组的目的就是为了更友好的找到人员，比如:<br />
                1. SELECT No,Name FROM Port_Dept <br />
                2. SELECT No,Name FROM Port_Dept WHERE ParentNo='@WebUser.FK_Dept'<br />
                3. SELECT No,Name FROM Port_Station WHERE No IN('01','02') 查询岗位编号是01，02 <br />
                4. 分组数据源可以为空，如果为空就显示的时候不分组.<br />
            </div>
 *     
 *  4. 代国强来完善这两个方法，参考 D:\ccflow\CCFlow\WF\CCBill\Admin\MethodDoc.htm 页面。
 *  
 */

$(document).ready(function () {

    //动态添加新风格 
    SetCSS();

    //设置帮助页面内容 
    // SetHelpPage();
    HelpDiv();

    //设置自动提示.
    initToggle();

    //设置 class="Help" class="HelpImg"  的图片 点击直接可以全屏放大打开.  @lz
    SetHelpImg();

    //设置放大的img容器   
    SetBigImgDiv();

    //设置SQL脚本编辑器.
    CheckSQLTextArea();

    //如何给按钮自动增加标签?
    AddBtnIcon();


    /* $(".cs-content-box legend").click(function () {  //给每个li元素添加点击事件
         $(".cs-content-box legend").removeClass('up');
         $(this).addClass('up');       
         $(this).parent().children(".cs-help").slideDown();
         $(this).parent().siblings().children(".cs-help").slideUp();
     });*/

})

function AddBtnIcon() {

    //保存按钮icon
    /*if ($("#Btn_Save").height() > 25) {
        $("#Btn_Save").addClass("cc-btn Btn_Save");
       // $("#Btn_Save").attr('style', 'background-image: url(../../../Img/Btn/Save.png); background-repeat: no-repeat; background-size: 14px 14px; background-position: 6px 8px;');
    }
    else
        $("#Btn_Save").attr('style', 'background-image: url(../../../Img/Btn/Save.png); background-repeat: no-repeat; background-size: 13px 13px; background-position: 1px 6px;');


    if ($("#Btn_Delete").height() > 25)
        $("#Btn_Delete").attr('style', 'background-image: url(../../../Img/Btn/Delete.png); background-repeat: no-repeat; background-size: 14px 14px; background-position: 6px 8px;');
    else
        $("#Btn_Delete").attr('style', 'background-image: url(../../../Img/Btn/Delete.png); background-repeat: no-repeat; background-size: 13px 13px; background-position: 1px 6px;');

    //返回按钮icon
    if ($("#Btn_Back").height() > 25)
        $("#Btn_Back").attr('style', 'background-image: url(../../../Img/Btn/Back.png); background-repeat: no-repeat; background-size: 14px 14px; background-position: 6px 8px;');
    else
        $("#Btn_Back").attr('style', 'background-image: url(../../../Img/Btn/Back.png); background-repeat: no-repeat; background-size: 13px 13px; background-position: 1px 6px;');

    //导入按钮icon
    if ($("#Btn_Imp").height() > 25)
        $("#Btn_Imp").attr('style', 'background-image: url(../../../Img/Btn/Imp.png); background-repeat: no-repeat; background-size: 14px 14px; background-position: 6px 8px;');
    else
        $("#Btn_Imp").attr('style', 'background-image: url(../../../Img/Btn/Imp.png); background-repeat: no-repeat; background-size: 13px 13px; background-position: 1px 6px;');

    //导出按钮icon
    if ($("#Btn_Exp").height() > 25)
        $("#Btn_Exp").attr('style', 'background-image: url(../../../Img/Btn/Exp.png); background-repeat: no-repeat; background-size: 14px 14px; background-position: 6px 8px;');
    else
        $("#Btn_Exp").attr('style', 'background-image: url(../../../Img/Btn/Exp.png); background-repeat: no-repeat; background-size: 13px 13px; background-position: 1px 6px;');

    //帮助按钮icon
    if ($("#Btn_Help").height() > 25)
        $("#Btn_Help").attr('style', 'background-image: url(../../../Img/Btn/Help.png); background-repeat: no-repeat; background-size: 14px 14px; background-position: 6px 8px;');
    else
        $("#Btn_Help").attr('style', 'background-image: url(../../../Img/Btn/Help.png); background-repeat: no-repeat; background-size: 13px 13px; background-position: 1px 6px;');

    //高级按钮icon
    if ($("#Btn_Advanced").height() > 25)
        $("#Btn_Advanced").attr('style', 'background-image: url(../../../Img/Btn/Advanced.png); background-repeat: no-repeat; background-size: 14px 14px; background-position: 6px 8px;');
    else
        $("#Btn_Advanced").attr('style', 'background-image: url(../../../Img/Btn/Advanced.png); background-repeat: no-repeat; background-size: 13px 13px; background-position: 1px 6px;');

    //批处理按钮
    $("#Btn_Batch").attr('style', 'background-image: url(../../../Img/Btn/Batch.png); background-repeat: no-repeat; background-size: 14px 14px; background-position: 6px 8px;');

    if ($("#Btn_New").height() > 25) {
        $("#Btn_New").attr('style', 'background-image: url(../../../Img/Btn/New.png); background-repeat: no-repeat; background-size: 14px 14px; background-position: 6px 8px;');
        $("#Btn_New").attr('height', 50);
    }
    else {
        $("#Btn_New").attr('style', 'background-image: url(../../../Img/Btn/New.png); background-repeat: no-repeat; background-size: 13px 13px; background-position: 1px 6px;');
        $("#Btn_New").attr('height', 50);
    }*/

    $("#Btn_Save").addClass("cc-btn-tab btn-save");
    $("#Btn_SaveAs").addClass("cc-btn-tab btn-save");

    $("#Btn_Batch").addClass("cc-btn-tab btn-batch");
    $("#Btn_Delete").addClass("cc-btn-tab btn-delete");
    $("#Btn_Back").addClass("cc-btn-tab btn-back");
    $("#Btn_Imp").addClass("cc-btn-tab btn-imp");
    $("#Btn_Exp").addClass("cc-btn-tab btn-exp");
    $("#Btn_Help").addClass("cc-btn-tab btn-hlep");
    $("#Btn_Advanced").addClass("cc-btn-tab btn-advanced");
    $("#Btn_New").addClass("cc-btn-tab btn-new");
    $("#Btn_Search").addClass("cc-btn-tab btn-search");
    $("#Btn_App").addClass("cc-btn-tab btn_app");


}

style = ''
//动态添加新风格  
function SetCSS() {
    //处理 ToolBar.
    //body下添加一个父Div
    var div = document.createElement('div');
    $(div).attr('class', 'cs-content-box');
    $('#bar').wrap(div);

    $('fieldset').wrapAll(div);
    //帮助ul风格
    div = document.createElement('div');
    $(div).attr('class', 'cs-help');
    var ulID = $('ul').attr('id');
    if (ulID != "ul1")
        $('ul').wrap(div);

    $.each($("legend"), function (i, obj) {

        var _html = $(obj).html();
        if (obj.id.indexOf("help") != -1) {

            $(obj).html("");

            var helpImg = basePath + "/WF/Admin/Img/Help.png";

            var div2 = "<div id='help1' class='help-title'> <img src='" + helpImg + "' alt='帮助' class='ico-help' />" + _html + " </div>";
            $($(obj).parent().find("ul").parent()[0]).append(div2);
        }
    })

    //bar风格
    $('#bar').attr('class', 'cs-tr cs-bar');
    ////删除重复的说明标题
    //var leg = $("legend");
    //for (var i = 0; i < leg.length; i++) {
    //    if (leg.eq(i).text() == "说明")
    //        leg.eq(i).remove();
    //}
}
//设置帮助页面内容 
function SetHelpPage() {
    return;

    var legends = $("legend#help");
    //隐藏所有兄弟级元素
    legends.siblings().hide();
    ////增加font  以便监听单击
    //var font = document.createElement('font');
    //$(font).attr('id', 'cl');
    //legends.wrap('#cl');
    //legends.wrap(font);
    $("font").on("click", function () {
        alert("1234");
        legends.siblings().show();

    });
}
function showPage() {
    var legends = $("legend#help");
    //隐藏所有兄弟级元素
    legends.siblings().show();
}
function HelpDiv() {

    $("form").find("div").each(function () {
        if (this.className.toLowerCase() == "help") {

            //var msg = "请输入SQL语句,支持ccbpm的表达式.";
            //  this.placeholder = msg;
            //    this.value = FormatSQL(this.value);

            this.css('color', 'Gray');
            this.css('display', 'none');

            //alert(this.id);
        }
    });
}

//设置  class="HelpImg" 的图片 点击直接可以全屏放大打开.  
function SetHelpImg() {

    $(function () {

        $(".HelpImg").click(function () {
            var _this = $(this);//将当前的pimg元素作为_this传入函数  
            imgShow("#outerdiv", "#innerdiv", "#bigimg", _this);
        });

        $(".Help").click(function () {
            var _this = $(this);//将当前的pimg元素作为_this传入函数  
            imgShow("#outerdiv", "#innerdiv", "#bigimg", _this);
        });
    });

    function imgShow(outerdiv, innerdiv, bigimg, _this) {
        var src = _this.attr("src");//获取当前点击的pimg元素中的src属性  
        $(bigimg).attr("src", src);//设置#bigimg元素的src属性  

        /*获取当前点击图片的真实大小，并显示弹出层及大图*/
        $("<img/>").attr("src", src).load(function () {
            var windowW = $(window).width();//获取当前窗口宽度  
            var windowH = $(window).height();//获取当前窗口高度  
            var realWidth = this.width;//获取图片真实宽度  
            var realHeight = this.height;//获取图片真实高度  
            var imgWidth, imgHeight;
            var scale = 0.8;//缩放尺寸，当图片真实宽度和高度大于窗口宽度和高度时进行缩放  

            if (realHeight > windowH * scale) {//判断图片高度  
                imgHeight = windowH * scale;//如大于窗口高度，图片高度进行缩放  
                imgWidth = imgHeight / realHeight * realWidth;//等比例缩放宽度  
                if (imgWidth > windowW * scale) {//如宽度扔大于窗口宽度  
                    imgWidth = windowW * scale;//再对宽度进行缩放  
                }
            } else if (realWidth > windowW * scale) {//如图片高度合适，判断图片宽度  
                imgWidth = windowW * scale;//如大于窗口宽度，图片宽度进行缩放  
                imgHeight = imgWidth / realWidth * realHeight;//等比例缩放高度  
            } else {//如果图片真实高度和宽度都符合要求，高宽不变  
                imgWidth = realWidth;
                imgHeight = realHeight;
            }
            $(bigimg).css("width", imgWidth);//以最终的宽度对图片缩放  

            var w = (windowW - imgWidth) / 2;//计算图片与窗口左边距  
            var h = (windowH - imgHeight) / 2;//计算图片与窗口上边距  
            $(innerdiv).css({ "top": h, "left": w });//设置#innerdiv的top和left属性  
            $(outerdiv).fadeIn("fast");//淡入显示#outerdiv及.pimg  
        });

        $(outerdiv).click(function () {//再次点击淡出消失弹出层  
            $(this).fadeOut("fast");
        });
    }

}
//加载放大的img容器 
function SetBigImgDiv() {
    var divs = "<div id='outerdiv' style='position:fixed;top:0;left:0;background:rgba(0,0,0,0.7);z-index:2;width:100%;height:100%;display:none;'><div id='innerdiv' style='position:absolute;'><img id='bigimg' style='border:5px solid #fff;' src=''/></div ></div >";
    $(".cs-content-box").append(divs);
}
//设置SQL脚本编辑器. 如果遇到 textarea 的className=SQL的，我们就默认为该文本框是
//要sql的格式，就给他增加上sql的模式.
function CheckSQLTextArea() {

    var isLoadSQLJS = false;

    $("form").find("input,textarea").each(function () {

        if (this.className == "SQL") {

            var msg = "请输入SQL语句,支持ccbpm的表达式.";
            this.placeholder = msg;
            this.value = FormatSQL(this.value);

        }
    });
}

function LoadCodeMirror(id) {
    var editor = CodeMirror.fromTextArea(document.getElementById(id), {
        lineNumbers: true,
        styleActiveLine: true,
        matchBrackets: true,
        theme: "eclipse"
    });

}

//显示表单的字段, 用于拼写SQL.
function ShowFrmFields(divID) {

    var frmID = GetQueryString("FK_MapData");
    if (frmID == null)
        frmID = GetQueryString("FrmID");

    if (frmID == null)
        return;

    if (divID == undefined)
        divID = "FrmFileds";

    var div = $("#" + divID);
    if (div.length == 0)
        return;

    var mapAttrs = new Entities("BP.Sys.MapAttrs");
    mapAttrs.Retrieve("FK_MapData", frmID, "GroupID,Idx");

    var html = "";
    html + "表单字段:";
    html += "<table>";
    html += "<caption>表单字段</caption>";

    html += "<tr>";
    html += "<th># </th>";
    html += "<th>名称</th>";
    html += "<th>字段</th>";
    html += "<th>类型</th>";
    html += "</tr>";

    var idx = 0;
    for (var i = 0; i < mapAttrs.length; i++) {
        var en = mapAttrs[i];

        if (en.MyDataType >= 8)
            continue;

        if (en.KeyOfEn == 'OID' || en.KeyOfEn == 'Rec' || en.KeyOfEn == 'FID' || en.KeyOfEn == 'RefPK')
            continue;

        idx++;

        html += "<tr>";
        html += "<td>" + idx + "</td>";
        html += "<td>" + en.Name + "</td>";
        html += "<td>" + en.KeyOfEn + "</td>";
        html += "<td>" + GetDBType(en.MyDataType) + "</td>";
        html += "</tr>";
    }
    html += "</table>";
    div.html(html);

    function GetDBType(type) {
        if (type == 1)
            return "String";
        if (type == 2 || type == 3 || type == 4)
            return "数值";

        if (type == 6)
            return "日期";

        if (type == 7)
            return "日期时间";

        if (type == 8)
            return "金额";
        return "未知";
    }
}





function CheckIsSQL(sql) {

    if (sql == '' || sql == null)
        return false;

    if (sql.replace(/(^\s*)/g, "").toUpperCase().indexOf('SELECT') == -1)
        return false;
    return true;
}

//格式化SQL, 原来的 ~修改为 '
function FormatSQL(sql) {
    sql = sql.replace(/~/g, "'");
    return sql;
}


//让所有具有
function initToggle() {

    var legends = document.getElementsByTagName('legend');
    return;


    for (var i = 0, len = legends.length; i < len; i++) {

        var en = legends[i];

        var lengID = en.id + en.name;

        if (lengID.indexOf('help') == -1)
            continue;
        //if (en.innerHTML) {
        //    en.innerHTML = "<label>" + en.innerHTML + "</label>";
        //} else {
        //    en.innerHTML = "<label>说明</label>";
        //}

        en.onclick = function () {

            // 绑定事件
            for (var j = 0, ln = this.parentElement.childNodes.length; j < ln; j++) {

                var dtl = this.parentElement.childNodes[j];

                if (dtl.style.display === 'none') {
                    dtl.display = 'block';
                } else {
                    dtl.display = 'none';
                }
                continue;


                var nodeName = this.parentElement.childNodes[j].nodeName;
                alert(nodeName);

                if (nodeName && nodeName.toUpperCase() ===
                    'TABLE') {//兼容浏览器,有的浏览器childNodes的个数不同
                    var tbl = this.parentElement.childNodes[j];
                    if (tbl.style.display === 'none') {
                        tbl.style.display = 'block';
                    } else {
                        tbl.style.display = 'none';
                    }
                }
            }
        }
    }
}
//document.onreadystatechange = function () { //页面加载完后，注册事件
//    if (document.readyState == "complete") {
//        initToggle();
//    }
//}

