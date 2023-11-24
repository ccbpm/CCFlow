export function decodeResponseParams(response: string) {
  try {
    const obj: Recordable = {};
    const url = response.trim().replace('url@', '');
    const args = url.split('?');
    //获取到页面名称
    let pageName = args[0].substring(args[0].lastIndexOf('/') + 1) || '';
    pageName = pageName.replace('.htm', '').replace('.html', '');
    if (args.length < 2 || !args[1].trim()) {
      return { PageName: pageName };
    }
    obj['PageName'] = pageName;
    args[1].split('&').forEach((arg: string) => {
      const [key, val] = arg.split('=');
      obj[key] = val;
    });
    return obj;
  } catch (e) {
    return {};
  }
}

export function getRequestParams(key: string) {
  if (!window) {
    return '';
  }
  const str = window.location.href;
  if (!str.includes('?')) {
    return '';
  }
  const args = str.substring(1, str.length).split('&');
  for (const arg of args) {
    const [k, v] = arg.split('=');
    if (k === key) return v;
  }
  return '';
}

export function getAllRequestParams(url: string) {
  if (!url.includes('?')) {
    return {};
  }
  const str = url.split('?')[1];
  if (!str) {
    return {};
  }
  const args = str.split('&');
  const requestParams: Record<string, any> = {};
  for (const arg of args) {
    const [k, v] = arg.split('=');
    requestParams[k] = v;
  }
  return requestParams;
}
