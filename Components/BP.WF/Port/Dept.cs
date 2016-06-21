using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web;

namespace BP.WF.Port
{
	/// <summary>
	/// 部门属性
	/// </summary>
    public class DeptAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 父节点编号
        /// </summary>
        public const string ParentNo = "ParentNo";
    }
	/// <summary>
	/// 部门
	/// </summary>
	public class Dept:EntityNoName
	{
		#region 属性
        /// <summary>
        /// 父节点编号
        /// </summary>
        public string ParentNo
        {
            get
            {
                return this.GetValStrByKey(DeptAttr.ParentNo);
            }
            set
            {
                this.SetValByKey(DeptAttr.ParentNo, value);
            }
        }
         
		#endregion

		#region 构造函数
		/// <summary>
		/// 部门
		/// </summary>
		public Dept(){}
		/// <summary>
		/// 部门
		/// </summary>
		/// <param name="no">编号</param>
        public Dept(string no) : base(no){}
		#endregion

		#region 重写方法
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
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
		/// Map
		/// </summary>
		public override Map EnMap
		{
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Port_Dept", "部门");

                map.Java_SetDepositaryOfEntity(Depositary.Application); //实体map的存放位置.
                map.DepositaryOfMap = Depositary.Application;    // Map 的存放位置.

                map.AdjunctType = AdjunctType.None;

                map.AddTBStringPK(DeptAttr.No, null, "编号", true, false, 1, 30, 40);
                map.AddTBString(DeptAttr.Name, null,"名称", true, false, 0, 60, 200);
                map.AddTBString(DeptAttr.ParentNo, null, "父节点编号", true, false, 0, 30, 40);
              //  map.AddTBString(DeptAttr.FK_Unit, "1", "隶属单位", false, false, 0, 50, 10);
                
                this._enMap = map;
                return this._enMap;
            }
		}
		#endregion
	}
	/// <summary>
	///部门集合
	/// </summary>
    public class Depts : EntitiesNoName
    {
        /// <summary>
        /// 查询全部。
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAll()
        {
            if (BP.Web.WebUser.No == "admin")
                return base.RetrieveAll();

            QueryObject qo = new QueryObject(this);
            qo.AddWhere(DeptAttr.No, " = ", BP.Web.WebUser.FK_Dept);
            qo.addOr();
            qo.AddWhere(DeptAttr.ParentNo, " = ", BP.Web.WebUser.FK_Dept);
            return qo.DoQuery();
        }
        /// <summary>
        /// 得到一个新实体
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Dept();
            }
        }
        /// <summary>
        /// create ens
        /// </summary>
        public Depts()
        {
        }
    }
}
