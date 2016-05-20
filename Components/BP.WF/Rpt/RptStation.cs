using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.WF.Rpt
{
    /// <summary>
    /// 报表岗位
    /// </summary>
    public class RptStationAttr
    {
        #region 基本属性
        /// <summary>
        /// 报表ID
        /// </summary>
        public const string FK_Rpt = "FK_Rpt";
        /// <summary>
        /// 岗位
        /// </summary>
        public const string FK_Station = "FK_Station";
        #endregion
    }
    /// <summary>
    /// RptStation 的摘要说明。
    /// </summary>
    public class RptStation : Entity
    {

        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.No == "admin")
                {
                    uac.IsView = true;
                    uac.IsDelete = true;
                    uac.IsInsert = true;
                    uac.IsUpdate = true;
                    uac.IsAdjunct = true;
                }
                return uac;
            }
        }

        #region 基本属性
        /// <summary>
        /// 报表ID
        /// </summary>
        public string FK_Rpt
        {
            get
            {
                return this.GetValStringByKey(RptStationAttr.FK_Rpt);
            }
            set
            {
                SetValByKey(RptStationAttr.FK_Rpt, value);
            }
        }
        public string FK_StationT
        {
            get
            {
                return this.GetValRefTextByKey(RptStationAttr.FK_Station);
            }
        }
        /// <summary>
        ///岗位
        /// </summary>
        public string FK_Station
        {
            get
            {
                return this.GetValStringByKey(RptStationAttr.FK_Station);
            }
            set
            {
                SetValByKey(RptStationAttr.FK_Station, value);
            }
        }
        #endregion

        #region 扩展属性

        #endregion

        #region 构造函数
        /// <summary>
        /// 报表岗位
        /// </summary> 
        public RptStation() { }
        /// <summary>
        /// 报表岗位对应
        /// </summary>
        /// <param name="_empoid">报表ID</param>
        /// <param name="wsNo">岗位编号</param> 	
        public RptStation(string _empoid, string wsNo)
        {
            this.FK_Rpt = _empoid;
            this.FK_Station = wsNo;
            if (this.Retrieve() == 0)
                this.Insert();
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

                Map map = new Map("Sys_RptStation", "报表岗位对应信息");
                map.Java_SetEnType(EnType.Dot2Dot);

                map.AddTBStringPK(RptStationAttr.FK_Rpt, null, "报表", false, false, 1, 15, 1);
                map.AddDDLEntitiesPK(RptStationAttr.FK_Station, null, "岗位", new Stations(), true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 报表岗位 
    /// </summary>
    public class RptStations : Entities
    {
        #region 构造
        /// <summary>
        /// 报表与岗位集合
        /// </summary>
        public RptStations() { }
        #endregion

        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new RptStation();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<RptStation> ToJavaList()
        {
            return (System.Collections.Generic.IList<RptStation>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<RptStation> Tolist()
        {
            System.Collections.Generic.List<RptStation> list = new System.Collections.Generic.List<RptStation>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((RptStation)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
