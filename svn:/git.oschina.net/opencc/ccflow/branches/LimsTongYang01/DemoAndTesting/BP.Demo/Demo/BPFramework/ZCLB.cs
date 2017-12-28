using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo.BPFramework
{
	/// <summary>
	/// �̶��ʲ���� ����
	/// </summary>
	public class ZCLBAttr: EntityNoNameAttr
	{
        /// <summary>
        /// ������
        /// </summary>
        public const string BZR = "BZR";
	}
	/// <summary>
    /// �̶��ʲ����
	/// </summary>
	public class ZCLB :BP.En.EntityNoName
	{	
		#region ��������
        /// <summary>
        /// ������
        /// </summary>
        public string BZR
        {
            get
            {
                return this.GetValStrByKey(ZCLBAttr.BZR);
            }
            set
            {
                this.SetValByKey(ZCLBAttr.BZR, value);
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
		/// �̶��ʲ����
		/// </summary>		
		public ZCLB(){}
		public ZCLB(string no):base(no)
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
             //   map.EnDBUrl = new DBUrl();
				map.PhysicsTable="Demo_ZCLB";   //��
				map.DepositaryOfEntity=Depositary.None;  //ʵ����λ��.
                map.IsAllowRepeatName = true;
				map.EnDesc="�̶��ʲ����";
				map.EnType=EnType.App;
				map.CodeStruct="3"; //������Ϊ3λ, ��001 �� 999 .
				#endregion

				#region �ֶ� 
                map.AddTBStringPK(ZCLBAttr.No, null, "���", true, true, 3, 3, 3);
				map.AddTBString(ZCLBAttr.Name,null,"����",true,false,0,50,200);
                map.AddTBString(ZCLBAttr.BZR, null, "������", true, false, 0, 50, 200);
				#endregion

				this._enMap=map;
				return this._enMap;
			}
		}
        public override Entities GetNewEntities
        {
            get { return new ZCLBs(); }
        }
		#endregion
	}
	/// <summary>
	/// �̶��ʲ����s
	/// </summary>
	public class ZCLBs : BP.En.EntitiesNoName
	{
		#region ��д
		/// <summary>
		/// �õ����� Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new ZCLB();
			}
		}
		#endregion 

		#region ���췽��
		/// <summary>
		/// �̶��ʲ����s
		/// </summary>
		public ZCLBs(){}
		#endregion
	}
	
}
