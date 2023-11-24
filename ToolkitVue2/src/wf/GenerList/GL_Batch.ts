import { PageBaseGenerList } from './PageBaseGenerList';
import { HttpHandler } from "../api/Gener";
import { GPNReturnObj, GPNReturnType } from "./GPNReturnObj";

export class GL_Batch extends PageBaseGenerList {
  constructor() {
    super('GL_Batch');
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

    // 定义列，这些列用于显示. IsRead,PRI是特殊字段.
    this.Columns = [
      { Key: 'FlowName', Name: '流程名称', IsShow: true, DataType: 1, width: 200 },
      { Key: 'NodeID', Name: '节点ID', IsShow: false, DataType: 1, width: 66 },
      { Key: 'Name', Name: '节点名称', IsShow: true, DataType: 1, width: 200 },
      { Key: 'BatchRole', Name: '规则', IsShow: false, DataType: 1, width: 200 },
      { Key: 'Num', Name: '待办数量', IsShow: true, DataType: 7, width: 144 },
    ];

    //获得数据源.
    const handler = new HttpHandler('BP.WF.HttpHandler.WF');
    //
    const data: any = handler.DoMethodReturnJSON('Batch_Init');
    this.Data = data;
  }

  //打开页面.
  LinkFieldClick(object: Record<string, any>) {
    alert('批量处理未完成.');
    //  if (object.BatchRole == 1)
    //,要根据不同的, 如何转向?  /#/WF/Comm/GenerList?EnName=GL_Batch
    const url = '/#/WF/Comm/GenerList?NodeID=' + object.NodeID + '&EnName=GL_BatchWorkCheckModel';
    // alert(url);
    const obj = new GPNReturnObj(GPNReturnType.OpenUrlByDrawer90, url);
    return obj;
  }

  BtnClick(btnName: string, object: Record<string, any>) {
    if (btnName === object.WorkID) return null;
    // throw new Error('Method not implemented.');
  }
}
