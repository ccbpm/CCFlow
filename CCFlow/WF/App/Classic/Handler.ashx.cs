using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using BP.WF;
using BP.Web;
using BP.Sys;
using BP.DA;
using BP.En;
using BP.WF.Template;
using Newtonsoft.Json.Converters;
//using System.Web.SessionState;

namespace LIMSApp.WF.App.Classic
{
	/// <summary>
	/// Handler 的摘要说明
	/// </summary>
	public class Handler : IHttpHandler//, IRequiresSessionState
	{
		#region 属性.
		/// <summary>
		/// 执行类型
		/// </summary>
		public string DoType
		{
			get
			{
				string str = context.Request.QueryString["DoType"];
				if (str == null || str == "" || str == "null")
					return null;
				return str;
			}
		}
		public string MyPK
		{
			get
			{
				string str = context.Request.QueryString["MyPK"];
				if (str == null || str == "" || str == "null")
					return null;
				return str;
			}
		}
		/// <summary>
		/// 字典表
		/// </summary>
		public string FK_SFTable
		{
			get
			{
				string str = context.Request.QueryString["FK_SFTable"];
				if (str == null || str == "" || str == "null")
					return null;
				return str;

			}
		}
		public string EnumKey
		{
			get
			{
				string str = context.Request.QueryString["EnumKey"];
				if (str == null || str == "" || str == "null")
					return null;
				return str;

			}
		}
		public string KeyOfEn
		{
			get
			{
				string str = context.Request.QueryString["KeyOfEn"];
				if (str == null || str == "" || str == "null")
					return null;
				return str;
			}
		}
		public string FK_MapData
		{
			get
			{
				string str = context.Request.QueryString["FK_MapData"];
				if (str == null || str == "" || str == "null")
					return null;
				return str;
			}
		}
		public int GroupField
		{
			get
			{
				string str = context.Request.QueryString["GroupField"];
				if (str == null || str == "" || str == "null")
					return 0;
				return int.Parse(str);
			}
		}

		/// <summary>
		/// 框架ID
		/// </summary>
		public string FK_MapFrame
		{
			get
			{
				string str = context.Request.QueryString["FK_MapFrame"];
				if (str == null || str == "" || str == "null")
					return null;
				return str;
			}
		}
		/// <summary>
		///  节点ID.
		/// </summary>
		public int FK_Node
		{
			get
			{
				string str = context.Request.QueryString["FK_Node"];
				if (str == null || str == "" || str == "null")
					return 0;
				return int.Parse(str);
			}
		}
		/// <summary>
		///   RefOID
		/// </summary>
		public int RefOID
		{
			get
			{
				string str = context.Request.QueryString["RefOID"];
				if (str == null || str == "" || str == "null")
					return 0;
				return int.Parse(str);
			}
		}
		/// <summary>
		/// 明细表
		/// </summary>
		public string FK_MapDtl
		{
			get
			{
				string str = context.Request.QueryString["FK_MapDtl"];
				if (str == null || str == "" || str == "null")
					return null;
				return str;
			}
		}

		/// <summary>
		/// 字段属性编号
		/// </summary>
		public string Ath
		{
			get
			{
				string str = context.Request.QueryString["Ath"];
				if (str == null || str == "" || str == "null")
					return null;
				return str;
			}
		}

		public HttpContext context = null;
		/// <summary>
		/// 获得表单的属性.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string GetValFromFrmByKey(string key)
		{
			string val = context.Request.Form[key];
			if (val == null)
				return null;
			val = val.Replace("'", "~");
			return val;
		}
		public int GetValIntFromFrmByKey(string key)
		{
			string str = this.GetValFromFrmByKey(key);
			if (str == null || str == "")
				throw new Exception("@参数:" + key + " 没有取到值.");

			return int.Parse(str);
		}
		public bool GetValBoolenFromFrmByKey(string key)
		{
			string val = this.GetValFromFrmByKey(key);
			if (val == null || val == "")
				return false;
			return true;
		}
		/// <summary>
		/// 公共方法获取值
		/// </summary>
		/// <param name="param">参数名</param>
		/// <returns></returns>
		public string GetRequestVal(string param)
		{
			return HttpUtility.UrlDecode(context.Request[param], System.Text.Encoding.UTF8);
		}
		/// <summary>
		/// 公共方法获取值
		/// </summary>
		/// <param name="param">参数名</param>
		/// <returns></returns>
		public int GetRequestValInt(string param)
		{
			string str = GetRequestVal(param);
			if (str == null || str == "")
				return 0;
			return int.Parse(str);
		}
		#endregion 属性.

		public void ProcessRequest(HttpContext mycontext)
		{
			context = mycontext;
			string msg = "";
			try
			{
				switch (this.DoType)
				{
					case "ChangePassword":
						msg = this.ChangePassword(context.Request);
						break;
				}
				context.Response.Write(msg);
			}
			catch (Exception ex)
			{
				context.Response.Write("err@" + ex.Message);
			}
		}

		/// <summary>
		/// 修改密码
		/// </summary>
		/// <param name="req">HttpRequest</param>
		/// <returns></returns>
		public string ChangePassword(HttpRequest req)
		{
			BP.Port.Emp emp = new BP.Port.Emp(WebUser.No);
			if (emp.RetrieveFromDBSources() == 0)
				return "当前用户已被删除或禁用，不能修改密码！";
			//验证旧密码
			var strPassOld = req["pwOld"];
			if (emp.Pass != strPassOld)
				return "原密码验证失败，请重新输入！";
			//更改密码
			emp.Pass = req["pwNew"];
			if (emp.Update() == 1)
			{
				return "修改成功！";
			}
			else
			{
				return "修改密码失败，请稍后重试！";
			}
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}