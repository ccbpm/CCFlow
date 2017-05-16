using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.Sys.FrmUI
{
    /// <summary>
    /// 实体属性
    /// </summary>
    public class MapAttrString : EntityMyPK
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
        /// 实体属性
        /// </summary>
        public MapAttrString()
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

                Map map = new Map("Sys_MapAttr", "文本字段");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap(Depositary.Application);
                map.Java_SetEnType(EnType.Sys);

                #region 基本信息.
                map.AddTBStringPK(MapAttrAttr.MyPK, null, "主键", false, false, 0, 300, 20);
                map.AddTBString(MapAttrAttr.FK_MapData, null, "实体标识", false, false, 1, 100, 20);

                map.AddTBString(MapAttrAttr.Name, null, "字段中文名", true, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.KeyOfEn, null, "字段名", true, true, 1, 200, 20);


                //默认值.
                string sql = "SELECT No,Name FROM Sys_GloVar WHERE GroupKey='DefVal'";
                //显示的分组.
                map.AddDDLSQL("ExtDefVal", "0", "系统默认值", sql, true);

                map.AddTBString(MapAttrAttr.DefVal, null, "默认值表达式", true, false, 0, 3000, 20);

                map.AddTBFloat(MapAttrAttr.MinLen, 0, "最小长度", true, false);
                map.AddTBFloat(MapAttrAttr.MaxLen, 50, "最大长度", true, false);

                map.AddTBFloat(MapAttrAttr.UIWidth, 100, "宽度", true, false);
                //map.AddTBFloat(MapAttrAttr.UIHeight, 23, "高度", false, false);
                map.AddTBFloat("ExtRows", 1, "文本框行数(决定高度)", true, false);


                map.AddBoolean(MapAttrAttr.UIVisible, true, "是否可见？", true, true);
                map.AddBoolean(MapAttrAttr.UIIsEnable, true, "是否可编辑？", true, true);
                map.AddBoolean(MapAttrAttr.UIIsInput, false, "是否必填项？", true, true);
                map.AddBoolean("IsRichText", false, "是否大块文本？", true, true);
                map.AddBoolean("IsSupperText", false, "是否富文本？", true, true);
                map.AddTBString(MapAttrAttr.Tip, null, "激活提示", true, false, 0, 500, 20,true);

                #endregion 基本信息.

                #region 傻瓜表单。
                //单元格数量 2013-07-24 增加
                map.AddDDLSysEnum(MapAttrAttr.ColSpan, 1, "单元格数量", true, true, "ColSpanAttrString", 
                    "@1=跨1个单元格@3=跨3个单元格@4=跨4个单元格");

                //显示的分组.
                map.AddDDLSQL(MapAttrAttr.GroupID, "0", "显示的分组",
                    "SELECT OID as No, Lab as Name FROM Sys_GroupField WHERE EnName='@FK_MapData'", true);

                map.AddDDLSysEnum(MapAttrAttr.IsSigan, 0, "签名模式", true, true,
                    MapAttrAttr.IsSigan, "@0=无@1=图片签名@2=山东CA@3=广东CA");
                #endregion 傻瓜表单。

                #region 执行的方法.
                RefMethod rm = new RefMethod();

              //  设置开窗返回值-正则表达式-文本框自动完成-脚本验证-扩展控件
                rm = new RefMethod();
                rm.Title = "设置开窗返回值";
                rm.ClassMethodName = this.ToString() + ".DoPopVal()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "正则表达式";
                rm.ClassMethodName = this.ToString() + ".DoRegularExpression()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "文本框自动完成";
                rm.ClassMethodName = this.ToString() + ".DoTBFullCtrl()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "脚本验证";
                rm.ClassMethodName = this.ToString() + ".DoInputCheck()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);
                
                rm = new RefMethod();
                rm.Title = "扩展控件";
                rm.ClassMethodName = this.ToString() + ".DoEditFExtContral()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "旧版本设置htm";
                rm.ClassMethodName = this.ToString() + ".DoOldVer()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "高级设置";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "旧版本设置aspx";
                rm.ClassMethodName = this.ToString() + ".DoOldVerAspx()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "高级设置";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "批处理";
                rm.ClassMethodName = this.ToString() + ".DoEleBatch()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "高级设置";
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

            //高度.
            attr.UIHeightInt = this.GetValIntByKey("ExtRows") * 23;

            attr.IsRichText = this.GetValBooleanByKey("IsRichText"); //是否是富文本？
            attr.IsSupperText = this.GetValBooleanByKey("IsSupperText"); //是否是大块文本？

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

            return base.beforeUpdateInsertAction();
        }
        #endregion

        #region 方法执行.
        /// <summary>
        /// 批处理
        /// </summary>
        /// <returns></returns>
        public string DoEleBatch()
        {
            return "/WF/Admin/FoolFormDesigner/EleBatch.aspx?EleType=MapAttr&KeyOfEn=" + this.KeyOfEn + "&FType=1&MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData;
        }
        public string DoOldVerAspx()
        {
            return "/WF/Admin/FoolFormDesigner/EditF.aspx?DoType=Edit&KeyOfEn=" + this.KeyOfEn + "&FType=1&MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData;
        }
        /// <summary>
        /// 旧版本设置
        /// </summary>
        /// <returns></returns>
        public string DoOldVer()
        {
            return "/WF/Admin/FoolFormDesigner/EditF.htm?KeyOfEn=" + this.KeyOfEn + "&FType=1&MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData;
        }

        /// <summary>
        /// 设置开窗返回值
        /// </summary>
        /// <returns></returns>
        public string DoPopVal()
        {
            return "/WF/Admin/FoolFormDesigner/MapExt/PopVal.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&MyPK=" + this.MyPK + "&FK_MapExt=PopVal_"+this.FK_MapData+"_"+this.KeyOfEn;
        }

        /// <summary>
        /// 正则表达式
        /// </summary>
        /// <returns></returns>
        public string DoRegularExpression()
        {
            return "/WF/Admin/FoolFormDesigner/MapExt/RegularExpression.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&MyPK=" + this.MyPK;
        }
        /// <summary>
        /// 文本框自动完成
        /// </summary>
        /// <returns></returns>
        public string DoTBFullCtrl()
        {
            return "/WF/Admin/FoolFormDesigner/MapExt/TBFullCtrl.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&MyPK=" + this.MyPK;
            //return "/WF/Admin/FoolFormDesigner/MapExt/TBFullCtrl.htm?FK_MapData=" + this.FK_MapData + "&ExtType=AutoFull&KeyOfEn=" + this.KeyOfEn + "&RefNo=" + this.MyPK;
        }

        /// <summary>
        /// 设置级联
        /// </summary>
        /// <returns></returns>
        public string DoInputCheck()
        {
            return "/WF/Admin/FoolFormDesigner/MapExt/InputCheck.aspx?FK_MapData=" + this.FK_MapData + "&OperAttrKey=" + this.KeyOfEn + "&RefNo=" + this.MyPK + "&DoType=New&ExtType=InputCheck";

           // return "/WF/Admin/FoolFormDesigner/MapExt/InputCheck.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn  +"&RefNo="+this.MyPK;
          //  return "/WF/Admin/FoolFormDesigner/MapExt/InputCheck.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=AutoFull&KeyOfEn=" + this.KeyOfEn + "&RefNo=" + this.MyPK;
        }
        /// <summary>
        /// 扩展控件
        /// </summary>
        /// <returns></returns>
        public string DoEditFExtContral()
        {
            return "/WF/Admin/FoolFormDesigner/EditFExtContral.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&MyPK=" + this.MyPK;
            //  return "/WF/Admin/FoolFormDesigner/MapExt/InputCheck.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=AutoFull&KeyOfEn=" + this.KeyOfEn + "&RefNo=" + this.MyPK;
        }
        #endregion 方法执行.
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
