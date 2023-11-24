import { GenerListPageShowModel, PageBaseGenerList } from './PageBaseGenerList';
import { HttpHandler } from "../api/Gener";
import { GPNReturnObj, GPNReturnType } from "./GPNReturnObj";

export class GL_StartEaryer extends PageBaseGenerList {
  constructor() {
    super('GL_StartEaryer');
    this.PageTitle = '最近发起';
  }
  //重写的构造方法.
  async Init() {
    this.DTFieldOfSearch = 'RDT'; // 按照日期范围查询的字段.
    this.DTFieldOfLabel = '发起日期'; //日期字段名.
    this.Icon = '';
    this.PageSize = 0; // 分页的页面行数, 0不分页.
    this.LinkField = 'Name';
    //  this.GroupFields = 'FK_FlowSortText';
    this.HisGLShowModel = GenerListPageShowModel.Table; //表格展示.
    //this.GroupFieldDefault='';
    // 定义列，这些列用于显示.
    this.Columns = [
      { Key: 'WorkID', Name: '工作ID', IsShow: false },
      { Key: 'FlowNo', Name: 'FlowNo', IsShow: false },
      { Key: 'Title', Name: '标题', width: 350 },
      { Key: 'StarterName', Name: '发起人' },
      { Key: 'FlowName', Name: '流程' },
      { Key: 'NodeName', Name: '停留节点' },
      { Key: 'DeptName', Name: '发起人部门' },
      { Key: 'RDT', Name: '发起日期' },
      { Key: 'TodoEmps', Name: '当前处理人' },
      { Key: 'PRI', Name: 'PRI' },
    ];

    //获得数据源.
    const handler = new HttpHandler('BP.WF.HttpHandler.WF');
    handler.AddPara('FlowNo', this.RequestVal('FlowNo'));
    const dbs = handler.DoMethodReturnJSON('StartEaryer_Init');
    this.Data = dbs['Start'];
  }

  //打开页面.
  LinkFieldClick(object: Record<string, any>) {
    let url = 'MyFlow.vue?FK_Flow=' + object.No;
    const keys = Object.keys(object);
    for (const key of keys) {
      url += `&${key}=${object[key]}`;
    }
    const obj = new GPNReturnObj(GPNReturnType.OpenUrlByDrawer90, url);
    return obj;
  }

  BtnClick(btnName: string, object: Record<string, any>) {
    if (btnName === object.Title) throw new Error('Method not implemented.');
  }
}
