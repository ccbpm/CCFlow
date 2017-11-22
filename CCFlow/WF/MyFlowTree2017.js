/*
树形表单工作处理器:
1,里面包含一些必备的数据.
2, @代国强 实现.
*/
var workNode = null;
function GenerTreeFrm(wn) {
    workNode = wn;

}

//表单树初始化
function FlowFormTree_Init() {
    var para = {
        DoType: "FlowFormTree_Init",
        FK_Flow: pageData.FK_Flow,
        FK_Node: pageData.FK_Node,
        WorkID: pageData.WorkID
    };

    AjaxService(para, function (data) {
        if (data.indexOf('err@') == 0) {//发送时发生错误
            alert(data);
            return;
        }
        var i = 0;
        var isSelect = false;
        var IsCC = pageData.IsCC;
        var pushData = eval('(' + data + ')');
        //pushData = pushData[0].children;
        var urlExt = pageParamToUrl();
        
        //加载类别树
        $("#flowFormTree").tree({
            data: pushData,
            iconCls: 'tree-folder',
            collapsed: true,
            lines: true,
            formatter: function (node) {
                if (i == 0) {
                    if (node.attributes.NodeType == "form|0" || node.attributes.NodeType == "form|1") {
                        i++;
                        var isEdit = node.attributes.IsEdit;
                        if (IsCC && IsCC == "1") isEdit = "0";
                        var url = "../CCForm/Frm.htm?FK_MapData=" + node.id + "&IsEdit=" + isEdit + "&IsPrint=0";

                    }
                }
                return node.text;
            },
            onClick: function (node) {
                if (node.attributes.NodeType == "form|0" || node.attributes.NodeType == "form|1") {/*普通表单和必填表单*/
                    var urlExt = pageParamToUrl();
                    var isEdit = node.attributes.IsEdit;
                    if (IsCC && IsCC == "1") isEdit = "0";
                    var url = "../CCForm/Frm.htm?FK_MapData=" + node.id + "&IsEdit=" + isEdit + "&IsPrint=0" + urlExt;
                    //addTab(node.id, node.text, url);
                } else if (node.attributes.NodeType == "tools|0") {/*工具栏按钮添加选项卡*/
                    var urlExt = urlExtFrm();
                    var url = node.attributes.Url;
                    while (url.indexOf('|') >= 0) {
                        url = url.replace('|', '/');
                    }
                    if (url.indexOf('?') > 0) {
                        url = url + "&FK_MapData=" + node.id + "&" + urlExt;
                    }
                    else {
                        url = url + "?FK_MapData=" + node.id + "&" + urlExt;
                    }
                    //addTab(node.id, node.text, url);
                } else if (node.attributes.NodeType == "tools|1") {/*工具栏按钮打开新窗体*/
                    var urlExt = urlExtFrm();
                    var url = node.attributes.Url;
                    while (url.indexOf('|') >= 0) {
                        url = url.replace('|', '/');
                    }
                    if (url.indexOf('?') > 0) {
                        url = url + "&FK_MapData=" + node.id + "&" + urlExt;
                    }
                    else {
                        url = url + "?FK_MapData=" + node.id + "&" + urlExt;
                    }
                    WinOpenPage("_blank", url, node.text);
                }
            }
        });
        $("#pageloading").hide();
    }, this);
}

$(function () {
    FlowFormTree_Init();
});