﻿using System;
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
                    case "Login_Submit":

                        string userNo = mycontext.Request.Form["TB_UserNo"];
                        string pass = mycontext.Request.Form["TB_Pass"];

                        string sql = "SELECT pass FROM Port_Emp WHERE No='" + userNo + "'";
                        string val = BP.DA.DBAccess.RunSQLReturnString(sql);

                        if (val != pass)
                            msg = "err@密码错误.";
                        else
                            msg = "登录成功.";

                        break;
                    case "Student_Init": //初始化实体demo.
                        msg = this.Student_Init();
                        break;
                    case "Student_Save": //保存实体demo.
                        msg = this.Student_Save();
                        break;
                    case "Student_Delete": //删除.
                        msg = this.Student_Delete();
                        break;
                    case "StudentList_Init": //获取学生列表。
                        msg = this.StudentList_Init();
                        break;
                    case "StudentList_Delete": //删除单个学生.
                        msg = this.StudentList_Delete();
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


        #region 学生列表的操作.
        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <returns></returns>
        public string StudentList_Init()
        {
            BP.Demo.BPFramework.Students ens = new BP.Demo.BPFramework.Students();
            ens.RetrieveAll();
            return ens.ToJson();
        }
        public string StudentList_Delete()
        {
            try
            {
                BP.Demo.BPFramework.Student stu = new BP.Demo.BPFramework.Student();
                stu.No = this.No;
                stu.Delete();
                return "删除成功";

            }catch(Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        #endregion 学生列表的操作.


        #region 学生实体 操作.
        /// <summary>
        /// 实体保存
        /// </summary>
        /// <returns></returns>
        public string Student_Save()
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
        public string Student_Init()
        {
            BP.Demo.BPFramework.Student en = new BP.Demo.BPFramework.Student();
            if (this.No ==null)
            {
                en.No = en.GenerNewNo;
                en.Age = 23;
                en.Addr = "山东.济南.";

            }
            else
            {
                en.No = this.No;
                en.Retrieve();
            }
            return en.ToJson();
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public string Student_Delete()
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
        #endregion 学生实体 操作.


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}