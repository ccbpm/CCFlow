//全局变量
var figureSetsURL = 'lib/sets/bpmn';
var CCPMB_Flow_V = "2";

$(function () {

    CCBPM_Data_FK_Flow = getArgsFromHref("FK_Flow");
    CCPMB_Flow_V = getArgsFromHref("Flow_V");

    /**default height for canvas*/
    CanvasProps.DEFAULT_HEIGHT = 2000;
    /**default width for canvas*/
    CanvasProps.DEFAULT_WIDTH = 1950;

    //初始化画板
    init(CCBPM_Data_FK_Flow);

    //显示网格
    showGrid();

    //右键菜单
    InitContexMenu();

    $("#nodeModel_Menu_For_CCBPM").hide();

    if (CCPMB_Flow_V == "0") {
        figureSets = [];
        figureSets = figureSets_CCBPM;
        figureSetsURL = 'lib/sets/ccbpm';
        Conver_CCBPM_V1ToV2();
        $("#nodeModel_Menu_For_CCBPM").show();
    } 
    
     if (CCPMB_Flow_V == "1") {
        figureSets = [];
        figureSets = figureSets_CCBPM;
        figureSetsURL = 'lib/sets/ccbpm';
        $("#nodeModel_Menu_For_CCBPM").show();
    }

    //初始节点元素
    buildPanel();
});

//初始化右键菜单
function InitContexMenu() {

	// 连线双击打开设置条件/节点双击打开属性
	$("#a").bind("dblclick", function (e) {
		var coords = getCanvasXY(e);
        var x = coords[0];
        var y = coords[1];
		lastClick = [x, y];
		// store id value (from Stack) of clicked text primitive
		var textPrimitiveId = -1;
		//find Connector at (x,y)
		var cId = CONNECTOR_MANAGER.connectorGetByXY(x, y);
		// check if we clicked a connector
        if (cId != -1) {
			textPrimitiveId = 0; // (0 by default)
			Line_MenusFuns({
				"iconCls" : "icon-edit",
				"name" : "linecondition"
			}, cId);
		} else {
			cId = CONNECTOR_MANAGER.connectorGetByTextXY(x, y);
			if (cId != -1) {

			} else {
				var fId = STACK.figureGetByXY(x, y);
				if (fId != -1) {
					$("#HD_BPMN_NodeID").val("");
					$("#HD_BPMN_FigureID").val(fId);
					var figure = STACK.figureGetById(fId);
					var tId = STACK.textGetByFigureXY(fId, x, y);
					if (tId == -1) {
						var bpm_Node = figure.CCBPM_OID;
						if (bpm_Node) {
							$("#HD_BPMN_NodeID").val(bpm_Node);
						}
						// 对应Designer.htm:196菜单div#nodeMenu子项div的data-options属性
						NodeProperty_Funs({
							"iconCls" : "icon-edit",
							"name" : "NodeProperty"
						});
					} else {
						$("#HD_BPMN_TextPrimitiveID").val(tId);
						TextProperty_Funs({
							"iconCls" : "icon-edit",
							"name" : "text_edit"
						});
					}
				}
			}
		}
	});

    //画板右键
    $("#a").bind('contextmenu', function (ev) {

        var coords = getCanvasXY(ev);
        var x = coords[0];
        var y = coords[1];
        lastClick = [x, y];

        // store id value (from Stack) of clicked text primitive
        var textPrimitiveId = -1;

        //find Connector at (x,y)
        var cId = CONNECTOR_MANAGER.connectorGetByXY(x, y);

        // check if we clicked a connector
        if (cId != -1) {
            textPrimitiveId = 0; // (0 by default)

            //右键菜单
            $('#lineMenu').menu({ onShow: function () {
            }, onClick: function (item) {
                Line_MenusFuns(item, cId);
            }
            });

            ev.preventDefault();
            $('#lineMenu').menu('show', {
                left: ev.pageX,
                top: ev.pageY
            });
        } else {
            cId = CONNECTOR_MANAGER.connectorGetByTextXY(x, y);

            // check if we clicked a text of connector
            if (cId != -1) {
                textPrimitiveId = 0; // (0 by default)
            } else {
                //find Figure at (x,y)
                var fId = STACK.figureGetByXY(x, y);

                // check if we clicked a figure
                if (fId != -1) {
                    var figure = STACK.figureGetById(fId);
                    var tId = STACK.textGetByFigureXY(fId, x, y);
                    // if we clicked text primitive inside of figure
                    if (tId !== -1) {
                        textPrimitiveId = tId;
                        $('#textMenu').menu({ onShow: function () {
                            $("#HD_BPMN_NodeID").val("");
                            $("#HD_BPMN_FigureID").val(fId);
                        }, onClick: TextProperty_Funs
                        });
                        //弹出右键菜单
                        ev.preventDefault();
                        $('#textMenu').menu('show', {
                            left: ev.pageX,
                            top: ev.pageY
                        });
                    } else {
                        $('#nodeMenu').menu({ onShow: function () {
                            $("#HD_BPMN_NodeID").val("");
                            $("#HD_BPMN_FigureID").val(fId);
                            var bpm_Node = figure.CCBPM_OID;
                            if (bpm_Node) {
                                $("#HD_BPMN_NodeID").val(bpm_Node);
                            }
                        }, onClick: NodeProperty_Funs
                        });

                        if (figure.CCBPM_Shape != null && figure.CCBPM_Shape == CCBPM_Shape_Node) {
                            //右键菜单
                            ev.preventDefault();
                            $('#nodeMenu').menu('show', {
                                left: ev.pageX,
                                top: ev.pageY
                            });
                        }
                    }                    
                } else {
                    //find Container at (x,y)
                    var contId = STACK.containerGetByXY(x, y);
                    ev.preventDefault();
                    $('#mFlowSheet').menu('show', {
                        left: ev.pageX,
                        top: ev.pageY
                    });
                }
            }
        }
    });
}

