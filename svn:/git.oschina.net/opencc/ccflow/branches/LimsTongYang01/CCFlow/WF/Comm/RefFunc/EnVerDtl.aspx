<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Comm/RefFunc/WinOpen.Master"
    AutoEventWireup="true" CodeBehind="EnVerDtl.aspx.cs" Inherits="CCFlow.WF.Comm.RefFunc.EnVerDtl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%
    
        string enName = this.Request.QueryString["EnName"];
        string pkVal = this.Request.QueryString["PK"];

        if (string.IsNullOrWhiteSpace(enName) || string.IsNullOrWhiteSpace(pkVal))
        {
    %>
    <h3>
        参数EnName，PK不能为空！</h3>
    <%
        }
        else
        {
            BP.Sys.EnVerDtls dtls = new BP.Sys.EnVerDtls();
            dtls.GetNewEntity.CheckPhysicsTable();

            dtls.Retrieve(BP.Sys.EnVerDtlAttr.EnVerPK, enName + "_" + pkVal, BP.Sys.EnVerDtlAttr.EnVer);

            Dictionary<int, List<BP.Sys.EnVerDtl>> chgs = new Dictionary<int, List<BP.Sys.EnVerDtl>>();

            foreach (BP.Sys.EnVerDtl dtl in dtls)
            {
                if (chgs.ContainsKey(dtl.EnVer) == false)
                    chgs.Add(dtl.EnVer, new List<BP.Sys.EnVerDtl>());

                if (dtl.OldVal != dtl.NewVal)
                    chgs[dtl.EnVer].Add(dtl);
            }

    %>
    <table class="Table" width="100%" cellspacing="1" cellpadding="0" border="1">
        <tr>
            <td colspan="5" class="GroupTitle">
                历史记录修改版本记录
            </td>
        </tr>
        <%
            foreach (KeyValuePair<int, List<BP.Sys.EnVerDtl>> chg in chgs)
            {
                if (chg.Value.Count == 0) continue;
        %>
        <tr>
            <td colspan="5" class="GroupTitle">
                修改时间：<%=chg.Value[0].RDT %>
            </td>
        </tr>
        <tr>
        <td class="GroupTitle" style="text-align:center">序</td>
        <td class="GroupTitle">字段名称</td>
        <th class="GroupTitle">旧值</th>
        <th class="GroupTitle">新值</th>
        <th class="GroupTitle">修改人</th>
        </tr>
        <%
                int i = 1;
                foreach (BP.Sys.EnVerDtl dtl in chg.Value)
                {
        %>
        <tr>
            <td class="Idx" style="width:40px">
                <%=i++%>
            </td>
            <td>
                <%=dtl.AttrName%>
            </td>
            <td>
                <%=dtl.OldVal??string.Empty%>
            </td>
            <td>
                <%=dtl.NewVal??string.Empty%>
            </td>
            <td>
                <%=dtl.Rec%>
            </td>
        </tr>
        <%
            }
            }
        %>
    </table>
    <%
        }
    %>
</asp:Content>
