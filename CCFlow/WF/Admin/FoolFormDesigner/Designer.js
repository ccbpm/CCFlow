//页面获取的参数数据


// fix firefox bug
document.body.ondrop = function (event) {
    event.preventDefault();
    event.stopPropagation();
}


var pageData = {
    WorkID: 0,
    OID: 0,
    FK_Flow: GetQueryString("FK_Flow"),
    FK_Node: GetQueryString("FK_Node")
}
var frmID = GetQueryString("FK_MapData");
var groupID = GetQueryString("GroupField");
var webUser = new WebUser();
var workModel = GetQueryString("WorkModel");
workModel = workModel == null || workModel == undefined ? 0 : workModel;

var nodeID = GetQueryString("FK_Node");
nodeID = nodeID == null || nodeID == undefined ? 0 : nodeID;

var W = 0;
var H = 0;
layui.extend({
    pinyin: "../../Scripts/layui/ext/pinyin"
});
layui.use(['dropdown', 'util', 'layer', 'table', 'form', 'pinyin', 'laydate'], function () {
    var dropdown = layui.dropdown,
        form = layui.form,
        layer = layui.layer,
        laydate = layui.laydate,
        pinyin = layui.pinyin,
        $ = layui.jquery;


    //1.初始化二级菜单
    dropdown.render({
        elem: '#Btn_Save',
        data: CovertEleMenuToDropdownMenu(),
        click: function (obj) {
            MenuClick(obj.id, pinyin);
        },
        mousemove: function (obj) {
            MenuClick(obj.id, pinyin);
        }
    });

    //初演示
    dropdown.render({
        elem: '#Btn_TableCol',
        data: [
            { title: '标准型4列', id: 0 },
            { title: '紧凑型6列', id: 1 }
        ],
        click: function (obj) {
            var en = new Entity("BP.WF.Template.Frm.MapFrmFool", frmID);
            en.TableCol = obj.id;
            en.Update();
            Reload();
        }
    });

    //2.设置显示或隐藏工具栏按钮
    hideBtton();

    //装载风格.
    LoadCss();

    //3.获取表单数据、解析表单的数据内容
    InitPage();

    //4.处理工具栏的操作
    $('.layui-toolbar').on('click', function () {
        getWindowWH();
        var url = "";
        var title = "";
        var type = $(this).data('type');
        switch (type) {
            case "MapDataEdit": //属性

                var en = new Entity("BP.Sys.MapData", frmID);
                var url = "../../Comm/RefFunc/En.htm?EnName=BP.WF.Template.Frm.MapFrmFool&PKVal=" + frmID;
                if (en.FrmType == 9)
                    url = "../../Comm/RefFunc/En.htm?EnName=BP.WF.Template.Frm.MapFrmWps&No=" + frmID;

                //url = '../../Comm/En.htm?EnName=BP.WF.Template.Frm.MapFrmFool&PKVal=' + frmID;
                //alert(url);

                title = "属性";
                break;
            case "FrmBillEdit": //单据属性
                title = "单据属性";
                if (workModel == 2)
                    url = '../../Comm/En.htm?EnName=BP.CCBill.FrmBill&PKVal=' + frmID;
                if (workModel == 3)
                    url = '../../Comm/En.htm?EnName=BP.CCBill.FrmDict&PKVal=' + frmID;
                break;
            case "ImpRefDict": //导入关联表单模板
                title = "导入关联表单模板";
                var bill = new Entity("BP.CCBill.FrmBill", frmID);
                url = "./ImpRefDictField.htm?FK_MapData=" + frmID + "&RefFrm=" + bill.RefDict;
                break;
            case "ExpImp": //导入/导出
                title = "导入/导出";
                url = './ImpExp/Imp/Default.htm?FrmID=' + frmID + "&DoType=FunList&FK_Flow=" + GetQueryString("FK_Flow") + "&FK_Node=" + nodeID;
                break;
            case "Batch": //导入/导出
                title = "批处理";
                url = './Batch/Default.htm?FrmID=' + frmID + "&DoType=FunList&FK_Flow=" + GetQueryString("FK_Flow") + "&FK_Node=" + nodeID;
                break;
            case "FrmNodeComponent": //节点组件
                if (nodeID == 0) {
                    layer.alert('非节点表单');
                    return;
                }
                title = "节点组件";
                url = '../../Comm/EnOnly.htm?EnName=BP.WF.Template.FrmNodeComponent&PKVal=' + nodeID;
                break;
            case "MobileFrm":
                title = "手机端";
                url = '../MobileFrmDesigner/Default.htm?FK_Flow=' + GetQueryString("FK_Flow") + '&FK_Node=' + nodeID + '&FK_MapData=' + frmID;
                OpenLayuiDialog(url, title, W, 0, null, false);
                return;
            case "StyletDfine":
                url = "./StyletDfine/Default.htm?FK_Flow=" + GetQueryString("FK_Flow") + "&FK_Node=" + nodeID + "&FK_MapData=" + frmID + "&FrmID=" + frmID;
                // window.open(url);
                SetHref(url);
                return;
            case "ToDevFrm":
                url = "../DevelopDesigner/Designer.htm?FK_Flow=" + GetQueryString("FK_Flow") + "&FK_Node=" + nodeID + "&FK_MapData=" + frmID + "&FrmID=" + frmID;
                //  window.open(url);
                SetHref(url);
                return;
            default:
                break;
        }

        if (type != "StyletDfine" && type != "ToDevFrm")
            OpenLayuiDialog(url, title, W, 0, null, true);

    });



    form.render(); //更新全部

    //时间
    $.each($(".ccdate"), function (i, item) {
        var format = $(item).attr("data-info");
        if (format.indexOf("HH") != -1) {
            laydate.render({
                elem: '#' + item.id,
                format: $(item).attr("data-info"), //可任意组合
                type: 'datetime',
                trigger: 'click',
                ready: function (date) {
                    var now = new Date();
                    var mm = "";
                    if (now.getMinutes() < 10)
                        mm = "0" + now.getMinutes();
                    else
                        mm = now.getMinutes();

                    var ss = "";
                    if (now.getSeconds() < 10)
                        ss = "0" + now.getSeconds();
                    else
                        ss = now.getSeconds();

                    this.dateTime.hours = now.getHours();
                    this.dateTime.minutes = mm;
                    this.dateTime.seconds = ss;
                },
                change: function (value, date, endDate) {
                    $('.laydate-btns-confirm').click();
                }
            });
        } else {
            laydate.render({
                elem: '#' + item.id,
                format: $(item).attr("data-info") //可任意组合
            });
        }

    })

});

//动态的装载css.
function LoadCss() {
    // 动态加载css
    var loadStyle = function (url) {
        var link = document.createElement('link');
        link.rel = "stylesheet";
        link.type = "text/css";
        link.href = url;
        var head = document.getElementsByTagName("head")[0];
        head.appendChild(link);
    };

    // css加载
    var url = "../../../DataUser/Style/FoolFrmStyle/Default.css?ref=11" + Math.random();
    loadStyle(url);
    url = "../../../DataUser/Style/GloVarsCSS.css?ref=11" + Math.random();
    loadStyle(url);

    $('head').children(':last').attr({
        rel: "stylesheet",
        type: 'text/css',
        href: url,
    });
}

/*
 *获取屏幕的高度、宽度
 */
function getWindowWH() {
    //获取页面的大小
    if (window.innerWidth)
        W = window.innerWidth;
    else if ((document.body) && (document.body.clientWidth))
        W = document.body.clientWidth;
    W = W - 400;
    if (window.innerHeight)
        H = window.innerHeight;
    else if ((document.body) && (document.body.clientHeight))
        H = document.body.clientHeight;
    H = H - 80;
}

/**
 * 隐藏工具栏上的按钮
 */
function hideBtton() {
    //导入关联表单按钮
    $("#RefDict").hide();

    //单据属性按钮
    if (workModel == 0 || workModel == 1)
        $("#FrmillBtn").hide();
    if (workModel == 3)
        $("#RefDict").show();

    //如果SAAS模式.
    if (webUser.CCBPMRunModel == 2) {
        $("#StyletDfine").hide();
        $("#GloValStyles").hide();
        $("#BatchAddF").hide();
        $("#MobileFrm").hide();
    }

    if (nodeID == 0)
        $("#FrmNodeComponent").hide();
}

/**
 * 表单数据初始化
 */
function InitPage() {

    var isF = GetQueryString("IsFirst"); //是否第一次加载?
    var hander = new HttpHandler("BP.WF.HttpHandler.WF_Admin_FoolFormDesigner");
    hander.Clear();
    hander.AddPara("IsFirst", isF);
    hander.AddPara("FK_MapData", frmID);
    hander.AddPara("FK_Flow", GetQueryString("FK_Flow"));
    hander.AddPara("FK_Node", GetQueryString("FK_Node"));
    var data = hander.DoMethodReturnString("Designer_Init");

    if (data.indexOf('err@') == 0) {
        alert(data);
        console.log(data);
        return;
    }

    if (data.indexOf('url@') == 0) {
        data = data.replace('url@', '');
        SetHref(data);
        return;
    }

    //里面有三个对象. Sys_MapAttr, Sys_GroupField, Sys_MapData
    data = JSON.parse(data);
    var groupFields = data.Sys_GroupField; //分组集合
    var sys_MapData = data.Sys_MapData[0]; //表单属性
    var frmName = sys_MapData.Name; //表单名称
    var tableCol = sys_MapData.TableCol; //表单分的列数
    if (tableCol == 0)
        $("#TB_TableCol").html("标准型4列");
    else
        $("#TB_TableCol").html("紧凑型6列");
    switch (tableCol) {
        case 0:
            tableCol = 4;
            break;
        case 1:
            tableCol = 6;
            break;
        case 2:
            tableCol = 3;
            break;
        default:
            ableCol = 4;
            break;
    }

    var _html = '<form class="layui-form " lay-filter="designer" >';
    _html += "<div class='layui-row wapper'>";
    //表头
    _html += '<div class="layui-col-xs12 FoolFrmTitle">';
    _html += '<div class="layui-col-xs12">'
    //_html += "   <div class='FoolFrmTitleIcon' style='float:left;margin-top:1px'  > <img src='../../../DataUser/ICON/LogBiger.png' style='height:50px;' /></div >";
    _html += "   <div class='FoolFrmTitleLable' style='float:right;margin-top:8px' >" + frmName + "</div>";
    _html += '</div>';
    _html += '</div>';


    //分组的解析
    for (var k = 0; k < groupFields.length; k++) {

        var groupObj = groupFields[k];
        //附件类的控件.
        if (groupObj.CtrlType == 'Ath') {
            //获取附件的主键
            var MyPK = groupObj.CtrlID;
            if (MyPK == "")
                continue;

            //创建附件描述信息.
            var ath = new Entity("BP.Sys.FrmAttachment");
            ath.MyPK = groupObj.CtrlID;
            if (ath.RetrieveFromDBSources() == 0)
                continue;
            if (ath.IsVisable == "0" || ath.NoOfObj == "FrmWorkCheck")
                continue;
        }
        //生成工具栏.
        _html += GenerGroupTR(groupObj, tableCol, data);

        //生成内容.
        _html += GenerGroupContext(groupObj, data, tableCol);

        //过滤attrs
        var mapAttrs = $.grep(data.Sys_MapAttr, function (val) { return val.GroupID == groupObj.OID; });
        if (tableCol == 4 || tableCol == 6)
            _html += InitMapAttr(mapAttrs, tableCol,data.Sys_MapExt);

        if (tableCol == 3)
            _html += InitThreeColMapAttr(mapAttrs, tableCol);
        continue;
    }
    _html += "</div>";
    _html += "</form>";
    $('#contentTable').html(_html);

    if ($("#WorkCheck").length == 1)
        loadScript("../../WorkOpt/WorkCheck.js", function () {
            NodeWorkCheck_Init();
        });

    if ($("#FlowBBS").length == 1)
        loadScript("../../WorkOpt/FlowBBS.js");

    if (data.Sys_FrmAttachment.length != 0) {
        Skip.addJs("../../CCForm/Ath.js");
        Skip.addJs("../../CCForm/JS/FileUpload/fileUpload.js");
        $('head').append("<link href='../../CCForm/JS/FileUpload/css/fileUpload.css' rel='stylesheet' type='text/css' />");
    }

    $.each(data.Sys_FrmAttachment, function (idex, ath) {
        AthTable_Init(ath, "Div_" + ath.MyPK);
    });

    //AfterBindEn_DealMapExt(data);

    //增加图标
    $.each(data.Sys_MapAttr, function (i, mapAttr) {
        if (mapAttr.UIVisible == 0)
            return true;
        AddICON(mapAttr);
    });
    $.each($(".wapper"), function (i, item) {
        if (item == null)
            return true;

        var ops = {
            animation: 1000,
            draggable: ".item",
            ghostClass: 'form-item-drag-class',
            //开始拖动记录下用户点击的那个元素
            onStart: function (evt) {
                moveItemId = evt.clone.dataset.id;
                layer.msg("正在移动", {
                    offset: 'center',
                    icon: 16
                })
            },
            //拖动结束
            onEnd: function (evt) {
                /**
                 * 获取排序后的结果
                 * 因为之前页面结构不正确，获取不方便。
                 * 但可以通过获取dom元素的位置解决
                 * 首先拿到当前触发的元素evt.item
                 * $(evt.item).prevAll()拿到所有前驱节点
                 * 从前驱节点里面找到分组标识
                 */
                var prevNodeList = Array.from($(evt.item).prevAll())
                var currentGroup = prevNodeList.filter(function (item) {
                    return item.classList.contains('FoolFrmGroupBar')
                })


                if (Array.isArray(currentGroup) && currentGroup.length > 0) {
                    currentGroup = currentGroup[0]
                } else {
                    layer.msg("获取的groupId为空")
                    return
                }


                // 此时再去根据拖动后的组寻找后面所有的节点，筛选
                var nextNodeList = Array.from($(currentGroup).nextAll())
                var requiredList = []
                nextNodeList.forEach(function (item, index) {
                    // 到下一个组，终止循环
                    if (item.classList.contains('FoolFrmGroupBar')) {
                        return
                    }
                    if (item.classList.contains('item')) {
                        requiredList.push(item)
                    }
                })
                var currentGroupId = currentGroup.dataset.id
                // 当前分组节点排序
                var currentMyPKs = requiredList.map(function (item) {
                    return item.dataset.id
                }).join(',')
                var currentItemId = evt.item.dataset.id
                // 分别为当前移动的元素的id，移动到的组的id，移动到的组的排序
                console.log('currentId:', currentItemId, 'currentGroupId:', currentGroupId, 'currentMyPKs:', currentMyPKs);

                var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_FoolFormDesigner");
                handler.AddPara("GroupID", currentGroupId); //当前的组ID。
                handler.AddPara("MyPKs", currentMyPKs); //分组中的IDs
                handler.AddPara("MyPK", currentItemId); // 当前选择的IDs.

                //handler.AddPara("GroupID", groupID);
                var data = handler.DoMethodReturnString("Designer_Move");

                //todo:这里增加气泡模式的信息提示.
                layer.msg(data);
                if (data.indexOf("err@") == 0)
                    Reload();
                return

            },
        };
        var sortable = Sortable.create(item, ops);

    })
}

