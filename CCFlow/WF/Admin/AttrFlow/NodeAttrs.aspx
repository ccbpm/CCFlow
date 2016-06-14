<%@ Page Title="" Language="C#" MasterPageFile="../WinOpen.master" AutoEventWireup="true"
    CodeBehind="NodeAttrs.aspx.cs" Inherits="CCFlow.WF.Admin.AttrFlow.NodeAttrs" %>

<%@ Register Src="../Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
   
    <script type="text/javascript">
        //节点属性.
        function OpenAttr(nodeID) {
            var url = "../../Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.NodeSheet&PK=" + nodeID;
            window.open(url, 'att1r', 'height=590, width=1030, top=200, left=200, toolbar=no, menubar=no, scrollbars=yes, resizable=yes, location=no, status=no');
        }
        // 表单方案.
        function OpenFrmAttr(nodeID) {
            var url = "../AttrNode/NodeFromWorkModel.aspx?FK_Node=" + nodeID;
           // window.open(url, 'OpenFrmAttr', 'height=670, width=997, top=200, left=200, toolbar=no, menubar=no, scrollbars=yes, resizable=yes, location=no, status=no');
            window.parent.addTab('OpenFrmAttr', '自由表单', url);
        }

        // 设计自由表单
        function OpenFreeFrm(nodeID) {
            var url = "../CCFormDesigner/FormDesigner.aspx?FK_MapData=ND" + nodeID;
           // window.open(url, 'OpenFreeFrm', 'height=auto, width=auto, top=200, left=200, toolbar=no, menubar=no, scrollbars=yes, resizable=yes, location=no, status=no');
        }

        // URL
        function OpenUrl(url) {
            window.open(url, 'attr', 'height=400, width=500, top=200, left=200, toolbar=no, menubar=no, scrollbars=no, resizable=yes, location=no, status=no');
        }
        // 审核组件
        function OpenAuditModule(nodeID) {
            var url = "/WF/Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.FrmWorkCheck&PK=" + nodeID;
            window.open(url, 'OpenFreeFrm', 'height=470, width=840, top=200, left=200, toolbar=no, menubar=no, scrollbars=no, resizable=yes, location=no, status=no');
        }

        //显示处理人.
        function FindWorker(nodeID) {
            var url = "../FindWorker/NodeAccepterRole.aspx?FK_Node=" + nodeID;
            window.open(url, 'FindWorker', 'height=720, width=1200, top=200, left=200, toolbar=no, menubar=no, scrollbars=no, resizable=yes, location=no, status=no');
        }
        function FindWorkerByStations(nodeID) {
            var url = "../../Comm/RefFunc/Dot2DotSingle.aspx?EnsName=BP.WF.Template.Selectors&EnName=BP.WF.Template.Selector&AttrKey=BP.WF.Template.NodeStations&NodeID=" + nodeID + "&r=1120035051&ShowWay=None";
            window.open(url, 'FindWorkerByStations', 'height=540, width=770, top=200, left=200, toolbar=no, menubar=no, scrollbars=yes, resizable=yes, location=no, status=no');
        }
        function FindWorkerByDepts(nodeID) {
            var url = "../../Comm/RefFunc/Dot2DotSingle.aspx?EnsName=BP.WF.Template.Selectors&EnName=BP.WF.Template.Selector&AttrKey=BP.WF.Template.NodeDepts&NodeID=" + nodeID + "&r=1120035051&ShowWay=None";
            window.open(url, 'FindWorkerByEmps', 'height=540, width=770, top=200, left=200, toolbar=no, menubar=no, scrollbars=no, resizable=yes, location=no, status=no');
        }
        function FindWorkerByEmps(nodeID) {
            var url = "../../Comm/RefFunc/Dot2DotSingle.aspx?EnsName=BP.WF.Template.Selectors&EnName=BP.WF.Template.Selector&AttrKey=BP.WF.Template.NodeEmps&NodeID=" + nodeID + "&r=1120035051&ShowWay=FK_Emp";
            window.open(url, 'FindWorkerByEmps', 'height=540, width=770, top=200, left=200, toolbar=no, menubar=no, scrollbars=no, resizable=yes, location=no, status=no');
        }

        function NodeCCRole(nodeID) {
            var url = "../FindWorker/NodeCCRole.aspx?FK_Node=" + nodeID;
            window.open(url, 'NodeCCRole', 'height=560, width=660, top=200, left=200, toolbar=no, menubar=no, scrollbars=no, resizable=yes, location=no, status=no');
        }

        function Action(nodeID, flowNo) {
            var url = "../Action.aspx?NodeID=" + nodeID + '&FK_Flow=' + flowNo;
            window.open(url, 'Action', 'height=auto, width=1050, top=200, left=200, toolbar=no, menubar=no, scrollbars=no, resizable=yes, location=no, status=no');
            window.parent.addTab('Action', '消息&事件', url);

        }

        function Cond(nodeID, flowNo) {
            var url = "../Cond.aspx?FK_MainNode=" + nodeID + '&FK_Node=' + nodeID + '&ToNodeID=' + nodeID + '&CondType=1&FK_Flow=' + flowNo;
          //  window.open(url, 'Cond', 'height=545, width=820, top=200, left=200, toolbar=no, menubar=no, scrollbars=no, resizable=yes, location=no, status=no');

            window.parent.addTab('Cond', '流程完成条件', url);

        }
        function Listen(nodeID, flowNo) {
            var url = "../Listen.aspx?FK_Node=" + nodeID + "&DoType=New";
          //  window.open(url, 'Listen', 'height=680, width=730, top=200, left=200, toolbar=no, menubar=no, scrollbars=no, resizable=yes, location=no, status=no');
            window.parent.addTab('Listen', '消息收听', url);
        }

        // 按钮权限配置.
        function OpenButtonControl(nodeID) {
            var url = "../../Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.BtnLab&PK=" + nodeID;
           // window.open(url, 'OpenFrmAttr', 'height=auto, width=810, top=200, left=200, toolbar=no, menubar=no, scrollbars=no, resizable=yes, location=no, status=no');
            window.parent.addTab('Btns', '按钮权限', url);
        }

        // 按钮权限配置.
        function OpenSubFlows(nodeID) {
            var url = "../AttrNode/SubFlows.aspx?FK_Node=" + nodeID;
            window.parent.closeTab('SubFlows');
            window.parent.addTab('SubFlows', '调起子流程', url, 'icon-SubFlows');
        }

        function Refurbish() {
            window.location.href = window.location.href;
        }
    
    
    </script>
    <style type="text/css">
        .Icon
        {
            width: 16px;
            height: 16px;
        }
    </style>
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%
        string flowNo = this.Request.QueryString["FK_Flow"];
        BP.WF.Nodes nds = new BP.WF.Nodes(flowNo);
        BP.WF.Flow fl = new BP.WF.Flow(flowNo);
        
        //获得节点传递过来的要突出选择的节点。
        int selectNodeID = 0;
        string nodeIDStr = this.Request.QueryString["FK_Node"];
        if (string.IsNullOrEmpty(nodeIDStr))
            selectNodeID = 0;
        else
            selectNodeID = int.Parse(nodeIDStr);
    %>
    <table width="100%">
        <caption>
            流程[<%=fl.Name %>] - 节点属性面板
            <div style="float:right">
                <a href="javascript:Refurbish();" > <img src="../../Img/Btn/Refurbish.gif" alt="刷新" />刷新 </a>
            </div>
        </caption>
        <tr>
            <th>序</th>
            <th><img src="../CCBPMDesigner/Img/Menu/Property.png" alt="节点名称" class="Icon" />节点属性</th>
            <th><img src="../../Img/Form.png" alt="设置表单的工作方式" class="Icon" />表单方案</th>
            <th><img src="../../Img/Btn/SelectAll.gif" alt="节点类型" class="Icon" />节点类型</th>
            <th><img src="../CCBPMDesigner/Img/Menu/Sender.png" alt="设置处理人" class="Icon" />设置处理人</th>
            <th><img src="../CCBPMDesigner/Img/Menu/CC.png" alt="设置处理人" class="Icon" />设置抄送人</th>
            <th><img src="../../Img/Event.png" alt="消息&事件" class="Icon" />消息&事件</th>
            <th><img src="../CCBPMDesigner/Img/Menu/Cond.png" alt="流程完成条件" class="Icon" />流程完成条件</th>
            <th><img src="../CCBPMDesigner/Img/Menu/Listion.png" alt="消息收听" class="Icon" />消息收听</th>
            <th><img src="../CCBPMDesigner/Img/Menu/CheckFlow.png" alt="按钮权限控制" class="Icon" />按钮权限</th>
            <th><img src="../CCBPMDesigner/Img/Menu/SubFlows.png" alt="父子流程" class="Icon" />子流程</th>
        </tr>
        <%
     
            int idx = 0;
            foreach (BP.WF.Node nd in nds)
            {
                // BP.WF.Template.NodeSheet ns = new BP.WF.Template.NodeSheet(nd.NodeID);
                idx++;
                bool isEnableFrmCheck = false;
                if (BP.DA.DBAccess.RunSQLReturnTable("SELECT NodeID FROM WF_Node WHERE (FWCSta=1 OR FWCSta=2) AND NodeID=" + nd.NodeID + " ").Rows.Count >= 1)
                    isEnableFrmCheck = true;

                BP.WF.Template.NodeSheet NS = new BP.WF.Template.NodeSheet(nd.NodeID);
            
            
        %>
        <tr onmouseover='TROver(this)' onmouseout='TROut(this)'>
            <td class="Idx">
                <%=idx %>
            </td>


            <!-- 节点属性 -->
            <% if (selectNodeID == nd.NodeID)
               { %>
            <td>
            <b>
                <a href="javascript:OpenAttr('<%=nd.NodeID %>');">
                    <%=nd.NodeID%>
                    <%=nd.Name%>
                </a>
                </b>
            </td>
            <% }
               else
               { %>

                 <td>
                <a href="javascript:OpenAttr('<%=nd.NodeID %>');">
                    <%=nd.NodeID%>
                    <%=nd.Name%>
                </a>
            </td>

            <%} %>


            <td>
                <a href="javascript:OpenFrmAttr('<%=nd.NodeID %>');">
                    <%=nd.HisFormTypeText%>
                </a>
               <%-- <% if (nd.HisFormType == BP.WF.NodeFormType.FreeForm && nd.NodeFrmID == "ND" + nd.NodeID)  // 自由表单.
                   { %>
                <a href="javascript:OpenFreeFrm('<%=nd.NodeID %>');">设计<%=nd.NodeFrmID %></a>
                <% } %>
                <% if (nd.HisFormType == BP.WF.NodeFormType.FixForm && nd.NodeFrmID == "ND" + nd.NodeID)  //傻瓜表单.
                   { %>
                <a href="javascript:OpenFixFrm('<%=nd.NodeID %>');">设计<%=nd.NodeFrmID %></a>
                <% } %>
                <% if (nd.HisFormType == BP.WF.NodeFormType.SDKForm || nd.HisFormType == BP.WF.NodeFormType.SelfForm)  //SDK表单.
                   { %>
                <a href="javascript:OpenUrl('<%=nd.FormUrl %>');" title="<%=nd.FormUrl %>">打开URL</a>
                <% } %>
                <% if (nd.HisFormType == BP.WF.NodeFormType.FreeForm || nd.HisFormType == BP.WF.NodeFormType.FixForm || isEnableFrmCheck == true)  // 自由表单.
                   { %>
                <a href="javascript:OpenAuditModule('<%=nd.NodeID %>');" title="<%=nd.FormUrl %>">审核组件</a>
                <%} %>--%>
            </td>
            <td>
                <%=nd.HisRunModelT%>
            </td>
            <td>
                <!-- 接受人 -->
                <a href="javascript:FindWorker('<%=nd.NodeID %>');">
                    <%=nd.HisDeliveryWayText%>
                </a>
                <% if (nd.HisDeliveryWay == BP.WF.DeliveryWay.ByStation)
                   {
                       BP.WF.Template.NodeStations nss = new BP.WF.Template.NodeStations();
                       nss.Retrieve(BP.WF.Template.NodeStationAttr.FK_Node, nd.NodeID);
                %>
                <a href="javascript:FindWorkerByStations('<%=nd.NodeID %>');">岗位(<%=nss.Count%>)</a>
                <%} %>
                <% if (nd.HisDeliveryWay == BP.WF.DeliveryWay.ByDept)
                   {
                       BP.WF.Template.NodeDepts nss = new BP.WF.Template.NodeDepts();
                       nss.Retrieve(BP.WF.Template.NodeDeptAttr.FK_Node, nd.NodeID);
                %>
                <a href="javascript:FindWorkerByDepts('<%=nd.NodeID %>');">部门(<%=nss.Count%>)</a>
                <%} %>
                <% if (nd.HisDeliveryWay == BP.WF.DeliveryWay.ByBindEmp)
                   {
                       BP.WF.Template.NodeEmps nes = new BP.WF.Template.NodeEmps();
                       nes.Retrieve(BP.WF.Template.NodeStationAttr.FK_Node, nd.NodeID);
                %>
                <a href="javascript:FindWorkerByEmps('<%=nd.NodeID %>');">人员(<%=nes.Count%>)</a>
                <%} %>
            </td>
            <!-- 抄送人 -->
            <td>
                <a href="javascript:NodeCCRole('<%=nd.NodeID %>');">
                    <%=nd.HisCCRoleText%>
                </a>
            </td>
            <!--  消息&事件 -->
            <td>
                <%
                BP.Sys.FrmEvents fes = new BP.Sys.FrmEvents();
                fes.Retrieve(BP.Sys.FrmEventAttr.FK_MapData, "ND" + nd.NodeID);
                %>
                <a href="javascript:Action('<%=nd.NodeID %>','<%=flowNo %>');">消息&事件(<%=fes.Count %>)</a>
            </td>
            <!--  流程完成条件 -->
            <td>
                <%
                    BP.WF.Template.Conds conds = new BP.WF.Template.Conds(BP.WF.Template.CondType.Flow, nd.NodeID);
                %>
                <a href="javascript:Cond('<%=nd.NodeID %>','<%=flowNo %>');">流程完成条件(<%=conds.Count%>)</a>
            </td>
            <!-- 消息收听 -->
            <td>
                <%
                    BP.WF.Template.Listens lns = new BP.WF.Template.Listens(nd.NodeID);  
                %>
                <a href="javascript:Listen('<%=nd.NodeID %>','<%=flowNo %>');">消息收听(<%=lns.Count%>)</a>
            </td>
            <!-- 按钮权限控制 -->
            <td>
                <a href="javascript:OpenButtonControl('<%=nd.NodeID %>');">设置</a>
            </td>
            <!-- 吊起子流程 -->
            <td>
                <a href="javascript:OpenSubFlows('<%=nd.NodeID %>');">子流程</a>
            </td>
        </tr>
        <%
}
        %>
        <tr class="GroupField">
            <th colspan=11>
            </th>
           
        </tr>
    </table>
</asp:Content>
