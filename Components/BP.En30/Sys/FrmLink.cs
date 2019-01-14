using System;
using System.Collections;
using BP.DA;
using BP.En;
namespace BP.Sys
{
    /// <summary>
    /// 超连接
    /// </summary>
    public class FrmLinkAttr : EntityMyPKAttr
    {
        /// <summary>
        /// Text
        /// </summary>
        public const string Text = "Text";
        /// <summary>
        /// 主表
        /// </summary>
        public const string FK_MapData = "FK_MapData";
        /// <summary>
        /// Target
        /// </summary>
        public const string Target = "Target";
        public const string URL = "URL";
        /// <summary>
        /// X
        /// </summary>
        public const string X = "X";
        /// <summary>
        /// Y
        /// </summary>
        public const string Y = "Y";
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
        public const string FontSize = "FontSize";
        /// <summary>
        /// 颜色
        /// </summary>
        public const string FontColor = "FontColor";
        /// <summary>
        /// FontName
        /// </summary>
        public const string FontName = "FontName";
        /// <summary>
        /// 字体风格
        /// </summary>
        public const string FontStyle = "FontStyle";
        /// <summary>
        /// GUID
        /// </summary>
        public const string GUID = "GUID";
    }
    /// <summary>
    /// 超连接
    /// </summary>
    public class FrmLink : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// FontStyle
        /// </summary>
        public string FontStyle
        {
            get
            {
                return this.GetValStringByKey(FrmLinkAttr.FontStyle);
            }
            set
            {
                this.SetValByKey(FrmLinkAttr.FontStyle, value);
            }
        }
        public string FontColorHtml
        {
            get
            {
                return PubClass.ToHtmlColor(this.FontColor);
            }
        }
        /// <summary>
        /// FontColor
        /// </summary>
        public string FontColor
        {
            get
            {
                return this.GetValStringByKey(FrmLinkAttr.FontColor);
            }
            set
            {
                this.SetValByKey(FrmLinkAttr.FontColor, value);
            }
        }
        public string URL
        {
            get
            {
                return this.GetValStringByKey(FrmLinkAttr.URL).Replace("#","@");
            }
            set
            {
                this.SetValByKey(FrmLinkAttr.URL, value);
            }
        }
        /// <summary>
        /// Font
        /// </summary>
        public string FontName
        {
            get
            {
                return this.GetValStringByKey(FrmLinkAttr.FontName);
            }
            set
            {
                this.SetValByKey(FrmLinkAttr.FontName, value);
            }
        }
        /// <summary>
        /// Y
        /// </summary>
        public float Y
        {
            get
            {
                return this.GetValFloatByKey(FrmLinkAttr.Y);
            }
            set
            {
                this.SetValByKey(FrmLinkAttr.Y, value);
            }
        }
        /// <summary>
        /// X
        /// </summary>
        public float X
        {
            get
            {
                return this.GetValFloatByKey(FrmLinkAttr.X);
            }
            set
            {
                this.SetValByKey(FrmLinkAttr.X, value);
            }
        }
        /// <summary>
        /// FontSize
        /// </summary>
        public int FontSize
        {
            get
            {
                return this.GetValIntByKey(FrmLinkAttr.FontSize);
            }
            set
            {
                this.SetValByKey(FrmLinkAttr.FontSize, value);
            }
        }
        /// <summary>
        /// FK_MapData
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.GetValStrByKey(FrmLinkAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(FrmLinkAttr.FK_MapData, value);
            }
        }
        /// <summary>
        /// Text
        /// </summary>
        public string Text
        {
            get
            {
                return this.GetValStrByKey(FrmLinkAttr.Text);
            }
            set
            {
                this.SetValByKey(FrmLinkAttr.Text, value);
            }
        }
        public string Target
        {
            get
            {
                return this.GetValStringByKey(FrmLinkAttr.Target);
            }
            set
            {
                this.SetValByKey(FrmLinkAttr.Target, value);
            }
        }
        public bool IsBold
        {
            get
            {
                return this.GetValBooleanByKey(FrmLabAttr.IsBold);
            }
            set
            {
                this.SetValByKey(FrmLabAttr.IsBold, value);
            }
        }
        public bool IsItalic
        {
            get
            {
                return this.GetValBooleanByKey(FrmLabAttr.IsItalic);
            }
            set
            {
                this.SetValByKey(FrmLabAttr.IsItalic, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 超连接
        /// </summary>
        public FrmLink()
        {
        }
        /// <summary>
        /// 超连接
        /// </summary>
        /// <param name="mypk"></param>
        public FrmLink(string mypk)
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
                Map map = new Map("Sys_FrmLink", "超连接");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetEnType(EnType.Sys);

                map.AddMyPK();

                map.AddTBString(FrmLinkAttr.FK_MapData, null, "FK_MapData", true, false, 1, 100, 20);
                map.AddTBString(FrmLinkAttr.Text, "New Link", "Label", true, false, 0, 500, 20);

                map.AddTBString(FrmLinkAttr.URL, null, "URL", true, false, 0, 500, 20);

                map.AddTBString(FrmLinkAttr.Target, "_blank", "Target", true, false, 0, 20, 20);

                map.AddTBFloat(FrmLinkAttr.X, 5, "X", true, false);
                map.AddTBFloat(FrmLinkAttr.Y, 5, "Y", false, false);

                map.AddTBInt(FrmLinkAttr.FontSize, 12, "FontSize", false, false);
                map.AddTBString(FrmLinkAttr.FontColor, "black", "FontColor", true, false, 0, 50, 20);
                map.AddTBString(FrmLinkAttr.FontName, null, "FontName", true, false, 0, 50, 20);
                map.AddTBString(FrmLinkAttr.FontStyle, "normal", "FontStyle", true, false, 0, 50, 20);

                map.AddTBInt(FrmLabAttr.IsBold, 0, "IsBold", false, false);
                map.AddTBInt(FrmLabAttr.IsItalic, 0, "IsItalic", false, false);

                map.AddTBString(FrmLabAttr.GUID, null, "GUID", true, false, 0, 128, 20);

             
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 超连接s
    /// </summary>
    public class FrmLinks : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 超连接s
        /// </summary>
        public FrmLinks()
        {
        }
        /// <summary>
        /// 超连接s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public FrmLinks(string fk_mapdata)
        {
            this.Retrieve(FrmLineAttr.FK_MapData, fk_mapdata);
        }  
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmLink();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmLink> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmLink>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmLink> Tolist()
        {
            System.Collections.Generic.List<FrmLink> list = new System.Collections.Generic.List<FrmLink>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmLink)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
