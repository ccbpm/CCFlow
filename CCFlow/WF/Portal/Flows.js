var baseInfo = new Vue({
    el: '#flow',
    data: {
        flowNodes: [],
        expandAll: false,
        loadingDialog: false
    },
    watch: {
        expandAll(val) {
            this.expandMenus(val)
        }
    },
    methods: {
        expandMenus: function (status) {
            for (var i = 0; i < this.flowNodes.length; i++) {
                this.flowNodes[i].open = status
            }
        },
        bindMenu: function () {
            var _this = this
            layui.use('dropdown', function () {
                var dropdown = layui.dropdown
                var topNodeItems = [
                    { title: '<i class=icon-plus></i> 新建流程', id: "NewFlow", Icon: "icon-plus" },
                    { title: '<i class=icon-star></i> 目录属性', id: "EditSort", Icon: "icon-options" },
                    { title: '<i class=icon-folder></i> 新建目录', id: "NewSort", Icon: "icon-magnifier-add" },
                    { title: '<i class=icon-pencil></i> 修改名称', id: "EditSortName", Icon: "icon-magnifier-add" },
                    { title: '<i class=icon-share-alt ></i> 导入流程模版', id: "ImpFlowTemplate", Icon: "icon-plus" },
                   // { title: '<i class=icon-share-alt ></i> 批量导入流程模版', id: "BatchImpFlowTemplate", Icon: "icon-plus" },
                  //  { title: '<i class=icon-share-alt ></i> 批量导出流程模版', id: "BatchExpFlowTemplate", Icon: "icon-plus" },
                    { title: '<i class=icon-close></i> 删除目录', id: "DeleteSort", Icon: "icon-close" }
                ]
                var tRenderOptions = [{
                    elem: '.item-top-dp',
                    trigger: 'contextmenu',
                    data: topNodeItems,
                    click: function (data, oThis) {
                        _this.topNodeOption(data.id, $(this.elem)[0].dataset.no, $(this.elem)[0].dataset.name, $(this.elem)[0].dataset.idx)
                    }
                }, {
                    elem: '.t-btn',
                    trigger: 'click',
                    data: topNodeItems,
                    click: function (data, oThis) {
                        _this.topNodeOption(data.id, $(this.elem)[0].dataset.no, $(this.elem)[0].dataset.name, $(this.elem)[0].dataset.idx)
                    }
                }]

                dropdown.render(tRenderOptions[0]);
                dropdown.render(tRenderOptions[1]);

                var childNodeMenuItems = [
                    { title: '<i class=icon-star></i> 流程属性', id: "Attr" },
                    { title: '<i class=icon-settings></i> 设计流程', id: "Designer" },
                    { title: '<i class=icon-plane></i> 测试容器', id: "Start" },
                    { title: '<i class=icon-docs></i> 复制流程', id: "Copy" },
                    { title: '<i class=icon-pencil></i> 修改名称', id: "EditFlowName" },
                    { title: '<i class=icon-close></i> 删除流程', id: "Delete" }
                ]
                var cRenderOptions = [{
                    elem: '.item-name-dp',
                    trigger: 'contextmenu',
                    data: childNodeMenuItems,
                    click: function (data, othis) {
                        _this.childNodeOption(data.id, $(this.elem)[0].dataset.no, $(this.elem)[0].dataset.name, $(this.elem)[0].dataset.pidx, $(this.elem)[0].dataset.idx)
                    }
                }, {
                    elem: '.c-btn',
                    trigger: 'click',
                    data: childNodeMenuItems,
                    click: function (data, othis) {
                        _this.childNodeOption(data.id, $(this.elem)[0].dataset.no, $(this.elem)[0].dataset.name, $(this.elem)[0].dataset.pidx, $(this.elem)[0].dataset.idx)
                    }
                }]
                dropdown.render(cRenderOptions[0]);
                dropdown.render(cRenderOptions[1]);
            })
        },

        //如果w=0 则是100%的宽度.
        openLayer: function (uri, name, w, h) {
            //console.log(uri, name);

            if (w == 0)
                w = window.innerWidth;

            if (w == undefined)
                w = window.innerWidth / 2;

            if (h == undefined)
                h = window.innerHeight;

            layer.open({
                type: 2,
                title: name,
                content: [uri, 'auto'],
                area: [w + 'px','100%'],
                offset: 'r',
                shadeClose: true,
            })
        },

        Designer: function (no, name) {
            var sid = GetQueryString("Token");
            var webUser = new WebUser();
            var url = basePath + "/WF/Admin/CCBPMDesigner/Designer.htm?FK_Flow=" + no + "&UserNo=" + webUser.No + "&Token=" + sid + "&OrgNo=" + webUser.OrgNo + "&From=Ver2021";
            // window.top.vm.openTab(name, url);
            var self = WinOpenFull(url, "xx");
            var loop = setInterval(function () {
                if (self.closed) {
                   //管理员登录
                    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_TestingContainer");
                    handler.AddPara("Token", GetQueryString("Token"));
                    handler.AddPara("UserNo", GetQueryString("UserNo"));
                    handler.DoMethodReturnString("Default_LetAdminerLogin");
                    clearInterval(loop)
                }
            }, 1);

        },
        EditSort: function (no, name) {
            var url = basePath + "/WF/Comm/EnOnly.htm?EnName=BP.WF.Template.FlowSort&No=" + no;
            this.openLayer(url, "目录:" + name);
        },
        testFlow: function (no, name) {
            var url = basePath + "/WF/Admin/TestingContainer/TestFlow2020.htm?FK_Flow=" + no;
            //window.top.vm.fullScreenOpen(url, name);
            //window.top.vm.openTab(name, url);
             this.openLayer(url, name);
        },
        flowAttr: function (no, name) {
            var url = basePath + "/WF/Comm/En.htm?EnName=BP.WF.Template.FlowExt&No=" + no;
            //window.top.vm.openTab(name, url);
            this.openLayer(url, name,900);
        },

        copyFlow: function (no) {
            if (window.confirm("确定要执行流程复制吗?") == false)
                return;
            var flow = new Entity("BP.WF.Flow", no);
            var data = flow.DoMethodReturnString("DoCopy");
            layer.msg(data);
            setTimeout(function () {
                window.location.reload();
            }, 800);
        },
        DeleteFlow: function (no, pidx, idx) {
            var msg = "提示: 确定要删除该流程吗?";
            msg += "\t\n1.如果该流程下有实例，您不能删除。";
            msg += "\t\n2.该流程为子流程的时候，被引用也不能删除.";
            if (window.confirm(msg) == false)
                return;

            var load = layer.msg("正在处理,请稍候...", {
                icon: 16,
                anim: 5
            })

            //开始执行删除.
            var flow = new Entity("BP.WF.Flow", no);
            var data = flow.DoMethodReturnString("DoDelete");
            layer.msg(data);
            if (data.indexOf("err@") == 0)
                return;

            layer.close(load)

            this.flowNodes[pidx].children.splice(idx, 1)
            var leaveItems = this.flowNodes[pidx].children
            this.$set(this.flowNodes[pidx], 'children', leaveItems)
        },

        childNodeOption: function (key, data, name, pidx, idx) {

            switch (key) {
                case "Attr":
                    this.flowAttr(data, name);
                    break;
                case "Designer":
                    this.Designer(data, name);
                    break;
                case "Start":
                    this.testFlow(data, name);
                    break;
                case "Copy":
                    this.copyFlow(data);
                    break;
                case "EditFlowName":
                    this.EditFlowName(data, name, pidx, idx);
                    break;
                case "Delete":
                    this.DeleteFlow(data, pidx, idx);
                    break;
            }
        },

        topNodeOption: function (key, data, name, idx) {

            switch (key) {
                case "EditSort":
                    this.EditSort(data, name);
                    break;
                case "EditSortName":
                    this.EditSortName(data, name, idx); //修改名字.
                    break;
                case "ImpFlowTemplate":
                    this.ImpFlowTemplate(data);
                    break;
                case "BatchImpFlowTemplate":
                    this.BatchImpFlowTemplate(data);
                    break;
                case "BatchExpFlowTemplate":
                    this.BatchExpFlowTemplate(data,name);
                    break;
                case "NewSort":
                    this.NewSort(data, true);
                    break;
                case "DeleteSort":
                    this.DeleteSort(data);
                    break;
                case "NewFlow":
                    this.NewFlow(data, name);
                    break;
                default:
                    alert("没有判断的命令" + key);
                    break;
            }
        },
        EditSortName(id, name, idx) {

            var val = prompt("新名称", name);
            if (val == null || val == undefined)
                return;
            var en = new Entity("BP.WF.Template.FlowSort", id);
            en.Name = val;
            en.Update();

            //Todo:wanglu , 修改名称.
            // $("#" + id).val(val);
            // var ctl = $("#" + id);
            this.flowNodes[idx].Name = val;
            layer.msg("修改成功.");

        },
        NewFlow: function (data, name) {

            url = basePath + "/WF/Admin/CCBPMDesigner/FlowDevModel/Default.htm?SortNo=" + data + "&From=Flows.htm&RunModel=1&s=" + Math.random();
            url += "&UserNo=" + GetQueryString("UserNo");
            url += "&Token=" + GetQueryString("Token");
            this.openLayer(url, "新建流程", 900);
            //window.open(url);
            //  addTab("NewFlow", "新建流程", url);

        },
        EditFlowName(id, name, pidx, idx) {

            var val = prompt("新名称", name);
            if (val == null || val == undefined)
                return;
            var en = new Entity("BP.WF.Flow", id);
            en.Name = val;
            en.Update();


            this.flowNodes[pidx].children[idx].Name = val;
            //Todo:wanglu , 修改名称.
            // $("#" + id).val(val);
            // var ctl = $("#" + id);
            layer.msg("修改成功.");

        },
        ImpFlowTemplate: function (data) {
            var fk_flowSort = data;
            url = basePath + "/WF/Admin/AttrFlow/Imp.htm?FK_FlowSort=" + fk_flowSort + "&Lang=CH";
            this.openLayer(url, "导入流程模版");
            //addTab("ImpFlowTemplate", "导入流程模版", url);
        },
        BatchImpFlowTemplate: function (data) {
            var fk_flowSort = data;
            url = basePath + "/WF/Admin/AttrFlow/Imp.htm?FK_FlowSort=" + fk_flowSort + "&Lang=CH";
            addTab("ImpFlowTemplate", "导入流程模版", url);
        },
        BatchExpFlowTemplate: function (flowSortNo,flowSortName) {
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_Portal");
            handler.AddPara("FK_Sort", flowSortNo);
            handler.AddPara("FlowSortName", flowSortName);
            var data = handler.DoMethodReturnString("Flow_BatchExpFlowTemplate");
            if (data.indexOf("err@") != -1) {
                layer.alert(data);
                return;
            }
            var url = data.replace("url@", "");
            if (url.indexOf("resources") == -1) {
                SetHref(basePath + "/" + url);
                return;
            }

            //这个是针对Springboot jar包发布后的下载
            SetHref(basePath + "/WF/Ath/DownloadByPath?filePath=" + encodeURIComponent(url));
        },

        DeleteSort: function (no) {

            if (window.confirm("确定要删除吗?") == false)
                return;

            var en = new Entity("BP.WF.Template.FlowSort", no);
            var data = en.Delete();
            layer.msg(data);

            //如果有错误.
            if (data.indexOf("err@") == 0)
                return;

            setTimeout(function () {
                window.location.reload()
            }, 2000)
        },

        NewSort: function (currentElem, sameLevel) {

            //只能创建同级.
            sameLevel = true;

            //例子2
            layer.prompt({
                value: '',
                title: '新建' + (sameLevel ? '同级' : '子级') + '流程类别',
            }, function (value, index, elem) {
                layer.close(index);
                var en = new Entity("BP.WF.Template.FlowSort", currentElem);
                var data = "";
                if (sameLevel == true) {
                    data = en.DoMethodReturnString("DoCreateSameLevelNodeMy", value);
                } else {
                    data = en.DoMethodReturnString("DoCreateSubNodeMy", value);
                }

                layer.msg("创建成功" + data);
                //this.EditSort(data, "编辑");
                //return;

                setTimeout(function () {
                    window.location.reload();
                }, 800);
            });
        },
        updateSort(rootNo, sortNos) {
            // 目录排序..
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_Portal");
            handler.AddPara("RootNo", rootNo);
            handler.AddPara("SortNos", sortNos);
            var data = handler.DoMethodReturnString("Flows_MoveSort");
            layer.msg(data)
        },
        updateFlow(pastNodeArrStr, pastNodeId, currentNodeArrStr, currentNodeId) {
          
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_Portal");
            handler.AddPara("SourceSortNo", pastNodeId); //所在的组编号.
            handler.AddPara("SourceFlowNos", pastNodeArrStr); // 流程编号.
            handler.AddPara("ToSortNo", currentNodeId); //所在的组编号.
            handler.AddPara("ToFlowNos", currentNodeArrStr); // 流程编号.
         
            var data = handler.DoMethodReturnString("Flows_Move");
            layer.msg(data)
        },
        initSortArea: function () {
            var _this = this
            this.$nextTick(function () {
                var sortContainer = this.$refs['sort-main']
                new Sortable(sortContainer, {
                    animation: 150,
                    dataIdAttr: 'data-id',
                    ghostClass: 'blue-background-class',
                    onStart: function ( /**Event*/ evt) {
                        _this.loadingDialog = layer.msg('正在移动...', {
                            timeout: 900 * 1000
                        })
                    },
                    onEnd: function (evt) {
                        layer.close(_this.loadingDialog)
                        var arr = this.toArray();
                        // 一级菜单的排序，默认为1
                        _this.updateSort('1', arr.join(','));
                    }
                });
                var childSortableContainers = this.$refs['child-row']
                console.log(childSortableContainers);
                for (var i = 0; i < childSortableContainers.length; i++) {
                    var csc = childSortableContainers[i]
                    new Sortable(csc, {
                        group: {
                            name: 'shared'
                        },
                        animation: 150,
                        dataIdAttr: 'data-id',
                        ghostClass: 'blue-background-class',
                        onStart: function ( /**Event*/ evt) {
                            _this.loadingDialog = layer.msg('正在移动...', {
                                timeout: 900 * 1000
                            })
                        },
                        onEnd: function (evt) {

                            /**
                             * 这里区分两种情况，一种是跨列移动，一种非跨列移动
                             * 如果跨列移动，可以利用以下四个参数来实现
                             *
                             * @param pastNodeArrStr    被移出的列的子节点排序
                             * @param pastNodeId        被移出的列的节点id
                             * @param currentNodeArrStr 移入的列的子节点排序
                             * @param currentNodeId     移入的列的节点id
                             *
                             * 假如非跨列，此时被移出的和移入的为同一个，使用前两个参数或者后两个参数都可以实现
                             */
                            layer.close(_this.loadingDialog)
                            var pastNodeArrStr = Array.from(evt.from.querySelectorAll('div[data-id]')).map(function (item) {
                                return item.dataset.id
                            }).join(',')
                            var pastNodeId = evt.from.dataset.id
                            var currentNodeArrStr = Array.from(evt.to.querySelectorAll('div[data-id]')).map(function (item) {
                                return item.dataset.id
                            }).join(',')
                            var currentNodeId = evt.to.dataset.id
                            // 二级菜单的排序
                            _this.updateFlow(pastNodeArrStr, pastNodeId, currentNodeArrStr, currentNodeId)
                            // 二级菜单的排序
                            // _this.updateFlow(evt.item.dataset.pid, arr.join(','));
                        }
                    })
                }

            })
        },
        init: function () {
            document.body.ondrop = function (event) {
                event.preventDefault();
                event.stopPropagation();
            }
            var webUser = new WebUser();
            if (webUser.CCBPMRunModel == 1) {
                window.location.href = window.location.href.replace("Flows.htm", "FlowTree.htm");
            }

            var handler = new HttpHandler("BP.WF.HttpHandler.WF_Portal");
            var fss = handler.DoMethodReturnJSON("Flows_InitSort");

            var nodes = fss;
            nodes = nodes.filter(function (item) {
                console.log(item)
                return item.Name !== '流程树';
            })

            var handler = new HttpHandler("BP.WF.HttpHandler.WF_Portal");
            var fls = handler.DoMethodReturnJSON("Flows_Init");

            for (var i = 0; i < nodes.length; i++) {
                var fs = nodes[i];
                fs.open = false;
                fs.children = [];
                if (parseInt(fs.ParentNo) === 0 || fs.Name === '流程树') continue;

                for (var j = 0; j < fls.length; j++) {
                    var flow = fls[j];
                    if (fs.No !== flow.FK_FlowSort)
                        continue;
                    fs.children.push(flow);
                }
            }

            this.flowNodes = nodes;
            this.bindMenu();
            this.initSortArea();
        }
    },
    mounted: function () {
        // fix firefox bug
        this.init();

    }
})
function AppendFlowToFlowSort(flowSort,no,name) {
    baseInfo.flowNodes.forEach(item => {
        if (item.No === flowSort) {
            if (item.children == null) item.children = [];
            item.children.push({
                No: no,
                Name: name,
                WorkModel: 0,
                FK_FlowSort: flowSort,
                WFSta2: 0,
                WFSta3: 0,
                WFSta5: 0,
                Ver: ''
            })
        }
    });
    baseInfo.bindMenu();
    baseInfo.initSortArea();
}
function addTab(no, name, url) {
    window.top.vm.openTab(name, url);
}