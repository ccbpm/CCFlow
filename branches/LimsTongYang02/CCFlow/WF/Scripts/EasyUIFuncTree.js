/*
easyui-tree功能树定义，added by liuxc，2015-10-27
说明：
1.Nodes:tree中的节点数组，功能支持如下：
①支持无限级节点设置；②支持任一级别从WebService获取DataTable的Json数据填充；③支持各级节点的图标、右键绑定菜单、双击链接Url的规则设置，支持多级嵌套规则设置；④链接Url支持node属性值、node.attributes属性值及WebUser属性值的自动替换（使用“@@属性字段名”来代替要替换的属性）
2.Nodes数组下元素：(1)Type:节点类型，Node=普通定义节点，Service=通过获取WebService数据填充的节点；(2)ServiceMethod:ajaxService方法传参中method的值，即调用的获取数据的方法，Service类型节点特有属性；(3)CodId:WebService返回的DataTable Json数据代表节点Id的列名，Service类型节点特有属性；(4)ColParentId:WebService返回的DataTable Json数据代表父级节点Id的列名，Service类型节点特有属性；(5)ColName:WebService返回的DataTable Json数据代表节点文字的列名，Service类型节点特有属性；(6)RootParentId:WebService返回的DataTable Json数据代表根节点的父级Id的值，Service类型节点特有属性；(7)ColDefine:WebService返回的DataTable Json数据，设置的此列，根据此列的值进行设置各节点的图标、右链菜单以及双击打开页面，Service类型节点特有属性；(8)Defines:此数组的元素代表ColDefine所设列的详细规则设置，每个元素代表一种情况，整个设置可以理解为：
swith(ColDefine1.Value){
case 'aaa':
node.IconCls='icon-aaa';
node.MenuId = 'menu-aaa';
node.Url = 'url-aaa';
break;
case 'bbb':
swith(ColDefine2.Value){
case 'ccc':
node.IconCls='icon-ccc';
node.MenuId = 'menu-ccc';
node.Url = 'url-ccc';
break;
default:
......
}
break;
default:    //未设置Value值，则表示此项
......
}
此项规则设置，可以进行多级嵌套设置，即Defines元素中再包含ColDefine设置，Service类型节点特有属性；(9)Id:节点node.id值，Node类型节点特有属性；(10)ParentId:节点的父节点node.id，根节点的父节点id请设置为null，Node类型节点特有属性；(11)Name:节点node.text值，Node类型节点特有属性
3.Defines数组下元素：(1)Value:规则判断值；(2)ColDefine:规则判断所用的字段名称；(3)Defines:具体规则设置，见上方规则设置说明；(4)IconCls:节点图标对应的css样式名称；(5)MenuId:节点右链菜单的id，为easyui-menu；(6)Url:节点双击在右侧tab页打开的网页Url，支持node属性值、node.attributes属性值及WebUser属性值的自动替换；(7)OpenType:含有Url属性时，此属性标识Url打开的方式，有"dialog"和"tab"两种方式，dialog方式为打开一个easyui-dialog的悬浮框显示网页，而tab方式是在oOptions.TabId指定的easyui-tabs控件中，打开一个新的标签页来显示网页；(8)OpenWidth:当OpenType="dialog"时，此属性标识打开悬浮框的宽度；(9)OpenHeight:当OpenType="dialog"时，此属性标识打开悬浮框的高度；(10)ClickFunc:此属性定义，则在单击树结点时，扫行此属性定义的Function函数，传递树结点对象node为参数
4.aFuncNodes定义节点数组中元素格式如下：
(1).远程获取WebService生成的节点定义：
{ Type: "Service", ServiceMethod: "GetFlowTree", ColId: "No", ColParentId: "ParentNo", ColName: "Name", RootParentId: "F0",
	ColDefine: "TType", Defines: [
								{ Value: "FLOWTYPE", ColDefine: "ParentNo",
									Defines: [
												{ Value: "F0", IconCls: "icon-flowtree", MenuId: "mFlowRoot", OpenType: "dialog", OpenWidth: 800, OpenHeight:600, ClickFunc: ShowFlowType },
												{ IconCls: "icon-tree_folder", MenuId: "mFlowSort" }
											]
								},
								{ Value: "FLOW", ColDefine: "DType", Defines: [
                                    { Value: "1", IconCls: "icon-flow1", MenuId: "mFlow", Url: "Designer.aspx?FK_Flow=@@id&UserNo=@@WebUser.No&SID=@@WebUser.SID&Flow_V=2" },
                                    { IconCls: "icon-flow1", MenuId: "mFlow", Url: "Designer.aspx?FK_Flow=@@id&UserNo=@@WebUser.No&SID=@@WebUser.SID&Flow_V=1" }
                                ]
								}
								]
}
(2).普通节点定义：
{ Type: "Node", Id: "FlowCloud", ParentId: null, Name: "ccbpm云服务-流程云", TType: "FLOWCLOUD", DType: "-1", IconCls: "icon-flowcloud",
	Nodes: [
			{ Type: "Node", Id: "ShareFlow", ParentId: "FlowCloud", Name: "共有流程云", TType: "SHAREFLOW", DType: "-1", IconCls: "icon-flowpublic", Url: "" },
			{ Type: "Node", Id: "FlowFav", ParentId: "FlowCloud", Name: "我的收藏", TType: "SHAREFLOW", DType: "-1", IconCls: "icon-flowpublic", Url: "" },
			{ Type: "Node", Id: "PriFlow", ParentId: "FlowCloud", Name: "私有流程云", TType: "PRIFLOW", DType: "-1", IconCls: "icon-flowprivate" }
			]
}
5.oOptions参数：定义功能树的一些参数。
*/
//定义功能树对象
function EasyUIFuncTree(sTreeId, aFuncNodes, oOptions) {
    /// <summary>功能树对象操作类</summary>
    /// <param name="sTreeId" type="String">功能树easyui-tree控件的id</param>
    /// <param name="aFuncNodes" type="Array">功能树的节点定义数组</param>
    /// <param name="oOptions" type="Object">功能树定义参数对象，包含：AttrCols（String类型的Array，代表通过Service获取的数据中，要写入node.attributes中的列名称数组），RootASC（Object对象，多根节点时的排序设置，包含Field属性，指排序的依据列名称；Index数组，指排列顺序数组），TabId（如果结点规则定义中的双击结点打开Url的OpenType方式为"tab"时，则此属性标识标签所属的easyui-tabs控件的id），Service（含有Service类型的节点时，此属性标识要连接的Service地址），LoadCompleted（树加载完成后运行的处理逻辑）</param>
    this.TreeId = sTreeId;
    this.FuncNodes = aFuncNodes;
    this.ServiceCount = 0;
    this.Options = {
        AttrCols: [],
        RootASC: { Field: "", Index: [] },
        TabId: "",
        Service: "",
        LoadCompleted: function () { }
    };

    //设置参数
    if (oOptions) {
        if (oOptions.AttrCols && typeof oOptions.AttrCols == "array") {
            this.Options = oOptions.AttrCols;
        }
        if (oOptions.RootASC && typeof oOptions.RootASC == "object") {
            if (oOptions.RootASC.Field && typeof oOptions.RootASC.Field == "string") {
                this.Options.RootASC.Field = oOptions.RootASC.Field;
            }
            if (oOptions.RootASC.Index && typeof oOptions.RootASC.Index == "array") {
                this.Options.RootASC.Index = oOptions.RootASC.Index;
            }
        }
        if (oOptions.TabId && typeof oOptions.TabId == "string") {
            this.Options.TabId = oOptions.TabId;
        }
        if (oOptions.Service && typeof oOptions.Service == "string") {
            this.Options.Service = oOptions.Service;
        }
        if (oOptions.LoadCompleted && typeof oOptions.LoadCompleted == "function") {
            this.Options.LoadCompleted = oOptions.LoadCompleted;
        }
    }

    //检索所有节点中，Service节点的数目
    var nodes;
    for (var i = 0; i < this.FuncNodes.length; i++) {
        if (this.FuncNodes[i].Type == "Service") {
            this.ServiceCount++;
            continue;
        }

        CalculateServiceCount(this.FuncNodes[i], this);
    }

    //加载节点
    if (this.ServiceCount > 0) {
        for (var i = 0; i < this.FuncNodes.length; i++) {
            LoadServiceNode(this.FuncNodes[i], null, this);
        }
    }
    else {
        $.each(this.FuncNodes, function () {
            LoadTreeNode(this, null, fc);
        });

        ExpandFirstLevelNode(this);
        OnContextMenu(this);
        OnDbClick(this);
        OnClick(this);

        if(this.Options.LoadCompleted){
            this.Options.LoadCompleted();
        }
    }

    if (typeof EasyUIFuncTree._initialized == "undefined") {
        EasyUIFuncTree.prototype.appendNode = function (sParentNodeId, oNode) {
            /// <summary>增加树节点</summary>
            /// <param name="sParentNodeId" type="String">待增加树节点的父节点id</param>
            /// <param name="oNode" type="Object">待增加的树节点对象，格式如:{ id: 'aaa', text: '节点1', iconCls: 'icon-new', attributes: {MenuId: "myMenu", Url: "xxx.aspx"} } </param>
            $("#" + this.TreeId).tree("append", {
                parent: $("#" + this.TreeId + " div[node-id='" + sParentNodeId + "']"),
                data: [oNode]
            });

            $("#" + this.TreeId).tree("select", $("#" + this.TreeId + " div[node-id='" + oNode.id + "']"));
        }

        EasyUIFuncTree.prototype.deleteNode = function (sNodeId) {
            /// <summary>删除树节点</summary>
            /// <param name="sTreeId" type="String">功能树easyui-tree控件的id</param>
            /// <param name="sNodeId" type="String">待删除树节点的id</param>
            $("#" + this.TreeId).tree("remove", $("#" + this.TreeId + " div[node-id='" + sNodeId + "']"));
        }
    }
}

