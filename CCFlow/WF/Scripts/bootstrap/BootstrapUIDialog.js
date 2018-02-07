function OpenBootStrapModal(url, iframeId, dlgTitle, dlgWidth, dlgHeight, dlgIcon, showBtns, okBtnFunc, okBtnFuncArgs, dlgClosedFunc) {
    ///<summary>使用EasyUiDialog打开一个页面</summary>
    ///<param name="url" type="String">页面链接</param>
    ///<param name="iframeId" type="String">嵌套url页面的iframe的id，在okBtnFunc中，可以通过document.getElementById('eudlgframe').contentWindow获取该页面，然后直接调用该页面的方法，比如获取选中值</param>
    ///<param name="dlgTitle" type="String">Dialog标题</param>
    ///<param name="dlgWidth" type="int">Dialog宽度</param>
    ///<param name="dlgHeight" type="int">Dialog高度</param>
    ///<param name="dlgIcon" type="String">Dialog图标，必须是一个样式class</param>
    ///<param name="showBtns" type="Boolean">Dialog下方是否显示“确定”“取消”按钮，如果显示，则后面的okBtnFunc参数要填写</param>
    ///<param name="okBtnFunc" type="Function">点击“确定”按钮调用的方法</param>
    ///<param name="okBtnFuncArgs" type="Object">okBtnFunc方法使用的参数</param>
    ///<param name="dlgClosedFunc" type="Function">窗体关闭调用的方法（注意：此方法中不能再调用dialog中页面的内容）</param>

    var dlg = $('#bootStrapdlg');
    var isTheFirst;

    if (dlg.length == 0) {
        isTheFirst = true;
        var divDom = $("<div class='modal fade' data-backdrop='static'></div>");
        divDom.attr("id", "bootStrapdlg");
        $(document.body).append(divDom);
        dlg = $('#bootStrapdlg');
    }

    if (dlg.length == 1) {
        dlg.children().remove();
        dlg.empty();
        dlg.html("");
    }

    var modalDialog = $("<div class='modal-dialog'></div>");
    modalDialog.width(dlgWidth).height(dlgHeight);
    //容器
    var modalContent = $("<div class='modal-content' style='border-radius:0px;text-align:left;'></div>");
    modalDialog.append(modalContent);
    //标题
    var modalHead = $("<div class='modal-header' style='padding: 5px;'></div>");
    //关闭按钮
    var btnClose = $("<button type='button' style='color:white;float: right;background: transparent;border: none;' data-dismiss='modal' aria-hidden='true'>&times;</button>");
    //标题
    var titleHead = $("<h4 class='modal-title'></h4>");

    titleHead.text(dlgTitle);
    modalHead.append(btnClose);
    modalHead.append(titleHead);

    //添加标题块
    modalContent.append(modalHead);

    //body块
    var modalBody = $("<div class='modal-body' style='margin:5px;padding:0px;'></div>");
    var iframeWidth = dlgWidth - 45;

    var iFrame = $("<iframe></iframe>");
    iFrame.attr("id", iframeId);
    iFrame.attr("name", iframeId);
    iFrame.attr("src", url);
    iFrame.attr("src", url);
    iFrame.css("width", iframeWidth + "px").css("height", dlgHeight + "px").css("border", "0px");

    modalBody.append(iFrame);
    //添加内容块
    modalContent.append(modalBody);
    //结束
    dlg.append(modalDialog);

    if (typeof window.doCloseDialog != "function") {
        window.doCloseDialog = function () {
            $('#bootStrapdlg').modal('hide');
        };
        // 当弹出框未获得焦点时触发
        $(document).bind("keyup", function (e) {
            e = e || window.event;
            var key = e.keyCode || e.which || e.charCode;
            if (key == 27) {
                if (typeof doCloseDialog === 'function') {
                    doCloseDialog.call();
                }
            }
        });
    }

    if (showBtns && okBtnFunc) {
        var modalFooter = $("<div class='modal-footer'></div>");
        var footerClose = $("<button type='button' class='btn' data-dismiss='modal'>关闭</button>");
        footerClose.click(function () {
            $('#bootStrapdlg').modal('hide');

        });
        var footerOK = $("<button type='button' class='btn'>确定</button>");
        footerOK.click(function () {
            if (okBtnFunc(okBtnFuncArgs) == false) {
                return;
            }
            $('#bootStrapdlg').modal('hide');
        });
        //添加确定和关闭按钮
        modalFooter.append(footerClose);
        modalFooter.append(footerOK);

        //添加底层脚本
        modalContent.append(modalFooter);
    }

    //关闭事件
    $('#bootStrapdlg').off('hide.bs.modal');
    $('#bootStrapdlg').on('hide.bs.modal', function () {
        /*防止缓存，切换页面不能显示问题*/
        $("#bootStrapdlg").remove();
        // 关闭时清空edit状态为add
        dlg.find("iframe").attr('src', '');
        if (dlgClosedFunc) {
            dlgClosedFunc();
        }
    });

    $('#bootStrapdlg').modal().show();
    return "bootStrapdlg";
}

//关闭弹出窗
function CloseBootstrapDialog() {
    $('#bootStrapdlg').modal('hide');
}
 