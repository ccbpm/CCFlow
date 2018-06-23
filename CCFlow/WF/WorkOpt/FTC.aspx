<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="FTC.aspx.cs" Inherits="CCFlow.WF.WorkOpt.FTC" %>

<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>流转自定义（简单模式）</title>
    <link href="../../DataUser/Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <link href="../Comm/JS/Calendar/skin/WdatePicker.css" rel="stylesheet" type="text/css" />
    <script language="JavaScript" src="../Comm/JS/Calendar/WdatePicker.js" defer="defer"
        type="text/javascript"></script>
    <script type="text/javascript">
        function Save() {
            try {
                document.getElementById('Btn_Save').click(); //调用btn_save事件.
                alert('保存成功.');
                return true; //保存成功，用户可以发送.
            } catch (e) {
                alert(e.name + " :  " + e.message);
                return false; // 保存失败不能发送.
            }
        }

        var currDdl;
        var currHid;
        var currNodeId;
        var prevVal = {};

        function selectEmps(ddl) {
            var val = ddl.options[ddl.selectedIndex].value;
            currDdl = ddl;
            var ids = ddl.id.split('_');
            currNodeId = ids[ids.length - 1];

            if (val != '0') {
                prevVal['node_' + currNodeId] = val;
                return;
            }

            currHid = $("input[id$='HID_" + currNodeId + "']");
            var emps = currHid[0].value.split(';');
            var select = $('#emps');

            select.empty();

            $.each(emps, function () {
                if (this.length == 0) {
                    return true;
                }

                select.append("<option value='" + this.split(',')[0] + "'>" + this + "</option>");
            });

            $('#dlg').dialog('open');
        }

        function saveSelectEmps(isSave) {
            if (isSave) {
                var selval = $('#emps').val();
                if (selval.length == 0) {
                    alert('请选择处理人员');
                    return;
                }
                //alert($('#emps').find("option:selected").text());

                var selvalstring = "," + selval.toString() + ",";
                var isexist = false;
                var seltext = '';
                var items;

                $.each($(currDdl).find('option'), function () {
                    items = this.value.split(',');
                    isexist = true;

                    for (var i = 0; i < items.length; i++) {
                        if (selvalstring.indexOf(',' + items[i] + ',') == -1) {
                            isexist = false;
                            break;
                        }
                    }

                    if (isexist && items.length == selval.length) {
                        isexist = true;
                        return false;
                    }
                });

                if (!isexist) {
                    $.each(selval, function () {
                        seltext += $('#' + currDdl.id + ' option[value="' + this + '"]').text() + ",";
                    });

                    $('#' + currDdl.id + ' option[value="0"]').before("<option value='" + selval + "'>" + seltext.substr(0, seltext.length - 1) + "</option>");
                }

                $('#' + currDdl.id).val(selval.toString());
                prevVal['node_' + currNodeId] = selval;
            }
            else {
                $('#' + currDdl.id).val(prevVal['node_' + currNodeId].toString());
            }

            $('#dlg').dialog('close');
        }

        $(function () {
            var ddls = $("select[id*='Pub1_DDL_']");
            var ids;

            $.each(ddls, function () {
                ids = this.id.split('_');
                prevVal['node_' + ids[ids.length - 1]] = this.options[this.selectedIndex].value;
            });
        });
    </script>
</head>
<body topmargin="0" leftmargin="0">
    <form id="form1" runat="server">
    <uc1:Pub ID="Pub1" runat="server" />
    <%--<div style="display:none ">--%>
    <div>
        <asp:Button ID="Btn_Save" runat="server" Text="保存" OnClick="Btn_Save_Click" />
    </div>
    <div id="dlg" class="easyui-dialog" title="选择多人处理" style="width: 200px; height: 200px;"
        data-options="resizable:true,modal:true,closed:true,
			toolbar:[{
				text:'确定',
				iconCls:'icon-ok',
				handler:function(){saveSelectEmps(true);}
			},{
				text:'取消',
				iconCls:'icon-cancel',
				handler:function(){saveSelectEmps(false);}
			}]">
        <select id="emps" multiple="multiple" style="width: 100%; height: 100%;">
        </select>
    </div>
    </form>
</body>
</html>
