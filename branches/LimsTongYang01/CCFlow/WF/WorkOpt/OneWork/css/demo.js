$(document).ready(function () {
    $(".side ul li").hover(function () {
        $(this).find(".sidebox").stop().animate({ "width": "150px" }, 50).css({ "opacity": "1", "filter": "Alpha(opacity=100)", "background": "#ae1c1c" })
    }, function () {
        $(this).find(".sidebox").stop().animate({ "width": "54px" }, 50).css({ "opacity": "0.8", "filter": "Alpha(opacity=80)", "background": "#000" })
    });

});
//�ص�����
function goTop() {
    $('html,body').animate({ 'scrollTop': 0 }, 200); //���ض�����ʱ�䣬ԽС�����ٶ�Խ��~
}
