//进度条类型sencond 秒，percent 百分比
var ProgressModel = { sencond: 'sencond', percent: 'percent' };
var ProgressGlo = { TimeOut: null, CurSteep: 0 };
//进度条配置
var progressConfig = { contentId: '', //容器id
    totalCount: 100, //总进度数：默认 100
    timing: 1000, //默认：100，进度速度，毫秒
    proModel: ProgressModel.sencond, //显示类型，百分比percent or 秒sencond
    clickFun: function () { //单击事件
    },
    ComplateFun: function () { //完成事件
    }
};

function ProgressManage(scorp) {
    this.config = scorp;
    this.contentId = this.config.contentId;
    //总进度数
    this.TotalCount = 100;
    if (this.config.totalCount) {
        this.TotalCount = this.config.totalCount;
    }
    //刷新速度
    this.Timing = 100;
    if (this.config.timing) {
        this.Timing = this.config.timing;
    }
    //显示类型，百分比percent or 秒sencond
    this.proModel = 'percent';
    if (this.config.proModel) {
        this.proModel = this.config.proModel;
    }
    this.OnClickEvent = null;
    if (this.config.clickFun) {
        this.OnClickEvent = this.config.clickFun;
    }
    this.ComplateEvent = null;
    if (this.config.ComplateFun) {
        this.ComplateEvent = this.config.ComplateFun;
    }
}

ProgressManage.prototype = {
    DoProgress: function () {
        var create = this.CreateProgress();
        if (create == false) {
            return;
        }
        ProgressGlo.CurSteep = 0;
        ProgressGlo.TimeOut = null;
        var totalKb = this.TotalCount;
        var speedTime = this.Timing;
        var proModel = this.proModel;
        var ComplateEvent = this.ComplateEvent;

        var animateFunc = function () {
            ProgressGlo.CurSteep++;
            if (proModel == ProgressModel.percent) {
                $("#progressLoading > div").css("width", String(100 * ProgressGlo.CurSteep / totalKb) + "%"); //控制#loading div宽度
                $("#progressLoading > div").html(String(parseInt(ProgressGlo.CurSteep * (100 / totalKb))) + "%"); //显示百分比
            } else {
                $("#progressLoading > div").css("width", String(100 * ProgressGlo.CurSteep / totalKb) + "%"); //控制#loading div宽度
                $("#progressLoading > div").html(String(ProgressGlo.CurSteep) + "\""); //显示数字
            }
            if (ProgressGlo.CurSteep < totalKb) {
                ProgressGlo.TimeOut = setTimeout(animateFunc, speedTime);
            } else {
                ProgressGlo.TimeOut = null;
                if (ComplateEvent) {
                    ComplateEvent();
                }
                //$("#progressLoading").remove();
            }
        }
        ProgressGlo.TimeOut = setTimeout(animateFunc, speedTime);
    },
    ReStartProgress: function () {
        var totalKb = this.TotalCount;
        var speedTime = this.Timing;
        var proModel = this.proModel;
        var ComplateEvent = this.ComplateEvent;
        var animateFunc = function () {
            ProgressGlo.CurSteep++;
            if (proModel == ProgressModel.percent) {
                $("#progressLoading > div").css("width", String(100 * ProgressGlo.CurSteep / totalKb) + "%"); //控制#loading div宽度
                $("#progressLoading > div").html(String(parseInt(ProgressGlo.CurSteep * (100 / totalKb))) + "%"); //显示百分比
            } else {
                $("#progressLoading > div").css("width", String(100 * ProgressGlo.CurSteep / totalKb) + "%"); //控制#loading div宽度
                $("#progressLoading > div").html(String(ProgressGlo.CurSteep) + "\""); //显示数字
            }
            if (ProgressGlo.CurSteep < totalKb) {
                ProgressGlo.TimeOut = setTimeout(animateFunc, speedTime);
            } else {
                ProgressGlo.TimeOut = null;
                if (ComplateEvent) {
                    ComplateEvent();
                }
            }
        }
        ProgressGlo.TimeOut = setTimeout(animateFunc, speedTime);
    },
    CreateProgress: function () {
        if (this.contentId) {
            if ($("#progressLoading")[0]) {
                return false;
            } else {
                $("#" + this.contentId).empty();
                $("<div id='progressLoading' class=\"loading_meter\"><div></div></div>").appendTo("#" + this.contentId);

                var clickEvent = this.OnClickEvent;
                if (clickEvent != null) {
                    $("#progressLoading").on("tap", function () {
                        clickEvent();
                    });
                }
            }
            return true;
        }
        return false;
    },
    RemoveLoading: function () {
        $("#progressLoading").remove();
    }
}