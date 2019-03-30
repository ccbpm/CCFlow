//定义多语言.
$(".lang").each(function (index, item) {
    var key = $(item).attr("data-key");
    var val = "";
    $.each(window.lang, function (i, n) {
        if (i == key) {
            val = n;
            return false;
        }

    });

    if (val.length > 0) {
        $(item).text(val);
    }
});