using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Web;
using BP.WF;
using BP.Web;
using BP.Sys;
using BP.DA;
using BP.En;

namespace CCFlow.WF.Admin.CCFormDesigner
{
	/// <summary>
	/// Handler 的摘要说明
	/// </summary>
	public class Handler : IHttpHandler
	{
        public void ProcessRequest(HttpContext mycontext)
        {
            //创建 contral 对象.
            BP.WF.WebContral.WF_Admin_CCFormDesigner ctrl =
                new BP.WF.WebContral.WF_Admin_CCFormDesigner(mycontext);
            try
            {
                //获得执行的方法.
                string doType = mycontext.Request.QueryString["DoType"];

                //执行方法返回json.
                string msg = ctrl.DoMethod(ctrl, doType); 
                
                //返回执行的结果.
                mycontext.Response.Write(msg);
            }
            catch (Exception ex)
            {
                mycontext.Response.Write("err@" + ex.Message);
            }
        }
		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}