//生成鼠标移动上去的修改样式.
function AddEditColTip(mapAttr) {
    if (mapAttr.ColSpan == 1) {
        return " ";
    }
}

//保存设置的宽度
function SaveColSpan(mapAttr, colspan) {

    mapAttr.ColSpan = colspan;
    mapAttr.Update();
    Reload();
}

function AddICON(mapAttr) {

    var icon = mapAttr.ICON;
    if (mapAttr.MyDataType == 6 || mapAttr.MyDataType == 7)
        icon = "icon-calendar";
    if (icon == "" || icon == "0") {
        return;
    }
    if (icon) {
        var html = "";
        html += ' <i class=" input-icon ' + icon + '"></i>';
        var obj = $("#TB_" + mapAttr.KeyOfEn);
        if (obj.length == 0)
            obj = $("#DDL_" + mapAttr.KeyOfEn);
        obj.wrapAll("<div style='display:inline'></div>");
        obj.css("padding-left", "40px");
        obj.before(html);
    }
}
/**
 * 分组标签的解析
 * @param {any} groupObj 分组
 * @param {any} tableCol 显示的列
 * @param {any} data 表单数据集合
 */
function GenerGroupTR(groupEn, tabCol, data) {
    var lab = groupEn.Lab;
    if (lab == "")
        lab = "编辑";

    //左边的按钮.
    var leftBtn = "<div style='text-align:left; float:left'>";

    var midBtn = "";
    var isShow = true;
    var tdId = "";
    switch (groupEn.CtrlType) {
        case "Ath":
            if (groupEn.CtrlID == "") {
                isShow = false;
                break;
            }
            //创建附件描述信息.
            var frmattachments = data.Sys_FrmAttachment;
            var ath = null;
            for (var i = 0; i < frmattachments.length; i++) {
                if (frmattachments[i].MyPK == groupEn.CtrlID) {
                    ath = frmattachments[i];
                    break;
                }

            }
            if (ath == null) {
                isShow = false;
                break;
            }
            if (ath.IsVisable == "0") {
                //如果是节点字段附件的时候就隐藏
                var mapAttr = $.grep(data.Sys_MapAttr, function (obj, i) {
                    if (obj.MyPK == groupEn.CtrlID)
                        return obj;
                });
                if (mapAttr.length != 0) {
                    isShow = false;
                    break;
                }
            }
            tdId = "id='THAth_" + ath.MyPK + "'";
            leftBtn += "<div title='编辑附件属性'  style='cursor: pointer' onclick=\"javascript:EditAth('" + groupEn.CtrlID + "')\" >附件:" + lab + "</div>";
            break;
        case "Frame":
            leftBtn += "<div title='编辑框架属性'  style='cursor: pointer' onclick=\"javascript:EditFrame('" + groupEn.CtrlID + "')\" >" + lab + "</div>";
            break;
        case "SubFlow":
            leftBtn += "<div title='编辑子流程属性'  style='cursor: pointer' onclick=\"javascript:EditSubFlow('" + groupEn.CtrlID + "')\" >" + lab + "</div>";
            break;
        case "Dtl":

            leftBtn += "<div title='编辑从表属性' style='cursor: pointer' onclick=\"javascript:EditDtl('" + groupEn.CtrlID + "')\" >从表属性:" + lab + "</div>";

            //中间连接.
            midBtn = "";
            midBtn += '<span style="cursor: pointer" onclick="javascript:AddFForDtl(\'' + groupEn.CtrlID + '\');" ><img src="../../Img/Btn/New.gif" border="0/"><font >插入列</font></span>';
            midBtn += "";
            tdId = "id='THDtl_" + groupEn.CtrlID + "'";
            break;
        case "FWC":
            leftBtn += "<div title='点击编辑属性' style='cursor: pointer' onclick=\"javascript:FrmNodeComponent()\" >审核审批</div>";
            break;
        default:
            leftBtn += "<div title='点击编辑属性' style='cursor: pointer' onclick=\"javascript:GroupField('" + groupEn.EnName + "','" + groupEn.OID + "')\" >" + lab + "</div>";
            break;
    }
    if (isShow == true) {
        leftBtn += "</div>";
        //右边的按钮都一样.
        var rightBtn = "<div class='cs-order'>" + midBtn;
        rightBtn += "<a href=\"javascript:GroupFieldDoUp('" + groupEn.OID + "' )\"><img src='../../Scripts/layui/Img/up.png' style='width:21px'/></a>";
        rightBtn += "<a href=\"javascript:GroupFieldDoDown('" + groupEn.OID + "');\"><i class='layui-icon layui-icon-triangle-d'></i></a>";
        rightBtn += "</div>";
        console.log(groupEn)
        var html = "<div class='layui-col-xs12 FoolFrmGroupBar'  data-id='" + groupEn.OID + "'>";
        html += "<div class='layui-col-xs12'>";
        html += leftBtn + rightBtn;
        html += "</div>";
        html += "</div>";
    }
    return html;
}
/**
 * 上移
 * @param {any} refOID
 */
function GroupFieldDoUp(refOID) {

    var en = new Entity("BP.Sys.GroupField", refOID);
    en.DoMethodReturnString("DoUp");
    Reload();
}
/**
 * 下移
 * @param {any} refoid
 */
function GroupFieldDoDown(refoid) {
    var en = new Entity("BP.Sys.GroupField", refoid);
    en.DoMethodReturnString("DoDown");
    Reload();
}

/**
 * 生成附件，父子流程，框架，审核组件，从表的内容
 * @param {any} groupEn
 * @param {any} data
 * @param {any} tableCol
 */
function GenerGroupContext(groupEn, data, tableCol) {
    var _html = '<div class="layui-col-xs12 FoolFrmFieldTR">';
    _html += '<div class="layui-col-xs12">'
    switch (groupEn.CtrlType) {

        case "Ath":
            if (groupEn.CtrlID == "")
                return "";
            var athEn = $.grep(data.Sys_FrmAttachment, function (ath) { return ath.MyPK == groupEn.CtrlID });

            if (athEn.length == 0)
                _html += "<div id='Ath_" + groupEn.CtrlID + "' style='width:100%;height:200px;' > 附件[" + groupEn.CtrlID + "]丢失</div>";

            if (athEn[0].IsVisable == "0")
                return "";

            _html += "<div id='Ath_" + athEn[0].MyPK + "' style='height:" + athEn[0].H + "px;'><div id='Div_" + athEn[0].MyPK + "'></div></div>";

            break;
        case "SubFlow":

            var flowNo = GetQueryString("FK_Flow");
            var nodeID = GetQueryString("FK_Node");

            Skip.addJs("../../WorkOpt/SubFlow.js");
            // _html += gfLabHtml;
            _html += "<div class='layui-row'>"
            _html += "<div class='layui-col-xs12'>";
            //说明是累加表单.
            if (groupEn.FrmID.indexOf(nodeID) == -1) {

                var myNodeID = groupEn.FrmID.substring(2);
                var myNode = new Entity("BP.WF.Node", myNodeID);
                _html += "<div id='SubFlow'>" + SubFlow_Init(myNode) + "</div>";
            }
            else {
                var node = new Entity("BP.WF.Node", nodeID);
                _html += "<div id='SubFlow'>" + SubFlow_Init(node) + "</div>";
            }

            /* var src = "../../WorkOpt/SubFlow.htm?WorkID=0&FK_Flow=" + flowNo + "&FK_Node=" + nodeID;
 
             var compent = new Entity("BP.WF.Template.SFlow.FrmSubFlow", nodeID);
 
             var frameDocs = "<iframe ID='F_SubFlow_" + nodeID + "' frameborder=0 style='padding:0px;border:0px;width:100%;height:" + compent.SF_H + "px;'  leftMargin='0'  topMargin='0' src='" + src + "'  scrolling='auto'  /></iframe>";
             _html += "<div  id='SubFlow" + nodeID + "'>" + frameDocs + "</div>";*/

            break;
        case "Frame":
            var frameEn = $.grep(data.Sys_MapFrame, function (frame) { return frame.MyPK == groupEn.CtrlID });
            if (frameEn.length == 0) {
                _html += "<div> 框架[" + groupEn.CtrlID + "]丢失</div>";
            }

            var src = frameEn[0].URL;
            if (src.indexOf('?') == -1)
                src = frameEn[0].URL + "?PKVal=0&FK_MapData=" + frmID + "&MapFrame=" + frameEn[0].MyPK;
            else
                src = frameEn[0].URL + "&PKVal=0&FK_MapData=" + frmID + "&MapFrame=" + frameEn[0].MyPK;

            var frameUrl = "<iframe ID='F" + frameEn[0].MyPK + "' frameborder=0 style='padding:0px;border:0px;width:100%;height:" + frameEn[0].H + "px;'  leftMargin='0'  topMargin='0' src='" + src + "'  scrolling='auto'  /></iframe>";
            _html += "<div  id='TD" + frameEn[0].MyPK + "' style='width:" + frameEn[0].H + "px;'>" + frameUrl + "</div>";

            break;
        case "FWC":
            _html += "<div><div id='WorkCheck'></div></div>";
            break;
        case "Dtl":

            var dtl = $.grep(data.Sys_MapDtl, function (dtl) { return dtl.No == groupEn.CtrlID });

            if (dtl.length == 1) {
                var src = "MapDtlDe2021.htm?DoType=Edit&FK_MapData=" + frmID + "&FK_MapDtl=" + dtl[0].No;
                var frameUrl = "<iframe ID='F" + dtl[0].No + "' frameborder=0 style='padding:0px;border:0px;width:100%;'  leftMargin='0'  topMargin='0' src='" + src + "'  scrolling='auto'  /></iframe>";
                _html += "<div id='Dtl_" + dtl[0].No + "'>" + frameUrl + "</div>";
            } else {
                _html += "<div  id='Dtl_" + groupEn.CtrlID + "'> 从表[" + groupEn.CtrlID + "]丢失</div>";
            }
            break;
        default:
            break;

    }
    _html += "</div>";
    _html += "</div>";
    return _html;
}

