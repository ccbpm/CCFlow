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
                  { proName: 'KeyOfEn', ProText: '英文名', DefVal : 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'DefVal', ProText: '默认值', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  { proName: 'UIIsEnable', ProText: '是否可编辑', DefVal: '1', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  { proName: 'MinLen', ProText: '最小长度', DefVal: '0', DType: 'int', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'MaxLen', ProText: '最大长度', DefVal: '300', DType: 'int', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
              //    { proName: 'MapExt', ProText: '扩展属性', DefVal: '', DType: 'grouplabel', ProType: BuilderProperty.TYPE_GROUP_LABEL },
               //   { proName: BuilderProperty.SEPARATOR, ProText: '', DefVal: '', DType: 'hr', ProType: BuilderProperty.SEPARATOR },
                  { proName: 'SignType', ProText: '签名模式', DefVal: '0', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  { proName: 'UIIsInput', ProText: '是否必填', DefVal: '0', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  { proName: 'WinPOP', ProText: '设置开窗返回值', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/PopVal.aspx?FK_MapData=@FrmID@&RefNo=@KeyOfEn@&MyPK=PopVal_@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'Expression', ProText: '正则表达式', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/RegularExpression.aspx?FK_MapData=@FrmID@&RefNo=@KeyOfEn@&OperAttrKey=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'TBFullCtrl', ProText: '文本框自动完成', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/TBFullCtrl.aspx?FK_MapData=@FrmID@&RefNo=@KeyOfEn@&MyPK=@FrmID@_TBFullCtrl_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'AutoFull', ProText: '脚本验证', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/InputCheck.aspx?FK_MapData=@FrmID@&ExtType=InputCheck&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
                  ],
    TextBoxInt: [{ proName: 'FieldText', ProText: '中文名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英文名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'DefVal', ProText: '默认值', DefVal: '0', DType: 'int', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'UIIsEnable', ProText: '是否可编辑', DefVal: '1', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  { proName: 'UIVisible', ProText: '是否可见', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  //{ proName: 'WinPOP', ProText: '设置开窗返回值', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/PopVal.aspx?FK_MapData=@FrmID@&RefNo=@KeyOfEn@&MyPK=PopVal_@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'Expression', ProText: '正则表达式', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/RegularExpression.aspx?FK_MapData=@FrmID@&RefNo=@KeyOfEn@&OperAttrKey=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'TBFullCtrl', ProText: '文本框自动完成', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/TBFullCtrl.aspx?FK_MapData=@FrmID@&RefNo=@KeyOfEn@&MyPK=@FrmID@_TBFullCtrl_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'AutoFull', ProText: '自动计算', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/AutoFull.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
                 ],
    TextBoxFloat: [{ proName: 'FieldText', ProText: '中文名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英文名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'DefVal', ProText: '默认值', DefVal: '0', DType: 'int', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'UIIsEnable', ProText: '是否可编辑', DefVal: '1', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  //{ proName: 'UIVisible', ProText: '是否可见', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  { proName: 'Expression', ProText: '正则表达式', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/RegularExpression.aspx?FK_MapData=@FrmID@&RefNo=@KeyOfEn@&OperAttrKey=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'AutoFull', ProText: '自动计算', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/AutoFull.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
                 ],
    TextBoxMoney: [{ proName: 'FieldText', ProText: '中文名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英文名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'DefVal', ProText: '默认值', DefVal: '0.00', DType: 'int', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'UIIsEnable', ProText: '是否可编辑', DefVal: '1', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  //{ proName: 'UIVisible', ProText: '是否可见', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  { proName: 'Expression', ProText: '正则表达式', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/RegularExpression.aspx?FK_MapData=@FrmID@&RefNo=@KeyOfEn@&OperAttrKey=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'AutoFull', ProText: '自动计算', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/AutoFull.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
                 ],
    TextBoxDate: [{ proName: 'FieldText', ProText: '中文名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英文名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'DefVal', ProText: '默认值', DefVal: '@RDT', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'UIIsEnable', ProText: '是否可编辑', DefVal: '1', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  //{ proName: 'UIVisible', ProText: '是否可见', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  { proName: 'Expression', ProText: '正则表达式', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/RegularExpression.aspx?FK_MapData=@FrmID@&RefNo=@KeyOfEn@&OperAttrKey=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'AutoFull', ProText: '自动计算', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/AutoFull.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
                 ],
    TextBoxDateTime: [{ proName: 'FieldText', ProText: '中文名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英文名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'DefVal', ProText: '默认值', DefVal: '@RDT', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'UIIsEnable', ProText: '是否可编辑', DefVal: '1', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                 // { proName: 'UIVisible', ProText: '是否可见', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  { proName: 'Expression', ProText: '正则表达式', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/RegularExpression.aspx?FK_MapData=@FrmID@&RefNo=@KeyOfEn@&OperAttrKey=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'AutoFull', ProText: '自动计算', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/AutoFull.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
                 ],
    TextBoxBoolean: [{ proName: 'FieldText', ProText: '中文名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英文名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'DefVal', ProText: '默认值', DefVal: '0', DType: 'int', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'UIIsEnable', ProText: '是否可编辑', DefVal: '1', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  { proName: 'AutoFull', ProText: '自动计算', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/AutoFull.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
                 ],
    DropDownListEnum: [{ proName: 'FieldText', ProText: '中文名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英文名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'UIBindKey', ProText: '枚举键', DefVal: 'UIBindKey', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'DefVal', ProText: '默认值', DefVal: '', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'AutoFullDLL', ProText: '设置列表过滤', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/AutoFullDLL.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'UIIsEnable', ProText: '是否可编辑', DefVal: '1', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  { proName: 'ActiveDDL', ProText: '设置联动(如:省份，城市联动)', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/ActiveDDL.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'DDLFullCtrl', ProText: '设置下拉框自动完成', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/DDLFullCtrl.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
                 ],
    RadioButton: [{ proName: 'FieldText', ProText: '中文名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英文名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'UIBindKey', ProText: '枚举键', DefVal: 'UIBindKey', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'DefVal', ProText: '默认值', DefVal: '', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'UIIsEnable', ProText: '是否可编辑', DefVal: '1', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  { proName: 'AutoFullDLL', ProText: '设置列表过滤', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/AutoFullDLL.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'ActiveDDL', ProText: '设置联动(如:省份，城市联动)', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/ActiveDDL.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'DDLFullCtrl', ProText: '设置下拉框自动完成', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/DDLFullCtrl.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
                 ],
    DropDownListTable: [{ proName: 'FieldText', ProText: '中文名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英文名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'UIBindKey', ProText: '外键/外部表', DefVal: 'UIBindKey', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'DefVal', ProText: '默认值', DefVal: '0', DType: 'int', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'UIIsEnable', ProText: '是否可编辑', DefVal: '1', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  { proName: 'AutoFullDLL', ProText: '设置列表过滤', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/AutoFullDLL.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'ActiveDDL', ProText: '设置联动(如:省份，城市联动)', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/ActiveDDL.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'DDLFullCtrl', ProText: '设置下拉框自动完成', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/DDLFullCtrl.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
                 ],
    Dtl: [{ proName: 'No', ProText: '明细表编号', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'Name', ProText: '名称', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'PTable', ProText: '存储表', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
//                  { proName: 'Set', ProText: '设置aspx', DefVal: '/WF/Admin/FoolFormDesigner/MapDefDtlFreeFrm.aspx?FK_MapData=@FrmID@&FK_MapDtl=@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'Set', ProText: '设置', DefVal: '/WF/Admin/FoolFormDesigner/MapDefDtlFreeFrm.htm?FK_MapData=@FrmID@&FK_MapDtl=@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }

                 ],
    Fieldset: [{ proName: 'No', ProText: '编号', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'Name', ProText: '名称', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT }
                 ],
    AthMulti: [{ proName: 'No', ProText: '编号', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'Name', ProText: '名称', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'Set', ProText: '设置', DefVal: '/WF/Admin/FoolFormDesigner/Attachment.aspx?FK_MapData=@FrmID@&Ath=@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
                 ],
    AthSingle: [{ proName: 'No', ProText: '编号', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'Name', ProText: '名称', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'Set', ProText: '设置', DefVal: '/WF/Admin/FoolFormDesigner/Attachment.aspx?FK_MapData=@FrmID@&Ath=@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
                 ],
    AthImg: [{ proName: 'No', ProText: '编号', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'Name', ProText: '名称', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT }
                 ]
     };

