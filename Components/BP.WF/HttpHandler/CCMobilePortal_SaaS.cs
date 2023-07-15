using System;
using System.Collections.Generic;
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
using BP.Difference;
using System.Collections;
using LitJson;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class CCMobilePortal_SaaS : BP.WF.HttpHandler.DirectoryPageBase
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        public CCMobilePortal_SaaS()
        {
            BP.Web.WebUser.SheBei = "Mobile";
        }

        #region  界面 
        public string Home_EditUserName()
        {
            string name = this.GetRequestVal("Name");
            Emp emp = new Emp(WebUser.OrgNo + "_" + WebUser.No);

            emp.Name = name;
            emp.Update();

            //更改登录人的名字.
            BP.Web.WebUser.Name = name;
            return "修改成功.";
        }

        /// <summary>
        /// 修改部门名字
        /// </summary>
        /// <returns></returns>
        public string Home_EditDeptName()
        {
            string name = this.GetRequestVal("Name");

            Dept dept = new Dept(WebUser.FK_Dept);
            dept.Name = name;
            dept.Update();

            //更改登录人的名字.
            BP.Web.WebUser.FK_DeptName = name;
            return "修改成功.";
        }

        public string Home_Init()
        {
            Hashtable ht = new Hashtable();
            ht.Add("UserNo", BP.Web.WebUser.No);
            ht.Add("UserName", BP.Web.WebUser.Name);

            //系统名称.
            ht.Add("SysName", SystemConfig.SysName);
            ht.Add("CustomerName", SystemConfig.CustomerName);

            ht.Add("Todolist_EmpWorks", BP.WF.Dev2Interface.Todolist_EmpWorks);
            ht.Add("Todolist_Runing", BP.WF.Dev2Interface.Todolist_Runing);
            ht.Add("Todolist_Complete", BP.WF.Dev2Interface.Todolist_Complete);
            ht.Add("Todolist_CCWorks", BP.WF.Dev2Interface.Todolist_CCWorks);

            ht.Add("Todolist_HuiQian", BP.WF.Dev2Interface.Todolist_HuiQian); //会签数量.
            ht.Add("Todolist_Drafts", BP.WF.Dev2Interface.Todolist_Draft); //会签数量.

            return BP.Tools.Json.ToJsonEntityModel(ht);
        }
        /// <summary>
        /// 获取当前用户 待处理，已处理，抄送给的消息
        /// </summary>
        /// <returns></returns>
        public string GetGenerWorks()
        {
            DataSet ds = new DataSet();
            //待办列表
            DataTable dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable(WebUser.No, 0);
            dt.TableName = "Todolist";
            ds.Tables.Add(dt);
            //获取审批过未完成的
            dt = new DataTable();
            dt = BP.WF.Dev2Interface.DB_GenerRuning(WebUser.No);
            dt.TableName = "Running";
            ds.Tables.Add(dt);

            //获取完成的
            dt = new DataTable();
            dt = BP.WF.Dev2Interface.DB_GenerRuning(WebUser.No);
            dt.TableName = "Complte";
            ds.Tables.Add(dt);


            //获取抄送当前登陆人的列表
            dt = new DataTable();
            dt = BP.WF.Dev2Interface.DB_CCList();
            dt.TableName = "CC";
            ds.Tables.Add(dt);
            //返回json格式
            return BP.Tools.Json.DataSetToJson(ds);
        }
        /// <summary>
        /// 获取当前用户发起流程的草稿，处理中，已完成的数据
        /// </summary>
        /// <returns></returns>
        public string GetMyStartGenerWorks()
        {
            DataSet ds = new DataSet();
            //流程处理中的
            DataTable dt = BP.WF.Dev2Interface.DB_GenerRuning(WebUser.No, null, true, this.Domain);
            dt.TableName = "Running";
            ds.Tables.Add(dt);

            //流程已完成
            dt = BP.WF.Dev2Interface.DB_FlowComplete(WebUser.No, true);
            dt.TableName = "Complete";
            ds.Tables.Add(dt);

            //获取草稿中的流程
            dt = BP.WF.Dev2Interface.DB_GenerDraftDataTable();
            dt.TableName = "Draflist";
            ds.Tables.Add(dt);

            return BP.Tools.Json.DataSetToJson(ds);
        }

        /// <summary>
        /// 获取当前用户最近使用的流程表单
        /// </summary>
        /// <returns></returns>
        public string GetUseFlowByUserNo()
        {
            string sql = "";
            int top = GetRequestValInt("Top");
            if (top == 0) top = 8;

            switch (BP.Difference.SystemConfig.AppCenterDBType)
            {
                case DBType.MSSQL:
                    sql = " SELECT TOP " + top + " FK_Flow,FlowName,F.Icon FROM WF_GenerWorkFlow G ,WF_Flow F WHERE  F.No=G.FK_Flow AND Starter='" + WebUser.No + "' GROUP BY FK_Flow,FlowName,ICON ORDER By Max(SendDT) DESC";
                    break;
                case DBType.MySQL:
                case DBType.PostgreSQL:
                case DBType.UX:
                    sql = " SELECT DISTINCT FK_Flow,FlowName,F.Icon FROM WF_GenerWorkFlow G ,WF_Flow F WHERE  F.No=G.FK_Flow AND Starter='" + WebUser.No + "'  Order By SendDT  limit  " + top;
                    break;
                case DBType.Oracle:
                case DBType.DM:
                case DBType.KingBaseR3:
                case DBType.KingBaseR6:
                    sql = " SELECT * From (SELECT DISTINCT FK_Flow as \"FK_Flow\",FlowName as \"FlowName\",F.Icon ,max(SendDT) SendDT FROM WF_GenerWorkFlow G ,WF_Flow F WHERE  F.No=G.FK_Flow AND Starter='" + WebUser.No + "' GROUP BY FK_Flow,FlowName,ICON Order By SendDT) WHERE  rownum <=" + top;
                    break;
                default:
                    throw new Exception("err@系统暂时还未开发使用" + BP.Difference.SystemConfig.AppCenterDBType + "数据库");
            }
            DataTable dt = DBAccess.RunSQLReturnTable(sql);


            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 获取用户下的组织
        /// </summary>
        /// <returns></returns>
        public string User_OrgNos()
        {
            string userId = this.GetRequestVal("UserID");
            if (DataType.IsNullOrEmpty(userId) == true)
                return "err@请输入用户的登录账号";
            string sql = " SELECT A.OrgNo as No,B.Name,B.OrgSta From Port_Emp A ,Port_Org B Where  A.OrgNo=B.No  AND UserID=" + SystemConfig.AppCenterDBVarStr + "UserID";
            Paras ps = new Paras();
            ps.Add("UserID", userId);
            DataTable dt = DBAccess.RunSQLReturnTable(sql, ps);
            if (dt.Rows.Count == 0)
                return "err@账号输入错误或者该用户已经被禁用";
            return BP.Tools.Json.ToJson(dt);
        }

        public string User_ChangeOrg()
        {
            string userId = this.GetRequestVal("UserID");
            if (DataType.IsNullOrEmpty(userId) == true)
                return "err@请输入用户的登录账号";
            if (DataType.IsNullOrEmpty(this.OrgNo) == true)
                return "err@组织编号不能为空";
            BP.WF.Dev2Interface.Port_Login(userId, this.OrgNo);
            return "切换成功";
        }
        public string Home_Near()
        {
            string sql = "";
            int top = GetRequestValInt("Top");
            if (top == 0) top = 8;

            switch (BP.Difference.SystemConfig.AppCenterDBType)
            {
                case DBType.MSSQL:
                    sql = " SELECT TOP " + top + " FK_Flow,FlowName,F.Icon FROM WF_GenerWorkFlow G ,WF_Flow F WHERE  F.No=G.FK_Flow AND Starter='" + WebUser.No + "' GROUP BY FK_Flow,FlowName,ICON ORDER By Max(SendDT) DESC";
                    break;
                case DBType.MySQL:
                case DBType.PostgreSQL:
                case DBType.UX:
                    sql = "SELECT DISTINCT *  From(SELECT   FK_Flow,FlowName,F.Icon FROM WF_GenerWorkFlow G ,WF_Flow F WHERE  F.No=G.FK_Flow AND Starter='" + WebUser.No + "'  Order By SendDT  limit  " + top*2+")A  LIMIT "+top;
                    break;
                case DBType.Oracle:
                case DBType.DM:
                case DBType.KingBaseR3:
                case DBType.KingBaseR6:
                    sql = " SELECT * From (SELECT DISTINCT FK_Flow as \"FK_Flow\",FlowName as \"FlowName\",F.Icon ,max(SendDT) SendDT FROM WF_GenerWorkFlow G ,WF_Flow F WHERE  F.No=G.FK_Flow AND Starter='" + WebUser.No + "' GROUP BY FK_Flow,FlowName,ICON Order By SendDT) WHERE  rownum <=" + top;
                    break;
                default:
                    throw new Exception("err@系统暂时还未开发使用" + BP.Difference.SystemConfig.AppCenterDBType + "数据库");
            }
            DataTable dt = DBAccess.RunSQLReturnTable(sql);


            return BP.Tools.Json.ToJson(dt);
        }
        public string Student_JiaoNaXueFei()
        {
            string no = this.GetRequestVal("No");
            string name = this.GetRequestVal("Name");
            string note = this.GetRequestVal("Note");
            var jine = this.GetRequestValFloat("JinE");


            return "学费缴纳成功[" + no + "][" + name + "][" + note + "][" + jine + "]";

        }
        /// <summary>
        /// 查询流程的
        /// </summary>
        /// <returns></returns>
        public string Search_Init()
        {
            DataSet ds = new DataSet();
            GenerWorkFlows gwfs = new GenerWorkFlows();
            QueryObject qo = new QueryObject(gwfs);
            //关键字查询
            string keyWord = this.GetRequestVal("SearchKey");

            //流程状态
            string wfstate = this.GetRequestVal("WFState");
            if (DataType.IsNullOrEmpty(wfstate) == true)
                qo.AddWhere(GenerWorkFlowAttr.WFState, "!=", 0);
            else
                qo.AddWhere(GenerWorkFlowAttr.WFState, int.Parse(wfstate));

            //查询我发起的，我参与的
            qo.addAnd();
            qo.addLeftBracket();
            qo.AddWhere(GenerWorkFlowAttr.StarterName, WebUser.No);
            qo.addOr();
            qo.AddWhere(GenerWorkFlowAttr.Emps, "like", "'%@" + WebUser.No + "@%'");
            qo.addOr();
            qo.AddWhere(GenerWorkFlowAttr.Emps, "like", "'%@" + WebUser.No + ",%'");
            qo.addRightBracket();

            if (DataType.IsNullOrEmpty(keyWord) == false)
            {
                qo.addAnd();
                //模糊查询流程标题，流程名称，发起人，处理人
                qo.addLeftBracket();
                if (BP.Difference.SystemConfig.AppCenterDBVarStr == "@" || BP.Difference.SystemConfig.AppCenterDBVarStr == "?")
                    qo.AddWhere(GenerWorkFlowAttr.Title, " LIKE ", BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL ? (" CONCAT('%'," + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey, '%')") : (" '%'+" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey+'%'"));
                else
                    qo.AddWhere(GenerWorkFlowAttr.Title, " LIKE ", " '%'||" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey|| '%'");

                qo.addOr();
                if (BP.Difference.SystemConfig.AppCenterDBVarStr == "@" || BP.Difference.SystemConfig.AppCenterDBVarStr == "?")
                    qo.AddWhere(GenerWorkFlowAttr.FlowName, " LIKE ", BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL ? (" CONCAT('%'," + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey, '%')") : (" '%'+" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey+'%'"));
                else
                    qo.AddWhere(GenerWorkFlowAttr.FlowName, " LIKE ", " '%'||" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey|| '%'");
                //qo.addOr();
                //if (BP.Difference.SystemConfig.AppCenterDBVarStr == "@" || BP.Difference.SystemConfig.AppCenterDBVarStr == "?")
                //    qo.AddWhere(GenerWorkFlowAttr.Emps, " LIKE ", BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL ? (" CONCAT('%'," + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey, '%')") : (" '%'+" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey+'%'"));
                //else
                //    qo.AddWhere(GenerWorkFlowAttr.Emps, " LIKE ", " '%'||" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey|| '%'");

                qo.MyParas.Add("SKey", keyWord);
                qo.addRightBracket();
            }
            //时间段查询
            int tspan = this.GetRequestValInt("TSpan");
            if (tspan != 3)
            {
                qo.addAnd();
                qo.AddWhere(GenerWorkFlowAttr.TSpan, tspan);
            }


            //发起时间
            string dtFrom = this.GetRequestVal("DTFrom");
            string dtTo = this.GetRequestVal("DTTo");
            if (DataType.IsNullOrEmpty(dtFrom) == true && DataType.IsNullOrEmpty(dtTo) == false)
            {
                qo.addAnd();
                qo.addLeftBracket();
                qo.SQL = GenerWorkFlowAttr.SendDT + " <= '" + dtTo + "'";
                qo.addRightBracket();
            }
            else if (DataType.IsNullOrEmpty(dtFrom) == false && DataType.IsNullOrEmpty(dtTo) == true)
            {
                qo.addAnd();
                qo.addLeftBracket();
                qo.SQL = GenerWorkFlowAttr.SendDT + " >= '" + dtFrom + "'";
                qo.addRightBracket();
            }
            else if (DataType.IsNullOrEmpty(dtFrom) == false && DataType.IsNullOrEmpty(dtTo) == false)
            {
                qo.addAnd();
                qo.addLeftBracket();
                dtTo += " 23:59:59";
                qo.SQL = GenerWorkFlowAttr.SendDT + " >= '" + dtFrom + "'";
                qo.addAnd();
                qo.SQL = GenerWorkFlowAttr.SendDT + " <= '" + dtTo + "'";
                qo.addRightBracket();
            }

            //增加当前用户所在的组织
            qo.addAnd();
            qo.AddWhere(GenerWorkFlowAttr.OrgNo, WebUser.OrgNo);
            qo.DoQuery();
            DataTable dt = gwfs.ToDataTableField("WF_GenerWorkFlows");
            ds.Tables.Add(dt);
            dt = new DataTable("Todolist_EmpWorks");
            dt.Columns.Add("EmpWorks");
            DataRow dr = dt.NewRow();
            dr["EmpWorks"] = BP.WF.Dev2Interface.Todolist_EmpWorks;
            dt.Rows.Add(dr);
            ds.Tables.Add(dt);
            return BP.Tools.Json.ToJson(ds);
        }

        public string JS_SDK_Signature()
        {
            string htmlPage = this.GetRequestVal("htmlPage");
            Hashtable ht = new Hashtable();

            //生成签名的时间戳
            string timestamp = DateTime.Now.ToString("yyyyMMDDHHmmss");
            //生成签名的随机串
            string nonceStr = BP.DA.DBAccess.GenerGUID();
            string url1 = htmlPage;
            //获取 AccessToken CorpID 
            string url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=wx5c371647cecd1db2&secret=d9db5f92622436dde57bf254b1664ac3";
            string res = new HttpWebResponseUtility().HttpResponseGet(url);
            JsonData jd = JsonMapper.ToObject(res);
            if (res.Contains("errcode") == true)
            {
                Object errcode = jd["errcode"];
                if (errcode != null)
                    throw new Exception("err@获取token信息失败");
            }

            string access_token = (string)jd["access_token"];

            //获取jsapi_ticket
            url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + access_token + "&type=jsapi";
            res = new HttpWebResponseUtility().HttpResponseGet(url);
            jd = JsonMapper.ToObject(res);
            if (res.Contains("errcode") == true)
            {
                string errcode1 = jd["errcode"].ToString();
                if (errcode1.Equals("0") == false)
                    throw new Exception("err@获取jsapi_ticket失败");
            }


            string ticket = (string)jd["ticket"];
            ht.Add("timestamp", timestamp);
            ht.Add("nonceStr", nonceStr);
            //企业微信的corpID
            ht.Add("AppID", "wx5c371647cecd1db2");

            //生成签名算法
            string str1 = "jsapi_ticket=" + ticket + "&noncestr=" + nonceStr + "&timestamp=" + timestamp + "&url=" + url1 + "";

            ht.Add("signature", BP.WF.Difference.Glo.Sha1Signature(str1));
            ht.Add("str", str1);

            return BP.Tools.Json.ToJson(ht);
        }
        #endregion 界面方法.

    }
}
