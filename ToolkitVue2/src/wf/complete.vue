<template>
  <div id="Complete">
    <el-form
      :inline="true"
      ref="ruleForm"
      :model="formInline"
      :rules="rules"
      class="demo-form-inline"
    >
      <el-form-item label="关键字" prop="keyWord">
        <el-input v-model="formInline.keyWord" placeholder="关键字"></el-input>
      </el-form-item>
      <el-form-item label="发起日期" prop="RageDate">
        <el-date-picker
          v-model="formInline.RageDate"
          type="daterange"
          range-separator="至"
          start-placeholder="开始日期"
          end-placeholder="结束日期"
          
        >
        </el-date-picker>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="onSubmit('ruleForm')">查询</el-button>
      </el-form-item>
    </el-form>

    <div style="display: flex">
      <div style="width: 25%">
        <el-menu default-active="2" class="el-menu-vertical-demo">
          <el-menu-item
            v-for="item in dataTree"
            :key="item.label"
            @click="handleNodeClick(item.WorkID)"
          >
            <i class="el-icon-paperclip"></i>
            <span slot="title">{{ item.label }}</span>
          </el-menu-item>
        </el-menu>
      </div>
      <div style="padding: 0 10px">
        <el-table
         :data="tableData.slice((currentPage - 1) * pageSize, currentPage * pageSize)"
          height="75vh"
          style="flex: 1"
          row-key="WorkID"
          default-expand-all
          :row-class-name="tableRowClassName"
          :tree-props="{ children: 'children', hasChildren: 'hasChildren' }"
        >
          <el-table-column prop="Title" label="标题" fixed min-width="250">
            <template slot-scope="scope">
              <span v-if="scope.row.type != null">{{ scope.row.Title }}</span>
              <span v-else>
                <el-link
                  :underline="false"
                  @click="sikpMyflow(scope.row)"
                  type="primary"
                >
                  {{ scope.row.Title }}
                </el-link>
              </span>
            </template>
          </el-table-column>
          <el-table-column
            prop="StarterName"
            label="发起人/部门"
            width="140"
            align="center"
          >
            <template slot-scope="scope">
              <span>{{ scope.row.StarterName }};{{ scope.row.DeptName }}</span>
            </template>
          </el-table-column>

          <el-table-column prop="FlowName" label="流程类型" width="170" align="center">
          </el-table-column>
          <el-table-column
            prop="RDT"
            label="发起时间"
            width="160"
            align="center"
          >
          </el-table-column>
          <el-table-column
            prop="SendDT"
            label="完成时间"
            width="160"
            align="center"
          >
          </el-table-column>
          <el-table-column label="操作" width="100" align="center">
            <template slot-scope="scope">
              <el-button
                v-show="scope.row.type == null"
                @click="DialogOpen(scope.row)"
                type="primary"
                size="mini"
                plain
                >轨迹</el-button
              >
            </template>
          </el-table-column>
        </el-table>
        <el-pagination
        background
        @size-change="handleSizeChange"
        @current-change="handleCurrentChange"
        :current-page="currentPage"
        :page-sizes="[5, 10, 15, 20]"
        :page-size="pageSize"
        layout="total,sizes, prev, pager, next"
        :total="total"
        v-show="total > 0"
      ></el-pagination>
      </div>
    </div>
    <el-dialog
      :title="title"
      :visible.sync="dialogFormVisible"
      :before-close="handleClose"
    >
    </el-dialog>
  </div>
</template>

