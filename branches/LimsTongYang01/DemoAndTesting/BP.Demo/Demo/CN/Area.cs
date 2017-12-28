using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;
namespace BP.CN
{
	/// <summary>
	/// ���б���
	/// </summary>
    public class AreaAttr : EntityNoNameAttr
    {
        #region ��������
        public const string FK_PQ = "FK_PQ";
        public const string FK_SF = "FK_SF";
        public const string Grade = "Grade";
        public const string Names = "Names";
        #endregion
    }
	/// <summary>
    /// ���б���
	/// </summary>
	public class Area :EntityNoName
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
                return this.GetValStrByKey(AreaAttr.FK_PQ);
            }
        }
        public string FK_SF
        {
            get
            {
                return this.GetValStrByKey(AreaAttr.FK_SF);
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
		/// ���б���
		/// </summary>		
		public Area(){}
		public Area(string no):base(no)
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
                map.PhysicsTable = "CN_Area";
                map.AdjunctType = AdjunctType.AllType;
                map.DepositaryOfMap = Depositary.Application;
                map.DepositaryOfEntity = Depositary.None;
                map.IsCheckNoLength = false;
                map.EnDesc = "���б���";
                map.EnType = EnType.App;
                map.CodeStruct = "4";
                #endregion

                #region �ֶ�
                map.AddTBStringPK(AreaAttr.No, null, "���", true, false, 0, 50, 50);
                map.AddTBString(AreaAttr.Name, null, "����", true, false, 0, 50, 200);
                map.AddTBString(AreaAttr.Names, null, "С��", true, false, 0, 50, 200);
                map.AddTBInt(AreaAttr.Grade, 0, "Grade", false, false);

                map.AddDDLEntities(AreaAttr.FK_SF, null, "ʡ��", new SFs(), true);
                map.AddDDLEntities(AreaAttr.FK_PQ, null, "Ƭ��", new PQs(), true);

                map.AddSearchAttr(AreaAttr.FK_SF);
                #endregion

                this._enMap = map;
                return this._enMap;
            }
		}
		#endregion

        public static string GenerAreaNoByName(string name1, string name2, string oldcity)
        {
            string fk_city1 = BP.CN.Area.GenerAreaNoByName(name1 , "");
            string fk_city2 = BP.CN.Area.GenerAreaNoByName(name2 , "");
            string fk_city = null;

            if (fk_city1.Length >= 4)
            {
                fk_city = fk_city1;
            }

            if (fk_city1.Length == 2)
            {
                if (fk_city2.Contains(fk_city1))
                    fk_city = fk_city2;
                else
                    fk_city = fk_city1;
            }
            return fk_city;
        }

        public static string GenerAreaNoByName(string name, string oldcity)
        {
            //����ģ��ƥ��������������ء�
            string sql = "SELECT NO FROM CN_Area WHERE indexof('" + name + "', names ) >0 ORDER BY GRADE DESC ";
            string val = DBAccess.RunSQLReturnString(sql);
            if (val != null)
                return val;
            else
                return oldcity;
        }
	}
	/// <summary>
	/// ���б���
	/// </summary>
    public class Areas : EntitiesNoName
    {
        #region  �õ����� Entity
        /// <summary>
        /// �õ����� Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Area();
            }
        }
        #endregion

        #region ���췽��
        /// <summary>
        /// ���б���s
        /// </summary>
        public Areas() { }
        /// <summary>
        /// ���б���s
        /// </summary>
        /// <param name="sf">ʡ��</param>
        public Areas(string sf)
        {
            this.Retrieve(AreaAttr.FK_SF, sf);
        }
        #endregion
    }
}