//初始化画板元素
function buildPanel() {
    //var first = true;
    for (var setName in figureSets) {
        var set = figureSets[setName];
        var groupSetDiv = document.createElement('div');
        groupSetDiv.className = "figurePanel";
        groupSetDiv.addEventListener("click", function (setName) {
            return function (evt) {
                evt.preventDefault();
                setFigurePanel(setName);
            };
        } (setName), false);

        var groupImg = document.createElement('img');
        groupImg.setAttribute('src', "Icons/Min.png");
        groupImg.setAttribute('align', "middle");
        groupImg.setAttribute('id', setName + "img");
        groupSetDiv.appendChild(groupImg);

        var groupText = document.createElement('div');
        groupText.innerHTML = set.name;
        groupSetDiv.appendChild(groupText);

        document.getElementById('figures').appendChild(groupSetDiv);

        var eSetDiv = document.createElement('div');
        eSetDiv.setAttribute('id', setName);
        document.getElementById('figures').appendChild(eSetDiv);

        //add figures to the div
        for (var figure in set['figures']) {
            figure = set['figures'][figure];

            var figureFunctionName = 'figure_' + figure.figureFunction;
            var figureThumbURL = figureSetsURL + '/' + setName + '/' + figure.image;

            var eFigure = document.createElement('img');
            eFigure.setAttribute('src', figureThumbURL);

            eFigure.addEventListener('mousedown', function (figureFunction, figureThumbURL) {
                return function (evt) {
                    evt.preventDefault();
                    createFigure(window[figureFunction], figureThumbURL);
                };
            } (figureFunctionName, figureThumbURL), false);

            //in case use drops the figure
            eFigure.addEventListener('mouseup', function () {
                createFigureFunction = null;
                selectedFigureThumb = null;
                state = STATE_NONE;
            }, false);

            eFigure.style.cursor = 'pointer';
            eFigure.style.marginRight = '5px';
            eFigure.style.marginTop = '2px';

            eSetDiv.appendChild(eFigure);
        }
    }
}

//统一弹出消息窗口
function Designer_ShowMsg(msg, callBack) {
    if (window.parent && window.parent.BPMN_Msg) {
        window.parent.BPMN_Msg(msg, callBack);
    } else {
        alert(msg);
        if (callBack) callBack();
    }
}

//流程属性
function FlowProperty() {

    url = "../../Comm/En.htm?EnName=BP.WF.Template.FlowExt&PK=" + CCBPM_Data_FK_Flow + "&Lang=CH";
    OpenEasyUiDialog(url, "eudlgframe", '流程属性', 1000, 550, "icon-property", true, null, null, null, function () {
        //window.location.href = window.location.href;
    });

    //    if (window.parent) {
    //        window.parent.addTab(CCBPM_Data_FK_Flow + "PO", "流程属性" + CCBPM_Data_FK_Flow, url);
    //    } else {
    //        WinOpen(url);
    //    }
}

//报表设计
function DesignMyRptNew() {

    var flowId = Number(CCBPM_Data_FK_Flow);
    flowId = String(flowId);

    url = "../RptDfine/Default.htm?FK_Flow=" + CCBPM_Data_FK_Flow + "&FK_MapData=ND" + flowId + "MyRpt";
    OpenEasyUiDialog(url, "eudlgframe", '流程属性', 990, 500, "icon-property", true, null, null, null, function () {

    });
}

