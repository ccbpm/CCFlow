import { GenerListPageShowModel, PageBaseGenerList } from './PageBaseGenerList';
import { HttpHandler } from "../api/Gener";
import { GPNReturnObj, GPNReturnType } from "./GPNReturnObj";

export class GL_FutureTodolist extends PageBaseGenerList {
  constructor() {
    super('GL_FutureTodolist');
    this.PageTitle = '未来待办';
  }
  //重写的构造方法.
  async Init() {
    this.DTFieldOfSearch = 'RDT'; // 按照日期范围查询的字段，为空就不需要日期段查询.
    this.DTFieldOfLabel = '发起日期'; //日期字段名.
    this.LinkField = 'Title';
    this.GroupFields = 'NodeName,FlowName,StarterName'; //分组字段.
    this.GroupFieldDefault = 'FlowName'; //分组字段.
    this.Icon = '';
    this.BtnOfToolbar = '批处理,打印';
    this.PageSize = 15; // 分页的页面行数, 0不分页.
    this.HisGLShowModel = GenerListPageShowModel.Table; //表格展示.

    // 定义列，这些列用于显示. IsRead,PRI是特殊字段.
    this.Columns = [
      { Key: 'WorkID', Name: '工作ID', IsShow: false, DataType: 2 },
      { Key: 'Title', Name: '标题', IsShow: true, DataType: 1, width: 350 },
      { Key: 'StarterName', Name: '发起人', IsShow: true, DataType: 1, width: 66 },
      { Key: 'NodeName', Name: '停留节点', IsShow: true, DataType: 1 },
      { Key: 'FlowName', Name: '流程', IsShow: true, DataType: 1, width: 150 },
      { Key: 'RDT', Name: '发起时间', IsShow: true, DataType: 7, width: 144 },
      { Key: 'Sender', Name: '发送人', IsShow: true, DataType: 1, width: 121 },
      { Key: 'PRI', Name: '优先', IsShow: true, DataType: 1, width: 46 },
      { Key: 'SDT', Name: '应完成时间', IsShow: true, DataType: 7, width: 144 },
      { Key: 'ADT', Name: '接受时间', IsShow: true, DataType: 7, width: 150 },
      { Key: 'IsRead', Name: '是否读取', IsShow: false, DataType: 2 },
      { Key: 'WFState', Name: '标签', IsShow: true, DataType: 2 },
    ];

    //获得数据源.
    const handler = new HttpHandler('BP.WF.HttpHandler.WF');
    const data: any = handler.DoMethodReturnJSON('FutureTodolist_Init');
    //处理数据,增加标签. @liuwei.
    data.forEach((en) => {
      // 判断是否是逾期.  SDT 应完成日期与当前日期对比.
      let lab = '';
      if (en.SDT >= '2022-02-09') {
        lab = '逾期';
      }
      if (en.WFState == 1) {
        en.WFState = '草稿';
      }
      if (en.WFState == 2) {
        en.WFState = '进行中';
      }

      if (en.WFState == 5) {
        en.WFState = lab + '<font color=red>退回</font>';
      }
      if (en.WFState == 3) {
        en.WFState = lab + '<font color=green>完成</font>';
      }
      if (en.WFState == 6) {
        en.WFState = lab + '<font color=red>移交</font>';
      }
      if (en.WFState == 8) {
        en.WFState = lab + '<font color=red>加签</font>';
      }
    });

    this.Data = data;
    console.log('data', this.Data);
  }

  //打开页面.
  LinkFieldClick(object: Record<string, any>) {
    let url = '/@/WF/MyFlow?WorkID=' + object.WorkID;
    const keys = Object.keys(object);
    for (const key of keys) {
      if (key === 'WorkID') continue;
      url += `&${key}=${object[key]}`;
    }
    return new GPNReturnObj(GPNReturnType.OpenUrlByDrawer90, url);
  }

  BtnClick(btnName: string, object: Record<string, any>) {
    const url = '';
    return new GPNReturnObj(GPNReturnType.OpenUrlByDrawer90, url);
    // throw new Error('Method not implemented.');
  }
}
