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


namespace CCFlow.WF.Admin.AttrFlow
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
					case "NodeAttrs_Init":
						msg = this.NodeAttrs_Init();
						break;
				}
				context.Response.Write(msg);
			}
			catch (Exception ex)
			{
				context.Response.Write("err@" + ex.Message);
			}
		}

		#region 节点属性（列表）的操作
		/// <summary>
		/// 初始化节点属性列表.
		/// </summary>
		/// <returns></returns>
		public string NodeAttrs_Init()
		{
			var strFlowId = GetRequestVal("FK_Flow");
			if (string.IsNullOrEmpty(strFlowId))
			{
				return "err@参数错误！";
			}
			Nodes nodes = new Nodes();
			nodes.Retrieve("FK_Flow", strFlowId);
			//因直接使用nodes.ToJson()无法获取某些字段（e.g.HisFormTypeText,原因：Node没有自己的Attr类）
			//故此处手动创建前台所需的DataTable
			DataTable dt = new DataTable();
			dt.Columns.Add("NodeID");	//节点ID
			dt.Columns.Add("Name");		//节点名称
			dt.Columns.Add("HisFormType");		//表单方案
			dt.Columns.Add("HisFormTypeText");
			dt.Columns.Add("HisRunModel");		//节点类型
			dt.Columns.Add("HisRunModelT");

			dt.Columns.Add("HisDeliveryWay");	//接收方类型
			dt.Columns.Add("HisDeliveryWayText");
			dt.Columns.Add("HisDeliveryWayJsFnPara");
			dt.Columns.Add("HisDeliveryWayCountLabel");
			dt.Columns.Add("HisDeliveryWayCount");	//接收方Count

			dt.Columns.Add("HisCCRole");	//抄送人
			dt.Columns.Add("HisCCRoleText");
			dt.Columns.Add("HisFrmEventsCount");	//消息&事件Count
			dt.Columns.Add("HisFinishCondsCount");	//流程完成条件Count
			dt.Columns.Add("HisListensCount");	//消息收听Count
			DataRow dr;
			foreach (Node node in nodes)
			{
				dr = dt.NewRow();
				dr["NodeID"] = node.NodeID;
				dr["Name"] = node.Name;
				dr["HisFormType"] = node.HisFormType;
				dr["HisFormTypeText"] = node.HisFormTypeText;
				dr["HisRunModel"] = node.HisRunModel;
				dr["HisRunModelT"] = node.HisRunModelT;
				dr["HisDeliveryWay"] = node.HisDeliveryWay;
				dr["HisDeliveryWayText"] = node.HisDeliveryWayText;

				//接收方数量
				var intHisDeliveryWayCount = 0;
				if (node.HisDeliveryWay == BP.WF.DeliveryWay.ByStation)
				{
					dr["HisDeliveryWayJsFnPara"] = "ByStation";
					dr["HisDeliveryWayCountLabel"] = "岗位";
					BP.WF.Template.NodeStations nss = new BP.WF.Template.NodeStations();
					intHisDeliveryWayCount = nss.Retrieve(BP.WF.Template.NodeStationAttr.FK_Node, node.NodeID);
				}
				else if (node.HisDeliveryWay == BP.WF.DeliveryWay.ByDept)
				{
					dr["HisDeliveryWayJsFnPara"] = "ByDept";
					dr["HisDeliveryWayCountLabel"] = "部门";
					BP.WF.Template.NodeDepts nss = new BP.WF.Template.NodeDepts();
					intHisDeliveryWayCount = nss.Retrieve(BP.WF.Template.NodeDeptAttr.FK_Node, node.NodeID);
				}
				else if (node.HisDeliveryWay == BP.WF.DeliveryWay.ByBindEmp)
				{
					dr["HisDeliveryWayJsFnPara"] = "ByDept";
					dr["HisDeliveryWayCountLabel"] = "人员";
					BP.WF.Template.NodeEmps nes = new BP.WF.Template.NodeEmps();
					intHisDeliveryWayCount = nes.Retrieve(BP.WF.Template.NodeStationAttr.FK_Node, node.NodeID);
				}
				dr["HisDeliveryWayCount"] = intHisDeliveryWayCount;

				//抄送
				dr["HisCCRole"] = node.HisCCRole;
				dr["HisCCRoleText"] = node.HisCCRoleText;

				//消息&事件Count
				BP.Sys.FrmEvents fes = new BP.Sys.FrmEvents();
				dr["HisFrmEventsCount"] = fes.Retrieve(BP.Sys.FrmEventAttr.FK_MapData, "ND" + node.NodeID);

				//流程完成条件Count
				BP.WF.Template.Conds conds = new BP.WF.Template.Conds(BP.WF.Template.CondType.Flow, node.NodeID);
				dr["HisFinishCondsCount"] = conds.Count;

				//消息收听Count
				BP.WF.Template.Listens lns = new BP.WF.Template.Listens(node.NodeID);
				dr["HisListensCount"] = lns.Count;

				dt.Rows.Add(dr);
			}
			return BP.Tools.Json.ToJson(dt);
		}
		#endregion

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}