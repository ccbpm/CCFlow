"use strict";
/// <reference path="raphael.js" />
/// <reference path="../easyUI/jquery-1.8.0.min.js" />
/// <reference path="../easyUI/jquery.easyui.min.js" />

var 
// global value for designer
 diagram = {
     isChanged: false, // 标识当前流程是否更新
     flow: null  //  current instance of CCFlow
 },
 container = null, // 当前CCFlow的UI对象,一个CCFlow
 ACTION = {
    NONE: "none",
    NODE_CREATE: "node_create",
    NODE_SELECTED: "node_selected",
    NODE_MOVED :'node_moved',
    DIRECTION_CREATE: "direction_create",
    DIRECTION_SELECTED: "direction_selected",
    DIRECTION_PICK_FIRST: "direction_pick_first",
    DIRECTION_PICK_SECOND: "direction_pick_second",
    DIRECTION_MOVE_POINT: "direction_move_point",

    SELECTING_MULTIPLE: "selecting_multiple",
    CONTAINER_SELECTED: "container_selected",
    TEXT_EDITING: "text_editing"
},
 state = ACTION.NONE;


//定义样式变量，可在此统一修改流程样式
var DATA_USER_ICON_PATH = '../../../DataUser/UserIcon/';
var DATA_USER_ICON_DEFAULT = 'Default.png';
var NODE_WIDTH = 40,
    NODE_WIDTH_CENTER = 6,
    NODE_HEIGHT = 40,
    NODE_BORDER_RADIUS = 5;
var NODE_ICON_PATH = '../../../DataUser/NodeIcon/';
var NODE_ICON_DEFAULT = 'Default.jpg';


var LABEL_FONT_SIZE = 14;
var LABEL_FONT_COLOR_FORE = 'none',
    LABEL_FONT_COLOR_HOVER = 'blue',
    LABEL_FONT_COLOR_DRAG = 'blue',
    LABEL_BORDER_COLOR_NORAML = 'none',
    LABEL_BORDER_COLOR_HOVER = 'blue',
    LABEL_BORDER_COLOR_DRAG = 'blue';    

var STYLE_LINE_HOVER_COLOR = 'blue',
    STYLE_LINE_TRACK_COLOR = 'red';

var  
STYLE_NORMAL = {
    BORDER_COLOR: 'black',
    BORDER_WIDTH: 2,
    LINE_WIDTH : 2,
    FONT_FORE: "black",
    FONT_SIZE:12
},
 STYLE_HOVER = {
    BORDER_COLOR: 'green',
    BORDER_WIDTH: 2,
    FONT_FORE: "blue"
},
 STYLE_FOCUSED = {
     BORDER: 'blue',
     BORDER_WIDTH :2,
     FONT_FORE: "blue"
 };

// CCFlow ENUMS
var PageEditType = {
    Add: 0,
    Modify: 1,
    None: 2
},
WorkState = {
    Run: 'Run',
    Designer: 'Designer'
},
ElementType = {
    FLOW :'flow',
    NODE : 'node',
    DIRECTION : 'direction',
    LABNOTE : 'labNote'
},
CONTAINERID = 'holder';// GET  UIOBJECT OF CURRENT CCFLOW

function CCFlow(n, ct, id, wID) {// 构造函数默认值
    this.Name = n;
    this.ChartType = ct;
    this.FK_Flow = id;
    this.WorkID = wID;


    this.nodes = new Array();
    this.directions = new Array();
    this.labNotes = new Array();
    this.params = null;
    this.wF_SelectorAcceptor = new Array();
    this.wF_GenerWorkList = new Array();

    this.workState = ''; // run、designer

    this.focusElement = null; //记录当前选中的控件，{type:'',ele:obj}
    this.oType = ElementType.FLOW;
};
CCFlow.load = function (data, w, h) {
    // flow property 
    var f = null;
    if (data.WF_FLOW.length == 1) {
        var flow = data.WF_FLOW[0];
        f = new CCFlow(flow.NAME, flow.CHARTTYPE, flow.NO, flow.WORKID);
        f.params = flow.PARAS;
        diagram.flow = f;
        container = CCFlow.getContainer(w, h);
        // 如果是设计状态，需要添加绑定事件
        f.WorkState = WorkState.Designer;
        if (f.WorkState == WorkState.Designer) {
            CCFlow.bindContainerAction();
        }
    }
    else {
        $.messager.alert('error', '流程数据只允许有一条数据!', 'info');
        return false;
    }

    // element property
    // json串全部用大写,兼容数据库问题
    f.nodes = CCNode.loadArray(data.WF_NODE);
    f.directions = CCDirection.loadArray(data.WF_DIRECTION);
    f.labNotes = CCLabNote.loadArray(data.WF_LABNOTE);

    //     f.wF_SelectorAcceptor = WF_SelectorAcceptor.loadArray(f.WF_SELECTORACCEPTOR);
    //     f.wF_GenerWorkList = WF_GenerWorkList.loadArray(fl.GENERWORKLIST);

    f._initialized = true;
    return f;
};
CCFlow.getContainer = function (w, h) {

    // 画布容器
    // 背景
    // 网格
    if (w == undefined) {
        w = window.screen.width;
        h = window.screen.height;

        w = 1500 > w ?  1200 : w;
        h = 600 > h ? 600 : h;
    }

    var c = new Raphael(CONTAINERID, w, h);
    return c;
};
CCFlow.bindContainerAction = function () {
    $('#' + CONTAINERID)
    .bind('mousedown', onContainerMouseDown)
    .bind('mousemove', onContainerMouseMove)
    .bind('mouseup', onContainerMouseUp)
    .bind("contextmenu", onContainerContextMenu);
};
CCFlow.prototype = {

    constructor: CCFlow, getSize: function () {
        return this.size
    },
    draw: function () {

        //绘制节点
        for (var b = 0; b < this.nodes.length; b++) {
            this.nodes[b].draw();
            this.nodes[b].bindAction();
        }

        //绘制节点的连线
        for (var b = 0; b < this.directions.length; b++) {
            this.directions[b].draw();
            this.directions[b].bindAction();
        }

        //绘制标签
        for (var b = 0; b < this.labNotes.length; b++) {
            this.labNotes[b].draw();
            this.labNotes[b].bindAction();
        }
    },
    add: function () {

    },
    del: function () {

    },
    save: function () {

        //        if (!diagram.isChanged) {
        //            return true;
        //        }
       loading();
        var params = new Params(),
            nodes = "",
            dirs = "",
            labes = "";
        params.push('fk_flow', this.FK_Flow);

        for (var b = 0; b < this.nodes.length; b++) {
            var n = this.nodes[b],
               x = n.x + NODE_WIDTH / 2,
               y = n.y + NODE_HEIGHT / 2;

            nodes += "~@Name=" + n.name
           + "@X=" + x
           + "@Y=" + y
           + "@NodeID=" + n.id
           + "@HisRunModel=" + n.nodeType
           + "@Icon=" + n.icon
           + "@HisPosType=" + n.nodePosType;
        }
        params.push('nodes', nodes);


        for (var b = 0; b < this.directions.length; b++) {
            var d = this.directions[b];
            if (d.toNode != null && d.toNode != 0 && d.node != null)  //不为虚开始
            {
                // 指向同一点的连线不保存
                if (d.toNode == d.node)
                    continue;
                //中间节与虚结束节点有连线时忽略不保存
                if (d.toNode == 1)
                    continue;
                var canBack = d.isCanBack ? 1 : 0,
                    myPk = d.node + "_" + d.toNode + "_" + d.dirType,
                    dots = '';
                if (d.lineType == DirectionUIType.Polyline) {
                    dots = "#" + d.pTurn1.X + "," + d.pTurn1.Y
                         + "#" + d.pTurn2.X + "," + d.pTurn2.Y;
                }

                dirs += "~@Node=" + d.node
                        + "@ToNode=" + d.toNode
                        + "@DirType=" + d.dirType
                        + "@IsCanBack=" + canBack
                        + "@Dots=" + dots
                        + "@MyPK=" + myPk;
            }
        }
        params.push('dirs', dirs);


        for (var b = 0; b < this.labNotes.length; b++) {
            var l = this.labNotes[b];
            labes += "~@FK_Flow=" + diagram.flow.FK_Flow
            + "@X=" + l.x + "@Y=" + l.y
            + "@MyPK=" + l.mypk + "@Label=" + l.name;
        }
        params.push('labes', labes);

        var ff = params.toJsonDataString();

        ajaxService('flow', 'FlowSave', ff, function (data) {
           loaded();
            var jdata = $.parseJSON(data);
            if (!jdata.success) {
                $.messager.alert('失败', jdata.msg, 'error');
            }
            else {
                diagram.isChanged = false;
            }
        });

        return;
    },
    getNodeById: function (nId) {
        /// <summary>根据指定节点ID获取该结点使用Raphael绘制的对象Set</summary>
        /// <param name="nodeid" Type="Int">流程编号</param>
        for (var i = 0, j = this.nodes.length; i < j; i++) {
            if (this.nodes[i].id == nId) {
                return this.nodes[i];
            }
        }

        return null;
    },
    getNodeByIconId: function (iconId) {
        /// <summary>根据绘制的节点中的ICON对象的id获取该结点使用Raphael绘制的对象</summary>
        /// <param name="raphaelid" Type="Int">流程编号</param>
        for (var i = 0, j = this.nodes.length; i < j; i++) {
            if (this.nodes[i].rIcon.id == iconId) {
                return this.nodes[i];
            }
        }

        return null;
    },
    clearFocus: function () {
        /// <summary>清除当前选中的对象有选中状态</summary>
        if (this.focusElement) {
            this.focusElement.clearFocus();
            this.focusElement = null;
        }
    },
    setFocus: function (ele) {
        var changed = false;
        if (ele != null && ele != this.focusElement) {
            ele.focus();
            changed = true;
        }

        if (changed) {
            if (this.focusElement != null)
                this.focusElement.clearFocus();
            this.focusElement = ele;
        }
    },
    getNewElementName: function (eleType) {
        var name;
        var maxid = 0;
        var idstr;
        var currId;
        switch (eleType) {
            case ElementType.NODE:
                name = '新建节点 ';

                $.each(diagram.flow.nodes, function () {
                    if (this.name.length > 5 && this.name.substr(0, 5) == name) {
                        currId = parseInt(this.name.substr(5));
                        if (!isNaN(currId)) {
                            maxid = Math.max(maxid, currId);
                        }
                    }
                });
                break;
            case ElementType.LABNOTE:
                name = '新建标签 ';
                $.each(diagram.flow.labNotes, function () {
                    if (this.name.length > 5 && this.name.substr(0, 5) == name) {
                        currId = parseInt(this.name.substr(5));
                        if (!isNaN(currId)) {
                            maxid = Math.max(maxid, currId);
                        }
                    }
                });
                break;
            default:
                break;
        }

        return name + (maxid + 1);
    }

};

