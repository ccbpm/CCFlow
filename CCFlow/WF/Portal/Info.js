new Vue({
    el: '#ccdiv',
    data: {
        loadingDialog: false,
        infoTypes: null,
        Groups: null,
        infos: null,
        TabActive: '0',
        dataList: null,
        tabClickType: null,
    },
    methods: {
        OpenIt: function (item) {

            //  this.DoRead(item.MyPK);
            // var url = basePath + "/WF/Comm/RefFunc/EnOnly.htm?EnName=BP.CCOA.CCInfo.Info&No=" + item.No;

            var url = "InfoDtl.htm?No=" + item.No;

            OpenLayuiDialog(url, item.Name, 980, 80, "auto", false, false);

            //OpenLayuiDialog(url, 100, 100);
            //  WinOpen(url);
        },
        tabClick: function (item) {
            if (item == "all") {
                this.infos = this.dataList;
            } else {
                this.infos = this.dataList.filter(lis => lis.InfoType == item);

            }
            this.tabClickType = item;


        },
        initData: function () {

            //获得类型.
            var infoTypes = new Entities("BP.CCOA.CCInfo.InfoTypes");
            infoTypes.RetrieveAll();
            infoTypes = infoTypes.TurnToArry();
            var all = { No: 'all', Name: '全部' };
            infoTypes.unshift(all);
            //tabClickType = infoTypes[0].No;

            //获得信息数据.
            var infos = new Entities("BP.CCOA.CCInfo.Infos");
            infos.Retrieve("InfoSta", 0, "InfoSta,RDT");
            infos = infos.TurnToArry();

            this.infoTypes = infoTypes;
            this.dataList = infos;
            this.infos = this.dataList;

        },
    },
    mounted: function () {
        this.initData();
    }
})


$(function () {

    var webUser = new WebUser();
    if (webUser.No == "admin") {
        // $("#infoEnd").show();
        $("#infoEnd").show();
    } else {
        $("#infoEnd").hide();
    }

});

function EditInfo() {

    var url = "../Comm/Search.htm?EnsName=BP.CCOA.CCInfo.Infos";
    OpenLayuiDialog(url, "信息维护",900,0,null,true,true);
}
function EditSort() {

    var url = "../Comm/Ens.htm?EnsName=BP.CCOA.CCInfo.InfoTypes";
    OpenLayuiDialog(url, "类别", 900, 0, null, true, true);
}

