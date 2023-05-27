<template id="QingJia">
  <el-container style="background-color: #f0f3f4">
    <el-header style="background-color: #ffff">
      <!--- 工具栏组件请见文档: https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=6455415&doc_id=31094 -->
      <tool-bar :pageFrom="pageFrom"  :urlParams="params" :Save="Save" ref="toolbar"></tool-bar>
    </el-header>
    <el-main justify="center" align="middle" class="el-main">
      <el-card style="width:1200px;margin:20px auto">
        <el-form ref="form" :model="form" label-width="100px">
          <el-form-item label="请假人">
            <el-input v-model="form.ShenQingRen" :disabled="true"></el-input>
          </el-form-item>
          <el-form-item label="请假人部门">
            <el-input v-model="form.ShenQingRenBuMen" :disabled="true"></el-input>
          </el-form-item>
          <el-form-item label="请假时间">
            <el-col :span="11">
              <el-date-picker type="date" placeholder="选择日期" v-model="form.QingJiaRiQiCong" style="width: 100%;"></el-date-picker>
            </el-col>
            <el-col class="line" :span="2">-</el-col>
            <el-col :span="11">
              <el-date-picker placeholder="选择日期" v-model="form.Dao" style="width: 100%;"></el-date-picker>
            </el-col>
          </el-form-item>
          <el-form-item label="请假类型">
            <el-radio-group v-model="form.QJLX" style="text-align:left" @change="changeSwitch">
              <el-radio :label="0">事假</el-radio>
              <el-radio :label="1">病假</el-radio>
              <el-radio :label="2">其他</el-radio>
            </el-radio-group>
          </el-form-item>
          <el-form-item label="请假原因">
            <el-input type="textarea" v-model="form.QingJiaYuanYin"></el-input>
          </el-form-item>
        </el-form>
        <!--- 工具栏组件请见文档: https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=6455415&doc_id=31094 -->
        <div  v-if="isHaveWorkCheck" style="text-align: left; font-size: 18px;box-sizing: border-box;padding-left: 12px;margin-bottom: 10px">审核信息</div>
        <WorkCheck v-if="isHaveWorkCheck" :isShowTagName="true"  :node="node" :isReadonly=isReadonly ref="WorkCheckRef"></WorkCheck>
      </el-card>

    </el-main>
  </el-container>
</template>

<script>
import WorkCheck from "@/wf/workopt/workcheck.vue";
import ToolBar from "@/wf/components/tool-bar";
export default {
  name: "F024SDKToolbarFrm",
  components: {
    ToolBar,
    WorkCheck,
  },
  data(){
    return{
      params: {}, //参数
      webUser: {}, //人员信息
      pageFrom:"MyFlow", //MyFlow 流程运行 MyView 流程查询 MyCC 抄送
      isHaveWorkCheck:false,//是否有审核组件
      isShowTagName:true, // 审核组件是否显示节点名称
      isReadonly:0, //是否只读状态
      node:{} , //节点信息
      enName:'',
      form: { //表单内容
        ShenQingRen: '',
        ShenQingRenBuMen: '',
        QingJiaRiQiCong: '',
        Dao: '',
        ShiFouBaoHanJieJiaRi: false,
        QJLX: 0,
        QingJiaYuanYin: '',
      }
    }
  },
  created(){
    this.params = this.$route.query; //请求的参数
    const isRreadonly = parseInt(this.params.IsReadonly || '0');
    const ccSta = parseInt(this.params.CCSta || '0');
    if(isRreadonly === 1 ){
      if(ccSta === 1)
        this.pageFrom = 'MyCC';
      else
        this.pageFrom = 'MyView';
    }
    this.webUser = this.$store.getters.getWebUser; //当前登录人员的信息，需要使用用户的信息时可以使用该方式获取
    this.enName = 'ND'+parseInt(this.params.FK_Flow )+'Rpt';
    this.initData();
  },
  methods:{
    /**
     * 初始化数据，
     * 需要的参数 WorkID,FK_Flow,FK_Node ,如果是分合流子线程 需要增加FID，如果是父子流程需要增加PWorkID
     */
    initData(){
      //获取表单的数据
      const en = new this.Entity(this.enName,this.params.WorkID);
      this.form.ShenQingRen = en.ShenQingRen;
      this.form.ShenQingRenBuMen = en.ShenQingRenBuMen;
      this.form.ShiFouBaoHanJieJiaRi = parseInt(en.ShiFouBaoHanJieJiaRi) == 1?true:false;
      this.form.QJLX = parseInt(en.QJLX || 0);
      //判断审核组件是否启用
      if(this.params.FK_Node && this.params.FK_Node!=0)
        this.node = new this.Entity("BP.WF.Node",this.params.FK_Node);
      if(this.node && this.node.FWCSta!==0)
        this.isHaveWorkCheck=true;
    },
    /**
     * Save,toolBar页面需要调用的页面保存方法
     * @param generWorkFlow (WorkID 实例的ID,WFState 状态 0=空白，1=草稿，2=运行中，3=已完成 5退回,TodoEmps 待办处理人,FlowEmps 参与人,FK_Node 停留节点,NodeName节点名字,Starter发起人ID,StarterName 发起人名称,FK_Dept 发起人部门,DeptName 部门名称)
     * @constructor
     */
    Save(generWorkFlow){
      //第1步: 检查页面逻辑是否正确，必填项、正则表达式等等.
      /*if (1==2)
      {
        this.$message.error("必填项不正确.");
        return false;
      }*/

      // 第2步: 增加系统字段到自己的JSON中
      for(let key in generWorkFlow){
        this.form[key] = generWorkFlow[key];
      }
      try{
        // 第3步: 执行入库保存
        //调用自己的保存逻辑.
        //保存表单
        //  const en = new this.Entity(this.enName,this.params.WorkID);
        //  en.CopyJSON(this.form);
        //  en.ShiFouBaoHanJieJiaRi = this.form.ShiFouBaoHanJieJiaRi==true?1:0;
        //  en.Update();

        //4.返回值
        //4.1 如果在流程中设计了节点表单，并且需要把当前表单中的字段保存到节点表单中，则返回
        //return this.form;

        //4.2 如果在流程中没有设计节点表单,但是需要把当前表单中的字段作为方向条件使用，则返回
        //return  '@QJLX='+this.form.QJLX+'@QJTS='+this.form.QJTS;

        //4.3 排除上面两种情况，则直接返回true即可
        return true;
        // eslint-disable-next-line no-unreachable
      }catch(e){
        this.$message.error(e.toString());
        return false; //保存失败.
      }
    },
    changeSwitch() {
      this.$forceUpdate()
    },
  },

}
</script>

<style lang="less" scoped>
/*
	找到html标签、body标签，和挂载的标签
	都给他们统一设置样式
*/
html,
body,
#app,
.el-container {
  /*设置内部填充为0，几个布局元素之间没有间距*/
  padding: 0px;
  /*外部间距也是如此设置*/
  margin: 0px;
  /*统一设置高度为100%*/
  height: 100%;
}

.el-header {
  width: 100%;
  height: 56px !important;
  line-height: 56px;
  padding: 0 20px 0 44px;
  background: #ffffff;
  box-shadow: 0px 4px 6px 0px rgba(231, 237, 249, 1);
  box-sizing: border-box;
  top: 0;
}

:root > .el-divider--horizontal {
  margin: 12px 0px;
}

.el-main {
  padding: 0 !important;
}
/deep/.el-form-item__content{
  text-align:left !important;
}
.el-switch {
  z-index: 9999;
}
</style>
