<template>
	<div id="Runing">
		<span content-position="left" class="Tip">请选择要批处理的节点</span>
		<el-divider ></el-divider>
		<el-table :data="tableData" 
			:show-header="true"  
			:stripe="true"  
			highlight-current-row
			@current-change="sikpflows($event)"
		>
		<el-table-column fixed type="index" label="#" width="100"></el-table-column>
			<el-table-column width="300" property="FlowName" label="流程名称">
			</el-table-column>
			<el-table-column  label="描述">
				<template #default="scope">
					<span>{{scope.row.Name}}({{scope.row.NUM}})</span>
				</template>
			</el-table-column>
		</el-table>
	</div>
</template>

<script>
	export default {
		name: "batchbo",
		components: {},
		data() {
			return {
				title: "批处理",
				data: {},
				tableData: [],
			};
		},

		beforeCreate() {},
		created() {
			this.loadData();
		},
		methods: {
			tableRowClassName({
				row
			}) {
				if (row.type === null)
					return '';

				if (row.type === 1) {
					return 'success-row';
				}
				return '';
			},
			// 获取数据
			loadData() {
				//let handler = new this.HttpHandler("BP.WF.HttpHandler.WF");
				//let data = handler.DoMethodReturnString("Runing_Init");
				let handler = new this.HttpHandler("BP.WF.HttpHandler.WF");
				let data = handler.DoMethodReturnString("Batch_Init");
				if (data.indexOf("err@") != -1) {
					this.$message.error(data);
					console.log(data);
					return;
				}
				this.data = JSON.parse(data);
				this.total = this.data.length;
				this.filterData();

			},

			// 过滤数据
			filterData() {
				//this.tableData=this.data;
				this.data.forEach(item => {
					this.tableData.push(item);
				});
				//this.data=this.tableData;
				console.log(this.tableData);
			},
			GetChildren(flowNo, gwfs) {
				var data = [];
				gwfs.forEach(item => {
					if (item.FK_Flow == flowNo) {
						if (item.TodoEmps.indexOf(',') != -1)
							item.TodoEmps = item.TodoEmps.substring(item.TodoEmps.indexOf(',') + 1);
						data.push(item);
					}

				});
				return data;
			},

			// 查询
			onSubmit() {},

			//跳转到jflow页面
			sikpflows(en) {
				let params = {};
				var uname = "";
				//url = "./WorkOpt/Batch/BatchList.htm?FK_Node=" + en.NodeID;
				uname = "batchList";
				//审核组件模式.
				if (en.BatchRole == 1)
					uname = "WorkCheckModel";
				//url = "./WorkOpt/Batch/WorkCheckModel.htm?FK_Node=" + en.NodeID;

				//审核分组模式.
				if (en.BatchRole == 2)
					uname = "WorkCheckModel";
				//url = "./WorkOpt/Batch/GroupFieldModel.htm?FK_Node=" + en.NodeID;

				console.log(uname);
				params.FromPage = 'batch';
				params.NodeID = en.NodeID;
				// this.$router.push("myFlow?FK_Flow=350");
				this.$router.push({
					name: uname,
					params
				});
				//this.$router.push({name:"myflow",params:{FK_Flow:item.No,FromPage:"Start"}});
			}
		},

		//监听
		computed: {},

		//监听后执行动作
		watch: {}
	};
</script>

<style lang="less" scoped="scoped">
	/* .el-table .success-row {
		background: #f0f9eb;
	} */
	.item {
		margin-top: 10px;
		margin-right: 40px;
	}
	.ele-tag-round{
		width: 20px;
		height: 20px;
		padding: 0 5px;
		line-height: 19px;
		padding-left: 0;
		padding-right: 0;
		border-radius: 50%;
		text-align: center;
	}
	.Tip{
		color: #409EFF;
	}
	::v-deep .el-table__body tr:hover{
		cursor: pointer;
	}
</style>
