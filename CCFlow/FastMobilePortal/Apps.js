function GenerDoc(menu) {
    return menu;
}
layui.carousel.render({
    elem: '#test3'
    , width: '100%'
    , height: '120px'
});
new Vue({
    el: '#flow',
    data: {
        flowNodes: [],
        expandAll: false,
        selectedTopMenuIndex: '',
        loadingDialog: false,
        menuTreeData: [], // 目录数据
        subMenuData: [], // 二级目录数据    
        systemNo: GetQueryString("SystemNo")
    },
    watch: {
        expandAll(val) {
            this.expandMenus(val)
        }
    },
    methods: {
        expandAssignMenu: function () {
            var sysNo = GetQueryString('SystemNo') || "";
            if (sysNo == "" || sysNo == "null")
                sysNo = localStorage.getItem('SystemNo');
            var moduleNo = GetQueryString('ModuleNo')
            if (!sysNo) return
            for (var i = 0; i < this.flowNodes.length; i++) {
                var system = this.flowNodes[i]
                if (system.No === sysNo) {
                    system.open = true
                    if (!moduleNo) {
                        system.children.forEach(function (item) {
                            item.open = true
                        })
                    } else {
                        for (var j = 0; j < system.children.length; j++) {
                            var module = system.children[j]
                            if (module.No === moduleNo) {
                                module.open = true
                            }
                        }
                    }
                }
            }

        },
        ChangeSystemNo(systemNo) {
            this.systemNo = systemNo;
            this.selectedTopMenuIndex = systemNo;
            localStorage.setItem('SystemNo', systemNo);
        },
        openPage(menu) {
            if (menu.MenuModel == ""|| menu.MenuModel == "Windows") {
                mui.alert("该功能暂未处理，请到PC端使用")
                return;
            }

            var url = menu.Url;
            if (url.indexOf("/CCFast") != -1 || url.indexOf("/Ens.htm") != -1) {
                mui.alert("该功能暂未处理，请到PC端使用")
                return;
            }
            url = url.replace("SearchEditer.htm", "SearchDict.htm");
            if (url.indexOf("?") == -1)
                url = url + "?1=1";
            url = url.replace("/WF/", "/CCMobile/");

            SetHref(url + "&SystemNo=" + this.systemNo);
        },

    },
    mounted: function () {
        // fix firefox bug
        document.body.ondrop = function (event) {
            event.preventDefault();
            event.stopPropagation();
        }

        var systems = new Entities("BP.CCFast.CCMenu.MySystems");
        systems.RetrieveAll();
        systems = obj2arr(systems)

        //模块.
        var modules = new Entities("BP.CCFast.CCMenu.Modules");
        modules.RetrieveAll();

        for (var i = 0; i < modules.length; i++) {
            var en = modules[i];
            if (en.Icon === "")
                modules[i].Icon = "icon-folder";
        }
        modules = obj2arr(modules);

        //菜单.
        var menus = new Entities("BP.CCFast.CCMenu.Menus");
        menus.RetrieveAll();
        menus = obj2arr(menus);

        //对菜单进行解析处理.
        for (var i = 0; i < menus.length; i++) {
            var menu = menus[i];
            if (menu.MenuModel == "FlowEntityBatchStart")
                continue;
            menu = DealMenuUrl(menu);
            if (menu.Icon === '')
                menu.Icon = 'icon-user';
        }

        //遍历系统.
        for (var i = 0; i < systems.length; i++) {
            var sys = systems[i];
            sys.open = false
            sys.children = []
            var childModules = modules.filter(function (module) {
                // return module.SystemNo === ''
                return module.SystemNo === sys.No
            })
            for (var j = 0; j < childModules.length; j++) {
                var module = childModules[j]
                module.open = false;

                module.children = menus.filter(function (menu) {
                    return menu.ModuleNo == module.No;
                })

            }
            sys.children = childModules;
        }

        this.flowNodes = systems;
        this.expandAssignMenu()
        this.selectedTopMenuIndex = GetQueryString("SystemNo") || "";//  urlGet()
        if (this.selectedTopMenuIndex == "" || this.selectedTopMenuIndex == "null")
            this.selectedTopMenuIndex = localStorage.getItem('SystemNo');
        console.log(this.selectedTopMenuIndex);
    }
})


function obj2arr(obj) {
    delete obj.Paras
    delete obj.ensName
    delete obj.length
    var arr = []
    for (var key in obj) {
        if (Object.hasOwnProperty.call(obj, key)) {
            arr.push(obj[key]);
        }
    }
    return arr
}
function urlGet() {
    var aQuery = GetHrefUrl().split("?");  //取得Get参数
    var aGET = new Array();
    if (aQuery.length > 1) {
        var aBuf = aQuery[1].split("&");
        for (var i = 0, iLoop = aBuf.length; i < iLoop; i++) {
            var aTmp = aBuf[i].split("=");  //分离key与Value
            aGET[aTmp[0]] = aTmp[1];
        }
    }
    return aGET['tabnum']
}
