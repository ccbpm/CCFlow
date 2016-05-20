<%@ Page Title="流程数据字段视图" Language="C#" MasterPageFile="../WinOpen.master" AutoEventWireup="true" CodeBehind="FlowFields.aspx.cs" Inherits="CCFlow.WF.Admin.AttrFlow.FlowFields" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <%
        string flowNo = this.Request.QueryString["FK_Flow"];
        BP.Sys.MapAttrs attrs = new BP.Sys.MapAttrs("ND" + int.Parse(flowNo) + "Rpt");
    %>

    <table style="width: 100%">
        <caption>流程数据字段视图</caption>
        <tr>
            <th>序</th>
            <th>字段名</th>
            <th>字段中文名称</th>
            <th>最小长度</th>
            <th>最大长度</th>
            <th>业务类型</th>
            <th>其他</th>
        </tr>

        <tr>
            <th colspan="7">流程系统字段</th>
        </tr>

        <% 
            int idx1 = 0;
            foreach (BP.Sys.MapAttr item in attrs)
            {
                switch (item.KeyOfEn)
                {
                    case BP.WF.Data. GERptAttr.OID:
                    case BP.WF.Data. GERptAttr.AtPara:
                    case BP.WF.Data. GERptAttr.BillNo:
                    case BP.WF.Data. GERptAttr.FID:
                    case BP.WF.Data. GERptAttr.FK_Dept:
                    case BP.WF.Data. GERptAttr.FK_NY :
                    case BP.WF.Data. GERptAttr.FlowDaySpan:
                    case BP.WF.Data. GERptAttr.FlowEmps:
                    case BP.WF.Data. GERptAttr.FlowEnder:
                    case BP.WF.Data. GERptAttr.FlowEnderRDT:
                    case BP.WF.Data. GERptAttr.FlowEndNode :
                    case BP.WF.Data. GERptAttr.FlowNote:
                    case BP.WF.Data. GERptAttr.FlowStarter:
                    case BP.WF.Data. GERptAttr.FlowStartRDT:
                    case BP.WF.Data. GERptAttr.GuestName:
                    case BP.WF.Data. GERptAttr.GuestNo:
                    case BP.WF.Data. GERptAttr.GUID :
                    case BP.WF.Data. GERptAttr.MyNum :
                    case BP.WF.Data. GERptAttr.PEmp:
                    case BP.WF.Data. GERptAttr.PFID:
                    case BP.WF.Data. GERptAttr.PFlowNo:
                    case BP.WF.Data. GERptAttr.PNodeID:
                    case BP.WF.Data. GERptAttr.PrjName: 
                    case BP.WF.Data. GERptAttr.PrjNo :
                    case BP.WF.Data. GERptAttr.PWorkID:
                    case BP.WF.Data. GERptAttr.Title:
                    case BP.WF.Data. GERptAttr.WFSta:
                    case BP.WF.Data.GERptAttr.WFState:
                        
                        break;
                    default:
                        continue;
                }
                idx1++;
        %>
        <tr>
            <td class="Idx"><%=idx1 %> </td>
            <td><%=item.KeyOfEn %> </td>
            <td><%=item.Name %>   </td>
            <td><%=item.MinLen %> </td>
            <td><%=item.MaxLen %> </td>
            <td><%=item.MyDataTypeStr %> </td>
            <td><%=item.UIBindKey %> </td>
        </tr>

        <%   
    }
        %>
        <tr>
        </tr>
        <tr>
            <th colspan="7">业务字段-普通字段</th>
        </tr>

        <% 
            int idx2 = 0;
            foreach (BP.Sys.MapAttr item in attrs)
            {
                switch (item.LGType)
                {
                    case BP.En.FieldTypeS.Normal:
                        break;
                    default:
                        continue;
                }
                idx2++;
        %>
        <tr>
            <td class="Idx"><%=idx2 %> </td>
            <td><%=item.KeyOfEn %> </td>
            <td><%=item.Name %>   </td>
            <td><%=item.MinLen %> </td>
            <td><%=item.MaxLen %> </td>
            <td><%=item.MyDataTypeStr %> </td>
            <td><%=item.UIBindKey %> </td>
        </tr>

        <%   
    }
        %>
        <tr>
        </tr>
        <tr>
            <th colspan="7">业务字段-枚举字段</th>
        </tr>

        <% 
            int idx3 = 0;
            foreach (BP.Sys.MapAttr item in attrs)
            {
                switch (item.LGType)
                {
                    case BP.En.FieldTypeS.Enum:
                        break;
                    default:
                        continue;
                }
                idx3++;
        %>

        <tr>
            <td class="Idx"><%=idx3 %> </td>
            <td><%=item.KeyOfEn %> </td>
            <td><%=item.Name %>   </td>
            <td><%=item.MinLen %> </td>
            <td><%=item.MaxLen %> </td>
            <td><%=item.MyDataTypeStr %> </td>
            <td><%=item.UIBindKey %> </td>
        </tr>

        <%   
    }
        %>
        <tr>
        </tr>


        <tr>
            <th colspan="7">业务字段-外键字段</th>
        </tr>

        <% 
            int idx4 = 0;
            foreach (BP.Sys.MapAttr item in attrs)
            {
                switch (item.LGType)
                {
                    case BP.En.FieldTypeS.FK:
                        break;
                    default:
                        continue;
                }
                idx4++;
               
        %>

        <tr>
            <td class="Idx"><%=idx4 %> </td>
            <td><%=item.KeyOfEn %> </td>
            <td><%=item.Name %>   </td>
            <td><%=item.MinLen %> </td>
            <td><%=item.MaxLen %> </td>
            <td><%=item.MyDataTypeStr %> </td>
            <td><%=item.UIBindKey %> </td>
        </tr>

        <%   
    }
        %>
        <tr>
        </tr>


        <tr>
            <th colspan="7">
                <div style="float: right">如果您想把该表的数据实时的同步到您指定的表，请执行【<a href="DTSBTable.aspx?FK_Flow=<%=flowNo %>">与业务数据表同步</a>】</div>
            </th>
        </tr>

    </table>



</asp:Content>
