<template>
  <div id="SelectEmps">
    <el-tree
        :props="props"
        :data="data"
        node-key="No"
        :default-expanded-keys="defaultExPandNode"
        :check-strictly="true"
        show-checkbox
        @node-click="handleNodeClick"
        @check-change="handleCheckChange"
        ref="tree"
    ></el-tree>
    <div class="btn-group">
      <el-button size="small" @click="toPrevDept">上一级</el-button>
      <el-button type="primary" size="small" @click="btnOK">确定</el-button>
    </div>
  </div>
</template>

<script>
export default {
  name: "SelectEmps",
  props: {
    parentComponent: {
      type: String
    },
    isSingle: {
      type: Boolean
    }
  },
  data() {
    return {
      props: {
        children: "children",
        label: "Name",
      },
      data: [],
      FK_Dept: "",//查询的部门
      ParentNo: "",
      judgeDept: "productionTree", //判断渲染上级还是下级
      childrenJSON: [],
      defaultExPandNode: [], //默认展开的节点
      checkEmps: [],//选择的人员
      params: {},//参数
      webUser: {},
    }
  },
  created() {
    this.params = this.$store.getters.getData;
    this.webUser = this.$store.getters.getWebUser;
    if (this.params.FK_Dept)
      this.FK_Dept = this.params.FK_Dept;
    else
      this.FK_Dept = this.webUser.FK_Dept;
    this.loadData();
  },
  methods: {
    loadData() {
      let handler = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
      handler.AddPara("FK_Dept", this.FK_Dept);
      let data = handler.DoMethodReturnString("SelectEmps_Init");
      if (data.indexOf('err@') == 0) {
        this.$message.error(data);
        console.log(data);
        return;
      }

      data = JSON.parse(data);
      data.Emps.forEach(emp=>{
        emp.No=emp.No.replace("Emp_","");
      })
      this[this.judgeDept](data);
      this.defaultExPandNode = [this.FK_Dept];
    },
    productionTree(data) {
      //分析数据
      var _this = this;
      var fkDeptObj = this.$.grep(data.Depts, function (value) {
        return value.No == _this.FK_Dept;
      });
      if (fkDeptObj != undefined && fkDeptObj.length == 1) {
        this.ParentNo = fkDeptObj[0].ParentNo;
      }

      var depts = this.$.grep(data.Depts, function (value) {
        return value.ParentNo == _this.ParentNo;
      })
      //子部门
      var childDepts = this.$.grep(data.Depts, function (value) {
        return value.ParentNo == _this.FK_Dept;
      });
      depts.forEach(item => {
        item.disabled = true;
        item.children = [];
        data.Emps.forEach(items => {
          if (item.No == items.FK_Dept) {
            item.children.push(items);
          }
        });
        childDepts.forEach(items => {
          if (item.No == _this.FK_Dept) {
            items.disabled = true;
            item.children.push(items);
          }
        });
        if (_this.FK_Dept == item.No) {
          _this.data.push(item);
          _this.ParentNo = item.ParentNo;
        } else {
          _this.data[0].children.push(item);
        }
      });
    },
    //下级树渲染
    lowerLevel(data) {
      console.log("下级树", data);

      this.$set(this.childrenJSON, 'children', []);

      data.Emps.forEach(item => {
        this.childrenJSON.children.push(item);
      });
      data.Depts.forEach(item => {
        if (item.ParentNo == this.childrenJSON.No) {
          item.disabled = true;
          this.childrenJSON.children.push(item);
        }
      });
    },
    handleNodeClick(node) {
      if (this.data[0].No != node.No) {
        this.childrenJSON = node;
        this.judgeDept = "lowerLevel";
        this.FK_Dept = node.No;
        this.loadData();
      }
    },
    btnOK() { //确定
      let res = this.$refs.tree.getCheckedNodes();
      if (res.length == 0) {
        this.$message.warning("请选择人员");
        return;
      }

      this.checkAll = [];
      if (this.parentComponent == "CC" || this.parentComponent == "Shift") {
        res.forEach(item => {
          if (item.disabled != true)
            this.checkAll.push(item.No + "," + item.Name);
        });
        this.$emit('selectEmpsValue', this.checkAll.join(";"));
      } else {
        res.forEach(item => {

          if (item.disabled != true)
            this.checkAll.push(item.No);
        });
        this.$emit('selectEmpsValue', this.checkAll.join(","));
      }

    },
    toPrevDept() {//上一级
      if (this.ParentNo == 0) {
        this.$message({
          message: "已经是第一级机构了",
          type: "warning"
        });
        return;
      }
      this.FK_Dept = this.ParentNo;
      this.data = [];
      this.loadData();
    },
    handleCheckChange(data, checked) {
      if (checked == true) {
        this.$refs.tree.setCheckedNodes([data]);
      }
    },
  }
}
</script>

<style scoped>
/deep/ .el-header {
  line-height: 26px;
}

.btn-group{
  display: flex;
  align-items: center;
  justify-content: center;
}

</style>
