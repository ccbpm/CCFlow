
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
                    { title: '<i class=icon-close></i> 删除目录', id: "DeleteSort" }
                ]
                var tRenderOptions = [{
                    elem: '.item-top-dp',
                    trigger: 'contextmenu',
                    data: topNodeItems,
                    click: function (data, oThis) {

                        var obj = $(this.elem)[0].dataset;
                        console.log(obj);

                        //    var obj = $(this.elem)[0].dataset;
                        //    debugger;

                        _this.topNodeOption(data.id, obj.no, obj.name, obj.idx);
                    }
                }, {
                    elem: '.t-btn',
                    trigger: 'click',
                    data: topNodeItems,
                    click: function (data, oThis) {

                        var obj = $(this.elem)[0].dataset;
                        console.log(obj);
                        //  debugger;

                        _this.topNodeOption(data.id, obj.no, obj.name, obj.idx);
                    }
                }]

                dropdown.render(tRenderOptions[0]);
                dropdown.render(tRenderOptions[1]);

                var childNodeMenuItems = [
                    { title: '<i class=icon-star></i> 方法属性', id: "Attr" },
                    { title: '<i class=icon-plus ></i> 新建方法', id: "NewmapAttr" },
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
                        debugger;
                        //_this.childNodeOption(data.id, $(this.elem)[0].dataset.No, $(this.elem)[0].dataset.name, $(this.elem)[0].dataset.pidx, $(this.elem)[0].dataset.idx)

                        // var obj = $(this.elem)[0].dataset;
                        // console.log(obj);

                        var obj = $(this.elem)[0].dataset;
                        console.log(obj);


                        _this.childNodeOption(data.id, $(this.elem)[0].dataset.no, $(this.elem)[0].dataset.name, $(this.elem)[0].dataset.pidx, $(this.elem)[0].dataset.idx)

                    }
                }, {
                    elem: '.c-btn',
                    trigger: 'click',
                    data: childNodeMenuItems,
                    click: function (data, othis) {

                        debugger;

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
        /* openLayer: function (uri, name, w, h) {
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
 
         mapAttrAttr: function (no) {
 
             console.log(en);
 
             var en = new Entity("BP.CCBill.Template.mapAttr", no);
 
             var enName = "BP.CCBill.Template.mapAttr";
 
 
             if (en.mapAttrModel == "Func") enName = "BP.CCBill.Template.mapAttrFunc";
             if (en.mapAttrModel == "Link") enName = "BP.CCBill.Template.mapAttrLink";
             if (en.mapAttrModel == "QRCode") enName = "BP.CCBill.Template.mapAttrQRCode";
             if (en.mapAttrModel == "FlowBaseData") enName = "BP.CCBill.Template.mapAttrFlowBaseData";
             if (en.mapAttrModel == "FlowNewEntity") enName = "BP.CCBill.Template.FlowNewEntity";
 
             if (en.mapAttrModel === "SingleDictGenerWorkFlows" || en.mapAttrModel === "SingleDictGenerWorkFlow")
                 enName = "BP.CCBill.Template.mapAttrSingleDictGenerWorkFlow";
 
             if (en.mapAttrModel == "FlowEtc")
                 enName = "BP.CCBill.Template.FlowEtc";
 
             var url = "../../Comm/En.htm?EnName=" + enName + "&MyPK=" + en.No + "&From=Ver2021";
             OpenLayuiDialog(url, "", 100000, 0, null, false);
 
         },
         EditSort: function (no, name) {
             var url = "../../Comm/En.htm?EnName=BP.CCBill.Template.GroupmapAttr&No=" + no;
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
 
         CopymapAttr: function (no) {
             if (window.confirm("确定要执行方法复制吗?") == false)
                 return;
             var flow = new Entity("BP.WF.Flow", no);
             var data = flow.DoMethodReturnString("DoCopy");
             layer.msg(data);
             setTimeout(function () {
                 window.location.reload();
             }, 2000);
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
             var flow = new Entity("BP.CCBill.Template.mapAttr", no);
             flow.Delete();
 
             // var data = flow.DoMethodReturnString("DoDelete");
             // layer.msg(data);
             //  if (data.indexOf("err@") == 0)
             //   return;
 
             layer.close(load)
 
             this.mapAttrs[pidx].children.splice(idx, 1)
             var leaveItems = this.mapAttrs[pidx].children
             this.$set(this.mapAttrs[pidx], 'children', leaveItems)
         },
 
         childNodeOption: function (key, mapAttrNo, name, pidx, idx) {
 
             // key=菜单标记, data 行的主键, name = 行的名称, pIdx=父级的编号, idx=当前的idx.
             switch (key) {
                 case "Attr": //方法的属性.
                     this.mapAttrAttr(mapAttrNo);
                     break;
                 case "Designer":
                     this.Designer(mapAttrNo, name);
                     break;
                 case "NewmapAttr":
 
                     var enmapAttr = new Entity("BP.CCBill.Template.mapAttr", mapAttrNo);
                     //  NewFlow(dat)
                     this.NewmapAttrByGroup(enmapAttr.GroupID);
                     break;
                 case "Copy":
                     this.CopymapAttr(mapAttrNo);
                     break;
                 case "EditmapAttrName":
                     this.EditmapAttrName(mapAttrNo, name, pidx, idx);
                     break;
                 case "Delete":
                     this.DeleteMethon(mapAttrNo, pidx, idx);
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
                 case "NewmapAttrByGroup": //新建方法.
                     this.NewmapAttrByGroup(groupID, name);
                     break;
                 default:
                     alert("没有判断的命令" + key);
                     break;
             }
         },
         EditSortName(id, name, idx) {
 
             var val = prompt("新名称", name);
             if (val == null || val == undefined) return;
 
             var en = new Entity("BP.CCBill.Template.GroupmapAttr", id);
             en.Name = val;
             en.Update();
 
             //修改名称.
             this.mapAttrs[idx].Name = val;
             layer.msg("目录修改成功.");
 
         },
         NewmapAttrByGroup: function (groupID, name) {
 
             var moduleNo = GetQueryString("ModuleNo");
             var frmID = GetQueryString("FrmID");
 
             url = "./Method/Func.htm?GroupID=" + groupID + "&FrmID=" + frmID + "&ModuleNo=" + moduleNo + "&s=" + Math.random();
 
             //新建方法.
             OpenLayuiDialog(url, '', 9000, 0, null, true);
         },
         EditmapAttrName(id, name, pidx, idx) {
 
 
             var en = new Entity("BP.CCBill.Template.mapAttr", id);
 
             //Todo: wanglu , 这里没有获取到名字，name的参数，也没有更新到数据。
             var val = prompt("新名称:", en.Name);
 
             if (val == null || val == undefined)
                 return;
 
             en.Name = val;
             en.Update();
             // console.log(this.mapAttrs[pidx].children[idx])
             this.mapAttrs[pidx].children[idx].Name = val;
             // this.$set(this.mapAttrs[pidx].children[idx],'Name',val)
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
         
 
         NewSort: function (currentElem, sameLevel) {
 
             //只能创建同级.
             sameLevel = true;
 
             //例子2
             layer.prompt({
                 value: '',
                 title: '新建目录',
             }, function (value, index, elem) {
                 layer.close(index);
 
                 var en = new Entity("BP.CCBill.Template.GroupmapAttr");
                 en.FrmID = GetQueryString("FrmID");
                 en.mapAttrType = "Self";
                 en.mapAttrID = "Self";
                 en.Icon = "icon-folder";
                 en.Name = value;
                 en.Insert();
 
                 layer.msg("创建成功");
                 //this.EditSort(data, "编辑");
                 //return;
 
                 setTimeout(function () {
                     window.location.reload();
                 }, 2000);
             });
         },
         updateSort(rootNo, sortNos) {
 
             console.log("sortNo:" + sortNos);
 
             // 目录排序..
             var handler = new HttpHandler("BP.CCBill.WF_CCBill_Admin");
             handler.AddPara("FrmID", GetQueryString("FrmID"));
             handler.AddPara("GroupIDs", sortNos);
             var data = handler.DoMethodReturnString("mapAttr_MoverGroup");
             layer.msg(data)
         },
         MoveItem(pastNodeArrStr, pastNodeId, currentNodeArrStr, currentNodeId) {
             // todo 需要重新实现接口
 
             if (currentNodeId == undefined) {
                 alert("没有获得当前分组的ID");
                 return;
             }
 
             // debugger;
             // 方法排序..
             var handler = new HttpHandler("BP.CCBill.WF_CCBill_Admin");
             handler.AddPara("GroupID", currentNodeId);
             handler.AddPara("mapAttrIDs", currentNodeArrStr);
             var data = handler.DoMethodReturnString("mapAttr_MovermapAttr");
             layer.msg(data)
         },
        */
        DeleteSort: function (no) {

            if (window.confirm("确定要删除吗?") == false)
                return;
            var en = new Entity("BP.Sys.MapAttr", no);
            var data = en.Delete();
            layer.msg(data);

            //如果有错误.
            if (data.indexOf("err@") == 0)
                return;

            setTimeout(function () {
                window.location.reload()
            }, 2000)
        },
        MoveItem(pastNodeArrStr, pastNodeId, currentNodeArrStr, currentNodeId) {
            // todo 需要重新实现接口

            var frmID = GetQueryString("No");

            // debugger;
            // 方法排序..
            var handler = new HttpHandler("BP.CCBill.WF_CCBill_Admin_MethodDoc");
            handler.AddPara("FrmID", frmID);
            handler.AddPara("MyPKs", currentNodeArrStr);
            var data = handler.DoMethodReturnString("MethodParas_Mover");

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
        // fix firefox bug
        document.body.ondrop = function (event) {
            event.preventDefault();
            event.stopPropagation();
        }

        //获得数据源.
        /*var handler = new HttpHandler("BP.CCBill.WF_CCBill_Admin");
        handler.AddUrlData();
        var ds = handler.DomapAttrReturnJSON("mapAttr_Init");
        var groups = ds["Groups"];
        var mapAttrs = ds["mapAttrs"];*/

        var frmID = GetQueryString("FrmID");
        var no = GetQueryString("No"); //方法的编号.
        var mapAttrs = new Entities("BP.Sys.MapAttrs");
        mapAttrs.Retrieve("FK_MapData", no, "Idx");
        mapAttrs = obj2arr(mapAttrs);
        var btnStyle = "class='layui-btn layui-btn-primary layui-border-blue layui-btn-xs'";
        mapAttrs.forEach(function (mapAttr) {

            if (mapAttr.UIContralType == 0) {
                //控件数据类型
                if (mapAttr.MyDataType == "1") {
                    mapAttr.UIContralType = "文本框- String";
                } else if (mapAttr.MyDataType == "2") {
                    mapAttr.UIContralType = "文本框 Int";
                } else if (mapAttr.MyDataType == "3") {
                    mapAttr.UIContralType = "文本框 Float";
                } else if (mapAttr.MyDataType == "4") {
                    mapAttr.UIContralType = "文本框 Boolean";
                } else if (mapAttr.MyDataType == "5") {
                    mapAttr.UIContralType = "文本框 Double";
                } else if (mapAttr.MyDataType == "6") {
                    mapAttr.UIContralType = "文本框 Date";
                } else if (mapAttr.MyDataType == "7") {
                    mapAttr.UIContralType = "文本框 DateTime";
                } else if (mapAttr.MyDataType == "8") {
                    mapAttr.UIContralType = "文本框 Money";
                }
            } else if (mapAttr.UIContralType == 1) {
                //枚举下拉框
                if (mapAttr.LGType == 1) {
                    mapAttr.UIContralType = "下拉框-枚举";
                } //外键下拉框
                else if (mapAttr.LGType == 2) {
                    mapAttr.UIContralType = "下拉框-外键";
                }
                //外部数据源
                else if (mapAttr.LGType == 0) {
                    mapAttr.UIContralType = "下拉框-外键";
                }
            } else if (mapAttr.UIContralType == 2) { //复选框.
                mapAttr.UIContralType = "复选框";
            } else if (mapAttr.UIContralType == 3) { //单选妞
                return;
            } else if (mapAttr.UIContralType == 8) { //签字版
                mapAttr.UIContralType = "Hand Siganture";
            }
            if (mapAttr.LGType == 0 && mapAttr.UIContralType == 1) {
                //mapAttr.Name = "<a href=\"javascript:EditTableSQL('" + mapAttr.No + "','" + mapAttr.KeyOfEn + "');\" > " + mapAttr.Name + "</a>";
                mapAttr.Name = "<a " + btnStyle + " href=\"javascript:addTab('../../../CCBill/SearchEditer.htm?FrmID=" + mapAttr.FK_MapData + "','" + mapAttr.Name + "');\" >列表</a>";
            }

            if (mapAttr.LGType == 0) {
                mapAttr.Name = "<a " + btnStyle + " href=\"javascript:Edit('" + mapAttr.MyPK + "','" + mapAttr.MyDataType + "','" + mapAttr.GroupID + "','" + mapAttr.LGType + "');\" > " + mapAttr.Name + "</a>";
            }

            if (mapAttr.LGType == 1)
                mapAttr.Name = "<a " + btnStyle + " href=\"javascript:EditEnum('" + mapAttr.FK_MapData + "','" + mapAttr.MyPK + "','" + mapAttr.KeyOfEn + "');\" > " + mapAttr.Name + "</a>";

            if (mapAttr.LGType == 2)
                mapAttr.Name = "<a " + btnStyle + " href=\"javascript:EditTable('" + mapAttr.FK_MapData + "','" + mapAttr.MyPK + "','" + mapAttr.KeyOfEn + "');\" > " + mapAttr.Name + "</a>";
        })

        this.mapAttrs = mapAttrs;

        console.log(this.mapAttrs);
        this.initSortArea();
        // this.bindMenu();

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
 
