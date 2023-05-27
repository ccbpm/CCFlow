<template>
	<div id="timeTable">
		<template>
			<el-table
					:data="data"
					style="width: 100%;"
					>
				<el-table-column
						fixed
            type="index"
						label="序号"
						width="50">
				</el-table-column>
				<el-table-column
						prop="RDT"
						label="日期"
						width="160">
				</el-table-column>
				<el-table-column
						prop="NDFrom"
						label="节点ID"
						width="70">
				</el-table-column>
				<el-table-column
						prop="NDFromT"
						label="节点名称"
						width="120">
				</el-table-column>
				<el-table-column
						prop="EmpFrom"
						label="操作人"
						width="110">
				</el-table-column>
				<el-table-column
						prop="EmpFromT"
						label="操作人名称"
						width="110">
				</el-table-column>
        <el-table-column label="操作" width="100">
          <template slot-scope="scope">
            <el-button
                size="mini"
                @click="Done(scope.$index, scope.row)">执行</el-button>
          </template>
        </el-table-column>
			</el-table>
		</template>

	</div>
</template>

<script>
export default {
	name: "Rollback",

	data() {
		return {
			data:[],
			params:{},
		};
	},

	beforeCreate() {},

	created() {},
	mounted() {
			this.params = this.$route.query;
			const handler = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
			handler.AddJson(this.params);
			let data = handler.DoMethodReturnString("Rollback_Init");
			this.data=JSON.parse(data);
	},

	methods: {
    Done:function(index,row){
      let _this = this;
      this.$prompt('请输入回滚原因', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
      }).then(({ value }) => {
        let flow = new this.Entity("BP.WF.Template.FlowExt", _this.params.FK_Flow);
        let data = flow.DoMethodReturnString("DoRebackFlowData", _this.params.WorkID, row.NDFrom, value);
        if(typeof data ==='string'&&data.includes('err@')){
          this.$message({
            type: 'error',
            message: data.replace('err@','')
          });
          return;
        }
        this.$message({
          type: 'success',
          message: data
        });
        this.$router.replace({
          path: '/todolist'
        })
        this.toolBarInstance.dialogFormVisible = false;
      })
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
/deep/ .el-card__body {
	p {
		line-height: 1;
	}
}
.portrait {
	width: 50px;
	height: 50px;
	border-radius: 50%;
}
</style>