function CalculateServiceCount(oNode, oFuncTree) {
    /// <summary>计算树定义中含有多少Service类型的节点</summary>
    /// <param name="oNode" type="Object">节点定义对象</param>
    /// <param name="oFuncTree" type="Object">树对象</param>
    if (oNode.Type == "Service") {
        oFuncTree.ServiceCount++;
        return;
    }

    if (oNode.Nodes && oNode.Nodes.length > 0) {
        for (var i = 0; i < oNode.Nodes.length; i++) {
            CalculateServiceCount(oNode.Nodes[i], oFuncTree);
        }
    }
}

function LoadServiceNode(oNode, oParentNode, oFuncTree) {
    /// <summary>从WebService返回节点数据，生成节点定义对象</summary>
    /// <param name="oNode" type="Object">节点定义对象</param>
    /// <param name="oParentNode" type="Object">节点定义对象的父级对象</param>
    /// <param name="oFuncTree" type="Object">树对象</param>
    if (oNode.Type == "Service") {
        AjaxService({ method: oNode.ServiceMethod }, oFuncTree.Options.Service, function (data, nd) {
            var re = $.parseJSON(data);

            //将所有获取的数据转换为Node
            var roots = Find(re, nd.ColParentId, nd.RootParentId);

            if (roots.length > 0) {
                nd.Type = "Node";
                nd.Id = roots[0][nd.ColId];
                nd.ParentId = oParentNode == null ? null : oParentNode.Id; // root[nd.ColParentId];
                nd.Name = roots[0][nd.ColName];

                if (oFuncTree.Options.AttrCols && oFuncTree.Options.AttrCols.length > 0) {
                    $.each(oFuncTree.Options.AttrCols, function (acidx, ac) {
                        nd[ac] = roots[0][ac];
                    });
                }

                var define = FindDefine(nd.ColDefine, nd.Defines, roots[0]);

                if (define) {
                    nd.IconCls = define.IconCls;
                    nd.MenuId = define.MenuId;
                    nd.Url = define.Url;
                    nd.OpenType = define.OpenType;
                    nd.OpenWidth = define.OpenWidth;
                    nd.OpenHeight = define.OpenHeight;
                    nd.ClickFunc = define.ClickFunc;
                }

                //生成子节点
                LoadServiceSubNode(re, nd, oParentNode, nd, oFuncTree);

                for (var i = 1; i < roots.length; i++) {
                    var nextND = {
                        Type: "Node",
                        Id: roots[i][nd.ColId],
                        ParentId: oParentNode == null ? null : oParentNode.Id,
                        Name: roots[i][nd.ColName]
                    };

                    if (oFuncTree.Options.AttrCols && oFuncTree.Options.AttrCols.length > 0) {
                        $.each(oFuncTree.Options.AttrCols, function (acidx, ac) {
                            nextND[ac] = roots[i][ac];
                        });
                    }

                    define = FindDefine(nd.ColDefine, nd.Defines, roots[i]);

                    if (define) {
                        nextND.IconCls = define.IconCls;
                        nextND.MenuId = define.MenuId;
                        nextND.Url = define.Url;
                        nextND.OpenType = define.OpenType;
                        nextND.OpenWidth = define.OpenWidth;
                        nextND.OpenHeight = define.OpenHeight;
                        nextND.ClickFunc = define.ClickFunc;
                    }

                    if (oParentNode == null) {
                        oFuncTree.FuncNodes.push(nextND);
                    }
                    else {
                        if (!oParentNode.FuncNodes) {
                            oParentNode.FuncNodes = [];
                        }

                        oParentNode.FuncNodes.push(nextND);
                    }

                    //生成子节点
                    LoadServiceSubNode(re, nextND, oParentNode, nd, oFuncTree);
                }
            }

            oFuncTree.ServiceCount--;

            //判断是否完成所有的服务调用，如果完成，则进行全树的生成
            if (oFuncTree.ServiceCount == 0) {
                //排序根节点顺序
                if (oFuncTree.Options.RootASC) {
                    oFuncTree.FuncNodes.sort(function (oNode1, oNode2) {
                        return IndexInArray(oNode1[oFuncTree.Options.RootASC.Field], oFuncTree.Options.RootASC.Index) > IndexInArray(oNode2[oFuncTree.Options.RootASC.Field], oFuncTree.Options.RootASC.Index);
                    });
                }

                $.each(oFuncTree.FuncNodes, function () {
                    LoadTreeNode(this, null, oFuncTree);
                });

                //只展开第一级节点
                ExpandFirstLevelNode(oFuncTree);
                OnContextMenu(oFuncTree);
                OnDbClick(oFuncTree);
                OnClick(oFuncTree);
                                
                if(oFuncTree.Options.LoadCompleted){
                    oFuncTree.Options.LoadCompleted();
                }
            }
        }, oNode);
    }
    else {
        if (oNode.FuncNodes && oNode.FuncNodes.length > 0) {
            $.each(oNode.FuncNodes, function () {
                LoadServiceNode(this, oNode, oFuncTree);
            });
        }
    }
}

