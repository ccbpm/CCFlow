import { getRequestParams } from './utils/Decode';

//列表模式.
export enum GenerListPageShowModel {
  //表格模式(待办模式)
  Table,
  //分组模式(类似于:PanelGroup模式)
  GroupBigIcon,
  //窗口模式(发起)
  Windows,
  //Icon模式(系统列表).
  BigIcon,
}
//帮助文档显示格式.
export enum HelpShowModel {
  //不显示
  None,
  //顶部按钮
  TopButton,
  //左侧Panel.
  LeftPanel,
}

export abstract class PageBaseGenerList {
  /**
   * 获得外部的参数
   * @param key 参数key
   * @returns
   */
  // public RequestVal(key: string) {
  //   const val = getRequestParams(key);
  //   if (!val) return getAllRequestParams(key);
  // }
  public PageTitle: string | null = '待办'; //页面标题.
  public Icon: string | null = 'icon-file'; //标题.
  public ClassID?: string; //实体类ID.

  public DTFieldOfSearch: string | null = 'RDT'; //日期查询字段.
  public DTFieldOfLabel: string | null = '记录日期'; //日期查询字段.
//  public BtnsOfRow = ''; //行操作按钮 逗号隔开
  public BtnOfToolbar = ''; //工具栏
  public LinkField = 'Title'; //链接字段,点击链接进入新窗口字段.
  public GroupFields = ''; //可以分组显示的字段.
  public GroupFieldDefault = ''; //默认的分组字段.
  public LabFields = ''; // 标签显示列
  public BtnsOfRow = '';

  //帮助按钮定义.
  public HelpDocs = ''; // 帮助文档，如果没有就不显示了.
  public HelpShowModel = HelpShowModel.None; // 帮助文档，如果没有就不显示了.

  // 显示格式: 0=表格(待办), 1=分组显示,  2= Windows模式(发起). 3=BigIcon(我的系统)
  public HisGLShowModel = GenerListPageShowModel.Table;

  // 参数，从外部传过来的
  public params: Record<string, any> = {};
  public setParams(params: Record<string, any>) {
    this.params = params;
  }
  /**
   * 获得外部的参数, 此方法为实现.
   * @param key 参数key
   * @returns
   */
  // 当前登录人的token
  public RequestVal(key: string) {
    return this.params[key] || getRequestParams(key);
  }

  public PageSize = 0; // 分页的页面行数, 0不分页.

  //返回的数据
  public Data: Array<Record<string, any>> = [];

  //显示的列.
  public Columns: Array<Record<string, any>> = [];

  /**
   * 构造方法
   * @param clsId 类的ID.
   */
  protected constructor(clsId) {
    this.ClassID = clsId;
  }

  //初始化数据.
  abstract Init();

  /** 标题点击事件 */
  abstract LinkFieldClick(object: Record<string, any>);

  /** 按钮点击事件 */
  abstract BtnClick(btnName: string, record: Record<string, any>);
  /**
   * 时间友好提示
   * @param adt
   * @returns
   */
  public FirendlyDT(adt: string) {
    const minute = 1000 * 60;
    const hour = minute * 60;
    const day = hour * 24;
    const week = day * 7;
    const month = day * 30;
    const time1 = new Date().getTime(); //当前的时间戳
    const time2 = Date.parse(new Date(adt).toString()); //指定时间的时间戳
    const time = time1 - time2;

    let result = '';
    if (time < 0) {
      result = '--';
    } else if (time / month >= 1) {
      result = Math.floor(time / month) + '月前';
    } else if (time / week >= 1) {
      result = Math.floor(time / week) + '周前';
    } else if (time / day >= 1) {
      result = Math.floor(time / day) + '天前';
    } else if (time / hour >= 1) {
      result = Math.floor(time / hour) + '小时前';
    } else if (time / minute >= 1) {
      result = Math.floor(time / minute) + '分钟前';
    } else {
      result = '刚刚';
    }
    return result;
  }
}
