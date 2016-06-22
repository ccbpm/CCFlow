<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true"
    CodeBehind="Flows.aspx.cs" Inherits="CCFlow.WF.Admin.CCBPMDesigner.Flows" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function OpenRpt(flowNo) {
            window.parent.closeTab('设计报表');
            window.parent.addTab('OpenRpt', '设计报表', '../../Rpt/OneFlow.aspx?FK_MapData=ND' + flowNo + 'Rpt&FK_Flow=' + flowNo, '');
            //WinOpen('../../Rpt/OneFlow.aspx?FK_MapData=ND' + flowNo + 'Rpt&FK_Flow=' + flowNo, 'csd');
        }
        function FlowAttr(flowNo) {
            window.parent.closeTab('流程属性');
            window.parent.addTab('FlowAttr', '流程属性', '../../Comm/RefFunc/UIEn.aspx?EnsName=BP.WF.Template.FlowSheets&No=' + flowNo, '');
            //WinOpen('../../Comm/RefFunc/UIEn.aspx?EnsName=BP.WF.Template.FlowSheets&No=' + flowNo, 'csd');
        }
        function NodesAttr(flowNo) {
            window.parent.closeTab('节点设置');
            window.parent.addTab('NodesAttr', '节点设置', '../AttrFlow/NodeAttrs.aspx?FK_Flow=' + flowNo, '');
        }
        function OpenDel(flowNo) {
            window.parent.closeTab('删除');
            window.parent.addTab('OpenDel', '删除', '../../Comm/RefMethod.aspx?Index=4&EnsName=BP.WF.Template.FlowSheets&No=' + flowNo, '');
            //window.open('../../Comm/RefMethod.aspx?Index=4&EnsName=BP.WF.Template.FlowSheets&No=' + flowNo, 'cc', 'height=100,width=400,toolbar=no,menubar=no,scrollbars=no,resizable=no,location=no,status=no');
        }
        function OpenRollback(flowNo) {
            window.parent.closeTab('回滚');
            window.parent.addTab('OpenRollback', '回滚', '../../Comm/RefMethod.aspx?Index=12&EnsName=BP.WF.Template.FlowSheets&No=' + flowNo, '');
            //window.open('../../Comm/RefMethod.aspx?Index=12&EnsName=BP.WF.Template.FlowSheets&No=' + flowNo, 'cc', 'height=100,width=400,toolbar=no,menubar=no,scrollbars=no,resizable=no,location=no,status=no');
        }
        //设计流程
        function openFlow(flowDType, flowName, flowNo, webUserNo, webUserSID) {
            if (flowDType == 2) {//BPMN模式
                window.parent.closeTab(flowName);
                window.parent.addTab('flow', flowName, "../CCBPMDesigner/Designer.aspx?FK_Flow=" +
                        flowNo + "&UserNo=" + webUserNo + "&SID=" + webUserSID + "&Flow_V=2", '');
            } else if (flowDType == 1) {//CCBPM
                window.parent.closeTab(flowName);
                window.parent.addTab('flow', flowName, '../CCBPMDesigner/Designer.aspx?FK_Flow=' +
                  flowNo + "&UserNo=" + webUserNo + "&SID=" + webUserSID + "&Flow_V=1", '');
            } else {
                if (confirm("此流程版本为V1.0,是否执行升级为V2.0 ?")) {
                    window.parent.closeTab(flowName);
                    window.parent.addTab('flow', flowName, "../CCBPMDesigner/Designer.aspx?FK_Flow=" +
                  flowNo + "&UserNo=" + webUserNo + "&SID=" + webUserSID + "&Flow_V=0", '');
                } else {
                    window.parent.closeTab(flowName);
                    window.parent.addTab('flow', flowName, "../CCBPMDesigner/DesignerSL.aspx?FK_Flow=" +
                  flowNo + "&UserNo=" + webUserNo + "&SID=" + webUserSID + "&Flow_V=0", '');
                }
            }
        }
    </script>
    <style type="text/css">
        .HS
        {
            background-color: InfoBackground;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width: 100%; min-width: 700px;">
        <caption>
            流程控制面板
        </caption>
        <tr>
            <th rowspan="2">
                流&nbsp;程
            </th>
            <th colspan="4">
                流程实例(状态)统计
            </th>
            <th colspan="4">
                耗时分析(单位:分钟)
            </th>
            <th colspan="5">
                考核状态分布（单位:个）
            </th>
            <th rowspan="2">
                设计报表
            </th>
            <th rowspan="2">
                设计流程
            </th>
            <th rowspan="2">
                考核分析
            </th>
            <th rowspan="2">
                流程属性
            </th>
            <th rowspan="2">
                节点设置
            </th>
            <th colspan="2">
                流程实例操作
            </th>
        </tr>
        <tr>
            <th>
                运行中
            </th>
            <th>
                已经完成
            </th>
            <th>
                其他
            </th>
            <th>
                逾期
            </th>
            <th>
                最长
            </th>
            <th>
                最短
            </th>
            <th>
                平均
            </th>
            <th>
                系数
            </th>
            <th>
                及时
            </th>
            <th>
                按期
            </th>
            <th>
                逾期
            </th>
            <th>
                超期
            </th>
            <th>
                按期办结率
            </th>
            <th>
                删除
            </th>
            <th>
                回滚
            </th>
        </tr>
        <%
            //类别.
            BP.WF.Template.FlowSorts flowSorts = new BP.WF.Template.FlowSorts();
            flowSorts.RetrieveAll();

            //流程.
            BP.WF.Flows flows = new BP.WF.Flows();
            flows.RetrieveAll();




            // 获得超期时间
            System.Data.DataTable dtOverTimeMin
             = BP.DA.DBAccess.RunSQLReturnTable("SELECT FK_Flow, SUM(OverMinutes) AS OverMinutes FROM WF_CH WHERE OverMinutes > 0 GROUP BY FK_Flow  ");

            //及时完成
            System.Data.DataTable dtInTimeCount
             = BP.DA.DBAccess.RunSQLReturnTable("SELECT FK_Flow,COUNT( distinct WorkID) Num FROM WF_CH WHERE CHSta='" +
                                               (int)BP.WF.Data.CHSta.JiShi + "' GROUP BY FK_Flow ");

            //按期完成
            System.Data.DataTable dtOnTimeCount
             = BP.DA.DBAccess.RunSQLReturnTable("SELECT FK_Flow,COUNT( distinct WorkID) Num FROM WF_CH WHERE CHSta='" +
                                               (int)BP.WF.Data.CHSta.AnQi + "' GROUP BY FK_Flow ");

            // 获得逾期的工作数量.
            System.Data.DataTable dtOverTimeCount
             = BP.DA.DBAccess.RunSQLReturnTable("SELECT FK_Flow,COUNT( distinct WorkID) Num FROM WF_CH WHERE CHSta='" +
                                               (int)BP.WF.Data.CHSta.YuQi + "' GROUP BY FK_Flow ");

            // 获得超期的工作数量.
            System.Data.DataTable dtCqTimeCount
             = BP.DA.DBAccess.RunSQLReturnTable("SELECT FK_Flow,COUNT( distinct WorkID) Num FROM WF_CH WHERE CHSta='" +
                                               (int)BP.WF.Data.CHSta.ChaoQi + "' GROUP BY FK_Flow  ");


            // 获得流程状态.
            System.Data.DataTable dt
                = BP.DA.DBAccess.RunSQLReturnTable("SELECT FK_Flow, WFSta, count(WorkID) as Num FROM WF_GenerWorkFlow WHERE WFState!=0 GROUP BY FK_Flow,WFSta ");

            foreach (BP.WF.Template.FlowSort flowSort in flowSorts)
            {
        %>
        <tr class="GroupField">
            <td colspan="21">
                <%=flowSort.Name%>
            </td>
        </tr>
        <%int idx = 1;

          foreach (BP.WF.Flow flow in flows)
          {
              if (flow.FK_FlowSort != flowSort.No)
                  continue; //非该类别下的流程.

              int sta0 = 0;
              int sta1 = 0;
              int sta2 = 0;

              //获得每个状态的值.
              foreach (System.Data.DataRow dr in dt.Rows)
              {
                  if (dr["FK_Flow"].ToString() != flow.No)
                      continue;

                  if (dr["WFSta"].ToString() == "0")
                  {
                      sta0 = int.Parse(dr["Num"].ToString());
                      continue;
                  }

                  if (dr["WFSta"].ToString() == "1")
                  {
                      sta1 = int.Parse(dr["Num"].ToString());
                      continue;
                  }

                  if (dr["WFSta"].ToString() == "2")
                  {
                      sta2 = int.Parse(dr["Num"].ToString());
                      continue;
                  }
              }
        %>
        <tr>
            <td class="flowName">
                <font style="color: Blue;">
                        <%=idx%></font>、<%=flow.Name %>
            </td>
            <td class="con">
                <%--运行中  Runing=0--%>
                <%if (sta0 == 0)
                  {
                %>
                0
                <%
                  }
                  else
                  {
                %>
                <a href="javascript:window.parent.closeTab('运行中');window.parent.addTab('running', '运行中', '../../Comm/Search.aspx?EnsName=BP.WF.Data.GenerWorkFlowViews&WFSta=0&FK_NY=all&FK_Flow=<%=flow.No %>&', '');">
                    <%=sta0%></a>
                <%}%>
            </td>
            <td class="con">
                <%--已完成  Complete--%>
                <%
              if (sta1 == 0)
              {%>
                <%=sta1%>
                <%}
              else
              {%>
                <a href="javascript:window.parent.closeTab('已完成');window.parent.addTab('complete', '已完成', '../../Comm/Search.aspx?EnsName=BP.WF.Data.GenerWorkFlowViews&WFSta=1&FK_NY=all&FK_Flow=<%=flow.No %>&', '');">
                    <%=sta1%></a>
                <%}
                %>
            </td>
            <td class="con">
                <%--其他  Etc--%>
                <%
              if (sta2 == 0)
              {%>
                <%=sta2%>
                <%}
              else
              {%>
                <a href="javascript:window.parent.closeTab('其他');window.parent.addTab('other', '其他', '../../Comm/Search.aspx?EnsName=BP.WF.Data.GenerWorkFlowViews&WFSta=2&FK_NY=all&FK_Flow=<%=flow.No %>&', '');">
                    <%=sta2%></a>
                <%} %>
            </td>
            <td class="con">
                <font color="red"><b>0</b></font>
            </td>
            <td class="HS con">
                0
            </td>
            <td class="HS con">
                0
            </td>
            <td class="HS con">
                0
            </td>
            <td class="HS con">
                0
            </td>
            <td class="con">
                0
            </td>
            <td class="con">
                0
            </td>
            <td class="con">
                0
            </td>
            <td class="con">
                0
            </td>
            <td class="con">
                0
            </td>
            <td class="con">
                <a href="javascript:OpenRpt('<%=flow.No %>');">设计报表</a>
            </td>
            <td class="con">
                <a href="javascript:openFlow('<%=flow.DType %>','<%=flow.Name %>','<%=flow.No %>','<%=BP.Web.WebUser.No %>','<%=BP.Web.WebUser.SID %>');">
                    设计流程</a>
            </td>
            <td class="con">
                <a href="javascript:window.parent.closeTab('考核分析');window.parent.addTab('KHFX', '考核分析', '../../CH/OneFlow.aspx?FK_Flow=<%=flow.No %>&', '');">
                    考核分析</a>
            </td>
            <td class="con">
                <a href="javascript:FlowAttr('<%=flow.No %>');">流程属性</a>
            </td>
            <td class="con">
                <a href="javascript:NodesAttr('<%=flow.No %>');">节点设置</a>
            </td>
            <td class="con">
                <a href="javascript:OpenDel('<%=flow.No %>');">删除</a>
            </td>
            <td class="con">
                <a href="javascript:OpenRollback('<%=flow.No %>');">回滚</a>
            </td>
        </tr>
        <% idx += 1;
          }

            }
        %>
    </table>
    <style type="text/css">
        body
        {
            margin: 0px;
            padding: 0px;
        }
        th
        {
            text-align: center;
            border: 1px dotted #CCCCCC;
        }
        .flowSort
        {
            padding-left: 2px;
        }
        .flowName
        {
            padding-left: 30px;
        }
        .con
        {
            border: 1px dotted #CCCCCC;
            text-align: center;
        }
    </style>
</asp:Content>
