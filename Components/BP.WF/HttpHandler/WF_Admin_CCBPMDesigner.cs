using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Template;
using LitJson;
using BP.WF.XML;

namespace BP.WF.HttpHandler
{
	/// <summary>
	/// 初始化函数
	/// </summary>
	public class WF_Admin_CCBPMDesigner : DirectoryPageBase
	{

		public string GetFlowTreeTable2019()
		{
			string sql = @"SELECT * FROM (SELECT 'F'+No as No,'F'+ParentNo ParentNo, Name, IDX, 1 IsParent,'FLOWTYPE' TTYPE,-1 DTYPE FROM WF_FlowSort
                           union 
                           SELECT NO, 'F'+FK_FlowSort as ParentNo,(NO + '.' + NAME) as Name,IDX,0 IsParent,'FLOW' TTYPE, 0 as DTYPE FROM WF_Flow) A  ORDER BY IDX";

			if (BP.Sys.SystemConfig.AppCenterDBType == DBType.Oracle || BP.Sys.SystemConfig.AppCenterDBType == DBType.PostgreSQL)
			{
				sql = @"SELECT * FROM (SELECT 'F'||No as No,'F'||ParentNo as ParentNo,Name, IDX, 1 IsParent,'FLOWTYPE' TTYPE,-1 DTYPE FROM WF_FlowSort
                        union 
                        SELECT NO, 'F'||FK_FlowSort as ParentNo,NO||'.'||NAME as Name,IDX,0 IsParent,'FLOW' TTYPE,0 as DTYPE FROM WF_Flow) A  ORDER BY IDX";
			}


