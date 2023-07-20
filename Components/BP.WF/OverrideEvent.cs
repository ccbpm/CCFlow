using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.En;
using BP.WF;
using BP.WF.Data;
using BP.WF.Template;
using BP.WF.Template.SFlow;
using BP.Port;
using System.Drawing.Imaging;
using System.Drawing;
using System.Configuration;
using BP.Tools;
using BP.Difference;
using BP.WF.Admin;
using System.Web.UI.WebControls;

namespace BP.WF
{
    /// <summary>
    /// 事件重写.
    /// </summary>
    public class OverrideEvent
    {
        /// <summary>
        /// 流程事件-总体拦截器.
        /// </summary>
        /// <param name="eventMark">流程标记</param>
        /// <param name="wn">worknode</param>
        /// <param name="paras">参数</param>
        /// <param name="checkNote">审核意见.</param>
        /// <returns>执行信息.</returns>
        public static string DoIt(string eventMark, WorkNode wn, string paras, string checkNote, int returnToNodeID, string returnToEmps, string returnMsg)
        {
            //发送成功.
            if (eventMark.Equals(EventListNode.SendSuccess) == true)
                return SendSuccess(wn);

            //退回后事件.
            if (eventMark.Equals(EventListNode.ReturnAfter) == true)
                return ReturnAfter(wn, returnToNodeID, returnToEmps, returnMsg);

            // 流程结束事件.
            if (eventMark.Equals(EventListFlow.FlowOverAfter) == true)
                return FlowOverAfter(wn);

            return null;
        }
        /// <summary>
        /// 执行发送.
        /// </summary>
        /// <param name="wn"></param>
        /// <returns></returns>
        public static string SendSuccess(WorkNode wn)
        {
            if (SystemConfig.CustomerNo.Equals("TianYu") == true)
            {
                if (wn.HisNode.IsStartNode == false)
                    return null; //如果不是开始节点发送,就不处理.

                //模板目录.
                string sortNo = wn.HisFlow.FK_FlowSort;

                //找到系统编号.
                Admin.FlowSort fs = new Admin.FlowSort(sortNo);

                //子系统:当前目录的上一级目录必定是子系统,系统约定的.
                SubSystem system = new SubSystem(fs.ParentNo);

                //检查是否配置了?
                if (DataType.IsNullOrEmpty(system.TokenPiv) == true)
                    return null;

                //执行事件.
                DoPost("SendSuccess", wn, system);
            }
            return null;
        }
        /// <summary>
        /// 执行退回操作.
        /// </summary>
        /// <param name="wn"></param>
        /// <param name="toNodeID"></param>
        /// <param name="toEmp"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string ReturnAfter(WorkNode wn, int toNodeID, string toEmp, string info)
        {
            if (SystemConfig.CustomerNo.Equals("TianYu") == true)
            {
                if (toNodeID.ToString().EndsWith("01") == false)
                    return null; //如果不是退回的开始节点.

                //模板目录.
                string sortNo = wn.HisFlow.FK_FlowSort;

                //找到系统编号.
                Admin.FlowSort fs = new Admin.FlowSort(sortNo);

                //子系统:当前目录的上一级目录必定是子系统,系统约定的.
                SubSystem system = new SubSystem(fs.ParentNo);

                //检查是否配置了?
                if (DataType.IsNullOrEmpty(system.TokenPiv) == true)
                    return null;

                //执行事件.
                DoPost("ReturnAfter", wn, system);
            }
            return null;
        }

        public static string FlowOverAfter(WorkNode wn)
        {
            if (SystemConfig.CustomerNo.Equals("TianYu") == true)
            {
                //模板目录.
                string sortNo = wn.HisFlow.FK_FlowSort;

                //找到系统编号.
                Admin.FlowSort fs = new Admin.FlowSort(sortNo);

                //子系统:当前目录的上一级目录必定是子系统,系统约定的.
                SubSystem system = new SubSystem(fs.ParentNo);

                //检查是否配置了?
                if (DataType.IsNullOrEmpty(system.TokenPiv) == true)
                    return null;

                //执行事件.
                DoPost("FlowOverAfter", wn, system);
            }
            return null;
        }
        /// <summary>
        /// 执行天宇的回调.
        /// </summary>
        /// <param name="eventMark">事件目录.</param>
        /// <param name="wn"></param>
        /// <param name="system"></param>
        /// <returns></returns>
        public static string DoPost(string eventMark, WorkNode wn, SubSystem system)
        {
            string myEventMark = "0";
            if (eventMark.Equals("ReturnAfter"))
                myEventMark = "3";
            if (eventMark.Equals("SendSuccess"))
                myEventMark = "1";
            if (eventMark.Equals("FlowOverAfter"))
                myEventMark = "2";

            string apiParas = system.ApiParas; //配置的json字符串.
            apiParas = apiParas.Replace("~", "\"");

            apiParas = apiParas.Replace("@WorkID", wn.WorkID.ToString()); //工作ID.
            apiParas = apiParas.Replace("@FlowNo", wn.HisFlow.No); //流程编号.
            apiParas = apiParas.Replace("@NodeID", wn.HisNode.NodeID.ToString()); //节点ID.
            apiParas = apiParas.Replace("@TimeSpan", DBAccess.GenerOID("TS").ToString()); //时间戳.
            apiParas = apiParas.Replace("@EventMark", myEventMark); //稳超定义的，事件标记.
            apiParas = apiParas.Replace("@EventID", eventMark); //EventID 定义的事件类型.
            apiParas = apiParas.Replace("@SPYJ", "xxx无xx"); //审批意见.

            //如果表单的数据有,就执行一次替换.
            apiParas = Glo.DealExp(apiParas, wn.rptGe);

            //需要补充算法. @WenChao.
            string sign = "密钥:" + system.TokenPiv + ",公约" + system.TokenPublie;
            apiParas = apiParas.Replace("@sign", sign); //签名，用于安全验证.

            if (apiParas.Contains("@") == true)
                return "err@配置的参数没有替换下来:" + apiParas;

            //替换url参数.
            string url = system.CallBack; //回调的全路径.
            url = Glo.DealExp(url, wn.rptGe);

            //执行post.
            string data = BP.Tools.PubGlo.HttpPostConnect(url, apiParas, system.RequestMethod, system.IsJson);
            return data;
        }
    }
}
