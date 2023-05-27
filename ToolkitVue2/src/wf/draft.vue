<template>
	<div id="Runing">
			<el-table ref="runTable"  :data="tableData" row-key="WorkID"  default-expand-all border style="width: 100%; margin-bottom: 20px;" :row-class-name="tableRowClassName" :tree-props="{children: 'children', hasChildren: 'hasChildren'}">		
				<el-table-column label="#" width="50" fixed type="index">
				</el-table-column>
				<el-table-column
						prop="Title"
						label="标题"
						fixed   min-width="250">
					<template slot-scope="scope">
						<span v-if="scope.row.type!=null">{{scope.row.Title}}</span>
						<span v-else><el-link  :underline="false" type="primary"  @click="sikpMyflow(scope.row)">{{scope.row.Title}}</el-link></span>
					</template>

				</el-table-column>
				<el-table-column
						prop="FlowName"
						label="流程">
				</el-table-column>
				<el-table-column
						prop="RDT"
						label="保存日期">
				</el-table-column>
				<el-table-column label="操作" width="100" align="center">
					<template slot-scope="scope">
						<el-button v-show="scope.row.type==null" @click="deleteRow(scope.row)" type="primary"
							size="mini" plain>删除</el-button>
					</template>
				</el-table-column>
				<!--<el-table-column label="操作" width="150">
					<template slot-scope="scope">
						<el-button @click="handleClick(scope.row)" type="text" size="small">轨迹</el-button>
						<el-button type="text" size="small">撤销</el-button>
						<el-button type="text" size="small">催办</el-button>
					</template>
				</el-table-column>-->
			</el-table>

		<el-pagination background
			@size-change="handleSizeChange"
			@current-change="handleCurrentChange"
			:current-page="currentPage"
			:page-sizes="[5,10,15,20]"
			:page-size="pageSize"
			layout="total,sizes, prev, pager, next"
			:total="total"
			v-show="total>0"
		></el-pagination>
		
	</div>
</template>

<script>
import {openMyFlow} from "./api/Dev2Interface";

export default {
	name: "Runing",
	components: {},
	data() {
		return {
			title: "在途",
			data: {},
			tableData: [],
			total: 0, // 总数
			currentPage: 1, // 当前页
			pageSize:15//一页显示的行数
		};
	},

	beforeCreate() {},
	created() {
		this.loadData();
	},
	methods: {
		tableRowClassName({row}) {
			if (row.type === null)
				return '';

			if (row.type === 1) {
				return 'success-row';
			}
			return '';
		},
		// 获取数据
		loadData() {
			let handler = new this.HttpHandler("BP.WF.HttpHandler.WF");
			let data = handler.DoMethodReturnString("Draft_Init");
			if(data.indexOf("err@")!=-1){
				this.$message.error(data);
				console.log(data);
				return;
			}
			this.data = JSON.parse(data);
			this.total = this.data.length;
			this.paging();

		},

		// 分页
		paging() {
			let start = (this.currentPage - 1) * this.pageSize;
			let end = this.currentPage * this.pageSize;
			let arr = this.data.slice(start, end);
			this.filterData(arr);
		},

		// 过滤数据
		filterData(data) {
			this.tableData=[];
			//var flowNos="";
			data.forEach(item => {
				this.tableData.push(item);
			});
			console.log(this.tableData);
		},
/* 		GetChildren(flowNo,gwfs){
			var data=[];
			gwfs.forEach(item => {
				if(item.FK_Flow==flowNo){
					if (item.TodoEmps.indexOf(',') != -1)
						item.TodoEmps = item.TodoEmps.substring(item.TodoEmps.indexOf(',') + 1);
					data.push(item);
				}

			});
			return data;
		}, */

		// 查询
		onSubmit() {},
		handleSizeChange(val) {
			this.pageSize = val;
			this.currentPage=1;
			this.paging();
			console.log(`每页 ${val} 条`);
		},
		handleCurrentChange(val) {
			this.currentPage = val;
			this.paging();
			console.log(`当前页: ${val}`);
		},
		deleteRow(rowData){
			this.$confirm('确定要删除吗？', '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
			}).then(() => {
				var handler = new this.HttpHandler("BP.WF.HttpHandler.WF");
				handler.AddPara("WorkID", rowData.WorkID);
				handler.AddPara("FK_Flow", rowData.FK_Flow);
				var data = handler.DoMethodReturnString("Draft_Delete");
				if (data.indexOf("err@") != -1) {
					alert(data);
					return;
				}
				//$("#msgContent").html(data);
				this.$message.error(data);
				this.loadData();
			});
		},

		//跳转到jflow页面
		sikpMyflow(work){
			let params = {};
			params.WorkID = work.WorkID;
			params.FK_Flow = work.FK_Flow;
			params.FK_Node = work.FK_Node;
			params.FID = work.FID;
			params.FromPage = 'Running';
      openMyFlow(params,this);
		}
	},

	//监听
	computed: {},

	//监听后执行动作
	watch: {}
};
</script>

<style>
	/* .el-table .success-row {
		background: #f0f9eb;
	} */
</style>