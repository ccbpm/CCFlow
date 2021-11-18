using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;
using System.Web;

namespace BP.Sys.FrmUI
{
    /// <summary>
    /// 实体属性
    /// </summary>
    public class MapAttrString : EntityMyPK
    {
        #region 文本字段参数属性.
        public bool IsSupperText
        {
            get
            {
                return this.GetValBooleanByKey(MapAttrAttr.IsSupperText);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.IsSupperText, value);
            }
        }
        public int IsSigan
        {
            get
            {
                return this.GetValIntByKey(MapAttrAttr.IsSigan);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.IsSigan, value);
            }
        }
        public bool IsRichText
        {
            get
            {
                return this.GetValBooleanByKey(MapAttrAttr.IsRichText);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.IsRichText, value);
            }
        }
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.GetValStringByKey(MapAttrAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.FK_MapData, value);
            }
        }
        /// <summary>
        /// 最大长度
        /// </summary>
        public int MaxLen
        {
            get
            {
                return this.GetValIntByKey(MapAttrAttr.MaxLen);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.MaxLen, value);
            }
        }

        /// <summary>
        /// 字段
        /// </summary>
        public string KeyOfEn
        {
            get
            {
                return this.GetValStringByKey(MapAttrAttr.KeyOfEn);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.KeyOfEn, value);
            }
        }
        /// <summary>
        /// 控件类型
        /// </summary>
        public UIContralType UIContralType
        {
            get
            {
                return (UIContralType)this.GetValIntByKey(MapAttrAttr.UIContralType);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.UIContralType, (int)value);
            }
        }
        /// <summary>
        /// 是否可见
        /// </summary>
        public bool UIVisible
        {
            get
            {
                return this.GetValBooleanByKey(MapAttrAttr.UIVisible);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.UIVisible, value);
            }
        }
        /// <summary>
        /// 是否可编辑
        /// </summary>
        public bool UIIsEnable
        {
            get
            {
                return this.GetValBooleanByKey(MapAttrAttr.UIIsEnable);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.UIIsEnable, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 控制权限
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsInsert = false;
                uac.IsUpdate = true;
                uac.IsDelete = true;
                return uac;
            }
        }
        /// <summary>
        /// 实体属性
        /// </summary>
        public MapAttrString()
        {
        }
        /// <summary>
        /// 实体属性
        /// </summary>
        public MapAttrString(string myPK)
        {
            this.MyPK = myPK;
            this.Retrieve();

        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Sys_MapAttr", "文本字段");

                #region 基本字段信息.
                map.AddTBStringPK(MapAttrAttr.MyPK, null, "主键", false, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.FK_MapData, null, "表单ID", false, false, 1, 100, 20);
                map.AddTBString(MapAttrAttr.Name, null, "字段中文名", true, false, 0, 200, 20, true);
                map.AddTBString(MapAttrAttr.KeyOfEn, null, "字段名", true, true, 1, 200, 20);

                //默认值.
                string sql = "SELECT No,Name FROM Sys_GloVar WHERE GroupKey='DefVal'";
                //显示的分组.
                map.AddDDLSQL("ExtDefVal", "0", "系统默认值", sql, true);

                map.AddTBString(MapAttrAttr.DefVal, null, "默认值表达式", true, false, 0, 400, 20, true);
                map.SetHelperAlert(MapAttrAttr.DefVal, "可以编辑的字段设置默认值后，保存时候按照编辑字段计算.");

                map.AddTBInt(MapAttrAttr.MinLen, 0, "最小长度", true, false);
                map.AddTBInt(MapAttrAttr.MaxLen, 50, "最大长度", true, false);
                map.SetHelperAlert(MapAttrAttr.MaxLen, "定义该字段的字节长度.");

                map.AddTBFloat(MapAttrAttr.UIWidth, 100, "宽度", true, false);
                map.SetHelperAlert(MapAttrAttr.UIWidth, "对自由表单,从表有效,显示文本框的宽度.");

                map.AddTBFloat(MapAttrAttr.UIHeight, 23, "高度", true, false);
                //map.AddTBInt(MapAttrAttr.UIContralType, 0, "控件", true, false); 
                //map.AddDDLSysEnum(MapAttrAttr.UIContralType, 0, "控件", true, false, MapAttrAttr.UIContralType);
                //map.AddTBFloat("ExtRows", 1, "文本框行数(决定高度)", true, false);

                map.AddBoolean(MapAttrAttr.UIVisible, true, "是否可见？", true, true);
                map.SetHelperAlert(MapAttrAttr.UIVisible, "对于不可见的字段可以在隐藏功能的栏目里找到这些字段进行编辑或者删除.");

                map.AddBoolean(MapAttrAttr.UIIsEnable, true, "是否可编辑？", true, true);
                map.SetHelperAlert(MapAttrAttr.UIIsEnable, "不可编辑,让该字段设置为只读.");

                map.AddBoolean(MapAttrAttr.UIIsInput, false, "是否必填项？", true, true);
                map.AddBoolean(MapAttrAttr.IsRichText, false, "是否富文本？", true, true);
                map.SetHelperAlert(MapAttrAttr.IsRichText, "以html编辑器呈现或者编写字段.");
                map.AddBoolean(MapAttrAttr.IsSecret, false, "是否保密？", true, true);

                map.AddBoolean(MapAttrAttr.IsSupperText, false, "是否大块文本？(是否该字段存放的超长字节字段)", true, true, true);
                map.SetHelperAlert(MapAttrAttr.IsSupperText, "大块文本存储字节比较长，超过4000个字符.");
                #endregion 基本字段信息.

                #region 傻瓜表单

                //单元格数量 2013-07-24 增加
                map.AddDDLSysEnum(MapAttrAttr.ColSpan, 1, "TextBox单元格数量", true, true, "ColSpanAttrDT",
                    "@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格@5=跨5个单元格@6=跨6个单元格");
                map.SetHelperAlert(MapAttrAttr.ColSpan, "对于傻瓜表单有效: 标识该字段TextBox横跨的宽度,占的单元格数量.");

                //文本占单元格数量
                map.AddDDLSysEnum(MapAttrAttr.TextColSpan, 1, "Label单元格数量", true, true, "ColSpanAttrString",
                    "@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格@5=跨6个单元格@6=跨6个单元格");
                map.SetHelperAlert(MapAttrAttr.TextColSpan, "对于傻瓜表单有效: 标识该字段Lable，标签横跨的宽度,占的单元格数量.");

                //文本跨行
                map.AddTBInt(MapAttrAttr.RowSpan, 1, "行数", true, false);

                //显示的分组.
                map.AddDDLSQL(MapAttrAttr.GroupID, 0, "显示的分组", MapAttrString.SQLOfGroupAttr, true);

                map.AddDDLSysEnum(MapAttrAttr.IsSigan, 0, "签名模式", true, true,
                    MapAttrAttr.IsSigan, "@0=无@1=图片签名@2=山东CA@3=广东CA@4=图片盖章@5=手写签名");
                map.SetHelperAlert(MapAttrAttr.IsSigan, "图片签名,需要的是当前是只读的并且默认值为@WebUser.No,其他签名需要个性化的编写数字签章的集成代码.");

                map.AddTBInt(MapAttrAttr.Idx, 0, "顺序号", true, false);
                map.SetHelperAlert(MapAttrAttr.Idx, "对傻瓜表单有效:用于调整字段在同一个分组中的顺序.");

                map.AddTBString(MapAttrAttr.ICON, null, "图标", true, false, 0, 50, 20, true);

                map.AddTBString(MapAttrAttr.Tip, null, "激活提示", true, false, 0, 400, 20, true);
                map.SetHelperAlert(MapAttrAttr.Tip, "在文本框输入的时候显示在文本框背景的提示文字,也就是文本框的 placeholder 的值.");

                //map.AddTBString("CSSLabel", null, "标签样式css", true, false, 0, 400, 20, true);
                //map.AddTBString("CSSCtrl", null, "控件样式css", true, false, 0, 400, 20, true);
                //CCS样式
                //map.AddDDLSQL(MapAttrAttr.CSS, "0", "控件样式", MapAttrString.SQLOfCSSAttr, true);

                map.AddDDLSQL("CSSLabel", "0", "标签样式", MapAttrString.SQLOfCSSAttr, true);
                map.AddDDLSQL(MapAttrAttr.CSSCtrl, "0", "控件样式", MapAttrString.SQLOfCSSAttr, true);
                #endregion 傻瓜表单.

                #region 基本功能.
                RefMethod rm = new RefMethod();

                rm = new RefMethod();
                rm.Title = "全局样式定义";
                rm.ClassMethodName = this.ToString() + ".DoStyleEditer()";
                rm.RefMethodType = RefMethodType.LinkModel;
                rm.RefAttrKey = "CSSCtrl";
                rm.GroupName = "高级设置";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "文本框自动完成";
                rm.ClassMethodName = this.ToString() + ".DoTBFullCtrl2019()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-energy";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "快速录入";
                rm.ClassMethodName = this.ToString() + ".DoFastEnter()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-docs";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "全局默认值";
                rm.ClassMethodName = this.ToString() + ".DoDefVal()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;

                //  map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "Pop返回值";
                rm.ClassMethodName = this.ToString() + ".DoPop2019()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-magnifier-add";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "事件绑函数";
                rm.ClassMethodName = this.ToString() + ".BindFunction()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-puzzle";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "正则表达式";
                rm.ClassMethodName = this.ToString() + ".DoRegularExpression()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-settings";
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "常用字段";
                //rm.ClassMethodName = this.ToString() + ".DoGeneralField()";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //map.AddRefMethod(rm);
                #endregion 基本功能.

                #region 输入多选.
                rm = new RefMethod();
                rm.GroupName = "输入内容多选";
                rm.Title = "小范围多选(combox)";
                rm.ClassMethodName = this.ToString() + ".DoMultipleChoiceSmall()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-equalizer";

                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.GroupName = "输入内容多选";
                rm.Title = "小范围单选(select)";
                rm.ClassMethodName = this.ToString() + ".DSingleChoiceSmall()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-equalizer";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.GroupName = "输入内容多选";
                rm.Title = "搜索选择";
                rm.ClassMethodName = this.ToString() + ".DoMultipleChoiceSearch()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-magnifier";

                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.GroupName = "输入内容多选";
                rm.Title = "高级快速录入";
                rm.ClassMethodName = this.ToString() + ".DoFastInput()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-magnifier";
                map.AddRefMethod(rm);
                #endregion 输入多选

                #region 高级设置.
                rm = new RefMethod();
                rm.GroupName = "高级设置";
                rm.Title = "字段重命名";
                rm.ClassMethodName = this.ToString() + ".DoRenameField()";
                rm.HisAttrs.AddTBString("key1", "@KeyOfEn", "字段重命名为?", true, false, 0, 100, 100);
                rm.RefMethodType = RefMethodType.Func;
                rm.Warning = "如果是节点表单，系统就会把该流程上的所有同名的字段都会重命名，包括NDxxxRpt表单。";
                rm.Icon = "icon-refresh";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.GroupName = "高级设置";
                rm.Title = "字段类型转换";
                rm.ClassMethodName = this.ToString() + ".DoTurnFieldType()";
                rm.HisAttrs.AddTBString("key1", "int", "输入类型，格式:int,float,double,date,datetime,boolean", true, false, 0, 100, 100);
                rm.RefMethodType = RefMethodType.Func;
                rm.Warning = "string类型转化为枚举，请先转为int,由int转枚举.";
                rm.Icon = "icon-refresh";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "批处理";
                rm.ClassMethodName = this.ToString() + ".DoEleBatch()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "高级设置";
                rm.Icon = "icon-calculator";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "转化为签批组件";
                rm.ClassMethodName = this.ToString() + ".DoSetCheck()";
                rm.Warning = "您确定要设置为签批组件吗？";
                rm.GroupName = "高级设置";
                rm.Icon = "icon-magic-wand";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "全局风格定义";
                rm.ClassMethodName = this.ToString() + ".DoGloValStyles()";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Icon = "icon-wrench";
                rm.RefAttrKey = MapAttrAttr.CSSCtrl;
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "转化为评论组件";
                //rm.ClassMethodName = this.ToString() + ".DoSetFlowBBS()";
                //rm.Warning = "您确定要设置为评论组件吗？";
                // rm.GroupName = "高级设置";
                //map.AddRefMethod(rm);

                #endregion 执行的方法.

                this._enMap = map;
                return this._enMap;
            }

        }
        public string DoGloValStyles()
        {
            return "../../Admin/FoolFormDesigner/StyletDfine/GloValStyles.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&MyPK=" + this.MyPK;
        }
        /// <summary>
        /// 字段分组查询语句
        /// </summary>
        public static string SQLOfGroupAttr
        {
            get
            {
                return "SELECT OID as No, Lab as Name FROM Sys_GroupField WHERE FrmID='@FK_MapData'  AND (CtrlType IS NULL OR CtrlType='')  ";
            }
        }
        /// <summary>
        /// 字段自定义样式查询
        /// </summary>
        public static string SQLOfCSSAttr
        {
            get
            {
                string sql = "SELECT ";
                switch (SystemConfig.AppCenterDBType)
                {
                    case DBType.MSSQL:
                        sql = "SELECT '' AS No, '默认风格' as Name ";
                        break;
                    case DBType.Oracle:
                    case DBType.PostgreSQL:
                        sql = "SELECT '' AS No, '默认风格' as Name FROM DUAL ";
                        break;
                    default:
                        sql = "SELECT '' AS No, '默认风格' as Name ";
                        break;
                }

                string mysql = sql + " UNION ";
                mysql += " SELECT No,Name FROM Sys_GloVar WHERE GroupKey='CSS' OR GroupKey='css' ";
                return mysql;
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        protected override void afterDelete()
        {
            //如果是附件字段删除附件属性
            //MapAttr attr = new MapAttr(this.MyPK);
            if (this.UIContralType == UIContralType.AthShow)
            {
                FrmAttachment ath = new FrmAttachment();
                ath.Delete(FrmAttachmentAttr.MyPK, this.MyPK);
            }
            //删除可能存在的关联属性.
            string sql = "DELETE FROM Sys_MapAttr WHERE FK_MapData='" + this.FK_MapData + "' AND KeyOfEn='" + this.KeyOfEn + "T'";
            DBAccess.RunSQL(sql);

            //删除相关的图片信息.
            if (DBAccess.IsExitsTableCol("Sys_FrmImg", "KeyOfEn") == true)
                sql = "DELETE FROM Sys_FrmImg WHERE FK_MapData='" + this.FK_MapData + "' AND KeyOfEn='" + this.KeyOfEn + "T'";
            DBAccess.RunSQL(sql);

            //删除相对应的rpt表中的字段
            if (this.FK_MapData.Contains("ND") == true)
            {
                string fk_mapData = this.FK_MapData.Substring(0, this.FK_MapData.Length - 2) + "Rpt";
                sql = "DELETE FROM Sys_MapAttr WHERE FK_MapData='" + fk_mapData + "' AND( KeyOfEn='" + this.KeyOfEn + "T' OR KeyOfEn='" + this.KeyOfEn + "')";
                DBAccess.RunSQL(sql);
            }

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);

            base.afterDelete();
        }


        protected override void afterInsertUpdateAction()
        {
            MapAttr mapAttr = new MapAttr();
            mapAttr.MyPK = this.MyPK;
            mapAttr.RetrieveFromDBSources();
            mapAttr.Update();

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);

            base.afterInsertUpdateAction();
        }

        #endregion

        #region 基本功能.
        /// <summary>
        /// 全局样式定义工具
        /// </summary>
        /// <returns></returns>
        public string DoStyleEditer()
        {
            return "../../Admin/FoolFormDesigner/StyletDfine/GloValStyles.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn;
        }
        public string DoTurnFieldType(string type)
        {
            if (type.ToLower().Equals("int")==true)
            {
                this.SetValByKey(MapAttrAttr.MyDataType, DataType.AppInt);
                this.SetValByKey(MapAttrAttr.LGType, 0); //设置成普通类型的.
                this.Update();
                return "转换为 int 类型执行成功，请您手工修改字段类型，以防止cc不能自动转换过来.";
            }
            if (type.ToLower().Equals("float") == true)
            {
                this.SetValByKey(MapAttrAttr.MyDataType, DataType.AppFloat);
                this.SetValByKey(MapAttrAttr.LGType, 0); //设置成普通类型的.
                this.Update();
                return "转换为 float 类型执行成功，请您手工修改字段类型，以防止cc不能自动转换过来.";
            }

            if (type.ToLower().Equals("double") == true)
            {
                this.SetValByKey(MapAttrAttr.MyDataType, DataType.AppDouble);
                this.SetValByKey(MapAttrAttr.LGType, 0); //设置成普通类型的.
                this.Update();
                return "转换为 double 类型执行成功，请您手工修改字段类型，以防止cc不能自动转换过来.";
            }


            if (type.ToLower().Equals("date") == true)
            {
                this.SetValByKey(MapAttrAttr.MyDataType, DataType.AppDate);
                this.SetValByKey(MapAttrAttr.LGType, 0); //设置成普通类型的.
                this.Update();
                return "转换为 date 类型执行成功.";
            }

            if (type.ToLower().Equals("datetime") == true)
            {
                this.SetValByKey(MapAttrAttr.MyDataType, DataType.AppDateTime);
                this.SetValByKey(MapAttrAttr.LGType, 0); //设置成普通类型的.
                this.Update();
                return "转换为 AppDateTime 类型执行成功.";
            }

            if (type.ToLower().Equals("boolean") == true)
            {
                this.SetValByKey(MapAttrAttr.MyDataType, DataType.AppBoolean);
                this.SetValByKey(MapAttrAttr.LGType, 0); //设置成普通类型的.
                this.Update();
                return "转换为 boolean 类型执行成功，请您手工修改字段类型int，以防止cc不能自动转换过来.";
            }


            return "err@输入的类型错误:" +type;
        }

        public string DoRenameField(string newField)
        {
            string sql = "";
            if (this.FK_MapData.IndexOf("ND") == 0)
            {
                string strs = this.FK_MapData.Replace("ND", "");
                strs = strs.Substring(0, strs.Length - 2);

                string rptTable = "ND" + strs + "Rpt";
                MapDatas mds = new MapDatas();
                mds.Retrieve(MapDataAttr.PTable, rptTable);

                foreach (MapData item in mds)
                {
                    sql = "UPDATE Sys_MapAttr SET KeyOfEn='" + newField + "',  MyPK='" + item.No + "_" + newField + "' WHERE KeyOfEn='" + this.KeyOfEn + "' AND FK_MapData='" + item.No + "'";
                    DBAccess.RunSQL(sql);
                }
            }
            else
            {
                sql = "UPDATE Sys_MapAttr SET KeyOfEn='" + newField + "', MyPK='" + this.FK_MapData + "_" + newField + "'  WHERE KeyOfEn='" + this.KeyOfEn + "' AND FK_MapData='" + this.FK_MapData + "'";
                DBAccess.RunSQL(sql);
            }

            return "重名称成功,如果是自由表单，请关闭表单设计器重新打开.";
        }
        /// <summary>
        /// 绑定函数
        /// </summary>
        /// <returns></returns>
        public string BindFunction()
        {
            return "../../Admin/FoolFormDesigner/MapExt/BindFunction.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn;
        }
        /// <summary>
        /// 快速录入
        /// </summary>
        /// <returns></returns>
        public string DoFastEnter()
        {
            return "../../Admin/FoolFormDesigner/MapExt/FastInput.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn;
        }
        /// <summary>
        /// 快速录入
        /// </summary>
        /// <returns></returns>
        public string DoPop2019()
        {
            return "../../Admin/FoolFormDesigner/Pop/Default.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn;
        }
        /// <summary>
        /// 设置常用字段
        /// </summary>
        /// <returns></returns>
        public string DoGeneralField()
        {
            return "../../Admin/FoolFormDesigner/General/GeneralField.htm?FrmID=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn;
        }
        /// <summary>
        /// 全局默认值
        /// </summary>
        /// <returns></returns>
        public string DoDefVal()
        {
            return "../../Admin/FoolFormDesigner/DefVal.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn;
        }
        #endregion

        #region 方法执行 Pop自动完成.
        /// <summary>
        /// 简单列表模式
        /// </summary>
        /// <returns></returns>
        public string DoPopFullCtrl()
        {
            return "../../Admin/FoolFormDesigner/MapExt/PopFullCtrl.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&MyPK=TBFullCtrl_" + this.MyPK;
        }
        /// <summary>
        /// 多条件查询列表模式
        /// </summary>
        /// <returns></returns>
        public string DoPopFullCtrlAdv()
        {
            return "../../Admin/FoolFormDesigner/MapExt/PopFullCtrl.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&MyPK=TBFullCtrl_" + this.MyPK;
        }
        #endregion 方法执行 Pop填充自动完成.

        #region 方法执行.
        /// <summary>
        /// 设置签批组件
        /// </summary>
        /// <returns>执行结果</returns>
        public string DoSetCheck()
        {
            this.UIContralType = UIContralType.SignCheck;
            this.UIIsEnable = false;
            this.UIVisible = false;
            this.Update();
            return "设置成功,当前文本框已经是签批组件了,请关闭掉当前的窗口.";
        }

        public string DoSetFlowBBS()
        {
            MapAttrs mapAttrs = new MapAttrs();
            mapAttrs.Retrieve(MapAttrAttr.FK_MapData, this.FK_MapData, MapAttrAttr.UIContralType, (int)UIContralType.FlowBBS);
            if (mapAttrs.Count == 0)
            {
                this.UIContralType = UIContralType.FlowBBS;
                this.UIIsEnable = false;
                this.UIVisible = false;
                this.Update();
                return "设置成功,当前文本框已经是评论组件了,请关闭掉当前的窗口.";
            }

            return "表单中只能存在一个评论组件，表单" + this.FK_MapData + "已经存在评论组件不能再增加";
        }
        /// <summary>
        /// 批处理
        /// </summary>
        /// <returns></returns>
        public string DoEleBatch()
        {
            //return "../../Admin/FoolFormDesigner/EleBatch.aspx?EleType=MapAttr&KeyOfEn=" + this.KeyOfEn + "&FType=1&MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData;
            return "../../Admin/FoolFormDesigner/EleBatch.htm?EleType=MapAttr&KeyOfEn=" + this.KeyOfEn + "&FType=1&MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData;
        }
        /// <summary>
        /// 小范围多选
        /// </summary>
        /// <returns></returns>
        public string DoMultipleChoiceSmall()
        {
            return "../../Admin/FoolFormDesigner/MapExt/MultipleChoiceSmall/Default.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&m=s";
        }


        public string DSingleChoiceSmall()
        {
            return "../../Admin/FoolFormDesigner/MapExt/SingleChoiceSmall/Default.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&m=s";

        }
        /// <summary>
        /// 大范围多选
        /// </summary>
        /// <returns></returns>
        public string DoMultipleChoiceSearch()
        {
            return "../../Admin/FoolFormDesigner/MapExt/MultipleChoiceSearch.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&m=s";
        }

        public string DoFastInput()
        {
            return "../../Admin/FoolFormDesigner/MapExt/MultipleInputSearch.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&m=s";

        }
        /// <summary>
        /// 超链接
        /// </summary>
        /// <returns></returns>
        public string DoLink()
        {
            return "../../Admin/FoolFormDesigner/MapExt/Link.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&MyPK=" + this.MyPK + "&FK_MapExt=Link_" + this.FK_MapData + "_" + this.KeyOfEn;
        }
        /// <summary>
        /// 设置开窗返回值
        /// </summary>
        /// <returns></returns>
        public string DoPopVal()
        {
            return "../../Admin/FoolFormDesigner/MapExt/PopVal.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&MyPK=" + this.MyPK + "&FK_MapExt=PopVal_" + this.FK_MapData + "_" + this.KeyOfEn;
        }
        /// <summary>
        /// 正则表达式
        /// </summary>
        /// <returns></returns>
        public string DoRegularExpression()
        {
            return "../../Admin/FoolFormDesigner/MapExt/RegularExpression.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&MyPK=" + this.MyPK;
        }

        /// <summary>
        /// 文本框自动完成
        /// </summary>
        /// <returns></returns>
        public string DoTBFullCtrl2019()
        {
            return "../../Admin/FoolFormDesigner/TBFullCtrl/Default.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&MyPK=TBFullCtrl_" + this.MyPK;
        }

        /// <summary>
        /// 设置级联
        /// </summary>
        /// <returns></returns>
        public string DoInputCheck()
        {
            return "../../Admin/FoolFormDesigner/MapExt/InputCheck.htm?FK_MapData=" + this.FK_MapData + "&OperAttrKey=" + this.KeyOfEn + "&RefNo=" + this.MyPK + "&DoType=New&ExtType=InputCheck";
        }
        /// <summary>
        /// 扩展控件
        /// </summary>
        /// <returns></returns>
        public string DoEditFExtContral()
        {
            return "../../Admin/FoolFormDesigner/EditFExtContral.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&MyPK=" + this.MyPK;
        }
        /// <summary>
        /// 扩展控件2019
        /// </summary>
        /// <returns></returns>
        public string DoEditFExtContral2019()
        {
            return "../../Admin/FoolFormDesigner/EditFExtContral/Default.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&MyPK=" + this.MyPK;
        }
        #endregion 方法执行.

        #region 重载.
        protected override bool beforeUpdateInsertAction()
        {
            MapAttr attr = new MapAttr();
            attr.MyPK = this.MyPK;
            attr.RetrieveFromDBSources();

            //高度.
            //  attr.UIHeightInt = this.GetValIntByKey("ExtRows") * 23;

            attr.IsRichText = this.GetValBooleanByKey(MapAttrAttr.IsRichText); //是否是富文本？
            attr.IsSupperText = this.GetValIntByKey(MapAttrAttr.IsSupperText); //是否是大块文本？

            if (attr.IsRichText || attr.IsSupperText == 1)
            {
                attr.MaxLen = 4000;
                this.SetValByKey(MapAttrAttr.MaxLen, 4000);
            }


            #region 自动扩展字段长度. 需要翻译.
            if (attr.MaxLen < this.MaxLen)
            {
                attr.MaxLen = this.MaxLen;

                string sql = "";
                MapData md = new MapData();
                md.No = this.FK_MapData;
                if (md.RetrieveFromDBSources() == 1)
                {
                    if (DBAccess.IsExitsTableCol(md.PTable, this.KeyOfEn) == true)
                    {
                        if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                            sql = "ALTER TABLE " + md.PTable + " ALTER column " + this.KeyOfEn + " NVARCHAR(" + attr.MaxLen + ")";

                        if (SystemConfig.AppCenterDBType == DBType.MySQL)
                            sql = "ALTER table " + md.PTable + " modify " + attr.Field + " NVARCHAR(" + attr.MaxLen + ")";

                        if (SystemConfig.AppCenterDBType == DBType.Oracle
                            || SystemConfig.AppCenterDBType == DBType.DM)
                            sql = "ALTER table " + md.PTable + " modify " + attr.Field + " NVARCHAR2(" + attr.MaxLen + ")";

                        if (SystemConfig.AppCenterDBType == DBType.PostgreSQL)
                            sql = "ALTER table " + md.PTable + " alter " + attr.Field + " type character varying(" + attr.MaxLen + ")";

                        DBAccess.RunSQL(sql); //如果是oracle如果有nvarchar与varchar类型，就会出错.
                    }
                }
            }
            #endregion 自动扩展字段长度.


            //默认值.
            string defval = this.GetValStrByKey("ExtDefVal");
            if (defval == "" || defval == "0")
            {
                string defVal = this.GetValStrByKey("DefVal");
                if (defval.Contains("@") == true)
                    this.SetValByKey("DefVal", "");
            }
            else
            {
                this.SetValByKey("DefVal", this.GetValByKey("ExtDefVal"));
            }

            //执行保存.
            attr.Save();

            if (this.GetValStrByKey("GroupID") == "无")
                this.SetValByKey("GroupID", "0");


            return base.beforeUpdateInsertAction();
        }
        #endregion
    }
    /// <summary>
    /// 实体属性s
    /// </summary>
    public class MapAttrStrings : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 实体属性s
        /// </summary>
        public MapAttrStrings()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapAttrString();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapAttrString> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapAttrString>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapAttrString> Tolist()
        {
            System.Collections.Generic.List<MapAttrString> list = new System.Collections.Generic.List<MapAttrString>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapAttrString)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
