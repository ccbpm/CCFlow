
function MenuConvertTools(data) {

    this.data = data;
}

// 获取系统菜单
MenuConvertTools.prototype.getSystemMenus = function () {

    var endtM = this.data;
    console.log(endtM);

    //求出所有的分组名称.
    //生成菜单.
    var adminMenuNodes = [];


    for (var idx = 0; idx < endtM.length; idx++) {

        var moduleEn = endtM[idx];
        adminMenuNodes.push(moduleEn)
    }

    return adminMenuNodes
}
MenuConvertTools.prototype.convertToTreeData = function () {
    var topNodes = [];
    //console.log(this.data)
    topNodes = topNodes.concat(this.getSystemMenus(this.data))

    return topNodes
}
