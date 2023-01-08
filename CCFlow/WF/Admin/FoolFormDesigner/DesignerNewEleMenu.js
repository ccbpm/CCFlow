var eleMenus = [{
    "id": "Base",
    "title": "基本字段",
    "icon": "iconfont icon-ziduan",
    "child": [
        { "id": "TextBox", "title": "文字", "icon": "iconfont icon-fuwenbenkuang", "MethodType": "Field", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "TextBoxInt", "title": "整数", "icon": "iconfont icon-zhengshu", "MethodType": "Field", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "TextBoxFloat", "title": "数值", "icon": "iconfont icon-ziduanleixing-zhengshu", "MethodType": "Field", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "TextBoxMoney", "title": "金额", "icon": "iconfont icon-yifabupiaoju-renminbi-xi", "MethodType": "Field", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "Date", "title": "日期", "icon": "iconfont icon-riqiqishu", "MethodType": "Field", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "DateTime", "title": "日期时间", "icon": "iconfont icon-shijian1", "MethodType": "Field", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "CheckBox", "title": "复选框", "icon": "iconfont icon-fuxuankuang", "MethodType": "Field", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { type: '-' },
        { "id": "Radio", "title": "单选按钮", "icon": "iconfont icon-danxuan", "MethodType": "Field", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "RadioSelect", "title": "枚举下拉框", "icon": "iconfont icon-xialakuang", "MethodType": "Field", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "RadioCheckBox", "title": "枚举多选按钮", "icon": "iconfont icon-xialakuangbiaodan", "MethodType": "Field", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { type: '-' },
        { "id": "Select", "title": "外键,外部数据源下拉框", "icon": "iconfont icon-xialakuang1", "MethodType": "Field", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "WorkCheck", "title": "签批组件(电子签章)", "icon": "iconfont icon-ptkj-lianxuqianpimoshi", "MethodType": "Field", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 }
    ]
}, {
    "id": "Ath",
    "title": "附件",
    "icon": "iconfont icon-fujian",
    "child": [
        { "id": "FieldAth", "title": "字段附件", "icon": "iconfont icon-attach", "MethodType": "Field", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "TableAth", "title": "表格附件", "icon": "iconfont icon-biaogefujian", "MethodType": "Ath", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "ImgAth", "title": "图片附件", "icon": "iconfont icon-tupianfujian", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "GovDoc", "title": "公文组件", "icon": "iconfont icon-tupianfujian", "MethodType": "Field", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "HandlerWriting", "title": "写字板", "icon": "iconfont icon-xiezi", "MethodType": "Field", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 }
    ]
}, {
    "id": "Dtl",
    "title": "从表",
    "icon": "iconfont icon-tianjiashujubiao"
    }
    , {
        "id": "Print",
        "title": "打印组件",
        "icon": "icon-printer",
        "child": [
            { "id": "PrintRTF", "title": "RTF模板打印", "icon": "icon-docs", "MethodType": "Field", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
            { "id": "PrintHtml", "title": "Html模板打印", "icon": "icon-printer", "MethodType": "Field", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 }
        ]
    }, {
    "id": "Group",
    "title": "字段分组",
    "icon": "iconfont icon-fenzu",
    "child": [
        { "id": "GroupFWC", "title": "审核字段分组", "icon": "iconfont icon-ziduan1", "MethodType": "Func", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "GroupField", "title": "字段分组", "icon": "iconfont icon-ziduanliebiao", "MethodType": "Func", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 }
    ]
}, {
    "id": "Component",
    "title": "通用组件",
    "icon": "iconfont icon-zujian",
    "child": [
        { "id": "IDCard", "title": "身份证上传", "icon": "iconfont icon-shenfenzheng", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "Button", "title": "按钮", "icon": "iconfont icon-anniu", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "Link", "title": "超链接", "icon": "iconfont icon-chaolianjie", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "Score", "title": "评分", "icon": "iconfont icon-pingfen", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "IFrame", "title": "框架", "icon": "iconfont icon-kuangjia1", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "Map", "title": "地图", "icon": "iconfont icon-jiedianleizhukongzhongxin1", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "Fiexed", "title": "系统定位", "icon": "iconfont icon-dingwei", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "BigText", "title": "大块html说明", "icon": "iconfont icon-html", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "JobSchedule", "title": "进度图", "icon": "iconfont icon-html", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 }
    ]
}, {
    "id": "Node",
    "title": "节点组件",
    "icon": "iconfont icon-jiedian",
    "child": [
        { "id": "NodeWorkCheck", "title": "审核组件", "icon": "iconfont icon-shenhe1", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "SubFlow", "title": "父子流程组件", "icon": "iconfont icon-xianchengzhuizong", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "Thread", "title": "子线程组件", "icon": "iconfont icon-shouye-xiancheng-", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "Track", "title": "轨迹组件", "icon": "iconfont icon-shitujiedianxianshi", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 },
        { "id": "Transfer", "title": "流转自定义组件", "icon": "iconfont icon-jiedianleizhongxinguanli", "MethodType": "Url", "MethodDoc": "..NewDatype.htm", "H": 400, "W": 300 }
    ]
}
];

/**
 * 创建控件
 * */
function NewEleByClassic(ctrlEn, beforeCtrlIdx) {

    //生成一个随机的字段.
    var handler = new HttpHandler("WF_Admin_FoolFormDesigner");
    handler.AddPara("FrmID", GetQueryString("FrmID"));
    var data = handler.DoMethodReturnString("GenerRandomFieldID"); //生成一个临时的ID.

    // 创建字段.
    var en = new Entity("BP.Sys.MapAttr");
    en.FK_MapData = GetQueryString("FrmID"); //表单ID.
    en.KeyOfEn = data; //字段ID.
    en.Name = ctrlEn.title + data; //字段名称
    en.Idx = beforeCtrlIdx;

    if (ctrlEn.id == "TextBox") {
        en.MaxLen = 50;
        en.MyDataType = 1; //string 类型.
    }
    if (ctrlEn.id == "TextBoxInt") {
        en.MyDataType = 2; //int 类型.
    }
    if (ctrlEn.id == "TextBoxFloat") {
        en.MyDataType = 3; //int 数值.
    }
    if (ctrlEn.id == "TextBoxMoney") {
        en.MyDataType = 8; //金额.
    }
    if (ctrlEn.id == "Date") {
        en.MyDataType = 6; //日期.
    }
    if (ctrlEn.id == "DateTime") {
        en.MyDataType = 7; //日期时间.
    }
    if (ctrlEn.id == "CheckBox") {
        en.MyDataType = 4; //复选框.
    }
    en.Insert();
}
/*
*转成菜单下拉框
*/
function CovertEleMenuToDropdownMenu() {
    $.each(eleMenus, function (i, eleMenu) {
        if (eleMenu.templet != "")
            eleMenu.templet = '<i class="' + eleMenu.icon + '" style="padding-right:3px"></i>' + eleMenu.title;
        if (eleMenu.child && eleMenu.child.length != 0) {
            eleMenu.child.forEach(function (item) {
                item.templet = '<i class="' + item.icon + '" style="padding-right:3px"></i>' + item.title;
            });
        }

    });
    return eleMenus;
}
/*
* 菜单单击时执行的事件
*/
function MenuClick(type, pinyin) {
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
        case "PrintRTF"://打印组件
            PrintRTF();
            break;
        case "ImgAth"://图片附件
            AddImgAth(pinyin);
            break;
        case "HandlerWriting"://写字板
            AddHandWriting(pinyin);
            break;
        case "GovDoc"://公文组件
            GovDoc(pinyin);
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
            AddBigNoteHtmlText(pinyin);
            break;
        case "JobSchedule"://进度图
            ExtJobSchedule();
            break;
        case "NodeWorkCheck":
            var url = '../../Comm/EnOnly.htm?EnName=BP.WF.Template.FrmNodeComponent&PK=' + nodeID + '&tabIndex=0';
            getWindowWH()
            OpenLayuiDialog(url, '审核组件', W, 0, null, true);
            //LayuiPopRight(url, '审核组件', W, true);
            break;
        case "SubFlow":
            var url = '../../Comm/EnOnly.htm?EnName=BP.WF.Template.FrmNodeComponent&PK=' + nodeID + '&tabIndex=1';
            getWindowWH()
            OpenLayuiDialog(url, '父子流程组件', W, 0, null, true);
            //LayuiPopRight(url, '父子流程组件', W, true);
            break;
        case "Thread":
            var url = '../../Comm/EnOnly.htm?EnName=BP.WF.Template.FrmNodeComponent&PK=' + nodeID + '&tabIndex=2';
            getWindowWH()
            OpenLayuiDialog(url, '子线程组件', W, 0, null, true);
            //LayuiPopRight(url, '子线程组件', W, true);
            break;
        case "Track":
            var url = '../../Comm/EnOnly.htm?EnName=BP.WF.Template.FrmNodeComponent&PK=' + nodeID + '&tabIndex=3';
            getWindowWH()
            OpenLayuiDialog(url, '轨迹组件', W, 0, null, true);
            //LayuiPopRight(url, '轨迹组件', W, true);
            break;
        case "Transfer":
            var url = '../../Comm/EnOnly.htm?EnName=BP.WF.Template.FrmNodeComponent&PK=' + nodeID + '&tabIndex=4';
            getWindowWH()
            OpenLayuiDialog(url, '流转自定义', W, 0, null, true);
            //LayuiPopRight(url, '流转自定义', W, true);
            break;
        default: break;
    }
}
