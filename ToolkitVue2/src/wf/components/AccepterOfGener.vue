<template>
  <div id="AccepterOfGener">
    <div v-if="isShowMsg">
      <el-form :inline="true" :model="formInline" class="demo-form-inline">
        <el-form-item label>
          <el-select
            style="width: 300px"
            v-model="value"
            filterable
            placeholder="查张三你可以输入张,zs,或者zhangs,zhangsan"
            :filter-method="changeSelect"
            @change="blurSelect"
          >
            <el-option
              v-for="item in options"
              :key="item.No"
              :label="item.Name"
              :value="item.No"
            ></el-option>
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-popover
            placement="bottom"
            trigger="manual"
            :value="personSelectVisible"
          >
            <el-tree
              :data="data"
              show-checkbox
              node-key="No"
              :props="defaultProps"
              @node-click="handleNodeClick"
              @check-change="handleCheckChange"
              :default-expanded-keys="defaultExpandNodes"
              :check-strictly="true"
              ref="tree"
            ></el-tree>
            <div class="btn-group">
              <el-button type="secondary" size="small" @click="superior"
                >上一级</el-button
              >
              <el-button type="primary" size="small" @click="determine"
                >添加</el-button
              >
            </div>
            <!--            <SelectEmps v-on:selectEmpsValue="handleCheckChange" :parentComponent="parentComponent"-->
            <!--                        :isSingle="true"></SelectEmps>-->
            <el-button
              type="primary"
              slot="reference"
              @click="personSelectVisible = !personSelectVisible"
              >选择人员</el-button
            >
          </el-popover>
          <el-button  type="primary" @click="onSubmit" :loading="sending">
            执行发送
          </el-button>
        </el-form-item>
      </el-form>
      <el-table
        :data="tableData"
        style="width: 100%; height: 300px; overflow-y: auto"
      >
        <el-table-column prop="EmpName" label="姓名"></el-table-column>
        <el-table-column prop="DeptName" label="所在部门"></el-table-column>
        <el-table-column label="操作" width="150">
          <template slot-scope="scope">
            <el-button @click="Up(scope.row.MyPK)" type="text" size="small"
              >上移</el-button
            >
            <el-button @click="Down(scope.row.MyPK)" type="text" size="small"
              >下移</el-button
            >
            <el-button
              @click="DeleteIt(scope.row.MyPK)"
              type="text"
              size="small"
              >删除</el-button
            >
          </template>
        </el-table-column>
      </el-table>
    </div>
    <div v-else>
      <p v-for="(item, index) in msg" :key="index" v-html="item"></p>
      <div style="text-align: center">
        <el-button type="primary" @click="closeDialog">确定</el-button>
      </div>
    </div>
  </div>
</template>

<script>
import { WebUser, Entity } from "@/wf/api/Gener.js";

