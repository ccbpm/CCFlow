using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Pub
{
	/// <summary>
	/// 年月
	/// </summary>
	public class NY :SimpleNoNameFix
	{
		#region 实现基本的方方法
		/// <summary>
		/// 物理表
		/// </summary>
		public override string  PhysicsTable
		{
			get
			{
				return "Pub_NY";
			}
		}
		/// <summary>
		/// 描述
		/// </summary>
		public override string  Desc
		{
			get
			{
                return  "年月";// "年月";
			}
		}
		#endregion 

		#region 构造方法
		 
		public NY(){}
		 
		public NY(string _No ): base(_No){}
		
		#endregion 
	}
	/// <summary>
	/// NDs
	/// </summary>
	public class NYs :BP.En.EntitiesNoName
	{
		/// <summary>
		/// 年月集合
		/// </summary>
		public NYs()
		{
		}
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new NY();
			}
		}

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<NY> ToJavaList()
        {
            return (System.Collections.Generic.IList<NY>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<NY> Tolist()
        {
            System.Collections.Generic.List<NY> list = new System.Collections.Generic.List<NY>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((NY)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
