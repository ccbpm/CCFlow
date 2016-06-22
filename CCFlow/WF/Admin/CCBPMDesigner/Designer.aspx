﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Designer.aspx.cs" Inherits="CCFlow.WF.Admin.CCBPMDesigner.Designer" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>CCBPM2.0</title>
    <meta http-equiv="X-UA-Compatible" content="IE=9" />
    <link href="../../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    
    <link href="./Img/Menu/Designer.css" rel="stylesheet" type="text/css" />


    <link rel="stylesheet" media="screen" type="text/css" href="./assets/css/style.css" />
    <link rel="stylesheet" media="screen" type="text/css" href="./assets/css/minimap.css" />

    <script type="text/javascript" src="./assets/javascript/json2.js"></script>
    <script type="text/javascript" src="./assets/javascript/jquery-1.11.0.min.js"></script>
    <script src="../../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="./lib/dashed.js"></script>
    <script type="text/javascript" src="./lib/canvasprops.js"></script>
    <script type="text/javascript" src="./lib/style.js"></script>
    <script type="text/javascript" src="./lib/primitives.js"></script>
    <script type="text/javascript" src="./lib/ImageFrame.js"></script>
    <script type="text/javascript" src="./lib/matrix.js"></script>
    <script type="text/javascript" src="./lib/util.js"></script>
    <script type="text/javascript" src="./lib/key.js"></script>
    <script type="text/javascript" src="./lib/groups.js"></script>
    <script type="text/javascript" src="./lib/stack.js"></script>
    <script type="text/javascript" src="./lib/connections.js"></script>
    <script type="text/javascript" src="./lib/connectionManagers.js"></script>
    <script type="text/javascript" src="./lib/handles.js"></script>
    <script type="text/javascript" src="./lib/builder.js"></script>
    <script type="text/javascript" src="./lib/text.js"></script>
    <script type="text/javascript" src="./lib/log.js"></script>
    <script type="text/javascript" src="./lib/browserReady.js"></script>
    <script type="text/javascript" src="./lib/containers.js"></script>
    <script type="text/javascript" src="./lib/importer.js"></script>
    <script type="text/javascript" src="./lib/main.js" charset="UTF-8"></script>
    <script type="text/javascript" src="./lib/minimap.js"></script>
    <script type="text/javascript" src="./lib/commands/History.js"></script>
    <script type="text/javascript" src="./lib/commands/FigureCreateCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/FigureCloneCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/FigureTranslateCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/FigureRotateCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/FigureScaleCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/FigureZOrderCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/FigureDeleteCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/GroupRotateCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/GroupScaleCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/GroupCreateCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/GroupCloneCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/GroupDestroyCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/GroupDeleteCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/GroupTranslateCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/ContainerCreateCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/ContainerDeleteCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/ContainerTranslateCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/ContainerScaleCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/ConnectorCreateCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/ConnectorDeleteCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/ConnectorAlterCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/ShapeChangePropertyCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/CanvasChangeColorCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/CanvasChangeSizeCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/CanvasFitCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/InsertedImageFigureCreateCommand.js"></script>
    <script type="text/javascript" src="./assets/javascript/colorPicker_new.js"></script>
    <script src="lib/sets/bpmn/Activity/Activity.js" type="text/javascript" charset="gb2312"></script>
    <script src="lib/sets/bpmn/Gateway/Gateway.js" type="text/javascript" charset="gb2312"></script>
    <script src="lib/sets/bpmn/Event/Event.js" type="text/javascript" charset="gb2312"></script>
    <script src="lib/sets/ccbpm/CCBPM.js" type="text/javascript"></script>
    <script src="js/ChangeNodeEventFactory.js" type="text/javascript"></script>
    <link rel="stylesheet" media="screen" type="text/css" href="./assets/css/colorPicker_new.css" />
    <script src="js/CCBPMDesignerData.js" type="text/javascript"></script>
    <script src="js/Designer.js" type="text/javascript"></script>
    <!--[if IE]>
        <script src="./assets/javascript/excanvas.js"></script>
    <![endif]-->
     

    <style type="text/css" >
     ToolbarIcon
     {
          text-align:left;
          width:9px;
          height:9px;
          border:0px;
          vertical-align:top;
          font-size:19px;
     }
    
    </style>
