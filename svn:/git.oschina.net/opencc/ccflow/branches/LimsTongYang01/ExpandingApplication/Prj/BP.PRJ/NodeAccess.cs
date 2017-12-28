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
    /// ���̸�λ��������	  
    /// </summary>
    public class NodeAccessAttr
    {
        /// <summary>
        /// �ļ�����
        /// </summary>
        public const string FileName = "FileName";
        /// <summary>
        /// ȫ��
        /// </summary>
        public const string FileFullName = "FileFullName";
        /// <summary>
        /// �����ڵ�
        /// </summary>
        public const string FK_Node = "FK_Node";

        /// <summary>
        /// �Ƿ�ɼ�
        /// </summary>
        public const string IsView = "IsView";
        /// <summary>
        /// �Ƿ���ϴ�
        /// </summary>
        public const string IsUpload = "IsUpload";
        /// <summary>
        /// �Ƿ������
        /// </summary>
        public const string IsDown = "IsDown";
        /// <summary>
        /// �Ƿ��ɾ��
        /// </summary>
        public const string IsDelete = "IsDelete";
        /// <summary>
        /// ��Ŀ
        /// </summary>
        public const string FK_Prj = "FK_Prj";

    }
    /// <summary>
    /// ���̸�λ����
    /// �ڵ�Ĺ����ڵ������������.	 
    /// ��¼�˴�һ���ڵ㵽�����Ķ���ڵ�.
    /// Ҳ��¼�˵�����ڵ�������Ľڵ�.
    /// </summary>
    public class NodeAccess : EntityMyPK
    {
        #region ��������
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
        ///�ڵ�
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
        /// �ļ���
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
        /// �ļ�ȫ��
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

        #region ���췽��
        /// <summary>
        /// ���̸�λ����
        /// </summary>
        public NodeAccess() { }
        /// <summary>
        /// ��д���෽��
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Prj_NodeAccess");
                map.EnDesc = "���ʹ���";

                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.AddMyPK();

                map.AddTBString(NodeAccessAttr.FK_Prj, null, "��Ŀ���", true, true, 0, 200, 20);
                map.AddTBString(NodeAccessAttr.FileName, null, "�ļ���", true, true, 0, 200, 20);
                map.AddTBString(NodeAccessAttr.FileFullName, null, "�ļ�ȫ��", true, true, 0, 200, 20);
                map.AddTBInt(NodeAccessAttr.FK_Node, 0, "�ڵ�", true, true);

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
    /// �ڵ����Ȩ��
    /// </summary>
    public class NodeAccesss : EntitiesMyPK
    {
        /// <summary>
        /// �ڵ����Ȩ��
        /// </summary>
        public NodeAccesss() { }
        /// <summary>
        /// �ڵ����Ȩ��
        /// </summary>
        /// <param name="NodeID">�ڵ�ID</param>
        public NodeAccesss(int NodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(NodeAccessAttr.FileName, NodeID);
            qo.DoQuery();
        }
        /// <summary>
        /// �õ����� Entity 
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
