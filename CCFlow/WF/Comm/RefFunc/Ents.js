
function MenuConvertTools(data) {
 
    this.data = data;
}

// 获取系统菜单
MenuConvertTools.prototype.getSystemMenus = function () {

    var endtM = this.data;
    console.log(GetQueryString("EnName"))
    var mypk = new WebUser().No + "_Funcs_HS_" + GetQueryString("EnName");
    var userRegedit = new Entity("BP.Sys.UserRegedit");
    userRegedit.SetPKVal(mypk);
    userRegedit.RetrieveFromDBSources();

    //求出所有的分组名称.
    var systemNode = [];
    var GroupName = [];
    var GroupNames = [];
    var j = 0;
    /*GroupNames[j] = "基本信息";
    Gl = { "Name": "基本信息", "No": "Tno" + j, "Icon": "icon-folder"};
    GroupName.push(Gl);*/
    for (var i = 0; i < endtM.length; i++) {
        var en = endtM[i];
        
        if (en.GroupName == null || en.GroupName == "") 
            continue;
        
        if (GroupNames.indexOf(en.GroupName) == -1) {
            j++;
            GroupNames[j] = en.GroupName;
            
            Gl = { "Name": en.GroupName, "No": "Tno" + j, "Icon":"icon-folder"}
            GroupName.push(Gl);
        }
    }
    if (GroupName.length==0)
        GroupName.push({ "Name": "基本信息", "No": "Tno0", "Icon": "icon-folder" });

    //生成菜单.
    var adminMenuNodes = [];
    for (var i = 0; i < GroupName.length; i++) {
        var systemNode = GroupName[i];
        systemNode.children = [];
        if (i == 0) { systemNode.open = true; } else {
            systemNode.open = false;
        }

       /* var gName = GroupName[i];
        if (gName == "")
            continue;
        if (userRegedit != null && userRegedit != undefined && userRegedit.MVals.indexOf(",dtM_" + gName + ",") != -1)
            continue;
        if (gName.length == 2 && gName.indexOf('基本信息') != -1)
            html += "";
        else
            systemNode=gName;*/


        //填入菜单内容.
        /*html += "<ul class='navlist' >";
        if (gName == "基本信息") {
            html += "<li><a href='javascript:OpenUrlInRightFrame(this,\"" + url + "\")'><span class='pull-right'><i class='iconfont icon-weimingmingwenjianjia_jiantou'></i></span> <i class='iconfont icon-shouye'></i> " + title + "</a></li>";
        }*/

        for (var idx = 0; idx < endtM.length; idx++) {

            var moduleEn = endtM[idx];
            
            if (moduleEn.RefAttrKey != null && moduleEn.RefAttrKey !="")
                continue;
            console.log(moduleEn)
            if (moduleEn.GroupName == null || moduleEn.GroupName == "")
                moduleEn.GroupName = "基本信息";
            if (moduleEn.GroupName != systemNode.Name)
                continue; //如果不是本系统的.
            moduleEn.children = [];
           /* var myName = moduleEn.GroupName;
            if (myName == null || myName == "")
                myName = "基本信息";

            if (gName != myName)
                continue;

            if ((moduleEn.RefAttrKey != null && moduleEn.RefAttrKey != "") || moduleEn.IsCanBatch == "1" || moduleEn.Visable == "0")
                continue;

            if (userRegedit != null && userRegedit != undefined && userRegedit.Vals.indexOf("," + moduleEn.No + ",") != -1)
                continue;
            moduleEn.open = false;
           html += "<li>" + GenerRM(en) + "</li>";*/
            if (!moduleEn.Icon) moduleEn.Icon = "icon-drop";
            if (moduleEn.Icon.indexOf('Img') != -1) {
                moduleEn.Icon = "icon-drop";
            }
            systemNode.children.push(moduleEn);
            //console.log(systemNode.children);
        }
        //console.log(systemNode);
        adminMenuNodes.push(systemNode)
    }  

    return adminMenuNodes
}
MenuConvertTools.prototype.convertToTreeData = function () {
    var topNodes = [];
    
    topNodes = topNodes.concat(this.getSystemMenus(this.data))
  
    return topNodes
}
