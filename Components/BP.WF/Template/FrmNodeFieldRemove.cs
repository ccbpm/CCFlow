using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.WF.Template
{
    /// <summary>
    /// 表单节点排除规则属性
    /// </summary>
    public class FrmNodeFieldRemoveAttr
    {
        /// <summary>
        /// 表单ID
        /// </summary>
        public const string FrmID = "FrmID";
        /// <summary>
        /// 流程
        /// </summary>
        public const string FlowNo = "FlowNo";
        /// <summary>
        /// 节点
        /// </summary>
        public const string NodeID = "NodeID";
        /// <summary>
        /// 要隐藏的字段s
        /// </summary>
        public const string Fields = "Fields";
        /// <summary>
        ///  表达式类型: Emps,Depts,Stas,SQL
        /// </summary>
        public const string ExpType = "ExpType";
        /// <summary>
        /// 组织结构
        /// </summary>
        public const string Exp = "Exp";
    }
    /// <summary>
    /// 表单节点排除规则
    /// </summary>
    public class FrmNodeFieldRemove : EntityMyPK
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
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FlowNo
        {
            get
            {
                return this.GetValStringByKey(FrmNodeFieldRemoveAttr.FlowNo);
            }
            set
            {
                this.SetValByKey(FrmNodeFieldRemoveAttr.FlowNo, value);
            }
        }
        public string FrmID
        {
            get
            {
                return this.GetValStringByKey(FrmNodeFieldRemoveAttr.FrmID);
            }
            set
            {
                this.SetValByKey(FrmNodeFieldRemoveAttr.FrmID, value);
            }
        }
        public string Fields
        {
            get
            {
                return this.GetValStringByKey(FrmNodeFieldRemoveAttr.Fields);
            }
            set
            {
                this.SetValByKey(FrmNodeFieldRemoveAttr.Fields, value);
            }
        }
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(FrmNodeFieldRemoveAttr.NodeID);
            }
            set
            {
                this.SetValByKey(FrmNodeFieldRemoveAttr.NodeID, value);
            }
        }
        #endregion 基本属性

        #region 构造方法
        /// <summary>
        /// 表单节点排除规则
        /// </summary>
        public FrmNodeFieldRemove() { }
        /// <summary>
        /// 表单节点排除规则
        /// </summary>
        /// <param name="mypk"></param>
        public FrmNodeFieldRemove(string mypk)
            : base(mypk)
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

                Map map = new Map("WF_FrmNodeFieldRemove", "表单节点排除规则");

                map.AddMyPK();
                map.AddTBString(FrmNodeFieldRemoveAttr.FrmID, null, "表单ID", true, true, 1, 200, 200);
                map.AddTBInt(FrmNodeFieldRemoveAttr.NodeID, 0, "节点编号", true, true);
                map.AddTBString(FrmNodeFieldRemoveAttr.FlowNo, null, "流程编号", true, true, 1, 10, 20);

                map.AddTBString(FrmNodeFieldRemoveAttr.Fields, null, "字段", false, false, 0, 50, 10, false);
                map.AddTBString(FrmNodeFieldRemoveAttr.ExpType, null, "表达式类型", false, false, 0, 50, 10, false);
                map.SetHelperAlert(FrmNodeFieldRemoveAttr.ExpType, "类型: Stas,Emps,Depts,SQL");
                map.AddTBString(FrmNodeFieldRemoveAttr.Exp, null, "表达式", false, false, 0, 500, 10, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

    }
    /// <summary>
    /// 表单节点排除规则s
    /// </summary>
    public class FrmNodeFieldRemoves : EntitiesMyPK
    {
        #region 构造方法..
        /// <summary>
        /// 表单节点排除规则
        /// </summary>
        public FrmNodeFieldRemoves() { }
        #endregion 构造方法..

        #region 公共方法.
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmNodeFieldRemove();
            }
        }
        #endregion 公共方法.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmNodeFieldRemove> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmNodeFieldRemove>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmNodeFieldRemove> Tolist()
        {
            System.Collections.Generic.List<FrmNodeFieldRemove> list = new System.Collections.Generic.List<FrmNodeFieldRemove>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmNodeFieldRemove)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

    }
}
