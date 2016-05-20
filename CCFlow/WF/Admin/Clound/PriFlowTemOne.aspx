<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PriFlowTemOne.aspx.cs"
    Inherits="CCFlow.WF.Admin.Clound.PriFlowTemOne" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<div id="LoadingBar" style="margin-left: auto; margin-right: auto; margin-top: 40%;
    width: 250px; height: 38px; line-height: 38px; padding-left: 50px; padding-right: 5px;
    background: url(../../Scripts/easyUI/themes/default/images/pagination_loading.gif) no-repeat scroll 5px 10px;
    border: 2px solid #95B8E7; color: #696969; font-family: 'Microsoft YaHei'">
    正在连接到云服务器,请稍候…
</div>
<head runat="server">
    <title></title>
    <link href="../../Scripts/jquery/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/jquery/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery/jquery.easyui.min.js" type="text/javascript"></script>
    <link href="../../Comm/Style/Table.css" rel="stylesheet" type="text/css" />
    <link href="../../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <script src="js/loading.js" type="text/javascript"></script>
    <style type="text/css">
        body
        {
            margin: 0px;
            padding: 0px;
            font-size: 12px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <% 
        string guid = this.Request.QueryString["GUID"];
        BP.WF.CloudWS.WSSoapClient ccflowCloud = BP.WF.Cloud.Glo.GetSoap();

        System.Data.DataTable dt = null;

        ClientScript.RegisterClientScriptBlock(this.GetType(), "msg", "<script>alert(111);</script>");

        dt = ccflowCloud.GetPriFlowTemplateByGuid(guid);
    %>
    <table width="100%" style="margin: 0px; padding: 0px; min-width: 800px;">
        <tr>
            <td valign="top" width="42%">
                <fieldset>
                    <legend>操作</legend><a href="?DoType=Download&GUID=<%=guid %>">
                        <%
                            string doType = this.Request.QueryString["DoType"];
                            byte[] bytes = ccflowCloud.GetFlowXML(false, guid);

                            if (this.Request.QueryString["DoType"] == "Download")
                            {
                                HttpContext.Current.Response.Clear();
                                HttpContext.Current.Response.ClearHeaders();
                                HttpContext.Current.Response.BinaryWrite(bytes);

                                //火狐不需要编码 add by qin 解决下载文件时文件名乱码的问题
                                string browserType = this.Request.Browser.Type;

                                string fileName = HttpUtility.UrlEncode(dt.Rows[0]["NAME"].ToString() + ".xml", Encoding.UTF8);

                                if (browserType.Contains("Firefox"))
                                    fileName = dt.Rows[0]["NAME"].ToString() + ".xml";


                                HttpContext.Current.Response.Charset = "UTF8";
                                HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + fileName);
                                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");

                                HttpContext.Current.Response.Flush();
                                HttpContext.Current.Response.End();
                                HttpContext.Current.Response.Close();
                            }
                        %>
                        <img src="imgs/Download.png" border="0" />下载到本机</a> |<a href="PriFlowTemImp.aspx?GUID=<%=guid %>"><img
                            src="imgs/ImportFlow.png" border="0" />导入流程</a>
                </fieldset>
                <fieldset>
                    <legend>基本信息</legend>
                    <table>
                        <tr>
                            <td>
                                名称
                            </td>
                            <td>
                                <%=dt.Rows[0]["NAME"].ToString()  %>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                类别
                            </td>
                            <td>
                                <%=dt.Rows[0]["DIRNAME"].ToString()  %>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                节点数
                            </td>
                            <td>
                                <%=dt.Rows[0]["NODENUM"].ToString()  %>个
                            </td>
                        </tr>
                    </table>
                </fieldset>
                <fieldset>
                    <legend>节点信息</legend>
                    <ul style="list-style: none; padding-left: 5px;">
                        <%
                            string[] nodes = dt.Rows[0]["NodeNames"].ToString().Split('@');
                            int index = 0;
                            foreach (string nd in nodes)
                            {
                                if (string.IsNullOrEmpty(nd))
                                    continue;
                                index += 1;
                        %>
                        <li style="margin-top: 5px;"><span style="font-size: 14px; color: Green;">
                            <%=index%>、</span><%=nd %></li><%}
                            %>
                    </ul>
                </fieldset>
            </td>
            <td valign="top">
                <h2>
                    &nbsp;&nbsp;流程图：<%=dt.Rows[0]["NAME"].ToString() %></h2>
                <img src="sss.png" onerror="this.src='imgs/NoTempleteImg.png'" width="100%" />
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
