
using System;
using System.Data;
using System.Collections;
using BP.DA;

namespace BP.En
{
    /// <summary>
    /// 通用实体
    /// </summary>
    public class TSEntityOID : EntityOID
    {
        #region 构造函数
        /// <summary>
        /// 主键
        /// </summary>
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
        /// <summary>
        /// 转化为类.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this._TSclassID;
        }
        /// <summary>
        /// 主键ID
        /// </summary>
        public override string ClassID
        {
            get
            {
                return this._TSclassID;
            }
        }
        public TSEntityOID()
        {
            //构造.
            BP.Port.StationType en = new Port.StationType();
            this._enMap = en.EnMap;
        }
        /// <summary>
        /// 主键
        /// </summary>
        public string _TSclassID = null;

        /// <summary>
        /// 通用编号名称实体
        /// </summary>
        /// <param name="_TSclassID">类ID</param>
        public TSEntityOID(string _TSclassID)
        {
            this._TSclassID = _TSclassID;
            this._enMap = BP.EnTS.Glo.GenerMap(_TSclassID);
            // this._enMap = map as Map;
            // this._enMap = BP.EnTS.Glo.GenerMap(_TSclassID);
        }
        /// <summary>
        /// 通用编号名称实体
        /// </summary>
        /// <param name="frmID">表单ID</param>
        /// <param name="pk"></param>
        public TSEntityOID(string classID, int pk)
        {
            this._TSclassID = classID;
            this._enMap = BP.EnTS.Glo.GenerMap(_TSclassID);
            this.OID = pk;
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

                if (this._TSclassID == null)
                    throw new Exception("没有给 _TSclassID 值，您不能获取它的Map。");

                this._enMap = BP.EnTS.Glo.GenerMap(this._TSclassID);
                return this._enMap;
            }
        }
        /// <summary>
        /// TSEntityOIDs
        /// </summary>
        public override Entities GetNewEntities
        {
            get
            {
                if (this._TSclassID == null)
                    throw new Exception("没有给 _TSclassID 值，您不能获取它的 Entities。");
                return new TSEntitiesOID(this._TSclassID);
            }
        }
        #endregion
    }
    /// <summary>
    /// 实体类
    /// </summary>
    public class TSEntitiesOID : EntitiesOID
    {
        #region 重载基类方法
        public override string ToString()
        {
            return this._TSclassID;
        }
        /// <summary>
        /// 类名
        /// </summary>
        public string _TSclassID = null;
        #endregion

        #region 方法
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                if (this._TSclassID == null)
                    return new BP.Port.StationType();
                //throw new Exception("没有给 _TSclassID 值，您不能获取它的 Entities。");
                return new TSEntityOID(this._TSclassID);
            }
        }
        public TSEntitiesOID()
        {

        }
        public TSEntitiesOID(string tsClassID)
        {
            this._TSclassID = tsClassID;
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<TSEntityOID> ToJavaList()
        {
            return (System.Collections.Generic.IList<TSEntityOID>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<TSEntityOID> Tolist()
        {
            System.Collections.Generic.List<TSEntityOID> list = new System.Collections.Generic.List<TSEntityOID>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((TSEntityOID)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
