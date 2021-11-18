
new Vue({
    el: '#mapAttrParas',
    data: {
        mapAttrs: [],
        expandAll: false,
        loadingDialog: false,
        Event: ''
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
                var systemNodeItems = [
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
                    var url = window.location.href;
                    url = url.replace('List.htm', data.id + '.htm');

                    window.location.href = url;

                    // window.parent.Condlist.openTab(data.title, url);
                    // OpenLayuiDialog(url, "", 0, 0, null, true);

                }
                var systemOptions = [{
                    elem: '.item-top-dp',
                    trigger: 'contextmenu',
                    data: systemNodeItems,
                    click: systemFunc
                }, {
                    elem: '.NewPara-btn',
                    trigger: 'click',
                    data: systemNodeItems,
                    click: systemFunc
                }]

                dropdown.render(systemOptions[0]);
                dropdown.render(systemOptions[1]);


                var menuFunc = function (data, oThis) {

                    var idx = $(this.elem)[0].dataset.idx;
                    var condIdx = idx - 1;
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
                    en.Idx = condIdx;
                    en.Insert();
                    layer.msg('添加成功', { time: 2000 }, function () {
                        window.location.href = window.location.href;
                    });
                }
                var menuNodeItems = [
                    { title: '( 左括号', condExp: '（' },
                    { title: ') 右括号', condExp: '）' },
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
        MoveItem(pastNodeArrStr, pastNodeId, currentNodeArrStr, currentNodeId) {
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_Cond2020");
            handler.AddPara("MyPKs", currentNodeArrStr);
            var data = handler.DoMethodReturnString("List_Move");
            //  layer.msg(data);
            //设置tonodeID.
            // handler.AddPara("ToNodeID", GetQueryString("ToNodeID"));

            handler.AddPara("ToNodeID", GetQueryString("ToNodeID"));
            handler.AddPara("FK_Node", GetQueryString("FK_Node"));
            handler.AddPara("CondType", GetQueryString("CondType"));

            var data = handler.DoMethodReturnString("List_DoCheck");
            //  alert(data);
            $("#msg").html(data);
            //  var data = handler.DoMethodReturnString("List_Move");
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
        changemapAttrEnableStatus(mapAttr, ctrl) {

            // 当前启用状态
            //var en = new Entity("BP.CCBill.Template.mapAttr", mapAttr.No);
            //if (en.IsEnable == 0)
            //    en.IsEnable = 1; // mapAttr.IsEnable;
            //else
            //    en.IsEnable = 0; // mapAttr.IsEnable;
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

        mapAttrs = obj2arr(conds);
        console.log(mapAttrs)
        mapAttrs.forEach(function (mapAttr) {

            //控件数据类型
            if (mapAttr.DataFrom == "0") {
                mapAttr.UIContralType = "表单字段";
            } else if (mapAttr.DataFrom == "1") {
                mapAttr.UIContralType = "独立表单";
            } else if (mapAttr.DataFrom == "2") {
                mapAttr.UIContralType = "按岗位";
            } else if (mapAttr.DataFrom == "3") {
                mapAttr.UIContralType = "按部门";
            } else if (mapAttr.DataFrom == "4") {
                mapAttr.UIContralType = "按SQL";
            } else if (mapAttr.DataFrom == "5") {
                mapAttr.UIContralType = "按SQL模板";
            } else if (mapAttr.DataFrom == "6") {
                mapAttr.UIContralType = "按参数";
            } else if (mapAttr.DataFrom == "7") {
                mapAttr.UIContralType = "按URL";
            } else if (mapAttr.DataFrom == "8") {
                mapAttr.UIContralType = "按WebApi返回值";
            } else if (mapAttr.DataFrom == "9") {
                mapAttr.UIContralType = "按审核组件立场";
            } else if (mapAttr.DataFrom == "100") {
                mapAttr.UIContralType = "运算符";
            }
            if (mapAttr.DataFrom == 100) {
                mapAttr.Express = mapAttr.OperatorValue;
            } else {
                mapAttr.Express = mapAttr.AttrKey + mapAttr.FK_Operator + mapAttr.OperatorValue;

                if (mapAttr.AttrKey != "")
                    mapAttr.Express = "说明：" + mapAttr.AttrName + " " + mapAttr.FK_Operator + " " + "" + mapAttr.OperatorValue;
            }

        })

        this.mapAttrs = mapAttrs;

        console.log(this.mapAttrs);
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



function Edit(mypk, ftype, gf, fk_mapdtl) {

    var url = 'EditF.htm?DoType=Edit&No=' + mypk + '&FType=' + ftype + '&FK_MapData=' + GetQueryString("FrmID") + '&GroupField=' + gf;
    var title = '';
    if (ftype == 1) {
        title = '字段String属性';
        url = '../../../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrString&PKVal=' + mypk + '&s=' + Math.random();
    }

    if (ftype == 2 || ftype == 3 || ftype == 5 || ftype == 8) {
        title = '字段Num属性';
        url = '../../../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrNum&PKVal=' + mypk + '&s=' + Math.random();
    }

    if (ftype == 6 || ftype == 7) {
        title = '字段 date 属性';
        url = '../../../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrDT&PKVal=' + mypk + '&s=' + Math.random();
    }

    if (ftype == 6 || ftype == 7) {
        title = '字段 datetime 属性';
        url = '../../../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrDT&PKVal=' + mypk + '&s=' + Math.random();
    }

    if (ftype == 4) {
        title = '字段 boolen 属性';
        url = '../../../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrBoolen&PKVal=' + mypk + '&s=' + Math.random();
    }

    /*OpenLayuiDialog(url, "eudlgframe", title, 800, 500, "icon-edit", true, null, null, null, function () {
        window.location.href = window.location.href;
    });*/

    OpenLayuiDialog(url, title, 730, 80, "auto");
    // OpenEasyUiDialog(url, "dd", title, 730, 500);
    return;
}

function EditEnum(fk_mapdata, mypk, keyOfEn) {

    var url = '../../../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrEnum&PKVal=' + mypk + '&s=' + Math.random();

    /*OpenLayuiDialog(url, "eudlgframe", '枚举' + keyOfEn + '属性', 730, 500, "icon-property", true, null, null, null, function () {
        window.location.href = window.location.href;
    });*/
    OpenLayuiDialog(url, '外键字段:' + keyOfEn + '属性', 730, 80, "auto");
}

function EditTableSQL(mypk, keyOfEn) {

    var url = '../../../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrSFSQL&PKVal=' + mypk + '&s=' + Math.random();

    /* OpenLayuiDialog(url, "eudlgframe", '外键SQL字段:' + keyOfEn + '属性', 730, 500, "icon-property", true, null, null, null, function () {
         window.location.href = window.location.href;
     });*/
    OpenLayuiDialog(url, '外键字段:' + keyOfEn + '属性', 730, 80, "auto");
}

function EditTable(fk_mapData, mypk, keyOfEn) {

    var url = '../../../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrSFTable&PKVal=' + mypk + '&s=' + Math.random();

    /* OpenLayuiDialog(url, "eudlgframe", '外键字段:' + keyOfEn + '属性', 730, 500, "icon-property", true, null, null, null, function () {
         window.location.href = window.location.href;
     });*/
    OpenLayuiDialog(url, '外键字段:' + keyOfEn + '属性', 730, 80, "auto");
}

function NewPara() {
    var url = "../../../Admin/FoolFormDesigner/FieldTypeList.htm?DoType=AddF&FK_MapData=" + GetQueryString("No");
    /* OpenLayuiDialog(url, "eudlgframe", "新建参数", 800, 500, "icon-edit", true, null, null, null, function () {
         window.location.href = window.location.href;
     });*/
    OpenLayuiDialog(url, '新建参数', 900, 80, "auto", true);
}
//删除.
function Delete(no) {
    if (window.confirm('您确定要删除吗？') == false)
        return;
    var en = new Entity("BP.Sys.MapAttr");
    en.No = no;
    en.Delete();
    window.location.href = window.location.href;
}
function TomapAttrDoc() {
    window.location.href = "Default.htm?No=" + GetQueryString("No");
}

