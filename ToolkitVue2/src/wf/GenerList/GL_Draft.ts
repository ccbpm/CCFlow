import { GenerListPageShowModel, PageBaseGenerList } from "./PageBaseGenerList";
import { HttpHandler } from "../api/Gener";
import { GPNReturnObj, GPNReturnType } from "./GPNReturnObj";
export class GL_Draft extends PageBaseGenerList {
  constructor() {
    super("GL_Draft");
    this.PageTitle = "草稿";
  }
  //重写的构造方法.
  async Init() {
    this.DTFieldOfSearch = "RDT"; // 按照日期范围查询的字段.
    this.DTFieldOfLabel = "保存日期"; //日期字段名.
    this.LinkField = "Title";
    this.Icon = "";
    this.PageSize = 0; // 分页的页面行数, 0不分页.
    this.HisGLShowModel = GenerListPageShowModel.Table;
    //this.GroupFields = 'FlowName';
    //this.GroupFieldDefault = 'FlowName';

    // 定义列，这些列用于显示.
    this.Columns = [
      { Key: "WorkID", Name: "工作ID", IsShow: false },
      { Key: "Title", Name: "标题", IsShow: true },
      { Key: "RDT", Name: "编写时间", IsShow: true },
      { Key: "FlowName", Name: "流程", IsShow: true },
    ];

    //获得数据源.
    const handler = new HttpHandler("BP.WF.HttpHandler.WF");
    const res = handler.DoMethodReturnJSON("Draft_Init");
    if (res) {
      this.Data = res;
    }
  }

  //打开页面.
  LinkFieldClick(object: Record<string, any>) {
    let url = "/#/WF/MyFlow?WorkID=" + object.WorkID;
    const keys = Object.keys(object);
    for (const key of keys) {
      if (key === "WorkID") continue;
      url += `&${key}=${object[key]}`;
    }

    return new GPNReturnObj(GPNReturnType.OpenUrlByDrawer90, url);
  }

  BtnClick(btnName: string, object: Record<string, any>) {
    if (btnName === object.WorkID) throw new Error("Method not implemented.");
  }
}
