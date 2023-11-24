import { PageBaseGenerList } from './PageBaseGenerList';
import { HttpHandler } from "../api/Gener";
import { GPNReturnObj, GPNReturnType } from "./GPNReturnObj";

export class GL_BatchWorkCheckModel extends PageBaseGenerList {
  constructor() {
    super('GL_BatchWorkCheckModel');
    this.PageTitle = '批处理';
  }
  //重写的构造方法.
  async Init() {
    // this.DTFieldOfSearch = 'RDT'; // 按照日期范围查询的字段，为空就不需要日期段查询.
    // this.DTFieldOfLabel = '发起日期'; //日期字段名.
    this.LinkField = 'FlowName';
    // this.GroupFields = 'FlowName'; //分组字段.
    // this.GroupFieldDefault = 'FlowName'; //分组字段.
    this.Icon = '';
    // this.BtnOfToolbar = '批处理,打印';
    this.PageSize = 0; // 分页的页面行数, 0不分页.

    this.BtnOfToolbar = '返回批处理,发送,退回';

    // 定义列，这些列用于显示. IsRead,PRI是特殊字段.
    this.Columns = [
      { Key: 'WorkID', Name: '流程名称', IsShow: true, DataType: 1, width: 200 },
      { Key: 'Title', Name: '流程名称', IsShow: true, DataType: 1, width: 200 },
      { Key: 'StarterName', Name: '节点ID', IsShow: false, DataType: 1, width: 66 },
      { Key: 'ADT', Name: '节点名称', IsShow: true, DataType: 1, width: 200 },
      { Key: 'ADT', Name: '规则', IsShow: false, DataType: 1, width: 200 },
      { Key: 'NUM', Name: '待办数量', IsShow: true, DataType: 7, width: 144 },
    ];

    //获得数据源.
    const handler = new HttpHandler('BP.WF.HttpHandler.WF_WorkOpt_Batch');
    handler.AddPara('FK_Node', this.RequestVal('NodeID')); //参数.
    //
    const data: any = handler.DoMethodReturnJSON('WorkCheckModel_Init');
    this.Data = data;
  }

  //打开页面.
  LinkFieldClick(object: Record<string, any>) {
    //  if (object.BatchRole == 1)
    const url = '/src/WF/Batch.vue?NodeID' + object.NodeID;
    window.open(url); //打开页面。
  }

  BtnClick(btnName: string, object: Record<string, any>) {
    if (btnName === '返回批处理') {
      const url = '/src/WF/Comm/GL?EnName=GL_Batch';
      const obj = new GPNReturnObj(GPNReturnType.GoToUrl, url);
      return obj;
    }
    if (btnName === object.WorkID) return null;
    // throw new Error('Method not implemented.');
  }
}
