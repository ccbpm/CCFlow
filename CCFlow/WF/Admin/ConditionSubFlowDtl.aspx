<%@ Page Title="条件设置" Language="C#" AutoEventWireup="true" CodeBehind="ConditionSubFlowDtl.aspx.cs"
    Inherits="CCFlow.WF.Admin.ConditionSubFlowDtl" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../Comm/Style/CommStyle.css" rel="stylesheet" type="text/css" />
    <link href="../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        var currCond = '<%=this.CurrentCond %>';

        function changeCond(c) {
            if (c == null || c.value.length == 0) return;

            $('#mainCond').layout('panel', 'center').panel('setTitle', c.text);
            $('#context').attr('src', c.value + '.htm?CondType=3&FK_Flow=<%=this.FK_Flow %>&FK_MainNode=<%=this.FK_Node %>&FK_Node=<%=this.FK_Node %>&FK_Attr=<%=this.FK_Attr %>&DirType=<%=this.DirType %>&ToFlow=<%=this.ToFlow %>');
        }

        $(document).ready(function () {
            if (currCond.length > 0) {
                $('#cond').combobox('select', currCond);
            }
            else {
                $('#cond').combobox('select', 'Cond');
            }
        });
    </script>
    <base target="_self" />
</head>
<body class="easyui-layout">
    <form id="form1" runat="server">
    <div data-options="region:'center',border:false,title:'<%=this.Title %>'">
        <div id="mainCond" class="easyui-layout" data-options="fit:true">
            <div data-options="region:'north',border:false" style="height:35px; padding:5px">
                <label for="">
                    请选择条件设置类型：</label>
                <select id="cond" class="easyui-combobox" name="cond" data-options="onSelect:function(rec){ changeCond(rec); }">
                    <option value="CondByFrm">表单条件</option>
                    <option value="CondStation">岗位条件</option>
                    <option value="CondDept">部门条件</option>
                    <option value="CondBySQL">SQL条件</option>
                    <option value="CondByPara">开发者参数</option>
                    <option value="CondByUrl">Url条件</option>
                </select>
            </div>
            <div data-options="region:'center',title:' '" style="overflow-y:hidden;">
             <iframe id="context" scrolling="no" frameborder="0" src=""
                    style="width: 100%; height: 100%;"></iframe>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