//解析表单字段 MapAttr.(表单4列/6列)
function InitMapAttr(Sys_MapAttr, tableCol,mapExts) {
    var html = "";
    var isDropTR = true;
    var colSpan = 1;
    var LabelColSpan = 1;
    var textWidth = "";
    var colWidth = "";
    var useColSpan = 0;

    for (var i = 0; i < Sys_MapAttr.length; i++) {
        var attr = Sys_MapAttr[i];
        if (attr.UIVisible == 0)
            continue;
        //单元格和标签占的列数
        colSpan = attr.ColSpan;
        LabelColSpan = attr.LabelColSpan;

        //单元格和标签占的列数对应的class
        colWidth = getColSpanClass(colSpan, tableCol);
        textWidth = getLabelColSpanClass(LabelColSpan, tableCol);

        //大文本备注信息 独占一行
        if (attr.UIContralType == 60) {
            if (isDropTR == false) {
                // html += "</div>";
                isDropTR = true;
            }
            textWidth = getLabelColSpanClass(tableCol, tableCol);
            //获取文本信息
            mapExts = mapExts || [];
            var myExts = $.grep(mapExts, function (item) {
                var mypk = "HtmlText_" + attr.MyPK;
                return item.MyPK == mypk;
            });
            var str = "设置大块文本说明";
            if (myExts.length > 0) {
                var mapExt = new Entity("BP.Sys.MapExt", myExts[0]);
                mapExt.MyPK = myExts[0].MyPK;
                str = mapExt.DoMethodReturnString("ReadBigNoteHtmlText");
            }
            html += "<div class='layui-col-xs12 item FoolFrmFieldRow' data-id='" + attr.MyPK + "' style='margin-bottom: 6px;'>";
            html += "<div  class='" + textWidth + " FoolFrmFieldLabel'><a href='#' onclick='EditBigText(\"" + attr.FK_MapData + "\",\"" + attr.KeyOfEn + "\")'>" + str + "</a></div>";
            html += "</div>";
            isDropTR = true;
            continue;
        }
        //跨列设置(显示的是文本)
        if (colSpan == 0) {

            if (LabelColSpan >= tableCol) {
                if (isDropTR == false) {
                    //html += "</div>";
                    isDropTR = true;
                }
                textWidth = getLabelColSpanClass(tableCol, tableCol);
                html += "<div class='layui-col-xs12 item FoolFrmFieldRow' data-id='" + attr.MyPK + "' style='margin-bottom: 6px;'>";
                html += "<div  class='" + textWidth + " FoolFrmFieldLabel'>" + GenerLabel(attr) + "</div>";
                html += "</div>";
                isDropTR = true;
                continue;
            }
            //线性展示都跨一个单元格
            //换行的情况
            if (isDropTR == true) {
                useColSpan = LabelColSpan;
                html += "<div class='layui-col-xs12 item FoolFrmFieldRow' data-id='" + attr.MyPK + "' style='margin-bottom: 6px;'>";
                html += "<div  class='" + textWidth + " FoolFrmFieldLabel'>" + GenerLabel(attr) + "</div>";
                if (useColSpan == tableCol) {
                    isDropTR = true;
                    //html += "</div>";
                } else
                    isDropTR = false;
                continue;
            }

            if (isDropTR == false) {
                useColSpan += LabelColSpan;
                if (useColSpan > tableCol) {
                    useColSpan = LabelColSpan;
                    //自动换行
                    //html += "</div>";
                    html += "<div class='layui-col-xs12 item FoolFrmFieldRow' data-id='" + attr.MyPK + "' style='margin-bottom: 6px;'>";
                    html += "<div  class='" + textWidth + " FoolFrmFieldLabel'>" + GenerLabel(attr) + "</div>";
                } else
                    html += "<div  class='" + textWidth + " FoolFrmFieldLabel'>" + GenerLabel(attr) + "</div>";

                if (useColSpan == tableCol) {
                    isDropTR = true;
                    //html += "</div>";
                } else
                    isDropTR = false;
                continue;
            }
        }
        //解析占一行的情况
        if (colSpan == tableCol) {
            if (isDropTR == false) {
                //html += "</div>";
                isDropTR = true;
            }
            useColSpan = 0;
            //自动换行
            html += "<div class='layui-col-xs12 item FoolFrmFieldRow' data-id='" + attr.MyPK + "' style='margin-bottom: 6px;'>";
            html += "<div class='" + colWidth + " FoolFrmFieldLabel'>" + GenerLabel(attr) + "</div>";
            html += "<div class='" + colWidth + " FoolFrmFieldInput' id='TD_" + attr.KeyOfEn + "' >" + InitMapAttrOfCtrlFool(attr) + "</div>";
            html += "</div>"
            continue;
        }
        var sumColSpan = colSpan + LabelColSpan;
        if (sumColSpan >= tableCol) {
            if (isDropTR == false) {
                //html += "</div>";
                isDropTR = true;
            }
            useColSpan = 0;
            colWidth = textWidth.replace("layui-col-xs", "");
            if (colWidth.length == 0)
                colWidth = "layui-col-xs12";
            else
                colWidth = "layui-col-xs" + (12 - parseInt(colWidth));
            html += "<div class='layui-col-xs12 item FoolFrmFieldRow' data-id='" + attr.MyPK + "' style='margin-bottom: 6px;'>";
            if (attr.MyDataType == 1 && attr.UIContralType == 0 && attr.LGType == 0 && (attr.IsSupperText == 1 || attr.UIHeight > 40))
                html += "<div  class='" + textWidth + " FoolFrmFieldLabel' style='height:" + attr.UIHeight + "px'>" + GenerLabel(attr) + "</div>";
            else
                html += "<div  class='" + textWidth + " FoolFrmFieldLabel'>" + GenerLabel(attr) + "</div>";
            html += "<div  class='" + colWidth + " FoolFrmFieldInput' id='TD_" + attr.KeyOfEn + "'>" + InitMapAttrOfCtrlFool(attr) + "</div>";
            html += "</div>";
            continue;
        }

        //换行的情况
        if (isDropTR == true) {
            useColSpan = LabelColSpan + colSpan;
            html += "<div class='layui-col-xs" + (getColSummary([textWidth, colWidth])) + " item FoolFrmFieldRow' style='margin-bottom: 6px' data-id='" + attr.MyPK + "'>";
            html += "<div  class='" + textWidth + " FoolFrmFieldLabel' style='width:" + (getColSummary([textWidth]) / getColSummary([textWidth, colWidth]) * 100 + '%') + "'>" + GenerLabel(attr) + "</div>";
            html += "<div  class='" + colWidth + " FoolFrmFieldInput' style='width:" + (getColSummary([colWidth]) / getColSummary([textWidth, colWidth]) * 100 + '%') + "' id='TD_" + attr.KeyOfEn + "'>" + InitMapAttrOfCtrlFool(attr) + "</div>";
            html += "</div>"
            if (useColSpan >= tableCol) {
                isDropTR = true;
                //html += "</div>";
            } else
                isDropTR = false;
            continue;

        }
        if (isDropTR == false) {
            useColSpan += LabelColSpan + colSpan;
            if (useColSpan > tableCol) {
                useColSpan = LabelColSpan + colSpan;
                //自动换行
                //html += "</div>";
                html += "<div class='layui-col-xs" + (getColSummary([textWidth, colWidth])) + " item FoolFrmFieldRow' style='margin-bottom: 6px' data-id='" + attr.MyPK + "'>";
                // html += "<div  class='" + textWidth + " FoolFrmFieldLabel'>" + GenerLabel(attr) + "</div>";
                // html += "<div  class='" + colWidth + " FoolFrmFieldInput' id='TD_" + attr.KeyOfEn + "'>" + InitMapAttrOfCtrlFool(attr) + "</div>";
                html += "<div  class='" + textWidth + " FoolFrmFieldLabel' style='width:" + (getColSummary([textWidth]) / getColSummary([textWidth, colWidth]) * 100 + '%') + "'>" + GenerLabel(attr) + "</div>";
                html += "<div  class='" + colWidth + " FoolFrmFieldInput' style='width:" + (getColSummary([colWidth]) / getColSummary([textWidth, colWidth]) * 100 + '%') + "' id='TD_" + attr.KeyOfEn + "'>" + InitMapAttrOfCtrlFool(attr) + "</div>";
                html += "</div>"
            } else {

                html += "<div class='layui-col-xs" + (getColSummary([textWidth, colWidth])) + " item FoolFrmFieldRow' style='margin-bottom: 6px' data-id='" + attr.MyPK + "'>";
                // html += "<div  class='" + textWidth + " FoolFrmFieldLabel'>" + GenerLabel(attr) + "</div>";
                // html += "<div  class='" + colWidth + " FoolFrmFieldInput' id='TD_" + attr.KeyOfEn + "'>" + InitMapAttrOfCtrlFool(attr) + "</div>";
                html += "<div  class='" + textWidth + " FoolFrmFieldLabel' style='width:" + (getColSummary([textWidth]) / getColSummary([textWidth, colWidth]) * 100 + '%') + "'>" + GenerLabel(attr) + "</div>";
                html += "<div  class='" + colWidth + " FoolFrmFieldInput' style='width:" + (getColSummary([colWidth]) / getColSummary([textWidth, colWidth]) * 100 + '%') + "' id='TD_" + attr.KeyOfEn + "'>" + InitMapAttrOfCtrlFool(attr) + "</div>";
                html += "</div>"
            }
            if (useColSpan == tableCol) {
                isDropTR = true;
                //html += "</div>";
            } else
                isDropTR = false;
            continue;
        }
    }
    //if (isDropTR == false)
    //    html += "</div>";

    html += "<div class='layui-col-xs12'></div>"
    return html;
}
/**
 * 获取Lab标签的显示内容
 * @param {any} attr
 */