//报表设计
function DesignMyRpt() {

    var flowId = Number(CCBPM_Data_FK_Flow);
    flowId = String(flowId);
    url = "../XAP/DoPort.htm?DoType=En&EnName=BP.WF.Rpt.MapRptExts&PK=ND" + flowId + "MyRpt&Lang=CH&SID=" + GetQueryString('SID') + "&UserNo=" + GetQueryString('UserNo');
    if (window.parent) {
        window.parent.addTab(CCBPM_Data_FK_Flow + "Rpt", "设计报表" + CCBPM_Data_FK_Flow, url);
    } else {
        WinOpen(url);
    }
}

//连线右键
function Line_MenusFuns(item, cId) {
    var rFirstFigure = STACK.figureGetAsFirstFigureForConnector(cId);
    var rSecondFigure = STACK.figureGetAsSecondFigureForConnector(cId);
    //连接线右键菜单点击事件
    switch (item.name) {
        case "linecondition":
            if (rFirstFigure == null) {
                Designer_ShowMsg("所选连线起点没有连接节点，不能设置方向条件。");
                return;
            }
            if (rSecondFigure == null) {
                Designer_ShowMsg("所选连线终点没有连接节点，不能设置方向条件。");
                return;
            }
            if (rFirstFigure.CCBPM_Shape != CCBPM_Shape_Node) {
                Designer_ShowMsg("所选连线起点连接的不是节点，不能设置方向条件。");
                return;
            }
            if (rSecondFigure.CCBPM_Shape != CCBPM_Shape_Node) {
                Designer_ShowMsg("所选连线终点连接的不是节点，不能设置方向条件。");
                return;
            }
            if (rFirstFigure.CCBPM_Shape == CCBPM_Shape_Node && rSecondFigure.CCBPM_Shape == CCBPM_Shape_Node) {
                var fNode = rFirstFigure.CCBPM_OID;
                var tNode = rSecondFigure.CCBPM_OID;
				var url = "../Cond/ConditionLine.htm?FK_Flow=" + CCBPM_Data_FK_Flow + "&FK_MainNode=" + fNode + "&FK_Node=" + fNode + "&ToNodeID=" + tNode + "&CondType=2&Lang=CH&t=" + new Date().getTime();
                //window.parent.addTab(CCBPM_Data_FK_Flow + fNode + "DIRECTION" + tNode, "设置方向条件" + fNode + "->" + tNode, url);
                OpenEasyUiDialog(url, CCBPM_Data_FK_Flow + fNode + "DIRECTION" + tNode, "设置方向条件" + fNode + "->" + tNode, 880, 500, "icon-property", true, null, null, null, function () {

                });
            }
            break;
		case "rename" :
			var connector = CONNECTOR_MANAGER.connectorGetById(cId);
			shape = connector;
			textPrimitiveId = 0; // (0 by default)
			if (textPrimitiveId != -1) {
				// if group selected
				if (state == STATE_GROUP_SELECTED) {
					var selectedGroup = STACK.groupGetById(selectedGroupId);
					// if group is temporary then destroy it
					if (!selectedGroup.permanent) {
						STACK.groupDestroy(selectedGroupId);
					}
					//deselect current group
					selectedGroupId = -1;
				}
				// deselect current figure
				selectedFigureId = -1;
				// deselect current container
				selectedContainerId = -1;
				// deselect current connector
				selectedConnectorId = -1;
				// set current state
				state = STATE_TEXT_EDITING;
				// set up text editor
				setUpTextEditorPopup(shape, textPrimitiveId);
				redraw = true;
			}
			draw();
			break;
        case "deleteline":
            CONNECTOR_MANAGER.connectorRemoveById(cId, true);
            state = STATE_NONE;
            redraw = true;
            draw();
            break;
    }
}

