new Vue({
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
        expandMenus: function(status) {
            for (var i = 0; i < this.flowNodes.length; i++) {
                this.flowNodes[i].open = status
            }
        },
        bindMenu: function() {
            var _this = this
            layui.use('dropdown', function() {
                var dropdown = layui.dropdown
                var topNodeItems = [
                    { title: '<i class=icon-plus></i> 新建流程', id: "NewFlow", Icon: "icon-plus" },
                    { title: '<i class=icon-star></i> 目录属性', id: "EditSort", Icon: "icon-options" },
                    { title: '<i class=icon-folder></i> 新建目录', id: "NewSort", Icon: "icon-magnifier-add" },
                    { title: '<i class=icon-pencil></i> 修改名称', id: "EditSortName", Icon: "icon-magnifier-add" },
                    { title: '<i class=icon-share-alt ></i> 导入流程模版', id: "ImpFlowTemplate", Icon: "icon-plus" },
                    //{ title: '新建下级目录', id: 5 },
                    { title: '<i class=icon-close></i> 删除目录', id: "DeleteSort", Icon: "icon-close" }
                ]
                var tRenderOptions = [{
                    elem: '.item-top-dp',
                    trigger: 'contextmenu',
                    data: topNodeItems,
                    click: function(data, oThis) {
                        _this.topNodeOption(data.id, $(this.elem)[0].dataset.no, $(this.elem)[0].dataset.name, $(this.elem)[0].dataset.idx)
                    }
                }, {
                    elem: '.t-btn',
                    trigger: 'click',
                    data: topNodeItems,
                    click: function(data, oThis) {
                        _this.topNodeOption(data.id, $(this.elem)[0].dataset.no, $(this.elem)[0].dataset.name, $(this.elem)[0].dataset.idx)
                    }
                }]

                dropdown.render(tRenderOptions[0]);
                dropdown.render(tRenderOptions[1]);

                var childNodeMenuItems = [
                    { title: '<i class=icon-star></i> 流程属性', id: "Attr" },
                    { title: '<i class=icon-settings></i> 设计流程', id: "Designer" },
                    { title: '<i class=icon-plane></i> 测试容器', id: "Start" },
                    { title: '<i class=icon-docs></i> 复制流程', id: "Copy"},
                    { title: '<i class=icon-pencil></i> 修改名称', id: "EditFlowName"},
                    { title: '<i class=icon-close></i> 删除流程', id: "Delete" }
                ]
                var cRenderOptions = [{
                    elem: '.item-name-dp',
                    trigger: 'contextmenu',
                    data: childNodeMenuItems,
                    click: function(data, othis) {
                        _this.childNodeOption(data.id, $(this.elem)[0].dataset.no, $(this.elem)[0].dataset.name, $(this.elem)[0].dataset.pidx, $(this.elem)[0].dataset.idx)
                    }
                }, {
                    elem: '.c-btn',
                    trigger: 'click',
                    data: childNodeMenuItems,
                    click: function(data, othis) {
                        _this.childNodeOption(data.id, $(this.elem)[0].dataset.no, $(this.elem)[0].dataset.name, $(this.elem)[0].dataset.pidx, $(this.elem)[0].dataset.idx)
                    }
                }]
                dropdown.render(cRenderOptions[0]);
                dropdown.render(cRenderOptions[1]);
            })
        },

        //如果w=0 则是100%的宽度.
        openLayer: function(uri, name, w, h) {
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
                content: [uri, 'no'],
                area: [w + 'px', h + 'px'],
                offset: 'rb',
                shadeClose: true
            })
        },

        Designer: function(no, name) {
            var sid = GetQueryString("SID");
            var webUser = new WebUser();
            var url = "../Admin/CCBPMDesigner/Designer.htm?FK_Flow=" + no + "&UserNo=" + webUser.No + "&SID=" + sid + "&OrgNo=" + webUser.OrgNo + "&From=Ver2021";
            window.top.vm.openTab(name, url);
        },
        EditSort: function(no, name) {
            var url = "../Comm/EnOnly.htm?EnName=BP.WF.Template.FlowSort&No=" + no;
            this.openLayer(url, "目录:" + name);
        },
        testFlow: function(no, name) {
            var url = "../Admin/TestingContainer/TestFlow2020.htm?FK_Flow=" + no;
            window.top.vm.fullScreenOpen(url, name);
            // this.openLayer(url, name);
        },
        flowAttr: function(no, name) {
            var url = "../Comm/En.htm?EnName=BP.WF.Template.FlowExt&No=" + no;
            window.top.vm.openTab(name, url);
            //this.openLayer(url, name,900);
        },

        copyFlow: function(no) {
            if (window.confirm("确定要执行流程复制吗?") == false)
                return;
            var flow = new Entity("BP.WF.Flow", no);
            var data = flow.DoMethodReturnString("DoCopy");
            layer.msg(data);
            setTimeout(function() {
                window.location.reload();
            }, 2000);
        },
        DeleteFlow: function(no, pidx, idx) {
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

        childNodeOption: function(key, data, name, pidx, idx) {

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

        topNodeOption: function(key, data, name, idx) {

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
        NewFlow: function(data, name) {

            ////  if (runModelType == 0)
            //   url = "../CCBPMDesigner/FlowDevModel/Default.htm?SortNo=" + flowSort + "&s=" + Math.random();
            //else
            url = "../Admin/CCBPMDesigner/FlowDevModel/Default.htm?SortNo=" + data + "&From=Flows.htm&RunModel=1&s=" + Math.random();
            addTab("NewFlow", "新建流程", url);

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
        ImpFlowTemplate: function(data) {
            var fk_flow = data;
            url = "./../Admin/AttrFlow/Imp.htm?FK_Flow=" + fk_flow + "&Lang=CH";
            addTab("ImpFlowTemplate", "导入流程模版", url);
        },
        DeleteSort: function(no) {

            if (window.confirm("确定要删除吗?") == false)
                return;

            var en = new Entity("BP.WF.Template.FlowSort", no);
            var data = en.Delete();
            layer.msg(data);

            //如果有错误.
            if (data.indexOf("err@") == 0)
                return;

            setTimeout(function() {
                window.location.reload()
            }, 2000)
        },

        NewSort: function(currentElem, sameLevel) {

            //只能创建同级.
            sameLevel = true;

            //例子2
            layer.prompt({
                value: '',
                title: '新建' + (sameLevel ? '同级' : '子级') + '流程类别',
            }, function(value, index, elem) {
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

                setTimeout(function() {
                    window.location.reload();
                }, 2000);
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
            // todo 需要重新实现接口

            return
            // 流程排序..
            console.log(sortNo, flowNos);
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_Portal");
            handler.AddPara("SortNo", sortNo);
            handler.AddPara("EnNos", flowNos);
            var data = handler.DoMethodReturnString("Flows_Move");
            layer.msg(data)
        },
        initSortArea: function() {
            var _this = this
            this.$nextTick(function() {
                var sortContainer = this.$refs['sort-main']
                new Sortable(sortContainer, {
                    animation: 150,
                    dataIdAttr: 'data-id',
                    ghostClass: 'blue-background-class',
                    onStart: function( /**Event*/ evt) {
                        _this.loadingDialog = layer.msg('正在移动...', {
                            timeout: 900 * 1000
                        })
                    },
                    onEnd: function(evt) {
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
                        onStart: function( /**Event*/ evt) {
                            _this.loadingDialog = layer.msg('正在移动...', {
                                timeout: 900 * 1000
                            })
                        },
                        onEnd: function(evt) {

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
                            var pastNodeArrStr = Array.from(evt.from.querySelectorAll('div[data-id]')).map(function(item) {
                                return item.dataset.id
                            }).join(',')
                            var pastNodeId = evt.from.dataset.pid
                            var currentNodeArrStr = Array.from(evt.to.querySelectorAll('div[data-id]')).map(function(item) {
                                return item.dataset.id
                            }).join(',')
                            var currentNodeId = evt.to.dataset.pid
                                // 二级菜单的排序
                            _this.updateFlow(pastNodeArrStr, pastNodeId, currentNodeArrStr, currentNodeId)
                                // 二级菜单的排序
                                // _this.updateFlow(evt.item.dataset.pid, arr.join(','));
                        }
                    })
                }

            })
        }
    },
    mounted: function() {
        // fix firefox bug
        document.body.ondrop = function(event) {
            event.preventDefault();
            event.stopPropagation();
        }

        var handler = new HttpHandler("BP.WF.HttpHandler.WF_Portal");
        var fss = handler.DoMethodReturnJSON("Flows_InitSort");

        var nodes = fss;
        nodes = nodes.filter(function(item) {
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
})

function addTab(no, name, url) {
    window.top.vm.openTab(name, url);
}