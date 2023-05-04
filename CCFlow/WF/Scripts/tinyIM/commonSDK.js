/** 判断js或者css是否引入 */
function isInclude (name) {
  var js = /js$/i.test(name);
  var es = document.getElementsByTagName(js ? 'script' : 'link');
  for (var i = 0; i < es.length; i++) 
  if (es[i][js ? 'src' : 'href'].indexOf(name) != -1) return true;
  return false;
}

/** 引入js文件 */
function addScript(url) {
	var script = document.createElement('script')
	script.setAttribute('type','text/javascript')
	script.setAttribute('src', url)
	document.getElementsByTagName('head')[0].appendChild(script)
}

/** 引入css文件 */
function addStyle(url) {
  var script = document.createElement('link')
	script.setAttribute('rel','stylesheet')
	script.setAttribute('href', url)
	document.getElementsByTagName('head')[0].appendChild(script)
}

/** 判断是pc环境 */
const isPC = !(/Android|webOS|iPhone|iPod|BlackBerry/i.test(navigator.userAgent))
if (isPC) {
  if (!isInclude('umgrid.tinyim.js')) {
    addScript('https://sdk.umnet.cn/umviewsdk/umgrid.tinyim.js')
  }
  if (!isInclude('umgrid.tinyim.css')) {
    addStyle('https://sdk.umnet.cn/umviewsdk/static/css/umgrid.tinyim.css')
  }
} else {
  if (!isInclude('mobile.tinyim.js')) {
    addScript('https://sdk.umnet.cn/umviewsdk/mobile.tinyim.js')
  }
  if (!isInclude('mumgrid.tinyim.css')) {
    addStyle('https://sdk.umnet.cn/umviewsdk/static/css/mumgrid.tinyim.css')
  }
}