//添加节点
function NodeCreate_ByFlowMenu() {
    var x = lastClick[0];
    var y = lastClick[1];
    if (CCPMB_Flow_V == "2") {
        //BPMN
        var cmdCreateFig = new FigureCreateCommand(figure_UserTask, x, y);
        cmdCreateFig.execute();
    } else {
        var cmdCreateFig = new FigureCreateCommand(figure_NodeOrdinary, x, y);
        cmdCreateFig.execute();
    }
}
//节点属性
function NodeProperty_Funs(item) {
    var figureId = $("#HD_BPMN_FigureID").val();
    var FK_Node = $("#HD_BPMN_NodeID").val();
    if (FK_Node == "") {
        alert("节点编号不存在，请删除后重新添加。");
        return;
    }

    //根据事件名称进行执行
    switch (item.name) {
        case "NodeProperty": //节点属性.
            url = "../../Comm/En.htm?EnsName=BP.WF.Template.NodeExts&NodeID=" + FK_Node + "&Lang=CH";
            // alert(url);
            if (window.parent) {
                window.parent.addTab(CCBPM_Data_FK_Flow + FK_Node + "PO", "节点属性" + FK_Node, url, item.iconCls);
            } else {
                WinOpen(url);
            }
            break;
        case "Node_EditNodeName": //修改节点名称
            var figure = STACK.figureGetById(figureId);
            var tId = 1; //STACK.textGetByFigureXY(fId, x, y);

            // check if we clicked a text primitive inside of shape
            if (tId != -1) {
                // deselect current figure
                selectedFigureId = -1;

                // deselect current container
                selectedContainerId = -1;

                // deselect current connector
                selectedConnectorId = -1;

                // set current state
                state = STATE_TEXT_EDITING;

                // set up text editor
                setUpTextEditorPopup(figure, 1);
                redraw = true;
                draw();
            }
            break;
        case "NodeEve_Ordinary": //节点转化为普通节点
            var changeNode = new ChangeNodeManager(figureId);
            changeNode.NodeOrdinary();
            break;
        case "NodeEve_FL": //节点转化为分流节点
            var changeNode = new ChangeNodeManager(figureId);
            changeNode.NodeFL();
            break;
        case "NodeEve_HL": //节点转化为合流节点
            var changeNode = new ChangeNodeManager(figureId);
            changeNode.NodeHL();
            break;
        case "NodeEve_FHL": //节点转化为分合流节点
            var changeNode = new ChangeNodeManager(figureId);
            changeNode.NodeFHL();
            break;
        case "NodeEve_SubThread": //节点转化为子线程节点
            var changeNode = new ChangeNodeManager(figureId);
            changeNode.NodeSubThread();
            break;

        case "NodeAccepterRole": // 工作处理人. NodeFromWorkModel
            //url = "../AttrNode/NodeAccepterRole.htm?FK_Node=" + FK_Node + "&Lang=CH";
            url = "../AttrNode/AccepterRole/Default.htm?FK_Node=" + FK_Node + "&Lang=CH";

            if (window.parent) {
                window.parent.addTab(CCBPM_Data_FK_Flow + FK_Node + "ND", "接收人" + FK_Node, url, item.iconCls);
            } else {
                WinOpen(url);
            }
            break;
        case "BindStations":  //绑定岗位.
            url = "../XAP/DoPort.htm?DoType=StaDefNew&PK=" + FK_Node + "&Lang=CH";
            WinOpenIt(url, 500, 400);
            break;
        // Glo.OpenDialog(Glo.BPMHost + url, "执行", 500, 400); 
        case "NodeCCRole": // 抄送人规则.
            url = "../AttrNode/NodeCCRole.htm?FK_Node=" + FK_Node + "&Lang=CH";
            if (window.parent) {
                window.parent.addTab(CCBPM_Data_FK_Flow + FK_Node + "ND", "抄送人规则" + FK_Node, url, item.iconCls);
            } else {
                WinOpen(url);
            }
            break;
        case "NodeEvent": // 节点事件.
            url = "./../AttrNode/Action.htm?NodeID=" + FK_Node + "&FK_Flow=" + CCBPM_Data_FK_Flow;
            if (window.parent) {
                window.parent.addTab(CCBPM_Data_FK_Flow + FK_Node + "ND", "节点事件" + FK_Node, url, item.iconCls);
            } else {
                WinOpen(url);
            }
            break;
        case "FlowCompleteCond": // 流程完成条件..
            url = "../Cond.htm?CondType=1&FK_Flow=" + CCBPM_Data_FK_Flow + "&FK_MainNode=" + FK_Node + "&FK_Node=" + FK_Node + "&FK_Attr=&DirType=&ToNodeID=" + FK_Node;
            if (window.parent) {
                window.parent.addTab(CCBPM_Data_FK_Flow + FK_Node + "ND", "流程完成条件" + FK_Node, url, item.iconCls);
            } else {
                WinOpen(url);
            }
            break;
        case "AdvFunc": // 高级功能..
        case "Node": // 批量设置 ..
            url = "../AttrFlow/NodeAttrs.htm?CondType=1&FK_Flow=" + CCBPM_Data_FK_Flow + "&FK_MainNode=" + FK_Node + "&FK_Node=" + FK_Node + "&FK_Attr=&DirType=&ToNodeID=" + FK_Node;
            //alert(url);
            if (window.parent) {
                window.parent.addTab(CCBPM_Data_FK_Flow + FK_Node + "ND", "批量设置" + FK_Node, url, item.iconCls);
            } else {
                WinOpen(url);
            }
            break;
        case "SelfToolbar": // 自定义工具栏..
            url = "../Cond.htm?CondType=1&FK_Flow=" + CCBPM_Data_FK_Flow + "&FK_MainNode=" + FK_Node + "&FK_Node=" + FK_Node + "&FK_Attr=&DirType=&ToNodeID=" + FK_Node;
            if (window.parent) {
                window.parent.addTab(CCBPM_Data_FK_Flow + FK_Node + "ND", "自定义工具栏" + FK_Node, url, item.iconCls);
            } else {
                WinOpen(url);
            }
            break;
        case "NodeFromWorkModel": // 设置表单. NodeFromWorkModel
            //url = "../AttrNode/NodeFromWorkModel.htm?FK_Flow=" + CCBPM_Data_FK_Flow + "&FK_Node=" + FK_Node + "&Lang=CH";
            url = "../AttrNode/FrmSln/Default.htm?FK_Flow=" + CCBPM_Data_FK_Flow + "&FK_Node=" + FK_Node + "&Lang=CH";
            //var url = "../AttrNode/FrmSln/Default.htm?FK_Node=" + nodeID;

            if (window.parent) {
                window.parent.addTab(CCBPM_Data_FK_Flow + FK_Node + "ND", "设置表单" + FK_Node, url, item.iconCls);
            } else {
                WinOpen(url);
            }
            break;
        case "DesignerNodeFormFixOld": //设计傻瓜表单.

//            if (plant == "JFlow") {
//                alert("请选择\"设计节点表单(H5测试版)\"");
//                break;
//            }

            url = "../FoolFormDesigner/Designer.aspx?IsFirst=1&FK_MapData=ND" + FK_Node + "&FK_Flow=" + CCBPM_Data_FK_Flow + "&FK_Node=" + FK_Node;
            if (window.parent && 1 == 3) {
                window.parent.addTab(CCBPM_Data_FK_Flow + FK_Node + "ND", "傻瓜表单" + FK_Node, url, item.iconCls);
            } else {
                WinOpen(url);
            }
            break;
        case "DesignerNodeFormFixNew": //设计傻瓜表单.

//            if (plant == "JFlow") {
//                alert("请选择\"设计节点表单(H5测试版) jflow 暂不支持\"");
//                break;
//            }

            url = "../FoolFormDesigner/Designer.htm?IsFirst=1&FK_MapData=ND" + FK_Node + "&FK_Flow=" + CCBPM_Data_FK_Flow + "&FK_Node=" + FK_Node;
            if (window.parent && 1 == 3) {
                window.parent.addTab(CCBPM_Data_FK_Flow + FK_Node + "ND", "傻瓜表单" + FK_Node, url, item.iconCls);
            } else {
                WinOpen(url);
            }
            break;
        case "DesignerNodeFormFix": //设计傻瓜表单.
            if (plant == "JFlow") {
                alert("请选择\"设计节点表单(H5测试版)\"");
                break;
            }
            url = "../FoolFormDesigner/Designer.htm?IsFirst=1&FK_MapData=ND" + FK_Node + "&FK_Flow=" + CCBPM_Data_FK_Flow + "&FK_Node=" + FK_Node;
            if (window.parent && 1 == 3) {
                window.parent.addTab(CCBPM_Data_FK_Flow + FK_Node + "ND", "傻瓜表单" + FK_Node, url, item.iconCls);
            } else {
                WinOpen(url);
            }
            break;
        case "DesignerNodeFormSL": //设计表单
//            if (plant == "JFlow")
//                alert("请选择\"设计节点表单(H5测试版)\"");
//            break;
            url = "../CCFormDesigner/CCFormDesignerSL.htm?FK_Flow=" + CCBPM_Data_FK_Flow + "&FK_MapData=ND" + FK_Node + "&UserNo=" + window.parent.WebUser.No + "&SID=" + window.parent.WebUser.SID;
            if (window.parent && 1 == 3) {
                window.parent.addTab(CCBPM_Data_FK_Flow + FK_Node + "ND", "自由表单" + FK_Node, url, item.iconCls);
            } else {
                WinOpen(url);
            }
            break;
        case "DesignerNodeForm": //设计表单.
            url = "../CCFormDesigner/FormDesigner.htm?FK_Node=" + FK_Node + "&FK_MapData=ND" + FK_Node + "&FK_Flow=" + CCBPM_Data_FK_Flow + "&UserNo=" + window.parent.WebUser.No + "&SID=" + window.parent.WebUser.SID;
            if (window.parent) {
                window.parent.addTab(CCBPM_Data_FK_Flow + FK_Node + "ND", "设计表单" + FK_Node, url, item.iconCls);
            } else {
                WinOpen(url);
            }
            break;
        case "bindflowfrms": //绑定独立表单
            url = "../Sln/BindFrms.htm?ShowType=FlowFrms&FK_Flow=" + CCBPM_Data_FK_Flow + "&FK_Node=" + FK_Node + "&Lang=CH";
            if (window.parent) {
                window.parent.addTab(CCBPM_Data_FK_Flow + FK_Node + "FRM", "绑定独立表单" + FK_Node, url, item.iconCls);
            } else {
                WinOpen(url);
            }
            break;
        case "deletenode": //删除节点
            var cmdDelFig = new FigureDeleteCommand(selectedFigureId);
            cmdDelFig.execute();
            draw();
            break;
        default:
            alert('没有处理的菜单ID:' + item.name);
            break;
    }

    $('#nodeMenu').menu('hide');
}

