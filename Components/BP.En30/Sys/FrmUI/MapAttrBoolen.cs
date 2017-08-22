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
    /// Boolen字段
    /// </summary>
    public class MapAttrBoolen : EntityMyPK
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
        /// Boolen字段
        /// </summary>
        public MapAttrBoolen()
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

                Map map = new Map("Sys_MapAttr", "Boolen字段");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap(Depositary.Application);
                map.Java_SetEnType(EnType.Sys);

                #region 基本信息.

                map.AddTBStringPK(MapAttrAttr.MyPK, null, "主键", false, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.FK_MapData, null, "实体标识", false, false, 1, 100, 20);

                map.AddTBString(MapAttrAttr.Name, null, "字段中文名", true, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.KeyOfEn, null, "字段名", true, true, 1, 200, 20);

                //数据类型.
                map.AddDDLSysEnum(MapAttrAttr.MyDataType, 4, "数据类型", true, false);

                map.AddBoolean(MapAttrAttr.UIVisible, true, "是否可见？", true, true);

                map.AddTBString(MapAttrAttr.DefVal, "0", "默认值(是否选中？0=否,1=是)", true, false, 0, 200, 20);

                map.AddBoolean(MapAttrAttr.UIIsEnable, true, "是否可编辑？", true, true);
                map.AddTBString(MapAttrAttr.Tip, null, "激活提示", true, false, 0, 800, 20, true);
                #endregion 基本信息.

                #region 傻瓜表单。
                //单元格数量 2013-07-24 增加。
                map.AddDDLSysEnum(MapAttrAttr.ColSpan, 1, "单元格数量", true, true, "ColSpanAttrString",
                  "@1=跨1个单元格@3=跨3个单元格@4=跨4个单元格");

                //显示的分组.
                map.AddDDLSQL(MapAttrAttr.GroupID, "0", "显示的分组",
                    "SELECT OID as No, Lab as Name FROM Sys_GroupField WHERE EnName='@FK_MapData'", true);
                #endregion 傻瓜表单。

                #region 执行的方法.
                RefMethod rm = new RefMethod();

                rm = new RefMethod();
                rm.Title = "旧版本设置";
                rm.ClassMethodName = this.ToString() + ".DoOldVer()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

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
            return "/WF/Admin/FoolFormDesigner/EditF.htm?KeyOfEn=" + HttpUtility.UrlEncode(this.KeyOfEn) + "&FType=" + this.MyDataType + "&MyPK=" + HttpUtility.UrlEncode(this.MyPK) + "&FK_MapData=" + this.FK_MapData;
        }
        #endregion 方法执行.
    }
    /// <summary>
    /// 实体属性s
    /// </summary>
    public class MapAttrBoolens : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 实体属性s
        /// </summary>
        public MapAttrBoolens()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapAttrBoolen();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapAttrBoolen> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapAttrBoolen>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapAttrBoolen> Tolist()
        {
            System.Collections.Generic.List<MapAttrBoolen> list = new System.Collections.Generic.List<MapAttrBoolen>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapAttrBoolen)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
