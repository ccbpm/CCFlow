
using System;
using System.Data;
using System.Collections;
using BP.DA;

namespace BP.En
{
    /// <summary>
    /// 通用NodeID实体
    /// </summary>
    public class TSEntityNodeID : Entity
    {
        #region 构造函数
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey("NodeID");
            }
            set
            {
                this.SetValByKey("NodeID", value);
            }
        }
        /// <summary>
        /// 主键
        /// </summary>
        public override string PK
        {
            get
            {
                return "NodeID";
            }
        }
        public override string PKField
        {
            get
            {
                return "NodeID";
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
        /// <summary>
        /// 通用NodeID实体
        /// </summary>
        public TSEntityNodeID()
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
        /// 通用NodeID实体
        /// </summary>
        /// <param name="_TSclassID">类ID</param>
        public TSEntityNodeID(string _TSclassID)
        {
            this._TSclassID = _TSclassID;
            this._enMap = BP.EnTS.Glo.GenerMap(_TSclassID);
        }
        /// <summary>
        /// 通用NodeID实体
        /// </summary>
        /// <param name="frmID">表单ID</param>
        /// <param name="pk"></param>
        public TSEntityNodeID(string classID, int pk)
        {
            this._TSclassID = classID;
            this._enMap = BP.EnTS.Glo.GenerMap(_TSclassID);
            this.NodeID = pk;
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
        /// TSEntityNodeIDs
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
    /// 通用NodeID实体s
    /// </summary>
    public class TSEntitiesNodeID : Entities
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
                return new TSEntityNodeID(this._TSclassID);
            }
        }
        public TSEntitiesNodeID()
        {

        }
        public TSEntitiesNodeID(string tsClassID)
        {
            this._TSclassID = tsClassID;
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<TSEntityNodeID> ToJavaList()
        {
            return (System.Collections.Generic.IList<TSEntityNodeID>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<TSEntityNodeID> Tolist()
        {
            System.Collections.Generic.List<TSEntityNodeID> list = new System.Collections.Generic.List<TSEntityNodeID>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((TSEntityNodeID)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