function TextProperty_Funs(item) {
    var figureId = $("#HD_BPMN_FigureID").val();
    var textPrimitiveId = $("#HD_BPMN_TextPrimitiveID").val() || 0;
    //根据事件名称进行执行
    switch (item.name) {
        case "text_edit": //编辑文本
            var figure = STACK.figureGetById(figureId);
            // check if we clicked a text primitive inside of shape
            // deselect current figure
            selectedFigureId = -1;
            // deselect current container
            selectedContainerId = -1;
            // deselect current connector
            selectedConnectorId = -1;
            // set current state
            state = STATE_TEXT_EDITING;
            // set up text editor
            setUpTextEditorPopup(figure, textPrimitiveId);
            redraw = true;
            draw();
            break;
        case "text_delete": //删除文本
            var cmdDelFig = new FigureDeleteCommand(figureId);
            cmdDelFig.execute();
            draw();
            break;
    }
}

//运行流程
function Run_Flow() {
    var url = "../TestFlow.htm?FK_Flow=" + CCBPM_Data_FK_Flow + "&Lang=CH";
    OpenEasyUiDialog(url, "eudlgframe", '流程测试运行', 900, 500, "icon-property", true, null, null, null, function () {
        //window.location.href = window.location.href;
    });
}

//运行流程
function Beta() {

    var url = "Designer2018.htm?FK_Flow=" + CCBPM_Data_FK_Flow + "&Lang=CH";

     window.location.href=url;

    
}

