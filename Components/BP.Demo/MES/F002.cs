using System;
using System.Threading;
using System.Collections;

using System.Data;
using BP.DA;
using BP.DTS;
using BP.En;
using BP.Web;
using BP.Sys;
using BP.WF;

namespace BP.MES
{
    /// <summary>
    /// 报销流程002
    /// 此类库必须放入到 BP.*.dll 才能被解析发射出来。
    /// </summary>
    public class F002 : BP.WF.FlowEventBase
    {
        #region 属性.
        /// <summary>
        /// 重写流程标记
        /// </summary>
        public override string FlowMark
        {
            get { return ",002,"; }
        }
        #endregion 属性.

        #region 构造.
        /// <summary>
        /// 报销流程事件
        /// </summary>
        public F002()
        {
        }
        #endregion 属性.

        #region 发送事件.
        /// <summary>
        /// 重写发送前事件
        /// </summary>
        /// <returns></returns>
        public override string SendWhen()


        {
            if (this.HisNode.NodeID == 201)
            {
                DBAccess.RunSQL("UPDATE ND201Dtl1 SET XTSta=1 WHERE  RefPK=(select PWorkID from ND1Rpt  WHERE OID=" + this.WorkID + ")");


            }

            if (this.HisNode.NodeID == 202)
            {
                    DBAccess.RunSQL("UPDATE ND201Dtl1 SET XTSta=2 WHERE  RefPK=(select PWorkID from ND1Rpt  WHERE OID=" + this.WorkID + ")");


            }
            if (this.HisNode.NodeID == 203)
            {
                DBAccess.RunSQL("UPDATE ND201Dtl1 SET XTSta=3 WHERE  RefPK=" + this.WorkID );


            }

            if (this.HisNode.NodeID == 204)
            {
              //  DBAccess.RunSQL("UPDATE ND201Dtl1 SET XTSta=4 WHERE XTSta=3 AND RefPK=" + this.WorkID);


            }

            return null;
        }
        #endregion 发送事件.

        /// <summary>
        /// 发送成功事件，发送成功时，把流程的待办写入其他系统里.
        /// </summary>
        /// <returns>返回执行结果，如果返回null就不提示。</returns>
        public override string SendSuccess()
        {
            try
            {
                // 组织必要的变量.
                Int64 workid = this.WorkID; // 工作id.w
                string flowNo = this.HisNode.FK_Flow; // 流程编号.
                int currNodeID = this.SendReturnObjs.VarCurrNodeID; //当前节点id
                int toNodeID = this.SendReturnObjs.VarToNodeID;     //到达节点id.
                string toNodeName = this.SendReturnObjs.VarToNodeName; // 到达节点名称。
                string acceptersID = this.SendReturnObjs.VarAcceptersID; // 接受人员id, 多个人员会用 逗号分看 ,比如 zhangsan,lisi。
                string acceptersName = this.SendReturnObjs.VarAcceptersName; // 接受人员名称，多个人员会用逗号分开比如:张三,李四.

                //执行向其他系统写入待办.
                /*
                 * 在这里需要编写你的业务逻辑，根据上面组织的变量.
                 */

                if (this.HisNode.NodeID == 201)
                {
                    StartSubFlows();
                    DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsEnable=0 WHERE  FK_Node !=201 and WorkID= " + workid);
                }

                if (this.HisNode.NodeID == 203)
                    DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsEnable = 0 WHERE FK_Node = 204 and WorkID= " + workid);
            
                //返回.
                return base.SendSuccess();
            }
            catch (Exception ex)
            {
                return base.SendSuccess();

                // throw new Exception("向其他系统写入待办失败，详细信息："+ex.Message);
            }
        }

        public void StartSubFlows()
        {
            //发起之前，删除有可能存在的垃圾数据.
            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.Retrieve(GenerWorkFlowAttr.PWorkID, this.WorkID);
            foreach (GenerWorkFlow item in gwfs)
            {
                BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(item.WorkID);
            }

            //求出来该订单下有多少个箱体.
            ND201Dtl1s dtls = new ND201Dtl1s();
            dtls.Retrieve(ND201Dtl1Attr.RefPK, this.WorkID);

            //当前实体的数据.
            ND2Rpt rpt002 = new ND2Rpt(this.WorkID);


            //遍历箱体，.
            foreach (ND201Dtl1 item in dtls)
            {
                 int i=item.Row.Count;
                //创建一个空白的workid.
                Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork("001", BP.Web.WebUser.No);

                //执行保存数据.
                ND1Rpt rpt001 = new ND1Rpt(workid);

                //把订单主表的字段，传入到箱体流程主表中去.
                rpt001.SetValByKey("DingDanHao", rpt002.GetValByKey("DingDanHao")); //订单编号
                rpt001.SetValByKey("PrjName", rpt002.GetValByKey("PrjName")); //项目名称
                rpt001.SetValByKey("KeHuMingCheng", rpt002.GetValByKey("KeHuMingCheng")); //客户名称
                rpt001.SetValByKey("QiXianXianDing", rpt002.GetValByKey("QiXianXianDing")); //期限限定
                rpt001.SetValByKey("JJCD", rpt002.GetValByKey("JJCD")); //紧急程度
                

                //把箱体信息，写入到 箱体流程 主表中去.
                rpt001.SetValByKey("XiangTiMingCheng", item.GetValByKey("XiangTiMingCheng")); //箱体名称.
                rpt001.SetValByKey("TuZhiBianHao", item.GetValByKey("TuZhiBianHao")); //图纸编号.
                rpt001.SetValByKey("TuZhiZhiTuRen", item.GetValByKey("TuZhiZhiTuRen")); //图纸制图人.
                //rpt001.SetValByKey("QiXianXianDing", item.GetValByKey("QiXianXianDing")); //期限限定.
                //rpt001.SetValByKey("JJCD", item.GetValByKey("JJCD")); //紧急程度.
                rpt001.Update();
                
            

                //求出来该箱体的装料单.
                ND201Dtl1Dtl1s dtl1s = new ND201Dtl1Dtl1s();
                dtl1s.Retrieve(ND201Dtl1Dtl1Attr.RefPK, item.OID);

                //把装料单据copy到子流程上去.
                ND101Dtl1 dtl101 = new ND101Dtl1();

                foreach (ND201Dtl1Dtl1 dtl1 in dtl1s)
                {
                    dtl101.Row = dtl1.Row;
                    dtl101.RefPK = workid;
                    dtl101.InsertAsOID(dtl1.OID);
                }

                //执行发送.
                BP.WF.Dev2Interface.Node_SendWork("001", workid);

                //设置父子关系.
                BP.WF.Dev2Interface.SetParentInfo("001", workid, this.WorkID, BP.Web.WebUser.No, 202, false);


             
                //BP.WF.Dev2Interface.Node_SetDraft2Todolist("001", workid);
            }
        }

    }
}