function GenerLabel(attr) {
    var tdUp = "";// "<a href=\"javascript:Up('" + attr.MyPK + "','1');\"  alt='向左动顺序' ></a>";
    var tdDown = "";//"<a href=\"javascript:Down('" + attr.MyPK + "','1');\" alt='向右动顺序' ></a>";
    if (attr.UIIsInput == 1)
        attr.Name = attr.Name + "<font color=red>*</font>";
    var ccsLab = "";
    if (attr.CSSLabel != "")
        ccsLab = " class='" + attr.CSSLabel + "'";
    if (attr.LGType == 0 && attr.UIContralType == 1) {
        return tdUp + "<span id='Span_" + attr.KeyOfEn + "' " + ccsLab + " style='cursor: pointer' onclick=\"javascript:EditTableSQL('" + attr.MyPK + "','" + attr.KeyOfEn + "');\" >" + attr.Name + "</span>" + tdDown;
    }

    if (attr.LGType == 0) {
        return tdUp + "<span id='Span_" + attr.KeyOfEn + "' " + ccsLab + " style='cursor: pointer' onclick=\"javascript:Edit('" + attr.MyPK + "','" + attr.MyDataType + "','" + attr.GroupID + "','" + attr.LGType + "','" + attr.UIContralType + "');\" >" + attr.Name + "</span>" + tdDown;
    }

    if (attr.LGType == 1)
        return tdUp + "<span id='Span_" + attr.KeyOfEn + "' " + ccsLab + " style='cursor: pointer' onclick=\"javascript:EditEnum('" + attr.FK_MapData + "','" + attr.MyPK + "','" + attr.KeyOfEn + "');\" >" + attr.Name + "</span>" + tdDown;

    if (attr.LGType == 2)
        return tdUp + "<span id='Span_" + attr.KeyOfEn + "' " + ccsLab + " style='cursor: pointer' onclick=\"javascript:EditTable('" + attr.FK_MapData + "','" + attr.MyPK + "','" + attr.KeyOfEn + "');\" >" + attr.Name + "</span>" + tdDown;
}
/**
 * 获取单元格显示的内容
 * @param {any} mapAttr
 */
function InitMapAttrOfCtrlFool(mapAttr) {
    var ccsCtrl = "";
    if (mapAttr.CSSCtrl != "" && mapAttr.CSSCtrl != "0")
        ccsCtrl = " class='" + mapAttr.CSSCtrl + "'";
    var eleHtml = "";

    //普通字段
    if (mapAttr.LGType == 0) {
        switch (parseInt(mapAttr.MyDataType)) {
            case 1: //普通文本
                switch (parseInt(mapAttr.UIContralType)) {
                    case 4: //地图
                        eleHtml = "<div style='text-align:left;padding-left:0px' id='athModel_" + mapAttr.KeyOfEn + "' data-type='1'>";
                        eleHtml += "<input type='button' name='select' value='选择' " + ccsCtrl + " />";
                        eleHtml += "<input type=text " + ccsCtrl + " style='width:75%' maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' />";
                        eleHtml += "</div>";
                        return eleHtml;
                    case 6: //字段附件
                        return "<div " + ccsCtrl + " style='text-align:left;padding-left:10px' id='athModel_" + mapAttr.KeyOfEn + "'><label>请点击[" + mapAttr.Name + "]执行上传</label></div>";
                    case 12: //图片肖像.

                        return "<div " + ccsCtrl + " style='text-align:left;padding-left:10px' id='athModel_" + mapAttr.KeyOfEn + "'><label>肖像图片</label></div>";
                    case 8: //写字板
                        return "<img  src='../../../DataUser/Siganture/admin.jpg' " + ccsCtrl + " style='border:0px;height:" + mapAttr.UIHeight + "px;' id='Img" + mapAttr.KeyOfEn + "' />";
                    case 9: //超链接
                        return "<a " + ccsCtrl + " id='Link_" + mapAttr.KeyOfEn + "' href='" + mapAttr.Tag2 + "' target='" + mapAttr.Tag1 + "' name='Link_" + mapAttr.KeyOfEn + "' >" + mapAttr.Name + "</a>";
                    case 13: //身份证
                        if (mapAttr.KeyOfEn == "IDCardAddress") {
                            eleHtml = "<div style='text-align:left;padding-left:0px'  data-type='1'>";
                            eleHtml += "<input type=text " + ccsCtrl + " style='width:75% !important;display:inline;' class='form-control' maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "'/>";
                            eleHtml += "<label class='image-local' style='margin-left:5px'><input type='file' accept='image/png,image/bmp,image/jpg,image/jpeg' style='width:25% !important;display:none' onchange='GetIDCardInfo()'/>上传身份证</label>";
                            eleHtml += "</div>";
                            return eleHtml;
                        } else {
                            return "<input " + ccsCtrl + "  maxlength = " + mapAttr.MaxLen + "  value = '" + mapAttr.DefVal + "' name = 'TB_" + mapAttr.KeyOfEn + "' id = 'TB_" + mapAttr.KeyOfEn + "' type = 'text'  disabled='disabled'/>";
                        }
                        break;
                    case 16: //系统定位
                        eleHtml = "<div style='text-align:left;padding-left:0px' >";
                        eleHtml += "<input type='button' " + ccsCtrl + " name='select' value='系统定位' />";
                        eleHtml += "</div>";
                        return eleHtml;
                    case 18: //按钮
                        return "<input type='button' " + ccsCtrl + "  id='Btn_" + mapAttr.KeyOfEn + "' name='Btn_" + mapAttr.KeyOfEn + "' value='" + mapAttr.Name + "' onclick=''/>";
                    case 50: //工作进度
                        return "<img  src='./Img/JobSchedule.png'  " + ccsCtrl + " style='border:0px;height:" + mapAttr.UIHeight + "px;width:100%;' id='Img" + mapAttr.KeyOfEn + "' />";
                    case 101: //评分标准
                        eleHtml = "<div style='text-align:left;padding-left:0px'  data-type='1'>";
                        eleHtml += "<span class='simplestar'>";

                        var num = mapAttr.Tag2;
                        for (var i = 0; i < num; i++) {

                            eleHtml += "<img src='../../Style/Img/star_2.png' />";
                        }
                        eleHtml += "&nbsp;&nbsp;<span " + ccsCtrl + " class='score-tips' style='vertical-align: middle;color:#ff6600;font: 12px/1.5 tahoma,arial,\"Hiragino Sans GB\",宋体,sans-serif;'><strong>" + num + "  分</strong></span>";
                        eleHtml += "</span></div>";
                        return eleHtml;
                    default:
                        if (mapAttr.UIHeight <= 40)
                            return "<div id='DIV_" + mapAttr.KeyOfEn + "' class='ccbpm-input-group'><input " + ccsCtrl + "  value='" + mapAttr.DefVal + "' maxlength=" + mapAttr.MaxLen + "  name='TB_" + mapAttr.KeyOfEn + "' id='TB_" + mapAttr.KeyOfEn + "'placeholder='" + (mapAttr.Tip || '') + "' type='text' " + (mapAttr.UIIsEnable == 1 ? '' : ' disabled="disabled"') + "/></div>";

                        if (mapAttr.UIHeight > 23) {
                            var uiHeight = mapAttr.UIHeight;
                            return "<div id='DIV_" + mapAttr.KeyOfEn + "' class='ccbpm-input-group'> <textarea " + ccsCtrl + "  maxlength=" + mapAttr.MaxLen + " style='height:" + uiHeight + "px;width:100%;' name='TB_" + mapAttr.KeyOfEn + "' id='TB_" + mapAttr.KeyOfEn + "'placeholder='" + (mapAttr.Tip || '') + "' type='text' " + (mapAttr.UIIsEnable == 1 ? '' : ' disabled="disabled"') + "/></div>";
                        }

                        return "<div id='DIV_" + mapAttr.KeyOfEn + "' class='ccbpm-input-group'> <input " + ccsCtrl + "  maxlength=" + mapAttr.MaxLen + "  value='" + mapAttr.DefVal + "' name='TB_" + mapAttr.KeyOfEn + "' id='TB_" + mapAttr.KeyOfEn + "'placeholder='" + (mapAttr.Tip || '') + "' type='text' " + (mapAttr.UIIsEnable == 1 ? '' : ' disabled="disabled"') + " /></div>";
                }
                break;
            case 2: //整数
                return "<div id='DIV_" + mapAttr.KeyOfEn + "' class='ccbpm-input-group'><input " + ccsCtrl + "  value='0' style='text-align:right;'  onkeyup=" + '"' + "valitationAfter(this, 'int');if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "valitationAfter(this, 'int');if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " id='TB_" + mapAttr.KeyOfEn + "'placeholder='" + (mapAttr.Tip || '') + "'/></div>";
            case 4: //复选框
                if (mapAttr.UIIsEnable == 0) {
                    enableAttr = "disabled='disabled'";
                } else {
                    enableAttr = "";
                }
                return "<div class='checkbox' id='DIV_" + mapAttr.KeyOfEn + "'><label for='CB_" + mapAttr.KeyOfEn + "' ></label><input " + ccsCtrl + " " + (mapAttr.DefVal == 1 ? "checked='checked'" : "") + " type='checkbox' " + enableAttr + " name='CB_" + mapAttr.KeyOfEn + "' id='CB_" + mapAttr.KeyOfEn + "' value='" + mapAttr.Name + "'  lay-skin='switch' lay-text='是|否'/>&nbsp;</div>";
            case 3: //浮点
            case 5: //双精度
                var attrdefVal = mapAttr.DefVal;
                var bit;
                if (attrdefVal != null && attrdefVal !== "" && attrdefVal.indexOf(".") >= 0)
                    bit = attrdefVal.substring(attrdefVal.indexOf(".") + 1).length;
                if (attrdefVal == null || attrdefVal == "")
                    attrdefVal = 0.00;


                return "<input " + ccsCtrl + "  value='" + attrdefVal + "' style='text-align:right;'  onkeyup=" + '"' + "valitationAfter(this, 'float');if(isNaN(value)) execCommand('undo');limitLength(this," + bit + ");" + '"' + " onafterpaste=" + '"' + " valitationAfter(this, 'float');if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text' id='TB_" + mapAttr.KeyOfEn + "' placeholder='" + (mapAttr.Tip || '') + "'/>";
            case 6: //日期类型
            case 7: //时间类型
                //生成中间的部分.
                var enableAttr = '';
                var dateFmt = "yyyy-MM-dd"; //日期格式.
                if (mapAttr.MyDataType == 7)
                    dateFmt = "yyyy-MM-dd HH:mm:ss";

                if (mapAttr.UIIsEnable == 0) {

                    enableAttr = "disabled='disabled' ";
                }
                ccsCtrl = " class ='ccdate ";

                if (mapAttr.CSSCtrl != "")
                    ccsCtrl += mapAttr.CSSCtrl + "'";
                else
                    ccsCtrl += "'";


                return "<div id='DIV_" + mapAttr.KeyOfEn + "' class='ccbpm-input-group'> <input " + ccsCtrl + "  data-info='" + dateFmt + "' maxlength=" + mapAttr.MaxLen + " value='" + mapAttr.DefVal + "'  type='text' " + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "' id='TB_" + mapAttr.KeyOfEn + "'/></div>";
            case 8: //金额
                //获取DefVal,根据默认的小数点位数来限制能输入的最多小数位数
                var attrdefVal = mapAttr.DefVal;
                var bit;
                if (attrdefVal != null && attrdefVal !== "" && attrdefVal.indexOf(".") >= 0)
                    bit = attrdefVal.substring(attrdefVal.indexOf(".") + 1).length;
                else
                    bit = 2;
                return "<div id='DIV_" + mapAttr.KeyOfEn + "' class='ccbpm-input-group'><input value='0.00' " + ccsCtrl + " style='text-align:right;' onkeyup=" + '"' + "valitationAfter(this, 'money');limitLength(this," + bit + "); FormatMoney(this, " + bit + ", ',')" + '"' + " onafterpaste=" + '"' + "valitationAfter(this, 'money');if(isNaN(value))execCommand('undo');" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text' id='TB_" + mapAttr.KeyOfEn + "' value='0.00' placeholder='" + (mapAttr.Tip || '') + "'/></div>";
            default:
                break;

        }
    }

    //下拉框 外键和外部数据源
    if ((mapAttr.LGType == "0" && mapAttr.MyDataType == "1" && mapAttr.UIContralType == 1) ||
        (mapAttr.LGType == "2" && mapAttr.MyDataType == "1")) {
        return "<div id='DIV_" + mapAttr.KeyOfEn + "' class='ccbpm-input-group'> <input " + ccsCtrl + "  placeholder='外键/外部数据源:" + mapAttr.UIBindKey + "'  maxlength=" + mapAttr.MaxLen + "  value='" + mapAttr.DefVal + "' name='TB_" + mapAttr.KeyOfEn + "' id='TB_" + mapAttr.KeyOfEn + "'placeholder='" + (mapAttr.Tip || '') + "' type='text' " + (mapAttr.UIIsEnable == 1 ? '' : ' disabled="disabled"') + " /></div>";

    }
    //枚举 单选枚举和下拉框枚举
    if (mapAttr.LGType == 1) {
        var ses = GetSysEnums(mapAttr.UIBindKey);
        if (mapAttr.UIContralType == 1) { //下拉框显示
            var operations = "";
            $.each(ses, function (i, obj) {
                operations += "<option  value='" + obj.IntKey + "'>" + obj.Lab + "</option>";
            });
            return "<div id='DIV_" + mapAttr.KeyOfEn + "'><select " + ccsCtrl + " name='DDL_" + mapAttr.KeyOfEn + "'  id='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable == 1 ? '' : 'disabled="disabled"') + "  onchange='changeEnable(this,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")'>" + operations + "</select></div>";
        }
        if (mapAttr.UIContralType == 2) { //复选框
            var rbHtmls = "";
            //显示方式,默认为 0=横向展示 3=横向.. 
            var RBShowModel = 0;
            if (mapAttr.AtPara.indexOf('@RBShowModel=3') >= 0)
                RBShowModel = 3;
            for (var i = 0; i < ses.length; i++) {
                var se = ses[i];
                var br = "";
                if (RBShowModel == 0)
                    br = "<br>";
                var checked = "";
                if (se.IntKey == mapAttr.DefVal)
                    checked = " checked=true ";
                rbHtmls += "<input " + ccsCtrl + " type=checkbox name='CB_" + mapAttr.KeyOfEn + "' id='CB_" + mapAttr.KeyOfEn + "_" + se.IntKey + "' value='" + se.IntKey + "' " + checked + " onclick='clickEnable( this ,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")' title='" + se.Lab + "'/>" + br;
            }
            return "<div id='DIV_" + mapAttr.KeyOfEn + "'>" + rbHtmls + "</div>";
        }

        if (mapAttr.UIContralType == 3) { //单选按钮显示
            var rbHtmls = "";
            //显示方式,默认为 0=横向展示 3=横向.. 
            var RBShowModel = 0;
            if (mapAttr.AtPara.indexOf('@RBShowModel=3') >= 0)
                RBShowModel = 3;
            for (var i = 0; i < ses.length; i++) {
                var se = ses[i];
                var br = "";
                if (RBShowModel == 0)
                    br = "<br>";
                var checked = "";
                if (se.IntKey == mapAttr.DefVal)
                    checked = " checked=true ";
                rbHtmls += "<input " + ccsCtrl + " type=radio name='RB_" + mapAttr.KeyOfEn + "' id='RB_" + mapAttr.KeyOfEn + "_" + se.IntKey + "' value='" + se.IntKey + "' " + checked + " onclick='clickEnable( this ,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")' title='" + se.Lab + "'/>" + br;
            }
            return "<div id='DIV_" + mapAttr.KeyOfEn + "'>" + rbHtmls + "</div>";
        }
    }
}

