<template>
	<div id="Accepter">
		<div v-if="isShowMsg">
			<el-checkbox
				:indeterminate="isIndeterminate"
				v-model="checkAll"
				@change="handleCheckAllChange"
			>全选</el-checkbox>
			<el-card class="box-card" v-for="(dept,index) in depts" :key="index">
				<div slot="header" class="clearfix">
					<span>{{dept.Name}}</span>
					<el-button style="float: right; padding: 3px 0" type="text"></el-button>
					<div v-if="isSimplate == 0">
						<el-checkbox-group v-model="selected" @change="handleCheckedCitiesChange">
							<span v-for="(emp,index) in emps" :key="index">
								<el-checkbox v-if="emp.FK_Dept == dept.No" :label="emp" >{{emp.Name}}</el-checkbox>
							</span>
						</el-checkbox-group>
					</div>
					<div v-else>
						<span v-for="(emp,index) in emps" :key="index">
							<el-radio
								v-if="emp.FK_Dept == item.No"
								v-model="radio"
								:label="index"
								@change="radioClick(emp)"
							>{{emp.Name}}</el-radio>
						</span>
					</div>
				</div>
			</el-card>
			<div style="text-align:right;">
				<el-button  type="success" @click="onSubmit">发送</el-button>
			</div>
		</div>
		<div v-else>
			<p v-for="(item,index) in msg" :key="index" v-html="item"></p>
      <div style="text-align: center">
        <el-button type="primary" @click="closeDialog">确定</el-button>
      </div>
		</div>
	</div>
</template>

<script>
export default {
	name: "Accepter",
	props: {
		data: Object,
    urlParams:{type:Object,default:()=>{}}
	},
  inject: {
    toolBarInstance: {}
  },
	data() {
		return {
			isShowMsg: true,
			msg:[],
			params: {},
			checkAll: false,
			isIndeterminate: true,
			depts: "", //部门集合
			emps: "", //人员集合
			selected: "", //已经选择的人员集合.
			isSimplate: "", // =0 是多选   =1 是单选.
			radio: "" ,//单选值
      isShowSend:true
		};
	},

	beforeCreate() {},

	created() {

		this.params =this.urlParams;
    this.isShowSend = this.params.IsSend==undefined?true : this.params.IsSend;
		this.depts = this.data.Depts; //部门集合. 这个集合也可能为空.
		this.emps = this.data.Emps; //人员集合.
		this.selected = this.data.Selected; //已经选择的人员集合.
		this.isSimplate = this.data.Selector[0].IsSimpleSelector;
		//是否是单选？  =0 是多选   =1 是单选.
		if (
			this.emps == null ||
			this.emps == undefined ||
			this.emps.length == 0
		) {
			this.$message({
				message:
					"当前节点设置的接收人范围为空,请联系管理员配置接收人范围",
				type: "warning"
			});
			return;
		}
		console.log("单选还是多选", this.isSimplate);
	},

	methods: {
		handleCheckAllChange(val) {
			this.selected = val ? this.emps : [];
			this.isIndeterminate = false;
			console.log(this.selected);
		},
		handleCheckedCitiesChange(value) {

			let checkedCount = value.length;
			this.checkAll = checkedCount === this.emps.length;
			this.isIndeterminate =
				checkedCount > 0 && checkedCount < this.emps.length;
			console.log(this.selected);
		},
		radioClick(val) {
			this.selected = [];
			this.selected.push(val);
			console.log(this.selected);
		},

    closeDialog(){
      this.toolBarInstance.dialogFormVisible = false
    },
    GetEmps(){
      let arr = [];
      this.selected.forEach(item => {
        arr.push(item.No);
      });
      return arr;
    },
		// 发送
		onSubmit() {
      if(this.isShowSend == false){
        this.$emit("accepterSend");
        return;
      }

      let selectedEmps;
			let arr = [];
			this.selected.forEach(item => {
				arr.push(item.No);
			});
			selectedEmps = arr.join(";");
			if (!selectedEmps) {
				this.$message({
					message: "请选择人员",
					type: "warning"
				});
				return;
			}
			const handler = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
			handler.AddJson(this.params); //这里是获得Url的参数.
			//多个参数用逗号分开的比如： zhangsan;lisi;wangwu
			handler.AddPara("SelectEmps", encodeURI(selectedEmps));
      const data = handler.DoMethodReturnString("Accepter_Send"); //执行发送方法.
			if (data.indexOf("err@") == 0) {
				alert(data);
				return;
			}
			this.msg = data.split("@");
			this.isShowMsg = false;
      this.toolBarInstance.title = '发送成功'
		}
	},

	//监听
	computed: {},

	components: {},

	//监听后执行动作
	watch: {}
};
</script>

<style lang="less" scoped>
/deep/ .el-card {
	margin: 5px 0;
}
/deep/ .el-card__header {
	height: 30px;
	line-height: 30px;
	padding: 0 20px;
}
	/deep/ .el-checkbox{
		margin-right:10px;
	}
</style>