// CCNODES ENUMS
var FlowNodePosType = {
    Start: 0,
    Mid: 1,
    End: 2
},
    FlowNode_BORDER_COLOR = {
        Start: 'green',
        End: 'red',
        Mid: 'black'
},
    FlowNodeType = {
        /// <summary>
        /// 普通
        /// </summary>
        Ordinary : 0,
        /// <summary>
        /// 合流
        /// </summary>
        HL : 1,
        /// <summary>
        /// 分流
        /// </summary>
        FL : 2,
        /// <summary>
        /// 分合流
        /// </summary>
        FHL:3,
        /// <summary>
        /// 子线程.
        /// </summary>
        SubThread:4,
        /// <summary>
        /// 虚节点
        /// </summary>
        VirtualStart:5,
        VirtualEnd:6,
        UnKnown:100
};
function CCNode(id, name, iX, iY) {
    /// <summary>节点</summary>
    /// <param name="iNodeID" Type="Int">节点ID</param>
    /// <param name="sNodeName" Type="String">节点名称</param>
    /// <param name="iX" Type="Int">节点中心点X坐标</param>
    /// <param name="iY" Type="Int">节点中心点Y坐标</param>
    this.id = id;
    this.name = name;
    this.x = iX;
    this.y = iY;
    this.icon = '';
    this.nodePosType = FlowNodePosType.Start;
    this.hisToNDs = '';
    this.nodeType = FlowNodeType.Ordinary;
    this.rCenter = null;
    this.rBorderColor = '';
    this.rBorder = null;
    this.rIcon = null;
    this.rText = null;
    this.rIsFocus = false;
    this.isDrag = false;
    this.oType = ElementType.NODE;
};
CCNode.load = function (n) {
    var x = n.X - NODE_WIDTH / 2,
         y = n.Y - NODE_HEIGHT / 2;
    var node = new CCNode(n.ID, n.NAME, x, y);
    node.icon = n.ICON; //liuxc,20150323
    return node;
};
CCNode.loadArray = function (a) {
    var c = [];
    for (var b = 0; b < a.length; b++) {
        c.push(CCNode.load(a[b]))
    }
    return c
};
CCNode.prototype = {
    initProperty: function () {
        //BorderColor
        switch (this.nodePosType) {
            case FlowNodePosType.Start:
                this.rBorderColor = FlowNode_BORDER_COLOR.Start;
                break;
            case FlowNodePosType.End:
                this.rBorderColor = FlowNode_BORDER_COLOR.End;
                break;
            default:
                this.rBorderColor = FlowNode_BORDER_COLOR.Mid;
                break;
        }

        // icon
        if (this.icon == null || this.icon.length == 0) {
            this.icon = NODE_ICON_PATH + NODE_ICON_DEFAULT;
        }
        else {
            if (this.icon.indexOf('.') == -1) {
                this.icon = NODE_ICON_PATH + this.icon + '.png';
            }
            else {
                this.icon = NODE_ICON_PATH + this.icon.substr(this.icon.lastIndexOf('/') + 1);
            }

            var isExist = checkUrl(encodeURI(this.icon));   //liuxc,20150324
            if (isExist == false) {
                this.icon = NODE_ICON_PATH + NODE_ICON_DEFAULT;
            }
        }
    },
    draw: function () {

        this.initProperty();
        this.rBorder = container.rect(this.x, this.y, NODE_WIDTH, NODE_HEIGHT, NODE_BORDER_RADIUS);
        this.rBorder.attr({
            "stroke": this.rBorderColor,
            "stroke-width": STYLE_NORMAL.BORDER_WIDTH,
            "fill":"white",
            "fill-opacity": 0.5
        });
        this.rIcon = container.image(this.icon, this.x + 1, this.y + 1, NODE_WIDTH - 2, NODE_HEIGHT - 2);
        this.rIcon.parent = this;
        this.rText = container.text(this.x + NODE_WIDTH / 2, this.y + NODE_HEIGHT + 10, this.name);
        //this.rText.attr({ "stroke": STYLE_NORMAL.FONT_FORE, "font-size": STYLE_NORMAL.FONT_SIZE });
        this.rText.attr({ "stroke": STYLE_NORMAL.FONT_FORE});
        this.rCenter = container.circle(this.x + NODE_WIDTH / 2, this.y + NODE_HEIGHT / 2, NODE_WIDTH_CENTER).attr({ "hue": .45, "fill": "green" });
        this.rCenter.parent = this;
        this.rCenter.hide();
    },
    bindAction: function () {
        this.rIconAction();
        this.rCenterAction();
        this.rTextAction();
    },
    rCenterAction: function () {

        function rCenterDown(e) {
            if (px == 0) {
                px = this.parent.rCenter.attr("cx"),
                py = this.parent.rCenter.attr("cy");

                var recDirBegin = {
                    x: px,
                    y: py,
                    width: NODE_WIDTH_CENTER,
                    height: NODE_WIDTH_CENTER
                };

                recDirEnd = recDirBegin;
                recDirEnd.x = px + 1;
                recDirEnd.y = py + 1;

                if (tempDirection == null) {
                    // 方向添加的时候确定开始节点
                    name = f.getNewElementName(ElementType.DIRECTION);
                    var d = new CCDirection(this.parent.id, "", DirType.Backward);
                    d.FromNode = this.parent;
                    tempDirection = d;
                    tempDirection.move(recDirBegin, recDirEnd);
                }
            }
            state = ACTION.DIRECTION_CREATE;
            return false;
        };
        this.rCenter.mousedown(rCenterDown);
        this.rCenter.mouseup(this.MouseUp);

    },
    rIconAction: function () {
        var pStartBorder, pStartText, pStartCenter;
        function iconMove(dx, dy, x, y, e) {
            if (!this.isDrag) {
                return false;
            }

            if (tempDirection != null) {
                onContainerMouseMove(e);
                return false;
            }
            //动态修改与ICON绑定的其他对象的坐标
            var att = { x: this.ox + dx, y: this.oy + dy }, ps, path1;
            this.attr(att);

            this.parent.rBorder.attr({ "x": pStartBorder.x + dx, "y": pStartBorder.y + dy });
            this.parent.rText.attr({ "x": pStartText.x + dx, "y": pStartText.y + dy });
            this.parent.rCenter.attr({ "cx": pStartCenter.x + dx, "cy": pStartCenter.y + dy });

            //重绘与该节点相连的连接线
            CCDirection.OnFlowNodeMove(this.parent, e);
        };
        function iconDown(x, y, e) {
            if (state != ACTION.NONE)
                return false;

            if (!((isie && e.button == 1) || e.button == 0)) {
                this.isDrag = false; //此处为解决非IE浏览器一些莫名其妙的问题，增加此变量来解决
                return false;
            }

            this.isDrag = true;
            this.attr({ "cursor": "hand" });

            this.ox = this.attr("x");
            this.oy = this.attr("y");
           // this.animate({ "fill-opacity": 0.5 }, 500);

            //记录与ICON绑定的其他对象的原始坐标
            pStartCenter = { x: this.parent.rCenter.attr("cx"), y: this.parent.rCenter.attr("cy") };
            pStartBorder = { x: this.parent.rBorder.attr("x"), y: this.parent.rBorder.attr("y") };
            pStartText = { x: this.parent.rText.attr("x"), y: this.parent.rText.attr("y") };

        };

        this.rCenter.mousemove(iconMove);
        this.rIcon.drag(iconMove, iconDown, this.MouseUp);
        this.rIcon.hover(this.Hover, this.UnHover);
        $(this.rIcon.node).bind("contextmenu", this.ContextMenu);
    },
    rTextAction: function () {
    },
    Hover: function () {
        if (this.parent.rIsFocus) {
            return false;
        }

        if (state == ACTION.DIRECTION_CREATE)
            this.parent.rIcon.attr({ "cursor": "move" });
        else
            this.parent.rIcon.attr({ "cursor": "arrow" });

        this.parent.rBorder.attr({ "stroke": STYLE_HOVER.BORDER_COLOR });
        this.parent.rText.attr({ "stroke": STYLE_HOVER.FONT_FORE });
        this.parent.rCenter.show();
    },
    UnHover: function () {
        if (this.parent.rIsFocus) {
            return false;
        }
        this.parent.setNormal();
    },
    MouseUp: function (e) {
        var r = addDirection(e,this);
        if (!r) {
            return false;
        }

        if (!this.isDrag) {
            return;
        } else {
            //this.animate({ "fill-opacity": 1 }, 500);
            this.parent.x = this.attr("x") - NODE_WIDTH / 2,
            this.parent.y = this.attr("y") - NODE_HEIGHT / 2;
        }
    },
    ContextMenu: function (e) {

        $('#nodeMenu').menu('show', {
            left: e.pageX,
            top: e.pageY
        });

        return false;
    },
    clearFocus: function () {
        this.rIsFocus = false;
        this.setNormal(); 
    },
    focus: function () {

        this.rBorder.attr({ "stroke": STYLE_FOCUSED.BORDER });
        this.rText.attr({ "stroke": STYLE_FOCUSED.FONT });
        this.rIsFocus = true;
    }, 
    setNormal: function () {
        this.rCenter.hide();
        this.rBorder.attr({ "stroke": this.rBorderColor  });
        this.rText.attr({ "stroke": STYLE_NORMAL.FONT_FORE });
    },
    add: function () {
        this.draw();
        this.bindAction();
        diagram.flow.nodes.push(this);

    },
    del: function () {

        //删除关联的连接线
        for (var i = 0; i < diagram.flow.directions.length; i++) {
            var dir = diagram.flow.directions[i];
            if (dir.node == this.id || dir.toNode == this.id)
                dir.del();
        }

        this.rBorder.remove();
        this.rIcon.remove();
        this.rText.remove();
        this.rCenter.remove();
        diagram.flow.nodes.remove(this);
    }
};


