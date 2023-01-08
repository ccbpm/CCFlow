using System;
using System.Threading;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web;
using BP.Sys;
using BP.WF;

namespace BP.JianYu
{
    /// <summary>
    /// 报销流程069
    /// 此类库必须放入到 BP.*.dll 才能被解析发射出来。
    /// </summary>
    public class F069 : BP.WF.FlowEventBase
    {
        #region 属性.
        /// <summary>
        /// 重写流程标记
        /// </summary>
        public override string FlowMark
        {
            get { return ",069,"; }
        }
        #endregion 属性.

        #region 构造.
        /// <summary>
        /// 报销流程事件
        /// </summary>
        public F069()
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
                Int64 workid = this.WorkID; // 工作id.
                string flowNo = this.HisNode.FK_Flow; // 流程编号.
                int currNodeID = this.SendReturnObjs.VarCurrNodeID; //当前节点id
                int toNodeID = this.SendReturnObjs.VarToNodeID;     //到达节点id.
                string toNodeName = this.SendReturnObjs.VarToNodeName; // 到达节点名称。
                string acceptersID = this.SendReturnObjs.VarAcceptersID; // 接受人员id, 多个人员会用 逗号分看 ,比如 zhangsan,lisi。
                string acceptersName = this.SendReturnObjs.VarAcceptersName; // 接受人员名称，多个人员会用逗号分开比如:张三,李四.

                // 当前的节点, 其他的变量请从 this.HisNode .
                int nodeID = this.HisNode.NodeID;    // int类型的ID.
                if (this.HisNode.NodeID == 6901)
                {
                    // 求出来所有的产品..
                    string sql = "SELECT * FROM ND6901Dtl1 WHERE RefPK=" + this.WorkID;
                    DataTable dt = DBAccess.RunSQLReturnTable(sql);

                    //开始启动子流程.
                    string intfos = "产品研发子流程启动信息如下：<br/>";
                    foreach (DataRow dr in dt.Rows)
                    {
                        //创建子流程.WorkID.
                        Int64 workidSubFlow = BP.WF.Dev2Interface.Node_CreateBlankWork("070");

                        //求出来：产品多个负责人，就用,都分开. zhangsan,lisi
                        string fuzren = dr["FuZeRen"].ToString();

                        //给子流程的主表，设置产品信息数据.
                        GEEntity rpt070 = new GEEntity("ND70Rpt", workidSubFlow);
                        // rpt070.Copy(dr);
                     //   rpt070.SetValByKey("ChanPinMingCheng", dr["ChanPinMingCheng"].ToString());
                        rpt070.SetValByKey("Tel", dr["Tel"].ToString());
                        rpt070.SetValByKey("FuZeRen", dr["FuZeRen"].ToString());
                        rpt070.Update();

                        //设置父子关系.
                        BP.WF.Dev2Interface.SetParentInfo("070", workidSubFlow, this.WorkID, BP.Web.WebUser.No, 6902, false);
                        //执行发送，到第2个节点上去. 
                        intfos +=  "<br>子流程: "+ BP.WF.Dev2Interface.Node_SendWork("070", workidSubFlow, 7002, fuzren).ToMsgOfText();
                    }

                    return intfos;
                }

                //返回.
                return base.SendSuccess();
            }
            catch (Exception ex)
            {
                return base.SendSuccess();

                // throw new Exception("向其他系统写入待办失败，详细信息："+ex.Message);
            }
        }

    }
}
