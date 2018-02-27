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
    public class CCMobile_MyFlow : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public CCMobile_MyFlow(HttpContext mycontext)
        {
            this.context = mycontext;
        }
        /// <summary>
        /// 获得工作节点
        /// </summary>
        /// <returns></returns>
        public string GenerWorkNode()
        {
            WF_MyFlow en = new WF_MyFlow(this.context);
            return en.GenerWorkNode();

            BP.Sys.MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.FK_MapData, "");
        }
        /// <summary>
        /// 获得toolbar
        /// </summary>
        /// <returns></returns>
        public string InitToolBar()
        {
            WF_MyFlow en = new WF_MyFlow(this.context);
            return en.InitToolBarForMobile();
        }
        public string MyFlow_Init()
        {
            WF_MyFlow en = new WF_MyFlow(this.context);
            return en.MyFlow_Init();
        }
        public string MyFlow_StopFlow()
        {
            WF_MyFlow en = new WF_MyFlow(this.context);
            return en.MyFlow_StopFlow();
        }
        public string Save()
        {
            WF_MyFlow en = new WF_MyFlow(this.context);
            return en.Save();
        }
        public string Send()
        {
            WF_MyFlow en = new WF_MyFlow(this.context);
            return en.Send();
        }
        public string StartGuide_Init()
        {
            WF_MyFlow en = new WF_MyFlow(this.context);
            return en.StartGuide_Init();
        }
        public string FrmGener_Init()
        {
            WF_CCForm ccfrm = new WF_CCForm(this.context);
            return ccfrm.FrmGener_Init();
        }
        public string FrmGener_Save()
        {
            WF_CCForm ccfrm = new WF_CCForm(this.context);
            return ccfrm.FrmGener_Save();
        }

        public string MyFlowGener_Delete()
        {
            BP.WF.Dev2Interface.Flow_DoDeleteFlowByWriteLog(this.FK_Flow, this.WorkID, Web.WebUser.Name+"用户删除", true);
            return "删除成功...";
        }

        public string AttachmentUpload_Down()
        {
            WF_CCForm ccform = new WF_CCForm(this.context);
            return ccform.AttachmentUpload_Down();
        }
      
    }
}
