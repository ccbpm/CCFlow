using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.En;
using BP.WF.Port;

namespace BP.WF.CCBill
{
    /// <summary>
    /// 单据可创建的工作岗位属性	  
    /// </summary>
    public class FrmStationCreateAttr
    {
        /// <summary>
        /// 单据
        /// </summary>
        public const string FrmID = "FrmID";
        /// <summary>
        /// 工作岗位
        /// </summary>
        public const string FK_Station = "FK_Station";
    }
    /// <summary>
    /// 单据可创建的工作岗位
    /// 单据的工作岗位有两部分组成.	 
    /// 记录了从一个单据到其他的多个单据.
    /// 也记录了到这个单据的其他的单据.
    /// </summary>
    public class FrmStationCreate : EntityMM
    {
        #region 基本属性
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenAll();
                return uac;
            }
        }
        /// <summary>
        ///单据
        /// </summary>
        public int FrmID
        {
            get
            {
                return this.GetValIntByKey(FrmStationCreateAttr.FrmID);
            }
            set
            {
                this.SetValByKey(FrmStationCreateAttr.FrmID, value);
            }
        }
        public string FK_StationT
        {
            get
            {
                return this.GetValRefTextByKey(FrmStationCreateAttr.FK_Station);
            }
        }
        /// <summary>
        /// 工作岗位
        /// </summary>
        public string FK_Station
        {
            get
            {
                return this.GetValStringByKey(FrmStationCreateAttr.FK_Station);
            }
            set
            {
                this.SetValByKey(FrmStationCreateAttr.FK_Station, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 单据可创建的工作岗位
        /// </summary>
        public FrmStationCreate() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_FrmStationCreate", "单据岗位");

                map.AddTBStringPK(FrmStationCreateAttr.FrmID, null, "表单", true, true, 1, 100, 100);
                map.AddDDLEntitiesPK(FrmStationCreateAttr.FK_Station, null, "可以创建岗位",
                   new BP.GPM.Stations(), true);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

    }
    /// <summary>
    /// 单据可创建的工作岗位
    /// </summary>
    public class FrmStationCreates : EntitiesMM
    {
        #region 构造函数.
        /// <summary>
        /// 单据可创建的工作岗位
        /// </summary>
        public FrmStationCreates() { }
        /// <summary>
        /// 单据可创建的工作岗位
        /// </summary>
        /// <param name="nodeID">单据ID</param>
        public FrmStationCreates(int nodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FrmStationCreateAttr.FrmID, nodeID);
            qo.DoQuery();
        }
        /// <summary>
        /// 单据可创建的工作岗位
        /// </summary>
        /// <param name="StationNo">StationNo </param>
        public FrmStationCreates(string StationNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FrmStationCreateAttr.FK_Station, StationNo);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmStationCreate();
            }
        }
        #endregion 构造函数.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmStationCreate> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmStationCreate>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmStationCreate> Tolist()
        {
            System.Collections.Generic.List<FrmStationCreate> list = new System.Collections.Generic.List<FrmStationCreate>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmStationCreate)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
