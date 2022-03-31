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
    /// 数值字段
    /// </summary>
    public class MapAttrNum : EntityMyPK
    {
        #region 属性.
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefVal
        {
            get
            {
                return this.GetValStrByKey(MapAttrAttr.DefVal);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.DefVal, value);
            }
        }
        public int DefValType
        {
            get
            {
                return this.GetValIntByKey(MapAttrAttr.DefValType);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.DefValType, value);
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
        /// 绑定的枚举ID
        /// </summary>
        public string UIBindKey
        {
            get
            {
                return this.GetValStringByKey(MapAttrAttr.UIBindKey);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.UIBindKey, value);
            }
        }
        /// <summary>
        /// 数据类型
        /// </summary>
        public int MyDataType
        {
            get
            {
                return this.GetValIntByKey(MapAttrAttr.MyDataType);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.MyDataType, value);
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
        /// 数值字段
        /// </summary>
        public MapAttrNum()
        {
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

                Map map = new Map("Sys_MapAttr", "数值字段");
                map.IndexField = MapAttrAttr.FK_MapData;

                #region 基本信息.
                map.AddTBStringPK(MapAttrAttr.MyPK, null, "主键", false, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.FK_MapData, null, "实体标识", false, false, 1, 100, 20);

                map.AddTBString(MapAttrAttr.Name, null, "字段中文名", true, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.KeyOfEn, null, "字段名", true, true, 1, 200, 20);

                map.AddDDLSysEnum(MapAttrAttr.MyDataType, 2, "数据类型", true, false);

                map.AddTBString(MapAttrAttr.DefVal, MapAttrAttr.DefaultVal, "默认值/小数位数", true, false, 0, 200, 20);

                map.AddDDLSysEnum(MapAttrAttr.DefValType, 1, "默认值选择方式", true, true, "DefValType", "@0=默认值为空@1=按照设置的默认值设置", false);
                string help = "给该字段设置默认值:\t\r";

                help += "\t\r 1. 如果是整形就设置一个整形的数字作为默认值.";
                help += "\t\r 2. 对于float,decimal数据类型，如果设置0.0000就是标识要保留4位小数,如果是1.0000 标识保留4位小数,默认值为1.";
                map.SetHelperAlert("DefVal", help);

                map.AddBoolean(MapAttrAttr.UIVisible, true, "是否可见？", true, true);
                map.AddBoolean(MapAttrAttr.UIIsEnable, true, "是否可编辑？", true, true);
                map.AddBoolean(MapAttrAttr.UIIsInput, false, "是否必填项？", true, true);
                map.AddBoolean(MapAttrAttr.IsSecret, false, "是否保密？", true, true);
                map.AddBoolean("ExtIsSum", false, "是否显示合计(对从表有效)", true, true);
                map.SetHelperAlert("ExtIsSum", "如果是从表，就需要显示该从表的合计,在从表的底部.");
                map.AddTBString(MapAttrAttr.Tip, null, "激活提示", true, false, 0, 400, 20, true);
                #endregion 基本信息.

                #region 傻瓜表单
                map.AddDDLSysEnum(MapAttrAttr.ColSpan, 1, "TextBox单元格数", true, true, "ColSpanAttrDT",
                   "@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格");

                //文本占单元格数量
                map.AddDDLSysEnum(MapAttrAttr.TextColSpan, 1, "Label文本单元格数", true, true, "ColSpanAttrString",
                    "@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格");

                map.AddTBFloat(MapAttrAttr.UIWidth, 80, "宽度", true, false);
                map.AddTBFloat(MapAttrAttr.UIHeight, 23, "高度", true, true);

                //文本跨行
                map.AddTBInt(MapAttrAttr.RowSpan, 1, "行数", true, false);
                //显示的分组.
                map.AddDDLSQL(MapAttrAttr.GroupID, 0, "显示的分组", MapAttrString.SQLOfGroupAttr, true);
                map.AddTBInt(MapAttrAttr.Idx, 0, "顺序号", true, false); //@李国文

                map.AddDDLSQL(MapAttrAttr.CSSCtrl, "0", "自定义样式", MapAttrString.SQLOfCSSAttr, true);
                #endregion 傻瓜表单

                #region 执行的方法.
                RefMethod rm = new RefMethod();
                rm = new RefMethod();
                rm.Title = "正则表达式";
                rm.ClassMethodName = this.ToString() + ".DoRegularExpression()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-settings"; //正则表达式
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "事件绑函数";
                rm.ClassMethodName = this.ToString() + ".BindFunction()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-puzzle"; //事件绑定函数。
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "字段计算";
                rm.ClassMethodName = this.ToString() + ".DoAutoFull()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-energy"; //取多个字段计算结果.
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "对从表列计算";
                rm.ClassMethodName = this.ToString() + ".DoAutoFullDtlField()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-energy"; //取多个字段计算结果.
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "求两个日期天数";
                rm.ClassMethodName = this.ToString() + ".DoReqDays()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-energy"; //取多个字段计算结果.
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设置文本框RMB大写";
                rm.ClassMethodName = this.ToString() + ".DoRMBDaXie()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-wrench";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "输入范围限制";
                rm.ClassMethodName = this.ToString() + ".DoLimit()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-wrench";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "全局风格定义";
                rm.ClassMethodName = this.ToString() + ".DoGloValStyles()";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Icon = "icon-wrench";
                rm.RefAttrKey = MapAttrAttr.CSSCtrl;
                map.AddRefMethod(rm);
                #endregion 执行的方法.

                this._enMap = map;
                return this._enMap;
            }
        }




        protected override bool beforeUpdateInsertAction()
        {
            #region 修改默认值.
            //如果没默认值.
            if (DataType.IsNullOrEmpty(this.DefVal) && this.DefValType == 0)
                this.DefVal = MapAttrAttr.DefaultVal;

            MapData md = new MapData();
            md.No = this.FK_MapData;
            if (md.RetrieveFromDBSources() == 1)
            {
                //修改默认值.
                if (this.MyDataType == BP.DA.DataType.AppInt)
                    BP.DA.DBAccess.UpdateTableColumnDefaultVal(md.PTable, this.KeyOfEn, int.Parse(this.DefVal));
                if (this.MyDataType == BP.DA.DataType.AppDouble)
                    BP.DA.DBAccess.UpdateTableColumnDefaultVal(md.PTable, this.KeyOfEn, double.Parse(this.DefVal));
                if (this.MyDataType == BP.DA.DataType.AppFloat)
                    BP.DA.DBAccess.UpdateTableColumnDefaultVal(md.PTable, this.KeyOfEn, float.Parse(this.DefVal));
                if (this.MyDataType == BP.DA.DataType.AppMoney)
                    BP.DA.DBAccess.UpdateTableColumnDefaultVal(md.PTable, this.KeyOfEn, decimal.Parse(this.DefVal));
            }
            #endregion 修改默认值.


            MapAttr attr = new MapAttr();
            attr.setMyPK(this.MyPK);
            attr.RetrieveFromDBSources();

            //是否显示合计
            attr.IsSum = this.GetValBooleanByKey("ExtIsSum");
            attr.Update();

            return base.beforeUpdateInsertAction();
        }

        protected override void afterInsertUpdateAction()
        {
            MapAttr mapAttr = new MapAttr();
            mapAttr.setMyPK(this.MyPK);
            mapAttr.RetrieveFromDBSources();
            mapAttr.Update();

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);

            base.afterInsertUpdateAction();
        }

        /// <summary>
        /// 删除后清缓存
        /// </summary>
        protected override void afterDelete()
        {
            //删除相对应的rpt表中的字段
            if (this.FK_MapData.Contains("ND") == true)
            {
                string fk_mapData = this.FK_MapData.Substring(0, this.FK_MapData.Length - 2) + "Rpt";
                string sql = "DELETE FROM Sys_MapAttr WHERE FK_MapData='" + fk_mapData + "' AND KeyOfEn='" + this.KeyOfEn + "'";
                DBAccess.RunSQL(sql);
            }

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);
            base.afterDelete();
        }
        #endregion

        #region 基本功能.
        /// <summary>
        /// 人民币大写
        /// </summary>
        /// <returns></returns>
        public string DoRMBDaXie()
        {
            return "../../Admin/FoolFormDesigner/MapExt/RMBDaXie.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn;
        }
        /// <summary>
        /// 求天数
        /// </summary>
        /// <returns></returns>
        public string DoReqDays()
        {
            return "../../Admin/FoolFormDesigner/MapExt/ReqDays.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn;
        }
        /// <summary>
        /// 绑定函数
        /// </summary>
        /// <returns></returns>
        public string BindFunction()
        {
            return "../../Admin/FoolFormDesigner/MapExt/BindFunction.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn;
        }
        #endregion

        #region 方法执行.

        public string DoLimit()
        {
            return "../../Admin/FoolFormDesigner/MapExt/NumEnterLimit.htm?&MyPK=" + this.MyPK + "&FrmID=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn;
        }

        public string DoAutoFullDtlField()
        {
            return "../../Admin/FoolFormDesigner/MapExt/AutoFullDtlField.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn;
        }
        /// <summary>
        /// 自动计算
        /// </summary>
        /// <returns></returns>
        public string DoAutoFull()
        {
            return "../../Admin/FoolFormDesigner/MapExt/AutoFull.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn;
        }
        /// <summary>
        /// 设置开窗返回值
        /// </summary>
        /// <returns></returns>
        public string DoPopVal()
        {
            return "../../Admin/FoolFormDesigner/MapExt/PopVal.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&MyPK=" + this.MyPK;
        }
        /// <summary>
        /// 正则表达式
        /// </summary>
        /// <returns></returns>
        public string DoRegularExpression()
        {
            return "../../Admin/FoolFormDesigner/MapExt/RegularExpressionNum.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&MyPK=" + this.MyPK;
        }
        /// <summary>
        /// 文本框自动完成
        /// </summary>
        /// <returns></returns>
        public string DoTBFullCtrl()
        {
            return "../../Admin/FoolFormDesigner/MapExt/TBFullCtrl.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&MyPK=" + this.MyPK;
        }
        /// <summary>
        /// 扩展控件
        /// </summary>
        /// <returns></returns>
        public string DoEditFExtContral()
        {
            return "../../Admin/FoolFormDesigner/EditFExtContral.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&MyPK=" + this.MyPK;
        }
        public string DoGloValStyles()
        {
            return "../../Admin/FoolFormDesigner/StyletDfine/GloValStyles.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&MyPK=" + this.MyPK;
        }
        #endregion 方法执行.
    }
    /// <summary>
    /// 实体属性s
    /// </summary>
    public class MapAttrNums : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 实体属性s
        /// </summary>
        public MapAttrNums()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapAttrNum();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapAttrNum> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapAttrNum>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapAttrNum> Tolist()
        {
            System.Collections.Generic.List<MapAttrNum> list = new System.Collections.Generic.List<MapAttrNum>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapAttrNum)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
