<template>
  <div id="Todolist">
    <el-form :inline="true" ref="ruleForm" :model="formInline" class="demo-form-inline">
      <el-form-item label="关键字" prop="keyWord">
        <el-input v-model="formInline.keyWord" placeholder="关键字"></el-input>
      </el-form-item>
      <el-form-item label="发起日期" prop="RageDate">
        <el-date-picker v-model="formInline.RageDate" type="daterange" range-separator="至" start-placeholder="开始日期"
          end-placeholder="结束日期" :default-time="['00:00:00', '23:59:59']">
        </el-date-picker>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="onSubmit('ruleForm')">查询</el-button>
      </el-form-item>
    </el-form>

    <el-table :data="tableData" height="75vh" ref="topicTable" row-key="WorkID" default-expand-all>
      <el-table-column label="#" width="50" fixed type="index">
      </el-table-column>
      <el-table-column prop="Title" label="标题" fixed min-width="240" max-width="350">
        <template slot-scope="scope">
          <span>
            <el-link :underline="false" @click="sikpMyflow(scope.row)">
              <i :class="
                scope.row.IsRead === 0
                  ? 'fas fa-envelope-open iconColor-w'
                  : 'fas fa-envelope iconColor'
              "></i><span :class="scope.row.IsRead === 0 ? 'pl-5' : 'cellColor pl-5'">{{ scope.row.Title }}</span>
            </el-link>
          </span>
        </template>
      </el-table-column>
      <el-table-column prop="NodeName" label="节点/流程" width="280">
        <template slot-scope="scope">
          <span>{{ scope.row.FlowName }}/{{ scope.row.NodeName }}</span>
        </template>
      </el-table-column>
      <el-table-column prop="StarterName" label="发起人" width="100">
      </el-table-column>
      <!-- <el-table-column prop="RDT" label="发起日期" width="150">
			</el-table-column> -->
      <el-table-column prop="ADT" label="送达日期" width="150">
        <template slot-scope="scope">
          <span>{{ scope.row.ADT.substr(0, 16) }}</span>
        </template>
      </el-table-column>
      <el-table-column prop="SDT" label="期限" width="150">
        <template slot-scope="scope">
          <span>{{ scope.row.SDT.substr(0, 16) }}</span>
        </template>
      </el-table-column>
      <el-table-column prop="ZT" label="状态" width="100">
        <template slot-scope="scope">
          <span v-if="scope.row.ZT == 0" style="color: green">正常</span>
          <span v-else-if="scope.row.ZT == 1" style="color: #ffac38">警告</span>
          <span v-else-if="scope.row.ZT == 2" style="color: red">逾期</span>
        </template>
      </el-table-column>
      <el-table-column prop="WFState" label="类型" width="100">
        <template slot-scope="scope">
          <span v-if="scope.row.WFState == 1">草稿</span>
          <span v-else-if="scope.row.WFState == 2">运行中</span>
          <span v-else-if="scope.row.WFState == 3">已完成</span>
          <span v-else-if="scope.row.WFState == 4">挂起</span>
          <span v-else-if="scope.row.WFState == 5" style="color: red">退回</span>
          <span v-else-if="scope.row.WFState == 6">转发</span>
          <span v-else-if="scope.row.WFState == 7">删除</span>
          <span v-else-if="scope.row.WFState == 8">加签</span>
          <span v-else-if="scope.row.WFState == 11">加签回复</span>
          <span v-else-if="scope.row.WFState == null"></span>
          <span v-else>其他</span>
        </template>
      </el-table-column>
      <el-table-column prop="PRI" label="优先级" width="80">
        <template slot-scope="scope">
          <img v-if="scope.row.PRI == 0" title="低" src="../assets/PRI/0.png" />
          <img v-if="scope.row.PRI == 1" title="中" src="../assets/PRI/1.png" />
          <img v-if="scope.row.PRI == 2" title="高" src="../assets/PRI/2.png" />
        </template>
      </el-table-column>
    </el-table>
    <div style="margin: 10px 0">
      <span style="
                  font-weight: 500;
                  font-size: 14px;
                  margin-right: 10px;
                  color: #606266;
                ">合计： {{ count }}条</span>
    </div>
  </div>
</template>

<script>
import { openMyFlow } from "./api/Dev2Interface";

