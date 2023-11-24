import { PageBaseGenerList } from './PageBaseGenerList';
import { HttpHandler } from "../api/Gener";
import { DataType } from './utils/DataType';
import { GPNReturnObj, GPNReturnType } from "./GPNReturnObj";

export class GL_RecentWork extends PageBaseGenerList {
  constructor() {
    super('GL_RecentWork');
    this.PageTitle = '近期工作';
  }
  //重写的构造方法.
  async Init() {
    this.DTFieldOfSearch = 'RDT'; // 按照日期范围查询的字段，为空就不需要日期段查询.
    this.DTFieldOfLabel = '发起日期'; //日期字段名.
    this.LinkField = 'Title';
    // this.GroupFields = 'NodeName,FlowName,StarterName'; //分组字段.
    // this.GroupFieldDefault = 'FlowName'; //分组字段.
    this.LabFields = 'WFState';
    this.Icon = '';
    this.BtnOfToolbar = '导出';
    this.PageSize = 15; // 分页的页面行数, 0不分页.

    // 定义列，这些列用于显示. IsRead,PRI是特殊字段.
    this.Columns = [
      { Key: 'WorkID', Name: '工作ID', IsShow: false, DataType: 2 },
      { Key: 'Title', Name: '标题', IsShow: true, DataType: 1, width: 350 },
      { Key: 'StarterName', Name: '发起人', IsShow: true, DataType: 1, width: 66 },
      { Key: 'NodeName', Name: '停留节点', IsShow: true, DataType: 1, width: 150 },
      { Key: 'FlowName', Name: '流程', IsShow: true, DataType: 1, width: 150 },
      { Key: 'RDT', Name: '发起时间', IsShow: true, DataType: 7, width: 144 },
      { Key: 'Sender', Name: '发送人', IsShow: true, DataType: 1, width: 121 },
      { Key: 'PRI', Name: 'PRI', IsShow: true, DataType: 1, width: 50 },
      // { Key: 'SDT', Name: '应完成时间', IsShow: true, DataType: 7, width: 144 },
      // { Key: 'ADT', Name: '接受时间', IsShow: true, DataType: 7, width: 150 },
      { Key: 'WFState', Name: '标签', IsShow: true, DataType: 2, width: 160 },
    ];

    //获得数据源.
    const handler = new HttpHandler('BP.WF.HttpHandler.WF');
    handler.AddPara('FlowNo', this.RequestVal('FlowNo')); //是否按照flowNo查询.
    const data: any = handler.DoMethodReturnJSON('RecentWork_Init');

    //处理数据,增加标签. @liuwei.
    data.forEach((en) => {
      // 判断是否是逾期.  SDT 应完成日期与当前日期对比.
      const lab = '';
      // if (en.SDT >= DataType.CurrentDateTime) lab = '@逾期=red'; //@lyc
      if (en.WFState == 1) en.WFState = '@草稿=orange';
      if (en.WFState == 2) en.WFState = '@进行中=green';
      if (en.WFState == 5) en.WFState = lab + '@退回=red';
      if (en.WFState == 3) en.WFState = lab + '@完成=green';
      if (en.WFState == 6) en.WFState = lab + '@移交=red';
      if (en.WFState == 8) en.WFState = lab + '@加签=red';
      if (en.PRI == 0) en.PRI = '<img src="resource/WF/Img/PRI/0.png" style="display:inline"/>';
      if (en.PRI == 1) en.PRI = '<img src="resource/WF/Img/PRI/1.png" style="display:inline"/>';
      if (en.PRI == 2) en.PRI = '<img src="resource/WF/Img/PRI/2.png" style="display:inline"/>';
    });

    this.Data = data;

    this.GroupFields = 'NodeName,FlowName'; //分组字段.
    this.GroupFieldDefault = ''; //分组字段.

    console.log('data', this.Data);
  }

  //打开页面.
  LinkFieldClick(object: Record<string, any>) {
    let url = '/#/WF/MyView?WorkID=' + object.WorkID;
    const keys = Object.keys(object);
    for (const key of keys) {
      if (key === 'WorkID') continue;
      url += `&${key}=${object[key]}`;
    }
    return new GPNReturnObj(GPNReturnType.OpenUrlByDrawer75, url);
  }

  BtnClick(btnName: string, object: Record<string, any>) {
    if (btnName == '批处理') {
      const url = '/@/WF/Batch?' + object.WorkID;
      return new GPNReturnObj(GPNReturnType.GoToUrl, url);
      return;
    }

    alert('没有处理功能' + btnName);
    return;
    // throw new Error('Method not implemented.');
  }
}
