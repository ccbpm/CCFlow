//从MyFlow.htm 拿过来的
$(function () {
    initPageParam(); //初始化参数
    initBar(); //工具栏.
    GenerWorkNode(); //表单数据.
    Common.MaxLengthError();
    if ($("#Message").html() == "") {
        $(".Message").hide();
    }
    if (parent != null && parent.document.getElementById('MainFrames') != undefined) {
        //计算高度，展示滚动条
        var height = $(parent.document.getElementById('MainFrames')).height() - 110;
        $('#topContentDiv').height(height);

        $(window).resize(function () {
            $("#CCForm").height($(window).height() - 150 + "px").css("overflow-y", "auto").css("scrollbar-face-color", "#fff");;
        });
    }

    $('#btnCloseMsg').bind('click', function () {
        $('.Message').hide();
    });
})