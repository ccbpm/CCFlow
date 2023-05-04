var vm = new Vue({
    el: '#v-db',
    data: {
        boxes: [],
        sortObj: null,
        loadingDialog: null,
        Veiwit: null,
        AdminVeit: null,
        windowlist: [],
        snum: 0,
    },
    methods: {
        bindMenu: function () {
            var _this = this;
            layui.use('dropdown', function () {
                var dropdown = layui.dropdown
                var menuFunc = function (data) {

                    var nameNo = $(this.elem)[0].dataset.nameno;
                    var WinDocModel = $(this.elem)[0].dataset.windocmodel;
                    console.log(nameNo);
                    switch (data.id) {
                        case 'New':
                            var no = GetQueryString("PageID");
                            var url = "../GPM/Window/Html.htm?PageID=" + no + "&MenuNo=" + no;
                            OpenLayuiDialog(url, '', 900, 80, "auto", true);
                            break;
                        case 'Power':
                            alert("未实现.");
                            break;
                        case 'Col1':
                            var en = new Entity("BP.CCFast.Portal.WindowTemplate", nameNo);
                            en.ColSpan = 1;
                            en.Update();
                            window.location.reload();
                            break;
                        case 'Col2':
                            var en = new Entity("BP.CCFast.Portal.WindowTemplate", nameNo);
                            en.ColSpan = 2;
                            en.Update();
                            window.location.reload();
                            break;
                        case 'Col3':
                            var en = new Entity("BP.CCFast.Portal.WindowTemplate", nameNo);
                            en.ColSpan = 3;
                            en.Update();
                            window.location.reload();
                            break;
                        case 'Col4':
                            var en = new Entity("BP.CCFast.Portal.WindowTemplate", nameNo);
                            en.ColSpan = 4;
                            en.Update();
                            window.location.reload();
                            break;
                        case 'Edit':
                            var url = "../Comm/En.htm?EnName=BP.CCFast.Portal.WindowExt." + WinDocModel + "&No=" + nameNo;
                            OpenLayuiDialog(url, '', 1100, 89, "auto", false);
                            break;
                        case 'Delete':
                            if (window.confirm("确定要删除吗?") == false)
                                return;
                            var en = new Entity("BP.CCFast.Portal.WindowTemplate", nameNo);
                            var data = en.Delete();
                            layer.msg(data);

                            //如果有错误.
                            if (data.indexOf("err@") == 0)
                                return;

                            setTimeout(function () {
                                window.location.reload();
                            }, 900)
                            break;
                    }
                }
                var menuNodeItems = [
                    { title: '<i class=icon-pencil></i> 新建', id: "New", Icon: "icon-plus" },
                    { title: '<i class=icon-note></i> 编辑', id: "Edit", Icon: "icon-plus" },
                    { title: '<i class=icon-user-following></i> 权限', id: "Power", Icon: "icon-plus" },
                    { title: '<i class=icon-close></i> 删除', id: "Delete", Icon: "icon-close" },
                    { title: '<i class=icon-user-following></i> 跨度1列', id: "Col1", Icon: "icon-frame" },
                    { title: '<i class=icon-user-following></i> 跨度2列', id: "Col2", Icon: "icon-frame" },
                    { title: '<i class=icon-user-following></i> 跨度3列', id: "Col3", Icon: "icon-frame" },
                    { title: '<i class=icon-user-following></i> 跨度4列', id: "Col4", Icon: "icon-frame" }
                ]
                var menuOptions = [{
                    elem: '.menu-btn',
                    trigger: 'click',
                    align: 'right',
                    data: menuNodeItems,
                    click: menuFunc
                }]

                dropdown.render(menuOptions[0]);
            })
        },

        itemStyle: function (item) {
            colspan = item.ColSpan || 1
            return {
                width: 'calc(' + colspan / 4 * 100 + '%' + ' - 14px)',
                height: '360px',
                margin: '6px 6px 6px 6px'
            }
        },
        itemadd: function () {
            colspan = 1
            return {
                width: 'calc(' + colspan / 4 * 100 + '%' + ' - 14px)',
                height: '300px',
                margin: '6px 6px 6px 6px'
            }
        },
        addfile: function () {

            var no = GetQueryString("PageID");

            // "Html.htm?PageID=f313bf7f-22cf-4d3a-ad16-1721c62572b4&MenuNo=f313bf7f-22cf-4d3a-ad16-1721c62572b4"
            var url = "../GPM/Window/Html.htm?PageID=" + no + "&MenuNo=" + no;
            //SetHref(url);
            OpenLayuiDialog(url, '', 900, 80, "auto", true);
        },
        editfile: function () {
            var no = GetQueryString("PageID");
            var url = "RptWhite.htm?PageID=" + no + "&MenuNo=" + no + "&viewid=Edit";
            SetHref(url);
            //OpenLayuiDialog(url, '', 900, 80, "auto", true);
        },
        seefile: function () {
            var no = GetQueryString("PageID");

            var url = "RptWhite.htm?PageID=" + no + "&MenuNo=" + no;
            //var url = "RptWhite.htm?PageID=" + no + "&MenuNo=" + no;
            SetHref(url);
            //OpenLayuiDialog(url, '', 900, 80, "auto", true);
        },
        expand: function (item) {
            try {
                var URI = item.ModrLink

                if (!item.MoreLinkModel) {
                    item.MoreLinkModel = 0
                }

                if (parseInt(item.MoreLinkModel) === 0) {
                    window.top.vm.openTab(item.Name, URI)
                    return
                }
                if (parseInt(item.MoreLinkModel) === 1) {
                    WinOpen(URI)
                    return
                }
                if (parseInt(item.MoreLinkModel) === 2) {
                    if (!item.proW) item.proW = -1
                    var w = item.proW < 0 ? 400 : item.proW
                    if (!item.proH) item.proH = -1
                    var h = item.proH < 0 ? 300 : item.proH

                    layer.open({
                        type: 2,
                        title: '颜色与布局',
                        content: [URI, 'no'],
                        area: [w + 'px', h + 'px'],
                        offset: 'center',
                        shadeClose: true
                    })
                    return
                }
                layer.msg("参数错误,未指定打开方式")
            } catch (e) {
                // print error msg
                layer.msg(e)
            }
        },


        // todo  需要排序接口
        updateSort: function (str) {
            // 拿到排序后的id数据
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_Portal");
            handler.AddPara("MyPK", str);
            handler.AddUrlData();
            var data = handler.DoMethodReturnString("Home_DoMove");
            layer.msg(data)
        },
        bindArea: function () {
            var _this = this
            this.$nextTick(function () {
                var wrapper = this.$refs['wrapper']
                this.sortObj = new Sortable(wrapper, {
                    animation: 150,
                    ghostClass: 'blue-background-class',
                    dataIdAttr: 'data-no',
                    onStart: function ( /**Event*/ evt) {
                        _this.loadingDialog = layer.msg('正在移动...', {
                            timeout: 900 * 1000
                        })
                    },
                    onEnd: function (evt) {
                        layer.close(_this.loadingDialog)
                        var arr = this.toArray();
                        _this.updateSort(arr.join(','))
                    }
                });
            })

        },
        //表格
        initTable: function () {
            var neededList = this.boxes.filter(function (item) {
                return item.WinDocModel === "Html" || item.WinDocModel === "System"
                    || item.WinDocModel === "SQLList" || item.WinDocModel === "Table"
                    || item.WinDocModel === "HtmlVar" || item.WinDocModel === "iFrame"
            })
            var _this = this
            this.$nextTick(function () {
                for (var i = 0; i < neededList.length; i++) {
                    (function (i) {
                        var item = neededList[i]
                        var el = document.querySelector('div[data-cid="' + item.No + '"]')
                        switch (item.WinDocModel) {
                            case "Html": //html的.

                            case "System": //内置的.
                                el.innerHTML = item.Docs;
                                break;
                            case "Table": //列表的时候的显示.
                                var data = JSON.parse(item.Docs);
                                // console.log(data);
                                var table = '<table class="layui-table">';
                                var startnum = data[0];
                                table += '<thead><tr>';
                                $.each(startnum, function (i) {
                                    table += '<th>' + i + '</th>';

                                });
                                table += '</tr></thead>';
                                table += '<tbody>';
                                for (var j = 0; j < data.length; j++) {
                                    var col = data[j]
                                    table += "<tr>"
                                    $.each(startnum, function (i) {

                                        table += "<td>" + col[i] + "</td>";
                                    });
                                    table += "</tr>"
                                }
                                table += "</tbody></table>"
                                el.innerHTML = table
                                break
                            case "HtmlVar": //列表的时候的显示.
                                var data = JSON.parse(item.Docs);
                                // console.log(item.Docs);
                                var table = '<div class="layui-row HtmlVar">';
                                for (var j = 0; j < data.length; j++) {
                                    var col = data[j]
                                    table += "<div class='layui-col-xs6 layui-col-sm6 layui-col-md4'><span>" + col.Name + "</span>";
                                    if (col.UrlExt == "") {
                                        table += "<strong><font color=" + col.FontColor + ">" + col.Exp0 + "</font></strong></div>";
                                    }
                                    else {
                                        table += "<a href='" + col.UrlExt + "'><strong ><font color=" + col.FontColor + ">" + col.Exp0 + "</font></strong></a></div>";
                                    }
                                }
                                table += "</div>";

                                if (item.MoreUrl != '')
                                    table = "<a href='" + item.MoreUrl + "' target=_blank >" + table + "</a>";

                                el.innerHTML = table;
                                break
                            case "iFrame":
                                $(el).css("height", "90%");
                                el.innerHTML = "<iframe src=" + item.Docs + " frameborder='0'  style='width:100%;height:100%' leftMargin='0' topMargin='0'></iframe>";
                                break;
                        }
                    })(i)

                }
            })

        },
        //图形
        initCharts: function () {
            var neededList = this.boxes.filter(function (item) {
                return item.WinDocModel !== "Html" && item.WinDocModel !== "System" && item.WinDocModel !== "SQLList" && item.WinDocModel !== "Table" && item.WinDocModel !== "HtmlVar" && item.WinDocModel !== "Tab" && item.WinDocModel !== "ChartChina"
            })

            var _this = this
            this.$nextTick(function () {
                for (var i = 0; i < neededList.length; i++) {

                    (function (i) {
                        var item = neededList[i].children;
                        if (item != null && item != undefined) {
                            for (var i = 0; i < item.length; i++) {
                                var iteminfo = item[i]
                                iteminfo.Docs = iteminfo.isData
                                //console.log(tabinfo)
                                var els = document.querySelector('div[data-mypk="' + iteminfo.isNo + '"]');
                                if (els == null)
                                    return;
                                //console.log(tabinfo.WindowsShowType)
                                if (i == 0) {
                                    var width = $('#' + iteminfo.isNo).width();
                                    var height = $('#' + iteminfo.isNo).height();
                                }

                                $('#' + iteminfo.isNo).css("width", width).css("height", height);


                                if (iteminfo.isName == '折线图') {
                                    _this.initLineChart(els, iteminfo);

                                }
                                if (iteminfo.isName == '扇形图') {
                                    _this.initPieChart(els, iteminfo);
                                }
                                if (iteminfo.isName == '环形图') {
                                    _this.initAnnular(els, iteminfo);
                                }
                                if (iteminfo.isName == '柱状图') {
                                    _this.initHistogram(els, iteminfo);
                                }
                            }


                        }


                    })(i)

                }
            })
        },
        initTab: function () {
            var neededList = this.boxes.filter(function (item) {
                return item.WinDocModel === "Tab"
            })
            var _this = this
            this.$nextTick(function () {
                for (var i = 0; i < neededList.length; i++) {
                    (function (i) {
                        var tab = neededList[i].children;
                        for (var i = 0; i < tab.length; i++) {
                            var tabinfo = tab[i]
                            tabinfo.Docs = tabinfo.Exp0
                            //console.log(tabinfo)
                            var els = document.querySelector('div[data-mypk="' + tabinfo.MyPK + '"]');
                            //console.log(tabinfo.WindowsShowType)
                            if (i == 0) {
                                var width = $('#' + tabinfo.MyPK).width();
                                var height = $('#' + tabinfo.MyPK).height();
                            }

                            $('#' + tabinfo.MyPK).css("width", width).css("height", height);


                            if (tabinfo.WindowsShowType == 0) {
                                _this.initPieChart(els, tabinfo);

                            }
                            if (tabinfo.WindowsShowType == 1) {
                                _this.initLineChart(els, tabinfo);
                            }
                            if (tabinfo.WindowsShowType == 2) {
                                _this.initHistogram(els, tabinfo);
                            }
                            //表格
                            if (tabinfo.WindowsShowType == 4) {
                                var dataExp = JSON.parse(tabinfo.Docs);
                                // console.log(dataExp)
                                var tabhtml = '<table class="layui-table">';
                                var startnum = dataExp[0];
                                tabhtml += '<thead><tr>';
                                $.each(startnum, function (i) {
                                    tabhtml += '<th>' + i + '</th>';
                                });
                                tabhtml += '</tr></thead>';
                                tabhtml += '<tbody>';
                                for (var j = 0; j < dataExp.length; j++) {
                                    var col = dataExp[j]
                                    tabhtml += "<tr>"
                                    $.each(startnum, function (i) {

                                        tabhtml += "<td>" + col[i] + "</td>"
                                    });
                                    tabhtml += "</tr>"
                                }
                                tabhtml += "</tbody></table>"
                                els.innerHTML = tabhtml
                            }

                        }

                    })(i)

                }
            })
        },
        //地图
        initMap: function () {
            var neededList = this.boxes.filter(function (item) {
                return item.WinDocModel === "ChartChina"
            })
            var _this = this
            this.$nextTick(function () {
                for (var i = 0; i < neededList.length; i++) {
                    var item = neededList[i];
                    (function (i) {
                        $.get('china.json', function (chinaJson) {
                            //注册地图
                            echarts.registerMap('china', chinaJson);
                            var els = document.querySelector('div[data-cid="' + item.No + '"]');

                            var myChart = echarts.init(els);
                            var series = [
                                {
                                    name: '数据',
                                    type: 'map',
                                    map: 'china',
                                    projection: {
                                        project: function (point) {
                                            return projection(point);
                                        },
                                        unproject: function (point) {
                                            return projection.invert(point);
                                        }
                                    },
                                    emphasis: {
                                        label: {
                                            show: true
                                        }
                                    },
                                    data: [
                                        { name: '吉林省', value: 4822023, tt: '12' },
                                        { name: '江苏省', value: 731449, tt: '10' },

                                    ]
                                }
                            ];

                            var option = {
                                backgroundColor: "rgba(0,0,0,.5)", //背景颜色
                                title: {
                                    text: '数据模拟',
                                    subtext: '数据纯属虚构',
                                    left: 'center',
                                    textStyle: {
                                        color: '#fff'
                                    }
                                },
                                tooltip: {
                                    trigger: 'item',
                                    showDelay: 0,
                                    transitionDuration: 0.2,
                                    formatter: function (params, ticket, callback) {
                                        if (params.data.name)
                                            return params.data.name + "<br/>已完成：" + params.data.value + "<br/>未完成：" + params.data.tt;
                                        else return params
                                    }
                                },
                                visualMap: {
                                    left: 'right',
                                    min: '',
                                    max: 38000000,
                                    inRange: {
                                        color: [
                                            '#313695',
                                            '#4575b4',
                                            '#74add1',
                                            '#abd9e9',
                                            '#e0f3f8',
                                            '#ffffbf',
                                            '#fee090',
                                            '#fdae61',
                                            '#f46d43',
                                            '#d73027',
                                            '#a50026'
                                        ]
                                    },
                                    text: ['High', 'Low'],
                                    calculable: true
                                },

                                series: series
                            };
                            // 使用刚指定的配置项和数据显示图表。
                            myChart.setOption(option);
                            myChart.resize();
                        })
                    })(i)

                }
            })
        },

        // 初始化折线图
        initLineChart: function (el, item) {

            var lineChart = echarts.init(el)
            var data = JSON.parse(item.Docs);
            var startnum = data[0];
            var xAxis = [];
            if (startnum) {
                var inf = [];
                var num = 0;
                $.each(startnum, function (i) {
                    if (isNaN(startnum[i])) {
                        xAxis = data.map(function (it) {
                            return it[i]
                        })
                    } else {
                        inf[num] = {
                            name: i,
                            type: 'line',
                            smooth: true,
                            data: data.map(function (it) {
                                return it[i]
                            })

                        }
                        num++
                    }

                });

                var option = {
                    legend: {},
                    xAxis: {
                        type: 'category',
                        data: xAxis
                    },
                    yAxis: {
                        type: 'value'
                    },
                    tooltip: {
                        trigger: 'axis'
                    },
                    series: inf
                };
                lineChart.setOption(option)
                lineChart.resize();
            }

        },
        // 初始化饼图
        initPieChart: function (el, item) {
            var pieChart = echarts.init(el);
            //console.log(pieChart)
            //var name = item.Name
            var data = JSON.parse(item.Docs)

            var jsonKey = [];
            for (var jsonVal in data[0]) {
                jsonKey.push(jsonVal);
            }
            var oldkey = {
                [jsonKey[0]]: "name",
                [jsonKey[1]]: "value",
            };

            for (var i = 0; i < data.length; i++) {
                var obj = data[i];
                for (var key in obj) {
                    var newKey = oldkey[key];
                    if (newKey) {
                        obj[newKey] = obj[key];
                        delete obj[key];
                    }
                }
            }
            //console.log(data);
            var option = {
                tooltip: {
                    trigger: 'item'
                },
                series: [{
                    //name: name,
                    type: 'pie',
                    radius: '50%',
                    data: data,
                    emphasis: {
                        itemStyle: {
                            shadowBlur: 10,
                            shadowOffsetX: 0,
                            shadowColor: 'rgba(0, 0, 0, 0.5)'
                        }
                    }
                }]
            };
            pieChart.setOption(option);
            pieChart.resize();
        },
        // 初始化柱状图
        initHistogram: function (el, item) {
            var hChart = echarts.init(el)
            var data = JSON.parse(item.Docs)
            var startnum = data[0];
            var xAxis = [];
            if (startnum) {
                var inf = [];
                var num = 0;
                $.each(startnum, function (i) {
                    if (isNaN(startnum[i])) {
                        xAxis = data.map(function (it) {
                            return it[i]
                        })
                    } else {
                        inf[num] = {
                            name: i,
                            type: 'bar',
                            data: data.map(function (it) {
                                return it[i]
                            })

                        }
                        num++
                    }
                });

                option = {
                    tooltip: {},
                    legend: {},
                    xAxis: [
                        {
                            type: 'category',
                            data: xAxis
                        }
                    ],
                    yAxis: [
                        {
                            type: 'value',

                        }
                    ],
                   /* dataZoom: [
                        {
                            show: true,
                            start: 0,
                            end: 100
                        },

                    ],*/
                    series: inf
                };
                hChart.setOption(option);
                hChart.resize();
            }
        },
        //百分比仪表盘
        initGauge: function (el, item) {
            var GChart = echarts.init(el);
            var option;
            //console.log(item);
            var num = item.SQLOfFZ / item.SQLOfFM * 100;

            option = {
                series: [{
                    type: 'gauge',
                    anchor: {
                        show: true,
                        showAbove: true,
                        size: 18,
                        itemStyle: {
                            color: '#FAC858'
                        }
                    },

                    progress: {
                        show: true,
                        overlap: true,
                        roundCap: true
                    },
                    axisLine: {
                        roundCap: true
                    },
                    data: [{
                        value: num.toFixed(2),
                        name: item.LabOfRate,
                        title: {
                            offsetCenter: ['0%', '75%']
                        },
                        detail: {
                            offsetCenter: ['0%', '95%']
                        }
                    },

                    ],
                    title: {
                        fontSize: 12
                    },
                    detail: {
                        width: 40,
                        height: 14,
                        fontSize: 12,
                        color: '#fff',
                        backgroundColor: 'auto',
                        borderRadius: 3,
                        formatter: '{value}%'
                    }
                }]
            };


            GChart.setOption(option);
            GChart.resize();
        },
        //环形
        initAnnular: function (el, item) {
            var AnnularChart = echarts.init(el);
            var name = item.isName;
            if (item.Docs == "") {
                return;
            }
            var data = JSON.parse(item.Docs);
            var jsonKey = [];
            for (var jsonVal in data[0]) {
                jsonKey.push(jsonVal);
            }
            var oldkey = {
                [jsonKey[0]]: "name",
                [jsonKey[1]]: "value",
            };

            for (var i = 0; i < data.length; i++) {
                var obj = data[i];
                for (var key in obj) {
                    var newKey = oldkey[key];
                    if (newKey) {
                        obj[newKey] = obj[key];
                        delete obj[key];
                    }
                }
            }
            
            var option = {
                tooltip: {
                    trigger: 'item'
                },
                legend: {},
                series: [{
                    name: item.isName,
                    type: 'pie',
                    radius: ['40%', '70%'],
                    avoidLabelOverlap: false,
                    itemStyle: {
                        borderRadius: 10,
                        borderColor: '#fff',
                        borderWidth: 2
                    },
                    label: {
                        show: false,
                        position: 'center'
                    },
                    emphasis: {
                        label: {
                            show: true,
                            fontSize: '14',
                            fontWeight: '500'
                        }
                    },
                    labelLine: {
                        show: false
                    },
                    data: data
                }]
            };
            AnnularChart.setOption(option);

            AnnularChart.resize();
        }

    },
    mounted: function () {

        // fix firefox bug
        document.body.ondrop = function (event) {
            event.preventDefault();
            event.stopPropagation();
        }

        var handler = new HttpHandler("BP.WF.HttpHandler.WF_Portal");
        var pageID = GetQueryString("PageID");
        handler.AddPara("PageID", pageID);
        //handler.AddUrlData();
        var windows = handler.DoMethodReturnJSON("Home_Init");
        console.log(windows);
        if (!windows) {
            var no = GetQueryString("PageID");
            var url = "../GPM/Window/Html.htm?PageID=" + no + "&MenuNo=" + no;
            OpenLayuiDialog(url, '', 900, 80, "auto", true);


        } else {

            for (var j = 0; j < windows.length; j++) {
                var win = windows[j];
                win.children = [];
                if (win.WinDocModel == 'Tab') {
                    win.children = JSON.parse(win.Docs);
                }
                if (win.WinDocModel == 'ChartLine') {
                    if (win.IsLine == 1) { win.children.push({ isShow: win.IsLine, isNo: win.No + 'line01', isName: '折线图', isData: win.Docs }) }
                    if (win.IsPie == 1) { win.children.push({ isShow: win.IsPie, isNo: win.No + 'pie01', isName: '扇形图', isData: win.Docs }) }
                    if (win.IsRate == 1) { win.children.push({ isShow: win.IsRate, isNo: win.No + 'rate01', isName: '扇形图', isData: win.Docs }) }
                    if (win.IsRing == 1) { win.children.push({ isShow: win.IsRing, isNo: win.No + 'ring01', isName: '环形图', isData: win.Docs }) }
                    if (win.IsZZT == 1) { win.children.push({ isShow: win.IsZZT, isNo: win.No + 'zzt01', isName: '柱状图', isData: win.Docs }) }
                    // win.children = [{ isShow: win.IsLine, isNo: win.No + 'line01', isName: '折线图', isData: win.Docs }, { isShow: win.IsPie, isNo: win.No + 'pie01', isName: '扇形图', isData: win.Docs }, { isShow: win.IsRing, isNo: win.No + 'ring01', isName: '环形图', isData: win.Docs }, { isShow: win.IsZZT, isNo: win.No + 'zzt01',isName: '柱状图', isData: win.Docs }];

                }
                if (win.WinDocModel == 'ChartZZT') {
                    win.children.push({ isShow: win.IsZZT, isNo: win.No, isName: '柱状图', isData: win.Docs });
                }
                if (win.WinDocModel == 'ChartPie') {
                    win.children.push({ isShow: win.IsPie, isNo: win.No, isName: '扇形图', isData: win.Docs });
                }
                if (win.WinDocModel == 'ChartRate') {
                    win.Docs = '[{"名称":"' + win.LabOfFZ + '","统计数":"' + win.SQLOfFZ + '"},{"名称":"' + win.LabOfFM + '","统计数":"' + win.SQLOfFM +'"}]';
                    win.children.push({ isShow: win.IsRing, isNo: win.No, isName: '环形图', isData: win.Docs });
                }
            }
        }
        //console.log(windows);
        var viewid = GetQueryString("viewid");
        if (viewid) {
            this.Veiwit = viewid;
        }
        var webUser = new WebUser();

        if (webUser.No == 'admin') {
            this.AdminVeit = 1;
        }
        this.bindMenu();

        //console.log(windows);
        //  handler.AddPara("MyPK", str);
        // var windows = new Entities("BP.CCFast.Portal.WindowTemplates");
        // windows.RetrieveAll();
        // handle bad json response
        //delete windows['Paras']
        //delete windows['ensName']
        //delete windows['length']

        try {

            this.boxes = windows;
            // this.snum = windows[0].DefaultChart + windows[0].No
            this.bindArea()
            this.initCharts()
            this.initTable()
            this.initTab()
            this.initMap()
            document.body.ondrop = function (event) {
                event.preventDefault();
                event.stopPropagation();
            }

        } catch (e) {
            console.error(e)
        }
    }
})

