using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo.BPFramework
{
	/// <summary>
	/// �༶ ����
	/// </summary>
	public class BanJiAttr: EntityNoNameAttr
	{
        /// <summary>
        /// ������
        /// </summary>
        public const string BZR = "BZR";
	}
	/// <summary>
    /// �༶
	/// </summary>
	public class BanJi :EntityNoName
	{	
		#region ��������
        /// <summary>
        /// ������
        /// </summary>
        public string BZR
        {
            get
            {
                return this.GetValStrByKey(BanJiAttr.BZR);
            }
            set
            {
                this.SetValByKey(BanJiAttr.BZR, value);
            }
        }
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
		/// �༶
		/// </summary>		
		public BanJi(){}
		public BanJi(string no):base(no)
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

				Map map = new Map();

				#region �������� 
                map.EnDBUrl = new DBUrl();
				map.PhysicsTable="Demo_BanJi";   //��
				map.DepositaryOfEntity=Depositary.None;  //ʵ����λ��.
                map.IsAllowRepeatName = true;
				map.EnDesc="�༶";
				map.EnType=EnType.App;
				map.CodeStruct="3"; //������Ϊ3λ, ��001 �� 999 .
				#endregion

				#region �ֶ� 
                map.AddTBStringPK(BanJiAttr.No, null, "���", true, true, 3, 3, 3);
				map.AddTBString(BanJiAttr.Name,null,"����",true,false,0,50,200);
                map.AddTBString(BanJiAttr.BZR, null, "������", true, false, 0, 50, 200);

				#endregion

				this._enMap=map;
				return this._enMap;
			}
		}
        public override Entities GetNewEntities
        {
            get { return new BanJis(); }
        }
		#endregion
	}
	/// <summary>
	/// �༶s
	/// </summary>
	public class BanJis : EntitiesNoName
	{
		#region ��д
		/// <summary>
		/// �õ����� Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new BanJi();
			}
		}	
		#endregion 

		#region ���췽��
		/// <summary>
		/// �༶s
		/// </summary>
		public BanJis(){}
		#endregion
	}
	
}
