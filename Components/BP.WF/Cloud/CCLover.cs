
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System;
using System.Web;
using System.Data;
using BP.En;
using BP.DA;
using System.Configuration;
using BP.Port;
using BP.Pub;
using BP.Sys;


namespace BP.WF.Cloud
{
    /// <summary>
    /// 云用户的用户信息。
    /// </summary>
    public class CCLover
    {
        #region 内存的用户与密码.
        /// <summary>
        /// 获得当前用户.
        /// </summary>
        public static string UserNo
        {
            get
            {
                return BP.Sys.GloVars.GetValByKey("CCLoverNo", null);
            }
            set
            {
                BP.Sys.GloVars.SetValByKey("CCLoverNo", value);
            }
        }
        /// <summary>
        /// 密码
        /// </summary>
        public static string Password
        {
            get
            {
                return BP.Sys.GloVars.GetValByKey("CCLoverPassword", null);
            }
            set
            {
                BP.Sys.GloVars.SetValByKey("CCLoverPassword", value);
            }
        }
        /// <summary>
        /// GUID
        /// </summary>
        public static string GUID
        {
            get
            {
                return BP.Sys.GloVars.GetValByKey("CCLoverGUID", null);
            }
            set
            {
                BP.Sys.GloVars.SetValByKey("CCLoverGUID", value);
            }
        }
        #endregion 内存的用户与密码.

    }
}
