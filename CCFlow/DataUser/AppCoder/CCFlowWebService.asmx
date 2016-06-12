<%@ WebService Language="C#" Class="CCFlowWebService" %>
using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

/*
 说明：此类的方法是用于用户重写，并实现自己的业务逻辑。
 */

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
//若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。 
// [System.Web.Script.Services.ScriptService]
public class CCFlowWebService : System.Web.Services.WebService
{
    /// <summary>
    /// 方法用途:检查用户名与密码是否正确.
    /// 调用位置: \WF\UC\Login.ascx.cs
    /// 实现的意义:重写这个方法的意义是您可以修改实现逻辑，实现您自己的校验方式。
    /// 比如：
    /// 1，用户不存在怎么办？
    /// 2，用户岗位不全怎么办？
    /// 3, 用户没有部门或者部门编号不对怎么办？
    /// 4，需要把密码进行二次加密怎么处理？
    /// </summary>
    /// <param name="userNo"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public bool CheckUserPass(string userNo, string password)
    {
        BP.Port.Emp emp = new BP.Port.Emp();
        emp.No = userNo;
        if (emp.RetrieveFromDBSources() == 0)
            return false; /*不存在*/

        #region 校验用户是否被禁用了。
        /* 检查用户是否被禁用*/
        BP.WF.Port.WFEmp wfemp = new BP.WF.Port.WFEmp();
        wfemp.No = emp.No;
        if (wfemp.RetrieveFromDBSources() == 0)
        {
            wfemp.UseSta = 1;
            wfemp.Copy(emp);
            wfemp.Insert();
        }
        else
        {
            if (wfemp.UseSta == 0)
                throw new Exception("改用户已经被禁用");
        }
        #endregion 校验用户是否被禁用了。

        #region 校验密码.
        if (emp.Pass == password)
            return true; /*密码正确：这里是用明文密码做的校验，您可以修改成自己的加密方式*/
        return false;
        #endregion 校验密码.

    }
}