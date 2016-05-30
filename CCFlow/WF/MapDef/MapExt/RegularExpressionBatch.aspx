<%@ Page Title="" Language="C#" MasterPageFile="~/WF/MapDef/WinOpen.master" AutoEventWireup="true" CodeBehind="RegularExpressionBatch.aspx.cs" Inherits="CCFlow.WF.MapDef.RegularExpressionBatch" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table style=" width:100%">
<caption>批量设置控件的正则表达式</caption>
<tr>
<th>序号</th>
<th>字段/名称</th>
<th>类型</th>
<th>表达式</th>
<th>操作</th>
</tr>

<%
    string fk_mapdata = this.Request.QueryString["FK_MapData"];

    if (string.IsNullOrEmpty(fk_mapdata))
        fk_mapdata = "ND101";
        
    BP.Sys.MapAttrs attrs = new BP.Sys.MapAttrs(fk_mapdata);
    int idx = 0;
    foreach (BP.Sys.MapAttr attr in attrs)
    {
        if (attr.UIVisible == false)
            continue;
        if (attr.MyDataType != BP.DA.DataType.AppString)
            continue;
        
        idx++;
        %>
        <tr>
        <td class="Idx"><%=idx%></td>
        <td > <input  type="checkbox" id="<%=attr.KeyOfEn %>"   value="<%=attr.KeyOfEn %>  - <%=attr.KeyOfEn %>"  name="<%=attr.KeyOfEn %>" />  <%=attr.KeyOfEn%>  - <%=attr.Name%></td>
         <td><%=attr.MyDataTypeStr%></td>
<td>无</td>
        </tr>
        <%
   }
%>

<tr>
<td colspan="6">    
<input type="button"  value="批量设置"  onclick="Save()" />
 <%--   <asp:Button ID="Btn_Save" runat="server" Text="批量设置" />--%>
    </td>
</tr>
</table>

<script  type="text/javascript">

    function Save() {


        alert('xxxx');

        //获得选择的字段.
        var arrObj = document.all;

        var fields = '';

        for (var i = 0; i < arrObj.length; i++) {

            if (typeof arrObj[i].type == "undefined")
                continue;
            if (arrObj[i].type != 'checkbox')
                continue;

            var cb = arrObj[i];

            if (cb.check == false)
                continue;

            fields += arrObj[i].name + ',';
        }

        alert('字段被选择' + fields);

        if (fields == '') {
            alert('请选择要批量设置的字段');
            return;
        }

        var url = 'RegularExpression.aspx?s=3&FK_MapData=ND207&RefNo=' + fields + '&ExtType=RegularExpression&OperAttrKey=ND207_QingJiaYuanYin&DoType=templete';
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');


    }

</script>
</asp:Content>
