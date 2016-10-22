<%@ Page Title="关注" Language="C#" MasterPageFile="~/WF/WinOpen.master" AutoEventWireup="true" CodeBehind="Focus.aspx.cs" Inherits="CCFlow.WF.Focus" %>
<%@ Import Namespace="BP.Sys" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">  
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript" src="Scripts/easyUI/jquery-1.8.0.min.js"></script>
    <script type="text/javascript">
        //取消关注
        function unfollow(workid)
        {
            $.ajax({
                url: "Do.aspx?ActionType=Focus&WorkID=" + workid,
                async: false,
                success: function () {
                    window.location.href = window.location.href;
                    alert("您已取消关注！");
                }

            });
           
        }
    </script>
   
    <table style="width:100%;height:700px;border:0px">
        <caption class='CaptionMsg'>&nbsp&nbsp&nbsp 我关注的流程 </caption>
        <tr>
            <td valign="top">
                <table width="100%";>
                    <tr>
                        <th>序</th>
                        <th>流程</th>
                        <th>标题</th>
                        <th>发起人</th>
                        <th>发起日期</th>
                        <th>状态</th>
                        <th>停留节点</th>
                        <th>最后处理人</th>
                        <th>操作</th>
                    </tr>
                    <% 
                        string flowNo = this.Request.QueryString["FK_Flow"];

                        int idx = 0;
                        //获得关注的数据.
                        System.Data.DataTable dt = BP.WF.Dev2Interface.DB_Focus(flowNo, BP.Web.WebUser.No);
                        SysEnums stas = new SysEnums("WFSta");

                        foreach (System.Data.DataRow dr in dt.Rows)
                        {
                            string Title = dr["Title"].ToString();
                            string flowName = dr["FlowName"].ToString();
                            string StarterName = dr["StarterName"].ToString();
                            string RDT = dr["RDT"].ToString();
                            string Starter = dr["Starter"].ToString();
                            int wfsta = int.Parse(dr["WFSta"].ToString());
                            //edit by liuxc,2016-10-22,修复状态显示不正确问题
                            string wfstaT = (stas.GetEntityByKey(SysEnumAttr.IntKey, wfsta) as SysEnum).Lab;

                               string nodeName = dr["NodeName"].ToString();
                               string WorkID = dr["WorkID"].ToString();
                               flowNo = dr["FK_Flow"].ToString();
                      %>
                    <tr>
                        <td class="Idx" ><%=idx+1%></td>
                        <td><%=flowName%></td>
                        <td><a href="javascript:WinOpen('WFRpt.aspx?WorkID=<%=WorkID %>&FK_Flow=<%= flowNo%>')"><%=Title%></td>
                        <td><%=StarterName%></td>
                        <td><%=RDT%></td>
                        <td><%=wfstaT%></td>
                        <td><%=nodeName%></td>
                        <td><%=Starter%></td>
                        <td><a  href="#" onclick="unfollow(<%=WorkID %>)" >取消关注</a></td>
                    </tr>
                    <% }%>
                  
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
