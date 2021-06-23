window.onload = function (){
    var vm = new Vue({
        el: '#v-db',
        data: {
            boxes: [],
            sortObj: null,
            loadingDialog: null
        },
        methods: {
            itemStyle: function(item) {
                colspan = item.ColSpan || 1
                return {
                    width: 'calc(' + colspan / 4 * 100 + '%' + ' - 14px)',
                    height: '300px',
                    margin: '6px 6px 6px 6px'
                }
            },
            expand: function(item) {
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
            updateSort: function(str) {
                // 拿到排序后的id数据
                var handler = new HttpHandler("BP.WF.HttpHandler.WF_Portal");
                handler.AddPara("MyPK", str);
                var data = handler.DoMethodReturnString("Home_DoMove");
                layer.msg(data)
            },
            bindArea: function() {
                var _this = this
                this.$nextTick(function() {
                    var wrapper = this.$refs['wrapper']
                    this.sortObj = new Sortable(wrapper, {
                        animation: 150,
                        ghostClass: 'blue-background-class',
                        dataIdAttr: 'data-no',
                        onStart: function( /**Event*/ evt) {
                            _this.loadingDialog = layer.msg('正在移动...', {
                                timeout: 900 * 1000
                            })
                        },
                        onEnd: function(evt) {
                            layer.close(_this.loadingDialog)
                            var arr = this.toArray();
                            _this.updateSort(arr.join(','))
                        }
                    });
                })

            },

            initTable: function() {
                var neededList = this.boxes.filter(function(item) {
                    return parseInt(item.WinDocType) === 0 || parseInt(item.WinDocType) === 1 || parseInt(item.WinDocType) === 2
                })
                this.$nextTick(function() {
                    for (var i = 0; i < neededList.length; i++) {

                        (function(i) {
                            var item = neededList[i]
                            var el = document.querySelector('div[data-cid="' + item.No + '"]')
                            console.log(item.WinDocType)
                            switch (item.WinDocType) {
                                case 0: //html的.
                                case 1: //内置的.
                                    el.innerHTML = item.Docs
                                    break;
                                case 2: //列表的时候的显示.
                                    var data = JSON.parse(item.Docs);
                                    var table = '<table class="layui-table">';
                                    table += '<colgroup><col width="120"><col width="120"></colgroup>' + '<tbody>';
                                    //table += '<thead><tr><th>流程</th><th>实例</th></tr></thead>' + '<tbody>';
                                    table += '</tbody>';
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
                            }
                        })(i)

                    }
                })

            },
            initCharts: function() {
                var neededList = this.boxes.filter(function(item) {
                    return parseInt(item.WinDocType) !== 0 && parseInt(item.WinDocType) !== 1 && parseInt(item.WinDocType) !== 2
                })

                var _this = this
                this.$nextTick(function() {
                    for (var i = 0; i < neededList.length; i++) {
                        (function(i) {
                            var item = neededList[i]
                            var el = document.querySelector('div[data-cid="' + item.No + '"]')
                            switch (item.WinDocType) {
                                case 3:
                                    _this.initLineChart(el, item)
                                    return
                                case 4:
                                    _this.initHistogram(el, item)
                                    return
                                case 5:
                                    _this.initPieChart(el, item)
                                    return
                                default:
                                    layer.msg("未知图表类型", {
                                        offset: 'rt',
                                        anim: 6
                                    })
                            }
                        })(i)

                    }
                })
            },
            // 初始化折线图
            initLineChart: function(el, item) {
                var lineChart = echarts.init(el)
                var data = JSON.parse(item.Docs)

                var xAxis = data.map(function(it) {
                    return it.FlowName
                })
                var actualData = data.map(function(it) {
                    return it.Num
                })
                var option = {
                    xAxis: {
                        type: 'category',
                        data: xAxis
                    },
                    yAxis: {
                        type: 'value'
                    },
                    series: [{
                        data: actualData,
                        type: 'line',
                        smooth: true
                    }]
                };
                lineChart.setOption(option)
            },
            // 初始化饼图
            initPieChart: function(el, item) {
                var pieChart = echarts.init(el);
                var name = item.Name
                var data = JSON.parse(item.Docs)
                data = data.map(function(it) {
                    return {
                        value: it.Num,
                        name: it.FlowName
                    }
                })
                var option = {
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
            initHistogram: function(el, item) {
                var hChart = echarts.init(el)
                var data = JSON.parse(item.Docs)

                var xAxis = data.map(function(it) {
                    return it.FlowName
                })
                var actualData = data.map(function(it) {
                    return it.Num
                })
                option = {
                    xAxis: {
                        type: 'category',
                        data: xAxis
                    },
                    yAxis: {
                        type: 'value'
                    },
                    series: [{
                        data: actualData,
                        type: 'bar',
                        showBackground: true,
                        backgroundStyle: {
                            color: 'rgba(180, 180, 180, 0.2)'
                        }
                    }]
                };
                hChart.setOption(option)
            }
        },
        mounted: function() {

            var handler = new HttpHandler("BP.WF.HttpHandler.WF_Portal");
            var windows = handler.DoMethodReturnJSON("Home_Init");

            //  handler.AddPara("MyPK", str);

            // var windows = new Entities("BP.GPM.Home.WindowTemplates");
            // windows.RetrieveAll();
            // handle bad json response
            //delete windows['Paras']
            //delete windows['ensName']
            //delete windows['length']

            try{
                this.boxes = windows

                this.bindArea()
                this.initCharts()
                this.initTable()
                document.body.ondrop = function(event) {
                    event.preventDefault();
                    event.stopPropagation();
                }

            }catch (e) {
                console.error(e)
            }

        }
    })
}