function OnClick(oFuncTree) {
    /// <summary>树节点的单击事件处理逻辑</summary>
    /// <param name="oFuncTree" type="Object">树对象</param>
    $("#" + oFuncTree.TreeId).tree({
        onClick: function (node) {
            $("#" + oFuncTree.TreeId).tree('select', node.target);

            if (!node.attributes.ClickFunc || typeof node.attributes.ClickFunc != "function") return;

            node.attributes.ClickFunc(node);
        }
    });
}

function OnDbClick(oFuncTree) {
    /// <summary>树节点的双击事件处理逻辑</summary>
    /// <param name="oFuncTree" type="Object">树对象</param>
    $("#" + oFuncTree.TreeId).tree({
        onDblClick: function (node) {
            $("#" + oFuncTree.TreeId).tree('select', node.target);
            //支持将url中的@@+字段格式自动替换成node节点及其属性、或WebUser中同名的属性值
            if (node.attributes.Url) {
                var url = node.attributes.Url;
                if (node.attributes.Url.indexOf("@@") != -1) {
                    for (field in node) {
                        if (typeof node[field] != "undefined" && url.indexOf("@@" + field) != -1) {
                            url = url.replace("@@" + field, node[field]);
                        }
                    }

                    for (field in node.attributes) {
                        if (typeof node.attributes[field] != "undefined" && url.indexOf("@@" + field) != -1) {
                            url = url.replace("@@" + field, node.attributes[field]);
                        }
                    }

                    for (field in WebUser) {
                        if (typeof WebUser[field] != "undefined" && url.indexOf("@@WebUser." + field) != -1) {
                            url = url.replace("@@WebUser." + field, WebUser[field]);
                        }
                    }
                }

                switch (node.OpenType) {
                    case "dialog":
                        OpenEasyUiDialog(node.Url, "euiframe", node.text, node.OpenWidth | 800, node.OpenHeight | 495, node.IconCls);
                        break;
                    case "tab":
                        OpenInTab(oFuncTree.Options.TabId, node.id, node.text, node.Url, node.IconCls);
                        break;
                }
            }
            else if ($("#" + oFuncTree.TreeId).tree('isLeaf', node.target) == false) {
                if (node.state == "closed") {
                    $("#" + oFuncTree.TreeId).tree("expand", node.target);
                }
                else {
                    $("#" + oFuncTree.TreeId).tree("collapse", node.target);
                }
            }
        }
    });
}

