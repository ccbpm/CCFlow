<template>
  <div v-if="!hasError" id="selectiveRecipient">
    <AccepterOfGener v-if="judge === 'AccepterOfGener'" ref="accepterOfGenerRef" :urlParams="params" @accepterSend="accepterSend"></AccepterOfGener>
    <Accepter v-if="judge === 'Accepter'" :data="data"  ref="accepterRef" :urlParams="params" @accepterSend="accepterSend"></Accepter>
  </div>
  <div v-else class="error-dialog">
    <p><i class="el-icon-warning-outline"></i></p>
    {{errorMsg}}
  </div>
</template>

<script>
import AccepterOfGener from './AccepterOfGener.vue'
import Accepter from './Accepter'

export default {
  name: "selectiveRecipient",
  props: {
    urlParams:{type:Object,default:()=>{}}
  },
  data() {
    return {
      params: {},
      judge: 'AccepterOfGener',
      data: {},
      hasError: false,
      errorMsg: ''
    };
  },

  created() {
    this.params = this.urlParams;
    this.loadData();
  },

  methods: {
    // 获取数据
    loadData() {
      const handler = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
      handler.AddJson(this.params); //这里是获得Url的参数.
      const data = handler.DoMethodReturnString("Accepter_Init");
      if (typeof data === 'string' && data.includes("err@")) {
        this.hasError = true
        this.errorMsg = data.replace("err@",'')
        return;
      }
      if (data.indexOf("AccepterOfGener") != -1) {
        this.judge = "AccepterOfGener";
      } else {
        this.judge = "Accepter";
        this.data = JSON.parse(data);
      }

    },
    GetEmps(){
      if(this.judge === 'AccepterOfGener')
        return this.$refs.accepterOfGenerRef.GetEmps();
      if(this.judge === 'Accepter')
        return this.$refs.accepterRef.GetEmps();
    },
    onSubmit(val) {
      console.log("val", val);
    },
    handleNodeClick() {
    },
    accepterSend(){
      this.$emit("accepterSend");
    }
  },

  //监听
  computed: {},

  components: {
    AccepterOfGener,
    Accepter
  },

  //监听后执行动作
  watch: {}
};
</script>

<style lang="less" scoped>
/deep/ .el-dialog__body {
  padding: 10px 20px;
}
.error-dialog{
  font-size: 22px;
  color: #ff3838;
  display: flex;
  align-items: center;
  flex-direction: column;
  p {
    i {
      font-size: 40px;
    }
  }
}
</style>
