using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.GPM
{
    /// <summary>
    /// 岗位属性
    /// </summary>
    public class StationAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 岗位类型
        /// </summary>
        public const string FK_StationType = "FK_StationType";
        /// <summary>
        /// 素质要求
        /// </summary>
        public const string Makings = "Makings";
        /// <summary>
        /// 职责要求
        /// </summary>
        public const string DutyReq = "DutyReq";
        /// 隶属组织
        /// </summary>
        public const string OrgNo = "OrgNo";
    }
    /// <summary>
    /// 岗位
    /// </summary>
    public class Station : EntityNoName
    {
        #region 属性
        public string FK_StationType
        {
            get
            {
                return this.GetValStrByKey(StationAttr.FK_StationType);
            }
            set
            {
                this.SetValByKey(StationAttr.FK_StationType, value);
            }
        }
        #endregion

        #region 实现基本的方方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 岗位
        /// </summary> 
        public Station()
        {
        }
        /// <summary>
        /// 岗位
        /// </summary>
        /// <param name="_No"></param>
        public Station(string _No) : base(_No) { }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Port_Station","岗位");

                map.Java_SetEnType(EnType.Admin);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetDepositaryOfEntity( Depositary.Application);
                map.Java_SetCodeStruct("4");

                map.AddTBStringPK(EmpAttr.No, null, "编号", true, true, 4, 4, 36);
                map.AddTBString(EmpAttr.Name, null, "名称", true, false, 0, 100, 200);
                map.AddDDLEntities(StationAttr.FK_StationType, null, "类型", new StationTypes(), true);
                map.AddTBStringDoc(StationAttr.DutyReq, null, "职责要求", true, false, true);
                map.AddTBStringDoc(StationAttr.Makings, null, "素质要求", true, false, true);
                map.AddTBString(StationAttr.OrgNo, null, "隶属组织", true, false, 0, 50, 250);
                map.AddSearchAttr(StationAttr.FK_StationType);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 岗位s
    /// </summary>
    public class Stations : EntitiesNoName
    {
        /// <summary>
        /// 岗位
        /// </summary>
        public Stations() { }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new BP.GPM.Station();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Station> ToJavaList()
        {
            return (System.Collections.Generic.IList<Station>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Station> Tolist()
        {
            System.Collections.Generic.List<Station> list = new System.Collections.Generic.List<Station>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Station)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
