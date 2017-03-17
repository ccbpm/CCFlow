using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;

namespace BP.WF.HttpHandler
{
    public class WF : WebContralBase
    {
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="mycontext"></param>
        public WF(HttpContext mycontext)
        {
            this.context = mycontext;
        }
         
    }
}
