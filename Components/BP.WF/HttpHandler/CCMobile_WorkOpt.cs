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
using System.Collections;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class CCMobile_WorkOpt : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public CCMobile_WorkOpt(HttpContext mycontext)
        {
            this.context = mycontext;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public CCMobile_WorkOpt()
        {
        }
        /// <summary>
        /// 打包下载
        /// </summary>
        /// <returns></returns>
        public string Packup_Init()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.Packup_Init();
        }
        /// <summary>
        /// 选择接受人
        /// </summary>
        /// <returns></returns>
        public string HuiQian_SelectEmps()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.HuiQian_SelectEmps();
        }

        #region 审核组件.
        public string WorkCheck_GetNewUploadedAths()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.WorkCheck_GetNewUploadedAths();
        }
        public string WorkCheck_Init()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.WorkCheck_Init();
        }
        public string WorkCheck_Save()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.WorkCheck_Save();
        }
        #endregion 审核组件

        #region 会签.
        public string HuiQian_AddEmps()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.HuiQian_AddEmps();
        }
        public string HuiQian_Delete()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.HuiQian_Delete();
        }
        public string HuiQian_Init()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.HuiQian_Init();
        }
        public string HuiQian_SaveAndClose()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.HuiQian_SaveAndClose();
        }
        #endregion 会签

        #region 接收人选择器(限定接受人范围的).
        public string Accepter_Init()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.Accepter_Init();
        }
        public string Accepter_Save()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.Accepter_Save();
        }
        public string Accepter_Send()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.Accepter_Send();
        }
        #endregion 接收人选择器(限定接受人范围的).

        #region 接收人选择器(通用).
        public string AccepterOfGener_Init()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.AccepterOfGener_Init();
        }
        public string AccepterOfGener_AddEmps()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.AccepterOfGener_AddEmps();
        }
        public string AccepterOfGener_Delete()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.AccepterOfGener_Delete();
        }
        public string AccepterOfGener_Send()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.AccepterOfGener_Send();
        }

        //string flowNo, Int64 workID, int unSendToNode = 0
        public string AccepterOfGener_UnSend()
        {
            return Dev2Interface.Flow_DoUnSend(this.GetRequestVal("flowNo"), int.Parse(this.GetRequestVal("WorkID")));
        }
        #endregion 接收人选择器(通用).

        #region 选择人员(通用).
        /// <summary>
        /// 将要去掉.
        /// </summary>
        /// <returns></returns>
        public string SelectEmps()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.SelectEmps_Init();
        }
        public string SelectEmps_Init()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.SelectEmps_Init();
        }
        #endregion 选择人员(通用).

        /// <summary>
        /// 抄送初始化.
        /// </summary>
        /// <returns></returns>
        public string CC_Init()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            Hashtable ht = new Hashtable();
            ht.Add("Title", gwf.Title);

            //计算出来曾经抄送过的人.
            string sql = "SELECT CCToName FROM WF_CCList WHERE FK_Node=" + this.FK_Node + " AND WorkID=" + this.WorkID;
            DataTable mydt = DBAccess.RunSQLReturnTable(sql);
            string toAllEmps = "";
            foreach (DataRow dr in mydt.Rows)
                toAllEmps += dr[0].ToString() + ",";

            ht.Add("CCTo", toAllEmps);

            // 根据他判断是否显示权限组。
            if (BP.DA.DBAccess.IsExitsObject("GPM_Group") == true)
                ht.Add("IsGroup", "1");
            else
                ht.Add("IsGroup", "0");

            //返回流程标题.
            return BP.Tools.Json.ToJsonEntityModel(ht);
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
            string sql = "SELECT NO,NAME FROM Port_StationType ORDER BY NO";
            DataSet ds = new DataSet();
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Port_StationType";
            ds.Tables.Add(dt);

            //岗位.
            string sqlStas = "SELECT NO,NAME,FK_STATIONTYPE FROM Port_Station ORDER BY FK_STATIONTYPE,NO";
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
            stations = stations.Replace(";", ",");

            //权限组. 格式:  001,002,003,
            string groups = this.GetRequestVal("Groups");
            if (groups == null)
                groups = "";
            groups = groups.Replace(";", ",");

            //部门信息.  格式: 001,002,003,
            string depts = this.GetRequestVal("Depts");
            //标题.
            string title = this.GetRequestVal("TB_Title");
            //内容.
            string doc = this.GetRequestVal("TB_Doc");

            //调用抄送接口执行抄送.
            string ccRec = BP.WF.Dev2Interface.Node_CC_WriteTo_CClist(this.FK_Node, this.WorkID, title, doc, emps, depts, stations, groups);

            if (ccRec == "")
                return "没有抄送到任何人。";
            else
                return "本次抄送给如下人员：" + ccRec;

            //return "执行抄送成功.emps=(" + emps + ")  depts=(" + depts + ") stas=(" + stations + ") 标题:" + title + " ,抄送内容:" + doc;
        }
        #region 退回.
        public string Return_Init()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.Return_Init();
        }
        //执行退回.
        public string DoReturnWork()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.DoReturnWork();
        }
        #endregion 退回.

        #region xxx 界面 .
        #endregion xxx 界面方法.
    }
}
