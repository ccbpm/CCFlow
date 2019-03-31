using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BP.Web;

namespace BP.En30.Services
{
    interface IUserService
    {
        WebUser GetUserInfo(string no);


        List<WebUser> GetUserInfo();
    }
}
