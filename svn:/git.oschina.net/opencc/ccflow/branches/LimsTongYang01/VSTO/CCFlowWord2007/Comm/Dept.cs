using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace BP.Port
{
    public class Depts : System.Collections.CollectionBase
    {
        /// <summary>
        /// 部门集合
        /// </summary>
        public Depts()
        {
        }
    }
    public class Dept
    {
        public Dept()
        {
        }
        public Dept(DataTable dt, int rowIdx)
        {
            this.No = dt.Rows[rowIdx]["No"].ToString();
            this.Name = dt.Rows[rowIdx]["Name"].ToString();
            this.FtpUser = dt.Rows[rowIdx]["FtpUser"].ToString();
            this.FtpPortNum = dt.Rows[rowIdx]["FtpPortNum"].ToString();
            this.FtpIP = dt.Rows[rowIdx]["FtpIP"].ToString();
            this.FtpPass = dt.Rows[rowIdx]["FtpPass"].ToString();
            this.FtpPath = dt.Rows[rowIdx]["FtpPath"].ToString();
        }

        public string No = "";
        public string Name = "";
        public string FtpIP = "";
        public string FtpPath = "";
        public string FtpPass = "";
        public string FtpPortNum = "";
        public string FtpUser = "";
        public string PathOfPriTempUser
        {
            get
            {
                return this.FtpPath + "/Pri.Temp/" + BP.Web.WebUser.No + "/";
            }
        }
    }
}