//检查流程
function Check_Flow() {
    var url = "../AttrFlow/CheckFlow.htm?FK_Flow=" + CCBPM_Data_FK_Flow + "&DoType11=FlowCheck&Lang=CH";
    OpenEasyUiDialog(url, "eudlgframe", '流程检查', 800, 500, "icon-property", true, null, null, null, function () {
        //window.location.href = window.location.href;
    });

    // WinOpen(url);
}

//工具栏展开缩放
function setFigurePanel(id) {
    var div = document.getElementById(id);
    if (div != null) {
        var divImg = document.getElementById(id + "img");
        var display = div.style.display;
        if (display == "none") {
            div.style.display = 'block';
            if (divImg) divImg.src = "Icons/Min.png";
        } else {
            div.style.display = 'none';
            if (divImg) divImg.src = "Icons/Max.png";
        }
    }
}

//网格显示
function GridLineVisible() {
    gridVisible = !gridVisible;
    document.getElementById("gridCheckbox").checked = gridVisible;
    backgroundImage = null; // reset cached background image of canvas
    //trigger a repaint;
    draw();

    var curVisible = document.getElementById("div_gridvisible").innerHTML;
    if (curVisible == "隐藏网格") {
        document.getElementById("div_gridvisible").innerHTML = "显示网格";
    } else if (curVisible == "显示网格") {
        document.getElementById("div_gridvisible").innerHTML = "隐藏网格";
    }
}
//打开窗体
function WinOpen(url) {
    var winWidth = 850;
    var winHeight = 680;
    if (screen && screen.availWidth) {
        winWidth = screen.availWidth;
        winHeight = screen.availHeight - 36;
    }
    window.open(url, "_blank", "height=" + winHeight + ",width=" + winWidth + ",top=0,left=0,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no");
}
//打开窗体
function WinOpenIt(url, winWidth, winHeight) {
    //var winWidth = 850;
    //var winHeight = 680;
//    if (screen && screen.availWidth) {
//        winWidth = screen.availWidth;
//        winHeight = screen.availHeight - 36;
    //    }

    var iTop = (window.screen.height - 30 - winHeight) / 2; //获得窗口的垂直位置;
    var iLeft = (window.screen.width - 10 - winWidth) / 2; //获得窗口的水平位置;
    window.open(url, "_blank", "height=" + winHeight + ",width=" + winWidth + ",top=" + iTop + ",left="+iLeft+",toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no");
}

