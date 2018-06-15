using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo.YS
{
	/// <summary>
	/// 项目属性
	/// </summary>
    public class ProjectAttr
    {
        public const string Doc = "Doc";
        /// <summary>
        /// 节点
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 收听节点
        /// </summary>
        public const string Nodes = "Nodes";
        /// <summary>
        /// 描述
        /// </summary>
        public const string NodesDesc = "NodesDesc";
        /// <summary>
        /// 收听方式
        /// </summary>
        public const string AlertWay = "AlertWay";
        /// <summary>
        /// FZR
        /// </summary>
        public const string FZR = "FZR";
    }
	/// <summary>
	/// 项目
	/// 节点的收听节点有两部分组成.	 
	/// 记录了从一个节点到其他的多个节点.
	/// 也记录了到这个节点的其他的节点.
	/// </summary>
    public class Project : EntityNoName
    {
        #region 基本属性
        /// <summary>
        ///节点
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(ProjectAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(ProjectAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 收听节点
        /// </summary>
        public string Nodes
        {
            get
            {
                return this.GetValStringByKey(ProjectAttr.Nodes);
            }
            set
            {
                this.SetValByKey(ProjectAttr.Nodes, value);
            }
        }
        public string NodesDesc
        {
            get
            {
                return this.GetValStringByKey(ProjectAttr.NodesDesc);
            }
            set
            {
                this.SetValByKey(ProjectAttr.NodesDesc, value);
            }
        }
        public string Doc
        {
            get
            {
                string s= this.GetValStringByKey(ProjectAttr.Doc);
                if (DataType.IsNullOrEmpty(s) == true)
                    s = "";
                return s;
            }
            set
            {
                this.SetValByKey(ProjectAttr.Doc, value);
            }
        }
        public string FZR
        {
            get
            {
                return this.GetValStringByKey(ProjectAttr.FZR);
            }
            set
            {
                this.SetValByKey(ProjectAttr.FZR, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 项目
        /// </summary>
        public Project()
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

                Map map = new Map("Demo_YS_Project", "项目");

                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.IsAutoGenerNo = true;

                map.AddTBStringPK(EmpAttr.No, null, "编号", true, true, 5, 5, 5);
                map.AddTBString(EmpAttr.Name, null, "名称", true, false, 0, 100, 100);
                map.AddTBStringDoc();

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
	/// 项目
	/// </summary>
    public class Projects : EntitiesNoName
    {
        /// <summary>
        /// 项目
        /// </summary>
        public Projects() { }
        /// <summary>
        /// 项目
        /// </summary>
        /// <param name="fk_flow"></param>
        public Projects(string fk_flow)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhereInSQL(ProjectAttr.FK_Node, "SELECT NodeID FROM WF_Node WHERE FK_Flow='" + fk_flow + "'");
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Project();
            }
        }
    }
}
