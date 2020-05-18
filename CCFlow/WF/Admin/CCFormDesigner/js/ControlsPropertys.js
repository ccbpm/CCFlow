/**枚举数组值集合**/
CCForm_Control_Enum = {
    /**
    this enum use for Image,Button
    **/
    WinOpenModel: [{ Text: '新窗口', Value: '_blank' },
                 { Text: '父窗口', Value: '_parent' },
                 { Text: '本窗口', Value: '_self' },
                 { Text: '自定义', Value: 'def' }
    ],
    /**
    this enum use for Image,
    **/
    ImgAppType: [{ Text: '本地图片', Value: '0' },
                 { Text: '指定路径', Value: '1' }
    ],
    /**
    this enum use for TextBox,
    **/
    UIIsEnable: [{ Text: '不可编辑', Value: '0' },
                 { Text: '可编辑', Value: '1' }
    ],
    /**
    this enum use for TextBox,
    **/
    UIVisible: [{ Text: '不可见', Value: '0' },
                { Text: '界面可见', Value: '1' }
    ],
    /**
    this enum use for Button,
    **/
    ButtonEvent: [{ Text: '禁用', Value: '0' },
                { Text: '执行存储过程', Value: '1' },
                { Text: '执行sql', Value: '2' },
                { Text: '执行URL', Value: '3' },
                { Text: '执行webservices', Value: '4' },
                { Text: '执行EXE', Value: '5' },
                { Text: '执行JS脚本', Value: '6' }
    ],
    /**
    this enum use for TextBox,
    **/
    DefVal: [{ Text: '选择系统约定默认值', Value: '' },
                { Text: '登陆人员账号', Value: '@WebUser.No' },
                { Text: '登陆人员名称', Value: '@WebUser.Name' },
                { Text: '登陆人员部门编号', Value: '@WebUser.FK_Dept' },
                { Text: '登陆人员部门名称', Value: '@WebUser.FK_DeptName' },
                { Text: '登陆人员部门全称', Value: '@WebUser.FK_DeptFullName' },
                { Text: '当前日期-yyyy年mm月dd日', Value: '@yyyy年mm月dd日' },
                { Text: '当前日期-yy年mm月dd日', Value: '@yy年mm月dd日' },
                { Text: '当前年度', Value: '@FK_ND' },
                { Text: '当前月份', Value: '@FK_YF' }/*,
                { Text: '当前工作可处理人员', Value: '@CurrWorker' }*/
    ],
    
    /** 
       this enum use for SignType,
     **/
    SignType: [ { Text: '无', Value: '0' },
                { Text: '图片签名', Value: '1' },
                { Text: '山东CA签名', Value: '2' },
                { Text: '山东CA签名', Value: '3' }
                ],
    /**
    this enum use for TextBox,
    **/
    UIIsInput: [{ Text: '否', Value: 'false' },
                { Text: '是', Value: 'true' }
    ]
};
/** CCForm 数据字段属性 
*** proName:属性英文名称
*** ProText:属性中文标签
*** DefVal:默认值,系统字段替换规则,节点编号@FrmID@,字段名@KeyOfEn@
*** DType:属性面板生成控件类型，字符型string,整数int,浮点型float,下拉框enum,超链接href,横线hr,分组标签grouplabel
*** ProType：属性类型，根据此类型生成不同的控件。
*************BuilderProperty.TYPE_GROUP_LABEL -- 分组标签
*************BuilderProperty.SEPARATOR        -- 横线
    
*************BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly -- 单行只读文本框
*************BuilderProperty.TYPE_SINGLE_TEXT          -- 单行可编辑文本框
*************BuilderProperty.TYPE_TEXT                 -- 多行可编辑文本框

*************BuilderProperty.TYPE_TEXT_FONT_FAMILY    -- 选择字体下拉框
*************BuilderProperty.TYPE_TEXT_FONT_SIZE      -- 选择字体大小下拉框
*************BuilderProperty.TYPE_TEXT_FONT_ALIGNMENT -- 字体对齐方式下拉框
*************BuilderProperty.TYPE_TEXT_UNDERLINED     -- 设置是否显示下划线按钮
*************BuilderProperty.TYPE_TEXT_FONTWEIGHT     -- 字体加粗样式下拉框
*************BuilderProperty.TYPE_COLOR               -- 颜色选择框

*************BuilderProperty.CCFormEnum   -- ccform定义枚举值下拉框
*************BuilderProperty.CCFormLink   -- ccform定义超链接
**/
CCForm_Control_Propertys = {
    TextBoxStr: [{ proName: 'FieldText', ProText: '中文名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英文名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'Propertys', ProText: '属性', DefVal: '/WF/Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrString&PK=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    TextBoxInt: [{ proName: 'FieldText', ProText: '中文名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英文名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'Propertys', ProText: '属性', DefVal: '/WF/Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&PKVal=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    TextBoxFloat: [{ proName: 'FieldText', ProText: '中文名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英文名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'Propertys', ProText: '属性', DefVal: '/WF/Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&PKVal=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    TextBoxMoney: [{ proName: 'FieldText', ProText: '中文名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英文名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'Propertys', ProText: '属性', DefVal: '/WF/Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&PKVal=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    TextBoxDate: [{ proName: 'FieldText', ProText: '中文名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英文名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'Propertys', ProText: '属性', DefVal: '/WF/Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrDT&PKVal=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    TextBoxDateTime: [{ proName: 'FieldText', ProText: '中文名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英文名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'Propertys', ProText: '属性', DefVal: '/WF/Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrDT&PK=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    TextBoxBoolean: [{ proName: 'FieldText', ProText: '中文名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英文名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'Propertys', ProText: '属性', DefVal: '/WF/Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrBoolen&PK=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    DropDownListEnum: [{ proName: 'FieldText', ProText: '中文名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英文名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'UIBindKey', ProText: '枚举键', DefVal: 'UIBindKey', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'Propertys', ProText: '属性', DefVal: '/WF/Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrEnum&PK=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    RadioButton: [{ proName: 'FieldText', ProText: '中文名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英文名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'Propertys', ProText: '属性', DefVal: '/WF/Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrEnum&PK=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    DropDownListTable: [{ proName: 'FieldText', ProText: '中文名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英文名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'Propertys', ProText: '属性', DefVal: '/WF/Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrSFTable&PK=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    Dtl: [{ proName: 'No', ProText: '明细表编号', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'Name', ProText: '名称', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                //  { proName: 'PTable', ProText: '存储表', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'Set', ProText: '设置', DefVal: '/WF/Admin/FoolFormDesigner/MapDefDtlFreeFrm.htm?FK_MapData=@FrmID@&FK_MapDtl=@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }

    ],
    Fieldset: [{ proName: 'No', ProText: '编号', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'Name', ProText: '名称', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT }
    ],
    AthMulti: [{ proName: 'No', ProText: '编号', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'Name', ProText: '名称', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'Set', ProText: '设置', DefVal: '/WF/Comm/En.htm?EnName=BP.Sys.FrmUI.FrmAttachmentExt&PK=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    AthSingle: [{ proName: 'No', ProText: '编号', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'Name', ProText: '名称', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'Set', ProText: '设置', DefVal: '/WF/Comm/En.htm?EnName=BP.Sys.FrmUI.FrmAttachmentExt&PK=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    AthImg: [{ proName: 'No', ProText: '编号', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'Name', ProText: '名称', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT }
    ],
    FlowChart: [{ proName: 'No', ProText: '编号', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                 { proName: 'Name', ProText: '名称', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                 { proName: 'Set', ProText: '设置', DefVal: '/WF/Admin/FoolFormDesigner/MapDefDtlFreeFrm.htm?FK_MapData=@FrmID', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    ThreadDtl: [{ proName: 'No', ProText: '编号', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
             { proName: 'Name', ProText: '名称', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
             { proName: 'Set', ProText: '设置', DefVal: '/WF/Admin/FoolFormDesigner/MapDefDtlFreeFrm.htm?FK_MapData=@FrmID', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    SubFlowDtl: [{ proName: 'No', ProText: '编号', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
             { proName: 'Name', ProText: '名称', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
             { proName: 'Set', ProText: '设置', DefVal: '/WF/Admin/FoolFormDesigner/MapDefDtlFreeFrm.htm?FK_MapData=@FrmID', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    FrmCheck: [{ proName: 'No', ProText: '编号', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
             { proName: 'Name', ProText: '名称', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
             { proName: 'Set', ProText: '设置', DefVal: '/WF/Admin/FoolFormDesigner/MapDefDtlFreeFrm.htm?FK_MapData=@FrmID', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    HandSiganture: [{ proName: 'No', ProText: '编号', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
             { proName: 'Name', ProText: '名称', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
             { proName: 'Set', ProText: '设置', DefVal: '/WF/Admin/FoolFormDesigner/MapDefDtlFreeFrm.htm?FK_MapData=@FrmID', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    iFrame: [{ proName: 'No', ProText: '编号', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                 { proName: 'Name', ProText: '名称', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                 { proName: 'Set', ProText: '设置', DefVal: '/WF/Admin/FoolFormDesigner/MapDefDtlFreeFrm.htm?FK_MapData=@FrmID', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    CheckGroup: [{ proName: 'No', ProText: '编号', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                 { proName: 'Name', ProText: '名称', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                 { proName: 'Set', ProText: '设置', DefVal: '/WF/Admin/FoolFormDesigner/MapDefDtlFreeFrm.htm?FK_MapData=@FrmID', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    Image: [{ proName: 'No', ProText: '编号', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'Name', ProText: '名称', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT }
    ]
};


/*控件的一些默认值  如宽、高  中英文 名称互换*/
CCForm_Control_DefaultPro = {
    TextBoxStr: { "DefaultWidth": 120, DefaultHeight: 23, ControlLab: "" },
    TextBoxInt: { "DefaultWidth": 120, DefaultHeight: 23, ControlLab: "" },
    TextBoxFloat: { "DefaultWidth": 120, DefaultHeight: 23, ControlLab: "" },
    TextBoxMoney: { "DefaultWidth": 120, DefaultHeight: 23, ControlLab: "" },
    TextBoxDate: { "DefaultWidth": 120, DefaultHeight: 23, ControlLab: "" },
    TextBoxDateTime: { "DefaultWidth": 120, DefaultHeight: 23, ControlLab: "" },
    TextBoxBoolean: { "DefaultWidth": 120, DefaultHeight: 23, ControlLab: "" },
    DropDownListEnum: { "DefaultWidth": 120, DefaultHeight: 23, ControlLab: "" },
    RadioButton: { "DefaultWidth": 120, DefaultHeight: 23, ControlLab: "" },
    DropDownListTable: { "DefaultWidth": 120, DefaultHeight: 23, ControlLab: "" },
    Dtl: { "DefaultWidth": 500, DefaultHeight: 120, ControlLab: "" },
    Fieldset: { "DefaultWidth": 100, DefaultHeight: 200, ControlLab: "" },
    AthMulti: { "DefaultWidth": 500, DefaultHeight: 120, ControlLab: "" },
    AthSingle: { "DefaultWidth": 100, DefaultHeight: 200, ControlLab: "" },
    AthImg: { "DefaultWidth": 200, DefaultHeight: 200, ControlLab: "" },
    FlowChart: { "DefaultWidth": 500, DefaultHeight: 120, ControlLab: "" },
    ThreadDtl: { "DefaultWidth": 500, DefaultHeight: 120, ControlLab: "" },
    SubFlowDtl: { "DefaultWidth": 500, DefaultHeight: 120, ControlLab: "" },
    FrmCheck: { "DefaultWidth": 500, DefaultHeight: 120, ControlLab: "" },
     Image: { "DefaultWidth": 150, DefaultHeight: 150, ControlLab: "" },
    HandSiganture: { "DefaultWidth": 500, DefaultHeight: 120, ControlLab: "" },
    iFrame: { "DefaultWidth": 500, DefaultHeight: 120, ControlLab: "" },
    CheckGroup: { "DefaultWidth": 500, DefaultHeight: 120, ControlLab: "" },
};


