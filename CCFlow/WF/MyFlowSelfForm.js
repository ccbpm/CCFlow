//定义全局的变量
var pageData = {};//全局的参数变量
var flowData = {}; // 流程数据
var isReadonly = false;//表单方案是只读时的变化
if (typeof webUser == "undefined" || webUser == null)
    webUser = new WebUser();
var UserICon = getConfigByKey("UserICon", '../DataUser/Siganture/'); //获取签名图片的地址
var UserIConExt = getConfigByKey("UserIConExt", '.jpg');  //签名图片的默认后缀
//加载页面
$(function () {
    UserICon = UserICon.replace("@basePath", basePath);

    //增加css样式
    $('head').append('<link href="../DataUser/Style/GloVarsCSS.css" rel="stylesheet" type="text/css" />');

    //初始化表单参数
    initPageData();

    //初始化表单数据
    GenerWorkNode();
})


/**
 * 初始化表单数据
 */
function initPageData() {
    pageData = {
        FK_Flow: GetQueryString("FK_Flow"),
        FK_Node: GetQueryString("FK_Node"),
        FID: GetQueryString("FID") == null ? 0 : GetQueryString("FID"),
        WorkID: GetQueryString("WorkID"),
        OID: pageData.WorkID,
        Paras: GetQueryString("Paras"),
        IsReadonly: 0,
        IsStartFlow: GetQueryString("IsStartFlow"),
        IsMobile: IsMobile()//是不是移动端
    }
}

var flowData;
var workNodeData = {};


/**
 * 初始化数据
 */
function GenerWorkNode() {

    var href = GetHrefUrl();
    var urlParam = href.substring(href.indexOf('?') + 1, href.length);
    urlParam = urlParam.replace('&DoType=', '&DoTypeDel=xx');
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddUrlData(urlParam);
    var data = handler.DoMethodReturnString("MyFlowSelfForm_Init");

    if (data.indexOf('err@') == 0) {
        layer.alert(data);
        return;
    }
    try {

        flowData = JSON.parse(data);
        workNodeData = flowData;

    } catch (err) {
        alert("GenerWorkNode转换JSON失败:" + data);
        return;
    }
    //设置标题.
    document.title = flowData.WF_Node[0].Name;
    $('#CCForm').html('');

    var wf_node = workNodeData.WF_Node[0];

    var url = wf_node.FormUrl;
    if (url == "")
        url = "../DataUser/DefaultSelfFormUrl.htm";
    else
        if (url.indexOf("http") == -1)
            url = basePath + url;

    if (url.indexOf('?') == -1) {
        url = url + "?1=2";
    }
    url += "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&FK_Node=" + pageData.FK_Node;

    var _html = "<iframe ID='SelfForm' src='" + url + "' frameborder=0  style='width:100%;' leftMargin='0' topMargin='0'/>";

    var compoents = workNodeData.WF_FrmNodeComponent;
    //增加审核分组
    for (var i = 0; i < compoents.length; i++) {
        var component = compoents[i];
        if (component.FWCSta != 0) {
            _html += "<div class='layui-row'>"
            _html += "<div class='layui-col-xs12'>";
            _html += "<div id='WorkCheck'></div>";
            _html += "</div>";
            _html += "</div>";
            continue;
        }
    }

    $('#CCForm').html("").append(_html);

    //显示提示信息
    ShowWorkReturnTip();

    //调整页面宽度
    //var w = flowData.Sys_MapData[0].FrmW;//设置的页面宽度
    //$('#ContentDiv').width(w);
    //$('#ContentDiv').css("margin-left", "auto").css("margin-right", "auto");

}

/**
 * 增加退回
 */
function ShowWorkReturnTip() {

    //显示退回消息
    if (flowData.AlertMsg.length != 0) {
        var _html = "";
        $.each(flowData.AlertMsg, function (i, item) {
            if (item.Title == "退回信息")
                _html += "<div style='padding: 10px 0px 0px 10px;line-height: 24px;color:red'>";
            else
                _html += "<div style='padding: 10px 0px 0px 10px;line-height: 24px;'>";
            _html += (i + 1) + "." + item.Title + "<br/>";
            _html += item.Msg;
            _html += "</div>";
        });
        var h = window.innerHeight - 240;
        //退回消息
        layer.open({
            type: 1,
            skin: '', //加上边框
            area: ['420px', h + 'px'], //宽高
            content: _html
        });
    }
}

//. 保存嵌入式表单. add 2015-01-22 for GaoLing.
function SaveSelfFrom() {

    // 不支持火狐浏览器。
    var frm = document.getElementById('SelfForm');
    if (frm == null) {
        layer.alert('系统错误,没有找到SelfForm的ID.');
    }

    //审核组件
    if ($("#WorkCheck_Doc").length == 1) {
        //保存审核信息
        SaveWorkCheck();
    }

    //执行保存.
    var frmSrc = document.getElementById('SelfForm').src;
    
    //嵌入的表单和当前页面在同一个域
    if (frmSrc.indexOf(basePath + "/") != -1)
        return frm.contentWindow.Save();

    //出现跨域问题
    /**
     嵌入的页面需要增加的方法
     window.addEventListener('message', function (e) {
           console.log(e.data);
           var data=e.data;
           if(data.Save){
               Save();
           }
       }, false);
     */
    //return frm.contentWindow.postMessage({ Save:"Save" }, "*");
}


function To(url) {
    window.name = "dialogPage"; window.open(url, "dialogPage")
}

function WinOpen(url, winName) {
    var newWindow = window.open(url, winName, 'width=700,height=400,top=100,left=300,scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes;');
    newWindow.focus();
    return;
}


function Do(warning, url) {
    if (window.confirm(warning) == false)
        return;
    SetHref(url);
}


//然浏览器最大化.
function ResizeWindow() {
    if (window.screen) {  //判断浏览器是否支持window.screen判断浏览器是否支持screen     
        var myw = screen.availWidth;   //定义一个myw，接受到当前全屏的宽     
        var myh = screen.availHeight;  //定义一个myw，接受到当前全屏的高     
        window.moveTo(0, 0);           //把window放在左上角     
        window.resizeTo(myw, myh);     //把当前窗体的长宽跳转为myw和myh     
    }
}


//保存
function Save() {
    SaveSelfFrom();
}



function setIframeHeight(iframe) {
    if (iframe) {
        var iframeWin = iframe.contentWindow || iframe.contentDocument.parentWindow;
        if (iframeWin.document.body) {
            iframe.height = iframeWin.document.documentElement.scrollHeight || iframeWin.document.body.scrollHeight;
        }
    }
};

window.onload = function () {
    setIframeHeight(document.getElementById('SelfForm'));
};






