<template>
  <el-row class="container-row">
    <div v-for="(btn, idx) of toolBtns" :key="idx">
      <div v-if="btn.No === 'Send'">
        <div v-if="currNode && currNode.CondModel == 2" class="flex-center">
          <el-button
              size="medium"
              :key="btn.No"
              @click="btnOpen(btn)"
              :disabled="btnDisabled"
              :loading="sendLoading"
          >
            <i :class="iconMap.get(btn.No)"></i> {{ btn.Name }}
          </el-button>
          <el-select
              v-if="toNodes.length!=0"
              v-model="selectNode.No"
              id="DDL_ToNode"
              placeholder="请选择下一个处理节点"
              v-show="toNodes.length !== 0"
              style="margin: 3px 5px 0 10px"
              @change="changeToNode"
          >
            <el-option
                v-for="item in toNodes"
                :key="item.No"
                :label="item.Name"
                :value="item.No"
            >
            </el-option>
          </el-select>
        </div>
        <div v-else-if="currNode && currNode.CondModel == 3">
          <div v-for="node in toNodes" :key="node.No">
            <el-button
                size="medium"
                @click="btnOpen(btn, node, idx)"
                :disabled="btnDisabled"
            >
              <i :class="iconMap.get(node.No)"></i> {{ node.Name }}
            </el-button>
          </div>
        </div>
        <div v-else>
          <el-button
              size="medium"
              :key="btn.No"
              @click="btnOpen(btn)"
              :disabled="btnDisabled"
              :loading="sendLoading"
          >
            <i :class="iconMap.get(btn.No)"></i> {{ btn.Name }}
          </el-button>
        </div>
      </div>
      <div v-else>
        <el-button
            size="medium"
            :key="btn.No"
            @click="btnOpen(btn, null)"
            :disabled="btnDisabled"
        >
          <i :class="iconMap.get(btn.No)"></i> {{ btn.Name }}
        </el-button>
      </div>
    </div>
    <el-dialog
        :title="title"
        :visible.sync="dialogFormVisible"
        :before-close="handleClose"
        destroy-on-close
    >
      <SelectiveRecipient ref="selectiveRef" v-if="judgeOperation === 'sendAccepter'" :urlParams="params" @accepterSend="accepterSend"></SelectiveRecipient>
      <div v-else-if="judgeOperation === 'ShowMsg'" v-html="SendMsg" style="height:400px;overflow-y: auto"></div>
      <OpenDialog v-else  :judgeOperation="judgeOperation" :params="params"></OpenDialog>
    </el-dialog>
  </el-row>
</template>

