using System;

namespace BP.DTS
{
	/// <summary>
	/// 运行类型．
	/// </summary>
	public enum RunTimeType
	{
		/// <summary>
		/// 每分钟
		/// </summary>
		Minute,
		/// <summary>
		/// 每小时
		/// </summary>
		Hour,
		/// <summary>
		/// 每天
		/// </summary>
		Day,
		/// <summary>
		/// 每月
		/// </summary>
		Month,		
		/// <summary>
		/// 没有指定
		/// </summary>
		UnName
	}
	/// <summary>
	/// 运行类型
	/// </summary>
    public enum RunType
    {
        /// <summary>
        /// 中间层方法
        /// </summary>
        Method,
        /// <summary>
        /// SQL文本
        /// </summary>
        SQL,
        /// <summary>
        /// 存储过程
        /// </summary>
        SP,
        /// <summary>
        /// 数据调度
        /// </summary>
        DataIO
    }
}
