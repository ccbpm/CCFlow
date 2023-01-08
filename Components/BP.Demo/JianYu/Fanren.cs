using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.JianYu
{
    /// <summary>
    /// 犯人 属性
    /// </summary>
    public class FanrenAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 班主任
        /// </summary>
        public const string BZR = "BZR";
        public const string Tel = "Tel";

        public const string WorkIDOfBatch = "WorkIDOfBatch";
        public const string WorkIDOfSubthread = "WorkIDOfSubthread";
        /// <summary>
        /// 分监区编号
        /// </summary>
        public const string FenJianQuNo = "FenJianQuNo";
        /// <summary>
        /// 监区编号
        /// </summary>
        public const string JianQuNo = "JianQuNo";
        /// <summary>
        /// 监狱编号
        /// </summary>
        public const string PrisonNo = "PrisonNo";
    }
    /// <summary>
    /// 犯人
    /// </summary>
    public class Fanren : BP.En.EntityNoName
    {
        #region 基本属性
        /// <summary>
        /// 班主任
        /// </summary>
        public string FenJianQuNo
        {
            get
            {
                return this.GetValStrByKey(FanrenAttr.FenJianQuNo);
            }
            set
            {
                this.SetValByKey(FanrenAttr.FenJianQuNo, value);
            }
        }
        public string JianQuNo
        {
            get
            {
                return this.GetValStrByKey(FanrenAttr.JianQuNo);
            }
            set
            {
                this.SetValByKey(FanrenAttr.JianQuNo, value);
            }
        }
        public string PrisonNo
        {
            get
            {
                return this.GetValStrByKey(FanrenAttr.PrisonNo);
            }
            set
            {
                this.SetValByKey(FanrenAttr.PrisonNo, value);
            }
        }
        /// <summary>
        /// 子线程ID.
        /// </summary>
        public int WorkIDOfSubthread
        {
            get
            {
                return this.GetValIntByKey(FanrenAttr.WorkIDOfSubthread);
            }
            set
            {
                this.SetValByKey(FanrenAttr.WorkIDOfSubthread, value);
            }
        }
        /// <summary>
        /// 批次ID
        /// </summary>
        public int WorkIDOfBatch
        {
            get
            {
                return this.GetValIntByKey(FanrenAttr.WorkIDOfBatch);
            }
            set
            {
                this.SetValByKey(FanrenAttr.WorkIDOfBatch, value);
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
        /// 犯人
        /// </summary>		
        public Fanren() { }
        public Fanren(string no) : base(no)
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

                Map map = new Map("JY_Fanren", "犯人");

                #region 字段 
                map.AddTBStringPK(FanrenAttr.No, null, "编号", true, true, 3, 3, 50);
                map.AddTBString(FanrenAttr.Name, null, "名称", true, false, 0, 50, 200);

                map.AddDDLEntities(FanrenAttr.PrisonNo, null, "监狱", new Prisons(), false);
                map.AddTBInt(FanrenAttr.WorkIDOfBatch, 0, "批次的WorkID", true, false);

                map.AddDDLEntities(FanrenAttr.JianQuNo, null, "监区", new JianQus(), false);
                map.AddTBInt(FanrenAttr.WorkIDOfSubthread, 0, "子线程WorkID", true, false);

                map.AddDDLEntities(FanrenAttr.FenJianQuNo, null, "分监区", new FenJianQus(), false);
                #endregion

                //按照监狱来查询.
                map.AddSearchAttr(JianQuAttr.PrisonNo);

                //不带有参数的方法.
                RefMethod rm = new RefMethod();
                rm.Title = "审批过程";
                rm.ClassMethodName = this.ToString() + ".DoFlow";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.IsCanBatch = false; //是否可以批处理？
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        public string DoFlow()
        {
            return "/WF/MyView.htm?WorkID=" + this.WorkIDOfSubthread+"&FID="+this.WorkIDOfBatch+"&FK_Flow=001";
        }
        protected override bool beforeInsert()
        {

            FenJianQu en = new FenJianQu(this.FenJianQuNo);
            this.PrisonNo = en.PrisonNo;
            this.JianQuNo = en.JianQuNo;

            return base.beforeInsert();
        }
        public override Entities GetNewEntities
        {
            get { return new Fanrens(); }
        }
        #endregion
    }
    /// <summary>
    /// 犯人s
    /// </summary>
    public class Fanrens : BP.En.EntitiesNoName
    {
        #region 重写
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Fanren();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 犯人s
        /// </summary>
        public Fanrens() { }
        #endregion
    }

}
