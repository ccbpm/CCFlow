using System;
using System.Collections;
using BP.En;

namespace BP.En
{
    /// <summary>
    /// 工作模式
    /// </summary>
    public enum Dot2DotModel
    {
        /// <summary>
        /// 默认模式
        /// </summary>
        Default,
        /// <summary>
        /// 树模式
        /// </summary>
        TreeDept,
        /// <summary>
        /// 树叶子模式
        /// </summary>
        TreeDeptEmp
    }
	/// <summary>
	/// SearchKey 的摘要说明。
	/// 用来处理一条记录的存放，问题。
	/// </summary>
	public class AttrOfOneVSM 
	{
		#region 基本属性
        /// <summary>
        /// 工作模式
        /// </summary>
        public Dot2DotModel Dot2DotModel = Dot2DotModel.Default;
        /// <summary>
        /// 树
        /// </summary>
        public EntitiesSimpleTree EnsTree = null;
        /// <summary>
        /// 默认的分组key.
        /// </summary>
        public string DefaultGroupAttrKey = null;
        /// <summary>
        /// 树的根节点
        /// </summary>
        public string RootNo = null;
        /// <summary>
        /// 关联的树字段
        /// </summary>
        public string RefTreeAttr =null; 
		/// <summary>
		/// 多对多的实体.
		/// </summary>
		private Entities _ensOfMM=null;
		/// <summary>
		/// 多对多的实体集合
		/// </summary>
		public Entities EnsOfMM
		{
			get
			{
				return _ensOfMM;
			}
			set
			{
				_ensOfMM=value;
			}
		}
		/// <summary>
		/// 多对多的实体
		/// </summary>
		private Entities _ensOfM=null;
		/// <summary>
		/// 多对多的实体集合
		/// </summary>
		public Entities EnsOfM
		{
			get
			{
				return _ensOfM;
			}
			set
			{
				_ensOfM=value;
			}
		}
		/// <summary>
		/// M的实体属性在多对多的实体中
		/// </summary>
		private string _Desc=null;
		/// <summary>
		/// 的实体属性在多对多的实体中
		/// </summary>
		public string Desc
		{
			get
			{
			    return _Desc;//edited by liuxc,2014-10-18 "<font color=blue ><u>" + _Desc + "</u></font>";
			}
			set
			{
				_Desc=value;
			}
		}
		/// <summary>
		/// 一的实体属性在多对多的实体中.
		/// </summary>
		private string _AttrOfOneInMM=null;
		/// <summary>
		/// 一的实体属性在多对多的实体中
		/// </summary>
		public string AttrOfOneInMM
		{
			get
			{
				return _AttrOfOneInMM;
			}
			set
			{
				_AttrOfOneInMM=value;
			}
		}

		/// <summary>
		/// M的实体属性在多对多的实体中
		/// </summary>
		private string _AttrOfMInMM=null;
		/// <summary>
		/// 的实体属性在多对多的实体中
		/// </summary>
		public string AttrOfMInMM
		{
			get
			{
				return _AttrOfMInMM;
			}
			set
			{
				_AttrOfMInMM=value;
			}
		}
		/// <summary>
		/// 标签
		/// </summary>
		private string _AttrOfMText=null;
		/// <summary>
		/// 标签
		/// </summary>
		public string AttrOfMText
		{
			get
			{
				return _AttrOfMText;
			}
			set
			{
				_AttrOfMText=value;
			}
		}
		/// <summary>
		/// Value
		/// </summary>
		private string _AttrOfMValue=null;
		/// <summary>
		/// Value
		/// </summary>
		public string AttrOfMValue
		{
			get
			{
				return _AttrOfMValue;
			}
			set
			{
				_AttrOfMValue=value;
			}
		}
		#endregion

		#region 构造方法
		/// <summary>
		/// AttrOfOneVSM
		/// </summary>
		public AttrOfOneVSM()
		{
        }
		/// <summary>
		/// AttrOfOneVSM
		/// </summary>
		/// <param name="_ensOfMM"></param>
		/// <param name="_ensOfM"></param>
		/// <param name="AttrOfOneInMM"></param>
		/// <param name="AttrOfMInMM"></param>
		/// <param name="AttrOfMText"></param>
		/// <param name="AttrOfMValue"></param>
		public AttrOfOneVSM(Entities _ensOfMM, Entities _ensOfM, string AttrOfOneInMM, string AttrOfMInMM , string AttrOfMText, string AttrOfMValue, string desc)
		{
			this.EnsOfM=_ensOfM;
			this.EnsOfMM=_ensOfMM;
			this.AttrOfOneInMM=AttrOfOneInMM;
			this.AttrOfMInMM=AttrOfMInMM;
			this.AttrOfMText=AttrOfMText;
			this.AttrOfMValue=AttrOfMValue;
			this.Desc=desc;
		}
		#endregion

	}
	/// <summary>
	/// AttrsOfOneVSM 集合
	/// </summary>
	public class AttrsOfOneVSM : System.Collections.CollectionBase
	{
		public AttrsOfOneVSM()
		{
		}
		public AttrOfOneVSM this[int index]
		{
			get
			{
				return (AttrOfOneVSM)this.InnerList[index];
			}
		}
		/// <summary>
		/// 增加一个SearchKey .
		/// </summary>
		/// <param name="r">SearchKey</param>
		public void Add(AttrOfOneVSM attr)
		{
			if (this.IsExits(attr))
				return ;
			this.InnerList.Add(attr);
		}

