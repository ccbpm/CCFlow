
using System;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.Sys
{
	/// <summary>
	/// 通用从表
	/// </summary>
    public class GEDtlAttr : EntityOIDAttr
    {
        public const string RefPK = "RefPK";
        public const string FID = "FID";
        public const string Rec = "Rec";
        public const string RDT = "RDT";
        /// <summary>
        /// 行是否要锁定
        /// </summary>
        public const string IsRowLock = "IsRowLock";

    }
    /// <summary>
    /// 通用从表
    /// </summary>
    public class GEDtl : EntityOID
    {
        #region 构造函数
        public override string ToString()
        {
            return this.FK_MapDtl;
        }
        public override string ClassID
        {
            get
            {
                return this.FK_MapDtl;
            }
        }
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(GEDtlAttr.RDT);
            }
            set
            {
                this.SetValByKey(GEDtlAttr.RDT, value);
            }
        }
        public string Rec
        {
            get
            {
                return this.GetValStringByKey(GEDtlAttr.Rec);
            }
            set
            {
                this.SetValByKey(GEDtlAttr.Rec, value);
            }
        }
        /// <summary>
        /// 关联的PK值
        /// </summary>
        public string RefPK
        {
            get
            {
                return this.GetValStringByKey(GEDtlAttr.RefPK);
            }
            set
            {
                this.SetValByKey(GEDtlAttr.RefPK, value);
            }
        }
        public Int64 RefPKInt64
        {
            get
            {
                return this.GetValInt64ByKey(GEDtlAttr.RefPK);
            }
            set
            {
                this.SetValByKey(GEDtlAttr.RefPK, value);
            }
        }
        /// <summary>
        /// 行是否锁定
        /// </summary>
        public bool IsRowLock
        {
            get
            {
                return this.GetValBooleanByKey(GEDtlAttr.IsRowLock);
            }
            set
            {
                this.SetValByKey(GEDtlAttr.IsRowLock, value);
            }
        }
        /// <summary>
        /// 关联的PKint
        /// </summary>
        public int RefPKInt
        {
            get
            {
                return this.GetValIntByKey(GEDtlAttr.RefPK);
            }
            set
            {
                this.SetValByKey(GEDtlAttr.RefPK, value);
            }
        }
        public Int64 FID
        {
            get
            {
                return this.GetValInt64ByKey(GEDtlAttr.FID);
            }
            set
            {
                this.SetValByKey(GEDtlAttr.FID, value);
            }
        }
        /// <summary>
        /// 主键
        /// </summary>
        public string FK_MapDtl = null;
        /// <summary>
        /// 通用从表
        /// </summary>
        public GEDtl()
        {
        }
        /// <summary>
        /// 通用从表
        /// </summary>
        /// <param name="nodeid">节点ID</param>
        public GEDtl(string fk_mapdtl)
        {
            this.FK_MapDtl = fk_mapdtl;
        }
        /// <summary>
        /// 通用从表
        /// </summary>
        /// <param name="nodeid">节点ID</param>
        /// <param name="_oid">OID</param>
        public GEDtl(string fk_mapdtl, int _oid)
        {
            this.FK_MapDtl = fk_mapdtl;
            this.OID = _oid;
        }
        #endregion

        #region Map
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                if (this.FK_MapDtl == null)
                    throw new Exception("没有给" + this.FK_MapDtl + "值，您不能获取它的Map。");

                BP.Sys.MapDtl md = new BP.Sys.MapDtl(this.FK_MapDtl);
                this._enMap = md.GenerMap();
                return this._enMap;
            }
        }
        /// <summary>
        /// GEDtls
        /// </summary>
        public override Entities GetNewEntities
        {
            get
            {
                if (this.FK_MapDtl == null)
                    return new GEDtls();

                return new GEDtls(this.FK_MapDtl);
            }
        }
        public bool IsChange(GEDtl dtl)
        {
            Attrs attrs = dtl.EnMap.Attrs;
            foreach (Attr attr in attrs)
            {
                if (this.GetValByKey(attr.Key) == dtl.GetValByKey(attr.Key))
                    continue;
                else
                    return true;
            }
            return false;
        }
        protected override bool beforeUpdate()
        {
            this.AutoFull(); /*处理自动计算。*/
            return base.beforeUpdate();
        }
        /// <summary>
        /// 记录人
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            // 判断是否有变化的项目，决定是否执行储存。
            MapAttrs mattrs = new MapAttrs(this.FK_MapDtl);
            bool isChange = false;
            foreach (MapAttr mattr in mattrs)
            {
                if (isChange)
                    break;
                switch (mattr.KeyOfEn)
                {
                    case "Rec":
                    case "RDT":
                    case "RefPK":
                    case "FID":
                        break;
                    default:
                        if (mattr.IsNum)
                        {
                            string s = this.GetValStrByKey(mattr.KeyOfEn);
                            if (DataType.IsNullOrEmpty(s))
                            {
                                this.SetValByKey(mattr.KeyOfEn, mattr.DefVal);
                                s = mattr.DefVal.ToString();
                            }

                            if (decimal.Parse(s) == mattr.DefValDecimal)
                                continue;
                            isChange = true;
                            break;
                        }
                        else
                        {
                            if (this.GetValStrByKey(mattr.KeyOfEn) == mattr.DefVal)
                                continue;
                            isChange = true;
                            break;
                        }
                        break;
                }
            }
            if (isChange == false)
                return false;

            this.Rec = BP.Web.WebUser.No;
            this.RDT = DataType.CurrentDataTime;

            this.AutoFull(); /*处理自动计算。*/
            return base.beforeInsert();
        }
        #endregion
    }
    /// <summary>
    /// 通用从表s
    /// </summary>
    public class GEDtls : EntitiesOID
    {
        #region 重载基类方法
        /// <summary>
        /// 节点ID
        /// </summary>
        public string FK_MapDtl = null;
        #endregion

        #region 方法
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                if (this.FK_MapDtl == null)
                    return new GEDtl();
                return new GEDtl(this.FK_MapDtl);
            }
        }
        /// <summary>
        /// 通用从表ID
        /// </summary>
        public GEDtls()
        {
        }
        /// <summary>
        /// 通用从表ID
        /// </summary>
        /// <param name="fk_mapdtl"></param>
        public GEDtls(string fk_mapdtl)
        {
            this.FK_MapDtl = fk_mapdtl;
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<GEDtl> ToJavaList()
        {
            return (System.Collections.Generic.IList<GEDtl>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<GEDtl> Tolist()
        {
            System.Collections.Generic.List<GEDtl> list = new System.Collections.Generic.List<GEDtl>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((GEDtl)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
