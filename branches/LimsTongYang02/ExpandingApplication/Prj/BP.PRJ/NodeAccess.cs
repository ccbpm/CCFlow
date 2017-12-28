using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.En;
using BP.Port;
using BP.WF;

namespace BP.PRJ
{
    /// <summary>
    /// 流程岗位属性属性	  
    /// </summary>
    public class NodeAccessAttr
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public const string FileName = "FileName";
        /// <summary>
        /// 全名
        /// </summary>
        public const string FileFullName = "FileFullName";
        /// <summary>
        /// 工作节点
        /// </summary>
        public const string FK_Node = "FK_Node";

        /// <summary>
        /// 是否可见
        /// </summary>
        public const string IsView = "IsView";
        /// <summary>
        /// 是否可上传
        /// </summary>
        public const string IsUpload = "IsUpload";
        /// <summary>
        /// 是否可下载
        /// </summary>
        public const string IsDown = "IsDown";
        /// <summary>
        /// 是否可删除
        /// </summary>
        public const string IsDelete = "IsDelete";
        /// <summary>
        /// 项目
        /// </summary>
        public const string FK_Prj = "FK_Prj";

    }
    /// <summary>
    /// 流程岗位属性
    /// 节点的工作节点有两部分组成.	 
    /// 记录了从一个节点到其他的多个节点.
    /// 也记录了到这个节点的其他的节点.
    /// </summary>
    public class NodeAccess : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// HisUAC
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
        ///节点
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(NodeAccessAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(NodeAccessAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName
        {
            get
            {
                return this.GetValStringByKey(NodeAccessAttr.FileName);
            }
            set
            {
                this.SetValByKey(NodeAccessAttr.FileName, value);
            }
        }
        /// <summary>
        /// FK_Prj
        /// </summary>
        public string FK_Prj
        {
            get
            {
                return this.GetValStringByKey(NodeAccessAttr.FK_Prj);
            }
            set
            {
                this.SetValByKey(NodeAccessAttr.FK_Prj, value);
            }
        }
        /// <summary>
        /// 文件全名
        /// </summary>
        public string FileFullName
        {
            get
            {
                return this.GetValStringByKey(NodeAccessAttr.FileFullName);
            }
            set
            {
                this.SetValByKey(NodeAccessAttr.FileFullName, value);
            }
        }

        public bool IsView
        {
            get
            {
                return this.GetValBooleanByKey(NodeAccessAttr.IsView);
            }
            set
            {
                this.SetValByKey(NodeAccessAttr.IsView, value);
            }
        }
        public bool IsUpload
        {
            get
            {
                return this.GetValBooleanByKey(NodeAccessAttr.IsUpload);
            }
            set
            {
                this.SetValByKey(NodeAccessAttr.IsUpload, value);
            }
        }
        public bool IsDown
        {
            get
            {
                return this.GetValBooleanByKey(NodeAccessAttr.IsDown);
            }
            set
            {
                this.SetValByKey(NodeAccessAttr.IsDown, value);
            }
        }
        public bool IsDelete
        {
            get
            {
                return this.GetValBooleanByKey(NodeAccessAttr.IsDelete);
            }
            set
            {
                this.SetValByKey(NodeAccessAttr.IsDelete, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 流程岗位属性
        /// </summary>
        public NodeAccess() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Prj_NodeAccess");
                map.EnDesc = "访问规则";

                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.AddMyPK();

                map.AddTBString(NodeAccessAttr.FK_Prj, null, "项目编号", true, true, 0, 200, 20);
                map.AddTBString(NodeAccessAttr.FileName, null, "文件名", true, true, 0, 200, 20);
                map.AddTBString(NodeAccessAttr.FileFullName, null, "文件全名", true, true, 0, 200, 20);
                map.AddTBInt(NodeAccessAttr.FK_Node, 0, "节点", true, true);

                map.AddTBInt(NodeAccessAttr.IsView, 0, "IsView", true, true);
                map.AddTBInt(NodeAccessAttr.IsUpload, 0, "IsUpload", true, true);
                map.AddTBInt(NodeAccessAttr.IsDown, 0, "IsDown", true, true);
                map.AddTBInt(NodeAccessAttr.IsDelete, 0, "IsDelete", true, true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 节点访问权限
    /// </summary>
    public class NodeAccesss : EntitiesMyPK
    {
        /// <summary>
        /// 节点访问权限
        /// </summary>
        public NodeAccesss() { }
        /// <summary>
        /// 节点访问权限
        /// </summary>
        /// <param name="NodeID">节点ID</param>
        public NodeAccesss(int NodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(NodeAccessAttr.FileName, NodeID);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new NodeAccess();
            }
        }
    }
}
