// 实体类
import request from '../request';
import { createWhereArgs, splitAtString } from './ParamsUtils';
import { REQUEST_URL } from '@/config/EnvProperties';

export default class BSEntity {
  [x: string]: any;
  private readonly EnName: string = '';
  private PKVal = '';
  private queryArgs: Array<string> = [];
  private extraParams: Map<string, any> = new Map<string, any>();
  private data: any = {};

  constructor(EnName: string, PKVal = '') {
    this.EnName = EnName;
    this.PKVal = PKVal;
  }

  public setPK(val: string) {
    this.PKVal = val;
  }

  public setVal(key: string, val: any) {
    this.data[key] = val;
    // this[key] = val;
  }

  public getVal(key: string) {
    return this.data[key];
  }

  public setData(data: any) {
    this.data = data;
  }

  // 分解额外参数
  private decodeExtraParams() {
    const { AtPara } = this.data;
    if (!AtPara) return;
    if (AtPara.startsWith('@')) {
      const tempArr = splitAtString(AtPara);
      tempArr.forEach((temp: string) => {
        const [key, val] = temp.split('=');
        this.extraParams.set(key, val);
      });
    }
  }

  // 获取url参数
  private getUrlParams() {
    const params: { [propsName: string]: string } = {};
    if (this.EnName) params['EnName'] = this.EnName;
    if (this.PKVal) params['PKVal'] = this.PKVal;
    params['t'] = Date.now() + '';
    return params;
  }

  // 将额外参数转为字符串
  private encodeExtraParams(): string {
    let paramsStr = '';
    for (const [key, val] of this.extraParams) {
      paramsStr += `@${key}=${val}`;
    }
    return paramsStr;
  }

  // 提交到后台时将原型上的key合并到Row
  private collectProperties() {
    const keys = Object.keys(this);
    const unRelatedKeys: string[] = [];
    for (const idxKey of keys) {
      if (this.data[idxKey]) {
        this.data[idxKey] = this[idxKey];
        continue;
      }
      unRelatedKeys.push(idxKey);
    }
    if (unRelatedKeys.length > 0) {
      console.warn('以下属性未在原始data中定义:' + JSON.stringify(unRelatedKeys) + ' , 更新/新增操作时将被忽略');
    }
  }

  // 获取表单传参 json => FormData
  private generateFormData(): FormData {
    this.collectProperties();
    const data = JSON.parse(JSON.stringify(this.data));
    delete data.AtPara;
    const keys = Object.keys(data);
    const formData = new FormData();
    for (const key of keys) {
      formData.append(key, `${this.data[key]}`);
    }
    formData.append('AtPara', this.encodeExtraParams());
    if (!keys.includes('pkval')) {
      formData.append('pkval', '');
    }
    return formData;
  }

  // 获取实体
  public async Init() {
    try {
      this.data = await request.post(REQUEST_URL, null, {
        params: {
          DoType: 'Entity_Init',
          ...this.getUrlParams(),
        },
      });
      this.mountProperty();
      this.decodeExtraParams();
    } catch (e: any) {
      console.error(e);
    }
  }

  private mountProperty() {
    const keys = Object.keys(this.data);
    const readonlyKeys: string[] = [];
    for (const key of keys) {
      try {
        if (typeof this[key] === 'function') continue;
        this[key] = this.data[key];
      } catch (e: any) {
        readonlyKeys.push(key);
      }
    }
    // if (readonlyKeys.length > 0) {
    //   console.warn('以下属性只读:' + JSON.stringify(readonlyKeys));
    // }
  }

  // 更新数据
  public async Update() {
    await request.post<string>(REQUEST_URL, this.generateFormData(), {
      params: {
        DoType: 'Entity_Update',
        ...this.getUrlParams(),
      },
    });
  }

  // 更新数据
  public async Delete() {
    await request.post<string>(REQUEST_URL, this.generateFormData(), {
      params: {
        DoType: 'Entity_Delete',
        ...this.getUrlParams(),
      },
    });
  }

  public async RetrieveFromDBSources() {
    this.data = await request.post<any>(REQUEST_URL, this.generateFormData(), {
      params: {
        DoType: 'Entity_RetrieveFromDBSources',
        ...this.getUrlParams(),
      },
    });
    this.decodeExtraParams();
    return this.data.RetrieveFromDBSources;
  }

  // 按条件查询,会替换掉data
  public async Retrieve(...args: string[]) {
    this.queryArgs = args;
    this.data = await request.post<any>(REQUEST_URL, this.generateFormData(), {
      params: {
        DoType: 'Entity_Init',
        ...this.getUrlParams(),
        Paras: createWhereArgs(this.queryArgs),
      },
    });
    this.decodeExtraParams();
  }

  // 新增
  public async Insert() {
    this.data = await request.post<any>(REQUEST_URL, this.generateFormData(), {
      params: {
        DoType: 'Entity_Insert',
        ...this.getUrlParams(),
      },
    });
    this.decodeExtraParams();
  }

  // 执行Entity方法返回String，二者没有任何区别，底层都已经处理好了
  public async DoMethodReturnString(methodName: string, ...params: string[]) {
    return await this.execEntityMethod(methodName, params);
  }

  // 执行Entity方法返回JSON，二者没有任何区别，底层都已经处理好了
  public async DoMethodReturnJSON(methodName: string, ...params: string[]) {
    return await this.execEntityMethod(methodName, params);
  }

  private async execEntityMethod(methodName: string, params: string[] = []) {
    const formData = new FormData();
    if (params.length > 0) {
      formData.append('paras', params.join('~'));
    }
    return await request.post<any, any>(REQUEST_URL, formData, {
      params: {
        DoType: 'Entity_DoMethodReturnString',
        EnName: this.EnName,
        PKVal: encodeURIComponent(this.PKVal),
        MethodName: methodName,
        t: Date.now(),
      },
    });
  }

  public getData() {
    return this.data;
  }

  // 获取表单额外参数
  public getPara(key: string) {
    return this.extraParams.get(key);
  }

  // 设置表单额外参数
  public setPara(key: string, val: any) {
    this.extraParams.set(key, val);
  }
}