</head>
<body id="body">
    <div id="actions">
        <a style="text-decoration: none;" href="javascript:save(true);" title="保存(Ctrl-S)">
            <img src="assets/images/icon_Save.jpg" id="SaveImg" border="0" width="14" height="14" /><label for="SaveImg" class="toolbarText">保存</label> </a>
        <img class="separator" src="assets/images/toolbar_separator.gif" border="0" width="1" height="18" />
        <a style="text-decoration: none;" href="javascript:FlowProperty();" title="属性">
            <img src="Img/Menu/property.png"   id="FlowAttr"   border="0" width="14" height="14" alt="流程属性,设置整个流程信息." /><label for="FlowAttr" class="toolbarText">流程属性</label></a>
        <img class="separator" src="assets/images/toolbar_separator.gif" border="0" width="1" height="18" />
        <a style="text-decoration: none;" href="javascript:Check_Flow();" title="检查流程">
            <img src="Img/Menu/CheckFlow.png"  id="FlowCheck"  border="0" width="14" height="14"  alt="检查流程设计是否正确，自动修复数据表......" /><label for="FlowCheck" class="toolbarText">检查流程</label></a>
        <img class="separator" src="assets/images/toolbar_separator.gif" border="0" width="1" height="18" />
        <a style="text-decoration: none;" href="javascript:Run_Flow();"  title="运行流程">
            <img src="Img/Menu/RunFlow.png"  id="FlowRun"   border="0" width="14" height="14"  alt="测试并运行流程." /><label for="FlowRun" class="toolbarText">运行流程</label> </a>
        <img class="separator" src="assets/images/toolbar_separator.gif" border="0" width="1" height="18" />
        <a  style="text-decoration: none;" href="javascript:action('connector-straight');" title="直线连接">
            <img src="Img/Menu/Line.gif" border="0" id="Line" /> <label for="Line" class="toolbarText">直线连接</label> </a>
        <img class="separator" src="assets/images/toolbar_separator.gif" border="0" width="1" height="18" />
        <a  style="text-decoration: none;" href="javascript:action('connector-jagged');" title="折线连接">
            <img src="assets/images/icon_connector_jagged.gif" border="0" id="LineZX" /> <label for="LineZX" class="toolbarText">折线连接</label>  </a>
       <%-- <img class="separator" src="assets/images/toolbar_separator.gif" border="0" width="1" height="18" />
        <a href="javascript:action('connector-organic');" title="曲线连接">
            <img src="assets/images/icon_connector_organic.gif" border="0" alt="曲线连接" id="Organic" /> <label for="Organic" class="toolbarText">折线连接</label>  </a>--%>
        <%--<img class="separator" src="assets/images/toolbar_separator.gif" border="0" width="1"
            height="18" />
        <a href="javascript:action('container');" title="Container (Experimental)">
            <img src="assets/images/container.png" border="0" alt="Container" /></a>--%>

        <img class="separator" src="assets/images/toolbar_separator.gif" border="0" width="1" height="18"/>            
        <input type="checkbox" onclick="showGrid();" checked="checked" id="gridCheckbox"  title="显示网格" style="vertical-align:middle;"/>
        <label for="gridCheckbox" class="toolbarText">显示网格</label>
        <img class="separator" src="assets/images/toolbar_separator.gif" border="0" width="1" height="18"/>
        <input type="checkbox" onclick="snapToGrid();" id="snapCheckbox" title="粘合网格线" style="vertical-align:middle;"/>
        <label for="snapCheckbox" class="toolbarText">粘合网格线</label>

        <img class="separator" src="assets/images/toolbar_separator.gif" border="0" width="1"
            height="18" />
        <a href="javascript:createFigure(figure_Text, 'assets/images/text.gif');" title="添加标签" >
            <img src="assets/images/text.gif" border="0" height="18" /></a>
        <%--<img class="separator" src="assets/images/toolbar_separator.gif" border="0" width="1"
            height="18" />
        <a href="javascript:Conver_CCBPM_V1ToV2();" title="重新转化流程">
            <img src="assets/images/rotate.png" border="0" height="18" /></a>--%>
        <%--<img class="separator" src="assets/images/toolbar_separator.gif" border="0" width="1"
            height="18" />
            <a href="javascript:action('duplicate');">Copy</a>--%>
        <%--<img class="separator" src="assets/images/toolbar_separator.gif" border="0" width="1"
            height="18" />
        <a href="javascript:action('undo');" title="Undo (Ctrl-Z)">
            <img src="assets/images/arrow_undo.png" border="0" /></a>
        <img class="separator" src="assets/images/toolbar_separator.gif" border="0" width="1"
            height="18" />
        <a href="javascript:action('redo');" title="Redo (Ctrl-Y)">
            <img src="assets/images/arrow_redo.png" border="0" /></a>--%>
        <script type="text/javascript" language="javascript">
            if (!isBrowserReady()) {
                document.write('<span style="background-color: red;" >');
                document.write("您的浏览器不支持HTML5。请升级您的浏览器到高级版本，或者使用火狐、谷歌浏览器。");
                document.write("</span>");
            }
        </script>
    </div> 
    <div id="editor">
        <div id="figures" style="height: 100%; overflow:auto;position:relative;">
            <%--<select style="width: 120px;" onchange="setFigureSet(this.options[this.selectedIndex].value);">
                <script type="text/javascript">
                    for (var setName in figureSets) {
                        var set = figureSets[setName];
                        document.write('<option value="' + setName + '">' + set['name'] + '</option>');
                    }
                </script>
            </select>--%>
            <div id="minimap" style="position:absolute; bottom:18px;">
            </div>
        </div>
        <!--THE canvas-->
        <div style="width: 100%">
            <div id="container">
                <canvas id="a" width="800" height="500" style="overflow: auto;">
                <fieldset>
                <legend>提示</legend>
                        您的浏览器不支持HTML5，请升级您的浏览器到高级版本IE10+，或者使用火狐、谷歌浏览器。
                        </fieldset>
                </canvas>
                <div id="text-editor">
                </div>
                <div id="text-editor-tools">
                </div>
            </div>
        </div>
        <!--Right panel-->
        <div id="right">
            <center>
                <div style="overflow: auto; display: none;" id="edit">
                </div>
            </center>
        </div>
    </div>
    <br />
    <div id="mFlowSheet" class="easyui-menu" style="width: 120px;">
        <div onclick="NodeCreate_ByFlowMenu()" data-options="iconCls:'icon-AddNode'">添加节点</div>
        <div onclick="action('connector-straight')" data-options="iconCls:'icon-Line'">添加连线</div>
        <div onclick="createFigure(figure_Text, 'assets/images/text.gif')" data-options="iconCls:'icon-Text'">添加标签</div>
        <div class="menu-sep"></div>
        <div onclick="FlowProperty()" data-options="iconCls:'icon-property'">流程属性</div>
        <div onclick="Run_Flow()" data-options="iconCls:'icon-RunFlow'">运行流程</div>
        <div onclick="Check_Flow()" data-options="iconCls:'icon-CheckFlow'">检查流程</div>
        <div class="menu-sep"></div>
        <div onclick="GridLineVisible()" data-options="iconCls:'icon-new'">
            <span id="div_gridvisible">隐藏网格</span>
        </div>
    </div>
    <div id="nodeMenu" class="easyui-menu" style="width: 180px;">
        <div data-options="iconCls:'icon-property',name:'NodePropertyNew'">节点属性</div>
        <%--<div data-options="iconCls:'icon-property',name:'NodeProperty'">节点属性</div>--%>
        <div data-options="iconCls:'icon-edit',name:'editnodename'">修改名称</div>
        <div id="nodeModel_Menu_For_CCBPM">
            <span>节点类型</span>
            <div>
                <div data-options="name:'NodeEve_Ordinary'">普通节点</div>
                <div data-options="name:'NodeEve_FL'">分流节点</div>
                <div data-options="name:'NodeEve_HL'">合流节点</div>
                <div data-options="name:'NodeEve_FHL'">分合流节点</div>
                <div data-options="name:'NodeEve_SubThread'">子线程节点</div>
            </div>
        </div>
        <div class="menu-sep"></div>
        <div data-options="iconCls:'icon-form',name:'NodeFromWorkModel'">表单方案</div>

        <div data-options="iconCls:'icon-form',name:'DesignerNodeFormSL'">设计节点表单</div>
        <div data-options="iconCls:'icon-form',name:'DesignerNodeForm'">设计节点表单(测试版)</div>

        <div class="menu-sep"></div> 
        <div data-options="iconCls:'icon-Sender',name:'NodeAccepterRole'">设置处理人(接受人)</div>
        <div data-options="iconCls:'icon-Sender',name:'BindStations'">绑定岗位</div>

        <div data-options="iconCls:'icon-CC',name:'NodeCCRole'">设置抄送人(呈阅人)</div>
