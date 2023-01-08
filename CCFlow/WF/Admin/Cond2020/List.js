
new Vue({
    el: '#condParas',
    data: {
        conds: [],
        expandAll: false,
        loadingDialog: false,
        Event: '',
        systemNodeItems: []
    },
    watch: {
        expandAll(val) {
            this.expandMenus(val);
        }
    },
    methods: {
        expandMenus: function (status) {
            for (var i = 0; i < this.conds.length; i++) {
                this.conds[i].open = status
            }
        },
        bindMenu: function () {
            var _this = this;
            layui.use('dropdown', function () {
                var dropdown = layui.dropdown
                _this.systemNodeItems = [
                    { title: '按表单条件计算', id: "CondByFrm" },
                    { title: '按已选择的独立表单条件计算', id: "StandAloneFrm" },
                    { title: '按指定操作员的岗位条件', id: "CondStation" },
                    { title: '按指定操作员的部门条件', id: "CondDept" },
                    { title: '按SQL条件计算', id: "CondBySQL" },
                    { title: '按SQL模版条件计算', id: "CondBySQLTemplate" },
                    { title: '按开发者参数计算', id: "CondByPara" },
                    { title: '按Url条件计算', id: "CondByUrl" },
                    { title: '按WebApi返回值', id: "CondByWebApi" },
                    { title: '按审核组件的立场计算', id: "CondByWorkCheck" },
                    

                ]

                var systemFunc = function (data, oThis) {
                    var url = GetHrefUrl();
                    url = url.replace('List.htm', data.id + '.htm');

                    SetHref(url);

                    // window.parent.Condlist.openTab(data.title, url);
                    // OpenLayuiDialog(url, "", 0, 0, null, true);

                }
                var systemOptions = [{
                    elem: '.item-top-dp',
                    trigger: 'contextmenu',
                    data: _this.systemNodeItems,
                    click: systemFunc
                }, {
                    elem: '.NewPara-btn',
                    trigger: 'click',
                    data: _this.systemNodeItems,
                    click: systemFunc
                }]

                dropdown.render(systemOptions[0]);
                dropdown.render(systemOptions[1]);

                var menuFunc = function (data, oThis) {

                    //var idx = $(this.elem)[0].dataset.idx;
                    //  var condIdx = idx - 1;
                    var en = new Entity("BP.WF.Template.Cond");
                    //生成一个随机数添加到运算符的主键中
                    var radomNum = parseInt(Math.random() * 1000) + 1;
                    en.SetPKVal(flowNo + "_" + nodeID + "_" + radomNum);
                    en.CondType = condType; //条件类型.
                    en.DataFrom = 100;  //运算符.
                    en.FK_Flow = flowNo;
                    en.FK_Node = nodeID;

                    en.FK_Operator = data.condExp; //都赋值，以免用错.
                    en.OperatorValue = data.condExp; //都赋值，以免用错.

                    en.ToNodeID = toNodeID;
                    en.Idx = 100;
                    en.Insert();

                    layer.msg('添加成功', { time: 2000 }, function () {
                        Reload();
                    });
                }
                var menuNodeItems = [
                    { title: '( 左括号', condExp: '(' },
                    { title: ') 右括号', condExp: ')' },
                    { title: 'AND 并且', condExp: 'AND' },
                    { title: 'OR 或者', condExp: 'OR' }
                ]
                var menuOptions = [{
                    elem: '.item-menu-dp',
                    trigger: 'contextmenu',
                    data: menuNodeItems,
                    click: menuFunc
                }, {
                    elem: '.condExp-btn',
                    trigger: 'click',
                    data: menuNodeItems,
                    click: menuFunc
                }]

                dropdown.render(menuOptions[0]);
                dropdown.render(menuOptions[1]);
            })
        },
        DeleteSort: function (no) {
            if (window.confirm("确定要删除吗?") == false)
                return;
            var en = new Entity("BP.WF.Template.Cond", no);
            var data = en.Delete();
            layer.msg(data);

            //如果有错误.
            if (data.indexOf("err@") == 0)
                return;

            setTimeout(function () {
                window.location.reload();
            }, 2000)
        },
        EditCond: function (item) {
            var url = GetHrefUrl();

            url = url.replace('List.htm', this.systemNodeItems[item.DataFrom].id + '.htm') + "&MyPK=" + item.MyPK;

            SetHref(url);
        },
        MoveItem(pastNodeArrStr, pastNodeId, currentNodeArrStr, currentNodeId) {
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_Cond2020");
            handler.AddPara("MyPKs", currentNodeArrStr);
            var data = handler.DoMethodReturnString("List_Move");
            this.CondCheck();
        },
        CondCheck: function () {
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_Cond2020");
            handler.AddPara("ToNodeID", GetQueryString("ToNodeID"));
            handler.AddPara("FK_Node", GetQueryString("FK_Node"));
            handler.AddPara("CondType", GetQueryString("CondType"));

            var data = handler.DoMethodReturnString("List_DoCheck");
            //  alert(data);
            $("#msg").html(data);
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
        changecondEnableStatus(cond, ctrl) {

            // 当前启用状态
            //var en = new Entity("BP.CCBill.Template.cond", cond.No);
            //if (en.IsEnable == 0)
            //    en.IsEnable = 1; // cond.IsEnable;
            //else
            //    en.IsEnable = 0; // cond.IsEnable;
            //en.Update();
            console.log("更新成功..");
        }
    },
    mounted: function () {
        document.body.ondrop = function (event) {
            event.preventDefault();
            event.stopPropagation();
        }

        var dir = null;
        var conds = new Entities("BP.WF.Template.Conds");

        //  alert(condType);
        conds.Retrieve("FK_Node", nodeID, "ToNodeID", toNodeID, "CondType", condType, "Idx");


        //输出条件类型.
        var Eventtitle = '';
        if (condType == 0)
            Eventtitle = '节点完成条件';

        if (condType == 2) {
            var toNode = new Entity("BP.WF.Node", toNodeID);
            Eventtitle = '到达节点[<font color=green>' + toNode.NodeID + " " + toNode.Name + '</font>]的方向条件';
        }

        if (condType == 1)
            Eventtitle = '流程完成条件';
        if (condType == 3)
            Eventtitle = '子流程启动条件';
        this.Event = Eventtitle;
        console.log(this.Event);

        conds = obj2arr(conds);
        console.log(conds)
        conds.forEach(function (cond) {

            //控件数据类型
            if (cond.DataFrom == "0") {
                cond.DataFromText = "表单字段";
            } else if (cond.DataFrom == "1") {
                cond.DataFromText = "独立表单";
            } else if (cond.DataFrom == "2") {
                cond.DataFromText = "按岗位";
            } else if (cond.DataFrom == "3") {
                cond.DataFromText = "按部门";
            } else if (cond.DataFrom == "4") {
                cond.DataFromText = "按SQL";
            } else if (cond.DataFrom == "5") {
                cond.DataFromText = "按SQL模板";
            } else if (cond.DataFrom == "6") {
                cond.DataFromText = "按参数";
            } else if (cond.DataFrom == "7") {
                cond.DataFromText = "按URL";
            } else if (cond.DataFrom == "8") {
                cond.DataFromText = "按WebApi返回值";
            } else if (cond.DataFrom == "9") {
                cond.DataFromText = "按审核组件立场";
            } else if (cond.DataFrom == "100") {
                cond.DataFromText = "运算符";
            }

            if (cond.DataFrom == 100) {
                cond.Express = cond.OperatorValue;
            } else {
                if (cond.OperatorValueT != "")
                    cond.Express = cond.AttrKey + cond.FK_Operator + cond.OperatorValueT;
                else
                    cond.Express = cond.AttrKey + cond.FK_Operator + cond.OperatorValue;

                if (cond.AttrKey != "")
                    cond.Express = "说明：" + cond.AttrName + " " + cond.FK_Operator + " " + " " + cond.OperatorValue;
            }
        })

        this.conds = conds;

        console.log(this.conds);
        this.CondCheck();
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
    var sid = GetQueryString("SID");
    var webUser = new WebUser();
    var url = "../Admin/CCBPMDesigner/Designer.htm?FK_Flow=" + no + "&UserNo=" + webUser.No + "&SID=" + sid + "&OrgNo=" + webUser.OrgNo + "&From=Ver2021";
    window.top.vm.openTab(name, url);
}

function addTab(no, name, url) {
    window.top.vm.openTab(name, url);
}


function NewPara() {
    var url = "../../../Admin/FoolFormDesigner/FieldTypeList.htm?DoType=AddF&FK_MapData=" + GetQueryString("No");
    /* OpenLayuiDialog(url, "eudlgframe", "新建参数", 800, 500, "icon-edit", true, null, null, null, function () {
         Reload();
     });*/
    OpenLayuiDialog(url, '新建参数', 900, 80, "auto", true);
}
//删除.
function Delete(no) {
    if (window.confirm('您确定要删除吗？') == false)
        return;
    var en = new Entity("BP.Sys.Cond");
    en.No = no;
    en.Delete();
    Reload();
}
function TocondDoc() {
    SetHref("Default.htm?No=" + GetQueryString("No"));
}

