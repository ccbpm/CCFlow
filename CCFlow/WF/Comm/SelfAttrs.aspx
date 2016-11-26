<%@ Page Title="自定义属性" Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" CodeBehind="SelfAttrs.aspx.cs" Inherits="CCFlow.Comm.SelfAttrs" %>
<%@ Register src="UC/Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Style/Table.css" rel="stylesheet" type="text/css" />
    <link href="Style/Table0.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<%
    string ensName = this.Request.QueryString["EnsName"];
    if (ensName == null)
        ensName = "BP.Port.Depts";
    else
        ensName = "BP.Port.Depts";

    BP.En.Entities ens = BP.En.ClassFactory.GetEns(ensName);
    BP.En.Entity en = ens.GetNewEntity;
    
    //执行.
    string doType = this.Request.QueryString["DoType"];
    BP.Sys.MapAttr attrEn = null;
    switch (doType)
    {
        case "Del":
            attrEn = new BP.Sys.MapAttr();
            attrEn.MyPK = this.Request.QueryString["MyPK"];
            attrEn.Delete();
            break;
        case "Up":
            attrEn = new BP.Sys.MapAttr();
            attrEn.MyPK = this.Request.QueryString["MyPK"];
            attrEn.DoUp();
            break;
        case "Down":
            attrEn = new BP.Sys.MapAttr();
            attrEn.MyPK = this.Request.QueryString["MyPK"];
            attrEn.DoDown();
            break;
        default:
            break;
    }
    
     
    
     %>
<table>
<caption><% =en.EnDesc %>  -  <a href="javascript:AddAttr('<%=ensName %>')">增加属性</a> - <a href="Search.aspx?EnsName=<%=ensName %>" >返回</a></caption>
<tr>
<th>序号</th>
<th>中文名</th>
<th>字段</th>
<th>类型</th>
<th>最大长度</th>
<th>相关</th>
<th>操作</th>
</tr>
<%
    BP.Sys.MapAttrs attrs = new BP.Sys.MapAttrs(ensName);
    int idx = 0;
    foreach (BP.Sys.MapAttr attr in attrs)
    {
      idx++;
 %>
    <tr>
<TD class="Idx" ><%=idx %></TD>
<TD><% =attr.Name %></TD>
<TD><% =attr.KeyOfEn %></TD>
<TD><% =attr.LGTypeT %></TD>
<TD><% =attr.MaxLen  %></TD>
<TD><% =attr.UIBindKey  %></TD>
<TD>[<a href="javascript:Del('<%=attr.MyPK %>')">删除</a>][<a href="javascript:Up('<%=attr.MyPK %>')">上移</a>][<a href="javascript:Down('<%=attr.MyPK %>')">下移</a>]</TD>
</tr>
<%  
    }
    %>
</table>

 <script type="text/javascript">
     function AddAttr(ensName) {

         var url = '../Admin/FoolFormDesigner/FieldTypeList.aspx?DoType=AddF&FK_MapData=' + ensName;
         var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
         window.location.href = window.location.href;

       //  var url = '?EnsName=<%=ensName %>';
//         var url = "/WF/Admin/FoolFormDesigner/Do.aspx?DoType=AddF&MyPK=0&IDX=0&GroupField=&FK_MapData=<%=ensName%>&RefNo=<%=ensName%>";
//         alert(url);
//         var str = window.showModalDialog(url, '', 'dialogHeight: 550px; dialogWidth:950px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no');
//         window.location.href = window.location.href;
     }
        function Del(mypk) {
            if (alert('您确定要删除吗?') == false)
                return;
          var url='?EnsName=<%=ensName %>&DoType=Del&MyPK='+mypk;
          window.location.href = url;
        }
        function Up(mypk) {
            var url = '?EnsName=<%=ensName %>&DoType=Up&MyPK=' + mypk;
            window.location.href = url;
        }
        function Down(mypk) {
            var url = '?EnsName=<%=ensName %>&DoType=Down&MyPK=' + mypk;
            window.location.href = url;
        }
    </script>
</asp:Content>