export default {
  name: "AccepterOfGener",
  props: {
    urlParams:{type:Object,default:()=>{}}
  },
  inject: {
    toolBarInstance: {},
  },
  data() {
    return {
      blogTitle: "",
      formInline: {
        user: "",
        region: "",
      },
      tableData: [],
      data: [],
      defaultProps: {
        children: "children",
        label: "Name",
      },
      params: {},
      FK_Dept: "", //部门级别
      ParentNo: "", //控制上一级
      No: "", // 控制下一级
      judgeDept: "productionTree", //判断渲染上级还是下级
      childrenJSON: [],
      checkAll: [],
      options: [],
      value: "",
      isShowMsg: true,
      msg: [],
      // 人员选择器可见性
      personSelectVisible: false,
      sending: false,
      isShowSend:true,
    };
  },

  beforeCreate() {},

  created() {
    this.isShowMsg = true;
    this.params = this.urlParams;
    this.isShowSend = this.params.IsSend==undefined?true : this.params.IsSend;
    if (this.params.FK_Dept) {
      this.FK_Dept = this.params.FK_Dept;
    } else {
      const webUser = new WebUser();
      this.FK_Dept = webUser.FK_Dept;
    }
    this.loadData();
    this.loadTable();
  },

  methods: {
    // 获取数据
    loadData() {
      console.log("this.FK_Dept---", this.FK_Dept);
      const handler = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
      handler.AddPara("FK_Dept", this.FK_Dept);
      let data = handler.DoMethodReturnString("SelectEmps_Init");
      if (data.indexOf("err@") == 0) {
        this.$message.error(data);
        console.log(data);
        return;
      }
      data = JSON.parse(data);
      data.Emps.forEach(emp=>{
         emp.No=emp.No.replace("Emp_","");
      });
      this[this.judgeDept](data);
    },

    closeDialog() {
      this.$router.replace({
        path: '/start'
      })
      this.toolBarInstance.dialogFormVisible = false;
    },

    // 获取表格数据
    loadTable() {
      const handler = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
      handler.AddJson(this.params);
      const data = handler.DoMethodReturnString("AccepterOfGener_Init");
      if (data.indexOf("err@") == 0) {
        alert(data);
        window.parent.close();
        return;
      }
      this.tableData = JSON.parse(data);
    },

    //生产部门树
    productionTree(jsonTree) {
      this.data = [];
      jsonTree.Depts.forEach((item) => {
        item.children = [];
        item.disabled = true;
        item.loadChild = false;
        jsonTree.Emps.forEach((items) => {
          if (item.No == items.FK_Dept) {
            item.children.push(items);
          }
        });
        if (this.FK_Dept == item.No) {
          this.data.push(item);
          this.ParentNo = item.ParentNo;
        } else {
          this.data[0].children.push(item);
        }
      });
      this.defaultExpandNodes = [this.FK_Dept];
    },

    //下级树渲染
    lowerLevel(data) {
      data.Emps.forEach((item) => {
        this.childrenJSON.children.push(item);
      });
      this.defaultExpandNodes = [this.FK_Dept];
    },

    //上级
    superior() {
      this.judgeDept = "productionTree";
      if (this.ParentNo == 0) {
        this.$message({
          message: "已经是第一级机构了",
          type: "warning",
        });
        return;
      }
      this.FK_Dept = this.ParentNo;
      this.loadData();
    },

    // tree候选
    handleCheckChange(data, checked, indeterminate) {
      console.log(data, checked, indeterminate);
    },

    //tree 确定
    determine() {
      let res = this.$refs.tree.getCheckedNodes();
      this.checkAll = [];
      res.forEach((item) => {
        if (item.FK_Dept) {
          this.checkAll.push(item.No);
        }
      });
      this.addEmpsExit(this.checkAll.join(","));
      this.personSelectVisible = false;
    },

    addEmpsExit(emps) {
      const handler = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
      handler.AddJson(this.params); //参数ToNode, WorkID,FK_Node,FK_Flow
      handler.AddPara("AddEmps", emps); //要增加的人员，多个可以用逗号分开.
      const data = handler.DoMethodReturnString("AccepterOfGener_AddEmps");
      if (data.indexOf("err@") == 0) {
        this.$message.error(data);
        return;
      }
      if (data.indexOf("info@") == 0) {
        this.$message(data.replace("info@", ""));
        return;
      }
      this.loadTable();
    },
    GetEmps(){
      let arr = [];
      this.tableData.forEach(item => {
        arr.push(item.FK_Emp);
      });
      return arr;
    },
    onSubmit() {
      this.sending = true;
      if(this.isShowSend == false){
        this.$emit("accepterSend");
        return;
      }

      setTimeout(() => {
        const httphandler = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
        httphandler.AddJson(this.params); //FK_Flow,FK_Node,WorkID,FID,ToNode 等参数.
        const data = httphandler.DoMethodReturnString("AccepterOfGener_Send");
        if (data.indexOf("err@") == 0) {
          //如果发送失败.
          this.$message.error(data);
          this.sending = false;
          return;
        }

        this.msg = data.split("<br>");
        this.isShowMsg = false;
        this.toolBarInstance.title = "发送成功";
        this.sending = false;
      },200);
    },
    handleNodeClick(data) {
      if (!Number(data.No)) {
        this.$message({
          message: "请选择其他部门",
          type: "warning",
        });
        return;
      }
      if (this.data[0].No != data.No && !data.loadChild) {
        this.childrenJSON = data;
        this.judgeDept = "lowerLevel";
        this.FK_Dept = data.No;
        this.loadData();
        data.loadChild = true;
      }
    },

    // 上移动
    Up(mypk) {
      const en = new Entity("BP.WF.Template.SelectAccper", mypk);
      en.DoMethodReturnString("DoUp");
      setTimeout(() => {
        this.loadTable();
      }, 100);
    },

    // 下移动
    Down(mypk) {
      const en = new Entity("BP.WF.Template.SelectAccper", mypk);
      en.DoMethodReturnString("DoDown");
      setTimeout(() => {
        this.loadTable();
      }, 100);
    },

    // 删除
    async DeleteIt(mypk) {
      const en = new Entity("BP.WF.Template.SelectAccper", mypk);
      en.Delete();
      await this.$nextTick();
      this.loadTable();
    },

    // select
    changeSelect(inputKey) {
      this.params.TB_Emps = inputKey;
      const handler = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
      handler.AddJson(this.params); //参数ToNode, WorkID,FK_Node,FK_Flow
      const data = handler.DoMethodReturnString("AccepterOfGener_SelectEmps");
      this.options = JSON.parse(data);
    },

    //blurSelect
    blurSelect() {
      this.addEmpsExit(this.value);
    },
  },

  //监听
  computed: {},

  components: {},

  //监听后执行动作
  watch: {},
};
</script>

<style lang="less" scoped>
/deep/ .el-dialog__body {
  padding: 10px 20px;
}

/deep/ .el-button {
  margin: 0 5px;
}
</style>

<style scoped>
#AccepterOfGener /deep/ .el-tree {
  margin-bottom: 20px;
}

#AccepterOfGener /deep/ .el-form-item {
  margin-bottom: 0;
}
</style>
