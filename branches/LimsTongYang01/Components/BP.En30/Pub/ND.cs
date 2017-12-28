using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Pub
{
	/// <summary>
	/// 年度
	/// </summary>
	public class ND :SimpleNoNameFix
	{
		#region 实现基本的方方法
		/// <summary>
		/// 物理表
		/// </summary>
		public override string  PhysicsTable
		{
			get
			{
				return "Pub_ND";
			}
		}
		/// <summary>
		/// 描述
		/// </summary>
		public override string  Desc
		{
			get
			{
                return "年度";// "年度";
			}
		}
		#endregion 

		#region 构造方法
		public ND()
		{
		}
		public ND(string _No ):base(_No)
		{
		}
		#endregion 
	}
	/// <summary>
	/// NDs
	/// </summary>
	public class NDs :SimpleNoNameFixs
	{
		/// <summary>
		/// 年度集合
		/// </summary>
		public NDs()
		{
		}
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new ND();
			}
		}
        public override int RetrieveAll()
        {
            return base.RetrieveAll();
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<ND> ToJavaList()
        {
            return (System.Collections.Generic.IList<ND>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<ND> Tolist()
        {
            System.Collections.Generic.List<ND> list = new System.Collections.Generic.List<ND>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((ND)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