$.messager.defaults.ok = "确定";
$.messager.defaults.cancel = "取消";

function OpenEasyUiDialog(url, iframeId, dlgTitle, dlgWidth, dlgHeight, dlgIcon, showBtns, okBtnFunc, okBtnFuncArgs) {
    ///<summary>使用EasyUiDialog打开一个页面</summary>
    ///<param name="url" type="String">页面链接</param>
    ///<param name="iframeId" type="String">嵌套url页面的iframe的id，在okBtnFunc中，可以通过document.getElementById('eudlgframe').contentWindow获取该页面，然后直接调用该页面的方法，比如获取选中值</param>
    ///<param name="dlgTitle" type="String">Dialog标题</param>
    ///<param name="dlgWidth" type="int">Dialog宽度</param>
    ///<param name="dlgHeight" type="int">Dialog高度</param>
    ///<param name="dlgIcon" type="String">Dialog图标，必须是一个样式class</param>
    ///<param name="showBtns" type="Boolean">Dialog下方是否显示“确定”“取消”按钮，如果显示，则后面的okBtnFunc参数要填写</param>
    ///<param name="okBtnFunc" type="Function">点击“确定”按钮调用的方法</param>
    ///<param name="okBtnFuncArgs" type="Object">okBtnFunc方法使用的参数</param>

    //    var inIframe = window.frameElement != null && window.frameElement.nodeName == 'IFRAME',
    //        doc = inIframe ? window.parent.document : window.document;
    var dlg = $('#eudlg');
    var isTheFirst;

    if (dlg.length == 0) {
        isTheFirst = true;
        var divDom = document.createElement('div');
        divDom.setAttribute('id', 'eudlg');
        document.body.appendChild(divDom);
        dlg = $('#eudlg');
        dlg.append("<iframe frameborder='0' src='' scrolling='auto' id='" + iframeId + "' style='width:100%;height:100%'></iframe>");
    }

    //此处为防止在一个页面使用多次此方法时，传进的iframeId不同，造成找不到非第一次创建的iframe的错误而设置的
    //todo:此处暂时有问题，同一个页面调用此方法，传进的iframeId必须相同，否则会出现问题，此问题有待以后解决
    //    if ($('#' + iframeId).length == 0) {
    //        dlg.empty();
    //        dlg.append("<iframe frameborder='0' src='' scrolling='auto' id='" + iframeId + "' style='width:100%;height:100%'></iframe>");
    //    }

    dlg.dialog({
        title: dlgTitle,
        width: dlgWidth,
        height: dlgHeight,
        iconCls: dlgIcon,
        resizable: true,
        modal: true,
        cache: false
    });

    if (showBtns && okBtnFunc) {
        dlg.dialog({
            buttons: [{
                text: '确定',
                handler: function () {
                    if (okBtnFunc(okBtnFuncArgs) == false) {
                        return;
                    }

                    dlg.dialog('close');
                    $('#' + iframeId).attr('src', '');
                }
            }, {
                text: '取消',
                handler: function () {
                    dlg.dialog('close');
                    $('#' + iframeId).attr('src', '');
                }
            }]
        });
    }
    else {
        dlg.dialog({
            buttons: null,
            onClose: function () {
                dlg.find("iframe").attr('src', '');
            }
        });
    }

    dlg.dialog('open');
    $('#' + iframeId).attr('src', url);
}

