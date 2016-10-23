using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;

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
                    return null;
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
        /// <summary>
        /// 部门编号
        /// </summary>
        public string FK_Dept
        {
            get
            {
                string str = context.Request.QueryString["FK_Dept"];
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
        /// 工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                string str = context.Request.QueryString["WorkID"];
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
                    case "Askfor":
                        msg = this.Askfor();
                        break;
                    case "SelectEmps":
                        msg = this.SelectEmps();
                        break;
                    case "AccepterInit": //选择接受人按钮.
                        msg= this.AccepterInit();
                        break;
                    case "FlowBBSList": //获得流程评论列表.
                        msg = this.FlowBBSList();
                        break;
                    case "ReturnToNodes": //获得可以退回的节点.
                        msg = ReturnToNodes();
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
            return "";
        }
        #endregion


        #region 工作退回.
        /// <summary>
        /// 获得可以退回的节点.
        /// </summary>
        /// <returns></returns>
        public string ReturnToNodes()
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