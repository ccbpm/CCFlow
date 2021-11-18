using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.Web;
using BP.En;
using System.Text.RegularExpressions;


namespace BP.GPM.Home.WindowExt
{
    public class DtlAttr : EntityNoNameAttr
    {
        public const string FontColor = "FontColor";
        public const string DBSrc = "DBSrc";
        public const string DBType = "DBType";
        public const string Exp0 = "Exp0";
        public const string Exp1 = "Exp1";

        public const string RefPK = "RefPK";

        /// <summary>
        /// 显示类型
        /// </summary>
        public const string WindowsShowType = "WindowsShowType";
    }
    /// <summary>
    /// 变量信息
    /// </summary>
    public class Dtl : EntityMyPK
    {
        #region 权限控制.
        /// <summary>
        /// 控制权限
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.IsAdmin == true)
                    uac.OpenAll();
                else
                    uac.IsView = false;

                uac.IsInsert = false;
                uac.IsDelete = false;

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
        public Dtl()
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

                Map map = new Map("GPM_WindowTemplateDtl", "Dtl变量信息");

                map.AddMyPK();
                map.AddTBString(DtlAttr.Name, null, "标签", true, false, 0, 300, 20, true);
                map.AddTBString(DtlAttr.FontColor, null, "颜色", true, false, 0, 300, 20, true);
                map.AddTBString(DtlAttr.Exp0, null, "表达式0", true, false, 0, 300, 20, true);
                map.AddTBString(DtlAttr.Exp1, null, "表达式1", true, false, 0, 300, 20, true);
                map.AddTBString(DtlAttr.DBSrc, null, "数据源", true, false, 0, 100, 20, true);

                map.AddTBInt(DtlAttr.WindowsShowType, 0, "显示类型", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

    }
    /// <summary>
    /// 变量信息s
    /// </summary>
    public class Dtls : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 变量信息s
        /// </summary>
        public Dtls()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Dtl();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Dtl> ToJavaList()
        {
            return (System.Collections.Generic.IList<Dtl>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Dtl> Tolist()
        {
            System.Collections.Generic.List<Dtl> list = new System.Collections.Generic.List<Dtl>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Dtl)this[i]);

            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
