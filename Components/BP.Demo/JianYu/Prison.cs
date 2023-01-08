using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.JianYu
{
    /// <summary>
    /// 监狱 属性
    /// </summary>
    public class PrisonAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 班主任
        /// </summary>
        public const string BZR = "BZR";
        public const string Tel = "Tel";
    }
    /// <summary>
    /// 监狱
    /// </summary>
    public class Prison : BP.En.EntityNoName
    {
        #region 基本属性
        /// <summary>
        /// 班主任
        /// </summary>
        public string BZR
        {
            get
            {
                return this.GetValStrByKey(PrisonAttr.BZR);
            }
            set
            {
                this.SetValByKey(PrisonAttr.BZR, value);
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
        /// 监狱
        /// </summary>		
        public Prison() { }
        public Prison(string no) : base(no)
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

                Map map = new Map("JY_Prison", "监狱");

                #region 字段 
                map.AddTBStringPK(PrisonAttr.No, null, "编号", true, true, 3, 3, 50);
                map.AddTBString(PrisonAttr.Name, null, "名称", true, false, 0, 50, 200);
                // map.AddTBString(PrisonAttr.BZR, null, "班主任", true, false, 0, 50, 200);
                //  map.AddTBString(PrisonAttr.Tel, null, "班主任电话", true, false, 0, 50, 200);

                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        public override Entities GetNewEntities
        {
            get { return new Prisons(); }
        }
        #endregion
    }
    /// <summary>
    /// 监狱s
    /// </summary>
    public class Prisons : BP.En.EntitiesNoName
    {
        #region 重写
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Prison();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 监狱s
        /// </summary>
        public Prisons() { }
        #endregion
    }

}
