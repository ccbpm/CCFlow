
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

    var sql1 = "SELECT No,Name,FK_Menu,ParentNo,Url,Tag1,Tag2,Tag3,WebPath,Icon,Idx ";
    sql1 += " FROM v_gpm_empmenu ";
    sql1 += " WHERE FK_Emp = '" + webUser.No + "' ";
    sql1 += " AND MenuType = '3' ";
    sql1 += " AND FK_App = '" + appNo + "' ";
    sql1 += " UNION ";  //加入不需要权限控制的菜单.
    sql1 += "SELECT No,Name, No as FK_Menu,ParentNo,Url,Tag1,Tag2,Tag3,WebPath,Icon,Idx";
    sql1 += " FROM GPM_Menu ";
    sql1 += " WHERE MenuCtrlWay=1 ";
    sql1 += " AND MenuType = '3' ";
    sql1 += " AND FK_App = '" + appNo + "' ORDER BY Idx ";

    var dirs = DBAccess.RunSQLReturnTable(sql1);

    var sql2 = "SELECT No,Name,FK_Menu,ParentNo,Url,Tag1,Tag2,Tag3,WebPath,Icon,Idx ";
    sql2 += " FROM v_gpm_empmenu ";
    sql2 += " WHERE FK_Emp = '" + webUser.No + "'";
    sql2 += " AND MenuType = '4' ";
    sql2 += " AND FK_App = '" + appNo + "' ";
    sql2 += " UNION ";  //加入不需要权限控制的菜单.
    sql2 += "SELECT No,Name, No as FK_Menu,ParentNo,Url,Tag1,Tag2,Tag3,WebPath,Icon,Idx ";
    sql2 += " FROM GPM_Menu "; //加入不需要权限控制的菜单.
    sql2 += " WHERE MenuCtrlWay=1 ";
    sql2 += " AND MenuType = '4' ";
    sql2 += " AND FK_App = '" + appNo + "' ORDER BY Idx ";

    var menus = DBAccess.RunSQLReturnTable(sql2);

    var firstIcon = "icon-folder";
    var secondIcon = "icon-book-open";
    $.each(dirs, function (i, dir) {

        var icon = dir.Icon;
        if (icon == undefined)
            icon = dir.ICON;

        var menulist = '';
        if (icon != null && icon != "")
            firstIcon = icon;

        var url = dir.Url;
        if (url == undefined)
            url = dir.URL;

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

                var url = menu.Url;
                if (url == undefined)
                    url = menu.URL;

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
