<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TruckSimple.aspx.cs" Inherits="CCFlow.WF.Admin.CCBPMDesigner.truck.TruckSimple" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>流程轨迹</title>
    <link href="../../../Scripts/easyUI15/themes/default/easyui.css" rel="stylesheet"
        type="text/css" />
    <link href="../../../Scripts/easyUI15/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../../../Scripts/easyUI15/jquery.min.js" type="text/javascript"></script>
    <script src="/WF/Scripts/QueryString.js" type="text/javascript"></script>
    <script src="../../../Scripts/easyUI15/jquery.easyui.min.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <style type="text/css">
        body
        {
            font-size: 12px;
        }
        .flowstep
        {
            float: left;
            width: 980px;
            margin: 0 auto 15px auto;
            padding: 0px;
        }
        .flowstep-1
        {
            margin: 15px auto 0 auto;
            padding: 0px;
            width: 980px;
        }
        .flowstep-1 li
        {
            list-style: none;
            text-align: center;
            float: left;
            width: 150px;
        }
        .step-name
        {
            padding: 3px 0px;
            font-weight: bold;
            color: #888888;
            height: 20px;
        }
        .step-name1
        {
            padding: 3px 0px;
            font-weight: bold;
            color: #0375D4;
            height: 20px;
        }
        .step-first1
        {
            height: 34px;
            line-height: 34px;
            font-size: 18px;
            background: url("/WF/Admin/CCBPMDesigner/Img/process.png") no-repeat scroll 50% -34px transparent;
        }
        .step-first2
        {
            height: 34px;
            line-height: 34px;
            font-size: 18px;
            background: url("/WF/Admin/CCBPMDesigner/Img/process.png") no-repeat scroll 50% -204px transparent;
        }
        .step-first3
        {
            height: 34px;
            line-height: 34px;
            font-size: 18px;
            background: url("/WF/Admin/CCBPMDesigner/Img/process.png") no-repeat scroll 50% -272px transparent;
        }
        .step-flow1
        {
            height: 34px;
            line-height: 34px;
            font-size: 18px;
            background: url("/WF/Admin/CCBPMDesigner/Img/process.png") no-repeat scroll 50% 0px transparent;
        }
        .step-flow0
        {
            height: 34px;
            line-height: 34px;
            font-size: 18px;
            background: url("/WF/Admin/CCBPMDesigner/Img/process.png") no-repeat scroll 50% -102px transparent;
        }
        .step-flow2
        {
            height: 34px;
            line-height: 34px;
            font-size: 18px;
            background: url("/WF/Admin/CCBPMDesigner/Img/process.png") no-repeat scroll 50% -170px transparent;
        }
        .step-flow3
        {
            height: 34px;
            line-height: 34px;
            font-size: 18px;
            background: url("/WF/Admin/CCBPMDesigner/Img/process.png") no-repeat scroll 50% -238px transparent;
        }
        .step-last0
        {
            height: 34px;
            line-height: 34px;
            font-size: 18px;
            background: url("/WF/Admin/CCBPMDesigner/Img/process.png") no-repeat scroll 50% -136px transparent;
        }
        .step-last1
        {
            height: 34px;
            line-height: 34px;
            font-size: 18px;
            background: url("/WF/Admin/CCBPMDesigner/Img/process.png") no-repeat scroll 50% -68px transparent;
        }
        .step-none
        {
            height: 34px;
            line-height: 34px;
            font-size: 18px;
            background: url("/WF/Admin/CCBPMDesigner/Img/process.png") no-repeat scroll 50% -306px transparent;
        }
        .step-first0
        {
            height: 34px;
            line-height: 34px;
            font-size: 18px;
            background: url("/WF/Admin/CCBPMDesigner/Img/process.png") no-repeat scroll 50% -340px transparent;
        }
        .step-last2
        {
            height: 34px;
            line-height: 34px;
            font-size: 18px;
            background: url("/WF/Admin/CCBPMDesigner/Img/process.png") no-repeat scroll 50% -374px transparent;
        }
        .step-time
        {
            color: #999999;
            margin-top: 10px;
        }
    </style>
    <div class="flowstep">
        <script language="javascript" type="text/javascript">
            var jdata;

            var fk_flow =  '<%=this.FK_Flow %>';
            var workid = '<%=this.WorkID %>';
            var fid = '<%=this.FID %>';
            var nodes;
            var dirs;
            var tracks;
            var flowDirs = [];
            var startNodeId;
            var possibles;
            var flowinfo;
            var host = '<%= Request.Url.Scheme + "://" + Request.Url.Authority %>';

            $(function () {
                $.ajax({
                    type: "Post",
                    contentType: "application/json;utf-8",
                    url: host + "/WF/Admin/CCBPMDesigner/truck/FlowDesignerSvr.asmx/GetFlowTrackJsonData",
                    dataType: "json",
                    data: "{fk_flow:'" + fk_flow + "',workid:'" + workid + "',fid:'" + fid + "'}",
                    success: function (re) {
                        jdata = $.parseJSON(re.d);
                        if (!jdata.success) {
                            alert(jdata.msg);
                        }
                        else {
                            loadTrackList(jdata.ds);
                        }
                    },
                    error: function (re) {
                        alert(re.responseText);
                    }
                });
            });

            function FlowDirection(oDir, aNodes) {
                ///<summary>流程流转方向对象</summary>
                ///<param name="oDir" type="Object">起始点连线信息</param>
                ///<param name="aNodes" type="Array">所有节点集合</param>
                this.firstDir = oDir;
                this.firstNodeId = oDir.NODE;
                this.lastNodeId = oDir.TONODE;
                this.no = ''
                this.name = '';
                this.nodes = new Array();
                this.allNodes = aNodes;

                if (typeof FlowDirection._initialized == 'undefined') {
                    FlowDirection.prototype.addNode = function (nId) {
                        var node = findFromArray(this.allNodes, 'ID', nId)[0];
                        this.nodes.push({ ID: node.ID, NAME: node.NAME });
                        this.no += this.no.length == 0 ? node.ID : ('_' + node.ID);
                        this.name += this.name.length == 0 ? node.NAME : ('→' + node.NAME);
                        this.lastNodeId = node.ID;
                    }

                    FlowDirection.prototype.clone = function () {
                        var newFD = new FlowDirection(this.firstDir, this.allNodes);

                        for (var i = 2; i < this.nodes.length; i++) {
                            newFD.addNode(this.nodes[i].ID);
                        }

                        return newFD;
                    }

                    FlowDirection._initialized = true;
                }

                this.addNode(oDir.NODE);
                this.addNode(oDir.TONODE);
            }

            function loadTrackList(aDS) {
                ///<summary>处理流转方向信息</summary>
                ///<param name="aDS" type="Array">信息集合</param>
                nodes = aDS.WF_NODE;
                tracks = aDS.TRACK;
                startNodeId = findFromArray(nodes, 'NODEPOSTYPE', 0)[0].ID;

                var html;
                var flowinfo = aDS.FLOWINFO[0];
                var nTracks = new Array();

                for (var i = 0; i < tracks.length; i++) {
                    if ((i < tracks.length - 1 && tracks[i + 1].ACTIONTYPE == 5) || tracks[i].ACTIONTYPE == 5) {
                        continue;
                    }

                    nTracks.push(tracks[i]);
                }

                for (var i = 0; i < nTracks.length; i++) {
                    html = '<li><div>';                    
                    html += '<div class="step-name">' + nTracks[i].NDFROMT + '</div>';

                    if (i == 0) {
                        step = 'step-first2';
                    }
                    else if (i == nTracks.length - 1 && flowinfo.WFSTA == 1) {
                        step = 'step-last2';
                    }
                    else {
                        switch (nTracks[i].ACTIONTYPE) {
                            case 2:
//                            case 5:
                                step = 'step-flow3';
                                break;
                            default:
                                step = 'step-flow2';
                                break;
                        }
                    }

                    html += '<div class="' + step + '"></div>';
                    html += '<div class="step-time">' + nTracks[i].RDT.split(' ')[0] + '<br />' + nTracks[i].EMPFROMT + '<br />（' + nTracks[i].ACTIONTYPETEXT + '）</div>';
                    html += '</div></li>';
                    $('.flowstep-1').append(html);
                }

                if (flowinfo.WFSTA != 1) {
                    html = '<li><div>';
                    html += '<div class="step-name"><img src="/WF/Admin/CCBPMDesigner/Img/arrow.png" align="middle" />' + (nTracks.length > 0 ? nTracks[nTracks.length - 1].NDTOT : findFromArray(nodes, 'ID', startNodeId)[0].NAME) + '</div>';
                    html += '<div class="'+(nTracks.length > 0 ? 'step-last1' : 'step-first0')+'"></div>';
                    html += '<div class="step-time">&nbsp;<br />' + (nTracks.length > 0 ? nTracks[nTracks.length - 1].EMPTOT : flowinfo.STARTERNAME) + '</div>';
                    html += '</div></li>';
                    $('.flowstep-1').append(html);
                }
            }

            function loadTrack(sFDNo) {
                ///<summary>加载流转进度图</summary>
                ///<param name="sFDNo" type="String">流转方向No</param>
                var fd = findFromArray(flowDirs, 'no', sFDNo)[0];
                var tks, tkTos;
                var step, emp;
                var currNode;
                var poss;
                var sdirs;

                //确定当前所处节点
                if (tracks.length == 0) {
                    currNode = startNodeId;
                }
                else {
                    if (tracks[tracks.length - 1].ACTIONTYPE != 8) {
                        currNode = tracks[tracks.length - 1].NDTO;
                    }
                    else {
                        currNode = 0;
                    }
                }

                $('.flowstep-1').empty();

                for (var i = 0; i < fd.nodes.length; i++) {
                    var html = '<li><div>';
                    html += '<div class="' + (currNode != fd.nodes[i].ID ? 'step-name' : 'step-name1') + '">' + (currNode != fd.nodes[i].ID ? '' : '<img src="/WF/Admin/CCBPMDesigner/Img/arrow.png" align="middle">') + fd.nodes[i].NAME + '</div>';
                    step = '';
                    emp = '';
                    tks = findFromArray(tracks, 'NDFROM', fd.nodes[i].ID);
                    tkTos = findFromArray(tracks, 'NDTO', fd.nodes[i].ID);
                    poss = findFromArray(possibles, 'FK_NODE', fd.nodes[i].ID);
                    sdirs = findFromArray(dirs, 'NODE', fd.nodes[i].ID);

                    if (tks.length == 0) {
                        if (i == 0) {
                            step = 'step-first1';   //开始节点未流动

                            if (flowinfo) {
                                emp = '&nbsp;<br />' + flowinfo.STARTERNAME;
                            }
                        }
                        else if (i == fd.nodes.length - 1) {
                            if (tkTos.length == 0) {
                                step = 'step-last0';    //结束节点未到
                                emp = poss.length > 0 ? getPossibleEmp(poss[0].EMPNAME) : '';
                            }
                            else {
                                step = 'step-last1';    //结束节点已到达
                                emp = '&nbsp;<br />' + tkTos[tkTos.length - 1].EMPTOT;
                            }
                        }
                        else {
                            if (tkTos.length == 0) {
                                step = 'step-flow0';  //中间节点未到达
                                emp = poss.length > 0 ? getPossibleEmp(poss[0].EMPNAME) : '';
                            }
                            else {
                                step = 'step-flow1';  //中间节点已到达
                                emp = '&nbsp;<br />' + tkTos[tkTos.length - 1].EMPTOT;
                            }
                        }
                    }
                    else {
                        var tk = tks[tks.length - 1];
                        var tkTo = tkTos[tkTos.length - 1];    //发出节点数据不为空，则到达节点数据必不为空

                        if (i == 0) {
                            if (tkTo) {
                                switch (tkTo.ACTIONTYPE) {
                                    case 2: //退回
                                    case 201:   //原路退回
                                        if (tkTo.RDT > tk.RDT) {
                                            step = 'step-first3';   //开始节点已流动,但退回
                                            emp = '&nbsp;<br />' + tkTo.EMPTOT;
                                        }
                                        else {
                                            step = 'step-first2';   //开始节点已流动
                                            emp = tk.RDT.split(' ')[0] + '<br />' + tk.EMPFROMT;
                                        }
                                        break;
                                    default:
                                        step = 'step-first2';   //开始节点已流动
                                        emp = tk.RDT.split(' ')[0] + '<br />' + tk.EMPFROMT;
                                        break;
                                }
                            }
                            else {
                                step = 'step-first2';   //开始节点已流动
                                emp = tk.RDT.split(' ')[0] + '<br />' + tk.EMPFROMT;
                            }
                        }
                        else if (i == fd.nodes.length - 1) {
                            step = 'step-last1';    //结束节点已到达                            
                            emp = (currNode != fd.nodes[i].ID ? tk.RDT.split(' ')[0] : '&nbsp;') + '<br />' + tk.EMPFROMT;
                        }
                        else {
                            switch (tkTo.ACTIONTYPE) {
                                case 2: //退回
                                case 201:   //原路退回
                                    if (tkTo.RDT > tk.RDT) {
                                        step = 'step-flow3';   //中间节点已流动,但退回
                                        emp = '&nbsp;<br />' + tkTo.EMPTOT;
                                    }
                                    else {
                                        step = 'step-flow2';   //中间节点已流动
                                        emp = (currNode != fd.nodes[i].ID ? tk.RDT.split(' ')[0] : '&nbsp;') + '<br />' + tk.EMPFROMT;
                                    }
                                    break;
                                default:
                                    step = currNode != fd.nodes[i].ID ? 'step-flow2' : 'step-flow1';   //中间节点已流动
                                    emp = (currNode != fd.nodes[i].ID ? tk.RDT.split(' ')[0] : '&nbsp;') + '<br />' + tk.EMPFROMT;
                                    break;
                            }
                        }
                    }

                    html += '<div class="' + step + '"></div>';
                    html += '<div class="step-time">' + emp + '</div>';
                    html += '</div></li>';
                    $('.flowstep-1').append(html);

                    //判断下一个点是否可以显示
                    if (sdirs.length > 1 && tks.length == 0 && poss.length == 0) {
                        html = '<li><div>';
                        html += '<div class="step-name"></div>';
                        html += '<div class="step-none"></div>';
                        html += '<div class="step-time"></div>';
                        html += '</div></li>';
                        $('.flowstep-1').append(html);
                        break;
                    }
                }
            }

            function getPossibleEmp(sEmp) {
                ///<summary>获取预期处理人的样式html</summary>
                ///<param name="sEmp" type="String">预期处理人</param>
                return '&nbsp;<br /><span style="color:gray;">' + sEmp + '</span>';
            }

            function doNextDirs(oDir, aDirs, oCurrFD, aFlowDirs) {
                ///<summary>获取指定流转方向的下一段连线</summary>
                ///<param name="oDir" type="Object">连线</param>
                ///<param name="aDirs" type="Array">连线集合</param>
                ///<param name="oCurrFD" type="Object">当前流转方向</param>
                ///<param name="aFlowDirs" type="Array">流转方向集合</param>
                var nextDirs = findFromArray(aDirs, 'NODE', oDir.TONODE);
                var fdClone;

                if (nextDirs.length == 0) {
                    return;
                }

                if (nextDirs.length > 1) {
                    fdClone = oCurrFD.clone();
                }

                for (var i = 0; i < nextDirs.length; i++) {
                    if (i > 0) {
                        var currFD = fdClone.clone();
                        currFD.addNode(nextDirs[i].TONODE);
                        aFlowDirs.push(currFD);
                        doNextDirs(nextDirs[i], dirs, currFD, aFlowDirs);
                    }
                    else {
                        oCurrFD.addNode(nextDirs[i].TONODE);
                        doNextDirs(nextDirs[i], dirs, oCurrFD, aFlowDirs);
                    }
                }
            }

            function getDir(aDirs, sDoneDirs, iNode) {
                ///<summary>从连线集合中获取以指定节点ID为起点的所有连线</summary>
                ///<param name="aDirs" type="Array">连线集合</param>
                ///<param name="sDoneDirs" type="String">已处理连线，用于排除重复</param>
                ///<param name="iNode" type="Int">节点ID</param>
                var dir;

                for (var i = 0; i < aDirs.length; i++) {
                    if (sDoneDirs.indexOf(aDirs[i].MYPK + ',') != -1) {
                        continue;
                    }

                    if (aDirs[i].NODE == iNode) {
                        dir = aDirs[i];
                        break;
                    }
                }

                return dir;
            }

            function findFromArray(aArray, sField1, oValue1, sField2, oValue2) {
                ///<summary>从数组中查找指定属性指定值的元素</summary>
                ///<param name="aArray" type="Array">数组</param>
                ///<param name="sField1" type="String">属性1</param>
                ///<param name="oValue1" type="Object">值1</param>
                ///<param name="sField2" type="String">属性2</param>
                ///<param name="oValue2" type="Object">值2</param>
                if (!aArray || aArray.length == 0) {
                    return [];
                }

                var re = [];

                for (var i = 0; i < aArray.length; i++) {
                    if (aArray[i] && aArray[i][sField1] == oValue1 && (!sField2 || aArray[i][sField2] == oValue2)) {
                        re.push(aArray[i]);
                    }
                }

                return re;
            }
        </script>
        <%--<div id="directions" style="width: 100%; height: 50px; padding: 10px">
            流转方向：
            <select id="cmbdirs" style="width: auto;">
            </select>
        </div>--%>
        <ul class="flowstep-1">
        </ul>
    </div>
    </form>
</body>
</html>
