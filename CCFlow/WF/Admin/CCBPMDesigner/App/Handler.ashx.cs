using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCFlow.WF.Admin.CCBPMDesigner.App
{
    /// <summary>
    /// Handler 的摘要说明
    /// </summary>
    public class Handler : IHttpHandler
    {
        #region 执行.
        public HttpContext context = null;
        /// <summary>
        /// 执行类型
        /// </summary>
        public string DoType
        {
            get
            {
                string str = GetRequestVal("DoType");
                if (str == null || str == "" || str == "null")
                {
                    if (string.IsNullOrEmpty(GetRequestVal("action")) == false)
                        str = context.Request["action"].ToString();
                    else
                        return null;
                }
                return str;
            }
        }
        /// <summary>
        /// FK_Flow
        /// </summary>
        public string FK_Flow
        {
            get
            {
                string str = GetRequestVal("FK_Flow");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// FK_Node
        /// </summary>
        public int FK_Node
        {
            get
            {
                string str = GetRequestVal("FK_Node");
                if (str == null || str == "" || str == "null")
                    return 0;
                return int.Parse(str);
            }
        }
        /// <summary>
        /// 主键
        /// </summary>
        public string MyPK
        {
            get
            {
                string str = GetRequestVal("MyPK");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 编号
        /// </summary>
        public string No
        {
            get
            {
                string str = GetRequestVal("No");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
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

        public void ProcessRequest(HttpContext mycontext)
        {
            context = mycontext;
            if (this.DoType == null)
                return;
            string msg = "";
            try
            {
                switch (this.DoType)
                {
                    case "Welcome_Init": //初始化登录界面.
                        msg = this.Welcome_Init();
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

            context.Response.ContentType = "text/plain";
            context.Response.Write(msg);
        }

        public string Welcome_Init()
        {
            int totalFlow = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(No) FROM WF_Flow ");
            int runFlowNum = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(No) FROM WF_Flow WHERE IsCanStart=1  ");
            int nodeNum = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(NodeID) FROM WF_Node ");
            int zsNum = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow ");
            int zzyxNum = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState!=" +  (int)BP.WF.WFState.Complete);
            int wxNum = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState=" + (int)BP.WF.WFState.Complete);
            int thzNum = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState=" + (int)BP.WF.WFState.ReturnSta);
            int deleteNum =  BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState=" + (int)BP.WF.WFState.Delete);
            int qtNum = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFSta=" + (int)BP.WF.WFSta.Etc);
            
            //平均每流程发起数量。
            decimal avgNum = 0;
            try
            {
                avgNum = (decimal)nodeNum / (decimal)totalFlow;
            }
            catch (Exception)
            {
            }
            //流程启用比率。
            decimal flowRate = 0;
            try
            {
                flowRate = (decimal)runFlowNum / (decimal)totalFlow * 100;
            }
            catch (Exception)
            {
            }
            //OverMinutes小于0表明提前 
            string sql = "SELECT SUM(OverMinutes) FROM WF_CH WHERE  OverMinutes <0";
            int beforeOver = BP.DA.DBAccess.RunSQLReturnValInt(sql, 0);

            //OverMinutes大于0表明逾期
            sql = "SELECT SUM(OverMinutes) FROM WF_CH WHERE OverMinutes >0 ";
            int afterOver = BP.DA.DBAccess.RunSQLReturnValInt(sql, 0);


            sql = "SELECT SUM (ASNum) AS ASNum , SUM(CSNum) CSNum ,SUM(AllNum) AllNum FROM V_TOTALCH  ";
            System.Data.DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

            int inTimeOverCount = 0;//按时
            int afterOverCount = 0;//超时
            int totalCount = 0;
            if (dt.Rows.Count == 1)
            {
                inTimeOverCount = int.Parse(string.IsNullOrEmpty(dt.Rows[0]["ASNum"].ToString()) ? "0" : dt.Rows[0]["ASNum"].ToString());
                afterOverCount = int.Parse(string.IsNullOrEmpty(dt.Rows[0]["CSNum"].ToString()) ? "0" : dt.Rows[0]["CSNum"].ToString());
                totalCount = int.Parse(string.IsNullOrEmpty(dt.Rows[0]["AllNum"].ToString()) ? "0" : dt.Rows[0]["AllNum"].ToString());
            }

            //求按时办结率.
            decimal asRate = 0;
            if (totalCount == 0)
                asRate = 0;
            else
                asRate = (decimal)inTimeOverCount / (decimal)totalCount * 100;

            //在运行的逾期.
            sql = "SELECT COUNT(WorkID) as Num  FROM WF_GenerWorkFlow WHERE SDTOfNode >='2015-07-06 10:43' AND WFState NOT IN (0,3)";
            int runningFlowOverTime = BP.DA.DBAccess.RunSQLReturnValInt(sql, 0);

            return "";
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