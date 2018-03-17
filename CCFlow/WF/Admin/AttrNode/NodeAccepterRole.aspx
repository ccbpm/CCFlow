<%@ Page Title="" Language="C#" MasterPageFile="~/WF/WinOpen.master" AutoEventWireup="true"
    CodeBehind="NodeAccepterRole.aspx.cs" Inherits="CCFlow.WF.Admin.FlowNodeAttr.NodeAccepterRole" %>

<%@ Register Assembly="BP.Web.Controls" Namespace="BP.Web.Controls" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Comm/Style/CommStyle.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../../Scripts/CommonUnite.js" type="text/javascript"></script>
    <script src="../../Comm/JScript.js" type="text/javascript"></script>
    <script src="../../Scripts/CommonUnite.js" type="text/javascript"></script>
    <style type="text/css">
    .icon-reddot{
	     background:url('../../Img/OK.png') no-repeat center center;
        }
     </style>
    <script language="javascript" type="text/javascript">
        $(function () {
            //按当前操作员所属组织结构逐级查找岗位
            $("#RB0").hide();
            $("#YC_1").hide();
            //按节点绑定的部门计算
            $("#RB1").hide();
            $("#YC_02").hide();
            //按节点绑定的人员计算
            $("#RB3").hide();
            $("#YC_03").hide();
            //按绑定的岗位与部门交集计算
            $("#RB9").hide();
            $("#YC_04").hide();
            //按绑定的岗位计算并且以绑定的部门集合为纬度
            $("#RB10").hide();
            $("#YC_05").hide();
            //按指定节点的人员岗位计算
            $("#RB11").hide();
            $("#YC_06").hide();
            //仅按绑定的岗位计算
            $("#RB14").hide();
            $("#YC_07").hide();
            //仅按绑定的岗位计算
            $("#RB7").hide();
            $("#YC_08").hide();
            //与上一节点处理人员相同
            $("#RB6").hide();
            $("#YC_09").hide();

            //与指定节点处理人相同
            $("#RB8").hide();
            $("#YC_10").hide();

            //按设置的SQL获取接受人计算
            $("#RB2").hide();
            $("#YC_11").hide();

            //按设置的SQLTemplate获取接受人计算
            $("#RB2_").hide();
            $("#YC_11_").hide();

            //按SQL确定子线程接受人与数据源
            $("#RB12").hide();
            $("#YC_12").hide();
            //由上一节点发送人通过“人员选择器”选择接受人
            $("#RB4").hide();
            $("#YC_13").hide();
            //按上一节点表单指定的字段值作为本步骤的接受人
            $("#RB5").hide();
            $("#YC_14").hide();
            //由上一节点的明细表来决定子线程的接受人
            $("#RB13").hide();
            $("#YC_15").hide();
            //由FEE来决定
            $("#RB15").hide();
            $("#YC_16").hide();
            //按绑定部门计算，该部门一人处理标识该工作结束(子线程)
            $("#RB16").hide();
            $("#YC_17").hide();

            //按人员从到来计算.
            $("#RB18").hide();
            $("#YC_18_").hide();

            //按ccBPM的BPM模式处理
            $("#RB100").hide();
            $("#YC_18").hide();

            if (document.getElementById("<%=RB_ByStation.ClientID%>").checked) {
                onclickSJ(1);
                
                $('#tt').tree('update', {
                    target: $('#tt').tree('find', "node_1").target,
                    iconCls: 'icon-reddot'
                });

            }
            if (document.getElementById("<%=RB_ByDept.ClientID%>").checked) {
                onclickSJ(2);
                $('#tt').tree('update', {
                    target: $('#tt').tree('find', "node_2").target,
                    iconCls: 'icon-reddot'
                });

            }
            if (document.getElementById("<%=RB_ByBindEmp.ClientID%>").checked) {
                onclickSJ(3);
                $('#tt').tree('update', {
                    target: $('#tt').tree('find', "node_3").target,
                    iconCls: 'icon-reddot'
                });
            }
            if (document.getElementById("<%=RB_ByDeptAndStation.ClientID%>").checked) {
                onclickSJ(4);
                $('#tt').tree('update', {
                    target: $('#tt').tree('find', "node_4").target,
                    iconCls: 'icon-reddot'
                });
            }
            if (document.getElementById("<%=RB_ByStationAndEmpDept.ClientID%>").checked) {
                onclickSJ(5);
                $('#tt').tree('update', {
                    target: $('#tt').tree('find', "node_5").target,
                    iconCls: 'icon-right'
                });
            }
            if (document.getElementById("<%=RB_BySpecNodeEmpStation.ClientID%>").checked) {
                onclickSJ(6);
                $('#tt').tree('update', {
                    target: $('#tt').tree('find', "node_6").target,
                    iconCls: 'icon-reddot'
                });
            }
            if (document.getElementById("<%=RB_ByStationOnly.ClientID%>").checked) {
                onclickSJ(7);
                $('#tt').tree('update', {
                    target: $('#tt').tree('find', "node_7").target,
                    iconCls: 'icon-reddot'
                });
            }
            if (document.getElementById("<%=RB_ByStarter.ClientID%>").checked) {
                onclickSJ(8);
                $('#tt').tree('update', {
                    target: $('#tt').tree('find', "node_8").target,
                    iconCls: 'icon-reddot'
                });
            }
            if (document.getElementById("<%=RB_ByPreviousNodeEmp.ClientID%>").checked) {
                onclickSJ(9);
                $('#tt').tree('update', {
                    target: $('#tt').tree('find', "node_9").target,
                    iconCls: 'icon-reddot'
                });
            }
            if (document.getElementById("<%=RB_BySpecNodeEmp.ClientID%>").checked) {
                onclickSJ(10);
                $('#tt').tree('update', {
                    target: $('#tt').tree('find', "node_10").target,
                    iconCls: 'icon-reddot'
                });
            }
            
            if (document.getElementById("<%=RB_BySQL.ClientID%>").checked) {
                onclickSJ(11);
                $('#tt').tree('update', {
                    target: $('#tt').tree('find', "node_11").target,
                    iconCls: 'icon-reddot'
                });
            }

            if (document.getElementById("<%=RB_BySQLTemplate.ClientID%>").checked) {
                onclickSJ(99);
                $('#tt').tree('update', {
                    target: $('#tt').tree('find', "node_99").target,
                    iconCls: 'icon-reddot'
                });
            }

            if (document.getElementById("<%=RB_BySQLAsSubThreadEmpsAndData.ClientID%>").checked) {
                onclickSJ(12);
                $('#tt').tree('update', {
                    target: $('#tt').tree('find', "node_12").target,
                    iconCls: 'icon-reddot'
                });
            }
            if (document.getElementById("<%=RB_BySelected.ClientID%>").checked) {
                onclickSJ(13);
                $('#tt').tree('update', {
                    target: $('#tt').tree('find', "node_13").target,
                    iconCls: 'icon-reddot'
                });
            }
            if (document.getElementById("<%=RB_ByPreviousNodeFormEmpsField.ClientID%>").checked) {
                onclickSJ(14);
                $('#tt').tree('update', {
                    target: $('#tt').tree('find', "node_14").target,
                    iconCls: 'icon-reddot'
                });
            }
            if (document.getElementById("<%=RB_ByDtlAsSubThreadEmps.ClientID%>").checked) {
                onclickSJ(15);
                $('#tt').tree('update', {
                    target: $('#tt').tree('find', "node_15").target,
                    iconCls: 'icon-reddot'
                });
            }
            if (document.getElementById("<%=RB_ByFEE.ClientID%>").checked) {
                onclickSJ(16);
                $('#tt').tree('update', {
                    target: $('#tt').tree('find', "node_16").target,
                    iconCls: 'icon-reddot'
                });
            }
            if (document.getElementById("<%=RB_BySetDeptAsSubthread.ClientID%>").checked) {
                onclickSJ(17);
                $('#tt').tree('update', {
                    target: $('#tt').tree('find', "node_17").target,
                    iconCls: 'icon-reddot'
                });
            }

            if (document.getElementById("<%=RB_ByCCFlowBPM.ClientID%>").checked) {
                onclickSJ(100);
                $('#tt').tree('update', {
                    target: $('#tt').tree('find', "node_18").target,
                    iconCls: 'icon-reddot'
                });
            }


            if (document.getElementById("<%=RB_ByFromEmpToEmp.ClientID%>").checked) {
                onclickSJ('RB_ByFromEmpToEmp');
                $('#tt').tree('update', {
                    target: $('#tt').tree('find', "node_18").target,
                    iconCls: 'icon-reddot'
                });
            }

        });
        function onclickSJ(runModel) {
            //按当前操作员所属组织结构逐级查找岗位
            $("#RB0").hide();
            $("#YC_1").hide();
            //按节点绑定的部门计算
            $("#RB1").hide();
            $("#YC_02").hide();
            //按节点绑定的人员计算
            $("#RB3").hide();
            $("#YC_03").hide();
            //按绑定的岗位与部门交集计算
            $("#RB9").hide();
            $("#YC_04").hide();
            //按绑定的岗位计算并且以绑定的部门集合为纬度
            $("#RB10").hide();
            $("#YC_05").hide();
            //按指定节点的人员岗位计算
            $("#RB11").hide();
            $("#YC_06").hide();
            //仅按绑定的岗位计算
            $("#RB14").hide();
            $("#YC_07").hide();
            //与上一节点处理人员相同
            $("#RB7").hide();
            $("#YC_08").hide();
            //与开始节点处理人相同
            $("#RB6").hide();
            $("#YC_09").hide();
            //与指定节点处理人相同
            $("#RB8").hide();
            $("#YC_10").hide();
            //按设置的SQL获取接受人计算
            $("#RB2").hide();
            $("#YC_11").hide();


            //按设置的SQLTemplate获取接受人计算
            $("#RB2_").hide();
            $("#YC_11_").hide();

            //按SQL确定子线程接受人与数据源
            $("#RB12").hide();
            $("#YC_12").hide();
            //由上一节点发送人通过“人员选择器”选择接受人
            $("#RB4").hide();
            $("#YC_13").hide();
            //按上一节点表单指定的字段值作为本步骤的接受人
            $("#RB5").hide();
            $("#YC_14").hide();
            //由上一节点的明细表来决定子线程的接受人
            $("#RB13").hide();
            $("#YC_15").hide();
            //由FEE来决定
            $("#RB15").hide();
            $("#YC_16").hide();
            //按绑定部门计算，该部门一人处理标识该工作结束(子线程)
            $("#RB16").hide();
            $("#YC_17").hide();

            $("#RB17").hide();
            $("#YC_17_").hide();
            
            //按ccBPM的BPM模式处理
            $("#RB100").hide();
            $("#YC_18").hide();

            if (runModel == "RB_ByFromEmpToEmp") {
                $("#RB18").show();
                document.getElementById("<%=RB_ByFromEmpToEmp.ClientID%>").checked = "checked";
                $("#YC_18_").show();
                return;
            }

            if (runModel == 1) {
                $("#RB0").show();
                document.getElementById("<%=RB_ByStation.ClientID%>").checked = "checked";
                $("#YC_1").show();
            }

            if (runModel == 2) {
                $("#RB1").show();
                document.getElementById("<%=RB_ByDept.ClientID%>").checked = "checked";
                $("#YC_02").show();
            }
            if (runModel == 3) {
                $("#RB3").show();
                document.getElementById("<%=RB_ByBindEmp.ClientID%>").checked = "checked";
                $("#YC_03").show();
            }
            if (runModel == 4) {
                $("#RB9").show();
                document.getElementById("<%=RB_ByDeptAndStation.ClientID%>").checked = "checked";
                $("#YC_04").show();
            }
            if (runModel == 5) {
                $("#RB10").show();
                document.getElementById("<%=RB_ByStationAndEmpDept.ClientID%>").checked = "checked";
                $("#YC_05").show();
            }
            if (runModel == 6) {
                $("#RB11").show();
                document.getElementById("<%=RB_BySpecNodeEmpStation.ClientID%>").checked = "checked";
                $("#YC_06").show();
            }
            if (runModel == 7) {
                $("#RB14").show();
                document.getElementById("<%=RB_ByStationOnly.ClientID%>").checked = "checked";
                $("#YC_07").show();
            }
            if (runModel == 8) {
                $("#RB7").show();
                document.getElementById("<%=RB_ByStarter.ClientID%>").checked = "checked";
                $("#YC_08").show();
            }
            if (runModel == 9) {
                $("#RB6").show();
                document.getElementById("<%=RB_ByPreviousNodeEmp.ClientID%>").checked = "checked";
                $("#YC_09").show();
            }
            if (runModel == 10) {
                $("#RB8").show();
                document.getElementById("<%=RB_BySpecNodeEmp.ClientID%>").checked = "checked";
                $("#YC_10").show();
            }
            
            if (runModel == 11) {
                $("#RB2").show();
                document.getElementById("<%=RB_BySQL.ClientID%>").checked = "checked";
                $("#YC_11").show();
            }

            if (runModel == 99) {
                $("#RB2_").show();
                document.getElementById("<%=RB_BySQLTemplate.ClientID%>").checked = "checked";
                $("#YC_11_").show();
            }


            if (runModel == 12) {
                $("#RB12").show();
                document.getElementById("<%=RB_BySQLAsSubThreadEmpsAndData.ClientID%>").checked = "checked";
                $("#YC_12").show();
            }
            if (runModel == 13) {
                $("#RB4").show();
                document.getElementById("<%=RB_BySelected.ClientID%>").checked = "checked";
                $("#YC_13").show();
            }
            if (runModel == 14) {
                $("#RB5").show();
                document.getElementById("<%=RB_ByPreviousNodeFormEmpsField.ClientID%>").checked = "checked";
                $("#YC_14").show();
            }
            if (runModel == 15) {
                $("#RB13").show();
                document.getElementById("<%=RB_ByDtlAsSubThreadEmps.ClientID%>").checked = "checked";
                $("#YC_15").show();
            }
            if (runModel == 16) {
                $("#RB15").show();
                document.getElementById("<%=RB_ByFEE.ClientID%>").checked = "checked";
                $("#YC_16").show();
            }
            if (runModel == 17) {
                $("#RB16").show();
                document.getElementById("<%=RB_BySetDeptAsSubthread.ClientID%>").checked = "checked";
                $("#YC_17").show();
            }
            if (runModel == 18 || runModel==100) {
                $("#RB100").show();
                document.getElementById("<%=RB_ByCCFlowBPM.ClientID%>").checked = "checked";
                $("#YC_18").show();
            }
        }

        //装载模版.
        function LoadTemplate() {
            var url = "../SettingTemplate.htm?TemplateType=NodeAccepterRole";
            WinOpen(url, 'ss');
        }
    </script>
    <style type="text/css">
        li
        {
            font-size: 12px;
        }
        img
        {
            border: none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%
        int nodeID = int.Parse(this.Request.QueryString["FK_Node"]);

        // int.Parse(this.Request.QueryString["FK_Node"]);
        BP.WF.Node nd = new BP.WF.Node(nodeID);
        string k = DateTime.Now.ToString("MMddhhmmss");
        BP.WF.Template.NodeStations nss = new BP.WF.Template.NodeStations();
        nss.Retrieve(BP.WF.Template.NodeStationAttr.FK_Node, this.NodeID);

        BP.WF.Template.NodeDepts ndepts = new BP.WF.Template.NodeDepts();
        ndepts.Retrieve(BP.WF.Template.NodeStationAttr.FK_Node, this.NodeID);

        BP.WF.Template.NodeEmps nEmps = new BP.WF.Template.NodeEmps();
        nEmps.Retrieve(BP.WF.Template.NodeStationAttr.FK_Node, this.NodeID);
        
    %>
    <div class="easyui-layout" data-options="fit:true">
        <div data-options="region:'west',split:true" style="width: 260px; height: auto;">
            <div class="easyui-tree" style="height: auto;" fit="true" border="false">
                <ul id="tt" class="easyui-tree" style="height: auto;">
                    <li><span>访问规则</span>
                        <ul>
                            <li><span>按组织结构绑定</span>
                                <ul>
                                    <li id="node_1">
                                        <div>
                                            <a  class='l-link' href="#" onclick="onclickSJ(1)"><span class="nav">按照岗位智能计算</span></a></div>
                                    </li>
                                    <li id="node_2">
                                        <div>
                                            <a  class='1-link' href="#" onclick="onclickSJ(2)"><span class="nav">按节点绑定的部门计算</span></a></div>
                                    </li>
                                    <li id="node_3">
                                        <div>
                                            <a  class='l-link' href="#" onclick="onclickSJ(3)"><span class="nav">按节点绑定的人员计算
                                            </span></a>
                                        </div>
                                    </li>
                                    <li id="node_4">
                                        <div>
                                            <a  class='l-link' href="#" onclick="onclickSJ(4)"><span class="nav">按绑定的岗位与部门交集计算</span></a></div>
                                    </li>
                                    <li id="node_5">
                                        <div>
                                            <a  class='l-link' href="#" onclick="onclickSJ(5)"><span class="nav">按绑定的岗位计算并且以绑定的部门集合为纬度</span></a></div>
                                    </li>
                                    <li id="node_6">
                                        <div>
                                            <a  class='l-link' href="#" onclick="onclickSJ(6)"><span class="nav">按指定节点的人员岗位计算</span></a></div>
                                    </li>
                                    <li id="node_7">
                                        <div>
                                            <a  class='l-link' href="#" onclick="onclickSJ(7)"><span class="nav">仅按绑定的岗位计算</span></a></div>
                                    </li>
                                    <li id="node_17">
                                        <div>
                                            <a  class='l-link' href="#" onclick="onclickSJ(17)"><span class="nav">按绑定部门计算，该部门一人处理标识该工作结束(子线程)</span></a></div>
                                    </li>
                                </ul>
                            </li>
                        </ul>
                        <ul>
                            <li><span>按访问规则选项</span>
                                <ul>
                                    <li id="node_8">
                                        <div>
                                            <a  class='l-link' onclick="onclickSJ(8)" href=" #"><span class="nav">与开始节点处理人相同</span></a></div>
                                    </li>
                                    <li id="node_9">
                                        <div>
                                            <a  class='l-link' onclick="onclickSJ(9)" href="#"><span class="nav">与上一节点处理人员相同</span></a></div>
                                    </li>
                                    <li id="node_10">
                                        <div>
                                            <a  class='l-link' onclick="onclickSJ(10)" href="#"><span class="nav">与指定节点处理人相同</span></a></div>
                                    </li>
                                </ul>
                            </li>
                        </ul>
                        <ul>
                            <li><span>按自定义SQL查询</span>
                                <ul>
                                    <li id="node_11">
                                        <div>
                                            <a  class='l-link' onclick="onclickSJ(11)" href="#RB2"><span class="nav">按设置的SQL获取接受人计算</span></a></div>
                                    </li>
                                     <li id="node_99">
                                        <div>
                                            <a  class='l-link' onclick="onclickSJ(99)" href="#RB2"><span class="nav">按设置的SQLTempate获取接受人计算</span></a></div>
                                    </li>

                                    <li id="node_12">
                                        <div>
                                            <a  class='l-link' onclick="onclickSJ(12)" href="#RB12"><span class="nav">按SQL确定子线程接受人与数据源</span></a></div>
                                    </li>
                                </ul>
                            </li>
                        </ul>


                        <ul>
                            <li><span>其他方式</span>
                                <ul>
                                    <li id="node_13">
                                        <div>
                                            <a  class='l-link' onclick="onclickSJ(13)" href="#RB4"><span class="nav">由上一节点发送人通过“人员选择器”选择接受人</span></a></div>
                                    </li>
                                    <li id="node_14">
                                        <div>
                                            <a  class='l-link' onclick="onclickSJ(14)" href="#RB5"><span class="nav">按上一节点表单指定的字段值作为本步骤的接受人</span></a></div>
                                    </li>
                                    <li id="node_15">
                                        <div>
                                            <a  class='l-link' onclick="onclickSJ(15)" href="#RB13"><span class="nav">由上一节点的明细表来决定子线程的接受人</span></a></div>
                                    </li>
                                    <li id="node_16">
                                        <div>
                                            <a  class='l-link' onclick="onclickSJ(16)" href="#RB15"><span class="nav">由FEE来决定</span></a></div>
                                    </li>
                                    <li id="node_18">
                                        <div>
                                            <a  class='l-link' onclick="onclickSJ('RB_ByFromEmpToEmp')" href="#RB18"><span class="nav">按照配置的人员路由列表计算</span></a></div>
                                    </li>
                                    <li id="node_100">
                                        <div>
                                            <a  class='l-link' onclick="onclickSJ(100)" href="#RB100"><span class="nav">
                                                按ccBPM的BPM模式处理</span></a></div>
                                    </li>
                                </ul>
                            </li>
                        </ul>
                    </li>
                </ul>
            </div>
        </div>
        <div class="easyui-layout" data-options=" region:'center',title:'访问规则设置' ">
            <div data-options="region:'center',border:false">
                <div id="tabs" class="easyui-tabs" data-options="fit:true ,border:false">
                    <div title="节点[<%=nd.Name %>]接受人规则设置">
                        <table style="width: 100%;">
                            <!-- ===================================  01.按当前操作员所属组织结构逐级查找岗位 -->
                            <tr id="RB0">
                                <th>
                                    <div style="float: left">
                                        <asp:RadioButton ID="RB_ByStation" Text="按照岗位智能计算" GroupName="xxx" runat="server" />
                                    </div>
                                    <div style="float: right">

                                    <a href="javascript:WinOpen('/WF/Comm/RefFunc/Dot2DotSingle.htm?EnsName=BP.WF.Template.Selectors&EnName=BP.WF.Template.Selector&AttrKey=BP.WF.Template.NodeStations&NodeID=<%=nd.NodeID %>&r=<%=k %>&ShowWay=None')">
                                            设置/更改岗位(<%=nss.Count %>)(旧版本)</a>|
                                        <a href="javascript:WinOpen('/WF/Comm/RefFunc/Dot2DotStationModel.htm?EnsName=BP.WF.Template.Selectors&EnName=BP.WF.Template.Selector&AttrKey=BP.WF.Template.NodeStations&NodeID=<%=nd.NodeID %>&r=<%=k %>&ShowWay=None')">
                                            设置/更改岗位(<%=nss.Count %>)</a> | <a href='http://ccbpm.mydoc.io' target='_blank'>
                                                <img src='../../Img/Help.png' style="vertical-align: middle" />帮助</a></div>
                                </th>
                            </tr>
                            <tr id="YC_1">
                                <td class="BigDoc">
                                    <ul>
                                        <li >该方式是系统默认的方式，也是常用的方式，系统可以智能的寻找您需要的接受人，点击右上角设置岗位。</li>
                                        <li >在寻找接受人的时候系统会考虑当前人的部门因素，A发到B，在B节点上绑定N个岗位，系统首先判断当前操作员部门内是否具有该岗位集合之一，如果有就投递给他，没有就把自己的部门提高一个级别，在寻找，依次类推，一直查找到最高级，如果没有就抛出异常。</li>
                                        <li >比如：一个省机关下面有n个县，n个市，n个县. n个所. 一个所员受理人员的业务，只能让自己的所长审批，所长的业务只能投递到本区县的相关业务部分审批，而非其它区县业务部分审批。</li>
                                    </ul>
                                </td>
                            </tr>
                            <!-- ===================================  02.按节点绑定的部门计算 -->
                            <tr id="RB1">
                                <th>
                                    <div style="float: left">
                                        <asp:RadioButton ID="RB_ByDept" Text="按节点绑定的部门计算" GroupName="xxx" runat="server" />
                                    </div>
                                    <div style="float:right">


                                    <a href="javascript:WinOpen('/WF/Comm/RefFunc/Dot2DotTreeDeptModel.htm?EnsName=BP.WF.Template.Selectors&EnName=BP.WF.Template.Selector&AttrKey=BP.WF.Template.NodeDepts&NodeID=<%=nd.NodeID %>&r=<%=k %>&ShowWay=FK_StationType')">
                                            设置/更改部门(<%=ndepts.Count %>)(旧版本)</a> |

                                           <a href="javascript:WinOpen('../../Comm/RefFunc/Branches.htm?EnName=BP.WF.Template.NodeSheet&Dot2DotEnsName=BP.WF.Template.NodeDepts&Dot2DotEnName=BP.WF.Template.NodeDept&AttrOfOneInMM=FK_Node&AttrOfMInMM=FK_Dept&EnsOfM=BP.GPM.Depts&DefaultGroupAttrKey=&NodeID=<%=nd.NodeID %>&PKVal=<%=nd.NodeID %>')">
                                            设置/更改部门(<%=ndepts.Count %>)</a>

                                            
                                            
                                            </div>
                                </th>
                            </tr>
                            <tr id="YC_02">
                                <td class="BigDoc">
                                    <ul>
                                        <li >该部门下所有的人员都可以处理该节点的工作。</li>
                                    </ul>
                                </td>
                            </tr>
                            <!-- ===================================  04.按节点绑定的人员计算 -->
                            <tr id="RB3">
                                <th>
                                    <div style="float: left">
                                        <asp:RadioButton ID="RB_ByBindEmp" Text="按节点绑定的人员计算" GroupName="xxx" runat="server" />
                                    </div>
                                    <div style="float: right">
                                        <a href="javascript:WinOpen('/WF/Comm/RefFunc/BranchesAndLeaf.htm?EnName=BP.WF.Template.NodeSheet&Dot2DotEnsName=BP.WF.Template.NodeEmps&Dot2DotEnName=BP.WF.Template.NodeEmp&AttrOfOneInMM=FK_Node&AttrOfMInMM=FK_Emp&EnsOfM=BP.Port.Emps&DefaultGroupAttrKey=FK_Dept&NodeID=<%=nd.NodeID %>&PKVal=<%=nd.NodeID %>')">

                                       <%-- <a href="javascript:WinOpen('/WF/Comm/RefFunc/BranchesAndLeaf.htm?EnsName=BP.WF.Template.Selectors&EnName=BP.WF.Template.Selector&AttrKey=BP.WF.Template.NodeEmps&NodeID=<%=nd.NodeID %>&r=<%=k %>&ShowWay=FK_StationType')">--%>

                                            设置/更改处理人(<%=nEmps.Count %>)</a></div>
                                </th>
                            </tr>
                            <tr id="YC_03">
                                <td class="BigDoc">
                                    <ul>
                                        <li >绑定的所有的人员，都可以处理该节点的工作。</li>
                                    </ul>
                                </td>
                            </tr>
                            <!-- ===================================  10.按绑定的岗位与部门交集计算 -->
                            <tr id="RB9">
                                <th>
                                    <div style="float: left">
                                        <asp:RadioButton ID="RB_ByDeptAndStation" Text="按绑定的岗位与部门交集计算" GroupName="xxx" runat="server" />
                                    </div>
                                    <div style="float: right">
                                        <a href="javascript:WinOpen('/WF/Comm/RefFunc/Dot2DotStationModel.htm?EnsName=BP.WF.Template.Selectors&EnName=BP.WF.Template.Selector&AttrKey=BP.WF.Template.NodeStations&NodeID=<%=nd.NodeID %>&r=<%=k %>&ShowWay=None')">
                                            设置与更改岗位(<%=nss.Count%>)</a>


                                             |
                                              <a href="javascript:WinOpen('../../Comm/RefFunc/Branches.htm?EnName=BP.WF.Template.NodeSheet&Dot2DotEnsName=BP.WF.Template.NodeDepts&Dot2DotEnName=BP.WF.Template.NodeDept&AttrOfOneInMM=FK_Node&AttrOfMInMM=FK_Dept&EnsOfM=BP.GPM.Depts&DefaultGroupAttrKey=&NodeID=<%=nd.NodeID %>&PKVal=<%=nd.NodeID %>')">
                                            设置/更改部门(<%=ndepts.Count %>)</a>|

                                             <a href="javascript:WinOpen('/WF/Comm/RefFunc/Dot2DotTreeDeptModel.htm?EnsName=BP.WF.Template.Selectors&EnName=BP.WF.Template.Selector&AttrKey=BP.WF.Template.NodeDepts&NodeID=<%=nd.NodeID %>&r=<%=k %>&ShowWay=FK_StationType')">
                                            设置/更改部门(<%=ndepts.Count %>)(旧版本)</a> |
                                             
                                             <%--<a href="javascript:WinOpen('/WF/Comm/RefFunc/Dot2DotTreeDeptModel.htm?EnsName=BP.WF.Template.Selectors&EnName=BP.WF.Template.Selector&AttrKey=BP.WF.Template.NodeDepts&NodeID=<%=nd.NodeID %>&r=1005101248&ShowWay=FK_StationType')">
                                                设置与更改部门(<%=ndepts.Count%>)</a> | 
                                                <a href="javascript:WinOpen('/WF/Comm/RefFunc/Dot2DotSingle.htm?EnsName=BP.WF.Template.Selectors&EnName=BP.WF.Template.Selector&AttrKey=BP.WF.Template.NodeDepts&NodeID=<%=nd.NodeID %>&r=<%=k %>&ShowWay=FK_StationType')">设置/更改部门(<%=ndepts.Count %>)(旧版本)</a>
--%>

                                         |
                                    </div>
                                </th>
                            </tr>
                            <tr id="YC_04">
                                <td class="BigDoc">
                                    <ul>
                                        <li >ccBPM会取既具备此岗位集合的又具备此部门集合的人员，做为本节点的接受人员。</li>
                                    </ul>
                                </td>
                            </tr>
                            <!-- ===================================  11.按绑定的岗位计算并且以绑定的部门集合为纬度 -->
                            <tr id="RB10">
                                <th>
                                    <div style="float: left">
                                        <asp:RadioButton ID="RB_ByStationAndEmpDept" Text="按绑定的岗位计算并且以绑定的部门集合为纬度" GroupName="xxx" runat="server" />
                                    </div>
                                    <div style="float: right">
                                        <a href="javascript:WinOpen('/WF/Comm/RefFunc/Dot2DotStationModel.htm?EnsName=BP.WF.Template.Selectors&EnName=BP.WF.Template.Selector&AttrKey=BP.WF.Template.NodeStations&NodeID=<%=nd.NodeID %>&r=<%=k %>&ShowWay=None')">
                                            设置与更改岗位(<%=nss.Count %>)</a> |
                                            
                                            
                                             <a href="javascript:WinOpen('../../Comm/RefFunc/Branches.htm?EnName=BP.WF.Template.NodeSheet&Dot2DotEnsName=BP.WF.Template.NodeDepts&Dot2DotEnName=BP.WF.Template.NodeDept&AttrOfOneInMM=FK_Node&AttrOfMInMM=FK_Dept&EnsOfM=BP.GPM.Depts&DefaultGroupAttrKey=&NodeID=<%=nd.NodeID %>&PKVal=<%=nd.NodeID %>')">
                                            设置/更改部门(<%=ndepts.Count %>)
                                            |
                                              <a href="javascript:WinOpen('/WF/Comm/RefFunc/Dot2DotTreeDeptModel.htm?EnsName=BP.WF.Template.Selectors&EnName=BP.WF.Template.Selector&AttrKey=BP.WF.Template.NodeDepts&NodeID=<%=nd.NodeID %>&r=<%=k %>&ShowWay=FK_StationType')">
                                            设置/更改部门(<%=ndepts.Count %>)(旧版本)</a> |
                                            </a>

                                       
                                    </div>
                                </th>
                            </tr>
                            <tr id="YC_05">
                                <td class="BigDoc">
                                    <ul>
                                        <li>ccBPM会取绑定部门集合下绑定岗位的人员 </li>
                                    </ul>
                                </td>
                            </tr>
                            <!-- ===================================  12.按指定节点的人员岗位计算 -->
                            <tr id="RB11">
                                <th>
                                    <div style="float: left">
                                        <asp:RadioButton ID="RB_BySpecNodeEmpStation" Text="按指定节点的人员岗位计算" GroupName="xxx" runat="server" />
                                    </div>
                                    <div style="float: right">
                                        请选择要指定的节点.
                                    </div>
                                </th>
                            </tr>
                            <tr id="YC_06">
                                <td class="BigDoc">
                                    <asp:CheckBoxList ID="CBL_BySpecNodeEmpStation" runat="server" RepeatDirection="Horizontal" RepeatColumns="5">
                                    </asp:CheckBoxList>
                                    <ul>
                                        <li>ccBPM在处理接受人时，会按指定节点上的人员身份计算 </li>
                                    </ul>
                                </td>
                            </tr>
                            <!-- ===================================  15.仅按绑定的岗位计算 -->
                            <tr id="RB14">
                                <th>
                                    <div style="float: left">
                                        <asp:RadioButton ID="RB_ByStationOnly" Text="仅按绑定的岗位计算" GroupName="xxx" runat="server" />
                                    </div>
                                     <div style="float: right">
                                        <a href="javascript:WinOpen('/WF/Comm/RefFunc/Dot2DotStationModel.htm?EnsName=BP.WF.Template.Selectors&EnName=BP.WF.Template.Selector&AttrKey=BP.WF.Template.NodeStations&NodeID=<%=nd.NodeID %>&r=<%=k %>&ShowWay=None')">
                                            设置/更改岗位-旧版(<%=nss.Count %>)</a>
                                    </div>

                                    <div style="float: right">
                                        <a href="javascript:WinOpen('NodeAccepterRoleStation.htmFK_Node=<%=nd.NodeID %>&r=<%=k %>&ShowWay=None')">
                                            设置/更改岗位(<%=nss.Count %>)</a>
                                    </div>
                                </th>
                            </tr>
                            <tr id="YC_07">
                                <td class="BigDoc">
                                    <ul>
                                        <li>按照节点上绑定的岗位来计算接受人，这里去掉了部门维度的过滤。 </li>
                                        <li>您设置岗位范围的集合下面有多少人，该节点就有多少人处理。 </li>
                                    </ul>
                                </td>
                            </tr>
                            <!-- ===================================  08.与开始节点处理人相同 -->
                            <tr id="RB7">
                                <th>
                                    <div style="float: left">
                                        <asp:RadioButton ID="RB_ByStarter" Text="与开始节点处理人相同" GroupName="xxx" runat="server" />
                                    </div>
                                </th>
                            </tr>
                            <tr id="YC_08">
                                <td class="BigDoc">
                                    <ul>
                                        <li >当前节点的处理人与开始节点一致，发起人是 zhangsan,现在节点的处理人也是zhangsan。</li>
                                        <li >多用于反馈给申请人节点，通知申请人审批审核结果，此工作已经审核审批完毕。</li>
                                    </ul>
                                </td>
                            </tr>
                            <!-- ===================================  07.与上一节点处理人员相同 -->
                            <tr id="RB6">
                                <th>
                                    <div style="float: left">
                                        <asp:RadioButton ID="RB_ByPreviousNodeEmp" Text="与上一节点处理人员相同" GroupName="xxx" runat="server" />
                                    </div>
                                </th>
                            </tr>
                            <tr id="YC_09">
                                <td class="BigDoc">
                                    <ul>
                                        <li >节点A是zhangsan处理，发送到节点B,也是需要甲zhangsan处理。</li>
                                        <li >就是自己发送给自己的模式。</li>
                                    </ul>
                                </td>
                            </tr>
                            <!-- ===================================  09.与指定节点处理人相同 -->
                            <tr id="RB8">
                                <th>
                                    <div style="float: left">
                                        <asp:RadioButton ID="RB_BySpecNodeEmp" Text="与指定节点处理人相同" GroupName="xxx" runat="server" />
                                    </div>
                                    <div style="float: right">指定一个节点，当前节点的处理人就是他.</div>

                                </th>
                            </tr>
                            <tr id="YC_10">
                                <td class="BigDoc">
                                    <asp:CheckBoxList ID="CBL_BySpecNodeEmp" runat="server" RepeatDirection="Horizontal" RepeatColumns="5">
                                    </asp:CheckBoxList>
                                </td>
                            </tr>
                            <!-- ===================================  03.按设置的SQL获取接受人计算 -->
                            <tr id="RB2">
                                <th>
                                    <div style="float: left">
                                        <asp:RadioButton ID="RB_BySQL" Text="按设置的SQL获取接受人计算" GroupName="xxx" runat="server" />
                                    </div>
                                    <div style="float: right">请在文本框里输入SQL.</div>
                                </th>
                            </tr>
                            <tr id="YC_11">
                                <td class="BigDoc">
                                    <asp:TextBox ID="TB_BySQL" runat="server" Width="98%" Rows="3" Height="63px" TextMode="MultiLine"></asp:TextBox>
                                    <ul>
                                        <li >该SQL是需要返回No,Name两个列，分别是人员编号,人员名称。</li>
                                        <li >该配置也适合于</li>
                                        <li ><a href="javascript:LoadTemplate();" >我要从模版里选择一个设置.</a></li>

                                    </ul>
                                </td>
                            </tr>
                            
                            <!-- ===================================  03.按设置的SQLTemplate获取接受人计算 -->
                            <tr id="RB2_">
                                <th>
                                    <div style="float: left">
                                        <asp:RadioButton ID="RB_BySQLTemplate" Text="按设置的SQLTemplate获取接受人计算" GroupName="xxx" runat="server" />
                                    </div>
                                    <div style="float: right">请选择一个SQLTemplate</div>
                                </th>
                            </tr>
                            <tr id="YC_11_">
                                <td class="BigDoc">

                                <br />
                                  请选择一个SQLTemplate:  <asp:DropDownList ID="DDL_SQLTemplate" runat="server">
                                    </asp:DropDownList>

                                    <ul>
                                        <li ><a href="javascript:WinOpen('../../Comm/Search.htm?EnsName=BP.WF.Template.SQLTemplates&SQLType=1');" >我要配置SQLTemplate</a></li>
                                    </ul>
                                </td>
                            </tr>

                            <!-- ===================================  13.按SQL确定子线程接受人与数据源 -->
                            <tr id="RB12">
                                <th>
                                    <div style="float: left">
                                        <asp:RadioButton ID="RB_BySQLAsSubThreadEmpsAndData" Text="按SQL确定子线程接受人与数据源" GroupName="xxx" runat="server" />
                                    </div>
                                    <div style="float: right">
                                        请在文本框里输入SQL.
                                    </div>
                                </th>
                                <%
                                    bool isEnable = false;
                                    if (nd.HisRunModel == BP.WF.RunModel.SubThread)
                                        isEnable = true;

                                    this.RB_BySQLAsSubThreadEmpsAndData.Enabled = isEnable;
                                    this.TB_BySQLAsSubThreadEmpsAndData.Enabled = isEnable;

                                    if (nd.HisDeliveryWay == BP.WF.DeliveryWay.BySQLAsSubThreadEmpsAndData)
                                        this.RB_BySQLAsSubThreadEmpsAndData.Checked = true;
                                %>
                            </tr>
                            <tr id="YC_12">
                                <td class="BigDoc">
                                    <asp:TextBox ID="TB_BySQLAsSubThreadEmpsAndData" runat="server" Width="98%" Rows="3" Height="63px" TextMode="MultiLine"></asp:TextBox>
                                    <ul>
                                        <li >此方法与分合流相关，只有当前节点是子线程才有意义，当前节点模式为：<%=nd.HisRunModel%>。
                                        </li>
                                    </ul>
                                </td>
                            </tr>
                            <!-- ===================================  05.由上一节点发送人通过“人员选择器”选择接受人 -->
                            <tr id="RB4">
                                <th>
                                    <div style="float: left">
                                        <asp:RadioButton ID="RB_BySelected" Text="由上一节点发送人通过“人员选择器”选择接受人" GroupName="xxx" runat="server" />
                                    </div>
                                    <div style="float: right">
                                        <a href="javascript:WinOpen('/WF/Comm/En.htm?EnName=BP.WF.Template.Selector&PK=<%=nd.NodeID %>')">
                                            设置处理人可以选择的范围</a></div>
                                </th>
                            </tr>
                            <tr id="YC_13">
                                <td class="BigDoc">
                                    <ul>
                                        <li >绑定的所有的人员，都可以处理该节点的工作。</li>
                                        <li >特别说名:<font color=red> 如果当前节点为开始节点，所有的人员都可以发起。</font></li>

                                    </ul>
                                </td>
                            </tr>
                            <!-- ===================================  06.按上一节点表单指定的字段值作为本步骤的接受人 -->
                            <tr id="RB5">
                                <th>
                                    <div style="float: left">
                                        <asp:RadioButton ID="RB_ByPreviousNodeFormEmpsField" Text="按上一节点表单指定的字段值作为本步骤的接受人" GroupName="xxx" runat="server" />
                                    </div>
                                    <div style="float: right">
                                        请选择一个字段</div>
                                </th>
                            </tr>
                            <tr id="YC_14">
                                <td class="BigDoc">
                                    请选择一个节点字段：
                                    <asp:DropDownList ID="DDL_ByPreviousNodeFormEmpsField" runat="server">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <!-- ===================================  14.由上一节点的明细表来决定子线程的接受人 -->
                            <tr id="RB13">
                                <th>
                                    <div style="float: left">
                                        <asp:RadioButton ID="RB_ByDtlAsSubThreadEmps" Text="由上一节点的明细表来决定子线程的接受人" GroupName="xxx" runat="server" />
                                    </div>
                                    <div style="float:right">
                                        请输入用户编号列.
                                    </div>
                                </th>
                                <% 
                                    bool isenabled = false;
                                    if (nd.HisRunModel == BP.WF.RunModel.SubThread)
                                        isenabled = true;

                                    this.RB_ByDtlAsSubThreadEmps.Enabled = isenabled;
                                    this.TB_ByDtlAsSubThreadEmps.Enabled = isenabled;

                                    if (nd.HisDeliveryWay == BP.WF.DeliveryWay.ByDtlAsSubThreadEmps)
                                        this.RB_ByDtlAsSubThreadEmps.Checked = true;
                                %>
                            </tr>
                            <tr id="YC_15">
                                <td class="BigDoc">
                                    <asp:TextBox ID="TB_ByDtlAsSubThreadEmps" runat="server" Width="98%" Rows="3" Height="63px" TextMode="MultiLine"></asp:TextBox>
                                    <ul>
                                        <li>此方法与分合流相关，只有当前节点是子线程才有意义。 </li>
                                        <li>当前参数为明细表的字段列，如果不填写，就默认为 UserNo 。</li>
                                    </ul>
                                </td>
                            </tr>
                            <!-- ===================================  16.由FEE来决定. -->
                            <tr id="RB15">
                                <th>
                                    <div style="float: left">
                                        <asp:RadioButton ID="RB_ByFEE" Text="由FEE来决定." GroupName="xxx" runat="server" />
                                    </div>
                                    <div style="float: right">
                                        请编写事件代码
                                    </div>
                                </th>
                            </tr>
                            <tr id="YC_16">
                                <td class="BigDoc">
                                    <ul>
                                        <li >用流程事件，通过调用设置接受的接口，来设置当前节点的接收人，实现的把接受人信息写入接收人列表里。 </li>
                                    </ul>
                                </td>
                            </tr>
                                                        <!-- ===================================  17.按绑定部门计算，该部门一人处理标识该工作结束(子线程) -->
                            <tr id="RB16">
                                <th>
                                    <div style="float: left">
                                        <asp:RadioButton ID="RB_BySetDeptAsSubthread" Text="按绑定部门计算，该部门一人处理标识该工作结束(子线程)." GroupName="xxx" runat="server" />
                                    </div>
                                    <div style="float: right">
                                       
                                         <a href="javascript:WinOpen('/WF/Comm/RefFunc/Dot2DotTreeDeptModel.htm?EnsName=BP.WF.Template.Selectors&EnName=BP.WF.Template.Selector&AttrKey=BP.WF.Template.NodeDepts&NodeID=<%=nd.NodeID %>&r=<%=k %>&ShowWay=FK_StationType')">
                                                设置与更改部门(<%=ndepts.Count %>)</a>|<a href="javascript:WinOpen('/WF/Comm/RefFunc/Dot2DotSingle.htm?EnsName=BP.WF.Template.Selectors&EnName=BP.WF.Template.Selector&AttrKey=BP.WF.Template.NodeDepts&NodeID=<%=nd.NodeID %>&r=<%=k %>&ShowWay=FK_StationType')">设置/更改部门(<%=ndepts.Count %>)(旧版本)</a>
                                       
                                    </div>
                                </th>
                            </tr>
                            <tr id="YC_17">
                                <td class="BigDoc">
                                    <ul>
                                        <li >仅适用于子线程节点，按照部门分组子线程上的处理人员，每个部门一个任务，如果该部门的其中有一个人处理了，就标识该部门的工作完成，可以流转到下一步 </li>
                                    </ul>
                                </td>
                            </tr>
                            <!-- ===================================  18.按ccBPM的BPM模式处理. -->
                            <tr id="RB100">
                                <th>
                                    <div style="float: left">
                                        <asp:RadioButton ID="RB_ByCCFlowBPM" Text="按ccBPM的BPM模式处理." GroupName="xxx" runat="server" />
                                    </div>
                                    <div style="float: right">
                                        <a href="javascript:WinOpen('/WF/Admin/FindWorker/List.aspx?FK_Node=<%=nd.NodeID%>&FK_Flow=<%=nd.FK_Flow%>')">
                                            BPM模式接收人设置规则</a>
                                    </div>
                                </th>
                            </tr>
                            <tr id="YC_18">
                                <td class="BigDoc">
                                    <ul>
                                        <li >使用与ccflow集成到别的系统之中</li>
                                    </ul>
                                </td>
                            </tr>

                             <!-- ===================================  18 按照从人员，到人员计算. -->
                            <tr id="RB18">
                                <th>
                                    <div style="float: left">
                                        <asp:RadioButton ID="RB_ByFromEmpToEmp" Text="按照人员配置的路由计算" GroupName="xxx" runat="server" />
                                    </div>
                                    <div style="float: right">请按照约定的格式设置从人员发送到人员的路由路径.</div>
                                </th>
                            </tr>
                            <tr id="YC_18_">
                                <td class="BigDoc">
                                    <asp:TextBox ID="TB_ByFromEmpToEmp" runat="server" Width="98%" Rows="3" Height="63px" TextMode="MultiLine"></asp:TextBox>
                                    <ul>
                                        <li >格式为 @zhangsan,lisi@wangwu,zhaoliu      从如果是张三发送的就发送到李四身上. 多个人员对用@分开. </li>
                                        <li >如果没有找到，就按照默认值寻找: @Defualt,zhangsan  着一样配置表示，没有找到人就按照默认值投递.</li>
                                    </ul>
                                </td>
                            </tr>
                           

                        </table>
                    </div>
                    <div title="辅助属性" style="padding: 10px">
                        <table border="0" width="100%">
                            <!-- =================================== 是否可以分配工作  -->
                            <tr>
                                <th>
                                    <div style="float: left">
                                        <asp:CheckBox ID="CB_IsSSS" Text="是否可以分配工作？" runat="server" />
                                    </div>
                                </th>
                            </tr>
                            <tr>
                                <td>
                                    <ul>
                                        <li>该属性是对于该节点上有多个人处理有效。 </li>
                                        <li>比如:A,发送到B,B节点上有张三，李四，王五可以处理，您可以指定1个或者多个人处理B节点上的工作。</li>
                                    </ul>
                                </td>
                            </tr>
                            <!-- =================================== 是否启用自动记忆功能  -->
                            <tr>
                                <th>
                                    <div style="float: left">
                                        <asp:CheckBox ID="CB_IsRememme" Text="是否启用自动记忆功能？" runat="server" />
                                    </div>
                                </th>
                            </tr>
                            <tr>
                                <td>
                                    <ul>
                                        <li>该属性是对于该节点上有多个人处理有效。 </li>
                                        <li>比如:A,发送到B,B节点上有张三，李四，王五可以处理，这次你把工作分配给李四，如果设置了记忆，那么ccbpm就在下次发送的时候，自动投递给李四，让然您也可以重新分配。</li>
                                    </ul>
                                </td>
                            </tr>
                            <!-- =================================== 本节点接收人不允许包含上一步发送人  -->
                            <tr>
                                <th>
                                    <div style="float: left">
                                        <asp:CheckBox ID="CB_IsExpSender" Text="本节点接收人不允许包含上一步发送人？" runat="server" />
                                    </div>
                                </th>
                            </tr>
                            <tr>
                                <td>
                                    <ul>
                                        <li>该属性是对于该节点上有多个人处理有效。 </li>
                                        <li>比如:A发送到B,B节点上有张三，李四，王五可以处理，如果是李四发送的，该设置是否需要把李四排除掉。</li>
                                    </ul>
                                </td>
                            </tr>


                        </table>
                    </div>
                </div>
            </div>
            <div data-options="region:'south'" style="height: 50px; float:left;">
                <div><span style="color:Red;"><%=PageMessage %></span></div>
                <asp:Button ID="Btn_Save" runat="server" Text="保存" OnClick="Btn_Save_Click" />
                <asp:Button ID="Btn_SaveAndClose" runat="server" Text="保存并关闭" OnClick="Btn_SaveAndClose_Click" />
            </div>
        </div>
    </div>
</asp:Content>
