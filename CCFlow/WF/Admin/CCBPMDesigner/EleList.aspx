<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EleList.aspx.cs" Inherits="CCFlow.WF.Admin.CCBPMDesigner.EleList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>BPMN2.0元素列表</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <%
        BP.BPMN.EleTypeXmls typs = new BP.BPMN.EleTypeXmls();
        typs.RetrieveAll();
        
        BP.BPMN.EleListXmls xmls = new BP.BPMN.EleListXmls();
        xmls.RetrieveAll();
     %>

    <%
        foreach (BP.BPMN.EleTypeXml myItem in typs)
        {
          %>
            <%=myItem.Name %>   <%=myItem.No %>

     
   <ul>
    <%
        foreach (BP.BPMN.EleListXml item in xmls)
        {
            if (item.EleType != myItem.No)
                continue;
            
          %> <li> <img alt='<%=myItem.EventDesc %>' src='./Icons/<%=myItem.No %>/<%=item.No %>.png' /> <%=item.Name%>  <%=myItem.EleType %> </li> <%
        }
            %>
             </ul>
       <% } %>
   
    </div>
    </form>
</body>
</html>
