import { GenerListPageShowModel, PageBaseGenerList } from './PageBaseGenerList';
import { GPNReturnObj, GPNReturnType } from "./GPNReturnObj";
// import { MySystems } from '/@/CCFast/GPM/MySystem';
// import { APP_MENU_CACHE_KEY } from '/@/enums/cacheEnum';
// import { setAuthCache } from '/@/utils/auth';

// @zhoupeng 需要重写，低代码部分不存在
export class GL_System extends PageBaseGenerList {
  constructor() {
    super('GL_System');
    this.PageTitle = '低代码';
  }
  //重写的构造方法.
  async Init() {
    this.LinkField = 'Name';
    this.Icon = '';
    this.BtnOfToolbar = '新建系统';
    this.HisGLShowModel = GenerListPageShowModel.BigIcon;

    //定义列,这些列用于显示.IsRead,PRI是特殊字段.
    this.Columns = [
      { Key: 'No', Name: '编号', IsShow: false, DataType: 2 },
      { Key: 'Name', Name: '名称', IsShow: true, DataType: 1, width: 350 },
      { Key: 'Icon', Name: '图标', IsShow: false, DataType: 1, width: 350 },
    ];

    // const ens = new MySystems();
    // await ens.RetrieveAll();
    // this.Data = ens;
    // // 进入低代码页面清理缓存
    // setAuthCache(APP_MENU_CACHE_KEY, []);
  }

  //打开页面.
  LinkFieldClick(object: Record<string, any>) {
    let url = '/#/WF/Comm/En?EnName=TS.GPM.MySystem&PKVal=' + object.No;
    const keys = Object.keys(object);
    for (const key of keys) {
      if (key === 'No') continue;
      url += `&${key}=${object[key]}`;
    }
    return new GPNReturnObj(GPNReturnType.GoToUrl, url);
  }
  override BtnClick(btnName: string, record: Record<string, any>) {
    // if (btnName == '新建系统') {
    //   const url = '/WF/Comm/GroupPageNew?EnName=GPN_System';
    //   // return url;
    //   //  const url = '/#/WF/Comm/UIEntity/GroupPageNew?EnName=GPN_System';
    //   return new GPNReturnObj(GPNReturnType.GoToUrl, url);
    // }

    if (btnName === '新建系统') {
      // WF/Comm/GroupPageNew?EnName=GPN_NewFlow
      // GotoUrl 文件地址  例如打开GPN /@/WF/Comm/UIEntity/GroupPageNew.vue?xxx=xxx
      // OpenUrlByNewWindow 路由地址 例如打开GPN /#/WF/Comm/GroupPageNew?xxx=xxx  这里看路由文件的配置
      const url = '/@/WF/Comm/UIEntity/GroupPageNew.vue?EnName=GPN_System';
      return new GPNReturnObj(GPNReturnType.OpenUrlByDrawer, url);
    }

    alert('没有判断的BtnName=' + btnName + record);
    return;
  }
}
