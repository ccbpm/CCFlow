<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelectDepts_Jq.aspx.cs"
    Inherits="CCFlow.WF.Comm.Port.SelectDepts_Jq" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!--by liuxc,2014.12.17-->
    <title>选择部门</title>
    <link href="../Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <style type="text/css">
        ul.alphalist
        {
            list-style-type: none;
            line-height: 30px;
            margin: 0;
            width: 100%;
            height: 100%;
            padding: 0;
        }
        
        ul.alphalist li
        {
            font-size:20px;
            float: left;
            width: 30px;
            margin: 2px;
            border: 1px dashed #99bbe8;
            text-align: center;
            vertical-align: middle;
        }
        
        ul.alphalist li:hover
        {
            background-color: #e0ecff;
            cursor: pointer;
        }
        
        ol.deptlist
        {
            list-style-position: inside;
            margin: 0;
            line-height: 30px;
            width: 100%;
            height: 100%;
            padding: 0;
        }
        
        ol.deptlist li
        {
            width: 100%;
            margin: 0 2px;
            border-bottom: 1px dashed #99bbe8;
            text-align: left;
            vertical-align: middle;
        }
        
        ol.deptlist li:hover
        {
            background-color: #e0ecff;
            cursor: pointer;
        }
    </style>
    <script type="text/javascript">
        var getDept;

        $(function () {
            $.messager.defaults.ok = '是';
            $.messager.defaults.cancel = '否';

            $('ul.alphalist li').bind('click', function () {
                querydata('group', $(this).text());
            });

            if (location.search.length > 1) {
                var params = location.search.substr(1).split('&');
                var param;

                $.each(params, function () {
                    param = this.split('=');

                    if (param.length > 1) {
                        if (param[0].toLowerCase() == 'in' && param[1].length > 0) {
                            getDept = true;
                            querydata('getdepts', param[1]);
                        }
                    }
                });
            }
        });

        function getReturnValue() {
            var value = new Array();

            $('#sDeptList li').each(function () {
                value.push($(this).find('a').attr('id').substr('dept_s_'.length));
            });

            return value;
        }

        function getReturnText() {
            var text = new Array();

            $('#sDeptList li').each(function () {
                text.push($(this).find('span').text().substr('   '.length));    //此处不知何原因，用replace去不掉空格
            });

            return text;
        }

        function querydata(method, kw) {
            var haveSub = $('#chkHaveSubDept').attr('checked') == 'checked';
            var haveSame = $('#chkHaveSameLevelDept').attr('checked') == 'checked';

            $.ajax({
                type: "POST",
                url: "SearchDepts.ashx?s=" + Math.random(),
                dataType: 'json',
                data: { "method": method, "kw": kw, "havesub": haveSub, "havesame": haveSame },
                success: function (depts) {
                    if (depts != "") {
                        if (getDept) {
                            for (var i = 0; i < depts.length; i++) {
                                $.parser.parse($('#sDeptList').append("<li><a id='dept_s_" + depts[i].No + "' href='javascript:void(0)' onclick=\"unSelectDept(this, '" + depts[i].No + "')\" class='easyui-linkbutton' data-options=\"iconCls:'icon-delete',plain:true\" style=\"float:right\"> </a><span>" + depts[i].Name + "</span></li>"));
                            }

                            $('.easyui-layout').layout('panel', 'east').panel('setTitle', '已选部门[' + depts.length + ']');

                            getDept = false;
                            return;
                        }

                        var isExist;
                        $('#tbDepts').empty();

                        for (var i = 0; i < depts.length; i++) {
                            isExist = $('#sDeptList').find("a[id='dept_s_" + depts[i].No + "']").length > 0;
                            $('#tbDepts').append("<li><input type='checkbox' id='dept_" + depts[i].No + "' " + (isExist ? "checked" : "") + " /><label for='dept_" + depts[i].No + "'>" + depts[i].Name + "</label></li>");
                        }
                        
                        $('.easyui-layout').layout('panel', 'center').panel('setTitle', '部门[' + depts.length + ']');

                        $("#tbDepts input[type='checkbox']").bind('click', function () {
                            selectDept(this);
                        });
                    }
                    else {
                        $('#tbDepts').empty();
                        $('#tbDepts').append("<li>未搜索到任何部门！请换关键词或助记码重新搜索！</li>");
                        $('.easyui-layout').layout('panel', 'center').panel('setTitle', '部门[0]');
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    $.messager.alert('错误', 'textStatus=' + textStatus + '&nbsp;errorThrown=' + errorThrown, 'error');
                }
            });
        }

        function selectDept(chkObj) {
            var sdeptid = chkObj.id.replace('_', '_s_');
            var deptid = chkObj.id.substr('dept_'.length);
            var sdept = $('#sDeptList').find("a[id='" + sdeptid + "']");

            if (chkObj.checked) {
                if (sdept.length == 0) {
                    $.parser.parse($('#sDeptList').append("<li><a id='" + sdeptid + "' href='javascript:void(0)' onclick=\"unSelectDept(this, '" + deptid + "')\" class='easyui-linkbutton' data-options=\"iconCls:'icon-delete',plain:true\" style=\"float:right\"> </a><span>" + $(chkObj).next().text() + "</span></li>"));
                    $('.easyui-layout').layout('panel', 'east').panel('setTitle', '已选部门[' + $('#sDeptList').children().length + ']');
                }
            }
            else {
                if (sdept.length > 0) {
                    sdept.first().parent().remove();
                    $('.easyui-layout').layout('panel', 'east').panel('setTitle', '已选部门[' + $('#sDeptList').children().length + ']');
                }
            }
        }

        function unSelectDept(obj, deptid) {
            $(obj).parent().remove();
            var sdept = $("#tbDepts input[id='dept_" + deptid + "']");

            if (sdept.length > 0) {
                sdept.first().removeAttr("checked");
            }

            $('.easyui-layout').layout('panel', 'east').panel('setTitle', '已选部门[' + $('#sDeptList').children().length + ']');
        }

        function search1() {
            var kw = $('#txtKeyword').val();

            if (kw == null || kw.length == 0) {
                $.messager.alert('提示', '请输入要搜索的关键词或助记码!', 'info', function () { $('#txtKeyword').focus(); });
                return;
            }

            querydata('search', kw);
        }

        function searchAll() {
            $.messager.confirm('询问', '你确认要列出所有部门吗？如果数据量较大，列出时间会慢一些，请耐心等待……', function (r) {
                if (r) {
                    querydata('all', '');
                }
            });
        }

        function selectDeptAll() {
            $.each($("#tbDepts input[type='checkbox']"), function () {
                this.checked = true;
                selectDept(this);
            });
        }

        function unSelectDeptAll() {
            $("#sDeptList a").click();
        }
    </script>
</head>
<body class="easyui-layout">
    <form id="form1" runat="server">
    <div data-options="region:'north',noheader:true" style="overflow-y: hidden; height: 35px;
        padding: 5px; background-color: #E0ECFF;">
        关键字：<input type="text" id="txtKeyword" maxlength="10" style="width: 100px" />
        搜索：<input type="checkbox" id="chkHaveSubDept" /><label for="chkHaveSubDept">子部门</label>
        <input type="checkbox" id="chkHaveSameLevelDept" /><label for="chkHaveSameLevelDept">同级部门</label>
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-search'"
            onclick="search1()">搜索</a> <a href="javascript:void(0)" class="easyui-linkbutton"
                onclick="searchAll()">所有</a>
    </div>
    <div data-options="region:'west',title:'字母分组',split:true" style="overflow-x: hidden;
        padding: 5px; width: 145px">
        <ul class="alphalist">
            <li>A</li>
            <li>B</li>
            <li>C</li>
            <li>D</li>
            <li>E</li>
            <li>F</li>
            <li>G</li>
            <li>H</li>
            <li>I</li>
            <li>J</li>
            <li>K</li>
            <li>L</li>
            <li>M</li>
            <li>N</li>
            <li>O</li>
            <li>P</li>
            <li>Q</li>
            <li>R</li>
            <li>S</li>
            <li>T</li>
            <li>U</li>
            <li>V</li>
            <li>W</li>
            <li>X</li>
            <li>Y</li>
            <li>Z</li>
        </ul>
    </div>
    <div data-options="region:'center',title:'部门',tools:[{ iconCls:'icon-ok', handler:selectDeptAll }]"
        style="padding: 5px">
        <ol id="tbDepts" class="deptlist">
        </ol>        
    </div>
    <div data-options="region:'east',title:'已选部门',split:true,tools:[{ iconCls:'icon-delete', handler:unSelectDeptAll }]"
        style="padding: 5px; width: 200px">
        <ol id="sDeptList" class="deptlist">
        </ol>
    </div>
    </form>
</body>
</html>
