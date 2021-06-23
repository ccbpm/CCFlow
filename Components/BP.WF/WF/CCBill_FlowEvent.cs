using System;
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
                Int64  frmWorkID = BP.CCBill.Dev2Interface.CreateBlankDictID(menu.FrmID, BP.Web.WebUser.No, row);

                //提交工作.
                BP.CCBill.Dev2Interface.SubmitWork(menu.FrmID, frmWorkID);
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
