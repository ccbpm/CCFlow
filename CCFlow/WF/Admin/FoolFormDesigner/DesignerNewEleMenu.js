var eleMenus = [{
     "id":"Base",
     "title": "基本字段",
    "icon": "iconfont icon-ziduan",
    "child": [
        {"id":"TextBox","title": "文字文本框", "icon": "icon-user", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        {"id":"TextBoxInt","title": "整数", "icon": "icon-user", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        {"id":"TextBoxFloat","title": "数值", "icon": "icon-user", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "TextBoxMoney", "title": "金额", "icon": "icon-wallet", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "Date", "title": "日期", "icon": "icon-calendar", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "DateTime", "title": "日期时间", "icon": "icon-calendar", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        {"id":"CheckBox", "title": "复选框", "icon": "icon-user", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        {type: '-'},
        { "id": "Radio", "title": "单选按钮", "icon": "icon-menu", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "RadioSelect", "title": "枚举下拉框", "icon": "icon-options", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "RadioCheckBox", "title": "枚举多选按钮", "icon": "icon-list", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        {type: '-'},
        { "id": "Select", "title": "外键,外部数据源下拉框", "icon": "icon-list", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "WorkCheck", "title": "签批组件", "icon": "icon-check", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 }
    ]},{
    "id":"Ath",
    "title": "附件",
    "icon": "iconfont icon-fujian",
    "child": [
        { "id": "FieldAth", "title": "字段附件", "icon": "icon-paper-clip", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        {"id":"TableAth",  "title": "表格附件", "icon": "icon-user", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "ImgAth", "title": "图片附件", "icon": "icon-camera", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "HandlerWriting", "title": "写字板", "icon": "icon-note", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 }
    ]},{
    "id":"Dtl",
    "title": "从表",
    "icon": "iconfont icon-tianjiashujubiao"
    },{
    "id":"Group",
    "title": "字段分组",
    "icon": "iconfont icon-fenzu",
    "child": [
        { "id": "GroupFWC", "title": "审核字段分组", "icon": "icon-folder-alt", "MethodType": "Func", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "GroupField", "title": "字段分组", "icon": "icon-folder", "MethodType": "Func", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 }
    ]},{
    "id":"Component",
    "title": "通用组件",
    "icon": "iconfont icon-zujian",
    "child": [
        {"id":"IDCard", "title": "身份证上传", "icon": "icon-user", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "Button", "title": "按钮", "icon": "icon-social-youtube", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "Link", "title": "超链接", "icon": "icon-share", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "Score", "title": "评分", "icon": "icon-badge", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "IFrame", "title": "框架", "icon": "icon-frame", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "Map", "title": "地图", "icon": "icon-globe", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "Fiexed", "title": "系统定位", "icon": "icon-location-pin", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "BigText", "title": "大块html说明", "icon": "icon-bag", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 }
    ]},{
    "id":"Node",
    "title": "节点组件",
    "icon": "iconfont icon-jiedian",
    "child": [
        { "id": "NodeWorkCheck", "title": "审核组件", "icon": "icon-check", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "SubFlow", "title": "父子流程组件", "icon": "icon-shuffle", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "Thread", "title": "子线程组件", "icon": "icon-equalizer", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "Track", "title": "轨迹组件", "icon": "icon-graph", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "Transfer", "title": "流转自定义组件", "icon": "icon-puzzle", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 }
    ]}
];
/*
*转成菜单下拉框
*/
function CovertEleMenuToDropdownMenu(){
    $.each(eleMenus,function(i,eleMenu){
        if(eleMenu.templet!="")
            eleMenu.templet = '<i class="'+eleMenu.icon+'" style="padding-right:3px"></i>'+eleMenu.title;
        if(eleMenu.child && eleMenu.child.length!=0){
            eleMenu.child.forEach(function(item){
                item.templet = '<i class="'+item.icon+'" style="padding-right:3px"></i>'+item.title;
            });
        }
        
    });
   return eleMenus;
}
/*
* 菜单单击时执行的事件
*/
function MenuClick(type,pinyin){
    switch (type) {
        case "TextBox":
        case "TextBoxInt":
        case "TextBoxFloat":
        case "TextBoxMoney":
        case "Date":
        case "DateTime":
        case "CheckBox":
            AddF(type);  //基本字段
            break;
        case "WorkCheck"://签批字段
            AddWorkCheck(pinyin);
            break;
        case "Radio":
        case "RadioSelect":
        case "RadioCheckBox":
            AddEnum(type);//枚举
            break;
        case "Select": //下拉框
            AddSelect();
            break;
        case "FieldAth": //字段附件
            AddFieldAth(pinyin);
            break;
        case "TableAth"://表格附件
            AddAth();
            break;
        case "ImgAth"://图片附件
            AddImgAth(pinyin);
            break;
        case "HandlerWriting"://写字板
            AddHandWriting(pinyin);
            break;
        case "GroupFWC"://审核分组
            AddGroupFWC(pinyin);
            break;
        case "GroupField"://字段分组
            AddGroupField();
            break;
        case "Dtl"://从表
            NewMapDtl();
            break;
        case "IDCard"://身份证
            AddIDCard();
            break;
        case "Button"://按钮
            AddBtn(pinyin)
            break;
        case "Link"://超链接
            AddLink(pinyin);
            break;
        case "Score"://评分
            AddScore(pinyin);
            break;
        case "IFrame"://框架
            AddFrame();
            break;
        case "Map"://地图
            AddMap(pinyin);
            break;
        case "Fiexed"://系统定位
            AddFixed();
            break;
        case "BigText"://大块文本
            AddBigNoteHtmlText();
            break;
        case "NodeWorkCheck":
            var url = '../../WF/Comm/EnOnly.htm?EnName=BP.WF.Template.FrmNodeComponent&PK=' + nodeID + '&tabIndex=0';
            getWindowWH()
            //OpenLayuiDialog(url, '审核组件', W, H, false, false, true);
            LayuiPopRight(url, '审核组件', W, true);
            break;
        case "SubFlow":
            var url = '../../WF/Comm/EnOnly.htm?EnName=BP.WF.Template.FrmNodeComponent&PK=' + nodeID + '&tabIndex=1';
            getWindowWH()
            //OpenLayuiDialog(url, '父子流程组件', W, H, false, false, true);
            LayuiPopRight(url, '父子流程组件', W, true);
            break;
        case "Thread":
            var url = '../../WF/Comm/EnOnly.htm?EnName=BP.WF.Template.FrmNodeComponent&PK=' + nodeID + '&tabIndex=2';
            getWindowWH()
            //OpenLayuiDialog(url, '子线程组件', W, H, false, false, true);
            LayuiPopRight(url, '子线程组件', W, true);
            break;
        case "Track":
            var url = '../../WF/Comm/EnOnly.htm?EnName=BP.WF.Template.FrmNodeComponent&PK=' + nodeID + '&tabIndex=3';
            getWindowWH()
            //OpenLayuiDialog(url, '轨迹组件', W, H, false, false, true);
            LayuiPopRight(url, '轨迹组件', W, true);
            break;
        case "Transfer":
            var url = '../../WF/Comm/EnOnly.htm?EnName=BP.WF.Template.FrmNodeComponent&PK=' + nodeID + '&tabIndex=4';
            getWindowWH()
            //OpenLayuiDialog(url, '流转自定义', W, H, false, false, true);
            LayuiPopRight(url, '流转自定义', W, true);
            break;
        default: break;
    }
 }
