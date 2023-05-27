<template>
    <div id="HuiQian">
        <el-table :data="tableData" style="width: 100%;height:200px;overflow-y:auto;">
            <el-table-column prop="FK_EmpT" label="姓名"></el-table-column>
            <el-table-column prop="FK_Dept" label="所在部门"></el-table-column>
            <el-table-column prop="State" label="状态"></el-table-column>
            <el-table-column label="操作" width="150">
                <template slot-scope="scope">
                    <el-button v-show="scope.row.Oper==1" @click="Delete(scope.row.FK_Emp)" type="text" size="small">删除</el-button>
                </template>
            </el-table-column>
        </el-table>
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
                    <el-button type="primary" slot="reference" :disabled="disabled">选择会签人</el-button>
                </el-popover>
                <el-link :underline="false">查看帮助 ?</el-link>
            </el-form-item>
        </el-form>
        <el-button type="primary" @click="onSubmit" :disabled="disabled">执行会签</el-button>
    </div>
</template>

<script>
    import {GetPara} from "@/wf/api/Gener.js";
    import SelectEmps from "../components/SelectEmps";
export default {
    name:'HuiQian',

    data() {
        return {
            params:{},
            webUser:{},
            tableData:[],
            restaurants:[],
            selectEmps:'',
            visible:false,//popover是否显示
            disabled:false,//按钮是否是禁用
            parentComponent:"HuiQian"

        };
    },

    beforeCreate() {

    },

    created() {
        this.params = this.$store.getters.getData;
        this.webUser = this.$store.getters.getWebUser;
        this.loadTable();
    },

    methods: {
        loadTable(){
            this.tableData=[];
            var hand = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
            hand.AddJson(this.params);
            var data = hand.DoMethodReturnString("HuiQian_Init");
            if (data.indexOf('err@') == 0) {
                this.$message.error(data);
                console.log(data);
                return;
            }
            data = JSON.parse(data);
            var gwls = data.WF_GenerWorkList;
            var myGwl = data.My_GenerWorkList[0];
            //处理数据
            var _this = this;
            gwls.forEach(gwl=>{
                var zhuChiRen = GetPara(gwl.AtPara, "HuiQianZhuChiRen");
                var addLeader = GetPara(gwl.AtPara, "HuiQianType");
                if (gwl.FK_Emp != _this.webUser.No) { //相同即为主持人
                    if (zhuChiRen != null && zhuChiRen != undefined && zhuChiRen!=  _this.webUser.No)
                        return true;

                    //获取增加组长的信息
                    if (_this.params.HuiQianType != null && _this.params.HuiQianType != undefined && _this.params.HuiQianType == "AddLeader") {
                        if (addLeader == null || addLeader != "AddLeader")
                            return true;
                    } else {
                        if (addLeader != null && addLeader == "AddLeader")
                            return true;
                    }

                    if (GetPara(myGwl.AtPara, "HuiQianZhuChiRen") == gwl.FK_Emp)
                        return true;
                }
                if(gwl.FK_EmpText.indexOf("<img")!=-1)
                    gwl.FK_EmpText = gwl.FK_EmpText.substr(gwl.FK_EmpText.lastIndexOf("/>")+2);
                if (gwl.IsPass == -1) {
                    _this.tableData.push({
                        FK_EmpT:gwl.FK_EmpText,
                        FK_Emp:gwl.FK_Emp,
                        FK_Dept:gwl.FK_DeptT,
                        State:"新增",
                        Oper:"1"
                    });
                }

                if (gwl.IsPass == 0) {
                    if (gwl.FK_Emp ==  _this.webUser.No) {
                        _this.tableData.push({
                            FK_EmpT:gwl.FK_EmpText,
                            FK_Emp:gwl.FK_Emp,
                            FK_Dept:gwl.FK_DeptT,
                            State:"主持人/未审批",
                            Oper:"0"
                        });

                    }
                    else {
                        _this.tableData.push({
                            FK_EmpT:gwl.FK_EmpText,
                            FK_Emp:gwl.FK_Emp,
                            FK_Dept:gwl.FK_DeptT,
                            State:"未审批",
                            Oper:"1"
                        });
                    }

                }



                //当前是主持人这个是协作模式处理
                if (gwl.IsPass == 100) {
                    _this.tableData.push({
                        FK_EmpT:gwl.FK_EmpText,
                        FK_Emp:gwl.FK_Emp,
                        FK_Dept:gwl.FK_DeptT,
                        State:"主持人/未审批",
                        Oper:"0"
                    });
                }
                if (gwl.IsPass == 1001) {
                    _this.tableData.push({
                        FK_EmpT:gwl.FK_EmpText,
                        FK_Emp:gwl.FK_Emp,
                        FK_Dept:gwl.FK_DeptT,
                        State:"主持人/已审批",
                        Oper:"0"
                    });
                }

                //当前是组长模式处理，并且是主持人
                if (gwl.IsPass == 90 ) {
                    _this.tableData.push({
                        FK_EmpT:gwl.FK_EmpText,
                        FK_Emp:gwl.FK_Emp,
                        FK_Dept:gwl.FK_DeptT,
                        State:"主持人（自己）/未审批",
                        Oper:"0"
                    });
                }

                //当前自己
                if (gwl.IsPass == 99) {
                    _this.tableData.push({
                        FK_EmpT:gwl.FK_EmpText,
                        FK_Emp:gwl.FK_Emp,
                        FK_Dept:gwl.FK_DeptT,
                        State:"主持人/未审批",
                        Oper:"0"
                    });
                }

                //当前自己
                if (gwl.IsPass == 9901) {
                    _this.tableData.push({
                        FK_EmpT:gwl.FK_EmpText,
                        FK_Emp:gwl.FK_Emp,
                        FK_Dept:gwl.FK_DeptT,
                        State:"主持人（您自己）/已审批",
                        Oper:"0"
                    });
                }

                if (gwl.IsPass == 1) {
                    _this.tableData.push({
                        FK_EmpT:gwl.FK_EmpText,
                        FK_Emp:gwl.FK_Emp,
                        FK_Dept:gwl.FK_DeptT,
                        State:"已审批",
                        Oper:"0"
                    });
                }


            });
        },
        //移除会签人员
        Delete(empNo){
            var hand = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
            hand.AddJson(this.params);
            hand.AddPara("FK_Emp", empNo);
            var data = hand.DoMethodReturnString("HuiQian_Delete");

            if (data.indexOf('err@') == 0 || data.indexOf('info@') == 0) {
                this.$message.error(data);
                console.log(data);
                return;
            }
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
            this.DoHuiQian(item.No);
        },
        selectEmpsValue(emps){
            this.DoHuiQian(emps);
            this.visible=false;
        },
        DoHuiQian(emps){
            //执行数据初始化工作.
            var hand = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
            hand.AddJson(this.params);
            hand.AddPara("AddEmps", emps);
            hand.AddPara("HuiQianType", this.params.HuiQianType); // 会签类型：AddLeader增加组长，其他增加普通员工会签
            var data = hand.DoMethodReturnString("HuiQian_AddEmps");

            if (data.indexOf('err@') == 0) {
                this.$message.error(data);
                console.log(data);
                return;
            }
            this.loadTable();

        },

        //执行会签
        onSubmit(){
            this.disabled= true;
            var flag = false;
            this.tableData.forEach(item=>{
                if(item.Oper==1){
                    flag = true;
                    return false;
                }
            });
            if(flag == false){
                this.$message.waring("table表中的会签人已经增加，请选择其他人加签");
                return;
            }
            var hand = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
            hand.AddJson(this.params);
            var data = hand.DoMethodReturnString("HuiQian_SaveAndClose");
            if(data.indexOf("err@")!=-1){
                this.$message.error(data.replace("err@",""))
                return;
            }
            if(data.indexOf("url@")!=-1){
                this.$parent.judgeOperation = "sendAccepter";
                this.$parent.Title = "选择下一个节点的接收人";
                return;
            }
            //如果需要发送,就执行发送.
            if (data.indexOf('Send@') == 0) {
                this.SendIt();
                return;
            }
            if (data.indexOf('close@') == 0){
                data = data.replace("close@","");
                data = data.replace("<br>","").replace("<br>","").replace("<br>","").replace("<br>","");
                this.$alert(data.replace("close@",""), '加签消息', {
                    dangerouslyUseHTMLString: true
                });
                this.$router.push({
                    name: "todolist"
                });
            }


        },
        SendIt(){
            //执行数据初始化工作.
            var hand = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
            hand.AddJson(this.params);
            var data = hand.DoMethodReturnString("AccepterOfGener_Send");
            if(data.indexOf("err@")!=-1){
                this.$message.error(data.replace("err@",""))
                return;
            }
            data = data.replace("'MyFlowInfo", "'../MyFlowInfo");
            data = data.replace("'MyFlow.htm", "'../MyFlow.htm");
            data = data.replace("'MyFlow.htm", "'../MyFlow.htm");

            data = data.replace("'WFRpt", "'../WFRpt");
            data = data.replace("'WFRpt", "'../WFRpt");

            data = data.replace("'./Img", "'../Img");
            data = data.replace("'./Img", "'../Img");
            data = data.replace("'./Img", "'../Img");
            data = data.replace("'./Img", "'../Img");
            data = data.replace("'./Img", "'../Img");

            data = data.replace("'./WorkOpt/", "");
            data = data.replace("'./WorkOpt/", "");
            data = data.replace("'./WorkOpt/", "");
            data = data.replace("'./WorkOpt/", "");


            data = data.replace('@', '<br/>@');
            data = data.replace(/@/g, '<br/>&nbsp;@');

            data = data.replace('@', '<br/>@');
            this.$alert(data, '发送成功消息', {
                dangerouslyUseHTMLString: true
            });
            this.$router.push({
                name: "todolist"
            });
        }
    },

    //监听
    computed: {

    },


    components: {
        SelectEmps
    },

    //监听后执行动作
    watch: {

    }
}
</script>

<style lang="less" scoped>
</style>