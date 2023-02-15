/**
 * **************************** 总体说明 ************************
 * 1. 该接口是菜单权限管理控制接口. 
 * 2. 需要引入到您的页面里，完成菜单的显示，以及功能点的控制.
 * 3. 我们提供了如下3个方法，
 *  3.1 获得菜单目录菜单的 GPM_GenerMenumsDB(appNo)。
 *  3.2 判断是否可以执行特定功能的GPM_IsCanExecuteFunction(appNo,functionFlat)
 *  3.3 自动设置页面元素显示隐藏的.
 */

/**
 * 获得当前操作员的菜单与目录的API
 * @param {系统编号} appNo 
 * 
 * 返回：两个结果集合的JSON,可以通过下列方式获取到他.
    var dirs = data["Dirs"];   //获得目录.
    var menus = data["Menus"]; //获得菜单.
 
 说明：
 1. 系统返回两个API接口. 目录与菜单，分别是两个数据集合。
 2. 目录的数据结构：dirs   No=编号,Name=标签, Icon=图标
 3. 菜单的数据结构：menus  No=编号,Name=标签, Icon=图标，URL=连接, 
    Target=打开方式0=新窗口,1=本窗口,2=覆盖新窗口. ParentNo=目录编码
 4. 您可以自己组织这些数据根据自己的需要生成菜单框架.
 5. ccbpm提供了两套风格，您可以参考 /Portal/GPMMenus.js ， /Portal20/GPMMenus.js
 */
function GPM_GenerMenumsDB() {

    var handler = new HttpHandler("BP.WF.HttpHandler.GPMPage");
    // alert(appNo);
    if (appNo == null) {
        alert('没有配置appNo,或者没有引入config.js 。');
        return;
    }

    handler.AddPara("AppNo", appNo);
    var data = handler.DoMethodReturnJSON("GPM_DB_Menus"); //获得菜单.
    return data;
}

/**
 * 判断当前用户是否可以执行当前的功能点
 * 
 * @param {项目编号} appNo
 * @param {标记} funcFlag
 * 
 * 返回true ,是可以执行这个功能点.  false=不可以执行功能点.
 * 
 * 说明：用于页面的功能点控制, 例如: 
 * 1. 获取是否可以删除的权限.
 * 2. IsCanDeleteUser 是在新建菜单的时候做的标记.
 * 3. 调用接口true,false 用于显示隐藏功能按钮.
 * 
 *  var isCanDeleteUser=GPM_IsCanExecuteFunction('CCOA','DeleteUser');
 *  
 *  if (isCanDeleteUser==false)
 *     $("#Btn_Delete").hid(); 
 *  else
 *     $("#Btn_Delete").show();
 *  
 */
function GPM_IsCanExecuteFunction(appNo, funcFlag) {
    var handler = new HttpHandler("BP.WF.HttpHandler.GPMPage");
    handler.AddPara("AppNo", appNo);
    handler.AddPara("FuncFlag", funcFlag);
    var data = handler.DoMethodReturnJSON("GPM_IsCanExecuteFunction"); //获得菜单.
    if (data == "1")
        return true;
    return false;
}

/**
 * 自动显示隐藏页面元素(批量控制页面元素的显示隐藏元素.)
 * 
 * @param {系统编号} appNo
 * 
 * 应用场景:
 * 1. 首先在前台配置好功能控制点标记，并把功能控制点分给相关的人员, 功能控制点的标记要与要控制的页面元素id对应.
 * 2. 其次需要把/GPM/API.js 引入到要控制的页面里面.
 * 3. 开发的个性化需要控制的元素ID,默认都是隐藏的.
 *   
 */
function GPM_AutoHidShowPageElement(appNo) {
    var handler = new HttpHandler("BP.WF.HttpHandler.GPMPage");
    handler.AddPara("AppNo", appNo);
    var data = handler.DoMethodReturnJSON("GPM_AutoHidShowPageElement"); //获得所有的标记数据.

    for (var i = 0; i < data.length; i++) {
        var ctrl = data[i].Flag;
        var ctl = $("#" + ctrl);
        if (ctl == null)
            continue;
        ctl.show(); //让其显示出来.
    }

}

