using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BP.Sys;
using BP.En;
using BP.WF.Data;
using BP.DA;
using System.Data;
using BP.WF;

namespace CCFlow.WF.Admin.FoolFormDesigner.Rpt
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
        /// 主键
        /// </summary>
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
                    case "S0_RptList_Init": //初始化.
                        msg = this.S0_RptList_Init();
                        break;
                    case "S0_RptList_New": //创建一个报表.
                        msg = this.S0_RptList_New();
                        break;
                    case "S0_RptList_Delete": //删除报表.
                        msg = this.S0_RptList_Delete();
                        break;
                    case "S0_RptList_Edit": //删除报表.
                        msg = this.S0_RptList_Edit();
                        break;
                    case "S0_OneRpt_Init": //获得单个流程报表的初始化信息
                        msg = this.S0_OneRpt_Init();
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
        }
        /// <summary>
        /// 获得流程报表列表
        /// </summary>
        /// <returns></returns>
        public string S0_RptList_Init()
        {
            BP.WF.Rpt.MapRpts ens = new BP.WF.Rpt.MapRpts();
            ens.Retrieve(BP.WF.Rpt.MapRptAttr.FK_Flow, this.FK_Flow);
            if (ens.Count == 0)
            {
                BP.WF.Rpt.MapRpt en = new BP.WF.Rpt.MapRpt();
                en.No = "ND" + int.Parse(this.FK_Flow) + "MyRpt";
                en.Name = "流程报表默认";
                en.FK_Flow = this.FK_Flow;
                en.Insert();
                ens.Retrieve(BP.WF.Rpt.MapRptAttr.FK_Flow, this.FK_Flow);
            }
            return ens.ToJson();
        }

        /// <summary>
        /// 获得单个流程报表的初始化信息
        /// </summary>
        /// <returns></returns>
        public string S0_OneRpt_Init()
        {
            string no = this.GetRequestVal("No");
            BP.WF.Rpt.MapRpt ens = new BP.WF.Rpt.MapRpt();
            ens.Retrieve(BP.WF.Rpt.MapRptAttr.No, no);
            if (ens == null)
            {
                BP.WF.Rpt.MapRpt en = new BP.WF.Rpt.MapRpt();
                en.No = "ND" + int.Parse(this.FK_Flow) + "MyRpt";
                en.Name = "流程报表默认";
                en.FK_Flow = this.FK_Flow;
                en.Insert();
                ens.Retrieve(BP.WF.Rpt.MapRptAttr.No, no);
            }
            return ens.ToJson();
        }

        /// <summary>
        /// 创建新报表
        /// </summary>
        /// <returns></returns>
        public string S0_RptList_New()
        {
            string no = this.GetRequestVal("No");
            string name = this.GetRequestVal("Name");
            string note = this.GetRequestVal("Note");
            //BP.WF.Rpt.MapRpt en = new BP.WF.Rpt.MapRpt();
            //en.No = no;
            //if (en.RetrieveFromDBSources() == 1)
            //{
            //    return "err@编号{" + en.No + "}已经存在.";
            //}

            //en.Name = name;
            //en.Note = note;
            //en.Insert();

            string mapRpt = no;
            MapData mapData = new MapData();
            mapData.No = mapRpt;
            mapData.Name = name;
            mapData.Note = mapData.Note;
            mapData.Insert();


            BP.WF.Rpt.MapRpt rpt = new BP.WF.Rpt.MapRpt();
            rpt.No = mapRpt;
            rpt.FK_Flow = FK_Flow;
            rpt.Note = note;
            rpt.Name = name;
            Flow flow = new Flow(this.FK_Flow);
            rpt.PTable = flow.PTable == "" ? "ND" + this.FK_Flow.TrimStart('0') + "RPT" : flow.PTable;

            rpt.Update();

            string sql = "";

            sql = "insert into Sys_MapAttr  select '" + no + "_'+KeyOfEn,'" + no + "',KeyOfEn,Name,DefVal,UIContralType,MyDataType,LGType,UIWidth,UIHeight,MinLen,MaxLen,UIBindKey,UIRefKey,UIRefKeyText,UIVisible,UIIsEnable,UIIsLine,UIIsInput,Idx,GroupID,IsSigan,x,y,GUID,Tag,EditType,ColSpan,AtPara from Sys_MapAttr WHERE FK_MapData IN ( SELECT 'ND' + cast(NodeID as varchar(20)) FROM WF_Node WHERE FK_Flow='"+flow.No+"')";
            //DBAccess.RunSQL(sql); // 写入 Sys_MapAttr
            //
            sql = "select * from Sys_MapAttr WHERE FK_MapData IN ( SELECT 'ND' + cast(NodeID as varchar(20)) FROM WF_Node WHERE FK_Flow='" + flow.No + "')";
            
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            sql = "SELECT KeyOfEn FROM Sys_MapAttr WHERE FK_MapData='" + no + "'";
            DataTable dtExits = DBAccess.RunSQLReturnTable(sql);
            string pks = "@";
            foreach (DataRow dr in dtExits.Rows)
                pks += dr[0] + "@";

            foreach (DataRow dr in dt.Rows)
            {
                string mypk = dr["MyPK"].ToString();
                if (pks.Contains("@" + dr["KeyOfEn"].ToString() + "@"))
                    continue;

                pks += dr["KeyOfEn"].ToString() + "@";

                BP.Sys.MapAttr ma = new BP.Sys.MapAttr(mypk);
                ma.MyPK = no + "_" + ma.KeyOfEn;
                ma.FK_MapData = no;
                ma.UIIsEnable = false;

                if (ma.DefValReal.Contains("@"))
                {
                    /*如果是一个有变量的参数.*/
                    ma.DefVal = "";
                }

                try
                {
                    ma.Insert();
                }
                catch
                {
                }
            }

            //DoCheck_CheckRpt(no, this.FK_Flow, name, rpt.PTable);
            return "@创建成功.";
        }

        /// <summary>
        /// 删除报表
        /// </summary>
        /// <returns></returns>
        public string S0_RptList_Delete()
        {
            string no = this.GetRequestVal("No");
            if (no == "ND" + int.Parse(this.FK_Flow) + "MyRpt")
                return "err@默认报表，不能删除。";

            BP.WF.Rpt.MapRpt en = new BP.WF.Rpt.MapRpt();
            en.No = no;
            en.Delete();
            return "@删除成功.";
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        public string S0_RptList_Edit()
        {
            string no = this.GetRequestVal("No");

            BP.WF.Rpt.MapRpt en = new BP.WF.Rpt.MapRpt();
            en.No = no;
            en.Retrieve();

            en.Name = this.GetValFromFrmByKey("Name");
            en.Note = this.GetValFromFrmByKey("Note");
            en.Update();

            return "@保存成功.";
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