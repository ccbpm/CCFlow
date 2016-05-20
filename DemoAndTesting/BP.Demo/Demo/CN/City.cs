using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;
namespace BP.CN
{
	/// <summary>
	/// ���� 
	/// </summary>
    public class CityAttr : EntityNoNameAttr
    {
        #region ��������
        public const string FK_PQ = "FK_PQ";
        public const string FK_SF = "FK_SF";
        public const string Grade = "Grade";
        public const string Names = "Names";
        #endregion
    }
	/// <summary>
    /// ����
	/// </summary>
    public class City : EntityNoName
    {
        #region ��������
        public string Names
        {
            get
            {
                return this.GetValStrByKey(CityAttr.Names);
            }
        }
        public string FK_PQ
        {
            get
            {
                return this.GetValStrByKey(CityAttr.FK_PQ);
            }
        }
        public string FK_SF
        {
            get
            {
                return this.GetValStrByKey(CityAttr.FK_SF);
            }
        }
        #endregion

        #region ���캯��
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
        /// ����
        /// </summary>		
        public City() { }
        public City(string no)
            : base(no)
        {
        }
        /// <summary>
        /// Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map();

                #region ��������
                map.EnDBUrl = new DBUrl(DBUrlType.AppCenterDSN);
                map.PhysicsTable = "CN_City";
                map.AdjunctType = AdjunctType.AllType;
                map.DepositaryOfMap = Depositary.Application;
                map.DepositaryOfEntity = Depositary.None;
                map.IsCheckNoLength = false;
                map.EnDesc = "����";
                map.EnType = EnType.App;
                map.CodeStruct = "4";
                #endregion

                #region �ֶ�
                map.AddTBStringPK(CityAttr.No, null, "���", true, false, 0, 50, 50);
                map.AddTBString(CityAttr.Name, null, "����", true, false, 0, 50, 200);
                map.AddTBString(CityAttr.Names, null, "С��", true, false, 0, 50, 200);
                map.AddTBInt(CityAttr.Grade, 0, "Grade", false, false);

                map.AddDDLEntities(CityAttr.FK_SF, null, "ʡ��", new SFs(), true);
                map.AddDDLEntities(CityAttr.FK_PQ, null, "Ƭ��", new PQs(), true);

                map.AddSearchAttr(CityAttr.FK_SF);
                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

    }
	/// <summary>
	/// ����
	/// </summary>
	public class Citys : EntitiesNoName
	{
		#region 
		/// <summary>
		/// �õ����� Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new City();
			}
		}	
		#endregion 

		#region ���췽��
		/// <summary>
		/// ����s
		/// </summary>
		public Citys(){}

        /// <summary>
        /// ����s
        /// </summary>
        /// <param name="sf">ʡ��</param>
        public Citys(string sf)
        {
            this.Retrieve(CityAttr.FK_SF, sf);
        }
		#endregion
	}
	
}