function CCLabNote(sPk, text, iX, iY) {
    /// <summary>标签</summary>
    /// <param name="sPk" Type="String">MyPk</param>
    /// <param name="sLabelName" Type="String">标签文本</param>
    /// <param name="iX" Type="Int">标签左上角X坐标</param>
    /// <param name="iY" Type="Int">标签左上角Y坐标</param>
    this.mypk = sPk;
    this.name = text;
    this.x = iX;
    this.y = iY;

    this.rBorder = null;
    this.rText = null;
    this.rIsFocus = false;
    this.oType = ElementType.LABNOTE;
}
CCLabNote.load = function (l) {

    var lbl = new CCLabNote(l.MYPK, l.NAME, l.X, l.Y);

    return lbl;
};
CCLabNote.loadArray = function (a) {
    var c = [];
    for (var b = 0; b < a.length; b++) {
        c.push(CCLabNote.load(a[b]))
    }
    return c;
};
CCLabNote.prototype = {
    draw: function () {

        this.rText = container.text(this.x, this.y, this.name);
        this.rText.attr({ "stroke": LABEL_FONT_COLOR_FORE, "font-size": LABEL_FONT_SIZE, "text-anchor": "start" });
        this.rText.toFront();
        this.rText.parent = this;
        var rec = this.rText.getBBox(true);
        this.rBorder = container.rect(this.x - 2, rec.y, rec.width + 2, rec.height + 2);
    },
    bindAction: function () {

        var isDrag = false, pStartBorder;

        $(this.rText.node).bind("contextmenu", function (e) {

            $('#labelMenu').menu('show', {
                left: e.pageX,
                top: e.pageY
            });

            return false;
        });

        this.rText.hover(
           function () {
               if (this.parent.rIsFocus) {
                   return false;
               }

               this.parent.rBorder.attr("stroke", LABEL_FONT_COLOR_HOVER);
               this.parent.rText.attr({ "stroke": LABEL_FONT_COLOR_HOVER, "cursor": "move" });
           },
           function () {
               if (this.parent.rIsFocus) {
                   return false;
               }

               this.parent.rBorder.attr("stroke", LABEL_FONT_COLOR_FORE);
               this.parent.rText.attr({ "stroke": LABEL_FONT_COLOR_FORE, "cursor": "pointer" });
           }
        );


        function txtDown(x, y, e) {
            if (!((isie && e.button == 1) || e.button == 0)) {
                isDrag = false;
                return;
            }

            isDrag = true;
            pStartBorder = { x: this.parent.rBorder.attr("x"), y: this.parent.rBorder.attr("y") };
            this.ox = this.attr("x");
            this.oy = this.attr("y");
            this.animate({ "fill-opacity": 0.5 }, 500);

            return false;
        }

        function txtMove(dx, dy) {
            if (!isDrag) {
                return;
            }

            this.attr({ x: this.ox + dx, y: this.oy + dy });
            this.parent.rBorder.attr({ x: this.ox + dx, y: pStartBorder.y + dy });
        }

        function txtUp() {
            if (!isDrag) {
                return;
            }
            this.parent.x = this.attr("x"),
            this.parent.y = this.attr("y");
            this.parent.rBorder.animate({ "fill-opacity": 1 }, 500);
            this.parent.rText.animate({ "fill-opacity": 1 }, 500);
        }

        this.rText.drag(txtMove, txtDown, txtUp);
    },
    clearFocus: function () {
        this.rText.attr({ "stroke": LABEL_FONT_COLOR_FORE });
        this.rIsFocus = false;
    },
    focus: function () {

        this.rBorder.attr("stroke", STYLE_FOCUSED.BORDER);
        this.rText.attr({ "stroke": STYLE_FOCUSED.FONT, "cursor": "hand" });
        this.rIsFocus = true;
    },
    add: function () {
        this.draw();
        this.bindAction();
        diagram.flow.labNotes.push(this);
    },
    del: function () {
        this.rBorder.remove();
        this.rText.remove();
        diagram.flow.labNotes.remove(this);
    }
};

