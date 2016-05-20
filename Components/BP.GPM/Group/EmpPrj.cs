using System;
using System.Data;
using BP.DA;
using BP.Port;
using BP.En;

namespace BP.PRJ
{
	/// <summary>
	/// ��Ա��Ŀ��
	/// </summary>
	public class EmpPrjAttr
	{
		#region ��������
		/// <summary>
		/// ������ԱID
		/// </summary>
		public const  string FK_Emp="FK_Emp";
		/// <summary>
		/// ��Ŀ��
		/// </summary>
		public const  string FK_Prj="FK_Prj";
        /// <summary>
        /// MyPK
        /// </summary>
        public const string MyPK = "MyPK";
        /// <summary>
        /// Name
        /// </summary>
        public const string Name = "Name";
		#endregion	
	}
	/// <summary>
    /// ��Ա��Ŀ�� ��ժҪ˵����
	/// </summary>
    public class EmpPrj : Entity
    {
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }

        #region ��������
        /// <summary>
        /// ������ԱID
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(EmpPrjAttr.FK_Emp);
            }
            set
            {
                SetValByKey(EmpPrjAttr.FK_Emp, value);
            }
        }
        public string FK_EmpT
        {
            get
            {
                return this.GetValRefTextByKey(EmpPrjAttr.FK_Emp);
            }
        }
        public string FK_PrjT
        {
            get
            {
                return this.GetValRefTextByKey(EmpPrjAttr.FK_Prj);
            }
        }
        /// <summary>
        ///��Ŀ��
        /// </summary>
        public string FK_Prj
        {
            get
            {
                return this.GetValStringByKey(EmpPrjAttr.FK_Prj);
            }
            set
            {
                SetValByKey(EmpPrjAttr.FK_Prj, value);
            }
        }
        public string MyPK
        {
            get
            {
                return this.GetValStringByKey(EmpPrjAttr.MyPK);
            }
            set
            {
                SetValByKey(EmpPrjAttr.MyPK, value);
            }
        }
        public string Name
        {
            get
            {
                return this.GetValStringByKey(EmpPrjAttr.Name);
            }
            set
            {
                SetValByKey(EmpPrjAttr.Name, value);
            }
        }
        #endregion

        #region ��չ����

        #endregion

        #region ���캯��
        /// <summary>
        /// ������Ա��Ŀ��
        /// </summary> 
        public EmpPrj()
        {
        }
        /// <summary>
        /// ������Ա��Ŀ���Ӧ
        /// </summary>
        /// <param name="_empoid">������ԱID</param>
        /// <param name="wsNo">��Ŀ����</param> 	
        public EmpPrj(string _empoid, string wsNo)
        {
            this.FK_Emp = _empoid;
            this.FK_Prj = wsNo;
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

                Map map = new Map("Prj_EmpPrj");
                map.EnDesc = "��Ա��Ŀ��";
                map.EnType = EnType.Dot2Dot;

                map.AddTBString(EmpPrjAttr.MyPK, null, "MyPK", true, true, 0, 20, 20);
                map.AddTBString(EmpPrjAttr.Name, null, "Name", true, true, 0, 3000, 20);

                map.AddDDLEntitiesPK(EmpPrjAttr.FK_Emp, null, "����Ա", new  Emps(), true);
                map.AddDDLEntitiesPK(EmpPrjAttr.FK_Prj, null, "��Ŀ��", new Prjs(), true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            Emp emp = new  Emp(this.FK_Emp);
            Prj p = new Prj(this.FK_Prj);

            this.MyPK = this.FK_Emp + "-" + this.FK_Prj;
            this.Name = p.Name + " - " + emp.Name;

            return base.beforeInsert();
        }
    }
	/// <summary>
    /// ��Ա��Ŀ��
	/// </summary>
    public class EmpPrjs : Entities
    {
        #region ����
        /// <summary>
        /// ������Ա��Ŀ��
        /// </summary>
        public EmpPrjs()
        {
        }
        /// <summary>
        /// ������Ա����Ŀ�鼯��
        /// </summary>
        public EmpPrjs(string GroupNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(EmpPrjAttr.FK_Prj, GroupNo);
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
                return new EmpPrj();
            }
        }
        #endregion

        #region Ϊ����Ӧ�Զ������java����Ҫ,��ʵ��ת����List.
        /// <summary>
        /// ת���� java list,C#���ܵ���.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<EmpPrj> ToJavaList()
        {
            return (System.Collections.Generic.IList<EmpPrj>)this;
        }
        /// <summary>
        /// ת����list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<EmpPrj> Tolist()
        {
            System.Collections.Generic.List<EmpPrj> list = new System.Collections.Generic.List<EmpPrj>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((EmpPrj)this[i]);
            }
            return list;
        }
        #endregion Ϊ����Ӧ�Զ������java����Ҫ,��ʵ��ת����List.
    }
}
