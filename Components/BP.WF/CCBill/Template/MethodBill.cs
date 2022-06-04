using System;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Sys;

namespace BP.CCBill.Template
{
    /// <summary>
    /// 方法单据
    /// </summary>
    public class MethodBill : EntityNoName
    {
        #region 基本属性
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStringByKey(MethodAttr.FrmID);
            }
            set
            {
                this.SetValByKey(MethodAttr.FrmID, value);
            }
        }
        /// <summary>
        /// 方法ID
        /// </summary>
        public string MethodID
        {
            get
            {
                return this.GetValStringByKey(MethodAttr.MethodID);
            }
            set
            {
                this.SetValByKey(MethodAttr.MethodID, value);
            }
        }

        public string Tag1
        {
            get
            {
                string tag1 = this.GetValStringByKey(MethodAttr.Tag1);
                return tag1;
            }
        }


        #endregion

        #region 构造方法
        /// <summary>
        /// 权限控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (WebUser.IsAdmin)
                {
                    uac.IsUpdate = true;
                    return uac;
                }
                return base.HisUAC;
            }
        }
        /// <summary>
        /// 方法单据
        /// </summary>
        public MethodBill()
        {
        }
        public MethodBill(string mypk)
        {
            this.No = mypk;
            this.Retrieve();
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

                Map map = new Map("Frm_Method", "实体单据");

                //主键.
                map.AddTBStringPK(MethodAttr.No, null, "编号", true, true, 0, 50, 10);
                map.AddTBString(MethodAttr.Name, null, "方法名", true, false, 0, 300, 10);
                map.AddTBString(MethodAttr.MethodID, null, "方法ID", true, true, 0, 300, 10);
                map.AddTBString(MethodAttr.GroupID, null, "分组ID", true, true, 0, 50, 10);

                //功能标记.
                map.AddTBString(MethodAttr.MethodModel, null, "方法模式", true, true, 0, 300, 10);
                map.AddTBString(MethodAttr.Tag1, null, "Tag1", true, true, 0, 300, 10);
                map.AddTBString(MethodAttr.Mark, null, "Mark", true, true, 0, 300, 10);


                map.AddTBString(MethodAttr.FrmID, null, "表单ID", true, true, 0, 300, 10);
                map.AddTBString(MethodAttr.FlowNo, null, "流程编号", true, true, 0, 10, 10);

                map.AddTBString(MethodAttr.Icon, null, "图标", true, false, 0, 50, 10, true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 新建工作
        /// </summary>
        /// <param name="workid">实体的实例ID</param>
        /// <returns></returns>
        public string CreateWorkID(Int64 workid)
        {
            //获得当前的实体.
            GEEntity ge = new GEEntity(this.FrmID, workid);

            //创建单据ID.
            Int64 workID = BP.CCBill.Dev2Interface.CreateBlankBillID(this.Tag1, null, ge.Row, this.FrmID, workid);
            return workID.ToString();
        }

        #region 执行方法.
        protected override bool beforeInsert()
        {
            if (DataType.IsNullOrEmpty(this.No) == true)
                this.No = DBAccess.GenerGUID();
            return base.beforeInsert();
        }
        #endregion 执行方法.

    }
    /// <summary>
    /// 方法单据
    /// </summary>
    public class MethodBills : EntitiesNoName
    {
        /// <summary>
        /// 方法单据
        /// </summary>
        public MethodBills() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MethodBill();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MethodBill> ToJavaList()
        {
            return (System.Collections.Generic.IList<MethodBill>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MethodBill> Tolist()
        {
            System.Collections.Generic.List<MethodBill> list = new System.Collections.Generic.List<MethodBill>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MethodBill)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
