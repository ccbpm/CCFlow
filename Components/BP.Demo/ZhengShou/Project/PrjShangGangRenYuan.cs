using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.ZS
{
    /// <summary>
    /// 上岗人员 attr
    /// </summary>
    public class PrjShangGangRenYuanAttr : EntityNoNameAttr
    {
        public const string Faren = "Faren";
        /// <summary>
        /// 电话
        /// </summary>
        public const string Tel = "Tel";
        /// <summary>
        /// 身份证
        /// </summary>
        public const string SFZ = "SFZ";
        /// <summary>
        /// 性别
        /// </summary>
        public const string XB = "XB";
        /// <summary>
        /// 状态
        /// </summary>
        public const string SGSta = "SGSta";
    }
    /// <summary>
    ///  上岗人员
    /// </summary>
    public class PrjShangGangRenYuan : EntityOID
    {
        #region 属性.
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenAll();
                return uac;
            }
        }
        public string No
        {
            get
            {
                return this.GetValStringByKey("No");
            }
            set
            {
                this.SetValByKey("No", value);
            }
        }
        public string Tel
        {
            get
            {
                return this.GetValStringByKey("Tel");
            }
            set
            {
                this.SetValByKey("Tel", value);
            }
        }
        public string SFZ
        {
            get
            {
                return this.GetValStringByKey("SFZ");
            }
            set
            {
                this.SetValByKey("SFZ", value);
            }
        }
        #endregion 属性.

        #region 构造方法
        /// <summary>
        /// 上岗人员
        /// </summary>
        public PrjShangGangRenYuan()
        {
        }
        /// <summary>
        /// 上岗人员
        /// </summary>
        /// <param name="_No"></param>
        public PrjShangGangRenYuan(int _No) : base(_No) { }
        /// <summary>
        /// 上岗人员Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("ZS_PrjShangGangRenYuan", "上岗人员");

                map.AddTBIntPKOID();

                map.AddTBString("project_id", null, "所属项目id", false, false, 0, 50, 50);

                map.AddTBString(ShiShiDanWeiAttr.No, null, "上岗证编号", true, false, 5, 5, 100);
                map.AddTBString(PrjShangGangRenYuanAttr.SFZ, null, "身份证", true, false, 0, 50, 200);

                map.AddDDLSysEnum(PrjShangGangRenYuanAttr.XB, 0, "性别", true, true, PrjShangGangRenYuanAttr.XB, "@0=女@1=男@2=其他");
                map.AddTBString(ShiShiDanWeiAttr.Name, null, "姓名", true, true, 0, 50, 50);
                map.AddTBString(PrjShangGangRenYuanAttr.Tel, null, "手机号", true, true, 0, 50, 50);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeUpdateInsertAction()
        {
            ///有上岗证.
            if (DataType.IsNullOrEmpty(this.No) == false)
            {
                ShangGangRenYuan en = new ShangGangRenYuan(this.No);
                this.Copy(en);
                return base.beforeUpdateInsertAction();
            }

            //如果有身份证.
            if (DataType.IsNullOrEmpty(this.SFZ) == false)
            {
                ShangGangRenYuan en = new ShangGangRenYuan();
                int i = en.Retrieve("SFZ", this.SFZ);
                if (i == 0)
                    throw new Exception("err@身份证错误，上岗人员查无此人.");
                this.Copy(en);
                return base.beforeUpdateInsertAction();
            }

            throw new Exception("err@身份证/上岗证号，不能为空.");

        }
    }
    /// <summary>
    /// 上岗人员s
    /// </summary>
    public class PrjShangGangRenYuans : EntitiesOID
    {
        /// <summary>
        /// 上岗人员s
        /// </summary>
        public PrjShangGangRenYuans() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new PrjShangGangRenYuan();
            }
        }
    }
}
