//import './fonts/SourceHanSans-Normal.ttf'
function InitPara(frmID, oid) {
    frmID = GetQueryString("FrmID");
    if (frmID == null)
        frmID = GetQueryString("FK_MapData");
    if (frmID == null)
        frmID = "Frm_ZhangJieBiaoDAN";

    oid = GetQueryString("OID");
    if (oid == null)
        oid = GetQueryString("WorkID");
    if (oid == null)
        oid = GetQueryString("PKVal");
    if (oid == null)
        oid = 100;
}
//获得从表的url.
function Ele_Dtl(frmID, isRadeonly) {
    isRadeonly = isRadeonly == null || isRadeonly == undefined ? 0 : 1;
    var frmDtl = new Entity("BP.Sys.MapDtl", frmID);
    var src = "";
    var href = GetHrefUrl();
    var urlParam = href.substring(href.indexOf('?') + 1, href.length);
    urlParam = urlParam.replace('&DoType=', '&DoTypeDel=xx');

    var ensName = frmDtl.No;
    if (ensName == undefined) {
        layer.alert('系统错误,请找管理员联系');
        return;
    }

    var currentURL = GetHrefUrl();

    var baseUrl = "./";
    if (currentURL.indexOf("AdminFrm.htm") != -1)
        baseUrl = "../../CCForm/";
    if (currentURL.indexOf("MyBill.htm") != -1 || currentURL.indexOf("MyDict.htm") != -1)
        baseUrl = "../CCForm/";
    if (currentURL.indexOf("FrmGener.htm") != -1 || currentURL.indexOf("FrmDBVer.htm") != -1 || currentURL.indexOf("DtlFrm.htm") != -1)
        baseUrl = "./";

    //表格模式
    if (frmDtl.ListShowModel == "0")
        src = baseUrl + "Dtl2017.htm?1=1";
    if (frmDtl.ListShowModel == "1")
        src = baseUrl + "DtlCard.htm?1=1";
    if (frmDtl.ListShowModel == "2") {
        if (frmDtl.UrlDtl == null || frmDtl.UrlDtl == undefined || frmDtl.UrlDtl == "")
            return "从表" + frmDtl.Name + "没有设置URL,请在" + frmDtl.FK_MapData + "_Self.js中解析";
        src = basePath + "/" + frmDtl.UrlDtl;
        if (src.indexOf("?") == -1)
            src += "?1=1";
    }
    var oid = GetQueryString("WorkID");
    src += "&EnsName=" + frmDtl.No + "&RefPKVal=" + oid + "&FK_MapData=" + frmID + "&IsReadonly=" + isRadeonly+"&" + urlParam + "&Version=1&FrmType=0";
    return src;

}
/**
 * 打开数据版本
 * @param {any} attrKey
 * @param {any} attrName
 */
var wf_node = null;
function FrmDBVerAndRemark(attrKey, attrName, frmID, isFrm, isDtl) {
    if (attrKey == null || attrKey == undefined || attrKey == "") {
        layer.alert("请选择章节");
        return;
    }

    var oid = GetQueryString("OID");
    if (oid == null)
        oid = GetQueryString("WorkID");
    if (oid == null)
        oid = GetQueryString("RefPKVal");

    var isEnable = 0;
    var isReadonly = parent.location.href.indexOf("IsReadonly=1") != -1 ? 1 : 0;
     wf_node = new Entity("BP.WF.Template.NodeExt",GetQueryString("FK_Node"));
    if (wf_node.FrmDBRemarkEnable == 1 && isReadonly == 0)
        isEnable = 1;
    if (isFrm == true) {
        wf_node.FormType = 11;
        wf_node.NodeFrmID = attrKey;
        FrmDBRemark(wf_node.FrmDBRemarkEnable);
        return;
    }
    var url = "";
    if(isDtl==true)
        url = "FrmDBVerAndRemark.htm?FrmID=" + frmID + "&RFrmID=" + frmID + "&RefPKVal=" + oid + "&Field=" + attrKey + "&FieldType=1&IsEnable=" + isEnable;
    else
        url = "FrmDBVerAndRemark.htm?FrmID=" + frmID + "&RFrmID=" + frmID + "&RefPKVal=" + oid + "&Field=" + attrKey + "&FieldType=0&IsEnable=" + isEnable;
    OpenLayuiDialog(url, "审阅:" + attrName, 600);
}

/**
 * 获得第一个字段，并把他显示出来.
 * @param {any} groupFields
 * @param {any} attrs
 */
