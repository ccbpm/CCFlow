
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

    var userNo = webUser.No;

    //通过API获得两个数据源.
    var sql1 = "SELECT No,Name,FK_Menu,MenuType,ParentNo,Url,UrlExt,Tag1,Tag2,Tag3,WebPath,Icon,Idx ";
    sql1 += " FROM v_gpm_empmenu ";
    sql1 += " WHERE FK_Emp = '" + webUser.No + "' ";
    sql1 += " AND ParentNo = '" + ParentNo + "' ";
    sql1 += " AND FK_App = '" + appNo + "' ";
    sql1 += " UNION ";  //加入不需要权限控制的菜单.
    sql1 += "SELECT No,Name, No as FK_Menu,MenuType,ParentNo,Url,UrlExt,Tag1,Tag2,Tag3,WebPath,Icon,Idx";
    sql1 += " FROM GPM_Menu ";
    if (userNo == "admin") {
        sql1 += " WHERE (MenuCtrlWay=1 or MenuCtrlWay=2)";
    }
    else {
        sql1 += " WHERE MenuCtrlWay=1 ";
    }
    
    sql1 += " AND ParentNo = '" + ParentNo + "' ";
    sql1 += " AND FK_App = '" + appNo + "' ORDER BY Idx ";

    var dirs = DBAccess.RunSQLReturnTable(sql1);

    var sql2 = "SELECT No,Name,FK_Menu,MenuType,ParentNo,Url,UrlExt,Tag1,Tag2,Tag3,WebPath,Icon,Idx,openway ";
    sql2 += " FROM v_gpm_empmenu ";
    sql2 += " WHERE FK_Emp = '" + webUser.No + "'";
    sql2 += " AND ParentNo != '" + ParentNo + "'  ";
    sql2 += " AND FK_App = '" + appNo + "' ";
    sql2 += " UNION ";  //加入不需要权限控制的菜单.
    sql2 += "SELECT No,Name, No as FK_Menu,MenuType,ParentNo,Url,UrlExt,Tag1,Tag2,Tag3,WebPath,Icon,Idx,openway ";
    sql2 += " FROM GPM_Menu "; //加入不需要权限控制的菜单.
    if (userNo == "admin") {
        sql2 += " WHERE (MenuCtrlWay=1 or MenuCtrlWay=2)";
    }
    else {
        sql2 += " WHERE MenuCtrlWay=1 ";
    }
    sql2 += " AND ParentNo != '" + ParentNo + "' ";
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

        var url = dir.UrlExt;
        if (url == undefined)
            url = dir.URLEXT;

        var name = dir.Name;
        if (name == undefined)
            name = dir.NAME;

        var no = dir.No;
        if (no == undefined)
            no = dir.NO;

        var menutype = dir.MenuType;
        if (menutype == undefined)
            menutype = MENUTYPE;

        var tag1 = dir.Tag1;
        if (tag1 == undefined)
            tag1 = dir.TAG1;

        var tag2 = dir.Tag2;
        if (tag2 == undefined)
            tag2 = dir.TAG2;

        //  menulist += '<li class='treeview'> <a class="J_menuItem"><i class="' + firstIcon + '"></i> <span class="' + tag1 + '">' + name + '</span><span class="' + tag2 + '"></span></a>';

        //开始增加目录.
        if (menutype == 4) {
            menulist += "<li class='treeview'><a title='" + name + "' href='javascript:' data-href='" + url + "' class='addTabPage'><i class='" + secondIcon + "'></i> <span>" + name + "</span></a>";
        }
        else {
            menulist += "<li class='treeview' >";

            //增加目录名称.
            menulist += "<a title='" + name + "' href='javascript:' data-href='blank' class='addTabPage' data-code='1069793827855179776'>";
            menulist += "<i class='" + firstIcon + "'></i> <span>" + name + "</span>";
            menulist += "<span class='pull-right-container'><i class='fa fa-angle-left pull-right'></i></span></a>";

            //开始增加菜单
            menulist += "<ul class='treeview-menu'>" + chilList(menus, no)+"</ul>";
        }
        menulist += '</li>';
        

        $('#docs').append(menulist);
    });
}
function chilList(menuData, ParentNo) {
    var firstIcon = "icon-folder";
    var secondIcon = "icon-book-open";
    var menulist = '';
    $.each(menuData, function (j, menu) {

        var parentNo = menu.ParentNo;
        if (parentNo == undefined)
            parentNo = menu.PARENTNO;

        var no = menu.No;
        if (no == undefined)
            no = menu.NO;

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

        if (parentNo == ParentNo) {
            //如果有子菜单
            if (menu.MenuType == 3) {
                menulist += "<li class='treeview' >";

                //增加目录名称.
                menulist += "<a title='" + name + "' href='javascript:' data-href='blank' class='addTabPage' data-code='1069793827855179776'>";
                menulist += "<i class='" + firstIcon + "'></i> <span>" + name + "</span>";
                menulist += "<span class='pull-right-container'><i class='fa fa-angle-left pull-right'></i></span></a>";

                //开始增加菜单
                menulist += "<ul class='treeview-menu'>" + chilList(menuData, no) + "</ul>";
            }
            else if (menu.MenuType == 4) {
                if (menu.OpenWay == 0) {
                    menulist += "<li><a title='" + name + "' href='" + url + "' target='_blank'><i class='" + secondIcon + "'></i> <span>" + name + "</span></li>";
                } else {
                    menulist += "<li><a title='" + name + "' href='javascript:' data-href='" + url + "' class='addTabPage'><i class='" + secondIcon + "'></i> <span>" + name + "</span></li>";
                }
            }
        }
    });
    return menulist;
}

function GotoUrl(url) {
    $('#J_iframe').attr('src', url);
}
