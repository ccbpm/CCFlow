using System;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Data;
using BP.DA;
using BP.Sys;
using BP.En;
using BP.WF;
using BP.WF.Template;

namespace CCFlow.WF.WorkOpt
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
                {
                    str = context.Request.Form["DoType"];
                }
                if (str == null || str == "" || str == "null") {

                    return null;
                }
                return str;
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                string str = context.Request.QueryString["FK_Flow"];
                if (str == null || str == "" || str == "null")
                {
                    str = context.Request.Form["FK_Flow"];
                }
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
                {
                    str = context.Request.Form["MyPK"];
                }
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
                {
                    str = context.Request.Form["FK_SFTable"];
                }
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
                {
                    str = context.Request.Form["EnumKey"];
                }
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
                {
                    str = context.Request.Form["KeyOfEn"];
                }
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 部门编号
        /// </summary>
        public string FK_Dept
        {
            get
            {
                string str = context.Request.QueryString["FK_Dept"];
                if (str == null || str == "" || str == "null")
                {
                    str = context.Request.Form["FK_Dept"];
                }
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
                {
                    str = context.Request.Form["FK_MapData"];
                }
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
                {
                    str = context.Request.Form["FK_Node"];
                }
                if (str == null || str == "" || str == "null")
                    return 0;
                return int.Parse(str);
            }
        }
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                string str = context.Request.QueryString["WorkID"];
                if (str == null || str == "" || str == "null")
                {
                    str = context.Request.Form["WorkID"];
                }
                if (str == null || str == "" || str == "null")
                    return 0;
                return Int64.Parse(str);
            }
        }
        /// <summary>
        /// FID
        /// </summary>
        public Int64 FID
        {
            get
            {
                string str = context.Request.QueryString["FID"];
                if (str == null || str == "" || str == "null")
                {
                    str = context.Request.Form["FID"];
                }
                if (str == null || str == "" || str == "null")
                    return 0;
                return Int64.Parse(str);
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
                {
                    str = context.Request.Form["FK_MapDtl"];
                }
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
        /// <param name="param">参数名</param>
        /// <returns></returns>
        public string GetRequestVal(string param)
        {
            return HttpUtility.UrlDecode(context.Request[param], System.Text.Encoding.UTF8);
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
                    case "CC_Init": //抄送，初始化.
                        msg = CC_Init();
                        break;
                    case "CC_SelectDepts": //抄送，选择部门.
                        msg= CC_SelectDepts();
                        break;
                    case "CC_SelectStations": //抄送，选择岗位.
                        msg= CC_SelectStations();
                        break;
                    case "CC_Send": //抄送，执行.
                        msg = CC_Send();
                        break;
                    case "DealSubThreadReturnToHL_Init":
                        msg = this.DealSubThreadReturnToHL_Init();
                        break;
                    case "DealSubThreadReturnToHL_Done":
                        msg = this.DealSubThreadReturnToHL_Done();
                        break;
                    case "ViewWorkNodeFrm": //查看一个表单.
                        msg = ViewWorkNodeFrm();
                        break;
                    case "Askfor": //加签.
                        msg = this.Askfor();
                        break;
                    case "AskForRe": //回复加签.
                        msg = this.AskForRe();
                        break;
                    case "SelectEmps":
                        msg = this.SelectEmps();
                        break;
                    case "AccepterInit": //首次加载.
                        msg= this.AccepterInit();
                        break;
                    case "AccepterSave": //保存.
                        msg = this.AccepterSave();
                        break;
                    case "AccepterSend": //发送.
                        msg = this.AccepterSend();
                        break;
                    case "FlowBBSList": //获得流程评论列表.
                        msg = this.FlowBBSList();
                        break;
                    case "Return_Init": //获得可以退回的节点.
                        msg = Return_Init();
                        break;
                    case "DoReturnWork": //执行退回.
                        msg = ReturnWork();
                        break;
                    case "Shift": //移交.
                        msg = Shift();
                        break;
                    case "UnShift": //撤销移交.
                        msg = UnShift();
                        break;
                    case "Press": //催办.
                        msg = Press();
                        break;
                    case "DeleteFlowInstance_DoDelete": //执行删除流程.
                        msg = DeleteFlowInstance_DoDelete();
                        break;
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
            //输出信息.
        }
        /// <summary>
        /// 抄送初始化.
        /// </summary>
        /// <returns></returns>
        public string CC_Init()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            Hashtable ht = new Hashtable();
            ht.Add("Title", gwf.Title);
            ht.Add("CCTo", "zhangsan,lisi");

            //返回流程标题.
            return BP.Tools.Json.ToJson(ht, false);
        }
        /// <summary>
        /// 选择部门呈现信息.
        /// </summary>
        /// <returns></returns>
        public string CC_SelectDepts()
        {
            BP.Port.Depts depts = new BP.Port.Depts();
            depts.RetrieveAll();
            return depts.ToJson();
        }
        /// <summary>
        /// 选择部门呈现信息.
        /// </summary>
        /// <returns></returns>
        public string CC_SelectStations()
        {
            //岗位类型.
            string sql = "SELECT NO,NAME FROM Port_StationType";
            DataSet ds = new DataSet();
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Port_StationType";
            ds.Tables.Add(dt);

            //岗位.ss
            string sqlStas = "SELECT NO,NAME,FK_STATIONTYPE FROM Port_Station";
            DataTable dtSta = BP.DA.DBAccess.RunSQLReturnTable(sqlStas);
            dtSta.TableName = "Port_Station";
            ds.Tables.Add(dtSta);
            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 抄送发送.
        /// </summary>
        /// <returns></returns>
        public string CC_Send()
        {
            //人员信息. 格式 zhangsan,张三;lisi,李四;
            string emps = this.GetRequestVal("Emps");

            //岗位信息. 格式:  001,002,003,
            string stations = this.GetRequestVal("Stations");

            //部门信息.  格式: 001,002,003,
            string depts = this.GetRequestVal("Depts");

            //标题.
            string title = this.GetRequestVal("TB_Title");
            // 内容.
            string doc = this.GetRequestVal("TB_Doc");

            //调用抄送接口执行抄送.
            //   BP.WF.Dev2Interface.Node_CC_WriteTo_CClist(this.FK_Node, 0, this.WorkID, title, doc, emps, depts, stations);

            return "执行抄送成功.emps=" + emps + "  depts=" + depts + " stas=" + stations;
        }

        #region 退回到分流节点处理器.
        /// <summary>
        /// 初始化.
        /// </summary>
        /// <returns></returns>
        public string DealSubThreadReturnToHL_Init()
        {
            /* 如果工作节点退回了*/
            BP.WF.ReturnWorks rws = new BP.WF.ReturnWorks();
            rws.Retrieve(BP.WF.ReturnWorkAttr.ReturnToNode, this.FK_Node,
                BP.WF.ReturnWorkAttr.WorkID, this.WorkID,
                BP.WF.ReturnWorkAttr.RDT);

            string msgInfo = "";
            if (rws.Count != 0)
            {
                foreach (BP.WF.ReturnWork rw in rws)
                {
                    msgInfo += "<fieldset width='100%' ><legend>&nbsp; 来自节点:" + rw.ReturnNodeName + " 退回人:" + rw.ReturnerName + "  " + rw.RDT + "&nbsp;<a href='./../../DataUser/ReturnLog/" + this.FK_Flow + "/" + rw.MyPK + ".htm' target=_blank>工作日志</a></legend>";
                    msgInfo += rw.NoteHtml;
                    msgInfo += "</fieldset>";
                }
            }

            //把节点信息也传入过去，用于判断不同的按钮显示. 
            BP.WF.Template.BtnLab btn = new BtnLab(this.FK_Node);
            BP.WF.Node nd = new Node(this.FK_Node);

            Hashtable ht = new Hashtable();
            //消息.
            ht.Add("MsgInfo", msgInfo);

            //是否可以移交？
            if (btn.ShiftEnable)
                ht.Add("ShiftEnable", "1");
            else
                ht.Add("ShiftEnable", "0");

            //是否可以撤销？
            if (nd.HisCancelRole == CancelRole.None)
                ht.Add("CancelRole", "0");
            else
                ht.Add("CancelRole", "1");

            //是否可以删除子线程? 在分流节点上.
            if (btn.ThreadIsCanDel)
                ht.Add("ThreadIsCanDel", "1");
            else
                ht.Add("ThreadIsCanDel", "0");

             //是否可以移交子线程? 在分流节点上.
            if (btn.ThreadIsCanShift)
                ht.Add("ThreadIsCanShift", "1");
            else
                ht.Add("ThreadIsCanShift", "0");

            return BP.Tools.Json.ToJson(ht, false);
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string DealSubThreadReturnToHL_Done()
        {
            //操作类型.
            string actionType = this.GetRequestVal("ActionType");
            string note = this.GetRequestVal("Note");


            if (actionType == "Return")
            {
                /*如果是退回. */
                BP.WF.ReturnWork rw = new BP.WF.ReturnWork();
                rw.Retrieve(BP.WF.ReturnWorkAttr.ReturnToNode, this.FK_Node,
                         BP.WF.ReturnWorkAttr.WorkID, this.WorkID);
                string info = BP.WF.Dev2Interface.Node_ReturnWork(this.FK_Flow, this.WorkID, this.FID,
                    this.FK_Node, rw.ReturnNode, note, false);
                return info;
            }


            if (actionType == "Shift")
            {
                /*如果是移交操作.*/
                string toEmps = this.GetRequestVal("ShiftToEmp");
                return BP.WF.Dev2Interface.Node_Shift(this.FK_Flow, this.FK_Node, this.WorkID, this.FID, toEmps, note);
            }

            if (actionType == "Kill")
            {
                string msg = BP.WF.Dev2Interface.Flow_DeleteSubThread(this.FK_Flow, this.WorkID, "手工删除");
                //提示信息.
                if (msg == "" || msg == null)
                    msg = "该工作删除成功...";
                return msg;
            }

            if (actionType == "UnSend")
            {
                return BP.WF.Dev2Interface.Flow_DoUnSend(this.FK_Flow, this.FID, this.FK_Node);
            }

            return "err@没有判断的类型"+actionType;
        }
        #endregion 退回到分流节点处理器.

        public string DeleteFlowInstance_DoDelete()
        {
            if (BP.WF.Dev2Interface.Flow_IsCanDeleteFlowInstance(this.FK_Flow, 
                this.WorkID, BP.Web.WebUser.No) == false)
                return "您没有删除该流程的权限.";

            string deleteWay = this.GetRequestVal("RB_DeleteWay");
            string doc = this.GetRequestVal("TB_Doc");

            //是否要删除子流程？ 这里注意变量的获取方式，你可以自己定义.
            string isDeleteSubFlow = this.GetRequestVal("CB_IsDeleteSubFlow");

            bool isDelSubFlow = false;
            if (isDeleteSubFlow == "1")
                isDelSubFlow = true;

            //按照标记删除.
            if (deleteWay == "0")
                BP.WF.Dev2Interface.Flow_DoDeleteFlowByFlag(this.FK_Flow, this.WorkID, doc, isDelSubFlow);

            //彻底删除.
            if (deleteWay == "1")
                BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(this.FK_Flow, this.WorkID, isDelSubFlow);

            //彻底并放入到删除轨迹里.
            if (deleteWay == "2")
                BP.WF.Dev2Interface.Flow_DoDeleteFlowByWriteLog(this.FK_Flow, this.WorkID, doc, isDelSubFlow);

            return "流程删除成功.";
        }
        /// <summary>
        /// 获得节点表单数据.
        /// </summary>
        /// <returns></returns>
        public string ViewWorkNodeFrm()
        {
            Node nd = new Node(this.FK_Node);

            Hashtable ht = new Hashtable();
            ht.Add("FormType", nd.FormType.ToString());
            ht.Add("Url", nd.FormUrl + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node);

            if (nd.FormType == NodeFormType.SDKForm)
                return BP.Tools.Json.ToJson(ht, false);

            if (nd.FormType == NodeFormType.SelfForm)
                return BP.Tools.Json.ToJson(ht, false);

            //表单模版.
            DataSet myds = BP.Sys.CCFormAPI.GenerHisDataSet(nd.NodeFrmID, true);
            string json = BP.WF.Dev2Interface.CCFrom_GetFrmDBJson(this.FK_Flow, this.MyPK);
            DataTable mainTable = BP.Tools.Json.ToDataTableOneRow(json);
            mainTable.TableName = "MainTable";
            myds.Tables.Add(mainTable);
           
            //MapExts exts = new MapExts(nd.HisWork.ToString());
            //DataTable dtMapExt = exts.ToDataTableDescField();
            //dtMapExt.TableName = "Sys_MapExt";
            //myds.Tables.Add(dtMapExt);
 
            return BP.Tools.Json.ToJson(myds);
        }
      
        /// <summary>
        /// 回复加签信息.
        /// </summary>
        /// <returns></returns>
        public string AskForRe()
        {
            string note = context.Request.QueryString["Note"]; //原因.
            return BP.WF.Dev2Interface.Node_AskforReply( this.WorkID,  note);
        }
        /// <summary>
        /// 执行加签
        /// </summary>
        /// <returns>执行信息</returns>
        public string Askfor()
        {
            Int64 workID = int.Parse(context.Request.QueryString["WorkID"]); //工作ID
            string toEmp = context.Request.QueryString["ToEmp"]; //让谁加签?
            string note = context.Request.QueryString["Note"]; //原因.
            string model = context.Request.QueryString["Model"]; //模式.

            BP.WF.AskforHelpSta sta = BP.WF.AskforHelpSta.AfterDealSend;
            if (model == "0")
                sta = BP.WF.AskforHelpSta.AfterDealSend;

            return BP.WF.Dev2Interface.Node_Askfor(workID, sta, toEmp, note);
        }
        /// <summary>
        /// 人员选择器
        /// </summary>
        /// <returns></returns>
        public string SelectEmps()
        {
            string fk_dept = this.FK_Dept;
            if (fk_dept == null)
                fk_dept = BP.Web.WebUser.FK_Dept;

            DataSet ds = new DataSet();

            string sql = "SELECT No,Name,ParentNo FROM Port_Dept WHERE No='" + fk_dept + "' OR ParentNo='" + fk_dept + "' ";
            DataTable dtDept = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dtDept.TableName = "Depts";
            ds.Tables.Add(dtDept);

            sql = "SELECT No,Name,FK_Dept FROM Port_Emp WHERE FK_Dept='" + fk_dept + "'";
            DataTable dtEmps = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dtEmps.TableName = "Emps";
            ds.Tables.Add(dtEmps);

            return BP.Tools.Json.ToJson(ds);
        }

        #region 选择接受人.
        /// <summary>
        /// 初始化接受人.
        /// </summary>
        /// <returns></returns>
        public string AccepterInit()
        {

            int toNodeID = 0;
            if (this.GetValFromFrmByKey("ToNode") != "0")
                toNodeID = this.GetValIntFromFrmByKey("ToNode");

            //当前节点.
            Node nd = new Node(this.FK_Node);

            //到达的节点, 用户生成到达节点的下拉框.
            Nodes toNodes = nd.HisToNodes;
            DataTable dtNodes = new DataTable();
            dtNodes.TableName = "Nodes";
            dtNodes.Columns.Add(new DataColumn("No", typeof(string)));
            dtNodes.Columns.Add(new DataColumn("Name", typeof(string)));
            dtNodes.Columns.Add(new DataColumn("SelectorDBShowWay", typeof(string)));
            dtNodes.Columns.Add(new DataColumn("SelectorModel", typeof(string)));
            foreach (Node item in toNodes)
            {
                DataRow dr = dtNodes.NewRow();
                Selector selectItem = new Selector(item.NodeID);
                dr["No"] = item.NodeID;
                dr["Name"] = item.Name;
                dr["SelectorDBShowWay"] = selectItem.SelectorDBShowWay;
                dr["SelectorModel"] = selectItem.SelectorModel;
                dtNodes.Rows.Add(dr);
            }

            //求到达的节点.
            if (toNodeID == 0)
                toNodeID = toNodes[0].GetValIntByKey("NodeID");   //取第一个.

            Selector select = new Selector(toNodeID);

            //获得 部门与人员.
            DataSet ds = select.GenerDataSet( toNodeID);

            //加入到到达节点的列表.
            ds.Tables.Add(dtNodes);

            //返回json.
            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 保存.
        /// </summary>
        /// <returns></returns>
        public string AccepterSave()
        {
            try
            {
                //求到达的节点. 
                int toNodeID = 0;
                if (this.GetRequestVal("ToNode") != "0")
                    toNodeID = int.Parse(this.GetRequestVal("ToNode"));

                if (toNodeID == 0)
                {   //没有就获得第一个节点.
                    Node nd = new Node(this.FK_Node);
                    Nodes nds = nd.HisToNodes;
                    toNodeID = nds[0].GetValIntByKey("NodeID");
                }

                //求发送到的人员.
                // string selectEmps = this.GetValFromFrmByKey("SelectEmps");
                string selectEmps = this.GetRequestVal("SelectEmps");
                selectEmps = selectEmps.Replace(";", ",");

                //保存接受人.
                BP.WF.Dev2Interface.Node_AddNextStepAccepters(this.WorkID, toNodeID, selectEmps);
                return "SaveOK@" + selectEmps;
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// 执行保存并发送.
        /// </summary>
        /// <returns>返回发送的结果.</returns>
        public string AccepterSend()
        {
            try
            {
                //求到达的节点. 
                int toNodeID = 0;
                if (this.GetRequestVal("ToNode") != "0")
                    toNodeID = int.Parse( this.GetRequestVal("ToNode"));

                if (toNodeID == 0)
                {   //没有就获得第一个节点.
                    Node nd = new Node(this.FK_Node);
                    Nodes nds = nd.HisToNodes;
                    toNodeID = nds[0].GetValIntByKey("NodeID");
                }

                //求发送到的人员.
               // string selectEmps = this.GetValFromFrmByKey("SelectEmps");
                string selectEmps = this.GetRequestVal("SelectEmps");
                selectEmps = selectEmps.Replace(";", ",");

                //执行发送.
                SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID, toNodeID, selectEmps);
                return objs.ToMsgOfHtml();
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        #endregion


        #region 工作退回.
        /// <summary>
        /// 获得可以退回的节点.
        /// </summary>
        /// <returns></returns>
        public string Return_Init()
        {
            DataTable dt = BP.WF.Dev2Interface.DB_GenerWillReturnNodes(this.FK_Node, this.WorkID, this.FID);
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 执行退回,返回退回信息.
        /// </summary>
        /// <returns></returns>
        public string ReturnWork()
        {
            int toNodeID = int.Parse(context.Request.QueryString["ReturnToNode"]);
            string reMesage = context.Request.QueryString["ReturnMsg"];

            bool isBackBoolen = false;
            string isBack = context.Request.QueryString["IsBack"];
            if (isBack == "1")
                isBackBoolen = true;
            

            return BP.WF.Dev2Interface.Node_ReturnWork(this.FK_Flow, this.WorkID, this.FID, this.FK_Node, toNodeID, reMesage, isBackBoolen);
        }
        #endregion

        /// <summary>
        /// 执行移交.
        /// </summary>
        /// <returns></returns>
        public string Shift()
        {
            string msg = context.Request.QueryString["Message"];
            string toEmp = context.Request.QueryString["ToEmp"];
            return BP.WF.Dev2Interface.Node_Shift(this.FK_Flow, this.FK_Node, this.WorkID, this.FID, toEmp, msg);
        }
        /// <summary>
        /// 撤销移交
        /// </summary>
        /// <returns></returns>
        public string UnShift()
        {
            return BP.WF.Dev2Interface.Node_ShiftUn(this.FK_Flow, this.WorkID);
        }
        /// <summary>
        /// 执行催办
        /// </summary>
        /// <returns></returns>
        public string Press()
        {
            string msg = context.Request.QueryString["Msg"];

            //调用API.
            return BP.WF.Dev2Interface.Flow_DoPress(this.WorkID, msg, true);
        }

        /// <summary>
        /// 获得发起的BBS评论.
        /// </summary>
        /// <returns></returns>
        public string FlowBBSList()
        {
            Paras ps = new Paras();
            ps.SQL=  "SELECT * FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE ActionType=" +BP.Sys.SystemConfig.AppCenterDBVarStr+"ActionType";
            ps.Add("ActionType",(int)BP.WF.ActionType.FlowBBS);
           
            //转化成json
            return BP.Tools.Json.ToJson( BP.DA.DBAccess.RunSQLReturnTable(ps));
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