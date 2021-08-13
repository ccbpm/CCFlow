padDate = (value) => {
    return value < 10 ? '0' + value : value;
}

new Vue({
    el: '#Dictlog-list',
    data: {
        list: []        
    },    
    methods: {},
    mounted: function () {
        frmID = GetQueryString("FrmID");
        workID = GetQueryString("WorkID");

        //获得所有的日志信息.
        var ens = new Entities("BP.CCBill.Tracks");
        ens.Retrieve("WorkID", workID, "RDT");
        var lists = ens;
        lists = obj2arr(lists);
       /* lists.forEach(function (lists) {
            lists.date = 0;
            lists.time = 1;

        })*/
        this.list = lists;
        console.log(this.list);
    },
    filters: {
        formatTime: function (value) {  //这里的 value 就是需要过滤的数据
            var date = new Date(value);           
            var hours = padDate(date.getHours());
            var minutes = padDate(date.getMinutes());           
            return  hours + ':' + minutes;
        },
        formatDate: function (value) {  //这里的 value 就是需要过滤的数据
            var date = new Date(value);
            var year = date.getFullYear();
            var month = padDate(date.getMonth() + 1);
            var day = padDate(date.getDate());
            var dateArray = value.split("-");
            var dates = new Date(year, month, day-1);
            var week = "星期" + "日一二三四五六".charAt(dates.getDay());         
           
            return year + '-' + month + '-' + day + '  ' +week;
        }
    }
  
})
    function obj2arr(obj) {
        delete obj.Paras
        delete obj.ensName
        delete obj.length
        var arr = []
        for (var key in obj) {
            if (Object.hasOwnProperty.call(obj, key)) {
                arr.push(obj[key]);
            }
        }
        return arr
    }