function OpenInTab(sTabId, sId, sTitle, sUrl, sIconCls) {
    /// <summary>在指定id的easyui-tabs控件中，使用一个新标签页打开指定url</summary>
    /// <param name="sTabId" type="String">easyui-tabs控件的id</param>
    /// <param name="sId" type="String">增加的tab标签页的id</param>
    /// <param name="sTitle" type="String">增加的tab标签页的文本</param>
    /// <param name="sUrl" type="String">增加的tab标签页中要打开的URL</param>
    /// <param name="sIconCls" type="String">增加的tab标签页的图标样式</param>
    if ($('#' + sTabId).tabs('exists', sTitle)) {
        $('#' + sTabId).tabs('select', sTitle);
    } else {
        var content = '<iframe scrolling="auto" frameborder="0" src="' + sUrl + '" style="width:100%;height:100%;"></iframe>';
        $('#' + sTabId).tabs('add', {
            title: sTitle,
            id: sId,
            content: content,
            iconCls: sIconCls,
            closable: true
        });
    }
}

function OnContextMenu(oFuncTree) {
    /// <summary>树的右键菜单处理逻辑</summary>
    /// <param name="oFuncTree" type="Object">树对象</param>
    $("#" + oFuncTree.TreeId).tree({
        onContextMenu: function (e, node) {
            e.preventDefault();

            $("#" + oFuncTree.TreeId).tree('select', node.target);

            if (node.attributes && node.attributes.MenuId) {
                $("#" + node.attributes.MenuId).menu('show', {
                    left: e.pageX,
                    top: e.pageY
                });
            }
        }
    });
}

