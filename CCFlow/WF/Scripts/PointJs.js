var isShowAll = false;
var webOffice = null;


var strTimeKey;

function InitOffice() {
    webOffice = document.all.WebOffice1;

    EnableMenu();
    OpenWeb();
    strTimeKey = "";
    var date = new Date();
    strTimeKey += date.getFullYear(); //年
    strTimeKey += date.getMonth() + 1; //月 月比实际月份要少1
    strTimeKey += date.getDate(); //日
    strTimeKey += date.getHours(); //HH
    strTimeKey += date.getMinutes(); //MM
    strTimeKey += date.getSeconds(); //SS

}

function SetTrack(track) {
    /// <summary>
    /// 设置留痕模式
    /// </summary>
    /// <param name="track">1-留痕模式 0-非留痕模式 x</param>
    webOffice.SetTrackRevisions(track);

}

function SetUser() {
    /// <summary>
    /// 设置当前用户
    /// </summary>
    var user = document.getElementById('TB_User').value;

    webOffice.SetCurrUserName(user);
}


function OpenWeb() {
    /// <summary>
    /// 打开服务器文件
    /// </summary>

    try {
        var type = document.getElementById('TB_FileType').value;
        var url = location.href + "&action=LoadFile";
        webOffice.LoadOriginalFile(url, type);

        SetUser();


        var isTrack = document.getElementById('TB_Track').value;
        if (isTrack == 1) {
            SetTrack(1);
        } else {
            SetTrack(0);
        }

        var isRead = document.getElementById('TB_IsReadOnly').value;

        if (isRead == 1) {
            ProtectDoc();
        } else {
            UnPortectDoc();
        }

        InitShowName();
    }
    catch (e) {
        alert(e.Message);
    }


}

function EnableMenu() {
    /// <summary>
    /// 设置按钮
    /// </summary>
    var isPrint = document.getElementById('TB_IsPrint').value;

    if (isPrint == 1) {
        webOffice.HideMenuItem(0x01 + 0x02 + 0x04);
    } else {
        webOffice.HideMenuItem(0x01 + 0x02 + 0x04 + 0x10 + 0x20);
    }


}

function ShowTrack(track) {
    /// <summary>
    /// 显示留痕
    /// </summary>
    /// <param name="track">0-隐藏 1-显示</param>

    webOffice.ShowRevisions(track);
}

function SaveTrack() {
    /// <summary>
    /// 保存修订
    /// </summary>
    // webOffice.AcceptAllRevisions();
    webOffice.SetTrackRevisions(4);
}
function ReturnTrack() {
    /// <summary>
    /// 拒绝所有修订
    /// </summary>
    var vCount = webOffice.GetRevCount();
    var strUserName;
    for (var i = 1; i <= vCount; i++) {
        strUserName = webOffice.GetRevInfo(i, 0);
        webOffice.AcceptRevision(strUserName, 1);
    }
}

function InitShowName() {
    var count = webOffice.GetRevCount();

    var showName = $("#sShowName");
    showName.empty();

    var list = "全部,";

    //GetRevInfo(i,int) int=1 获取时间  int=3 获取内容  int=0 获取名字
    for (var i = 1; i <= count; i++) {
        var strOpt = webOffice.GetRevInfo(i, 0);

        if (list.indexOf(strOpt) < 0) {
            list += strOpt + ",";
        }
    }
    var data = list.split(',');
    for (var i = 0; i < data.length; i++) {

        if (data[i] != null && data[i] != "") {
            var option = $("<option>").text(data[i]).val(data[i]);
            showName.append(option);
        }
    }
}

function SaveService() {
    /// <summary>
    /// 服务器保存
    /// </summary>
    try {

        var path = document.getElementById("TB_FilePath").value;

        webOffice.HttpInit();
        webOffice.HttpAddPostCurrFile("File", "");
        var src = location.href + "&action=SaveFile";

        webOffice.HttpPost(src);
        alert('保存成功');
    } catch (e) {
        alert(e.message);
    }
}

