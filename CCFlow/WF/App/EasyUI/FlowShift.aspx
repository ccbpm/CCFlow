<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FlowShift.aspx.cs" Inherits="CCFlow.AppDemoLigerUI.FlowShift" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>工作移交</title>
     <script src="../../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
   
    <link href="../../Scripts/Jquery-plug/jquery-autocomplete/jquery.autocomplete.css"
        rel="stylesheet" type="text/css" />
    <script src="../../Scripts/Jquery-plug/jquery-autocomplete/jquery.autocomplete.pack.js"
        type="text/javascript"></script>
    <script src="../../Scripts/Jquery-plug/jquery-autocomplete/jquery.autocomplete.js"
        type="text/javascript"></script>
    <script src="../../Scripts/Jquery-plug/jquery-autocomplete/jquery.autocomplete.min.js"
        type="text/javascript"></script>
    <script type="text/javascript">

        $(function () {
      
            initComp();
        });


        function initComp() {
            $('#<%=TB_Emp.ClientID%>').autocomplete("Base/DataServices.ashx?action=getEmp&key=" + encodeURI($('#<%= TB_Emp.ClientID%>').val()), {
                max: 10,    //列表里的条目数
                minChars: 0,    //自动完成激活之前填入的最小字符
                width: 200,     //提示的宽度，溢出隐藏
                scrollHeight: 300,   //提示的高度，溢出显示滚动条
                matchContains: true,    //包含匹配，就是data参数里的数据，是否只要包含文本框里的数据就显示
                autoFill: false,    //自动填充
                parse: function (data) {
                    return $.map(eval(data), function (row) {
                        return {
                            data: row,
                            value: row.Name,
                            result: row.Name+"("+row.No+")"
                        }
                    });
                },
                formatItem: function (row, i, max) {
                   
                    return row.Name+"("+row.No+")";
                },
                formatMatch: function (row, i, max) {
                    return row.Name + "(" + row.No + ")";
                },
                formatResult: function (row) {
                    return row.Name + "(" + row.No + ")";
                }
            }).result(function (event, row, formatted) {
                $('#<%= TB_Emp.ClientID%>').val(row.No);
            });
        }
    </script>
    <base target="_self" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <fieldset>
            <legend>工作请选择要把此工作移交的人员</legend>请输入移交人员:
            <%--<select id="cmb_Emp" class="easyui-combogrid" style="width: 215px" />--%>
             <asp:TextBox ID="TB_Emp" runat="server" Width="200px"></asp:TextBox>
           <%-- <asp:TextBox ID="TB_Emp" Style="visibility: hidden" runat="server" Width="159px">
            </asp:TextBox>--%>
            <br />
            原因:
            <br />
            <asp:TextBox ID="TB_Note" TextMode="MultiLine" runat="server" Width="336px" Height="91px"></asp:TextBox>
            <br/>
            说明:只能输入一个人员. &nbsp;<hr>
            <asp:Button ID="Btn_OK" runat="server" Text="确定移交" OnClick="Btn_OK_Click" />
            <asp:Button ID="Btn_Cancel" runat="server" Text="取消并关闭" OnClick="Btn_Cancel_Click" />
        </fieldset>
    </div>
    </form>
</body>
</html>
