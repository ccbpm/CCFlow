window.onload = function () {
    var vm = new Vue({
        el: '#v-db',
        data: {
            boxes: [],
            sortObj: null,
            loadingDialog: null
        },
        methods: {
            itemStyle: function (item) {
                colspan = item.ColSpan || 1
                return {
                    width: 'calc(' + colspan / 4 * 100 + '%' + ' - 14px)',
                    height: '300px',
                    margin: '6px 6px 6px 6px'
                }
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

            initTable: function () {
                var neededList = this.boxes.filter(function (item) {
                    return item.WinDocModel === "Html" || item.WinDocModel === "System" || item.WinDocModel === "SQLList" || item.WinDocModel === "Table" || item.WinDocModel === "HtmlVar"
                })
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
                                    //console.log(data);
                                    var table = '<table class="layui-table">';
                                    //table += '<colgroup><col width="120"><col width="120"></colgroup>' + '<tbody>';
                                    //table += '<thead><tr><th>流程</th><th>实例</th></tr></thead>' + '<tbody>';
                                    table += '<tbody>';
                                    // table += '<colgroup><col width="120"><col width="120"></colgroup>' + '<thead><tr><th>流程</th><th>实例</th></tr></thead>' + '<tbody>'

                                    for (var j = 0; j < data.length; j++) {
                                        var col = data[j]
                                        table += "<tr>"
                                        table += "<td>" + col.FlowName + "</td>"
                                        table += "<td>" + col.Num + "</td>"
                                        table += "</tr>"
                                    }
                                    table += "</tbody></table>"
                                    el.innerHTML = table
                                    break
                                case "HtmlVar": //列表的时候的显示.
                                    var data = JSON.parse(item.Docs);
                                    console.log(item.Docs);
                                    
                                    var table = '<table class="layui-table HtmlVar">';
                                    table += '<tbody><tr>';
                                    for (var j = 0; j < data.length; j++) {
                                        var col = data[j]
                                        table += "<td><span>" + col.Name + "</span>"
                                        table += "<strong><font color=" + col.FontColor + ">" + col.Exp0 + "</font></strong></td>"
                                      
                                    }
                                    table += "</tr></tbody></table>"
                                    el.innerHTML = table
                                    break
                            }
                        })(i)

                    }
                })

            },
            initCharts: function () {
                var neededList = this.boxes.filter(function (item) {
                    return item.WinDocModel !== "Html" && item.WinDocModel !== "System" && item.WinDocModel !== "SQLList" && item.WinDocModel !== "Table" && item.WinDocModel !== "HtmlVar"
                })
                var _this = this
                this.$nextTick(function () {
                    for (var i = 0; i < neededList.length; i++) {
                        (function (i) {
                            var item = neededList[i];
                            var el = document.querySelector('div[data-cid="' + item.No + '"]');
                           

                            switch (item.WinDocModel) {
                                case "ChartLine":
                                    _this.initLineChart(el, item);
                                    return
                                case "ChartZZT":
                                    _this.initHistogram(el, item);
                                    return
                                case "ChartPie":
                                    _this.initPieChart(el, item);
                                    return
                                case "ChartRate":
                                    _this.initGauge(el, item);
                                    return
                                case "ChartRing":
                                    _this.initAnnular(el, item);
                                    return
                                default:
                                    break;
                                //alert(item.WinDocModel);
                                //layer.msg("未知图表类型" + item.WinDocModel, {
                                //    offset: 'rt',
                                //    anim: 6
                                //})
                            }
                        })(i)

                    }
                })
            },
            // 初始化折线图
            initLineChart: function (el, item) {

                var lineChart = echarts.init(el)
                var data = JSON.parse(item.Docs);
                var startnum = data[0];
                if (startnum) {
                var inf = [];
                var num = 0;
                $.each(startnum, function (i) {  
                    if (isNaN(startnum[i])) {
                        xAxis = data.map(function (it) {
                            return it[i]
                        })
                    }
                    else {
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
                    series:inf
                   
                    
                };
                    lineChart.setOption(option)
                }
            },
            // 初始化饼图
            initPieChart: function (el, item) {
                var pieChart = echarts.init(el);
                var name = item.Name
                var data = JSON.parse(item.Docs)          
                data = data.map(function (it) {
                    return {
                        value: it.Num,
                        name: it.FlowName
                    }
                })
                var option = {
                    tooltip: {
                        trigger: 'item'
                    },
                    series: [{
                        name: name,
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
            },
            // 初始化柱状图
            initHistogram: function (el, item) {              
                var hChart = echarts.init(el)
                var data = JSON.parse(item.Docs)
                var startnum = data[0];
                if (startnum) {
                    var inf = [];
                    var num = 0;
                    $.each(startnum, function (i) {
                        if (isNaN(startnum[i])) {
                            xAxis = data.map(function (it) {
                                return it[i]
                            })
                        }
                        else {
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
                        dataZoom: [
                            {
                                show: true,
                                start: 94,
                                end: 100
                            },

                        ],
                        series: inf
                    };
                    hChart.setOption(option);
                }
            },
            //百分比仪表盘
            initGauge: function (el, item) {
                var GChart = echarts.init(el);
                var option;
                
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
                        }

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
            },
            //环形
            initAnnular: function (el, item) {

                var AnnularChart = echarts.init(el);
                var name = item.Name
                var data = JSON.parse(item.Docs)
                data = data.map(function (it) {
                    return {
                        value: it.Num,
                        name: it.FlowName
                    }
                })
                //console.log(data);
                var option = {
                    tooltip: {
                        trigger: 'item'
                    },
                    legend: {},
                    series: [{
                        name: name,
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

                
            }
            
        },
        mounted: function () {

            var handler = new HttpHandler("BP.WF.HttpHandler.WF_Portal");
            handler.AddUrlData();
            var windows = handler.DoMethodReturnJSON("Home_Init");
            //console.log(windows);
            //  handler.AddPara("MyPK", str);
            // var windows = new Entities("BP.GPM.Home.WindowTemplates");
            // windows.RetrieveAll();
            // handle bad json response
            //delete windows['Paras']
            //delete windows['ensName']
            //delete windows['length']

            try {

                this.boxes = windows;

                this.bindArea()
                this.initCharts()
                this.initTable()
                document.body.ondrop = function (event) {
                    event.preventDefault();
                    event.stopPropagation();
                }

            } catch (e) {
                console.error(e)
            }

        }
    })
}
