using System;
using System.Collections.Generic;
using System.Collections;
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
using System.IO;
using System.Net;
using System.Drawing;
using System.Security.Cryptography;
using System.Net.Http;
using BP.Tools;

namespace BP.JianYu
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class AppHandler : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public AppHandler()
        {

        }

        public string ND102_SendIt()
        {
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID);
            return objs.ToMsgOfText();
        }
    }
}
