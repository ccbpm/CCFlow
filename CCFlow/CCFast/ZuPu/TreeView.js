var ens = new Entities("BP.ZHOU.Peoples");
ens.Retrieve("JiaPuNo", GetQueryString("No"));
ens = ens.TurnToArry();
//console.log(JSON.stringify(ens))
//var treeData = ens;



var treeData = {
    id:'1',
    name: "王源",
    wife: [{ id: '11', name: "李有" },
        { id: '12', name: "张丽" }],
    children: [
        {
            id: '2',
            name: "王三",
            wife: [{ id: '22', name: "袁望" },
                { id: '23', name: "牛宽" }],
        },
        {
            id: '3',
            name: "王五",
            wife: [{ id: '31', name: "客服" },
                { id: '32', name: "李丽" }],
        },
        
    ],
   
};

// define the tree-item component
Vue.component("tree-item", {
    template: "#item-template",
    props: {
        item: Object
    },
    data: function () {
        return {
            isOpen: false,
            isOpenChild:false
        };
    },
    computed: {
        isFolder: function () {
            return this.item.children && this.item.children.length;
        },
        isWife: function () {
            return this.item.wife && this.item.wife.length;
        }
    },
    methods: {
        toggle: function () {
            if (this.isFolder) {
                this.isOpen = !this.isOpen;
                this.isOpenChild = !this.isOpenChild;
            }
        },
        makeFolder: function () {
            if (!this.isFolder) {
                this.$emit("make-folder", this.item);
                this.isOpen = true;
                this.isOpenChild = true;
            }
        }
    }
});
new Vue({
    el: '#zp-tree',

    data: {
            ShowXing: true,
            ShowNv: true,
            ShowShi: true,
            ShowRight: false,
            ShowCateEdit: false,
            zpInfo:'',
        treelist: [],
        treeData: treeData
    },
    methods: {
        inte: function () {
            var enZp = new Entity("BP.ZHOU.JiaPu", GetQueryString("No"));
            enZp.Retrieve();
            console.log(enZp)
            this.zpInfo = enZp;
           

        },
       
        savePx: function () {
            var en = new Entity("BP.ZHOU.JiaPu");
            en.CopyForm();
            en.Update();
            layer.msg('添加成功', { time: 1000 })
            this.inte();
        },
        treeVeiw: function (No) {
            location.href = "Treeview.htm?No=" + No
        },
        makeFolder: function (item) {
            Vue.set(item, "children", []);
            Vue.set(item, "wife", []);
            this.addItem(item);
            this.addWife(item);
        
        },
        addItem: function (item) {
            item.children.push({
                name: "子女",
                children: [],
                isOpenChild:false
            });           
        },
        addWife: function (item) {
            item.wife.push({
                name: "配偶",                
            });
            console.log(item)
        },
      
        


    },
    mounted: function () {
        this.inte()
    }

})