function GenerFistKey(key, groupFields, attrs) {
    //设置第一个选择的值.
    if (key != null)
        return;

    for (var i = 0; i < groupFields.length; i++) {
        var gf = groupFields[i];
        if (gf.CtrlID.length > 2)
            continue;


        for (var idx = 0; idx < attrs.length; idx++) {
            var attr = attrs[idx];

            if (gf.OID == attr.GroupID) {
                key = attrs[idx].KeyOfEn;
                break;
            }
        }
    }
   
}

function getHeight(doc) {
    var body = doc.body,
        html = doc.documentElement;

    var height = Math.max(body.scrollHeight, body.offsetHeight,
        html.clientHeight, html.scrollHeight, html.offsetHeight);
    return height;
}
//替换所有的回车换行 
function TransferString(content) {
    var string = content;
    try {
        string = string.replace(/\r\n/g, "\n")
        string = string.replace(/\n/g, "\n");
    } catch (e) {
        alert(e.message);
    }
    return string;
}

//替换所有的回车换行 
function TransbrString(content) {
    var string = content;
    try {
        string = string.replace(/\r\n/g, "<br/>")
        string = string.replace(/\n/g, "<br/>");
    } catch (e) {
        alert(e.message);
    }
    return string;
}


function CheckGroupFieldStr(groupID, item) {
    let rate = $(item).find('.rate');
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    handler.AddPara("FrmID", frmID);
    handler.AddPara("OID", oid);
    handler.AddPara("GroupField", groupID);
    var data = handler.DoMethodReturnJSON("ChapterFrm_CheckGroupFieldStar");
    console.log(data)
    // 0=不加 ， 1=加*
    if (data == "1") {
        $(rate).show()
    }
}

var db = [];

// 改变存储值
function changeDBval(vals, attrKey) {
    db.forEach(item => {
        if (item.attrKey == attrKey) {
            item.vals = vals;
        }
    })
}

//获取旧数据
function getOldVal(attrKey) {
     let  obj = '';
     db.forEach(item => {
        if (item.attrKey == attrKey) {
            obj = {
              "attrKey":item.attrKey,
               "vals":item.vals,
            }
        }
     })
    return obj
}



function toPDF() {
    let doc = new jsPDF('portrait', 'pt', 'a4');
    doc.addFont('NotoSansCJKtc-Regular.ttf', 'NotoSansCJKtc', 'normal');
    doc.setFont('NotoSansCJKtc');
    let response = $('#pdf_container')[0];
    doc.addHTML(
        response,{ 
        pagesplit: true,
        scale: 2,
    }, function () {
            var Name = 'report_pdf_' + new Date().getTime();
            doc.save(Name+ '.pdf');
            doc.output('dataurlnewwindow', Name + ".pdf");
    });
    $("#header").show();
}

function fromHTML() {
    var element = $("#pdf_container");    
    var w = element.width();    
    var h = element.height();    
    var offsetTop = element.offset().top;   
    var offsetLeft = element.offset().left;  
    var canvas = document.createElement("canvas");
    var abs = 0;
    var win_i = $(window).width();    
    var win_o = window.innerWidth;    
    if (win_o > win_i) {
        abs = (win_o - win_i) / 2;    
    }
    canvas.width = w * 2;    
    canvas.height = h * 2;
    var context = canvas.getContext("2d");
    context.scale(2, 2);
    context.translate(-offsetLeft - abs, -offsetTop);
    var element = $("#pdf_container")[0];
    html2canvas(element,{ async: false,}).then(function (canvas) {
            var contentWidth = canvas.width;
            var contentHeight = canvas.height;
            var pageHeight = contentWidth / 592.28 * 841.89;
            var leftHeight = contentHeight;
            var position = 0;
            var imgWidth = 595.28;
            var imgHeight = 592.28 / contentWidth * contentHeight;
            console.log(canvas);
            var pageData = canvas.toDataURL('image/jpeg', 1.0);
            var pdf = new jsPDF('', 'pt', 'a4');
            if (leftHeight < pageHeight) {
                pdf.addImage(pageData, 'JPEG', 20, 0, imgWidth, imgHeight);
            } else {    // 分页
                while (leftHeight > 0) {
                    pdf.addImage(pageData, 'JPEG', 0, position, imgWidth, imgHeight)
                    leftHeight -= pageHeight;
                    position -= 841.89;
                    //避免添加空白页
                    if (leftHeight > 0) {
                        pdf.addPage();
                    }
                }
            }
            var Name = 'report_pdf_' + new Date().getTime();
            pdf.save(Name + '.pdf');
            window.parent.layer.closeAll()
           // pdf.output('dataurlnewwindow', Name + ".pdf");
        });
}
let isBottomSort = false;
let Direction = '';
let iframe = '';
let istop = null;
function tabChange(sort) {
    Direction = sort
    TabViews();
}

