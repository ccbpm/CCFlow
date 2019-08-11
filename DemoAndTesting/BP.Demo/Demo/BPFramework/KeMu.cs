using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo.BPFramework
{
	/// <summary>
	/// 科目 属性
	/// </summary>
	public class KeMuAttr: EntityNoNameAttr
	{

	}
	/// <summary>
    /// 科目
	/// </summary>
	public class KeMu :BP.En.EntityNoName
	{	

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
		/// 科目
		/// </summary>		
		public KeMu(){}
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

				Map map = new Map("Demo_KeMu","科目");

				#region 基本属性 
				map.DepositaryOfEntity=Depositary.None;  //实体村放位置.
                map.IsAllowRepeatName = true;
				map.EnType=EnType.App;
				map.CodeStruct="3"; //让其编号为3位, 从001 到 999 .
				#endregion

				#region 字段 
                map.AddTBStringPK(KeMuAttr.No, null, "编号", true, true, 3, 3, 50);
				map.AddTBString(KeMuAttr.Name,null,"名称",true,false,0,50,200);
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
	/// 科目s
	/// </summary>
	public class KeMus : BP.En.EntitiesNoName
	{
		#region 重写
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new KeMu();
			}
		}
		#endregion

		#region 构造方法
		/// <summary>
		/// 科目s
		/// </summary>
		public KeMus(){}
		#endregion
	}
	
}
