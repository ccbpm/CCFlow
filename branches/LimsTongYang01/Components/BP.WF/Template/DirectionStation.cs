using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.En;
using BP.WF.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 方向与工作岗位对应属性	  
    /// </summary>
    public class DirectionStationAttr
    {
        /// <summary>
        /// 节点
        /// </summary>
        public const string FK_Direction = "FK_Direction";
        /// <summary>
        /// 工作岗位
        /// </summary>
        public const string FK_Station = "FK_Station";
    }
    /// <summary>
    /// 方向与工作岗位对应
    /// 节点的工作岗位有两部分组成.	 
    /// 记录了从一个节点到其他的多个节点.
    /// 也记录了到这个节点的其他的节点.
    /// </summary>
    public class DirectionStation : EntityMM
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
        ///节点
        /// </summary>
        public int FK_Direction
        {
            get
            {
                return this.GetValIntByKey(DirectionStationAttr.FK_Direction);
            }
            set
            {
                this.SetValByKey(DirectionStationAttr.FK_Direction, value);
            }
        }
        public string FK_StationT
        {
            get
            {
                return this.GetValRefTextByKey(DirectionStationAttr.FK_Station);
            }
        }
        /// <summary>
        /// 工作岗位
        /// </summary>
        public string FK_Station
        {
            get
            {
                return this.GetValStringByKey(DirectionStationAttr.FK_Station);
            }
            set
            {
                this.SetValByKey(DirectionStationAttr.FK_Station, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 方向与工作岗位对应
        /// </summary>
        public DirectionStation() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_DirectionStation", "节点岗位");

                map.AddTBIntPK(DirectionStationAttr.FK_Direction, 0,"节点", false,false);

                if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneOne)
                {
                    map.AddDDLEntitiesPK(DirectionStationAttr.FK_Station, null, "工作岗位",
                        new BP.Port.Stations(), true);
                }
                else
                {
 // #warning ,这里为了方便用户选择，让分组都统一采用了枚举类型. edit zhoupeng. 2015.04.28. 注意jflow也要修改.
                    map.AddDDLEntitiesPK(DirectionStationAttr.FK_Station, null, "工作岗位",
                       new BP.GPM.Stations(), true);
                }
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 方向与工作岗位对应
    /// </summary>
    public class DirectionStations : EntitiesMM
    {
        /// <summary>
        /// 方向与工作岗位对应
        /// </summary>
        public DirectionStations() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new DirectionStation();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<DirectionStation> ToJavaList()
        {
            return (System.Collections.Generic.IList<DirectionStation>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<DirectionStation> Tolist()
        {
            System.Collections.Generic.List<DirectionStation> list = new System.Collections.Generic.List<DirectionStation>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((DirectionStation)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
