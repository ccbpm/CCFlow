<%@ WebService Language="C#" Class="DataInterface" %>

using System;
using System.Web;
using System.Data;
using System.Web.Services;
using System.Web.Services.Protocols;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
//若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。 
// [System.Web.Script.Services.ScriptService]
public class DataInterface  : System.Web.Services.WebService {

    [WebMethod]
    public string HelloWorld() {
        return "Hello World";
    }
    /// <summary>
    /// 执行SQL
    /// </summary>
    /// <param name="sql"></param>
    /// <returns></returns>
    public static int RunSQL(string sql)
    {
        //throw new Exception("您需要重写此方法以让ccflow调用。");
       // return BP.DA.DBAccess.RunSQL(sql);
        return 0;
    }
    /// <summary>
    /// 运行sql返回datatable.
    /// </summary>
    /// <param name="sql"></param>
    /// <returns></returns>
    public static DataTable RunSQLReturnTable(string sql)
    {
        //throw new Exception("您需要重写此方法以让ccflow调用。");
        //return BP.DA.DBAccess.RunSQLReturnTable(sql);
        return new DataTable();
    }
}