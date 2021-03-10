using BP.DA;
using BP.En;
using BP.Port;
using BP.Sys;
using BP.WF.Data;
using BP.WF.Template;
using System;
using System.Collections;
using System.Data;
using System.Reflection;

namespace BP.WF.DTS
{
    /// <summary>
    /// Method 的摘要说明
    /// </summary>
    public class Auto_Rpt_Dtl_DTS : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public Auto_Rpt_Dtl_DTS()
        {
            this.Title = "自动报表的发送";
            this.Help = "自动报表配置到WF_AutoRpt, 与WF_AutoRptDtl中，读取之后进行发送消息或者数据.";
            this.GroupName = "流程自动执行定时任务";

        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
        }
        /// <summary>
        /// 当前的操纵员是否可以执行这个方法
        /// </summary>
        public override bool IsCanDo
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {
            //获得定时任务信息.
            AutoRpts rpts = new AutoRpts();
            rpts.RetrieveAllFromDBSource();

            int nowInt = int.Parse(DateTime.Now.ToString("HHmm"));
            //比如: 2009
            string strHours = DateTime.Now.ToString("yyyy-MM-dd HH:");

            string msg = "";

            foreach (AutoRpt rpt in rpts)
            {
                #region 判断是否到了发起时间.
                if (rpt.Dots.Contains(strHours) == true)
                    continue;

                //StartDT 格式:  20:02,18:02
                string[] strs = rpt.StartDT.Split(',');
                bool isHave = false;
                foreach (string str in strs)
                {
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;

                    string mystr = str.Replace(":", "");
                    int mynum = int.Parse(mystr);

                    if (nowInt >= mynum)
                    {
                        isHave = true;
                        break;
                    }
                }
                if (isHave == false)
                    continue;
                #endregion 判断是否到了发起时间.

                #region 组织内容.
                //组织内容.
                string title = rpt.Name;

                //获得消息.
                AutoRptDtls dtls = new AutoRptDtls();
                dtls.Retrieve(AutoRptDtlAttr.AutoRptNo, rpt.No);

                //找到可以发送的人员.
                #endregion 组织内容.

                #region 求出可以发起的人员,并执行发送.
                string empOfSQLs = BP.WF.Glo.DealExp(rpt.ToEmpOfSQLs, null);
                DataTable dtEmp = DBAccess.RunSQLReturnTable(empOfSQLs);
                foreach (DataRow dr in dtEmp.Rows)
                {
                    //执行登录.
                    string empNo = dr["No"].ToString();
                    BP.WF.Dev2Interface.Port_Login(empNo);

                    //求出内容.
                    string docs = "";
                    foreach (AutoRptDtl dtl in dtls)
                    {
                        string sql = dtl.SQLExp.Clone().ToString();
                        BP.WF.Glo.DealExp(sql, null);

                        string val = DBAccess.RunSQLReturnStringIsNull(sql, "无");
                        docs += "\t\n" + dtl.Name + " (" + val + "): " + dtl.BeiZhu;

                        string url = dtl.UrlExp.Clone().ToString();
                        BP.WF.Glo.DealExp(url, null);
                        docs += " <a href='" + url + "'> 打开连接</a>";
                    }

                    string agentId = BP.Sys.SystemConfig.WX_AgentID ?? null;
                    if (agentId != null)
                    {
                        string accessToken = GPM.WeiXin.WeiXinEntity.getAccessToken();//获取 AccessToken

                        BP.GPM.Emp emp = new BP.GPM.Emp(empNo);
                        BP.GPM.WeiXin.MsgText msgText = new GPM.WeiXin.MsgText();
                        msgText.content = docs;
                        msgText.Access_Token = accessToken;
                        msgText.agentid = BP.Sys.SystemConfig.WX_AgentID;
                        msgText.touser = emp.No;
                        msgText.safe = "0";

                        //执行发送
                        BP.GPM.WeiXin.Glo.PostMsgOfText(msgText);
                    }
                }
                #endregion 求出可以发起的人员.并执行发送

                //更新时间点.
                if (rpt.Dots.Length > 3999)
                    rpt.Dots = rpt.Dots.Substring(200);
                rpt.Dots = rpt.Dots + "," + strHours + ",";
                rpt.Update();
            }

            return "执行成功.";

        }
    }
}
