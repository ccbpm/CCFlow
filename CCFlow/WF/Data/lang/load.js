//定义多语言.
//zh-cn   zh-tw  zh-hk  en-us ja-jp ko-kr


$(function () {
    var currentLang = localStorage.getItem("Lange") || 'zh-cn';
    Skip.addJs(basePath + "/WF/Data/lang/js/" + currentLang + ".js");
    loadScript(basePath + "/WF/Data/lang/js/" + currentLang + ".js");
});
function getNameByLange(key) {
    var name = '';
    if (currentLang === 'zh') {
        name = window.lang['zh'][key];
        if (typeof name === 'undefined') {
            alert(key + '在zh-cn.js中未添加');
            return key;
        }
        return name;
    }
   
    if (currentLang === 'en') {
        name = window.lang['en'][key];
        if (typeof name === 'undefined') {
            alert(key + '在en-us.js中未添加');
            return key;
        }
        return name;
    }
    alert('语言' + currentLang + '还未解析');
    return key;
}
