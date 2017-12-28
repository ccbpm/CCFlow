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
	public class EmpPrjStationAttr
	{
		#region ��������
		/// <summary>
		/// ��Ŀ��Ա
		/// </summary>
        public const string FK_EmpPrj = "FK_EmpPrj";
		/// <summary>
		/// ��Ŀ��
		/// </summary>
		public const  string FK_Station="FK_Station";
        /// <summary>
        /// FK_Emp
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// FK_Prj
        /// </summary>
        public const string FK_Prj = "FK_Prj";
		#endregion
	}
	/// <summary>
    /// ��Ա��Ŀ�� ��ժҪ˵����
	/// </summary>
    public class EmpPrjStation : Entity
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
        public string FK_Prj
        {
            get
            {
                return this.GetValStringByKey(EmpPrjStationAttr.FK_Prj);
            }
            set
            {
                SetValByKey(EmpPrjStationAttr.FK_Prj, value);
            }
        }
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(EmpPrjStationAttr.FK_Emp);
            }
            set
            {
                SetValByKey(EmpPrjStationAttr.FK_Emp, value);
            }
        }
        /// <summary>
        /// ��Ŀ��Ա
        /// </summary>
        public string FK_EmpPrj
        {
            get
            {
                return this.GetValStringByKey(EmpPrjStationAttr.FK_EmpPrj);
            }
            set
            {
                SetValByKey(EmpPrjStationAttr.FK_EmpPrj, value);
            }
        }
        public string FK_StationT
        {
            get
            {
                return this.GetValRefTextByKey(EmpPrjStationAttr.FK_Station);
            }
        }
        /// <summary>
        ///��Ŀ��
        /// </summary>
        public string FK_Station
        {
            get
            {
                return this.GetValStringByKey(EmpPrjStationAttr.FK_Station);
            }
            set
            {
                SetValByKey(EmpPrjStationAttr.FK_Station, value);
            }
        }
        #endregion

        #region ��չ����
        #endregion

        #region ���캯��
        /// <summary>
        /// ������Ա��Ŀ��
        /// </summary> 
        public EmpPrjStation() { }
        /// <summary>
        /// ������Ա��Ŀ���Ӧ
        /// </summary>
        /// <param name="_empoid">��Ŀ��Ա</param>
        /// <param name="wsNo">��Ŀ����</param> 	
        public EmpPrjStation(string _empoid, string wsNo)
        {
            this.FK_EmpPrj = _empoid;
            this.FK_Station = wsNo;
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

                Map map = new Map("Prj_EmpPrjStation");
                map.EnDesc = "��Ա��Ŀ��";
                map.EnType = EnType.Dot2Dot;
                map.AddTBStringPK(EmpPrjStationAttr.FK_EmpPrj, null, "FK_EmpPrj", true, true, 0, 20, 20);
                map.AddDDLEntitiesPK(EmpPrjStationAttr.FK_Station, null, "��λ", new Stations(), true);
                map.AddTBString(EmpPrjStationAttr.FK_Emp, null, "FK_Emp", true, true, 0, 20, 20);
                map.AddTBString(EmpPrjStationAttr.FK_Prj, null, "FK_Prj", true, true, 0, 20, 20);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
        protected override bool beforeInsert()
        {
            string[] strs = this.FK_EmpPrj.Split('-');
            this.FK_Prj = strs[1];
            this.FK_Emp = strs[0];
            return base.beforeInsert();
        }
    }
	/// <summary>
    /// ��Ա��Ŀ��
	/// </summary>
    public class EmpPrjStations : Entities
    {
        #region ����
        /// <summary>
        /// ������Ա��Ŀ��
        /// </summary>
        public EmpPrjStations()
        {
        }
        /// <summary>
        /// ������Ա����Ŀ�鼯��
        /// </summary>
        public EmpPrjStations(string GroupNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(EmpPrjStationAttr.FK_Station, GroupNo);
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
                return new EmpPrjStation();
            }
        }
        #endregion
    }
}
