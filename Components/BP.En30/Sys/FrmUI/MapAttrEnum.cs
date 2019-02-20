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
    /// 枚举字段
    /// </summary>
    public class MapAttrEnum : EntityMyPK
    {
        #region 文本字段参数属性.
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
        /// 枚举字段
        /// </summary>
        public MapAttrEnum()
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

                Map map = new Map("Sys_MapAttr", "枚举字段");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap(Depositary.Application);
                map.Java_SetEnType(EnType.Sys);

                #region 基本信息.
                map.AddTBStringPK(MapAttrAttr.MyPK, null, "主键", false, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.FK_MapData, null, "实体标识", false, false, 1, 100, 20);

                map.AddTBString(MapAttrAttr.Name, null, "字段中文名", true, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.KeyOfEn, null, "字段名", true, true, 1, 200, 20);

                //默认值.
                map.AddDDLSQL(MapAttrAttr.DefVal, "0", "默认值（选中）",
                    "SELECT  IntKey as No, Lab as Name FROM Sys_Enum where EnumKey='@UIBindKey'", true);

              //  map.AddTBString(MapAttrAttr.DefVal, "0", "默认值", true, true, 0, 3000, 20);

                map.AddDDLSysEnum(MapAttrAttr.UIContralType, 0, "控件类型", true, true, "EnumUIContralType",
                 "@1=下拉框@3=单选按钮");

                map.AddDDLSysEnum("RBShowModel", 0, "单选按钮的展现方式", true, true, "RBShowModel",
            "@0=竖向@3=横向");

                //map.AddDDLSysEnum(MapAttrAttr.LGType, 0, "逻辑类型", true, false, MapAttrAttr.LGType, 
                // "@0=普通@1=枚举@2=外键@3=打开系统页面");

                map.AddTBFloat(MapAttrAttr.UIWidth, 100, "宽度", true, false);
                map.AddTBFloat(MapAttrAttr.UIHeight, 23, "高度", true, true);

                map.AddTBString(MapAttrAttr.UIBindKey, null, "枚举ID", true, true, 0, 100, 20);

                map.AddBoolean(MapAttrAttr.UIVisible, true, "是否可见?", true, true);
                map.AddBoolean(MapAttrAttr.UIIsEnable, true, "是否可编辑?", true, true);

                map.AddBoolean(MapAttrAttr.UIIsInput, false, "是否必填项？", true, true);

                map.AddBoolean("IsEnableJS", false, "是否启用JS高级设置？", true, true); //参数字段.
                #endregion 基本信息.

                #region 傻瓜表单。
                //单元格数量 2013-07-24 增加。
                map.AddDDLSysEnum(MapAttrAttr.ColSpan, 1, "单元格数量", true, true, "ColSpanAttrDT", "@1=跨1个单元格@3=跨3个单元格");


                //显示的分组.
                map.AddDDLSQL(MapAttrAttr.GroupID, "0", "显示的分组", MapAttrString.SQLOfGroupAttr, true);
                map.AddTBInt(MapAttrAttr.Idx, 0, "顺序号", true, false); //@李国文

                #endregion 傻瓜表单。

                #region 执行的方法.
                RefMethod rm = new RefMethod();

                rm = new RefMethod();
                rm.Title = "设置联动";
                rm.ClassMethodName = this.ToString() + ".DoActiveDDL()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "填充其他控件";
                rm.ClassMethodName = this.ToString() + ".DoDDLFullCtrl()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "填充其他控件v2019";
                rm.ClassMethodName = this.ToString() + ".DoDDLFullCtrl2019()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "编辑枚举值";
                rm.ClassMethodName = this.ToString() + ".DoSysEnum()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "高级JS设置";
                rm.ClassMethodName = this.ToString() + ".DoRadioBtns()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "高级设置";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "事件绑函数";
                rm.ClassMethodName = this.ToString() + ".BindFunction()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                #endregion 执行的方法.

                this._enMap = map;
                return this._enMap;
            }
        }

        protected override bool beforeUpdateInsertAction()
        {
            MapAttr attr = new MapAttr();
            attr.MyPK = this.MyPK;
            attr.RetrieveFromDBSources();

            //是否启用高级js设置.
            attr.IsEnableJS = this.GetValBooleanByKey("IsEnableJS");

            //单选按钮的展现方式.
            attr.RBShowModel = this.GetValIntByKey("RBShowModel");

            //执行保存.
            attr.Save();

            return base.beforeUpdateInsertAction();
        }
        #endregion

        protected override void afterDelete()
        {
            //删除可能存在的数据.
            BP.DA.DBAccess.RunSQL("DELETE FROM Sys_FrmRB WHERE KeyOfEn='" + this.KeyOfEn + "' AND FK_MapData='" + this.FK_MapData + "'");
            base.afterDelete();
        }

        #region 基本功能.
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
        /// <summary>
        /// 编辑枚举值
        /// </summary>
        /// <returns></returns>
        public string DoSysEnum()
        {
            return "../../Admin/CCFormDesigner/DialogCtr/EnumerationNew.htm?DoType=FrmEnumeration_SaveEnum&EnumKey=" + this.UIBindKey;
        }
        /// <summary>
        /// 设置自动填充
        /// </summary>
        /// <returns></returns>
        public string DoDDLFullCtrl()
        {
            return "../../Admin/FoolFormDesigner/MapExt/DDLFullCtrl.htm?FK_MapData=" + this.FK_MapData + "&ExtType=AutoFull&KeyOfEn=" + HttpUtility.UrlEncode(this.KeyOfEn) + "&RefNo=" + HttpUtility.UrlEncode(this.MyPK);
        }
         public string DoDDLFullCtrl2019()
        {
            return "../../Admin/FoolFormDesigner/DDLSetting/Default.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn;
        }

        /// <summary>
        /// 设置自动填充
        /// </summary>
        /// <returns></returns>
        public string DoAutoFull()
        {
            return "../../Admin/FoolFormDesigner/MapExt/AutoFullDLL.htm?FK_MapData=" + this.FK_MapData + "&ExtType=AutoFull&KeyOfEn=" + HttpUtility.UrlEncode(this.KeyOfEn) + "&RefNo=" + HttpUtility.UrlEncode(this.MyPK);
        }
        /// <summary>
        /// 高级设置
        /// </summary>
        /// <returns></returns>
        public string DoRadioBtns()
        {
            return "../../Admin/FoolFormDesigner/MapExt/RadioBtns.htm?FK_MapData=" + this.FK_MapData + "&ExtType=AutoFull&KeyOfEn=" + HttpUtility.UrlEncode(this.KeyOfEn) + "&RefNo=" + HttpUtility.UrlEncode(this.MyPK);
        }
        /// <summary>
        /// 设置级联
        /// </summary>
        /// <returns></returns>
        public string DoActiveDDL()
        {
           return "../../Admin/FoolFormDesigner/MapExt/ActiveDDL.htm?FK_MapData=" + this.FK_MapData + "&ExtType=AutoFull&KeyOfEn=" + HttpUtility.UrlEncode(this.KeyOfEn) + "&RefNo=" + HttpUtility.UrlEncode(this.MyPK);
        }
       
        #endregion 方法执行.
    }
    /// <summary>
    /// 实体属性s
    /// </summary>
    public class MapAttrEnums : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 实体属性s
        /// </summary>
        public MapAttrEnums()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapAttrEnum();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapAttrEnum> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapAttrEnum>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapAttrEnum> Tolist()
        {
            System.Collections.Generic.List<MapAttrEnum> list = new System.Collections.Generic.List<MapAttrEnum>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapAttrEnum)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
