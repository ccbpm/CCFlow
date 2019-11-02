/*
 * admin.js文件说明
 *  1. 为后台管理的页面增加的文件, 用于可以自动化的改变后台设置的一些特效.
 *  2. 解决帮助的统一风格问题.
 *  3. 目前增加了两个模式的操作。 
 *     3.1 fieldset 的 legend 的ID 包含 help 就是帮助内容页面. 出现的效果是加载后，是隐藏的，用户点击文字就要显示里面的内容
 *     3.2 对于textArea 如果在 class='SQL' 系统就认为是表达式sql文本输入框.
 *  4. 代国强来完善这两个方法，参考 D:\ccflow\CCFlow\WF\CCBill\Admin\MethodDoc.htm 页面。
 */

$(document).ready(function () {

    //设置自动提示.
    initToggle();

    //设置SQL脚本编辑器.
    CheckSQLTextArea();


})

//设置SQL脚本编辑器. 如果遇到 textarea 的className=SQL的，我们就默认为该文本框是
//要sql的格式，就给他增加上sql的模式.
function CheckSQLTextArea() {

    var isLoadSQLJS = false;

    $("form").find("input,textarea").each(function () {

        if (this.className == "SQL") {
            //alert(this.id);
           // alert(basePath);


            //开始为这个文本框设置sql模式的显示.
            if (isLoadSQLJS == false) {
                /*加载相关的JS*/
                $.getScript(basePath + '/Scripts/codemirror/doc/docs.css', function () {
                });

                $.getScript(basePath + '/Scripts/codemirror/lib/codemirror.css', function () {
                });

                $.getScript(basePath + '/Scripts/codemirror/theme/eclipse.css', function () {
                });

                //加载js.
                $.getScript(basePath + '/Scripts/codemirror/lib/codemirror.js', function () {
                    $.getScript(basePath + '/Scripts/codemirror/mode/javascript/javascript.js', function () {
                        $.getScript(basePath + '/Scripts/codemirror/addon/selection/active-line.js', function () {
                            $.getScript(basePath + '/Scripts/codemirror/addon/edit/matchbrackets.js', function () {
                                isLoadSQLJS = true;
                                LoadCodeMirror(this.id);
                                alert('ss');
                            });
                        });
                    });
                });
                isLoadSQLJS = true;
            } else {
                LoadCodeMirror(this.id);
            }
        }
    });
}

function LoadCodeMirror(id)
{
    var editor = CodeMirror.fromTextArea(document.getElementById(id), {
        lineNumbers: true,
        styleActiveLine: true,
        matchBrackets: true,
        theme: "eclipse"
    });

}


//< link rel = "stylesheet" href = "../../Scripts/codemirror/doc/docs.css" />
//    <link rel="stylesheet" href="../../Scripts/codemirror/lib/codemirror.css" />
//    <link rel="stylesheet" href="../../Scripts/codemirror/theme/eclipse.css" />
//    <link rel="stylesheet" href="../../Scripts/codemirror/theme/elegant.css" />
//    <link rel="stylesheet" href="../../Scripts/codemirror/theme/erlang-dark.css" />
//    <link rel="stylesheet" href="../../Scripts/codemirror/theme/idea.css" />
//    <script src="../../Scripts/codemirror/lib/codemirror.js"></script>
//    <script src="../../Scripts/codemirror/mode/javascript/javascript.js" type="text/javascript"></script>
//    <script src="../../Scripts/codemirror/addon/selection/active-line.js" type="text/javascript"></script>
//    <script src="../../Scripts/codemirror/addon/edit/matchbrackets.js" type="text/javascript"></script>


function CheckIsSQL(sql) {
    if (sql == '' || sql == null)
        return false;

    if (sql.toUpperCase().indexOf('SELECT') == -1)
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

    for (var i = 0, len = legends.length; i < len; i++) {

        debugger
        var en = legends[i];
        if (en.id.indexOf('help') == -1)
            continue;

        en.innerHTML = "<font color=green><b><img src='" + basePath + "/WF/Img/Help.png' >" + en.innerHTML + "</b></font>";
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

