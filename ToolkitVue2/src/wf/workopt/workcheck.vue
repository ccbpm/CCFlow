<template>
  <div id="WorkCheck">
    <template v-for="track in tracks">
      <template v-if="isShowNodeName == false">
        <table
          :key="track.WorkID"
          style="border: 1px solid #dcdfe6; width: 100%"
        >
          <tr>
            <td style="width: 30%">
              <div v-if="track.IsDoc == false">{{ track.Msg }}</div>
              <div v-else style="margin: 10px 30px 5px 10px">
                <el-input
                  type="textarea"
                  :rows="3"
                  placeholder="请输入内容"
                  v-model="textarea"
                >
                </el-input>
              </div>
            </td>
            <td style="text-align: right">
              <div style="margin: 0px 30px 5px 0px">{{ track.RDT }}</div>
            </td>
          </tr>
        </table>
      </template>
      <template v-else>
        <table
          :key="track.WorkID"
          style="border: 1px solid #dcdfe6; width: 100%"
        >
          <tr>
            <td rowspan="2" style="border-right: 1px solid #dcdfe6; width: 30%">
              {{ track.NodeName }}
            </td>
            <td>
              <div
                v-if="track.IsDoc == false"
                style="margin: 10px 30px 5px 10px"
              >
                {{ track.Msg }}
              </div>
              <div v-else style="margin: 10px 30px 5px 10px">
                <el-input
                  type="textarea"
                  :rows="3"
                  placeholder="请输入内容"
                  v-model="WorkCheck_Doc"
                >
                </el-input>
              </div>
            </td>
          </tr>
          <tr>
            <td style="text-align: right">
              <div style="margin: 0px 30px 5px 0px">
                {{ track.EmpFromT }} {{ track.RDT }}
              </div>
            </td>
          </tr>
        </table>
      </template>
    </template>
  </div>
</template>

<script>
export default {
  name: "WorkCheck",
  props: {
    node: {},
    isReadonly: {
      type: Number,
      default: 0,
    },
    isShowNodeName: {
      type: Boolean,
      default: true,
    },
    nide: {
      type: Object,
    },
  },
  data() {
    return {
      FWCVer: 0, //审核组件的版本号
      frmWorkCheck: {}, // 当前节点的审核信息
      tracks: [], //审核信息轨迹
      SignType: [], //审核签名
      WorkCheck_Doc: "",
      isDoc: false,
    };
  },
  created() {
    this.FWCVer = this.node.FWCVer;
    //初始化审核信息，获取列表
    this.WorkCheck_Init();
    //处理显示的审核信息
    this.WorkCheck_Press();
  },

  methods: {
    WorkCheck_Init() {
      console.log("WorkCheck_Init", this.$route.query);
      //获取审核组件的基本信息
      var handler = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
      handler.AddJson(this.$route.query);
      var data;
      if (this.FWCVer == 0)
        data = handler.DoMethodReturnString("WorkCheck_Init");
      else data = handler.DoMethodReturnString("WorkCheck_Init2019");
      if (data.indexOf("err@") != -1) {
        this.$message(data);
        return;
      }
      data = JSON.parse(data);
      this.frmWorkCheck = data.WF_FrmWorkCheck[0];
      this.tracks = data.Tracks;
      //签名的人员 No,SignType 列, SignType=0 不签名, 1=图片签名, 2=电子签名。
      this.SignType = data.SignType;
    },
    WorkCheck_Press() {
      var _this = this;
      this.tracks.forEach((track) => {
        //节点名称
        var nodeName = track.NodeName;
        nodeName = nodeName.replace(
          "(会签)",
          "<br>(<font color=Gray>会签</font>)"
        );
        track.NodeName = nodeName;
        //是否可以审批
        if (track.IsDoc == true && _this.isReadonly == 0) {
          track.IsDoc = true;
          _this.WorkCheck_Doc = track.Msg;
          _this.isDoc = true;
        } else track.IsDoc = false;

        //处理审核意见
        var returnMsg = track.ActionType == 2 ? "退回原因：" : "";
        if (this.FWCVer == 1) {
          var val = track.Msg.split("WorkCheck@");
          if (val.length == 2) track.Msg = val[1];
        }
        track.Msg = returnMsg + track.Msg;

        //是否显示立场

        //显示审核人信息
        if (track.RDT == undefined || track.RDT == null || track.RDT == "") {
          var dt = new Date();
          track.RDT =
            dt.getFullYear() + "-" + (dt.getMonth() + 1) + "-" + dt.getDate();
        } else track.RDT = track.RDT.substring(0, 16);
      });
    },
    WorkCheck_Save() {
      //是否需要填写审核意见
      if (this.isDoc == false) return true;
      if (this.WorkCheck_Doc == null || this.WorkCheck_Doc == "") {
        this.$message.error("审核意见不能为空");
        return false;
      }
      //保存审核意见
      let handler = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
      handler.AddJson(this.$route.query);
      handler.AddPara("Doc", this.WorkCheck_Doc);
      let data = handler.DoMethodReturnString("WorkCheck_Save");
      if (data.indexOf("err@") != -1) {
        this.$message.error(data);
        return false;
      }
      return true;
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
</style>