new Vue({
    el: '#zupu-Default',
    data: {
        puxiAddShow: false,
        list: [],       
    },
    methods: {
        inte: function () {

            var ens = new Entities("BP.ZHOU.JiaPus");
            ens.RetrieveAll();
            ens = ens.TurnToArry();
            console.log(ens)
            this.list = ens;

            
        },
        savePx: function () {
            var en = new Entity("BP.ZHOU.JiaPu");
            en.CopyForm();
            en.Insert();
            layer.msg('添加成功', { time: 1000 })
            this.inte();
        },
        treeVeiw: function (No) {
            location.href = "Treeview.htm?No="+No
        }

    },
    mounted: function () {
        this.inte()
    }

})