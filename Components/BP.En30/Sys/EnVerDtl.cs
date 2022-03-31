using System;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.Sys
{

    public class EnVerDtlAttr
    {
        #region 基本属性

        public const string RefPK = "RefPK";
        public const string FrmID = "FrmID";
        public const string EnPKValue = "EnPKValue";

        public const string AttrKey = "AttrKey";
        public const string AttrName = "AttrName";
        public const string MyVal = "MyVal";
        public const string EnVer = "EnVer";
        public const string RDT = "RDT";
        public const string Rec = "Rec";

        public const string RefVerMyPK = "RefVerMyPK";
        public const string LGType = "LGType";
        public const string BindKey = "BindKey";
        #endregion
    }
    /// <summary>
    /// 部门岗位对应 的摘要说明。
    /// </summary>
    public class EnVerDtl : EntityMyPK
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
                uac.OpenForSysAdmin();
                return uac;

            }
        }
        public string RefPK
        {
            get
            {
                return this.GetValStringByKey(EnVerDtlAttr.RefPK);
            }
            set
            {
                SetValByKey(EnVerDtlAttr.RefPK, value);
            }
        }
        /// <summary>
        /// 实体名称
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStringByKey(EnVerDtlAttr.FrmID);
            }
            set
            {
                SetValByKey(EnVerDtlAttr.FrmID, value);
            }
        }
        public string EnPKValue
        {
            get
            {
                return this.GetValStringByKey(EnVerDtlAttr.EnPKValue);
            }
            set
            {
                SetValByKey(EnVerDtlAttr.EnPKValue, value);
            }
        }
        /// <summary>
        /// 字段
        /// </summary>
        public string AttrKey
        {
            get
            {
                return this.GetValStringByKey(EnVerDtlAttr.AttrKey);
            }

            set {
                SetValByKey(EnVerDtlAttr.AttrKey, value);
            }
        }
        /// <summary>
        /// 版本主表PK
        /// </summary>
        public string BindKey
        {
            get
            {
                return this.GetValStringByKey(EnVerDtlAttr.BindKey);
            }
            set
            {
                SetValByKey(EnVerDtlAttr.BindKey, value);
            }
        }
        /// <summary>
        ///字段名
        /// </summary>
        public string AttrName
        {
            get
            {
                return this.GetValStringByKey(EnVerDtlAttr.AttrName);
            }
            set
            {
                SetValByKey(EnVerDtlAttr.AttrName, value);
            }
        }
        public int LGType
        {
            get
            {
                return this.GetValIntByKey(EnVerDtlAttr.LGType);
            }
            set
            {
                SetValByKey(EnVerDtlAttr.LGType, value);
            }
        }
        /// <summary>
        /// 旧值
        /// </summary>
        public string MyVal
        {
            get
            {
                return this.GetValStringByKey(EnVerDtlAttr.MyVal);
            }
            set
            {
                if (value ==null)
                SetValByKey(EnVerDtlAttr.MyVal, "");
                else
                    SetValByKey(EnVerDtlAttr.MyVal, value);


            }
        }
        #endregion

        #region 扩展属性

        #endregion

        #region 构造函数
        /// <summary>
        /// 工作部门岗位对应
        /// </summary> 
        public EnVerDtl() { }

        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Sys_EnVerDtl", "版本明细");
                map.setEnType(EnType.Dot2Dot); //实体类型，admin 系统管理员表，PowerAble 权限管理表,也是用户表,你要想把它加入权限管理里面请在这里设置。。
                map.IndexField = EnVerDtlAttr.FrmID;

                map.AddMyPK();
                map.AddTBString(EnVerDtlAttr.RefPK, null, "关联版本主键", true, false, 0, 50, 30);

                map.AddTBString(EnVerDtlAttr.FrmID, null, "FrmID", false, false, 0, 200, 1);
                map.AddTBString(EnVerDtlAttr.EnPKValue, null, "EnPKValue", true, false, 0, 50, 30);

                map.AddTBString(EnVerDtlAttr.AttrKey, null, "字段", false, false, 0, 200, 1);
                map.AddTBString(EnVerDtlAttr.AttrName, null, "字段名", true, false, 0, 200, 30);
                map.AddTBInt(EnVerDtlAttr.LGType, 0, "逻辑类型", true, false);
                map.AddTBString(EnVerDtlAttr.BindKey, null, "外部数据源", true, false, 0, 200, 30);

                map.AddTBString(EnVerDtlAttr.MyVal, null, "数据值", true, false, 0, 4000, 30);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 部门岗位对应 
    /// </summary>
    public class EnVerDtls : EntitiesMyPK
    {
        #region 构造
       
        public EnVerDtls()
        {
        }
        #endregion

        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new EnVerDtl();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<EnVerDtl> ToJavaList()
        {
            return (System.Collections.Generic.IList<EnVerDtl>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<EnVerDtl> Tolist()
        {
            System.Collections.Generic.List<EnVerDtl> list = new System.Collections.Generic.List<EnVerDtl>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((EnVerDtl)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

    }
}
