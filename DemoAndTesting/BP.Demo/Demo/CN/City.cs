using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;
namespace BP.CN
{
	/// <summary>
	/// ��������
	/// </summary>
    public class CityAttr : EntityNoNameAttr
    {
        #region ��������
        public const string FK_PQ = "FK_PQ";
        public const string FK_SF = "FK_SF";
        public const string Grade = "Grade";
        public const string Names = "Names";
        public const string PinYin = "PinYin";
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
            set
            {
                this.SetValByKey(CityAttr.FK_PQ, value);
            }
        }
        public string FK_SF
        {
            get
            {
                return this.GetValStrByKey(CityAttr.FK_SF);
            }
            set
            {
                this.SetValByKey(CityAttr.FK_SF, value);
            }
        }
        public string PinYin
        {
            get
            {
                return this.GetValStrByKey(CityAttr.PinYin);
            }
            set
            {
                this.SetValByKey(CityAttr.PinYin, value);
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
                Map map = new Map("CN_City","����");

                #region ��������
                map.EnDBUrl = new DBUrl(DBUrlType.AppCenterDSN);
                map.AdjunctType = AdjunctType.AllType;
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.IsCheckNoLength = false;
                map.Java_SetEnType(EnType.App);
                map.Java_SetCodeStruct("4");
                #endregion

                #region �ֶ�
                map.AddTBStringPK(CityAttr.No, null, "���", true, false, 0, 50, 50);
                map.AddTBString(CityAttr.Name, null, "����", true, false, 0, 50, 200);
                map.AddTBString(CityAttr.Names, null, "С��", true, false, 0, 50, 200);
                map.AddTBInt(CityAttr.Grade, 0, "Grade", false, false);

                map.AddDDLEntities(CityAttr.FK_SF, null, "ʡ��", new SFs(), true);
                map.AddDDLEntities(CityAttr.FK_PQ, null, "Ƭ��", new PQs(), true);
                map.AddTBString(CityAttr.PinYin, null, "����ƴ��", true, false, 0, 200, 200);

                map.AddTBString(CityAttr.PinYin, null, "ƴ��", true, false, 0, 200, 200);
                
                map.AddSearchAttr(CityAttr.FK_SF);
                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

    }
	/// <summary>
	/// ����s
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
