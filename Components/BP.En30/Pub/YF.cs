using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Pub
{
	/// <summary>
	/// 月份
	/// </summary>
	public class YF :SimpleNoNameFix
	{
		#region 实现基本的方方法
		 
		/// <summary>
		/// 物理表
		/// </summary>
		public override string  PhysicsTable
		{
			get
			{
				return "Pub_YF";
			}
		}
		/// <summary>
		/// 描述
		/// </summary>
		public override string  Desc
		{
			get
			{
                return  "月份";  // "月份";
			}
		}
		#endregion 

		#region 构造方法
		public YF()
        {
        }
        /// <summary>
        /// _No
        /// </summary>
        /// <param name="_No"></param>
		public YF(string _No ): base(_No)
        {
        }
		#endregion 
	}
	/// <summary>
	/// NDs
	/// </summary>
	public class YFs :SimpleNoNameFixs
	{
		/// <summary>
		/// 月份集合
		/// </summary>
		public YFs()
		{
		}
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new YF();
			}
		}
        public override int RetrieveAll()
        {
            int num= base.RetrieveAll();

            if (num != 12)
            {
                BP.DA.DBAccess.RunSQL("DELETE FROM Pub_YF ");

                for (int i = 1; i <= 12; i++)
                {
                    BP.Pub.YF yf = new YF();
                    yf.No = i.ToString().PadLeft( 2,'0');
                    yf.Name = i.ToString().PadLeft(2, '0');
                    yf.Insert();
                }

               return base.RetrieveAll();
            }
            return 12;
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<YF> ToJavaList()
        {
            return (System.Collections.Generic.IList<YF>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<YF> Tolist()
        {
            System.Collections.Generic.List<YF> list = new System.Collections.Generic.List<YF>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((YF)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
