using System;
using System.Collections;
using BP.DA;
using BP.En;
namespace BP.Sys
{
    /// <summary>
    /// 附件在扩展控件里的显示方式
    /// </summary>
    public enum AthShowModel
    {
        /// <summary>
        /// 简单的
        /// </summary>
        Simple,
        /// <summary>
        /// 只有文件名称
        /// </summary>
        FileNameOnly
    }
      
    /// <summary>
    /// 扩展控件属性
    /// </summary>
    public class ExtContralAttr : EntityOIDNameAttr
    {
        /// <summary>
        /// 主表
        /// </summary>
        public const string FK_MapData = "FK_MapData";
        /// <summary>
        /// X1
        /// </summary>
        public const string X1 = "X1";
        /// <summary>
        /// Y1
        /// </summary>
        public const string Y1 = "Y1";
        /// <summary>
        /// X2
        /// </summary>
        public const string X2 = "X2";
        /// <summary>
        /// Y2
        /// </summary>
        public const string Y2 = "Y2";
        /// <summary>
        /// 宽度
        /// </summary>
        public const string BorderWidth = "BorderWidth";
        /// <summary>
        /// 颜色
        /// </summary>
        public const string BorderColor = "BorderColor";
        /// <summary>
        /// GUID
        /// </summary>
        public const string GUID = "GUID";
    }
    /// <summary>
    /// 扩展控件
    /// </summary>
    public class ExtContral : EntityMyPK
    {
        #region 基本-属性
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.GetValStrByKey(MapAttrAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.FK_MapData, value);
            }
        }
        /// <summary>
        /// 字段名
        /// </summary>
        public string KeyOfEn
        {
            get
            {
                return this.GetValStrByKey(MapAttrAttr.KeyOfEn);
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
        #endregion

        #region 附件属性 
        /// <summary>
        /// 关联的字段.
        /// </summary>
        public string AthRefObj
        {
            get
            {
                return this.GetParaString("AthRefObj");
            }
            set
            {
                this.SetPara("AthRefObj", value);
            }
        }
        /// <summary>
        /// 显示方式
        /// </summary>
        public AthShowModel AthShowModel
        {
            get
            {
                return (AthShowModel)this.GetParaInt("AthShowModel");
            }
            set
            {
                this.SetPara("AthShowModel", (int)value);
            }
        }
        #endregion 附件属性

       

        #region 构造方法
        /// <summary>
        /// 扩展控件
        /// </summary>
        public ExtContral()
        {
        }
        public ExtContral(string fk_mapdata, string keyofEn)
        {
            this.MyPK = fk_mapdata + "_" + keyofEn;
            this.Retrieve();
        }
        public ExtContral(string mypk)
        {
            this.MyPK = mypk;
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
                Map map = new Map("Sys_MapAttr", "扩展控件");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetEnType(EnType.Sys);

                map.AddMyPK();
                map.AddTBString(MapAttrAttr.FK_MapData, null, "实体标识", true, true, 1, 100, 20);
                map.AddTBString(MapAttrAttr.KeyOfEn, null, "属性", true, true, 1, 200, 20);

                map.AddTBString(MapAttrAttr.Name, null, "描述", true, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.DefVal, null, "默认值", false, false, 0, 4000, 20);

                map.AddTBInt(MapAttrAttr.UIContralType, 0, "控件", true, false);
                map.AddTBInt(MapAttrAttr.MyDataType, 0, "数据类型", true, false);

                map.AddDDLSysEnum(MapAttrAttr.LGType, 0, "逻辑类型", true, false, MapAttrAttr.LGType,
                    "@0=普通@1=枚举@2=外键@3=打开系统页面");

                map.AddTBFloat(MapAttrAttr.UIWidth, 100, "宽度", true, false);
                map.AddTBFloat(MapAttrAttr.UIHeight, 23, "高度", true, false);

                map.AddTBInt(MapAttrAttr.MinLen, 0, "最小长度", true, false);
                map.AddTBInt(MapAttrAttr.MaxLen, 300, "最大长度", true, false);

                map.AddTBString(MapAttrAttr.UIBindKey, null, "绑定的信息", true, false, 0, 100, 20);
                map.AddTBString(MapAttrAttr.UIRefKey, null, "绑定的Key", true, false, 0, 30, 20);
                map.AddTBString(MapAttrAttr.UIRefKeyText, null, "绑定的Text", true, false, 0, 30, 20);
               

                map.AddBoolean(MapAttrAttr.UIVisible, true, "是否可见", true, true);
                map.AddBoolean(MapAttrAttr.UIIsEnable, true, "是否启用", true, true);
                map.AddBoolean(MapAttrAttr.UIIsLine, false, "是否单独栏显示", true, true);
                map.AddBoolean(MapAttrAttr.UIIsInput, false, "是否必填字段", true, true);


                map.AddTBFloat(MapAttrAttr.X, 5, "X", true, false);
                map.AddTBFloat(MapAttrAttr.Y, 5, "Y", false, false);


                map.AddTBString(MapAttrAttr.Tag, null, "标识（存放临时数据）", true, false, 0, 100, 20);
                map.AddTBInt(MapAttrAttr.EditType, 0, "编辑类型", true, false);

                //单元格数量。2013-07-24 增加。
                map.AddTBString(MapAttrAttr.ColSpan, "1", "单元格数量", true, false, 0, 3, 3);
             //   map.AddTBInt(MapAttrAttr.ColSpan, 1, "单元格数量", true, false);

                map.AddTBInt(MapAttrAttr.Idx, 0, "序号", true, false);
                map.AddTBString(MapAttrAttr.GUID, null, "GUID", true, false, 0, 128, 20);

                //参数属性.
                map.AddTBAtParas(4000); //

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
       
    }
    /// <summary>
    /// 扩展控件s
    /// </summary>
    public class ExtContrals : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 扩展控件s
        /// </summary>
        public ExtContrals()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ExtContral();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<ExtContral> ToJavaList()
        {
            return (System.Collections.Generic.IList<ExtContral>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<ExtContral> Tolist()
        {
            System.Collections.Generic.List<ExtContral> list = new System.Collections.Generic.List<ExtContral>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((ExtContral)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
