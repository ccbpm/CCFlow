using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

/// <summary>
///Event 的摘要说明
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
//若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。 
// [System.Web.Script.Services.ScriptService]
public class Event : System.Web.Services.WebService {

    public Event () {

        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string HelloWorld() {
        return "Hello World";
    }

    [WebMethod]
    public string DoEvent(int fk_node, int workid, string eventType, string paras)
    {
        switch (fk_node)
        {
            case 102:
                this.Do102(  workid,   eventType,   paras);
                break;
            default:
                break;
        }
        return "Hello World";
    }

    public string Do102(int workid, string eventType, string paras)
    {

        return null;
    }

    
      
}