var DirType = {
    Forward: 0,    //前进线
    Backward: 1,    //回退线
    Virtual: 2    //虚拟线
},
    DirectionUIType =
    {
        Line: 0,
        Polyline: 1 
    },
    DirectionMove =
    {
        Begin: 0,
        Line: 1,
        End: 2 
    };
function CCDirection(nId, tonId, dirType, IsCanback,dots1) {
    /// <summary>结点连接线</summary>
    /// <param name="iFromNodeID" Type="Int">开始节点ID</param>
    /// <param name="iToNodeID" Type="Int">结束节点ID</param>
    /// <param name="iDirType" Type="Int">节点类型 0-前进 1-返回</param>
    /// <param name="iIsCanBack" Type="Int">是否可以原路返回</param>
    this.node = nId;
    this.toNode = tonId;
    this.dirType = dirType;
    this.isCanBack = IsCanback;

    if (dots1 !=null && dots1.length>0) {
        var dots= new Array();  
        var strs= new Array();  
            strs = dots1.split('@');
        for(var i=0;i<strs.length;i++) {
            str = strs[i];
            if (str != null && str != ""){
                var tmp= new Array();  
                tmp = str.split(',');
                if (tmp.length() == 2) {
                    dots.push(Number(tmp[0]));
                    dots.push(Number(tmp[1]));
                }
            }
        }

        this.pTurn1 = { x: dots[0], y: dots[1] };
        this.pTurn2 = { x: dots[2], y: dots[3] };
        this.lineType = DirectionUIType.Polyline;
       
    }
    else     
        this.lineType = DirectionUIType.Line;

    this.FromNode = diagram.flow.getNodeById(this.node);
    this.ToNode = diagram.flow.getNodeById(this.toNode);

    this.MoveType = DirectionMove.Line;
    this.IsReturnTypeDir = false;
    this.rPath = null;
    this.rectB = null;
    this.rectE = null;
    this.rIsFocus = false;
    this.oType = ElementType.DIRECTION;
}
CCDirection.load = function (d) {
    var dir = new CCDirection(d.NODE, d.TONODE, d.DIRTYPE, d.ISCANBACK, d.DOTS);
    return dir;
};
CCDirection.loadArray = function (a) {
    var c = [];
    for (var b = 0; b < a.length; b++) {
        c.push(CCDirection.load(a[b]))
    }
    return c
};
CCDirection.BidRad = 10.0; //往返箭头之间的半径
CCDirection.OnFlowNodeMove = function (node, e) {
    for (var i = 0, j = diagram.flow.directions.length; i < j; i++) {
        var dir = diagram.flow.directions[i];

        if (node.id != dir.node && node.id != dir.toNode)
            continue;
        dir.draw();
    }
};
CCDirection.prototype = {
    draw: function () {

        if (this.FromNode == null || this.ToNode == null)
            return true; //跳出本次循环

        var rectB = container.getBound(this.FromNode.rBorder),
            rectE = container.getBound(this.ToNode.rBorder);

        this.move(rectB, rectE);
    },
    move: function (rectB, rectE) {// 节点移动时调用

        if (this.lineType != DirectionUIType.Line) {
            //
        }
        else {

            var isDbline = false;
            for (var i = 0; i < diagram.flow.directions.length; i++) {
                var d = diagram.flow.directions[i];

                // 双向线
                if (d.node == this.toNode && d.toNode == this.node) {
                    isDbline = true;
                    break;
                }
            }
        }

        if (rectB != null && rectE != null) {

            var points = container.getLinePoints(rectB, rectE, this.lineType);

            if (isDbline) {
                var pStartBack = new Point(points.x1, points.y1),
                    pEndBack = new Point(points.x2, points.y2);

                var pointsNew = PointCount.IntPoint(pStartBack, pEndBack, CCDirection.BidRad, this.dirType, NODE_WIDTH, NODE_HEIGHT);

                points.x1 = pointsNew[0].X;
                points.y1 = pointsNew[0].Y;
                points.x2 = pointsNew[1].X;
                points.y2 = pointsNew[1].Y;

            }
            var path = container.getArrow(points.x1, points.y1, points.x2, points.y2, 8);
            if (this.rPath == null) {
                this.rPath = container.path(path);
                this.rPath.attr({ "stroke": STYLE_NORMAL.BORDER_COLOR, "stroke-width": STYLE_NORMAL.LINE_WIDTH });
                this.rPath.parent = this;

                this.rectB = container.circle(points.x1, points.y1, 5)
                    .attr({ "hue": .45, "fill": "green" })
                    .hide();
                this.rectE = container.circle(points.x2, points.y2, 5)
                    .attr({ "hue": .45, "fill": "green" })
                    .hide();
            }
            else {
                this.rPath.attr({ path: path });
                this.rectB.attr({ "cx": points.x1, "cy": points.y1 });
                this.rectE.attr({ "cx": points.x2, "cy": points.y2 });
            }
        }
    },
    bindAction: function () {
        $(this.rPath.node).bind("contextmenu", this.ContextMenu);

        this.rPath.hover(
            function () {
                if (this.parent.rIsFocus) {
                    return;
                }

                this.attr({ "stroke": STYLE_LINE_HOVER_COLOR, "cursor": "move" });
            },
            function () {
                if (this.parent.rIsFocus) {
                    return;
                }
                this.attr({ "stroke": STYLE_NORMAL.BORDER_COLOR, "cursor": "pointer" });
            }
        );

        this.rPath.mousedown(function (e) {
            if (!((isie && e.button == 1) || e.button == 0)) {
                return;
            }

        });
    },
    ContextMenu: function (e) {

        $('#dirMenu').menu('show', {
            left: e.pageX,
            top: e.pageY
        });

        return false;
    },
    clearFocus: function () {

        this.rectB.attr({ "fill": STYLE_NORMAL.BORDER_COLOR }).hide();
        this.rectE.attr({ "fill": STYLE_NORMAL.BORDER_COLOR }).hide();

        this.rPath.attr({ "stroke": STYLE_NORMAL.BORDER_COLOR });
        this.rIsFocus = false;
    },
    focus: function () {

        this.rectB.attr({ "fill": STYLE_FOCUSED.BORDER }).show();
        this.rectE.attr({ "fill": STYLE_FOCUSED.BORDER }).show();
        this.rPath.attr({ "stroke": STYLE_FOCUSED.BORDER, "cursor": "move" });
        this.rIsFocus = true;
    },
    add: function () {
        this.draw();
        this.bindAction();
        diagram.flow.directions.push(this);
    } ,
    del:function(){
        
        this.rPath.remove();
        this.rectB.remove();
        this.rectE.remove();
        diagram.flow.directions.remove(this);
    }
};


