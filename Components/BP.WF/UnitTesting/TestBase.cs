using System;
using System.Threading;
using System.Collections;
using System.Data;
using BP.DA;
using BP.DTS;
using BP.En;
using BP.Web.Controls;
using BP.Web;

namespace BP.UnitTesting
{
    public enum EditState
    {
        /// <summary>
        /// 已经完成
        /// </summary>
        Passed,
        /// <summary>
        /// 编辑中
        /// </summary>
        Editing,
        /// <summary>
        /// 未完成
        /// </summary>
        UnOK
    }
	/// <summary>
	/// 测试基类
	/// </summary>
    abstract public class TestBase
    {
        public EditState EditState = EditState.Editing;
        /// <summary>
        /// 执行步骤信息
        /// </summary>
        public int TestStep = 0;
        public string Note = "";
        /// <summary>
        /// 增加测试内容.
        /// </summary>
        /// <param name="note">测试内容的详细描述.</param>
        public void AddNote(string note)
        {
            TestStep++;
            if (Note == "")
            {
                Note += "\t\n 进行:" + TestStep + "项测试";
                Note += "\t\n" + note;
            }
            else
            {
                Note += "\t\n测试通过.";
                Note += "\t\n 进行:" + TestStep + "项测试";
                Note += "\t\n" + note;
            }
        }
        public string sql = "";
        public DataTable dt = null;
        /// <summary>
        /// 让子类重写
        /// </summary>
        public virtual void Do()
        {
        }

        #region 基本属性.
        /// <summary>
        /// 标题
        /// </summary>
        public string Title = "未命名的单元测试";
        public string DescIt = "描述";
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrInfo = "";
        #endregion
        /// <summary>
        /// 测试基类
        /// </summary>
        public TestBase() { }
    }

}
