﻿using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.Sys.FrmUI
{
      
    /// <summary>
    /// 外键字段
    /// </summary>
    public class MapAttrSFTable : EntityMyPK
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
        /// 外键字段
        /// </summary>
        public MapAttrSFTable()
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

                Map map = new Map("Sys_MapAttr", "外键字段");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap(Depositary.Application);
                map.Java_SetEnType(EnType.Sys);

                #region 基本信息.
                map.AddTBStringPK(MapAttrAttr.MyPK, null, "主键", false, false, 0, 300, 20);
                map.AddTBString(MapAttrAttr.FK_MapData, null, "实体标识", false, false, 1, 100, 20);

                map.AddTBString(MapAttrAttr.Name, null, "字段中文名", true, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.KeyOfEn, null, "字段名", true, true, 1, 200, 20);

                //默认值.
                map.AddDDLSysEnum(MapAttrAttr.LGType, 4, "类型", true, false);
                map.AddTBString(MapAttrAttr.DefVal, null, "默认值", true, false, 0, 300, 20);

                map.AddTBFloat(MapAttrAttr.UIWidth, 100, "宽度", true, false);
                map.AddTBFloat(MapAttrAttr.UIHeight, 23, "高度", true, true);

                map.AddTBString(MapAttrAttr.UIBindKey, null, "外键SFTable", true, true, 0, 100, 20);

                map.AddBoolean(MapAttrAttr.UIVisible, true, "是否可见", true, true);
                map.AddBoolean(MapAttrAttr.UIIsEnable, true, "是否启用", true, true);

               // map.AddBoolean(MapAttrAttr.UIIsInput, false, "是否必填项？", true, true);
               // map.AddBoolean("IsEnableJS", false, "是否启用JS高级设置？", true, true); //参数字段.

                #endregion 基本信息.

                #region 傻瓜表单。
                map.AddDDLSysEnum(MapAttrAttr.ColSpan, 1, "单元格数量", true, true, "ColSpanAttrDT", "@1=跨1个单元格@3=跨3个单元格");

                //显示的分组.
                map.AddDDLSQL(MapAttrAttr.GroupID, "0", "显示的分组",
                    "SELECT OID as No, Lab as Name FROM Sys_GroupField WHERE EnName='@FK_MapData'", true);
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
                rm.Title = "外键表属性";
                rm.ClassMethodName = this.ToString() + ".DoSFTable()";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.GroupName = "高级";
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "旧版本设置aspx";
                //rm.ClassMethodName = this.ToString() + ".DoOldVerAspx()";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.GroupName = "高级设置";
                //map.AddRefMethod(rm);


                //rm = new RefMethod();
                //rm.Title = "旧版本设置htm";
                //rm.ClassMethodName = this.ToString() + ".DoOldVer()";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.GroupName = "高级设置";
                //map.AddRefMethod(rm);
                #endregion 执行的方法.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 方法执行.
        /// <summary>
        /// 旧版本设置
        /// </summary>
        /// <returns></returns>
        public string DoOldVer()
        {
            return "/WF/Admin/FoolFormDesigner/EditTable.htm?KeyOfEn=" + this.KeyOfEn + "&EnumKey=" + this.UIBindKey + "&MyPK=" + this.MyPK + "&UIBindKey=" + this.UIBindKey;
        }
        public string DoOldVerAspx()
        {
            return "/WF/Admin/FoolFormDesigner/EditTable.aspx?KeyOfEn=" + this.KeyOfEn + "&EnumKey=" + this.UIBindKey + "&MyPK=" + this.MyPK + "&UIBindKey=" + this.UIBindKey;
        }
        /// <summary>
        /// 外键表属性
        /// </summary>
        /// <returns></returns>
        public string DoSFTable()
        {
            return "/WF/Admin/FoolFormDesigner/GuideSFTableAttr.htm?FK_SFTable=" + this.UIBindKey;
        }
        /// <summary>
        /// 设置自动填充
        /// </summary>
        /// <returns></returns>
        public string DoDDLFullCtrl()
        {
            return "/WF/Admin/FoolFormDesigner/MapExt/DDLFullCtrl.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=AutoFull&KeyOfEn=" + this.KeyOfEn + "&RefNo=" + this.MyPK;
        }
        /// <summary>
        /// 设置级联
        /// </summary>
        /// <returns></returns>
        public string DoActiveDDL()
        {
            return "/WF/Admin/FoolFormDesigner/MapExt/ActiveDDL.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=AutoFull&KeyOfEn=" + this.KeyOfEn + "&RefNo=" + this.MyPK;
        }
        #endregion 方法执行.
    }
    /// <summary>
    /// 实体属性s
    /// </summary>
    public class MapAttrSFTables : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 实体属性s
        /// </summary>
        public MapAttrSFTables()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapAttrSFTable();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapAttrSFTable> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapAttrSFTable>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapAttrSFTable> Tolist()
        {
            System.Collections.Generic.List<MapAttrSFTable> list = new System.Collections.Generic.List<MapAttrSFTable>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapAttrSFTable)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
