var toolbar = [{ 'text': '新建', 'iconCls': 'icon-new', 'handler': 'addApp' }, { 'text': '编辑系统菜单', 'iconCls': 'icon-config', 'handler': 'EditMenus'}];

function addApp() {
    var url = "../WF/Comm/RefFunc/UIEn.aspx?EnsName=BP.GPM.Apps";
    window.showModalDialog(url, "属性", "dialogWidth=800px;dialogHeight=500px;dialogTop=140px;dialogLeft=260px");
    LoadGrid();
}
function winOpen(url) {
    window.showModalDialog(url, "属性", "dialogWidth=800px;dialogHeight=500px;dialogTop=140px;dialogLeft=260px");
    LoadGrid();
}
function AddTab(title, url) {
    window.parent.addTab(title, url);
}
//编辑菜单
function EditMenus() {
    var row = $('#appGrid').datagrid('getSelected');
    if (row) {
        var url = "AppMenu.aspx?FK_App=" + row.No;
        AddTab(row.Name + '系统菜单', url);
    }
    else {
        CC.Message.showError("系统提示", "请先选择项！");
    }
}
function LoadGrid() {
    Application.data.getApps(function (js, scope) {
        if (js) {
            if (js.status == "500" || js == "nologin") {
                return;
            }

            if (js == "") js = "[]";
            var pushData = eval('(' + js + ')');
            $('#appGrid').datagrid({
                data: pushData,
                width: 'auto',
                toolbar: toolbar,
                striped: true,
                rownumbers: true,
                singleSelect: true,
                loadMsg: '数据加载中......',
                columns: [[
                       { field: 'No', title: '编号', width: 60 },
                       { field: 'Name', title: '名称', width: 200, formatter: function (value, rec) {
                           var url = "../../WF/Comm/En.htm?EnsName=BP.GPM.Apps&PK=" + rec.No
                           + "&No=" + rec.No
                           + "&AppModel=" + rec.AppModel
                           + "&FK_AppSort=" + rec.FK_AppSort
                           + "&OpenWay=" + rec.OpenWay;
                           return "<a href='javascript:void(0)' onclick=winOpen('" + url + "')>" + value + "</a>";
                       }
                       },
                       { field: 'AppModelText', title: '应用类型', width: 80, align: 'left' },
                       { field: 'FK_AppSortText', title: '类别', width: 60, align: 'left' },
                       { field: 'Url', title: '连接', width: 160, align: 'left' },
                       { field: 'OpenWayText', title: '打开方式', width: 60 },
                       { field: 'Idx', title: '显示顺序', width: 60 },
                       { field: 'MyFilePath', title: '是否启用', width: 60, formatter: function (value, rec) {
                           if (value == 1)
                               return "是";
                           else
                               return "否";
                       }
                       },
                       { field: 'RefMenuNo', title: '关联菜单编号', width: 90 },
                       { field: 'control', title: '操作', width: 180, formatter: function (value, rec) {
                           var url = "AppMenu.aspx?FK_App=" + rec.No;
                           var title = "<a href='javascript:void(0)' onclick=AddTab('" + rec.Name + "系统菜单','" + url + "') >编辑系统菜单</a>";
                           return title;
                       }
                       }
                       ]]
            });
        }
    }, this);

}
//初始页面
$(function () {
    LoadGrid();
});