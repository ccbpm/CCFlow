import { PageBaseGenerList } from './PageBaseGenerList';
import { dealClassId } from './utils/StringUtils';
const CLASS_PREFIX = '/GL_';
// const allTsInfo = import.meta.glob('/src/**/*.ts');
const allTsInfo = require.context('./', false, /\.ts&/);
const glTsKeys = Object.keys(allTsInfo).filter((key) => key.includes(CLASS_PREFIX));
const modules: Record<string,any> = {};

glTsKeys.forEach((glTsKey) => {
  modules[glTsKey] = allTsInfo[glTsKey];
});

export class ClassFactoryOfGenerList {
  public static async GetEn(classID: string): Promise<PageBaseGenerList> {
    classID = dealClassId(classID, CLASS_PREFIX);
    const filePath = glTsKeys.find((key) => key.includes(classID + '.ts'));
    if (!filePath) {
      throw new Error('GL-GetEn: 没有找到文件路径' + classID);
    }
    const loader = modules[filePath];
    if (!loader) {
      throw new Error('ClassFactoryOfGenerList没有判断的类名:' + classID);
    }
    const esModule = (await loader()) as object;
    const obj = esModule?.[classID];
    if (!obj) {
      const keys = Object.keys(esModule);
      const message = `GL-GetEn 文件 [${filePath}] 中 没有正确导出名为 ${classID} 的模块，现有模块名为 [${[...keys]}]  请检查`;
      alert(message);
      throw new Error(message);
    }
    return new obj();
  }
  public static async toJSON(filteredPrefix: string[]) {
    const pathList = Object.keys(modules).map((key) => dealClassId(key, CLASS_PREFIX));
    const loaders = pathList
      .filter((path) => {
        for (const prefix of filteredPrefix) {
          if (path.startsWith(prefix)) return false;
        }
        return true;
      })
      .map(async (classID) => {
        const obj = await this.GetEn(classID);
        return {
          No: classID,
          Name: obj.PageTitle + `(${classID})`,
        };
      });
    const result = await Promise.allSettled(loaders);
    return JSON.stringify(result.filter((p) => p.status === 'fulfilled').map((r) => (r.status !== 'rejected' ? r?.value : '')));
  }
}
