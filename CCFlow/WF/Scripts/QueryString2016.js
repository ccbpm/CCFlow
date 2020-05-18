//获取QueryString的数组 
function getQueryString() {
    var result = location.search.match(new RegExp("[\?\&][^\?\&]+=[^\?\&]+", "g"));
    for (var i = 0; i < result.length; i++) {
        result[i] = result[i].substring(1);
    }
    return result;
}
//根据QueryString参数名称获取值 
function getQueryStringByName(name) {
    var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
    if (result == null || result.length < 1) {
        return "";
    }
    return result[1];
}
//根据QueryString参数索引获取值 
function getQueryStringByIndex(index) {
    if (index == null) {
        return "";
    }
    var queryStringList = getQueryString();
    if (index >= queryStringList.length) {
        return "";
    }
    var result = queryStringList[index];
    var startIndex = result.indexOf("=") + 1;
    result = result.substring(startIndex);
    return result;
}
//Other
function GetQueryString(name) {

    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");

    var r = window.location.search.substr(1).match(reg);

    if (r != null) return decodeURI(r[2]); return null;

}

//软通杨玉慧 通过URL获取它的一些参数值
//获取QueryString的数组 
function getQueryStringFromUrl(url) {
    if (url.indexOf('?') >= 0) {
        url = url.substring(url.indexOf('?'));
        var result = url.match(new RegExp("[\?\&][^\?\&]+=[^\?\&]+", "g"));
        if (result != undefined) {
            for (var i = 0; i < result.length; i++) {
                result[i] = result[i].substring(1);
            }
            return result;
        }
        else {
            return [];
        }
    }
    else {
        return [];
    }
}
//根据QueryString参数名称获取值 
function getQueryStringByNameFromUrl(url, name) {
    if (url.indexOf('?') >= 0) {
        url = url.substring(url.indexOf('?'));
        var result = url.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
        if (result == null || result.length < 1) {
            return "";
        }
        return result[1];
    }
    else {
        return "";
    }
}
//根据QueryString参数索引获取值 
function getQueryStringByIndexFromUrl(url, index) {

    if (index == null) {
        return "";
    }
    var queryStringList = getQueryStringFromUrl(url);
    if (index >= queryStringList.length) {
        return "";
    }
    var result = queryStringList[index];
    var startIndex = result.indexOf("=") + 1;
    result = result.substring(startIndex);
    return result;
}
//Other
function GetQueryStringFromUrl(name) {
    if (url.indexOf('?') >= 0) {
        url = url.substring(url.indexOf('?'));
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");

        var r = url.substr(1).match(reg);

        if (r != null) return decodeURI(r[2]); return null;
    }
    else {
        return null;
    }
}