/**
 * 获取枚举
 * @param {any} enumKey
 */
function GetSysEnums(enumKey) {

    if (webUser.CCBPMRunModel == 0 || webUser.CCBPMRunModel == 1) {
        var ses = new Entities("BP.Sys.SysEnums");
        ses.Retrieve("EnumKey", enumKey, "IntKey");
        return ses;
    }

    var ses = new Entities("BP.Cloud.Sys.SysEnums");
    ses.Retrieve("EnumKey", enumKey.replace(webUser.OrgNo + "_", ""), "OrgNo", webUser.OrgNo, "IntKey");
    return ses;
}


function getColSummary(cols) {
    return cols.map(function (col) {
        return parseInt(col.replace('layui-col-xs', ''))
    }).reduce(function (total, num) {
        return total + num
    })
}

/**
 * 获取字段占的列数
 * @param {any} colSpan
 * @param {any} tabCol
 */
function getColSpanClass(colSpan, tabCol) {
    if (tabCol == 4) {
        switch (colSpan) {
            case 1:
                return "layui-col-xs4";
            case 2:
                return "layui-col-xs6";
            case 3:
                return "layui-col-xs10";
            case 4:
                return "layui-col-xs12";
            default:
                return "layui-col-xs4";
        }
    }
    if (tabCol == 6) {
        switch (colSpan) {
            case 1:
                return "layui-col-xs3";
            case 2:
                return "layui-col-xs4";
            case 3:
                return "layui-col-xs7";
            case 4:
                return "layui-col-xs8";
            case 5:
                return "layui-col-xs11";
            case 6:
                return "layui-col-xs12";
            default:
                return "layui-col-xs3";
        }
    }
}
/**
 * 获取标签占的列数
 * @param {any} LabelColSpan
 * @param {any} tabCol
 */
function getLabelColSpanClass(LabelColSpan, tabCol) {
    if (tabCol == 4) {
        switch (LabelColSpan) {
            case 1:
                return "layui-col-xs2";
            case 2:
                return "layui-col-xs6";
            case 3:
                return "layui-col-xs8";
            case 4:
                return "layui-col-xs12";
            default:
                return "layui-col-xs2";
        }
    }
    if (tabCol == 6) {
        switch (LabelColSpan) {
            case 1:
                return "layui-col-xs1";
            case 2:
                return "layui-col-xs4";
            case 3:
                return "layui-col-xs5";
            case 4:
                return "layui-col-xs8";
            case 5:
                return "layui-col-xs9";
            case 6:
                return "layui-col-xs12";
            default:
                return "layui-col-xs1";
        }
    }
}

/************************************************编辑表单字段属性中的操作****************************************************************************************************************************************************************/
/**
 * 编辑大文本字段的属性
 * @param {any} frmID
 * @param {any} keyOfEn
 */
function EditBigText(frmID, keyOfEn) {
    getWindowWH();
    url = './EditFExtContral/60.BigNoteHtmlText.htm?FrmID=' + frmID + '&KeyOfEn=' + keyOfEn;
    OpenLayuiDialog(url, '大文本编辑', W, 0, null, true);
}
/**
 * 编辑字段属性
 * @param {any} mypk
 * @param {any} ftype
 * @param {any} gf
 * @param {any} fk_mapdtl
 * @param {any} uiContralType
 */
function Edit(mypk, ftype, gf, fk_mapdtl, uiContralType) {
    getWindowWH();
    var url = './EditF.htm?DoType=Edit&MyPK=' + mypk + '&FType=' + ftype + '&FK_MapData=' + frmID + '&GroupField=' + gf;
    var title = '';
    switch (parseInt(ftype)) {
        case 1:
            switch (parseInt(uiContralType)) {
                case 0:
                    title = '字段String属性';
                    url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrString&PKVal=' + mypk + '&s=' + Math.random();
                    break;
                case 4:
                    title = '地图';
                    url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtMap&PKVal=' + mypk + '&s=' + Math.random();
                    break;
                case 6:
                    title = '附件组件';
                    url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.FrmAttachmentExt&PKVal=' + mypk + '&s=' + Math.random();
                    break;
                case 8:
                    title = '手写签名版';
                    url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrString&PKVal=' + mypk + '&s=' + Math.random();
                    break;
                case 9:
                    title = '字段String超连接';
                    url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtLink&PKVal=' + mypk + '&s=' + Math.random();
                    break;
                case 11:
                    var imgEn = new Entity("BP.Sys.FrmUI.ExtImg");
                    imgEn.SetPKVal(mypk);

                    if (imgEn.RetrieveFromDBSources() == 0) {
                        var mapAttr = new Entity("BP.Sys.MapAttr", mypk);

                        imgEn.CopyJSON(mapAttr);
                        imgEn.MyPK = mypk;
                        imgEn.Insert();
                    }

                    title = '装饰类图片属性';
                    url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtImg&PKVal=' + mypk + '&s=' + Math.random();
                    break;
                case 12:
                    title = '图片附件';
                    url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.FrmImgAth&PKVal=' + mypk + '&s=' + Math.random();
                    break;
                case 13:
                    title = '证件字段属性';
                    url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrCard&PKVal=' + mypk + '&s=' + Math.random();
                    break;
                case 14:
                    title = '签批组件';
                    url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrCheck&PKVal=' + mypk + '&s=' + Math.random();
                    break;
                case 15:
                    title = '评论组件';
                    url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrFlowBBS&PKVal=' + mypk + '&s=' + Math.random();
                    break;
                case 16:
                    title = '系统定位属性';
                    url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrFixed&PKVal=' + mypk + '&s=' + Math.random();
                    break;
                case 17:
                    title = '发文字号属性';
                    url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrDocWord&PKVal=' + mypk + '&s=' + Math.random();
                    break;
                case 170:
                    title = '收文字号属性';
                    url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrDocWordReceive&PKVal=' + mypk + '&s=' + Math.random();
                    break;
                case 18:
                    title = '按钮属性';
                    url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.FrmBtn&PKVal=' + mypk + '&s=' + Math.random();
                    break;
                case 50:
                    title = '流程进度图';
                    url = '../../Comm/EnOnly.htm?EnName=BP.WF.Template.ExtJobSchedule&PKVal=' + mypk + '&s=' + Math.random();
                    break;
                case 101:
                    title = '评分控件';
                    url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtScore&PKVal=' + mypk + '&s=' + Math.random();
                    break;
                case 110:
                    title = '公文组件';
                    url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrGovDocFile&PKVal=' + mypk + '&s=' + Math.random();
                    break;
                case 111:
                    title = '打印组件';
                    url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrPrint&PKVal=' + mypk + '&s=' + Math.random();
                    break;
                default:
                    layer.alert("不可能出现的控件:FType:" + ftype + " UIControlType:" + uiContralType);
                    return;
            }
            break;
        case 2:
        case 3:
        case 5:
        case 8:
            title = '字段Num属性';
            url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&PKVal=' + mypk + '&s=' + Math.random();
            break;
        case 6:
        case 7:
            title = '字段 date/datetime 属性';
            url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrDT&PKVal=' + mypk + '&s=' + Math.random();
            break;
        case 4:
            title = '字段 boolen 属性';
            url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrBoolen&PKVal=' + mypk + '&s=' + Math.random();
            break;

        default:
            layer.alert("不可能出现的控件:FType:" + ftype);
            return;
    }
    OpenLayuiDialog(url, title, W, 0, null, true);
    return;
}
/**
 * 修改枚举字段的属性
 * @param {any} fk_mapdata
 * @param {any} mypk
 * @param {any} keyOfEn
 */
function EditEnum(fk_mapdata, mypk, keyOfEn) {
    getWindowWH();
    var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrEnum&PKVal=' + mypk + '&s=' + Math.random();
    OpenLayuiDialog(url, '枚举' + keyOfEn + '属性', W, false, false, true);

}
/**
 * 编辑外部数据源的属性
 * @param {any} mypk
 * @param {any} keyOfEn
 */
function EditTableSQL(mypk, keyOfEn) {
    getWindowWH();
    var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrSFSQL&PKVal=' + mypk + '&s=' + Math.random();
    OpenLayuiDialog(url, '外键SQL字段:' + keyOfEn + '属性', W, 0, null, true);
}
/**
 * 编辑外键字段的属性
 * @param {any} fk_mapData
 * @param {any} mypk
 * @param {any} keyOfEn
 */
function EditTable(fk_mapData, mypk, keyOfEn) {
    getWindowWH();
    var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrSFTable&PKVal=' + mypk + '&s=' + Math.random();
    OpenLayuiDialog(url, '外键字段:' + keyOfEn + '属性', W, false, false, true);

}

