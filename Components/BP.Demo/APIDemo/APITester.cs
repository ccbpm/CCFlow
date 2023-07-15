using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanYiSoft
{
    public class APITester
    {
        #region 门户 - 登陆测试.
        public static void Test_Port_Login()
        {
            string data = QuanYiSoft.ClientAPISAAS.Port_Login("UserNo");
        }
        #endregion 登陆测试
    }
}
