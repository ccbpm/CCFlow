<template>
	<div class="power">
		<el-divider content-position="left">[{{webuser.No}}]授权列表</el-divider>
		<el-table :data="tableData" default-expand-all size="small">
      <el-table-column label="#" width="50" fixed type="index">
      </el-table-column>
      <el-table-column prop="AutherToEmpName" label="授权给" width="100">
      </el-table-column>
      <el-table-column prop="AuthType" label="授权类型">
        <template slot-scope="scope">
          <span v-if="scope.row.AuthType == 0" >不授权</span>
          <span v-else-if="scope.row.AuthType == 1">全部流程授权</span>
          <span v-else-if="scope.row.AuthType == 2">指定流程授权</span>
        </template>
      </el-table-column>
      <el-table-column  label="流程" width="100">
        <template slot-scope="scope">
          <span v-if="scope.row.AuthType == 2">{{scope.row.flowNames}}</span>
          <span v-else>{{scope.row.FlowNo}}.{{scope.row.FlowName}}</span>
        </template>
      </el-table-column>
      <el-table-column prop="TakeBackDT" label="收回日期" width="100">
      </el-table-column>
      <el-table-column prop="TakeBackDT" label="状态" width="100">
      </el-table-column>
      <el-table-column fixed="right" label="操作" width="100">
          <template slot-scope="scope">
            <el-button  @click.native.prevent="deleteRow(scope.$index, tableData)" type="text" size="small">删除</el-button>
          </template>
      </el-table-column>
		</el-table>
	</div>
</template>

<script>
	import {
		mapState
	} from 'vuex'
	export default {
		data() {
			return {
				tableData:""
			}
		},
		computed: {
			...mapState([
				'webuser'
			]),
		},
		created() {
      this.loadData();
		},
		methods: {
			loadData(){
				let ens = new this.Entities("BP.WF.Auths");
				ens.Retrieve("Auther", this.webuser.No);
				console.log(ens)
			},
			GetFlowNames(auths,auth) {
				var items = this.$.grep(auths, function (item) {
					return item.AuthType == 2 && auth.TakeBackDT == item.TakeBackDT;
				});
				console.log(items)
				var flowNames = [];
				items.forEach(function (item) {
					flowNames.push(item.FlowNo+"."+item.FlowName);
				});
				return flowNames.join(",")
			},
      deleteRow(index, rows) {
          rows.splice(index, 1);
       }
		}

	}
</script>

<style>
</style>
