
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
            })
        },
        DeleteEn: function (no) {
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
            //var en = new Entity("BP.CCBill.Template.Frm.MapAttr", mapAttr.No);
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
        var en = new Entity("BP.CCBill.Template.Method", GetQueryString("No"));
        if (mapAttrs.length > 0)
            en.IsHavePara = 1;
        else
            en.IsHavePara = 0;
        en.Update();
       

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
    var sid = GetQueryString("Token");
    var webUser = new WebUser();
    var url = "../Admin/CCBPMDesigner/Designer.htm?FK_Flow=" + no + "&UserNo=" + webUser.No + "&Token=" + sid + "&OrgNo=" + webUser.OrgNo + "&From=Ver2021";
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
        url = '../../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrString&PKVal=' + mypk + '&s=' + Math.random();
    }

    if (ftype == 2 || ftype == 3 || ftype == 5 || ftype == 8) {
        title = '字段Num属性';
        url = '../../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrNum&PKVal=' + mypk + '&s=' + Math.random();
    }

    if (ftype == 6 || ftype == 7) {
        title = '字段 date 属性';
        url = '../../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrDT&PKVal=' + mypk + '&s=' + Math.random();
    }

    if (ftype == 6 || ftype == 7) {
        title = '字段 datetime 属性';
        url = '../../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrDT&PKVal=' + mypk + '&s=' + Math.random();
    }

    if (ftype == 4) {
        title = '字段 boolen 属性';
        url = '../../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrBoolen&PKVal=' + mypk + '&s=' + Math.random();
    }

    /*OpenLayuiDialog(url, "eudlgframe", title, 800, 500, "icon-edit", true, null, null, null, function () {
        Reload();
    });*/

    OpenLayuiDialog(url, title, 730, 80, "auto");
    // OpenEasyUiDialog(url, "dd", title, 730, 500);
    return;
}

function EditEnum(fk_mapdata, mypk, keyOfEn) {

    var url = '../../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrEnum&PKVal=' + mypk + '&s=' + Math.random();

    /*OpenLayuiDialog(url, "eudlgframe", '枚举' + keyOfEn + '属性', 730, 500, "icon-property", true, null, null, null, function () {
        Reload();
    });*/
    OpenLayuiDialog(url, '外键字段:' + keyOfEn + '属性', 730, 80, "auto");
}

function EditTableSQL(mypk, keyOfEn) {

    var url = '../../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrSFSQL&PKVal=' + mypk + '&s=' + Math.random();

    /* OpenLayuiDialog(url, "eudlgframe", '外键SQL字段:' + keyOfEn + '属性', 730, 500, "icon-property", true, null, null, null, function () {
         Reload();
     });*/
    OpenLayuiDialog(url, '外键字段:' + keyOfEn + '属性', 730, 80, "auto");
}

function EditTable(fk_mapData, mypk, keyOfEn) {

    var url = '../../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrSFTable&PKVal=' + mypk + '&s=' + Math.random();

    /* OpenLayuiDialog(url, "eudlgframe", '外键字段:' + keyOfEn + '属性', 730, 500, "icon-property", true, null, null, null, function () {
         Reload();
     });*/
    OpenLayuiDialog(url, '外键字段:' + keyOfEn + '属性', 730, 80, "auto");
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
    var en = new Entity("BP.Sys.MapAttr");
    en.No = no;
    en.Delete();
    Reload();
}
function TomapAttrDoc() {
    SetHref("Default.htm?No=" + GetQueryString("No"));
}
 