//将v1版本流程转换为v2
function Conver_CCBPM_V1ToV2() {

    //transe old flow to bpm

    $.post(Handler, {
        DoType: 'Flow_AllElements_ResponseJson',
        FK_Flow: CCBPM_Data_FK_Flow
    }, function (data) {

        if (data.indexOf('err@') == 0) {
            alert(data);
            return;
        }

        var flow_Data = $.parseJSON(data);

        //循环节点
        for (var idx = 0; idx < flow_Data.Nodes.length; idx++) {

            var curNode = flow_Data.Nodes[idx];

            var createdFigure = figure_Node_Template(curNode.NODEID, curNode.NAME, curNode.X, curNode.Y, curNode.RUNMODEL);
            //move it into position
            createdFigure.transform(Matrix.translationMatrix(curNode.X - createdFigure.rotationCoords[0].x, curNode.Y - createdFigure.rotationCoords[0].y))
            createdFigure.style.lineWidth = defaultLineWidth;
            //add to STACK
            STACK.figureAdd(createdFigure);
        }

        //循环连接线
        for (var lineIdx = 0; lineIdx < flow_Data.Direction.length; lineIdx++) {
            var line = flow_Data.Direction[lineIdx];
            var fromFigureId = FigureIdGetByCCBPM_OID(line.NODE);
            var secondFigureId = FigureIdGetByCCBPM_OID(line.TONODE);

            if (fromFigureId != null && secondFigureId != null) {
                var cps_FromFigure = CONNECTOR_MANAGER.connectionPointGetAllByParent(fromFigureId);
                var cps_SecondFigure = CONNECTOR_MANAGER.connectionPointGetAllByParent(secondFigureId);
                var point_Start = cps_FromFigure[3];
                var point_End = cps_SecondFigure[2];

                var cId = CONNECTOR_MANAGER.connectorCreate(point_Start.point, point_End.point, Connector.TYPE_STRAIGHT);
                var cps = CONNECTOR_MANAGER.connectionPointGetAllByParent(cId);
                CONNECTOR_MANAGER.glueCreate(point_Start.id, cps[0].id, true);
                CONNECTOR_MANAGER.glueCreate(point_End.id, cps[1].id, true);
                //solutions
                var candidate = CONNECTOR_MANAGER.getClosestPointsOfConnection(true, true, fromFigureId, point_Start.point, secondFigureId, point_End.point);
                var rStartBounds = STACK.figureGetById(fromFigureId).getBounds();
                var rEndBounds = STACK.figureGetById(secondFigureId).getBounds();
                DIAGRAMO.debugSolutions = CONNECTOR_MANAGER.connector2Points(Connector.TYPE_STRAIGHT, candidate[0], candidate[1], rStartBounds, rEndBounds);
            }
        }
        //循环标签
        for (var labIdx = 0; labIdx < flow_Data.LabNote.length; labIdx++) {
            var labNote = flow_Data.LabNote[labIdx];
            var labX = labNote.X + 55;
            var labY = labNote.Y + 24;
            var createdFigure = figure_Text_Template(labNote.MYPK, labNote.NAME, labX, labY);
            //move it into position
            createdFigure.transform(Matrix.translationMatrix(labX - createdFigure.rotationCoords[0].x, labY - createdFigure.rotationCoords[0].y))
            createdFigure.style.lineWidth = defaultLineWidth;
            //add to STACK
            STACK.figureAdd(createdFigure);
        }

        redraw = true;
        draw();
        save(false);

    });
}

