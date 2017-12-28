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
	public class EmpPrjExtAttr 
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
        /// EmpPrjExt
        /// </summary>
        public const string EmpPrjExt = "EmpPrjExt";
        /// <summary>
        /// Name
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// ��λ����
        /// </summary>
        public const string StationStrs = "StationStrs";
		#endregion	
	}
	/// <summary>
    /// ��Ա��Ŀ�� ��ժҪ˵����
	/// </summary>
    public class EmpPrjExt : EntityMyPK
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
                return this.GetValStringByKey(EmpPrjExtAttr.FK_Emp);
            }
            set
            {
                SetValByKey(EmpPrjExtAttr.FK_Emp, value);
            }
        }
        public string StationStrs
        {
            get
            {
                return this.GetValStringByKey(EmpPrjExtAttr.StationStrs);
            }
            set
            {
                SetValByKey(EmpPrjExtAttr.StationStrs, value);
            }
        }
        
        public string FK_PrjT
        {
            get
            {
                return this.GetValRefTextByKey(EmpPrjExtAttr.FK_Prj);
            }
        }
        /// <summary>
        ///��Ŀ��
        /// </summary>
        public string FK_Prj
        {
            get
            {
                return this.GetValStringByKey(EmpPrjExtAttr.FK_Prj);
            }
            set
            {
                SetValByKey(EmpPrjExtAttr.FK_Prj, value);
            }
        }
        public string Name
        {
            get
            {
                return this.GetValStringByKey(EmpPrjExtAttr.Name);
            }
            set
            {
                SetValByKey(EmpPrjExtAttr.Name, value);
            }
        }
        #endregion

        #region ��չ����

        #endregion

        #region ���캯��
        /// <summary>
        /// ������Ա��Ŀ��
        /// </summary> 
        public EmpPrjExt()
        {
        }
        /// <summary>
        /// ������Ա��Ŀ���Ӧ
        /// </summary>
        /// <param name="_empoid">������ԱID</param>
        /// <param name="wsNo">��Ŀ����</param> 	
        public EmpPrjExt(string _empoid, string wsNo)
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

                map.AddMyPK();
                map.AddTBString(EmpPrjExtAttr.Name, null, "Name", false, false, 0, 3000, 20);

                map.AddDDLEntities(EmpPrjExtAttr.FK_Prj, null, "��Ŀ��", new Prjs(), true);
                map.AddDDLEntities(EmpPrjExtAttr.FK_Emp, null, "��Ա", new BP.WF.Port.WFEmps(), true);


                map.AddTBString(EmpPrjExtAttr.StationStrs, null, "��λ����", true, true, 0, 4000, 20);

                map.AddSearchAttr(EmpPrjExtAttr.FK_Prj);

                map.AttrsOfOneVSM.Add(new EmpPrjStations(), new Stations(),
                    EmpPrjStationAttr.FK_EmpPrj, EmpPrjStationAttr.FK_Station,
                    DeptAttr.Name, DeptAttr.No, "��λ");

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion


        protected override bool beforeUpdate()
        {
            EmpPrjStations ens = new EmpPrjStations();
            ens.Retrieve(EmpPrjStationAttr.FK_EmpPrj, this.MyPK);

            string strs = "";
            foreach (EmpPrjStation en in ens)
            {
                strs += en.FK_StationT + ",";
            }

            this.StationStrs = strs;
            return base.beforeUpdate();
        }
    }
	/// <summary>
    /// ��Ա��Ŀ��
	/// </summary>
    public class EmpPrjExts : Entities
    {
        #region ����
        /// <summary>
        /// ������Ա��Ŀ��
        /// </summary>
        public EmpPrjExts()
        {
        }
        /// <summary>
        /// ������Ա����Ŀ�鼯��
        /// </summary>
        public EmpPrjExts(string GroupNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(EmpPrjExtAttr.FK_Prj, GroupNo);
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
                return new EmpPrjExt();
            }
        }
        #endregion
    }
}
