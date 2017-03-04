using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using BP.WF;
using BP.DA;
using BP.Web;
using BP.Sys;
using BP.En;
using BP.WF.Data;
using BP.WF.Template;

namespace BP.WF
{
    /// <summary>
    /// 抄送
    /// </summary>
    public class CCWork
    {
        #region 属性.
        /// <summary>
        /// 工作节点
        /// </summary>
        public WorkNode HisWorkNode = null;
        /// <summary>
        /// 节点
        /// </summary>
        public Node HisNode
        {
            get
            {
                return this.HisWorkNode.HisNode;
            }
        }
        /// <summary>
        /// 报表
        /// </summary>
        public GERpt rptGe
        {
            get
            {
                return this.HisWorkNode.rptGe;
            }
        }
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return this.HisWorkNode.WorkID;
            }
        }
        #endregion 属性.

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="wn"></param>
        public CCWork(WorkNode wn)
        {
            this.HisWorkNode = wn;
          //  AutoCC();
            CCByEmps();
        }
       
        /// <summary>
        /// 按照约定的字段 SysCCEmps 系统人员.
        /// </summary>
       
    }
}
