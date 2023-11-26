using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.CN
{
	/// <summary>
	/// ������
	/// </summary>
    public class QXSAttr : EntityNoNameAttr
    {
        #region ��������
        public const string FK_PQ = "FK_PQ";
        public const string FK_SF = "FK_SF";
        public const string NameS = "NameS";
        #endregion
    }
	/// <summary>
    /// ������
	/// </summary>
	public class QXS :EntityNoName
	{	
		#region ��������
        public string NameS
        {
            get
            {
                return this.GetValStrByKey(QXSAttr.NameS);
            }
        }
        public string FK_PQ
        {
            get
            {
                return this.GetValStrByKey(QXSAttr.FK_PQ);
            }
        }
        public string FK_SF
        {
            get
            {
                return this.GetValStrByKey(QXSAttr.FK_SF);
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
		/// ������
		/// </summary>		
		public QXS(){}
		public QXS(string no):base(no)
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
                map.PhysicsTable = "CN_QXS";
                map.AdjunctType = AdjunctType.AllType;
                map.DepositaryOfMap = Depositary.Application;
                map.DepositaryOfEntity = Depositary.None;
                map.IsCheckNoLength = false;
                map.EnDesc = "������";
                map.EnType = EnType.App;
                map.CodeStruct = "4";
                #endregion

                #region �ֶ�
                map.AddTBStringPK(QXSAttr.No, null, "���", true, false, 0, 50, 50);
                map.AddTBString(QXSAttr.Name, null, "����", true, false, 0, 50, 200);
                map.AddTBString(QXSAttr.NameS, null, "NameS", true, false, 0, 50, 200);


                map.AddDDLEntities(QXSAttr.FK_SF, null, "ʡ��", new SFs(), true);
                map.AddDDLEntities(QXSAttr.FK_PQ, null, "Ƭ��", new PQs(), true);

                map.AddSearchAttr(QXSAttr.FK_SF);
                #endregion

                this._enMap = map;
                return this._enMap;
            }
		}
		#endregion

        /// <summary>
        /// ��ȡһ���ַ������Ƿ�����������ƣ���������ͷ������ı�ţ��������ͷ���Ĭ�ϵ�ֵ��
        /// </summary>
        /// <param name="name">�ִ�</param>
        /// <param name="defVal">Ĭ��ֵ</param>
        /// <returns>���ش���</returns>
        public static string GenerQXSNoByName(string name, string defVal)
        {
            //����ģ��ƥ�������
            QXSs qxss = new QXSs();
            qxss.RetrieveAll();

            foreach (QXS qxs in qxss)
            {
                if (name.Contains(qxs.NameS))
                    return qxs.No;
            }

            SFs sfs = new SFs();
            sfs.RetrieveAll();
            foreach (SF sf in sfs)
            {
                if (name.Contains(sf.Names))
                    return sf.No;
            }

            return defVal;
        }
	}
	/// <summary>
	/// ������
	/// </summary>
	public class QXSs : EntitiesNoName
	{
		#region 
		/// <summary>
		/// �õ����� Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new QXS();
			}
		}	
		#endregion 

		#region ���췽��
		/// <summary>
		/// ������s
		/// </summary>
		public QXSs(){}

        /// <summary>
        /// ������s
        /// </summary>
        /// <param name="sf">ʡ��</param>
        public QXSs(string sf)
        {
            this.Retrieve(QXSAttr.FK_SF, sf);
        }
		#endregion
	}
	
}
