using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo.BPFramework
{
	/// <summary>
	/// 学校 属性
	/// </summary>
	public class SchoolAttr: EntityNoNameAttr
	{
        /// <summary>
        /// 班主任
        /// </summary>
        public const string BZR = "BZR";
        public const string Tel = "Tel";

	}
	/// <summary>
    /// 学校
	/// </summary>
	public class School :BP.En.EntityNoName
	{	
		#region 基本属性
        /// <summary>
        /// 班主任
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

		#region 构造函数
        /// <summary>
        /// 实体的权限控制
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
		/// 学校
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

				Map map = new Map("Demo_School","学校");

				#region 基本属性 
				map.DepositaryOfEntity=Depositary.None;  //实体村放位置.
                map.IsAllowRepeatName = true;
				map.EnType=EnType.App;
				map.CodeStruct="3"; //让其编号为3位, 从001 到 999 .
				#endregion

				#region 字段 
                map.AddTBStringPK(SchoolAttr.No, null, "编号", true, true, 3, 3, 50);
				map.AddTBString(SchoolAttr.Name,null,"名称",true,false,0,50,200);
                map.AddTBString(SchoolAttr.BZR, null, "班主任", true, false, 0, 50, 200);
                map.AddTBString(SchoolAttr.Tel, null, "班主任电话", true, false, 0, 50, 200);

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
	/// 学校s
	/// </summary>
	public class Schools : BP.En.EntitiesNoName
	{
		#region 重写
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new School();
			}
		}
		#endregion

		#region 构造方法
		/// <summary>
		/// 学校s
		/// </summary>
		public Schools(){}
		#endregion
	}
	
}
