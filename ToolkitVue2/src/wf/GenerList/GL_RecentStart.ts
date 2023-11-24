import { GenerListPageShowModel, PageBaseGenerList } from './PageBaseGenerList';
import { HttpHandler } from "../api/Gener";
import { GPNReturnObj, GPNReturnType } from "./GPNReturnObj";

export class GL_RecentStart extends PageBaseGenerList {
  constructor() {
    super('GL_RecentStart');
    this.PageTitle = '我发起的';
  }
  //重写的构造方法.
  async Init() {
    this.DTFieldOfSearch = 'RDT'; // 按照日期范围查询的字段，为空就不需要日期段查询.
    this.DTFieldOfLabel = '发起日期'; //日期字段名.
    this.LinkField = 'Title';
    const flowNo = this.RequestVal('FlowNo');

    if (flowNo == null) {
      this.GroupFields = 'NodeName,FlowName'; //分组字段.
      this.GroupFieldDefault = ''; //分组字段.
    } else {
      this.GroupFields = 'NodeName'; //停留节点.
      this.GroupFieldDefault = ''; //停留节点.
    }

    //alert(this.GroupFieldDefault);

    this.Icon = '';
    this.BtnOfToolbar = '打印';
    this.PageSize = 15; // 分页的页面行数, 0不分页.
    this.HisGLShowModel = GenerListPageShowModel.Table; //表格展示.

    // 定义列，这些列用于显示. IsRead,PRI是特殊字段.
    this.Columns = [
      { Key: 'WorkID', Name: '工作ID', IsShow: false, DataType: 2 },
      { Key: 'Title', Name: '标题', IsShow: true, DataType: 1, width: 350 },
      { Key: 'FK_Flow', Name: 'FK_Flow', IsShow: false, DataType: 1, width: 150 },
      { Key: 'FlowName', Name: '流程', IsShow: true, DataType: 1, width: 150 },
      { Key: 'NodeName', Name: '停留节点', IsShow: true, DataType: 1, width: 150 },
      { Key: 'TodoEmps', Name: '当前处理人', IsShow: true, DataType: 1, width: 150 },
      { Key: 'RDT', Name: '发起时间', IsShow: true, DataType: 7, width: 144 },
      { Key: 'PRI', Name: 'PRI', IsShow: true, DataType: 1, width: 50 },
      { Key: 'WFState', Name: '标签', IsShow: true, DataType: 2 },
    ];

    this.BtnsOfRow = 'Copy发起';
    //获得数据源.
    const handler = new HttpHandler('BP.WF.HttpHandler.WF');
    handler.AddPara('FlowNo', flowNo); //我发起的流程.
    const data: any = handler.DoMethodReturnJSON('RecentStart_Init');

    //处理数据,
    data.forEach((en) => {
      // 判断是否是逾期.  SDT 应完成日期与当前日期对比.
      let lab = '';
      if (en.SDT >= '2022-02-09') {
        lab = '逾期';
      }
      if (en.WFState == 0) {
        en.WFState = '空白';
        en.TodoEmps = en.StarterName;
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
      if (en.PRI == 0) en.PRI = '<img src="resource/WF/Img/PRI/0.png" style="display:inline"/>';
      if (en.PRI == 1) en.PRI = '<img src="resource/WF/Img/PRI/1.png" style="display:inline"/>';
      if (en.PRI == 2) en.PRI = '<img src="resource/WF/Img/PRI/2.png" style="display:inline"/>';
    });

    this.Data = data;
    console.log('data', this.Data);
  }

  //打开页面.
  LinkFieldClick(object: Record<string, any>) {
    let url = '/#/WF/MyFlow?WorkID=' + object.WorkID;
    const keys = Object.keys(object);
    for (const key of keys) {
      if (key === 'WorkID') continue;
      url += `&${key}=${object[key]}`;
    }

    return new GPNReturnObj(GPNReturnType.OpenUrlByDrawer75, url);
  }

  BtnClick(btnName: string, object: Record<string, any>) {
    if (btnName === 'Copy发起') {
      const handler = new HttpHandler('BP.WF.HttpHandler.WF');
      handler.AddPara('FK_Flow', object.FK_Flow);
      handler.AddPara('WorkID', object.WorkID);
      const data = handler.DoMethodReturnString('Start_CopyAsWorkID');
      // @zhoupeng 这里需要修改
      const url = '/#/WF/MyFlow?WorkID=' + data + '&FK_Flow=' + object.FK_Flow;
      const obj = new GPNReturnObj(GPNReturnType.OpenUrlByDrawer90, url);
      return obj;
    }
    return;
    // throw new Error('Method not implemented.');
  }
}
