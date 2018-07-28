<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmVSTO.aspx.cs" Inherits="CCFlow.WF.CCForm.FrmVSTO" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<meta charset="UTF-8" />
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
</head>
<body>
	<form id="form1" runat="server">
	<div>
		<%
		
			//string userNo = this.Request.QueryString["UserNo"];
			//string sid = this.Request.QueryString["SID"];
			//string nodeID = this.Request.QueryString["FK_Node"];
			//string flowNo = this.Request.QueryString["FK_Flow"];
			//string workID = this.Request.QueryString["WorkID"];
			//string fid = this.Request.QueryString["FID"];
			//string frmID = this.Request.QueryString["FK_MapData"];

			string paras = this.RequestParas;
            if (paras.Contains("SID") == false)
                paras += "&SID=" + BP.Web.WebUser.SID;
		    
            if (paras.Contains("UserNo") == false)
                paras += "&UserNo=" + BP.Web.WebUser.No;

            if (string.IsNullOrWhiteSpace(BP.Sys.SystemConfig.AppSettings["IsAutoTesting"]))
                paras += "&IsAutoTesting=0";
            else
                paras += "&IsAutoTesting=" + Convert.ToInt32(BP.Sys.SystemConfig.AppSettings["IsAutoTesting"]); //用于自动化测试
                        
			paras = paras.Replace("&", ",");

			string urlWS = "http://" + this.Request.Url.Authority + "/WF/CCForm/CCFormAPI.asmx";
			string url = "excelform://-fromccflow,App=FrmExcel" + paras + ",WSUrl=" + urlWS;

			//string urlWS = "http://localhost:26507/WF/CCForm/CCFormAPI.asmx";
			//string url = "excelform://-fromccflow,App=FrmExcel,UserNo=" + userNo + ",SID=" + sid + ",FK_Flow=" + flowNo + ",FK_Node=" + nodeID + ",FrmID=" + frmID + ",WorkID="+workID+",WSUrl="+urlWS;
			///  string urlOfFree1 = "Frm.aspx?IsFreeFrm=1&UseNo="+userNo+"&SID="+sid+"&FK_MapData="+frmID+"&FK_Flow="+flowNo+"&FK_Node="+nodeID+"&FrmID="+frmID+"&WorkID="+workID+"&OID="+workID+"&FID="+fid;
			string urlOfFree = "Frm.htm?IsFreeFrm=1" + this.RequestParas;
		
		%>
		<br />
		<br />

        <fieldset>
        <legend>处理表单</legend>
		<ul>
			<li><a href="<%=url %>">打开VSTO版本excel模式的表单</a></li>
			<li><a href="<%=urlOfFree %>">打开自由表单</a></li>
		</ul>
        </fieldset>


        <fieldset>
        <legend>插件安装说明</legend>
        <ul>
          <li >该插件是一个ccflow开发的excel插件，用户打开处理excel表单的插件。</li>
          <li>该需要下载后安装</li>
		   <li><a href="http://<%= this.Request.Url.Authority %>/DataUser/FrmOfficeTemplate/Excel表单插件安装程序.zip">点击此处下载VSTO表单插件</a></li>
		</ul>
        </fieldset>
	</div>
	</form>
</body>
</html>
