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

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 初始化函数
    /// </summary>
    public class WF_Admin_CCBPMDesigner : WebContralBase
    {
        #region 执行.
        public HttpContext context = null;

        /// <summary>
        /// 枚举值
        /// </summary>
        public string EnumKey
        {
            get
            {
                string str = GetRequestVal("EnumKey");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 实体 EnsName
        /// </summary>
        public string EnsName
        {
            get
            {
                string str = GetRequestVal("EnsName");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        public string SFTable
        {
            get
            {
                string str = GetRequestVal("SFTable");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 表单外键
        /// </summary>
        public string FK_MapData
        {
            get
            {
                string str = GetRequestVal("FK_MapData");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 公共方法获取值
        /// </summary>
        /// <param name="param">参数名</param>
        /// <returns></returns>
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(context.Request[param], System.Text.Encoding.UTF8);
        }
        /// <summary>
        /// 获得表单的属性.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValFromFrmByKey(string key)
        {
            string val = getUTF8ToString(key);
            if (val == null)
                return null;
            val = val.Replace("'", "~");
            return val;
        }
        public int GetValIntFromFrmByKey(string key)
        {
            return int.Parse(this.GetValFromFrmByKey(key));
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
        /// <param name="key">参数名,可以从 form 与request 里面获取.</param>
        /// <returns></returns>
        public string GetRequestVal(string key)
        {
            string val = context.Request[key];
            if (val == null)
                val = context.Request.Form[key];
            if (val == null)
                return null;
            return HttpUtility.UrlDecode(val, System.Text.Encoding.UTF8);
        }
        #endregion 执行.

        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Admin_CCBPMDesigner(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        /// <summary>
        /// 流程信息.
        /// </summary>
        /// <returns></returns>
        public string Flows_Init()
        {
            DataSet ds = new DataSet();

            FlowSorts sorts = new FlowSorts();
            sorts.RetrieveAll();

            //把类别数据放入.
            DataTable dt = sorts.ToDataTableField();
            dt.TableName = "Sorts";
            ds.Tables.Add(dt);

            Flows fls = new Flows();
            fls.RetrieveAll();

            dt = fls.ToDataTableField();
            dt.TableName = "Flows";

            dt.Columns.Add("NumOfRuning", typeof(int)); // 耗时分析.
            dt.Columns.Add("NumOfComplete", typeof(int));
            dt.Columns.Add("NumOfEtc", typeof(int));
            dt.Columns.Add("NumOfOverTime", typeof(int));

            foreach (DataRow dr in dt.Rows)
            {
                string no = dr["No"].ToString();
                dr["NumOfRuning"] = DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE FK_Flow='" + no + "' AND WFSta=1");
                dr["NumOfComplete"] = DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE FK_Flow='" + no + "' AND WFSta=1");
                dr["NumOfEtc"] = DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE FK_Flow='" + no + "' AND WFSta=1");
                dr["NumOfOverTime"] = DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE FK_Flow='" + no + "' AND WFSta=1");
            }
            ds.Tables.Add(dt);
            return BP.Tools.Json.ToJson(ds);
        }

        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            string msg = "";
            try
            {
                switch (this.DoType)
                {
                    case "Logout": //获得枚举列表的JSON.
                        BP.WF.Dev2Interface.Port_SigOut();
                        break;
                    case "load"://获取流程图表数据
                        msg = Flow_LoadFlowJsonData();
                        break;
                    case "save"://保存流程图
                        msg = Flow_Save();
                        break;
                    case "saveAs"://另存为流程
                        msg = Flow_SaveAs();
                        break;
                    case "editnodename"://修改节点名称
                        msg = Node_EditNodeName();
                        break;

                    case "GetFormTree"://获取表单库数据
                        msg = GetFormTreeTable();//GetFormTree();
                        break;
                    case "GetSrcTree"://获取数据源数据
                        msg = GetSrcTreeTable();
                        break;
                    case "GetStructureTree"://获取组织结构数据
                        msg = GetStructureTreeTable();
                        break;
                    case "GetStructureTreeRoot"://获取组织结构根结点数据
                        msg = GetStructureTreeRootTable();
                        break;
                    case "GetSubDepts": //获取指定部门下一级子部门及岗位列表
                        msg = GetSubDeptsTable();
                        break;
                    case "GetEmpsByStation":    //根据部门、岗位获取人员列表
                        msg = GetEmpsByStationTable();
                        break;
                    case "GetBindingForms"://获取流程绑定表单列表
                        msg = GetBindingFormsTable();
                        break;
                    case "Do"://公共方法
                        msg = Do();
                        break;
                    case "LetLogin":    //使管理员登录
                        msg = string.IsNullOrWhiteSpace(WebUser.No) ? LetAdminLogin(getUTF8ToString("userNo"), true) : string.Empty;
                        break;
                    default:
                        msg = "err@没有判断的标记:" + this.DoType;
                        break;
                }
            }
            catch (Exception ex)
            {
                msg = "err@" + ex.Message;
            }
            return msg;
        }
        #endregion 执行父类的重写方法.

        /// <summary>
        /// 根据部门、岗位获取人员列表
        /// </summary>
        /// <returns></returns>
        public string GetEmpsByStationTable()
        {
            string deptid = context.Request.QueryString["deptid"];
            string stid = context.Request.QueryString["stationid"];

            if (string.IsNullOrWhiteSpace(deptid) || string.IsNullOrWhiteSpace(stid))
                return "[]";

            DataTable dt = new DataTable();
            dt.Columns.Add("NO", typeof(string));
            dt.Columns.Add("PARENTNO", typeof(string));
            dt.Columns.Add("NAME", typeof(string));
            dt.Columns.Add("TTYPE", typeof(string));

            if (BP.WF.Glo.OSModel == OSModel.OneOne)
            {
                BP.GPM.DeptEmp de = null;
                BP.Port.Emp emp = null;
                BP.WF.Port.EmpStations ess = new BP.WF.Port.EmpStations(stid);

                BP.GPM.DeptEmps des = new BP.GPM.DeptEmps();
                des.Retrieve(BP.GPM.DeptEmpAttr.FK_Dept, deptid);

                BP.Port.Emps emps = new BP.Port.Emps();
                emps.RetrieveAll();

                foreach (BP.WF.Port.EmpStation es in ess)
                {
                    de = des.GetEntityByKey(BP.GPM.DeptEmpAttr.FK_Emp, es.FK_Emp) as BP.GPM.DeptEmp;

                    if (de == null)
                        continue;

                    emp = emps.GetEntityByKey(es.FK_Emp) as BP.Port.Emp;

                    dt.Rows.Add(emp.No, deptid + "|" + stid, emp.Name, "EMP");
                }
            }
            else
            {
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
            }

            return BP.Tools.Json.DataTableToJson(dt);
        }

        public string GetStructureTreeRootTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("NO", typeof(string));
            dt.Columns.Add("PARENTNO", typeof(string));
            dt.Columns.Add("NAME", typeof(string));
            dt.Columns.Add("TTYPE", typeof(string));

            string parentrootid = context.Request.QueryString["parentrootid"];
            string newRootId = "";

            //判断AdminEmp
            AdminEmpExt aext = new AdminEmpExt();

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

            return BP.Tools.Json.DataTableToJson(dt);
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

            string rootid = context.Request.QueryString["rootid"];

            if (BP.WF.Glo.OSModel == OSModel.OneOne)
            {
                BP.Port.Depts depts = new BP.Port.Depts();
                depts.Retrieve(BP.Port.DeptAttr.ParentNo, rootid, BP.Port.DeptAttr.Name);
                BP.Port.Stations sts = new BP.Port.Stations();
                sts.RetrieveAll();
                BP.Port.Emps emps = new BP.Port.Emps();
                emps.Retrieve(BP.Port.EmpAttr.FK_Dept, rootid, BP.Port.EmpAttr.Name);
                BP.Port.EmpStations empsts = new BP.Port.EmpStations();
                empsts.RetrieveAll();

                BP.Port.EmpStations ess = null;
                List<string> insts = new List<string>();
                List<BP.Port.Emp> inemps = new List<BP.Port.Emp>();

                //增加部门
                foreach (BP.Port.Dept dept in depts)
                {
                    dt.Rows.Add(dept.No, dept.ParentNo, dept.Name, "DEPT");
                }

                //增加岗位
                foreach (BP.Port.Emp emp in emps)
                {
                    ess = new BP.Port.EmpStations();
                    ess.Retrieve(BP.Port.EmpStationAttr.FK_Emp, emp.No);

                    foreach (BP.Port.EmpStation es in ess)
                    {
                        if (insts.Contains(es.FK_Station))
                            continue;

                        insts.Add(es.FK_Station);
                        dt.Rows.Add(es.FK_Station, rootid, es.FK_StationT, "STATION");
                    }

                    if (ess.Count == 0)
                        inemps.Add(emp);
                }

                //增加没有岗位的人员
                foreach (BP.Port.Emp emp in inemps)
                {
                    dt.Rows.Add(emp.No, rootid, emp.Name, "EMP");
                }
            }
            else
            {
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
            }

            return BP.Tools.Json.DataTableToJson(dt);
        }

        #region 主页.
        /// <summary>
        /// 初始化登录界面.
        /// </summary>
        /// <returns></returns>
        public string Default_Init()
        {
            //让admin登录
            if (string.IsNullOrEmpty(BP.Web.WebUser.No))
                return "url@Login.htm?DoType=Logout";

            if (BP.Web.WebUser.IsAdmin == false)
                return "url@Login.htm?DoType=Logout";

            //如果没有流程表，就执行安装.
            if (BP.DA.DBAccess.IsExitsObject("WF_Flow") == false)
                return "url@../DBInstall.htm";

            Hashtable ht = new Hashtable();
            if (BP.WF.Glo.OSModel == OSModel.OneOne)
                ht.Add("OSModel", "0");
            else
                ht.Add("OSModel", "1");

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
        #endregion

        #region 登录窗口.
        /// <summary>
        /// 初始化登录界面.
        /// </summary>
        /// <returns></returns>
        public string Login_Init()
        {
            //让admin登录
            if (string.IsNullOrEmpty(BP.Web.WebUser.No) || BP.Web.WebUser.IsAdmin==false )
                return "url@Login.htm?DoType=Logout";

            //如果没有流程表，就执行安装.
            if (BP.DA.DBAccess.IsExitsObject("WF_Flow") == false)
                return "url@../DBInstall.htm";

            try
            {
                // 执行升级
                string str = BP.WF.Glo.UpdataCCFlowVer();
                if (str == null)
                    str = "";
                return str;
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        public string Login_Submit()
        {
            BP.Port.Emp emp = new BP.Port.Emp();
            emp.No = this.GetValFromFrmByKey("TB_UserNo");

            if (emp.RetrieveFromDBSources() == 0)
                return "err@用户名或密码错误.";

            if (emp.No != "admin")
            {
                //检查是否是管理员？
                BP.WF.Port.AdminEmp adminEmp = new Port.AdminEmp();
                adminEmp.No = emp.No;
                if (adminEmp.RetrieveFromDBSources() == 0)
                    return "err@您非管理员用户，不能登录.";

                if (adminEmp.IsAdmin == false)
                    return "err@您非管理员用户，不能登录.";
            }

            string pass = this.GetValFromFrmByKey("TB_Pass");
            if (emp.CheckPass(pass) == false)
                return "err@用户名或密码错误.";

            //让其登录.
            BP.WF.Dev2Interface.Port_Login(emp.No);
            return "SID=" + emp.SID + "&UserNo=" + emp.No;
        }
        #endregion 登录窗口.



        #region 流程相关 Flow
        /// <summary>
        /// 加载流程图数据 
        /// </summary>
        /// <returns></returns>
        public string Flow_LoadFlowJsonData()
        {
            string diagramId = this.GetValFromFrmByKey("diagramId");
            BP.WF.Flow fl = new BP.WF.Flow(diagramId);
            return fl.FlowJson;
        }
        /// <summary>
        /// 保存流程图信息
        /// </summary>
        /// <returns></returns>
        public string Flow_Save()
        {
            //流程格式.
            string diagram = GetValFromFrmByKey("diagram");
            //流程图.
            string png = GetValFromFrmByKey("png");
            // 流程编号.
            string flowNo = GetValFromFrmByKey("diagramId");
            //节点到节点关系
            string direction = GetValFromFrmByKey("direction");

            //直接保存流程图信息
            BP.WF.Flow fl = new BP.WF.Flow(flowNo);
            //修改版本
            fl.DType = fl.DType == BP.WF.CCBPM_DType.BPMN ? BP.WF.CCBPM_DType.BPMN : BP.WF.CCBPM_DType.CCBPM;
            //直接保存了.
            fl.FlowJson = diagram;
            fl.Update();

            //节点方向
            string[] dir_Nodes = direction.Split('@');
            Direction drToNode = new Direction();
            drToNode.Delete(DirectionAttr.FK_Flow, flowNo);
            foreach (string item in dir_Nodes)
            {
                if (string.IsNullOrEmpty(item))
                    continue;

                string[] nodes = item.Split(':');
                if (nodes.Length == 2)
                {
                    drToNode = new Direction();
                    drToNode.FK_Flow = flowNo;
                    drToNode.Node = int.Parse(nodes[0]);
                    drToNode.ToNode = int.Parse(nodes[1]);
                    drToNode.Insert();
                }
            }

            #region //保存节点坐标及标签

            //清空标签
            LabNote labelNode = new LabNote();
            labelNode.Delete(LabNoteAttr.FK_Flow, flowNo);

            JsonData flowJsonData = JsonMapper.ToObject(diagram);
            if (flowJsonData.IsObject == true)
            {
                JsonData flow_Nodes = flowJsonData["s"]["figures"];
                for (int iNode = 0, jNode = flow_Nodes.Count; iNode < jNode; iNode++)
                {
                    JsonData figure = flow_Nodes[iNode];
                    //不存在不进行处理，继续循环
                    if (figure == null || figure["CCBPM_Shape"] == null)
                        continue;
                    if (figure["CCBPM_Shape"].ToString() == "Node")
                    {
                        //节点坐标处理
                        BP.WF.Node node = new BP.WF.Node();
                        node.RetrieveByAttr(NodeAttr.NodeID, figure["CCBPM_OID"]);
                        if (!string.IsNullOrEmpty(node.Name) && figure["rotationCoords"].Count > 0)
                        {
                            JsonData rotationCoord = figure["rotationCoords"][0];
                            node.X = Convert.ToInt32(float.Parse(rotationCoord["x"].ToString()));
                            node.Y = Convert.ToInt32(float.Parse(rotationCoord["y"].ToString()));
                            node.DirectUpdate();
                        }
                    }
                    else if (figure["CCBPM_Shape"].ToString() == "Text")
                    {
                        //流程标签处理
                        JsonData primitives = figure["primitives"][0];
                        JsonData vector = primitives["vector"][0];
                        labelNode = new LabNote();
                        labelNode.FK_Flow = flowNo;
                        labelNode.Name = primitives["str"].ToString();
                        labelNode.X = Convert.ToInt32(float.Parse(vector["x"].ToString()));
                        labelNode.Y = Convert.ToInt32(float.Parse(vector["y"].ToString()));
                        labelNode.Insert();
                    }
                }
                return "true";
            }

            #endregion 保存节点坐标及标签

            return "true";
        }
        /// <summary>
        /// 另存为流程图
        /// </summary>
        /// <returns></returns>
        public string Flow_SaveAs()
        {
            return "";
        }

        /// <summary>
        /// 重置流程版本为1.0
        /// </summary>
        /// <returns></returns>
        public string Flow_ResetFlowVersion()
        {
            DBAccess.RunSQL("UPDATE WF_FLOW SET DType=0, FlowJson='' WHERE No='" + this.FK_Flow + "'");
            return "重置成功.";
        }

        /// <summary>
        /// 获取流程所有元素
        /// </summary>
        /// <returns>json data</returns>
        public string Flow_AllElements_ResponseJson()
        {
            BP.WF.Flow flow = new BP.WF.Flow();
            flow.No = this.FK_Flow;
            flow.RetrieveFromDBSources();

            //获取所有节点
            string sqls = "SELECT NODEID,NAME,X,Y,RUNMODEL FROM WF_NODE WHERE FK_FLOW='" + this.FK_Flow + "';" + Environment.NewLine
                        + "SELECT NODE,TONODE FROM WF_DIRECTION WHERE FK_FLOW='" + this.FK_Flow + "';" + Environment.NewLine
                        + "SELECT MYPK,NAME,X,Y FROM WF_LABNOTE WHERE FK_FLOW='" + this.FK_Flow + "';";

            DataSet ds = DBAccess.RunSQLReturnDataSet(sqls);

            ds.Tables[0].TableName = "Nodes";
            ds.Tables[1].TableName = "Direction";
            ds.Tables[2].TableName = "LabNote";

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
                int iX = 0;
                int iY = 0;
                if (!string.IsNullOrEmpty(x)) iX = (int)double.Parse(x);
                if (!string.IsNullOrEmpty(y)) iY = (int)double.Parse(y);

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

            string NodeName = GetValFromFrmByKey("NodeName");


            BP.WF.Node node = new BP.WF.Node();
            node.NodeID = int.Parse(FK_Node);
            int iResult = node.RetrieveFromDBSources();
            if (iResult > 0)
            {
                node.Name = NodeName;
                node.Update();
                return "true";
            }

            return "false";
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

            BP.Port.Emp emp = new BP.Port.Emp(WebUser.No);
            
            string re = "{\"WebUser\":" + emp.ToJson() + ",\"AdminEmp\":";
            AdminEmpExt aext = new AdminEmpExt();

            if (WebUser.No != "admin")
            {
                BP.WF.Port.AdminEmp aemp = new Port.AdminEmp();
                aemp.No = WebUser.No;

                if(aemp.RetrieveFromDBSources() == 0)
                {
                    aext.RootOfDept = "-999";
                    aext.RootOfFlow = "-999";
                    aext.RootOfForm = "-999";
                }
                else
                {
                    aext.RootOfDept = aemp.RootOfDept;
                    aext.RootOfFlow = "F" + aemp.RootOfFlow;
                    aext.RootOfForm = aext.RootOfForm;
                }
            }
            else
            {
                aext.IsAdmin = true;
                aext.RootOfDept = "0";
                aext.RootOfFlow = "F0";
                aext.RootOfForm = "";
            }

            return re + LitJson.JsonMapper.ToJson(aext) + "}";
        }

        public class AdminEmpExt
        {
            public bool IsAdmin { get; set; }
            public string RootOfFlow { get; set; }
            public string RootOfForm { get; set; }
            public string RootOfDept { get; set; }
        }

        StringBuilder sbJson = new StringBuilder();
        /// <summary>
        /// 获取流程树数据
        /// </summary>
        /// <returns>返回结果Json,流程树</returns>
        public string GetFlowTreeTable()
        {
            string sql = @"SELECT 'F'+No NO,'F'+ParentNo PARENTNO, NAME, IDX, 1 ISPARENT,'FLOWTYPE' TTYPE,-1 DTYPE FROM WF_FlowSort
                           union 
                           SELECT NO, 'F'+FK_FlowSort as PARENTNO,(NO + '.' + NAME) NAME,IDX,0 ISPARENT,'FLOW' TTYPE,DTYPE FROM WF_Flow";

            if (BP.Sys.SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                sql = @"SELECT 'F'||No NO,'F'||ParentNo PARENTNO,NAME, IDX, 1 ISPARENT,'FLOWTYPE' TTYPE,-1 DTYPE FROM WF_FlowSort
                        union 
                        SELECT NO, 'F'||FK_FlowSort as PARENTNO,NO||'.'||NAME NAME,IDX,0 ISPARENT,'FLOW' TTYPE,DTYPE FROM WF_Flow";
            }
            else if (BP.Sys.SystemConfig.AppCenterDBType == DBType.MySQL)
            {
                sql = @"SELECT CONCAT('F', No) NO, CONCAT('F', ParentNo) PARENTNO, NAME, IDX, 1 ISPARENT,'FLOWTYPE' TTYPE,-1 DTYPE FROM WF_FlowSort
                           union 
                           SELECT NO, CONCAT('F', FK_FlowSort) PARENTNO, CONCAT(NO, '.', NAME) NAME,IDX,0 ISPARENT,'FLOW' TTYPE,DTYPE FROM WF_Flow";
            }

            DataTable dt = DBAccess.RunSQLReturnTable(sql);

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

            //判断AdminEmp
            AdminEmpExt aext = new AdminEmpExt();

            if (WebUser.No != "admin")
            {
                BP.WF.Port.AdminEmp aemp = new Port.AdminEmp();
                aemp.No = WebUser.No;

                if (aemp.RetrieveFromDBSources() != 0 && aemp.UserType == 1 && !string.IsNullOrWhiteSpace(aemp.RootOfFlow))
                {
                    DataRow rootRow = dt.Select("PARENTNO='F0'")[0];
                    DataRow newRootRow = dt.Select("NO='F" + aemp.RootOfFlow + "'")[0];

                    newRootRow["PARENTNO"] = "F0";
                    DataTable newDt = dt.Clone();
                    newDt.Rows.Add(newRootRow.ItemArray);

                    GenerChildRows(dt, newDt, newRootRow);
                    dt = newDt;
                }
            }

            return BP.Tools.Json.DataTableToJson(dt, false);
        }

        public void GenerChildRows(DataTable dt, DataTable newDt, DataRow parentRow)
        {
            DataRow[] rows = dt.Select("PARENTNO='" + parentRow["NO"] + "'");

            foreach(DataRow r in rows)
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
            return BP.Tools.Json.DataTableToJson(dt, false);
        }

        public string GetFormTreeTable()
        {
            string sql1 = "SELECT NO ,PARENTNO,NAME, IDX, 1 ISPARENT, 'FORMTYPE' TTYPE, DBSRC FROM Sys_FormTree ORDER BY Idx ASC";
            string sql2 = "SELECT NO, FK_FrmSort as PARENTNO,NAME,IDX,0 ISPARENT, 'FORM' TTYPE FROM Sys_MapData   WHERE AppType=0 AND FK_FormTree IN (SELECT No FROM Sys_FormTree)";
            string sqls = sql1 + ";" + Environment.NewLine
                          + sql2 + ";";
            DataSet ds = DBAccess.RunSQLReturnDataSet(sqls);
            DataTable dt = ds.Tables[1].Clone();
            DataRow[] rows = ds.Tables[0].Select("NAME='表单库'");

            if (rows.Length == 0)
            {
                dt.Rows.Add("0", null, "表单库", 0, 1, "FORMTYPE");
            }
            else
            {
                dt.Rows.Add(rows[0]["NO"], null, rows[0]["NAME"], rows[0]["IDX"], rows[0]["ISPARENT"], rows[0]["TTYPE"]);
            }

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                dt.Rows.Add(dr["NO"], dr["PARENTNO"], dr["NAME"], dr["IDX"], dr["ISPARENT"], dr["TTYPE"]);
            }

            foreach (DataRow row in ds.Tables[1].Rows)
            {
                dt.Rows.Add(row.ItemArray);
            }
            
            //判断AdminEmp
            AdminEmpExt aext = new AdminEmpExt();

            if (WebUser.No != "admin")
            {
                BP.WF.Port.AdminEmp aemp = new Port.AdminEmp();
                aemp.No = WebUser.No;

                if (aemp.RetrieveFromDBSources() != 0 && aemp.UserType == 1 && !string.IsNullOrWhiteSpace(aemp.RootOfForm))
                {
                    DataRow rootRow = dt.Select("PARENTNO IS NULL")[0];
                    DataRow newRootRow = dt.Select("NO='" + aemp.RootOfForm + "'")[0];

                    newRootRow["PARENTNO"] = null;
                    DataTable newDt = dt.Clone();
                    newDt.Rows.Add(newRootRow.ItemArray);

                    GenerChildRows(dt, newDt, newRootRow);
                    dt = newDt;
                }
            }

            return BP.Tools.Json.DataTableToJson(dt, false);
        }

        /// <summary>
        /// 获取表单库数据
        /// </summary>
        /// <returns>返回结果Json,表单库</returns>
        public string GetFormTree_deleted()
        {
            var sqls = "SELECT No ,ParentNo,Name, Idx, 1 IsParent, 'FORMTYPE' TType FROM Sys_FormTree"
                      + " union "
                      +
                      " SELECT No, FK_FrmSort as ParentNo,Name,Idx,0 IsParent, 'FORM' TType FROM Sys_MapData   where AppType=0 AND FK_FormTree IN (SELECT No FROM Sys_FormTree);" + Environment.NewLine
                      +
                      " SELECT ss.No,'SrcRoot' AS ParentNo,ss.Name,0 AS Idx, 1 IsParent, 'SRC' TType FROM Sys_SFDBSrc ss ORDER BY ss.DBSrcType ASC;" + Environment.NewLine
                      +
                      " SELECT st.No, st.FK_SFDBSrc AS ParentNo,st.Name,0 AS Idx, 0 IsParent, 'SRCTABLE' TType FROM Sys_SFTable st;";

            DataSet ds = DBAccess.RunSQLReturnDataSet(sqls);
            var dt = ds.Tables[0].Clone();

            //1.表单类别；2.表单
            foreach (DataRow row in ds.Tables[0].Rows)
                dt.Rows.Add(row.ItemArray);

            //3.数据源字典表
            dt.Rows.Add("SrcRoot", "-1", "数据源字典表", 0, 1, "SRCROOT");

            //4.数据源
            foreach (DataRow row in ds.Tables[1].Rows)
                dt.Rows.Add(row.ItemArray);

            //5.数据接口表
            foreach (DataRow row in ds.Tables[2].Rows)
                dt.Rows.Add(row["No"], Equals(row["ParentNo"], null) || DBNull.Value == row["ParentNo"] || string.IsNullOrWhiteSpace(row["ParentNo"].ToString()) ? "local" : row["ParentNo"], row["Name"], row["Idx"], row["IsParent"],
                            row["TType"]);

            //6.表单相关；7.枚举列表；8.JS验证库；9.Internet云数据；10.私有表单库；11.共享表单库
            dt.Rows.Add("FormRef", "-1", "表单相关", 0, 1, "FORMREF");
            dt.Rows.Add("Enums", "FormRef", "枚举列表", 0, 0, "ENUMS");
            dt.Rows.Add("JSLib", "FormRef", "JS验证库", 0, 0, "JSLIB");
            dt.Rows.Add("FUNCM", "FormRef", "功能执行", 0, 0, "FUNCM");

            dt.Rows.Add("CloundData", "-1", "ccbpm云服务-表单云", 0, 1, "CLOUNDDATA");
            dt.Rows.Add("PriForm", "CloundData", "私有表单云", 0, 0, "PRIFORM");
            dt.Rows.Add("ShareForm", "CloundData", "共有表单云", 0, 0, "SHAREFORM");

            sbJson.Clear();

            string sTmp = "";

            if (dt.Rows.Count > 0)
            {
                GetTreeJsonByTable(dt, "", attrFields: new[] { "TType" });
            }
            sTmp += sbJson.ToString();

            return sTmp;
        }

        public string GetSrcTreeTable()
        {
            string sql1 = "SELECT ss.NO,'SrcRoot' PARENTNO,ss.NAME,0 IDX, 1 ISPARENT, 'SRC' TTYPE FROM Sys_SFDBSrc ss ORDER BY ss.DBSrcType ASC";
            string sql2 = "SELECT st.NO, st.FK_SFDBSrc AS PARENTNO,st.NAME,0 AS IDX, 0 ISPARENT, 'SRCTABLE' TTYPE FROM Sys_SFTable st";
            string sqls = sql1 + ";" + Environment.NewLine + sql2 + " ;";
            DataSet ds = DBAccess.RunSQLReturnDataSet(sqls);
            DataTable dt = ds.Tables[0].Clone();

            foreach (DataRow row in ds.Tables[0].Rows)
                dt.Rows.Add(row.ItemArray);

            foreach (DataRow row in ds.Tables[1].Rows)
                dt.Rows.Add(row.ItemArray);

            return BP.Tools.Json.DataTableToJson(dt, false);
        }

        public string GetStructureTreeTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("NO", typeof(string));
            dt.Columns.Add("PARENTNO", typeof(string));
            dt.Columns.Add("NAME", typeof(string));
            dt.Columns.Add("TTYPE", typeof(string));

            if (BP.WF.Glo.OSModel == OSModel.OneOne)
            {
                BP.WF.Port.Depts depts = new BP.WF.Port.Depts();
                depts.RetrieveAll();
                BP.WF.Port.Stations sts = new BP.WF.Port.Stations();
                sts.RetrieveAll();
                BP.WF.Port.Emps emps = new BP.WF.Port.Emps();
                emps.RetrieveAll(BP.WF.Port.EmpAttr.Name);
                BP.WF.Port.EmpStations empsts = new BP.WF.Port.EmpStations();
                empsts.RetrieveAll();
                BP.GPM.DeptEmps empdetps = new BP.GPM.DeptEmps();
                empdetps.RetrieveAll();

                //部门人员
                Dictionary<string, List<string>> des = new Dictionary<string, List<string>>();
                //岗位人员
                Dictionary<string, List<string>> ses = new Dictionary<string, List<string>>();
                //部门岗位
                Dictionary<string, List<string>> dss = new Dictionary<string, List<string>>();
                BP.WF.Port.Station stt = null;
                BP.WF.Port.Emp empt = null;

                foreach (BP.WF.Port.Dept dept in depts)
                {
                    //增加部门
                    dt.Rows.Add(dept.No, dept.ParentNo, dept.Name, "DEPT");
                    des.Add(dept.No, new List<string>());
                    dss.Add(dept.No, new List<string>());

                    //获取部门下的岗位
                    empdetps.Retrieve(BP.GPM.DeptEmpAttr.FK_Dept, dept.No);
                    foreach (BP.GPM.DeptEmp empdept in empdetps)
                    {
                        des[dept.No].Add(empdept.FK_Emp);
                        //判断该人员拥有的岗位
                        empsts.Retrieve(BP.WF.Port.EmpStationAttr.FK_Emp, empdept.FK_Emp);
                        foreach (BP.WF.Port.EmpStation es in empsts)
                        {
                            if (ses.ContainsKey(es.FK_Station))
                            {
                                if (ses[es.FK_Station].Contains(es.FK_Emp) == false)
                                    ses[es.FK_Station].Add(es.FK_Emp);
                            }
                            else
                            {
                                ses.Add(es.FK_Station, new List<string> { es.FK_Emp });
                            }

                            //增加部门的岗位
                            if (dss[dept.No].Contains(es.FK_Station) == false)
                            {
                                stt = sts.GetEntityByKey(es.FK_Station) as BP.WF.Port.Station;

                                if (stt == null) continue;

                                dss[dept.No].Add(es.FK_Station);
                                dt.Rows.Add(dept.No + "|" + es.FK_Station, dept.No, stt.Name, "STATION");
                            }
                        }
                    }
                }

                foreach (KeyValuePair<string, List<string>> ds in dss)
                {
                    foreach (string st in ds.Value)
                    {
                        foreach (string emp in ses[st])
                        {
                            empt = emps.GetEntityByKey(emp) as BP.WF.Port.Emp;

                            if (empt == null) continue;

                            dt.Rows.Add(ds.Key + "|" + st + "|" + emp, ds.Key + "|" + st, empt.Name, "EMP");
                        }
                    }
                }
            }
            else
            {
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
            }

            return BP.Tools.Json.DataTableToJson(dt, false);
        }

        public string GetStructureDatas(string deptNo, string stationNo, string empNo)
        {
            return null;
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
                                if (string.IsNullOrEmpty(row[field].ToString()))
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

        /// <summary>
        /// 删除流程
        /// </summary>
        /// <returns></returns>
        public string DelFlow()
        {
            return WorkflowDefintionManager.DeleteFlowTemplete(this.FK_Flow);
        }
        /// <summary>
        /// 树节点管理
        /// </summary>
        public string Do()
        {
            string doWhat = GetValFromFrmByKey("doWhat");
            string para1 = GetValFromFrmByKey("para1");
            // 如果admin账户登陆时有错误发生，则返回错误信息
            var result = LetAdminLogin("CH", true);

            if (string.IsNullOrEmpty(result) == false)
                return result;

            switch (doWhat)
            {
                case "NewSameLevelFrmSort": //创建同级别的 表单树 目录.
                    SysFormTree frmSort = null;
                    try
                    {
                        var para = para1.Split(',');
                        frmSort = new SysFormTree(para[0]);
                        string sameNodeNo = frmSort.DoCreateSameLevelNode().No;
                        frmSort = new SysFormTree(sameNodeNo);
                        frmSort.Name = para[1];
                        frmSort.Update();
                        return null;
                    }
                    catch (Exception ex)
                    {
                        return "Do Method NewFormSort Branch has a error , para:\t" + para1 + ex.Message;
                    }
                case "NewSubLevelFrmSort": //创建子级别的 表单树 目录.
                    SysFormTree frmSortSub = null;
                    try
                    {
                        var para = para1.Split(',');
                        frmSortSub = new SysFormTree(para[0]);
                        string sameNodeNo = frmSortSub.DoCreateSubNode().No;
                        frmSortSub = new SysFormTree(sameNodeNo);
                        frmSortSub.Name = para[1];
                        frmSortSub.Update();
                        return null;
                    }
                    catch (Exception ex)
                    {
                        return "Do Method NewSubLevelFrmSort Branch has a error , para:\t" + para1 + ex.Message;
                    }
                case "SetBUnit":
                    try
                    {
                        if (string.IsNullOrEmpty(para1) == false)
                        {
                            BP.WF.Node nd = new BP.WF.Node(int.Parse(para1));
                            nd.IsTask = !nd.IsBUnit;
                            nd.Update();
                        }
                        else
                        {
                            throw new Exception("@参数错误:" + para1);
                        }
                    }
                    catch (Exception ex)
                    {
                        return "err:" + ex.Message;
                    }
                    return null;
                case "GetSettings":
                    return SystemConfig.AppSettings[para1];
                default:
                    throw new Exception("@没有约定的执行标记:" + doWhat);
            }
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
                string pTable = ps[3]; // 物理表名。
                string flowMark = ps[4]; // 流程标记.
                string flowVer = ps[5]; // 流程版本

                string flowNo = BP.WF.Template.TemplateGlo.NewFlow(fk_floSort, flowName, dataSaveModel, pTable, flowMark, flowVer);
                return flowNo;

            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        public string DelNode()
        {
            try
            {
                BP.WF.Node nd = new BP.WF.Node();
                nd.NodeID = this.FK_Node;
                nd.Delete();
                return "删除成功.";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        public string GetFlowSorts()
        {
            FlowSorts flowSorts = new FlowSorts();
            flowSorts.RetrieveAll(FlowSortAttr.Idx);

            BP.WF.Port.AdminEmp emp = new Port.AdminEmp(BP.Web.WebUser.No);
            return BP.Tools.Entitis2Json.ConvertEntitis2GenerTree(flowSorts, emp.RootOfFlow);
        }

        public string DelFlowSort()
        {
            string fk_flowSort = this.GetRequestVal("FK_FlowSort").Replace("F", "");

            FlowSort fs = new FlowSort();
            fs.No = fk_flowSort;

            //检查是否有子流程？
            string sql = "SELECT COUNT(*) FROM WF_Flow WHERE FK_FlowSort='" + fk_flowSort + "'";
            if (DBAccess.RunSQLReturnValInt(sql) != 0)
                return "err@该目录下有流程，您不能删除。";

            //检查是否有子目录？
            sql = "SELECT COUNT(*) FROM WF_FlowSort WHERE ParentNo='" + fk_flowSort + "'";
            if (DBAccess.RunSQLReturnValInt(sql) != 0)
                return "err@该目录下有子目录，您不能删除。";

            fs.Delete();

            return "删除成功.";
        }
        /// <summary>
        /// 新建同级流程类别
        /// </summary>
        /// <returns></returns>
        public string NewSameLevelFlowSort()
        {
            FlowSort fs = null;
            fs = new FlowSort(this.No.Replace("F", ""));//传入的编号多出F符号，需要替换掉
            string sameNodeNo = fs.DoCreateSameLevelNode().No;
            fs = new FlowSort(sameNodeNo);
            fs.Name = this.EnsName;
            fs.Update();
            return "F" + fs.No;
        }
        /// <summary>
        /// 新建下级类别.
        /// </summary>
        /// <returns></returns>
        public string NewSubFlowSort()
        {
            FlowSort fsSub = new FlowSort(this.No.Replace("F", ""));//传入的编号多出F符号，需要替换掉
            string subNodeNo = fsSub.DoCreateSubNode().No;
            FlowSort subFlowSort = new FlowSort(subNodeNo);
            subFlowSort.Name = this.Name;
            subFlowSort.Update();
            return "F" + subFlowSort.No;
        }

        public string EditFlowSort()
        {
            FlowSort fs = new FlowSort();//传入的编号多出F符号，需要替换掉
            fs.No = this.No;
            fs.RetrieveFromDBSources();
            fs.Name = this.GetRequestVal("Name");
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
            catch (Exception exception)
            {
                return exception.Message;
            }
            return string.Empty;
        }
        #endregion

    }
}
