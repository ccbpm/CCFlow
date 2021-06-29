﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BP.Sys;
using BP.En;

namespace BP.WF
{
    /// <summary>
    /// 单据业务流程
    /// </summary>
    public class CCBill_FlowEvent
    {

        /// <summary>
        /// 流程结束的时候要处理的事件
        /// </summary>
        /// <param name="wn"></param>
        /// <param name="paras"></param>
        public static void DealFlowOver(WorkNode wn, string paras)
        {
            #region 如果是修改基础资料流程。
            ///如果是修改基础资料流程.
            if (wn.HisGenerWorkFlow.GetParaBoolen("FlowBaseData", false) == true)
            {
                Int64 dictWorkID = wn.HisGenerWorkFlow.PWorkID;
                string dictFrmID = wn.HisGenerWorkFlow.PFlowNo;

                string flowFrmID = "ND" + int.Parse(wn.HisFlow.No + "01");

                // MapData md = new MapData(flowFrmID);

                //同步主表数据.
                Row row = wn.rptGe.Row;

                //创建实体.
                GEEntity geEn = new GEEntity(dictFrmID, dictWorkID);
                foreach (string key in row.Keys)
                {
                    if (key.IndexOf("bak") == 0)
                        continue;

                    if (BP.WF.Glo.FlowFields.Contains("," + key + ",") == true)
                        continue;

                    //设置值.
                    geEn.SetValByKey(key, row.GetValByKey(key));
                }
                geEn.Update(); //更新.

                //更新从表.
                //  MapDtls dtls = new MapDtls(flowFrmID);
            }
            #endregion 如果是修改基础资料流程。

            #region 如果是新建实体流程.
            if (wn.HisGenerWorkFlow.GetParaBoolen("FlowNewEntity", false) == true)
            {

                string menuNo = wn.HisGenerWorkFlow.GetParaString("MenuNo");

                BP.CCBill.Template.MethodFlowNewEntity menu = new CCBill.Template.MethodFlowNewEntity(menuNo);

                //创建工作，并copy数据过去.
                Row row = wn.rptGe.Row;
                BP.CCBill.Dev2Interface.SaveDictWork(menu.FrmID, wn.WorkID, row);

                //替换实体名字.
                if (row.ContainsKey("DictName") == true)
                {
                    string dictName = row["DictName"].ToString();
                    GEEntity ge = new GEEntity(menu.FrmID, wn.WorkID);
                    ge.SetValByKey("Title", dictName);
                    ge.Update();
                }

                //GenerWorkFlow gwf = wn.HisGenerWorkFlow;
                //gwf.PWorkID = gwf.WorkID;
                //gwf.PFlowNo = menu.FrmID;

                //写入日志.
                string myparas = "";
                myparas += "@PWorkID=" + wn.WorkID;
                myparas += "@PFlowNo=" + wn.HisFlow.No;
                myparas += "@PNodeID=" + wn.HisNode.NodeID;
                BP.CCBill.Dev2Interface.Dict_AddTrack(menu.FrmID, null, wn.WorkID, CCBill.FrmActionType.StartRegFlow, "流程创建实体",
                    myparas, wn.HisFlow.No, wn.HisFlow.Name, wn.HisNode.NodeID, wn.WorkID);
            }
            #endregion 如果是新建实体流程.

        }

        public static void DoFlow(string flowMark, WorkNode wn, string paras)
        {
            ///流程结束之前.
            if (flowMark.Equals(EventListFlow.FlowOverBefore) == true)
                DealFlowOver(wn, paras);
        }

    }
}
