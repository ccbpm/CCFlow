new Vue({
    el: '#flow',
    data: {
        loadingDialog: false,
        Groups: null,
        msgList:null,
        TabActive: '0',
        dataList: null,
        tabClickType:null
    },
    methods: {
        DoDelete: function (mypk) {
            if (confirm('确定要删除吗') == true) {
                var en = new Entity("BP.WF.SMS", mypk);
                console.log(mypk);
               // en.DoMethodReturnString("DoDelete");
                en.Delete();
                this.initData();
            }

            //en.Delete();
        },
        //OpenIt: function (mypk, paraStr) {
        OpenIt: function (item) {
            this.DoRead(item.MyPK);
            paraStr = AtParaToJson(item.AtPara);
            var url = basePath + "/WF/MyView.htm?FK_Flow=" + paraStr.FK_Flow + "&WorkID=" + paraStr.WorkID;
            WinOpen(url);

        },
        DoRead: function (mypk) {
            var en = new Entity("BP.WF.SMS", mypk);
            en.DoMethodReturnString("DoRead");
            this.initData();
        },
        Replay: function (doc, myPK) {
            var en = new Entity("BP.WF.SMS", mypk);
            en.DoMethodReturnString("DoRead");
        },
        SendMsg: function (msg, sendTo) {
            // var en = new Entity("BP.WF.SMS", mypk);
            // en.DoMethodReturnString("DoRead");
        },
        tabClick: function (item) {
            this.tabClickType = item;
            this.msgList = this.dataList["Messages"].filter(item => item.MsgType == this.tabClickType);
        },
        initData: function () {
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_Portal");
            var data = handler.DoMethodReturnJSON("Message_Init");
            if (data.toString().indexOf("err@") != -1) {
                alert(data);
                return;
            }
            this.dataList = data;

            //获得消息分组.
            var groups = data["Groups"];
            //前台处理消息类型.
            for (var i = 0; i < groups.length; i++) {


                if (i == 0) {//选择第一个
                    if (!this.tabClickType)
                        this.tabClickType = groups[i].MsgType;
                }
                if (groups[i].MsgType == "SendSuccess") {
                    groups[i].TypeName = "新工作";
                    continue;
                }
                if (groups[i].MsgType == "HuiQian") {
                    groups[i].TypeName = "会签邀请";
                    continue;
                }
                if (groups[i].MsgType == "ReturnAfter") {
                    groups[i].TypeName = "退回";
                    continue;
                }

            }
            this.Groups = groups;
            //获得消息.
            this.msgList = this.dataList["Messages"].filter(item => item.MsgType == this.tabClickType);

        },
    },
    mounted: function () {

        this.initData();
        
    }
 })
