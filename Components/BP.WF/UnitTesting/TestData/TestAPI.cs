using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.WF.UnitTesting
{
    /// <summary>
    /// 测试过程
    /// </summary>
    public class TestAPIAttr : EntityNoNameAttr
    {
    }
	/// <summary>
    ///  测试过程
	/// </summary>
	public class TestAPI :EntityNoName
    {
		#region 构造方法
		/// <summary>
		/// 测试过程
		/// </summary>
		public TestAPI()
        {
        }
        /// <summary>
        /// 测试过程
        /// </summary>
        /// <param name="_No"></param>
        public TestAPI(string _No) : base(_No) { }
		#endregion 

		/// <summary>
		/// 测试过程Map
		/// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_TestAPI");
                map.EnDesc = "测试过程";

                map.AddTBStringPK(TestAPIAttr.No, null, "编号", true, false, 1, 92, 2);
                map.AddTBString(TestAPIAttr.Name, null, "名称", true, false, 1, 50, 20);
                this._enMap = map;
                return this._enMap;
            }
        }
	}
	/// <summary>
    /// 测试过程
	/// </summary>
    public class TestAPIs : EntitiesNoName
	{
		/// <summary>
		/// 测试过程s
		/// </summary>
        public  TestAPIs() { }
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
                return new TestAPI();
			}
		}

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<TestAPI> ToJavaList()
        {
            return (System.Collections.Generic.IList<TestAPI>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<TestAPI> Tolist()
        {
            System.Collections.Generic.List<TestAPI> list = new System.Collections.Generic.List<TestAPI>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((TestAPI)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

	}
}
