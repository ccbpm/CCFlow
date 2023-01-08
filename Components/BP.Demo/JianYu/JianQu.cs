using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.JianYu
{
    /// <summary>
    /// 监区 属性
    /// </summary>
    public class JianQuAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 监狱编号
        /// </summary>
        public const string PrisonNo = "PrisonNo";
        public const string Tel = "Tel";
    }
    /// <summary>
    /// 监区
    /// </summary>
    public class JianQu : BP.En.EntityNoName
    {
        #region 基本属性
        /// <summary>
        /// 班主任
        /// </summary>
        public string PrisonNo
        {
            get
            {
                return this.GetValStrByKey(JianQuAttr.PrisonNo);
            }
            set
            {
                this.SetValByKey(JianQuAttr.PrisonNo, value);
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
        /// 监区
        /// </summary>		
        public JianQu() { }
        public JianQu(string no) : base(no)
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

                Map map = new Map("JY_JianQu", "监区");

                #region 字段 
                map.AddTBStringPK(JianQuAttr.No, null, "编号", true, true, 3, 3, 50);
                map.AddTBString(JianQuAttr.Name, null, "名称", true, false, 0, 50, 200);

                map.AddTBString(JianQuAttr.PrisonNo, null, "监狱编号", true, false, 0, 50, 200);

              //  map.AddDDLEntities(JianQuAttr.PrisonNo, null, "监狱", new Prisons(), false);

                //map.AddTBString(JianQuAttr.PrisonNo, null, "班主任", true, false, 0, 50, 200);
                // map.AddTBString(JianQuAttr.Tel, null, "班主任电话", true, false, 0, 50, 200);

                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        public override Entities GetNewEntities
        {
            get { return new JianQus(); }
        }
        #endregion
    }
    /// <summary>
    /// 监区s
    /// </summary>
    public class JianQus : BP.En.EntitiesNoName
    {
        #region 重写
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new JianQu();
            }
        }
        /// <summary>
        /// 查询全部
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAll()
        {
            string no = BP.Web.WebUser.FK_Dept;

            no = no.Substring(0, 4);

            return this.Retrieve("PrisonNo", no);

           // return base.RetrieveAll();
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 监区s
        /// </summary>
        public JianQus() { }
        #endregion
    }

}
