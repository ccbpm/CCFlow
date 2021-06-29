
new Vue({
    el: '#method',
    data: {
        flowNodes: [],
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
            for (var i = 0; i < this.flowNodes.length; i++) {
                this.flowNodes[i].open = status
            }
        },
        bindMenu: function () {
            var _this = this
            layui.use('dropdown', function () {
                var dropdown = layui.dropdown
                var topNodeItems = [
                    { title: '<i class=icon-plus></i> 查询', id: "NewFlow" },
                    { title: '<i class=icon-star></i> 分析', id: "EditSort" }
                   
                ]
                var tRenderOptions = [{
                    elem: '.item-top-dp',
                    trigger: 'contextmenu',
                    data: topNodeItems,
                    click: function (data, oThis) {
                        _this.topNodeOption(data.id, $(this.elem)[0].dataset.no, $(this.elem)[0].dataset.name, $(this.elem)[0].dataset.idx);
                    }
                }, {
                    elem: '.t-btn',
                    trigger: 'click',
                    data: topNodeItems,
                    click: function (data, oThis) {
                        _this.topNodeOption(data.id, $(this.elem)[0].dataset.no, $(this.elem)[0].dataset.name, $(this.elem)[0].dataset.idx);
                    }
                }]

                dropdown.render(tRenderOptions[0]);
                dropdown.render(tRenderOptions[1]);

                var childNodeMenuItems = [
                    { title: '<i class=icon-star></i> 查看表单', id: "Attr" },
                    { title: '<i class=icon-plane></i> 移交', id: "Start" },
                    { title: '<i class=icon-settings></i> 退回', id: "Designer" },
                    { title: '<i class=icon-docs></i> 回滚', id: "Copy" },
                    { title: '<i class=icon-pencil></i> 修改', id: "EditFlowName" },
                    { title: '<i class=icon-close></i> 删除方法', id: "Delete" }
                ]
                var cRenderOptions = [{
                    elem: '.item-name-dp',
                    trigger: 'contextmenu',
                    data: childNodeMenuItems,
                    click: function (data, othis) {
                        _this.childNodeOption(data.id, $(this.elem)[0].dataset.mypk, $(this.elem)[0].dataset.name, $(this.elem)[0].dataset.pidx, $(this.elem)[0].dataset.idx)
                    }
                }, {
                    elem: '.c-btn',
                    trigger: 'click',
                    data: childNodeMenuItems,
                    click: function (data, othis) {
                        _this.childNodeOption(data.id, $(this.elem)[0].dataset.mypk, $(this.elem)[0].dataset.name, $(this.elem)[0].dataset.pidx, $(this.elem)[0].dataset.idx)
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
                content: [uri, 'no'],
                area: [w + 'px', h + 'px'],
                offset: 'rb',
                shadeClose: true
            })
        },

        MethodAttr: function (key, data, name, pidx, idx) {

            var en = new Entity("BP.CCBill.Template.Method", data);

            var enName = "BP.CCBill.Template.Method";

            if (en.MethodModel == "Func")
                enName = "BP.CCBill.Template.MethodFunc";

            if (en.MethodModel == "Link")
                enName = "BP.CCBill.Template.MethodLink";

            if (en.MethodModel == "FlowBaseData")
                enName = "BP.CCBill.Template.MethodFlowBaseData";

            if (en.MethodModel == "FlowNewEntity")
                enName = "BP.CCBill.Template.FlowNewEntity";

            if (en.MethodModel == "FlowEtc")
                enName = "BP.CCBill.Template.FlowEtc";


            var url = "../../Comm/En.htm?EnName=" + enName + "&MyPK=" + data + "&From=Ver2021";
            OpenLayuiDialog(url, "", 100000, false);

        },
        EditSort: function (no, name) {
            var url = "../Comm/EnOnly.htm?EnName=BP.WF.Template.FlowSort&No=" + no;
            this.openLayer(url, "目录:" + name);
        },
        testFlow: function (no, name) {
            var url = "../Admin/TestingContainer/TestFlow2020.htm?FK_Flow=" + no;
            window.top.vm.fullScreenOpen(url, name);
            // this.openLayer(url, name);
        },
        flowAttr: function (no, name) {
            var url = "../Comm/En.htm?EnName=BP.WF.Template.FlowExt&No=" + no;
            window.top.vm.openTab(name, url);
            //this.openLayer(url, name,900);
        },

        copyFlow: function (no) {
            if (window.confirm("确定要执行方法复制吗?") == false)
                return;
            var flow = new Entity("BP.WF.Flow", no);
            var data = flow.DoMethodReturnString("DoCopy");
            layer.msg(data);
            setTimeout(function () {
                window.location.reload();
            }, 2000);
        },
        DeleteFlow: function (no, pidx, idx) {
            var msg = "提示: 确定要删除该方法吗?";
            msg += "\t\n1.如果该方法下有实例，您不能删除。";
            msg += "\t\n2.该方法为子方法的时候，被引用也不能删除.";
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
                    this.MethodAttr(key, data, name, pidx, idx);
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
            if (val == null || val == undefined) return;

            var en = new Entity("BP.CCBill.Template.GroupMethod", id);
            en.Name = val;
            en.Update();

            //修改名称.
            this.flowNodes[idx].Name = val;

            //Todo:wanglu , 修改名称.
            $("#" + id).val(val);
            // var ctl = $("#" + id);

            layer.msg("目录修改成功.");

        },
        NewFlow: function (data, name) {

            url = "./Method/Func.htm?GroupID=" + data + "&FrmID=" + GetQueryString("FrmID") + "&s=" + Math.random();

            //新建方法.
            OpenLayuiDialog(url, '', 9000, false, true, true, false, true);

            // addTab("NewFlow", "新建方法", url);
            //   pop
        },
        EditFlowName(id, name, pidx, idx) {

            //Todo: wanglu , 这里没有获取到名字，name的参数，也没有更新到数据。 
            var val = prompt("新名称:", name);

            if (val == null || val == undefined)
                return;

            var en = new Entity("BP.CCBill.Template.Method", id);
            en.Name = val;
            en.Update();

            this.flowNodes[pidx].children[idx].Name = val;

            //Todo:wanglu , 修改名称.
            // $("#" + id).val(val);
            // var ctl = $("#" + id);
            layer.msg("修改成功.");

        },
        ImpFlowTemplate: function (data) {
            var fk_flow = data;
            url = "./../Admin/AttrFlow/Imp.htm?FK_Flow=" + fk_flow + "&Lang=CH";
            addTab("ImpFlowTemplate", "导入方法模版", url);
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
                title: '新建' + (sameLevel ? '同级' : '子级') + '方法类别',
            }, function (value, index, elem) {
                layer.close(index);
                var en = new Entity("BP.WF.Template.FlowSort", currentElem);
                var data = "";
                if (sameLevel) {
                    data = en.DoMethodReturnString("DoCreateSameLevelNodeMy", value);
                } else {
                    data = en.DoMethodReturnString("DoCreateSubNodeMy", value);
                }

                layer.msg("创建成功" + data);
                //this.EditSort(data, "编辑");
                //return;

                setTimeout(function () {
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
            // 方法排序..
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_Portal");
            handler.AddPara("SortNo", sortNo);
            handler.AddPara("EnNos", flowNos);
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
                            var pastNodeId = evt.from.dataset.pid
                            var currentNodeArrStr = Array.from(evt.to.querySelectorAll('div[data-id]')).map(function (item) {
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
        },
        // 是否启用
        changeMethodEnableStatus(method, ctrl) {
            // 当前启用状态

            // console.log(method.IsEnable);
            //console.log(method.MyPK);

            // alert(ctrl);

            var en = new Entity("BP.CCBill.Template.Method", method.MyPK);
            if (en.IsEnable == 0)
                en.IsEnable = 1; // method.IsEnable;
            else
                en.IsEnable = 0; // method.IsEnable;

            en.Update();

            console.log("更新成功..");

            // 先调接口，然后更新前端的值
            // var data = xxx
            // method.IsEnable = data
        }
    },
    mounted: function () {
        // fix firefox bug
        document.body.ondrop = function (event) {
            event.preventDefault();
            event.stopPropagation();
        }

        //获得数据源.
        var handler = new HttpHandler("BP.CCBill.WF_CCBill_OptOneFlow");
        handler.AddUrlData();
        var ds = handler.DoMethodReturnJSON("SingleDictGenerWorkFlows_Init");

        // console.log(ds); 
        var flows = ds["Flows"];
        var ens = ds["GenerWorkFlows"];
        console.log(ens);
        var nodes = flows;

        for (var i = 0; i < nodes.length; i++) {
            var fs = nodes[i];
            fs.open = false;
            fs.children = [];
            fs.Icon = "icon-folder";

            for (var j = 0; j < ens.length; j++) {

                var en = ens[j];

                if (fs.No !== en.FK_Flow) continue;

                if (en.WFState <= 1) continue; //草稿与空白的

                //退回的.
                if (en.WFState === 2) { en.Icon = "icon-clock"; en.Icontitle = "运行中"; }//运行中的.
                if (en.WFState === 3) {
                    en.Icon = "icon-check"; en.Icontitle = "已完成";
            }//已完成的.
                if (en.WFState === 5) {
                    en.Icon = "icon-action-undo"; en.Icontitle = "退回";
        } //退回的.
                var dateArray = en.TodoEmps.split(",");
                en.TodoEmps = dateArray[1];
                fs.children.push(en);
            }
        }

        this.flowNodes = nodes;
        console.log(this.flowNodes);
        this.bindMenu();
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

function addTab(no, name, url) {
    window.top.vm.openTab(name, url);
}

/**
 * 分组-右键操作方法  &&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
 * */

//新建:分组.
function NewGroup() {
    var val = window.prompt("请输入分组名", "基本信息");
    if (!val)
        return null;

    var en = new Entity("BP.CCBill.Template.GroupMethod");

    en.Name = val;
    en.FrmID = frmID;
    en.Insert();
    return val; // 更新单元格label,不要用户刷新了.
}

//修改：分组名.
function EditGroupName(groupID) {

    var en = new Entity("BP.CCBill.Template.GroupMethod", groupID);
    var val = window.prompt("请输入分组名", en.Name);
    if (!val)
        return null;

    en.Name = val;
    en.Update();
    return val; // 更新单元格label,不要用户刷新了.
}

/**
 * 方法的-右键操作方法  &&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
 * */

//新建:方法.
function NewMethod(groupID) {

    var url = "./Method/Func.htm?FrmID=" + frmID + "&GroupID=" + groupID;
    window.open(url);
}

//修改：方法名.
function EditMethodName(methodID) {

    var en = new Entity("BP.CCBill.Template.Method", groupID);
    var val = window.prompt("请输入方法名", en.Name);
    if (!val)
        return null;
    en.Name = val;
    en.Update();
    return val; // 更新单元格 label,不要用户刷新了.
}

/**
 *  移动方法   $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
 * @param {any} groupIDs
 */

/**
 * 移动分组
 * @param {分组的IDs, 用逗号分开的.} groupIDs
 */
function MoverGroup(groupIDs) {
    // 目录排序..
    var handler = new HttpHandler("BP.CCBill.WF_CCBill_Admin");
    handler.AddPara("GroupIDs", groupIDs);
    var data = handler.DoMethodReturnString("Method_MoverGroup");
    layer.msg(data)
}

/**
 * 移动方法
 * @param {移动到的分组ID} groupID
 * @param {改组内的方法s, 用逗号分开的.} methodIDs
 */
function MoverMethod(groupID, methodIDs) {
    // 目录排序..
    var handler = new HttpHandler("BP.CCBill.WF_CCBill_Admin");
    handler.AddPara("GroupID", groupID);
    handler.AddPara("MethodIDs", methodIDs);
    var data = handler.DoMethodReturnString("Method_MoverMethod");
    layer.msg(data);

}
