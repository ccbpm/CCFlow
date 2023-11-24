
using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Sys
{
    /// <summary>
    /// 通用OID实体
    /// </summary>
    public class GEEntityMyPK : Entity
    {
        #region 构造函数
        /// <summary>
        /// 设置或者获取主键值.
        /// </summary>
        public string MyPK
        {
            get
            {
                return this.GetValStrByKey("MyPK");
            }
            set
            {
                this.SetValByKey("MyPK", value);
            }
        }
        public void setMyPK(string val)
        {
            this.SetValByKey("MyPK", val);
        }
        /// <summary>
        /// 主键值
        /// </summary>
        public override string PK
        {
            get
            {
                return "MyPK";
            }
        }
        /// <summary>
        ///  主键字段
        /// </summary>
        public override string PKField
        {
            get
            {
                return "MyPK";
            }
        }
        /// <summary>
        /// 转化为类.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.FrmID;
        }
        public override string ClassID
        {
            get
            {
                return this.FrmID;
            }
        }
        /// <summary>
        /// 主键
        /// </summary>
        public string FrmID = null;
        /// <summary>
        /// 通用OID实体
        /// </summary>
        public GEEntityMyPK()
        {
        }
        /// <summary>
        /// 通用OID实体
        /// </summary>
        /// <param name="frmID">节点ID</param>
        public GEEntityMyPK(string frmID)
        {
            this.FrmID = frmID;
        }
        public GEEntityMyPK(string frmID, string pk)
        {
            this.FrmID = frmID;
            this.PKVal = pk;
            this.Retrieve();
        }
        #endregion

        #region 构造映射.
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                if (this.FrmID == null)
                    throw new Exception("没有给" + this.FrmID + "值，您不能获取它的Map。");

                this._enMap = BP.Sys.MapData.GenerHisMap(this.FrmID);
                return this._enMap;
            }
        }
        /// <summary>
        /// GEEntityMyPKs
        /// </summary>
        public override Entities GetNewEntities
        {
            get
            {
                if (this.FrmID == null)
                    return new GEEntityMyPKs();
                return new GEEntityMyPKs(this.FrmID);
            }
        }
        #endregion

    }
    /// <summary>
    /// 通用OID实体s
    /// </summary>
    public class GEEntityMyPKs : Entities
    {
        #region 重载基类方法
        public override string ToString()
        {
            //if (this.FrmID == null)
            //    throw new Exception("@没有能 FK_MapData 给值。");
            return this.FrmID;
        }
        /// <summary>
        /// 主键
        /// </summary>
        public string FrmID = null;
        #endregion

        #region 方法
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                if (this.FrmID == null)
                    return new GEEntityMyPK();
                return new GEEntityMyPK(this.FrmID);
            }
        }
        /// <summary>
        /// 通用OID实体ID
        /// </summary>
        public GEEntityMyPKs()
        {
        }
      
        public GEEntityMyPKs(string frmID)
        {
            this.FrmID = frmID;
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<GEEntityMyPK> ToJavaList()
        {
            return (System.Collections.Generic.IList<GEEntityMyPK>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<GEEntityMyPK> Tolist()
        {
            System.Collections.Generic.List<GEEntityMyPK> list = new System.Collections.Generic.List<GEEntityMyPK>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((GEEntityMyPK)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
