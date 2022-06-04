new Vue({
    el: '#ccdiv',
    data: {
        loadingDialog: false,
        BBSTypes: null,
        Groups: null,
        BBSs: null,
        TabActive: '0',
        dataList: null,
        tabClickType: null,
    },
    methods: {
        OpenIt: function (item) {

            var url = "Dtl.htm?No=" + item.No;
            OpenLayuiDialog(url, item.Name, 980, 0, "r", false, false);

        },
        tabClick: function (item) {
            if (item == "all") {
                this.BBSs = this.dataList;
            } else {
                this.BBSs = this.dataList.filter(lis => lis.BBSType == item);
            }
            this.tabClickType = item;
        },
        initData: function () {

            //获得类型.
            var types = new Entities("BP.CCOA.CCBBS.BBSTypes");
            types.RetrieveAll();
            types = types.TurnToArry();
            var all = { No: 'all', Name: '全部' };
            types.unshift(all);
            //tabClickType = BBSTypes[0].No;

            //获得信息数据.
            var ens = new Entities("BP.CCOA.CCBBS.BBSs");
            ens.Retrieve("BBSSta", 0, "RDT DESC ");
            ens = ens.TurnToArry();

            this.BBSTypes = types;
            this.dataList = ens;
            this.BBSs = this.dataList;

        },
    },
    mounted: function () {
        this.initData();
    }
})

function Ask() {
    var url = "../../WF/Comm/EnOnly.htm?EnName=BP.CCOA.CCBBS.BBSExt";
    OpenLayuiDialog(url, "我要提问", 980, 80, "auto", false, false);
}


$(function () {

    var webUser = new WebUser();
    if (webUser.No == "admin") {
        $("#BBSEnd").show();
    } else {
        $("#BBSEnd").hide();
    }

});

function EditBBS() {

    var url = "../../WF/Comm/Search.htm?EnsName=BP.CCOA.CCBBS.BBSs";
    OpenLayuiDialog(url, "维护", 900, 0, null, true, true);
}
function EditSort() {

    var url = "../../WF/Comm/Ens.htm?EnsName=BP.CCOA.CCBBS.BBSTypes";
    OpenLayuiDialog(url, "类别", 900, 0, null, true, true);
}

