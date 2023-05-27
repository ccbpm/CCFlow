<template>
  <div id="Returnwork">
    <el-form
      ref="form"
      :model="form"
      label-width="180px"
      v-show="isShowForm"
      label-position="top"
    >
      <el-form-item label="选择退回到的节点和人员">
        <el-select v-model="form.selectedNodeID">
          <el-option
            v-for="item in returnData"
            :key="item.No"
            :label="item.RecName+'=>' + item.Name"
            :value="item.No + '@' + item.Rec"
          ></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="退回原因">
        <el-input type="textarea" v-model="form.doc"></el-input>
      </el-form-item>
    </el-form>
    <div class="btn-groups">

      <el-button @click="onSubmit(0)">确定退回</el-button>
      <el-button @click="onSubmit(1)" v-show="isShowBackTracking"
        >对方修改后直接发送给我</el-button
      >
      <el-button @click="onSubmit(2)" v-show="isShowKillEtcThread"
        >全部子线程退回</el-button
      >

    </div>
    <div v-show="isShowDiv" v-html="msg"></div>
  </div>
</template>

<script>
export default {
  name: "Returnwork",
  props: {
    urlParams:{type:Object,default:()=>{}}
  },
  data() {
    return {
      form: {
        selectedNodeID: "",
        doc: "",
      },
      returnData: [],
      isBack: false,
      params: {},
      isShowForm: true, //点击退回按钮直接退回时隐藏
      isShowBackTracking: false, //是否显示原路返回
      isShowKillEtcThread: false, //是否显示退回所有子线程

      isShowDiv: false,
      msg: "",
    };
  },

  created() {
    this.params = this.urlParams ||  this.$store.getters.getData;
    this.loadData();
  },
  methods: {
    loadData() {
      let handler = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
      handler.AddJson(this.params);
      //初始化数据.
      let data = handler.DoMethodReturnString("Return_Init");
      if (data.includes("info@")) {
        data = data.replace("info@", "");
        this.isShowDiv = true;
        this.isShowForm = false;
        this.msg = data;
        return;
      }
      if (data.includes("err@")) {
        data = data.replace("err@", "");
        this.isShowDiv = true;
        this.isShowForm = false;
        this.msg = data;
        // this.$message.error(data);
        return;
      }
      let node = new this.Entity("BP.WF.Node", this.params.FK_Node);
      this.isShowBackTracking = parseInt(node.IsBackTracking)==0?false:true;
      this.isShowKillEtcThread = parseInt(node.IsKillEtcThread) === 1;
      this.returnData = JSON.parse(data);
      if(this.returnData.length!=0)
       this.form.selectedNodeID = this.returnData[0].No + '@' + this.returnData[0].Rec;
    },
    onSubmit(type) {
      if (!this.form.selectedNodeID) {
        this.$message({
          message: "请选择要退回的节点",
          type: "warning",
        });
        return;
      }
      if (!this.form.doc) {
        this.$message({
          message: "请输入退回原因",
          type: "warning",
        });
        return;
      }
      this.params.ReturnToNode = this.form.selectedNodeID;
      this.params.ReturnInfo = this.form.doc;
      if(type==1)
        this.params.IsBack = 1;
      else
        this.params.IsBack = 0;
      if(type==2)
        this.params.IsKillEtcThread = 1;
      else
        this.params.IsKillEtcThread =0;
      let data = null;
      if(typeof this.params.WorkIDs === 'undefined'){
        let handler = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
        handler.AddJson(this.params);
        //调用退回方法.
        data = handler.DoMethodReturnString("DoReturnWork"); //执行方法.
      }else{
        let handler = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt_Batch");
        handler.AddJson(this.params);
        //调用退回方法.
        data = handler.DoMethodReturnString("Batch_Return"); //执行方法.
      }
      if (data.includes("err@")) {
        this.$message.error(data);
        return;
      }
      this.$message({
        message: data,
        type: "success",
      });
      this.$Bus.$emit("closeMsg", "退回");
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
.btn-groups {
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: flex-end;
}
</style>
