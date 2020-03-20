
/*
1. 引用该文件之前需要首先引用 config.js的变量定义.
2. 该文件不要被个性化修改, 最新的版本都在 CCOA 里面.
*/

function InitLeftMenuComm() {

    webUser = new WebUser(appNo);
    if (webUser.No == null) {
        window.location.href = 'Login.htm';
        return;
    }

    //通过API获得两个数据源.
    var data = GPM_GenerMenumsDB();
    var dirs = data["Dirs"]; //获得目录.
    var menus = data["Menus"]; //获得菜单.

    var firstIcon = "icon-folder";
    var secondIcon = "icon-book-open";
    $.each(dirs, function (i, dir) {

        var icon = dir.Icon;
        if (icon == undefined)
            icon = dir.ICON;

        var menulist = '';
        if (icon != null && icon != "")
            firstIcon = icon;

        var url = dir.UrlExt;
        if (url == undefined)
            url = dir.URLEXT;

        var name = dir.Name;
        if (name == undefined)
            name = dir.NAME;

        var tag1 = dir.Tag1;
        if (tag1 == undefined)
            tag1 = dir.TAG1;

        var tag2 = dir.Tag2;
        if (tag2 == undefined)
            tag2 = dir.TAG2;

        menulist += '<li><a class="J_menuItem"><i class="' + firstIcon + '"></i> <span class="' + tag1 + '">' + name + '</span><span class="' + tag2 + '"></span></a>';
        menulist += '<ul class="nav nav-second-level collapse">';

        var dirNo = dir.No;
        if (dirNo == undefined)
            dirNo = dir.NO;

        $.each(menus, function (j, menu) {

            var parentNo = menu.ParentNo;
            if (parentNo == undefined)
                parentNo = menu.PARENTNO;

            if (parentNo == dirNo) {

                var icon = menu.Icon;
                if (icon == undefined)
                    icon = menu.ICON;
                if (icon != null && icon != "")
                    secondIcon = icon;

                var url = menu.UrlExt;
                if (url == undefined)
                    url = menu.URLEXT;

                var name = menu.Name;
                if (name == undefined)
                    name = menu.NAME;

                menulist += "<li><a href=\"javascript:GotoUrl('" + url + "');\"  class='J_iframe'  ><i class='" + secondIcon + "'></i>" + name + "</a></li>";
            }

        });
        menulist += '</ul>';
        $('#side-menu').append(menulist);
    });
}

function GotoUrl(url) {
    $('#J_iframe').attr('src', url);
}