<%--        <div class="menu-sep"></div>
        <div data-options="iconCls:'icon-Event',name:'NodeEvent'">事件&消息</div>
        <div data-options="iconCls:'icon-Cond',name:'FlowCompleteCond'">流程完成条件</div>
        <div data-options="iconCls:'icon-Listion',name:'Listion'">消息收听</div>
         <div data-options="iconCls:'icon-Node',name:'Node'" >批量设置</div>--%>

         <%--   <span>批量设置</span>
            <div>
               <div data-options="iconCls:'icon-edit',name:'ToolBar'" >功能按钮控制</div>
               <div data-options="iconCls:'icon-edit',name:'SubFlows'" >父子流程</div>
               <div data-options="iconCls:'icon-edit',name:'SelfToolbar'" >审核组件</div>
               <div data-options="iconCls:'icon-edit',name:'SelfToolbar'">移动设置</div>
               <div data-options="iconCls:'icon-edit',name:'SelfToolbar'">考核</div>
               <div data-options="iconCls:'icon-edit',name:'SelfToolbar'">自定义工具栏</div>
            </div>
        </div>--%>

        <%--<div onclick="SetBUnit()" data-options="iconCls:'icon-bind',name:'SetBUnit'"> 设置为节点模版(业务单元)</div>--%>
        <div class="menu-sep"></div>
        <div data-options="iconCls:'icon-delete',name:'deletenode'">删除</div>
    </div>
    <div id="lineMenu" class="easyui-menu" style="width: 180px;">
        <div data-options="iconCls:'icon-config',name:'linecondition'">设置方向条件</div>
        <div data-options="iconCls:'icon-delete',name:'deleteline'">删除连线</div>
    </div>
    <div id="textMenu" class="easyui-menu" style="width: 180px;">
        <div data-options="iconCls:'icon-edit',name:'text_edit'">编辑</div>
        <div data-options="iconCls:'icon-delete',name:'text_delete'">删除</div>
    </div>
    <input type="hidden" id="HD_BPMN_NodeID" />
    <input type="hidden" id="HD_BPMN_FigureID" />
</body>
</html>
