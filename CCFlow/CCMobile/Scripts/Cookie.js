///js操作cookie

//两个参数，一个是cookie的名子，一个是值
function SetCookie(name,value)
{
    var Days = 30; //此 cookie 将被保存 30 天
    var exp  = new Date();    //new Date("December 31, 9998");
    exp.setTime(exp.getTime() + Days*24*60*60*1000);
    document.cookie = name + "="+ escape (value) + ";expires=" + exp.toGMTString();
}

//取cookies函数
function GetCookie(name)        
{
    var arr = document.cookie.match(new RegExp("(^| )"+name+"=([^;]*)(;|$)"));
     if(arr != null) return unescape(arr[2]); return null;
}

//删除cookie
function DelCookie(name)
{
    var exp = new Date();
    exp.setTime(exp.getTime() - 1);
    var cval=getCookie(name);
    if(cval!=null) document.cookie= name + "="+cval+";expires="+exp.toGMTString();
}

function GetCookieParamValue(cke, pname) {
    var cs = cke.split('&');
    var val;

    $.each(cs, function () {
        if (this.split('=')[0] == pname) {
            val = this.split('=')[1];
        }
    });

    return val;
}