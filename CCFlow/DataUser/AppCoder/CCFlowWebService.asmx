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
    public bool CheckUserPass(string userNo, string strpw)
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
        if (emp.Pass == strpw)
            return true; /*密码正确：这里是用明文密码做的校验，您可以修改成自己的加密方式*/
        return false;
        #endregion 校验密码.

    }
}