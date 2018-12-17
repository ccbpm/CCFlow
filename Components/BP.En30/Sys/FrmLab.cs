using System;
using System.Collections;
using BP.DA;
using BP.En;
namespace BP.Sys
{
    /// <summary>
    /// 标签
    /// </summary>
    public class FrmLabAttr : EntityMyPKAttr
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
        /// X
        /// </summary>
        public const string X = "X";
        /// <summary>
        /// Y
        /// </summary>
        public const string Y = "Y";
        /// <summary>
        /// 宽度
        /// </summary>
        public const string FontSize = "FontSize";
        /// <summary>
        /// 颜色
        /// </summary>
        public const string FontColor = "FontColor";
        /// <summary>
        /// 风格
        /// </summary>
        public const string FontName = "FontName";
        /// <summary>
        /// 字体风格
        /// </summary>
        public const string FontStyle = "FontStyle";
        /// <summary>
        /// 字体
        /// </summary>
        public const string FontWeight = "FontWeight";
        /// <summary>
        /// 是否粗体
        /// </summary>
        public const string IsBold = "IsBold";
        /// <summary>
        /// 斜体
        /// </summary>
        public const string IsItalic = "IsItalic";
        /// <summary>
        /// GUID
        /// </summary>
        public const string GUID = "GUID";
    }
    /// <summary>
    /// 标签
    /// </summary>
    public class FrmLab : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// FontStyle
        /// </summary>
        public string FontStyle
        {
            get
            {
                return this.GetValStringByKey(FrmLabAttr.FontStyle);
            }
            set
            {
                this.SetValByKey(FrmLabAttr.FontStyle, value);
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
                return this.GetValStringByKey(FrmLabAttr.FontColor);
            }
            set
            {
                switch (value)
                {
                    case "#FF000000":
                        this.SetValByKey(FrmLabAttr.FontColor, "Red");
                        return;
                    default:
                        break;
                }
                this.SetValByKey(FrmLabAttr.FontColor, value);
            }
        }
        public string FontWeight
        {
            get
            {
                return this.GetValStringByKey(FrmLabAttr.FontWeight);
            }
            set
            {
                this.SetValByKey(FrmLabAttr.FontWeight, value);
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
        /// <summary>
        /// FontName
        /// </summary>
        public string FontName
        {
            get
            {
                return this.GetValStringByKey(FrmLabAttr.FontName);
            }
            set
            {
                this.SetValByKey(FrmLabAttr.FontName, value);
            }
        }
        /// <summary>
        /// Y
        /// </summary>
        public float Y
        {
            get
            {
                return this.GetValFloatByKey(FrmLabAttr.Y);
            }
            set
            {
                this.SetValByKey(FrmLabAttr.Y, value);
            }
        }
        /// <summary>
        /// X
        /// </summary>
        public float X
        {
            get
            {
                return this.GetValFloatByKey(FrmLabAttr.X);
            }
            set
            {
                this.SetValByKey(FrmLabAttr.X, value);
            }
        }
        /// <summary>
        /// FontSize
        /// </summary>
        public int FontSize
        {
            get
            {
                return this.GetValIntByKey(FrmLabAttr.FontSize);
            }
            set
            {
                this.SetValByKey(FrmLabAttr.FontSize, value);
            }
        }
        /// <summary>
        /// FK_MapData
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.GetValStrByKey(FrmLabAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(FrmLabAttr.FK_MapData, value);
            }
        }
        /// <summary>
        /// Text
        /// </summary>
        public string Text
        {
            get
            {
                return this.GetValStrByKey(FrmLabAttr.Text);
            }
            set
            {
                this.SetValByKey(FrmLabAttr.Text, value);
            }
        }
        public string TextHtml
        {
            get
            {
                if (this.IsBold)
                    return "<b>" + this.GetValStrByKey(FrmLabAttr.Text).Replace("@","<br>") + "</b>";
                else
                    return this.GetValStrByKey(FrmLabAttr.Text).Replace("@", "<br>");
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 标签
        /// </summary>
        public FrmLab()
        {
        }
        /// <summary>
        /// 标签
        /// </summary>
        /// <param name="mypk"></param>
        public FrmLab(string mypk)
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
                Map map = new Map("Sys_FrmLab", "标签");

                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetEnType(EnType.Sys);

                map.AddMyPK();
                map.AddTBString(FrmLabAttr.FK_MapData, null, "FK_MapData", true, false, 1, 100, 20);
                map.AddTBString(FrmLabAttr.Text, "New Label", "Label", true, false, 0, 3900, 20);

                map.AddTBFloat(FrmLabAttr.X, 5, "X", true, false);
                map.AddTBFloat(FrmLabAttr.Y, 5, "Y", false, false);

                map.AddTBInt(FrmLabAttr.FontSize, 12, "字体大小", false, false);
                map.AddTBString(FrmLabAttr.FontColor, "black", "颜色", true, false, 0, 50, 20);
                map.AddTBString(FrmLabAttr.FontName, null, "字体名称", true, false, 0, 50, 20);
                map.AddTBString(FrmLabAttr.FontStyle, "normal", "字体风格", true, false, 0, 200, 20);
                map.AddTBString(FrmLabAttr.FontWeight, "normal", "字体宽度", true, false, 0, 50, 20);

                map.AddTBInt(FrmLabAttr.IsBold, 0, "是否粗体", false, false);
                map.AddTBInt(FrmLabAttr.IsItalic, 0, "是否斜体", false, false);
                map.AddTBString(FrmLabAttr.GUID, null, "GUID", true, false, 0, 128, 20);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion


        /// <summary>
        /// 是否存在相同的数据?
        /// </summary>
        /// <returns></returns>
        public bool IsExitGenerPK()
        {
            string sql = "SELECT COUNT(*) FROM " + this.EnMap.PhysicsTable + " WHERE FK_MapData='" + this.FK_MapData + "' AND X=" + this.X + " AND Y=" + this.Y + "  and Text='" + this.Text+"'";
            if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                return false;
            return true;
        }

    }
    /// <summary>
    /// 标签s
    /// </summary>
    public class FrmLabs : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 标签s
        /// </summary>
        public FrmLabs()
        {
        }
        /// <summary>
        /// 标签s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public FrmLabs(string fk_mapdata)
        {
            if (SystemConfig.IsDebug)
                this.Retrieve(FrmLineAttr.FK_MapData, fk_mapdata);
            else
                this.RetrieveFromCash(FrmLineAttr.FK_MapData, (object)fk_mapdata);
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmLab();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmLab> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmLab>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmLab> Tolist()
        {
            System.Collections.Generic.List<FrmLab> list = new System.Collections.Generic.List<FrmLab>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmLab)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
