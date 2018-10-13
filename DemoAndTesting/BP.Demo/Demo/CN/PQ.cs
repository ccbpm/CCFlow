using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.CN
{
	/// <summary>
	/// Ƭ��
	/// </summary>
	public class PQAttr: EntityNoNameAttr
	{
		#region ��������
		public const string FK_SF="FK_SF";
		#endregion
	}
	/// <summary>
    /// Ƭ��
	/// </summary>
	public class PQ :EntityNoName
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
		/// Ƭ��
		/// </summary>		
		public PQ(){}
		public PQ(string no):base(no)
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
				Map map = new Map("CN_PQ","Ƭ��");

				#region �������� 
				map.EnDBUrl =new DBUrl(DBUrlType.AppCenterDSN) ; 
				map.AdjunctType = AdjunctType.AllType ;  
				map.DepositaryOfMap=Depositary.Application; 
				map.DepositaryOfEntity=Depositary.None; 
				map.IsCheckNoLength=false;
				map.EnType=EnType.App;
				map.CodeStruct="4";
				#endregion

				#region �ֶ� 
				map.AddTBStringPK(PQAttr.No,null,"���",true,false,0,50,50);
				map.AddTBString(PQAttr.Name,null,"����",true,false,0,50,200);
				#endregion

				this._enMap=map;
				return this._enMap;
			}
		}
        public override Entities GetNewEntities
        {
            get { return new PQs(); }
        }
		#endregion
	}
	/// <summary>
	/// Ƭ��
	/// </summary>
	public class PQs : EntitiesNoName
	{
		#region 
		/// <summary>
		/// �õ����� Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new PQ();
			}
		}	
		#endregion 

		#region ���췽��
		/// <summary>
		/// Ƭ��s
		/// </summary>
		public PQs(){}
		#endregion
	}
	
}
