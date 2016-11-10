using System;
using System.Web;
using BP.DA;
using BP.Web;
using System.Data;
using BP.WF;

namespace CCFlow.WF.WorkOpt.OneWork
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


        public string Msg
        {
            get
            {
                string str = context.Request.QueryString["TB_Msg"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }


        
        public string UserName
        {
            get
            {
                string str = context.Request.QueryString["UserName"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }


        public string Title
        {
            get
            {
                string str = context.Request.QueryString["Title"];
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



        public string Name
        {
            get
            {
                string str = BP.Web.WebUser.Name;
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
                    case "FlowBBSUser": //获得当前用户no.
                        msg = this.FlowBBSUser();
                        break;
                    case "FlowBBSDept": //获得用户部门.
                        msg = this.FlowBBSDept();
                        break;
                    case "FlowBBSList": //获得流程评论列表.
                        msg = this.FlowBBSList();
                        break;
                    case "FlowBBSSave": //提交评论..
                        msg = this.FlowBBSSave();
                        break;
                    case "FlowBBSDelete": //删除评论.
                        msg = BP.WF.Dev2Interface.Flow_BBSDelete(this.FK_Flow, this.MyPK,WebUser.No);
                        break;
                    case "FlowBBSCheck": //查看某一用户评论.
                          msg = this.FlowBBSCheck();
                        break;
                    case "FlowBBSReplay": //评论回复.
                        msg = this.FlowBBSReplay();
                        break;
                    case "FlowBBSCount": //统计评论条数
                        msg = this.FlowBBSCount();
                        break;
                    case "FlowBBSUserName": //获得当前用户名称.
                        msg = this.FlowBBSUserName();
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
        /// 获得发起的BBS评论.
        /// </summary>
        /// <returns></returns>
        public string FlowBBSList()
        {        
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE ActionType=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "ActionType";
            ps.Add("ActionType", (int)BP.WF.ActionType.FlowBBS);

            //转化成json
            return BP.Tools.Json.ToJson(BP.DA.DBAccess.RunSQLReturnTable(ps));
        }

        public string FlowBBSUser()
        {
            string name = string.Empty;
            name=BP.Web.WebUser.No;
            return name;

        }
        public string FlowBBSUserName()
        {
            string name = string.Empty;
            name = BP.Web.WebUser.Name;
            return name;

        }

        public string FlowBBSDept()
        {

            Paras ps = new Paras();
            ps.SQL = "select a.name from port_dept a INNER join port_emp b on b.FK_Dept=a.no and b.No='"+this.UserName+"'";

            return BP.Tools.Json.ToJson(BP.DA.DBAccess.RunSQLReturnString(ps));
        }

        /// 查看某一用户的评论.
        public  string FlowBBSCheck()
        {
            Paras pss = new Paras();
            pss.SQL = "SELECT * FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE ActionType=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "ActionType AND  EMPFROMT='"+this.UserName+"'";
            pss.Add("ActionType", (int)BP.WF.ActionType.FlowBBS);
            return BP.Tools.Json.ToJson(BP.DA.DBAccess.RunSQLReturnTable(pss));

       }
        /// <summary>
        /// 提交评论.
        /// </summary>
        /// <returns></returns>
        public string FlowBBSSave()
        {
            string msg = this.GetValFromFrmByKey("TB_Msg");
            string mypk = BP.WF.Dev2Interface.Flow_BBSAdd(this.FK_Flow, this.WorkID, this.FID, msg, WebUser.No, WebUser.Name);
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE MyPK=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "MyPK";
            ps.Add("MyPK", mypk);
            return BP.Tools.Json.ToJson( BP.DA.DBAccess.RunSQLReturnTable(ps));
        }

        /// <summary>
        /// 回复评论.
        /// </summary>
        /// <returns></returns>
        public string FlowBBSReplay()
        {
              SMS  sms = new SMS();
              sms.RetrieveByAttr(SMSAttr.MyPK, MyPK);
              sms.MyPK = DBAccess.GenerGUID();
              sms.RDT = DataType.CurrentDataTime;
              sms.SendToEmpNo = this.UserName;  
              sms.Sender = WebUser.No;
              sms.Title = this.Title;
              sms.DocOfEmail = this.Msg;
              sms.Insert(); 
              return null;
        }
        /// <summary>
        /// 统计评论条数.
        /// </summary>
        /// <returns></returns>

        public string FlowBBSCount()
        {
  
            Paras ps = new Paras();
            ps.SQL = "SELECT COUNT(ActionType) FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE ActionType=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "ActionType";
            ps.Add("ActionType", (int)BP.WF.ActionType.FlowBBS);
            string count = BP.DA.DBAccess.RunSQLReturnValInt(ps).ToString();
            return  count;
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