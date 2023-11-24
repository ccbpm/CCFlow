import request from '../request';
import { REQUEST_URL } from '@/config/EnvProperties';

export default class HttpHandler {
  HttpHandlerName = '';
  params: { [propName: string]: any } = {};
  files: File[] = [];

  // 构造方法创建
  constructor(handlerName: string) {
    this.HttpHandlerName = handlerName;
  }

  public validate(str: string) {
    if (!str) {
      return false;
    }
    const s = str.replace(/^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g, '');
    return !(s == '' || s == 'null' || s == 'undefined');
  }

  // 添加参数
  public AddPara(key: string, val: any) {
    this.params[key] = val;
  }

  // 添加json类型
  public AddJson(json: { [propName: string]: any }) {
    this.params = {
      ...this.params,
      ...json,
    };
  }
  public AddFile(file) {
    this.files.push(file);
  }
  public Clear() {
    this.params = {};
  }

  // 添加url参数
  public AddUrlData(url = '') {
    let queryString = url;
    if (!url) {
      queryString = window.location.href.split('?')[1];
      if (!queryString) {
        return;
      }
    }
    queryString = decodeURI(queryString);
    for (const data of queryString.split('&')) {
      const [key, val] = data.split('=');
      if (!this.validate(key) || !this.validate(val)) {
        continue;
      }
      if (key == 'DoType' || key == 'DoMethod' || key == 'HttpHandlerName') {
        continue;
      }
      this.AddPara(key, decodeURIComponent(val));
    }
  }

  // todo vue中应该是用不到此方法，历史原因保留
  // 添加formData
  public AddFormData(fData: Record<string, any>) {
    const keys = Object.keys(fData);
    for (const key of keys) {
      this.AddPara(key, fData[key]);
    }
  }

  // 获取url参数, 向外提供？
  public getParams() {
    const params = JSON.parse(JSON.stringify(this.params));
    const keys = Object.keys(params);
    const pArr: Array<string> = [];
    for (const key of keys) {
      let val = params[key];
      if (val.includes('<script')) {
        val = '';
      }
      pArr.push(`${key}=${val}`);
    }
    return pArr.join('&');
  }

  private paramsToFormData() {
    const params = JSON.parse(JSON.stringify(this.params));
    const keys = Object.keys(params);
    // if (keys.length === 0) return null;
    const formData = new FormData();
    for (const key of keys) {
      formData.append(key, `${params[key]}`);
    }

    if (this.files.length > 0) {
      for (const file of this.files) {
        formData.append('file', file);
        // console.log(file);
      }
    }
    return formData;
  }

  // 自定义请求
  public CustomRequest<T>(params: Record<string, any>): Promise<T> {
    return request.post<null, T>(REQUEST_URL, this.paramsToFormData(), {
      params,
    });
  }

  // 这两个方法其实没有差别，因为已经在axios里面判断过了.
  public DoMethodReturnString(methodName: string) {
    return request.post<null, string>(REQUEST_URL, this.paramsToFormData(), {
      params: {
        DoType: 'HttpHandler',
        DoMethod: methodName,
        HttpHandlerName: this.HttpHandlerName,
        t: Math.random(),
      },
    });
  }

  public DoMethodReturnJson<T = Recordable>(methodName: string) {
    // REQUEST_URL 请求地址
    // this.paramsToFormData() 通过方法获取参数
    // params 实际是拼接在url里面的
    return request.post<null, T>(REQUEST_URL, this.paramsToFormData(), {
      params: {
        DoType: 'HttpHandler',
        DoMethod: methodName,
        HttpHandlerName: this.HttpHandlerName,
        t: Math.random(),
      },
    });
  }
}
