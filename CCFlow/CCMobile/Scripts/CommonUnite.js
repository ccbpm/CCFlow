Application = {}

DataFactory = function () {
    this.common = new commonUnite;
}

jQuery(function ($) {
    Application = new DataFactory();
});

//公共方法
commonUnite = function () {
    //sArgName表示要获取哪个参数的值
    this.getArgsFromHref = function (sArgName) {
        var sHref = GetHrefUrl();
        var args = sHref.split("?");
        var retval = "";
        if (args[0] == sHref) /*参数为空*/
        {
            return retval; /*无需做任何处理*/
        }
        var str = args[1];
        args = str.split("&");
        for (var i = 0; i < args.length; i++) {
            str = args[i];
            var arg = str.split("=");
            if (arg.length <= 1) continue;
            if (arg[0] == sArgName) retval = arg[1];
        }
        return retval;
    }
}