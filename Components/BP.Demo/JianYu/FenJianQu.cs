using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.JianYu
{
    /// <summary>
    /// 分监区 属性
    /// </summary>
    public class FenJianQuAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 班主任
        /// </summary>
        public const string JianYuNo = "BZR";
        public const string Tel = "Tel";
        /// <summary>
        /// 监狱编号
        /// </summary>
        public const string PrisonNo = "PrisonNo";
        /// <summary>
        /// 监区编号
        /// </summary>
        public const string JianQuNo = "JianQuNo";


    }
    /// <summary>
    /// 分监区
    /// </summary>
    public class FenJianQu : BP.En.EntityNoName
    {
        #region 基本属性
        /// <summary>
        /// 班主任
        /// </summary>
        public string PrisonNo
        {
            get
            {
                return this.GetValStrByKey(FenJianQuAttr.PrisonNo);
            }
            set
            {
                this.SetValByKey(FenJianQuAttr.PrisonNo, value);
            }
        }
        public string JianQuNo
        {
            get
            {
                return this.GetValStrByKey(FenJianQuAttr.JianQuNo);
            }
            set
            {
                this.SetValByKey(FenJianQuAttr.JianQuNo, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 实体的权限控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();

                if (BP.Web.WebUser.No == "zhoupeng" || BP.Web.WebUser.No.Equals("admin") == true)
                {
                    uac.IsDelete = true;
                    uac.IsUpdate = true;
                    uac.IsInsert = true;
                }
                else
                {
                    uac.IsDelete = false;
                    uac.IsUpdate = false;
                    uac.IsInsert = false;
                }
                return uac;
            }
        }
        /// <summary>
        /// 分监区
        /// </summary>		
        public FenJianQu() { }
        public FenJianQu(string no) : base(no)
        {
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

                Map map = new Map("JY_FenJianQu", "分监区");

                #region 字段 
                map.AddTBStringPK(FenJianQuAttr.No, null, "编号", true, true, 3, 3, 50);
                map.AddTBString(FenJianQuAttr.Name, null, "名称", true, false, 0, 50, 200);

                map.AddTBString(FenJianQuAttr.PrisonNo, null, "监狱", true, false, 0, 50, 200);
                map.AddTBString(FenJianQuAttr.JianQuNo, null, "监区", true, false, 0, 50, 200);

               // map.AddDDLEntities(JianQuAttr.PrisonNo, null, "监狱", new Prisons(), false);
               // map.AddDDLEntities(FenJianQuAttr.JianQuNo, null, "监区", new JianQus(), false);

                //	map.AddTBString(FenJianQuAttr.BZR, null, "班主任", true, false, 0, 50, 200);
                //  map.AddTBString(FenJianQuAttr.Tel, null, "班主任电话", true, false, 0, 50, 200);
                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        public override Entities GetNewEntities
        {
            get { return new FenJianQus(); }
        }
        #endregion
    }
    /// <summary>
    /// 分监区s
    /// </summary>
    public class FenJianQus : BP.En.EntitiesNoName
    {
        #region 重写
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FenJianQu();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 分监区s
        /// </summary>
        public FenJianQus() { }
        #endregion
    }

}
