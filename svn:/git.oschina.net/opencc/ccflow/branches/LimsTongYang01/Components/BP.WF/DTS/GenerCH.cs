using System;
using System.Data;
using System.Collections;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.DA;
using BP.En;
using BP.Sys;
namespace BP.WF.DTS
{
    /// <summary>
    /// 生成考核数据
    /// </summary>
    public class GenerCH : Method
    {
        /// <summary>
        /// 生成考核数据
        /// </summary>
        public GenerCH()
        {
            this.Title = "生成考核数据（为所有的流程,根据最新配置的节点考核信息，生成考核数据。）";
            this.Help = "需要先删除运行时生成的数据，然后为每个流程的每个节点运行数据自动生成。";
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
                if (BP.Web.WebUser.No == "admin")
                    return true;
                return false;
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {
            string err = "";
            try
            {
                //删除现有的数据。
                BP.DA.DBAccess.RunSQL("DELETE FROM WF_CH");

                //查询全部的数据.
                BP.WF.Nodes nds = new Nodes();
                nds.RetrieveAll();

                foreach (Node nd in nds)
                {
                    string sql = "SELECT * FROM ND" + int.Parse(nd.FK_Flow) + "TRACK WHERE NDFrom=" + nd.NodeID + " ORDER BY WorkID, RDT ";
                    System.Data.DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                    string priRDT = null;
                    string sdt = null;
                    foreach (DataRow dr in dt.Rows)
                    {
                        //向下发送.
                        int atInt = (int)dr[BP.WF.TrackAttr.ActionType];
                        ActionType at = (ActionType)atInt;
                        switch (at)
                        {
                            case ActionType.Forward:
                            case ActionType.ForwardAskfor:
                            case ActionType.ForwardFL:
                            case ActionType.ForwardHL:
                                break;
                            default:
                                continue;
                        }

                        //相关的变量.
                        Int64 workid = Int64.Parse(dr[TrackAttr.WorkID].ToString());
                        Int64 fid = Int64.Parse(dr[TrackAttr.FID].ToString());

                        //当前的人员，如果不是就让其登录.
                        string fk_emp = dr[BP.WF.TrackAttr.EmpFrom] as string;
                        if (BP.Web.WebUser.No != fk_emp)
                        {
                            try
                            {
                                BP.WF.Dev2Interface.Port_Login(fk_emp);
                            }
                            catch (Exception ex)
                            {
                                err += "@人员错误:" + fk_emp + "可能该人员已经删除." + ex.Message;
                            }
                        }

                        //标题.
                        string title = BP.DA.DBAccess.RunSQLReturnStringIsNull("select title from wf_generworkflow where workid=" + workid, "");

                        ////调用他.
                        //Glo.InitCH2017(nd.HisFlow, nd, workid, fid, title, priRDT, sdt,
                        //    DataType.ParseSysDate2DateTime(dr[TrackAttr.RDT].ToString()));

                        priRDT = dr[TrackAttr.RDT].ToString();
                        sdt = "无";
                    }
                }
            }
            catch (Exception ex)
            {
                return "生成考核失败:" + ex.StackTrace;
            }

            //登录.
            BP.WF.Dev2Interface.Port_Login("admin");
            return "执行成功,有如下信息:" + err;
        }
    }
}