function ExpandFirstLevelNode(oFuncTree) {
    /// <summary>设置树的第一级节点全部为展开状态</summary>
    /// <param name="oFuncTree" type="Object">树对象</param>
    $("#" + oFuncTree.TreeId).tree("collapseAll");

    if (oFuncTree.FuncNodes.length > 0) {
        $("#" + oFuncTree.TreeId).tree("expand", $("#" + oFuncTree.TreeId + " div[node-id='" + oFuncTree.FuncNodes[0].Id + "']"));
    }
}

function IndexInArray(oVal, aSortArray) {
    /// <summary>获取元素在数组中的索引序号</summary>
    /// <param name="oVal" type="Object">数组中的元素</param>
    /// <param name="aSortArray" type="Array">数组对象</param>
    /// <return type="Int">返回索引序号</return>
    for (var i = 0; i < aSortArray.length; i++) {
        if (aSortArray[i] == oVal) {
            return i;
        }
    }
    return -1;
}

function LoadServiceSubNode(aServiceNodes, oNode, oParentNode, oServiceNode, oFuncTree) {
    /// <summary>加载节点定义对象的子级对象</summary>
    /// <param name="aServiceNodes" type="Array">WebService返回的节点定义对象集合</param>
    /// <param name="oNode" type="Object">节点定义对象</param>
    /// <param name="oParentNode" type="Object">节点定义对象的父级对象</param>
    /// <param name="oServiceNode" type="Object">初始节点定义对象，此对象含有Service类节点的一些参数</param>
    /// <param name="oFuncTree" type="Object">树定义对象</param>
    var subs = Find(aServiceNodes, oServiceNode.ColParentId, oNode.Id);

    $.each(subs, function (sidx, sub) {
        var subNode = {
            Type: "Node",
            Id: this[oServiceNode.ColId],
            ParentId: oNode.Id,
            Name: this[oServiceNode.ColName]
        };

        if (oFuncTree.Options.AttrCols && oFuncTree.Options.AttrCols.length > 0) {
            $.each(oFuncTree.Options.AttrCols, function (acidx, ac) {
                subNode[ac] = sub[ac];
            });
        }

        define = FindDefine(oServiceNode.ColDefine, oServiceNode.Defines, this);

        if (define) {
            subNode.IconCls = define.IconCls;
            subNode.MenuId = define.MenuId;
            subNode.Url = define.Url;
            subNode.OpenType = define.OpenType;
            subNode.OpenWidth = define.OpenWidth;
            subNode.OpenHeight = define.OpenHeight;
            subNode.ClickFunc = define.ClickFunc;
        }

        if (!oNode.Nodes) {
            oNode.Nodes = [];
        }

        oNode.Nodes.push(subNode);

        //生成子节点
        LoadServiceSubNode(aServiceNodes, subNode, oNode, oServiceNode, oFuncTree);
    });
}

