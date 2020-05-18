using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web.Controls;
using BP.Web;
using BP.Sys;

namespace BP.Sys
{
	/// <summary>
	/// 表单从表事件基类
	/// 0,集成该基类的子类,可以重写事件的方法与基类交互.
	/// 1,一个子类必须与一个表单模版绑定.
	/// 2,基类里有很多表单运行过程中的变量，这些变量可以辅助开发者在编写复杂的业务逻辑的时候使用.
	/// 3,该基类有一个子类模版，位于:\CCForm\WF\Admin\AttrForm\F001Templepte.cs .
	/// </summary>
	abstract public class FormEventBaseDtl
	{
		#region 要求子类强制重写的属性.
		/// <summary>
		/// 表单编号/表单标记.
		/// 该参数用于说明要把此事件注册到那一个表单模版上.
		/// </summary>
		abstract public string FormDtlMark
		{
			get;
		}
		#endregion 要求子类重写的属性.

		#region 属性/内部变量(表单在运行的时候触发各类事件，子类可以访问这些属性来获取引擎内部的信息).
		/// <summary>
		/// 实体，一般是工作实体
		/// </summary>
		public Entity HisEn = null;
		/// <summary>
		/// 
		/// </summary>
		public Entity HisEnDtl = null;
		/// <summary>
		/// 参数对象.
		/// </summary>
		private Row _SysPara = null;
		/// <summary>
		/// 参数
		/// </summary>
		public Row SysPara
		{
			get
			{
				if (_SysPara == null)
					_SysPara = new Row();
				return _SysPara;
			}
			set
			{
				_SysPara = value;
			}
		}
		#endregion 属性/内部变量(表单在运行的时候触发各类事件，子类可以访问这些属性来获取引擎内部的信息).

		#region 系统参数
		/// <summary>
		/// 表单ID
		/// </summary>
		public string FK_Mapdata
		{
			get
			{
				return this.GetValStr("FK_MapData");
			}
		}
		/// <summary>
		/// 从表ID
		/// </summary>
		public string FK_MapDtl
		{
			get
			{
				return this.GetValStr("FK_MapDtl");
			}
		}
		#endregion

		#region 常用属性.
		/// <summary>
		/// 从表ID
		/// </summary>
		public int OID
		{
			get
			{
				return this.GetValInt("OID");
			}
		}
		/// <summary>
		/// 主表ID
		/// </summary>
		public int OIDOfMainEn
		{
			get
			{
				return this.GetValInt("OIDOfMainEn");
			}
		}
		#endregion 常用属性.

		#region 获取参数方法
		/// <summary>
		/// 事件参数
		/// </summary>
		/// <param name="key">时间字段</param>
		/// <returns>根据字段返回一个时间,如果为Null,或者不存在就抛出异常.</returns>
		public DateTime GetValDateTime(string key)
		{
			try
			{
				string str = this.SysPara.GetValByKey(key).ToString();
				return DataType.ParseSysDateTime2DateTime(str);
			}
			catch (Exception ex)
			{
				throw new Exception("@表单从表事件实体在获取参数期间出现错误，请确认字段(" + key + ")是否拼写正确,技术信息:" + ex.Message);
			}
		}
		/// <summary>
		/// 获取字符串参数
		/// </summary>
		/// <param name="key">key</param>
		/// <returns>如果为Null,或者不存在就抛出异常</returns>
		public string GetValStr(string key)
		{
			try
			{
				return this.SysPara.GetValByKey(key).ToString();
			}
			catch (Exception ex)
			{
				throw new Exception("@表单从表事件实体在获取参数期间出现错误，请确认字段(" + key + ")是否拼写正确,技术信息:" + ex.Message);
			}
		}
		/// <summary>
		/// 获取Int64的数值
		/// </summary>
		/// <param name="key">键值</param>
		/// <returns>如果为Null,或者不存在就抛出异常</returns>
		public Int64 GetValInt64(string key)
		{
			return Int64.Parse(this.GetValStr(key));
		}
		/// <summary>
		/// 获取int的数值
		/// </summary>
		/// <param name="key">键值</param>
		/// <returns>如果为Null,或者不存在就抛出异常</returns>
		public int GetValInt(string key)
		{
			return int.Parse(this.GetValStr(key));
		}
		/// <summary>
		/// 获取Boolen值
		/// </summary>
		/// <param name="key">字段</param>
		/// <returns>如果为Null,或者不存在就抛出异常</returns>
		public bool GetValBoolen(string key)
		{
			if (int.Parse(this.GetValStr(key)) == 0)
				return false;
			return true;
		}
		/// <summary>
		/// 获取decimal的数值
		/// </summary>
		/// <param name="key">字段</param>
		/// <returns>如果为Null,或者不存在就抛出异常</returns>
		public decimal GetValDecimal(string key)
		{
			return decimal.Parse(this.GetValStr(key));
		}
		#endregion 获取参数方法

