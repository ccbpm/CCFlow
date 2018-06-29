using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web;

namespace BP.WF.Port
{
	/// <summary>
	/// 独立组织属性
	/// </summary>
    public class IncAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 父节点编号
        /// </summary>
        public const string ParentNo = "ParentNo";
    }
	/// <summary>
	/// 独立组织
	/// </summary>
	public class Inc:EntityNoName
	{
		#region 属性
        /// <summary>
        /// 父节点编号
        /// </summary>
        public string ParentNo
        {
            get
            {
                return this.GetValStrByKey(IncAttr.ParentNo);
            }
            set
            {
                this.SetValByKey(IncAttr.ParentNo, value);
            }
        }
         
		#endregion

		#region 构造函数
		/// <summary>
		/// 独立组织
		/// </summary>
		public Inc(){}
		/// <summary>
		/// 独立组织
		/// </summary>
		/// <param name="no">编号</param>
        public Inc(string no) : base(no){}
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
                uac.IsDelete = false;
                uac.IsInsert = false;
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

                Map map = new Map("Port_Inc", "独立组织");

                map.Java_SetDepositaryOfEntity(Depositary.Application); //实体map的存放位置.
                map.Java_SetDepositaryOfMap( Depositary.Application);    // Map 的存放位置.

                map.AdjunctType = AdjunctType.None;
                map.EnType = EnType.View; //独立组织是一个视图.

                map.AddTBStringPK(IncAttr.No, null, "编号", true, false, 1, 30, 40);
                map.AddTBString(IncAttr.ParentNo, null, "父节点编号", true, false, 0, 30, 40);
                map.AddTBString(IncAttr.Name, null,"名称", true, false, 0, 60, 200,true);

                RefMethod rm = new RefMethod();
                rm.Title = "设置二级管理员";
                rm.Warning = "设置为子公司后，系统就会在流程树上分配一个目录节点.";
                rm.ClassMethodName = this.ToString() + ".SetSubInc";
                rm.HisAttrs.AddTBString("No", null, "子公司管理员编号", true, false, 0, 100, 100);
                map.AddRefMethod(rm);
                
                this._enMap = map;
                return this._enMap;
            }
		}
		#endregion

        public string SetSubInc(string userNo)
        {
            BP.WF.Port.SubInc.Dept dept = new WF.Port.SubInc.Dept(this.No);
            return dept.SetSubInc(userNo);
        }

	}
	/// <summary>
	///独立组织集合
	/// </summary>
    public class Incs : EntitiesNoName
    {
        /// <summary>
        /// 得到一个新实体
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Inc();
            }
        }
        /// <summary>
        /// create ens
        /// </summary>
        public Incs()
        {
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Inc> ToJavaList()
        {
            return (System.Collections.Generic.IList<Inc>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Inc> Tolist()
        {
            System.Collections.Generic.List<Inc> list = new System.Collections.Generic.List<Inc>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Inc)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
