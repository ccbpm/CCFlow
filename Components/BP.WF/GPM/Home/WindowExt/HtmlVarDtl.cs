using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.Web;
using BP.En;
using System.Text.RegularExpressions;


namespace BP.GPM.Home.WindowExt
{
    /// <summary>
    /// 变量信息
    /// </summary>
    public class HtmlVarDtl : EntityMyPK
    {
        #region 属性.
        /// <summary>
        /// 表达式
        /// </summary>
        public string RefPK
        {
            get
            {
                return this.GetValStrByKey(DtlAttr.RefPK);
            }
            set
            {
                this.SetValByKey(DtlAttr.RefPK, value);
            }
        }
        public string Exp0
        {
            get
            {
                return this.GetValStrByKey(DtlAttr.Exp0);
            }
            set
            {
                this.SetValByKey(DtlAttr.Exp0, value);
            }
        }
        public int DBType
        {
            get
            {
                return this.GetValIntByKey(DtlAttr.DBType);
            }
            set
            {
                this.SetValByKey(DtlAttr.DBType, value);
            }
        }
        public string FontColor
        {
            get
            {
                return this.GetValStrByKey(DtlAttr.FontColor);
            }
            set
            {
                this.SetValByKey(DtlAttr.FontColor, value);
            }
        }
        #endregion 属性.

        #region 权限控制.
        /// <summary>
        /// 控制权限
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();

                uac.IsInsert = true;
                uac.IsDelete = true;
                uac.IsView = true;
                uac.IsUpdate = true;
                return uac;
            }
        }
        #endregion 权限控制.

        #region 属性
        #endregion 属性

        #region 构造方法
        /// <summary>
        /// 变量信息
        /// </summary>
        public HtmlVarDtl()
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

                Map map = new Map("GPM_WindowTemplateDtl", "数据项");

                map.AddMyPK(false);
                map.AddTBString(DtlAttr.RefPK, null, "RefPK", false, false, 0, 40, 20, false);
                map.AddTBString(DtlAttr.Name, null, "标签", true, false, 0, 300, 70, true);

                map.AddDDLSysEnum(DtlAttr.DBType, 0, "数据源类型", true, true, "WindowsDBType",
          "@0=数据库查询SQL@1=执行Url返回Json@2=执行\\DataUser\\JSLab\\Windows.js的函数.");
                map.AddDDLEntities(DtlAttr.DBSrc, null, "数据源", new BP.Sys.SFDBSrcs(), true);

                map.AddTBString(DtlAttr.Exp0, null, "表达式", true, false, 0, 300, 700, true);
                map.AddTBString(DtlAttr.FontColor, null, "字体风格", true, false, 0, 300, 100, true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            this.setMyPK(DBAccess.GenerGUID());
            return base.beforeInsert();
        }

    }
    /// <summary>
    /// 变量信息s
    /// </summary>
    public class HtmlVarDtls : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 变量信息s
        /// </summary>
        public HtmlVarDtls()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new HtmlVarDtl();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<HtmlVarDtl> ToJavaList()
        {
            return (System.Collections.Generic.IList<HtmlVarDtl>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<HtmlVarDtl> Tolist()
        {
            System.Collections.Generic.List<HtmlVarDtl> list = new System.Collections.Generic.List<HtmlVarDtl>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((HtmlVarDtl)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
