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
            webOffice.ProtectDoc(1, 2, Math.random() + "");
        }

        function replace(field, text, type) {
            /// <summary>替换文本</summary>
            /// <param name="field" type="String">要设置的字段名</param>
            /// <param name="text" type="String">要设置的值</param>
            //app.Selection.Find.Execute(oldStr, true, true, false, false, false, true, 1, false, text, 2);
            var ccflow_bm_name = 'cbm_' + field;
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
            var ccflow_table_bm_prefix = 'cbd_' + dtlno + '_';
            var ccflow_formula_bm_prefix = 'cbdf_';
            var ccflow_table = getTable(ccflow_table_bm_prefix);

            if (ccflow_table != null) {
                var cellIDPrefixLen = ccflow_table_bm_prefix.length;
                var formulaPrefixLen = ccflow_formula_bm_prefix.length;

                //计算明细表的区域
                var titleRowIndex = 0;
                var arrColumns = [];
                var tbm;
                var cell;
                var re;
                var item;
                var currTableRowsCount = 0;
                var cellRegion = { begin: 0, end: 0 };

                for (var i = 1; i <= doc.Bookmarks.Count; i++) {
                    tbm = doc.Bookmarks(i);

                    if (tbm.Name.indexOf(ccflow_table_bm_prefix) == -1) {
                        continue;
                    }

                    cell = tbm.Range.Cells(1);
                    cellRegion.start = cell.Range.Start;
                    cellRegion.end = cell.Range.End;
                    re = Find(arrColumns, "colIndex", cell.ColumnIndex);
                    //tbm.Range.Text = cell.Width + "," + cell.Height;
                    if (re.length != 0) {
                        continue;   //一个单元格中只能设置一个字段的书签
                    }

                    titleRowIndex = Math.max(cell.RowIndex, titleRowIndex);

                    item = {
                        bmName: tbm.Name,
                        field: tbm.Name.substr(ccflow_table_bm_prefix.length),
                        formula: "",
                        rowIndex: cell.RowIndex,
                        colIndex: cell.ColumnIndex
                    };

                    //判断是否有汇总函数
                    for (var j = 1; j <= doc.Bookmarks.Count; j++) {
                        if (doc.Bookmarks(j).Name.indexOf(ccflow_formula_bm_prefix) == -1
                         || !(doc.Bookmarks(j).Range.Start >= cellRegion.start && doc.Bookmarks(j).Range.End <= cellRegion.end)
                         || doc.Bookmarks(j).Name == tbm.Name) {
                            continue;
                        }

                        if (doc.Bookmarks(j).Name.length > formulaPrefixLen && doc.Bookmarks(j).Name.substr(0, formulaPrefixLen) == ccflow_formula_bm_prefix) {
                            item.formula = doc.Bookmarks(j).Name.substr(formulaPrefixLen).split('_')[0];
                        }
                    }

                    arrColumns.push(item);
                }

                //计算当前表格的行数
                var islast;

                while (currTableRowsCount < ccflow_table.Rows.Count - titleRowIndex) {
                    islast = false;

                    for (var i = 0; i < arrColumns.length; i++) {
                        try {
                            var c = ccflow_table.Cell(titleRowIndex + currTableRowsCount + 1, arrColumns[i].colIndex);
                        }
                        catch (e) {
                            islast = true;
                            break;
                        }
                    }

                    if (islast) {
                        break;
                    }

                    currTableRowsCount++;
                }

                alert(currTableRowsCount);

                //补全行数
                if (currTableRowsCount < rows.length) {
                    ccflow_table.Cell(titleRowIndex + currTableRowsCount, arrColumns[0].colIndex).Select();
                    app.Selection.InsertRowsBelow(rows.length - currTableRowsCount);
                }

                //如果有汇总行，则再增加一行
                if (Find(arrColumns, "formula", "").length < arrColumns.length && currTableRowsCount < rows.length + 1) {
                    app.Selection.InsertRowsBelow(1);
                }

                //填充数据
                $.each(arrColumns, function () {
                    for (var i = 0; i < rows.length; i++) {
                        cell = getCell(rows[i], this.field);

                        if (cell == null) continue;

                        ccflow_table.Cell(titleRowIndex + i + 1, this.colIndex).Range.Text = this.field == "rowid" ? (i+1).toString() : cell.value;

                        if (cell.type == "sign") {
                            AddSignPicture(ccflow_table.Cell(titleRowIndex + i + 1, this.colIndex).Range, cell.value);
                        }
                    }

                    //增加汇总域函数
                    if (this.formula.length > 0) {
                        var colAlpha = String.fromCharCode(64 + this.colIndex);
                        var formulaString = '=' + this.formula + '(' + colAlpha + (titleRowIndex + 1) + ':' + colAlpha + (titleRowIndex + rows.length) + ')';
                        var formulaFormat = '';
                        var formulaLower = this.formula.toLocaleLowerCase();

                        if (formulaLower == 'count') {
                            formulaFormat = '总数：0';
                        }
                        else if (formulaLower == 'average') {
                            formulaFormat = '平均：0.00';
                        }

                        //此处使用Cell.Formula方法会出现问题，原因不详
                        ccflow_table.Cell(titleRowIndex + rows.length + 1, this.colIndex).Select();
                        app.Selection.MoveLeft(1, 1);
                        app.Selection.InsertFormula(formulaString, formulaFormat);
                    }
                });
            }
        }

        function Find(aItems, sField, oValue, sField1, oValue1) {
            /// <summary>查找数组中指定字段值的元素</summary>
            /// <param name="aItems" type="Array">要查找的数组</param>
            /// <param name="sField" type="String">依据字段名称</param>
            /// <param name="oValue" type="Object">字段的值</param>
            /// <param name="sField1" type="String">第2个依据字段名称</param>
            /// <param name="oValue1" type="Object">第2个字段的值</param>
            /// <return>返回集合</return>
            var re = [];

            $.each(aItems, function () {
                if (this[sField] == oValue && (!sField1 || this[sField1] == oValue1)) {
                    re.push(this);
                }
            });

            return re;
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
