using System;

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
