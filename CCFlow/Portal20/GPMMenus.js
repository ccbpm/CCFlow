
/*
1. 引用该文件之前需要首先引用 config.js的变量定义.
2. 该文件不要被个性化修改, 最新的版本都在 CCOA 里面.
*/

function InitLeftMenuComm() {

    webUser = new WebUser();
    if (webUser.No == null) {
        window.location.href = 'Login.htm';
        return;
    }

    //通过API获得两个数据源.
    var data = GPM_GenerMenumsDB(appNo);
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

        //  menulist += '<li class='treeview'> <a class="J_menuItem"><i class="' + firstIcon + '"></i> <span class="' + tag1 + '">' + name + '</span><span class="' + tag2 + '"></span></a>';

        //开始增加目录.
        menulist += "<li class='treeview' >";

        //增加目录名称.
        menulist += "<a title='" + name + "' href='javascript:' data-href='blank' class='addTabPage' data-code='1069793827855179776'>";
        menulist += "<i class='" + firstIcon + "'></i> <span>" + name + "</span>";
        menulist += "<span class='pull-right-container'><i class='fa fa-angle-left pull-right'></i></span></a>";

        //开始增加菜单
        menulist += "<ul class='treeview-menu'>";

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


                menulist += "<li class='treeview'>";
                if (menu.OpenWay == 0) {
                    menulist += "<a title='" + name + "' href='" + url + "' target='_blank'>";
                } else {
                    menulist += "<a title='" + name + "' href='javascript:' data-href='" + url + "' class='addTabPage'>";
                }

                menulist += "<i class='" + secondIcon + "'></i> <span>" + name + "</span></a>";
                menulist += "</li>";

                // menulist += "<li><a href=\"javascript:GotoUrl('" + url + "');\"  class='J_iframe'  ><i class='" + secondIcon + "'></i>" + name + "</a></li>";
            }

        });

        menulist += "</ul>"; //结束菜单.
        menulist += '</li>'; //结束目录.

        $('#docs').append(menulist);
    });
}


function GotoUrl(url) {
    $('#J_iframe').attr('src', url);
}
