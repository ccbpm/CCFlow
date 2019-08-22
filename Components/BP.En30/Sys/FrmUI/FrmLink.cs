using System;
using System.Collections;
using BP.DA;
using BP.En;
namespace BP.Sys.FrmUI
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
                return this.GetValStringByKey(FrmLinkAttr.URL).Replace("#", "@");
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
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.Readonly();
                if (BP.Web.WebUser.No == "admin")
                    uac.IsUpdate = true;
                return uac;
            }
        }
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
                map.Java_SetDepositaryOfMap(Depositary.Application);
                map.Java_SetEnType(EnType.Sys);
                map.IndexField = MapAttrAttr.FK_MapData;


                //连接目标.
                map.AddMyPK();
                map.AddTBString(FrmLinkAttr.Target, "_blank", "连接目标(_blank,_parent,_self)", true, false, 0, 20, 20);
                map.AddTBString(FrmLinkAttr.Text, "New Link", "标签", true, false, 0, 500, 20, true);
                map.AddTBString(FrmLinkAttr.URL, null, "URL", true, false, 0, 500, 20, true);
                map.AddTBString(FrmLinkAttr.FK_MapData, null, "表单ID", false, false, 1, 100, 20);

                //显示的分组.
                map.AddDDLSQL(MapAttrAttr.GroupID, 0, "显示的分组",
                    "SELECT OID as No, Lab as Name FROM Sys_GroupField WHERE FrmID='@FK_MapData' AND (CtrlType IS NULL OR CtrlType='')  ", true);


                this._enMap = map;
                return this._enMap;
            }
        }

        protected override void afterInsertUpdateAction()
        {
            BP.Sys.FrmLink frmLink = new BP.Sys.FrmLink();
            frmLink.MyPK = this.MyPK;
            frmLink.RetrieveFromDBSources();
            frmLink.Update();

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);

            base.afterInsertUpdateAction();
        }

        /// <summary>
        /// 删除后清缓存
        /// </summary>
        protected override void afterDelete()
        {
            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);
            base.afterDelete();
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