		#region 构造方法
		/// <summary>
		/// 表单从表事件基类
		/// </summary>
		public FormEventBaseDtl()
		{
		}
		#endregion 构造方法

		#region 节点表单从表事件
		public virtual string FrmLoadAfter()
		{
			return null;
		}
		public virtual string FrmLoadBefore()
		{
			return null;
		}
		#endregion

		#region 要求子类重写的方法(表单从表事件).
		/// <summary>
		/// 表单删除前
		/// </summary>
		/// <returns>返回null,不提示信息,返回信息，提示删除警告/提示信息, 抛出异常阻止删除操作.</returns>
		public virtual string BeforeFormDel()
		{
			return null;
		}
		/// <summary>
		/// 表单删除后
		/// </summary>
		/// <returns>返回null,不提示信息,返回信息，提示删除警告/提示信息, 抛出异常不处理.</returns>
		public virtual string AfterFormDel()
		{
			return null;
		}
		#endregion 要求子类重写的方法(表单从表事件).

		#region 要求子类重写的方法(节点事件).
		/// <summary>
		/// 附件上传前
		/// </summary>
		public virtual string AthUploadeBefore()
		{
			return null;
		}
		/// <summary>
		/// 上传后
		/// </summary>
		public virtual string AthUploadeAfter()
		{
			return null;
		}
		/// <summary>
		/// 从表保存前
		/// </summary>
		public virtual string RowSaveBefore()
		{
			return null;
		}
		/// <summary>
		/// 从表保存后
		/// </summary>
		public virtual string RowSaveAfter()
		{
			return null;
		}

        /// <summary>
        /// 从表del前
        /// </summary>
        public virtual string DtlRowDelBefore()
        {
            return null;
        }
        /// <summary>
        /// 从表del后
        /// </summary>
        public virtual string DtlRowDelAfter()
        {
            return null;
        }

		/// <summary>
		/// 创建工作ID后的事件
		/// </summary>
		/// <returns></returns>
		public virtual string CreateOID()
		{
			return null;
		}
		#endregion 要求子类重写的方法(节点事件).

		#region 基类方法.
		/// <summary>
		/// 执行事件
		/// </summary>
		/// <param name="eventType">事件类型</param>
		/// <param name="en">主表实体</param>
		/// <param name="enDtl">从表实体</param>
		/// <param name="atPara">参数</param>
		/// <returns>返回执行的结果</returns>
		public string DoIt(string eventType, Entity en, Entity enDtl, string atPara)
		{
			this.HisEn = en;

			#region 处理参数.

			if (en == null)
				return null;
			Row r = en.Row;
			try
			{
				//系统参数.
				r.Add("FK_MapData", en.ClassID);
			}
			catch
			{
				r["FK_MapData"] = en.ClassID;
			}

			if (atPara != null)
			{
				AtPara ap = new AtPara(atPara);
				foreach (string s in ap.HisHT.Keys)
				{
					try
					{
						r.Add(s, ap.GetValStrByKey(s));
					}
					catch
					{
						r[s] = ap.GetValStrByKey(s);
					}
				}
			}

			if (SystemConfig.IsBSsystem == true)
			{
				/*如果是bs系统, 就加入外部url的变量.*/
				foreach (string key in HttpContextHelper.RequestParamKeys)
				{
					string val = HttpContextHelper.RequestParams(key);
					try
					{
						r.Add(key, val);
					}
					catch
					{
						r[key] = val;
					}
				}
			}
			this.SysPara = r;
			#endregion 处理参数.

			#region 执行事件.
			switch (eventType)
			{
				case FrmEventListDtl.RowSaveBefore: // 从表-保存前.。
					return this.RowSaveBefore();
				case FrmEventListDtl.RowSaveAfter: // 从表-保存后.。
					return this.RowSaveAfter();
                case FrmEventListDtl.DtlRowDelBefore: // 从表-保存前.。
                    return this.DtlRowDelBefore();
                case FrmEventListDtl.DtlRowDelAfter: // 从表-保存后.。
                    return this.DtlRowDelAfter();
				default:
					throw new Exception("@没有判断的表单从表事件类型:" + eventType);
					break;
			}
			#endregion 执行事件.

			return null;
		}
		#endregion 基类方法.
	}
}
