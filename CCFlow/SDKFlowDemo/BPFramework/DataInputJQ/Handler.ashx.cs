using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCFlow.SDKFlowDemo.BPFramework.DataInputJQ
{
    /// <summary>
    /// Handler 的摘要说明
    /// </summary>
    public class Handler : IHttpHandler
    {
        #region 执行.
        public HttpContext context=null;
        /// <summary>
        /// 执行类型
        /// </summary>
        public string DoType
        {
            get
            {
                string str = context.Request.QueryString["DoType"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 主键
        /// </summary>
        public string MyPK
        {
            get
            {
                string str = context.Request.QueryString["MyPK"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 编号
        /// </summary>
        public string No
        {
            get
            {
                string str = context.Request.QueryString["No"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        #endregion 执行.

        public void ProcessRequest(HttpContext mycontext)
        {

            context = mycontext;
            string msg = "";
            try
            {
                switch (this.DoType)
                {
                    case "StudentInit": //初始化实体demo.
                        msg = this.StudentInit();
                        break;
                    case "StudentSave": //保存实体demo.
                        msg = this.StudentSave();
                        break;
                    case "StudentDelete":
                        msg = this.StudentDelete();
                        break;
                    default:
                        msg = "err@没有判断的标记:" + this.DoType;
                        break;
                }
            }
            catch (Exception ex)
            {
                msg = "err@" + ex.Message;
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(msg);
        }
        /// <summary>
        /// 实体保存
        /// </summary>
        /// <returns></returns>
        public string StudentSave()
        {
            BP.Demo.BPFramework.Student stu = new BP.Demo.BPFramework.Student();
            if (this.No != null)
            {
                stu.No = this.No;
                stu.Retrieve();
            }

            stu = BP.Sys.PubClass.CopyFromRequestByPost(stu, context.Request) as BP.Demo.BPFramework.Student;
            stu.Save();  //执行保存.
            
            return "编号["+stu.No+"]名称["+stu.Name+"]保存成功...";
        }
        /// <summary>
        /// 初始化学生信息.
        /// </summary>
        /// <returns></returns>
        public string StudentInit()
        {
            BP.Demo.BPFramework.Student en = new BP.Demo.BPFramework.Student();
            if (this.No ==null)
            {
                en.No = en.GenerNewNo;
            }
            else
            {
                en.No = this.No;
                en.Retrieve();
            }
            return en.ToJson();
        }
        /// <summary>
        /// 实体保存
        /// </summary>
        /// <returns></returns>
        public string StudentDelete()
        {
            BP.Demo.BPFramework.Student stu = new BP.Demo.BPFramework.Student();
            if (this.No != null)
            {
                stu.No = this.No;
                stu.Delete();
                return "删除成功...";
            }
            else
            {
                return "err@删除失败...";
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