function LoadTreeNode(oNode, oParentNode, oFuncTree) {
    /// <summary>加载树节点</summary>
    /// <param name="oNode" type="Object">节点定义对象</param>
    /// <param name="oParentNode" type="Object">节点定义对象的父级对象</param>
    /// <param name="oFuncTree" type="Object">树定义对象</param>
    //生成附加属性
    var attrs = { MenuId: oNode.MenuId, Url: oNode.Url, ShowType: oNode.ShowType, ShowWidth: oNode.ShowWidth, ShowHeight: oNode.ShowHeight, ClickFunc: oNode.ClickFunc };

    if (oFuncTree.Options.AttrCols) {
        $.each(oFuncTree.Options.AttrCols, function () {
            attrs[this] = oNode[this];
        });
    }

    $("#" + oFuncTree.TreeId).tree("append", {
        parent: oParentNode,
        data: [{
            id: oNode.Id,
            text: oNode.Name,
            iconCls: oNode.IconCls,
            attributes: attrs
        }]
    });

    if (oNode.Nodes && oNode.Nodes.length > 0) {
        $.each(oNode.Nodes, function () {
            LoadTreeNode(this, $("#" + oFuncTree.TreeId + " div[node-id='" + oNode.Id + "']"), oFuncTree);
        });
    }
}

function Find(aItems, sField, oValue) {
    /// <summary>查找数组中指定字段值的元素</summary>
    /// <param name="aItems" type="Array">要查找的数组</param>
    /// <param name="sField" type="String">依据字段名称</param>
    /// <param name="oValue" type="Object">字段的值</param>
    /// <return>返回集合</return>
    var re = [];

    $.each(aItems, function () {
        if (this[sField] == oValue) {
            re.push(this);
        }
    });

    return re;
}

function FindDefine(sColDefine, aDefines, oNode) {
    /// <summary>查找指定节点的设置规则</summary>
    /// <param name="sColDefine" type="String">规则依据的字段名称</param>
    /// <param name="aDefines" type="Array">当前规则集合</param>
    /// <param name="oNode" type="Object">要查找规则的节点</param>
    var define;

    $.each(aDefines, function () {
        if (typeof this.Value != "undefined") {
            if (oNode[sColDefine] == this.Value) {
                if (!this.Defines) {
                    define = this;
                    return false;
                }

                define = FindDefine(this.ColDefine, this.Defines, oNode);
            }
            else {
                return true;
            }
        }
        else {
            define = this;
            return false;
        }
    });

    return define;
}

function AjaxService(param, serviceUrl, callback, scope) {
    $.ajax({
        type: "GET", //使用GET或POST方法访问后台
        dataType: "text", //返回json格式的数据
        contentType: "application/json; charset=utf-8",
        url: serviceUrl, //要访问的后台地址
        data: param, //要发送的数据
        async: true,
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