<script>
import OpenDialog from "@/wf/components/OpenDialog.vue";
import {GetPara} from "@/wf/api/Gener.js";
import SelectiveRecipient from "./selectiveRecipient.vue";
export default {
  props: {
    nodeID: { type: Number, default: 0 },
    selectItems: {type:Array ,  default: () => []},
    loadData:{type:Function,default:null}
  },
  provide: function () {
    return {
      toolBarInstance: this,
    };
  },
  components: {
    OpenDialog,
    SelectiveRecipient
  },
  data() {
    const iconMap = new Map([
      ["Return", "el-icon-circle-close"],
      ["Shift", "el-icon-sort"],
      ["Track", "el-icon-connection"],
      ["Search", "el-icon-search"],
      ["Send", "el-icon-s-promotion"],
    ]);
    return {
      params: {}, //传递的参数
      webUser: {}, //用户信息
      toolBtns: [], //按钮对象的集合
      toNodes: [], //跳转到的节点
      currNode: {}, //当前节点的信息
      btnDisabled: false, //操作按钮禁用
      sendLoading: false, //发送加载操作
      selectNode: {}, //按钮旁的下拉框，发送时选择的节点
      workcheckMsg:"", //审核意见
      /***弹出窗页面的定义**/
      title: "", //弹出窗的标题
      dialogFormVisible: false, //是否显示弹出窗
      judgeOperation: "", //弹出窗的类型
      componentList: [],
      currentView: "ch",
      returnData: {}, //返回select数据
      iconMap,
      SendMsg:'',
    };
  },
  created() {
    this.webUser = this.$store.getters.getWebUser;
    this.initToolBar();
    this.params.FK_Node = this.nodeID;
    if (this.toNodes != undefined && this.toNodes.length != 0)
      this.selectNode = JSON.parse(JSON.stringify(this.toNodes[0]));
    else this.toNodes = [];
  },

  methods: {
    initToolBar: function () {
      let handler = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt_Batch");
      handler.AddPara("FK_Node",this.nodeID);
      let data = handler.DoMethodReturnString("BatchToolBar_Init");
      if (data.indexOf("err@") != -1) {
        this.$alert(data.replace("err@", ""), {
          dangerouslyUseHTMLString: true,
        });
        return;
      }
      data = JSON.parse(data);
      this.toolBtns = data['ToolBar'];
      this.toNodes = data['ToNodes'];
      if (this.toNodes != undefined)
        this.toNodes.forEach((toNode) => {
          toNode.text = toNode.Name;
        });
      this.currNode =data['WF_Node'][0];
      this.params.FK_Flow  = this.currNode.FK_Flow;
    },
    btnOpen: function (btn, toNode) {
      if(this.selectItems.length==0){
        this.$message({type:"error",message:'请选择需要批量发送的流程实例'});
        return;
      }
      if(this.selectItems.length>0){
        if(this.currNode.FWCSta === 1){
          const workids= this.selectItems.map(item=>item.WorkID);
          this.params.WorkIDs = workids.join(',');
          this.params.WorkID = workids[0];
          const batchCheckNoteModel =  GetPara(this.currNode.AtPara,"BatchCheckNoteModel") || "0";
          if(batchCheckNoteModel === "1"){
            this.params.SelectItems =encodeURIComponent(JSON.stringify(this.selectItems));
          }
        }

      }
      switch (btn.No) {
        case "Send": //发送
          this.params.ToNode = this.selectNode.No;
          if (this.currNode && this.currNode.CondModel == 3) {
            this.selectNode = JSON.parse(JSON.stringify(toNode));
            this.params.ToNode = this.selectNode.No;
          }
          this.SendFlow();
          break;
        case "DeleteFlow": //删除流程
          this.DeleteFlow();
          break;
        case "EndFlow": //结束流程
          this.StopFlow();
          break;
        case "Return": //结束流程
          this.DialogOpen(btn);
          break;
        case "WorkCheckMsg"://如何审核意见
          this.$prompt('请输入审核意见', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            inputType:'text',
            inputValue: this.workcheckMsg
          }).then(({ value }) => {
            this.workcheckMsg = value;
            this.params.CheckMsg = value;
          })
              break;
        default:
          //弹框
          this.DialogOpen(btn);
          break;
      }
    },
    changeToNode(nextNodeNo) {
      let params = this.$store.getters.getData;
      const selectedNode = this.toNodes.filter(
          (node) => node.No === nextNodeNo
      );
      if (selectedNode.length > 1 || selectedNode.length === 0) {
        this.$message.error("下级节点id错误");
        return;
      }
      this.selectNode = JSON.parse(JSON.stringify(selectedNode[0]));
      params.ToNode = this.selectNode.No;
    },
    SendFlow: function () {
      try{
        this.Send();
      }catch(e){
        this.$message.error(e.toString());
      }finally{
        this.btnDisabled = false;
        this.sendLoading = false;
      }

    },

    //弹出新页面窗体
    DialogOpen: function (btn) {
      //弹出框
      this.title = btn.Name;
      this.dialogFormVisible = true;
      this.judgeOperation = btn.No;
    },

    //删除流程
    DeleteFlow: function () {
      this.$confirm("您确定要删除改流程数据吗, 是否继续?", "", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
      }).then(() => {
        let handler = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt_Batch");
        handler.AddPara("WorkIDs", this.workids.join(","));
        let str = handler.DoMethodReturnString("Batch_Delete");
        if (typeof str ==='string' && str.includes("err@")) {
          this.$message({
            type: "error",
            message: "删除失败，请查看控制台，或者联系管理员",
          });
        } else {
          this.$message({
            type: "success",
            message: str,
          });
          if(this.loadData)
            this.loadData();
        }
      });
    },
    //结束流程
    StopFlow: function () {
      this.$confirm("您确定要结束该流程吗 ?", "", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
      }).then(() => {
        let handler = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt_Batch");
        handler.AddPara("WorkIDs", this.workids.join(","));
        let data = handler.DoMethodReturnString("Batch_StopFlow");
        if (data.indexOf("err@") != -1) {
          this.$message({
            type: "error",
            message: "流程结束失败，请查看控制台，或者联系管理员",
          });
          console.log(data);
        } else {
          this.$message({
            type: "success",
            message: data,
          });
          if(this.loadData)
            this.loadData();
        }
      });
    },
    Send: function () {
      this.btnDisabled = true;
      this.sendLoading = true;
      if(this.currNode.FWCSta === 1){
        const batchCheckNoteModel = GetPara(this.currNode.AtPara,"BatchCheckNoteModel") || "0";
        if(batchCheckNoteModel == "0"){
          this.$prompt('请输入审核意见', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            inputType:'text',
            inputValue: this.workcheckMsg
          }).then(({ value }) => {
            this.workcheckMsg = value;
            if(this.workcheckMsg!=""){
              this.params.CheckMsg = value;
              this.beforeSend();
              return;
            }
          })
          return ;
        }
        if(batchCheckNoteModel == "1"){
          let msg="";
          this.selectItems.forEach(item=>{
             if(!item.CheckMsg)
               msg+=item.WorkID+",";
          })
          if(msg != ""){
            this.$message({
              type: "error",
              message: "请检查选择的流程实例是否都填写了审核意见",
            });
            return ;
          }
          this.beforeSend();
        }
      }
    },
    beforeSend(){
      //选择节点
      if (this.toNodes.length > 0) {
        if (this.selectNode.IsSelectEmps == "1") {
          //跳到选择接收人窗口
          this.params.IsSend = false;
          this.dialogFormVisible = true;
          this.judgeOperation = "sendAccepter";
          this.title = "选择接收人";
          this.btnDisabled = false;
          this.sendLoading = false;
          return false;
        }
        if (this.selectNode.IsSelectEmps == "2") {
          this.dialogFormVisible = true;
          this.judgeOperation = "BySelfUrl";
          this.title = "选择接收人";
          this.btnDisabled = false;
          this.sendLoading = false;
          return false;
        }
        if (this.selectNode.IsSelectEmps == "3") {
          this.dialogFormVisible = true;
          this.params.IsSend = false;
          this.judgeOperation = "sendAccepterOfOrg";
          this.title = "选择接收人";
          this.btnDisabled = false;
          this.sendLoading = false;
          return false;
        }

        if (this.selectNode.IsSelectEmps == "4") {
          this.dialogFormVisible = true;
          this.params.IsSend = false;
          this.judgeOperation = "AccepterOfDept";
          this.title = "选择接收人";
          this.btnDisabled = false;
          this.sendLoading = false;
          return false;
        }
      }
      this.execSend();
    },
    execSend() {
      let handler = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt_Batch");
      handler.AddJson(this.params);
      let data = handler.DoMethodReturnString("WorkCheckModelVue_Send"); //执行保存方法.
      this.sendLoading = false;
      if (data.indexOf("err@") == 0) {
        //发送时发生错误
        let reg = new RegExp("err@", "g");
        data = data.replace(reg, "");
        this.$alert(data);
        this.btnDisabled = false;
        return false;
      }

      if (data.indexOf("TurnUrl@") == 0) {
        //发送成功时转到指定的URL
        let url = data;
        url = url.replace("TurnUrl@", "");
        this.$router.push({ name: url });
        return false;
      }
      if (data.indexOf("SelectNodeUrl@") == 0) {
        this.dialogFormVisible = true;
        this.params.IsSend = false;
        this.judgeOperation = "SelectNodeUrl";
        this.title = "选择接收人";
        this.btnDisabled = false;
        this.sendLoading = false;
        return false;
      }

      if (data.indexOf("BySelfUrl@") == 0) {
        //发送成功时转到自定义的URL
        let url = data;
        url = url.replace("BySelfUrl@", "");
        this.$router.push({ name: url });
        return false;
      }

      if (data.indexOf("url@") == 0) {
        //发送成功时转到指定的URL
        let params = data.split("&");
        params.forEach((param) => {
          if (param.indexOf("ToNode") != -1) {
            let toNodeID = param.split("=")[1];
            let params = this.$store.getters.getData;
            params.ToNode = toNodeID;
            this.$store.commit("setData", params);
          }
        });

        if (data.indexOf("AccepterOfOrg") != -1) {
          this.dialogFormVisible = true;
          this.params.IsSend = false;
          this.judgeOperation = "sendAccepterOfOrg";
          this.title = "选择接收人";
          this.btnDisabled = false;
          return false;
        }

        if (data.indexOf("AccepterOfDept") != -1) {
          this.dialogFormVisible = true;
          this.params.IsSend = false;
          this.judgeOperation = "AccepterOfDept";
          this.title = "选择接收人";
          this.btnDisabled = false;
          return false;
        }

        if (
            data.indexOf("Accepter") != 0 &&
            data.indexOf("AccepterGener") == -1
        ) {
          this.dialogFormVisible = true;
          this.params.IsSend = false;
          this.judgeOperation = "sendAccepter";
          this.title = "选择接收人";
          this.btnDisabled = false;
          return false;
        }
        return false;
      }
      this.OptSuc(data);
    },
    //发送 退回 移交等执行成功后转到  指定页面
    OptSuc(msg) {
      this.dialogFormVisible = false;
      msg = msg.replace("@查看<img src='/WF/Img/Btn/PrintWorkRpt.gif' >", "");

      msg = msg.replace(/@/g, "<br/>").replace(/null/g, "");
      this.SendMsg = msg;
      this.judgeOperation='ShowMsg';
      this.dialogFormVisible = true;
      /*this.$alert(msg, "发送成功", {
        dangerouslyUseHTMLString: true,
      });*/
      /*this.$router.push({
        name: "todolist",
      });*/
      this.loadData();
      return;
    },
    accepterSend(){
      this.dialogFormVisible = false;
      if(['SelectNodeUrl','sendAccepterOfOrg','AccepterOfDept','sendAccepter'].includes(this.judgeOperation) ){
        const todoEmps=this.$refs.selectiveRef.GetEmps();
        if(todoEmps.length==0){
          this.$message({
            type: "error",
            message: "请选择下一个节点接收的人员",
          });

          return;
        }
        this.params.ToEmps = todoEmps.join(",");
        this.judgeOperation = "";
        try{
          this.execSend()
        }catch(e){
          this.$message({
            type: "error",
            message: e,
          });
        }

      }
    },
    //关闭
    handleClose() {
      this.dialogFormVisible = false;
      this.judgeOperation = "";
    },
  },

  mounted() {
    this.$Bus.$off("closeMsg");
    this.$Bus.$on("closeMsg", (item) => {
      this.dialogFormVisible = false;
      if (item == "退回" || item == "回滚") {
        this.loadData();
      }
    });
  },
};
</script>

<style lang="less" scoped>
/deep/ .el-tabs__header{
  margin: 0;
}


/deep/ .el-input__inner {
  height: 36.5px;
  line-height: 36.5px;
}
.container-row {
  display: flex;
  align-items: center;
}

/deep/ .el-button {
  margin-left: 10px;
}

.flex-center {
  display: flex;
  align-items: center;
  box-sizing: border-box;
}
</style>