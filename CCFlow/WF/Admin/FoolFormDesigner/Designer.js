//公共方法
function AjaxServiceGener(param, extUrl, callbackFunc, scope) {

    $.ajax({
        type: "GET", //使用GET或POST方法访问后台
        dataType: "text", //返回json格式的数据
        contentType: "application/json; charset=utf-8",
        url: Handler + extUrl, //要访问的后台地址
        data: param, //要发送的数据
        async: true,
        cache: false,
        complete: function () { }, //AJAX请求完成时隐藏loading提示
        error: function (XMLHttpRequest, errorThrown) {
            callback(XMLHttpRequest);
        },
        success: function (msg) { //msg为返回的数据，在这里做数据绑定
            var data = msg;
            callbackFunc(data, scope);
        }
    });
}
function ReLoad(data) {
    window.location.href = window.location.href;
}

function FrmEvent(mypk) {
    var url = 'FrmEvent.htm?FK_MapData=' + mypk;
    var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
}

function Insert(mypk, IDX) {
    var url = 'FieldTypeList.htm?DoType=AddF&MyPK=' + mypk + '&IDX=' + IDX;
    var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
    window.location.href = window.location.href;
}
function AddF() {
    
    var url = 'FieldTypeList.htm?DoType=AddF&FK_MapData=' + GetQueryString('FK_MapData');
    var h = 500;
    var w = 600;
    var l = (screen.width - w) / 2;
    var t = (screen.height - h) / 2;
    var s = 'width=' + w + ', height=' + h + ', top=' + t + ', left=' + l;
    s += ', toolbar=no, scrollbars=no, menubar=no, location=no, resizable=no';
    var b = window.showModalDialog(url, 'ass', s);
    window.location.href = window.location.href;
}
function AddField(fk_mapdata, groupID) {

    var url = 'FieldTypeList.htm?DoType=AddF&FK_MapData=' + fk_mapdata + '&GroupField=' + groupID;

    OpenEasyUiDialog(url, "eudlgframe", '增加字段', 800, 500, "icon-edit", true, null, null, null, function () {
        window.location.href = window.location.href;
    });

}
function AddTable(mypk) {
    var url = 'EditCells.htm?MyPK=' + mypk;

    OpenEasyUiDialog(url, "eudlgframe", '新建', 800, 500, "icon-edit", true, null, null, null, function () {
        window.location.href = window.location.href;
    });
}

function BatchEdit(mypk) {
    var url = 'BatchEdit.htm?FK_MapData=' + mypk;

    OpenEasyUiDialog(url, "eudlgframe", '批处理', 800, 500, "icon-property", true, null, null, null, function () {
        window.location.href = window.location.href;

    });
}

function CopyFieldFromNode(mypk) {
    var url = 'CopyFieldFromNode.htm?DoType=AddF&FK_Node=' + mypk;

    OpenEasyUiDialog(url, "eudlgframe", '复制', 800, 500, "icon-edit", true, null, null, null, function () {
        window.location.href = window.location.href;
    });
}
function GroupFieldNew() {

    var url = 'GroupField.htm?FK_MapData=' + GetQueryString('FK_MapData') + "&RefOID=0&DoType=FunList";

    OpenEasyUiDialog(url, "eudlgframe", '新建', 800, 500, "icon-property", true, null, null, null, function () {

        window.location.href = window.location.href;

    });

}
function ExpImp() {

    var url = "ExpImp.htm?FK_MapData=" + GetQueryString('FK_MapData') + "&DoType=FunList&FK_Flow=" +  GetQueryString('FK_Flow');

    OpenEasyUiDialog(url, "eudlgframe", '导入导出', 800, 500, "icon-edit", true, null, null, null, function () {
        window.location.href = window.location.href;
    });
}

function GroupField(mypk, OID) {

    // var url = 'GroupFieldEdit.htm?FK_MapData=' + mypk + "&GroupField=" + OID;
    var url = "../../Comm/En.htm?EnName=BP.Sys.GroupField&PKVal=" + OID;
    OpenEasyUiDialog(url, "eudlgframe", '分组', 800, 500, "icon-property", true, null, null, null, function () {
        window.location.href = window.location.href;
    });

}

function GroupFieldDel(mypk, refoid) {
    var url = 'GroupField.htm?RefNo=' + mypk + '&DoType=DelIt&RefOID=' + refoid;
    var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
    window.location.href = window.location.href;
}