// 子线程.
function EditFrmThread(mypk) {
    getWindowWH();
    if (mypk == null || mypk == "undefined")
        mypk = nodeID;
    var url = '../../Comm/En.htm?EnName=BP.WF.Template.FrmTrack&PKVal=' + mypk;
    OpenLayuiDialog(url, '子线程', W, 0, null, true);


}
// 轨迹图.
function EditTrack(mypk) {
    getWindowWH();
    if (mypk == null || mypk == "undefined")
        mypk = nodeID;
    var url = '../../Comm/EnOnly.htm?EnName=BP.WF.Template.FrmNodeComponent&PKVal=' + mypk + '&tab=轨迹组件';
    OpenLayuiDialog(url, '轨迹图', W, 0, null, true);

}

/// 审核组件.
function EditFWC(mypk) {
    getWindowWH();
    if (mypk == null || mypk == "undefined")
        mypk = nodeID;
    var url = '../../Comm/EnOnly.htm?EnName=BP.WF.Template.FrmNodeComponent&PK=' + mypk + '&tab=审核组件';
    OpenLayuiDialog(url, '组件', W, 0, null, true);
}
/**
 * 子流程属性编辑
 * @param {any} mypk
 */
function EditSubFlow(mypk) {
    getWindowWH();
    mypk = nodeID;
    var url = '../../Comm/En.htm?EnName=BP.WF.Template.SFlow.FrmSubFlow&PKVal=' + mypk + '&tab=父子流程组件';
    OpenLayuiDialog(url, '子流程组件', W, 0, null, true);

}
/**
 * 编辑附件属性
 * @param {any} ath
 */
function EditAth(ath) {
    getWindowWH();
    var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.FrmAttachmentExt&FK_MapData=' + frmID + '&MyPK=' + ath;
    OpenLayuiDialog(url, '附件', W, 0, null, true);
}
/**
 * 编辑框架的属性
 * @param {any} myPK
 */
function EditFrame(myPK) {
    getWindowWH();
    var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapFrameExt&FK_MapData=' + frmID + '&MyPK=' + myPK;
    OpenLayuiDialog(url, '框架', W, 0, null, true);
}
/**
 * 编辑从表
 * @param {any} mypk
 */
function EditDtl(mypk) {
    getWindowWH();
    var url = '../../Comm/En.htm?EnName=BP.WF.Template.Frm.MapDtlExt&FK_MapData=' + frmID + '&No=' + mypk;
    OpenLayuiDialog(url, '从表', W, 0, null, true);
}
/**
 * 从表插入例
 * @param {any} fk_mapdtl
 */
function AddFForDtl(fk_mapdtl) {
    getWindowWH();
    var url = './FieldTypeList.htm?FK_MapData=' + fk_mapdtl + '&inlayer=1&s=' + Math.random();

    OpenLayuiDialog(url, '从表', W, 0, null, true, null, function () {
        var frm = document.getElementById("F" + fk_mapdtl);
        frm.src = frm.src;
    });
}
/**
 * 编辑组件信息
 * @param {any} mypk
 */
function FrmNodeComponent(mypk) {
    getWindowWH();
    if (mypk == null || mypk == "undefined")
        mypk = nodeID;

    if (mypk == null || mypk == undefined) {
        layer.alert('非节点表单');
        return;
    }
    var url = '../../Comm/EnOnly.htm?EnName=BP.WF.Template.FrmNodeComponent&PKVal=' + mypk;
    OpenLayuiDialog(url, '组件', W, 0, null, true);
}
/**
 * 编辑分组信息
 * @param {any} mypk
 * @param {any} OID
 */
function GroupField(mypk, OID) {
    getWindowWH();
    var url = "../../Comm/En.htm?EnName=BP.Sys.GroupField&PKVal=" + OID;
    OpenLayuiDialog(url, '分组', W, 0, null, true);

}

/************************************************新增表单字段属性中的操作****************************************************************************************************************************************************************/
/**
 * 增加基本字段
 */
function AddF(typeName) {
    getWindowWH();
    var url = './FrmTextBox.htm?DataType=' + typeName + '&FK_MapData=' + frmID + '&s=' + Math.random();
    OpenLayuiDialog(url, '增加字段', W, 0, null, true);

}
/**
 * 增加枚举字段
 * @param {any} typeName
 */
function AddEnum(typeName) {
    getWindowWH();
    var type = 3;
    if (typeName == "RadioSelect") type = 1;
    if (typeName == "RadioCheckBox") type = 2;
    var url = "";
    if (webUser.CCBPMRunModel == 2) {
        url = '../../../Admin/CCFormDesigner/SysEnum/List.htm?DoType=AddEnum&FK_MapData=' + frmID + '&GroupField=' + groupID + "&Type=" + type;
    } else {
        url = './SysEnumList.htm?DoType=AddEnum&FK_MapData=' + frmID + '&GroupField=' + groupID + "&Type=" + type;
    }

    OpenLayuiDialog(url, '', W, 0, null, true);
    //  OpenLayuiDialog(url, '新建枚举', W, 0, null, true);


}
/**
 * 创建外键或者外部数据源
 */
function AddSelect() {
    getWindowWH();
    var url = "";
    // SaaS模式下进入SaaS下页面
    if (webUser.CCBPMRunModel == 2) {
        url = './SFList.htm?DoType=AddSFTable&FK_MapData=' + frmID + '&FType=Class&GroupField=' + groupID;
    } else {
        url = './SFList.htm?DoType=AddSFTable&FK_MapData=' + frmID + '&FType=Class&GroupField=' + groupID;
    }
    OpenLayuiDialog(url, '创建外键/外部数据源', W, 0, null, true);
    //LayuiPopRight(url, '创建外键/外部数据源', W, true);
}


function GovDoc(pinyin) {
    layer.prompt({
        title: '请输入文件名称,比如:正文',
        formType: 0,
        value: '正文',
    }, function (text, index) {
        layer.close(index);
        if (text == null || text == undefined)
            return;
        var mapAttrs = new Entities("BP.Sys.MapAttrs");
        mapAttrs.Retrieve("FK_MapData", frmID, "Name", text);
        if (mapAttrs.length >= 1) {
            alert('名称：[' + text + "]已经存在.");
            AddHandWriting(pinyin);
            return;
        }

        var id = pinyin.makePy(text);
        var mypk = frmID + "_" + id;
        var mapAttr = new Entity("BP.Sys.MapAttr");
        mapAttr.MyPK = mypk;
        if (mapAttr.IsExits == true) {
            alert('名称：[' + text + "]已经存在.");
            AddHandWriting(pinyin);
            return;
        }
        var mypk = frmID + "_" + id;
        var mapAttr = new Entity("BP.Sys.MapAttr");
        mapAttr.UIContralType = 110; //公文组件.
        mapAttr.MyPK = mypk;
        mapAttr.FK_MapData = frmID;
        mapAttr.KeyOfEn = id;
        mapAttr.Name = text;
        mapAttr.GroupID = groupID;
        mapAttr.MyDataType = 1;
        mapAttr.LGType = 0;
        mapAttr.ColSpan = 1; //
        mapAttr.UIWidth = 150;
        mapAttr.UIHeight = 170;
        mapAttr.Insert(); //插入字段.
        mapAttr.Retrieve();
        var url = "../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrGovDocFile&MyPK=" + mapAttr.MyPK;
        getWindowWH();
        OpenLayuiDialog(url, '公文组件', W, 0, null, true);

    });
}
/**
 * 写字板
 * @param {any} pinyin
 */
function AddHandWriting(pinyin) {
    layer.prompt({
        title: '请输入签名版名称,比如:签字版、签名',
        formType: 0,
        value: '签字版',
    }, function (text, index) {
        layer.close(index);
        if (text == null || text == undefined)
            return;
        var mapAttrs = new Entities("BP.Sys.MapAttrs");
        mapAttrs.Retrieve("FK_MapData", frmID, "Name", text);
        if (mapAttrs.length >= 1) {
            alert('名称：[' + text + "]已经存在.");
            AddHandWriting(pinyin);
            return;
        }

        var id = pinyin.makePy(text);
        var mypk = frmID + "_" + id;
        var mapAttr = new Entity("BP.Sys.MapAttr");
        mapAttr.MyPK = mypk;
        if (mapAttr.IsExits == true) {
            alert('名称：[' + text + "]已经存在.");
            AddHandWriting(pinyin);
            return;
        }
        var mypk = frmID + "_" + id;
        var mapAttr = new Entity("BP.Sys.MapAttr");
        mapAttr.UIContralType = 8; //手写签名版.
        mapAttr.MyPK = mypk;
        mapAttr.FK_MapData = frmID;
        mapAttr.KeyOfEn = id;
        mapAttr.Name = text;
        mapAttr.GroupID = groupID;
        mapAttr.MyDataType = 1;
        mapAttr.LGType = 0;
        mapAttr.ColSpan = 1; //
        mapAttr.UIWidth = 150;
        mapAttr.UIHeight = 170;
        mapAttr.Insert(); //插入字段.
        mapAttr.Retrieve();
        var url = "../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtHandWriting&MyPK=" + mapAttr.MyPK;
        getWindowWH();
        OpenLayuiDialog(url, '签字版', W, 0, null, true);

    });
}

//签批组件
function AddWorkCheck(pinyin) {
    layer.prompt({
        title: '请输入签批组件的名称:\t\n比如:办公室意见、拟办意见',
        formType: 0,
        value: '',
    }, function (name, index) {
        layer.close(index);
        if (name == null || name == undefined || name.trim() == "")
            return "";
        var mapAttrs = new Entities("BP.Sys.MapAttrs");
        mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
        if (mapAttrs.length >= 1) {
            layer.alert('名称：[' + name + "]已经存在.");
            AddWorkCheck(pinyin);
            return "";
        }
        var keyOfEn = pinyin.makePy(name);
        var mypk = frmID + "_" + keyOfEn;
        var mapAttr = new Entity("BP.Sys.MapAttr");
        mapAttr.MyPK = mypk;
        if (mapAttr.IsExits == true) {
            layer.alert('名称：[' + name + "]已经存在.");
            return "";
        }
        mapAttr.FK_MapData = frmID;
        mapAttr.KeyOfEn = keyOfEn;
        mapAttr.Name = name;
        mapAttr.GroupID = 1;
        mapAttr.UIContralType = 14; //签批意见
        mapAttr.MyDataType = 1;
        mapAttr.LGType = 0;
        mapAttr.ColSpan = 3; //
        mapAttr.LabelColSpan = 1; //
        mapAttr.UIWidth = 150;
        mapAttr.UIHeight = 50;
        mapAttr.IsEnableInAPP = 1;
        mapAttr.Insert(); //插入字段.
        var url = "../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrCheck&MyPK=" + mapAttr.MyPK;
        getWindowWH();
        OpenLayuiDialog(url, '签批字段', W, 0, null, true);


    });

}

/**
 * 创建审核分组
 * @param {any} pinyin
 */
function AddGroupFWC(pinyin) {
    var mapData = new Entity("BP.Sys.MapData", GetQueryString("FK_MapData"));
    layer.prompt({
        title: '请输入分组名称, 比如:' + mapData.Name,
        formType: 0,
        value: mapData.Name,
    }, function (text, index) {
        layer.close(index);
        if (text == null || text == undefined)
            return;
        var defVal = pinyin.makePy(text);
        if (defVal == '' || defVal == null)
            defVal = "";

        var msg = "<span>系统就会自动创建,如下三个审核字段到数据库里,并且设置了默认值.</span></br>";
        msg += "<span>" + defVal + "_Checker审核人</span><br/>";
        msg += "<span>" + defVal + "_RDT 审核日期</span><br/>";
        msg += "<span>" + defVal + "_Note 审核意见</span><br/>";
        msg += "<div class='layui-layer-content'style='padding-left:0px'><input type=text id='TB_key' value='" + defVal + "' class='layui-layer-input' style='margin:0px 0px'/></div>"
        layer.prompt({
            title: "请输入分组前缀，比如您输入:" + defVal,
            content: msg,
            formType: 0,
            value: defVal,
        }, function (prix, index) {
            layer.close(index);
            if (prix == null || prix == undefined || prix == "")
                return;
            if (!CheckID(prix)) {
                layer.alert("编号不符合规则");
                return;
            }
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_FoolFormDesigner");
            handler.AddPara("TB_Check_Name", text);
            handler.AddPara("TB_Check_No", prix);
            handler.AddPara("FK_MapData", frmID);

            var data = handler.DoMethodReturnString("GroupField_SaveCheck");
            layer.alert(data);
            Reload();
        });

    });

}
/**
 * 创建字段分组 
 */
