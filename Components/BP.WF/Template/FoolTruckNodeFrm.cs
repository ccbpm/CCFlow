using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.WF.Template
{
    /// <summary>
    /// 累加表单方案
    /// </summary>
    public class FoolTruckNodeFrm : EntityMyPK
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
                uac.IsDelete = false;
                uac.IsInsert = false;
                return uac;
            }
        }
        /// <summary>
        ///节点
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(FrmNodeAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FK_Frm
        {
            get
            {
                return this.GetValStringByKey(FrmNodeAttr.FK_Frm);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.FK_Frm, value);
            }
        }
        /// <summary>
        /// 对应的解决方案
        /// 0=默认方案.节点编号= 自定义方案, 1=不可编辑.
        /// </summary>
        public int FrmSln
        {
            get
            {
                return this.GetValIntByKey(FrmNodeAttr.FrmSln);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.FrmSln, value);
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(FrmNodeAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.FK_Flow, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 累加表单方案
        /// </summary>
        public FoolTruckNodeFrm() { }
        /// <summary>
        /// 累加表单方案
        /// </summary>
        /// <param name="mypk"></param>
        public FoolTruckNodeFrm(string mypk)
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

                Map map = new Map("WF_FrmNode", "累加表单方案");
                map.AddMyPK();

                map.AddTBInt(FrmNodeAttr.FK_Node, 0, "要作用的节点ID", true, false);
                map.AddTBString(FrmNodeAttr.FK_Frm, null, "表单ID", true, true, 1, 200, 200);
                map.AddDDLSysEnum(FrmNodeAttr.FrmSln, 0, "表单控制方案", true, true, FrmNodeAttr.FrmSln,
                   "@0=默认方案@1=只读方案@2=自定义方案");

                map.AddTBString(FrmNodeAttr.FK_Flow, null, "流程编号", true, true, 1, 20, 20);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 修改前的操作
        /// </summary>
        /// <returns></returns>
        protected override bool beforeUpdate()
        {
            //表单方案如果是只读或者默认方案时，删除对应的设置的权限
            if (this.FrmSln == 0 || this.FrmSln == 1)
            {
                string sql = "";
                sql += "@DELETE FROM Sys_FrmSln WHERE FK_MapData='" + this.FK_Frm + "' AND FK_Node='"+this.FK_Node+"'";
                sql += "@DELETE FROM Sys_FrmAttachment WHERE FK_MapData='" + this.FK_Frm + "' AND FK_Node='" + this.FK_Node + "'";
                sql += "@DELETE FROM Sys_MapDtl WHERE FK_MapData='" + this.FK_Frm + "' AND FK_Node='" + this.FK_Node + "'";
                DBAccess.RunSQLs(sql);
               
            }
            return base.beforeUpdate();
        }
    }
    /// <summary>
    /// 累加表单方案s
    /// </summary>
    public class FoolTruckNodeFrms : EntitiesMyPK
    {
        #region 构造方法..
        /// <summary>
        /// 累加表单方案
        /// </summary>
        public FoolTruckNodeFrms() { }
        #endregion 构造方法..

        #region 公共方法.
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FoolTruckNodeFrm();
            }
        }
        #endregion 公共方法.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FoolTruckNodeFrm> ToJavaList()
        {
            return (System.Collections.Generic.IList<FoolTruckNodeFrm>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FoolTruckNodeFrm> Tolist()
        {
            System.Collections.Generic.List<FoolTruckNodeFrm> list = new System.Collections.Generic.List<FoolTruckNodeFrm>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FoolTruckNodeFrm)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

    }
}
