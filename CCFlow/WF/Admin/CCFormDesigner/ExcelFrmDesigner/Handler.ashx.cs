using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using BP.WF;
using BP.Web;
using BP.Sys;
using BP.DA;
using BP.En;


namespace CCFlow.WF.Admin.CCFormDesigner.ExcelFrmDesigner
{
	/// <summary>
	/// Summary description for Handler
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
					//case "LoadFile": //加载文件（表单模板）
					//    LoadFile(context);
					//    break;
					case "GetFileUrl": //获取文件（表单模板）路径
						msg = GetFileUrl(mycontext.Request);
						break;
					case "SaveFile": //保存文件（表单模板）
						msg = SaveFile(mycontext.Request);
						break;
					case "NewSub": //新建子表
						msg = NewSub();
						break;
					case "SetBindCell":
						msg = SetBindCell();
						break;
					case "GetSubTables": //获取所有子表信息
						msg = GetSubTables();
						break;
					case "GetEnumList": //获取枚举值列表
						msg = GetEnumList();
						break;
					case "GetFkeyList":
						msg = GetFkeyList();
						break;
					case "DelSub": //删除子表
						break;
					//case "FiledsList_Init": //初始化字段列表//暂时使用公用（FieldsList）的
					//    msg = "";//this.FiledsList_Init();
					//    break;
					//case "NewColumn": //新建字段//不需要在此实现
					//    break;
					default:
						msg = "err@没有判断的执行类型：" + this.DoType;
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
		/// 获取文件路径
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public string GetFileUrl(HttpRequest req)
		{
			//文件保存路径
			var root = SystemConfig.PathOfDataUser + "\\FrmOfficeTemplate\\";
			var rootInfo = new DirectoryInfo(root);
			if (!rootInfo.Exists)
				rootInfo.Create();

			//根据FK_MapData获取模板文件
			var files = rootInfo.GetFiles(this.FK_MapData + ".*");
			if (files.Length == 0)
			{
				return ("err@表单模板文件不存在，请确认已经上传Excel表单模板，该模版的位于服务器：" + rootInfo.FullName + ".");
			}
			else
			{
				//var a1 = req.ApplicationPath; // /
				//var a2 = req.PhysicalApplicationPath; // D:\ccflow\CCFlow\
				//var a3 = files[0].FullName; // D:\ccflow\CCFlow\DataUser\FrmOfficeTemplate\CSExcelBD.xlsx
				//var a4 = req.Url.Scheme; // http
				//var a5 = req.Url.Authority; // localhost:xxxxx
				return req.Url.Scheme + "://" + req.Url.Authority + "/" +
					   files[0].FullName.Replace(req.PhysicalApplicationPath, string.Empty).Replace("\\", "/");
			}
		}

		/// <summary>
		/// 保存Excel模板
		/// </summary>
		/// <returns></returns>
		public string SaveFile(HttpRequest req)
		{
			//模板保存路径
			var root = SystemConfig.PathOfDataUser + "\\FrmOfficeTemplate\\";
			var rootInfo = new DirectoryInfo(root);
			if (!rootInfo.Exists)
				rootInfo.Create();

			//保存模板
			if (req.Files.Count > 0)
			{
				HttpPostedFile file = req.Files[0];
				//var extension = System.IO.Path.GetExtension(file.FileName); //.xlsx
				file.SaveAs(root + this.FK_MapData + System.IO.Path.GetExtension(file.FileName));
				return "true";
			}
			return "false";

			//string message = "true";
			//try
			//{
			//    HttpFileCollection files = req.Files;
			//    string file = req["Path"];
			//    //r path = SystemConfig.PathOfDataUser + "\\FrmOfficeFiles" + name;
			//    file = HttpUtility.UrlDecode(file, Encoding.UTF8);
			//    if (files.Count > 0)
			//    {
			//        ///'检查文件扩展名字
			//        HttpPostedFile postedFile = files[0]; //C:\Users\Mayy\AppData\Local\Temp\14604.xlsx
			//        string fileName, fileExtension;
			//        fileName = System.IO.Path.GetFileName(postedFile.FileName);
			//        fileExtension = 
			//        string path = "";

			//        /*try
			//        {
			//            path = Server.MapPath("~/" + FileFullName);
			//        }
			//        catch
			//        {
			//            path = FileFullName;
			//        }*/

			//        if (fileName != "")
			//        {
			//            fileExtension = System.IO.Path.GetExtension(fileName);
			//            postedFile.SaveAs(path);
			//        }
			//    }
			//}
			//catch (Exception ex)
			//{
			//    message = ex.Message.ToString();
			//}
			//return message;
		}

