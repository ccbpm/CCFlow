
function GetDBGroup() {

    var json = [

        { "No": "LCode", "Name": "低代码" },
        { "No": "Flow", "Name": "流程设计" },
        { "No": "Frm", "Name": "表单模板" },
        { "No": "System", "Name": "系统管理" }
    ];
    return json;
}

function GetDBDtl() {

    var json = [

        { "No": "1", "Name": "菜单体系", "GroupNo": "LCode", "Url": "/WF/GPM/SystemList.htm", "Icon":"icon-grid" },
        { "No": "2", "Name": "新建系统", "GroupNo": "LCode", "Url": "/WF/GPM/NewSystem.htm", "Icon": "icon-folder"},

        { "No": "3", "Name": "流程模板", "GroupNo": "Flow", "Url": "/WF/Comm/Search.htm?EnsName=BP.WF.Admin.Flows/WF/GPM/NewSystem.htm", "Icon": "icon-grid" },
        { "No": "4", "Name": "模板目录", "GroupNo": "Flow", "Url": "/WF/Comm/Ens.htm?EnsName=BP.WF.Admin.FlowSorts", "Icon": "icon-folder" },
        { "No": "5", "Name": "流程实例", "GroupNo": "Flow", "Url": "/WF/Comm/Search.htm?EnsName=BP.WF.Data.GenerWorkFlowViews", "Icon": "icon-layers"},
        { "No": "6", "Name": "流程分析", "GroupNo": "Flow", "Url": "/WF/Comm/Group.htm?EnsName=BP.WF.Data.GenerWorkFlowViews", "Icon": "icon-chart" },

        { "No": "7", "Name": "表单模板", "GroupNo": "Frm", "Url": "/WF/Comm/Search.htm?EnsName=BP.WF.Admin.Frms", "Icon": "icon-chart" },
        { "No": "8", "Name": "目录", "GroupNo": "Frm", "Url": "/WF/Comm/Ens.htm?EnsName=BP.WF.Admin.FrmSorts", "Icon": "icon-chart"},
        { "No": "9", "Name": "数据源", "GroupNo": "Frm", "Url": "/WF/Comm/Search.htm?EnsName=BP.Sys.SFDBSrcs", "Icon": "icon-chart" },
        { "No": "10", "Name": "枚举", "GroupNo": "Frm", "Url": "/WF/Comm/Search.htm?EnsName=BP.Sys.EnumMains", "Icon": "icon-chart" },
        { "No": "11", "Name": "外键", "GroupNo": "Frm", "Url": "/WF/Comm/Search.htm?EnsName=BP.Sys.SFTables", "Icon": "icon-chart" },

        { "No": "12", "Name": "组织结构", "GroupNo": "System", "Url": "/GPM/Organization.htm", "Icon": "icon-chart" },
        { "No": "13", "Name": "岗位", "GroupNo": "System", "Url": "/WF/Comm/Search.htm?EnsName=BP.Port.Stations", "Icon": "icon-chart" },
        { "No": "14", "Name": "岗位类型", "GroupNo": "System", "Url": "/WF/Comm/Ens.htm?EnsName=BP.Port.StationTypes", "Icon": "icon-chart" },
        { "No": "15", "Name": "部门", "GroupNo": "System", "Url": "/WF/Comm/Search.htm?EnsName=BP.Port.Depts","Icon": "icon-chart"},
        

    ];
    return json;
}