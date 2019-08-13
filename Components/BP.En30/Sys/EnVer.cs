using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web;

namespace BP.Sys
{
    /// <summary>
    /// 实体版本号属性
    /// </summary>
    public class EnVerAttr : EntityMyPKAttr
    {
        /// <summary>
        ///  实体类
        /// </summary>
        public const string No = "No";
        /// <summary>
        ///  实体类名称
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// 主键值
        /// </summary>
        public const string PKValue = "PKValue";
        /// <summary>
        /// 版本号
        /// </summary>
        public const string EVer = "EVer";
        /// <summary>
        /// 记录人
        /// </summary>
        public const string Rec = "Rec";
        /// <summary>
        /// 记录日期
        /// </summary>
        public const string RDT = "RDT";
    }
    /// <summary>
    /// 实体版本号
    /// </summary>
    public class EnVer : EntityMyPK
    {
        #region 属性

        /// <summary>
        /// 实体类
        /// </summary>
        public string No
        {
            get
            {
                return this.GetValStrByKey(EnVerAttr.No);
            }
            set
            {
                this.SetValByKey(EnVerAttr.No, value);
            }
        }

        /// <summary>
        /// 实体类名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStrByKey(EnVerAttr.Name);
            }
            set
            {
                this.SetValByKey(EnVerAttr.Name, value);
            }
        }

        /// <summary>
        /// 版本号
        /// </summary>
        public int EVer
        {
            get
            {
                return this.GetValIntByKey(EnVerAttr.EVer);
            }
            set
            {
                this.SetValByKey(EnVerAttr.EVer, value);
            }
        }
        /// <summary>
        /// 修改人
        /// </summary>
        public string Rec
        {
            get
            {
                return this.GetValStrByKey(EnVerAttr.Rec);
            }
            set
            {
                this.SetValByKey(EnVerAttr.Rec, value);
            }
        }

        /// <summary>
        /// 修改日期
        /// </summary>
        public string RDT
        {
            get
            {
                return this.GetValStrByKey(EnVerAttr.RDT);
            }
            set
            {
                this.SetValByKey(EnVerAttr.RDT, value);
            }
        }

        /// <summary>
        /// 主键值
        /// </summary>
        public string PKValue
        {
            get
            {
                return this.GetValStrByKey(EnVerAttr.PKValue);
            }
            set
            {
                this.SetValByKey(EnVerAttr.PKValue, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 实体版本号
        /// </summary>
        public EnVer() { }
        #endregion

        #region 重写方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        /// <summary>
        /// Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map();
                map.EnDBUrl = new DBUrl(DBUrlType.AppCenterDSN); //连接到的那个数据库上. (默认的是: AppCenterDSN )
                map.PhysicsTable = "Sys_EnVer";
                map.Java_SetEnType(EnType.Admin);
                map.EnDesc = "实体版本号"; //  实体的描述.
                map.Java_SetDepositaryOfEntity( Depositary.Application); //实体map的存放位置.
                map.Java_SetDepositaryOfMap( Depositary.Application);    // Map 的存放位置.
                map.IndexField = EnVerAttr.EVer;


                map.AddMyPK();
                map.AddTBString(EnVerAttr.No, null, "实体类", true, false, 1, 50, 20);
                map.AddTBString(EnVerAttr.Name, null, "实体名", true, false, 0, 100, 30);
                map.AddTBString(EnVerAttr.PKValue, null, "主键值", true, true, 0, 300, 100);
                map.AddTBInt(EnVerAttr.EVer, 1, "版本号", true, true);
                map.AddTBString(EnVerAttr.Rec, null, "修改人", true, true, 0, 100, 30);
                map.AddTBDateTime(EnVerAttr.RDT, null, "修改日期", true, true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            this.SetValByKey("MyPK", this.No + "_" + this.PKValue);
            return base.beforeInsert();
        }
    }
    /// <summary>
    ///实体版本号s
    /// </summary>
    public class EnVers : EntitiesMyPK
    {
        /// <summary>
        /// 得到一个新实体
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new EnVer();
            }
        }
        /// <summary>
        /// 实体版本号集合
        /// </summary>
        public EnVers()
        {
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<EnVer> ToJavaList()
        {
            return (System.Collections.Generic.IList<EnVer>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<EnVer> Tolist()
        {
            System.Collections.Generic.List<EnVer> list = new System.Collections.Generic.List<EnVer>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((EnVer)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