function Edit(fk_mapdata, mypk, ftype, gf) {

    var url = 'EditF.htm?DoType=Edit&MyPK=' + mypk + '&FType=' + ftype + '&FK_MapData=' + fk_mapdata + '&GroupField=' + gf;
    var title = '';
    if (ftype == 1) {
        title = '字段String属性';
        url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrString&PKVal=' + mypk + '&s=' + Math.random();
    }

    if (ftype == 2 || ftype == 3 || ftype == 5 || ftype == 8) {
        title = '字段Num属性';
        url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&PKVal=' + mypk + '&s=' + Math.random();
    }

    if (ftype == 6 || ftype == 7) {
        title = '字段 date 属性';

        url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrDT&PKVal=' + mypk + '&s=' + Math.random();
    }

    if (ftype == 6 || ftype == 7) {
        title = '字段 datetime 属性';

        url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrDT&PKVal=' + mypk + '&s=' + Math.random();
    }

    if (ftype == 4) {
        title = '字段 boolen 属性';
        url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrBoolen&PKVal=' + mypk + '&s=' + Math.random();
    }

    OpenEasyUiDialog(url, "eudlgframe", title, 800, 500, "icon-edit", true, null, null, null, function () {
        window.location.href = window.location.href;
    });

    // OpenEasyUiDialog(url, "dd", title, 730, 500);
    return;
}
 

function EditTable(fk_mapdata, keyOfEn, mypk, sfTable, gf) {
    var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrSFTable&PKVal=' + mypk + '&s=' + Math.random();

    OpenEasyUiDialog(url, "eudlgframe", '外键' + keyOfEn + '属性', 730, 500, "icon-property", true, null, null, null, function () {
        window.location.href = window.location.href;
    });
}
// 向上移动.
function Up(fk_mapdata, mypk, idx, t) {
    var url = '?DoType=Up&FK_MapData=' + fk_mapdata + '&MyPK=' + mypk + '&ToIdx=' + idx + '&T=' + t;
    AjaxServiceGener(null, url, ReLoad, this);
}
//向下移动.
function Down(fk_mapdata, mypk, idx, t) {
    var url = '?DoType=Down&FK_MapData=' + fk_mapdata + '&MyPK=' + mypk + '&ToIdx=' + idx + '&T=' + t;
    AjaxServiceGener(null, url, ReLoad, this);
}

function GFDoUp(refoid) {
    var url = '?DoType=GFDoUp&RefOID=' + refoid;
    AjaxServiceGener(null, url, ReLoad, this);
}
function GFDoDown(refoid) {
    var url = '?DoType=GFDoDown&RefOID=' + refoid;
    AjaxServiceGener(null, url, ReLoad, this);
}

function Del(mypk, refoid) {
    if (window.confirm('您确定要删除吗？') == false)
        return;
    var url = '?DoType=Del&MyPK=' + mypk + '&RefOID=' + refoid;
    AjaxServiceGener(null, url, ReLoad, this);
}
function Esc() {
    if (event.keyCode == 27)
        window.close();
    return true;
}

function GroupBarClick(rowIdx) {

    var alt = document.getElementById('Img' + rowIdx).alert;
    var sta = 'block';
    if (alt == 'Max') {
        sta = 'block';
        alt = 'Min';
    } else {
        sta = 'none';
        alt = 'Max';
    }

    document.getElementById('Img' + rowIdx).src = './Img/' + alt + '.gif';
    document.getElementById('Img' + rowIdx).alert = alt;
    var i = 0
    for (i = 0; i <= 40; i++) {
        if (document.getElementById(rowIdx + '_' + i) == null)
            continue;
        if (sta == 'block') {
            document.getElementById(rowIdx + '_' + i).style.display = '';
        } else {
            document.getElementById(rowIdx + '_' + i).style.display = sta;
        }
    }
}

var isInser = "";

function CopyFieldFromNode(mypk) {

    var url = 'CopyFieldFromNode.htm?FK_Node=' + mypk;

    OpenEasyUiDialog(url, "eudlgframe", '复制', 730, 900, "icon-property", true, null, null, null, function () {
        window.location.href = window.location.href;
    });
}

// 子线程.
function EditFrmThread(mypk) {
    var url = '../../Comm/En.htm?EnName=BP.WF.Template.FrmTrack&PKVal=' + mypk;

    OpenEasyUiDialog(url, "eudlgframe", '子线程', 400, 700, "icon-property", true, null, null, null, function () {
        window.location.href = window.location.href;
    });
}

// 轨迹图.
function EditTrack(mypk) {
    var url = '../../Comm/En.htm?EnName=BP.WF.Template.FrmTrack&PK=' + mypk;

    OpenEasyUiDialog(url, "eudlgframe", '轨迹图', 400, 700, "icon-property", true, null, null, null, function () {
        window.location.href = window.location.href;
    });
}

