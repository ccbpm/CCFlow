using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo
{
	/// <summary>
	/// 章节属性
	/// </summary>
    public class ZhiDuDtlAttr : EntityNoNameAttr
    {
        #region 基本属性
        public const string IsDir = "IsDir";
        public const string Idx = "Idx";
        public const string FK_Main = "FK_Main";
        /// <summary>
        /// DocHtml
        /// </summary>
        public const string DocHtml = "DocHtml";
        /// <summary>
        /// DocText
        /// </summary>
        public const string DocText = "DocText";
        /// <summary>
        /// 文档级别
        /// </summary>
        public const string DocLevel = "DocLevel";
        /// <summary>
        /// 父项
        /// </summary>
        public const string ParagraphIndex = "ParagraphIndex";
        /// <summary>
        /// 父节点
        /// </summary>
        public const string ParentParagraphIndex = "ParentParagraphIndex";
        #endregion
    }
	/// <summary>
	/// 章节
	/// </summary>
    public class ZhiDuDtl : EntityNoName
    {
        #region 属性
        /// <summary>
        /// 序号
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(ZhiDuDtlAttr.Idx);
            }
            set
            {
                this.SetValByKey(ZhiDuDtlAttr.Idx, value);
            }
        }
        /// <summary>
        /// 文档级别
        /// </summary>
        public int DocLevel
        {
            get
            {
                return this.GetValIntByKey(ZhiDuDtlAttr.DocLevel);
            }
            set
            {
                this.SetValByKey(ZhiDuDtlAttr.DocLevel, value);
            }
        }

        /// <summary>
        /// 子级
        /// </summary>
        public int ParagraphIndex
        {
            get
            {
                return this.GetValIntByKey(ZhiDuDtlAttr.ParagraphIndex);
            }
            set
            {
                this.SetValByKey(ZhiDuDtlAttr.ParagraphIndex, value);
            }
        }
        /// <summary>
        /// 文档父级
        /// </summary>
        public int ParentParagraphIndex
        {
            get
            {
                return this.GetValIntByKey(ZhiDuDtlAttr.ParentParagraphIndex);
            }
            set
            {
                this.SetValByKey(ZhiDuDtlAttr.ParentParagraphIndex, value);
            }
        }
        /// <summary>
        /// 制度外键
        /// </summary>
        public string FK_Main
        {
            get
            {
                return this.GetValStringByKey(ZhiDuDtlAttr.FK_Main);
            }
            set
            {
                this.SetValByKey(ZhiDuDtlAttr.FK_Main, value);
            }
        }
        /// <summary>
        /// word存放目录
        /// </summary>
        public string DocText
        {
            get
            {
                return this.GetValStringByKey(ZhiDuDtlAttr.DocText);
            }
            set
            {
                this.SetValByKey(ZhiDuDtlAttr.DocText, value);
            }
        }
        /// <summary>
        /// html存放目录
        /// </summary>
        public string DocHtml
        {
            get
            {
                return this.GetValStringByKey(ZhiDuDtlAttr.DocHtml);
            }
            set
            {
                this.SetValByKey(ZhiDuDtlAttr.DocHtml, value);
            }
        }
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (Web.WebUser.No != "admin")
                {
                    uac.IsView = false;
                    return uac;
                }
                uac.IsDelete = false;
                uac.IsInsert = false;
                uac.IsUpdate = true;
                return uac;
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// Dtl
        /// </summary>
        public ZhiDuDtl()
        {
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Demo_ZhiDuDtl", "章节");
                map.Java_SetEnType(EnType.Admin);

                map.AddTBStringPK(ZhiDuDtlAttr.No, null, "编号", true, true, 5, 5, 5);
                map.AddTBString(ZhiDuDtlAttr.Name, null, "名称", true, true, 0, 200, 4);
                map.AddTBString(ZhiDuDtlAttr.DocLevel, null, "文档级别", true, true, 20, 20, 20);
                map.AddTBString(ZhiDuDtlAttr.ParagraphIndex, null, "文档级", true, true, 20, 20, 20);
                map.AddTBString(ZhiDuDtlAttr.ParentParagraphIndex, null, "文档父级", true, true, 20, 20, 20);
                map.AddTBInt(ZhiDuDtlAttr.IsDir, 0, "是否是章节", true, true);
                map.AddTBInt(ZhiDuDtlAttr.Idx, 0, "序号", true, true);

                map.AddTBString(ZhiDuDtlAttr.FK_Main, null, "制度", true, true, 0, 5, 5);

                map.AddTBString(ZhiDuDtlAttr.DocText, null, "DocText", true, true, 0, 4000, 4);
                map.AddTBString(ZhiDuDtlAttr.DocHtml, null, "DocHtml", true, true, 0, 4000, 4);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
	/// 章节s
	/// </summary>
    public class ZhiDuDtls : EntitiesNoName
	{
		#region 方法
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new ZhiDuDtl();
			}
		}
		/// <summary>
        /// 章节
		/// </summary>
        public ZhiDuDtls() { } 		 
		#endregion
	}
}
