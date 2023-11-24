import { PageBaseGenerList } from './PageBaseGenerList';
import { HttpHandler } from "../api/Gener";
import { GPNReturnObj, GPNReturnType } from "./GPNReturnObj";

export class GL_AuthorTodolist extends PageBaseGenerList {

  constructor() {
    super('GL_AuthorTodolist');
    this.PageTitle = '授权待办';
  }
  //重写的构造方法.
  async Init() {
    this.DTFieldOfSearch = 'RDT'; // 按照日期范围查询的字段，为空就不需要日期段查询.
    this.DTFieldOfLabel = '发起日期'; //日期字段名.
    this.LinkField = 'Title';
    this.GroupFields = 'NodeName,FlowName,StarterName'; //分组字段.
    this.GroupFieldDefault = 'FlowName'; //分组字段.
    this.LabFields = 'WFState';
    this.Icon = '';
    this.BtnOfToolbar = '批处理,打印';
    this.PageSize = 15; // 分页的页面行数, 0不分页.

    //定义列,这些列用于显示.IsRead,PRI是特殊字段.
    this.Columns = [
      { Key: 'WorkID', Name: '工作ID', IsShow: false, DataType: 2 },
      { Key: 'Title', Name: '标题', IsShow: true, DataType: 1, width: 350 },
      { Key: 'StarterName', Name: '发起人', IsShow: true, DataType: 1, width: 66 },
      { Key: 'NodeName', Name: '停留节点', IsShow: true, DataType: 1, width: 150 },
      { Key: 'FlowName', Name: '流程', IsShow: true, DataType: 1, width: 150 },
      { Key: 'RDT', Name: '发起时间', IsShow: true, DataType: 7, width: 144, RefFunc: 'FirendlyDT' },
      { Key: 'Sender', Name: '发送人', IsShow: true, DataType: 1, width: 121 },
      { Key: 'PRI', Name: 'PRI', IsShow: true, DataType: 1, width: 50, RefFunc: 'PRI' },
      { Key: 'SDT', Name: '应完成时间', IsShow: true, DataType: 7, width: 144 },
      { Key: 'ADT', Name: '接受时间', IsShow: true, DataType: 7, width: 150 },
      { Key: 'IsRead', Name: '是否读取', IsShow: false, DataType: 2, RefFunc: 'IsRead' },
      { Key: 'WFState', Name: '标签', IsShow: true, DataType: 2 },
    ];

    //获得数据源.
    const handler = new HttpHandler('BP.WF.HttpHandler.WF');
    handler.AddPara('RefNo', this.RequestVal('RefNo'));
    const data: any = handler.DoMethodReturnJSON('AuthorTodolist_Init');
    this.Data = data;
  }

  //打开页面.
  LinkFieldClick(object: Record<string, any>) {
    const workID = object.WorkID;
    let url = '/#/WF/MyFlow?WorkID=' + workID;
    const keys = Object.keys(object);
    for (const key of keys) {
      if (key === 'WorkID') continue;
      url += `&${key}=${object[key]}`;
    }
    // window.open(url); //打开页面。
    return new GPNReturnObj(GPNReturnType.OpenUrlByDrawer90, url);
  }

  BtnClick(btnName: string, object: Record<string, any>) {
    if (btnName === object.OO) return;
  }
}