function ShowUserName() {
    /// <summary>
    /// 显当前用户留痕
    /// </summary>

    try {

        var user = $("#sShowName option:selected").val();
        if (user == "全部") {
            if (isShowAll) {
                webOffice.GetDocumentObject().Application.ActiveWindow.ToggleShowAllReviewers();
                isShowAll = false;
            }
        } else {
            if (!isShowAll) {
                webOffice.GetDocumentObject().Application.ActiveWindow.ToggleShowAllReviewers();
                isShowAll = true;
            } else {
                webOffice.GetDocumentObject().Application.ActiveWindow.ToggleShowAllReviewers();
                webOffice.GetDocumentObject().Application.ActiveWindow.ToggleShowAllReviewers();
            }
            webOffice.GetDocumentObject().Application.ActiveWindow.View.Reviewers(user).Visible = true;
        }
    } catch (e) {
        alert(e.message);
    }
}

function CloseDoc() {
    webOffice.CloseDoc(0);
}


function ProtectDoc() {
    webOffice.ProtectDoc(1, 2, "");
}

function UnPortectDoc() {
    webOffice.ProtectDoc(0, 1, "");
}

function InsertFile() {
    webOffice.InSertFile("我是好人的说", 0);
}


//-----作用：动态添加在WORD中下载附件的超级连接-----------------------------------//
function WebWordDownFile() {
    var no = $('#CB_Flow').combobox('getValue');
    var text = $('#CB_Flow').combobox('getText');
    try {
        if (text != null && text != '' && no != null && no != '') {
            var myRange = webOffice.GetDocumentObject().Application.Selection.Range;  //定义光标位置

            var myHyperLink = "http://" + location.host + "/WF/WorkOpt/OneWork/OneWork.htm?CurrTab=Truck&FK_Flow=" + no + "&DoType=Chart&T=" + strTimeKey;
            //var myHyperLink = "http://www.goldgrid.cn/iSignature/MakeSignGif.rar";
            //定义下载地址，也可以为其它网址，这段内容可以通过后台获取
            var myTextToDisplay = text;                           //定义提示索引信息
            var myHyperLinkName = text;          //定义显示的文字名称
            var Hyperlinks = webOffice.GetDocumentObject().Application.ActiveDocument.Hyperlinks;
            Hyperlinks.Add(myRange, myHyperLink, "", myTextToDisplay, myHyperLinkName, "4");
            //最后一个参数IE6：4;IE5：3
        }

    } catch (e) {
        alert("插入超链接出现异常..." + e.message);
    }
}

$(function () {
    LoadFLow();
});

function LoadFLow() {

    $('#CB_Flow').combobox({
        url: location.href + "&action=LoadFlow",
        valueField: 'No',
        textField: 'Name'
    });
}


function InputFiles() {
    var file = document.getElementById('TB_Image').value;
    if (file != '' && file != null) {
        webOffice.InSertFile(file, 8);
    }
}

//电子签章
function Signature(name) {


    var url = window.location.protocol + "//" + window.location.host + "/DataUser/Seal/" + name + ".png";
    //    document.all.WebOffice1.SetFieldValue("mark_1", "北京", "::ADDMARK::");
    //    webOffice.SetFieldValue("mark_1", url, "::JPG::");
    webOffice.InSertFile(url, 8);

    //AddPicture("Signature", url, 5);
}

function AddPicture(strMarkName, strBmpPath, vType) {
    //定义一个对象，用来存储ActiveDocument对象
    var obj = new Object(webOffice.GetDocumentObject());
    if (obj != null) {
        var pBookMarks;
        // VAB接口获取书签集合
        pBookMarks = obj.Bookmarks;

        var date = new Date().getFullYear() + "" + new Date().getMonth() + "" + new Date().getDay() + "" + new Date().getHours() + "" + new Date().getMinutes() + "" + new Date().getSeconds();


        webOffice.SetFieldValue("Signature" + date, "", "::ADDMARK::");

        var pBookM;
        // VAB接口获取书签strMarkName
        pBookM = pBookMarks(strMarkName);
        var pRange;
        // VAB接口获取书签strMarkName的Range对象
        pRange = pBookM.Range;
        var pRangeInlines;
        // VAB接口获取书签strMarkName的Range对象的InlineShapes对象
        pRangeInlines = pRange.InlineShapes;
        var pRangeInline;
        // VAB接口通过InlineShapes对象向文档中插入图片
        pRangeInline = pRangeInlines.AddPicture(strBmpPath);
        //设置图片的样式，5为浮动在文字上面
        pRangeInline.ConvertToShape().WrapFormat.TYPE = vType;
        delete obj;
    }
}


 