/* drag to add direction between 2 node*/
var px = 0 ,  py=0,
recDirEnd = {
    x: 0,
    y: 0,
    width: NODE_WIDTH_CENTER,
    heigth: NODE_WIDTH_CENTER
},
tempDirection ;
function onContainerMouseDown(e) {
    var ele = container.getElementByPoint(e.pageX, e.pageY);
    if(ele == null) {
        diagram.flow.clearFocus();
    }
    else  if (ele.parent) {
        diagram.flow.setFocus(ele.parent);
    }

    if (!((isie && e.button == 1) || e.button == 0)) {
        return;
    }

    if (state == ACTION.DIRECTION_CREATE) {
        state == ACTION.DIRECTION_MOVE_POINT;
     }

     return;
}
function onContainerMouseMove(e) {

    if (tempDirection != null){

        var p = getRaphaelPoint(e.pageX, e.pageY);

        var recDirBegin = {
            x: px,
            y: py,
            width: NODE_WIDTH_CENTER,
            height: NODE_WIDTH_CENTER
        };

        if (p.x > 5)
            p.x = p.x - 5;
        
        recDirEnd.x =  p.x;
        recDirEnd.y =  p.y;

        tempDirection.move(recDirBegin, recDirEnd);
    }
}
function onContainerMouseUp(e) {

//    if (state != ACTION.DIRECTION_CREATE && state != ACTION.DIRECTION_MOVE_POINT)
//        return ;

    if(tempDirection == null)
        return;

    var ele = container.getElementByPoint(e.pageX, e.pageY);

    var needAdded = false,
        dToNode = null,
        dNode = tempDirection.node;

    if (ele != null && ele.parent != null && ele.parent.oType == ElementType.NODE) {
        dToNode = ele.parent.id;
        // 起止点相同
        if (dToNode != null && dNode != dToNode) {
            needAdded = true;
        }

        for (var i = 0; i < diagram.flow.directions.length; i++) {
            var d = diagram.flow.directions[i];
            if (d == this) { // 同一个规则箭头
                needAdded = false;
                break; ;
            }

            if (d.node == dNode && d.toNode == dToNode) {//重复
                needAdded = false;
                break;
            }

//            if (d.node == dToNode && d.toNode == dNode) {// 往返箭头
//                continue;
//            }
        }
    } 

    if (needAdded) {
        // 方向添加的时候确定开始结束节点
        var dir = new CCDirection(dNode, dToNode, DirType.Forward, false);
       
        dir.isNew = true;  //标识是新加的连接线
        tempDirection.rPath.remove();
        dir.add();
    } else {
        tempDirection.rPath.remove();
    }

    tempDirection = null;
    px = py = 0;
    state = ACTION.NONE;
}
function onContainerContextMenu(e) {

    lastMousePoint.x = e.pageX;
    lastMousePoint.y = e.pageY;

    $('#flowMenu').menu('show', {
        left: e.pageX,
        top: e.pageY
    });

    return false;
}
// 在节点上松开鼠标时创建方向
function addDirection(e) {
    if (tempDirection != null) {
        onContainerMouseUp(e);
        return false;
    }
    return true;
}

