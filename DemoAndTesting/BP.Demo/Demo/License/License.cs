using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;
using BP.GPM;
namespace BP.Demo.License
{
    public class LicenseType
    {
        /// <summary>
        /// 身份证
        /// </summary>
        public const string SFZ = "SFZ";
        /// <summary>
        /// 驾驶证
        /// </summary>
        public const string JSZ = "JSZ";
        /// <summary>
        /// 房产证
        /// </summary>
        public const string FCZ = "FCZ";
        /// <summary>
        /// 结婚证
        /// </summary>
        public const string JHZ = "JHZ";


        public static string GetNameByID(string id)
        {
            switch (id)
            {
                case LicenseType.SFZ:
                    return "身份证";
                case LicenseType.JSZ:
                    return "驾驶证";
                case LicenseType.FCZ:
                    return "房产证";
                case LicenseType.JHZ:
                    return "结婚证";
                default:
                    throw new Exception("@没有涉及到的ID" + id);
            }
        }
        public static string GetIDByName(string name)
        {
            switch (name)
            {
                case "身份证" :
                    return LicenseType.SFZ;
                case "驾照":
                case "驾驶证":
                    return LicenseType.JSZ;
                case "房产证" :
                    return LicenseType.FCZ;
                case "结婚证":
                    return LicenseType.JHZ;
                default:
                    throw new Exception("@没有涉及到的ID" + name);
            }
        }
    }
    /// <summary>
    /// 证照属性
    /// </summary>
    public class LicenseAttr : BP.En.EntityNoNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 身份证号
        /// </summary>
        public const string SFZ = "SFZ";
        /// <summary>
        /// 类型ID
        /// </summary>
        public const string ZJLX = "ZJLX";
        /// <summary>
        /// 证件类型名称
        /// </summary>
        public const string ZJLXName = "ZJLXName";
        /// <summary>
        /// 证件代码
        /// </summary>
        public const string ZJCode = "ZJCode";
        /// <summary>
        /// 性别
        /// </summary>
        public const string XB = "XB";
        /// <summary>
        /// 性别
        /// </summary>
        public const string FilePath = "FilePath";

        /// <summary>
        /// 上传日期
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 上传人
        /// </summary>
        public const string Rec = "Rec";
        /// <summary>
        /// 扩展
        /// </summary>
        public const string Ext = "Ext";
        /// <summary>
        /// 文件大小
        /// </summary>
        public const string FileSize = "FileSize";
        #endregion

    }
    /// <summary>
    /// 证照 的摘要说明。
    /// </summary>
    public class License : EntityMyPK
    {
        #region 扩展属性
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return LicenseType.GetNameByID(this.ZJLX);
            }
        }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string SFZ
        {
            get
            {
                return this.GetValStrByKey(LicenseAttr.SFZ);
            }
            set
            {
                this.SetValByKey(LicenseAttr.SFZ, value);
            }
        }
        public string FilePath
        {
            get
            {
                return this.GetValStrByKey(LicenseAttr.FilePath);
            }
            set
            {
                this.SetValByKey(LicenseAttr.FilePath, value);
            }
        }
        /// <summary>
        /// 证件类型ID
        /// </summary>
        public string ZJLX
        {
            get
            {
                return this.GetValStrByKey(LicenseAttr.ZJLX);
            }
            set
            {
                this.SetValByKey(LicenseAttr.ZJLX, value);
            }
        }
        /// <summary>
        /// 上传人
        /// </summary>
        public string Rec
        {
            get
            {
                return this.GetValStrByKey(LicenseAttr.Rec);
            }
            set
            {
                this.SetValByKey(LicenseAttr.Rec, value);
            }
        }
        /// <summary>
        /// 上传日期
        /// </summary>
        public string RDT
        {
            get
            {
                return this.GetValStrByKey(LicenseAttr.RDT);
            }
            set
            {
                this.SetValByKey(LicenseAttr.RDT, value);
            }
        }
        /// <summary>
        /// 证件代码
        /// </summary>
        public string ZJCode
        {
            get
            {
                string str= this.GetValStrByKey(LicenseAttr.ZJCode);
                if (string.IsNullOrEmpty(str))
                    return "无";
                return str;
            }
            set
            {
                this.SetValByKey(LicenseAttr.ZJCode, value);
            }
        }
        /// <summary>
        /// 文件ext
        /// </summary>
        public string Ext
        {
            get
            {
                return this.GetValStrByKey(LicenseAttr.Ext);
            }
            set
            {
                this.SetValByKey(LicenseAttr.Ext, value);
            }
        }
        public float FileSize
        {
            get
            {
                return this.GetValFloatByKey(LicenseAttr.FileSize);
            }
            set
            {
                this.SetValByKey(LicenseAttr.FileSize, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 证照
        /// </summary>
        public License()
        {
        }
        /// <summary>
        /// 证照
        /// </summary>
        /// <param name="no">编号</param>
        public License(string no)
        {
            this.MyPK = no.Trim();
            if (this.MyPK.Length == 0)
                throw new Exception("@要查询的证照编号为空。");
            try
            {
                this.Retrieve();
            }
            catch (Exception ex)
            {
                int i = this.RetrieveFromDBSources();
                if (i == 0)
                    throw new Exception("@用户或者密码错误：[" + no + "]，或者帐号被停用。@技术信息(从内存中查询出现错误)：ex1=" + ex.Message);
            }
        }
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForAppAdmin();
                return uac;
            }
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

                Map map = new Map();

                #region 基本属性
                map.PhysicsTable = "Demo_License"; // 要物理表。
                map.DepositaryOfMap = Depositary.Application;    //实体map的存放位置.
                map.DepositaryOfEntity = Depositary.None; //实体存放位置
                map.EnDesc = "用户"; // "用户"; // 实体的描述.
                map.EnType = EnType.App;   //实体类型。
                #endregion

                #region 字段
                /*关于字段属性的增加 */
                map.AddMyPK();
                map.AddTBString(LicenseAttr.SFZ, null, "身份证号", true, false, 0, 500, 30);

                map.AddTBString(LicenseAttr.Name, null, "证件名称", true, false, 0, 500, 30);
                map.AddTBString(LicenseAttr.ZJLX, null, "证件类型", true, false, 0, 500, 30);

                map.AddTBString(LicenseAttr.FilePath, null, "文件路径", false, false, 0, 500, 10);
                map.AddTBString(LicenseAttr.Ext, null, "扩展名", false, false, 0, 20, 10);

                map.AddTBString(LicenseAttr.ZJCode, null, "证件代码", false, false, 0, 500, 10);

                map.AddTBString(LicenseAttr.RDT, null, "上传日期", false, false, 0, 500, 10);
                map.AddTBString(LicenseAttr.Rec, null, "上传人", false, false, 0, 500, 10);

                map.AddTBFloat(LicenseAttr.FileSize, 0, "大小", false, false);
                #endregion 字段


                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 获取集合
        /// </summary>
        public override Entities GetNewEntities
        {
            get { return new Licenses(); }
        }
        #endregion 构造函数
    }
    /// <summary>
    /// 证照s
    // </summary>
    public class Licenses : EntitiesMyPK
    {
        #region 构造方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new License();
            }
        }
        /// <summary>
        /// 证照s
        /// </summary>
        public Licenses()
        {
        }
        /// <summary>
        /// 身份证
        /// </summary>
        /// <param name="sfz"></param>
        public Licenses(string sfz)
        {
            this.Retrieve(LicenseAttr.SFZ, sfz);
        }
        #endregion 构造方法
    }
}
