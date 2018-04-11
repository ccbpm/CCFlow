<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true" CodeBehind="SepcFiledsSepcUsers.aspx.cs" Inherits="CCFlow.WF.Admin.AttrNode.SepcFiledsSepcUsers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<fieldset>
<legend>特别控件特别用户权限 </legend>
<table style="border-width:100%;" >
<tr>
<th>序号</th>
<th>字段/名称</th>
<th>类型</th>
<th>业务类型</th>
<th>操作</th>
</tr>

<%
    string fk_mapdata = this.Request.QueryString["FK_MapData"];
    if (BP.DA.DataType.IsNullOrEmpty(fk_mapdata))
        fk_mapdata = "ND101";

    string fk_node = this.Request.QueryString["FK_Node"];
    if (BP.DA.DataType.IsNullOrEmpty(fk_node))
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
        <td><%=idx%></td>
        <td > <input  type="checkbox" id="<%=attr.KeyOfEn %>"   value="<%=attr.KeyOfEn %>  - <%=attr.KeyOfEn %>"  name="<%=attr.KeyOfEn %>" />  <%=attr.KeyOfEn%>  - <%=attr.Name%></td>
         <td><%=attr.MyDataTypeStr%></td>
<td><%=attr.LGTypeT %></td>
<td>无</td>
        </tr>
  <% } %>


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
        <td > <%=idx %></td>
        <td colspan=2>
        <input  type="checkbox" id="<%=img.MyPK %>" name="<%=img.MyPK %>"   value="<%=img.MyPK %>"  />  <%=img.MyPK%>  - <%=img.Name%>
        </td>

        <td> 公章</td>
        <td>无 </td>
        </tr>
        <%
    }
%>

<!-- 历史设置. -->
<tr>
<td colspan="6" > 已经设定的权限</td>
</tr>

<tr>
<td colspan="6">
<ul>
<% 
    BP.Sys.MapExts exts = new BP.Sys.MapExts();
    int mecount =  exts.Retrieve(BP.Sys.MapExtAttr.FK_MapData, fk_mapdata, 
        BP.Sys.MapExtAttr.Tag, this.Request.QueryString["FK_Node"],
        BP.Sys.MapExtAttr.ExtType, "SepcFiledsSepcUsers");
                                    

    foreach (BP.Sys.MapExt item in exts)
    {
        %>
       <li> <a href="javascript:OpenIt('<%=fk_mapdata %>','<%=fk_node %>','<%=item.MyPK %>','Fileds')">设置:<%=item.Doc %></a>  - <%=item.Tag1 %>-<%=item.Tag2 %>-<%=item.Tag3 %></li>
        
  <%  } %>
</ul>
</td>
</tr>


<tr>
<td colspan="6">
<input type="button"  value="设置特别权限"  onclick="Save('<%=fk_mapdata %>','<%=fk_node %>','Fileds')" />
 <%--   <asp:Button ID="Btn_Save" runat="server" Text="批量设置" />--%>
    </td>
</tr>
</table>
</fieldset>


<fieldset>
<legend>特别附件特别权限</legend>
<table> 
<tr>
<th>序号</th>
<th>附件</th>
</tr>
<%
    idx = 0;
    BP.Sys.FrmAttachments aths = new BP.Sys.FrmAttachments(fk_mapdata);
    foreach (BP.Sys.FrmAttachment ath in aths)
    {
        idx++;
        %>
        <tr>
        <td> <%=idx %></td>
        <td>
        <input  type="checkbox" id="Checkbox1" name="<%=ath.MyPK %>"   value="<%=ath.MyPK %>"  />  <%=ath.MyPK%>  - <%=ath.Name%>
        </td>

        </tr>
        <%
    }
%>


<tr>
<td colspan="6">
<ul>
<% 
    exts = new BP.Sys.MapExts();
    exts.Retrieve(BP.Sys.MapExtAttr.FK_MapData, fk_mapdata, 
        BP.Sys.MapExtAttr.Tag, this.Request.QueryString["FK_Node"],
        BP.Sys.MapExtAttr.ExtType, "SepcAthSepcUsers");
    foreach (BP.Sys.MapExt item in exts)
    {
        %>
       <li> <a href="javascript:OpenIt('<%=fk_mapdata %>','<%=fk_node %>','<%=item.MyPK %>','Ath')">设置:<%=item.Doc %></a>  - <%=item.Tag1 %>-<%=item.Tag2 %>-<%=item.Tag3 %></li>
        
  <%  } %>
</ul>
</td>
</tr>


<tr>
<td colspan="2">
   <input type="button"  value="设置附件特别权限"  onclick="Save('<%=fk_mapdata %>','<%=fk_node %>','Ath')" />
 </td>
</tr>


</table>
</fieldset>

<script  type="text/javascript">

    function OpenIt(mapdata, fk_node, extpk) {
        var url = 'SepcFiledsSepcUsersDtl.aspx?s=3&FK_Node=' + fk_node + '&FK_MapData=' + mapdata + '&MyPK=' + extpk;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
        if (b != null) {
            window.location.href = window.location.href;
        }
    }

    function Save(fk_mapdata, fk_node,type) {

        //获得选择的字段.
        var arrObj = document.all;
        var fields = '';
        for (var i = 0; i < arrObj.length; i++) {

            if (typeof arrObj[i].type != "undefined" && arrObj[i].type == 'checkbox') {
                var cid = arrObj[i].name + ',';
                if (arrObj[i].checked) {

                    if (type == "Ath") {  //如果是附件类型.
                        if (cid.indexOf('_At') < 0)
                            continue;
                    }

                    if (type == "Fields") {  //如果是字段类型.
                        if (cid.indexOf('_At') > 0)
                            continue;
                    }  


                    fields += arrObj[i].name + ',';
                }
            }
        }

        if (fields == '') {
            alert('请选择要批量设置的字段');
            return;
        }

        var url = 'SepcFiledsSepcUsersDtl.aspx?s=3&FK_Node=' + fk_node + '&FK_MapData=' + fk_mapdata + '&Fields=' + fields + '&DoType=' + type;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');

        if (b != null) {
            window.location.href = window.location.href;
        }
    }
</script>

</asp:Content>