export default {
  name: "Todolist",
  components: {},
  data() {
    return {
      title: "待办",
      tableData: [],
      parseData: [],
      count: 0,
      activeNames: [],
      formInline: {
        keyWord: "",
        RageDate: "",
      },
    };
  },

  beforeCreate() { },
  created() {
    this.loadData();
  },
  methods: {
    // 获取数据
    loadData() {
      let handler = new this.HttpHandler("BP.WF.HttpHandler.WF");
      handler.AddUrlData();
      handler.AddPara("Domain", process.env.VUE_APP_DOMAIN);
      let data = handler.DoMethodReturnString("Todolist_Init");
      if (data.indexOf("err@") != -1) {
        this.$message.error(data);
        console.log(data);
        return;
      }
      this.tableData = JSON.parse(data);
      this.parseData = this.tableData;
      this.count = this.tableData.length;
    },

    // 过滤数据
    filterData(data) {
      this.count = data.length;
      let flowNos = "";
      data.forEach((item) => {
        if (flowNos.includes(item.FK_Flow + ",")) {
          this.tableData.push({
            Title: item.FlowName,
            WorkID: parseInt(item.FK_Flow),
            type: 1, //流程
            children: this.GetChildren(item.FK_Flow, data),
          });
          flowNos += item.FK_Flow + ",";
        }
      });
    },
    GetChildren(flowNo, gwfs) {
      let data = [];
      gwfs.forEach((item) => {
        let paras = item.AtPara;
        if (paras == null) paras = "";
        //判断期限是否少于三天，加警告颜色
        const date = new Date();
        let edt = item.SDT.replace(/\-/g, "/");//eslint-disable-line
        edt = new Date(Date.parse(edt.replace(/-/g, "/")));
        const passTime = edt.getTime() - date.getTime();
        //判断流程是否逾期
        if (
          date.getTime() > edt.getTime() &&
          item.WFState == 2 &&
          item.FK_Node != parseInt(item.FK_Flow) + "01" &&
          item.RDT != item.SDT &&
          paras.indexOf("&IsCC=1") == -1
        ) {
          item.ZT = 2;
        } else {
          if (passTime >= 0 && passTime < 2 * 24 * 3600 * 1000) {
            item.ZT = 1;
          } else {
            item.ZT = 0;
          }
        }
        if (item.FK_Flow == flowNo) {
          data.push(item);
        }
      });
      return data;
    },

    //跳转myflow
    sikpMyflow(work) {
      let params = {
        IsRead: work.IsRead,
        FK_Flow: work.FK_Flow,
        FK_Node: work.FK_Node,
        FID: work.FID,
        WorkID: work.WorkID,
        PWorkID: work.PWorkID,
        Paras: work.AtPara ? work.AtPara : "",
      };
      openMyFlow(params, this);
    },
    onSubmit(formName) {
      this.$refs[formName].validate((valid) => {
        if (valid) {
          const Data = this.parseData.filter((data) => {
            if (
              data.Title.toLowerCase().includes(
                this.formInline.keyWord.toLowerCase())
              || data.FlowName.toLowerCase().includes(
                this.formInline.keyWord.toLowerCase())
              || data.NodeName.toLowerCase().includes(
                this.formInline.keyWord.toLowerCase())
            ) {
              return data;
            }
          });
          this.tableData = Data;
          if (this.formInline.RageDate !== "") {
            let time = this.formInline.RageDate;
            let startTime = this.moment(time[0]).unix() * 1000;
            let endTime = this.moment(time[1]).unix() * 1000;
            const NewData = Data.filter((data) => {
              let nowTime = this.moment(data.RDT).unix() * 1000;
              if (nowTime >= startTime && nowTime <= endTime) {
                return data;
              }
            });
            this.tableData = NewData;
          }
        } else {
          return false;
        }
      });
    },
  },

  //监听
  computed: {},

  //监听后执行动作
  watch: {
    formInline: {
      handler(oldval, newval) {
        if (
          newval.keyWord == "" ||
          newval.RageDate[0] == "" ||
          newval.RageDate[1] == ""
        ) {
          this.tableData = this.parseData;
        }
      },
      deep: true, //true 深度监听
    },
  },
};
</script>
<style>
.el-table .success-row {
  background: #f5f7fa;
  padding: 5px 0px;
}

.el-table td,
.el-table th {
  padding: 8px 0px !important;
}
</style>
<style lang="less" scoped>
.ml-5 {
  padding-left: 5px;
}

.mr-20 {
  padding-right: 20px;
}

.iconColor {
  color: #f56c6c;
}

.iconColor-w {
  color: #909399;
}

.cellfontColor {
  color: #545454;
  font-weight: 600;
  display: inline-table;
  width: 90%;
}

.cellColor {
  color: #545454;
}
</style>
