using System;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.GPM
{
	/// <summary>
	/// ����Ա
	/// </summary>
	public class ByEmpAttr  
	{
		#region ��������
		/// <summary>
		/// ���ƶ���ID
		/// </summary>
		public const  string RefObj="RefObj";
		/// <summary>
		/// ��Ա
		/// </summary>
		public const  string FK_Emp="FK_Emp";		 
		#endregion	
	}
	/// <summary>
    /// ����Ա
	/// </summary>
    public class ByEmp : Entity
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
        /// ���ƶ���ID
        /// </summary>
        public string RefObj
        {
            get
            {
                return this.GetValStringByKey(ByEmpAttr.RefObj);
            }
            set
            {
                SetValByKey(ByEmpAttr.RefObj, value);
            }
        }
        public string FK_EmpT
        {
            get
            {
                return this.GetValRefTextByKey(ByEmpAttr.FK_Emp);
            }
        }
        /// <summary>
        ///��Ա
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(ByEmpAttr.FK_Emp);
            }
            set
            {
                SetValByKey(ByEmpAttr.FK_Emp, value);
            }
        }
        #endregion

        #region ���캯��
        /// <summary>
        /// ����Ա
        /// </summary> 
        public ByEmp() { }
        /// <summary>
        /// ����Ա
        /// </summary>
        /// <param name="RefObj"></param>
        /// <param name="fk_Emp"></param>
        public ByEmp(string RefObj, string fk_Emp)
        {
            this.RefObj = RefObj;
            this.FK_Emp = fk_Emp;
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

                Map map = new Map("GPM_ByEmp");
                map.EnDesc = "����Ա";
                map.EnType = EnType.Dot2Dot;

                map.AddTBStringPK(ByEmpAttr.RefObj, null, "���ƶ���", false, false, 1, 15, 1);
                map.AddDDLEntitiesPK(ByEmpAttr.FK_Emp, null, "��Ա", new Emps(), true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
    /// ����Ա 
	/// </summary>
	public class ByEmps : Entities
	{
		#region ����
		/// <summary>
        /// ����Ա
		/// </summary>
		public ByEmps()
		{
		}
		/// <summary>
        /// ����Աs
		/// </summary>
		public ByEmps(string EmpNo)
		{
			QueryObject qo = new QueryObject(this);
			qo.AddWhere(ByEmpAttr.FK_Emp, EmpNo);
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
				return new ByEmp();
			}
		}	
		#endregion 

        #region Ϊ����Ӧ�Զ������java����Ҫ,��ʵ��ת����List.
        /// <summary>
        /// ת���� java list,C#���ܵ���.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<ByEmp> ToJavaList()
        {
            return (System.Collections.Generic.IList<ByEmp>)this;
        }
        /// <summary>
        /// ת����list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<ByEmp> Tolist()
        {
            System.Collections.Generic.List<ByEmp> list = new System.Collections.Generic.List<ByEmp>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((ByEmp)this[i]);
            }
            return list;
        }
        #endregion Ϊ����Ӧ�Զ������java����Ҫ,��ʵ��ת����List.
	}
}
