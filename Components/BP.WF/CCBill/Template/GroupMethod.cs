using System;
using BP.DA;
using BP.En;

namespace BP.CCBill.Template
{
    /// <summary>
    /// 方法类型
    /// </summary>
    public class GroupMethodType
    {
        /// <summary>
        /// 自定义
        /// </summary>
        public const string Self = "Self";
        /// <summary>
        /// 基础资料变更
        /// </summary>
        public const string FlowBaseData = "FlowBaseData";
        /// <summary>
        /// 新创建实体
        /// </summary>
        public const string FlowNewEntity = "FlowNewEntity";
        /// <summary>
        /// 其他业务组件
        /// </summary>
        public const string FlowFlowEtc = "FlowFlowEtc";
    }
    /// <summary>
    /// 分组 - 属性
    /// </summary>
    public class GroupMethodAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 表单ID
        /// </summary>
        public const string FrmID = "FrmID";
        /// <summary>
        /// 顺序
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 方法类型
        /// </summary>
        public const string MethodType = "MethodType";
        /// <summary>
        /// 方法ID
        /// </summary>
        public const string MethodID = "MethodID";
        /// <summary>
        /// Icon
        /// </summary>
        public const string Icon = "Icon";
        /// <summary>
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
    }
    /// <summary>
    /// 分组
    /// </summary>
    public class GroupMethod : EntityNoName
    {
        #region 权限控制
        /// <summary>
        /// 权限控制.
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.No.Equals("admin") == true || BP.Web.WebUser.IsAdmin)
                {
                    uac.IsDelete = true;
                    uac.IsInsert = false;
                    uac.IsUpdate = true;
                    return uac;
                }
                uac.Readonly();
                uac.IsView = false;
                return uac;
            }
        }
        #endregion 权限控制

        #region 属性
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStrByKey(GroupMethodAttr.FrmID);
            }
            set
            {
                this.SetValByKey(GroupMethodAttr.FrmID, value);
            }
        }
        public string OrgNo
        {
            get
            {
                return this.GetValStrByKey(GroupMethodAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(GroupMethodAttr.OrgNo, value);
            }
        }
        public string MethodType
        {
            get
            {
                return this.GetValStrByKey(GroupMethodAttr.MethodType);
            }
            set
            {
                this.SetValByKey(GroupMethodAttr.MethodType, value);
            }
        }
        /// <summary>
        /// 顺序号
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(GroupMethodAttr.Idx);
            }
            set
            {
                this.SetValByKey(GroupMethodAttr.Idx, value);
            }
        }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon
        {
            get
            {
                return this.GetValStrByKey(GroupMethodAttr.Icon);
            }
            set
            {
                this.SetValByKey(GroupMethodAttr.Icon, value);
            }
        }
        public string MethodID
        {
            get
            {
                return this.GetValStrByKey(GroupMethodAttr.MethodID);
            }
            set
            {
                this.SetValByKey(GroupMethodAttr.MethodID, value);
            }
        }

        #endregion

        #region 构造方法
        /// <summary>
        /// GroupMethod
        /// </summary>
        public GroupMethod()
        {
        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Frm_GroupMethod", "方法分组");

                #region 字段.
                map.AddTBStringPK(GroupMethodAttr.No, null, "编号", true, true, 0, 150, 20);
                map.AddTBString(GroupMethodAttr.FrmID, null, "表单ID", true, true, 0, 200, 20);

                map.AddTBString(GroupMethodAttr.Name, null, "标签", true, false, 0, 500, 20, true);
                map.AddTBString(GroupMethodAttr.Icon, null, "Icon", true, true, 0, 200, 20, true);

                map.AddTBString(GroupMethodAttr.OrgNo, null, "OrgNo", true, true, 0, 40, 20, true);


                map.AddTBInt(GroupMethodAttr.Idx, 0, "顺序号", true, false);
                map.AddTBAtParas(3000);
                #endregion 字段.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 方法.
        protected override bool beforeDelete()
        {
            Methods ens = new Methods();
            ens.Retrieve(MethodAttr.GroupID, this.No);
            if (ens.Count != 0)
                throw new Exception("err@该目录下有数据，您不能删除目录。");

            return base.beforeDelete();
        }
        protected override bool beforeUpdate()
        {
            return base.beforeUpdate();
        }
        public string DoDown()
        {
            this.DoOrderDown(GroupMethodAttr.FrmID, this.FrmID, GroupMethodAttr.Idx);
            return "执行成功";
        }
        public string DoUp()
        {
            this.DoOrderUp(GroupMethodAttr.FrmID, this.FrmID, GroupMethodAttr.Idx);
            return "执行成功";
        }
        protected override bool beforeInsert()
        {
            //设置主键.
            if (DataType.IsNullOrEmpty(this.No) == true)
                this.No = DBAccess.GenerGUID();

            this.OrgNo = BP.Web.WebUser.OrgNo;

            return base.beforeInsert();
        }
        #endregion 方法.
    }
    /// <summary>
    /// 分组-集合
    /// </summary>
    public class GroupMethods : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 分组s
        /// </summary>
        public GroupMethods()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new GroupMethod();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<GroupMethod> ToJavaList()
        {
            return (System.Collections.Generic.IList<GroupMethod>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<GroupMethod> Tolist()
        {
            System.Collections.Generic.List<GroupMethod> list = new System.Collections.Generic.List<GroupMethod>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((GroupMethod)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
