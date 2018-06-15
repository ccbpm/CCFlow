using System;
using System.Threading;
using System.Collections;
using BP.Web.Controls;
using System.Data;
using BP.DA;
using BP.DTS;
using BP.En;
using BP.Web;
using BP.Sys;
using BP.WF;

namespace BP.Demo.FlowEvent
{
    /// <summary>
    /// 报销流程001
    /// 此类库必须放入到 BP.*.dll 才能被解析发射出来。
    /// </summary>
    public class F001: BP.WF.FlowEventBase
    {
        #region 属性.
        /// <summary>
        /// 重写流程标记
        /// </summary>
        public override string FlowMark
        {
            get { return "001,002";   }
        }
        #endregion 属性.

        #region 构造.
        /// <summary>
        /// 报销流程事件
        /// </summary>
        public F001()
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
          //  throw new            Exception("222");

            if (SystemConfig.CustomerNo != "CCFlow")
                return null;

            //相关的变量,
            // 当前的节点, 其他的变量请从 this.HisNode .
            int nodeID = this.HisNode.NodeID;    // int类型的ID.
            string nodeName = this.HisNode.Name; // 当前节点名称.

            switch (nodeID)
            {
                case 101:  //当是第1个节点的时候.
                    //  throw new Exception("不符合流程发送条件,阻止向下运行.");
                    //this.JumpToEmps = "zhangsan,lisi";
                    //this.JumpToNode = new Node(102);

                  //  this.JumpToNodeID = 103;
                   // this.JumpToEmps = "zhoupeng,liping";
                   // this.ND01_SaveAfter();

                    return "SendWhen事件已经执行成功。";
                default:
                    break;
            }
            return null;
        }
        #endregion 发送事件.

        /// <summary>
        /// 执行装载前的事件.
        /// </summary>
        /// <returns>return null 不刷新，任何数据，都会刷新数据.</returns>
        public override string FrmLoadBefore()
        {
            //throw new Exception("sssssssssssssss");
            return "执行成功.";
            //return base.FrmLoadBefore();
        }
        /// <summary>
        /// 保存后执行的事件
        /// </summary>
        /// <returns></returns>
        public override string SaveAfter()
        {
            switch (this.HisNode.NodeID)
            {
                case 101:
                    this.ND01_SaveAfter();
                    break;
                default:
                    break;
            }
            return base.SaveAfter();
        }
        /// <summary>
        /// 节点保存后事件
        /// 方法命名规则为:ND+节点名_事件名.
        /// </summary>
        public void ND01_SaveAfter()
        {

            if (DBAccess.IsExitsObject("ND101Dtl1") == false)
                return;

            if (DBAccess.IsExitsTableCol("ND101Dtl1", "XiaoJi") == false)
                return;

            if (DBAccess.IsExitsObject("ND101") == false)
                return;

          //  string val=this.getva

            //求出明细表的合计.
            float hj = BP.DA.DBAccess.RunSQLReturnValFloat("SELECT SUM(XiaoJi) as Num FROM ND101Dtl1 WHERE RefPK=" + this.OID, 0);

            //更新合计小写 , 把合计转化成大写.
            string sql = "UPDATE ND101 SET DaXie='" + BP.DA.DataType.ParseFloatToCash(hj) + "',HeJi="+hj+"  WHERE OID=" + this.OID;
            BP.DA.DBAccess.RunSQL(sql);

            sql = "UPDATE ND1Rpt SET DaXie='" + BP.DA.DataType.ParseFloatToCash(hj) + "',HeJi=" + hj + "  WHERE OID=" + this.OID;
            BP.DA.DBAccess.RunSQL(sql);

            //if (1 == 2)
            //    throw new Exception("@执行错误xxxxxx.");
            //如果你要向用户提示执行成功的信息，就给他赋值，否则就不必赋值。
            //this.SucessInfo = "执行成功提示.";
        }
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
                int toNodeID = this.SendReturnObjs.VarToNodeID; // 到达节点id.
                string toNodeName = this.SendReturnObjs.VarToNodeName; // 到达节点名称。
                string acceptersID = this.SendReturnObjs.VarAcceptersID; // 接受人员id, 多个人员会用 逗号分看 ,比如 zhangsan,lisi。
                string acceptersName = this.SendReturnObjs.VarAcceptersName; // 接受人员名称，多个人员会用逗号分开比如:张三,李四.

                //执行向其他系统写入待办.
                /*
                 * 在这里需要编写你的业务逻辑，根据上面组织的变量.
                 */

                if (this.HisNode.NodeID == 102)
                {
                    /*根据不同的节点，执行不同的业务逻辑*/
                }

                //返回.
                return base.SendSuccess();
            }
            catch(Exception ex)
            {
                throw new Exception("向其他系统写入待办失败，详细信息："+ex.Message);
            }
        }
         
    }
}
