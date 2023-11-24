// 生成where参数
export function createWhereArgs(queryArgs: string[]) {
  let str = '';
  if (queryArgs.length < 2) {
    return '';
  }
  if (queryArgs.length % 2 === 1) {
    for (let i = 0; i < queryArgs.length - 1; i += 2) {
      str += `@${queryArgs[i]}=${queryArgs[i + 1]}`;
    }
    str += `@orderBy=${queryArgs[queryArgs.length - 1]}`;
    return str;
  }
  for (let i = 0; i < queryArgs.length; i += 2) {
    str += `@${queryArgs[i]}=${queryArgs[i + 1]}`;
  }
  return str;
}

// 将AtPara 参数解析为Map
export function decodeExtraParams(AtPara: string) {
  const params = new Map();
  if (!AtPara) return params;
  if (AtPara.startsWith('@')) {
    const tempArr = splitAtString(AtPara);
    tempArr.forEach((temp: string) => {
      const [key, val] = temp.split('=');
      params.set(key, val);
    });
  }
  return params;
}

// 处理包含@符号的字符串
export function splitAtString(str: string) {
  str = str.trim();
  const firstSymbolIndex = str.indexOf('@');
  if (firstSymbolIndex < 0) {
    return [];
  }
  return str
    .substring(firstSymbolIndex + 1)
    .split('@')
    .filter((item) => item !== '');
}
