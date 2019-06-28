using System;
using System.Collections;
using BP.DA;
using BP.En;
namespace BP.Sys
{
    /// <summary>
    /// 图片应用类型
    /// </summary>
    public enum ImgAppType
    {
        /// <summary>
        /// 图片
        /// </summary>
        Img,
        /// <summary>
        /// 图片公章
        /// </summary>
        Seal,
        /// <summary>
        /// 北京安证通公章CA
        /// </summary>
        SealESA,
        /// <summary>
        /// 二维码
        /// </summary>
        QRCode
    }
    /// <summary>
    /// 图片
    /// </summary>
    public class FrmImgAttr : EntityMyPKAttr
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
        /// W
        /// </summary>
        public const string W = "W";
        /// <summary>
        /// H
        /// </summary>
        public const string H = "H";
        /// <summary>
        /// URL
        /// </summary>
        public const string ImgURL = "ImgURL";
        /// <summary>
        /// 文件路径
        /// </summary>
        public const string ImgPath = "ImgPath";
        /// <summary>
        /// LinkURL
        /// </summary>
        public const string LinkURL = "LinkURL";
        /// <summary>
        /// LinkTarget
        /// </summary>
        public const string LinkTarget = "LinkTarget";
        /// <summary>
        /// GUID
        /// </summary>
        public const string GUID = "GUID";
        /// <summary>
        /// 应用类型
        /// </summary>
        public const string ImgAppType = "ImgAppType";
        /// <summary>
        /// 参数
        /// </summary>
        public const string Tag0 = "Tag0";
        /// <summary>
        /// 数据来源类型 0=本地 , 1=外部.
        /// </summary>
        public const string ImgSrcType = "ImgSrcType";
        /// <summary>
        /// 是否可以编辑
        /// </summary>
        public const string IsEdit = "IsEdit";
        /// <summary>
        /// 中文名称
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// 英文名称
        /// </summary>
        public const string EnPK = "EnPK";
    }
    /// <summary>
    /// 图片
    /// </summary>
    public class FrmImg : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 中文名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStringByKey(FrmImgAttr.Name);
            }
            set
            {
                this.SetValByKey(FrmImgAttr.Name, value);
            }
        }
        /// <summary>
        /// 对应字段名称
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
        /// 英文名称
        /// </summary>
        public string EnPK
        {
            get
            {
                return this.GetValStringByKey(FrmImgAttr.EnPK);
            }
            set
            {
                this.SetValByKey(FrmImgAttr.EnPK, value);
            }
        }
        /// <summary>
        /// 是否可以编辑
        /// </summary>
        public int IsEdit
        {
            get
            {
                return this.GetValIntByKey(FrmImgAttr.IsEdit);
            }
            set
            {
                this.SetValByKey(FrmImgAttr.IsEdit, (int)value);
            }
        }
        /// <summary>
        /// 应用类型
        /// </summary>
        public ImgAppType HisImgAppType
        {
            get
            {
                return (ImgAppType)this.GetValIntByKey(FrmImgAttr.ImgAppType);
            }
            set
            {
                this.SetValByKey(FrmImgAttr.ImgAppType, (int)value);
            }
        }
        /// <summary>
        /// 数据来源
        /// </summary>
        public int ImgSrcType
        {
            get
            {
                return this.GetValIntByKey(FrmImgAttr.ImgSrcType);
            }
            set
            {
                this.SetValByKey(FrmImgAttr.ImgSrcType, value);
            }
        }
        
        public string Tag0
        {
            get
            {
                return this.GetValStringByKey(FrmImgAttr.Tag0);
            }
            set
            {
                this.SetValByKey(FrmImgAttr.Tag0, value);
            }
        }
       
        public string LinkTarget
        {
            get
            {
                return this.GetValStringByKey(FrmImgAttr.LinkTarget);
            }
            set
            {
                this.SetValByKey(FrmImgAttr.LinkTarget, value);
            }
        }
        /// <summary>
        /// URL
        /// </summary>
        public string LinkURL
        {
            get
            {
                return this.GetValStringByKey(FrmImgAttr.LinkURL);
            }
            set
            {
                this.SetValByKey(FrmImgAttr.LinkURL, value);
            }
        }
        public string ImgPath
        {
            get
            {
                string src = this.GetValStringByKey(FrmImgAttr.ImgPath);
                if (DataType.IsNullOrEmpty(src))
                {
                    string appPath = BP.Sys.Glo.Request.ApplicationPath;
                    src = appPath + "DataUser/ICON/" + BP.Sys.SystemConfig.CustomerNo + "/LogBiger.png";
                }
                return src;
            }
            set
            {
                this.SetValByKey(FrmImgAttr.ImgPath, value);
            }
        }
        public string ImgURL
        {
            get
            {
                string src = this.GetValStringByKey(FrmImgAttr.ImgURL);
                if (DataType.IsNullOrEmpty(src) || src.Contains("component/Img"))
                {
                    string appPath = BP.Sys.Glo.Request.ApplicationPath;
                    src = appPath + "DataUser/ICON/" + BP.Sys.SystemConfig.CustomerNo + "/LogBiger.png";
                }
                return src;
            }
            set
            {
                this.SetValByKey(FrmImgAttr.ImgURL, value);
            }
        }
        /// <summary>
        /// Y
        /// </summary>
        public float Y
        {
            get
            {
                return this.GetValFloatByKey(FrmImgAttr.Y);
            }
            set
            {
                this.SetValByKey(FrmImgAttr.Y, value);
            }
        }
        /// <summary>
        /// X
        /// </summary>
        public float X
        {
            get
            {
                return this.GetValFloatByKey(FrmImgAttr.X);
            }
            set
            {
                this.SetValByKey(FrmImgAttr.X, value);
            }
        }
        /// <summary>
        /// H
        /// </summary>
        public float H
        {
            get
            {
                return this.GetValFloatByKey(FrmImgAttr.H);
            }
            set
            {
                this.SetValByKey(FrmImgAttr.H, value);
            }
        }
        /// <summary>
        /// W
        /// </summary>
        public float W
        {
            get
            {
                return this.GetValFloatByKey(FrmImgAttr.W);
            }
            set
            {
                this.SetValByKey(FrmImgAttr.W, value);
            }
        }
        /// <summary>
        /// FK_MapData
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.GetValStrByKey(FrmImgAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(FrmImgAttr.FK_MapData, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 图片
        /// </summary>
        public FrmImg()
        {
        }
        /// <summary>
        /// 图片
        /// </summary>
        /// <param name="mypk"></param>
        public FrmImg(string mypk)
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
                Map map = new Map("Sys_FrmImg", "图片");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetEnType(EnType.Sys);
                map.AddMyPK();

                map.AddTBString(FrmImgAttr.FK_MapData, null, "FK_MapData", true, false, 1, 100, 20);
                map.AddTBString(MapAttrAttr.KeyOfEn, null, "对应字段", true, false, 1, 100, 20);

                map.AddTBInt(FrmImgAttr.ImgAppType, 0, "应用类型", false, false);
                
                map.AddTBFloat(FrmImgAttr.X, 5, "X", true, false);
                map.AddTBFloat(FrmImgAttr.Y, 5, "Y", false, false);

                map.AddTBFloat(FrmImgAttr.H, 200, "H", true, false);
                map.AddTBFloat(FrmImgAttr.W, 160, "W", false, false);

                map.AddTBString(FrmImgAttr.ImgURL, null, "ImgURL", true, false, 0, 200, 20);
                map.AddTBString(FrmImgAttr.ImgPath, null, "ImgPath", true, false, 0, 200, 20);
                
                map.AddTBString(FrmImgAttr.LinkURL, null, "LinkURL", true, false, 0, 200, 20);
                map.AddTBString(FrmImgAttr.LinkTarget, "_blank", "LinkTarget", true, false, 0, 200, 20);

                map.AddTBString(FrmImgAttr.GUID, null, "GUID", true, false, 0, 128, 20);

                //如果是 seal 就是岗位集合。
                map.AddTBString(FrmImgAttr.Tag0, null, "参数", true, false, 0, 500, 20);
                map.AddTBInt(FrmImgAttr.ImgSrcType, 0, "图片来源0=本地,1=URL", true, false);
                map.AddTBInt(FrmImgAttr.IsEdit, 0, "是否可以编辑", true, false);
                map.AddTBString(FrmImgAttr.Name, null, "中文名称", true, false, 0, 500, 20);
                map.AddTBString(FrmImgAttr.EnPK, null, "英文名称", true, false, 0, 500, 20);
                map.AddTBInt(MapAttrAttr.ColSpan, 0, "单元格数量", false, true);
                map.AddTBInt(MapAttrAttr.TextColSpan, 1, "文本单元格数量", false, true);
                map.AddTBInt(MapAttrAttr.RowSpan, 1, "行数", false, true);

                //显示的分组.
                map.AddDDLSQL(MapAttrAttr.GroupID, "0", "显示的分组",BP.Sys.FrmUI.MapAttrString.SQLOfGroupAttr, true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            if(DataType.IsNullOrEmpty(this.KeyOfEn) == false)
                this.MyPK = this.FK_MapData + "_" + this.KeyOfEn ;
            return base.beforeInsert();
        }

        /// <summary>
        /// 是否存在相同的数据?
        /// </summary>
        /// <returns></returns>
        public bool IsExitGenerPK()
        {
            string sql = "SELECT COUNT(*) FROM Sys_FrmImg WHERE FK_MapData='" + this.FK_MapData + "' AND X=" + this.X + " AND Y=" + this.Y;
            if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                return false;
            return true;
        }
    }
    /// <summary>
    /// 图片s
    /// </summary>
    public class FrmImgs : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 图片s
        /// </summary>
        public FrmImgs()
        {
        }
        /// <summary>
        /// 图片s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public FrmImgs(string fk_mapdata)
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
                return new FrmImg();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmImg> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmImg>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmImg> Tolist()
        {
            System.Collections.Generic.List<FrmImg> list = new System.Collections.Generic.List<FrmImg>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmImg)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