			if (BP.Sys.SystemConfig.AppCenterDBType == DBType.MySQL)
			{
				sql = @"SELECT * FROM (SELECT CONCAT('F', No) No, CONCAT('F', ParentNo) ParentNo, Name, IDX, 1 IsParent,'FLOWTYPE' TTYPE,-1 DTYPE FROM WF_FlowSort
                           union 
                           SELECT NO, CONCAT('F', FK_FlowSort) ParentNo, CONCAT(NO, '.', NAME) Name,IDX,0 IsParent,'FLOW' TTYPE,0 as DTYPE FROM WF_Flow) A  ORDER BY IDX";
			}

			DataTable dt = DBAccess.RunSQLReturnTable(sql);


			if (BP.Sys.SystemConfig.AppCenterDBType == DBType.PostgreSQL)
			{
				dt.Columns["no"].ColumnName = "No";
				dt.Columns["name"].ColumnName = "Name";
				dt.Columns["parentno"].ColumnName = "ParentNo";
				dt.Columns["idx"].ColumnName = "IDX";

				dt.Columns["ttype"].ColumnName = "TTYPE";
				dt.Columns["dtype"].ColumnName = "DTYPE";
			}

			if (BP.Sys.SystemConfig.AppCenterDBType == DBType.Oracle)
			{
				dt.Columns["NO"].ColumnName = "No";
				dt.Columns["NAME"].ColumnName = "Name";
				dt.Columns["PARENTNO"].ColumnName = "ParentNo";
				dt.Columns["IDX"].ColumnName = "IDX";

				dt.Columns["TTYPE"].ColumnName = "TTYPE";
				dt.Columns["DTYPE"].ColumnName = "DTYPE";
			}

			//判断是否为空，如果为空，则创建一个流程根结点，added by liuxc,2016-01-24
			if (dt.Rows.Count == 0)
			{
				FlowSort fs = new FlowSort();
				fs.No = "99";
				fs.ParentNo = "0";
				fs.Name = "流程树";
				fs.Insert();

				dt.Rows.Add("F99", "F0", "流程树", 0, 1, "FLOWTYPE", -1);
			}
			else
			{
				DataRow[] drs = dt.Select("NAME='流程树'");
				if (drs.Length > 0 && !Equals(drs[0]["PARENTNO"], "F0"))
					drs[0]["ParentNo"] = "F0";
			}


			if (WebUser.No != "admin")
			{
				BP.WF.Port.AdminEmp aemp = new Port.AdminEmp();
				aemp.No = WebUser.No;
				if (aemp.RetrieveFromDBSources() == 0)
					return "err@登录帐号错误.";

				if (aemp.IsAdmin == false)
					return "err@非管理员用户.";

				DataRow rootRow = dt.Select("ParentNo='F0'")[0];
				DataRow newRootRow = dt.Select("No='F" + aemp.RootOfFlow + "'")[0];

				newRootRow["ParentNo"] = "F0";
				DataTable newDt = dt.Clone();
				newDt.Rows.Add(newRootRow.ItemArray);
				GenerChildRows(dt, newDt, newRootRow);
				dt = newDt;
			}

			return BP.Tools.Json.ToJson(dt);
		}


		/// <summary>
		/// 按照管理员登录.
		/// </summary>
		/// <param name="userNo">管理员编号</param>
		/// <returns>登录信息</returns>
		public string AdminerChang_LoginAs()
		{
			string orgNo = this.GetRequestVal("OrgNo");

			BP.WF.Port.AdminEmp ae = new Port.AdminEmp();
			ae.No = WebUser.No + "@" + orgNo;
			if (ae.RetrieveFromDBSources() == 0)
				return "err@您不是该组织的管理员.";

			BP.WF.Port.AdminEmp ae1 = new Port.AdminEmp();
			ae1.No = WebUser.No;
			ae1.RetrieveFromDBSources();

			if (ae1.RootOfDept.Equals(orgNo) == true)
				return "info@当前已经是该组织的管理员了，您不用切换.";

			ae1.Copy(ae);
			ae1.No = WebUser.No;
			ae1.Update();

			//AdminEmp ad = new AdminEmp();
			//ad.No = userNo;
			//if (ad.RetrieveFromDBSources() == 0)
			//    return "err@用户名错误.";
			return "info@登录成功, 如果系统不能自动刷新，请手工刷新。";
		}

		public string Flows_Init()
		{
			DataTable dt = new DataTable();

			dt.Columns.Add("FlowNo");
			dt.Columns.Add("FlowName");

			dt.Columns.Add("NumOfRuning"); //运行中的.
			dt.Columns.Add("NumOfOK"); //已经完成的.
			dt.Columns.Add("NumOfEtc"); //其他.

			Flows fls = new Flows();
			fls.RetrieveAll();

			foreach (Flow fl in fls)
			{
				DataRow dr = dt.NewRow();
				dr["FlowNo"] = fl.No;
				dr["FlowName"] = fl.Name;
				dr["NumOfRuning"] = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM  WF_GenerWorkFlow WHERE FK_Flow='" + fl.No + "' AND WFState in (2,5)", 0);
				dr["NumOfOK"] = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM  WF_GenerWorkFlow WHERE FK_Flow='" + fl.No + "' AND WFState = 3 ", 0);
				dr["NumOfEtc"] = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM  WF_GenerWorkFlow WHERE FK_Flow='" + fl.No + "' AND WFState in (4,5,6,7,8) ", 0);

				dt.Rows.Add(dr);
			}

			return BP.Tools.Json.ToJson(dt);
		}
		/// <summary>
		/// 初始化函数
		/// </summary>
		/// <param name="mycontext"></param>
		public WF_Admin_CCBPMDesigner(HttpContext mycontext)
		{
			this.context = mycontext;
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public WF_Admin_CCBPMDesigner()
		{
		}
		/// <summary>
		/// 执行流程设计图的保存.
		/// </summary>
		/// <returns></returns>
		public string Designer_Save()
		{
			string sql = "";
			try
			{
				{
					StringBuilder sBuilder = new StringBuilder();

					//保存节点位置. @101,2,30@102,3,1
					string[] nodes = this.GetRequestVal("Nodes").Split('@');
					foreach (string item in nodes)
					{
						if (item == "" || item == null)
							continue;
						string[] strs = item.Split(',');
						sBuilder.Append("UPDATE WF_Node SET X=" + strs[1] + ",Y=" + strs[2] + " WHERE NodeID=" + strs[0] + ";");
					}

					DBAccess.RunSQL(sBuilder.ToString());

					//保存方向.
					sBuilder = new StringBuilder();
					string[] dirs = this.GetRequestVal("Dirs").Split('@');
					foreach (string item in dirs)
					{
						if (item == "" || item == null)
							continue;
						string[] strs = item.Split(',');
						sBuilder.Append("DELETE FROM WF_Direction where MyPK='" + strs[0] + "';");
						sBuilder.Append("INSERT INTO WF_Direction(MyPK,FK_Flow,Node,ToNode,IsCanBack) values('" + strs[0] + "','" + strs[1] + "','" + strs[2] + "','" + strs[3] + "'," + "0);");
					}

					DBAccess.RunSQL(sBuilder.ToString());

					//保存label位置.
					sBuilder = new StringBuilder();
					string[] labs = this.GetRequestVal("Labs").Split('@');
					foreach (string item in labs)
					{
						if (item == "" || item == null)
							continue;
						string[] strs = item.Split(',');
						sBuilder.Append("UPDATE WF_LabNote SET X=" + strs[1] + ",Y=" + strs[2] + " WHERE MyPK='" + strs[0] + "';");
					}

					DBAccess.RunSQL(sBuilder.ToString());

					return "保存成功.";
				}
			}
			catch (Exception ex)
			{
				return "err@" + ex.Message;
			}
		}

		/// <summary>
		/// 下载流程模版
		/// </summary>
		/// <returns></returns>
		public string ExpFlowTemplete()
		{
			Flow flow = new Flow(this.FK_Flow);
			string fileXml = flow.GenerFlowXmlTemplete();
			string docs = DataType.ReadTextFile(fileXml);
			return docs;
		}
		public string DownFormTemplete()
		{
			DataSet ds = BP.Sys.CCFormAPI.GenerHisDataSet_AllEleInfo(this.FK_MapData);
			string file = BP.Sys.SystemConfig.PathOfTemp + this.FK_MapData + ".xml";
			ds.WriteXml(file);
			string docs = DataType.ReadTextFile(file);
			return docs;

			//DataTable dt = new DataTable();
			//dt.Columns.Add("FileName");
			//dt.Columns.Add("FileType");
			//dt.Columns.Add("FlieContent");
			//DataRow dr = dt.NewRow();
			//dr["FileName"] = md.Name+".xml";
			//dr["FileType"] = "xml";
			//dr["FlieContent"] = docs;
			//dt.Rows.Add(dr);
			//return BP.Tools.Json.ToJson(dt);
		}

		#region 执行父类的重写方法.
		/// <summary>
		/// 默认执行的方法
		/// </summary>
		/// <returns></returns>
		protected override string DoDefaultMethod()
		{

			return "err@没有判断的标记:" + this.DoType;
		}
		#endregion 执行父类的重写方法.

		/// <summary>
		/// 使管理员登录使管理员登录    /// </summary>
		/// <returns></returns>
		public string LetLogin()
		{
			LetAdminLogin(this.GetRequestVal("UserNo"), true);
			return "登录成功.";
		}
		/// <summary>
		/// 获得枚举列表的JSON.
		/// </summary>
		/// <returns></returns>
		public string Logout()
		{
			BP.WF.Dev2Interface.Port_SigOut();
			return "您已经安全退出,欢迎使用ccbpm.";
		}

		/// <summary>
		/// 根据部门、岗位获取人员列表
		/// </summary>
		/// <returns></returns>
		public string GetEmpsByStationTable()
		{
			string deptid = this.GetRequestVal("DeptNo");
			string stid = this.GetRequestVal("StationNo");

			if (string.IsNullOrWhiteSpace(deptid) || string.IsNullOrWhiteSpace(stid))
				return "[]";

			DataTable dt = new DataTable();
			dt.Columns.Add("NO", typeof(string));
			dt.Columns.Add("PARENTNO", typeof(string));
			dt.Columns.Add("NAME", typeof(string));
			dt.Columns.Add("TTYPE", typeof(string));


			BP.GPM.Emp emp = null;
			BP.GPM.Emps emps = new BP.GPM.Emps();
			emps.RetrieveAll();

			BP.GPM.DeptEmpStations dess = new BP.GPM.DeptEmpStations();
			dess.Retrieve(BP.GPM.DeptEmpStationAttr.FK_Dept, deptid, BP.GPM.DeptEmpStationAttr.FK_Station, stid);

			foreach (BP.GPM.DeptEmpStation des in dess)
			{
				emp = emps.GetEntityByKey(des.FK_Emp) as BP.GPM.Emp;

				dt.Rows.Add(emp.No, deptid + "|" + stid, emp.Name, "EMP");
			}

			return BP.Tools.Json.ToJson(dt);
		}

		public string GetStructureTreeRootTable()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("NO", typeof(string));
			dt.Columns.Add("PARENTNO", typeof(string));
			dt.Columns.Add("NAME", typeof(string));
			dt.Columns.Add("TTYPE", typeof(string));

			string parentrootid = this.GetRequestVal("parentrootid");  // context.Request.QueryString["parentrootid"];
			string newRootId = "";

			if (WebUser.No != "admin")
			{
				BP.WF.Port.AdminEmp aemp = new Port.AdminEmp();
				aemp.No = WebUser.No;

				if (aemp.RetrieveFromDBSources() != 0 && aemp.UserType == 1 && !string.IsNullOrWhiteSpace(aemp.RootOfDept))
				{
					newRootId = aemp.RootOfDept;
				}
			}

			if (BP.WF.Glo.OSModel == OSModel.OneOne)
			{
				BP.WF.Port.Dept dept = new BP.WF.Port.Dept();

				if (!string.IsNullOrWhiteSpace(newRootId))
				{
					if (dept.Retrieve(BP.WF.Port.DeptAttr.No, newRootId) == 0)
					{
						dept.No = "-1";
						dept.Name = "无部门";
						dept.ParentNo = "";
					}
				}
				else
				{
					if (dept.Retrieve(BP.WF.Port.DeptAttr.ParentNo, parentrootid) == 0)
					{
						dept.No = "-1";
						dept.Name = "无部门";
						dept.ParentNo = "";
					}
				}

				dt.Rows.Add(dept.No, dept.ParentNo, dept.Name, "DEPT");
			}
			else
			{
				BP.GPM.Dept dept = new BP.GPM.Dept();

				if (!string.IsNullOrWhiteSpace(newRootId))
				{
					if (dept.Retrieve(BP.GPM.DeptAttr.No, newRootId) == 0)
					{
						dept.No = "-1";
						dept.Name = "无部门";
						dept.ParentNo = "";
					}
				}
				else
				{
					if (dept.Retrieve(BP.GPM.DeptAttr.ParentNo, parentrootid) == 0)
					{
						dept.No = "-1";
						dept.Name = "无部门";
						dept.ParentNo = "";
					}
				}

				dt.Rows.Add(dept.No, dept.ParentNo, dept.Name, "DEPT");
			}

			return BP.Tools.Json.ToJson(dt);
		}

		/// <summary>
		/// 获取指定部门下一级子部门及岗位列表
		/// </summary>
		/// <returns></returns>
		public string GetSubDeptsTable()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("NO", typeof(string));
			dt.Columns.Add("PARENTNO", typeof(string));
			dt.Columns.Add("NAME", typeof(string));
			dt.Columns.Add("TTYPE", typeof(string));

			string rootid = this.GetRequestVal("rootid");// context.Request.QueryString["rootid"];


			BP.GPM.Depts depts = new BP.GPM.Depts();
			depts.Retrieve(BP.GPM.DeptAttr.ParentNo, rootid);
			BP.GPM.Stations sts = new BP.GPM.Stations();
			sts.RetrieveAll();
			BP.GPM.DeptStations dss = new BP.GPM.DeptStations();
			dss.Retrieve(BP.GPM.DeptStationAttr.FK_Dept, rootid);
			BP.GPM.DeptEmps des = new BP.GPM.DeptEmps();
			des.Retrieve(BP.GPM.DeptEmpAttr.FK_Dept, rootid);
			BP.GPM.DeptEmpStations dess = new BP.GPM.DeptEmpStations();
			dess.Retrieve(BP.GPM.DeptEmpStationAttr.FK_Dept, rootid);
			BP.GPM.Station stt = null;
			BP.GPM.Emp emp = null;
			List<string> inemps = new List<string>();

			foreach (BP.GPM.Dept dept in depts)
			{
				//增加部门
				dt.Rows.Add(dept.No, dept.ParentNo, dept.Name, "DEPT");
			}

			//增加部门岗位
			foreach (BP.GPM.DeptStation ds in dss)
			{
				stt = sts.GetEntityByKey(ds.FK_Station) as BP.GPM.Station;

				if (stt == null) continue;

				dt.Rows.Add(ds.FK_Station, rootid, stt.Name, "STATION");
			}

			//增加没有岗位的人员
			foreach (BP.GPM.DeptEmp de in des)
			{
				if (dess.GetEntityByKey(BP.GPM.DeptEmpStationAttr.FK_Emp, de.FK_Emp) == null)
				{
					if (inemps.Contains(de.FK_Emp))
						continue;

					inemps.Add(de.FK_Emp);
				}
			}

			foreach (string inemp in inemps)
			{
				emp = new BP.GPM.Emp(inemp);
				dt.Rows.Add(emp.No, rootid, emp.Name, "EMP");
			}

			return BP.Tools.Json.ToJson(dt);
		}

		#region 主页.
		/// <summary>
		/// 初始化登录界面.
		/// </summary>
		/// <returns></returns>
		public string Default_Init()
		{
			try
			{
				//如果登录信息丢失了,就让其重新登录一次.
				if (DataType.IsNullOrEmpty(BP.Web.WebUser.NoOfRel) == true)
				{
					string userNo = this.GetRequestVal("UserNo");
					string sid = this.GetRequestVal("SID");
					BP.WF.Dev2Interface.Port_LoginBySID(userNo, sid);
				}

				if (BP.Web.WebUser.IsAdmin == false)
					return "url@Login.htm?DoType=Logout&Err=NoAdminUsers";

				//如果没有流程表，就执行安装.
				if (BP.DA.DBAccess.IsExitsObject("WF_Flow") == false)
					return "url@../DBInstall.htm";

				Hashtable ht = new Hashtable();
				if (BP.WF.Glo.OSModel == OSModel.OneOne)
					ht.Add("OSModel", "0");
				else
					ht.Add("OSModel", "1");

				//把系统信息加入里面去.
				ht.Add("SysNo", SystemConfig.SysNo);
				ht.Add("SysName", SystemConfig.SysName);

				ht.Add("CustomerNo", SystemConfig.CustomerNo);
				ht.Add("CustomerName", SystemConfig.CustomerName);

				//集成的平台.
				ht.Add("RunOnPlant", SystemConfig.RunOnPlant);

				try
				{
					// 执行升级
					string str = BP.WF.Glo.UpdataCCFlowVer();
					if (str == null)
						str = "";
					ht.Add("Msg", str);
				}
				catch (Exception ex)
				{
					return "err@" + ex.Message;
				}

				//生成Json.
				return BP.Tools.Json.ToJsonEntityModel(ht);
			}
			catch (Exception ex)
			{
				return "err@初始化界面期间出现如下错误:" + ex.Message;
			}
		}
		#endregion

		#region 登录窗口.
		public string Login_InitInfo()
		{
			Hashtable ht = new Hashtable();
			ht.Add("SysNo", SystemConfig.SysNo);
			ht.Add("SysName", SystemConfig.SysName);

			return BP.Tools.Json.ToJson(ht);
		}
		/// <summary>
		/// 初始化登录界面.
		/// </summary>
		/// <returns></returns>
		public string Login_Init()
		{

			//检查数据库连接.
			try
			{
				DBAccess.TestIsConnection();
			}
			catch (Exception ex)
			{
				return "err@异常信息:" + ex.Message;
			}


			//检查是否缺少Port_Emp 表，如果没有就是没有安装.
			if (DBAccess.IsExitsObject("Port_Emp") == false && DBAccess.IsExitsObject("WF_Flow") == false)
				return "url@../DBInstall.htm";

			////让admin登录
			//if (DataType.IsNullOrEmpty(BP.Web.WebUser.No) || BP.Web.WebUser.IsAdmin == false)
			//    return "url@Login.htm?DoType=Logout";

			//如果没有流程表，就执行安装.
			if (BP.DA.DBAccess.IsExitsObject("WF_Flow") == false)
				return "url@../DBInstall.htm";

			//是否需要自动登录
			string userNo = this.GetRequestVal("UserNo");
			string sid = this.GetRequestVal("SID");

			if (sid != null && userNo != null)
			{
				/*  */
				try
				{
					string str = BP.WF.Glo.UpdataCCFlowVer();

					BP.WF.Dev2Interface.Port_LoginBySID(userNo, sid);
					if (this.FK_Flow == null)
						return "url@Default.htm?UserNo=" + userNo + "&Key=" + DateTime.Now.ToBinary();
					else
						return "url@Designer.htm?UserNo=" + userNo + "&FK_Flow=" + this.FK_Flow + "&Key=" + DateTime.Now.ToBinary();
				}
				catch (Exception ex)
				{
					return "err@登录失败" + ex.Message;
				}
			}

			try
			{
				// 执行升级
				string str = BP.WF.Glo.UpdataCCFlowVer();
				if (str == null)
					str = "ccbpm 准备完毕,欢迎登录,当前小版本号为:" + BP.WF.Glo.Ver;

				return str;
				//Hashtable ht = new Hashtable();
				//ht.Add("Msg", str);
				//ht.Add("Title", SystemConfig.SysName);
				//return BP.Tools.Json.ToJson(ht);

			}
			catch (Exception ex)
			{
				string msg = "err@升级失败(ccbpm有自动修复功能,您可以刷新一下系统会自动创建字段,刷新多次扔解决不了问题,请反馈给我们.www.ccflow.org)";
				msg += "@系统信息:" + ex.Message;
				return msg;
			}
		}
		//流程设计器登陆前台，转向规则，判断是否为天业BPM
		public string Login_Redirect()
		{
			if (SystemConfig.CustomerNo == "TianYe")
				return "url@../../../BPM/pages/login.html";

			return "url@../../AppClassic/Login.htm?DoType=Logout";
		}
		/// <summary>
		/// 提交
		/// </summary>
		/// <returns></returns>
		public string Login_Submit()
		{
			string[] para = new string[0];
			BP.Port.Emp emp = new BP.Port.Emp();
			emp.No = this.GetRequestVal("TB_No").Trim();
			if (emp.RetrieveFromDBSources() == 0)
				return "err@用户名或密码错误.";
			//return BP.WF.Glo.lang("invalid_username_or_pwd", para);

			if (emp.No != "admin")
			{
				//检查是否是管理员？
				BP.WF.Port.AdminEmp adminEmp = new Port.AdminEmp();
				adminEmp.No = emp.No;
				if (adminEmp.RetrieveFromDBSources() == 0)
					return "err@您非管理员用户，不能登录.";
				//return BP.WF.Glo.lang("no_permission_login_1", para);

				if (adminEmp.IsAdmin == false)
					return "err@您非管理员用户或已被禁用,不能登录,请联系管理员初始化账户.";
				//return BP.WF.Glo.lang("no_permission_login_2", para);

				if (string.IsNullOrWhiteSpace(adminEmp.RootOfFlow) == true)
					return "err@二级管理员用户没有设置流程树的权限..";
				//return BP.WF.Glo.lang("secondary_user_no_permission_wf_tree", para);
			}

			string pass = this.GetRequestVal("TB_PW").Trim();
			if (emp.CheckPass(pass) == false)
				return "err@用户名或密码错误.";
			//return BP.WF.Glo.lang("invalid_username_or_pwd", para);

			//让其登录.
			BP.WF.Dev2Interface.Port_Login(emp.No);
			return "url@Default.htm?SID=" + emp.SID + "&UserNo=" + emp.No;
		}
		#endregion 登录窗口.



		#region 流程相关 Flow
		/// <summary>
		/// 获取流程所有元素
		/// </summary>
		/// <returns>json data</returns>
		public string Flow_AllElements_ResponseJson()
		{
			BP.WF.Flow flow = new BP.WF.Flow();
			flow.No = this.FK_Flow;
			flow.RetrieveFromDBSources();

			DataSet ds = new DataSet();
			DataTable dtNodes = DBAccess.RunSQLReturnTable("SELECT NODEID,NAME,X,Y,RUNMODEL FROM WF_NODE WHERE FK_FLOW='" + this.FK_Flow + "'");
			dtNodes.TableName = "Nodes";
			ds.Tables.Add(dtNodes);

			DataTable dtDirection = DBAccess.RunSQLReturnTable("SELECT NODE,TONODE FROM WF_DIRECTION WHERE FK_FLOW='" + this.FK_Flow + "'");
			dtDirection.TableName = "Direction";
			ds.Tables.Add(dtDirection);

			DataTable dtLabNote = DBAccess.RunSQLReturnTable("SELECT MYPK,NAME,X,Y FROM WF_LABNOTE WHERE FK_FLOW='" + this.FK_Flow + "'");
			dtLabNote.TableName = "LabNote";
			ds.Tables.Add(dtLabNote);


			// return BP.Tools.Json.DataSetToJson(ds, false);
			return BP.Tools.Json.ToJson(ds);
		}
		#endregion end Flow

		#region 节点相关 Nodes
		/// <summary>
		/// 创建流程节点并返回编号
		/// </summary>
		/// <returns></returns>
		public string CreateNode()
		{
			try
			{
				string FK_Flow = this.GetValFromFrmByKey("FK_Flow");
				string figureName = this.GetValFromFrmByKey("FigureName");
				string x = this.GetValFromFrmByKey("x");
				string y = this.GetValFromFrmByKey("y");
				int iX = 20;
				int iY = 20;
				if (!DataType.IsNullOrEmpty(x)) iX = (int)double.Parse(x);
				if (!DataType.IsNullOrEmpty(y)) iY = (int)double.Parse(y);

				int nodeId = BP.WF.Template.TemplateGlo.NewNode(FK_Flow, iX, iY);

				BP.WF.Node node = new BP.WF.Node(nodeId);
				node.HisRunModel = Node_GetRunModelByFigureName(figureName);
				node.Update();

				Hashtable ht = new Hashtable();
				ht.Add("NodeID", node.NodeID);
				ht.Add("Name", node.Name);

				return BP.Tools.Json.ToJsonEntityModel(ht);
			}
			catch (Exception ex)
			{
				return "err@" + ex.Message;
			}
		}
		/// <summary>
		/// gen
		/// </summary>
		/// <param name="figureName"></param>
		/// <returns></returns>
		public BP.WF.RunModel Node_GetRunModelByFigureName(string figureName)
		{
			BP.WF.RunModel runModel = BP.WF.RunModel.Ordinary;
			switch (figureName)
			{
				case "NodeOrdinary":
					runModel = BP.WF.RunModel.Ordinary;
					break;
				case "NodeFL":
					runModel = BP.WF.RunModel.FL;
					break;
				case "NodeHL":
					runModel = BP.WF.RunModel.HL;
					break;
				case "NodeFHL":
					runModel = BP.WF.RunModel.FHL;
					break;
				case "NodeSubThread":
					runModel = BP.WF.RunModel.SubThread;
					break;
				default:
					runModel = BP.WF.RunModel.Ordinary;
					break;
			}
			return runModel;
		}
		/// <summary>
		/// 根据节点编号删除流程节点
		/// </summary>
		/// <returns>执行结果</returns>
		public string DeleteNode()
		{
			try
			{
				BP.WF.Node node = new BP.WF.Node();
				node.NodeID = this.FK_Node;
				if (node.RetrieveFromDBSources() == 0)
					return "err@删除失败,没有删除到数据，估计该节点已经别删除了.";

				if (node.IsStartNode == true)
					return "err@开始节点不允许被删除。";

				node.Delete();
				return "删除成功.";
			}
			catch (Exception ex)
			{
				return "err@" + ex.Message;
			}
		}
		/// <summary>
		/// 修改节点名称
		/// </summary>
		/// <returns></returns>
		public string Node_EditNodeName()
		{
			string FK_Node = this.GetValFromFrmByKey("NodeID");
			string NodeName = System.Web.HttpContext.Current.Server.UrlDecode(this.GetValFromFrmByKey("NodeName"));

			BP.WF.Node node = new BP.WF.Node();
			node.NodeID = int.Parse(FK_Node);
			int iResult = node.RetrieveFromDBSources();
			if (iResult > 0)
			{
				node.Name = NodeName;
				node.Update();
				return "@修改成功.";
			}

			return "err@修改节点失败，请确认该节点是否存在？";
		}
		/// <summary>
		/// 修改节点运行模式
		/// </summary>
		/// <returns></returns>
		public string Node_ChangeRunModel()
		{
			string runModel = GetValFromFrmByKey("RunModel");
			BP.WF.Node node = new BP.WF.Node(this.FK_Node);
			//节点运行模式
			switch (runModel)
			{
				case "NodeOrdinary":
					node.HisRunModel = BP.WF.RunModel.Ordinary;
					break;
				case "NodeFL":
					node.HisRunModel = BP.WF.RunModel.FL;
					break;
				case "NodeHL":
					node.HisRunModel = BP.WF.RunModel.HL;
					break;
				case "NodeFHL":
					node.HisRunModel = BP.WF.RunModel.FHL;
					break;
				case "NodeSubThread":
					node.HisRunModel = BP.WF.RunModel.SubThread;
					break;
			}
			node.Update();

			return "设置成功.";
		}
		#endregion end Node

		#region CCBPMDesigner
		/// <summary>
		/// 获取用户信息
		/// </summary>
		/// <returns></returns>
		public string GetWebUserInfo()
		{
			if (WebUser.No == null)
				return "err@当前用户没有登录，请登录后再试。";

			Hashtable ht = new Hashtable();

			BP.Port.Emp emp = new BP.Port.Emp(WebUser.No);

			ht.Add("No", emp.No);
			ht.Add("Name", emp.Name);
			ht.Add("FK_Dept", emp.FK_Dept);
			ht.Add("SID", emp.SID);


			if (WebUser.No == "admin")
			{
				ht.Add("IsAdmin", "1");
				ht.Add("RootOfDept", "0");
				ht.Add("RootOfFlow", "F0");
				ht.Add("RootOfForm", "");
			}
			else
			{
				BP.WF.Port.AdminEmp aemp = new Port.AdminEmp();
				aemp.No = WebUser.No;

				if (aemp.RetrieveFromDBSources() == 0)
				{
					ht.Add("RootOfDept", "-9999");
					ht.Add("RootOfFlow", "-9999");
					ht.Add("RootOfForm", "-9999");
				}
				else
				{
					ht.Add("RootOfDept", aemp.RootOfDept);
					ht.Add("RootOfFlow", "F" + aemp.RootOfFlow);
					ht.Add("RootOfForm", aemp.RootOfForm);
				}
			}

			return BP.Tools.Json.ToJsonEntityModel(ht);
		}

		StringBuilder sbJson = new StringBuilder();
		/// <summary>
		/// 获取流程树数据
		/// </summary>
		/// <returns>返回结果Json,流程树</returns>
		public string GetFlowTreeTable()
		{
			string sql = @"SELECT * FROM (SELECT 'F'+No as NO,'F'+ParentNo PARENTNO, NAME, IDX, 1 ISPARENT,'FLOWTYPE' TTYPE, -1 DTYPE FROM WF_FlowSort
                           union 
                           SELECT NO, 'F'+FK_FlowSort as PARENTNO,(NO + '.' + NAME) as NAME,IDX,0 ISPARENT,'FLOW' TTYPE, 0 as DTYPE FROM WF_Flow) A  ORDER BY DTYPE, IDX ";

			if (BP.Sys.SystemConfig.AppCenterDBType == DBType.Oracle || BP.Sys.SystemConfig.AppCenterDBType == DBType.PostgreSQL)
			{
				sql = @"SELECT * FROM (SELECT 'F'||No as NO,'F'||ParentNo as PARENTNO,NAME, IDX, 1 ISPARENT,'FLOWTYPE' TTYPE,-1 DTYPE FROM WF_FlowSort
                        union 
                        SELECT NO, 'F'||FK_FlowSort as PARENTNO,NO||'.'||NAME as NAME,IDX,0 ISPARENT,'FLOW' TTYPE,0 as DTYPE FROM WF_Flow) A  ORDER BY DTYPE, IDX";
			}


			if (BP.Sys.SystemConfig.AppCenterDBType == DBType.MySQL)
			{
				sql = @"SELECT * FROM (SELECT CONCAT('F', No) NO, CONCAT('F', ParentNo) PARENTNO, NAME, IDX, 1 ISPARENT,'FLOWTYPE' TTYPE,-1 DTYPE FROM WF_FlowSort
                           union 
                           SELECT NO, CONCAT('F', FK_FlowSort) PARENTNO, CONCAT(NO, '.', NAME) NAME,IDX,0 ISPARENT,'FLOW' TTYPE, 0 as DTYPE FROM WF_Flow) A  ORDER BY DTYPE, IDX";
			}

			DataTable dt = DBAccess.RunSQLReturnTable(sql);


			if (BP.Sys.SystemConfig.AppCenterDBType == DBType.PostgreSQL)
			{
				dt.Columns["no"].ColumnName = "NO";
				dt.Columns["name"].ColumnName = "NAME";
				dt.Columns["parentno"].ColumnName = "PARENTNO";
				dt.Columns["idx"].ColumnName = "IDX";
				dt.Columns["isparent"].ColumnName = "ISPARENT";
				dt.Columns["ttype"].ColumnName = "TTYPE";
				dt.Columns["dtype"].ColumnName = "DTYPE";
			}

			//判断是否为空，如果为空，则创建一个流程根结点，added by liuxc,2016-01-24
			if (dt.Rows.Count == 0)
			{
				FlowSort fs = new FlowSort();
				fs.No = "99";
				fs.ParentNo = "0";
				fs.Name = "流程树";
				fs.Insert();

				dt.Rows.Add("F99", "F0", "流程树", 0, 1, "FLOWTYPE", -1);
			}
			else
			{
				DataRow[] drs = dt.Select("NAME='流程树'");
				if (drs.Length > 0 && !Equals(drs[0]["PARENTNO"], "F0"))
					drs[0]["PARENTNO"] = "F0";
			}


			if (WebUser.No != "admin")
			{
				BP.WF.Port.AdminEmp aemp = new Port.AdminEmp();
				aemp.No = WebUser.No;
				if (aemp.RetrieveFromDBSources() == 0)
					return "err@登录帐号错误.";

				if (aemp.IsAdmin == false)
					return "err@非管理员用户.";

				DataRow rootRow = dt.Select("PARENTNO='F0'")[0];
				DataRow newRootRow = dt.Select("NO='F" + aemp.RootOfFlow + "'")[0];

				newRootRow["PARENTNO"] = "F0";
				DataTable newDt = dt.Clone();
				newDt.Rows.Add(newRootRow.ItemArray);
				GenerChildRows(dt, newDt, newRootRow);
				dt = newDt;
			}

			string str = BP.Tools.Json.ToJson(dt);
			return str;
		}

		public void GenerChildRows(DataTable dt, DataTable newDt, DataRow parentRow)
		{
			DataRow[] rows = dt.Select("ParentNo='" + parentRow["NO"] + "'");
			foreach (DataRow r in rows)
			{
				newDt.Rows.Add(r.ItemArray);
				GenerChildRows(dt, newDt, r);
			}
		}

		public string GetBindingFormsTable()
		{
			string fk_flow = GetValFromFrmByKey("fk_flow");
			if (string.IsNullOrWhiteSpace(fk_flow))
				return "[]";

			StringBuilder sql = new StringBuilder();
			sql.AppendLine("SELECT wfn.FK_Frm NO,");
			sql.AppendLine("       smd.NAME,");
			sql.AppendLine("       NULL PARENTNO,");
			sql.AppendLine("       'FORM' TTYPE,");
			sql.AppendLine("       -1 DTYPE,");
			sql.AppendLine("       0 ISPARENT");
			sql.AppendLine("FROM   WF_FrmNode wfn");
			sql.AppendLine("       INNER JOIN Sys_MapData smd");
			sql.AppendLine("            ON  smd.No = wfn.FK_Frm");
			sql.AppendLine("WHERE  wfn.FK_Flow = '{0}'");
			sql.AppendLine("       AND wfn.FK_Node = (");
			sql.AppendLine("               SELECT wn.NodeID");
			sql.AppendLine("               FROM   WF_Node wn");
			sql.AppendLine("               WHERE  wn.FK_Flow = '{0}' AND wn.NodePosType = 0");
			sql.AppendLine("           )");

			DataTable dt = DBAccess.RunSQLReturnTable(string.Format(sql.ToString(), fk_flow));
			return BP.Tools.Json.ToJson(dt);
		}

		public string GetFormTreeTable()
		{
			#region 检查数据是否符合规范.
			string rootNo = DBAccess.RunSQLReturnStringIsNull("SELECT No FROM Sys_FormTree WHERE ParentNo='' OR ParentNo IS NULL", null);
			if (DataType.IsNullOrEmpty(rootNo) == false)
			{
				//删除垃圾数据.
				DBAccess.RunSQL(string.Format("DELETE FROM Sys_FormTree WHERE No='{0}'", rootNo));
			}
			//检查根目录是否符合规范.
			FrmTree ft = new FrmTree();
			ft.No = "1";
			if (ft.RetrieveFromDBSources() == 0)
			{
				ft.Name = "表单库";
				ft.ParentNo = "0";
				ft.Insert();
			}
			if (ft.ParentNo.Equals("0") == false)
			{
				ft.ParentNo = "0";
				ft.Update();
			}
			#endregion 检查数据是否符合规范.

			//组织数据源.
			string sqls = "";

			if (SystemConfig.AppCenterDBType == DBType.Oracle)
			{
				sqls = "SELECT No \"No\", ParentNo \"ParentNo\",Name \"Name\", Idx \"Idx\", 1 \"IsParent\", 'FORMTYPE' \"TType\" FROM Sys_FormTree ORDER BY Idx ASC ; ";
				sqls += "SELECT No \"No\", FK_FormTree as \"ParentNo\", Name \"Name\",Idx \"Idx\", 0 \"IsParent\", 'FORM' \"TType\" FROM Sys_MapData  WHERE AppType=0 AND FK_FormTree IN (SELECT No FROM Sys_FormTree) ORDER BY Idx ASC";
			}
			else
			{
				sqls = "SELECT No,ParentNo,Name, Idx, 1 IsParent, 'FORMTYPE' TType FROM Sys_FormTree ORDER BY Idx ASC ; ";
				sqls += "SELECT No, FK_FormTree as ParentNo,Name,Idx,0 IsParent, 'FORM' TType FROM Sys_MapData  WHERE AppType=0 AND FK_FormTree IN (SELECT No FROM Sys_FormTree) ORDER BY Idx ASC";
			}

			DataSet ds = DBAccess.RunSQLReturnDataSet(sqls);



			//获得表单数据.
			DataTable dtSort = ds.Tables[0]; //类别表.
			DataTable dtForm = ds.Tables[1].Clone(); //表单表,这个是最终返回的数据.

			if (SystemConfig.AppCenterDBType == DBType.PostgreSQL)
			{
				dtForm.Columns["no"].ColumnName = "No";
				dtForm.Columns["name"].ColumnName = "Name";
				dtForm.Columns["parentno"].ColumnName = "ParentNo";
				dtForm.Columns["idx"].ColumnName = "Idx";
				dtForm.Columns["isparent"].ColumnName = "IsParent";
				dtForm.Columns["ttype"].ColumnName = "TType";
			}

			//增加顶级目录.
			DataRow[] rowsOfSort = dtSort.Select("ParentNo='0'");
			DataRow drFormRoot = dtForm.NewRow();
			drFormRoot[0] = rowsOfSort[0]["No"];
			drFormRoot[1] = "0";
			drFormRoot[2] = rowsOfSort[0]["Name"];
			drFormRoot[3] = rowsOfSort[0]["Idx"];
			drFormRoot[4] = rowsOfSort[0]["IsParent"];
			drFormRoot[5] = rowsOfSort[0]["TType"];
			dtForm.Rows.Add(drFormRoot); //增加顶级类别..

			//把类别数据组装到form数据里.
			foreach (DataRow dr in dtSort.Rows)
			{
				DataRow drForm = dtForm.NewRow();
				drForm[0] = dr["No"];
				drForm[1] = dr["ParentNo"];
				drForm[2] = dr["Name"];
				drForm[3] = dr["Idx"];
				drForm[4] = dr["IsParent"];
				drForm[5] = dr["TType"];
				dtForm.Rows.Add(drForm); //类别.
			}

			foreach (DataRow row in ds.Tables[1].Rows)
			{
				dtForm.Rows.Add(row.ItemArray);
			}

			if (WebUser.No.Equals("admin") == false)
			{
				BP.WF.Port.AdminEmp aemp = new Port.AdminEmp();
				aemp.No = WebUser.No;
				aemp.RetrieveFromDBSources();

				if (aemp.UserType != 1)
					return "err@您[" + WebUser.No + "]已经不是二级管理员了.";
				if (aemp.RootOfForm == "")
					return "err@没有给二级管理员[" + WebUser.No + "]设置表单树的权限...";

				DataRow[] rootRows = dtForm.Select("No='" + aemp.RootOfForm + "'");
				DataRow newRootRow = rootRows[0];

				newRootRow["ParentNo"] = "0";
				DataTable newDt = dtForm.Clone();
				newDt.Rows.Add(newRootRow.ItemArray);

				GenerChildRows(dtForm, newDt, newRootRow);
				dtForm = newDt;
			}

			String str = BP.Tools.Json.ToJson(dtForm);
			return str;
		}


		public string GetStructureTreeTable()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("NO", typeof(string));
			dt.Columns.Add("PARENTNO", typeof(string));
			dt.Columns.Add("NAME", typeof(string));
			dt.Columns.Add("TTYPE", typeof(string));


			BP.GPM.Depts depts = new BP.GPM.Depts();
			depts.RetrieveAll();
			BP.GPM.Stations sts = new BP.GPM.Stations();
			sts.RetrieveAll();
			BP.GPM.Emps emps = new BP.GPM.Emps();
			emps.RetrieveAll(BP.WF.Port.EmpAttr.Name);
			BP.GPM.DeptStations dss = new BP.GPM.DeptStations();
			dss.RetrieveAll();
			BP.GPM.DeptEmpStations dess = new BP.GPM.DeptEmpStations();
			dess.RetrieveAll();
			BP.GPM.Station stt = null;
			BP.GPM.Emp empt = null;

			foreach (BP.GPM.Dept dept in depts)
			{
				//增加部门
				dt.Rows.Add(dept.No, dept.ParentNo, dept.Name, "DEPT");

				//增加部门岗位
				dss.Retrieve(BP.GPM.DeptStationAttr.FK_Dept, dept.No);
				foreach (BP.GPM.DeptStation ds in dss)
				{
					stt = sts.GetEntityByKey(ds.FK_Station) as BP.GPM.Station;

					if (stt == null) continue;

					dt.Rows.Add(dept.No + "|" + ds.FK_Station, dept.No, stt.Name, "STATION");

					//增加部门岗位人员
					dess.Retrieve(BP.GPM.DeptEmpStationAttr.FK_Dept, dept.No, BP.GPM.DeptEmpStationAttr.FK_Station,
								  ds.FK_Station);

					foreach (BP.GPM.DeptEmpStation des in dess)
					{
						empt = emps.GetEntityByKey(des.FK_Emp) as BP.GPM.Emp;

						if (empt == null) continue;

						dt.Rows.Add(dept.No + "|" + ds.FK_Station + "|" + des.FK_Emp, dept.No + "|" + ds.FK_Station,
									empt.Name, "EMP");
					}
				}
			}

			return BP.Tools.Json.ToJson(dt);
		}

		/// <summary>
		/// 获取设计器 - 系统维护菜单数据
		/// 系统维护管理员菜单 需要翻译
		/// </summary>
		/// <returns></returns>
		public string GetTreeJson_AdminMenu()
		{
			//查询全部.
			AdminMenuGroups groups = new AdminMenuGroups();
			groups.RetrieveAll();

			AdminMenus menus = new AdminMenus();
			menus.RetrieveAll();

			// 定义容器.
			AdminMenus newMenus = new AdminMenus();

			foreach (AdminMenuGroup menu in groups)
			{
				//是否可以使用？
				if (menu.IsCanUse(WebUser.No) == false)
					continue;

				AdminMenu newMenu = new AdminMenu();
				newMenu.No = menu.No;
				newMenu.Name = menu.Name;
				newMenu.GroupNo = "0";
				newMenu.For = menu.For;
				newMenu.Url = "";
				newMenus.Add(newMenu);
			}

			foreach (AdminMenu menu in menus)
			{
				//是否可以使用？
				if (menu.IsCanUse(WebUser.No) == false)
					continue;

				newMenus.Add(menu);
			}
			//添加默认，无权限
			if (newMenus.Count == 0)
			{
				AdminMenu menu = new AdminMenu();
				menu.No = "1";
				menu.GroupNo = "0";
				menu.Name = "无权限";
				menu.Url = "";
				newMenus.Add(menu);
			}
			DataTable dt = newMenus.ToDataTable();
			return BP.Tools.Json.ToJson(newMenus.ToDataTable());
		}

		/// <summary>
		/// 根据DataTable生成Json树结构
		/// </summary>
		public string GetTreeJsonByTable(DataTable tabel, object pId, string rela = "ParentNo", string idCol = "No", string txtCol = "Name", string IsParent = "IsParent", string sChecked = "", string[] attrFields = null)
		{
			string treeJson = string.Empty;

			if (tabel.Rows.Count > 0)
			{
				sbJson.Append("[");
				string filer = string.Empty;
				if (pId.ToString() == "")
				{
					filer = string.Format("{0} is null or {0}='-1' or {0}='0' or {0}='F0'", rela);
				}
				else
				{
					filer = string.Format("{0}='{1}'", rela, pId);
				}

				DataRow[] rows = tabel.Select(filer, idCol);
				if (rows.Length > 0)
				{
					for (int i = 0; i < rows.Length; i++)
					{
						DataRow row = rows[i];

						string jNo = row[idCol] as string;
						string jText = row[txtCol] as string;
						if (jText.Length > 25)
							jText = jText.Substring(0, 25) + "<img src='../Scripts/easyUI/themes/icons/add2.png' onclick='moreText(" + jNo + ")'/>";

						string jIsParent = row[IsParent].ToString();
						string jState = "1".Equals(jIsParent) ? "open" : "closed";
						jState = "open".Equals(jState) && i == 0 ? "open" : "closed";

						DataRow[] rowChild = tabel.Select(string.Format("{0}='{1}'", rela, jNo));
						string tmp = "{\"id\":\"" + jNo + "\",\"text\":\"" + jText;

						//增加自定义attributes列，added by liuxc,2015-10-6
						var attrs = string.Empty;
						if (attrFields != null && attrFields.Length > 0)
						{
							foreach (var field in attrFields)
							{
								if (!tabel.Columns.Contains(field)) continue;
								if (DataType.IsNullOrEmpty(row[field].ToString()))
								{
									attrs += ",\"" + field + "\":\"\"";
									continue;
								}
								attrs += ",\"" + field + "\":" +
										 (tabel.Columns[field].DataType == typeof(string)
											  ? string.Format("\"{0}\"", row[field])
											  : row[field]);
							}
						}

						if ("0".Equals(pId.ToString()) || row[rela].ToString() == "F0")
						{
							tmp += "\",\"attributes\":{\"IsParent\":\"" + jIsParent + "\",\"IsRoot\":\"1\"" + attrs + "}";
						}
						else
						{
							tmp += "\",\"attributes\":{\"IsParent\":\"" + jIsParent + "\"" + attrs + "}";
						}

						if (rowChild.Length > 0)
						{
							tmp += ",\"checked\":" + sChecked.Contains("," + jNo + ",").ToString().ToLower()
								+ ",\"state\":\"" + jState + "\"";
						}
						else
						{
							tmp += ",\"checked\":" + sChecked.Contains("," + jNo + ",").ToString().ToLower();
						}

						sbJson.Append(tmp);
						if (rowChild.Length > 0)
						{
							sbJson.Append(",\"children\":");
							GetTreeJsonByTable(tabel, jNo, rela, idCol, txtCol, IsParent, sChecked, attrFields);
						}

						sbJson.Append("},");
					}
					sbJson = sbJson.Remove(sbJson.Length - 1, 1);
				}
				sbJson.Append("]");
				treeJson = sbJson.ToString();
			}
			return treeJson;
		}
		public string NewFlow()
		{
			try
			{
				string[] ps = this.GetRequestVal("paras").Split(',');
				if (ps.Length != 6)
					throw new Exception("@创建流程参数错误");

				string fk_floSort = ps[0]; //类别编号.
				fk_floSort = fk_floSort.Replace("F", "");//传入的编号多出F符号，需要替换掉

				string flowName = ps[1]; // 流程名称.
				DataStoreModel dataSaveModel = (DataStoreModel)int.Parse(ps[2]); //数据保存方式。
				string pTable = ps[3]; // 物理表名.
				string flowMark = ps[4]; // 流程标记.
				string flowVer = ps[5]; // 流程版本.

				string flowNo = BP.WF.Template.TemplateGlo.NewFlow(fk_floSort, flowName, dataSaveModel, pTable, flowMark, flowVer);

				//清空WF_Emp 的StartFlows
				DBAccess.RunSQL("UPDATE  WF_Emp Set StartFlows =''");
				return flowNo;
			}
			catch (Exception ex)
			{
				return "err@" + ex.Message;
			}
		}
		/// <summary>
		/// 上移流程
		/// </summary>
		/// <returns></returns>
		public string MoveUpFlow()
		{
			Flow flow = new Flow(this.FK_Flow);
			flow.DoUp();
			return flow.No;
		}
		/// <summary>
		/// 下移流程
		/// </summary>
		/// <returns></returns>
		public string MoveDownFlow()
		{
			Flow flow = new Flow(this.FK_Flow);
			flow.DoDown();
			return flow.No;
		}
		/// <summary>
		/// 删除流程类别.
		/// </summary>
		/// <returns></returns>
		public string DelFlowSort()
		{
			string fk_flowSort = this.GetRequestVal("FK_FlowSort").Replace("F", "");

			FlowSort fs = new FlowSort();
			fs.No = fk_flowSort;

			//检查是否有流程？
			Paras ps = new Paras();
			ps.SQL = "SELECT COUNT(*) FROM WF_Flow WHERE FK_FlowSort=" + SystemConfig.AppCenterDBVarStr + "fk_flowSort";
			ps.Add("fk_flowSort", fk_flowSort);
			//string sql = "SELECT COUNT(*) FROM WF_Flow WHERE FK_FlowSort='" + fk_flowSort + "'";
			if (DBAccess.RunSQLReturnValInt(ps) != 0)
				return "err@该目录下有流程，您不能删除。";

			//检查是否有子目录？
			ps = new Paras();
			ps.SQL = "SELECT COUNT(*) FROM WF_FlowSort WHERE ParentNo=" + SystemConfig.AppCenterDBVarStr + "ParentNo";
			ps.Add("ParentNo", fk_flowSort);
			//sql = "SELECT COUNT(*) FROM WF_FlowSort WHERE ParentNo='" + fk_flowSort + "'";
			if (DBAccess.RunSQLReturnValInt(ps) != 0)
				return "err@该目录下有子目录，您不能删除。";

			fs.Delete();

			return "删除成功.";
		}
		/// <summary>
		/// 新建同级流程类别 对照需要翻译
		/// </summary>
		/// <returns></returns>
		public string NewSameLevelFlowSort()
		{
			FlowSort fs = null;
			fs = new FlowSort(this.No.Replace("F", "")); //传入的编号多出F符号，需要替换掉.

			string orgNo = fs.OrgNo; //记录原来的组织结构编号. 对照需要翻译

			string sameNodeNo = fs.DoCreateSameLevelNode().No;
			fs = new FlowSort(sameNodeNo);
			fs.Name = this.Name;
			fs.OrgNo = orgNo; // 组织结构编号. 对照需要翻译
			fs.Update();
			return "F" + fs.No;
		}
		/// <summary>
		/// 新建下级类别. 
		/// </summary>
		/// <returns></returns>
		public string NewSubFlowSort()
		{
			FlowSort fsSub = new FlowSort(this.No.Replace("F", ""));//传入的编号多出F符号，需要替换掉.
			string orgNo = fsSub.OrgNo; //记录原来的组织结构编号. 对照需要翻译

			string subNodeNo = fsSub.DoCreateSubNode().No;
			FlowSort subFlowSort = new FlowSort(subNodeNo);
			subFlowSort.Name = this.Name;
			subFlowSort.OrgNo = orgNo; // 组织结构编号. 对照需要翻译.
			subFlowSort.Update();
			return "F" + subFlowSort.No;
		}
		/// <summary>
		/// 上移流程类别
		/// </summary>
		/// <returns></returns>
		public string MoveUpFlowSort()
		{
			string fk_flowSort = this.GetRequestVal("FK_FlowSort").Replace("F", "");
			FlowSort fsSub = new FlowSort(fk_flowSort);//传入的编号多出F符号，需要替换掉
			fsSub.DoUp();
			return "F" + fsSub.No;
		}
		/// <summary>
		/// 下移流程类别
		/// </summary>
		/// <returns></returns>
		public string MoveDownFlowSort()
		{
			string fk_flowSort = this.GetRequestVal("FK_FlowSort").Replace("F", "");
			FlowSort fsSub = new FlowSort(fk_flowSort);//传入的编号多出F符号，需要替换掉
			fsSub.DoDown();
			return "F" + fsSub.No;
		}

		/// <summary>
		/// 表单树 - 编辑表单类别
		/// </summary>
		/// <returns></returns>
		public string CCForm_EditCCFormSort()
		{
			SysFormTree formTree = new SysFormTree(this.No);
			formTree.Name = this.Name;
			formTree.Update();
			return this.No;
		}
		/// <summary>
		/// 表单树 - 删除表单类别
		/// </summary>
		/// <returns></returns>
		public string CCForm_DelFormSort()
		{
			SysFormTree formTree = new SysFormTree(this.No);

			//检查是否有子类别？
			Paras ps = new Paras();
			ps.SQL = "SELECT COUNT(*) FROM Sys_FormTree WHERE ParentNo=" + SystemConfig.AppCenterDBVarStr + "ParentNo";
			ps.Add("ParentNo", this.No);
			//string sql = "SELECT COUNT(*) FROM Sys_FormTree WHERE ParentNo='" + this.No + "'";
			if (DBAccess.RunSQLReturnValInt(ps) != 0)
				return "err@该目录下有子类别，您不能删除。";

			//检查是否有表单？
			ps = new Paras();
			ps.SQL = "SELECT COUNT(*) FROM Sys_MapData WHERE FK_FormTree=" + SystemConfig.AppCenterDBVarStr + "FK_FormTree";
			ps.Add("FK_FormTree", this.No);
			//sql = "SELECT COUNT(*) FROM Sys_MapData WHERE FK_FormTree='" + this.No + "'";
			if (DBAccess.RunSQLReturnValInt(ps) != 0)
				return "err@该目录下有表单，您不能删除。";

			formTree.Delete();
			return "删除成功";
		}
		/// <summary>
		/// 表单树-上移表单类别
		/// </summary>
		/// <returns></returns>
		public string CCForm_MoveUpCCFormSort()
		{
			SysFormTree formTree = new SysFormTree(this.No);
			formTree.DoUp();
			return formTree.No;
		}
		/// <summary>
		/// 表单树-下移表单类别
		/// </summary>
		/// <returns></returns>
		public string CCForm_MoveDownCCFormSort()
		{
			SysFormTree formTree = new SysFormTree(this.No);
			formTree.DoDown();
			return formTree.No;
		}

		/// <summary>
		/// 表单树-上移表单
		/// </summary>
		/// <returns></returns>
		public string CCForm_MoveUpCCFormTree()
		{
			MapData mapData = new MapData(this.FK_MapData);
			mapData.DoUp();
			return mapData.No;
		}
		/// <summary>
		/// 表单树-下移表单
		/// </summary>
		/// <returns></returns>
		public string CCForm_MoveDownCCFormTree()
		{
			MapData mapData = new MapData(this.FK_MapData);
			mapData.DoOrderDown();
			return mapData.No;
		}

		/// <summary>
		/// 表单树 - 删除表单
		/// </summary>
		/// <returns></returns>
		public string CCForm_DeleteCCFormMapData()
		{
			try
			{
				MapData mapData = new MapData(this.FK_MapData);
				mapData.Delete();
				return "删除成功.";
			}
			catch (Exception ex)
			{
				return "err@" + ex.Message;
			}
		}

		public string EditFlowSort()
		{
			FlowSort fs = new FlowSort();//传入的编号多出F符号，需要替换掉
			fs.No = this.No.TrimStart('F');
			fs.RetrieveFromDBSources();
			fs.Name = this.Name;
			fs.Update();
			return fs.No;
		}

		/// <summary>
		/// 让admin 登陆
		/// </summary>
		/// <param name="lang">当前的语言</param>
		/// <returns>成功则为空，有异常时返回异常信息</returns>
		public string LetAdminLogin(string empNo, bool islogin)
		{
			try
			{
				if (islogin)
				{
					BP.Port.Emp emp = new BP.Port.Emp(empNo);
					WebUser.SignInOfGener(emp);
				}
			}
			catch (Exception ex)
			{
				return "err@" + ex.Message;
			}
			return "@登录成功.";
		}
		#endregion

	}
}
