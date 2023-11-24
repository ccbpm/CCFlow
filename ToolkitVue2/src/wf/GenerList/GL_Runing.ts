// import { message } from 'ant-design-vue';
import { Message as message } from 'element-ui';
import { GenerListPageShowModel, PageBaseGenerList } from './PageBaseGenerList';
import { HttpHandler } from "../api/Gener";
import { GPNReturnObj, GPNReturnType } from "./GPNReturnObj";

export class GL_Runing extends PageBaseGenerList {
  constructor() {
    super('GL_Runing');
    this.PageTitle = '在途';
  }
  //重写的构造方法.
  async Init() {
    this.DTFieldOfSearch = 'RDT'; // 按照日期范围查询的字段.
    this.DTFieldOfLabel = '发起日期'; //日期字段名.
    this.PageTitle = '在途';
    // this.BtnsOfRow = '催办,撤销发送'; //行操作按钮.
    //  this.BtnOfToolbar = '待办,草稿'; //table按钮.
    this.GroupFields = 'NodeName,FlowName'; //可以分组显示的字段..
    this.GroupFieldDefault = 'FlowName';
    this.PageSize = 10; // 分页的页面行数, 0不分页.
    this.Icon = '';

    this.HisGLShowModel = GenerListPageShowModel.Table; //表格展示.

    // 定义列，这些列用于显示.
    this.Columns = [
      { Key: 'WorkID', Name: '工作ID', IsShow: false, IsShowMobile: false },
      { Key: 'Title', Name: '标题', width: 350 },
      { Key: 'StarterName', Name: '发起人', width: 120 },
      { Key: 'FlowName', Name: '流程', width: 120 },
      { Key: 'NodeName', Name: '停留节点', width: 160 },
      { Key: 'DeptName', Name: '发起人部门', width: 150 },
      { Key: 'RDT', Name: '到达时间', width: 160 },
      { Key: 'TodoEmps', Name: '当前处理人', width: 350 },
      { Key: 'Btns', Name: 'Btns', IsShow: false, IsShowMobile: false },

      // { Key: 'PRI', Name: 'PRI', width: 350 },
    ];

    //获得数据源.
    const handler = new HttpHandler('BP.WF.HttpHandler.WF');
    handler.AddUrlData(); //获得页面参数.
    const data: any = handler.DoMethodReturnJSON('Runing_Init');
    //处理数据,增加标签.
    data.forEach((en) => {
      //到达时间友好提示.
      en.RDT = this.FirendlyDT(en.RDT);
      en.Btns = '催办,撤销发送';
    });

    //设置数据源.
    this.Data = data;
  }

  //打开页面.
  LinkFieldClick(object: Record<string, any>) {
    let url = '/#/WF/MyView?';
    const keys = Object.keys(object);
    const useKeys = ['WorkID', 'FK_Flow', 'FlowNo', 'FK_Node', 'FID', 'PWorkID'];
    for (const key of keys) {
      if (useKeys.includes(key)) url += `&${key}=${object[key]}`;
    }
    return new GPNReturnObj(GPNReturnType.OpenUrlByDrawer75, url);
  }

  //按钮事件.
  BtnClick(btnName: string, object: Record<string, any>) {
    const workID = object.WorkID;
    if (btnName === '撤销发送') {
      return this.UnSend(workID);
    }

    if (btnName === '催办') {
      this.Prsss(workID);
      return;
    }
  }

  //执行催办.
  async Prsss(workID: string) {
    // @hyh , 这里是Js的，目前需要翻译.
    const msg = window.confirm('请输入催办信息,该工作因为xxx原因,需要您优先处理.');
    if (msg == null) return;

    const handler = new HttpHandler('BP.WF.HttpHandler.WF');
    handler.AddPara('WorkID', workID);
    handler.AddPara('Msg', msg);
    const data: any = await handler.DoMethodReturnString('Runing_Press');

    if (data.indexOf('err@') == 0) {
      message.warning(data);
      return;
    }
    message.info(data);
    return new GPNReturnObj(GPNReturnType.DoNothing);
  }

  //撤销发送工作.
  async UnSend(workID: string) {
    if (window.confirm('您确定要撤销本次发送吗？') == false) return;

    const handler = new HttpHandler('BP.WF.HttpHandler.WF');
    handler.AddPara('WorkID', workID);
    handler.AddPara('UnSendToNode', 0); //撤销发送到的节点.
    const data: any = await handler.DoMethodReturnString('Runing_UnSend');

    if (data.indexOf('err@') == 0) {
      return alert(data);
    }

    if (data.indexOf('KillSubThared') == 0) {
      message.error(data.replace('KillSubThared@', ''));
      return;
    }
    const url = '/@/WF/MyFlow?WorkID=' + workID;
    return new GPNReturnObj(GPNReturnType.GoToUrl, url);
  }
}
