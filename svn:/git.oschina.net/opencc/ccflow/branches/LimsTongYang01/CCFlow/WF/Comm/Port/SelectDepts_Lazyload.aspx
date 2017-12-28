<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelectDepts_Lazyload.aspx.cs"
    Inherits="CCFlow.WF.Comm.Port.SelectDepts_Lazyload" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>选择部门</title>
    <link href="../JS/EasyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <link href="../JS/EasyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <script src="../JS/EasyUI/jquery.min.js" type="text/javascript"></script>
    <script src="../JS/EasyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        var valctrlid = GetQueryString("valctrl");
        var namectrlid = GetQueryString("namectrl");
        var idx = 0;
        var checkNodeId;

        $(function () {
            $('#selectedDepts').tree({
                checkbox: true,
                onCheck: function (node, checked) {
                    if (!checked) {
                        var onode = $('#depts').tree('find', node.id.substr(1));
//                        alert(checked);
//                        alert(onode.id);
                        if (onode != null) {
                            $('#depts').tree('uncheck', onode.target);
                        }

                        //$(this).tree('remove', node.target);
                    }
                }
            });

            if (namectrlid.length > 0 && valctrlid.length > 0) {
                var names = $(window.parent.document).find('[id$="_TB_' + namectrlid + '"]').val().split(',');
                var values = $(window.parent.document).find('[id$="_TB_' + valctrlid + '"]').val().split(',');
                var maxidx = Math.min(names.length, values.length);

                for (var i = 0; i < maxidx; i++) {
                    if (!values[i] || values[i].length == 0) {
                        continue;
                    }

                    $('#selectedDepts').tree('append', {
                        parent: null,
                        data: [{
                            id: 's' + values[i],
                            text: names[i],
                            checked: true
                        }]
                    });
                }
            }

            $('#depts').tree({
                checkbox: true,
                onBeforeExpand: function (node) {
                    var children = $(this).tree('getChildren', node.target);
                    if (children && children.length == 1 && children[0].text == "加载中...") {
                        if (node.checked) {
                            checkNodeId = node.id;
                        }

                        getDepts(node);
                    }
                },
                onBeforeCheck: function (node, checked) {
                    if (checked) {
                        var children = $(this).tree('getChildren', node.target);
                        if (children && children.length == 1 && children[0].text == "加载中...") {
                            if ($(this).tree('getParent', node.target) == null) {
                                if (confirm('当前为根部门，如果组织结构数据比较庞大，选择此节点会造成浏览器卡住假死，你确定要选择根部门吗？') == false) {
                                    return false;
                                };
                            }

                            checkNodeId = node.id;
                            getDepts(node, checked);
                            return false;
                        }
                    }
                },
                onCheck: function (node, checked) {
                    var snode = $('#selectedDepts').tree('find', 's' + node.id);

                    if (checked) {
                        if (snode == null && node.text != "加载中..." && node.id != undefined) {
                            $('#selectedDepts').tree('append', {
                                parent: null,
                                data: [{
                                    id: 's' + node.id,
                                    text: node.text,
                                    checked: true
                                }]
                            });
                        }

                        selectDept(node);
                        return;
                    }

                    if (snode != null) {
                        $('#selectedDepts').tree('remove', snode.target);
                    }

                    //删除下面的子级部门
                    unSelectDept(node);
                }
            });

            getDepts();
        });

        function selectDept(node) {
            var children = $('#depts').tree('getChildren', node.target);

            $.each(children, function () {
                var snode = $('#selectedDepts').tree('find', 's' + this.id);

                if (this.text == "加载中...") {
                    return true;
                }

                if (snode == null) {
                    $('#selectedDepts').tree('append', {
                        parent: null,
                        data: [{
                            id: 's' + this.id,
                            text: this.text,
                            checked: true
                        }]
                    });
                }

                selectDept(this);
            });
        }

        function unSelectDept(node) {
            var children = $('#depts').tree('getChildren', node.target);
            var snode;

            $.each(children, function () {
                if (this.text == "加载中...") {
                    return true;
                }

                snode = $('#selectedDepts').tree('find', 's' + this.id);

                if (snode != null) {
                    $('#selectedDepts').tree('remove', snode.target);
                }

                unSelectDept(this);
            });
        }

        function getDepts(pnode, checked) {
            ajax({ method: 'getdepts', pno: pnode ? pnode.id : '0' }, function (data) {
                var ds = eval('(' + data + ')');
                var nds = [];

                $.each(ds.data, function () {
                    nds.push({
                        id: this.NO,
                        text: this.NAME,
                        state: 'closed',
                        checked: $('#selectedDepts').tree('find', 's' + this.NO) != null,
                        children: [{
                            id: 'n' + (idx++),
                            text: '加载中...'
                        }]
                    });
                });

                //删除掉等待加载中节点
                if (pnode) {
                    var children = $(this).tree('getChildren', pnode.target);
                    $('#depts').tree('remove', children[0].target);
                }

                $('#depts').tree('append', {
                    parent: pnode ? pnode.target : null,
                    data: nds
                });

                if (checked && checkNodeId && pnode.id == checkNodeId) {
                    $('#depts').tree('check', pnode.target);

                    var children = $(this).tree('getChildren', pnode.target);
                    var snode;

                    $.each(children, function () {
                        if (this.text == "加载中..." || this.id == undefined) {
                            return true;
                        }

                        snode = $('#selectedDepts').tree('find', 's' + this.id);

                        if (snode != null) {
                            return true;
                        }

                        $('#selectedDepts').tree('append', {
                            parent: null,
                            data: [{
                                id: 's' + this.id,
                                text: this.text,
                                checked: true
                            }]
                        });
                    });
                }
            });
        }

        function removeSelectedDepts() {
            var sNodes = $('#selectedDepts').tree('getChecked');

            for (var i = 0; i < sNodes.length; i++) {
                $('#selectedDepts').tree('uncheck', sNodes[i].target);
            }
        }

        function getSelected() {
            var vals = '';
            var names = '';
            var sNodes = $('#selectedDepts').tree('getChecked');

            $.each(sNodes, function () {
                vals += this.id.substr(1) + ',';
                names += this.text + ',';
            });

            if (vals.length > 0) {
                vals = vals.substr(0, vals.length - 1);
                names = names.substr(0, names.length - 1);
            }

            if (valctrlid.length > 0) {
                $(window.parent.document).find('[id$="_TB_' + valctrlid + '"]').val(vals);
            }

            return { Name: names };
        }

        function ajax(param, callback, scope, method) {
            if (!method) method = 'GET';
            $.ajax({
                type: method, //使用GET或POST方法访问后台
                dataType: "text", //返回json格式的数据
                contentType: "application/json; charset=utf-8",
                url: "SelectDepts_Lazyload.aspx?r=" + Math.random(), //要访问的后台地址
                data: param, //要发送的数据
                async: false,
                cache: false,
                complete: function () { }, //AJAX请求完成时隐藏loading提示
                error: function (XMLHttpRequest, errorThrown) {
                    callback(XMLHttpRequest);
                },
                success: function (msg) {//msg为返回的数据，在这里做数据绑定
                    var data = msg;
                    callback(data, scope);
                }
            });
        }

        function GetQueryString(name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
            var r = window.location.search.substr(1).match(reg);
            if (r != null) return unescape(r[2]); return '';
        }
    </script>
</head>
<body class="easyui-layout">
    <form id="form1" runat="server">
    <div data-options="region:'west',title:'部门列表',split:true" style="width: 260px; padding: 5px;">
        <ul id="depts" class="easyui-tree">
        </ul>
    </div>
    <div data-options="region:'center',title:'已选部门',tools:[{iconCls:'icon-delete',handler:removeSelectedDepts}]"
        style="padding: 5px;">
        <ul id="selectedDepts" class="easyui-tree" data-options="fit:true">
        </ul>
    </div>
    </form>
</body>
</html>
