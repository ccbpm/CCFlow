import { HelpShowModel, PageBaseGenerList } from "./PageBaseGenerList";
import { HttpHandler } from "../api/Gener";
import { WebUser } from "../api/Gener";
// import { FlowExt } from "/@/WF/Admin/AttrFlow/FlowExt";
// import { getAuthCache, setAuthCache } from "/@/utils/auth";
// import { ADMIN_TOKEN_KEY, TESTER_KEY, TOKEN_KEY } from "/@/enums/cacheEnum";
// import { message } from "ant-design-vue";
// import { GetUrlToJSON } from "/@/utils/gener/StringUtils";
// import { useUserStoreWithOut } from "/@/store/modules/user";
import { GPNReturnObj, GPNReturnType } from "./GPNReturnObj";


// @zhoupeng 这个文件需要重写， 因为引用了实体部分
export class GL_FlowTester extends PageBaseGenerList {
  LinkFieldClick(object: Record<string, any>) {
    throw new Error("Method not implemented.");
  }
  BtnClick(btnName: string, record: Record<string, any>) {
    throw new Error("Method not implemented.");
  }
  constructor() {
    super("GL_FlowTester");
    this.PageTitle = "测试容器-选择发起人";
  }
  //重写的构造方法.
  async Init() {
    this.LinkField = "Name";
    this.GroupFields = "FK_DeptText"; //分组字段.
    this.GroupFieldDefault = "FK_DeptText"; //分组字段.
    //this.BtnOfToolbar = '返回流程设计器,设置流程发起人,更多发起人,检查流程';
    this.BtnOfToolbar = "设置流程发起人,更多发起人";
    this.PageSize = 0; // 分页的页面行数, 0不分页.

    //帮助文档显示.
    this.HelpDocs = this.MyHelpDocs;
    this.HelpShowModel = HelpShowModel.LeftPanel;

    //定义列.
    this.Columns = [
      { Key: "No", Name: "人员账号", IsShow: true, DataType: 2 },
      { Key: "Name", Name: "人员名称", IsShow: true, DataType: 1, width: 350 },
      {
        Key: "FK_DeptText",
        Name: "所在部门",
        IsShow: true,
        DataType: 1,
        width: 66,
      },
      //   { Key: 'DeptFullName', Name: '部门全称', IsShow: true, DataType: 1, width: 150 },
    ];

    //获得数据.
    const handler = new HttpHandler(
      "BP.WF.HttpHandler.WF_Admin_TestingContainer"
    );
    const flowNo = this.RequestVal("FlowNo");
    handler.AddPara("FK_Flow", flowNo);
    const data: any = handler.DoMethodReturnJSON("TestFlow2020_Init");
    this.Data = data;
    console.log("data", this.Data);
  }

  public readonly MyHelpDocs = `
  #### 帮助
   - 当前的列表是可以发起当前流程的人员.
   - 一个流程被哪些人能发起在开始节点的接受人规则设置.
   - 选择一个人员,就可以以他的身份登录,并进入测试容器发起流程.
   - 当您选择一个发起人发起流程后，系统就会把该人员记录下来下次登录仅仅显示当前处理人.
   #### 其它功能
   - 设置流程发起人:如果人员太多就可以设置一个指定的发起人发起该流程.
   - 更多发起人:全部可以发起流程人员.
   - 检查流程:自动修复数据表结构,检查流程设计错误.
   `;

  //打开页面.
  // async LinkFieldClick(record: Record<string, any>) {
  //   const flowNo = this.RequestVal("FlowNo");
  //   //存储Admin的Token
  //   setAuthCache(ADMIN_TOKEN_KEY, getAuthCache(TOKEN_KEY));
  //   const handler = new HttpHandler(
  //     "BP.WF.HttpHandler.WF_Admin_TestingContainer"
  //   );
  //   handler.AddPara("FK_Flow", flowNo);
  //   handler.AddPara("TesterNo", record.No);

