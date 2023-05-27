<template>
  <div id="Shift">

    <el-form inline style="margin-top:10px">
      <el-form-item label>
        <el-autocomplete
            class="inline-input"
            v-model="selectEmpNos"
            :fetch-suggestions="querySearch"
            placeholder="查张三你可以输入张,zs,或者zhangs,zhangsan"
            :trigger-on-focus="false"
            @select="handleSelect"
            style="width: 400px"
        ></el-autocomplete>
      </el-form-item>
      <el-form-item>
        <el-popover placement="bottom" v-model="visible" trigger="click">
          <SelectEmps v-on:selectEmpsValue="selectEmpsValue" :parentComponent="parentComponent"
                      :isSingle="true"></SelectEmps>
          <el-button type="primary" slot="reference" :disabled="disabled">选择移交人</el-button>
        </el-popover>
      </el-form-item>
    </el-form>
    选择的移交人:{{ selectEmpNames }}
    <el-form>
      移交原因:
      <el-input type="textarea" v-model="doc"></el-input>
      <el-button type="primary" @click="DoShift" :disabled="disabled" style="text-align:right">确定移交</el-button>
    </el-form>
  </div>
</template>

<script>
import SelectEmps from "../components/SelectEmps";
export default {
  name: "Shift",
  data() {
    return {
      params: {},
      restaurants: [],
      selectEmpNos: '',
      selectEmpNames: "",
      visible: false,//popover是否显示
      disabled: false,//按钮是否是禁用
      parentComponent: "Shift",
      doc: ""
    }
  },
  inject: {
    toolBarInstance: {}
  },
  created() {
    this.params = this.$store.getters.getData;
  },
  methods: {
    //自动完成
    querySearch(queryString, cb) {
      if (this.selectEmpNos == "") {
        this.restaurants = [];
        return "";
      }
      this.restaurants = [];
      var hand = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
      hand.AddJson(this.params);
      hand.AddPara("TB_Emps", this.selectEmpNos);
      var data = hand.DoMethodReturnString("HuiQian_SelectEmps");
      if (data.indexOf('err@') == 0) {
        this.$message.error(data);
        console.log(data);
        return;
      }
      data = JSON.parse(data);
      data.forEach(item => {
        this.restaurants.push({
          "No": item.No,
          "value": item.Name
        });
      })
      cb(this.restaurants);
    },
    handleSelect(item) {
      this.selectEmpNos = item.No;
      this.selectEmpNames = item.value;
    },
    selectEmpsValue(emps) {
      if (emps != null && emps !== "") {
        this.selectEmpNos = emps.split(",")[0];
        this.selectEmpNames = emps.split(",")[1];
      }
      this.visible = false;
    },

    DoShift() {
      var handler = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
      handler.AddJson(this.params);
      handler.AddPara("ToEmp", this.selectEmpNos);
      handler.AddPara("Message", this.doc);
      var data = handler.DoMethodReturnString("Shift_Save");
      data = data.replace("@null", "");
      data = data.replace(/@/g, '');
      if (data.includes('@err')) {
        this.$message.error(data.replace('@err',''));
        return;
      }
      this.$message.success(data)
      this.toolBarInstance.dialogFormVisible = false
      this.$router.push({
        name: "todolist"
      });
    }
  },
  components: {
    SelectEmps
  },
}
</script>

<style scoped>

</style>
