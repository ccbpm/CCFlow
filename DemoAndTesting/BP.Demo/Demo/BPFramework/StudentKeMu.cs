using System;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.Demo.BPFramework
{
	/// <summary>
	/// 学生科目对应 -属性
	/// </summary>
	public class StudentKeMuAttr  
	{
		#region 基本属性
		/// <summary>
		/// 学生
		/// </summary>
		public const  string FK_Student="FK_Student";
		/// <summary>
		/// 科目
		/// </summary>
		public const  string FK_KeMu="FK_KeMu";		 
		#endregion	
	}
	/// <summary>
    /// 学生科目对应
	/// </summary>
	public class StudentKeMu :EntityMM
	{
		#region 基本属性
		/// <summary>
		/// 学生
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
        /// 科目名称
        /// </summary>
        public string FK_KeMuT
        {
            get
            {
                return this.GetValRefTextByKey(StudentKeMuAttr.FK_KeMu);
            }
        }
		/// <summary>
		///科目
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

		#region 构造函数
		/// <summary>
		/// 学生科目对应
		/// </summary> 
		public StudentKeMu(){}
		/// <summary>
		/// 工作学生科目对应
		/// </summary>
		/// <param name="_empoid">学生</param>
        /// <param name="fk_km">科目编号</param> 	
		public StudentKeMu(string fk_student,string fk_km)
		{
            this.FK_Student = fk_student;
            this.FK_KeMu = fk_km;
            this.Retrieve();
		}
		/// <summary>
		/// 重写基类方法
		/// </summary>
		public override Map EnMap
		{
			get
			{
				if (this._enMap!=null) 
					return this._enMap;

                Map map = new Map("Demo_StudentKeMu", "学生科目对应");
				map.EnType=EnType.Dot2Dot;

                map.AddTBStringPK(StudentKeMuAttr.FK_Student, null, "学生", false, false, 1, 20, 1);
				map.AddDDLEntitiesPK(StudentKeMuAttr.FK_KeMu,null,"科目",new BP.Demo.BPFramework.KeMus(),true);

				this._enMap=map;
				return this._enMap;
			}
		}
		#endregion

		#region 重载基类方法
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
		/// 插入前所做的工作
		/// </summary>
		/// <returns>true/false</returns>
		protected override bool beforeInsert()
		{
			return base.beforeInsert();			
		}
		/// <summary>
		/// 更新前所做的工作
		/// </summary>
		/// <returns>true/false</returns>
		protected override bool beforeUpdate()
		{
			return base.beforeUpdate(); 
		}
		/// <summary>
		/// 删除前所做的工作
		/// </summary>
		/// <returns>true/false</returns>
		protected override bool beforeDelete()
		{
			return base.beforeDelete(); 
		}
		#endregion 
	}
	/// <summary>
    /// 学生科目对应s -集合 
	/// </summary>
	public class StudentKeMus : EntitiesMM
	{
		#region 构造
		/// <summary>
        /// 学生科目对应s
		/// </summary>
		public StudentKeMus(){}
		/// <summary>
        /// 学生科目对应s
		/// </summary>
		/// <param name="FK_Student">FK_Student</param>
		public StudentKeMus(string  FK_Student)
		{
			QueryObject qo = new QueryObject(this);
			qo.AddWhere(StudentKeMuAttr.FK_Student, FK_Student);
			qo.DoQuery();
		}
		#endregion

		#region 重写方法
		/// <summary>
		/// 得到它的 Entity 
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
