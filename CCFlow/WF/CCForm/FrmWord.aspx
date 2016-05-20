<%@ Page Title="" Language="C#" AutoEventWireup="true"
    CodeBehind="FrmWord.aspx.cs" Inherits="CCFlow.WF.CCForm.FrmWord" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../Scripts/jBox/jquery.jBox-2.3.min.js" type="text/javascript"></script>
    <link href="../Scripts/jBox/Skins/Blue/jbox.css" rel="stylesheet" type="text/css" />
    <script src="../Comm/JScript.js" type="text/javascript"></script>
    <style type="text/css">
        #mapdata, #divMenu
        {
            display: block;
            float: left;
        }
        #mapdata
        {
            border-right: 1px solid #ccc;
            padding-right: 10px;
        }
        #divMenu
        {
            padding-left: 10px;
        }
    </style>
    <script language="javascript" type="text/javascript">
        var isShowAll = false;
        var webOffice = null;
        var strTimeKey;
        var isOpen = false;
        var isInfo = false;
        var marksType = "doc,docx";
        var doc = null;
        var win = null;
        var app = null;
        var fields = $.parseJSON('<%=ReplaceFields %>');
        var dtlNos = $.parseJSON('<%=ReplaceDtlNos %>');

        $(function () {
            InitOffice();

            $('body').attr('onunload', 'closeDoc()');
        });

        function InitOffice() {
            /// <summary>
            /// 初始化Office
            /// </summary>
            webOffice = document.all.WebOffice1;

            if ($('#<%=fileName.ClientID %>').val() != "") {
                OpenWeb("1");
            }

            EnableMenu();
        }

        function EnableMenu() {
            /// <summary>
            /// 设置按钮
            /// </summary>
            webOffice.HideMenuItem(0x01 + 0x02 + 0x04 + 0x10 + 0x20);
        }

        //打开本地文件
        function OpenFile() {
            pageLoadding('正在打开...');

            try {
                if (readOnly()) {
                    return false;
                }

                OpenDoc("open", "doc");
            } catch (e) {
                $.jBox.alert("异常\r\nOpenFile Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }

            loaddingOut('打开完成');
            return false;
        }

        function OpenWeb(loadtype) {
            pageLoadding('正在打开模板...');

            try {
                var type = $("#<%=fileType.ClientID %>").val();
                var fileName = $('#<%=fileName.ClientID %>').val();
                var url = location.href + "&action=LoadFile&LoadType=" + loadtype + "&fileName=" + fileName;
                OpenDoc(url, type);
            } catch (e) {
                $.jBox.alert("异常\r\nOpenWeb Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }

            loaddingOut('打开完成。');
        }

        //打开文件
        function OpenDoc(url, type) {
            var openType = webOffice.LoadOriginalFile(url, type);
            
            app = webOffice.GetDocumentObject().Application;
            doc = app.ActiveDocument;
            win = app.ActiveWindow;

            if (openType > 0) {
                if ("<%=IsMarks %>" == "True")
                    SetTrack(1);
                else
                    SetTrack(0);

                isOpen = true;

                if ("<%=IsFirst %>" == "True") {
                    replaceParams();
                }
                else {
                    loadInfos();
                }
            } else {
                $.jBox.alert('OpenDoc 打开文档失败', '异常');
            }
        }

        function SetTrack(track) {
            /// <summary>
            /// 设置留痕,显示所有的留痕用户,是否只读文档
            /// </summary>
            if ("<%=ReadOnly %>" == "True") {
                webOffice.ProtectDoc(1, 2, "");
            }
            else {
                webOffice.ProtectDoc(0, 1, "");
            }

            webOffice.SetTrackRevisions(track);

            if (track == 1) {
                webOffice.ShowRevisions(0); //隐藏修订
            }

            SetUsers();

            var types = $('#<%=fileType.ClientID %>').val();
            if (marksType.indexOf(types) >= 0) {
                ShowUserName();
            }
        }

        //设置当前操作用户
        function SetUsers() {
            try {
                webOffice.SetCurrUserName("<%=UserName %>");

                InitShowName();

            } catch (e) {
                $.jBox.alert("异常\r\nSetUsers Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
        }

        //显示指定用户的留痕
        function ShowUserName() {
            /// <summary>
            /// 显当前用户留痕
            /// </summary>

            try {

                var user = $("#marks option:selected").val();

                if (user == "全部" || user == undefined) {
                    webOffice.ShowRevisions(1);
                    if (isShowAll) {
                        win.ToggleShowAllReviewers();
                        isShowAll = false;
                    } else {
                        win.ToggleShowAllReviewers();
                    }

                } else {
                    //                    if (!isShowAll) {
                    //                        webOffice.GetDocumentObject().Application.ActiveWindow.ToggleShowAllReviewers();
                    //                        webOffice.GetDocumentObject().Application.ActiveWindow.ToggleShowAllReviewers();
                    //                        isShowAll = true;
                    //                    }
                    isShowAll = true;
                    webOffice.ShowRevisions(1); //显示修订
                    win.View.Reviewers(user).Visible = true;
                }
            } catch (e) {
                $.jBox.alert("异常\r\nShowUserName Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
        }

        //加载所留痕用户
        function InitShowName() {
            try {
                var count = webOffice.GetRevCount();

                var showName = $("#marks");
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

            } catch (e) {
                $.jBox.alert("异常\r\InitShowName Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
        }

        function closeDoc() {
            webOffice.SetCurrUserName("");
            webOffice.closeDoc(0);
        }

        function OpenTempLate() {
            if (readOnly()) {
                return false;
            }

            LoadTemplate('word', '公文模板', OpenWeb);
            return false;
        }

        //加载模板
        function LoadTemplate(type, title, callback) {
            try {
                $.jBox("iframe:/WF/WebOffice/TempLate.aspx?LoadType=" + type, {
                    title: title,
                    width: 800,
                    height: 350,
                    buttons: { '确定': 'ok' },
                    submit: function (v, h, f) {
                        var row = h[0].firstChild.contentWindow.getSelected();

                        if (row == null) {
                            $.jBox.info('请选一个模板');
                            return false;
                        } else {
                            pageLoadding('打开中...');
                            $("#<%=fileName.ClientID %>").val(row.Name);
                            $("#<%=fileType.ClientID %>").val(row.Type);
                            callback();
                            loaddingOut('打开完成...');

                            return true;
                        }
                    }
                });
            } catch (e) {
                $.jBox.alert("异常\r\nLoadTemplate Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
        }

        //保存到服务器
        function saveOffice() {
            //            alert();
            //            return;

            if (readOnly()) {
                return false;
            }

            pageLoadding('正在保存...');

            try {
                if (isOpen) {
                    if ("<%=IsCheck %>" == "True") {
                        if (isInfo) {
                            isInfo = false;
                            doc.Content.InsertAfter("\n<%=NodeInfo %>");
                        }
                    }

                    var fieldJson = getPostJsonString();
                    var dtlsJson = getDtlsPostJsonString();

                    //此处为了解决表单域提交默认最大1146[测试出的，可能是非准确数值]字符的问题，将大于1000字符的分段发送
                    var len = dtlsJson.length;
                    var arrs = new Array();
                    var spanLen = 1000;
                    var startIdx = 0;

                    webOffice.HttpInit();
                    webOffice.HttpAddPostCurrFile("File", "");
                    //明细表
                    for (var i = 1; ; i++) {
                        startIdx = (i - 1) * spanLen;

                        if (startIdx > len) break;

                        arrs.push(dtlsJson.substr(startIdx, Math.min(len - startIdx, spanLen)));
                    }

                    for (var i = 0; i < arrs.length; i++) {
                        webOffice.HttpAddPostString("dtls" + i, arrs[i]);
                    }

                    len = fieldJson.length;
                    arrs = new Array();
                    //主表
                    for (var i = 1; ; i++) {
                        startIdx = (i - 1) * spanLen;

                        if (startIdx > len) break;

                        arrs.push(fieldJson.substr(startIdx, Math.min(len - startIdx, spanLen)));
                    }

                    for (var i = 0; i < arrs.length; i++) {
                        webOffice.HttpAddPostString("field" + i, arrs[i]);
                    }

                    var src = location.href + "&action=SaveFile&filename=" + $('#<%=fileName.ClientID %>').val();
                    webOffice.HttpPost(src);
                } else {
                    $.jBox.alert('请打开公文!', '提示');
                }
            } catch (e) {
                $.jBox.alert("异常\r\nsaveOffice1 Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }

            loaddingOut('保存完成...');
            var types = $('#<%=fileType.ClientID %>').val();

            try {
                if (marksType.indexOf(types) >= 0) {
                    InitShowName();

                    if (isShowAll) {
                        win.ToggleShowAllReviewers();
                        //win.ToggleShowAllReviewers();
                        isShowAll = false;
                    } else {
                        win.ToggleShowAllReviewers();
                    }

                    ShowUserName();
                }
            } catch (e) {
                $.jBox.alert("异常\r\nsaveOffice2 Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }

            return false;
        }

        //套红头文件
        function TaoHong() {
            try {
                var mark = AddBooks();
                var type = $('#<%=fileType.ClientID %>').val();
                var name = $('#<%=fileName.ClientID %>').val();

                if (type == "png" || type == "jpg" || type == "bmp") {
                    webOffice.SetFieldValue(mark, window.location.protocol + "//" + window.location.host + "/DataUser/OfficeOverTemplate/" + name, "::JPG::");
                } else {
                    webOffice.SetFieldValue(mark, window.location.protocol + "//" + window.location.host + "/DataUser/OfficeOverTemplate/" + name, "::FILE::");
                }
            } catch (e) {
                $.jBox.alert("异常\r\nTaoHong Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
        }

        //接受修订
        function acceptOffice() {
            try {
                if (readOnly()) {
                    return false;
                }

                webOffice.SetTrackRevisions(4);
            } catch (e) {
                $.jBox.alert("异常\r\nacceptOffice Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }

            return false;
        }

        //拒绝修订
        function refuseOffice() {
            try {
                if (readOnly()) {
                    return false;
                }

                var vCount = webOffice.GetRevCount();
                var strUserName;

                for (var i = 1; i <= vCount; i++) {
                    strUserName = webOffice.GetRevInfo(i, 0);
                    webOffice.AcceptRevision(strUserName, 1);
                }
            } catch (e) {
                $.jBox.alert("异常\r\nrefuseOffice Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }

            return false;
        }

        //加载红头文件模板
        function overOffice() {
            if (readOnly()) {
                return false;
            }

            if (isOpen) {
                LoadTemplate('over', '套红模板', TaoHong);
            } else {
                $.jBox.alert('请打开公文!', '提示');
            }

            return false;
        }

        //打印公文
        function printOffice() {
            try {
                if (isOpen) {
                    webOffice.PrintDoc(1);
                } else {
                    $.jBox.alert('请打开公文!', '提示');
                }
            } catch (e) {
                $.jBox.alert("异常\r\nprintOffice Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }

            return false;
        }

        //加载所有的公文印章
        function sealOffice() {
            if (readOnly()) {
                return false;
            }

            if (isOpen) {
                LoadTemplate('seal', '公文盖章', seal);
            } else {
                $.jBox.alert('请打开公文!', '提示');
            }

            return false;
        }

        //盖章
        function seal() {
            try {
                var name = $('#<%=fileName.ClientID %>').val();
                var type = $('#<%=fileType.ClientID %>').val();
                var url = window.location.protocol + "//" + window.location.host + "/DataUser/OfficeSeal/" + name;

                var mark = AddBooks();
                //webOffice.InSertFile(url, 8);
                AddPicture(mark, url, 5);
            } catch (e) {
                $.jBox.alert("异常\r\nseal Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
        }

        //添加书签
        function AddBooks() {
            var d = new Date();
            var date = d.getFullYear() + "" + d.getMonth() + "" + d.getDay() + "" + d.getHours() + "" + d.getMinutes() + "" + d.getSeconds();
            var strMarkName = "mark_" + date;
            var obj = new Object(webOffice.GetDocumentObject());

            if (obj != null) {
                var pBookM;
                var pBookMarks;
                // VAB接口获取书签集合
                pBookMarks = obj.Bookmarks;
                try {
                    pBookM = pBookMarks(strMarkName);
                    return pBookM.Name;
                } catch (e) {
                    webOffice.SetFieldValue(strMarkName, "", "::ADDMARK::");
                }
            }

            return strMarkName;
        }

        //通过VBA 来插入图片
        function AddPicture(strMarkName, strBmpPath, vType) {
            //定义一个对象，用来存储ActiveDocument对象
            var obj = new Object(webOffice.GetDocumentObject());
            if (obj != null) {
                var pBookMarks;
                // VAB接口获取书签集合
                pBookMarks = obj.Bookmarks;
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
                pRangeInline = pRangeInlines.AddPicture(strBmpPath, 128);
                //设置图片的样式，5为浮动在文字上面
                pRangeInline.ConvertToShape().WrapFormat.TYPE = vType;
                delete obj;
            }
        }

        function AddSignPicture(range, picName) {
            /// <summary>在指定标签内插入签名图片</summary>
            /// <param name="range" type="String">要插入图片的区域</param>
            /// <param name="picName" type="String">签名人名称</param>
            var url = window.location.protocol + "//" + window.location.host + "/DataUser/Siganture/" + picName + ".JPG";
            var charCount = range.Characters.Count;
            range.Select();

            try {
                //先将签名图片下载到本地，解决直接插入网络图片速度过慢的问题
                var path = webOffice.GetTempFilePath();
                webOffice.DownLoadFile(url, path, "", "");

                app.Selection.MoveLeft(1, 1);
                app.Selection.InlineShapes.AddPicture(path, false, true);
                app.Selection.MoveRight(1, 1);
                app.Selection.Delete(1, charCount);

                //删除下载的签名图片
                webOffice.DelLocalFile(path);
            } catch (e) {

            }
        }

        function DownLoad() {
            try {
                if (isOpen) {
                    webOffice.ShowDialog(84);
                } else {
                    $.jBox.alert('请打开公文!', '提示');
                }
            } catch (e) {
                $.jBox.alert("异常\r\nDownLoad Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }

            return false;
        }

        //文档只读提示
        function readOnly() {
            if ("<%=ReadOnly %>" == "True") {
                $.jBox.alert('文档只读不能进行此操作!', '提示');
                return true;
            }
        }

        /*以下是为程序设计而定义的辅助对象*/
        function JWDtl(sDtlNo, oWebTable) {
            /// <summary>JS端Word表格</summary>
            /// <param name="sDtlNo" Type="String">编号，对应服务器端的明细表编号</param>
            /// <param name="oWebTable" Type="Object">从服务器获取的存储数据的Table对象</param>
            this.dtlNo = sDtlNo;
            this.webTable = oWebTable;
            this.wordTable = null;
            this.columns = new Array();  //JWColumn对象数组

            if (typeof JWDtl._initialized == "undefined") {

                JWDtl.prototype.getFieldValue = function (rowIdx, colIdx) {
                    /// <summary>获取指定行列单元格的值</summary>
                    /// <param name="rowIdx" Type="Int">行号，从1开始</param>
                    /// <param name="colIdx" Type="Int">列号，从1开始</param>

                    if (this.wordTable == null) return null;

                    var text = this.wordTable.Cell(rowIdx, colIdx).Range.Text;
                    return text.substr(0, text.length - 2);
                };

                JWDtl.prototype.getValuesJsonString = function () {

                    if (this.wordTable == null) return null;

                    var rows = this.wordTable.Rows.Count;
                    var json = '{"dtlno":"' + this.dtlNo + '","dtl":[';
                    var rowstr = '';
                    var isLastRow = true;
                    var cellValue = '';

                    for (var i = 2; i <= rows; i++) {
                        json += '{"rowid":"' + (i - 1) + '","cells":[';

                        rowstr = '';
                        isLastRow = true;
                        cellValue = '';

                        for (var ci = 0; ci < this.columns.length; ci++) {
                            if (this.columns[ci].field == 'rowid') continue;

                            cellValue = this.getFieldValue(i, this.columns[ci].columnIdx);

                            if (isLastRow && cellValue.length > 0) {
                                isLastRow = false;
                            }

                            rowstr += '{"key":"' + this.columns[ci].field + '","value":"' + cellValue + '"},';
                        }

                        if (!isLastRow) {
                            json += rowstr;
                        }
                        else {
                            json = removeLastComma(json) + ']},';
                            break;
                        }

                        json = removeLastComma(json) + ']},';
                    }

                    json = removeLastComma(json) + ']}';
                    return json;
                }

                JWDtl._initialized = true;
            }
        }

        function JWColumn(sField, sBookmarkName, iColumnIdx) {
            /// <summary>Word中明细表列字段信息</summary>
            /// <param name="sField" Type="String">字段名称</param>
            /// <param name="sBookmarkName" Type="String">标识字段的书签名称</param>
            /// <param name="iColumnIdx" Type="Int">字段所在表的列序号</param>
            this.field = sField;
            this.bookmarkName = sBookmarkName;
            this.columnIdx = iColumnIdx;
        }

        function JWField(sField, sBookmarkName, sValue) {
            /// <summary>Word中主表字段信息</summary>
            /// <param name="sField" Type="String">字段名称</param>
            /// <param name="sBookmarkName" Type="String">标识字段的书签名称</param>
            /// <param name="sValue" Type="String">字段值</param>
            this.field = sField;
            this.bookmarkName = sBookmarkName;
            this.value = sValue;
        }

        function getPostJsonString() {
            //获取并生成主表填充字段的新值JSON字符串
            var mainJson = '[';
            var fieldValue = '';

            $.each(jwMains, function () {
                mainJson += '{"key":"' + this.field + '","value":"';
                fieldValue = doc.Bookmarks(this.bookmarkName).Range.Text;

                if (fieldValue != null) {
                    if (fieldValue.length > 1 && fieldValue.charAt(0) == ' ') {
                        fieldValue = fieldValue.substr(1);
                    }

                    if (fieldValue.length > 0 && fieldValue.charAt(fieldValue.length - 1) == ' ') {
                        fieldValue = fieldValue.substr(0, fieldValue.length - 1);
                    }

                    fieldValue = fieldValue.replace('"', '\"').replace('\\', '\\\\');
                }

                mainJson += fieldValue + '"},';
            });

            mainJson = mainJson.substr(0, mainJson.length - 1) + ']';
            return mainJson;
        }

        function getDtlsPostJsonString() {
            var dtlsJson = '[';

            $.each(jwDtls, function () {
                dtlsJson += this.getValuesJsonString() + ',';
            });

            dtlsJson = removeLastComma(dtlsJson) + ']';
            return dtlsJson;
        }

        function removeLastComma(str) {
            /// <summary>去除指定字符串最后的逗号</summary>
            /// <param name="str" Type="String">字符串</param>
            if (str.charAt(str.length - 1) == ',') {
                return str.substr(0, str.length - 1);
            }

            return str;
        }

        function loadInfos() {

            var ccflow_main_bm_prefix = 'ccflow_bm_main_';
            var ccflow_dtl_bm_prefix = 'ccflow_bm_dtl_';            
            var ccflow_dtl_bm_prefix_len = ccflow_dtl_bm_prefix.length;
            var jwfield;
            var jwdtl;
            var bmName;
            var dtlBmNamePrefix;
            var dtlBmNamePrefix_len;
            var dtlWordTable;
            var firstCell;
            var colField;
            var isExist;

            //获取主表填充字段集合
            for (var i = 0; i < fields.length; i++) {
                if (doc.Bookmarks.Exists(ccflow_main_bm_prefix + fields[i])) {
                    jwfield = new JWField(fields[i], ccflow_main_bm_prefix + fields[i], '');
                    jwMains.push(jwfield);
                }
            }

            for (var i = 1; i <= doc.Bookmarks.Count; i++) {
                bmName = doc.Bookmarks(i).Name;

                if (bmName.length <= ccflow_dtl_bm_prefix_len || bmName.substr(0, ccflow_dtl_bm_prefix_len) != ccflow_dtl_bm_prefix) {
                    continue;
                }

                for (var d = 0; d < dtlNos.length;d++) {
                    //判断是否已经保存过这个dtl在jwDtls中
                    isExist = false;

                    for (var jw = 0; jw < jwDtls.length; jw++) {
                        if (jwDtls[jw].dtlNo == dtlNos[d]) {
                            isExist = true;
                            break;
                        }
                    }

                    if (isExist) {
                        continue;
                    }

                    dtlBmNamePrefix = ccflow_dtl_bm_prefix + dtlNos[d] + '_';
                    dtlBmNamePrefix_len = dtlBmNamePrefix.length;

                    if (bmName.length <= dtlBmNamePrefix_len || bmName.substr(0, dtlBmNamePrefix_len) != dtlBmNamePrefix) {
                        continue;
                    }

                    dtlWordTable = getTable(dtlBmNamePrefix);

                    if (dtlWordTable == null) {
                        continue;
                    }

                    jwdtl = new JWDtl(dtlNos[d], $.parseJSON('[]'));
                    jwdtl.wordTable = dtlWordTable;
                    jwDtls.push(jwdtl);

                    for (var j = 1; j <= dtlWordTable.Columns.Count; j++) {
                        firstCell = dtlWordTable.Rows(1).Cells(j);

                        if (firstCell.Range.Bookmarks.Count == 0) continue;

                        for (var b = 1; b <= firstCell.Range.Bookmarks.Count; b++) {
                            //此处firstCell.Range.Bookmarks获取的是当前表格中的所有Bookmark，与设想的只获取当前单元格中的Bookmark不一致，因此要对单元格范围与书签范围进行对比排除非当前单元格内的书签
                            if ((firstCell.Range.Bookmarks(b).Range.Start >= firstCell.Range.Start && firstCell.Range.Bookmarks(b).Range.End < firstCell.Range.End) == false) {
                                continue;
                            }

                            if (firstCell.Range.Bookmarks(b).Name.length > dtlBmNamePrefix_len && firstCell.Range.Bookmarks(b).Name.substr(0, dtlBmNamePrefix_len) == dtlBmNamePrefix) {
                                colField = firstCell.Range.Bookmarks(b).Name.substr(dtlBmNamePrefix_len);

                                //记录填充列信息
                                jwdtl.columns.push(new JWColumn(colField, firstCell.Range.Bookmarks(b).Name, j));
                            }
                        }
                    }
                }
            }
        }

        var jwMains = new Array();
        var jwDtls = new Array();

        function replaceParams() {
            /// <summary>替换所有属性</summary>
            var params = $.parseJSON('<%=ReplaceParams %>');
            var dtls = $.parseJSON('<%=ReplaceDtls %>');

            //替换主表数据
            $.each(params, function () {
                replace(this.key, this.value.replace("\\\\", "\\").replace("\'", "'"));
            });

            //替换明细表数据
            $.each(dtls, function () {
                var jwdtl = new JWDtl(this.dtlno, this.dtl);
                jwDtls.push(jwdtl);
                replaceDtl(this.dtlno, this.dtl, jwdtl);
            });

            //接受修订
            doc.AcceptAllRevisions();
        }

        function replace(field, text, type) {
            /// <summary>替换文本</summary>
            /// <param name="field" type="String">要设置的字段名</param>
            /// <param name="text" type="String">要设置的值</param>
            /// <param name="type" type="String">值的类型，string/sign</param>
            //app.Selection.Find.Execute(oldStr, true, true, false, false, false, true, 1, false, text, 2);
            var ccflow_bm_name = 'ccflow_bm_main_' + field;
            if (doc.Bookmarks.Exists(ccflow_bm_name)) {
                var bm = doc.Bookmarks(ccflow_bm_name);
                var bmRange = bm.Range;
                var bmRangeStart = bmRange.Start;
                var jwfield = new JWField(field, ccflow_bm_name, text);

                jwMains.push(jwfield);
                bm.Select();

                if (text == null || text == '') {
                    text = '     '; //如果为空，默认为5个空格，可以在填写时避免不处于书签中
                }

                bmRange.Text = text;
                app.Selection.SetRange(app.Selection.Start, app.Selection.Start + text.length);
                app.Selection.Bookmarks.Add(ccflow_bm_name);

                if (type == "sign") {
                    AddSignPicture(doc.Bookmarks(ccflow_bm_name).Range, text);
                }
            }
        }

        function replaceDtl(dtlno, rows, jwdtl) {
            /// <summary>填充明细表数据</summary>
            /// <param name="dtlno" type="String">明细表No</param>
            /// <param name="rows" type="Array">明细表数据行集合</param>
            /// <param name="jwdtl" type="JWDtl">明细表数据行集合</param>
            var ccflow_table_bm_prefix = 'ccflow_bm_dtl_' + dtlno + '_';
            var ccflow_formula_bm_prefix = 'ccflow_bm_dtl_formula_';
            var ccflow_table = getTable(ccflow_table_bm_prefix);

            if (ccflow_table != null) {
                //存储在内存中，在保存的时候好用
                jwdtl.wordTable = ccflow_table;

                //补全行数
                if (ccflow_table.Rows.Count < rows.length + 1) {
                    ccflow_table.Rows(ccflow_table.Rows.Count).Select();
                    app.Selection.InsertRowsBelow(rows.length + 1 - ccflow_table.Rows.Count);
                }

                var cellIDPrefixLen = ccflow_table_bm_prefix.length;
                var formulaPrefixLen = ccflow_formula_bm_prefix.length;

                //填充数据
                for (var j = 1; j <= ccflow_table.Columns.Count; j++) {
                    var firstCell = ccflow_table.Rows(1).Cells(j);

                    if (firstCell.Range.Bookmarks.Count == 0) continue;
                    var validBookmark;
                    var colField;
                    var formula;
                    var cell;

                    for (var b = 1; b <= firstCell.Range.Bookmarks.Count; b++) {
                        //此处firstCell.Range.Bookmarks获取的是当前表格中的所有Bookmark，与设想的只获取当前单元格中的Bookmark不一致，因此要对单元格范围与书签范围进行对比排除非当前单元格内的书签
                        if ((firstCell.Range.Bookmarks(b).Range.Start >= firstCell.Range.Start && firstCell.Range.Bookmarks(b).Range.End < firstCell.Range.End) == false) {
                            continue;
                        }

                        //判断该列是否有汇总
                        if (firstCell.Range.Bookmarks(b).Name.length > formulaPrefixLen && firstCell.Range.Bookmarks(b).Name.substr(0, formulaPrefixLen) == ccflow_formula_bm_prefix) {
                            formula = firstCell.Range.Bookmarks(b).Name.substr(formulaPrefixLen).split('_')[0];
                        }
                        else {
                            formula = '';
                        }

                        if (firstCell.Range.Bookmarks(b).Name.length > cellIDPrefixLen && firstCell.Range.Bookmarks(b).Name.substr(0, cellIDPrefixLen) == ccflow_table_bm_prefix) {
                            colField = firstCell.Range.Bookmarks(b).Name.substr(cellIDPrefixLen);

                            //记录填充列信息
                            jwdtl.columns.push(new JWColumn(colField, firstCell.Range.Bookmarks(b).Name, j));

                            for (var i = 0; i < rows.length; i++) {
                                cell = getCell(rows[i], colField);

                                if (cell == null) continue;

                                ccflow_table.Rows(i + 2).Cells(j).Range.Text = cell.value;

                                if (cell.type == "sign") {
                                    AddSignPicture(ccflow_table.Rows(i + 2).Cells(j).Range, cell.value);
                                }
                            }
                        }

                        //增加汇总域函数
                        if (formula.length > 0) {
                            var colAlpha = String.fromCharCode(64 + j);
                            var formulaString = '=' + formula + '(' + colAlpha + '2:' + colAlpha + (rows.length + 1) + ')';
                            var formulaFormat = '';
                            var formulaLower = formula.toLocaleLowerCase();

                            if (formulaLower == 'count') {
                                formulaFormat = '总数：0';
                            }
                            else if (formulaLower == 'average') {
                                formulaFormat = '平均：0.00';
                            }

                            //判断是否已经增加了汇总行
                            if (ccflow_table.Rows.Count < rows.length + 2) {
                                ccflow_table.Rows(ccflow_table.Rows.Count).Select();
                                app.Selection.InsertRowsBelow(1);
                            }

                            //此处使用Cell.Formula方法会出现问题，原因不详
                            ccflow_table.Cell(ccflow_table.Rows.Count, j).Select();
                            app.Selection.MoveLeft(1, 1);
                            app.Selection.InsertFormula(formulaString, formulaFormat);
                        }
                    }
                }
            }
        }

        function getCell(row, colName) {
            if (colName == 'rowid') {
                return { key: 'rowid', value: row.rowid, type: 'string' };
            }

            for (var i = 0; i < row.cells.length; i++) {
                if (row.cells[i].key == colName) {
                    return row.cells[i];
                }
            }

            return null;
        }

        function getTable(ccflow_table_bm_prefix) {
            var bms = doc.Bookmarks;
            var intable_bm;

            for (var i = 1; i <= bms.Count; i++) {
                if (bms(i).Name.length <= ccflow_table_bm_prefix.length)
                    continue;

                if (bms(i).Name.substr(0, ccflow_table_bm_prefix.length) == ccflow_table_bm_prefix) {
                    intable_bm = bms(i);
                    break;
                }
            }

            if (intable_bm == null) return null;

            intable_bm.Select();
            //此处未考虑书签未在表格内的情况，逻辑中未有此种情况
            return app.Selection.Tables(1);
        }

        function pageLoadding(msg) {
            $.jBox.tip(msg, 'loading');
        }

        function loaddingOut(msg) {
            $.jBox.tip(msg, 'success');
        }
    </script>
</head>
<body class="easyui-layout">
    <form id="form1" runat="server">
        <div data-options="region:'north',border:false,noheader:true" style="background-color: #E0ECFF;
        line-height: 30px; height: 30px; padding: 2px">
            <div id="divMenu" runat="server">
            </div>
        </div>
        <div style="display: none">
            <asp:TextBox ID="fileName" runat="server" Text=""></asp:TextBox>
            <asp:TextBox ID="fileType" runat="server" Text=""></asp:TextBox>
        </div>
        <div data-options="region:'center',border:false,noheader:true">
            <script src="../Scripts/LoadWebOffice.js" type="text/javascript"></script>
        </div>
     </form>
</body>
</html>
