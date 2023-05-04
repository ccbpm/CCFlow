using System.Data;
using BP.DA;
using BP.En;
namespace BP.WF.DTS
{
    /// <summary>
    /// 升级ccflow6 要执行的调度
    /// </summary>
    public class UpdateEmpDeptNo : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public UpdateEmpDeptNo()
        {
            this.Title = "从Port_DeptEmp表里,随机抽出来一个部门编号给Port_Emp的FK_Dept。";
            this.Help = "宁波项目:人员表里没有主部门.";
        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
        }
        /// <summary>
        /// 当前的操纵员是否可以执行这个方法
        /// </summary>
        public override bool IsCanDo
        {
            get
            {
                if (BP.Web.WebUser.No.Equals("admin")==true)
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {
            string sql = "";
           
            return  "执行完成.";
        }
    }
}
