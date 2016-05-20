using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BP.DA;
using BP.WF;
using BP.Web;

namespace CCFlow.AppDemoLigerUI
{
    public partial class ProjectCheck : System.Web.UI.Page
    {
        /// <summary>
        /// 获取传入参数
        /// </summary>
        /// <param name="param">参数名</param>
        /// <returns></returns>
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(Request[param], System.Text.Encoding.UTF8);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string method = string.Empty;
            //返回值
            string s_responsetext = string.Empty;
            if (string.IsNullOrEmpty(Request["method"]))
                return;

            method = Request["method"].ToString();
            switch (method)
            {
                case "iscanstartthisflow"://判断是否可以发起流程
                    s_responsetext = IsCanStartThisFlow();
                    break;
                case "loadoverflowdata"://办结数据
                    s_responsetext = LoadOverFlowData();
                    break;
            }
            if (string.IsNullOrEmpty(s_responsetext))
                s_responsetext = "";
            //组装ajax字符串格式,返回调用客户端
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "text/html";
            Response.Expires = 0;
            Response.Write(s_responsetext);
            Response.End();
        }

        /// <summary>
        /// 获取办结项目信息
        /// </summary>
        /// <returns></returns>
        private string LoadOverFlowData()
        {
            string FK_Flow = getUTF8ToString("FK_Flow");
            string ProjNo = getUTF8ToString("ProjNo");
            string keyWords = getUTF8ToString("keyWords");

            string sql = "select * from V_FlowData where 1=2";
            //局对分公司的检查
            if (!string.IsNullOrEmpty(FK_Flow) && FK_Flow == "139")
            {
                sql = "select a.*,b.Name UserName,c.Name DeptName from ND139Rpt a,Port_Emp b,Port_Dept c where a.FlowStarter=b.No and a.FK_Dept=c.No and a.WFSta=1";
            }
            //分公司对项目的检查
            if (!string.IsNullOrEmpty(FK_Flow) && FK_Flow == "138")
            {
                sql = "select a.*,b.Name UserName,c.Name DeptName from ND138Rpt a,Port_Emp b,Port_Dept c where a.FlowStarter=b.No and a.FK_Dept=c.No and a.WFSta=1";
            }

            //项目编号
            if (!string.IsNullOrEmpty(ProjNo))
            {
                sql += " and a.ProjNo='" + ProjNo + "'";
            }
            else if (!string.IsNullOrEmpty(WebUser.FK_Dept))
            {
                //如果不使用项目编号就是用部门级别来控制
                string depts = GetPersonDeptAndChild();
                sql += " and a.FK_Dept in (" + depts + ")";
            }
            //关键字
            if (!string.IsNullOrEmpty(keyWords))
            {
                sql += " and (a.XMMC like '%" + keyWords + "%' or a.BiaoTi like '%" + keyWords + "%')";
            }
            sql += " order by a.FlowStartRDT DESC";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return GetEasyUIJson(dt);
        }

        /// <summary>
        /// 获取本部门与子级部门
        /// </summary>
        /// <returns></returns>
        private string GetPersonDeptAndChild()
        {
            string sql = "select gsid from V_ORG where bmid=" + WebUser.FK_Dept;
            string curDeptNo = DBAccess.RunSQLReturnStringIsNull(sql, WebUser.FK_Dept);

            string strDepts = "'" + curDeptNo + "'";
            GetChildDept(curDeptNo, ref strDepts);
            return strDepts;
        }

        /// <summary>
        /// 增加子级
        /// </summary>
        /// <param name="parentNo"></param>
        /// <param name="depts"></param>
        private void GetChildDept(string parentNo, ref string strDepts)
        {
            BP.Port.Depts depts = new BP.Port.Depts(parentNo);
            if (depts != null && depts.Count > 0)
            {
                foreach (BP.Port.Dept item in depts)
                {
                    strDepts += ",'" + item.No + "'";
                    GetChildDept(item.No, ref strDepts);
                }
            }
        }

        /// <summary>
        /// 判断是否有权限发起
        /// </summary>
        /// <returns></returns>
        private string IsCanStartThisFlow()
        {
            string FK_Flow = getUTF8ToString("FK_Flow");
            Flow flow = new Flow();
            flow.RetrieveByAttr("No", FK_Flow);

            string reVal = "{FlowName:'" + flow.Name + "',IsCan:'false'}";
            bool isRight = BP.WF.Dev2Interface.Flow_IsCanStartThisFlow(FK_Flow, WebUser.No);
            if (isRight)
                reVal = "{FlowName:'" + flow.Name + "',IsCan:'true'}";
            return reVal;
        }
        public string GetEasyUIJson(DataTable table)
        {
            string json = "{rows:" + Newtonsoft.Json.JsonConvert.SerializeObject(table) + ",total:" + table.Rows.Count + "}";
            return json;
        }
    }
}