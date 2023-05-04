using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
using BP.Sys;
using BP.Difference;
namespace BP.TA
{
    /// <summary>
    /// 模板节点属性
    /// </summary>
    public class NodeAttr : EntityTreeAttr
    {
        /// <summary>
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// 模板编号
        /// </summary>
        public const string TemplateNo = "TemplateNo";
        /// <summary>
        /// 域/系统编号
        /// </summary>
        public const string DWayXieZhu = "DWayXieZhu";
        public const string StaNos = "StaNos";
        public const string DeptNos = "DeptNos";
        public const string EmpNos = "EmpNos";

        public const string DWayFZR = "DWayFZR";
        public const string FZRDeptNos = "FZRDeptNos";
        public const string FZRStaNos = "FZRStaNos";
        public const string FZREmpNos = "FZREmpNos";
    }
    /// <summary>
    ///  模板节点
    /// </summary>
    public class Node : EntityTree
    {
        #region 属性.
        /// <summary>
        /// 组织编号
        /// </summary>
        public string FZREmpNos
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.FZREmpNos);
            }
            set
            {
                this.SetValByKey(NodeAttr.FZREmpNos, value);
            }
        }
        public int DWayFZR
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.DWayFZR);
            }
            set
            {
                this.SetValByKey(NodeAttr.DWayFZR, value);
            }
        }
        public string StaNos
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.StaNos);
            }
            set
            {
                this.SetValByKey(NodeAttr.StaNos, value);
            }
        }
        public string DeptNos
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.DeptNos);
            }
            set
            {
                this.SetValByKey(NodeAttr.DeptNos, value);
            }
        }
        public string EmpNos
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.EmpNos);
            }
            set
            {
                this.SetValByKey(NodeAttr.EmpNos, value);
            }
        }
        #endregion 属性.

        #region 构造方法
        /// <summary>
        /// 模板节点
        /// </summary>
        public Node()
        {
        }
        /// <summary>
        /// 模板节点
        /// </summary>
        /// <param name="_No"></param>
        public Node(string _No) : base(_No) { }
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsDelete = false;
                uac.IsInsert = false;
                uac.IsUpdate = true;
                return uac;
            }
        }
        #endregion

        /// <summary>
        /// 模板节点Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("TA_Node", "模板节点");

                map.AddTBStringPK(NodeAttr.No, null, "编号", false, false, 1, 100, 20);
                map.AddTBString(NodeAttr.ParentNo, null, "父节点No", false, false, 0, 100, 30);

                map.AddTBString(NodeAttr.Name, null, "名称", true, false, 0, 200, 30, true);
                map.AddTBString(NodeAttr.TemplateNo, null, "模板编号", true, false, 0, 200, 30, true);

                //负责人生成方式
                map.AddTBInt(NodeAttr.DWayFZR, 0, "负责人生成方式", true, true);
                map.AddTBString(NodeAttr.FZRDeptNos, null, "岗位编号", true, false, 0, 50, 10, false);
                map.AddTBString(NodeAttr.FZRStaNos, null, "岗位编号", true, false, 0, 50, 10, false);
                map.AddTBString(NodeAttr.FZREmpNos, null, "人员编号", true, false, 0, 50, 10, false);

                //协助人生成方式.
                //0=手工选择.1=按岗位2=按部门3=按人员
                map.AddTBInt(NodeAttr.DWayXieZhu, 0, "协助人员生成方式", true, true);
                map.AddTBString(NodeAttr.StaNos, null, "岗位编号", true, false, 0, 50, 10, false);
                map.AddTBString(NodeAttr.DeptNos, null, "部门编号", true, false, 0, 50, 10, false);
                map.AddTBString(NodeAttr.EmpNos, null, "人员编号", true, false, 0, 50, 10, false);

                map.AddTBInt(NodeAttr.Idx, 0, "Idx", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }

        /// <summary>
        /// 生成负责人
        /// </summary>
        /// <returns></returns>
        public string GenerFZR()
        {
            return this.FZREmpNos;
        }

        /// <summary>
        /// 创建的时候，给他增加一个OrgNo。
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            return base.beforeInsert();
        }
        protected override bool beforeUpdate()
        {
            ////更新流程引擎控制表.
            //string sql = "UPDATE WF_GenerWorkFlow SET Domain="" + this.Domain + "" WHERE FK_Node="" + this.No + """;
            //DBAccess.RunSQL(sql);

            //// sql = "UPDATE WF_Flow SET Domain="" + this.Domain + "" WHERE FK_Node="" + this.No + """;
            ////DBAccess.RunSQL(sql);

            //if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
            //    sql = "UPDATE WF_Emp SET StartFlows="" ";
            //else
            //    sql = "UPDATE WF_Emp SET StartFlows="" ";
           // DBAccess.RunSQL(sql);
            return base.beforeUpdate();
        }
        /// <summary>
        /// 删除之前的逻辑
        /// </summary>
        /// <returns></returns>
        protected override bool beforeDelete()
        {
            ////检查是否有流程？
            //Paras ps = new Paras();
            //ps.SQL = "SELECT COUNT(*) FROM TA_Template WHERE Node=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "fk_Node";
            //ps.Add("NodeNo", this.No);
            ////string sql = "SELECT COUNT(*) FROM WF_Flow WHERE FK_Node="" + fk_Node + """;
            //if (DBAccess.RunSQLReturnValInt(ps) != 0)
            //    throw new Exception("err@该目录下有流程，您不能删除。");

            ////检查是否有子目录？
            //ps = new Paras();
            //ps.SQL = "SELECT COUNT(*) FROM WF_Node WHERE ParentNo=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "ParentNo";
            //ps.Add("ParentNo", this.No);
            ////sql = "SELECT COUNT(*) FROM WF_Node WHERE ParentNo="" + fk_Node + """;
            //if (DBAccess.RunSQLReturnValInt(ps) != 0)
            //    throw new Exception("err@该目录下有子目录，您不能删除...");

            return base.beforeDelete();
        }
    }
    /// <summary>
    /// 模板节点
    /// </summary>
    public class Nodes : EntitiesTree
    {
        /// <summary>
        /// 模板节点s
        /// </summary>
        public Nodes() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Node();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Node> ToJavaList()
        {
            return (System.Collections.Generic.IList<Node>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Node> Tolist()
        {
            System.Collections.Generic.List<Node> list = new System.Collections.Generic.List<Node>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Node)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
