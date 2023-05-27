<template>
    <div id="CC">
        <el-form inline style="margin-top:10px">
            <el-form-item label>
                <el-autocomplete
                        class="inline-input"
                        v-model="selectEmps"
                        :fetch-suggestions="querySearch"
                        placeholder="查张三你可以输入张,zs,或者zhangs,zhangsan"
                        :trigger-on-focus="false"
                        @select="handleSelect"
                ></el-autocomplete>
            </el-form-item>
            <el-form-item>
                <el-popover placement="bottom" width="400" v-model="visible" trigger="click">
                    <SelectEmps v-on:selectEmpsValue="selectEmpsValue" :parentComponent="parentComponent" :isSingle="false"></SelectEmps>
                    <el-button type="primary" slot="reference" :disabled="disabled">选人</el-button>
                </el-popover>
                <el-link :underline="false">查看帮助 ?</el-link>
            </el-form-item>
        </el-form>
        <el-table :data="tableData" style="width: 100%;height:300px;overflow-y:auto;">
        <el-table-column prop="CCToName" label="姓名"></el-table-column>
        <el-table-column prop="CCToDeptName" label="所在部门"></el-table-column>
        <el-table-column label="操作" width="150">
            <template slot-scope="scope">
                <el-button  @click="Delete(scope.row.MyPK)" type="text" size="small">删除</el-button>
            </template>
        </el-table-column>
        </el-table>
    </div>
</template>

<script>
    import SelectEmps from "../components/SelectEmps";

    export default {
        name: "CC",
        components:{
            SelectEmps
        },
        data(){
            return{
                params:{},
                webUser:{},
                tableData:[],
                restaurants:[],
                selectEmps:'',
                visible:false,//popover是否显示
                disabled:false,//按钮是否是禁用
                parentComponent:"CC"
            }
        },

        created() {
            this.params = this.$store.getters.getData;
            this.webUser = this.$store.getters.getWebUser;
            this.loadTable();
        },
        methods:{
            loadTable(){//加载表格
                this.tableData=[]
                var ccs = new this.Entities("BP.WF.CCLists");
                ccs.Retrieve("FK_Node", this.params.FK_Node, "WorkID", this.params.WorkID);
                for(var i=0;i<ccs.length;i++){
                    var cc = ccs[i];
                    this.tableData.push({
                        CCToName:cc.CCToName,
                        CCToDeptName:cc.CCToDeptName,
                        MyPK:cc.MyPK
                    })
                }

            },
            Delete(mypk){ //移除
                var cc = new this.Entity("BP.WF.CCList");
                cc.MyPK = mypk;
                cc.SetPKVal(mypk);
                cc.Delete();
                this.loadTable();
            },
            //自动完成
            querySearch(queryString, cb) {
                if(this.selectEmps==""){
                    this.restaurants=[];
                    return "";
                }
                this.restaurants=[];
                var hand = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
                hand.AddJson(this.params);
                hand.AddPara("TB_Emps", this.selectEmps);
                var data = hand.DoMethodReturnString("HuiQian_SelectEmps");
                if (data.indexOf('err@') == 0) {
                    this.$message.error(data);
                    console.log(data);
                    return;
                }
                data = JSON.parse(data);
                data.forEach(item=>{
                    this.restaurants.push({
                        "No":item.No,
                        "value":item.Name
                    });
                })
                cb(this.restaurants);
            },
            handleSelect(item){
                this.selectEmps="";
                this.DoCC(item.No+","+item.value);
            },
            selectEmpsValue(emps){
                this.DoCC(emps);
                this.visible=false;
            },
            DoCC(emps){
                var handler = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
                handler.AddPara("Emps", emps);
                handler.AddPara("WorkID", this.params.WorkID);
                handler.AddPara("FK_Node", this.params.FK_Node);
                var data = handler.DoMethodReturnString("CC_Send");
                if(data.indexOf("err@")!=-1){
                    this.$message.error(data);
                    return;
                }
                this.loadTable();
            }
        }
    }
</script>

<style scoped>

</style>
