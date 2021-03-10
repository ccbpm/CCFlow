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
    public class WF_WorkOpt_StartGuide : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_WorkOpt_StartGuide()
        {

        }

        //执行启动,并设置父子关系.
        public string ParentFlowModel_StartIt()
        {
            Int64 pworkid = this.GetRequestValInt64("PWorkID");
            GenerWorkFlow gwfP = new GenerWorkFlow(pworkid);

            Flow fl = new Flow(this.FK_Flow);
            Work wk=fl.NewWork();

            BP.WF.Dev2Interface.SetParentInfo(this.FK_Flow,this.WorkID,pworkid,null,gwfP.FK_Node,false);
            return wk.OID.ToString();
        }


    }
}