		/// <summary>
		/// 是不是存在集合里面
		/// </summary>
		/// <param name="en">要检查的EnDtl</param>
		/// <returns>true/false</returns>
		public bool IsExits(AttrOfOneVSM en)
		{
			foreach (AttrOfOneVSM attr in this )
			{
				if (attr.EnsOfMM == en.EnsOfMM  )
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 增加一个属性
		/// </summary>
		/// <param name="_ensOfMM">多对多的实体</param>
		/// <param name="_ensOfM">多实体</param>
		/// <param name="AttrOfOneInMM">点实体,在MM中的属性</param>
		/// <param name="AttrOfMInMM">多实体主键在MM中的属性</param>
		/// <param name="AttrOfMText"></param>
		/// <param name="AttrOfMValue"></param>
		/// <param name="desc">描述</param>
		public void Add(Entities _ensOfMM, Entities _ensOfM, string AttrOfOneInMM, string AttrOfMInMM , string AttrOfMText,
            string AttrOfMValue, string desc, Dot2DotModel model= Dot2DotModel.Default, EntitiesSimpleTree ensTree=null, string refTreeAttr=null)
		{

            //属性.
			AttrOfOneVSM en = new AttrOfOneVSM(_ensOfMM,_ensOfM,AttrOfOneInMM,AttrOfMInMM,AttrOfMText,AttrOfMValue,desc);
            
            //工作模式.
            en.Dot2DotModel = model;
            en.EnsTree = ensTree;
            en.RefTreeAttr = refTreeAttr;
			 
			this.Add(en);				
		}
        /// <summary>
        /// 增加树杆叶子类型
        /// </summary>
        /// <param name="_ensOfMM"></param>
        /// <param name="_ensOfM"></param>
        /// <param name="AttrOfOneInMM"></param>
        /// <param name="AttrOfMInMM"></param>
        /// <param name="desc"></param>
        /// <param name="defaultGroupKey"></param>
        /// <param name="AttrOfMText"></param>
        /// <param name="AttrOfMValue"></param>
        public void AddBranchesAndLeaf(Entities _ensOfMM, Entities _ensOfM, string AttrOfOneInMM, string AttrOfMInMM,
            string desc, string defaultGroupKey = null, string AttrOfMText = "Name", string AttrOfMValue = "No", string rootNo="0")
        {
            //属性.
            AttrOfOneVSM en = new AttrOfOneVSM(_ensOfMM, _ensOfM, AttrOfOneInMM,
                AttrOfMInMM, AttrOfMText, AttrOfMValue, desc);

            //工作模式.
            en.Dot2DotModel = Dot2DotModel.TreeDeptEmp; //分组模式.

            //默认的分组字段，可以是一个类名或者枚举.
            en.DefaultGroupAttrKey = defaultGroupKey;
            en.RootNo = defaultGroupKey;

            this.Add(en);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_ensOfMM"></param>
        /// <param name="_ensOfM"></param>
        /// <param name="AttrOfOneInMM"></param>
        /// <param name="AttrOfMInMM"></param>
        /// <param name="desc">标签或者描述</param>
        /// <param name="AttrOfMText">显示的标签,一般为 Name</param>
        /// <param name="AttrOfMValue">存储的值字段,一般为 No</param>
        /// <param name="defaultGroupKey">默认的分组外键或者枚举,如果为空就不分组.</param>
        public void AddGroupModel(Entities _ensOfMM, Entities _ensOfM, string AttrOfOneInMM, string AttrOfMInMM,
            string desc, string defaultGroupKey = null, string AttrOfMText = "Name", string AttrOfMValue = "No")
        {
            //属性.
            AttrOfOneVSM en = new AttrOfOneVSM(_ensOfMM, _ensOfM, AttrOfOneInMM, AttrOfMInMM, AttrOfMText, AttrOfMValue, desc);

            //工作模式.
            en.Dot2DotModel = Dot2DotModel.Default; //分组模式.

            //默认的分组字段，可以是一个类名或者枚举.
            en.DefaultGroupAttrKey = defaultGroupKey;

            this.Add(en);
        }
		 
	}
}