/// 审核组件.
function EditFWC(mypk) {
    var url = '../../Comm/EnOnly.htm?EnName=BP.WF.Template.FrmNodeComponent&PKVal=' + mypk + '&tab=审核组件';

    OpenEasyUiDialog(url, "eudlgframe", '组件', 400, 700, "icon-property", true, null, null, null, function () {
        window.location.href = window.location.href;
    });
}
//子流程.
function EditSubFlow(mypk) {
    // var url = '../Comm/En.htm?EnName=BP.WF.Template.FrmSubFlow&PK=' + mypk
    var url = '../../Comm/EnOnly.htm?EnName=BP.WF.Template.FrmNodeComponent&PKVal=' + mypk + '&tab=父子流程组件';

    OpenEasyUiDialog(url, "eudlgframe", '组件', 400, 700, "icon-property", true, null, null, null, function () {
        window.location.href = window.location.href;
    });
}

//子线程.
function EditThread(mypk) {
    // var url = '../Comm/En.htm?EnName=BP.WF.Template.FrmSubFlow&PK=' + mypk
    var url = '../../Comm/EnOnly.htm?EnName=BP.WF.Template.FrmNodeComponent&PKVal=' + mypk + '&tab=子线程组件';

    OpenEasyUiDialog(url, "eudlgframe", '组件', 400, 700, "icon-property", true, null, null, null, function () {
        window.location.href = window.location.href;
    });

}

//流转自定义.
function EditFTC(mypk) {
    // var url = '../Comm/En.htm?EnName=BP.WF.Template.FrmSubFlow&PK=' + mypk
    var url = '../../Comm/EnOnly.htm?EnName=BP.WF.Template.FrmNodeComponent&PKVal=' + mypk + '&tab=流转自定义';

    OpenEasyUiDialog(url, "eudlgframe", '组件', 400, 700, "icon-property", true, null, null, null, function () {
        window.location.href = window.location.href;
    });

}

//轨迹组件.
function EditTrack(mypk) {
    // var url = '../Comm/En.htm?EnName=BP.WF.Template.FrmSubFlow&PK=' + mypk
    var url = '../../Comm/EnOnly.htm?EnName=BP.WF.Template.FrmNodeComponent&PKVal=' + mypk + '&tab=轨迹组件';


    OpenEasyUiDialog(url, "eudlgframe", '组件', 400, 700, "icon-property", true, null, null, null, function () {
        window.location.href = window.location.href;
    });

}

function MapDataEdit() {

    var mypk = GetQueryString('FK_MapData');

    var url = '../../Comm/En.htm?EnName=BP.WF.Template.MapFrmFool&PKVal=' + mypk;

    OpenEasyUiDialog(url, "eudlgframe", '属性', 400, 450, "icon-property", true, null, null, null, function () {
        window.location.href = window.location.href;
    });

}

function FrmNodeComponent() {

    var mypk = GetQueryString('FK_MapData');
    mypk = mypk.replace('ND', '');

    var url = '../../Comm/EnOnly.htm?EnName=BP.WF.Template.FrmNodeComponent&PKVal=' + mypk;

    OpenEasyUiDialog(url, "eudlgframe", '组件', 500, 400, "icon-property", true, null, null, null, function () {
        window.location.href = window.location.href;
    });
}

//新增从表.
function NewMapDtl(fk_mapdata) {

    var fk_mapdata = GetQueryString('FK_MapData');

    var val = prompt('请输入从表ID，要求表单唯一。', fk_mapdata + 'Dtl1');
    if (val == null) {
        return;
    }

    if (val == '') {
        alert('请输入从表ID不能为空，请重新输入！');
        NewMapDtl(fk_mapdata);
        return;
    }

    $.ajax({
        type: 'post',
        async: true,
        url: Handler + "?DoType=Designer_NewMapDtl&FK_MapData=" + fk_mapdata + "&DtlNo=" + val + "&m=" + Math.random(),
        dataType: 'html',
        success: function (data) {
            if (data.indexOf('err@') == 0) {
                alert(data);
                return;
            }

            var url = '../../Comm/En.htm?EnName=BP.WF.Template.MapDtlExt&FK_MapData=' + fk_mapdata + '&No=' + data;
            OpenEasyUiDialog(url, "eudlgframe", '从表属性', 800, 500, "icon-edit", true, null, null, null, function () {
                window.location.href = window.location.href;
            });

            return;
        }
    });
}


///编辑从表.
function EditDtl(fk_mapdata, mypk) {
    var url = '../../Comm/En.htm?EnName=BP.WF.Template.MapDtlExt&FK_MapData=' + fk_mapdata + '&No=' + mypk;

    OpenEasyUiDialog(url, "eudlgframe", '从表', 800, 500, "icon-edit", true, null, null, null, function () {
        window.location.href = window.location.href;
    });
}

