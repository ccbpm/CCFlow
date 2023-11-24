import { GenerListPageShowModel, PageBaseGenerList } from "./PageBaseGenerList";
import { HttpHandler } from "../api/Gener";
import { DataType } from "./utils/DataType";
import { GPNReturnObj, GPNReturnType } from "./GPNReturnObj";

export class GL_Complete extends PageBaseGenerList {
  constructor() {
    super("GL_Complete");
    this.PageTitle = "已完成";
  }
  //重写的构造方法.
  async Init() {
    this.DTFieldOfSearch = "RDT"; // 按照日期范围查询的字段，为空就不需要日期段查询.
    this.DTFieldOfLabel = "发起日期"; //日期字段名.
    this.LinkField = "Title"; //焦点字段.
    this.GroupFields = "NodeName,FlowName,StarterName"; //分组字段.
    this.GroupFieldDefault = "FlowName"; //默认分组字段.
    this.LabFields = "WFState";
    this.Icon = "";
    // this.BtnOfToolbar = '批处理,导出,打印';
    this.PageSize = 15; // 分页的页面行数, 0不分页.
    this.HisGLShowModel = GenerListPageShowModel.Table;

    //定义列,这些列用于显示, IsRead, PRI是特殊字段.
    this.Columns = [
      {
        Key: "WorkID",
        Name: "工作ID",
        IsShow: false,
        IsShowMobile: false,
        DataType: 2,
      },
      {
        Key: "Title",
        Name: "标题",
        IsShow: true,
        IsShowMobile: true,
        DataType: 1,
        width: 350,
      },
      {
        Key: "StarterName",
        Name: "发起人",
        IsShow: true,
        IsShowMobile: true,
        DataType: 1,
        width: 66,
      },
      {
        Key: "RDT",
        Name: "发起日期",
        IsShow: true,
        IsShowMobile: true,
        DataType: DataType.AppDateTime,
        width: 160,
      },
      {
        Key: "NodeID",
        Name: "节点ID",
        IsShow: false,
        IsShowMobile: false,
        DataType: 1,
        width: 150,
      },
      {
        Key: "NodeName",
        Name: "停留节点",
        IsShow: true,
        IsShowMobile: false,
        DataType: 1,
        width: 150,
      },
      {
        Key: "FlowName",
        Name: "流程",
        IsShow: true,
        IsShowMobile: false,
        DataType: 1,
        width: 150,
      },
      //{ Key: 'RDT', Name: '发起', IsShow: true, IsShowMobile: false, DataType: 7, width: 160 },
      { Key: "Sender", Name: "发送人", IsShow: true, DataType: 1, width: 121 },
      {
        Key: "PRI",
        Name: "PRI",
        IsShow: true,
        IsShowMobile: false,
        DataType: 1,
        width: 50,
        RefFunc: "PRI",
      },
      {
        Key: "SDT",
        Name: "应完成日期",
        IsShow: true,
        IsShowMobile: true,
        DataType: DataType.AppDateTime,
        width: 100,
      },
      {
        Key: "ADT",
        Name: "接受时间",
        IsShow: true,
        IsShowMobile: true,
        DataType: DataType.AppDateTime,
        width: 90,
        RefFunc: "FirendlyDT",
      },
    ];

    //获得数据源.
    const handler = new HttpHandler("BP.WF.HttpHandler.WF");
    handler.AddPara("Tel", "123");
    const data: any = handler.DoMethodReturnJSON("Complete_Init");

    //处理数据,增加标签.
    data.forEach((en) => {
      // 判断是否是逾期.  SDT 应完成日期与当前日期对比.
      let lab = "";
      if (en.SDT >= DataType.CurrentDateTime) lab = "@逾期=red"; //@lyc
      if (en.WFState == 1) en.WFState = "@草稿=orange";
      if (en.WFState == 2) en.WFState = "@进行中=green";
      if (en.WFState == 5) en.WFState = lab + "@退回=red";
      if (en.WFState == 3) en.WFState = lab + "@完成=green";
      if (en.WFState == 6) en.WFState = lab + "@移交=red";
      if (en.WFState == 8) en.WFState = lab + "@加签=red";
      if (en.PRI == 0)
        en.PRI =
          '<img src="resource/WF/Img/PRI/0.png" style="display:inline"/>';
      if (en.PRI == 1)
        en.PRI =
          '<img src="resource/WF/Img/PRI/1.png" style="display:inline"/>';
      if (en.PRI == 2)
        en.PRI =
          '<img src="resource/WF/Img/PRI/2.png" style="display:inline"/>';
      en.ADT = this.FirendlyDT(en.ADT);
    });

    //设置数据源.
    this.Data = data;
  }

  //打开页面.
  LinkFieldClick(object: Record<string, any>) {
    const workID = object.WorkID;
    let url = "/#/WF/MyView?WorkID=" + workID;
    const keys = Object.keys(object);
    const useKeys = [
      "WorkID",
      "FK_Flow",
      "FlowNo",
      "FK_Node",
      "FID",
      "PWorkID",
    ];
    for (const key of keys) {
      if (key === "WorkID") continue;
      if (useKeys.includes(key)) url += `&${key}=${object[key]}`;
    }
    //   window.open(url); //打开页面。
    return new GPNReturnObj(GPNReturnType.OpenUrlByDrawer75, url);
  }

  BtnClick(btnName: string, record: Record<string, any>) {
    console.log(btnName);
    console.log(record);
  }
}
