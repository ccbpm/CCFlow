$(function () {

    var webuser = new WebUser();
    var typeid = GetQueryString("typeid");
    var urldata = [
        { url: 'Default.htm', text: '主页' },
        { url: 'TreeEdit.htm', text: '编辑' },
        { url: 'TreeView.htm', text: '预览' },
        { url: 'ShareTo.htm', text: '族谱' },
        { url: 'CheckRZ.htm', text: '审核日志' }
    ]
    var html = "";
   // html += " 您好: " + webuser.Name;
    for (var i = 0; i < urldata.length; i++) {
        var ud = urldata[i];
        //console.log(en)
        if (typeid == i||(typeid==null&&i==0))
            html += "<a href='" + ud.url + "?typeid=" + i+ "' class='layui-btn layui-btn-danger layui-btn-sm'>" + ud.text + "</a>  ";
        else
            html += "<a href='" + ud.url + "?typeid=" + i + "' class='layui-btn layui-btn-sm'>" + ud.text + "</a>  ";
    }

    $("#toolbar").html(html);

});
