using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using BP.WF;
using BP.Web;
using BP.Sys;
using BP.DA;
using BP.En;

namespace CCFlow.WF.Admin.CCFormDesigner
{
	/// <summary>
	/// Handler 的摘要说明
	/// </summary>
	public class Handler : IHttpHandler
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
					case "FiledsList_Init":
						msg = this.FiledsList_Init();
						break;
					case "FiledsList_Delete":
						msg = this.FiledsList_Delete();
						break;
				}
				context.Response.Write(msg);
			}
			catch (Exception ex)
			{
				context.Response.Write("err@" + ex.Message);
			}
		}

		#region 字段列表 的操作
		/// <summary>
		/// 初始化字段列表.
		/// </summary>
		/// <returns></returns>
		public string FiledsList_Init()
		{
			//DataSet ds = new DataSet();
			MapAttrs attrs = new MapAttrs();
			attrs.Retrieve(MapAttrAttr.FK_MapData, this.FK_MapData);
			foreach (MapAttr item in attrs)
			{
				if (item.LGType == FieldTypeS.Enum)
				{
					SysEnumMain se = new SysEnumMain(item.UIBindKey);
					item.UIRefKey = se.CfgVal;
					continue;
				}

				if (item.LGType == FieldTypeS.FK)
				{
					item.UIRefKey = item.UIBindKey;
					continue;
				}

				item.UIRefKey = "无";
			}

			//DataTable dt = attrs.ToDataTableField();
			//dt.TableName = "Sys_MapAttr";
			//ds.Tables.Add(dt);

			//GroupFields gfs = new GroupFields();
			//gfs.Retrieve(GroupFieldAttr.EnName, this.FK_MapData);

			//DataTable dtG = gfs.ToDataTableField();
			//dtG.TableName = "Sys_GroupField";
			//ds.Tables.Add(dtG);

			//return BP.Tools.Json.ToJson(ds);
			return attrs.ToJson();
		}

		/// <summary>
		/// 删除字段
		/// </summary>
		/// <returns></returns>
		public string FiledsList_Delete()
		{
			MapAttr attr = new MapAttr(this.MyPK);
			if (attr.Delete() == 1)
			{
				return "删除成功！";
			}
			else
			{
				return "err@删除失败！";
			}
		}
		#endregion 字段列表 的操作

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}