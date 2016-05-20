<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="RptDoc.aspx.cs"
    Inherits="CCFlow.WF.Rpt.RptDoc" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/jBox/Skins/Blue/jbox.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../Scripts/jBox/jquery.jBox-2.3.min.js" type="text/javascript"></script>
    <script src="../Comm/JScript.js" type="text/javascript"></script>
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
                webOffice.SetTrackRevisions(0);
                replaceParams();
                isOpen = true;
            } else {
                $.jBox.alert('OpenDoc 打开文档失败', '异常');
            }
        }

        function closeDoc() {
            webOffice.SetCurrUserName("");
            webOffice.closeDoc(0);
        }

        //保存到服务器
        function saveOffice() {

            pageLoadding('正在保存...');

            try {
                if (isOpen) {
                    webOffice.HttpInit();
                    webOffice.HttpAddPostCurrFile("File", "");

                    var src = location.href + "&action=SaveFile&filename=" + $('#<%=fileName.ClientID %>').val();
                    webOffice.HttpPost(src);
                } else {
                    $.jBox.alert('请打开公文!', '提示');
                }
            } catch (e) {
                $.jBox.alert("异常\r\nsaveOffice1 Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }

            loaddingOut('保存完成...');
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
                app.Selection.Delete(1, charCount + 2);

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

        function replaceParams() {
            /// <summary>替换所有属性</summary>
            var params = $.parseJSON('<%=ReplaceParams %>');
            var dtls = $.parseJSON('<%=ReplaceDtls %>');

            //替换主表数据
            $.each(params, function () {
                replace(this.key, this.value.replace("\\\\", "\\").replace("\'", "'"), this.type);
            });

            //替换明细表数据
            $.each(dtls, function () {
                replaceDtl(this.dtlno, this.dtl);
            });

            //接受修订
            doc.AcceptAllRevisions();
        }

        function replace(field, text, type) {
            /// <summary>替换文本</summary>
            /// <param name="field" type="String">要设置的字段名</param>
            /// <param name="text" type="String">要设置的值</param>
            //app.Selection.Find.Execute(oldStr, true, true, false, false, false, true, 1, false, text, 2);
            var ccflow_bm_name = 'ccflow_bm_main_' + field;
            if (doc.Bookmarks.Exists(ccflow_bm_name)) {
                var bm = doc.Bookmarks(ccflow_bm_name);
                var bmRange = bm.Range;
                var bmRangeStart = bmRange.Start;

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

        function replaceDtl(dtlno, rows) {
            /// <summary>填充明细表数据</summary>
            /// <param name="dtlno" type="String">明细表No</param>
            /// <param name="rows" type="Array">明细表数据行集合</param>
            var ccflow_table_bm_prefix = 'ccflow_bm_dtl_' + dtlno + '_';
            var ccflow_formula_bm_prefix = 'ccflow_bm_dtl_formula_';
            var ccflow_table = getTable(ccflow_table_bm_prefix);

            if (ccflow_table != null) {

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
    <form id="form1" runat="server" style="height: 100%">
    <div data-options="region:'north',border:false,noheader:true" style="background-color: #E0ECFF;
        line-height: 30px; height: auto; padding: 2px">
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
