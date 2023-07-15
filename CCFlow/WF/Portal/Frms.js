var baseInfo = new Vue({
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
                    { title: '<i class=icon-share-alt ></i> 批量导出表单模版', id: "BatchExpFrmTemplate", Icon: "icon-plus" },
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
                content: [uri, 'auto'],
                area: [w + 'px', h + 'px'],
                offset: 'rb',
                shadeClose: true
            })
        },
        Designer: function(no, name) {
            var sid = GetQueryString("Token");
            var webUser = new WebUser();
            var url = basePath + "/WF/Admin/FoolFormDesigner/GoToFrmDesigner.htm?FK_MapData=" + no + "&FrmID=" + no + "&UserNo=" + webUser.No + "&Token=" + sid + "&OrgNo=" + webUser.OrgNo + "&From=Ver2021";
            try {
                window.top.vm.openTab(name, url);
            } catch (e) {
                WinOpenFull(url);
            }
        },
        EditSort: function(no, name) {

            var val = prompt("请输入名称", name);
            if (val == null || val == '')
                return;

            var en = new Entity("BP.WF.Template.SysFormTree", no);
            en.Name = val;
            en.Update();

            // alert("修改成功");
            Reload();
            return;

            // var url = "../Comm/EnOnly.htm?EnName=BP.WF.Template.FlowSort&No=" + no;
            // this.openLayer(url, "目录:" + name);
        },
        StartFrm: function(no, name) {
            var sid = GetQueryString("Token");
            var webUser = new WebUser();
            var en = new Entity("BP.Sys.MapData", no);
            if (en.EntityType == 0) {
                layer.alert("表单:[" + en.Name + "]是独立表单不能运行,如果要调用表单，请参考/WF/CCBill/Demo/index.htm");
                return;
            }
            var url = "";
            if (en.EntityType == 1)
                url = basePath + "/WF/CCBill/SearchDict.htm?FrmID=" + en.No;
            if (en.EntityType == 2)
                url = basePath + "/WF/CCBill/SearchBill.htm?FrmID=" + en.No;
            if (en.EntityType == 3)
                url = basePath + "/WF/CCBill/SearchTree.htm?FrmID=" + en.No;
            
            //var url = basePath + "/WF/Admin/FoolFormDesigner/GoToRunFrm.htm?FK_MapData=" + no + "&FrmID=" + no + "&UserNo=" + webUser.No + "&Token=" + sid + "&OrgNo=" + webUser.OrgNo + "&From=Ver2021";
            //var url = "../Comm/En.htm?EnName=BP.WF.Template.FlowExt&No=" + no;
            try {
                window.top.vm.openTab(name, url);
            } catch (e) {
                WinOpenFull(url);
            }
           

            // var url = "../Admin/TestingContainer/TestFlow2020.htm?FK_Flow=" + no;
            //  window.top.vm.fullScreenOpen(url, name);
            // this.openLayer(url, name);
        },
        flowAttr: function (no, name) {
            var sid = GetQueryString("Token");
            var webUser = new WebUser();
            //var url = basePath + "/WF/Admin/FoolFormDesigner/GoToFrmAttr.htm?FK_MapData=" + no + "&FrmID=" + no + "&UserNo=" + webUser.No + "&Token=" + sid + "&OrgNo=" + webUser.OrgNo + "&From=Ver2021";
            //var url = "../Comm/En.htm?EnName=BP.WF.Template.FlowExt&No=" + no;
            var url = "/WF/Comm/RefFunc/En.htm?EnName=BP.WF.Template.Frm.MapFrmFool&PKVal=" + no + "&s=" + Math.random();
            try {
                window.top.vm.openTab(name, url);
            } catch (e) {
                WinOpenFull(url);
            }
            //this.openLayer(url, name,900);
        },

        copyFrm: function(no) {
           /* if (window.confirm("确定要执行表单复制吗?") == false)
                return;*/
            var flow = new Entity("BP.Sys.MapData", no);
            var frmID = promptGener("表单ID:" + no + "Copy", no + "Copy");

            if (frmID == undefined || frmID == null || frmID == '') return;

            var frmName = promptGener("表单名称:" + flow.Name + "Copy", flow.Name + "Copy");
            if (frmName == undefined || frmName == null || frmName == '') return;
            var data = flow.DoMethodReturnString("DoCopy", frmID + '~' + frmName);
            layer.msg(data);
            Reload();
            //setTimeout(function() {
            //    window.location.reload();
            //}, 800);

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
            var data = en.Delete() || "";
            if (data != "")
                layer.msg(data);
            if (data.indexOf("err@") == 0)
                return;
            layer.close(load);
            Reload();
            return;
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
                case "BatchExpFrmTemplate":
                    this.BatchExpFrmTemplate(data, name);
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

            url = basePath + "/WF/Admin/FoolFormDesigner/NewFrmGuide.htm?SortNo=" + data + "&From=Frms.htm&RunModel=1&s=" + Math.random();
            this.openLayer(url, "新建表单");
            //addTab("NewFlow", "新建表单", url);

        },
        ImpFlowTemplate: function(data) {

            var url = basePath + "/WF/Admin/Template/ImpFrmLocal.htm?SortNo=" + data;
            // url = "../Admin/FoolFormDesigner/NewFrmGuide.htm?SortNo=" + data + "&From=Flows.htm&RunModel=1&s=" + Math.random();
            // url = "./../Admin/AttrFlow/Imp.htm?FK_Flow=" + fk_flow + "&Lang=CH";
            addTab("ImpFlowTemplate", "导入表单模版", url);
        },
        BatchExpFrmTemplate: function (frmTreeNo,frmTreeName) {
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_Portal");
            handler.AddPara("FK_FrmTree", frmTreeNo);
            handler.AddPara("FrmTreeName", frmTreeName);
            var data = handler.DoMethodReturnString("Form_BatchExpFrmTemplate");
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
                title: '新建' + (sameLevel ? '同级' : '子级') + '表单类别',
            }, function(value, index, elem) {
                layer.close(index);
                var en = new Entity("BP.WF.Template.SysFormTree", currentElem);
                var data = "";
                if (sameLevel == true) {
                    data = en.DoMethodReturnString("DoCreateSameLevelNodeIt", value);
                } else {
                    data = en.DoMethodReturnString("DoCreateSubFormNodeMy", value);
                }

                layer.msg("创建成功" + data);
                //this.EditSort(data, "编辑");
                //return;

                setTimeout(function() {
                    window.location.reload();
                }, 800);
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
        },
        changeFrmInfo: function (en) {
            if (en.FrmType == 1) en.FrmType = "傻瓜表单";
            if (en.FrmType == 0) en.FrmType = "傻瓜表单";
            if (en.FrmType == 2) en.FrmType = "自由表单";
            if (en.FrmType == 3) en.FrmType = "嵌入式表单";
            if (en.FrmType == 4) en.FrmType = "Word表单";
            if (en.FrmType == 5) en.FrmType = "在线编辑模式Excel表单";
            if (en.FrmType == 6) en.FrmType = "VSTO模式Excel表单";
            if (en.FrmType == 7) en.FrmType = "实体类组件";
            if (en.FrmType == 8) en.FrmType = "开发者表单";
            if (en.FrmType == 10) en.FrmType = "章节表单";


            if (en.Icon == "" || en.Icon == null) {
                if (en.EntityType == 0) en.Icon = "icon-flag";
                if (en.EntityType == 1) en.Icon = "icon-info";
                if (en.EntityType == 2) en.Icon = "icon-doc";
                if (en.EntityType == 3) en.Icon = "icon-organization";
            }

            if (en.EntityType === 0) en.EntityType = "独立表单";
            if (en.EntityType === 1) en.EntityType = "单据";
            if (en.EntityType === 2) en.EntityType = "实体";
            if (en.EntityType === 3) en.EntityType = "树结构实体";
        },
        init: function () {
            // fix firefox bug
            document.body.ondrop = function (event) {
                event.preventDefault();
                event.stopPropagation();
            }
            var webUser = new WebUser();
            window.location.href = "FrmTree.htm";
            return; 
            
            //var handler = new HttpHandler("BP.WF.HttpHandler.WF_Portal");
            //var fss = handler.DoMethodReturnJSON("Frms_InitSort");

            //查询全部.
            var fss = new Entities("BP.WF.Template.SysFormTrees");
            fss.RetrieveAll();
         
            var nodes = fss;
            nodes = nodes.filter(function (item) {
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
                    this.changeFrmInfo(flow);
                    fs.children.push(flow);
                }
            }

            this.flowNodes = nodes;
            this.bindMenu();
            this.initSortArea();
        }
    },
    mounted: function() {
        this.init();
    }
})
function AppendFrmToFormTree(sort, no, name) {
    baseInfo.flowNodes.forEach(item => {
        if (item.No === sort) {
            if (item.children == null) item.children = [];
            var en = new Entity("BP.WF.Template.Frm.MapDataExt", no);
            baseInfo.changeFrmInfo(en);
            item.children.push({
                No: no,
                Name: name,
                FrmType: en.FrmType,
                FK_FormTree: en.FK_FormTree,
                PTable: en.PTable,
                DBSrc: en.DBSrc,
                Icon: en.Icon,
                EntityType: en.EntityType,
                Ver:en.Ver
            })
        }
    });
    baseInfo.bindMenu();
    baseInfo.initSortArea();
}
function addTab(no, name, url) {

    window.top.vm.openTab(name, url);
}