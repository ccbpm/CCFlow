using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 流程数据同步 属性
    /// </summary>
    public class SyncDataFieldAttr : EntityMyPKAttr
    {
        ///流程编号
        public const string FlowNo = "FlowNo";
        //同步类型.
        public const string RefPKVal = "RefPKVal";
        //数据源
        public const string AttrKey = "AttrKey";
        public const string AttrName = "AttrName";
        public const string AttrType = "AttrType";

        //备注.
        public const string ToFieldKey = "ToFieldKey";
        public const string ToFieldName = "ToFieldName";
        public const string ToFieldType = "ToFieldType";

        //使用的转换函数
        public const string TurnFunc = "TurnFunc";
        //是否同步?
        public const string IsSync = "IsSync";
    }
    /// <summary>
    /// 流程数据同步
    /// </summary>
    public class SyncDataField : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FlowNo
        {
            get
            {
                return this.GetValStringByKey(SyncDataFieldAttr.FlowNo);
            }
            set
            {
                this.SetValByKey(SyncDataFieldAttr.FlowNo, value);
            }
        }

        /// <summary>
        /// 字段
        /// </summary>
        public string ToFieldKey
        {
            get
            {
                return this.GetValStringByKey(SyncDataFieldAttr.ToFieldKey);
            }
            set
            {
                this.SetValByKey(SyncDataFieldAttr.ToFieldKey, value);
            }
        }
        /// <summary>
        /// 表集合
        /// </summary>
        public string ToFieldName
        {
            get
            {
                return this.GetValStringByKey(SyncDataFieldAttr.ToFieldName);
            }
            set
            {
                this.SetValByKey(SyncDataFieldAttr.ToFieldName, value);
            }
        }
        /// <summary>
        /// 表主键
        /// </summary>
        public string RefPKVal
        {
            get
            {
                return this.GetValStringByKey(SyncDataFieldAttr.RefPKVal);
            }
            set
            {
                this.SetValByKey(SyncDataFieldAttr.RefPKVal, value);
            }
        }
        /// <summary>
        /// 表
        /// </summary>
        public string AttrKey
        {
            get
            {
                return this.GetValStringByKey(SyncDataFieldAttr.AttrKey);
            }
            set
            {
                this.SetValByKey(SyncDataFieldAttr.AttrKey, value);
            }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string AttrName
        {
            get
            {
                return this.GetValStringByKey(SyncDataFieldAttr.AttrName);
            }
            set
            {
                this.SetValByKey(SyncDataFieldAttr.AttrName, value);
            }
        }
        /// <summary>
        /// URL 
        /// </summary>
        public string AttrType
        {
            get
            {
                return this.GetValStringByKey(SyncDataFieldAttr.AttrType);
            }
            set
            {
                this.SetValByKey(SyncDataFieldAttr.AttrType, value);
            }
        }
        /// <summary>
        /// 数据源
        /// </summary>
        public string TurnFunc
        {
            get
            {
                return this.GetValStringByKey(SyncDataFieldAttr.TurnFunc);
            }
            set
            {
                this.SetValByKey(SyncDataFieldAttr.TurnFunc, value);
            }
        }
        /// <summary>
        /// 类型
        /// </summary>
        public bool IsSync
        {
            get
            {
                return this.GetValBooleanByKey(SyncDataFieldAttr.IsSync);
            }
            set
            {
                this.SetValByKey(SyncDataFieldAttr.IsSync, value);
            }
        }

        #endregion

        #region 构造函数
        /// <summary>
        /// SyncDataField
        /// </summary>
        public SyncDataField()
        {
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
                Map map = new Map("WF_SyncDataField", "流程数据同步");

                map.AddMyPK();
                map.AddTBString(SyncDataFieldAttr.FlowNo, null, "流程编号", false, false, 0, 10, 50, true);
                map.AddTBString(SyncDataFieldAttr.RefPKVal, null, "关键内容", false, false, 0, 50, 50, true);

                map.AddTBString(SyncDataFieldAttr.AttrKey, null, "业务字段", true, true, 0, 100, 50, false);
                map.AddTBString(SyncDataFieldAttr.AttrName, null, "字段名", true, true, 0, 100, 50, false);
                map.AddTBString(SyncDataFieldAttr.AttrType, null, "类型", true, true, 0, 100, 50, false);

                map.AddTBString(SyncDataFieldAttr.ToFieldKey, null, "同步到字段", true, true, 0, 100, 50, false);
                map.AddTBString(SyncDataFieldAttr.ToFieldName, null, "字段名", true, true, 0, 100, 50, false);
                map.AddTBString(SyncDataFieldAttr.ToFieldType, null, "类型", true, true, 0, 100, 50, false);

                map.AddBoolean(SyncDataFieldAttr.IsSync, false, "同步?", true, true);
                map.AddTBString(SyncDataFieldAttr.TurnFunc, null, "转换函数", true, false, 0, 50, 50);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

    }
    /// <summary>
    /// 流程数据同步
    /// </summary>
    public class SyncDataFields : EntitiesMyPK
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SyncDataField();
            }
        }
        /// <summary>
        /// 流程数据同步
        /// </summary>
        public SyncDataFields() { }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SyncDataField> ToJavaList()
        {
            return (System.Collections.Generic.IList<SyncDataField>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SyncDataField> Tolist()
        {
            System.Collections.Generic.List<SyncDataField> list = new System.Collections.Generic.List<SyncDataField>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SyncDataField)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