function AddGroupField() {
    var mapData = new Entity("BP.Sys.MapData", GetQueryString("FK_MapData"));
    layer.prompt({
        title: '请输入分组名称',
        formType: 0,
        value: mapData.Name,
    }, function (text, index) {
        layer.close(index);
        if (text == null || text == undefined)
            return;

        var gf = new Entity("BP.Sys.GroupField");//  gf = new GroupField();
        gf.FrmID = frmID; //this.FK_MapData;
        gf.Lab = text;// this.GetRequestVal("Lab");
        gf.EnName = frmID;// this.FK_MapData;
        gf.Insert();

        layer.alert("创建成功.");

        Reload();
        return;
    });

}
/**
 * 增加附件字段
 * @param {any} pingyin
 */
function AddFieldAth(pinyin) {
    layer.prompt({
        title: '请输入附件名称:\t\n比如:报送材料、报销资料',
        formType: 0,
        value: '附件',
    }, function (name, index) {
        layer.close(index);
        if (name == null || name == undefined || name.trim() == "") {
            layer.alert("字段附件的名称不能为空");
            AddFieldAth(pinyin);
            return "";
        }
        var mapAttrs = new Entities("BP.Sys.MapAttrs");
        mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
        if (mapAttrs.length >= 1) {
            layer.alert('名称：[' + name + ']的附件已经存在.');
            AddFieldAth(pinyin);
            return "";
        }
        //获得ID.
        var id = pinyin.makePy(name);
        layer.prompt({
            title: '请输入附件编号:\t\n比如:BSCL、BXZL',
            formType: 0,
            value: id,
        }, function (no, index) {
            layer.close(index);
            if (no == null || no == undefined || no.trim() == "") {
                layer.alert("字段附件的编号不能为空");
                AddFieldAth(pinyin);
                return "";
            }
            var mypk = frmID + "_" + no;
            var mapAttr = new Entity("BP.Sys.MapAttr");
            mapAttr.MyPK = mypk;
            if (mapAttr.IsExits == true) {
                layer.alert('名称为：[' + name + ']，编号为[' + id + ']的附件已经存在.');
                AddFieldAth(pinyin);
                return "";
            }
            mapAttr.FK_MapData = frmID;
            mapAttr.KeyOfEn = no;
            mapAttr.Name = name;
            mapAttr.GroupID = groupID;
            mapAttr.UIContralType = 6; //附件控件.
            mapAttr.MyDataType = 1;
            mapAttr.LGType = 0;
            mapAttr.ColSpan = 3; //
            mapAttr.LabelColSpan = 1; //
            mapAttr.UIWidth = 150;
            mapAttr.UIHeight = 170;
            mapAttr.IsEnableInAPP = 0;
            mapAttr.Insert(); //插入字段.

            mapAttr.Retrieve();

            var en = new Entity("BP.Sys.FrmAttachment");
            en.MyPK = frmID + "_" + no;
            en.FK_MapData = frmID;
            en.NoOfObj = no;
            en.GroupID = mapAttr.GroupID; //设置分组列.
            en.Name = name;
            en.UploadType = 1; //多附件.
            en.IsVisable = 0; //让其不可见.
            en.CtrlWay = 4; // 按流程WorkID计算
            en.SetPara("IsShowMobile", 1);
            en.SetPara("IsFieldAth", 1);
            en.Insert(); //插入到数据库.

            var url = "../../Comm/En.htm?EnName=BP.Sys.FrmUI.FrmAttachmentExt&MyPK=" + en.MyPK;
            getWindowWH();
            OpenLayuiDialog(url, '附件属性', W, 0, null, true);
        });

    });
}
/**
 * 新建附件
 */
function AddAth() {
    layer.prompt({
        title: '请输入附件ID:(要求是字母数字下划线，非中文等特殊字符.)',
        formType: 0,
        value: 'Ath1',
    }, function (val, index) {
        layer.close(index);
        if (val == null) {
            return;
        }

        if (val == '') {
            layer.alert('附件ID不能为空，请重新输入！');
            return;
        }

        if (!CheckID(val)) {
            layer.alert("编号不符合规则");
            return;
        }

        var en = new Entity("BP.Sys.FrmAttachment");
        en.MyPK = frmID + "_" + val;
        if (en.RetrieveFromDBSources() == 1) {
            alert("标识为:" + val + "附件已经存在.");
            return;
        }
        en.NoOfObj = val;
        en.FK_MapData = frmID;
        en.UploadType = 1;
        en.Name = "附件:" + en.NoOfObj;
        en.SetPara("IsShowMobile", 1);
        en.Insert();

        //var gf = new Entity("BP.Sys.GroupField");
        //gf.Retrieve("FrmID", frmID, "CtrlID", en.MyPK);
        //gf.Retrieve("FrmID", frmID, "CtrlID", en.MyPK);

        //var gfs = new Entities("BP.Sys.GroupFields");
        //gfs.Retrieve("FrmID", frmID, "CtrlID", en.MyPK);





        getWindowWH();
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.FrmAttachmentExt&FK_MapData=' + frmID + '&MyPK=' + en.MyPK;
        OpenLayuiDialog(url, '附件属性', W, 0, null, true);
        return;
    });

}
//图片附件.
function AddImgAth(pinyin) {
    layer.prompt({
        title: '请输入图片名称:\t\n比如:肖像、头像、ICON',
        formType: 0,
        value: '肖像',
    }, function (name, index) {
        layer.close(index);
        if (name == null || name == undefined || name.trim() == "")
            return "";

        var mapAttrs = new Entities("BP.Sys.MapAttrs");
        mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
        if (mapAttrs.length >= 1) {
            layer.alert('名称：[' + name + "]已经存在.");
            AddImgAth(pinyin);
            return "";
        }

        //获得ID.
        var id = pinyin.makePy(name);

        var mypk = frmID + "_" + id;
        var mapAttr = new Entity("BP.Sys.MapAttr");
        mapAttr.MyPK = mypk;
        if (mapAttr.IsExits == true) {
            alert('名称：[' + name + "]已经存在.");
            return "";
        }
        mapAttr.FK_MapData = frmID;
        mapAttr.KeyOfEn = id;
        mapAttr.Name = name;
        mapAttr.GroupID = groupID;
        mapAttr.UIContralType = 12; //FrmImgAth 类型的控件.
        mapAttr.MyDataType = 1;
        mapAttr.ColSpan = 1; //单元格.
        mapAttr.LGType = 0;
        mapAttr.UIWidth = 150;
        mapAttr.UIHeight = 170;
        mapAttr.Insert(); //插入字段.
        mapAttr.Retrieve();

        var en = new Entity("BP.Sys.FrmUI.FrmImgAth");
        en.MyPK = frmID + "_" + id;
        en.FK_MapData = frmID;
        en.CtrlID = id;
        en.Name = name;
        en.GroupID = mapAttr.GroupID; //设置分组列.

        en.Insert(); //插入到数据库.
        getWindowWH();
        var url = "../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.FrmImgAth&MyPK=" + en.MyPK;
        OpenLayuiDialog(url, '图片附件属性', W, 0, null, true);
        return;

    });

}
/**
 * 新增从表
 */
function NewMapDtl() {
    layer.prompt({
        title: '请输入从表ID，要求表单唯一',
        formType: 0,
        value: frmID + 'Dtl1',
    }, function (text, index) {
        layer.close(index);
        if (text == null)
            return;

        if (!CheckID(text)) {
            layer.alert("编号不符合规则");
            return;
        }

        if (text == '') {
            layer.alert('请输入从表ID不能为空，请重新输入！');
            NewMapDtl();
            return;
        }

        var en = new Entity("BP.Sys.MapDtl");
        en.No = text;
        if (en.RetrieveFromDBSources() == 1) {
            alert("已经存在:" + text);
            return;
        }
        en.FK_Node = 0;
        en.PTable = en.No;
        en.Name = "从表" + en.No;
        en.FK_MapData = frmID;
        en.H = 300;
        en.Insert();
        var data = en.DoMethodReturnString("IntMapAttrs");

        if (data.indexOf('err@') == 0) {
            layer.alert(data);
            return;
        }

        getWindowWH();
        var url = '../../Comm/En.htm?EnName=BP.WF.Template.Frm.MapDtlExt&FK_MapData=' + frmID + '&No=' + en.No;
        OpenLayuiDialog(url, '从表属性', W, 0, null, true);
        return;
    });
}

/**
 *创建身份证
 */
function AddIDCard() {
    var IDCard = [{ No: "IDCardName", Name: "姓名" }, { No: "IDCardNo", Name: '身份证号' }, { No: "IDCardAddress", Name: "地址" }];
    for (var i = 0; i < IDCard.length; i++) {
        var mapAttr = new Entity("BP.Sys.MapAttr");
        mapAttr.SetPKVal(frmID + "_" + IDCard[i].No);
        if (mapAttr.RetrieveFromDBSources() == 0) {
            mapAttr.FK_MapData = frmID;
            mapAttr.KeyOfEn = IDCard[i].No;
            mapAttr.Name = IDCard[i].Name;
            mapAttr.GroupID = groupID;
            mapAttr.UIContralType = 13; //身份证号.
            mapAttr.MyDataType = 1;
            if (IDCard[i].No == "IDCardAddress")
                mapAttr.ColSpan = 3; //单元格.
            else
                mapAttr.ColSpan = 1;
            mapAttr.LabelColSpan = 1;
            mapAttr.LGType = 0; //文本
            mapAttr.UIIsEnable = 0; //不可编辑
            mapAttr.UIIsInput = 1; //必填
            mapAttr.UIWidth = 150;
            mapAttr.UIHeight = 23;
            mapAttr.Insert(); //插入字段.
        } else {
            layer.alert("字段" + IDCard[i].No + "已存在，请变更表单中的" + mapAttr.Name + "的编号");
            return "";

        }

    }
    var url = "../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrCard&MyPK=" + frmID + "_IDCardNo";
    getWindowWH();
    OpenLayuiDialog(url, '身份证', W, 0, null, true);
    return;
}
/**
 * 创建按钮
 * @param {any} pinyin
 */
function AddBtn(pinyin) {
    layer.prompt({
        title: '请输入按钮名称:\t\n比如:保存、发送',
        formType: 0,
        value: '',
    }, function (name, index) {
        layer.close(index);
        if (name == null || name == undefined || name.trim() == "")
            return "";

        var mapAttrs = new Entities("BP.Sys.MapAttrs");
        mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
        if (mapAttrs.length >= 1) {
            layer.alert('名称：[' + name + "]已经存在.");
            AddBtn(pinyin);
            return;
        }
        var id = pinyin.makePy(name);
        var mypk = frmID + "_" + id;
        var mapAttr = new Entity("BP.Sys.MapAttr");
        mapAttr.MyPK = mypk;
        if (mapAttr.IsExits == true) {
            layer.alert('名称：[' + name + "]已经存在.");
            return "";
        }

        mapAttr.FK_MapData = frmID;
        mapAttr.KeyOfEn = id;
        mapAttr.Name = name;
        mapAttr.GroupID = groupID;
        mapAttr.UIContralType = 18; //按钮
        mapAttr.MyDataType = 1;
        mapAttr.LGType = 0;
        mapAttr.ColSpan = 0; //
        mapAttr.LabelColSpan = 1; //
        mapAttr.IsEnableInAPP = 1;
        mapAttr.Insert(); //插入字段.
        mapAttr.Retrieve();
        var url = "../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.FrmBtn&MyPK=" + mapAttr.MyPK;
        getWindowWH();
        OpenLayuiDialog(url, '按钮属性', W, 0, null, true);
        //LayuiPopRight(url, '按钮属性', W, true);

    });
}

