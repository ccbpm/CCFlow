// 返回对象.
export class GPNReturnObj {
  public data = '';
  public ReturnType: GPNReturnType = GPNReturnType.Close;
  constructor(ReturnType: GPNReturnType = GPNReturnType.Close, data: any = '') {
    this.data = data;
    this.ReturnType = ReturnType;

    if (drawerTypes.includes(ReturnType)) {
      for (const page of iframePages) {
        if (typeof data === 'string' && data.includes(page)) {
          // alert(`错误！[${page}]不能使用抽屉打开，应使用OpenIframeByDrawer打开\n
          // 例:iframe /#${page}?xxx=xx\n
          //    组件: /src${page}?xxx=xx`);
          return;
        }
      }
    }
  }
}


export enum GPNReturnType {
  //提示消息.
  Message,
  //提示错误.
  Error,
  //转到url.
  GoToUrl,
  //关闭.
  Close,
  //关闭并重载.
  CloseAndReload,
  //刷新页面.
  Reload,
  //侧滑的方式打开窗口.
  OpenUrlByDrawer75,
  OpenUrlByDrawer30,
  OpenUrlByDrawer90,
  //侧滑的方式打开窗口.
  OpenUrlByDrawer,
  // 抽屉打开iframe
  OpenIframeByDrawer,
  OpenIframeByDrawer30,
  OpenIframeByDrawer75,
  OpenIframeByDrawer90,
  //打开新窗口.
  OpenUrlByNewWindow,
  OpenUrlByTab,
  ReBind,
  ///执行参数返回值.
  DoWhatParas,
  //不做任何事情.
  DoNothing,
  Update,
}

const iframePages = ['/WF/FlowError', '/WF/MyCC', '/WF/MyCCGener', '/WF/MyFlow', '/WF/MyFlowGener', '/WF/FlowTree', '/WF/MyView', '/WF/MyViewGener'];
const drawerTypes = [GPNReturnType.OpenUrlByDrawer, GPNReturnType.OpenUrlByDrawer30, GPNReturnType.OpenUrlByDrawer75, GPNReturnType.OpenUrlByDrawer90];

