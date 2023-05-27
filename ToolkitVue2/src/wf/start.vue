<template>
  <div id="Start">
    <el-table
        :data="dataList"
        height="75vh"
        ref="topicTable"
        row-key="WorkID"
        default-expand-all
    >
      <el-table-column label="#" width="50" fixed type="index"/>
      <el-table-column prop="FK_FlowSortText" label="流程类别" />
      <el-table-column prop="No" label="流程编号"/>
      <el-table-column prop="Name" label="流程名称"/>
      <el-table-column
          label="操作">
        <template slot-scope="scope">
          <el-button @click="StartFlow(scope.row)" type="text" size="small">发起流程</el-button>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>

<script>
import {domain} from "./api/config.js";
import {openMyFlow} from "./api/Dev2Interface";
export default {
  name: "Start",
  data() {
    return {
      title: "流程发起",
      dataList: [],
    };
  },
  created() {
    this.loadData();
  },

  methods: {
    // 获取数据
    loadData() {
      let handler = new this.HttpHandler("BP.WF.HttpHandler.WF");
      handler.AddUrlData();
      handler.AddPara("Domain",domain);
      const data = handler.DoMethodReturnString("Start_Init");
      if(typeof data === 'string' && data.includes('err@')){
        this.$message.error(data);
        this.dataList = [];
        return;
      }
      const statFlows = JSON.parse(data).Start;
      this.dataList = statFlows.filter(item=>item.Domain===domain)
    },
    //发起流程
    StartFlow(item){
      let params={};
      params.FK_Flow = item.No;
      openMyFlow(params,this);
    },
  },
};
</script>

<style lang="less" scoped>
.el-row {
  margin-bottom: 20px;
  &:last-child {
    margin-bottom: 0;
  }
}
.el-col {
  border-radius: 4px;
}
/deep/ .el-card {
  margin-bottom: 20px;
}
/deep/ .el-card__body {
  padding: 10px;
}
/deep/ .el-collapse-item__header {
  height: auto;
  line-height: 1;
  border: none;
  font-size: 1.5rem;
  font-weight: 700;
}
/deep/ .el-collapse {
  border: none;
}
/deep/ .el-collapse-item__wrap {
  border: none;
}
.menuBox {
  border: none !important;
}
/deep/ .el-menu-item {
  height: 40px;
  line-height: 40px;
  padding-left: 5px !important;
}
/deep/ .el-icon-view {
  cursor: pointer;
}
.rowMargin {
  margin: 15px 0;
}
</style>