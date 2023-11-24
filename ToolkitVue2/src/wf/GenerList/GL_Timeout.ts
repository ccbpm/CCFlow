import { HttpHandler } from "../api/Gener";
import { GPNReturnObj, GPNReturnType } from "./GPNReturnObj";
import { GenerListPageShowModel, PageBaseGenerList } from "./PageBaseGenerList";
import { DataType } from "./utils/DataType";
export class GL_Timeout extends PageBaseGenerList {
  constructor() {
    super("GL_Timeout");
    this.PageTitle = "超时工作";
  }
  //重写的构造方法.
  async Init() {
    this.DTFieldOfSearch = "RDT"; // 按照日期范围查询的字段，为空就不需要日期段查询.
    this.DTFieldOfLabel = "发起日期"; //日期字段名.
    this.LinkField = "Title";
    this.GroupFields = "NodeName,FlowName,StarterName"; //分组字段.
    this.GroupFieldDefault = "FlowName"; //分组字段.
    this.LabFields = "WFState";
    this.Icon = "";
    this.BtnOfToolbar = "批处理,打印";
    this.PageSize = 30; // 分页的页面行数, 0不分页.
    this.HisGLShowModel = GenerListPageShowModel.Table; //表格展示.

    // 定义列，这些列用于显示. IsRead,PRI是特殊字段.
    this.Columns = [
      { Key: "WorkID", Name: "工作ID", IsShow: false, DataType: 2 },
      { Key: "Title", Name: "标题", IsShow: true, DataType: 1, width: 350 },
      {
        Key: "StarterName",
        Name: "发起人",
        IsShow: true,
        DataType: 1,
        width: 66,
      },
      {
        Key: "NodeName",
        Name: "停留节点",
        IsShow: true,
        DataType: 1,
        width: 150,
      },
      { Key: "FlowName", Name: "流程", IsShow: true, DataType: 1, width: 150 },
      {
        Key: "RDT",
        Name: "发起时间",
        IsShow: true,
        DataType: 7,
        width: 144,
        RefFunc: "FirendlyDT",
      },
      {
        Key: "PRI",
        Name: "PRI",
        IsShow: true,
        DataType: 1,
        width: 50,
        RefFunc: "PRI",
      },
      { Key: "SDT", Name: "应完成时间", IsShow: true, DataType: 7, width: 144 },
      {
        Key: "IsRead",
        Name: "是否读取",
        IsShow: false,
        DataType: 2,
        RefFunc: "IsRead",
      },
      { Key: "WFState", Name: "标签", IsShow: true, DataType: 2 },
    ];

    //获得数据源.
    const handler = new HttpHandler("BP.WF.HttpHandler.WF");
    const data: any = handler.DoMethodReturnJSON("Timeout_Init");

    //处理数据,增加标签.
    data.forEach((en) => {
      // 判断是否是逾期.  SDT 应完成日期与当前日期对比.
      let lab = "";
      if (en.SDT >= DataType.CurrentDateTime) {
        lab = "@逾期=red";
      }
      if (en.WFState == 1) {
        en.WFState = "@草稿=orange";
      }
      if (en.WFState == 2) {
        en.WFState = "@进行中=green";
      }

      if (en.WFState == 5) {
        en.WFState = lab + "@退回=red";
      }
      if (en.WFState == 3) {
        en.WFState = lab + "@完成=green";
      }
      if (en.WFState == 6) {
        en.WFState = lab + "@移交=red";
      }
      if (en.WFState == 8) {
        en.WFState = lab + "@加签=red";
      }
    });

    this.Data = data;
    console.log("data", this.Data);
  }

  //打开页面.
  LinkFieldClick(object: Record<string, any>) {
    const workID = object.WorkID;
    let url = "/#/WF/MyFlow?WorkID=" + workID;
    const keys = Object.keys(object);
    for (const key of keys) {
      if (key === "WorkID") continue;
      url += `&${key}=${object[key]}`;
    }
    return new GPNReturnObj(GPNReturnType.OpenUrlByDrawer75, url);
  }

  BtnClick(btnName: string, object: Record<string, any>) {
    if (btnName == "批处理") {
      const url = "/src/WF/Batch.vue?xxx=" + object.WorkID;
      window.location.href = url;
      return;
    }
    return;
  }
}