//$("#toIframe").on("mousewheel", function (event, delta) {
//    console.log(event)
//    console.log(delta)
//    event.preventDefault();
//    if (delta > 0) {
//        tabChange("up")
//    } else {
//        tabChange("down")
//    }
//    return false;
//});
const scrollEvent = function(){
    //滚动时的函数执行代码
    var viewH = $(this).height(),//可见高度
        contentH = iframe.contentDocument.body.scrollHeight,//内容高度
        scrollTop = $(this).scrollTop();//滚动高度
    const valheight = contentH - viewH;
    if (scrollTop == 0 ) {
        Direction = "up"
        //alert("到顶了")
        layer.msg('到顶了');
       
    }
    console.log("scrollTop", scrollTop)
    console.log("valheight", valheight)
    if (scrollTop >= valheight ) {
        Direction = "down"
       // alert("到底了")
        layer.msg('到底了');
    }

   // isBottomSort =false;
}

const TniscrollEvent = function (view) {
    //滚动时的函数执行代码
    var viewH = $(view).height(),//可见高度
        contentH = view.document.body.scrollHeight,//内容高度
        scrollTop = view.scrollY;//滚动高度

    const valheight = scrollTop + viewH;
    
    if (contentH <= valheight) {
        Direction = "down"
        //alert("到底了")
        if (isBottomSort == false) {
            isBottomSort = true;
        } else {
            TabViews()
        }
    }
    if (valheight == viewH) {
        Direction = "up"
       // alert("到顶了")
        if (istop == null) {
            istop = true;
        } else {
            TabViews()
        }
    }
    // isBottomSort =false;
}
//ArcScorll('#mytextarea_ifr')
function ArcScorll(elem) {
    iframe = document.querySelector(elem);
    iframe.onload = function () {
      //  alert("Local iframe is now loaded.");
        document.querySelector(elem).contentWindow.addEventListener('scroll', scrollEvent,true)
    };
}



let Indx = '';
let tObj = {
    data: '',
    elem: {},
    state:''
}
var conut = 0;
var page=''
//切换页面方法
function TabViews() {
    const list = document.querySelectorAll('.layui-tree  .layui-tree-set');
    const activeElem = document.querySelector('.layui-tree .tree-txt-active');
    const acId = $(activeElem).data("id");
    list.forEach((item, index) => {
        if (item.dataset.id == acId) {
            Indx = index;
        }
    })
    
    tObj.elem = list.item(Indx + 1);
    if (Direction == "down") {
        if (Indx > list.length) {
            layer.msg('已经是最后一章节了');
            return;
        }
        if (tObj.elem[0] !== undefined) {
            if (tObj.elem[0].children.length == 2) {
                Indx = Indx + 2;
                tObj.elem = $(list.item(Indx));
            }
        } else {
            if (tObj.elem.children.length == 2) {
                Indx = Indx + 2;
                tObj.elem = $(list.item(Indx));
            }
        }
        
    } else {
        if (Indx <= 0) {
            layer.msg('已经是第一章节了');
            return;
        }
        tObj.elem = $(list.item(Indx - 1));
        if (tObj.elem[0] !== undefined) {
            if (tObj.elem[0].children.length == 2) {
                Indx = Indx - 2;
                tObj.elem = $(list.item(Indx));
            }
        } else {
            if (tObj.elem[0].children.length == 2) {
                Indx = Indx - 2;
                tObj.elem = $(list.item(Indx));
            }
        }
        
    }
    const Nid = $(tObj.elem).data("id");
    groupFields.forEach(item => item.OID == Nid ? tObj.data = item : '');
    attrs.forEach(item => item.KeyOfEn == Nid ? tObj.data = item : '');
    treeClick(tObj);
    var ele = $("#Pnode" + Nid).get(0)
    $(ele).addClass("tree-txt-active")
    $(tObj.elem).addClass("tree-txt-active")
    document.querySelector('#toIframe').contentWindow.removeEventListener('scroll', scrollEvent, true);

    setTimeout(function () {

        if (isBindMouseWheel)
            return;

        $("#mytextarea_ifr").contents().find("#tinymce").off('mousewheel').on("mousewheel", function (event, delta) {
            isBindMouseWheel = true;
            event.preventDefault();
            if (delta > 0) {
                tabChange("up")
            } else {
                tabChange("down")
            }
            return false;
        });
    }, 1000);

}
var isBindMouseWheel = false;