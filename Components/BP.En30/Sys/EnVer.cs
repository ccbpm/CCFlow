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
        public const string FrmID = "FrmID";
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
        public const string EnVer = "EnVer";
        /// <summary>
        /// 说明
        /// </summary>
        public const string MyNote = "MyNote";
        /// <summary>
        /// 记录人
        /// </summary>
        public const string RecNo = "RecNo";
        /// <summary>
        /// 记录人名字
        /// </summary>
        public const string RecName = "RecName";
        /// <summary>
        /// 记录日期
        /// </summary>
        public const string RDT = "RDT";

        public const string EnPKValue = "EnPKValue";

    }
    /// <summary>
    /// 实体版本号
    /// </summary>
    public class EnVer : EntityMyPK
    {
        #region 属性
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
        public string EnPKValue
        {
            get
            {
                return this.GetValStrByKey(EnVerAttr.EnPKValue);
            }
            set
            {
                this.SetValByKey(EnVerAttr.EnPKValue, value);
            }
        }


        /// <summary>
        /// 版本号
        /// </summary>
        public int Ver
        {
            get
            {
                return this.GetValIntByKey(EnVerAttr.EnVer);
            }
            set
            {
                this.SetValByKey(EnVerAttr.EnVer, value);
            }
        }
        /// <summary>
        /// 修改人
        /// </summary>
        public string RecNo
        {
            get
            {
                return this.GetValStrByKey(EnVerAttr.RecNo);
            }
            set
            {
                this.SetValByKey(EnVerAttr.RecNo, value);
            }
        }
        public string RecName
        {
            get
            {
                return this.GetValStrByKey(EnVerAttr.RecName);
            }
            set
            {
                this.SetValByKey(EnVerAttr.RecName, value);
            }
        }
        public string MyNote
        {
            get
            {
                return this.GetValStrByKey(EnVerAttr.MyNote);
            }
            set
            {
                this.SetValByKey(EnVerAttr.MyNote, value);
            }
        }
        public string FrmID
        {
            get
            {
                return this.GetValStrByKey(EnVerAttr.FrmID);
            }
            set
            {
                this.SetValByKey(EnVerAttr.FrmID, value);
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
        public EnVer(string mypk) 
        {
            this.MyPK = mypk;
            this.Retrieve();
        }

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

                Map map = new Map("Sys_EnVer", "实体版本号");
                map.EnDBUrl = new DBUrl(DBUrlType.AppCenterDSN); //连接到的那个数据库上. (默认的是: AppCenterDSN )

                map.AddMyPK();

                map.AddTBString(EnVerAttr.FrmID, null, "实体类", true, false, 1, 50, 20);
                map.AddTBString(EnVerAttr.Name, null, "实体名", true, false, 0, 100, 30);
                map.AddTBString(EnVerAttr.EnPKValue, null, "主键值", true, true, 0, 40, 100);

                map.AddTBInt(EnVerAttr.EnVer, 0, "版本号", true, true);

                map.AddTBString(EnVerAttr.RecNo, null, "修改人账号", true, true, 0, 100, 30);
                map.AddTBString(EnVerAttr.RecName, null, "修改人名称", true, true, 0, 100, 30);

                map.AddTBString(EnVerAttr.MyNote, null, "备注", true, true, 0, 100, 30);
                map.AddTBDateTime(EnVerAttr.RDT, null, "创建日期", true, true);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            return base.beforeInsert();
        }
        protected override void afterDelete()
        {
            //删除数据.
            EnVerDtls dtls = new EnVerDtls();
            dtls.Delete(EnVerDtlAttr.RefPK, this.MyPK);
            base.afterDelete();
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