//超链接.
function AddLink(pinyin) {
    layer.prompt({
        title: '请输入超链接名称:\t\n比如:我的连接、点击这里打开',
        formType: 0,
        value: '我的连接',
    }, function (name, index) {
        layer.close(index);
        if (name == null || name == undefined || name.trim() == "")
            return "";

        var mapAttrs = new Entities("BP.Sys.MapAttrs");
        mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
        if (mapAttrs.length >= 1) {
            layer.alert('名称：[' + name + "]已经存在.");
            AddLink(pinyin);
            return "";
        }

        //获得ID.
        var id = pinyin.makePy(name);
        var mypk = frmID + "_" + id;
        var mapAttr = new Entity("BP.Sys.MapAttr");
        mapAttr.MyPK = mypk;
        if (mapAttr.IsExits == true) {
            layer.alert('名称：[' + name + "]已经存在.");
            AddLink(pinyin);
            return "";
        }
        layer.prompt({
            title: '请输入超链地址:\t\n比如:https://gitee.com/opencc',
            formType: 0,
            value: 'https://gitee.com/opencc',
        }, function (link, index) {
            layer.close(index);
            if (link == null || link == undefined)
                return "";

            var mypk = frmID + "_" + id;
            var mapAttr = new Entity("BP.Sys.MapAttr");
            mapAttr.SetPara("Url", link.replace(/@/g, '$'));
            mapAttr.UIContralType = 9; //超链接.
            mapAttr.MyPK = mypk;
            mapAttr.FK_MapData = frmID;
            mapAttr.KeyOfEn = id;
            mapAttr.Name = name;
            mapAttr.GroupID = groupID;
            mapAttr.MyDataType = 1;
            mapAttr.LGType = 0;
            mapAttr.ColSpan = 1; //
            mapAttr.UIWidth = 150;
            mapAttr.UIHeight = 170;
            mapAttr.Tag1 = "_blank"; //打开目标.
            mapAttr.Tag2 = link; // 超链接地址.
            mapAttr.Insert(); //插入字段.
            mapAttr.Retrieve();
            var url = "../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtLink&MyPK=" + mapAttr.MyPK;
            getWindowWH();
            OpenLayuiDialog(url, '超链接属性', W, 0, null, true);
            //LayuiPopRight(url, '超链接属性', W, true);
        });
    });
}

function AddFixed() {
    var mapAttr = new Entity("BP.Sys.FrmUI.MapAttrFixed");
    mapAttr.SetPKVal(frmID + "_Fixed");
    if (mapAttr.RetrieveFromDBSources() == 0) {
        mapAttr.frmID = frmID;
        mapAttr.KeyOfEn = "Fixed";
        mapAttr.Name = "系统定位";
        mapAttr.GroupID = groupID;
        mapAttr.UIContralType = 16; //系统定位
        mapAttr.MyDataType = 1;
        mapAttr.ColSpan = 1;
        mapAttr.LabelColSpan = 1;
        mapAttr.LGType = 0; //文本
        mapAttr.UIIsEnable = 0; //不可编辑
        mapAttr.UIIsInput = 0;
        mapAttr.UIWidth = 150;
        mapAttr.UIHeight = 23;
        mapAttr.Insert(); //插入字段.
        layer.alert("创建成功");
    } else {
        layer.alert("表单" + frmID + "已经存在系统定位按钮，不能重复创建");
        return "";
    }
    var url = "../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrFixed&MyPK=" + mapAttr.MyPK;
    getWindowWH();
    OpenLayuiDialog(url, '评分控件', W, 0, null, true);
    //LayuiPopRight(url, '评分控件', W, true);

}

function AddFrame() {

    var msg = "<span>为了更好的支持应用扩展,ccform可以用iFrame的地图、流程轨迹图、轨迹表的展示.</span></br>";
    msg += "<span>在创建一个框架后，在框架属性里设置</span><br/>";
    msg += "<span>请输入框架ID,要求是字母数字下划线，非中文等特殊字符</span><br/>";
    msg += "<div class='layui-layer-content'style='padding-left:0px'><input type=text id='TB_key' value='Frame1' class='layui-layer-input' style='margin:0px 0px'/></div>"
    layer.prompt({
        title: "新建框架",
        content: msg,
        formType: 0,
        value: 'Frame1',
    }, function (val, index) {
        layer.close(index);
        if (val == null || val.trim() == "") {
            return "";
        }

        if (val == '') {
            alert('框架ID不能为空，请重新输入！');
            AddFrame();
            return;
        }

        var en = new Entity("BP.Sys.MapFrame");
        en.MyPK = frmID + "_" + val;
        if (en.IsExits() == true) {
            layer.alert("该编号[" + val + "]已经存在");
            return "";
        }

        en.FK_MapData = frmID;
        en.Name = "我的框架" + val;
        en.FrameURL = 'MapFrameDefPage.htm';
        en.H = 200;
        en.W = 200;
        en.X = 100;
        en.Y = 100;
        en.Insert();
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapFrameExt&FK_MapData=' + frmID + '&MyPK=' + en.MyPK;
        getWindowWH();
        OpenLayuiDialog(url, '框架属性', W, 0, null, true);
        //LayuiPopRight(url, '框架属性', W, true);
    });
}


//评分控件
function AddScore(pinyin) {
    layer.prompt({
        title: '请输入评分事项名称:\t\n比如:快递速度，服务水平',
        formType: 0,
        value: '评分事项',
    }, function (name, index) {
        layer.close(index);
        if (name == null || name == undefined || name.trim() == "")
            return "";
        var mapAttrs = new Entities("BP.Sys.MapAttrs");
        mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
        if (mapAttrs.length >= 1) {
            layer.alert('名称：[' + name + "]已经存在.");
            AddScore(pinyin);
            return "";
        }

        var id = pinyin.makePy(name);
        var mypk = frmID + "_" + id;
        var mapAttr = new Entity("BP.Sys.MapAttr");
        mapAttr.MyPK = mypk;
        if (mapAttr.IsExits == true) {
            layer.alert('名称：[' + name + "]已经存在.");
            AddScore(pinyin);
            return "";
        }
        layer.prompt({
            title: '请设定总分:\t\n比如:5，10',
            formType: 0,
            value: '5',
        }, function (score, index) {
            layer.close(index);
            if (score == null || score == undefined)
                return "";

            var mypk = frmID + "_" + id;
            var mapAttr = new Entity("BP.Sys.MapAttr");
            mapAttr.UIContralType = 101; //评分控件.
            mapAttr.MyPK = mypk;
            mapAttr.FK_MapData = frmID;
            mapAttr.KeyOfEn = id;
            mapAttr.Name = name;
            mapAttr.GroupID = groupID;
            mapAttr.MyDataType = 1;
            mapAttr.LGType = 0;
            mapAttr.ColSpan = 1; //
            mapAttr.UIWidth = 150;
            mapAttr.UIHeight = 170;
            mapAttr.Tag2 = score; // 总分
            mapAttr.Insert(); //插入字段.
            mapAttr.Retrieve();
            var url = "../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtScore&MyPK=" + mapAttr.MyPK;
            getWindowWH();
            OpenLayuiDialog(url, '评分控件', W, 0, null, true);
            //LayuiPopRight(url, '评分控件', W, true);
        });
    });
}
/**
 * 地图
 * @param {any} pinyin
 */
function AddMap(pinyin) {
    layer.prompt({
        title: '请输入地图名称:\t\n比如:中国地图',
        formType: 0,
        value: '地图',
    }, function (name, index) {
        layer.close(index);
        if (name == null || name == undefined || name.trim() == "")
            return "";
        var mapAttrs = new Entities("BP.Sys.MapAttrs");
        mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
        if (mapAttrs.length >= 1) {
            layer.alert('名称：[' + name + "]已经存在.");
            AddMap(pinyin);
            return "";
        }

        //获得ID.
        var id = pinyin.makePy(name);
        var mypk = frmID + "_" + id;
        var mapAttr = new Entity("BP.Sys.MapAttr");
        mapAttr.MyPK = mypk;
        if (mapAttr.IsExits == true) {
            layer.alert('名称：[' + name + "]已经存在.");
            AddMap(pinyin);
            return "";
        }

        var mypk = frmID + "_" + id;
        var mapAttr = new Entity("BP.Sys.MapAttr");
        mapAttr.UIContralType = 4; //地图.
        mapAttr.MyPK = mypk;
        mapAttr.FK_MapData = frmID;
        mapAttr.KeyOfEn = id;
        mapAttr.Name = name;
        mapAttr.MyDataType = 1;
        mapAttr.LGType = 0;
        mapAttr.ColSpan = 1; //
        mapAttr.UIWidth = 800; //宽度
        mapAttr.UIHeight = 500; //高度
        mapAttr.Insert(); //插入字段.

        var mapAttr1 = new Entity("BP.Sys.MapAttr");
        mapAttr.UIContralType = 0;
        mapAttr1.MyPK = frmID + "_AtPara";
        mapAttr1.FK_MapData = frmID;
        mapAttr1.KeyOfEn = "AtPara";
        mapAttr1.UIVisible = 0;
        mapAttr1.Name = "AtPara";
        mapAttr1.MyDataType = 1;
        mapAttr1.LGType = 0;
        mapAttr1.ColSpan = 1; //
        mapAttr1.UIWidth = 100;
        mapAttr1.UIHeight = 23;
        mapAttr1.Insert(); //插入字段

        mapAttr.Retrieve();
        var url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtMap&MyPK=' + mapAttr.MyPK;
        getWindowWH();
        OpenLayuiDialog(url, '地图控件', W, 0, null, true);
        //LayuiPopRight(url, '地图控件', W, true);
    });
}

//大块文本
function AddBigNoteHtmlText(pinyin) {
    var name = promptGener('请输入字段名', '大块提示文本');
    if (name == null || name == undefined || name.trim() == "")
        return "";

    var mapAttrs = new Entities("BP.Sys.MapAttrs");
    mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
    if (mapAttrs.length >= 1) {
        alert('名称：[' + name + "]已经存在.");
        AddBigNoteHtmlText();
        return "";
    }

    //获得ID.
    var id = pinyin.makePy(name);
    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert('名称：[' + name + "]已经存在.");
        AddBigNoteHtmlText();
        return "";
    }


    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.UIContralType = 60; //大块文本.
    mapAttr.FK_MapData = frmID;
    mapAttr.Name = name;
    mapAttr.KeyOfEn = id;
    mapAttr.MyDataType = 1;
    mapAttr.LGType = 0;
    mapAttr.ColSpan = 4; //
    mapAttr.UIWidth = 0;
    mapAttr.UIHeight = 100;
    mapAttr.Idx = 0;
    mapAttr.Insert(); //插入字段.
    var url ="./EditFExtContral/60.BigNoteHtmlText.htm?FrmID=" + frmID + "&KeyOfEn=" + id;
    getWindowWH();
    OpenLayuiDialog(url, '大块文本', W, 0, null, true);
    

}

//流程进度图.
function ExtJobSchedule() {

    var name = "流程进度图";
    var id = "JobSchedule";
    //获得ID.
    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert("已经存在，一个表单仅仅允许有一个流程进度图.");
        return "";
    }

    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.UIContralType = 50; //流程进度图.
    mapAttr.MyPK = mypk;
    mapAttr.FK_MapData = frmID;
    mapAttr.KeyOfEn = id;
    mapAttr.Name = name;
    mapAttr.GroupID = groupID;
    mapAttr.MyDataType = 1;
    mapAttr.LGType = 0;
    mapAttr.ColSpan = 4; //
    mapAttr.UIWidth = 0;
    mapAttr.UIHeight = 100;
    mapAttr.Idx = 0;
    mapAttr.Insert(); //插入字段.
    mapAttr.Retrieve();
    getWindowWH();
    var url = "../../Comm/EnOnly.htm?EnName=BP.WF.Template.ExtJobSchedule&MyPK=" + mapAttr.MyPK;
    OpenLayuiDialog(url, '进度图', W, 0, null, true);

}