Raphael.fn.getArrow = function (x1, y1, x2, y2, size) {
    /// <summary>获取两点之间连线的路径，带箭头</summary>
    /// <param name="x1" Type="Int">开始点X坐标</param>
    /// <param name="y1" Type="Int">开始点Y坐标</param>
    /// <param name="x2" Type="Int">结束点X坐标</param>
    /// <param name="y2" Type="Int">结束点Y坐标</param>
    /// <param name="size" Type="Int">箭头长度</param>
    var angle = Raphael.angle(x1, y1, x2, y2); //得到两点之间的角度
    var a45 = Raphael.rad(angle - 45); //角度转换成弧度
    var a45m = Raphael.rad(angle + 45);
    var x2a = x2 + Math.cos(a45) * size;
    var y2a = y2 + Math.sin(a45) * size;
    var x2b = x2 + Math.cos(a45m) * size;
    var y2b = y2 + Math.sin(a45m) * size;


    // M:move表示画笔起点移动到此点
    // L:line表示从某点绘制到某点，绘制直线
    return ["M", x1, y1, "L", x2, y2, "L", x2a, y2a, "M", x2, y2, "L", x2b, y2b];
}
Raphael.fn.getLinePoints = function (rectB, rectE) {
    /// <summary>获取两个绘图元素之间的连接端点的坐标</summary>
    /// <param name="rElementFrom" Type="Raphael Element">连接线开始的绘图元素</param>
    /// <param name="rElementTo" Type="Raphael Element">连接线结束的绘图元素</param>

    var p = [
            { x: rectB.x + rectB.width / 2, y: rectB.y - 1 },
            { x: rectB.x + rectB.width / 2, y: rectB.y + rectB.height + 1 },
            { x: rectB.x - 1, y: rectB.y + rectB.height / 2 },
            { x: rectB.x + rectB.width + 1, y: rectB.y + rectB.height / 2 },

            { x: rectE.x + rectE.width / 2, y: rectE.y - 1 },
            { x: rectE.x + rectE.width / 2, y: rectE.y + rectE.height + 1 },
            { x: rectE.x - 1, y: rectE.y + rectE.height / 2 },
            { x: rectE.x + rectE.width + 1, y: rectE.y + rectE.height / 2 }
     ];

    var d = {}, dis = [];

    for (var i = 0; i < 4; i++) {
        for (var j = 4; j < 8; j++) {
            var dx = Math.abs(p[i].x - p[j].x),
                dy = Math.abs(p[i].y - p[j].y);
            if (
                (i == j - 4)
                || (((i != 3 && j != 6) || p[i].x < p[j].x)
                && ((i != 2 && j != 7) || p[i].x > p[j].x)
                && ((i != 0 && j != 5) || p[i].y > p[j].y)
                && ((i != 1 && j != 4) || p[i].y < p[j].y))
            ) {
                dis.push(dx + dy);
                d[dis[dis.length - 1]] = [i, j];
            }
        }
    }

    if (dis.length == 0) {
        var res = [0, 4];
    } else {
        res = d[Math.min.apply(Math, dis)];
    }

    return { x1: p[res[0]].x, y1: p[res[0]].y, x2: p[res[1]].x, y2: p[res[1]].y };
}
Raphael.fn.getBound = function (obj) {
    var rect = obj.getBBox();
    return rect;
};


/* Creates an instance of Point
*
* @constructor
* @this {Point}
* @param {Number} x The x coordinate of point.
* @param {Number} y The y coordinate of point.
* Note: Even if it is named Point this class should be named Dot as Dot is closer
* then Point from math perspective.
**/
function Point(x, y) {
    /*  coordinate of point*/
    this.X = x;
    this.Y = y;

    /**The {@link Style} of the Point*/
    this.style = null;//new Style();

    /**Serialization type*/
    this.oType = 'Point'; //object type used for JSON deserialization
}
Point.load = function (o) {
/* Creates a {Point} out of JSON parsed object
*@param {JSONObject} o - the JSON parsed object
*@return {Point} a newly constructed Point
**/

    var newPoint = new Point(Number(o.X), Number(o.Y));
    return newPoint;
}
Point.loadArray = function (v) {
/* Creates an array of points from an array of {JSONObject}s
*@param {Array} v - the array of JSONObjects
*@return an {Array} of {Point}s
**/

    var newPoints = [];
    for (var i = 0; i < v.length; i++) {
        newPoints.push(Point.load(v[i]));
    }
    return newPoints;
}
Point.cloneArray = function (v) {
/* Clones an array of points
*@param {Array} v - the array of {Point}s
*@return an {Array} of {Point}s
**/

    var newPoints = [];
    for (var i = 0; i < v.length; i++) {
        newPoints.push(v[i].clone());
    }
    return newPoints;
}
Point.prototype = {
    constructor: Point,

    /**Paint current {Point} withing a context
    *If you want to use a different style then the default one change the style
    **/
    paint: function (context) {
       
    },

    clone: function () {
        var newPoint = new Point(this.x, this.y);
        return newPoint;
    },

    /**Tests to see if a point (x, y) is within a range of current Point
    *@param {Numeric} x - the x coordinate of tested point
    *@param {Numeric} y - the x coordinate of tested point
    *@param {Numeric} radius - the radius of the vicinity
    *@author Alex Gheorghiu <alex@scriptoid.com>
    **/
    near: function (x, y, radius) {
        var distance = Math.sqrt(Math.pow(this.X - x, 2) + Math.pow(this.Y - y, 2));

        return (distance <= radius);
    },

    contains: function (x, y) {
        return this.X == x && this.Y == y;
    },
    equals: function (value) {
        return this.X == value.X && this.Y == value.Y;
    },
    toString: function () {
        return 'point(' + this.X + ',' + this.Y + ')';
    },

    getPoints: function () {
        return [this];
    },

    getBounds: function () {
        return Point.getBounds(this.getPoints());
    }
}
Point.getBounds=function(points){
    if (!points.length)
        return null;
    var minX = points[0].X;
    var maxX = minX;
    var minY = points[0].Y;
    var maxY = minY;
    for (var i = 1; i < points.length; i++) {
        minX = Math.min(minX, points[i].X);
        minY = Math.min(minY, points[i].Y);
        maxX = Math.max(maxX, points[i].X);
        maxY = Math.max(maxY, points[i].Y);
    }
    return [minX, minY, maxX, maxY];
};

