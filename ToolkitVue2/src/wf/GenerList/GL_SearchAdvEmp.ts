import { GenerListPageShowModel, PageBaseGenerList } from './PageBaseGenerList';
import { GPNReturnObj, GPNReturnType } from "./GPNReturnObj";
// import WebUser from '/@/bp/web/WebUser';
// import { FlowExts } from '/@/WF/Admin/AttrFlow/FlowExt';


// @zhoupeng 需要重写这部分
export class GL_SearchAdvEmp extends PageBaseGenerList {
  constructor() {
    super('GL_SearchAdvEmp');
    this.PageTitle = '高级人员查询';
  }
  //重写的构造方法，初始化参数.
  async Init() {
    this.Icon = '';
    this.PageSize = 0; // 分页的页面行数, 0不分页.
    this.DTFieldOfSearch = ''; //按照日期范围查询的字段.
    this.DTFieldOfLabel = ''; //日期字段名.
    this.LinkField = 'Name'; //关键字段.
    this.GroupFields = ''; //分组字段.
    this.GroupFieldDefault = ''; //默认分组字段.
    this.HisGLShowModel = GenerListPageShowModel.Windows; //窗口的模式.
    this.BtnsOfRow = '';
    // this.BtnOfToolbar = '宫格展示';
    // this.GroupFieldDefault='';
    // 定义列，这些列用于显示.
    this.Columns = [
      { Key: 'No', Name: '编号', IsShow: true, width: 50 },
      { Key: 'Name', Name: '名称', IsShow: true, width: 100 },
      { Key: 'FlowNote', Name: '说明', IsShow: true, width: 100 },
      { Key: 'FK_FlowSortText', Name: '目录', IsShow: true },
      { Key: 'Icon', Name: 'Icon', IsShow: false },
    ];

    //获得数据源.
    // const flows = new FlowExts();
    // await flows.RetrieveLikeKey(WebUser.No + ',', 'AdvEmps', null, null, 'No');
    // this.Data = flows;
  }

  //打开页面.
  LinkFieldClick(object: Record<string, any>) {
    const flowNo = object.No;
    let url = '/src/WF/Comm/Search?EnName=TS.FlowData.GenerWorkFlowExt&FK_Flow=' + flowNo;
    const keys = Object.keys(object);
    for (const key of keys) {
      url += `&${key}=${object[key]}`;
    }
    return new GPNReturnObj(GPNReturnType.OpenUrlByDrawer90, url);
  }

  BtnClick(btnName: string, object: Record<string, any>) {
    console.log(object);
    alert('没有解析:' + btnName);
  }
}
