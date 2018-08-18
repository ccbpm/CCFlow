//树节点操作
function treeNodeManage(dowhat, nodeNo, callback, scope) {
    var enName = GetEnName();
    var en = new Entity(enName, nodeNo);
    var returnVal = "";
    switch (dowhat) {
        case "sample": //新建同级节点

            var val = window.prompt('请输入名称', '新建节点');
            if (val == null)
                return;

            var sampleEn = en.DoMethodReturnString("DoMyCreateSameLevelNode");
            if (sampleEn.indexOf('err@') == 0) {
                alert(sampleEn);
                return;
            }

           sampleEn = JSON.parse(sampleEn);
           var myen = new Entity(enName, sampleEn);
           myen.Name= val;
           myen.Update();


          returnVal = "{No:'" + myen.No + "',Name:'" + myen.Name + "'}";
            break;
        case "children": //新建下级节点

            var val = window.prompt('请输入名称', '新建节点');
            if (val == null)
              return;

            var val = "xxx";

            var subEn = en.DoMethodReturnString("DoMyCreateSubNode");
            if (subEn.indexOf('err@') == 0) {
                alert(subEn);
                return;
            }

            subEn = JSON.parse(subEn);
            var myen = new Entity(enName, subEn);
            myen.Name = val;
            myen.Update();

            returnVal = "{No:'" + myen.No + "',Name:'" + myen.Name + "'}";
            break;
        case "doup": //上移
            en.DoMethodReturnString("DoUp");

            break;
        case "dodown": //下移
            en.DoMethodReturnString("DoDown");
            break;
        case "delete": //删除
            en.Delete();
            break;
        default: break;

    }
    callback(returnVal);

}

//创建同级目录
function CreateSampleNode() {
    var node = $('#enTree').tree('getSelected');
    if (node) {

        treeNodeManage("sample", node.id, function (js) {
            if (js) {
                var parentNode = $('#enTree').tree('getParent', node.target);
                var pushData = eval('(' + js + ')');
                $('#enTree').tree('append', {
                    parent: (parentNode ? parentNode.target : null),
                    data: [{
                        id: pushData.No,
                        text: pushData.Name,
                        iconCls: 'tree_folder'
                    }]
                });
            }

        }, this);
    } else {
        $.messager.alert('提示', '请选择节点！', 'info');
    }
}
//创建下级目录
function CreateSubNode() {
    var node = $('#enTree').tree('getSelected');
    if (node) {
        treeNodeManage("children", node.id, function (js) {
            if (js) {
                var pushData = eval('(' + js + ')');
                $('#enTree').tree('append', {
                    parent: (node ? node.target : null),
                    data: [{
                        id: pushData.No,
                        text: pushData.Name,
                        iconCls: 'tree_folder'
                    }]
                });
            }

        }, this);
    } else {
        $.messager.alert('提示', '请选择节点！', 'info');
    }
}

//修改
function EditNode(type) {
    var node = $('#enTree').tree('getSelected');
    if (node) {
        var enName = GetEnName();
        if (enName == "" || enName == undefined) {
            $.messager.alert('提示', '没有找到类名！', 'info');
            return;
        }
        var url = "";
        //编辑
        if (type == 0)
            url = "En.htm?EnName=" + enName + "&PKVal=" + node.id + "&isTree=1";
        else
            url = "En.htm?EnName=" + enName + "&PKVal=" + node.id + "&isTree=1" + "&isReadonly=1";

        var cfg = new Entity("BP.Sys.EnCfg");
        cfg.No = GetQueryString("EnsName");
        cfg.RetrieveFromDBSources();

        var windowW = cfg.GetPara("WinCardW");
        if (windowW == "" || windowW == undefined)
            windowW = 700;

        var windowH = cfg.GetPara("WinCardH");
        if (windowH == "" || windowH == undefined)
            windowH = 500;


        OpenEasyUiDialog(url, 'treeFrame', '编辑', windowW, windowH, null, null, null, null, null, function () {
            var en = new Entity(enName, node.id);
            $('#enTree').tree('update', { target: node.target, text: en.Name });

        });

    } else {
        $.messager.alert('提示', '请选择节点！', 'info');
    }
}

//删除节点
function DeleteNode() {
    if (!confirm("是否真的需要删除?"))
        return;
    var node = $('#enTree').tree('getSelected');
    if (node) {
        //删除
        treeNodeManage("delete", node.id, function (js) {
            $('#enTree').tree('remove', node.target);
        }, this);
    } else {
        $.messager.alert('提示', '请选择节点。', 'info');
    }
}

//上移
function DoUp() {
    var node = $('#enTree').tree('getSelected');
    if (node) {
        treeNodeManage("doup", node.id, function (js) {
            BindTree();
            //$('#enTree').tree('expandAll');
        }, this);
    } else {
        $.messager.alert('提示', '请选择节点。', 'info');
    }
}
//下移
function DoDown() {
    var node = $('#enTree').tree('getSelected');
    if (node) {
        treeNodeManage("dodown", node.id, function (js) {
            BindTree();
            // $('#enTree').tree('expandAll');
        }, this);
    } else {
        $.messager.alert('提示', '请选择节点。', 'info');
    }
}

