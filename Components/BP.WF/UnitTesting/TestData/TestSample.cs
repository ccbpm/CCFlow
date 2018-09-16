using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.WF.UnitTesting
{
    /// <summary>
    /// 测试明细
    /// </summary>
    public class TestSampleAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 测试的API
        /// </summary>
        public const string FK_API = "FK_API";
        /// <summary>
        /// 版本
        /// </summary>
        public const string FK_Ver = "FK_Ver";
        /// <summary>
        /// 时间从
        /// </summary>
        public const string DTFrom = "DTFrom";
        /// <summary>
        /// 到
        /// </summary>
        public const string DTTo = "DTTo";
        /// <summary>
        /// 用了多少毫秒
        /// </summary>
        public const string TimeUse = "TimeUse";
        /// <summary>
        /// 每秒跑了多少个？
        /// </summary>
        public const string TimesPerSecond = "TimesPerSecond";
    }
	/// <summary>
    ///  测试明细
	/// </summary>
	public class TestSample :EntityMyPK
    {

        #region 构造方法
        public string FK_API
        {
            get
            {
                return this.GetValStrByKey(TestSampleAttr.FK_API);
            }
            set
            {
                this.SetValByKey(TestSampleAttr.FK_API,value);
            }
        }
        public string FK_Ver
        {
            get
            {
                return this.GetValStrByKey(TestSampleAttr.FK_Ver);
            }
            set
            {
                this.SetValByKey(TestSampleAttr.FK_Ver, value);
            }
        }
        public string Name
        {
            get
            {
                return this.GetValStrByKey(TestSampleAttr.Name);
            }
            set
            {
                this.SetValByKey(TestSampleAttr.Name, value);
            }
        }
        public string DTFrom
        {
            get
            {
                return this.GetValStrByKey(TestSampleAttr.DTFrom);
            }
            set
            {
                this.SetValByKey(TestSampleAttr.DTFrom, value);
            }
        }
        public string DTTo
        {
            get
            {
                return this.GetValStrByKey(TestSampleAttr.DTTo);
            }
            set
            {
                this.SetValByKey(TestSampleAttr.DTTo, value);
            }
        }
        public double TimeUse
        {
            get
            {
                return this.GetValDoubleByKey(TestSampleAttr.TimeUse);
            }
            set
            {
                this.SetValByKey(TestSampleAttr.TimeUse, value);
            }
        }
        public double TimesPerSecond
        {
            get
            {
                return this.GetValDoubleByKey(TestSampleAttr.TimesPerSecond);
            }
            set
            {
                this.SetValByKey(TestSampleAttr.TimesPerSecond, value);
            }
        }
        #endregion 构造方法

        #region 构造方法
        /// <summary>
		/// 测试明细
		/// </summary>
		public TestSample()
        {
        }
        /// <summary>
        /// 测试明细
        /// </summary>
        /// <param name="_No"></param>
        public TestSample(string _No) : base(_No) { }
		#endregion 

		/// <summary>
		/// 测试明细Map
		/// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("WF_TestSample");
                map.EnDesc = "测试明细";
                map.Java_SetCodeStruct("2");

                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap( Depositary.Application);

                map.AddMyPK();

                map.AddTBString(TestSampleAttr.Name, null, "测试名称", true, false, 1, 50, 20);

                map.AddDDLEntities(TestSampleAttr.FK_API, null, "测试的API", new TestAPIs(), false);
                map.AddDDLEntities(TestSampleAttr.FK_Ver, null, "测试的版本", new TestVers(), false);

                map.AddTBDateTime(TestSampleAttr.DTFrom, null, "从", true, false);
                map.AddTBDateTime(TestSampleAttr.DTTo, null, "到", true, false);
                map.AddTBFloat(TestSampleAttr.TimeUse, 0, "用时(毫秒)", true, false);
                map.AddTBFloat(TestSampleAttr.TimesPerSecond, 0, "每秒跑多少个?", true, false);

                map.AddSearchAttr(TestSampleAttr.FK_API);
                this._enMap = map;
                return this._enMap;
            }
        }
	}
	/// <summary>
    /// 测试明细
	/// </summary>
    public class TestSamples : EntitiesMyPK
	{
		/// <summary>
		/// 测试明细s
		/// </summary>
        public  TestSamples() { }
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
                return new TestSample();
			}
		}

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<TestSample> ToJavaList()
        {
            return (System.Collections.Generic.IList<TestSample>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<TestSample> Tolist()
        {
            System.Collections.Generic.List<TestSample> list = new System.Collections.Generic.List<TestSample>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((TestSample)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

	}
}