function PointCount(){
  
}   
PointCount.GetPointAtEllipse=function(start, end, r) {
    /// <summary>
    /// 返回圆外点投射到圆心的圆边上坐标
    /// </summary>
    /// <param name="start">圆的中心点坐标</param>
    /// <param name="end">圆外点坐标</param>
    /// <param name="r">圆半径</param>
    /// <returns></returns>

    var pReturn = new Point();
    if (start.X == end.X) {
        pReturn.X = start.X;
        if (end.Y <= start.Y)
            pReturn.Y = start.Y - r;
        else
            pReturn.Y = start.Y + r;
    }
    else if (start.Y == end.Y)  {
        pReturn.Y = start.Y;
        if (end.X <= start.X)
            pReturn.X = start.X - r;
        else
            pReturn.X = start.X + r;
    }
    else  {
        pReturn.Y = r * (end.Y - start.Y) / Math.sqrt(Math.pow(end.Y - start.Y, 2) + Math.pow(end.X - start.X, 2)) + start.Y;
        pReturn.X = (pReturn.Y - start.Y) * (end.X - start.X) / (end.Y - start.Y) + start.X;
    }
    return pReturn;
};
PointCount.GetPointAtRectangle=function(start, end, width, height){
/// <summary>
/// 返回矩形外点投射到矩形中心的边上的坐标
/// </summary>
/// <param name="start">矩形的中心点坐标</param>
/// <param name="end">矩形外点坐标</param>
/// <param name="width">矩形的宽</param>
/// <param name="height">矩形的高</param>
/// <returns></returns>

    //需要判断宽和高的比率问题
    var pReturn = new Point();
    if (start.X == end.X)
    {
        pReturn.X = start.X;
        if (end.Y <= start.Y)
            pReturn.Y = start.Y - height/2;
        else
            pReturn.Y = start.Y + height / 2;
    }
    else if (start.Y == end.Y)
    {
        pReturn.Y = start.Y;
        if (end.X <= start.X)
            pReturn.X = start.X - width / 2;
        else
            pReturn.X = start.X + width / 2;
    }
    else
    {
        if (height / width >= Math.abs(end.Y - start.Y) / Math.abs(end.X - start.X))
        {
            if (end.X - start.X <= 0)
            {
                pReturn.X = start.X - width / 2;
                pReturn.Y = start.Y + (end.Y - start.Y) * (-width / 2) / (end.X - start.X);
            }
            else
            {
                pReturn.X = start.X + width / 2;
                pReturn.Y = start.Y + (end.Y - start.Y) * (width / 2) / (end.X - start.X);
            }
        }
        else
        {
            if (end.Y - start.Y <= 0)
            {
                pReturn.Y = start.Y - height / 2;
                pReturn.X = (-height / 2) * (end.X - start.X) / (end.Y - start.Y) + start.X;
            }
            else
            {
                pReturn.Y = start.Y + height / 2;
                pReturn.X = (height / 2) * (end.X - start.X) / (end.Y - start.Y) + start.X;
            }
        }
    }
    return pReturn;
}
PointCount.IntPoint = function (start, end, r, dirType, width, height) {
    /// <summary>
    /// 根据两活动的中心点，返回往返的边缘坐标
    /// </summary>
    /// <param name="start">起始活动中心点</param>
    /// <param name="end">结束活动中心点</param>
    /// <param name="r">往返之间的半径</param>
    /// <param name="bid">往返类型</param>
    /// <param name="width">活动的宽</param>
    /// <param name="height">活动的高</param>
    /// <returns></returns>

    //获取新的起点
    var newStart = PointCount.GetBidPoint(start, end, r, dirType, true);
    //获取新的终点
    var newEnd = PointCount.GetBidPoint(start, end, r, dirType, false);

    var intersection = new Array();
    intersection.push(new Point());
    intersection.push(new Point());
    if (newStart.X == newEnd.X) {
        if (newStart.Y >= newEnd.Y) {
            intersection[0].X = newStart.X;
            intersection[0].Y = newStart.Y - height / 2;
            intersection[1].X = newEnd.X;
            intersection[1].Y = newEnd.Y + height / 2;
        }
        else {
            intersection[0].X = newStart.X;
            intersection[0].Y = newStart.Y + height / 2;
            intersection[1].X = newEnd.X;
            intersection[1].Y = newEnd.Y - height / 2;
        }
    }
    else if (newStart.Y == newEnd.Y) {
        if (newStart.X >= newEnd.X) {
            intersection[0].X = newStart.X - width / 2;
            intersection[0].Y = newStart.Y;
            intersection[1].X = newEnd.X + width / 2;
            intersection[1].Y = newEnd.Y;
        }
        else {
            intersection[0].X = newStart.X + width / 2;
            intersection[0].Y = newStart.Y;
            intersection[1].X = newEnd.X - width / 2;
            intersection[1].Y = newEnd.Y;
        }
    }
    else {
        //定义起点的四个角的坐标
        var Srec1 = new Point(start.X - width / 2, start.Y - height / 2); //左上角
        var Srec2 = new Point(start.X + width / 2, start.Y - height / 2); //右上角
        var Srec3 = new Point(start.X + width / 2, start.Y + height / 2); //右下角
        var Srec4 = new Point(start.X - width / 2, start.Y + height / 2); //左下角

        //定义交叉点落在哪条边上
        var Lin1 = 0; //0为上边，1为右边，2为下边，3为左边

        //定义结束点点的四个角的坐标
        var Erec1 = new Point(end.X - width / 2, end.Y - height / 2); //左上角
        var Erec2 = new Point(end.X + width / 2, end.Y - height / 2); //右上角
        var Erec3 = new Point(end.X + width / 2, end.Y + height / 2); //右下角
        var Erec4 = new Point(end.X - width / 2, end.Y + height / 2); //左下角

        //定义交叉点落在哪条边上
        var Lin2 = 0; //0为上边，1为右边，2为下边，3为左边

        // 计算交叉点落在那条边上
        if (newStart.Y < newEnd.Y) {
            if (newStart.X > newEnd.X)
                if (Math.abs((newStart.Y - newEnd.Y) / (newStart.X - newEnd.X)) >= Math.abs((newStart.Y - Srec4.Y) / (newStart.X - Srec4.X)))
                    Lin1 = 2;
                else
                    Lin1 = 3;
            else
                if (Math.abs((newStart.Y - newEnd.Y) / (newStart.X - newEnd.X)) >= Math.abs((newStart.Y - Srec3.Y) / (newStart.X - Srec3.X)))
                    Lin1 = 2;
                else
                    Lin1 = 1;

            if (newStart.X > newEnd.X)
                if (Math.abs((newEnd.Y - newStart.Y) / (newEnd.X - newStart.X)) >= Math.abs((newEnd.Y - Erec2.Y) / (newEnd.X - Erec2.X)))
                    Lin2 = 0;
                else
                    Lin2 = 1;
            else
                if (Math.abs((newEnd.Y - newStart.Y) / (newEnd.X - newStart.X)) >= Math.abs((newEnd.Y - Erec1.Y) / (newEnd.X - Erec1.X)))
                    Lin2 = 0;
                else
                    Lin2 = 3;
        }
        else {
            if (newStart.X > newEnd.X)
                if (Math.abs((newEnd.Y - newStart.Y) / (newEnd.X - newStart.X)) >= Math.abs((newStart.Y - Srec1.Y) / (newStart.X - Srec1.X)))
                    Lin1 = 0;
                else
                    Lin1 = 3;
            else
                if (Math.abs((newEnd.Y - newStart.Y) / (newEnd.X - newStart.X)) >= Math.abs((newStart.Y - Srec2.Y) / (newStart.X - Srec2.X)))
                    Lin1 = 0;
                else
                    Lin1 = 1;

            if (newStart.X > newEnd.X)
                if (Math.abs((newStart.Y - newEnd.Y) / (newStart.X - newEnd.X)) >= Math.abs((newEnd.Y - Erec3.Y) / (newEnd.X - Erec3.X)))
                    Lin2 = 2;
                else
                    Lin2 = 1;
            else
                if (Math.abs((newStart.Y - newEnd.Y) / (newStart.X - newEnd.X)) >= Math.abs((newEnd.Y - Erec4.Y) / (newEnd.X - Erec4.X)))
                    Lin2 = 2;
                else
                    Lin2 = 3;
        }


        // 计算交叉点
        var p1 = newStart;
        var p2 = newEnd;
        var p3 = new Point();
        var p4 = new Point();
        switch (Lin1) {
            case 0:
                {
                    p3 = Srec1;
                    p4 = Srec2;
                    break;
                }
            case 1:
                {
                    p3 = Srec2;
                    p4 = Srec3;
                    break;
                }
            case 2:
                {
                    p3 = Srec3;
                    p4 = Srec4;
                    break;
                }
            case 3:
                {
                    p3 = Srec4;
                    p4 = Srec1;
                    break;
                }
        }
        intersection[0] = PointCount.Interaction(p1, p2, p3, p4);
        if (intersection[0].equals(new Point()))
            intersection[0] = newStart;
        switch (Lin2) {
            case 0:
                {
                    p3 = Erec1;
                    p4 = Erec2;
                    break;
                }
            case 1:
                {
                    p3 = Erec2;
                    p4 = Erec3;
                    break;
                }
            case 2:
                {
                    p3 = Erec3;
                    p4 = Erec4;
                    break;
                }
            case 3:
                {
                    p3 = Erec4;
                    p4 = Erec1;
                    break;
                }
        }
        intersection[1] = PointCount.Interaction(p1, p2, p3, p4);
        if (intersection[1].equals(new Point()))
            intersection[1] = newEnd;

    }
    return intersection;
}
PointCount.Interaction=function(p1, p2, p3, p4){
/// <summary>
/// 返回 P1 P2 与 P3 P4 的交叉点，如果不在 P3 P4 范围内，则返回 new point()
/// </summary>
/// <param name="p1"></param>
/// <param name="p2"></param>
/// <param name="p3"></param>
/// <param name="p4"></param>
/// <returns></returns>

    var intersection = new Point();
    //k为intersection(交点)到p1的距离除以p1到p2的距离,k<0表明intersection在p1之外,k>1表明intersection在p2之外
    var k = ((p1.Y - p4.Y) * (p3.X - p4.X) - (p1.X - p4.X) * (p3.Y - p4.Y)) / ((p2.X - p1.X) * (p3.Y - p4.Y) - (p2.Y - p1.Y) * (p3.X - p4.X));
    if (k >= 0 && k <= 1)  {
        intersection.X = p1.X + (p2.X - p1.X) * k;
        intersection.Y = p1.Y + (p2.Y - p1.Y) * k;
    }
    return intersection;
}
PointCount.GetBidPoint=function(start, end, r, dirType, SorE){
/// <summary>
/// 返回双向路由的新点
/// </summary>
/// <param name="start">起点</param>
/// <param name="end">终点</param>
/// <param name="r">双向箭头之间的半径</param>
/// <param name="bid">走向</param>
/// <param name="SorE">返回的是起始点还是终结点</param>
/// <returns></returns>

    var pReturn = new Point();
    var pBegin = new Point();
    if (SorE)
        pBegin = start;
    else
        pBegin = end;

    if (start.X == end.X && start.Y == end.Y)    {
        pReturn.X = start.X + r;
        pReturn.Y = start.Y;
    }
    else {
        if (dirType == DirType.Forward) {
            if (start.X >= end.X && start.Y > end.Y) {
                pReturn.X = pBegin.X + Math.sqrt(Math.pow(r, 2) * Math.pow(start.Y - end.Y, 2) / (Math.pow(start.Y - end.Y, 2) + Math.pow(start.X - end.X, 2)));
                pReturn.Y = pBegin.Y - Math.sqrt(Math.pow(r, 2) * Math.pow(start.X - end.X, 2) / (Math.pow(start.Y - end.Y, 2) + Math.pow(start.X - end.X, 2)));
            }
            else if (start.X < end.X && start.Y > end.Y)  {
                pReturn.X = pBegin.X + Math.sqrt(Math.pow(r, 2) * Math.pow(start.Y - end.Y, 2) / (Math.pow(start.Y - end.Y, 2) + Math.pow(start.X - end.X, 2)));
                pReturn.Y = pBegin.Y + Math.sqrt(Math.pow(r, 2) * Math.pow(start.X - end.X, 2) / (Math.pow(start.Y - end.Y, 2) + Math.pow(start.X - end.X, 2)));
            }
            else if (start.X < end.X && start.Y <= end.Y)  {
                pReturn.X = pBegin.X - Math.sqrt(Math.pow(r, 2) * Math.pow(start.Y - end.Y, 2) / (Math.pow(start.Y - end.Y, 2) + Math.pow(start.X - end.X, 2)));
                pReturn.Y = pBegin.Y + Math.sqrt(Math.pow(r, 2) * Math.pow(start.X - end.X, 2) / (Math.pow(start.Y - end.Y, 2) + Math.pow(start.X - end.X, 2)));
            }
            else {
                pReturn.X = pBegin.X - Math.sqrt(Math.pow(r, 2) * Math.pow(start.Y - end.Y, 2) / (Math.pow(start.Y - end.Y, 2) + Math.pow(start.X - end.X, 2)));
                pReturn.Y = pBegin.Y - Math.sqrt(Math.pow(r, 2) * Math.pow(start.X - end.X, 2) / (Math.pow(start.Y - end.Y, 2) + Math.pow(start.X - end.X, 2)));
            }
        }
        else  {
            if (start.X <= end.X && start.Y <= end.Y)  {
                pReturn.X = pBegin.X - Math.sqrt(Math.pow(r, 2) * Math.pow(end.Y - start.Y, 2) / (Math.pow(start.Y - end.Y, 2) + Math.pow(end.X - start.X, 2)));
                pReturn.Y = pBegin.Y + Math.sqrt(Math.pow(r, 2) * Math.pow(end.X - start.X, 2) / (Math.pow(start.Y - end.Y, 2) + Math.pow(end.X - start.X, 2)));
            }
            else if (start.X > end.X && start.Y <= end.Y) {
                pReturn.X = pBegin.X - Math.sqrt(Math.pow(r, 2) * Math.pow(end.Y - start.Y, 2) / (Math.pow(start.Y - end.Y, 2) + Math.pow(end.X - start.X, 2)));
                pReturn.Y = pBegin.Y - Math.sqrt(Math.pow(r, 2) * Math.pow(end.X - start.X, 2) / (Math.pow(start.Y - end.Y, 2) + Math.pow(end.X - start.X, 2)));
            }
            else if (start.X > end.X && start.Y > end.Y)  {
                pReturn.X = pBegin.X + Math.sqrt(Math.pow(r, 2) * Math.pow(end.Y - start.Y, 2) / (Math.pow(start.Y - end.Y, 2) + Math.pow(end.X - start.X, 2)));
                pReturn.Y = pBegin.Y - Math.sqrt(Math.pow(r, 2) * Math.pow(end.X - start.X, 2) / (Math.pow(start.Y - end.Y, 2) + Math.pow(end.X - start.X, 2)));
            }
            else {
                pReturn.X = pBegin.X + Math.sqrt(Math.pow(r, 2) * Math.pow(end.Y - start.Y, 2) / (Math.pow(start.Y - end.Y, 2) + Math.pow(end.X - start.X, 2)));
                pReturn.Y = pBegin.Y + Math.sqrt(Math.pow(r, 2) * Math.pow(end.X - start.X, 2) / (Math.pow(start.Y - end.Y, 2) + Math.pow(end.X - start.X, 2)));
            }
        }
    }

    return pReturn;
}
