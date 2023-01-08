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

namespace BP.LIMS
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
                // StartSubFlows();
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

                if (this.HisNode.NodeID == 203)
                {
                    //设置样本的状态为分析完成.
                    DBAccess.RunSQL("UPDATE YB_YBFenXi SET YBSta=3 WHERE RefPK=" + this.WorkID);

                    //设置样本池也是完成状态.
                    YBFenXis ens = new YBFenXis();
                    ens.Retrieve(YBFenXiAttr.RefPK, this.WorkID);
                    foreach (YBFenXi en in ens)
                    {
                        DBAccess.RunSQL("UPDATE YB_Pool SET YBSta=3 WHERE OID=" + en.OID);
                    }

                    //检测是否有同类的完成
                    Let001Run();
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
        /// <summary>
        /// 让父流程运行到下一个节点上去.
        /// </summary>
        public void Let001Run()
        {
            //首先获得本次分析，有多少个委托？
            string sql = "SELECT DISTINCT WorkIDOfWT FROM YB_YBFenXi WHERE RefPK=" + this.WorkID;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);


            string webUserNo = BP.Web.WebUser.No;

            //遍历委托，执行检查，该委托下，是否已经全部完成了，样本池的状态.
            foreach (DataRow dr in dt.Rows)
            {
                Int64 workidOfWT = Int64.Parse(dr[0].ToString());
                sql = "SELECT count(*) as Num FROM YB_Pool WHERE YBSta!=3 where refpk=" + workidOfWT;

                //所有的都已分析完毕. 
                if (DBAccess.RunSQLReturnValInt(sql) == 0)
                {
                }
                else
                {
                    continue;
                }


                //让 委托流程，从等待分析节点，运动到 授权人签字节点.

                //求出来，当前人的待办 ， 让其登录.
                sql = "select fk_emp from wf_generworkerlist where ispass=0 and workid="+workidOfWT +" and fk_node=103";
                string empNo = DBAccess.RunSQLReturnString(sql);
                BP.WF.Dev2Interface.Port_Login(empNo);

                // 执行发送。
                BP.WF.Dev2Interface.Node_SendWork("001", workidOfWT, 0, null);

            }
        }

    }
}
