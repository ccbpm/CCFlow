
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FormDesigner.aspx.cs" Inherits="CCFlow.WF.Admin.CCFormDesigner.FormDesigner" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>CCForm 表单设计器 </title>
    <meta http-equiv="X-UA-Compatible" content="IE=9" />
    <link rel="stylesheet" type="text/css" href="../../Scripts/easyUI/themes/default/easyui.css" />
    <link rel="stylesheet" type="text/css" href="../../Scripts/easyUI/themes/icon.css" />
    <link rel="stylesheet" media="screen" type="text/css" href="./assets/css/style.css" />
    <link rel="stylesheet" media="screen" type="text/css" href="./assets/css/minimap.css" />
    <script type="text/javascript" src="./assets/javascript/json2.js"></script>
    <script type="text/javascript" src="./assets/javascript/jquery-1.11.0.min.js"></script>
    <script type="text/javascript" src="../../Scripts/easyUI/jquery.easyui.min.js"></script>
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
    <script type="text/javascript" src="./lib/commands/ConnectorCloneCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/ShapeChangePropertyCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/CanvasChangeColorCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/CanvasChangeSizeCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/CanvasFitCommand.js"></script>
    <script type="text/javascript" src="./lib/commands/InsertedImageFigureCreateCommand.js"></script>
    <script type="text/javascript" src="./Controls/Controls.js"></script>
    <script type="text/javascript" src="./Controls/ControlsPropertys.js"></script>
    <script type="text/javascript" src="./assets/javascript/colorPicker_new.js"></script>
    <link rel="stylesheet" media="screen" type="text/css" href="./assets/css/colorPicker_new.css" />
    <script src="../../Scripts/CommonUnite.js" type="text/javascript"></script>
    <script src="../../Scripts/EasyUIUtility.js" type="text/javascript"></script>
    <script src="js/FormDesignerService.js" type="text/javascript"></script>
    <script src="js/FormDesigner.js" type="text/javascript"></script>
    <!--[if IE]>
        <script src="./assets/javascript/excanvas.js"></script>
    <![endif]-->
    <link href="Img/Menu/icon.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        ToolbarIcon
        {
            text-align: left;
            width: 9px;
            height: 9px;
            border: 0px;
            vertical-align: top;
            font-size: 19px;
        }
    </style>
