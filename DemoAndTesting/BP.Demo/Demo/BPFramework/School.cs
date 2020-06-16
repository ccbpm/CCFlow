using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo.BPFramework
{
	/// <summary>
	/// ѧУ ����
	/// </summary>
	public class SchoolAttr: EntityNoNameAttr
	{
        /// <summary>
        /// ������
        /// </summary>
        public const string BZR = "BZR";
        public const string Tel = "Tel";

	}
	/// <summary>
    /// ѧУ
	/// </summary>
	public class School :BP.En.EntityNoName
	{	
		#region ��������
        /// <summary>
        /// ������
        /// </summary>
        public string BZR
        {
            get
            {
                return this.GetValStrByKey(SchoolAttr.BZR);
            }
            set
            {
                this.SetValByKey(SchoolAttr.BZR, value);
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

                if (BP.Web.WebUser.No == "zhoupeng" || BP.Web.WebUser.No.Equals("admin")==true)
                {
                    uac.IsDelete = true;
                    uac.IsUpdate = true;
                    uac.IsInsert = true;
                }
                else
                {
                    uac.IsDelete = false;
                    uac.IsUpdate = false;
                    uac.IsInsert = false;
                }
				return uac;
			}
		}
		/// <summary>
		/// ѧУ
		/// </summary>		
		public School(){}
		public School(string no):base(no)
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

				Map map = new Map("Demo_School","ѧУ");

				#region �������� 
				map.DepositaryOfEntity=Depositary.None;  //ʵ����λ��.
                map.IsAllowRepeatName = true;
				map.EnType=EnType.App;
				map.CodeStruct="3"; //������Ϊ3λ, ��001 �� 999 .
				#endregion

				#region �ֶ� 
                map.AddTBStringPK(SchoolAttr.No, null, "���", true, true, 3, 3, 50);
				map.AddTBString(SchoolAttr.Name,null,"����",true,false,0,50,200);
                map.AddTBString(SchoolAttr.BZR, null, "������", true, false, 0, 50, 200);
                map.AddTBString(SchoolAttr.Tel, null, "�����ε绰", true, false, 0, 50, 200);

				#endregion

				this._enMap=map;
				return this._enMap;
			}
		}
        public override Entities GetNewEntities
        {
            get { return new Schools(); }
        }
		#endregion
	}
	/// <summary>
	/// ѧУs
	/// </summary>
	public class Schools : BP.En.EntitiesNoName
	{
		#region ��д
		/// <summary>
		/// �õ����� Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new School();
			}
		}
		#endregion

		#region ���췽��
		/// <summary>
		/// ѧУs
		/// </summary>
		public Schools(){}
		#endregion
	}
	
}
