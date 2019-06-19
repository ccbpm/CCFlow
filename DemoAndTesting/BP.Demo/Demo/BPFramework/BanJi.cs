using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo.BPFramework
{
	/// <summary>
	/// 班级 属性
	/// </summary>
	public class BanJiAttr: EntityNoNameAttr
	{
        /// <summary>
        /// 班主任
        /// </summary>
        public const string BZR = "BZR";
        public const string Tel = "Tel";

	}
	/// <summary>
    /// 班级
	/// </summary>
	public class BanJi :BP.En.EntityNoName
	{	
		#region 基本属性
        /// <summary>
        /// 班主任
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

		#region 构造函数
        /// <summary>
        /// 实体的权限控制
        /// </summary>
		public override UAC HisUAC
		{
			get
			{
				UAC uac = new UAC();

                if (BP.Web.WebUser.No == "zhoupeng" || BP.Web.WebUser.No == "admin")
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
		/// 班级
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

				Map map = new Map("Demo_BanJi","班级");

				#region 基本属性 
				map.DepositaryOfEntity=Depositary.None;  //实体村放位置.
                map.IsAllowRepeatName = true;
				map.EnType=EnType.App;
				map.CodeStruct="3"; //让其编号为3位, 从001 到 999 .
				#endregion

				#region 字段 
                map.AddTBStringPK(BanJiAttr.No, null, "编号", true, true, 3, 3, 50);
				map.AddTBString(BanJiAttr.Name,null,"名称",true,false,0,50,200);
                map.AddTBString(BanJiAttr.BZR, null, "班主任", true, false, 0, 50, 200);
                map.AddTBString(BanJiAttr.Tel, null, "班主任电话", true, false, 0, 50, 200);

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
	/// 班级s
	/// </summary>
	public class BanJis : BP.En.EntitiesNoName
	{
		#region 重写
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new BanJi();
			}
		}
		#endregion

		#region 构造方法
		/// <summary>
		/// 班级s
		/// </summary>
		public BanJis(){}
		#endregion
	}
	
}
