using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.En;
using System.Collections;
using BP.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 接受人规则属性
    /// </summary>
    public class AccepterRoleAttr:BP.En.EntityOIDNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 节点编号
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 模式类型
        /// </summary>
        public const string FK_ModeSort = "FK_ModeSort";
        /// <summary>
        /// 模式
        /// </summary>
        public const string FK_Mode = "FK_Mode";

        public const string Tag0 = "Tag0";
        public const string Tag1 = "Tag1";
        public const string Tag2 = "Tag2";
        public const string Tag3 = "Tag3";
        public const string Tag4 = "Tag4";
        public const string Tag5 = "Tag5";
        #endregion
    }
    /// <summary>
    /// 这里存放每个接受人规则的信息.	 
    /// </summary>
    public class AccepterRole : EntityOID
    {
        #region 基本属性
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsUpdate = true;
                return uac;
            }
        }
        /// <summary>
        /// 节点编号
        /// </summary>
        public string FK_Node
        {
            get
            {
                return this.GetValStringByKey(AccepterRoleAttr.FK_Node);
            }
            set
            {
                SetValByKey(AccepterRoleAttr.FK_Node, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 接受人规则
        /// </summary>
        public AccepterRole() { }
        /// <summary>
        /// 接受人规则
        /// </summary>
        /// <param name="oid">接受人规则ID</param>	
        public AccepterRole(int oid)
        {
            this.OID = oid;
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

                Map map = new Map("WF_AccepterRole", "接受人规则");

                map.AddTBIntPKOID();

                map.AddTBString(AccepterRoleAttr.Name, null, null, true, false, 0, 200, 10, true);
                map.AddTBString(AccepterRoleAttr.FK_Node, null, "节点", false, true, 0, 100, 10);
                map.AddTBInt(AccepterRoleAttr.FK_Mode, 0, "模式类型", false, true);

                map.AddTBString(AccepterRoleAttr.Tag0, null, "Tag0", false, true, 0, 999, 10);
                map.AddTBString(AccepterRoleAttr.Tag1, null, "Tag1", false, true, 0, 999, 10);
                map.AddTBString(AccepterRoleAttr.Tag2, null, "Tag2", false, true, 0, 999, 10);
                map.AddTBString(AccepterRoleAttr.Tag3, null, "Tag3", false, true, 0, 999, 10);
                map.AddTBString(AccepterRoleAttr.Tag4, null, "Tag4", false, true, 0, 999, 10);
                map.AddTBString(AccepterRoleAttr.Tag5, null, "Tag5", false, true, 0, 999, 10);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
       
    }
    /// <summary>
    /// 接受人规则集合
    /// </summary>
    public class AccepterRoles : Entities
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new AccepterRole();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 接受人规则集合
        /// </summary>
        public AccepterRoles()
        {
        }
        /// <summary>
        /// 接受人规则集合.
        /// </summary>
        /// <param name="FlowNo"></param>
        public AccepterRoles(string FK_Node)
        {
            this.Retrieve(AccepterRoleAttr.FK_Node, FK_Node);
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<AccepterRole> ToJavaList()
        {
            return (System.Collections.Generic.IList<AccepterRole>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<AccepterRole> Tolist()
        {
            System.Collections.Generic.List<AccepterRole> list = new System.Collections.Generic.List<AccepterRole>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((AccepterRole)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
