

var Common = {};

Common.MaxLengthError = function () {

    if ((navigator.userAgent.indexOf('MSIE') >= 0) && (navigator.userAgent.indexOf('Opera') < 0) ||
        (navigator.userAgent.indexOf('Trident') >= 0)) {
        $('textarea[maxlength]').bind('blur', function () {
            var isCalendar = $(this).hasClass('TBcalendar');
            //日期控件部做截取控制
            if (isCalendar) { return; }

            var mx = parseInt($(this).attr('maxlength'));
            var str = $(this).val();
            if (str.length > mx) {
                $(this).val(str.substr(0, mx));
                return false;
            }
        });
        $('input[maxlength]').bind('blur', function () {
            var isCalendar = $(this).hasClass('TBcalendar');
            //日期控件部做截取控制
            if (isCalendar) { return; }

            var str = $(this).val();
            var mx = parseInt($(this).attr('maxlength'));
            if (str.length > mx) {
                $(this).val(str.substr(0, mx));
                return false;
            }
        });
    }
}

/*自定义分页插件
默认值如下：
DivId: 'listDiv', 
RenderOverFun: undefined, 表格渲染完时执行的函数
IsShowAll: false,  //也是是否显示全部   这个为TRUE时，分页条不好使  ...(不要分页条的时候用)
LocalData: null, // 和IsUseLocalData一起使用  使用时 不分页
IsUseLocalData: false
DataEmptyRender:数据为空时的处理函数
*/
Common.CustomPagePlug = function (operation) {
    var PageData = function (operation) {
        var _this = this;
        this.InitData = {
            DivId: 'listDiv',
            ////渲染完TABLE执行的函数的参数
            RenderOverParam: {},
            //渲染完TABLE执行的函数
            RenderOverFun: undefined,
            IsShowAll: true,
            LocalData: null,
            IsUseLocalData: true,
            DataEmptyRender: function (obj) {
                var html = "";
                html += "<tr style='text-align: center;'>";
                var ths = $("#" + obj.InitData.DivId + " table thead tr th");
                var colSpan = ths.length;
                for (var i = 0; i < ths.length; i++) {
                    if ($(ths[i]).css('display') == "none") {
                        colSpan -= 1;
                    }
                }
                html += "<td colspan='" + colSpan + "'>";
                //console.log(colSpan)
                //  html += '-';
                html += "</td>";
                html += "</tr>";
                $("#" + obj.InitData.DivId + ' table tbody').html(html);
                $("#" + obj.InitData.DivId + " .loadDate").css("display", "none");
            }
        };


        //数据分页
        //未设置参数使用默认参数 对设置的进行配置
        for (var property in operation) {
            _this.InitData[property] = operation[property];
        }
        _this.SetDataToDivData($('#' + _this.InitData.DivId), _this.InitData);
        _this.InitPlanList(operation.DivId);
        
    };
    PageData.prototype.DateFromMSJsonString = function (value) {
        var d = cceval('new ' + (value.replace(/\//g, '')));
        var result = d.getFullYear() + '-' + (d.getMonth() + 1) + '-' + d.getDate() + ' ' + d.getHours() + ':' + d.getMinutes() + ':' + d.getSeconds();
        return result;
    };
    //Load
    PageData.prototype.Load = function (obj) {
        this.InitData = obj;
    };
    PageData.prototype.InitPlanList = function (DivId) {
        var _this = this;
        var initList = function (data) {
            //初始化
            _this.Load($('#' + DivId).data());
            var obj = JSON.parse(data).DTObjs;
            var html = "";
            var operHtml = "";
            $.each(obj, function (k, obje) {
                 html += "<tr>";
                if (obje) {
                    //把OBJE的值过一遍  replace 掉：  json 转的时候转化了这些值，转化回来
                    for (var ele in obje) {
                        obje[ele] = obje[ele];
                    }
                    //判断是否是多表头
                    var headTrs = $("#" + _this.InitData.DivId + " table thead tr");
                    var isMultihead = false;
                    if (headTrs.length > 1)
                        isMultihead = true;
                    var headers = $("#" + _this.InitData.DivId + " table thead tr th");
                    if (isMultihead == true) {
                        //如果是多表头，需要整合整个th信息
                        headers = [];
                        var firstHeadTh = $(headTrs[0]).find("th");
                        var secondHeadTh = $(headTrs[1]).find("th");
                        for (var i = 0; i < firstHeadTh.length; i++) {
                            var dataInfo = $(firstHeadTh[i]).attr("data-info");
                            if (dataInfo != null && dataInfo != undefined && dataInfo.indexOf("Multi,") != -1) {
                                $.each(secondHeadTh, function (idx, item) {
                                    if (dataInfo.indexOf($(item).attr("data-info") + ",") != -1)
                                        headers.push(item);
                                });
                            } else {
                                headers.push(firstHeadTh[i]);
                            }
                        }
                    }
                    for (var i = 0; i < headers.length; i++) {
                        var style = "";
                        if ($(headers[i]).css('display') == "none")
                            style = "style='display:none'";
                        if ($(headers[i]).data != undefined && $(headers[i]).data().colname != undefined && obje[$(headers[i]).data().colname] != undefined) {
                            if ($(headers[i]).data().coltype != undefined && $(headers[i]).data().coltype == "date") {//类型是日期的TD
                                html += "<td " + style+">" + _this.DateFromMSJsonString(obje[$(headers[i]).data().colname]) + "</td>"
                            } else if ($(headers[i]).data().coltype != undefined && $(headers[i]).data().coltype == "Money") {
                                var defVal = $(headers[i]).data().DefVal;
                                var bit = 2;
                                if (defVal != null && defVal !== "" && defVal.indexOf(".") >= 0)
                                    bit = defVal.substring(defVal.indexOf(".") + 1).length;
                                var val = obje[$(headers[i]).data().colname] + "";
                                var num = 0;
                                if (/\./.test(val))
                                    num = val.substring(val.indexOf(".") + 1).length;
                                else
                                    val += ".";
                                //补全小数位数
                                for (var k = num; k < bit; k++) {
                                    val += "0";
                                }
                               
                                val = formatNumber(val, bit, ',');
                                html += "<td " + style + ">" + val + "</td>"
                            }
                            else if ($(headers[i]).data().opeation != undefined) {//存在操作按钮的TD
                                //添加字符截取功能
                                if ($(headers[i]).data().charlength != undefined) {
                                    var charlength = $(headers[i]).data().charlength;
                                    html += "<td title='" + obje[$(headers[i]).data().colname] + "'" + style +"> <a onclick=" + $(headers[i]).data().opeation + ">" + (obje[$(headers[i]).data().colname].length <= charlength ? obje[$(headers[i]).data().colname] : (obje[$(headers[i]).data().colname].substr(0, charlength)) + "...") + "</a></td>"
                                } else {
                                    html += "<td " + style +"> <a onclick=" + $(headers[i]).data().opeation + ">" + obje[$(headers[i]).data().colname] + "</a></td>"
                                }
                            }
                            //是否把TITLE放上去
                            else if ($(headers[i]).data().title != undefined) {
                                if ($(headers[i]).data().title != undefined) {
                                    var title = $(headers[i]).data().title;
                                    if (title == "true" || title) {
                                        html += "<td " + style +" title='" + obje[$(headers[i]).data().colname] + "'>" + obje[$(headers[i]).data().colname] + "</td>"
                                    }
                                    else {
                                        html += "<td " + style +">" + obje[$(headers[i]).data().colname] + "</td>"
                                    }
                                }
                            }
                            else if ($(headers[i]).data().visiable != undefined) {
                                html += "<td style='display:none;'>" + obje[$(headers[i]).data().colname] + "</td>"
                            }
                            else {//添加字符截取功能 没有类型的TD
                                if ($(headers[i]).data().charlength != undefined) {
                                    var charlength = $(headers[i]).data().charlength;

                                    html += "<td title='" + obje[$(headers[i]).data().colname] + "' " + style +">" + (obje[$(headers[i]).data().colname].length <= charlength ? obje[$(headers[i]).data().colname] : (obje[$(headers[i]).data().colname].substr(0, charlength) + "...")) + "</td>"
                                } else {
                                    html += "<td " + style +">" + obje[$(headers[i]).data().colname] + "</td>";
                                }
                            }
                        }
                        else if ($(headers[i]).data().coltype != undefined && $(headers[i]).data().coltype == "SN") {//序号  序号的类型是SN colname列名称为空
                            //html += "<td><div style='width:26px'>" + (parseInt(k) + 1 + parseInt(_this.InitData.PageSize) * (parseInt(_this.InitData.PageIndex) - 1)) + "</div></td>";
                            html += "<td " + style +">" + (parseInt(k) + 1 ) + "</td>";
                        }
                        else if ($(headers[i]).data().coltype != undefined && $(headers[i]).data().coltype == "Operation") {//序号  序号的类型是SN colname列名称为空){
                            html += ('<td style="white-space: nowrap"><a style="text-decoration:underline;" href="#" onclick="updateReport(this)" class="btn btn-link btn_det">编辑</a>' + '<a href="#" style="text-decoration:underline;" onclick="delReport(this)" class="btn btn-link btn_det">删除</a></td>');
                            html += "</tr>";
                        }
                        //自定义内容
                        else if ($(headers[i]).data().custom != undefined && $(headers[i]).data().customcontent != undefined) {
                            style = "style='white-space: nowrap'";
                            var tmp = '';
                            var customCount = parseInt($(headers[i]).data().customcontent);
                            for (var j = 1; j <= customCount; j++) {
                                //标签; 属性;VALUE
                                var tmpC = $(headers[i]).data()["customcontent" + j];
                                tmpC = tmpC.split('@');
                                tmp += '<' + tmpC[0] + ' ' + tmpC[1] + '>' + tmpC[2] + '</' + tmpC[0] + '>';
                            }
                                html += ('<td ' + style+'>' + tmp + '</td>');
                        }
                        else {
                            html += "<td " + style +">" + "" + "</td>"
                        }
                    }
                }
            });
            if (obj == "") {
                if (_this.InitData.DataEmptyRender != null && typeof (_this.InitData.DataEmptyRender) == "function") {
                    _this.InitData.DataEmptyRender(_this);
                }
            } else {
                $("#" + _this.InitData.DivId + ' table tbody').html(html);
                $("#" + _this.InitData.DivId + " .loadDate").css("display", "none");
            }

            //把TR的 DATA-DATA设置为该行的对象值
            var trData = $("#" + _this.InitData.DivId + " table thead tr");
            if (trData != undefined && trData.data != undefined && trData.data() != undefined && trData.data().data != undefined && trData.data().data == true) {
                $.each(obj, function (i, obje) {
                    $($('#' + _this.InitData.DivId + ' table tbody tr')[i]).data().data = obje;
                });
            }

            //改变一下父窗体中IFRAME的高度
            //当列表被嵌在id=dayReporFrame的IFRAME里时，初始完页面后对父页面中的IFRAME设置高度
            /*if (parent.document.getElementById('dayReporFrame') != null) {
            var bodyHeight = $('body').height() + 30;
            $(parent.document.getElementById('dayReporFrame')).height(bodyHeight);
            }*/
            //绑定完成的回调函数
            if (_this.InitData.RenderOverFun != null && typeof (_this.InitData.RenderOverFun) == 'function') {
                _this.InitData.RenderOverFun(_this.InitData.RenderOverParam);
            }
        }
        initList(_this.InitData.LocalData);
       
        
    };

    //将dataObj的每个属性依次赋值给divObj的data() 上 divObj是HTML元素即可
    PageData.prototype.SetDataToDivData = function (divObj, dataObj) {
        for (var property in dataObj) {
            divObj.data()[property] = dataObj[property];
        }
    }

    return new PageData(operation);
}






