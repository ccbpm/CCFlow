using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo.YS
{
	/// <summary>
	/// ��Ŀ����
	/// </summary>
    public class ProjectAttr
    {
        public const string Doc = "Doc";
        /// <summary>
        /// �ڵ�
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// �����ڵ�
        /// </summary>
        public const string Nodes = "Nodes";
        /// <summary>
        /// ����
        /// </summary>
        public const string NodesDesc = "NodesDesc";
        /// <summary>
        /// ������ʽ
        /// </summary>
        public const string AlertWay = "AlertWay";
        /// <summary>
        /// FZR
        /// </summary>
        public const string FZR = "FZR";
    }
	/// <summary>
	/// ��Ŀ
	/// �ڵ�������ڵ������������.	 
	/// ��¼�˴�һ���ڵ㵽�����Ķ���ڵ�.
	/// Ҳ��¼�˵�����ڵ�������Ľڵ�.
	/// </summary>
    public class Project : EntityNoName
    {
        #region ��������
        /// <summary>
        ///�ڵ�
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
        /// �����ڵ�
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

        #region ���췽��
        /// <summary>
        /// ��Ŀ
        /// </summary>
        public Project()
        {

        }
        /// <summary>
        /// ��д���෽��
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Demo_YS_Project", "��Ŀ");

                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.IsAutoGenerNo = true;

                map.AddTBStringPK(EmpAttr.No, null, "���", true, true, 5, 5, 5);
                map.AddTBString(EmpAttr.Name, null, "����", true, false, 0, 100, 100);
                map.AddTBStringDoc();

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
	/// ��Ŀ
	/// </summary>
    public class Projects : EntitiesNoName
    {
        /// <summary>
        /// ��Ŀ
        /// </summary>
        public Projects() { }
        /// <summary>
        /// ��Ŀ
        /// </summary>
        /// <param name="fk_flow"></param>
        public Projects(string fk_flow)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhereInSQL(ProjectAttr.FK_Node, "SELECT NodeID FROM WF_Node WHERE FK_Flow='" + fk_flow + "'");
            qo.DoQuery();
        }
        /// <summary>
        /// �õ����� Entity 
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
