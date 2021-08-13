new Vue({
    el: '#flow',
    data: {
        flowNodes: [],
        expandAll: false
    },
    watch: {
        expandAll: function(val) {
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
                    { title: '<i class=icon-plus></i> 新建表单', id: "NewFlow", Icon: "icon-plus" },
                    { title: '<i class=icon-star></i> 重命名', id: "EditSort", Icon: "icon-options" },
                    { title: '<i class=icon-folder></i> 新建目录', id: "NewSort", Icon: "icon-magnifier-add" },
                    { title: '<i class=icon-share-alt ></i> 导入表单模版', id: "ImpFlowTemplate", Icon: "icon-plus" },
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
                    { title: '<i class=icon-star></i> 表单属性', id: "Attr", Icon: "icon-options" },
                    { title: '<i class=icon-settings></i> 设计表单', id: "Designer", Icon: "icon-settings" },
                    { title: '<i class=icon-plane></i> 运行表单', id: "Start", Icon: "icon-paper-plane" },
                    { title: '<i class=icon-docs></i> 复制表单', id: "Copy", Icon: "icon-docs" },
                    { title: '<i class=icon-close></i> 删除表单', id: "Delete", Icon: "icon-close" }
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
            var url = "../Admin/CCFormDesigner/GoToFrmDesigner.htm?FK_MapData=" + no + "&FrmID=" + no + "&UserNo=" + webUser.No + "&SID=" + sid + "&OrgNo=" + webUser.OrgNo + "&From=Ver2021";
            window.top.vm.openTab(name, url);
        },
        EditSort: function(no, name) {

            var val = prompt("请输入名称", name);
            if (val == null || val == '')
                return;

            var en = new Entity("BP.WF.Template.SysFormTree", no);
            en.Name = val;
            en.Update();

            // alert("修改成功");
            window.location.href = window.location.href;
            return;


            // var url = "../Comm/EnOnly.htm?EnName=BP.WF.Template.FlowSort&No=" + no;
            // this.openLayer(url, "目录:" + name);
        },
        StartFrm: function(no, name) {
            var sid = GetQueryString("SID");
            var webUser = new WebUser();
            var url = "../Admin/CCFormDesigner/GoToRunFrm.htm?FK_MapData=" + no + "&FrmID=" + no + "&UserNo=" + webUser.No + "&SID=" + sid + "&OrgNo=" + webUser.OrgNo + "&From=Ver2021";
            //var url = "../Comm/En.htm?EnName=BP.WF.Template.FlowExt&No=" + no;
            window.top.vm.openTab(name, url);

            // var url = "../Admin/TestingContainer/TestFlow2020.htm?FK_Flow=" + no;
            //  window.top.vm.fullScreenOpen(url, name);
            // this.openLayer(url, name);
        },
        flowAttr: function(no, name) {
            var sid = GetQueryString("SID");
            var webUser = new WebUser();
            var url = "../Admin/CCFormDesigner/GoToFrmAttr.htm?FK_MapData=" + no + "&FrmID=" + no + "&UserNo=" + webUser.No + "&SID=" + sid + "&OrgNo=" + webUser.OrgNo + "&From=Ver2021";
            //var url = "../Comm/En.htm?EnName=BP.WF.Template.FlowExt&No=" + no;
            window.top.vm.openTab(name, url);
            //this.openLayer(url, name,900);
        },

        copyFrm: function(no) {
           /* if (window.confirm("确定要执行表单复制吗?") == false)
                return;*/
            var flow = new Entity("BP.Sys.MapData", no);
            var frmID = window.prompt("表单ID:" + no + "Copy");
            if (frmID == undefined || frmID == null || frmID == '') return;

            var frmName = window.prompt("表单名称:" + flow.Name + "Copy");
            if (frmName == undefined || frmName == null || frmName == '') return;
            var data = flow.DoMethodReturnString("DoCopy", frmID + '~' + frmName);
            layer.msg(data);
            setTimeout(function() {
                window.location.reload();
            }, 2000);
        },
        DeleteFlow: function(no, pidx, idx) {
            var msg = "提示: 确定要删除该表单吗?";
            //   msg += "\t\n1.如果该流程下有实例，您不能删除。";
            //  msg += "\t\n2.该流程为子流程的时候，被引用也不能删除.";
            if (window.confirm(msg) == false)
                return;

            var load = layer.msg("正在处理,请稍候...", {
                icon: 16,
                anim: 5
            })

            //开始执行删除.
            var en = new Entity("BP.Sys.MapData", no);
            en.Delete();
            //  var data = flow.DoMethodReturnString("DoDelete");

            layer.msg(data);
            if (data.indexOf("err@") == 0)
                return;

            layer.close(load);

            window.location.href = window.location.href;
            return;

            this.flowNodes[pidx].children.splice(idx, 1);
            var leaveItems = this.flowNodes[pidx].children;
            this.$set(this.flowNodes[pidx], 'children', leaveItems);
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
                    this.StartFrm(data, name);
                    break;
                case "Copy":
                    this.copyFrm(data);
                    break;
                case "Delete":
                    this.DeleteFlow(data, pidx, idx);
                    break;
            }
        },

        topNodeOption: function(key, data, name) {

            switch (key) {
                case "EditSort":
                    this.EditSort(data, name);
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
        NewFlow: function(data, name) {

            url = "../Admin/FoolFormDesigner/NewFrmGuide.htm?SortNo=" + data + "&From=Flows.htm&RunModel=1&s=" + Math.random();
            addTab("NewFlow", "新建表单", url);

        },
        ImpFlowTemplate: function(data) {

            var url = "../Admin/Template/ImpFrmLocal.htm?SortNo=" + data;
            // url = "../Admin/FoolFormDesigner/NewFrmGuide.htm?SortNo=" + data + "&From=Flows.htm&RunModel=1&s=" + Math.random();
            // url = "./../Admin/AttrFlow/Imp.htm?FK_Flow=" + fk_flow + "&Lang=CH";
            addTab("ImpFlowTemplate", "导入表单模版", url);
        },

        DeleteSort: function(no) {

            if (window.confirm("确定要删除吗?") == false)
                return;

            var en = new Entity("BP.WF.Template.SysFormTree", no);
            var data = en.Delete();
            layer.msg(data);

            //如果有错误.
            if (data.indexOf("err@") == 0)
                return;

            setTimeout(function() {
                window.location.reload();
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
                var en = new Entity("BP.WF.Template.SysFormTree", currentElem);
                var data = "";
                if (sameLevel == true) {
                    data = en.DoMethodReturnString("DoCreateSameLevelNodeIt", value);
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
        updateSort: function(rootNo, sortNos) {
            // 目录排序..
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_Portal");
            handler.AddPara("RootNo", rootNo);
            handler.AddPara("SortNos", sortNos);
            var data = handler.DoMethodReturnString("Frms_MoveSort");
            layer.msg(data)
        },
        updateFlow: function(sortNo, flowNos) {

            // alert(sortNo);
            // alert(flowNos);

            // 流程排序..
            console.log(sortNo, flowNos);
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_Portal");
            handler.AddPara("SortNo", sortNo);
            handler.AddPara("EnNos", flowNos);
            var data = handler.DoMethodReturnString("Frms_Move");
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
                    onEnd: function(evt) {
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
                        animation: 150,
                        dataIdAttr: 'data-id',
                        ghostClass: 'blue-background-class',
                        onEnd: function(evt) {
                            var arr = this.toArray();
                            // 二级菜单的排序
                            _this.updateFlow(evt.item.dataset.pid, arr.join(','));
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
        var fss = handler.DoMethodReturnJSON("Frms_InitSort");

        var nodes = fss;
        nodes = nodes.filter(function(item) {
            console.log(item)
            return item.Name !== '表单树';
        })

        var handler = new HttpHandler("BP.WF.HttpHandler.WF_Portal");
        var fls = handler.DoMethodReturnJSON("Frms_Init");

        for (var i = 0; i < nodes.length; i++) {
            var fs = nodes[i];
            fs.open = false;
            fs.children = [];
            if (parseInt(fs.ParentNo) === 0 || fs.Name === '表单树') continue;

            for (var j = 0; j < fls.length; j++) {
                var flow = fls[j];
                if (fs.No !== flow.FK_FormTree)
                    continue;

                if (flow.FrmType == 1) flow.FrmType = "傻瓜表单";
                if (flow.FrmType == 0) flow.FrmType = "傻瓜表单";
                if (flow.FrmType == 2) flow.FrmType = "自由表单";
                if (flow.FrmType == 3) flow.FrmType = "嵌入式表单";
                if (flow.FrmType == 4) flow.FrmType = "Word表单";
                if (flow.FrmType == 5) flow.FrmType = "在线编辑模式Excel表单";
                if (flow.FrmType == 6) flow.FrmType = "VSTO模式Excel表单";
                if (flow.FrmType == 7) flow.FrmType = "实体类组件";
                if (flow.FrmType == 8) flow.FrmType = "开发者表单";

                if (flow.Icon == "" || flow.Icon == null) {
                    if (flow.EntityType == 0) flow.Icon = "icon-flag";
                    if (flow.EntityType == 1) flow.Icon = "icon-info";
                    if (flow.EntityType == 2) flow.Icon = "icon-doc";
                    if (flow.EntityType == 3) flow.Icon = "icon-organization";
                }

                if (flow.EntityType === 0) flow.EntityType = "独立表单";
                if (flow.EntityType === 1) flow.EntityType = "单据";
                if (flow.EntityType === 2) flow.EntityType = "实体";
                if (flow.EntityType === 3) flow.EntityType = "树结构实体";

                //   if (flow.FrmType == 9) flow.FrmType = "傻瓜表单";


                fs.children.push(flow);
            }
        }

        this.flowNodes = nodes;
        this.bindMenu()
        this.initSortArea()

    }
})

function addTab(no, name, url) {

    window.top.vm.openTab(name, url);
}