</head>
<body id="body">
<%
  BP.Sys.MapData md = new BP.Sys.MapData(this.Request.QueryString["FK_MapData"]);
            if (md.DesignerTool != "H5")
            {
               // BP.Sys.PubClass.WinCloseAndAlertMsg("@当前表单并非是H5表单设计器设计的，您不能打开。");
               // return;
            }
 %>
    <div id="actions" class="actions" style="font-size: 12px;">
        <a style="text-decoration: none;" href="javascript:Save(true);" title="保存(Ctrl-S)">
            <img src="assets/images/icon_save.jpg" id="SaveImg" border="0" width="16" height="16" />
            <label for="SaveImg" class="toolbarText">保存</label>
        </a>
        <img class="separator" src="assets/images/toolbar_separator.gif" border="0" width="1" height="18" alt='预览表单工作模式.' />
        <a style="text-decoration: none;" href="javascript:CCForm_BrowserView();" title="预览">
            <img src="Img/toolbar/View.png" id="BrowserView" border="0" width="16" height="16" alt="" />
            <label for="BrowserView" class="toolbarText">预览</label>
        </a>

    <img class="separator" src="assets/images/toolbar_separator.gif" border="0" width="1" height="18" alt='设计傻瓜表单.' />
        <a style="text-decoration: none;" href="javascript:CCForm_FoolFrm();" title="傻瓜表单">
            <img src="Img/toolbar/View.png" id="Img2" border="0" width="16" height="16" alt="" />
            <label for="BrowserView" class="toolbarText">设计傻瓜表单</label>
        </a>

        <img class="separator" src="assets/images/toolbar_separator.gif" border="0" width="1" height="18" alt='导出表单模版.' />
        <a style="text-decoration: none;" href="javascript:Exp();" title="导出表单模版">
            <img src="Img/toolbar/Exp.png" id="ExpFrmXml" border="0" width="16" height="16" alt="" />
            <label for="ExpFrmXml" class="toolbarText">导出</label>
        </a>
        <img class="separator" src="assets/images/toolbar_separator.gif" border="0" width="1" height="18" />
        <a style="text-decoration: none;" href="javascript:Imp();" title="导入表单模版">
            <img src="Img/toolbar/Imp.png" id="ImpFrmXml" border="0" width="16" height="16" alt="" />
            <label for="ImpFrmXml" class="toolbarText">导入</label>
        </a>
        <img class="separator" src="assets/images/toolbar_separator.gif" border="0" width="1" height="18" />
        <a style="text-decoration: none;" href="javascript:Show_HidenField_Panel();" title="显示隐藏字段">
            <img src="Img/toolbar/hidenF.png" id="HField" border="0" width="16" height="16" alt="" />
            <label for="HField" class="toolbarText">隐藏字段</label>
        </a>
        <img class="separator" src="assets/images/toolbar_separator.gif" border="0" width="1" height="18" />
        <a style="text-decoration: none; display:inline;" href="javascript:Show_HidenField_Panel();" title="左对齐">
            <img src="Img/toolbar/Alignment_Left.png" id="Img1" border="0" alt="" />
            <label class="toolbarText">左对齐</label>
        </a>
        <img class="separator" src="assets/images/toolbar_separator.gif" border="0" width="1" height="18" />
            <input type="checkbox" onclick="showGrid();" checked="checked" id="gridCheckbox" title="显示网格" style="vertical-align: middle;" />
            <label for="gridCheckbox" class="toolbarText">显示网格</label>
        <img class="separator" src="assets/images/toolbar_separator.gif" border="0" width="1" height="18" />
            <input type="checkbox" onclick="snapToGrid();" id="snapCheckbox" title="粘合网格线" style="vertical-align: middle;" />
            <label for="snapCheckbox" class="toolbarText">粘合网格线</label>
        <script type="text/javascript" language="javascript">
            if (!isBrowserReady()) {
                document.write('<span style="background-color: red;" >');
                document.write("您的浏览器不支持HTML5。请升级您的浏览器到高级版本，或者使用火狐、谷歌浏览器。");
                document.write("</span>");
            }
        </script>
    </div>
    <div id="editor">
        <div id="figures" style="height: 100%; overflow: auto; position: relative;">
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
            <div style="background-color: #eae9e9; padding: 3px; background-image:url(Img/Form.png); background-repeat:no-repeat; background-position:left;background-size:16px 16px;">
                <span style="margin-left:15px; font-size:small;">属性设置</span>
            </div>
            <center>
                <div style="overflow: auto;" id="edit">
                </div>
            </center>
            <div style="overflow:auto;" id="editCaption">
            </div>
            <div id="minimap" style="position: absolute; bottom: 18px; display: none;">
            </div>
        </div>
    </div>
    <br />
    <div id="mFormSheet" class="easyui-menu" style="width: 120px;">
        <%--<div onclick="NodeCreate_ByFlowMenu()" data-options="iconCls:'icon-AddNode'">添加节点</div>
        <div onclick="action('connector-straight')" data-options="iconCls:'icon-Line'">添加连线</div>
        <div onclick="createFigure(figure_Text, 'assets/images/text.gif')" data-options="iconCls:'icon-Text'">添加标签</div>
        <div class="menu-sep">
        </div>
        <div onclick="FlowProperty()" data-options="iconCls:'icon-property'">流程属性</div>
        <div onclick="Run_Flow()" data-options="iconCls:'icon-RunFlow'">运行流程</div>--%>
        <div onclick="View()" data-options="iconCls:'icon-CheckFlow'">预览</div>
        <div class="menu-sep">
        </div>
        <div onclick="GridLineVisible()" data-options="iconCls:'icon-new'">
            <span id="div_gridvisible">隐藏网格</span>
        </div>
    </div>
    <div id="textMenu" class="easyui-menu" style="width: 180px;">
        <div data-options="iconCls:'icon-delete',name:'text_delete'">删除</div>
    </div>
</body>
</html>