		/// <summary>
		/// 新建子表
		/// </summary>
		/// <returns></returns>
		public string NewSub()
		{
			MapDtl dtl = new MapDtl();
			//var intNo = dtl.GetCountByFK("FK_MapData", this.FK_MapData) + 1;
			//dtl.No = this.FK_MapData + "Dtl" + intNo.ToString();
			dtl.FK_MapData = this.FK_MapData;
			dtl.No = this.GetRequestVal("SubId");
			dtl.Name = this.FK_MapData;
			if (dtl.Insert() == 1)
			{
				dtl.IntMapAttrs();
				return dtl.No;
			}
			else
			{
				return "err@存在相同名称的表：" + dtl.No;
			}
		}

		/// <summary>
		/// 删除子表（暂时没用）
		/// </summary>
		/// <returns></returns>
		public string DelSub()
		{
			return null;
		}

		/// <summary>
		/// 设置字段对应单元格
		/// </summary>
		/// <returns></returns>
		public string SetBindCell()
		{
			MapAttr ma = new MapAttr(this.MyPK);
			ma.SetPara("BindCell", this.GetRequestVal("Cell"));
			ma.Update();
			return "success";
		}

		/// <summary>
		/// 获取所有子表
		/// </summary>
		/// <returns></returns>
		public string GetSubTables()
		{
			MapDtls dtls = new MapDtls();
			dtls.Retrieve(MapDtlAttr.FK_MapData, this.FK_MapData);
			return dtls.ToJson();
		}

		/// <summary>
		/// 获取枚举值列表
		/// </summary>
		/// <returns></returns>
		public string GetEnumList()
		{
			SysEnums enums = new SysEnums();
			enums.Retrieve(SysEnumAttr.EnumKey, this.GetRequestVal("EnumKey"));
			return enums.ToJson();
		}

		/// <summary>
		/// 获取外键列表
		/// </summary>
		/// <returns></returns>
		public string GetFkeyList()
		{
			MapAttr ma = new MapAttr(this.MyPK);
			//ma.Retrieve(MapAttrAttr.FK_MapData, this.FK_MapData, MapAttrAttr.KeyOfEn, this.GetRequestVal("Fkey"));
			DataTable dt = ma.HisDT;
			return BP.Tools.Json.ToJson(dt);
		}

		/// <summary>
		/// 获取字段列表（暂时没用）
		/// </summary>
		/// <returns></returns>
		public string GetAttrList() //copy from: CCFlow.WF.Admin.CCFormDesigner.Handler.FiledsList_Init()
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

			return attrs.ToJson();
		}

		/// <summary>
		/// 加载Excel模板（暂时没用）
		/// </summary>
		/// <returns></returns>
		public void LoadFile(HttpContext context)
		{
			string name = context.Request.QueryString["FileName"];
			var path = SystemConfig.PathOfDataUser + "\\FrmOfficeFiles" + name;
			var result = File.ReadAllBytes(path);
			context.Response.Clear();
			context.Response.BinaryWrite(result);
			context.Response.End();

			//FileInfo file = new FileInfo(filename);//创建一个文件对象
			//Response.Clear();//清除所有缓存区的内容
			//Response.Charset = "GB2312";//定义输出字符集
			//Response.ContentEncoding = Encoding.Default;//输出内容的编码为默认编码
			//Response.AddHeader("Content-Disposition", "attachment;filename=" + file.Name);//添加头信息。为“文件下载/另存为”指定默认文件名称
			//Response.AddHeader("Content-Length", file.Length.ToString());//添加头文件，指定文件的大小，让浏览器显示文件下载的速度
			//Response.WriteFile(file.FullName);// 把文件流发送到客户端
			//Response.End();//将当前所有缓冲区的输出内容发送到客户端，并停止页面的执行
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}

	public class Server
	{
	}
}