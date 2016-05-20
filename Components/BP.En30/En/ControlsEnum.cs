using System;
 

namespace BP.Web.Controls
{
	/// <summary>
	/// 按钮类型
	/// </summary>
	public enum BtnType
	{
		/// <summary>
		/// 确认 ,需要给　hit 赋值。 
		/// ＸＸＸ　确认马？
		/// </summary>
		ConfirmHit,
		/// <summary>
		/// 正常
		/// </summary>
		Normal,			
		/// <summary>
		/// 确定
		/// </summary>
		Confirm,
		/// <summary>
		/// 保存
		/// </summary>
		Save,
		/// <summary>
		/// 保存并新建
		/// </summary>
		SaveAndNew,
		/// <summary>
		/// 查找
		/// </summary>
		Search,
		/// <summary>
		/// 取消
		/// </summary>
		Cancel,
		/// <summary>
		/// 删除
		/// </summary>
		Delete,
		/// <summary>
		/// 更新
		/// </summary>
		Update,
		/// <summary>
		/// 插入
		/// </summary>
		Insert,
		/// <summary>
		/// 编辑
		/// </summary>
		Edit,
		/// <summary>
		/// 新建
		/// </summary>
		New,
		/// <summary>
		/// 浏览
		/// </summary>
		View,
		/// <summary>
		/// 关闭
		/// </summary>
		Close,
		/// <summary>
		/// 导出
		/// </summary>
		Export,
		/// <summary>
		/// 打印
		/// </summary>
		Print,
		/// <summary>
		/// 增加
		/// </summary>
		Add,
		/// <summary>
		/// 一处
		/// </summary>
		Reomve,
		/// <summary>
		/// 返回
		/// </summary>
		Back,
		/// <summary>
		/// 刷新
		/// </summary>
		Refurbish,
		/// <summary>
		/// 申请任务
		/// </summary>
		ApplyTask,
		/// <summary>
		/// 选者全部
		/// </summary>
		SelectAll,
		/// <summary>
		/// 全不选
		/// </summary>
		SelectNone
	} 
	/// <summary>
	/// TaxBox 类型
	/// </summary>
	public enum TBType
	{
		/// <summary>
		/// Entities 的DataHelp, 如果这里说明了，他是Ens , 就要指明DataHelpKey. 
		/// 这样，系统就会在右键帮助会出现他。
		/// </summary>
		Ens,
		/// <summary>
		/// Entities 的DataHelp, 如果这里说明了，他是Ens , 就要指明DataHelpKey. 
		/// 这样，系统就会在右键帮助会出现他。
		/// 可能是需要多个值得选择问题。当选择多个值的时间，就用',' 把他们分开返回。 
		/// </summary>
		EnsOfMany,
		/// <summary>
		/// 自定义的类型。
		/// </summary>
		Self,
		/// <summary>
		/// 正常的
		/// </summary>
		TB,
		/// <summary>
		/// Num
		/// </summary>
		Num,
		/// <summary>
		/// Int
		/// </summary>
		Int,		 
		/// <summary>
		/// Float
		/// </summary>
		Float,
		/// <summary>
		/// Decimal
		/// </summary>
		Decimal,
		/// <summary>
		/// Moneny
		/// </summary>
		Moneny,
		/// <summary>
		/// Date
		/// </summary>
		Date,
		/// <summary>
		/// DateTime
		/// </summary>
		DateTime,
		/// <summary>
		/// Email
		/// </summary>
		Email,		
		/// <summary>
		/// Key
		/// </summary>
		Key,
		Area

	} 
	/// <summary>
	/// AddAllLocation
	/// </summary>
	public enum AddAllLocation
	{
		/// <summary>
		/// 显示上方
		/// </summary>
		Top,
		/// <summary>
		/// 显示下方
		/// </summary>
		End,
		/// <summary>
		/// 显示上方和下方
		/// </summary>
		TopAndEnd,
		/// <summary>
		/// 不显示
		/// </summary>
		None,
        /// <summary>
        /// 多选
        /// </summary>
        TopAndEndWithMVal
	} 
	/// <summary>
	/// DDLShowType
	/// </summary>
	public enum DDLShowType
	{
		/// <summary>
		/// None
		/// </summary>
		None,
		/// <summary>
		/// Gender
		/// </summary>
		Gender,
		/// <summary>
		/// Boolean
		/// </summary>
		Boolean,
		/// <summary>
		/// 
		/// </summary> 
		SysEnum,
		/// <summary>
		/// Self
		/// </summary>
		Self,
		/// <summary>
		/// 实体集合
		/// </summary>
		Ens,
		/// <summary>
		/// 与Table 相关联
		/// </summary>
		BindTable
	}
	/// <summary>
	/// DDLShowType
	/// </summary>
	public enum DDLShowType_del
	{
		/// <summary>
		/// None
		/// </summary>
		None,
		/// <summary>
		/// Gender
		/// </summary>
		Gender,
		/// <summary>
		/// Boolean
		/// </summary>
		Boolean,
		/// <summary>
		/// Year
		/// </summary>
		Year,
		/// <summary>
		/// Month
		/// </summary>
		Month,
		/// <summary>
		/// Day
		/// </summary>
		Day,
		/// <summary>
		/// hh
		/// </summary>
		hh,
		/// <summary>
		/// mm
		/// </summary>
		mm,
		/// <summary>
		/// 季度
		/// </summary>
		Quarter,
		/// <summary>
		/// Week
		/// </summary>
		Week,
		/// <summary>
		/// 系统枚举类型 SelfBindKey="系统枚举Key"
		/// </summary>
		SysEnum,
		/// <summary>
		/// Self
		/// </summary>
		Self,
		/// <summary>
		/// 实体集合
		/// </summary>
		Ens,
		/// <summary>
		/// 与Table 相关联
		/// </summary>
		BindTable
	}
    /// <summary>
    /// 显示方式
    /// </summary>
    public enum FormShowType
    {
        /// <summary>
        /// 未设置
        /// </summary>
        NotSet,
        /// <summary>
        /// 傻瓜表单
        /// </summary>
        FixForm,
        /// <summary>
        /// 自由表单
        /// </summary>
        FreeForm
    }
}