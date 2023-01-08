
new Vue({
    el: '#mapAttrParas',
    data: {
        mapAttrs: [],
        expandAll: false,
        loadingDialog: false
    },
    watch: {
        expandAll(val) {
            this.expandMenus(val);
        }
    },
    methods: {
        expandMenus: function (status) {
            for (var i = 0; i < this.mapAttrs.length; i++) {
                this.mapAttrs[i].open = status
            }
        },
        bindMenu: function () {
            var _this = this
            layui.use('dropdown', function () {
                var dropdown = layui.dropdown
                var topNodeItems = [
                    { title: '<i class=icon-plus></i> 新建方法', id: "NewmapAttrByGroup" },
                    { title: '<i class=icon-star></i> 目录属性', id: "EditSort" },
                    { title: '<i class=icon-folder></i> 新建目录', id: "NewSort" },
                    { title: '<i class=icon-pencil></i> 修改名称', id: "EditSortName" },
                    { title: '<i class=icon-close></i> 删除目录', id: "DeleteIt" }
                ]
                var tRenderOptions = [{
                    elem: '.item-top-dp',
                    trigger: 'contextmenu',
                    data: topNodeItems,
                    click: function (data, oThis) {

                        var obj = $(this.elem)[0].dataset;
                        console.log(obj);

                        _this.topNodeOption(data.id, obj.no, obj.name, obj.idx);
                    }
                }, {
                    elem: '.t-btn',
                    trigger: 'click',
                    data: topNodeItems,
                    click: function (data, oThis) {

                        var obj = $(this.elem)[0].dataset;
                        console.log(obj);
                        _this.topNodeOption(data.id, obj.no, obj.name, obj.idx);
                    }
                }]

                dropdown.render(tRenderOptions[0]);
                dropdown.render(tRenderOptions[1]);

                var childNodeMenuItems = [
                    { title: '<i class=icon-star></i> 方法属性', id: "Attr" },
                 //   { title: '<i class=icon-plus ></i> 新建方法', id: "NewmapAttr" },
                    /*    { title: '<i class=icon-settings></i> 设计方法', id: "Designer" },*/
                    /*    { title: '<i class=icon-docs></i> 复制方法', id: "Copy" },*/
                    { title: '<i class=icon-pencil></i> 修改名称', id: "EditmapAttrName" },
                    { title: '<i class=icon-close></i> 删除方法', id: "Delete" }
                ]
                var cRenderOptions = [{
                    elem: '.item-name-dp',
                    trigger: 'contextmenu',
                    data: childNodeMenuItems,
                    click: function (data, othis) {
                        var obj = $(this.elem)[0].dataset;
                        console.log(obj);


                        _this.childNodeOption(data.id, $(this.elem)[0].dataset.no, $(this.elem)[0].dataset.name, $(this.elem)[0].dataset.pidx, $(this.elem)[0].dataset.idx)

                    }
                }, {
                    elem: '.c-btn',
                    trigger: 'click',
                    data: childNodeMenuItems,
                    click: function (data, othis) {

                        var obj = $(this.elem)[0].dataset;
                        var no = obj.No;
                        if (no == undefined)
                            no = obj.no;

                        var idx = obj.idx;

                        //console.log(obj);

                        _this.childNodeOption(data.id, no, $(this.elem)[0].dataset.name, $(this.elem)[0].dataset.pidx, idx)
                    }
                }]
                dropdown.render(cRenderOptions[0]);
                dropdown.render(cRenderOptions[1]);
            })
        },
        MethodAttr: function (no) {

            console.log(en);

            var en = new Entity("BP.CCBill.Template.Collection", no);

            var enName = "BP.CCBill.Template.Collection";

            if (en.MethodModel == "Func") enName = "BP.CCBill.Template.CollectionFunc";
            if (en.MethodModel == "Link") enName = "BP.CCBill.Template.CollectionLink";
            if (en.MethodModel == "QRCode") enName = "BP.CCBill.Template.CollectionQRCode";
            if (en.MethodModel == "FlowBaseData") enName = "BP.CCBill.Template.CollectionFlowBaseData";
            if (en.MethodModel == "FlowNewEntity") enName = "BP.CCBill.Template.CollectionFlowNewEntity";

            if (en.MethodModel === "SingleDictGenerWorkFlows" || en.MethodModel === "SingleDictGenerWorkFlow")
                enName = "BP.CCBill.Template.MethodSingleDictGenerWorkFlow";

            if (en.MethodModel == "FlowEtc")
                enName = "BP.CCBill.Template.FlowEtc";

            var url = "../../Comm/En.htm?EnName=" + enName + "&No=" + en.No + "&From=Ver2021";
            OpenLayuiDialog(url, "", 100000, 0, null, false);

        },
        DeleteIt: function (no) {

            if (window.confirm("确定要删除吗?") == false)
                return;
            var en = new Entity("BP.CCBill.Template.Collection", no);
            var data = en.Delete();
            if (en.MethodModel === 'FlowEntityBatchStart') {
                //删除对应的方法
                var method = new Entity("BP.CCBill.Template.Method",en.FrmID+"_"+en.FlowNo);
                method.Delete();
            }
            layer.msg(data);

            //如果有错误.
            if (data.indexOf("err@") == 0) return;

            setTimeout(function () {
                window.location.reload()
            }, 2000)
        },
        MoveItem(pastNodeArrStr, pastNodeId, currentNodeArrStr, currentNodeId) {
            // todo 需要重新实现接口

            var frmID = GetQueryString("No");
            // 方法排序..
            var handler = new HttpHandler("BP.CCBill.WF_CCBill_Admin");
            handler.AddPara("FrmID", frmID);
            handler.AddPara("MyPKs", currentNodeArrStr);
            var data = handler.DoMethodReturnString("Collection_Mover");

            layer.msg(data);

        },
        initSortArea: function () {
            var _this = this
            this.$nextTick(function () {
                var childSortableContainers = this.$refs['child-row']

                new Sortable(childSortableContainers, {
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
                        _this.MoveItem(pastNodeArrStr, pastNodeId, currentNodeArrStr, currentNodeId)
                    }
                })
            })
        },
        // 是否启用
        changeMethodEnableStatus(method, ctrl) {
            // 当前启用状态

            var en = new Entity("BP.CCBill.Template.Collection", method.No);
            if (en.IsEnable == 0)
                en.IsEnable = 1; // method.IsEnable;
            else
                en.IsEnable = 0; // method.IsEnable;

            en.Update();

            console.log("更新成功..");

        }
    },
    mounted: function () {
        // fix firefox bug
        document.body.ondrop = function (event) {
            event.preventDefault();
            event.stopPropagation();
        }

        var handler = new HttpHandler("BP.CCBill.WF_CCBill_Admin");
        handler.AddPara("FrmID", GetQueryString("FrmID"));
        var ds = handler.DoMethodReturnString("Collection_Init");
        if (ds.indexOf("err@") != -1) {
            layer.alert(data);
            return;
        }

        var ens = JSON.parse(ds);
        var btnStyle = "class='layui-btn layui-btn-primary layui-border-blue layui-btn-xs'";
        ens.forEach(function (en) {

            if (en.Mark === "FlowNewEntity" || en.Mark === "FlowEntityBatchStart"  ) {
                var doc = "<a " + btnStyle + "  href=\"javascript:DesignerFlow('" + en.FlowNo + "','" + en.Name + "');\" >设计流程</a>";
                en.Docs = doc;
            }
            var url = "./PowerCenter.htm?CtrlObj=Menu&CtrlPKVal=" + en.No + "&CtrlGroup=Menu";

            en.enCtrlWayText = "<a " + btnStyle + "  href =\"javascript:OpenLayuiDialog('" + url + "','" + en.No + "','700',0,null,false);\" >权限</a>";
            if (en.MethodID == "Delete" || en.MethodID == "New" || en.MethodID == "Search"
                || en.MethodID == "Group" || en.MethodID == "ExpExcel" || en.MethodID == "ImpExcel")
                en.IsDelete = false;
            else
                en.IsDelete = true;
        })

        this.mapAttrs = ens;

       // console.log(this.mapAttrs);
        this.initSortArea();

        layui.use('form', function () {
            var form = layui.form;
            form.render()
            // form.on("switch(enable)", function (e) {
            //     console.log(e)
            // })
        });
        var _this = this
        setTimeout(function () {
            _this.expandAll = true
        }, 300)
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
//属性.
function AttrFrm(enName, title, pkVal) {
    var url = "../../Comm/En.htm?EnName=" + enName + "&No=" + pkVal;
    title = "";
    OpenLayuiDialog(url, title, 5000, 0, null, false);
    return;
}


function DesignerFlow(no, name) {
    var sid = GetQueryString("Token");
    var webUser = new WebUser();
    var url = basePath+"/WF/Admin/CCBPMDesigner/Designer.htm?FK_Flow=" + no + "&UserNo=" + webUser.No + "&Token=" + sid + "&OrgNo=" + webUser.OrgNo + "&From=Ver2021";
    window.top.vm.openTab(name, url);
}

function addTab(no, name, url) {
    window.top.vm.openTab(name, url);
}



