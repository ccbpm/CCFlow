import { GenerListPageShowModel, PageBaseGenerList } from './PageBaseGenerList';
import { HttpHandler } from "../api/Gener";
import { GPNReturnObj, GPNReturnType } from "./GPNReturnObj";

export class GL_Start extends PageBaseGenerList {
  constructor() {
    super('GL_Start');
    this.PageTitle = '发起流程';
  }
  //重写的构造方法，初始化参数.
  async Init() {
    this.Icon = '';
    this.PageSize = 0; // 分页的页面行数, 0不分页.
    this.DTFieldOfSearch = ''; //按照日期范围查询的字段.
    this.DTFieldOfLabel = ''; //日期字段名.
    this.LinkField = 'Name'; //关键字段.
    this.GroupFields = 'FK_FlowSortText'; //分组字段.
    this.GroupFieldDefault = 'FK_FlowSortText'; //默认分组字段.
    this.HisGLShowModel = GenerListPageShowModel.Windows; //窗口的模式.
    // this.BtnOfToolbar = '宫格展示';
    // this.GroupFieldDefault='';
    // 定义列，这些列用于显示.
    this.Columns = [
      { Key: 'No', Name: '编号', IsShow: true, width: 50 },
      { Key: 'Name', Name: '名称', IsShow: true, width: 100 },
      { Key: 'FlowNote', Name: '说明', IsShow: true, width: 100 },
      { Key: 'FK_FlowSortText', Name: '目录', IsShow: true },
      { Key: 'Icon', Name: 'Icon', IsShow: false },
      { Key: 'Btns', Name: '操作', IsShow: false },
    ];

    //获得数据源.
    const handler = new HttpHandler('BP.WF.HttpHandler.WF');
    const dbs = handler.DoMethodReturnJSON('Start_Init');

    // @ts-ignore
    const data = dbs['Start'];

    //处理数据,增加ICON.
    data.forEach((en) => {
      // 判断是否是逾期.  SDT 应完成日期与当前日期对比.
      if (!en.Icon) en.Icon = 'icon-user';

      en.Title = '<i class=' + en.Icon + '></i><br/>' + en.Name;
      en.Btns = '启动流程,近期发起,近期参与,报表';

      //'<img src="resource/WF/Img/PRI/2.png" style="display:inline"/>'+en.Name;
    });
    //设置数据源.
    this.Data = data;
  }

  //打开页面.
  LinkFieldClick(object: Record<string, any>) {
    const flowNo = object.No;
    let url = '/#/WF/MyFlow?FK_Flow=' + flowNo + '&RoutFrom=MyFlow';
    const keys = Object.keys(object);
    for (const key of keys) {
      url += `&${key}=${object[key]}`;
    }
    // return new GPNReturnObj(GPNReturnType.OpenIframeByDrawer90, url);
    return new GPNReturnObj(GPNReturnType.OpenUrlByDrawer90, url);
  }

  BtnClick(btnName: string, object: Record<string, any>) {
    if (btnName === '近期发起') {
      const url = '/#/WF/GenerList?EnName=GL_RecentStart&FlowNo=' + object.No;
      return new GPNReturnObj(GPNReturnType.OpenUrlByDrawer90, url);
    }
    if (btnName === '近期参与') {
      const url = '/#/WF/GenerList?EnName=GL_RecentWork&FlowNo=' + object.No;
      return new GPNReturnObj(GPNReturnType.OpenUrlByDrawer90, url);
    }
    if (btnName === '启动流程' || btnName === '发起流程') {
      return this.LinkFieldClick(object);
    }

    alert('没有解析:' + btnName);
  }
}
