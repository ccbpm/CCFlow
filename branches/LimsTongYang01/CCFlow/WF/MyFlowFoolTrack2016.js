//从MyFlow.htm 拿过来的
$(function () {
    initPageParam(); //初始化参数
    initBar(); //工具栏.
    GenerWorkNode(); //表单数据.
    Common.MaxLengthError();
    $('[name=showCol]').bind('change', function (obj) {
        if (obj.target.value == "8") {
            Col4To8();
        } else {
            Col8To4();
        }
    });
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
    function movetb() {
        var move;
        $("#nav").css("top", top);
    }
    nav.init();
    $(".navbars").css("margin-top", ($(window).height() - $(".navbars").height()) / 2);
    if (navigator.userAgent.indexOf('Chrome') >= 0) {
        $(".navbars i").click(function () {
            $("#nav").removeClass("s");
        });
        $("#nav").bind("mouseover", function () {
            $("#nav").addClass("s");
            $("#nav").css("top", "30px");
        });
    }

    $('#btnCloseMsg').bind('click', function () {
        $('.Message').hide();
    });
})