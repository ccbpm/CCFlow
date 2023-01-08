
(function ($) {
    var defaults = {
        processData: {}, //步骤节点数据
        //processUrl:'',//步骤节点数据
        fnRepeat: function () {
            alert("步骤连接重复");
        },
        fnClick: function () {
            alert("单击");
        },
        fnDbClick: function () {
            alert("双击");
        },
        canvasMenus: {
            "one": function (t) { alert('画面右键') }
        },
        processMenus: {
            "one": function (t) {
                alert('步骤右键');
            }
        },
        canvasLabMenu: {
            "one": function (t) { alert('标签右键') }
        },
        /*右键菜单样式*/
        menuStyle: {
            border: '1px solid #5a6377',
            minWidth: '150px',
        },
        itemStyle: {
            color: '#333',
            fontSize: '12px',
            border: '0',
            /*borderLeft:'5px solid #fff',*/
            padding: '5px 15px 5px 10px'
        },
        itemHoverStyle: {
            border: '0',
            cursor: 'pointer',
            /*borderLeft:'5px solid #49afcd',*/
            color: '#fff',
            /*backgroundColor: '#2980b9'*/
        },
        mtAfterDrop: function (params) {

            //alert('连接成功后调用');
            //alert("连接："+params.sourceId +" -> "+ params.targetId);
        },
        //这是连接线路的绘画样式
        connectorPaintStyle: {
            lineWidth: 3,
            strokeStyle: "#4f7bba",
            joinstyle: "round"
        },
        //鼠标经过样式
        connectorHoverStyle: {
            lineWidth: 3,
            strokeStyle: "#32b291"
        }

    }; /*defaults end*/
    var initEndPoints = function () {
        $(".process-flag").each(function (i, e) {
            var p = $(e).parent();
            jsPlumb.makeSource($(e), {
                parent: p,
                anchor: "Continuous",
                endpoint: ["Dot", { radius: 1 }],
                connector: ["Flowchart", { stub: [5, 5] }],
                connectorStyle: defaults.connectorPaintStyle,
                hoverPaintStyle: defaults.connectorHoverStyle,
                dragOptions: {},
                maxConnections: -1
            });
        });
    }

    /*设置隐藏域保存关系信息*/
    var aConnections = [];
    var setConnections = function (conn, remove) {
        if (!remove) aConnections.push(conn);
        else {
            var idx = -1;
            for (var i = 0; i < aConnections.length; i++) {
                if (aConnections[i] == conn) {
                    idx = i; break;
                }
            }
            if (idx != -1) aConnections.splice(idx, 1);
        }
        if (aConnections.length > 0) {
            var s = "";
            for (var j = 0; j < aConnections.length; j++) {
                var from = $('#' + aConnections[j].sourceId).attr('process_id');
                var target = $('#' + aConnections[j].targetId).attr('process_id');
                s = s + "<input type='hidden' value=\"" + from + "," + target + "\">";
            }
            $('#leipi_process_info').html(s);
        } else {
            $('#leipi_process_info').html('');
        }
        jsPlumb.repaintEverything(); //重画
    };

    /*Flowdesign 命名纯粹为了美观，而不是 formDesign */
    $.fn.Flowdesign = function (options) {

        var _canvas = $(this);
        //右键步骤的步骤号
        _canvas.append('<input type="hidden" id="leipi_active_id" value="0"/><input type="hidden" id="leipi_copy_id" value="0"/>');
        _canvas.append('<div id="leipi_process_info"></div>');


        /*配置*/
        $.each(options, function (i, val) {
            if (typeof val == 'object' && defaults[i])
                $.extend(defaults[i], val);
            else
                defaults[i] = val;
        });
        /*画布右键绑定*/
        var contextmenu = {
            bindings: defaults.canvasMenus,
            menuStyle: defaults.menuStyle,
            itemStyle: defaults.itemStyle,
            itemHoverStyle: defaults.itemHoverStyle
        }
        $(this).contextMenu('canvasMenu', contextmenu);

        jsPlumb.importDefaults({
            DragOptions: { cursor: 'pointer' },
            EndpointStyle: { fillStyle: '#225588' },
            Endpoint: ["Dot", { radius: 1 }],
            ConnectionOverlays: [
                ["Arrow", { location: 1 }],
                ["Label", {
                    location: 0.1,
                    id: "label",
                    cssClass: "aLabel"
                }]
            ],
            Anchor: 'Continuous',
            ConnectorZIndex: 5,
            HoverPaintStyle: defaults.connectorHoverStyle
        });
        if ($.browser.msie && $.browser.version < '9.0') { //ie9以下，用VML画图
            jsPlumb.setRenderMode(jsPlumb.VML);
        } else { //其他浏览器用SVG
            jsPlumb.setRenderMode(jsPlumb.SVG);
        }


        //初始化原步骤
        var lastProcessId = 0;
        var processData = defaults.processData;
        if (processData.list) {
            $.each(processData.list, function (i, row) {
                var nodeDiv = document.createElement('div');
                var nodeId = "window" + row.id, badge = 'badge-inverse', icon = 'icon-star';


                if (lastProcessId == 0)//第一步
                {
                    badge = 'badge-info';
                    icon = 'icon-play';
                }
                if (row.icon) {
                    icon = row.icon;
                }
                $(nodeDiv).attr("id", nodeId)
                    .attr("style", row.style)
                    .attr("process_to", row.process_to)
                    .attr("process_id", row.id)
                    .addClass("process-step btn btn-small")//给节点名称添加一个span元素
                    .html('<span class="process-flag badge ' + badge + '"  alt=' + nodeId + ' title="点击右键,执行相关操作" ><i class="' + icon + ' icon-white"></i></span>&nbsp;<span id="span_' + row.id + '"   title="点击右键,执行相关操作" >' + row.process_name + '</span>')
                    .mousedown(function (e) {
                        if (e.which == 3) { //右键绑定
                            _canvas.find('#leipi_active_id').val(row.id);
                            contextmenu.bindings = defaults.processMenus;

                            var nodeID = document.getElementById("leipi_active_id");
                            if (nodeID.value.indexOf("S_") != -1) {
                               // $(this).contextMenu('processMenu', contextmenu);
                                return;
                            }
                               
                            var node = new Entity("BP.WF.Node", nodeID.value);
                            //如果是第一个节点，把接收人规则文字换成可启动流程的人员
                            if (nodeID.value.substr(nodeID.value.length - 2) == "01")
                                $('#pmNodeAccepterRole span').text("设置发起人");
                            else
                                $('#pmNodeAccepterRole span').text("设置接受人");


                            if (node.RunModel == 0) {
                                $('#pmfun span').text("线型:" + nodeID.value);
                            }
                            if (node.RunModel == 1) {
                                $('#pmfun span').text("合流:" + nodeID.value);
                            }
                            if (node.RunModel == 2) {
                                $('#pmfun span').text("分流:" + nodeID.value);
                            }
                            if (node.RunModel == 3) {
                                $('#pmfun span').text("分合流:" + nodeID.value);
                            }
                            if (node.RunModel == 4) {
                                $('#pmfun span').text("子线程:" + nodeID.value);
                            }

                            if (node.FWCSta == 0) {
                                $('#pmWorkCheck span').text("审核组件-禁用");
                            }
                            if (node.FWCSta == 1) {
                                $('#pmWorkCheck span').text("审核组件-启用");
                            }
                            if (node.FWCSta == 2) {
                                $('#pmWorkCheck span').text("审核组件-只读");
                            }

                            $(this).contextMenu('processMenu', contextmenu);
                            ////$(this).contextMenu('processMenu3', contextmenu);
                            //$(this).mouseenter(function () {
                            //	$(this).contextMenu('processMenu3', contextmenu);
                            //});
                        }
                    });
                _canvas.append(nodeDiv);
                //索引变量
                lastProcessId = row.id;
            }); //each
        }

        //显示标签
        var labNoteData = defaults.labNoteData;
        if (labNoteData.list) {
            $.each(labNoteData.list, function (i, lab) {
                var labDiv = document.createElement('div');
                var labId = "lab" + lab.id, badge = 'badge-inverse';
                $(labDiv).attr("id", labId)
                    .attr("style", lab.style)
                    .attr("process_id", lab.id)
                    .addClass("process-lab")
                    .html('<span class="process-flag badge ' + badge + '"></span>&nbsp;<span id="lab_span_' + lab.id + '">' + lab.process_name + '</span>')
                    .mousedown(function (e) {
                        if (e.which == 3) { //右键绑定
                            _canvas.find('#leipi_active_id').val(lab.id);
                            contextmenu.bindings = defaults.canvasLabMenu;
                            $(this).contextMenu('canvasLabMenu', contextmenu);
                        }
                    })
                    .dblclick(function (e) {
                        console.log('e', e.currentTarget.id);
                        var activeId = e.currentTarget.id.replace('lab', ''); //右键当前的ID
                        var windowtext = $("#lab" + activeId).text();
                        windowtext = windowtext.replace(/(^\s*)|(\s*$)/g, ""); //去掉左右空格.
                        $("#alertModal3 div:eq(2) button").attr("class", "btn btn-primary savetext" + activeId);
                        $("#alertModal3 div:eq(2) button").attr("onclick", "saveLabName(\"" + activeId + "\")");
                        var xiuNodename = '<input style="width:90%" id="TB_LAB_' + activeId + '" type="text" value="' + windowtext + '">'
                        $("#lab" + activeId + " span").html();
                        labAlert(xiuNodename);
                    });
                _canvas.append(labDiv);
            });
        }

        var timeout = null;
        //点击或双击事件,这里进行了一个单击事件延迟，因为同时绑定了双击事件
        $(".process-step").live('click', function () {
            //激活
            _canvas.find('#leipi_active_id').val($(this).attr("process_id")),
                clearTimeout(timeout);
            var obj = this;
            timeout = setTimeout(defaults.fnClick, 300);
        }).live('dblclick', function () {
            clearTimeout(timeout);
            defaults.fnDbClick();
        });

        //使节点可拖动
        jsPlumb.draggable(jsPlumb.getSelector(".process-step"), { containment: $("#flowdesign_canvas") });
        initEndPoints();

        //使标签可拖动
        jsPlumb.draggable(jsPlumb.getSelector(".process-lab"), { containment: $("#flowdesign_canvas") });
        initEndPoints();

        //绑定添加连接操作。画线-input text值  拒绝重复连接
        jsPlumb.bind("jsPlumbConnection", function (info) {
            if (info.sourceId == info.targetId) {
                jsPlumb.detach(info);
                return;
            }

            setConnections(info.connection)
        });
        //绑定删除connection事件
        jsPlumb.bind("jsPlumbConnectionDetached", function (info) {
            setConnections(info.connection, true);
        });

        //绑定删除确认操作
        jsPlumb.bind("click", function (c) {

            fAlert("", 0, c);
            $("#lineDel").unbind("click");
            $("#lineSet").unbind("click");
            $("#lineLabSave").unbind("click");
            //删除节点方向连接线
            $("#lineDel").click(function () {

                //获取连接线连接的ID
                var fromNodeID = c.sourceId.replace('window', '');
                var toNodeID = c.targetId.replace('window', '');

                //获取流程编号
                var flowNo = GetQueryString("FK_Flow");
                if (window.confirm("您确定要删除从节点[" + fromNodeID + "]，到节点[" + toNodeID + "]吗？") == false)
                    return;

                var hander = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCBPMDesigner2018");
                hander.AddPara("FK_Node", fromNodeID);
                hander.AddPara("FK_Flow", flowNo);
                hander.AddPara("ToNode", toNodeID);
                var data = hander.DoMethodReturnString("Direction_Delete");
                if (data.indexOf('err@') == 0) {
                    alert(data); //删除失败的情况.
                    return;
                }

                jsPlumb.detach(c);

            });

            $("#lineSet").click(function () {

                var fromNodeID = c.sourceId.replace('window', '');
                var targetId = c.targetId.replace('window', '');

                var flowNo = GetQueryString("FK_Flow");
                var url = "";
                if (GetHrefUrl().indexOf("/WF/Admin/CCBPMDesigner") == -1)
                    url = basePath + "/WF/Admin/";
                else
                    url = "../";
                url += "Cond2020/ConditionLine.htm?FK_Flow=" + flowNo + "&FK_MainNode=" + fromNodeID + "&FK_Node=" + fromNodeID + "&ToNodeID=" + targetId + "&CondType=2&Lang=CH&t=" + new Date().getTime();
                $("#LineModal").hide();
                $(".modal-backdrop").hide();
                var w = window.innerWidth - 240;
                var h = window.innerHeight - 120;

                CondDir(fromNodeID);
                return;
                OpenEasyUiDialog(url, flowNo + fromNodeID + "DIRECTION" + targetId, "设置方向条件" + fromNodeID + "->" + targetId, w, h, "icon-property", true, null, null, null, function () {

                });

            })
            $("#lineLabSave").click(function () {

                //获取连接线连接的ID
                var fromNodeID = c.sourceId.replace('window', '');
                var toNodeID = c.targetId.replace('window', '');

                //获取流程编号
                var flowNo = GetQueryString("FK_Flow");
                var dirLabId = "TB_Direction_LAB_" + fromNodeID + "_" + toNodeID;
                var des = $("#" + dirLabId).val();
                des = des.replace(/(^\s*)|(\s*$)/g, "");

                var hander = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCBPMDesigner2018");
                hander.AddPara("FK_Node", fromNodeID);
                hander.AddPara("FK_Flow", flowNo);
                hander.AddPara("ToNode", toNodeID);
                hander.AddPara("Des", des);
                var data = hander.DoMethodReturnString("Direction_Save");
                if (data.indexOf('err@') == 0) {
                    alert(data); //删除失败的情况.
                    return;
                }

                c.removeOverlay("des_" + fromNodeID + "_" + toNodeID);
                c.addOverlay(['Label', {
                    label: '<label style="font-size:14px;margin-bottom:25px;margin-left:-20px;color:#00a6ac;">' + des + '</label>', width: 12, length: 12, location: 0.5, id: "des_" + fromNodeID + "_" + toNodeID, events: {
                        click: function (labelOverlay, originalEvent) {
                        }
                    }
                }])
            })

            // if(confirm("你确定取消连接吗?是：取消 否：方向指向"))
            // {
            //     jsPlumb.detach(c);
            // }
            // else
            // {
            //     window.open('');
            // }


        });



        //连接成功回调函数
        function mtAfterDrop(params) {
            //console.log(params)
            //创建一个实体. 保存连接线
            var dir = new Entity("BP.WF.Template.Direction");
            dir.FK_Flow = flowNo;
            dir.Node = $('#' + params.sourceId).attr('process_id');
            dir.ToNode = $('#' + params.targetId).attr('process_id');
            dir.MyPK = dir.FK_Flow + "_" + dir.Node + "_" + dir.ToNode;
            dir.DirType = 2;
            dir.Save();
            //defaults.mtAfterDrop({ sourceId: $("#" + params.sourceId).attr('process_id'), targetId: $("#" + params.targetId).attr('process_id') });

        }

        jsPlumb.makeTarget(jsPlumb.getSelector(".process-step"), {
            dropOptions: { hoverClass: "hover", activeClass: "active" },
            anchor: "Continuous",
            maxConnections: -1,
            endpoint: ["Dot", { radius: 1 }],
            paintStyle: { fillStyle: "#ec912a", radius: 1 },
            hoverPaintStyle: this.connectorHoverStyle,
            beforeDrop: function (params) {
                if (params.sourceId == params.targetId) return false; /*不能链接自己*/
                var j = 0;
                $('#leipi_process_info').find('input').each(function (i) {
                    var str = $('#' + params.sourceId).attr('process_id') + ',' + $('#' + params.targetId).attr('process_id');
                    if (str == $(this).val()) {
                        j++;
                        return;
                    }
                })
                if (j > 0) {
                    defaults.fnRepeat();
                    return false;
                } else {
                    mtAfterDrop(params);
                    return true;
                }
            }
        });
        //reset  start
        var _canvas_design = function () {

            //连接关联的步骤
            $('.process-step').each(function (i) {
                var sourceId = $(this).attr('process_id');
                //var nodeId = "window"+id;
                var prcsto = $(this).attr('process_to');
                var toArr = prcsto.split(",");
                var processData = defaults.processData;
                $.each(toArr, function (j, targetId) {
                    if (targetId != '' && targetId != 0) {
                        //检查 source 和 target是否存在
                        var is_source = false, is_target = false;
                        $.each(processData.list, function (i, row) {
                            if (row.id == sourceId) {
                                is_source = true;
                            } else if (row.id == targetId) {
                                is_target = true;
                            }
                            if (is_source && is_target)
                                return true;
                        });

                        if (is_source && is_target) {
                            var desid = sourceId + "_" + targetId;
                            var linedes = processData.process_des.filter(function (el) { return el.id == desid; })[0].des;
                            jsPlumb.connect({
                                source: "window" + sourceId,
                                target: "window" + targetId,
                                overlays: [
                                    ['Label', {
                                        label: '<label style="font-size:14px;margin-bottom:25px;margin-left:-20px;color:#00a6ac;">' + (linedes == null ? '' : linedes) + '</label>', width: 12, length: 12, location: 0.5, id: "des_" + sourceId + "_" + targetId, events: {
                                            click: function (labelOverlay, originalEvent) {
                                            }
                                        }
                                    }]]
                                /* ,labelStyle : { cssClass:"component label" }
                                ,label : id +" - "+ n*/
                            });
                            return;
                        }
                    }
                })
            });
        } //_canvas_design end reset
        _canvas_design();

        //-----外部调用----------------------

        var Flowdesign = {

            addProcess: function (row) {

                if (row.id <= 0) {
                    return false;
                }
                var nodeDiv = document.createElement('div');
                var nodeId = "window" + row.id, badge = 'badge-inverse', icon = 'icon-star';

                if (row.icon) {
                    icon = row.icon;
                }
                $(nodeDiv).attr("id", nodeId)
                    .attr("style", row.style)
                    .attr("process_to", row.process_to)
                    .attr("process_id", row.id)
                    .addClass("process-step btn btn-small")
                    .html('<span class="process-flag badge ' + badge + '"><i class="' + icon + ' icon-white"></i></span>&nbsp;<span id="span_' + row.id + '">' + row.process_name + '</span>')
                    .mousedown(function (e) {
                        if (e.which == 3) { //右键绑定
                            _canvas.find('#leipi_active_id').val(row.id);
                            contextmenu.bindings = defaults.processMenus;
                            var nodeID = document.getElementById("leipi_active_id");
                            var node = new Entity("BP.WF.Node", nodeID.value);


                            $('#pmAttribute span').text("节点属性");


                            if (node.RunModel == 0) {
                                $('#pmfun span').text("普通" + nodeID.value);
                            }

                            if (node.RunModel == 1) {
                                $('#pmfun span').text("合流");
                            }

                            if (node.RunModel == 2) {
                                $('#pmfun span').text("分流");
                            }

                            if (node.RunModel == 3) {
                                $('#pmfun span').text("分合流");
                            }

                            if (node.RunModel == 4) {
                                $('#pmfun span').text("子线程");
                            }

                            if (node.FWCSta == 0) {
                                $('#pmWorkCheck span').text("审核组件-禁用");
                            }
                            if (node.FWCSta == 1) {
                                $('#pmWorkCheck span').text("审核组件-启用");
                            }
                            if (node.FWCSta == 2) {
                                $('#pmWorkCheck span').text("审核组件-只读");
                            }

                            $(this).contextMenu('processMenu', contextmenu);
                        }
                    });

                _canvas.append(nodeDiv);
                //使之可拖动 和 连线
                jsPlumb.draggable(jsPlumb.getSelector(".process-step"));
                initEndPoints();
                //使可以连接线
                jsPlumb.makeTarget(jsPlumb.getSelector(".process-step"), {
                    dropOptions: { hoverClass: "hover", activeClass: "active" },
                    anchor: "Continuous",
                    maxConnections: -1,
                    endpoint: ["Dot", { radius: 1 }],
                    paintStyle: { fillStyle: "#ec912a", radius: 1 },
                    hoverPaintStyle: this.connectorHoverStyle,
                    beforeDrop: function (params) {
                        var j = 0;
                        $('#leipi_process_info').find('input').each(function (i) {
                            var str = $('#' + params.sourceId).attr('process_id') + ',' + $('#' + params.targetId).attr('process_id');
                            if (str == $(this).val()) {
                                j++;
                                return;
                            }
                        })
                        if (j > 0) {
                            defaults.fnRepeat();
                            return false;
                        } else {
                            return true;
                        }
                    }
                });
                return true;

            },
            addLabProcess: function (lab) {

                if (lab.id <= 0) {
                    return false;
                }
                var labDiv = document.createElement('div');
                var labId = "lab" + lab.id, badge = 'badge-inverse';

                $(labDiv).attr("id", labId)
                    .attr("style", lab.style)
                    .attr("process_id", lab.id)
                    .addClass("process-lab")
                    .html('<span class="process-flag badge ' + badge + '"></span>&nbsp;<span id="lab_span_' + lab.id + '">' + lab.process_name + '</span>')
                    .mousedown(function (e) {
                        if (e.which == 3) { //右键绑定
                            _canvas.find('#leipi_active_id').val(lab.id);
                            contextmenu.bindings = defaults.canvasLabMenu;
                            $(this).contextMenu('canvasLabMenu', contextmenu);
                        }
                    });

                _canvas.append(labDiv);
                //使之可拖动 和 连线
                jsPlumb.draggable(jsPlumb.getSelector(".process-lab"));
                initEndPoints();
                return true;

            },
            delProcess: function (activeId) {
                if (activeId <= 0) return false;

                $("#window" + activeId).remove();
                return true;
            },
            delLabNote: function (activeId) {
                if (activeId <= 0) return false;

                $("#lab" + activeId).remove();
                return true;
            },
            getActiveId: function () {
                return _canvas.find("#leipi_active_id").val();
            },
            copy: function (active_id) {
                if (!active_id)
                    active_id = _canvas.find("#leipi_active_id").val();

                _canvas.find("#leipi_copy_id").val(active_id);
                return true;
            },
            paste: function () {
                return _canvas.find("#leipi_copy_id").val();
            },
            getProcessInfo: function () {
                try {
                    /*连接关系*/
                    var aProcessData = {};
                    $("#leipi_process_info input[type=hidden]").each(function (i) {
                        var processVal = $(this).val().split(",");
                        if (processVal.length == 2) {
                            if (!aProcessData[processVal[0]]) {
                                aProcessData[processVal[0]] = { "top": 0, "left": 0, "process_to": [] };
                            }
                            aProcessData[processVal[0]]["process_to"].push(processVal[1]);
                        }
                    })
                    /*位置*/
                    _canvas.find("div.process-step").each(function (i) { //生成Json字符串，发送到服务器解析
                        if ($(this).attr('id')) {
                            var pId = $(this).attr('process_id');
                            var pLeft = parseInt($(this).css('left'));
                            var pTop = parseInt($(this).css('top'));
                            if (!aProcessData[pId]) {
                                aProcessData[pId] = { "top": 0, "left": 0, "process_to": [] };
                            }
                            aProcessData[pId]["top"] = pTop;
                            aProcessData[pId]["left"] = pLeft;

                        }
                    })
                    return JSON.stringify(aProcessData);
                } catch (e) {
                    return '';
                }

            },
            getLabNoteInfo: function () {
                try {
                    var aLabNoteData = {};
                    /*位置*/
                    _canvas.find("div.process-lab").each(function (i) { //生成Json字符串，发送到服务器解析
                        if ($(this).attr('id')) {
                            var pId = $(this).attr('process_id');
                            var pLeft = parseInt($(this).css('left'));
                            var pTop = parseInt($(this).css('top'));
                            if (!aLabNoteData[pId]) {
                                aLabNoteData[pId] = { "top": 0, "left": 0 };
                            }
                            aLabNoteData[pId]["top"] = pTop;
                            aLabNoteData[pId]["left"] = pLeft;

                        }
                    })
                    return JSON.stringify(aLabNoteData);
                } catch (e) {
                    return '';
                }

            },
            clear: function () {
                try {

                    jsPlumb.detachEveryConnection();
                    jsPlumb.deleteEveryEndpoint();
                    $('#leipi_process_info').html('');
                    jsPlumb.repaintEverything();
                    return true;
                } catch (e) {
                    return false;
                }
            }, refresh: function () {
                try {
                    //jsPlumb.reset();
                    this.clear();
                    _canvas_design();
                    return true;
                } catch (e) {
                    return false;
                }
            }
        };
        return Flowdesign;


    } //$.fn
})(jQuery);