//流程转换节点模板
function figure_Node_Template(nodeId, name, x, y, NodeWorkType) {
    var f = new Figure("UserTask");

    //ccbpm
    f.CCBPM_Shape = "Node";
    f.CCBPM_OID = nodeId;
    f.style.fillStyle = FigureDefaults.fillStyle;
    f.style.strokeStyle = FigureDefaults.strokeStyle;

    //Image
    var url = figureSetsURL + "/Nodes/nodeOrdinary_big.png";
    switch (NodeWorkType) {
        case 0: ;
            url = figureSetsURL + "/Nodes/nodeOrdinary_big.png";
            break;
        case 1:
            url = figureSetsURL + "/Nodes/nodeHL_big.png";
            break;
        case 2:
            url = figureSetsURL + "/Nodes/nodeFL_big.png";
            break;
        case 3:
            url = figureSetsURL + "/Nodes/nodeFHL_big.png";
            break;
        case 4:
            url = figureSetsURL + "/Nodes/nodeSubThread_big.png";
            break;
    }

    var ifig = new ImageFrame(url, x, y, true, 90, 35);
    ifig.debug = true;
    f.addPrimitive(ifig);

    //Text
    f.properties.push(new BuilderProperty('Text', 'primitives.1.str', BuilderProperty.TYPE_TEXT));
    f.properties.push(new BuilderProperty('Text Size', 'primitives.1.size', BuilderProperty.TYPE_TEXT_FONT_SIZE));
    f.properties.push(new BuilderProperty('Font', 'primitives.1.font', BuilderProperty.TYPE_TEXT_FONT_FAMILY));
    f.properties.push(new BuilderProperty('Alignment', 'primitives.1.align', BuilderProperty.TYPE_TEXT_FONT_ALIGNMENT));
    f.properties.push(new BuilderProperty('Text Underlined', 'primitives.1.underlined', BuilderProperty.TYPE_TEXT_UNDERLINED));
    f.properties.push(new BuilderProperty('Text Color', 'primitives.1.style.fillStyle', BuilderProperty.TYPE_COLOR));

    f.properties.push(new BuilderProperty(BuilderProperty.SEPARATOR));
    f.properties.push(new BuilderProperty('URL', 'url', BuilderProperty.TYPE_URL));

    var t2 = new Text(name, x, y + 36, FigureDefaults.textFont, FigureDefaults.textSize);
    t2.style.fillStyle = FigureDefaults.textColor;
    f.addPrimitive(t2);

    //Connection Points
    CONNECTOR_MANAGER.connectionPointCreate(f.id, new Point(x + 54, y), ConnectionPoint.TYPE_FIGURE);
    CONNECTOR_MANAGER.connectionPointCreate(f.id, new Point(x - 54, y), ConnectionPoint.TYPE_FIGURE);
    CONNECTOR_MANAGER.connectionPointCreate(f.id, new Point(x, y - 24), ConnectionPoint.TYPE_FIGURE);
    CONNECTOR_MANAGER.connectionPointCreate(f.id, new Point(x, y + 45), ConnectionPoint.TYPE_FIGURE);

    f.finalise();
    return f;
}
//添加标签
function figure_Text_Template(myPK, name, x, y) {
    var f = new Figure('Text');
    //ccbpm
    f.CCBPM_Shape = "Text";
    f.CCBPM_OID = myPK;

    f.style.fillStyle = FigureDefaults.fillStyle;
    f.properties.push(new BuilderProperty('Text', 'primitives.0.str', BuilderProperty.TYPE_TEXT));

    //when we change textSize we need to transform the connectionPoints, 
    //this is the only time connecitonPoints get transformed for text
    f.properties.push(new BuilderProperty('Text Size ', 'primitives.0.size', BuilderProperty.TYPE_TEXT_FONT_SIZE));
    f.properties.push(new BuilderProperty('Font ', 'primitives.0.font', BuilderProperty.TYPE_TEXT_FONT_FAMILY));
    f.properties.push(new BuilderProperty('Alignment ', 'primitives.0.align', BuilderProperty.TYPE_TEXT_FONT_ALIGNMENT));
    f.properties.push(new BuilderProperty('Text Underlined', 'primitives.0.underlined', BuilderProperty.TYPE_TEXT_UNDERLINED));
    f.properties.push(new BuilderProperty('Text Color', 'primitives.0.style.fillStyle', BuilderProperty.TYPE_COLOR));
    //f.properties.push(new BuilderProperty('Vertical Alignment ', 'primitives.0.valign', Text.VALIGNMENTS);

    //    f.properties.push(new BuilderProperty(BuilderProperty.SEPARATOR));
    f.properties.push(new BuilderProperty('URL', 'url', BuilderProperty.TYPE_URL));


    var t2 = new Text(name, x, y + FigureDefaults.radiusSize / 2, FigureDefaults.textFont, FigureDefaults.textSize);
    t2.style.fillStyle = FigureDefaults.textColor;

    f.addPrimitive(t2);

    f.finalise();
    return f;
}

function FigureIdGetByCCBPM_OID(CCBPM_OID) {
    for (var i = 0; i < STACK.figures.length; i++) {
        if (STACK.figures[i].CCBPM_OID != null && STACK.figures[i].CCBPM_OID == CCBPM_OID) {
            return STACK.figures[i].id;
            break;
        }
    }
}