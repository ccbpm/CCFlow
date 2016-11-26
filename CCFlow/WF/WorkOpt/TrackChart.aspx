<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TrackChart.aspx.cs" Inherits="CCFlow.WF.WorkOpt.TrackChart" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
     <div id="silverlightControlHost" style='height: 600px;'>
                    <object data="data:application/x-silverlight-2," type="application/x-silverlight-2"
                        width="99%" height="500px">
                        <param name="source" value="../Admin/ClientBin/CCBPMDesigner.xap" />
                        <param name="onerror" value="onSilverlightError" />
                        <param name="background" value="white" />
                        <param name="minRuntimeVersion" value="2.0.31005.0" />
                        <%--	<param name="windowless" value="true"/>--%>
                        <param name="autoUpgrade" value="true" />
                        <a href="<%=BP.WF.Glo.SilverlightDownloadUrl %>">" style="text-decoration: none;">
                            <img src="http://go.microsoft.com/fwlink/?LinkId=108181" alt="Get Microsoft Silverlight"
                                style="border-style: none" />
                        </a>
                    </object>
                </div>
    </form>
</body>
</html>
