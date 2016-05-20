using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BP.En;

namespace BP.Sys
{
    /// <summary>
    /// 属性
    /// </summary>
    public class GEEntityExcelFrmAttr
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public const string FilePath = "FilePath";
        /// <summary>
        /// 记录时间
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 最后修改人
        /// </summary>
        public const string LastEditer = "LastEditer";
    }
    /// <summary>
    /// excel表单实体
    /// </summary>
    public class GEEntityExcelFrm : EntityOID
    {
        #region 属性。
        /// <summary>
        /// 最后修改人
        /// </summary>
        public string LastEditer
        {
            get
            {
                return this.GetValStringByKey(GEEntityExcelFrmAttr.LastEditer);
            }
            set
            {
                this.SetValByKey(GEEntityExcelFrmAttr.LastEditer, value);
            }
        }
        /// <summary>
        /// 记录时间
        /// </summary>
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(GEEntityExcelFrmAttr.RDT);
            }
            set
            {
                this.SetValByKey(GEEntityExcelFrmAttr.RDT, value);
            }
        }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath
        {
            get
            {
                return this.GetValStringByKey(GEEntityExcelFrmAttr.FilePath);
            }
            set
            {
                this.SetValByKey(GEEntityExcelFrmAttr.FilePath, value);
            }
        }
        #endregion 属性。

        #region 构造函数
        public override string PK
        {
            get
            {
                return "OID";
            }
        }
        public override string PKField
        {
            get
            {
                return "OID";
            }
        }
        public override string ToString()
        {
            return this.FK_MapData;
        }
        public override string ClassID
        {
            get
            {
                return this.FK_MapData;
            }
        }
        /// <summary>
        /// 主键
        /// </summary>
        public string FK_MapData = null;
        /// <summary>
        /// 通用实体
        /// </summary>
        public GEEntityExcelFrm()
        {
        }
        /// <summary>
        /// 通用实体
        /// </summary>
        /// <param name="nodeid">节点ID</param>
        public GEEntityExcelFrm(string fk_mapdata)
        {
            this.FK_MapData = fk_mapdata;
        }
        /// <summary>
        /// 通用实体
        /// </summary>
        /// <param name="nodeid">节点ID</param>
        /// <param name="_oid">OID</param>
        public GEEntityExcelFrm(string fk_mapdata, int oid)
        {
            this.FK_MapData = fk_mapdata;
            this.OID = oid;
            int i =this.RetrieveFromDBSources();
        }
        #endregion

        #region Map
        /// <summary>
        /// 重写基类方法=
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                if (this.FK_MapData == null)
                    throw new Exception("没有给" + this.FK_MapData + "值，您不能获取它的Map。");

                this._enMap = BP.Sys.MapData.GenerHisMap(this.FK_MapData);
                return this._enMap;
            }
        }
        /// <summary>
        /// GEEntitys
        /// </summary>
        public override Entities GetNewEntities
        {
            get
            {
                if (this.FK_MapData == null)
                    return new GEEntityWordFrms();
                return new GEEntityWordFrms(this.FK_MapData);
            }
        }
        #endregion

        #region 其他属性.
        private ArrayList _Dtls = null;
        public ArrayList Dtls
        {
            get
            {
                if (_Dtls == null)
                    _Dtls = new ArrayList();
                return _Dtls;
            }
        }
        #endregion 其他属性.
    }
    /// <summary>
    /// excel表单实体s
    /// </summary>
    public class GEEntityExcelFrms : EntitiesOID
    {
        #region 重载基类方法
        public override string ToString()
        {
            return this.FK_MapData;
        }
        /// <summary>
        /// 主键
        /// </summary>
        public string FK_MapData = null;
        #endregion

        #region 方法
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                //if (this.FK_MapData == null)
                //    throw new Exception("@没有能 FK_MapData 给值。");

                if (this.FK_MapData == null)
                    return new GEEntity();
                return new GEEntity(this.FK_MapData);
            }
        }
        /// <summary>
        /// 通用实体ID
        /// </summary>
        public GEEntityExcelFrms()
        {
        }
        /// <summary>
        /// 通用实体ID
        /// </summary>
        /// <param name="fk_mapdtl"></param>
        public GEEntityExcelFrms(string fk_mapdata)
        {
            this.FK_MapData = fk_mapdata;
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<GEEntityExcelFrm> ToJavaList()
        {
            return (System.Collections.Generic.IList<GEEntityExcelFrm>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<GEEntityExcelFrm> Tolist()
        {
            System.Collections.Generic.List<GEEntityExcelFrm> list = new System.Collections.Generic.List<GEEntityExcelFrm>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((GEEntityExcelFrm)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
