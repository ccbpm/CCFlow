<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmVSTO.aspx.cs" Inherits="CCFlow.WF.CCForm.FrmVSTO" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>

    <%
        
        string userNo = this.Request.QueryString["UserNo"];
        string sid = this.Request.QueryString["SID"];
        string nodeID = this.Request.QueryString["FK_Node"];
        string flowNo = this.Request.QueryString["FK_Flow"];
        string workID = this.Request.QueryString["WorkID"];
        string fid = this.Request.QueryString["FID"];
        string frmID = this.Request.QueryString["FK_MapData"];

        string urlWS = "http://localhost:26507/WF/CCForm/CCFormAPI.asmx";
        string url = "excelform://-fromccflow,App=FrmExcel,UserNo=" + userNo + ",SID=" + sid + ",FK_Flow=" + flowNo + ",FK_Node=" + nodeID + ",FrmID=" + frmID + ",WorkID="+workID+",WSUrl="+urlWS;

        string urlOfFree = "Frm.aspx?IsFreeFrm=1&UseNo="+userNo+"&SID="+sid+"&FK_MapData="+frmID+"&FK_Flow="+flowNo+"&FK_Node="+nodeID+"&FrmID="+frmID+"&WorkID="+workID+"&OID="+workID+"&FID="+fid;
        
         %>

         <br />
         <br />


    <a href="<%=url %>" >打开VSTO表单</a>

    <a href="<%=urlOfFree %>" >打开自由表单</a>

    
    </div>
    </form>
</body>
</html>
