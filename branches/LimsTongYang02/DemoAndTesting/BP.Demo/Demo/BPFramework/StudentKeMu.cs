using System;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.Demo.BPFramework
{
	/// <summary>
	/// ѧ����Ŀ��Ӧ -����
	/// </summary>
	public class StudentKeMuAttr  
	{
		#region ��������
		/// <summary>
		/// ѧ��
		/// </summary>
		public const  string FK_Student="FK_Student";
		/// <summary>
		/// ��Ŀ
		/// </summary>
		public const  string FK_KeMu="FK_KeMu";		 
		#endregion	
	}
	/// <summary>
    /// ѧ����Ŀ��Ӧ
	/// </summary>
	public class StudentKeMu :EntityMM
	{
		#region ��������
		/// <summary>
		/// ѧ��
		/// </summary>
		public string FK_Student
		{
			get
			{
				return this.GetValStringByKey(StudentKeMuAttr.FK_Student);
			}
			set
			{
				SetValByKey(StudentKeMuAttr.FK_Student,value);
			}
		}
        /// <summary>
        /// ��Ŀ����
        /// </summary>
        public string FK_KeMuT
        {
            get
            {
                return this.GetValRefTextByKey(StudentKeMuAttr.FK_KeMu);
            }
        }
		/// <summary>
		///��Ŀ
		/// </summary>
		public string FK_KeMu
		{
			get
			{
				return this.GetValStringByKey(StudentKeMuAttr.FK_KeMu);
			}
			set
			{
				SetValByKey(StudentKeMuAttr.FK_KeMu,value);
			}
		}		  
		#endregion

		#region ���캯��
		/// <summary>
		/// ѧ����Ŀ��Ӧ
		/// </summary> 
		public StudentKeMu(){}
		/// <summary>
		/// ����ѧ����Ŀ��Ӧ
		/// </summary>
		/// <param name="_empoid">ѧ��</param>
        /// <param name="fk_km">��Ŀ���</param> 	
		public StudentKeMu(string fk_student,string fk_km)
		{
            this.FK_Student = fk_student;
            this.FK_KeMu = fk_km;
            this.Retrieve();
		}
		/// <summary>
		/// ��д���෽��
		/// </summary>
		public override Map EnMap
		{
			get
			{
				if (this._enMap!=null) 
					return this._enMap;

                Map map = new Map("Demo_StudentKeMu", "ѧ����Ŀ��Ӧ");
				map.EnType=EnType.Dot2Dot;

                map.AddTBStringPK(StudentKeMuAttr.FK_Student, null, "ѧ��", false, false, 1, 20, 1);
				map.AddDDLEntitiesPK(StudentKeMuAttr.FK_KeMu,null,"��Ŀ",new BP.Demo.BPFramework.KeMus(),true);

				this._enMap=map;
				return this._enMap;
			}
		}
		#endregion

		#region ���ػ��෽��
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();

                //if (BP.Web.WebUser.No == "admin")
                //{
                    uac.IsView = true;
                    uac.IsDelete = true;
                    uac.IsInsert = true;
                    uac.IsUpdate = true;
                    uac.IsAdjunct = true;
              //  }
                return uac;
            }
        }
		/// <summary>
		/// ����ǰ�����Ĺ���
		/// </summary>
		/// <returns>true/false</returns>
		protected override bool beforeInsert()
		{
			return base.beforeInsert();			
		}
		/// <summary>
		/// ����ǰ�����Ĺ���
		/// </summary>
		/// <returns>true/false</returns>
		protected override bool beforeUpdate()
		{
			return base.beforeUpdate(); 
		}
		/// <summary>
		/// ɾ��ǰ�����Ĺ���
		/// </summary>
		/// <returns>true/false</returns>
		protected override bool beforeDelete()
		{
			return base.beforeDelete(); 
		}
		#endregion 
	}
	/// <summary>
    /// ѧ����Ŀ��Ӧs -���� 
	/// </summary>
	public class StudentKeMus : EntitiesMM
	{
		#region ����
		/// <summary>
        /// ѧ����Ŀ��Ӧs
		/// </summary>
		public StudentKeMus(){}
		#endregion

		#region ��д����
		/// <summary>
		/// �õ����� Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new StudentKeMu();
			}
		}	
		#endregion 
	}
	
}
