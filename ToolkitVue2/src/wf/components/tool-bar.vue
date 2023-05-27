<template>

  <el-row class="container-row">
    <div v-for="(btn, idx) of toolBtns" :key="idx">
      <div v-if="btn.No === 'Send'">
        <div v-if="currNode.CondModel == 2" class="flex-center">
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
        <div v-else-if="currNode.CondModel == 3">
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
      <div v-else-if="btn.No === 'Save'">
        <el-button
            size="medium"
            :key="btn.No"
            @click="btnOpen(btn, null, idx)"
            :disabled="btnDisabled"
            :loading="saveLoading"
        ><i :class="iconMap.get(btn.No)"></i> {{ btn.Name }}
        </el-button>
      </div>
      <div v-else>
        <el-button
            size="medium"
            :key="btn.No"
            @click="btnOpen(btn, null, idx)"
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
      <OpenDialog :judgeOperation="judgeOperation" :params="params"></OpenDialog>
    </el-dialog>
  </el-row>
</template>
<script>
import {
  beforeSave,
  beforeReturn,
  beforeDelete,
} from "@/datauser/jslidata/MyPublic.js";
import OpenDialog from "./OpenDialog.vue";
import {openMyFlow} from "@/wf/api/Dev2Interface.js";

export default {
  props: {
    pageFrom: { type: String, default: "MyFlow" },
    urlParams:{type: Object,default:()=>{}},
    Save:{type:Function,default:null}
  },
  provide: function () {
    return {
      toolBarInstance: this,
    };
  },
  components: {
    OpenDialog,
  },
  data() {
    const iconMap = new Map([
      ["Return", "el-icon-circle-close"],
      ["Shift", "el-icon-sort"],
      ["Track", "el-icon-connection"],
      ["Search", "el-icon-search"],
      ["Focus", "el-icon-aim"],
      ["Send", "el-icon-s-promotion"],
      ["Save", "el-icon-document-checked"],
    ]);
    return {
      params: {}, //传递的参数
      webUser: {}, //用户信息
      toolBtns: [], //按钮对象的集合
      toNodes: [], //跳转到的节点
      currNode: {}, //当前节点的信息
      btnDisabled: false, //操作按钮禁用
      saveLoading: false, //保存加载操作
      sendLoading: false, //发送加载操作
      selectNode: {}, //按钮旁的下拉框，发送时选择的节点

      /***弹出窗页面的定义**/
      title: "", //弹出窗的标题
      dialogFormVisible: false, //是否显示弹出窗
      judgeOperation: "", //弹出窗的类型
      componentList: [],
      currentView: "ch",
      returnData: {}, //返回select数据
      iconMap,
    };
  },
  created() {
    //初始化按钮
    this.params = this.urlParams;
    this.webUser = this.$store.getters.getWebUser;
    this.initToolBar();

    if (this.toNodes != undefined && this.toNodes.length != 0){
      this.selectNode = JSON.parse(JSON.stringify(this.toNodes[0]));
      this.params.ToNode = this.selectNode.No;
    }
    else this.toNodes = [];
  },
  watch: {

  },
  methods: {
    initToolBar: function () {
      let handlerName = "";
      switch (this.pageFrom) {
        case "MyFlow":
          handlerName = "BP.WF.HttpHandler.WF_MyFlow";
          break;
        case "MyView":
          handlerName = "BP.WF.HttpHandler.WF_MyView";
          break;
        case "MyCC":
          handlerName = "BP.WF.HttpHandler.WF_MyCC";
          break;
        default:
          handlerName = "BP.WF.HttpHandler.WF_MyView";
      }
      let handler = new this.HttpHandler(handlerName);
      handler.AddJson(this.params);

      let data = handler.DoMethodReturnString("InitToolBar");
      if (data.indexOf("err@") != -1) {
        this.$alert(data.replace("err@", ""), {
          dangerouslyUseHTMLString: true,
        });
        return;
      }
      data = JSON.parse(data);
      this.toolBtns = data.ToolBar==undefined?data:data.ToolBar;
      this.toNodes = data['ToNodes'];
      if (this.toNodes != undefined)
        this.toNodes.forEach((toNode) => {
          toNode.text = toNode.Name;
        });
      this.currNode =data.WF_Node!=undefined? data.WF_Node[0]:null;
    },
    btnOpen: function (btn, toNode, idx) {
      switch (btn.No) {
        case "Send": //发送
          if (this.currNode['CondModel'] === 3) {
            this.selectNode = JSON.parse(JSON.stringify(toNode));
            this.params.ToNode = this.selectNode.No;
            this.$store.commit("setData", this.params);
          }
          this.SendFlow();
          break;
        case "SendHuiQian": //会签确定
          this.SendFlow();
          break;
        case "Save": //保存，需要调用父组件的方法
          this.SaveFlow(false);
          break;
        case "Help": //帮助提示
          this.HelpAlert(btn.Role);
          break;
        case "DeleteFlow": //删除流程
          this.DeleteFlow();
          break;
        case "EndFlow": //结束流程
          this.StopFlow();
          break;
        case "ReSet": //重置数据
          this.ReSet();
          break;
        case "Focus": //关注
          this.Focus(btn.Name, idx);
          break;
        case "Confirm": //确认
          this.Confirm(btn.Name, idx);
          break;
        case "Press": //催办
          this.Press();
          break;
        case "UnSend": //撤销
              this.UnSend();
              break;
        default:
          //弹框
          if (btn.No == "Return") {
            if (
                typeof beforeReturn != "undefined" &&
                beforeReturn instanceof Function
            )
              if (beforeReturn() == false) return false;
            if(this.Save)
              this.Save(false);
          }

          this.DialogOpen(btn);
          break;
      }
    },
    changeToNode(nextNodeNo) {
      const selectedNode = this.toNodes.filter(
          (node) => node.No === nextNodeNo
      );
      if (selectedNode.length > 1 || selectedNode.length === 0) {
        this.$message.error("下级节点id错误");
        return;
      }
      this.selectNode = JSON.parse(JSON.stringify(selectedNode[0]));
      this.params.ToNode = this.selectNode.No;
      this.$store.commit("setData", this.params);
    },
    SendFlow: function () {
      //发送
      try{
        this.Send();
      }catch(e){
        this.$message({
          type: "error",
          message: e.toString(),
        });
      }finally {
        this.btnDisabled = false;
        this.sendLoading = false;
      }

    },
    SaveFlow: function (isSend) {
      //保存
      try{
        if (typeof beforeSave != "undefined" && beforeSave instanceof Function)
          if (beforeSave() == false) return false;
        this.saveLoading = true;
        if ((this.currNode['FormType'] === 2 || this.currNode['FormType'] === 3) && this.Save) {
          this.SendSelfFrom(false);
          this.$message.success("保存成功");
          return;
        }
        if(this.Save)
          this.Save(isSend);
      }catch(e){
        this.$message({type:"error",message:e.toString()})
      }finally{
        this.saveLoading = false;
      }
    },
    //弹出新页面窗体
    DialogOpen: function (btn) {
      //弹出框
      // 如果是轨迹图 先请求接口
      this.title = btn.Name;
      this.dialogFormVisible = true;
      this.judgeOperation = btn.No;
    },
    //帮助提示
    HelpAlert: function (helpRole) {
      if (helpRole != 0) {
        let count = 0;
        let mypk =
            this.webUser.No + "_ND" + this.currNode.NodeID + "_HelpAlert";
        let userRegedit = new this.Entity("BP.Sys.UserRegedit");
        userRegedit.SetPKVal(mypk);
        count = userRegedit.RetrieveFromDBSources();
        if (helpRole == 2 || (count == 0 && helpRole == 3)) {
          let filename =
              "/DataUser/CCForm/HelpAlert/" + this.currNode.NodeID + ".htm";
          let htmlobj = this.$.ajax({ url: filename, async: false });
          if (htmlobj.status == 404) return;
          let str = htmlobj.responseText;
          if (str != null && str != "" && str != undefined) {
            this.$alert(str, "帮助指引", {
              dangerouslyUseHTMLString: true,
              beforeClose: (action, instance, done) => {
                if (count == 0) {
                  //保存数据
                  userRegedit.FK_Emp = this.webUser.No;
                  userRegedit.FK_MapData = "ND" + this.currNode.NodeID;
                  userRegedit.Insert();
                }
                done();
              },
            });
          }
        }
      }
    },

    //删除流程
    DeleteFlow: function () {
      if (
          typeof beforeDelete != "undefined" &&
          beforeDelete instanceof Function
      )
        if (beforeDelete() == false) return false;
      this.$confirm("您确定要删除改流程数据吗, 是否继续?", "", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
      }).then(() => {
        let handler = new this.HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
        handler.AddPara("FK_Flow", this.params.FK_Flow);
        handler.AddPara("FK_Node", this.params.FK_Node);
        handler.AddPara("WorkID", this.params.WorkID);
        let str = handler.DoMethodReturnString("DeleteFlow");
        if (str.indexOf("err@") != -1) {
          this.$message({
            type: "error",
            message: "删除失败，请查看控制台，或者联系管理员",
          });
          console.log(str);
        } else {
          this.$message({
            type: "success",
            message: str,
          });
        }
      });
    },
    //结束流程
    StopFlow: function () {
      this.$confirm("您确定要结束该流程吗 ?", "", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
      }).then(() => {
        let handler = new this.HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
        handler.AddPara("FK_Flow", this.params.FK_Flow);
        handler.AddPara("WorkID", this.params.WorkID);
        let data = handler.DoMethodReturnString("MyFlow_StopFlow");
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
        }
      });
    },
    //数据重置
    ReSet: function () {},
    //关注
    Focus: function (btnLab, idx) {
      if (btnLab == "关注") btnLab = "取消关注";
      else btnLab = "关注";
      this.toolBtns[idx].Name = btnLab;
      let handler = new this.HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
      handler.AddPara("WorkID", this.params.WorkID);
      handler.DoMethodReturnString("Focus"); //执行保存方法.
    },
    //确认
    ConfirmBtn: function (btnLab, idx) {
      if (btnLab == "确认") btnLab = "取消确认";
      btnLab = "确认";
      this.toolBtns[idx].Name = btnLab;
      let handler = new this.HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
      handler.AddPara("WorkID", this.params.WorkID);
      handler.DoMethodReturnString("Confirm");
    },
    Press: function(){
      this.$prompt('请输入催办信息', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        inputPattern: /\S/,
        inputErrorMessage: '催办原因不为空'
      }).then(({ value }) => {
        let handler = new this.HttpHandler("BP.WF.HttpHandler.WF");
        handler.AddJson(this.params);
        handler.AddPara("Msg", value);
        let data = handler.DoMethodReturnString("Runing_Press");
        if (typeof data ==='string' && data.includes("err@")) {
          this.$message({
            type: "error",
            message: data.replace('err@',''),
          });
        } else {
          this.$message({
            type: "success",
            message: data,
          });
        }
      });
    },
    UnSend:function(){
      this.$confirm("您确定要撤销本次发送吗 ?", "", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
      }).then(() => {
        let handler = new this.HttpHandler("BP.WF.HttpHandler.WF_MyView");
        handler.AddJson(this.params);
        let data = handler.DoMethodReturnString("MyView_UnSend");
        if (typeof data==='string' && data.includes("err@")) {
          this.$message({
            type: "error",
            message: data.replace('err@',''),
          });
        } else {
          //跳转到待办处理页面
          let params={};
          params.FK_Flow = this.params.FK_Flow;
          params.WorkID = this.params.WorkID;
          params.FID = this.params.FID;
          openMyFlow(params,this);
        }
      });
    },
    OpenOffice: function () {},
    Send: function () {
      this.btnDisabled = true;
      this.sendLoading = true;
      //如果时嵌入式表单、SDK表单
      if (this.currNode['FormType'] === 2 || this.currNode['FormType'] === 3) {
        if (this.SendSelfFrom(true) == false) {
          this.btnDisabled = false;
          this.sendLoading = false;
          this.$alert("嵌入式或者SDK表单模式发送时，保存数据失败");
          return;
        }
      }
      //表单方案：傻瓜表单、自由表单、开发者表单、累加表单、绑定表单库的表单（单表单)
      if (
          this.currNode['FormType'] === 0 ||
          this.currNode['FormType'] === 1 ||
          this.currNode['FormType'] === 10 ||
          this.currNode['FormType'] === 11 ||
          this.currNode['FormType'] === 12
      ) {
        if (this.NodeFormSend() == false) {
          this.btnDisabled = false;
          this.sendLoading = false;
          return;
        }
      }
      //绑定表单库的多表单
      if (this.currNode['FormType'] === 5 && this.FromTreeSend() == false) {
        this.btnDisabled = false;
        this.sendLoading = false;
        return;
      }
      //流转自定义的设置
      let btns = this.toolBtns.filter((item) => {
        return item.No == "TransferCustom";
      });
      if (btns.length > 0) {
        let ens = new this.Entities("BP.WF.TransferCustoms");
        ens.Retrieve("WorkID", this.params.WorkID, "IsEnable", 1);
        if (ens.length == 0) {
          this.$alert(
              "该节点启用了流程流转自定义，但是没有设置流程流转的方向，请点击流转自定义按钮进行设置"
          );
          return false;
        }
      }
      let isReturnNode = false;
      //选择节点
      if (this.toNodes.length > 0) {
        let gwf = new this.Entity("BP.WF.GenerWorkFlow", this.params.WorkID);
        let isLastHuiQian = true;
        //待办人数
        let todoEmps = gwf.TodoEmps;
        if (todoEmps != null && todoEmps != undefined) {
          let huiqianSta = gwf.GetPara("HuiQianTaskSta") == 1 ? true : false;
          if (
              this.currNode['TodolistModel'] === 1 &&
              huiqianSta == true &&
              todoEmps.split(";").length > 1
          )
            isLastHuiQian = false;
        }
        if (this.selectNode['IsSelected'] === 2) isReturnNode = true;
        if (this.selectNode['IsSelectEmps'] == "1" && isLastHuiQian == true) {
          //跳到选择接收人窗口
          this.SaveFlow(true);
          this.dialogFormVisible = true;
          this.judgeOperation = "sendAccepter";
          this.title = "选择接收人";
          this.btnDisabled = false;
          this.sendLoading = false;
          return false;
        }
        if (this.selectNode['IsSelectEmps'] === "2") {
          this.SaveFlow(true);
          //let url = this.selectNode.DeliveryParas;
          this.dialogFormVisible = true;
          this.judgeOperation = "BySelfUrl";
          this.title = "选择接收人";
          this.btnDisabled = false;
          this.sendLoading = false;
          return false;
        }
        if (this.selectNode['IsSelectEmps'] === "3") {
          this.SaveFlow(true);
          this.dialogFormVisible = true;
          this.judgeOperation = "sendAccepterOfOrg";
          this.title = "选择接收人";
          this.btnDisabled = false;
          this.sendLoading = false;
          return false;
        }

        if (this.selectNode['IsSelectEmps'] === "4") {
          this.SaveFlow(true);
          this.dialogFormVisible = true;
          this.judgeOperation = "AccepterOfDept";
          this.title = "选择接收人";
          this.btnDisabled = false;
          this.sendLoading = false;
          return false;
        }
      }
      this.execSend(isReturnNode);
    },
    execSend(isRetunNode) {

      let handler = new this.HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
      if (this.currNode['FormType'] != 3 && this.currNode['FormType'] != 2) {
        let mainData = this.$parent.$parent.$parent.mainData;
        for (let key in mainData) {
          let val = mainData[key];
          if (val instanceof Array) {
            handler.AddPara(key, val.join(","));
          } else handler.AddPara(key, val);
        }
      }
      handler.AddJson(this.params);
      handler.AddPara("IsReturnNode", isRetunNode);
      let data = handler.DoMethodReturnString("Send"); //执行保存方法.
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
            let params = this.params;
            params.ToNode = toNodeID;
            this.$store.commit("setData", params);
          }
        });

        if (data.indexOf("AccepterOfOrg") != -1) {
          this.dialogFormVisible = true;
          this.judgeOperation = "sendAccepterOfOrg";
          this.title = "选择接收人";
          this.btnDisabled = false;
          return false;
        }

        if (data.indexOf("AccepterOfDept") != -1) {
          this.dialogFormVisible = true;
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
      this.$alert(msg, "发送成功", {
        dangerouslyUseHTMLString: true,
      });
      this.$router.push({
        name: "todolist",
      });
      return;
    },

    /**
     * 嵌入式，SDK模式执行组件中的保存方法
     * @constructor
     */
    SendSelfFrom(isSend) {
      let val = false;
      if(this.Save){
        const gwf = new this.Entity("BP.WF.GenerWorkFlow",this.params.WorkID);
        let params = {};
        for(let key in gwf){
          if(['WorkID','WFState','TodoEmps','FlowEmps','FK_Node','NodeName','Starter','StarterName','FK_Dept','DeptName'].includes(key))
            params[key] = gwf[key];
          continue;
        }
        val = this.Save(params);
      }else{
        return true;
      }

      if (typeof val == "boolean" && val === false)
        return val;
      if(typeof val ==="string" && val !== ""){
        //就说明是传来的参数，这些参数需要存储到WF_GenerWorkFlow里面去，用于方向条件的判断。
        let handler = new this.HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
        handler.AddPara("WorkID", this.params.WorkID);
        handler.AddPara("Paras", val);
        const data = handler.DoMethodReturnString("SaveParas");
        if(typeof data === 'string' && data.includes('err@')){
          this.$message.error(data);
        }
      }
      //保存审核组件
      let workCheckRef = null;
      let refs = this.$parent.$refs;
      if(refs.toolbar == undefined)
        refs = this.$parent.$parent.$refs;
      if(refs.toolbar == undefined)
        refs = this.$parent.$parent.$parent.$refs;
      if(refs.toolbar != undefined)
        workCheckRef = refs.WorkCheckRef;
      if(workCheckRef!=null){
        const filedFlag = workCheckRef.WorkCheck_Save();
        if(filedFlag==false && isSend==true)
          return false;
      }
      if(typeof val ==="object"){
        let handler = new this.HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
        handler.AddJson(this.params);
        handler.AddJson(val);
        const data = handler.DoMethodReturnString("Save");
        if(typeof data === 'string' && data.includes('err@')){
          this.$message.error(data);
          return false;
        }
      }
      return true;
    },
    /**
     * 表单的保存
     * @constructor
     */
    NodeFormSend() {
      //审核组件，附件，校验
      let flag = this.$parent.$parent.$parent.validateForm();
      if (flag == false) {
        this.$alert("表单数据填写不完整或者");
        return false;
      }
      return true;
    },
    FromTreeSend() {
      return true;
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
      if (item == "退回") {
        this.$router.push({ name: "todolist" });
      }
    });
  },
};
</script>

<style lang="less" scoped>
/deep/ .el-dialog__body {
  padding: 10px 20px;
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