<script>
import {openMyView} from "./api/Dev2Interface"; // 轨迹
export default {
  name: "complete",
  data() {
    return {
      title: "已完成",
      data: {},
      dialogFormVisible: false, //弹出框显示
      titile: "", //标题
      formInline: {
        keyWord: "",
        RageDate: "",
      },
      rules: {},
      tableData: [],
      dataTree: [],
      total: 0, // 总数
      currentPage: 1, // 当前页
      pageSize: 15, //一页显示的行数
      selevalue: "",
    };
  },

  beforeCreate() {},

  created() {
    this.loadData();
	this.timeDefault();
  },

  methods: {
	// 默认时间
	timeDefault () {
		let date = new Date()
		// 通过时间戳计算
		let defalutStartTime = date.getTime() - 30 * 24 * 3600 * 1000 // 转化为时间戳
		let defalutEndTime = date.getTime()
		let startDateNs = new Date(defalutStartTime) 
		let endDateNs = new Date(defalutEndTime)
		// 月，日 不够10补0
		defalutStartTime = startDateNs.getFullYear() + '-' + ((startDateNs.getMonth() + 1) >= 10 ? (startDateNs.getMonth() + 1) : '0' + (startDateNs.getMonth() + 1)) + '-' + (startDateNs.getDate() >= 10 ? startDateNs.getDate() : '0' + startDateNs.getDate())
		defalutEndTime = endDateNs.getFullYear() + '-' + ((endDateNs.getMonth() + 1) >= 10 ? (endDateNs.getMonth() + 1) : '0' + (endDateNs.getMonth() + 1)) + '-' + (endDateNs.getDate() >= 10 ? endDateNs.getDate() : '0' + endDateNs.getDate())
		this.formInline.RageDate = [defalutStartTime, defalutEndTime];
	},
    tableRowClassName({ row }) {
      if (row.type === null) return "";

      if (row.type === 1) {
        return "success-row";
      }
      return "";
    },
    // 获取数据
    loadData() {
      let handler = new this.HttpHandler("BP.WF.HttpHandler.WF");
      let data = handler.DoMethodReturnString("Complete_Init");
      if (data.indexOf("err@") != -1) {
        this.$message.error(data);
        console.log(data);
        return;
      }
      this.data = JSON.parse(data);
      this.filterData();
    },

    // 分页
    paging() {
      /*this.total = this.tableData.length;
      let start = (this.currentPage - 1) * this.pageSize;
      let end = this.currentPage * this.pageSize;
      let arr = this.tableData.slice(start, end);*/
      //this.filterData(arr);
    },
    // 查询
    // onSubmit() {},
    // handleSizeChange(val) {
    //   this.pageSize = val;
    //   this.currentPage = 1;
    //   console.log(`每页 ${val} 条`);
    // },
    // handleCurrentChange(val) {
    //   this.currentPage = val;
    //   console.log(`当前页: ${val}`);
    // },

    // 过滤数据
    filterData() {
      this.tableData = [];
      var flowNos = "";
      this.data.forEach((item) => {
        this.tableData.push(item);
        if (flowNos.indexOf(item.FK_Flow + ",") == -1) {
          var childrenWork = this.GetChildren(item.FK_Flow);
          this.total = childrenWork.length;
          this.dataTree.push({
            label: item.FlowName + " (" + childrenWork.length + ")",
            name: item.FlowName,
            WorkID: parseInt(item.FK_Flow),
            childrenWork: childrenWork,
            count: childrenWork.length,
          });

          flowNos += item.FK_Flow + ",";
          
        }
      });
    },

    GetChildren(flowNo) {
      var data = [];
      this.data.forEach((item) => {
        if (item.FK_Flow == flowNo) {
          if (item.TodoEmps.indexOf(",") != -1)
            item.TodoEmps = item.TodoEmps.substring(
              item.TodoEmps.indexOf(",") + 1
            );
          data.push(item);
        }
      });
      return data;
    },
    handleNodeClick(data) {
      this.tableData = this.GetChildren(data);
      this.total = this.tableData.length;
      //this.GetChildren(data.name,data.childrenWork)
      //this.paging();
    },
    // 查询
    onSubmit(formName) {
      this.$refs[formName].validate((valid) => {
        if (valid) {
          console.log(this.tableData);
          const Data = this.tableData.filter((data) => {
            if (
              data.Title.toLowerCase().includes(
                this.formInline.keyWord.toLowerCase()
              )
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
          console.log(this.formInline);
        } else {
          console.log("error submit!!");
          return false;
        }
      });
    },
    handleSizeChange(val) {
      this.pageSize = val;
      this.currentPage = 1;
      this.paging();
      console.log(`每页 ${val} 条`);
    },
    handleCurrentChange(val) {
      this.currentPage = val;
      this.paging();
      console.log(`当前页: ${val}`);
    },
    //跳转到jflow页面
    sikpMyflow(work) {
      let params = {};
      params.WorkID = work.WorkID;
      params.FK_Flow = work.FK_Flow;
      params.FK_Node = work.FK_Node;
      params.FID = work.FID;
      params.FromPage = "Complete";
      openMyView(params,this);
    },
    DialogOpen: function (rowData) {
      this.$store.commit("setData", rowData);
      // 如果是轨迹图 先请求接口
      this.title = "查看轨迹图";
      this.dialogFormVisible = true;
      this.judgeOperation = "track";
    },
    //关闭
    handleClose() {
      this.dialogFormVisible = false;
      this.judgeOperation = "";
    },
  },

  //监听
  computed: {},

  //监听后执行动作
  watch: {},
};
</script>

<style lang="less" scoped>
.mb-10 {
  margin-bottom: 10px;
}
</style>
