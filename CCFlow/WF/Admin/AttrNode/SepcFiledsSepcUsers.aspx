<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true" CodeBehind="SepcFiledsSepcUsers.aspx.cs" Inherits="CCFlow.WF.Admin.AttrNode.SepcFiledsSepcUsers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


<table style=" width;100%;" >
<caption>特别字段特殊用户权限</caption>
<tr>
<th>序号</th>
<th>字段/名称</th>
<th>类型</th>
<th>业务类型</th>
<th>操作</th>
</tr>

<%
    string fk_mapdata = this.Request.QueryString["FK_MapData"];
    if (string.IsNullOrEmpty(fk_mapdata))
        fk_mapdata = "ND101";

    string fk_node = this.Request.QueryString["FK_Node"];
    if (string.IsNullOrEmpty(fk_node))
        fk_mapdata = "101";
    
        
    BP.Sys.MapAttrs attrs = new BP.Sys.MapAttrs(fk_mapdata);
    int idx = 0;
    foreach (BP.Sys.MapAttr attr in attrs)
    {
        if (attr.UIVisible == false)
            continue;
        
        if (attr.UIIsEnable==true) 
            continue;
        
        idx++;
        %>
        <tr>
        <td class="Idx"><%=idx%></td>
        <td > <input  type="checkbox" id="<%=attr.KeyOfEn %>"   value="<%=attr.KeyOfEn %>  - <%=attr.KeyOfEn %>"  name="<%=attr.KeyOfEn %>" />  <%=attr.KeyOfEn%>  - <%=attr.Name%></td>
         <td><%=attr.MyDataTypeStr%></td>
<td><%=attr.LGTypeT %></td>
<td>无</td>
        </tr>
  <% } %>

<tr>
<th>序号</th>
<th colspan="3">电子公章ID/名称</th>
<th>操作</th>
</tr>

<%
    BP.Sys.FrmImgs imgs = new BP.Sys.FrmImgs(fk_mapdata);
    foreach (BP.Sys.FrmImg img in imgs)
    {
        if (img.HisImgAppType == BP.Sys.ImgAppType.Img)
             continue;
        if (img.IsEdit == 1)
            continue;

        idx++;
        
        %>
        <tr>
        <td class="Idx"> <%=idx %></td>
        <td colspan=3>
        <input  type="checkbox" id="<%=img.MyPK %>" name="<%=img.MyPK %>"   value="<%=img.MyPK %>"  />  <%=img.MyPK%>  - <%=img.Name%>
        </td>

        <td>无 </td>

        </tr>
        <%
    }
%>

<tr>
<td colspan="6">

<input type="button"  value="设置特别权限"  onclick="Save('<%=fk_mapdata %>','<%=fk_node %>')" />
 <%--   <asp:Button ID="Btn_Save" runat="server" Text="批量设置" />--%>
    </td>
</tr>

<!-- 历史设置. -->
<tr>
<th colspan="6" > 已经设定的权限</th>
</tr>

<tr>
<td colspan="6">
<ul>
<% 
    BP.Sys.MapExts exts = new BP.Sys.MapExts();
    exts.Retrieve(BP.Sys.MapExtAttr.FK_MapData, fk_mapdata, BP.Sys.MapExtAttr.Tag, this.Request.QueryString["FK_Node"], BP.Sys.MapExtAttr.ExtType, "SepcFiledsSepcUsers");

    foreach (BP.Sys.MapExt item in exts)
    {
        %>
       <li> <a href="javascript:OpenIt('<%=fk_mapdata %>','<%=fk_node %>','<%=item.MyPK %>')">设置:<%=item.Doc %></a>  - <%=item.Tag1 %>-<%=item.Tag2 %>-<%=item.Tag3 %></li>
        <%
    }
     %>

     </ul>
</td>
</tr>


</table>

<script  type="text/javascript">

    function OpenIt(mapdata, fk_node, extpk) {
        var url = 'SepcFiledsSepcUsersDtl.aspx?s=3&FK_Node=' + fk_node + '&FK_MapData=' + mapdata + '&MyPK=' + extpk;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
        if (b != null) {
            window.location.href = window.location.href;
        }
    }

    function Save(fk_mapdata, fk_node) {

        //获得选择的字段.
        var arrObj = document.all;
        var fields = '';
        for (var i = 0; i < arrObj.length; i++) {

            if (typeof arrObj[i].type != "undefined" && arrObj[i].type == 'checkbox') {
                var cid = arrObj[i].name + ',';
                if (arrObj[i].checked) {
                    fields += arrObj[i].name + ',';
                }
            }
        }

       // alert('字段被选择' + fields);

        if (fields == '') {
            alert('请选择要批量设置的字段');
            return;
        }

        var url = 'SepcFiledsSepcUsersDtl.aspx?s=3&FK_Node=' + fk_node + '&FK_MapData=' + fk_mapdata + '&Fields=' + fields + '&ExtType=RegularExpression&OperAttrKey=ND207_QingJiaYuanYin&DoType=templete';
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');

        if (b != null) {
            window.location.href = window.location.href;
        }
    }
</script>

</asp:Content>
