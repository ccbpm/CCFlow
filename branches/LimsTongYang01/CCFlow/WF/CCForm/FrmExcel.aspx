<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmExcel.aspx.cs" Inherits="CCFlow.WF.CCForm.FrmExcel" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/jBox/Skins/Blue/jbox.css" rel="stylesheet" type="text/css" />
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
        var marksType = "xls,xlsx";
        var sheetPrefix = '';
        var wb = null;
        var win = null;
        var app = null;
        var sheet = null;
        var sheets = [];   //记录当前表单所属的sheet集合
        var fields = $.parseJSON('<%=ReplaceFields %>');
        var dtlNos = $.parseJSON('<%=ReplaceDtlNos %>');
        var fk_mapdatas = $.parseJSON('<%=FK_MapDatas %>');
        var fk_mapdata = fk_mapdatas.length > 0 ? fk_mapdatas[0] : null;
        var toolbarslns = $.parseJSON('<%=ToolbarSlns %>');
        var toolbarsln = toolbarslns.length > 0 ? toolbarslns[0] : null;
        var isFirsts = $.parseJSON('<%=IsFirsts %>');
        var params = $.parseJSON('<%=ReplaceParams %>'); //主表数据。
        var dtls = $.parseJSON('<%=ReplaceDtls %>'); //从表数据.
        var fieldCtrls = $.parseJSON('<%=ReplaceFieldCtrls %>');
        var jwMains = [];
        var jwDtls = [];
        var exceptions = ["sum", "count", "counta", "average", "max", "min"]; //暂时写这些常用的函数，但excel中支持的更多

        //excel常量        
        var xlSheetVeryHidden = 2;
        var xlSheetHidden = 0;
        var xlSheetVisible = -1;

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

                OpenDoc("open", "xls");
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

        //打开文件excel
        function OpenDoc(url, type) {

            //打开原始的对象. type =excle .
            var openType = webOffice.LoadOriginalFile(url, type);

            wb = webOffice.GetDocumentObject(); // 当前活动的excel文件.
            app = wb.Application;
            win = app.ActiveWindow;  // 当前的工作窗口.

            if (openType > 0) {  // 成功打开.

                //                if ("<%=IsMarks %>" == "True")
                //                    SetTrack(1); // 如果留痕. 
                //                else
                //                    SetTrack(0);

                isOpen = true; //文件打开.

                $.each(fk_mapdatas, function () {
                    if ('<%=IsEdit %>' == 'True') {
                        if (getObjectFromArray(this.Name, isFirsts)) {
                            replaceParams(this.Name); // 第一次加载,就填充读取的数据.
                        }
                        else {
                            loadInfos(this.Name);
                        }
                    }
                    else {
                        webOffice.ProtectDoc(1, 2, String(Math.random()));
                    }

                    AddBtn("Btn_" + this.Name, this.Text, "showSheets('" + this.Name + "')", 'icon-sheet', '#mapdata');
                });

                $.parser.parse('#mapdata');

                //此处需要放于以上逻辑的后面，要先进行填充，而后再隐藏非当前excel表单的sheet
                showSheets(fk_mapdata.Name);

            } else {
                $.jBox.alert('打开文档失败', '异常');
            }
        }

        function SetTrack(track) {
            /// <summary>
            /// 设置留痕,显示所有的留痕用户,是否只读文档.
            /// </summary>
            if ("<%= !toolbar.OfficeSaveEnable %>" == "True") {
                webOffice.ProtectDoc(1, 2, "");
            }
            else {
                webOffice.ProtectDoc(0, 1, "");
            }

            webOffice.SetTrackRevisions(track);

            SetUsers();

            var types = $('#<%=fileType.ClientID %>').val();
            if (marksType.indexOf(types) >= 0) {
                ShowUserName();
            }
        }

        //设置当前操作用户.
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
                        isShowAll = false;
                    }

                    //wb.KeepChangeHistory = true;
                    //wb.HighlightChangesOptions(2, 'Everyone');
                    //wb.ListChangesOnNewSheet = false;
                    //wb.HighlightChangesOnScreen = true;
                } else {
                    isShowAll = true;
                    webOffice.ShowRevisions(1);
                    //wb.KeepChangeHistory = true;
                    //wb.HighlightChangesOptions(2, user);
                    //wb.ListChangesOnNewSheet = false;
                    //wb.HighlightChangesOnScreen = true;
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
            if (app && wb) {
                try {
                    webOffice.SetCurrUserName("");
                    webOffice.closeDoc(0);
                }
                catch (e) {
                }
            }
        }

        function OpenTempLate() {
            if (readOnly()) {
                return false;
            }

            LoadTemplate('excel', '公文模板', OpenWeb);
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
        function SaveOffice() {
            if (!isOpen) {
                $.jBox.alert('请打开公文!', '提示');
                return false;
            }

            if ('<%=IsEdit %>' != 'True') {
                return false;
            }

            var fieldJson,
                dtlsJson,
                len,
                arrs1 = {},
                arrs2 = {},
                spanLen = 1000,
                startIdx = 0;

            for (var fk = 0; fk < fk_mapdatas.length; fk++) {
                fieldJson = getPostJsonString(fk_mapdatas[fk].Name);

                if (fieldJson == null) return false;

                dtlsJson = getDtlsPostJsonString(fk_mapdatas[fk].Name);

                //此处为了解决表单域提交默认最大1146[测试出的，可能是非准确数值]字符的问题，将大于1000字符的分段发送
                len = dtlsJson.length;
                arrs1[fk_mapdatas[fk].Name] = [];
                startIdx = 0;

                //明细表
                for (var i = 1; ; i++) {
                    startIdx = (i - 1) * spanLen;

                    if (startIdx > len)
                        break;

                    arrs1[fk_mapdatas[fk].Name].push(dtlsJson.substr(startIdx, Math.min(len - startIdx, spanLen)));
                }

                len = fieldJson.length;
                arrs2[fk_mapdatas[fk].Name] = [];
                //主表
                for (var i = 1; ; i++) {
                    startIdx = (i - 1) * spanLen;

                    if (startIdx > len)
                        break;

                    arrs2[fk_mapdatas[fk].Name].push(fieldJson.substr(startIdx, Math.min(len - startIdx, spanLen)));
                }
            }

            pageLoadding('正在保存...');

            try {
                webOffice.HttpInit();
                webOffice.HttpAddPostCurrFile("File", "");

                for (var fk = 0; fk < fk_mapdatas.length; fk++) {
                    for (var i = 0; i < arrs1[fk_mapdatas[fk].Name].length; i++) {
                        webOffice.HttpAddPostString("dtls_" + fk_mapdatas[fk].Name + i, arrs1[fk_mapdatas[fk].Name][i]);
                    }

                    //主表
                    for (var i = 0; i < arrs2[fk_mapdatas[fk].Name].length; i++) {
                        webOffice.HttpAddPostString("field_" + fk_mapdatas[fk].Name + i, arrs2[fk_mapdatas[fk].Name][i]);
                    }
                }

                var src = location.href + "&action=SaveFile&filename=" + $('#<%=fileName.ClientID %>').val();
                webOffice.HttpPost(src);
            } catch (e) {
                $.jBox.alert("异常\r\nsaveOffice1 Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }

            loaddingOut('保存完成...');
            var types = $('#<%=fileType.ClientID %>').val();

            try {
                if (marksType.indexOf(types) >= 0) {
                    InitShowName();

                    if (isShowAll) {
                        isShowAll = false;
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
                var type = $('#<%=fileType.ClientID %>').val();
                var name = $('#<%=fileName.ClientID %>').val();

                if (type == "png" || type == "jpg" || type == "bmp") {
                    AddPicture(window.location.protocol + "//" + window.location.host + "/DataUser/OfficeOverTemplate/" + name, app.Selection, 300, 300);
                } else {
                    AddFile(window.location.protocol + "//" + window.location.host + "/DataUser/OfficeOverTemplate/" + name, app.Selection);
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

                AddPicture(window.location.protocol + "//" + window.location.host + "/DataUser/OfficeSeal/" + name, app.Selection, 160, 160);
            } catch (e) {
                $.jBox.alert("异常\r\nseal Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
        }

        //插入图片
        function AddPicture(sPicUrl, range, picWidth, picHeight) {
            try {
                //先将图片下载到本地，解决直接插入网络图片速度过慢的问题
                var path = webOffice.GetTempFilePath();
                webOffice.DownLoadFile(sPicUrl, path, "", "");

                sheet.Shapes.AddPicture(path, 0, -1, range.Left, range.Top, picWidth, picHeight);
                //删除下载的图片
                webOffice.DelLocalFile(path);
            } catch (e) {
                $.jBox.alert("异常\r\nAddPicture Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
        }

        function AddFile(sFileUrl, range) {

            try {
                //先将文件下载到本地
                var fileExt = sFileUrl.substr(sFileUrl.lastIndexOf('.'));
                var path = webOffice.GetTempFilePath();
                path = path.substr(0, path.lastIndexOf('.')) + fileExt;
                webOffice.DownLoadFile(sFileUrl, path, "", "");

                sheet.OLEObjects.Add(path, false, false).Select();
                app.Selection.ShapeRange.Left = range.Left;
                app.Selection.ShapeRange.Top = range.Top;
                //删除下载的文件
                webOffice.DelLocalFile(path);
            } catch (e) {
                $.jBox.alert("异常\r\nAddFile Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
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
            if ("<%= !toolbar.OfficeSaveEnable %>" == "True") {
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
            this.excelRange = null; //Excel中该明细表对应的Range
            this.excelName = '';    //Excel中该明细表的名称
            this.sheetName = null;    //该明细表命名区域所在的sheet
            this.columns = new Array();  //JWColumn对象数组
            this.header = {};    //表头信息

            if (typeof JWDtl._initialized == "undefined") {

                JWDtl.prototype.getCellValue = function (rowIdx, colIdx) {
                    /// <summary>获取指定行列单元格的值</summary>
                    /// <param name="rowIdx" Type="Int">行号，从1开始</param>
                    /// <param name="colIdx" Type="Int">列号，从1开始</param>

                    if (this.excelRange == null) return null;

                    var text = this.excelRange.Cells(rowIdx, colIdx).Text;

                    if (text == null) {
                        text = '';
                    }

                    return text;
                };

                JWDtl.prototype.getValuesJsonString = function () {

                    if (this.excelRange == null) return null;

                    var rows = this.excelRange.Rows.Count;
                    var json = '{"dtlno":"' + this.dtlNo + '","dtl":[';
                    var rowstr = '';
                    var isLastRow = true;
                    var cellValue = '';

                    for (var i = this.header.InBeginRow; i <= this.header.InBeginRow + rows - 1; i++) {
                        json += '{"rowid":"' + (i - this.header.InBeginRow + 1) + '","cells":[';

                        rowstr = '';
                        isLastRow = true;
                        cellValue = '';

                        //此处获取明细表的数据时，还未考虑如果列的顺序变化的情况，如果列变化了，则获取数据会错误，待以后完善
                        for (var ci = 0; ci < this.columns.length; ci++) {
                            if (this.columns[ci].field == 'rowid') continue;

                            cellValue = this.getCellValue(i, this.columns[ci].columnIdx);

                            if (isLastRow && cellValue.length > 0) {
                                isLastRow = false;
                            }

                            rowstr += '{"key":"' + this.columns[ci].field + '","value":"' + encodeURIComponent(cellValue) + '"},';
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

        function JWColumn(sField, sFormula, sSheetName, sRangeName, iColumnIdx) {
            /// <summary>Excel中明细表列字段信息</summary>
            /// <param name="sField" Type="String">字段名称</param>
            /// <param name="sFormula" Type="String">汇总</param>
            /// <param name="sSheetName" Type="String">所属sheet的名称</param>
            /// <param name="sRangeName" Type="String">标识字段的区域名称</param>
            /// <param name="iColumnIdx" Type="Int">字段所在表的列序号</param>
            this.field = sField;
            this.formula = sFormula;
            this.sheetName = sSheetName;
            this.rangeName = sRangeName;
            this.columnIdx = iColumnIdx;
        }

        function JWField(sField, sValue, sSheetName, sName) {
            /// <summary>Excel中主表字段信息</summary>
            /// <param name="sField" Type="String">字段名称</param>
            /// <param name="sValue" Type="String">字段值</param>
            /// <param name="sSheetName" Type="String">所属的sheet</param>
            /// <param name="sName" Type="String">命名区域的名称</param>
            this.field = sField;
            this.value = sValue;
            this.sheetName = sSheetName;
            this.name = sName;
        }

        function getPostJsonString(fk_md) {
            /// <summary>获取并生成主表填充字段的新值JSON字符串</summary>
            /// <param name="fk_md" Type="String">excel表单编号</param>
            var mainJson = '[';
            var fieldValue = '';
            var fieldNamePrefix = '';
            var fieldCell;
            var currSheets = getSheets(fk_md);
            var fieldCtrl;
            var errorInfo = '';

            $.each(getObjectFromArray(fk_md, jwMains), function () {
                mainJson += '{"key":"' + this.field + '","value":"';
                fieldCell = getRangeName(this.field, fieldNamePrefix, currSheets);

                if (fieldCell != null) {
                    fieldValue = (fieldCell.RefersToRange.Text == null ? '' : fieldCell.RefersToRange.Text).replace('"', '\"').replace('\\', '\\\\');
                }
                else {
                    fieldValue = '';
                }

                //检测不可编辑的字段，是否被编辑了，被编辑过则不允许提交
                fieldCtrl = getObjectFromArray(this.field, getObjectFromArray(fk_md, fieldCtrls));

                if (fieldCtrl) {
                    if (fieldCtrl.UIIsEnable == false && fieldValue != fieldCtrl.OldValue) {
                        errorInfo += '“' + fieldCtrl.Name + '”不允许进行修改，原始值为：' + (fieldCtrl.OldValue.length == 0 ? 'NULL(空)' : fieldCtrl.OldValue) + '。<br />';
                    }
                    else if (fieldCtrl.UIIsEnable == true && fieldCtrl.IsNotNull == true && fieldValue == '') {
                        errorInfo += '“' + fieldCtrl.Name + '”不允许为空，请填写！<br />';
                    }
                }

                mainJson += encodeURIComponent(fieldValue) + '"},';
            });

            if (errorInfo.length > 0) {
                $.jBox.alert(errorInfo, '错误');
                return null;
            }

            return removeLastComma(mainJson) + ']';
        }

        function getDtlsPostJsonString(fk_md) {
            /// <summary>获取并生成明细表填充字段的新值JSON字符串</summary>
            /// <param name="fk_md" Type="String">excel表单编号</param>
            var dtlsJson = '[';

            $.each(getObjectFromArray(fk_md, jwDtls), function () {
                dtlsJson += this.getValuesJsonString() + ',';
            });

            return removeLastComma(dtlsJson) + ']';
        }

        function removeLastComma(str) {
            /// <summary>去除指定字符串最后的逗号</summary>
            /// <param name="str" Type="String">字符串</param>
            if (str.charAt(str.length - 1) == ',') {
                return str.substr(0, str.length - 1);
            }
            return str;
        }

        function loadInfos(fk_md) {
            /// <summary>非第一次打开excel，加载填充字段信息</summary>
            /// <param name="fk_md" Type="String">excel表单的编号</param>
            var ccflow_main_bm_prefix = '',
                ccflow_dtl_bm_prefix = 'dtl_',
                ccflow_dtl_bm_prefix_len = ccflow_dtl_bm_prefix.length,
                jwfield,
                jwdtl,
                bmName,
                dtlBmNamePrefix,
                dtlBmNamePrefix_len,
                firstCell,
                colFields,
                name,
                formula,
                currSheets = getSheets(fk_md),
                currFields = getObjectFromArray(fk_md, fields),
                currDtlNos = getObjectFromArray(fk_md, dtlNos),
                main,
                mainArr = getObjectFromArray(fk_md, jwMains),
                dtlRows = getObjectFromArray(fk_md, jwDtls),
                field,
                dtlObj;

            if (mainArr == null) {
                main = {};
                main[fk_md] = mainArr = new Array();
                jwMains.push(main);
            }

            if (dtlRows == null) {
                dtlObj = {};
                dtlObj[fk_md] = dtlRows = new Array();
                jwDtls.push(dtlObj);
            }

            //获取主表填充字段集合
            for (var i = 0; i < currFields.length; i++) {
                name = getRangeName(currFields[i], ccflow_main_bm_prefix, currSheets);

                if (name == null) {
                    continue;
                }

                jwfield = new JWField(currFields[i], name.RefersToRange.Text, name.Parent.Name, name.Name);
                mainArr.push(jwfield);
            }

            //获取明细表填充字段信息
            for (var d = 0; d < currDtlNos.length; d++) {
                name = getRangeName(currDtlNos[d], ccflow_dtl_bm_prefix, currSheets);

                if (name == null) {
                    continue;
                }

                jwdtl = new JWDtl(currDtlNos[d], $.parseJSON('[]'));
                jwdtl.excelRange = name.RefersToRange;
                jwdtl.excelName = name.Name;
                jwdtl.sheetName = name.Parent.Name;
                jwdtl.header = getDtlHeaderRange(jwdtl.dtlNo, currSheets);
                dtlRows.push(jwdtl);

                firstCell = name.RefersToRange.Cells(1, 1);
                dtlBmNamePrefix = ccflow_dtl_bm_prefix + currDtlNos[d] + '_';
                dtlBmNamePrefix_len = dtlBmNamePrefix.length;

                for (var i = 1; i <= name.Parent.Names.Count; i++) {
                    bmName = name.Parent.Names(i).NameLocal.split('!')[1];

                    if (bmName.length <= dtlBmNamePrefix_len || bmName.substr(0, dtlBmNamePrefix_len) != dtlBmNamePrefix) {
                        continue;
                    }

                    colFields = bmName.substr(dtlBmNamePrefix_len).split('_');
                    formula = getFormula(colFields);
                    field = formula.length > 0 ? colFields.slice(0, colFields.length - 1).join('_') : bmName.substr(dtlBmNamePrefix_len);

                    jwdtl.columns.push(new JWColumn(
                        field,
                        formula,
                        jwdtl.sheetName,
                        name.Parent.Names(i).NameLocal,
                        jwdtl.header.field(field).InColumn));
                }
            }
        }

        // 给excel 填充数据. 
        function replaceParams(fk_md) {
            /// <summary>替换所有属性</summary>
            /// <param name="fk_md" Type="String">excel表单的编号</param>
            var dtl,
                jwdtl,
                dtlObj,
                dtlRows = getObjectFromArray(fk_md, jwDtls),
                currSheets = getSheets(fk_md),
                mainArr = getObjectFromArray(fk_md, jwMains);

            if (mainArr == null) {
                main = {};
                main[fk_md] = mainArr = new Array();
                jwMains.push(main);
            };

            if (dtlRows == null) {
                dtlObj = {};
                dtlObj[fk_md] = dtlRows = new Array();
                jwDtls.push(dtlObj);
            }

            //替换主表数据, 整理数据符合预期的格式.
            $.each(getObjectFromArray(fk_md, params), function () {
                // 替换区域的值.
                replace(this.key, this.value.replace("\\\\", "\\").replace("\'", "'"), this.type, fk_md, currSheets, mainArr);
            });

            //替换明细表数据,按照明细表名称，来检索明细表进行填充
            $.each(getObjectFromArray(fk_md, dtlNos), function () {
                dtl = getObjectFromArray(this, getObjectFromArray(fk_md, dtls));
                if (dtl == null) return true;

                jwdtl = new JWDtl(this, dtl);
                dtlRows.push(jwdtl);
                replaceDtl(this, dtl, jwdtl, currSheets);
            });

            //接受修订, 接受修订后把数据作为一个版本保存. 调用 weboffice 的方法.
            webOffice.SetTrackRevisions(4);
        }

        function getSheets(fk_md) {
            /// <summary>获取指定excel表单的所有sheet集合</summary>
            /// <param name="fk_md" Type="String">excel表单编号</param>
            var prefix = fk_md + '_';
            var ss = new Array();

            for (var i = 1; i <= wb.Sheets.Count; i++) {
                if (wb.Sheets(i).Name.length < prefix.length || wb.Sheets(i).Name.substr(0, prefix.length) != prefix) {
                    continue;
                }

                ss.push(wb.Sheets(i));
            }

            return ss;
        }

        function getObjectFromArray(objField, objArr) {
            /// <summary>从对象数组中获取该数组中指定属性名的对象</summary>
            /// <param name="objField" type="String">对象中的属性</param>
            /// <param name="objArr" type="Array">对象数组</param>
            if (objArr == null) return null;

            for (var i = 0, j = objArr.length; i < j; i++) {
                if (objArr[i][objField] != undefined) {
                    return objArr[i][objField];
                }
            }

            return null;
        }

        function getObjectFromArray2(objField, objValue, objArr) {
            /// <summary>从对象数组中获取该数组中指定属性值的对象</summary>
            /// <param name="objField" type="String">对象中的属性名称</param>
            /// <param name="objValue" type="Object">对象中的属性值</param>
            /// <param name="objArr" type="Array">对象数组</param>
            for (var i = 0; i < objArr.length; i++) {
                if (objArr[i][objField] == objValue) {
                    return objArr[i];
                }
            }

            return null;
        }

        function replace(field, text, type, fk_md, currSheets, mainArr) {
            /// <summary>替换文本</summary>
            /// <param name="oldStr" type="String">要替换的文本</param>
            /// <param name="newStr" type="String">要替换成的文本</param>
            /// <param name="type" type="String">值的类型，string/sign</param>
            /// <param name="fk_md" Type="String">excel表单的编号</param>
            /// <param name="currSheets" type="Array">要填充的所有sheet数组</param>
            /// <param name="mainArr" type="Array">前端保存主字段替换信息的数组</param>

            var ccflow_bm_name_prefix = '';
            var na = getRangeName(field, ccflow_bm_name_prefix, currSheets); //获得单元格区域对象.
            if (na != null && na.RefersToRange != null) {
                na.Parent.Activate();   //将Name所属sheet激活，使处于当前工作窗口
                na.RefersToRange.Select();

                if (type == "sign") {
                    AddPicture(window.location.protocol + "//" + window.location.host + "/DataUser/Siganture/" + text + ".JPG", app.Selection, 180, 100);
                }
                else {
                    na.RefersToRange.Value = text;
                }

                mainArr.push(new JWField(field, text, na.Parent.Name, na.Name));
            }
        }

        function getDtlHeaderRange(dtlno, currSheets) {
            /// <summary>获取明细表在excel中的表头数据</summary>
            /// <param name="dtlno" type="String">明细表No</param>
            /// <param name="currSheets" type="Array">要填充的所有sheet数组</param>
            var dtl_name = "dtl_" + dtlno;
            var dtl_name_prefix = dtl_name + "_";
            var dtl_table = getRangeName(dtlno, "dtl_", currSheets);
            var nm_name_prefix;
            var dtl_range;
            var dtl_sheet;
            var nm;
            var nm_range;
            var headers = {
                DtlRangeBeginRow: 0,
                BeginRow: 0,
                EndRow: 0,
                InBeginRow: 0,
                Cols: [],   //Name,FieldName,Row,Column,Rows,Columns,Range
                field: function (fieldName) {
                    /// <summary>获取指定字段所属的excel标头区域</summary>
                    var col;

                    $.each(this.Cols, function () {
                        if (this.FieldName == fieldName) {
                            col = this;
                            return false;
                        }
                    });

                    return col;
                },
                fieldByColumn: function (columnIdx) {
                    /// <summary>获取指定列号所包含的excel标头区域</summary>
                    var col;

                    $.each(this.Cols, function () {
                        if (this.InColumn == columnIdx) {
                            col = this;
                            return false;
                        }
                    });

                    return col;
                }
            };

            if (dtl_table != null) {
                dtl_sheet = dtl_table.Parent;
                dtl_range = dtl_table.RefersToRange;
                nm_name_prefix = dtl_table.Name + "_";
                headers.DtlRangeBeginRow = dtl_range.Row;

                for (var i = 1; i <= dtl_sheet.Names.Count; i++) {
                    //此处去除之前建立的单元格名称，但是现在已经失效了的
                    if (typeof dtl_sheet.Names(i) == "unknown" ||  dtl_sheet.Names(i).RefersToLocal.indexOf("!#REF!") != -1) {
                        continue;
                    }
                    
                    nm = dtl_sheet.Names(i);
                    nm_range = nm.RefersToRange;

                    if (nm.Name.indexOf(dtl_name_prefix) == -1) {
                        continue;
                    }

                    if (nm_range.Row < dtl_range.Row || nm_range.Row > dtl_range.Row + dtl_range.Rows.Count ||
                    nm_range.Column < dtl_range.Column || nm_range.Column > dtl_range.Column + dtl_range.Columns.Count) {
                        continue;
                    }

                    if (headers.Cols.length == 0) {
                        headers.BeginRow = nm_range.Row;
                    }

                    headers.BeginRow = Math.min(headers.BeginRow, nm_range.Row);
                    headers.EndRow = Math.max(headers.EndRow, nm_range.Row + (nm_range.MergeCells ? nm_range.MergeArea.Rows.Count - 1 : nm_range.Rows.Count - 1));
                    headers.InBeginRow = headers.EndRow - headers.DtlRangeBeginRow + 2;

                    headers.Cols.push({
                        Name: nm.Name,
                        FieldName: getFieldName(nm.Name.substr(nm_name_prefix.length)),
                        Row: nm_range.Row,
                        Column: nm_range.Column,
                        InRow: nm_range.Row - dtl_range.Row + 1,
                        InColumn: nm_range.Column - dtl_range.Column + 1,
                        Rows: nm_range.MergeCells ? nm_range.MergeArea.Rows.Count : nm_range.Rows.Count,
                        Columns: nm_range.MergeCells ? nm_range.MergeArea.Columns.Count : nm_range.Columns.Count,
                        Range: nm_range
                    });
                }
            }

            return headers;
        }

        function replaceDtl(dtlno, rows, jwdtl, currSheets) {
            /// <summary>填充明细表数据</summary>
            /// <param name="dtlno" type="String">明细表No</param>
            /// <param name="rows" type="Array">明细表数据行集合</param>
            /// <param name="jwdtl" type="JWDtl">明细表数据行集合</param>
            /// <param name="currSheets" type="Array">要填充的所有sheet数组</param>
            jwdtl.excelName = 'dtl_' + dtlno;

            var ccflow_table_bm_prefix = jwdtl.excelName + '_';
            var ccflow_table = getRangeName(dtlno, 'dtl_', currSheets);
            var dtlRange;
            var rng_header;

            if (ccflow_table != null) {
                //存储在内存中，在保存的时候好用
                jwdtl.excelRange = dtlRange = ccflow_table.RefersToRange;
                var currRowsCount = dtlRange.Rows.Count;
                jwdtl.sheetName = ccflow_table.Parent.Name;
                jwdtl.header = rng_header = getDtlHeaderRange(dtlno, currSheets);
                ccflow_table.Parent.Activate();

                //补全行数
                var currRows = currRowsCount - (rng_header.EndRow - rng_header.DtlRangeBeginRow + 1);
                if (currRows < rows.length + 1) {
                    dtlRange.Rows(currRowsCount).Select();
                    for (var i = rows.length + 1 - currRows; i > 0; i--) {
                        app.Selection.Insert(-4121, 0);
                        dtlRange.Rows(dtlRange.Rows.Count).Select();
                        app.Selection.Copy();
                        dtlRange.Rows(currRowsCount).Select();
                        app.Selection.PasteSpecial(-4122, -4142, false, false);
                        app.CutCopyMode = false;
                    }
                }

                var cellIDPrefixLen = ccflow_table_bm_prefix.length;
                var cellNamePrefix = jwdtl.sheetName + '!' + ccflow_table_bm_prefix;
                var cellNamePrefixLen = cellNamePrefix.length;
                var firstRow = dtlRange.Rows(1);
                var firstCell;
                var colField;
                var formula;
                var arrCellFields;
                var isRowId;
                var col;

                //填充数据
                for (var j = 1; j <= dtlRange.Columns.Count; j++) {
                    col = rng_header.fieldByColumn(j);

                    if (typeof col == "undefined") {
                        continue;
                    }

                    firstCell = col.Range;

                    if (firstCell == null || typeof firstCell.Name == 'unknown') {
                        continue;
                    }

                    if (firstCell.Name.Name.length <= cellNamePrefixLen || firstCell.Name.Name.substr(0, cellNamePrefixLen) != cellNamePrefix) {
                        continue;
                    }

                    arrCellFields = firstCell.Name.Name.substr(cellNamePrefixLen);
                    colField = getFieldName(arrCellFields);
                    formula = arrCellFields.length > colField.length ? arrCellFields.substr(colField.length + 1) : '';
                    isRowId = colField.toLowerCase() == 'rowid';

                    //记录填充列信息
                    jwdtl.columns.push(new JWColumn(colField, formula, jwdtl.sheetName, firstCell.Name.Name, j));

                    //明细表中的列不考虑签名列的情况
                    for (var i = 0; i < rows.length; i++) {
                        //dtlRange.Rows(i + 2).Cells(j).Select();
                        dtlRange.Rows(i + rng_header.InBeginRow).Cells(j).Value = isRowId ? (i + 1).toString() : rows[i][colField];
                    }

                    //增加汇总
                    if (formula.length > 0) {
                        var colAlpha = String.fromCharCode(64 + firstCell.Column);
                        var formulaString = '=' + formula + '(' + colAlpha + (rng_header.EndRow + 1) + ':' + colAlpha + (rng_header.EndRow + rows.length) + ')';
                        var formulaFormat = '';
                        var formulaLower = formula.toLocaleLowerCase();

                        if (formulaLower == 'count' || formulaLower == 'counta') {
                            formulaFormat = '"总数："0';
                        }
                        else if (formulaLower == 'average') {
                            formulaFormat = '"平均："0.00';
                        }

                        //判断是否已经增加了汇总行
                        //汇总行不属于明细表所处的命名区域内
                        var dtlLastRow = dtlRange.Rows(dtlRange.Rows.Count).Row;

                        if (dtlRange.Rows.Count - rng_header.InBeginRow + 1 < rows.length + 2) {
                            ccflow_table.Parent.Rows(dtlLastRow + 1).Select();
                            app.Selection.Insert(-4121);
                            ccflow_table.Parent.Rows(dtlLastRow).Select();
                            app.Selection.Copy();
                            ccflow_table.Parent.Rows(dtlLastRow + 1).Select();
                            app.Selection.PasteSpecial(-4122, -4142, false, false);
                            app.CutCopyMode = false;
                            app.Selection.ClearContents();
                        }

                        ccflow_table.Parent.Cells(dtlLastRow + 1, firstCell.Column + j - 1).Select();
                        app.ActiveCell.FormulaLocal = formulaString;
                        app.Selection.NumberFormatLocal = formulaFormat;
                    }
                }
            }
        }

        function getFieldName(name) {
            /// <summary>获取属性名称，比如从DeptName_Sum中获得DeptName,去掉后面的是汇总函数名</summary>
            /// <param name="name" type="String">名称</param>
            var arr = name.split('_');
            if (arr.length < 2) return name;
            var last = arr[arr.length - 1];

            for (var i = 0, j = exceptions.length; i < j; i++) {
                if (last.toLowerCase() == exceptions[i]) {
                    return arr.slice(0, arr.length - 1).join('_');
                }
            }

            return name;
        }

        function getFormula(arr) {
            /// <summary>获取汇总函数名</summary>
            /// <param name="arr" type="Array">名称按照'_'进行拆分的数组</param>
            if (arr.length < 2) return '';
            var last = arr[arr.length - 1];

            for (var i = 0, j = exceptions.length; i < j; i++) {
                if (last.toLowerCase() == exceptions[i]) {
                    return last;
                }
            }

            return '';
        }

        // 根据前缀与字段名，返回他的区域对象，如果没有就返回空.
        function getRangeName(sName, sCcflowPrefix, aCurrSheets) {
            /// <summary>获取指定字段所标识的单元格区域名称对象</summary>
            /// <param name="sName" type="String">字段名称</param>
            /// <param name="sCcflowPrefix" type="String">ccflow的名称标识字符串</param>
            /// <param name="aCurrSheets" type="Array">要检索的sheet集合</param>

            var nms, //当前工作簿的命名区域集合.
                na,
                na_len;

            for (var s = 0; s < aCurrSheets.length; s++) {
                nms = aCurrSheets[s].Names;
                na = aCurrSheets[s].Name + '!' + sCcflowPrefix + sName;
                na_len = na.length;

                for (var i = 1; i <= nms.Count; i++) {
                    if (aCurrSheets[s].Names(i).Name == na ||
                    (aCurrSheets[s].Names(i).Name.length > na_len &&
                    aCurrSheets[s].Names(i).Name.substr(0, na_len) == na &&
                    aCurrSheets[s].Names(i).Name.substr(na_len).split('_').length < 3)) {    //此处过滤掉有相同前缀的属性
                        return aCurrSheets[s].Names(i);
                    }
                }
            }

            return null;
        }

        function showSheets(fk_md) {
            /// <summary>显示当前表单模板中所有与指定FK_MapData相关的表单Sheet</summary>
            /// <param name="fk_md" Type="String">指定FK_MapData,即表单库中表单的No号</param>
            fk_mapdata = getObjectFromArray2("Name", fk_md, fk_mapdatas);
            sheetPrefix = fk_md + '_';
            sheets.length = 0;
            var notCurrSheets = [];

            //获取所有属于当前表单的sheet集合，隐藏非当前表单的sheet之前，要先将当前表单的sheet显示出来，因不能全部隐藏所有sheet
            for (var i = 1; i <= wb.Sheets.Count; i++) {
                if (wb.Sheets(i).Name.length < sheetPrefix.length || wb.Sheets(i).Name.substr(0, sheetPrefix.length) != sheetPrefix) {
                    notCurrSheets.push(wb.Sheets(i));
                    continue;
                }

                wb.Sheets(i).Visible = xlSheetVisible;
                sheets.push(wb.Sheets(i));
            }

            if (sheets.length == 0) {
                for (var i = 1; i <= wb.Sheets.Count; i++) {
                    if (wb.Sheets(i).Visible == xlSheetVisible) {
                        sheets.push(wb.Sheets(i));
                    }
                }
            }
            else {
                //将非当前表单的sheet隐藏掉
                for (var i = 0; i < notCurrSheets.length; i++) {
                    notCurrSheets[i].Visible = xlSheetVeryHidden;
                }
            }

            if (sheets.length == 0) {
                $.jBox.alert('表单模板不正确，未找到与“' + fk_mapdata.Text + '”相关的表单，请与管理员联系', '异常');
                return;
            }

            sheet = sheets[0]; // 当前活动的工作簿.
            sheet.Activate();

            if ('<%=IsEdit %>' == 'True') {
                initToolbar(fk_mapdata);
            }
        }

        function initToolbar(fk_md) {
            /// <summary>根据FK_MapData加载相应表单的功能按钮</summary>
            /// <param name="fk_md" Type="String">FK_MapData</param>
            toolbarsln = getObjectFromArray(fk_md.Name, toolbarslns);

            if (toolbarsln == null) {
                $.jBox.alert('表单模板对应的按钮权限不正确，未找到与“' + fk_md.Text + '”相关的权限，请与管理员联系', '异常');
                return;
            }

            $('#divMenu').empty();

            if (toolbarsln.OfficeSaveEnable)
                AddBtn("Btn_Save", toolbarsln.OfficeSaveLab, "SaveOffice()", "icon-save");
            else
                return; //就不要向下显示其他的按钮了。

            if (toolbarsln.OfficeIsMarks)
                $('#divMenu').append("<select id='marks' onchange='ShowUserName()' style='width: 100px'><option value='1'>全部</option><select>&nbsp;&nbsp;");

            if (toolbarsln.OfficeOpenEnable)
                AddBtn("Btn_Open", toolbarsln.OfficeOpenLab, "OpenTempLate()", "icon-open");

            if (toolbarsln.OfficeRefuseEnable) {
                AddBtn("Btn_Accept", "接受修订", "acceptOffice()", "icon-accept");
                AddBtn("Btn_Refuse", "拒绝修订", "refuseOffice()", "icon-refuse");
            }

            if (toolbarsln.OfficeTHEnable)
                AddBtn("over", "套红文件", "overOffice()", "");

            if (toolbarsln.OfficePrintEnable)
                AddBtn("Btn_Print", toolbarsln.OfficePrintLab, "printOffice()", "icon-print");

            if (toolbarsln.OfficeSealEnable)
                AddBtn("Btn_Seal", "签章", "sealOffice()", "icon-seal");

            if (toolbarsln.OfficeDownEnable)
                AddBtn("Btn_Download", toolbarsln.OfficeDownLab, "DownLoad()", "icon-download");

            $.parser.parse('#divMenu');
        }

        function AddBtn(id, label, clickEvent, iconCls, container) {
            /// <summary>根据FK_MapData加载相应表单的功能按钮</summary>
            /// <param name="fk_md" Type="String">FK_MapData</param>
            var c = container == undefined || container == null ? $('#divMenu') : $(container);
            c.append("<a href=\"javascript:void(0)\" id='" + id + "' onclick=\"return " + clickEvent + "\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'" + iconCls + "'\">" + label + "</a>&nbsp;&nbsp;");
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
        <div id="mapdata">
        </div>
        <div id="divMenu" runat="server">
        </div>
        <div style="clear: both">
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
