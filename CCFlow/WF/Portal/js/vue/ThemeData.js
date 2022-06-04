var themeData = {
    defaultTheme: {
        logo: '#226A62',
        selected: '#009688',
        header: '#226A62',
        sideBar: '#226A62',
        navBar: '#fff',
        selectedMenu: '#009688',
        formColor: '#5FB878',
        alias: '默认主题'
    },
    pink: {
        logo: '#50314F',
        selected: '#7A4D7B',
        header: '#50314F',
        sideBar: '#50314F',
        navBar: '#fff',
        selectedMenu: '#7A4D7B',
        formColor: '#5FB878',
        alias: '紫色迷情'
    },
    black: {
        logo: '#20222A',
        selected: '#3e3e3e',
        header: '#20222A',
        sideBar: '#20222A',
        navBar: '#fff',
        selectedMenu: '#3e3e3e',
        alias: '典雅黑'
    },

    blue: {
        logo: 'rgb(24, 144, 255)',
        selected: 'rgb(57, 158, 253)',
        header: 'rgb(24, 144, 255)',
        sideBar: 'linear-gradient(90deg, rgb(0, 108, 255), rgb(57, 158, 253))',
        navBar: 'rgb(57, 158, 253)',
        selectedMenu: 'rgb(57, 158, 253)',
        formColor: 'rgb(57, 158, 253)',
        alias: '海之蓝'
    },
    rynn: {
        logo: 'rgb(24, 144, 255)',
        selected: 'rgba(51, 125, 254, 1)', //选择系统的颜色
        header: 'rgba(28, 90, 201, 1)',
        sideBar: 'rgba(28, 90, 201, 1)',
        navBar: 'rgba(28, 90, 201, 1)',
        selectedMenu: 'rgba(51, 125, 254, 1)', //选择菜单的颜色
        alias: '深之蓝'
    },
    pro: {
        logo: 'rgb(32, 34, 42)',
        selected: 'rgb(230, 0, 18)',
        header: 'rgb(32, 34, 42)',
        sideBar: 'rgb(32, 34, 42)',
        navBar: 'rgb(255,0,24)',
        selectedMenu: 'rgb(230, 0, 18)',
        alias: '红黑经典'
    },
    classical: {
        logo: '#2E241B',
        sideBar: '#2E241B',
        selected: '#A48566',
        header: '#2E241B',
        navBar: '#A48566',
        selectedMenu: '#A48566',
        alias: '古典'
    }
}

function chooseTheme(color) {
    var defTheme = getPortalConfigByKey("DefaultTheme", 'defaultTheme');
    if (!color) color = defTheme;
    try {
        var styleScope = document.getElementById("theme-data");
        var layoutType = localStorage.getItem('classicalLayout') || 1;

        var html = "";
        html = "\n .layui-nav-tree .layui-this,\n .layui-nav-tree .layui-this>a,\n .layui-nav-tree .layui-nav-child dd.layui-this,\n .layui-nav-tree .layui-nav-child dd.layui-this a{\n background-color:".concat(themeData[color].selectedMenu, " !important;\n  ").concat(parseInt(layoutType) === 0 ? 'color: #fff !important' : '', "\n                                }\n                                .layui-header .layui-nav .layui-nav-more{\n                                    border-top-color: ").concat(themeData[color].header !== 'white' && themeData[color].header !== '#fff' ? '#fff' : '#333', " !important\n                                }\n                                .layui-header .layui-nav .layui-nav-mored{\n                                    border-color: transparent transparent ").concat(themeData[color].header !== 'white' && themeData[color].header !== '#fff' ? '#fff' : '#333', " !important\n                                }\n                                .g-admin-layout .layui-header .layui-nav .layui-this:after, .g-admin-layout .layui-header .layui-nav-bar,\n                                .g-admin-layout .layui-header .layui-nav-bar {\n                                    height: 2px;\n                                    background-color: ").concat("\n                                }\n                                .g-admin-layout .layui-header a,\n                                .g-admin-layout .layui-header a:hover,\n                                .g-admin-layout .layui-header a cite{\n                                    color: ").concat(themeData[color].header !== 'white' && themeData[color].header !== '#fff' ? '#fff' : '#333', "\n                                }\n                                .drop-down a{\n                                    color: #2d2d2d\n                                }\n                                .layui-nav-tree .layui-nav-bar{\n                                    background-color: ").concat("\n                                }\n                                .sideMenuBar{\n                                    background: ").concat(themeData[color].sideBar, " !important\n                                }\n                                ");
        if (parseInt(layoutType) === 0) {
            html += "\n .layui-side-menu .layui-nav .layui-nav-item a:hover{\n  background-color: ".concat(themeData[color].selectedMenu, " !important;\n  color:#f2f2f2 !important;\n }\n .layui-side-menu .layui-nav .layui-nav-item a:hover span{\n border-top-color: rgba(255,255,255,0.7) !important\n }\n\n .selected-top-menu{\n background-color: ").concat(themeData[color].selected, " !important;\n  color: #f2f2f2;\n   }\n   .layui-logo{\n background-color: #fff !important\n  }\n  .layui-nav-more{\n  border-top-color: #5f626e44 !important;\n   }\n  ");
        } else {
            html += "\n .layui-logo{\n                                            background-color: ".concat(themeData[color].logo, " !important\n                                            \n                                        }\n                                        .layui-nav-more{\n                                            border-top-color: #ffffff99;\n                                        }\n                                    ");
        }
        html = DealText(html);
        styleScope.innerHTML = html;

        document.getElementsByClassName("layui-side-menu")[0].style.background = DealText(themeData[color].sideBar);
        document.getElementsByClassName("layui-header")[0].style.backgroundColor = DealText(themeData[color].header);
        localStorage.setItem("themeColor", DealText( color) );
        localStorage.setItem("themeColorInfo", JSON.stringify(themeData[color]));
        document.documentElement.style.setProperty('--sub-menu-hover', DealText(themeData[color].selectedMenu));
    } catch (e) {
        console.log(e)
        console.log("设置主题失败")
    }

}