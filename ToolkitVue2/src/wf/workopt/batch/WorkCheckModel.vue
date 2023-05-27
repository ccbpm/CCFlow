<template>
	<div id="GroupFieldModel">
		{{workName}}
		<el-table :data="tableData" height="80vh" ref="topicTable" align="center" row-key="WorkID" default-expand-all 
			:row-class-name="tableRowClassName" @selection-change="handleSelectionChange">
			    <el-table-column
			            label="序号" width="50px">
			        <template slot-scope="scope">
			            {{scope.$index+1}}
			        </template>
			    </el-table-column>
			<el-table-column type="selection" />
      <el-table-column prop="Title" label="标题"  min-width="240" max-width="400">
        <template slot-scope="scope">
          <el-link type="primary" @click="OpenMyFlow(scope.row)">{{scope.row.Title}}</el-link>
        </template>
      </el-table-column>
			<el-table-column prop="Starter" label="发起人" width="80"/>
			<el-table-column prop="ADT" label="接收日期" width="150"/>
      <template v-for="(item,index) in tableHead">
        <el-table-column v-if="item.prop === 'CheckMsg'" :prop="item.prop" :label="item.label" :key="index">
          <template slot-scope="scope">
            <el-input type="textarea" v-model="scope.row.CheckMsg"></el-input>
          </template>
        </el-table-column>
        <el-table-column v-else :prop="item.prop" :label="item.label" :key="index" />
      </template>
		</el-table>
		<BatchToolBar :nodeID="NodeID" :selectItems="selectItems" :loadData="loadData"/>
		<div style="margin:10px 0;">
			<span style="font-weight: 500;font-size: 14px;margin-right:10px;color: #606266;">合计： {{count}}条</span>
		</div>
	</div>
</template>

<script>
	import { Entity } from "@/wf/api/Gener.js";
  import BatchToolBar from "../../components/BatchToolBar.vue";
  import {openMyFlow} from "@/wf/api/Dev2Interface";
	export default {
		name: "WorkCheckModel",
		components: {
      BatchToolBar,
    },

		data() {
			return {
				title: "批处理-审核分组",
				tableData: [],
				count: 0,
				activeNames: [],
				NodeID:"",
				workName:"",
				node:null,
				tableHead:[],
				Sort:1,
        selectItems: [],
        dialogFlowVisible:false,

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
        this.tableHead = [];
        this.tableData = [];
				this.NodeID = this.$route.params.NodeID;
				if(this.NodeID==null){
					return;
				}
				let handler = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt_Batch");
				handler.AddPara("FK_Node", this.NodeID);
				let data = handler.DoMethodReturnString("WorkCheckModel_Init");
				if (data.indexOf("err@") != -1) {
					this.$message.error(data);
					console.log(data);
					return;
				}
				data = JSON.parse(data);
				this.filterData(data);
			},

			// 过滤数据
			filterData(data) {
				const works = data.Works; //数据.
				let mapAttrs = data.Sys_MapAttr; //要显示的字段.
				this.node = new Entity("BP.WF.Node", this.NodeID);
				
				let BatchCheckNoteModel = this.node.GetPara("BatchCheckNoteModel");
				if (BatchCheckNoteModel == undefined)
				    BatchCheckNoteModel = "0"; //审核意见填写方式,默认为选择的Item一个意见.
				this.workName = "流程：" +this. node.FlowName + " => 节点：" + this.node.Name;
				
				// 是否动态添加字段
				const files = this.node.GetPara("BatchFields");
				for (let i = 0; i < mapAttrs.length; i++) {
				    const attr = mapAttrs[i];
				    if (files == undefined)
				        continue;
				    if (files.indexOf(attr.KeyOfEn) == -1)
				        continue;
				    if (attr.Name == "审核意见")
				        continue;
					const file={
						label:attr.Name,
						prop:attr.KeyOfEn
					};
					this.tableHead.push(file);
				}
				
				//判断该节点是否启用审核组件
				const sta = this.node.FWCSta;
				////启用审核组件并且可编辑 ，如果是：每个记录后面都有一个意见框.的模式.
				if (sta == 1 && BatchCheckNoteModel == "1") {
					const file={
						label:"审核意见",
						prop:"CheckMsg"
					};
          this.tableHead.push(file);
				}
				this.count = works.length;
				this.tableData = works;
			},
      handleSelectionChange(val) {
        this.selectItems = val;
      },
      OpenMyFlow(rowData){
        let params={};
        params.WorkID = rowData.WorkID;
        params.FK_Flow = this.node.FK_Flow;
        params.FK_Node = this.node.NodeID;
        params.FID = rowData.FID;
        openMyFlow(params,this);
      },
			Btn_Back(){
				this.$router.push({name:"batch"});
			},
		},

		//监听
		computed: {},

		//监听后执行动作
		watch: {}
	};
</script>
<style>
	.el-table .success-row {
		background: #f5f7fa;
		padding: 5px 0px;
	} 
	.el-table td, .el-table th{
			padding: 8px 0px !important;
	}
</style>
<style lang="less" scoped>
	
	
	.ml-5 {
		padding-left: 5px;
	}
    .mr-20{
		padding-right: 20px;
	}
	.iconColor {
		color: #F56C6C;
	}

	.iconColor-w {
		color: #909399;
	}

	.cellfontColor {
		color: #545454;
		font-weight: 600;
	}
</style>
