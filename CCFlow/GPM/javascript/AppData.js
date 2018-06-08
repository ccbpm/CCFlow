var DefuaultUrl = "Base/DataService.aspx";

ccflow = {}
Application = {}

DataFactory = function () {
    this.data = new ccflow.Data(DefuaultUrl);
    this.common = new ccflow.common();
}

jQuery(function ($) {

    Application = new DataFactory();

});
//公共方法
ccflow.common = function () {
    //sArgName表示要获取哪个参数的值
    this.getArgsFromHref = function (sArgName) {
        var sHref = window.location.href;
        var args = sHref.split("?");
        var retval = "";
        if (args[0] == sHref) /*参数为空*/
        {
            return retval; /*无需做任何处理*/
        }
        var str = args[1];
        args = str.split("&");
        for (var i = 0; i < args.length; i++) {
            str = args[i];
            var arg = str.split("=");
            if (arg.length <= 1) continue;
            if (arg[0] == sArgName) retval = arg[1];
        }
        return retval;
    }
}
//数据访问
ccflow.Data = function (url) {
    this.url = url;
    //获取所有系统
    this.getApps = function (callback, scope) {
        var tUrl = this.url;
        var params = { method: "getapps" };
        queryData(tUrl, params, callback, scope);
    }
    //获取左侧菜单
    this.getLeftMenu = function (callback, scope) {
        var tUrl = this.url;
        var params = { method: "getleftmenu" };
        queryData(tUrl, params, callback, scope);
    }
    //根据系统编号获取系统菜单
    this.getAppMenus = function (appName, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "getsystemmenu",
            appName: appName
        };
        queryData(tUrl, params, callback, scope);
    }
    //获取所有菜单
    this.getMenus = function (callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "getmenus"
        };
        queryData(tUrl, params, callback, scope);
    }
    //按菜单分配权限
    this.getMenusOfMenuForEmp = function (parentNo, isLoadChild, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "getmenusofmenuforemp",
            parentNo: parentNo,
            isLoadChild: isLoadChild
        };
        queryData(tUrl, params, callback, scope);
    }
    //操作节点
    this.nodeManage = function (nodeNo, dowhat, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "menunodemanage",
            nodeNo: nodeNo,
            dowhat: dowhat
        };
        queryData(tUrl, params, callback, scope);
    }
    //根据用户编号获取菜单
    this.getMenuByEmpNo = function (empNo, appName, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "getmenubyempno",
            fk_emp: empNo,
            fk_app: appName
        };
        queryData(tUrl, params, callback, scope);
    }
    //获取菜单根据编号
    this.getMenusById = function (id, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "getmenusbyid",
            Id: id
        };
        queryData(tUrl, params, callback, scope);
    }
    //获取所有人员信息
    this.getEmps = function (callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "getemps"
        };
        queryData(tUrl, params, callback, scope);
    }
    //根据用户名或编号模糊查找用户
    this.getEmpsByNoOrName = function (objSearch, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "getempsbynoorname",
            objSearch: objSearch
        };
        queryData(tUrl, params, callback, scope);
    }
    //根据用户名或部门模糊查找用户
    this.getEmpsByNameOrDept = function (deptNo,objSearch, pageNumber, pageSize, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "getempsbynameordept",
            deptNo: deptNo,
            objSearch: objSearch,
            pageNumber: pageNumber,
            pageSize: pageSize
        };
        queryData(tUrl, params, callback, scope);
    }
    //根据用户账号、工号、姓名或手机号
    this.searchByEmpNoOrName = function (objSearch, pageNumber, pageSize, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "searchbyempnoorname",
            objSearch: objSearch,
            pageNumber: pageNumber,
            pageSize: pageSize
        };
        queryData(tUrl, params, callback, scope);
    }
    //获取所有权限组
    this.getEmpGroups = function (callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "getempgroups"
        };
        queryData(tUrl, params, callback, scope);
    }
    //权限组模糊查找
    this.getEmpGroupsByName = function (objSearch, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "getempgroupsbyname",
            objSearch: objSearch
        };
        queryData(tUrl, params, callback, scope);
    }
    //获取部门人员信息
    this.getDeptEmpTree = function (callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "getdeptemptree"
        };
        queryData(tUrl, params, callback, scope);
    }
    //获取子部门人员
    this.getDeptEmpChildNodes = function (nodeNo, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "getdeptempchildnodes",
            nodeNo: nodeNo
        };
        queryData(tUrl, params, callback, scope);
    }
    //保存用户与菜单关系
    this.saveUserOfMenus = function (empNo, menuIds, menuIdsUn, menuIdsUnExt, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "saveuserofmenus",
            empNo: empNo,
            menuIds: menuIds,
            menuIdsUn: menuIdsUn,
            menuIdsUnExt: menuIdsUnExt
        };
        queryPostData(tUrl, params, callback, scope);
    }
    //用户菜单权限
    this.getEmpOfMenusByEmpNo = function (empNo, parentNo, isLoadChild, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "getempofmenusbyempno",
            empNo: empNo,
            parentNo: parentNo,
            isLoadChild: isLoadChild
        };
        queryData(tUrl, params, callback, scope);
    }
    //获取权限组菜单
    this.getEmpGroupOfMenusByNo = function (groupNo, parentNo, isLoadChild, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "getempgroupofmenusbyno",
            groupNo: groupNo,
            parentNo: parentNo,
            isLoadChild: isLoadChild
        };
        queryData(tUrl, params, callback, scope);
    }
    //保存权限组菜单
    this.saveUserGroupOfMenus = function (groupNo, menuIds, menuIdsUn, menuIdsUnExt, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "saveusergroupofmenus",
            groupNo: groupNo,
            menuIds: menuIds,
            menuIdsUn: menuIdsUn,
            menuIdsUnExt: menuIdsUnExt
        };
        queryPostData(tUrl, params, callback, scope);
    }
    //清空式复制用户权限
    this.clearOfCopyUserPower = function (copyUser, pastUsers, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "clearofcopyuserpower",
            copyUser: copyUser,
            pastUsers: pastUsers
        };
        queryData(tUrl, params, callback, scope);
    }
    //覆盖式复制用户权限
    this.coverOfCopyUserPower = function (copyUser, pastUsers, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "coverofcopyuserpower",
            copyUser: copyUser,
            pastUsers: pastUsers
        };
        queryData(tUrl, params, callback, scope);
    }
    //清空式权限组权限
    this.clearOfCopyUserGroupPower = function (copyGroupNo, pastGroupNos, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "clearofcopyusergrouppower",
            copyGroupNo: copyGroupNo,
            pastGroupNos: pastGroupNos
        };
        queryData(tUrl, params, callback, scope);
    }
    //覆盖权限组权限
    this.coverOfCopyUserGroupPower = function (copyGroupNo, pastGroupNos, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "coverofcopyusergrouppower",
            copyGroupNo: copyGroupNo,
            pastGroupNos: pastGroupNos
        };
        queryData(tUrl, params, callback, scope);
    }
    //打开子菜单
    this.getAppChildMenus = function (appname, no, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "getAppChildMenus",
            appname: appname,
            no: no
        };
        queryData(tUrl, params, callback, scope);
    }
    //获取所有部门 2013-09-24
    this.getAllDept = function (callback, scope) {
        var tUrl = this.url;
        var params = { method: "GetAllDept" };
        queryData(tUrl, params, callback, scope);
    }
    //按菜单分配权限，获取模版数据
    this.getTemplateData = function (menuNo, model, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "gettemplatedata",
            menuNo: menuNo,
            model: model
        };
        queryData(tUrl, params, callback, scope);
    }
    //保存按菜单分配权限
    this.saveMenuForEmp = function (menuNo, ckNos, model, saveChildNode, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "savemenuforemp",
            menuNo: menuNo,
            ckNos: ckNos,
            model: model,
            saveChildNode: saveChildNode
        };
        queryPostData(tUrl, params, callback, scope);
    }
    //获取系统登录日志
    this.getSystemLoginLogs = function (callback, scope, startdate, enddate) {
        var tUrl = this.url;
        var params = { method: "getsystemloginlogs", startdate: startdate, enddate: enddate };
        queryData(tUrl, params, callback, scope);
    }
    //获取所有岗位  2013-12-30 Ｈ
    this.getStations = function (callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "getstations"
        };
        queryData(tUrl, params, callback, scope);
    }
    //保存岗位菜单 2013-12-30 Ｈ
    this.saveStationOfMenus = function (stationNo, menuIds, menuIdsUn, menuIdsUnExt, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "savestationofmenus",
            stationNo: stationNo,
            menuIds: menuIds,
            menuIdsUn: menuIdsUn,
            menuIdsUnExt: menuIdsUnExt
        };
        queryPostData(tUrl, params, callback, scope);
    }
    //获取岗位 菜单 2013-12-30 Ｈ
    this.getStationOfMenusByNo = function (stationNo, parentNo, isLoadChild, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "getstationofmenusbyno",
            stationNo: stationNo,
            parentNo: parentNo,
            isLoadChild: isLoadChild
        };
        queryData(tUrl, params, callback, scope);
    }
    //清空式 复制岗位 权限 2013-12-31 Ｈ
    this.clearOfCopyStation = function (copyStationNo, pastStationNos, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "clearofcopystation",
            copyStationNo: copyStationNo,
            pastStationNos: pastStationNos
        };
        queryData(tUrl, params, callback, scope);
    }
    //覆盖式 复制岗位 权限  2013-12-31 Ｈ
    this.coverOfCopyStation = function (copyStationNo, pastStationNos, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "coverofcopystation",
            copyStationNo: copyStationNo,
            pastStationNos: pastStationNos
        };
        queryData(tUrl, params, callback, scope);
    }
    //岗位 模糊查找 2013-12-31 Ｈ
    this.getStationByName = function (stationName, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "getstationbyname",
            stationName: stationName
        };
        queryData(tUrl, params, callback, scope);
    }
    //获取
    this.getAppTreeForAdmin = function (rootNo, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "getapptreeforadmin",
            rootNo: rootNo
        };
        queryData(tUrl, params, callback, scope);
    }
    //二级管理员--部门管理 qin
    this.getDeptTreeForAdmin = function (rootNo, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "getManagerDept",
            rootNo: rootNo
        };
        queryData(tUrl, params, callback, scope);
    }
    this.getOrganizationTreeForAdmin = function (rootNo, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "getOrganizationDept",
            rootNo: rootNo
        };
        queryData(tUrl, params, callback, scope);
    }
    //获取该系统的管理员列表
    this.LoadDataGridEmpApp = function (menuNo, orderBy, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "loaddatagridempapp",
            menuNo: menuNo,
            orderBy: orderBy
        };
        queryData(tUrl, params, callback, scope);
    }
    //获取该部门的管理员列表
    this.loaddatagridDeptManager = function (deptNo, orderBy, pageNumber, pageSize, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "loaddatagridDeptManager",
            deptNo: deptNo,
            orderBy: orderBy,
            pageNumber: pageNumber,
            pageSize: pageSize
        };
        queryData(tUrl, params, callback, scope);
    }
    //获取该部门的人员列表
    this.LoadDataGridDeptEmp = function (deptNo, orderBy, searchText, pageNumber, pageSize, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "loaddatagriddeptemp",
            deptNo: deptNo,
            orderBy: orderBy,
            searchText: encodeURI(searchText),
            pageNumber: pageNumber,
            pageSize: pageSize
        };
        queryData(tUrl, params, callback, scope);
    }
    //保存系统管理员
    this.saveEmpApp = function (emps, menuNo, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "saveempapp",
            emps: emps,
            menuNo: menuNo
        };
        queryData(tUrl, params, callback, scope);
    }
    //保存部门管理员
    this.saveDeptManager = function (emps, deptNo, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "saveDeptManager",
            emps: emps,
            deptNo: deptNo
        };
        queryData(tUrl, params, callback, scope);
    }
    //保存新增人员信息
    this.saveDeptEmp = function (infoStr, deptNo, empStationStr, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "savedeptemp",
            infoStr: infoStr,
            deptNo: deptNo,
            empStationStr: empStationStr
        };
        queryData(tUrl, params, callback, scope);
    }
    //删除系统管理员
    this.deleteEmpApp = function (MyPKList, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "deleteempapp",
            MyPKList: MyPKList
        };
        queryData(tUrl, params, callback, scope);
    }
    //删除部门管理员
    this.deleteEmpDept = function (MyPKList, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "deleteempdept",
            MyPKList: MyPKList
        };
        queryData(tUrl, params, callback, scope);
    }
    //删除部门人员
    this.deleteDeptEmp = function (deptNo, emps, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "deletedeptemp",
            emps: emps,
            deptNo: deptNo
        };
        queryData(tUrl, params, callback, scope);
    }
    //禁用人员
    this.disableDeptEmp = function (emps, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "disabledeptemp",
            emps: emps
        };
        queryData(tUrl, params, callback, scope);
    }
    //获取已禁用用户列表
    this.generDisableEmps = function (pageNumber, pageSize, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "generdisableemps",
            pageNumber: pageNumber,
            pageSize: pageSize
        };
        queryData(tUrl, params, callback, scope);
    }
    //调整人员主部门
    this.replaceEmpBelongDept = function (FK_Dept, FK_Emp, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "replaceempbelongdept",
            FK_Dept: FK_Dept,
            FK_Emp: FK_Emp
        };
        queryData(tUrl, params, callback, scope);
    }
    //新增部门--qin 15/7/6
    this.appendData = function (deptSort, deptNo, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "appendDataMet",
            deptNo: deptNo,
            deptSort: deptSort
        };
        queryData(tUrl, params, callback, scope);
    }
    //删除选中的部门节点
    this.deleteNode = function (deptNo, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "deleteNodeMet",
            deptNo: deptNo
        };
        queryData(tUrl, params, callback, scope);
    }
    //上/下移 操作
    this.floatNode = function (upOrDown, selectedNodeId, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "floatNodeMet",
            selectedNodeId: selectedNodeId,
            floatWay: upOrDown
        };
        queryData(tUrl, params, callback, scope);
    }
    //上/下移（人员） 操作
    this.EmpFloatNode = function (upOrDown, curEmpNo, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "EmpFloatNodeMet",
            selectedEmpNo: curEmpNo,
            floatWay: upOrDown
        };
        queryData(tUrl, params, callback, scope);
    }
    //获取部门基本信息
    this.checkDeptInfo = function (selectedNodeId, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "checkDeptInfoMet",
            selectedNodeId: selectedNodeId
        };
        queryData(tUrl, params, callback, scope);
    }
    //获取tabs部门岗位
    this.checkDeptStationInfo = function (selectedNodeId, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "checkDeptStationInfoMet",
            selectedNodeId: selectedNodeId
        };
        queryData(tUrl, params, callback, scope);
    }
    //获取tabs部门职位
    this.checkDeptDutyInfo = function (selectedNodeId, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "checkDeptDutyInfoMet",
            selectedNodeId: selectedNodeId
        };
        queryData(tUrl, params, callback, scope);
    }
    //保存数据
    this.saveDeptInfo = function (deptNo, deptParentNo, deptName, deptLeader, stationTreeNodesStr, deptDutyTreeNodesStr, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "saveDeptInfoMet",
            deptName: encodeURI(deptName),
            deptLeader: encodeURI(deptLeader),
            deptNo: deptNo,
            deptParentNo: deptParentNo,
            stationStr: stationTreeNodesStr,
            dutyStr: deptDutyTreeNodesStr
        };
        queryData(tUrl, params, callback, scope);
    }
    //查询
    this.doSearch = function (selectedNodeId, searchVal, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "doSearchMet",
            selectedNodeId: selectedNodeId,
            searchVal: encodeURI(searchVal)
        };
        queryData(tUrl, params, callback, scope);
    }
    //人员信息
    this.getEmpInfo = function (selectedNodeId, empNo, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "getEmpInfoMet",
            selectedNodeId: selectedNodeId,
            empNo: empNo
        };
        queryData(tUrl, params, callback, scope);
    }
    //保存修改信息
    this.editDeptEmp = function (selectedNodeId, info, selectEmpNo, stationTreeNodesStr, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "editDeptEmpMet",
            selectEmpNo: selectEmpNo,
            info: info,
            stationTreeNodesStr: stationTreeNodesStr,
            selectedNodeId: selectedNodeId
        };
        queryData(tUrl, params, callback, scope);
    }
    //加载人员岗位tree
    this.getEmpStationInfo = function (selectedNodeId, selectEmpNo, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "getEmpStationInfoMet",
            selectEmpNo: selectEmpNo,
            selectedNodeId: selectedNodeId
        };
        queryData(tUrl, params, callback, scope);
    }
    //检查是否重名
    this.checkEmpNo = function (empNo, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "checkEmpNoMet",
            empNo: encodeURI(empNo)
        };
        queryData(tUrl, params, callback, scope);
    }
    //添加人员职务下拉框
    this.getDutyStationDllInfo = function (selectedNodeId, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "getDutyStationDllInfoMet",
            selectedNodeId: selectedNodeId
        };
        queryData(tUrl, params, callback, scope);
    }
    //关联人员
    this.getOtherDeptEmps = function (pageNumber, pageSize, deptNo, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "getOtherEmpsMet",
            deptNo: deptNo,
            pageNumber: pageNumber,
            pageSize: pageSize
        };
        queryData(tUrl, params, callback, scope);
    }
    //保存关联人员数据
    this.glEmp = function (deptNo, empNoStr, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "glEmpMet",
            deptNo: deptNo,
            empNoStr: empNoStr
        };
        queryData(tUrl, params, callback, scope);
    }
    //密码重置
    this.modifyPwd = function (empNo, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "modifyPwdMet",
            empNo: empNo
        };
        queryData(tUrl, params, callback, scope);
    }
    //检查该部门职务和岗位是否健全
    this.checkDeptDutyAndStation = function (deptNo, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "checkDeptDutyAndStationMet",
            deptNo: deptNo
        };
        queryData(tUrl, params, callback, scope);
    }
    //岗位类型与岗位组成的树形数据
    this.generStationTree = function (callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "generstationtree"
        };
        queryPostData(tUrl, params, callback, scope);
    }
    //岗位下的人员权限，获取模版数据
    this.getDeptEmpStationTemplateData = function (FK_Station, FK_Dept, ViewModel, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "getdeptempstationtemplatedata",
            FK_Station: FK_Station,
            FK_Dept: FK_Dept,
            ViewModel: ViewModel
        };
        queryData(tUrl, params, callback, scope);
    }
    //保存岗位所选的人员
    this.saveStationForDeptEmps = function (FK_Station, FK_Dept, Vals, IsClearSave, callback, scope) {
        var tUrl = this.url;
        var params = {
            method: "savestationfordeptemps",
            FK_Station: FK_Station,
            FK_Dept: FK_Dept,
            Vals: Vals,
            IsClearSave: IsClearSave
        };
        queryPostData(tUrl, params, callback, scope);
    }
    //公共方法
    function queryData(url, param, callback, scope, method, showErrMsg) {
        if (!method) method = 'GET';
        $.ajax({
            type: method, //使用GET或POST方法访问后台
            dataType: "text", //返回json格式的数据
            contentType: "application/json; charset=utf-8",
            url: url, //要访问的后台地址
            data: param, //要发送的数据
            async: true,
            cache: false,
            complete: function () { }, //AJAX请求完成时隐藏loading提示
            error: function (XMLHttpRequest, errorThrown) {
                callback(XMLHttpRequest);
            },
            success: function (msg) {//msg为返回的数据，在这里做数据绑定
                var data = msg;
                callback(data, scope);
            }
        });
    }

    //公共方法
    function queryPostData(url, param, callback, scope) {
        $.post(url, param, callback);
    }
}