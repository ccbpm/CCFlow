
using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Sys
{
    public class GEEntityWordFrmAttr
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public const string FilePath="FilePath";
        /// <summary>
        /// 记录时间
        /// </summary>
        public const string RDT="RDT";
        /// <summary>
        /// 最后修改人
        /// </summary>
        public const string LastEditer="LastEditer";

        public const string OID="OID";

    }
    /// <summary>
    /// 通用实体
    /// </summary>
    public class GEEntityWordFrm  : Entity
    {
#region 属性。
        public int OID
        {
            get
            {
               return this.GetValIntByKey(GEEntityWordFrmAttr.OID);
            }
            set
            {
                this.SetValByKey(GEEntityWordFrmAttr.OID,value);
            }
        }
        /// <summary>
        /// 最后修改人
        /// </summary>
          public string LastEditer
        {
            get
            {
               return this.GetValStringByKey(GEEntityWordFrmAttr.LastEditer);
            }
            set
            {
                this.SetValByKey(GEEntityWordFrmAttr.LastEditer,value);
            }
        }
        /// <summary>
        /// 记录时间
        /// </summary>
           public string RDT
        {
            get
            {
               return this.GetValStringByKey(GEEntityWordFrmAttr.RDT);
            }
            set
            {
                this.SetValByKey(GEEntityWordFrmAttr.RDT,value);
            }
        }

        /// <summary>
        /// 文件路径
        /// </summary>
           public string FilePath
           {
               get
               {
                   return this.GetValStringByKey(GEEntityWordFrmAttr.FilePath);
               }
               set
               {
                   this.SetValByKey(GEEntityWordFrmAttr.FilePath, value);
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
        public GEEntityWordFrm()
        {
        }
        /// <summary>
        /// 通用实体
        /// </summary>
        /// <param name="nodeid">节点ID</param>
        public GEEntityWordFrm(string fk_mapdata)
        {
            this.FK_MapData = fk_mapdata;
        }
        /// <summary>
        /// 通用实体
        /// </summary>
        /// <param name="nodeid">节点ID</param>
        /// <param name="_oid">OID</param>
        public GEEntityWordFrm(string fk_mapdata, object pk)
        {
            this.FK_MapData = fk_mapdata;
            this.PKVal = pk;
            this.Retrieve();
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
    }
    /// <summary>
    /// 通用实体s
    /// </summary>
    public class GEEntityWordFrms : EntitiesOID
    {
        #region 重载基类方法
        public override string ToString()
        {
            //if (this.FK_MapData == null)
            //    throw new Exception("@没有能 FK_MapData 给值。");
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
        public GEEntityWordFrms()
        {
        }
        /// <summary>
        /// 通用实体ID
        /// </summary>
        /// <param name="fk_mapdtl"></param>
        public GEEntityWordFrms(string fk_mapdata)
        {
            this.FK_MapData = fk_mapdata;
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<GEEntityWordFrm> ToJavaList()
        {
            return (System.Collections.Generic.IList<GEEntityWordFrm>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<GEEntityWordFrm> Tolist()
        {
            System.Collections.Generic.List<GEEntityWordFrm> list = new System.Collections.Generic.List<GEEntityWordFrm>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((GEEntityWordFrm)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
