<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Rpt2DBAs.aspx.cs" Inherits="CCOA.WF.Comm.Rpt2DBAs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        #designInfo
        {
            width: auto;
            height: 95px;
            font-size: 14px;
            padding-top: 10px;
            background-color: #AFD8F8;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div width="100%">
        <%
            string rpt2Name = this.Request.QueryString["Rpt2Name"];
            BP.Rpt.Rpt2Base rpt = BP.En.ClassFactory.GetRpt2Base(rpt2Name);
            if (rpt.AttrsOfGroup.Count == 1)
            {
                this.Response.Redirect("Rpt2DBA.aspx?rpt2Name=" + rpt2Name, true);
                return;
            }

            int idx = -1;
            foreach (BP.Rpt.Rpt2Attr attr in rpt.AttrsOfGroup)
            {
                idx++;
                if (attr.ColumnChartShowType.ToString() == "HengXiang")
                {
        %>
        <iframe src='Rpt2DBA.aspx?Rpt2Name=<%= rpt2Name %>&Idx=<%= idx %>' width="100%" height="970px"
            style="border: none;"></iframe>
        <%
}
                else
                { 
        %>
        <iframe src='Rpt2DBA.aspx?Rpt2Name=<%= rpt2Name %>&Idx=<%= idx %>' width="100%" height="470px"
            style="border: none;"></iframe>
        <%
}
            }
        %>
        <%  //读取webConfig配置的底部信息
            string FootInformation = BP.Sys.SystemConfig.AppSettings["FootInformation"]; %>
        <div id='designInfo'>
            <div align='center'>
                <%=FootInformation%>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
