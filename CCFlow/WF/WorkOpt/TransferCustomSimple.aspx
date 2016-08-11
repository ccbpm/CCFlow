<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TransferCustomSimple.aspx.cs"
    Inherits="CCFlow.WF.WorkOpt.TransferCustomSimple" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="BP.WF" %>
<%@ Import Namespace="BP.WF.Template" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>流转自定义（Dom插入版）</title>
    <link href="../Scripts/easyUI15/themes/icon.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI15/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/easyUI15/jquery.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI15/jquery.easyui.min.js" type="text/javascript"></script>
    <link href="../Comm/JS/Calendar/skin/WdatePicker.css" rel="stylesheet" type="text/css" />
    <script language="JavaScript" src="../Comm/JS/Calendar/WdatePicker.js" defer="defer"
        type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <style type="text/css">
        .table
        {
            border: 1px outset #C0C0C0;
            padding: inherit;
            margin: 0;
            border-collapse: collapse;
        }
        th
        {
            border-width: 1px;
            border-color: #C2D5E3;
            border-style: solid;
            line-height: 25px;
            color: 0a0a0a;
            white-space: nowrap;
            padding: 0 2px;
            background-color: #e0ecff;
            font-size: 14px;
            text-align: left;
            font-size: 12px;
            font-weight: normal;
            line-height: 26px;
        }
        td
        {
            border-right-style: none;
            border-left-style: none;
            border-style: solid;
            padding: 4px;
            text-align: left;
            color: #333333;
            font-size: 12px;
            border-width: 1px;
            border-color: #C2D5E3;
        }
        .Idx
        {
            font-size: 12px;
            border: 1px solid #ccc;
            text-align: center;
            font-weight: bold;
            width: 20px;
            line-height: 26px;
        }
        .TBcalendar
        {
            background-position: left center;
            border-style: none none dotted none;
            border-width: 1px;
            border-color: #003366;
        }
    </style>
    <table class="table" cellpadding="0" cellspacing="0" style="width: 100%">
        <thead>
            <tr>
                <th style="width: 80px; text-align: center">
                    编号
                </th>
                <th style="width: 140px">
                    步骤
                </th>
                <th style="width: 240px">
                    处理人
                </th>
                <th>
                    预计处理日期
                </th>
            </tr>
        </thead>
        <tbody>
            <%
                string flowNo = Request.QueryString["flowNo"];
                if (string.IsNullOrWhiteSpace(flowNo)) flowNo = "002";
                long workid = Dev2Interface.Node_CreateBlankWork(flowNo);
                GenerWorkFlow gwf = new GenerWorkFlow(workid);
                TransferCustoms tcs = new TransferCustoms(workid);
                GenerWorkerLists gwls = new GenerWorkerLists(workid);
                Nodes nodes = new Nodes();
                nodes.Retrieve(NodeAttr.FK_Flow, flowNo, "Step");
                TransferCustom tc = null;
                string es = string.Empty;
                string load = string.Empty;
                string host = Request.Url.Scheme + "://" + Request.Url.Authority;

                foreach (Node node in nodes)
                {
                    if (node.IsStartNode == true)
                    {
            %>
            <tr>
                <td class="Idx">
                    <input type="hidden" id="hidWorkId" value="<%=workid %>" />
                    <script language="javascript" type="text/javascript">
                        var isLoad = false;

                        function saveCfg(cmb, newPlan) {
                            if(isLoad) {
                                return;
                             }

                            var ids = cmb.id.split('_');

                            $.ajax({
                                type: 'GET',
                                dataType: 'text', 
                                contentType: 'application/json; charset=utf-8',
                                url: '<%=host %>/WF/WorkOpt/TransferCustomSimple.aspx', 
                                data: {
                                    method:'savecfg',
                                    workId:<%=workid %>,
                                    nodeId:ids[1],
                                    empNos:$(cmb).combobox('getValues').toString(),
                                    empNames:$(cmb).combobox('getText'),
                                    step:ids[2],
                                    plan: newPlan ? newPlan : $('#plan_' + ids[1]).val()
                                }, 
                                error: function (XMLHttpRequest, errorThrown) {
                                    alert('错误：' + XMLHttpRequest);
                                },
                                success: function (msg) {
                                    var data = $.parseJSON(msg);
                                    if(!data.success){
                                        alert(data.msg);
                                    }
                                }
                            });
                        }
                    </script>
                    <%=node.NodeID %>
                </td>
                <td>
                    <%=node.Name %>
                </td>
                <td>
                    <%=gwf.StarterName%>
                </td>
                <td>
                    <%=gwf.RDT %>
                </td>
            </tr>
            <%
                        continue;
                    }

                    GenerWorkerList gwl =
                        gwls.GetEntityByKey(GenerWorkerListAttr.FK_Node, node.NodeID) as GenerWorkerList;

                    if (gwl == null)
                    {
                        /* 还没有到达的节点. */
                        tc =
                            tcs.GetEntityByKey(GenerWorkerListAttr.FK_Node, node.NodeID) as
                            TransferCustom;

                        if (tc == null)
                            tc = new TransferCustom();

                        es = "[";

                        if (!string.IsNullOrWhiteSpace(tc.Worker))
                        {
                            foreach (string e in tc.Worker.Split(','))
                                es += "'" + e + "',";
                        }

                        if (es.Substring(es.Length - 1, 1) == ",")
                            es = es.Substring(0, es.Length - 1);

                        es += "]";

                        load = es.Length > 2 ? ",onLoadSuccess:function(){ isLoad = true; $(this).combobox('setValues', " + es + "); isLoad = false; }" : "";
                    
            %>
            <tr>
                <td class="Idx">
                    <%=node.NodeID %>
                </td>
                <td>
                    <%=node.Name %>
                </td>
                <td>
                    <input id="emps_<%=node.NodeID %>_<%=node.Step %>" class="easyui-combobox" style="width: 220px"
                        data-options="url:'<%=host %>/WF/WorkOpt/TransferCustomSimple.aspx?method=findemps&workId=<%=workid %>&nodeId=<%=node.NodeID %>',
                    method:'get',
                    onChange:function(){
                        var ids = this.id.split('_');
                        saveCfg(this);                        
                    },
                    multiple:true,
                    valueField:'no',
                    textField:'name',
                    groupField:'dept'<%=load %>" />
                </td>
                <td>
                    <input id="plan_<%=node.NodeID %>" type="text" class="Wdate" style="width: 100px;"
                        onfocus="WdatePicker({dchanged:function(){saveCfg($('input[id*=\'emps_'+this.id.split('_')[1]+'_\'')[0], $dp.cal.getNewDateStr())}})"
                        value="<%=tc.PlanDT %>" />
                </td>
            </tr>
            <%
                        continue;
                    }

                    //已经走完节点
            %>
            <tr>
                <td class="Idx">
                    <%=node.NodeID %>
                </td>
                <td>
                    <%=node.Name %>
                </td>
                <td>
                    <%=gwl.FK_EmpText%>
                </td>
                <td>
                    <%=gwl.RDT%>
                </td>
            </tr>
            <%
                }
            %>
        </tbody>
    </table>
    </form>
</body>
</html>
