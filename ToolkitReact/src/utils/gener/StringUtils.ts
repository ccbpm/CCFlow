import { useUserStore } from '@/stores/user';
import { message } from 'ant-design-vue';

/**
 * 去除空格\b\s等
 * @param str
 * @constructor
 */
export function Trim(str: string) {
  return str.replace(/[\n\b\t\s+]/g, '');
}

/**
 * 表达式短语的替换
 * @param expStr 表达式
 * @param mainData 替换的JSON格式
 * @constructor
 */

export function DealExp(expStr: string, mainData: Record<string, any> | null = {}) {
  const userStore = useUserStore();
  expStr = expStr.replace(/~/g, "'");
  if (typeof expStr !== 'string') return '';
  if (expStr.includes('@') == false) return expStr;
  //替换表达式常用的用户信息
  expStr = expStr.replace(/@WebUser.No/g, <string>userStore.webUser.No);
  expStr = expStr.replace(/@WebUser.Name/g, <string>userStore.webUser.Name);
  //expStr = expStr.replace("@WebUser.FK_DeptNameOfFull", WebUser.FK_DeptNameOfFull);
  expStr = expStr.replace(/@WebUser.FK_DeptName/g, userStore.webUser.FK_DeptName);
  expStr = expStr.replace(/@WebUser.FK_Dept/g, <string>userStore.webUser.FK_Dept);
  expStr = expStr.replace(/@WebUser.OrgNo/g, <string>userStore.webUser.OrgNo);
  expStr = expStr.replace(/@WebUser.OrgName/g, <string>userStore.webUser.OrgName);
  if (expStr.includes('@') == false) return expStr;
  if (mainData == null) {
    if (expStr.includes('@') == true) {
      message.error(expStr + '含有@未被替换');
      return expStr;
    }
    return expStr;
  }

  for (const key in mainData) {
    expStr = expStr.replace(new RegExp('@' + key, 'g'), mainData[key]);
    if (expStr.includes('@') == false) return expStr;
  }
  if (expStr.includes('@') == true) {
    message.error(expStr + '含有@未被替换');
    return expStr;
  }
  return expStr;
}

/**
 * 获取数据形式为@Name=张三@Age=24@XingBie=男，根据key值获取对应的数据
 * @param atPara
 * @param key
 * @constructor
 */
export function GetPara(atPara: string, key: string) {
  const reg = new RegExp('(^|@)' + key + '=([^@]*)(@|$)');
  const results = atPara.match(reg);
  if (results != null) {
    return unescape(results[2]);
  }
  return '';
}

/**
 * JSON转params的url字符串
 * @param data
 * @constructor
 */
export function GetParamsUrl(data: Record<string,any>) {
  const params = JSON.parse(JSON.stringify(data));
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

/**
 * 把URL转成JSON格式
 * @param data
 * @constructor
 */
export function GetUrlToJSON(data: Record<string,any>) {
  const obj: { [propName: string]: any } = {};
  const args = data.split('?');
  if (args.length < 2 || !args[1].trim()) {
    return {};
  }
  args[1].split('&').forEach((arg: string) => {
    const [key, val] = arg.split('=');
    obj[key] = val;
  });
  return obj;
}

export function dealClassId(classID: string, targetFilePrefix: string) {
  if (classID.startsWith('/src') && classID.endsWith('.ts')) {
    const startPosition = classID.lastIndexOf(targetFilePrefix);
    const endPosition = classID.indexOf('.ts');
    classID = classID.substring(startPosition + 1, endPosition);
  }
  return classID;
}

/**
 * 判断是否是移动端打开
 * @constructor
 */
export function IsMobile() {
  const info = navigator.userAgent;
  const agents = ['Android', 'iPhone', 'SymbianOS', 'Windows Phone', 'iPod', 'iPad'];
  for (let i = 0; i < agents.length; i++) {
    if (info.indexOf(agents[i]) >= 0) return true;
  }
  return false;
}
