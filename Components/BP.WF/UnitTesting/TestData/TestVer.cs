using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.WF.UnitTesting
{
    /// <summary>
    /// 测试版本
    /// </summary>
    public class TestVerAttr : EntityNoNameAttr
    {
    }
	/// <summary>
    ///  测试版本
	/// </summary>
	public class TestVer :EntityNoName
    {
		#region 构造方法
		/// <summary>
		/// 测试版本
		/// </summary>
		public TestVer()
        {
        }
        /// <summary>
        /// 测试版本
        /// </summary>
        /// <param name="_No"></param>
        public TestVer(string _No) : base(_No) { }
		#endregion 

		/// <summary>
		/// 测试版本Map
		/// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_TestVer");
                map.EnDesc = "测试版本";

                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap( Depositary.Application);

                map.AddTBStringPK(TestVerAttr.No, null, "编号", true, false, 1, 92, 2);
                map.AddTBString(TestVerAttr.Name, null, "名称", true, false, 1, 50, 20);
                this._enMap = map;
                return this._enMap;
            }
        }
	}
	/// <summary>
    /// 测试版本
	/// </summary>
    public class TestVers : EntitiesNoName
	{
		/// <summary>
		/// 测试版本s
		/// </summary>
        public  TestVers() { }
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
                return new TestVer();
			}
		}

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<TestVer> ToJavaList()
        {
            return (System.Collections.Generic.IList<TestVer>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<TestVer> Tolist()
        {
            System.Collections.Generic.List<TestVer> list = new System.Collections.Generic.List<TestVer>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((TestVer)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

	}
}
