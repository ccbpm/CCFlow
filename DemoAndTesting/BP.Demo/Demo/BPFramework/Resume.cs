using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port;

namespace BP.Demo
{
    /// <summary>
    /// 简历 属性
    /// </summary>
    public class ResumeAttr
    {
        #region 基本属性
        /// <summary>
        /// 学生编号
        /// </summary>
        public const string RefPK = "RefPK";
        /// <summary>
        /// 工作单位
        /// </summary>
        public const string GongZuoDanWei = "GongZuoDanWei";
        /// <summary>
        /// 证明人
        /// </summary>
        public const string ZhengMingRen = "ZhengMingRen";
        /// <summary>
        /// 备注
        /// </summary>
        public const string BeiZhu = "BeiZhu";
        /// <summary>
        /// 年月
        /// </summary>
        public const string NianYue = "NianYue";
        #endregion
    }
    /// <summary>
    /// 简历
    /// </summary>
    public class Resume : BP.En.EntityOID
    {
        #region 属性
        /// <summary>
        /// 年月
        /// </summary>
        public string NianYue
        {
            get
            {
                return this.GetValStringByKey(ResumeAttr.NianYue);
            }
            set
            {
                this.SetValByKey(ResumeAttr.NianYue, value);
            }
        }
        /// <summary>
        /// 人员
        /// </summary>
        public string RefPK
        {
            get
            {
                return this.GetValStringByKey(ResumeAttr.RefPK);
            }
            set
            {
                this.SetValByKey(ResumeAttr.RefPK, value);
            }
        }
        /// <summary>
        /// 工作单位
        /// </summary>
        public string GongZuoDanWei
        {
            get
            {
                return this.GetValStringByKey(ResumeAttr.GongZuoDanWei);
            }
            set
            {
                this.SetValByKey(ResumeAttr.GongZuoDanWei, value);
            }
        }
        /// <summary>
        /// 证明人
        /// </summary>
        public string ZhengMingRen
        {
            get
            {
                return this.GetValStringByKey(ResumeAttr.ZhengMingRen);
            }
            set
            {
                this.SetValByKey(ResumeAttr.ZhengMingRen, value);
            }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string BeiZhu
        {
            get
            {
                return this.GetValStringByKey(ResumeAttr.BeiZhu);
            }
            set
            {
                this.SetValByKey(ResumeAttr.BeiZhu, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 简历
        /// </summary>
        public Resume()
        {
        }
        /// <summary>
        /// 简历
        /// </summary>
        /// <param name="oid">实体类</param>
        public Resume(int oid):base(oid)
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
                Map map = new Map("Demo_Resume", "简历");

                map.AddTBIntPKOID();
                map.AddTBString(ResumeAttr.RefPK, null, "学生编号", false, false, 0, 200, 10);
                map.AddTBString(ResumeAttr.NianYue, null, "年月", true, false, 0, 200, 50);
                map.AddTBString(ResumeAttr.GongZuoDanWei, null, "工作单位", true, false, 0, 200, 70);
                map.AddTBString(ResumeAttr.ZhengMingRen, "", "证明人", true, false, 1, 200, 50);
                map.AddTBString(ResumeAttr.BeiZhu, null, "备注", true, false, 0, 200, 150);
                map.AddTBString("QT", null, "其他", true, false, 0, 200, 150);
                map.AddTBString("QT2", null, "其他11", true, false, 0, 200, 150);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeUpdateInsertAction()
        {
            if (this.ZhengMingRen.Length == 0)
                throw new Exception("@证明人信息不能为空.");

            return base.beforeUpdateInsertAction();
        }

        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsDelete = true;
                uac.IsUpdate = true;
                uac.IsInsert = true;
                return uac;
            }
        }
    }
    /// <summary>
    /// 简历s
    /// </summary>
    public class Resumes : BP.En.EntitiesOID
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Resume();
            }
        }
        /// <summary>
        /// 简历s
        /// </summary>
        public Resumes() { }
        #endregion
    }
}
