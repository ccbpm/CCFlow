// 实体类
import request from "../request";
import { REQUEST_URL } from "@/config/EnvProperties";
import { getAppEnvConfig } from "@/utils/env";

export default class DBAccess {
  public static GenerGUID() {
    return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(
      /[xy]/g,
      function (c) {
        const r = (Math.random() * 16) | 0,
          v = c == "x" ? r : (r & 0x3) | 0x8;
        return v.toString(16);
      }
    );

    //  const dt= new  Date();
    //  return Math.random().toString(); // dt.toString();
    // return Math.random().toString(2); // dt.toString();
  }
  public static GenerOID(type?: string) {
    return 100;
  }

  public static data: Array<unknown> = [];

  public static async RunDBSrc(dbSrc: string, dbType: number, dbSource = "") {
    if (dbSrc == null || dbSrc === "") {
      throw new Error("执行的数据语句不能为空");
    }
    if (dbType == 0) return await this.RunSQLReturnTable(dbSrc, dbSource);
    if (dbType == 1) return this.RunUrlReturnJSON(dbSrc);
    if (dbType == 2) {
      const str = this.RunFunctionReturnStr(dbSrc);
      if (str == null || str == "") {
        return [];
      }
      return JSON.parse(str);
    }
    return [];
  }

  /**
   * 执行SQL语句
   * @param sql 执行的语句
   * @param dbSource 数据源类型 本地和外部数据源
   * @constructor
   */
  public static async RunSQLReturnTable(sql: string, dbSource = "local") {
    if (sql == null || typeof sql === "undefined" || sql == "") {
      throw new Error("数据查询为空，请求无效");
    }
    // if (sql.includes('@')==true)
    const formData = new FormData();
    formData.append("SQL", encodeURIComponent(sql));
    formData.append("DBSrc", dbSource);
    return await request.post<unknown, unknown>(REQUEST_URL, formData, {
      params: {
        DoType: "DBAccess_RunSQLReturnTable",
        t: Date.now(),
      },
    });
  }

  /**
   * 根据URl请求返回字符串数据
   * @param url
   * @constructor
   */
  public static async RunUrlReturnString(url: string) {
    if (url == null || typeof url === "undefined") {
      throw new Error("url为空，请求无效");
    }

    if (url.match(/^http:\/\//) == null) {
      const { VITE_GLOB_API_URL } = getAppEnvConfig();
      url = VITE_GLOB_API_URL + "/" + url;
    }
    const formData = new FormData();
    formData.append("urlExt", url);
    return await request.post<unknown, unknown>(REQUEST_URL, formData, {
      params: {
        DoType: "RunUrlCrossReturnString",
        t: Date.now(),
      },
    });
  }

  /**
   * 根据URl请求返回JSON格式数据
   * @param url
   * @constructor
   */
  public static async RunUrlReturnJSON(url: string) {
    const str = await this.RunUrlReturnString(url);
    if (typeof str == "string") {
      if (str.includes("url为空") == true) {
        this.data = [];
        return;
      }
      if (str.includes("err@") == true) {
        this.data = [];
        throw new Error(str.replace("err@", ""));
      }
      try {
        this.data = JSON.parse(str);
      } catch (e: unknown) {
        this.data = [];
        console.log("RunUrlReturnJSON数据解析失败:" + str);
        throw new Error("值获取失败");
      }
    }
  }
  /**
   * 执行方法
   * @param funcName 方法名称
   * @constructor
   */
  public static RunFunctionReturnStr(funcName: string) {
    //可能需要动态引入js/ts文件
    try {
      funcName = funcName.replace(/~/g, "'");
      if (funcName.indexOf("(") == -1) return eval(funcName + "()");
      else return eval(funcName);
    } catch (e: unknown) {
      if (e.message)
        throw new Error("执行方法[" + funcName + "]错误:" + e.message);
    }
  }
}
