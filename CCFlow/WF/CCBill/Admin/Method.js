
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
                    { title: '<i class=icon-plus></i> 新建组件(方法)', id: "NewMethodByGroup" },
                    { title: '<i class=icon-star></i> 目录属性', id: "EditSort" },
                    { title: '<i class=icon-folder></i> 新建目录', id: "NewSort" },
                    { title: '<i class=icon-pencil></i> 修改名称', id: "EditSortName" },
                    { title: '<i class=icon-close></i> 删除目录', id: "DeleteSort" }
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
                  //  { title: '<i class=icon-plus ></i> 新建方法', id: "NewMethod" },
                    /*    { title: '<i class=icon-settings></i> 设计方法', id: "Designer" },*/
                    /*    { title: '<i class=icon-docs></i> 复制方法', id: "Copy" },*/
                    { title: '<i class=icon-pencil></i> 修改名称', id: "EditMethodName" },
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

        MethodAttr: function (no) {

            console.log(en);

            var en = new Entity("BP.CCBill.Template.Method", no);

            var enName = "BP.CCBill.Template.Method";

            if (en.MethodModel == "Func") enName = "BP.CCBill.Template.MethodFunc";
            if (en.MethodModel == "Link") enName = "BP.CCBill.Template.MethodLink";
            if (en.MethodModel == "QRCode") enName = "BP.CCBill.Template.MethodQRCode";
            if (en.MethodModel == "FlowBaseData") enName = "BP.CCBill.Template.MethodFlowBaseData";
            if (en.MethodModel == "FlowNewEntity") enName = "BP.CCBill.Template.FlowNewEntity";

            if (en.MethodModel === "SingleDictGenerWorkFlows" || en.MethodModel === "SingleDictGenerWorkFlow")
                enName = "BP.CCBill.Template.MethodSingleDictGenerWorkFlow";

            if (en.MethodModel == "FlowEtc")
                enName = "BP.CCBill.Template.MethodFlowEtc";

            var url = "../../Comm/En.htm?EnName=" + enName + "&MyPK=" + en.No + "&From=Ver2021";
            OpenLayuiDialog(url, "", 100000, 0, null, false);

        },
        EditSort: function (no, name) {
            var url = "../../Comm/En.htm?EnName=BP.CCBill.Template.GroupMethod&No=" + no;
            //  alert(url);
            OpenLayuiDialog(url, "", 0, 0, null, true);
            //this.openLayer(url, "目录:" + name);
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

        CopyMethod: function (no) {
            if (window.confirm("确定要执行方法复制吗?") == false)
                return;
            var flow = new Entity("BP.WF.Flow", no);
            var data = flow.DoMethodReturnString("DoCopy");
            layer.msg(data);
            setTimeout(function () {
                window.location.reload();
            }, 800);
        },
        DeleteMethon: function (no, pidx, idx) {

            var msg = "提示: 确定要删除【" + no + "】方法吗?";
            msg += "\t\n1.如果该方法下有实例，您不能删除。";
            msg += "\t\n2.该方法为子方法的时候，被引用也不能删除.";
            if (window.confirm(msg) == false)
                return;

            var load = layer.msg("正在处理,请稍候...", {
                icon: 16,
                anim: 5
            })

            //开始执行删除.
            var flow = new Entity("BP.CCBill.Template.Method", no);
            flow.Delete();

            // var data = flow.DoMethodReturnString("DoDelete");
            // layer.msg(data);
            //  if (data.indexOf("err@") == 0)
            //   return;

            layer.close(load)

            this.flowNodes[pidx].children.splice(idx, 1)
            var leaveItems = this.flowNodes[pidx].children
            this.$set(this.flowNodes[pidx], 'children', leaveItems)
        },

        childNodeOption: function (key, methodNo, name, pidx, idx) {

            // key=菜单标记, data 行的主键, name = 行的名称, pIdx=父级的编号, idx=当前的idx.
            switch (key) {
                case "Attr": //方法的属性.
                    this.MethodAttr(methodNo);
                    break;
                case "Designer":
                    this.Designer(methodNo, name);
                    break;
                case "NewMethod":

                    var enMethod = new Entity("BP.CCBill.Template.Method", methodNo);
                    //  NewFlow(dat)
                    this.NewMethodByGroup(enMethod.GroupID);
                    break;
                case "Copy":
                    this.CopyMethod(methodNo);
                    break;
                case "EditMethodName":
                    this.EditMethodName(methodNo, name, pidx, idx);
                    break;
                case "Delete":
                    this.DeleteMethon(methodNo, pidx, idx);
                    break;
            }
        },  //分组上的事件.
        topNodeOption: function (key, groupID, name, idx) {

            switch (key) {
                case "EditSort":
                    this.EditSort(groupID, name);
                    break;
                case "EditSortName":
                    this.EditSortName(groupID, name, idx); //修改名字.
                    break;
                case "ImpFlowTemplate":
                    this.ImpFlowTemplate(groupID);
                    break;
                case "NewSort":
                    this.NewSort(groupID, true);
                    break;
                case "DeleteSort":
                    this.DeleteSort(groupID);
                    break;
                case "NewMethodByGroup": //新建方法.
                    this.NewMethodByGroup(groupID, name);
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
            layer.msg("目录修改成功.");

        },
        NewMethodByGroup: function (groupID, name) {

            var moduleNo = GetQueryString("ModuleNo");
            var frmID = GetQueryString("FrmID");

            url = "./Method/Func.htm?GroupID=" + groupID + "&FrmID=" + frmID + "&ModuleNo=" + moduleNo + "&s=" + Math.random();

            //新建方法.
            OpenLayuiDialog(url, '', 9000, 0, null, true);
        },
        EditMethodName(id, name, pidx, idx) {


            var en = new Entity("BP.CCBill.Template.Method", id);

            //Todo: wanglu , 这里没有获取到名字，name的参数，也没有更新到数据。
            var val = prompt("新名称:", en.Name);

            if (val == null || val == undefined)
                return;

            en.Name = val;
            en.Update();
            // console.log(this.flowNodes[pidx].children[idx])
            this.flowNodes[pidx].children[idx].Name = val;
            // this.$set(this.flowNodes[pidx].children[idx],'Name',val)
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

            var en = new Entity("BP.CCBill.Template.GroupMethod", no);
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
                title: '新建目录',
            }, function (value, index, elem) {
                layer.close(index);

                var en = new Entity("BP.CCBill.Template.GroupMethod");
                en.FrmID = GetQueryString("FrmID");
                en.MethodType = "Self";
                en.MethodID = "Self";
                en.Icon = "icon-folder";
                en.Name = value;
                en.Insert();

                layer.msg("创建成功");
                //this.EditSort(data, "编辑");
                //return;

                setTimeout(function () {
                    window.location.reload();
                }, 800);
            });
        },
        updateSort(rootNo, sortNos) {

            console.log("sortNo:" + sortNos);

            // 目录排序..
            var handler = new HttpHandler("BP.CCBill.WF_CCBill_Admin");
            handler.AddPara("FrmID", GetQueryString("FrmID"));
            handler.AddPara("GroupIDs", sortNos);
            var data = handler.DoMethodReturnString("Method_MoverGroup");
            layer.msg(data)
        },
        updateFlow(pastNodeArrStr, pastNodeId, currentNodeArrStr, currentNodeId) {
            // todo 需要重新实现接口

            if (currentNodeId == undefined) {
                alert("没有获得当前分组的ID");
                return;
            }
            // 方法排序..
            var handler = new HttpHandler("BP.CCBill.WF_CCBill_Admin");
            handler.AddPara("GroupID", currentNodeId);
            handler.AddPara("MethodIDs", currentNodeArrStr);
            var data = handler.DoMethodReturnString("Method_MoverMethod");
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
                        _this.loadingDialog = layer.msg('正在移动目录...', {
                            timeout: 900 * 1000
                        })
                    },
                    onEnd: function (evt) {
                        layer.close(_this.loadingDialog);

                        var arr = this.toArray();

                        //  console.log(arr);

                        // 一级菜单的排序，默认为1
                        _this.updateSort('1', arr.join(','));
                    }
                });
                var childSortableContainers = this.$refs['child-row']
                console.log(childSortableContainers)
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

                        }
                    })
                }

            })
        },
        // 是否启用
        changeMethodEnableStatus(method, ctrl) {
            // 当前启用状态

            var en = new Entity("BP.CCBill.Template.Method", method.No);
            if (en.IsEnable == 0)
                en.IsEnable = 1; // method.IsEnable;
            else
                en.IsEnable = 0; // method.IsEnable;

            en.Update();

            console.log("更新成功..");

        },
        // 是否启用
        changeMethodListEnableStatus(method, ctrl) {
            // 当前启用状态

            var en = new Entity("BP.CCBill.Template.Method", method.No);
            if (en.IsList == 0)
                en.IsList = 1; // method.IsEnable;
            else
                en.IsList = 0; // method.IsEnable;

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

        //获得数据源.
        var handler = new HttpHandler("BP.CCBill.WF_CCBill_Admin");
        handler.AddUrlData();
        var ds = handler.DoMethodReturnJSON("Method_Init");

        var groups = ds["Groups"];
        var methods = ds["Methods"];
        console.log(groups);
        var nodes = groups;

        var btnStyle = "class='layui-btn layui-btn-primary layui-border-blue layui-btn-xs'";

        for (var i = 0; i < nodes.length; i++) {
            var fs = nodes[i];
            fs.open = false;
            fs.children = [];

            if (fs.Icon == "")
                fs.Icon = "icon-folder";

            for (var j = 0; j < methods.length; j++) {

                var method = methods[j];
                if (fs.No !== method.GroupID) continue;

                //生成方法内容.
                var doc = "";

                if (method.MethodModel == MethodModel.Link) doc = method.Tag1;
                if (method.MethodModel == MethodModel.QRCode) doc = "手机扫描一下，就可以看到该表单信息.";
                if (method.MethodModel === MethodModel.DictLog) doc = "操作轨迹日志";
                if (method.MethodModel === MethodModel.FrmBBS) doc = "日志-日常记录-评论";
                if (method.MethodModel === MethodModel.DataVer) doc = "数据版本控制-数据快照";

                if (method.MethodModel == MethodModel.Func) {

                    doc = "<a " + btnStyle + "  href=\"javascript:AttrFrm('BP.CCBill.Template.MethodFunc','" + method.Name + "','" + method.No + "')\" >功能属性</a>";
                    var url = "./MethodDoc/Default.htm?No=" + method.No;
                    doc += "<a " + btnStyle + "  href=\"javascript:OpenLayuiDialog('" + url + "','',9000,0,null,false)\" >编写脚本</a>";
                }


                //修改实体资料流程.
                if (method.MethodModel == MethodModel.FlowBaseData) {

                    method.MethodModel = "流程:" + method.FlowNo;
                    doc = "<a  " + btnStyle + "  href=\"javascript:DesignerFlow('" + method.MethodID + "','" + method.Name + "');\" ><i class=icon-heart ></i>设计流程 </a> ";

                    //if (method.Mark === "StartFlow") {
                    //    method.MethodModel = "发起流程:" + method.FlowNo;
                    //    doc = "<a  " + btnStyle + "  href=\"javascript:DesignerFlow('" + method.MethodID + "','" + method.Name + "');\" ><i class=icon-heart ></i>设计流程 </a> ";
                    //}

                    //if (method.Mark === "Search") {
                    //    method.MethodModel = "流程数据查询:" + method.FlowNo;
                    //    doc = "<a  " + btnStyle + "  href=\"javascript:DesignerFlow('" + method.MethodID + "','" + method.Name + "');\" ><i class=icon-heart ></i>设计流程 </a> ";
                    //    //  doc = "<a href='' ><i class=icon-grid >  设置查询内容</a>";
                    //    //doc += "<a class='layui-btn layui-btn-primary layui-border-blue layui-btn-xs' href=\"alert();\" >方法</a>";
                    //}
                    //if (method.Mark === "Group") {
                    //    method.MethodModel = "流程数据分析:" + + method.FlowNo;
                    //    //  doc = "<a href='' ><i class=icon-chart >设置分析内容</a>";
                    //}
                }

                // 流程汇总
                if (method.MethodModel == MethodModel.SingleDictGenerWorkFlows) {
                    method.MethodModel = "流程汇总";
                    if (method.Icon == "") method.Icon = "icon-layers";
                    doc = "所有启动的流程进行列表汇总.";
                }

                //实体新建流程.
                if (method.MethodModel == MethodModel.FlowNewEntity) {
                    method.MethodModel = "新建实体流程";
                    doc = "<a href=\"javascript:DesignerFlow('" + method.MethodID + "','" + method.Name + "');\" ><i class=icon-heart ></i>设计流程 </a> ";
                }

                //实体其他业务流程.
                if (method.MethodModel == MethodModel.FlowEtc) {
                    method.MethodModel = "其他业务流程:" + method.FlowNo;
                    doc = "<a  " + btnStyle + " href=\"javascript:DesignerFlow('" + method.MethodID + "','" + method.Name + "');\" ><i class=icon-heart ></i>设计流程 </a> ";
                }

                //如果是单据.
                if (method.MethodModel == MethodModel.Bill) {
                    method.MethodModel = "单据";
                    var html = "";

                    // if (method.Mark === "BillDictSearch") {
                    html += "<a " + btnStyle + "  href=\"javascript:AttrFrm('BP.CCBill.FrmBill','" + method.Name + "','" + method.Tag1 + "')\" >单据属性</a>";
                    html += "<a " + btnStyle + "  href=\"javascript:GoToFrmDesigner('" + method.Tag1 + "')\" >表单设计</a>";
                    //}

                    doc = html;
                }

                //方法内容.
                method.Docs = doc;
                var url = "./PowerCenter.htm?CtrlObj=Menu&CtrlPKVal=" + method.No + "&CtrlGroup=Menu";

                method.methodCtrlWayText = "<a " + btnStyle + "  href =\"javascript:OpenLayuiDialog('" + url + "','" + method.Name + "','700',0,null,false);\" >权限</a>";
                

                if (method.Icon == "") method.Icon = "icon-drop";

                fs.children.push(method);
            }
            var fsurl = "./PowerCenter.htm?CtrlObj=Menu&CtrlPKVal=" + fs.No + "&CtrlGroup=Menu";

            fs.groupCtrlWayText = "<a " + btnStyle + "  href =\"javascript:OpenLayuiDialog('" + fsurl + "','" + fs.Name + "','700',0,null,false);\" >权限</a>";

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
    var url = basePath +"/WF/Admin/CCBPMDesigner/Designer.htm?FK_Flow=" + no + "&UserNo=" + webUser.No + "&Token=" + sid + "&OrgNo=" + webUser.OrgNo + "&From=Ver2021";
    window.top.vm.openTab(name, url);
}

function addTab(no, name, url) {
    window.top.vm.openTab(name, url);
}



/**
 * 分组-右键操作方法  &&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
 * */

//新建:分组.
function NewGroup() {
    var val = promptGener("请输入分组名", "基本信息");
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
    var val = promptGener("请输入分组名", en.Name);
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
    var val = promptGener("请输入方法名", en.Name);
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