function EditM2M(mypk, dtlKey) {
    var url = 'MapM2M.htm?DoType=Edit&FK_MapData=' + mypk + '&NoOfObj=' + dtlKey;
    // OpenEasyUiDialog(url, 'dtl', '属性', 800, 500, "icon-edit");
}


/// 多选.
function MapM2M() {
    var fk_mapdata = GetQueryString('FK_MapData');

    var url = 'MapM2M.htm?DoType=List&FK_MapData=' + fk_mapdata;

    OpenEasyUiDialog(url, "eudlgframe", '多选', 800, 500, "icon-edit", true, null, null, null, function () {
        window.location.href = window.location.href;
    });
}

function MapM2MM(mypk) {
    var url = 'MapM2MM.htm?FK_MapData=' + mypk;

    OpenEasyUiDialog(url, "eudlgframe", '多选', 800, 500, "icon-edit", true, null, null, null, function () {
        window.location.href = window.location.href;
    });
}

//修改附件.
function EditAth(fk_mapdata, ath) {
    var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.FrmAttachmentExt&FK_MapData=' + fk_mapdata + '&MyPK=' + fk_mapdata + "_" + ath;

    OpenEasyUiDialog(url, "eudlgframe", '附件', 800, 500, "icon-edit", true, null, null, null, function () {
        window.location.href = window.location.href;
    });
}

//新增附件.
function NewAth() {

    var fk_mapdata = GetQueryString('FK_MapData');
    var val = prompt('请输入附件ID:(要求是字母数字下划线，非中文等特殊字符.)', 'Ath1');
    if (val == null) {
        return;
    }
    if (val == '') {
        alert('附件ID不能为空，请重新输入！');
        NewAth(fk_mapdata);
        return;
    }

    $.ajax({
        type: 'post',
        async: true,
        url: Handler + "?DoType=Designer_AthNew&FK_MapData=" + fk_mapdata + "&AthNo=" + val + "&m=" + Math.random(),
        dataType: 'html',
        success: function (data) {
            if (data.indexOf('err@') == 0) {
                alert(data);
                return;
            }

            var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.FrmAttachmentExt&FK_MapData=' + fk_mapdata + '&MyPK=' + data;
            OpenEasyUiDialog(url, "eudlgframe", '附件', 800, 500, "icon-edit", true, null, null, null, function () {
                window.location.href = window.location.href;
            });

            return;
        }
    });
}

function EditFrame(fk_mapdata, myPK) {

    var url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapFrameExt&FK_MapData=' + fk_mapdata + '&MyPK=' + myPK;

    OpenEasyUiDialog(url, "eudlgframe", '框架', 800, 500, "icon-property", true, null, null, null, function () {
        window.location.href = window.location.href;
    });
}

function MapFrame() {

    var fk_mapdata = GetQueryString('FK_MapData');
    var url = 'MapFrame.htm?FK_MapData=' + GetQueryString('fk_mapdata');
    OpenEasyUiDialog(url, "eudlgframe", '框架', 800, 500, "icon-property", true, null, null, null, function () {
        window.location.href = window.location.href;
    });
}

function HidAttr() {
    var url = 'HidAttr.htm?FK_MapData=' + GetQueryString('fk_mapdata');

    OpenEasyUiDialog(url, "eudlgframe", '隐藏字段', 800, 500, "icon-property", true, null, null, null, function () {
        window.location.href = window.location.href;
    });
}
function EnableAthM(fk_MapDtl) {
    var url = '../CCForm/Ath.htm?IsBTitle=1&PKVal=0&Ath=AthMDtl&FK_MapData=' + fk_MapDtl + '&FK_FrmAttachment=' + fk_MapDtl + '_AthMDtl';

    OpenEasyUiDialog(url, "eudlgframe", '多附件', 800, 500, "icon-property", true, null, null, null, function () {
        window.location.href = window.location.href;
    });

}
function Sln(fk_mapdata) {
    var url = 'Sln.htm?IsBTitle=1&PKVal=0&Ath=AthM&FK_MapData=' + fk_mapdata + '&FK_FrmAttachment=' + fk_mapdata + '_AthM';
    WinOpen(url);
    //var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
}


////然浏览器最大化.
//function ResizeWindow() {
//    if (window.screen) {  //判断浏览器是否支持window.screen判断浏览器是否支持screen     
//        var myw = screen.availWidth;   //定义一个myw，接受到当前全屏的宽.   
//        var myh = screen.availHeight;  //定义一个myw，接受到当前全屏的高.
//        window.moveTo(0, 0);           //把window放在左上角     
//        window.resizeTo(myw, myh);     //把当前窗体的长宽跳转为myw和myh     
//    }
//}
//window.onload = ResizeWindow();