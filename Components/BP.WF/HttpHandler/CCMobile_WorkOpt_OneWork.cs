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

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class CCMobile_WorkOpt_OneWork : DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CCMobile_WorkOpt_OneWork()
        {
           // BP.Web.WebUser.SheBei = "Mobile";

        }

        #region xxx 界面 .
        public string TimeBase_Init()
        {
            WF_WorkOpt_OneWork en = new WF_WorkOpt_OneWork();
            return en.TimeBase_Init();
        }
        /// <summary>
        /// 执行撤销操作.
        /// </summary>
        /// <returns></returns>
        public string TimeBase_UnSend()
        {
            //获取撤销到的节点
            int unSendToNode = this.GetRequestValInt("FK_Node");
            return BP.WF.Dev2Interface.Flow_DoUnSend(this.FK_Flow, this.WorkID, unSendToNode, this.FID);
            //WF_WorkOpt_OneWork en = new WF_WorkOpt_OneWork();
            //return en.OP_UnSend();
        }
        public string TimeBase_OpenFrm()
        {
            WF en = new WF();
            return en.Runing_OpenFrm();
        }
        #endregion xxx 界面方法.

    }
}
