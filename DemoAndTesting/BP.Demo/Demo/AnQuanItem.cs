using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo
{
	/// <summary>
	/// ��ȫ��Ŀ
	/// </summary>
	public class AnQuanItemAttr: EntityNoNameAttr
	{
		#region ��������
		public const  string FK_SF="FK_SF";
        public const string Addr = "Addr";

		#endregion
	}
	/// <summary>
    /// ��ȫ��Ŀ
	/// </summary>
    public class AnQuanItem : EntityNoName
    {
        #region ��������

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
        /// ��ȫ��Ŀ
        /// </summary>		
        public AnQuanItem() { }
        /// <summary>
        /// ��ȫ��Ŀ
        /// </summary>
        /// <param name="no"></param>
        public AnQuanItem(string no)
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
                map.PhysicsTable = "Demo_AnQuanItem";
                map.AdjunctType = AdjunctType.AllType;
                map.DepositaryOfMap = Depositary.Application;
                map.DepositaryOfEntity = Depositary.None;
                map.IsCheckNoLength = false;
                map.EnDesc = "��ȫ��Ŀ";
                map.EnType = EnType.App;
                map.CodeStruct = "2";
                #endregion

                #region �ֶ�
                map.AddTBStringPK(AnQuanItemAttr.No, null, "���", true, false, 2, 2, 50);
                map.AddTBString(AnQuanItemAttr.Name, null, "����", true, false, 0, 100, 200);
                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        public override Entities GetNewEntities
        {
            get { return new AnQuanItems(); }
        }
        #endregion
    }
	/// <summary>
	/// ��ȫ��Ŀ
	/// </summary>
	public class AnQuanItems : EntitiesNoName
	{
		#region 
		/// <summary>
		/// �õ����� Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new AnQuanItem();
			}
		}	
		#endregion 

		#region ���췽��
		/// <summary>
		/// ��ȫ��Ŀs
		/// </summary>
		public AnQuanItems(){}
		#endregion
	}
	
}
