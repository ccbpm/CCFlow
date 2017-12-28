using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo.BPFramework
{
	/// <summary>
	/// ��Ŀ ����
	/// </summary>
	public class KeMuAttr: EntityNoNameAttr
	{
	}
	/// <summary>
    /// ��Ŀ
	/// </summary>
	public class KeMu :EntityNoName
	{	
		#region ��������
		#endregion 

		#region ���캯��
        /// <summary>
        /// ʵ���Ȩ�޿���
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsDelete = true;
                uac.IsUpdate = true;
                uac.IsInsert = true;
                return uac;
            }
        }
		/// <summary>
		/// ��Ŀ
		/// </summary>		
		public KeMu(){}
        /// <summary>
        /// ��Ŀ
        /// </summary>
        /// <param name="no">���</param>
		public KeMu(string no):base(no)
		{
		}
		/// <summary>
		/// Map
		/// </summary>
		public override Map EnMap
		{
			get
			{
				if (this._enMap!=null) 
					return this._enMap;
                Map map = new Map("Demo_KeMu", "��Ŀ");

				#region �������� 
				map.EnDBUrl =new DBUrl(DBUrlType.AppCenterDSN) ; 
				map.DepositaryOfEntity=Depositary.None;  //ʵ����λ��.
                map.IsAllowRepeatName = true;
				map.IsCheckNoLength=false;
				map.EnType=EnType.App;
				map.CodeStruct="3"; //������Ϊ3λ, ��001 �� 999 .
				#endregion

				#region �ֶ� 
                map.AddTBStringPK(KeMuAttr.No, null, "���", true, true, 3, 3, 3);
				map.AddTBString(KeMuAttr.Name,null,"����",true,false,0,50,200);
				#endregion

				this._enMap=map;
				return this._enMap;
			}
		}
        public override Entities GetNewEntities
        {
            get { return new KeMus(); }
        }
		#endregion
		 
	}
	/// <summary>
	/// ��Ŀ
	/// </summary>
	public class KeMus : EntitiesNoName
	{
		#region ��д
		/// <summary>
		/// �õ����� Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new KeMu();
			}
		}	
		#endregion 

		#region ���췽��
		/// <summary>
		/// ��Ŀs
		/// </summary>
		public KeMus(){}
		#endregion
	}
	
}
