﻿using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.WF.Port.Admin2
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
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// 顺序号
        /// </summary>
        public const string Idx = "Idx";

    }
    /// <summary>
    /// 岗位
    /// </summary>
    public class Station : EntityNoName
    {
        #region 属性.
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStrByKey(StationAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(StationAttr.OrgNo, value);
            }
        }
        /// <summary>
        /// 岗位类型
        /// </summary>
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
        #endregion 属性.

        #region 实现基本的方方法
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
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

                Map map = new Map("Port_Station", "岗位");
                map.Java_SetEnType(EnType.Admin);

                map.Java_SetDepositaryOfEntity(Depositary.Application);
                map.Java_SetCodeStruct("4"); // 最大级别是7.

                map.AddTBStringPK(StationAttr.No, null, "编号", true, true, 1, 50, 36);
                map.AddTBString(StationAttr.Name, null, "名称", true, false, 1, 50, 250);
                map.AddDDLEntities(StationAttr.FK_StationType, null, "岗位类型", new StationTypes(), true);
                map.AddTBString(StationAttr.OrgNo, null, "OrgNo", true, true, 0, 60, 100);
                map.AddTBInt(StationAttr.Idx, 0, "顺序", true, false);

                //增加隐藏查询条件.
                map.AddHidden(StationAttr.OrgNo, "=", BP.Web.WebUser.OrgNo);

                //查询条件.
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
                return new Station();
            }
        }

        public override int RetrieveAll()
        {
            return this.Retrieve(StationAttr.OrgNo, BP.Web.WebUser.OrgNo, StationAttr.Idx);
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