  //   const data = await handler.DoMethodReturnString("TestFlow2020_StartIt");
  //   if (typeof data === "string" && data.includes("err@")) {
  //     message.error(data.replace("err@", ""));
  //     return;
  //   }
  //   //data转JSON
  //   const result = GetUrlToJSON(data);
  //   //获取当前测试用户的Token
  //   setAuthCache(TOKEN_KEY, result.Token);
  //   setAuthCache(TESTER_KEY, record.No);
  //   //获取当前测试用户的信息
  //   const userStore = useUserStoreWithOut();
  //   userStore.token = result.Token;
  //   await userStore.getUserInfoAction();
  //   //生成WorkID
  //   const handler1 = new HttpHandler(
  //     "BP.WF.HttpHandler.WF_Admin_TestingContainer"
  //   );
  //   handler1.AddPara("FK_Flow", flowNo);
  //   handler1.AddPara("TesterNo", result.TesterNo);
  //   const resultData = await handler1.DoMethodReturnString("Default_Init");
  //   console.log(resultData);
  //   if (typeof resultData === "string" && resultData.includes("err@")) {
  //     message.error(resultData);
  //     return;
  //   }
  //   //页面跳转到测试容器
  //   //const url = '/#/WF/TestingContainer/Default?FlowNo=' + flowNo + '&TesterNo=' + result.TesterNo;
  //   const url =
  //     "/#/WF/MyFlow?TesterNo=" +
  //     result.TesterNo +
  //     "&FlowNo=" +
  //     flowNo +
  //     "&WorkID=" +
  //     resultData +
  //     "&RoutFrom=MyFlow&t=" +
  //     Math.random();
  //   return new GPNReturnObj(GPNReturnType.GoToUrl, url);
  // }

  // /**
  //  * 按钮操作，包含工具栏、行操作 ，
  //  * @param btnName 按钮名称
  //  * @param object 行数据
  //  * @param params 组件参数
  //  * @param callback 回调函数
  //  * @constructor
  //  */
  // async BtnClick(btnName: string, record: Record<string, any>) {
  //   const flowNo = this.RequestVal("FlowNo") || "";
  //   if (btnName == "返回流程设计器") {
  //     window.location.replace(
  //       "/#/WF/Designer/EditFlow?FlowNo=" +
  //         flowNo +
  //         "&FK_Flow=" +
  //         flowNo +
  //         "&EnName=NewFlow"
  //     );
  //     return;
  //   }
  //   if (btnName === "设置流程发起人") {
  //     let msg = "请输入测试人员的帐号，多个人员用逗号分开";
  //     msg += "\t\n 比如:zhangsan,lisi";
  //     msg +=
  //       "\t\n 帐号就是登录该系统的编号，如果输入的帐号没有发起该流程的权限，系统就会提示错误。";
  //     const emps = window.prompt(msg, WebUser.No);
  //     const en = new FlowExt(flowNo);
  //     await en.RetrieveFromDBSources();
  //     en.SetValByKey("Tester", emps);
  //     await en.Update();
  //     await this.Init();

  //     //让数据重新绑定.
  //     return new GPNReturnObj(GPNReturnType.ReBind, this.Data);
  //   }
  //   if (btnName === "更多发起人") {
  //     const en = new FlowExt(flowNo);
  //     await en.RetrieveFromDBSources();
  //     en.SetValByKey("Tester", "");
  //     await en.Update();
  //     await this.Init();

  //     return new GPNReturnObj(GPNReturnType.ReBind, this.Data);
  //   }
  //   if (btnName === "检查流程") {
  //     const url = `/@/WF/Admin/FlowDesigner/components/CheckFlow.vue?FlowNo=${flowNo}`;
  //     return new GPNReturnObj(GPNReturnType.OpenUrlByDrawer, url);
  //   }
  //   console.log(record);
  //   return;
  // }
}
