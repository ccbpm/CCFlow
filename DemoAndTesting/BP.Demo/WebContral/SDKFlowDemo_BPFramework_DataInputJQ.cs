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


namespace BP.Demo.HandlerPage
{
    public class SDKFlowDemo_BPFramework_DataInputJQ : BP.WF.HttpHandler.WebContralBase
    {
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="mycontext"></param>
        public SDKFlowDemo_BPFramework_DataInputJQ(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        #region 学生列表的操作.
        /// <summary>
        /// 学生主页信息
        /// </summary>
        /// <returns></returns>
        public string Home_Init()
        {
            Hashtable ht = new Hashtable();
            ht.Add("NanSheng", 110);
            ht.Add("NvSheng", 100);
            ht.Add("SumNum", 210);
            return BP.DA.DataType.ToJsonEntityModel(ht);
        }
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
            }
            catch (Exception ex)
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

            return "编号[" + stu.No + "]名称[" + stu.Name + "]保存成功...";
        }
        /// <summary>
        /// 初始化学生信息.
        /// </summary>
        /// <returns></returns>
        public string Student_Init()
        {
            BP.Demo.BPFramework.Student en = new BP.Demo.BPFramework.Student();
            if (this.No == null)
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



    }
}
