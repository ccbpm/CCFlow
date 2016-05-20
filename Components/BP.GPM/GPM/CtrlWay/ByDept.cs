using System;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.GPM
{
	/// <summary>
	/// ������
	/// </summary>
	public class ByDeptAttr  
	{
		#region ��������
		/// <summary>
		/// ���ƶ���
		/// </summary>
		public const  string RefObj="RefObj";
		/// <summary>
		/// ����
		/// </summary>
		public const  string FK_Dept="FK_Dept";		 
		#endregion	
	}
	/// <summary>
    /// ������
	/// </summary>
    public class ByDept : Entity
    {
        #region ����Ȩ�޿���
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        #endregion ����Ȩ�޿���

        #region ��������
        /// <summary>
        /// ���ƶ���
        /// </summary>
        public string RefObj
        {
            get
            {
                return this.GetValStringByKey(ByDeptAttr.RefObj);
            }
            set
            {
                SetValByKey(ByDeptAttr.RefObj, value);
            }
        }
        public string FK_DeptT
        {
            get
            {
                return this.GetValRefTextByKey(ByDeptAttr.FK_Dept);
            }
        }
        /// <summary>
        ///����
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStringByKey(ByDeptAttr.FK_Dept);
            }
            set
            {
                SetValByKey(ByDeptAttr.FK_Dept, value);
            }
        }
        #endregion

        #region ���캯��
        /// <summary>
        /// ������
        /// </summary> 
        public ByDept() { }
        /// <summary>
        /// ������
        /// </summary>
        /// <param name="RefObj"></param>
        /// <param name="fk_Dept"></param>
        public ByDept(string RefObj, string fk_Dept)
        {
            this.RefObj = RefObj;
            this.FK_Dept = fk_Dept;
            if (this.Retrieve() == 0)
                this.Insert();
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

                Map map = new Map("GPM_ByDept");
                map.EnDesc = "������";
                map.EnType = EnType.Dot2Dot;

                map.AddTBStringPK(ByDeptAttr.RefObj, null, "���ƶ���", false, false, 1, 15, 1);
                map.AddDDLEntitiesPK(ByDeptAttr.FK_Dept, null, "����", new Depts(), true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
    /// ������ 
	/// </summary>
	public class ByDepts : Entities
	{
		#region ����
		/// <summary>
        /// ������
		/// </summary>
		public ByDepts()
		{
		}
		/// <summary>
        /// ������s
		/// </summary>
		public ByDepts(string DeptNo)
		{
			QueryObject qo = new QueryObject(this);
			qo.AddWhere(ByDeptAttr.FK_Dept, DeptNo);
			qo.DoQuery();
		}		 
		#endregion

		#region ����
		/// <summary>
		/// �õ����� Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new ByDept();
			}
		}	
		#endregion 

        #region Ϊ����Ӧ�Զ������java����Ҫ,��ʵ��ת����List.
        /// <summary>
        /// ת���� java list,C#���ܵ���.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<ByDept> ToJavaList()
        {
            return (System.Collections.Generic.IList<ByDept>)this;
        }
        /// <summary>
        /// ת����list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<ByDept> Tolist()
        {
            System.Collections.Generic.List<ByDept> list = new System.Collections.Generic.List<ByDept>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((ByDept)this[i]);
            }
            return list;
        }
        #endregion Ϊ����Ӧ�Զ������java����Ҫ,��ʵ��ת����List.
	}
}
